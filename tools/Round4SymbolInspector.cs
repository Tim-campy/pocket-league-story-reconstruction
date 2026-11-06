// Read-only method bodies and compiler-produced PDB/MDB sequence points.
// Existing ProbeInspector remains the single canonical body serializer.
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;
using Mono.Cecil;
using Mono.Cecil.Cil;

public static class Round4SymbolInspector {
    static Dictionary<string,object> Obj(params object[] pairs) {
        var result=new Dictionary<string,object>();
        for(int i=0;i<pairs.Length;i+=2) result[(string)pairs[i]]=pairs[i+1];
        return result;
    }
    static IEnumerable<TypeDefinition> Types(IEnumerable<TypeDefinition> roots) {
        foreach(var type in roots) { yield return type; foreach(var child in Types(type.NestedTypes)) yield return child; }
    }
    public static int Main(string[] args) {
        Console.OutputEncoding=new UTF8Encoding(false);
        try {
            if(args.Length<1) throw new ArgumentException("Usage: Round4SymbolInspector assembly [--no-symbols] [--points-only] [stable-id-or-token ...]");
            var selectors=new HashSet<string>(args.Skip(1).Where(a=>!a.StartsWith("--")));
            bool pointsOnly=args.Contains("--points-only");
            var parameters=new ReaderParameters();
            string symbolKind="NONE";
            if(!args.Contains("--no-symbols")) {
                if(File.Exists(Path.ChangeExtension(args[0],".pdb"))) {
                    parameters.SymbolReaderProvider=new Mono.Cecil.Pdb.PdbReaderProvider(); symbolKind="PDB";
                } else if(File.Exists(args[0]+".mdb")) {
                    parameters.SymbolReaderProvider=new Mono.Cecil.Mdb.MdbReaderProvider(); symbolKind="MDB";
                }
                parameters.ReadSymbols=symbolKind!="NONE";
            }
            var module=ModuleDefinition.ReadModule(args[0],parameters);
            var data=pointsOnly?null:File.ReadAllBytes(args[0]);
            var dump=typeof(ProbeInspector).GetMethod("Dump",BindingFlags.NonPublic|BindingFlags.Static);
            var stable=typeof(ProbeInspector).GetMethod("StableId",BindingFlags.NonPublic|BindingFlags.Static);
            var methods=new List<object>();
            foreach(var type in Types(module.Types)) foreach(var method in type.Methods) {
                string id=(string)stable.Invoke(null,new object[]{method});
                string token="0x"+method.MetadataToken.ToUInt32().ToString("x8");
                if(selectors.Count!=0&&!selectors.Contains(id)&&!selectors.Contains(token)&&!selectors.Contains(type.FullName+"::"+method.Name)) continue;
                var body=pointsOnly?Obj("stable_id",id,"token",token):(Dictionary<string,object>)dump.Invoke(null,new object[]{data,method});
                var points=new List<object>();
                if(method.HasBody) foreach(var instruction in method.Body.Instructions) {
                    var point=instruction.SequencePoint;
                    if(point==null) continue;
                    points.Add(Obj("offset",instruction.Offset,"document",point.Document.Url,
                        "start_line",point.StartLine,"start_column",point.StartColumn,
                        "end_line",point.EndLine,"end_column",point.EndColumn,
                        "hidden",point.StartLine==0xfeefee));
                }
                body["sequence_points"]=points; methods.Add(body);
            }
            Console.WriteLine(new JavaScriptSerializer{MaxJsonLength=128000000}.Serialize(
                Obj("assembly",module.Assembly.Name.FullName,"mvid",module.Mvid.ToString(),
                    "symbol_kind",symbolKind,"methods",methods)));
            return 0;
        } catch(Exception error) { Console.Error.WriteLine(error.ToString()); return 1; }
    }
}
