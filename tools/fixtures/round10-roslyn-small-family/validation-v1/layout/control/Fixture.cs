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
    Font selectedFont=new Font(); Layout cached;
    Font GetFont(){return selectedFont;} float GetScale(){return selectedFont.GetSize();}
    Layout SearchCache(string text,float w,float h,int color,int space,int anchor,int size,float scale){return cached;}
    void AddCache(Layout value){cached=value;}
    public void Run(TextFormat lf,string text,float x,float y,float w,float h,int color,int lineSpace,int anchor) {
        Font font=GetFont();
        float scale=GetScale();
        int num=-1;
        if(lf!=null && lf.GetFont()!=-1){num=font.GetSize();font.SetSize(lf.GetFont());}
        bool num2=(lf == null || lf.GetFont() == -1) && font.IsNeedLocalizeScale();
        if(num2) {
            num=font.GetSize();
            int height=font.GetHeight();
            font.SetSize(font.GetLocalizeScale());
            if((anchor&0x60)==0){y+=(float)(height-font.GetHeight())/2f;}
            Font.LockLocalizeSetting();
        }
        Layout layout=SearchCache(text,w,h,color,lineSpace,anchor,font.GetSize(),scale);
        if(layout!=null){layout.Draw(x,y);}
        else {
            layout=new Layout();layout.SetSize(w,h);layout.SetText(text);layout.Flush();AddCache(layout);layout.Draw(x,y);
        }
        if(num2){Font.UnlockLocalizeSetting();}
        if(num!=-1){font.SetSize(num);}
    }
}
