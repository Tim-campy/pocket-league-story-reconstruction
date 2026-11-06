// Opt-in source hypothesis. Original Concat(String,String,String,String) is a required hint.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static partial class Round7TransformSyntax {
    static bool ConcatBuiltinString(ExpressionSyntax expression,SemanticModel model) {
        var info=model.GetTypeInfo(expression);
        return (info.Type??info.ConvertedType)?.SpecialType==SpecialType.System_String;
    }
    static bool ConcatFlattenStrings(ExpressionSyntax expression,SemanticModel model,List<ExpressionSyntax> parts) {
        if(expression is ParenthesizedExpressionSyntax paren && paren.Expression is BinaryExpressionSyntax)
            return ConcatFlattenStrings(paren.Expression,model,parts);
        if(expression is BinaryExpressionSyntax binary && binary.IsKind(SyntaxKind.AddExpression) && ConcatBuiltinString(binary,model)) {
            if(model.GetSymbolInfo(binary).Symbol is IMethodSymbol method && method.MethodKind==MethodKind.UserDefinedOperator)return false;
            return ConcatFlattenStrings(binary.Left,model,parts)&&ConcatFlattenStrings(binary.Right,model,parts);
        }
        if(!ConcatBuiltinString(expression,model))return false;
        parts.Add(expression);return true;
    }
    static void AddRound7StringConcatRules(JsonElement input,string text,StatementSyntax mapped,BlockSyntax body,
        SemanticModel model,int point,List<Patch> patches,HashSet<string> seen) {
        if(!input.TryGetProperty("round7_cycle_rule",out var selected)||selected.GetString()!="explicit-string-concat-preserve-literals")return;
        if(!input.TryGetProperty("original_string_concat_arity",out var arity)||!arity.TryGetInt32(out var count)||count!=4)return;
        foreach(var binary in mapped.DescendantNodesAndSelf().OfType<BinaryExpressionSyntax>().Where(b=>b.IsKind(SyntaxKind.AddExpression))) {
            SyntaxNode parent=binary.Parent;while(parent is ParenthesizedExpressionSyntax)parent=parent.Parent;
            if(parent is BinaryExpressionSyntax enclosing&&enclosing.IsKind(SyntaxKind.AddExpression)&&ConcatBuiltinString(enclosing,model))continue;
            var parts=new List<ExpressionSyntax>();if(!ConcatFlattenStrings(binary,model,parts)||parts.Count!=4)continue;
            // Only the folding pattern actually diagnosed in the original is targeted.
            bool adjacent=false;
            for(int i=1;i<parts.Count;i++){var left=model.GetConstantValue(parts[i-1]);var right=model.GetConstantValue(parts[i]);if(left.HasValue&&right.HasValue&&left.Value is string&&right.Value is string)adjacent=true;}
            if(!adjacent)continue;
            string after="global::System.String.Concat("+string.Join(", ",parts.Select(p=>p.ToString()))+")";
            string key=binary.SpanStart+":"+binary.Span.End+":"+after;
            if(seen.Add(key))patches.Add(new Patch{rule="explicit-string-concat-preserve-literals",start=binary.SpanStart,end=binary.Span.End,
                before=text.Substring(binary.SpanStart,binary.Span.Length),after=after,distance=binary.Span.Contains(point)?0:Math.Abs(binary.SpanStart-point)});
        }
    }
}
