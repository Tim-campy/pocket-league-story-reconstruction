// Source-only hypothesis. Opt-in and independent IL evidence required.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static partial class Round7TransformSyntax {
    static bool Round7TailGuardBoolean(ExpressionSyntax expression,SemanticModel model) {
        if(model.GetTypeInfo(expression).Type?.SpecialType!=SpecialType.System_Boolean)return false;
        return !expression.DescendantNodesAndSelf().OfType<ExpressionSyntax>().Any(node=>
            model.GetSymbolInfo(node).Symbol is IMethodSymbol method&&
            (method.MethodKind==MethodKind.UserDefinedOperator||method.MethodKind==MethodKind.Conversion));
    }
    static bool Round7DoOwnedContinue(DoStatementSyntax loop) => loop.Statement.DescendantNodesAndSelf()
        .OfType<ContinueStatementSyntax>().Any(statement=>statement.Ancestors().FirstOrDefault(node=>
            node is WhileStatementSyntax||node is DoStatementSyntax||node is ForStatementSyntax||node is ForEachStatementSyntax)==loop);
    static void AddRound7InfiniteTailGuardRules(JsonElement input,string text,StatementSyntax mapped,BlockSyntax methodBody,
        SemanticModel model,int point,List<Patch> patches,HashSet<string> seen) {
        string compiler=input.TryGetProperty("compiler",out var c)?c.GetString():"";
        string selected=input.TryGetProperty("round7_cycle_rule",out var s)?s.GetString():"";
        bool learned=input.TryGetProperty("round7_cycle_rules",out var active)&&active.ValueKind==JsonValueKind.Array
            &&active.EnumerateArray().Any(value=>value.ValueKind==JsonValueKind.String&&value.GetString()=="smcs-do-infinite-tail-break");
        if((compiler!=""&&compiler!="smcs"&&compiler!="mono-smcs")||(selected!="smcs-do-infinite-tail-break"&&!learned))return;
        if(!Round7Flag(input,"original_infinite_tail_guard"))return;
        var body=Round7CallableBody(mapped,methodBody);if(body==null)return;
        var loops=mapped.DescendantNodesAndSelf().OfType<DoStatementSyntax>()
            .Concat(mapped.Ancestors().TakeWhile(n=>n!=body).OfType<DoStatementSyntax>())
            .Where(loop=>Round7WithinCallable(loop,body)).Distinct();
        foreach(var loop in loops) {
            if(!Round7TailGuardBoolean(loop.Condition,model)||Round7DoOwnedContinue(loop))continue;
            // Keep the original body block as a child block, so the old tail
            // condition cannot bind to a variable declared only inside it.
            // Captures and arbitrary transfers require a separate lifetime proof.
            if(loop.Statement.DescendantNodesAndSelf().Any(node=>node is AnonymousFunctionExpressionSyntax
                ||node is LocalFunctionStatementSyntax||node is GotoStatementSyntax||node is LabeledStatementSyntax
                ||node is YieldStatementSyntax))continue;
            string after="while (true) {\n"+loop.Statement+"\nif ("+Negate(loop.Condition,model)+") { break; }\n}";
            int start=loop.SpanStart,end=loop.Span.End;string before=text.Substring(start,end-start);
            if(start<body.SpanStart||end>body.Span.End||!seen.Add(start+":"+end+":"+after))continue;
            patches.Add(new Patch {rule="smcs-do-infinite-tail-break",start=start,end=end,before=before,after=after,
                distance=start<=point&&point<end?0:Math.Min(Math.Abs(start-point),Math.Abs(end-point))});
        }
    }
}
