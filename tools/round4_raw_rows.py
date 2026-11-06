"""Summarize row counts of raw-different metadata tables, without repairs."""
import argparse
import json
from pathlib import Path

from compare_kairo import sha

ROOT = Path(__file__).resolve().parents[1]


def summarize(path):
    path = Path(path).resolve()
    report = json.loads(path.read_text(encoding='utf-8-sig'))
    rows = []
    for table in report['metadata_tables']:
        if table['different_or_unpaired_bytes'] == 0:
            continue
        original = table['rows']['original_rows']
        rebuilt = table['rows']['rebuilt_rows']
        rows.append({'table': table['name'], 'original_rows': original, 'rebuilt_rows': rebuilt,
                     'row_count_equal': original == rebuilt,
                     'raw_different_or_unpaired_bytes': table['different_or_unpaired_bytes']})
    return {'measurement': path.relative_to(ROOT).as_posix(), 'measurement_sha256': sha(path),
            'original_sha256': report['original']['sha256'], 'rebuilt_sha256': report['rebuilt']['sha256'],
            'different_tables': rows,
            'different_row_count_tables': [row['table'] for row in rows if not row['row_count_equal']]}


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--kairo', type=Path, required=True)
    parser.add_argument('--app', type=Path, required=True)
    parser.add_argument('--label', required=True)
    args = parser.parse_args()
    result = {'schema_version': 1, 'operation': 'raw-row-count-measurement-only',
              'corrections_applied': False, 'assemblies': {'KairoLibrary': summarize(args.kairo), 'Assembly-CSharp': summarize(args.app)}}
    output = ROOT / 'reports' / ('round4-' + args.label + '-raw-row-counts.json')
    output.write_text(json.dumps(result, indent=2) + '\n', encoding='utf-8')
    print(output.relative_to(ROOT).as_posix())


if __name__ == '__main__':
    main()
