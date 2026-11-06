"""Calculate a missing readonly parameter alias from original and rebuilt IL.

The local/argument/branch projection below is diagnostic only. No assembly is
modified, compiled or accepted by this module. Source proposals still require
the source-only slot model, real compilation and the strict body verifier.
"""
import argparse
from difflib import SequenceMatcher
import json
from pathlib import Path

from compare_kairo import differences, sha

ROOT = Path(__file__).resolve().parents[1]


def local_operation(instruction):
    opcode = instruction['opcode']
    for family in ('ldloca', 'ldloc', 'stloc'):
        if opcode.startswith(family + '.') and opcode[len(family) + 1:].isdigit():
            return family, int(opcode[len(family) + 1:])
        if opcode in (family, family + '.s') and isinstance(instruction.get('operand'), dict):
            return family, instruction['operand'].get('local')
    return None


def parameter_operation(instruction, method):
    """Return the metadata Parameter.Index, excluding the implicit instance this."""
    opcode = instruction['opcode']
    for family in ('ldarga', 'ldarg', 'starg'):
        if opcode.startswith(family + '.') and opcode[len(family) + 1:].isdigit():
            has_this = ';this=True;' in method.get('signature', '')
            return family, int(opcode[len(family) + 1:]) - int(has_this)
        if opcode in (family, family + '.s') and isinstance(instruction.get('operand'), dict):
            # Cecil operand.Index already excludes this for long argument forms.
            return family, instruction['operand'].get('parameter')
    return None


def _token(instruction, method, missing_slot=None, alias_parameter=None):
    local = local_operation(instruction)
    if local:
        family, slot = local
        if missing_slot is not None:
            if slot == missing_slot and family == 'ldloc':
                return 'ldarg', alias_parameter
            if slot > missing_slot:
                slot -= 1
        return family, slot
    parameter = parameter_operation(instruction, method)
    if parameter:
        return parameter
    operand = instruction.get('operand')
    if isinstance(operand, dict) and any(k in operand for k in ('branch', 'switch', 'branches')):
        return instruction['opcode'].removesuffix('.s'), '<diagnostic-label>'
    return instruction['opcode'], json.dumps(operand, sort_keys=True, separators=(',', ':'))


def _unique_context(left, right, old_index, new_index, block):
    """Require one occurrence of a context wholly inside the matched block."""
    for radius in range(4, 0, -1):
        start = max(block.a, old_index - radius)
        stop = min(block.a + block.size, old_index + radius + 1)
        if stop - start < 3:
            continue
        wanted = left[start:stop]
        locations = [i for i in range(len(right) - len(wanted) + 1) if right[i:i + len(wanted)] == wanted]
        if locations == [new_index - (old_index - start)]:
            return {'instructions': stop - start, 'original_start_index': start,
                    'rebuilt_start_index': locations[0]}
    return None


def _store_dominates_uses(method, store, uses):
    """Conservative normal-flow dominance; EH methods are outside this rule."""
    instructions = method.get('instructions', [])
    if method.get('exception_regions') or not instructions:
        return False
    offsets = {row['offset']: n for n, row in enumerate(instructions)}
    successors = {n: set() for n in range(len(instructions))}
    for n, row in enumerate(instructions):
        opcode = row['opcode'].removesuffix('.s')
        operand = row.get('operand')
        targets = []
        if isinstance(operand, dict):
            if 'branch' in operand:
                targets = [operand['branch']]
            else:
                targets = operand.get('switch', operand.get('branches', []))
        if any(target not in offsets for target in targets):
            return False
        successors[n].update(offsets[target] for target in targets)
        if opcode not in ('br', 'leave', 'ret', 'throw', 'rethrow', 'endfinally', 'endfilter', 'jmp') and n + 1 < len(instructions):
            successors[n].add(n + 1)
    reachable, todo = set(), [0]
    while todo:
        node = todo.pop()
        if node not in reachable:
            reachable.add(node)
            todo.extend(successors[node])
    if store not in reachable or not set(uses).issubset(reachable):
        return False
    predecessors = {n: {p for p in reachable if n in successors[p]} for n in reachable}
    dominators = {n: ({0} if n == 0 else set(reachable)) for n in reachable}
    changed = True
    while changed:
        changed = False
        for node in sorted(reachable - {0}):
            incoming = predecessors[node]
            new = {node} | set.intersection(*(dominators[p] for p in incoming))
            if new != dominators[node]:
                dominators[node] = new
                changed = True
    return all(store in dominators[use] for use in uses)


def calculate_parameter_aliases(original, rebuilt):
    """Find one missing alias slot, with every original use uniquely aligned.

    The restricted supported case is original local count = rebuilt count + 1.
    Removing that original slot must exactly reproduce the rebuilt signature.
    The original alias has one direct parameter store, no address uses, and its
    parameter has no writes/address uses. A result is a source hypothesis.
    """
    result = {'operation': 'readonly-single-missing-parameter-alias-calculus', 'diagnostic_only': True,
              'original_local_count': len(original.get('locals', [])), 'rebuilt_local_count': len(rebuilt.get('locals', [])),
              'aliases': [], 'rejected': []}
    old, new = original.get('instructions', []), rebuilt.get('instructions', [])
    if len(original.get('locals', [])) != len(rebuilt.get('locals', [])) + 1:
        result['status'] = 'unsupported_local_count_difference'
        return result
    for index in range(1, len(old)):
        store = local_operation(old[index])
        argument = parameter_operation(old[index - 1], original)
        if not store or store[0] != 'stloc' or not argument or argument[0] != 'ldarg' or argument[1] is None or argument[1] < 0:
            continue
        slot, parameter = store[1], argument[1]
        accesses = [(i, local_operation(ins)) for i, ins in enumerate(old)]
        stores = [i for i, item in accesses if item == ('stloc', slot)]
        loads = [i for i, item in accesses if item == ('ldloc', slot)]
        addresses = [i for i, item in accesses if item == ('ldloca', slot)]
        reject = None
        if len(stores) != 1 or not loads or addresses:
            reject = 'alias_is_not_readonly_with_at_least_one_value_use'
        elif not _store_dominates_uses(original, index, loads):
            reject = 'alias_store_does_not_dominate_all_uses_or_has_exception_flow'
        elif any(parameter_operation(ins, original) in [('starg', parameter), ('ldarga', parameter)] for ins in old):
            reject = 'parameter_has_write_or_address_use'
        elif original['locals'][:slot] + original['locals'][slot + 1:] != rebuilt.get('locals', []):
            reject = 'removing_alias_does_not_explain_local_signature'
        if reject:
            result['rejected'].append({'original_slot': slot, 'parameter_index': parameter, 'reason': reject})
            continue
        selected = [i for i in range(len(old)) if i not in (index - 1, index)]
        old_indices = {position: projected for projected, position in enumerate(selected)}
        left = [_token(old[i], original, slot, parameter) for i in selected]
        right = [_token(ins, rebuilt) for ins in new]
        blocks = SequenceMatcher(None, left, right, autojunk=False).get_matching_blocks()
        mappings = []
        for use in loads:
            projected = old_indices[use]
            block = next((b for b in blocks if b.a <= projected < b.a + b.size), None)
            if block is None:
                reject = 'original_alias_use_not_in_matching_sequence'
                break
            candidate = block.b + projected - block.a
            if parameter_operation(new[candidate], rebuilt) != ('ldarg', parameter):
                reject = 'rebuilt_use_is_not_the_same_direct_parameter'
                break
            context = _unique_context(left, right, projected, candidate, block)
            if context is None:
                reject = 'ambiguous_repeated_alias_use_context'
                break
            mappings.append({'original_offset': old[use]['offset'], 'rebuilt_offset': new[candidate]['offset'],
                             'context': context})
        if reject:
            result['rejected'].append({'original_slot': slot, 'parameter_index': parameter, 'reason': reject})
            continue
        result['aliases'].append({'original_alias_parameter_index': parameter, 'original_alias_slot': slot,
                                  'original_alias_type': original['locals'][slot],
                                  'original_alias_initialize_offset': old[index - 1]['offset'],
                                  'original_alias_store_offset': old[index]['offset'],
                                  'original_alias_first_use': old[loads[0]]['offset'], 'original_alias_last_use': old[loads[-1]]['offset'],
                                  'original_alias_use_count': len(loads), 'original_alias_store_count': 1,
                                  'original_alias_store_dominates_all_uses': True,
                                  'alias_use_rebuilt_offsets': [m['rebuilt_offset'] for m in mappings],
                                  'use_mapping': mappings, 'status': 'unique-calculated-source-hypothesis'})
    result['status'] = 'calculated' if result['aliases'] else 'no_supported_missing_alias'
    return result


def calculated_alias_hints(original, rebuilt, symbol_method):
    """Map calculated IL uses to exact current MDB statement spans and ordinals.

    Require identical complete debug/production bodies before using symbols.
    An occurrence ordinal distinguishes multiple reads of the same parameter
    inside one sequence point; the AST rule must verify the occurrence count.
    """
    if symbol_method is None or differences(rebuilt, symbol_method)[0]:
        return {'alias_calculus_status': 'symbols_body_not_proved_equal'}
    calculation = calculate_parameter_aliases(original, rebuilt)
    if len(calculation['aliases']) != 1:
        return {'alias_calculus_status': calculation['status']}
    alias = calculation['aliases'][0]
    points = sorted((p for p in symbol_method.get('sequence_points', []) if not p.get('hidden')), key=lambda p: p['offset'])
    uses = []
    for offset in alias['alias_use_rebuilt_offsets']:
        preceding = [p for p in points if p['offset'] <= offset]
        if not preceding:
            return {'alias_calculus_status': 'alias_use_has_no_visible_sequence_point'}
        point = preceding[-1]
        stop = next((p['offset'] for p in points if p['offset'] > point['offset']), rebuilt['header']['code_size'])
        operands = [ins['offset'] for ins in rebuilt['instructions'] if point['offset'] <= ins['offset'] < stop
                    and parameter_operation(ins, rebuilt) == ('ldarg', alias['original_alias_parameter_index'])]
        if offset not in operands:
            return {'alias_calculus_status': 'alias_use_not_in_parameter_occurrences'}
        if point.get('start_line', 0) < 1:
            return {'alias_calculus_status': 'source_line_unknown'}
        exact_span = (point.get('end_line', 0) >= point['start_line'] and
                      point.get('start_column', 0) >= 1 and point.get('end_column', 0) >= 1)
        uses.append(dict(point, alias_use_il_offset=offset, parameter_occurrence=operands.index(offset),
                         parameter_occurrences=len(operands), source_span_kind='exact-sequence-span' if exact_span else 'line-only-requires-unique-AST-statement'))
    return {'alias_calculus_status': 'unique-calculated-alias-with-equal-body-symbols',
            'original_alias_parameter_index': alias['original_alias_parameter_index'],
            'original_alias_slot': alias['original_alias_slot'],
            'alias_use_rebuilt_offsets': alias['alias_use_rebuilt_offsets'],
            'alias_use_source_points': uses, 'original_alias_evidence': alias}


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--original', type=Path, default=ROOT / 'build/round6/baseline/Assembly-CSharp/original-methods.json')
    parser.add_argument('--rebuilt', type=Path, default=ROOT / 'build/round6/baseline/Assembly-CSharp/rebuilt-methods.json')
    parser.add_argument('--method-name', default='::DrawGradation(')
    parser.add_argument('--output', type=Path, default=ROOT / 'reports/round6-calculated-parameter-aliases.json')
    args = parser.parse_args()
    old, new = [{m['stable_id']: m for m in json.loads(path.read_text(encoding='utf-8-sig'))['selected_methods']}
                for path in (args.original, args.rebuilt)]
    rows = [dict(calculate_parameter_aliases(method, new[key]), method_id=key)
            for key, method in old.items() if args.method_name in key]
    report = {'operation': 'read-only-original-parameter-alias-calculus', 'baseline_commit':
              '66eea9ddeb8e02a9f357f97f8e7c9dc60f54d6e9', 'candidate_compilations': 0,
              'game_sources_modified': False, 'ledger_modified': False,
              'calculator_sha256': sha(Path(__file__)),
              'inputs': [{'path': str(path), 'sha256': sha(path)} for path in (args.original, args.rebuilt)], 'methods': rows}
    args.output.write_text(json.dumps(report, indent=2) + '\n', encoding='utf-8', newline='\n')
    print(json.dumps([{'method_id': r['method_id'], 'status': r['status'], 'aliases': len(r['aliases']),
                      'use_offsets': [a['alias_use_rebuilt_offsets'] for a in r['aliases']]} for r in rows]))


if __name__ == '__main__':
    main()
