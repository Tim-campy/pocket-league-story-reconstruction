"""Independent historical csc controls for three Single parameter occurrences."""
import hashlib,json,shutil,subprocess,sys
from pathlib import Path
ROOT=next(p for p in Path(__file__).resolve().parents if (p/'tools/compiler_probes.py').is_file())
sys.path.insert(0,str(ROOT/'tools'))
from compiler_probes import ROSLYN
from round7_roslyn_for_declaration_fixture import compile_fixture
from round6_alignment import align_methods
from compare_kairo import differences,sha
try:from round9_float_identity_hints import float_identity_conversion_hints
except ImportError:from hints import float_identity_conversion_hints
SCRIPT=Path(__file__).resolve().parent
WORK=ROOT/'build/round9/part-d-float';RULE=SCRIPT/'Round9FloatingIdentityRules.cs'
HOST=r'''using System;using System.IO;using System.Linq;using System.Collections.Generic;using System.Text.Json;
using Microsoft.CodeAnalysis;using Microsoft.CodeAnalysis.CSharp;using Microsoft.CodeAnalysis.CSharp.Syntax;
public static partial class Round7TransformSyntax {
 sealed class Patch{public string rule{get;set;}public int start{get;set;}public int end{get;set;}public string before{get;set;}public string after{get;set;}public int distance{get;set;}}
 public static void Main(string[]args){string path=Path.GetFullPath(args[0]),text=File.ReadAllText(path);var tree=CSharpSyntaxTree.ParseText(text,path:path);var refs=new List<MetadataReference>();var trees=new List<SyntaxTree>();
 using(var m=JsonDocument.Parse(File.ReadAllText(args[1]))){foreach(var s in m.RootElement.GetProperty("sources").EnumerateArray()){string logical=Path.GetFullPath(s.GetProperty("path").GetString()),actual=s.TryGetProperty("input_path",out var a)?a.GetString():logical;trees.Add(logical.Equals(path,StringComparison.OrdinalIgnoreCase)?tree:CSharpSyntaxTree.ParseText(File.ReadAllText(actual),path:logical));}foreach(var r in m.RootElement.GetProperty("references").EnumerateArray())refs.Add(MetadataReference.CreateFromFile(r.GetProperty("path").GetString()));}
 var compilation=CSharpCompilation.Create("NoEmitFloatOccurrence",trees,refs,new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,optimizationLevel:OptimizationLevel.Release));var sem=compilation.GetSemanticModel(tree);var rows=new List<object>();using(var request=JsonDocument.Parse(args.Length>2?args[2]:"{}")){
 foreach(var method in tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>()){if(method.Body==null)continue;if(request.RootElement.TryGetProperty("target_method",out var target)&&method.Identifier.ValueText!=target.GetString())continue;int point=request.RootElement.TryGetProperty("point",out var pp)?pp.GetInt32():method.Body.SpanStart;StatementSyntax mapped=method.Body;if(method.Body.Span.Contains(point)){var node=tree.GetRoot().FindToken(point).Parent;mapped=node.AncestorsAndSelf().OfType<StatementSyntax>().FirstOrDefault(s=>method.Body.Span.Contains(s.Span))??method.Body;}var patches=new List<Patch>();AddRound9FloatingIdentityRules(request.RootElement,text,mapped,method.Body,sem,point,patches,new HashSet<string>());rows.Add(new{method=method.Identifier.ValueText,patches=patches,declaration_start=method.SpanStart,declaration_end=method.Span.End});}}
 Console.WriteLine(JsonSerializer.Serialize(rows));}
}'''
SOURCE='''// UTF-16 control: 🔎
public class Fixture {
 public static float StaticFirst(float v0,float v1,float length,float frame,float edge){if(frame<0f)return v0;if(frame>=length)return v1;float ratio=frame/length;return v0+(v1-v0)*ratio;}
 public static float ExpectedStatic(float v0,float v1,float length,float frame,float edge){if(frame<0f)return v0;if(frame>=length)return v1;float ratio=(float)frame/length;return v0+(v1-v0)*ratio;}
 public float Instance(float width,float swidth,float height,float sheight){float x=width/swidth;float y=height/sheight;return x+y;}
 public float ExpectedWidth(float width,float swidth,float height,float sheight){float x=(float)width/swidth;float y=height/sheight;return x+y;}
 public float ExpectedBoth(float width,float swidth,float height,float sheight){float x=(float)width/swidth;float y=(float)height/sheight;return x+y;}
 public static float Already(float frame,float length){return (float)frame/length;}
 public static double DoubleCase(double frame,double length){return frame/length;}
 public static float Constant(float length){return (float)1f/length;}
 public static float IntCase(int frame,float length){return (float)frame/length;}
}
'''.replace('\n','\r\n')

def ensure_host():
    digest=hashlib.sha256(RULE.read_bytes()+HOST.encode()).hexdigest();folder=WORK/'helper'/digest[:16];folder.mkdir(parents=True,exist_ok=True);target=folder/'FloatHost.dll'
    vs=next(p for p in ROSLYN.parents if (p/'dotnet').is_dir());runtime=vs/'dotnet/net10.0/runtime';framework=runtime/'shared/Microsoft.NETCore.App/10.0.10'
    refs=[framework/(n+'.dll') for n in ['System.Private.CoreLib','System.Runtime','System.Console','System.Linq','System.Collections','System.Text.Json','System.Collections.Immutable','System.Memory','netstandard']]+[ROSLYN.parent/(n+'.dll') for n in ['Microsoft.CodeAnalysis','Microsoft.CodeAnalysis.CSharp']]
    if not target.exists():
        host=folder/'Host.cs';host.write_text(HOST,encoding='utf-8');rsp=folder/'compile.rsp';args=['/nologo','/nostdlib+','/target:exe','/out:'+str(target)]+['/r:'+str(p) for p in refs]+[str(host),str(RULE)];rsp.write_text('\n'.join('"'+a+'"' for a in args)+'\n',encoding='utf-8');result=subprocess.run([str(ROSLYN),'/noconfig','@'+str(rsp)],capture_output=True)
        if result.returncode:raise RuntimeError((result.stdout+result.stderr).decode(errors='replace'))
        for p in refs[-2:]:shutil.copy2(p,folder/p.name)
        target.with_suffix('.runtimeconfig.json').write_text(json.dumps({'runtimeOptions':{'tfm':'net10.0','framework':{'name':'Microsoft.NETCore.App','version':'10.0.10'}}}))
    return [str(runtime/'dotnet.exe'),str(target)]

def query(path,manifest,request):
    return json.loads(subprocess.run(ensure_host()+[str(path),str(manifest),json.dumps(request)],capture_output=True,check=True).stdout)
def apply(source,patch):
    raw=source.encode('utf-16-le');assert raw[patch['start']*2:patch['end']*2].decode('utf-16-le')==patch['before'];return(raw[:patch['start']*2]+patch['after'].encode('utf-16-le')+raw[patch['end']*2:]).decode('utf-16-le')
def method(rows,name):return next(m for k,m in rows.items() if k.startswith('Fixture::'+name+'('))
def body_diff(a,b):return [d for d in differences(a,b)[0] if d!='signature']

def main():
    manifest=json.loads((ROOT/'build/round9/cycles/a05/after/KairoLibrary/build-manifest.json').read_text(encoding='utf-8-sig'))
    folder=WORK/'fixtures';folder.mkdir(parents=True,exist_ok=True);path=folder/'Input.cs';mp=folder/'manifest.json'
    mp.write_text(json.dumps(dict(manifest,sources=[dict(path=str(path),input_path=str(path))])))
    baseline,_=compile_fixture(folder/'baseline',SOURCE,manifest);current=SOURCE;checks=[];case_dumps=[]
    for name,control,needle in [('StaticFirst','ExpectedStatic','frame/length'),('Instance','ExpectedWidth','width/swidth'),('Instance','ExpectedBoth','height/sheight')]:
        actual_method=method(baseline if not case_dumps else case_dumps[-1],name);reference=method(baseline,control)
        aligned=align_methods(reference,actual_method);hints=float_identity_conversion_hints(reference,actual_method,aligned['original_offset'],aligned['rebuilt_offset'])
        assert hints.get('round9_identity_conversion_opcode')=='conv.r4',(name,hints,aligned)
        path.write_bytes(current.encode('utf-8'));start=current.index(' '+name+'(');position=current.index(needle,start);point=len(current[:position].encode('utf-16-le'))//2
        request=dict(hints,round7_cycle_rule='d9-explicit-float-identity-parameter',target_method=name,point=point)
        patches=[p for row in query(path,mp,request) for p in row['patches']];assert len(patches)==1,(name,patches)
        current=apply(current,patches[0]);dumps,_=compile_fixture(folder/('step%d'%len(checks)),current,manifest);case_dumps.append(dumps)
        delta=body_diff(method(dumps,name),reference);assert not delta,(name,delta)
        checks.append(dict(method=name,control=control,hints=hints,patch=patches[0],body_differences=delta,actual=method(dumps,name),expected=reference))
    # One baseline plus three one-occurrence steps; no runtime/game execution.
    path.write_bytes(current.encode('utf-8'))
    nohint=query(path,mp,dict(round7_cycle_rule='d9-explicit-float-identity-parameter'))
    assert not any(r['patches'] for r in nohint)
    for name,ordinal in [('Already',0),('DoubleCase',0),('IntCase',0)]:
        rows=query(path,mp,dict(round7_cycle_rule='d9-explicit-float-identity-parameter',round9_identity_conversion_opcode='conv.r4',round9_identity_parameter_ordinal=ordinal,target_method=name))
        assert not any(r['patches'] for r in rows),name
    constant=method(baseline,'Constant');assert not any(i['opcode']=='conv.r4' for i in constant['instructions'])
    constant_hints=float_identity_conversion_hints(constant,constant,0,0);assert not constant_hints
    assert not any(r['patches'] for r in query(path,mp,dict(constant_hints,round7_cycle_rule='d9-explicit-float-identity-parameter',target_method='Constant')))
    int_control=method(baseline,'IntCase');position=next(i['offset'] for i in int_control['instructions'] if i['opcode']=='conv.r4')
    assert not float_identity_conversion_hints(int_control,method(baseline,'StaticFirst'),position,0)
    result=dict(operation='historical-csc-Single-identity-cast-occurrence-fixtures',helper_sha256=sha(RULE),hints_sha256=sha(SCRIPT/'round9_float_identity_hints.py'),fixture_driver_sha256=sha(Path(__file__)),compiler_sha256=manifest['compiler_sha256'],
        historical_fixture_compilations=4,game_compilations=0,full_body_controls_equal=3,fixtures=checks,
        negatives=['no original-conversion evidence','existing Single cast','Double parameter','constant expression','Int32 numeric conversion'],
        original_unordered_float_branch_opcodes_preserved=True)
    (WORK/'fixture-results.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in result.items() if k!='fixtures'}))

if __name__=='__main__':main()
