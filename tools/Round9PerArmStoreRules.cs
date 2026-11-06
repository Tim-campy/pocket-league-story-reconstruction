// Source-only reconstruction of an original store in each conditional arm.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static partial class Round7TransformSyntax {
 static void AddRound9PerArmStoreRules(JsonElement input,string text,StatementSyntax mapped,BlockSyntax methodBody,
  SemanticModel model,int point,List<Patch> patches,HashSet<string> seen){
  if(!input.TryGetProperty("round7_cycle_rule",out var rule)||rule.GetString()!="d9-calculated-per-arm-local-stores")return;
  if(!input.TryGetProperty("round9_original_same_slot_arm_stores",out var evidence)||evidence.ValueKind!=JsonValueKind.True)return;
  if(!input.TryGetProperty("round9_per_arm_store_slot",out var slot)||!slot.TryGetInt32(out int index)||index<0)return;
  if(!input.TryGetProperty("round9_per_arm_store_order",out var order)||order.GetString()!="false-first")return;
  var scope=mapped??(StatementSyntax)methodBody;
  foreach(var statement in scope.DescendantNodesAndSelf(n=>!(n is AnonymousFunctionExpressionSyntax)&&!(n is LocalFunctionStatementSyntax)).OfType<ExpressionStatementSyntax>()){
   if(!(statement.Expression is AssignmentExpressionSyntax assignment)||!assignment.IsKind(SyntaxKind.SimpleAssignmentExpression))continue;
   if(!(assignment.Left is IdentifierNameSyntax name))continue;
   var local=model.GetSymbolInfo(name).Symbol as ILocalSymbol;
   if(local==null||local.IsConst||local.Type.TypeKind==TypeKind.Dynamic)continue;
   ExpressionSyntax value=assignment.Right;while(value is ParenthesizedExpressionSyntax paren)value=paren.Expression;
   if(!(value is ConditionalExpressionSyntax ternary))continue;
   if(model.GetTypeInfo(ternary.Condition).Type?.SpecialType!=SpecialType.System_Boolean)continue;
   if(ternary.DescendantNodesAndSelf().Any(n=>n is AwaitExpressionSyntax||n is AnonymousFunctionExpressionSyntax))continue;
   // Keep the condition verbatim: negating its value preserves unordered .un semantics.
   string left=text.Substring(name.SpanStart,name.Span.Length),condition=text.Substring(ternary.Condition.SpanStart,ternary.Condition.Span.Length);
   string a=text.Substring(ternary.WhenTrue.SpanStart,ternary.WhenTrue.Span.Length),b=text.Substring(ternary.WhenFalse.SpanStart,ternary.WhenFalse.Span.Length);
   int line=text.LastIndexOf('\n',Math.Max(0,statement.SpanStart-1));string indent=text.Substring(line+1,statement.SpanStart-line-1);
   if(indent.Any(c=>c!=' '&&c!='\t'))indent="";
   string nl=text.Contains("\r\n")?"\r\n":"\n";
   string after="if (!("+condition+"))"+nl+indent+"{"+nl+indent+"\t"+left+" = "+b+";"+nl+indent+"}"+nl+indent+"else"+nl+indent+"{"+nl+indent+"\t"+left+" = "+a+";"+nl+indent+"}";
   string key=rule.GetString()+":"+statement.SpanStart;if(!seen.Add(key))continue;
   patches.Add(new Patch{rule=rule.GetString(),start=statement.SpanStart,end=statement.Span.End,before=text.Substring(statement.SpanStart,statement.Span.Length),after=after,distance=Math.Abs(point-statement.SpanStart)});
  }
 }
}
