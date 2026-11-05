"""Two identical isolated full Roslyn 1.1 builds; observe, never normalize.

The selected main output and its verification receipt are protected by hashes.
MVID location comes from REA Module row 1/#GUID, not a GUID byte search.
"""
import json
from pathlib import Path
import subprocess
import sys
import time
import uuid
from check_kairo_repeatability import volatile_ranges
from compare_kairo import sha
from fetch_roslyn_neighbors import PINNED

ROOT = Path(__file__).resolve().parents[1]


def observe(target, rea_root):
    result = subprocess.run(['node', str(ROOT / 'tools/rea_mvid_coordinate.mjs'), str(rea_root), str(target)],
                            capture_output=True, text=True, encoding='utf-8', check=True)
    parser = json.loads(result.stdout)
    data = target.read_bytes()
    if parser['artifact_sha256'] != sha(target):
        raise ValueError('Output changed during read-only observation')
    info = {'sha256': parser['artifact_sha256'], 'mvid': str(uuid.UUID(bytes_le=bytes.fromhex(parser['mvid_hex_le']))),
            'mvid_offset': parser['mvid_offset'], 'timestamp_offset': parser['timestamp_offset'],
            'timestamp': parser['timestamp'], 'rea_coordinate_proof': parser}
    return data, info


def main():
    compiler = ROOT / 'analysis/tools/roslyn-1.1.0/tools/csc.exe'
    if sha(compiler) != PINNED['1.1.0'][1]:
        raise ValueError('Compiler identity mismatch')
    receipt_path = ROOT / 'build/verification-KairoLibrary.json'
    protected = {str(receipt_path): sha(receipt_path)}
    receipt = json.loads(receipt_path.read_text(encoding='utf-8'))
    protected[str(ROOT / receipt['build_artifact'])] = sha(ROOT / receipt['build_artifact'])
    for name in ('config/methods.csv', 'config/types.csv', 'PROGRESS.md'):
        protected[str(ROOT / name)] = sha(ROOT / name)
    config = json.loads((ROOT / 'config/toolchain.json').read_text(encoding='utf-8'))
    rea_root = Path(config['rea_package_root'])
    label = 'repeatability-1.1.0'
    target = ROOT / ('build/KairoLibrary-' + label + '/KairoLibrary.dll')
    command = [sys.executable, str(ROOT / 'tools/build_kairo.py'), '--compiler', str(compiler), '--label', label]
    subprocess.run(command, cwd=ROOT, check=True, capture_output=True)
    first, a = observe(target, rea_root)
    manifest = json.loads((target.parent / 'build-manifest.json').read_text(encoding='utf-8'))
    if '/unsafe-' not in manifest['flags'] or '/unsafe+' in manifest['flags']:
        raise ValueError('Expected selected unsafe- configuration')
    (target.parent / 'first.dll').write_bytes(first)
    (target.parent / 'first-manifest.json').write_text(json.dumps(manifest, indent=2) + '\n', encoding='utf-8')
    time.sleep(1.1)
    subprocess.run(command, cwd=ROOT, check=True, capture_output=True)
    second, b = observe(target, rea_root)
    other_manifest = json.loads((target.parent / 'build-manifest.json').read_text(encoding='utf-8'))
    for key in ('compiler', 'compiler_sha256', 'flags', 'sources', 'references'):
        if manifest[key] != other_manifest[key]:
            raise ValueError('Build input or output path changed: ' + key)
    if len(first) != len(second):
        raise ValueError('Output length changed')
    ranges = volatile_ranges(first, a)
    other_ranges = volatile_ranges(second, b)
    if ranges != other_ranges:
        raise ValueError('PE field layout changed')
    changed = [index for index, (x, y) in enumerate(zip(first, second)) if x != y]
    covered = {index for field in ranges for index in range(field['offset'], field['offset'] + field['size'])}
    for field in ranges:
        offset, end = field['offset'], field['offset'] + field['size']
        field['changed_bytes'] = sum(index in changed for index in range(offset, end))
        field['first_hex'] = first[offset:end].hex()
        field['second_hex'] = second[offset:end].hex()
    protected_unchanged = all(Path(path).is_file() and sha(Path(path)) == expected for path, expected in protected.items())
    result = {'schema_version': 1, 'scope': 'two identical complete KairoLibrary builds; package1.1.0 executable1.1.0.51109 unsafe- debug:pdbonly deterministic-',
              'command': command, 'isolated_output': str(target.relative_to(ROOT)),
              'compiler_sha256': manifest['compiler_sha256'], 'flags': manifest['flags'],
              'sources': manifest['sources'], 'references': manifest['references'],
              'source_count': len(manifest['sources']), 'first': a, 'second': b,
              'output_length': len(first), 'changed_byte_count': len(changed),
              'changed_offsets': changed, 'identified_ranges': ranges,
              'unexplained_changed_offsets': [index for index in changed if index not in covered],
              'all_identified_fields_observed_changing': all(field['changed_bytes'] > 0 for field in ranges),
              'protected_main_and_state_hashes': protected, 'protected_main_and_state_unchanged': protected_unchanged,
              'comparison_exceptions_applied': False, 'post_compile_binary_edits': False,
              'original_build_determinism': 'UNKNOWN'}
    report = ROOT / 'reports/KairoLibrary-repeatability-1.1.0.json'
    report.write_text(json.dumps(result, indent=2) + '\n', encoding='utf-8')
    print(json.dumps({key: result[key] for key in ('source_count', 'output_length', 'changed_byte_count',
          'identified_ranges', 'unexplained_changed_offsets', 'all_identified_fields_observed_changing',
          'protected_main_and_state_unchanged')}))
    if result['unexplained_changed_offsets'] or not result['all_identified_fields_observed_changing'] or not protected_unchanged:
        raise ValueError('Repeated-build proof incomplete or protected main/state changed')


if __name__ == '__main__':
    main()
