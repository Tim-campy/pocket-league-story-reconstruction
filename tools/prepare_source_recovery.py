"""Build offline ILSpy/Roslyn syntax helper using already installed VS tools."""
import json
from pathlib import Path
import shutil
import subprocess
from compiler_probes import ROSLYN

ROOT=Path(__file__).resolve().parents[1]


def main():
    vs=next(p for p in ROSLYN.parents if (p/'dotnet').is_dir())
    runtime=vs/'dotnet/net10.0/runtime';framework=runtime/'shared/Microsoft.NETCore.App/10.0.10'
    engine=vs/'Common7/IDE/CommonExtensions/Microsoft/VBCSharp/LanguageServices/Core/ICSharpCode.Decompiler.dll'
    folder=ROOT/'build/source-recovery';folder.mkdir(parents=True,exist_ok=True)
    libs=['System.Private.CoreLib','System.Runtime','System.Console','System.Linq','System.Collections',
        'System.Text.Json','System.Reflection.Metadata','System.Collections.Immutable','System.Memory','netstandard']
    references=[framework/(name+'.dll') for name in libs]+[engine,ROSLYN.parent/'Microsoft.CodeAnalysis.dll',ROSLYN.parent/'Microsoft.CodeAnalysis.CSharp.dll']
    output=folder/'SourceRecovery.dll'
    rsp=folder/'compile.rsp'
    flags=['/nologo','/nostdlib+','/target:exe','/out:'+str(output)]+['/r:'+str(p) for p in references]+[str(ROOT/'tools/SourceRecovery.cs')]
    rsp.write_text('\n'.join('"'+s+'"' for s in flags)+'\n',encoding='utf-8')
    subprocess.run([str(ROSLYN),'/noconfig','@'+str(rsp)],check=True)
    for lib in (engine,ROSLYN.parent/'Microsoft.CodeAnalysis.dll',ROSLYN.parent/'Microsoft.CodeAnalysis.CSharp.dll'):
        shutil.copy2(lib,folder/lib.name)
    (folder/'SourceRecovery.runtimeconfig.json').write_text(json.dumps({'runtimeOptions':{'tfm':'net10.0',
        'framework':{'name':'Microsoft.NETCore.App','version':'10.0.10'}}})+'\n')
    print(json.dumps({'driver':str(output.relative_to(ROOT)),'dotnet':str(runtime/'dotnet.exe')}))


if __name__=='__main__': main()
