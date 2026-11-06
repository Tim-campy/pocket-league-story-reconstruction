"""Independent csc 1.1 controls for the declaration immediately before a for."""
import hashlib
import json
from pathlib import Path
import shutil
import subprocess

from compare_kairo import differences, sha
from compiler_probes import ROSLYN
from round4_symbols import inspect_methods
from round5_local_cases import local_inspector
from round7_roslyn_model import SlotModelClient

ROOT=Path(__file__).resolve().parents[1]
EXPERIMENT=ROOT/'build/round7/roslyn-for-declaration-fixture'
RULE=ROOT/'tools/Round7RoslynDeclarationRules.cs'
HOST=r'''using System;using System.IO;using System.Linq;using System.Collections.Generic;using System.Text.Json;
using Microsoft.CodeAnalysis;using Microsoft.CodeAnalysis.CSharp;using Microsoft.CodeAnalysis.CSharp.Syntax;
public static partial class Round7TransformSyntax {
 sealed class Patch {public string rule{get;set;}public int start{get;set;}public int end{get;set;}public string before{get;set;}public string after{get;set;}public int distance{get;set;}}
 public static void Main(string[] args){string text=File.ReadAllText(args[0]);string path=Path.GetFullPath(args[0]);var tree=CSharpSyntaxTree.ParseText(text,path:path);var trees=new List<SyntaxTree>();var refs=new List<MetadataReference>();
 if(Path.GetExtension(args[1])==".json"){using(var doc=JsonDocument.Parse(File.ReadAllText(args[1]))){foreach(var s in doc.RootElement.GetProperty("sources").EnumerateArray()){string logical=Path.GetFullPath(s.GetProperty("path").GetString());string actual=s.TryGetProperty("input_path",out var a)?a.GetString():logical;trees.Add(logical.Equals(path,StringComparison.OrdinalIgnoreCase)?tree:CSharpSyntaxTree.ParseText(File.ReadAllText(actual),path:logical));}foreach(var r in doc.RootElement.GetProperty("references").EnumerateArray())refs.Add(MetadataReference.CreateFromFile(r.GetProperty("path").GetString()));}}
 else{trees.Add(tree);refs.Add(MetadataReference.CreateFromFile(args[1]));}
 var compilation=CSharpCompilation.Create("SourceOnly",trees,refs,new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,optimizationLevel:OptimizationLevel.Release));var sem=compilation.GetSemanticModel(tree);var rows=new List<object>();
 string hint=args.Length>2?args[2]:"roslyn-for-declaration-immediately-before";
 using(var doc=JsonDocument.Parse(hint=="nohint"?"{}":JsonSerializer.Serialize(new Dictionary<string,string>{{"round7_cycle_rule",hint},{"compiler","roslyn"}})))
 foreach(var method in tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>()){if(method.Body==null)continue;var patches=new List<Patch>();AddRound7RoslynDeclarationRules(doc.RootElement,text,method.Body,method.Body,sem,method.Body.SpanStart,patches,new HashSet<string>());rows.Add(new{method=method.Identifier.ValueText,patches=patches});}
 Console.WriteLine(JsonSerializer.Serialize(rows));}
}'''
SOURCE=r'''using System;using System.Collections.Generic;
public delegate void D();
public static class Fixture {
 public static int index;static int count;
 static void TouchArray(ref int[] a){}static void TouchInt(ref int a){}static void TouchDouble(ref double a){}static void TouchString(ref string a){}
 static void SinkInt(int n){}static void SinkDouble(double n){}static void SinkString(string n){}
 static string Read(){count++;return count<3?"x":null;}
 public static void IntCase(int n){int[] a=new int[1];TouchArray(ref a);for(int index=0;index<n;index++)SinkInt(index);double later=0;TouchDouble(ref later);}
 public static void DoubleCase(int n){string a="a";TouchString(ref a);for(double value=0;value<n;value+=1)SinkDouble(value);int later=0;TouchInt(ref later);}
 public static void StringCase(){double a=0;TouchDouble(ref a);for(string text=Read();text!=null;text=Read())SinkString(text);int later=0;TouchInt(ref later);}
 public static void QualifiedField(int n){int[] a=new int[1];TouchArray(ref a);SinkInt(Fixture.index);for(int index=0;index<n;index++)SinkInt(index);double later=0;TouchDouble(ref later);}
 public static void Captured(int n){for(int i=0;i<n;i++){D f=delegate{SinkInt(i);};f();}}
 public static void Multiple(int n){for(int i=0,j=1;i<n;i++)SinkInt(i+j);}
 public static void ImplicitType(int n){for(var i=0;i<n;i++)SinkInt(i);}
 public static void WithoutBlock(int n){if(n>0)for(int i=0;i<n;i++)SinkInt(i);}
 public static void SiblingName(int n){for(int i=0;i<n;i++)SinkInt(i);for(int i=0;i<n;i++)SinkInt(i);}
 public static void UnqualifiedField(int n){SinkInt(index);for(int index=0;index<n;index++)SinkInt(index);}
 public static void NoInitializer(){for(int unused;false;){} }
 public static IEnumerable<int> Iterator(int n){for(int i=0;i<n;i++){yield return i;}}
 public static void Jump(int n){if(n<0)goto done;for(int i=0;i<n;i++)SinkInt(i);done:;}
}'''
CONTROLS={
 'IntCase':('for(int index=0;index<n;index++)','int index;for(index=0;index<n;index++)'),
 'DoubleCase':('for(double value=0;value<n;value+=1)','double value;for(value=0;value<n;value+=1)'),
 'StringCase':('for(string text=Read();text!=null;text=Read())','string text;for(text=Read();text!=null;text=Read())'),
 'QualifiedField':('for(int index=0;index<n;index++)','int index;for(index=0;index<n;index++)'),
}


def prepare_helper():
    vs=next(p for p in ROSLYN.parents if (p/'dotnet').is_dir());runtime=vs/'dotnet/net10.0/runtime';framework=runtime/'shared/Microsoft.NETCore.App/10.0.10'
    identity=hashlib.sha256(RULE.read_bytes()+HOST.encode()).hexdigest();folder=EXPERIMENT/'helper'/identity[:16];folder.mkdir(parents=True,exist_ok=True)
    target=folder/'DeclarationHost.dll'
    libs=['System.Private.CoreLib','System.Runtime','System.Console','System.Linq','System.Collections','System.Text.Json','System.Collections.Immutable','System.Memory','netstandard']
    refs=[framework/(n+'.dll') for n in libs]+[ROSLYN.parent/(n+'.dll') for n in ['Microsoft.CodeAnalysis','Microsoft.CodeAnalysis.CSharp']]
    if not target.exists():
        host=folder/'Host.cs';host.write_text(HOST,encoding='utf-8',newline='\n')
        args=['/nologo','/nostdlib+','/target:exe','/out:'+str(target)]+['/r:'+str(r) for r in refs]+[str(host),str(RULE)]
        rsp=folder/'compile.rsp';rsp.write_text('\n'.join('"'+a+'"' for a in args)+'\n',encoding='utf-8',newline='\n')
        run=subprocess.run([str(ROSLYN),'/noconfig','@'+str(rsp)],capture_output=True)
        (folder/'compiler.log').write_bytes(run.stdout+run.stderr)
        if run.returncode:raise RuntimeError(run.stdout+run.stderr)
        for r in refs[-2:]:shutil.copy2(r,folder/r.name)
        (folder/'DeclarationHost.runtimeconfig.json').write_text(json.dumps({'runtimeOptions':{'tfm':'net10.0','framework':{'name':'Microsoft.NETCore.App','version':'10.0.10'}}}),encoding='utf-8')
    return [str(runtime/'dotnet.exe'),str(target)]


def compile_fixture(folder,source,manifest):
    folder.mkdir(parents=True,exist_ok=True);path=folder/'Fixture.cs';path.write_text(source,encoding='utf-8',newline='\n');out=folder/'Fixture.dll'
    flags=[f for f in manifest['flags'] if not f.startswith('/out:') and f!='/noconfig'];args=flags+['/out:'+str(out)]+['/r:'+r['path'] for r in manifest['references']]+[str(path)]
    rsp=folder/'compile.rsp';rsp.write_text('\n'.join('"'+a+'"' for a in args)+'\n',encoding='utf-8',newline='\n')
    run=subprocess.run([manifest['compiler'],'/noconfig','@'+str(rsp)],capture_output=True,cwd=ROOT);(folder/'compiler.log').write_bytes(run.stdout+run.stderr)
    if run.returncode:raise RuntimeError(run.stdout+run.stderr)
    raw=subprocess.run([str(local_inspector()),str(out),'--locals-only'],capture_output=True,check=True)
    types={m['stable_id']:m.get('plain_local_types',[]) for m in json.loads(raw.stdout.decode('utf-8-sig'))['methods']}
    return {m['stable_id']:m for m in inspect_methods(out,symbols=False)['methods']},types


def main():
    folder=EXPERIMENT/'fixture';folder.mkdir(parents=True,exist_ok=True);manifest=json.loads((ROOT/'build/round7/baseline/KairoLibrary/build-manifest.json').read_text(encoding='utf-8-sig'))
    path=folder/'Input.cs';path.write_text(SOURCE,encoding='utf-8',newline='\n');command=prepare_helper()+[str(path),manifest['references'][0]['path']]
    rows=json.loads(subprocess.run(command,capture_output=True,check=True,cwd=ROOT).stdout);empty=json.loads(subprocess.run(command+['nohint'],capture_output=True,check=True,cwd=ROOT).stdout)
    assert not any(r['patches'] for r in empty)
    rejected=['Captured','Multiple','ImplicitType','WithoutBlock','SiblingName','UnqualifiedField','NoInitializer','Iterator','Jump']
    assert not any(r['patches'] for r in rows if r['method'] in rejected)
    selected=[r['patches'][0] for r in rows if r['method'] in CONTROLS];assert len(selected)==4,rows
    candidate=SOURCE
    for patch in sorted(selected,key=lambda p:p['start'],reverse=True):
        assert candidate[patch['start']:patch['end']]==patch['before'];candidate=candidate[:patch['start']]+patch['after']+candidate[patch['end']:]
    # Replace by method boundaries so two Int32 fixtures do not alter negative controls.
    control=SOURCE
    for row in rows:
        if row['method'] not in CONTROLS:continue
        p=row['patches'][0];before,after=CONTROLS[row['method']];assert before==p['before'].split(')')[0]+')' or p['before'].startswith(before)
    # A manually written declaration/initializer form supplies each control.
    for patch in sorted(selected,key=lambda p:p['start'],reverse=True):
        name=next(r['method'] for r in rows if r['patches'] and r['patches'][0] is patch);before,after=CONTROLS[name]
        replacement=patch['before'].replace(before,after,1);control=control[:patch['start']]+replacement+control[patch['end']:]
    recovered,_=compile_fixture(folder/'recovered',SOURCE,manifest);expected,types=compile_fixture(folder/'control',control,manifest);actual,_=compile_fixture(folder/'candidate',candidate,manifest)
    isolated=dict(manifest,sources=[{'path':str(path),'input_path':str(path)}]);mp=folder/'manifest.json';mp.write_text(json.dumps(isolated),encoding='utf-8')
    results=[]
    with SlotModelClient(mp) as model:
        for name in CONTROLS:
            identity=next(k for k in actual if k.startswith('Fixture::'+name+'('));delta,_=differences(expected[identity],actual[identity]);different,_=differences(expected[identity],recovered[identity]);prediction=model.predict(path,candidate,identity)
            results.append(dict(method=name,body_exact_against_handwritten_control=not delta,recovered_scope_differs=bool(different),differences=delta,model_complete=prediction['complete'],model_types=prediction['predicted_types'],expected_types=types[identity],model_exact=prediction['complete'] and prediction['predicted_types']==types[identity],candidate_instructions=actual[identity]['instructions'],control_instructions=expected[identity]['instructions']))
    report=dict(operation='independent-roslyn-for-declaration-fixture',rule_sha256=sha(RULE),model_sha256=sha(ROOT/'tools/Round7RoslynModel.cs'),compiler_sha256=manifest['compiler_sha256'],fixture_assembly_compilations=3,game_compilations=0,hint_required=True,rejected_shapes=rejected,fixtures=results)
    (ROOT/'reports/round7-roslyn-for-declaration-fixture.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8',newline='\n');print([(r['method'],r['body_exact_against_handwritten_control'],r['recovered_scope_differs'],r['model_exact']) for r in results])
    assert all(r['body_exact_against_handwritten_control'] and r['recovered_scope_differs'] and r['model_exact'] for r in results)


if __name__=='__main__':main()
