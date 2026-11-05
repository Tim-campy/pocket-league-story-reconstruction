"""Build the independent read-only semantic inventory helper from installed tools."""
import hashlib
import json
from pathlib import Path
import shutil
import subprocess
from compiler_probes import ROSLYN

ROOT = Path(__file__).resolve().parents[1]


def main():
    vs = next(p for p in ROSLYN.parents if (p/'dotnet').is_dir())
    runtime = vs/'dotnet/net10.0/runtime'
    framework = runtime/'shared/Microsoft.NETCore.App/10.0.10'
    folder = ROOT/'build/order-syntax'
    folder.mkdir(parents=True, exist_ok=True)
    libraries = ['System.Private.CoreLib', 'System.Runtime', 'System.Console', 'System.Linq', 'System.Collections',
                 'System.Text.Json', 'System.Reflection.Metadata', 'System.Collections.Immutable', 'System.Memory', 'netstandard']
    references = [framework/(name+'.dll') for name in libraries]
    roslyn_libraries = [ROSLYN.parent/'Microsoft.CodeAnalysis.dll', ROSLYN.parent/'Microsoft.CodeAnalysis.CSharp.dll']
    references.extend(roslyn_libraries)
    output = folder/'OrderSyntax.dll'
    options = ['/nologo', '/nostdlib+', '/target:exe', '/out:'+str(output)]
    options += ['/r:'+str(p) for p in references]+[str(ROOT/'tools/OrderSyntax.cs')]
    rsp = folder/'compile.rsp'
    rsp.write_text('\n'.join('"'+s+'"' for s in options)+'\n', encoding='utf-8')
    subprocess.run([str(ROSLYN), '/noconfig', '@'+str(rsp)], check=True)
    for library in roslyn_libraries:
        shutil.copy2(library, folder/library.name)
    (folder/'OrderSyntax.runtimeconfig.json').write_text(json.dumps({'runtimeOptions': {'tfm':'net10.0',
        'framework': {'name':'Microsoft.NETCore.App','version':'10.0.10'}}})+'\n')
    print(json.dumps({'driver': str(output.relative_to(ROOT)), 'dotnet': str(runtime/'dotnet.exe'),
        'source_sha256': hashlib.sha256((ROOT/'tools/OrderSyntax.cs').read_bytes()).hexdigest()}))


if __name__ == '__main__':
    main()
