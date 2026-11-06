// Read-only compiler-produced local names/scopes. Names come from symbols.
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Mono.Cecil;
using Mono.Cecil.Cil;

public static class Round5LocalInspector {
    static Dictionary<string,object> Obj(params object[] pairs) {
        var d=new Dictionary<string,object>();
        for(int i=0;i<pairs.Length;i+=2)d[(string)pairs[i]]=pairs[i+1];
        return d;
    }
    static IEnumerable<TypeDefinition> Types(IEnumerable<TypeDefinition> roots) {
        foreach(var type in roots){yield return type;foreach(var nested in Types(type.NestedTypes))yield return nested;}
    }
    static object ScopeShape(Scope scope,int end) {
        if(scope==null)return null;
        return Obj("start",scope.Start==null?0:scope.Start.Offset,"end",scope.End==null?end:scope.End.Offset,
            "variables",scope.Variables.Select(v=>(object)Obj("slot",v.Index,"name",v.Name,"type",v.VariableType.FullName)).ToArray(),
            "scopes",scope.Scopes.Select(s=>ScopeShape(s,end)).ToArray());
    }
    public static int Main(string[] args) {
        Console.OutputEncoding=new UTF8Encoding(false);
        try {
            if(args.Length<1)throw new ArgumentException("Usage: Round5LocalInspector assembly [stable-id ...]");
            var parameters=new ReaderParameters();string symbols="NONE";
            if(File.Exists(args[0]+".mdb")){parameters.SymbolReaderProvider=new Mono.Cecil.Mdb.MdbReaderProvider();symbols="MDB";}
            else if(File.Exists(Path.ChangeExtension(args[0],".pdb"))){parameters.SymbolReaderProvider=new Mono.Cecil.Pdb.PdbReaderProvider();symbols="PDB";}
            parameters.ReadSymbols=symbols!="NONE";
            var module=ModuleDefinition.ReadModule(args[0],parameters);var bytes=File.ReadAllBytes(args[0]);
            bool localsOnly=args.Contains("--locals-only");
            var selected=new HashSet<string>(args.Skip(1).Where(a=>!a.StartsWith("--")));var methods=new List<object>();
            var stable=typeof(ProbeInspector).GetMethod("StableId",BindingFlags.NonPublic|BindingFlags.Static);
            var dump=typeof(ProbeInspector).GetMethod("Dump",BindingFlags.NonPublic|BindingFlags.Static);
            foreach(var type in Types(module.Types))foreach(var method in type.Methods){
                var id=(string)stable.Invoke(null,new object[]{method});if(selected.Count!=0&&!selected.Contains(id))continue;
                var body=localsOnly?Obj("stable_id",id):(Dictionary<string,object>)dump.Invoke(null,new object[]{bytes,method});
                if(method.HasBody){
                    body["plain_local_types"]=method.Body.Variables.Select(v=>v.VariableType.FullName).ToArray();
                    body["locals_debug"]=method.Body.Variables.Select(v=>(object)Obj("slot",v.Index,"name",v.Name,"type",v.VariableType.FullName)).ToArray();
                    body["scope_debug"]=ScopeShape(method.Body.Scope,method.Body.CodeSize);
                }
                methods.Add(body);
            }
            Console.WriteLine(new JavaScriptSerializer{MaxJsonLength=128000000}.Serialize(Obj("symbol_kind",symbols,"methods",methods)));
            return 0;
        }catch(Exception error){Console.Error.WriteLine(error);return 1;}
    }
}
