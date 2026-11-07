// Independent emission molds. These are not game classes or game builds.
public class StoreMolds
{
    int counter,state_,gameState_,direct_;
    int[] rand_cos=new int[]{10,20,30},temp_direct_s=new int[]{1,2},directBox_=new int[]{1,2,3};
    Vec pos_=new Vec();OwnUser udat_=new OwnUser();
    static Marker instance_=new Marker();
    void Use(float value){counter+=(int)value;}
    void Collision_proc(Player a,Player b){counter++;}
    void Collision_updateCollisionBallState(Player a){counter++;}
    void CollisionBall_proc(){counter++;}
    static float[] GetImageRect(int n){return new float[]{n,n,n,n};}
    public void Shoot(object player,int shoot_mode)
    {
        float[] array=new float[]{1,2};
        int num7=0;int num8=counter;
        if(num8>rand_cos[1]){num7=2;}else if(num8>rand_cos[0]){num7=1;}else{num7=0;}
        float num9=0f;
        if ((num7 == 0)) {
num9=array[0];
}
else if ((num7 == 1)) {
num9=array[1];
}
else {
num9=0f;
}
        Use(num9);
        float num13=counter*2f;
        Use(0f-num13/4f);Use(0f+num13/4f);
        if ((shoot_mode == 0)) {
{int stamp=3;Use(stamp);}
}
else if ((shoot_mode == 1)) {
{float shootV0y=counter*3f;Use((-shootV0y)*350f/100f);}
}
        Use(40);
        if ((shoot_mode == 0)) {
Use(0);
}
else if ((shoot_mode == 1)) {
Use(1);
}
    }
    public void Collision()
    {
        int num=counter%2;
        for(int k=0;k<2;k++)
        {
            int num2=0;
            if(k==0){num2=num;}else{num2=1-num;}
            for(int l=0;l<udat_.teams_[num2].players_.Length;l++)
            {
                Player p=udat_.teams_[num2].players_[l];
                for(int m=0;m<udat_.teams_[0].players_.Length;m++)
                {
                    {if(num2==0 && l==m){continue;} Player p2=udat_.teams_[0].players_[m];Collision_proc(p,p2);}
                }
                for(int n=0;n<udat_.teams_[1].players_.Length;n++)
                {
                    if(num2==1 && l==n){continue;} {Player p3=udat_.teams_[1].players_[n];Collision_proc(p,p3);}
                }
            }
        }
        if(state_==0 && gameState_==2 && BitUtil.Check(udat_.ball_.flag_,8)){
        for(int num3=0;num3<2;num3++)
        {
            for(int num4=0;num4<udat_.teams_[num3].players_.Length;num4++)
            {Collision_updateCollisionBallState(udat_.teams_[num3].players_[num4]);}
        }
        CollisionBall_proc();
    }}
    public void Move(Vec go_pos)
    {
        float num5=counter*0.01f;
        int num6=0;
        if(num5>=GameUtil.Cos(22)){num6=0;}else if(num5>=GameUtil.Cos(67)){if(go_pos.z_>pos_.z_){num6=1;}else{num6=7;}}else if(num5>=GameUtil.Cos(112)){if(go_pos.z_>pos_.z_){num6=2;}else{num6=6;}}else if(num5>=GameUtil.Cos(157)){if(go_pos.z_>pos_.z_){num6=3;}else{num6=5;}}else{num6=4;}
        Use(num6);
        int num7=0;
        if(temp_direct_s[0]>0){if(temp_direct_s[1]>0){num7=0;}else{num7=1;}}else{if(temp_direct_s[1]>0){num7=3;}else{num7=2;}}
        Use(num7);
        int num9=num7;
        for(int i=1;i<directBox_.Length;i++){if(num9!=directBox_[i]){break;}if(i==directBox_.Length-1){direct_=num9;}}
    }
    public void GameParam(bool flag,bool flag2,int num)
    {
        int num2=0;
        if(flag){if(num>=0){num2=num2+GameUtil.RateConvert(num,0,50,5,1,true);}else{num2=num2+GameUtil.RateConvert(num,0,-50,5,30,true);}}else if(flag2){num2=num2+GameUtil.RateConvert(num,-50,50,6,-3,true);}else{num2=num2+GameUtil.RateConvert(num,-50,50,0,-6,true);}
        Use(num2);
    }
    public static Component Touch(Graphics g,int id,int x,int y,object seb,int frame,int value,object opt)
    {
        short[] boundingRect=new short[]{1,2};
        int num=x;int num2=y;int num3=3;int num4=4;
        if(g!=null)
        {
            switch(id){case 1:case 18:case 26:{
                int num5=-1;
                int num6=((frame==0 || frame==3)?3:1);
                if(instance_.CheckTouch(id,value)){num5=((frame==0 || frame==3)?2:3);}else{num5=((frame==0 || frame==3)?0:1);}
                float[] imageRect=GetImageRect(num5);
                num+=(int)imageRect[0]+boundingRect[0];
                if(id==18)
                {
                    switch(num6){case 3:
num-=30;num2-=10;num3+=40;num4+=20;
break;case 1:
num-=10;num2-=10;num3+=40;num4+=20;
break;}
                }
                instance_.DrawImage(g,x,y,num5);
            break;}}
        }
        Component touchComponent=new Component();
        touchComponent.value=num+num2+num3+num4;
        return touchComponent;
    }
}
public class Vec{public float x_,z_;}
public class Player{}
public class OwnTeam{public Player[] players_=new Player[2];}
public class OwnBall{public int flag_;}
public class OwnUser{public OwnTeam[] teams_=new OwnTeam[]{new OwnTeam(),new OwnTeam()};public OwnBall ball_=new OwnBall();}
public static class BitUtil{public static bool Check(int v,int mask){return (v&mask)!=0;}}
public static class GameUtil{public static float Cos(int angle){return angle*0.01f;}public static int RateConvert(int n,int a,int b,int c,int d,bool yes){return n+c+d;}}
public class Graphics{}
public class Component{public int value;}
public class Marker{public int count;public bool CheckTouch(int id,int value){count++;return id==value;}public void DrawImage(Graphics g,int x,int y,int n){count+=x+y+n;}}
