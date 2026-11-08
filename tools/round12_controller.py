"""Read-only Round12 planning. Part A is uncapped; Part B has its own 40-cycle cap."""
import argparse
import hashlib
import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
BASE_COMMIT = 'a075dcc8ca7b08e2cb9fcdd0cbcb5ce1fe8cd236'


class GuardError(ValueError):
    pass


def load(path):
    return json.loads(Path(path).read_text(encoding='utf-8-sig'))


def file_hash(path):
    return hashlib.sha256(Path(path).read_bytes()).hexdigest()


def part_rows(journal, phase):
    return [r for r in journal.get('cycles', []) if r.get('phase') == phase]


def queue_empty(cfg, phase='B'):
    return not any(q['status'] == 'pending' and q['phase'] == phase
                   for q in cfg.get('prepared_queue', []))


def measured_zero_streak(journal):
    count = 0
    for row in reversed(part_rows(journal, 'B')):
        if not row.get('queue_empty_before') or not row.get('queue_empty'):
            break
        gain = row.get('methods_gained')
        if not isinstance(gain, int) or gain < 0:
            raise GuardError('Invalid measured Part B gain')
        if gain:
            break
        count += 1
    return count


def stop_reason(cfg, journal):
    if cfg.get('requested_stop', {}).get('source') == 'user':
        return 'explicit_user_stop'
    if len(part_rows(journal, 'B')) >= cfg['maximum_cycles']:
        return 'maximum_40_Part_B_cycles'
    if queue_empty(cfg) and measured_zero_streak(journal) >= cfg['consecutive_zero_stop']:
        return 'five_zero_Part_B_cycles_with_queue_empty_before_and_after'
    return None


def validate_config(cfg):
    expected = {'maximum_cycles': 40, 'consecutive_zero_stop': 5,
                'maximum_consecutive_primary_same_method': 2,
                'incomplete_candidate_variants_per_method_per_pass': 12,
                'incomplete_candidate_variants_per_method_per_cycle': 24}
    if any(cfg.get(k) != v for k, v in expected.items()):
        raise GuardError('Round12 user protocol constants changed')
    if cfg.get('baseline_commit') != BASE_COMMIT:
        raise GuardError('Wrong Round12 baseline')
    if cfg.get('part_a_count_limit') is not None or cfg.get('part_a_zero_stop') is not None:
        raise GuardError('Do not impose Part B stopping rules on Part A')


def semantic_closed(measurement):
    row = measurement['assemblies']['KairoLibrary']
    # A count-only test is insufficient: held equal helpers must be released by verifier.
    return row['divergent_bodies'] == 0 and row['equal_body_held_methods'] == 0


def verify_raw_gate(cfg, measurement):
    if not semantic_closed(measurement):
        raise GuardError('Kairo divergent/held methods still pending')
    gate = cfg.get('part_a_raw_gate', {})
    if gate.get('status') != 'PASS':
        raise GuardError('Part B awaits root exhaustive raw comparison PASS')
    path = ROOT / gate['report']
    if file_hash(path) != gate['sha256']:
        raise GuardError('Root raw report hash changed')
    report = load(path)
    if (report.get('assembly') != 'KairoLibrary' or report.get('status') != 'PASS'
            or any(report.get(k) != 0 for k in ('semantic_divergent_bodies', 'held_methods',
                                               'outside_exceptions_differing_bytes'))
            or report.get('recompiled_sha256') != gate.get('recompiled_sha256')):
        raise GuardError('Root raw report does not prove byte closure outside four exceptions')
    # Bind to the current own frozen PE, not merely a previous passing report.
    snapshot = ROOT / measurement['snapshot'] if measurement.get('snapshot') else ROOT / gate['snapshot']
    artifacts = load(snapshot / 'assemblies/artifacts.json')
    artifact = artifacts['KairoLibrary']
    dll = snapshot / artifact['path']
    if artifact['sha256'] != report['recompiled_sha256'] or file_hash(dll) != artifact['sha256']:
        raise GuardError('Passing raw report belongs to another recompiled PE')
    live = load(ROOT / 'build/verification-KairoLibrary.json')
    from apply_il_rules import receipt_is_fresh
    if not receipt_is_fresh(ROOT, live):
        raise GuardError('Canonical Kairo source/profile no longer matches its verified PE')
    if (live['build_sha256'] != report['recompiled_sha256']
            or file_hash(ROOT / live['build_artifact']) != report['recompiled_sha256']):
        raise GuardError('Canonical Kairo PE changed after its literal raw PASS; renew comparison')
    return report


def initial_item(cfg, phase):
    return next((q for q in cfg.get('prepared_queue', []) if q['status'] == 'pending'
                 and q['phase'] == phase and q.get('selection_mode') == 'queue_initial'), None)


def selection_mode(cfg, request, measurement):
    phase = request['phase']
    if phase == 'A' and request['assembly'] != 'KairoLibrary':
        raise GuardError('Part A compiles KairoLibrary only')
    if phase == 'B':
        verify_raw_gate(cfg, measurement)
        if request['assembly'] != 'Assembly-CSharp':
            raise GuardError('Part B follows Kairo raw closure and targets Assembly-CSharp')
    ids = request.get('queue_ids', [])
    first = initial_item(cfg, phase)
    if first:
        if ids != [first['id']] or request['primary_target'] not in first['method_ids']:
            raise GuardError('Consume the ordered initial queue before autonomous discovery')
        if request['assembly'] != first['assembly']:
            raise GuardError('Initial queue assembly mismatch')
        return 'queue_initial'
    pending = {q['id']: q for q in cfg.get('prepared_queue', []) if q['status'] == 'pending'}
    if any(q not in pending or pending[q]['phase'] != phase for q in ids):
        raise GuardError('Queue item absent, consumed, or belongs to another part')
    return 'autonomous_selection'


def validate_cycle(cfg, journal, request, measurement):
    validate_config(cfg)
    reason = stop_reason(cfg, journal)
    if reason or cfg.get('phase') == 'stopped':
        raise GuardError('Reconstruction stopped: ' + str(reason))
    if request['number'] != len(journal.get('cycles', [])) + 1:
        raise GuardError('Journal number must be next; never repeat a journaled cycle')
    phase = request['phase']
    if phase not in ('A', 'B'):
        raise GuardError('Round12 reconstruction cycles have only Part A or Part B')
    if phase == 'A' and part_rows(journal, 'B'):
        raise GuardError('Cannot return to A after B started')
    if request.get('target_substring') or request.get('method_ids'):
        raise GuardError('Main pass must cover every pending body of its assembly')
    rows = part_rows(journal, phase)
    if len(rows) >= 2 and rows[-1]['primary_target'] == rows[-2]['primary_target'] == request['primary_target']:
        raise GuardError('Same primary cannot head three consecutive cycles in a part')
    if phase == 'A' and semantic_closed(measurement):
        raise GuardError('Semantic closure reached: hand off for root raw test, not another target build')
    selection_mode(cfg, request, measurement)
    return phase.lower() + '%02d' % (len(rows) + 1)


def current_measurement(cfg, journal):
    rows = journal.get('cycles', [])
    raw_source_path = ROOT / 'reports/round12-raw-source-journal.json'
    source_commits = load(raw_source_path).get('commit_provenance', []) if raw_source_path.exists() else []
    pointer = cfg.get('raw_source_current_measurement')
    if pointer and not part_rows(journal, 'B'):
        path = ROOT / pointer['path']
        expected = pointer['sha256']
        if source_commits and (source_commits[-1]['measurement'] != pointer['path']
                              or source_commits[-1]['measurement_sha256'] != expected):
            raise GuardError('Raw-source pointer does not describe latest verified source/PE')
    elif source_commits and not part_rows(journal, 'B'):
        raise GuardError('Explicit hash-bound raw-source current measurement is required before B')
    else:
        path = ROOT / (rows[-1]['after_measurement'] if rows else cfg['baseline_measurement'])
        expected = rows[-1]['after_sha256'] if rows else cfg.get('baseline_measurement_sha256')
    if not expected or file_hash(path) != expected:
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
            raise GuardError('Ambiguous official identity')
        result[stable] = (key, row)
    return result


def main():
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument('--config', type=Path, default=ROOT / 'config/round12-protocol.json')
    args = p.parse_args()
    cfg = load(args.config)
    validate_config(cfg)
    journal_path = ROOT / 'reports/round12-cycle-journal.json'
    journal = load(journal_path) if journal_path.exists() else {'cycles': []}
    print(json.dumps({'review_only': True, 'target_builds': 0,
        'A_count': len(part_rows(journal, 'A')), 'B_count': len(part_rows(journal, 'B')),
        'B_zero_streak': measured_zero_streak(journal), 'stop_reason': stop_reason(cfg, journal)}))


if __name__ == '__main__':
    main()
