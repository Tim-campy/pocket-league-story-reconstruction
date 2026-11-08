"""Pure orchestration tests: no target build, model execution, verifier or ledger writes."""
import copy
import json
from pathlib import Path
import unittest
from unittest.mock import patch
import round11_controller as controller
import round11_report as report
from round11_parts import validate_zero_gain
from round11_raw_gate import due_events
from round10_model_policy import prediction_allows_compilation

ROOT = Path(__file__).resolve().parents[1]


class Protocol(unittest.TestCase):
    def setUp(self):
        self.cfg = json.loads((ROOT / 'config/round11-protocol.json').read_text())
        self.cfg['required_parts'] = dict(A='complete', B='complete', C='complete')
        self.cfg['phase'] = 'part_C'
        for front in self.cfg['c_fronts'].values():
            front['status'] = 'verified'
        self.measurement = {'assemblies': {'KairoLibrary': {'divergent_bodies': 24},
                                          'Assembly-CSharp': {'divergent_bodies': 65}}}
        self.journal = {'cycles': [{'phase': 'C', 'label': 'c01', 'methods_gained': 0}]}

    def empty_queue(self):
        for item in self.cfg['prepared_queue']:
            item['status'] = 'tested'

    def request(self, **overrides):
        row = dict(number=len(self.journal['cycles']) + 1, phase='D',
                   assembly='KairoLibrary', primary_target='new', queue_ids=[])
        row.update(overrides)
        return row

    def append_d(self, count, gain=1):
        self.journal['cycles'] += [dict(phase='D', primary_target=str(i), methods_gained=gain,
                                       queue_empty_before=True, queue_empty=True) for i in range(count)]

    def test_initial_app_is_explicit_priority_exception(self):
        first = self.cfg['prepared_queue'][0]
        request = self.request(assembly=first['assembly'], primary_target=first['method_ids'][0], queue_ids=[first['id']])
        self.assertEqual('cycle01', controller.validate_cycle(self.cfg, self.journal, request, self.measurement))
        self.assertEqual('queue_initial', controller.selection_mode(self.cfg, request, self.measurement))

    def test_initial_queue_order_is_required(self):
        second = self.cfg['prepared_queue'][1]
        with self.assertRaises(controller.GuardError):
            controller.validate_cycle(self.cfg, self.journal, self.request(
                primary_target=second['method_ids'][0], queue_ids=[second['id']]), self.measurement)

    def test_discovery_cannot_skip_initial_queue(self):
        with self.assertRaises(controller.GuardError):
            controller.validate_cycle(self.cfg, self.journal, self.request(), self.measurement)

    def test_autonomous_app_waits_for_kairo(self):
        self.empty_queue()
        with self.assertRaises(controller.GuardError):
            controller.validate_cycle(self.cfg, self.journal, self.request(assembly='Assembly-CSharp'), self.measurement)

    def test_autonomous_app_allowed_after_kairo_body_closure(self):
        self.empty_queue()
        self.measurement['assemblies']['KairoLibrary']['divergent_bodies'] = 0
        self.assertEqual('cycle01', controller.validate_cycle(self.cfg, self.journal,
            self.request(assembly='Assembly-CSharp'), self.measurement))

    def test_d40_allowed_d41_rejected(self):
        self.empty_queue()
        self.append_d(39)
        self.assertEqual('cycle40', controller.validate_cycle(self.cfg, self.journal, self.request(), self.measurement))
        self.append_d(1)
        with self.assertRaises(controller.GuardError):
            controller.validate_cycle(self.cfg, self.journal, self.request(), self.measurement)

    def test_c_zero_does_not_count_toward_zero_stop(self):
        self.empty_queue()
        self.append_d(4, gain=0)
        self.assertIsNone(controller.stop_reason(self.cfg, self.journal))
        self.assertEqual(4, controller.measured_zero_streak(self.journal))

    def test_five_empty_queue_zeros_stop(self):
        self.empty_queue()
        self.append_d(5, gain=0)
        self.assertEqual('five_zero_D_cycles_with_queue_empty_before_and_after', controller.stop_reason(self.cfg, self.journal))

    def test_queue_consumption_zero_is_not_empty_before(self):
        self.empty_queue()
        self.append_d(5, gain=0)
        self.journal['cycles'][1]['queue_empty_before'] = False
        self.assertEqual(4, controller.measured_zero_streak(self.journal))
        self.assertIsNone(controller.stop_reason(self.cfg, self.journal))

    def test_same_primary_third_refused(self):
        self.empty_queue()
        self.append_d(2)
        for row in self.journal['cycles'][-2:]:
            row['primary_target'] = 'new'
        with self.assertRaises(controller.GuardError):
            controller.validate_cycle(self.cfg, self.journal, self.request(), self.measurement)

    def test_partial_c5_blocks_d(self):
        self.cfg['c_fronts']['C5']['status'] = 'partial'
        with self.assertRaises(controller.GuardError):
            controller.validate_cycle(self.cfg, self.journal, self.request(), self.measurement)

    def test_c_requires_front_and_does_not_consume_d(self):
        self.empty_queue()
        self.cfg['required_parts']['C'] = 'not_started'
        self.assertEqual('c02', controller.validate_cycle(self.cfg, self.journal,
            self.request(phase='C', front='C4', assembly='Assembly-CSharp')))
        with self.assertRaises(controller.GuardError):
            controller.validate_cycle(self.cfg, self.journal, self.request(phase='C'))

    def test_ab_cannot_run_target_cycles(self):
        for phase in ('A', 'B'):
            with self.assertRaises(controller.GuardError):
                controller.validate_cycle(self.cfg, self.journal, self.request(phase=phase))

    def test_ab_zero_gain(self):
        row = dict(matched_methods=10, matched_original_il_bytes=30, total_methods=20,
                   original_total_il_bytes=40, divergent_bodies=5)
        before = {'assemblies': {a: copy.deepcopy(row) for a in ('KairoLibrary', 'Assembly-CSharp')}}
        after = copy.deepcopy(before)
        validate_zero_gain(before, after)
        after['assemblies']['KairoLibrary']['matched_methods'] += 1
        with self.assertRaises(ValueError):
            validate_zero_gain(before, after)

    def test_raw_thresholds_and_once_only(self):
        self.assertEqual([], due_events(self.cfg, self.measurement))
        self.measurement['assemblies']['KairoLibrary']['divergent_bodies'] = 0
        self.measurement['assemblies']['Assembly-CSharp']['divergent_bodies'] = 10
        self.assertEqual([('KairoLibrary', 0), ('Assembly-CSharp', 10)], due_events(self.cfg, self.measurement))
        self.cfg['raw_evaluations'] = {'KairoLibrary': {}, 'Assembly-CSharp': {}}
        self.assertEqual([], due_events(self.cfg, self.measurement))

    def test_incomplete_allowed_complete_wrong_refused(self):
        self.assertTrue(prediction_allows_compilation({'complete': False}, ['A']))
        self.assertFalse(prediction_allows_compilation({'complete': True, 'predicted_types': ['B']}, ['A']))

    def test_report_cannot_publish_before_stop(self):
        with patch.object(report, 'ROOT', ROOT / 'build/round11/no-stop-fixture-does-not-exist'):
            with self.assertRaises(RuntimeError):
                report.load_context()

    def test_renderer_eight_sections_and_partial_is_not_matched(self):
        # A synthetic render context tests presentation only; it is never written
        # as a Round11 result and does not claim any model or target validation.
        measured = json.loads((ROOT / 'reports/round10-final-measurement.json').read_text())
        selected = json.loads((ROOT / 'config/toolchain.json').read_text())
        cfg = copy.deepcopy(self.cfg)
        cfg['part_evidence'] = {'A': {'report': 'A.json', 'result': 'PDB path measured'},
                                'B': {'report': 'B.json', 'result': 'DEFLATE measured'}}
        for front in cfg['c_fronts'].values():
            front['attempts'] = [{'label': 'c01'}]
        cycle = dict(label='c01', phase='C', front='C1', assembly='KairoLibrary', pair='br => leave', hypothesis='Toy hypothesis',
                     result='INCONCLUSIVA: partial', methods_gained=0, il_bytes_gained=0)
        target = measured['assemblies']['KairoLibrary']['alignment']['opcode_histogram'][0]['method_ids'][0]
        engine = dict(assembly='KairoLibrary', methods=[dict(id=target, steps=[{}], trials=[],
            body_equal=False, initial_offset=1, final_offset=2, stop_reason='unknown')])
        manifest = dict(compiler_sha256='fixture-compiler', flags=['/optimize+', '/pdb:E:/toy.pdb'], references=[])
        prior_manifest = dict(compiler_sha256='fixture-compiler', flags=['/optimize+'], references=[])
        ctx = dict(cfg=cfg, stop=dict(reason='toy-stop', recommendations=['first', 'second']),
            baseline=measured, final=measured, journal={'cycles': [cycle]},
            manifests={a: manifest for a in report.ASSEMBLIES},
            baseline_manifests={a: prior_manifest for a in report.ASSEMBLIES},
            engines=[('toy-engine', engine)], historical=[], stable_sources={},
            models={a: dict(matched_bodies=10, matched_model_complete=6,
                           matched_complete_exact=6) for a in report.ASSEMBLIES},
            metadata={a: dict(types_equal=1, types_total=1,
                             whole_structural_metadata_equal=True) for a in report.ASSEMBLIES})
        data = {'A.json': dict(before_path_and_padding_different_bytes=97,
                    after_path_and_padding_different_bytes=0, original_pdb_path='E:/toy.pdb',
                    assembly_bytes_rewritten_after_compilation=False),
                'B.json': dict(assemblies={}, apk_built=False, target_executed=False),
                'reports/round11-hypotheses.json': {'hypotheses': []},
                'config/toolchain.json': selected,
                'build/round11/baseline/toolchain.json': selected}
        with patch.object(report, 'read', side_effect=lambda p: data[str(p)]):
            text = report.render(ctx)
        self.assertEqual(report.SECTIONS, tuple(line for line in text.splitlines() if line in report.SECTIONS))
        self.assertIn('INCONCLUSIVA', text)
        self.assertIn('corpo integral ainda diferente', text)
        self.assertIn('avanço de prefixo não é método concluído', text)
        self.assertIn('/pdb:E:/toy.pdb', text)
        self.assertIn('Compilador/opções alterados desde baseline: True', text)

    def test_report_rejects_nested_adoption_losses(self):
        import round11_report as report
        report.validate_adoption({'official_verified': True, 'assemblies': {'KairoLibrary': {'matched_losses': []}}})
        with self.assertRaises(ValueError):
            report.validate_adoption({'official_verified': True, 'assemblies': {'KairoLibrary': {'matched_losses': ['fixture-method']}}})

    def test_all_round11_imports_exist_including_late_imports(self):
        import ast
        for path in (ROOT / 'tools').glob('round11*.py'):
            for node in ast.walk(ast.parse(path.read_text(encoding='utf-8'))):
                names = ([node.module] if isinstance(node, ast.ImportFrom) else
                         [name.name for name in node.names] if isinstance(node, ast.Import) else [])
                for name in names:
                    if name and name.startswith('round11'):
                        self.assertTrue((ROOT / 'tools' / (name + '.py')).is_file(),
                                        str(path) + ':' + str(node.lineno) + ' imports ' + name)

    def test_final_validator_matches_production_model_namespace(self):
        from round11_finish import selected_smcs_validator
        cfg=copy.deepcopy(self.cfg)
        for name,expected in [('round7_slot_model','tools/round7_slot_validate.py'),
                              ('round11_slot_model','tools/round11_slot_validate.py')]:
            cfg['models']['Assembly-CSharp']['module']=name
            self.assertEqual(selected_smcs_validator(cfg),expected)
        cfg['models']['Assembly-CSharp']['module']='unregistered_model'
        with self.assertRaises(ValueError):selected_smcs_validator(cfg)


if __name__ == '__main__':
    unittest.main()
