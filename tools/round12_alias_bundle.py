"""Give related frozen source patches one diagnostic family; never change C# payloads."""
import argparse
import json
from pathlib import Path
from compare_kairo import sha

ROOT = Path(__file__).resolve().parents[1]


def main():
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument('--bundle', required=True, type=Path)
    p.add_argument('--bundle-sha256', required=True)
    p.add_argument('--prefix', required=True)
    p.add_argument('--output', required=True, type=Path)
    args = p.parse_args()
    if sha(args.bundle) != args.bundle_sha256:
        raise ValueError('Frozen parent bundle changed')
    doc = json.loads(args.bundle.read_text(encoding='utf-8-sig'))
    for row in doc['patches']:
        old = row['rule']
        row.setdefault('original_rule', old)
        row['rule'] = args.prefix + old
    doc.setdefault('dependency_sha256', {})[Path(__file__).relative_to(ROOT).as_posix()] = sha(Path(__file__))
    doc.setdefault('evidence_sha256', {})[args.bundle.as_posix()] = args.bundle_sha256
    doc['family_alias'] = dict(parent=str(args.bundle), parent_sha256=args.bundle_sha256,
        prefix=args.prefix, source_payloads_unchanged=True, target_compilations=0)
    args.output.write_text(json.dumps(doc, indent=2) + '\n', encoding='utf-8')
    print(json.dumps(dict(output=str(args.output), sha256=sha(args.output), patches=len(doc['patches']))))


if __name__ == '__main__':
    main()
