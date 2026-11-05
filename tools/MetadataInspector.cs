// Read metadata only. No target assembly is loaded or executed by the CLR.
// Tokens/RVAs are coordinates; compared identities explicitly resolve tokens.
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Web.Script.Serialization;
using Mono.Cecil;

public static class MetadataInspector {
    static Dictionary<string,object> O(params object[] pairs) {
        var d=new Dictionary<string,object>();for(int i=0;i<pairs.Length;i+=2)d[(string)pairs[i]]=pairs[i+1];return d;
    }
    static string Hex(byte[] data) { return BitConverter.ToString(data); }
    static string Hash(byte[] data) { using(var sha=SHA256.Create())return Hex(sha.ComputeHash(data)).Replace("-", "").ToLowerInvariant(); }
    static string Scope(TypeReference t) {
        var module=t.Scope as ModuleDefinition;
        return module!=null?module.Assembly.Name.FullName:t.Scope==null?"UNKNOWN":t.Scope.ToString();
    }
    static string TypeKey(TypeReference t) {
        if(t==null)return null;
        var g=t as GenericParameter;if(g!=null)return (g.Type==GenericParameterType.Method?"!!":"!")+g.Position;
        var i=t as GenericInstanceType;if(i!=null)return TypeKey(i.ElementType)+"<"+string.Join(",",i.GenericArguments.Select(TypeKey).ToArray())+">";
        var a=t as ArrayType;if(a!=null)return TypeKey(a.ElementType)+(a.IsVector?"[]":"["+string.Join(",",a.Dimensions.Select(x=>x.ToString()).ToArray())+"]");
        var r=t as RequiredModifierType;if(r!=null)return "modreq("+TypeKey(r.ModifierType)+")"+TypeKey(r.ElementType);
        var o=t as OptionalModifierType;if(o!=null)return "modopt("+TypeKey(o.ModifierType)+")"+TypeKey(o.ElementType);
        var s=t as TypeSpecification;
        if(s!=null) {
            string suffix=t is ByReferenceType?"&":t is PointerType?"*":t is PinnedType?" pinned":t is SentinelType?" sentinel":null;
            if(suffix==null)throw new InvalidDataException("Unsupported TypeSpec "+t.GetType().FullName);
            return TypeKey(s.ElementType)+suffix;
        }
        return t.FullName;
    }
    static object Type(TypeReference t) {
        if(t==null)return null;
        var g=t as GenericParameter;if(g!=null)return O("generic",TypeKey(g));
        var i=t as GenericInstanceType;if(i!=null)return O("generic_instance",Type(i.ElementType),"arguments",i.GenericArguments.Select(Type).ToArray());
        var a=t as ArrayType;if(a!=null)return O("array",Type(a.ElementType),"vector",a.IsVector,"dimensions",a.Dimensions.Select(x=>(object)O("lower",x.LowerBound,"upper",x.UpperBound)).ToArray());
        var r=t as RequiredModifierType;if(r!=null)return O("modreq",Type(r.ModifierType),"element",Type(r.ElementType));
        var o=t as OptionalModifierType;if(o!=null)return O("modopt",Type(o.ModifierType),"element",Type(o.ElementType));
        var s=t as TypeSpecification;if(s!=null)return O("specification",t.GetType().Name,"element",Type(s.ElementType),"key",TypeKey(t));
        return O("name",t.FullName,"scope",Scope(t),"value_type",t.IsValueType);
    }
    static string StableId(MethodReference m) {
        return m.DeclaringType.FullName+"::"+m.Name+(m.GenericParameters.Count==0?"":"``"+m.GenericParameters.Count)+"("+string.Join(",",m.Parameters.Select(p=>TypeKey(p.ParameterType)).ToArray())+")->"+TypeKey(m.ReturnType);
    }
    static object MethodReferenceShape(MethodReference m) {
        return O("id",StableId(m),"declaring_type",Type(m.DeclaringType),"return_type",Type(m.ReturnType),
            "parameters",m.Parameters.Select(p=>Type(p.ParameterType)).ToArray(),"has_this",m.HasThis,
            "explicit_this",m.ExplicitThis,"calling_convention",(int)m.CallingConvention);
    }
    static object[] Attributes(Mono.Cecil.ICustomAttributeProvider provider) {
        return provider.CustomAttributes.Select(a=>(object)O("constructor",MethodReferenceShape(a.Constructor),"blob",Hex(a.GetBlob()))).ToArray();
    }
    static object Constant(object value) {
        if(value==null)return null;
        if(value is float)return O("type","System.Single","bits",Hex(BitConverter.GetBytes((float)value)));
        if(value is double)return O("type","System.Double","bits",Hex(BitConverter.GetBytes((double)value)));
        if(value is char)return O("type","System.Char","value",(int)(char)value);
        return O("type",value.GetType().FullName,"value",value);
    }
    static object Marshal(IMarshalInfoProvider provider) {
        if(!provider.HasMarshalInfo)return null;
        var info=provider.MarshalInfo;var properties=new Dictionary<string,object>();
        foreach(var p in info.GetType().GetProperties(BindingFlags.Instance|BindingFlags.Public).OrderBy(p=>p.Name)) {
            if(!p.CanRead||p.GetIndexParameters().Length!=0)continue;
            object value=p.GetValue(info,null);
            if(value is TypeReference)value=Type((TypeReference)value);
            else if(value!=null&&value.GetType().IsEnum)value=Convert.ToInt64(value,CultureInfo.InvariantCulture);
            else if(value!=null&&!(value is string)&&!value.GetType().IsPrimitive)throw new InvalidDataException("Unsupported marshal property "+p.Name);
            properties[p.Name]=value;
        }
        return O("kind",info.GetType().Name,"properties",properties);
    }
    static object[] Security(ISecurityDeclarationProvider p) {
        return p.SecurityDeclarations.Select(s=>(object)O("action",(int)s.Action,"blob",Hex(s.GetBlob()))).ToArray();
    }
    static object[] GenericParameters(IGenericParameterProvider p) {
        return p.GenericParameters.Select(g=>(object)O("name",g.Name,"position",g.Position,"attributes",(int)g.Attributes,
            "constraints",g.Constraints.Select(Type).ToArray(),"custom_attributes",Attributes(g))).ToArray();
    }
    static object Parameter(ParameterDefinition p) {
        return O("name",p.Name,"index",p.Index,"attributes",(int)p.Attributes,"type",Type(p.ParameterType),
            "has_constant",p.HasConstant,"constant",p.HasConstant?Constant(p.Constant):null,
            "marshal",Marshal(p),"custom_attributes",Attributes(p));
    }
    static object Return(MethodReturnType p) {
        var property=typeof(MethodReturnType).GetProperty("Parameter",BindingFlags.Instance|BindingFlags.NonPublic);
        if(property==null)throw new InvalidDataException("Cecil return parameter metadata accessor unavailable");
        var parameter=(ParameterDefinition)property.GetValue(p,null);
        return O("name",parameter.Name,"has_param_row",p.MetadataToken.RID!=0,"attributes",(int)p.Attributes,"type",Type(p.ReturnType),
            "has_constant",p.HasConstant,"constant",p.HasConstant?Constant(p.Constant):null,
            "marshal",Marshal(p),"custom_attributes",Attributes(p));
    }
    static object Method(MethodDefinition m) {
        return O("id",StableId(m),"name",m.Name,"attributes",(int)m.Attributes,"impl_attributes",(int)m.ImplAttributes,
            "semantics_attributes",(int)m.SemanticsAttributes,"has_body",m.HasBody,
            "has_this",m.HasThis,"explicit_this",m.ExplicitThis,"calling_convention",(int)m.CallingConvention,
            "return",Return(m.MethodReturnType),"parameters",m.Parameters.Select(Parameter).ToArray(),
            "generic_parameters",GenericParameters(m),"custom_attributes",Attributes(m),"security",Security(m),
            "overrides",m.Overrides.Select(MethodReferenceShape).ToArray(),
            "pinvoke",m.HasPInvokeInfo?O("attributes",(int)m.PInvokeInfo.Attributes,"entry_point",m.PInvokeInfo.EntryPoint,"module",m.PInvokeInfo.Module.Name):null);
    }
    static object Field(FieldDefinition f) {
        return O("id",f.DeclaringType.FullName+"::"+f.Name,"name",f.Name,"type",Type(f.FieldType),"attributes",(int)f.Attributes,
            "has_constant",f.HasConstant,"constant",f.HasConstant?Constant(f.Constant):null,
            "offset",f.Offset,"initial_value",Hex(f.InitialValue),"marshal",Marshal(f),"custom_attributes",Attributes(f));
    }
    static object Property(PropertyDefinition p) {
        string id=p.DeclaringType.FullName+"::"+p.Name+"("+string.Join(",",p.Parameters.Select(x=>TypeKey(x.ParameterType)).ToArray())+")->"+TypeKey(p.PropertyType);
        return O("id",id,"name",p.Name,"attributes",(int)p.Attributes,"type",Type(p.PropertyType),"has_this",p.HasThis,
            "has_constant",p.HasConstant,"constant",p.HasConstant?Constant(p.Constant):null,
            "parameters",p.Parameters.Select(Parameter).ToArray(),"get",p.GetMethod==null?null:StableId(p.GetMethod),
            "set",p.SetMethod==null?null:StableId(p.SetMethod),"other_methods",p.OtherMethods.Select(StableId).ToArray(),"custom_attributes",Attributes(p));
    }
    static object Event(EventDefinition e) {
        return O("id",e.DeclaringType.FullName+"::"+e.Name+":"+TypeKey(e.EventType),"name",e.Name,"attributes",(int)e.Attributes,"type",Type(e.EventType),
            "add",e.AddMethod==null?null:StableId(e.AddMethod),"remove",e.RemoveMethod==null?null:StableId(e.RemoveMethod),
            "invoke",e.InvokeMethod==null?null:StableId(e.InvokeMethod),"other_methods",e.OtherMethods.Select(StableId).ToArray(),"custom_attributes",Attributes(e));
    }
    static object TypeDefinitionShape(TypeDefinition t) {
        return O("name",t.FullName,"namespace",t.Namespace,"attributes",(int)t.Attributes,"base_type",Type(t.BaseType),
            "declaring_type",t.DeclaringType==null?null:t.DeclaringType.FullName,"interfaces",t.Interfaces.Select(Type).ToArray(),
            "custom_attributes",Attributes(t),"security",Security(t),"packing_size",t.PackingSize,"class_size",t.ClassSize,
            "generic_parameters",GenericParameters(t),"fields",t.Fields.Select(Field).ToArray(),"methods",t.Methods.Select(Method).ToArray(),
            "properties",t.Properties.Select(Property).ToArray(),"events",t.Events.Select(Event).ToArray(),"nested_types",t.NestedTypes.Select(x=>x.FullName).ToArray());
    }
    static IEnumerable<TypeDefinition> AllTypes(IEnumerable<TypeDefinition> types) {
        foreach(var t in types) {yield return t;foreach(var n in AllTypes(t.NestedTypes))yield return n;}
    }
    static object Resource(Resource r) {
        var e=r as EmbeddedResource;var l=r as LinkedResource;var a=r as AssemblyLinkedResource;
        return O("name",r.Name,"attributes",(int)r.Attributes,"kind",r.ResourceType.ToString(),
            "data",e==null?null:Hex(e.GetResourceData()),"file",l==null?null:l.File,"hash",l==null?null:Hex(l.Hash),
            "assembly",a==null?null:a.Assembly.FullName);
    }
    public static int Main(string[] args) {
        Console.OutputEncoding=new UTF8Encoding(false);
        if(args.Length!=1){Console.Error.WriteLine("Usage: MetadataInspector.exe assembly");return 2;}
        try {
            // Cecil resolves no dependency to execute code; all identities use admitted references.
            var module=ModuleDefinition.ReadModule(args[0]);var types=AllTypes(module.Types).OrderBy(t=>t.MetadataToken.RID).ToArray();
            var result=O("schema_version",1,"artifact_sha256",Hash(File.ReadAllBytes(args[0])),"assembly",module.Assembly.Name.FullName,
                "assembly_definition",O("name",module.Assembly.Name.Name,"version",module.Assembly.Name.Version.ToString(),
                    "culture",module.Assembly.Name.Culture,"attributes",(int)module.Assembly.Name.Attributes,
                    "hash_algorithm",(int)module.Assembly.Name.HashAlgorithm,"public_key",Hex(module.Assembly.Name.PublicKey)),
                "module_definition",O("name",module.Name,"runtime_version",module.RuntimeVersion),
                "coordinates",O("mvid",module.Mvid.ToString(),"type_tokens",types.Select(t=>t.MetadataToken.ToString()).ToArray()),
                "types",types.Select(TypeDefinitionShape).ToArray(),"assembly_attributes",Attributes(module.Assembly),
                "module_attributes",Attributes(module),"assembly_security",Security(module.Assembly),
                "assembly_references",module.AssemblyReferences.Select(a=>(object)O("identity",a.FullName,"attributes",(int)a.Attributes,
                    "hash",Hex(a.Hash),"hash_algorithm",(int)a.HashAlgorithm)).ToArray(),
                "module_references",module.ModuleReferences.Select(m=>m.Name).ToArray(),"resources",module.Resources.Select(Resource).ToArray(),
                "exported_types",module.ExportedTypes.Select(t=>(object)O("name",t.FullName,"attributes",(int)t.Attributes,
                    "identifier",t.Identifier,"scope",t.Scope.ToString())).ToArray(),
                "limitations",new[]{"Structural metadata comparison resolves token identities; it is not raw table/heap/PE byte identity.",
                    "MVID is retained as a coordinate and is not used to assert type metadata equality. No output binary is modified."});
            Console.WriteLine(new JavaScriptSerializer {MaxJsonLength=128000000}.Serialize(result));return 0;
        }catch(Exception ex){Console.Error.WriteLine(ex.ToString());return 1;}
    }
}
