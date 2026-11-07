"""Register fixture-proved families for bounded tests, never method states."""
import json
from pathlib import Path
from compare_kairo import sha

ROOT = Path(__file__).resolve().parents[1]


def main():
    path = ROOT / 'config/round10-protocol.json'
    cfg = json.loads(path.read_text(encoding='utf-8'))
    journal = json.loads((ROOT / 'reports/round10-cycle-journal.json').read_text(encoding='utf-8'))
    assert journal['cycles'][-1]['label'] == 'cycle04'
    assert journal['cycles'][-1]['methods_gained'] == 1
    registered = []
    for family in ('smcs-empty-arms', 'roslyn-lifetimes', 'static-bool'):
        bundle_path = ROOT / 'reports' / ('round10-' + family + '-patches.json')
        bundle = json.loads(bundle_path.read_text(encoding='utf-8'))
        item_id = 'd-ready-' + family
        assert not any(item['id'] == item_id for item in cfg['prepared_queue'])
        cfg['prepared_queue'].append(dict(
            id=item_id, phase='D', status='pending', readiness='READY_FOR_GAME_PROBE',
            method_ids=[p['method_id'] for p in bundle['patches']],
            required_work='Fresh model, real target compiler, all-pending pass, full official verification; no gain assumed',
            report=bundle_path.relative_to(ROOT).as_posix(), report_sha256=sha(bundle_path)))
        registered.append(item_id)
    cfg['report_schema_reviewed'] = True
    cfg['controller_review_status'] = 'EXECUTED_WITH_MEASURED_FULL_VERIFICATION_PROOFS'
    cfg.setdefault('preparation_notes', []).append(
        'After cycle04, three additional fixture-tested source families were registered as pending tests. '
        'They prevent counting zero-gain cycles until tested; method states remain exclusively verifier-generated. '
        'The standalone eight-section report adapter was reviewed for A/B/C evidence, baseline runtime scope, '
        'raw PDB-path persistence, exact current measurements, models, and ten inline IL examples.')
    path.write_text(json.dumps(cfg, indent=2) + '\n', encoding='utf-8')
    print(json.dumps({'queue_registered': registered, 'method_states_modified': False,
                      'report_adapter_reviewed': True}))


if __name__ == '__main__':
    main()
