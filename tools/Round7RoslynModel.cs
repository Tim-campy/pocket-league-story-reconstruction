// Source-only model of the Roslyn 1.1 local allocator. No Emit is called.
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static class Round7RoslynModel {
    static Dictionary<string,SyntaxTree> Trees=new Dictionary<string,SyntaxTree>(StringComparer.OrdinalIgnoreCase);
    static CSharpCompilation Compilation;
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
    static string LocalKey(ITypeSymbol t){if(t is ITypeParameterSymbol p)return p.Name;if(t is IArrayTypeSymbol a)return LocalKey(a.ElementType)+(a.Rank==1?"[]":"["+string.Join(",",Enumerable.Repeat("0...",a.Rank))+"]");if(t is INamedTypeSymbol n){var args=new List<ITypeSymbol>();var parents=new Stack<INamedTypeSymbol>();for(var c=n;c!=null;c=c.ContainingType)parents.Push(c);foreach(var c in parents)args.AddRange(c.TypeArguments);return TypeName(n.OriginalDefinition)+(args.Count>0?"<"+string.Join(",",args.Select(LocalKey))+">":"");}return Key(t);}
    static string Method(IMethodSymbol m) => m==null?null:TypeName(m.ContainingType)+"::"+m.MetadataName+(m.Arity==0?"":"``"+m.Arity)+"("+string.Join(",",m.Parameters.Select(p=>Key(p.Type)+(p.RefKind==RefKind.None?"":"&")))+")->"+Key(m.ReturnType);
    sealed class Local {
        public string name {get;set;} public string type {get;set;} public string kind {get;set;}
        public int start {get;set;} public int end {get;set;} public int slot {get;set;}=-1;
        public int block {get;set;} public string type_source {get;set;}
        public int statement_start {get;set;}=-1; public int statement_end {get;set;}=-1;
        public int lexical_scope_start {get;set;}=-1; public int lexical_scope_end {get;set;}=-1;
        public string initializer {get;set;} public int next_statement_start {get;set;}=-1;public int next_statement_end {get;set;}=-1;public string next_statement_kind {get;set;}
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
        public Dictionary<string,Stack<int>> Free=new Dictionary<string,Stack<int>>(); public int ReturnSlot=-1; public HashSet<string> StackLocals=new HashSet<string>();
        public Model(SemanticModel sem,SyntaxNode dec,BlockSyntax body,IMethodSymbol symbol){Sem=sem;Declaration=dec;Body=body;Symbol=symbol;}
        public Block New(Block parent,SyntaxNode node,string kind){var b=new Block{id=Blocks.Count,parent=parent==null?-1:parent.id,start=node.SpanStart,end=node.Span.End,kind=kind};Blocks.Add(b);if(parent!=null)parent.children.Add(b);return b;}
        public string Type(SyntaxNode n){return LocalKey(Sem.GetTypeInfo(n).Type);}
        void Add(Block b,string name,string type,SyntaxNode node,string kind,string syntaxType=null){var v=new Local{name=name,type=type,start=node.SpanStart,end=node.Span.End,block=b.id,kind=kind,type_source=syntaxType};
            if(node is VariableDeclaratorSyntax variable && variable.Parent?.Parent is LocalDeclarationStatementSyntax statement && statement.Parent is BlockSyntax scope){v.statement_start=statement.SpanStart;v.statement_end=statement.Span.End;v.lexical_scope_start=scope.SpanStart;v.lexical_scope_end=scope.Span.End;v.initializer=variable.Initializer?.Value.ToString();int index=scope.Statements.IndexOf(statement);if(index+1<scope.Statements.Count){var next=scope.Statements[index+1];v.next_statement_start=next.SpanStart;v.next_statement_end=next.Span.End;v.next_statement_kind=next.Kind().ToString();}}
            if(type=="UNKNOWN"||type.Contains("?"))Errors.Add("unresolved_type:"+name);(kind=="declared"?b.variables:b.registered).Add(v);}
        void DeclarationVars(Block b,VariableDeclarationSyntax d,bool isConst=false){foreach(var v in d.Variables){var s=Sem.GetDeclaredSymbol(v) as ILocalSymbol;if(!isConst)Add(b,v.Identifier.ValueText,LocalKey(s?.Type),v,"declared",d.Type.ToString());}}
        bool Nested(SyntaxNode n)=>n is AnonymousFunctionExpressionSyntax || n is LocalFunctionStatementSyntax;
        IEnumerable<SyntaxNode> Own(SyntaxNode n)=>n.DescendantNodesAndSelf(x=>!Nested(x));
        void Expressions(SyntaxNode n) {
            // Expression lowerings which allocate additional locals need an explicit
            // rule; a candidate is never approved by guessing an omitted temporary.
            foreach(var x in Own(n)) {
                if(x is AnonymousFunctionExpressionSyntax)Errors.Add("anonymous_function_or_capture");
                if(x is DefaultExpressionSyntax)Errors.Add("default_expression_temporary");
                if(x is AssignmentExpressionSyntax ass && ass.Left is IdentifierNameSyntax ident && Sem.GetSymbolInfo(ident).Symbol is IParameterSymbol par && par.RefKind!=RefKind.None && ass.Ancestors().Any(a=>a is AssignmentExpressionSyntax))Errors.Add("ref_assignment_value_temporary");
                if(x is MemberAccessExpressionSyntax mem && !(Sem.GetSymbolInfo(mem.Name).Symbol?.IsStatic??false) && !(mem.Expression is ElementAccessExpressionSyntax) && Sem.GetTypeInfo(mem.Expression).Type?.IsValueType==true && !(Sem.GetSymbolInfo(mem.Expression).Symbol is ILocalSymbol) && !(Sem.GetSymbolInfo(mem.Expression).Symbol is IParameterSymbol) && !(Sem.GetSymbolInfo(mem.Expression).Symbol is IFieldSymbol) && !(mem.Expression is ThisExpressionSyntax))Errors.Add("rvalue_value_type_member_temporary");
                if(x is BinaryExpressionSyntax binary && binary.IsKind(SyntaxKind.AddExpression) && Type(binary)=="System.String" && (Sem.GetTypeInfo(binary.Left).Type?.IsValueType==true || Sem.GetTypeInfo(binary.Right).Type?.IsValueType==true))Errors.Add("string_concat_value_temporary");
                if(x is ElementAccessExpressionSyntax index && index.ArgumentList.Arguments.Any(arg=>arg.Expression is PostfixUnaryExpressionSyntax))Errors.Add("array_postfix_temporary");
                if(x is AwaitExpressionSyntax)Errors.Add("async_lowering");
                if(x is StackAllocArrayCreationExpressionSyntax)Errors.Add("stackalloc");
                if(x is ConditionalAccessExpressionSyntax)Errors.Add("conditional_access_temporary");
                if(x is ObjectCreationExpressionSyntax o && o.Initializer!=null)Errors.Add("object_or_collection_initializer");
            }
        }
        Block Sequence(Block current,IEnumerable<StatementSyntax> statements) {
            // EmitBlock defines *all* its locals before emitting statements.
            foreach(var s in statements) if(s is LocalDeclarationStatementSyntax local)
                DeclarationVars(current,local.Declaration,local.Modifiers.Any(SyntaxKind.ConstKeyword));
            foreach(var s in statements) {
                if(s is LocalDeclarationStatementSyntax local) {
                    foreach(var v in local.Declaration.Variables)if(v.Initializer!=null)Expressions(v.Initializer);
                } else ResolveStatement(current,s);
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
                var child=New(b,each,"foreach");var sym=Sem.GetDeclaredSymbol(each) as ILocalSymbol;Add(child,each.Identifier.ValueText,LocalKey(sym?.Type),each,"declared",each.Type.ToString());
                var type=Sem.GetTypeInfo(each.Expression).Type;Expressions(each.Expression);
                if(type is IArrayTypeSymbol array){if(array.Rank!=1)Errors.Add("multidimensional_array_foreach");Add(child,"$array",LocalKey(type),each.Expression,"foreach-array-copy");for(int rank=0;rank<array.Rank;rank++){Add(child,"$index"+rank,"System.Int32",each,"foreach-index");if(array.Rank>1)Add(child,"$length"+rank,"System.Int32",each,"foreach-length");}}
                else if(type?.SpecialType==SpecialType.System_String){Errors.Add("foreach_string_lowering");}
                else {var info=Sem.GetForEachStatementInfo(each);Add(child,"$enumerator",LocalKey(info.GetEnumeratorMethod?.ReturnType),each.Expression,"foreach-enumerator");if(info.GetEnumeratorMethod?.ReturnType?.IsValueType!=true && !(info.GetEnumeratorMethod?.ReturnType as INamedTypeSymbol)?.AllInterfaces.Any(t=>t.SpecialType==SpecialType.System_IDisposable)==true)Errors.Add("foreach_runtime_dispose");}
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
        void AllocateBlock(Block b){foreach(var v in b.variables.Concat(b.registered)){if(StackLocals.Contains(v.name+":"+v.start)){v.slot=-1;continue;}v.slot=Slots.Count;Slots.Add(v);}}
        void Meta(Block b){AllocateBlock(b);foreach(var child in b.children)Meta(child);}
        int Alloc(string type,string name,SyntaxNode n){if(Free.TryGetValue(type,out var stack)&&stack.Count>0)return stack.Pop();var v=new Local{name=name,type=type,kind="emit-temporary",start=n.SpanStart,end=n.Span.End,slot=Slots.Count,block=-1};Slots.Add(v);return v.slot;}
        void Release(string type,int slot){if(!Free.TryGetValue(type,out var stack))Free[type]=stack=new Stack<int>();stack.Push(slot);}
        bool InstanceStorage(ExpressionSyntax e){var symbol=Sem.GetSymbolInfo(e).Symbol;return symbol is IFieldSymbol f?!f.IsStatic:symbol is IPropertySymbol p?!p.IsStatic:e is ElementAccessExpressionSyntax;}
        void EmitExpression(ExpressionSyntax e,bool used=true){
            if(e==null||Nested(e))return;
            if(e is ObjectCreationExpressionSyntax creation && creation.ArgumentList?.Arguments.Count==0 && Sem.GetTypeInfo(creation).Type?.IsValueType==true){
                bool direct=creation.Parent is EqualsValueClauseSyntax initializer&&initializer.Parent is VariableDeclaratorSyntax;
                if(!direct&&used){string created=Type(creation);int slot=Alloc(created,"$default-value",creation);Release(created,slot);}return;
            }
            if(e is AssignmentExpressionSyntax assign){EmitExpression(assign.Left,false);EmitExpression(assign.Right,true);if(used&&InstanceStorage(assign.Left)){string type=Type(assign.Left);int slot=Alloc(type,"$assignment-value",e);Release(type,slot);}return;}
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
        bool Same(ILocalSymbol symbol,SyntaxNode n)=>SymbolEqualityComparer.Default.Equals(symbol,Sem.GetSymbolInfo(n).Symbol);
        int ReadDepth(ExpressionSyntax expression){
            int depth=0;SyntaxNode child=expression;
            for(var p=child.Parent;p!=null && !(p is StatementSyntax);child=p,p=p.Parent){
                if(p is InitializerExpressionSyntax initializer && initializer.IsKind(SyntaxKind.ArrayInitializerExpression))depth+=2;
                if(p is BinaryExpressionSyntax bin && child==bin.Right && !bin.IsKind(SyntaxKind.LogicalAndExpression)&&!bin.IsKind(SyntaxKind.LogicalOrExpression))depth++;
                if(p is ArgumentSyntax arg && arg.Parent is BaseArgumentListSyntax list){int position=list.Arguments.IndexOf(arg);depth+=position;if(list.Parent is InvocationExpressionSyntax call){var m=Sem.GetSymbolInfo(call).Symbol as IMethodSymbol;if(m?.IsStatic==false)depth++;}else if(list.Parent is ElementAccessExpressionSyntax)depth++;}
                if(p is AssignmentExpressionSyntax ass && child==ass.Right){if(!ass.IsKind(SyntaxKind.SimpleAssignmentExpression))depth++;var symbol=Sem.GetSymbolInfo(ass.Left).Symbol;if(ass.Left is ElementAccessExpressionSyntax)depth+=2;else if(symbol is IFieldSymbol field&&!field.IsStatic || symbol is IPropertySymbol prop&&!prop.IsStatic || symbol is IParameterSymbol param&&param.RefKind!=RefKind.None)depth++;}
            }
            return depth;
        }
        void OptimizeLocals(){
            foreach(var block in Blocks)foreach(var v in block.variables){
                var node=Body.DescendantNodes().OfType<VariableDeclaratorSyntax>().FirstOrDefault(n=>n.SpanStart==v.start);
                if(node==null)continue;
                var symbol=Sem.GetDeclaredSymbol(node) as ILocalSymbol;
                var uses=Body.DescendantNodes().OfType<IdentifierNameSyntax>().Where(n=>Same(symbol,n)).ToArray();
                // Literal stores with no reads disappear; arbitrary side effects are kept but their value is popped.
                if(uses.Length==0){StackLocals.Add(v.name+":"+v.start);continue;}
                bool address=uses.Any(n=>n.Parent is ArgumentSyntax arg&&arg.RefOrOutKeyword.RawKind!=0 || n.Parent is MemberAccessExpressionSyntax m&&m.Expression==n&&symbol.Type.IsValueType);
                bool assigned=uses.Any(n=>n.Parent is AssignmentExpressionSyntax ass&&ass.Left==n || n.Parent is PrefixUnaryExpressionSyntax pre&&(pre.IsKind(SyntaxKind.PreIncrementExpression)||pre.IsKind(SyntaxKind.PreDecrementExpression)) || n.Parent is PostfixUnaryExpressionSyntax post&&(post.IsKind(SyntaxKind.PostIncrementExpression)||post.IsKind(SyntaxKind.PostDecrementExpression)));
                bool control=uses.Any(n=>n.Ancestors().TakeWhile(p=>p!=Body).Any(p=>(p is ForStatementSyntax || p is WhileStatementSyntax || p is DoStatementSyntax || p is ForEachStatementSyntax) && !p.Span.Contains(node.Span)));
                // A single direct return following a declaration is a guaranteed zero-depth stack schedule.
                if(uses.Length==1 && node.Parent?.Parent is LocalDeclarationStatementSyntax d && d.Parent is BlockSyntax scope && uses[0].Parent is ReturnStatementSyntax r && r.Expression==uses[0] && r.Parent==scope && scope.Statements.IndexOf(r)==scope.Statements.IndexOf(d)+1){StackLocals.Add(v.name+":"+v.start);continue;}
                // Addressed variables, mutated variables, loops and repeated uses have explicit survival constraints.
                if(address||assigned||control)continue;
                if(uses.Length>0&&ReadDepth(uses[0])>0)continue;
                if(uses.Length>1){Errors.Add("stack_scheduler_multiple_use:"+v.name);continue;}
                if(uses.Any(n=>ReadDepth(n)!=0))continue;
                // Receiver/first argument may be scheduled at stack depth zero; leave this uncertainty visible.
                Errors.Add((uses.Length==1?"stack_scheduler_single_use:":"stack_scheduler_multiple_use:")+v.name);
            }
        }
        Block Find(SyntaxNode node,string kind=null)=>Blocks.FirstOrDefault(b=>b.start==node.SpanStart&&(kind==null||b.kind==kind));
        void EmitBlock(Block block,BlockSyntax syntax){if(block==null){Errors.Add("unmapped_block");return;}AllocateBlock(block);foreach(var s in syntax.Statements)EmitRoslyn(s);}
        void EmitRoslyn(StatementSyntax s){
            if(s==null)return;
            if(s is BlockSyntax plain){EmitBlock(Find(plain),plain);return;}
            if(s is ForStatementSyntax f){AllocateBlock(Find(f,"for"));if(f.Declaration!=null)foreach(var v in f.Declaration.Variables)if(v.Initializer!=null)EmitExpression(v.Initializer.Value);foreach(var e in f.Initializers)EmitExpression(e);EmitExpression(f.Condition);EmitRoslyn(f.Statement);foreach(var e in f.Incrementors)EmitExpression(e);return;}
            if(s is IfStatementSyntax i){EmitExpression(i.Condition);EmitRoslyn(i.Statement);EmitRoslyn(i.Else?.Statement);return;}
            if(s is WhileStatementSyntax w){EmitExpression(w.Condition);EmitRoslyn(w.Statement);return;}
            if(s is DoStatementSyntax d){EmitRoslyn(d.Statement);EmitExpression(d.Condition);return;}
            if(s is ForEachStatementSyntax each){var block=Find(each,"foreach");if(Sem.GetSymbolInfo(each.Expression).Symbol is ILocalSymbol)Errors.Add("foreach_collection_local_stack_scheduler");if(!(Sem.GetTypeInfo(each.Expression).Type is IArrayTypeSymbol)&&each.Statement.DescendantNodesAndSelf().OfType<ReturnStatementSyntax>().Any(r=>r.Expression!=null))Errors.Add("foreach_protected_return");
                foreach(var v in block.registered){v.slot=Alloc(v.type,v.name,each);}
                var sym=Sem.GetDeclaredSymbol(each) as ILocalSymbol;
                var refs=each.Statement.DescendantNodes().OfType<IdentifierNameSyntax>().Where(n=>Same(sym,n)).ToArray();
                foreach(var v in block.variables){bool address=refs.Any(n=>n.Parent is ArgumentSyntax a&&a.RefOrOutKeyword.RawKind!=0 || n.Parent is MemberAccessExpressionSyntax m&&m.Expression==n&&sym.Type.IsValueType);if(refs.Length==1&&!address&&ReadDepth(refs[0])==0){v.slot=-1;}else{if(!(address&&sym.Type.IsValueType&&Sem.GetTypeInfo(each.Expression).Type is IArrayTypeSymbol))Errors.Add("foreach_iteration_stack_scheduler");v.slot=Slots.Count;Slots.Add(v);}}
                EmitRoslyn(each.Statement);foreach(var v in block.registered)Release(v.type,v.slot);return;}
            if(s is TryStatementSyntax t){Errors.Add("exception_lowering");EmitRoslyn(t.Block);foreach(var c in t.Catches){if(c.Declaration?.Identifier.RawKind>0)AllocateBlock(Find(c,"catch-variable"));EmitRoslyn(c.Block);}EmitRoslyn(t.Finally?.Block);return;}
            if(s is UsingStatementSyntax u){Errors.Add("using_lowering");if(u.Declaration!=null)AllocateBlock(Find(u,"using-declaration"));EmitRoslyn(u.Statement);return;}
            if(s is LockStatementSyntax l){Errors.Add("lock_lowering");EmitRoslyn(l.Statement);return;}
            if(s is SwitchStatementSyntax sw){Errors.Add("switch_lowering");foreach(var section in sw.Sections){AllocateBlock(Find(section,"switch-section"));foreach(var st in section.Statements)EmitRoslyn(st);}return;}
            if(s is ReturnStatementSyntax r){EmitExpression(r.Expression);return;}
            if(s is LocalDeclarationStatementSyntax ld){foreach(var v in ld.Declaration.Variables)if(v.Initializer!=null)EmitExpression(v.Initializer.Value);return;}
            if(s is ExpressionStatementSyntax ex){EmitExpression(ex.Expression,false);return;}
            foreach(var child in s.ChildNodes())if(child is StatementSyntax st)EmitRoslyn(st);else if(child is ExpressionSyntax expr)EmitExpression(expr);
        }
        public object Run(){
            if(Body==null)return new{method_id=Method(Symbol),complete=true,predicted_types=new string[0],variables=new Local[0],blocks=new Block[0],unsupported_reasons=new string[0],declaration_start=Declaration.SpanStart,declaration_end=Declaration.Span.End};
            var root=New(null,Body,"method");Sequence(root,Body.Statements);OptimizeLocals();EmitBlock(root,Body);
            // CS0012 is expected when Roslyn reads the mixed 2.0/2.0.5 reference
            // identities accepted by this smcs profile. All slot types still
            // require resolved symbols; other semantic failures reject a model.
            var diagnostics=Sem.GetDiagnostics(Declaration.Span).Where(d=>d.Severity==DiagnosticSeverity.Error&&d.Id!="CS0012").Select(d=>d.Id).Distinct().ToArray();
            if(diagnostics.Length>0)Errors.Add("semantic_errors:"+string.Join(",",diagnostics));
            return new{method_id=Method(Symbol),complete=Errors.Count==0,predicted_types=Slots.Select(v=>v.type).ToArray(),variables=Slots,blocks=Blocks,unsupported_reasons=Errors.Distinct().ToArray(),declaration_start=Declaration.SpanStart,declaration_end=Declaration.Span.End,body_start=Body.SpanStart,body_end=Body.Span.End};
        }
    }
    static object IteratorPrediction(SemanticModel sem,SyntaxNode root,string wanted){
        if(wanted==null||!wanted.Contains("/<GetEnumerator>d__")||!wanted.EndsWith("::MoveNext()->System.Boolean"))return null;
        string parent=wanted.Substring(0,wanted.IndexOf("/<GetEnumerator>d__"));
        foreach(var node in root.DescendantNodes().OfType<MethodDeclarationSyntax>()){
            var sym=sem.GetDeclaredSymbol(node);if(sym==null||TypeName(sym.ContainingType)!=parent||node.Identifier.ValueText!="GetEnumerator"||node.Body==null)continue;
            var statements=node.Body.Statements;var each=statements.LastOrDefault() as ForEachStatementSyntax;
            if(each==null||!(sem.GetTypeInfo(each.Expression).Type is IArrayTypeSymbol array)||array.Rank!=1||!(each.Statement is BlockSyntax block)||block.Statements.Count!=1||!(block.Statements[0] is YieldStatementSyntax yield)||!yield.IsKind(SyntaxKind.YieldReturnStatement)||!(yield.Expression is IdentifierNameSyntax id)||id.Identifier.ValueText!=each.Identifier.ValueText)continue;
            var types=new List<string>{"System.Int32"};var variables=new List<object>();bool valid=true;
            foreach(var statement in statements.Take(statements.Count-1)){
                if(!(statement is LocalDeclarationStatementSyntax declaration)){valid=false;break;}
                foreach(var v in declaration.Declaration.Variables){var local=sem.GetDeclaredSymbol(v) as ILocalSymbol;var refs=node.Body.DescendantNodes().OfType<IdentifierNameSyntax>().Where(n=>SymbolEqualityComparer.Default.Equals(local,sem.GetSymbolInfo(n).Symbol)).ToArray();if(refs.Length!=1||refs[0]!=each.Expression||v.Initializer==null){valid=false;break;}types.Add(LocalKey(local.Type));}
            }
            if(!valid)continue;
            types.Add(LocalKey(array.ElementType));return new{method_id=wanted,complete=true,predicted_types=types.ToArray(),variables=variables,blocks=new object[0],unsupported_reasons=new string[0],declaration_start=node.SpanStart,declaration_end=node.Span.End,body_start=node.Body.SpanStart,body_end=node.Body.Span.End,generated_model="array-foreach-yield-element-movenext",hidden_hoisted_fields=new[]{"<>7__wrap1:"+LocalKey(array),"<>7__wrap2:System.Int32"}};
        }
        return null;
    }
    static object Respond(JsonElement input){
        string path=Path.GetFullPath(input.GetProperty("path").GetString());var original=Trees[path];var tree=original;var compilation=Compilation;
        if(input.TryGetProperty("source",out var src)){tree=CSharpSyntaxTree.ParseText(src.GetString(),path:path);compilation=Compilation.ReplaceSyntaxTree(original,tree);}
        var sem=compilation.GetSemanticModel(tree);var root=tree.GetRoot();var output=new List<object>();string wanted=input.TryGetProperty("method_id",out var mi)?mi.GetString():null;
        var iterator=IteratorPrediction(sem,root,wanted);if(iterator!=null)output.Add(iterator);
        foreach(var node in root.DescendantNodes().Where(n=>n is BaseMethodDeclarationSyntax || n is AccessorDeclarationSyntax)){
            var symbol=sem.GetDeclaredSymbol(node) as IMethodSymbol;if(symbol==null)continue;string id=Method(symbol);if(wanted!=null&&id!=wanted)continue;
            BlockSyntax body=node is BaseMethodDeclarationSyntax m?m.Body:((AccessorDeclarationSyntax)node).Body;
            var model=new Model(sem,node,body,symbol);output.Add(model.Run());
        }
        return new{methods=output,source_only=true,candidate_compiled=false};
    }
    public static void Main(string[] args){
        var manifest=JsonDocument.Parse(File.ReadAllText(args[0])).RootElement;
        foreach(var item in manifest.GetProperty("sources").EnumerateArray()){string logical=item.ValueKind==JsonValueKind.String?item.GetString():item.GetProperty("path").GetString();string file=item.ValueKind==JsonValueKind.Object&&item.TryGetProperty("input_path",out var ip)?ip.GetString():logical;string path=Path.GetFullPath(logical);Trees[path]=CSharpSyntaxTree.ParseText(File.ReadAllText(file),path:path);}
        var refs=manifest.GetProperty("references").EnumerateArray().Select(item=>MetadataReference.CreateFromFile(item.ValueKind==JsonValueKind.String?item.GetString():item.GetProperty("path").GetString()));
        Compilation=CSharpCompilation.Create("Round6SourceOnly",Trees.Values,refs,new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,optimizationLevel:OptimizationLevel.Release));
        string line;while((line=Console.ReadLine())!=null){try{using(var d=JsonDocument.Parse(line))Console.WriteLine(JsonSerializer.Serialize(Respond(d.RootElement)));}catch(Exception e){Console.WriteLine(JsonSerializer.Serialize(new{error=e.GetType().Name,message=e.Message}));}}
    }
}
