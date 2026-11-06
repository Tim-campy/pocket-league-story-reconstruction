// Root-controlled opt-in dispatcher adapter. Add this and Candidates only when integrating D.
using System;using System.Linq;using System.Collections.Generic;using System.Text.Json;
using Microsoft.CodeAnalysis;using Microsoft.CodeAnalysis.CSharp.Syntax;
public static partial class Round7TransformSyntax {
 static void AddRound9ScopedExitRules(JsonElement input,string text,StatementSyntax mapped,BlockSyntax methodBody,SemanticModel model,int point,List<Patch> patches,HashSet<string> seen){
  if(!input.TryGetProperty("compiler",out var compiler)||compiler.GetString()!="smcs")return;
  if(!input.TryGetProperty("round9_scoped_exit_valid",out var valid)||valid.ValueKind!=JsonValueKind.True)return;
  if(!input.TryGetProperty("round9_scoped_exit_original_opcode",out var opcode)||!input.TryGetProperty("round9_scoped_exit_join_kind",out var join))return;
  string selected=input.TryGetProperty("round7_cycle_rule",out var r)?r.GetString():"";
  string[] learned=input.TryGetProperty("round7_cycle_rules",out var learnedJson)&&learnedJson.ValueKind==JsonValueKind.Array?learnedJson.EnumerateArray().Select(x=>x.GetString()).ToArray():new string[0];
  int sourcepoint=input.TryGetProperty("round9_scoped_exit_join_source_point",out var sp)?sp.GetInt32():-1;
  foreach(string family in new[]{"terminal-if-arm-return-flatten-else","scoped-terminal-arm-common-join"}){
   if(selected!=family&&!learned.Contains(family))continue;
   foreach(var candidate in Round9ScopedExitCandidates.Generate(text,methodBody.SyntaxTree.GetRoot(),model,point,family,opcode.GetString(),join.GetString(),sourcepoint)){
    if(!seen.Add(candidate.start+":"+candidate.end+":"+candidate.after))continue;
    patches.Add(new Patch{rule=candidate.rule,start=candidate.start,end=candidate.end,before=candidate.before,after=candidate.after,distance=candidate.distance});
   }
  }
 }
}
