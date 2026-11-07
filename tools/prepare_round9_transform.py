"""Build an immutable round-9 helper from retained rules plus opt-in extensions."""
import hashlib,json,shutil,subprocess
from pathlib import Path
from compiler_probes import ROSLYN
ROOT=Path(__file__).resolve().parents[1]
LEGACY=('Round7TransformSyntax.cs','Round7NewRules.cs','Round7AliasRules.cs','Round7SmcsRules.cs',
 'Round7RoslynRules.cs','round7_smcs_cycle_rules.cs','Round7RoslynCycleRules.cs','Round7SmcsExitRules.cs',
 'Round7SmcsGuardRules.cs','Round7StringConcatRules.cs','Round7LoopConditionRules.cs','Round7SmcsSwitchRules.cs',
 'Round7RoslynDeclarationRules.cs','Round7TerminalArmRules.cs','Round7BalancedBranchRules.cs','Round7InfiniteTailGuardRules.cs')

def source_inputs():
    paths=[ROOT/'tools'/name for name in LEGACY]+[ROOT/'tools/Round9Rules.cs',ROOT/'tools/Round8CompoundRules.cs',ROOT/'tools/Round8NullConditionalPolarityRules.cs',ROOT/'tools/Round9PartBRules.cs',ROOT/'tools/Round9FloatingIdentityRules.cs',ROOT/'tools/Round9PerArmStoreRules.cs',ROOT/'tools/Round9ScopedExitCandidates.cs',ROOT/'tools/Round9ScopedExitAdapter.cs']
    paths += [ROOT/'tools/Round9ArrayPopulationRules.cs']
    anchor='AddRound7InfiniteTailGuardRules(cycleRequest.RootElement,text,mapped,methodBody,model,point,patches,seen);'
    main=paths[0].read_text(encoding='utf-8-sig')
    if main.count(anchor)!=1:raise ValueError('Unrecognized retained dispatcher')
    main=main.replace(anchor,anchor+'\n                AddRound9Rules(cycleRequest.RootElement,text,mapped,methodBody,model,point,patches,seen);')
    return paths,main

def identity():
    paths,main=source_inputs()
    return hashlib.sha256(main.encode()+b''.join(p.read_bytes() for p in paths[1:])+str(ROSLYN).encode()).hexdigest()

def ensure_helper():
    paths,main=source_inputs();digest=identity()
    folder=ROOT/'build/round9-transform'/digest[:16];folder.mkdir(parents=True,exist_ok=True)
    target=folder/'Round9TransformSyntax.dll'
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
