// Opt-in composition of existing source forms. No emitted game bytes are used.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static partial class Round7TransformSyntax {
    static bool R8SelectorCompFlag(JsonElement input,string key) => input.TryGetProperty(key,out var p)&&p.ValueKind==JsonValueKind.True;
    static bool R8SelectorCompBoolean(ExpressionSyntax expression,SemanticModel model) =>
        model.GetTypeInfo(expression).Type?.SpecialType==SpecialType.System_Boolean
        &&!expression.DescendantNodesAndSelf().OfType<ExpressionSyntax>().Any(e=>model.GetSymbolInfo(e).Symbol is IMethodSymbol op
            &&(op.MethodKind==MethodKind.UserDefinedOperator||op.MethodKind==MethodKind.Conversion));
    static bool R8SelectorCompBoundary(SyntaxNode node) => node is AnonymousFunctionExpressionSyntax||node is LocalFunctionStatementSyntax
        ||node is GotoStatementSyntax||node is LabeledStatementSyntax||node is YieldStatementSyntax;
    static StatementSyntax R8SelectorCompLoopBody(SyntaxNode loop) => loop is ForStatementSyntax f?f.Statement:
        loop is WhileStatementSyntax w?w.Statement:loop is DoStatementSyntax d?d.Statement:loop is ForEachStatementSyntax e?e.Statement:null;
    static void AddRound8SelectorContinueComposition(JsonElement input,string text,StatementSyntax mapped,BlockSyntax methodBody,
        SemanticModel model,int point,List<Patch> patches,HashSet<string> seen) {
        if(!input.TryGetProperty("round7_cycle_rule",out var selection)||selection.GetString()!="calculated-selector-continue-composition")return;
        if(!input.TryGetProperty("compiler",out var compiler)||(compiler.GetString()!="smcs"&&compiler.GetString()!="mono-smcs"))return;
        if(!R8SelectorCompFlag(input,"round8_selector_named_local_proved"))return;
        if(!input.TryGetProperty("round8_selector_call_name",out var callName)||callName.ValueKind!=JsonValueKind.String)return;
        if(mapped.Ancestors().TakeWhile(n=>n!=methodBody).Any(n=>n is AnonymousFunctionExpressionSyntax||n is LocalFunctionStatementSyntax))return;
        var guards=mapped.DescendantNodesAndSelf().OfType<IfStatementSyntax>()
            .Concat(mapped.Ancestors().TakeWhile(n=>n!=methodBody).OfType<IfStatementSyntax>()).Distinct();
        foreach(var guard in guards) {
            if(guard.Else!=null||!(guard.Parent is BlockSyntax block)||!R8SelectorCompBoolean(guard.Condition,model))continue;
            StatementSyntax single=guard.Statement is BlockSyntax b&&b.Statements.Count==1?b.Statements[0]:guard.Statement;
            if(!(single is ContinueStatementSyntax))continue;
            var loop=guard.Ancestors().FirstOrDefault(n=>R8SelectorCompLoopBody(n)!=null);
            if(loop==null||R8SelectorCompLoopBody(loop)!=block)continue;
            int index=block.Statements.IndexOf(guard);
            if(index<0||index>=block.Statements.Count-1||!(block.Statements.Last() is BreakStatementSyntax))continue;
            if(block.Statements.Skip(index+1).SelectMany(n=>n.DescendantNodesAndSelf()).Any(R8SelectorCompBoundary))continue;
            string tail=string.Concat(block.Statements.Skip(index+1).Select(n=>n.ToFullString()));
            string guardAfter="if ("+Negate(guard.Condition,model)+") {\n"+tail+"\n}";
            foreach(var dispatch in methodBody.DescendantNodes().OfType<SwitchStatementSyntax>()) {
                if(dispatch.SpanStart<=loop.Span.End||dispatch.Sections.Count!=2)continue;
                if(dispatch.Ancestors().TakeWhile(n=>n!=methodBody).Any(R8SelectorCompBoundary))continue;
                if(!(dispatch.Expression is InvocationExpressionSyntax invocation))continue;
                if(input.TryGetProperty("round8_selector_source_start",out var provedStart)&&provedStart.TryGetInt32(out int sourceStart)
                    &&invocation.SpanStart!=sourceStart)continue;
                if(input.TryGetProperty("round8_selector_source_end",out var provedEnd)&&provedEnd.TryGetInt32(out int sourceEnd)
                    &&invocation.Span.End!=sourceEnd)continue;
                if(!(model.GetSymbolInfo(invocation).Symbol is IMethodSymbol called)||called.Name!=callName.GetString()
                    ||called.ReturnType.SpecialType!=SpecialType.System_Int32)continue;
                if(dispatch.DescendantNodes().Any(R8SelectorCompBoundary))continue;
                var sections=dispatch.Sections.ToArray();bool safe=true;
                for(int n=0;n<2;n++) {
                    if(sections[n].Labels.Count!=1||!(sections[n].Labels[0] is CaseSwitchLabelSyntax label)) {safe=false;break;}
                    var value=model.GetConstantValue(label.Value);
                    if(!value.HasValue||!(value.Value is int key)||key!=n) {safe=false;break;}
                    if(sections[n].Statements.Count!=1||!(sections[n].Statements[0] is BlockSyntax arm)
                        ||!(arm.Statements.LastOrDefault() is ReturnStatementSyntax)&&!(arm.Statements.LastOrDefault() is ThrowStatementSyntax)) {safe=false;break;}
                }
                if(!safe)continue;
                string alias=Fresh("recoveredSelector",methodBody);
                string dispatchAfter="int "+alias+" = "+invocation+";\nif ("+alias+" == 0) "+sections[0].Statements[0]
                    +"\nelse if ("+alias+" == 1) "+sections[1].Statements[0];
                int start=guard.SpanStart,end=dispatch.Span.End,guardEnd=block.Statements.Last().Span.End;
                if(start<methodBody.SpanStart||end>methodBody.Span.End)continue;
                string after=Change(text,start,end,new[]{(start,guardEnd,guardAfter),(dispatch.SpanStart,dispatch.Span.End,dispatchAfter)});
                string before=text.Substring(start,end-start);
                if(!seen.Add(start+":"+end+":"+after))continue;
                patches.Add(new Patch {rule="calculated-selector-continue-composition",start=start,end=end,before=before,after=after,
                    distance=start<=point&&point<end?0:Math.Min(Math.Abs(start-point),Math.Abs(end-point))});
            }
        }
    }
}
