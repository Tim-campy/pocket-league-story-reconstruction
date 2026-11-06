// Five explicit Part-B source cases only. No IL extraction or binary rewriting.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static partial class Round7TransformSyntax {
    static ExpressionSyntax B9Strip(ExpressionSyntax expression){
        while(expression is ParenthesizedExpressionSyntax p)expression=p.Expression;
        return expression;
    }
    static string B9Text(string text,SyntaxNode node)=>text.Substring(node.SpanStart,node.Span.Length);
    static bool B9OnlyReturn(StatementSyntax statement)=>statement is ReturnStatementSyntax r&&r.Expression==null ||
        statement is BlockSyntax b&&b.Statements.Count==1&&b.Statements[0] is ReturnStatementSyntax rb&&rb.Expression==null;
    static bool B9OnlyContinue(StatementSyntax statement)=>statement is ContinueStatementSyntax ||
        statement is BlockSyntax b&&b.Statements.Count==1&&b.Statements[0] is ContinueStatementSyntax;
    static void B9Add(string rule,string text,int start,int end,string after,int point,List<Patch> patches,HashSet<string> seen){
        string before=text.Substring(start,end-start);if(before==after)return;
        string key=rule+":"+start+":"+end+":"+after;if(!seen.Add(key))return;
        patches.Add(new Patch{rule=rule,start=start,end=end,before=before,after=after,distance=Math.Abs(point-start)});
    }
    static string B9Statements(string text,IEnumerable<StatementSyntax> statements){
        var array=statements.ToArray();return array.Length==0?"":text.Substring(array[0].SpanStart,array.Last().Span.End-array[0].SpanStart);
    }
    static bool B9IntComparison(SemanticModel model,BinaryExpressionSyntax binary)=>
        model.GetTypeInfo(binary.Left).Type?.SpecialType==SpecialType.System_Int32&&
        model.GetTypeInfo(binary.Right).Type?.SpecialType==SpecialType.System_Int32;

    static void AddRound9PartBRules(JsonElement input,string text,StatementSyntax mapped,BlockSyntax methodBody,
        SemanticModel model,int point,List<Patch> patches,HashSet<string> seen){
        if(!input.TryGetProperty("round7_cycle_rule",out var hint)||hint.ValueKind!=JsonValueKind.String)return;
        string selected=hint.GetString();if(selected==null||!selected.StartsWith("b9-"))return;
        var declaration=methodBody.Parent as MethodDeclarationSyntax;if(declaration==null)return;
        var symbol=model.GetDeclaredSymbol(declaration);string owner=symbol?.ContainingType.ToDisplayString();
        string method=declaration.Identifier.ValueText,nl=text.Contains("\r\n")?"\r\n":"\n";
        if(methodBody.DescendantNodes().Any(n=>n is AnonymousFunctionExpressionSyntax||n is LocalFunctionStatementSyntax||n is YieldStatementSyntax||n is AwaitExpressionSyntax))return;
        if(selected=="b9-event-loop-split-guard"&&owner=="form.GameForm"&&method=="_update"){
            foreach(var guard in methodBody.DescendantNodes().OfType<IfStatementSyntax>()){
                if(!B9OnlyContinue(guard.Statement)||guard.Else!=null||!(guard.Parent is BlockSyntax body)||!(body.Parent is ForStatementSyntax loop))continue;
                var condition=B9Strip(guard.Condition) as BinaryExpressionSyntax;
                if(condition==null||!condition.IsKind(SyntaxKind.LogicalOrExpression)||!(B9Strip(condition.Right) is PrefixUnaryExpressionSyntax neg)||!neg.IsKind(SyntaxKind.LogicalNotExpression))continue;
                int index=body.Statements.IndexOf(guard);var tail=body.Statements.Skip(index+1).ToArray();
                if(index!=0||tail.Length!=2||!(tail[0] is ExpressionStatementSyntax)||!(tail[1] is ReturnStatementSyntax ret)||!ret.Expression.IsKind(SyntaxKind.FalseLiteralExpression))continue;
                if(!B9Text(text,condition.Left).Contains("eventData_")||!B9Text(text,neg.Operand).Contains("CheckTermScript"))continue;
                string after="if ("+B9Text(text,condition.Left)+") { continue; }"+nl+"if ("+B9Text(text,neg.Operand)+") {"+nl+B9Statements(text,tail)+nl+"}";
                B9Add(selected,text,guard.SpanStart,tail.Last().Span.End,after,point,patches,seen);
            }
        }
        if(selected=="b9-empty-arm-common-return"&&owner=="form.SubForm"&&method=="Init"){
            foreach(var guard in methodBody.DescendantNodes().OfType<IfStatementSyntax>()){
                var condition=B9Strip(guard.Condition) as BinaryExpressionSyntax;
                if(condition==null||!condition.IsKind(SyntaxKind.NotEqualsExpression)||!B9IntComparison(model,condition)||guard.Else!=null||!(guard.Parent is ElseClauseSyntax))continue;
                if(B9Text(text,condition.Left)!="type_"||B9Text(text,condition.Right)!="6")continue;
                // Preserve the existing common-return leaf; this creates no return
                // expression, loop, switch, goto or break in the reconstructed code.
                string positive=B9Text(text,condition.Left)+" == "+B9Text(text,condition.Right);
                string after="if ("+positive+") { } else "+B9Text(text,guard.Statement);
                B9Add(selected,text,guard.SpanStart,guard.Span.End,after,point,patches,seen);
            }
        }
        if(selected=="b9-guard-return-empty-positive-arm"&&owner=="form.SoccerForm"&&method=="Draw_newPlayerPopular"){
            foreach(var guard in methodBody.DescendantNodes().OfType<IfStatementSyntax>()){
                if(!B9OnlyReturn(guard.Statement)||guard.Else!=null||!(guard.Parent is BlockSyntax body)||!(body.Parent is IfStatementSyntax arm)||arm.Else==null)continue;
                var condition=B9Strip(guard.Condition) as BinaryExpressionSyntax;
                if(condition==null||!condition.IsKind(SyntaxKind.LessThanExpression)||!B9IntComparison(model,condition))continue;
                if(B9Text(text,condition.Left)!="stateCnt_"||!B9Text(text,condition.Right).Contains("PBONUS_NP_LV_POPULAR"))continue;
                if(!B9Text(text,arm.Condition).Contains("select_"))continue;
                int index=body.Statements.IndexOf(guard);var tail=body.Statements.Skip(index+1).ToArray();
                if(index!=0||tail.Length!=1||!(tail[0] is IfStatementSyntax)||!arm.Ancestors().OfType<ForStatementSyntax>().Any())continue;
                string after="if ("+B9Text(text,guard.Condition)+") { } else {"+nl+B9Statements(text,tail)+nl+"}";
                B9Add(selected,text,guard.SpanStart,tail.Last().Span.End,after,point,patches,seen);
            }
        }
        if(selected=="b9-positive-render-with-gui-else"&&owner=="kairo.unity.ui.Balloon"&&method=="Draw"){
            foreach(var outer in methodBody.Statements.OfType<IfStatementSyntax>()){
                if(outer.Else!=null||!(outer.Statement is BlockSyntax nonGUI)||!B9Text(text,outer.Condition).Contains("!g.IsGUI()"))continue;
                if(nonGUI.Statements.Count<3||!(nonGUI.Statements[0] is IfStatementSyntax guard)||!B9OnlyReturn(guard.Statement)||guard.Else!=null)continue;
                var condition=B9Strip(guard.Condition) as BinaryExpressionSyntax;
                if(condition==null||!condition.IsKind(SyntaxKind.EqualsExpression)||!condition.Right.IsKind(SyntaxKind.NullLiteralExpression)||B9Text(text,condition.Left)!="g.render_")continue;
                var op=model.GetSymbolInfo(condition).Symbol as IMethodSymbol;if(op?.MethodKind==MethodKind.UserDefinedOperator)continue;
                if(model.GetTypeInfo(condition.Left).Type?.IsReferenceType!=true)continue;
                if(!(nonGUI.Statements.Last() is ReturnStatementSyntax last)||last.Expression!=null)continue;
                int index=methodBody.Statements.IndexOf(outer);var guiTail=methodBody.Statements.Skip(index+1).ToArray();if(guiTail.Length==0)continue;
                string after="if ("+B9Text(text,outer.Condition)+") {"+nl+"if ("+B9Text(text,condition.Left)+" != null) {"+nl+
                    B9Statements(text,nonGUI.Statements.Skip(1))+nl+"}"+nl+"} else {"+nl+B9Statements(text,guiTail)+nl+"}";
                B9Add(selected,text,outer.SpanStart,guiTail.Last().Span.End,after,point,patches,seen);
            }
        }
        if(selected=="b9-boolean-value-before-if"&&owner=="kairo.unity.ui.Graphics"&&method=="_drawBitmap"){
            const string local="recoveredBitmapDrawPredicate";
            if(methodBody.DescendantNodes().OfType<IdentifierNameSyntax>().Any(n=>n.Identifier.ValueText==local))return;
            foreach(var branch in methodBody.DescendantNodes().OfType<IfStatementSyntax>()){
                string condition=B9Text(text,branch.Condition);
                if(!(branch.Parent is BlockSyntax)||!condition.Contains("renderIsReplace_")||!condition.Contains("!setFigureClip_")||!condition.Contains("renderForceMaterial_"))continue;
                if(model.GetTypeInfo(branch.Condition).Type?.SpecialType!=SpecialType.System_Boolean)continue;
                string after="bool "+local+" = "+condition+";"+nl+"if ("+local+")";
                B9Add(selected,text,branch.SpanStart,branch.CloseParenToken.Span.End,after,point,patches,seen);
            }
        }
    }
}
