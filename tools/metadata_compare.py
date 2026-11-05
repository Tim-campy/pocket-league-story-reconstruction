"""Read-only structural metadata comparison, without editing ledgers or sources.

Type success requires the original TypeDef ordinal and all recorded members,
including their order and metadata, to agree. Method bodies are checked by the
existing body verifier. A no-body declaration proof requires both an identical
method declaration and an identical declaring type. No mismatching metadata is
normalized. PE/table heap byte identity is a separate, stronger check.
"""
import argparse
from collections import Counter
import hashlib
import json
from pathlib import Path
import subprocess
from prepare_metadata_inspector import ROOT, ensure_inspector


def _index(rows, key):
    indexed = {row[key]: row for row in rows}
    if len(indexed) != len(rows):
        raise ValueError('Ambiguous metadata identity: ' + key)
    return indexed


def compare_metadata(original, rebuilt):
    """Compare MetadataInspector dictionaries; return evidence, never mutate inputs."""
    a = _index(original['types'], 'name')
    b = _index(rebuilt['types'], 'name')
    a_order = {name: n for n, name in enumerate(a)}
    b_order = {name: n for n, name in enumerate(b)}
    types, declarations, group_counts = {}, {}, Counter()
    for name, left in a.items():
        right = b.get(name)
        differences = []
        if right is None:
            differences = ['missing_type']
        else:
            if a_order[name] != b_order[name]:
                differences.append('type_order')
            for key in left:
                if left[key] != right.get(key):
                    if key in ('methods', 'fields', 'properties', 'events'):
                        id_key = 'id'
                        la, rb = _index(left[key], id_key), _index(right[key], id_key)
                        if set(la) != set(rb):
                            differences.append(key + '_inventory')
                        if list(la) != list(rb):
                            differences.append(key + '_order')
                        if any(la[k] != rb[k] for k in la.keys() & rb.keys()):
                            differences.append(key + '_metadata')
                    else:
                        differences.append(key)
            extra_keys = set(right) - set(left)
            if extra_keys:
                raise ValueError('Inspector schemas differ: ' + ','.join(sorted(extra_keys)))
        types[name] = {'equal': not differences, 'differences': differences,
                       'original_type_ordinal': a_order[name] + 1,
                       'rebuilt_type_ordinal': b_order.get(name, -1) + 1 if right is not None else None}
        group_counts.update(differences)
        other_methods = _index(right['methods'], 'id') if right else {}
        other_order = {m['id']: i for i, m in enumerate(right['methods'])} if right else {}
        for i, method in enumerate(left['methods']):
            if method['has_body']:
                continue
            candidate = other_methods.get(method['id'])
            equal = candidate == method and other_order.get(method['id']) == i
            declarations[method['id']] = {'metadata_equal': equal,
                'declaring_type_metadata_equal': not differences,
                'proved': equal and not differences,
                'differences': ([] if equal else ['method_declaration_or_order']) +
                    ([] if not differences else ['declaring_type_metadata']),
                'declaring_type': name}
    global_keys = ('assembly', 'assembly_definition', 'module_definition', 'assembly_attributes', 'module_attributes', 'assembly_security',
                   'assembly_references', 'module_references', 'resources', 'exported_types')
    global_differences = [key for key in global_keys if original[key] != rebuilt[key]]
    return {'schema_version': 1, 'original_sha256': original['artifact_sha256'],
            'rebuilt_sha256': rebuilt['artifact_sha256'], 'types': types,
            'types_total': len(a), 'types_equal': sum(t['equal'] for t in types.values()),
            'type_difference_counts_nonexclusive': dict(sorted(group_counts.items())),
            'extra_types': sorted(set(b) - set(a)),
            'type_order_equal': list(a) == list(b), 'declarations': declarations,
            'declarations_total': len(declarations),
            'declarations_proved': sum(d['proved'] for d in declarations.values()),
            'declarations_method_metadata_equal': sum(d['metadata_equal'] for d in declarations.values()),
            'global_metadata_equal': not global_differences,
            'global_differences': global_differences,
            'resources_equal': original['resources'] == rebuilt['resources'],
            'original_resource_count': len(original['resources']),
            'rebuilt_resource_count': len(rebuilt['resources']),
            'whole_structural_metadata_equal': not global_differences and len(a) == len(b) and all(t['equal'] for t in types.values()),
            'raw_tables_heaps_pe_identity': 'NOT_EVALUATED',
            'limitations': original.get('limitations', [])}


def inspect_metadata(binary, cache_dir=None):
    binary = Path(binary).resolve()
    exe = ensure_inspector()
    sha = lambda p: hashlib.sha256(p.read_bytes()).hexdigest()
    inputs = {'binary_sha256': sha(binary), 'inspector_sha256': sha(exe)}
    cache_dir = Path(cache_dir or ROOT / 'build/metadata-cache')
    cache_dir.mkdir(parents=True, exist_ok=True)
    key = hashlib.sha256(json.dumps(inputs, sort_keys=True).encode()).hexdigest()
    output = cache_dir / (key + '.json')
    if output.is_file():
        result = json.loads(output.read_text(encoding='utf-8-sig'))
        if result['artifact_sha256'] != inputs['binary_sha256']:
            raise ValueError('Metadata cache artifact hash mismatch')
        return result
    result = subprocess.run([str(exe), str(binary)], check=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE)
    document = json.loads(result.stdout.decode('utf-8-sig'))
    if document['artifact_sha256'] != inputs['binary_sha256']:
        raise ValueError('Artifact changed during inspection')
    output.write_text(json.dumps(document, ensure_ascii=True, indent=2) + '\n', encoding='utf-8')
    return document


def compare_paths(original, rebuilt, output=None):
    report = compare_metadata(inspect_metadata(original), inspect_metadata(rebuilt))
    sha = lambda p: hashlib.sha256(p.read_bytes()).hexdigest()
    exe = ensure_inspector()
    report['provenance'] = {
        'source_hashes': {p: sha(ROOT / p) for p in ('tools/MetadataInspector.cs',
            'tools/prepare_metadata_inspector.py', 'tools/metadata_compare.py')},
        'inspector_sha256': sha(exe), 'cecil_sha256': sha(exe.parent / 'Mono.Cecil.dll'),
        'inspection': 'read-only PE/CLI parsing; no target CLR load or execution',
    }
    if output:
        Path(output).parent.mkdir(parents=True, exist_ok=True)
        Path(output).write_text(json.dumps(report, ensure_ascii=True, indent=2) + '\n', encoding='utf-8')
    return report


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--original', required=True, type=Path)
    parser.add_argument('--rebuilt', required=True, type=Path)
    parser.add_argument('--output', required=True, type=Path)
    args = parser.parse_args()
    r = compare_paths(args.original, args.rebuilt, args.output)
    print(json.dumps({k: r[k] for k in ('types_total', 'types_equal', 'declarations_total',
          'declarations_proved', 'type_difference_counts_nonexclusive', 'global_differences')}))


if __name__ == '__main__':
    main()
