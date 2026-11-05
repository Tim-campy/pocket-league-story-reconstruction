"""Negative and nested-span controls for the metadata ordering planner."""
import unittest
from plan_all_member_order import choose_order, render_edits, namespace_compile_order


def field(name):
    return {'name':name,'kind':'FieldDeclaration','field_names':[name],'method_ids':[],'weight':1}


def method(name):
    return {'name':name,'kind':'MethodDeclaration','field_names':[],'method_ids':[name],'weight':1}


def original(methods=(),fields=()):
    return {'methods':list(methods),'fields':[{'name':f} for f in fields], 'properties':[], 'events':[], 'nested_types':[]}


class OrderingControls(unittest.TestCase):
    def test_separate_tables_do_not_impose_field_method_interleaving(self):
        members=[method('A'),field('x'),method('B'),field('y')]
        sequence,_=choose_order(members,original(['A','B'],['x','y']),[])
        self.assertEqual(sequence,[0,1,2,3])

    def test_independent_method_and_field_order_is_enforced(self):
        members=[field('x'),method('A'),field('y'),method('B')]
        sequence,_=choose_order(members,original(['B','A'],['y','x']),[])
        self.assertLess(sequence.index(3),sequence.index(1))
        self.assertLess(sequence.index(2),sequence.index(0))

    def test_single_declaration_cannot_hide_accessor_order_difference(self):
        member=method('P');member['method_ids']=['get_P','set_P']
        with self.assertRaisesRegex(ValueError,'within-declaration order'):
            choose_order([member],original(['set_P','get_P']),[])

    def test_interleaving_property_accessors_is_rejected(self):
        member=method('P');member['method_ids']=['get_P','set_P']
        with self.assertRaisesRegex(ValueError,'interleave'):
            choose_order([member,method('Other')],original(['get_P','Other','set_P']),[])

    def test_generated_ordinal_is_preserved_while_reordering(self):
        members=[field('x'),method('A'),method('B')]
        constraints=[{'id':'A','original_slot_candidates':[2]}]
        sequence,_=choose_order(members,original(['B','A'],['x']),constraints)
        self.assertEqual(sequence,[0,2,1])

    def test_parent_and_nested_edits_keep_original_coordinates(self):
        encoded='0123456789abcdef'.encode('utf-16-le')
        edits=[{'start':0,'end':32,'chunks':[(16,32),(0,16)]},
               {'start':4,'end':12,'chunks':[(8,12),(4,8)]}]
        self.assertEqual(render_edits(encoded,edits).decode('utf-16-le'),'89abcdef01452367')

    def test_namespace_groups_reverse_recursively_but_types_stay_forward(self):
        names=['Z.B.First','Z.B.Second','Z.A.Third','A.Fourth']
        files={name:name+'.cs' for name in names}
        order,possible=namespace_compile_order(names,files,True)
        self.assertTrue(possible)
        self.assertEqual(order,['A.Fourth.cs','Z.A.Third.cs','Z.B.First.cs','Z.B.Second.cs'])

    def test_unrepresentable_namespace_interleaving_is_explicit(self):
        names=['Z.First','A.Second','Z.Third']
        order,possible=namespace_compile_order(names,{name:name+'.cs' for name in names},True)
        self.assertFalse(possible)


if __name__=='__main__':
    unittest.main()
