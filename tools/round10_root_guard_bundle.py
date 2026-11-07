"""Register tested guard compositions alongside the generic guard family.

Only diagnostic rule names change. The before/after C# and upstream fixture
evidence stay byte-for-byte equal, and the real compiler remains the judge.
"""
import json
from pathlib import Path
from compare_kairo import sha

ROOT=Path(__file__).resolve().parents[1]


def main():
    original=ROOT/'reports/round10-guard-composition-patches.json'
    document=json.loads(original.read_text(encoding='utf-8-sig'))
    old=[(p['method_id'],p['before'],p['after']) for p in document['patches']]
    dependencies={**document['dependency_sha256'],Path(__file__).relative_to(ROOT).as_posix():sha(Path(__file__))}
    evidence={**document['evidence_sha256'],original.relative_to(ROOT).as_posix():sha(original)}
    for p in document['patches']:
        p['upstream_rule']=p['rule']
        p['rule']='root-guard-positive-tail-composed-'+p['rule'].removeprefix('d10-guard-lifetime-composition-')
    assert old==[(p['method_id'],p['before'],p['after']) for p in document['patches']]
    document.update(dependency_sha256=dependencies,evidence_sha256=evidence,
                    upstream_bundle_sha256=sha(original),adapter_sha256=sha(Path(__file__)),
                    change_scope='Rule-family registration only; C# unchanged')
    target=ROOT/'reports/round10-root-guard-composition-patches.json';assert not target.exists()
    target.write_text(json.dumps(document,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({'proposals':len(document['patches']),'source_changed':False,'target':str(target.relative_to(ROOT))}))


if __name__=='__main__':main()
