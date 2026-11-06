"""Meaningful format/proof fixtures for the self-contained round report."""
import copy
import json
from pathlib import Path
import tempfile
import unittest

from round4_report import ASSEMBLIES, validate_evidence_counts
from round4_report_measurements import _inline, measurement_markdown


def evidence_fixture():
    baseline, final, engines, receipts, raw, ledger = {}, {}, {}, {}, {}, []
    adoption = {'official_verified': True, 'matched_losses': 0, 'matched_gains': 0, 'assemblies': {}}
    for assembly in ASSEMBLIES:
        key = assembly + '::Example::M()->System.Void'
        ledger.append({'id': key, 'assembly': assembly, 'estado': 'decompiled', 'tamanho_il': '10'})
        item = {'total_methods': 1, 'matched_methods': 0, 'original_total_il_bytes': 10,
                'matched_original_il_bytes': 0, 'divergent_bodies': 1,
                'provenance': {'build_sha256': 'a' * 64}}
        baseline[assembly] = copy.deepcopy(item)
        final[assembly] = copy.deepcopy(item)
        engines[assembly] = {'completed': True, 'adoption_eligible': True, 'body_losses': [], 'methods': [{}]}
        receipts[assembly] = {'counts': {'matched': 0}, 'build_sha256': 'a' * 64, 'original_sha256': 'b' * 64,
                              'types_equal': 1, 'types_total': 1, 'regressions': 0, 'body_regressions': [], 'type_regressions': [],
                              'methods': {key: {'state': 'decompiled', 'body_present': True, 'body_equal': False}}}
        raw[assembly] = {'rebuilt_sha256': 'a' * 64, 'original_sha256': 'b' * 64}
        adoption['assemblies'][assembly] = {'matched_losses': [], 'matched_gains': [], 'counts': {'matched': 0},
                                            'official_sha256': 'a' * 64, 'types_equal': 1}
    return baseline, final, engines, adoption, receipts, raw, ledger, copy.deepcopy(ledger)


class EvidenceGuardTests(unittest.TestCase):
    def test_consistent_completed_evidence_is_accepted(self):
        validate_evidence_counts(*evidence_fixture())

    def test_unfinished_official_verification_cannot_produce_success_report(self):
        fixture = list(evidence_fixture())
        fixture[3]['official_verified'] = False
        with self.assertRaisesRegex(ValueError, 'official verification'):
            validate_evidence_counts(*fixture)

    def test_another_rebuilt_binary_cannot_be_reported_with_old_raw_tables(self):
        fixture = list(evidence_fixture())
        fixture[5]['KairoLibrary']['rebuilt_sha256'] = 'c' * 64
        with self.assertRaisesRegex(ValueError, 'artifact hashes differ'):
            validate_evidence_counts(*fixture)

    def test_official_regression_blocks_zero_regression_prose(self):
        fixture = list(evidence_fixture())
        fixture[4]['Assembly-CSharp']['regressions'] = 1
        with self.assertRaisesRegex(ValueError, 'official regressions'):
            validate_evidence_counts(*fixture)

    def test_partial_engine_cannot_be_presented_as_processing_all_methods(self):
        fixture = list(evidence_fixture())
        fixture[2]['KairoLibrary']['methods'] = []
        with self.assertRaisesRegex(ValueError, 'every baseline divergent body'):
            validate_evidence_counts(*fixture)

    def test_forged_byte_progress_is_rejected(self):
        fixture = list(evidence_fixture())
        fixture[1]['KairoLibrary']['matched_original_il_bytes'] = 10
        with self.assertRaisesRegex(ValueError, 'matched IL differs'):
            validate_evidence_counts(*fixture)


class MarkdownFixtureTests(unittest.TestCase):
    def test_generic_names_double_backticks_and_pipe_remain_one_code_span(self):
        self.assertEqual(_inline('Vector`1::M``0|x'), '```Vector`1::M``0\\|x```')

    def test_longer_backtick_run_never_closes_its_own_fence(self):
        self.assertEqual(_inline('ldstr "a```b"'), '````ldstr "a```b"````')

    def test_complete_fragment_keeps_full_operands_and_neutral_stage_name(self):
        operand = 'class Example`1 @ Fixture, Version=1.2.3.4, Culture=neutral, PublicKeyToken=null::M``0("a|b")'
        left = {'offset': 1, 'opcode': 'call', 'operand': {'method': operand}}
        right = {'offset': 1, 'opcode': 'callvirt', 'operand': {'method': operand}}
        before = {'offset': 0, 'opcode': 'ldarg.0', 'operand': None}
        after = {'offset': 6, 'opcode': 'ret', 'operand': None}
        example = {'id': 'Fixture::Example`1::M()->System.Void', 'first_il_difference_offset': 1,
                   'first_original_instruction': left, 'first_rebuilt_instruction': right,
                   'original_context': [before, left, after], 'rebuilt_context': [before, right, after]}
        group = {'methods': 1, 'original_il_bytes': 7, 'example': example}
        document = {'ledger_sha256': 'd' * 64, 'assemblies': {'Fixture': {'compiler': 'fixture',
                    'size_distribution': {'<=64': {'methods': 1, 'original_il_bytes': 7}},
                    'divergent_bodies': 1, 'divergent_original_il_bytes': 7,
                    'first_pair_exact_histogram': [group],
                    'first_pair_opcode_histogram': [{'pair': 'call => callvirt', 'methods': 1, 'original_il_bytes': 7}],
                    'provenance': {'build_sha256': 'a' * 64, 'original_dump_sha256': 'b' * 64, 'rebuilt_dump_sha256': 'c' * 64}}}}
        with tempfile.TemporaryDirectory() as folder:
            source = Path(folder) / 'fixture.json'
            source.write_text(json.dumps(document), encoding='utf-8')
            rendered = measurement_markdown(source)
        self.assertIn('distribuição dos corpos divergentes', rendered)
        self.assertNotIn('distribuição inicial', rendered)
        self.assertIn('Version=1.2.3.4, Culture=neutral, PublicKeyToken=null::M``0', rendered)
        self.assertIn('a\\|b', rendered)
        self.assertIn('IL_0000: ldarg.0', rendered)
        self.assertIn('IL_0006: ret', rendered)
        self.assertIn('1 grupos exatos; 1 métodos', rendered)


if __name__ == '__main__':
    unittest.main()
