// Source-only model of the Unity Mono 2.6.5 local allocator. No Emit is called.
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static class Round7SlotModel {
    static Dictionary<string,SyntaxTree> Trees=new Dictionary<string,SyntaxTree>(StringComparer.OrdinalIgnoreCase);
    static CSharpCompilation Compilation;
    sealed class CaptureScope {
        public string method; public int start; public int order; public string type;
        public HashSet<int> locals=new HashSet<int>();
    }
    static Dictionary<string,List<CaptureScope>> BaseCaptures=new Dictionary<string,List<CaptureScope>>(StringComparer.OrdinalIgnoreCase);
    static List<CaptureScope> Captures(SyntaxTree tree,SemanticModel sem) {
        var output=new List<CaptureScope>();
        foreach(var dec in tree.GetRoot().DescendantNodes().Where(n=>n is BaseMethodDeclarationSyntax || n is AccessorDeclarationSyntax)) {
            var method=sem.GetDeclaredSymbol(dec) as IMethodSymbol;if(method==null)continue;
            var scopes=new Dictionary<int,CaptureScope>();
            foreach(var lambda in dec.DescendantNodes().OfType<AnonymousFunctionExpressionSyntax>()) {
                foreach(var name in lambda.DescendantNodes().OfType<IdentifierNameSyntax>()) {
                    var sym=sem.GetSymbolInfo(name).Symbol;
                    if(!(sym is ILocalSymbol) && !(sym is IParameterSymbol))continue;
                    var syntax=sym.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax();if(syntax==null||lambda.Span.Contains(syntax.Span)||!dec.Span.Contains(syntax.Span))continue;
                    // Variables are hoisted into their ExplicitBlock, not the parser's
                    // implicit declaration-after-statement block. Parameters use the body.
                    var scope=sym is IParameterSymbol ? dec.ChildNodes().OfType<BlockSyntax>().FirstOrDefault() : syntax.Ancestors().OfType<BlockSyntax>().FirstOrDefault();
                    if(scope==null)continue;
                    if(!scopes.TryGetValue(scope.SpanStart,out var capture)) {
                        capture=new CaptureScope{method=Method(method),start=scope.SpanStart,order=name.SpanStart};scopes.Add(scope.SpanStart,capture);output.Add(capture);
                    }
                    if(sym is ILocalSymbol)capture.locals.Add(syntax.SpanStart);
                }
            }
        }
        return output.OrderBy(s=>s.order).ToList();
    }
    static List<CaptureScope> NumberCaptures(string path,List<CaptureScope> replacement) {
        var all=new List<CaptureScope>();int ordinal=0;
        foreach(var pair in Trees)foreach(var capture in (string.Equals(pair.Key,path,StringComparison.OrdinalIgnoreCase)?replacement:BaseCaptures[pair.Key])) {
            var copy=new CaptureScope{method=capture.method,start=capture.start,order=capture.order,locals=capture.locals};
            var split=copy.method.IndexOf("::");var end=copy.method.IndexOf('(',split);string name=copy.method.Substring(split+2,end-split-2);
            copy.type=copy.method.Substring(0,split)+"/<"+name+">c__AnonStorey"+(ordinal++).ToString("X");all.Add(copy);
        }
        return all.Where(c=>replacement.Any(r=>r.method==c.method&&r.start==c.start)).ToList();
    }
    static string TypeName(INamedTypeSymbol n) => n.ContainingType!=null ? TypeName(n.ContainingType)+"/"+n.MetadataName : (n.ContainingNamespace.IsGlobalNamespace?"":n.ContainingNamespace+".")+n.MetadataName;
    static string Key(ITypeSymbol t) {
        if(t==null)return "UNKNOWN";
        if(t is ITypeParameterSymbol p)return (p.TypeParameterKind==TypeParameterKind.Method?"!!":"!")+p.Ordinal;
        if(t is IArrayTypeSymbol a)return Key(a.ElementType)+(a.Rank==1?"[]":"["+new string(',',a.Rank-1)+"]");
        if(t is IPointerTypeSymbol ptr)return Key(ptr.PointedAtType)+"*";
        if(!(t is INamedTypeSymbol n))return t.ToDisplayString();
        var args=new List<ITypeSymbol>();var parents=new Stack<INamedTypeSymbol>();for(var container=n;container!=null;container=container.ContainingType)parents.Push(container);foreach(var container in parents)args.AddRange(container.TypeArguments);
        return TypeName(n.OriginalDefinition)+(args.Count>0?"<"+string.Join(",",args.Select(Key))+">":"");
    }
    static string Method(IMethodSymbol m) => m==null?null:TypeName(m.ContainingType)+"::"+m.MetadataName+(m.Arity==0?"":"``"+m.Arity)+"("+string.Join(",",m.Parameters.Select(p=>Key(p.Type)+(p.RefKind==RefKind.None?"":"&")))+")->"+Key(m.ReturnType);
    sealed class Local {
        public string name {get;set;} public string type {get;set;} public string kind {get;set;}
        public int start {get;set;} public int end {get;set;} public int slot {get;set;}=-1;
        public int block {get;set;} public string type_source {get;set;}
        public int statement_start {get;set;}=-1; public int statement_end {get;set;}=-1;
        public int lexical_scope_start {get;set;}=-1; public int lexical_scope_end {get;set;}=-1;
        public string initializer {get;set;}
    }
    sealed class Block {
        public int id {get;set;} public string kind {get;set;} public int start {get;set;} public int end {get;set;}
        public int parent {get;set;} public bool used {get;set;}
        public List<Local> variables {get;set;}=new List<Local>();
        public List<Local> registered {get;set;}=new List<Local>();
        [System.Text.Json.Serialization.JsonIgnore]
        public List<Block> children {get;set;}=new List<Block>();
    }
    sealed class Model {
        public SemanticModel Sem; public SyntaxNode Declaration; public BlockSyntax Body; public IMethodSymbol Symbol;
        public List<string> Errors=new List<string>(); public List<Local> Slots=new List<Local>(); public List<Block> Blocks=new List<Block>();
        public Dictionary<string,Stack<int>> Free=new Dictionary<string,Stack<int>>(); public int ReturnSlot=-1;
        public List<CaptureScope> CaptureScopes;
        public List<string> RelaxedDiagnostics=new List<string>();
        public List<string> DiagnosticEvidence=new List<string>();
        public Model(SemanticModel sem,SyntaxNode dec,BlockSyntax body,IMethodSymbol symbol,List<CaptureScope> captures){Sem=sem;Declaration=dec;Body=body;Symbol=symbol;CaptureScopes=captures.Where(c=>c.method==Method(symbol)).ToList();}
        public Block New(Block parent,SyntaxNode node,string kind){var b=new Block{id=Blocks.Count,parent=parent==null?-1:parent.id,start=node.SpanStart,end=node.Span.End,kind=kind};Blocks.Add(b);if(parent!=null)parent.children.Add(b);return b;}
        public string Type(SyntaxNode n){return Key(Sem.GetTypeInfo(n).Type);}
        void Add(Block b,string name,string type,SyntaxNode node,string kind,string syntaxType=null){var v=new Local{name=name,type=type,start=node.SpanStart,end=node.Span.End,block=b.id,kind=kind,type_source=syntaxType};
            if(node is VariableDeclaratorSyntax variable && variable.Parent?.Parent is LocalDeclarationStatementSyntax statement && statement.Parent is BlockSyntax scope){v.statement_start=statement.SpanStart;v.statement_end=statement.Span.End;v.lexical_scope_start=scope.SpanStart;v.lexical_scope_end=scope.Span.End;v.initializer=variable.Initializer?.Value.ToString();}
            if(type=="UNKNOWN"||type.Contains("?"))Errors.Add("unresolved_type:"+name);(kind=="declared"?b.variables:b.registered).Add(v);}
        void DeclarationVars(Block b,VariableDeclarationSyntax d,bool isConst=false){foreach(var v in d.Variables){var s=Sem.GetDeclaredSymbol(v) as ILocalSymbol;if(!isConst)Add(b,v.Identifier.ValueText,Key(s?.Type),v,"declared",d.Type.ToString());}}
        bool Nested(SyntaxNode n)=>n is AnonymousFunctionExpressionSyntax || n is LocalFunctionStatementSyntax;
        IEnumerable<SyntaxNode> Own(SyntaxNode n)=>n.DescendantNodesAndSelf(x=>!Nested(x));
        void Expressions(SyntaxNode n) {
            // Expression lowerings which allocate additional locals need an explicit
            // rule; a candidate is never approved by guessing an omitted temporary.
            foreach(var x in Own(n)) {
                if(x is AnonymousFunctionExpressionSyntax a && a.DescendantNodes().OfType<AnonymousFunctionExpressionSyntax>().Any())Errors.Add("nested_anonymous_function");
                if(x is AwaitExpressionSyntax)Errors.Add("async_lowering");
                if(x is StackAllocArrayCreationExpressionSyntax)Errors.Add("stackalloc");
                if(x is ConditionalAccessExpressionSyntax)Errors.Add("unsupported_smcs_syntax");
                if(x is ObjectCreationExpressionSyntax o && o.Initializer!=null && (!o.Initializer.IsKind(SyntaxKind.ObjectInitializerExpression)||o.Initializer.Expressions.Any(v=>v is not AssignmentExpressionSyntax a||a.Right is InitializerExpressionSyntax)))Errors.Add("unsupported_initializer");
            }
        }
        Block Sequence(Block current,IEnumerable<StatementSyntax> statements) {
            foreach(var s in statements) {
                if(s is LocalDeclarationStatementSyntax local) {
                    if(current.used)current=New(current,s,"implicit-after-statement");
                    DeclarationVars(current,local.Declaration,local.Modifiers.Any(SyntaxKind.ConstKeyword));
                    if(local.Declaration.Variables.Any(v=>v.Initializer!=null)){current.used=true;foreach(var v in local.Declaration.Variables)if(v.Initializer!=null)Expressions(v.Initializer);}
                } else { ResolveStatement(current,s);current.used=true; }
            }
            return current;
        }
        void Embedded(Block b,StatementSyntax s){if(s==null)return;if(s is BlockSyntax block)Sequence(New(b,block,"explicit"),block.Statements);else ResolveStatement(b,s);}
        void ResolveStatement(Block b,StatementSyntax s) {
            if(s is BlockSyntax plain){Sequence(New(b,plain,"explicit"),plain.Statements);return;}
            if(s is IfStatementSyntax i){Expressions(i.Condition);Embedded(b,i.Statement);Embedded(b,i.Else?.Statement);return;}
            if(s is WhileStatementSyntax w){Expressions(w.Condition);Embedded(b,w.Statement);return;}
            if(s is DoStatementSyntax d){Embedded(b,d.Statement);Expressions(d.Condition);return;}
            if(s is ForStatementSyntax f){var child=New(b,f,"for");if(f.Declaration!=null){DeclarationVars(child,f.Declaration);Expressions(f.Declaration);}foreach(var e in f.Initializers)Expressions(e);if(f.Condition!=null)Expressions(f.Condition);foreach(var e in f.Incrementors)Expressions(e);Embedded(child,f.Statement);return;}
            if(s is ForEachStatementSyntax each){
                var child=New(b,each,"foreach");var sym=Sem.GetDeclaredSymbol(each) as ILocalSymbol;Add(child,each.Identifier.ValueText,Key(sym?.Type),each,"declared",each.Type.ToString());
                var type=Sem.GetTypeInfo(each.Expression).Type;Expressions(each.Expression);
                if(type is IArrayTypeSymbol array){Add(child,"$array",Key(type),each.Expression,"foreach-array-copy");for(int rank=0;rank<array.Rank;rank++){Add(child,"$index"+rank,"System.Int32",each,"foreach-index");if(array.Rank>1)Add(child,"$length"+rank,"System.Int32",each,"foreach-length");}}
                else if(type?.SpecialType==SpecialType.System_String){Errors.Add("foreach_string_lowering");}
                else {var info=Sem.GetForEachStatementInfo(each);Add(child,"$enumerator",Key(info.GetEnumeratorMethod?.ReturnType),each.Expression,"foreach-enumerator");if(info.GetEnumeratorMethod?.ReturnType?.IsValueType!=true && !(info.GetEnumeratorMethod?.ReturnType as INamedTypeSymbol)?.AllInterfaces.Any(t=>t.SpecialType==SpecialType.System_IDisposable)==true)Errors.Add("foreach_runtime_dispose");}
                Embedded(child,each.Statement);return;
            }
            if(s is LockStatementSyntax l){Expressions(l.Expression);Embedded(b,l.Statement);Add(b,"$lock",Type(l.Expression),l.Expression,"lock-copy");return;}
            if(s is UsingStatementSyntax u){var child=u.Declaration!=null?New(b,u,"using-declaration"):b;if(u.Declaration!=null){DeclarationVars(child,u.Declaration);Expressions(u.Declaration);}else{Expressions(u.Expression);Add(child,"$using",Type(u.Expression),u.Expression,"using-copy");}Embedded(child,u.Statement);return;}
            if(s is SwitchStatementSyntax sw){Expressions(sw.Expression);Block current=b;foreach(var section in sw.Sections){current=New(current,section,"switch-section");Sequence(current,section.Statements);}return;}
            if(s is TryStatementSyntax t){Embedded(b,t.Block);foreach(var c in t.Catches){var catchBlock=c.Declaration?.Identifier.RawKind>0?New(b,c,"catch-variable"):b;if(c.Declaration?.Identifier.RawKind>0)Add(catchBlock,c.Declaration.Identifier.ValueText,Type(c.Declaration.Type),c.Declaration,"declared",c.Declaration.Type.ToString());Embedded(catchBlock,c.Block);if(c.Filter!=null)Errors.Add("exception_filter");}if(t.Finally!=null)Embedded(b,t.Finally.Block);return;}
            if(s is CheckedStatementSyntax ck){Embedded(b,ck.Block);return;}
            if(s is UnsafeStatementSyntax un){Embedded(b,un.Block);return;}
            if(s is LabeledStatementSyntax label){ResolveStatement(b,label.Statement);return;}
            if(s is YieldStatementSyntax){Errors.Add("iterator_state_machine");return;}
            if(s is FixedStatementSyntax){Errors.Add("fixed_pinning");return;}
            Expressions(s);
        }
        void Meta(Block b){foreach(var v in b.variables.Concat(b.registered)){if(CaptureScopes.Any(c=>c.locals.Contains(v.start)))continue;v.slot=Slots.Count;Slots.Add(v);}foreach(var child in b.children)Meta(child);}
        int Alloc(string type,string name,SyntaxNode n){if(Free.TryGetValue(type,out var stack)&&stack.Count>0)return stack.Pop();var v=new Local{name=name,type=type,kind="emit-temporary",start=n.SpanStart,end=n.Span.End,slot=Slots.Count,block=-1};Slots.Add(v);return v.slot;}
        void Release(string type,int slot){if(!Free.TryGetValue(type,out var stack))Free[type]=stack=new Stack<int>();stack.Push(slot);}
        bool InstanceStorage(ExpressionSyntax e){var symbol=Sem.GetSymbolInfo(e).Symbol;return symbol is IFieldSymbol f?!f.IsStatic:symbol is IPropertySymbol p?!p.IsStatic:e is ElementAccessExpressionSyntax;}
        void StringLeaves(ExpressionSyntax e,List<ExpressionSyntax> leaves) {
            if(e is ParenthesizedExpressionSyntax p){StringLeaves(p.Expression,leaves);return;}
            if(e is BinaryExpressionSyntax b&&b.IsKind(SyntaxKind.AddExpression)&&Type(b)=="System.String"){StringLeaves(b.Left,leaves);StringLeaves(b.Right,leaves);return;}
            leaves.Add(e);
        }
        bool StringParams(ExpressionSyntax e) {
            var leaves=new List<ExpressionSyntax>();StringLeaves(e,leaves);
            int count=1;bool previousConstant=false,allStrings=true;
            foreach(var leaf in leaves){var c=Sem.GetConstantValue(leaf);bool isConstant=c.HasValue&&c.Value is string;
                if(!(previousConstant&&isConstant))count++;previousConstant=isConstant;
                if(Type(leaf)!="System.String"&&!(c.HasValue&&c.Value==null))allStrings=false;
            }
            return count>4||(count>3&&!allStrings);
        }
        void EmitExpression(ExpressionSyntax e,bool used=true){
            if(e==null||Nested(e))return;

            if(e is ObjectCreationExpressionSyntax initialized && initialized.Initializer!=null) {
                if(initialized.ArgumentList!=null)foreach(var arg in initialized.ArgumentList.Arguments)EmitExpression(arg.Expression,true);
                string type=Type(initialized);int slot=Alloc(type,"$object-initializer",initialized);
                foreach(var item in initialized.Initializer.Expressions)if(item is AssignmentExpressionSyntax initializedMember)EmitExpression(initializedMember.Right,true);
                Release(type,slot);return;
            }
            if(e is ObjectCreationExpressionSyntax creation && creation.ArgumentList?.Arguments.Count==0 && Sem.GetTypeInfo(creation).Type?.IsValueType==true){
                bool direct=creation.Parent is EqualsValueClauseSyntax initializer&&initializer.Parent is VariableDeclaratorSyntax;
                if(!direct&&used){string created=Type(creation);int slot=Alloc(created,"$default-value",creation);Release(created,slot);}return;
            }
            if(e is AssignmentExpressionSyntax assign){
                bool stringParams=assign.IsKind(SyntaxKind.AddAssignmentExpression)&&Type(assign.Left)=="System.String"&&Sem.GetSymbolInfo(assign.Left).Symbol is ILocalSymbol&&StringParams(assign.Right);
                // String.Concat resolves a5+term concat as params array. Its
                // CompoundAssign.TargetExpression is evaluated/spilled before
                // Newarr (expression.cs5965-5969,6376-6378), released after.
                int stringSlot=stringParams?Alloc("System.String","$concat-left-before-params-array",assign.Left):-1;
                EmitExpression(assign.Left,false);EmitExpression(assign.Right,true);
                if(stringSlot>=0)Release("System.String",stringSlot);
                if(used&&InstanceStorage(assign.Left)){string type=Type(assign.Left);int slot=Alloc(type,"$assignment-value",e);Release(type,slot);}return;}
            if(e is PostfixUnaryExpressionSyntax post&&(post.IsKind(SyntaxKind.PostIncrementExpression)||post.IsKind(SyntaxKind.PostDecrementExpression))){EmitExpression(post.Operand,false);if(used&&InstanceStorage(post.Operand)){string type=Type(post.Operand);int slot=Alloc(type,"$increment-value",e);Release(type,slot);}return;}
            if(e is PrefixUnaryExpressionSyntax pre&&(pre.IsKind(SyntaxKind.PreIncrementExpression)||pre.IsKind(SyntaxKind.PreDecrementExpression))){EmitExpression(pre.Operand,false);if(used&&InstanceStorage(pre.Operand)){string type=Type(pre.Operand);int slot=Alloc(type,"$increment-value",e);Release(type,slot);}return;}
            if(e is InvocationExpressionSyntax call&&call.Expression is MemberAccessExpressionSyntax member){
                EmitExpression(member.Expression,true);int temporary=-1;string type=Type(member.Expression);var receiverType=Sem.GetTypeInfo(member.Expression).Type;var sym=Sem.GetSymbolInfo(member.Expression).Symbol;
                bool addressable=sym is ILocalSymbol || sym is IParameterSymbol || sym is IFieldSymbol field&&!field.IsReadOnly || member.Expression is ThisExpressionSyntax || member.Expression is ElementAccessExpressionSyntax;
                var invoked=Sem.GetSymbolInfo(call).Symbol as IMethodSymbol;
                if(invoked?.IsStatic!=true&&receiverType?.IsValueType==true&&!addressable)temporary=Alloc(type,"$rvalue-receiver",member.Expression);
                foreach(var arg in call.ArgumentList.Arguments)EmitExpression(arg.Expression,true);if(temporary>=0)Release(type,temporary);return;
            }
            foreach(var child in e.ChildNodes())if(child is ExpressionSyntax ex)EmitExpression(ex,true);else if(child is ArgumentListSyntax args)foreach(var arg in args.Arguments)EmitExpression(arg.Expression,true);
        }
        void EmitStatement(StatementSyntax s,int protectedDepth=0){
            if(s is BlockSyntax block)foreach(var capture in CaptureScopes.Where(c=>c.start==block.SpanStart))Alloc(capture.type,"$storey",block);
            if(s is SwitchStatementSyntax sw){EmitExpression(sw.Expression);var st=Sem.GetTypeInfo(sw.Expression).Type;string type=st is INamedTypeSymbol named&&named.TypeKind==TypeKind.Enum?Key(named.EnumUnderlyingType):Key(st);bool constant=Sem.GetConstantValue(sw.Expression).HasValue;int slot=constant?-1:Alloc(type,"$switch",sw.Expression);int stringValue=-1;
                if(type=="System.String"&&!constant){string dictionary="System.Collections.Generic.Dictionary`2<System.String,System.Int32>";int dictionarySlot=Alloc(dictionary,"$string-switch-map",sw.Expression);Release(dictionary,dictionarySlot);stringValue=Alloc("System.Int32","$string-switch-index",sw.Expression);}
                foreach(var section in sw.Sections)foreach(var childStatement in section.Statements)EmitStatement(childStatement,protectedDepth);if(stringValue>=0)Release("System.Int32",stringValue);if(slot>=0)Release(type,slot);return;}
            if(s is ReturnStatementSyntax r){EmitExpression(r.Expression);if(r.Expression!=null&&protectedDepth>0&&ReturnSlot<0){ReturnSlot=Slots.Count;Slots.Add(new Local{name="$return",type=Key(Symbol.ReturnType),kind="protected-return",start=r.SpanStart,end=r.Span.End,slot=ReturnSlot,block=-1});}return;}
            if(s is TryStatementSyntax tr){EmitStatement(tr.Block,protectedDepth+1);foreach(var c in tr.Catches)EmitStatement(c.Block,protectedDepth+1);if(tr.Finally!=null)EmitStatement(tr.Finally.Block,protectedDepth+1);return;}
            if(s is LocalDeclarationStatementSyntax declaration){foreach(var v in declaration.Declaration.Variables)if(v.Initializer!=null)EmitExpression(v.Initializer.Value);return;}
            if(s is ExpressionStatementSyntax expr){EmitExpression(expr.Expression,false);return;}
            if(s is IfStatementSyntax iff){EmitExpression(iff.Condition);EmitStatement(iff.Statement,protectedDepth);if(iff.Else!=null)EmitStatement(iff.Else.Statement,protectedDepth);return;}
            if(s is ForStatementSyntax forLoop){if(forLoop.Declaration!=null)foreach(var v in forLoop.Declaration.Variables)if(v.Initializer!=null)EmitExpression(v.Initializer.Value);foreach(var init in forLoop.Initializers)EmitExpression(init,false);EmitExpression(forLoop.Condition);EmitStatement(forLoop.Statement,protectedDepth);foreach(var iter in forLoop.Incrementors)EmitExpression(iter,false);return;}
            if(s is WhileStatementSyntax wh){EmitExpression(wh.Condition);EmitStatement(wh.Statement,protectedDepth);return;}
            if(s is DoStatementSyntax dow){EmitStatement(dow.Statement,protectedDepth);EmitExpression(dow.Condition);return;}
            if(s is UsingStatementSyntax us){if(us.Expression!=null)EmitExpression(us.Expression);if(us.Declaration!=null)foreach(var v in us.Declaration.Variables)if(v.Initializer!=null)EmitExpression(v.Initializer.Value);EmitStatement(us.Statement,protectedDepth+1);return;}
            if(s is LockStatementSyntax lo){EmitExpression(lo.Expression);EmitStatement(lo.Statement,protectedDepth+1);return;}
            if(s is ForEachStatementSyntax fe){EmitExpression(fe.Expression);bool array=Sem.GetTypeInfo(fe.Expression).Type is IArrayTypeSymbol;EmitStatement(fe.Statement,protectedDepth+(array?0:1));return;}
            foreach(var child in s.ChildNodes())if(child is StatementSyntax st)EmitStatement(st,protectedDepth);else if(child is ElseClauseSyntax el)EmitStatement(el.Statement,protectedDepth);
        }
        public object Run(){
            if(Body==null)return new{method_id=Method(Symbol),complete=true,predicted_types=new string[0],variables=new Local[0],blocks=new Block[0],unsupported_reasons=new string[0],declaration_start=Declaration.SpanStart,declaration_end=Declaration.Span.End};
            var root=New(null,Body,"method");Sequence(root,Body.Statements);Meta(root);EmitStatement(Body);
            // CS0012 is expected when Roslyn reads the mixed 2.0/2.0.5 reference
            // identities accepted by this smcs profile. All slot types still
            // require resolved symbols; other semantic failures reject a model.
            foreach(var diagnostic in Sem.GetDiagnostics(Declaration.Span).Where(x=>x.Id=="CS0030"))DiagnosticEvidence.Add(diagnostic.ToString()+" source="+Declaration.SyntaxTree.GetText().ToString(diagnostic.Location.SourceSpan));
            var diagnostics=Sem.GetDiagnostics(Declaration.Span).Where(d=>d.Severity==DiagnosticSeverity.Error&&!Relax(d)).Select(d=>d.Id).Distinct().ToArray();
            if(diagnostics.Length>0)Errors.Add("semantic_errors:"+string.Join(",",diagnostics));
            return new{method_id=Method(Symbol),complete=Errors.Count==0,predicted_types=Slots.Select(v=>v.type).ToArray(),variables=Slots,blocks=Blocks,unsupported_reasons=Errors.Distinct().ToArray(),relaxed_diagnostics=RelaxedDiagnostics.Distinct().ToArray(),diagnostic_evidence=DiagnosticEvidence.ToArray(),declaration_start=Declaration.SpanStart,declaration_end=Declaration.Span.End,body_start=Body.SpanStart,body_end=Body.Span.End};
        }
        bool Relax(Diagnostic d) {
            bool allowed=d.Id=="CS0012";
            if(d.Id=="CS0115"&&Symbol.IsOverride&&Symbol.Parameters.Length==0&&((Symbol.Name=="ToString"&&Key(Symbol.ReturnType)=="System.String")||(Symbol.Name=="GetHashCode"&&Key(Symbol.ReturnType)=="System.Int32")))allowed=true;
            if(d.Id=="CS0155") {
                var caught=Declaration.FindNode(d.Location.SourceSpan).AncestorsAndSelf().OfType<CatchDeclarationSyntax>().FirstOrDefault();
                var type=caught==null?null:Sem.GetTypeInfo(caught.Type).Type as INamedTypeSymbol;
                for(var t=type;t!=null;t=t.BaseType)if(TypeName(t)=="System.Exception")allowed=true;
            }
            if(d.Id=="CS1660") {
                var node=Declaration.FindNode(d.Location.SourceSpan,getInnermostNodeForTie:true);var lambda=node.AncestorsAndSelf().OfType<AnonymousFunctionExpressionSyntax>().FirstOrDefault()??node.DescendantNodesAndSelf().OfType<AnonymousFunctionExpressionSyntax>().FirstOrDefault();
                DiagnosticEvidence.Add(d.GetMessage()+" node="+node.Kind()+" lambda="+(lambda?.Kind().ToString()??"null"));
                if(lambda!=null) {
                    // A resolved historical delegate has Invoke even when its
                    // System.MulticastDelegate belongs to the other corlib identity.
                    var argument=lambda.Parent as ArgumentSyntax;var call=argument?.Parent?.Parent;
                    if(call is InvocationExpressionSyntax || call is ObjectCreationExpressionSyntax) {
                        var info=Sem.GetSymbolInfo(call);int index=((ArgumentListSyntax)argument.Parent).Arguments.IndexOf(argument);
                        var constructors=call is ObjectCreationExpressionSyntax created?(Sem.GetTypeInfo(created).Type as INamedTypeSymbol)?.InstanceConstructors.AsEnumerable():Enumerable.Empty<IMethodSymbol>();
                        foreach(var candidate in info.CandidateSymbols.OfType<IMethodSymbol>().Concat(new[]{info.Symbol as IMethodSymbol}).Concat(constructors??Enumerable.Empty<IMethodSymbol>()).Where(m=>m!=null)) {
                            var n=index<candidate.Parameters.Length?candidate.Parameters[index].Type as INamedTypeSymbol:null;
                            DiagnosticEvidence.Add("parameter="+Key(n)+" kind="+n?.Kind+" base="+Key(n?.BaseType));
                            if(n is IErrorTypeSymbol)n=Sem.Compilation.GetTypeByMetadataName(TypeName(n));
                            if(n!=null&&n.GetMembers("Invoke").OfType<IMethodSymbol>().Any())for(var t=n.BaseType;t!=null;t=t.BaseType)if(TypeName(t)=="System.MulticastDelegate")allowed=true;
                        }
                    }
                }
            }
            if(allowed)RelaxedDiagnostics.Add(d.Id);return allowed;
        }
    }

    static List<(SyntaxTree tree,AnonymousFunctionExpressionSyntax lambda)> OrderedLambdas(string path,SyntaxTree replacement,CSharpCompilation compilation) {
        var types=new Dictionary<string,(SyntaxTree tree,TypeDeclarationSyntax node,INamedTypeSymbol symbol)>();
        foreach(var pair in Trees) {
            var tree=string.Equals(pair.Key,path,StringComparison.OrdinalIgnoreCase)?replacement:pair.Value;var sem=compilation.GetSemanticModel(tree);
            foreach(var type in tree.GetRoot().DescendantNodes().OfType<TypeDeclarationSyntax>()) {var symbol=sem.GetDeclaredSymbol(type) as INamedTypeSymbol;if(symbol!=null)types[TypeName(symbol)]=(tree,type,symbol);}
        }
        var result=new List<(SyntaxTree tree,AnonymousFunctionExpressionSyntax lambda)>();var visited=new HashSet<string>();
        Action<string> visit=null;visit=name=> {
            if(!visited.Add(name)||!types.TryGetValue(name,out var type))return;
            if(type.symbol.BaseType!=null)visit(TypeName(type.symbol.BaseType));
            foreach(var lambda in type.node.DescendantNodes().OfType<AnonymousFunctionExpressionSyntax>().Where(l=>l.Ancestors().OfType<TypeDeclarationSyntax>().FirstOrDefault()==type.node))result.Add((type.tree,lambda));
        };
        foreach(var name in types.Keys)visit(name);return result;
    }
    static IMethodSymbol AnonymousSymbol(AnonymousFunctionExpressionSyntax lambda,SemanticModel sem) {
        var converted=sem.GetTypeInfo(lambda).ConvertedType as INamedTypeSymbol;
        if(converted is IErrorTypeSymbol)converted=sem.Compilation.GetTypeByMetadataName(TypeName(converted));
        var convertedInvoke=converted?.GetMembers("Invoke").OfType<IMethodSymbol>().FirstOrDefault();if(convertedInvoke!=null)return convertedInvoke;
        var symbol=sem.GetSymbolInfo(lambda).Symbol as IMethodSymbol;
        var argument=lambda.Parent as ArgumentSyntax;var call=argument?.Parent?.Parent;
        if(!(call is InvocationExpressionSyntax)&&!(call is ObjectCreationExpressionSyntax))return symbol;
        int index=((ArgumentListSyntax)argument.Parent).Arguments.IndexOf(argument);var info=sem.GetSymbolInfo(call);
        var constructors=call is ObjectCreationExpressionSyntax created?(sem.GetTypeInfo(created).Type as INamedTypeSymbol)?.InstanceConstructors.AsEnumerable():Enumerable.Empty<IMethodSymbol>();
        var invokes=new List<IMethodSymbol>();
        foreach(var candidate in info.CandidateSymbols.OfType<IMethodSymbol>().Concat(new[]{info.Symbol as IMethodSymbol}).Concat(constructors??Enumerable.Empty<IMethodSymbol>()).Where(m=>m!=null)) {
            var type=index<candidate.Parameters.Length?candidate.Parameters[index].Type as INamedTypeSymbol:null;
            if(type is IErrorTypeSymbol)type=sem.Compilation.GetTypeByMetadataName(TypeName(type));
            if(type!=null)for(var parent=type.BaseType;parent!=null;parent=parent.BaseType)if(TypeName(parent)=="System.MulticastDelegate")invokes.AddRange(type.GetMembers("Invoke").OfType<IMethodSymbol>());
        }
        bool valueReturned=lambda.Body.DescendantNodesAndSelf().OfType<ReturnStatementSyntax>().Any(r=>r.Expression!=null);
        var distinct=invokes.Where(m=>m.ReturnsVoid!=valueReturned).GroupBy(m=>Method(m)).Select(g=>g.First()).ToArray();return distinct.Length==1?distinct[0]:null;
    }
    static List<object> GeneratedModels(string path,SyntaxTree tree,CSharpCompilation compilation,string wanted,List<CaptureScope> captures) {
        int ordinal=0;var output=new List<object>();
        foreach(var entry in OrderedLambdas(path,tree,compilation)) {
            var active=entry.tree;
            var sem=compilation.GetSemanticModel(active);
            {
                var lambda=entry.lambda;int number=ordinal++;if(!string.Equals(active.FilePath,path,StringComparison.OrdinalIgnoreCase))continue;
                var host=lambda.Ancestors().FirstOrDefault(n=>n is BaseMethodDeclarationSyntax||n is AccessorDeclarationSyntax);
                var hostSymbol=host==null?null:sem.GetDeclaredSymbol(host) as IMethodSymbol;if(hostSymbol==null)continue;
                // This experiment supports only methods hosted directly in the type,
                // never storey-hosted, nested, iterator or expression-bodied lambdas.
                if(captures.Any(c=>c.method==Method(hostSymbol))||lambda.Ancestors().OfType<AnonymousFunctionExpressionSyntax>().Any()||lambda.Body is not BlockSyntax body)continue;
                if(body.DescendantNodes().OfType<YieldStatementSyntax>().Any()||body.DescendantNodes().OfType<AnonymousFunctionExpressionSyntax>().Any())continue;
                var symbol=AnonymousSymbol(lambda,sem);if(symbol==null)continue;
                string id=TypeName(hostSymbol.ContainingType)+"::<"+hostSymbol.MetadataName+">m__"+number.ToString("X")+"("+string.Join(",",symbol.Parameters.Select(p=>Key(p.Type)+(p.RefKind==RefKind.None?"":"&")))+")->"+Key(symbol.ReturnType);
                if(wanted!=null&&wanted!=id)continue;
                var result=JsonSerializer.Deserialize<Dictionary<string,JsonElement>>(JsonSerializer.Serialize(new Model(sem,lambda,body,symbol,new List<CaptureScope>()).Run()));
                result["method_id"]=JsonSerializer.SerializeToElement(id);
                result["generated_source_method"]=JsonSerializer.SerializeToElement(Method(hostSymbol));
                result["generated_mapping"]=JsonSerializer.SerializeToElement("source-declaration-global-anonymous-ordinal");
                output.Add(result);
            }
        }
        return output;
    }

    static object Respond(JsonElement input){
        string path=Path.GetFullPath(input.GetProperty("path").GetString());var original=Trees[path];var tree=original;var compilation=Compilation;
        if(input.TryGetProperty("source",out var src)){tree=CSharpSyntaxTree.ParseText(src.GetString(),path:path);compilation=Compilation.ReplaceSyntaxTree(original,tree);}
        var sem=compilation.GetSemanticModel(tree);var captures=NumberCaptures(path,Captures(tree,sem));var root=tree.GetRoot();var output=new List<object>();string wanted=input.TryGetProperty("method_id",out var mi)?mi.GetString():null;
        foreach(var node in root.DescendantNodes().Where(n=>n is BaseMethodDeclarationSyntax || n is AccessorDeclarationSyntax)){
            var symbol=sem.GetDeclaredSymbol(node) as IMethodSymbol;if(symbol==null)continue;string id=Method(symbol);if(wanted!=null&&id!=wanted)continue;
            BlockSyntax body=node is BaseMethodDeclarationSyntax m?m.Body:((AccessorDeclarationSyntax)node).Body;
            var model=new Model(sem,node,body,symbol,captures);output.Add(model.Run());
        }
        if(wanted==null||wanted.Contains(">m__"))output.AddRange(GeneratedModels(path,tree,compilation,wanted,captures));
        return new{methods=output,source_only=true,candidate_compiled=false};
    }
    public static void Main(string[] args){
        var manifest=JsonDocument.Parse(File.ReadAllText(args[0])).RootElement;
        foreach(var item in manifest.GetProperty("sources").EnumerateArray()){string logical=item.ValueKind==JsonValueKind.String?item.GetString():item.GetProperty("path").GetString();string file=item.ValueKind==JsonValueKind.Object&&item.TryGetProperty("input_path",out var ip)?ip.GetString():logical;string path=Path.GetFullPath(logical);Trees[path]=CSharpSyntaxTree.ParseText(File.ReadAllText(file),path:path);}
        var refs=manifest.GetProperty("references").EnumerateArray().Select(item=>MetadataReference.CreateFromFile(item.ValueKind==JsonValueKind.String?item.GetString():item.GetProperty("path").GetString()));
        Compilation=CSharpCompilation.Create("Round7SourceOnly",Trees.Values,refs,new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        foreach(var pair in Trees)BaseCaptures[pair.Key]=Captures(pair.Value,Compilation.GetSemanticModel(pair.Value));
        string line;while((line=Console.ReadLine())!=null){try{using(var d=JsonDocument.Parse(line))Console.WriteLine(JsonSerializer.Serialize(Respond(d.RootElement)));}catch(Exception e){Console.WriteLine(JsonSerializer.Serialize(new{error=e.GetType().Name,message=e.Message}));}}
    }
}
