// Opt-in source declaration calculation; the frozen model and IL verifier remain mandatory.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static partial class Round7TransformSyntax {
    static bool Round7ForDeclarationConflict(BlockSyntax scope,ForStatementSyntax loop,VariableDeclaratorSyntax variable) {
        string name=variable.Identifier.ValueText;
        foreach(var node in scope.DescendantNodes()) {
            if(node is VariableDeclaratorSyntax other && other!=variable && other.Identifier.ValueText==name)return true;
            if(node is ForEachStatementSyntax each && each.Identifier.ValueText==name)return true;
            if(node is CatchDeclarationSyntax caught && caught.Identifier.ValueText==name)return true;
            if(node is ParameterSyntax parameter && parameter.Identifier.ValueText==name)return true;
            // Moving a declaration into this block can change binding even in
            // statements before its textual position (e.g. an unqualified field).
            if(node is IdentifierNameSyntax identifier && identifier.Identifier.ValueText==name && !loop.Span.Contains(identifier.Span)) {
                if(identifier.Parent is MemberAccessExpressionSyntax member && member.Name==identifier)continue;
                return true;
            }
        }
        return false;
    }
    static void AddRound7RoslynDeclarationRules(JsonElement input,string text,StatementSyntax mapped,BlockSyntax methodBody,
        SemanticModel model,int point,List<Patch> patches,HashSet<string> seen) {
        if(!input.TryGetProperty("round7_cycle_rule",out var selected)||selected.GetString()!="roslyn-for-declaration-immediately-before")return;
        string compiler=input.TryGetProperty("compiler",out var c)?c.GetString():"";
        if(compiler!=""&&compiler!="roslyn"&&!compiler.StartsWith("roslyn-"))return;
        if(methodBody.DescendantNodes().Any(n=>n is YieldStatementSyntax || n is AwaitExpressionSyntax))return;
        foreach(var loop in mapped.DescendantNodesAndSelf().OfType<ForStatementSyntax>()) {
            var declaration=loop.Declaration;
            if(!(loop.Parent is BlockSyntax scope)||declaration==null||declaration.Variables.Count!=1||declaration.Type.IsVar||loop.ContainsDirectives)continue;
            var variable=declaration.Variables[0];var symbol=model.GetDeclaredSymbol(variable) as ILocalSymbol;
            if(variable.Initializer==null||symbol==null||symbol.Type.TypeKind==TypeKind.Error||symbol.RefKind!=RefKind.None)continue;
            if(loop.DescendantNodes().Any(n=>n is AnonymousFunctionExpressionSyntax || n is LocalFunctionStatementSyntax))continue;
            if(scope.DescendantNodes().Any(n=>n is GotoStatementSyntax || n is LabeledStatementSyntax))continue;
            if(Round7ForDeclarationConflict(scope,loop,variable))continue;
            int a=declaration.Type.SpanStart,b=declaration.Type.Span.End;
            string withoutType=text.Substring(loop.SpanStart,a-loop.SpanStart)+text.Substring(b,loop.Span.End-b);
            string after=declaration.Type+" "+variable.Identifier.Text+";\n"+withoutType;
            string key=loop.SpanStart+":"+loop.Span.End+":"+after;
            if(seen.Add(key))patches.Add(new Patch{rule="roslyn-for-declaration-immediately-before",start=loop.SpanStart,end=loop.Span.End,
                before=text.Substring(loop.SpanStart,loop.Span.Length),after=after,distance=loop.Span.Contains(point)?0:Math.Abs(loop.SpanStart-point)});
        }
    }
}
