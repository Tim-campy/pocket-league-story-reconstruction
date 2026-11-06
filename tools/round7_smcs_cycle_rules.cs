// Prepared for autonomous cycles; active dispatcher must opt in explicitly.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static partial class Round7TransformSyntax {
    static bool Round7TailToMethod(StatementSyntax statement,BlockSyntax body) {
        SyntaxNode current=statement;
        while(current!=body) {
            if(current.Parent is BlockSyntax block) {
                if(block.Statements.LastOrDefault()!=current)return false;
                current=block;
            } else if(current.Parent is ElseClauseSyntax clause)current=clause;
            else if(current.Parent is IfStatementSyntax branch)current=branch;
            else return false;
        }
        return true;
    }
    static void AddRound7SmcsCycleRules(JsonElement input,string text,StatementSyntax mapped,BlockSyntax methodBody,
        SemanticModel model,int point,List<Patch> patches,HashSet<string> seen) {
        string compiler=input.TryGetProperty("compiler",out var hint)?hint.GetString():"";
        if(compiler!=""&&compiler!="smcs"&&compiler!="mono-smcs")return;
        string selected=input.TryGetProperty("round7_cycle_rule",out var selection)&&selection.ValueKind==JsonValueKind.String?selection.GetString():"";
        if(selected!="terminal-guard-positive-tail"&&selected!="empty-if-terminal-polarity")return;
        var body=Round7CallableBody(mapped,methodBody);if(body==null)return;
        Action<string,int,int,string> add=(rule,start,end,after)=>{
            if(start<body.SpanStart||end>body.Span.End||start>=end)return;
            string before=text.Substring(start,end-start);
            if(before==after||!seen.Add(start+":"+end+":"+after))return;
            patches.Add(new Patch{rule=rule,start=start,end=end,before=before,after=after,
                distance=start<=point&&point<end?0:Math.Min(Math.Abs(start-point),Math.Abs(end-point))});
        };
        var branch=mapped.AncestorsAndSelf().OfType<IfStatementSyntax>().FirstOrDefault();
        if(branch==null)return;
        // A guard inside the terminal else of an if chain can reach the same
        // single method ret by making the rest of that block a positive if.
        if(selected=="terminal-guard-positive-tail"&&branch.Else==null&&Only(branch.Statement) is ReturnStatementSyntax returned&&returned.Expression==null
            &&branch.Parent is BlockSyntax block&&Round7TailToMethod(block,body)) {
            int index=block.Statements.IndexOf(branch);
            if(index>=0&&index<block.Statements.Count-1) {
                int end=block.Statements.Last().Span.End;
                string tail=text.Substring(branch.Span.End,end-branch.Span.End);
                add("terminal-guard-positive-tail",branch.SpanStart,end,"if ("+Negate(branch.Condition,model)+") {\n"+tail+"\n}");
            }
        }
        bool empty=branch.Statement is EmptyStatementSyntax||branch.Statement is BlockSyntax emptyBlock&&emptyBlock.Statements.Count==0;
        if(selected=="empty-if-terminal-polarity"&&empty&&branch.Else==null&&model.GetTypeInfo(branch.Condition).Type?.SpecialType==SpecialType.System_Boolean) {
            ExpressionSyntax terminal=Unwrap(branch.Condition);
            bool safe=true;
            while(terminal is BinaryExpressionSyntax logical&&(logical.IsKind(SyntaxKind.LogicalAndExpression)||logical.IsKind(SyntaxKind.LogicalOrExpression))) {
                if(model.GetSymbolInfo(logical).Symbol is IMethodSymbol method&&method.MethodKind==MethodKind.UserDefinedOperator) {safe=false;break;}
                terminal=Unwrap(logical.Right);
            }
            if(safe&&model.GetTypeInfo(terminal).Type?.SpecialType==SpecialType.System_Boolean)
                add("empty-if-terminal-polarity",branch.SpanStart,branch.Span.End,Change(text,branch.SpanStart,branch.Span.End,
                    new[]{(terminal.SpanStart,terminal.Span.End,Negate(terminal,model))}));
        }
    }
}
