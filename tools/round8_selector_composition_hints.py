"""Calculate the named-selector vs emitted-switch-temporary displacement."""
import re


def local_slot(instruction):
    opcode=instruction.get('opcode','');operand=instruction.get('operand')
    if opcode in ('stloc.0','stloc.1','stloc.2','stloc.3','ldloc.0','ldloc.1','ldloc.2','ldloc.3'):return int(opcode[-1])
    if opcode in ('stloc','stloc.s','ldloc','ldloc.s') and isinstance(operand,dict):return operand.get('local')
    return None


def selector_composition_hints(original,rebuilt,expected_types,prediction):
    if prediction.get('complete') is not True:return {}
    current=prediction.get('predicted_types',[])
    for temporary in prediction.get('variables',[]):
        if temporary.get('kind')!='emit-temporary' or temporary.get('name')!='$switch' or temporary.get('type')!='System.Int32':continue
        emitted_slot=temporary.get('slot')
        if not isinstance(emitted_slot,int) or not 0<=emitted_slot<len(current):continue
        for n,call in enumerate(original.get('instructions',[])[:-3]):
            if call['opcode'] not in ('call','callvirt'):continue
            signature=call.get('operand',{}).get('method','') if isinstance(call.get('operand'),dict) else ''
            if '->valuetype System.Int32 @' not in signature:continue
            store,load,branch=original['instructions'][n+1:n+4]
            named_slot=local_slot(store)
            if not store['opcode'].startswith('stloc') or not load['opcode'].startswith('ldloc') or local_slot(load)!=named_slot:continue
            if not isinstance(named_slot,int) or named_slot==emitted_slot or not 0<=named_slot<len(expected_types):continue
            if expected_types[named_slot]!='System.Int32' or not branch['opcode'].startswith(('brtrue','brfalse','beq','bne')):continue
            rearranged=current[:];moved=rearranged.pop(emitted_slot);rearranged.insert(named_slot,moved)
            if rearranged!=expected_types:continue
            for j,other in enumerate(rebuilt.get('instructions',[])[:-1]):
                if other.get('opcode')!=call['opcode'] or other.get('operand')!=call.get('operand'):continue
                next_store=rebuilt['instructions'][j+1]
                if not next_store['opcode'].startswith('stloc') or local_slot(next_store)!=emitted_slot:continue
                name=re.search(r'::([^:`]+)``\d+\(',signature)
                if not name:continue
                return {'round8_selector_named_local_proved':True,'round8_selector_call_name':name[1],
                    'round8_original_selector_slot':named_slot,'round8_rebuilt_selector_slot':emitted_slot,
                    'round8_selector_original_call_offset':call['offset'],'round8_selector_rebuilt_call_offset':other['offset'],
                    'round8_signature_relocation_exact':True,
                    'round8_selector_source_start':temporary.get('start'),'round8_selector_source_end':temporary.get('end')}
    return {}
