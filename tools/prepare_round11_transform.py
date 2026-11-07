"""Build an immutable round-9 helper from retained rules plus opt-in extensions."""
import hashlib,json,shutil,subprocess
from pathlib import Path
import sys
sys.path.insert(0,str(next(p for p in Path(__file__).resolve().parents if(p/'AGENTS.md').is_file())/'tools'))
from compiler_probes import ROSLYN
ROOT=next(p for p in Path(__file__).resolve().parents if(p/'AGENTS.md').is_file())
from prepare_round10_transform import source_inputs as retained_inputs

def source_inputs():
    paths,main=retained_inputs()
    anchor='AddRound10RootGuardRules(cycleRequest.RootElement,text,mapped,methodBody,model,point,patches,seen);'
    if main.count(anchor)!=1:raise ValueError('Retained dispatcher changed')
    main=main.replace(anchor,anchor+'\n                AddRound11ComparisonRules(cycleRequest.RootElement,text,mapped,methodBody,model,point,patches,seen);')
    return paths+[Path(__file__).with_name('Round11ComparisonRules.cs')],main


def identity():
    paths,main=source_inputs()
    return hashlib.sha256(main.encode()+b''.join(p.read_bytes() for p in paths[1:])+str(ROSLYN).encode()).hexdigest()

def ensure_helper():
    paths,main=source_inputs();digest=identity()
    folder=ROOT/'build/round11-transform'/digest[:16];folder.mkdir(parents=True,exist_ok=True)
    target=folder/'Round11TransformSyntax.dll'
    vs=next(p for p in ROSLYN.parents if (p/'dotnet').is_dir())
    runtime=vs/'dotnet/net10.0/runtime';framework=runtime/'shared/Microsoft.NETCore.App/10.0.10'
    if not target.exists():
        patched=folder/'Round7TransformSyntax.cs';patched.write_text(main,encoding='utf-8',newline='\n')
        refs=[framework/(n+'.dll') for n in ('System.Private.CoreLib','System.Runtime','System.Console','System.Linq',
          'System.Collections','System.Text.Json','System.Reflection.Metadata','System.Collections.Immutable','System.Memory','netstandard')]
        refs += [ROSLYN.parent/(n+'.dll') for n in ('Microsoft.CodeAnalysis','Microsoft.CodeAnalysis.CSharp')]
        args=['/nologo','/nostdlib+','/target:exe','/out:'+str(target)]+['/r:'+str(p) for p in refs]+[str(p) for p in [patched,*paths[1:]]]
        rsp=folder/'compile.rsp';rsp.write_text('\n'.join('"'+arg+'"' for arg in args)+'\n',encoding='utf-8')
        subprocess.run([str(ROSLYN),'/noconfig','@'+str(rsp)],check=True)
        for reference in refs[-2:]:shutil.copy2(reference,folder/reference.name)
        target.with_suffix('.runtimeconfig.json').write_text(json.dumps({'runtimeOptions':{'tfm':'net10.0','framework':{'name':'Microsoft.NETCore.App','version':'10.0.10'}}})+'\n')
        (folder/'source.sha256').write_text(digest)
    return [str(runtime/'dotnet.exe'),str(target)]

if __name__=='__main__':print(json.dumps({'command':ensure_helper()}))
