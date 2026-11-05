"""Compile an offline Roslyn syntax helper, with installed SDK binaries."""
import json,subprocess
from pathlib import Path
from compiler_probes import ROSLYN
ROOT=Path(__file__).resolve().parents[1]
def main():
    vs=next(p for p in ROSLYN.parents if (p/'dotnet').is_dir())
    runtime=vs/'dotnet/net10.0/runtime'; framework=runtime/'shared/Microsoft.NETCore.App/10.0.10'
    folder=ROOT/'build/source-recovery';folder.mkdir(exist_ok=True)
    refs=[framework/(x+'.dll') for x in ['System.Private.CoreLib','System.Runtime','System.Console','System.Linq','System.Collections','System.Text.Json','System.Reflection.Metadata','System.Collections.Immutable','System.Memory','netstandard']]
    refs += [ROSLYN.parent/(x+'.dll') for x in ['Microsoft.CodeAnalysis','Microsoft.CodeAnalysis.CSharp']]
    out=folder/'IlRuleSyntax.dll';rsp=folder/'il-rules.rsp'
    args=['/nologo','/nostdlib+','/target:exe','/out:'+str(out)]+['/r:'+str(p) for p in refs]+[str(ROOT/'tools/IlRuleSyntax.cs')]
    rsp.write_text('\n'.join('"'+x+'"' for x in args),encoding='utf-8')
    subprocess.run([str(ROSLYN),'/noconfig','@'+str(rsp)],check=True)
    (folder/'IlRuleSyntax.runtimeconfig.json').write_text(json.dumps({'runtimeOptions':{'tfm':'net10.0','framework':{'name':'Microsoft.NETCore.App','version':'10.0.10'}}}))
    print(json.dumps({'dotnet':str(runtime/'dotnet.exe'),'helper':str(out)}))
if __name__=='__main__':main()
