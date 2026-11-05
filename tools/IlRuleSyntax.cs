// Syntax-only hypotheses. Every generated patch needs complete IL verification.
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
public static class IlRuleSyntax {
 static string TypeName(INamedTypeSymbol n) => n.ContainingType!=null?TypeName(n.ContainingType)+"/"+n.MetadataName:(n.ContainingNamespace.IsGlobalNamespace?"":n.ContainingNamespace+".")+n.MetadataName;
 static string Key(ITypeSymbol t) {
  if(t is ITypeParameterSymbol p)return(p.TypeParameterKind==TypeParameterKind.Method?"!!":"!")+p.Ordinal;
  if(t is IArrayTypeSymbol a)return Key(a.ElementType)+(a.Rank==1?"[]":"["+new string(',',a.Rank-1)+"]");
  if(t is IPointerTypeSymbol ptr)return Key(ptr.PointedAtType)+"*";
  var n=(INamedTypeSymbol)t;return TypeName(n.OriginalDefinition)+(n.TypeArguments.Length>0?"<"+string.Join(",",n.TypeArguments.Select(Key))+">":"");
 }
 static string Method(IMethodSymbol m)=>TypeName(m.ContainingType)+"::"+m.MetadataName+(m.Arity==0?"":"``"+m.Arity)+"("+string.Join(",",m.Parameters.Select(p=>Key(p.Type)+(p.RefKind==RefKind.None?"":"&")))+")->"+Key(m.ReturnType);
 static bool Integral(ITypeSymbol t) => t!=null && new[]{SpecialType.System_SByte,SpecialType.System_Byte,SpecialType.System_Int16,SpecialType.System_UInt16,SpecialType.System_Int32,SpecialType.System_UInt32,SpecialType.System_Int64,SpecialType.System_UInt64,SpecialType.System_Char}.Contains(t.SpecialType);
 static string Negate(ExpressionSyntax e,SemanticModel model) {
  if(e is ParenthesizedExpressionSyntax p)return Negate(p.Expression,model);
  if(e is PrefixUnaryExpressionSyntax u&&u.IsKind(SyntaxKind.LogicalNotExpression))return u.Operand.ToString();
  if(e is BinaryExpressionSyntax b){
   if(b.IsKind(SyntaxKind.LogicalAndExpression)||b.IsKind(SyntaxKind.LogicalOrExpression))return "("+Negate(b.Left,model)+") "+(b.IsKind(SyntaxKind.LogicalAndExpression)?"||":"&&")+" ("+Negate(b.Right,model)+")";
   var ops=new Dictionary<SyntaxKind,string>{{SyntaxKind.EqualsExpression,"!="},{SyntaxKind.NotEqualsExpression,"=="}};
   if(Integral(model.GetTypeInfo(b.Left).Type)&&Integral(model.GetTypeInfo(b.Right).Type)){
    ops[SyntaxKind.LessThanExpression]=">=";ops[SyntaxKind.LessThanOrEqualExpression]=">";
    ops[SyntaxKind.GreaterThanExpression]="<=";ops[SyntaxKind.GreaterThanOrEqualExpression]="<";
   }
   // Floating, lifted and unresolved relational comparisons retain explicit !.
   if(ops.TryGetValue(b.Kind(),out var op))return b.Left+" "+op+" "+b.Right;
  }
  return "!("+e+")";
 }
 static bool Terminates(StatementSyntax s) {
  if(s is ReturnStatementSyntax||s is ThrowStatementSyntax)return true;
  return s is BlockSyntax b&&b.Statements.Count>0&&Terminates(b.Statements.Last());
 }
 static string GuardChain(StatementSyntax s,SemanticModel model) {
  if(s is BlockSyntax block&&block.Statements.Count>0){
   var last=block.Statements.Last();
   return "{\n"+string.Join("\n",block.Statements.Take(block.Statements.Count-1))+"\n"+GuardChain(last,model)+"\n}";
  }
  if(s is IfStatementSyntax branch&&branch.Else==null)return "if ("+Negate(branch.Condition,model)+") { return; }\n"+GuardChain(branch.Statement,model);
  return s.ToString();
 }
 public static void Main(string[] args) {
  var input=JsonDocument.Parse(File.ReadAllText(args[0])).RootElement;
  var trees=input.GetProperty("sources").EnumerateArray().Select(p=>CSharpSyntaxTree.ParseText(File.ReadAllText(p.GetString()),path:p.GetString())).ToArray();
  var refs=input.GetProperty("references").EnumerateArray().Select(p=>MetadataReference.CreateFromFile(p.GetString())).ToArray();
  var c=CSharpCompilation.Create("Recovered",trees,refs,new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
  var records=new List<object>();
  foreach(var tree in trees){var model=c.GetSemanticModel(tree,true);var text=tree.GetText().ToString();
   foreach(var node in tree.GetRoot().DescendantNodes().Where(n=>n is BaseMethodDeclarationSyntax||n is AccessorDeclarationSyntax)){
    var symbol=model.GetDeclaredSymbol(node) as IMethodSymbol;if(symbol==null)continue;
    BlockSyntax body=node is BaseMethodDeclarationSyntax m?m.Body:((AccessorDeclarationSyntax)node).Body;if(body==null)continue;
    var patches=new List<object>();
    Action<string,SyntaxNode,string> add=(rule,n,replacement)=>patches.Add(new{rule,start=n.SpanStart,end=n.Span.End,before=text.Substring(n.SpanStart,n.Span.Length),after=replacement});
    foreach(var tern in body.DescendantNodes().OfType<ConditionalExpressionSyntax>())add("ternary-polarity",tern,"("+Negate(tern.Condition,model)+") ? ("+tern.WhenFalse+") : ("+tern.WhenTrue+")");
    foreach(var branch in body.DescendantNodes().OfType<IfStatementSyntax>()){
     if(branch.Else!=null)add("if-polarity",branch,"if ("+Negate(branch.Condition,model)+") "+branch.Else.Statement+" else "+branch.Statement);
     if(symbol.ReturnsVoid&&branch.Else==null&&branch.Parent is BlockSyntax block&&block.Statements.Last()==branch){
      var stmts=branch.Statement is BlockSyntax sb?string.Join("\n",sb.Statements):branch.Statement.ToString();
      add("tail-early-return",branch,"if ("+Negate(branch.Condition,model)+") { return; }\n"+stmts);
     }
     if(symbol.ReturnsVoid&&branch.Else==null&&branch.Parent==body&&body.Statements.Last()==branch)
      add("guard-return",branch,"if ("+Negate(branch.Condition,model)+") { return; }\n"+GuardChain(branch.Statement,model));
     if(branch.Else!=null){
      if(Terminates(branch.Statement))add("guard-return",branch,"if ("+branch.Condition+") "+branch.Statement+"\n"+branch.Else.Statement);
      else if(Terminates(branch.Else.Statement))add("guard-return",branch,"if ("+Negate(branch.Condition,model)+") "+branch.Else.Statement+"\n"+branch.Statement);
      if(Terminates(branch.Statement)&&Terminates(branch.Else.Statement))
       add("guard-return-reverse",branch,"if ("+Negate(branch.Condition,model)+") "+branch.Else.Statement+"\n"+branch.Statement);
     }
     ExpressionSyntax value=branch.Condition;bool negative=false;
     while(value is ParenthesizedExpressionSyntax par)value=par.Expression;
     if(value is PrefixUnaryExpressionSyntax not&&not.IsKind(SyntaxKind.LogicalNotExpression)){value=not.Operand;negative=true;}
     var valueType=model.GetTypeInfo(value).Type;
     if(valueType!=null&&valueType.SpecialType==SpecialType.System_Boolean&&value is not IdentifierNameSyntax&&value is not LiteralExpressionSyntax){
      string name="recoveredCondition_"+branch.SpanStart;
      string edited=branch.WithCondition(SyntaxFactory.ParseExpression((negative?"!":"")+name)).ToString();
      var patch=new{rule="condition-local",start=branch.SpanStart,end=branch.Span.End,before=text.Substring(branch.SpanStart,branch.Span.Length),
       after="{ bool "+name+" = "+value+";\n"+edited+"\n}",temp_type="System.Boolean"};
      patches.Add(patch);
     }
    }
    foreach(var sw in body.DescendantNodes().OfType<SwitchStatementSyntax>()) {
     var inputSymbol=model.GetSymbolInfo(sw.Expression).Symbol;
     if(!(inputSymbol is ILocalSymbol||inputSymbol is IParameterSymbol||inputSymbol is IFieldSymbol))continue;
     if(sw.Sections.Count>8||sw.Sections.Any(s=>s.Statements.Count==0))continue;
     var pieces=new List<string>();string otherwise=null;bool valid=true;
     foreach(var section in sw.Sections){
      if(section.Statements.Take(section.Statements.Count-1).Any(s=>s.DescendantNodesAndSelf().Any(n=>n is BreakStatementSyntax||n is GotoStatementSyntax))){valid=false;break;}
      var statements=section.Statements.ToList();if(statements.Last() is BreakStatementSyntax)statements.RemoveAt(statements.Count-1);
      else if(!(statements.Last() is ReturnStatementSyntax||statements.Last() is ThrowStatementSyntax)){valid=false;break;}
      string block="{ "+string.Join("\n",statements)+" }";
      if(section.Labels.Any(l=>l is DefaultSwitchLabelSyntax)){otherwise=block;continue;}
      var labels=section.Labels.OfType<CaseSwitchLabelSyntax>().ToArray();
      if(labels.Length!=section.Labels.Count){valid=false;break;}
      var condition=string.Join(" || ",labels.Select(l=>"("+sw.Expression+" == "+l.Value+")"));
      pieces.Add("if ("+condition+") "+block);
     }
     if(valid&&pieces.Count>0)add("switch-if",sw,string.Join(" else ",pieces)+(otherwise==null?"":" else "+otherwise));
    }
    var returns=body.DescendantNodes().OfType<ReturnStatementSyntax>().Where(r=>r.Expression!=null&&!r.Ancestors().TakeWhile(a=>a!=body).Any(a=>a is AnonymousFunctionExpressionSyntax)).ToArray();
    foreach(var r in returns){
     ExpressionSyntax expression=r.Expression;
     while(expression is ParenthesizedExpressionSyntax p)expression=p.Expression;
     if(expression is ConditionalExpressionSyntax conditional){
      add("value-guard",r,"{ if ("+conditional.Condition+") { return "+conditional.WhenTrue+"; } return "+conditional.WhenFalse+"; }");
      add("value-guard-reverse",r,"{ if ("+Negate(conditional.Condition,model)+") { return "+conditional.WhenFalse+"; } return "+conditional.WhenTrue+"; }");
     }
    }
    if(!symbol.ReturnsVoid&&returns.Length>0){var last=returns.Last();add("return-local",last,"{ "+symbol.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)+" recoveredReturn = "+last.Expression+"; return recoveredReturn; }");}
    var locals=body.DescendantNodes().OfType<VariableDeclaratorSyntax>().Select(v=>new{name=v.Identifier.ValueText,type=model.GetDeclaredSymbol(v) is ILocalSymbol l?Key(l.Type):null,start=v.SpanStart}).Where(l=>l.type!=null).ToArray();
    var constructs=new{foreach_count=body.DescendantNodes().OfType<ForEachStatementSyntax>().Count(),using_count=body.DescendantNodes().OfType<UsingStatementSyntax>().Count(),lock_count=body.DescendantNodes().OfType<LockStatementSyntax>().Count(),switch_count=body.DescendantNodes().OfType<SwitchStatementSyntax>().Count(),lambda_count=body.DescendantNodes().OfType<AnonymousFunctionExpressionSyntax>().Count()};
    records.Add(new{id=Method(symbol),path=tree.FilePath,locals,constructs,patches});
   }
  }
  Console.WriteLine(JsonSerializer.Serialize(new{methods=records,errors=c.GetDiagnostics().Where(d=>d.Severity==DiagnosticSeverity.Error).Take(8).Select(d=>d.ToString()).ToArray()}));
 }
}
