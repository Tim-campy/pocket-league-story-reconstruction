"""Source-only, bounded first-IL-divergence search. Never promotes ledger states.

Only metadata token operands are resolved for prefix comparison. Header, local
signature and EH equality are still mandatory for a body to be called equal.
Trial builds remain isolated; the official verifier alone adopts ledger states.
"""
import argparse
from collections import Counter
import csv
import hashlib
import json
import os
from pathlib import Path
import shutil
import subprocess
import sys
import time

from compare_kairo import body_cells, differences, sha
from compiler_probes import MONO, UNITY

ROOT = Path(__file__).resolve().parents[1]


def read(path):
    return json.loads(Path(path).read_text(encoding='utf-8-sig'))


def il_cells(method):
    cells = body_cells(method)
    if cells is None:
        return []
    start = method['header']['size']
    return cells[start:start + method['header']['code_size']]


def prefix(original, rebuilt):
    a, b = il_cells(original), il_cells(rebuilt)
    for i, (x, y) in enumerate(zip(a, b)):
        if x != y:
            return i
    return min(len(a), len(b))


def instruction_at(method, offset):
    items = method.get('instructions', [])
    return next((i for i in reversed(items) if i['offset'] <= offset), None)


def apply_patch(text, patch):
    raw = text.encode('utf-16-le')
    start, end = 2 * patch['start'], 2 * patch['end']
    if raw[start:end].decode('utf-16-le') != patch['before']:
        raise ValueError('Stale syntax patch')
    return (raw[:start] + patch['after'].encode('utf-16-le') + raw[end:]).decode('utf-16-le')


def local_slot(instruction, prefix_op):
    op = instruction['opcode']
    if op.startswith(prefix_op + '.') and op[len(prefix_op) + 1:].isdigit():
        return int(op[len(prefix_op) + 1:])
    if op in (prefix_op, prefix_op + '.s') and isinstance(instruction.get('operand'), dict):
        return instruction['operand'].get('local')
    return None


def syntax_identity():
    return hashlib.sha256((ROOT / 'tools/Round5TransformSyntax.cs').read_bytes() +
                          (ROOT / 'tools/Round5NewRules.cs').read_bytes()).hexdigest()


def alias_hints(original, current, offset, plain_types, type_lookup=None):
    """Use original slot types and actual load/store evidence, never guessed names."""
    lookup = type_lookup or {}
    rebuilt_types = [lookup.get(t, 'UNKNOWN') for t in current.get('locals', [])]
    instructions = original.get('instructions', [])
    at = instruction_at(original, offset)
    slots = []
    if at:
        index = instructions.index(at)
        for ins in instructions[index:index + 3]:
            operand = ins.get('operand')
            if isinstance(operand, dict) and ('branch' in operand or 'switch' in operand):
                break
            if ins['opcode'] in ('ret', 'throw', 'rethrow'):
                break
            slot = local_slot(ins, 'stloc')
            if ins is at and slot is None:
                slot = local_slot(ins, 'ldloc')
            if slot is not None and slot < len(plain_types):
                slots.append(plain_types[slot])
    unused = []
    zero_chains = []
    for i, ins in enumerate(instructions):
        if ins['opcode'] != 'ldc.i4.0':
            continue
        stores, j = [], i + 1
        while j < len(instructions):
            candidate = instructions[j]
            slot = local_slot(candidate, 'stloc')
            if slot is not None:
                stores.append(slot)
            elif candidate['opcode'] != 'dup':
                break
            j += 1
        if len(stores) >= 2:
            end = instructions[j]['offset'] if j < len(instructions) else original.get('header', {}).get('code_size', offset + 1)
            if ins['offset'] <= offset < end:
                zero_chains.append({'offset': ins['offset'], 'slots': stores})
    for n, type_name in enumerate(plain_types):
        loads = sum(local_slot(i, 'ldloc') == n or local_slot(i, 'ldloca') == n for i in instructions)
        stores = sum(local_slot(i, 'stloc') == n for i in instructions)
        if not loads:
            unused.append({'index': n, 'type': type_name, 'loads': loads, 'stores': stores})
    return {'expected_alias_types': list(dict.fromkeys(slots)), 'original_local_types': plain_types,
            'rebuilt_local_types': rebuilt_types, 'original_unused_slots': unused,
            'original_zero_chain_stores': max((len(c['slots']) for c in zero_chains), default=0),
            'original_zero_chains_at_divergence': zero_chains,
            'original_local_count': len(original.get('locals', [])), 'rebuilt_local_count': len(current.get('locals', []))}


class Compiler:
    def __init__(self, assembly, reset=False):
        self.a = assembly
        self.base = ROOT / 'build/round5/baseline' / assembly
        self.folder = ROOT / 'build/round5/engine' / assembly
        self.folder.mkdir(parents=True, exist_ok=True)
        self.manifest = read(self.base / 'build-manifest.json')
        self.paths = {}
        for source in self.manifest['sources']:
            logical = source['path']
            out = self.folder / 'sources' / logical
            out.parent.mkdir(parents=True, exist_ok=True)
            if reset or not out.exists():
                shutil.copyfile(self.base / logical, out)
            self.paths[logical] = out
        self.binary = self.folder / (assembly + '.dll')
        self.last_log = ''
        self.builds = 0
        self.build_seconds = 0

    def build(self, debug=False, screen_paths=None):
        out = self.folder / ('screen' if screen_paths else 'debug' if debug else 'current') / (self.a + '.dll')
        out.parent.mkdir(exist_ok=True)
        is_app = self.a == 'Assembly-CSharp'
        flags = [f for f in self.manifest['flags'] if not f.startswith(('/out:', '-out:')) and f not in ('/noconfig', '-noconfig')]
        if debug and is_app:
            flags = ['-debug+' if f == '-debug-' else f for f in flags]
        flags += [('-out:' if is_app else '/out:') + str(out)]
        args = flags + [('-r:' if is_app else '/reference:') + r['path'] for r in self.manifest['references']]
        paths = screen_paths or self.paths
        args += [str(paths[s['path']]) for s in self.manifest['sources']]
        rsp = out.parent / 'compile.rsp'
        rsp.write_text('\n'.join('"' + x + '"' for x in args) + '\n', encoding='utf-8')
        cmd = ([self.manifest['runtime']] if is_app else []) + [self.manifest['compiler'], '-noconfig' if is_app else '/noconfig']
        # /shared is a client option: Roslyn 1.1 rejects it inside an RSP.
        # The frozen-source control proves it preserves every canonical body.
        if not is_app:
            cmd += ['/shared']
        cmd += ['@' + str(rsp)]
        env = os.environ.copy()
        env.update(self.manifest.get('scoped_environment', {}))
        started = time.monotonic()
        p = subprocess.run(cmd, cwd=ROOT, env=env, capture_output=True)
        self.build_seconds += time.monotonic() - started
        self.builds += 1
        self.last_log = (p.stdout + p.stderr).decode('utf-8', errors='replace')
        (out.parent / 'compiler.log').write_text(self.last_log, encoding='utf-8')
        return out if p.returncode == 0 else None

    def dump(self, binary, ids=None):
        # Token ordinals may change when source emits different generated members.
        selectors = ['--all'] if ids is None else sorted({i.split('(', 1)[0] for i in ids})
        p = subprocess.run([str(ROOT / 'build/tools/ProbeInspector.exe'), str(binary)] + selectors,
                           capture_output=True, cwd=ROOT, check=True)
        return {m['stable_id']: m for m in json.loads(p.stdout.decode('utf-8-sig'))['selected_methods']}


def main():
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument('--assembly', required=True, choices=['KairoLibrary', 'Assembly-CSharp'])
    ap.add_argument('--limit-iterations', type=int, default=16)
    ap.add_argument('--max-candidates', type=int, default=96)
    ap.add_argument('--method-limit', type=int)
    ap.add_argument('--method-ids', type=Path, help='Selected stable IDs; search in descending original IL size')
    ap.add_argument('--resume', action='store_true')
    ap.add_argument('--retry-ids', type=Path, help='Retry selected unfinished methods with an expanded repertoire after a completed pass')
    ap.add_argument('--restore-checkpoint', action='store_true', help='Replay only published accepted edits from frozen source before resuming')
    ap.add_argument('--fast-screen', action='store_true', help='Source-only screening with per-method full-body equivalence gate; full real build still required to accept')
    args = ap.parse_args()
    c = Compiler(args.assembly, reset=not args.resume or args.restore_checkpoint)
    original = {m['stable_id']: m for m in read(c.base / 'original-methods.json')['selected_methods']}
    baseline = {m['stable_id']: m for m in read(c.base / 'rebuilt-methods.json')['selected_methods']}
    initially_equal = {k for k, v in original.items() if 'raw_body_hex' in v and not differences(v, baseline.get(k))[0]}
    targets = [k for k, v in original.items() if 'raw_body_hex' in v and k not in initially_equal]
    if args.method_ids:
        selected_ids = set(read(args.method_ids))
        targets = [key for key in targets if key in selected_ids]
    targets.sort(key=lambda key: (-original[key]['header']['code_size'], key))
    if args.method_limit is not None:
        targets = targets[:args.method_limit]
    report_path = ROOT / 'reports' / ('round5-' + args.assembly + '-engine.json')
    report = {'assembly': args.assembly, 'baseline_body_equal': len(initially_equal),
              'limit_iterations': args.limit_iterations, 'max_candidates_per_step': args.max_candidates,
              'prefix_metric': 'IL bytes with only metadata-token identities resolved; no branch/operand/offset normalization',
              'adopted_ledger_states': False, 'methods': [], 'transformations': {}, 'completed': False}
    report['trial_compiler_server'] = '/shared (CLI only)' if args.assembly == 'KairoLibrary' else 'not applicable'
    if args.resume and report_path.exists():
        report = read(report_path)
        c.builds = report.get('builds', 0)
        c.build_seconds = report.get('build_seconds', 0)
        if args.restore_checkpoint:
            for row in report['methods']:
                if not row['steps']:
                    continue
                logical = (row.get('stop_point') or row.get('last_attempt_point') or {})['source']
                path = c.paths[logical]
                text = path.read_bytes().decode('utf-8-sig')
                for step in row['steps']:
                    text = apply_patch(text, step)
                path.write_text(text, encoding='utf-8', newline='')
        if args.retry_ids:
            if args.restore_checkpoint or not report.get('completed'):
                raise ValueError('Retry requires a completed preserved source checkpoint')
            retry_ids = set(read(args.retry_ids))
            for logical, expected_hash in report['source_hashes'].items():
                if sha(c.paths[logical]) != expected_hash:
                    raise ValueError('Completed checkpoint source changed: ' + logical)
            history = report.setdefault('previous_passes', {})
            for row in report['methods']:
                if row['id'] in retry_ids and not row['body_equal']:
                    history.setdefault(row['id'], []).append(row)
            report['methods'] = [row for row in report['methods'] if row['id'] not in retry_ids or row['body_equal']]
            report['completed'] = False
    done = {m['id'] for m in report['methods']}
    # Helpers are built once and kept alive; they never write source/ledger files.
    from round5_transform import TransformClient
    from round4_symbols import inspect_methods, select_point
    syntax_manifest = dict(c.manifest)
    syntax_manifest['sources'] = [{'path': str(c.paths[s['path']]), 'input_path': str(c.paths[s['path']])} for s in c.manifest['sources']]
    syntax_path = c.folder / 'syntax-manifest.json'
    syntax_path.write_text(json.dumps(syntax_manifest), encoding='utf-8')
    syntax = TransformClient(syntax_path)
    plain_local_types = read(c.base / 'plain-local-types.json') if (c.base / 'plain-local-types.json').exists() else {}
    rebuilt_plain = read(c.base / 'plain-local-types-rebuilt.json') if (c.base / 'plain-local-types-rebuilt.json').exists() else {}
    type_lookup = {scoped: plain for key, method in original.items() for scoped, plain in zip(method.get('locals', []), plain_local_types.get(key, []))}
    type_lookup.update({scoped: plain for key, method in baseline.items() for scoped, plain in zip(method.get('locals', []), rebuilt_plain.get(key, []))})
    syntax_hash = syntax_identity()
    skeletons = None
    if args.fast_screen:
        from prepare_round4_screen import prepare_skeletons
        screen_manifest = dict(c.manifest)
        screen_manifest['sources'] = [dict(s, input_path=str(c.base / s['path'])) for s in c.manifest['sources']]
        screen_manifest_path = c.folder / 'screen-manifest.json'
        screen_manifest_path.write_text(json.dumps(screen_manifest), encoding='utf-8')
        skeletons = prepare_skeletons(screen_manifest_path, c.folder / 'screen-sources')
    report.setdefault('screening', {'baseline_checks': 0, 'baseline_equal': 0, 'baseline_different_or_failed': 0,
                                   'candidates': 0, 'rejected': 0, 'full_positive_checks': 0, 'false_positive_prefixes': 0, 'batches': []})
    now_binary = c.build()
    if now_binary is None:
        raise RuntimeError('Initial isolated build failed: ' + c.last_log[-2000:])
    current_all = c.dump(now_binary)
    if initially_equal - {k for k in initially_equal if not differences(original[k], current_all.get(k))[0]}:
        raise RuntimeError('Initial body regression')
    eq = {k for k, v in original.items() if 'raw_body_hex' in v and not differences(v, current_all.get(k))[0]}
    if set(report.get('body_gains', [])) - eq:
        raise ValueError('A prior confirmed body gain did not survive checkpoint reload')
    counters = Counter(report.get('transformations', {}))
    for number, key in enumerate(targets, 1):
        if key in done:
            continue
        method = current_all.get(key)
        seen_sources = set()
        row = {'id': key, 'number': number, 'initial_offset': prefix(original[key], method),
               'steps': [], 'trials': [], 'mapping': None, 'stop_reason': None}
        method_snapshot = {p: path.read_bytes() for p, path in c.paths.items()}
        method_start_eq = set(eq)
        for iteration in range(args.limit_iterations):
            old_prefix = prefix(original[key], method)
            if not differences(original[key], method)[0]:
                row['stop_reason'] = 'body_equal'
                break
            # Failed trials overwrite the isolated binary, so rebuild the current
            # accepted source before acquiring its own symbol mapping.
            if args.assembly == 'KairoLibrary':
                now_binary = c.build()
                if now_binary is None:
                    raise RuntimeError('Accepted source stopped compiling')
                symbol_method = next(iter(inspect_methods(now_binary, [key], symbols=True)['methods']), None)
                point = select_point(symbol_method, old_prefix) if symbol_method else None
                mapping_kind = 'native-PDB-sequence-point'
            else:
                debug_binary = c.build(debug=True)
                debug_method = next(iter(inspect_methods(debug_binary, [key], symbols=True)['methods']), None) if debug_binary else None
                if debug_method and not differences(method, debug_method)[0]:
                    point = select_point(debug_method, old_prefix)
                    mapping_kind = 'MDB-full-body-equal-debug-plus-vs-minus'
                else:
                    point = None
                    mapping_kind = 'MDB-debug-body-differed-UNKNOWN'
            row['mapping'] = mapping_kind
            if not point or not point.get('document'):
                row['stop_reason'] = 'source_mapping_UNKNOWN'
                row['mapping_detail'] = point
                break
            physical = Path(point['document']).resolve()
            logical = next((p for p, v in c.paths.items() if v.resolve() == physical), None)
            if logical is None:
                row['stop_reason'] = 'source_document_UNKNOWN'
                row['mapping_detail'] = point
                break
            text = c.paths[logical].read_bytes().decode('utf-8-sig')
            new_syntax_hash = syntax_identity()
            if new_syntax_hash != syntax_hash:
                syntax.close()
                syntax = TransformClient(syntax_path)
                syntax_hash = new_syntax_hash
            response = syntax.request(path=str(c.paths[logical]), source=text, line=point['start_line'], column=max(1, point['start_column']), method_id=key, max_candidates=512, **alias_hints(original[key], method, old_prefix, plain_local_types.get(key, []), type_lookup))
            row['transform_helper_sha256'] = syntax_hash
            row['stop_point'] = {'offset': old_prefix, 'source': logical, 'point': point, 'statement': response.get('mapped')}
            if response.get('error') or not response.get('mapped'):
                row['stop_reason'] = 'syntax_mapping_UNKNOWN'
                row['mapping_detail'] = response
                break
            patches = response.get('patches', [])
            # Use the actual first pair to prioritize a finite repertoire. Do not
            # discard branch-operand differences from the acceptance criterion.
            oi, ri = instruction_at(original[key], old_prefix), instruction_at(method, old_prefix)
            prefer = ['restore-zero-chain-local', 'reserve-unused-slot-and-defer-local',
                      'foreach-array-indexed-for', 'alias-parameter-block-tail',
                      'restore-dead-zero-chain', 'alias-parameter-command', 'alias-field-command',
                      'reserve-unused-original-slot']
            if oi and oi['opcode'] == 'neg':
                prefer = ['unary-negation'] + prefer
            elif oi and oi['opcode'].startswith('stloc'):
                prefer += ['assignment-chain', 'condition-local', 'return-local']
            elif oi and oi['opcode'] in ('br', 'br.s'):
                prefer += ['continue-guard', 'break-guard', 'guard-return', 'split-condition']
            patches.sort(key=lambda p: (next((i for i, r in enumerate(prefer) if r in p['rule']), len(prefer)), p.get('distance', 0), p['end'] - p['start']))
            # Try each transformation family before using the remaining budget
            # on further individual occurrences in this mapped command.
            first_by_rule = {}
            for patch in patches:
                first_by_rule.setdefault(patch['rule'], patch)
            first_ids = {id(p) for p in first_by_rule.values()}
            patches = list(first_by_rule.values()) + [p for p in patches if id(p) not in first_ids]
            screen_paths = None
            source_filter = None
            if skeletons:
                proposed_screen = {p: c.paths[p] if p == logical else Path(skeletons[p]) for p in c.paths}
                declaration_start = response.get('declaration_start')
                if declaration_start is not None:
                    # The kept method is real source, including its lambdas;
                    # other bodies are source-only screening placeholders.
                    target_screen = c.folder / 'target-screen' / Path(logical).name
                    target_screen.parent.mkdir(exist_ok=True)
                    target_screen.write_text(syntax.screen_source(text, declaration_start), encoding='utf-8', newline='')
                    proposed_screen[logical] = target_screen
                    source_filter = lambda candidate, offset=declaration_start: syntax.screen_source(candidate, offset)
                check = c.build(screen_paths=proposed_screen)
                checked_method = c.dump(check, [key]).get(key) if check else None
                report['screening']['baseline_checks'] += 1
                if checked_method and not differences(method, checked_method)[0]:
                    screen_paths = proposed_screen
                    report['screening']['baseline_equal'] += 1
                else:
                    report['screening']['baseline_different_or_failed'] += 1
                row['screen_baseline_equal'] = screen_paths is not None
            progressed = False
            tested = 0
            pool = None
            pool_indices = {}
            if screen_paths:
                from round4_trial_pool import TrialPool
                batch_folder = c.folder / 'screen-pools' / ('method-%03d-step-%02d-%d' % (number, iteration, time.time_ns()))
                pool = TrialPool(c.manifest, args.assembly, screen_paths, logical, batch_folder, max_workers=3)
                eligible = []
                eligible_hashes = set()
                for patch in patches:
                    candidate_hash = hashlib.sha256(apply_patch(text, patch).encode('utf-8')).hexdigest()
                    if (logical, candidate_hash) not in seen_sources and candidate_hash not in eligible_hashes and len(eligible) < args.max_candidates:
                        eligible.append(patch)
                        eligible_hashes.add(candidate_hash)
                indices = pool.submit(eligible, text, source_filter=source_filter)
                pool_indices = {id(p): index for p, index in zip(eligible, indices)}
            for patch in patches:
                candidate = apply_patch(text, patch)
                source_key = (logical, hashlib.sha256(candidate.encode('utf-8')).hexdigest())
                if source_key in seen_sources:
                    continue
                seen_sources.add(source_key)
                if tested >= args.max_candidates:
                    break
                tested += 1
                c.paths[logical].write_text(candidate, encoding='utf-8', newline='')
                pool_result = pool.result(pool_indices[id(patch)]) if pool else None
                built = pool_result['binary'] if pool_result else c.build(screen_paths=screen_paths)
                trial = {'iteration': iteration + 1, 'rule': patch['rule'], 'start': patch['start'], 'end': patch['end'],
                         'before': patch['before'], 'after': patch['after'], 'compile_pass': built is not None,
                         'old_prefix': old_prefix, 'accepted': False}
                trial['screen_used'] = screen_paths is not None
                if pool_result:
                    trial['screen_log_path'] = pool_result['log_path']
                    trial['screen_build_seconds'] = pool_result['seconds']
                counters[patch['rule'] + ':attempts'] += 1
                if built:
                    newmethod = c.dump(built, [key]).get(key)
                    if screen_paths:
                        report['screening']['candidates'] += 1
                        screen_prefix = prefix(original[key], newmethod) if newmethod else -1
                        screen_exact = newmethod is not None and not differences(original[key], newmethod)[0]
                        trial.update(screen_prefix=screen_prefix, screen_body_equal=screen_exact)
                        if screen_prefix > old_prefix or screen_exact:
                            report['screening']['full_positive_checks'] += 1
                            built = c.build()
                            newmethod = c.dump(built, [key]).get(key) if built else None
                            trial['full_compile_pass'] = built is not None
                            if not built:
                                trial['full_compiler_error'] = c.last_log[-3000:]
                            if newmethod and prefix(original[key], newmethod) <= old_prefix and differences(original[key], newmethod)[0]:
                                report['screening']['false_positive_prefixes'] += 1
                        else:
                            report['screening']['rejected'] += 1
                            trial['screen_rejected'] = True
                            newmethod = None
                    if newmethod:
                        new_prefix = prefix(original[key], newmethod)
                        exact = not differences(original[key], newmethod)[0]
                        trial.update(new_prefix=new_prefix, body_equal=exact)
                        if (new_prefix > old_prefix or exact) and il_cells(newmethod)[:old_prefix] == il_cells(original[key])[:old_prefix]:
                            # Require all previously equal bodies, including gains
                            # of earlier methods, to remain equal after every step.
                            all_methods = c.dump(built)
                            next_eq = {k for k, v in original.items() if 'raw_body_hex' in v and not differences(v, all_methods.get(k))[0]}
                            losses = sorted(eq - next_eq)
                            trial['body_regressions'] = losses
                            if not losses:
                                from metadata_compare import compare_paths
                                meta = compare_paths(ROOT / 'original/extracted/assets/bin/Data/Managed' / (args.assembly + '.dll'), built,
                                                     c.folder / 'step-metadata.json')
                                trial['metadata_types_equal'] = meta['types_equal']
                                trial['metadata_whole_structural_equal'] = meta['whole_structural_metadata_equal']
                                if not meta['whole_structural_metadata_equal']:
                                    trial['metadata_rejected'] = True
                                    row['trials'].append(trial)
                                    c.paths[logical].write_text(text, encoding='utf-8', newline='')
                                    continue
                                trial['accepted'] = True
                                trial['new_equal_ids'] = sorted(next_eq - eq)
                                trial['matched_IL_gain'] = sum(original[k]['header']['code_size'] for k in next_eq - eq)
                                counters[patch['rule'] + ':accepted_steps'] += 1
                                counters[patch['rule'] + ':body_gains'] += len(next_eq - eq)
                                counters[patch['rule'] + ':IL_gains'] += trial['matched_IL_gain']
                                eq, current_all, method = next_eq, all_methods, newmethod
                                row['steps'].append(trial.copy())
                                progressed = True
                else:
                    trial['compiler_error'] = (pool_result['log'] if pool_result else c.last_log)[-3000:]
                row['trials'].append(trial)
                if progressed:
                    break
                c.paths[logical].write_text(text, encoding='utf-8', newline='')
            if pool:
                pool_summary = pool.close()
                report['screening']['batches'].append({'method_id': key, 'iteration': iteration + 1, 'folder': str(batch_folder),
                                                      **{k: v for k, v in pool_summary.items() if k != 'statuses'},
                                                      'statuses': [{k: v for k, v in status.items() if k != 'compiler_log'} for status in pool_summary['statuses']]})
                c.builds += pool_summary['finished']
                c.build_seconds += pool_summary['seconds']
            if not progressed:
                row['stop_reason'] = 'candidate_limit' if tested >= args.max_candidates and len(patches) > tested else 'no_transformation_advances'
                row['candidate_count'] = len(patches)
                break
        if row['stop_reason'] is None:
            row['stop_reason'] = 'body_equal' if not differences(original[key], method)[0] else 'iteration_limit'
            if row['stop_reason'] == 'iteration_limit':
                row['last_attempt_point'] = row.pop('stop_point', None)
                row['stop_point'] = {'offset': prefix(original[key], method), 'statement': 'UNKNOWN: iteration limit before remapping'}
        row['final_offset'] = prefix(original[key], method)
        row['body_equal'] = not differences(original[key], method)[0]
        row['body_gains'] = sorted(eq - method_start_eq)
        row['body_losses'] = sorted(method_start_eq - eq)
        if row['body_losses']:
            for p, data in method_snapshot.items():
                c.paths[p].write_bytes(data)
            raise RuntimeError('Body regression escaped acceptance check')
        report['methods'].append(row)
        report['transformations'] = dict(counters)
        report.update(body_equal=len(eq), body_gains=sorted(eq - initially_equal), body_losses=sorted(initially_equal - eq),
                      matched_IL_gains=sum(original[k]['header']['code_size'] for k in eq - initially_equal),
                      builds=c.builds, build_seconds=c.build_seconds,
                      iteration_limit_hits=sum(m['stop_reason'] == 'iteration_limit' for m in report['methods']))
        report_path.write_text(json.dumps(report, indent=2, ensure_ascii=True) + '\n', encoding='utf-8')
        print(json.dumps({'assembly': args.assembly, 'processed': number, 'total': len(targets), 'body_gains': len(eq - initially_equal),
                          'IL_gains': report['matched_IL_gains'], 'offset': row['final_offset'], 'stop': row['stop_reason']}), flush=True)
    syntax.close()
    final = c.build()
    final_methods = c.dump(final)
    (c.folder / 'final-methods.json').write_text(json.dumps({'selected_methods': list(final_methods.values())}), encoding='utf-8')
    mapping = {p: str(path) for p, path in c.paths.items() if path.read_bytes() != (c.base / p).read_bytes()}
    (c.folder / 'source-map.json').write_text(json.dumps(mapping, indent=2), encoding='utf-8')
    from metadata_compare import compare_paths
    metadata = compare_paths(ROOT / 'original/extracted/assets/bin/Data/Managed' / (args.assembly + '.dll'), final,
                             c.folder / 'metadata.json')
    report.update(completed=True, builds=c.builds, build_seconds=c.build_seconds, final_binary=str(final), final_sha256=sha(final), source_map=str(c.folder / 'source-map.json'),
                  metadata_types_equal=metadata['types_equal'], metadata_types_total=metadata['types_total'],
                  metadata_whole_structural_equal=metadata['whole_structural_metadata_equal'])
    report['adoption_eligible'] = not report.get('body_losses') and metadata['whole_structural_metadata_equal']
    report['baseline_manifest_sha256'] = sha(c.base / 'build-manifest.json')
    report['source_hashes'] = {p: sha(path) for p, path in c.paths.items()}
    report_path.write_text(json.dumps(report, indent=2) + '\n', encoding='utf-8')


if __name__ == '__main__':
    main()
