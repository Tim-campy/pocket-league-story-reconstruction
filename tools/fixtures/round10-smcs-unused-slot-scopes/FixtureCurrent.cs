// OWN FIXTURES/STUBS WRITTEN FROM ZERO. No game source is compiled.
public class MatchFixture
{
    public static data.MapchipMatch[] mapchipGrounds=new data.MapchipMatch[3];
    public static int MAP_GROUND,MAPCHIP_FIELD_HNUM=2,MAPCHIP_FIELD_WNUM=3;
    public int sum;
    void Leaf(int n){sum=sum*31+n;}
    public void Run()
    {
        bool flag=false;
        int num=0;
        global::System.Int32 originalUnusedSlot2;
        int num2;
        bool flag2=false;
        global::data.MapchipMatch mapchipMatch=mapchipGrounds[MAP_GROUND];
        num2=0;
        int num3=0;
        for(int num4=MAPCHIP_FIELD_HNUM-1;num4>=0;num4--)
        {
            for(int i=0;i<MAPCHIP_FIELD_WNUM;i++)
            {
                num2++;
                data.MapchipMatch mapchipMatch2=mapchipGrounds[i];
                if(mapchipMatch2.flag_!=0){mapchipMatch2=mapchipMatch;}
                Image image=mapchipMatch2.image;
                int height=image.GetHeight();
                if(height>=30){continue;}
                int width=image.GetWidth();
                flag=width>height;
                if(flag){num3++;Leaf(width+height+num4+i);}
            }
        }
    }
}
public class MainFixture
{
    public static data.MapchipMain[] mapchipGrounds=new data.MapchipMain[3];
    public static int MAP_GROUND,MAPCHIP_FIELD_HNUM=2,MAPCHIP_FIELD_WNUM=3;
    public int sum;
    void Leaf(int n){sum=sum*31+n;}
    public void Run()
    {
        bool flag=false;
        int num=0;
        global::System.Int32 originalUnusedSlot2;
        int num2;
        bool flag2=false;
        global::data.MapchipMain mapchipMain=null;
        mapchipMain=((MAP_GROUND!=0)?mapchipGrounds[MAP_GROUND-1]:null);
        num2=0;
        int num3=0;
        for(int num4=MAPCHIP_FIELD_HNUM-1;num4>=0;num4--)
        {
            for(int i=0;i<MAPCHIP_FIELD_WNUM;i++)
            {
                num2++;
                data.MapchipMain mapchipMain2=mapchipGrounds[i];
                if(mapchipMain!=null&&mapchipMain2.flag_!=0){mapchipMain2=mapchipMain;}
                Image image=mapchipMain2.image;
                int height=image.GetHeight();
                if(height>=30){continue;}
                int width=image.GetWidth();
                flag=width>height;
                if(flag){num3++;Leaf(width+height+num4+i);}
            }
        }
    }
}
public class DefaultFixture
{
    public static int MAPCHIP_COAT_HNUM=3,MAPCHIP_COAT_WNUM=3;
    public static int[][] COAT_GOAL2_ADDRESS={new int[]{1,2},new int[]{0,1}};
    public int sum;
    void Leaf(float f){sum=sum*31+(int)f;}
    public void Run()
    {
        Leaf(0);
        float num=100f;
        int num5;
        float num2=1000f;
        global::System.Single originalUnusedSlot1;
        for(int i=0;i<MAPCHIP_COAT_HNUM;i++)
        {
            float num3=num;
            float num4=num2;
            num5=COAT_GOAL2_ADDRESS[1][1]-i;
            if(num5>0){for(int j=0;j<num5;j++){num3*=0.8f;num4*=0.1f;}}
            for(int k=0;k<MAPCHIP_COAT_WNUM;k++){Leaf(num3+num4+k);}
        }
        for(int l=0;l<2;l++)
        {
            float num3=num;
            float num4=num2;
            for(int num6=0;num6<2;num6++){num3*=0.8f;num4*=0.1f;Leaf(num3+num4);}
            num3=num;num4=num2;
            for(int m=0;m<2;m++){num3*=0.8f;num4*=0.1f;Leaf(num3+num4);}
        }
        for(int num7=1;num7>=0;num7--)
        {
            for(int num8=0;num8<2;num8++)
            {
                float num3=num7+num8;
                if(num3>2f){num3=2f;}
                float num4=num7-num8;
                if(num3>num4){num4=num3;}
                Leaf(num3+num4);
            }
            for(int n=0;n<2;n++)
            {
                float num3=num7+n;
                if(num3>2f){num3=2f;}
                float num4=num7-n;
                if(num4>num3){num4=num3;}
                Leaf(num3+num4);
            }
        }
        for(int num9=4;num9<=6;num9++){Leaf(num9);}
    }
}
public class Image
{
    public int width=10,height=20;
    public int GetWidth(){return width;}
    public int GetHeight(){return height;}
}
namespace data
{
    // Minimal fixture stubs; unrelated to actual Assembly-CSharp definitions.
    public class MapchipMatch{public int flag_;public Image image=new Image();}
    public class MapchipMain{public int flag_;public Image image=new Image();}
}
