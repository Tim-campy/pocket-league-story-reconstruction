// Read-only inspection of selected methods through Mono.Cecil. No target code
// is loaded into the CLR or invoked. Canonical results resolve metadata tokens.
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web.Script.Serialization;
using Mono.Cecil;
using Mono.Cecil.Cil;

public static class ProbeInspector {
    static Dictionary<string,object> Obj(params object[] pairs) {
        var r=new Dictionary<string,object>();
        for(int i=0;i<pairs.Length;i+=2) r[(string)pairs[i]]=pairs[i+1];
        return r;
    }
    static string Scope(TypeReference t) {
        var module=t.Scope as ModuleDefinition;
        if(module!=null) return module.Assembly.Name.FullName;
        return t.Scope==null ? "UNKNOWN" : t.Scope.ToString();
    }
    static string TypeId(TypeReference t) {
        var generic=t as GenericParameter;
        if(generic!=null) return (generic.Type==GenericParameterType.Method?"!!":"!")+generic.Position;
        var instance=t as GenericInstanceType;
        if(instance!=null) return TypeId(instance.ElementType)+"<"+string.Join(",",instance.GenericArguments.Select(TypeId).ToArray())+">";
        var array=t as ArrayType;
        if(array!=null) return TypeId(array.ElementType)+(array.IsVector?"[]":"["+string.Join(",",array.Dimensions.Select(d=>d.ToString()).ToArray())+"]");
        var required=t as RequiredModifierType;
        if(required!=null) return "modreq("+TypeId(required.ModifierType)+")"+TypeId(required.ElementType);
        var optional=t as OptionalModifierType;
        if(optional!=null) return "modopt("+TypeId(optional.ModifierType)+")"+TypeId(optional.ElementType);
        var specification=t as TypeSpecification;
        if(specification!=null) {
            string suffix=t is ByReferenceType?"&":t is PointerType?"*":t is PinnedType?" pinned":t is SentinelType?" sentinel":null;
            if(suffix==null) throw new InvalidDataException("Unsupported TypeSpec: "+t.GetType().FullName);
            return TypeId(specification.ElementType)+suffix;
        }
        return (t.IsValueType?"valuetype ":"class ")+t.FullName+" @ "+Scope(t);
    }
    static string TypeKey(TypeReference t) {
        var generic=t as GenericParameter;
        if(generic!=null) return (generic.Type==GenericParameterType.Method?"!!":"!")+generic.Position;
        var instance=t as GenericInstanceType;
        if(instance!=null) return TypeKey(instance.ElementType)+"<"+string.Join(",",instance.GenericArguments.Select(TypeKey).ToArray())+">";
        var array=t as ArrayType;
        if(array!=null) return TypeKey(array.ElementType)+(array.IsVector?"[]":"["+string.Join(",",array.Dimensions.Select(d=>d.ToString()).ToArray())+"]");
        var spec=t as TypeSpecification;
        if(spec!=null) {
            if(t is ByReferenceType) return TypeKey(spec.ElementType)+"&";
            if(t is PointerType) return TypeKey(spec.ElementType)+"*";
        }
        return t.FullName;
    }
    static string StableId(MethodReference m) {
        string arity=m.GenericParameters.Count==0?"":"``"+m.GenericParameters.Count;
        return m.DeclaringType.FullName+"::"+m.Name+arity+"("+string.Join(",",m.Parameters.Select(p=>TypeKey(p.ParameterType)).ToArray())+")->"+TypeKey(m.ReturnType);
    }
    static string MethodId(MethodReference m) {
        var generic=m as GenericInstanceMethod;
        string args=generic==null?"":"<"+string.Join(",",generic.GenericArguments.Select(TypeId).ToArray())+">";
        return TypeId(m.DeclaringType)+"::"+m.Name+"``"+m.GenericParameters.Count+args+"("+
            string.Join(",",m.Parameters.Select(p=>TypeId(p.ParameterType)).ToArray())+")"+
            "->"+TypeId(m.ReturnType)+";this="+m.HasThis+";explicit="+m.ExplicitThis+";cc="+m.CallingConvention;
    }
    static object Operand(object o) {
        if(o==null) return null;
        var ins=o as Instruction;
        if(ins!=null) return Obj("branch",ins.Offset);
        var targets=o as Instruction[];
        if(targets!=null) return Obj("switch",targets.Select(i=>i.Offset).ToArray());
        var method=o as MethodReference;
        if(method!=null) return Obj("method",MethodId(method));
        var field=o as FieldReference;
        if(field!=null) return Obj("field",TypeId(field.DeclaringType)+"::"+field.Name+":"+TypeId(field.FieldType));
        var type=o as TypeReference;
        if(type!=null) return Obj("type",TypeId(type));
        var variable=o as VariableDefinition;
        if(variable!=null) return Obj("local",variable.Index);
        var parameter=o as ParameterDefinition;
        if(parameter!=null) return Obj("parameter",parameter.Index);
        var site=o as CallSite;
        if(site!=null) return Obj("callsite",site.FullName,"this",site.HasThis,"explicit_this",site.ExplicitThis,"cc",site.CallingConvention.ToString());
        if(o is float) return Obj("float_bits",BitConverter.ToString(BitConverter.GetBytes((float)o)));
        if(o is double) return Obj("double_bits",BitConverter.ToString(BitConverter.GetBytes((double)o)));
        if(o is string || o is int || o is long || o is byte || o is sbyte) return o;
        throw new InvalidDataException("Unsupported operand: "+o.GetType().FullName);
    }
    static object[] DirectReferences(MethodDefinition method) {
        if(!method.HasBody) return new object[0];
        var result=new List<object>();
        foreach(var i in method.Body.Instructions) {
            var m=i.Operand as MethodReference;
            var f=i.Operand as FieldReference;
            var t=i.Operand as TypeReference;
            if(m!=null) {
                MethodReference identity=m;
                if(Scope(m.DeclaringType.GetElementType())==method.Module.Assembly.Name.FullName) {
                    var definition=m.Resolve();
                    if(definition==null) throw new InvalidDataException("Unresolved local generated-method reference");
                    identity=definition;
                }
                result.Add(Obj("offset",i.Offset,"opcode",i.OpCode.Name,"kind","method",
                    "id",StableId(identity),"declaring_type",m.DeclaringType.GetElementType().FullName));
            }
            if(f!=null) result.Add(Obj("offset",i.Offset,"opcode",i.OpCode.Name,"kind","field",
                "id",f.DeclaringType.GetElementType().FullName+"::"+f.Name,
                "declaring_type",f.DeclaringType.GetElementType().FullName));
            if(t!=null) result.Add(Obj("offset",i.Offset,"opcode",i.OpCode.Name,"kind","type",
                "id",t.GetElementType().FullName,"declaring_type",t.GetElementType().FullName));
        }
        return result.ToArray();
    }
    static object[] CustomAttributes(ICustomAttributeProvider provider) {
        return provider.CustomAttributes.Select(a=>(object)Obj("type",TypeId(a.AttributeType),
            "blob",BitConverter.ToString(a.GetBlob()))).ToArray();
    }
    static object TypeShape(TypeDefinition t) {
        return Obj("name",t.FullName,"attributes",(int)t.Attributes,
            "base_type",t.BaseType==null?null:TypeId(t.BaseType),
            "interfaces",t.Interfaces.Select(TypeId).ToArray(),"custom_attributes",CustomAttributes(t),
            "packing_size",t.PackingSize,"class_size",t.ClassSize,
            "generic_parameters",t.GenericParameters.Select(g=>(object)Obj("name",g.Name,
                "attributes",(int)g.Attributes,"constraints",g.Constraints.Select(TypeId).ToArray())).ToArray(),
            "fields",t.Fields.Select(f=>(object)Obj("id",t.FullName+"::"+f.Name,"name",f.Name,
                "type",TypeId(f.FieldType),"attributes",(int)f.Attributes,
                "has_constant",f.HasConstant,"constant",f.HasConstant?f.Constant:null,
                "offset",f.Offset,"initial_value",BitConverter.ToString(f.InitialValue),
                "custom_attributes",CustomAttributes(f))).ToArray(),
            "methods",t.Methods.Select(StableId).ToArray(),
            "properties",t.Properties.Select(p=>(object)Obj("name",p.Name,"type",TypeId(p.PropertyType),
                "attributes",(int)p.Attributes,"get",p.GetMethod==null?null:StableId(p.GetMethod),
                "set",p.SetMethod==null?null:StableId(p.SetMethod),"custom_attributes",CustomAttributes(p))).ToArray(),
            "events",t.Events.Select(e=>(object)Obj("name",e.Name,"type",TypeId(e.EventType),
                "attributes",(int)e.Attributes,"add",e.AddMethod==null?null:StableId(e.AddMethod),
                "remove",e.RemoveMethod==null?null:StableId(e.RemoveMethod),"custom_attributes",CustomAttributes(e))).ToArray(),
            "nested_types",t.NestedTypes.Select(n=>n.FullName).ToArray());
    }
    static int Offset(byte[] data,uint rva) {
        int pe=BitConverter.ToInt32(data,60), count=BitConverter.ToUInt16(data,pe+6);
        int first=pe+24+BitConverter.ToUInt16(data,pe+20);
        for(int i=0;i<count;i++) {
            int s=first+i*40; uint va=BitConverter.ToUInt32(data,s+12);
            uint size=Math.Max(BitConverter.ToUInt32(data,s+8),BitConverter.ToUInt32(data,s+16));
            if(rva>=va && rva-va<size) return checked((int)(BitConverter.ToUInt32(data,s+20)+rva-va));
        }
        throw new InvalidDataException("Unmapped RVA");
    }
    static object Dump(byte[] data,MethodDefinition m) {
        string key=m.DeclaringType.FullName+"::"+m.Name+"("+string.Join(",",m.Parameters.Select(p=>p.ParameterType.FullName).ToArray())+")";
        if(!m.HasBody) return Obj("key",key,"stable_id",StableId(m),"token",m.MetadataToken.ToString(),"body",null,
            "direct_references",DirectReferences(m),
            "signature",MethodId(m),"method_attributes",(int)m.Attributes,"impl_attributes",(int)m.ImplAttributes);
        var b=m.Body; int start=Offset(data,(uint)m.RVA); int first=data[start];
        int header=(first&3)==2?1:(BitConverter.ToUInt16(data,start)>>12)*4;
        int flags=(first&3)==2?2:BitConverter.ToUInt16(data,start)&0xfff;
        var instructions=b.Instructions.Select(i=>(object)Obj("offset",i.Offset,"opcode",i.OpCode.Name,"operand",Operand(i.Operand))).ToArray();
        var metadataOperands=new List<object>();
        foreach(var instruction in b.Instructions) {
            switch(instruction.OpCode.OperandType) {
                case OperandType.InlineField: case OperandType.InlineMethod:
                case OperandType.InlineType: case OperandType.InlineTok:
                case OperandType.InlineString: case OperandType.InlineSig:
                    metadataOperands.Add(Obj("body_offset",header+instruction.Offset+instruction.OpCode.Size,
                        "width",4,"identity",Operand(instruction.Operand)));
                    break;
            }
        }
        if(header>=12) metadataOperands.Add(Obj("body_offset",8,"width",4,"identity",
            Obj("locals_signature_present",BitConverter.ToUInt32(data,start+8)!=0,
                "variables",b.Variables.Select(v=>TypeId(v.VariableType)).ToArray())));
        var clauses=b.ExceptionHandlers.Select(e=>(object)Obj("kind",e.HandlerType.ToString(),
            "try_start",e.TryStart.Offset,"try_end",e.TryEnd==null?b.CodeSize:e.TryEnd.Offset,
            "handler_start",e.HandlerStart.Offset,"handler_end",e.HandlerEnd==null?b.CodeSize:e.HandlerEnd.Offset,
            "filter_start",e.FilterStart==null?-1:e.FilterStart.Offset,"catch_type",e.CatchType==null?null:TypeId(e.CatchType))).ToArray();
        // Header/exception section formats and padding are retained separately
        // so semantic instruction equality cannot hide a layout discrepancy.
        int end=start+header+b.CodeSize;
        var sections=new List<object>();
        if((flags&8)!=0) {
            int aligned=(end+3)&~3;
            string padding=BitConverter.ToString(data,end,aligned-end); end=aligned;
            int clauseIndex=0;
            while(true) {
                int kind=data[end]; bool fat=(kind&0x40)!=0;
                int size=fat?data[end+1]|data[end+2]<<8|data[end+3]<<16:data[end+1];
                sections.Add(Obj("kind",kind,"size",size,"padding_before",padding));
                if((kind&0x3f)!=1) throw new InvalidDataException("Unsupported method extra section");
                int clauseSize=fat?24:12;
                if((size-4)%clauseSize!=0) throw new InvalidDataException("Invalid EH section size");
                for(int pos=end+4;pos<end+size;pos+=clauseSize) {
                    int clauseFlags=fat?BitConverter.ToInt32(data,pos):BitConverter.ToUInt16(data,pos);
                    if(clauseIndex>=b.ExceptionHandlers.Count) throw new InvalidDataException("EH clause count mismatch");
                    var clause=b.ExceptionHandlers[clauseIndex++];
                    if(clauseFlags==0) metadataOperands.Add(Obj("body_offset",pos-start+(fat?20:8),"width",4,
                        "identity",Obj("catch_type",clause.CatchType==null?null:TypeId(clause.CatchType))));
                }
                end+=size;
                if((kind&0x80)==0) break;
                aligned=(end+3)&~3; padding=BitConverter.ToString(data,end,aligned-end);end=aligned;
            }
        }
        return Obj("key",key,"stable_id",StableId(m),"token",m.MetadataToken.ToString(),"signature",MethodId(m),
            "direct_references",DirectReferences(m),
            "header",Obj("format",(first&3)==2?"tiny":"fat","size",header,"flags",flags,
                "maxstack",b.MaxStackSize,"initlocals",b.InitLocals,"code_size",b.CodeSize),
            "locals",b.Variables.Select(v=>TypeId(v.VariableType)).ToArray(),"instructions",instructions,
            "exception_regions",clauses,"extra_sections",sections,
            "metadata_operands",metadataOperands,
            "raw_body_hex",BitConverter.ToString(data,start,end-start),
            "method_attributes",(int)m.Attributes,"impl_attributes",(int)m.ImplAttributes);
    }
    static IEnumerable<TypeDefinition> Types(IEnumerable<TypeDefinition> roots) {
        foreach(var type in roots) { yield return type; foreach(var nested in Types(type.NestedTypes)) yield return nested; }
    }
    public static int Main(string[] args) {
        Console.OutputEncoding=new UTF8Encoding(false);
        if(args.Length<2) { Console.Error.WriteLine("Usage: ProbeInspector assembly token-or-Type::Method [...]");return 2; }
        try {
            var data=File.ReadAllBytes(args[0]);
            var module=ModuleDefinition.ReadModule(args[0]);
            {
                var selectors=new HashSet<string>(args.Skip(1)); var methods=new List<object>();
                foreach(var type in Types(module.Types)) foreach(var method in type.Methods) {
                    string token="0x"+method.MetadataToken.ToUInt32().ToString("x8");
                    if(selectors.Contains("--all")||selectors.Contains("--structure")||selectors.Contains(token)||selectors.Contains(type.FullName+"::"+method.Name)) methods.Add(Dump(data,method));
                }
                var attributes=module.Assembly.CustomAttributes.Where(a=>a.AttributeType.FullName=="System.Diagnostics.DebuggableAttribute")
                    .Select(a=>BitConverter.ToString(a.GetBlob())).ToArray();
                var result=Obj("assembly",module.Assembly.Name.FullName,"mvid",module.Mvid.ToString(),
                    "debuggable_blobs",attributes,"selected_methods",methods,
                    "references",module.AssemblyReferences.Select(a=>a.FullName).ToArray());
                if(selectors.Contains("--structure")) result["types"]=Types(module.Types).Select(TypeShape).ToArray();
                Console.WriteLine(new JavaScriptSerializer { MaxJsonLength=128000000 }.Serialize(result));
            }
            return 0;
        } catch(Exception e) { Console.Error.WriteLine(e.ToString());return 1; }
    }
}
