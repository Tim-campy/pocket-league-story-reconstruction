// NOT_TESTED / NOT_COMPILED own source template. No API ran on this text.
using System;
public sealed class FixturePaymentRetryElse {
 public static class Config { public static bool DEBUG; }
 public static class Log { public static int Effects; public static void Info(object value){ Effects++; } }
 public static class JThread { public static void Sleep(long value){ Log.Effects++; } }
 public byte[][] Data;
 void ConnectAsync(){ Log.Effects++; }
 byte[][] Connect(){ return Data; }
 public int Current(bool async, int num) {
  int num2 = 0; int result = 0;
  do {
   num2++;
   try {
    if(async) { ConnectAsync(); break; }
    byte[][] array = Connect();
    if (array == null || array.Length == 0) { result = -1; goto IL_0304; }
    for (int j = 0; j < array.Length; j++) { result += array[j].Length; }
   } catch(Exception t) {
    if(Config.DEBUG) { Log.Info(t); }
    result = -1;
    goto IL_0304;
   }
   break;
   IL_0304:
   JThread.Sleep(100L);
  } while(num2 < num);
  return result;
 }
 public int Expected(bool async, int num) {
  int num2 = 0; int result = 0;
  do {
   num2++;
   try {
    if(async) { ConnectAsync(); break; }
    byte[][] array = Connect();
    if (array == null || array.Length == 0) { result = -1; }
    else {
     for (int j = 0; j < array.Length; j++) { result += array[j].Length; }
     break;
    }
   } catch(Exception t) {
    if(Config.DEBUG) { Log.Info(t); }
    result = -1;
   }
   JThread.Sleep(100L);
  } while(num2 < num);
  return result;
 }
 // Compare try-end branch versus leave, unchanged async outer-do break, catch
 // fallthrough to Sleep and retry increment. Negatives: another break in scan;
 // extra statements after scan/before try end; another goto/label/closure.
}
