"""Narrow value-vs-branch IL shapes. Original bytes are diagnostic only."""
def boolean_context_hints(original,rebuilt,original_offset,rebuilt_offset):
 left=original.get('instructions',[]);right=rebuilt.get('instructions',[])
 l=next((i for i,x in enumerate(left) if x['offset']==original_offset),None);r=next((i for i,x in enumerate(right) if x['offset']==rebuilt_offset),None)
 if l is None or r is None:return {}
 if l+4<len(left) and left[l]['opcode']=='ldc.i4.0' and left[l+1]['opcode']=='ceq' and left[l+2]['opcode'] in ('br','br.s') and left[l+3]['opcode']=='ldc.i4.0' and left[l+4]['opcode'] in ('brfalse','brfalse.s') and left[l+2].get('operand',{}).get('branch')==left[l+4]['offset'] and right[r]['opcode'] in ('brtrue','brtrue.s'):
  return {'round9_boolean_context_mode':'materialize-tail','round9_boolean_context_evidence':{'original':left[l:l+5],'rebuilt':right[r:r+2],'no_named_local':True}}
 if left[l]['opcode'] in ('brfalse','brfalse.s','brtrue','brtrue.s') and r+2<len(right) and right[r]['opcode']=='ldc.i4.0' and right[r+1]['opcode'] in ('cgt.un','ceq') and right[r+2]['opcode'] in ('brfalse','brfalse.s','brtrue','brtrue.s'):
  return {'round9_boolean_context_mode':'inline-adjacent-alias','round9_boolean_context_evidence':{'original':left[l:l+2],'rebuilt':right[r:r+3],'source_adjacent_single_use_required':True}}
 return {}
