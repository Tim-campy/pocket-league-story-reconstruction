"""Negative controls for metadata proof, using the admitted original metadata.

The target is only parsed as metadata. Each control changes one evidence facet;
the comparison must reject it without changing the method/type ledger.
"""
from copy import deepcopy
import json
from metadata_compare import ROOT, compare_metadata, inspect_metadata


def main():
    original = inspect_metadata(ROOT / 'original/extracted/assets/bin/Data/Managed/Assembly-CSharp.dll')
    same = compare_metadata(original, original)
    assert same['types_equal'] == same['types_total'] == 129
    assert same['declarations_proved'] == same['declarations_total'] == 6
    assert same['whole_structural_metadata_equal']
    results = [{'control': 'original_self_comparison', 'pass': True}]

    candidate = deepcopy(original)
    candidate['types'][1:3] = reversed(candidate['types'][1:3])
    report = compare_metadata(original, candidate)
    assert not report['whole_structural_metadata_equal'] and report['type_difference_counts_nonexclusive']['type_order'] == 2
    results.append({'control': 'reordered_typedefs_rejected', 'pass': True})

    candidate = deepcopy(original)
    target = next(t for t in candidate['types'] if any(f['has_constant'] for f in t['fields']))
    field = next(f for f in target['fields'] if f['has_constant'])
    field['constant'] = {'type': 'System.String', 'value': 'deliberate-negative-control'}
    report = compare_metadata(original, candidate)
    assert not report['types'][target['name']]['equal'] and 'fields_metadata' in report['types'][target['name']]['differences']
    results.append({'control': 'changed_constant_rejected', 'pass': True})

    candidate = deepcopy(original)
    target = next(t for t in candidate['types'] if any(not m['has_body'] and m['parameters'] for m in t['methods']))
    method = next(m for m in target['methods'] if not m['has_body'] and m['parameters'])
    method['parameters'][0]['name'] += '_changed'
    report = compare_metadata(original, candidate)
    assert not report['declarations'][method['id']]['proved']
    assert not report['declarations'][method['id']]['metadata_equal']
    assert report['declarations_proved'] < 6
    results.append({'control': 'changed_no_body_parameter_rejected', 'pass': True})

    candidate = deepcopy(original)
    target = next(t for t in candidate['types'] if t['custom_attributes'])
    target['custom_attributes'][0]['blob'] += '-00'
    report = compare_metadata(original, candidate)
    assert 'custom_attributes' in report['types'][target['name']]['differences']
    results.append({'control': 'changed_attribute_blob_rejected', 'pass': True})

    candidate = deepcopy(original)
    target = candidate['types'][1]
    target['class_size'] += 1
    report = compare_metadata(original, candidate)
    assert 'class_size' in report['types'][target['name']]['differences']
    results.append({'control': 'changed_layout_rejected', 'pass': True})

    candidate = deepcopy(original)
    target = next(t for t in candidate['types'] if len(t['methods']) > 1)
    target['methods'][0:2] = reversed(target['methods'][0:2])
    report = compare_metadata(original, candidate)
    assert 'methods_order' in report['types'][target['name']]['differences']
    results.append({'control': 'changed_method_order_rejected', 'pass': True})

    candidate = deepcopy(original)
    target = next(t for t in candidate['types'] if len(t['fields']) > 1)
    target['fields'][0:2] = reversed(target['fields'][0:2])
    report = compare_metadata(original, candidate)
    assert 'fields_order' in report['types'][target['name']]['differences']
    results.append({'control': 'changed_field_order_rejected', 'pass': True})

    candidate = deepcopy(original)
    candidate['resources'].append({'name': 'negative-control', 'kind': 'Embedded', 'attributes': 1, 'data': '00'})
    report = compare_metadata(original, candidate)
    assert not report['resources_equal'] and not report['global_metadata_equal'] and not report['whole_structural_metadata_equal']
    results.append({'control': 'changed_resource_rejected', 'pass': True})

    candidate = deepcopy(original)
    target = candidate['types'].pop()
    report = compare_metadata(original, candidate)
    assert report['types'][target['name']]['differences'] == ['missing_type']
    assert not report['whole_structural_metadata_equal']
    results.append({'control': 'missing_type_rejected', 'pass': True})

    candidate = deepcopy(original)
    candidate['types'].append(deepcopy(candidate['types'][1]))
    try:
        compare_metadata(original, candidate)
    except ValueError:
        pass
    else:
        raise AssertionError('Duplicate identity must fail closed')
    results.append({'control': 'ambiguous_identity_rejected', 'pass': True})

    candidate = deepcopy(original)
    candidate['coordinates']['mvid'] = 'different-coordinate'
    report = compare_metadata(original, candidate)
    assert report['whole_structural_metadata_equal'] and report['raw_tables_heaps_pe_identity'] == 'NOT_EVALUATED'
    results.append({'control': 'structural_result_does_not_claim_raw_binary_identity', 'pass': True})

    for key in ('attributes', 'hash_algorithm'):
        candidate = deepcopy(original)
        candidate['assembly_definition'][key] ^= 1
        report = compare_metadata(original, candidate)
        assert 'assembly_definition' in report['global_differences']
        assert not report['whole_structural_metadata_equal']
        results.append({'control': 'changed_assembly_' + key + '_rejected', 'pass': True})
    candidate = deepcopy(original)
    candidate['assembly_definition']['culture'] = 'negative-control'
    report = compare_metadata(original, candidate)
    assert 'assembly_definition' in report['global_differences']
    results.append({'control': 'changed_assembly_culture_rejected', 'pass': True})
    candidate = deepcopy(original)
    candidate['module_definition']['runtime_version'] += '-changed'
    report = compare_metadata(original, candidate)
    assert 'module_definition' in report['global_differences']
    results.append({'control': 'changed_module_runtime_version_rejected', 'pass': True})

    output = ROOT / 'reports/metadata-negative-controls.json'
    output.write_text(json.dumps({'controls_total': len(results), 'controls': results}, indent=2) + '\n', encoding='utf-8')
    print(json.dumps({'controls_passed': len(results), 'controls_total': len(results)}))


if __name__ == '__main__':
    main()
