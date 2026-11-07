using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
public static partial class Round7TransformSyntax {
 static bool Round9DivisionReceiver(ExpressionSyntax expression,SemanticModel model){
  while(expression is ParenthesizedExpressionSyntax p)expression=p.Expression;
  if(expression is ThisExpressionSyntax||expression is BaseExpressionSyntax)return true;
  if(expression is IdentifierNameSyntax){var symbol=model.GetSymbolInfo(expression).Symbol;return symbol is ILocalSymbol l&&!l.IsConst||symbol is IParameterSymbol a&&a.RefKind==RefKind.None||symbol is IFieldSymbol f&&!f.IsVolatile||symbol is INamedTypeSymbol;}
  if(expression is MemberAccessExpressionSyntax m){var f=model.GetSymbolInfo(m).Symbol as IFieldSymbol;return f!=null&&!f.IsVolatile&&!f.Type.IsValueType&&(f.IsStatic||Round9DivisionReceiver(m.Expression,model));}
  return false;
 }
 static void AddRound9FieldDivisionRules(JsonElement input,string text,StatementSyntax mapped,BlockSyntax methodBody,SemanticModel model,int point,List<Patch> patches,HashSet<string> seen){
  const string family="d9-explicit-field-division";
  bool selected=input.TryGetProperty("round7_cycle_rule",out var r)&&r.GetString()==family;
  bool learned=input.TryGetProperty("round7_cycle_rules",out var rs)&&rs.ValueKind==JsonValueKind.Array&&rs.EnumerateArray().Any(x=>x.GetString()==family);
  if(!selected&&!learned)return;
  if(!input.TryGetProperty("compiler",out var c)||c.GetString()!="smcs"||!input.TryGetProperty("round9_division_repeated_receiver",out var hint)||hint.ValueKind!=JsonValueKind.True)return;
  foreach(var assignment in (mapped??(StatementSyntax)methodBody).DescendantNodesAndSelf(n=>!(n is AnonymousFunctionExpressionSyntax)&&!(n is LocalFunctionStatementSyntax)).OfType<AssignmentExpressionSyntax>()){
   if(!assignment.IsKind(SyntaxKind.DivideAssignmentExpression)||!(assignment.Parent is ExpressionStatementSyntax))continue;
   var field=model.GetSymbolInfo(assignment.Left).Symbol as IFieldSymbol;
   if(field==null||field.IsVolatile||field.IsReadOnly||field.IsConst||field.ContainingType.IsValueType)continue;
   var type=field.Type.SpecialType;if(type!=SpecialType.System_Int32&&type!=SpecialType.System_Int64&&type!=SpecialType.System_Single&&type!=SpecialType.System_Double)continue;
   if(!SymbolEqualityComparer.Default.Equals(field.Type,model.GetTypeInfo(assignment.Right).Type))continue;
   if(assignment.Left is MemberAccessExpressionSyntax member){if(!Round9DivisionReceiver(member.Expression,model))continue;}
   else if(!(assignment.Left is IdentifierNameSyntax))continue;
   string lhs=assignment.Left.ToString(),after=lhs+" = ("+lhs+") / ("+assignment.Right+")";
   int start=assignment.SpanStart,end=assignment.Span.End;string before=text.Substring(start,end-start);
   if(!seen.Add(family+":"+start+":"+after))continue;
   patches.Add(new Patch{rule=family,start=start,end=end,before=before,after=after,distance=Math.Abs(start-point)});
  }
 }
}
