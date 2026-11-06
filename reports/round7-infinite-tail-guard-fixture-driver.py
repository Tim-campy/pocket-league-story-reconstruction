"""smcs toy fixtures for explicit tail breaks; no target compilation."""
import hashlib,json,os,shutil,subprocess,sys
from pathlib import Path
ROOT=next(p for p in Path(__file__).resolve().parents if (p/'AGENTS.md').is_file())
sys.path.insert(0,str(ROOT/'tools'))
import round7_smcs_fixture_helper as helper
from compiler_probes import MONO,SMCS,UNITY,ROSLYN
from test_round7_smcs_rules import CORE,apply,compile_toy
from round7_engine import prefix,il_cells
from round7_infinite_tail_guard_hints import infinite_tail_guard_hints

FOLDER=ROOT/'build/round7/infinite-tail-guard-experiment'
RULE=ROOT/'tools/Round7InfiniteTailGuardRules.cs'
BASE_HELPER=helper.ensure_helper


def ensure_tail_helper():
    command=BASE_HELPER();base=Path(command[-1]).parent
    sources={p.name:p.read_text(encoding='utf-8-sig') for p in base.glob('*.cs')}
    sources[RULE.name]=RULE.read_text(encoding='utf-8-sig')
    anchor='AddRound7SmcsRules(input,text,mapped,methodBody,model,point,patches,seen);'
    sources['Round7TransformSyntax.cs']=sources['Round7TransformSyntax.cs'].replace(anchor,anchor+'\n        AddRound7InfiniteTailGuardRules(input,text,mapped,methodBody,model,point,patches,seen);')
    digest=hashlib.sha256(''.join(sources.values()).encode()).hexdigest()
    folder=FOLDER/'helper'/digest[:16];target=folder/'Round7TransformSyntax.dll'
    if not target.exists():
        folder.mkdir(parents=True,exist_ok=True)
        for name,source in sources.items():(folder/name).write_text(source,encoding='utf-8',newline='\n')
        rsp=(base/'compile.rsp').read_text(encoding='utf-8-sig').replace(str(base),str(folder))
        rsp+='"'+str(folder/RULE.name)+'"\n';(folder/'compile.rsp').write_text(rsp,encoding='utf-8')
        compiled=subprocess.run([str(ROSLYN),'/noconfig','@'+str(folder/'compile.rsp')],capture_output=True,text=True)
        assert compiled.returncode==0,compiled.stdout+compiled.stderr
        for path in base.glob('Microsoft.CodeAnalysis*.dll'):shutil.copy2(path,folder/path.name)
        shutil.copy2(base/'Round7TransformSyntax.runtimeconfig.json',folder/'Round7TransformSyntax.runtimeconfig.json')
    return [command[0],str(target)]


SOURCE='''public struct CustomValue {
 public int n;
 public static bool operator ==(CustomValue a,CustomValue b) {return a.n==b.n;}
 public static bool operator !=(CustomValue a,CustomValue b) {return a.n!=b.n;}
 public override bool Equals(object x) {return false;}
 public override int GetHashCode() {return n;}
}
public class Fixture {
 string trace; int n; int limit; int value;
 delegate void Action();
 void Step() {trace+="S;";n++;}
 bool Again() {trace+="C;";return n<limit;}
 void Use(int x) {trace+=x+";";}
 public string Bool(int count) {trace="";n=0;limit=count;do {Step();}while(Again());return trace;}
 public string ExpectedBool(int count) {trace="";n=0;limit=count;while(true) {{Step();}if(!Again()) {break;}}return trace;}
 public string Float(float x) {trace="";n=0;do {Step();if(n==2) {x=1f;}}while(x<0f);return trace;}
 public string ExpectedFloat(float x) {trace="";n=0;while(true) {{Step();if(n==2) {x=1f;}}if(!(x<0f)) {break;}}return trace;}
 public string NestedContinue(int count) {trace="";n=0;limit=count;do {for(int i=0;i<2;i++) {if(i==0) {continue;}Use(i);}Step();}while(Again());return trace;}
 public string ExpectedNestedContinue(int count) {trace="";n=0;limit=count;while(true) {{for(int i=0;i<2;i++) {if(i==0) {continue;}Use(i);}Step();}if(!Again()) {break;}}return trace;}
 public string Protected(int count) {trace="";n=0;limit=count;do {try {Step();}finally {Use(7);}}while(Again());return trace;}
 public string ExpectedProtected(int count) {trace="";n=0;limit=count;while(true) {{try {Step();}finally {Use(7);}}if(!Again()) {break;}}return trace;}
 public string Shadow(int count) {trace="";n=0;this.value=count;do {int value=3;Use(value);Step();}while(n<this.value);return trace;}
 public string ExpectedShadow(int count) {trace="";n=0;this.value=count;while(true) {{int value=3;Use(value);Step();}if(n>=this.value) {break;}}return trace;}
 public string InnerBreak(int count) {trace="";n=0;limit=count;do {Step();if(n==2) {break;}}while(Again());return trace;}
 public string ExpectedInnerBreak(int count) {trace="";n=0;limit=count;while(true) {{Step();if(n==2) {break;}}if(!Again()) {break;}}return trace;}
 void OwnedContinue() {do {Step();if(n==2) {continue;}}while(Again());}
 void SwitchContinue() {do {Step();switch(n) {case 2:continue;}}while(Again());}
 void Captured() {do {Action a=delegate {Use(n);};a();Step();}while(Again());}
 void Label() {do {inner:Step();if(n<0) {goto inner;}}while(Again());}
 void Custom(CustomValue a,CustomValue b) {do {Step();a.n++;}while(a!=b);}
}
public class Program {
 static void Equal(string name,string a,string b) {if(a!=b) {throw new System.Exception(name+":"+a+"!="+b);}System.Console.WriteLine(name+":"+a);}
 public static void Main() {Fixture f=new Fixture();int[] counts=new int[] {0,1,3};
  for(int i=0;i<counts.Length;i++) {int n=counts[i];Equal("bool/"+n,f.Bool(n),f.ExpectedBool(n));Equal("nested/"+n,f.NestedContinue(n),f.ExpectedNestedContinue(n));Equal("eh/"+n,f.Protected(n),f.ExpectedProtected(n));Equal("shadow/"+n,f.Shadow(n),f.ExpectedShadow(n));Equal("break/"+n,f.InnerBreak(n),f.ExpectedInnerBreak(n));}
  float[] values=new float[] {float.NaN,float.NegativeInfinity,-1f,0f,1f,float.PositiveInfinity};
  for(int i=0;i<values.Length;i++) {Equal("float/"+i,f.Float(values[i]),f.ExpectedFloat(values[i]));}
 }
}
'''


def selected(methods,name):return next(m for key,m in methods.items() if key.startswith('Fixture::'+name+'('))


def run_trace(source,name):
    folder=FOLDER/'runtime'/name;folder.mkdir(parents=True,exist_ok=True)
    path=folder/'Fixture.cs';path.write_text(source,encoding='utf-8');binary=folder/'Fixture.exe'
    env=os.environ.copy();env['MONO_PATH']=str(UNITY/'Editor/Data/Mono/lib/mono/unity');env['MONO_CFG_DIR']=str(UNITY/'Editor/Data/Mono/etc')
    compiled=subprocess.run([str(MONO),str(SMCS),'-nologo','-noconfig','-nostdlib','-target:exe','-debug-','-optimize+',
        '-out:'+str(binary),'-r:'+str(CORE),str(path)],env=env,capture_output=True,text=True,timeout=45)
    assert compiled.returncode==0,compiled.stdout+compiled.stderr
    result=subprocess.run([str(MONO),str(binary)],env=env,capture_output=True,text=True,timeout=15)
    assert result.returncode==0,result.stdout+result.stderr
    return result.stdout.splitlines()


def main():
    helper.ensure_helper=ensure_tail_helper
    baseline=compile_toy(SOURCE,'infinite-tail-baseline')
    positives=['Bool','Float','NestedContinue','Protected','Shadow','InnerBreak']
    client=helper.FixtureClient(SOURCE,[CORE]);patches=[];checks=[]
    evidence={'original_infinite_tail_guard':True}
    def query(name,**hints):return [p for p in client.request(name,'do {',**hints) if p['rule']=='smcs-do-infinite-tail-break']
    try:
        for name in positives:
            found=query('string '+name+'(',round7_cycle_rule='smcs-do-infinite-tail-break',**evidence)
            assert found,name
            patches.append((name,found[0]));checks.append(name+':positive')
        for name in ['OwnedContinue','SwitchContinue','Captured','Label','Custom']:
            assert not query('void '+name+'(',round7_cycle_rule='smcs-do-infinite-tail-break',**evidence),name
            checks.append(name+':reject')
        assert not query('string Bool(',**evidence);checks.append('reject-without-opt-in')
        assert not query('string Bool(',round7_cycle_rule='smcs-do-infinite-tail-break');checks.append('reject-without-IL-evidence')
    finally:client.close()
    runtime=run_trace(SOURCE,'baseline');proofs=[]
    facets=('header','locals','instructions','exception_regions','extra_sections','method_attributes','impl_attributes')
    for name,patch in patches:
        before=selected(baseline,name);original=selected(baseline,'Expected'+name)
        changed=apply(SOURCE,patch);after=selected(compile_toy(changed,'infinite-tail-'+name),name)
        differences=[facet for facet in facets if original[facet]!=after[facet]]
        assert not differences,(name,differences)
        assert run_trace(changed,name)==runtime
        # The emitted original has a conditional forward to the explicit back
        # edge, with a break transfer in between. Find that IL fact mechanically.
        hint={}
        for ins in original['instructions']:
            for other in before['instructions']:
                hint=infinite_tail_guard_hints(original,before,ins['offset'],other['offset'])
                if hint:break
            if hint:break
        assert hint,(name,'missing tail evidence')
        proofs.append({'method':name,'original_il_size':original['header']['code_size'],
            'recovered_il_size':before['header']['code_size'],'baseline_prefix':prefix(original,before),
            'candidate_prefix':prefix(original,after),'complete_body_equal':True,'runtime_trace_equal':True,
            'runtime_scenarios':len(runtime),'il_evidence':hint,'original_tail':original['instructions'][-10:],
            'recovered_tail':before['instructions'][-8:]})
    report={'operation':'source-only-tail-break-fixtures','checks':checks,'syntax_safety_checks':len(checks),
        'toy_library_compiles':len(proofs)+1,'exact_reference_body_matches':len(proofs),
        'runtime_runs':len(proofs)+1,'target_compilations':0,'proofs':proofs,
        'earlier_fixture_design_failure':{'compilations':1,'errors':4,'code':'CS0135',
            'detail':'Unqualified field value and a same-named local inside the do block conflict in historical smcs; fixture corrected to explicit this.value. This was an invalid fixture source, not an emitted-code mismatch.'},
        'rule_sha256':hashlib.sha256(RULE.read_bytes()).hexdigest(),
        'hint_sha256':hashlib.sha256((ROOT/'tools/round7_infinite_tail_guard_hints.py').read_bytes()).hexdigest(),
        'driver_sha256':hashlib.sha256(Path(__file__).read_bytes()).hexdigest(),
        'smcs_sha256':hashlib.sha256(SMCS.read_bytes()).hexdigest()}
    (ROOT/'reports/round7-infinite-tail-guard-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({k:report[k] for k in ('syntax_safety_checks','toy_library_compiles','exact_reference_body_matches','runtime_runs')}))


if __name__=='__main__':main()
