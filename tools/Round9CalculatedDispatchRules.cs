using System;using System.Linq;using System.Text.Json;using System.Collections.Generic;
using Microsoft.CodeAnalysis;using Microsoft.CodeAnalysis.CSharp;using Microsoft.CodeAnalysis.CSharp.Syntax;
public static partial class Round7TransformSyntax {
 static bool DNextBreakable(SyntaxNode n)=>n is SwitchStatementSyntax||n is ForStatementSyntax||n is ForEachStatementSyntax||n is WhileStatementSyntax||n is DoStatementSyntax;
 static void AddRound9CalculatedDispatchRules(JsonElement input,string text,StatementSyntax mapped,BlockSyntax body,SemanticModel model,int point,List<Patch>patches,HashSet<string>seen){
  if(!input.TryGetProperty("round7_cycle_rule",out var rule)||rule.GetString()!="d9-calculated-state-dispatch-index")return;
  var selected=body.DescendantNodes().OfType<SwitchStatementSyntax>().Where(s=>s.Expression.ToString()=="num2").ToArray();if(selected.Length!=1)return;var node=selected[0];
  if(model.GetTypeInfo(node.Expression).Type?.SpecialType!=SpecialType.System_Int32||node.Sections.Count!=2)return;
  var allOwned=node.DescendantNodes().OfType<BreakStatementSyntax>().Where(b=>b.Ancestors().FirstOrDefault(DNextBreakable)==node).ToArray();if(allOwned.Length!=2)return;
  if(node.DescendantNodes().Any(n=>n is GotoStatementSyntax))return;
  string nl=text.Contains("\r\n")?"\r\n":"\n",after="";int ordinal=0;
  foreach(var section in node.Sections){
   if(section.Labels.Count!=1||!(section.Labels[0]is CaseSwitchLabelSyntax label)||section.Statements.Count!=1||!(section.Statements[0]is BlockSyntax block)||block.Statements.Count<1)return;
   var constant=model.GetConstantValue(label.Value);if(!constant.HasValue||!(constant.Value is int value)||value!=ordinal)return;
   if(!(block.Statements.Last()is BreakStatementSyntax last)||!allOwned.Contains(last))return;
   string commands=block.Statements.Count==1?"":text.Substring(block.Statements[0].SpanStart,last.SpanStart-block.Statements[0].SpanStart);
   after+=(ordinal==0?"if":"else if")+" (num2 == "+value+")"+nl+"{"+nl+commands+"}"+nl;ordinal++;
  }
  patches.Add(new Patch{rule=rule.GetString(),start=node.SpanStart,end=node.Span.End,before=text.Substring(node.SpanStart,node.Span.Length),after=after.TrimEnd('\r','\n'),distance=0});
 }
}
