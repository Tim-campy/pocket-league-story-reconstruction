"""Independent guards for read-only local-slot diagnostic extraction."""
import unittest

from round5_local_cases import access_counts, classify, initial_zero_chain, local_operation, source_depths


def i(offset,opcode,operand=None):
    return {'offset':offset,'opcode':opcode,'operand':operand}


class LocalDiagnosticTests(unittest.TestCase):
    def test_explicit_and_implicit_slots_keep_address_uses_separate(self):
        m={'locals':['A','B','C','D','E'], 'instructions':[
            i(0,'stloc.3'),i(1,'ldloc.s',{'local':4}),i(3,'ldloca.s',{'local':4})]}
        counts=access_counts(m)
        self.assertEqual(counts[3],{'loads':0,'stores':1,'address_loads':0})
        self.assertEqual(counts[4],{'loads':1,'stores':0,'address_loads':1})
        self.assertIsNone(local_operation(i(5,'ldarg.3')))

    def test_zero_chain_excludes_later_independent_zero_store(self):
        m={'instructions':[i(0,'ldc.i4.0'),i(1,'dup'),i(2,'stloc.3'),i(3,'dup'),i(4,'stloc.2'),
                           i(5,'dup'),i(6,'stloc.1'),i(7,'stloc.0'),i(8,'ldc.i4.0'),i(9,'stloc.s',{'local':4})]}
        self.assertEqual(initial_zero_chain(m),{'offset':0,'slots':[3,2,1,0]})

    def test_scope_depth_ignores_regular_verbatim_char_and_comment_braces(self):
        text='void F() { string s="{int x;}"; string p=@"{int x;\"\"}"; char c=\'{\'; /* { int x; } */\nint x; // { int x;\nif(true) { int x=2; } }'
        self.assertEqual([x['brace_depth'] for x in source_depths(text,'x')],[1,2])

    def test_unused_slot_is_distinguished_from_omitted_store(self):
        old={'locals':['View','Int32','Int32','Int32'], 'instructions':[i(0,'stloc.0'),i(1,'ldloc.0'),i(2,'stloc.2'),i(3,'stloc.3')]}
        new={'locals':['View','Int32','Int32'], 'instructions':[i(0,'stloc.0'),i(1,'ldloc.0'),i(2,'stloc.2'),i(3,'stloc.1')]}
        d=classify('system.form.LanguageForm::DrawButton(X)->System.Void',old,new,{'text':''},[])
        self.assertIn('omitted_unused_declaration',d['classification'])
        self.assertNotIn('omitted_assignment',d['classification'])
        self.assertEqual(d['original_slot_accesses'][1],{'loads':0,'stores':0,'address_loads':0})


if __name__=='__main__':unittest.main()
