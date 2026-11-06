"""Isolated ownership-aware switch fixtures; never compiles target source."""
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
from round7_switch_hints import original_empty_zero_arm_hint

FOLDER=ROOT / 'build/round7/switch-structural-experiment'
RULE=ROOT / 'tools/Round7SmcsSwitchRules.cs'
BASE_HELPER=helper.ensure_helper


def ensure_switch_helper():
    command=BASE_HELPER();base=Path(command[-1]).parent
    sources={p.name:p.read_text(encoding='utf-8-sig') for p in base.glob('*.cs')}
    sources[RULE.name]=RULE.read_text(encoding='utf-8-sig')
    anchor='AddRound7SmcsRules(input,text,mapped,methodBody,model,point,patches,seen);'
    sources['Round7TransformSyntax.cs']=sources['Round7TransformSyntax.cs'].replace(anchor,anchor+'\n        AddRound7SmcsSwitchRules(input,text,mapped,methodBody,model,point,patches,seen);')
    digest=hashlib.sha256(''.join(sources.values()).encode()).hexdigest()
    folder=FOLDER / 'helper' / digest[:16];target=folder / 'Round7TransformSyntax.dll'
    if not target.exists():
        folder.mkdir(parents=True,exist_ok=True)
        for name,source in sources.items():(folder / name).write_text(source,encoding='utf-8',newline='\n')
        rsp=(base / 'compile.rsp').read_text(encoding='utf-8-sig').replace(str(base),str(folder))
        rsp+='"'+str(folder / RULE.name)+'"\n';(folder / 'compile.rsp').write_text(rsp,encoding='utf-8')
        compiled=subprocess.run([str(ROSLYN),'/noconfig','@'+str(folder / 'compile.rsp')],capture_output=True,text=True)
        assert compiled.returncode==0,compiled.stdout+compiled.stderr
        for path in base.glob('Microsoft.CodeAnalysis*.dll'):shutil.copy2(path,folder / path.name)
        shutil.copy2(base / 'Round7TransformSyntax.runtimeconfig.json',folder / 'Round7TransformSyntax.runtimeconfig.json')
    return [command[0],str(target)]


SOURCE='''public class Fixture {
 string trace; int selector; volatile int volatileSelector;
 void Use(int n) {trace+=n+";";}
 int GetSelector() {trace+="G;";return selector;}
 int Property {get {return GetSelector();}}
 public string Block(int value) {trace="";switch(value) {case 0: {int n=10;Use(n);break;}case 1:Use(1);break;}return trace;}
 public string ExpectedBlock(int value) {trace="";if(value==0) {int n=10;Use(n);}else if(value==1) {Use(1);}return trace;}
 public string Early(int value,bool stop) {trace="";switch(value) {case 0: {if(stop) {Use(0);break;}Use(1);break;}case 1:Use(2);break;}return trace;}
 public string ExpectedEarly(int value,bool stop) {trace="";{if(value==0) {if(stop) {Use(0);goto done;}Use(1);}else if(value==1) {Use(2);}done: ;}return trace;}
 public string InnerLoop(int value) {trace="";switch(value) {case 0: {for(int i=0;i<4;i++) {if(i==2) {break;}if(i==0) {continue;}Use(i);}break;}case 1:Use(9);break;}return trace;}
 public string ExpectedInnerLoop(int value) {trace="";if(value==0) {for(int i=0;i<4;i++) {if(i==2) {break;}if(i==0) {continue;}Use(i);}}else if(value==1) {Use(9);}return trace;}
 public string OuterLoop(int value) {trace="";for(int i=0;i<3;i++) {switch(value) {case 0: {if(i==1) {Use(20);break;}Use(i);break;}case 1:Use(1);break;}Use(9);}return trace;}
 public string ExpectedOuterLoop(int value) {trace="";for(int i=0;i<3;i++) {{if(value==0) {if(i==1) {Use(20);goto next;}Use(i);}else if(value==1) {Use(1);}next: ;}Use(9);}return trace;}
 public string Nested(int value,int other) {trace="";switch(value) {case 0: {switch(other) {case 0:Use(10);break;default:Use(11);break;}break;}case 1:Use(20);break;}return trace;}
 public string ExpectedNested(int value,int other) {trace="";if(value==0) {switch(other) {case 0:Use(10);break;default:Use(11);break;}}else if(value==1) {Use(20);}return trace;}
 public string Labels(int value) {trace="";switch(value) {case 0:case 1:Use(1);break;default:Use(9);break;}return trace;}
 public string ExpectedLabels(int value) {trace="";if(value==0||value==1) {Use(1);}else {Use(9);}return trace;}
 public string Field(int value) {trace="";selector=value;switch(selector) {case 0:selector=1;Use(0);break;case 1:Use(1);break;}return trace;}
 public string ExpectedField(int value) {trace="";selector=value;if(selector==0) {selector=1;Use(0);}else if(selector==1) {Use(1);}return trace;}
 public string Zero(int value) {trace="";switch(value) {case 1:Use(1);break;case 2:Use(2);break;}return trace;}
 public string ExpectedZero(int value) {trace="";if(value==0) {}else if(value==1) {Use(1);}else if(value==2) {Use(2);}return trace;}
 public string Large(int value) {trace="";switch(value) {%LARGE_CASES%}return trace;}
 public string ExpectedLarge(int value) {trace="";%LARGE_IFS%return trace;}
 public void SideEffect() {switch(GetSelector()) {case 0:Use(0);break;case 1:Use(1);break;}}
 public void Getter() {switch(Property) {case 0:Use(0);break;case 1:Use(1);break;}}
 public void Volatile() {switch(volatileSelector) {case 0:Use(0);break;case 1:Use(1);break;}}
 public void OuterContinue(int value) {for(int i=0;i<3;i++) {switch(value) {case 0:continue;default:Use(i);break;}Use(9);}}
 public void CrossCase(int value) {switch(value) {case 0:int shared=1;Use(shared);break;case 1:shared=2;Use(shared);break;}}
 public void Transfer(int value) {switch(value) {case 0:Use(0);goto case 1;case 1:Use(1);break;}}
}
public class Program {
 static void Equal(string name,string a,string b) {if(a!=b) {throw new System.Exception(name+":"+a+"!="+b);}System.Console.WriteLine(name+":"+a);}
 public static void Main() {Fixture f=new Fixture();for(int n=-1;n<5;n++) {
  Equal("block/"+n,f.Block(n),f.ExpectedBlock(n));
  Equal("early-true/"+n,f.Early(n,true),f.ExpectedEarly(n,true));
  Equal("early-false/"+n,f.Early(n,false),f.ExpectedEarly(n,false));
  Equal("loop/"+n,f.InnerLoop(n),f.ExpectedInnerLoop(n));
  Equal("outer-loop/"+n,f.OuterLoop(n),f.ExpectedOuterLoop(n));
  Equal("nested0/"+n,f.Nested(n,0),f.ExpectedNested(n,0));
  Equal("nested2/"+n,f.Nested(n,2),f.ExpectedNested(n,2));
  Equal("labels/"+n,f.Labels(n),f.ExpectedLabels(n));
  Equal("field/"+n,f.Field(n),f.ExpectedField(n));
  Equal("zero/"+n,f.Zero(n),f.ExpectedZero(n));
 }for(int n=-1;n<22;n++) {Equal("large/"+n,f.Large(n),f.ExpectedLarge(n));}}
}
'''.replace('%LARGE_CASES%',''.join('case '+str(i)+':Use('+str(i)+');break;' for i in range(20))) \
   .replace('%LARGE_IFS%',' else '.join('if(value=='+str(i)+') {Use('+str(i)+');}' for i in range(20)))


def run_trace(source,name):
    folder=FOLDER / 'runtime' / name;folder.mkdir(parents=True,exist_ok=True)
    path=folder / 'Fixture.cs';path.write_text(source,encoding='utf-8');binary=folder / 'Fixture.exe'
    env=os.environ.copy();env['MONO_PATH']=str(UNITY / 'Editor/Data/Mono/lib/mono/unity');env['MONO_CFG_DIR']=str(UNITY / 'Editor/Data/Mono/etc')
    compiled=subprocess.run([str(MONO),str(SMCS),'-nologo','-noconfig','-nostdlib','-target:exe','-debug-','-optimize+',
        '-out:'+str(binary),'-r:'+str(CORE),str(path)],env=env,capture_output=True,text=True,timeout=45)
    assert compiled.returncode==0,compiled.stdout+compiled.stderr
    run=subprocess.run([str(MONO),str(binary)],env=env,capture_output=True,text=True,timeout=15)
    assert run.returncode==0,run.stdout+run.stderr
    return run.stdout.splitlines()


def main():
    helper.ensure_helper=ensure_switch_helper
    client=helper.FixtureClient(SOURCE,[CORE]);checks=[];candidates=[]
    def has(method,reference,rule='smcs-owned-switch-if',**hints):
        found=[p for p in client.request('string '+method+'(','switch(',round7_cycle_rule='smcs-owned-switch-if',**hints)
            if p['rule']==rule and p['before'].startswith('switch(value)' if method!='Field' else 'switch(selector)')]
        assert found,method
        candidates.append((method,reference,found[0]));checks.append(method+':'+rule)
    def lacks(method,rule='smcs-owned-switch-if',selected=True):
        hints={'round7_cycle_rule':'smcs-owned-switch-if'} if selected else {}
        assert not any(p['rule'].startswith(rule) for p in client.request('void '+method+'(','switch(',**hints)),method
        checks.append(method+':reject-'+rule)
    try:
        for name in ['Block','Early','InnerLoop','OuterLoop','Nested','Labels','Field','Large']:has(name,'Expected'+name)
        has('Zero','ExpectedZero','smcs-owned-switch-if-empty-zero',original_empty_zero_arm=True)
        for name in ['SideEffect','Getter','Volatile','OuterContinue','CrossCase','Transfer']:lacks(name)
        assert not any(p['rule']=='smcs-owned-switch-if-empty-zero' for p in client.request('string Zero(','switch(',round7_cycle_rule='smcs-owned-switch-if'))
        checks.append('zero-arm-requires-original-evidence')
        lacks('SideEffect',selected=False)
        assert not any(p['rule'].startswith('smcs-owned-switch-if') for p in client.request('string Block(','switch('))
        checks.append('valid-switch-requires-opt-in')
        assert any(p['rule']=='smcs-owned-switch-if' for p in client.request('string Block(','switch(',round7_cycle_rules=['smcs-owned-switch-if']))
        checks.append('learned-family-explicit-opt-in')
    finally:client.close()
    facets=('header','locals','instructions','exception_regions','extra_sections','method_attributes','impl_attributes')
    proofs=[];baseline=run_trace(SOURCE,'baseline')
    for method,reference,patch in candidates:
        changed=apply(SOURCE,patch);methods=compile_toy(changed,'owned-switch-'+reference)
        actual=next(m for k,m in methods.items() if k.startswith('Fixture::'+method+'('))
        expected=next(m for k,m in methods.items() if k.startswith('Fixture::'+reference+'('))
        different=[facet for facet in facets if actual[facet]!=expected[facet]]
        assert not different,(method,different)
        trace=run_trace(changed,reference);assert trace==baseline,method
        proofs.append({'method':method,'reference':reference,'complete_body_equal':True,'compared_facets':list(facets),
            'runtime_trace_equal_to_original_switch':True,'scenarios':len(trace)})
    evidence={'instructions':[{'offset':0,'opcode':'brtrue','operand':{'branch':10}},
        {'offset':5,'opcode':'br','operand':{'branch':50}}, {'offset':10,'opcode':'ldloc.0','operand':None}]}
    assert original_empty_zero_arm_hint(evidence,0)=={'original_empty_zero_arm':True}
    hint_checks=['positive-zero-empty-then']
    evidence['instructions'][0]['operand']['branch']=99
    assert not original_empty_zero_arm_hint(evidence,0);hint_checks.append('reject-nonadjacent-nonzero-body')
    evidence['instructions'][0]['operand']['branch']=10;evidence['instructions'][1]['operand']['branch']=5
    assert not original_empty_zero_arm_hint(evidence,0);hint_checks.append('reject-backward-or-nonexit-zero-jump')
    assert not original_empty_zero_arm_hint(evidence,77);hint_checks.append('reject-missing-original-offset')
    report={'operation':'isolated-switch-owner-fixtures','syntax_safety_checks':len(checks),'pure_hint_checks':len(hint_checks),'hint_checks':hint_checks,'toy_library_compiles':len(proofs),
        'exact_reference_body_matches':len(proofs),'toy_executable_compiles':len(proofs)+1,'runtime_runs':len(proofs)+1,
        'target_compilations':0,'rule_sha256':hashlib.sha256(RULE.read_bytes()).hexdigest(),
        'hints_sha256':hashlib.sha256((ROOT / 'tools/round7_switch_hints.py').read_bytes()).hexdigest(),
        'fixture_driver_sha256':hashlib.sha256(Path(__file__).read_bytes()).hexdigest(),
        'compiler_path':str(SMCS),'compiler_sha256':hashlib.sha256(SMCS.read_bytes()).hexdigest(),
        'checks':checks,'proofs':proofs}
    (ROOT / 'reports/round7-switch-structural-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({k:report[k] for k in ('syntax_safety_checks','toy_library_compiles','exact_reference_body_matches','runtime_runs')}))


if __name__=='__main__':main()
