"""Opt-in isolated syntax/real-smcs fixtures; no target assembly compilation."""
import hashlib,json,os,shutil,subprocess,sys
from pathlib import Path
ROOT=next(p for p in Path(__file__).resolve().parents if (p/'tools/round7_slot_model.py').exists())
sys.path.insert(0,str(ROOT/'tools'))
from compiler_probes import ROSLYN
from compare_kairo import body_cells,sha
FOLDER=ROOT/'build/round7/terminal-arm-experiment';FOLDER.mkdir(parents=True,exist_ok=True)
RULE='smcs-terminal-actions-common-exit'
SOURCE='''// UTF16 control: \U0001f50e
using System;
class Resource:IDisposable {public void Dispose(){}}
class Fixture {
 delegate void Work(); static int state;
 static void Use(int value){state+=value;} static bool Other(){return state!=0;}
 public static void Original(bool flag){if(flag){Use(1);Use(2);}else{int x=3;Use(x);}}
 public static void Recovered(bool flag){if(flag){Use(1);Use(2);return;}int x=3;Use(x);}
 public static void OriginalNested(int mode,bool flag){if(mode==0){Use(0);}else{if(flag){Use(1);Use(2);}else{Use(3);}}}
 public static void RecoveredNested(int mode,bool flag){if(mode==0){Use(0);}else{if(flag){Use(1);Use(2);return;}Use(3);}}
 public static void OriginalTailLoop(bool flag){if(flag){Use(1);}else{for(int i=0;i<3;i++)Use(i);}}
 public static void RecoveredTailLoop(bool flag){if(flag){Use(1);return;}for(int i=0;i<3;i++)Use(i);}
 public static void OriginalThenLocal(bool flag){if(flag){int x=2;Use(x);}else{int y=3;Use(y);}}
 public static void RecoveredThenLocal(bool flag){if(flag){int x=2;Use(x);return;}int y=3;Use(y);}
 public static void NoActions(bool flag){if(flag){return;}Use(2);}
 public static void Nonterminal(int mode,bool flag){if(mode==0){Use(0);}else{if(flag){Use(1);return;}Use(2);}Use(99);}
 public static void LoopAncestor(bool flag){while(flag){if(flag){Use(1);return;}Use(2);}}
 public static void Protected(bool flag){try{if(flag){Use(1);return;}Use(2);}finally{Use(3);}}
 public static void TailEH(bool flag){if(flag){Use(1);return;}using(new Resource()){Use(2);}}
 public static void NestedReturn(bool flag){if(flag){if(Other())return;Use(1);return;}Use(2);}
 public static void Captured(bool flag){if(flag){Work w=delegate{Use(1);};w();return;}Use(2);}
 public static void LambdaBody(bool flag){Work w=delegate{if(flag){Use(1);return;}Use(2);};w();}
 public static int ValueReturn(bool flag){if(flag){Use(1);return 1;}return 2;}
 public static void ExistingElse(bool flag){if(flag){Use(1);return;}else{Use(2);}}
}
'''


def ensure_helper():
 stems=['TransformSyntax','NewRules','AliasRules','SmcsRules','RoslynRules','RoslynCycleRules','SmcsExitRules','SmcsGuardRules','StringConcatRules','LoopConditionRules']
 stems += [name for name in ['SmcsSwitchRules','RoslynDeclarationRules'] if (ROOT/('tools/Round7'+name+'.cs')).is_file()]
 paths=[ROOT/('tools/Round7'+s+'.cs') for s in stems]+[ROOT/'tools/round7_smcs_cycle_rules.cs',ROOT/'tools/Round7TerminalArmRules.cs']
 contents={p.name:p.read_text(encoding='utf-8-sig') for p in paths}
 anchor='AddRound7LoopConditionRules(cycleRequest.RootElement,text,mapped,methodBody,model,point,patches,seen);'
 assert anchor in contents['Round7TransformSyntax.cs']
 if 'AddRound7TerminalArmRules(cycleRequest.RootElement' not in contents['Round7TransformSyntax.cs']:
  contents['Round7TransformSyntax.cs']=contents['Round7TransformSyntax.cs'].replace(anchor,anchor+'\n                AddRound7TerminalArmRules(cycleRequest.RootElement,text,mapped,methodBody,model,point,patches,seen);')
 proof_anchor='mapped=new{start=mapped.SpanStart'
 assert proof_anchor in contents['Round7TransformSyntax.cs']
 contents['Round7TransformSyntax.cs']=contents['Round7TransformSyntax.cs'].replace(proof_anchor,'mapped=new{terminal_arm_ancestry=Round7TerminalArmPath(mapped,methodBody),start=mapped.SpanStart')
 identity=hashlib.sha256((''.join(contents.values())+str(ROSLYN)).encode()).hexdigest()
 folder=FOLDER/'helper'/identity[:16];folder.mkdir(parents=True,exist_ok=True)
 for name,text in contents.items():(folder/name).write_text(text,encoding='utf-8',newline='\n')
 vs=next(p for p in ROSLYN.parents if (p/'dotnet').is_dir());runtime=vs/'dotnet/net10.0/runtime';framework=runtime/'shared/Microsoft.NETCore.App/10.0.10'
 refs=[framework/(n+'.dll') for n in ['System.Private.CoreLib','System.Runtime','System.Console','System.Linq','System.Collections','System.Text.Json','System.Reflection.Metadata','System.Collections.Immutable','System.Memory','netstandard']]
 refs += [ROSLYN.parent/(n+'.dll') for n in ['Microsoft.CodeAnalysis','Microsoft.CodeAnalysis.CSharp']]
 dll=folder/'Round7TransformSyntax.dll'
 if not dll.exists():
  args=['/nologo','/nostdlib+','/target:exe','/out:'+str(dll)]+['/r:'+str(p) for p in refs]+[str(folder/name) for name in contents]
  rsp=folder/'compile.rsp';rsp.write_text('\n'.join('"'+a+'"' for a in args),encoding='utf-8')
  subprocess.run([str(ROSLYN),'/noconfig','@'+str(rsp)],check=True)
  for p in refs[-2:]:shutil.copyfile(p,folder/p.name)
  (folder/'Round7TransformSyntax.runtimeconfig.json').write_text(json.dumps({'runtimeOptions':{'tfm':'net10.0','framework':{'name':'Microsoft.NETCore.App','version':'10.0.10'}}}),encoding='utf-8')
 return [str(runtime/'dotnet.exe'),str(dll)],{str(p.relative_to(ROOT)):sha(p) for p in paths},identity


class Client:
 def __init__(self,manifest):
  command,self.hashes,self.identity=ensure_helper()
  self.process=subprocess.Popen(command+[str(manifest)],stdin=subprocess.PIPE,stdout=subprocess.PIPE,stderr=subprocess.PIPE,text=True,encoding='utf-8')
 def request(self,path,source,point,**hints):
  request={'path':str(path.resolve()),'source':source,'offset':len(source[:point].encode('utf-16-le'))//2,'compiler':'smcs','round7_cycle_rule':RULE,'max_candidates':4096,**hints}
  self.process.stdin.write(json.dumps(request)+'\n');self.process.stdin.flush();line=self.process.stdout.readline()
  if not line:raise RuntimeError(self.process.stderr.read())
  result=json.loads(line)
  if 'error' in result:raise RuntimeError(result)
  self.last_response=result
  return [p for p in result['patches'] if p['rule']==RULE]
 def close(self):self.process.stdin.close();self.process.wait(timeout=10)


def main():
 source=FOLDER/'Fixture.cs';source.write_text(SOURCE,encoding='utf-8',newline='\n')
 base=ROOT/'build/round7/cycles/cycle09/baseline/Assembly-CSharp'
 manifest=json.loads((base/'build-manifest.json').read_text(encoding='utf-8-sig'))
 manifest['sources']=[{'path':str(source),'input_path':str(source)}]
 mp=FOLDER/'manifest.json';mp.write_text(json.dumps(manifest),encoding='utf-8')
 client=Client(mp);checks=[];changed=SOURCE
 try:
  for name,needle in [('Recovered','if(flag)'),('RecoveredNested','if(flag)'),('RecoveredTailLoop','if(flag)'),('RecoveredThenLocal','if(flag)')]:
   start=SOURCE.index('void '+name+'(');point=SOURCE.index(needle,start)
   found=client.request(source,SOURCE,point);assert len(found)==1,(name,len(found))
   patch=found[0];raw=SOURCE.encode('utf-16-le');assert raw[2*patch['start']:2*patch['end']].decode('utf-16-le')==patch['before']
   # Applying by unique text avoids shifting following fixture offsets.
   assert changed.count(patch['before'])==1
   changed=changed.replace(patch['before'],patch['after'],1)
   checks.append({'case':name,'status':'PASS','proposals':1})
  for name in ['NoActions','Nonterminal','LoopAncestor','Protected','TailEH','NestedReturn','Captured','LambdaBody','ValueReturn','ExistingElse']:
   point=SOURCE.index('if(flag)',SOURCE.index(name+'('));found=client.request(source,SOURCE,point)
   assert not found,(name,found);checks.append({'case':'reject-'+name,'status':'PASS'})
  point=SOURCE.index('if(flag)',SOURCE.index('void Recovered('))
  for hints in [{'round7_cycle_rule':''},{'compiler':'roslyn'}]:
   assert not client.request(source,SOURCE,point,**hints)
   checks.append({'case':'opt-in-'+str(hints),'status':'PASS'})
 finally:client.close()
 output=FOLDER/'compile';output.mkdir(exist_ok=True);cs=output/'Fixture.cs';cs.write_text(changed,encoding='utf-8',newline='\n')
 dll=output/'Fixture.dll';args=[f for f in manifest['flags'] if not f.startswith('-out:') and f!='-noconfig']
 args+=['-out:'+str(dll)]+['-r:'+r['path'] for r in manifest['references']]+[str(cs)]
 rsp=output/'compile.rsp';rsp.write_text('\n'.join('"'+a+'"' for a in args),encoding='utf-8')
 env=os.environ.copy();env.update(manifest['scoped_environment'])
 p=subprocess.run([manifest['runtime'],manifest['compiler'],'-noconfig','@'+str(rsp)],cwd=ROOT,env=env,capture_output=True)
 (output/'compiler.log').write_bytes(p.stdout+p.stderr)
 if p.returncode:raise RuntimeError(p.stdout+p.stderr)
 raw=subprocess.run([str(ROOT/'build/tools/ProbeInspector.exe'),str(dll),'--all'],capture_output=True,check=True)
 methods={m['stable_id']:m for m in json.loads(raw.stdout.decode('utf-8-sig'))['selected_methods']}
 for old,new,args in [('Original','Recovered','System.Boolean'),('OriginalNested','RecoveredNested','System.Int32,System.Boolean'),('OriginalTailLoop','RecoveredTailLoop','System.Boolean'),('OriginalThenLocal','RecoveredThenLocal','System.Boolean')]:
  a,b=methods['Fixture::'+old+'('+args+')->System.Void'],methods['Fixture::'+new+'('+args+')->System.Void']
  equal=body_cells(a)==body_cells(b) and a['locals']==b['locals'] and a['exception_regions']==b['exception_regions']
  checks.append({'case':old+'='+new,'status':'PASS' if equal else 'FAIL','complete_body_equal':equal});assert equal,(old,new)
 result={'operation':'isolated-opt-in-terminal-actions-fixtures','candidate_game_compilations':0,'fixture_assembly_compilations':1,'rule':RULE,
         'rule_source_sha256':sha(ROOT/'tools/Round7TerminalArmRules.cs'),'isolated_helper_identity':client.identity,'source_hashes':client.hashes,
         'compiler_sha256':manifest['compiler_sha256'],'checks':checks,'passed':len(checks),'failed':0,
         'fixture_identity_note':'Examples have different method names; equality compares complete canonical body cells, local signature and EH, without changing any game state.'}
 (ROOT/'reports/round7-terminal-arm-fixtures.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8')
 print(json.dumps({k:v for k,v in result.items() if k not in {'checks','source_hashes'}}))


if __name__=='__main__':main()
