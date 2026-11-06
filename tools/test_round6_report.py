"""Do not turn model coverage, candidate signatures or partial engines into proof."""
import copy
import unittest
from unittest.mock import patch

from round6_report import compiled_signature_statistics, model_statistics, signature_changes, validate_engine_selection, validate_evidence, validate_model_validation_pin
from round6_update_toolchain import EVIDENCE_KEY, make_current_section, prepend_current_section, unchanged_selection


class ModelReportingTests(unittest.TestCase):
    def test_complete_coverage_and_incidental_incomplete_matches_have_separate_denominators(self):
        rows = [
            {'method_id': 'A', 'state': 'matched', 'source': 'a.cs', 'body_equal': True,
             'complete': True, 'prediction_equal': True, 'validation_classification': []},
            {'method_id': 'B', 'state': 'matched', 'source': 'b.cs', 'body_equal': True,
             'complete': False, 'prediction_equal': True, 'validation_classification': ['semantic_errors:CS0115']},
            {'method_id': 'C', 'state': 'matched', 'body_equal': True,
             'complete': False, 'prediction_equal': False, 'validation_classification': ['generated_or_unmapped_source_method']},
            {'method_id': 'D', 'state': 'decompiled', 'source': 'd.cs', 'body_equal': False,
             'complete': False, 'prediction_equal': False, 'validation_classification': ['capture', 'semantic_error']},
        ]
        result = model_statistics({'methods': rows})
        self.assertEqual((result['all_matched'], result['mapped'], result['unmapped']), (3, 2, 1))
        self.assertEqual((result['exact_all'], result['complete'], result['complete_exact']), (2, 1, 1))
        self.assertEqual(result['pending']['incorrect_current_signature'], 1)
        self.assertEqual(sum(result['pending']['incorrect_classes'].values()), 2)

    def test_wrong_complete_prediction_and_unclassified_error_remain_visible(self):
        result = model_statistics({'methods': [{'method_id': 'Bad', 'state': 'matched', 'source': 'b.cs',
                                  'body_equal': True, 'complete': True, 'prediction_equal': False,
                                  'validation_classification': []}]})
        self.assertEqual(result['complete_errors'], 1)
        self.assertEqual(result['unexplained'], ['Bad'])

    def test_candidate_signature_confirmations_do_not_require_acceptance_or_count_as_adoption(self):
        first = {'id': 'A', 'trials': [{'original_local_signature_equal': True,
                                      'predicted_signature_confirmed': True, 'accepted': False}]}
        second = {'id': 'A', 'trials': [{'original_local_signature_equal': True,
                                       'predicted_signature_confirmed': True, 'accepted': False}]}
        other = {'id': 'B', 'trials': [{'predicted_signature_confirmed': False, 'accepted': False}]}
        result = compiled_signature_statistics({'methods': [second, other], 'previous_passes': {'A': [first]}})
        self.assertEqual(result['methods_with_compiled_original_signature_equal'], ['A'])
        self.assertEqual(result['compiled_attempts_with_original_signature_equal'], 2)
        self.assertEqual(result['methods_with_prediction_disagreement'], ['B'])

    def test_adopted_signature_population_keeps_closed_bodies_and_distinguishes_losses(self):
        original = {k: {'locals': ['I4']} for k in ('A', 'B', 'C')}
        old = {'A': {'locals': ['R']}, 'B': {'locals': ['I4']}, 'C': {'locals': ['I4']}}
        new = {'A': {'locals': ['I4']}, 'B': {'locals': ['I4']}, 'C': {'locals': ['R']}}
        before = {'methods': [{'stable_id': k} for k in original]}
        after = {'methods': [{'stable_id': 'B'}, {'stable_id': 'C'}]}
        with patch('round6_report._dumps', side_effect=[(original, old), (original, new)]):
            result = signature_changes(before, after)
        self.assertEqual(result['newly_equal'], ['A'])
        self.assertEqual(result['newly_equal_and_closed'], ['A'])
        self.assertEqual(result['lost_equal'], ['C'])
        self.assertEqual(result['signature_equal_after_for_same_ids'], 2)


class ReportProofGuards(unittest.TestCase):
    def engine(self):
        return {'methods': [{'id': 'A'}, {'id': 'B'}], 'completed': True, 'adoption_eligible': True,
                'adopted_ledger_states': False, 'body_losses': []}

    def test_partial_wrong_inventory_and_duplicate_engine_cannot_be_reported_as_complete(self):
        validate_engine_selection('App', self.engine(), {'A', 'B'}, {'A', 'B', 'C'})
        for bad in [dict(self.engine(), completed=False), dict(self.engine(), methods=[{'id': 'A'}]),
                    dict(self.engine(), methods=[{'id': 'A'}, {'id': 'C'}]),
                    dict(self.engine(), methods=[{'id': 'A'}, {'id': 'A'}, {'id': 'B'}]),
                    dict(self.engine(), body_losses=['OldMatched']), dict(self.engine(), adopted_ledger_states=True)]:
            with self.assertRaises(ValueError):
                validate_engine_selection('App', bad, {'A', 'B'}, {'A', 'B', 'C'})

    def test_unverified_adoption_is_rejected_before_any_files_are_read(self):
        with self.assertRaisesRegex(ValueError, 'official adoption verification'):
            validate_evidence({}, {}, {}, {'official_verified': False}, {}, {})

    def test_exact_archived_engine_input_can_gain_only_derived_summary_fields(self):
        archived = {'methods': [{'method_id': 'A', 'complete': True}], 'source_sha256': 'source'}
        enhanced = dict(archived, matched_source_mapped=1)
        identity = {'validation_sha256': 'old-input'}
        self.assertEqual(validate_model_validation_pin(enhanced, identity, 'new-summary', archived, 'old-input'),
                         'archived-engine-input-with-identical-prediction-rows')
        for bad in [dict(enhanced, methods=[{'method_id': 'A', 'complete': False}]),
                    dict(enhanced, source_sha256='other'), dict(enhanced, unrecognized_mutation=True)]:
            with self.assertRaises(ValueError):
                validate_model_validation_pin(bad, identity, 'new-summary', archived, 'old-input')
        with self.assertRaises(ValueError):
            validate_model_validation_pin(enhanced, identity, 'new-summary', archived, 'wrong-archive')


class ToolchainProtectionTests(unittest.TestCase):
    def configuration(self):
        return {'assemblies': {'KairoLibrary': {'compiler': 'csc', 'flags': ['O+'], 'validation_scope': 'old'},
                               'Assembly-CSharp': {'compiler': 'smcs', 'references': ['Unity'], 'validation_scope': 'old'}},
                'non_deterministic_exceptions': ['MVID'], 'historical_evidence': {'keep': True}}

    def test_only_validation_text_and_this_round_evidence_are_mutable(self):
        before = self.configuration()
        after = copy.deepcopy(before)
        after['assemblies']['KairoLibrary']['validation_scope'] = 'new'
        after[EVIDENCE_KEY] = {'measured': True}
        unchanged_selection(before, after)
        for mutate in [lambda d: d['assemblies']['KairoLibrary'].update(compiler='new'),
                       lambda d: d['assemblies']['KairoLibrary']['flags'].append('debug'),
                       lambda d: d['assemblies']['Assembly-CSharp']['references'].append('Other'),
                       lambda d: d['non_deterministic_exceptions'].append('IL'),
                       lambda d: d['historical_evidence'].update(keep=False)]:
            bad = copy.deepcopy(after)
            mutate(bad)
            with self.assertRaises(ValueError):
                unchanged_selection(before, bad)

    def test_current_section_refresh_preserves_previous_history_and_is_idempotent(self):
        old = '# Toolchain e evidências\n\n## Estado atual — rodada iniciada em 7d9fbc8\n\nPrevious proof\n\n## Older\n\nKeep this\n'
        section = '## Estado atual — rodada iniciada em 66eea9d\n\nCurrent proof\n'
        updated = prepend_current_section(old, section)
        self.assertIn('## Histórico — rodada iniciada em 7d9fbc8\n\nPrevious proof', updated)
        self.assertIn('## Older\n\nKeep this', updated)
        self.assertEqual(prepend_current_section(updated, section), updated)


if __name__ == '__main__':
    unittest.main()
