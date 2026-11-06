"""Prove smcs debug-body stability and measure native PDB/MDB source coverage.

This is a read-only probe of isolated builds. It does not change a ledger or
adopt a compiler flag for the reconstruction. Run after the frozen round-4
baseline measurement exists.
"""
from collections import Counter
import hashlib
import json
from pathlib import Path
import shutil
import subprocess
import sys

from compare_kairo import differences
from round4_symbols import inspect_methods, select_point, align_source_point

ROOT = Path(__file__).resolve().parents[1]


def read(path):
    return json.loads(path.read_text(encoding='utf-8-sig'))


def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def main():
    measured = ROOT / 'reports/round4-baseline-measurement.json'
    if not measured.is_file():
        raise RuntimeError('Complete the initial measurement before this compiler probe')
    baseline = ROOT / 'build/round4/baseline'
    folder = ROOT / 'build/Assembly-CSharp-round4-symbols-debug-plus'
    subprocess.run([sys.executable, str(ROOT / 'tools/build_csharp.py'), '--debug', '+', '--source-map',
                    str(baseline / 'Assembly-CSharp/source-map.json'), '--label', 'round4-symbols-debug-plus'],
                   cwd=ROOT, check=True)
    plain_doc = read(baseline / 'Assembly-CSharp/rebuilt-methods.json')
    plain = {m['stable_id']: m for m in plain_doc['selected_methods']}
    debug_doc = inspect_methods(folder / 'Assembly-CSharp.dll')
    debug = {m['stable_id']: m for m in debug_doc['methods']}
    (ROOT / 'build/round4/smcs-debug-plus-methods.json').write_text(json.dumps(debug_doc), encoding='utf-8')
    changed = {}
    facets = Counter()
    raw_changed = []
    for key, method in plain.items():
        diff, _ = differences(method, debug.get(key))
        if diff:
            changed[key] = diff
            facets.update(diff)
        if method.get('raw_body_hex') != debug[key].get('raw_body_hex'):
            raw_changed.append(key)
    kairo_binary = baseline / 'KairoLibrary/KairoLibrary.dll'
    if not kairo_binary.is_file():
        current_kairo = ROOT / 'build/KairoLibrary-roslyn-1.1.0/KairoLibrary.dll'
        frozen_receipt = read(baseline / 'KairoLibrary/verification.json')
        if sha(current_kairo) != frozen_receipt['build_sha256']:
            raise RuntimeError('Current Kairo binary no longer matches the measured frozen baseline')
        shutil.copy2(current_kairo, kairo_binary)
        shutil.copy2(current_kairo.with_suffix('.pdb'), kairo_binary.with_suffix('.pdb'))
    kairo_doc = inspect_methods(kairo_binary, points_only=True)
    kairo = {m['stable_id']: m for m in kairo_doc['methods']}
    measurement = read(measured)
    maps = {}
    coverage = {}
    for assembly in ('KairoLibrary', 'Assembly-CSharp'):
        values = []
        for method in measurement['assemblies'][assembly]['methods']:
            key = method['stable_id']
            offset = method['first_il_difference_offset']
            if offset is None:
                # Equal IL but a local/header/EH mismatch has no first command.
                point = None
            elif assembly == 'KairoLibrary':
                point = select_point(kairo[key], offset)
            else:
                point = align_source_point(plain[key], debug[key], offset)
            values.append({'id': method['id'], 'stable_id': key, 'first_il_difference_offset': offset,
                           'source_point': point})
        maps[assembly] = values
        coverage[assembly] = {'divergent_methods': len(values),
                              'with_first_il_difference': sum(v['first_il_difference_offset'] is not None for v in values),
                              'mapped_first_difference': sum(v['source_point'] is not None for v in values),
                              'unmapped': [v['id'] for v in values if v['source_point'] is None]}
    inputs = [measured, baseline / 'Assembly-CSharp/build-manifest.json', folder / 'build-manifest.json',
              folder / 'Assembly-CSharp.dll', folder / 'Assembly-CSharp.dll.mdb', kairo_binary,
              kairo_binary.with_suffix('.pdb'), ROOT / 'tools/Round4SymbolInspector.cs', ROOT / 'tools/ProbeInspector.cs']
    result = {'schema_version': 1, 'operation': 'isolated-compiler-source-mapping-probe',
              'provenance': {str(path.relative_to(ROOT)): sha(path) for path in inputs},
              'smcs': {'plain_flag': '-debug-', 'probe_flag': '-debug+', 'optimize': '+',
                       'methods_compared': len(plain), 'canonical_whole_bodies_changed': len(changed),
                       'raw_method_bodies_changed': len(raw_changed), 'changed_methods': changed,
                       'raw_changed_methods': raw_changed, 'changed_facets': dict(facets),
                       'plain_debuggable_blobs': plain_doc.get('debuggable_blobs'),
                       'symbol_kind': debug_doc['symbol_kind'],
                       'methods_with_visible_sequence_points': sum(any(not p['hidden'] for p in m['sequence_points']) for m in debug.values()),
                       'mapping_method': 'Same IL offsets in debug+ MDB; complete bodies proved equal to debug-. No instruction alignment needed for this frozen baseline.' if not changed else 'Diagnostic no-debug-to-MDB instruction alignment; exact mapping may be UNKNOWN.'},
              'roslyn': {'symbol_kind': kairo_doc['symbol_kind'], 'methods': len(kairo),
                         'methods_with_visible_sequence_points': sum(any(not p['hidden'] for p in m['sequence_points']) for m in kairo.values()),
                         'mapping_method': 'Native PDB visible sequence point preceding the first divergent IL offset.'},
              'coverage': coverage, 'method_mappings': maps,
              'limits': ['Sequence points identify a compiler source span; the syntax engine must select the containing command.',
                         'The debug- vs debug+ body identity is measured for the frozen baseline, not presumed for every future source.',
                         'Symbols are diagnostic only; headers, locals, EH and all body bytes still require the full verifier.']}
    output = ROOT / 'reports/round4-symbol-mapping.json'
    output.write_text(json.dumps(result, indent=2, ensure_ascii=True) + '\n', encoding='utf-8')
    print(json.dumps({'report': str(output.relative_to(ROOT)), 'smcs_changed': len(changed),
                      'raw_changed': len(raw_changed), 'coverage': coverage}))


if __name__ == '__main__':
    main()
