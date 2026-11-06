"""Original-slot evidence must constrain alias/dead-local hypotheses."""
import unittest
from round5_engine import alias_hints, local_slot


class OriginalSlotTests(unittest.TestCase):
    def test_alias_type_comes_from_original_store_slot(self):
        original = {'locals': ['I4', 'ARRAY'], 'instructions': [
            {'offset': 0, 'opcode': 'ldarg.0', 'operand': None},
            {'offset': 1, 'opcode': 'stloc.1', 'operand': None},
            {'offset': 2, 'opcode': 'ldloc.1', 'operand': None}]}
        current = {'locals': ['I4']}
        h = alias_hints(original, current, 1, ['System.Int32', 'System.Int32[]'], {'I4': 'System.Int32'})
        self.assertEqual(h['expected_alias_types'], ['System.Int32[]'])
        self.assertEqual(h['original_unused_slots'], [{'index': 0, 'type': 'System.Int32', 'loads': 0, 'stores': 0}])
        self.assertEqual(h['rebuilt_local_types'], ['System.Int32'])

    def test_address_use_prevents_unused_classification(self):
        original = {'locals': ['I4'], 'instructions': [
            {'offset': 0, 'opcode': 'ldloca.s', 'operand': {'local': 0}}]}
        h = alias_hints(original, {'locals': []}, 0, ['System.Int32'])
        self.assertEqual(h['original_unused_slots'], [])

    def test_dead_store_is_distinct_from_no_reference(self):
        original = {'locals': ['I4'], 'instructions': [
            {'offset': 0, 'opcode': 'stloc.s', 'operand': {'local': 0}}]}
        h = alias_hints(original, {'locals': []}, 0, ['System.Int32'])
        self.assertEqual(h['original_unused_slots'][0]['stores'], 1)

    def test_alias_hint_does_not_scan_through_a_branch(self):
        original = {'locals': ['I4'], 'instructions': [
            {'offset': 0, 'opcode': 'brfalse', 'operand': {'branch': 100}},
            {'offset': 5, 'opcode': 'stloc.0', 'operand': None}]}
        self.assertEqual(alias_hints(original, {'locals': []}, 1, ['System.Int32'])['expected_alias_types'], [])

    def test_zero_chain_is_proved_at_the_actual_divergence(self):
        original = {'locals': ['I4', 'I4'], 'header': {'code_size': 6}, 'instructions': [
            {'offset': 0, 'opcode': 'ldc.i4.0', 'operand': None},
            {'offset': 1, 'opcode': 'dup', 'operand': None},
            {'offset': 2, 'opcode': 'stloc.1', 'operand': None},
            {'offset': 3, 'opcode': 'stloc.0', 'operand': None},
            {'offset': 4, 'opcode': 'ldarg.0', 'operand': None}]}
        self.assertEqual(alias_hints(original, {'locals': []}, 2, ['System.Int32'] * 2)['original_zero_chain_stores'], 2)
        self.assertEqual(alias_hints(original, {'locals': []}, 4, ['System.Int32'] * 2)['original_zero_chain_stores'], 0)


if __name__ == '__main__':
    unittest.main()
