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
                switch(firstScript[0])
                {
                    case 1:
                        if(firstScript[1]<0){script_.RemoveElementAt(0);continue;}
                        Leaf(firstScript[1]);
                        break;
                    case 2:
                        script_.RemoveElementAt(0);
                        continue;
                    case 3:
                    {
                        int num=firstScript[1];
                        float a=1.25f*num;
                        Leaf((int)a);
                        break;
                    }
                    case 4:
                        return true;
                }
            }
            finally{insertScriptIndex_=-1;}
            break;
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
        int num5;int num6;int num7;int num8;int num9;int num12;int num11;int num10;
        int num4=(num5=(num6=(num7=(num8=(num9=(num10=(num11=(num12=0))))))));
        int num13=20;
        num4=72;
        int num18=0;
        if(value>0){Leaf(num13);}
        else
        {
            num5=num13;
            switch(array2[4])
            {
                case 0:{int n=2;Leaf(n+num4);break;}
                case 1:Leaf(num5);break;
                case 2:Leaf(num13);break;
            }
        }
        for(int k=0;k<2;k++)
        {
            switch(num2)
            {
                case 0:
                case 1:
                    Leaf(k+num4);break;
                case 2:
                {
                    Leaf(k+num13);
                    switch(num18){case 10:num6=1;break;case 3:num6=2;break;}
                    Leaf(num6);
                    break;
                }
            }
        }
        for(int l=0;l<3;l++)
        {
            switch(l)
            {
                case 0:Leaf(num4);break;
                case 1:{for(int j=0;j<2;j++){if(j==1){break;}Leaf(j);}break;}
                case 2:Leaf(num13);break;
            }
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
