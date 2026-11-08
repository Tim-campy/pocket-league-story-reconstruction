"""Pure guards: no game compilation, verifier, or ledger writes."""
import copy
import json
from pathlib import Path
import sys
import tempfile
import unittest
from unittest.mock import patch

from round12_controller import (BASE_COMMIT, GuardError, measured_zero_streak,
    selection_mode, stop_reason, validate_config, validate_cycle, verify_raw_gate)

ROOT = Path(__file__).resolve().parents[1]


class Protocol(unittest.TestCase):
    def setUp(self):
        self.cfg = json.loads((ROOT / 'config/round12-protocol.json').read_text())
        self.cfg['phase'] = 'part_A'
        self.cfg['part_a_raw_gate'] = {'status': 'NOT_EVALUATED'}
        self.cfg['prepared_queue'] = []
        self.cfg.pop('requested_stop', None)
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


if __name__ == '__main__':
    unittest.main()
