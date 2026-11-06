"""Isolated, frozen-baseline body/metadata measurement for two slot-order cases."""
import json
from pathlib import Path

from compare_kairo import differences, sha
from metadata_compare import compare_paths
from round3_metadata_candidates import compile_frozen, dump
from round4_engine import apply_patch, il_cells, prefix
from round4_transform import TransformClient

ROOT = Path(__file__).resolve().parents[1]


def read(path):
    return json.loads(Path(path).read_text(encoding='utf-8-sig'))


def main():
    assembly = 'Assembly-CSharp'
    base = ROOT / 'build/round4/baseline' / assembly
    manifest = read(base / 'build-manifest.json')
    original = {m['stable_id']: m for m in read(base / 'original-methods.json')['selected_methods']}
    baseline = {m['stable_id']: m for m in read(base / 'rebuilt-methods.json')['selected_methods']}
    equal = {key for key, method in original.items()
             if 'raw_body_hex' in method and not differences(method, baseline.get(key))[0]}
    targets = [
        ('GetSum', 'src/Assembly-CSharp/game/Team.cs', 'public int GetSum(',
         'for (int i = 0; i < CEVA_PASSER_MP2.Length; i++)', 'int num3;\nint num4;',
         'game.Team::GetSum(System.Int32[][],System.Int32,System.Int32,System.Boolean)->System.Int32'),
        ('ChMoveDirect', 'src/Assembly-CSharp/game/Player.cs', 'public bool ChMoveDirect(',
         'int[] array = null;', 'int num2;\nint num3;',
         'game.Player::ChMoveDirect(System.Int32,System.Int32)->System.Boolean'),
    ]
    records = []
    for name, logical, method_needle, needle, expected, method_id in targets:
        source = (base / logical).read_bytes().decode('utf-8-sig')
        index = source.index(needle, source.index(method_needle))
        with TransformClient(base / 'build-manifest.json') as client:
            result = client.request(ROOT / logical, source=source,
                                    offset=len(source[:index].encode('utf-16-le')) // 2,
                                    max_candidates=256)
        patch = next(p for p in result['patches']
                     if p['rule'] == 'local-declaration-scope-hoist' and p['after'].startswith(expected))
        folder = ROOT / 'analysis/variants' / ('round4-hoist-' + name)
        folder.mkdir(parents=True, exist_ok=True)
        candidate_source = folder / Path(logical).name
        candidate_source.write_text(apply_patch(source, patch), encoding='utf-8', newline='')
        mapping = {entry['path']: str(base / entry['path']) for entry in manifest['sources']}
        mapping[logical] = str(candidate_source)
        label = 'round4-hoist-' + name
        compiled = compile_frozen(assembly, label, manifest, mapping)
        record = {'name': name, 'method_id': method_id, 'rule': patch['rule'],
                  'compile_exit_code': compiled.returncode, 'patch': patch,
                  'main_sources_modified': False, 'ledgers_modified': False,
                  'baseline_matched_bodies': len(equal),
                  'baseline_artifact_sha256': manifest['output_sha256']}
        if compiled.returncode == 0:
            build = ROOT / 'build' / (assembly + '-' + label)
            binary = build / (assembly + '.dll')
            rebuilt = {m['stable_id']: m for m in dump(binary, build / 'probe.json')['selected_methods']}
            after_equal = {key for key, method in original.items()
                           if 'raw_body_hex' in method and not differences(method, rebuilt.get(key))[0]}
            metadata = compare_paths(ROOT / 'original/extracted/assets/bin/Data/Managed/Assembly-CSharp.dll',
                                     binary, build / 'metadata.json')
            record.update(
                candidate_artifact_sha256=sha(binary),
                methods_gained=sorted(after_equal - equal), methods_lost=sorted(equal - after_equal),
                body_equal_total=len(after_equal),
                target_body_equal=method_id in after_equal,
                target_facets=differences(original[method_id], rebuilt.get(method_id))[0],
                original_locals=original[method_id].get('locals'),
                baseline_locals=baseline[method_id].get('locals'),
                candidate_locals=rebuilt[method_id].get('locals'),
                original_code_bytes=original[method_id]['header']['code_size'],
                old_first_offset=prefix(original[method_id], baseline[method_id]),
                new_first_offset=prefix(original[method_id], rebuilt[method_id]),
                prefix_preserved=il_cells(original[method_id])[:prefix(original[method_id], baseline[method_id])] == il_cells(rebuilt[method_id])[:prefix(original[method_id], baseline[method_id])],
                types_equal=metadata['types_equal'], types_total=metadata['types_total'],
                whole_structural_metadata_equal=metadata['whole_structural_metadata_equal'],
                metadata_global_differences=metadata['global_differences'])
        else:
            record['compiler_log'] = (compiled.stdout + compiled.stderr).decode('utf-8', errors='replace')
        records.append(record)
        report = {'scope': 'two isolated source-only candidates against frozen round-4 baseline',
                  'official_states_adopted': False, 'cases': records}
        (ROOT / 'reports/round4-hoist-probe.json').write_text(json.dumps(report, indent=2) + '\n', encoding='utf-8')
        print(json.dumps({k: v for k, v in record.items() if k not in ('patch', 'original_locals', 'baseline_locals', 'candidate_locals')}), flush=True)


if __name__ == '__main__':
    main()
