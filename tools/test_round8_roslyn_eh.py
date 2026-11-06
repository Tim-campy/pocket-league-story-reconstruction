"""Independent historical Roslyn fixtures. This script never compiles game code."""
import json
import sys
from pathlib import Path

ROOT=Path(__file__).resolve().parents[1]
WORK=ROOT/'build/round8/roslyn-eh-fixture'
sys.path.insert(0,str(ROOT/'tools'))
sys.path.insert(0,str(WORK))
from compare_kairo import differences,sha
from round7_roslyn_for_declaration_fixture import compile_fixture
from test_round7_roslyn_model import SOURCE as REGRESSION_SOURCE
from round8_roslyn_model import SlotModelClient

EH_SOURCE='''
public static class EhFixture {
 static object field;
 static int Next(){return 3;}
 static object Make(){return new object();}
 static void Sink(object value) {field=value;}
 static void Number(int value){}
 static void Mutate(ref int value){value++;}
 public static void NoLocals(){try{Number(3);}finally{Number(4);}}
 public static void EarlyVoid(bool stop){try{if(stop)return;Number(3);}finally{Number(4);}}
 public static void NamedFinally(){object value=Make();try{Number(3);}finally{Sink(value);}}
 public static void NamedRepeated(){object value=Make();try{Sink(value);}finally{Sink(value);}}
 public static void StructFinally(){int value=Next();try{Number(3);}finally{Mutate(ref value);}}
 public static void NestedFinally(){object value=Make();try{try{Number(3);}finally{Sink(value);}}finally{Number(4);}}
 public static void ForeachOnlyFinally(object[] items){foreach(object value in items){try{Number(3);}finally{Sink(value);}}}
 public static void ForeachTryAndFinally(object[] items){foreach(object value in items){try{Sink(value);}finally{Sink(value);}}}
 public static void LoopExit(object[] items){for(int i=0;i<items.Length;i++){try{if(i==2)break;if(i==1)continue;Number(i);}finally{Number(4);}}}
 public static int ValueReturn(){try{return Next();}finally{Number(4);}}
 public static void Caught(){try{Number(3);}catch(System.Exception e){Sink(e);}finally{Number(4);}}
 public static void Filter(){try{Number(3);}catch(System.Exception e)when(e!=null){Sink(e);}}
 public static void WriteOnlyFinally(){object value;try{Number(3);}finally{value=Make();Sink(value);}}
}
'''
POSITIVES=['NoLocals','EarlyVoid','NamedFinally','NamedRepeated','StructFinally','NestedFinally','ForeachOnlyFinally','ForeachTryAndFinally','LoopExit']
NEGATIVES=['ValueReturn','Caught','Filter','WriteOnlyFinally']


def main():
    manifest=json.loads((ROOT/'build/round8/baseline/KairoLibrary/build-manifest.json').read_text(encoding='utf-8-sig'))
    source=REGRESSION_SOURCE+EH_SOURCE
    directory=WORK/'fixtures'
    # Same independent fixture, distinct compilation invocations. All method
    # headers, EH clauses, locals and normalized instruction operands are compared.
    first,types=compile_fixture(directory/'first',source,manifest)
    second,types2=compile_fixture(directory/'second',source,manifest)
    path=directory/'first/Fixture.cs'
    isolated=dict(manifest,sources=[dict(path=str(path),input_path=str(path))])
    mp=directory/'manifest.json';mp.write_text(json.dumps(isolated,indent=2)+'\n',encoding='utf-8')
    with SlotModelClient(mp) as model:
        predictions=model.predict_all(path,source)
        for identity in first:
            if '/<GetEnumerator>d__' in identity and 'MoveNext' in identity:
                predictions.append(model.predict(path,source,identity))
    rows=[]
    for prediction in predictions:
        identity=prediction['method_id']
        delta,_=differences(first[identity],second[identity])
        rows.append(dict(prediction,actual_types=types[identity],
            prediction_equal=prediction['predicted_types']==types[identity],
            full_method_comparison=delta,first_body=first[identity],second_body=second[identity]))
    for name in POSITIVES:
        row=next(r for r in rows if r['method_id'].startswith('EhFixture::'+name+'('))
        assert row['complete'] and row['prediction_equal'],(name,row['unsupported_reasons'],row['predicted_types'],row['actual_types'])
        assert not row['full_method_comparison'],(name,row['full_method_comparison'])
    for name in NEGATIVES:
        row=next(r for r in rows if r['method_id'].startswith('EhFixture::'+name+'('))
        assert not row['complete'],name
    assert all(not r['complete'] or r['prediction_equal'] for r in rows)
    assert all(not r['full_method_comparison'] for r in rows)
    assert types==types2
    result=dict(operation='independent-historical-roslyn-void-finally-model-fixtures',
        model_sha256=sha(ROOT/'tools/Round8RoslynModel.cs'),client_sha256=sha(ROOT/'tools/round8_roslyn_model.py'),
        fixture_driver_sha256=sha(Path(__file__)),compiler_sha256=manifest['compiler_sha256'],
        flags=manifest['flags'],references=manifest['references'],
        fixture_assembly_compilations=2,game_compilations=0,
        positive_EH_fixtures=POSITIVES,negative_EH_fixtures=NEGATIVES,
        fixture_methods=len(rows),complete=sum(r['complete'] for r in rows),
        complete_exact=sum(r['complete'] and r['prediction_equal'] for r in rows),
        complete_wrong=sum(r['complete'] and not r['prediction_equal'] for r in rows),
        full_method_differences=sum(bool(r['full_method_comparison']) for r in rows),methods=rows)
    (ROOT/'reports/round8-roslyn-eh-fixtures.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({k:v for k,v in result.items() if k not in ['methods','flags','references']}))


if __name__=='__main__':main()
