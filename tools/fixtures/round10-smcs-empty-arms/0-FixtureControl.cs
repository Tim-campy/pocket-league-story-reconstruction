// INDEPENDENTLY AUTHORED OWN CONTROL. NOT_COMPILED / NOT_EXECUTED.
public class EmptyArmFixture
{
    public int type_,id_,value;
    public Area tArea_=new Area();
    public BallList tBallList_=new BallList();
    private void Leaf(int n){value=value*17+n;}

    public bool FriendRun()
    {
        Leaf(7);
        if(type_==11){string text="friend";Leaf(text.Length);}
        else if(type_==12){}
        else if(type_==13){}else if(type_==15){}
        Leaf(9);
        return true;
    }

    public void TouchRun()
    {
        if(id_==0){float x=2.5f;Leaf((int)x);}
        else if(id_==1){}
        else if(id_==2){}else if(id_==3){}
    }

    public void BillingDrawRun()
    {
        if(type_==0){Leaf(0);}
        else if(type_==1){Leaf(1);}
        else
        {
            if(type_==2){}else if(type_==3){}else if(type_==4){}else if(type_==5)
            {
                int num3=174;
                int num4=107;
                Leaf(num3+num4);
                for(int i=0;i<2;i++){int k=i+num3;Leaf(k);}
            }
        }
    }

    public Ball GroupBallRun(Player player,int type)
    {
        if(tArea_.tMode_==2){Leaf(2);}
        else if(tArea_.tMode_==4){}
        else if(tArea_.tMode_==3)
        {
            for(int k=0;k<tBallList_.Size();k++)
            {
                Ball tBall3=tBallList_.ElementAt(k);
                if(tBall3.t_owner_!=null&&tBall3.t_owner_.trainingId_[0]==player.trainingId_[0]){return tBall3;}
            }
        }
        return null;
    }

    public bool BillingInitRun()
    {
        if(type_==3){Leaf(3);}else if(type_==4){}
        Leaf(8);
        return true;
    }

    public void MatchDrawRun()
    {
        Leaf(8);
        if(type_==0){}else if(type_==1){}
    }
}
public class Area{public int tMode_;}
public class Player{public int[] trainingId_=new int[1];}
public class Ball{public Player t_owner_;}
public class BallList{
    public Ball[] items=new Ball[1];
    public int Size(){return items.Length;}
    public Ball ElementAt(int i){return items[i];}
}
