// Source hypotheses. A matching source span is never proof of matching IL.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static partial class Round6TransformSyntax {
    static bool Round6Stable(ExpressionSyntax expression,SemanticModel model) {
        expression=Unwrap(expression);
        var symbol=model.GetSymbolInfo(expression).Symbol;
        if(symbol is ILocalSymbol||symbol is IParameterSymbol)return true;
        if(symbol is IFieldSymbol field&&!field.IsVolatile) {
            if(expression is IdentifierNameSyntax)return true;
            return expression is MemberAccessExpressionSyntax m&&(m.Expression is ThisExpressionSyntax||m.Expression is BaseExpressionSyntax);
        }
        return false;
    }
    static bool Round6Selector(ExpressionSyntax condition,bool equal,SemanticModel model,
        out ExpressionSyntax selector,out List<ExpressionSyntax> values) {
        selector=null;values=new List<ExpressionSyntax>();
        var parts=new List<ExpressionSyntax>();
        Action<ExpressionSyntax> collect=null;
        collect=e=> {
            e=Unwrap(e);
            if(e is BinaryExpressionSyntax logic&&logic.IsKind(equal?SyntaxKind.LogicalOrExpression:SyntaxKind.LogicalAndExpression)) {collect(logic.Left);collect(logic.Right);}
            else parts.Add(e);
        };
        collect(condition);
        ExpressionSyntax chosen=null;var constants=new List<ExpressionSyntax>();
        foreach(var part in parts) {
            if(!(part is BinaryExpressionSyntax compare)||!compare.IsKind(equal?SyntaxKind.EqualsExpression:SyntaxKind.NotEqualsExpression))return false;
            ExpressionSyntax candidate,value;
            if(model.GetConstantValue(compare.Right).HasValue){candidate=Unwrap(compare.Left);value=compare.Right;}
            else if(model.GetConstantValue(compare.Left).HasValue){candidate=Unwrap(compare.Right);value=compare.Left;}
            else return false;
            var type=model.GetTypeInfo(candidate).Type;
            if(!Round6Stable(candidate,model)||!(Integral(type)||type?.TypeKind==TypeKind.Enum))return false;
            if(chosen!=null&&!Same(model.GetSymbolInfo(chosen).Symbol,model.GetSymbolInfo(candidate).Symbol))return false;
            chosen=candidate;constants.Add(value);
        }
        if(chosen==null)return false;selector=chosen;values=constants;return true;
    }
    static bool Round6NoOuterJump(StatementSyntax body) => !body.DescendantNodesAndSelf().Any(n=>n is GotoStatementSyntax||n is BreakStatementSyntax);
    static string Round6CaseBody(StatementSyntax body) => "{ "+BodyText(body)+(Terminates(body)?"":"\nbreak;")+" }";
    static string Round6Bound(ExpressionSyntax receiver,ExpressionSyntax binding) => "("+receiver+")"+binding;
    static void AddRound6CalculatedRules(JsonElement input,string text,StatementSyntax mapped,BlockSyntax body,
        SemanticModel model,int point,List<Patch> patches,HashSet<string> seen) {
        string compiler=input.TryGetProperty("compiler",out var compilerHint)?compilerHint.GetString():"";
        bool smcs=compiler==""||compiler=="smcs"||compiler=="mono-smcs";
        bool roslyn=compiler==""||compiler=="roslyn"||compiler.StartsWith("roslyn-");
        Action<string,int,int,string> add=(rule,start,end,after)=> {
            if(start<body.SpanStart||end>body.Span.End||start>=end)return;
            string before=text.Substring(start,end-start);
            if(before==after||!seen.Add(start+":"+end+":"+after))return;
            patches.Add(new Patch{rule=rule,start=start,end=end,before=before,after=after,distance=start<=point&&point<end?0:Math.Min(Math.Abs(point-start),Math.Abs(point-end))});
        };
        var nearby=mapped.DescendantNodesAndSelf().Where(n=>!n.Ancestors().TakeWhile(a=>a!=mapped).Any(a=>a is AnonymousFunctionExpressionSyntax)).ToList();
        foreach(var parent in mapped.Ancestors().OfType<IfStatementSyntax>())nearby.Add(parent);
        if(smcs) {
            foreach(var first in nearby.OfType<IfStatementSyntax>().Distinct()) {
                if(first.Parent is ElseClauseSyntax)continue;
                ExpressionSyntax selector;List<ExpressionSyntax> values;
                if(Round6Selector(first.Condition,true,model,out selector,out values)) {
                    var sections=new List<string>();var constants=new HashSet<string>();var cursor=first;bool valid=true;
                    while(cursor!=null) {
                        ExpressionSyntax current;List<ExpressionSyntax> labels;
                        if(!Round6Selector(cursor.Condition,true,model,out current,out labels)||
                            !Same(model.GetSymbolInfo(selector).Symbol,model.GetSymbolInfo(current).Symbol)||!Round6NoOuterJump(cursor.Statement)){valid=false;break;}
                        foreach(var label in labels) {
                            string value=Convert.ToString(model.GetConstantValue(label).Value,System.Globalization.CultureInfo.InvariantCulture);
                            if(!constants.Add(value)){valid=false;break;}
                        }
                        if(!valid)break;
                        sections.Add(string.Join("\n",labels.Select(label=>"case "+label+":"))+"\n"+Round6CaseBody(cursor.Statement));
                        if(cursor.Else?.Statement is IfStatementSyntax next){cursor=next;continue;}
                        if(cursor.Else!=null) {
                            if(!Round6NoOuterJump(cursor.Else.Statement)){valid=false;break;}
                            sections.Add("default:\n"+Round6CaseBody(cursor.Else.Statement));
                        }
                        cursor=null;
                    }
                    if(valid&&constants.Count>=2)add("if-chain-switch",first.SpanStart,first.Span.End,"switch ("+selector+") {\n"+string.Join("\n",sections)+"\n}");
                }
                // A recovered early-return guard can hide the original switch
                // selector temporary even when only two case values exist.
                if(first.Else==null&&IsJump(first.Statement)&&Round6Selector(first.Condition,false,model,out selector,out values)&&values.Count>=2&&Round6NoOuterJump(first.Statement))
                    add("if-guard-switch",first.SpanStart,first.Span.End,"switch ("+selector+") {\n"+string.Join("\n",values.Select(value=>"case "+value+":"))+" break;\ndefault: "+first.Statement+"\n}");
                if(first.Else==null&&Only(first.Statement) is ReturnStatementSyntax returnGuard&&returnGuard.Expression==null&&
                    first.Parent is BlockSyntax guardBlock&&Round6Selector(first.Condition,false,model,out selector,out values)&&values.Count>=2) {
                    int guardIndex=guardBlock.Statements.IndexOf(first);
                    var tail=guardBlock.Statements.Skip(guardIndex+1).ToArray();
                    if(tail.Length>0&&tail.All(Round6NoOuterJump)) {
                        string statements=string.Join("\n",tail.Select(s=>s.ToString()));
                        string stop=Terminates(tail.Last())?"":"\nbreak;";
                        add("if-guard-tail-switch",first.SpanStart,tail.Last().Span.End,"switch ("+selector+") {\n"+
                            string.Join("\n",values.Select(value=>"case "+value+":"))+" {\n"+statements+stop+"\n}\n}");
                    }
                }
            }
            // Swap the complete evaluations, unlike declaration-only hoisting.
            // Restrict to adjacent independent pure initializers; calls,
            // properties, writes and volatile reads cannot be interchanged.
            foreach(var block in mapped.AncestorsAndSelf().OfType<BlockSyntax>().Take(1)) {
                for(int index=0;index+1<block.Statements.Count;index++) {
                    var first=block.Statements[index] as LocalDeclarationStatementSyntax;
                    var second=block.Statements[index+1] as LocalDeclarationStatementSyntax;
                    if(first==null||second==null||!SimpleLocal(first)||!SimpleLocal(second))continue;
                    if(Distance(first,point)>200&&Distance(second,point)>200)continue;
                    var a=first.Declaration.Variables[0];var b=second.Declaration.Variables[0];
                    if(a.Initializer==null||b.Initializer==null)continue;
                    var asymbol=model.GetDeclaredSymbol(a);var bsymbol=model.GetDeclaredSymbol(b);
                    Func<ExpressionSyntax,bool> pure=e=> !e.DescendantNodesAndSelf().Any(n=>n is InvocationExpressionSyntax||n is ObjectCreationExpressionSyntax||n is AssignmentExpressionSyntax||n is PrefixUnaryExpressionSyntax u&&(u.IsKind(SyntaxKind.PreIncrementExpression)||u.IsKind(SyntaxKind.PreDecrementExpression))||n is PostfixUnaryExpressionSyntax||n is AwaitExpressionSyntax||n is ElementAccessExpressionSyntax)
                        && !e.DescendantNodesAndSelf().OfType<ExpressionSyntax>().Any(n=>model.GetSymbolInfo(n).Symbol is IPropertySymbol||model.GetSymbolInfo(n).Symbol is IFieldSymbol f&&f.IsVolatile)
                        && !e.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>().Any(n=>Same(model.GetSymbolInfo(n).Symbol,asymbol)||Same(model.GetSymbolInfo(n).Symbol,bsymbol));
                    if(!pure(a.Initializer.Value)||!pure(b.Initializer.Value))continue;
                    add("initialized-declaration-swap",first.SpanStart,second.Span.End,second.ToString()+"\n"+first);
                }
            }
        }
        if(!roslyn)return;
        foreach(var conditional in nearby.OfType<ConditionalAccessExpressionSyntax>().Distinct()) {
            if(!Round6Stable(conditional.Expression,model)||model.GetTypeInfo(conditional.Expression).Type?.IsReferenceType!=true)continue;
            string guard="((object)("+conditional.Expression+") != null)";
            string access=Round6Bound(conditional.Expression,conditional.WhenNotNull);
            if(conditional.Parent is ExpressionStatementSyntax statement)
                add("null-conditional-explicit",statement.SpanStart,statement.Span.End,"if ("+guard+") { "+access+"; }");
            if(conditional.Parent is ReturnStatementSyntax ret)
                add("null-conditional-return-explicit",ret.SpanStart,ret.Span.End,"if ("+guard+") { return "+access+"; }\nreturn null;");
            if(conditional.Parent is EqualsValueClauseSyntax initializer&&initializer.Parent is VariableDeclaratorSyntax variable&&variable.Parent.Parent is LocalDeclarationStatementSyntax declaration&&SimpleLocal(declaration)) {
                string type=(model.GetDeclaredSymbol(variable) as ILocalSymbol)?.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                if(type!=null)add("null-conditional-local-explicit",declaration.SpanStart,declaration.Span.End,type+" "+variable.Identifier+";\nif ("+guard+") { "+variable.Identifier+" = "+access+"; } else { "+variable.Identifier+" = null; }");
            }
        }
        foreach(var coalesce in nearby.OfType<BinaryExpressionSyntax>().Where(b=>b.IsKind(SyntaxKind.CoalesceExpression)).Distinct()) {
            if(!Round6Stable(coalesce.Left,model)||model.GetTypeInfo(coalesce.Left).Type?.IsReferenceType!=true)continue;
            string guard="((object)("+coalesce.Left+") != null)";
            if(coalesce.Parent is ReturnStatementSyntax ret)
                add("null-coalesce-return-explicit",ret.SpanStart,ret.Span.End,"if ("+guard+") { return "+coalesce.Left+"; }\nreturn "+coalesce.Right+";");
            if(coalesce.Parent is EqualsValueClauseSyntax initializer&&initializer.Parent is VariableDeclaratorSyntax variable&&variable.Parent.Parent is LocalDeclarationStatementSyntax declaration&&SimpleLocal(declaration)) {
                string type=(model.GetDeclaredSymbol(variable) as ILocalSymbol)?.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                if(type!=null)add("null-coalesce-local-explicit",declaration.SpanStart,declaration.Span.End,type+" "+variable.Identifier+";\nif ("+guard+") { "+variable.Identifier+" = "+coalesce.Left+"; } else { "+variable.Identifier+" = "+coalesce.Right+"; }");
            }
        }
    }
    static bool Same(ISymbol a,ISymbol b) => SymbolEqualityComparer.Default.Equals(a,b);
    static string Fresh(string stem,SyntaxNode body) {
        string clean=new string(stem.Where(c=>char.IsLetterOrDigit(c)||c=='_').ToArray());
        if(clean.Length==0||char.IsDigit(clean[0]))clean="value"+clean;
        var names=new HashSet<string>(body.DescendantTokens().Where(t=>t.IsKind(SyntaxKind.IdentifierToken)).Select(t=>t.ValueText));
        string name=clean;int n=1;while(names.Contains(name))name=clean+(n++);return name;
    }
    static bool Written(SyntaxNode body,ISymbol symbol,SemanticModel model) {
        foreach(var id in body.DescendantNodes().OfType<IdentifierNameSyntax>()) {
            if(!Same(model.GetSymbolInfo(id).Symbol,symbol))continue;
            if(id.Ancestors().OfType<AssignmentExpressionSyntax>().Any(a=>a.Left.Span.Contains(id.Span)))return true;
            if(id.Parent is PrefixUnaryExpressionSyntax p&&(p.IsKind(SyntaxKind.PreIncrementExpression)||p.IsKind(SyntaxKind.PreDecrementExpression)))return true;
            if(id.Parent is PostfixUnaryExpressionSyntax)return true;
            if(id.Parent is ArgumentSyntax a&&!a.RefKindKeyword.IsKind(SyntaxKind.None))return true;
        }
        return false;
    }
    static bool ReadExpression(ExpressionSyntax e) {
        if(e.Ancestors().OfType<AssignmentExpressionSyntax>().Any(a=>a.Left.Span.Contains(e.Span)))return false;
        if(e.Parent is ArgumentSyntax a&&!a.RefKindKeyword.IsKind(SyntaxKind.None))return false;
        if(e.Parent is PrefixUnaryExpressionSyntax p&&(p.IsKind(SyntaxKind.PreIncrementExpression)||p.IsKind(SyntaxKind.PreDecrementExpression)))return false;
        if(e.Parent is PostfixUnaryExpressionSyntax)return false;
        if(e.Ancestors().OfType<InvocationExpressionSyntax>().Any(i=>i.Expression.ToString()=="nameof"))return false;
        if(e.Ancestors().Any(a=>a is TypeOfExpressionSyntax||a is SizeOfExpressionSyntax))return false;
        return true;
    }
    static string Change(string text,int start,int end,IEnumerable<(int start,int end,string after)> edits) {
        string part=text.Substring(start,end-start);
        foreach(var e in edits.OrderByDescending(e=>e.start))part=part.Substring(0,e.start-start)+e.after+part.Substring(e.end-start);
        return part;
    }
    static string[] HintTypes(JsonElement input,string key) {
        if(!input.TryGetProperty(key,out var p))return new string[0];
        if(p.ValueKind==JsonValueKind.String)return new[]{p.GetString()};
        return p.ValueKind==JsonValueKind.Array?p.EnumerateArray().Where(x=>x.ValueKind==JsonValueKind.String).Select(x=>x.GetString()).ToArray():new string[0];
    }
    static int LocalCount(JsonElement input,string count,string types) => input.TryGetProperty(count,out var n)&&n.ValueKind==JsonValueKind.Number?n.GetInt32():HintTypes(input,types).Length;
    static void SharedRootZero(JsonElement input,string text,BlockSyntax body,BlockSyntax block,LocalDeclarationStatementSyntax declaration,
        ExpressionSyntax value,List<ILocalSymbol> locals,LocalDeclarationStatementSyntax[] earlier,SemanticModel model,
        Action<string,int,int,string> add) {
        int originalStores=input.TryGetProperty("original_zero_chain_stores",out var count)&&count.ValueKind==JsonValueKind.Number?count.GetInt32():0;
        bool explicitEvidence=input.TryGetProperty("zero_chain_missing_store",out var missing)&&missing.ValueKind==JsonValueKind.True;
        if(originalStores<locals.Count+1&&!explicitEvidence)return;
        var all=body.DescendantNodes().OfType<LocalDeclarationStatementSyntax>()
            .Where(d=>d.SpanStart>declaration.Span.End&&SimpleLocal(d))
            .Where(d=>!d.Ancestors().TakeWhile(a=>a!=body).Any(a=>a is AnonymousFunctionExpressionSyntax))
            .Select(d=>new{node=d,variable=d.Declaration.Variables[0],symbol=model.GetDeclaredSymbol(d.Declaration.Variables[0]) as ILocalSymbol})
            .Where(d=>d.symbol!=null&&!d.symbol.IsConst&&Same(d.symbol.Type,locals[0].Type));
        foreach(var group in all.GroupBy(d=>d.variable.Identifier.ValueText)) {
            var chosen=group.ToArray();if(chosen.Length<2)continue;
            string name=group.Key;
            if(locals.Any(l=>l.Name==name))continue;
            var symbols=chosen.Select(c=>c.symbol).ToArray();
            bool acceptable=true;
            // Hoisting must not change a field, parameter, other local, catch
            // variable, foreach variable, or a lambda's captured environment.
            foreach(var id in body.DescendantNodes().OfType<IdentifierNameSyntax>().Where(id=>id.Identifier.ValueText==name)) {
                if(!symbols.Any(s=>Same(s,model.GetSymbolInfo(id).Symbol))||
                    id.Ancestors().TakeWhile(a=>a!=body).Any(a=>a is AnonymousFunctionExpressionSyntax)) {acceptable=false;break;}
            }
            if(!acceptable)continue;
            if(body.DescendantNodes().OfType<VariableDeclaratorSyntax>().Where(v=>v.Identifier.ValueText==name)
                .Any(v=>!chosen.Any(c=>c.variable==v)))continue;
            if(body.DescendantNodes().OfType<ParameterSyntax>().Any(p=>p.Identifier.ValueText==name)||
                body.DescendantNodes().OfType<ForEachStatementSyntax>().Any(p=>p.Identifier.ValueText==name)||
                body.DescendantNodes().OfType<CatchDeclarationSyntax>().Any(p=>p.Identifier.ValueText==name))continue;
            int start=earlier.Min(d=>d.SpanStart),end=body.CloseBraceToken.SpanStart;
            string identifier=chosen[0].variable.Identifier.ToString();
            var chainNames=new[]{identifier}.Concat(locals.Select(l=>SyntaxFacts.GetKeywordKind(l.Name)==SyntaxKind.None?l.Name:"@"+l.Name)).ToArray();
            string type=locals[0].Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            var edits=earlier.Select(d=>(d.SpanStart,d.Span.End,"")).ToList();
            edits.Add((declaration.SpanStart,declaration.Span.End,string.Join(" = ",chainNames)+" = "+value+";"));
            foreach(var item in chosen) {
                string replacement=item.variable.Initializer==null?"":item.variable.Identifier+" = "+item.variable.Initializer.Value+";";
                edits.Add((item.node.SpanStart,item.node.Span.End,replacement));
            }
            string prefix=string.Join("\n",chainNames.Select(n=>type+" "+n+";"))+"\n";
            add("restore-shared-root-zero",start,end,prefix+Change(text,start,end,edits));
        }
    }

    static void AddRound6LegacyRules(JsonElement input,string text,StatementSyntax mapped,BlockSyntax body,SemanticModel model,int point,List<Patch> patches,HashSet<string> seen) {
        Action<string,int,int,string> add=(rule,start,end,after)=>{
            if(start<body.SpanStart||end>body.Span.End||start>end)return;
            string before=text.Substring(start,end-start);
            if(before==after||!seen.Add(start+":"+end+":"+after))return;
            patches.Add(new Patch{rule=rule,start=start,end=end,before=before,after=after,distance=start<=point&&point<end?0:Math.Min(Math.Abs(point-start),Math.Abs(point-end))});
        };
        var expected=new HashSet<string>(HintTypes(input,"expected_alias_types").Concat(HintTypes(input,"expected_alias_type")));
        var readNodes=mapped.DescendantNodesAndSelf().OfType<ExpressionSyntax>().Where(e=>e is IdentifierNameSyntax||e is MemberAccessExpressionSyntax)
            .Where(e=>!(e.Parent is MemberAccessExpressionSyntax ma&&ma.Name==e)).Where(ReadExpression)
            .Where(e=>!e.Ancestors().TakeWhile(a=>a!=mapped).OfType<AnonymousFunctionExpressionSyntax>().Any(a=>!a.FullSpan.Contains(point))).ToArray();
        foreach(var expression in readNodes) {
            var symbol=model.GetSymbolInfo(expression).Symbol;
            var parameter=symbol as IParameterSymbol;var field=symbol as IFieldSymbol;
            if(parameter==null&&field==null)continue;
            if(parameter!=null&&(parameter.RefKind!=RefKind.None||Written(body,parameter,model)))continue;
            if(field!=null&&(field.IsConst||field.IsVolatile))continue;
            if(field!=null&&expression is MemberAccessExpressionSyntax access&&!(access.Expression is ThisExpressionSyntax||access.Expression is IdentifierNameSyntax||field.IsStatic))continue;
            var type=parameter?.Type??field.Type;
            if(!expected.Contains(Key(type))&&!expected.Contains(type.ToDisplayString()))continue;
            string name=Fresh((symbol.Name.TrimEnd('_').Length>0?symbol.Name.TrimEnd('_'):"value")+"Alias",body);
            string declaration=type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)+" "+name+" = "+expression+";\n";
            var one=mapped.ReplaceNode(expression,SyntaxFactory.IdentifierName(name).WithTriviaFrom(expression));
            add(parameter!=null?"alias-parameter-occurrence":"alias-field-occurrence",mapped.SpanStart,mapped.Span.End,declaration+one);
            if(parameter!=null&&mapped.Parent is BlockSyntax tailBlock) {
                var tailReads=tailBlock.DescendantNodes().OfType<IdentifierNameSyntax>()
                    .Where(id=>id.SpanStart>=mapped.SpanStart&&Same(model.GetSymbolInfo(id).Symbol,parameter)).ToArray();
                if(tailReads.Length>1&&!tailReads.Any(id=>id.Ancestors().TakeWhile(a=>a!=tailBlock).Any(a=>a is AnonymousFunctionExpressionSyntax))) {
                    int end=tailBlock.CloseBraceToken.SpanStart;
                    var edits=tailReads.Select(id=>(id.SpanStart,id.Span.End,name));
                    add("alias-parameter-block-tail",mapped.SpanStart,end,declaration+Change(text,mapped.SpanStart,end,edits));
                }
            }
            var sameReads=readNodes.Where(e=>Same(model.GetSymbolInfo(e).Symbol,symbol)&&e.ToString()==expression.ToString()).ToArray();
            if(sameReads.Length>1) {
                var all=mapped.ReplaceNodes(sameReads,(old,rewritten)=>SyntaxFactory.IdentifierName(name).WithTriviaFrom(old));
                add(parameter!=null?"alias-parameter-command":"alias-field-command",mapped.SpanStart,mapped.Span.End,declaration+all);
            }
            // Reuse a prior, unmodified alias rather than reserving another slot.
            foreach(var localDecl in body.DescendantNodes().OfType<LocalDeclarationStatementSyntax>().Where(SimpleLocal).Where(d=>d.Span.End<=mapped.SpanStart)) {
                var variable=localDecl.Declaration.Variables[0];
                if(variable.Initializer==null||!Same(model.GetSymbolInfo(Unwrap(variable.Initializer.Value)).Symbol,symbol))continue;
                var local=model.GetDeclaredSymbol(variable) as ILocalSymbol;
                if(local==null||Written(body,local,model)||!model.LookupSymbols(mapped.SpanStart,name:variable.Identifier.ValueText).Any(s=>Same(s,local)))continue;
                var replaced=mapped.ReplaceNode(expression,SyntaxFactory.IdentifierName(variable.Identifier).WithTriviaFrom(expression));
                add("reuse-local-alias",mapped.SpanStart,mapped.Span.End,replaced.ToString());
            }
        }
        foreach(var loop in mapped.DescendantNodesAndSelf().OfType<ForEachStatementSyntax>()) {
            if(!(model.GetTypeInfo(loop.Expression).Type is IArrayTypeSymbol array)||array.Rank!=1)continue;
            var sequence=model.GetSymbolInfo(loop.Expression).Symbol;
            if(!(sequence is ILocalSymbol||sequence is IParameterSymbol)||Written(loop.Statement,sequence,model))continue;
            if(sequence is IParameterSymbol parameter&&parameter.RefKind!=RefKind.None)continue;
            string index=Fresh("spriteIndex",body);
            string elementType=(model.GetDeclaredSymbol(loop) as ILocalSymbol)?.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)??loop.Type.ToString();
            string after="for (int "+index+" = 0; "+index+" < "+loop.Expression+".Length; "+index+"++) {\n"+elementType+" "+loop.Identifier+" = "+loop.Expression+"["+index+"];\n"+BodyText(loop.Statement)+"\n}";
            add("foreach-array-indexed-for",loop.SpanStart,loop.Span.End,after);
            if(loop.Parent is BlockSyntax block) {
                int i=block.Statements.IndexOf(loop);
                if(i>0&&block.Statements[i-1] is LocalDeclarationStatementSyntax previous&&SimpleLocal(previous)) {
                    var v=previous.Declaration.Variables[0];
                    if(v.Initializer!=null&&Same(model.GetDeclaredSymbol(v),sequence)&&body.DescendantNodes().OfType<IdentifierNameSyntax>().Count(id=>Same(model.GetSymbolInfo(id).Symbol,sequence))==1)
                        add("foreach-direct-array-expression",previous.SpanStart,loop.Span.End,loop.WithExpression(v.Initializer.Value).ToString());
                }
            }
        }
        var unused=new List<(int index,string type,int loads,int stores)>();
        if(input.TryGetProperty("original_unused_slots",out var u)&&u.ValueKind==JsonValueKind.Array)foreach(var item in u.EnumerateArray()) {
            if(!item.TryGetProperty("type",out var t)||t.ValueKind!=JsonValueKind.String)continue;
            int loads=item.TryGetProperty("loads",out var l)?l.GetInt32():0,stores=item.TryGetProperty("stores",out var s)?s.GetInt32():0;
            if(loads==0)unused.Add((item.GetProperty("index").GetInt32(),t.GetString(),loads,stores));
        }
        foreach(var slot in unused.Where(s=>s.stores==0)) {
            string name=Fresh("originalUnusedSlot"+slot.index,body);
            string declaration="global::"+slot.type+" "+name+";\n";
            add("reserve-unused-original-slot",mapped.SpanStart,mapped.Span.End,declaration+mapped);
            if(mapped is ExpressionStatementSyntax statement&&Unwrap(statement.Expression) is AssignmentExpressionSyntax assign&&assign.IsKind(SyntaxKind.SimpleAssignmentExpression)) {
                var local=model.GetSymbolInfo(assign.Left).Symbol as ILocalSymbol;
                var variable=local?.DeclaringSyntaxReferences.Select(r=>r.GetSyntax()).OfType<VariableDeclaratorSyntax>().SingleOrDefault();
                var old=variable?.Parent?.Parent as LocalDeclarationStatementSyntax;
                if(old!=null&&SimpleLocal(old)&&variable.Initializer==null&&old.Parent==mapped.Parent&&old.Span.End<mapped.SpanStart) {
                    bool usedBetween=body.DescendantNodes().OfType<IdentifierNameSyntax>().Any(id=>id.SpanStart>old.Span.End&&id.Span.End<mapped.SpanStart&&Same(model.GetSymbolInfo(id).Symbol,local));
                    if(!usedBetween) {
                        string type=local.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                        string middle=text.Substring(old.Span.End,mapped.SpanStart-old.Span.End);
                        add("reserve-unused-slot-and-defer-local",old.SpanStart,mapped.Span.End,declaration+middle+type+" "+assign.Left+" = "+assign.Right+";");
                    }
                }
            }
        }
        foreach(var declaration in mapped.DescendantNodesAndSelf().OfType<LocalDeclarationStatementSyntax>().Where(SimpleLocal)) {
            if(!(declaration.Parent is BlockSyntax block))continue;
            var variable=declaration.Declaration.Variables[0];if(variable.Initializer==null)continue;
            var locals=new List<ILocalSymbol>();var rootLocal=model.GetDeclaredSymbol(variable) as ILocalSymbol;if(rootLocal==null||!Integral(rootLocal.Type))continue;
            locals.Add(rootLocal);ExpressionSyntax value=Unwrap(variable.Initializer.Value);
            while(value is AssignmentExpressionSyntax assign&&assign.IsKind(SyntaxKind.SimpleAssignmentExpression)) {
                var local=model.GetSymbolInfo(assign.Left).Symbol as ILocalSymbol;if(local==null||!Same(local.Type,rootLocal.Type))break;
                locals.Add(local);value=Unwrap(assign.Right);
            }
            if(locals.Count<2||!LiteralZero(value,model))continue;
            var earlier=block.Statements.OfType<LocalDeclarationStatementSyntax>().Where(d=>d.SpanStart<declaration.SpanStart&&SimpleLocal(d)&&d.Declaration.Variables[0].Initializer==null)
                .Where(d=>locals.Skip(1).Any(l=>Same(l,model.GetDeclaredSymbol(d.Declaration.Variables[0])))).ToArray();
            if(earlier.Length!=locals.Count-1)continue;
            SharedRootZero(input,text,body,block,declaration,value,locals,earlier,model,add);
            int start=earlier.Min(d=>d.SpanStart);string type=rootLocal.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            var later=block.Statements.OfType<LocalDeclarationStatementSyntax>().Where(d=>d.SpanStart>declaration.Span.End&&SimpleLocal(d)&&d.Declaration.Variables[0].Initializer!=null)
                .Where(d=>Same((model.GetDeclaredSymbol(d.Declaration.Variables[0]) as ILocalSymbol)?.Type,rootLocal.Type))
                .Where(d=>model.GetConstantValue(d.Declaration.Variables[0].Initializer.Value).HasValue&&!LiteralZero(d.Declaration.Variables[0].Initializer.Value,model)).Take(5);
            foreach(var future in later) {
                var v=future.Declaration.Variables[0];var chainNames=new[]{v.Identifier.ToString()}.Concat(locals.Select(l=>l.Name)).ToArray();
                string chain=string.Join(" = ",chainNames)+" = "+value+";";
                var edits=earlier.Select(d=>(d.SpanStart,d.Span.End,"")).ToList();
                edits.Add((declaration.SpanStart,declaration.Span.End,chain));
                edits.Add((future.SpanStart,future.Span.End,v.Identifier+" = "+v.Initializer.Value+";"));
                string prefix=string.Join("\n",chainNames.Select(n=>type+" "+n+";"))+"\n";
                add("restore-zero-chain-local",start,future.Span.End,prefix+Change(text,start,future.Span.End,edits));
            }
            if(LocalCount(input,"original_local_count","original_local_types")>LocalCount(input,"rebuilt_local_count","rebuilt_local_types"))foreach(var slot in unused.Where(s=>s.stores>0&&s.type==Key(rootLocal.Type))) {
                string dead=Fresh("originalDeadSlot"+slot.index,body);
                for(int position=0;position<=locals.Count;position++) {
                    var names=locals.Select(l=>l.Name).ToList();names.Insert(position,dead);
                    string chain=string.Join(" = ",names)+" = "+value+";";
                    var edits=earlier.Select(d=>(d.SpanStart,d.Span.End,"")).ToList();edits.Add((declaration.SpanStart,declaration.Span.End,chain));
                    add("restore-dead-zero-chain",start,declaration.Span.End,string.Join("\n",names.Select(n=>type+" "+n+";"))+"\n"+Change(text,start,declaration.Span.End,edits));
                }
            }
        }
    }
}
