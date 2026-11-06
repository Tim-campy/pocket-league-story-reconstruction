"""Compile the frozen round-4 Kairo C# against three reference combinations.

Read-only w.r.t. originals, selected build, sources and ledgers. Raw table
differences are measured, never repaired or used as body acceptance criteria.
"""
import json
from pathlib import Path
import subprocess

from compare_kairo import differences, sha
from metadata_compare import compare_paths
from measure_raw_assemblies import inspect_layout, measure_pair

ROOT = Path(__file__).resolve().parents[1]
SNAP = ROOT / 'build/round4/baseline/KairoLibrary'
OUT = ROOT / 'build/round4/reference-probes'
REPORT = ROOT / 'reports/round4-reference-probes.json'


def read(path):
    return json.loads(Path(path).read_text(encoding='utf-8-sig'))


def dump(binary, output):
    with output.open('wb') as stream:
        subprocess.run([str(ROOT / 'build/tools/ProbeInspector.exe'), str(binary), '--structure'], stdout=stream, check=True)
    return {m['stable_id']: m for m in read(output)['selected_methods']}


def decode_blob(data, at):
    first = data[at]
    if first & 0x80 == 0:
        size, width = first, 1
    elif first & 0xc0 == 0x80:
        size, width = ((first & 0x3f) << 8) | data[at + 1], 2
    else:
        size, width = ((first & 0x1f) << 24) | (data[at + 1] << 16) | (data[at + 2] << 8) | data[at + 3], 4
    return data[at + width:at + width + size].hex()


def assembly_ref_columns(original, rebuilt, layouts):
    """Decode ECMA II.22.5 fields, keeping raw heap indexes and all byte deltas."""
    files = (Path(original).read_bytes(), Path(rebuilt).read_bytes())
    parsed = []
    for data, layout in zip(files, layouts):
        table = next(t for t in layout['tables'] if t['name'] == 'AssemblyRef')
        widths = layout['index_widths']
        schema = [('MajorVersion', 2), ('MinorVersion', 2), ('BuildNumber', 2), ('RevisionNumber', 2), ('Flags', 4),
                  ('PublicKeyOrToken', widths['blob']), ('Name', widths['strings']),
                  ('Culture', widths['strings']), ('HashValue', widths['blob'])]
        if sum(width for _, width in schema) != table['row_size']:
            raise ValueError('AssemblyRef schema width mismatch')
        streams = {s['name']: s for s in layout['streams']}
        rows = []
        for ordinal in range(table['row_count']):
            cursor = table['offset'] + ordinal * table['row_size']
            row = {}
            for name, width in schema:
                value = int.from_bytes(data[cursor:cursor + width], 'little')
                resolved = value
                if name in ('Name', 'Culture'):
                    at = streams['#Strings']['offset'] + value
                    resolved = data[at:data.index(b'\x00', at)].decode('utf-8')
                elif name in ('PublicKeyOrToken', 'HashValue'):
                    resolved = decode_blob(data, streams['#Blob']['offset'] + value)
                row[name] = {'value': value, 'resolved': resolved, 'file_offset': cursor, 'raw_hex': data[cursor:cursor + width].hex()}
                cursor += width
            rows.append(row)
        parsed.append(rows)
    deltas = []
    for ordinal, (left, right) in enumerate(zip(*parsed), 1):
        for name in left:
            if left[name]['raw_hex'] == right[name]['raw_hex']:
                continue
            l, r = left[name], right[name]
            deltas.append({'row': ordinal, 'assembly_name_original': left['Name']['resolved'],
                           'assembly_name_rebuilt': right['Name']['resolved'], 'column': name,
                           'original': l, 'rebuilt': r, 'resolved_equal': l['resolved'] == r['resolved'],
                           'different_column_byte_offsets': [i for i, (a, b) in enumerate(zip(bytes.fromhex(l['raw_hex']), bytes.fromhex(r['raw_hex']))) if a != b]})
    return {'original_rows': len(parsed[0]), 'rebuilt_rows': len(parsed[1]), 'column_differences': deltas,
            'different_bytes': sum(len(d['different_column_byte_offsets']) for d in deltas)}


def main():
    OUT.mkdir(parents=True, exist_ok=True)
    manifest = read(SNAP / 'build-manifest.json')
    sources = read(SNAP / 'source-map.json')
    original_binary = ROOT / 'original/extracted/assets/bin/Data/Managed/KairoLibrary.dll'
    original = {m['stable_id']: m for m in read(SNAP / 'original-methods.json')['selected_methods']}
    baseline = {m['stable_id']: m for m in read(SNAP / 'rebuilt-methods.json')['selected_methods']}
    baseline_equal = {k for k, m in original.items() if 'raw_body_hex' in m and not differences(m, baseline.get(k))[0]}
    microsoft = {'System': Path('C:/Windows/Microsoft.NET/Framework/v2.0.50727/System.dll'),
                 'System.Core': Path('C:/Program Files (x86)/Reference Assemblies/Microsoft/Framework/v3.5/System.Core.dll')}
    for path in microsoft.values():
        if not path.is_file():
            raise FileNotFoundError(path)
    result = {'schema_version': 1, 'scope': 'frozen commit 1047b1f sources; isolated reference probes; no source or state edits',
              'baseline_manifest_sha256': sha(SNAP / 'build-manifest.json'),
              'baseline_body_equal': len(baseline_equal), 'compiler': manifest['compiler'],
              'compiler_sha256': manifest['compiler_sha256'], 'source_map_sha256': sha(SNAP / 'source-map.json'),
              'microsoft_references': {name: {'path': str(path), 'sha256': sha(path)} for name, path in microsoft.items()}, 'variants': {}}
    orig_layout = inspect_layout(original_binary)
    for label, names in (('control', ()), ('ms-System', ('System',)), ('ms-SystemCore', ('System.Core',)), ('ms-both', ('System', 'System.Core'))):
        folder = OUT / label
        folder.mkdir(exist_ok=True)
        output = folder / 'KairoLibrary.dll'
        flags = [f if not f.startswith('/out:') else '/out:' + str(output) for f in manifest['flags']]
        references = [microsoft.get(Path(r['path']).stem, Path(r['path'])) if Path(r['path']).stem in names else Path(r['path']) for r in manifest['references']]
        args = [f for f in flags if f != '/noconfig'] + ['/reference:' + str(p) for p in references] + [sources[s['path']] for s in manifest['sources']]
        response = folder / 'compile.rsp'
        response.write_text('\n'.join('"' + a + '"' for a in args) + '\n', encoding='utf-8')
        process = subprocess.run([manifest['compiler'], '/noconfig', '@' + str(response)], capture_output=True)
        (folder / 'compiler.log').write_bytes(process.stdout + process.stderr)
        item = {'exit_code': process.returncode, 'flags': flags, 'references': [{'path': str(p), 'sha256': sha(p)} for p in references]}
        result['variants'][label] = item
        if process.returncode:
            item['failure_log'] = str((folder / 'compiler.log').relative_to(ROOT))
        else:
            candidate = dump(output, folder / 'methods.json')
            equal = {k for k, m in original.items() if 'raw_body_hex' in m and not differences(m, candidate.get(k))[0]}
            gains, losses = sorted(equal - baseline_equal), sorted(baseline_equal - equal)
            body_changes = [k for k, m in baseline.items() if differences(m, candidate.get(k))[0]]
            metadata = compare_paths(original_binary, output, folder / 'metadata.json')
            raw = measure_pair(original_binary, output)
            raw_path = ROOT / 'reports' / ('round4-reference-' + label + '-raw-measurement.json')
            raw_path.write_text(json.dumps(raw, indent=2) + '\n', encoding='utf-8')
            item.update({'output_sha256': sha(output), 'body_equal': len(equal), 'gains': gains, 'losses': losses,
                         'changed_from_baseline_methods': body_changes, 'missing_ids': sorted(set(original) - set(candidate)),
                         'extra_ids': sorted(set(candidate) - set(original)), 'types_equal': metadata['types_equal'],
                         'types_total': metadata['types_total'], 'global_differences': metadata['global_differences'],
                         'whole_structural_metadata_equal': metadata['whole_structural_metadata_equal'],
                         'assembly_ref': assembly_ref_columns(original_binary, output, (orig_layout, raw['layouts']['rebuilt'])),
                         'raw_measurement': raw_path.relative_to(ROOT).as_posix()})
        REPORT.write_text(json.dumps(result, indent=2) + '\n', encoding='utf-8')
        print(json.dumps({'variant': label, 'exit_code': item['exit_code'], 'body_equal': item.get('body_equal'),
                          'gains': len(item.get('gains', [])), 'losses': len(item.get('losses', [])),
                          'changed_from_baseline': len(item.get('changed_from_baseline_methods', [])),
                          'types_equal': item.get('types_equal'), 'assembly_ref_bytes': item.get('assembly_ref', {}).get('different_bytes')}), flush=True)


if __name__ == '__main__':
    main()
