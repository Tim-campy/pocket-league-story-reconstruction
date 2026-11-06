// Parse-only inventory. This never compiles or executes game code.
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static class Round6RoslynSyntax {
    public static int Main(string[] args) {
        var rows = new List<object>();
        foreach (var path in args) {
            var text = File.ReadAllText(path);
            var tree = CSharpSyntaxTree.ParseText(text, path: path);
            foreach (var node in tree.GetRoot().DescendantNodes()) {
                var conditional = node as ConditionalAccessExpressionSyntax;
                var coalesce = node as BinaryExpressionSyntax;
                if (conditional == null && (coalesce == null || !coalesce.IsKind(SyntaxKind.CoalesceExpression))) continue;
                var method = node.Ancestors().OfType<BaseMethodDeclarationSyntax>().FirstOrDefault();
                string methodName = method is MethodDeclarationSyntax m ? m.Identifier.ValueText : method is ConstructorDeclarationSyntax c ? c.Identifier.ValueText : "<initializer-or-accessor>";
                rows.Add(new {
                    path, kind = conditional != null ? (conditional.WhenNotNull is ElementBindingExpressionSyntax ? "conditional-element-access" : "conditional-member-access") : "null-coalescing",
                    method = methodName, line = tree.GetLineSpan(node.Span).StartLinePosition.Line + 1,
                    start = node.Span.Start, end = node.Span.End, source = node.ToString(),
                    receiver = conditional?.Expression.ToString(), when_not_null = conditional?.WhenNotNull.ToString(),
                    left = coalesce?.Left.ToString(), right = coalesce?.Right.ToString()
                });
            }
        }
        Console.WriteLine(JsonSerializer.Serialize(rows));
        return 0;
    }
}
