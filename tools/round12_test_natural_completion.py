"""Exercise a PRIVATE proposed stop guard; no production file or target changes."""
import copy,json,sys,unittest
from pathlib import Path
from unittest.mock import patch
W=Path(__file__).resolve().parent;R=next(p for p in W.parents if(p/'AGENTS.md').exists());sys.path.insert(0,str(R/'tools'))
import apply_il_rules
SOURCE=R/'tools/round12_controller.py'

class CompletionTests(unittest.TestCase):
 def setUp(self):
  self.ns={'__file__':str(SOURCE),'__name__':'private_controller_proposal'};exec(compile(SOURCE.read_text(encoding='utf-8'),str(SOURCE),'exec'),self.ns)
  self.cfg={'phase':'part_B','required_parts':{'A':'complete'}};self.journal={'cycles':[{'phase':'B','after_measurement':'measured','after_sha256':'proof'}]};self.m={'ledger_sha256':'ledger','assemblies':{}};self.receipts={}
  for a,methods,types in [('KairoLibrary',3287,337),('Assembly-CSharp',1958,129)]:
   self.m['assemblies'][a]={'total_methods':methods,'matched_methods':methods,'divergent_bodies':0,'equal_body_held_methods':0,'matched_original_il_bytes':123,'original_total_il_bytes':123,'provenance':{'build_sha256':a}}
   self.receipts[a]={'types_equal':types,'types_total':types,'build_sha256':a,'methods':{str(i):{'state':'matched','body_present':True,'body_equal':True,'generated_group_pending':False}for i in range(methods)},'helper_source_hashes':{}}
  self.ns['current_measurement']=lambda cfg,journal:self.m;self.ns['file_hash']=lambda p:'ledger';self.ns['load']=lambda p:self.receipts['KairoLibrary'if'KairoLibrary'in str(p)else'Assembly-CSharp'];self.raw=[];self.ns['verify_raw_gate']=lambda cfg,m:self.raw.append(True)
  self.fresh=patch.object(apply_il_rules,'receipt_is_fresh',return_value=True);self.fresh.start();self.addCleanup(self.fresh.stop)
 def complete(self):return self.ns['measured_semantic_completion'](self.cfg,self.journal)
 def test_fully_verified_closure(self):self.assertTrue(self.complete());self.assertEqual(self.raw,[True])
 def test_pending_body_does_not_stop(self):self.m['assemblies']['Assembly-CSharp']['divergent_bodies']=1;self.assertFalse(self.complete());self.assertFalse(self.raw)
 def test_held_helper_does_not_stop(self):self.m['assemblies']['Assembly-CSharp']['equal_body_held_methods']=1;self.assertFalse(self.complete())
 def test_incomplete_il_does_not_stop(self):self.m['assemblies']['Assembly-CSharp']['matched_original_il_bytes']-=1;self.assertFalse(self.complete())
 def test_wrong_inventory_does_not_stop(self):self.m['assemblies']['Assembly-CSharp']['total_methods']=0;self.m['assemblies']['Assembly-CSharp']['matched_methods']=0;self.assertFalse(self.complete())
 def test_stale_ledger_refused(self):self.m['ledger_sha256']='stale';self.assertRaises(self.ns['GuardError'],self.complete)
 def test_stale_source_receipt_refused(self):
  with patch.object(apply_il_rules,'receipt_is_fresh',return_value=False):self.assertRaises(self.ns['GuardError'],self.complete)
 def test_wrong_pe_refused(self):self.receipts['Assembly-CSharp']['build_sha256']='other';self.assertRaises(self.ns['GuardError'],self.complete)
 def test_type_mismatch_refused(self):self.receipts['Assembly-CSharp']['types_equal']-=1;self.assertRaises(self.ns['GuardError'],self.complete)
 def test_unverified_body_cannot_hide_behind_matched_state(self):self.receipts['Assembly-CSharp']['methods']['0']['body_equal']=False;self.assertRaises(self.ns['GuardError'],self.complete)
 def test_bodyless_declaration_requires_metadata(self):self.receipts['Assembly-CSharp']['methods']['0']['body_present']=False;self.assertRaises(self.ns['GuardError'],self.complete)
 def test_verified_bodyless_declaration(self):self.receipts['Assembly-CSharp']['methods']['0'].update(body_present=False,metadata_declaration_equal=True);self.assertTrue(self.complete())
 def test_changed_verifier_helper_refused(self):self.receipts['Assembly-CSharp']['helper_source_hashes']={'tools/verify':'stale'};self.assertRaises(self.ns['GuardError'],self.complete)
 def test_raw_gate_remains_required(self):
  def reject(cfg,m):raise self.ns['GuardError']('raw stale')
  self.ns['verify_raw_gate']=reject;self.assertRaises(self.ns['GuardError'],self.complete)
 def test_part_a_alone_does_not_stop(self):self.journal['cycles']=[{'phase':'A'}];self.assertFalse(self.complete())
 def test_part_a_phase_does_not_stop(self):self.cfg['phase']='part_A';self.assertFalse(self.complete())
 def test_incomplete_journal_does_not_stop(self):self.journal['cycles'][0].pop('after_sha256');self.assertFalse(self.complete())

if __name__=='__main__':
 suite=unittest.defaultTestLoader.loadTestsFromTestCase(CompletionTests);result=unittest.TextTestRunner(verbosity=1).run(suite)
 output={'scope':'installed natural-completion guard tests only','production_mutations':0,'target_compilations':0,'tests':result.testsRun,'failures':len(result.failures),'errors':len(result.errors)};(R/'reports/round12-natural-completion-guard-tests.json').write_text(json.dumps(output,indent=2)+'\n',encoding='utf-8');sys.exit(not result.wasSuccessful())
