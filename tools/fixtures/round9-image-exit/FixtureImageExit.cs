// SOURCE TEMPLATE ONLY: NOT_TESTED / NOT_COMPILED.
// Current and Expected share this declaring type and all callees.
using System;
public sealed class FixtureGraphicsD29Residual {
 public sealed class Image {
  public int optimizePass_;
  public sbyte[] optimizeSeb_;
  public sbyte[] optimizeAccess_;
  public short[] optimizeAccessL_;
  public int[] GetOptimizeSeb(int index) { Effects += index; return null; }
  public int[] GetOptimize(sbyte index) { Effects += index; return new int[] { index }; }
  public int[] GetOptimize(short index) { Effects += index; return new int[] { index }; }
 }
 public static int Effects;
 static void Consume(int value) { Effects += value; }
 static void Fallback(Image image) { Effects += image.optimizePass_; }
 static void Cleanup() { Effects++; }
 public static void Current(Image image, int sx) {
  if(image == null) { return; }
  try {
   if(image.optimizePass_ != 0) {
    {
     int num25 = -1;
     int num26 = image.optimizeSeb_.Length / 4;
     for(int j = 0; j < num26; j++) {
      if(image.optimizeSeb_[j * 4] == sx) { num25 = j; break; }
     }
     if (num25 != -1) {
      image.GetOptimizeSeb(num25);
      for(int k = 0; k < image.optimizePass_; k++) {
       int num28 = num25 * image.optimizePass_ + k;
       int[] array = null;
       if (image.optimizeAccess_ == null) { array = image.GetOptimize(image.optimizeAccessL_[num28]); } else { array = image.GetOptimize(image.optimizeAccess_[num28]); }
       if(array == null) { continue; }
       Consume(array[0]);
      }
     }
    }
   } else { Fallback(image); }
  } finally { Cleanup(); }
 }
 public static void Expected(Image image, int sx) {
  if(image == null) { return; }
  try {
   if(image.optimizePass_ != 0) {
    {
     int num25 = -1;
     int num26 = image.optimizeSeb_.Length / 4;
     for(int j = 0; j < num26; j++) {
      if(image.optimizeSeb_[j * 4] == sx) { num25 = j; break; }
     }
     if (num25 != -1) {
      image.GetOptimizeSeb(num25);
      for(int k = 0; k < image.optimizePass_; k++) {
       int num28 = num25 * image.optimizePass_ + k;
       int[] array = null;
       if (image.optimizeAccess_ != null) { array = image.GetOptimize(image.optimizeAccess_[num28]); } else { array = image.GetOptimize(image.optimizeAccessL_[num28]); }
       if(array == null) { continue; }
       Consume(array[0]);
      }
     }
     return;
    }
   } else { Fallback(image); }
  } finally { Cleanup(); }
 }
 // Proposed controls, not executed here:
 // 1. Only reverse the access condition+arms; leave terminal return absent.
 // 2. Only add terminal return after positive lookup guard.
 // Proposed negatives for the pure calculator:
 // - Add Consume(7) after the positive lookup guard but before lookup block exit.
 // - Add an existing terminal return; the API must not duplicate it.
 // - Change one selected overload/index; do not assume a same-named call matches.
 // - Replace the lookup child with another lexical parent or add a capture.
 // Compare complete IL, real local signatures and EH in all independently
 // compiled fixtures. No inference from a prefix-only suffix control result.
}
