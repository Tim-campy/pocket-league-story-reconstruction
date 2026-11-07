// One root-level void-return guard at a time. Compilation is the body oracle.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static partial class Round7TransformSyntax {
    static System.Collections.Generic.IEnumerable<ExpressionSyntax> RootGuardDisjuncts(ExpressionSyntax expression) {
        expression=Unwrap(expression);
        if(expression is BinaryExpressionSyntax logical && logical.IsKind(SyntaxKind.LogicalOrExpression)) {
            foreach(var term in RootGuardDisjuncts(logical.Left))yield return term;
            foreach(var term in RootGuardDisjuncts(logical.Right))yield return term;
        } else yield return expression;
    }
    static void AddRound10RootGuardRules(JsonElement input,string text,StatementSyntax mapped,BlockSyntax body,
            SemanticModel model,int point,List<Patch> patches,HashSet<string> seen) {
        if(!input.TryGetProperty("round7_cycle_rule",out var selected)
            || selected.GetString()!="root-guard-positive-tail")return;
        foreach(var guard in body.Statements.OfType<IfStatementSyntax>()) {
            if(guard.Else!=null || !(Only(guard.Statement) is ReturnStatementSyntax returned)
                || returned.Expression!=null)continue;
            if(guard.Condition.DescendantNodesAndSelf().Any(n=>model.GetSymbolInfo(n).Symbol is IMethodSymbol op
                && op.MethodKind==MethodKind.UserDefinedOperator))continue;
            int index=body.Statements.IndexOf(guard);
            if(index<0 || index==body.Statements.Count-1)continue;
            int end=body.Statements.Last().Span.End;
            string tail=text.Substring(guard.Span.End,end-guard.Span.End);
            string after="if ("+Negate(guard.Condition,model)+") {\n"+tail+"\n}";
            string before=text.Substring(guard.SpanStart,end-guard.SpanStart);
            if(!seen.Add(guard.SpanStart+":"+end+":"+after))continue;
            patches.Add(new Patch{rule="root-guard-positive-tail",start=guard.SpanStart,end=end,
                before=before,after=after,distance=Distance(guard,point)});
            // A merged OR guard may contain a real early return followed by
            // a separate positive region. Keep each evaluation in source order.
            var terms=RootGuardDisjuncts(guard.Condition).ToArray();
            for(int prefix=1;prefix<terms.Length;prefix++) {
                string early=string.Join("\n",terms.Take(prefix).Select(t=>"if ("+t+") { return; }"));
                string positive=string.Join(" && ",terms.Skip(prefix).Select(t=>"("+Negate(t,model)+")"));
                string split=early+"\nif ("+positive+") {\n"+tail+"\n}";
                if(seen.Add(guard.SpanStart+":"+end+":"+split))
                    patches.Add(new Patch{rule="root-guard-positive-tail-split-prefix-"+prefix,
                        start=guard.SpanStart,end=end,before=before,after=split,distance=Distance(guard,point)});
            }
        }
    }
}
