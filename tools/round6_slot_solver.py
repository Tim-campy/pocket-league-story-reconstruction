"""Calculated source declarations from slot uses; never emits or patches IL.

The IL contributes type-constrained slot-use correspondences and lifetime ranges.
All proposed text still requires the source-only predictor and the real verifier.
"""
from collections import Counter, defaultdict
from difflib import SequenceMatcher
import hashlib
import json
import re


def local_access(instruction):
    op = instruction['opcode']
    for family in ('ldloca', 'ldloc', 'stloc'):
        if op.startswith(family + '.') and op[len(family) + 1:].isdigit():
            return family, int(op[len(family) + 1:])
        if op in (family, family + '.s') and isinstance(instruction.get('operand'), dict):
            return family, instruction['operand']['local']
    return None


def lifetimes(method):
    rows = defaultdict(list)
    for ins in method.get('instructions', []):
        access = local_access(ins)
        if access:
            rows[access[1]].append((ins['offset'], access[0]))
    return [{'slot': slot, 'first_use': uses[0][0], 'last_use': uses[-1][0],
             'loads': sum(op.startswith('ldloc') for _, op in uses),
             'stores': sum(op == 'stloc' for _, op in uses)}
            for slot, uses in sorted(rows.items())]


def correspondences(original, rebuilt, original_types, rebuilt_types):
    """Align access families and types, then vote on explicit slot identities."""
    left, right = original.get('instructions', []), rebuilt.get('instructions', [])

    def token(ins, types):
        access = local_access(ins)
        if access:
            family, slot = access
            return family, types[slot] if slot < len(types) else 'UNKNOWN'
        operand = ins.get('operand')
        if isinstance(operand, dict) and any(k in operand for k in ('branch', 'switch', 'branches')):
            return ins['opcode'].removesuffix('.s'), '<label>'
        return ins['opcode'], json.dumps(operand, sort_keys=True)

    match = SequenceMatcher(a=[token(i, original_types) for i in left],
                            b=[token(i, rebuilt_types) for i in right], autojunk=False)
    votes = defaultdict(Counter)
    for block in match.get_matching_blocks():
        for k in range(block.size):
            old, new = local_access(left[block.a + k]), local_access(right[block.b + k])
            if old and new and old[0] == new[0]:
                votes[new[1]][old[1]] += 1
    selected, evidence = {}, []
    for current, candidates in sorted(votes.items()):
        ranked = candidates.most_common()
        wanted, count = ranked[0]
        total = sum(candidates.values())
        unique = len(ranked) == 1 or count > ranked[1][1]
        # Distinct majority and >= 3/4 of aligned uses. Ambiguous identities are
        # deliberately not assigned; lifetime intervals alone cannot prove them.
        if unique and count * 4 >= total * 3:
            selected[current] = wanted
        evidence.append({'rebuilt_slot': current, 'original_slot': wanted,
                         'votes': dict(candidates), 'selected': current in selected})
    return selected, evidence


def _text_at(source, start, end):
    return source.encode('utf-16-le')[start * 2:end * 2].decode('utf-16-le')


def _edit(source, edits):
    raw = source.encode('utf-16-le')
    for start, end, replacement in sorted(edits, reverse=True):
        raw = raw[:start * 2] + replacement.encode('utf-16-le') + raw[end * 2:]
    return raw.decode('utf-16-le')


def _patch(source, model, replacement, rule):
    start, end = model['declaration_start'], model['declaration_end']
    return {'start': start, 'end': end, 'before': _text_at(source, start, end),
            'after': _text_at(replacement, start, len(replacement.encode('utf-16-le')) // 2
                              - (len(source.encode('utf-16-le')) // 2 - end)),
            'rule': rule, 'distance': 0}


def proposals(source, model, original, rebuilt, original_types, rebuilt_types):
    result = {'operation': 'declaration-calculation-from-original-slot-uses',
              'original_lifetimes': lifetimes(original), 'patches': [],
              'constraints': [], 'source_only': True}
    if not model.get('complete'):
        result['constraints'].append('source_model_incomplete')
        return result
    slots, evidence = correspondences(original, rebuilt, original_types, rebuilt_types)
    result['slot_correspondence'] = evidence
    declared = [v for v in model['variables'] if v['kind'] == 'declared'
                and v.get('statement_start', -1) >= 0]
    if len(declared) < 2:
        result['constraints'].append('fewer_than_two_ordinary_block_declarations')
        return result
    result['calculated_declarations'] = [{'name': v['name'], 'type': v['type'],
        'rebuilt_slot': v['slot'], 'desired_slot': slots.get(v['slot']),
        'lexical_block': v['lexical_scope_start'],
        'constraint': 'same lexical block; group consecutive desired slots before earliest declaration'
                      if v['slot'] in slots else 'UNKNOWN: ambiguous aligned uses'} for v in declared]
    scopes = defaultdict(list)
    for variable in declared:
        if variable['slot'] in slots:
            scopes[variable['lexical_scope_start']].append(variable)
    clusters=[]
    for scope, variables in scopes.items():
        ordered=sorted(variables,key=lambda v:slots[v['slot']])
        run=[]
        for variable in ordered:
            if run and slots[variable['slot']] != slots[run[-1]['slot']] + 1:
                if len(run)>1:clusters.append(run)
                run=[]
            run.append(variable)
        if len(run)>1:clusters.append(run)
    if not clusters:
        result['constraints'].append('no_injective_consecutive_slot_run_within_one_lexical_block')
        return result
    combined=[]
    for cluster in clusters:
        group_edits, group_patches = _group_candidates(source,model,original,original_types,slots,declared,cluster)
        combined.extend(group_edits)
        result['patches'].extend(group_patches)
    if len(clusters)>1 and combined:
        result['patches'].append(_patch(source,model,_edit(source,combined),'solver-group-all-calculated-blocks'))
    result['patches'] = list({hashlib.sha256(p['after'].encode()).hexdigest(): p for p in result['patches']}.values())
    return result


def _group_candidates(source,model,original,original_types,slots,all_declared,declared):
    patches=[]
    # Types and desired order come from the original, while names remain those
    # of the source. No names are guessed to influence hash enumeration.
    ordered = sorted(declared, key=lambda v: slots[v['slot']])
    if any(slots[v['slot']] >= len(original_types)
           or original_types[slots[v['slot']]] != v['type'] for v in ordered):
        return [],[]
    groups = []
    for variable in ordered:
        syntax_type = variable['type_source']
        if syntax_type == 'var':
            syntax_type = variable['type'].replace('/', '.').replace('`1', '').replace('`2', '')
        if groups and groups[-1][0] == syntax_type:
            groups[-1][1].append(variable['name'])
        else:
            groups.append((syntax_type, [variable['name']]))
    declarations = '\n' + '\n'.join(t + ' ' + ', '.join(names) + ';' for t, names in groups) + '\n'
    by_statement = defaultdict(list)
    for variable in declared:
        by_statement[(variable['statement_start'], variable['statement_end'])].append(variable)
    selected_slots={v['slot'] for v in declared}
    if any(v['slot'] not in selected_slots for v in all_declared
           if (v['statement_start'],v['statement_end']) in by_statement):
        return [],[]
    first=min(start for start,end in by_statement)
    edits = []
    for (start, end), variables in by_statement.items():
        replacement = '\n'.join(v['name'] + ' = ' + v['initializer'] + ';'
                                for v in sorted(variables, key=lambda v: v['start']) if v.get('initializer') is not None)
        edits.append((start, end, (declarations if start==first else '')+replacement))
    grouped = _edit(source, edits)
    if grouped != source:
        patches.append(_patch(source, model, grouped, 'solver-group-block-declarations'))
    # The chained zero assignments in the original are a direct store-order
    # constraint. Group declarations first, then reproduce exactly that chain.
    instructions = original.get('instructions', [])
    wanted_to_name = {slots[v['slot']]: v['name'] for v in declared}
    for i, ins in enumerate(instructions):
        if ins['opcode'] != 'ldc.i4.0':
            continue
        stores = []
        for candidate in instructions[i + 1:]:
            access = local_access(candidate)
            if access and access[0] == 'stloc':
                stores.append(access[1])
            elif candidate['opcode'] != 'dup':
                break
        if len(stores) < 2 or not all(slot in wanted_to_name for slot in stores):
            continue
        # Find the corresponding existing zero chain in source by its names;
        # its statement is replaced atomically with the original store order.
        names = [wanted_to_name[slot] for slot in reversed(stores)]
        name_set = set(names)
        for (start, end), variables in sorted(by_statement.items()):
            initializer = next((v.get('initializer') for v in variables
                                if v.get('initializer') and v['name'] in name_set), None)
            if not initializer or not re.search(r'\b0\b', initializer):
                continue
            assigned = set(re.findall(r'\b([A-Za-z_]\w*)\s*=', initializer))
            assigned.update(v['name'] for v in variables if v.get('initializer') == initializer)
            if len(assigned & name_set) < 2 or not assigned.issubset(name_set):
                continue
            zero_edits = [(a, b, (declarations if a==first else '')+' = '.join(names) + ' = 0;' if (a, b) == (start, end) else c)
                          for a, b, c in edits]
            chained = _edit(source, zero_edits)
            if chained != grouped:
                patches.append(_patch(source, model, chained, 'solver-group-original-zero-chain'))
            break
        break
    # Hashes make deduplication cheap without treating calculated candidates as
    # accepted. Only the predictor followed by the official verifier can do so.
    return edits,patches
