"""Validate source-only local predictions against frozen Roslyn bodies."""
import argparse
from collections import Counter
import csv
import json
from pathlib import Path
import sys
ROOT=Path(__file__).resolve().parents[1]
WORK=ROOT/'build/round10/roslyn-model-validation'
sys.path.insert(0,str(ROOT/"tools"))
from compare_kairo import sha
from round8_roslyn_model import SlotModelClient




def read(p): return json.loads(Path(p).read_text(encoding='utf-8-sig'))


def main():
    parser=argparse.ArgumentParser()
    parser.add_argument('--baseline',default='build/round10/baseline/KairoLibrary')
    parser.add_argument('--output',default='reports/round10-roslyn-model-validation.json')
    args=parser.parse_args()
    base=ROOT/args.baseline
    receipt=read(base/'verification.json')
    manifest=read(base/'build-manifest.json')
    actual=read(base/'plain-local-types-rebuilt.json')
    original=read(base/'plain-local-types.json')
    methods={v['stable_id']:v for v in receipt['methods'].values() if v.get('body_present')}
    WORK.mkdir(parents=True,exist_ok=True)
    inputs=dict(manifest,sources=[dict(row,input_path=str((base/row['path']).resolve())) for row in manifest['sources']])
    (WORK/'baseline-manifest.json').write_text(json.dumps(inputs,indent=2)+'\n',encoding='utf-8')
    predictions={}
    with SlotModelClient(WORK/'baseline-manifest.json') as client:
        for source in manifest['sources']:
            logical=source['path']
            frozen=base/logical
            text=(frozen if frozen.exists() else ROOT/logical).read_text(encoding='utf-8-sig')
            for record in client.predict_all(ROOT/logical,text):
                record['source']=logical
                predictions[record['method_id']]=record
        # A generated MoveNext is credited only when its source matches the
        # narrow independent foreach-array/yield model, never for an empty list.
        for identity in methods:
            if '/<GetEnumerator>d__' not in identity or '::MoveNext()' not in identity:
                continue
            parent=identity.split('/<GetEnumerator>d__',1)[0]
            source_method=next((p for k,p in predictions.items() if k.startswith(parent+'::GetEnumerator(')),None)
            if source_method is None:
                continue
            logical=source_method['source']
            frozen=base/logical
            text=(frozen if frozen.exists() else ROOT/logical).read_text(encoding='utf-8-sig')
            record=client.predict(ROOT/logical,text,identity)
            if record['complete']:
                record['source']=logical
                predictions[identity]=record
    rows=[]
    for identity,proof in methods.items():
        record=predictions.get(identity)
        mapped=record is not None
        if record is None:
            record={'method_id':identity,'complete':False,'unsupported_reasons':['generated_or_unmapped_source_method'],
                    'predicted_types':[], 'variables':[], 'blocks':[]}
        expected=actual[identity]
        equal=mapped and record['predicted_types']==expected
        reasons=record['unsupported_reasons']
        if not equal and not reasons:
            if len(record['predicted_types'])<len(expected):reasons=['unmodeled_emit_temporary_or_capture']
            elif len(record['predicted_types'])>len(expected):reasons=['eliminated_unreachable_or_captured_local']
            else:reasons=['type_or_scope_order']
        rows.append(dict(record,actual_types=expected,original_types=original[identity],
            prediction_equal=equal,validation_classification=reasons,
            body_equal=proof['body_equal'],state=proof['state'],
            original_signature_equal=original[identity]==expected))
    matched=[r for r in rows if r['state']=='matched']
    approved=[r for r in matched if r['complete']]
    mapped=[r for r in matched if 'generated_or_unmapped_source_method' not in r['unsupported_reasons']]
    result={'operation':'source-only-roslyn-slot-model-validation','candidate_compilations':0,
        'source_sha256':sha(ROOT/'tools/Round8RoslynModel.cs'),'client_sha256':sha(ROOT/'tools/round8_roslyn_model.py'),
        'baseline_receipt_sha256':sha(base/'verification.json'),'matched_bodies':len(matched),
        'matched_exact_predictions':sum(r['prediction_equal'] for r in matched),
        'matched_model_complete':len(approved),'matched_complete_exact':sum(r['prediction_equal'] for r in approved),
        'matched_source_mapped':len(mapped),'matched_source_unmapped':len(matched)-len(mapped),
        'matched_mapped_incomplete':sum(not r['complete'] for r in mapped),
        'matched_mapped_incomplete_equal':sum(not r['complete'] and r['prediction_equal'] for r in mapped),
        'matched_complete_mismatches':sum(not r['prediction_equal'] for r in approved),
        'matched_incomplete_classifications':dict(Counter(c for r in matched if not r['complete'] for c in r['unsupported_reasons'])),
        'matched_error_classifications':dict(Counter(c for r in matched if not r['prediction_equal'] for c in r['validation_classification'])),
        'pending_bodies':sum(not r['body_equal'] for r in rows),
        'pending_original_signature_equal':sum(not r['body_equal'] and r['original_signature_equal'] for r in rows),
        'methods':rows}
    out=ROOT/args.output
    out.write_text(json.dumps(result,indent=2,ensure_ascii=True)+'\n',encoding='utf-8',newline='\n')
    print(json.dumps({k:v for k,v in result.items() if k!='methods'}))
    assert result['matched_complete_mismatches']==0, 'Incorrect complete prediction'


if __name__=='__main__': main()
