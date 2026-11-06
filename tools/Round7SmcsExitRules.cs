// Prototype only: not included by the production syntax-helper builder.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static partial class Round7TransformSyntax {
    static void AddRound7SmcsExitRules(JsonElement input,string text,StatementSyntax mapped,BlockSyntax methodBody,
        SemanticModel model,int point,List<Patch> patches,HashSet<string> seen) {
        string compiler=input.TryGetProperty("compiler",out var c)?c.GetString():"";
        string selected=input.TryGetProperty("round7_cycle_rule",out var s)?s.GetString():"";
        if((compiler!=""&&compiler!="smcs"&&compiler!="mono-smcs")
            ||(selected!="void-empty-then-common-exit"&&selected!="loop-guard-return-to-continue"))return;
        string kind=input.TryGetProperty("original_exit_target_kind",out var k)?k.GetString():"";
        var body=Round7CallableBody(mapped,methodBody);if(body==null)return;
        var branch=mapped.AncestorsAndSelf().OfType<IfStatementSyntax>().FirstOrDefault();
        if(branch==null||branch.Else!=null||!(Only(branch.Statement) is ReturnStatementSyntax ret)||ret.Expression!=null)return;
        Action<string,int,int,string> add=(rule,start,end,after)=>{
            if(start<body.SpanStart||end>body.Span.End||start>=end)return;
            string before=text.Substring(start,end-start);
            if(before==after||!seen.Add(start+":"+end+":"+after))return;
            patches.Add(new Patch{rule=rule,start=start,end=end,before=before,after=after,
                distance=start<=point&&point<end?0:Math.Min(Math.Abs(start-point),Math.Abs(end-point))});
        };
        if(selected=="void-empty-then-common-exit"&&kind=="method-ret"&&branch.Parent is BlockSyntax block&&Round7TailToMethod(block,body)) {
            int index=block.Statements.IndexOf(branch);
            if(index>=0&&index<block.Statements.Count-1) {
                int end=block.Statements.Last().Span.End;
                string tail=text.Substring(branch.Span.End,end-branch.Span.End);
                add("void-empty-then-common-exit",branch.SpanStart,end,"if ("+branch.Condition+") { } else {\n"+tail+"\n}");
            }
        }
        if(selected=="loop-guard-return-to-continue"&&kind=="loop-increment") {
            // The original target is the counter increment, so return is a
            // recovered spelling error. Keep the float condition untouched.
            var loop=ret.Ancestors().TakeWhile(a=>a!=body).FirstOrDefault(IsLoop);
            if(loop is ForStatementSyntax forLoop&&forLoop.Incrementors.Count>0)
                add("loop-guard-return-to-continue",ret.SpanStart,ret.Span.End,"continue;");
        }
    }
}
