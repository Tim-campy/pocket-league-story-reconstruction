"""Immutable isolated helper for smcs rule fixtures; never compiles game source."""
import hashlib
import json
from pathlib import Path
import shutil
import subprocess
from compiler_probes import ROSLYN

ROOT = Path(__file__).resolve().parents[1]


def ensure_helper():
    sources = {}
    for stem in ('TransformSyntax', 'NewRules', 'AliasRules'):
        # Historical files are fixed while other agents extend the active helper.
        content = (ROOT / ('tools/Round6' + stem + '.cs')).read_text(encoding='utf-8-sig').replace('Round6', 'Round7')
        if stem == 'TransformSyntax':
            anchor = 'AddRound7CalculatedRules(input,text,mapped,methodBody,model,point,patches,seen);'
            content = content.replace(anchor, anchor + '\n        AddRound7SmcsRules(input,text,mapped,methodBody,model,point,patches,seen);')
        sources['Round7' + stem + '.cs'] = content
    sources['Round7SmcsRules.cs'] = (ROOT / 'tools/Round7SmcsRules.cs').read_text(encoding='utf-8-sig')
    cycles=ROOT / 'tools/round7_smcs_cycle_rules.cs'
    if cycles.is_file():
        sources[cycles.name]=cycles.read_text(encoding='utf-8-sig')
        anchor='AddRound7SmcsRules(input,text,mapped,methodBody,model,point,patches,seen);'
        sources['Round7TransformSyntax.cs']=sources['Round7TransformSyntax.cs'].replace(anchor,anchor+'\n        AddRound7SmcsCycleRules(input,text,mapped,methodBody,model,point,patches,seen);')
    identity = hashlib.sha256((''.join(sources.values()) + str(ROSLYN)).encode()).hexdigest()
    folder = ROOT / 'build/round7-smcs-fixtures/helper' / identity[:16]
    output = folder / 'Round7TransformSyntax.dll'
    vs = next(p for p in ROSLYN.parents if (p / 'dotnet').is_dir())
    runtime = vs / 'dotnet/net10.0/runtime'
    framework = runtime / 'shared/Microsoft.NETCore.App/10.0.10'
    if not output.is_file():
        folder.mkdir(parents=True, exist_ok=True)
        for name, content in sources.items():
            (folder / name).write_text(content, encoding='utf-8', newline='\n')
        libraries = ['System.Private.CoreLib', 'System.Runtime', 'System.Console',
                     'System.Linq', 'System.Collections', 'System.Text.Json',
                     'System.Reflection.Metadata', 'System.Collections.Immutable',
                     'System.Memory', 'netstandard']
        refs = [framework / (name + '.dll') for name in libraries]
        refs += [ROSLYN.parent / (name + '.dll') for name in ['Microsoft.CodeAnalysis', 'Microsoft.CodeAnalysis.CSharp']]
        args = ['/nologo', '/nostdlib+', '/target:exe', '/out:' + str(output)]
        args += ['/r:' + str(path) for path in refs] + [str(folder / name) for name in sources]
        rsp = folder / 'compile.rsp'
        rsp.write_text('\n'.join('"' + arg + '"' for arg in args) + '\n', encoding='utf-8')
        result = subprocess.run([str(ROSLYN), '/noconfig', '@' + str(rsp)], capture_output=True, text=True)
        if result.returncode:
            raise RuntimeError(result.stdout + result.stderr)
        for reference in refs[-2:]:
            shutil.copy2(reference, folder / reference.name)
        (folder / 'Round7TransformSyntax.runtimeconfig.json').write_text(json.dumps({'runtimeOptions': {
            'tfm': 'net10.0', 'framework': {'name': 'Microsoft.NETCore.App', 'version': '10.0.10'}}}) + '\n')
    return [str(runtime / 'dotnet.exe'), str(output)]


class FixtureClient:
    def __init__(self, source, references):
        folder = ROOT / 'build/round7-smcs-fixtures'
        folder.mkdir(parents=True, exist_ok=True)
        self.path = folder / 'Fixture.cs'
        self.path.write_text(source, encoding='utf-8', newline='\n')
        self.source = source
        manifest = folder / 'manifest.json'
        manifest.write_text(json.dumps({'sources': [str(self.path)], 'references': list(map(str, references))}))
        self.process = subprocess.Popen(ensure_helper() + [str(manifest)], stdin=subprocess.PIPE,
                                        stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True, encoding='utf-8')

    def request(self, method, needle, **hints):
        point = self.source.index(needle, self.source.index(method))
        request = dict(path=str(self.path), source=self.source, compiler='smcs',
                       offset=len(self.source[:point].encode('utf-16-le')) // 2, max_candidates=2048, **hints)
        self.process.stdin.write(json.dumps(request) + '\n')
        self.process.stdin.flush()
        line = self.process.stdout.readline()
        if not line:
            raise RuntimeError(self.process.stderr.read())
        result = json.loads(line)
        if result.get('error'):
            raise RuntimeError(result)
        for patch in result['patches']:
            assert self.source.encode('utf-16-le')[patch['start']*2:patch['end']*2].decode('utf-16-le') == patch['before']
        return result['patches']

    def close(self):
        self.process.stdin.close()
        self.process.wait(timeout=10)
