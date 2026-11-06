// WIP source hypotheses: no target emission and no ledger writes.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static partial class Round7TransformSyntax {
    static void Round7LoopAndParts(ExpressionSyntax expression,List<ExpressionSyntax> parts) {
        expression=Unwrap(expression);
        if(expression is BinaryExpressionSyntax binary&&binary.IsKind(SyntaxKind.LogicalAndExpression)) {
            Round7LoopAndParts(binary.Left,parts);Round7LoopAndParts(binary.Right,parts);
        } else parts.Add(expression);
    }
    static bool Round7LoopOrdinaryBoolean(ExpressionSyntax expression,SemanticModel model) {
        if(model.GetTypeInfo(expression).Type?.SpecialType!=SpecialType.System_Boolean)return false;
        // && on a custom type can call op_False/op_BitwiseAnd. Negating a
        // custom comparison can call an unrelated operator. Do not speculate.
        foreach(var node in expression.DescendantNodesAndSelf().OfType<ExpressionSyntax>()) {
            if(model.GetSymbolInfo(node).Symbol is IMethodSymbol method&&method.MethodKind==MethodKind.UserDefinedOperator)return false;
        }
        return true;
    }
    static void AddRound7LoopConditionRules(JsonElement input,string text,StatementSyntax mapped,BlockSyntax methodBody,
        SemanticModel model,int point,List<Patch> patches,HashSet<string> seen) {
        string compiler=input.TryGetProperty("compiler",out var c)?c.GetString():"";
        string selected=input.TryGetProperty("round7_cycle_rule",out var s)?s.GetString():"";
        if((compiler!=""&&compiler!="smcs"&&compiler!="mono-smcs")||selected!="smcs-loop-condition-body-guard")return;
        var body=Round7CallableBody(mapped,methodBody);if(body==null)return;
        var loops=mapped.DescendantNodesAndSelf().Where(n=>n is WhileStatementSyntax||n is ForStatementSyntax)
            .Concat(mapped.Ancestors().TakeWhile(n=>n!=body).Where(n=>n is WhileStatementSyntax||n is ForStatementSyntax))
            .Where(n=>Round7WithinCallable(n,body)).Distinct().ToArray();
        foreach(var node in loops) {
            var loop=(StatementSyntax)node;
            ExpressionSyntax condition;StatementSyntax oldBody;
            if(loop is WhileStatementSyntax w) {condition=w.Condition;oldBody=w.Statement;}
            else {var f=(ForStatementSyntax)loop;condition=f.Condition;oldBody=f.Statement;}
            if(condition==null||!Round7LoopOrdinaryBoolean(condition,model))continue;
            var parts=new List<ExpressionSyntax>();Round7LoopAndParts(condition,parts);
            if(parts.Count<2||parts.Any(p=>!Round7LoopOrdinaryBoolean(p,model)))continue;
            // Each proposal moves exactly one boundary. Operand order, number
            // of evaluations, and the for initializer/increment stay intact.
            for(int split=1;split<parts.Count;split++) {
                string prefix=string.Join(" && ",parts.Take(split).Select(p=>"("+p+")"));
                string stop=string.Join(" || ",parts.Skip(split).Select(p=>"("+Negate(p,model)+")"));
                var newBody=SyntaxFactory.ParseStatement("{\nif ("+stop+") { break; }\n"+BodyText(oldBody)+"\n}");
                string after=loop is WhileStatementSyntax whileLoop
                    ?whileLoop.WithCondition(SyntaxFactory.ParseExpression(prefix)).WithStatement(newBody).ToString()
                    :((ForStatementSyntax)loop).WithCondition(SyntaxFactory.ParseExpression(prefix)).WithStatement(newBody).ToString();
                int start=loop.SpanStart,end=loop.Span.End;
                string before=text.Substring(start,end-start);
                if(start<body.SpanStart||end>body.Span.End||before==after||!seen.Add(start+":"+end+":"+after))continue;
                patches.Add(new Patch {rule="smcs-loop-condition-body-guard",start=start,end=end,before=before,after=after,
                    distance=start<=point&&point<end?0:Math.Min(Math.Abs(start-point),Math.Abs(end-point))});
            }
        }
    }
}
