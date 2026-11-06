"""Bounded IL proof of two arm stores; not a CFG/AST/control-tree extractor."""
def stored_local(instruction):
 op=instruction['opcode'];operand=instruction.get('operand')
 if op in ['stloc','stloc.s'] and isinstance(operand,dict):return operand.get('local')
 if op.startswith('stloc.') and op[-1:] in ['0','1','2','3']:return int(op[-1])
 return None

def branch_target(instruction):
 operand=instruction.get('operand')
 return operand.get('branch') if isinstance(operand,dict) else None

def per_arm_store_evidence(method,first_store_offset):
 instructions=method.get('instructions',[]);index={n['offset']:i for i,n in enumerate(instructions)}
 pos=index.get(first_store_offset)
 if pos is None or pos+1>=len(instructions):return {}
 slot=stored_local(instructions[pos]);jump=instructions[pos+1]
 if slot is None or jump['opcode'] not in ['br','br.s']:return {}
 join=jump.get('operand',{}).get('branch');end=index.get(join)
 if end is None or end<=pos+2 or stored_local(instructions[end-1])!=slot:return {}
 branches=[i for i in range(max(0,pos-32),pos) if instructions[i]['opcode'] not in ['br','br.s'] and branch_target(instructions[i]) in index
  and pos+1<index[instructions[i]['operand']['branch']]<end]
 if not branches:return {}
 condition=branches[-1];second=index[instructions[condition]['operand']['branch']]
 # One straight-line value expression in each arm; no hidden edges/returns/stores.
 for start,stop in [(condition+1,pos),(second,end-1)]:
  for instruction in instructions[start:stop]:
   if branch_target(instruction) is not None or instruction['opcode'] in ['ret','throw','switch','leave','leave.s']:return {}
   if stored_local(instruction) is not None:return {}
 return dict(round9_original_same_slot_arm_stores=True,round9_per_arm_store_slot=slot,
  evidence=dict(condition_branch=instructions[condition],first_value=instructions[condition+1:pos],first_store=instructions[pos],
    first_arm_join_jump=jump,second_value=instructions[second:end-1],second_store=instructions[end-1],join=instructions[end],
    same_slot=True,straight_line_arm_values=True,source_orientation_not_inferred=True))

def per_arm_local_store_hints(original,rebuilt,original_offset,rebuilt_offset):
 """Only admit the existing ternary's unchanged value sequences and common store.

 This is a bounded pattern proof, not source recovery. Roslyn EmitConditionalOperator
 emits the Alternative first; the helper still verifies an assignment ternary to
 a plain local. Neither branch polarity nor a raw offset alone sets orientation.
 """
 hint=per_arm_store_evidence(original,original_offset)
 if not hint:return {}
 evidence=hint['evidence'];instructions=rebuilt.get('instructions',[])
 indexes={n['offset']:i for i,n in enumerate(instructions)};position=indexes.get(rebuilt_offset)
 if position is None or instructions[position]['opcode'] not in ['br','br.s']:return {}
 join=indexes.get(branch_target(instructions[position]));slot=hint['round9_per_arm_store_slot']
 if join is None or join<=position or stored_local(instructions[join])!=slot:return {}
 first=evidence['first_value'];second=evidence['second_value']
 token=lambda n:(n['opcode'],n.get('operand'))
 if position<len(first) or [token(n) for n in instructions[position-len(first):position]]!=[token(n) for n in first]:return {}
 second_start=join-len(second)
 if second_start<=position or [token(n) for n in instructions[second_start:join]]!=[token(n) for n in second]:return {}
 first_start=position-len(first)
 if first_start<1:return {}
 condition=instructions[first_start-1]
 if condition['opcode']!=evidence['condition_branch']['opcode'] or branch_target(condition)!=instructions[second_start]['offset']:return {}
 hint.update(round9_per_arm_store_order='false-first',
  round9_per_arm_store_rebuilt_evidence=dict(first_value=instructions[first_start:position],second_value=instructions[second_start:join],
   common_store=instructions[join],condition_branch=condition,common_store_slot=slot,
   original_arm_values_identical_to_current_ternary=True,orientation_basis='Roslyn EmitConditionalOperator emits Alternative first; unchanged value anchors'))
 return hint
