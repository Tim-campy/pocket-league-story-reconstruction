"""Test separate early guards in two closure-labelled methods, in frozen C#.

The delegates and their generated methods are untouched. Only isolated sources
are written; accepting main changes requires a separate full verification.
"""
import json
from pathlib import Path
import subprocess
import sys
from compare_kairo import differences,sha

ROOT=Path(__file__).resolve().parents[1]
PREFIX='kairo.unity.ui.Image::'
PATCHES={
    'Use':('if ((object)image_ != null || (src_ == null && file_ == null))',
        'if ((object)image_ != null)\n\t\t\t{\n\t\t\t\treturn;\n\t\t\t}\n\t\t\tif (src_ == null && file_ == null)'),
    'Unuse':('if ((object)image_ == null || (!isCreateImage_ && src_ == null && file_ == null))',
        'if ((object)image_ == null)\n\t\t\t{\n\t\t\t\treturn;\n\t\t\t}\n\t\t\tif (!isCreateImage_ && src_ == null && file_ == null)')}


def main():
    snapshot=ROOT/'analysis/variants/kairo-lock-frozen'
    mapping=json.loads((snapshot/'source-map.json').read_text())
    receipt=json.loads((snapshot/'receipt.json').read_text())
    for source in receipt['sources']:
        if sha(Path(mapping[source['path']]))!=source['sha256']:raise ValueError('Frozen source hash changed')
    original=json.loads((ROOT/'build/KairoLibrary-roslyn-1.3.2/original-methods.json').read_text(encoding='utf-8-sig'))
    before=json.loads((ROOT/'build/KairoLibrary-lock-frozen/rebuilt-methods.json').read_text(encoding='utf-8-sig'))
    oracle={m['stable_id']:m for m in original['selected_methods']};baseline={m['stable_id']:m for m in before['selected_methods']}
    existing={key for key,m in oracle.items() if 'raw_body_hex' in m and not differences(m,baseline.get(key))[0]}
    logical='src/KairoLibrary/kairo/unity/ui/Image.cs';text=Path(mapping[logical]).read_bytes().decode('utf-8-sig')
    folder=ROOT/'analysis/variants/kairo-closure-guards';folder.mkdir(parents=True,exist_ok=True);results=[]
    for name,(needle,replacement) in PATCHES.items():
        if text.count(needle)!=1:raise ValueError('Guard shape changed: '+name)
        replacement=replacement.replace('\n','\r\n') if '\r\n' in text else replacement
        edited=text.replace(needle,replacement);output=folder/name/'Image.cs';output.parent.mkdir(exist_ok=True)
        output.write_bytes(edited.encode('utf-8'));selected=dict(mapping);selected[logical]=str(output)
        plan=output.parent/'source-map.json';plan.write_text(json.dumps(selected))
        label='closure-guard-'+name.lower()
        subprocess.run([sys.executable,str(ROOT/'tools/build_kairo.py'),'--compiler',str(ROOT/'analysis/tools/roslyn-1.3.2/tools/csc.exe'),
            '--label',label,'--source-map',str(plan)],cwd=ROOT,check=True,capture_output=True)
        build=ROOT/'build'/('KairoLibrary-'+label);dump=build/'rebuilt-methods.json'
        with dump.open('wb') as file:subprocess.run([str(ROOT/'build/tools/ProbeInspector.exe'),str(build/'KairoLibrary.dll'),'--structure'],stdout=file,check=True)
        candidates={m['stable_id']:m for m in json.loads(dump.read_text(encoding='utf-8-sig'))['selected_methods']}
        key=PREFIX+name+'()->System.Void';delta,_=differences(oracle[key],candidates.get(key))
        lost=[previous for previous in existing if differences(oracle[previous],candidates.get(previous))[0]]
        results.append({'id':key,'candidate_body_equal':not delta,'differences':delta,'previously_equal_bodies_lost':lost,
            'before_source_sha256':sha(Path(mapping[logical])),'candidate_source':output.relative_to(ROOT).as_posix(),
            'source_needle':needle,'source_replacement':replacement,'candidate_sha256':sha(output),
            'original_body_size':len(oracle[key]['raw_body_hex'].replace('-',''))//2,
            'build_artifact_sha256':sha(build/'KairoLibrary.dll'),'main_source_modified':False})
    (ROOT/'reports/KairoLibrary-closure-guard-probes.json').write_text(json.dumps({'frozen_body_equal':len(existing),'results':results},indent=2)+'\n',encoding='utf-8')
    print(json.dumps({'results':[{'id':r['id'],'body_equal':r['candidate_body_equal'],'losses':len(r['previously_equal_bodies_lost'])} for r in results]}))


if __name__=='__main__':main()
