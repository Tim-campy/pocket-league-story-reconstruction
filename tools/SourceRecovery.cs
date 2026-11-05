// Offline source variants and syntax/symbol spans. Never executes target code.
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using Lang=ICSharpCode.Decompiler.CSharp.LanguageVersion;

public static class SourceRecovery {
    static string TypeKey(ITypeSymbol type) {
        if(type is ITypeParameterSymbol p) return (p.TypeParameterKind==TypeParameterKind.Method?"!!":"!")+p.Ordinal;
        if(type is IArrayTypeSymbol a) return TypeKey(a.ElementType)+(a.Rank==1?"[]":"["+new string(',',a.Rank-1)+"]");
        if(type is IPointerTypeSymbol ptr) return TypeKey(ptr.PointedAtType)+"*";
        var n=(INamedTypeSymbol)type;
        string name=TypeName(n.OriginalDefinition);
        if(n.TypeArguments.Length>0) name+="<"+string.Join(",",n.TypeArguments.Select(TypeKey))+">";
        return name;
    }
    static string TypeName(INamedTypeSymbol n) {
        return n.ContainingType!=null ? TypeName(n.ContainingType)+"/"+n.MetadataName :
            (n.ContainingNamespace.IsGlobalNamespace?"":n.ContainingNamespace.ToDisplayString()+".")+n.MetadataName;
    }
    static string MethodKey(IMethodSymbol method) {
        string arity=method.Arity==0?"":"``"+method.Arity;
        return TypeName(method.ContainingType)+"::"+method.MetadataName+arity+"("+
            string.Join(",",method.Parameters.Select(p=>TypeKey(p.Type)+(p.RefKind==RefKind.None?"":"&")))+")->"+TypeKey(method.ReturnType);
    }
    static void Write(object value) { Console.WriteLine(JsonSerializer.Serialize(value)); }
    static int MemberWeight(MemberDeclarationSyntax member) {
        if(member is FieldDeclarationSyntax f) return f.Declaration.Variables.Count;
        if(member is EventFieldDeclarationSyntax e) return 3*e.Declaration.Variables.Count;
        if(member is BasePropertyDeclarationSyntax p) {
            int accessors=p.AccessorList?.Accessors.Count??1;
            bool automatic=member is PropertyDeclarationSyntax && p.AccessorList!=null &&
                p.AccessorList.Accessors.All(a=>a.Body==null&&a.ExpressionBody==null);
            return 1+accessors+(automatic?1:0);
        }
        return 1;
    }
    public static int Main(string[] args) {
        if(args[0]=="settings") {
            var settings=new DecompilerSettings(Lang.CSharp6);
            Write(settings.GetType().GetProperties().Where(p=>p.PropertyType==typeof(bool)&&p.CanWrite)
                .ToDictionary(p=>p.Name,p=>p.GetValue(settings)));return 0;
        }
        if(args[0]=="members") {
            var manifest=JsonDocument.Parse(File.ReadAllText(args[1])).RootElement;
            var trees=manifest.GetProperty("sources").EnumerateArray().Select(p=>CSharpSyntaxTree.ParseText(
                File.ReadAllText(p.GetString()),path:p.GetString())).ToArray();
            var references=manifest.GetProperty("references").EnumerateArray().Select(p=>MetadataReference.CreateFromFile(p.GetString())).ToArray();
            var compilation=CSharpCompilation.Create("KairoLibrary",trees,references,new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            var output=new List<object>();
            foreach(var tree in trees) {
                var model=compilation.GetSemanticModel(tree,true);var types=new List<object>();
                foreach(var type in tree.GetRoot().DescendantNodes().OfType<TypeDeclarationSyntax>()) {
                    var symbol=model.GetDeclaredSymbol(type);var members=new List<object>();
                    int slot=0;
                    for(int index=0;index<type.Members.Count;index++) {
                        var member=type.Members[index];var entity=model.GetDeclaredSymbol(member);
                        var keys=new List<string>();
                        if(entity is IMethodSymbol method) keys.Add(MethodKey(method));
                        if(entity is IPropertySymbol prop) {
                            if(prop.GetMethod!=null) keys.Add(MethodKey(prop.GetMethod));
                            if(prop.SetMethod!=null) keys.Add(MethodKey(prop.SetMethod));
                        }
                        string name=entity?.MetadataName;
                        if(member is FieldDeclarationSyntax field) name=string.Join(",",field.Declaration.Variables.Select(v=>v.Identifier.ValueText));
                        members.Add(new {ordinal=index,roslyn_slot=slot,weight=MemberWeight(member),kind=member.Kind().ToString(),name,method_ids=keys,
                            start=member.SpanStart,end=member.Span.End,full_start=member.FullSpan.Start,full_end=member.FullSpan.End});
                        slot+=MemberWeight(member);
                    }
                    types.Add(new {name=TypeName(symbol),members});
                }
                output.Add(new {path=tree.FilePath,text=tree.GetText().ToString(),types});
            }
            var errors=compilation.GetDiagnostics().Where(d=>d.Severity==DiagnosticSeverity.Error).Select(d=>d.ToString()).Take(12).ToArray();
            Write(new {sources=output,errors});return 0;
        }
        if(args[0]=="variant") {
            string input=Path.GetFullPath(args[1]),unity=args[3];
            var settings=new DecompilerSettings(Lang.CSharp6) {UseDebugSymbols=false,ShowXmlDocumentation=false,ThrowOnAssemblyResolveErrors=true};
            var flags=JsonDocument.Parse(File.ReadAllText(args[4])).RootElement;
            foreach(var property in flags.EnumerateObject()) {
                var info=settings.GetType().GetProperty(property.Name);
                if(info==null||info.PropertyType!=typeof(bool)||!info.CanWrite) throw new ArgumentException("Invalid boolean setting: "+property.Name);
                info.SetValue(settings,property.Value.GetBoolean());
            }
            var resolver=new UniversalAssemblyResolver(input,true,".NETFramework,Version=v3.5",null,PEStreamOptions.PrefetchEntireImage,MetadataReaderOptions.Default);
            foreach(var directory in resolver.GetSearchDirectories()) resolver.RemoveSearchDirectory(directory);
            resolver.AddSearchDirectory(Path.Combine(unity,"Editor/Data/Mono/lib/mono/2.0"));
            resolver.AddSearchDirectory(Path.GetDirectoryName(input));
            resolver.AddSearchDirectory(Path.Combine(unity,"Editor/Data/Mono/lib/mono/unity"));
            var decompiler=new CSharpDecompiler(input,resolver,settings);
            var manifest=JsonDocument.Parse(File.ReadAllText(args[2])).RootElement;int count=0;
            foreach(var entry in manifest.EnumerateArray()) {
                string type=entry.GetProperty("type").GetString(),output=entry.GetProperty("output").GetString();
                if(File.Exists(output)) throw new IOException("Refusing overwrite: "+output);
                Directory.CreateDirectory(Path.GetDirectoryName(output));
                File.WriteAllText(output,decompiler.DecompileTypeAsString(new FullTypeName(type)));count++;
            }
            Write(new {decompiled_types=count,flags=JsonSerializer.Deserialize<Dictionary<string,bool>>(flags.GetRawText())});return 0;
        }
        throw new ArgumentException("Expected settings, members or variant");
    }
}
