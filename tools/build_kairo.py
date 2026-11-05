"""Build all recovered KairoLibrary C# with Roslyn 5.8 and explicit options.

No original IL or DLL is copied into the output. SDK/runtime DLLs are references
only. The original remains read-only; generated artifacts are confined to build/.
"""
import hashlib
import argparse
import json
import re
from pathlib import Path
import subprocess
from compiler_probes import UNITY, ROSLYN
from kairo_candidate import CORELIB_PATH,CORELIB_SHA256

ROOT=Path(__file__).resolve().parents[1]


def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--compiler',type=Path,default=ROSLYN)
    parser.add_argument('--label',default='')
    parser.add_argument('--source-map',type=Path,help='Isolated trial: logical source path to replacement input path')
    parser.add_argument('--compile-order',type=Path,help='Isolated trial: alternate source order, confined to the project')
    options=parser.parse_args()
    compiler=options.compiler.resolve()
    if not compiler.is_file(): raise SystemExit('Compiler not found: '+str(compiler))
    name='KairoLibrary'+('-'+options.label if options.label else '')
    folder=(ROOT/'build'/name).resolve()
    if not folder.is_relative_to((ROOT/'build').resolve()): raise SystemExit('Output must stay within build/')
    folder.mkdir(parents=True,exist_ok=True)
    order_path=options.compile_order.resolve() if options.compile_order else ROOT/'config/compile-order/KairoLibrary.txt'
    if not order_path.is_relative_to(ROOT) or order_path.is_relative_to(ROOT/'original'):raise ValueError('Trial order must stay in the project outside original/')
    order=[p for p in order_path.read_text(encoding='utf-8').splitlines() if p and not p.startswith('#')]
    attributes='src/KairoLibrary/AssemblyInfo.cs'
    if attributes not in order: order.append(attributes)
    if len(set(order))!=len(order):raise ValueError('Duplicate source in compile order')
    replacements=json.loads(options.source_map.read_text(encoding='utf-8')) if options.source_map else {}
    if set(replacements)-set(order): raise SystemExit('Source map contains unknown logical paths')
    selected={p:Path(replacements[p]).resolve() if p in replacements else ROOT/p for p in order}
    if any(not p.is_relative_to(ROOT) for p in selected.values()): raise SystemExit('Trial source input must stay inside the project')
    missing=[p for p in order if not selected[p].is_file()]
    if missing: raise SystemExit('Missing source files: '+str(len(missing)))
    profile=UNITY/'Editor/Data/Mono/lib/mono/2.0'
    # The original KairoLibrary references the classic .NET 2.0/3.5 profile,
    # whereas Assembly-CSharp uses Unity's 2.0.5.0 profile. Preserve that split.
    corelib=Path(CORELIB_PATH)
    if not corelib.is_file() or hashlib.sha256(corelib.read_bytes()).hexdigest()!=CORELIB_SHA256:
        raise ValueError('Pinned Microsoft mscorlib2.0 reference missing or changed')
    references=[corelib,profile/'System.Core.dll',
        ROOT/'original/extracted/assets/bin/Data/Managed/UnityEngine.dll',profile/'System.dll']
    for path in references:
        if not path.is_file(): raise SystemExit('Missing reference: '+str(path))
    output=folder/'KairoLibrary.dll'
    flags=['/nologo','/noconfig','/nostdlib+','/target:library','/platform:anycpu','/optimize+',
        '/debug:pdbonly','/deterministic-','/langversion:6','/unsafe-','/checked-',
        '/codepage:65001','/utf8output','/out:'+str(output)]
    # Both original assemblies lack the unsafe-generated module/security rows.
    # Own fixtures prove their origin; the full verifier checks this selection.
    args=[flag for flag in flags if flag!='/noconfig']+['/reference:'+str(p) for p in references]+[str(selected[p]) for p in order]
    rsp=folder/'compile.rsp';rsp.write_text('\n'.join('"'+a+'"' for a in args)+'\n',encoding='utf-8',newline='\n')
    result=subprocess.run([str(compiler),'/noconfig','@'+str(rsp)],capture_output=True,text=True,encoding='utf-8',errors='replace')
    log=result.stdout+result.stderr;(folder/'compiler.log').write_text(log,encoding='utf-8',newline='\n')
    manifest={'compiler':str(compiler),'compiler_sha256':hashlib.sha256(compiler.read_bytes()).hexdigest(),
        'flags':flags,'references':[{'path':str(p),'sha256':hashlib.sha256(p.read_bytes()).hexdigest()} for p in references],
        'sources':[dict({'path':p,'sha256':hashlib.sha256(selected[p].read_bytes()).hexdigest()},
            **({'input_path':str(selected[p])} if p in replacements else {})) for p in order],
        'exit_code':result.returncode,'output_sha256':hashlib.sha256(output.read_bytes()).hexdigest() if result.returncode==0 and output.exists() else None}
    (folder/'build-manifest.json').write_text(json.dumps(manifest,indent=2)+'\n',encoding='utf-8')
    errors=[line for line in log.splitlines() if re.search(r'\berror\s+CS\d+',line)]
    print(json.dumps({'assembly':'KairoLibrary','source_files':len(order),'exit_code':result.returncode,
        'errors':len(errors),'first_errors':errors[:18],'log':(folder/'compiler.log').relative_to(ROOT).as_posix()},ensure_ascii=True))
    return result.returncode


if __name__=='__main__': raise SystemExit(main())
