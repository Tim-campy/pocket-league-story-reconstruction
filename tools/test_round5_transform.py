"""Round-5 syntax/safety controls; game equality is measured separately."""
import json
from pathlib import Path
from round5_transform import TransformClient

ROOT=Path(__file__).resolve().parents[1]

def main():
    source='''// \U0001f50d UTF-16 probe
class Round5Fixture {
 int field; volatile int changing;
 int Alias(int[] terms) { int n=terms[1]; return n+terms[2]; }
 int Written(int[] terms) { terms=null; return terms[1]; }
 int Ref(ref int[] terms) { return terms[1]; }
 int Field() { return field+1; }
 int Volatile() { return changing+1; }
 void Loop(int[][] sprites) { foreach(int[] row in sprites) { Use(row); } }
 void Goal() { int a; int b; int c=(a=(b=0)); int other=0; int future=-13; c=17; Use(a,b,c,other,future); }
 void Unused() { int y; int x=20; y=20; Use(x,y); }
 void MissingDead() { int a; int b; int c=(a=(b=0)); Use(a,b,c); }
 void Shared(int kind) { int a; int b; int c=(a=(b=0)); if(kind==0) {int x=20;Use(x);} else {int x=30;Use(x);} Use(c); }
 void Captured(int kind) { int a; int b; int c=(a=(b=0)); if(kind==0) {int x=20;System.Action f=delegate {Use(x);};f();} else {int x=30;Use(x);} }
 int collision;
 void Collision(int kind) { int a; int b; int c=(a=(b=0)); Use(collision);if(kind==0) {int collision=20;Use(collision);} else {int collision=30;Use(collision);} }
 void Use(params object[] args) {}
}
'''
    path=ROOT/'build/round5-transform/SafetyFixture.cs';path.parent.mkdir(parents=True,exist_ok=True);path.write_text(source,encoding='utf-8',newline='\n')
    checks=[]
    with TransformClient(ROOT/'build/round5/baseline/Assembly-CSharp/build-manifest.json') as client:
      def query(needle,**hints):
        n=source.index(needle)
        response=client.request(path,source=source,offset=len(source[:n].encode('utf-16-le'))//2,max_candidates=512,**hints)
        assert not response.get('error'),response
        return response
      def has(response,rule,expected=None):
        found=[p for p in response['patches'] if p['rule']==rule]
        assert found,rule
        if expected:assert any(expected in p['after'] for p in found),(rule,found)
        for p in found:
          assert source.encode('utf-16-le')[2*p['start']:2*p['end']].decode('utf-16-le')==p['before']
        checks.append(rule)
      alias=query('int n=terms[1];',expected_alias_type='System.Int32[]')
      has(alias,'alias-parameter-occurrence','termsAlias = terms;')
      has(alias,'alias-parameter-block-tail','return n+termsAlias[2]')
      assert not any(p['rule'].startswith('alias-') for p in query('int n=terms[1];')['patches']);checks.append('alias-requires-original-type-hint')
      assert not any(p['rule'].startswith('alias-parameter') for p in query('terms=null;',expected_alias_type='System.Int32[]')['patches']);checks.append('written-parameter-rejected')
      assert not any(p['rule'].startswith('alias-parameter') for p in query('return terms[1];',expected_alias_type='System.Int32[]')['patches']);checks.append('ref-parameter-rejected')
      has(query('return field+1;',expected_alias_type='System.Int32'),'alias-field-occurrence','fieldAlias = field;')
      assert not any(p['rule'].startswith('alias-field') for p in query('return changing+1;',expected_alias_type='System.Int32')['patches']);checks.append('volatile-field-rejected')
      has(query('foreach(int[] row'),'foreach-array-indexed-for','spriteIndex < sprites.Length')
      has(query('int c=(a=(b=0));'),'restore-zero-chain-local','future = c = a = b = 0;')
      unused=query('y=20;',original_unused_slots=[{'index':1,'type':'System.Int32','loads':0,'stores':0}])
      has(unused,'reserve-unused-original-slot','originalUnusedSlot1;')
      has(unused,'reserve-unused-slot-and-defer-local','int y = 20;')
      dead_start=source.index('void MissingDead()')
      pos=source.index('int c=(a=(b=0));',dead_start)
      dead=client.request(path,source=source,offset=len(source[:pos].encode('utf-16-le'))//2,
        original_unused_slots=[{'index':2,'type':'System.Int32','loads':0,'stores':1}],original_local_count=4,rebuilt_local_count=3)
      has(dead,'restore-dead-zero-chain','originalDeadSlot2')
      def scoped_query(method,**hints):
        pos=source.index('int c=(a=(b=0));',source.index('void '+method+'('))
        return client.request(path,source=source,offset=len(source[:pos].encode('utf-16-le'))//2,max_candidates=512,**hints)
      has(scoped_query('Shared',original_zero_chain_stores=4),'restore-shared-root-zero','x = c = a = b = 0;')
      assert not any(p['rule']=='restore-shared-root-zero' for p in scoped_query('Shared')['patches']);checks.append('shared-root-requires-original-chain-evidence')
      assert not any(p['rule']=='restore-shared-root-zero' for p in scoped_query('Captured',original_zero_chain_stores=4)['patches']);checks.append('shared-root-capture-rejected')
      assert not any(p['rule']=='restore-shared-root-zero' for p in scoped_query('Collision',original_zero_chain_stores=4)['patches']);checks.append('shared-root-other-binding-rejected')
      before=query('int n=terms[1];',expected_alias_type='System.Int32[]')
      screened=client.screen_source(source,before['declaration_start'])
      after=query('int n=terms[1];',expected_alias_type='System.Int32[]')
      assert before==after and 'int n=terms[1]' in screened and 'throw new global::System.NotImplementedException' in screened
      checks.append('screen-mode-stateless')
    target=ROOT/'reports/round5-transform-tests.json'
    target.write_text(json.dumps({'fixture_pass_count':len(checks),'checks':checks,'scope':'syntax/safety only; no state promotions'},indent=2)+'\n',encoding='utf-8',newline='\n')
    print(json.dumps({'fixture_checks_passed':len(checks)}))

if __name__=='__main__':main()
