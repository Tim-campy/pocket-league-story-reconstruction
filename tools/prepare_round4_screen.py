"""Generate hash-bound C# body skeletons for optional search screening only.

The caller must prove the target body matches a complete build before using a
screen, then fully build/verify every advancing candidate before accepting it.
"""
import hashlib
import json
from pathlib import Path
import shutil
import subprocess

from compiler_probes import ROSLYN

ROOT = Path(__file__).resolve().parents[1]


def digest(path):
    return hashlib.sha256(Path(path).read_bytes()).hexdigest()


def prepare_helper():
    source = ROOT / 'tools/Round4ScreenSyntax.cs'
    identity = hashlib.sha256(source.read_bytes() + str(ROSLYN).encode()).hexdigest()
    folder = ROOT / 'build/round4-screen-tool' / identity[:16]
    folder.mkdir(parents=True, exist_ok=True)
    output = folder / 'Round4ScreenSyntax.dll'
    vs = next(path for path in ROSLYN.parents if (path / 'dotnet').is_dir())
    runtime = vs / 'dotnet/net10.0/runtime'
    framework = runtime / 'shared/Microsoft.NETCore.App/10.0.10'
    if not output.exists():
        libs = ['System.Private.CoreLib', 'System.Runtime', 'System.Console',
                'System.Linq', 'System.Collections', 'System.Text.Json',
                'System.Reflection.Metadata', 'System.Collections.Immutable',
                'System.Memory', 'System.Text.Encoding.Extensions', 'netstandard']
        refs = [framework / (name + '.dll') for name in libs]
        refs += [ROSLYN.parent / (name + '.dll') for name in
                 ['Microsoft.CodeAnalysis', 'Microsoft.CodeAnalysis.CSharp']]
        args = ['/nologo', '/nostdlib+', '/target:exe', '/out:' + str(output)]
        args += ['/r:' + str(ref) for ref in refs] + [str(source)]
        rsp = folder / 'compile.rsp'
        rsp.write_text('\n'.join('"' + arg + '"' for arg in args) + '\n', encoding='utf-8')
        subprocess.run([str(ROSLYN), '/noconfig', '@' + str(rsp)], check=True)
        for ref in refs[-2:]:
            shutil.copy2(ref, folder / ref.name)
        (folder / 'Round4ScreenSyntax.runtimeconfig.json').write_text(json.dumps({
            'runtimeOptions': {'tfm': 'net10.0', 'framework': {
                'name': 'Microsoft.NETCore.App', 'version': '10.0.10'}}}) + '\n')
    return [str(runtime / 'dotnet.exe'), str(output)]


def prepare_skeletons(manifest_path, folder):
    """Return logical source path -> skeleton file, without changing real sources."""
    manifest_path = Path(manifest_path).resolve()
    folder = Path(folder).resolve()
    manifest = json.loads(manifest_path.read_text(encoding='utf-8-sig'))
    paths = []
    for item in manifest['sources']:
        logical = item if isinstance(item, str) else item['path']
        candidate = Path(logical)
        if candidate.is_absolute() or '..' in candidate.parts:
            raise ValueError('Skeletons require project-relative logical paths: ' + logical)
        source = Path(item.get('input_path', logical) if isinstance(item, dict) else logical)
        if not source.is_absolute():
            source = ROOT / source
        paths.append({'path': logical, 'input_path': str(source.resolve()), 'sha256': digest(source)})
    identity = {'screener_sha256': digest(ROOT / 'tools/Round4ScreenSyntax.cs'), 'sources': paths}
    stamp = folder / 'screen-manifest.json'
    outputs = {item['path']: str(folder / item['path']) for item in paths}
    if stamp.exists():
        previous = json.loads(stamp.read_text(encoding='utf-8'))
        if previous.get('identity') == identity and all(Path(path).exists() for path in outputs.values()):
            return outputs
    folder.mkdir(parents=True, exist_ok=True)
    resolved_manifest = folder / 'screen-input.json'
    resolved_manifest.write_text(json.dumps({'sources': paths}) + '\n', encoding='utf-8')
    result = subprocess.run(prepare_helper() + [str(resolved_manifest), str(folder)],
                            cwd=ROOT, capture_output=True, text=True, encoding='utf-8', check=True)
    stamp.write_text(json.dumps({'identity': identity, 'result': json.loads(result.stdout),
                                'screen_only': True, 'outputs': outputs}, indent=2) + '\n', encoding='utf-8')
    return outputs


if __name__ == '__main__':
    import argparse
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('manifest', type=Path)
    parser.add_argument('folder', type=Path)
    args = parser.parse_args()
    outputs = prepare_skeletons(args.manifest, args.folder)
    print(json.dumps({'sources': outputs, 'count': len(outputs), 'screening_only': True}))
