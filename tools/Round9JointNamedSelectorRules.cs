using System;using System.Collections.Generic;using System.Linq;using System.Text.Json;
using Microsoft.CodeAnalysis;using Microsoft.CodeAnalysis.CSharp;using Microsoft.CodeAnalysis.CSharp.Syntax;
public static partial class Round7TransformSyntax {
 static void AddRound9JointNamedSelectorRules(JsonElement input,string text,StatementSyntax mapped,BlockSyntax methodBody,SemanticModel model,int point,List<Patch> patches,HashSet<string>seen){
  if(!input.TryGetProperty("round7_cycle_rule",out var rule)||rule.GetString()!="d9-joint-flow-local-stores")return;
  var switches=methodBody.DescendantNodes().OfType<SwitchStatementSyntax>().Where(s=>s.Expression.ToString()=="udat_.GetMatchInfoJoin()").ToArray();
  if(switches.Length!=2)return;
  var generated=new List<Patch>();int ordinal=0;foreach(var node in switches){ordinal++;string name="testedMatchInfoJoin"+ordinal;
   if(methodBody.DescendantNodes().OfType<IdentifierNameSyntax>().Any(i=>i.Identifier.ValueText==name))return;
   if(model.GetTypeInfo(node.Expression).Type?.SpecialType!=SpecialType.System_Int32)return;
   if(node.Sections.Any(s=>s.Labels.Count!=1||s.Statements.Count==0))return;
   if(node.DescendantNodes().Any(n=>n is GotoStatementSyntax||n is BreakStatementSyntax))return;
   // Every selected arm terminates in return; neither goto nor switch-owned break is rewritten.
   foreach(var section in node.Sections){StatementSyntax last=section.Statements.Last();if(last is BlockSyntax b)last=b.Statements.LastOrDefault();if(!(last is ReturnStatementSyntax))return;}
   int line=text.LastIndexOf('\n',Math.Max(0,node.SpanStart-1));string indent=text.Substring(line+1,node.SpanStart-line-1);if(indent.Any(c=>c!=' '&&c!='\t'))indent="";
   string nl=text.Contains("\r\n")?"\r\n":"\n",after="int "+name+" = "+text.Substring(node.Expression.SpanStart,node.Expression.Span.Length)+";"+nl+indent;
   bool first=true;foreach(var section in node.Sections){var label=section.Labels[0];if(label is DefaultSwitchLabelSyntax){if(first)return;after+="else";}else if(label is CaseSwitchLabelSyntax c){var constant=model.GetConstantValue(c.Value);if(!constant.HasValue||!(constant.Value is int))return;after+=(first?"if":"else if")+" ("+name+" == "+text.Substring(c.Value.SpanStart,c.Value.Span.Length)+")";}else return;
    after+=nl+indent+"{"+nl;
    foreach(var statement in section.Statements)after+=indent+"\t"+text.Substring(statement.SpanStart,statement.Span.Length)+nl;
    after+=indent+"}"+nl+indent;first=false;
   }
   generated.Add(new Patch{rule=rule.GetString(),start=node.SpanStart,end=node.Span.End,before=text.Substring(node.SpanStart,node.Span.Length),after=after.TrimEnd('\r','\n','\t',' '),distance=0});
  }
  foreach(var patch in generated)if(seen.Add("joint:"+patch.start))patches.Add(patch);
 }
}
