// Offline syntax and semantic member inventory. Never loads target code.
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static class OrderSyntax {
    static string TypeName(INamedTypeSymbol n) { return n.ContainingType!=null ? TypeName(n.ContainingType)+"/"+n.MetadataName :
        (n.ContainingNamespace.IsGlobalNamespace?"":n.ContainingNamespace.ToDisplayString()+".")+n.MetadataName; }
    static string TypeKey(ITypeSymbol t) {
        if(t is ITypeParameterSymbol p)return (p.TypeParameterKind==TypeParameterKind.Method?"!!":"!")+p.Ordinal;
        if(t is IArrayTypeSymbol a)return TypeKey(a.ElementType)+(a.Rank==1?"[]":"["+new string(',',a.Rank-1)+"]");
        if(t is IPointerTypeSymbol ptr)return TypeKey(ptr.PointedAtType)+"*";
        var n=(INamedTypeSymbol)t;return TypeName(n.OriginalDefinition)+(n.TypeArguments.Length==0?"":"<"+string.Join(",",n.TypeArguments.Select(TypeKey))+">");
    }
    static string MethodKey(IMethodSymbol m) {return TypeName(m.ContainingType)+"::"+m.MetadataName+(m.Arity==0?"":"``"+m.Arity)+"("+
        string.Join(",",m.Parameters.Select(p=>TypeKey(p.Type)+(p.RefKind==RefKind.None?"":"&")))+")->"+TypeKey(m.ReturnType);}
    static int Weight(MemberDeclarationSyntax m) {
        if(m is FieldDeclarationSyntax f)return f.Declaration.Variables.Count;
        if(m is EventFieldDeclarationSyntax e)return 3*e.Declaration.Variables.Count;
        if(m is BasePropertyDeclarationSyntax p)return 1+(p.AccessorList?.Accessors.Count??1)+
            ((m is PropertyDeclarationSyntax&&p.AccessorList!=null&&p.AccessorList.Accessors.All(a=>a.Body==null&&a.ExpressionBody==null))?1:0);
        return 1;
    }
    public static int Main(string[] args) {
        var input=JsonDocument.Parse(File.ReadAllText(args[0])).RootElement;
        var trees=input.GetProperty("sources").EnumerateArray().Select(v=>CSharpSyntaxTree.ParseText(File.ReadAllText(v.GetString()),path:v.GetString())).ToArray();
        var refs=input.GetProperty("references").EnumerateArray().Select(v=>MetadataReference.CreateFromFile(v.GetString())).ToArray();
        var compilation=CSharpCompilation.Create(input.GetProperty("assembly").GetString(),trees,refs,new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var sources=new List<object>();
        foreach(var tree in trees) {
            var model=compilation.GetSemanticModel(tree,true);var types=new List<object>();
            foreach(var node in tree.GetRoot().DescendantNodes().Where(n=>n is BaseTypeDeclarationSyntax||n is DelegateDeclarationSyntax)) {
                var symbol=(INamedTypeSymbol)model.GetDeclaredSymbol(node);var members=new List<object>();int slot=0;
                var declarations=node is TypeDeclarationSyntax td?td.Members.Cast<SyntaxNode>():
                    node is EnumDeclarationSyntax en?en.Members.Cast<SyntaxNode>():Enumerable.Empty<SyntaxNode>();
                foreach(var declaration in declarations) {
                    var entity=model.GetDeclaredSymbol(declaration);var methods=new List<string>();var fields=new List<string>();var accessors=new List<object>();
                    string name=entity?.MetadataName;
                    if(entity is IMethodSymbol method)methods.Add(MethodKey(method));
                    if(entity is IPropertySymbol property) {
                        if(declaration is BasePropertyDeclarationSyntax ps&&ps.AccessorList!=null) {
                            foreach(var accessor in ps.AccessorList.Accessors) {
                                var methodSymbol=(IMethodSymbol)model.GetDeclaredSymbol(accessor);string key=MethodKey(methodSymbol);methods.Add(key);
                                accessors.Add(new {id=key,full_start=accessor.FullSpan.Start,full_end=accessor.FullSpan.End});
                            }
                        } else if(property.GetMethod!=null)methods.Add(MethodKey(property.GetMethod));
                    }
                    if(entity is IEventSymbol ev&&declaration is EventDeclarationSyntax ed) {
                        foreach(var accessor in ed.AccessorList.Accessors) {
                            var methodSymbol=(IMethodSymbol)model.GetDeclaredSymbol(accessor);string key=MethodKey(methodSymbol);methods.Add(key);
                            accessors.Add(new {id=key,full_start=accessor.FullSpan.Start,full_end=accessor.FullSpan.End});
                        }
                    }
                    if(declaration is FieldDeclarationSyntax field)fields.AddRange(field.Declaration.Variables.Select(v=>v.Identifier.ValueText));
                    if(declaration is EventFieldDeclarationSyntax eventField) {
                        foreach(var v in eventField.Declaration.Variables) {
                            fields.Add(v.Identifier.ValueText);var es=(IEventSymbol)model.GetDeclaredSymbol(v);
                            if(es.AddMethod!=null)methods.Add(MethodKey(es.AddMethod));if(es.RemoveMethod!=null)methods.Add(MethodKey(es.RemoveMethod));
                        }
                    }
                    if(declaration is EnumMemberDeclarationSyntax em)fields.Add(em.Identifier.ValueText);
                    if(fields.Count>0)name=string.Join(",",fields);
                    int weight=declaration is MemberDeclarationSyntax md?Weight(md):1;
                    members.Add(new {ordinal=members.Count,roslyn_slot=slot,weight,kind=declaration.Kind().ToString(),name,field_names=fields,method_ids=methods,accessors,
                        start=declaration.SpanStart,end=declaration.Span.End,full_start=declaration.FullSpan.Start,full_end=declaration.FullSpan.End});
                    slot+=weight;
                }
                types.Add(new {name=TypeName(symbol),kind=node.Kind().ToString(),parent=symbol.ContainingType==null?null:TypeName(symbol.ContainingType),
                    start=node.SpanStart,end=node.Span.End,full_start=node.FullSpan.Start,full_end=node.FullSpan.End,members});
            }
            sources.Add(new {path=tree.FilePath,text=tree.GetText().ToString(),types});
        }
        var errors=compilation.GetDiagnostics().Where(d=>d.Severity==DiagnosticSeverity.Error).Select(d=>d.ToString()).ToArray();
        Console.WriteLine(JsonSerializer.Serialize(new {sources,errors}));return 0;
    }
}
