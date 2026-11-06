"""WIP syntax and historical smcs fixtures, no target compilation or active edits."""
import hashlib
import json
from pathlib import Path
import shutil
import subprocess
import sys

ROOT=next(parent for parent in Path(__file__).resolve().parents if (parent / 'AGENTS.md').is_file())
sys.path.insert(0,str(ROOT / 'tools'))
import round7_smcs_fixture_helper as helper
from compiler_probes import ROSLYN
from test_round7_smcs_rules import CORE,apply,compile_toy

FOLDER=ROOT / 'build/round7/guard-experiment'
BASE_HELPER=helper.ensure_helper


def ensure_guard_helper():
    command=BASE_HELPER();base=Path(command[-1]).parent
    sources={p.name:p.read_text(encoding='utf-8-sig') for p in base.glob('*.cs')}
    sources['Round7SmcsGuardRules.cs']=(ROOT / 'tools/Round7SmcsGuardRules.cs').read_text(encoding='utf-8-sig')
    anchor='AddRound7SmcsRules(input,text,mapped,methodBody,model,point,patches,seen);'
    sources['Round7TransformSyntax.cs']=sources['Round7TransformSyntax.cs'].replace(anchor,anchor+'\n        AddRound7SmcsGuardRules(input,text,mapped,methodBody,model,point,patches,seen);')
    digest=hashlib.sha256(''.join(sources.values()).encode()).hexdigest()
    folder=FOLDER / 'helper' / digest[:16];target=folder / 'Round7TransformSyntax.dll'
    if not target.exists():
        folder.mkdir(parents=True,exist_ok=True)
        for name,source in sources.items():
            (folder / name).write_text(source,encoding='utf-8',newline='\n')
        rsp=(base / 'compile.rsp').read_text(encoding='utf-8-sig').replace(str(base),str(folder))
        rsp += '"'+str(folder / 'Round7SmcsGuardRules.cs')+'"\n'
        (folder / 'compile.rsp').write_text(rsp,encoding='utf-8')
        compiled=subprocess.run([str(ROSLYN),'/noconfig','@'+str(folder / 'compile.rsp')],capture_output=True,text=True)
        assert compiled.returncode==0,compiled.stdout+compiled.stderr
        for path in base.glob('Microsoft.CodeAnalysis*.dll'):
            shutil.copy2(path,folder / path.name)
        shutil.copy2(base / 'Round7TransformSyntax.runtimeconfig.json',folder / 'Round7TransformSyntax.runtimeconfig.json')
    return [command[0],str(target)]


SOURCE='''class Fixture {
 void Use(int x) {System.Console.WriteLine(x);}
 void Empty(int x) {if(x!=36) {Use(x);}}
 void ExpectedEmpty(int x) {if(x==36) {}else {Use(x);}}
 void EmptyAnd(int a,int b) {if(a!=4&&b!=5) {Use(a+b);}}
 void ExpectedEmptyAnd(int a,int b) {if(a==4) {}else if((b!=5)) {Use(a+b);}}
 void EmptySuffix(bool flag,int length,char c) {if(flag&&(length!=0||c!=',')) {Use(length);}}
 void ExpectedEmptySuffix(bool flag,int length,char c) {if((flag)) {if(((length==0)&&(c==','))) {}else {Use(length);}}}
 void ContinueLoop(int x) {for(int i=0;i<3;i++) {if(x!=3) {continue;}Use(i);}}
 void ExpectedContinueLoop(int x) {for(int i=0;i<3;i++) {if(x==3) {Use(i);}}}
 void ContinueFloat(float x) {for(int i=0;i<3;i++) {if(x>0f) {continue;}Use(i);}}
 void ExpectedContinueFloat(float x) {for(int i=0;i<3;i++) {if(!(x>0f)) {Use(i);}}}
 void ContinueForeach(System.Collections.ArrayList values) {foreach(object value in values) {if(value==null) {continue;}Use(value.GetHashCode());}}
 void ExpectedContinueForeach(System.Collections.ArrayList values) {foreach(object value in values) {if(value!=null) {Use(value.GetHashCode());}}}
 bool Value(int x) {if(x!=-3) {Use(2);return true;}Use(1);return false;}
 bool ExpectedValue(int x) {if(x==-3) {Use(1);return false;}else {Use(2);return true;}}
 void Protected(int mode) {try {if(mode==1) {Use(1);}else {if(mode!=2) {return;}Use(2);}}catch(System.Exception) {}finally {Use(9);}}
 void ExpectedProtected(int mode) {try {if(mode==1) {Use(1);}else {if(mode==2) {Use(2);}}}catch(System.Exception) {}finally {Use(9);}}
 void GuardBreak(int x) {for(int i=0;i<3;i++) {if(x!=3) {break;}Use(i);}}
 void NotLoopTail(int x) {for(int i=0;i<3;i++) {if(x>1) {if(x!=3) {continue;}Use(i);}Use(9);}}
 void NotTryTail(int x) {try {if(x!=3) {return;}Use(1);}finally {Use(2);}Use(3);}
}
'''


def main():
    helper.ensure_helper=ensure_guard_helper
    client=helper.FixtureClient(SOURCE,[CORE]);checks=[];candidates=[]
    def has(method,needle,selected,rule,reference):
        found=[p for p in client.request(method,needle,round7_cycle_rule=selected) if p['rule']==rule]
        assert found,(method,rule)
        candidates.append((method,reference,found[0]));checks.append(method+':'+rule)
    def lacks(method,needle,rule,selected=None):
        hints={} if selected is None else {'round7_cycle_rule':selected}
        assert not any(p['rule'].startswith(rule) for p in client.request(method,needle,**hints)),(method,rule)
        checks.append(method+':reject-'+rule)
    try:
        has('void Empty(','if(x!=','smcs-empty-arm-restore','smcs-empty-arm-restore','ExpectedEmpty')
        has('void EmptyAnd(','if(a!=','smcs-empty-arm-restore','smcs-empty-arm-first-and','ExpectedEmptyAnd')
        has('void EmptySuffix(','if(flag','smcs-empty-arm-restore','smcs-empty-arm-and-suffix','ExpectedEmptySuffix')
        has('void ContinueLoop(','if(x!=','smcs-continue-positive-tail','smcs-continue-positive-tail','ExpectedContinueLoop')
        has('void ContinueFloat(','if(x>','smcs-continue-positive-tail','smcs-continue-positive-tail','ExpectedContinueFloat')
        has('void ContinueForeach(','if(value','smcs-continue-positive-tail','smcs-continue-positive-tail','ExpectedContinueForeach')
        has('bool Value(','if(x!=','smcs-value-return-guard-reverse','smcs-value-return-guard-reverse','ExpectedValue')
        has('void Protected(','if(mode!=','smcs-protected-void-guard-tail','smcs-protected-void-guard-tail','ExpectedProtected')
        lacks('void GuardBreak(','if(x!=','smcs-continue-positive-tail','smcs-continue-positive-tail')
        lacks('void NotLoopTail(','if(x!=','smcs-continue-positive-tail','smcs-continue-positive-tail')
        lacks('void NotTryTail(','if(x!=','smcs-protected-void-guard-tail','smcs-protected-void-guard-tail')
        lacks('void Empty(','if(x!=','smcs-empty-arm')
    finally:client.close()
    proofs=[];facets=('header','locals','instructions','exception_regions','extra_sections','method_attributes','impl_attributes')
    for method,expected,patch in candidates:
        methods=compile_toy(apply(SOURCE,patch),'guard-experiment-'+expected)
        name=method.split('(')[0].split()[-1]
        actual=next(m for k,m in methods.items() if k.startswith('Fixture::'+name+'('))
        reference=next(m for k,m in methods.items() if k.startswith('Fixture::'+expected+'('))
        different=[facet for facet in facets if actual[facet]!=reference[facet]]
        assert not different,(name,expected,different)
        proofs.append({'method':name,'reference':expected,'rule':patch['rule'],'complete_body_equal':True,
            'compared_facets':list(facets),'exception_regions':len(actual['exception_regions'])})
    report={'operation':'source-only-prototypes-plus-isolated-toy-smcs-compiles','syntax_safety_checks':len(checks),
        'toy_compiles':len(proofs),'exact_reference_body_matches':len(proofs),'target_compilations':0,
        'rule_sha256':hashlib.sha256((ROOT / 'tools/Round7SmcsGuardRules.cs').read_bytes()).hexdigest(),
        'fixture_driver_sha256':hashlib.sha256(Path(__file__).read_bytes()).hexdigest(),
        'checks':checks,'proofs':proofs}
    (ROOT / 'reports/round7-guard-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({k:report[k] for k in ('syntax_safety_checks','toy_compiles','exact_reference_body_matches')}))


if __name__=='__main__':main()
