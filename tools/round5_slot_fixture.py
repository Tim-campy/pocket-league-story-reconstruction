"""Empirical variable-name/declaration/block-order tests with the installed smcs."""
import json
import os
from pathlib import Path
import subprocess

from apply_il_rules import dump
from compare_kairo import differences, sha
from compiler_probes import MONO, SMCS, UNITY

ROOT = Path(__file__).resolve().parents[1]


def source(names, order):
    obj, integer, real = names
    decl = {'O': 'object ' + obj + ' = Obj();', 'I': 'int ' + integer + ' = Int();', 'D': 'double ' + real + ' = Real();'}
    body = ' '.join(decl[k] for k in order) + ' return Sink(' + ','.join(names) + ');'
    return '''using System; using System.Collections;
public class SlotFixture {
 static object Obj(){return new object();} static int Int(){return 7;} static double Real(){return 3.25;}
 static int Sink(object o,int i,double d){return o.GetHashCode()+i+(int)d;}
 static int Run(Func<int> f){return f();} static int Run(Func<string> f){return f().Length;}
 public static int Ordinary(){BODY}
 public static int DeclaredGroup(){object OBJ;int INTEGER;double REAL;OBJ=Obj();{REAL=Real();}INTEGER=Int();return Sink(OBJ,INTEGER,REAL);}
 public static int UnusedSlot(){int originalUnusedSlot; int used=Int();return used;}
 public static int Nested(){object OBJ=Obj(); {double REAL=Real(); Sink(null,0,REAL);} int INTEGER=Int(); return Sink(OBJ,INTEGER,0);}
 public static int Lambda(){Func<int> f=delegate {BODY}; return f();}
 public static int Overloaded(){return Run(delegate {BODY});}
 sealed class R:IDisposable {public void Dispose(){}}
 public static int Temps(IEnumerable items,object gate){int count=0;
 foreach(object entry in items){short nested=3; Sink(entry,nested,0);count++;}
 using(R resource=new R()){byte nested=2;Sink(resource,nested,0);}
 lock(gate){float nested=1;Sink(gate,0,nested);}
 switch(count){case 0: double nested=Real();Sink(null,0,nested);break;default:break;}
 return count;}
}'''.replace('BODY', body).replace('OBJ', obj).replace('INTEGER', integer).replace('REAL', real)


def main():
    folder = ROOT / 'build/round5/slot-fixture'
    folder.mkdir(parents=True, exist_ok=True)
    env = os.environ.copy()
    env.update(MONO_PATH=str(UNITY / 'Editor/Data/Mono/lib/mono/unity'), MONO_CFG_DIR=str(UNITY / 'Editor/Data/Mono/etc'))
    sets = [('zebra', 'alpha', 'middle'), ('alpha', 'middle', 'zebra'), ('num', 'num2', 'num3'), ('aardvark', 'zzzz', 'recoveredValue')]
    records = []
    for n, names in enumerate(sets):
        for order in ('OID', 'DIO'):
            label = str(n) + '-' + order
            dest = folder / label
            dest.mkdir(exist_ok=True)
            text = source(names, order)
            src, binary = dest / 'Fixture.cs', dest / 'SlotFixture.dll'
            src.write_text(text, encoding='utf-8', newline='\n')
            cmd = [str(MONO), str(SMCS), '-noconfig', '-nologo', '-target:library', '-optimize+', '-debug-', '-unsafe-', '-checked-', '-out:' + str(binary),
                   '-r:' + str(ROOT / 'original/extracted/assets/bin/Data/Managed/System.Core.dll'), str(src)]
            p = subprocess.run(cmd, env=env, cwd=ROOT, capture_output=True)
            (dest / 'compiler.log').write_bytes(p.stdout + p.stderr)
            if p.returncode:
                raise RuntimeError((p.stdout + p.stderr).decode(errors='replace'))
            methods = dump(binary, dest / 'methods.json')
            records.append({'label': label, 'names': names, 'declaration_order': order, 'source': text,
                            'source_sha256': sha(src), 'binary_sha256': sha(binary), 'methods': methods})
    baseline = records[0]['methods']
    comparisons = []
    for r in records:
        changed = [key for key, method in baseline.items() if differences(method, r['methods'].get(key))[0]]
        comparisons.append({'label': r['label'], 'names': r['names'], 'declaration_order': r['declaration_order'],
                            'changed_methods': changed,
                            'locals': {key: value.get('locals', []) for key, value in r['methods'].items() if 'raw_body_hex' in value},
                            'instructions': {key: value.get('instructions', []) for key, value in r['methods'].items() if 'Ordinary' in key or 'm__' in key}})
    report = {'operation': 'installed-smcs-name-vs-declaration-order-probe', 'compiler': str(SMCS), 'compiler_sha256': sha(SMCS),
              'runtime': str(MONO), 'runtime_sha256': sha(MONO), 'game_sources_modified': False, 'ledger_states_modified': False,
              'variants': [{k: v for k, v in r.items() if k != 'methods'} for r in records], 'comparisons': comparisons}
    (ROOT / 'reports/round5-slot-fixture.json').write_text(json.dumps(report, indent=2) + '\n', encoding='utf-8', newline='\n')
    print(json.dumps([{k: v for k, v in r.items() if k in ('label', 'names', 'declaration_order', 'changed_methods', 'locals')} for r in comparisons]))


if __name__ == '__main__':
    main()
