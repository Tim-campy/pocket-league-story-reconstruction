// NOT_TESTED / NOT_COMPILED own source template.
public sealed class FixtureOffscreenTwoGuards {
 public sealed class Image { public enum Format { A } public enum Filter { A } }
 public delegate void LayoutMethod(out int width,out int height,out Image.Format format,out Image.Filter filterMode);
 Image image_; bool dispose_; LayoutMethod GetLayout; int Effects;
 void Resize(int width,int height,Image.Format format,Image.Filter filterMode){Effects+=width+height;}
 public void Current() {
  if (image_ != null && !dispose_ && GetLayout != null) {
   int width;
   int height;
   Image.Format format;
   Image.Filter filterMode;
   GetLayout(out width, out height, out format, out filterMode);
   Resize(width, height, format, filterMode);
  }
 }
 public void Expected() {
  if(image_ == null || dispose_) { return; }
  if(GetLayout == null) { return; }
  int width;
  int height;
  Image.Format format;
  Image.Filter filterMode;
  GetLayout(out width, out height, out format, out filterMode);
  Resize(width, height, format, filterMode);
 }
 // Controls to distinguish old failed forms: ONE combined OR early-return
 // guard; nested positive AND ifs. Full Expected is TWO successive return sites.
 // Negative proposals: extra payload effect or statement before/after the sole
 // positive branch; changed delegate receiver/index; capture/goto.
}
