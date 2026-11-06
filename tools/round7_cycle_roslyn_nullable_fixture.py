"""Historical-compiler controls for nullable conditional access plus coalesce."""
import json
from pathlib import Path
import subprocess

from compare_kairo import differences, sha
from round7_cycle_roslyn_fixture import ROOT, helper, compile_fixture

SOURCE='''using System;
public class Box {public int Get(){return 3;}public int? Maybe(){return null;}public string Text(){return null;}}
public static class Fixture {
 static long field;static void Sink(long x){field+=x;}static Box GetBox(){return new Box();}static int Fallback(){return 7;}
 public static long ArrayLength(Array array){long n=array?.GetLength(0)??(-1);Sink(n);return n;}
 public static int MethodCall(Box box){int n=box?.Get()??7;Sink(n);return n;}
 public static int ArrayIndex(int[] array){int n=array?[0]??(-1);Sink(n);return n;}
 public static int NullableReturn(Box box){return box?.Maybe()??7;}
 public static int ImpureReceiver(){return GetBox()?.Get()??7;}
 public static int ImpureFallback(Box box){return box?.Get()??Fallback();}
 public static string ReferenceReturn(Box box){return box?.Text()??"";}
}'''
CONTROLS={
 'ArrayLength':('array?.GetLength(0)??(-1)','((array!=null)?array.GetLength(0):(-1))'),
 'MethodCall':('box?.Get()??7','((box!=null)?box.Get():7)'),
 'ArrayIndex':('array?[0]??(-1)','((array!=null)?array[0]:(-1))'),
}


def main():
    folder=ROOT/'build/round7/cycle-roslyn-nullable-fixture';folder.mkdir(parents=True,exist_ok=True)
    manifest=json.loads((ROOT/'build/round7/baseline/KairoLibrary/build-manifest.json').read_text(encoding='utf-8-sig'))
    path=folder/'Input.cs';path.write_text(SOURCE,encoding='utf-8',newline='\n')
    command=helper(folder)+[str(path),manifest['references'][0]['path']]
    rows=json.loads(subprocess.run(command+['nullable-conditional-coalesce-explicit'],capture_output=True,check=True).stdout)
    nohint=json.loads(subprocess.run(command+['nohint'],capture_output=True,check=True).stdout)
    assert not any(row['patches'] for row in nohint)
    unsupported=['NullableReturn','ImpureReceiver','ImpureFallback','ReferenceReturn']
    assert not any(row['patches'] for row in rows if row['method'] in unsupported)
    selected=[row['patches'][0] for row in rows if row['method'] in CONTROLS]
    assert len(selected)==3
    candidate=SOURCE
    for patch in sorted(selected,key=lambda p:p['start'],reverse=True):
        assert candidate[patch['start']:patch['end']]==patch['before']
        candidate=candidate[:patch['start']]+patch['after']+candidate[patch['end']:]
    control=SOURCE
    for before,after in CONTROLS.values():control=control.replace(before,after)
    recovered=compile_fixture(folder/'recovered',SOURCE,manifest)
    expected=compile_fixture(folder/'control',control,manifest)
    actual=compile_fixture(folder/'candidate',candidate,manifest)
    results=[]
    for name in CONTROLS:
        identity=next(k for k in actual if k.startswith('Fixture::'+name+'('))
        delta,_=differences(expected[identity],actual[identity])
        results.append({'method':name,'body_exact_against_explicit_ternary_control':not delta,'differences':delta,
                        'recovered_instructions':recovered[identity]['instructions'],'candidate_instructions':actual[identity]['instructions'],
                        'control_instructions':expected[identity]['instructions']})
    report={'operation':'independent-nullable-conditional-coalesce-rule-fixture','game_candidates_compiled':0,'toy_assembly_compilations':3,
            'rule_sha256':sha(ROOT/'tools/Round7RoslynCycleRules.cs'),'compiler_sha256':manifest['compiler_sha256'],
            'hint_required':True,'rejected_shapes':unsupported,'fixtures':results}
    (ROOT/'reports/round7-roslyn-cycle-nullable-fixture.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8',newline='\n')
    print(json.dumps({k:v for k,v in report.items() if k!='fixtures'}))
    print([(r['method'],r['body_exact_against_explicit_ternary_control']) for r in results])
    assert all(r['body_exact_against_explicit_ternary_control'] for r in results)


if __name__=='__main__':main()
