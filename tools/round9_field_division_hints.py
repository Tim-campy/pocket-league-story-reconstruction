"""Recognize a repeated original receiver-load sequence versus rebuilt dup."""
def field_division_hints(original,rebuilt,original_offset,rebuilt_offset):
 left=original.get('instructions',[]);right=rebuilt.get('instructions',[])
 l=next((i for i,x in enumerate(left) if x['offset']==original_offset),None);r=next((i for i,x in enumerate(right) if x['offset']==rebuilt_offset),None)
 if l is None or r is None or right[r]['opcode']!='dup':return {}
 def signature(items):return [(x['opcode'],x.get('operand')) for x in items]
 def allowed(x):return x['opcode'].startswith(('ldarg','ldloc')) or x['opcode'] in ('ldfld','ldsfld')
 for count in range(1,5):
  if l<count or r<count or l+count>=len(left):continue
  prior=left[l-count:l];again=left[l:l+count]
  if not all(allowed(x) for x in prior) or signature(prior)!=signature(again) or signature(prior)!=signature(right[r-count:r]):continue
  if left[l+count]['opcode']!='ldfld':continue
  tail=left[l+count+1:l+count+8]
  if not any(x['opcode'] in ('div','div.un') for x in tail):continue
  return {'round9_division_repeated_receiver':True,'round9_division_evidence':{
   'original_receiver':prior,'original_repeated_receiver':again,'original_field':left[l+count],
   'original_following':tail,'rebuilt_receiver_dup':right[r-count:r+1],'diagnostic_only':True}}
 return {}
