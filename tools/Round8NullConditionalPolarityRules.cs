// Original call-first IL chooses the negative equality form of a ternary.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static partial class Round7TransformSyntax {
    static void AddRound8NullConditionalPolarityRules(JsonElement input,string text,StatementSyntax mapped,BlockSyntax methodBody,
        SemanticModel model,int point,List<Patch> patches,HashSet<string> seen) {
        string selected=input.TryGetProperty("round7_cycle_rule",out var family)?family.GetString():"";
        bool learned=input.TryGetProperty("round7_cycle_rules",out var active)&&active.ValueKind==JsonValueKind.Array
            &&active.EnumerateArray().Any(value=>value.ValueKind==JsonValueKind.String&&value.GetString()=="nullable-coalesce-negative-ternary");
        if(selected!="nullable-coalesce-negative-ternary"&&!learned)return;
        if(!input.TryGetProperty("compiler",out var compiler)||compiler.GetString()!="roslyn")return;
        if(!input.TryGetProperty("original_nullable_call_first",out var evidence)||evidence.ValueKind!=JsonValueKind.True)return;
        foreach(var coalesce in mapped.DescendantNodesAndSelf().OfType<BinaryExpressionSyntax>().Where(n=>n.IsKind(SyntaxKind.CoalesceExpression))) {
            var expression=Unwrap(coalesce.Left);
            if(!(expression is ConditionalAccessExpressionSyntax conditional))continue;
            var receiver=conditional.Expression;var symbol=model.GetSymbolInfo(receiver).Symbol;
            bool stable=symbol is ILocalSymbol||symbol is IParameterSymbol parameter&&parameter.RefKind==RefKind.None
                ||symbol is IFieldSymbol field&&!field.IsVolatile&&(receiver is IdentifierNameSyntax
                    ||receiver is MemberAccessExpressionSyntax access&&(access.Expression is ThisExpressionSyntax||access.Expression is BaseExpressionSyntax));
            if(!stable||model.GetTypeInfo(receiver).Type?.IsReferenceType!=true)continue;
            var result=model.GetTypeInfo(conditional.WhenNotNull).Type;
            if(result==null||!result.IsValueType||result.SpecialType==SpecialType.System_Void
                ||result is INamedTypeSymbol named&&named.OriginalDefinition.SpecialType==SpecialType.System_Nullable_T)continue;
            if(!model.GetConstantValue(coalesce.Right).HasValue)continue;
            string accessText=conditional.WhenNotNull.ToString();
            if(!(accessText.StartsWith(".")||accessText.StartsWith("[")))continue;
            // The pinned emitter emits Alternative before Consequence. A true
            // null condition therefore leaves the non-null call as Alternative.
            // Object cast deliberately bypasses overloaded receiver equality.
            string after="(((object)("+receiver+") == null) ? ("+coalesce.Right+") : "+receiver+accessText+")";
            int start=coalesce.SpanStart,end=coalesce.Span.End;
            if(!seen.Add(start+":"+end+":"+after))continue;
            patches.Add(new Patch{rule="nullable-coalesce-negative-ternary",start=start,end=end,
                before=text.Substring(start,end-start),after=after,distance=start<=point&&point<end?0:Math.Min(Math.Abs(start-point),Math.Abs(end-point))});
        }
    }
}
