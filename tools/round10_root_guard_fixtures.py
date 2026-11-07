"""Independent historical-compiler controls for root void guards and constructors."""
import argparse,json
from pathlib import Path
from compare_kairo import differences,sha
from round9_part_b_fixtures import compile_smcs
from round7_roslyn_for_declaration_fixture import compile_fixture as compile_roslyn
from round10_transform import TransformClient
from round10_engine import apply_patch

ROOT=Path(__file__).resolve().parents[1]
RULE='root-guard-positive-tail'
CASES=[
 ('smcs-constructor','Assembly-CSharp','GuardFixture::.ctor(System.Exception)->System.Void',
  'using System; public class GuardFixture { int count; public GuardFixture(Exception th) { count=0; if(th==null) { return; } count=th.Message.Length; } }',
  'using System; public class GuardFixture { int count; public GuardFixture(Exception th) { count=0; if(th!=null) { count=th.Message.Length; } } }',RULE),
 ('roslyn-prefix-and-positive','KairoLibrary','GuardFixture::Run(System.Boolean,System.Int32,System.Int32)->System.Void',
  'public class GuardFixture { public int count; public void Run(bool invalid,int touches,int frames) { if(invalid || touches<=0 || frames<=10) { return; } count=touches+frames; } }',
  'public class GuardFixture { public int count; public void Run(bool invalid,int touches,int frames) { if(invalid) { return; } if(touches>0 && frames>10) { count=touches+frames; } } }',RULE+'-split-prefix-1'),
 ('smcs-float-negation','Assembly-CSharp','GuardFixture::Run(System.Single)->System.Void',
  'public class GuardFixture { public int count; public void Run(float value) { if(value<0f) { return; } count=7; } }',
  'public class GuardFixture { public int count; public void Run(float value) { if(!(value<0f)) { count=7; } } }',RULE),
]


def query(folder,source,manifest,key,selected=RULE):
    folder.mkdir(parents=True,exist_ok=True)
    path=folder/'Fixture.cs';path.write_text(source,encoding='utf-8')
    mp=folder/'manifest.json';mp.write_text(json.dumps(dict(manifest,sources=[{'path':str(path)}])),encoding='utf-8')
    offset=len(source[:source.index('if(')].encode('utf-16-le'))//2
    with TransformClient(mp) as client:
        return client.request(path=path,source=source,offset=offset,method_id=key,
            compiler='smcs' if Path(manifest['compiler']).name.lower()=='smcs.exe' else 'roslyn',round7_cycle_rule=selected)


def main():
    p=argparse.ArgumentParser();p.add_argument('--label',default='initial');args=p.parse_args()
    assert args.label and all(c.isalnum() or c in '-_' for c in args.label)
    work=ROOT/'build/round10/root-guard-fixtures'/args.label
    assert not work.exists(),'Use a fresh label'
    rows=[]
    for label,assembly,key,before,control,chosen in CASES:
        manifest=json.loads((ROOT/f'build/round10/baseline/{assembly}/build-manifest.json').read_text(encoding='utf-8-sig'))
        result=query(work/label/'syntax',before,manifest,key)
        patches=[patch for patch in result['patches'] if patch['rule']==chosen]
        assert len(patches)==1,(label,[v['rule'] for v in result.get('patches',[])])
        candidate=apply_patch(before,patches[0])
        if assembly=='KairoLibrary':
            def compile_one(folder,source): return compile_roslyn(folder,source,manifest)[0]
        else:
            def compile_one(folder,source): return compile_smcs(folder,source,manifest)
        a,b,c=[compile_one(work/label/part,source)[key] for part,source in
               [('before',before),('control',control),('candidate',candidate)]]
        assert differences(a,b)[0],label
        assert not differences(b,c)[0],(label,differences(b,c)[0])
        if label=='smcs-float-negation': assert '!(value<0f)' in candidate.replace(' ','')
        rows.append({'case':label,'compiler_sha256':manifest['compiler_sha256'],'rule':chosen,
                     'before_body_different':True,'candidate_full_body_matches_independent_control':True,
                     'before':before,'control':control,'candidate':candidate})
    manifest=json.loads((ROOT/'build/round10/baseline/Assembly-CSharp/build-manifest.json').read_text(encoding='utf-8-sig'))
    negatives=[]
    for label,source,key,selected in [
      ('opt-in','class GuardFixture { void Run(bool flag) { if(flag) {return;} Consume(); } void Consume(){} }','GuardFixture::Run(System.Boolean)->System.Void',''),
      ('nonvoid','class GuardFixture { int Run(bool flag) { if(flag) {return 1;} return 2; } }','GuardFixture::Run(System.Boolean)->System.Int32',RULE),
      ('nonempty-guard','class GuardFixture { int count; void Run(bool flag) { if(flag) {count++;return;} count=7; } }','GuardFixture::Run(System.Boolean)->System.Void',RULE),
    ]:
        result=query(work/'negative'/label,source,manifest,key,selected)
        assert not [v for v in result.get('patches',[]) if v['rule'].startswith(RULE)],label
        negatives.append(label)
    report={'operation':'round10-root-guard-independent-historical-fixtures','game_compilations':0,
       'toy_compilations':9,'positive_full_bodies':len(rows),'negative_controls':negatives,
       'rules_sha256':sha(ROOT/'tools/Round10RootGuardRules.cs'),'dispatcher_sha256':sha(ROOT/'tools/Round10Transform.cs'),
       'driver_sha256':sha(Path(__file__)),'cases':rows,'target_body_verification_required':True}
    output=ROOT/f'reports/round10-root-guard-fixtures-{args.label}.json';assert not output.exists()
    output.write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({'report':str(output.relative_to(ROOT)),'positive_full_bodies':len(rows),'toy_compilations':9,'negative_controls':len(negatives)}))


if __name__=='__main__':main()
