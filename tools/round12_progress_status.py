"""Read-only completion badge: a stale PE/source proof never means CONCLUIDA."""
import json
from pathlib import Path
import hashlib


def kairo_completion(root):
    cfg_path = root/'config/round12-protocol.json'
    if not cfg_path.exists(): return dict(complete=False, status='NOT_EVALUATED')
    cfg = json.loads(cfg_path.read_text(encoding='utf-8-sig'))
    gate = cfg.get('part_a_raw_gate', {})
    if gate.get('status') != 'PASS': return dict(complete=False, status=gate.get('status', 'NOT_EVALUATED'))
    try:
        from round12_controller import current_measurement, verify_raw_gate
        journal = json.loads((root/'reports/round12-cycle-journal.json').read_text(encoding='utf-8-sig'))
        measured = current_measurement(cfg,journal)
        report = verify_raw_gate(cfg,measured)
        live = json.loads((root/'build/verification-KairoLibrary.json').read_text(encoding='utf-8-sig'))
        if live['counts']['matched'] != 3287 or live['types_equal'] != 337:
            raise ValueError('Completion counts/types changed')
        return dict(complete=True,status='CONCLUIDA',report=gate['report'].replace('\\','/'),report_sha256=gate['sha256'],
                    recompiled_sha256=report['recompiled_sha256'],
                    bytes_outside_exceptions=report['original_size']-40)
    except (ValueError, KeyError, OSError) as error:
        # verify--all legitimately builds a new MVID before its postverify raw
        # renewal. Show pending instead of deadlocking verification/progress.
        return dict(complete=False,status='RAW_RENEWAL_REQUIRED',reason=str(error))


def app_completion(root):
    """The App badge needs current literal and semantic proofs, including ownership."""
    try:
        from round12_controller import current_measurement
        cfg=json.loads((root/'config/round12-protocol.json').read_text(encoding='utf-8-sig'))
        proof=cfg.get('app_raw_measurements',[])[-1]
        if proof['status']!='PASS':return dict(complete=False,status=proof['status'])
        journal=json.loads((root/'reports/round12-cycle-journal.json').read_text(encoding='utf-8-sig'))
        measured=current_measurement(cfg,journal)
        raw_path=root/proof['report']
        digest=lambda path:hashlib.sha256(path.read_bytes()).hexdigest()
        if digest(raw_path)!=proof['sha256']:raise ValueError('App raw report changed')
        raw=json.loads(raw_path.read_text(encoding='utf-8-sig'))
        if raw['producer_sha256']!=digest(root/'tools/round12_raw_compare.py'):raise ValueError('App raw producer changed')
        if raw['status']!='PASS' or any(raw[k]for k in ('semantic_divergent_bodies','held_methods','outside_exceptions_differing_bytes')):
            raise ValueError('App semantic/literal closure is incomplete')
        fields=raw['exception_coordinates']['original']
        if (fields!=raw['exception_coordinates']['rebuilt']or set(fields)!={'COFF.TimeDateStamp','Module.Mvid'}
                or fields['COFF.TimeDateStamp']['size']!=4 or fields['Module.Mvid']['size']!=16):
            raise ValueError('App exception geometry is not the approved two-field profile')
        live=json.loads((root/'build/verification-Assembly-CSharp.json').read_text(encoding='utf-8-sig'))
        from apply_il_rules import receipt_is_fresh
        if not receipt_is_fresh(root,live):raise ValueError('App source/profile proof is stale')
        for path,expected in live.get('helper_source_hashes',{}).items():
            if digest(root/path)!=expected:raise ValueError('App verifier helper changed')
        if (live['counts']['matched']!=1958 or live['types_equal']!=129 or live['types_total']!=129
                or measured['assemblies']['Assembly-CSharp']['matched_methods']!=1958
                or raw['original_sha256']!=live['original_sha256']
                or raw['recompiled_sha256']!=live['build_sha256']
                or digest(root/live['build_artifact'])!=live['build_sha256']):
            raise ValueError('App source/PE/receipt counts changed')
        return dict(complete=True,status='CONCLUIDA',report=proof['report'].replace('\\','/'),
                    report_sha256=proof['sha256'],recompiled_sha256=raw['recompiled_sha256'],
                    bytes_outside_exceptions=raw['original_size']-20,
                    transport=proof.get('transport'))
    except (ValueError,KeyError,OSError,IndexError)as error:
        return dict(complete=False,status='RAW_RENEWAL_REQUIRED',reason=str(error))
