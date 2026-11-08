"""Group authenticated disjoint source proposals without altering any C# payload."""
import argparse
import copy
import json
from pathlib import Path
from compare_kairo import sha

ROOT = Path(__file__).resolve().parents[1]
MAPS = ('dependency_sha256', 'evidence_sha256', 'frozen_evidence_sha256',
        'historical_frozen_evidence_sha256', 'pinned_compiler_source_sha256', 'own_fixture_source_sha256')


def merge(documents):
    result = dict(source_only=True, target_compilations=0, patches=[], source_payloads_unchanged=True)
    ids = set()
    for document in documents:
        for row in document['patches']:
            if row['method_id'] in ids:
                raise ValueError('Duplicate method proposal')
            ids.add(row['method_id'])
            result['patches'].append(copy.deepcopy(row))
        for key in MAPS:
            for name, value in document.get(key, {}).items():
                target = result.setdefault(key, {})
                if name in target and target[name] != value:
                    raise ValueError('Conflicting dependency: ' + name)
                target[name] = value
    return result


def main():
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument('--input', action='append', required=True, help='path=sha256')
    p.add_argument('--output', required=True)
    a = p.parse_args()
    documents, evidence = [], {}
    for item in a.input:
        name, digest = item.rsplit('=', 1)
        path = ROOT / name
        assert sha(path) == digest
        documents.append(json.loads(path.read_text(encoding='utf-8-sig')))
        evidence[name] = digest
    result = merge(documents)
    result.setdefault('evidence_sha256', {}).update(evidence)
    result.setdefault('dependency_sha256', {})['tools/round12_merge_source_bundles.py'] = sha(Path(__file__))
    result['grouped_parent_bundles'] = evidence
    out = ROOT / a.output
    assert not out.exists()
    out.write_text(json.dumps(result, indent=2) + '\n', encoding='utf-8')
    print(json.dumps(dict(patches=len(result['patches']), output=a.output, sha256=sha(out))))


if __name__ == '__main__':
    main()
