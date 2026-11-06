// Opt-in source proposals only: no target emission or state writes.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static partial class Round7TransformSyntax {
    static SyntaxNode Round7SwitchBreakOwner(BreakStatementSyntax statement) => statement.Ancestors().FirstOrDefault(n=>n is SwitchStatementSyntax||IsLoop(n));
    static SyntaxNode Round7SwitchContinueOwner(ContinueStatementSyntax statement) => statement.Ancestors().FirstOrDefault(IsLoop);
    static StatementSyntax Round7SwitchLast(StatementSyntax statement) {
        while(statement is BlockSyntax block&&block.Statements.Count>0)statement=block.Statements.Last();
        return statement;
    }
    static bool Round7SwitchScopeIsLocal(SwitchStatementSyntax sw,SwitchSectionSyntax section,SemanticModel model) {
        foreach(var variable in section.DescendantNodes().OfType<VariableDeclaratorSyntax>()) {
            var symbol=model.GetDeclaredSymbol(variable) as ILocalSymbol;if(symbol==null)continue;
            if(sw.DescendantNodes().OfType<IdentifierNameSyntax>().Any(id=>!section.Span.Contains(id.Span)&&Same(model.GetSymbolInfo(id).Symbol,symbol)))return false;
        }
        return true;
    }
    static bool Round7SwitchCanConvert(SwitchStatementSyntax sw,SemanticModel model) {
        var type=model.GetTypeInfo(sw.Expression).Type;
        if(!Round7Stable(sw.Expression,model)||!(Integral(type)||type?.TypeKind==TypeKind.Enum))return false;
        foreach(var statement in sw.DescendantNodes().OfType<ContinueStatementSyntax>()) {
            var owner=Round7SwitchContinueOwner(statement);
            if(owner==null||!sw.Span.Contains(owner.Span))return false;
        }
        // A nested switch retains its own gotos. Cross-case transfer for this
        // converted switch needs separate label/data-flow analysis.
        foreach(var statement in sw.DescendantNodes().OfType<GotoStatementSyntax>())
            if(statement.Ancestors().OfType<SwitchStatementSyntax>().FirstOrDefault()==sw)return false;
        return sw.Sections.All(section=>section.Statements.Count>0&&Round7SwitchScopeIsLocal(sw,section,model));
    }
    static string Round7SwitchSectionBody(string text,SwitchStatementSyntax sw,SwitchSectionSyntax section,string exit,
        out bool usesExit) {
        usesExit=false;
        var terminal=Round7SwitchLast(section.Statements.Last());
        var edits=new List<(int,int,string)>();
        foreach(var statement in section.DescendantNodes().OfType<BreakStatementSyntax>()) {
            if(Round7SwitchBreakOwner(statement)!=sw)continue;
            bool last=statement==terminal;
            edits.Add((statement.SpanStart,statement.Span.End,last?"":"goto "+exit+";"));
            if(!last)usesExit=true;
        }
        int start=section.Statements.First().SpanStart,end=section.Statements.Last().Span.End;
        string commands=Change(text,start,end,edits);
        // Retain an existing explicit case block exactly, so declarations keep
        // their lexical scope; otherwise create the if arm's block.
        return section.Statements.Count==1&&section.Statements[0] is BlockSyntax?commands:"{\n"+commands+"\n}";
    }
    static void AddRound7SmcsSwitchRules(JsonElement input,string text,StatementSyntax mapped,BlockSyntax methodBody,
        SemanticModel model,int point,List<Patch> patches,HashSet<string> seen) {
        string compiler=input.TryGetProperty("compiler",out var c)?c.GetString():"";
        string selected=input.TryGetProperty("round7_cycle_rule",out var s)?s.GetString():"";
        bool learned=input.TryGetProperty("round7_cycle_rules",out var active)&&active.ValueKind==JsonValueKind.Array
            &&active.EnumerateArray().Any(value=>value.ValueKind==JsonValueKind.String&&value.GetString()=="smcs-owned-switch-if");
        if((compiler!=""&&compiler!="smcs"&&compiler!="mono-smcs")||(selected!="smcs-owned-switch-if"&&!learned))return;
        var body=Round7CallableBody(mapped,methodBody);if(body==null)return;
        var switches=mapped.DescendantNodesAndSelf().OfType<SwitchStatementSyntax>()
            .Concat(mapped.Ancestors().TakeWhile(n=>n!=body).OfType<SwitchStatementSyntax>())
            .Where(n=>Round7WithinCallable(n,body)).Distinct().ToArray();
        foreach(var sw in switches) {
            if(!Round7SwitchCanConvert(sw,model))continue;
            string exit=Fresh("dispatchExit",body);bool anyExit=false,hasZero=false;
            var pieces=new List<string>();string otherwise=null;
            foreach(var section in sw.Sections) {
                string contents=Round7SwitchSectionBody(text,sw,section,exit,out bool usesExit);anyExit|=usesExit;
                if(section.Labels.Any(l=>l is DefaultSwitchLabelSyntax)) {otherwise=contents;continue;}
                var labels=section.Labels.OfType<CaseSwitchLabelSyntax>().ToArray();
                if(labels.Length!=section.Labels.Count) {pieces.Clear();break;}
                hasZero|=labels.Any(label=>model.GetConstantValue(label.Value).HasValue&&Convert.ToString(model.GetConstantValue(label.Value).Value)=="0");
                string condition=string.Join(" || ",labels.Select(label=>"("+sw.Expression+" == "+label.Value+")"));
                pieces.Add("if ("+condition+") "+contents);
            }
            if(pieces.Count==0)continue;
            Action<string,string> add=(rule,chain)=>{
                string after=anyExit?"{\n"+chain+"\n"+exit+": ;\n}":chain;
                int start=sw.SpanStart,end=sw.Span.End;
                string before=text.Substring(start,end-start);
                if(start<body.SpanStart||end>body.Span.End||before==after||!seen.Add(start+":"+end+":"+after))return;
                patches.Add(new Patch {rule=rule,start=start,end=end,before=before,after=after,
                    distance=start<=point&&point<end?0:Math.Min(Math.Abs(start-point),Math.Abs(end-point))});
            };
            string tail=otherwise==null?"":" else "+otherwise;
            add("smcs-owned-switch-if",string.Join(" else ",pieces)+tail);
            // Restore a zero no-op case only with direct original IL evidence.
            if(!hasZero&&otherwise==null&&Round7Flag(input,"original_empty_zero_arm"))
                add("smcs-owned-switch-if-empty-zero","if ("+sw.Expression+" == 0) { } else "+string.Join(" else ",pieces));
        }
    }
}
