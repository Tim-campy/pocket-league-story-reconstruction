"""B2 prototype fixtures, entirely isolated from the active engine helper/model."""
import hashlib
import json
from pathlib import Path
import shutil
import subprocess
import round7_smcs_fixture_helper as helper
from compiler_probes import ROSLYN
from test_round7_smcs_rules import CORE, apply, compile_toy
from round7_smcs_branch_exit_diagnose import branch_exit_hints

ROOT=Path(__file__).resolve().parents[1]
BASE_HELPER=helper.ensure_helper


def ensure_exit_helper():
    command=BASE_HELPER();base=Path(command[-1]).parent
    sources={p.name:p.read_text(encoding='utf-8-sig') for p in base.glob('*.cs')}
    sources['Round7SmcsExitRules.cs']=(ROOT / 'tools/Round7SmcsExitRules.cs').read_text(encoding='utf-8-sig')
    anchor='AddRound7SmcsRules(input,text,mapped,methodBody,model,point,patches,seen);'
    sources['Round7TransformSyntax.cs']=sources['Round7TransformSyntax.cs'].replace(anchor,anchor+'\n        AddRound7SmcsExitRules(input,text,mapped,methodBody,model,point,patches,seen);')
    digest=hashlib.sha256(''.join(sources.values()).encode()).hexdigest()
    folder=ROOT / 'build/round7-smcs-fixtures/exit-helper' / digest[:16]
    target=folder / 'Round7TransformSyntax.dll'
    if not target.exists():
        folder.mkdir(parents=True,exist_ok=True)
        for name,source in sources.items():
            (folder / name).write_text(source,encoding='utf-8',newline='\n')
        rsp=(base / 'compile.rsp').read_text(encoding='utf-8-sig').replace(str(base),str(folder))
        rsp += '"'+str(folder / 'Round7SmcsExitRules.cs')+'"\n'
        (folder / 'compile.rsp').write_text(rsp,encoding='utf-8')
        compiled=subprocess.run([str(ROSLYN),'/noconfig','@'+str(folder / 'compile.rsp')],capture_output=True,text=True)
        assert compiled.returncode==0,compiled.stdout+compiled.stderr
        for path in base.glob('Microsoft.CodeAnalysis*.dll'):
            shutil.copy2(path,folder / path.name)
        shutil.copy2(base / 'Round7TransformSyntax.runtimeconfig.json',folder / 'Round7TransformSyntax.runtimeconfig.json')
    return [command[0],str(target)]


SOURCE='''class Fixture {
 int field; void Use(int x) {field=x;}
 void Guard(bool skip,int count) {if(skip) {return;}for(int i=0;i<count;i++) {Use(i);}}
 void ExpectedGuard(bool skip,int count) {if(skip) {}else {for(int i=0;i<count;i++) {Use(i);}}}
 void Loop(Fixture[] values) {for(int i=0;i<values.Length;i++) {Fixture value=values[i];if(value==null) {return;}Use(value.field);}}
 void ExpectedLoop(Fixture[] values) {for(int i=0;i<values.Length;i++) {Fixture value=values[i];if(value==null) {continue;}Use(value.field);}}
 void Floating(float x) {for(int i=0;i<3;i++) {if(x>0f) {return;}Use(i);}}
 void ExpectedFloating(float x) {for(int i=0;i<3;i++) {if(x>0f) {continue;}Use(i);}}
 void NotTerminal(bool skip) {if(skip) {if(field==0) {return;}Use(1);}Use(99);}
}
'''


def main():
    helper.ensure_helper=ensure_exit_helper
    client=helper.FixtureClient(SOURCE,[CORE]);checks=[];candidates=[]
    def query(method,**hints):
        return client.request(method,'return;',**hints)
    def has(method,rule,kind,expected):
        found=[p for p in query(method,round7_cycle_rule=rule,original_exit_target_kind=kind) if p['rule']==rule]
        assert found,(method,rule)
        candidates.append((method,expected,found[0]));checks.append(method+':'+rule)
    def lacks(method,rule,**hints):
        assert not any(p['rule']==rule for p in query(method,**hints)),(method,rule,hints)
        checks.append(method+':reject-'+rule)
    try:
        has('void Guard(','void-empty-then-common-exit','method-ret','ExpectedGuard')
        has('void Loop(','loop-guard-return-to-continue','loop-increment','ExpectedLoop')
        has('void Floating(','loop-guard-return-to-continue','loop-increment','ExpectedFloating')
        lacks('void Guard(','void-empty-then-common-exit',round7_cycle_rule='void-empty-then-common-exit')
        lacks('void Loop(','loop-guard-return-to-continue',round7_cycle_rule='loop-guard-return-to-continue')
        lacks('void Guard(','void-empty-then-common-exit',original_exit_target_kind='method-ret')
        lacks('void NotTerminal(','void-empty-then-common-exit',round7_cycle_rule='void-empty-then-common-exit',original_exit_target_kind='method-ret')
        lacks('void Guard(','loop-guard-return-to-continue',round7_cycle_rule='loop-guard-return-to-continue',original_exit_target_kind='loop-increment')
    finally:
        client.close()
    proofs=[]
    facets=('header','locals','instructions','exception_regions','extra_sections','method_attributes','impl_attributes')
    for method,expected,patch in candidates:
        methods=compile_toy(apply(SOURCE,patch),'branch-exit-'+expected)
        name=method.split('(')[0].split()[-1]
        actual=next(m for key,m in methods.items() if key.startswith('Fixture::'+name+'('))
        reference=next(m for key,m in methods.items() if key.startswith('Fixture::'+expected+'('))
        differences=[facet for facet in facets if actual[facet]!=reference[facet]]
        assert not differences,(method,differences)
        proofs.append({'method':name,'reference':expected,'body_facets_identical':list(facets),'compile_exit':0})
    # Negative target evidence: similar-looking load/store of a different slot
    # cannot be called a proven counter increment.
    for changed,wanted in [(False,'loop-increment'),(True,'UNKNOWN')]:
        ins=[{'offset':0,'opcode':'br','operand':{'branch':1}},
             {'offset':1,'opcode':'ldloc.0','operand':None},
             {'offset':2,'opcode':'ldc.i4.1','operand':None},
             {'offset':3,'opcode':'add','operand':None},
             {'offset':4,'opcode':'stloc.1' if changed else 'stloc.0','operand':None}]
        assert branch_exit_hints({'instructions':ins},0)['original_exit_target_kind']==wanted
        checks.append('counter-target-'+wanted)
    report={'operation':'isolated-B2-prototype-fixtures','syntax_and_evidence_fixture_count':len(checks),
        'historical_smcs_compiles':len(proofs),'complete_body_comparisons_equal':len(proofs),
        'game_candidate_compilations':0,'checks':checks,'proofs':proofs}
    (ROOT / 'reports/round7-smcs-exit-rule-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({k:report[k] for k in ('syntax_and_evidence_fixture_count','historical_smcs_compiles','complete_body_comparisons_equal')}))


if __name__=='__main__':main()
