"""Read-only alias calculations require immutable slots and unique use evidence."""
import copy
import unittest

from round6_alias_calculus import calculate_parameter_aliases, calculated_alias_hints, parameter_operation


def method(rows, locals_, instance=False):
    instructions = [{'offset': i, 'opcode': row[0], 'operand': row[1] if len(row)>1 else None} for i,row in enumerate(rows)]
    return {'locals': locals_, 'instructions': instructions, 'header': {'code_size': len(rows)},
            'signature': ';this=' + str(instance) + ';'}


def pair():
    original = method([('ldarg.1',),('stloc.0',),('ldarg.2',),('stloc.1',),('ldarg.0',),
                       ('ldloc.0',),('ldloc.1',),('add',),('call',{'method':'Use'}),('ret',)], ['I4','I4'])
    rebuilt = method([('ldarg.2',),('stloc.0',),('ldarg.0',),('ldarg.1',),('ldloc.0',),
                      ('add',),('call',{'method':'Use'}),('ret',)], ['I4'])
    return original, rebuilt


class AliasCalculusTests(unittest.TestCase):
    def test_missing_alias_maps_only_original_local_use_to_direct_argument(self):
        original, rebuilt = pair()
        result = calculate_parameter_aliases(original,rebuilt)
        self.assertEqual(len(result['aliases']),1)
        alias = result['aliases'][0]
        self.assertEqual(alias['original_alias_parameter_index'],1)
        self.assertEqual(alias['original_alias_slot'],0)
        self.assertEqual(alias['alias_use_rebuilt_offsets'],[3])
        self.assertEqual((alias['original_alias_first_use'],alias['original_alias_last_use']),(5,5))

    def test_parameter_metadata_index_accounts_for_instance_this_only_in_implicit_forms(self):
        instance={'signature':';this=True;'}
        self.assertEqual(parameter_operation({'opcode':'ldarg.1','operand':None},instance),('ldarg',0))
        self.assertEqual(parameter_operation({'opcode':'ldarg.s','operand':{'parameter':0}},instance),('ldarg',0))
        self.assertEqual(parameter_operation({'opcode':'ldarg.0','operand':None},instance),('ldarg',-1))

    def test_mutated_alias_or_address_use_is_rejected(self):
        for extra in [('stloc.0',),('ldloca.s',{'local':0})]:
            original,rebuilt=pair()
            original['instructions'].append({'offset':10,'opcode':extra[0],'operand':extra[1] if len(extra)>1 else None})
            self.assertFalse(calculate_parameter_aliases(original,rebuilt)['aliases'])

    def test_parameter_write_or_address_use_is_rejected(self):
        for opcode in ('starg.s','ldarga.s'):
            original,rebuilt=pair()
            original['instructions'].append({'offset':10,'opcode':opcode,'operand':{'parameter':1}})
            self.assertFalse(calculate_parameter_aliases(original,rebuilt)['aliases'])

    def test_wrong_local_type_or_more_than_one_missing_slot_is_rejected(self):
        original,rebuilt=pair()
        rebuilt['locals']=['R']
        self.assertFalse(calculate_parameter_aliases(original,rebuilt)['aliases'])
        rebuilt['locals']=[]
        self.assertEqual(calculate_parameter_aliases(original,rebuilt)['status'],'unsupported_local_count_difference')

    def test_conditional_store_bypassed_by_branch_is_not_a_proved_parameter_alias(self):
        original=method([('ldarg.0',),('brtrue',{'branch':5}),('ldarg.1',),('stloc.0',),
                         ('nop',),('ldloc.0',),('ret',)],['I4'])
        rebuilt=method([('ldarg.0',),('brtrue',{'branch':3}),('nop',),('ldarg.1',),('ret',)],[])
        result=calculate_parameter_aliases(original,rebuilt)
        self.assertFalse(result['aliases'])
        self.assertIn('dominate',result['rejected'][0]['reason'])

    def test_exception_region_dominance_is_not_guessed(self):
        original,rebuilt=pair()
        original['exception_regions']=[{'kind':'finally'}]
        self.assertFalse(calculate_parameter_aliases(original,rebuilt)['aliases'])

    def test_existing_rebuilt_local_use_is_not_replaced_as_if_it_were_parameter(self):
        original,rebuilt=pair()
        rebuilt['instructions'][3]['opcode']='ldloc.0'
        self.assertFalse(calculate_parameter_aliases(original,rebuilt)['aliases'])

    def test_symbols_with_unproved_body_are_not_used_for_source_hints(self):
        original,rebuilt=pair()
        # No raw-body fields means these instruction dictionaries still differ.
        self.assertEqual(calculated_alias_hints(original,rebuilt,None),{'alias_calculus_status':'symbols_body_not_proved_equal'})
        other=copy.deepcopy(rebuilt)
        other['locals']=['R']
        self.assertEqual(calculated_alias_hints(original,rebuilt,other),{'alias_calculus_status':'symbols_body_not_proved_equal'})

    def test_equal_body_symbols_provide_parameter_occurrence_within_current_statement(self):
        original,rebuilt=pair()
        symbols=copy.deepcopy(rebuilt)
        symbols['sequence_points']=[{'offset':2,'start_line':3,'start_column':4,'end_line':3,'end_column':30,'hidden':False}]
        hints=calculated_alias_hints(original,rebuilt,symbols)
        self.assertEqual(hints['original_alias_parameter_index'],1)
        self.assertEqual(hints['alias_use_source_points'][0]['parameter_occurrence'],0)
        self.assertEqual(hints['alias_use_source_points'][0]['parameter_occurrences'],1)

    def test_historical_mdb_line_only_span_is_explicitly_delegated_to_ast_uniqueness(self):
        original,rebuilt=pair()
        symbols=copy.deepcopy(rebuilt)
        symbols['sequence_points']=[{'offset':2,'start_line':3,'start_column':-1,'end_line':-1,'end_column':-1,'hidden':False}]
        hints=calculated_alias_hints(original,rebuilt,symbols)
        self.assertEqual(hints['alias_use_source_points'][0]['source_span_kind'],'line-only-requires-unique-AST-statement')


if __name__=='__main__':
    unittest.main()
