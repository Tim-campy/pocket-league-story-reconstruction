// WIP: paired source edits; no target compilation or state promotion.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static partial class Round7TransformSyntax {
    static bool Round7BalancedEmpty(StatementSyntax statement) => statement is EmptyStatementSyntax
        ||statement is BlockSyntax block&&block.Statements.Count==0;
    static bool Round7BalancedBoolean(ExpressionSyntax expression,SemanticModel model) {
        if(model.GetTypeInfo(expression).Type?.SpecialType!=SpecialType.System_Boolean)return false;
        foreach(var node in expression.DescendantNodesAndSelf().OfType<ExpressionSyntax>())
            if(model.GetSymbolInfo(node).Symbol is IMethodSymbol method&&
                (method.MethodKind==MethodKind.UserDefinedOperator||method.MethodKind==MethodKind.Conversion))return false;
        return true;
    }
    static void AddRound7BalancedBranchRules(JsonElement input,string text,StatementSyntax mapped,BlockSyntax methodBody,
        SemanticModel model,int point,List<Patch> patches,HashSet<string> seen) {
        string compiler=input.TryGetProperty("compiler",out var c)?c.GetString():"";
        string selected=input.TryGetProperty("round7_cycle_rule",out var s)?s.GetString():"";
        bool learned=input.TryGetProperty("round7_cycle_rules",out var active)&&active.ValueKind==JsonValueKind.Array
            &&active.EnumerateArray().Any(value=>value.ValueKind==JsonValueKind.String&&value.GetString()=="smcs-balanced-empty-arms");
        if((compiler!=""&&compiler!="smcs"&&compiler!="mono-smcs")||(selected!="smcs-balanced-empty-arms"&&!learned))return;
        // Both five-byte transfers must be observed in the independent IL.
        if(!Round7Flag(input,"balanced_missing_long_branch")||!Round7Flag(input,"balanced_extra_fallthrough_long_branch"))return;
        var body=Round7CallableBody(mapped,methodBody);if(body==null)return;
        var descendants=mapped.DescendantNodesAndSelf().OfType<IfStatementSyntax>()
            .Concat(mapped.Ancestors().TakeWhile(n=>n!=body).OfType<IfStatementSyntax>())
            .Where(branch=>Round7WithinCallable(branch,body)).Distinct().ToArray();
        foreach(var descendant in descendants) {
            if(descendant.Else!=null||!Round7BalancedBoolean(descendant.Condition,model))continue;
            foreach(var ancestor in descendant.Ancestors().TakeWhile(n=>n!=body).OfType<IfStatementSyntax>()) {
                if(!Round7WithinCallable(ancestor,body)||ancestor.Else==null||!Round7BalancedEmpty(ancestor.Else.Statement))continue;
                if(!ancestor.Statement.Span.Contains(descendant.Span))continue;
                var flow=model.AnalyzeControlFlow(ancestor.Statement);
                if(!flow.Succeeded||!flow.EndPointIsReachable)continue;
                string recipient="if ("+Negate(descendant.Condition,model)+") { } else "+descendant.Statement;
                string after=Change(text,ancestor.SpanStart,ancestor.Span.End,new[]{
                    (descendant.SpanStart,descendant.Span.End,recipient),
                    (ancestor.Else.SpanStart,ancestor.Else.Span.End,"")});
                int start=ancestor.SpanStart,end=ancestor.Span.End;
                string before=text.Substring(start,end-start);
                if(start<body.SpanStart||end>body.Span.End||before==after||!seen.Add(start+":"+end+":"+after))continue;
                patches.Add(new Patch {rule="smcs-balanced-empty-arms",start=start,end=end,before=before,after=after,
                    distance=start<=point&&point<end?0:Math.Min(Math.Abs(start-point),Math.Abs(end-point))});
            }
        }
    }
}
