"""Exact frontier evidence: newarr; stloc; ldloc; ldc0 versus newarr; dup.

Only diagnostics/source proposal permission. The independent model and full
body verifier remain mandatory; no bytes are copied to the compiled result.
"""
from round6_slot_solver import local_access

def array_population_hints(original, rebuilt, original_offset, rebuilt_offset):
    left=original.get('instructions',[]);right=rebuilt.get('instructions',[])
    l=next((i for i,x in enumerate(left) if x['offset']==original_offset),None)
    r=next((i for i,x in enumerate(right) if x['offset']==rebuilt_offset),None)
    if l is None or r is None or l==0 or r==0 or l+2>=len(left):return {}
    store=local_access(left[l]);load=local_access(left[l+1])
    if not store or store[0]!='stloc' or load!=('ldloc',store[1]):return {}
    if left[l+2]['opcode']!='ldc.i4.0' or right[r]['opcode']!='dup':return {}
    if left[l-1]['opcode']!='newarr' or right[r-1]['opcode']!='newarr':return {}
    if left[l-1].get('operand')!=right[r-1].get('operand'):return {}
    return {'round9_array_population_valid':True,
        'round9_array_population_slot':store[1],
        'round9_array_population_evidence':{'original_newarr':left[l-1],
            'original_store_load_index':left[l:l+3], 'rebuilt_newarr_dup':right[r-1:r+1],
            'source_only':True,'body_oracle_required':True}}
