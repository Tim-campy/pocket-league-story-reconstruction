"""Independent fixtures for owner-aware switch conversion; no game candidates."""
import json
from pathlib import Path
import shutil
import subprocess

from compare_kairo import differences, sha
from compiler_probes import ROSLYN
from round4_symbols import inspect_methods

ROOT=Path(__file__).resolve().parents[1]
HOST=r'''using System;using System.IO;using System.Linq;using System.Collections.Generic;using System.Text.Json;
using Microsoft.CodeAnalysis;using Microsoft.CodeAnalysis.CSharp;using Microsoft.CodeAnalysis.CSharp.Syntax;
public static partial class Round7TransformSyntax {
 sealed class Patch {public string rule{get;set;}public int start{get;set;}public int end{get;set;}public string before{get;set;}public string after{get;set;}public int distance{get;set;}}
 static bool IsLoop(SyntaxNode n)=>n is ForStatementSyntax||n is ForEachStatementSyntax||n is WhileStatementSyntax||n is DoStatementSyntax;
 public static void Main(string[] args){string text=File.ReadAllText(args[0]);var tree=CSharpSyntaxTree.ParseText(text);var compilation=CSharpCompilation.Create("SourceOnly",new[]{tree},new[]{MetadataReference.CreateFromFile(args[1])});var sem=compilation.GetSemanticModel(tree);var rows=new List<object>();
 using(var doc=JsonDocument.Parse(args.Length>2&&args[2]=="nohint"?"{}":JsonSerializer.Serialize(new Dictionary<string,string>{{"round7_cycle_rule",args.Length>2?args[2]:"switch-owned-break-if-chain"}})))
 foreach(var method in tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>()){if(method.Body==null)continue;var patches=new List<Patch>();AddRound7RoslynCycleRules(doc.RootElement,text,method.Body,method.Body,sem,method.Body.SpanStart,patches,new HashSet<string>());rows.Add(new{method=method.Identifier.ValueText,patches=patches});}
 Console.WriteLine(JsonSerializer.Serialize(rows));}
}'''

SWITCHES={
 'Blocks':('switch(x){case 0:{for(int i=0;i<3;i++){if(i==1)break;sum+=i;}break;}case 1:{sum+=1;break;}default:{sum+=2;break;}}',
           'if(x==0){{for(int i=0;i<3;i++){if(i==1)break;sum+=i;}}}else if(x==1){{sum+=1;}}else{{sum+=2;}}'),
 'Nested':('switch(x){case 0:{switch(y){case 0:Sink(1);break;default:Sink(2);break;}break;}case 1:Sink(3);break;default:Sink(4);break;}',
           'if(x==0){{switch(y){case 0:Sink(1);break;default:Sink(2);break;}}}else if(x==1){Sink(3);}else{Sink(4);}'),
 'Large':('switch(x){'+''.join('case '+str(i)+':Sink('+str(i)+');break;' for i in range(10))+'}',
          ' else '.join('if(x=='+str(i)+'){Sink('+str(i)+');}' for i in range(10))),
 'MiddleBreak':('switch(x){case 0:{if(y){break;}Sink(0);break;}case 1:Sink(1);break;}',
                '{if(x==0){{if(y){goto done;}Sink(0);}}else if(x==1){Sink(1);}done:;}'),
}
SOURCE='''public static class Fixture {
 static int field;static void Sink(int x){field+=x;}static int Selector(){return field;}
 public static int Blocks(int x){int sum=0;for(int n=0;n<4;n++){SW_BLOCKS}return sum;}
 public static void Nested(int x,int y){SW_NESTED}
 public static void Large(int x){SW_LARGE}
 public static void MiddleBreak(int x,bool y){SW_MIDDLE}
 public static void GotoCase(int x){switch(x){case 0:goto case 1;case 1:Sink(1);break;}}
 public static void CallSelector(){switch(Selector()){case 0:Sink(0);break;}}
}'''
TOKENS={'Blocks':'SW_BLOCKS','Nested':'SW_NESTED','Large':'SW_LARGE','MiddleBreak':'SW_MIDDLE'}


def helper(folder):
    vs=next(p for p in ROSLYN.parents if (p/'dotnet').is_dir())
    runtime=vs/'dotnet/net10.0/runtime'
    framework=runtime/'shared/Microsoft.NETCore.App/10.0.10'
    libs=['System.Private.CoreLib','System.Runtime','System.Console','System.Linq','System.Collections','System.Text.Json','System.Collections.Immutable','System.Memory','netstandard']
    refs=[framework/(name+'.dll') for name in libs]+[ROSLYN.parent/(name+'.dll') for name in ['Microsoft.CodeAnalysis','Microsoft.CodeAnalysis.CSharp']]
    host=folder/'Host.cs';host.write_text(HOST,encoding='utf-8')
    target=folder/'CycleFixtureHost.dll'
    args=['/nologo','/nostdlib+','/target:exe','/out:'+str(target)]+['/r:'+str(r) for r in refs]+[str(host),str(ROOT/'tools/Round7RoslynCycleRules.cs')]
    rsp=folder/'host.rsp';rsp.write_text('\n'.join('"'+a+'"' for a in args)+'\n',encoding='utf-8')
    subprocess.run([str(ROSLYN),'/noconfig','@'+str(rsp)],check=True,capture_output=True)
    for reference in refs[-2:]:shutil.copy2(reference,folder/reference.name)
    (folder/'CycleFixtureHost.runtimeconfig.json').write_text(json.dumps({'runtimeOptions':{'tfm':'net10.0','framework':{'name':'Microsoft.NETCore.App','version':'10.0.10'}}}),encoding='utf-8')
    return [str(runtime/'dotnet.exe'),str(target)]


def compile_fixture(folder,source,manifest):
    folder.mkdir(parents=True,exist_ok=True)
    file=folder/'Fixture.cs';file.write_text(source,encoding='utf-8',newline='\n')
    out=folder/'Fixture.dll'
    flags=[f for f in manifest['flags'] if not f.startswith('/out:') and f!='/noconfig']
    args=flags+['/out:'+str(out)]+['/r:'+r['path'] for r in manifest['references']]+[str(file)]
    rsp=folder/'compile.rsp';rsp.write_text('\n'.join('"'+a+'"' for a in args)+'\n',encoding='utf-8')
    run=subprocess.run([manifest['compiler'],'/noconfig','@'+str(rsp)],capture_output=True)
    (folder/'compiler.log').write_bytes(run.stdout+run.stderr)
    if run.returncode:raise RuntimeError(run.stdout+run.stderr)
    return {m['stable_id']:m for m in inspect_methods(out,symbols=False)['methods']}


def main():
    folder=ROOT/'build/round7/cycle-roslyn-fixture';folder.mkdir(parents=True,exist_ok=True)
    manifest=json.loads((ROOT/'build/round7/baseline/KairoLibrary/build-manifest.json').read_text(encoding='utf-8-sig'))
    before=SOURCE;control=SOURCE
    for name,(switch,chain) in SWITCHES.items():before=before.replace(TOKENS[name],switch);control=control.replace(TOKENS[name],chain)
    boundaries='public static class Boundaries {'+''.join(
        'public static void Boundary'+str(count)+'(int x){switch(x){'+''.join(
            'case '+str(i)+':break;' for i in range(count))+'}}' for count in (32,33))+'}'
    before+=boundaries;control+=boundaries
    input_path=folder/'Input.cs';input_path.write_text(before,encoding='utf-8',newline='\n')
    command=helper(folder)
    args=command+[str(input_path),manifest['references'][0]['path']]
    rows=json.loads(subprocess.run(args,capture_output=True,check=True).stdout)
    nohint=json.loads(subprocess.run(args+['nohint'],capture_output=True,check=True).stdout)
    assert not any(row['patches'] for row in nohint)
    assert not any(row['patches'] for row in rows if row['method'] in ['GotoCase','CallSelector'])
    assert len(next(row['patches'] for row in rows if row['method']=='Boundary32'))==1
    assert not next(row['patches'] for row in rows if row['method']=='Boundary33')
    selected=[min(row['patches'],key=lambda p:(p['start'],-p['end'])) for row in rows if row['method'] in SWITCHES]
    assert len(selected)==4
    candidate=before
    for patch in sorted(selected,key=lambda p:p['start'],reverse=True):
        assert candidate[patch['start']:patch['end']]==patch['before']
        candidate=candidate[:patch['start']]+patch['after']+candidate[patch['end']:]
    old=compile_fixture(folder/'switch',before,manifest)
    expected=compile_fixture(folder/'control',control,manifest)
    actual=compile_fixture(folder/'candidate',candidate,manifest)
    results=[]
    for name in SWITCHES:
        identity=next(k for k in actual if k.startswith('Fixture::'+name+'('))
        delta,_=differences(expected[identity],actual[identity])
        results.append({'method':name,'body_exact_against_handwritten_if_control':not delta,'differences':delta,
                        'original_switch_has_switch_opcode':any(i['opcode']=='switch' for i in old[identity]['instructions']),
                        'candidate_instructions':actual[identity]['instructions'],'control_instructions':expected[identity]['instructions']})
    report={'operation':'independent-owner-aware-switch-rule-fixture','game_candidates_compiled':0,'toy_assembly_compilations':3,
            'rule_sha256':sha(ROOT/'tools/Round7RoslynCycleRules.cs'),'compiler_sha256':manifest['compiler_sha256'],
            'hint_required':True,'goto_case_rejected':True,'call_selector_rejected':True,
            'boundary_32_allowed':True,'boundary_33_rejected':True,'fixtures':results}
    (ROOT/'reports/round7-roslyn-cycle-switch-fixture.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8',newline='\n')
    print(json.dumps({k:v for k,v in report.items() if k!='fixtures'},ensure_ascii=False))
    print([(r['method'],r['body_exact_against_handwritten_if_control']) for r in results])
    assert all(r['body_exact_against_handwritten_if_control'] for r in results)


if __name__=='__main__':main()
