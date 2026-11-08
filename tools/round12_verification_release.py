"""Measured verifier-origin release, separate from source reconstruction cycles.

This helper never compiles targets, changes source/ledger, or edits the cycle
journal. Its caller must first run the ordinary complete verifier. It freezes
that output, checks the transition, renews literal raw evidence, then publishes
a hash-bound measurement pointer. Nonces may refresh; the event's core and
the last reconstruction row remain immutable.
"""
import argparse
import hashlib
import json
from pathlib import Path
import subprocess
import sys

ROOT = next(p for p in Path(__file__).resolve().parents if (p/'AGENTS.md').exists())
BASE_COMMIT = 'a075dcc8ca7b08e2cb9fcdd0cbcb5ce1fe8cd236'
ASSEMBLIES = ('KairoLibrary', 'Assembly-CSharp')
EVENT_PATH = 'reports/round12-verification-origin-release.json'
PROFILE_KEYS = ('compiler', 'compiler_sha256', 'runtime', 'runtime_sha256', 'flags',
                'references', 'stdlib_binding', 'scoped_environment')
METRICS = ('matched_methods', 'matched_original_il_bytes', 'divergent_bodies',
           'divergent_original_il_bytes', 'equal_body_held_methods')

def load(path): return json.loads(Path(path).read_text(encoding='utf-8-sig'))
def sha(path): return hashlib.sha256(Path(path).read_bytes()).hexdigest()
def digest(value):
    return hashlib.sha256(json.dumps(value,sort_keys=True,separators=(',',':'),ensure_ascii=True).encode()).hexdigest()
def save(path,value): Path(path).write_text(json.dumps(value,indent=2,ensure_ascii=True)+'\n',encoding='utf-8')
def require(condition,message):
    if not condition: raise ValueError(message)
def run(args): subprocess.run([sys.executable,'-X','utf8',*map(str,args)],cwd=ROOT,check=True)
def metrics(measurement):
    return {a:{k:measurement['assemblies'][a][k] for k in METRICS} for a in ASSEMBLIES}
def index(receipt):
    rows=list(receipt['methods'].values())
    result={r['stable_id']:r for r in rows}
    require(len(result)==len(rows),'Ambiguous stable IDs in official receipt')
    return result

def verified_matched_rows(receipt):
    for row in receipt['methods'].values():
        if row['state']=='matched':
            require(row.get('generated_group_pending',False) is False,'Matched member still has pending generated ownership')
            require(row.get('body_equal') is True if row.get('body_present') else
                    row.get('metadata_declaration_equal') is True,'Matched member has no proved body/declaration')

def compilation_profile(manifest):
    result={k:manifest.get(k) for k in PROFILE_KEYS}
    pdb=manifest.get('pdb_output')
    # Keep the full PDB output separately as artifact authority. Its GUID/hash
    # and whether SUBST was created this run are not compiler configuration.
    if pdb is not None:
        require(isinstance(pdb,dict),'Unknown PDB configuration schema')
        result['pdb_configuration']={k:v for k,v in pdb.items()
                                    if k not in ('pdb_sha256','alias_created_this_build')}
    return result

def receipt_metrics(receipt,il_sizes):
    rows=index(receipt)
    require(set(rows)==set(il_sizes),'Original inventory does not match the official receipt')
    return dict(matched_methods=sum(r['state']=='matched' for r in rows.values()),
        matched_original_il_bytes=sum(il_sizes[mid] for mid,r in rows.items() if r['state']=='matched'),
        divergent_bodies=sum(bool(r.get('body_present') and not r.get('body_equal')) for r in rows.values()),
        divergent_original_il_bytes=sum(il_sizes[mid] for mid,r in rows.items() if r.get('body_present') and not r.get('body_equal')),
        equal_body_held_methods=sum(bool(r.get('body_present') and r.get('body_equal') and r['state']!='matched') for r in rows.values()))

def original_sizes(snapshot,assembly):
    return {m['stable_id']:(m.get('header') or {}).get('code_size',0)
            for m in load(snapshot/assembly/'original-methods.json')['selected_methods']}

def origin_certificate(row):
    proof=row.get('ownership_proof',{})
    availability=row.get('source_availability_proof',{})
    require(proof.get('proved') is True and proof.get('kind')=='compiler_static_data_default_ctor',
            'Release requires the generic compiler static-data constructor certificate')
    require(proof.get('origins') and proof.get('field_ids') and proof.get('payload_bytes',0)>0,
            'Static-data consumers, fields and payload evidence are absent')
    require(proof.get('all_consumer_bodies_equal') is True and proof.get('field_witnesses'),
            'Static-data consumer/ldtoken witnesses are not complete')
    require(all(proof.get(k) is True for k in ('matching_holder_metadata','matching_backing_layouts',
            'matching_consumer_declarations','implicit_constructor_shape'))
            and proof.get('direct_constructor_references')==0,'Generated holder metadata or implicit constructor shape differs')
    require(set(proof['field_ids'])=={w['field'] for w in proof['field_witnesses']}
            and set(proof['origins'])=={w['origin'] for w in proof['field_witnesses']}
            and all(w.get('matched_in_both') is True for w in proof['field_witnesses']),
            'Paired field witnesses do not cover all fields and origins')
    require(availability.get('proved') is True and availability.get('source_paths')
            and availability.get('unavailable_origins')==[]
            and set(availability.get('origins',[]))==set(proof['origins'])
            and availability.get('kind')=='compiled_origins_for_generated_data_holder',
            'Compiled source origins are required for a source-less generated holder')
    return dict(ownership_proof=proof,source_availability_proof=availability)

def bind_reviewed_origin(certificate,released,authority):
    require(certificate.get('proved') is True and certificate.get('method_id')==released['method_id']
            and certificate.get('original_il_bytes')==released['original_il_bytes'],
            'Reviewed certificate belongs to another release')
    require(all(certificate.get(k)==released[k] for k in ('ownership_proof','source_availability_proof')),
            'Full-verifier origin proof differs from the reviewed generic certificate')
    helpers=authority[released['assembly']]['helper_source_hashes']
    require(certificate.get('generic_rule_sha256')==helpers.get('tools/generated_groups.py')
            and certificate.get('sidecar_sha256')==helpers.get('tools/verify_csharp_sidecar.py'),
            'Reviewed certification code is not the code that produced the official receipt')

def validate_transition(before,after,before_receipts,after_receipts,il_sizes):
    """Pure transition guard; no authority to change a method state."""
    releases=[];gains={}
    for assembly in ASSEMBLIES:
        old=before['assemblies'][assembly];new=after['assemblies'][assembly]
        verified_matched_rows(before_receipts[assembly]);verified_matched_rows(after_receipts[assembly])
        require({k:old[k] for k in METRICS}==receipt_metrics(before_receipts[assembly],il_sizes[assembly])
                and {k:new[k] for k in METRICS}==receipt_metrics(after_receipts[assembly],il_sizes[assembly]),
                'Measurement counts disagree with official method states or original IL sizes')
        require(old['divergent_bodies']==new['divergent_bodies']==0,'A body differs before/after release')
        require(new['equal_body_held_methods']==0,'A held method remains after full verification')
        left=index(before_receipts[assembly]);right=index(after_receipts[assembly])
        require(set(left)==set(right),'Method inventory changed')
        for mid in left:
            l,r=left[mid],right[mid]
            require(l.get('body_present')==r.get('body_present'),'Body presence changed')
            if l.get('body_present'):
                require(l.get('body_equal') is True and r.get('body_equal') is True,'A body is not equal')
            else:
                require(l.get('metadata_declaration_equal') is True and r.get('metadata_declaration_equal') is True,
                        'Bodyless metadata declaration is not equal')
            if l['state']!=r['state']:
                require(l.get('body_present') and l.get('body_equal') and l['state']!='matched'
                        and r['state']=='matched' and l.get('generated_group_pending') is True,
                        'Only previously held, equal generated bodies may be released')
                certificate=origin_certificate(r)
                require(all(origin in right and right[origin].get('body_equal') is True
                            and right[origin]['state']=='matched' for origin in certificate['ownership_proof']['origins']),
                        'A source consumer is absent or not verified matched')
                releases.append(dict(assembly=assembly,method_id=mid,original_il_bytes=il_sizes[assembly][mid],
                                     before_state=l['state'],after_state=r['state'],**certificate))
        gain=dict(methods_gained=new['matched_methods']-old['matched_methods'],
                  il_bytes_gained=new['matched_original_il_bytes']-old['matched_original_il_bytes'])
        require(gain==dict(methods_gained=sum(x['assembly']==assembly for x in releases),
                           il_bytes_gained=sum(x['original_il_bytes'] for x in releases if x['assembly']==assembly)),
                'Measured gain does not equal the verified held-state transition')
        gains[assembly]=gain
    require(len(releases)==1 and sum(x['original_il_bytes'] for x in releases)==7,
            'This authorized origin-release event must be exactly one held method / seven IL bytes')
    require(sum(before['assemblies'][a]['equal_body_held_methods'] for a in ASSEMBLIES)==1,
            'Unexpected prior held inventory')
    return dict(releases=releases,gains=gains)

def validate_anchor(event,journal,journal_sha):
    core=event['event_core'];anchor=core['anchor']
    require(event.get('event_core_sha256')==digest(core),'Immutable event core changed')
    require(journal_sha==anchor['journal_sha256'],'Reconstruction cycle journal changed after release')
    require(journal.get('cycles') and digest(journal['cycles'][-1])==anchor['last_row_sha256'],
            'The event is not anchored to the current last reconstruction row')
    row=journal['cycles'][-1]
    require(row['label']==anchor['label'] and row['after_measurement']==anchor['after_measurement']
            and row['after_sha256']==anchor['after_measurement_sha256'],'Last cycle measurement was rewritten')
    require(sum(r['phase']=='B' for r in journal['cycles'])==anchor['B_cycles'],
            'Verifier event must not add or remove reconstruction cycles')

def bind_snapshot_inputs(snapshot,measurement):
    require(measurement.get('baseline_commit')==BASE_COMMIT,'Measurement belongs to another round')
    require((ROOT/measurement['snapshot']).resolve()==snapshot.resolve(),'Measurement snapshot differs')
    require(measurement['ledger_sha256']==sha(snapshot/'methods.csv')==load(snapshot/'snapshot.json')['ledger_sha256'],
            'Frozen ledger is not bound to its measurement')
    artifacts=load(snapshot/'assemblies/artifacts.json');result={}
    for assembly in ASSEMBLIES:
        proof=measurement['assemblies'][assembly]['provenance'];bound={}
        for name,filename,hash_key in [('original_dump','original-methods.json','original_dump_sha256'),
                ('rebuilt_dump','rebuilt-methods.json','rebuilt_dump_sha256'),
                ('receipt','verification.json','receipt_sha256')]:
            actual=snapshot/assembly/filename
            require((ROOT/proof[name]).resolve()==actual.resolve(),'Measurement points outside its frozen input')
            require(sha(actual)==proof[hash_key],'Frozen dump/receipt changed after measurement: '+filename)
            bound[name]=dict(path=proof[name],sha256=proof[hash_key])
        require(artifacts[assembly]['sha256']==proof['build_sha256'],'Measured PE differs from artifacts manifest')
        result[assembly]=bound
    return result

def source_authority(snapshot,measurement,live=False):
    """Check ordinary sources/profile, canonical receipts and the frozen PE links."""
    from apply_il_rules import receipt_is_fresh
    out={};artifacts=load(snapshot/'assemblies/artifacts.json')
    bindings=bind_snapshot_inputs(snapshot,measurement)
    for assembly in ASSEMBLIES:
        manifest=load(snapshot/assembly/'build-manifest.json')
        receipt=load(snapshot/assembly/'verification.json')
        verified_matched_rows(receipt)
        artifact=artifacts[assembly]
        require(sha(snapshot/artifact['path'])==artifact['sha256']==receipt['build_sha256']==manifest['output_sha256'],
                'Snapshot PE / manifest / receipt disagreement')
        for source in manifest['sources']:
            require(sha(snapshot/assembly/source['path'])==source['sha256'],'Frozen C# source changed')
            if live: require(sha(ROOT/source['path'])==source['sha256'],'Live C# source changed')
        require(receipt['types_equal']==receipt['types_total'] and not any(receipt.get(k) for k in
                ('regressions','body_regressions','type_regressions')),'Type loss or regression')
        if live:
            for name,hash_key in (('compiler','compiler_sha256'),('runtime','runtime_sha256')):
                if manifest.get(name):require(sha(ROOT/manifest[name])==manifest[hash_key], 'Compiler/runtime changed')
            for reference in manifest['references']:
                require(sha(ROOT/reference['path'])==reference['sha256'],'Reference artifact changed')
            if manifest.get('pdb_output'):
                pdb=manifest['pdb_output']
                require(sha(ROOT/pdb['analysis_mirror'])==pdb['pdb_sha256'],'Current PDB mirror changed after its manifest')
            canonical=load(ROOT/('build/verification-'+assembly+'.json'))
            require(receipt_is_fresh(ROOT,canonical) and canonical['build_sha256']==receipt['build_sha256'],
                    'Canonical receipt or PE is stale')
            require(sha(ROOT/('build/verification-'+assembly+'.json'))==sha(snapshot/assembly/'verification.json'),
                    'Canonical method/type proof differs from its frozen receipt')
            for name,value in canonical.get('helper_source_hashes',{}).items():
                require(sha(ROOT/name)==value,'Verifier helper changed since the receipt: '+name)
        out[assembly]=dict(profile=compilation_profile(manifest),sources=manifest['sources'],
                           pdb_output_evidence=manifest.get('pdb_output'),input_bindings=bindings[assembly],
                           receipt_sha256=sha(snapshot/assembly/'verification.json'),
                           helper_source_hashes=receipt.get('helper_source_hashes',{}),
                           types_equal=receipt['types_equal'],types_total=receipt['types_total'],
                           receipt_metrics=receipt_metrics(receipt,original_sizes(snapshot,assembly)),
                           build_sha256=artifact['sha256'])
    return out

def compare_authority(before,after,refresh=False):
    for assembly in ASSEMBLIES:
        for field in ('profile','sources','types_equal','types_total'):
            require(before[assembly][field]==after[assembly][field], 'C# / compiler / flags / references changed: '+assembly)
        if refresh:
            require(before[assembly]['helper_source_hashes']==after[assembly]['helper_source_hashes'],
                    'The origin verifier changed since the confirmed event')

def verifier_delta(before,after):
    changes={}
    for assembly in ASSEMBLIES:
        left=before[assembly]['helper_source_hashes'];right=after[assembly]['helper_source_hashes']
        changes[assembly]={name:dict(before_sha256=left.get(name),after_sha256=right.get(name))
            for name in set(left)|set(right) if left.get(name)!=right.get(name)}
    require({name for rows in changes.values() for name in rows}==
            {'tools/generated_groups.py','tools/verify_csharp_sidecar.py'},
            'Initial release must use exactly the reviewed generic ownership and source-availability correction')
    return changes

def body_inventory_proof(before,after):
    from compare_kairo import differences
    result={}
    for assembly in ASSEMBLIES:
        lp=before/assembly/'rebuilt-methods.json';rp=after/assembly/'rebuilt-methods.json'
        left={m['stable_id']:m for m in load(lp)['selected_methods']}
        right={m['stable_id']:m for m in load(rp)['selected_methods']}
        require(set(left)==set(right),'Emitted method inventory changed')
        changed=[mid for mid in left if differences(left[mid],right[mid])[0]]
        require(not changed,'An emitted body/header/locals/EH changed')
        result[assembly]=dict(before_dump_sha256=sha(lp),after_dump_sha256=sha(rp),
                              compared_methods=len(left),changed_methods=[],all_body_components_unchanged=True)
    return result

def validate_pointer(root,cfg,journal):
    """Strict current_measurement reader. A stale nonce/source never falls back."""
    global ROOT
    ROOT=Path(root)
    pointer=cfg['verification_current_measurement'];binding=cfg['verification_origin_release']
    path=ROOT/binding['record'];require(sha(path)==binding['sha256']==pointer['event_record_sha256'],'Event hash changed')
    event=load(path);validate_anchor(event,journal,sha(ROOT/'reports/round12-cycle-journal.json'))
    require(event['event_core']['producer_sha256']==sha(Path(__file__)),'Release producer changed after the confirmed event')
    origin=event['event_core']['origin_proof'];require(sha(ROOT/origin['path'])==origin['sha256'],'Origin certificate changed')
    last=event['commit_provenance'][-1]
    require(last['measurement']==pointer['path'] and last['measurement_sha256']==pointer['sha256'],
            'Measurement pointer is not the current event provenance')
    mp=ROOT/pointer['path'];require(sha(mp)==pointer['sha256'],'Current release measurement hash changed')
    result=load(mp);require(result['snapshot']==pointer['snapshot']==last['snapshot'],'Snapshot link differs')
    require(result['ledger_sha256']==sha(ROOT/'config/methods.csv'),'Ledger changed after the event measurement')
    require(metrics(result)==event['event_core']['after_metrics'],'Event counts changed during nonce renewal')
    authority=source_authority(ROOT/result['snapshot'],result,live=True)
    require({a:authority[a]['receipt_metrics'] for a in ASSEMBLIES}==metrics(result),
            'Current measurement disagrees with the real canonical verifier states')
    compare_authority(event['event_core']['after_authority'],authority,refresh=True)
    require(authority==last['authority'],'PE / receipt / helper authority changed')
    return result

def renewed_raw(cfg,snapshot,label,measurement):
    from round12_controller import verify_raw_gate
    from round12_app_raw_trigger import measure_if_due
    gate=cfg['part_a_raw_gate'];old=load(ROOT/gate['report'])
    require(sha(ROOT/gate['report'])==gate['sha256'],'Previous Kairo raw evidence changed')
    require(sha(ROOT/'tools/round12_raw_compare.py')==old['producer_sha256'],'Approved raw producer changed')
    run(['tools/round12_raw_compare.py','--snapshot',snapshot,'--assembly','KairoLibrary','--label',label])
    name='reports/round12-'+label+'-KairoLibrary-raw.json';report=load(ROOT/name)
    gate.update(status=report['status'],report=name,sha256=sha(ROOT/name),snapshot=snapshot.as_posix(),
                recompiled_sha256=report['recompiled_sha256'])
    verify_raw_gate(cfg,measurement)
    app=measure_if_due(measurement,snapshot,label)
    require(app is not None,'Release must record the App raw measurement too')
    cfg.setdefault('app_raw_measurements',[]).append(app)
    return dict(KairoLibrary=dict(report=name,sha256=sha(ROOT/name)),Assembly_CSharp=app)

def main():
    p=argparse.ArgumentParser(description=__doc__);p.add_argument('--label',required=True)
    p.add_argument('--origin-proof');p.add_argument('--origin-proof-sha256');args=p.parse_args()
    require(args.label.replace('-','').replace('_','').isalnum(),'Unsafe verification label')
    journal_path=ROOT/'reports/round12-cycle-journal.json';journal=load(journal_path);journal_sha=sha(journal_path)
    cfg_path=ROOT/'config/round12-protocol.json';cfg=load(cfg_path)
    event_path=ROOT/EVENT_PATH;existing=event_path.exists()
    require(journal['cycles'],'Release requires an existing reconstruction journal')
    if existing:
        event=load(event_path);require(sha(event_path)==cfg['verification_origin_release']['sha256'],'Existing event changed')
        validate_anchor(event,journal,journal_sha)
        before_measurement=event['event_core']['before_measurement'];old_after=event['event_core']['after_metrics']
        origin=event['event_core']['origin_proof']
    else:
        require(args.origin_proof and args.origin_proof_sha256,'Initial event requires explicit reviewed origin-proof SHA')
        require(sha(ROOT/args.origin_proof)==args.origin_proof_sha256,'Origin proof hash differs')
        origin=dict(path=args.origin_proof,sha256=args.origin_proof_sha256)
        row=journal['cycles'][-1];before_measurement=dict(path=row['after_measurement'],sha256=row['after_sha256'])
    require(sha(ROOT/origin['path'])==origin['sha256'],'Reviewed origin proof changed')
    require(sha(ROOT/before_measurement['path'])==before_measurement['sha256'],'Before measurement changed')
    before=load(ROOT/before_measurement['path']);before_snapshot=ROOT/before['snapshot']
    number=1
    while True:
        label=args.label+'-verification%02d'%number
        relative=Path('build/round12/verification-origin-release')/label
        mp=Path('reports/round12-'+label+'-measurement.json')
        if not (ROOT/relative).exists() and not (ROOT/mp).exists():break
        number+=1
    run(['tools/round12_freeze.py','--destination',relative])
    run(['tools/round12_measure.py','--snapshot',relative,'--label',label])
    current=load(ROOT/mp);snapshot=ROOT/relative
    authority=source_authority(snapshot,current,live=True)
    previous_authority=source_authority(before_snapshot,before)
    compare_authority(previous_authority,authority)
    bodies=body_inventory_proof(before_snapshot,snapshot)
    if existing:
        require(metrics(current)==old_after,'Verified event gains changed during nonce refresh')
        compare_authority(event['event_core']['after_authority'],authority,refresh=True)
    else:
        br={a:load(before_snapshot/a/'verification.json') for a in ASSEMBLIES}
        ar={a:load(snapshot/a/'verification.json') for a in ASSEMBLIES}
        sizes={a:original_sizes(before_snapshot,a) for a in ASSEMBLIES}
        # The authoritative DTO stores code_size in its header; no original IL is emitted here.
        transition=validate_transition(before,current,br,ar,sizes)
        for released in transition['releases']:
            bind_reviewed_origin(load(ROOT/origin['path']),released,authority)
            source_paths={s['path'] for s in authority[released['assembly']]['sources']}
            require(set(released['source_availability_proof']['source_paths'])<=source_paths,
                    'Origin certificate names files absent from the real compilation manifest')
        row=journal['cycles'][-1]
        core=dict(event='verification-origin-release',status='CONFIRMED_BY_FULL_VERIFY',source_changes=0,
                  producer_sha256=sha(Path(__file__)),
                  reconstruction_cycles_added=0,before_measurement=before_measurement,
                  before_metrics=metrics(before),after_metrics=metrics(current),
                  before_authority=previous_authority,after_authority=authority,
                  verifier_helper_changes=verifier_delta(previous_authority,authority),
                  origin_proof=origin,body_inventory_proof=bodies,**transition,
                  anchor=dict(journal_sha256=journal_sha,last_row_sha256=digest(row),label=row['label'],
                              after_measurement=row['after_measurement'],after_measurement_sha256=row['after_sha256'],
                              B_cycles=sum(r['phase']=='B' for r in journal['cycles'])))
        event=dict(schema_version=1,event_core=core,event_core_sha256=digest(core),commit_provenance=[])
    raw=renewed_raw(cfg,relative,label,current)
    event['commit_provenance'].append(dict(measurement=mp.as_posix(),measurement_sha256=sha(ROOT/mp),
        snapshot=current['snapshot'],authority=authority,body_inventory_proof=bodies,raw=raw,
        event_gains_unchanged=True,cycle_journal_unchanged=True))
    require(sha(journal_path)==journal_sha,'Reconstruction journal changed while certifying the event')
    draft=Path('build/round12/verification-origin-release-drafts')/(label+'.json')
    (ROOT/draft).parent.mkdir(parents=True,exist_ok=True)
    require(not (ROOT/draft).exists(),'Never overwrite a prior event candidate')
    save(ROOT/draft,event)
    cfg['verification_origin_release']=dict(record=draft.as_posix(),sha256=sha(ROOT/draft))
    cfg['verification_current_measurement']=dict(path=mp.as_posix(),sha256=sha(ROOT/mp),
        snapshot=current['snapshot'],event_record_sha256=sha(ROOT/draft))
    validate_pointer(ROOT,cfg,journal)
    save(event_path,event)
    cfg['verification_origin_release']['record']=EVENT_PATH
    save(cfg_path,cfg)
    print(json.dumps(dict(event=EVENT_PATH,event_sha256=sha(event_path),snapshot=relative.as_posix(),
                         measurement=mp.as_posix(),journal_unchanged=True,cycles_added=0,target_compilations=0)))

if __name__=='__main__':main()
