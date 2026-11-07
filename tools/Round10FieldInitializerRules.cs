// Source-only field -> implicit .cctor mapping. No constructor is synthesized.
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
public static partial class Round7TransformSyntax {
    static string StaticInitializerOwner(Microsoft.CodeAnalysis.IFieldSymbol field) {
        if(field==null || !field.IsStatic || field.IsConst)return null;
        return TypeName(field.ContainingType)+"::.cctor()->System.Void";
    }
    static object DescribeStaticInitializers(System.Text.Json.JsonElement input,
            Microsoft.CodeAnalysis.SyntaxNode root,Microsoft.CodeAnalysis.SemanticModel model) {
        string expected=input.GetProperty("method_id").GetString();
        var fields=new System.Collections.Generic.List<object>();
        foreach(var variable in root.DescendantNodes().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclaratorSyntax>()) {
            if(!(variable.Parent?.Parent is Microsoft.CodeAnalysis.CSharp.Syntax.FieldDeclarationSyntax declaration)
                || variable.Initializer==null)continue;
            var symbol=model.GetDeclaredSymbol(variable) as Microsoft.CodeAnalysis.IFieldSymbol;
            if(StaticInitializerOwner(symbol)!=expected)continue;
            var pos=variable.SyntaxTree.GetLineSpan(variable.Initializer.Value.Span).StartLinePosition;
            fields.Add(new {field_id=TypeName(symbol.ContainingType)+"::"+symbol.MetadataName,
                method_id=expected,start=variable.Initializer.Value.SpanStart,end=variable.Initializer.Value.Span.End,
                declaration_start=declaration.SpanStart,declaration_end=declaration.Span.End,
                line=pos.Line+1,column=pos.Character+1,expression=variable.Initializer.Value.ToString(),
                explicit_static_constructor=symbol.ContainingType.StaticConstructors.Any(c=>!c.IsImplicitlyDeclared)});
        }
        return new {method_id=expected,fields=fields,source_only=true,
            proof_required="Original and rebuilt .cctor stsfld anchor with this exact field identity"};
    }
    static object RespondStaticInitializer(System.Text.Json.JsonElement input,string text,
            Microsoft.CodeAnalysis.SyntaxNode node,Microsoft.CodeAnalysis.SemanticModel model,int point) {
        var variable=node.AncestorsAndSelf().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclaratorSyntax>().FirstOrDefault();
        if(variable==null || variable.Initializer==null
            || !(variable.Parent?.Parent is Microsoft.CodeAnalysis.CSharp.Syntax.FieldDeclarationSyntax declaration))
            return new {error="point_outside_method_or_static_initializer",offset=point};
        var field=model.GetDeclaredSymbol(variable) as Microsoft.CodeAnalysis.IFieldSymbol;
        string owner=StaticInitializerOwner(field);
        if(owner==null || !input.TryGetProperty("method_id",out var requested) || requested.GetString()!=owner)
            return new {error="static_initializer_owner_mismatch",offset=point};
        var expression=variable.Initializer.Value;
        var patches=new System.Collections.Generic.List<Patch>();
        foreach(var conditional in expression.DescendantNodesAndSelf().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.ConditionalExpressionSyntax>()) {
            string after="("+Negate(conditional.Condition,model)+") ? "+conditional.WhenFalse+" : "+conditional.WhenTrue;
            patches.Add(new Patch{rule="static-initializer-ternary-polarity",start=conditional.SpanStart,
                end=conditional.Span.End,before=text.Substring(conditional.SpanStart,conditional.Span.Length),after=after,
                distance=Distance(conditional,point)});
        }
        var pos=variable.SyntaxTree.GetLineSpan(expression.Span).StartLinePosition;
        return new {declaration_start=declaration.SpanStart,declaration_end=declaration.Span.End,
            disable_target_screening=true,
            mapped=new {start=expression.SpanStart,end=expression.Span.End,line=pos.Line+1,column=pos.Character+1,
                text=expression.ToString(),kind="StaticFieldInitializer",method_id=owner,point=point,
                field_id=TypeName(field.ContainingType)+"::"+field.MetadataName,
                declaration_start=declaration.SpanStart,declaration_end=declaration.Span.End},
            patches=patches,candidate_count=patches.Count};
    }
}
