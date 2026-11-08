"""Synthetic verifier receipts/files only. No target/model/compiler invocation."""
import copy
import json
from pathlib import Path
import tempfile
import unittest
from unittest.mock import patch
import sys

HERE=Path(__file__).resolve().parent
ROOT=next(p for p in HERE.parents if (p/'AGENTS.md').exists())
sys.path.insert(0,str(ROOT/'tools'))
sys.path.insert(0,str(HERE))
import round12_verification_release as v

def row(held=False):
    return dict(stable_id='Fixture.Generated::.ctor()->Void',state='todo' if held else 'matched',
        body_present=True,body_equal=True,generated_group_pending=held,
        ownership_proof=dict(proved=not held,kind='compiler_static_data_default_ctor',
            origins=['Fixture.Run'],field_ids=['Fixture.Data'],payload_bytes=4,
            matching_holder_metadata=True,matching_backing_layouts=True,matching_consumer_declarations=True,
            implicit_constructor_shape=True,direct_constructor_references=0,
            all_consumer_bodies_equal=True,field_witnesses=[dict(field='Fixture.Data',origin='Fixture.Run',matched_in_both=True)]),
        source_availability_proof=dict(proved=True,kind='compiled_origins_for_generated_data_holder',
                                       source_paths=['Fixture.cs'],origins=['Fixture.Run'],unavailable_origins=[]))

class ReleaseGuards(unittest.TestCase):
    def setUp(self):
        self.before={'assemblies':{a:dict(matched_methods=int(a=='KairoLibrary'),matched_original_il_bytes=7*int(a=='KairoLibrary'),
            divergent_bodies=0,divergent_original_il_bytes=0,equal_body_held_methods=int(a=='Assembly-CSharp'))
            for a in v.ASSEMBLIES}}
        self.after=copy.deepcopy(self.before)
        self.after['assemblies']['Assembly-CSharp'].update(matched_methods=1,matched_original_il_bytes=7,equal_body_held_methods=0)
        self.left={a:dict(methods={'method':row(a=='Assembly-CSharp')}) for a in v.ASSEMBLIES}
        self.right=copy.deepcopy(self.left)
        self.right['Assembly-CSharp']['methods']['method']=row()
        self.sizes={a:{'Fixture.Generated::.ctor()->Void':7} for a in v.ASSEMBLIES}
        for receipts in (self.left,self.right):
            receipts['Assembly-CSharp']['methods']['consumer']=dict(stable_id='Fixture.Run',state='matched',body_present=True,body_equal=True)
        self.sizes['Assembly-CSharp']['Fixture.Run']=2
        self.before={'assemblies':{a:v.receipt_metrics(self.left[a],self.sizes[a]) for a in v.ASSEMBLIES}}
        self.after={'assemblies':{a:v.receipt_metrics(self.right[a],self.sizes[a]) for a in v.ASSEMBLIES}}

    def validate(self):return v.validate_transition(self.before,self.after,self.left,self.right,self.sizes)
    def test_exact_one_seven_release(self):
        result=self.validate();self.assertEqual(len(result['releases']),1)
        self.assertEqual(result['gains']['Assembly-CSharp'],dict(methods_gained=1,il_bytes_gained=7))
    def test_body_divergence_rejected(self):
        self.after['assemblies']['Assembly-CSharp']['divergent_bodies']=1
        with self.assertRaises(ValueError):self.validate()
    def test_held_remaining_rejected(self):
        self.after['assemblies']['Assembly-CSharp']['equal_body_held_methods']=1
        with self.assertRaises(ValueError):self.validate()
    def test_not_a_previously_held_member(self):
        self.left['Assembly-CSharp']['methods']['method']['generated_group_pending']=False
        with self.assertRaises(ValueError):self.validate()
    def test_unproved_origin_rejected(self):
        self.right['Assembly-CSharp']['methods']['method']['ownership_proof']['proved']=False
        with self.assertRaises(ValueError):self.validate()
    def test_wrong_origin_kind_rejected(self):
        self.right['Assembly-CSharp']['methods']['method']['ownership_proof']['kind']='arbitrary_equal_body'
        with self.assertRaises(ValueError):self.validate()
    def test_missing_payload_rejected(self):
        self.right['Assembly-CSharp']['methods']['method']['ownership_proof']['payload_bytes']=0
        with self.assertRaises(ValueError):self.validate()
    def test_unequal_consumer_rejected(self):
        self.right['Assembly-CSharp']['methods']['method']['ownership_proof']['all_consumer_bodies_equal']=False
        with self.assertRaises(ValueError):self.validate()
    def test_source_availability_missing_rejected(self):
        self.right['Assembly-CSharp']['methods']['method']['source_availability_proof']={}
        with self.assertRaises(ValueError):self.validate()
    def test_false_body_rejected(self):
        self.right['Assembly-CSharp']['methods']['method']['body_equal']=False
        with self.assertRaises(ValueError):self.validate()
    def test_matched_generated_member_cannot_still_be_pending(self):
        self.right['Assembly-CSharp']['methods']['method']['generated_group_pending']=True
        with self.assertRaises(ValueError):self.validate()
    def test_matched_consumer_cannot_still_be_pending(self):
        self.right['Assembly-CSharp']['methods']['consumer']['generated_group_pending']=True
        with self.assertRaises(ValueError):self.validate()
    def test_unverified_bodyless_declaration_rejected(self):
        self.right['KairoLibrary']['methods']['other']=dict(stable_id='Fixture.NoBody',body_present=False,state='matched',metadata_declaration_equal=False)
        self.left['KairoLibrary']['methods']['other']=copy.deepcopy(self.right['KairoLibrary']['methods']['other'])
        with self.assertRaises(ValueError):self.validate()
    def test_new_method_rejected(self):
        self.right['KairoLibrary']['methods']['other']=dict(stable_id='other',state='matched',body_present=True,body_equal=True)
        with self.assertRaises(ValueError):self.validate()
    def test_unexpected_gain_rejected(self):
        self.after['assemblies']['Assembly-CSharp']['matched_original_il_bytes']=8
        with self.assertRaises(ValueError):self.validate()
    def test_wrong_authorized_release_size_rejected(self):
        self.sizes['Assembly-CSharp']['Fixture.Generated::.ctor()->Void']=8
        self.after['assemblies']['Assembly-CSharp']['matched_original_il_bytes']=10
        with self.assertRaises(ValueError):self.validate()
    def test_unmatched_origin_rejected(self):
        self.right['Assembly-CSharp']['methods']['consumer']['state']='todo'
        self.after['assemblies']['Assembly-CSharp']=v.receipt_metrics(self.right['Assembly-CSharp'],self.sizes['Assembly-CSharp'])
        with self.assertRaises(ValueError):self.validate()
    def test_reviewed_certificate_binding(self):
        released=self.validate()['releases'][0]
        certificate=dict(proved=True,method_id=released['method_id'],original_il_bytes=7,
            ownership_proof=released['ownership_proof'],source_availability_proof=released['source_availability_proof'],
            generic_rule_sha256='generic',sidecar_sha256='sidecar')
        authority={'Assembly-CSharp':dict(helper_source_hashes={'tools/generated_groups.py':'generic',
            'tools/verify_csharp_sidecar.py':'sidecar'})}
        v.bind_reviewed_origin(certificate,released,authority)
        certificate['method_id']='other'
        with self.assertRaises(ValueError):v.bind_reviewed_origin(certificate,released,authority)
    def test_profile_source_and_type_drift(self):
        authority={a:dict(profile={'flags':['same']},sources=[dict(path='Fixture.cs',sha256='hash')],
            helper_source_hashes={'helper':'hash'},types_equal=1,types_total=1) for a in v.ASSEMBLIES}
        for key in ('profile','sources','types_total'):
            other=copy.deepcopy(authority);other['KairoLibrary'][key]=None
            with self.subTest(key=key),self.assertRaises(ValueError):v.compare_authority(authority,other)
    def test_verifier_change_rejected_on_refresh(self):
        authority={a:dict(profile={},sources=[],helper_source_hashes={'helper':'hash'},types_equal=1,types_total=1) for a in v.ASSEMBLIES}
        other=copy.deepcopy(authority);other['KairoLibrary']['helper_source_hashes']={}
        with self.assertRaises(ValueError):v.compare_authority(authority,other,refresh=True)

class PointerGuards(unittest.TestCase):
    def setUp(self):
        self.tmp=tempfile.TemporaryDirectory();self.addCleanup(self.tmp.cleanup);self.root=Path(self.tmp.name)
        self.journal={'cycles':[dict(label='b39',phase='B',after_measurement='before.json',after_sha256='before-sha')]}
        (self.root/'reports').mkdir();(self.root/'config').mkdir()
        (self.root/'config/methods.csv').write_text('synthetic ledger')
        v.save(self.root/'reports/round12-cycle-journal.json',self.journal)
        self.m=dict(snapshot='snapshot',ledger_sha256=v.sha(self.root/'config/methods.csv'),assemblies={a:
            dict(matched_methods=4,matched_original_il_bytes=107,divergent_bodies=0,
                 divergent_original_il_bytes=0,equal_body_held_methods=0) for a in v.ASSEMBLIES})
        v.save(self.root/'measurement.json',self.m)
        self.authority={a:dict(profile={},sources=[],types_equal=1,types_total=1,
            helper_source_hashes={},receipt_sha256='receipt',build_sha256='pe',receipt_metrics=v.metrics(self.m)[a]) for a in v.ASSEMBLIES}
        (self.root/'origin.json').write_text('synthetic proof')
        core=dict(after_metrics=v.metrics(self.m),after_authority=self.authority,producer_sha256=v.sha(Path(v.__file__)),
            origin_proof=dict(path='origin.json',sha256=v.sha(self.root/'origin.json')),anchor=dict(
            journal_sha256=v.sha(self.root/'reports/round12-cycle-journal.json'),last_row_sha256=v.digest(self.journal['cycles'][-1]),
            label='b39',after_measurement='before.json',after_measurement_sha256='before-sha',B_cycles=1))
        self.event=dict(event_core=core,event_core_sha256=v.digest(core),commit_provenance=[dict(
            measurement='measurement.json',measurement_sha256=v.sha(self.root/'measurement.json'),snapshot='snapshot',authority=self.authority)])
        v.save(self.root/'event.json',self.event)
        self.cfg=dict(verification_origin_release=dict(record='event.json',sha256=v.sha(self.root/'event.json')),
            verification_current_measurement=dict(path='measurement.json',sha256=v.sha(self.root/'measurement.json'),
                snapshot='snapshot',event_record_sha256=v.sha(self.root/'event.json')))
    def validate(self,authority=None):
        with patch.object(v,'source_authority',return_value=self.authority if authority is None else authority):
            return v.validate_pointer(self.root,self.cfg,self.journal)
    def test_valid_explicit_pointer(self):self.assertEqual(self.validate(),self.m)
    def test_event_hash_changed(self):
        with (self.root/'event.json').open('a') as f:f.write(' ')
        with self.assertRaises(ValueError):self.validate()
    def test_journal_changed(self):
        self.journal['cycles'][0]['label']='b40';v.save(self.root/'reports/round12-cycle-journal.json',self.journal)
        with self.assertRaises(ValueError):self.validate()
    def test_measurement_changed(self):
        with (self.root/'measurement.json').open('a') as f:f.write(' ')
        with self.assertRaises(ValueError):self.validate()
    def test_ledger_changed(self):
        (self.root/'config/methods.csv').write_text('different')
        with self.assertRaises(ValueError):self.validate()
    def test_pe_authority_changed(self):
        other=copy.deepcopy(self.authority);other['Assembly-CSharp']['build_sha256']='new-nonce'
        with self.assertRaises(ValueError):self.validate(other)
    def test_source_profile_changed(self):
        other=copy.deepcopy(self.authority);other['Assembly-CSharp']['profile']={'flags':['different']}
        with self.assertRaises(ValueError):self.validate(other)
    def test_receipt_counts_disagree(self):
        other=copy.deepcopy(self.authority);other['Assembly-CSharp']['receipt_metrics']['matched_methods']=0
        with self.assertRaises(ValueError):self.validate(other)
    def test_reviewed_origin_changed(self):
        (self.root/'origin.json').write_text('different')
        with self.assertRaises(ValueError):self.validate()
    def test_pointer_cannot_choose_old_measurement(self):
        self.cfg['verification_current_measurement']['path']='different.json'
        with self.assertRaises(ValueError):self.validate()
    def test_windows_snapshot_spelling_roundtrip(self):
        self.m['snapshot']='build\\round12\\snapshot'
        v.save(self.root/'measurement.json',self.m)
        entry=self.event['commit_provenance'][-1]
        entry.update(snapshot=self.m['snapshot'],measurement_sha256=v.sha(self.root/'measurement.json'))
        v.save(self.root/'event.json',self.event)
        event_sha=v.sha(self.root/'event.json')
        self.cfg['verification_origin_release']['sha256']=event_sha
        self.cfg['verification_current_measurement'].update(snapshot=self.m['snapshot'],
            sha256=v.sha(self.root/'measurement.json'),event_record_sha256=event_sha)
        self.assertEqual(self.validate()['snapshot'],self.m['snapshot'])

class BodyInventoryGuards(unittest.TestCase):
    def setUp(self):
        self.tmp=tempfile.TemporaryDirectory();self.addCleanup(self.tmp.cleanup);root=Path(self.tmp.name)
        self.left=root/'before';self.right=root/'after'
        method=dict(stable_id='Fixture.Zero()->Int32',header=dict(format='tiny',code_size=2),locals=[],
                    instructions=[dict(offset=0,opcode='ldc.i4.0',operand=None),dict(offset=1,opcode='ret',operand=None)],
                    exception_regions=[],extra_sections=[],raw_body_hex='0A-16-2A',metadata_operands=[])
        for base in (self.left,self.right):
            for assembly in v.ASSEMBLIES:
                (base/assembly).mkdir(parents=True)
                v.save(base/assembly/'rebuilt-methods.json',dict(selected_methods=[method]))
    def mutate(self,field,value):
        p=self.right/'Assembly-CSharp/rebuilt-methods.json';data=v.load(p)
        data['selected_methods'][0][field]=value;v.save(p,data)
    def test_whole_body_components_equal(self):
        result=v.body_inventory_proof(self.left,self.right)
        self.assertTrue(all(r['all_body_components_unchanged'] for r in result.values()))
    def test_header_change_rejected(self):
        self.mutate('header',dict(format='fat',maxstack=2))
        with self.assertRaises(ValueError):v.body_inventory_proof(self.left,self.right)
    def test_local_change_rejected(self):
        self.mutate('locals',['Int32'])
        with self.assertRaises(ValueError):v.body_inventory_proof(self.left,self.right)
    def test_eh_change_rejected(self):
        self.mutate('exception_regions',[dict(kind='finally',try_start=0,try_end=1)])
        with self.assertRaises(ValueError):v.body_inventory_proof(self.left,self.right)
    def test_raw_byte_change_rejected(self):
        self.mutate('raw_body_hex','0A-17-2A')
        with self.assertRaises(ValueError):v.body_inventory_proof(self.left,self.right)

class SnapshotInputGuards(unittest.TestCase):
    def setUp(self):
        self.tmp=tempfile.TemporaryDirectory();self.addCleanup(self.tmp.cleanup)
        self.root=Path(self.tmp.name);self.snapshot=self.root/'snap';self.snapshot.mkdir()
        (self.snapshot/'methods.csv').write_text('frozen ledger')
        digest=v.sha(self.snapshot/'methods.csv')
        v.save(self.snapshot/'snapshot.json',dict(ledger_sha256=digest))
        self.measurement=dict(baseline_commit=v.BASE_COMMIT,snapshot='snap',ledger_sha256=digest,assemblies={})
        (self.snapshot/'assemblies').mkdir();artifacts={}
        for assembly in v.ASSEMBLIES:
            folder=self.snapshot/assembly;folder.mkdir()
            proof={}
            for name,file,key in [('original_dump','original-methods.json','original_dump_sha256'),
                    ('rebuilt_dump','rebuilt-methods.json','rebuilt_dump_sha256'),('receipt','verification.json','receipt_sha256')]:
                v.save(folder/file,dict(synthetic=True))
                proof.update({name:'snap/'+assembly+'/'+file,key:v.sha(folder/file)})
            proof['build_sha256']='PE';artifacts[assembly]=dict(path=assembly+'.fixture',sha256='PE')
            self.measurement['assemblies'][assembly]=dict(provenance=proof)
        v.save(self.snapshot/'assemblies/artifacts.json',artifacts)
    def validate(self):
        with patch.object(v,'ROOT',self.root):return v.bind_snapshot_inputs(self.snapshot,self.measurement)
    def test_exact_frozen_inputs_accepted(self):self.assertEqual(set(self.validate()),set(v.ASSEMBLIES))
    def test_original_dump_tamper_rejected(self):
        (self.snapshot/'Assembly-CSharp/original-methods.json').write_text('{}')
        with self.assertRaises(ValueError):self.validate()
    def test_rebuilt_dump_tamper_rejected(self):
        (self.snapshot/'Assembly-CSharp/rebuilt-methods.json').write_text('{}')
        with self.assertRaises(ValueError):self.validate()
    def test_receipt_tamper_rejected(self):
        (self.snapshot/'Assembly-CSharp/verification.json').write_text('{}')
        with self.assertRaises(ValueError):self.validate()
    def test_dump_path_outside_snapshot_rejected(self):
        self.measurement['assemblies']['Assembly-CSharp']['provenance']['original_dump']='snap/KairoLibrary/original-methods.json'
        with self.assertRaises(ValueError):self.validate()
    def test_wrong_pe_link_rejected(self):
        self.measurement['assemblies']['Assembly-CSharp']['provenance']['build_sha256']='different'
        with self.assertRaises(ValueError):self.validate()
    def test_wrong_round_rejected(self):
        self.measurement['baseline_commit']='other'
        with self.assertRaises(ValueError):self.validate()
    def test_frozen_ledger_tamper_rejected(self):
        (self.snapshot/'methods.csv').write_text('different')
        with self.assertRaises(ValueError):self.validate()

class PdbNonceGuards(unittest.TestCase):
    def test_nonce_evidence_is_not_configuration(self):
        before=dict(flags=['/debug:pdbonly'],pdb_output=dict(compiler_output_path='E:/same.pdb',
            configuration_sha256='configuration',pdb_sha256='old',alias_created_this_build=False))
        after=copy.deepcopy(before);after['pdb_output'].update(pdb_sha256='new',alias_created_this_build=True)
        self.assertEqual(v.compilation_profile(before),v.compilation_profile(after))
        self.assertNotEqual(before['pdb_output'],after['pdb_output'])
    def test_pdb_configuration_change_remains_visible(self):
        before=dict(pdb_output=dict(compiler_output_path='E:/same.pdb',pdb_sha256='old'))
        after=copy.deepcopy(before);after['pdb_output']['compiler_output_path']='E:/different.pdb'
        self.assertNotEqual(v.compilation_profile(before),v.compilation_profile(after))

if __name__=='__main__':unittest.main(verbosity=2)
