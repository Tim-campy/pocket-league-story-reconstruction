"""Independent syntax and Roslyn 1.1 toy-emission controls. No game edits."""
import json
from pathlib import Path
import subprocess
from round7_transform import TransformClient

ROOT = Path(__file__).resolve().parents[1]
SOURCE = '''struct Color { public float r,g,b,a; }
class Fixture {
 static Color Fieldwise(float x) {Color c=default(Color); c.r=x;c.g=x;c.b=x;c.a=1;return c;}
 static Color Partial(float x) {Color c=default(Color);c.r=x;return c;}
 static Color Reads(float x) {Color c=default(Color);c.r=x;c.g=c.r;c.b=x;c.a=1;return c;}
 static float ArrayLoop(float[] values) {float total=0;for(int i=0;i<values.Length;i++){float value=values[i];total+=value;}return total;}
 static float IndexUsed(float[] values) {float total=0;for(int i=0;i<values.Length;i++){float value=values[i];total+=value+i;}return total;}
 static void Writes(float[] values) {for(int i=0;i<values.Length;i++){float value=values[i];values[i]=value+1;}}
}
'''


def main():
    folder=ROOT/'build/round7-roslyn-rule-fixtures'
    folder.mkdir(parents=True,exist_ok=True)
    source=folder/'Fixture.cs';source.write_text(SOURCE,encoding='utf-8')
    baseline=json.loads((ROOT/'build/round7/baseline/KairoLibrary/build-manifest.json').read_text())
    core=baseline['references'][0]['path']
    manifest=folder/'manifest.json'
    manifest.write_text(json.dumps({'sources':[str(source)],'references':[core]}))
    checks=[];selected={}
    with TransformClient(manifest) as client:
        for method,needle,rule,expected in [
            ('Fieldwise','Color c','struct-fieldwise-no-initializer',True),
            ('Partial','Color c','struct-fieldwise-no-initializer',False),
            ('Reads','Color c','struct-fieldwise-no-initializer',False),
            ('ArrayLoop','for(','array-index-loop-foreach',True),
            ('IndexUsed','for(','array-index-loop-foreach',False),
            ('Writes','for(','array-index-loop-foreach',False)]:
            point=SOURCE.index(needle,SOURCE.index(method+'('))
            result=client.request(source,source=SOURCE,offset=point,compiler='roslyn',max_candidates=2048)
            patches=[p for p in result['patches'] if p['rule']==rule]
            assert bool(patches)==expected,(method,patches)
            checks.append({'fixture':method,'rule':rule,'status':'PASS','candidate':expected})
            if patches:selected[method]=patches[0]
    def compile_dump(text,name):
        target=folder/name;target.mkdir(exist_ok=True)
        path=target/'Fixture.cs';path.write_text(text,encoding='utf-8')
        dll=target/'Fixture.dll'
        result=subprocess.run([baseline['compiler'],'/nologo','/noconfig','/nostdlib+','/target:library','/optimize+','/debug:pdbonly','/langversion:6','/r:'+core,'/out:'+str(dll),str(path)],capture_output=True)
        (target/'compiler.log').write_bytes(result.stdout+result.stderr)
        assert result.returncode==0,result.stdout
        result=subprocess.run([str(ROOT/'build/tools/ProbeInspector.exe'),str(dll),'--all'],capture_output=True,check=True)
        dump=json.loads(result.stdout.decode('utf-8-sig'))
        return {m['key']:m for m in dump['selected_methods']}
    before=compile_dump(SOURCE,'baseline')
    afters={}
    for method,patch in selected.items():
        text=SOURCE[:patch['start']]+patch['after']+SOURCE[patch['end']:]
        after=compile_dump(text,method)
        key=next(k for k in after if '::'+method+'(' in k)
        old=[i['opcode'] for i in before[key]['instructions']]
        new=[i['opcode'] for i in after[key]['instructions']]
        if method=='Fieldwise':assert 'initobj' in old and 'initobj' not in new
        else:assert new.count('stloc.1')>=1 and len(after[key]['locals'])>len(before[key]['locals'])
        afters[method]={'original_fixture':before[key],'transformed_fixture':after[key]}
    (ROOT/'reports/round7-roslyn-rules-fixtures.json').write_text(json.dumps({'checks':checks,'emission':afters},indent=2)+'\n',encoding='utf-8')
    print(json.dumps({'syntax_controls':len(checks),'historical_compiles':1+len(selected),'passed':True}))


if __name__=='__main__':main()
