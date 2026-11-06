"""Independent C# AST reader. Only analysis helper is compiled; game sources never Emit."""
import hashlib,json,shutil,subprocess,sys
from pathlib import Path
ROOT=next(p for p in Path(__file__).resolve().parents if (p/'tools/round7_slot_model.py').exists());sys.path.insert(0,str(ROOT/'tools'))
from compiler_probes import ROSLYN
WIP=ROOT/'build/round9/control-ast'
SOURCE=Path(__file__).with_name('Round9ControlAst.cs')
def ensure_helper():
 source=SOURCE;digest=hashlib.sha256(source.read_bytes()).hexdigest();folder=WIP/'helper'/digest[:16];folder.mkdir(parents=True,exist_ok=True);target=folder/'Round9ControlAst.dll';vs=next(p for p in ROSLYN.parents if(p/'dotnet').is_dir());runtime=vs/'dotnet/net10.0/runtime';framework=runtime/'shared/Microsoft.NETCore.App/10.0.10';refs=[framework/(n+'.dll') for n in ('System.Private.CoreLib','System.Runtime','System.Console','System.Linq','System.Collections','System.Text.Json','System.Reflection.Metadata','System.Collections.Immutable','System.Memory','netstandard')];refs += [ROSLYN.parent/(n+'.dll') for n in ('Microsoft.CodeAnalysis','Microsoft.CodeAnalysis.CSharp')]
 if not target.exists():
  args=['/nologo','/nostdlib+','/target:exe','/out:'+str(target)]+['/r:'+str(p) for p in refs]+[str(source)];rsp=folder/'compile.rsp';rsp.write_text('\n'.join('"'+x+'"' for x in args),encoding='utf-8');subprocess.run([str(ROSLYN),'/noconfig','@'+str(rsp)],check=True)
  for ref in refs[-2:]:shutil.copyfile(ref,folder/ref.name)
  target.with_suffix('.runtimeconfig.json').write_text(json.dumps({'runtimeOptions':{'tfm':'net10.0','framework':{'name':'Microsoft.NETCore.App','version':'10.0.10'}}}),encoding='utf-8')
 return [str(runtime/'dotnet.exe'),str(target)]
class Client:
 def __init__(self,manifest):self.process=subprocess.Popen(ensure_helper()+[str(Path(manifest).resolve())],stdin=subprocess.PIPE,stdout=subprocess.PIPE,stderr=subprocess.PIPE,text=True,encoding='utf-8')
 def extract(self,path,source=None,method_id=None):
  req={'path':str(Path(path).resolve())}
  if source is not None:req['source']=source
  if method_id is not None:req['method_id']=method_id
  self.process.stdin.write(json.dumps(req,ensure_ascii=True)+'\n');self.process.stdin.flush();line=self.process.stdout.readline()
  if not line:raise RuntimeError(self.process.stderr.read())
  result=json.loads(line)
  if 'error'in result:raise RuntimeError(result)
  return result['methods']
 def close(self):self.process.stdin.close();self.process.wait(timeout=10)
 def __enter__(self):return self
 def __exit__(self,*args):self.close()
if __name__=='__main__':print(json.dumps(ensure_helper()))
