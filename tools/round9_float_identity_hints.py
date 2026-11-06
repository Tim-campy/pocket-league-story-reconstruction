"""Narrow original-IL evidence for an explicit Single identity cast occurrence."""

def parameter_types(identity):
    value=identity.split('::',1)[1];body=value[value.index('(')+1:value.rindex(')->')]
    if not body:return []
    depth=0;start=0;result=[]
    for i,char in enumerate(body):
        if char in '<[(':depth+=1
        elif char in '>])':depth-=1
        elif char==',' and depth==0:result.append(body[start:i]);start=i+1
    result.append(body[start:]);return result

def loaded_parameter(instruction,method):
    op=instruction['opcode'];operand=instruction.get('operand')
    if op in ['ldarg','ldarg.s'] and isinstance(operand,dict):return operand.get('parameter')
    if op.startswith('ldarg.') and op[-1:] in ['0','1','2','3']:
        return int(op[-1])-(0 if method['method_attributes']&0x10 else 1)
    return None

def float_identity_conversion_hints(original,rebuilt,original_offset,rebuilt_offset):
    ins=original.get('instructions',[]);index=next((i for i,n in enumerate(ins) if n['offset']==original_offset),None)
    if index is None or index==0 or ins[index]['opcode']!='conv.r4':return {}
    current=next((n for n in rebuilt.get('instructions',[]) if n['offset']==rebuilt_offset),None)
    if current is None or current['opcode']=='conv.r4':return {}
    ordinal=loaded_parameter(ins[index-1],original);types=parameter_types(original['stable_id'])
    if ordinal is None or ordinal<0 or ordinal>=len(types) or types[ordinal]!='System.Single':return {}
    return dict(round9_identity_conversion_opcode='conv.r4',round9_identity_parameter_ordinal=ordinal,
        round9_identity_conversion_evidence=dict(original_load=ins[index-1],original_conversion=ins[index],
            rebuilt_at_frontier=current,original_parameter_type=types[ordinal],diagnostic_only=True))
