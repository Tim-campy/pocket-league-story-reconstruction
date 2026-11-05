"""Compile only own fixtures to measure loop/block-driven delegate caches."""
import json,subprocess
from pathlib import Path
from compiler_probes import UNITY
from kairo_candidate import COMPILER_PATH
ROOT=Path(__file__).resolve().parents[1]
SOURCE='''using System;
public class ScopeFixture {
 static bool Check(Func<bool> a) { return true; }
 public void Condition(int x) { while (!Check(delegate { return x>0; })) {} }
 public void BodyIf(int x) { while (true) { if (Check(delegate { return x>0; })) break; } }
 public void BodyBool(int x) { while (true) { bool done=Check(delegate { return x>0; }); if(done) break; } }
 public void BodyDummy(int x) { while (true) { int dummy=0; if(Check(delegate { return x>0; }))break; } }
 public void BodyDo(int x) { bool done; do { done=Check(delegate { return x>0; }); } while(!done); }
 public void BodyFor(int x) { for (;;) { bool done=Check(delegate { return x>0; }); if(done)break; } }
}'''
def main():
    folder=ROOT/'build/round3-cache-scope-fixtures';folder.mkdir(exist_ok=True)
    source=folder/'ScopeFixture.cs';source.write_text(SOURCE,encoding='utf-8')
    output=folder/'ScopeFixture.dll'
    args=[str(ROOT/COMPILER_PATH),'/nologo','/noconfig','/nostdlib+','/target:library','/optimize+','/debug:pdbonly','/deterministic-','/unsafe-','/langversion:6','/out:'+str(output),'/r:'+str(UNITY/'Editor/Data/Mono/lib/mono/2.0/mscorlib.dll'),'/r:'+str(UNITY/'Editor/Data/Mono/lib/mono/2.0/System.Core.dll'),str(source)]
    result=subprocess.run(args,capture_output=True);(folder/'compiler.log').write_bytes(result.stdout+result.stderr)
    if result.returncode:raise ValueError(result.stdout+result.stderr)
    with (folder/'methods.json').open('wb') as file:subprocess.run([str(ROOT/'build/tools/ProbeInspector.exe'),str(output),'--structure'],stdout=file,check=True)
    d=json.loads((folder/'methods.json').read_text(encoding='utf-8-sig'))
    expected={'Condition':False,'BodyIf':False,'BodyBool':True,'BodyDummy':True,'BodyDo':False,'BodyFor':True}
    controls=[]
    for method,wanted in expected.items():
        closure=next(t for t in d['types'] if any(('::<'+method+'>') in m for m in t['methods']))
        found=any(f['name'].startswith('<>9__') for f in closure['fields'])
        if found!=wanted:raise ValueError('Closure-cache fixture changed: '+method)
        controls.append({'method':method,'expected_cache':wanted,'observed_cache':found,'passed':found==wanted})
    report={'scope':'Own fixture only; no target source/assembly modified','compiler':'1.1.0.51109','source':SOURCE,'closures':[{k:t[k] for k in ['name','fields','methods']} for t in d['types'] if 'DisplayClass'in t['name']],
      'methods':[{'id':m['stable_id'],'locals':m.get('locals'),'instructions':m.get('instructions')} for m in d['selected_methods'] if m['stable_id'].startswith('ScopeFixture::')],
      'source_reference':'https://raw.githubusercontent.com/dotnet/roslyn/version-1.1.0/src/Compilers/CSharp/Portable/Lowering/LambdaRewriter/LambdaRewriter.cs#L1025','controls':controls,'controls_passed':len(controls)}
    (ROOT/'reports/round3-cache-scope-fixtures.json').write_text(json.dumps(report,indent=2)+'\n')
    for t in report['closures']:print(t['methods'],[f['name'] for f in t['fields']])
if __name__=='__main__':main()
