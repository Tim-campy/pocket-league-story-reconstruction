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
        Vector[] array = new Vector[2]; array[0] = new Vector(); array[1] = new Vector();
        return array[0].value*10+array[1].value;
    }
    public static int InverseChain()
    {
        int local;
        local = Read(); sink = local;
        return local+sink;
    }
    public static void MapInts(int[] array)
    {
        COAT_CONNER[0][0] = (0 - array[0]);
        COAT_CONNER[0][1] = (0 - array[1]);
        COAT_CONNER[1][1] = (0 - (MAPCHIP_COAT_WNUM * 30 / 2)) - array[1];
        COAT_GOAL1[0][1] = (0 - (MAPCHIP_COAT_WNUM * 30 / 2 / 4)) - array[1];
        COAT_GOAL1[1][1] = (0 - (MAPCHIP_COAT_WNUM * 3 * 30 / 2 / 4)) - array[1];
        COAT_GOAL1_IN[0][1] = (0 - (MAPCHIP_COAT_WNUM * 15 * 3 / 8)) - array[1];
        COAT_GOAL1_IN[1][1] = (0 - (MAPCHIP_COAT_WNUM * 15 * 5 / 8)) - array[1];
        GOAL1[0][1] = (0 - (MAPCHIP_COAT_WNUM * 30 / 2 / 2)) - array[1];
    }
    public static int LtFloat(float a,float b)
    {
        if ((b > a)) return 11; return 19;
    }
    public static int NotLtFloat(float a,float b)
    {
        if (!(b > a)) return 11; return 19;
    }
    public static int LeDouble(double a,double b)
    {
        if ((b >= a)) return 11; return 19;
    }
    public static int GeDouble(double a,double b)
    {
        if ((b <= a)) return 11; return 19;
    }
    public static int GtInt(int a,int b)
    {
        if ((b < a)) return 11; return 19;
    }
    public static int EqFloat(float a,float b)
    {
        if ((b == a)) return 11; return 19;
    }
    public static int NeDouble(double a,double b)
    {
        if ((b != a)) return 11; return 19;
    }
    public static int WrongNegatedFloatComplement(float a,float b)
    {
        if (b >= a) return 11; return 19;
    }
    public static float FloatNeg(float a){return -a;}
    public static float FloatZeroSub(float a){return 0f-a;}
    public static int SemanticChecks()
    {
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
