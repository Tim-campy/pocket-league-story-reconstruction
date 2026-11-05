// Read-only source census. Correlations are hypotheses, never matching proofs.
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
public static class SmcsLocalSyntax {
 static string Name(INamedTypeSymbol n)=>n.ContainingType!=null?Name(n.ContainingType)+"/"+n.MetadataName:(n.ContainingNamespace.IsGlobalNamespace?"":n.ContainingNamespace+".")+n.MetadataName;
 static string Key(ITypeSymbol t){
  if(t is ITypeParameterSymbol p)return(p.TypeParameterKind==TypeParameterKind.Method?"!!":"!")+p.Ordinal;
  if(t is IArrayTypeSymbol a)return Key(a.ElementType)+(a.Rank==1?"[]":"["+new string(',',a.Rank-1)+"]");
  if(t is IPointerTypeSymbol ptr)return Key(ptr.PointedAtType)+"*";
  if(t is INamedTypeSymbol n)return Name(n.OriginalDefinition)+(n.TypeArguments.Length>0?"<"+string.Join(",",n.TypeArguments.Select(Key))+">":"");
  return "UNKNOWN:"+t;
 }
 static string Method(IMethodSymbol m)=>Name(m.ContainingType)+"::"+m.MetadataName+(m.Arity==0?"":"``"+m.Arity)+"("+string.Join(",",m.Parameters.Select(p=>Key(p.Type)+(p.RefKind==RefKind.None?"":"&")))+")->"+Key(m.ReturnType);
 public static void Main(string[] args){
  var input=JsonDocument.Parse(File.ReadAllText(args[0])).RootElement;
  var trees=input.GetProperty("sources").EnumerateArray().Select(p=>CSharpSyntaxTree.ParseText(File.ReadAllText(p.GetString()),path:p.GetString())).ToArray();
  var refs=input.GetProperty("references").EnumerateArray().Select(p=>MetadataReference.CreateFromFile(p.GetString())).ToArray();
  var c=CSharpCompilation.Create("Recovered",trees,refs,new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
  var output=new List<object>();
  foreach(var tree in trees){var model=c.GetSemanticModel(tree,true);
   foreach(var node in tree.GetRoot().DescendantNodes().Where(n=>n is BaseMethodDeclarationSyntax||n is AccessorDeclarationSyntax)){
    var symbol=model.GetDeclaredSymbol(node) as IMethodSymbol;if(symbol==null)continue;
    BlockSyntax body=node is BaseMethodDeclarationSyntax m?m.Body:((AccessorDeclarationSyntax)node).Body;if(body==null)continue;
    var nodes=body.DescendantNodes().Where(n=>!n.Ancestors().TakeWhile(a=>a!=body).Any(a=>a is AnonymousFunctionExpressionSyntax)).ToArray();
    var locals=new List<object>();
    Action<SyntaxNode,string> add=(n,kind)=>{
     var l=model.GetDeclaredSymbol(n) as ILocalSymbol;
     if(l!=null)locals.Add(new{kind,name=l.Name,type=Key(l.Type),start=n.SpanStart});
    };
    foreach(var n in nodes.OfType<VariableDeclaratorSyntax>())add(n,"declared");
    foreach(var n in nodes.OfType<CatchDeclarationSyntax>())if(!n.Identifier.IsKind(SyntaxKind.None))add(n,"catch");
    foreach(var n in nodes.OfType<ForEachStatementSyntax>())add(n,"foreach");
    var switches=nodes.OfType<SwitchStatementSyntax>().Select(s=>new{expression=s.Expression.ToString(),
     expression_kind=s.Expression.Kind().ToString(),symbol_kind=model.GetSymbolInfo(s.Expression).Symbol?.Kind.ToString(),
     sections=s.Sections.Count,labels=s.Sections.SelectMany(x=>x.Labels).Select(x=>x.ToString()).ToArray(),
     has_goto=s.DescendantNodes().OfType<GotoStatementSyntax>().Any(),start=s.SpanStart,end=s.Span.End}).ToArray();
    var constructs=new{
     lambda_count=body.DescendantNodes().OfType<AnonymousFunctionExpressionSyntax>().Count(),
     foreach_count=nodes.OfType<ForEachStatementSyntax>().Count(),using_count=nodes.OfType<UsingStatementSyntax>().Count(),
     lock_count=nodes.OfType<LockStatementSyntax>().Count(),switch_count=switches.Length,
     catch_count=nodes.OfType<CatchDeclarationSyntax>().Count(),
     return_in_try_count=nodes.OfType<ReturnStatementSyntax>().Count(r=>r.Expression!=null&&r.Ancestors().TakeWhile(a=>a!=body).Any(a=>a is TryStatementSyntax)),
     postincrement_count=nodes.Count(n=>n.IsKind(SyntaxKind.PostIncrementExpression)||n.IsKind(SyntaxKind.PostDecrementExpression)),
     preincrement_count=nodes.Count(n=>n.IsKind(SyntaxKind.PreIncrementExpression)||n.IsKind(SyntaxKind.PreDecrementExpression)),
     compound_assignment_count=nodes.OfType<AssignmentExpressionSyntax>().Count(n=>!n.IsKind(SyntaxKind.SimpleAssignmentExpression)),
     ref_out_argument_count=nodes.OfType<ArgumentSyntax>().Count(n=>!n.RefKindKeyword.IsKind(SyntaxKind.None)),
     shortcircuit_count=nodes.Count(n=>n.IsKind(SyntaxKind.LogicalAndExpression)||n.IsKind(SyntaxKind.LogicalOrExpression)),
     ternary_count=nodes.OfType<ConditionalExpressionSyntax>().Count(),null_coalesce_count=nodes.Count(n=>n.IsKind(SyntaxKind.CoalesceExpression))};
    output.Add(new{id=Method(symbol),path=tree.FilePath,return_type=Key(symbol.ReturnType),locals,constructs,switches});
   }
  }
  Console.WriteLine(JsonSerializer.Serialize(new{methods=output,semantic_errors=c.GetDiagnostics().Where(d=>d.Severity==DiagnosticSeverity.Error).Take(8).Select(d=>d.ToString()).ToArray()}));
 }
}
