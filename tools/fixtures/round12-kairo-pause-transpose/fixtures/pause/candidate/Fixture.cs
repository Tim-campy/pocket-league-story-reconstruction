using System;
public class NotifyManager{public static NotifyManager GetInstance(){return null;}public void DoNotify(){}public void DoCancel(){}}
public class FormBase{public void Suspend(){}public bool Resume(){return true;}}
public class FormManagerBase{public int GetFormsNum(){return 3;}public FormBase GetForm(int n){return null;}public static void ProcessEvent(AppEventType e,params object[] args){}}
public enum AppEventType{Hidden=0,Resume=4}
public static class KairoPlugin{public static void SetNotificationBackground(bool p){}}
public static class AnalyticsTracker{public static void DispatchPageView(){}}
public class Fixture{
 bool isPause_;public virtual FormManagerBase GetFormManager(){return null;}public virtual void OnSuspend(){}public virtual void OnResume(){}
 public void Run(bool pauseStatus){
isPause_=pauseStatus;
if (pauseStatus){
 NotifyManager pauseNotify = NotifyManager.GetInstance();
if ((object)pauseNotify != null) { pauseNotify.DoNotify(); }
 KairoPlugin.SetNotificationBackground(true);FormManagerBase.ProcessEvent(AppEventType.Hidden);
 FormManagerBase formManager=GetFormManager();
 for(int num=formManager.GetFormsNum()-1;num>=0;num--){FormBase form=formManager.GetForm(num);try{form.Suspend();}catch (Exception){}}
 OnSuspend();return;
} else {
NotifyManager resumeNotify = NotifyManager.GetInstance();
if ((object)resumeNotify != null) { resumeNotify.DoCancel(); }
KairoPlugin.SetNotificationBackground(false);FormManagerBase.ProcessEvent(AppEventType.Resume);AnalyticsTracker.DispatchPageView();
FormManagerBase formManager2=GetFormManager();
for(int num2=formManager2.GetFormsNum()-1;num2>=0;num2--){FormBase form2=formManager2.GetForm(num2);try{form2.Resume();}catch (Exception){}}
OnResume();

}}
}