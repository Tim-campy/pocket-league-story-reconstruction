public class TextFormat {
    public int value; public int GetFont(){return value;}
}
public class Font {
    int size = 2; public int GetSize(){return size;} public void SetSize(int value){size=value;}
    public int GetHeight(){return size*2;} public int GetLocalizeScale(){return size+1;}
    public bool IsNeedLocalizeScale(){return size>0;}
    public static void LockLocalizeSetting(){} public static void UnlockLocalizeSetting(){}
}
public class Layout {
    public int value; public void SetSize(float w,float h){value=(int)(w+h);}
    public void SetText(string text){value+=text.Length;} public void Flush(){value++;}
    public void Draw(float x,float y){value+=(int)(x+y);}
}
public class Fixture {
    Font font_ = new Font(); int sink;
    void Paint(float x,float y,float width,int height,int color,int lineSpace,int anchor){sink+=(int)(x+y+width)+height+color+lineSpace+anchor;}
    public void Run(TextFormat lf,float x,float y,float width,int anchor) {
        int num=anchor;
        int num2=-1;
        if(lf!=null && lf.GetFont()!=-1){num2=font_.GetSize();font_.SetSize(lf.GetFont());}
        bool num3=(lf == null || lf.GetFont() == -1) && font_.IsNeedLocalizeScale();
        if(num3) {
            num2=font_.GetSize();
            int height=font_.GetHeight();
            font_.SetSize(font_.GetLocalizeScale());
            if((anchor&0x60)==0){y+=(float)(height-font_.GetHeight())/2f;}
            Font.LockLocalizeSetting();
        }
        if((num&0x20)!=0){y-=(float)(font_.GetHeight()/2);}
        else if((num&0x40)!=0){y-=(float)font_.GetHeight();}
        Paint(x,y,width,font_.GetHeight(),7,8,anchor);
        if(num3){Font.UnlockLocalizeSetting();}
        if(num2!=-1){font_.SetSize(num2);}
    }
}
