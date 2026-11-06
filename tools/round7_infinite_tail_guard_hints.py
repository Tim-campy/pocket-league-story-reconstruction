"""Read-only evidence for an original explicit tail break before a back edge."""


def infinite_tail_guard_hints(original, rebuilt, original_offset, rebuilt_offset):
    left,right=original.get('instructions',[]),rebuilt.get('instructions',[])
    def branch(instruction):
        operand=instruction.get('operand')
        return operand.get('branch') if isinstance(operand,dict) else None
    oi=next((n for n,i in enumerate(left) if i['offset']==original_offset),None)
    ri=next((n for n,i in enumerate(right) if i['offset']==rebuilt_offset),None)
    if oi is None or ri is None:return {}
    families=('brfalse','brtrue','beq','bne','blt','ble','bgt','bge')
    # Alignment commonly points at the inserted break branch, immediately
    # after an otherwise matched conditional, rather than at the condition.
    for start in range(oi,max(-1,oi-3),-1):
        if start+2>=len(left):continue
        conditional,exit_branch,back=left[start:start+3]
        old_target=branch(conditional);exit_target=branch(exit_branch);back_target=branch(back)
        if not conditional['opcode'].startswith(families):continue
        if exit_branch['opcode']!='br' or back['opcode']!='br':continue
        if old_target!=back['offset'] or not isinstance(exit_target,int) or exit_target!=back['offset']+5:continue
        if not isinstance(back_target,int) or back_target>=conditional['offset']:continue
        for n in range(ri,max(-1,ri-3),-1):
            rebuilt_target=branch(right[n])
            if not right[n]['opcode'].startswith(families):continue
            if not isinstance(rebuilt_target,int) or rebuilt_target>=right[n]['offset']:continue
            return {'original_infinite_tail_guard':True,'infinite_tail_original_offset':conditional['offset'],
                'infinite_tail_original_loop_start':back_target,'infinite_tail_original_exit':exit_target,
                'infinite_tail_rebuilt_offset':right[n]['offset']}
    return {}
