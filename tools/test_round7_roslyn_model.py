"""Challenge the source-only Roslyn model with independent csc 1.1 fixtures."""
import json
from pathlib import Path
import subprocess

from compare_kairo import sha
from round5_local_cases import local_inspector
from round7_roslyn_model import SlotModelClient

ROOT = Path(__file__).resolve().parents[1]
SOURCE = '''using System; using System.Collections; using System.Collections.Generic;
public struct Pair { public int X; }
public static class Fixture {
 static object field; static int Next(){return 3;}
 static void Sink(object a,int b,double c){}
 public static void BlockOrder(int count){object a=new object();for(int i=0;i<count;i++)Sink(a,i,0);double b=2;Sink(a,0,b);}
 public static void HoistedIndex(int count){int i;object a=new object();for(i=0;i<count;i++)Sink(a,i,0);}
 public static void NamedNeverReused(){ {int a=Next();Sink(null,a,0);} {int b=Next();Sink(null,b,0);} }
 public static int[] ArrayInitializer(){int a=Next();int b=Next();return new int[]{a,b};}
 public static int StackReturn(){int a=Next();return a;}
 public static void NamedAddress(){int a=Next();Mutate(ref a);}
 static void Mutate(ref int a){a++;}
 public static object Assignment(){return field=new object();}
 public static void ArrayForeach(object[] array){foreach(object item in array)Sink(item,0,0);}
 public static void ArrayStruct(Pair[] array){foreach(Pair item in array)Sink(null,item.X,0);}
 public static void EnumeratorForeach(List<object> array){foreach(object item in array)Sink(item,0,0);}
 public static void ReusedEnumerators(List<object> array){foreach(object item in array)Sink(item,0,0);foreach(object item in array)Sink(item,0,0);}
 public static int ComplexStack(int a,int b){int x=a*a;int y=b*b;return x+y*y;}
 public static int Protected(){try{return Next();}finally{Console.WriteLine(1);}}
}
public class IteratorFixture<T> {
 public T[] Values;
 public IEnumerator GetEnumerator(){T[] array=Values;foreach(T item in array){yield return item;}}
}
'''


def main():
    baseline = ROOT / 'build/round7/baseline/KairoLibrary'
    manifest = json.loads((baseline / 'build-manifest.json').read_text(encoding='utf-8-sig'))
    folder = ROOT / 'build/round7/roslyn-fixture'
    folder.mkdir(parents=True, exist_ok=True)
    source = folder / 'Fixture.cs'
    binary = folder / 'Fixture.dll'
    source.write_text(SOURCE, encoding='utf-8', newline='\n')
    isolated = dict(manifest, sources=[{'path': str(source), 'input_path': str(source)}])
    path = folder / 'manifest.json'
    path.write_text(json.dumps(isolated), encoding='utf-8')
    # Only independent toy code is compiled here; no source from the game.
    flags = [f for f in manifest['flags'] if not f.startswith('/out:') and f != '/noconfig']
    args = flags + ['/out:' + str(binary)] + ['/r:' + r['path'] for r in manifest['references']] + [str(source)]
    rsp = folder / 'compile.rsp'
    rsp.write_text('\n'.join('"' + a + '"' for a in args) + '\n', encoding='utf-8')
    compiled = subprocess.run([manifest['compiler'], '/noconfig', '@' + str(rsp)], capture_output=True, cwd=ROOT)
    (folder / 'compiler.log').write_bytes(compiled.stdout + compiled.stderr)
    if compiled.returncode:
        raise RuntimeError(compiled.stdout + compiled.stderr)
    raw = subprocess.run([str(local_inspector()), str(binary), '--locals-only'], capture_output=True, check=True)
    actual = {m['stable_id']: m.get('plain_local_types', []) for m in json.loads(raw.stdout.decode('utf-8-sig'))['methods']}
    with SlotModelClient(path) as client:
        predictions = client.predict_all(source, SOURCE)
        for mid in actual:
            if '/<GetEnumerator>d__' in mid and 'MoveNext' in mid:
                predictions.append(client.predict(source, SOURCE, mid))
    rows = [dict(p, actual_types=actual[p['method_id']], prediction_equal=p['predicted_types'] == actual[p['method_id']]) for p in predictions]
    result = {'operation': 'independent-roslyn-1.1-fixture-validation', 'game_candidates_compiled': 0,
              'fixture_assembly_compilations': 1, 'compiler_sha256': manifest['compiler_sha256'],
              'source_sha256': sha(source), 'methods': rows,
              'complete': sum(r['complete'] for r in rows),
              'complete_equal': sum(r['complete'] and r['prediction_equal'] for r in rows),
              'incomplete': sum(not r['complete'] for r in rows)}
    (ROOT / 'reports/round7-roslyn-fixture.json').write_text(json.dumps(result, indent=2) + '\n', encoding='utf-8', newline='\n')
    print(json.dumps({k: v for k, v in result.items() if k != 'methods'}))
    assert result['complete'] == result['complete_equal'], 'Source model approved an incorrect fixture prediction'
    return result


if __name__ == '__main__':
    main()
