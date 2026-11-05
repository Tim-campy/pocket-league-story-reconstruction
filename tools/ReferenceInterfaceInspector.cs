// Read metadata from framework references only; no target CLR load or execution.
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Web.Script.Serialization;
using Mono.Cecil;
public static class ReferenceInterfaceInspector {
 public static void Main(string[] args) {
  var rows=new List<object>();
  foreach(var path in args){
   var module=ModuleDefinition.ReadModule(path);
   var selected=new[]{"System.Collections.Generic.IEnumerator`1","System.Collections.Generic.IEnumerable`1","System.Collections.IEnumerator","System.IDisposable"};
   string hash;using(var sha=SHA256.Create())hash=BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(path))).Replace("-","").ToLowerInvariant();
   rows.Add(new{path,sha256=hash,assembly=module.Assembly.Name.FullName,types=selected.Select(name=>{
    var type=module.Types.Single(t=>t.FullName==name);
    return new{name,interfaces=type.Interfaces.Select(i=>new{name=i.FullName,scope=i.Scope.ToString()}).ToArray()};
   }).ToArray()});
  }
  Console.WriteLine(new JavaScriptSerializer{MaxJsonLength=1000000}.Serialize(rows));
 }
}
