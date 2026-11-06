// Search screening only. Screened assemblies are never verification artifacts.
using System;
using System.IO;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static class Round4ScreenSyntax {
    sealed class SkeletonRewriter : CSharpSyntaxRewriter {
        public int Bodies;
        public override SyntaxNode VisitBlock(BlockSyntax block) {
            if(block.Parent is BaseMethodDeclarationSyntax || block.Parent is AccessorDeclarationSyntax) {
                Bodies++;
                return SyntaxFactory.ParseStatement("{ throw new global::System.NotImplementedException(); }").WithTriviaFrom(block);
            }
            return base.VisitBlock(block);
        }
    }
    public static void Main(string[] args) {
        var manifest=JsonDocument.Parse(File.ReadAllText(args[0])).RootElement;
        string folder=Path.GetFullPath(args[1]);int bodies=0,files=0;
        foreach(var item in manifest.GetProperty("sources").EnumerateArray()) {
            string logical=item.ValueKind==JsonValueKind.String?item.GetString():item.GetProperty("path").GetString();
            string input=item.ValueKind==JsonValueKind.Object&&item.TryGetProperty("input_path",out var path)?path.GetString():logical;
            var tree=CSharpSyntaxTree.ParseText(File.ReadAllText(input),path:input);
            var rewriter=new SkeletonRewriter();var root=rewriter.Visit(tree.GetRoot());
            string output=Path.Combine(folder,logical);
            Directory.CreateDirectory(Path.GetDirectoryName(output));
            File.WriteAllText(output,root.ToFullString(),new System.Text.UTF8Encoding(false));
            files++;bodies+=rewriter.Bodies;
        }
        Console.WriteLine(JsonSerializer.Serialize(new{files=files,stubbed_bodies=bodies,screening_only=true}));
    }
}
