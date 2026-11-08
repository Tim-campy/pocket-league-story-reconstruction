using System;
public static class Config {public static bool PRINT;}
public static class Log {public static int Info(string value){return value.Length;}}
public class JarInflater {public byte[] GetData(string name){return new byte[name.Length];}}
public class Texture {public int width=1;}
public class Fixture {
 byte[] src_;string file_,paletteFile_;Texture image_;int alphaType_=-1;int maxCount_=2,maxPixels_=3;long stamp_;Fixture[] items=new Fixture[1];
 static JarInflater OpenJar(){return new JarInflater();}void OverwritePaletteData(byte[] data,byte[] palette){}
 Texture DecodeByteArray(byte[] data){return new Texture{width=data.Length};}void CopeOutOfMemory(int number,OutOfMemoryException error){stamp_=number;}
 int GetAlphaType(byte[] data){return data.Length;}void SetAlpha(int value){alphaType_=value;}int Width(){return image_==null?0:image_.width;}void Touch(){stamp_++;}long Clock(){return stamp_++;}void Mark(int value){stamp_+=value;}
 public void Run(){
 Action action=delegate{
 byte[] array = src_; int num2; int num3;
 if(array==null && file_!=null){
 JarInflater jarInflater=OpenJar();array=jarInflater.GetData(file_);
 if(paletteFile_!=null){OverwritePaletteData(array,jarInflater.GetData(paletteFile_));}
 }
 if(array==null){if(Config.PRINT){Log.Info("Image.use() missing ["+file_+"]");}return;}{
 int num=0;
 while(true){
 try{image_ = DecodeByteArray(array);}
 catch(OutOfMemoryException error){CopeOutOfMemory(num,error);goto retry;}
 break;
 retry:num++;
 }
 if(alphaType_==-1){int alphaType=GetAlphaType(array);if(alphaType!=-1){SetAlpha(alphaType);}}
 Mark(0);
 Mark(1);
 Mark(2);
 Mark(3);
 Mark(4);
 Mark(5);
 Mark(6);
 Mark(7);
 Mark(8);
 Mark(9);
 Mark(10);
 Mark(11);
 Mark(12);
 Mark(13);
 Mark(14);
 Mark(15);
 Mark(16);
 Mark(17);

 num2=maxCount_;num3=maxPixels_;
 while(num2>1 && num3>0){
 Fixture image=null;long num4=long.MinValue;
 for(int i=0;i<items.Length;i++){
 Fixture image2=items[i];long num5=Clock()-image2.stamp_+image2.Width();
 if(num4<num5){num4=num5;image=image2;}
 }
 if(image!=null){image.Touch();}num3--;
 }
}
 };
 action();
 }
}
