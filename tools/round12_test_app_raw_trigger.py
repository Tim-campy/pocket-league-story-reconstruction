import unittest
from unittest.mock import patch
from round12_app_raw_trigger import due, measure_if_due


class TriggerTests(unittest.TestCase):
    def sample(self, bodies, held):
        return {'assemblies': {'Assembly-CSharp': dict(divergent_bodies=bodies, equal_body_held_methods=held)}}

    def test_threshold_includes_held(self):
        self.assertFalse(due(self.sample(10, 1)))
        self.assertTrue(due(self.sample(9, 1)))

    def test_zero_remains_measured(self):
        self.assertTrue(due(self.sample(0, 0)))

    def test_above_threshold_never_invokes_comparer(self):
        with patch('round12_app_raw_trigger.subprocess.run') as run:
            self.assertIsNone(measure_if_due(self.sample(11, 0), 'unused', 'unused'))
            run.assert_not_called()


if __name__ == '__main__':
    unittest.main()
