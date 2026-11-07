using System;
using System.Reflection;
public static class Runner
{
    public static int Main(string[] args)
    {
        Assembly assembly=Assembly.LoadFrom(args[0]);
        Type fixture=assembly.GetType("Fixture",true);
        int semantic=(int)fixture.GetMethod("SemanticChecks").Invoke(null,null);
        float a=(float)fixture.GetMethod("FloatNeg").Invoke(null,new object[]{0f});
        float b=(float)fixture.GetMethod("FloatZeroSub").Invoke(null,new object[]{0f});
        Console.WriteLine("{\"semantic\":"+semantic+",\"neg_positive_zero_bits\":"+
            BitConverter.ToInt32(BitConverter.GetBytes(a),0)+",\"zero_sub_positive_zero_bits\":"+
            BitConverter.ToInt32(BitConverter.GetBytes(b),0)+"}");
        return 0;
    }
}
