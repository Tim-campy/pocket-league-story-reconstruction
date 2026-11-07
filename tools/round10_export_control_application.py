"""Publish a frozen source-only audit/bundle without changing target source."""
import json
from pathlib import Path
from compare_kairo import sha

ROOT=Path(__file__).resolve().parents[1]


def main():
    folder=ROOT/'build/round10/control-application/c4-source-proposals'
    bundle_path=folder/'patches.json';audit_path=folder/'audit.json'
    bundle=json.loads(bundle_path.read_text(encoding='utf-8-sig'))
    audit=json.loads(audit_path.read_text(encoding='utf-8-sig'))
    assert audit['inputs_changed'] is False and audit['game_compilations']==0
    assert audit['pending_bodies']==114
    for name,digest in audit['input_sha256'].items():
        assert sha(Path(name))==digest,('Historical audit input changed before export',name)
    dependencies=dict(bundle['dependency_sha256'])
    frozen={}
    historical={}
    for name,digest in {**bundle['extractor_dependency_sha256'],**bundle['evidence_sha256']}.items():
        path=Path(name).resolve();assert sha(path)==digest
        relative=path.relative_to(ROOT).as_posix()
        if path.is_relative_to((ROOT/'build/round10').resolve()):frozen[relative]=digest
        elif relative.startswith('reports/'):
            pass
        else:historical[relative]=digest
    for name,digest in bundle['extractor_dependency_sha256'].items():
        public=ROOT/'tools'/Path(name).name
        assert sha(public)==digest,('Frozen/public extractor differs',public)
        dependencies[public.relative_to(ROOT).as_posix()]=digest
    targets={name:ROOT/'reports'/name for name in ('round10-control-application-audit.json',
        'round10-control-application-export.json','round10-control-rewrite-patches.json')}
    assert not any(p.exists() for p in targets.values())
    ap=targets['round10-control-application-audit.json']
    ap.write_text(json.dumps(audit,indent=2)+'\n',encoding='utf-8')
    validation='reports/round10-control-validation-stage04.json'
    evidence={validation:sha(ROOT/validation),ap.relative_to(ROOT).as_posix():sha(ap)}
    ep=targets['round10-control-application-export.json']
    export={'operation':'verified-frozen-control-application-publication','source_only':True,
       'target_source_writes':False,'method_states_modified':False,'game_compilations':0,
       'original_bundle_sha256':sha(bundle_path),'original_audit_sha256':sha(audit_path),
       'frozen_evidence_sha256':frozen,'dependency_sha256':dependencies,
       'historical_config_input_sha256':historical,'historical_inputs_rechecked_at_export':True,
       'producer_sha256':sha(Path(__file__)),'limitations':'Proposal only; fresh engine model and full real compiler/body/metadata verification remain required'}
    ep.write_text(json.dumps(export,indent=2)+'\n',encoding='utf-8')
    evidence[ep.relative_to(ROOT).as_posix()]=sha(ep)
    dependencies[Path(__file__).relative_to(ROOT).as_posix()]=sha(Path(__file__))
    patches=[]
    for p in bundle['patches']:
        p=dict(p,dependency_sha256=dependencies,evidence_sha256=evidence,
               frozen_evidence_sha256=frozen,historical_evidence_sha256=bundle['evidence_sha256'])
        patches.append(p)
    public=dict(bundle,patches=patches,dependency_sha256=dependencies,evidence_sha256=evidence,
                frozen_evidence_sha256=frozen,historical_evidence_sha256=bundle['evidence_sha256'])
    targets['round10-control-rewrite-patches.json'].write_text(json.dumps(public,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({'pending_audited':audit['pending_bodies'],'proposals':len(patches),
       'potential_original_il_bytes':bundle['potential_original_IL_bytes'],'game_compilations':0,
       'frozen_inputs_checked':len(frozen)}))


if __name__=='__main__':main()
