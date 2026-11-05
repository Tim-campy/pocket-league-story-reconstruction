"""Read-only smcs local census on commit 2ab24ee source and frozen round baseline.

Writes only isolated syntax tooling/copies and a diagnostic report. No source,
target build, ledger, or matched state is changed. Candidate pools are not gains.
"""
from collections import Counter
import hashlib
import json
from pathlib import Path
import subprocess
from compiler_probes import ROSLYN

ROOT = Path(__file__).resolve().parents[1]
COMMIT = '2ab24ee6f9b8d8f204c587b878d38ed120151467'


def read(path):
    return json.loads(path.read_text(encoding='utf-8-sig'))


def main():
    frozen = ROOT / 'build/round2'
    baseline = read(frozen / 'Assembly-CSharp-baseline-verification.json')
    original = {m['stable_id']: m for m in read(frozen / 'Assembly-CSharp-original-methods.json')['selected_methods']}
    rebuilt = {m['stable_id']: m for m in read(frozen / 'Assembly-CSharp-rebuilt-methods.json')['selected_methods']}
    dest = ROOT / 'build/smcs-local-census'
    dest.mkdir(exist_ok=True)
    inputs = []
    for source in baseline['sources']:
        path = dest / source['path']
        path.parent.mkdir(parents=True, exist_ok=True)
        contents = subprocess.run(['git', 'show', COMMIT + ':' + source['path']], cwd=ROOT,
                                  check=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE).stdout
        path.write_bytes(contents)
        inputs.append(str(path))
    manifest = read(ROOT / 'build/Assembly-CSharp-mono-smcs/build-manifest.json')
    syntax_input = dest / 'input.json'
    syntax_input.write_text(json.dumps({'sources': inputs, 'references': [r['path'] for r in manifest['references']]}), encoding='utf-8')
    vs = next(p for p in ROSLYN.parents if (p / 'dotnet').is_dir())
    runtime = vs / 'dotnet/net10.0/runtime'
    framework = runtime / 'shared/Microsoft.NETCore.App/10.0.10'
    references = [framework / (name + '.dll') for name in ('System.Private.CoreLib', 'System.Runtime',
        'System.Console', 'System.Linq', 'System.Collections', 'System.Text.Json', 'System.Reflection.Metadata',
        'System.Collections.Immutable', 'System.Memory', 'netstandard')]
    references += [ROSLYN.parent / (name + '.dll') for name in ('Microsoft.CodeAnalysis', 'Microsoft.CodeAnalysis.CSharp')]
    helper = dest / 'SmcsLocalSyntax.dll'
    rsp = dest / 'compile.rsp'
    args = ['/nologo', '/nostdlib+', '/target:exe', '/out:' + str(helper)] + ['/r:' + str(p) for p in references] + [str(ROOT / 'tools/SmcsLocalSyntax.cs')]
    rsp.write_text('\n'.join('"' + x + '"' for x in args), encoding='utf-8')
    subprocess.run([str(ROSLYN), '/noconfig', '@' + str(rsp)], check=True)
    for name in ('Microsoft.CodeAnalysis.dll', 'Microsoft.CodeAnalysis.CSharp.dll'):
        import shutil
        shutil.copy2(ROSLYN.parent / name, dest / name)
    (dest / 'SmcsLocalSyntax.runtimeconfig.json').write_text(json.dumps({'runtimeOptions': {'tfm': 'net10.0',
        'framework': {'name': 'Microsoft.NETCore.App', 'version': '10.0.10'}}}), encoding='utf-8')
    result = subprocess.run([str(runtime / 'dotnet.exe'), str(helper), str(syntax_input)],
                            check=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE)
    (dest / 'syntax.json').write_bytes(result.stdout)
    syntax = json.loads(result.stdout.decode('utf-8-sig'))
    members = {m['id']: m for m in syntax['methods']}
    matched_ids = {m['stable_id'] for m in baseline['methods'].values() if m['state'] == 'matched'}
    corpus, residuals = Counter(), []
    for key in sorted(matched_ids):
        if key not in members or members[key]['constructs']['lambda_count']:
            continue
        src, il = members[key], original[key]
        corpus['lambda_free_explicit_matched'] += 1
        declared = len(src['locals'])
        if declared == len(il['locals']):
            corpus['expanded_declared_count_equals_IL'] += 1
        else:
            corpus['expanded_declared_count_differs_from_IL'] += 1
            residuals.append({'id': key, 'source_local_count': declared, 'IL_local_count': len(il['locals']),
                              'locals': src['locals'], 'IL_locals': il['locals'], 'constructs': src['constructs']})
    targets = [m['stable_id'] for m in baseline['methods'].values()
               if set(m['differences']) - {'resolved_body_bytes'} == {'header', 'locals', 'instructions'}]
    candidate_ids, switches, delta = [], [], Counter()
    for key in targets:
        delta[str(len(original[key]['locals']) - len(rebuilt[key]['locals']))] += 1
        if key not in members:
            continue
        src = members[key]
        if src['constructs']['switch_count'] and len(rebuilt[key]['locals']) == len(original[key]['locals']) + 1:
            switches.append(key)
            safe = [s for s in src['switches'] if not s['has_goto'] and s['symbol_kind'] in ('Local', 'Parameter')
                    and all(not x.startswith('case "') and not x.startswith('case null') for x in s['labels'])]
            if safe:
                candidate_ids.append(key)
    report = {'assembly': 'Assembly-CSharp', 'compiler': 'smcs 2.6.5.0', 'baseline_commit': COMMIT,
        'scope': 'read-only census, no target compilation or method promotion', 'corpus': dict(corpus),
        'remaining_count_mismatches': residuals, 'HLI_total': len(targets),
        'HLI_original_minus_rebuilt_local_count': dict(delta),
        'HLI_switch_and_one_extra_rebuilt_local': switches,
        'switch_to_if_first_pass_candidates': candidate_ids,
        'candidate_rule': 'Restore ordered if/else comparisons where recovered small integral switch introduces a cache local absent from original; only stable local/parameter operands, no goto. Original IL must decide label order and empty branches.',
        'rule_verification_status': 'UNKNOWN: not compiled in this diagnostic',
        'hypothesis_status': 'INCONCLUSIVE: expanded syntax census is not a per-slot causal proof',
        'semantic_errors': syntax['semantic_errors'],
        'provenance': {'helper_sha256': hashlib.sha256((ROOT / 'tools/SmcsLocalSyntax.cs').read_bytes()).hexdigest(),
                       'baseline_build_sha256': baseline['build_sha256']}}
    output = ROOT / 'reports/Assembly-CSharp-local-census.json'
    output.write_text(json.dumps(report, indent=2, ensure_ascii=True) + '\n', encoding='utf-8')
    families = Counter()
    for residual in residuals:
        kind, key = residual['constructs'], residual['id']
        if kind['switch_count']:
            family = 'switch-cache'
        elif kind['return_in_try_count']:
            family = 'return-across-EH'
        elif kind['foreach_count']:
            family = 'foreach-enumerator-or-array-cache'
        elif key.startswith(('surface.GamePad::DrawWatch', 'surface.GameView::Draw(')):
            family = 'value-type-receiver-address'
        elif key.startswith(('surface.TouchEffect::Init', 'system.friend.MyFriend::.ctor')):
            family = 'chained-assignment-cache'
        elif key.startswith('game.Player::Act_freeRun'):
            family = 'postincrement-value-preservation'
        else:
            family = 'UNKNOWN'
        residual['candidate_lowering_family'] = family
        residual['causal_proof_status'] = 'HYPOTHESIS: matched corpus/source/IL correlation, no isolated construction probe'
        families[family] += 1
    report['candidate_lowering_family_counts'] = dict(families)
    candidates = []
    for key in switches:
        instructions = original[key]['instructions']
        comparators = []
        for n, instruction in enumerate(instructions):
            if instruction['opcode'].split('.')[0] in ('brtrue', 'brfalse', 'beq', 'bne', 'bge', 'bgt', 'ble', 'blt'):
                comparators.append(dict(instruction, preceding=instructions[max(0, n - 2):n]))
        candidates.append({'id': key, 'source_path': members[key]['path'],
            'original_local_count': len(original[key]['locals']), 'rebuilt_local_count': len(rebuilt[key]['locals']),
            'switch_count': members[key]['constructs']['switch_count'], 'original_comparators': comparators,
            'original_instruction_count': len(instructions), 'rebuilt_instruction_count': len(rebuilt[key]['instructions'])})
    origin_report = {'assembly': 'Assembly-CSharp', 'baseline_commit': COMMIT, 'HLI_total': len(targets),
        'candidate_count': len(switches), 'switch_to_if_candidate_ids': switches, 'candidates': candidates,
        'rule_verification_status': report['rule_verification_status'],
        'local_origin_hypothesis_status': 'INCONCLUSIVE globally; observed count residuals have candidate lowering families, not formal per-slot proofs.',
        'expanded_census': report}
    (ROOT / 'reports/Assembly-CSharp-local-origin-census.json').write_text(
        json.dumps(origin_report, indent=2, ensure_ascii=True) + '\n', encoding='utf-8')
    print(json.dumps({'corpus': dict(corpus), 'HLI_total': len(targets), 'switch_extra_local': len(switches),
                      'first_pass_candidate_count': len(candidate_ids)}))


if __name__ == '__main__':
    main()
