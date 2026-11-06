// Syntax hypotheses only; original IL and the independent verifier decide.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static partial class Round7TransformSyntax {
    static void AddRound7RoslynRules(JsonElement input,string text,StatementSyntax mapped,BlockSyntax body,
        SemanticModel model,int point,List<Patch> patches,HashSet<string> seen) {
        string compiler=input.TryGetProperty("compiler",out var c)?c.GetString():"";
        if(compiler!="roslyn"&&!compiler.StartsWith("roslyn-"))return;
        Action<string,SyntaxNode,string> add=(rule,node,after)=> {
            if(node.SpanStart<body.SpanStart||node.Span.End>body.Span.End)return;
            string before=text.Substring(node.SpanStart,node.Span.Length);
            if(before==after||!seen.Add(node.SpanStart+":"+node.Span.End+":"+after))return;
            patches.Add(new Patch{rule=rule,start=node.SpanStart,end=node.Span.End,before=before,after=after,distance=Distance(node,point)});
        };
        var nodes=mapped.DescendantNodesAndSelf().Concat(mapped.Ancestors().TakeWhile(n=>n!=body)).Distinct().ToArray();
        foreach(var loop in nodes.OfType<ForStatementSyntax>()) {
            if(loop.Declaration?.Variables.Count!=1||loop.Initializers.Count!=0||loop.Incrementors.Count!=1)continue;
            var variable=loop.Declaration.Variables[0];
            if(variable.Initializer==null||!LiteralZero(variable.Initializer.Value,model))continue;
            var index=model.GetDeclaredSymbol(variable) as ILocalSymbol;
            if(index?.Type.SpecialType!=SpecialType.System_Int32)continue;
            if(!(Unwrap(loop.Condition) is BinaryExpressionSyntax less)||!less.IsKind(SyntaxKind.LessThanExpression)||!Same(model.GetSymbolInfo(less.Left).Symbol,index))continue;
            if(!(Unwrap(less.Right) is MemberAccessExpressionSyntax length)||length.Name.Identifier.ValueText!="Length")continue;
            var array=length.Expression;
            if(!(model.GetTypeInfo(array).Type is IArrayTypeSymbol arrayType)||arrayType.Rank!=1||!Round7Stable(array,model))continue;
            var increment=Unwrap(loop.Incrementors[0]);
            ExpressionSyntax operand=increment is PostfixUnaryExpressionSyntax post&&post.IsKind(SyntaxKind.PostIncrementExpression)?post.Operand:
                increment is PrefixUnaryExpressionSyntax pre&&pre.IsKind(SyntaxKind.PreIncrementExpression)?pre.Operand:null;
            if(operand==null||!Same(model.GetSymbolInfo(operand).Symbol,index))continue;
            var accesses=loop.Statement.DescendantNodesAndSelf().OfType<ElementAccessExpressionSyntax>().Where(e=>
                e.ArgumentList.Arguments.Count==1&&Same(model.GetSymbolInfo(e.Expression).Symbol,model.GetSymbolInfo(array).Symbol)&&
                Same(model.GetSymbolInfo(e.ArgumentList.Arguments[0].Expression).Symbol,index)).ToArray();
            if(accesses.Length==0)continue;
            var indexUses=loop.Statement.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>().Where(n=>Same(model.GetSymbolInfo(n).Symbol,index)).ToArray();
            if(indexUses.Any(n=>!accesses.Any(a=>a.ArgumentList.Span.Contains(n.Span))))continue;
            if(accesses.Any(a=>a.Parent is AssignmentExpressionSyntax assign&&assign.Left==a || a.Parent is ArgumentSyntax arg&&!arg.RefKindKeyword.IsKind(SyntaxKind.None)))continue;
            // Keep the original element local, avoiding a second copy and slot.
            if(!(loop.Statement is BlockSyntax block)||!(block.Statements.FirstOrDefault() is LocalDeclarationStatementSyntax first)||!SimpleLocal(first))continue;
            var element=first.Declaration.Variables[0];
            if(element.Initializer==null||!accesses.Any(a=>Unwrap(element.Initializer.Value)==a)||accesses.Length!=1)continue;
            var elementSymbol=model.GetDeclaredSymbol(element) as ILocalSymbol;
            if(elementSymbol==null||!Same(elementSymbol.Type,arrayType.ElementType))continue;
            var rest=block.WithStatements(block.Statements.RemoveAt(0));
            add("array-index-loop-foreach",loop,"foreach ("+first.Declaration.Type+" "+element.Identifier+" in "+array+") "+rest);
        }
        foreach(var declaration in nodes.OfType<LocalDeclarationStatementSyntax>().Where(SimpleLocal)) {
            var variable=declaration.Declaration.Variables[0];
            var local=model.GetDeclaredSymbol(variable) as ILocalSymbol;
            if(local==null||!local.Type.IsValueType||local.Type.TypeKind!=TypeKind.Struct||variable.Initializer==null||!(declaration.Parent is BlockSyntax block))continue;
            var initializer=Unwrap(variable.Initializer.Value);
            bool zero=initializer is DefaultExpressionSyntax || initializer.IsKind(SyntaxKind.DefaultLiteralExpression) ||
                initializer is ObjectCreationExpressionSyntax creation&&creation.ArgumentList?.Arguments.Count==0&&creation.Initializer==null;
            if(!zero)continue;
            var required=new HashSet<ISymbol>(local.Type.GetMembers().OfType<IFieldSymbol>().Where(f=>!f.IsStatic),SymbolEqualityComparer.Default);
            if(required.Count==0||required.Any(f=>((IFieldSymbol)f).IsReadOnly))continue;
            bool valid=true;int at=block.Statements.IndexOf(declaration)+1;
            while(required.Count>0&&at<block.Statements.Count) {
                var statement=block.Statements[at++] as ExpressionStatementSyntax;
                if(!(statement?.Expression is AssignmentExpressionSyntax assignment)||!assignment.IsKind(SyntaxKind.SimpleAssignmentExpression)||
                    !(assignment.Left is MemberAccessExpressionSyntax member)||!Same(model.GetSymbolInfo(member.Expression).Symbol,local)||
                    !(model.GetSymbolInfo(member).Symbol is IFieldSymbol field)||!required.Remove(field)||
                    assignment.Right.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>().Any(n=>Same(model.GetSymbolInfo(n).Symbol,local))) {valid=false;break;}
            }
            if(valid&&required.Count==0)add("struct-fieldwise-no-initializer",declaration,declaration.ReplaceNode(variable,variable.WithInitializer(null)).ToString());
        }
    }
}
