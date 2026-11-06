"""Isolated loop-condition fixtures, no game compilation or active helper edits."""
import hashlib
import json
import os
from pathlib import Path
import shutil
import subprocess
import sys
ROOT=next(p for p in Path(__file__).resolve().parents if (p / 'AGENTS.md').is_file())
sys.path.insert(0,str(ROOT / 'tools'))
import round7_smcs_fixture_helper as helper
from compiler_probes import MONO,SMCS,UNITY,ROSLYN
from test_round7_smcs_rules import CORE,apply,compile_toy

FOLDER=ROOT / 'build/round7/loop-condition-experiment'
RULE=ROOT / 'tools/Round7LoopConditionRules.cs'
BASE_HELPER=helper.ensure_helper


def ensure_loop_helper():
    command=BASE_HELPER();base=Path(command[-1]).parent
    sources={p.name:p.read_text(encoding='utf-8-sig') for p in base.glob('*.cs')}
    sources[RULE.name]=RULE.read_text(encoding='utf-8-sig')
    anchor='AddRound7SmcsRules(input,text,mapped,methodBody,model,point,patches,seen);'
    sources['Round7TransformSyntax.cs']=sources['Round7TransformSyntax.cs'].replace(anchor,anchor+'\n        AddRound7LoopConditionRules(input,text,mapped,methodBody,model,point,patches,seen);')
    digest=hashlib.sha256(''.join(sources.values()).encode()).hexdigest()
    folder=FOLDER / 'helper' / digest[:16];target=folder / 'Round7TransformSyntax.dll'
    if not target.exists():
        folder.mkdir(parents=True,exist_ok=True)
        for name,source in sources.items():(folder / name).write_text(source,encoding='utf-8',newline='\n')
        rsp=(base / 'compile.rsp').read_text(encoding='utf-8-sig').replace(str(base),str(folder))
        rsp+='"'+str(folder / RULE.name)+'"\n'
        (folder / 'compile.rsp').write_text(rsp,encoding='utf-8')
        compiled=subprocess.run([str(ROSLYN),'/noconfig','@'+str(folder / 'compile.rsp')],capture_output=True,text=True)
        assert compiled.returncode==0,compiled.stdout+compiled.stderr
        for path in base.glob('Microsoft.CodeAnalysis*.dll'):shutil.copy2(path,folder / path.name)
        shutil.copy2(base / 'Round7TransformSyntax.runtimeconfig.json',folder / 'Round7TransformSyntax.runtimeconfig.json')
    return [command[0],str(target)]


SOURCE='''public struct CustomTruth {
 public int value;
 public static CustomTruth operator &(CustomTruth a,CustomTruth b) {return a;}
 public static bool operator true(CustomTruth a) {return a.value!=0;}
 public static bool operator false(CustomTruth a) {return a.value==0;}
}
public struct CustomCompare {
 public int value;
 public static bool operator ==(CustomCompare a,CustomCompare b) {return a.value==b.value;}
 public static bool operator !=(CustomCompare a,CustomCompare b) {return a.value!=b.value;}
 public override bool Equals(object x) {return false;}
 public override int GetHashCode() {return value;}
}
public class Fixture {
 int state; string trace;
 void Reset() {state=0;trace="I";}
 bool A() {trace+="A";return state<4;}
 bool B() {trace+="B";return state!=2;}
 bool C() {trace+="C";return state!=3;}
 bool Stop(int x) {trace+="S";return x==2;}
 void Mark() {trace+="M";}
 void Increment() {trace+="+";state++;}
 public string WhileCalls(int initial) {Reset();state=initial;while(A()&&B()) {Mark();Increment();}return trace;}
 public string ExpectedWhileCalls(int initial) {Reset();state=initial;while(A()) {if(!B()) {break;}Mark();Increment();}return trace;}
 public string ForCalls() {Reset();for(int i=0;A()&&B();Increment(),i++) {Mark();if(i==1) {continue;}}return trace;}
 public string ExpectedForCalls() {Reset();for(int i=0;A();Increment(),i++) {if(!B()) {break;}Mark();if(i==1) {continue;}}return trace;}
 public string ForEmpty(int n) {Reset();for(int i=0;i<n&&!Stop(i);i++) {}return trace;}
 public string ExpectedForEmpty(int n) {Reset();for(int i=0;i<n;i++) {if(Stop(i)) {break;}}return trace;}
 public string WhileFloat(float x) {Reset();int n=3;while(n>0&&x<0f) {Mark();n--;}return trace;}
 public string ExpectedWhileFloat(float x) {Reset();int n=3;while(n>0) {if(!(x<0f)) {break;}Mark();n--;}return trace;}
 public string Chain() {Reset();while(A()&&B()&&C()) {Mark();Increment();}return trace;}
 public string ExpectedChainFirst() {Reset();while(A()) {if(!B()||!C()) {break;}Mark();Increment();}return trace;}
 public string ExpectedChainLast() {Reset();while(A()&&B()) {if(!C()) {break;}Mark();Increment();}return trace;}
 public string ForeachOnly(bool[] values) {Reset();foreach(bool value in values) {if(value&&B()) {Mark();}}return trace;}
 public string OrOnly() {Reset();while(A()||B()) {break;}return trace;}
 public void Custom(CustomTruth a,CustomTruth b) {while(a&&b) {break;}}
 public void CustomEquality(bool flag,CustomCompare a,CustomCompare b) {while(flag&&a==b) {break;}}
}
public class Program {
 static void Equal(string name,string a,string b) {if(a!=b) {throw new System.Exception(name+":"+a+"!="+b);}System.Console.WriteLine(name+":"+a);}
 public static void Main() {
  Fixture fixture=new Fixture();
  Equal("while-calls",fixture.WhileCalls(0),fixture.ExpectedWhileCalls(0));
  Equal("while-A-false-no-B",fixture.WhileCalls(4),fixture.ExpectedWhileCalls(4));
  Equal("for-calls-continue-increment",fixture.ForCalls(),fixture.ExpectedForCalls());
  Equal("for-empty",fixture.ForEmpty(5),fixture.ExpectedForEmpty(5));
  Equal("for-A-false-no-Stop",fixture.ForEmpty(0),fixture.ExpectedForEmpty(0));
  Equal("chain-first",fixture.Chain(),fixture.ExpectedChainFirst());
  Equal("chain-last",fixture.Chain(),fixture.ExpectedChainLast());
  float[] values=new float[] {float.NaN,float.NegativeInfinity,-1f,0f,1f,float.PositiveInfinity};
  for(int i=0;i<values.Length;i++) {Equal("float-"+i,fixture.WhileFloat(values[i]),fixture.ExpectedWhileFloat(values[i]));}
 }
}
'''


def run_trace(source,name):
    folder=FOLDER / 'runtime' / name;folder.mkdir(parents=True,exist_ok=True)
    path=folder / 'Fixture.cs';path.write_text(source,encoding='utf-8')
    binary=folder / 'Fixture.exe'
    env=os.environ.copy();env['MONO_PATH']=str(UNITY / 'Editor/Data/Mono/lib/mono/unity');env['MONO_CFG_DIR']=str(UNITY / 'Editor/Data/Mono/etc')
    command=[str(MONO),str(SMCS),'-nologo','-noconfig','-nostdlib','-target:exe','-debug-','-optimize+',
        '-out:'+str(binary),'-r:'+str(CORE),str(path)]
    built=subprocess.run(command,env=env,capture_output=True,text=True,timeout=45)
    assert built.returncode==0,built.stdout+built.stderr
    run=subprocess.run([str(MONO),str(binary)],env=env,capture_output=True,text=True,timeout=15)
    assert run.returncode==0,run.stdout+run.stderr
    (folder / 'stdout.txt').write_text(run.stdout,encoding='utf-8')
    return run.stdout.splitlines()


def main():
    helper.ensure_helper=ensure_loop_helper
    client=helper.FixtureClient(SOURCE,[CORE]);checks=[];candidates=[]
    def has(method,reference,selector=None):
        found=[p for p in client.request(method,'while(' if 'For' not in method else 'for(',round7_cycle_rule='smcs-loop-condition-body-guard') if p['rule']=='smcs-loop-condition-body-guard']
        if selector:found=[p for p in found if selector(p['after'])]
        assert found,(method,reference)
        candidates.append((method,reference,found[0]));checks.append(method+':'+reference)
    def lacks(method,needle,selected=True):
        hints={'round7_cycle_rule':'smcs-loop-condition-body-guard'} if selected else {}
        assert not any(p['rule']=='smcs-loop-condition-body-guard' for p in client.request(method,needle,**hints)),method
        checks.append(method+':reject')
    try:
        has('string WhileCalls(','ExpectedWhileCalls')
        has('string ForCalls(','ExpectedForCalls')
        has('string ForEmpty(','ExpectedForEmpty')
        has('string WhileFloat(','ExpectedWhileFloat')
        has('string Chain(','ExpectedChainFirst',lambda s:' || ' in s)
        has('string Chain(','ExpectedChainLast',lambda s:' || ' not in s)
        lacks('string ForeachOnly(','foreach(')
        lacks('string OrOnly(','while(')
        lacks('void Custom(','while(')
        lacks('void CustomEquality(','while(')
        lacks('string WhileCalls(','while(',False)
    finally:client.close()
    facets=('header','locals','instructions','exception_regions','extra_sections','method_attributes','impl_attributes')
    proofs=[];baseline_trace=run_trace(SOURCE,'baseline')
    for method,reference,patch in candidates:
        changed=apply(SOURCE,patch)
        methods=compile_toy(changed,'loop-condition-'+reference)
        name=method.split('(')[0].split()[-1]
        actual=next(m for k,m in methods.items() if k.startswith('Fixture::'+name+'('))
        expected=next(m for k,m in methods.items() if k.startswith('Fixture::'+reference+'('))
        differences=[facet for facet in facets if actual[facet]!=expected[facet]]
        assert not differences,(name,reference,differences)
        trace=run_trace(changed,reference)
        assert trace==baseline_trace,(name,trace,baseline_trace)
        proofs.append({'method':name,'reference':reference,'complete_body_equal':True,'compared_facets':list(facets),
            'runtime_trace_equal_to_fused_source':True,'runtime_trace':trace})
    report={'operation':'isolated-loop-condition-source-fixtures','syntax_safety_checks':len(checks),'toy_library_compiles':len(proofs),
        'toy_executable_compiles':len(proofs)+1,'runtime_runs':len(proofs)+1,'exact_reference_body_matches':len(proofs),
        'target_compilations':0,'rule_sha256':hashlib.sha256(RULE.read_bytes()).hexdigest(),
        'fixture_driver_sha256':hashlib.sha256(Path(__file__).read_bytes()).hexdigest(),
        'compiler_path':str(SMCS),'compiler_sha256':hashlib.sha256(SMCS.read_bytes()).hexdigest(),
        'checks':checks,'proofs':proofs}
    (ROOT / 'reports/round7-loop-condition-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({k:report[k] for k in ('syntax_safety_checks','toy_library_compiles','runtime_runs','exact_reference_body_matches')}))


if __name__=='__main__':main()
