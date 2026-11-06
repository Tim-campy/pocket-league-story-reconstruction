"""Original-IL evidence for source proposals, not a source or slot oracle.

Names for a zero chain come only from the independent source model plus
type-constrained aligned slot-use votes. Ambiguity emits no name hint.
"""
from collections import defaultdict
from round6_slot_solver import correspondences, local_access


def rule_hints(original, rebuilt, offset, original_types, rebuilt_types, model=None):
    instructions = original.get('instructions', [])
    if not instructions:
        return {}
    index = next((n for n, ins in enumerate(instructions) if ins['offset'] >= offset), len(instructions)-1)
    hints = {}
    hints['original_has_no_switch'] = not any(i['opcode']=='switch' for i in instructions)
    test_types, new_types = [], []
    # The first divergent instruction can precede the store (e.g. callvirt).
    for n in range(max(0, index-2), min(len(instructions), index+4)):
        access = local_access(instructions[n])
        if not access or access[0] != 'stloc' or access[1] >= len(original_types):
            continue
        slot, name = access[1], original_types[access[1]]
        if n+2 < len(instructions) and local_access(instructions[n+1]) == ('ldloc', slot):
            tail = instructions[n+2:n+6]
            if any(i['opcode'].startswith(('brtrue','brfalse','beq','bne','bgt','blt','bge','ble')) for i in tail):
                test_types.append(name)
        if n and instructions[n-1]['opcode'] == 'newobj':
            new_types.append(name)
    if test_types:
        hints['expected_test_local_types'] = list(dict.fromkeys(test_types))
    if new_types:
        hints['expected_newobj_local_types'] = list(dict.fromkeys(new_types))
    def targets(ins, offset):
        operand = ins.get('operand')
        return isinstance(operand, dict) and operand.get('branch') == offset
    if instructions[-1]['opcode'] == 'ret':
        returns = sum(i['opcode']=='ret' for i in instructions)
        hints['original_void_common_exit'] = returns==1 and any(targets(i,instructions[-1]['offset']) for i in instructions[:-1])
        if len(instructions)>1:
            access=local_access(instructions[-2])
            if returns==1 and access and access[0]=='ldloc':
                slot=access[1]
                if any(i['opcode'] in ('br','br.s','leave','leave.s') and targets(i,instructions[-2]['offset']) for i in instructions[:-2]):
                    hints['original_unified_return']=True
                    hints['original_return_slot']=slot
    if not model or not model.get('complete'):
        return hints
    votes, _ = correspondences(original, rebuilt, original_types, rebuilt_types)
    wanted_names=defaultdict(set)
    for variable in model.get('variables', []):
        slot=variable.get('slot')
        if variable.get('kind')=='declared' and slot in votes:
            wanted=votes[slot]
            if wanted < len(original_types) and variable.get('type')==original_types[wanted]:
                wanted_names[wanted].add(variable['name'])
    for n, ins in enumerate(instructions):
        if ins['opcode']!='ldc.i4.0':
            continue
        stores=[]
        end=n+1
        while end<len(instructions):
            candidate=instructions[end]
            access=local_access(candidate)
            if access and access[0]=='stloc':
                stores.append(access[1])
            elif candidate['opcode']!='dup':
                break
            end+=1
        end_offset=instructions[end]['offset'] if end<len(instructions) else original['header']['code_size']
        if not (ins['offset']<=offset<end_offset) or len(stores)<2 or len(set(stores))!=len(stores):
            continue
        if any(len(wanted_names[s])!=1 for s in stores):
            continue
        names={s:next(iter(wanted_names[s])) for s in stores}
        if len(set(names.values()))!=len(names):
            continue
        hints['original_zero_chain_names']=[names[s] for s in reversed(stores)]
        hints['original_zero_chain_declaration_order']=[names[s] for s in sorted(stores)]
        break
    return hints
