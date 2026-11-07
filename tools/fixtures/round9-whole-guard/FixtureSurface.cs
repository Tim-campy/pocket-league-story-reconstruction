// NOT_TESTED / NOT_COMPILED own source template.
public sealed class FixtureSurfacePositive {
 public static class Config { public static bool DEBUG; }
 public sealed class SurfaceBase { public int value; }
 public sealed class SurfaceList { public int count; public int Size(){ return count; } public SurfaceBase FirstElement(){ return new SurfaceBase(); } }
 public sealed class PaintList { public int Count; }
 public sealed class Graphics { public int Effects; public void Mark(int value){ Effects += value; } }
 readonly SurfaceList surfaceList_ = new SurfaceList(); readonly PaintList paintRects_ = new PaintList();
 bool IsAutoGLRender(){ return true; }
 public void Current(Graphics g) {
  bool flag = IsAutoGLRender(); int i;
  for(i=0; i<surfaceList_.Size(); i++){ if(flag){g.Mark(i);} }
  if (!Config.DEBUG || surfaceList_.Size() <= 0 || paintRects_.Count <= 0) { return; }
  SurfaceBase surfaceBase = surfaceList_.FirstElement();
  try {
    g.Mark(surfaceBase.value + 0);
    g.Mark(surfaceBase.value + 1);
    g.Mark(surfaceBase.value + 2);
    g.Mark(surfaceBase.value + 3);
    g.Mark(surfaceBase.value + 4);
    g.Mark(surfaceBase.value + 5);
    g.Mark(surfaceBase.value + 6);
    g.Mark(surfaceBase.value + 7);
    g.Mark(surfaceBase.value + 8);
    g.Mark(surfaceBase.value + 9);
    g.Mark(surfaceBase.value + 10);
    g.Mark(surfaceBase.value + 11);
    g.Mark(surfaceBase.value + 12);
    g.Mark(surfaceBase.value + 13);
    g.Mark(surfaceBase.value + 14);
    g.Mark(surfaceBase.value + 15);
    g.Mark(surfaceBase.value + 16);
    g.Mark(surfaceBase.value + 17);
    g.Mark(surfaceBase.value + 18);
    g.Mark(surfaceBase.value + 19);
    g.Mark(surfaceBase.value + 20);
    g.Mark(surfaceBase.value + 21);
    g.Mark(surfaceBase.value + 22);
    g.Mark(surfaceBase.value + 23);
    g.Mark(surfaceBase.value + 24);
    g.Mark(surfaceBase.value + 25);
    g.Mark(surfaceBase.value + 26);
    g.Mark(surfaceBase.value + 27);
    g.Mark(surfaceBase.value + 28);
    g.Mark(surfaceBase.value + 29);
    g.Mark(surfaceBase.value + 30);
    g.Mark(surfaceBase.value + 31);
    g.Mark(surfaceBase.value + 32);
    g.Mark(surfaceBase.value + 33);
    g.Mark(surfaceBase.value + 34);
    g.Mark(surfaceBase.value + 35);
    g.Mark(surfaceBase.value + 36);
    g.Mark(surfaceBase.value + 37);
    g.Mark(surfaceBase.value + 38);
    g.Mark(surfaceBase.value + 39);
  } finally { g.Mark(-1); }
 }
 public void Expected(Graphics g) {
  bool flag = IsAutoGLRender(); int i;
  for(i=0; i<surfaceList_.Size(); i++){ if(flag){g.Mark(i);} }
  if (Config.DEBUG && surfaceList_.Size() > 0 && paintRects_.Count > 0) {
   SurfaceBase surfaceBase = surfaceList_.FirstElement();
   try {
    g.Mark(surfaceBase.value + 0);
    g.Mark(surfaceBase.value + 1);
    g.Mark(surfaceBase.value + 2);
    g.Mark(surfaceBase.value + 3);
    g.Mark(surfaceBase.value + 4);
    g.Mark(surfaceBase.value + 5);
    g.Mark(surfaceBase.value + 6);
    g.Mark(surfaceBase.value + 7);
    g.Mark(surfaceBase.value + 8);
    g.Mark(surfaceBase.value + 9);
    g.Mark(surfaceBase.value + 10);
    g.Mark(surfaceBase.value + 11);
    g.Mark(surfaceBase.value + 12);
    g.Mark(surfaceBase.value + 13);
    g.Mark(surfaceBase.value + 14);
    g.Mark(surfaceBase.value + 15);
    g.Mark(surfaceBase.value + 16);
    g.Mark(surfaceBase.value + 17);
    g.Mark(surfaceBase.value + 18);
    g.Mark(surfaceBase.value + 19);
    g.Mark(surfaceBase.value + 20);
    g.Mark(surfaceBase.value + 21);
    g.Mark(surfaceBase.value + 22);
    g.Mark(surfaceBase.value + 23);
    g.Mark(surfaceBase.value + 24);
    g.Mark(surfaceBase.value + 25);
    g.Mark(surfaceBase.value + 26);
    g.Mark(surfaceBase.value + 27);
    g.Mark(surfaceBase.value + 28);
    g.Mark(surfaceBase.value + 29);
    g.Mark(surfaceBase.value + 30);
    g.Mark(surfaceBase.value + 31);
    g.Mark(surfaceBase.value + 32);
    g.Mark(surfaceBase.value + 33);
    g.Mark(surfaceBase.value + 34);
    g.Mark(surfaceBase.value + 35);
    g.Mark(surfaceBase.value + 36);
    g.Mark(surfaceBase.value + 37);
    g.Mark(surfaceBase.value + 38);
    g.Mark(surfaceBase.value + 39);
   } finally { g.Mark(-1); }
  }
 }
 // Heavy tail deliberately makes long branches observable. No measured result.
 // Negative proposals: guard includes an effect/assignment; non-Int32 comparison;
 // capture/goto entering the moved tail; body after guard changes a leaf.
}
