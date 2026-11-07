// Own fixtures only. No source or classes from the game are compiled.
public class DispatchFixture
{
    SmallList script_=new SmallList();
    int insertScriptIndex_,value;
    int[] GetFirstScript(){return script_.data;}
    void Leaf(int n){value=value*31+n;}
    public bool ScriptRun()
{
    while(script_.Size()>0)
    {
        insertScriptIndex_=1;
        try
        {
            int[] firstScript=GetFirstScript();
            int recoveredScriptCommand=firstScript[0];
            if((recoveredScriptCommand==1))
            {
                if(firstScript[1]<0){script_.RemoveElementAt(0);continue;}
                Leaf(firstScript[1]);
            }
            else if((recoveredScriptCommand==2))
            {
                script_.RemoveElementAt(0);
                continue;
            }
            else if((recoveredScriptCommand==3))
            {
                {
                    int num=firstScript[1];
                    float a=1.25f*num;
                    Leaf((int)a);
                }
            }
            else if((recoveredScriptCommand==4))
            {
                return true;
            }
            break;
        }
        finally{insertScriptIndex_=-1;}
    }
    return false;
}
}
public class BonusFixture
{
    int value,num2;
    int[] data=new int[5];
    void Leaf(int n){value=value*31+n;}
    public void BonusRun()
{
    int[] array2=data;
    int num13,num4,num5,num6,num7,num8,num9,num10,num11,num12;
    num13=num4=num5=num6=num7=num8=num9=num10=num11=num12=0;
    num13=20;
    num4=72;
    int num18=0;
    if(value>0){Leaf(num13);}
    else
    {
        num5=num13;
        int recoveredBonusKind=array2[4];
        if((recoveredBonusKind==0)){{int n=2;Leaf(n+num4);}}
        else if((recoveredBonusKind==1)){Leaf(num5);}
        else if((recoveredBonusKind==2)){Leaf(num13);}
    }
    for(int k=0;k<2;k++)
    {
        if((num2==0)||(num2==1)){Leaf(k+num4);}
        else if((num2==2))
        {
            {
                Leaf(k+num13);
                if((num18==10)){num6=1;}
                else if((num18==3)){num6=2;}
                Leaf(num6);
            }
        }
    }
    for(int l=0;l<3;l++)
    {
        if((l==0)){Leaf(num4);}
        else if((l==1)){{for(int j=0;j<2;j++){if(j==1){break;}Leaf(j);}}}
        else if((l==2)){Leaf(num13);}
    }
}
}
public class SmallList
{
    public int[] data=new int[3];
    int count=1;
    public int Size(){return count;}
    public void RemoveElementAt(int i){count=0;}
}
