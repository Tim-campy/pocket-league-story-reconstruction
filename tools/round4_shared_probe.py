"""Measure Roslyn 1.1 /shared as an isolated engine-trial optimization."""
import json
from pathlib import Path
import subprocess
from time import perf_counter

from compare_kairo import differences, sha
from metadata_compare import compare_paths
from round4_reference_probe import ROOT, SNAP, read, dump


def main():
    manifest = read(SNAP / 'build-manifest.json')
    sources = read(SNAP / 'source-map.json')
    baseline = {m['stable_id']: m for m in read(SNAP / 'rebuilt-methods.json')['selected_methods']}
    for source in manifest['sources']:
        if sha(Path(sources[source['path']])) != source['sha256']:
            raise ValueError('Frozen source mismatch')
    folder = ROOT / 'build/round4/shared-probe'
    folder.mkdir(parents=True, exist_ok=True)
    result = {'schema_version': 1, 'scope': 'isolated frozen-source compiler client-server transport probe',
              'compiler': manifest['compiler'], 'compiler_sha256': manifest['compiler_sha256'],
              'server': str(Path(manifest['compiler']).with_name('VBCSCompiler.exe')),
              'server_sha256': sha(Path(manifest['compiler']).with_name('VBCSCompiler.exe')),
              'production_options_changed': False, 'iterations': [],
              'timing_limitation': 'Wall time on this shared Windows host while independent analysis may run; not a controlled performance claim.'}
    report_path = ROOT / 'reports/round4-shared-compiler-probe.json'
    if report_path.exists():
        previous = read(report_path)
        if previous['compiler_sha256'] != manifest['compiler_sha256']:
            raise ValueError('Previous compiler probe used another compiler')
        result['iterations'] = previous['iterations']
        for item in result['iterations']:
            item.setdefault('shared_option_location', 'response-file' if item['shared'] else 'absent')
    plan = [(False, 'absent'), (False, 'absent'), (True, 'response-file'), (True, 'response-file'),
            (True, 'command-line'), (True, 'command-line')]
    for shared, placement in plan[len(result['iterations']):]:
        number = len(result['iterations']) + 1
        trial = folder / str(number)
        trial.mkdir(exist_ok=True)
        output = trial / 'KairoLibrary.dll'
        flags = [f if not f.startswith('/out:') else '/out:' + str(output) for f in manifest['flags']]
        if shared and placement == 'response-file':
            flags.append('/shared')
        args = [f for f in flags if f != '/noconfig'] + ['/reference:' + r['path'] for r in manifest['references']] + [sources[s['path']] for s in manifest['sources']]
        response = trial / 'compile.rsp'
        response.write_text('\n'.join('"' + a + '"' for a in args) + '\n', encoding='utf-8')
        start = perf_counter()
        command = [manifest['compiler']] + (['/shared'] if shared and placement == 'command-line' else []) + ['/noconfig', '@' + str(response)]
        completed = subprocess.run(command, capture_output=True)
        elapsed = perf_counter() - start
        (trial / 'compiler.log').write_bytes(completed.stdout + completed.stderr)
        item = {'shared': shared, 'shared_option_location': placement, 'command': command,
                'elapsed_seconds': elapsed, 'exit_code': completed.returncode, 'flags': flags}
        if completed.returncode == 0:
            methods = dump(output, trial / 'methods.json')
            changed = [key for key, method in baseline.items() if differences(method, methods.get(key))[0]]
            metadata = compare_paths(ROOT / 'build/round4/reference-probes/control/KairoLibrary.dll', output, trial / 'metadata.json')
            item.update({'changed_canonical_method_ids': changed, 'extra_ids': sorted(set(methods) - set(baseline)),
                         'types_equal': metadata['types_equal'], 'types_total': metadata['types_total'],
                         'global_differences': metadata['global_differences'],
                         'whole_structural_metadata_equal': metadata['whole_structural_metadata_equal'], 'output_sha256': sha(output)})
        result['iterations'].append(item)
        report_path.write_text(json.dumps(result, indent=2) + '\n', encoding='utf-8')
        print(json.dumps({k: item[k] for k in ('shared', 'elapsed_seconds', 'exit_code')} | {'canonical_changes': len(item.get('changed_canonical_method_ids', [])), 'metadata_equal': item.get('whole_structural_metadata_equal')}), flush=True)


if __name__ == '__main__':
    main()
