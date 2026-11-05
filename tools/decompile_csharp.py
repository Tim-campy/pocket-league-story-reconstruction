"""Recover Assembly-CSharp using the existing VS ILSpy/runtime and Unity profile.

Refuses existing files. The user explicitly authorized this assembly's initial
whole decompilation. It neither downloads software nor executes the target.
"""
import csv
import json
from pathlib import Path
import shutil
import subprocess
from compiler_probes import UNITY,ROSLYN

ROOT=Path(__file__).resolve().parents[1]


def main():
    vs=ROSLYN.parents[4]
    runtime=vs/'dotnet/net10.0/runtime'
    shared=runtime/'shared/Microsoft.NETCore.App/10.0.10'
    engine=vs/'Common7/IDE/CommonExtensions/Microsoft/VBCSharp/LanguageServices/Core/ICSharpCode.Decompiler.dll'
    output=ROOT/'build/decompiler-csharp';output.mkdir(parents=True,exist_ok=True)
    references=['System.Private.CoreLib','System.Runtime','System.Console','System.Linq','System.Collections',
        'System.Text.Json','System.Reflection.Metadata','System.Collections.Immutable','System.Memory','netstandard']
    for path in [runtime/'dotnet.exe',engine,*[shared/(name+'.dll') for name in references]]:
        if not path.is_file(): raise FileNotFoundError(path)
    flags=['/nologo','/nostdlib+','/target:exe','/out:'+str(output/'DecompileAssemblyCSharp.dll')]
    flags+=['/reference:'+str(shared/(name+'.dll')) for name in references]
    flags+=['/reference:'+str(engine),str(ROOT/'tools/DecompileAssemblyCSharp.cs')]
    rsp=output/'compile.rsp';rsp.write_text('\n'.join('"'+f+'"' for f in flags)+'\n',encoding='utf-8')
    subprocess.run([str(ROSLYN),'/noconfig','@'+str(rsp)],check=True,capture_output=True)
    shutil.copy2(engine,output/'ICSharpCode.Decompiler.dll')
    (output/'DecompileAssemblyCSharp.runtimeconfig.json').write_text(json.dumps({'runtimeOptions':{'tfm':'net10.0',
        'framework':{'name':'Microsoft.NETCore.App','version':'10.0.10'}}}),encoding='utf-8')
    with (ROOT/'config/types.csv').open(encoding='utf-8',newline='') as file:rows=list(csv.DictReader(file))
    manifest=[{'type':r['tipo'],'output':str(ROOT/r['fonte'])} for r in rows
        if r['assembly']=='Assembly-CSharp' and not r['tipo_pai'] and r['fonte']]
    manifest_path=output/'manifest.json';manifest_path.write_text(json.dumps(manifest,indent=2),encoding='utf-8')
    subprocess.run([str(runtime/'dotnet.exe'),str(output/'DecompileAssemblyCSharp.dll'),
        str(ROOT/'original/extracted/assets/bin/Data/Managed/Assembly-CSharp.dll'),str(manifest_path),str(UNITY),
        'CSharp3',str(ROOT/'src/Assembly-CSharp/AssemblyInfo.cs')],check=True,cwd=ROOT)


if __name__=='__main__':main()
