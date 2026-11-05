"""Reproduce the old Unity compiler's IndexOf ambiguity on synthetic source."""
import json
import os
from pathlib import Path
import subprocess
from compiler_probes import UNITY,MONO,SMCS,GMCS
from build_csharp import sha

ROOT=Path(__file__).resolve().parents[1]


def main():
    folder=ROOT/'build/csharp-indexof-probe';folder.mkdir(parents=True,exist_ok=True)
    core=ROOT/'original/extracted/assets/bin/Data/Managed/mscorlib.dll'
    bleeding=UNITY/'Editor/Data/MonoBleedingEdge'
    drivers=[('smcs-2.6.5',MONO,SMCS,UNITY/'Editor/Data/Mono/lib/mono/unity',UNITY/'Editor/Data/Mono/etc'),
        ('gmcs-2.6.5',MONO,GMCS,UNITY/'Editor/Data/Mono/lib/mono/unity',UNITY/'Editor/Data/Mono/etc'),
        ('mcs-3.4',bleeding/'bin/mono.exe',bleeding/'lib/mono/4.5/mcs.exe',bleeding/'lib/mono/4.5',bleeding/'etc')]
    variants={'two_arguments':'return s.IndexOf("x",0);',
        'named_argument':'return s.IndexOf("x",startIndex:0);',
        'int_variable':'int startIndex=0;return s.IndexOf("x",startIndex);',
        'one_argument_control':'return s.IndexOf("x");'}
    rows=[]
    for name,runtime,compiler,profile,config in drivers:
        env=os.environ.copy();env['MONO_PATH']=str(profile);env['MONO_CFG_DIR']=str(config)
        for label,body in variants.items():
            source=folder/(name+'-'+label+'.cs');source.write_text('public class P { public int M(string s) {'+body+'}}',encoding='utf-8')
            target=source.with_suffix('.dll')
            command=[str(runtime),str(compiler),'-noconfig','-nostdlib','-target:library','-langversion:Default',
                '-r:'+str(core),'-out:'+str(target),str(source)]
            result=subprocess.run(command,env=env,capture_output=True,text=True,encoding='utf-8',errors='replace')
            rows.append({'candidate':name,'variant':label,'exit_code':result.returncode,
                'compiler_sha256':sha(compiler),'runtime_sha256':sha(runtime),
                'error_codes':['CS0121'] if 'CS0121' in result.stdout+result.stderr else [],
                'source':source.read_text(encoding='utf-8')})
    report={'reference_sha256':sha(core),'scope':'synthetic source; no target execution',
        'reference_identity':'mscorlib 2.0.5.0, PublicKeyToken7cec85d7bea7798e','results':rows,
        'interpretation':'ambiguity reproduced with only corelib; exact cause UNKNOWN; one-argument fallback changes the target MemberRef and remains a known game method mismatch'}
    (ROOT/'reports/Assembly-CSharp-indexof-probe.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(json.dumps([{'candidate':r['candidate'],'variant':r['variant'],'exit_code':r['exit_code']} for r in rows]))


if __name__=='__main__':main()
