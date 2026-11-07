// Independent own mold. None of these bodies are copied from the game.
public class FanOwn
{
    static int tick;
    static void Leaf(int n){tick+=n;}
    static void SetBPFData(Bonus b,string message,int[] param){tick++;}
    public static void Run()
{
    UserData instance2=new UserData();Vector vector2=new Vector();
    for(int i=0;i<5;i++)
    {
        int num=i;
        if((num==0)){Leaf(0);}
        else if((num==13)){Leaf(13);}
        else if((num==1)){Leaf(1);}
        else if((num==2))
        {{if(instance2.gm_.score_[0]>instance2.gm_.score_[1]){int inside=4;Leaf(inside);}}}
        else if((num==15)||(num==16))
        {{if(instance2.gm_.score_[0]>instance2.gm_.score_[1]){int inside2=5;Leaf(inside2);}}}
        else if((num==3))
        {int recoveredMatchInfoJoin=instance2.GetMatchInfoJoin();if(recoveredMatchInfoJoin==0 && LeagueData.IsVictoryLeague()){Leaf(3);}}
        else if((num==14))
        {{if(LeagueData.IsLastMatch()){int inside3=6;Leaf(inside3);}}}
        else if((num==4)){Leaf(4);}
        else if((num==5))
        {{
            bool flag=false;
            for(int num7=0;num7<2;num7++)
            {
                int num8=num7+1;flag=num8==2;
                if(flag){continue;}vector2.AddElement(new int[2]{num8,1});
            }
        }}
        else if((num==6)){Leaf(6);}
        else if((num==7)){Leaf(7);}
        else if((num==8)){Leaf(8);}
        else if((num==10)){}
        else if((num==11))
        {{if(instance2.gm_.score_[0]>instance2.gm_.score_[1]){int inside4=7;Leaf(inside4);}}}
        else if((num==12))
        {{
            for(int j=0;j<3;j++)
            {
                Bonus fanBonusData4=new Bonus();
                if(fanBonusData4.state_==1){continue;}
                if(instance2.customer_total_>=fanBonusData4.termModeValue_*10000)
                {SetBPFData(fanBonusData4,fanBonusData4.message_,fanBonusData4.param);break;}
            }
        }}
    }
}
}
public class SoccerOwn
{
    int state_,stateCnt_,select_,value;
    AppData apdat_=new AppData();UserData udat_=new UserData();
    int[] PBONUS_NP_LV_TIME=new int[4],PBONUS_NP_LV_POPULAR=new int[4],PBONUS_NP_LV_TRAINING1=new int[4];
    int bonus_softKey;
    void ChangeState(int n){value+=n;}
    void Leaf(int n){value+=n;}
    public bool Run()
{
    if(state_==2)
    {
        int matchInfoJoin=udat_.GetMatchInfoJoin();
        if((matchInfoJoin==0)){Leaf(0);}else if((matchInfoJoin==1)){{int x=3;Leaf(x);}}
    }
    else if(state_==4)
    {
        apdat_.camera.SetMapMainBasePos();
        int recoveredMatchInfoJoin4=udat_.GetMatchInfoJoin();
        if(recoveredMatchInfoJoin4==0){Leaf(4);}else{Leaf(5);}
    }
    else if(state_==9)
    {
        for(int j=0;j<3;j++)
        {
            int num3=j;int num4=value;
            if(num4==1)
            {
                if((num3==0)){{int a=1;Leaf(a);}}
                else if((num3==1)){{int b=2;Leaf(b);}}
            }
        }
    }
    else if(state_==10)
    {
        for(int n=0;n<6;n++)
        {
            if((n==0)||(n==1)||(n==2)){Leaf(0);}
            else if((n==3)){Leaf(3);}else if((n==4)){Leaf(4);}else{Leaf(5);}
        }
    }
    else if(state_==11)
    {
        ChangeState(12);int recoveredMatchInfoJoin11=udat_.GetMatchInfoJoin();
        if(recoveredMatchInfoJoin11==0){ChangeState(12);}else{ChangeState(4);}
    }
    else if(state_==14)
    {
        if(value==0)
        {
            if(stateCnt_<PBONUS_NP_LV_TIME[3]){stateCnt_=2;}
            else if(stateCnt_>=PBONUS_NP_LV_TIME[3] && select_==0){ChangeState(16);}else{}
        }
        else if(value==6){bonus_softKey=1-bonus_softKey;}
    }
    else if(state_==15)
    {
        if(value==0)
        {
            if(stateCnt_<PBONUS_NP_LV_POPULAR[3]){stateCnt_=3;}
            else if(select_==0){for(int k=0;k<2;k++){Leaf(k);}ChangeState(16);}else{}
        }
        else if(value==6){bonus_softKey=1-bonus_softKey;}
    }
    else if(state_==16)
    {
        if(value==0)
        {
            if(stateCnt_<PBONUS_NP_LV_TRAINING1[1]){stateCnt_=4;}
            else if(select_==0)
            {
                for(int num12=0;num12<11;num12++)
                {
                    PlayerData playerData9=new PlayerData();
                    if(playerData9.bonus_research[0]==-1){continue;}
                    udat_.AddResearchPoint(playerData9.bonus_research[0],playerData9.bonus_research[1]);
                }
                ChangeState(10);
            }else{}
        }
        else if(value==6){bonus_softKey=1-bonus_softKey;}
    }
    return false;
}
}
public class UserData{public Manager gm_=new Manager();public int customer_total_;public int GetMatchInfoJoin(){return customer_total_;}public void AddResearchPoint(int a,int b){customer_total_+=a+b;}}
public class Manager{public int[] score_=new int[2];}
public class AppData{public Camera camera=new Camera();}
public class Camera{public int value;public void SetMapMainBasePos(){value++;}}
public static class LeagueData{public static bool IsVictoryLeague(){return true;}public static bool IsLastMatch(){return false;}}
public class Vector{public int count;public void AddElement(object item){count++;}}
public class Bonus{public int state_,termModeValue_;public string message_;public int[] param;}
public class PlayerData{public int[] bonus_research=new int[2];}
