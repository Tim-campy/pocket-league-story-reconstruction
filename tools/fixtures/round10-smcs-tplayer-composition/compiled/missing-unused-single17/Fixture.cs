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
int recoveredScriptCommand = firstScript[0];
                if ((recoveredScriptCommand == 20)) {
{
                        Ball ball9=GetBall();
                        if (BitUtil.Check(ball9.flag_, 8)) {
                        int[] collisionData19=Player.udat_.GetCollisionData(0,1);
                        int[] collisionData20=Player.udat_.GetCollisionData(1,1);

                        if(collisionData19[0]!=collisionData20[0])
                        {
                            int angle3=Read(); int angle4=Read();
                            float y3=angle3*2f; float x4=angle4*3f; float z3=y3+x4;
                            Use(z3);
                        }

}

                    }
}
else if ((recoveredScriptCommand == 22)) {
{
                        int num9=Read();
                        if(num9!=-1)
                        {
                            float y=state*2f; float x=state*3f; Use(x+y);
                        float recoveredUnusedSingle33;
}
                        else
                        {
                            int angle=Read();int angle2=Read();
                            float y2=angle*2f;float x2=angle2*3f;float z=y2+x2;Use(z);
                        }
                        if ((num9 == 0)) {
Use(10);
}
else if ((num9 == 1)) {
Use(20);
}
else if ((num9 == 2)) {
Use(30);
}
else if ((num9 == -1)) {
{int saved=Read();Use(saved);}
}
                        script_.RemoveElementAt(0);continue;
                    }
}
else if ((recoveredScriptCommand == 24)) {
{
                        float num21=state*2f;goalPos_.z_ += -num21;
                    }
}
else if ((recoveredScriptCommand == 30)) {
{
                        for(int k=0;k<2;k++)
                        {
                            int toGoalLineTime=Read();
                            if(toGoalLineTime<=0){continue;} if(toGoalLineTime>=Player.SEB_H_SAVING_ANIME[0]*5/10){continue;}
                            Use(k);break;
                        }
                        if (!BitUtil.Check(flag_, 8)) {
                        int[] collisionData13=Player.udat_.GetCollisionData(0,5);
                        int[] collisionData14=Player.udat_.GetCollisionData(1,5);
                        Use(collisionData13[0]+collisionData14[0]);
}

                    }
}
else if ((recoveredScriptCommand == 31)) {
{
                        for(int i=0;i<2;i++)
                        {
                            Ball tBall=GetBall();
                            if(!tBall.ChBallDirectHere(this)){continue;} CollisionSaving(tBall);
                        }

                    }
}
else if ((recoveredScriptCommand == 32)) {
{
                        int num13=0;int num14=0;
                        if(pos_.x_>=tGroupArea_.centerPos_.x_+tGroupArea_.coatW_/2f){num13=2;} else if(pos_.x_>tGroupArea_.centerPos_.x_-tGroupArea_.coatW_/2f){num13=1;} else{num13=0;}
                        if(pos_.z_>=tGroupArea_.centerPos_.z_+tGroupArea_.coatH_/2f){num14=0;} else if(pos_.z_>tGroupArea_.centerPos_.z_-tGroupArea_.coatH_/2f){num14=1;} else{num14=2;}
                        if ((num13 == 0)) {
Use(11);
}
else if ((num13 == 1)) {
Use(12);
}
else {
Use(13);
}
                        if ((num14 == 0)) {
Use(21);
}
else if ((num14 == 1)) {
Use(22);
}
else {
Use(23);
}
                        int num15=num13+num14*3;int num16=0;
                        if ((num15 == 1) || (num15 == 2)) {
num16=0;
}
else if ((num15 == 0) || (num15 == 3)) {
num16=3;
}
else if ((num15 == 6) || (num15 == 7)) {
num16=2;
}
else if ((num15 == 8) || (num15 == 5)) {
num16=1;
}
else {
num16=Read();
}
                        if ((num16 == 0)) {
Use(31);
}
else if ((num16 == 1)) {
Use(32);
}
else if ((num16 == 2)) {
Use(33);
}
else if ((num16 == 3)) {
Use(34);
}

                    }
}
            break;
}
            finally{insertScriptIndex_=-1;}

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
