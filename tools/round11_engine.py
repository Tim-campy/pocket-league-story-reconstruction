"""Source-only, bounded first-IL-divergence search. Never promotes ledger states.

Only metadata token operands are resolved for prefix comparison. Header, local
signature and EH equality are still mandatory for a body to be called equal.
Trial builds remain isolated; the official verifier alone adopts ledger states.
"""
import argparse
import copy
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
import importlib
from round10_model_policy import prediction_allows_compilation,compiled_signature_allows_search,reserve_incomplete_variant

from compare_kairo import body_cells, differences, sha
from compiler_probes import MONO, UNITY
from round6_alignment import align_methods

ROOT = Path(__file__).resolve().parents[1]
ROUND7_PRIORITY = ('tested-value-local', 'unified-result-return', 'void-common-exit',
                  'new-object-local-before-field', 'new-object-argument-local', 'extended-original-zero-chain',
                  'array-index-loop-foreach', 'struct-fieldwise-no-initializer')


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
    from prepare_round10_transform import identity
    return identity()


def model_approval(prediction, expected_types):
    return prediction_allows_compilation(prediction,expected_types)


def cycle_candidates(patches, cycle_rule=None, accepted_steps=0):
    """A fresh cycle must first advance its new hypothesis, then may close tails.

    Filtering precedes both the source-model gate and any candidate build.
    Family variants share the selected rule's prefix. No input is mutated.
    """
    if not cycle_rule or accepted_steps:
        return list(patches)
    return [patch for patch in patches if patch.get('rule', '').startswith(cycle_rule)]


class AcceptedSymbolCache:
    """Read-only symbol snapshots; never an oracle for candidate acceptance.

    One ALL-method dump belongs to one accepted-source generation. Failed trials
    do not change that generation, even if their binary overwrites the same path.
    A caller receives a copy, so it cannot mutate the stored method or its IL.
    """
    def __init__(self, loader, source_proof):
        self.loader, self.source_proof = loader, source_proof
        self.generation = 0
        self._methods = None
        self.stats = {'enabled': True, 'generation': 0, 'requests': 0, 'hits': 0,
                      'loads': 0, 'load_failures': 0, 'missing_methods': 0,
                      'MDB_body_mismatches': 0, 'accepted_invalidations': 0,
                      'generation_proofs': [], 'invalidations': [],
                      'oracle_use': False, 'mutable_binary_paths_used_on_hit': False}

    def get(self, key, current_method=None, require_equal_body=False):
        self.stats['requests'] += 1
        if self._methods is None:
            proof = copy.deepcopy(self.source_proof())
            result = self.loader()
            if result is None:
                self.stats['load_failures'] += 1
                return None
            methods = result.get('methods', [])
            snapshot = {method['stable_id']: copy.deepcopy(method) for method in methods}
            if len(snapshot) != len(methods):
                raise ValueError('Duplicate stable IDs in ALL-method symbol inventory')
            self._methods = snapshot
            self.stats['loads'] += 1
            self.stats['generation_proofs'].append(dict(proof, generation=self.generation,
                method_count=len(snapshot), symbol_kind=result.get('symbol_kind', 'UNKNOWN'),
                binary_sha256=result.get('mapping_binary_sha256', 'UNKNOWN')))
        else:
            self.stats['hits'] += 1
        method = self._methods.get(key)
        if method is None:
            self.stats['missing_methods'] += 1
            return None
        # This is the pre-existing MDB eligibility check against the live
        # accepted method, including its header, locals, EH and every IL byte.
        if require_equal_body and (current_method is None or differences(current_method, method)[0]):
            self.stats['MDB_body_mismatches'] += 1
            return None
        return copy.deepcopy(method)

    def accepted(self, method_id, rule):
        """Called only after the independent full body/metadata gates accept."""
        self.generation += 1
        self._methods = None
        self.stats['generation'] = self.generation
        self.stats['accepted_invalidations'] += 1
        self.stats['invalidations'].append({'generation': self.generation,
            'accepted_method': method_id, 'rule': rule, 'full_verification_passed': True})


def aligned_progress(original, before, after):
    """Alignment selects source only. The raw prefix still decides progress."""
    old_prefix, new_prefix = prefix(original, before), prefix(original, after)
    return ((new_prefix > old_prefix or not differences(original, after)[0])
            and il_cells(after)[:old_prefix] == il_cells(original)[:old_prefix])


def search_progress(original, before, after, allow_labelled=False):
    """Partial search may advance labelled instructions, never claim equality.

    A forward branch depends on all later instruction widths. Repairing one
    later edit may leave its raw operand unequal until the other edits close.
    Keep every previously identical raw byte; in the opt-in mode additionally
    allow the first real aligned edit to move strictly forward. Body acceptance
    and regression protection still use the independent, unmodified oracle.
    """
    if aligned_progress(original, before, after):
        return True
    if not allow_labelled or after is None:
        return False
    old_prefix = prefix(original, before)
    if prefix(original, after) < old_prefix or il_cells(after)[:old_prefix] != il_cells(original)[:old_prefix]:
        return False
    old_alignment, new_alignment = align_methods(original, before), align_methods(original, after)
    return (old_alignment['kind'] != 'aligned_identical'
            and new_alignment['original_index'] > old_alignment['original_index'])


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
    def __init__(self, assembly, reset=False, baseline_dir='build/round11/baseline', run_label='part-a'):
        self.a = assembly
        self.base = ROOT / baseline_dir / assembly
        self.folder = ROOT / 'build/round11/runs' / run_label / 'engine' / assembly
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
        if p.returncode == 0 and not is_app:
            from round11_pdb_mirror import mirror
            mirror(out, flags)
        return out if p.returncode == 0 else None

    def dump(self, binary, ids=None):
        # Token ordinals may change when source emits different generated members.
        # Cecil MethodDefinition.Name omits the stable-ID generic arity suffix.
        selectors = ['--all'] if ids is None else sorted({i.split('(', 1)[0].split('``', 1)[0] for i in ids})
        p = subprocess.run([str(ROOT / 'build/tools/ProbeInspector.exe'), str(binary)] + selectors,
                           capture_output=True, cwd=ROOT, check=True)
        return {m['stable_id']: m for m in json.loads(p.stdout.decode('utf-8-sig'))['selected_methods']}


def main():
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument('--assembly', required=True, choices=['KairoLibrary', 'Assembly-CSharp'])
    ap.add_argument('--run-label', default='part-a')
    ap.add_argument('--baseline-dir', default='build/round11/baseline')
    ap.add_argument('--cycle-rule', help='Only this rule family may start progress; after its first accepted step all rules may close the method')
    ap.add_argument('--isolate-family',action='store_true',help='Only selected family for every occurrence; no composition with inherited rules')
    ap.add_argument('--cache-symbol-mapping', action='store_true',
                    help='Cache one ALL-method PDB/MDB dump per accepted-source generation; candidate body verification remains live')
    ap.add_argument('--limit-iterations', type=int, default=16)
    ap.add_argument('--max-candidates', type=int, default=96)
    ap.add_argument('--max-model-candidates', type=int, default=192,
                    help='Source-only predictions per step; no unapproved candidate is compiled')
    ap.add_argument('--max-incomplete-candidates',type=int,default=12,
                    help='Incomplete-prediction candidate variants actually compiled per method across all iterations')
    ap.add_argument('--method-limit', type=int)
    ap.add_argument('--method-ids', type=Path, help='Selected stable IDs; search in descending original IL size')
    ap.add_argument('--calculated-alternatives', type=Path,
                    help='Rebase model-approved calculated baseline variants onto each current method')
    ap.add_argument('--external-proposals', type=Path, help='Hash-bound source-only calculated declaration patches')
    ap.add_argument('--calculated-parameter-aliases', action='store_true',
                    help='Calculate missing readonly aliases and their exact rebuilt use points')
    ap.add_argument('--aligned-search-progress', action='store_true',
                    help='Advance labelled instruction prefix while preserving the raw equal prefix; never changes the body oracle')
    ap.add_argument('--resume', action='store_true')
    ap.add_argument('--retry-ids', type=Path, help='Retry selected unfinished methods with an expanded repertoire after a completed pass')
    ap.add_argument('--restore-checkpoint', action='store_true', help='Replay only published accepted edits from frozen source before resuming')
    ap.add_argument('--fast-screen', action='store_true', help='Source-only screening with per-method full-body equivalence gate; full real build still required to accept')
    args = ap.parse_args()
    if args.max_incomplete_candidates<0:raise ValueError('Negative incomplete candidate budget')
    if not args.run_label or any(char not in 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_' for char in args.run_label):
        raise ValueError('run-label must contain only letters, digits, hyphen or underscore')
    is_smcs = args.assembly == 'Assembly-CSharp'
    c = Compiler(args.assembly, reset=not args.resume or args.restore_checkpoint,
                 baseline_dir=args.baseline_dir, run_label=args.run_label)
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
    report_path = ROOT / 'reports' / ('round11-' + args.run_label + '-' + args.assembly + '-engine.json')
    report = {'assembly': args.assembly, 'baseline_body_equal': len(initially_equal),
              'limit_iterations': args.limit_iterations, 'max_candidates_per_step': args.max_candidates,
              'max_model_predictions_per_step': args.max_model_candidates,
              'prefix_metric': 'IL bytes with only metadata-token identities resolved; no branch/operand/offset normalization',
              'adopted_ledger_states': False, 'methods': [], 'transformations': {}, 'completed': False,
              'run_label':args.run_label, 'baseline_dir':str(c.base), 'cycle_rule':args.cycle_rule}
    report['trial_compiler_server'] = '/shared (CLI only)' if args.assembly == 'KairoLibrary' else 'not applicable'
    report['model_policy']={'complete_wrong_signature_rejected':True,'incomplete_prediction_compilation_allowed':True,
        'incomplete_variants_per_method':args.max_incomplete_candidates,'budget_scope':'all iterations of this method in this cycle',
        'actual_original_signature_required':True,'body_oracle_unchanged':True}
    if args.resume and report_path.exists():
        report = read(report_path)
        report.setdefault('search_passes', [{'phase': 'initial',
            'limit_iterations': report['limit_iterations'], 'max_candidates_per_step': report['max_candidates_per_step'],
            'max_model_predictions_per_step': report.get('max_model_predictions_per_step', 0)}])
        report['search_passes'].append({'phase': 'resume', 'limit_iterations': args.limit_iterations,
            'max_candidates_per_step': args.max_candidates, 'max_model_predictions_per_step': args.max_model_candidates,
            'retry_ids': read(args.retry_ids) if args.retry_ids else [],
            'calculated_alternatives': str(args.calculated_alternatives) if args.calculated_alternatives else None,
            'aligned_search_progress': args.aligned_search_progress, 'cycle_rule':args.cycle_rule,
            'cache_symbol_mapping':args.cache_symbol_mapping})
        report.update(limit_iterations=args.limit_iterations, max_candidates_per_step=args.max_candidates,
                      max_model_predictions_per_step=args.max_model_candidates)
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
    report['cycle_rule']=args.cycle_rule
    learned_rules=[]
    if args.cycle_rule:
        for journal_name in ('round7-cycle-journal.json','round8-cycle-journal.json','round11-cycle-journal.json'):
            journal_path=ROOT/'reports'/journal_name
            if journal_path.exists():
                learned_rules += [r['rule'] for r in read(journal_path)['cycles'] if r.get('rule') and r.get('rule_accepted_steps',0)>0]
        learned_rules=list(dict.fromkeys(learned_rules+[args.cycle_rule]))
    report['learned_cycle_rules']=learned_rules
    report['cycle_repertoire_policy']={
        'selected_rule_prefix':args.cycle_rule,
        'before_first_accepted_step':'only selected rule family' if args.cycle_rule else 'all repertoire',
        'after_first_accepted_step':'all repertoire',
        'filter_before_model_gate_and_candidate_build':True,
        'all_pending_methods_still_mapped_and_evaluated':True}
    report.setdefault('cycle_repertoire_counts', {'filtered_steps':0, 'removed_legacy_candidates':0,
                                                 'kept_selected_candidates':0})
    # Helpers are built once and kept alive; they never write source/ledger files.
    from round10_transform import TransformClient
    from round4_symbols import inspect_methods, select_point
    syntax_manifest = dict(c.manifest)
    syntax_manifest['sources'] = [{'path': str(c.paths[s['path']]), 'input_path': str(c.paths[s['path']])} for s in c.manifest['sources']]
    syntax_path = c.folder / 'syntax-manifest.json'
    syntax_path.write_text(json.dumps(syntax_manifest), encoding='utf-8')
    syntax = TransformClient(syntax_path)
    model_config=read(ROOT/'config/round11-protocol.json')['models'][args.assembly]
    SlotModelClient=importlib.import_module(model_config['module']).SlotModelClient
    validation_path=ROOT/model_config['validation'];model_source=ROOT/model_config['source']
    model_client=ROOT/model_config['client'];model_solver=ROOT/model_config['solver']
    validation = read(validation_path)
    if (validation['source_sha256'] != sha(model_source)
            or validation['client_sha256'] != sha(model_client)
            or validation['matched_model_complete'] != validation['matched_complete_exact']):
        raise ValueError('Slot model has stale or incorrect complete predictions')
    model_identity = {'validation_sha256': sha(validation_path),
        'source_sha256': validation['source_sha256'], 'client_sha256': validation['client_sha256'],
        'solver_sha256': sha(model_solver)}
    if report.get('slot_model_identity') and report['slot_model_identity'] != model_identity:
        report.setdefault('slot_model_identity_history', []).append(report['slot_model_identity'])
    report['slot_model_identity'] = model_identity
    slot_model = SlotModelClient(syntax_path)
    external=[]
    if args.external_proposals:
        proposal_doc=read(args.external_proposals)
        dependencies=dict(proposal_doc.get('dependency_sha256',{}))
        for proposal in proposal_doc['patches']:
            dependencies.update(proposal.get('dependency_sha256',{}))
        for relative,expected in dependencies.items():
            dependency=(ROOT/relative).resolve()
            if not dependency.is_relative_to(ROOT/'tools') or sha(dependency)!=expected:
                raise ValueError('External proposal dependency changed: '+relative)
        evidence=dict(proposal_doc.get('evidence_sha256',{}))
        for proposal in proposal_doc['patches']:
            evidence.update(proposal.get('evidence_sha256',{}))
        for relative,expected in evidence.items():
            path=(ROOT/relative).resolve()
            if not path.is_relative_to((ROOT/'reports').resolve()) or path.suffix.lower()!='.json' or sha(path)!=expected:
                raise ValueError('External read-only evidence changed: '+relative)
        frozen_evidence=dict(proposal_doc.get('frozen_evidence_sha256',{}))
        for proposal in proposal_doc['patches']:
            frozen_evidence.update(proposal.get('frozen_evidence_sha256',{}))
        for relative,expected in frozen_evidence.items():
            path=(ROOT/relative).resolve()
            if not path.is_relative_to((ROOT/'build/round11').resolve()) or sha(path)!=expected:
                raise ValueError('External frozen round11 evidence changed: '+relative)
        historical=dict(proposal_doc.get('historical_frozen_evidence_sha256',{}))
        for proposal in proposal_doc['patches']:
            historical.update(proposal.get('historical_frozen_evidence_sha256',{}))
        for relative,expected in historical.items():
            path=(ROOT/relative).resolve()
            if not path.is_relative_to((ROOT/'build/round10/final-corpus').resolve()) or path.suffix!='.json' or sha(path)!=expected:
                raise ValueError('Read-only historical JSON evidence changed: '+relative)
        for field in ('calculator','syntax_source'):
            expected=proposal_doc.get(field+'_sha256')
            if expected and not any(sha(p)==expected for p in (ROOT/'tools').glob('*') if p.is_file()):
                raise ValueError('External proposal producer missing: '+field)
        external=proposal_doc['patches']
        report['external_proposals']={'path':str(args.external_proposals),'sha256':sha(args.external_proposals),
            'dependencies_checked':dependencies,'read_only_evidence_checked':evidence,
            'frozen_evidence_checked':frozen_evidence,
            'historical_frozen_evidence_checked':historical,
            'fresh_model_required':True,'exact_unique_rebase':True}
    baseline_variants = {}
    if args.calculated_alternatives:
        if not is_smcs:
            raise ValueError('Calculated alternatives belong to the smcs model')
        alternative_doc = read(args.calculated_alternatives)
        identity = report['slot_model_identity']
        if (alternative_doc['model_source_sha256'] != identity['source_sha256']
                or alternative_doc['client_sha256'] != identity['client_sha256']
                or alternative_doc['solver_sha256'] != identity['solver_sha256']
                or alternative_doc.get('completed') is not True):
            raise ValueError('Calculated baseline alternatives are stale or incomplete')
        for variant in alternative_doc['variants']:
            if sha(c.base / variant['logical_path']) != variant['baseline_source_sha256']:
                raise ValueError('Calculated alternative uses another baseline source')
            baseline_variants.setdefault(variant['method_id'], []).append(variant)
        report['calculated_alternatives'] = {'path': str(args.calculated_alternatives),
            'sha256': sha(args.calculated_alternatives), 'methods': len(baseline_variants),
            'variants': len(alternative_doc['variants']), 'diagnostic_only_until_fresh_model_and_compilation': True}
    if args.calculated_parameter_aliases:
        if not is_smcs:
            raise ValueError('Calculated parameter aliases require the smcs model')
        report['calculated_parameter_aliases'] = {
            'calculator_sha256': sha(ROOT / 'tools/round6_alias_calculus.py'),
            'syntax_rule_sha256': sha(ROOT / 'tools/Round7AliasRules.cs'),
            'fresh_equal_body_symbols_required': True}
    if args.aligned_search_progress:
        report['aligned_partial_search'] = {
            'raw_equal_prefix_must_not_regress': True,
            'first_real_aligned_original_instruction_must_advance': True,
            'body_oracle_unchanged': True,
            'scope': 'only resume passes explicitly enabling --aligned-search-progress'}
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
    report.setdefault('slot_model_gate', {'predicted': 0, 'approved': 0, 'incomplete': 0,
                                          'wrong_signature': 0, 'post_compile_disagreements': 0})
    now_binary = c.build()
    if now_binary is None:
        raise RuntimeError('Initial isolated build failed: ' + c.last_log[-2000:])
    current_all = c.dump(now_binary)
    if initially_equal - {k for k in initially_equal if not differences(original[k], current_all.get(k))[0]}:
        raise RuntimeError('Initial body regression')
    eq = {k for k, v in original.items() if 'raw_body_hex' in v and not differences(v, current_all.get(k))[0]}
    if set(report.get('body_gains', [])) - eq:
        raise ValueError('A prior confirmed body gain did not survive checkpoint reload')
    symbol_cache = None
    if args.cache_symbol_mapping:
        def mapping_source_proof():
            hashes = {logical: sha(path) for logical, path in c.paths.items()}
            payload = {'sources': hashes, 'manifest_sha256': sha(c.base / 'build-manifest.json')}
            return dict(payload, accepted_source_set_sha256=hashlib.sha256(
                json.dumps(payload, sort_keys=True, separators=(',', ':')).encode()).hexdigest())

        def load_accepted_symbols():
            binary = c.build(debug=is_smcs)
            if binary is None:
                if not is_smcs:
                    raise RuntimeError('Accepted source stopped compiling')
                return None
            # No selectors means ALL methods, as verified against the independent
            # fixture inventory. This data is used solely to locate source.
            symbols = inspect_methods(binary, None, symbols=True)
            symbols['mapping_binary_sha256'] = sha(binary)
            return symbols

        symbol_cache = AcceptedSymbolCache(load_accepted_symbols, mapping_source_proof)
        if report.get('symbol_mapping_cache'):
            report.setdefault('previous_symbol_mapping_caches', []).append(report['symbol_mapping_cache'])
        report['symbol_mapping_cache'] = symbol_cache.stats
    else:
        report['symbol_mapping_cache'] = {'enabled': False, 'oracle_use': False}
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
            alignment = align_methods(original[key], method)
            mapped_offset = alignment['rebuilt_offset']
            tail_mapping=None
            if is_smcs:
                from round7_infinite_tail_guard_hints import infinite_tail_guard_hints
                from round7_tail_source_mapping import tail_source_mapping_offset,tail_source_mapping_record
                source_tail_hints=infinite_tail_guard_hints(original[key],method,
                    alignment['original_offset'],alignment['rebuilt_offset'])
                if source_tail_hints.get('original_infinite_tail_guard'):
                    tail_mapping=tail_source_mapping_offset(mapped_offset,source_tail_hints,compiler='smcs')
                    mapped_offset=tail_mapping['new_offset']
            if mapped_offset >= method['header']['code_size']:
                mapped_offset = max(0, method['header']['code_size'] - 1)
            # Optional cache holds an immutable dump of the accepted generation;
            # failed trial binaries can overwrite disk without changing it.
            symbol_method=None
            if symbol_cache:
                symbol_method = symbol_cache.get(key, current_method=method,
                                                 require_equal_body=is_smcs)
                debug_method = symbol_method if is_smcs else None
                point = select_point(symbol_method, mapped_offset) if symbol_method else None
                mapping_kind = ('MDB-full-body-equal-debug-plus-vs-minus' if symbol_method
                    else 'MDB-debug-body-differed-UNKNOWN') if is_smcs else 'native-PDB-sequence-point'
                row.setdefault('symbol_mapping_cache_generations', []).append(symbol_cache.generation)
            # Default path preserves the Part A build/mapping policy.
            elif args.assembly == 'KairoLibrary':
                now_binary = c.build()
                if now_binary is None:
                    raise RuntimeError('Accepted source stopped compiling')
                symbol_method = next(iter(inspect_methods(now_binary, [key], symbols=True)['methods']), None)
                point = select_point(symbol_method, mapped_offset) if symbol_method else None
                mapping_kind = 'native-PDB-sequence-point'
            else:
                debug_binary = c.build(debug=True)
                debug_method = next(iter(inspect_methods(debug_binary, [key], symbols=True)['methods']), None) if debug_binary else None
                if debug_method and not differences(method, debug_method)[0]:
                    symbol_method=debug_method
                    point = select_point(debug_method, mapped_offset)
                    mapping_kind = 'MDB-full-body-equal-debug-plus-vs-minus'
                else:
                    point = None
                    mapping_kind = 'MDB-debug-body-differed-UNKNOWN'
            row['mapping'] = mapping_kind
            # Implicit static constructors need not have an MDB sequence point.
            # Match the first stsfld after the aligned hunk on BOTH sides to a
            # semantic AST field. This maps source only; whole-body and metadata
            # verification remain mandatory and no constructor is synthesized.
            if not point or not point.get('document'):
                from round10_initializer_mapping import bind_initializer
                initializer_binding = bind_initializer(syntax, c.paths, original[key], method, alignment)
                if initializer_binding:
                    point = initializer_binding['point']
                    mapping_kind = initializer_binding['mapping_kind']
                    row['mapping'] = mapping_kind
                    row.setdefault('static_initializer_mapping', []).append(initializer_binding)
                    report['static_initializer_mapping_module_sha256'] = sha(ROOT/'tools/round10_initializer_mapping.py')
            if tail_mapping is not None:
                row.setdefault('tail_source_mapping',[]).append(tail_source_mapping_record(tail_mapping,point))
                report['tail_source_mapping_module_sha256']=sha(ROOT/'tools/round7_tail_source_mapping.py')
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
            scoped_exit_evidence={}
            if is_smcs:
                from round9_scoped_exit_hints import scoped_exit_hints
                source_bound=False
                if symbol_cache and symbol_method and symbol_cache.stats['generation_proofs']:
                    recorded=symbol_cache.stats['generation_proofs'][-1]
                    live=mapping_source_proof()
                    source_bound=(recorded.get('generation')==symbol_cache.generation
                        and recorded.get('accepted_source_set_sha256')==live['accepted_source_set_sha256'])
                scoped_exit_evidence=scoped_exit_hints(original[key],method,alignment,
                    symbol_method,text,source_identity_confirmed=source_bound)
                if scoped_exit_evidence.get('round9_scoped_exit_valid'):
                    selected=select_point(symbol_method,scoped_exit_evidence['round9_scoped_exit_mapping_rebuilt_offset'])
                    if selected and Path(selected['document']).resolve()==c.paths[logical].resolve():
                        point=selected
                    else:
                        scoped_exit_evidence['round9_scoped_exit_valid']=False
                        scoped_exit_evidence['round9_scoped_exit_evidence']['reason']='fresh_anchor_source_document_not_exact'
                row.setdefault('scoped_exit_evidence',[]).append(scoped_exit_evidence)
            new_syntax_hash = syntax_identity()
            if new_syntax_hash != syntax_hash:
                syntax.close()
                syntax = TransformClient(syntax_path)
                syntax_hash = new_syntax_hash
            hints = alias_hints(original[key], method, alignment['original_offset'], plain_local_types.get(key, []), type_lookup)
            hints.update(scoped_exit_evidence)
            if is_smcs:
                from round9_array_population_hints import array_population_hints
                array_evidence=array_population_hints(original[key],method,alignment['original_offset'],alignment['rebuilt_offset'])
                hints.update(array_evidence)
                if array_evidence:row.setdefault('round9_array_population_evidence',[]).append(array_evidence)
                from round9_field_division_hints import field_division_hints
                division_evidence=field_division_hints(original[key],method,alignment['original_offset'],alignment['rebuilt_offset'])
                hints.update(division_evidence)
                if division_evidence:row.setdefault('round9_field_division_evidence',[]).append(division_evidence)
                from round9_protected_exit_hints import protected_exit_hints
                protected_evidence=protected_exit_hints(original[key],method,alignment['original_offset'],alignment['rebuilt_offset'])
                hints.update(protected_evidence)
                if protected_evidence:row.setdefault('round9_protected_exit_evidence',[]).append(protected_evidence)
                from round7_smcs_hints import rule_hints
                current_prediction=slot_model.predict(path=str(c.paths[logical]),source=text,method_id=key)
                current_types=[type_lookup.get(t,'UNKNOWN') for t in method.get('locals',[])]
                hints.update(rule_hints(original[key],method,alignment['original_offset'],
                    plain_local_types.get(key,[]),current_types,model=current_prediction))
                from round7_smcs_branch_exit_diagnose import branch_exit_hints
                hints.update(branch_exit_hints(original[key],alignment['original_offset']))
                from round7_concat_hints import original_concat_hints
                hints.update(original_concat_hints(original[key],alignment['original_offset']))
                from round7_switch_hints import original_empty_zero_arm_hint
                hints.update(original_empty_zero_arm_hint(original[key],alignment['original_offset']))
                from round7_balanced_branch_hints import balanced_branch_hints
                hints.update(balanced_branch_hints(original[key],method,
                    alignment['original_offset'],alignment['rebuilt_offset']))
                from round7_infinite_tail_guard_hints import infinite_tail_guard_hints
                hints.update(infinite_tail_guard_hints(original[key],method,
                    alignment['original_offset'],alignment['rebuilt_offset']))
            if args.calculated_parameter_aliases:
                from round6_alias_calculus import calculated_alias_hints
                calculated = calculated_alias_hints(original[key], method, debug_method)
                hints.update(calculated)
                row.setdefault('calculated_alias_evidence', []).append(calculated)
            if not is_smcs:
                from round9_boolean_context_hints import boolean_context_hints
                context_evidence=boolean_context_hints(original[key],method,alignment['original_offset'],alignment['rebuilt_offset'])
                hints.update(context_evidence)
                if context_evidence:row.setdefault('round9_boolean_context_evidence',[]).append(context_evidence)
                from round8_null_polarity_hints import null_polarity_hints
                nullable_hints=null_polarity_hints(original[key],method,alignment['original_offset'],alignment['rebuilt_offset'])
                hints.update(nullable_hints)
                row.setdefault('null_polarity_evidence',[]).append(nullable_hints)
                from round9_float_identity_hints import float_identity_conversion_hints
                from round9_per_arm_store_hints import per_arm_local_store_hints
                calculated_float=float_identity_conversion_hints(original[key],method,alignment['original_offset'],alignment['rebuilt_offset'])
                calculated_stores=per_arm_local_store_hints(original[key],method,alignment['original_offset'],alignment['rebuilt_offset'])
                hints.update(calculated_float)
                hints.update(calculated_stores)
                row.setdefault('round9_calculated_emission_evidence',[]).append({'float_identity':calculated_float,'per_arm_stores':calculated_stores})
            response = syntax.request(path=str(c.paths[logical]), source=text, line=point['start_line'], column=max(1, point['start_column']), method_id=key, max_candidates=512, compiler='smcs' if is_smcs else 'roslyn', round7_cycle_rule=args.cycle_rule or '', round7_cycle_rules=learned_rules, round8_expand_compound_fields=is_smcs, **hints)
            row['transform_helper_sha256'] = syntax_hash
            row['stop_point'] = {'offset': old_prefix, 'source': logical, 'point': point,
                                 'alignment': alignment, 'statement': response.get('mapped')}
            if response.get('error') or not response.get('mapped'):
                row['stop_reason'] = 'syntax_mapping_UNKNOWN'
                row['mapping_detail'] = response
                break
            patches = response.get('patches', [])
            if external:
                from round7_coalesce_calculate import rebase_unique_patch
                for proposal in external:
                    if proposal['method_id']!=key or proposal['logical_path'].replace('\\','/')!=logical.replace('\\','/'):
                        continue
                    if proposal.get('model_source_sha256',model_identity['source_sha256'])!=model_identity['source_sha256'] or proposal.get('client_sha256',model_identity['client_sha256'])!=model_identity['client_sha256']:
                        raise ValueError('External proposal model changed')
                    try:patches.insert(0,rebase_unique_patch(text,proposal))
                    except ValueError as error:row.setdefault('external_rebase_rejections',[]).append(str(error))
            # Calculated declarations may affect a wider block than the aligned
            # command. The solver produces source patches, never IL or states.
            if slot_model and hasattr(slot_model, 'solve'):
                # Retained v7 client.solve discarded incomplete proposals.
                # Use its source-only calculator directly; every proposed edit
                # now reaches the shared round9 complete/incomplete policy gate.
                calculator=importlib.import_module(Path(model_config['solver']).stem)
                source_prediction=slot_model.predict(path=str(c.paths[logical]),source=text,method_id=key)
                solved=calculator.proposals(text,source_prediction,original[key],method,
                    plain_local_types.get(key,[]),source_prediction.get('predicted_types',[]))
                solved['shared_round9_model_policy']=True
                patches = solved.get('patches', []) + patches
                row.setdefault('solver', []).append({k: v for k, v in solved.items() if k != 'patches'})
            if slot_model and key in baseline_variants:
                from round6_solver_alternatives import rebase_variant
                if response.get('mapped', {}).get('method_id') != key:
                    raise ValueError('Calculated alternative declaration has the wrong identity')
                rebased = [rebase_variant(text, response['declaration_start'], response['declaration_end'],
                                          variant, method_id=key) for variant in baseline_variants[key]
                           if variant['logical_path'] == logical]
                patches = rebased + patches
            if not is_smcs:
                proposals = ROOT / 'reports/round11-roslyn-patches.json'
                if proposals.exists():
                    proposed = read(proposals)
                    proposed = proposed.get('patches', proposed) if isinstance(proposed, dict) else proposed
                    raw = text.encode('utf-16-le')
                    extras = []
                    for proposal in proposed:
                        if proposal.get('method_id') != key:
                            continue
                        p = dict(proposal)
                        if raw[p['start'] * 2:p['end'] * 2].decode('utf-16-le') != p['before']:
                            needle = p['before'].encode('utf-16-le')
                            positions = [pos for pos in range(0, len(raw) - len(needle) + 1, 2) if raw[pos:pos + len(needle)] == needle]
                            if len(positions) != 1:
                                continue
                            p['start'], p['end'] = positions[0] // 2, (positions[0] + len(needle)) // 2
                        extras.append(p)
                    patches = extras + patches
            if is_smcs and args.cycle_rule=='calculated-original-local-coalescence':
                proposal_path=ROOT/'reports/round11-coalesce-patches.json'
                proposal_doc=read(proposal_path)
                from round7_coalesce_calculate import rebase_unique_patch
                if (proposal_doc['calculator_sha256']!=sha(ROOT/'tools/round7_coalesce_calculate.py')
                        or proposal_doc['syntax_source_sha256']!=sha(ROOT/'tools/round7_coalesce_syntax.cs')):
                    raise ValueError('Coalescence calculator proof changed')
                report['calculated_coalescence']={'proposals_sha256':sha(proposal_path),
                    'fresh_model_before_compilation':True,'exact_unique_declaration_rebase':True}
                for proposal in proposal_doc['patches']:
                    if proposal['method_id']!=key or proposal['logical_path'].replace('\\','/')!=logical.replace('\\','/'):
                        continue
                    if (proposal['model_source_sha256']!=report['slot_model_identity']['source_sha256']
                            or proposal['client_sha256']!=report['slot_model_identity']['client_sha256']):
                        raise ValueError('Coalescence proposal uses another model')
                    try:rebased=rebase_unique_patch(text,proposal)
                    except ValueError as error:
                        row.setdefault('coalescence_rebase_rejections',[]).append(str(error));continue
                    patches.insert(0,rebased)
            if is_smcs and args.cycle_rule=='calculated-store-anchored-declarations':
                proposal_path=ROOT/'reports/round11-store-anchor-patches.json'
                proposal_doc=read(proposal_path)
                from round7_store_anchor_calculate import rebase_unique_patch
                if proposal_doc['calculator_sha256']!=sha(ROOT/'tools/round7_store_anchor_calculate.py'):
                    raise ValueError('Store-anchor calculator proof changed')
                report['calculated_store_anchors']={'proposals_sha256':sha(proposal_path),
                    'fresh_model_before_compilation':True,'exact_unique_declaration_rebase':True}
                for proposal in proposal_doc['patches']:
                    if proposal['method_id']!=key or proposal['logical_path'].replace('\\','/')!=logical.replace('\\','/'):
                        continue
                    if (proposal['model_source_sha256']!=report['slot_model_identity']['source_sha256']
                            or proposal['client_sha256']!=report['slot_model_identity']['client_sha256']):
                        raise ValueError('Store-anchor proposal uses another model')
                    try:rebased=rebase_unique_patch(text,proposal)
                    except ValueError as error:
                        row.setdefault('store_anchor_rebase_rejections',[]).append(str(error));continue
                    patches.insert(0,rebased)
            if is_smcs and args.cycle_rule=='calculated-named-switch-selector-coalescence':
                proposal_path=ROOT/'reports/round11-draw6-calculated-patches.json'
                proposal_doc=read(proposal_path)
                from round7_draw6_slots_calculate import rebase_unique_patch
                if (proposal_doc['calculator_sha256']!=sha(ROOT/'tools/round7_draw6_slots_calculate.py')
                        or proposal_doc['syntax_source_sha256']!=sha(ROOT/'tools/round7_draw6_slot_syntax.cs.txt')):
                    raise ValueError('Named-selector calculator proof changed')
                for dependency,expected_hash in proposal_doc.get('dependency_sha256',{}).items():
                    dependency_path=(ROOT/dependency).resolve()
                    if not dependency_path.is_relative_to(ROOT/'tools') or sha(dependency_path)!=expected_hash:
                        raise ValueError('Named-selector dependency proof changed: '+dependency)
                report['calculated_named_switch_selector_coalescence']={
                    'proposals_sha256':sha(proposal_path),'fresh_model_before_compilation':True,
                    'exact_unique_declaration_rebase':True,'dependencies_checked':proposal_doc.get('dependency_sha256',{})}
                for proposal in proposal_doc['patches']:
                    if proposal['method_id']!=key or proposal['logical_path'].replace('\\','/')!=logical.replace('\\','/'):
                        continue
                    if (proposal['model_source_sha256']!=report['slot_model_identity']['source_sha256']
                            or proposal['client_sha256']!=report['slot_model_identity']['client_sha256']):
                        raise ValueError('Named-selector proposal uses another model')
                    try:rebased=rebase_unique_patch(text,proposal)
                    except ValueError as error:
                        row.setdefault('named_selector_rebase_rejections',[]).append(str(error));continue
                    patches.insert(0,rebased)
            # A new autonomous cycle cannot repeat exhausted legacy hypotheses
            # before the selected family has itself advanced this method.
            before_cycle_filter=len(patches)
            patches=cycle_candidates(patches,args.cycle_rule,0 if args.isolate_family else len(row['steps']))
            if args.cycle_rule and not row['steps']:
                removed=before_cycle_filter-len(patches)
                row.setdefault('cycle_repertoire_filters',[]).append({'iteration':iteration+1,
                    'candidate_count_before':before_cycle_filter,'kept_selected_candidates':len(patches),
                    'removed_legacy_candidates':removed,'selected_rule_prefix':args.cycle_rule})
                report['cycle_repertoire_counts']['filtered_steps']+=1
                report['cycle_repertoire_counts']['removed_legacy_candidates']+=removed
                report['cycle_repertoire_counts']['kept_selected_candidates']+=len(patches)
            # Budget across transformation families before source-only prediction.
            cycle_priority=(args.cycle_rule,) if args.cycle_rule else ()
            preferred_rules = cycle_priority + ROUND7_PRIORITY + ('calculated-parameter-alias', 'solver-', 'roslyn-slot-', 'null-', 'if-chain-switch',
                               'if-guard-switch', 'initialized-declaration-swap',
                               'restore-zero-chain-local', 'reserve-unused-slot-and-defer-local')
            patches.sort(key=lambda p: (next((i for i, r in enumerate(preferred_rules) if p['rule'].startswith(r)), len(preferred_rules)),
                                        p.get('distance', 0), p['end'] - p['start']))
            by_family = {}
            for patch in patches:
                by_family.setdefault(patch['rule'], patch)
            first_ids = {id(p) for p in by_family.values()}
            patches = list(by_family.values()) + [p for p in patches if id(p) not in first_ids]
            if slot_model:
                approved = []
                rejected_counts = Counter()
                model_seen = set()
                predictions = 0
                for patch in patches:
                    candidate = apply_patch(text, patch)
                    if candidate == text:
                        continue
                    candidate_hash = hashlib.sha256(candidate.encode('utf-8')).hexdigest()
                    if candidate_hash in model_seen:
                        continue
                    model_seen.add(candidate_hash)
                    if predictions >= args.max_model_candidates:
                        rejected_counts['prediction_budget_remaining'] += 1
                        continue
                    predictions += 1
                    prediction = slot_model.predict(path=str(c.paths[logical]), source=candidate, method_id=key)
                    report['slot_model_gate']['predicted'] += 1
                    if model_approval(prediction, plain_local_types.get(key, [])):
                        summary = {k: prediction[k] for k in ('complete', 'predicted_types', 'method_id', 'unsupported_reasons') if k in prediction}
                        patch = dict(patch, slot_model_prediction=summary)
                        approved.append(patch)
                        report['slot_model_gate']['approved'] += 1
                        if prediction.get('complete') is not True:
                            report['slot_model_gate']['incomplete_permitted']=report['slot_model_gate'].get('incomplete_permitted',0)+1
                    else:
                        reason = 'incomplete' if not prediction.get('complete') else 'wrong_signature'
                        report['slot_model_gate'][reason] += 1
                        rejected_counts[reason] += 1
                row.setdefault('model_rejections', []).append(dict(rejected_counts))
                patches = approved
            if not patches:
                row['stop_reason'] = ('model_prediction_limit' if rejected_counts.get('prediction_budget_remaining')
                                      else 'no_model_approved_candidate') if slot_model else 'no_candidate'
                row['candidate_count'] = 0
                break
            # Use the actual first pair to prioritize a finite repertoire. Do not
            # discard branch-operand differences from the acceptance criterion.
            oi, ri = instruction_at(original[key], alignment['original_offset']), instruction_at(method, mapped_offset)
            prefer = list(cycle_priority + ROUND7_PRIORITY) + ['calculated-parameter-alias', 'solver-', 'roslyn-slot-', 'null-', 'if-chain-switch', 'initialized-declaration-swap', 'restore-zero-chain-local', 'reserve-unused-slot-and-defer-local',
                      'foreach-array-indexed-for', 'alias-parameter-block-tail',
                      'restore-dead-zero-chain', 'alias-parameter-command', 'alias-field-command',
                      'reserve-unused-original-slot']
            if args.aligned_search_progress and oi and oi['opcode'].startswith(('beq', 'bne', 'bgt', 'bge', 'blt', 'ble')):
                prefer = ['ternary-polarity'] + prefer
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
                if declaration_start is not None and not response.get('disable_target_screening'):
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
            # Incomplete variants run sequentially, so only actually invoked
            # candidates consume their cumulative method budget. Complete-only
            # batches keep the existing parallel screening implementation.
            if screen_paths and all(p['slot_model_prediction'].get('complete') is True for p in patches):
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
                incomplete=slot_model and patch['slot_model_prediction'].get('complete') is not True
                if incomplete and not reserve_incomplete_variant(row,args.max_incomplete_candidates):
                    row.setdefault('incomplete_budget_rejections',[]).append({'rule':patch['rule'],'source_sha256':source_key[1]})
                    continue
                tested += 1
                from round9_candidate_sources import write_candidate_sources
                source_proof=write_candidate_sources(candidate,c.paths[logical],
                    screen_paths if pool is None else None,logical,source_filter)
                pool_result = pool.result(pool_indices[id(patch)]) if pool else None
                built = pool_result['binary'] if pool_result else c.build(screen_paths=screen_paths)
                trial = {'iteration': iteration + 1, 'rule': patch['rule'], 'start': patch['start'], 'end': patch['end'],
                         'before': patch['before'], 'after': patch['after'], 'compile_pass': built is not None,
                         'old_prefix': old_prefix, 'accepted': False}
                if slot_model:
                    trial['slot_model_prediction'] = patch['slot_model_prediction']
                trial['screen_used'] = screen_paths is not None
                trial['candidate_source_provenance']=source_proof
                if pool_result:
                    trial['screen_log_path'] = pool_result['log_path']
                    trial['screen_build_seconds'] = pool_result['seconds']
                counters[patch['rule'] + ':attempts'] += 1
                trial['incomplete_prediction']=bool(incomplete)
                if incomplete:
                    trial['candidate_compiler_invocations']=1
                    row['incomplete_compiler_invocations']=row.get('incomplete_compiler_invocations',0)+1
                if built:
                    newmethod = c.dump(built, [key]).get(key)
                    if slot_model and newmethod:
                        actual_types = [type_lookup.get(t, 'UNKNOWN') for t in newmethod.get('locals', [])]
                        trial['predicted_signature_confirmed'] = actual_types == patch['slot_model_prediction']['predicted_types']
                        trial['original_local_signature_equal'] = newmethod.get('locals') == original[key].get('locals')
                        allowed,why=compiled_signature_allows_search(patch['slot_model_prediction'],actual_types,
                            newmethod.get('locals'),original[key].get('locals'))
                        trial['compiled_signature_decision']=why
                        if not allowed:
                            if why=='complete_prediction_disagrees_with_emitter':
                                report['slot_model_gate']['post_compile_disagreements'] += 1
                            else:report['slot_model_gate']['compiled_signature_rejections']=report['slot_model_gate'].get('compiled_signature_rejections',0)+1
                            trial['model_disagreement_stage'] = 'screen' if screen_paths else 'full'
                            newmethod = None
                    if screen_paths:
                        report['screening']['candidates'] += 1
                        screen_prefix = prefix(original[key], newmethod) if newmethod else -1
                        screen_exact = newmethod is not None and not differences(original[key], newmethod)[0]
                        trial.update(screen_prefix=screen_prefix, screen_body_equal=screen_exact)
                        if screen_exact or (newmethod and search_progress(original[key], method, newmethod, args.aligned_search_progress)):
                            report['screening']['full_positive_checks'] += 1
                            built = c.build()
                            if incomplete:
                                trial['candidate_compiler_invocations']+=1
                                row['incomplete_compiler_invocations']+=1
                            newmethod = c.dump(built, [key]).get(key) if built else None
                            trial['full_compile_pass'] = built is not None
                            if not built:
                                trial['full_compiler_error'] = c.last_log[-3000:]
                            if newmethod and not search_progress(original[key], method, newmethod, args.aligned_search_progress):
                                report['screening']['false_positive_prefixes'] += 1
                        else:
                            report['screening']['rejected'] += 1
                            trial['screen_rejected'] = True
                            newmethod = None
                    if newmethod:
                        if slot_model:
                            actual_types = [type_lookup.get(t, 'UNKNOWN') for t in newmethod.get('locals', [])]
                            trial['predicted_signature_confirmed'] = actual_types == patch['slot_model_prediction']['predicted_types']
                            allowed,why=compiled_signature_allows_search(patch['slot_model_prediction'],actual_types,
                                newmethod.get('locals'),original[key].get('locals'))
                            trial['compiled_signature_decision']=why
                            if not allowed:
                                if why=='complete_prediction_disagrees_with_emitter':
                                    report['slot_model_gate']['post_compile_disagreements'] += 1
                                else:report['slot_model_gate']['compiled_signature_rejections']=report['slot_model_gate'].get('compiled_signature_rejections',0)+1
                                trial['model_disagreement_stage'] = 'full-after-screen'
                                row['trials'].append(trial)
                                c.paths[logical].write_text(text, encoding='utf-8', newline='')
                                continue
                        new_prefix = prefix(original[key], newmethod)
                        exact = not differences(original[key], newmethod)[0]
                        trial.update(new_prefix=new_prefix, body_equal=exact)
                        if search_progress(original[key], method, newmethod, args.aligned_search_progress):
                            trial['progress_kind'] = ('complete-body' if exact else 'raw-prefix'
                                if new_prefix > old_prefix else 'labelled-instruction-prefix-with-raw-prefix-preserved')
                            if trial['progress_kind'].startswith('labelled'):
                                trial['old_aligned_original_index'] = alignment['original_index']
                                trial['new_aligned_original_index'] = align_methods(original[key], newmethod)['original_index']
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
                                if symbol_cache:
                                    symbol_cache.accepted(key, patch['rule'])
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
                if row.get('incomplete_budget_skipped'):row['stop_reason']='incomplete_candidate_budget'
                if slot_model and rejected_counts.get('prediction_budget_remaining') and row['stop_reason'] == 'no_transformation_advances':
                    row['stop_reason'] = 'model_prediction_limit'
                row['candidate_count'] = len(patches)
                break
        if row['stop_reason'] is None:
            row['stop_reason'] = 'body_equal' if not differences(original[key], method)[0] else 'iteration_limit'
            if row['stop_reason'] == 'iteration_limit':
                row['last_attempt_point'] = row.pop('stop_point', None)
                row['stop_point'] = {'offset': prefix(original[key], method), 'statement': 'UNKNOWN: iteration limit before remapping'}
        row['final_offset'] = prefix(original[key], method)
        row['body_equal'] = not differences(original[key], method)[0]
        row['incomplete_budget_limit_reached']=row.get('incomplete_variants_compiled',0)>=args.max_incomplete_candidates and args.max_incomplete_candidates>0
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
    if slot_model:
        slot_model.close()
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
    report['incomplete_budget_summary']={'variants_compiled':sum(m.get('incomplete_variants_compiled',0) for m in report['methods']),
        'candidate_compiler_invocations':sum(m.get('incomplete_compiler_invocations',0) for m in report['methods']),
        'methods_with_variants':sum(m.get('incomplete_variants_compiled',0)>0 for m in report['methods']),
        'methods_at_limit':sum(m.get('incomplete_budget_limit_reached',False) for m in report['methods'])}
    report['adoption_eligible'] = not report.get('body_losses') and metadata['whole_structural_metadata_equal']
    report['baseline_manifest_sha256'] = sha(c.base / 'build-manifest.json')
    report['source_hashes'] = {p: sha(path) for p, path in c.paths.items()}
    report_path.write_text(json.dumps(report, indent=2) + '\n', encoding='utf-8')


if __name__ == '__main__':
    main()
