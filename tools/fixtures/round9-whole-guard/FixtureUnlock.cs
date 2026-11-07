// NOT_TESTED / NOT_COMPILED own source template.
using System;
public sealed class FixtureSaveUnlockPositive {
 public static class Config { public static bool PRINT; }
 public static class Log { public static int Effects; public static void Info(object value){ Effects++; } }
 int lockThreadCount_; object lockThread_; bool[] saveReserve_ = new bool[2];
 void SaveGame(bool first, bool second){ Log.Effects++; }
 public void Current(bool forced) {
  lockThreadCount_--;
  if(lockThreadCount_<=0 || forced){lockThreadCount_=0;lockThread_=null;}
  if (!forced) { return; }
  for (int i = 0; i < saveReserve_.Length; i++) {
   if(!saveReserve_[i]) { continue; }
   try { saveReserve_[i]=false; SaveGame(i==1,true); }
   catch(Exception t) { if(Config.PRINT) { Log.Info(t); } }
  }
 }
 public void Expected(bool forced) {
  lockThreadCount_--;
  if(lockThreadCount_<=0 || forced){lockThreadCount_=0;lockThread_=null;}
  if (forced) {
   for (int i = 0; i < saveReserve_.Length; i++) {
    if(!saveReserve_[i]) { continue; }
    try { saveReserve_[i]=false; SaveGame(i==1,true); }
    catch(Exception t) { if(Config.PRINT) { Log.Info(t); } }
   }
  }
 }
 // Negative proposals: action in null/false guard; non-root guard; captures;
 // extra following local use outside the proposed positive tail scope.
}
