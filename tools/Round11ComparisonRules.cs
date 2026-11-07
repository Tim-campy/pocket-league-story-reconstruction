// Primitive operand mirroring only. Preserve any enclosing explicit negation.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static partial class Round7TransformSyntax {
    static ExpressionSyntax R11Operand(ExpressionSyntax expression) {
        while (expression is ParenthesizedExpressionSyntax p) expression = p.Expression;
        return expression;
    }
    static void AddRound11ComparisonRules(JsonElement input,string text,StatementSyntax mapped,BlockSyntax body,
        SemanticModel model,int point,List<Patch> patches,HashSet<string> seen) {
        if (!input.TryGetProperty("round7_cycle_rule",out var rule) ||
            !(rule.GetString() ?? "").StartsWith("r11-c5",StringComparison.Ordinal)) return;
        var scope=mapped ?? (StatementSyntax)body;
        var mirrors=new Dictionary<string,string>{{"<",">"},{">","<"},{"<=",">="},{">=","<="},{"==","=="},{"!=","!="}};
        var allowed=new HashSet<SpecialType>{SpecialType.System_Int32,SpecialType.System_UInt32,
            SpecialType.System_Int64,SpecialType.System_UInt64,SpecialType.System_Single,SpecialType.System_Double};
        foreach(var binary in scope.DescendantNodesAndSelf(n => !(n is AnonymousFunctionExpressionSyntax) || n.FullSpan.Contains(point)).OfType<BinaryExpressionSyntax>()) {
            if (!mirrors.TryGetValue(binary.OperatorToken.Text,out var opposite)) continue;
            var left=R11Operand(binary.Left) as IdentifierNameSyntax;
            var right=R11Operand(binary.Right) as IdentifierNameSyntax;
            if(left==null || right==null || left.Identifier.ValueText==right.Identifier.ValueText) continue;
            var a=model.GetSymbolInfo(left).Symbol;var b=model.GetSymbolInfo(right).Symbol;
            if(!((a is ILocalSymbol) || (a is IParameterSymbol pa && pa.RefKind==RefKind.None)) ||
               !((b is ILocalSymbol) || (b is IParameterSymbol pb && pb.RefKind==RefKind.None))) continue;
            var ta=model.GetTypeInfo(left).Type;var tb=model.GetTypeInfo(right).Type;
            if(ta==null || tb==null || !allowed.Contains(ta.SpecialType) || !SymbolEqualityComparer.Default.Equals(ta,tb)) continue;
            var operation=model.GetSymbolInfo(binary).Symbol as IMethodSymbol;
            if(operation!=null && operation.MethodKind==MethodKind.UserDefinedOperator) continue;
            string before=text.Substring(binary.SpanStart,binary.Span.Length);
            string after="("+binary.Right.WithoutTrivia()+" "+opposite+" "+binary.Left.WithoutTrivia()+")";
            string name="r11-c5-mirror-primitive-operands";
            if(!seen.Add(name+":"+binary.SpanStart)) continue;
            patches.Add(new Patch{rule=name,start=binary.SpanStart,end=binary.Span.End,before=before,after=after,
                distance=Math.Abs(point-binary.SpanStart)});
        }
    }
}
