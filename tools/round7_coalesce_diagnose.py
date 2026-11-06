"""Read-only many-to-one local-slot evidence; never compiles game candidates."""
from collections import Counter, defaultdict
import json
from pathlib import Path
from compare_kairo import sha
from round7_slot_model import SlotModelClient
from round7_slot_solver import correspondences, lifetimes, proposals
from round6_alignment import align_methods

ROOT=Path(__file__).resolve().parents[1]
BASE=ROOT/'build/round7/cycles/cycle02/baseline/Assembly-CSharp'
TARGETS=[('form.SubForm::_draw3(kairo.unity.ui.Graphics)->System.Void','src/Assembly-CSharp/form/SubForm.cs'),
         ('form.SubForm::_draw5(kairo.unity.ui.Graphics)->System.Void','src/Assembly-CSharp/form/SubForm.cs'),
         ('game.UserData::Draw_edit2(kairo.unity.ui.Graphics,System.Boolean)->System.Void','src/Assembly-CSharp/game/UserData.cs')]


def read(path):return json.loads(Path(path).read_text(encoding='utf-8-sig'))


def main():
    folder=ROOT/'build/round7/slot-coalesce'
    folder.mkdir(parents=True,exist_ok=True)
    manifest=read(BASE/'build-manifest.json')
    for source in manifest['sources']:source['input_path']=str(BASE/source['path'])
    mp=folder/'manifest.json';mp.write_text(json.dumps(manifest),encoding='utf-8')
    original={m['stable_id']:m for m in read(BASE/'original-methods.json')['selected_methods']}
    rebuilt={m['stable_id']:m for m in read(BASE/'rebuilt-methods.json')['selected_methods']}
    original_types,rebuilt_types=read(BASE/'plain-local-types.json'),read(BASE/'plain-local-types-rebuilt.json')
    rows=[]
    with SlotModelClient(mp) as client:
        for identity,logical in TARGETS:
            source=(BASE/logical).read_text(encoding='utf-8-sig')
            model=client.predict(ROOT/logical,source,identity)
            mapping,votes=correspondences(original[identity],rebuilt[identity],original_types[identity],rebuilt_types[identity])
            grouped=defaultdict(list)
            for slot,wanted in mapping.items():grouped[wanted].append(slot)
            groups=[{'original_slot':wanted,'type':original_types[identity][wanted],
                     'rebuilt_slots':slots,'variables':[v for v in model['variables'] if v['slot'] in slots],
                     'aligned_use_evidence':[v for v in votes if v['rebuilt_slot'] in slots]}
                    for wanted,slots in grouped.items() if len(slots)>1]
            solved=proposals(source,model,original[identity],rebuilt[identity],original_types[identity],rebuilt_types[identity])
            approved=[]
            candidate_rows=[]
            for patch in solved['patches']:
                raw=source.encode('utf-16-le')
                candidate=(raw[:2*patch['start']]+patch['after'].encode('utf-16-le')+raw[2*patch['end']:]).decode('utf-16-le')
                predicted=client.predict(ROOT/logical,candidate,identity)
                equal=predicted.get('complete') and predicted['predicted_types']==original_types[identity]
                if equal:approved.append(patch['rule'])
                candidate_rows.append({'rule':patch['rule'],'complete':predicted.get('complete'),
                    'signature_equal':equal,'predicted_count':len(predicted.get('predicted_types',[]))})
            start,end=model['declaration_start'],model['declaration_end']
            method_source=source.encode('utf-16-le')[start*2:end*2].decode('utf-16-le')
            alignment=align_methods(original[identity],rebuilt[identity])
            row={'method_id':identity,'source':logical,'source_sha256':sha(BASE/logical),
                 'original_il_bytes':original[identity]['header']['code_size'],
                 'original_types':original_types[identity],'rebuilt_types':rebuilt_types[identity],
                 'model_complete':model['complete'],'model_predicts_rebuilt':model['predicted_types']==rebuilt_types[identity],
                 'model':model,'source_method':method_source,'source_method_line':source[:len(source.encode('utf-16-le')[:start*2].decode('utf-16-le'))].count('\n')+1,
                 'alignment':alignment,'slot_correspondence':votes,'many_to_one_groups':groups,
                 'original_lifetimes':lifetimes(original[identity]),'rebuilt_lifetimes':lifetimes(rebuilt[identity]),
                 'existing_solver_constraints':solved['constraints'],'existing_solver_candidates':candidate_rows,
                 'existing_solver_model_approved_rules':approved,
                 'original_first_instructions':original[identity]['instructions'][:40],
                 'rebuilt_first_instructions':rebuilt[identity]['instructions'][:40]}
            rows.append(row)
            print(json.dumps({k:row[k] for k in ['method_id','original_il_bytes','model_complete','model_predicts_rebuilt','existing_solver_constraints','existing_solver_model_approved_rules']}))
            print(json.dumps({'original_count':len(row['original_types']),'rebuilt_count':len(row['rebuilt_types']),
                              'many_to_one':[(g['original_slot'],g['rebuilt_slots'],[(v['name'],v['slot']) for v in g['variables']]) for g in groups]}))
    result={'operation':'read-only-many-to-one-slot-diagnosis','candidate_compilations':0,
            'baseline':str(BASE.relative_to(ROOT)),'baseline_manifest_sha256':sha(BASE/'build-manifest.json'),
            'methods':rows}
    (ROOT/'reports/round7-coalesce-diagnosis.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8',newline='\n')


if __name__=='__main__':main()
