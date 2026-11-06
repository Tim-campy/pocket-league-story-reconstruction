"""Compile independent toy fixtures with installed smcs to challenge the model."""
import json
from pathlib import Path
import subprocess
import os
from round6_slot_model import SlotModelClient
from round5_local_cases import local_inspector
from compare_kairo import sha

ROOT=Path(__file__).resolve().parents[1]
SOURCE=r'''using System; using System.Collections; using System.Collections.Generic;
public struct Token : IDisposable { public void Dispose(){} public int Value(){return 1;} }
public static class Fixture {
 static int field; static int[] array={1,2}; static object gate=new object();
 static Token Get(){return new Token();}
 static void Sink(object a,int b,double c){}
 public static void Ordered(){object a=new object();int b=1;double c=2;Sink(a,b,c);}
 public static void Grouped(){object a;int b;double c;a=new object();c=2;b=1;Sink(a,b,c);}
 public static void Nested(){object a=new object();{double c=2;Console.WriteLine(c);}int b=1;Sink(a,b,0);}
 public static int Array(){int n=0;foreach(int value in array)n+=value;return n;}
 public static int Matrix(int[,] values){int n=0;foreach(int value in values)n+=value;return n;}
 public static int Collection(List<int> values){int n=0;foreach(int value in values)n+=value;return n;}
 public static void UsingExpression(){using(Get()){Console.WriteLine(1);}}
 public static void UsingDeclaration(){using(Token t=Get()){Console.WriteLine(1);}}
 public static void Lock(){lock(gate){Console.WriteLine(1);}}
 public static int Switch(int n){switch(n){case 1:return 2;case 3:return 4;default:return 5;}}
 public static int NestedSwitch(int n,int m){switch(n){case 1:switch(m){case 2:return 3;default:return 4;}default:return 5;}}
 public static int Protected(){try{return 1;}finally{Console.WriteLine(2);}}
 public static int UsingReturn(){using(Get()){return 1;}}
 public static int Increment(){return field++;}
 public static int Assignment(){return field=(field=7);}
 public static int Receiver(){return Get().Value();}
 public static int StaticParse(string x){return int.Parse(x);}
 public static int ReusedSwitches(int n){int sum=0;switch(n){case 1:sum++;break;}switch(n){case 2:sum++;break;}return sum;}
 public static int StringSwitch(string n){switch(n){case "a":return 1;case "b":return 2;default:return 0;}}
}'''


def main():
    base=ROOT/'build/round6/baseline/Assembly-CSharp'
    manifest=json.loads((base/'build-manifest.json').read_text(encoding='utf-8-sig'))
    folder=ROOT/'build/round6/slot-fixture';folder.mkdir(exist_ok=True)
    source=folder/'Fixture.cs';source.write_text(SOURCE,encoding='utf-8',newline='\n')
    binary=folder/'Fixture.dll'
    flags=[f for f in manifest['flags'] if not f.startswith('-out:') and f!='-noconfig']
    args=flags+['-out:'+str(binary)]+['-r:'+r['path'] for r in manifest['references']]+[str(source)]
    rsp=folder/'compile.rsp';rsp.write_text('\n'.join('"'+a+'"' for a in args)+'\n',encoding='utf-8')
    env=os.environ.copy();env.update(manifest['scoped_environment'])
    p=subprocess.run([manifest['runtime'],manifest['compiler'],'-noconfig','@'+str(rsp)],capture_output=True,env=env,cwd=ROOT)
    (folder/'compiler.log').write_bytes(p.stdout+p.stderr)
    if p.returncode:raise RuntimeError(p.stdout+p.stderr)
    raw=subprocess.run([str(local_inspector()),str(binary),'--locals-only'],capture_output=True,check=True)
    actual={m['stable_id']:m.get('plain_local_types',[]) for m in json.loads(raw.stdout.decode('utf-8-sig'))['methods']}
    isolated=dict(manifest,sources=[{'path':str(source),'input_path':str(source)}])
    mp=folder/'manifest.json';mp.write_text(json.dumps(isolated),encoding='utf-8')
    with SlotModelClient(mp) as client:models=client.predict_all(source,SOURCE)
    rows=[dict(m,actual_types=actual[m['method_id']],prediction_equal=m['predicted_types']==actual[m['method_id']]) for m in models]
    result={'operation':'independent-smcs-fixture-validation','game_candidates_compiled':0,'fixture_assembly_compilations':1,
            'compiler_sha256':manifest['compiler_sha256'],'source_sha256':sha(source),'methods':rows,
            'complete':sum(r['complete'] for r in rows),'complete_equal':sum(r['complete'] and r['prediction_equal'] for r in rows),
            'incomplete':sum(not r['complete'] for r in rows)}
    (ROOT/'reports/round6-slot-fixture.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8',newline='\n')
    print(json.dumps({k:v for k,v in result.items() if k!='methods'}))
    return result


if __name__=='__main__':main()
