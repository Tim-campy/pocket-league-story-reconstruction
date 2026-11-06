"""Build the round-7 persistent syntax helper with already installed Roslyn."""
import hashlib
import json
from pathlib import Path
import shutil
import subprocess
from compiler_probes import ROSLYN

ROOT = Path(__file__).resolve().parents[1]


def ensure_helper():
    vs = next(p for p in ROSLYN.parents if (p / 'dotnet').is_dir())
    runtime = vs / 'dotnet/net10.0/runtime'
    framework = runtime / 'shared/Microsoft.NETCore.App/10.0.10'
    sources = [ROOT / ('tools/Round7' + stem + '.cs') for stem in
               ('TransformSyntax', 'NewRules', 'AliasRules', 'SmcsRules', 'RoslynRules')]
    sources += [ROOT/'tools/round7_smcs_cycle_rules.cs',ROOT/'tools/Round7RoslynCycleRules.cs']
    identity = hashlib.sha256(b''.join(p.read_bytes() for p in sources) + str(ROSLYN).encode()).hexdigest()
    # Immutable hash folders let an older engine process keep its helper loaded.
    folder = ROOT / 'build/round7-transform' / identity[:16]
    folder.mkdir(parents=True, exist_ok=True)
    output = folder / 'Round7TransformSyntax.dll'
    libraries = ['System.Private.CoreLib', 'System.Runtime', 'System.Console',
                 'System.Linq', 'System.Collections', 'System.Text.Json',
                 'System.Reflection.Metadata', 'System.Collections.Immutable',
                 'System.Memory', 'netstandard']
    refs = [framework / (name + '.dll') for name in libraries]
    refs += [ROSLYN.parent / (name + '.dll') for name in
             ['Microsoft.CodeAnalysis', 'Microsoft.CodeAnalysis.CSharp']]
    stamp = folder / 'source.sha256'
    if not output.exists() or not stamp.exists() or stamp.read_text() != identity:
        args = ['/nologo', '/nostdlib+', '/target:exe', '/out:' + str(output)]
        args += ['/r:' + str(path) for path in refs] + [str(source) for source in sources]
        rsp = folder / 'compile.rsp'
        rsp.write_text('\n'.join('"' + arg + '"' for arg in args) + '\n', encoding='utf-8')
        subprocess.run([str(ROSLYN), '/noconfig', '@' + str(rsp)], check=True)
        for reference in refs[-2:]:
            shutil.copy2(reference, folder / reference.name)
        (folder / 'Round7TransformSyntax.runtimeconfig.json').write_text(json.dumps({
            'runtimeOptions': {'tfm': 'net10.0', 'framework': {
                'name': 'Microsoft.NETCore.App', 'version': '10.0.10'}}}) + '\n')
        stamp.write_text(identity)
    return [str(runtime / 'dotnet.exe'), str(output)]


if __name__ == '__main__':
    print(json.dumps({'command': ensure_helper()}))
