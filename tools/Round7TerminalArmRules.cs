// Opt-in hypothesis only. Not included by the active helper until root adopts it.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static partial class Round7TransformSyntax {
    // Read-only fixture evidence; production dispatch does not depend on it.
    static string[] Round7TerminalArmPath(StatementSyntax mapped,BlockSyntax methodBody) {
        var body=Round7CallableBody(mapped,methodBody);
        var branch=mapped.AncestorsAndSelf().OfType<IfStatementSyntax>().FirstOrDefault();
        var path=new List<string>();SyntaxNode node=branch?.Parent;
        while(node!=null) {
            path.Add(node.Kind()+"@"+node.SpanStart+(node==body?":method-body":""));
            if(node==body)break;
            if(node.Parent is BlockSyntax block) {
                if(block.Statements.LastOrDefault()!=node){path.Add("REJECT:following-statement");break;}
                node=block;
            } else if(node.Parent is ElseClauseSyntax clause)node=clause;
            else if(node.Parent is IfStatementSyntax outer)node=outer;
            else {path.Add("REJECT:"+node.Parent?.Kind());break;}
        }
        return path.ToArray();
    }
    static bool Round7TerminalArmUnsafe(SyntaxNode region) => region.DescendantNodesAndSelf().Any(n=>
        n is AnonymousFunctionExpressionSyntax || n is LocalFunctionStatementSyntax ||
        n is TryStatementSyntax || n is UsingStatementSyntax || n is LockStatementSyntax ||
        n is FixedStatementSyntax || n is YieldStatementSyntax || n is GotoStatementSyntax ||
        n is LabeledStatementSyntax || n is ForEachStatementSyntax);

    static void AddRound7TerminalArmRules(JsonElement input,string text,StatementSyntax mapped,BlockSyntax methodBody,
        SemanticModel model,int point,List<Patch> patches,HashSet<string> seen) {
        const string rule="smcs-terminal-actions-common-exit";
        string compiler=input.TryGetProperty("compiler",out var c)?c.GetString():"";
        string selected=input.TryGetProperty("round7_cycle_rule",out var r)?r.GetString():"";
        if((compiler!="smcs"&&compiler!="mono-smcs")||selected!=rule)return;
        var body=Round7CallableBody(mapped,methodBody);
        if(body==null||body.Parent is AnonymousFunctionExpressionSyntax)return;
        var symbol=model.GetDeclaredSymbol(body.Parent) as IMethodSymbol;
        if(symbol==null||!symbol.ReturnsVoid)return;
        var branch=mapped.AncestorsAndSelf().OfType<IfStatementSyntax>()
            .FirstOrDefault(n=>Round7WithinCallable(n,body));
        if(branch==null||branch.Else!=null||branch.Parent is not BlockSyntax block
            ||!Round7TailToMethod(block,body)||branch.Statement is not BlockSyntax arm
            ||arm.Statements.Count<2||arm.Statements.Last() is not ReturnStatementSyntax ret||ret.Expression!=null)return;
        // This new family covers straight-line actions preceding the terminal
        // return. Nested returns/control flow belong to other proven families.
        if(arm.Statements.Take(arm.Statements.Count-1).Any(s=>s is not ExpressionStatementSyntax
            &&s is not LocalDeclarationStatementSyntax&&s is not EmptyStatementSyntax))return;
        int index=block.Statements.IndexOf(branch);
        if(index<0||index>=block.Statements.Count-1)return;
        var tail=block.Statements.Skip(index+1).ToArray();
        if(Round7TerminalArmUnsafe(arm)||tail.Any(Round7TerminalArmUnsafe))return;
        int start=branch.SpanStart,end=tail.Last().Span.End;
        string nl=text.Contains("\r\n")?"\r\n":"\n";
        string actions=text.Substring(arm.OpenBraceToken.Span.End,ret.SpanStart-arm.OpenBraceToken.Span.End);
        string suffix=text.Substring(branch.Span.End,end-branch.Span.End);
        string after="if ("+branch.Condition+") {"+actions+nl+"} else {"+suffix+nl+"}";
        string before=text.Substring(start,end-start);
        if(before==after||!seen.Add(start+":"+end+":"+after))return;
        patches.Add(new Patch{rule=rule,start=start,end=end,before=before,after=after,
            distance=start<=point&&point<end?0:Math.Min(Math.Abs(start-point),Math.Abs(end-point))});
    }
}
