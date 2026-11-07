"""Keep distinct first-divergence families in separate measured queue cycles."""
import json
from pathlib import Path
from compare_kairo import sha

ROOT = Path(__file__).resolve().parents[1]


def main():
    parent = ROOT / 'reports/round10-roslyn-large-patches.json'
    document = json.loads(parent.read_text(encoding='utf-8'))
    targets = {
        'seb': 'kairo.unity.ui.Seb::_load(java.io.InputStream)->System.Void',
        'resource': 'kairo.unity.ui.ResourceManager/<>c__DisplayClass51_0::<LoadStart>b__0()->System.Void',
        'http': 'kairo.unity.io.HttpObject/<>c__DisplayClass10_0::<_connect>b__2()->System.Void',
    }
    assert {p['method_id'] for p in document['patches']} == set(targets.values())
    written = []
    for label, identity in targets.items():
        selected = next(p for p in document['patches'] if p['method_id'] == identity)
        child = dict(document)
        child['patches'] = [selected]
        child['evidence_sha256'] = dict(document['evidence_sha256'])
        child['evidence_sha256'][parent.relative_to(ROOT).as_posix()] = sha(parent)
        child['dependency_sha256'] = dict(document['dependency_sha256'])
        child['dependency_sha256'][Path(__file__).relative_to(ROOT).as_posix()] = sha(Path(__file__))
        child['publication_split'] = 'Distinct aligned pairs; same immutable C# proposal and fixture proofs'
        assert child['patches'][0]['before'] == selected['before']
        assert child['patches'][0]['after'] == selected['after']
        target = ROOT / 'reports' / ('round10-roslyn-large-' + label + '-patches.json')
        assert not target.exists()
        target.write_text(json.dumps(child, indent=2) + '\n', encoding='utf-8')
        written.append(target.relative_to(ROOT).as_posix())
    print(json.dumps({'source_text_changed': False, 'target_compilations': 0,
                      'separate_bundles': written}))


if __name__ == '__main__':
    main()
