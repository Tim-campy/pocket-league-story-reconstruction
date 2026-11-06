"""Read-only candidate and local-model preflight; NO game compilation."""
import sys,json,collections,hashlib,argparse
from pathlib import Path
SOURCE_DIR=Path(__file__).resolve().parent;ROOT=next(p for p in SOURCE_DIR.parents if (p/'AGENTS.md').is_file());HERE=ROOT/'build/round9/d-scoped-exit-calculated';HERE.mkdir(parents=True,exist_ok=True);sys.path.insert(0,str(ROOT/'tools'))
from test_round9_scoped_exit import Client,apply
from round7_slot_model import SlotModelClient

def u16(text):return len(text.encode('utf-16-le'))//2
def point(source,seq):
 lines=source.splitlines(keepends=True);line=seq['start_line']-1
 if line<0 or line>=len(lines):return None
 head=''.join(lines[:line]);row=lines[line];column=seq['start_column']
 return u16(head)+(column-1 if column and column>0 else u16(row)-u16(row.lstrip()))
def apply16(source,p):
 data=source.encode('utf-16-le');assert data[p['start']*2:p['end']*2].decode('utf-16-le')==p['before'];return (data[:p['start']*2]+p['after'].encode('utf-16-le')+data[p['end']*2:]).decode('utf-16-le')
def write_bundles(report,paths,sources):
 dependencies={('tools/'+name):hashlib.sha256((SOURCE_DIR/name).read_bytes()).hexdigest() for name in ('Round9ScopedExitCandidates.cs','Round9ScopedExitFixtureDriver.cs','test_round9_scoped_exit.py','round9_scoped_exit_calculate.py','Round9ScopedExitAdapter.cs','round9_scoped_exit_hints.py')}
 modelhash=hashlib.sha256((ROOT/'tools/Round7SlotModel.cs').read_bytes()).hexdigest();clienthash=hashlib.sha256((ROOT/'tools/round7_slot_model.py').read_bytes()).hexdigest()
 for family in report['families']:
  patches=[]
  for row in report['methods']:
   if row['family']!=family:continue
   source=paths[row['logical_path']].read_bytes().decode('utf-8-sig');raw=source.encode('utf-16-le')
   for evidence in row['candidates']:
    if not evidence['model_approved_original_signature']:continue
    p=evidence['patch'];start=p['declaration_start'];end=p['declaration_end'];before=raw[start*2:end*2].decode('utf-16-le');delta=p['start']-start;stop=p['end']-start;dr=before.encode('utf-16-le');after=(dr[:delta*2]+p['after'].encode('utf-16-le')+dr[stop*2:]).decode('utf-16-le')
    assert source.encode('utf-16-le').count(before.encode('utf-16-le'))==1
    patches.append({'method_id':row['id'],'logical_path':row['logical_path'],'rule':family,'start':start,'end':end,'before':before,'after':after,'distance':0,'baseline_source_sha256':row['baseline_source_sha256'],'model_source_sha256':modelhash,'client_sha256':clienthash,'original_signature_model_complete':True,'predicted_types':evidence['predicted_types'],'expected_types':evidence['expected_types'],'original_transfer_offset':row['original_offset'],'original_target_proof':row['original_target_proof'],'source_only':True})
  bundle={'schema':'round9-scoped-exit-proposals-v1','family':family,'calculator_sha256':dependencies['tools/round9_scoped_exit_calculate.py'],'syntax_source_sha256':dependencies['tools/Round9ScopedExitCandidates.cs'],'dependency_sha256':dependencies,'patches':patches,'game_compilations':0,'fresh_model_before_compilation_required':True,'full_verifier_required':True,'source_only':True,'requires_published_tools_binding':True}
  path=HERE/(family+'-patches.json');path.write_text(json.dumps(bundle,indent=2),encoding='utf-8')
  report['families'][family]['calculated_bundle']=str(path.relative_to(ROOT));report['families'][family]['calculated_bundle_sha256']=hashlib.sha256(path.read_bytes()).hexdigest()

def main():
 parser=argparse.ArgumentParser(description=__doc__);parser.add_argument('--snapshot',type=Path,default=ROOT/'build/round9/cycles/b10/after/Assembly-CSharp');parser.add_argument('--measurement',type=Path,default=ROOT/'reports/round9-b10-after-measurement.json');parser.add_argument('--symbols',type=Path,default=ROOT/'build/round9/runs/control-mapping-b10/engine/Assembly-CSharp/mapping-symbols.json');args=parser.parse_args();snapshot=args.snapshot.resolve();manifest=json.loads((snapshot/'build-manifest.json').read_text(encoding='utf-8-sig'));paths={s['path']:(snapshot/'src'/Path(s['path']).relative_to('src')).resolve() for s in manifest['sources']}
 manifest['sources']=[{'path':str(p),'input_path':str(p)} for p in paths.values()];mp=HERE/'model-manifest.json';mp.write_text(json.dumps(manifest),encoding='utf-8')
 expected=json.loads((snapshot/'plain-local-types.json').read_text(encoding='utf-8-sig'))
 measured=json.loads(args.measurement.read_text(encoding='utf-8-sig'))['assemblies']['Assembly-CSharp'];sources={m['stable_id']:m['source'] for m in measured['methods']}
 originals={m['stable_id']:m for m in json.loads((snapshot/'original-methods.json').read_text(encoding='utf-8-sig'))['selected_methods']}
 symbols={m['stable_id']:m for m in json.loads(args.symbols.read_text(encoding='utf-8-sig'))['methods']}
 helper=Client();rows=[];model=SlotModelClient(mp)
 try:
  for diff in measured['alignment']['methods']:
   if diff['opcode_pair'] not in ('br => ret','ret => br') or 'SubForm::_draw4' in diff['stable_id']:continue
   mid=diff['stable_id'];source=paths[sources[mid]].read_bytes().decode('utf-8-sig');sym=symbols[mid];sp=max((s for s in sym['sequence_points'] if not s['hidden'] and s['offset']<=diff['rebuilt_offset']),key=lambda s:s['offset'],default=None)
   if sp is None:continue
   sourcepoint=point(source,sp);family='terminal-if-arm-return-flatten-else' if diff['opcode_pair']=='ret => br' else 'scoped-terminal-arm-common-join';opcode=diff['original_instruction']['opcode'];join='method-return' if opcode=='ret' else 'method-ret';joinpoint=-1;proof={}
   if opcode=='br':
    dest=diff['original_instruction']['operand']['branch'];target=next(i for i in originals[mid]['instructions'] if i['offset']==dest);proof={'original_target':dest,'original_target_opcode':target['opcode']}
    if target['opcode']!='ret':
     join='scope-next';label=diff['original_labels'].get(str(dest));rlook=[int(k) for k,v in diff['rebuilt_labels'].items() if label is not None and v==label]
     if len(rlook)==1:
      jp=max((s for s in sym['sequence_points'] if not s['hidden'] and s['offset']<=rlook[0]),key=lambda s:s['offset'],default=None)
      if jp is not None:joinpoint=point(source,jp);proof.update(rebuilt_target=rlook[0],label=label,join_source_line=jp['start_line'])
   patches=helper.generate(source,sourcepoint,family,opcode,join,joinpoint);predictions=[]
   for p in patches:
    candidate=apply16(source,p);prediction=model.predict(path=paths[sources[mid]],source=candidate,method_id=mid);ok=prediction['complete'] and prediction['predicted_types']==expected[mid]
    predictions.append({'patch':p,'candidate_source_sha256':hashlib.sha256(candidate.encode()).hexdigest(),'model_complete':prediction['complete'],'expected_types':expected[mid],'predicted_types':prediction['predicted_types'],'model_reasons':prediction.get('unsupported_reasons',[]),'model_approved_original_signature':ok})
   rows.append({'id':mid,'family':family,'original_IL_bytes':diff['original_il_bytes'],'original_offset':diff['original_offset'],'rebuilt_offset':diff['rebuilt_offset'],'source_line':sp['start_line'],'source_point':sourcepoint,'join_kind':join,'join_source_point':joinpoint,'original_target_proof':proof,'logical_path':sources[mid],'source_path':str(paths[sources[mid]]),'baseline_source_sha256':hashlib.sha256(paths[sources[mid]].read_bytes()).hexdigest(),'candidate_count':len(patches),'model_approved_candidate_count':sum(p['model_approved_original_signature'] for p in predictions),'candidates':predictions})
 finally:helper.close();model.close()
 totals={}
 for fam in set(r['family'] for r in rows):
  eligible=[r for r in rows if r['family']==fam];ok=[r for r in eligible if r['model_approved_candidate_count']]
  totals[fam]={'methods_considered':len(eligible),'IL_bytes_considered':sum(r['original_IL_bytes'] for r in eligible),'methods_with_candidates':sum(bool(r['candidate_count']) for r in eligible),'complete_model_approved_methods':len(ok),'complete_model_approved_IL_bytes':sum(r['original_IL_bytes'] for r in ok)}
 report={'snapshot':str(snapshot.relative_to(ROOT)),'source_only':True,'game_compilations':0,'families':totals,'methods':rows,'helper_sha256':hashlib.sha256((SOURCE_DIR/'Round9ScopedExitCandidates.cs').read_bytes()).hexdigest()}
 write_bundles(report,paths,sources)
 (HERE/'source-model-preflight.json').write_text(json.dumps(report,indent=2),encoding='utf-8');print(json.dumps(totals,indent=2))
 for r in rows:print(r['candidate_count'],r['model_approved_candidate_count'],r['id'],r['join_kind'],r['original_target_proof'])
if __name__=='__main__':main()
