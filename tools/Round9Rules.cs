// Source transformations only. State and body equality belong to the verifier.
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
public static partial class Round7TransformSyntax {
    static void AddRound9Rules(JsonElement input,string text,StatementSyntax mapped,BlockSyntax methodBody,
        SemanticModel model,int point,List<Patch> patches,HashSet<string> seen) {
        AddRound8CompoundRules(input,text,mapped,methodBody,model,point,patches,seen);
        AddRound8NullConditionalPolarityRules(input,text,mapped,methodBody,model,point,patches,seen);
    }
}
