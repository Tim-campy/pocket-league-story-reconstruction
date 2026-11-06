"""Read-only zero-chain/slot evidence; source-model predictions never emit game IL."""
import argparse
import json
from pathlib import Path
from round7_slot_model import SlotModelClient
from round6_slot_solver import correspondences
from round7_smcs_hints import rule_hints
from round7_transform import TransformClient

ROOT=Path(__file__).resolve().parents[1]


def read(path): return json.loads(path.read_text(encoding='utf-8-sig'))


def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--run-label',default='probes')
    args=parser.parse_args()
    base=ROOT / 'build/round7/baseline/Assembly-CSharp'
    folder=ROOT / 'build/round7/runs' / args.run_label / 'engine/Assembly-CSharp'
    original={m['stable_id']:m for m in read(base / 'original-methods.json')['selected_methods']}
    rebuilt={m['stable_id']:m for m in read(folder / 'final-methods.json')['selected_methods']}
    old_types=read(base / 'plain-local-types.json')
    type_lookup={t:n for key,method in original.items() for t,n in zip(method.get('locals',[]),old_types[key])}
    rows=[]
    with SlotModelClient(folder / 'syntax-manifest.json') as client, TransformClient(folder / 'syntax-manifest.json') as syntax:
        for key,path in [('form.SubForm::_draw2(kairo.unity.ui.Graphics)->System.Void','form/SubForm.cs'),
                         ('system.form.FriendForm::<_update>m__17()->System.Void','system/form/FriendForm.cs')]:
            source=folder / 'sources/src/Assembly-CSharp' / path
            prediction=client.predict(source,source.read_text(encoding='utf-8-sig'),key)
            new_types=[type_lookup.get(t,'UNKNOWN') for t in rebuilt[key].get('locals',[])]
            slots,evidence=correspondences(original[key],rebuilt[key],old_types[key],new_types)
            hints=rule_hints(original[key],rebuilt[key],2 if '_draw2' in key else 18,old_types[key],new_types,prediction)
            response=syntax.request(source,source=source.read_text(encoding='utf-8-sig'),line=2784 if '_draw2' in key else 686,column=1,compiler='smcs',max_candidates=2048,**hints)
            candidates=[]
            for patch in response.get('patches',[]):
                if not patch['rule'].startswith(('extended-original-zero-chain','new-object-argument-local')):
                    continue
                raw=source.read_text(encoding='utf-8-sig').encode('utf-16-le')
                changed=(raw[:patch['start']*2]+patch['after'].encode('utf-16-le')+raw[patch['end']*2:]).decode('utf-16-le')
                proposed=client.predict(source,changed,key)
                candidates.append({'rule':patch['rule'],'complete':proposed.get('complete'),
                    'predicted_types':proposed.get('predicted_types'),'original_signature_equal':proposed.get('predicted_types')==old_types[key],
                    'unsupported_reasons':proposed.get('unsupported_reasons')})
            rows.append({'id':key,'model_complete':prediction.get('complete'),
                'unsupported_reasons':prediction.get('unsupported_reasons'),
                'predicted_types':prediction.get('predicted_types'),
                'original_types':old_types[key],
                'variables':[{**v,'desired_slot':slots.get(v.get('slot'))} for v in prediction.get('variables',[])],
                'slot_votes':evidence,'hints':hints,'source_only_candidate_predictions':candidates})
    output=ROOT / 'reports/round7-smcs-zerochain-diagnosis.json'
    output.write_text(json.dumps({'operation':'source-only-diagnostic','run_label':args.run_label,'methods':rows},indent=2)+'\n',encoding='utf-8')
    print(json.dumps([{'id':row['id'],'complete':row['model_complete'],'hints':row['hints'],
                      'candidate_predictions':row['source_only_candidate_predictions']} for row in rows],indent=2))


if __name__=='__main__': main()
