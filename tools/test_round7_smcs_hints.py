"""Pure diagnostic-hint fixtures; no compiler or game-state access."""
import json
from pathlib import Path
from round7_smcs_hints import rule_hints

ROOT=Path(__file__).resolve().parents[1]


def method(rows):
    return {'instructions':[dict(offset=n,opcode=op,operand=operand) for n,(op,operand) in enumerate(rows)],
            'header':{'code_size':len(rows)},'locals':['placeholder']}


def main():
    checks=[]
    test=method([('call',{'method':'Get'}),('stloc.0',None),('ldloc.0',None),('brfalse',{'branch':4}),('ret',None)])
    h=rule_hints(test,test,0,['System.Int32'],['System.Int32'])
    assert h['expected_test_local_types']==['System.Int32'];checks.append('nonboolean-tested-slot')
    obj=method([('newobj',{'method':'Fixture::.ctor'}),('stloc.0',None),('ret',None)])
    assert rule_hints(obj,obj,0,['Fixture'],['Fixture'])['expected_newobj_local_types']==['Fixture'];checks.append('newobj-store-type')
    result=method([('ldc.i4.0',None),('stloc.0',None),('br',{'branch':5}),('ldc.i4.1',None),('stloc.0',None),('ldloc.0',None),('ret',None)])
    assert rule_hints(result,result,2,['System.Int32'],['System.Int32'])['original_unified_return'];checks.append('single-result-exit')
    result['instructions'][3]['opcode']='ret'
    assert not rule_hints(result,result,2,['System.Int32'],['System.Int32']).get('original_unified_return');checks.append('reject-multiple-ret')
    end=method([('br',{'branch':1}),('ret',None)])
    assert rule_hints(end,end,0,[],[])['original_void_common_exit'];checks.append('void-shared-ret')
    chain=method([('ldc.i4.0',None),('dup',None),('stloc.2',None),('dup',None),('stloc.1',None),('stloc.0',None),
                  ('ldloc.0',None),('ldloc.1',None),('add',None),('ldloc.2',None),('add',None),('ret',None)])
    types=['System.Int32']*3
    model={'complete':True,'variables':[{'slot':n,'name':name,'kind':'declared','type':'System.Int32'} for n,name in enumerate(['a','b','c'])]}
    h=rule_hints(chain,chain,2,types,types,model)
    assert h['original_zero_chain_names']==['a','b','c'] and h['original_zero_chain_declaration_order']==['a','b','c'];checks.append('zerochain-voted-source-names')
    model['complete']=False
    assert 'original_zero_chain_names' not in rule_hints(chain,chain,2,types,types,model);checks.append('reject-incomplete-model')
    model['complete']=True;model['variables'].append({'slot':0,'name':'unrelated','kind':'declared','type':'System.Int32'})
    assert 'original_zero_chain_names' not in rule_hints(chain,chain,2,types,types,model);checks.append('reject-ambiguous-source-binding')
    report={'fixture_count':len(checks),'checks':checks,'status':'PASS','game_compilations':0}
    (ROOT / 'reports/round7-smcs-hint-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(json.dumps(report))


if __name__=='__main__': main()
