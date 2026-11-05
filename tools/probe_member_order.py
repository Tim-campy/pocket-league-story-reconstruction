"""Empirically test Roslyn 1.3.2 generated ordinals using our own C# fixtures."""
import json
from pathlib import Path
import subprocess
from compiler_probes import UNITY
from compare_kairo import sha

ROOT=Path(__file__).resolve().parents[1]


def main():
    folder=ROOT/'build/member-order-probes';folder.mkdir(parents=True,exist_ok=True)
    compiler=ROOT/'analysis/tools/roslyn-1.3.2/tools/csc.exe'
    methods={'A':'public Thunk A() { return () => 1; }',
        'B':'public Thunk B(int x) { return () => x; }',
        'field':'public int field;', 'property':'public int P { get; set; }',
        'ctor':'public Probe() {}','nested':'public class Nested {}',
        'manual_property':'public int P { get { return 1; } set {} }',
        'readonly_auto':'public int P { get; }', 'two_fields':'public int x,y;',
        'event':'public event System.EventHandler Changed;'}
    orders=[['A','B'],['field','A','B'],['property','A','B'],['ctor','A','B'],
        ['nested','A','B'],['B','A'],['A','property','B'],['A','field','B'],
        ['manual_property','A','B'],['readonly_auto','A','B'],['two_fields','A','B'],['event','A','B']]
    results=[]
    for index,order in enumerate(orders):
        text='public delegate int Thunk(); public class Probe { '+ ' '.join(methods[m] for m in order)+' }'
        source=folder/f'probe-{index}.cs';source.write_text(text,encoding='utf-8')
        output=folder/f'probe-{index}.dll'
        command=[str(compiler),'/nologo','/noconfig','/nostdlib+','/target:library','/optimize+',
            '/langversion:6','/out:'+str(output),'/r:'+str(UNITY/'Editor/Data/Mono/lib/mono/2.0/mscorlib.dll'),str(source)]
        subprocess.run(command,check=True,capture_output=True)
        dump=json.loads(subprocess.check_output([str(ROOT/'build/tools/ProbeInspector.exe'),str(output),'--structure']).decode('utf-8-sig'))
        results.append({'declared_members':order,'source_sha256':sha(source),
            'generated_types':[t['name'] for t in dump['types'] if '<' in t['name']],
            'generated_methods':[m['stable_id'] for m in dump['selected_methods'] if '<' in m['stable_id']]})
    report={'compiler_package':'Microsoft.Net.Compilers 1.3.2','executable_version':'1.3.1.60621',
        'compiler_sha256':sha(compiler),'fixtures':results,
        'scope':'our own fixture; records declaration-order sensitivity, not recovered historical order'}
    (ROOT/'reports/KairoLibrary-member-order-probe.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(json.dumps(results,indent=2))


if __name__=='__main__': main()
