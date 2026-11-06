// Opt-in source hypothesis only. The complete IL verifier decides acceptance.
using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
public static partial class Round7TransformSyntax {
    static bool Round8StableCompoundReceiver(ExpressionSyntax expression,SemanticModel model) {
        var value=Unwrap(expression);
        if(value is ThisExpressionSyntax||value is BaseExpressionSyntax)return true;
        if(value is IdentifierNameSyntax) {
            var symbol=model.GetSymbolInfo(value).Symbol;
            return symbol is ILocalSymbol local&&!local.IsConst || symbol is IParameterSymbol parameter&&parameter.RefKind==RefKind.None
                || symbol is IFieldSymbol field&&!field.IsVolatile || symbol is INamedTypeSymbol;
        }
        if(value is MemberAccessExpressionSyntax member) {
            var field=model.GetSymbolInfo(member).Symbol as IFieldSymbol;
            return field!=null&&!field.IsVolatile&&(field.IsStatic||Round8StableCompoundReceiver(member.Expression,model));
        }
        return false;
    }
    static void AddRound8CompoundRules(JsonElement input,string text,StatementSyntax mapped,BlockSyntax methodBody,
        SemanticModel model,int point,List<Patch> patches,HashSet<string> seen) {
        bool selected=input.TryGetProperty("round7_cycle_rule",out var cycle)&&cycle.GetString()=="expand-compound-field-multiply";
        bool enabled=input.TryGetProperty("round8_expand_compound_fields",out var flag)?flag.ValueKind==JsonValueKind.True:selected;
        if(!enabled)return;
        string compiler=input.TryGetProperty("compiler",out var c)?c.GetString():"";
        if(compiler!="smcs"&&compiler!="mono-smcs")return;
        if(mapped is not ExpressionStatementSyntax statement||Unwrap(statement.Expression) is not AssignmentExpressionSyntax assignment
            ||!assignment.IsKind(SyntaxKind.MultiplyAssignmentExpression))return;
        if(Unwrap(assignment.Left) is not MemberAccessExpressionSyntax member)return;
        var field=model.GetSymbolInfo(member).Symbol as IFieldSymbol;
        if(field==null||field.IsVolatile||field.IsReadOnly||field.IsConst)return;
        if(field.Type.SpecialType!=SpecialType.System_Single&&field.Type.SpecialType!=SpecialType.System_Double)return;
        if(!SymbolEqualityComparer.Default.Equals(field.Type,model.GetTypeInfo(assignment.Right).Type))return;
        if(!field.IsStatic&&!Round8StableCompoundReceiver(member.Expression,model))return;
        // Parentheses keep RHS ternaries, assignments, and arithmetic grouped.
        string lhs=assignment.Left.ToString();
        string after=lhs+" = ("+lhs+") * ("+assignment.Right+")";
        int start=assignment.SpanStart,end=assignment.Span.End;
        if(start<methodBody.SpanStart||end>methodBody.Span.End||start>=end)return;
        string before=text.Substring(start,end-start);
        if(before==after||!seen.Add(start+":"+end+":"+after))return;
        patches.Add(new Patch{rule="expand-compound-field-multiply",start=start,end=end,before=before,after=after,
            distance=start<=point&&point<end?0:Math.Min(Math.Abs(start-point),Math.Abs(end-point))});
    }
}
