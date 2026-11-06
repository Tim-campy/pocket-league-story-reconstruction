// Opt-in scoped exit candidates. This file is NOT integrated into a game helper.
using System;using System.Collections.Generic;using System.Linq;using System.Text.Json;
using Microsoft.CodeAnalysis;using Microsoft.CodeAnalysis.CSharp;using Microsoft.CodeAnalysis.CSharp.Syntax;
public static class Round9ScopedExitCandidates {
 public sealed class Candidate { public string rule{get;set;} public int start{get;set;} public int end{get;set;} public string before{get;set;} public string after{get;set;} public int distance{get;set;} public string evidence{get;set;} public int declaration_start{get;set;} public int declaration_end{get;set;} }
 static StatementSyntax Only(StatementSyntax s)=>s is BlockSyntax b&&b.Statements.Count==1?b.Statements[0]:s;
 static bool Tail(SyntaxNode node,BlockSyntax body) {while(node!=body){if(node.Parent is BlockSyntax block){if(block.Statements.LastOrDefault()!=node)return false;node=block;}else if(node.Parent is ElseClauseSyntax clause)node=clause;else if(node.Parent is IfStatementSyntax branch)node=branch;else return false;}return true;}
 static bool Unsafe(SyntaxNode region)=>region.DescendantNodesAndSelf().Any(n=>n is AnonymousFunctionExpressionSyntax||n is LocalFunctionStatementSyntax||n is YieldStatementSyntax||n is GotoStatementSyntax||n is LabeledStatementSyntax||n is FixedStatementSyntax);
 static bool EndsInTransfer(StatementSyntax statement){statement=Only(statement);return statement is ReturnStatementSyntax||statement is ThrowStatementSyntax||statement is BreakStatementSyntax||statement is ContinueStatementSyntax;}
 static string AppendReturn(StatementSyntax statement,string source){if(statement is BlockSyntax block)return source.Substring(block.SpanStart,block.CloseBraceToken.SpanStart-block.SpanStart)+"\nreturn;\n}";return "{\n"+statement+"\nreturn;\n}";}
 public static List<Candidate> Generate(string text,SyntaxNode root,SemanticModel model,int point,string family,string originalOpcode,string joinKind,int joinSourcePoint=-1){
  var output=new List<Candidate>();if(point<0||point>=text.Length)return output;
  var token=root.FindToken(point);var owner=token.Parent.AncestorsAndSelf().OfType<BaseMethodDeclarationSyntax>().FirstOrDefault();
  if(owner is not MethodDeclarationSyntax declaration||declaration.Body==null)return output;
  if(token.Parent.AncestorsAndSelf().TakeWhile(n=>n!=owner).Any(n=>n is AnonymousFunctionExpressionSyntax||n is LocalFunctionStatementSyntax))return output;
  var symbol=model.GetDeclaredSymbol(owner) as IMethodSymbol;if(symbol==null||!symbol.ReturnsVoid)return output;
  var body=declaration.Body;var seen=new HashSet<string>();
  foreach(var branch in token.Parent.AncestorsAndSelf().TakeWhile(n=>n!=owner).OfType<IfStatementSyntax>()){
   int start=branch.SpanStart,end=branch.Span.End;string after=null,evidence=null;
   if(family=="terminal-if-arm-return-flatten-else"&&originalOpcode=="ret"&&joinKind=="method-return"){
    if(branch.Else==null||!Tail(branch,body)||Unsafe(branch)||EndsInTransfer(branch.Statement))continue;
    after="if ("+branch.Condition+") "+AppendReturn(branch.Statement,text)+"\n"+branch.Else.Statement;
    // Replacing an embedded else-if with two statements must retain its lexical owner.
    if(branch.Parent is not BlockSyntax)after="{\n"+after+"\n}";
    evidence="Original ret proves method exit. Current terminal if/else has a falling true arm; add ret and retain lexical else block as following statement.";
   }
   if(family=="scoped-terminal-arm-common-join"&&originalOpcode=="br"){
    if(branch.Else!=null||branch.Parent is not BlockSyntax block)continue;
    ReturnStatementSyntax ret=null;string actions;
    if(branch.Statement is BlockSyntax arm&&arm.Statements.LastOrDefault() is ReturnStatementSyntax r&&r.Expression==null){ret=r;actions=text.Substring(arm.OpenBraceToken.Span.End,ret.SpanStart-arm.OpenBraceToken.Span.End);}
    else if(branch.Statement is ReturnStatementSyntax r2&&r2.Expression==null){ret=r2;actions="";}
    else continue;
    if(ret==null)continue;
    // These exact simple method-exit candidates were exhausted by v7. Only
    // new local joins or nonlinear true arms belong to this family.
    if(joinKind=="method-ret"){
     if(Only(branch.Statement) is ReturnStatementSyntax)continue;
     if(branch.Statement is BlockSyntax oldArm&&oldArm.Statements.Take(oldArm.Statements.Count-1).All(s=>s is ExpressionStatementSyntax||s is LocalDeclarationStatementSyntax||s is EmptyStatementSyntax))continue;
    }
    int index=block.Statements.IndexOf(branch);if(index<0||index>=block.Statements.Count-1)continue;
    bool proved=joinKind=="method-ret"&&Tail(block,body);
    if(joinKind=="scope-next"&&block.Parent is IfStatementSyntax enclosing&&enclosing.Statement==block&&enclosing.Else==null&&enclosing.Parent is BlockSyntax outer){
     int at=outer.Statements.IndexOf(enclosing);proved=at>=0&&at+1<outer.Statements.Count&&outer.Statements[at+1].Span.Contains(joinSourcePoint);
    }
    if(!proved)continue;var tail=block.Statements.Skip(index+1).ToArray();if(Unsafe(branch.Statement)||tail.Any(Unsafe))continue;
    end=tail.Last().Span.End;string suffix=text.Substring(branch.Span.End,end-branch.Span.End);
    after="if ("+branch.Condition+") {"+actions+"\n} else {"+suffix+"\n}";
    evidence="Original br target is a proved method ret or next statement after enclosing if; preserve condition and arm actions, replace only terminal return with explicit join.";
   }
   if(after==null||start<body.SpanStart||end>body.Span.End)continue;string before=text.Substring(start,end-start);if(before==after||!seen.Add(start+":"+end+":"+after))continue;
   output.Add(new Candidate{rule=family,start=start,end=end,before=before,after=after,evidence=evidence,declaration_start=owner.SpanStart,declaration_end=owner.Span.End,distance=start<=point&&point<end?0:Math.Min(Math.Abs(start-point),Math.Abs(end-point))});
  }
  return output;
 }
}
