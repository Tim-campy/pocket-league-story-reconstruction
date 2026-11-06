"""Read-only IL alignment. This never changes the byte-exact body oracle.

Branches are sequence-aligned by operation and symbolic label arity, then their
destinations are checked through the matched-instruction map. Thus an inserted
instruction does not make an earlier branch operand the source diagnosis, while
a genuinely different target remains visible. Short/long branch encodings are
reported separately, not used to hide sequence edits.
"""
import argparse
from collections import defaultdict
from difflib import SequenceMatcher
import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]


def _branch_targets(instruction):
    operand = instruction.get('operand')
    if isinstance(operand, dict):
        if 'branch' in operand:
            return [operand['branch']]
        if 'branches' in operand:
            return list(operand['branches'])
        if 'switch' in operand:
            return list(operand['switch'])
    return None


def _token(instruction):
    targets = _branch_targets(instruction)
    opcode = instruction['opcode']
    if targets is not None:
        if opcode.endswith('.s'):
            opcode = opcode[:-2]
        return (opcode, 'LABELS', len(targets))
    return (opcode, json.dumps(instruction.get('operand'), sort_keys=True, separators=(',', ':')))


def _end(method, instructions):
    return method.get('header', {}).get('code_size', instructions[-1]['offset'] + 1 if instructions else 0)


def _at(instructions, index):
    return instructions[index] if index < len(instructions) else {'opcode': 'END_IL', 'operand': None}


def _labelled_context(instructions, start, stop, labels):
    result = []
    for i in range(max(0, start - 2), min(len(instructions), max(start + 1, stop) + 2)):
        row = dict(instructions[i])
        targets = _branch_targets(row)
        if targets is not None:
            row['operand'] = {'labels': [labels.get(offset, 'INVALID_' + str(offset)) for offset in targets]}
        if row['offset'] in labels:
            row['label'] = labels[row['offset']]
        result.append(row)
    return result[:15]


def align_methods(original, rebuilt):
    """Locate the first sequence edit, retaining genuine target changes.

    original_offset/rebuilt_offset are independently mapped instruction starts,
    not an assumption that equal numeric offsets denote the same command.
    Empty sides of insert/delete are represented by the next instruction.
    """
    rebuilt = rebuilt or {}
    left, right = original.get('instructions', []), rebuilt.get('instructions', [])
    left_end, right_end = _end(original, left), _end(rebuilt, right)
    matcher = SequenceMatcher(None, [_token(i) for i in left], [_token(i) for i in right], autojunk=False)
    opcodes = matcher.get_opcodes()
    pairs = {}
    changes = []
    for kind, a, b, c, d in opcodes:
        if kind == 'equal':
            pairs.update(zip(range(a, b), range(c, d)))
        else:
            changes.append({'kind': kind, 'original_start': a, 'original_stop': b,
                            'rebuilt_start': c, 'rebuilt_stop': d})
    left_indices = {row['offset']: i for i, row in enumerate(left)}
    right_indices = {row['offset']: i for i, row in enumerate(right)}
    left_indices[left_end], right_indices[right_end] = len(left), len(right)
    pairs[len(left)] = len(right)
    retargets, deferred, width_changes = [], [], []
    for i, j in sorted(pairs.items()):
        if i == len(left):
            continue
        targets_l, targets_r = _branch_targets(left[i]), _branch_targets(right[j])
        if targets_l is None:
            continue
        if left[i]['opcode'] != right[j]['opcode']:
            width_changes.append({'original_offset': left[i]['offset'], 'rebuilt_offset': right[j]['offset'],
                                  'original_opcode': left[i]['opcode'], 'rebuilt_opcode': right[j]['opcode']})
        for ordinal, (a, b) in enumerate(zip(targets_l, targets_r)):
            ai, bi = left_indices.get(a), right_indices.get(b)
            if ai in pairs:
                if pairs[ai] != bi:
                    item = {'kind': 'branch_retarget', 'original_start': i, 'original_stop': i + 1,
                            'rebuilt_start': j, 'rebuilt_stop': j + 1, 'target_ordinal': ordinal,
                            'original_target': a, 'rebuilt_target': b}
                    retargets.append(item)
            else:
                # Target belongs to a changed range: report it, and diagnose
                # that changed range rather than pretending the target matched.
                deferred.append({'original_offset': left[i]['offset'], 'rebuilt_offset': right[j]['offset'],
                                 'target_ordinal': ordinal, 'original_target': a, 'rebuilt_target': b,
                                 'status': 'target_in_unmatched_sequence_range'})
    # A mismatching destination can also be a symptom of replacing/deleting its
    # target block. Prefer a substantive edit when the target lies in that edit.
    events = changes + retargets
    selected = min(events, key=lambda e: (e['original_start'] + e['rebuilt_start'], e['original_start'])) if events else None
    labels_l, labels_r = {}, {}
    destinations_l = {x for row in left for x in (_branch_targets(row) or [])}
    destinations_r = {x for row in right for x in (_branch_targets(row) or [])}
    label_number = 0
    for i, j in sorted(pairs.items()):
        a = left_end if i == len(left) else left[i]['offset']
        b = right_end if j == len(right) else right[j]['offset']
        if a in destinations_l or b in destinations_r:
            labels_l[a] = labels_r[b] = 'L' + str(label_number)
            label_number += 1
    for a in sorted(destinations_l - labels_l.keys()):
        labels_l[a] = 'O' + str(a)
    for b in sorted(destinations_r - labels_r.keys()):
        labels_r[b] = 'R' + str(b)
    if selected:
        a, b, c, d = (selected[k] for k in ('original_start', 'original_stop', 'rebuilt_start', 'rebuilt_stop'))
        li = _at(left, a) if a < b else {'opcode': 'INSERTION_POINT', 'operand': None}
        ri = _at(right, c) if c < d else {'opcode': 'DELETION_POINT', 'operand': None}
        kind = selected['kind']
    else:
        a = b = len(left)
        c = d = len(right)
        li = ri = {'opcode': 'IL_ALIGNED_IDENTICAL', 'operand': None}
        kind = 'aligned_identical'
    exact = lambda row: json.dumps({k: row.get(k) for k in ('opcode', 'operand')}, sort_keys=True, separators=(',', ':'))
    return {'algorithm': 'SequenceMatcher-autojunk-false-with-branch-label-correspondence',
            'diagnostic_only': True, 'kind': kind,
            'original_offset': left[a]['offset'] if a < len(left) else left_end,
            'rebuilt_offset': right[c]['offset'] if c < len(right) else right_end,
            'original_index': a, 'rebuilt_index': c,
            'original_instruction': li, 'rebuilt_instruction': ri,
            'pair': exact(li) + ' => ' + exact(ri),
            'opcode_pair': li['opcode'] + ' => ' + ri['opcode'],
            'original_context': _labelled_context(left, a, b, labels_l),
            'rebuilt_context': _labelled_context(right, c, d, labels_r),
            'original_labels': labels_l, 'rebuilt_labels': labels_r,
            'sequence_edits': changes, 'branch_retarget_differences': retargets,
            'deferred_branch_targets': deferred, 'branch_width_differences': width_changes,
            'original_exception_regions': original.get('exception_regions', []),
            'rebuilt_exception_regions': rebuilt.get('exception_regions', [])}


def make_histogram(original_dump, rebuilt_dump, selected_ids=None):
    def dictionary(value):
        if isinstance(value, (str, Path)):
            value = json.loads(Path(value).read_text(encoding='utf-8-sig'))
        return {m['stable_id']: m for m in value['selected_methods']} if 'selected_methods' in value else value
    original, rebuilt = dictionary(original_dump), dictionary(rebuilt_dump)
    selected = selected_ids if selected_ids is not None else original
    rows = []
    for identity in selected:
        left = original[identity]
        if 'raw_body_hex' not in left:
            continue
        row = align_methods(left, rebuilt.get(identity))
        row.update(stable_id=identity, original_il_bytes=left['header']['code_size'])
        rows.append(row)
    def group(field):
        groups = defaultdict(list)
        for row in rows:
            groups[row[field]].append(row)
        return [{'pair': key, 'methods': len(values), 'original_il_bytes': sum(v['original_il_bytes'] for v in values),
                 'method_ids': [v['stable_id'] for v in values], 'example': values[0]}
                for key, values in sorted(groups.items(), key=lambda item: (-len(item[1]), item[0]))]
    return {'methods': rows, 'exact_histogram': group('pair'), 'opcode_histogram': group('opcode_pair')}


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('original', type=Path)
    parser.add_argument('rebuilt', type=Path)
    parser.add_argument('--ids', type=Path)
    parser.add_argument('--output', type=Path, required=True)
    args = parser.parse_args()
    selected = json.loads(args.ids.read_text()) if args.ids else None
    args.output.write_text(json.dumps(make_histogram(args.original, args.rebuilt, selected), indent=2) + '\n', encoding='utf-8', newline='\n')
