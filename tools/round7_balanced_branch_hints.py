"""Original missing branch and rebuilt redundant long branch, read-only evidence."""


def balanced_branch_hints(original, rebuilt, original_offset, rebuilt_offset):
    left,right=original.get('instructions',[]),rebuilt.get('instructions',[])
    oi=next((n for n,i in enumerate(left) if i['offset']==original_offset),None)
    ri=next((n for n,i in enumerate(right) if i['offset']==rebuilt_offset),None)
    if oi is None or ri is None or oi+2>=len(left) or ri+1>=len(right):return {}
    conditional,skip,body=left[oi:oi+3]
    families=('brfalse','brtrue','beq','bne','bge','bgt','ble','blt')
    if not conditional['opcode'].startswith(families) or not right[ri]['opcode'].startswith(families):return {}
    original_branch=conditional.get('operand');rebuilt_branch=right[ri].get('operand')
    if not isinstance(original_branch,dict) or 'branch' not in original_branch:return {}
    if not isinstance(rebuilt_branch,dict) or 'branch' not in rebuilt_branch:return {}
    if conditional['opcode'].endswith('.s') or right[ri]['opcode'].endswith('.s'):return {}
    if skip['opcode']!='br' or skip['offset']-conditional['offset']!=5 or body['offset']-skip['offset']!=5:return {}
    if original_branch['branch']!=body['offset']:return {}
    if right[ri+1]['opcode']=='br':return {}
    donors=[instruction['offset'] for n,instruction in enumerate(right[:-1])
        if instruction['offset']>rebuilt_offset and instruction['opcode']=='br'
        and right[n+1]['offset']-instruction['offset']==5 and isinstance(instruction.get('operand'),dict)
        and instruction['operand'].get('branch')==right[n+1]['offset']]
    if not donors:return {}
    return {'balanced_missing_long_branch':True,'balanced_extra_fallthrough_long_branch':True,
        'balanced_added_width':5,'balanced_removed_width':5,'balanced_donor_il_offsets':donors}
