"""Negative controls for metadata-state receipts, entirely in a temporary project."""
from copy import deepcopy
import csv
import hashlib
import json
from pathlib import Path
import tempfile
from metadata_state import apply_type_metadata, validate_type_metadata, METADATA_HELPERS


def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def read_types(root):
    with (root / 'config/types.csv').open(encoding='utf-8', newline='') as file:
        return list(csv.DictReader(file))


def main():
    results = []
    with tempfile.TemporaryDirectory(prefix='metadata-proof-') as temporary:
        root = Path(temporary)
        names = set(METADATA_HELPERS) | {'tools/kairo_candidate.py','tools/metadata_state.py', 'tools/verify_kairo.py',
            'tools/build_kairo.py', 'tools/compiler_probes.py', 'source.cs', 'reference.dll',
            'compiler.exe', 'build/metadata-tools/MetadataInspector.exe', 'build/metadata-tools/Mono.Cecil.dll',
            'config/compile-order/KairoLibrary.txt', 'build/KairoLibrary-roslyn-1.1.0/KairoLibrary.dll',
            'original/extracted/assets/bin/Data/Managed/KairoLibrary.dll'}
        for name in names:
            path = root / name
            path.parent.mkdir(parents=True, exist_ok=True)
            path.write_text('fixture:' + name, encoding='utf-8')
        rows = [{'id': 'KairoLibrary::T', 'assembly': 'KairoLibrary', 'tipo': 'T', 'ordem': '1', 'metadados': 'UNKNOWN'},
                {'id': 'Assembly-CSharp::U', 'assembly': 'Assembly-CSharp', 'tipo': 'U', 'ordem': '1', 'metadados': 'UNKNOWN'}]
        with (root / 'config/types.csv').open('w', encoding='utf-8', newline='') as file:
            writer = csv.DictWriter(file, fieldnames=list(rows[0]))
            writer.writeheader()
            writer.writerows(rows)
        binary = 'build/KairoLibrary-roslyn-1.1.0/KairoLibrary.dll'
        original = 'original/extracted/assets/bin/Data/Managed/KairoLibrary.dll'
        manifest = {'compiler': 'compiler.exe', 'compiler_sha256': sha(root / 'compiler.exe'),
            'sources': [{'path': 'source.cs', 'sha256': sha(root / 'source.cs')}],
            'references': [{'path': 'reference.dll', 'sha256': sha(root / 'reference.dll')}],
            'exit_code': 0, 'output_sha256': sha(root / binary)}
        (root / 'build/KairoLibrary-roslyn-1.1.0/build-manifest.json').write_text(json.dumps(manifest), encoding='utf-8')
        report = {'schema_version': 1, 'original_sha256': sha(root / original), 'rebuilt_sha256': sha(root / binary),
            'types': {'T': {'equal': True, 'differences': [], 'original_type_ordinal': 1, 'rebuilt_type_ordinal': 1}},
            'provenance': {'source_hashes': {p: sha(root / p) for p in METADATA_HELPERS},
                'inspector_sha256': sha(root / 'build/metadata-tools/MetadataInspector.exe'),
                'cecil_sha256': sha(root / 'build/metadata-tools/Mono.Cecil.dll')}}
        apply_type_metadata(root, 'KairoLibrary', report)
        current = read_types(root)
        assert current[0]['metadados'] == 'matched'
        assert current[1] == rows[1]
        validate_type_metadata(root, current)
        results.append({'control': 'verified_type_promoted_other_assembly_unchanged', 'pass': True})

        def rejected(action):
            try:
                action()
            except (ValueError, OSError):
                return
            raise AssertionError('Invalid metadata proof accepted')

        for name, label in [('source.cs', 'stale_source'), ('reference.dll', 'stale_reference'),
                (binary, 'stale_binary'), ('compiler.exe', 'stale_compiler'),
                ('tools/metadata_compare.py', 'stale_helper'),
                ('build/metadata-tools/MetadataInspector.exe', 'stale_inspector'),
                ('config/compile-order/KairoLibrary.txt', 'stale_compile_order'),
                (original, 'changed_original')]:
            path = root / name
            old = path.read_bytes()
            path.write_bytes(old + b'changed')
            rejected(lambda: validate_type_metadata(root, current))
            path.write_bytes(old)
            validate_type_metadata(root, current)
            results.append({'control': label + '_rejected', 'pass': True})

        path = root / 'reports/KairoLibrary-metadata.json'
        old = path.read_bytes()
        tampered = json.loads(old)
        tampered['types']['T']['rebuilt_type_ordinal'] += 1
        path.write_text(json.dumps(tampered), encoding='utf-8')
        rejected(lambda: validate_type_metadata(root, current))
        path.write_bytes(old)
        results.append({'control': 'tampered_report_rejected', 'pass': True})

        manual = deepcopy(current)
        manual[1]['metadados'] = 'matched'
        rejected(lambda: validate_type_metadata(root, manual))
        results.append({'control': 'manual_promotion_without_receipt_rejected', 'pass': True})

        bad = deepcopy(report)
        bad['types']['T']['rebuilt_type_ordinal'] = 2
        old_ledger = (root / 'config/types.csv').read_bytes()
        rejected(lambda: apply_type_metadata(root, 'KairoLibrary', bad))
        assert (root / 'config/types.csv').read_bytes() == old_ledger
        results.append({'control': 'contradictory_ordinal_rejected_without_writing_ledger', 'pass': True})

    root = Path(__file__).resolve().parents[1]
    (root / 'reports/metadata-state-negative-controls.json').write_text(json.dumps({
        'controls_total': len(results), 'controls': results}, indent=2) + '\n', encoding='utf-8')
    print(json.dumps({'controls_passed': len(results), 'controls_total': len(results)}))


if __name__ == '__main__':
    main()
