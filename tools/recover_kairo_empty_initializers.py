"""Restore two omitted default-only static initializers, outside main source.

Uses the empirically verified Roslyn 1.3.2 default-initializer lowering. This
does not introduce explicit constructors or change the BeforeFieldInit flag.
"""
import argparse
import hashlib
import json
from pathlib import Path
import re
from recover_kairo_initializers import replace_initializer,verify

ROOT=Path(__file__).resolve().parents[1]
FOLDER=ROOT/'analysis/variants/kairo-empty-initializers'
PLANS=[('kairo.unity.ui.SoundPlayer','src/KairoLibrary/kairo/unity/ui/SoundPlayer.cs','instance_'),
    ('kairo.unity.util.CRC64','src/KairoLibrary/kairo/unity/util/CRC64.cs','crcTable_')]


def main():
    parser=argparse.ArgumentParser(description=__doc__);parser.add_argument('--verify',action='store_true');args=parser.parse_args()
    oracle=json.loads((ROOT/'build/KairoLibrary-roslyn-1.3.2/original-methods.json').read_text(encoding='utf-8-sig'))
    types={t['name']:t for t in oracle['types']};methods={m['stable_id']:m for m in oracle['selected_methods']}
    mapping={};inputs=[]
    for name,logical,field in PLANS:
        target=name+'::.cctor()->System.Void';definition=methods[target]
        if types[name]['attributes']!=1048577 or [i['opcode'] for i in definition['instructions']]!=['ret']:
            raise ValueError('Expected BeforeFieldInit and a default-only empty original static initializer')
        path=ROOT/logical;data=path.read_bytes();source=data.decode('utf-8')
        if re.search(r'\bstatic\s+'+re.escape(name.rsplit('.',1)[1])+r'\s*\(',source):
            raise ValueError('Main source already declares an explicit static constructor')
        result=replace_initializer(source,field,'null')
        output=FOLDER/logical;output.parent.mkdir(parents=True,exist_ok=True);output.write_bytes(result.encode('utf-8'))
        mapping[logical]=str(output)
        inputs.append({'logical_path':logical,'base_sha256':hashlib.sha256(data).hexdigest(),
            'candidate_sha256':hashlib.sha256(output.read_bytes()).hexdigest(),'changed_fields':[field],
            'explicit_static_constructor_added':False})
    map_path=ROOT/'analysis/kairo-empty-initializers-source-map.json';map_path.write_text(json.dumps(mapping,indent=2)+'\n')
    report={'scope_version':3,'source_changes':inputs,'source_map':map_path.relative_to(ROOT).as_posix(),
        'applied_to_main':False,'explicit_static_constructors_added':False,
        'rule_evidence':'reports/KairoLibrary-static-initializer-probe.json'}
    if args.verify:report['verification']=verify(mapping,inputs,target_types=tuple(p[0] for p in PLANS),
        label_prefix='empty-static-init',snapshot_folder=FOLDER)
    (ROOT/'reports/KairoLibrary-empty-initializers.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8',newline='\n')
    print(json.dumps(report))
    if args.verify and not report['verification']['candidate_validated']:raise SystemExit(1)


if __name__=='__main__':main()
