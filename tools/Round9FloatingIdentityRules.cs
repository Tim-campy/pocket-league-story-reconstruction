// Source-only explicit Single identity casts, one parameter occurrence per patch.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static partial class Round7TransformSyntax {
    static void AddRound9FloatingIdentityRules(JsonElement input,string text,StatementSyntax mapped,BlockSyntax methodBody,
        SemanticModel model,int point,List<Patch> patches,HashSet<string> seen){
        if(!input.TryGetProperty("round7_cycle_rule",out var rule)||rule.GetString()!="d9-explicit-float-identity-parameter")return;
        if(!input.TryGetProperty("round9_identity_conversion_opcode",out var opcode)||opcode.GetString()!="conv.r4")return;
        if(!input.TryGetProperty("round9_identity_parameter_ordinal",out var ordinal)||!ordinal.TryGetInt32(out int index)||index<0)return;
        var scope=mapped??(StatementSyntax)methodBody;
        foreach(var expression in scope.DescendantNodesAndSelf(n=>!(n is AnonymousFunctionExpressionSyntax)&&!(n is LocalFunctionStatementSyntax)).OfType<IdentifierNameSyntax>()){
            var parameter=model.GetSymbolInfo(expression).Symbol as IParameterSymbol;
            if(parameter==null||parameter.Ordinal!=index||parameter.RefKind!=RefKind.None||parameter.Type.SpecialType!=SpecialType.System_Single)continue;
            if(model.GetConstantValue(expression).HasValue)continue;
            SyntaxNode parent=expression.Parent;while(parent is ParenthesizedExpressionSyntax)parent=parent.Parent;
            if(parent is CastExpressionSyntax cast&&model.GetTypeInfo(cast.Type).Type?.SpecialType==SpecialType.System_Single)continue;
            if(parent is AssignmentExpressionSyntax assignment&&assignment.Left.Span.Contains(expression.Span))continue;
            if(parent is ArgumentSyntax argument&&argument.RefOrOutKeyword.RawKind!=0)continue;
            if(parent is PrefixUnaryExpressionSyntax prefix&&(prefix.IsKind(SyntaxKind.PreIncrementExpression)||prefix.IsKind(SyntaxKind.PreDecrementExpression)))continue;
            if(parent is PostfixUnaryExpressionSyntax postfix&&(postfix.IsKind(SyntaxKind.PostIncrementExpression)||postfix.IsKind(SyntaxKind.PostDecrementExpression)))continue;
            if(expression.Ancestors().TakeWhile(a=>a!=scope).OfType<InvocationExpressionSyntax>().Any(call=>call.Expression.ToString()=="nameof"))continue;
            string before=text.Substring(expression.SpanStart,expression.Span.Length),after="(float)("+before+")";
            string key=rule.GetString()+":"+expression.SpanStart;if(!seen.Add(key))continue;
            patches.Add(new Patch{rule=rule.GetString(),start=expression.SpanStart,end=expression.Span.End,before=before,after=after,distance=Math.Abs(point-expression.SpanStart)});
        }
    }
}
