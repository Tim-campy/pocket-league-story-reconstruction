"""Generate isolated ILSpy variants, never overwrite recovered project source."""
import json
from pathlib import Path
import subprocess
from compiler_probes import UNITY,ROSLYN

ROOT=Path(__file__).resolve().parents[1]
VARIANTS={
    'expressions':{'NullPropagation':False,'IntroduceIncrementAndDecrement':False,
        'MakeAssignmentExpressions':False,'StringConcat':False,'StringInterpolation':False,
        'ObjectOrCollectionInitializers':False,'SeparateLocalVariableDeclarations':True,
        'UseImplicitMethodGroupConversion':False},
    'control_flow':{'NullPropagation':False,'ForStatement':False,'DoWhileStatement':False,
        'ForEachStatement':False,'UsingStatement':False,'LockStatement':False,
        'SwitchStatementOnString':False,'SparseIntegerSwitch':False,'StringInterpolation':False},
    'conservative':{'NullPropagation':False,'IntroduceIncrementAndDecrement':False,
        'MakeAssignmentExpressions':False,'ObjectOrCollectionInitializers':False,
        'StringInterpolation':False,'SeparateLocalVariableDeclarations':True,
        'UseImplicitMethodGroupConversion':False,'QueryExpressions':False,
        'ForStatement':False,'DoWhileStatement':False,'UsingStatement':False,'LockStatement':False},
}


def helper():
    vs=next(p for p in ROSLYN.parents if (p/'dotnet').is_dir())
    return [str(vs/'dotnet/net10.0/runtime/dotnet.exe'),str(ROOT/'build/source-recovery/SourceRecovery.dll')]


def main():
    manifest=json.loads((ROOT/'build/kairo-decompile-manifest.json').read_text())
    reports=[]
    for label,flags in VARIANTS.items():
        folder=ROOT/f'analysis/variants/kairo-ilspy-{label}';folder.mkdir(parents=True,exist_ok=True)
        entries=[];mapping={}
        for entry in manifest:
            logical=Path(entry['output']).relative_to(ROOT).as_posix();output=folder/logical
            mapping[logical]=str(output)
            if not output.is_file():entries.append({'type':entry['type'],'output':str(output)})
        options=folder/'settings.json';options.write_text(json.dumps(flags,indent=2)+'\n')
        plan=folder/'manifest.json';plan.write_text(json.dumps(entries,indent=2)+'\n')
        if entries:
            result=subprocess.run(helper()+['variant',str(ROOT/'original/extracted/assets/bin/Data/Managed/KairoLibrary.dll'),
                str(plan),str(UNITY),str(options)],capture_output=True,text=True,encoding='utf-8',errors='replace')
            if result.returncode:raise RuntimeError(result.stderr[-4000:])
        (folder/'source-map.json').write_text(json.dumps(mapping,indent=2)+'\n')
        reports.append({'label':label,'settings':flags,'type_files':len(mapping),'source_map':str((folder/'source-map.json').relative_to(ROOT))})
        print(json.dumps(reports[-1]),flush=True)
    (ROOT/'reports/KairoLibrary-decompiler-variants.json').write_text(json.dumps({'variants':reports,
        'source_changed':False,'compiler_package':'Microsoft.Net.Compilers 1.3.2'},indent=2)+'\n')


if __name__=='__main__':main()
