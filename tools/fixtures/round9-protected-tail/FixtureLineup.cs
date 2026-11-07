// TEXT TEMPLATE ONLY: NOT_TESTED / NOT_COMPILED / API_ACCEPTANCE_UNKNOWN.
using System;
public static class FixtureLineupStartComposition {
 public static int Effects;
 public static class Config { public static bool PRINT; }
 public static class Log { public static void Info(object value) { Effects++; } }
 public static class Storage {
  public static byte[] Input;
  public static byte[] Read(int mode, bool option, string file) { return Input; }
 }
 public class JarInflater {
  readonly byte[] data;
  public JarInflater(byte[] value) { data = value; }
  public byte[] GetData(string key) { return data; }
  public void Close() { Effects++; }
 }
 static string ReadPreference() { return "update.txt"; }
 static void LoadLineupList(byte[] data) { Effects += data.Length; }
 public static void Current(int num) {
  JarInflater jarInflater = null;
  try {
   byte[] array = Storage.Read(3, false, "lineup.dat");
   if (array == null) { return; }
   jarInflater = new JarInflater(array);
   string time = "";
   try { time = ReadPreference(); } catch(Exception t) { if(Config.PRINT) { Log.Info(t); } }
   byte[] array3 = null;
   try { array3 = jarInflater.GetData(time); } catch(Exception t2) { if(Config.PRINT) { Log.Info(t2); } }
   if(array3 == null) { array3 = jarInflater.GetData("list.bin"); }
   if(array3 == null) { return; }
   LoadLineupList(array3);
   int index = 0;
   while(index < num) {
    if(index == array3.Length) { break; }
    Effects += array3[index];
    index++;
   }
   if(Effects < 0) { return; }
   for(int j = 0; j < num; j++) { Effects += j; }
  } catch(Exception t6) { if(Config.PRINT) { Log.Info(t6); } }
  finally { jarInflater?.Close(); jarInflater = null; }
 }
 public static void Expected(int num) {
  JarInflater jarInflater = null;
  try {
   byte[] array = Storage.Read(3, false, "lineup.dat");
   if ((object)array != null) {
   jarInflater = new JarInflater(array);
   string time = "";
   try { time = ReadPreference(); } catch(Exception t) { if(Config.PRINT) { Log.Info(t); } }
   byte[] array3 = null;
   try { array3 = jarInflater.GetData(time); } catch(Exception t2) { if(Config.PRINT) { Log.Info(t2); } }
   if(array3 == null) { array3 = jarInflater.GetData("list.bin"); }
   if(array3 == null) { return; }
   LoadLineupList(array3);
   int index = 0;
   while(index < num) {
    if(index == array3.Length) { break; }
    Effects += array3[index];
    index++;
   }
   if(Effects < 0) { return; }
   for(int j = 0; j < num; j++) { Effects += j; }
   }
  } catch(Exception t6) { if(Config.PRINT) { Log.Info(t6); } }
  finally { if ((object)jarInflater != null) { jarInflater.Close(); } jarInflater = null; }
 }
 // Proposed controls: positive first guard only; Close explicit-null guard only.
 // Proposed negatives: effect inside removed null guard; changed receiver/call;
 // extra statement between Storage.Read and guard; finally reading a tail local;
 // capture/goto entering a tail block. API rejection/semantics UNKNOWN here.
 // Preserve later early returns, break owners, both catches and cleanup order.
 // Compare complete IL, actual locals and EH with the historical compiler.
}
