"""Read-only evidence: original null branch skips a non-null member call."""


def null_polarity_hints(original,rebuilt,original_offset,rebuilt_offset):
    left,right=original.get('instructions',[]),rebuilt.get('instructions',[])
    oi=next((n for n,i in enumerate(left) if i['offset']==original_offset),None)
    ri=next((n for n,i in enumerate(right) if i['offset']==rebuilt_offset),None)
    if oi is None or ri is None:return {}
    old,new=left[oi],right[ri]
    if old['opcode'] not in ('brfalse','brfalse.s') or new['opcode'] not in ('brtrue','brtrue.s'):return {}
    def branch(instruction):
        operand=instruction.get('operand')
        return operand.get('branch') if isinstance(operand,dict) else None
    fallback=branch(old);nonnull=branch(new)
    if not isinstance(fallback,int) or not isinstance(nonnull,int):return {}
    default=next((i for i in left if i['offset']==fallback),None)
    if default is None or not default['opcode'].startswith(('ldc.','ldnull','ldstr')):return {}
    calls=[i for i in left[oi+1:] if i['offset']<fallback and i['opcode'] in ('call','callvirt')]
    exits=[i for i in left[oi+1:] if i['offset']<fallback and i['opcode'] in ('br','br.s') and isinstance(branch(i),int) and branch(i)>fallback]
    if len(calls)!=1 or not exits:return {}
    original_call=calls[0]
    for candidate in right[ri+1:]:
        if candidate['offset']<nonnull or candidate['opcode'] not in ('call','callvirt'):continue
        if candidate.get('operand')==original_call.get('operand'):
            return {'original_nullable_call_first':True,'nullable_original_conditional_offset':old['offset'],
                'nullable_original_call_offset':original_call['offset'],'nullable_original_call_opcode':original_call['opcode'],
                'nullable_original_fallback_offset':fallback,'nullable_rebuilt_nonnull_offset':nonnull,
                'nullable_rebuilt_call_offset':candidate['offset']}
        break
    return {}
