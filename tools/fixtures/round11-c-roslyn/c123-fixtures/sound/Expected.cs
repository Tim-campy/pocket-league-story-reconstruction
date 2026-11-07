using System;
public class Track {public bool isPlaying,loop;public float time;public void Play(){isPlaying=true;}}
public class SoundBase {protected int state_;Track selected=new Track();protected Track track_{get{return selected;}}}
public class SoundPlayer {
 public static SoundPlayer GetInstance(){return new SoundPlayer();}public int GetPort(int p){return p;}public void RemovePort(int p){}public void SetPort(int p,object value){}public void AddPlayingList(object value){}public void Mark(object value){}
}
public static class Config {public static bool PRINT;}
public static class Log {public static int Info(object value){return 1;}}
public class Fixture:SoundBase {
 int type_,loop_,priority_;void SetVolume(int volume,bool mute){priority_=mute?0:volume;}void Stop(){state_=0;}
 public void Run(int volume,bool mute){
 Action action=delegate{
 SoundPlayer instance=SoundPlayer.GetInstance();
 try{
 if(base.track_!=null && base.state_==2 && base.track_.isPlaying){SetVolume(volume,mute);base.track_.time=0f;return;}
 {
 Stop();
 if(type_==1 && mute){return;}
 {
if(loop_!=0){priority_=2;}else if(type_==0){priority_=1;}else{priority_=0;}
 int port=instance.GetPort(priority_);
 if(port==-1){return;}
{
 instance.RemovePort(port);instance.SetPort(port,this);SetVolume(volume,mute);base.track_.loop=loop_!=0;
 if(loop_!=0){instance.AddPlayingList(this);}base.track_.Play();
}
 }
 }
 }catch(Exception t){if(Config.PRINT){Log.Info(t);}Stop();}
 };
 action();
 }
}
