"""Fixture-check guard/scope compositions, then publish C# hypotheses only."""
import json
from pathlib import Path
from compare_kairo import differences,sha
from round9_part_b_fixtures import compile_smcs
from round7_roslyn_for_declaration_fixture import compile_fixture as compile_roslyn
import round10_guard_composition_source as api
from round10_training_source import extract

ROOT=Path(__file__).resolve().parents[1]
RULE='d10-guard-lifetime-composition'


def main():
    work=ROOT/'build/round10/guard-composition-fixtures';assert not work.exists()
    templates=ROOT/'tools/fixtures/round10-guard-compositions'
    specs=[('Canvas','KairoLibrary',api.transform_canvas,'Fixture::Run()->System.Void','public void Run()',
            'src/KairoLibrary/kairo/unity/ui/Canvas.cs','public void OnInputUpdate()','kairo.unity.ui.Canvas::OnInputUpdate()->System.Void'),
           ('Sp','Assembly-CSharp',api.transform_sp_constructor,'Fixture::.ctor(System.Exception)->System.Void','public Fixture(Exception th)',
            'src/Assembly-CSharp/form/SpForm.cs','public SpForm(Exception th)','form.SpForm::.ctor(System.Exception)->System.Void')]
    rows=[];patches=[];dependencies={p.relative_to(ROOT).as_posix():sha(p) for p in
       [Path(__file__),ROOT/'tools/round10_guard_composition_source.py',ROOT/'tools/round9_whole_guard_source.py',ROOT/'tools/round10_training_source.py']}
    for label,assembly,transform,key,marker,logical,target_marker,target_id in specs:
        base=ROOT/'build/round10/baseline'/assembly
        manifest=json.loads((base/'build-manifest.json').read_text(encoding='utf-8-sig'))
        bp,cp=templates/(label+'Current.cs'),templates/(label+'Control.cs')
        before=bp.read_text(encoding='utf-8-sig');control=cp.read_text(encoding='utf-8-sig')
        start,end,method=extract(before,marker)
        candidate=before[:start]+transform(method)+before[end:]
        if assembly=='KairoLibrary':
            def compile_one(folder,source):return compile_roslyn(folder,source,manifest)[0]
        else:
            def compile_one(folder,source):return compile_smcs(folder,source,manifest)
        a,b,c=[compile_one(work/label/tag,text)[key] for tag,text in
               [('before',before),('control',control),('candidate',candidate)]]
        assert differences(a,b)[0],label
        assert not differences(b,c)[0],(label,differences(b,c)[0])
        dependencies.update({p.relative_to(ROOT).as_posix():sha(p) for p in (bp,cp)})
        rows.append({'fixture':label,'compiler_sha256':manifest['compiler_sha256'],
                     'candidate_matches_manual_control':True,'before_body_different':True,
                     'before_locals':len(a['locals']),'control_locals':len(b['locals']),
                     'original_control':control,'candidate':candidate})
        source=(base/logical).read_bytes().decode('utf-8-sig')
        start,end,target=extract(source,target_marker)
        patches.append({'rule':RULE+'-'+label.lower(),'method_id':target_id,'logical_path':logical,
                        'start':len(source[:start].encode('utf-16-le'))//2,'end':len(source[:end].encode('utf-16-le'))//2,
                        'before':target,'after':transform(target)})
    fixture={'operation':'round10-guard-lifetime-independent-fixtures','toy_compilations':6,'game_compilations':0,
             'positive_full_bodies':len(rows),'fixtures':rows,'dependencies':dependencies,
             'target_body_status':'UNKNOWN_UNTIL_VERIFIER'}
    fp=ROOT/'reports/round10-guard-composition-fixtures.json';assert not fp.exists()
    fp.write_text(json.dumps(fixture,indent=2)+'\n',encoding='utf-8')
    bp=ROOT/'reports/round10-guard-composition-patches.json';assert not bp.exists()
    bp.write_text(json.dumps({'calculator_sha256':sha(Path(__file__)),'dependency_sha256':dependencies,
      'evidence_sha256':{fp.relative_to(ROOT).as_posix():sha(fp)},'patches':patches},indent=2)+'\n',encoding='utf-8')
    print(json.dumps({'toy_compilations':6,'positive_full_bodies':len(rows),'game_compilations':0,'target_proposals':len(patches)}))


if __name__=='__main__':main()
