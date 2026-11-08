using System;
public static class Fixture
{
    public static int sink,readCount;
    public static int[][] COAT_CONNER=new int[][]{new int[2],new int[2]};
    public static int[][] COAT_GOAL1=new int[][]{new int[2],new int[2]};
    public static int[][] COAT_GOAL1_IN=new int[][]{new int[2],new int[2]};
    public static int[][] GOAL1=new int[][]{new int[2]};
    public static int MAPCHIP_COAT_WNUM;
    static int Read(){readCount++;return readCount*7;}
    public static int ArrayLocal()
    {
        Vector[] array = new Vector[2] { new Vector(), new Vector() };
        return array[0].value*10+array[1].value;
    }
    public static int InverseChain()
    {
        int local;
        sink = local = Read();
        return local+sink;
    }
    public static void MapInts(int[] array)
    {
        COAT_CONNER[0][0] = -array[0];
        COAT_CONNER[0][1] = -array[1];
        COAT_CONNER[1][1] = -(MAPCHIP_COAT_WNUM * 30 / 2) - array[1];
        COAT_GOAL1[0][1] = -(MAPCHIP_COAT_WNUM * 30 / 2 / 4) - array[1];
        COAT_GOAL1[1][1] = -(MAPCHIP_COAT_WNUM * 3 * 30 / 2 / 4) - array[1];
        COAT_GOAL1_IN[0][1] = -(MAPCHIP_COAT_WNUM * 15 * 3 / 8) - array[1];
        COAT_GOAL1_IN[1][1] = -(MAPCHIP_COAT_WNUM * 15 * 5 / 8) - array[1];
        GOAL1[0][1] = -(MAPCHIP_COAT_WNUM * 30 / 2 / 2) - array[1];
    }
    public static int LtFloat(float a,float b)
    {
        if (a < b) return 11; return 19;
    }
    public static int NotLtFloat(float a,float b)
    {
        if (!(a < b)) return 11; return 19;
    }
    public static int LeDouble(double a,double b)
    {
        if (a <= b) return 11; return 19;
    }
    public static int GeDouble(double a,double b)
    {
        if (a >= b) return 11; return 19;
    }
    public static int GtInt(int a,int b)
    {
        if (a > b) return 11; return 19;
    }
    public static int EqFloat(float a,float b)
    {
        if (a == b) return 11; return 19;
    }
    public static int NeDouble(double a,double b)
    {
        if (a != b) return 11; return 19;
    }
    public static int WrongNegatedFloatComplement(float a,float b)
    {
        if (b >= a) return 11; return 19;
    }
    public static float FloatNeg(float a){return -a;}
    public static float FloatZeroSub(float a){return 0f-a;}
    public static int field;
    public static int Field(int b){return field < b ? 1 : 0;}
    public static int Ref(ref int a,int b){return a < b ? 1 : 0;}
    public static int Mixed(int a,float b){return a < b ? 1 : 0;}
    public static int Call(int a){return a < Read() ? 1 : 0;}
public static int ZeroInt(int a){return 0 > a ? 1 : 0;}
public static int NegatedFloat(float a){return !(0f > a) ? 1 : 0;}
public static int NegativeZeroDouble(double a){return -0d >= a ? 1 : 0;}
public static int ConstantFirstFloat(float a){return a > 0f ? 1 : 0;}
public static int ZeroUInt(uint a){return 0u == a ? 1 : 0;}
public static int ZeroLong(long a){return 0L <= a ? 1 : 0;}
public static int ZeroDouble(double a){return 0d != a ? 1 : 0;}
public static int ConvertedFloat(float a){return 0 > a ? 1 : 0;}
public static int BoolLiteral(bool a){return a == true ? 1 : 0;}
public static int ConstantOnly(){return 0 < 1 ? 1 : 0;}

    public static int SemanticChecks()
    {
if(NegatedFloat(float.NaN)!=1 || NegativeZeroDouble(double.NaN)!=0 || ZeroDouble(double.NaN)!=1 || ConstantFirstFloat(float.PositiveInfinity)!=1 || ConvertedFloat(float.NegativeInfinity)!=1 || ZeroDouble(-0d)!=0)return 90;
        Vector.count=0;Vector.trace=0;readCount=0;sink=0;
        if(ArrayLocal()!=12 || Vector.count!=2 || Vector.trace!=12)return 1;
        if(InverseChain()!=14 || readCount!=1 || sink!=7)return 2;
        float minusZero=BitConverter.ToSingle(new byte[]{0,0,0,128},0);
        float[] single=new float[]{float.NaN,float.PositiveInfinity,float.NegativeInfinity,0f,minusZero,-1f,1f};
        double[] twice=new double[]{double.NaN,double.PositiveInfinity,double.NegativeInfinity,0d,-0d,-1d,1d};
        for(int i=0;i<single.Length;i++)for(int j=0;j<single.Length;j++)
        {
            float a=single[i],b=single[j];
            if(LtFloat(a,b)!=(a<b?11:19))return 3;
            if(NotLtFloat(a,b)!=(!(a<b)?11:19))return 4;
            if(EqFloat(a,b)!=(a==b?11:19))return 5;
        }
        for(int i=0;i<twice.Length;i++)for(int j=0;j<twice.Length;j++)
        {
            double a=twice[i],b=twice[j];
            if(LeDouble(a,b)!=(a<=b?11:19))return 6;
            if(GeDouble(a,b)!=(a>=b?11:19))return 7;
            if(NeDouble(a,b)!=(a!=b?11:19))return 8;
        }
        int[] ints=new int[]{int.MinValue,-1,0,1,int.MaxValue};
        for(int i=0;i<ints.Length;i++)for(int j=0;j<ints.Length;j++)if(GtInt(ints[i],ints[j])!=(ints[i]>ints[j]?11:19))return 9;
        for(int k=0;k<ints.Length;k++)
        {
            MAPCHIP_COAT_WNUM=ints[k];int[] a=new int[]{ints[k],17};MapInts(a);
            unchecked
            {
                if(COAT_CONNER[0][0]!=(0-a[0]) || COAT_CONNER[0][1]!=(0-a[1]))return 10;
                if(COAT_CONNER[1][1]!=(0-(MAPCHIP_COAT_WNUM*30/2))-a[1])return 11;
            }
        }
        if(NotLtFloat(float.NaN,0f)==WrongNegatedFloatComplement(float.NaN,0f))return 12;
        return 0;
    }
}
public class Vector
{
    public static int count,trace;
    public int value;
    public Vector(){value=++count;trace=trace*10+value;}
}
