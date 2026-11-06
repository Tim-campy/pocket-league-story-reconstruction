using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// Proposals only. The slot predictor and complete verifier decide acceptance.
public static partial class Round7TransformSyntax {
    static bool Round7Flag(JsonElement input,string name) => input.TryGetProperty(name,out var v)&&v.ValueKind==JsonValueKind.True;
    static bool Round7WithinCallable(SyntaxNode node,BlockSyntax body) => !node.Ancestors().TakeWhile(a=>a!=body)
        .Any(a=>a is AnonymousFunctionExpressionSyntax||a is LocalFunctionStatementSyntax);
    static string Round7Identifier(string name) => SyntaxFacts.GetKeywordKind(name)==SyntaxKind.None?name:"@"+name;
    static BlockSyntax Round7CallableBody(StatementSyntax mapped,BlockSyntax outer) {
        var nested=mapped.Ancestors().TakeWhile(n=>n!=outer).OfType<AnonymousFunctionExpressionSyntax>().FirstOrDefault();
        return nested==null?outer:nested.Body as BlockSyntax;
    }
    // Hoist only the first expression necessarily evaluated by a condition.
    // In particular the right hand side of &&/|| and ternary arms stay guarded.
    static bool Round7FirstEvaluation(ExpressionSyntax expr,ExpressionSyntax condition) {
        for(SyntaxNode cursor=expr;cursor!=condition&&cursor?.Parent!=null;cursor=cursor.Parent) {
            var parent=cursor.Parent;
            if(parent is ParenthesizedExpressionSyntax||parent is PrefixUnaryExpressionSyntax||parent is CastExpressionSyntax)continue;
            if(parent is BinaryExpressionSyntax b) {if(b.Left.Span.Contains(expr.Span))continue;return false;}
            if(parent is ConditionalExpressionSyntax q) {if(q.Condition.Span.Contains(expr.Span))continue;return false;}
            // An invocation/receiver or argument can have earlier side effects.
            return false;
        }
        return condition.Span.Contains(expr.Span);
    }
    static void AddRound7SmcsRules(JsonElement input,string text,StatementSyntax mapped,BlockSyntax methodBody,
        SemanticModel model,int point,List<Patch> patches,HashSet<string> seen) {
        string compiler=input.TryGetProperty("compiler",out var c)?c.GetString():"";
        if(compiler!=""&&compiler!="smcs"&&compiler!="mono-smcs")return;
        var body=Round7CallableBody(mapped,methodBody);if(body==null)return;
        Action<string,int,int,string> add=(rule,start,end,after)=>{
            if(start<body.SpanStart||end>body.Span.End||start>=end)return;
            string before=text.Substring(start,end-start);
            if(before==after||!seen.Add(start+":"+end+":"+after))return;
            patches.Add(new Patch {rule=rule,start=start,end=end,before=before,after=after,
                distance=start<=point&&point<end?0:Math.Min(Math.Abs(start-point),Math.Abs(end-point))});
        };
        var testTypes=HintTypes(input,"expected_test_local_types");
        if(testTypes.Length==0)testTypes=HintTypes(input,"expected_alias_types");
        var ifStatement=mapped as IfStatementSyntax;
        if(ifStatement!=null&&testTypes.Length!=0) {
            var condition=ifStatement.Condition;
            foreach(var expr in condition.DescendantNodesAndSelf().OfType<ExpressionSyntax>()) {
                if(expr is ParenthesizedExpressionSyntax||expr is LiteralExpressionSyntax)continue;
                if(expr is TypeSyntax) {
                    var valueSymbol=model.GetSymbolInfo(expr).Symbol;
                    if(!(valueSymbol is ILocalSymbol||valueSymbol is IParameterSymbol||valueSymbol is IFieldSymbol))continue;
                }
                var type=model.GetTypeInfo(expr).Type;
                if(type==null||type.TypeKind==TypeKind.Error||!testTypes.Contains(Key(type))||!Round7FirstEvaluation(expr,condition))continue;
                if(expr is AssignmentExpressionSyntax||expr is AnonymousFunctionExpressionSyntax||expr is AwaitExpressionSyntax)continue;
                string name=Fresh("testedValue",body),declaration=type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)+" "+name+" = "+expr+";\n";
                string changed=Change(text,mapped.SpanStart,mapped.Span.End,new[]{(expr.SpanStart,expr.Span.End,name)});
                // Same-block and explicit-block forms have distinct smcs allocation.
                if(mapped.Parent is BlockSyntax)add("tested-value-local",mapped.SpanStart,mapped.Span.End,declaration+changed);
                add("tested-value-local-block",mapped.SpanStart,mapped.Span.End,"{ "+declaration+changed+" }");
            }
        }
        var callableReturns=body.DescendantNodes().OfType<ReturnStatementSyntax>().Where(r=>Round7WithinCallable(r,body)).ToArray();
        if(Round7Flag(input,"original_unified_return")&&callableReturns.Length>0&&callableReturns.All(r=>r.Expression!=null)
            &&!body.DescendantNodes().Any(n=>n is YieldStatementSyntax&&Round7WithinCallable(n,body))) {
            ITypeSymbol returnType=null;
            var owner=body.Parent;
            if(owner is AnonymousFunctionExpressionSyntax lambda)returnType=(model.GetSymbolInfo(lambda).Symbol as IMethodSymbol)?.ReturnType;
            else returnType=(model.GetDeclaredSymbol(owner) as IMethodSymbol)?.ReturnType;
            if(returnType==null&&owner is AccessorDeclarationSyntax accessor)returnType=(model.GetDeclaredSymbol(accessor) as IMethodSymbol)?.ReturnType;
            if(returnType!=null&&returnType.SpecialType!=SpecialType.System_Void&&!returnType.IsRefLikeType) {
                string fresh=Fresh("resultValue",body),exit=Fresh("resultExit",body);
                var choices=new List<(string name,bool fresh)> { (fresh,true) };
                // An unused recovered local often already occupies the result slot.
                foreach(var declaration in body.Statements.OfType<LocalDeclarationStatementSyntax>().Where(SimpleLocal)) {
                    var variable=declaration.Declaration.Variables[0];var symbol=model.GetDeclaredSymbol(variable) as ILocalSymbol;
                    if(symbol==null||symbol.IsConst||!Same(symbol.Type,returnType))continue;
                    if(body.DescendantNodes().OfType<IdentifierNameSyntax>().Any(n=>Same(model.GetSymbolInfo(n).Symbol,symbol)))continue;
                    choices.Add((variable.Identifier.ToString(),false));
                }
                foreach(var choice in choices) {
                    string prefix=choice.fresh?returnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)+" "+choice.name+";\n":"";
                    var edits=callableReturns.Select(r=>(r.SpanStart,r.Span.End,"{ "+choice.name+" = "+r.Expression+"; goto "+exit+"; }")).ToList();
                    string inside=Change(text,body.OpenBraceToken.Span.End,body.CloseBraceToken.SpanStart,edits);
                    // Common simple layout: if (...) return a; return b;.
                    if(body.Statements.Count>=2&&body.Statements.Last() is ReturnStatementSyntax tail
                        &&body.Statements[body.Statements.Count-2] is IfStatementSyntax branch&&branch.Else==null
                        &&Only(branch.Statement) is ReturnStatementSyntax first&&first.Expression!=null) {
                        string before=text.Substring(body.OpenBraceToken.Span.End,branch.SpanStart-body.OpenBraceToken.Span.End);
                        string after="{\n"+prefix+before+"if ("+branch.Condition+") { "+choice.name+" = "+first.Expression+"; } else { "+choice.name+" = "+tail.Expression+"; }\nreturn "+choice.name+";\n}";
                        add("unified-result-return-if-else",body.SpanStart,body.Span.End,after);
                    }
                    add("unified-result-return-goto",body.SpanStart,body.Span.End,"{\n"+prefix+inside+"\n"+exit+": return "+choice.name+";\n}");
                }
            }
        }
        // A generated common-exit candidate may still contain a goto that
        // immediately falls through to its label. mcs emits that goto literally.
        foreach(var label in body.Statements.OfType<LabeledStatementSyntax>()) {
            int index=body.Statements.IndexOf(label);if(index<1)continue;
            var previous=body.Statements[index-1];
            var jump=previous as GotoStatementSyntax;
            if(previous is BlockSyntax previousBlock)jump=previousBlock.Statements.LastOrDefault() as GotoStatementSyntax;
            if(jump!=null&&jump.IsKind(SyntaxKind.GotoStatement)&&jump.Expression is IdentifierNameSyntax name&&name.Identifier.ValueText==label.Identifier.ValueText)
                add("common-exit-remove-fallthrough-goto",jump.SpanStart,jump.Span.End,"");
        }
        if(Round7Flag(input,"original_void_common_exit")&&callableReturns.All(r=>r.Expression==null)) {
            // An early return guarding the method tail becomes a structured else.
            var guard=mapped.AncestorsAndSelf().OfType<IfStatementSyntax>().FirstOrDefault();
            if(guard!=null&&guard.Else==null&&guard.Parent is BlockSyntax block
                &&block==body&&Only(guard.Statement) is ReturnStatementSyntax) {
                int index=block.Statements.IndexOf(guard);
                if(index>=0&&index<block.Statements.Count-1) {
                    int end=block.Statements.Last().Span.End;
                    string tail=text.Substring(guard.Span.End,end-guard.Span.End);
                    add("void-common-exit-if-else",guard.SpanStart,end,"if ("+Negate(guard.Condition,model)+") {\n"+tail+"\n}");
                    if(Round7Selector(guard.Condition,false,model,out var selector,out var values)&&Round7NoOuterJump(block.Statements.Last())) {
                        string cases=string.Join("\n",values.Select(v=>"case "+v+":"));
                        add("void-common-exit-switch-break",guard.SpanStart,end,"switch ("+selector+") {\n"+cases+"\n"+tail+"\nbreak;\n}");
                        // A recovered alias can itself be the switch's hidden
                        // selector slot. Preserve the parameter loads in its body.
                        var alias=model.GetSymbolInfo(selector).Symbol as ILocalSymbol;
                        var prior=index>0?block.Statements[index-1] as LocalDeclarationStatementSyntax:null;
                        if(alias!=null&&prior!=null&&SimpleLocal(prior)) {
                            var variable=prior.Declaration.Variables[0];var value=variable.Initializer?.Value;
                            var parameter=value==null?null:model.GetSymbolInfo(Unwrap(value)).Symbol as IParameterSymbol;
                            if(Same(model.GetDeclaredSymbol(variable),alias)&&parameter!=null&&!Written(body,parameter,model)&&!Written(body,alias,model)) {
                                var uses=body.DescendantNodes().OfType<IdentifierNameSyntax>().Where(n=>Same(model.GetSymbolInfo(n).Symbol,alias)).ToArray();
                                if(uses.All(n=>n.SpanStart>=guard.SpanStart&&n.Span.End<=end)) {
                                    var edits=uses.Where(n=>n.SpanStart>=guard.Span.End).Select(n=>(n.SpanStart,n.Span.End,value.ToString()));
                                    string replaced=Change(text,guard.Span.End,end,edits);
                                    add("void-common-exit-switch-parameter",prior.SpanStart,end,"switch ("+value+") {\n"+cases+"\n"+replaced+"\nbreak;\n}");
                                }
                            }
                        }
                    }
                }
            }
            // Existing terminal switch cases must leave the switch, then reach ret.
            var sw=mapped.AncestorsAndSelf().OfType<SwitchStatementSyntax>().FirstOrDefault();
            if(sw!=null&&sw.Parent==body&&body.Statements.Last()==sw) {
                var returns=sw.DescendantNodes().OfType<ReturnStatementSyntax>().Where(r=>Round7WithinCallable(r,body)).ToArray();
                if(returns.Length>0&&returns.All(r=>!r.Ancestors().TakeWhile(a=>a!=sw).Any(a=>IsLoop(a)||a is SwitchStatementSyntax)))
                    add("void-common-exit-switch-break",sw.SpanStart,sw.Span.End,Change(text,sw.SpanStart,sw.Span.End,returns.Select(r=>(r.SpanStart,r.Span.End,"break;"))));
            }
        }
        var newTypes=HintTypes(input,"expected_newobj_local_types");
        if(mapped is ExpressionStatementSyntax es&&Unwrap(es.Expression) is AssignmentExpressionSyntax assign
            &&assign.IsKind(SyntaxKind.SimpleAssignmentExpression)&&Unwrap(assign.Right) is ObjectCreationExpressionSyntax created
            &&created.Initializer==null&&model.GetSymbolInfo(assign.Left).Symbol is IFieldSymbol field&&!field.IsStatic) {
            bool stable=assign.Left is IdentifierNameSyntax||assign.Left is MemberAccessExpressionSyntax access&&(Unwrap(access.Expression) is ThisExpressionSyntax||Unwrap(access.Expression) is BaseExpressionSyntax);
            var type=model.GetTypeInfo(created).Type;
            if(stable&&type!=null&&newTypes.Contains(Key(type))) {
                string name=Fresh("createdValue",body),after=type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)+" "+name+" = "+created+";\n"+assign.Left+" = "+name+";";
                if(mapped.Parent is BlockSyntax)add("new-object-local-before-field",mapped.SpanStart,mapped.Span.End,after);
                add("new-object-local-before-field-block",mapped.SpanStart,mapped.Span.End,"{ "+after+" }");
            }
        }
        // A decompiler object initializer passed as an argument hides the
        // explicit source local and can move the invocation's this load earlier.
        if(mapped is ExpressionStatementSyntax callStatement&&Unwrap(callStatement.Expression) is InvocationExpressionSyntax call
            &&call.ArgumentList.Arguments.Count==1&&Unwrap(call.ArgumentList.Arguments[0].Expression) is ObjectCreationExpressionSyntax newArgument) {
            bool stableReceiver=call.Expression is IdentifierNameSyntax||call.Expression is MemberAccessExpressionSyntax member
                &&(Unwrap(member.Expression) is ThisExpressionSyntax||Unwrap(member.Expression) is BaseExpressionSyntax);
            var type=model.GetTypeInfo(newArgument).Type;
            var initializers=newArgument.Initializer?.Expressions.ToArray()??new ExpressionSyntax[0];
            if(stableReceiver&&type!=null&&newTypes.Contains(Key(type))&&initializers.All(e=>e is AssignmentExpressionSyntax a
                &&a.IsKind(SyntaxKind.SimpleAssignmentExpression)&&a.Left is IdentifierNameSyntax&&!(a.Right is InitializerExpressionSyntax))) {
                string name=Fresh("createdValue",body),creation=newArgument.WithInitializer(null).ToString();
                string assignments=string.Join("\n",initializers.Cast<AssignmentExpressionSyntax>().Select(a=>name+"."+a.Left+" = "+a.Right+";"));
                string invoke=Change(text,mapped.SpanStart,mapped.Span.End,new[]{(newArgument.SpanStart,newArgument.Span.End,name)});
                string after=type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)+" "+name+" = "+creation+";\n"+assignments+"\n"+invoke;
                if(mapped.Parent is BlockSyntax)add("new-object-argument-local",mapped.SpanStart,mapped.Span.End,after);
                add("new-object-argument-local-block",mapped.SpanStart,mapped.Span.End,"{ "+after+" }");
            }
        }
        Round7ZeroChain(input,text,mapped,body,model,add);
    }
    static void Round7ZeroChain(JsonElement input,string text,StatementSyntax mapped,BlockSyntax body,SemanticModel model,Action<string,int,int,string> add) {
        var names=HintTypes(input,"original_zero_chain_names");
        if(names.Length<2||names.Distinct().Count()!=names.Length)return;
        var order=HintTypes(input,"original_zero_chain_declaration_order");
        if(order.Length!=names.Length||!new HashSet<string>(order).SetEquals(names))return;
        // Require a real zero assignment at the divergence, not an invented value.
        ExpressionSyntax value=null;
        if(mapped is LocalDeclarationStatementSyntax declaration&&SimpleLocal(declaration))value=declaration.Declaration.Variables[0].Initializer?.Value;
        if(mapped is ExpressionStatementSyntax es)value=es.Expression;
        if(value==null)return;
        while(Unwrap(value) is AssignmentExpressionSyntax assignment)value=assignment.Right;
        if(!LiteralZero(Unwrap(value),model))return;
        var declarations=body.DescendantNodes().OfType<LocalDeclarationStatementSyntax>().Where(SimpleLocal)
            .Where(d=>Round7WithinCallable(d,body)&&names.Contains(d.Declaration.Variables[0].Identifier.ValueText)).ToArray();
        if(names.Any(n=>!declarations.Any(d=>d.Declaration.Variables[0].Identifier.ValueText==n)))return;
        var symbols=declarations.Select(d=>model.GetDeclaredSymbol(d.Declaration.Variables[0]) as ILocalSymbol).ToArray();
        if(symbols.Any(s=>s==null||s.IsConst)||symbols.Any(s=>!Same(s.Type,symbols[0].Type)))return;
        // Rebinding a different field/parameter/capture with the same spelling is forbidden.
        foreach(var id in body.DescendantNodes().OfType<IdentifierNameSyntax>().Where(id=>names.Contains(id.Identifier.ValueText)))
            if(!Round7WithinCallable(id,body)||!symbols.Any(s=>Same(s,model.GetSymbolInfo(id).Symbol)))return;
        int start=Math.Min(mapped.SpanStart,declarations.Min(d=>d.SpanStart)),end=body.CloseBraceToken.SpanStart;
        var edits=new List<(int,int,string)>();
        foreach(var d in declarations) {
            if(d==mapped)continue;
            var variable=d.Declaration.Variables[0];
            edits.Add((d.SpanStart,d.Span.End,variable.Initializer==null?"":variable.Identifier+" = "+variable.Initializer.Value+";"));
        }
        edits.Add((mapped.SpanStart,mapped.Span.End,string.Join(" = ",names.Select(Round7Identifier))+" = "+value+";"));
        string type=symbols[0].Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        string prefix=type+" "+string.Join(", ",order.Select(Round7Identifier))+";\n";
        add("extended-original-zero-chain",start,end,prefix+Change(text,start,end,edits));
        // Restoring the first slots can expose a separate extra selector
        // temporary later in the method. Combine established source operations
        // so that the source-only gate can approve the complete signature.
        if(Round7Flag(input,"original_has_no_switch")) {
            var switches=body.DescendantNodes().OfType<SwitchStatementSyntax>().Where(s=>Round7WithinCallable(s,body)).ToArray();
            var switchEdits=new List<(int,int,string)>();bool safe=switches.Length>0;
            foreach(var sw in switches) {
                if(sw.SpanStart<start||sw.Span.End>end||sw.Ancestors().TakeWhile(a=>a!=body).Any(a=>a is SwitchStatementSyntax)
                    ||edits.Any(e=>e.Item1<sw.Span.End&&e.Item2>sw.SpanStart)||!Round7SwitchAsIf(sw,model,out var replacement)) {safe=false;break;}
                switchEdits.Add((sw.SpanStart,sw.Span.End,replacement));
            }
            if(safe)add("extended-original-zero-chain-and-switch-if",start,end,prefix+Change(text,start,end,edits.Concat(switchEdits)));
        }
    }
    static bool Round7SwitchAsIf(SwitchStatementSyntax sw,SemanticModel model,out string replacement) {
        replacement=null;if(!Round7Stable(sw.Expression,model))return false;
        var pieces=new List<string>();string otherwise=null;
        foreach(var section in sw.Sections) {
            var statements=section.Statements.ToList();if(statements.Count==0)return false;
            if(statements.Take(statements.Count-1).Any(s=>s.DescendantNodesAndSelf().Any(n=>n is BreakStatementSyntax||n is GotoStatementSyntax)))return false;
            if(statements.Last() is BreakStatementSyntax)statements.RemoveAt(statements.Count-1);
            else if(!Terminates(statements.Last()))return false;
            string block="{ "+string.Join("\n",statements)+" }";
            if(section.Labels.Any(l=>l is DefaultSwitchLabelSyntax)) {if(section.Labels.Count!=1)return false;otherwise=block;continue;}
            var labels=section.Labels.OfType<CaseSwitchLabelSyntax>().ToArray();if(labels.Length!=section.Labels.Count)return false;
            pieces.Add("if ("+string.Join(" || ",labels.Select(l=>"("+sw.Expression+" == "+l.Value+")"))+") "+block);
        }
        if(pieces.Count==0)return false;
        replacement=string.Join(" else ",pieces)+(otherwise==null?"":" else "+otherwise);return true;
    }
}
