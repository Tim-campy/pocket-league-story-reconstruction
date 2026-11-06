"""Paired branch-layout fixtures; no target compilation and no IL patching."""
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
from round7_engine import prefix,il_cells
from round7_balanced_branch_hints import balanced_branch_hints

FOLDER=ROOT / 'build/round7/balanced-branch-experiment'
RULE=ROOT / 'tools/Round7BalancedBranchRules.cs'
BASE_HELPER=helper.ensure_helper


def ensure_balanced_helper():
    command=BASE_HELPER();base=Path(command[-1]).parent
    sources={p.name:p.read_text(encoding='utf-8-sig') for p in base.glob('*.cs')}
    sources[RULE.name]=RULE.read_text(encoding='utf-8-sig')
    anchor='AddRound7SmcsRules(input,text,mapped,methodBody,model,point,patches,seen);'
    sources['Round7TransformSyntax.cs']=sources['Round7TransformSyntax.cs'].replace(anchor,anchor+'\n        AddRound7BalancedBranchRules(input,text,mapped,methodBody,model,point,patches,seen);')
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


SOURCE='''public struct CustomValue {
 public int n;
 public static bool operator ==(CustomValue a,CustomValue b) {return a.n==b.n;}
 public static bool operator !=(CustomValue a,CustomValue b) {return a.n!=b.n;}
 public override bool Equals(object x) {return false;}
 public override int GetHashCode() {return n;}
}
public class Fixture {
 string trace;
 delegate void Action();
 void Use(int n) {trace+=n+";";}
 bool Outer(bool value) {trace+="O;";return value;}
 bool Check(bool value) {trace+="C;";return value;}
 public string Pair(bool outer,bool inner) {trace="";if(Outer(outer)) {int i=0;while(i<3) {if(!Check(inner)) {Use(i);}i++;}}else {}Use(9);return trace;}
 public string ExpectedPair(bool outer,bool inner) {trace="";if(Outer(outer)) {int i=0;while(i<3) {if(Check(inner)) {}else {Use(i);}i++;}}Use(9);return trace;}
 public string Float(bool outer,float x) {trace="";if(Outer(outer)) {int i=0;while(i<3) {if(!(x<0f)) {Use(i);}i++;}}else {}Use(9);return trace;}
 public string ExpectedFloat(bool outer,float x) {trace="";if(Outer(outer)) {int i=0;while(i<3) {if(x<0f) {}else {Use(i);}i++;}}Use(9);return trace;}
 public void NoAncestor(bool inner) {if(!Check(inner)) {Use(0);}if(Outer(inner)) {Use(1);}else {}}
 public void NonemptyElse(bool outer,bool inner) {if(outer) {if(!Check(inner)) {Use(0);}}else {Use(1);}}
 public void ReturningDonor(bool outer,bool inner) {if(outer) {if(!Check(inner)) {Use(0);}return;}else {}}
 public void LambdaBoundary(bool outer,bool inner) {if(outer) {Action work=delegate {if(!Check(inner)) {Use(0);}};work();}else {}}
 public void Custom(bool outer,CustomValue a,CustomValue b) {if(outer) {if(a!=b) {Use(0);}}else {}}
}
public class Program {
 static void Equal(string name,string a,string b) {if(a!=b) {throw new System.Exception(name+":"+a+"!="+b);}System.Console.WriteLine(name+":"+a);}
 public static void Main() {Fixture f=new Fixture();bool[] flags=new bool[] {false,true};
  for(int a=0;a<flags.Length;a++) {for(int b=0;b<flags.Length;b++) {Equal("pair/"+a+"/"+b,f.Pair(flags[a],flags[b]),f.ExpectedPair(flags[a],flags[b]));}}
  float[] values=new float[] {float.NaN,float.NegativeInfinity,-1f,0f,1f,float.PositiveInfinity};
  for(int a=0;a<flags.Length;a++) {for(int b=0;b<values.Length;b++) {Equal("float/"+a+"/"+b,f.Float(flags[a],values[b]),f.ExpectedFloat(flags[a],values[b]));}}
 }
}
'''


def run_trace(source,name):
    folder=FOLDER / 'runtime' / name;folder.mkdir(parents=True,exist_ok=True)
    path=folder / 'Fixture.cs';path.write_text(source,encoding='utf-8');binary=folder / 'Fixture.exe'
    env=os.environ.copy();env['MONO_PATH']=str(UNITY / 'Editor/Data/Mono/lib/mono/unity');env['MONO_CFG_DIR']=str(UNITY / 'Editor/Data/Mono/etc')
    compiled=subprocess.run([str(MONO),str(SMCS),'-nologo','-noconfig','-nostdlib','-target:exe','-debug-','-optimize+',
        '-out:'+str(binary),'-r:'+str(CORE),str(path)],env=env,capture_output=True,text=True,timeout=45)
    assert compiled.returncode==0,compiled.stdout+compiled.stderr
    result=subprocess.run([str(MONO),str(binary)],env=env,capture_output=True,text=True,timeout=15)
    assert result.returncode==0,result.stdout+result.stderr
    return result.stdout.splitlines()


def selected(methods,name):return next(m for key,m in methods.items() if key.startswith('Fixture::'+name+'('))


def main():
    helper.ensure_helper=ensure_balanced_helper
    baseline=compile_toy(SOURCE,'balanced-baseline')
    client=helper.FixtureClient(SOURCE,[CORE]);patches=[];checks=[]
    def query(name,needle,**hints):return client.request(name,needle,**hints)
    evidence={'balanced_missing_long_branch':True,'balanced_extra_fallthrough_long_branch':True}
    try:
        for name,needle in [('Pair','if(!Check'),('Float','if(!(x')]:
            before=selected(baseline,name);original=selected(baseline,'Expected'+name)
            # Compute evidence from the actual toy compiler output, not from
            # source assumptions or hand-authored fixture IL.
            hint={}
            for ins in original['instructions']:
                hint=balanced_branch_hints(original,before,ins['offset'],ins['offset'])
                if hint:break
            assert hint,(name,'missing compiled five-byte proof')
            found=[p for p in query('string '+name+'(',needle,round7_cycle_rule='smcs-balanced-empty-arms',**hint) if p['rule']=='smcs-balanced-empty-arms']
            assert found,name
            patches.append((name,found[0]));checks.append(name+':paired-proposal')
        for name,needle in [('NoAncestor','if(!Check'),('NonemptyElse','if(!Check'),('ReturningDonor','if(!Check'),('LambdaBoundary','if(!Check'),('Custom','if(a!=')]:
            assert not any(p['rule']=='smcs-balanced-empty-arms' for p in query('void '+name+'(',needle,round7_cycle_rule='smcs-balanced-empty-arms',**evidence)),name
            checks.append(name+':reject')
        for hints in [{},{'balanced_missing_long_branch':True},{'balanced_extra_fallthrough_long_branch':True}]:
            assert not any(p['rule']=='smcs-balanced-empty-arms' for p in query('string Pair(','if(!Check',round7_cycle_rule='smcs-balanced-empty-arms',**hints))
            checks.append('Pair:reject-incomplete-IL-evidence')
        assert not any(p['rule']=='smcs-balanced-empty-arms' for p in query('string Pair(','if(!Check',**evidence));checks.append('Pair:reject-no-opt-in')
    finally:client.close()
    proofs=[];runtime=run_trace(SOURCE,'baseline');facets=('header','locals','instructions','exception_regions','extra_sections','method_attributes','impl_attributes')
    for name,patch in patches:
        before=selected(baseline,name);original=selected(baseline,'Expected'+name)
        changed=apply(SOURCE,patch);built=compile_toy(changed,'balanced-paired-'+name);after=selected(built,name)
        differences=[facet for facet in facets if original[facet]!=after[facet]];assert not differences,(name,differences)
        # Two isolated edits are negative controls: their +/-5 sizes alter an
        # earlier matching branch operand; the paired patch preserves it.
        if name=='Pair':
            recipient_only=SOURCE.replace('if(!Check(inner)) {Use(i);}','if(Check(inner)) {}else {Use(i);}',1)
        else:
            recipient_only=SOURCE.replace('if(!(x<0f)) {Use(i);}','if(x<0f) {}else {Use(i);}',1)
        start=SOURCE.index('public string '+name+'(');end=SOURCE.index('public string Expected'+name+'(',start)
        donor_piece=SOURCE[start:end].replace('else {}','',1)
        donor_only=SOURCE[:start]+donor_piece+SOURCE[end:]
        singles=[];old_prefix=prefix(original,before)
        for label,source,delta in [('recipient-only',recipient_only,5),('donor-only',donor_only,-5)]:
            single=selected(compile_toy(source,'balanced-'+label+'-'+name),name)
            assert single['header']['code_size']-before['header']['code_size']==delta
            assert prefix(original,single)<old_prefix,(name,label,old_prefix,prefix(original,single))
            singles.append({'edit':label,'size_delta':delta,'raw_prefix':prefix(original,single),'prefix_regressed':True})
        assert il_cells(after)[:old_prefix]==il_cells(original)[:old_prefix]
        assert after['header']['code_size']==before['header']['code_size']
        assert run_trace(changed,name)==runtime
        proofs.append({'method':name,'baseline_prefix':old_prefix,'paired_prefix':prefix(original,after),
            'original_il_size':original['header']['code_size'],'paired_size_delta':0,'complete_body_equal':True,
            'singles':singles,'runtime_scenarios':len(runtime),'runtime_trace_equal':True})
    report={'operation':'balanced-source-branch-fixtures','syntax_safety_checks':len(checks),
        'toy_library_compiles':1+len(proofs)*3,'exact_reference_body_matches':len(proofs),
        'runtime_runs':len(proofs)+1,'target_compilations':0,'checks':checks,'proofs':proofs,
        'rule_sha256':hashlib.sha256(RULE.read_bytes()).hexdigest(),
        'hint_sha256':hashlib.sha256((ROOT/'tools/round7_balanced_branch_hints.py').read_bytes()).hexdigest(),
        'driver_sha256':hashlib.sha256(Path(__file__).read_bytes()).hexdigest(),
        'smcs_sha256':hashlib.sha256(SMCS.read_bytes()).hexdigest()}
    (ROOT / 'reports/round7-balanced-branch-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({k:report[k] for k in ('syntax_safety_checks','toy_library_compiles','exact_reference_body_matches','runtime_runs')}))


if __name__=='__main__':main()
