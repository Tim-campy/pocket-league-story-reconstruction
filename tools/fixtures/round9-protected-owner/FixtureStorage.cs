// NOT_TESTED / NOT_COMPILED. Own stubs, no external file operations.
using System;
public sealed class FixtureStorageWriteExits {
 public static class Config { public static bool USE_HEAP_STORAGE; public static string ConvertExtension(string value){return value;} }
 public sealed class Vector { public void SetElementAt(object value,int index){ Effects++; } public void AddElement(object value){ Effects++; } }
 public static int Effects; int media_;
 static readonly Vector heapData_=new Vector(), heapName_=new Vector();
 int _SearchHeapFile(string filename){return Effects;}
 void WriteFile(string name,byte[] data){ Effects += data.Length; }
 void Record(int value){ Effects += value; }
 public void Current(string filename, byte[] data) {
  try {
   filename=Config.ConvertExtension(filename);
   if(Config.USE_HEAP_STORAGE && media_==2) {
    int num=_SearchHeapFile(filename);
    if (num != -1) { heapData_.SetElementAt(data, num); return; }
    heapName_.AddElement(filename);
    heapData_.AddElement(data);
   }
   else if (media_ != 3 && (media_ == 1 || media_ == 4)) {
    if(data==null) { data=new byte[0]; }
    WriteFile(filename,data);
    Record(0);
    Record(1);
    Record(2);
    Record(3);
    Record(4);
    Record(5);
    Record(6);
    Record(7);
    Record(8);
    Record(9);
    Record(10);
    Record(11);
    Record(12);
    Record(13);
    Record(14);
    Record(15);
    Record(16);
    Record(17);
    Record(18);
    Record(19);
    Record(20);
    Record(21);
    Record(22);
    Record(23);
   }
  } catch(UnauthorizedAccessException ex) { throw new Exception(ex.Message,ex); }
 }
 public void Expected(string filename, byte[] data) {
  try {
   filename=Config.ConvertExtension(filename);
   if(Config.USE_HEAP_STORAGE && media_==2) {
    int num=_SearchHeapFile(filename);
    if(num!=-1) { heapData_.SetElementAt(data,num); }
    else { heapName_.AddElement(filename); heapData_.AddElement(data); }
    return;
   }
   else {
    if(media_==3) { return; }
    if(media_==1 || media_==4) {
     if(data==null) { data=new byte[0]; }
     WriteFile(filename,data);
    Record(0);
    Record(1);
    Record(2);
    Record(3);
    Record(4);
    Record(5);
    Record(6);
    Record(7);
    Record(8);
    Record(9);
    Record(10);
    Record(11);
    Record(12);
    Record(13);
    Record(14);
    Record(15);
    Record(16);
    Record(17);
    Record(18);
    Record(19);
    Record(20);
    Record(21);
    Record(22);
    Record(23);
    }
   }
  } catch(UnauthorizedAccessException ex) { throw new Exception(ex.Message,ex); }
 }
 // Independent controls: heap if/else+common return only; media3 guard only.
 // Negatives: extra effect between heap branches; changed heap setter/index;
 // noninteger media comparisons; capture or protected-exit observer.
}
