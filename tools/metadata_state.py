"""Bind structural type proofs to current build inputs; callable by verifiers only.

This module has no command-line mutation entry point. apply_type_metadata is the
only writer and must be called after a complete assembly build/comparison.
"""
from copy import deepcopy
import csv
import hashlib
import json
from pathlib import Path
from kairo_candidate import BUILD_FOLDER

ASSEMBLIES = {
    'KairoLibrary': (BUILD_FOLDER, 'tools/verify_kairo.py', 'tools/build_kairo.py'),
    'Assembly-CSharp': ('build/Assembly-CSharp-mono-smcs', 'tools/verify_csharp.py', 'tools/build_csharp.py'),
}
METADATA_HELPERS = ('tools/MetadataInspector.cs', 'tools/prepare_metadata_inspector.py', 'tools/metadata_compare.py')


def _sha(path):
    return hashlib.sha256(Path(path).read_bytes()).hexdigest()


def _report_hash(report):
    return hashlib.sha256(json.dumps({k: v for k, v in report.items() if k != 'verification_receipt'},
                                    sort_keys=True, separators=(',', ':')).encode('utf-8')).hexdigest()


def _path(root, value):
    path = Path(value)
    return path if path.is_absolute() else root / path


def _check_files(root, rows, reason):
    for row in rows:
        if _sha(_path(root, row.get('input_path', row['path']))) != row['sha256']:
            raise ValueError(reason + ': ' + row['path'])


def _check_receipt(root, assembly, report):
    if assembly not in ASSEMBLIES:
        raise ValueError('Unsupported metadata assembly: ' + assembly)
    receipt = report.get('verification_receipt')
    if not receipt or receipt.get('assembly') != assembly:
        raise ValueError('Missing or unsupported metadata receipt: ' + assembly)
    if receipt['report_sha256'] != _report_hash(report):
        raise ValueError('Metadata report changed since verification: ' + assembly)
    for path, expected in receipt['helper_source_hashes'].items():
        if _sha(root / path) != expected:
            raise ValueError('Metadata verification helper changed: ' + path)
    required = set(METADATA_HELPERS) | {'tools/kairo_candidate.py','tools/metadata_state.py', ASSEMBLIES[assembly][1], ASSEMBLIES[assembly][2], 'tools/compiler_probes.py'}
    if not required.issubset(receipt['helper_source_hashes']):
        raise ValueError('Metadata receipt does not cover every required verification helper')
    for key in ('build_artifact', 'original_artifact', 'manifest', 'compile_order', 'inspector', 'cecil'):
        if _sha(_path(root, receipt[key])) != receipt[key + '_sha256']:
            raise ValueError('Metadata verification input changed: ' + key)
    if report['rebuilt_sha256'] != receipt['build_artifact_sha256'] or report['original_sha256'] != receipt['original_artifact_sha256']:
        raise ValueError('Metadata report artifact identities differ from the receipt')
    _check_files(root, receipt['sources'], 'Source changed since metadata verification')
    _check_files(root, receipt['references'], 'Reference changed since metadata verification')
    _check_files(root, receipt['toolchain_binaries'], 'Compiler/runtime changed since metadata verification')
    return receipt


def apply_type_metadata(root, assembly, metadatareport):
    """Verifier API: persist proofs and replace only this assembly's type states.

    Returns the stored report, including regression names and its receipt. It
    does not build or compare binaries and never updates the method ledger.
    """
    root = Path(root).resolve()
    if assembly not in ASSEMBLIES:
        raise ValueError('Unsupported metadata assembly: ' + assembly)
    report = deepcopy(metadatareport)
    report.pop('verification_receipt', None)
    folder, verifier, builder = ASSEMBLIES[assembly]
    manifest_path = root / folder / 'build-manifest.json'
    manifest = json.loads(manifest_path.read_text(encoding='utf-8'))
    if manifest['exit_code'] != 0:
        raise ValueError('Metadata proof requires a successful full build')
    binary = root / folder / (assembly + '.dll')
    original = root / 'original/extracted/assets/bin/Data/Managed' / (assembly + '.dll')
    if report['rebuilt_sha256'] != _sha(binary) or manifest['output_sha256'] != _sha(binary):
        raise ValueError('Metadata proof does not belong to the current rebuilt assembly')
    if report['original_sha256'] != _sha(original):
        raise ValueError('Metadata proof does not belong to the original assembly')
    provenance = report['provenance']
    if set(provenance['source_hashes']) != set(METADATA_HELPERS):
        raise ValueError('Metadata producer provenance is incomplete')
    for path, expected in provenance['source_hashes'].items():
        if _sha(root / path) != expected:
            raise ValueError('Metadata producer changed after comparison: ' + path)
    columns = None
    with (root / 'config/types.csv').open(encoding='utf-8', newline='') as file:
        reader = csv.DictReader(file)
        columns, rows = reader.fieldnames, list(reader)
    selected = [row for row in rows if row['assembly'] == assembly]
    names = {row['tipo'].replace('+', '/') for row in selected}
    if names != set(report['types']) or len(names) != len(selected):
        raise ValueError('Type ledger differs from the metadata oracle inventory')
    regressions = []
    for row in selected:
        proof = report['types'][row['tipo'].replace('+', '/')]
        if int(row['ordem']) != proof['original_type_ordinal']:
            raise ValueError('Type ledger original ordinal differs from metadata oracle')
        if proof['equal'] != (not proof['differences']):
            raise ValueError('Contradictory type metadata proof')
        if proof['equal'] and proof['original_type_ordinal'] != proof['rebuilt_type_ordinal']:
            raise ValueError('Matched type proof has different TypeDef ordinal')
        if row['metadados'] == 'matched' and not proof['equal']:
            regressions.append(row['id'])
        row['metadados'] = 'matched' if proof['equal'] else 'different'
    report['type_state_regressions'] = regressions
    binaries = [{'path': manifest['compiler'], 'sha256': manifest['compiler_sha256']}]
    if 'runtime' in manifest:
        binaries.append({'path': manifest['runtime'], 'sha256': manifest['runtime_sha256']})
    helper_paths = set(METADATA_HELPERS) | {'tools/kairo_candidate.py','tools/metadata_state.py', verifier, builder, 'tools/compiler_probes.py'}
    receipt = {'schema_version': 1, 'assembly': assembly,
        'sources': manifest['sources'], 'references': manifest['references'], 'toolchain_binaries': binaries,
        'helper_source_hashes': {p: _sha(root / p) for p in sorted(helper_paths)}}
    files = {'build_artifact': binary, 'original_artifact': original, 'manifest': manifest_path,
        'compile_order': root / 'config/compile-order' / (assembly + '.txt'),
        'inspector': root / 'build/metadata-tools/MetadataInspector.exe', 'cecil': root / 'build/metadata-tools/Mono.Cecil.dll'}
    for key, path in files.items():
        receipt[key] = path.relative_to(root).as_posix()
        receipt[key + '_sha256'] = _sha(path)
    if provenance['inspector_sha256'] != receipt['inspector_sha256'] or provenance['cecil_sha256'] != receipt['cecil_sha256']:
        raise ValueError('Metadata inspector binary changed after comparison')
    receipt['report_sha256'] = _report_hash(report)
    report['verification_receipt'] = receipt
    _check_receipt(root, assembly, report)
    # No state is written before every input/proof has been checked.
    destination = root / 'reports' / (assembly + '-metadata.json')
    destination.parent.mkdir(parents=True, exist_ok=True)
    destination.write_text(json.dumps(report, indent=2, ensure_ascii=True) + '\n', encoding='utf-8')
    with (root / 'config/types.csv').open('w', encoding='utf-8', newline='') as file:
        writer = csv.DictWriter(file, fieldnames=columns, lineterminator='\n')
        writer.writeheader()
        writer.writerows(rows)
    return report


def validate_type_metadata(root, types):
    """Read-only stale-proof guard for progress and verification_state."""
    root = Path(root).resolve()
    measured = {}
    for row in types:
        if row['metadados'] in ('matched', 'different'):
            measured.setdefault(row['assembly'], []).append(row)
    reports = {}
    for assembly, rows in measured.items():
        path = root / 'reports' / (assembly + '-metadata.json')
        if not path.is_file():
            raise ValueError('Type metadata state without verification report: ' + assembly)
        report = json.loads(path.read_text(encoding='utf-8'))
        _check_receipt(root, assembly, report)
        for row in rows:
            proof = report['types'].get(row['tipo'].replace('+', '/'))
            if not proof or int(row['ordem']) != proof['original_type_ordinal']:
                raise ValueError('Type state lacks matching metadata identity: ' + row['id'])
            expected = 'matched' if proof['equal'] and not proof['differences'] and proof['original_type_ordinal'] == proof['rebuilt_type_ordinal'] else 'different'
            if row['metadados'] != expected:
                raise ValueError('Unsupported type metadata state: ' + row['id'])
        reports[assembly] = report
    return reports
