// Own five small emission molds. No game source is compiled.
public class JoinFixture
{
    int counter,bonus_softKey,select_,stateCnt_;
    long markComponentKey_;
    int[] TBAR_W=new int[]{1,2,3,4,5,6};
    int[] PBONUS_NP_LV_T_GET=new int[]{2,3};
    Store apdat_=new Store();
    void Use(int n){counter+=n;}
    void ClearMarker(){counter++;}
    Event GetTouchEventId(int id,int type){Use(id);return new Event();}
    Event GetTouchEventKey(long key,int type){Use(type);return new Event();}
    public void Bar(int type,int dx,int w)
    {
        {
            if((type==0)){Use(0);}
            else if((type==1)){Use(1);}
            else if((type==2))
            {
                if(w<=TBAR_W[type]){Use(dx);goto dispatchExit;}
                int num4=w;int num5=0;
                while(num4>0){int num6=num4-1;Use(num6+num5);num4--;num5++;}
            }
            else if((type==3)){Use(3);}
            else if((type==4))
            {
                if(w<=TBAR_W[type]){Use(dx+1);goto dispatchExit;}
                int num=w;int num2=0;
                while(num>0){int num3=num-1;Use(num3+num2);num--;num2++;}
            }
            else if((type==5)){Use(5);}
            dispatchExit:;
        }
    }
    public void Research()
    {
        for(int i=0;i<3;i++)
        {
            if(select_==0)
            {
                int num5=stateCnt_-i;
                if(num5<0){continue;}
                if(num5<PBONUS_NP_LV_T_GET[0]){int num6=num5+1;Use(num6);}
                else{Use(3);}
            }
            else{Use(4);}
        }
        Use(5);
    }
    public void Touch(int type,int value)
    {
        switch(type)
        {
            case 0:Use(0);break;
            case 524288:
                if(apdat_.sysSave_.ints[3]==value){}else{Use(value+1);}
                break;
            case 589824:
                if(apdat_.sysSave_.ints[4]!=value){Use(value+2);}
                break;
            case 720896:
                if(!Config.ADMOB_ENABLE){Use(value+3);}
                break;
            case 1441792:case 1507328:break;
            default:Use(value+4);break;
        }
    }
    public void Unmarker(int type)
    {
        Event touchEvent=null;
        switch(type)
        {
            case 0:Use(0);break;
            case 7:
                if(markComponentKey_==-1){}else
                {
                    touchEvent=GetTouchEventKey(markComponentKey_,7);
                    if(touchEvent==null){ClearMarker();}
                }
                break;
            case 1:
                if(markComponentKey_==-1){break;}
                touchEvent=GetTouchEventId(12,1);if(touchEvent!=null){break;}
                touchEvent=GetTouchEventId(14,1);if(touchEvent!=null){break;}
                touchEvent=GetTouchEventId(15,1);if(touchEvent!=null){break;}
                touchEvent=GetTouchEventId(22,1);
                if(touchEvent==null)
                {
                    touchEvent=GetTouchEventKey(markComponentKey_,1);
                    if(touchEvent==null){ClearMarker();}
                }
                break;
        }
    }
    public bool Receive()
    {
        int num5=counter;
        if(num5==-1)
        {
            try
            {
                for(int j=0;j<5;j++)
                {
                    Use(j);
                    if(j!=0)
                    {
                        try
                        {
                            if(counter<0){return false;}
                            bool flag=j==3||j==4;
                            int num6=((j!=3&&j!=1)?1:0);
                            Use(((!flag)?6:11)+num6);
                            Use(((!flag)?4:8)+num6);
                            Use(((!flag)?2:6)+num6);
                        }
                        catch(System.Exception){Use(3);}
                    }
                }
            }
            finally{counter++;}
        }else{}
        if(num5==-2){Use(7);}
        return true;
    }
}
public class Store{public Property sysSave_=new Property();}
public class Property{public int[] ints=new int[5];}
public class Event{}
public static class Config{public static bool ADMOB_ENABLE;}
