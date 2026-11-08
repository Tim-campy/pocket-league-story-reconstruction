"""Certify compiler-data ownership and independently reread every App PE byte."""
import csv
import hashlib
import json
from pathlib import Path
import struct
import uuid

from compare_kairo import differences
from generated_groups import prove_groups, compiled_static_data_origins

ROOT=Path(__file__).resolve().parents[1]
KEY='<PrivateImplementationDetails>::.ctor()->System.Void'
SNAPSHOT=ROOT/'build/round12/runs/b39/commit/postverify01'


def sha(path):return hashlib.sha256(Path(path).read_bytes()).hexdigest()
def read(path):return json.loads(Path(path).read_text(encoding='utf-8-sig'))
def write(path,value):
    with Path(path).open('x',encoding='utf-8',newline='\n')as stream:
        stream.write(json.dumps(value,indent=2,ensure_ascii=False)+'\n')


def main():
    folder=SNAPSHOT/'Assembly-CSharp'
    original=read(folder/'original-methods.json');rebuilt=read(folder/'rebuilt-methods.json')
    left={m['stable_id']:m for m in original['selected_methods']}
    right={m['stable_id']:m for m in rebuilt['selected_methods']}
    equal={k for k in left if 'raw_body_hex'in left[k]and not differences(left[k],right.get(k))[0]}
    proofs,groups=prove_groups(original,rebuilt,{KEY},equal)
    proof=proofs[KEY]
    assert proof['proved']and proof['kind']=='compiler_static_data_default_ctor'
    assert len(proof['origins'])==32 and len(proof['field_ids'])==217 and proof['payload_bytes']==6660
    receipt=read(folder/'verification.json');manifest=read(folder/'build-manifest.json')
    methods=receipt['methods'];old_result=next(r for r in methods.values()if r['stable_id']==KEY)
    assert old_result['body_equal']and old_result['generated_group_pending']and old_result['state']!='matched'
    with (SNAPSHOT/'methods.csv').open(encoding='utf-8-sig',newline='')as stream:
        rows={r['token']:r for r in csv.DictReader(stream)if r['assembly']=='Assembly-CSharp'}
    token=lambda m:'0x'+format(0x06000000|int(m['token'].split('0x')[1].rstrip(']'),16),'08x')
    source_rows={m['stable_id']:rows[token(m)]for m in original['selected_methods']}
    assert source_rows[KEY]['fonte']==''
    compiled={s['path']:s for s in manifest['sources']}
    available=compiled_static_data_origins(proof,source_rows,set(compiled))
    assert available['proved']and len(available['source_paths'])==19
    for path in available['source_paths']:
        assert sha(folder/path)==sha(ROOT/path)==compiled[path]['sha256']
    assert all(next(r for r in methods.values()if r['stable_id']==identity)['state']=='matched'for identity in proof['origins'])
    cfg=read(ROOT/'config/round12-protocol.json');raw_proof=cfg['app_raw_measurements'][-1]
    assert sha(ROOT/raw_proof['report'])==raw_proof['sha256']
    raw=read(ROOT/raw_proof['report'])
    assert raw['semantic_divergent_bodies']==0 and raw['held_methods']==1 and raw['outside_exceptions_differing_bytes']==0
    artifact=read(SNAPSHOT/'assemblies/artifacts.json')['Assembly-CSharp']
    paths=[ROOT/'original/extracted/assets/bin/Data/Managed/Assembly-CSharp.dll',SNAPSHOT/artifact['path']]
    data=[p.read_bytes()for p in paths]
    assert sha(paths[0])==raw['original_sha256']==receipt['original_sha256']
    assert sha(paths[1])==raw['recompiled_sha256']==receipt['build_sha256']==manifest['output_sha256']==artifact['sha256']
    assert len(data[0])==len(data[1])==895488
    first=raw['exception_coordinates']['original'];second=raw['exception_coordinates']['rebuilt']
    assert first==second and set(first)=={'COFF.TimeDateStamp','Module.Mvid'}
    allowed=set()
    for name,size in [('COFF.TimeDateStamp',4),('Module.Mvid',16)]:
        assert first[name]['size']==size
        offset=first[name]['offset'];allowed.update(range(offset,offset+size))
    for image in data:
        pe=struct.unpack_from('<I',image,0x3c)[0]
        assert image[pe:pe+4]==b'PE\0\0'and first['COFF.TimeDateStamp']['offset']==pe+8
    guid=first['Module.Mvid']['offset']
    assert str(uuid.UUID(bytes_le=data[0][guid:guid+16]))==original['mvid']
    assert str(uuid.UUID(bytes_le=data[1][guid:guid+16]))==rebuilt['mvid']
    changed=[i for i,(a,b)in enumerate(zip(*data))if a!=b]
    outside=[i for i in changed if i not in allowed]
    assert len(allowed)==20 and not outside
    physical=dict(operation='independent-positional-App-byte-acceptance',literal_status='PASS',
                  semantic_ownership_still_pending=True,original_sha256=sha(paths[0]),recompiled_sha256=sha(paths[1]),
                  size=895488,allowed_field_span_bytes=20,literally_identical_bytes_outside_fields=895468,
                  differing_bytes=len(changed),outside_exceptions_differing_bytes=0,
                  allowed_fields=first,compared_every_byte=True,normalization_applied=False,
                  DLL_writes=0,target_compilations=0,method_or_type_states_modified=False,
                  snapshot=SNAPSHOT.relative_to(ROOT).as_posix(),raw_report=raw_proof['report'],
                  raw_report_sha256=raw_proof['sha256'])
    acceptance=ROOT/'reports/round12-app-independent-raw-acceptance.json';write(acceptance,physical)
    certificate=dict(schema_version=1,operation='read-only-static-data-constructor-certification',proved=True,
                     method_id=KEY,original_il_bytes=7,ownership_proof=proof,source_availability_proof=available,
                     snapshot=SNAPSHOT.relative_to(ROOT).as_posix(),snapshot_sha256=sha(SNAPSHOT/'snapshot.json'),
                     frozen_receipt_sha256=sha(folder/'verification.json'),ledger_sha256=sha(SNAPSHOT/'methods.csv'),
                     original_sha256=sha(paths[0]),recompiled_sha256=sha(paths[1]),
                     compiled_source_proofs=[dict(path=p,sha256=compiled[p]['sha256'])for p in available['source_paths']],
                     source_binding='Implicit compiler artifact has no unique authored C# file; all paired initializer consumers were compiled.',
                     before_state=old_result['state'],before_body_equal=True,before_ownership_pending=True,
                     own_fixture_report='reports/round12-static-data-constructor-emission-fixture-report.json',
                     own_fixture_report_sha256=sha(ROOT/'reports/round12-static-data-constructor-emission-fixture-report.json'),
                     compiler_source_evidence='reports/round12-static-data-constructor-compiler-evidence.json',
                     compiler_source_evidence_sha256=sha(ROOT/'reports/round12-static-data-constructor-compiler-evidence.json'),
                     generic_rule_sha256=sha(ROOT/'tools/generated_groups.py'),
                     sidecar_sha256=sha(ROOT/'tools/verify_csharp_sidecar.py'),
                     independent_raw_acceptance=acceptance.relative_to(ROOT).as_posix(),
                     independent_raw_acceptance_sha256=sha(acceptance),
                     verifier_state_writes=0,DLL_writes=0,target_compilations=0,producer_sha256=sha(Path(__file__)))
    path=ROOT/'reports/round12-static-data-constructor-ownership.json';write(path,certificate)
    print(json.dumps(dict(certificate=path.relative_to(ROOT).as_posix(),sha256=sha(path),
                         fields=len(proof['field_ids']),consumers=len(proof['origins']),
                         payload_bytes=proof['payload_bytes'],compiled_source_files=len(available['source_paths']),
                         independent_literal_outside_exceptions=0,verifier_state_writes=0)))


if __name__=='__main__':main()
