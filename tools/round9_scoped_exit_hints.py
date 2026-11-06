"""Fresh transfer/join evidence for opt-in smcs exits. Read-only; never an oracle."""
from compare_kairo import differences

def _u16(text):return len(text.encode('utf-16-le'))//2

def scoped_exit_hints(original,rebuilt,alignment,symbol_method,source_text,*,source_identity_confirmed=False):
 result={'round9_scoped_exit_valid':False,'round9_scoped_exit_evidence':{'source_only_patch_generation':True,'original_bytes_copied':False}}
 proof=result['round9_scoped_exit_evidence']
 def reject(reason):proof['reason']=reason;return result
 if not source_identity_confirmed:return reject('caller_source_identity_not_confirmed')
 if symbol_method is None:return reject('MDB_method_missing')
 try:body_delta=differences(rebuilt,symbol_method)[0]
 except (ValueError,KeyError,TypeError):return reject('MDB_body_oracle_UNKNOWN')
 if body_delta:return reject('MDB_debug_plus_body_not_exactly_current_debug_minus')
 proof['MDB_body_exact']=True
 left=next((i for i in original.get('instructions',[]) if i['offset']==alignment.get('original_offset')),None)
 right=next((i for i in rebuilt.get('instructions',[]) if i['offset']==alignment.get('rebuilt_offset')),None)
 if left is None or right is None:return reject('aligned_transfer_not_instruction')
 pair=left['opcode'],right['opcode'];proof.update(original_offset=left['offset'],rebuilt_offset=right['offset'],opcode_pair=list(pair))
 if pair not in (('ret','br'),('br','ret')):return reject('not_supported_exact_transfer_pair')
 points=[p for p in symbol_method.get('sequence_points',[]) if not p.get('hidden',False)]
 if not points:return reject('no_visible_MDB_points')
 result['round9_scoped_exit_original_opcode']=left['opcode'];mapping=right['offset'];joinpoint=-1
 if pair==('ret','br'):
  result['round9_scoped_exit_join_kind']='method-return'
  # A synthetic join has no source statement. Its point can already belong to
  # the next else-if; the preceding instruction is inside the true arm or its
  # condition when empty. The source adapter still requires a terminal if/else.
  at=next(n for n,i in enumerate(rebuilt['instructions']) if i['offset']==right['offset'])
  if at>0:mapping=rebuilt['instructions'][at-1]['offset']
 else:
  destination=left.get('operand',{}).get('branch');target=next((i for i in original['instructions'] if i['offset']==destination),None)
  if target is None:return reject('original_branch_target_missing')
  proof.update(original_target=destination,original_target_opcode=target['opcode'])
  if target['opcode']=='ret' and target==original['instructions'][-1]:result['round9_scoped_exit_join_kind']='method-ret'
  else:
   label=alignment.get('original_labels',{}).get(str(destination));rlook=[int(k) for k,v in alignment.get('rebuilt_labels',{}).items() if label is not None and v==label]
   if not isinstance(label,str) or not label.startswith('L') or len(rlook)!=1:return reject('branch_target_has_no_unique_proved_alignment_label')
   rtarget=next((i for i in rebuilt['instructions'] if i['offset']==rlook[0]),None)
   if rtarget is None or (target['opcode'],target.get('operand'))!=(rtarget['opcode'],rtarget.get('operand')):return reject('paired_target_instruction_not_exact_identity')
   seq=max((p for p in points if p['offset']<=rlook[0]),key=lambda p:p['offset'],default=None)
   if seq is None:return reject('paired_join_has_no_MDB_source_point')
   lines=source_text.splitlines(keepends=True);line=seq['start_line']-1
   if line<0 or line>=len(lines):return reject('join_source_line_out_of_bounds')
   row=lines[line];column=seq.get('start_column',-1);joinpoint=_u16(''.join(lines[:line]))+(column-1 if column and column>0 else _u16(row)-_u16(row.lstrip()))
   if not 0<=joinpoint<_u16(source_text):return reject('join_source_point_out_of_bounds')
   result['round9_scoped_exit_join_kind']='scope-next';proof.update(alignment_label=label,rebuilt_target=rlook[0],join_source_line=seq['start_line'],join_source_point_UTF16=joinpoint,lexical_next_statement_check_required=True)
 if not any(p['offset']<=mapping for p in points):return reject('transfer_anchor_has_no_MDB_point')
 result.update(round9_scoped_exit_valid=True,round9_scoped_exit_mapping_rebuilt_offset=mapping,round9_scoped_exit_join_source_point=joinpoint)
 proof.update(mapping_original_rebuilt_offset=right['offset'],mapping_selected_rebuilt_offset=mapping,caller_source_identity_confirmed=True,reason='exact_transfer_pair_and_required_join_evidence_proved')
 return result
