"""Read-only, exhaustive positional PE/CLI comparison after semantic closure.

Every unequal/unpaired byte is retained in CSV evidence. Proven nondeterministic
fields exempt a byte only when its absolute offset, field name and byte index
agree on both sides. Moving a field cannot hide unrelated bytes at either end.
No normalized image/hash is created and no target executable is run.
"""
import argparse
from collections import Counter
import csv
from functools import lru_cache
import hashlib
import json
from pathlib import Path
import re
import struct

from measure_raw_assemblies import measure_pair
from round10_raw_measurement import measure_detail, proof_profile, csv_outputs

ROOT = Path(__file__).resolve().parents[1]
FIELD_SIZES = {'Module.Mvid': 16, 'COFF.TimeDateStamp': 4,
               'IMAGE_DEBUG_DIRECTORY.TimeDateStamp': 4, 'CodeView.RSDS.PdbGuid': 16}


def digest(data):
    return hashlib.sha256(data).hexdigest()


def read(path):
    return json.loads(Path(path).read_text(encoding='utf-8-sig'))


def compilation_authority(base, assembly, receipt, output_sha256):
    """A missing build manifest must never silently weaken the proof profile."""
    folder = base / assembly
    manifest_path = folder / 'build-manifest.json'
    manifest = read(manifest_path)
    if (not re.fullmatch('[0-9a-f]{64}', manifest.get('compiler_sha256', '')) or
            manifest.get('output_sha256') != output_sha256 or manifest.get('exit_code') != 0):
        raise ValueError('Build manifest does not bind compiler and measured output')
    if not manifest.get('sources') or manifest['sources'] != receipt.get('sources'):
        raise ValueError('Build and official verification source inventories differ')
    for item in manifest['sources']:
        path = (folder / item['path']).resolve()
        if not path.is_relative_to(folder.resolve()) or digest(path.read_bytes()) != item['sha256']:
            raise ValueError('Frozen source is absent, outside snapshot, or differs from build')
    snapshot_path = base / 'snapshot.json'
    snapshot = read(snapshot_path)
    ledger = base / 'methods.csv'
    if snapshot.get('ledger_sha256') != digest(ledger.read_bytes()):
        raise ValueError('Snapshot ledger hash is stale')
    if any(receipt.get(k) for k in ('regressions', 'body_regressions', 'type_regressions')):
        raise ValueError('Semantic receipt contains regressions')
    return dict(compiler_sha256=manifest['compiler_sha256'],
                manifest_sha256=digest(manifest_path.read_bytes()),
                snapshot_sha256=digest(snapshot_path.read_bytes()),
                ledger_sha256=digest(ledger.read_bytes()), source_files=len(manifest['sources']))


@lru_cache(maxsize=1)
def opcode_geometry():
    result = read(ROOT / 'tools/round9_control_opcodes.json')
    # Cecil uses these historical display names; the byte stream and the
    # exhaustive comparator remain untouched by this diagnostic-width lookup.
    result['ldelem.any'] = result['ldelem']
    result['stelem.any'] = result['stelem']
    return result


def checked_method_geometry(method, payload):
    """Bound DTO labels by independently parsed header and encoded operand widths."""
    if not payload:
        raise ValueError('Missing method body')
    if payload[0] & 3 == 2:
        size, code_size, form = 1, payload[0] >> 2, 'tiny'
    elif payload[0] & 3 == 3 and len(payload) >= 12:
        size = (int.from_bytes(payload[:2], 'little') >> 12) * 4
        code_size, form = int.from_bytes(payload[4:8], 'little'), 'fat'
        if size < 12:
            raise ValueError('Fat header is shorter than required fields')
    else:
        raise ValueError('Unsupported or truncated method header')
    header = method['header']
    if (header['size'] != size or header['code_size'] != code_size or header['format'] != form
            or size + code_size > len(payload)):
        raise ValueError('Method DTO geometry differs from authenticated bytes')
    opcodes = opcode_geometry()
    widths = {'InlineNone': 0, 'ShortInlineI': 1, 'ShortInlineVar': 1,
              'ShortInlineBrTarget': 1, 'InlineVar': 2, 'InlineI': 4,
              'InlineBrTarget': 4, 'ShortInlineR': 4, 'InlineField': 4,
              'InlineMethod': 4, 'InlineSig': 4, 'InlineString': 4,
              'InlineTok': 4, 'InlineType': 4, 'InlineI8': 8, 'InlineR': 8}
    cursor = 0
    for instruction in method['instructions']:
        if instruction['offset'] != cursor or cursor >= code_size:
            raise ValueError('Instruction offsets do not partition the actual IL')
        info = opcodes[instruction['opcode']]
        kind = info['operand_type']
        operand = size + cursor + info['size']
        if kind == 'InlineSwitch':
            if operand + 4 > size + code_size:
                raise ValueError('Truncated switch count')
            width = 4 + 4 * int.from_bytes(payload[operand:operand + 4], 'little')
        else:
            width = widths[kind]
        cursor += info['size'] + width
        if cursor > code_size:
            raise ValueError('Instruction leaves the authenticated IL body')
    if cursor != code_size:
        raise ValueError('Instructions do not cover every IL byte')
    return size, code_size


def field_map(size, fields):
    result = {}
    for name, span in fields.items():
        if name not in FIELD_SIZES or span['size'] != FIELD_SIZES[name]:
            raise ValueError('Unreviewed field or field size: ' + name)
        start, length = span['offset'], span['size']
        if not isinstance(start, int) or start < 0 or start + length > size:
            raise ValueError('Exception leaves its own input')
        for index in range(length):
            if start + index in result:
                raise ValueError('Overlapping nondeterministic fields')
            result[start + index] = (name, index)
    return result


def differences(left, right, original_fields, rebuilt_fields):
    """Yield literal byte pairs; exemption never uses the union of endpoints."""
    a, b = field_map(len(left), original_fields), field_map(len(right), rebuilt_fields)
    if set(original_fields) != set(rebuilt_fields):
        raise ValueError('Exception field identities differ between inputs')
    for offset in range(max(len(left), len(right))):
        x = left[offset] if offset < len(left) else None
        y = right[offset] if offset < len(right) else None
        if x == y:
            continue
        identity = a.get(offset)
        allowed = identity is not None and identity == b.get(offset)
        yield dict(offset=offset, original=x, rebuilt=y, allowed=allowed,
                   field=identity[0] if allowed else '',
                   original_field=a.get(offset), rebuilt_field=b.get(offset))


def regions(data, layout, detail, side, method_rows=None, methods=None):
    """Deepest region per byte, including rows/columns and all leftover bytes."""
    labels = ['FILE/unclassified-or-overlay'] * len(data)

    def paint(offset, size, label):
        if offset < 0 or size < 0 or offset + size > len(data):
            raise ValueError('Classification region leaves input: ' + label)
        labels[offset:offset + size] = [label] * size

    paint(layout['headers']['offset'], layout['headers']['size'], 'PE_HEADERS/padding')
    for s in layout['sections']:
        paint(s['offset'], s['size'], 'SECTION/' + s['name'])
    # Method rows are parsed from this same PE; every dump body is authenticated
    # against the actual bytes before contributing any diagnostic labels.
    body_groups = {}
    for method in methods or []:
        if not method.get('raw_body_hex'):
            continue
        token = re.fullmatch(r'\[Method:0x([0-9a-fA-F]+)\]', method['token'])
        if not token:
            raise ValueError('Unsupported method-token representation')
        ordinal = int(token.group(1), 16) & 0xffffff
        if not ordinal or not method_rows or ordinal > len(method_rows):
            raise ValueError('Method dump token leaves this PE MethodDef table')
        rva = method_rows[ordinal - 1]['RVA']
        payload = bytes.fromhex(method['raw_body_hex'].replace('-', ''))
        section = next((s for s in layout['sections'] if s['virtual_address'] <= rva and
                        rva + len(payload) <= s['virtual_address'] + s['size']), None)
        if section is None:
            raise ValueError('Method body leaves raw section')
        offset = section['offset'] + rva - section['virtual_address']
        if data[offset:offset + len(payload)] != payload:
            raise ValueError('Method dump body is not the measured PE body')
        body_groups.setdefault(offset, []).append((method, payload))
    for offset, aliases in body_groups.items():
        method, payload = aliases[0]
        if any(raw != payload for _, raw in aliases):
            raise ValueError('Shared method RVA has inconsistent dump bytes')
        label = 'METHOD/' + '|'.join(m['stable_id'] for m, _ in aliases)
        paint(offset, len(payload), label + '/body')
        header, code_size = checked_method_geometry(method, payload)
        paint(offset, header, label + '/header')
        if method['header']['format'] == 'fat':
            paint(offset + 8, 4, label + '/local-signature-token')
        instructions = method['instructions']
        for index, instruction in enumerate(instructions):
            start = instruction['offset']
            end = instructions[index + 1]['offset'] if index + 1 < len(instructions) else code_size
            paint(offset + header + start, end - start,
                  label + '/IL_' + format(start, '04x') + '/' + instruction['opcode'])
    pe = struct.unpack_from('<I', data, 60)[0]
    paint(0, pe, 'DOS_HEADER_AND_STUB')
    paint(pe, 4, 'PE_SIGNATURE')
    coff = [('Machine', 2), ('NumberOfSections', 2), ('TimeDateStamp', 4),
            ('PointerToSymbolTable', 4), ('NumberOfSymbols', 4),
            ('SizeOfOptionalHeader', 2), ('Characteristics', 2)]
    cursor = pe + 4
    for name, size in coff:
        paint(cursor, size, 'COFF/' + name)
        cursor += size
    optional = pe + 24
    optional_size = struct.unpack_from('<H', data, pe + 20)[0]
    paint(optional, optional_size, 'OPTIONAL_HEADER')
    # All bytes are covered even if a future target has an unfamiliar directory.
    magic = struct.unpack_from('<H', data, optional)[0]
    directory_base = optional + (96 if magic == 0x10b else 112)
    common = [('Magic', 0, 2), ('LinkerVersion', 2, 2), ('SizeOfCode', 4, 4),
              ('SizeOfInitializedData', 8, 4), ('SizeOfUninitializedData', 12, 4),
              ('AddressOfEntryPoint', 16, 4), ('BaseOfCode', 20, 4),
              ('SectionAlignment', 32, 4), ('FileAlignment', 36, 4),
              ('VersionFields', 40, 16), ('SizeOfImage', 56, 4),
              ('SizeOfHeaders', 60, 4), ('CheckSum', 64, 4),
              ('Subsystem', 68, 2), ('DllCharacteristics', 70, 2)]
    for name, offset, size in common:
        paint(optional + offset, size, 'OPTIONAL_HEADER/' + name)
    directories = ('Export', 'Import', 'Resource', 'Exception', 'Security',
                   'BaseRelocation', 'Debug', 'Architecture', 'GlobalPtr', 'TLS',
                   'LoadConfig', 'BoundImport', 'IAT', 'DelayImport', 'CLI', 'Reserved')
    number_offset = optional + (92 if magic == 0x10b else 108)
    count = struct.unpack_from('<I', data, number_offset)[0]
    for index in range(min(count, 16)):
        at = directory_base + index * 8
        if at + 8 > optional + optional_size:
            raise ValueError('Data directory leaves optional header')
        paint(at, 4, 'DATA_DIRECTORY/' + directories[index] + '/RVA-or-offset')
        paint(at + 4, 4, 'DATA_DIRECTORY/' + directories[index] + '/Size')
    section_headers = optional + optional_size
    for i, section in enumerate(layout['sections']):
        paint(section_headers + i * 40, 40, 'SECTION_HEADER/' + section['name'])
    m = layout['metadata']
    paint(m['offset'], m['size'], 'METADATA/root-or-gap')
    for stream in layout['streams']:
        paint(stream['offset'], stream['size'], 'METADATA/' + stream['name'])
        paint(stream['header_offset'], 8, 'METADATA/stream-header/' + stream['name'])
    columns = {r['table']: r['columns'] for r in detail['table_columns']}
    for table in layout['tables']:
        cols = columns[table['name']]
        width_key = 'width_' + side
        if sum(c[width_key] for c in cols) != table['row_size']:
            raise ValueError('Column widths do not partition metadata row')
        for ordinal in range(table['row_count']):
            cursor = table['offset'] + ordinal * table['row_size']
            for col in cols:
                width = col[width_key]
                paint(cursor, width, 'METADATA/' + table['name'] +
                      '/row=' + str(ordinal + 1) + '/' + col['name'])
                cursor += width
    coord = detail['coordinates'][side]
    for key in ('debug_directory', 'codeview'):
        if coord.get(key):
            span = coord[key]
            paint(span['offset'], span['size'], 'DEBUG/' + key)
    return labels


def cause(original_region, rebuilt_region):
    if original_region != rebuilt_region:
        return 'Different physical regions at this offset; layout cause UNKNOWN'
    if original_region.endswith('/RVA') or original_region.startswith('DATA_DIRECTORY/'):
        return 'Address or extent differs; compilation/layout cause requires diagnosis'
    if original_region.startswith('METADATA/'):
        return 'Raw metadata/heap bytes differ; index or content cause not inferred'
    if original_region.startswith('METHOD/'):
        return 'Raw method bytes differ; check literal token/index versus other bytes'
    return 'UNKNOWN'


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--snapshot', required=True, type=Path)
    parser.add_argument('--assembly', choices=['KairoLibrary', 'Assembly-CSharp'], default='KairoLibrary')
    parser.add_argument('--label', required=True)
    parser.add_argument('--allow-pending-measurement', action='store_true')
    args = parser.parse_args()
    if not re.fullmatch('[A-Za-z0-9][A-Za-z0-9_.-]*', args.label):
        raise ValueError('Unsafe report label')
    base = args.snapshot.resolve()
    receipt = read(base / args.assembly / 'verification.json')
    manifest = read(base / 'assemblies/artifacts.json')[args.assembly]
    original = ROOT / 'original/extracted/assets/bin/Data/Managed' / (args.assembly + '.dll')
    rebuilt = base / manifest['path']
    left, right = original.read_bytes(), rebuilt.read_bytes()
    if digest(left) != receipt['original_sha256'] or digest(right) != receipt['build_sha256'] or digest(right) != manifest['sha256']:
        raise ValueError('Snapshot/receipt/input identity mismatch')
    authority = compilation_authority(base, args.assembly, receipt, digest(right))
    body_rows = [r for r in receipt['methods'].values() if r.get('body_present')]
    divergent = sum(not r.get('body_equal') for r in body_rows)
    held = sum(r.get('body_equal') and r.get('state') != 'matched' for r in body_rows)
    if (divergent or held) and not args.allow_pending_measurement:
        raise ValueError('Final raw check requires zero divergent bodies and zero held methods')
    if args.assembly == 'Assembly-CSharp' and divergent > 10:
        raise ValueError('Assembly-CSharp raw threshold has not been reached')
    fields, proof = proof_profile(base, args.assembly, False)
    expected = set(FIELD_SIZES) if args.assembly == 'KairoLibrary' else {'Module.Mvid', 'COFF.TimeDateStamp'}
    if set(fields) != expected:
        raise ValueError('Exactly the proven assembly-specific fields are required')
    raw = measure_pair(original, rebuilt)
    detail, metadata_rows = measure_detail(raw, fields)
    coordinates = detail['coordinates']
    selected = {s: {f: coordinates[s]['fields'][f] for f in fields} for s in ('original', 'rebuilt')}
    dumps = {s: read(base / args.assembly / (s + '-methods.json')) for s in ('original', 'rebuilt')}
    maps = {s: regions(data, raw['layouts'][s], detail, s,
                       metadata_rows[s].get('MethodDef'), dumps[s]['selected_methods'])
            for s, data in [('original', left), ('rebuilt', right)]}
    prefix = 'round12-' + args.label + '-' + args.assembly
    outputs = {'report': ROOT / 'reports' / (prefix + '-raw.json'),
               'all_bytes': ROOT / 'reports' / (prefix + '-all-different-bytes.csv'),
               'residual_bytes': ROOT / 'reports' / (prefix + '-remaining-bytes.csv'),
               'layout': ROOT / 'reports' / (prefix + '-layout.json'),
               'detail': ROOT / 'reports' / (prefix + '-detail.json')}
    layer_csv = csv_outputs(raw, detail)
    for name in layer_csv:
        outputs[name] = ROOT / 'reports' / (prefix + '-' + name)
    if any(p.exists() for p in outputs.values()):
        raise FileExistsError('Reports are immutable; choose a new label')
    counts, regions_count = Counter(), Counter()
    headers = ['file_offset_hex', 'original_byte', 'rebuilt_byte', 'allowed_field',
               'original_region', 'rebuilt_region', 'cause']
    with outputs['all_bytes'].open('x', encoding='utf-8', newline='') as af, outputs['residual_bytes'].open('x', encoding='utf-8', newline='') as rf:
        all_writer, residual_writer = (csv.DictWriter(f, fieldnames=headers, lineterminator='\n') for f in (af, rf))
        all_writer.writeheader(); residual_writer.writeheader()
        for d in differences(left, right, selected['original'], selected['rebuilt']):
            off = d['offset']
            a = maps['original'][off] if off < len(left) else 'NO_BYTE'
            b = maps['rebuilt'][off] if off < len(right) else 'NO_BYTE'
            row = dict(file_offset_hex=hex(off), original_byte='' if d['original'] is None else f"{d['original']:02x}",
                       rebuilt_byte='' if d['rebuilt'] is None else f"{d['rebuilt']:02x}",
                       allowed_field=d['field'], original_region=a, rebuilt_region=b,
                       cause='Proven nondeterministic field' if d['allowed'] else cause(a, b))
            all_writer.writerow(row)
            counts['all_different_or_unpaired_bytes'] += 1
            if d['allowed']:
                counts['excluded_differing_bytes'] += 1
            else:
                residual_writer.writerow(row)
                counts['outside_exceptions_differing_bytes'] += 1
                regions_count[a + ' => ' + b] += 1
    residual = counts['outside_exceptions_differing_bytes']
    fields_same = selected['original'] == selected['rebuilt']
    status = 'PASS' if not residual and fields_same and not divergent and not held and len(left) == len(right) else 'DIFFERENT'
    if digest(original.read_bytes()) != digest(left) or digest(rebuilt.read_bytes()) != digest(right):
        raise ValueError('Read-only inputs changed during measurement')
    for key, document in [('layout', raw), ('detail', detail)]:
        outputs[key].write_text(json.dumps(document, indent=2) + '\n', encoding='utf-8')
    for name, text in layer_csv.items():
        outputs[name].write_text(text, encoding='utf-8', newline='\n')
    report = dict(operation='exhaustive-read-only-PE-CLI-byte-comparison', status=status,
                  assembly=args.assembly, baseline=str(base), snapshot=str(base),
                  original_sha256=digest(left), recompiled_sha256=digest(right),
                  original_size=len(left), rebuilt_size=len(right),
                  semantic_divergent_bodies=divergent, held_methods=held,
                  outside_exceptions_differing_bytes=residual,
                  all_different_or_unpaired_bytes=counts['all_different_or_unpaired_bytes'],
                  excluded_differing_bytes=counts['excluded_differing_bytes'],
                  exception_coordinates_equal=fields_same, exception_coordinates=selected,
                  exception_proof=proof, remaining_regions=dict(regions_count.most_common()),
                  compilation_authority=authority,
                  outputs={k: dict(path=str(p.relative_to(ROOT)), sha256=digest(p.read_bytes())) for k, p in outputs.items() if k != 'report'},
                  semantic_receipt_sha256=digest((base / args.assembly / 'verification.json').read_bytes()),
                  method_dump_sha256={s: digest((base / args.assembly / (s + '-methods.json')).read_bytes())
                                      for s in ('original', 'rebuilt')},
                  producer_sha256=digest(Path(__file__).read_bytes()),
                  region_opcode_schema_sha256=digest((ROOT / 'tools/round9_control_opcodes.json').read_bytes()),
                  DLL_writes=False, normalization_applied=False, target_executed=False,
                  method_or_type_states_modified=False,
                  limitations='Causes remain UNKNOWN unless independently established. Sections, streams and table summaries overlap and are not additive. PASS uses literal whole-file offsets, not aligned region summaries.')
    outputs['report'].write_text(json.dumps(report, indent=2) + '\n', encoding='utf-8')
    print(json.dumps({k: report[k] for k in ('status', 'assembly', 'semantic_divergent_bodies', 'held_methods', 'outside_exceptions_differing_bytes', 'excluded_differing_bytes')}))


if __name__ == '__main__':
    main()
