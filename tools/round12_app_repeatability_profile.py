"""Revalidate retained Round3 PEs; no compilation, DLL edit or new exception."""
import hashlib
import json
from pathlib import Path
import uuid

from measure_raw_assemblies import measure_pair
from round10_raw_measurement import measure_detail

ROOT = Path(__file__).resolve().parents[1]
OLD = ROOT / 'reports/round3-Assembly-CSharp-repeatability.json'
OUTPUT = ROOT / 'reports/round12-Assembly-CSharp-repeatability-profile.json'


def sha(path):
    return hashlib.sha256(Path(path).read_bytes()).hexdigest()


def read(path):
    return json.loads(Path(path).read_text(encoding='utf-8-sig'))


def main():
    if OUTPUT.exists():
        raise FileExistsError('Historical and derived reports are immutable')
    old_hash = sha(OLD)
    old = read(OLD)
    folder = ROOT / 'build/Assembly-CSharp-round3-repeatability'
    paths = [folder / 'first.dll', folder / 'Assembly-CSharp.dll']
    payloads = [p.read_bytes() for p in paths]
    manifests = [read(folder / 'first-manifest.json'), read(folder / 'build-manifest.json')]
    assert len(payloads[0]) == len(payloads[1]) == old['output_length']
    for index, side in enumerate(('first', 'second')):
        assert sha(paths[index]) == old[side]['sha256'] == manifests[index]['output_sha256']
        assert manifests[index]['exit_code'] == 0
    assert {k: v for k, v in manifests[0].items() if k != 'output_sha256'} == {
        k: v for k, v in manifests[1].items() if k != 'output_sha256'}
    manifest = manifests[0]
    current = read(ROOT / 'build/Assembly-CSharp-mono-smcs/build-manifest.json')
    for key in ('compiler_sha256', 'runtime_sha256', 'stdlib_binding', 'scoped_environment', 'references'):
        assert manifest[key] == current[key], 'Toolchain changed: ' + key
    assert manifest['compiler_sha256'] == old['compiler_sha256'] == sha(manifest['compiler'])
    assert manifest['runtime_sha256'] == old['runtime_sha256'] == sha(manifest['runtime'])
    assert [f for f in manifest['flags'] if not f.startswith('-out:')] == [
        f for f in current['flags'] if not f.startswith('-out:')]
    assert manifest['sources'] == old['sources']
    available_sources = 0
    for source in manifest['sources']:
        path = Path(source.get('input_path', ROOT / source['path']))
        if path.is_file():
            assert sha(path) == source['sha256'], 'Retained source differs'
            available_sources += 1
    for reference in manifest['references']:
        assert sha(reference['path']) == reference['sha256']
    assert old['post_compile_binary_edits'] is False
    assert old['identified_fields_not_observed_changing'] == []
    assert set(old['fields_observed_changing']) == {'MVID', 'COFF timestamp'}
    # Parse both retained images with the existing REA-backed layout reader.
    raw = measure_pair(*paths)
    detail, _ = measure_detail(raw, [])
    names = {'Module.Mvid': ('MVID', 16), 'COFF.TimeDateStamp': ('COFF timestamp', 4)}
    old_ranges = {row['field']: row for row in old['identified_ranges']}
    assert set(old_ranges) == {name for name, size in names.values()}
    ranges = []
    for name, (legacy, size) in names.items():
        left = detail['coordinates']['original']['fields'][name]
        right = detail['coordinates']['rebuilt']['fields'][name]
        assert left['offset'] == right['offset'] == old_ranges[legacy]['offset']
        assert left['size'] == right['size'] == old_ranges[legacy]['size'] == size
        offset = left['offset']
        data = [value[offset:offset + size] for value in payloads]
        count = sum(a != b for a, b in zip(*data))
        assert count == old_ranges[legacy]['changed_bytes'] and count > 0
        assert data[0].hex() == old_ranges[legacy]['first_hex']
        assert data[1].hex() == old_ranges[legacy]['second_hex']
        ranges.append(dict(field=name, source_field=legacy, offset=offset, size=size,
                           changed_bytes=count, first_hex=data[0].hex(), second_hex=data[1].hex()))
    changed = [i for i, (a, b) in enumerate(zip(*payloads)) if a != b]
    assert changed == old['changed_offsets'] and len(changed) == old['changed_byte_count']
    unexplained = [i for i in changed if not any(r['offset'] <= i < r['offset'] + r['size'] for r in ranges)]
    assert unexplained == old['unexplained_changed_offsets'] == []
    for index, side in enumerate(('first', 'second')):
        coords = detail['coordinates']['original' if index == 0 else 'rebuilt']['fields']
        stamp = coords['COFF.TimeDateStamp']['offset']
        mvid = coords['Module.Mvid']['offset']
        assert int.from_bytes(payloads[index][stamp:stamp + 4], 'little') == old[side]['timestamp']
        assert str(uuid.UUID(bytes_le=payloads[index][mvid:mvid + 16])) == old[side]['mvid']
    result = dict(schema_version=2, operation='physical-revalidation-of-legacy-repeatability-proof',
                  assembly='Assembly-CSharp', source_report=str(OLD.relative_to(ROOT)),
                  source_report_sha256=old_hash, source_report_modified=False,
                  schema_issue='Legacy v1 omits aggregate Boolean; physical ranges and observations were present.',
                  scope=old['scope'], compiler_sha256=manifest['compiler_sha256'],
                  runtime_sha256=manifest['runtime_sha256'], flags=manifest['flags'],
                  scoped_environment=manifest['scoped_environment'], references=manifest['references'],
                  retained_sources_checked=available_sources, retained_source_count=len(manifest['sources']),
                  manifests_identical_except_output_hash=True,
                  manifest_proofs=[dict(path=str(p.relative_to(ROOT)), sha256=sha(p)) for p in
                                   (folder / 'first-manifest.json', folder / 'build-manifest.json')],
                  artifacts=[dict(path=str(p.relative_to(ROOT)), sha256=sha(p), size=len(data))
                             for p, data in zip(paths, payloads)],
                  coordinate_parser_source_hashes=raw['producer_source_hashes'],
                  coordinates=detail['coordinates'], identified_ranges=ranges,
                  fields_observed_changing=[r['field'] for r in ranges],
                  identified_fields_not_observed_changing=[r['field'] for r in ranges if not r['changed_bytes']],
                  all_identified_fields_observed_changing=all(r['changed_bytes'] > 0 for r in ranges),
                  changed_byte_count=len(changed), changed_offsets=changed,
                  unexplained_changed_offsets=unexplained,
                  post_compile_binary_edits=False, compiler_invocations=0, DLL_writes=0,
                  normalization_applied=False, method_or_type_states_modified=False,
                  new_exceptions=[], producer_sha256=sha(Path(__file__)))
    assert sha(OLD) == old_hash
    for path, data in zip(paths, payloads):
        assert sha(path) == hashlib.sha256(data).hexdigest()
    OUTPUT.write_text(json.dumps(result, indent=2, ensure_ascii=False) + '\n', encoding='utf-8', newline='\n')
    print(json.dumps(dict(report=str(OUTPUT.relative_to(ROOT)), sha256=sha(OUTPUT),
                         changed_bytes=len(changed), fields=[r['field'] for r in ranges],
                         retained_sources_checked=available_sources, compiler_invocations=0)))


if __name__ == '__main__':
    main()
