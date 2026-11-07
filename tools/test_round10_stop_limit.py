"""The explicit user cap permits D35 and prevents D36 before any target work."""
import copy
import json
from pathlib import Path
import unittest
from round10_controller import GuardError, stop_reason, validate_cycle

ROOT = Path(__file__).resolve().parents[1]


class UserStopLimit(unittest.TestCase):
    def setUp(self):
        self.cfg = json.loads((ROOT / 'config/round10-protocol.json').read_text())
        self.journal = {'cycles': [{'phase': 'C'}] + [
            {'phase': 'D', 'assembly': 'KairoLibrary', 'primary_target': str(i),
             'methods_gained': 1, 'queue_empty_before': True, 'queue_empty': True}
            for i in range(34)]}

    def test_d35_allowed(self):
        self.assertIsNone(stop_reason(self.cfg, self.journal))
        self.assertEqual('cycle35', validate_cycle(self.cfg, self.journal, {
            'phase': 'D', 'assembly': 'Assembly-CSharp',
            'primary_target': 'fresh', 'number': 36}))

    def test_d36_rejected(self):
        self.journal['cycles'].append(dict(self.journal['cycles'][-1]))
        self.assertEqual('maximum_35_D_cycles', stop_reason(self.cfg, self.journal))
        with self.assertRaises(GuardError):
            validate_cycle(self.cfg, self.journal, {'phase': 'D',
                'assembly': 'KairoLibrary', 'primary_target': 'fresh', 'number': 37})

    def test_unrecorded_cap_rejected(self):
        bad = copy.deepcopy(self.cfg)
        del bad['user_stop_limit_override']
        with self.assertRaises(GuardError):
            validate_cycle(bad, self.journal, {'phase': 'D',
                'assembly': 'KairoLibrary', 'primary_target': 'fresh', 'number': 36})


if __name__ == '__main__':
    unittest.main()
