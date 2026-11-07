// Source transformations only. State and body equality belong to the verifier.
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
public static partial class Round7TransformSyntax {
    static void AddRound9Rules(JsonElement input,string text,StatementSyntax mapped,BlockSyntax methodBody,
        SemanticModel model,int point,List<Patch> patches,HashSet<string> seen) {
        AddRound8CompoundRules(input,text,mapped,methodBody,model,point,patches,seen);
        AddRound8NullConditionalPolarityRules(input,text,mapped,methodBody,model,point,patches,seen);
        AddRound9PartBRules(input,text,mapped,methodBody,model,point,patches,seen);
        AddRound9FloatingIdentityRules(input,text,mapped,methodBody,model,point,patches,seen);
        AddRound9PerArmStoreRules(input,text,mapped,methodBody,model,point,patches,seen);
        AddRound9ScopedExitRules(input,text,mapped,methodBody,model,point,patches,seen);
        AddRound9ArrayPopulationRules(input,text,mapped,methodBody,model,point,patches,seen);
        AddRound9FieldDivisionRules(input,text,mapped,methodBody,model,point,patches,seen);
        AddRound9BooleanContextRules(input,text,mapped,methodBody,model,point,patches,seen);
        AddRound9ProtectedExitRules(input,text,mapped,methodBody,model,point,patches,seen);
        // Generation may include previously verified families. The engine still
        // filters all other families until this cycle's selected rule advances.
        if(input.TryGetProperty("round7_cycle_rules",out var learned)&&learned.ValueKind==JsonValueKind.Array) {
            string selected=input.TryGetProperty("round7_cycle_rule",out var current)?current.GetString():null;
            foreach(string family in learned.EnumerateArray().Where(e=>e.ValueKind==JsonValueKind.String)
                .Select(e=>e.GetString()).Distinct()) {
                if(family==selected||family!="d9-explicit-float-identity-parameter"&&family!="d9-calculated-per-arm-local-stores")continue;
                var request=input.EnumerateObject().ToDictionary(p=>p.Name,p=>p.Value.Clone());
                request["round7_cycle_rule"]=JsonSerializer.SerializeToElement(family);
                using(var inherited=JsonDocument.Parse(JsonSerializer.Serialize(request))) {
                    AddRound9FloatingIdentityRules(inherited.RootElement,text,mapped,methodBody,model,point,patches,seen);
                    AddRound9PerArmStoreRules(inherited.RootElement,text,mapped,methodBody,model,point,patches,seen);
                }
            }
        }
    }
}
