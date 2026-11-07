"""Validate the prepared training composition on an independent smcs fixture.

Then publish a hash-bound C# proposal. The game body remains UNKNOWN until the
engine compiles it and the official verifier adopts it.
"""
import json
from pathlib import Path
from compare_kairo import differences,sha
from round9_part_b_fixtures import compile_smcs
import round10_training_source as api

ROOT=Path(__file__).resolve().parents[1]
RULE='d10-exact-setwin-chara-training'


def main():
    work=ROOT/'build/round10/training-fixture'
    assert not work.exists(),'Preserve evidence; use a new driver label for a new attempt'
    templates=ROOT/'tools/fixtures/round10-training'
    before=(templates/'Current.cs').read_text(encoding='utf-8-sig')
    control=(templates/'Control.cs').read_text(encoding='utf-8-sig')
    a,b,method=api.extract(before,'public void Run(int time, int tMode)')
    after=before[:a]+api.transform_training(method)+before[b:]
    manifest=json.loads((ROOT/'build/round10/baseline/Assembly-CSharp/build-manifest.json').read_text(encoding='utf-8-sig'))
    key='Fixture::Run(System.Int32,System.Int32)->System.Void'
    left,want,actual=[compile_smcs(work/label,text,manifest)[key]
                      for label,text in [('before',before),('control',control),('candidate',after)]]
    assert differences(left,want)[0]
    assert not differences(want,actual)[0],differences(want,actual)[0]
    negative=[]
    for label,text in [
      ('other-case',method.replace('case 3:','case 4:')),
      ('default',method.replace('case 3:','default:')),
      ('other-transfer',method.replace('num5 = time % 9;','return; num5 = time % 9;')),
    ]:
        try:api.transform_training(text)
        except ValueError: negative.append(label)
        else:raise AssertionError(label)
    logical='src/Assembly-CSharp/form/SubForm.cs'
    source=(ROOT/'build/round10/baseline/Assembly-CSharp'/logical).read_bytes().decode('utf-8-sig')
    start,end,target=api.extract(source,'public void SetWinCharaTraining(int time, int tMode)')
    replacement,steps=api._training(target)
    assert api.inventory(target)['lexical_call_like_names']==api.inventory(replacement)['lexical_call_like_names']
    assert api.inventory(target)['literal_tokens']==api.inventory(replacement)['literal_tokens']
    dependencies={str(p.relative_to(ROOT)).replace('\\','/'):sha(p) for p in
       [Path(__file__),ROOT/'tools/round10_training_source.py',templates/'Current.cs',templates/'Control.cs']}
    fixture={'operation':'round10-training-independent-smcs-fixture','compiler_sha256':manifest['compiler_sha256'],
       'toy_compilations':3,'game_compilations':0,'positive_full_bodies':1,
       'candidate_matches_manual_control':True,'current_differs_from_control':True,
       'negative_controls':negative,'dependencies':dependencies,'original_slot_count':len(left['locals']),
       'candidate_slot_count':len(actual['locals']),'game_body_status':'UNKNOWN_UNTIL_VERIFIER'}
    fp=ROOT/'reports/round10-training-fixture.json';assert not fp.exists()
    fp.write_text(json.dumps(fixture,indent=2)+'\n',encoding='utf-8')
    proposal={'rule':RULE,'method_id':'form.SubForm::SetWinCharaTraining(System.Int32,System.Int32)->System.Void',
       'logical_path':logical,'start':len(source[:start].encode('utf-16-le'))//2,
       'end':len(source[:end].encode('utf-16-le'))//2,'before':target,'after':replacement,
       'component_steps':steps,'baseline_source_sha256':sha(ROOT/'build/round10/baseline/Assembly-CSharp'/logical)}
    bundle={'calculator_sha256':sha(Path(__file__)),'dependency_sha256':dependencies,
       'evidence_sha256':{'reports/round10-training-fixture.json':sha(fp)},'patches':[proposal]}
    bp=ROOT/'reports/round10-training-patches.json';assert not bp.exists()
    bp.write_text(json.dumps(bundle,indent=2)+'\n',encoding='utf-8')
    ready={'state':'READY','meaning':'Fixture validated; proposal only, target body UNKNOWN',
       'files_sha256':{**dependencies,'reports/round10-training-fixture.json':sha(fp),
                       'reports/round10-training-patches.json':sha(bp)}}
    rp=ROOT/'reports/round10-training-ready.json';assert not rp.exists()
    rp.write_text(json.dumps(ready,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({'toy_compilations':3,'positive_full_bodies':1,'negative_controls':len(negative),
                     'target_proposals':1,'game_compilations':0,'ready':str(rp.relative_to(ROOT))}))


if __name__=='__main__':main()
