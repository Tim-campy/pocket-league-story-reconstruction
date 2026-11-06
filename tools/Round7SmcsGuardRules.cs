// Frozen candidate repertoire is published only in a root-controlled window.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static partial class Round7TransformSyntax {
    static bool Round7GuardTailTo(SyntaxNode node,SyntaxNode boundary,bool throughTry=false) {
        while(node!=boundary) {
            if(node.Parent is BlockSyntax block) {
                if(block.Statements.LastOrDefault()!=node)return false;
                node=block;
            } else if(node.Parent is ElseClauseSyntax clause)node=clause;
            else if(node.Parent is IfStatementSyntax branch)node=branch;
            else if(throughTry&&node.Parent is TryStatementSyntax attempt&&attempt.Block==node)node=attempt;
            else return false;
        }
        return true;
    }
    static void Round7GuardAndParts(ExpressionSyntax expression,List<ExpressionSyntax> parts) {
        expression=Unwrap(expression);
        if(expression is BinaryExpressionSyntax binary&&binary.IsKind(SyntaxKind.LogicalAndExpression)) {
            Round7GuardAndParts(binary.Left,parts);Round7GuardAndParts(binary.Right,parts);
        } else parts.Add(expression);
    }
    static void AddRound7SmcsGuardRules(JsonElement input,string text,StatementSyntax mapped,BlockSyntax methodBody,
        SemanticModel model,int point,List<Patch> patches,HashSet<string> seen) {
        string compiler=input.TryGetProperty("compiler",out var c)?c.GetString():"";
        string selected=input.TryGetProperty("round7_cycle_rule",out var s)?s.GetString():"";
        if(compiler!=""&&compiler!="smcs"&&compiler!="mono-smcs")return;
        if(selected!="smcs-empty-arm-restore"&&selected!="smcs-continue-positive-tail"
            &&selected!="smcs-value-return-guard-reverse"&&selected!="smcs-protected-void-guard-tail")return;
        var body=Round7CallableBody(mapped,methodBody);if(body==null)return;
        Action<string,int,int,string> add=(rule,start,end,after)=>{
            if(start<body.SpanStart||end>body.Span.End||start>=end)return;
            string before=text.Substring(start,end-start);
            if(before==after||!seen.Add(start+":"+end+":"+after))return;
            patches.Add(new Patch{rule=rule,start=start,end=end,before=before,after=after,
                distance=start<=point&&point<end?0:Math.Min(Math.Abs(start-point),Math.Abs(end-point))});
        };
        var branches=mapped.DescendantNodesAndSelf().OfType<IfStatementSyntax>()
            .Concat(mapped.Ancestors().TakeWhile(n=>n!=body).OfType<IfStatementSyntax>())
            .Where(n=>Round7WithinCallable(n,body)).Distinct().ToArray();
        foreach(var branch in branches) {
            if(branch.Else!=null)continue;
            if(selected=="smcs-empty-arm-restore") {
                // A real empty then arm still causes mcs to emit br over else.
                // A missing else therefore changes opcode AND block layout.
                add("smcs-empty-arm-restore",branch.SpanStart,branch.Span.End,
                    "if ("+Negate(branch.Condition,model)+") { } else "+branch.Statement);
                var parts=new List<ExpressionSyntax>();Round7GuardAndParts(branch.Condition,parts);
                if(parts.Count>1) {
                    string rest=string.Join(" && ",parts.Skip(1).Select(e=>"("+e+")"));
                    add("smcs-empty-arm-first-and",branch.SpanStart,branch.Span.End,
                        "if ("+Negate(parts[0],model)+") { } else if ("+rest+") "+branch.Statement);
                    // Preserve an already matching leading conjunction while
                    // restoring an empty case inside its guarded suffix.
                    for(int split=1;split<parts.Count;split++) {
                        string prefix=string.Join(" && ",parts.Take(split).Select(e=>"("+e+")"));
                        string negatedSuffix=string.Join(" || ",parts.Skip(split).Select(e=>"("+Negate(e,model)+")"));
                        add("smcs-empty-arm-and-suffix",branch.SpanStart,branch.Span.End,
                            "if ("+prefix+") { if ("+negatedSuffix+") { } else "+branch.Statement+" }");
                    }
                }
                continue;
            }
            if(!(branch.Parent is BlockSyntax block))continue;
            int index=block.Statements.IndexOf(branch);if(index<0||index>=block.Statements.Count-1)continue;
            int end=block.Statements.Last().Span.End;
            string tail=text.Substring(branch.Span.End,end-branch.Span.End);
            var only=Only(branch.Statement);
            if(selected=="smcs-continue-positive-tail"&&only is ContinueStatementSyntax) {
                var loop=branch.Ancestors().TakeWhile(n=>n!=body).FirstOrDefault(IsLoop);
                StatementSyntax loopBody=null;
                if(loop is ForStatementSyntax f)loopBody=f.Statement;
                else if(loop is ForEachStatementSyntax e)loopBody=e.Statement;
                else if(loop is WhileStatementSyntax w)loopBody=w.Statement;
                else if(loop is DoStatementSyntax d)loopBody=d.Statement;
                if(loopBody!=null&&Round7GuardTailTo(block,loopBody))
                    add("smcs-continue-positive-tail",branch.SpanStart,end,"if ("+Negate(branch.Condition,model)+") {\n"+tail+"\n}");
            }
            if(selected=="smcs-value-return-guard-reverse"&&Terminates(branch.Statement)
                &&branch.Statement.DescendantNodesAndSelf().OfType<ReturnStatementSyntax>().Any(r=>r.Expression!=null&&Round7WithinCallable(r,body)))
                add("smcs-value-return-guard-reverse",branch.SpanStart,end,
                    "if ("+Negate(branch.Condition,model)+") {\n"+tail+"\n} else "+branch.Statement);
            if(selected=="smcs-protected-void-guard-tail"&&only is ReturnStatementSyntax ret&&ret.Expression==null
                &&!Round7TailToMethod(block,body)&&Round7GuardTailTo(block,body,true))
                add("smcs-protected-void-guard-tail",branch.SpanStart,end,"if ("+Negate(branch.Condition,model)+") {\n"+tail+"\n}");
        }
    }
}
