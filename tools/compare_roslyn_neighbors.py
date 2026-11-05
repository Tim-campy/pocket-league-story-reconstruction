"""Freeze inputs and compare complete Kairo builds across Roslyn 1.0--1.3.

Run only after the current main build has passed its complete verification.
No target source, method ledger or generated verification state is modified.
The one option exception is omission of unsupported /deterministic- on 1.0.0.
"""
import argparse
import hashlib
import json
from pathlib import Path
import re
import shutil
import subprocess
import sys
from compare_kairo import body_cells, differences, sha
from fetch_roslyn_neighbors import PINNED

ROOT = Path(__file__).resolve().parents[1]
VERSIONS = ('1.0.0', '1.1.0', '1.2.0', '1.3.2')
BASELINE = '1.3.2'
CSC_1_3_2 = 'd14ba119edd71bdcb82b12659647427aa46d819caf6e72ac6d4ae86c81eb36c9'
EXPECTED_FLAGS = ['/nologo', '/noconfig', '/nostdlib+', '/target:library', '/platform:anycpu',
                  '/optimize+', '/debug:pdbonly', '/deterministic-', '/langversion:6',
                  '/unsafe-', '/checked-', '/codepage:65001', '/utf8output']


def write_json(path, data):
    path.write_text(json.dumps(data, indent=2, ensure_ascii=True) + '\n', encoding='utf-8')


def index_methods(document):
    methods = document['selected_methods']
    indexed = {method['stable_id']: method for method in methods}
    if len(indexed) != len(methods):
        raise ValueError('Ambiguous method stable IDs')
    return indexed


def match_delta(baseline, candidate):
    return {'gained_vs_1_3_2': sorted(candidate - baseline),
            'lost_vs_1_3_2': sorted(baseline - candidate),
            'same_match_count': len(candidate) == len(baseline),
            'same_match_set': candidate == baseline}


def self_checks():
    swapped = match_delta({'a', 'b'}, {'a', 'c'})
    if not swapped['same_match_count'] or swapped['same_match_set']:
        raise ValueError('Equal-count swapped-method negative control failed')
    if swapped['gained_vs_1_3_2'] != ['c'] or swapped['lost_vs_1_3_2'] != ['b']:
        raise ValueError('Gain/loss identity negative control failed')
    if not match_delta({'a'}, {'a'})['same_match_set']:
        raise ValueError('Identical-set positive control failed')
    return {'equal_count_different_ids_rejected': True,
            'gain_loss_ids_preserved': True, 'identical_sets_recognized': True}


def checked_copy(source, target, expected):
    data = source.read_bytes()
    if hashlib.sha256(data).hexdigest() != expected:
        raise ValueError('Input changed before freezing: ' + str(source))
    target.parent.mkdir(parents=True, exist_ok=True)
    target.write_bytes(data)
    if sha(target) != expected:
        raise ValueError('Frozen copy hash mismatch')


def freeze_inputs(snapshot):
    main = ROOT / 'build/KairoLibrary-roslyn-1.3.2'
    manifest_path = main / 'build-manifest.json'
    manifest = json.loads(manifest_path.read_text(encoding='utf-8'))
    if manifest['exit_code'] != 0 or sha(main / 'KairoLibrary.dll') != manifest['output_sha256']:
        raise ValueError('Main baseline build is not valid')
    if manifest['compiler_sha256'] != CSC_1_3_2:
        raise ValueError('Main compiler is not the pinned baseline')
    flags = [flag for flag in manifest['flags'] if not flag.startswith('/out:')]
    if flags != EXPECTED_FLAGS:
        raise ValueError('Main flags changed; review intended options before matrix')
    source_order = [line for line in (ROOT / 'config/compile-order/KairoLibrary.txt').read_text(encoding='utf-8').splitlines()
                    if line and not line.startswith('#')]
    if 'src/KairoLibrary/AssemblyInfo.cs' not in source_order:
        source_order.append('src/KairoLibrary/AssemblyInfo.cs')
    if [item['path'] for item in manifest['sources']] != source_order:
        raise ValueError('Compile order changed since main verification')
    sources, references = [], []
    for item in manifest['sources']:
        source = (ROOT / item['path']).resolve()
        if not source.is_relative_to((ROOT / 'src/KairoLibrary').resolve()):
            raise ValueError('Unexpected target source input')
        target = snapshot / 'sources' / item['path']
        checked_copy(source, target, item['sha256'])
        sources.append({'logical_path': item['path'], 'original_path': str(source),
                        'frozen_path': str(target), 'sha256': item['sha256']})
    for index, item in enumerate(manifest['references']):
        source = Path(item['path'])
        # Separate directories preserve duplicate basenames without changing order.
        target = snapshot / 'references' / str(index) / source.name
        checked_copy(source, target, item['sha256'])
        references.append({'ordinal': index, 'original_path': str(source),
                           'frozen_path': str(target), 'sha256': item['sha256']})
    main_dump = main / 'rebuilt-methods.json'
    if not main_dump.is_file():
        raise ValueError('Main verifier method dump missing')
    checked_copy(main_dump, snapshot / 'baseline-main-methods.json', sha(main_dump))
    write_json(snapshot / 'input-receipt.json', {'main_manifest_sha256': sha(manifest_path),
               'main_output_sha256': manifest['output_sha256'], 'flags': flags,
               'sources': sources, 'references': references})
    return sources, references


def require_frozen_inputs(sources, references):
    for item in sources + references:
        if sha(Path(item['frozen_path'])) != item['sha256']:
            raise ValueError('Frozen input changed during compiler matrix')


def dump_methods(inspector, assembly, output):
    with output.open('wb') as stream:
        result = subprocess.run([str(inspector), str(assembly), '--structure'], stdout=stream, stderr=subprocess.PIPE)
    if result.returncode:
        raise RuntimeError(result.stderr.decode('utf-8', errors='replace'))
    return index_methods(json.loads(output.read_text(encoding='utf-8-sig')))


def compile_version(version, folder, sources, references):
    compiler = ROOT / ('analysis/tools/roslyn-' + version + '/tools/csc.exe')
    expected = CSC_1_3_2 if version == BASELINE else PINNED[version][1]
    if not compiler.is_file() or sha(compiler) != expected:
        raise ValueError('Missing or changed pinned compiler: ' + version)
    flags = [flag for flag in EXPECTED_FLAGS if version != '1.0.0' or flag != '/deterministic-']
    output = folder / 'KairoLibrary.dll'
    rsp = folder / 'compile.rsp'
    parameters = [flag for flag in flags if flag != '/noconfig'] + ['/out:' + str(output)]
    parameters += ['/reference:' + item['frozen_path'] for item in references]
    parameters += [item['frozen_path'] for item in sources]
    rsp.write_text('\n'.join('"' + value + '"' for value in parameters) + '\n', encoding='utf-8')
    result = subprocess.run([str(compiler), '/noconfig', '@' + str(rsp)], capture_output=True,
                            text=True, encoding='utf-8', errors='replace')
    log = result.stdout + result.stderr
    (folder / 'compiler.log').write_text(log, encoding='utf-8')
    report = {'package_version': version, 'compiler': str(compiler.relative_to(ROOT)),
              'compiler_sha256': expected, 'exit_code': result.returncode,
              'flags': flags, 'sources': sources, 'references': references,
              'option_exceptions': ['/deterministic- omitted: unsupported by 1.0.0'] if version == '1.0.0' else [],
              'source_files': len(sources), 'reference_files': len(references),
              'errors': [line for line in log.splitlines() if re.search(r'\berror\s+CS\d+', line)],
              'output_sha256': sha(output) if result.returncode == 0 else None}
    write_json(folder / 'build-manifest.json', report)
    return report


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--label', default='round2')
    args = parser.parse_args()
    if not re.fullmatch('[A-Za-z0-9_-]+', args.label):
        raise ValueError('Label must be a simple filename component')
    tests = self_checks()
    snapshot = ROOT / ('analysis/variants/roslyn-neighbors-' + args.label)
    folder = ROOT / ('build/roslyn-neighbors-' + args.label)
    report_path = ROOT / ('reports/KairoLibrary-roslyn-neighbors-' + args.label + '.json')
    if snapshot.exists() or folder.exists() or report_path.exists():
        raise ValueError('Run label already exists; choose a new label to preserve evidence')
    # Complete comparison requires all four tools to be present before snapshotting.
    for version in VERSIONS:
        compiler = ROOT / ('analysis/tools/roslyn-' + version + '/tools/csc.exe')
        expected = CSC_1_3_2 if version == BASELINE else PINNED[version][1]
        if not compiler.is_file() or sha(compiler) != expected:
            raise ValueError('Acquire pinned compiler before matrix: ' + version)
    snapshot.mkdir(parents=True)
    folder.mkdir(parents=True)
    sources, references = freeze_inputs(snapshot)
    subprocess.run([sys.executable, str(ROOT / 'tools/prepare_probe_inspector.py')], check=True, cwd=ROOT, capture_output=True)
    inspector_folder = snapshot / 'inspector'
    inspector_folder.mkdir()
    for name in ('ProbeInspector.exe', 'Mono.Cecil.dll'):
        shutil.copy2(ROOT / 'build/tools' / name, inspector_folder / name)
    inspector = inspector_folder / 'ProbeInspector.exe'
    original_path = ROOT / 'original/extracted/assets/bin/Data/Managed/KairoLibrary.dll'
    original_sha256 = sha(original_path)
    project = json.loads((ROOT / 'config/project.json').read_text(encoding='utf-8'))
    expected_original = next(item['sha256'] for item in project['managed_assemblies']
                             if item['path'].endswith('/KairoLibrary.dll'))
    if original_sha256 != expected_original:
        raise ValueError('Original target identity mismatch')
    oracle = dump_methods(inspector, original_path, folder / 'original-methods.json')
    original_bodies = {key for key, method in oracle.items() if body_cells(method) is not None}
    candidates, matched_sets, per_version = {}, {}, {}
    for version in VERSIONS:
        require_frozen_inputs(sources, references)
        build = folder / version
        build.mkdir()
        info = compile_version(version, build, sources, references)
        if info['exit_code'] != 0:
            per_version[version] = info
            continue
        methods = dump_methods(inspector, build / 'KairoLibrary.dll', build / 'rebuilt-methods.json')
        candidates[version] = methods
        matches = {key for key in original_bodies if not differences(oracle[key], methods.get(key))[0]}
        matched_sets[version] = matches
        info['body_equal'] = len(matches)
        info['missing_ids'] = sorted(set(oracle) - set(methods))
        info['extra_ids'] = sorted(set(methods) - set(oracle))
        info['matched_original_ids'] = sorted(matches)
        per_version[version] = info
        print(json.dumps({'version': version, 'body_equal': len(matches),
                          'missing': len(info['missing_ids']), 'extra': len(info['extra_ids'])}), flush=True)
    require_frozen_inputs(sources, references)
    if sha(original_path) != original_sha256:
        raise ValueError('Original changed during matrix')
    baseline = matched_sets.get(BASELINE)
    frozen_baseline_reproduces_main = False
    if baseline is not None:
        main_methods = index_methods(json.loads((snapshot / 'baseline-main-methods.json').read_text(encoding='utf-8-sig')))
        baseline_outputs = candidates[BASELINE]
        main_ids = set(main_methods) | set(baseline_outputs)
        main_differences = [key for key in sorted(main_ids)
                            if key not in main_methods or differences(main_methods[key], baseline_outputs.get(key))[0]]
        frozen_baseline_reproduces_main = not main_differences
        for version, matches in matched_sets.items():
            item = per_version[version]
            item.update(match_delta(baseline, matches))
            item['gain_count'] = len(item['gained_vs_1_3_2'])
            item['loss_count'] = len(item['lost_vs_1_3_2'])
            item['net_body_equal_change'] = len(matches) - len(baseline)
            item['exclusive_vs_all_tested'] = sorted(matches - set().union(
                *(values for other, values in matched_sets.items() if other != version)))
            outputs = candidates[version]
            union_ids = set(baseline_outputs) | set(outputs)
            output_diffs = []
            for key in sorted(union_ids):
                if key not in baseline_outputs:
                    output_diffs.append({'id': key, 'facets': ['only_candidate']})
                else:
                    facets, _ = differences(baseline_outputs[key], outputs.get(key))
                    if facets:
                        output_diffs.append({'id': key, 'facets': facets})
            item['outputs_different_vs_1_3_2'] = output_diffs
            item['output_difference_count'] = len(output_diffs)
    complete = len(matched_sets) == len(VERSIONS) and frozen_baseline_reproduces_main
    same_sets = complete and all(values == baseline for values in matched_sets.values())
    same_outputs = complete and all(not item['outputs_different_vs_1_3_2'] for item in per_version.values())
    if not complete:
        conclusion = 'INCONCLUSIVE: failed build or frozen baseline does not reproduce current main output'
    elif same_sets and same_outputs:
        conclusion = 'INDISTINGUISHABLE on all inspected method definitions and bodies for this frozen source/reference/option set'
    elif same_sets:
        conclusion = 'Match sets identical; no compiler selection advantage by exact-match set, but method outputs remain distinguishable'
    else:
        conclusion = 'DISTINGUISHABLE by exact-match method identities; equal counts, if any, do not imply equivalence'
    result = {'schema_version': 1, 'scope': 'full KairoLibrary, frozen identical sources/references/order',
              'original_sha256': original_sha256, 'original_method_declarations': len(oracle),
              'original_method_bodies': len(original_bodies), 'baseline_version': BASELINE,
              'snapshot_receipt': str((snapshot / 'input-receipt.json').relative_to(ROOT)),
              'snapshot_receipt_sha256': sha(snapshot / 'input-receipt.json'),
              'inspector_sha256': sha(inspector), 'inspector_source_sha256': sha(ROOT / 'tools/ProbeInspector.cs'),
              'cecil_sha256': sha(inspector_folder / 'Mono.Cecil.dll'),
              'baseline_body_equal': len(baseline) if baseline is not None else None,
              'frozen_baseline_reproduces_main': frozen_baseline_reproduces_main,
              'baseline_freeze_differences': main_differences if baseline is not None else None,
              'complete_four_compilers': complete, 'match_sets_identical': same_sets,
              'all_method_outputs_identical': same_outputs, 'conclusion': conclusion,
              'historical_compiler_identity': 'UNKNOWN', 'per_version': per_version,
              'negative_controls': tests, 'ledger_states_promoted': False, 'main_sources_modified': False,
              'normalized_assembly_fields': [],
              'metric': 'Full inspector definition facets plus resolved method-body byte cells, header, locals and exception sections',
              'limitations': ['Historical compiler cannot be identified uniquely by this source set alone.',
                              '1.0.0 omits unsupported /deterministic-; all other codegen options are identical.',
                              'No metadata normalization, IL copying, target execution or post-build assembly edits.']}
    write_json(report_path, result)
    print(json.dumps({'report': str(report_path.relative_to(ROOT)), 'conclusion': conclusion,
                      'baseline_body_equal': result['baseline_body_equal'],
                      'counts': {version: item.get('body_equal') for version, item in per_version.items()}}), flush=True)
    return 0 if complete else 1


if __name__ == '__main__':
    raise SystemExit(main())
