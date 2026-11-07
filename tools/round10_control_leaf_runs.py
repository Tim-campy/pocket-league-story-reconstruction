"""Associate independently predicted source runs with real own-build IL runs.

This module does not infer AST structure from IL. It requires the entire
independently predicted emission plan to equal the compiled plan first. It
links maximal value runs, not individual instructions to guessed expressions.
It neither aligns the original nor establishes method/body equality.
"""
from collections import defaultdict
from difflib import SequenceMatcher
import json
from round10_control_il import canonical_plan, canonical_tree


def value_runs(ir):
    targets = set()
    for event in ir['emission_plan']:
        if event.get('target_label') is not None:
            targets.add(event['target_label'])
        targets.update(event.get('target_labels', []))
    leaves = {leaf['id']: leaf for leaf in ir.get('leaves', [])}
    runs, current = [], []

    def flush():
        if current:
            runs.append(list(current))
            current.clear()

    for event in ir['emission_plan']:
        if event['kind'] == 'label':
            if event.get('label', event.get('id')) in targets:
                flush()
        elif event['kind'] == 'leaf':
            identity = event.get('leaf_id', event.get('source_node'))
            if identity not in leaves:
                raise ValueError('A value-run event has no recorded leaf: ' + str(identity))
            current.append(leaves[identity])
        else:
            flush()
    flush()
    return runs


def bind_source_to_own_runs(source_ir, compiled_ir):
    result = {'operation': 'source-value-runs-to-own-compiled-IL',
              'complete': False, 'reasons': [], 'bindings': [],
              'original_IL_used': False, 'AST_inferred_from_IL': False,
              'body_equality_claimed': False}
    if source_ir.get('method_id') != compiled_ir.get('method_id'):
        result['reasons'].append('method_identity_differs')
        return result
    if not source_ir.get('emission_plan_complete') or not compiled_ir.get('emission_plan_complete'):
        result['reasons'].append('emission_plan_incomplete')
        return result
    if canonical_plan(source_ir) != canonical_plan(compiled_ir):
        result['reasons'].append('independent_source_plan_differs_from_compiled_plan')
        return result
    try:
        source_runs, compiled_runs = value_runs(source_ir), value_runs(compiled_ir)
    except ValueError as error:
        result['reasons'].append(str(error))
        return result
    if len(source_runs) != len(compiled_runs):
        result['reasons'].append('value_run_count_differs_despite_equal_plan')
        return result
    for ordinal, (source, compiled) in enumerate(zip(source_runs, compiled_runs)):
        spans = [{'start': leaf.get('source_start'), 'end': leaf.get('source_end'),
                  'leaf_id': leaf['id'], 'role': leaf.get('role')} for leaf in source]
        if any(s['start'] is None or s['end'] is None for s in spans):
            result['reasons'].append('source_value_run_has_no_exact_AST_span')
            return result
        result['bindings'].append({'ordinal': ordinal, 'source_spans': spans,
            'own_il_start': min(leaf['il_start'] for leaf in compiled),
            'own_il_end': max(leaf['il_end'] for leaf in compiled),
            'own_instruction_offsets': [offset for leaf in compiled for offset in leaf['instruction_offsets']],
            'own_leaf_ids': [leaf['id'] for leaf in compiled]})
    result['complete'] = True
    return result


def _token(instruction):
    """Diagnostic anchor only; branch destinations are compared by the tree.

    Width, polarity and .un remain. No token from this function is a replacement
    instruction or an input to method verification.
    """
    operand = instruction.get('operand')
    if isinstance(operand, dict):
        for key in ('branch', 'branches', 'switch'):
            if key in operand:
                return instruction['opcode'], 'targets', 1 if key == 'branch' else len(operand[key])
    return instruction['opcode'], json.dumps(operand, sort_keys=True, separators=(',', ':'))


def _occurs_once(sequence, needle):
    if not needle:
        return False
    hits = 0
    for n in range(len(sequence) - len(needle) + 1):
        if sequence[n:n + len(needle)] == needle:
            hits += 1
            if hits > 1:
                return False
    return hits == 1


def unique_instruction_anchors(original, compiled):
    """Only unambiguous exact substrings become original-to-own coordinates."""
    left, right = original['instructions'], compiled['instructions']
    a, b = tuple(map(_token, left)), tuple(map(_token, right))
    mapping, blocks = {}, []
    for block in SequenceMatcher(None, a, b, autojunk=False).get_matching_blocks():
        if not block.size:
            continue
        values = a[block.a:block.a + block.size]
        unique = _occurs_once(a, values) and _occurs_once(b, values)
        blocks.append({'original_start': block.a, 'own_start': block.b,
                       'instructions': block.size, 'unique_in_both': unique})
        if unique:
            for k in range(block.size):
                mapping[left[block.a + k]['offset']] = right[block.b + k]['offset']
    return mapping, blocks


def _leaf_instructions(projected_leaf, method):
    spans = [origin['node']['span'] for origin in projected_leaf['_origins']]
    return [instruction for instruction in method['instructions']
            if any(span['start'] <= instruction['offset'] < span['end'] for span in spans)]


def _projected_leaves(tree,path=()):
    from round10_control_trace import leaves
    return leaves(tree,path)


def bind_control_leaves(original_method, compiled_method, original_ir, source_ir,
                        compiled_ir, project):
    """Require a unique bijection of original value leaves to own source leaves.

    `project` is an independent tree provenance projection whose stripped tree
    must equal canonical_tree. Source/own trees must agree before any pairing.
    Reordered leaves are accepted only with identical complete fingerprints;
    repeated fingerprints need unique instruction context. Ambiguity refuses
    the rewrite, even if one arbitrary pairing would be convenient.
    """
    result = {'operation': 'unique-control-leaf-source-binding', 'complete': False,
              'reasons': [], 'bindings': [], 'offset_map': {},
              'body_equality_claimed': False, 'source_structure_inferred_from_IL': False}
    run_binding = bind_source_to_own_runs(source_ir, compiled_ir)
    result['source_to_own_run_gate'] = {k: v for k, v in run_binding.items() if k != 'bindings'}
    if not run_binding['complete']:
        result['reasons'].append('independent-source-plan-to-own-build-gate-failed')
        return result
    for side, ir in (('original', original_ir), ('source', source_ir), ('own', compiled_ir)):
        if not ir.get('tree_complete') or canonical_tree(ir) is None:
            result['reasons'].append(side + '-structured-tree-incomplete')
    if result['reasons']:
        return result
    if canonical_tree(source_ir) != canonical_tree(compiled_ir):
        result['reasons'].append('source-tree-differs-from-own-build-tree')
        return result
    original_tree, source_tree, compiled_tree = [project(ir, canonical_tree)
        for ir in (original_ir, source_ir, compiled_ir)]
    originals, own = _projected_leaves(original_tree), _projected_leaves(compiled_tree)
    source_by_path = dict(_projected_leaves(source_tree))
    result.update(original_leaf_count=len(originals), own_leaf_count=len(own),
                  source_leaf_count=len(source_by_path))
    if len(originals) != len(own):
        result['reasons'].append('original-own-value-leaf-count-differs')
        return result
    anchors, blocks = unique_instruction_anchors(original_method, compiled_method)
    result['instruction_anchor_blocks'] = blocks
    own_rows, candidates = [], {}
    by_key = defaultdict(list)
    for n, (path, leaf) in enumerate(own):
        instructions = _leaf_instructions(leaf, compiled_method)
        row = {'path': path, 'leaf': leaf, 'instructions': instructions,
               'offsets': [i['offset'] for i in instructions],
               'tokens': tuple(map(_token, instructions))}
        own_rows.append(row)
        by_key[row['tokens']].append(n)
    original_rows = []
    for n, (path, leaf) in enumerate(originals):
        instructions = _leaf_instructions(leaf, original_method)
        tokens = tuple(map(_token, instructions))
        allowed = set(by_key.get(tokens, []))
        used_anchors = [(p, anchors[i['offset']]) for p, i in enumerate(instructions) if i['offset'] in anchors]
        if used_anchors:
            allowed = {j for j in allowed if all(own_rows[j]['offsets'][p] == offset for p, offset in used_anchors)}
        candidates[n] = allowed
        original_rows.append({'path': path, 'leaf': leaf, 'instructions': instructions,
                              'anchored_instructions': len(used_anchors)})
    assigned, used = {}, set()
    while True:
        progress = False
        for n in range(len(originals)):
            if n in assigned:
                continue
            choices = candidates[n] - used
            if len(choices) == 1:
                chosen = next(iter(choices))
                assigned[n] = chosen
                used.add(chosen)
                progress = True
        if not progress:
            break
    result['candidate_leaf_pairs'] = [{'original_path': list(originals[n][0]),
        'own_paths': [list(own[j][0]) for j in sorted(choices)]} for n, choices in candidates.items()]
    if len(assigned) != len(originals):
        result['reasons'].append('value-leaf-fingerprints-differ-or-bijection-not-unique')
        result['unresolved_leaves'] = [list(originals[n][0]) for n in range(len(originals)) if n not in assigned]
        return result
    if len(used) != len(own):
        result['reasons'].append('own-value-leaves-not-fully-consumed')
        return result
    offset_map = {}
    for n, j in sorted(assigned.items()):
        left, right = original_rows[n], own_rows[j]
        source_leaf = source_by_path[right['path']]
        for a, b in zip(left['instructions'], right['instructions']):
            offset_map[a['offset']] = b['offset']
        result['bindings'].append({'original_path': list(left['path']),
            'own_path': list(right['path']), 'source_path': list(right['path']),
            'original_spans': [o['node']['span'] for o in left['leaf']['_origins']],
            'own_spans': [o['node']['span'] for o in right['leaf']['_origins']],
            'original_offsets': [i['offset'] for i in left['instructions']],
            'own_offsets': right['offsets'],
            'source_leaf_ids': [o['node']['id'] for o in source_leaf['_origins']],
            'source_spans': [o['node']['span'] for o in source_leaf['_origins']],
            'anchored_instructions': left['anchored_instructions'],
            'whole_leaf_fingerprint_equal': True})
    result['offset_map'] = {str(k): v for k, v in sorted(offset_map.items())}
    result['complete'] = True
    return result
