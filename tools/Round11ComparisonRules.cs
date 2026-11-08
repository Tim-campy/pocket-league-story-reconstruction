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
    static bool R11PureNumericOperand(ExpressionSyntax expression,SemanticModel model,out bool identifier) {
        expression=R11Operand(expression);identifier=expression is IdentifierNameSyntax;
        if(identifier) {
            var symbol=model.GetSymbolInfo(expression).Symbol;
            return symbol is ILocalSymbol || symbol is IParameterSymbol p && p.RefKind==RefKind.None;
        }
        if(expression is PrefixUnaryExpressionSyntax unary &&
           (unary.IsKind(SyntaxKind.UnaryMinusExpression)||unary.IsKind(SyntaxKind.UnaryPlusExpression)))
            expression=R11Operand(unary.Operand);
        return expression is LiteralExpressionSyntax literal && literal.IsKind(SyntaxKind.NumericLiteralExpression);
    }
    static void AddRound11ComparisonRules(JsonElement input,string text,StatementSyntax mapped,BlockSyntax body,
        SemanticModel model,int point,List<Patch> patches,HashSet<string> seen) {
        bool enabled=input.TryGetProperty("round7_cycle_rule",out var rule) &&
            (rule.GetString() ?? "").StartsWith("r11-c5",StringComparison.Ordinal);
        if(input.TryGetProperty("round7_cycle_rules",out var retained) && retained.ValueKind==JsonValueKind.Array)
            enabled |= retained.EnumerateArray().Any(r => (r.GetString() ?? "").StartsWith("r11-c5",StringComparison.Ordinal));
        if(!enabled) return;
        var scope=mapped ?? (StatementSyntax)body;
        var mirrors=new Dictionary<string,string>{{"<",">"},{">","<"},{"<=",">="},{">=","<="},{"==","=="},{"!=","!="}};
        var allowed=new HashSet<SpecialType>{SpecialType.System_Int32,SpecialType.System_UInt32,
            SpecialType.System_Int64,SpecialType.System_UInt64,SpecialType.System_Single,SpecialType.System_Double};
        foreach(var binary in scope.DescendantNodesAndSelf(n => !(n is AnonymousFunctionExpressionSyntax) || n.FullSpan.Contains(point)).OfType<BinaryExpressionSyntax>()) {
            if (!mirrors.TryGetValue(binary.OperatorToken.Text,out var opposite)) continue;
            var left=R11Operand(binary.Left);var right=R11Operand(binary.Right);
            if(!R11PureNumericOperand(left,model,out var leftId)||!R11PureNumericOperand(right,model,out var rightId)||!(leftId||rightId)) continue;
            if(leftId&&rightId&&((IdentifierNameSyntax)left).Identifier.ValueText==((IdentifierNameSyntax)right).Identifier.ValueText) continue;
            var la=model.GetTypeInfo(left);var rb=model.GetTypeInfo(right);
            var ta=la.ConvertedType;var tb=rb.ConvertedType;
            if(ta==null || tb==null || !allowed.Contains(ta.SpecialType) || !SymbolEqualityComparer.Default.Equals(ta,tb)) continue;
            if((leftId&&(la.Type==null||!allowed.Contains(la.Type.SpecialType))) ||
               (rightId&&(rb.Type==null||!allowed.Contains(rb.Type.SpecialType))))continue;
            if(model.GetConversion(binary.Left).IsUserDefined||model.GetConversion(binary.Right).IsUserDefined)continue;
            // Keep mixed named operands outside this conservative family. A
            // literal may adopt its comparison's converted primitive type.
            if(leftId&&rightId&&!SymbolEqualityComparer.Default.Equals(la.Type,rb.Type))continue;
            if((!leftId&&!model.GetConstantValue(left).HasValue)||(!rightId&&!model.GetConstantValue(right).HasValue))continue;
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
