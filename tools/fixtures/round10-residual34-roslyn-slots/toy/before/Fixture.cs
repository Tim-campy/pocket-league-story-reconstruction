using System;using System.IO;
class JThread {public static void Sleep(long x){}}
class IApplication {static IApplication a=new IApplication();public static IApplication GetCurrentApp(){return a;}public object RunOnUiThread(Func<object> d,bool wait){return d();}}
class JarInflater {public int Read(byte[] a){return a.Length;}}
public class Fixture {byte[] src_=new byte[3];int sourceImageMaxCount_=3,sourceImageMaxPixels_=4;int total;string[] rows=new string[3];
public void Capture(){Action a=delegate{for(int i=0;i<rows.Length;i++){string path=rows[i];for(int j=0;j<rows.Length;j++){total+=j;}bool flag=false;while(!File.Exists(path)){JThread.Sleep(10L);}if(!(bool)IApplication.GetCurrentApp().RunOnUiThread(delegate{return path.Length>1;},true)){i--;}}};a();}
public void Use(){Action a=delegate{byte[] array=src_;if(array!=null){JarInflater jarInflater=new JarInflater();total+=jarInflater.Read(array);}int retry=0;while(retry<2){retry++;}int num2=sourceImageMaxCount_;int num3=sourceImageMaxPixels_;for(int i=0;i<num2;i++){total+=num3+i;}total+=array.Length;};a();}
}
