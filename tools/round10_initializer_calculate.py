"""Validate static-initializer mapping and publish one source-only .cctor patch."""
import argparse,json
from pathlib import Path
from compare_kairo import differences,sha
from round6_alignment import align_methods
from round9_part_b_fixtures import compile_smcs
from round10_transform import TransformClient
from round10_initializer_mapping import bind_initializer
from round10_engine import apply_patch

ROOT=Path(__file__).resolve().parents[1]
RULE='static-initializer-ternary-polarity'


def main():
    parser=argparse.ArgumentParser();parser.add_argument('--label',default='isolated-manifest');args=parser.parse_args()
    assert args.label and all(c.isalnum() or c in '-_' for c in args.label)
    work=ROOT/'build/round10/initializer-fixture'/args.label;assert not work.exists()
    work.mkdir(parents=True)
    base=ROOT/'build/round10/baseline/Assembly-CSharp'
    manifest=json.loads((base/'build-manifest.json').read_text(encoding='utf-8-sig'))
    before='public class InitializerFixture { public static int Flag=1; public static string Query=(Flag!=0) ? "right" : "left"; }'
    control='public class InitializerFixture { public static int Flag=1; public static string Query=(Flag==0) ? "left" : "right"; }'
    path=work/'Source.cs';path.write_text(before,encoding='utf-8')
    mp=work/'manifest.json';mp.write_text(json.dumps(dict(manifest,sources=[{'path':str(path)}])),encoding='utf-8')
    key='InitializerFixture::.cctor()->System.Void'
    a=compile_smcs(work/'before',before,manifest)[key]
    b=compile_smcs(work/'control',control,manifest)[key]
    assert differences(a,b)[0]
    with TransformClient(mp) as client:
        bound=bind_initializer(client,{'Source.cs':path},b,a,align_methods(b,a))
        assert bound and bound['field']['field_id']=='InitializerFixture::Query'
        response=client.request(path=path,method_id=key,line=bound['point']['start_line'],
                                column=bound['point']['start_column'])
        assert len(response['patches'])==1
        after=apply_patch(before,response['patches'][0])
    c=compile_smcs(work/'candidate',after,manifest)[key]
    assert not differences(b,c)[0],differences(b,c)[0]
    paths={s['path']:base/s['path'] for s in manifest['sources']}
    originals={m['stable_id']:m for m in json.loads((base/'original-methods.json').read_text(encoding='utf-8-sig'))['selected_methods']}
    rebuilt={m['stable_id']:m for m in json.loads((base/'rebuilt-methods.json').read_text(encoding='utf-8-sig'))['selected_methods']}
    key='main.AppData::.cctor()->System.Void'
    isolated=dict(manifest,sources=[{'path':str(p.resolve())} for p in paths.values()])
    target_manifest=work/'target-source-manifest.json'
    target_manifest.write_text(json.dumps(isolated),encoding='utf-8')
    with TransformClient(target_manifest) as client:
        binding=bind_initializer(client,paths,originals[key],rebuilt[key],align_methods(originals[key],rebuilt[key]))
        assert binding and binding['field']['field_id']=='main.AppData::CGI_COMMON_QUERY'
        logical=binding['logical_path'];text=paths[logical].read_bytes().decode('utf-8-sig')
        response=client.request(path=paths[logical],source=text,method_id=key,
                                line=binding['point']['start_line'],column=binding['point']['start_column'])
        assert len(response['patches'])==1
        patch=dict(response['patches'][0],method_id=key,logical_path=logical)
        assert patch['rule']==RULE and 'Config.APPLI_TYPE == 0' in patch['after']
    dependencies={str(p.relative_to(ROOT)).replace('\\','/'):sha(p) for p in
       [Path(__file__),ROOT/'tools/Round10FieldInitializerRules.cs',ROOT/'tools/round10_initializer_mapping.py',
        ROOT/'tools/Round10Transform.cs',ROOT/'tools/prepare_round10_transform.py',ROOT/'tools/round10_transform.py']}
    fixture={'operation':'round10-static-initializer-smcs-fixture','toy_compilations':3,'game_compilations':0,
       'candidate_full_body_matches_independent_control':True,'before_differs_from_control':True,
       'compiler_sha256':manifest['compiler_sha256'],'before':before,'control':control,'candidate':after,
       'mapping':bound,'game_mapping':binding,'dependencies':dependencies,
       'game_body_status':'UNKNOWN_UNTIL_VERIFIER','explicit_constructor_inserted':False}
    fp=ROOT/f'reports/round10-initializer-{args.label}-fixture.json';assert not fp.exists()
    fp.write_text(json.dumps(fixture,indent=2)+'\n',encoding='utf-8')
    bp=ROOT/f'reports/round10-initializer-{args.label}-patches.json';assert not bp.exists()
    bp.write_text(json.dumps({'calculator_sha256':sha(Path(__file__)),'dependency_sha256':dependencies,
       'evidence_sha256':{fp.relative_to(ROOT).as_posix():sha(fp)},'patches':[patch]},indent=2)+'\n',encoding='utf-8')
    rp=ROOT/f'reports/round10-initializer-{args.label}-ready.json';assert not rp.exists()
    rp.write_text(json.dumps({'state':'READY','meaning':'Source mapping and own fixture verified; game body UNKNOWN',
       'files_sha256':{**dependencies,fp.relative_to(ROOT).as_posix():sha(fp),
                       bp.relative_to(ROOT).as_posix():sha(bp)}},indent=2)+'\n',encoding='utf-8')
    pending=[m['stable_id'] for m in originals.values() if m['stable_id'].endswith('::.cctor()->System.Void')
             and differences(m,rebuilt.get(m['stable_id']))[0]]
    print(json.dumps({'toy_compilations':3,'positive_full_bodies':1,'game_compilations':0,
                     'field':binding['field']['field_id'],'pending_cctors':len(pending),'pending_cctor_ids':pending}))


if __name__=='__main__':main()
