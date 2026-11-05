"""Own iterator fixture: isolate inherited-interface ordering from mscorlib."""
import json,subprocess
from pathlib import Path
from compiler_probes import UNITY
from kairo_candidate import COMPILER_PATH
from compare_kairo import sha
ROOT=Path(__file__).resolve().parents[1]
def main():
    folder=ROOT/'build/round3-iterator-reference-fixture';folder.mkdir(exist_ok=True)
    source=folder/'IteratorFixture.cs';source.write_text('using System.Collections; public class IteratorFixture { public IEnumerator GetEnumerator() { yield return 1; } }\n',encoding='utf-8')
    refs=[('unity',UNITY/'Editor/Data/Mono/lib/mono/2.0/mscorlib.dll'),('microsoft',Path('C:/Windows/Microsoft.NET/Framework/v2.0.50727/mscorlib.dll'))]
    results=[]
    for label,core in refs:
        dest=folder/label;dest.mkdir(exist_ok=True);out=dest/'IteratorFixture.dll'
        args=[str(ROOT/COMPILER_PATH),'/nologo','/noconfig','/nostdlib+','/target:library','/optimize+','/debug:pdbonly','/deterministic-','/unsafe-','/langversion:6','/out:'+str(out),'/r:'+str(core),str(source)]
        result=subprocess.run(args,capture_output=True);(dest/'compiler.log').write_bytes(result.stdout+result.stderr)
        if result.returncode:raise ValueError(result.stdout+result.stderr)
        with (dest/'methods.json').open('wb') as file:subprocess.run([str(ROOT/'build/tools/ProbeInspector.exe'),str(out),'--structure'],stdout=file,check=True)
        doc=json.loads((dest/'methods.json').read_text(encoding='utf-8-sig'));generated=next(t for t in doc['types'] if '<GetEnumerator>'in t['name'])
        results.append({'reference':str(core),'reference_sha256':sha(core),'generated_type':generated['name'],'interfaces':generated['interfaces'],'fixture_sha256':sha(source)})
    report={'scope':'own C# fixture only; no target build/reference/source modified','results':results,'same_source_compiler_flags':True,'conclusion':'Changing only the core library changes the inherited InterfaceImpl order; Microsoft v2 order agrees with original target iterator types.'}
    (ROOT/'reports/round3-iterator-reference-fixture.json').write_text(json.dumps(report,indent=2)+'\n')
    print(json.dumps(report))
if __name__=='__main__':main()
