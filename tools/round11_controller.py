"""Read-only Round11 planner and protocol guards. No compiler or state writes."""
import argparse
import hashlib
import json
from pathlib import Path
import sys

ROOT = Path(__file__).resolve().parents[1]
BASE_COMMIT = 'b1116f9635778eaf19218175bb7391ee0adf2a9a'


class GuardError(ValueError):
    pass


def d_rows(journal):
    return [r for r in journal.get('cycles', []) if r.get('phase') == 'D']


def queue_empty(cfg):
    return not any(q['status'] == 'pending' for q in cfg.get('prepared_queue', []))


def measured_zero_streak(journal):
    count = 0
    for row in reversed(d_rows(journal)):
        if not row.get('queue_empty_before') or not row.get('queue_empty'):
            break
        gain = row.get('methods_gained')
        if not isinstance(gain, int) or gain < 0:
            raise GuardError('Invalid measured gain')
        if gain:
            break
        count += 1
    return count


def stop_reason(cfg, journal):
    request = cfg.get('requested_stop', {})
    if (request.get('source') == 'user' and
            request.get('request') == 'me envie o relatorio final' and
            request.get('operation') == 'final_report_requested'):
        return 'user_requested_final_report'
    if len(d_rows(journal)) >= cfg['maximum_cycles']:
        return 'maximum_40_D_cycles'
    if queue_empty(cfg) and measured_zero_streak(journal) >= cfg['consecutive_zero_stop']:
        return 'five_zero_D_cycles_with_queue_empty_before_and_after'
    return None


def validate_config(cfg):
    expected = {'maximum_cycles': 40, 'consecutive_zero_stop': 5,
                'maximum_consecutive_primary_same_method': 2,
                'incomplete_candidate_variants_per_method_per_pass': 12,
                'incomplete_candidate_variants_per_method_per_cycle': 24}
    if any(cfg.get(k) != v for k, v in expected.items()):
        raise GuardError('User protocol constants changed')
    if cfg.get('baseline_commit') != BASE_COMMIT:
        raise GuardError('Wrong Round11 baseline')
    if set(cfg.get('c_fronts', {})) != {f'C{i}' for i in range(1, 7)}:
        raise GuardError('Six explicit C fronts are required')


def initial_item(cfg):
    return next((q for q in cfg['prepared_queue'] if q['status'] == 'pending'
                 and q.get('selection_mode') == 'queue_initial'), None)


def selection_mode(cfg, request, measurement):
    ids = request.get('queue_ids', [])
    first = initial_item(cfg)
    if first:
        if ids != [first['id']] or request['primary_target'] not in first['method_ids']:
            raise GuardError('The ordered explicit initial queue must be audited before discovery')
        if request['assembly'] != first['assembly']:
            raise GuardError('Initial queue assembly does not match its evidence')
        return 'queue_initial'
    if ids:
        pending = {q['id']: q for q in cfg['prepared_queue'] if q['status'] == 'pending'}
        if any(q not in pending for q in ids):
            raise GuardError('Queue item is absent or already consumed')
    if measurement is None:
        raise GuardError('A hash-bound current measurement is required for autonomous priority')
    remaining = measurement['assemblies']['KairoLibrary']['divergent_bodies']
    if remaining > 0 and request['assembly'] != 'KairoLibrary':
        raise GuardError('Autonomous discovery prioritizes Kairo until its bodies close')
    return 'autonomous_selection'


def validate_cycle(cfg, journal, request, measurement=None):
    validate_config(cfg)
    reason = stop_reason(cfg, journal)
    if reason or cfg.get('phase') == 'stopped':
        raise GuardError('Reconstruction stopped: ' + str(reason))
    if request['number'] != len(journal.get('cycles', [])) + 1:
        raise GuardError('Journal number must be next; never repeat a journaled cycle')
    phase = request['phase']
    if phase in ('A', 'B'):
        raise GuardError('A/B record evidence only; they cannot run reconstruction cycles')
    if phase == 'C':
        if d_rows(journal):
            raise GuardError('C cannot bypass an already-started D round')
        if any(cfg['required_parts'].get(p) != 'complete' for p in ('A', 'B')):
            raise GuardError('A/B evidence must be registered before C')
        if request.get('front') not in cfg['c_fronts']:
            raise GuardError('C requires an explicit C1-C6 front')
        return 'c%02d' % (1 + sum(r['phase'] == 'C' for r in journal.get('cycles', [])))
    if phase != 'D':
        raise GuardError('Unknown phase')
    if any(cfg['required_parts'].get(p) != 'complete' for p in 'ABC'):
        raise GuardError('A/B and all six C fronts must precede D')
    if any(f['status'] != 'verified' for f in cfg['c_fronts'].values()):
        raise GuardError('One or more C fronts lack official full verification')
    if request.get('target_substring') or request.get('method_ids'):
        raise GuardError('D main must cover all pending bodies of its assembly')
    rows = d_rows(journal)
    if len(rows) >= 2 and rows[-1]['primary_target'] == rows[-2]['primary_target'] == request['primary_target']:
        raise GuardError('Same primary cannot head three consecutive D cycles')
    selection_mode(cfg, request, measurement)
    return 'cycle%02d' % (len(rows) + 1)


def file_hash(path):
    return hashlib.sha256(Path(path).read_bytes()).hexdigest()


def load(path):
    return json.loads(Path(path).read_text(encoding='utf-8-sig'))


def current_measurement(cfg, journal):
    rows = journal.get('cycles', [])
    path = ROOT / (rows[-1]['after_measurement'] if rows else cfg['baseline_measurement'])
    expected = rows[-1]['after_sha256'] if rows else cfg.get('baseline_measurement_sha256')
    if not expected or not path.is_file() or file_hash(path) != expected:
        raise GuardError('Current measurement is missing or not hash-bound')
    result = load(path)
    if result.get('baseline_commit') != BASE_COMMIT:
        raise GuardError('Measurement belongs to another round')
    return result


def official_identity_map(receipt):
    result = {}
    for key, row in receipt['methods'].items():
        stable = row.get('stable_id')
        if not stable or stable in result:
            raise GuardError('Ambiguous official stable identity')
        result[stable] = (key, row)
    return result


def main():
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument('--config', type=Path, default=ROOT / 'config/round11-protocol.json')
    p.add_argument('--journal', type=Path, default=ROOT / 'reports/round11-cycle-journal.json')
    sub = p.add_subparsers(dest='mode', required=True)
    sub.add_parser('check')
    q = sub.add_parser('plan-cycle')
    q.add_argument('--phase', choices=['C', 'D'], required=True)
    q.add_argument('--front', choices=[f'C{i}' for i in range(1, 7)])
    q.add_argument('--assembly', choices=['KairoLibrary', 'Assembly-CSharp'], required=True)
    q.add_argument('--primary-id', required=True)
    q.add_argument('--probe-id', action='append', default=[])
    q.add_argument('--queue-id', action='append', default=[])
    for name in ('rule', 'pair', 'hypothesis'):
        q.add_argument('--' + name, required=True)
    q.add_argument('--external-proposals')
    q.add_argument('--push', action='store_true')
    q = sub.add_parser('finish-command')
    q.add_argument('--recommendations', required=True)
    q.add_argument('--push', action='store_true')
    args = p.parse_args()
    cfg = load(args.config)
    validate_config(cfg)
    journal = load(args.journal) if args.journal.exists() else {'cycles': []}
    result = dict(review_only=True, executes_commands=False, writes_state=False,
                  D_count=len(d_rows(journal)), zero_streak=measured_zero_streak(journal),
                  prepared_queue_empty=queue_empty(cfg), stop_reason=stop_reason(cfg, journal),
                  input_sha256={str(args.config): file_hash(args.config)})
    if args.journal.exists():
        result['input_sha256'][str(args.journal)] = file_hash(args.journal)
    if args.mode == 'plan-cycle':
        index = official_identity_map(load(ROOT / ('build/verification-' + args.assembly + '.json')))
        probes = [args.primary_id, *args.probe_id]
        if len(set(probes)) != len(probes) or any(x not in index for x in probes):
            raise GuardError('Isolated probe identity absent or duplicate')
        request = dict(number=len(journal['cycles']) + 1, phase=args.phase, front=args.front,
                       assembly=args.assembly, primary_target=args.primary_id, queue_ids=args.queue_id)
        measurement = current_measurement(cfg, journal) if args.phase == 'D' else None
        label = validate_cycle(cfg, journal, request, measurement)
        command = [sys.executable, '-X', 'utf8', 'tools/round11_cycle.py', '--number',
                   str(request['number']), '--phase', args.phase, '--assembly', args.assembly,
                   '--rule', args.rule, '--pair', args.pair, '--hypothesis', args.hypothesis]
        if args.front:
            command += ['--front', args.front]
        for probe in probes:
            command += ['--probe-substring', probe]
        for item in args.queue_id:
            command += ['--queue-id', item]
        if args.external_proposals:
            command += ['--external-proposals', args.external_proposals]
        if args.push:
            command += ['--push']
        result.update(label=label, command=command, primary_ledger_key=index[args.primary_id][0])
        if args.phase == 'D':
            result['selection_mode'] = selection_mode(cfg, request, measurement)
    elif args.mode == 'finish-command':
        if not result['stop_reason']:
            raise GuardError('No measured normal stop')
        result['command'] = [sys.executable, '-X', 'utf8', 'tools/round11_finish.py',
                             '--recommendations', args.recommendations] + (['--push'] if args.push else [])
    print(json.dumps(result, indent=2))


if __name__ == '__main__':
    main()
