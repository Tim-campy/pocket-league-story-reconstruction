import unittest

from round6_alignment import align_methods, make_histogram


def method(*instructions, size=None, eh=None):
    rows = [{'offset': p[0], 'opcode': p[1], 'operand': p[2] if len(p) > 2 else None} for p in instructions]
    return {'instructions': rows, 'header': {'code_size': size if size is not None else rows[-1]['offset'] + 1},
            'raw_body_hex': '00', 'exception_regions': eh or [], 'stable_id': 'T::M()->System.Void'}


class AlignmentTests(unittest.TestCase):
    def test_insert_moves_branch_target_without_diagnosing_the_branch(self):
        left = method((0, 'brfalse', {'branch': 7}), (5, 'ldc.i4.0'), (6, 'pop'), (7, 'ret'))
        right = method((0, 'brfalse', {'branch': 8}), (5, 'ldc.i4.0'), (6, 'nop'), (7, 'pop'), (8, 'ret'))
        result = align_methods(left, right)
        self.assertEqual(result['kind'], 'insert')
        self.assertEqual(result['rebuilt_offset'], 6)
        self.assertEqual(result['branch_retarget_differences'], [])
        self.assertEqual(result['original_context'][0]['operand'], result['rebuilt_context'][0]['operand'])

    def test_retarget_to_another_existing_instruction_is_retained(self):
        left = method((0, 'brfalse', {'branch': 7}), (5, 'ldc.i4.0'), (6, 'pop'), (7, 'ret'))
        right = method((0, 'brfalse', {'branch': 6}), (5, 'ldc.i4.0'), (6, 'pop'), (7, 'ret'))
        result = align_methods(left, right)
        self.assertEqual(result['kind'], 'branch_retarget')
        self.assertEqual(result['original_offset'], 0)

    def test_branch_width_changes_are_diagnostic_only(self):
        left = method((0, 'br.s', {'branch': 2}), (2, 'ret'))
        right = method((0, 'br', {'branch': 5}), (5, 'ret'))
        result = align_methods(left, right)
        self.assertEqual(result['kind'], 'aligned_identical')
        self.assertEqual(len(result['branch_width_differences']), 1)

    def test_switch_target_order_is_not_ignored(self):
        left = method((0, 'switch', {'switch': [13, 14]}), (13, 'nop'), (14, 'ret'))
        right = method((0, 'switch', {'switch': [14, 13]}), (13, 'nop'), (14, 'ret'))
        self.assertEqual(len(align_methods(left, right)['branch_retarget_differences']), 2)

    def test_constants_and_local_slots_remain_significant(self):
        left = method((0, 'ldc.i4.0'), (1, 'stloc.2'), (2, 'ret'))
        right = method((0, 'ldc.i4.0'), (1, 'stloc.0'), (2, 'ret'))
        result = align_methods(left, right)
        self.assertEqual(result['kind'], 'replace')
        self.assertEqual(result['opcode_pair'], 'stloc.2 => stloc.0')

    def test_exception_regions_are_retained_separately(self):
        left = method((0, 'ret'), eh=[{'try_start': 0, 'try_end': 1}])
        right = method((0, 'ret'))
        result = align_methods(left, right)
        self.assertEqual(result['kind'], 'aligned_identical')
        self.assertNotEqual(result['original_exception_regions'], result['rebuilt_exception_regions'])

    def test_target_inside_changed_range_is_explicitly_unresolved(self):
        left = method((0, 'brfalse', {'branch': 5}), (5, 'ldc.i4.0'), (6, 'ret'))
        right = method((0, 'brfalse', {'branch': 5}), (5, 'ldc.i4.1'), (6, 'ret'))
        result = align_methods(left, right)
        self.assertEqual(result['original_offset'], 5)
        self.assertEqual(len(result['deferred_branch_targets']), 1)

    def test_rebuilt_target_into_inserted_range_remains_conservative_retarget(self):
        left = method((0, 'brfalse', {'branch': 7}), (5, 'ldc.i4.0'), (6, 'pop'), (7, 'ret'))
        right = method((0, 'brfalse', {'branch': 7}), (5, 'ldc.i4.0'), (6, 'pop'), (7, 'nop'), (8, 'ret'))
        result = align_methods(left, right)
        self.assertEqual(result['kind'], 'branch_retarget')
        self.assertEqual(result['original_offset'], 0)
        self.assertEqual(len(result['branch_retarget_differences']), 1)

    def test_missing_body_is_delete_and_histogram_accepts_dump(self):
        left = method((0, 'ret'))
        result = align_methods(left, None)
        self.assertEqual(result['kind'], 'delete')
        hist = make_histogram({'selected_methods': [left]}, {'selected_methods': []})
        self.assertEqual(hist['opcode_histogram'][0]['methods'], 1)


if __name__ == '__main__':
    unittest.main()
