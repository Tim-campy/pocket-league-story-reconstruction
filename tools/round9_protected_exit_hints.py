"""Narrow EH instruction evidence; lexical transfer ownership is checked in source."""
def protected_exit_hints(original,rebuilt,original_offset,rebuilt_offset):
 left=original.get('instructions',[]);right=rebuilt.get('instructions',[])
 l=next((i for i,x in enumerate(left) if x['offset']==original_offset),None);r=next((i for i,x in enumerate(right) if x['offset']==rebuilt_offset),None)
 if l is None or r is None:return {}
 oe=original.get('exception_regions',[]);re=rebuilt.get('exception_regions',[])
 if not oe or not re:return {}
 mode=None
 if l+2<len(left) and r+1<len(right) and left[l]['opcode'] in ('leave','leave.s') and left[l+1]['opcode'] in ('leave','leave.s') and right[r]['opcode'] in ('leave','leave.s') and right[r+1]['opcode'] not in ('leave','leave.s'):
  if any(e['kind']=='Catch' and e['handler_start']==left[l+2]['offset'] for e in oe) and any(e['kind']=='Catch' and e['handler_start']==right[r+1]['offset'] for e in re):mode='loop-try-break'
 if l>0 and r>0 and r+1<len(right) and left[l-1]['opcode'] in ('leave','leave.s') and right[r-1]['opcode'] in ('leave','leave.s') and right[r]['opcode'] in ('leave','leave.s') and (left[l]['opcode'],left[l].get('operand'))==(right[r+1]['opcode'],right[r+1].get('operand')):
  if any(e['kind']=='Catch' and e['handler_start']==left[l]['offset'] for e in oe) and any(e['kind']=='Catch' and e['handler_start']==right[r+1]['offset'] for e in re):mode='guard-try-returns'
 # The sequence aligner can associate the automatic original leave with
 # either member of the reconstructed pair. EH boundaries disambiguate the
 # same insertion without changing the comparison or deleting an edge.
 if l+1<len(left) and r+2<len(right) and left[l]['opcode'] in ('leave','leave.s') and right[r]['opcode'] in ('leave','leave.s') and right[r+1]['opcode'] in ('leave','leave.s') and left[l+1]['opcode'] not in ('leave','leave.s') and (left[l+1]['opcode'],left[l+1].get('operand'))==(right[r+2]['opcode'],right[r+2].get('operand')):
  if any(e['kind']=='Catch' and e['handler_start']==left[l+1]['offset'] for e in oe) and any(e['kind']=='Catch' and e['handler_start']==right[r+2]['offset'] for e in re):mode='guard-try-returns'
 if not mode:return {}
 return {'round9_protected_exit_mode':mode,'round9_protected_exit_evidence':{'original_context':left[max(0,l-1):l+3],'rebuilt_context':right[max(0,r-1):r+3],'original_exception_regions':oe,'rebuilt_exception_regions':re,'lexical_source_owner_check_required':True,'tree_EH_completeness_not_assumed':True}}
