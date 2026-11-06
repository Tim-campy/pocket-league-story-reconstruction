"""Slot identity evidence and UTF-16 source edits must remain conservative."""
import unittest
from round7_slot_solver import correspondences, lifetimes, _patch, _edit, proposals


class SlotSolverTests(unittest.TestCase):
    def test_lifetimes_include_address_uses(self):
        method={'instructions':[{'offset':0,'opcode':'stloc.1','operand':None},
            {'offset':2,'opcode':'ldloca.s','operand':{'local':1}},
            {'offset':5,'opcode':'ldloc.1','operand':None}]}
        self.assertEqual(lifetimes(method),[{'slot':1,'first_use':0,'last_use':5,'loads':2,'stores':1}])

    def test_correspondence_uses_types_and_operation(self):
        old={'instructions':[{'offset':0,'opcode':'stloc.0','operand':None},
                            {'offset':1,'opcode':'ldloc.0','operand':None}]}
        new={'instructions':[{'offset':0,'opcode':'stloc.1','operand':None},
                            {'offset':1,'opcode':'ldloc.1','operand':None}]}
        mapping,evidence=correspondences(old,new,['System.String'],['System.Int32','System.String'])
        self.assertEqual(mapping,{1:0})

    def test_whole_method_patch_handles_utf16_prefix(self):
        source='// \U0001f600\nvoid M(){int a=1;}\nvoid N(){}'
        start=len(source[:source.index('void M')].encode('utf-16-le'))//2
        end=len(source[:source.index('\nvoid N')].encode('utf-16-le'))//2
        updated=source.replace('int a=1;','int a; a=1;')
        patch=_patch(source,{'declaration_start':start,'declaration_end':end},updated,'solver-test')
        self.assertEqual(_edit(source,[(patch['start'],patch['end'],patch['after'])]),updated)
        self.assertEqual(patch['before'],'void M(){int a=1;}')

    def test_incomplete_prediction_emits_no_patch_but_retains_lifetimes(self):
        result=proposals('x',{'complete':False},{'instructions':[{'offset':0,'opcode':'stloc.0','operand':None}]},{},[],[])
        self.assertFalse(result['patches'])
        self.assertEqual(result['original_lifetimes'][0]['slot'],0)


if __name__=='__main__':unittest.main()
