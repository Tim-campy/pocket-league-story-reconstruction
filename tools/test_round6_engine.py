"""Independent negative controls for the model gate and raw-prefix boundary."""
import copy
import json
from types import SimpleNamespace
import unittest
from unittest.mock import patch

from compare_kairo import differences
from round6_alignment import align_methods
from round6_engine import Compiler, aligned_progress, model_approval, prefix, search_progress


def method(code, instructions=None, *, locals_=None, eh=None):
    """Small complete body dictionaries; the fixture never emits or executes IL."""
    return {'stable_id': 'T::M()->System.Void',
            'raw_body_hex': bytes([(len(code) << 2) | 2] + code).hex(),
            'metadata_operands': [],
            'header': {'size': 1, 'code_size': len(code)},
            'locals': locals_ or [], 'exception_regions': eh or [],
            'instructions': instructions or []}


class ModelGateTests(unittest.TestCase):
    def test_only_complete_and_exact_ordered_signature_authorizes_build(self):
        expected = ['System.Object', 'System.Int32', 'System.Double']
        self.assertTrue(model_approval({'complete': True, 'predicted_types': expected}, expected))
        self.assertFalse(model_approval({'complete': False, 'predicted_types': expected}, expected))
        self.assertFalse(model_approval({'complete': True, 'predicted_types': list(reversed(expected))}, expected))

    def test_absent_or_truthy_non_boolean_completeness_fails_closed(self):
        for value in ({'predicted_types': []}, {'complete': 1, 'predicted_types': []},
                      {'complete': 'true', 'predicted_types': []}, {'complete': True}):
            self.assertFalse(model_approval(value, []))

    def test_unknown_type_does_not_match_a_known_original_slot(self):
        self.assertFalse(model_approval({'complete': True, 'predicted_types': ['UNKNOWN']}, ['System.Int32']))


class GenericInspectorContractTests(unittest.TestCase):
    def test_cecil_name_selector_omits_method_arity_but_retains_type_arity(self):
        identity = 'Example.Box`1::Init``1(System.String)->System.Void'
        def inspector(command, **kwargs):
            # ProbeInspector selects TypeDefinition.FullName + MethodDefinition.Name.
            found = [{'stable_id': identity}] if 'Example.Box`1::Init' in command else []
            return SimpleNamespace(stdout=json.dumps({'selected_methods': found}).encode())
        compiler = Compiler.__new__(Compiler)
        with patch('round6_engine.subprocess.run', side_effect=inspector):
            self.assertIn(identity, compiler.dump('unused.dll', [identity]))


class PrefixAcceptanceTests(unittest.TestCase):
    def test_forward_branch_can_wait_for_multiple_later_edits_without_false_match(self):
        original = method([0x2b, 4, 0x17, 0x26, 0x18, 0x26, 0x2a], [
            {'offset': 0, 'opcode': 'br.s', 'operand': {'branch': 6}},
            {'offset': 2, 'opcode': 'ldc.i4.1', 'operand': None},
            {'offset': 3, 'opcode': 'pop', 'operand': None},
            {'offset': 4, 'opcode': 'ldc.i4.2', 'operand': None},
            {'offset': 5, 'opcode': 'pop', 'operand': None},
            {'offset': 6, 'opcode': 'ret', 'operand': None}])
        before = method([0x2b, 5, 0x16, 0x26, 0x16, 0x26, 0, 0x2a], [
            {'offset': 0, 'opcode': 'br.s', 'operand': {'branch': 7}},
            {'offset': 2, 'opcode': 'ldc.i4.0', 'operand': None},
            {'offset': 3, 'opcode': 'pop', 'operand': None},
            {'offset': 4, 'opcode': 'ldc.i4.0', 'operand': None},
            {'offset': 5, 'opcode': 'pop', 'operand': None},
            {'offset': 6, 'opcode': 'nop', 'operand': None},
            {'offset': 7, 'opcode': 'ret', 'operand': None}])
        after = copy.deepcopy(before)
        after['instructions'][1]['opcode'] = 'ldc.i4.1'
        after['raw_body_hex'] = bytes([0x22, 0x2b, 5, 0x17, 0x26, 0x16, 0x26, 0, 0x2a]).hex()
        self.assertEqual(prefix(original, before), prefix(original, after))
        self.assertFalse(search_progress(original, before, after))
        self.assertTrue(search_progress(original, before, after, allow_labelled=True))
        self.assertFalse(search_progress(original, after, after, allow_labelled=True))
        self.assertTrue(differences(original, after)[0])
        changed_earlier = copy.deepcopy(after)
        changed_earlier['raw_body_hex'] = bytes([0x22, 0x38, 5, 0x17, 0x26, 0x16, 0x26, 0, 0x2a]).hex()
        self.assertFalse(search_progress(original, before, changed_earlier, allow_labelled=True))

    def test_alignment_moves_source_target_but_does_not_accept_unchanged_bytes(self):
        left = method([0x02, 0x39, 2, 0, 0, 0, 0x16, 0x26, 0x2a], [
            {'offset': 0, 'opcode': 'ldarg.0', 'operand': None},
            {'offset': 1, 'opcode': 'brfalse', 'operand': {'branch': 8}},
            {'offset': 6, 'opcode': 'ldc.i4.0', 'operand': None},
            {'offset': 7, 'opcode': 'pop', 'operand': None},
            {'offset': 8, 'opcode': 'ret', 'operand': None}])
        right = method([0x02, 0x39, 3, 0, 0, 0, 0x16, 0x00, 0x26, 0x2a], [
            {'offset': 0, 'opcode': 'ldarg.0', 'operand': None},
            {'offset': 1, 'opcode': 'brfalse', 'operand': {'branch': 9}},
            {'offset': 6, 'opcode': 'ldc.i4.0', 'operand': None},
            {'offset': 7, 'opcode': 'nop', 'operand': None},
            {'offset': 8, 'opcode': 'pop', 'operand': None},
            {'offset': 9, 'opcode': 'ret', 'operand': None}])
        self.assertEqual(prefix(left, right), 2)
        self.assertEqual(align_methods(left, right)['rebuilt_offset'], 7)
        self.assertTrue(differences(left, right)[0])
        self.assertFalse(aligned_progress(left, right, copy.deepcopy(right)))
        self.assertTrue(aligned_progress(left, right, left))

    def test_raw_prefix_must_strictly_advance_or_entire_body_must_match(self):
        original = method([0x16, 0x17, 0x58, 0x2a])
        before = method([0x16, 0x18, 0x59, 0x2a])
        after = method([0x16, 0x17, 0x59, 0x2a])
        self.assertEqual(prefix(original, before), 1)
        self.assertEqual(prefix(original, after), 2)
        self.assertTrue(aligned_progress(original, before, after))
        self.assertFalse(aligned_progress(original, before, before))

    def test_later_repair_does_not_authorize_an_earlier_regression(self):
        original = method([0x16, 0x17, 0x58, 0x2a])
        before = method([0x16, 0x18, 0x59, 0x2a])
        changed_earlier = method([0x17, 0x17, 0x58, 0x2a])
        self.assertFalse(aligned_progress(original, before, changed_earlier))

    def test_same_il_with_wrong_locals_or_exception_regions_is_not_complete(self):
        original = method([0x2a], locals_=['System.Int32'], eh=[{'kind': 'finally'}])
        before = method([0x2a], locals_=['System.Object'])
        self.assertEqual(prefix(original, before), 1)
        self.assertFalse(aligned_progress(original, before, before))
        locals_only_fixed = method([0x2a], locals_=['System.Int32'])
        self.assertFalse(aligned_progress(original, before, locals_only_fixed))
        self.assertTrue(aligned_progress(original, before, copy.deepcopy(original)))

    def test_short_long_branch_alignment_never_replaces_raw_equality(self):
        original = method([0x2b, 0, 0x2a], [
            {'offset': 0, 'opcode': 'br.s', 'operand': {'branch': 2}},
            {'offset': 2, 'opcode': 'ret', 'operand': None}])
        rebuilt = method([0x38, 0, 0, 0, 0, 0x2a], [
            {'offset': 0, 'opcode': 'br', 'operand': {'branch': 5}},
            {'offset': 5, 'opcode': 'ret', 'operand': None}])
        self.assertEqual(align_methods(original, rebuilt)['kind'], 'aligned_identical')
        self.assertTrue(differences(original, rebuilt)[0])
        self.assertFalse(aligned_progress(original, rebuilt, rebuilt))


if __name__ == '__main__':
    unittest.main()
