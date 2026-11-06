// Calculated alias source hypothesis. Activation waits for the engine owner.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static partial class Round6TransformSyntax {
    static void AddRound6CalculatedParameterAlias(JsonElement input,string text,StatementSyntax mapped,BlockSyntax body,
        SemanticModel model,int point,List<Patch> patches,HashSet<string> seen) {
        if(input.TryGetProperty("compiler",out var c)&&c.ValueKind==JsonValueKind.String&&
            c.GetString()!="smcs"&&c.GetString()!="mono-smcs")return;
        if(!input.TryGetProperty("alias_calculus_status",out var status)||
            status.GetString()!="unique-calculated-alias-with-equal-body-symbols")return;
        if(!input.TryGetProperty("original_alias_parameter_index",out var p)||p.ValueKind!=JsonValueKind.Number)return;
        if(!input.TryGetProperty("alias_use_source_points",out var points)||points.ValueKind!=JsonValueKind.Array)return;
        if(!(mapped.Parent is BlockSyntax insertionBlock))return;
        var method=model.GetEnclosingSymbol(mapped.SpanStart) as IMethodSymbol;
        var parameter=method?.Parameters.SingleOrDefault(x=>x.Ordinal==p.GetInt32());
        if(parameter==null||parameter.RefKind!=RefKind.None||Written(body,parameter,model))return;
        var identifiers=body.DescendantNodes().OfType<IdentifierNameSyntax>()
            .Where(id=>Same(model.GetSymbolInfo(id).Symbol,parameter)).ToArray();
        if(identifiers.Any(id=>id.Ancestors().TakeWhile(n=>n!=body).Any(n=>n is AnonymousFunctionExpressionSyntax)))return;
        var source=body.SyntaxTree.GetText();
        var chosen=new List<IdentifierNameSyntax>();
        foreach(var item in points.EnumerateArray()) {
            if(!item.TryGetProperty("start_line",out var sl)||!item.TryGetProperty("start_column",out var sc)||
               !item.TryGetProperty("end_line",out var el)||!item.TryGetProperty("end_column",out var ec)||
               !item.TryGetProperty("parameter_occurrence",out var occurrence)||!item.TryGetProperty("parameter_occurrences",out var count))return;
            int startLine=sl.GetInt32()-1,endLine=el.GetInt32()-1;
            if(startLine<0||startLine>=source.Lines.Count)return;
            int start,end;
            if(endLine>=startLine&&endLine<source.Lines.Count&&sc.GetInt32()>0&&ec.GetInt32()>0) {
                var a=source.Lines[startLine];var b=source.Lines[endLine];
                start=a.Start+sc.GetInt32()-1;end=b.Start+ec.GetInt32()-1;
                if(start<a.Start||start>a.End||end<b.Start||end>b.End||end<=start)return;
            } else {
                // Historical MDB often records only the source line. Refuse
                // multiple leaf statements on that line instead of guessing.
                var candidates=body.DescendantNodes().OfType<StatementSyntax>().Where(s=>!(s is BlockSyntax))
                    .Where(s=>{var span=s.GetLocation().GetLineSpan();return span.StartLinePosition.Line<=startLine&&span.EndLinePosition.Line>=startLine;})
                    .Where(s=>identifiers.Any(id=>s.Span.Contains(id.Span)&&ReadExpression(id))).ToArray();
                var leaves=candidates.Where(s=>!candidates.Any(other=>other!=s&&s.Span.Contains(other.Span))).ToArray();
                if(leaves.Length!=1)return;
                start=leaves[0].SpanStart;end=leaves[0].Span.End;
            }
            var matches=identifiers.Where(id=>id.SpanStart>=start&&id.Span.End<=end&&ReadExpression(id))
                .OrderBy(id=>id.SpanStart).ToArray();
            int ordinal=occurrence.GetInt32();
            if(matches.Length!=count.GetInt32()||ordinal<0||ordinal>=matches.Length)return;
            var selected=matches[ordinal];
            if(selected.SpanStart<mapped.SpanStart||!selected.Ancestors().Any(n=>n==insertionBlock))return;
            if(chosen.Any(id=>id.Span==selected.Span))return;
            chosen.Add(selected);
        }
        if(chosen.Count==0)return;
        string alias=Fresh(parameter.Name+"Alias",body);
        string parameterName=SyntaxFacts.GetKeywordKind(parameter.Name)==SyntaxKind.None?parameter.Name:"@"+parameter.Name;
        string declaration=parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)+" "+alias+" = "+parameterName+";\n";
        int from=mapped.SpanStart,to=Math.Max(mapped.Span.End,chosen.Max(id=>id.Span.End));
        var edits=chosen.Select(id=>(id.SpanStart,id.Span.End,alias));
        string before=text.Substring(from,to-from),after=declaration+Change(text,from,to,edits);
        if(before==after||!seen.Add(from+":"+to+":"+after))return;
        patches.Add(new Patch{rule="calculated-parameter-alias",start=from,end=to,before=before,after=after,distance=0});
    }
}
