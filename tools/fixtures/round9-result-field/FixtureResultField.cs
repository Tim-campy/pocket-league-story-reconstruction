// NOT_TESTED / NOT_COMPILED, pure source template, no transformation ran.
using System.Collections.Generic;
public sealed class FixtureRenderPassResults {
 public struct Color { public int Value; }
 public sealed class Graphics { }
 public sealed class MaterialStub { }
 Graphics g_;
 int RenderOperator, RenderSrcRatio, RenderDstRatio;
 MaterialStub Material;
 Color[] Colors, resultColors_;
 static readonly Color[] DefaultColors = new Color[0];
 static readonly Color[] OneColors = new Color[1];
 static readonly Dictionary<int,Color[]> ResultColors = new Dictionary<int,Color[]>();
 public void CurrentMaterial(Graphics g, MaterialStub material, params Color[] colors) {
  g_ = g;
  RenderOperator = 0;
  RenderSrcRatio = 255;
  RenderDstRatio = 0;
  Material = material;
  if(colors.Length == 0) { Colors = DefaultColors; }
  else if(colors.Length == 1) { Colors = OneColors; OneColors[0] = colors[0]; }
  else { Colors = colors; }
  if (!ResultColors.TryGetValue(Colors.Length, out resultColors_)) {
   Color[] array = (ResultColors[Colors.Length] = new Color[Colors.Length]);
   resultColors_ = array;
  }
 }
 public void ExpectedMaterial(Graphics g, MaterialStub material, params Color[] colors) {
  g_ = g;
  RenderOperator = 0;
  RenderSrcRatio = 255;
  RenderDstRatio = 0;
  Material = material;
  if(colors.Length == 0) { Colors = DefaultColors; }
  else if(colors.Length == 1) { Colors = OneColors; OneColors[0] = colors[0]; }
  else { Colors = colors; }
  if (!ResultColors.TryGetValue(Colors.Length, out resultColors_)) {
   resultColors_ = (ResultColors[Colors.Length] = new Color[Colors.Length]);
  }
 }
 public void CurrentOperators(Graphics g, int renderOperator, int renderSrcRatio, int renderDstRatio, int colorRatio, int alphaRatio, params Color[] colors) {
  g_ = g;
  RenderOperator = renderOperator;
  RenderSrcRatio = renderSrcRatio;
  RenderDstRatio = renderDstRatio;
  Colors = ((colors.Length == 0) ? DefaultColors : colors);
  Material = null;
  if(colors.Length == 0) { Colors = DefaultColors; }
  else if(colors.Length == 1) { Colors = OneColors; OneColors[0] = colors[0]; }
  else { Colors = colors; }
  if (!ResultColors.TryGetValue(Colors.Length, out resultColors_)) {
   Color[] array = (ResultColors[Colors.Length] = new Color[Colors.Length]);
   resultColors_ = array;
  }
 }
 public void ExpectedOperators(Graphics g, int renderOperator, int renderSrcRatio, int renderDstRatio, int colorRatio, int alphaRatio, params Color[] colors) {
  g_ = g;
  RenderOperator = renderOperator;
  RenderSrcRatio = renderSrcRatio;
  RenderDstRatio = renderDstRatio;
  Colors = ((colors.Length == 0) ? DefaultColors : colors);
  Material = null;
  if(colors.Length == 0) { Colors = DefaultColors; }
  else if(colors.Length == 1) { Colors = OneColors; OneColors[0] = colors[0]; }
  else { Colors = colors; }
  if (!ResultColors.TryGetValue(Colors.Length, out resultColors_)) {
   resultColors_ = (ResultColors[Colors.Length] = new Color[Colors.Length]);
  }
 }
 // Proposed negative controls, NOT executed:
 // 1. Use array in another call/assignment after resultColors_=array: reject.
 // 2. Put an extra effect between the two target statements: reject.
 // 3. Capture array in a delegate or observe it from a protected finally: reject.
 // 4. Replace this stable field/indexer proof with an arbitrary property receiver.
 // Current must retain two real Color[] slots versus Expected's one synthesized
 // temp, and the original this receiver must move BEFORE setter operands.
 // Those are hypotheses, not measured fixture results; compare full IL, actual
 // signature and maxstack with the historical compiler, never only byte counts.
}
