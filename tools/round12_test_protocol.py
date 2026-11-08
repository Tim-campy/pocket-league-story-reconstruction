"""Pure guards: no game compilation, verifier, or ledger writes."""
import copy
import json
from pathlib import Path
import sys
import tempfile
import unittest
from unittest.mock import patch

from round12_controller import (BASE_COMMIT, GuardError, measured_zero_streak,
    selection_mode, stop_reason, validate_config, validate_cycle, verify_raw_gate,
    current_measurement, file_hash)
from round12_postverify_provenance import reconcile, METRICS
from round12_progress_status import kairo_completion,app_completion

ROOT = Path(__file__).resolve().parents[1]


class Protocol(unittest.TestCase):
    def setUp(self):
        self.cfg = json.loads((ROOT / 'config/round12-protocol.json').read_text())
        self.cfg['phase'] = 'part_A'
        self.cfg['part_a_raw_gate'] = {'status': 'NOT_EVALUATED'}
        self.cfg['prepared_queue'] = []
        self.cfg.pop('requested_stop', None)
        self.cfg.pop('raw_source_current_measurement', None)
        self.cfg.pop('verification_current_measurement', None)
        self.cfg.pop('verification_origin_release', None)
        self.journal = {'cycles': []}
        self.measurement = {'assemblies': {'KairoLibrary': {'divergent_bodies': 13,
                                                          'equal_body_held_methods': 2}}}
        self.request = dict(number=1, phase='A', assembly='KairoLibrary',
                            primary_target='fixture::Method', queue_ids=[])

    def row(self, phase='B', gain=0, before=True, after=True, primary='other'):
        return dict(phase=phase, methods_gained=gain, queue_empty_before=before,
                    queue_empty=after, primary_target=primary)

    def test_constants_and_baseline(self):
        validate_config(self.cfg)
        self.assertEqual(self.cfg['baseline_commit'], BASE_COMMIT)

    def test_A_unlimited_no_five_zero_stop(self):
        self.journal['cycles'] = [self.row('A') for _ in range(80)]
        self.request['number'] = 81
        self.assertIsNone(stop_reason(self.cfg, self.journal))
        self.assertEqual(validate_cycle(self.cfg, self.journal, self.request, self.measurement), 'a81')

    def test_B_cap_40_rejects_41(self):
        self.journal['cycles'] = [self.row(gain=1) for _ in range(40)]
        self.assertEqual(stop_reason(self.cfg, self.journal), 'maximum_40_Part_B_cycles')

    def test_39_B_not_limit(self):
        self.journal['cycles'] = [self.row(gain=1) for _ in range(39)]
        self.assertIsNone(stop_reason(self.cfg, self.journal))

    def test_only_B_counted(self):
        self.journal['cycles'] = [self.row('A') for _ in range(50)] + [self.row(gain=1)]
        self.assertIsNone(stop_reason(self.cfg, self.journal))

    def test_five_empty_B_zeros(self):
        self.journal['cycles'] = [self.row() for _ in range(5)]
        self.assertEqual(measured_zero_streak(self.journal), 5)
        self.assertIsNotNone(stop_reason(self.cfg, self.journal))

    def test_pending_B_queue_blocks_zero_stop(self):
        self.journal['cycles'] = [self.row() for _ in range(5)]
        self.cfg['prepared_queue'] = [dict(status='pending', phase='B')]
        self.assertIsNone(stop_reason(self.cfg, self.journal))

    def test_queue_not_empty_before_breaks_streak(self):
        self.journal['cycles'] = [self.row(before=False)] + [self.row() for _ in range(4)]
        self.assertEqual(measured_zero_streak(self.journal), 4)

    def test_no_A_limit_in_config(self):
        self.cfg['part_a_count_limit'] = 40
        with self.assertRaises(GuardError): validate_config(self.cfg)

    def test_A_rejects_App(self):
        self.request['assembly'] = 'Assembly-CSharp'
        with self.assertRaises(GuardError): validate_cycle(self.cfg, self.journal, self.request, self.measurement)

    def test_third_same_primary_rejected(self):
        self.journal['cycles'] = [self.row('A', primary=self.request['primary_target']) for _ in range(2)]
        self.request['number'] = 3
        with self.assertRaises(GuardError): validate_cycle(self.cfg, self.journal, self.request, self.measurement)

    def test_main_subset_rejected_both_parts(self):
        for phase in ('A', 'B'):
            self.request.update(phase=phase, target_substring=['one'])
            with self.assertRaises(GuardError): validate_cycle(self.cfg, self.journal, self.request, self.measurement)

    def test_held_blocks_semantic_closure(self):
        self.measurement['assemblies']['KairoLibrary']['divergent_bodies'] = 0
        with self.assertRaises(GuardError): verify_raw_gate(self.cfg, self.measurement)

    def test_semantic_zero_hands_off_before_new_A_build(self):
        self.measurement['assemblies']['KairoLibrary'].update(divergent_bodies=0, equal_body_held_methods=0)
        with self.assertRaisesRegex(GuardError, 'hand off'):
            validate_cycle(self.cfg, self.journal, self.request, self.measurement)

    def test_zero_bodies_does_not_imply_raw_PASS(self):
        self.measurement['assemblies']['KairoLibrary'].update(divergent_bodies=0, equal_body_held_methods=0)
        with self.assertRaisesRegex(GuardError, 'raw comparison PASS'):
            verify_raw_gate(self.cfg, self.measurement)

    def test_B_requires_raw_gate(self):
        self.request.update(phase='B', assembly='Assembly-CSharp')
        with self.assertRaises(GuardError): validate_cycle(self.cfg, self.journal, self.request, self.measurement)

    def test_initial_queue_order(self):
        self.cfg['prepared_queue'] = [dict(id='first', phase='A', assembly='KairoLibrary',
            status='pending', selection_mode='queue_initial', method_ids=['fixture::First'])]
        with self.assertRaises(GuardError): selection_mode(self.cfg, self.request, self.measurement)

    def test_duplicate_journal_number_rejected(self):
        self.request['number'] = 2
        with self.assertRaises(GuardError): validate_cycle(self.cfg, self.journal, self.request, self.measurement)

    def test_user_stop_overrides(self):
        self.cfg['requested_stop'] = dict(source='user', reason='explicit request')
        self.assertEqual(stop_reason(self.cfg, self.journal), 'explicit_user_stop')

    def provenance_fixture(self):
        before = {'assemblies': {'KairoLibrary': dict(zip(METRICS, [10, 100, 3, 30, 2]))}}
        after = {'assemblies': {'KairoLibrary': dict(zip(METRICS, [11, 110, 2, 20, 2]))}}
        row = dict(after_measurement='old.json', after_sha256='old-sha',
                   gains={'KairoLibrary': dict(methods_gained=1, il_bytes_gained=10)})
        return before, after, row

    def test_postverify_preserves_gains_and_old_provenance(self):
        before, after, row = self.provenance_fixture()
        result = reconcile(row, before, after, copy.deepcopy(after), 'new.json', 'new-sha')
        self.assertEqual(result['gains'], row['gains'])
        self.assertEqual(result['after_measurement'], 'new.json')
        self.assertEqual(result['postverify_provenance'][0]['previous_after_measurement'], 'old.json')
        self.assertEqual(row['after_measurement'], 'old.json')

    def test_postverify_rejects_changed_methods(self):
        before, after, row = self.provenance_fixture()
        changed = copy.deepcopy(after)
        changed['assemblies']['KairoLibrary']['matched_methods'] += 1
        with self.assertRaises(ValueError): reconcile(row, before, after, changed, 'new.json', 'sha')

    def test_postverify_rejects_changed_held_count(self):
        before, after, row = self.provenance_fixture()
        changed = copy.deepcopy(after)
        changed['assemblies']['KairoLibrary']['equal_body_held_methods'] = 0
        with self.assertRaises(ValueError): reconcile(row, before, after, changed, 'new.json', 'sha')

    def test_postverify_rejects_inflated_gain(self):
        before, after, row = self.provenance_fixture()
        row['gains']['KairoLibrary']['methods_gained'] = 2
        with self.assertRaises(ValueError): reconcile(row, before, after, after, 'new.json', 'sha')

    def test_raw_source_measurement_precedes_old_A11_until_B_starts(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory); (root/'reports').mkdir()
            for name in ['A', 'metadata', 'B']:
                (root/(name+'.json')).write_text(json.dumps(dict(baseline_commit=BASE_COMMIT, marker=name)))
            (root/'reports/round12-raw-source-journal.json').write_text(json.dumps(dict(commit_provenance=[
                dict(measurement='metadata.json',measurement_sha256=file_hash(root/'metadata.json'))])))
            journal = dict(cycles=[dict(phase='A',after_measurement='A.json',after_sha256=file_hash(root/'A.json'))])
            cfg = dict(self.cfg,raw_source_current_measurement=dict(path='metadata.json',sha256=file_hash(root/'metadata.json')))
            with patch('round12_controller.ROOT', root):
                self.assertEqual(current_measurement(cfg,journal)['marker'], 'metadata')
                journal['cycles'].append(dict(phase='B',after_measurement='B.json',after_sha256=file_hash(root/'B.json')))
                self.assertEqual(current_measurement(cfg,journal)['marker'], 'B')

    def test_raw_source_measurement_hash_failure_never_falls_back_to_A11(self):
        with tempfile.TemporaryDirectory() as directory:
            root=Path(directory); (root/'reports').mkdir()
            (root/'metadata.json').write_text(json.dumps(dict(baseline_commit=BASE_COMMIT)))
            (root/'reports/round12-raw-source-journal.json').write_text(json.dumps(dict(commit_provenance=[
                dict(measurement='metadata.json',measurement_sha256='incorrect')])))
            cfg = dict(self.cfg,raw_source_current_measurement=dict(path='metadata.json',sha256='incorrect'))
            with patch('round12_controller.ROOT', root):
                with self.assertRaises(GuardError): current_measurement(cfg,dict(cycles=[]))

    def test_valid_old_A11_hash_is_not_a_current_metadata_pointer(self):
        with tempfile.TemporaryDirectory() as directory:
            root=Path(directory); (root/'reports').mkdir()
            for name in ('old','fresh'):
                (root/(name+'.json')).write_text(json.dumps(dict(baseline_commit=BASE_COMMIT)))
            (root/'reports/round12-raw-source-journal.json').write_text(json.dumps(dict(commit_provenance=[
                dict(measurement='fresh.json',measurement_sha256=file_hash(root/'fresh.json'))])))
            cfg=dict(self.cfg,raw_source_current_measurement=dict(path='old.json',sha256=file_hash(root/'old.json')))
            with patch('round12_controller.ROOT',root):
                with self.assertRaises(GuardError): current_measurement(cfg,dict(cycles=[]))

    def test_progress_stale_PE_never_claims_complete_or_blocks_verifier(self):
        with tempfile.TemporaryDirectory() as directory:
            root=Path(directory); (root/'config').mkdir(); (root/'reports').mkdir()
            (root/'config/round12-protocol.json').write_text(json.dumps(dict(part_a_raw_gate=dict(status='PASS'))))
            (root/'reports/round12-cycle-journal.json').write_text(json.dumps(dict(cycles=[])))
            with patch('round12_controller.current_measurement',return_value={}), patch('round12_controller.verify_raw_gate',side_effect=GuardError('canonical SHA changed')):
                result=kairo_completion(root)
            self.assertFalse(result['complete'])
            self.assertEqual(result['status'],'RAW_RENEWAL_REQUIRED')

    def test_app_badge_rejects_stale_current_source_proof(self):
        with tempfile.TemporaryDirectory() as directory:
            root=Path(directory);(root/'config').mkdir();(root/'reports').mkdir()
            (root/'config/round12-protocol.json').write_text(json.dumps(dict(app_raw_measurements=[dict(status='PASS')],)))
            (root/'reports/round12-cycle-journal.json').write_text(json.dumps(dict(cycles=[])))
            with patch('round12_controller.current_measurement',side_effect=GuardError('source changed')):
                result=app_completion(root)
            self.assertFalse(result['complete'])
            self.assertEqual(result['status'],'RAW_RENEWAL_REQUIRED')

    def test_app_badge_does_not_promote_pending_raw_result(self):
        with tempfile.TemporaryDirectory() as directory:
            root=Path(directory);(root/'config').mkdir()
            (root/'config/round12-protocol.json').write_text(json.dumps(dict(app_raw_measurements=[dict(status='DIFFERENT')],)))
            self.assertFalse(app_completion(root)['complete'])


if __name__ == '__main__':
    unittest.main()
