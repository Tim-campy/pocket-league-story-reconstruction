"""Compare unsafe+/- on an owned safe fixture, never compiling target sources."""
import hashlib
import json
import os
from pathlib import Path
import subprocess
from compiler_probes import ROOT
from metadata_compare import inspect_metadata, compare_metadata


def read(path):
    return json.loads(path.read_text(encoding='utf-8-sig'))


def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def main():
    root = Path(__file__).resolve().parents[1]
    folder = root / 'build/unsafe-metadata-fixtures'
    folder.mkdir(exist_ok=True)
    source = folder / 'SafeFixture.cs'
    source.write_text('public class SafeFixture { public static int Add(int a,int b) { return a+b; } }\n', encoding='utf-8')
    compilers = [('KairoLibrary', 'roslyn-1.3.2'), ('Assembly-CSharp', 'mono-smcs')]
    results = []
    for assembly, label in compilers:
        manifest = read(root / 'build' / (assembly + '-' + label) / 'build-manifest.json')
        variants = {}
        for setting in ('+', '-'):
            dest = folder / label / ('plus' if setting == '+' else 'minus')
            dest.mkdir(parents=True, exist_ok=True)
            output = dest / 'SafeFixture.dll'
            style = '/' if assembly == 'KairoLibrary' else '-'
            flags = [x for x in manifest['flags'] if not x.startswith(style + 'out:')]
            flags = [style + 'unsafe' + setting if x.startswith(style + 'unsafe') else x for x in flags]
            flags.append(style + 'out:' + str(output))
            flags = [x for x in flags if x != style + 'noconfig']
            flags += [style + ('reference:' if style == '/' else 'r:') + r['path'] for r in manifest['references']]
            flags += [str(source)]
            rsp = dest / 'compile.rsp'
            rsp.write_text('\n'.join('"' + x + '"' for x in flags), encoding='utf-8')
            env = os.environ.copy()
            env.update(manifest.get('scoped_environment', {}))
            command = ([manifest['runtime']] if 'runtime' in manifest else []) + [manifest['compiler'], style + 'noconfig', '@' + str(rsp)]
            p = subprocess.run(command, cwd=root, env=env, capture_output=True)
            (dest / 'compiler.log').write_bytes(p.stdout + p.stderr)
            if p.returncode:
                raise ValueError('Owned fixture compile failed: ' + str(dest))
            metadata = inspect_metadata(output)
            variants[setting] = {'metadata': metadata, 'output_sha256': sha(output), 'flags': flags,
                'compiler_sha256': manifest['compiler_sha256'], 'source_sha256': sha(source),
                'output': output.relative_to(root).as_posix()}
        report = compare_metadata(variants['-']['metadata'], variants['+']['metadata'])
        results.append({'assembly_candidate': assembly, 'compiler_candidate': label,
            'compiler_sha256': manifest['compiler_sha256'],
            'source_sha256': sha(source), 'fixture_contains_unsafe_syntax': False,
            'structural_difference': report['global_differences'],
            'type_metadata_equal': report['types_equal'] == report['types_total'],
            'variants': {k: {n: v for n, v in x.items() if n != 'metadata'} for k, x in variants.items()},
            'unsafe_minus_globals': {k: variants['-']['metadata'][k] for k in ('module_attributes', 'assembly_attributes', 'assembly_security')},
            'unsafe_plus_globals': {k: variants['+']['metadata'][k] for k in ('module_attributes', 'assembly_attributes', 'assembly_security')}})
    old = []
    hashes = {}
    for assembly, _ in compilers:
        hashes[assembly] = sha(root / 'original/extracted/assets/bin/Data/Managed' / (assembly + '.dll'))
    for cache in (root / 'build/metadata-cache').glob('*.json'):
        document = read(cache)
        name = document.get('assembly', '').split(',')[0]
        if name not in hashes:
            continue
        old.append({'assembly': name, 'artifact_sha256': document['artifact_sha256'],
            'is_original': document['artifact_sha256'] == hashes[name],
            'evidence_cache': cache.relative_to(root).as_posix(),
            **{k: document.get(k, 'NOT_RECORDED') for k in ('module_attributes', 'assembly_security')}})
    report = {'scope': 'Owned safe fixture compiled four times; existing original/target metadata caches read only',
        'main_sources_modified': False, 'main_build_flags_modified': False, 'targets_compiled': False,
        'results': results, 'previous_metadata_cache_observations': old}
    (root / 'reports/unsafe-metadata-fixtures.json').write_text(json.dumps(report, indent=2) + '\n', encoding='utf-8')
    print(json.dumps({'results': [{k: r[k] for k in ('assembly_candidate', 'structural_difference', 'type_metadata_equal')} for r in results], 'old_cache_records': len(old)}))


if __name__ == '__main__':
    main()
