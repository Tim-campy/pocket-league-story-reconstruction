"""Read-only completion badge: a stale PE/source proof never means CONCLUIDA."""
import json
from pathlib import Path


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
