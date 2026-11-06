"""Independent negative controls for the model gate and raw-prefix boundary."""
import copy
import json
from types import SimpleNamespace
import unittest
from unittest.mock import patch

from compare_kairo import differences
from round6_alignment import align_methods
from round7_engine import AcceptedSymbolCache, Compiler, aligned_progress, cycle_candidates, model_approval, prefix, search_progress


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


class CycleRepertoireTests(unittest.TestCase):
    def setUp(self):
        self.patches=[{'rule':'guard-return'}, {'rule':'new-rule'},
                      {'rule':'new-rule-block'}, {'rule':'solver-declarations'}]

    def test_fresh_cycle_allows_only_selected_family_and_variants(self):
        self.assertEqual([p['rule'] for p in cycle_candidates(self.patches,'new-rule',0)],
                         ['new-rule','new-rule-block'])
        self.assertEqual(len(self.patches),4)

    def test_first_accepted_step_unlocks_existing_repertoire(self):
        self.assertEqual(cycle_candidates(self.patches,'new-rule',1),self.patches)

    def test_initial_full_pass_keeps_existing_repertoire(self):
        self.assertEqual(cycle_candidates(self.patches,None,0),self.patches)

    def test_absent_selected_rule_does_not_fall_back_to_exhausted_rules(self):
        self.assertEqual(cycle_candidates(self.patches,'other-new-rule',0),[])


class AcceptedSymbolCacheTests(unittest.TestCase):
    def setUp(self):
        self.accepted = method([0x2a], locals_=['System.Int32'], eh=[{'kind':'finally'}])
        self.accepted['sequence_points']=[{'offset':0, 'start_line':7, 'document':'accepted.cs'}]
        self.payload={'methods':[self.accepted], 'symbol_kind':'MDB', 'mapping_binary_sha256':'accepted-binary'}
        self.loads=0
        self.proofs=0
        self.version=0
        def loader():
            self.loads+=1
            return self.payload
        def proof():
            self.proofs+=1
            return {'manifest_sha256':'manifest', 'accepted_source_set_sha256':'source-'+str(self.version)}
        self.cache=AcceptedSymbolCache(loader,proof)

    def test_one_all_dump_is_reused_for_methods_in_the_same_generation(self):
        self.assertIsNotNone(self.cache.get(self.accepted['stable_id']))
        self.assertIsNone(self.cache.get('Other::Missing()->System.Void'))
        self.assertEqual((self.loads,self.proofs),(1,1))
        self.assertEqual(self.cache.stats['hits'],1)
        self.assertEqual(self.cache.stats['generation_proofs'][0]['method_count'],1)

    def test_failed_trial_overwrite_does_not_replace_accepted_snapshot(self):
        first=self.cache.get(self.accepted['stable_id'])
        self.payload['methods'][0]['raw_body_hex']='DEAD'
        self.payload['methods'][0]['sequence_points'][0]['document']='failed-trial.cs'
        second=self.cache.get(first['stable_id'])
        self.assertEqual(second,first)
        self.assertEqual(self.loads,1)
        self.assertEqual(self.cache.generation,0)

    def test_returned_method_cannot_mutate_cached_body_or_sequence_points(self):
        first=self.cache.get(self.accepted['stable_id'])
        first['sequence_points'][0]['document']='caller-mutated.cs'
        first['locals'].append('System.Object')
        second=self.cache.get(self.accepted['stable_id'])
        self.assertEqual(second['sequence_points'][0]['document'],'accepted.cs')
        self.assertEqual(second['locals'],['System.Int32'])

    def test_accepted_full_verified_change_invalidates_and_records_new_generation(self):
        self.cache.get(self.accepted['stable_id'])
        self.cache.accepted(self.accepted['stable_id'],'new-rule')
        self.version=1
        self.payload['methods'][0]['sequence_points'][0]['document']='accepted-generation-1.cs'
        fresh=self.cache.get(self.accepted['stable_id'])
        self.assertEqual(fresh['sequence_points'][0]['document'],'accepted-generation-1.cs')
        self.assertEqual((self.loads,self.proofs),(2,2))
        self.assertEqual(self.cache.stats['accepted_invalidations'],1)
        self.assertEqual([p['generation'] for p in self.cache.stats['generation_proofs']],[0,1])
        self.assertEqual(self.cache.stats['generation_proofs'][1]['accepted_source_set_sha256'],'source-1')

    def test_MDB_mapping_rejects_wrong_live_body_locals_EH_or_missing_method(self):
        self.assertIsNotNone(self.cache.get(self.accepted['stable_id'],self.accepted,True))
        for current in (method([0x00,0x2a]), method([0x2a],locals_=['System.Object']),
                        method([0x2a],locals_=['System.Int32']), None):
            self.assertIsNone(self.cache.get(self.accepted['stable_id'],current,True))
        self.assertEqual(self.cache.stats['MDB_body_mismatches'],4)
        self.assertEqual(self.loads,1)

    def test_failed_mapping_build_is_not_saved_as_an_all_method_snapshot(self):
        responses=iter([None,self.payload])
        cache=AcceptedSymbolCache(lambda:next(responses),lambda:{})
        self.assertIsNone(cache.get(self.accepted['stable_id']))
        self.assertIsNotNone(cache.get(self.accepted['stable_id']))
        self.assertEqual(cache.stats['load_failures'],1)
        self.assertEqual(cache.stats['loads'],1)


class GenericInspectorContractTests(unittest.TestCase):
    def test_cecil_name_selector_omits_method_arity_but_retains_type_arity(self):
        identity = 'Example.Box`1::Init``1(System.String)->System.Void'
        def inspector(command, **kwargs):
            # ProbeInspector selects TypeDefinition.FullName + MethodDefinition.Name.
            found = [{'stable_id': identity}] if 'Example.Box`1::Init' in command else []
            return SimpleNamespace(stdout=json.dumps({'selected_methods': found}).encode())
        compiler = Compiler.__new__(Compiler)
        with patch('round7_engine.subprocess.run', side_effect=inspector):
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
