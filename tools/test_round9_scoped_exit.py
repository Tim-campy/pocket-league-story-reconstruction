"""Isolated smcs exit-template toy controls; never compile the game."""
import hashlib,json,os,subprocess,sys,shutil
from pathlib import Path
SOURCE_DIR=Path(__file__).resolve().parent;ROOT=next(p for p in SOURCE_DIR.parents if (p/'AGENTS.md').is_file());HERE=ROOT/'build/round9/d-scoped-exit';HERE.mkdir(parents=True,exist_ok=True);sys.path.insert(0,str(ROOT/'tools'))
from compiler_probes import MONO,SMCS,UNITY,ROSLYN
SOURCE='''using System;
public sealed class Flag {public bool Value;public static int Calls;public static bool operator true(Flag f){Calls++;return f.Value;}public static bool operator false(Flag f){Calls++;return !f.Value;}}
public class Fixture {public int Field;
 public void ExpectedFlat(bool guard,int n){if(guard){for(int i=0;i<n;i++){if(i==2)break;Field+=i;}return;} {Field+=7;}}
 public void Flat(bool guard,int n){if(guard){for(int i=0;i<n;i++){if(i==2)break;Field+=i;}}else{Field+=7;}}
 public void ExpectedEmpty(bool guard){if(guard){return;} {Field+=7;}}
 public void Empty(bool guard){if(guard){}else{Field+=7;}}
 public void ExpectedCustom(Flag guard){if(guard){Field++;return;} {Field+=7;}}
 public void Custom(Flag guard){if(guard){Field++;}else{Field+=7;}}
 public void ExpectedFloat(float guard){if(guard<0f){Field++;return;} {Field+=7;}}
 public void Float(float guard){if(guard<0f){Field++;}else{Field+=7;}}
 public void ExpectedEmbedded(bool outer,bool guard){if(outer){Field+=2;}else{if(guard){Field++;return;} {Field+=7;}}}
 public void Embedded(bool outer,bool guard){if(outer){Field+=2;}else if(guard){Field++;}else{Field+=7;}}
 public void ExpectedEh(bool guard){if(guard){try{Field++;}catch(Exception){Field+=2;}return;} {Field+=7;}}
 public void Eh(bool guard){if(guard){try{Field++;}catch(Exception){Field+=2;}}else{Field+=7;}}
 public void ExpectedScope(bool outer,bool guard,int n){if(outer){if(guard){}else{for(int i=0;i<n;i++){if(i==2)continue;Field+=i;}}}Field+=7;}
 public void Scope(bool outer,bool guard,int n){if(outer){if(guard)return;for(int i=0;i<n;i++){if(i==2)continue;Field+=i;}}Field+=7;}
 public void ExpectedNested(bool guard,int n){if(guard){Field++;if(n>0){Field+=n;}}else{Field+=7;}}
 public void Nested(bool guard,int n){if(guard){Field++;if(n>0){Field+=n;}return;}Field+=7;}
 public void ExpectedLoop(bool guard,int n){if(guard){for(int i=0;i<n;i++){if(i==2)break;Field+=i;}}else{for(int j=0;j<n;j++){Field-=j;}}}
 public void Loop(bool guard,int n){if(guard){for(int i=0;i<n;i++){if(i==2)break;Field+=i;}return;}for(int j=0;j<n;j++){Field-=j;}}
 public void ExpectedScopedFloat(bool outer,float guard){if(outer){if(guard<0f){}else{Field++;}}Field+=7;}
 public void ScopedFloat(bool outer,float guard){if(outer){if(guard<0f)return;Field++;}Field+=7;}
 public void OldCoveredEmpty(bool guard){if(guard)return;Field+=7;}
 public void OldCoveredSimple(bool guard){if(guard){Field++;return;}Field+=7;}
 public void RejectFollowing(bool guard){if(guard){}else{Field++;}Field+=7;}
 public int RejectValue(bool guard){if(guard){Field++;}else{Field+=7;}return Field;}
 public void RejectLabel(bool guard){if(guard){Field++;}else{goto label;label:Field+=7;}}
 public void RejectCapture(bool guard){if(guard){System.Threading.ThreadStart a=delegate{Field++;};a();}else{Field+=7;}}
}
'''
def ensure_helper():
 folder=HERE/'helper';folder.mkdir(exist_ok=True);vs=next(p for p in ROSLYN.parents if (p/'dotnet').is_dir());runtime=vs/'dotnet/net10.0/runtime';framework=runtime/'shared/Microsoft.NETCore.App/10.0.10'
 refs=[framework/(n+'.dll') for n in ('System.Private.CoreLib','System.Runtime','System.Console','System.Linq','System.Collections','System.Text.Json','System.Collections.Immutable','System.Memory','netstandard')]+[ROSLYN.parent/(n+'.dll') for n in ('Microsoft.CodeAnalysis','Microsoft.CodeAnalysis.CSharp')]
 target=folder/'FixtureDriver.dll';args=['/nologo','/nostdlib+','/target:exe','/out:'+str(target)]+['/r:'+str(p) for p in refs]+[str(SOURCE_DIR/'Round9ScopedExitCandidates.cs'),str(SOURCE_DIR/'Round9ScopedExitFixtureDriver.cs')]
 rsp=folder/'compile.rsp';rsp.write_text('\n'.join('"'+a+'"' for a in args),encoding='utf-8');process=subprocess.run([str(ROSLYN),'/noconfig','@'+str(rsp)],capture_output=True,text=True);assert process.returncode==0,process.stdout+process.stderr
 for ref in refs[-2:]:shutil.copyfile(ref,folder/ref.name)
 target.with_suffix('.runtimeconfig.json').write_text(json.dumps({'runtimeOptions':{'tfm':'net10.0','framework':{'name':'Microsoft.NETCore.App','version':'10.0.10'}}}),encoding='utf-8')
 return [str(runtime/'dotnet.exe'),str(target)]
class Client:
 def __init__(self):self.p=subprocess.Popen(ensure_helper(),stdin=subprocess.PIPE,stdout=subprocess.PIPE,stderr=subprocess.PIPE,text=True,encoding='utf-8')
 def generate(self,source,point,family,opcode,join,joinpoint=-1):
  request={'source':source,'point':point,'family':family,'original_transfer_opcode':opcode,'join_kind':join,'join_source_point':joinpoint};self.p.stdin.write(json.dumps(request)+'\n');self.p.stdin.flush();response=json.loads(self.p.stdout.readline());assert 'error'not in response,response;return response['patches']
 def close(self):self.p.stdin.close();self.p.wait(timeout=10)
def compile_toy(source,label):
 folder=HERE/'fixtures'/label;folder.mkdir(parents=True,exist_ok=True);path=folder/'Fixture.cs';path.write_text(source,encoding='utf-8');binary=folder/'Fixture.dll';env=os.environ.copy();env['MONO_PATH']=str(UNITY/'Editor/Data/Mono/lib/mono/unity');env['MONO_CFG_DIR']=str(UNITY/'Editor/Data/Mono/etc')
 process=subprocess.run([str(MONO),str(SMCS),'-nologo','-noconfig','-nostdlib','-optimize+','-debug-','-target:library','-out:'+str(binary),'-r:'+str(ROOT/'original/extracted/assets/bin/Data/Managed/mscorlib.dll'),str(path)],env=env,capture_output=True,text=True,timeout=45);assert process.returncode==0,process.stdout+process.stderr
 process=subprocess.run([str(ROOT/'build/tools/ProbeInspector.exe'),str(binary),'--all'],capture_output=True,text=True,encoding='utf-8',timeout=25,check=True);dto=json.loads(process.stdout);(folder/'methods.json').write_text(json.dumps(dto,indent=2),encoding='utf-8');return {m['stable_id'].split('::',1)[1].split('(',1)[0]:m for m in dto['selected_methods'] if m['stable_id'].startswith('Fixture::')}
def locate(source,name):return source.index('if(',source.index(' '+name+'('))
def apply(source,patch):assert source[patch['start']:patch['end']]==patch['before'];return source[:patch['start']]+patch['after']+source[patch['end']:]
def main():
 client=Client();baseline=compile_toy(SOURCE,'baseline');rows=[]
 for name in ('Flat','Empty','Custom','Float','Embedded','Eh'):
  family='terminal-if-arm-return-flatten-else';probe=SOURCE.index('if(guard)',SOURCE.index(' void Embedded(')) if name=='Embedded' else locate(SOURCE,name);patches=client.generate(SOURCE,probe,family,'ret','method-return');assert patches,name;patch=patches[0];candidate=apply(SOURCE,patch);actual=compile_toy(candidate,name)[name];expected=baseline['Expected'+name];assert actual['raw_body_hex']==expected['raw_body_hex'],(name,actual['instructions'],expected['instructions']);rows.append({'name':name,'family':family,'body_exact':True,'locals_equal':actual['locals']==expected['locals'],'source_patch':patch})
 for name in ('Scope','Nested','Loop','ScopedFloat'):
  family='scoped-terminal-arm-common-join';start=SOURCE.index(' void '+name+'(');point=SOURCE.index('if(guard',start);join='scope-next' if name in ('Scope','ScopedFloat') else 'method-ret';joinpoint=SOURCE.index('Field+=7;',point) if join=='scope-next' else -1
  patches=client.generate(SOURCE,point,family,'br',join,joinpoint);assert patches,name;patch=patches[0];actual=compile_toy(apply(SOURCE,patch),name)[name];expected=baseline['Expected'+name];assert actual['raw_body_hex']==expected['raw_body_hex'],(name,actual['instructions'],expected['instructions']);rows.append({'name':name,'family':family,'body_exact':True,'locals_equal':actual['locals']==expected['locals'],'source_patch':patch})
 negatives=[]
 for name,opcode,join in [('RejectFollowing','ret','method-return'),('RejectValue','ret','method-return'),('RejectLabel','ret','method-return'),('RejectCapture','ret','method-return'),('Empty','br','method-return')]:
  got=client.generate(SOURCE,locate(SOURCE,name),'terminal-if-arm-return-flatten-else',opcode,join);assert not got,name;negatives.append(name)
 point=SOURCE.index('if(guard',SOURCE.index(' void Scope('));assert not client.generate(SOURCE,point,'scoped-terminal-arm-common-join','br','method-ret');negatives.append('Scope-method-ret-wrong-target')
 assert not client.generate(SOURCE,point,'scoped-terminal-arm-common-join','br','scope-next',SOURCE.index(' void RejectFollowing('));negatives.append('Scope-next-wrong-source-boundary')
 assert not client.generate(SOURCE,point,'scoped-terminal-arm-common-join','leave','scope-next',SOURCE.index('Field+=7;',point));negatives.append('leave-is-not-br')
 for name in ('OldCoveredEmpty','OldCoveredSimple'):
  assert not client.generate(SOURCE,locate(SOURCE,name),'scoped-terminal-arm-common-join','br','method-ret');negatives.append(name+'-already-v7-repertoire')
 client.close();report={'operation':'isolated-smcs-scoped-exit-toy-fixtures','game_compilations':0,'source_sha256':hashlib.sha256(SOURCE.encode()).hexdigest(),'positive_body_exact':len(rows),'negative_rejections':len(negatives),'positives':rows,'negatives':negatives,'compiler_sha256':hashlib.sha256(SMCS.read_bytes()).hexdigest(),'initial_harness_failures':['Unity profile has no System.Action; replaced toy negative with System.Threading.ThreadStart','Negative-method locator assumed void and missed int method; locator fixed before final assertions'],'limitations':'No game candidate has been compiled; all adoption still needs complete original-signature model gate and full body oracle.'}
 (HERE/'fixture-report.json').write_text(json.dumps(report,indent=2),encoding='utf-8');print(json.dumps({k:v for k,v in report.items() if k not in ('positives',)},indent=2))
if __name__=='__main__':main()
