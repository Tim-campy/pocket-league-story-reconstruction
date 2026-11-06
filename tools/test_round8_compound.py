"""Independent smcs fixture for opt-in stable-field compound multiplication expansion."""
import json,sys,os,subprocess,shutil,hashlib
from pathlib import Path
ROOT=next(p for p in Path(__file__).resolve().parents if (p/'tools/round7_slot_model.py').exists());sys.path.insert(0,str(ROOT/'tools'))
from prepare_round8_transform import source_inputs
from compiler_probes import ROSLYN
from compare_kairo import body_cells,sha
WIP=ROOT/'build/round8/smcs-slot-anchors/compound'
RULE=Path(__file__).with_name('Round8CompoundRules.cs')
SOURCE='''class Fixture {
 class Vector{public float x;public volatile float volatileX;public double d;public float Prop{get{return x;}set{x=value;}}}
 class Node{public Vector vector=new Vector();}static Vector v=new Vector();static Node node=new Node();static volatile Vector volatileV;static Vector P{get{return v;}}static Vector Get(){return v;}static Vector[] vectors=new[]{new Vector()};
 public static void Original(float rate){v.x = v.x * rate;}
 public static void Recovered(float rate){v.x *= rate;}
 public static void OriginalChain(float a,float b){node.vector.x = node.vector.x * (a+b);}
 public static void RecoveredChain(float a,float b){node.vector.x *= a+b;}
 public static void OriginalDouble(double rate){v.d = v.d * rate;}
 public static void RecoveredDouble(double rate){v.d *= rate;}
 public static void OriginalLocal(float rate){Vector local=v;local.x = local.x * rate;}
 public static void RecoveredLocal(float rate){Vector local=v;local.x *= rate;}
 public static void RejectPropertyReceiver(float rate){P.x *= rate;}
 public static void RejectCallReceiver(float rate){Get().x *= rate;}
 public static void RejectIndexerReceiver(float rate){vectors[0].x *= rate;}
 public static void RejectVolatileReceiver(float rate){volatileV.x *= rate;}
 public static void RejectVolatileField(float rate){v.volatileX *= rate;}
 public static void RejectPropertyTarget(float rate){v.Prop *= rate;}
 public static void RejectConversion(int rate){v.x *= rate;}
}'''
def helper(manifest):
 paths,main=source_inputs();anchor='AddRound8Rules(cycleRequest.RootElement,text,mapped,methodBody,model,point,patches,seen);';assert main.count(anchor)==1;main=main.replace(anchor,anchor+'\nAddRound8CompoundRules(cycleRequest.RootElement,text,mapped,methodBody,model,point,patches,seen);');folder=WIP/'helper';folder.mkdir(exist_ok=True);source=folder/'Round7TransformSyntax.cs';source.write_text(main,encoding='utf-8',newline='\n');binary=folder/'FixtureTransform.dll';vs=next(p for p in ROSLYN.parents if (p/'dotnet').is_dir());runtime=vs/'dotnet/net10.0/runtime';framework=runtime/'shared/Microsoft.NETCore.App/10.0.10';refs=[framework/(n+'.dll') for n in ('System.Private.CoreLib','System.Runtime','System.Console','System.Linq','System.Collections','System.Text.Json','System.Reflection.Metadata','System.Collections.Immutable','System.Memory','netstandard')];refs += [ROSLYN.parent/(n+'.dll') for n in ['Microsoft.CodeAnalysis','Microsoft.CodeAnalysis.CSharp']];args=['/nologo','/nostdlib+','/target:exe','/out:'+str(binary)]+['/r:'+str(p) for p in refs]+[str(p) for p in [source,*paths[1:],RULE]];rsp=folder/'compile.rsp';rsp.write_text('\n'.join('"'+a+'"' for a in args),encoding='utf-8');subprocess.run([str(ROSLYN),'/noconfig','@'+str(rsp)],check=True)
 for ref in refs[-2:]:shutil.copyfile(ref,folder/ref.name)
 binary.with_suffix('.runtimeconfig.json').write_text(json.dumps({'runtimeOptions':{'tfm':'net10.0','framework':{'name':'Microsoft.NETCore.App','version':'10.0.10'}}}),encoding='utf-8')
 return subprocess.Popen([str(runtime/'dotnet.exe'),str(binary),str(manifest)],stdin=subprocess.PIPE,stdout=subprocess.PIPE,stderr=subprocess.PIPE,text=True,encoding='utf-8')
def request(process,path,source,method,flag=True,compiler='smcs'):
 at=source.index(method+'(');start=source.index('{',at);point=source.index('*=',start);offset=len(source[:point].encode('utf-16-le'))//2
 process.stdin.write(json.dumps({'path':str(path),'source':source,'offset':offset,'compiler':compiler,'round7_cycle_rule':'expand-compound-field-multiply','round8_expand_compound_fields':flag,'max_candidates':2048})+'\n');process.stdin.flush();line=process.stdout.readline()
 if not line:raise RuntimeError(process.stderr.read())
 result=json.loads(line)
 if 'error' in result:raise RuntimeError(result)
 return [p for p in result['patches'] if p['rule']=='expand-compound-field-multiply']
def edit(source,patch):
 raw=source.encode('utf-16-le');return (raw[:patch['start']*2]+patch['after'].encode('utf-16-le')+raw[patch['end']*2:]).decode('utf-16-le')
def main():
 folder=WIP/'fixture';folder.mkdir(exist_ok=True);source=folder/'Fixture.cs';source.write_text(SOURCE,encoding='utf-8',newline='\n');base=json.loads((ROOT/'build/round8/baseline/Assembly-CSharp/build-manifest.json').read_text(encoding='utf-8'));mp=folder/'manifest.json';mp.write_text(json.dumps(dict(base,sources=[{'path':str(source),'input_path':str(source)}])),encoding='utf-8');process=helper(mp);changed=SOURCE;results=[]
 try:
  for name in ['Recovered','RecoveredChain','RecoveredDouble','RecoveredLocal']:
   candidates=request(process,source,changed,name);assert len(candidates)==1,(name,candidates);patch=candidates[0];changed=edit(changed,patch);results.append({'method':name,'before':patch['before'],'after':patch['after']})
  negatives=['RejectPropertyReceiver','RejectCallReceiver','RejectIndexerReceiver','RejectVolatileReceiver','RejectVolatileField','RejectPropertyTarget','RejectConversion'];rejected={name:not request(process,source,changed,name) for name in negatives};assert all(rejected.values()),rejected
  assert not request(process,source,SOURCE,'Recovered',flag=False)
  assert not request(process,source,SOURCE,'Recovered',compiler='roslyn-1.1.0')
 finally:process.stdin.close();process.wait(timeout=10)
 source.write_text(changed,encoding='utf-8',newline='\n');binary=folder/'Fixture.dll';args=[f for f in base['flags'] if not f.startswith('-out:') and f!='-noconfig']+['-out:'+str(binary)]+['-r:'+r['path'] for r in base['references']]+[str(source)];rsp=folder/'compile.rsp';rsp.write_text('\n'.join('"'+a+'"' for a in args),encoding='utf-8');env=os.environ.copy();env.update(base['scoped_environment']);p=subprocess.run([base['runtime'],base['compiler'],'-noconfig','@'+str(rsp)],env=env,cwd=ROOT,capture_output=True);(folder/'compiler.log').write_bytes(p.stdout+p.stderr)
 if p.returncode:raise RuntimeError(p.stdout+p.stderr)
 raw=subprocess.check_output([str(ROOT/'build/tools/ProbeInspector.exe'),str(binary),'--all']);methods={m['stable_id']:m for m in json.loads(raw.decode('utf-8-sig'))['selected_methods']};exact={}
 for suffix,args in [('', 'System.Single'),('Chain','System.Single,System.Single'),('Double','System.Double'),('Local','System.Single')]:
  a=methods['Fixture::Original'+suffix+'('+args+')->System.Void'];b=methods['Fixture::Recovered'+suffix+'('+args+')->System.Void'];equal=body_cells(a)==body_cells(b) and a['locals']==b['locals'] and a['exception_regions']==b['exception_regions'];assert equal,suffix;exact[suffix or 'Field']={'body_exact':equal,'il_bytes':a['header']['code_size'],'local_count':len(a['locals'])}
 result={'candidate_game_compilations':0,'fixture_assembly_compilations':1,'exact_cases':exact,'negative_cases':rejected,'opt_in_disabled_rejected':True,'roslyn_rejected':True,'proposals':results,'rule_sha256':sha(RULE),'compiler_sha256':base['compiler_sha256'],'fixture_source_sha256':sha(source)};(WIP/'fixture-result.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8');print(json.dumps(result))
if __name__=='__main__':main()
