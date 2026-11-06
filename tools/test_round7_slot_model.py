"""Challenge the source-only predictor with an independent real smcs assembly."""
import json
import os
from pathlib import Path
import subprocess
from round6_slot_fixture import SOURCE as PREVIOUS_SOURCE
from round7_slot_model import SlotModelClient
from round5_local_cases import local_inspector
from compare_kairo import sha

ROOT = Path(__file__).resolve().parents[1]
SOURCE = PREVIOUS_SOURCE + r'''
public delegate void Callback();
public class InitToken {public int Value;}
public class CaptureFixture {
 static Callback keep; int field;
 public static void Parameter(int n){keep=delegate{Console.WriteLine(n);};}
 public static void CapturedLocal(){int n=2;string text="x";keep=delegate{Console.WriteLine(n);};Console.WriteLine(text);}
 public static void NestedScope(int n){int a=0;{int b=1;keep=delegate{Console.WriteLine(n+b);};Console.WriteLine(a);}}
 public void ThisOnly(){keep=delegate{Console.WriteLine(field);};}
 public static void Captureless(){keep=delegate{Console.WriteLine(1);};}
 public void HistoricalDelegate(){new java.lang.JThread(delegate{Console.WriteLine(field);}).Start();}
 public static void HistoricalCatch(){try{Console.WriteLine(1);}catch(kairo.unity.util.NotPermitException e){Console.WriteLine(e);}}
 public void GeneratedInitializer(){keep=delegate{string text="x";try{Console.WriteLine(new InitToken{Value=field});}catch(Exception e){Console.WriteLine(e);}Console.WriteLine(text);};}
 public void GeneratedExplicitLocal(){keep=delegate{string text="x";try{InitToken token=new InitToken();token.Value=field;Console.WriteLine(token);}catch(Exception e){Console.WriteLine(e);}Console.WriteLine(text);};}
}
public class HistoricalDerived:kairo.unity.form.FormBase {
 public HistoricalDerived():base(0){}
 public override string ToString(){return "x";}
 public override int GetHashCode(){return 3;}
}
'''


def main():
    base = ROOT / 'build/round7/baseline/Assembly-CSharp'
    manifest = json.loads((base / 'build-manifest.json').read_text(encoding='utf-8-sig'))
    folder = ROOT / 'build/round7/slot-fixture'
    folder.mkdir(parents=True, exist_ok=True)
    source, binary = folder / 'Fixture.cs', folder / 'Fixture.dll'
    source.write_text(SOURCE, encoding='utf-8', newline='\n')
    flags = [f for f in manifest['flags'] if not f.startswith('-out:') and f != '-noconfig']
    args = flags + ['-out:' + str(binary)] + ['-r:' + r['path'] for r in manifest['references']] + [str(source)]
    rsp = folder / 'compile.rsp'
    rsp.write_text('\n'.join('"' + a + '"' for a in args) + '\n', encoding='utf-8')
    env = os.environ.copy()
    env.update(manifest['scoped_environment'])
    p = subprocess.run([manifest['runtime'], manifest['compiler'], '-noconfig', '@' + str(rsp)], capture_output=True, env=env, cwd=ROOT)
    (folder / 'compiler.log').write_bytes(p.stdout + p.stderr)
    if p.returncode:
        raise RuntimeError(p.stdout + p.stderr)
    raw = subprocess.run([str(local_inspector()), str(binary), '--locals-only'], capture_output=True, check=True)
    actual = {m['stable_id']: m.get('plain_local_types', []) for m in json.loads(raw.stdout.decode('utf-8-sig'))['methods']}
    mp = folder / 'manifest.json'
    mp.write_text(json.dumps(dict(manifest, sources=[{'path': str(source), 'input_path': str(source)}])), encoding='utf-8')
    with SlotModelClient(mp) as client:
        models = client.predict_all(source, SOURCE)
        invalid = SOURCE + r'''
public class NotDelegate {}
public class InvalidFixture {
 static void Sink(NotDelegate value){}
 public void InvalidDelegate(){Sink(delegate{});}
 public override int MissingOverride(){return 1;}
 public void InvalidCatch(){try{}catch(NotDelegate value){}}
}
'''
        negative = [r for r in client.predict_all(source, invalid) if r['method_id'].startswith('InvalidFixture::Invalid') or '::MissingOverride(' in r['method_id']]
    rows = [dict(m, actual_types=actual[m['method_id']], prediction_equal=m['predicted_types'] == actual[m['method_id']]) for m in models]
    result = {'operation': 'independent-smcs-fixture-validation', 'game_candidates_compiled': 0,
              'fixture_assembly_compilations': 1, 'compiler_sha256': manifest['compiler_sha256'],
              'source_sha256': sha(source), 'model_sha256': sha(ROOT/'tools/Round7SlotModel.cs'),
              'complete': sum(r['complete'] for r in rows),
              'complete_equal': sum(r['complete'] and r['prediction_equal'] for r in rows),
              'incomplete': sum(not r['complete'] for r in rows), 'methods': rows,
              'semantic_negative_controls': [{'method_id': r['method_id'], 'complete': r['complete'], 'unsupported_reasons': r['unsupported_reasons']} for r in negative]}
    (ROOT/'reports/round7-slot-fixture.json').write_text(json.dumps(result, indent=2) + '\n', encoding='utf-8', newline='\n')
    print(json.dumps({k: v for k, v in result.items() if k != 'methods'}))
    assert result['complete_equal'] == result['complete'], [(r['method_id'], r['predicted_types'], r['actual_types']) for r in rows if r['complete'] and not r['prediction_equal']]
    assert len(negative) == 3 and all(not r['complete'] for r in negative), negative
    return result


if __name__ == '__main__':
    main()
