"""Read-only first-IL-divergence and size measurements; never updates state.

Body acceptance remains compare_kairo.differences. This diagnostic slices out
the IL bytes, retaining branches/operand widths and resolving metadata operands
exactly as the verifier does. Headers, locals and EH are reported separately.
"""
import argparse
from collections import defaultdict
import csv
import json
from pathlib import Path

from compare_kairo import body_cells, differences, sha

ROOT = Path(__file__).resolve().parents[1]
LABELS = {'KairoLibrary': 'roslyn-1.1.0', 'Assembly-CSharp': 'mono-smcs'}
BINS = ((64, '<=64'), (256, '65-256'), (1024, '257-1024'), (float('inf'), '>1024'))


def read(path):
    return json.loads(Path(path).read_text(encoding='utf-8-sig'))


def il_cells(method):
    cells = body_cells(method)
    if cells is None:
        return []
    start = method['header']['size']
    return cells[start:start + method['header']['code_size']]


def first_difference(original, rebuilt):
    """The offset can point inside an operand; no branch normalization occurs."""
    left, right = il_cells(original), il_cells(rebuilt) if rebuilt else []
    common = min(len(left), len(right))
    for i in range(common):
        if left[i] != right[i]:
            return i
    return common if len(left) != len(right) else None


def instruction_at(method, offset):
    if not method or offset is None or offset >= method.get('header', {}).get('code_size', 0):
        return {'opcode': 'END_IL', 'operand': None}, []
    instructions = method.get('instructions', [])
    for index, instruction in enumerate(instructions):
        stop = instructions[index + 1]['offset'] if index + 1 < len(instructions) else method['header']['code_size']
        if instruction['offset'] <= offset < stop:
            return instruction, instructions[max(0, index - 2):index + 3]
    raise ValueError('IL offset is not covered by an instruction')


def instruction_key(instruction):
    return json.dumps({k: instruction[k] for k in ('opcode', 'operand')}, sort_keys=True, separators=(',', ':'))


def measure(assembly, folder, receipt_path, ledger):
    original_path, rebuilt_path = folder / 'original-methods.json', folder / 'rebuilt-methods.json'
    original = {m['stable_id']: m for m in read(original_path)['selected_methods']}
    rebuilt = {m['stable_id']: m for m in read(rebuilt_path)['selected_methods']}
    receipt = read(receipt_path)
    receipt_by_stable = {v['stable_id']: (key, v) for key, v in receipt['methods'].items()}
    bins = {name: {'methods': 0, 'original_il_bytes': 0} for _, name in BINS}
    exact_groups, opcode_groups = defaultdict(list), defaultdict(list)
    rows = []
    for key, left in original.items():
        if 'raw_body_hex' not in left:
            continue
        right = rebuilt.get(key)
        facets, _ = differences(left, right)
        if not facets:
            continue
        ledger_id, proof = receipt_by_stable[key]
        size = left['header']['code_size']
        for maximum, name in BINS:
            if size <= maximum:
                bins[name]['methods'] += 1
                bins[name]['original_il_bytes'] += size
                break
        offset = first_difference(left, right)
        li, lc = instruction_at(left, offset)
        ri, rc = instruction_at(right, offset)
        if offset is None:
            li = ri = {'opcode': 'IL_IDENTICAL', 'operand': None}
        exact = instruction_key(li) + ' => ' + instruction_key(ri)
        opcode = li['opcode'] + ' => ' + ri['opcode']
        item = {'id': ledger_id, 'stable_id': key, 'source': ledger[ledger_id]['fonte'],
                'original_il_bytes': size, 'rebuilt_il_bytes': right['header']['code_size'] if right else 0,
                'first_il_difference_offset': offset, 'first_original_instruction': li,
                'first_rebuilt_instruction': ri, 'original_context': lc, 'rebuilt_context': rc,
                'first_pair_exact': exact, 'first_pair_opcodes': opcode,
                'body_differences': facets, 'original_exception_regions': left.get('exception_regions', []),
                'rebuilt_exception_regions': right.get('exception_regions', []) if right else []}
        rows.append(item)
        exact_groups[exact].append(ledger_id)
        opcode_groups[opcode].append(ledger_id)
    by_id = {r['id']: r for r in rows}
    def histogram(groups):
        return [{'pair': pair, 'methods': len(ids), 'original_il_bytes': sum(by_id[i]['original_il_bytes'] for i in ids),
                 'method_ids': ids, 'example': by_id[ids[0]]}
                for pair, ids in sorted(groups.items(), key=lambda p: (-len(p[1]), p[0]))]
    assembly_ledger = [r for r in ledger.values() if r['assembly'] == assembly]
    matched = [r for r in assembly_ledger if r['estado'] == 'matched']
    return {'assembly': assembly, 'compiler': LABELS[assembly],
            'provenance': {'original_dump': str(original_path.relative_to(ROOT)), 'original_dump_sha256': sha(original_path),
                           'rebuilt_dump': str(rebuilt_path.relative_to(ROOT)), 'rebuilt_dump_sha256': sha(rebuilt_path),
                           'receipt': str(receipt_path.relative_to(ROOT)), 'receipt_sha256': sha(receipt_path),
                           'build_sha256': receipt['build_sha256']},
            'total_methods': len(assembly_ledger), 'matched_methods': len(matched),
            'original_total_il_bytes': sum(int(r['tamanho_il']) for r in assembly_ledger),
            'matched_original_il_bytes': sum(int(r['tamanho_il']) for r in matched),
            'divergent_bodies': len(rows), 'divergent_original_il_bytes': sum(r['original_il_bytes'] for r in rows),
            'equal_il_but_other_body_differences': sum(r['first_il_difference_offset'] is None for r in rows),
            'size_distribution': bins, 'first_pair_exact_histogram': histogram(exact_groups),
            'first_pair_opcode_histogram': histogram(opcode_groups), 'methods': rows}


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--snapshot', type=Path, help='Folder containing assembly-original/ rebuilt-methods and verification JSON and methods.csv')
    parser.add_argument('--label', default='baseline')
    args = parser.parse_args()
    if args.snapshot:
        args.snapshot = args.snapshot.resolve()
    ledger_path = args.snapshot / 'methods.csv' if args.snapshot else ROOT / 'config/methods.csv'
    with ledger_path.open(encoding='utf-8-sig', newline='') as stream:
        ledger = {r['id']: r for r in csv.DictReader(stream)}
    result = {'schema_version': 1, 'operation': 'read-only-diagnostic',
              'metric': 'First unequal IL byte with only metadata tokens resolved; containing instruction pair. Header, local signature and EH remain mandatory in body verdict.',
              'pair_grouping': 'Exact opcode and resolved operand; opcode-only projection is diagnostic and not verification normalization.',
              'ledger_sha256': sha(ledger_path), 'assemblies': {}}
    for assembly, label in LABELS.items():
        folder = args.snapshot / assembly if args.snapshot else ROOT / 'build' / (assembly + '-' + label)
        receipt = folder / 'verification.json' if args.snapshot else ROOT / 'build' / ('verification-' + assembly + '.json')
        result['assemblies'][assembly] = measure(assembly, folder, receipt, ledger)
    target = ROOT / 'reports' / ('round4-' + args.label + '-measurement.json')
    target.write_text(json.dumps(result, indent=2) + '\n', encoding='utf-8')
    csv_path = target.with_suffix('.csv')
    with csv_path.open('w', encoding='utf-8', newline='') as stream:
        fields = ('assembly', 'id', 'original_il_bytes', 'rebuilt_il_bytes', 'first_il_difference_offset', 'first_pair_opcodes', 'first_pair_exact', 'body_differences')
        writer = csv.DictWriter(stream, fieldnames=fields, lineterminator='\n'); writer.writeheader()
        for assembly, data in result['assemblies'].items():
            for row in data['methods']:
                record = {k: row[k] for k in fields if k != 'assembly'}
                record['assembly'] = assembly
                record['body_differences'] = ';'.join(record['body_differences'])
                writer.writerow(record)
    print(json.dumps({'output': target.relative_to(ROOT).as_posix(), 'assemblies': {a: {k: d[k] for k in ('divergent_bodies', 'divergent_original_il_bytes', 'equal_il_but_other_body_differences', 'size_distribution')} for a, d in result['assemblies'].items()}}))


if __name__ == '__main__':
    main()
