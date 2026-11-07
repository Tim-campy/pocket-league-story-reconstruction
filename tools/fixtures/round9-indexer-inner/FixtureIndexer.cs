// SOURCE TEMPLATE ONLY: NOT_TESTED / NOT_COMPILED.
using System.Collections.Generic;
public sealed class FixtureMaterialIndexerInner {
 public sealed class Image { }
 public sealed class Shader { }
 public sealed class Material {
  public string key_;
  public int NotAccessFrame;
  public Material(Shader shader, Image image) { }
 }
 readonly Dictionary<Image,Dictionary<string,Material>> materlas_ = new Dictionary<Image,Dictionary<string,Material>>();
 static void Mark(Material material) { material.NotAccessFrame = 0; }
 public Material CurrentGraphics(Shader shader, Image image, bool mark) {
  Image key = image;
  string text = "fixture";
  Dictionary<string, Material> value;
  if(!materlas_.TryGetValue(key, out value)) {
   value = (materlas_[key] = new Dictionary<string, Material>());
  }
  Material value2 = null;
  if(!value.TryGetValue(text, out value2)) {
   value2 = (value[text] = new Material(shader, image));
   value2.key_ = text;
   if(mark) { Mark(value2); }
  }
  value2.NotAccessFrame = 0;
  return value2;
 }
 public Material ExpectedGraphics(Shader shader, Image image, bool mark) {
  Image key = image;
  string text = "fixture";
  Dictionary<string, Material> value;
  if(!materlas_.TryGetValue(key, out value)) {
   materlas_[key] = (value = new Dictionary<string, Material>());
  }
  Material value2 = null;
  if(!value.TryGetValue(text, out value2)) {
   value[text] = (value2 = new Material(shader, image));
   value2.key_ = text;
   if(mark) { Mark(value2); }
  }
  value2.NotAccessFrame = 0;
  return value2;
 }
 public Material CurrentShader(Shader shader, Image image, params object[] args) {
  Image key = image;
  string text = "fixture";
  Dictionary<string, Material> value;
  if(!materlas_.TryGetValue(key, out value)) {
   value = (materlas_[key] = new Dictionary<string, Material>());
  }
  for(int i = 0; i < args.Length; i++) { text = text + ":" + args[i]; }
  Material value2 = null;
  if(value.TryGetValue(text, out value2)) {
   value2.NotAccessFrame = 0;
   return value2;
  }
  value2 = (value[text] = new Material(shader, image));
  value2.key_ = text;
  for(int j = 0; j < args.Length; j += 2) {
   string propertyName = (string)args[j];
   object obj = args[j + 1];
   if(obj is int) { value2.NotAccessFrame += propertyName.Length + (int)obj; }
  }
  return value2;
 }
 public Material ExpectedShader(Shader shader, Image image, params object[] args) {
  Image key = image;
  string text = "fixture";
  Dictionary<string, Material> value;
  if(!materlas_.TryGetValue(key, out value)) {
   materlas_[key] = (value = new Dictionary<string, Material>());
  }
  for(int i = 0; i < args.Length; i++) { text = text + ":" + args[i]; }
  Material value2 = null;
  if(value.TryGetValue(text, out value2)) {
   value2.NotAccessFrame = 0;
   return value2;
  }
  value[text] = (value2 = new Material(shader, image));
  value2.key_ = text;
  for(int j = 0; j < args.Length; j += 2) {
   string propertyName = (string)args[j];
   object obj = args[j + 1];
   if(obj is int) { value2.NotAccessFrame += propertyName.Length + (int)obj; }
  }
  return value2;
 }
 // Proposed independent controls for BOTH methods:
 // - Swap dictionary/local assignment only.
 // - Swap material/local assignment only.
 // - Keep both Current assignment forms unchanged.
 // Proposed negatives for the calculator: capture value/value2 in a delegate;
 // finally observe the named variable on a throwing setter; change exact stable
 // local index to a property/call; omit one of the two paired assignment sites.
 // A custom side-effecting indexer setter is outside the narrow Dictionary
 // proof and MUST NOT be treated as generally semantics-preserving.
 // No compilation or full-IL/local-signature comparison has run here.
}
