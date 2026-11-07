"""Test the prepared nine-edit control/scope composition with historical smcs."""
import argparse,json
from pathlib import Path
from compare_kairo import differences,sha
from round9_part_b_fixtures import compile_smcs
from round10_training_source import extract
import round10_draw4_source as api

ROOT=Path(__file__).resolve().parents[1]
RULE='d10-subform-draw4-composition'


def main():
    parser=argparse.ArgumentParser();parser.add_argument('--label',default='receipt-schema-fix');args=parser.parse_args()
    assert args.label and all(c.isalnum() or c in '-_' for c in args.label)
    work=ROOT/'build/round10/draw4-fixture'/args.label;assert not work.exists()
    templates=ROOT/'tools/fixtures/round10-draw4'
    current=templates/'FixtureCurrent.cs';control=templates/'FixtureControl.cs'
    before=current.read_text(encoding='utf-8-sig');want=control.read_text(encoding='utf-8-sig')
    start,end,method=extract(before,'public void Run(Graphics g)')
    candidate=before[:start]+api.transform_draw4(method)+before[end:]
    base=ROOT/'build/round10/baseline/Assembly-CSharp'
    manifest=json.loads((base/'build-manifest.json').read_text(encoding='utf-8-sig'))
    key='Draw4Fixture::Run(Graphics)->System.Void'
    a,b,c=[compile_smcs(work/tag,text,manifest)[key]
           for tag,text in [('before',before),('control',want),('candidate',candidate)]]
    assert differences(a,b)[0]
    assert not differences(b,c)[0],differences(b,c)[0]
    logical='src/Assembly-CSharp/form/SubForm.cs'
    source=(base/logical).read_bytes().decode('utf-8-sig')
    start,end,target=extract(source,'private void _draw4(Graphics g)')
    calculated=api.calculate_draw4(target)
    dependencies={p.relative_to(ROOT).as_posix():sha(p) for p in
       [Path(__file__),ROOT/'tools/round10_draw4_source.py',ROOT/'tools/round10_training_source.py',current,control]}
    fixture={'operation':'round10-draw4-own-smcs-fixture','compiler_sha256':manifest['compiler_sha256'],
       'toy_compilations':3,'game_compilations':0,'positive_full_bodies':1,
       'candidate_matches_manual_control':True,'before_body_different':True,
       'before_locals':len(a['locals']),'control_locals':len(b['locals']),
       'before_maxstack':a['header']['maxstack'],'control_maxstack':b['header']['maxstack'],
       'dependencies':dependencies,'target_body_status':'UNKNOWN_UNTIL_VERIFIER'}
    fp=ROOT/'reports/round10-draw4-fixture.json';assert not fp.exists()
    fp.write_text(json.dumps(fixture,indent=2)+'\n',encoding='utf-8')
    patch={'rule':RULE,'method_id':'form.SubForm::_draw4(kairo.unity.ui.Graphics)->System.Void',
       'logical_path':logical,'start':len(source[:start].encode('utf-16-le'))//2,
       'end':len(source[:end].encode('utf-16-le'))//2,'before':target,'after':calculated['after'],
       'component_steps':calculated['changes'],'game_body_status':'UNKNOWN_UNTIL_VERIFIER'}
    bp=ROOT/'reports/round10-draw4-patches.json';assert not bp.exists()
    bp.write_text(json.dumps({'calculator_sha256':sha(Path(__file__)),'dependency_sha256':dependencies,
      'evidence_sha256':{fp.relative_to(ROOT).as_posix():sha(fp)},'patches':[patch]},indent=2)+'\n',encoding='utf-8')
    rp=ROOT/'reports/round10-draw4-ready.json';assert not rp.exists()
    rp.write_text(json.dumps({'state':'READY','meaning':'Own fixture passed; model and target body still unverified',
       'files_sha256':{**dependencies,fp.relative_to(ROOT).as_posix():sha(fp),bp.relative_to(ROOT).as_posix():sha(bp)}},indent=2)+'\n',encoding='utf-8')
    print(json.dumps({'toy_compilations':3,'positive_full_bodies':1,'source_edits':len(calculated['changes']),
                     'game_compilations':0,'target_proposals':1}))


if __name__=='__main__':main()
