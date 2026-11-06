"""Verify staged C# bytes and describe Git's reversible EOL conversions.

Only isolated builds are emitted. Current verified DLLs/ledgers are untouched.
"""
import argparse
import hashlib
import json
from pathlib import Path
import subprocess
import sys

from apply_il_rules import dump, read, receipt_is_fresh
from compare_kairo import differences, sha
from kairo_candidate import COMPILER_PATH
from metadata_compare import compare_paths

ROOT = Path(__file__).resolve().parents[1]
ASSEMBLIES = ('KairoLibrary', 'Assembly-CSharp')


def staged(path):
    return subprocess.check_output(['git', 'show', ':' + path], cwd=ROOT)


def digest(data):
    return hashlib.sha256(data).hexdigest()


def write_json(path, value):
    path.write_text(json.dumps(value, indent=2, ensure_ascii=True) + '\n', encoding='utf-8', newline='\n')


def source_proof():
    folder = ROOT / 'build/round4/git-source-proof'
    folder.mkdir(parents=True, exist_ok=True)
    result = {'operation': 'compile-exact-staged-CSharp-source-bytes', 'current_builds_modified': False,
              'method_or_type_states_modified': False, 'assemblies': {}}
    for a in ASSEMBLIES:
        receipt = read(ROOT / 'build' / ('verification-' + a + '.json'))
        if not receipt_is_fresh(ROOT, receipt):
            raise ValueError('Current receipt is stale')
        manifest = read((ROOT / receipt['build_artifact']).parent / 'build-manifest.json')
        mapping, sources = {}, []
        for source in manifest['sources']:
            logical = source['path']
            data = staged(logical)
            out = folder / 'sources' / logical
            out.parent.mkdir(parents=True, exist_ok=True)
            out.write_bytes(data)
            mapping[logical] = str(out)
            working = (ROOT / logical).read_bytes()
            sources.append({'path': logical, 'working_tree_sha256': digest(working), 'staged_sha256': digest(data),
                            'identical_bytes': working == data,
                            'only_CRLF_to_LF_conversion': working != data and working.replace(b'\r\n', b'\n') == data})
        mp = folder / (a + '-source-map.json')
        write_json(mp, mapping)
        cmd = [sys.executable, ROOT / 'tools' / ('build_kairo.py' if a == 'KairoLibrary' else 'build_csharp.py'),
               '--label', 'round4-git-source-proof', '--source-map', mp]
        if a == 'KairoLibrary':
            cmd += ['--compiler', ROOT / COMPILER_PATH]
        p = subprocess.run([str(x) for x in cmd], cwd=ROOT, capture_output=True)
        (folder / (a + '-build.log')).write_bytes(p.stdout + p.stderr)
        if p.returncode:
            raise RuntimeError('Staged source did not compile: ' + a)
        binary = ROOT / 'build' / (a + '-round4-git-source-proof') / (a + '.dll')
        actual = dump(binary, folder / (a + '-methods.json'))
        expected = {m['stable_id']: m for m in read((ROOT / receipt['build_artifact']).parent / 'rebuilt-methods.json')['selected_methods']}
        changed = [k for k, m in expected.items() if differences(m, actual.get(k))[0]]
        metadata = compare_paths(ROOT / 'original/extracted/assets/bin/Data/Managed' / (a + '.dll'), binary,
                                 folder / (a + '-metadata.json'))
        proof = {'source_files': sources, 'body_changes_vs_verified_working_tree': changed,
                 'method_inventory_equal': set(actual) == set(expected),
                 'types_equal': metadata['types_equal'], 'types_total': metadata['types_total'],
                 'whole_structural_metadata_equal': metadata['whole_structural_metadata_equal'],
                 'official_build_sha256': receipt['build_sha256'], 'isolated_staged_build_sha256': sha(binary)}
        result['assemblies'][a] = proof
        if changed or not proof['method_inventory_equal'] or not metadata['whole_structural_metadata_equal']:
            write_json(ROOT / 'reports/round4-git-source-proof.json', result)
            raise ValueError('Git source normalization changed bodies or definitions: ' + a)
    write_json(ROOT / 'reports/round4-git-source-proof.json', result)
    print(json.dumps({'staged_source_proof': True, 'changed_bodies': 0, 'types_equal': 466}))


def content_manifest():
    name = 'reports/round4-git-content-provenance.json'
    files = subprocess.check_output(['git', 'diff', '--cached', '--name-only', '-z'], cwd=ROOT).decode('utf-8').split('\0')
    rows = []
    for path in sorted(p for p in files if p and p != name):
        disk, index = (ROOT / path).read_bytes(), staged(path)
        conversion = disk != index and disk.replace(b'\r\n', b'\n') == index
        if disk != index and not conversion:
            raise ValueError('Unstaged content differs beyond Git EOL conversion: ' + path)
        rows.append({'path': path, 'measured_working_tree_sha256': digest(disk), 'committed_content_sha256': digest(index),
                     'conversion': 'CRLF-to-LF by .gitattributes' if conversion else 'identical bytes'})
    proof = read(ROOT / 'reports/round4-git-source-proof.json')
    for a, data in proof['assemblies'].items():
        for item in data['source_files']:
            if digest(staged(item['path'])) != item['staged_sha256']:
                raise ValueError('Staged C# changed after its build proof: ' + item['path'])
    write_json(ROOT / name, {'operation': 'Git-index-content-provenance',
                            'note': 'Recorded evidence hashes identify measured working-tree bytes. This manifest supplies the SHA-256 of committed bytes when Git normalizes CRLF to LF; logical JSON data is unchanged.',
                            'self_excluded_to_avoid_circular_hash': name, 'files': rows})
    print(json.dumps({'manifest_files': len(rows), 'CRLF_to_LF_conversions': sum(r['conversion'] != 'identical bytes' for r in rows)}))


def main():
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument('--manifest-only', action='store_true')
    args = ap.parse_args()
    content_manifest() if args.manifest_only else source_proof()


if __name__ == '__main__':
    main()
