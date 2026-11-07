"""Register newly fixture-tested families between committed cycles."""
import argparse
import json
from pathlib import Path
import subprocess
from compare_kairo import sha
from round10_controller import stop_reason

ROOT = Path(__file__).resolve().parents[1]


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--family', action='append', required=True)
    args = parser.parse_args()
    cfg_path = ROOT / 'config/round10-protocol.json'
    cfg = json.loads(cfg_path.read_text(encoding='utf-8'))
    journal = json.loads((ROOT / 'reports/round10-cycle-journal.json').read_text(encoding='utf-8'))
    assert stop_reason(cfg, journal) is None, 'A measured stop cannot be bypassed by adding work'
    label = journal['cycles'][-1]['label']
    proof_path = 'reports/round10-' + label + '-git-source-proof.json'
    subprocess.run(['git', 'cat-file', '-e', 'HEAD:' + proof_path], cwd=ROOT, check=True)
    message = subprocess.check_output(['git', 'log', '-1', '--format=%s'], cwd=ROOT).decode('utf-8').strip()
    assert message.startswith('match: round 10 ' + label + ' '), message
    added = []
    for family in args.family:
        bundle_path = ROOT / 'reports' / ('round10-' + family + '-patches.json')
        document = json.loads(bundle_path.read_text(encoding='utf-8'))
        item_id = 'd-ready-' + family
        assert not any(item['id'] == item_id for item in cfg['prepared_queue'])
        assert document.get('source_only') is True and document['patches']
        cfg['prepared_queue'].append(dict(
            id=item_id, phase='D', status='pending', readiness='READY_FOR_GAME_PROBE',
            method_ids=[p['method_id'] for p in document['patches']],
            required_work='Fresh model, actual target compiler, all-pending pass, full official verification; no gain assumed',
            report=bundle_path.relative_to(ROOT).as_posix(), report_sha256=sha(bundle_path)))
        added.append(item_id)
    cfg.setdefault('preparation_notes', []).append(
        'After ' + label + ', newly fixture-tested families were added to the prepared queue: ' +
        ', '.join(added) + '. Adding proposals does not promote any method state.')
    cfg_path.write_text(json.dumps(cfg, indent=2) + '\n', encoding='utf-8')
    print(json.dumps({'registered': added, 'method_states_modified': False}))


if __name__ == '__main__':
    main()
