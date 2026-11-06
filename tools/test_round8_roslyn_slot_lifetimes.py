"""Independent csc 1.1 fixtures for declaration identity and lexical scope."""
import json,sys
from pathlib import Path
root=next(p for p in Path(__file__).resolve().parents if (p/'tools/compiler_probes.py').is_file())
sys.path.insert(0,str(root/'tools'))
from round7_roslyn_for_declaration_fixture import compile_fixture
from compare_kairo import differences,sha
from round8_roslyn_model import SlotModelClient

work=root/'build/round8/roslyn-slot-lifetimes-fixtures'
PRELUDE='''public static class Fixture {
 static void Obj(ref object value){} static void Flt(ref float value){}
 static void Dbl(ref double value){} static void Bit(ref bool value){}
 static void Number(int value){}
'''
RECOVERED={
'Shared':'''public static int Shared(int[] values){int sum=0;for(int i=0;i<values.Length;i++){int scratch=values[i];sum=sum+scratch;}for(int j=0;j<values.Length;j++){int scratch=values[j]+1;sum=sum+scratch;}return sum;}''',
'Counter':'''public static void Counter(int[] values){if(values.Length>0){object mesh=new object();Obj(ref mesh);float scalar=1;Flt(ref scalar);}int pass=0;while(pass<values.Length){Number(pass);pass++;}}''',
'Late':'''public static void Late(int[] values){object owner=new object();Obj(ref owner);for(int i=0;i<values.Length;i++){Number(i);}double value=1;Dbl(ref value);bool ready=true;Bit(ref ready);}''',
}
CONTROLS={
'Shared':'''public static int Shared(int[] values){int sum=0;int scratch;for(int i=0;i<values.Length;i++){scratch=values[i];sum=sum+scratch;}for(int j=0;j<values.Length;j++){scratch=values[j]+1;sum=sum+scratch;}return sum;}''',
'Counter':'''public static void Counter(int[] values){if(values.Length>0){object mesh=new object();Obj(ref mesh);float scalar=1;Flt(ref scalar);}for(int pass=0;pass<values.Length;){Number(pass);pass++;}}''',
'Late':'''public static void Late(int[] values){object owner=new object();Obj(ref owner);for(int i=0;i<values.Length;i++){Number(i);}{double value=1;Dbl(ref value);bool ready=true;Bit(ref ready);}}''',
}

def main():
    manifest=json.loads((root/'build/round8/cycles/cycle03/after/KairoLibrary/build-manifest.json').read_text(encoding='utf-8-sig'))
    candidate=dict(RECOVERED)
    candidate['Shared']=candidate['Shared'].replace('int sum=0;','int sum=0;int scratch;').replace('int scratch=','scratch=')
    candidate['Counter']=candidate['Counter'].replace('int pass=0;while(pass<values.Length)','for(int pass=0;pass<values.Length;)')
    candidate['Late']=candidate['Late'].replace('double value=1;','{double value=1;')[:-1]+'}}'
    sources={name:PRELUDE+'\n'.join(methods.values())+'\n}' for name,methods in [('recovered',RECOVERED),('control',CONTROLS),('candidate',candidate)]}
    dumps={};types={}
    for name,source in sources.items():dumps[name],types[name]=compile_fixture(work/'fixtures'/name,source,manifest)
    path=work/'fixtures/candidate/Fixture.cs';mp=work/'fixtures/manifest.json'
    isolated=dict(manifest,sources=[dict(path=str(path),input_path=str(path))]);mp.write_text(json.dumps(isolated,indent=2)+'\n')
    rows=[]
    with SlotModelClient(mp) as model:
        for name in RECOVERED:
            identity=next(k for k in dumps['candidate'] if k.startswith('Fixture::'+name+'('))
            actual=dumps['candidate'][identity];control=dumps['control'][identity]
            delta,_=differences(control,actual);before_delta,_=differences(control,dumps['recovered'][identity])
            prediction=model.predict(path,sources['candidate'],identity)
            row=dict(method_id=identity,full_candidate_vs_control_differences=delta,
                full_recovered_vs_control_differences=before_delta,
                before_types=types['recovered'][identity],expected_types=types['control'][identity],actual_types=types['candidate'][identity],prediction=prediction,
                candidate_body=actual,control_body=control,recovered_body=dumps['recovered'][identity])
            assert not delta and before_delta,(name,delta,before_delta)
            assert prediction['complete'] and prediction['predicted_types']==row['expected_types'],row
            rows.append(row)
    result=dict(operation='independent-Roslyn-source-identity-and-scope-fixtures',compiler_sha256=manifest['compiler_sha256'],
        model_sha256=sha(root/'tools/Round8RoslynModel.cs'),client_sha256=sha(root/'tools/round8_roslyn_model.py'),
        fixture_assembly_compilations=3,game_compilations=0,fixtures=rows,
        exact_full_methods_against_independent_controls=sum(not r['full_candidate_vs_control_differences'] for r in rows))
    (root/'reports/round8-roslyn-slot-lifetime-fixtures.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8')
    print([(r['method_id'],r['before_types'],r['actual_types'],not r['full_candidate_vs_control_differences']) for r in rows])

if __name__=='__main__':main()
