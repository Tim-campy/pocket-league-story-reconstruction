"""Independent original-IL hint; importing does not touch source or emit IL."""


def original_empty_zero_arm_hint(original, offset):
    instructions=original.get('instructions',[])
    index=next((n for n,i in enumerate(instructions) if i['offset']==offset),None)
    if index is None or index+2>=len(instructions):
        return {}
    condition,jump,body=instructions[index:index+3]
    if condition['opcode'] not in ('brtrue','brtrue.s') or jump['opcode'] not in ('br','br.s'):
        return {}
    condition_target=condition.get('operand')
    jump_target=jump.get('operand')
    if not isinstance(condition_target,dict) or not isinstance(jump_target,dict):
        return {}
    # Zero falls through directly to an unconditional skip; nonzero jumps
    # exactly over that skip. The syntax rule still requires no case0/default.
    if condition_target.get('branch')==body['offset'] and jump_target.get('branch',-1)>body['offset']:
        return {'original_empty_zero_arm':True}
    return {}
