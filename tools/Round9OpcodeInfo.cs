using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Web.Script.Serialization;
public static class DumpOpcodeInfo {
    public static void Main() {
        var rows=new List<object>();
        foreach(var field in typeof(OpCodes).GetFields(BindingFlags.Public|BindingFlags.Static)) {
            if(field.FieldType!=typeof(OpCode))continue;
            var op=(OpCode)field.GetValue(null);
            rows.Add(new {name=op.Name,size=op.Size,operand_type=op.OperandType.ToString(),flow=op.FlowControl.ToString(),
                pop=op.StackBehaviourPop.ToString(),push=op.StackBehaviourPush.ToString()});
        }
        Console.WriteLine(new JavaScriptSerializer().Serialize(rows));
    }
}
