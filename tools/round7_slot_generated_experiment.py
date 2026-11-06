"""Isolated source-only experiment: predict smcs direct anonymous bodies.

Does not modify the frozen Round7SlotModel helper/client or compile game code.
"""
import json
from pathlib import Path
import shutil
import subprocess
from compiler_probes import ROSLYN
from compare_kairo import sha
from round7_slot_model import ensure_helper

ROOT=Path(__file__).resolve().parents[1]
INJECTION=r'''
    static List<(SyntaxTree tree,AnonymousFunctionExpressionSyntax lambda)> OrderedLambdas(string path,SyntaxTree replacement,CSharpCompilation compilation) {
        var types=new Dictionary<string,(SyntaxTree tree,TypeDeclarationSyntax node,INamedTypeSymbol symbol)>();
        foreach(var pair in Trees) {
            var tree=string.Equals(pair.Key,path,StringComparison.OrdinalIgnoreCase)?replacement:pair.Value;var sem=compilation.GetSemanticModel(tree);
            foreach(var type in tree.GetRoot().DescendantNodes().OfType<TypeDeclarationSyntax>()) {var symbol=sem.GetDeclaredSymbol(type) as INamedTypeSymbol;if(symbol!=null)types[TypeName(symbol)]=(tree,type,symbol);}
        }
        var result=new List<(SyntaxTree tree,AnonymousFunctionExpressionSyntax lambda)>();var visited=new HashSet<string>();
        Action<string> visit=null;visit=name=> {
            if(!visited.Add(name)||!types.TryGetValue(name,out var type))return;
            if(type.symbol.BaseType!=null)visit(TypeName(type.symbol.BaseType));
            foreach(var lambda in type.node.DescendantNodes().OfType<AnonymousFunctionExpressionSyntax>().Where(l=>l.Ancestors().OfType<TypeDeclarationSyntax>().FirstOrDefault()==type.node))result.Add((type.tree,lambda));
        };
        foreach(var name in types.Keys)visit(name);return result;
    }
    static IMethodSymbol AnonymousSymbol(AnonymousFunctionExpressionSyntax lambda,SemanticModel sem) {
        var converted=sem.GetTypeInfo(lambda).ConvertedType as INamedTypeSymbol;
        if(converted is IErrorTypeSymbol)converted=sem.Compilation.GetTypeByMetadataName(TypeName(converted));
        var convertedInvoke=converted?.GetMembers("Invoke").OfType<IMethodSymbol>().FirstOrDefault();if(convertedInvoke!=null)return convertedInvoke;
        var symbol=sem.GetSymbolInfo(lambda).Symbol as IMethodSymbol;
        var argument=lambda.Parent as ArgumentSyntax;var call=argument?.Parent?.Parent;
        if(!(call is InvocationExpressionSyntax)&&!(call is ObjectCreationExpressionSyntax))return symbol;
        int index=((ArgumentListSyntax)argument.Parent).Arguments.IndexOf(argument);var info=sem.GetSymbolInfo(call);
        var constructors=call is ObjectCreationExpressionSyntax created?(sem.GetTypeInfo(created).Type as INamedTypeSymbol)?.InstanceConstructors.AsEnumerable():Enumerable.Empty<IMethodSymbol>();
        var invokes=new List<IMethodSymbol>();
        foreach(var candidate in info.CandidateSymbols.OfType<IMethodSymbol>().Concat(new[]{info.Symbol as IMethodSymbol}).Concat(constructors??Enumerable.Empty<IMethodSymbol>()).Where(m=>m!=null)) {
            var type=index<candidate.Parameters.Length?candidate.Parameters[index].Type as INamedTypeSymbol:null;
            if(type is IErrorTypeSymbol)type=sem.Compilation.GetTypeByMetadataName(TypeName(type));
            if(type!=null)for(var parent=type.BaseType;parent!=null;parent=parent.BaseType)if(TypeName(parent)=="System.MulticastDelegate")invokes.AddRange(type.GetMembers("Invoke").OfType<IMethodSymbol>());
        }
        bool valueReturned=lambda.Body.DescendantNodesAndSelf().OfType<ReturnStatementSyntax>().Any(r=>r.Expression!=null);
        var distinct=invokes.Where(m=>m.ReturnsVoid!=valueReturned).GroupBy(m=>Method(m)).Select(g=>g.First()).ToArray();return distinct.Length==1?distinct[0]:null;
    }
    static List<object> GeneratedModels(string path,SyntaxTree tree,CSharpCompilation compilation,string wanted,List<CaptureScope> captures) {
        int ordinal=0;var output=new List<object>();
        foreach(var entry in OrderedLambdas(path,tree,compilation)) {
            var active=entry.tree;
            var sem=compilation.GetSemanticModel(active);
            {
                var lambda=entry.lambda;int number=ordinal++;if(!string.Equals(active.FilePath,path,StringComparison.OrdinalIgnoreCase))continue;
                var host=lambda.Ancestors().FirstOrDefault(n=>n is BaseMethodDeclarationSyntax||n is AccessorDeclarationSyntax);
                var hostSymbol=host==null?null:sem.GetDeclaredSymbol(host) as IMethodSymbol;if(hostSymbol==null)continue;
                // This experiment supports only methods hosted directly in the type,
                // never storey-hosted, nested, iterator or expression-bodied lambdas.
                if(captures.Any(c=>c.method==Method(hostSymbol))||lambda.Ancestors().OfType<AnonymousFunctionExpressionSyntax>().Any()||lambda.Body is not BlockSyntax body)continue;
                if(body.DescendantNodes().OfType<YieldStatementSyntax>().Any()||body.DescendantNodes().OfType<AnonymousFunctionExpressionSyntax>().Any())continue;
                var symbol=AnonymousSymbol(lambda,sem);if(symbol==null)continue;
                string id=TypeName(hostSymbol.ContainingType)+"::<"+hostSymbol.MetadataName+">m__"+number.ToString("X")+"("+string.Join(",",symbol.Parameters.Select(p=>Key(p.Type)+(p.RefKind==RefKind.None?"":"&")))+")->"+Key(symbol.ReturnType);
                if(wanted!=null&&wanted!=id)continue;
                var result=JsonSerializer.Deserialize<Dictionary<string,JsonElement>>(JsonSerializer.Serialize(new Model(sem,lambda,body,symbol,new List<CaptureScope>()).Run()));
                result["method_id"]=JsonSerializer.SerializeToElement(id);
                result["generated_source_method"]=JsonSerializer.SerializeToElement(Method(hostSymbol));
                result["generated_mapping"]=JsonSerializer.SerializeToElement("source-declaration-global-anonymous-ordinal");
                output.Add(result);
            }
        }
        return output;
    }
'''


def main():
    folder=ROOT/'build/round7/slot-generated-experiment'
    folder.mkdir(parents=True,exist_ok=True)
    helper_command=ensure_helper()
    helper=Path(helper_command[-1])
    runtime=Path(helper_command[0]).parent
    framework=runtime/'shared/Microsoft.NETCore.App/10.0.10'
    text=(ROOT/'tools/Round7SlotModel.cs').read_text(encoding='utf-8-sig')
    integrated='static List<object> GeneratedModels' in text
    if not integrated:text=text.replace('    static object Respond(JsonElement input){',INJECTION+'\n    static object Respond(JsonElement input){')
    text=text.replace('if(x is ObjectCreationExpressionSyntax o && o.Initializer!=null)Errors.Add("object_or_collection_initializer");',
                     'if(x is ObjectCreationExpressionSyntax o && o.Initializer!=null && (!o.Initializer.IsKind(SyntaxKind.ObjectInitializerExpression)||o.Initializer.Expressions.Any(v=>v is not AssignmentExpressionSyntax a||a.Right is InitializerExpressionSyntax)))Errors.Add("unsupported_initializer");')
    if not integrated:text=text.replace('            if(e is ObjectCreationExpressionSyntax creation && creation.ArgumentList?.Arguments.Count==0',r'''
            if(e is ObjectCreationExpressionSyntax initialized && initialized.Initializer!=null) {
                if(initialized.ArgumentList!=null)foreach(var arg in initialized.ArgumentList.Arguments)EmitExpression(arg.Expression,true);
                string type=Type(initialized);int slot=Alloc(type,"$object-initializer",initialized);
                foreach(var item in initialized.Initializer.Expressions)if(item is AssignmentExpressionSyntax initializedMember)EmitExpression(initializedMember.Right,true);
                Release(type,slot);return;
            }
            if(e is ObjectCreationExpressionSyntax creation && creation.ArgumentList?.Arguments.Count==0''')
    if not integrated:text=text.replace('        return new{methods=output,source_only=true,candidate_compiled=false};',
                      '        if(wanted==null||wanted.Contains(">m__"))output.AddRange(GeneratedModels(path,tree,compilation,wanted,captures));\n        return new{methods=output,source_only=true,candidate_compiled=false};')
    source=folder/'Round7SlotGenerated.cs'
    source.write_text(text,encoding='utf-8',newline='\n')
    binary=folder/'Round7SlotGenerated.dll'
    libraries=['System.Private.CoreLib','System.Runtime','System.Console','System.Linq','System.Collections','System.Text.Json','System.Reflection.Metadata','System.Collections.Immutable','System.Memory','netstandard']
    refs=[framework/(n+'.dll') for n in libraries]+[helper.parent/(n+'.dll') for n in ['Microsoft.CodeAnalysis','Microsoft.CodeAnalysis.CSharp']]
    rsp=folder/'compile.rsp'
    args=['/nologo','/nostdlib+','/target:exe','/out:'+str(binary)]+['/r:'+str(p) for p in refs]+[str(source)]
    rsp.write_text('\n'.join('"'+a+'"' for a in args),encoding='utf-8')
    subprocess.run([str(ROSLYN),'/noconfig','@'+str(rsp)],check=True)
    for p in refs[-2:]:shutil.copyfile(p,folder/p.name)
    shutil.copyfile(helper.with_suffix('.runtimeconfig.json'),binary.with_suffix('.runtimeconfig.json'))
    baseline=ROOT/'build/round7/baseline/Assembly-CSharp'
    manifest=json.loads((baseline/'build-manifest.json').read_text(encoding='utf-8-sig'))
    # Source files are pinned to the frozen baseline, preserving logical identities.
    for item in manifest['sources']:item['input_path']=str(baseline/item['path'])
    mp=folder/'manifest.json'
    mp.write_text(json.dumps(manifest),encoding='utf-8')
    process=subprocess.Popen([str(runtime/'dotnet.exe'),str(binary),str(mp)],stdin=subprocess.PIPE,stdout=subprocess.PIPE,stderr=subprocess.PIPE,text=True,encoding='utf-8')
    models={}
    try:
        for item in manifest['sources']:
            process.stdin.write(json.dumps({'path':str(ROOT/item['path'])})+'\n')
            process.stdin.flush()
            response=process.stdout.readline()
            if not response:raise RuntimeError(process.stderr.read())
            result=json.loads(response)
            if 'error' in result:raise RuntimeError(result)
            for row in result['methods']:
                if row.get('generated_mapping'):models[row['method_id']]=row
    finally:
        process.stdin.close();process.wait(timeout=10)
    actual=json.loads((baseline/'plain-local-types-rebuilt.json').read_text(encoding='utf-8-sig'))
    receipt=json.loads((baseline/'verification.json').read_text(encoding='utf-8-sig'))
    proofs={p['stable_id']:p for p in receipt['methods'].values()}
    rows=[]
    for identity,row in models.items():
        expected=actual.get(identity)
        rows.append(dict(row,actual_types=expected,prediction_equal=expected is not None and row['predicted_types']==expected,state=proofs.get(identity,{}).get('state','NOT_FOUND')))
    result={'operation':'source-only-generated-smcs-model-experiment','candidate_game_compilations':0,
            'model_source_sha256':sha(source),'rows':rows,'generated_models':len(rows),
            'mapped_generated_models':sum(r['actual_types'] is not None for r in rows),
            'complete':sum(r['complete'] for r in rows),
            'complete_exact':sum(r['complete'] and r['prediction_equal'] for r in rows)}
    (ROOT/'reports/round7-slot-generated-experiment.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8',newline='\n')
    print(json.dumps({k:v for k,v in result.items() if k!='rows'}))
    print(json.dumps([{'method_id':r['method_id'],'complete':r['complete'],'predicted_types':r['predicted_types'],'actual_types':r['actual_types'],'unsupported_reasons':r['unsupported_reasons']} for r in rows if not r['prediction_equal'] or 'm__17(' in r['method_id']]))


if __name__=='__main__':main()
