using System.Collections.Generic;
class Image { public bool IsDisposed; }
class Material { public int NotAccessFrame; public bool UntiColor; public void Dispose(){NotAccessFrame=-1;} }
class Fixture {
 Dictionary<Image,Dictionary<string,Material>> materlas_ = new Dictionary<Image,Dictionary<string,Material>>();
 Image empty_;
 public void Run() {
  List<Image> list = null;
  foreach (Image key in materlas_.Keys) {
   if (key != null && key != empty_ && key.IsDisposed) {
    foreach (KeyValuePair<string, Material> item in materlas_[key]) { item.Value.Dispose(); }
    if (list == null) { list = new List<Image>(); }
    list.Add(key);
    continue;
   }
   Dictionary<string, Material> dictionary = materlas_[key];
   string text = null;
   foreach (KeyValuePair<string, Material> item2 in dictionary) {
    item2.Value.NotAccessFrame++;
    if (item2.Value.NotAccessFrame > 50) {
     bool flag = true;
     if (item2.Value.UntiColor && item2.Value.NotAccessFrame < 500) { flag = false; }
     if (flag) { item2.Value.Dispose(); text = item2.Key; break; }
    }
   }
   if (text != null) { dictionary.Remove(text); }
  }
  if (list == null) { return; }
  foreach (Image item3 in list) { materlas_.Remove(item3); }
 }
}
