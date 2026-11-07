using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
public static partial class Round7TransformSyntax {
 static void AddRound9BooleanContextRules(JsonElement input,string text,StatementSyntax mapped,BlockSyntax methodBody,SemanticModel model,int point,List<Patch> patches,HashSet<string> seen){
  const string family="d9-boolean-value-branch-context";
  bool selected=input.TryGetProperty("round7_cycle_rule",out var r)&&r.GetString()==family;
  bool learned=input.TryGetProperty("round7_cycle_rules",out var rs)&&rs.ValueKind==JsonValueKind.Array&&rs.EnumerateArray().Any(x=>x.GetString()==family);
  if(!selected&&!learned)return;
  if(!input.TryGetProperty("compiler",out var c)||c.GetString()!="roslyn"||!input.TryGetProperty("round9_boolean_context_mode",out var mm))return;
  string mode=mm.GetString();var scope=mapped??(StatementSyntax)methodBody;
  var candidates=scope.DescendantNodesAndSelf(n=>!(n is AnonymousFunctionExpressionSyntax)&&!(n is LocalFunctionStatementSyntax)).OfType<IfStatementSyntax>().Concat(scope.Ancestors().OfType<IfStatementSyntax>());
  if(mode=="inline-adjacent-alias"&&scope is LocalDeclarationStatementSyntax mappedDeclaration&&mappedDeclaration.Parent is BlockSyntax mappedBlock){
   int at=mappedBlock.Statements.IndexOf(mappedDeclaration);if(at>=0&&at+1<mappedBlock.Statements.Count&&mappedBlock.Statements[at+1] is IfStatementSyntax adjacent)candidates=candidates.Concat(new[]{adjacent});
  }
  foreach(var branch in candidates.Distinct()){
   if(!methodBody.Span.Contains(branch.Span))continue;
   int start,end;string after;
   if(mode=="materialize-tail"){
    var condition=branch.Condition;while(condition is ParenthesizedExpressionSyntax p)condition=p.Expression;
    if(condition is not BinaryExpressionSyntax binary||!binary.IsKind(SyntaxKind.LogicalAndExpression))continue;
    if(model.GetTypeInfo(binary.Left).Type?.SpecialType!=SpecialType.System_Boolean||model.GetTypeInfo(binary.Right).Type?.SpecialType!=SpecialType.System_Boolean)continue;
    // Only the outer Boolean && is restructured. Inner calls/comparisons,
    // including overloaded equality, remain textually and semantically intact.
    if(model.GetSymbolInfo(binary).Symbol is IMethodSymbol op&&op.MethodKind==MethodKind.UserDefinedOperator)continue;
    start=branch.Condition.SpanStart;end=branch.Condition.Span.End;
    after="!("+binary.Left+") ? false : ("+binary.Right+")";
   }else if(mode=="inline-adjacent-alias"){
    if(branch.Parent is not BlockSyntax block||branch.Condition is not IdentifierNameSyntax identifier)continue;
    int index=block.Statements.IndexOf(branch);if(index<1||block.Statements[index-1] is not LocalDeclarationStatementSyntax declaration||declaration.Declaration.Variables.Count!=1)continue;
    var variable=declaration.Declaration.Variables[0];if(variable.Initializer==null)continue;
    var local=model.GetDeclaredSymbol(variable) as ILocalSymbol;
    if(local==null||local.Type.SpecialType!=SpecialType.System_Boolean||!SymbolEqualityComparer.Default.Equals(model.GetSymbolInfo(identifier).Symbol,local))continue;
    var refs=methodBody.DescendantNodes().OfType<IdentifierNameSyntax>().Where(i=>SymbolEqualityComparer.Default.Equals(model.GetSymbolInfo(i).Symbol,local)).ToArray();if(refs.Length!=1||refs[0]!=identifier)continue;
    if(variable.Initializer.Value.DescendantNodesAndSelf().Any(n=>n is AnonymousFunctionExpressionSyntax||n is AwaitExpressionSyntax)||declaration.ContainsDirectives||branch.ContainsDirectives)continue;
    start=declaration.SpanStart;end=branch.Span.End;
    after=branch.WithCondition(SyntaxFactory.ParenthesizedExpression(variable.Initializer.Value.WithoutTrivia())).ToString();
   }else continue;
   string before=text.Substring(start,end-start);if(!seen.Add(family+":"+start+":"+after))continue;
   patches.Add(new Patch{rule=family,start=start,end=end,before=before,after=after,distance=Math.Abs(start-point)});
  }
 }
}
