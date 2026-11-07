// NOT_TESTED / NOT_COMPILED. Heavy catch exposes backward leave width.
using System;
public sealed class FixtureHttpRetryContinue {
 public bool askRetry_; public static int Effects;
 byte[][] Core(){ Effects++; return null; }
 bool Retry(){ return true; }
 void Trace(int value){ Effects += value; }
 public byte[][] Current() {
  int num = 0;
  while(true) {
   try { return Core(); }
   catch(Exception ex) {
    if(!askRetry_) { goto IL_00ab; }
    if(!Retry()) { goto IL_00ab; }
    Trace(0);
    Trace(1);
    Trace(2);
    Trace(3);
    Trace(4);
    Trace(5);
    Trace(6);
    Trace(7);
    Trace(8);
    Trace(9);
    Trace(10);
    Trace(11);
    Trace(12);
    Trace(13);
    Trace(14);
    Trace(15);
    Trace(16);
    Trace(17);
    Trace(18);
    Trace(19);
    Trace(20);
    Trace(21);
    Trace(22);
    Trace(23);
    Trace(24);
    Trace(25);
    Trace(26);
    Trace(27);
    num++;
    goto end_IL_0014;
    IL_00ab:
    throw ex;
    end_IL_0014:;
   }
  }
 }
 public byte[][] Expected() {
  int num = 0;
  while(true) {
   try { return Core(); }
   catch(Exception ex) {
    if(!askRetry_) { goto IL_00ab; }
    if(!Retry()) { goto IL_00ab; }
    Trace(0);
    Trace(1);
    Trace(2);
    Trace(3);
    Trace(4);
    Trace(5);
    Trace(6);
    Trace(7);
    Trace(8);
    Trace(9);
    Trace(10);
    Trace(11);
    Trace(12);
    Trace(13);
    Trace(14);
    Trace(15);
    Trace(16);
    Trace(17);
    Trace(18);
    Trace(19);
    Trace(20);
    Trace(21);
    Trace(22);
    Trace(23);
    Trace(24);
    Trace(25);
    Trace(26);
    Trace(27);
    num++;
    continue;
    IL_00ab:
    throw ex;
   }
  }
 }
 // Proposed negatives: different continue owner/nested loop; effect following
 // end label; changed throw ex to rethrow; additional go-to path after num++.
}
