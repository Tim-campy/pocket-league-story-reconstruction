"""Explicitly acquire/build an independent decompiler in ignored analysis/.

Uses existing Git, Visual Studio MSBuild/Roslyn and Windows .NET Framework.
Only portable reference assemblies are fetched; no SDK/editor is installed.
"""
import argparse
import hashlib
import json
import os
from pathlib import Path
import subprocess
from urllib.request import urlopen
import zipfile

ROOT=Path(__file__).resolve().parents[1]
REPOSITORY='https://github.com/codemerx/JustDecompileEngine.git'
COMMIT='35f93d8fae9f6220bc8c976d1dbf376e57d6321d'
REF_URL='https://api.nuget.org/v3-flatcontainer/microsoft.netframework.referenceassemblies.net472/1.0.3/microsoft.netframework.referenceassemblies.net472.1.0.3.nupkg'
REF_SHA='ffa0a5570a39f911399164d0581ffddef99b5e3dfbaa5f220e5ce22969bcf57c'


def sha(path): return hashlib.sha256(path.read_bytes()).hexdigest()


def run(command,**kwargs):
    result=subprocess.run(command,capture_output=True,text=True,encoding='utf-8',errors='replace',**kwargs)
    if result.returncode: raise RuntimeError((result.stdout+result.stderr)[-5000:])
    return result.stdout


def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--decompile',action='store_true',help='Run the CLI on KairoLibrary; requires a console/PTY')
    args=parser.parse_args()
    repo=ROOT/'analysis/tools/JustDecompileEngine'
    if not (repo/'.git').is_dir():
        repo.mkdir(parents=True,exist_ok=True)
        run(['git','init',str(repo)])
        run(['git','-C',str(repo),'remote','add','origin',REPOSITORY])
        run(['git','-C',str(repo),'fetch','--depth','1','origin',COMMIT],timeout=180)
        run(['git','-C',str(repo),'-c','core.longpaths=true','checkout','--detach',COMMIT],timeout=90)
    actual=run(['git','-C',str(repo),'rev-parse','HEAD']).strip()
    if actual!=COMMIT or run(['git','-C',str(repo),'status','--porcelain']).strip():
        raise ValueError('Expected the pinned, unmodified upstream decompiler source')
    package=ROOT/'analysis/downloads/microsoft.netframework.referenceassemblies.net472.1.0.3.nupkg'
    if not package.is_file():
        package.parent.mkdir(parents=True,exist_ok=True)
        with urlopen(REF_URL,timeout=60) as response:data=response.read()
        if hashlib.sha256(data).hexdigest()!=REF_SHA:raise ValueError('Reference package hash mismatch')
        package.write_bytes(data)
    if sha(package)!=REF_SHA:raise ValueError('Reference package hash mismatch')
    references=ROOT/'analysis/tools/net472-reference-assemblies'
    if not (references/'build/.NETFramework/v4.7.2/mscorlib.dll').is_file():
        with zipfile.ZipFile(package) as archive:
            for info in archive.infolist():
                if not (references/info.filename).resolve().is_relative_to(references.resolve()):
                    raise ValueError('Unsafe archive member')
            archive.extractall(references)
    msbuild=Path(os.environ.get('MSBUILD_EXE','C:/Program Files/Microsoft Visual Studio/18/Community/MSBuild/Current/Bin/MSBuild.exe'))
    if not msbuild.is_file():raise ValueError('Existing MSBuild is required; set MSBUILD_EXE')
    command=[str(msbuild),str(repo/'src/JustDecompileEngine.sln'),'/t:ConsoleRunner',
        '/p:Configuration=Debug','/p:Platform=Any CPU','/p:LangVersion=latest',
        '/p:TargetFrameworkRootPath='+str(references/'build'),'/p:PostBuildEvent=',
        '/p:PreBuildEvent=','/verbosity:minimal','/nologo']
    log=run(command,timeout=180)
    (ROOT/'analysis/justdecompile-build-success.log').write_text(log,encoding='utf-8',newline='\n')
    output=repo/'src/ConsoleRunner/bin/Debug';runner=output/'ConsoleRunner.exe'
    files=[runner]+sorted(output.glob('*.dll'))
    record={'decompiler':'Codemerx/JustDecompileEngine','repository':REPOSITORY,'commit':COMMIT,
        'engine_assembly_version_observed':'2019.1.118.0','build_configuration':'Debug / EngineOnly, LangVersion=latest',
        'upstream_source_modified':False,'global_installations':[],
        'portable_reference_package':{'name':'Microsoft.NETFramework.ReferenceAssemblies.net472',
            'version':'1.0.3','bytes':package.stat().st_size,'sha256':REF_SHA,'url':REF_URL},
        'binaries':[{'path':p.relative_to(ROOT).as_posix(),'sha256':sha(p)} for p in files],
        'console_required':True,'generated_source_folder':'analysis/variants/kairo-codemerx',
        'source_license':'AGPL-3.0; external source/binaries not redistributed in this repository'}
    if args.decompile:
        target=ROOT/'original/extracted/assets/bin/Data/Managed/KairoLibrary.dll'
        destination=ROOT/'analysis/variants/kairo-codemerx'
        # Inherit the real console. Redirecting this upstream CLI breaks its
        # ConsoleColor functions on Windows; Codex must allocate tty:true.
        code=subprocess.run([str(runner),'/target:'+str(target),'/out:'+str(destination),
            '/lang:csharp','/vs:2015','/nodoc','/norename','/nohex'],cwd=ROOT,timeout=180).returncode
        if code:raise RuntimeError('Decompiler CLI failed: '+str(code))
    variants=ROOT/'analysis/variants/kairo-codemerx'
    record['generated_sources']=[{'path':p.relative_to(ROOT).as_posix(),'sha256':sha(p)} for p in sorted(variants.rglob('*.cs'))]
    record['generated_source_count']=len(record['generated_sources'])
    (ROOT/'reports/Codemerx-decompiler.json').write_text(json.dumps(record,indent=2)+'\n',encoding='utf-8',newline='\n')
    print(json.dumps({key:record[key] for key in ('decompiler','commit','upstream_source_modified','generated_source_count')}))


if __name__=='__main__':main()
