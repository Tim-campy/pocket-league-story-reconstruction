"""Recover two implicit static initializers as isolated C# candidates.

Config RHS expressions come from the independently decompiled C# constructor.
AdMob's empty initializer comes from a default-valued field initializer, a
lowering verified with our own fixture. No explicit static ctor is introduced.
"""
import argparse
import hashlib
import json
from pathlib import Path
import re
import subprocess
import sys
from compare_kairo import differences

ROOT=Path(__file__).resolve().parents[1]
FOLDER=ROOT/'analysis/variants/kairo-static-initializers'
CONFIG='src/KairoLibrary/kairo/common/cfg/Config.cs'
ADMOB='src/KairoLibrary/kairo/unity/native/AdMobPlugin.cs'
TARGETS=('kairo.common.cfg.Config','kairo.unity.native.AdMobPlugin')


def sha(path):return hashlib.sha256(path.read_bytes()).hexdigest()


def replace_initializer(source,name,rhs):
    pattern=re.compile(r'(?m)^(?P<declaration>[ \t]*(?:public|private|protected|internal)\s+static\s+(?:readonly\s+)?[\w.<>\[\],?]+\s+'
        +re.escape(name)+r')(?P<initializer>[ \t]*=[^\r\n;]*)?;(?P<tail>[ \t]*\r?)$')
    matches=list(pattern.finditer(source))
    if len(matches)!=1:raise ValueError('Expected exactly one static field declaration: '+name)
    match=matches[0]
    replacement=match['declaration']+' = '+rhs+';'+match['tail']
    return source[:match.start()]+replacement+source[match.end():]


def prepare():
    code=(ROOT/'analysis/variants/kairo-codemerx/kairo.common.cfg/Config.cs').read_text(encoding='utf-8-sig')
    block=re.search(r'\bstatic Config\(\)\s*\{(?P<body>.*?)\n\t\t\}',code,re.S)
    if block is None:raise ValueError('Independent Config constructor not found')
    assignments=[]
    for line in block['body'].splitlines():
        if not line.strip():continue
        match=re.fullmatch(r'\s*Config\.(?P<field>[A-Za-z_]\w*)\s*=\s*(?P<rhs>.+);\s*',line)
        if match is None:raise ValueError('Constructor requires statement reconstruction, not simple initializers')
        assignments.append((match['field'],match['rhs']))
    if len({name for name,_ in assignments})!=len(assignments):raise ValueError('Repeated field assignment')
    mapping={};inputs=[]
    for logical in (CONFIG,ADMOB):
        current=ROOT/logical;source=current.read_bytes().decode('utf-8')
        type_name='Config' if logical==CONFIG else 'AdMobPlugin'
        if re.search(r'\bstatic\s+'+type_name+r'\s*\(',source):raise ValueError('Main source already declares an explicit static constructor')
        original=source
        expressions=assignments if logical==CONFIG else [('app_','null')]
        for name,rhs in expressions:source=replace_initializer(source,name,rhs)
        if logical==CONFIG:
            declaration_positions=[re.search(r'\b'+re.escape(name)+r'\s*=',source).start() for name,_ in assignments]
            if declaration_positions!=sorted(declaration_positions):raise ValueError('Field order differs from the original constructor assignment order')
        destination=FOLDER/logical;destination.parent.mkdir(parents=True,exist_ok=True)
        destination.write_bytes(source.encode('utf-8'))
        mapping[logical]=str(destination)
        inputs.append({'logical_path':logical,'base_sha256':hashlib.sha256(original.encode('utf-8')).hexdigest(),
            'candidate_sha256':sha(destination),'changed_fields':[name for name,rhs in expressions],
            'explicit_static_constructor_added':False})
    path=ROOT/'analysis/kairo-static-initializers-source-map.json'
    path.write_text(json.dumps(mapping,indent=2)+'\n',encoding='utf-8',newline='\n')
    return mapping,inputs


def dump(assembly,output):
    with output.open('wb') as stream:
        subprocess.run([str(ROOT/'build/tools/ProbeInspector.exe'),str(assembly),'--structure'],stdout=stream,check=True)
    return json.loads(output.read_text(encoding='utf-8-sig'))


def verify(mapping,inputs,target_types=TARGETS,label_prefix='static-init',snapshot_folder=FOLDER):
    # Freeze all C# inputs before two full builds, so another agent's source
    # edits cannot corrupt the regression comparison while this trial runs.
    order=[p for p in (ROOT/'config/compile-order/KairoLibrary.txt').read_text().splitlines() if p and not p.startswith('#')]
    order.append('src/KairoLibrary/AssemblyInfo.cs')
    snapshot=snapshot_folder/'baseline-snapshot';base_map={}
    for logical in order:
        current=ROOT/logical
        expected=next((r['base_sha256'] for r in inputs if r['logical_path']==logical),None)
        if expected is not None and sha(current)!=expected:raise ValueError('Target source changed while preparing candidate')
        target=snapshot/logical;target.parent.mkdir(parents=True,exist_ok=True);target.write_bytes(current.read_bytes())
        base_map[logical]=str(target)
    candidate_map=dict(base_map);candidate_map.update(mapping)
    docs=[];build_records=[]
    compiler=ROOT/'analysis/tools/roslyn-1.3.2/tools/csc.exe'
    for label,selection in ((label_prefix+'-baseline',base_map),(label_prefix+'-candidate',candidate_map)):
        map_path=ROOT/'analysis'/(label+'-source-map.json');map_path.write_text(json.dumps(selection,indent=2)+'\n')
        result=subprocess.run([sys.executable,str(ROOT/'tools/build_kairo.py'),'--compiler',str(compiler),
            '--label',label,'--source-map',str(map_path)],cwd=ROOT,capture_output=True,text=True)
        if result.returncode:raise ValueError('Full isolated build failed: '+result.stdout+result.stderr)
        folder=ROOT/'build'/('KairoLibrary-'+label)
        manifest=json.loads((folder/'build-manifest.json').read_text())
        build_records.append({'label':label,'exit_code':manifest['exit_code'],'sha256':manifest['output_sha256'],
            'manifest_path':(folder/'build-manifest.json').relative_to(ROOT).as_posix()})
        docs.append(dump(folder/'KairoLibrary.dll',folder/'methods.json'))
    original=json.loads((ROOT/'build/KairoLibrary-roslyn-1.3.2/original-methods.json').read_text(encoding='utf-8-sig'))
    oracle={m['stable_id']:m for m in original['selected_methods']}
    before={m['stable_id']:m for m in docs[0]['selected_methods']}
    after={m['stable_id']:m for m in docs[1]['selected_methods']}
    regressions=[];gains=[]
    for key,method in oracle.items():
        if 'raw_body_hex' not in method:continue
        old_equal=not differences(method,before.get(key))[0];new_equal=not differences(method,after.get(key))[0]
        if old_equal and not new_equal:regressions.append(key)
        if new_equal and not old_equal:gains.append(key)
    type_states=[];target_states=[]
    original_types={t['name']:t for t in original['types']}
    before_types={t['name']:t for t in docs[0]['types']};after_types={t['name']:t for t in docs[1]['types']}
    for name in target_types:
        key=name+'::.cctor()->System.Void'
        delta,percent=differences(oracle[key],after.get(key))
        target_states.append({'id':key,'body_equal':not delta,'differences':delta,'percent':percent})
        type_states.append({'type':name,'original_attributes':original_types[name]['attributes'],
            'baseline_attributes':before_types[name]['attributes'],'candidate_attributes':after_types[name]['attributes'],
            'before_field_init_preserved':bool(after_types[name]['attributes']&0x100000),
            'fields_equal_to_original':original_types[name]['fields']==after_types[name]['fields'],
            'method_order_equal_to_original':original_types[name]['methods']==after_types[name]['methods']})
    success=not regressions and all(r['body_equal'] for r in target_states) and all(
        t['original_attributes']==t['candidate_attributes'] and t['fields_equal_to_original'] for t in type_states)
    return {'builds':build_records,'target_methods':target_states,'type_states':type_states,
        'previously_equal_bodies_lost':regressions,'new_equal_bodies':gains,'candidate_validated':success,
        'scope':'full baseline/candidate builds from frozen C#; all oracle bodies checked; no ledger changes'}


def main():
    parser=argparse.ArgumentParser(description=__doc__);parser.add_argument('--verify',action='store_true');args=parser.parse_args()
    mapping,inputs=prepare()
    report={'source_changes':inputs,'source_map':'analysis/kairo-static-initializers-source-map.json',
        'applied_to_main':False,'explicit_static_constructors_added':False}
    if args.verify:report['verification']=verify(mapping,inputs)
    (ROOT/'reports/KairoLibrary-static-initializers.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8',newline='\n')
    print(json.dumps(report))
    if args.verify and not report['verification']['candidate_validated']:raise SystemExit(1)


if __name__=='__main__':main()
