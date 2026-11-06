"""Protect the diagnostic's IL boundary and its strict branch-byte contract."""
import copy
import unittest

from compare_kairo import differences
from round4_measure import first_difference, instruction_at


def body(code, instructions, prefix=b'\x02', metadata=()):
    return {'raw_body_hex': (prefix + bytes(code)).hex(),
            'header': {'size': len(prefix), 'code_size': len(code)},
            'metadata_operands': list(metadata), 'instructions': instructions,
            'locals': [], 'exception_regions': []}


class FirstDifferenceBoundaryTests(unittest.TestCase):
    def test_branch_operand_difference_remains_a_real_first_difference(self):
        left = body([0x2b, 2, 0x00, 0x00, 0x2a], [{'offset': 0, 'opcode': 'br.s', 'operand': {'branch': 4}}, {'offset': 2, 'opcode': 'nop', 'operand': None}, {'offset': 3, 'opcode': 'nop', 'operand': None}, {'offset': 4, 'opcode': 'ret', 'operand': None}])
        right = copy.deepcopy(left)
        right['raw_body_hex'] = '022b0100002a'
        right['instructions'][0]['operand'] = {'branch': 3}
        self.assertEqual(first_difference(left, right), 1)
        self.assertEqual(instruction_at(left, 1)[0]['opcode'], 'br.s')

    def test_metadata_token_values_are_resolved_but_identity_is_not_erased(self):
        left = body([0x28, 1, 0, 0, 6, 0x2a], [{'offset': 0, 'opcode': 'call', 'operand': {'method': 'A::M'}}, {'offset': 5, 'opcode': 'ret', 'operand': None}], metadata=[{'body_offset': 2, 'width': 4, 'identity': {'method': 'A::M'}}])
        right = copy.deepcopy(left)
        right['raw_body_hex'] = '02280300000a2a'
        self.assertIsNone(first_difference(left, right))
        right['metadata_operands'][0]['identity'] = {'method': 'A::N'}
        self.assertEqual(first_difference(left, right), 1)

    def test_header_and_local_differences_do_not_become_false_il_offsets(self):
        left = body([0x2a], [{'offset': 0, 'opcode': 'ret', 'operand': None}])
        right = copy.deepcopy(left)
        right['raw_body_hex'] = '062a'
        right['locals'] = ['System.Int32']
        self.assertIsNone(first_difference(left, right))
        self.assertIn('locals', differences(left, right)[0])
        self.assertIn('resolved_body_bytes', differences(left, right)[0])

    def test_unpaired_il_tail_reports_end_of_shorter_body(self):
        left = body([0x00, 0x2a], [{'offset': 0, 'opcode': 'nop', 'operand': None}, {'offset': 1, 'opcode': 'ret', 'operand': None}])
        right = body([0x00], [{'offset': 0, 'opcode': 'nop', 'operand': None}])
        self.assertEqual(first_difference(left, right), 1)
        self.assertEqual(instruction_at(right, 1)[0]['opcode'], 'END_IL')


if __name__ == '__main__':
    unittest.main()
