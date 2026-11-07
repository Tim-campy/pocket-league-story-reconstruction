// Own independent compiler fixture; target verification remains separate.
public class Draw4Fixture
{
    int type_, training_cnt, udArrowCnt;
    static int[] TRA_ANIME_TIME2 = { 0, 25, 50 }, TRA_AT2 = { 1 }, TRA_AT = { 0, 25 };
    App apdat_ = new App();
    Formation formationData = new Formation();
    static void _draw5(Graphics g) { g.Mark(5); }
    static void Draw_upParam(Graphics g, int a, int b, int c, int d, int e) { g.Mark(a+b+c+d+e); }

    public void Run(Graphics g)
    {
        int num4=0, num=0, num2=0, num3=0;
        if(type_==48){ return; }
        if(type_==49)
        {
            int[] array = { 8, 11, 13, 9, 11 };
            int num15 = 0;
            for(int n=0;n<TrainingData.TRA_PLAYER.Length;n++)
            {
                num2 = n + 2;
                if(training_cnt<TRA_ANIME_TIME2[0]||training_cnt>=TRA_ANIME_TIME2[1]) { continue; }
                int num19=training_cnt-TRA_ANIME_TIME2[0];
                int num20=num19/25;
                int num21=num19%25;
                int num22=num2-13;
                int num23=87;
                if(num21>=TRA_AT2[0])
                {
                    int num24=array[num20];
                    g.Mark(num24);
                }
                num15=n;
                int num25=GameUtil.RateConvert(num21,TRA_AT[0],TRA_AT[1],12,0,true);
                Draw_upParam(g,78,num22+6,num23+15+num25,num15,1);
                g.PopClip();
            }
            for(int num28=0;num28<5;num28++)
            {
                for(int num31=0;num31<TrainingData.TRA_PLAYER.Length;num31++)
                {
                    g.Mark(num31);
                    if(TrainingData.TRA_PLAYER[num31]!=-1)
                    {
                        PlayerData playerData4=apdat_.pData_[TrainingData.TRA_PLAYER[num31]];
                        g.Mark(playerData4.value);
                        switch(num28)
                        {
                            case 0:g.SetColor(Graphics.GetColorOfRGB(252,54,0));break;
                            case 1:g.SetColor(Graphics.GetColorOfRGB(84,188,255));break;
                            case 2:g.SetColor(Graphics.GetColorOfRGB(103,255,121));break;
                            case 3:g.SetColor(Graphics.GetColorOfRGB(248,248,248));break;
                            case 4:g.SetColor(Graphics.GetColorOfRGB(255,251,0));break;
                        }
                        g.Mark(num28);
                    }
                }
            }
        }
        else if(type_==50)
        {
            int num35=75;
            int num36=34;
            g.Mark(num35+num36);
            int num37=0;
            for(int num38=0;num38<formationData.formation_.Length;num38++)
            {
                int num39=GameUtil.RateConvert(formationData.formation_[num38][1],0,8,num2+num35-10,num2+10,true);
                num37=GameUtil.RateConvert(formationData.formation_[num38][0],0,8,num3+1+1,num3+num36-1-2,true);
                if(num38==0){num39=num2+3;}
                g.FillRect(num39-1,num37-1,3f,3f);
            }
        }
        else if(type_==51)
        {
            num=((!MyFormBase.Jp())?51:50);
            g.Mark(num);
        }
        else if(type_==52)
        {
            for(int num47=0;num47<5;num47++)
            {
                switch(num47)
                {
                    case 0:g.SetColor(Graphics.GetColorOfRGB(252,54,0));break;
                    case 1:g.SetColor(Graphics.GetColorOfRGB(84,188,255));break;
                    case 2:g.SetColor(Graphics.GetColorOfRGB(103,255,121));break;
                    case 3:g.SetColor(Graphics.GetColorOfRGB(248,248,248));break;
                    case 4:g.SetColor(Graphics.GetColorOfRGB(255,251,0));break;
                }
                g.Mark(num47);
            }
            if(udArrowCnt==0)
            {
                GameView.AddTouch(g,2,num4-25-5,num-50,70,30,1376456,TouchOption.Create(2));
                GameView.AddTouch(g,2,num4-25-5,num+100,70,30,1376456,TouchOption.Create(2));
                return;
            }
            TouchOption touchOption=TouchOption.Create(2);
            TouchOption touchOption2=TouchOption.Create(2);
            GameView.AddTouch(g,24,num4,num-13,apdat_.resCom2_.seb[24],0,17,touchOption.Margin(40,40,45,0));
            GameView.AddTouch(g,24,num4,num+64+13,apdat_.resCom2_.seb[24],1,19,touchOption2.Margin(40,40,-15,60));
        }
        else { _draw5(g); }
    }
}
public class Graphics {
    public int value;
    public void Mark(int x){value=value*31+x;}
    public void PopClip(){Mark(31);}
    public void SetColor(int x){Mark(x);}
    public void FillRect(float x,float y,float w,float h){Mark((int)(x+y+w+h));}
    public static int GetColorOfRGB(int r,int g,int b){return r*65536+g*256+b;}
}
public class App { public PlayerData[] pData_=new PlayerData[16]; public Resources resCom2_=new Resources(); }
public class PlayerData { public int value; }
public class Resources { public object[] seb=new object[25]; }
public class Formation { public int[][] formation_=new int[][]{new int[]{1,2},new int[]{3,4}}; }
public static class TrainingData { public static int[] TRA_PLAYER={1,-1,2}; }
public static class GameUtil { public static int RateConvert(int x,int a,int b,int c,int d,bool f){return x+a+b+c+d;} }
public static class MyFormBase { public static bool Jp(){return System.Environment.TickCount==0;} }
public class TouchOption {
    public static TouchOption Create(int x){return new TouchOption();}
    public TouchOption Margin(int a,int b,int c,int d){return this;}
}
public static class GameView {
    public static void AddTouch(Graphics g,int id,int x,int y,int w,int h,int bits,TouchOption option){g.Mark(id+x+y+w+h+bits);}
    public static void AddTouch(Graphics g,int id,int x,int y,object seb,int a,int b,TouchOption option){g.Mark(id+x+y+a+b);}
}
