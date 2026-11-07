// Original-guided source population after storing a one-dimensional array local.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
public static partial class Round7TransformSyntax {
 static void AddRound9ArrayPopulationRules(JsonElement input,string text,StatementSyntax mapped,BlockSyntax methodBody,SemanticModel model,int point,List<Patch> patches,HashSet<string> seen){
  const string family="d9-array-local-before-population";
  string selected=input.TryGetProperty("round7_cycle_rule",out var r)?r.GetString():null;
  bool learned=input.TryGetProperty("round7_cycle_rules",out var rs)&&rs.ValueKind==JsonValueKind.Array&&rs.EnumerateArray().Any(x=>x.GetString()==family);
  if(selected!=family&&!learned)return;
  if(!input.TryGetProperty("compiler",out var c)||c.GetString()!="smcs")return;
  if(!input.TryGetProperty("round9_array_population_valid",out var valid)||valid.ValueKind!=JsonValueKind.True)return;
  var scope=mapped??(StatementSyntax)methodBody;
  foreach(var array in scope.DescendantNodesAndSelf(n=>!(n is AnonymousFunctionExpressionSyntax)&&!(n is LocalFunctionStatementSyntax)).OfType<ArrayCreationExpressionSyntax>()){
   if(array.Initializer==null||array.Type.RankSpecifiers.Count!=1||array.Type.RankSpecifiers[0].Sizes.Count!=1)continue;
   var values=array.Initializer.Expressions;if(values.Count==0||values.Any(v=>v.DescendantNodesAndSelf().Any(n=>n is AnonymousFunctionExpressionSyntax||n is AwaitExpressionSyntax)))continue;
   var rank=array.Type.RankSpecifiers[0];var size=rank.Sizes[0];
   if(!(size is OmittedArraySizeExpressionSyntax)){
    var value=model.GetConstantValue(size);if(!value.HasValue||!(value.Value is int length)||length!=values.Count)continue;
   }
   StatementSyntax statement=null;ILocalSymbol local=null;string name=null;
   if(array.Parent is EqualsValueClauseSyntax eq&&eq.Parent is VariableDeclaratorSyntax variable&&variable.Parent is VariableDeclarationSyntax declaration&&declaration.Variables.Count==1&&declaration.Parent is LocalDeclarationStatementSyntax ls){
    statement=ls;local=model.GetDeclaredSymbol(variable) as ILocalSymbol;name=variable.Identifier.Text;
   }else if(array.Parent is AssignmentExpressionSyntax assignment&&assignment.IsKind(SyntaxKind.SimpleAssignmentExpression)&&assignment.Right==array&&assignment.Left is IdentifierNameSyntax identifier&&assignment.Parent is ExpressionStatementSyntax es){
    statement=es;local=model.GetSymbolInfo(identifier).Symbol as ILocalSymbol;name=identifier.Identifier.Text;
   }
   if(statement==null||!(statement.Parent is BlockSyntax)||local==null||local.Type is not IArrayTypeSymbol type||type.Rank!=1)continue;
   // No alias lifetime, capture or self-reference is invented by this template.
   if(values.Any(v=>v.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>().Any(i=>SymbolEqualityComparer.Default.Equals(model.GetSymbolInfo(i).Symbol,local))))continue;
   if(type.ElementType.TypeKind==TypeKind.Struct&&type.ElementType.SpecialType==SpecialType.None)continue;
   if(type.ElementType is IArrayTypeSymbol)continue;
   if(model.AnalyzeDataFlow(statement).Captured.Any(s=>SymbolEqualityComparer.Default.Equals(s,local)))continue;
   var sized=rank.WithSizes(SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,SyntaxFactory.Literal(values.Count))));
   var created=array.WithInitializer(null).WithType(array.Type.WithRankSpecifiers(SyntaxFactory.SingletonList(sized)));
   string after=statement.ReplaceNode(array,created).ToString();
   int lineStart=text.LastIndexOf('\n',Math.Max(0,statement.SpanStart-1))+1;string indent=text.Substring(lineStart,statement.SpanStart-lineStart);if(indent.Any(ch=>ch!=' '&&ch!='\t'))indent="";
   for(int i=0;i<values.Count;i++)after+="\n"+indent+name+"["+i+"] = "+values[i].ToFullString().Trim()+";";
   string before=text.Substring(statement.SpanStart,statement.Span.Length),key=family+":"+statement.SpanStart;
   if(!seen.Add(key))continue;
   patches.Add(new Patch{rule=family,start=statement.SpanStart,end=statement.Span.End,before=before,after=after,distance=Math.Abs(point-statement.SpanStart)});
  }
 }
}
