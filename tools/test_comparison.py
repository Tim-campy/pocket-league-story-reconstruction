"""Negative controls for the method-body equality boundary and receipt guards."""
import copy
import json
from pathlib import Path
import tempfile
import unittest
from compare_kairo import body_cells, differences
from verification_state import digest, validate_matched
from generated_groups import prove_groups


class ComparisonTests(unittest.TestCase):
    def test_private_data_field_proof_with_generated_constructor(self):
        owner='<PrivateImplementationDetails>'
        root='T::Load()->System.Void';constructor=owner+'::.ctor()->System.Void'
        field={'id':owner+'::Data','name':'Data','initial_value':'01-02','type':'bytes'}
        doc={'selected_methods':[{'stable_id':root,'direct_references':[
            {'offset':0,'opcode':'ldtoken','kind':'field','id':field['id'],'declaring_type':owner}]},
            {'stable_id':constructor,'direct_references':[]}],
            'types':[{'name':'T','fields':[],'properties':[]},{'name':owner,'fields':[field],'properties':[]}]}
        proof,_=prove_groups(doc,copy.deepcopy(doc),{constructor})
        self.assertTrue(proof[root]['proved'])
        other=copy.deepcopy(doc);other['types'][1]['fields'][0]['initial_value']='01-03'
        proof,_=prove_groups(doc,other,{constructor})
        self.assertFalse(proof[root]['proved'])

    def test_shared_cache_helpers_wait_for_every_lambda_body(self):
        owner='T/<>c';root='T::Run()->System.Void';method=owner+'::<Run>b__0_0()->System.Void'
        constructor=owner+'::.ctor()->System.Void'
        doc={'selected_methods':[{'stable_id':root,'direct_references':[
            {'offset':0,'opcode':'ldftn','kind':'method','id':method,'declaring_type':owner}]},
            {'stable_id':method,'direct_references':[]},{'stable_id':constructor,'direct_references':[]}],
            'types':[{'name':'T','fields':[],'properties':[]},{'name':owner,'fields':[],'properties':[]}]}
        proof,_=prove_groups(doc,copy.deepcopy(doc),{method,constructor},{root,constructor})
        self.assertFalse(proof[constructor]['proved'])
        proof,_=prove_groups(doc,copy.deepcopy(doc),{method,constructor},{root,constructor,method})
        self.assertTrue(proof[constructor]['proved'])

    def test_generated_ownership_requires_matching_il_edge_and_name(self):
        root='T::Run()->System.Void';generated='T/<>c::<Run>b__0_0()->System.Void'
        missing='T/<>c::<Other>b__1_0()->System.Void'
        edge={'offset':0,'opcode':'ldftn','kind':'method','id':generated,'declaring_type':'T/<>c'}
        doc={'selected_methods':[{'stable_id':root,'direct_references':[edge]},
            {'stable_id':generated,'direct_references':[]},{'stable_id':missing,'direct_references':[]}],
            'types':[{'name':'T','fields':[],'properties':[]},{'name':'T/<>c','fields':[],'properties':[]}]}
        built=copy.deepcopy(doc);built['selected_methods'].pop()
        proof,_=prove_groups(doc,built,{generated,missing})
        self.assertTrue(proof[generated]['proved'])
        self.assertFalse(proof[missing]['proved'])
        built['selected_methods'][0]['direct_references']=[]
        proof,_=prove_groups(doc,built,{generated,missing})
        self.assertFalse(proof[generated]['proved'])

    def test_only_resolved_token_bytes_can_differ(self):
        method={'raw_body_hex':'16-28-01-00-00-0A-2A','metadata_operands':[
            {'body_offset':2,'width':4,'identity':{'method':'[Core]Type::Call()'}}]}
        other=copy.deepcopy(method);other['raw_body_hex']='16-28-99-00-00-0A-2A'
        self.assertEqual(body_cells(method),body_cells(other))
        other['metadata_operands'][0]['identity']['method']='[Other]Type::Call()'
        self.assertNotEqual(body_cells(method),body_cells(other))

    def test_control_flow_header_and_exception_bytes_are_strict(self):
        method={'raw_body_hex':'13-30-08-00-03-00-00-00-00-00-00-00-00-2B-00-2A-01-00-00-00',
                'metadata_operands':[]}
        for offset in (0,2,14,18):
            other=copy.deepcopy(method);data=bytearray.fromhex(other['raw_body_hex'].replace('-',''))
            data[offset]^=1;other['raw_body_hex']=data.hex()
            self.assertIn('resolved_body_bytes',differences(method,other)[0])

    def test_stale_source_and_manual_promotions_are_rejected(self):
        with tempfile.TemporaryDirectory() as directory:
            root=Path(directory);(root/'build').mkdir();(root/'tools').mkdir()
            for name in ('tools/verify_kairo.py','tools/ProbeInspector.cs','source.cs','build/test.dll'):
                (root/name).write_text('fixture')
            receipt={'verifier_sha256':digest(root/'tools/verify_kairo.py'),
                'inspector_source_sha256':digest(root/'tools/ProbeInspector.cs'),
                'build_artifact':'build/test.dll','build_sha256':digest(root/'build/test.dll'),
                'sources':[{'path':'source.cs','sha256':digest(root/'source.cs')}],
                'methods':{'A::M()':{'state':'matched','body_equal':True,'generated_group_pending':False}}}
            (root/'build/verification-KairoLibrary.json').write_text(json.dumps(receipt))
            row={'assembly':'KairoLibrary','id':'A::M()','estado':'matched','percentual':'100.0000'}
            validate_matched(root,[row])
            with self.assertRaises(ValueError): validate_matched(root,[dict(row,id='A::Other()')])
            (root/'source.cs').write_text('changed')
            with self.assertRaises(ValueError): validate_matched(root,[row])

    def test_metadata_flag_cannot_replace_body_or_missing_declaration_proof(self):
        with tempfile.TemporaryDirectory() as directory:
            root=Path(directory)
            for name in ('build','tools','config'):(root/name).mkdir()
            for name in ('tools/verify_kairo.py','tools/ProbeInspector.cs','source.cs','build/test.dll'):
                (root/name).write_text('fixture')
            (root/'config/types.csv').write_text('assembly,tipo,ordem,metadados\n')
            result={'state':'matched','body_equal':False,'generated_group_pending':False,
                    'metadata_declaration_equal':True,'body_present':True,'stable_id':'T::M()->System.Void'}
            receipt={'verifier_sha256':digest(root/'tools/verify_kairo.py'),
                'inspector_source_sha256':digest(root/'tools/ProbeInspector.cs'),
                'build_artifact':'build/test.dll','build_sha256':digest(root/'build/test.dll'),
                'sources':[{'path':'source.cs','sha256':digest(root/'source.cs')}],
                'methods':{'A::M()':result}}
            row={'assembly':'KairoLibrary','id':'A::M()','estado':'matched','percentual':'100.0000'}
            path=root/'build/verification-KairoLibrary.json';path.write_text(json.dumps(receipt))
            with self.assertRaisesRegex(ValueError,'cannot replace a method body'):validate_matched(root,[row])
            result['body_present']=False;path.write_text(json.dumps(receipt))
            with self.assertRaisesRegex(ValueError,'lacks a current metadata proof'):validate_matched(root,[row])


if __name__=='__main__': unittest.main()
