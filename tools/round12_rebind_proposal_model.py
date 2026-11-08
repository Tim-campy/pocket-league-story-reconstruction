"""Rebind a source proposal only after a real prediction by the live model.

Preserves the old bundle and model evidence. Emits no target, IL or ledger state.
"""
import argparse
import copy
import importlib
import json
from pathlib import Path
from compare_kairo import sha

ROOT=Path(__file__).resolve().parents[1]

def read(path):return json.loads(Path(path).read_text(encoding='utf-8-sig'))

def main():
    p=argparse.ArgumentParser(description=__doc__)
    p.add_argument('--bundle',required=True)
    p.add_argument('--bundle-sha256',required=True)
    p.add_argument('--baseline',required=True)
    p.add_argument('--assembly',required=True,choices=['Assembly-CSharp','KairoLibrary'])
    p.add_argument('--output',required=True)
    p.add_argument('--proof',required=True)
    p.add_argument('--allow-genuine-incomplete',action='store_true',help='Keep a real incomplete prediction and require the ordinary emitted-signature oracle later')
    a=p.parse_args()
    old_path=ROOT/a.bundle
    assert sha(old_path)==a.bundle_sha256
    original=read(old_path);result=copy.deepcopy(original)
    cfg=read(ROOT/'config/round12-protocol.json')['models'][a.assembly]
    source=ROOT/cfg['source'];client=ROOT/cfg['client'];validation=ROOT/cfg['validation']
    validated=read(validation)
    assert validated['source_sha256']==sha(source) and validated['client_sha256']==sha(client)
    assert validated['matched_model_complete']==validated['matched_complete_exact']
    base=ROOT/a.baseline
    expected=read(base/'plain-local-types.json')
    manifest=base/'build-manifest.json'
    records=[]
    with importlib.import_module(cfg['module']).SlotModelClient(manifest) as model:
        for patch in result['patches']:
            logical=patch['logical_path'];frozen=base/logical;live=ROOT/logical
            assert sha(frozen)==sha(live),'Source changed since the prepared baseline: '+logical
            text=frozen.read_bytes().decode('utf-8-sig')
            assert text.count(patch['before'])==1,'Ambiguous/stale source payload'
            candidate=text.replace(patch['before'],patch['after'],1)
            prediction=model.predict(live,candidate,patch['method_id'])
            assert prediction['method_id']==patch['method_id']
            equal=prediction['predicted_types']==expected[patch['method_id']]
            if prediction['complete'] is True:
                assert equal,'Complete real prediction does not equal original signature'
            else:
                assert a.allow_genuine_incomplete and prediction.get('unsupported_reasons'),'Explicit permission and actual incompleteness reasons are required'
            previous={k:patch.get(k) for k in ('model_source_sha256','client_sha256','complete','predicted_types','unsupported_reasons')}
            patch.update(previous_model_binding=previous,model_source_sha256=sha(source),client_sha256=sha(client),
                         complete=prediction['complete'],predicted_types=prediction['predicted_types'],
                         unsupported_reasons=prediction['unsupported_reasons'],
                         source_model_fields_derived_not_promoted=True,game_body_status='UNKNOWN')
            import hashlib
            records.append(dict(method_id=patch['method_id'],logical_source=logical,
                frozen_full_source_sha256=sha(frozen),live_full_source_sha256=sha(live),
                candidate_full_source_sha256=hashlib.sha256(candidate.encode('utf-8')).hexdigest(),
                previous_model_binding=previous,prediction=prediction,
                original_signature_equal=equal,original_signature_used_as_prediction=False,
                complete_wrong_prediction_bypassed=False,emitted_original_signature_still_mandatory=True))
    producer=Path(__file__).relative_to(ROOT).as_posix()
    dependencies={name:sha(ROOT/name) for name in [producer,cfg['source'],cfg['client'],cfg['solver']]}
    proof=dict(operation='fresh-source-only-model-binding',old_bundle=a.bundle,old_bundle_sha256=sha(old_path),
               assembly=a.assembly,baseline=a.baseline,manifest_sha256=sha(manifest),
               model_validation=cfg['validation'],model_validation_sha256=sha(validation),
               dependency_sha256=dependencies,records=records,target_compilations=0,
               ledger_writes=0,binary_writes=0,source_writes=0,game_body_status='UNKNOWN')
    proof_path=ROOT/a.proof;out=ROOT/a.output
    assert not proof_path.exists() and not out.exists()
    proof_path.write_text(json.dumps(proof,indent=2)+'\n',encoding='utf-8')
    result.setdefault('dependency_sha256',{}).update(dependencies)
    result.setdefault('evidence_sha256',{}).update({a.bundle:sha(old_path),a.proof:sha(proof_path),cfg['validation']:sha(validation)})
    result['source_model_rebinding']=dict(proof=a.proof,sha256=sha(proof_path),original_bundle_preserved=True)
    out.write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8')
    assert sha(old_path)==a.bundle_sha256
    print(json.dumps(dict(patches=len(records),complete_real_predictions=sum(r['prediction']['complete'] for r in records),genuine_incomplete_predictions=sum(not r['prediction']['complete'] for r in records),target_compilations=0,output=a.output,sha256=sha(out))))

if __name__=='__main__':main()
