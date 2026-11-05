"""Compile all recovered Assembly-CSharp sources with Unity Mono 2.6.5.0.

Original KairoLibrary is a compilation reference only. No original IL is copied
or used as a build output. Manifest records sources, options and reference hashes.
"""
import argparse
import hashlib
import json
import os
from pathlib import Path
import re
import subprocess
from compiler_probes import UNITY,MONO,SMCS,GMCS

ROOT=Path(__file__).resolve().parents[1]
def sha(path):return hashlib.sha256(path.read_bytes()).hexdigest()


def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--compiler',choices=['smcs','gmcs','mcs34'],default='smcs')
    parser.add_argument('--optimize',choices=['+','-'],default='+')
    parser.add_argument('--debug',choices=['+','-'],default='-')
    parser.add_argument('--label',default='mono-smcs')
    parser.add_argument('--stdlib',choices=['default','explicit'],default='default',
        help='explicit retains the baseline -nostdlib; default allows the compiler core library binding')
    parser.add_argument('--source-map',type=Path,help='Isolated trial: logical source path to replacement input path')
    args=parser.parse_args();compiler=SMCS if args.compiler=='smcs' else GMCS
    runtime=MONO;profile=UNITY/'Editor/Data/Mono/lib/mono/unity';config=UNITY/'Editor/Data/Mono/etc'
    if args.compiler=='mcs34':
        compiler=UNITY/'Editor/Data/MonoBleedingEdge/lib/mono/4.5/mcs.exe'
        runtime=UNITY/'Editor/Data/MonoBleedingEdge/bin/mono.exe'
        profile=UNITY/'Editor/Data/MonoBleedingEdge/lib/mono/4.5';config=UNITY/'Editor/Data/MonoBleedingEdge/etc'
    folder=(ROOT/'build'/('Assembly-CSharp-'+args.label)).resolve()
    if not folder.is_relative_to((ROOT/'build').resolve()):raise ValueError('Output outside build/')
    folder.mkdir(parents=True,exist_ok=True)
    sources=[p for p in (ROOT/'config/compile-order/Assembly-CSharp.txt').read_text(encoding='utf-8').splitlines() if p and not p.startswith('#')]
    sources.append('src/Assembly-CSharp/AssemblyInfo.cs')
    replacements=json.loads(args.source_map.read_text(encoding='utf-8-sig')) if args.source_map else {}
    if not isinstance(replacements,dict) or set(replacements)-set(sources):raise ValueError('Unknown logical source path')
    selected={p:(ROOT/Path(replacements[p])).resolve() if p in replacements else ROOT/p for p in sources}
    if any(not path.is_relative_to(ROOT) or path.is_relative_to(ROOT/'original') for path in selected.values()):
        raise ValueError('Source overrides must stay inside the workspace and outside original/')
    missing=[p for p in sources if not selected[p].is_file()]
    if missing:raise ValueError('Missing sources: '+str(len(missing)))
    managed=ROOT/'original/extracted/assets/bin/Data/Managed'
    references=[managed/name for name in ('mscorlib.dll','KairoLibrary.dll','UnityEngine.dll','System.Core.dll','System.dll')]
    target=folder/'Assembly-CSharp.dll'
    flags=['-nologo','-noconfig']+(['-nostdlib'] if args.stdlib=='explicit' else [])+['-target:library','-platform:anycpu',
        '-optimize'+args.optimize,'-debug'+args.debug,'-langversion:Default','-unsafe+','-checked-',
        '-codepage:65001','-out:'+str(target)]
    parameters=[f for f in flags if f!='-noconfig']+['-r:'+str(p) for p in references]+[str(selected[p]) for p in sources]
    rsp=folder/'compile.rsp';rsp.write_text('\n'.join('"'+p+'"' for p in parameters)+'\n',encoding='utf-8',newline='\n')
    env=os.environ.copy();env['MONO_PATH']=str(profile);env['MONO_CFG_DIR']=str(config)
    result=subprocess.run([str(runtime),str(compiler),'-noconfig','@'+str(rsp)],env=env,capture_output=True,text=True,encoding='utf-8',errors='replace')
    log=result.stdout+result.stderr;(folder/'compiler.log').write_text(log,encoding='utf-8',newline='\n')
    manifest={'assembly':'Assembly-CSharp','compiler':str(compiler),'compiler_sha256':sha(compiler),
        'runtime':str(runtime),'runtime_sha256':sha(runtime),'flags':flags,'stdlib_binding':args.stdlib,
        'scoped_environment':{k:env[k] for k in ('MONO_PATH','MONO_CFG_DIR')},
        'sources':[dict({'path':p,'sha256':sha(selected[p])},
            **({'input_path':str(selected[p])} if p in replacements else {})) for p in sources],
        'references':[{'path':str(p),'sha256':sha(p)} for p in references],
        'exit_code':result.returncode,'output_sha256':sha(target) if result.returncode==0 else None}
    (folder/'build-manifest.json').write_text(json.dumps(manifest,indent=2)+'\n',encoding='utf-8',newline='\n')
    errors=[line for line in log.splitlines() if re.search(r'\berror\s+CS\d+',line)]
    print(json.dumps({'assembly':'Assembly-CSharp','sources':len(sources),'exit_code':result.returncode,'errors':len(errors),
        'first_errors':errors[:15],'log':str((folder/'compiler.log').relative_to(ROOT))},ensure_ascii=True))
    return result.returncode


if __name__=='__main__':raise SystemExit(main())
