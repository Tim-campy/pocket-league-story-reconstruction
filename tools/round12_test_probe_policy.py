import unittest
from round12_cycle import probe_should_stop


class ProbePolicyTests(unittest.TestCase):
    def test_default_keeps_existing_first_success_policy(self):
        self.assertTrue(probe_should_stop(True))
        self.assertFalse(probe_should_stop(False))

    def test_explicit_all_retains_each_result_even_after_success(self):
        self.assertFalse(probe_should_stop(True, True))
        self.assertFalse(probe_should_stop(False, True))


if __name__ == '__main__':
    unittest.main()
