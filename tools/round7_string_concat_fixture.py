"""Toy smcs 2.6.5 fixture: no game sources are compiled or changed."""
import hashlib,json,os,shutil,subprocess,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1]
sys.path.insert(0,str(ROOT/'tools'))
from compiler_probes import ROSLYN
from compare_kairo import differences
from round4_symbols import inspect_methods

FOLDER=ROOT/'build/round7/string-concat-fixture'
RULE=ROOT/'tools/Round7StringConcatRules.cs'
HOST=r'''using System;using System.IO;using System.Linq;using System.Collections.Generic;using System.Text.Json;
using Microsoft.CodeAnalysis;using Microsoft.CodeAnalysis.CSharp;using Microsoft.CodeAnalysis.CSharp.Syntax;
public static partial class Round7TransformSyntax {
 sealed class Patch {public string rule{get;set;}public int start{get;set;}public int end{get;set;}public string before{get;set;}public string after{get;set;}public int distance{get;set;}}
 public static void Main(string[] args){string text=File.ReadAllText(args[0]);var tree=CSharpSyntaxTree.ParseText(text);var compilation=CSharpCompilation.Create("SourceOnly",new[]{tree},new[]{MetadataReference.CreateFromFile(args[1])});var sem=compilation.GetSemanticModel(tree);var rows=new List<object>();
 using(var doc=JsonDocument.Parse(args.Length>2&&args[2]=="nohint"?"{}":JsonSerializer.Serialize(new Dictionary<string,object>{{"round7_cycle_rule",args.Length>2?args[2]:"explicit-string-concat-preserve-literals"},{"original_string_concat_arity",args.Length>3?int.Parse(args[3]):4}})))
 foreach(var method in tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>()){if(method.Body==null)continue;var patches=new List<Patch>();AddRound7StringConcatRules(doc.RootElement,text,method.Body,method.Body,sem,method.Body.SpanStart,patches,new HashSet<string>());rows.Add(new{method=method.Identifier.ValueText,patches=patches});}
 Console.WriteLine(JsonSerializer.Serialize(rows));}
}'''
SOURCE=r'''public class Box { public string A; public string B; }
public class Weird {public static string operator +(Weird a,string b){return b;}}
public static class Fixture {
 static int count;static string Read(int value){count=count*10+value;return "x";}
 public static string Params(string a,string b){return a+"\t"+" "+b;}
 public static string Fields(Box box){return box.A+"\t"+" "+box.B;}
 public static string Calls(){return Read(1)+"\t"+" "+Read(2);}
 public static string Constants(string a){const string Tab="\t";const string Space=" ";return a+Tab+Space+"z";}
 public static string ThreeParts(string a,string b){return a+"\t"+b;}
 public static string FiveParts(string a,string b){return a+"\t"+" "+"z"+b;}
 public static string FourNoAdjacent(string a,string b,string c,string d){return a+b+c+d;}
 public static string ObjectPart(object a,string b){return a+"\t"+" "+b;}
 public static string OperatorPart(Weird a,string b){return a+"\t"+" "+b;}
}'''
CONTROLS={
 'Params':('a+"\\t"+" "+b','global::System.String.Concat(a,"\\t"," ",b)'),
 'Fields':('box.A+"\\t"+" "+box.B','global::System.String.Concat(box.A,"\\t"," ",box.B)'),
 'Calls':('Read(1)+"\\t"+" "+Read(2)','global::System.String.Concat(Read(1),"\\t"," ",Read(2))'),
 'Constants':('a+Tab+Space+"z"','global::System.String.Concat(a,Tab,Space,"z")'),
}


def helper():
    vs=next(p for p in ROSLYN.parents if (p/'dotnet').is_dir());runtime=vs/'dotnet/net10.0/runtime'
    framework=runtime/'shared/Microsoft.NETCore.App/10.0.10'
    libs=['System.Private.CoreLib','System.Runtime','System.Console','System.Linq','System.Collections','System.Text.Json','System.Collections.Immutable','System.Memory','netstandard']
    refs=[framework/(n+'.dll') for n in libs]+[ROSLYN.parent/(n+'.dll') for n in ['Microsoft.CodeAnalysis','Microsoft.CodeAnalysis.CSharp']]
    host=FOLDER/'Host.cs';host.write_text(HOST,encoding='utf-8',newline='\n');target=FOLDER/'ConcatHost.dll'
    args=['/nologo','/nostdlib+','/target:exe','/out:'+str(target)]+['/r:'+str(r) for r in refs]+[str(host),str(RULE)]
    rsp=FOLDER/'host.rsp';rsp.write_text('\n'.join('"'+a+'"' for a in args)+'\n',encoding='utf-8',newline='\n')
    result=subprocess.run([str(ROSLYN),'/noconfig','@'+str(rsp)],capture_output=True)
    (FOLDER/'host.log').write_bytes(result.stdout+result.stderr)
    if result.returncode:raise RuntimeError(result.stdout+result.stderr)
    for r in refs[-2:]:shutil.copy2(r,FOLDER/r.name)
    (FOLDER/'ConcatHost.runtimeconfig.json').write_text(json.dumps({'runtimeOptions':{'tfm':'net10.0','framework':{'name':'Microsoft.NETCore.App','version':'10.0.10'}}}),encoding='utf-8')
    return [str(runtime/'dotnet.exe'),str(target)]


def compile_fixture(kind,source,manifest):
    folder=FOLDER/kind;folder.mkdir(parents=True,exist_ok=True);path=folder/'Fixture.cs';path.write_text(source,encoding='utf-8',newline='\n');binary=folder/'Fixture.dll'
    flags=[f for f in manifest['flags'] if not f.startswith('-out:') and f!='-noconfig']
    args=flags+['-out:'+str(binary)]+['-r:'+r['path'] for r in manifest['references']]+[str(path)]
    rsp=folder/'compile.rsp';rsp.write_text('\n'.join('"'+a+'"' for a in args)+'\n',encoding='utf-8',newline='\n')
    env=os.environ.copy();env.update(manifest['scoped_environment'])
    run=subprocess.run([manifest['runtime'],manifest['compiler'],'-noconfig','@'+str(rsp)],capture_output=True,env=env,cwd=ROOT)
    (folder/'compiler.log').write_bytes(run.stdout+run.stderr)
    if run.returncode:raise RuntimeError(run.stdout+run.stderr)
    return {m['stable_id']:m for m in inspect_methods(binary,symbols=False)['methods']}


def main():
    FOLDER.mkdir(parents=True,exist_ok=True)
    manifest=json.loads((ROOT/'build/round7/baseline/Assembly-CSharp/build-manifest.json').read_text(encoding='utf-8-sig'))
    path=FOLDER/'Input.cs';path.write_text(SOURCE,encoding='utf-8',newline='\n');command=helper()+[str(path),manifest['references'][0]['path']]
    rows=json.loads(subprocess.run(command,capture_output=True,check=True).stdout)
    for suffix in [['nohint'],['explicit-string-concat-preserve-literals','3']]:
        rejected=json.loads(subprocess.run(command+suffix,capture_output=True,check=True).stdout)
        assert not any(row['patches'] for row in rejected)
    unsupported=['ThreeParts','FiveParts','FourNoAdjacent','ObjectPart','OperatorPart']
    assert not any(row['patches'] for row in rows if row['method'] in unsupported)
    selected=[row['patches'][0] for row in rows if row['method'] in CONTROLS]
    assert len(selected)==4,rows
    candidate=SOURCE
    for p in sorted(selected,key=lambda p:p['start'],reverse=True):
        assert candidate[p['start']:p['end']]==p['before'];candidate=candidate[:p['start']]+p['after']+candidate[p['end']:]
    control=SOURCE
    for before,after in CONTROLS.values():control=control.replace(before,after)
    recovered=compile_fixture('recovered',SOURCE,manifest);expected=compile_fixture('control',control,manifest);actual=compile_fixture('candidate',candidate,manifest)
    results=[]
    for name in CONTROLS:
        identity=next(k for k in actual if k.startswith('Fixture::'+name+'('));delta,_=differences(expected[identity],actual[identity]);different,_=differences(expected[identity],recovered[identity])
        results.append(dict(method=name,body_exact_against_explicit_concat4_control=not delta,recovered_plus_differs=bool(different),differences=delta,recovered_instructions=recovered[identity]['instructions'],candidate_instructions=actual[identity]['instructions'],control_instructions=expected[identity]['instructions']))
    report=dict(operation='independent-smcs-explicit-concat4-fixture',compiler_sha256=manifest['compiler_sha256'],runtime_sha256=manifest['runtime_sha256'],rule_sha256=hashlib.sha256(RULE.read_bytes()).hexdigest(),fixture_compilations=3,game_candidates_compiled=0,no_hint_rejected=True,wrong_arity_rejected=True,rejected_shapes=unsupported,fixtures=results,patches=selected)
    (FOLDER/'fixture-report.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8',newline='\n');(ROOT/'reports/round7-string-concat-fixture.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8',newline='\n');print([(r['method'],r['body_exact_against_explicit_concat4_control'],r['recovered_plus_differs']) for r in results]);assert all(r['body_exact_against_explicit_concat4_control'] and r['recovered_plus_differs'] for r in results)


if __name__=='__main__':main()
