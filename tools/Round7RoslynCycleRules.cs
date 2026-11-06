// Cycle hypotheses only. Kept separate from the active helper until explicitly selected.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static partial class Round7TransformSyntax {
    static void AddRound7RoslynCycleRules(JsonElement input,string text,StatementSyntax mapped,BlockSyntax body,
        SemanticModel model,int point,List<Patch> patches,HashSet<string> seen) {
        if(!input.TryGetProperty("round7_cycle_rule",out var selected))return;
        if(selected.GetString()=="nullable-conditional-coalesce-explicit") {
            AddRound7NullableConditionalCoalesce(text,mapped,model,point,patches,seen);
            return;
        }
        if(selected.GetString()!="switch-owned-break-if-chain")return;
        foreach(var sw in mapped.DescendantNodesAndSelf().OfType<SwitchStatementSyntax>()) {
            if(sw.Sections.Count==0 || sw.Sections.Count>32)continue;
            var selector=model.GetSymbolInfo(sw.Expression).Symbol;
            if(!(selector is ILocalSymbol || selector is IParameterSymbol || selector is IFieldSymbol field&&!field.IsVolatile))continue;
            // goto case/default cannot survive the removal of their switch.
            if(sw.DescendantNodes().OfType<GotoStatementSyntax>().Any(g=>g.CaseOrDefaultKeyword.RawKind!=0))continue;
            var owned=sw.DescendantNodes().OfType<BreakStatementSyntax>().Where(b=>
                b.Ancestors().FirstOrDefault(a=>a is SwitchStatementSyntax || IsLoop(a))==sw).ToArray();
            string exit="recoveredSwitchExit_"+sw.SpanStart;
            bool labelNeeded=false,valid=true;string otherwise=null;var pieces=new List<string>();
            foreach(var section in sw.Sections) {
                if(section.Statements.Count==0){valid=false;break;}
                var labels=section.Labels.OfType<CaseSwitchLabelSyntax>().ToArray();
                bool fallback=section.Labels.Any(l=>l is DefaultSwitchLabelSyntax);
                if(!fallback && labels.Length!=section.Labels.Count){valid=false;break;}
                if(fallback && section.Labels.Count!=1){valid=false;break;}
                // A terminal break may be wrapped by one or more case blocks.
                StatementSyntax last=section.Statements.Last();
                while(last is BlockSyntax block && block.Statements.Count>0)last=block.Statements.Last();
                int terminal=last is BreakStatementSyntax terminalBreak && owned.Any(b=>b.SpanStart==terminalBreak.SpanStart)?last.SpanStart:-1;
                int begin=section.Statements.First().SpanStart,end=section.Statements.Last().Span.End;
                string content=text.Substring(begin,end-begin);
                foreach(var br in owned.Where(b=>b.SpanStart>=begin&&b.Span.End<=end).OrderByDescending(b=>b.SpanStart)) {
                    string replacement=br.SpanStart==terminal?"":"goto "+exit+";";
                    if(replacement.Length>0)labelNeeded=true;
                    content=content.Substring(0,br.SpanStart-begin)+replacement+content.Substring(br.Span.End-begin);
                }
                string branch="{ "+content+" }";
                if(fallback){otherwise=branch;continue;}
                pieces.Add("if ("+string.Join(" || ",labels.Select(l=>"("+sw.Expression+" == "+l.Value+")"))+") "+branch);
            }
            if(!valid||pieces.Count==0)continue;
            string after=string.Join(" else ",pieces)+(otherwise==null?"":" else "+otherwise);
            if(labelNeeded)after="{ "+after+"\n"+exit+": ; }";
            string key=sw.SpanStart+":"+sw.Span.End+":"+after;
            if(after!=sw.ToString()&&seen.Add(key))patches.Add(new Patch{rule="switch-owned-break-if-chain",start=sw.SpanStart,end=sw.Span.End,
                before=text.Substring(sw.SpanStart,sw.Span.Length),after=after,distance=sw.Span.Contains(point)?0:Math.Abs(sw.SpanStart-point)});
        }
    }
    static void AddRound7NullableConditionalCoalesce(string text,StatementSyntax mapped,SemanticModel model,int point,
        List<Patch> patches,HashSet<string> seen) {
        foreach(var coalesce in mapped.DescendantNodesAndSelf().OfType<BinaryExpressionSyntax>().Where(b=>b.IsKind(SyntaxKind.CoalesceExpression))) {
            ExpressionSyntax left=coalesce.Left;
            while(left is ParenthesizedExpressionSyntax paren)left=paren.Expression;
            if(!(left is ConditionalAccessExpressionSyntax conditional))continue;
            var receiver=conditional.Expression;var symbol=model.GetSymbolInfo(receiver).Symbol;
            if(!(symbol is ILocalSymbol || symbol is IParameterSymbol || symbol is IFieldSymbol field&&!field.IsVolatile))continue;
            if(model.GetTypeInfo(receiver).Type?.IsReferenceType!=true)continue;
            var result=model.GetTypeInfo(conditional.WhenNotNull).Type;
            // If the member itself returns Nullable<T>, its own null still has
            // to select the fallback. A receiver-only test cannot replace that.
            if(result==null || !result.IsValueType || result.SpecialType==SpecialType.System_Void ||
                result is INamedTypeSymbol named&&named.OriginalDefinition.SpecialType==SpecialType.System_Nullable_T)continue;
            if(!model.GetConstantValue(coalesce.Right).HasValue)continue;
            string access=conditional.WhenNotNull.ToString();
            if(!(access.StartsWith(".")||access.StartsWith("[")))continue;
            string after="(((object)("+receiver+") != null) ? "+receiver+access+" : ("+coalesce.Right+"))";
            string key=coalesce.SpanStart+":"+coalesce.Span.End+":"+after;
            if(seen.Add(key))patches.Add(new Patch{rule="nullable-conditional-coalesce-explicit",start=coalesce.SpanStart,end=coalesce.Span.End,
                before=text.Substring(coalesce.SpanStart,coalesce.Span.Length),after=after,distance=coalesce.Span.Contains(point)?0:Math.Abs(coalesce.SpanStart-point)});
        }
    }
}
