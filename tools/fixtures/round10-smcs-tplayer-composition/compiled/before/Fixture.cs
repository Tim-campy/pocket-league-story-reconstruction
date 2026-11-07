// Own emission mold, deliberately independent of game implementation.
public class TPlayerMoldFixture
{
    Queue script_=new Queue(); Ball ball_=new Ball(); Vec goalPos_=new Vec();
    Vec pos_=new Vec(); Group tGroupArea_=new Group(); int flag_,insertScriptIndex_,state;
    static int[] SEB_H_SAVING_ANIME=new int[]{40};
    int[] GetFirstScript(){return script_.data;}
    Ball GetBall(){return ball_;}
    int Read(){return state;}
    void Use(float x){state+=(int)x;}
    void CollisionSaving(Ball b){state++;}
    public bool Run()
    {
        while(script_.Size()>0)
        {
            insertScriptIndex_=1;
            try
            {
                int[] firstScript=GetFirstScript();
                switch(firstScript[0])
                {
                    case 20:
                    {
                        Ball ball9=GetBall();
                        if(!BitUtil.Check(ball9.flag_,8)){break;}
                        int[] collisionData19=Player.udat_.GetCollisionData(0,1);
                        int[] collisionData20=Player.udat_.GetCollisionData(1,1);
                        if(collisionData19[0]!=collisionData20[0])
                        {
                            int angle3=Read(); int angle4=Read();
                            float y3=angle3*2f; float x4=angle4*3f; float z3=y3+x4;
                            Use(z3);
                        }
                        break;
                    }
                    case 22:
                    {
                        int num9=Read();
                        if(num9!=-1)
                        {
                            float y=state*2f; float x=state*3f; Use(x+y);
                        }
                        else
                        {
                            int angle=Read();int angle2=Read();
                            float y2=angle*2f;float x2=angle2*3f;float z=y2+x2;Use(z);
                        }
                        switch(num9)
                        {
                            case 0:Use(10);break;
                            case 1:Use(20);break;
                            case 2:Use(30);break;
                            case -1:{int saved=Read();Use(saved);break;}
                        }
                        script_.RemoveElementAt(0);continue;
                    }
                    case 24:
                    {
                        float num21=state*2f;goalPos_.z_ += 0f - num21;break;
                    }
                    case 30:
                    {
                        for(int k=0;k<2;k++)
                        {
                            int toGoalLineTime=Read();
                            if(toGoalLineTime<=0 || toGoalLineTime>=Player.SEB_H_SAVING_ANIME[0]*5/10){continue;}
                            Use(k);break;
                        }
                        if(BitUtil.Check(flag_,8)){break;}
                        int[] collisionData13=Player.udat_.GetCollisionData(0,5);
                        int[] collisionData14=Player.udat_.GetCollisionData(1,5);
                        Use(collisionData13[0]+collisionData14[0]);break;
                    }
                    case 31:
                    {
                        for(int i=0;i<2;i++)
                        {
                            Ball tBall=GetBall();
                            if(tBall.ChBallDirectHere(this)){CollisionSaving(tBall);}
                        }
                        break;
                    }
                    case 32:
                    {
                        int num13=0;int num14=0;
                        num13=((pos_.x_>=tGroupArea_.centerPos_.x_+tGroupArea_.coatW_/2f)?2:((pos_.x_>tGroupArea_.centerPos_.x_-tGroupArea_.coatW_/2f)?1:0));
                        num14=((!(pos_.z_>=tGroupArea_.centerPos_.z_+tGroupArea_.coatH_/2f))?((pos_.z_>tGroupArea_.centerPos_.z_-tGroupArea_.coatH_/2f)?1:2):0);
                        switch(num13){case 0:Use(11);break;case 1:Use(12);break;default:Use(13);break;}
                        switch(num14){case 0:Use(21);break;case 1:Use(22);break;default:Use(23);break;}
                        int num15=num13+num14*3;int num16=0;
                        switch(num15)
                        {
                            case 1:case 2:num16=0;break;
                            case 0:case 3:num16=3;break;
                            case 6:case 7:num16=2;break;
                            case 5:case 8:num16=1;break;
                            default:num16=Read();break;
                        }
                        switch(num16){case 0:Use(31);break;case 1:Use(32);break;case 2:Use(33);break;case 3:Use(34);break;}
                        break;
                    }
                }
            }
            finally{insertScriptIndex_=-1;}
            break;
        }
        return false;
    }
}
public class Queue{public int[] data=new int[3];public int count=1;public int Size(){return count;}public void RemoveElementAt(int n){count=0;}}
public class Ball{public int flag_;public bool ChBallDirectHere(object p){return flag_==0;}}
public class Vec{public float x_,z_;}
public class Group{public Vec centerPos_=new Vec();public float coatW_,coatH_;}
public class User{public int[] GetCollisionData(int a,int b){return new int[]{a,b};}}
public class Player{public static User udat_=new User();public static int[] SEB_H_SAVING_ANIME=new int[]{40};}
public static class BitUtil{public static bool Check(int value,int flag){return (value&flag)!=0;}}
