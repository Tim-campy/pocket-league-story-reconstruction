using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
public static partial class Round7TransformSyntax {
 static bool Round9ProtectedUnsafe(SyntaxNode node){return node.ContainsDirectives||node.DescendantNodesAndSelf().Any(n=>n is AnonymousFunctionExpressionSyntax||n is LocalFunctionStatementSyntax||n is LabeledStatementSyntax||n is GotoStatementSyntax||n is YieldStatementSyntax||n is AwaitExpressionSyntax);}
 static void AddRound9ProtectedExitRules(JsonElement input,string text,StatementSyntax mapped,BlockSyntax methodBody,SemanticModel model,int point,List<Patch> patches,HashSet<string> seen){
  const string family="d9-protected-exit-owner";
  bool selected=input.TryGetProperty("round7_cycle_rule",out var r)&&r.GetString()==family;
  bool learned=input.TryGetProperty("round7_cycle_rules",out var rs)&&rs.ValueKind==JsonValueKind.Array&&rs.EnumerateArray().Any(x=>x.GetString()==family);
  if(!selected&&!learned)return;
  if(!input.TryGetProperty("compiler",out var c)||c.GetString()!="smcs"||!input.TryGetProperty("round9_protected_exit_mode",out var mode))return;
  var rows=new List<Tuple<SyntaxNode,SyntaxNode>>();
  if(mode.GetString()=="loop-try-break"){
   foreach(var loop in methodBody.DescendantNodes(n=>!(n is AnonymousFunctionExpressionSyntax)).OfType<ForStatementSyntax>()){
    if(loop.Statement is not BlockSyntax body||body.Statements.Count!=2||body.Statements[0] is not TryStatementSyntax tr||body.Statements[1] is not BreakStatementSyntax br||tr.Catches.Count==0||Round9ProtectedUnsafe(loop))continue;
    if(tr.Block.DescendantNodes().Any(n=>n is ReturnStatementSyntax||n is BreakStatementSyntax||n is ContinueStatementSyntax))continue;
    if(tr.Catches.Any(cc=>cc.Filter!=null||cc.Block.Statements.Count==0||cc.Block.Statements.Last() is not ContinueStatementSyntax))continue;
    var catches=SyntaxFactory.List(tr.Catches.Select(cc=>cc.WithBlock(cc.Block.WithStatements(cc.Block.Statements.RemoveAt(cc.Block.Statements.Count-1)))));
    var changed=tr.WithBlock(tr.Block.WithStatements(tr.Block.Statements.Add(br.WithoutTrivia()))).WithCatches(catches);
    var replacement=loop.WithStatement(body.WithStatements(SyntaxFactory.SingletonList<StatementSyntax>(changed)));
    rows.Add(Tuple.Create((SyntaxNode)loop,(SyntaxNode)replacement));
   }
  }else if(mode.GetString()=="guard-try-returns"){
   if(methodBody.Parent is not MethodDeclarationSyntax md||model.GetDeclaredSymbol(md)?.ReturnsVoid!=true)return;
   for(int i=0;i<methodBody.Statements.Count-1;i++){
    if(methodBody.Statements[i] is not IfStatementSyntax guard||guard.Else!=null||guard.Statement is not BlockSyntax trueBody||trueBody.Statements.Count!=1||trueBody.Statements[0] is not TryStatementSyntax tr||tr.Finally!=null||tr.Catches.Count==0||Round9ProtectedUnsafe(guard))continue;
    if(tr.Block.Statements.LastOrDefault() is not ReturnStatementSyntax ret||ret.Expression!=null||tr.Catches.Any(cc=>cc.Filter!=null||cc.Block.Statements.LastOrDefault() is not ReturnStatementSyntax rr||rr.Expression!=null))continue;
    if(tr.Block.DescendantNodes().OfType<ReturnStatementSyntax>().Count()!=1||tr.Catches.Any(cc=>cc.Block.DescendantNodes().OfType<ReturnStatementSyntax>().Count()!=1))continue;
    var tail=methodBody.Statements.Skip(i+1).ToArray();if(tail.Any(Round9ProtectedUnsafe)||tail.Any(s=>s.DescendantNodesAndSelf().Any(n=>n is ReturnStatementSyntax||n is BreakStatementSyntax||n is ContinueStatementSyntax)))continue;
    var changed=tr.WithBlock(tr.Block.WithStatements(tr.Block.Statements.RemoveAt(tr.Block.Statements.Count-1))).WithCatches(SyntaxFactory.List(tr.Catches.Select(cc=>cc.WithBlock(cc.Block.WithStatements(cc.Block.Statements.RemoveAt(cc.Block.Statements.Count-1))))));
    var next=guard.WithStatement(trueBody.WithStatements(SyntaxFactory.SingletonList<StatementSyntax>(changed))).WithElse(SyntaxFactory.ElseClause(SyntaxFactory.Block(tail)));
    var replacement=methodBody.WithStatements(SyntaxFactory.List(methodBody.Statements.Take(i).Concat(new StatementSyntax[]{next})));
    rows.Add(Tuple.Create((SyntaxNode)methodBody,(SyntaxNode)replacement));
   }
  }
  foreach(var row in rows){int start=row.Item1.SpanStart,end=row.Item1.Span.End;string before=text.Substring(start,end-start),after=row.Item2.ToString();if(!seen.Add(family+":"+start+":"+after))continue;patches.Add(new Patch{rule=family,start=start,end=end,before=before,after=after,distance=Math.Abs(start-point)});}
 }
}
