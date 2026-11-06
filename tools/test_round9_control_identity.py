"""Two corlib-identity toy assemblies; no game source Emit or reference mutation."""
import json,os,subprocess,sys,inspect
from pathlib import Path
import round9_control_ast as module
from round9_control_ast import ROOT,WIP
# Deliberately instantiate a separate Default-comparer analysis helper. No public
# helper or production reference is edited for the negative control.
control_folder=WIP/'identity-fixture/default-comparer';control_folder.mkdir(parents=True,exist_ok=True)
control_source=control_folder/'Round9ControlAst.cs'
control_source.write_text(module.SOURCE.read_text(encoding='utf-8').replace(',assemblyIdentityComparer:DesktopAssemblyIdentityComparer.Default',''),encoding='utf-8')
namespace=dict(vars(module));namespace.update(WIP=control_folder,SOURCE=control_source)
exec(inspect.getsource(module.ensure_helper),namespace);exec(inspect.getsource(module.Client),namespace)
Default=namespace['Client'];Desktop=module.Client
from compare_kairo import sha
FOLDER=WIP/'identity-fixture/fixture'
def compile_toy(base,source,target,references):
 args=[x for x in base['flags']if not x.startswith('-out:')and x!='-noconfig']+['-nostdlib','-out:'+str(target)]+['-r:'+str(r)for r in references]+[str(source)];rsp=target.with_suffix('.rsp');rsp.write_text('\n'.join('"'+x+'"'for x in args),encoding='utf-8');env=os.environ.copy();env.update(base['scoped_environment']);proc=subprocess.run([base['runtime'],base['compiler'],'-noconfig','@'+str(rsp)],cwd=ROOT,env=env,capture_output=True);target.with_suffix('.log').write_bytes(proc.stdout+proc.stderr)
 if proc.returncode:raise RuntimeError(proc.stdout+proc.stderr)
def main():
 FOLDER.mkdir(parents=True,exist_ok=True);base=json.loads((ROOT/'build/round9/cycles/a05/after/Assembly-CSharp/build-manifest.json').read_text(encoding='utf-8'));kairo=json.loads((ROOT/'build/KairoLibrary-roslyn-1.1.0/build-manifest.json').read_text(encoding='utf-8'));microsoft=Path(next(x['path']for x in kairo['references']if Path(x['path']).name=='mscorlib.dll'));unity=Path(next(x['path']for x in base['references']if Path(x['path']).name=='mscorlib.dll'));producer=FOLDER/'Producer.cs';producer.write_text('public class Producer {public object Echo(object x){return x;}}',encoding='utf-8');producer_dll=FOLDER/'Producer.dll';producer_command=[kairo['compiler'],'/noconfig','/nologo','/nostdlib+','/target:library','/optimize+','/r:'+str(microsoft),'/out:'+str(producer_dll),str(producer)];prod=subprocess.run(producer_command,cwd=ROOT,capture_output=True);producer_dll.with_suffix('.log').write_bytes(prod.stdout+prod.stderr);assert prod.returncode==0,(prod.stdout+prod.stderr);consumer=FOLDER/'Consumer.cs';text='class Consumer {public static bool Test(Producer producer,object value){if(producer.Echo(value)!=null)return true;return false;}}';consumer.write_text(text,encoding='utf-8');references=[unity,producer_dll];mp=FOLDER/'manifest.json';mp.write_text(json.dumps(dict(base,sources=[{'path':str(consumer),'input_path':str(consumer)}],references=[{'path':str(p),'sha256':sha(p)}for p in references])),encoding='utf-8');rows={};key='Consumer::Test(Producer,System.Object)->System.Boolean'
 for label,client in [('default',Default),('desktop',Desktop)]:
  with client(mp)as c:ir=c.extract(consumer,text,key)[0]
  rows[label]={'complete':ir['complete'],'reasons':ir['reasons'],'plan':ir['emission_plan']}
 print({k:{'complete':v['complete'],'reasons':v['reasons']}for k,v in rows.items()});assert not rows['default']['complete']and any('CS0012'in r.get('detail','')for r in rows['default']['reasons']);assert rows['desktop']['complete']
 consumer_dll=FOLDER/'Consumer.dll';compile_toy(base,consumer,consumer_dll,references)
 def inspect(binary):return json.loads(subprocess.check_output([str(ROOT/'build/tools/ProbeInspector.exe'),str(binary),'--all']).decode('utf-8-sig'))['selected_methods']
 pmethods=inspect(producer_dll);cmethods=inspect(consumer_dll);pm=next(m for m in pmethods if '::Echo('in m['stable_id']);cm=next(m for m in cmethods if '::Test('in m['stable_id']);result={'operation':'analysis-only-desktop-corlib-unification-fixture','game_compilations':0,'fixture_assembly_compilations':2,'producer_compiler_sha256':kairo['compiler_sha256'],'initial_unity_smcs_producer_retargeted_to_Unity_not_a_valid_identity_fixture':True,'production_reference_paths_unchanged':True,'old_CS0012_not_ignored':True,'default_vs_desktop':rows,'microsoft_reference':{'path':str(microsoft),'sha256':sha(microsoft)},'unity_reference':{'path':str(unity),'sha256':sha(unity)},'producer_method_signature':pm.get('signature'),'consumer_method_signature':cm.get('signature'),'consumer_call':next(i for i in cm['instructions']if i['opcode']in ('call','callvirt')),'smcs_accepts_same_consumer_source':True};(WIP/'identity-fixture-result.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in result.items()if k not in {'default_vs_desktop','producer_method_signature','consumer_method_signature'}},indent=2))
if __name__=='__main__':main()
