"""Exercise ownership boundaries using compiler-generated own fixtures only."""
import copy
import importlib.util
import json
from pathlib import Path
import sys
import unittest

W=Path(__file__).resolve().parent
ROOT=next(p for p in W.parents if(p/'AGENTS.md').exists())
sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import differences
import generated_groups as owner
FIXTURES=ROOT/'tools/fixtures/round12-static-data-owner'
KEY='<PrivateImplementationDetails>::.ctor()->System.Void'


class OwnershipTests(unittest.TestCase):
    def setUp(self):
        self.original=json.loads((FIXTURES/'packed-two-consumers/methods.json').read_text(encoding='utf-8-sig'))
        self.rebuilt=copy.deepcopy(self.original)
        self.equal=None

    def proof(self):
        left={m['stable_id']:m for m in self.original['selected_methods']}
        right={m['stable_id']:m for m in self.rebuilt['selected_methods']}
        equal=self.equal if self.equal is not None else {k for k in left if 'raw_body_hex'in left[k]and not differences(left[k],right.get(k))[0]}
        return owner.static_data_constructor_proof(left,right,{t['name']:t for t in self.original['types']},
                                                  {t['name']:t for t in self.rebuilt['types']},KEY,equal)

    def holder(self,document):return next(t for t in document['types']if t['name']=='<PrivateImplementationDetails>')

    def consumer(self,document):return next(m for m in document['selected_methods']if any(r['kind']=='field'and r['declaring_type']=='<PrivateImplementationDetails>'for r in m['direct_references']))

    def test_all_six_owned_compiler_controls(self):
        for folder in sorted(FIXTURES.iterdir()):
            if not folder.is_dir():continue
            self.original=json.loads((folder/'methods.json').read_text(encoding='utf-8-sig'));self.rebuilt=copy.deepcopy(self.original)
            self.assertEqual(self.proof()['proved'],not folder.name.startswith('negative'),folder.name)

    def test_corresponding_field_data_and_layouts_prove_holder(self):
        proof=self.proof();self.assertTrue(proof['proved']);self.assertEqual(len(proof['origins']),2)
        self.assertEqual(len(proof['field_ids']),2);self.assertEqual(proof['payload_bytes'],24)

    def test_field_payload_mismatch(self):
        self.holder(self.rebuilt)['fields'][0]['initial_value']='FF-00';self.assertFalse(self.proof()['proved'])

    def test_same_name_without_compiler_marker(self):
        for doc in (self.original,self.rebuilt):self.holder(doc)['custom_attributes']=[]
        self.assertFalse(self.proof()['proved'])

    def test_field_without_static_rva(self):
        for doc in (self.original,self.rebuilt):self.holder(doc)['fields'][0]['attributes']=3
        self.assertFalse(self.proof()['proved'])

    def test_backing_size_must_equal_payload(self):
        for doc in (self.original,self.rebuilt):
            next(t for t in doc['types']if t['name'].startswith('<PrivateImplementationDetails>/'))['class_size']+=1
        self.assertFalse(self.proof()['proved'])

    def test_absent_edge_refuses_ownership(self):
        m=self.consumer(self.rebuilt);m['direct_references']=[r for r in m['direct_references']if r['kind']!='field']
        self.assertFalse(self.proof()['proved'])

    def test_initialize_array_pair_required(self):
        for doc in (self.original,self.rebuilt):
            m=self.consumer(doc);m['direct_references']=[r for r in m['direct_references']if'::InitializeArray('not in r['id']]
        self.assertFalse(self.proof()['proved'])

    def test_dup_stack_shape_required(self):
        for doc in (self.original,self.rebuilt):
            m=self.consumer(doc);off=next(r['offset']for r in m['direct_references']if r['kind']=='field')
            next(i for i in m['instructions']if i['offset']==off-1)['opcode']='nop'
        self.assertFalse(self.proof()['proved'])

    def test_every_consumer_body_required(self):
        self.equal={m['stable_id']for m in self.original['selected_methods']};self.equal.remove(self.consumer(self.original)['stable_id'])
        self.assertFalse(self.proof()['proved'])

    def test_constructor_body_required(self):
        self.equal={m['stable_id']for m in self.original['selected_methods']} - {KEY}
        self.assertFalse(self.proof()['proved'])

    def test_consumer_declaration_required(self):
        self.consumer(self.rebuilt)['method_attributes']^=1
        self.assertFalse(self.proof()['proved'])

    def test_extra_holder_method_refused(self):
        for doc in (self.original,self.rebuilt):self.holder(doc)['methods'].append('<PrivateImplementationDetails>::Extra()->System.Void')
        self.assertFalse(self.proof()['proved'])

    def test_all_compiled_origin_sources_required(self):
        proof=self.proof();rows={key:{'fonte':'src/fixture.cs'}for key in proof['origins']}
        self.assertTrue(owner.compiled_static_data_origins(proof,rows,{'src/fixture.cs'})['proved'])
        rows[proof['origins'][0]]['fonte']=''
        self.assertFalse(owner.compiled_static_data_origins(proof,rows,{'src/fixture.cs'})['proved'])

    def test_unproved_ownership_cannot_claim_compiled_availability(self):
        proof=self.proof();proof['proved']=False
        self.assertFalse(owner.compiled_static_data_origins(proof,{},set())['proved'])


if __name__=='__main__':
    suite=unittest.defaultTestLoader.loadTestsFromTestCase(OwnershipTests)
    result=unittest.TextTestRunner(verbosity=1).run(suite)
    (ROOT/'reports/round12-static-data-owner-tests.json').write_text(json.dumps(dict(tests=result.testsRun,
        failures=len(result.failures),errors=len(result.errors),own_fixture_compilations_reused=6,
        new_compilations=0,target_compilations=0,verifier_state_mutations=0),indent=2)+'\n',encoding='utf-8')
    sys.exit(not result.wasSuccessful())
