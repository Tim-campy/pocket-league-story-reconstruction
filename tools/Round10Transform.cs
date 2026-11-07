// Local syntax hypotheses only. The IL oracle decides whether to accept any patch.
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

public static partial class Round7TransformSyntax {
    sealed class TargetScreenRewriter : CSharpSyntaxRewriter {
        readonly int KeepStart;
        public int StubbedBodies;
        public TargetScreenRewriter(int keepStart) { KeepStart=keepStart; }
        public override SyntaxNode VisitBlock(BlockSyntax block) {
            if((block.Parent is BaseMethodDeclarationSyntax || block.Parent is AccessorDeclarationSyntax) && block.Parent.SpanStart!=KeepStart) {
                StubbedBodies++;
                return SyntaxFactory.ParseStatement("{ throw new global::System.NotImplementedException(); }").WithTriviaFrom(block);
            }
            return base.VisitBlock(block);
        }
    }
    sealed class Patch {
        public string rule { get; set; }
        public int start { get; set; }
        public int end { get; set; }
        public string before { get; set; }
        public string after { get; set; }
        public int distance { get; set; }
    }
    static readonly Dictionary<string,SyntaxTree> Trees = new Dictionary<string,SyntaxTree>(StringComparer.OrdinalIgnoreCase);
    static CSharpCompilation Compilation;
    static string Full(string path) => Path.GetFullPath(path);
    static string TypeName(INamedTypeSymbol n) => n.ContainingType != null ? TypeName(n.ContainingType)+"/"+n.MetadataName : (n.ContainingNamespace.IsGlobalNamespace ? "" : n.ContainingNamespace+".")+n.MetadataName;
    static string Key(ITypeSymbol t) {
        if (t == null) return "UNKNOWN";
        if (t is ITypeParameterSymbol p) return (p.TypeParameterKind == TypeParameterKind.Method ? "!!" : "!")+p.Ordinal;
        if (t is IArrayTypeSymbol a) return Key(a.ElementType)+(a.Rank==1 ? "[]" : "["+new string(',',a.Rank-1)+"]");
        if (t is IPointerTypeSymbol ptr) return Key(ptr.PointedAtType)+"*";
        if (!(t is INamedTypeSymbol n)) return t.ToDisplayString();
        return TypeName(n.OriginalDefinition)+(n.TypeArguments.Length>0 ? "<"+string.Join(",",n.TypeArguments.Select(Key))+">" : "");
    }
    static string Method(IMethodSymbol m) => m == null ? null : TypeName(m.ContainingType)+"::"+m.MetadataName+(m.Arity==0 ? "" : "``"+m.Arity)+"("+string.Join(",",m.Parameters.Select(p=>Key(p.Type)+(p.RefKind==RefKind.None ? "" : "&")))+")->"+Key(m.ReturnType);
    static ExpressionSyntax Unwrap(ExpressionSyntax e) { while(e is ParenthesizedExpressionSyntax p)e=p.Expression; return e; }
    static bool Integral(ITypeSymbol t) => t != null && new[]{SpecialType.System_SByte,SpecialType.System_Byte,SpecialType.System_Int16,SpecialType.System_UInt16,SpecialType.System_Int32,SpecialType.System_UInt32,SpecialType.System_Int64,SpecialType.System_UInt64,SpecialType.System_Char}.Contains(t.SpecialType);
    static string Negate(ExpressionSyntax expression, SemanticModel model) {
        var e=Unwrap(expression);
        if(e is PrefixUnaryExpressionSyntax u && u.IsKind(SyntaxKind.LogicalNotExpression))return u.Operand.ToString();
        if(e is BinaryExpressionSyntax b) {
            if(b.IsKind(SyntaxKind.LogicalAndExpression)||b.IsKind(SyntaxKind.LogicalOrExpression))return "("+Negate(b.Left,model)+") "+(b.IsKind(SyntaxKind.LogicalAndExpression) ? "||" : "&&")+" ("+Negate(b.Right,model)+")";
            string op=null;
            if(b.IsKind(SyntaxKind.EqualsExpression))op="!=";
            if(b.IsKind(SyntaxKind.NotEqualsExpression))op="==";
            if(Integral(model.GetTypeInfo(b.Left).Type)&&Integral(model.GetTypeInfo(b.Right).Type)) {
                if(b.IsKind(SyntaxKind.LessThanExpression))op=">=";
                if(b.IsKind(SyntaxKind.LessThanOrEqualExpression))op=">";
                if(b.IsKind(SyntaxKind.GreaterThanExpression))op="<=";
                if(b.IsKind(SyntaxKind.GreaterThanOrEqualExpression))op="<";
            }
            if(op!=null)return b.Left+" "+op+" "+b.Right;
        }
        // Do not rewrite floating/lifted/unresolved relational operators.
        return "!("+e+")";
    }
    static IEnumerable<StatementSyntax> Statements(StatementSyntax s) => s is BlockSyntax b ? b.Statements : new[]{s};
    static string BodyText(StatementSyntax s) => string.Join("\n",Statements(s));
    static StatementSyntax Only(StatementSyntax s) => s is BlockSyntax b && b.Statements.Count==1 ? b.Statements[0] : s;
    static bool Terminates(StatementSyntax s) {
        s=Only(s);
        if(s is ReturnStatementSyntax || s is ThrowStatementSyntax || s is ContinueStatementSyntax || s is BreakStatementSyntax)return true;
        return s is BlockSyntax b && b.Statements.Count>0 && Terminates(b.Statements.Last());
    }
    static bool IsJump(StatementSyntax s) { s=Only(s);return s is ReturnStatementSyntax || s is ContinueStatementSyntax || s is BreakStatementSyntax || s is ThrowStatementSyntax; }
    static bool IsLoop(SyntaxNode n) => n is ForStatementSyntax || n is ForEachStatementSyntax || n is WhileStatementSyntax || n is DoStatementSyntax || n is ForEachVariableStatementSyntax;
    static string JumpKind(StatementSyntax s) => Only(s) is ContinueStatementSyntax ? "continue" : Only(s) is BreakStatementSyntax ? "break" : "return";
    static bool LiteralZero(ExpressionSyntax expression, SemanticModel model) {
        var constant=model.GetConstantValue(expression);
        if(!constant.HasValue||constant.Value==null)return false;
        try{return Convert.ToDouble(constant.Value)==0.0;}catch{return false;}
    }
    static int Distance(SyntaxNode n,int point) => n.Span.Contains(point) ? 0 : Math.Min(Math.Abs(point-n.SpanStart),Math.Abs(point-n.Span.End));
    static bool SimpleLocal(LocalDeclarationStatementSyntax declaration) => declaration.Declaration.Variables.Count==1 && declaration.UsingKeyword.IsKind(SyntaxKind.None) && declaration.Modifiers.Count==0;
    static object ScreenTarget(JsonElement input) {
        // Parse-only and stateless: screening must not replace the full syntax
        // tree used for semantic transformations in the persistent compilation.
        string source=input.GetProperty("source").GetString();int keep=input.GetProperty("keep_offset").GetInt32();
        var tree=CSharpSyntaxTree.ParseText(source);var root=tree.GetRoot();
        var declarations=root.DescendantNodes().Where(n=>n is BaseMethodDeclarationSyntax || n is AccessorDeclarationSyntax).ToArray();
        var declaration=declarations.FirstOrDefault(n=>n.SpanStart==keep) ?? declarations.Where(n=>n.Span.Contains(keep)).OrderBy(n=>n.Span.Length).FirstOrDefault();
        if(declaration==null)return new{error="screen_target_declaration_UNKNOWN",keep_offset=keep};
        var rewriter=new TargetScreenRewriter(declaration.SpanStart);var screened=rewriter.Visit(root);
        return new{source=screened.ToFullString(),kept_declaration_start=declaration.SpanStart,kept_declaration_end=declaration.Span.End,stubbed_bodies=rewriter.StubbedBodies,screening_only=true};
    }
    static object Respond(JsonElement input) {
        if(input.TryGetProperty("mode",out var mode)&&mode.GetString()=="screen-target")return ScreenTarget(input);
        string path=Full(input.GetProperty("path").GetString());
        if(!Trees.TryGetValue(path,out var tree)) {
            tree=CSharpSyntaxTree.ParseText(input.TryGetProperty("source",out var newSource) ? newSource.GetString() : File.ReadAllText(path),path:path);
            Trees[path]=tree;Compilation=Compilation.AddSyntaxTrees(tree);
        } else if(input.TryGetProperty("source",out var source) && source.GetString()!=tree.GetText().ToString()) {
            var replacement=CSharpSyntaxTree.ParseText(source.GetString(),path:path);
            Compilation=Compilation.ReplaceSyntaxTree(tree,replacement);Trees[path]=tree=replacement;
        }
        string text=tree.GetText().ToString();var sourceText=tree.GetText();var root=tree.GetRoot();
        if(input.TryGetProperty("mode",out var fieldMode) && fieldMode.GetString()=="map-static-initializers")
            return DescribeStaticInitializers(input,root,Compilation.GetSemanticModel(tree,true));
        int point;
        if(input.TryGetProperty("offset",out var offset))point=offset.GetInt32();
        else {
            int line=input.GetProperty("line").GetInt32()-1,column=input.TryGetProperty("column",out var col)?col.GetInt32()-1:0;
            if(line<0||line>=sourceText.Lines.Count)return new{error="source_line_out_of_range",line=line+1,line_count=sourceText.Lines.Count};
            point=sourceText.Lines[line].Start+Math.Max(0,Math.Min(column,sourceText.Lines[line].Span.Length));
        }
        point=Math.Max(0,Math.Min(point,Math.Max(0,text.Length-1)));
        var token=root.FindToken(point);var node=token.Parent;
        var methodDeclaration=node.AncestorsAndSelf().FirstOrDefault(n=>n is BaseMethodDeclarationSyntax || n is AccessorDeclarationSyntax);
        if(methodDeclaration==null)return RespondStaticInitializer(input,text,node,Compilation.GetSemanticModel(tree,true),point);
        var model=Compilation.GetSemanticModel(tree,true);var symbol=model.GetDeclaredSymbol(methodDeclaration) as IMethodSymbol;
        var methodBody=methodDeclaration is BaseMethodDeclarationSyntax m ? m.Body : ((AccessorDeclarationSyntax)methodDeclaration).Body;
        if(methodBody==null)return new{error="method_has_no_block"};
        StatementSyntax mapped=node.AncestorsAndSelf().OfType<StatementSyntax>().FirstOrDefault(s=>!(s is BlockSyntax));
        if(mapped==null || mapped==methodBody) {
            mapped=methodBody.Statements.FirstOrDefault(s=>s.Span.End>point) ?? methodBody.Statements.LastOrDefault();
        }
        if(mapped==null)return new{error="empty_body"};
        // A sequence point within an if condition belongs to the whole if command.
        var conditional=node.AncestorsAndSelf().OfType<IfStatementSyntax>().FirstOrDefault(s=>s.Condition.FullSpan.Contains(point));
        if(conditional!=null)mapped=conditional;
        var patches=new List<Patch>();var seen=new HashSet<string>();
        Action<string,SyntaxNode,string> add=(rule,n,after)=> {
            if(n.SpanStart<methodBody.SpanStart||n.Span.End>methodBody.Span.End)return;
            string before=text.Substring(n.SpanStart,n.Span.Length);
            if(before==after||!seen.Add(n.SpanStart+":"+n.Span.End+":"+after))return;
            patches.Add(new Patch{rule=rule,start=n.SpanStart,end=n.Span.End,before=before,after=after,distance=Distance(n,point)});
        };
        // Individual nodes in the mapped statement, not a blanket method rewrite.
        AddRound7LegacyRules(input,text,mapped,methodBody,model,point,patches,seen);
        AddRound7CalculatedRules(input,text,mapped,methodBody,model,point,patches,seen);
        AddRound7CalculatedParameterAlias(input,text,mapped,methodBody,model,point,patches,seen);
        AddRound7SmcsRules(input,text,mapped,methodBody,model,point,patches,seen);
        AddRound7RoslynRules(input,text,mapped,methodBody,model,point,patches,seen);
        var cycleRules=new List<string>();
        if(input.TryGetProperty("round7_cycle_rules",out var learned)&&learned.ValueKind==JsonValueKind.Array)
            cycleRules.AddRange(learned.EnumerateArray().Where(v=>v.ValueKind==JsonValueKind.String).Select(v=>v.GetString()));
        if(input.TryGetProperty("round7_cycle_rule",out var selectedCycle)&&selectedCycle.ValueKind==JsonValueKind.String)
            cycleRules.Add(selectedCycle.GetString());
        foreach(string cycleRule in cycleRules.Where(r=>!string.IsNullOrEmpty(r)).Distinct()) {
            var properties=input.EnumerateObject().ToDictionary(p=>p.Name,p=>p.Value.Clone());
            properties["round7_cycle_rule"]=JsonSerializer.SerializeToElement(cycleRule);
            using(var cycleRequest=JsonDocument.Parse(JsonSerializer.Serialize(properties))) {
                AddRound7SmcsCycleRules(cycleRequest.RootElement,text,mapped,methodBody,model,point,patches,seen);
                AddRound7SmcsExitRules(cycleRequest.RootElement,text,mapped,methodBody,model,point,patches,seen);
                AddRound7SmcsGuardRules(cycleRequest.RootElement,text,mapped,methodBody,model,point,patches,seen);
                AddRound7StringConcatRules(cycleRequest.RootElement,text,mapped,methodBody,model,point,patches,seen);
                AddRound7LoopConditionRules(cycleRequest.RootElement,text,mapped,methodBody,model,point,patches,seen);
                AddRound7SmcsSwitchRules(cycleRequest.RootElement,text,mapped,methodBody,model,point,patches,seen);
                AddRound7RoslynDeclarationRules(cycleRequest.RootElement,text,mapped,methodBody,model,point,patches,seen);
                AddRound7TerminalArmRules(cycleRequest.RootElement,text,mapped,methodBody,model,point,patches,seen);
                AddRound7BalancedBranchRules(cycleRequest.RootElement,text,mapped,methodBody,model,point,patches,seen);
                AddRound7InfiniteTailGuardRules(cycleRequest.RootElement,text,mapped,methodBody,model,point,patches,seen);
                AddRound10RootGuardRules(cycleRequest.RootElement,text,mapped,methodBody,model,point,patches,seen);
                AddRound7RoslynCycleRules(cycleRequest.RootElement,text,mapped,methodBody,model,point,patches,seen);
            }
        }
        var nodes=mapped.DescendantNodesAndSelf().Where(n=>!n.Ancestors().TakeWhile(a=>a!=mapped).OfType<AnonymousFunctionExpressionSyntax>().Any(a=>!a.FullSpan.Contains(point))).ToArray();
        // Local-table order is a source declaration-order hypothesis. Initializers
        // remain at their old command positions; only declarations are hoisted.
        foreach(var declaration in nodes.OfType<LocalDeclarationStatementSyntax>().Where(SimpleLocal)) {
            if(!(declaration.Parent is BlockSyntax block))continue;
            var variable=declaration.Declaration.Variables[0];var local=model.GetDeclaredSymbol(variable) as ILocalSymbol;
            if(local==null||local.IsConst)continue;
            int index=block.Statements.IndexOf(declaration);
            string ownType=local.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            string ownName=variable.Identifier.ToString();
            if(variable.Initializer!=null) {
                // Move this declaration earlier, preserving its assignment.
                var preceding=block.Statements.Take(index).OfType<LocalDeclarationStatementSyntax>().Where(SimpleLocal).TakeLast(6).ToArray();
                foreach(var anchor in preceding) {
                    int start=anchor.SpanStart,end=declaration.Span.End;
                    string before=text.Substring(start,end-start);
                    string middle=text.Substring(start,declaration.SpanStart-start);
                    string after=ownType+" "+ownName+";\n"+middle+ownName+" = "+variable.Initializer.Value+";";
                    if(seen.Add(start+":"+end+":"+after))patches.Add(new Patch{rule="local-declaration-order-earlier",start=start,end=end,before=before,after=after,distance=Distance(declaration,point)});
                }
            }
            // A later declaration may occupy an earlier original slot. Test
            // one declaration and short prefixes, without moving evaluations.
            var following=block.Statements.Skip(index+1).OfType<LocalDeclarationStatementSyntax>().Where(SimpleLocal).Where(d=>d.Declaration.Variables[0].Initializer!=null).Take(5).ToArray();
            foreach(int count in Enumerable.Range(1,following.Length)) {
                var chosen=following.Take(count).ToArray();
                var hoisted=new List<string>();bool valid=true;
                foreach(var later in chosen) {
                    var v=later.Declaration.Variables[0];var l=model.GetDeclaredSymbol(v) as ILocalSymbol;
                    if(l==null||l.IsConst){valid=false;break;}
                    hoisted.Add(l.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)+" "+v.Identifier+";");
                }
                if(!valid)continue;
                int start=declaration.SpanStart,end=chosen.Last().Span.End;
                string before=text.Substring(start,end-start),changed=before;
                foreach(var later in chosen.Reverse()) {
                    var v=later.Declaration.Variables[0];
                    string assignment=v.Identifier+" = "+v.Initializer.Value+";";
                    int relative=later.SpanStart-start;
                    changed=changed.Substring(0,relative)+assignment+changed.Substring(relative+later.Span.Length);
                }
                string after=string.Join("\n",hoisted)+"\n"+changed;
                if(seen.Add(start+":"+end+":"+after))patches.Add(new Patch{rule="local-declaration-order-later",start=start,end=end,before=before,after=after,distance=Distance(declaration,point)});
            }
        }
        // smcs often retained method-scope declarations that the decompiler put
        // inside a for/if body. Group later declarations by actual type so an
        // integral pair can precede a loop's index without hoisting its float
        // temporary. This is still a source hypothesis, never an IL rewrite.
        StatementSyntax scopeAnchor=mapped;
        while(scopeAnchor.Parent!=methodBody && scopeAnchor.Parent!=null) {
            var outer=scopeAnchor.Ancestors().OfType<StatementSyntax>().FirstOrDefault(s=>s.Parent==methodBody);
            if(outer==null)break;scopeAnchor=outer;
        }
        if(scopeAnchor.Parent==methodBody) {
            var laterLocals=methodBody.DescendantNodes().OfType<LocalDeclarationStatementSyntax>()
                .Where(d=>d.SpanStart>mapped.SpanStart&&SimpleLocal(d))
                .Where(d=>!d.Ancestors().TakeWhile(a=>a!=methodBody).Any(a=>a is AnonymousFunctionExpressionSyntax))
                .Select(d=>new{node=d,variable=d.Declaration.Variables[0],local=model.GetDeclaredSymbol(d.Declaration.Variables[0]) as ILocalSymbol})
                .Where(item=>item.local!=null&&!item.local.IsConst).Take(20).ToArray();
            foreach(var group in laterLocals.GroupBy(item=>Key(item.local.Type))) {
                var possible=group.Take(5).ToArray();
                foreach(int count in Enumerable.Range(1,possible.Length)) {
                    var chosen=possible.Take(count).ToArray();int start=scopeAnchor.SpanStart,end=chosen.Last().node.Span.End;
                    if(end<=start)continue;
                    string before=text.Substring(start,end-start),changed=before;
                    foreach(var item in chosen.Reverse()) {
                        string replacement=item.variable.Initializer==null?"":item.variable.Identifier+" = "+item.variable.Initializer.Value+";";
                        int relative=item.node.SpanStart-start;
                        changed=changed.Substring(0,relative)+replacement+changed.Substring(relative+item.node.Span.Length);
                    }
                    string declarations=string.Join("\n",chosen.Select(item=>item.local.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)+" "+item.variable.Identifier+";"));
                    string after=declarations+"\n"+changed;
                    if(seen.Add(start+":"+end+":"+after))patches.Add(new Patch{rule="local-declaration-scope-hoist",start=start,end=end,before=before,after=after,distance=Distance(mapped,point)});
                }
            }
        }
        foreach(var tern in nodes.OfType<ConditionalExpressionSyntax>())
            add("ternary-polarity",tern,"("+Negate(tern.Condition,model)+") ? ("+tern.WhenFalse+") : ("+tern.WhenTrue+")");
        foreach(var binary in nodes.OfType<BinaryExpressionSyntax>()) {
            if(binary.IsKind(SyntaxKind.SubtractExpression)&&LiteralZero(binary.Left,model))
                add("unary-negation",binary,"-("+binary.Right+")");
        }
        foreach(var assignment in nodes.OfType<AssignmentExpressionSyntax>().Where(a=>a.IsKind(SyntaxKind.SimpleAssignmentExpression))) {
            if(Unwrap(assignment.Right) is ConditionalExpressionSyntax choice && assignment.Parent is ExpressionStatementSyntax statement) {
                string left=assignment.Left.ToString();
                add("assignment-ternary-if",statement,"if ("+choice.Condition+") { "+left+" = "+choice.WhenTrue+"; } else { "+left+" = "+choice.WhenFalse+"; }");
                add("assignment-ternary-if-reverse",statement,"if ("+Negate(choice.Condition,model)+") { "+left+" = "+choice.WhenFalse+"; } else { "+left+" = "+choice.WhenTrue+"; }");
            }
            if(!(Unwrap(assignment.Right) is AssignmentExpressionSyntax inner)||!inner.IsKind(SyntaxKind.SimpleAssignmentExpression))continue;
            var outerSymbol=model.GetSymbolInfo(assignment.Left).Symbol;var innerSymbol=model.GetSymbolInfo(inner.Left).Symbol;
            // Exchange only a local and a field, with side-effect-free field receiver.
            IFieldSymbol field=outerSymbol as IFieldSymbol ?? innerSymbol as IFieldSymbol;
            bool localField=(outerSymbol is ILocalSymbol&&innerSymbol is IFieldSymbol)||(outerSymbol is IFieldSymbol&&innerSymbol is ILocalSymbol);
            ExpressionSyntax fieldExpression=outerSymbol is IFieldSymbol?assignment.Left:inner.Left;
            bool safeReceiver=fieldExpression is IdentifierNameSyntax || fieldExpression is MemberAccessExpressionSyntax access && (access.Expression is ThisExpressionSyntax || access.Expression is IdentifierNameSyntax);
            if(field!=null&&localField&&safeReceiver)
                add("assignment-chain-order",assignment,inner.Left+" = ("+assignment.Left+" = "+inner.Right+")");
        }
        foreach(var localStatement in nodes.OfType<LocalDeclarationStatementSyntax>().Where(SimpleLocal)) {
            var variable=localStatement.Declaration.Variables[0];
            if(variable.Initializer==null||!(Unwrap(variable.Initializer.Value) is ConditionalExpressionSyntax choice))continue;
            var local=model.GetDeclaredSymbol(variable) as ILocalSymbol;if(local==null||local.IsConst)continue;
            string prefix=local.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)+" "+variable.Identifier+";\n";
            add("declaration-ternary-if",localStatement,prefix+"if ("+choice.Condition+") { "+variable.Identifier+" = "+choice.WhenTrue+"; } else { "+variable.Identifier+" = "+choice.WhenFalse+"; }");
            add("declaration-ternary-if-reverse",localStatement,prefix+"if ("+Negate(choice.Condition,model)+") { "+variable.Identifier+" = "+choice.WhenFalse+"; } else { "+variable.Identifier+" = "+choice.WhenTrue+"; }");
        }
        foreach(var branch in nodes.OfType<IfStatementSyntax>()) {
            if(branch.Else!=null) {
                add("if-polarity",branch,"if ("+Negate(branch.Condition,model)+") "+branch.Else.Statement+" else "+branch.Statement);
                if(Terminates(branch.Statement))add("guard-"+JumpKind(branch.Statement),branch,"if ("+branch.Condition+") "+branch.Statement+"\n"+BodyText(branch.Else.Statement));
                if(Terminates(branch.Else.Statement))add("guard-"+JumpKind(branch.Else.Statement)+"-reverse",branch,"if ("+Negate(branch.Condition,model)+") "+branch.Else.Statement+"\n"+BodyText(branch.Statement));
            }
            bool tailInBlock=branch.Parent is BlockSyntax parentBlock && parentBlock.Statements.Last()==branch;
            if(branch.Else==null&&tailInBlock&&symbol!=null&&symbol.ReturnsVoid&&Round7TailToMethod(branch,methodBody)) {
                add("tail-early-return",branch,"if ("+Negate(branch.Condition,model)+") { return; }\n"+BodyText(branch.Statement));
            }
            ExpressionSyntax condition=Unwrap(branch.Condition);bool negative=false;
            if(condition is PrefixUnaryExpressionSyntax not&&not.IsKind(SyntaxKind.LogicalNotExpression)){condition=Unwrap(not.Operand);negative=true;}
            if(model.GetTypeInfo(condition).Type?.SpecialType==SpecialType.System_Boolean && !(condition is IdentifierNameSyntax) && !(condition is LiteralExpressionSyntax)) {
                string name="recoveredCondition_"+branch.SpanStart;
                add("condition-local",branch,"{ bool "+name+" = "+condition+";\n"+branch.WithCondition(SyntaxFactory.ParseExpression((negative?"!":"")+name))+"\n}");
            }
            var unwrapped=Unwrap(branch.Condition);
            if(branch.Else==null&&unwrapped is BinaryExpressionSyntax logic) {
                if(logic.IsKind(SyntaxKind.LogicalAndExpression)) {
                    add("condition-split-and",branch,"if ("+logic.Left+") { if ("+logic.Right+") "+branch.Statement+" }");
                    // A sequence of individual loop guards is often lowered differently by smcs.
                    if(tailInBlock && branch.Parent.Parent!=null && IsLoop(branch.Parent.Parent))
                        add("loop-continue-guards",branch,"if ("+Negate(logic.Left,model)+") { continue; }\nif ("+Negate(logic.Right,model)+") { continue; }\n"+BodyText(branch.Statement));
                }
                if(logic.IsKind(SyntaxKind.LogicalOrExpression)&&IsJump(branch.Statement))
                    add("condition-split-or-guards",branch,"if ("+logic.Left+") "+branch.Statement+"\nif ("+logic.Right+") "+branch.Statement);
            }
            if(branch.Else==null&&Only(branch.Statement) is IfStatementSyntax innerIf&&innerIf.Else==null)
                add("condition-merge-and",branch,"if (("+branch.Condition+") && ("+innerIf.Condition+")) "+innerIf.Statement);
            if(branch.Else==null&&tailInBlock&&branch.Parent.Parent!=null&&IsLoop(branch.Parent.Parent))
                add("loop-continue-guard",branch,"if ("+Negate(branch.Condition,model)+") { continue; }\n"+BodyText(branch.Statement));
            // Two adjacent same-kind guards can merge; this patch still starts at the mapped guard.
            if(branch.Else==null&&IsJump(branch.Statement)&&branch.Parent is BlockSyntax sequence) {
                int index=sequence.Statements.IndexOf(branch);
                if(index+1<sequence.Statements.Count&&sequence.Statements[index+1] is IfStatementSyntax next&&next.Else==null&&Only(branch.Statement).ToString()==Only(next.Statement).ToString()) {
                    var span=TextSpan.FromBounds(branch.SpanStart,next.Span.End);
                    string before=text.Substring(span.Start,span.Length),after="if (("+branch.Condition+") || ("+next.Condition+")) "+branch.Statement;
                    if(seen.Add(span.Start+":"+span.End+":"+after))patches.Add(new Patch{rule="condition-merge-or-guards",start=span.Start,end=span.End,before=before,after=after,distance=Distance(branch,point)});
                }
            }
        }
        foreach(var ret in nodes.OfType<ReturnStatementSyntax>().Where(r=>r.Expression!=null)) {
            if(Unwrap(ret.Expression) is ConditionalExpressionSyntax tern) {
                add("value-guard",ret,"{ if ("+tern.Condition+") { return "+tern.WhenTrue+"; } return "+tern.WhenFalse+"; }");
                add("value-guard-reverse",ret,"{ if ("+Negate(tern.Condition,model)+") { return "+tern.WhenFalse+"; } return "+tern.WhenTrue+"; }");
            }
            var returnType=model.GetTypeInfo(ret.Expression).ConvertedType ?? symbol?.ReturnType;
            if(returnType!=null&&returnType.SpecialType!=SpecialType.System_Void)
                add("return-local",ret,"{ "+returnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)+" recoveredReturn_"+ret.SpanStart+" = "+ret.Expression+"; return recoveredReturn_"+ret.SpanStart+"; }");
        }
        foreach(var sw in nodes.OfType<SwitchStatementSyntax>()) {
            var inputSymbol=model.GetSymbolInfo(sw.Expression).Symbol;
            if(!(inputSymbol is ILocalSymbol||inputSymbol is IParameterSymbol||inputSymbol is IFieldSymbol)||sw.Sections.Count>8)continue;
            var pieces=new List<string>();string otherwise=null;bool valid=true;
            foreach(var section in sw.Sections) {
                var statements=section.Statements.ToList();if(statements.Count==0){valid=false;break;}
                if(statements.Take(statements.Count-1).Any(s=>s.DescendantNodesAndSelf().Any(n=>n is BreakStatementSyntax||n is GotoStatementSyntax))){valid=false;break;}
                if(statements.Last() is BreakStatementSyntax)statements.RemoveAt(statements.Count-1);
                else if(!Terminates(statements.Last())){valid=false;break;}
                string block="{ "+string.Join("\n",statements)+" }";
                if(section.Labels.Any(l=>l is DefaultSwitchLabelSyntax)){otherwise=block;continue;}
                var labels=section.Labels.OfType<CaseSwitchLabelSyntax>().ToArray();if(labels.Length!=section.Labels.Count){valid=false;break;}
                pieces.Add("if ("+string.Join(" || ",labels.Select(l=>"("+sw.Expression+" == "+l.Value+")"))+") "+block);
            }
            if(valid&&pieces.Count>0)add("switch-if",sw,string.Join(" else ",pieces)+(otherwise==null?"":" else "+otherwise));
        }
        foreach(var loop in nodes.OfType<WhileStatementSyntax>().Where(w=>!w.Condition.IsKind(SyntaxKind.TrueLiteralExpression))) {
            if(Unwrap(loop.Condition) is LiteralExpressionSyntax)continue;
            string name="completed_"+loop.SpanStart;
            // The temporary is declared inside each loop iteration, changing capture scope.
            add("loop-cache-scope",loop,"while (true) { bool "+name+" = "+loop.Condition+"; if (!"+name+") { break; }\n"+BodyText(loop.Statement)+"\n}");
            if(Unwrap(loop.Condition) is PrefixUnaryExpressionSyntax negated&&negated.IsKind(SyntaxKind.LogicalNotExpression))
                add("loop-cache-scope-positive",loop,"while (true) { bool "+name+" = "+negated.Operand+"; if ("+name+") { break; }\n"+BodyText(loop.Statement)+"\n}");
        }
        int max=input.TryGetProperty("max_candidates",out var maximum)?maximum.GetInt32():64;
        var startPos=sourceText.Lines.GetLinePosition(mapped.SpanStart);
        return new {declaration_start=methodDeclaration.SpanStart,declaration_end=methodDeclaration.Span.End,mapped=new{start=mapped.SpanStart,end=mapped.Span.End,line=startPos.Line+1,column=startPos.Character+1,text=mapped.ToString(),kind=mapped.Kind().ToString(),method_id=Method(symbol),point=point,declaration_start=methodDeclaration.SpanStart,declaration_end=methodDeclaration.Span.End},patches=patches.OrderBy(p=>p.distance).ThenBy(p=>p.end-p.start).Take(Math.Max(0,max)).ToArray(),candidate_count=patches.Count};
    }
    public static void Main(string[] args) {
        var manifest=JsonDocument.Parse(File.ReadAllText(args[0])).RootElement;
        foreach(var item in manifest.GetProperty("sources").EnumerateArray()) {
            string logical=item.ValueKind==JsonValueKind.String?item.GetString():item.GetProperty("path").GetString();
            string input=item.ValueKind==JsonValueKind.Object&&item.TryGetProperty("input_path",out var ip)?ip.GetString():logical;
            string path=Full(logical);Trees[path]=CSharpSyntaxTree.ParseText(File.ReadAllText(input),path:path);
        }
        var refs=manifest.GetProperty("references").EnumerateArray().Select(item=>MetadataReference.CreateFromFile(item.ValueKind==JsonValueKind.String?item.GetString():item.GetProperty("path").GetString()));
        Compilation=CSharpCompilation.Create("Round4Recovered",Trees.Values,refs,new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        string line;
        while((line=Console.ReadLine())!=null) {
            try { using(var request=JsonDocument.Parse(line))Console.WriteLine(JsonSerializer.Serialize(Respond(request.RootElement))); }
            catch(Exception error) {Console.WriteLine(JsonSerializer.Serialize(new{error=error.GetType().Name,message=error.Message}));}
        }
    }
}
