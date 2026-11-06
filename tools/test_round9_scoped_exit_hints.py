"""Read-only fresh-MDB hint controls. No game compilation."""
import copy,json,sys,hashlib
from pathlib import Path
SOURCE_DIR=Path(__file__).resolve().parent;ROOT=next(p for p in SOURCE_DIR.parents if (p/'AGENTS.md').is_file());sys.path.insert(0,str(ROOT/'tools'))
from round9_scoped_exit_hints import scoped_exit_hints
from test_round9_scoped_exit import Client
HERE=ROOT/'build/round9/d-scoped-exit-calculated'
def main():
 snapshot=ROOT/'build/round9/cycles/b10/after/Assembly-CSharp';measurement=json.loads((ROOT/'reports/round9-b10-after-measurement.json').read_text(encoding='utf-8-sig'))['assemblies']['Assembly-CSharp'];original={m['stable_id']:m for m in json.loads((snapshot/'original-methods.json').read_text(encoding='utf-8-sig'))['selected_methods']};rebuilt={m['stable_id']:m for m in json.loads((snapshot/'rebuilt-methods.json').read_text(encoding='utf-8-sig'))['selected_methods']};symbols={m['stable_id']:m for m in json.loads((ROOT/'build/round9/runs/control-mapping-b10/engine/Assembly-CSharp/mapping-symbols.json').read_text(encoding='utf-8-sig'))['methods']};sources={m['stable_id']:m['source'] for m in measurement['methods']}
 client=Client();rows=[]
 try:
  for a in measurement['alignment']['methods']:
   if a['opcode_pair'] not in ('br => ret','ret => br'):continue
   mid=a['stable_id'];path=snapshot/'src'/Path(sources[mid]).relative_to('src');text=path.read_bytes().decode('utf-8-sig');h=scoped_exit_hints(original[mid],rebuilt[mid],a,symbols[mid],text,source_identity_confirmed=True);assert h['round9_scoped_exit_valid'],(mid,h)
   seq=max((p for p in symbols[mid]['sequence_points'] if not p.get('hidden') and p['offset']<=h['round9_scoped_exit_mapping_rebuilt_offset']),key=lambda p:p['offset']);lines=text.splitlines(keepends=True);line=lines[seq['start_line']-1];point=len((''.join(lines[:seq['start_line']-1])+line[:len(line)-len(line.lstrip())]).encode('utf-16-le'))//2;family='terminal-if-arm-return-flatten-else' if a['opcode_pair']=='ret => br' else 'scoped-terminal-arm-common-join';patches=client.generate(text,point,family,h['round9_scoped_exit_original_opcode'],h['round9_scoped_exit_join_kind'],h['round9_scoped_exit_join_source_point']);
   if 'SubForm::_draw4' in mid:assert not patches,'v7-covered simple empty-arm method-exit must not be repeated'
   else:assert patches,(mid,seq)
   rows.append({'id':mid,'hints':h,'mapped_source_line':seq['start_line'],'candidate_count':len(patches)})
  first=next(a for a in measurement['alignment']['methods'] if a['opcode_pair']=='ret => br');mid=first['stable_id'];text=(snapshot/'src'/Path(sources[mid]).relative_to('src')).read_bytes().decode('utf-8-sig');negatives=[]
  assert not scoped_exit_hints(original[mid],rebuilt[mid],first,symbols[mid],text)['round9_scoped_exit_valid'];negatives.append('source_identity_unconfirmed')
  mutated=copy.deepcopy(symbols[mid]);mutated['instructions'][-1]['opcode']='nop';assert not scoped_exit_hints(original[mid],rebuilt[mid],first,mutated,text,source_identity_confirmed=True)['round9_scoped_exit_valid'];negatives.append('MDB_body_changed')
  without=copy.deepcopy(symbols[mid]);without['sequence_points']=[];assert not scoped_exit_hints(original[mid],rebuilt[mid],first,without,text,source_identity_confirmed=True)['round9_scoped_exit_valid'];negatives.append('MDB_points_missing')
  branch=next(a for a in measurement['alignment']['methods'] if 'Team::Update_coatEvaNewDeffence' in a['stable_id']);mid=branch['stable_id'];text=(snapshot/'src'/Path(sources[mid]).relative_to('src')).read_bytes().decode('utf-8-sig');bad=copy.deepcopy(branch);bad['original_labels']={};assert not scoped_exit_hints(original[mid],rebuilt[mid],bad,symbols[mid],text,source_identity_confirmed=True)['round9_scoped_exit_valid'];negatives.append('join_label_unproved')
  report={'operation':'fresh-read-only-MDB-transfer-hint-controls','game_compilations':0,'source_identity_required':True,'positive_hint_pairs':len(rows),'new_family_candidate_methods':sum(bool(r['candidate_count']) for r in rows),'negatives':negatives,'methods':rows,'hints_sha256':hashlib.sha256((SOURCE_DIR/'round9_scoped_exit_hints.py').read_bytes()).hexdigest()};(HERE/'fresh-hints-report.json').write_text(json.dumps(report,indent=2),encoding='utf-8');print(len(rows),'fresh hint pairs,',len(negatives),'negative refusals')
 finally:client.close()
if __name__=='__main__':main()
