"""Independent smcs fixture and syntax rejections; all outputs stay in WIP."""
import hashlib,json,os,shutil,subprocess,sys
from pathlib import Path
"""Reuse already built frozen tools. Never build or edit a tool or game source."""
import hashlib,sys
from pathlib import Path
ROOT=next(p for p in Path(__file__).resolve().parents if (p/'AGENTS.md').is_file())
sys.path.insert(0,str(ROOT/'tools'))
from compiler_probes import ROSLYN
import round7_transform,round7_slot_model


def bind():
    vs=next(p for p in ROSLYN.parents if (p/'dotnet').is_dir())
    runtime=vs/'dotnet/net10.0/runtime/dotnet.exe'
    sources=[ROOT/('tools/Round7'+stem+'.cs') for stem in ('TransformSyntax','NewRules','AliasRules','SmcsRules','RoslynRules')]
    sources += [ROOT/'tools'/name for name in ('round7_smcs_cycle_rules.cs','Round7RoslynCycleRules.cs','Round7SmcsExitRules.cs',
        'Round7SmcsGuardRules.cs','Round7StringConcatRules.cs','Round7LoopConditionRules.cs','Round7SmcsSwitchRules.cs',
        'Round7RoslynDeclarationRules.cs','Round7TerminalArmRules.cs','Round7BalancedBranchRules.cs','Round7InfiniteTailGuardRules.cs')]
    identity=hashlib.sha256(b''.join(p.read_bytes() for p in sources)+str(ROSLYN).encode()).hexdigest()
    transform=ROOT/'build/round7-transform'/identity[:16]/'Round7TransformSyntax.dll'
    model_identity=hashlib.sha256((ROOT/'tools/Round7SlotModel.cs').read_bytes()+str(ROSLYN).encode()).hexdigest()
    model=ROOT/'build/round7/slot-model'/model_identity[:16]/'Round7SlotModel.dll'
    assert transform.is_file(),str(transform)
    assert model.is_file(),str(model)
    round7_transform.ensure_helper=lambda:[str(runtime),str(transform)]
    round7_slot_model.ensure_helper=lambda:[str(runtime),str(model)]
    return {'transform_sha256':hashlib.sha256(transform.read_bytes()).hexdigest(),
        'model_sha256':hashlib.sha256(model.read_bytes()).hexdigest(),
        'transform_source_identity':identity,'model_source_identity':model_identity}

sys.path.insert(0,str(ROOT/'tools'))
import round7_transform
from round7_slot_model import SlotModelClient
from compiler_probes import MONO,SMCS,UNITY,ROSLYN
from test_round7_smcs_rules import apply
FOLDER=ROOT/'build/round8/loop-polarity'
CORE=ROOT/'original/extracted/assets/bin/Data/Managed/mscorlib.dll'
RULE=ROOT/'tools/Round8SelectorContinueComposition.cs'
BASE=bind()
ORIGINAL_HELPER=round7_transform.ensure_helper


def ensure_composition_helper():
    command=ORIGINAL_HELPER();base=Path(command[-1]).parent
    # RSP source paths are fixed inputs; snapshot every source into this WIP.
    rsp=(base/'compile.rsp').read_text(encoding='utf-8-sig')
    paths=[Path(line.strip().strip('"')) for line in rsp.splitlines() if line.strip().strip('"').endswith('.cs')]
    sources={p.name:p.read_text(encoding='utf-8-sig') for p in paths}
    sources[RULE.name]=RULE.read_text(encoding='utf-8-sig')
    anchor='AddRound7SmcsRules(input,text,mapped,methodBody,model,point,patches,seen);'
    sources['Round7TransformSyntax.cs']=sources['Round7TransformSyntax.cs'].replace(anchor,anchor+'\n        AddRound8SelectorContinueComposition(input,text,mapped,methodBody,model,point,patches,seen);')
    identity=hashlib.sha256(''.join(sources.values()).encode()).hexdigest()
    folder=FOLDER/'helper'/identity[:16];binary=folder/'Round7TransformSyntax.dll'
    if not binary.exists():
        folder.mkdir(parents=True,exist_ok=True)
        for name,source in sources.items():(folder/name).write_text(source,encoding='utf-8',newline='\n')
        for p in paths:rsp=rsp.replace(str(p),str(folder/p.name))
        rsp=rsp.replace(str(base/'Round7TransformSyntax.dll'),str(binary))
        rsp+='"'+str(folder/RULE.name)+'"\n';(folder/'compile.rsp').write_text(rsp,encoding='utf-8')
        process=subprocess.run([str(ROSLYN),'/noconfig','@'+str(folder/'compile.rsp')],capture_output=True,text=True)
        assert process.returncode==0,process.stdout+process.stderr
        for lib in base.glob('Microsoft.CodeAnalysis*.dll'):shutil.copy2(lib,folder/lib.name)
        shutil.copy2(base/'Round7TransformSyntax.runtimeconfig.json',folder/'Round7TransformSyntax.runtimeconfig.json')
    return [command[0],str(binary)]


SOURCE='''public class Box {public int n;public Box(int value){n=value;}}
public class Other {public int n;public Other(int value){n=value;}}
public class Fixture {
 delegate int Getter();
 string trace;int choice;volatile int volatileSelector;
 int GetSelector(){trace+="S;";return choice;}
 public string Pair(int[] values,int needle,int selector) {
  trace="";choice=selector;bool found=false;
  for(int i=0;i<values.Length;i++){int value=values[i];if(value!=needle){continue;}found=true;break;}
  switch(GetSelector()){
   case 0:{Box first=new Box(10);trace+="A;"+first.n;return trace+found;}
   case 1:{Other second=new Other(20);trace+="B;"+second.n;return trace+found;}
  }
  return trace+found;
 }
 public string ExpectedPair(int[] values,int needle,int selector) {
  trace="";choice=selector;bool found=false;
  for(int i=0;i<values.Length;i++){int value=values[i];if(value==needle){found=true;break;}}
  int recoveredSelector=GetSelector();
  if(recoveredSelector==0){Box first=new Box(10);trace+="A;"+first.n;return trace+found;}
  else if(recoveredSelector==1){Other second=new Other(20);trace+="B;"+second.n;return trace+found;}
  return trace+found;
 }
 public string Float(float[] values,int selector) {
  trace="";choice=selector;bool found=false;
  for(int i=0;i<values.Length;i++){float value=values[i];if(!(value<0f)){continue;}found=true;break;}
  switch(GetSelector()){
   case 0:{Box first=new Box(10);trace+="A;"+first.n;return trace+found;}
   case 1:{Other second=new Other(20);trace+="B;"+second.n;return trace+found;}
  }
  return trace+found;
 }
 public string ExpectedFloat(float[] values,int selector) {
  trace="";choice=selector;bool found=false;
  for(int i=0;i<values.Length;i++){float value=values[i];if(value<0f){found=true;break;}}
  int recoveredSelector=GetSelector();
  if(recoveredSelector==0){Box first=new Box(10);trace+="A;"+first.n;return trace+found;}
  else if(recoveredSelector==1){Other second=new Other(20);trace+="B;"+second.n;return trace+found;}
  return trace+found;
 }
 public int Volatile(int[] values,int needle) {
  bool found=false;for(int i=0;i<values.Length;i++){if(values[i]!=needle){continue;}found=true;break;}
  switch(volatileSelector){case 0:{return found?1:0;}case 1:{return 2;}}return 3;
 }
 public int Capture(int[] values,int needle) {
  bool found=false;for(int i=0;i<values.Length;i++){if(values[i]!=needle){continue;}found=true;break;}
  switch(GetSelector()){case 0:{Getter getter=delegate{return found?1:0;};return getter();}case 1:{return 2;}}return 3;
 }
 public int CrossCase(int[] values,int needle) {
  bool found=false;for(int i=0;i<values.Length;i++){if(values[i]!=needle){continue;}found=true;break;}
  switch(GetSelector()){case 0:int shared=1;return shared;case 1:shared=2;return shared;}return found?3:4;
 }
 public int CaseBreak(int[] values,int needle) {
  bool found=false;for(int i=0;i<values.Length;i++){if(values[i]!=needle){continue;}found=true;break;}
  switch(GetSelector()){case 0:{found=true;break;}case 1:{return 2;}}return found?3:4;
 }
}
public class Program {
 static void Equal(string label,string a,string b){if(a!=b)throw new System.Exception(label+":"+a+"!="+b);System.Console.WriteLine(label+":"+a);}
 public static void Main(){Fixture f=new Fixture();
  for(int v=0;v<3;v++){int[] ints;if(v==0)ints=new int[0];else if(v==1)ints=new int[]{1,2,3};else ints=new int[]{2,2};for(int needle=1;needle<=4;needle++)for(int choice=-1;choice<=2;choice++)Equal("pair/"+v+"/"+needle+"/"+choice,f.Pair(ints,needle,choice),f.ExpectedPair(ints,needle,choice));}
  for(int v=0;v<4;v++){float[] floats;if(v==0)floats=new float[0];else if(v==1)floats=new float[]{float.NaN,float.PositiveInfinity,0f};else if(v==2)floats=new float[]{float.NaN,-1f};else floats=new float[]{float.NegativeInfinity,1f};for(int choice=-1;choice<=2;choice++)Equal("float/"+v+"/"+choice,f.Float(floats,choice),f.ExpectedFloat(floats,choice));}
 }
}
'''


def compile_fixture(source,label,exe=False):
 folder=FOLDER/'fixtures'/label;folder.mkdir(parents=True,exist_ok=True)
 path=folder/'Fixture.cs';path.write_text(source,encoding='utf-8',newline='\n')
 binary=folder/('Fixture.exe' if exe else 'Fixture.dll')
 environment=os.environ.copy();environment['MONO_PATH']=str(UNITY/'Editor/Data/Mono/lib/mono/unity');environment['MONO_CFG_DIR']=str(UNITY/'Editor/Data/Mono/etc')
 cmd=[str(MONO),str(SMCS),'-nologo','-noconfig','-nostdlib','-optimize+','-debug-',
    '-target:'+('exe' if exe else 'library'),'-out:'+str(binary),'-r:'+str(CORE),str(path)]
 process=subprocess.run(cmd,env=environment,capture_output=True,text=True,timeout=45)
 assert process.returncode==0,process.stdout+process.stderr
 if exe:
  process=subprocess.run([str(MONO),str(binary)],env=environment,capture_output=True,text=True,timeout=15)
  assert process.returncode==0,process.stdout+process.stderr
  return process.stdout.splitlines()
 process=subprocess.run([str(ROOT/'build/tools/ProbeInspector.exe'),str(binary),'--all'],capture_output=True,text=True,encoding='utf-8',timeout=20,check=True)
 methods=json.loads(process.stdout)['selected_methods']
 (folder/'methods.json').write_text(json.dumps(methods,indent=2)+'\n',encoding='utf-8')
 return {m['stable_id']:m for m in methods}


def main():
 round7_transform.ensure_helper=ensure_composition_helper
 folder=FOLDER/'syntax';folder.mkdir(exist_ok=True)
 path=folder/'Fixture.cs';path.write_text(SOURCE,encoding='utf-8')
 manifest=folder/'manifest.json';manifest.write_text(json.dumps({'sources':[str(path)],'references':[str(CORE)]}),encoding='utf-8')
 hints={'round8_selector_named_local_proved':True,'round8_selector_call_name':'GetSelector'}
 patches=[];checks=[]
 with round7_transform.TransformClient(manifest) as syntax:
  def query(name,**flags):
   point=SOURCE.index('if(',SOURCE.index(' '+name+'('))
   response=syntax.request(path,source=SOURCE,offset=len(SOURCE[:point].encode('utf-16-le'))//2,
    compiler='smcs',max_candidates=4096,**flags)
   return [p for p in response.get('patches',[]) if p['rule']=='calculated-selector-continue-composition']
  for name in ('Pair','Float'):
   found=query(name,round7_cycle_rule='calculated-selector-continue-composition',**hints);assert len(found)==1,(name,found)
   patches.append((name,found[0]));checks.append(name+':positive')
  for name in ('Volatile','Capture','CrossCase','CaseBreak'):
   assert not query(name,round7_cycle_rule='calculated-selector-continue-composition',**hints),name
   checks.append(name+':reject')
  assert not query('Pair',**hints);checks.append('no-opt-in:reject')
  assert not query('Pair',round7_cycle_rule='calculated-selector-continue-composition');checks.append('no-evidence:reject')
 baseline=compile_fixture(SOURCE,'baseline');trace=compile_fixture(SOURCE,'baseline-run',True);proofs=[]
 select=lambda methods,name:next(m for key,m in methods.items() if key.startswith('Fixture::'+name+'('))
 facets=('header','locals','instructions','exception_regions','extra_sections','method_attributes','impl_attributes')
 for name,patch in patches:
  candidate=apply(SOURCE,patch);after=select(compile_fixture(candidate,name),name);original=select(baseline,'Expected'+name)
  differences=[facet for facet in facets if original[facet]!=after[facet]];assert not differences,(name,differences)
  assert compile_fixture(candidate,name+'-run',True)==trace
  before=select(baseline,name)
  proofs.append({'method':name,'original_il_bytes':original['header']['code_size'],'full_body_equal':True,
    'before_local_types':before['locals'],'original_local_types':original['locals'],'candidate_local_types':after['locals'],
    'runtime_scenarios':len(trace),'runtime_traces_equal':True,
    'float_un_present':any('.un' in i['opcode'] for i in after['instructions']) if name=='Float' else None})
 report={'operation':'known-family-composition-fixtures','syntax_controls':len(checks),'checks':checks,
  'toy_library_compilations':3,'toy_executable_compilations':3,'runtime_runs':3,'proofs':proofs,'game_compilations':0,
  'earlier_fixture_failure':'Initial runtime harness constructed jagged arrays and failed before comparisons with ArrayTypeMismatchException in Program.Main. Harness changed to one-dimensional array arguments; cause of runtime jagged-array failure UNKNOWN; not a transformation result.',
  'rule_sha256':hashlib.sha256(RULE.read_bytes()).hexdigest(),
  'driver_sha256':hashlib.sha256(Path(__file__).read_bytes()).hexdigest(),
  'smcs_sha256':hashlib.sha256(SMCS.read_bytes()).hexdigest()}
 (ROOT/'reports/round8-selector-continue-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
 print(json.dumps({'controls':len(checks),'body_matches':len(proofs),'runtime_scenarios':len(trace)}))


if __name__=='__main__':main()
