"""Five case-specific Part B source/IL fixtures; never compiles game source."""
import hashlib,json,os,shutil,subprocess,sys
from pathlib import Path
ROOT=next(p for p in Path(__file__).resolve().parents if (p/'tools/compiler_probes.py').is_file())
sys.path.insert(0,str(ROOT/'tools'))
from compiler_probes import ROSLYN
from round7_roslyn_for_declaration_fixture import compile_fixture as compile_roslyn
from round4_symbols import inspect_methods
from compare_kairo import differences,sha
WORK=ROOT/'build/round9/part-b-fixtures'
RULE=ROOT/'tools/Round9PartBRules.cs'

HOST=r'''using System;using System.IO;using System.Linq;using System.Collections.Generic;using System.Text.Json;
using Microsoft.CodeAnalysis;using Microsoft.CodeAnalysis.CSharp;using Microsoft.CodeAnalysis.CSharp.Syntax;
public static partial class Round7TransformSyntax {
 sealed class Patch{public string rule{get;set;}public int start{get;set;}public int end{get;set;}public string before{get;set;}public string after{get;set;}public int distance{get;set;}}
 public static void Main(string[]args){string path=Path.GetFullPath(args[0]),text=File.ReadAllText(path);var tree=CSharpSyntaxTree.ParseText(text,path:path);var trees=new List<SyntaxTree>();var refs=new List<MetadataReference>();
 using(var doc=JsonDocument.Parse(File.ReadAllText(args[1]))){foreach(var s in doc.RootElement.GetProperty("sources").EnumerateArray()){string logical=Path.GetFullPath(s.GetProperty("path").GetString());string actual=s.TryGetProperty("input_path",out var a)?a.GetString():logical;trees.Add(logical.Equals(path,StringComparison.OrdinalIgnoreCase)?tree:CSharpSyntaxTree.ParseText(File.ReadAllText(actual),path:logical));}foreach(var r in doc.RootElement.GetProperty("references").EnumerateArray())refs.Add(MetadataReference.CreateFromFile(r.GetProperty("path").GetString()));}
 var compilation=CSharpCompilation.Create("PartB_NoEmit",trees,refs,new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,optimizationLevel:OptimizationLevel.Release));var sem=compilation.GetSemanticModel(tree);var rows=new List<object>();
 using(var request=JsonDocument.Parse(args.Length>2?JsonSerializer.Serialize(new Dictionary<string,string>{{"round7_cycle_rule",args[2]}}):"{}"))
 foreach(var method in tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>()){if(method.Body==null)continue;var symbol=sem.GetDeclaredSymbol(method);var patches=new List<Patch>();AddRound9PartBRules(request.RootElement,text,method.Body,method.Body,sem,method.Body.SpanStart,patches,new HashSet<string>());
 var owners=patches.Select(p=>{var node=tree.GetRoot().FindToken(p.start).Parent;var statements=node.AncestorsAndSelf().OfType<StatementSyntax>().ToArray();return new{rule=p.rule,start=p.start,end=p.end,lexical_statement_ancestors=statements.Select(s=>new{kind=s.Kind().ToString(),start=s.SpanStart,end=s.Span.End}).ToArray(),source_fragment=text.Substring(p.start,Math.Min(p.end-p.start,100))};}).ToArray();
 rows.Add(new{owner=symbol.ContainingType.ToDisplayString(),method=method.Identifier.ValueText,declaration_start=method.SpanStart,declaration_end=method.Span.End,patches=patches,scope_proofs=owners});}
 Console.WriteLine(JsonSerializer.Serialize(rows));}
}'''

SMCS_SOURCE=r'''namespace form {
 public class GameForm {
  object[] eventData_=new object[3];bool CheckAlreadyEvent(object e){return e==this;}bool CheckTermScript(object e){return e!=this;}void ExecEvent(object e){}
  public bool _update(int count){for(int i=0;i<eventData_.Length;i++){if(((eventData_[i]==null)||(CheckAlreadyEvent(eventData_[i])))||(!(CheckTermScript(eventData_[i])))){continue;}ExecEvent(eventData_[i]);return false;}return false;}
  public bool ExpectedUpdate(int count){for(int i=0;i<eventData_.Length;i++){if((eventData_[i]==null)||(CheckAlreadyEvent(eventData_[i]))){continue;}if(CheckTermScript(eventData_[i])){ExecEvent(eventData_[i]);return false;}}return false;}
 }
 public class SubForm {
  int type_;void Use(int n){}bool Init2(){return false;}
  public bool Init(){if(type_==3){Use(3);}else if(type_!=6){if(type_==7){Use(7);}else if(type_==8){return Init2();}}return true;}
  public bool ExpectedInit(){if(type_==3){Use(3);}else if(type_==6){}else{if(type_==7){Use(7);}else if(type_==8){return Init2();}}return true;}
  public bool DirectReturnInit(){if(type_==3){Use(3);}else if(type_==6){return true;}else{if(type_==7){Use(7);}else if(type_==8){return Init2();}}return true;}
 }
 public class SoccerForm {
  int select_;int stateCnt_;int[] PBONUS_NP_LV_POPULAR=new int[]{3,5};void Use(int n){}
  public void Draw_newPlayerPopular(object g){for(int i=0;i<3;i++){Use(0);if(select_==0){if(stateCnt_<PBONUS_NP_LV_POPULAR[0]){return;}if(stateCnt_<PBONUS_NP_LV_POPULAR[1]){Use(1);}else{Use(2);}}else{Use(3);}}Use(4);}
  public void ExpectedPopular(object g){for(int i=0;i<3;i++){Use(0);if(select_==0){if(stateCnt_<PBONUS_NP_LV_POPULAR[0]){}else{if(stateCnt_<PBONUS_NP_LV_POPULAR[1]){Use(1);}else{Use(2);}}}else{Use(3);}}Use(4);}
  public void DirectContinuePopular(object g){for(int i=0;i<3;i++){Use(0);if(select_==0){if(stateCnt_<PBONUS_NP_LV_POPULAR[0]){continue;}if(stateCnt_<PBONUS_NP_LV_POPULAR[1]){Use(1);}else{Use(2);}}else{Use(3);}}Use(4);}
 }
}'''

ROSLYN_SOURCE=r'''namespace kairo.unity.ui {
 public class Graphics {
  public object render_;public bool renderIsReplace_;public int blendMode_;public object renderForceMaterial_;public int localMatrixIndex_;public bool useFigureClip_,setFigureClip_;
  public bool IsGUI(){return render_==this;}public void Submit(int value){}public void Gui(float value){}
  public void _drawBitmap(bool flag){if(flag&&renderIsReplace_&&(blendMode_==0||blendMode_==3)&&renderForceMaterial_==null&&localMatrixIndex_==-1&&!useFigureClip_&&!setFigureClip_){Submit(3);}else{Submit(4);}}
  public void ExpectedBitmap(bool flag){bool expected=flag&&renderIsReplace_&&(blendMode_==0||blendMode_==3)&&renderForceMaterial_==null&&localMatrixIndex_==-1&&!useFigureClip_&&!setFigureClip_;if(expected){Submit(3);}else{Submit(4);}}
 }
 public class Balloon {
  string Text;string Normalize(string value){return value;}
  public void Draw(Graphics g,float x,float y,object matrix){Text=Normalize(Text);if(!g.IsGUI()){if(g.render_==null){return;}int num=60;int num2=20;g.Submit(num+num2);return;}float num6=x;float num7=y;g.Gui(num6+num7);}
  public void ExpectedDraw(Graphics g,float x,float y,object matrix){Text=Normalize(Text);if(!g.IsGUI()){if(g.render_!=null){int num=60;int num2=20;g.Submit(num+num2);return;}}else{float num6=x;float num7=y;g.Gui(num6+num7);}}
  public void DirectPositiveDraw(Graphics g,float x,float y,object matrix){Text=Normalize(Text);if(!g.IsGUI()){if(g.render_!=null){int num=60;int num2=20;g.Submit(num+num2);}return;}float num6=x;float num7=y;g.Gui(num6+num7);}
 }
}'''

CASES=[('smcs','b9-event-loop-split-guard','form.GameForm','_update','ExpectedUpdate'),
 ('smcs','b9-empty-arm-common-return','form.SubForm','Init','ExpectedInit'),
 ('smcs','b9-guard-return-empty-positive-arm','form.SoccerForm','Draw_newPlayerPopular','ExpectedPopular'),
 ('roslyn','b9-positive-render-with-gui-else','kairo.unity.ui.Balloon','Draw','ExpectedDraw'),
 ('roslyn','b9-boolean-value-before-if','kairo.unity.ui.Graphics','_drawBitmap','ExpectedBitmap')]

def ensure_host():
    identity=hashlib.sha256(HOST.encode()+RULE.read_bytes()).hexdigest();folder=WORK/'helper'/identity[:16];folder.mkdir(parents=True,exist_ok=True);target=folder/'PartBHost.dll'
    vs=next(p for p in ROSLYN.parents if (p/'dotnet').is_dir());runtime=vs/'dotnet/net10.0/runtime';framework=runtime/'shared/Microsoft.NETCore.App/10.0.10'
    refs=[framework/(n+'.dll') for n in ['System.Private.CoreLib','System.Runtime','System.Console','System.Linq','System.Collections','System.Text.Json','System.Collections.Immutable','System.Memory','netstandard']]
    refs += [ROSLYN.parent/(n+'.dll') for n in ['Microsoft.CodeAnalysis','Microsoft.CodeAnalysis.CSharp']]
    if not target.exists():
        host=folder/'Host.cs';host.write_text(HOST,encoding='utf-8');rsp=folder/'compile.rsp';args=['/nologo','/nostdlib+','/target:exe','/out:'+str(target)]+['/r:'+str(p) for p in refs]+[str(host),str(RULE)]
        rsp.write_text('\n'.join('"'+v+'"' for v in args)+'\n',encoding='utf-8')
        result=subprocess.run([str(ROSLYN),'/noconfig','@'+str(rsp)],capture_output=True);(folder/'compiler.log').write_bytes(result.stdout+result.stderr)
        if result.returncode:raise RuntimeError((result.stdout+result.stderr).decode(errors='replace'))
        for p in refs[-2:]:shutil.copy2(p,folder/p.name)
        target.with_suffix('.runtimeconfig.json').write_text(json.dumps({'runtimeOptions':{'tfm':'net10.0','framework':{'name':'Microsoft.NETCore.App','version':'10.0.10'}}}))
    return [str(runtime/'dotnet.exe'),str(target)]

def query(source_path,manifest,rule=None):
    command=ensure_host()+[str(source_path),str(manifest)]
    if rule:command.append(rule)
    result=subprocess.run(command,capture_output=True,check=True)
    return json.loads(result.stdout)

def apply(source,patch):
    raw=source.encode('utf-16-le');assert raw[2*patch['start']:2*patch['end']].decode('utf-16-le')==patch['before']
    return (raw[:2*patch['start']]+patch['after'].encode('utf-16-le')+raw[2*patch['end']:]).decode('utf-16-le')

def compile_smcs(folder,source,manifest):
    folder.mkdir(parents=True,exist_ok=True);path=folder/'Fixture.cs';path.write_text(source,encoding='utf-8',newline='\n');binary=folder/'Fixture.dll'
    flags=[f for f in manifest['flags'] if not f.startswith('-out:') and f!='-noconfig']
    command=[manifest['runtime'],manifest['compiler'],'-noconfig',*flags,'-out:'+str(binary)]
    command+=['-r:'+r['path'] for r in manifest['references']]+[str(path)]
    env=os.environ.copy();env.update(manifest['scoped_environment']);result=subprocess.run(command,capture_output=True,env=env);(folder/'compiler.log').write_bytes(result.stdout+result.stderr)
    if result.returncode:raise RuntimeError((result.stdout+result.stderr).decode(errors='replace'))
    return {m['stable_id']:m for m in inspect_methods(binary,symbols=False)['methods']}

def find(methods,owner,name):return next(v for k,v in methods.items() if k.startswith(owner+'::'+name+'('))
def body_difference(control,actual):
    # The independently named control has the same parameter/return types. Its
    # declaration name is the only facet outside the body comparison contract.
    delta,_=differences(control,actual)
    return [facet for facet in delta if facet!='signature']

def main():
    base=ROOT/'build/round9/cycles/a05/after';results=[];negative=[];compiler_count=0
    for compiler,assembly,source in [('smcs','Assembly-CSharp',SMCS_SOURCE),('roslyn','KairoLibrary',ROSLYN_SOURCE)]:
        folder=WORK/'fixtures'/compiler;folder.mkdir(parents=True,exist_ok=True);path=folder/'Input.cs';path.write_text(source,encoding='utf-8',newline='\n')
        manifest=json.loads((base/assembly/'build-manifest.json').read_text(encoding='utf-8-sig'))
        isolated=dict(manifest,sources=[dict(path=str(path),input_path=str(path))]);mp=folder/'manifest.json';mp.write_text(json.dumps(isolated))
        assert not any(r['patches'] for r in query(path,mp)), 'Opt-in is mandatory'
        candidate=source
        for family,rule,owner,name,expected in CASES:
            if family!=compiler:continue
            rows=query(path,mp,rule);patches=[p for row in rows for p in row['patches']]
            assert len(patches)==1,(rule,[(r['owner'],r['method'],len(r['patches'])) for r in rows])
        # Each toy method receives just one isolated change. Rebase subsequent
        # patches uniquely rather than carrying stale offsets from another method.
        candidate=source
        for family,rule,owner,name,expected in CASES:
            if family!=compiler:continue
            patch=next(p for row in query(path,mp,rule) for p in row['patches']);assert candidate.count(patch['before'])==1
            point=candidate.index(patch['before']);start=len(candidate[:point].encode('utf-16-le'))//2
            candidate=apply(candidate,dict(patch,start=start,end=start+len(patch['before'].encode('utf-16-le'))//2))
        if compiler=='smcs':before=compile_smcs(folder/'before',source,manifest);after=compile_smcs(folder/'candidate',candidate,manifest)
        else:before,_=compile_roslyn(folder/'before',source,manifest);after,_=compile_roslyn(folder/'candidate',candidate,manifest)
        compiler_count+=2
        for family,rule,owner,name,expected in CASES:
            if family!=compiler:continue
            actual=find(after,owner,name);control=find(after,owner,expected);recovered=find(before,owner,name)
            delta=body_difference(control,actual);old_delta=body_difference(control,recovered);assert not delta and old_delta,(rule,delta,old_delta)
            results.append(dict(compiler=compiler,rule=rule,full_body_differences=delta,recovered_body_differences=old_delta,
                actual=actual,control=control,recovered=recovered))
        for owner,wrong,expected in ([('form.SubForm','DirectReturnInit','ExpectedInit'),('form.SoccerForm','DirectContinuePopular','ExpectedPopular')] if compiler=='smcs' else [('kairo.unity.ui.Balloon','DirectPositiveDraw','ExpectedDraw')]):
            delta=body_difference(find(after,owner,expected),find(after,owner,wrong));assert delta,(wrong,expected)
            negative.append(dict(method=owner+'::'+wrong,control=expected,differences=delta))
        if compiler=='smcs':
            floating=source.replace('int stateCnt_;','float stateCnt_;');fp=folder/'Floating.cs';fp.write_text(floating,encoding='utf-8',newline='\n');fm=folder/'floating-manifest.json';fm.write_text(json.dumps(dict(manifest,sources=[dict(path=str(fp),input_path=str(fp))])))
            assert not any(r['patches'] for r in query(fp,fm,'b9-guard-return-empty-positive-arm'))
            negative.append(dict(method='SoccerForm floating comparison',rejected_without_operator_rewrite=True))
    report=dict(operation='five-explicit-PartB-historical-compiler-fixtures',helper_sha256=sha(RULE),driver_sha256=sha(Path(__file__)),
        historical_fixture_compilations=compiler_count,game_compilations=0,full_body_controls_equal=len(results),fixtures=results,negative_controls=negative)
    (ROOT/'reports/round9-part-b-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({k:v for k,v in report.items() if k not in ['fixtures','negative_controls']}))

if __name__=='__main__':main()
