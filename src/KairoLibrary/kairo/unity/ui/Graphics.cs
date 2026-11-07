using System;
using System.Collections.Generic;
using java.lang;
using java.util;
using kairo.common.cfg;
using kairo.unity.graphics;
using kairo.unity.m3d4.util3d;
using kairo.unity.math;
using kairo.unity.native;
using kairo.unity.surface;
using kairo.unity.util;
using UnityEngine;

namespace kairo.unity.ui
{
	public class Graphics
	{
		private class GLText
		{
			public Rect Position;

			public string Text;

			public UnityEngine.Font Font;

			public int Size;

			public FontStyle Style;

			public Color Color;

			public Rectangle Clip = new Rectangle();

			public bool IsClip;

			public void Init(Rect pos, string text, GUIStyle style, Rectangle clip)
			{
				Position = pos;
				Text = text;
				Font = style.font;
				Size = style.fontSize;
				Style = style.fontStyle;
				Color = style.normal.textColor;
				IsClip = clip != null;
				if (clip != null)
				{
					Clip.left = clip.left;
					Clip.top = clip.top;
					Clip.bottom = clip.bottom;
					Clip.right = clip.right;
				}
			}
		}

		private class StackState
		{
			public int Clip;

			public int Font;

			public int RenderMode;

			public int Matrix;

			public int BlendMode;

			public int ClipEnable;

			public int Origin;

			public int RenderQueue;

			public int ImageClip;
		}

		protected class RenderPass
		{
			private Graphics g_;

			private static Dictionary<int, Color[]> ResultColors = new Dictionary<int, Color[]>();

			private static Color[] DefaultColors = new Color[1] { Color.white };

			private Color[] OneColors = new Color[1];

			public int RenderOperator;

			public int RenderSrcRatio;

			public int RenderDstRatio;

			public Color[] Colors;

			public kairo.unity.graphics.Material Material;

			public int opaqueTransOrder_;

			private bool pushBlendColor_;

			private int colorRatio_;

			private int alphaRatio_;

			private Color[] resultColors_;

			public void Init(Graphics g, kairo.unity.graphics.Material material, Color color)
			{
				OneColors[0] = color;
				Init(g, material, OneColors);
			}

			public void Init(Graphics g, kairo.unity.graphics.Material material, params Color[] colors)
			{
				g_ = g;
				RenderOperator = 0;
				RenderSrcRatio = 255;
				RenderDstRatio = 0;
				pushBlendColor_ = false;
				colorRatio_ = 255;
				alphaRatio_ = 255;
				Material = material;
				opaqueTransOrder_ = -1;
				if (colors.Length == 0)
				{
					Colors = DefaultColors;
				}
				else if (colors.Length == 1)
				{
					Colors = OneColors;
					OneColors[0] = colors[0];
				}
				else
				{
					Colors = colors;
				}
				if (!ResultColors.TryGetValue(Colors.Length, out resultColors_))
				{
					Color[] array = (ResultColors[Colors.Length] = new Color[Colors.Length]);
					resultColors_ = array;
				}
			}

			public void Init(Graphics g, int renderOperator, int renderSrcRatio, int renderDstRatio, int colorRatio, int alphaRatio, Color color)
			{
				OneColors[0] = color;
				Init(g, renderOperator, renderSrcRatio, renderDstRatio, colorRatio, alphaRatio, OneColors);
			}

			public void Init(Graphics g, int renderOperator, int renderSrcRatio, int renderDstRatio, int colorRatio, int alphaRatio, params Color[] colors)
			{
				g_ = g;
				RenderOperator = renderOperator;
				RenderSrcRatio = renderSrcRatio;
				RenderDstRatio = renderDstRatio;
				Colors = ((colors.Length == 0) ? DefaultColors : colors);
				pushBlendColor_ = false;
				colorRatio_ = colorRatio;
				alphaRatio_ = alphaRatio;
				Material = null;
				opaqueTransOrder_ = -1;
				if (colors.Length == 0)
				{
					Colors = DefaultColors;
				}
				else if (colors.Length == 1)
				{
					Colors = OneColors;
					OneColors[0] = colors[0];
				}
				else
				{
					Colors = colors;
				}
				if (!ResultColors.TryGetValue(Colors.Length, out resultColors_))
				{
					Color[] array = (ResultColors[Colors.Length] = new Color[Colors.Length]);
					resultColors_ = array;
				}
			}

			public void Copy(RenderPass from)
			{
				g_ = from.g_;
				RenderOperator = from.RenderOperator;
				RenderSrcRatio = from.RenderSrcRatio;
				RenderDstRatio = from.RenderDstRatio;
				Colors = from.Colors;
				Material = from.Material;
				opaqueTransOrder_ = from.opaqueTransOrder_;
				pushBlendColor_ = from.pushBlendColor_;
				colorRatio_ = from.colorRatio_;
				alphaRatio_ = from.alphaRatio_;
				resultColors_ = from.resultColors_;
			}

			private void _begin()
			{
				if (g_.IsRenderGUIMode())
				{
					GL.PushMatrix();
					GL.LoadPixelMatrix();
					GL.Viewport(new Rect(0f, 0f, UnityEngine.Screen.width, UnityEngine.Screen.height));
				}
				if (Material != null)
				{
					return;
				}
				if (RenderOperator != -1)
				{
					g_.PushRenderMode(RenderOperator, RenderSrcRatio, RenderDstRatio);
				}
				if (RenderSrcRatio == 0 && RenderDstRatio < 255)
				{
					pushBlendColor_ = g_.blendMode_ == 1;
					if (pushBlendColor_)
					{
						g_.PushBlendMode(0);
					}
				}
			}

			public Color[] Begin()
			{
				_begin();
				for (int i = 0; i < resultColors_.Length; i++)
				{
					resultColors_[i].a = Colors[i].a * (float)alphaRatio_ / 255f;
					resultColors_[i].r = Colors[i].r * (float)colorRatio_ / 255f;
					resultColors_[i].g = Colors[i].g * (float)colorRatio_ / 255f;
					resultColors_[i].b = Colors[i].b * (float)colorRatio_ / 255f;
					if (g_.blendMode_ == 2)
					{
						resultColors_[i].r = g_.blendColor_.r * g_.blendColor_.a * resultColors_[i].r;
						resultColors_[i].g = g_.blendColor_.g * g_.blendColor_.a * resultColors_[i].g;
						resultColors_[i].b = g_.blendColor_.b * g_.blendColor_.a * resultColors_[i].b;
					}
				}
				return resultColors_;
			}

			public Color[] BeginShape()
			{
				_begin();
				for (int i = 0; i < resultColors_.Length; i++)
				{
					resultColors_[i] = Colors[i];
					if (g_.blendMode_ == 1)
					{
						resultColors_[i].r = g_.blendColor_.r * g_.blendColor_.a + resultColors_[i].r * (1f - g_.blendColor_.a);
						resultColors_[i].g = g_.blendColor_.g * g_.blendColor_.a + resultColors_[i].g * (1f - g_.blendColor_.a);
						resultColors_[i].b = g_.blendColor_.b * g_.blendColor_.a + resultColors_[i].b * (1f - g_.blendColor_.a);
					}
					resultColors_[i].a = resultColors_[i].a * (float)alphaRatio_ / 255f;
					resultColors_[i].r = resultColors_[i].r * (float)colorRatio_ / 255f;
					resultColors_[i].g = resultColors_[i].g * (float)colorRatio_ / 255f;
					resultColors_[i].b = resultColors_[i].b * (float)colorRatio_ / 255f;
					if (g_.blendMode_ == 2)
					{
						resultColors_[i].r = g_.blendColor_.r * g_.blendColor_.a * resultColors_[i].r;
						resultColors_[i].g = g_.blendColor_.g * g_.blendColor_.a * resultColors_[i].g;
						resultColors_[i].b = g_.blendColor_.b * g_.blendColor_.a * resultColors_[i].b;
					}
				}
				return resultColors_;
			}

			public void End()
			{
				if (g_.IsRenderGUIMode())
				{
					GL.PopMatrix();
				}
				if (Material == null)
				{
					if (RenderOperator != -1)
					{
						g_.PopRenderMode();
					}
					if (pushBlendColor_)
					{
						g_.PopBlendMode();
					}
				}
			}
		}

		private class RenderCache
		{
			public Image Image;

			public int AlphaType;

			public float[] Source = new float[4];

			public RectangleF SourceRect = new RectangleF();

			public bool Valid;

			public MeshBuilder[] Meshes = new MeshBuilder[5];

			public Color[] Colors = new Color[5];

			public int Count;

			public bool Empty = true;

			public void Reset()
			{
				Empty = true;
			}
		}

		private class GrayscaleImage
		{
			public Image Source;

			public Image Image;

			public int X;

			public int Y;

			public int Width;

			public int Height;
		}

		public const int AQUA = -16711681;

		public const int BLACK = -16777216;

		public const int BLUE = -16776961;

		public const int FUCHSIA = -65281;

		public const int GRAY = -8355712;

		public const int GREEN = -16744448;

		public const int LIME = -16711936;

		public const int MAROON = -8388608;

		public const int NAVY = -16777088;

		public const int OLIVE = -8355840;

		public const int PURPLE = -8388480;

		public const int RED = -65536;

		public const int SILVER = -4144960;

		public const int TEAL = -16744320;

		public const int WHITE = -1;

		public const int YELLOW = -256;

		public const int FLIP_NONE = 0;

		public const int FLIP_HORIZONTAL = 1;

		public const int FLIP_VERTICAL = 2;

		public const int FLIP_ROTATE = 3;

		public const int FLIP_ROTATE_LEFT = 4;

		public const int FLIP_ROTATE_RIGHT = 5;

		public const int ANCHOR_LEFT = 1;

		public const int ANCHOR_CENTER = 2;

		public const int ANCHOR_RIGHT = 4;

		public const int ANCHOR_TOP = 16;

		public const int ANCHOR_MIDDLE = 32;

		public const int ANCHOR_BOTTOM = 64;

		public const int OP_REPL = 0;

		public const int OP_ADD = 1;

		public const int OP_SUB = 2;

		public const int BLEND_NONE = 0;

		public const int BLEND_COLOR = 1;

		public const int BLEND_LIGHT = 2;

		public const int BLEND_GRAYSCALE = 3;

		protected static GUIStyle defaultGuiStyle_;

		private Canvas canvas_;

		internal Image render_;

		internal Camera camera_;

		protected int[] renderRect_ = new int[4];

		protected int[] renderRectOrigin_ = new int[4];

		protected int width_;

		protected int height_;

		protected int flipmode_;

		protected bool linearFilter_;

		protected float[] drawString_buf;

		protected GUIStyle guiStyle_ = new GUIStyle();

		protected int color_;

		protected float scale_ = 100f;

		protected int transX_ = 1;

		protected int transY_ = 1;

		protected Matrix backupMatrix_ = new Matrix();

		protected Matrix mtxTmp_ = new Matrix();

		private bool isGUI_;

		private int meshId_;

		internal MeshManager[] meshManList_ = new MeshManager[1];

		internal Offscreen offscreen_;

		public bool glRender_;

		public int glRenderCount_;

		public Matrix4x4 glMatrix_;

		private Mesh clearMesh_;

		internal int cameraDepth_;

		private static bool imageClipWarning_;

		private float[] imageClip_ = new float[4];

		private int imageClipCount_;

		private List<float[]> imageClipStack_ = new List<float[]>();

		private int glTextCount_;

		private List<GLText> glTextList_ = new List<GLText>();

		protected int renderModeCount_;

		protected List<int[]> renderModeStack_ = new List<int[]>();

		internal int renderOperator_;

		internal int renderSrcRatio_;

		internal int renderDstRatio_;

		internal bool renderIsReplace_;

		protected int blendModeCount_;

		protected List<int[]> blendModeStack_ = new List<int[]>();

		internal int blendMode_;

		internal int blendRgba_;

		internal Color blendColor_;

		protected int clipCount_;

		protected float[] clipRect_ = new float[4];

		protected List<float[]> clipStack_ = new List<float[]>();

		protected Rectangle clipRegion_ = new Rectangle();

		protected Rectangle clipRegionGL_ = new Rectangle();

		protected bool isClipRegionGL;

		protected int clipEnableCount_;

		protected List<bool> clipEnableStack_ = new List<bool>();

		internal bool clipEnable_;

		internal bool setFigureClip_;

		internal bool useFigureClip_;

		internal bool isFigureClipMask_;

		private static int clipWarning_;

		protected Font font_;

		protected Font backupFont_;

		protected int fontCount_;

		protected List<Font> fontStack_ = new List<Font>();

		protected string localizedFormat_;

		protected static Rectangle guiGroupRect_ = new Rectangle();

		protected static bool isGuiGroupRect_;

		internal static bool guiBeginGroup_;

		protected Matrix worldMatrix_ = new Matrix();

		protected Matrix[][] localMatrixStack_ = new Matrix[0][];

		protected int localMatrixIndex_ = -1;

		protected Matrix screenMatrix_ = new Matrix();

		protected int backupWorldMatrixCount_;

		protected List<Matrix> backupWorldMatrix_ = new List<Matrix>();

		protected Matrix localCoordinateMatrix_ = new Matrix();

		protected int localCoordinateCount_;

		public kairo.unity.graphics.Material renderForceMaterial_;

		private static MaterialManager mateMan_;

		private static bool autoTextLayout_;

		[ThreadStatic]
		private static List<bool> autoTextLayoutStack_;

		[ThreadStatic]
		private static int autoTextLayoutCount_;

		protected int originX_;

		protected int originY_;

		protected int originCount_;

		protected List<int[]> originStack_ = new List<int[]>();

		protected int renderQueue_;

		protected int renderQueueCount_;

		protected List<int> renderQueueStack_ = new List<int>();

		internal static int DepthBits = 0;

		internal static Graphics current_;

		private static int[] addIndiciesBuffer_ = new int[24];

		private static readonly string[] NUMBERS = new string[10] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

		internal static float outlineWidth_ = 1f;

		internal static int outlineQuality_;

		private static int lineSpacing_;

		private static float imageExpand_ = 0.2f;

		internal bool fontScaleOffsetEnable_ = true;

		private List<StackState> stackState_ = new List<StackState>();

		private int stackStateCount_;

		private RenderCache renderCache_ = new RenderCache();

		private static bool renderCacheEnable_ = true;

		private List<GrayscaleImage> grayscaleImages_ = new List<GrayscaleImage>();

		private static RectangleF SetClip_tmp = new RectangleF();

		private static RectangleF ClipRect_tmp = new RectangleF();

		private int[] getClip_tmp = new int[4];

		private static float[][] drawArcPoints = JTool.MakeArray<float[][]>(new int[2] { 4, 2 });

		private static Color[] DrawPolygon_ColorBuffer = new Color[0];

		private static Vector3[] DrawPolygon_VertexBuffer = new Vector3[0];

		private static Vector2[] DrawPolygon_TexCoordBuffer = new Vector2[0];

		private static float[] DrawPolygon_Pt = new float[2];

		private static float[] DrawImageRepeat_Verticies = new float[12];

		private static float[] DrawImageRepeat_TexCoords = new float[8];

		private static int[] DrawImageRepeat_Indicies = new int[6] { 0, 1, 2, 0, 2, 3 };

		private static RectangleF drawImageRect_ = new RectangleF();

		private static RectangleF _drawBitmap_rc1 = new RectangleF();

		private static RectangleF _drawBitmap_rc2 = new RectangleF();

		private static RectangleF _drawBitmap_rc3 = new RectangleF();

		private static RectangleF _drawBitmap_rc4 = new RectangleF();

		private static Vector3[] _drawBitmap_vertices = new Vector3[4];

		private static Color[] _drawBitmap_colros;

		private static RectangleF _drawBitmapSharpnessRect = new RectangleF();

		private static Matrix _drawBitmapMatrix = new Matrix();

		private static Font _drawBitmapFont = new Font();

		private static Vector3[] DrawLine_Vertices = new Vector3[4];

		private static Vector3[] DrawRect_Vertices1 = new Vector3[4];

		private static Vector3[] DrawRect_Vertices2 = new Vector3[4];

		private static RectangleF DrawString_tmp = new RectangleF();

		private static float[] DrawString_tmp2 = new float[2];

		private static float[][] fillArcPoints_ = JTool.MakeArray<float[][]>(new int[2] { 3, 2 });

		private static Vector3[] FillRect_Vertices = new Vector3[4];

		private static float[] FillTriangle_tmp1 = new float[2];

		private static float[] FillTriangle_tmp2 = new float[2];

		private static float[] FillTriangle_tmp3 = new float[2];

		private static int[] getAnchorPosition_tmp = new int[2];

		private static float[] getAnchorPositionF_tmp = new float[2];

		private int[] getRenderRect_tmp = new int[4];

		private static int[] getSharpnessInfo_result = new int[4];

		private static Color[] GetRenderPass_tmp;

		private static Color[] GetRenderPass_color = new Color[1];

		private static RenderPass[][] GetRenderPass_Buffer;

		public Graphics()
		{
			meshManList_[0] = new MeshManager(this);
			isGUI_ = true;
			Init(null, null);
		}

		public Graphics(Image img, Camera camera = null)
		{
			meshManList_[0] = new MeshManager(this);
			Init(null, img, camera);
		}

		public Graphics(Offscreen offscreen, bool dynamic = true)
		{
			if (dynamic)
			{
				offscreen_ = offscreen;
				meshManList_[0] = new MeshManager(this, offscreen);
				Init(null, offscreen.Image, offscreen_.GetComponent<Camera>());
			}
			else
			{
				meshManList_[0] = new MeshManager(this, offscreen, false);
				Init(null, offscreen.Image, offscreen.GetComponent<Camera>());
			}
		}

		internal void Init(object g, Image img, Camera camera = null)
		{
			canvas_ = Canvas.GetInstance();
			mateMan_ = MaterialManager.GetInstance();
			render_ = img;
			camera_ = camera;
			float scaleRatio = IApplication.GetCurrentApp().GetScaleRatio();
			if (drawString_buf == null)
			{
				drawString_buf = new float[1024];
			}
			if (img != null)
			{
				width_ = img.GetWidth();
				height_ = img.GetHeight();
			}
			else if (camera_ != null)
			{
				width_ = (int)((float)camera_.pixelWidth + 0.1f);
				height_ = (int)((float)camera_.pixelHeight + 0.1f);
			}
			else
			{
				width_ = DisplayMetrics.WidthPixels;
				height_ = DisplayMetrics.HeightPixels;
			}
			if (camera_ == null)
			{
				camera_ = Camera.main;
			}
			renderRect_[0] = 0;
			renderRect_[1] = 0;
			renderRect_[2] = width_;
			renderRect_[3] = height_;
			if (font_ == null)
			{
				font_ = new Font();
			}
			if (backupFont_ == null)
			{
				backupFont_ = new Font();
			}
			font_.Init();
			SetFont(font_);
			linearFilter_ = scaleRatio % 100f != 0f;
			linearFilter_ = true;
			Scale(100f);
			SetRenderMode(0, 255, 0);
			localMatrixIndex_ = -1;
		}

		public object GetGraphics()
		{
			return null;
		}

		private bool CheckClip()
		{
			if (clipEnable_)
			{
				return true;
			}
			if (clipWarning_ == 0 && Config.PRINT)
			{
				Log.Info("クリップが無効です");
			}
			clipWarning_++;
			return false;
		}

		private void _setClip(float x, float y, float w, float h)
		{
			CheckClip();
			clipRect_[0] = x;
			clipRect_[1] = y;
			clipRect_[2] = w;
			clipRect_[3] = h;
			clipRegion_.Set((int)(x + 0.01f), (int)(y + 0.01f), (int)(x + w + 0.01f), (int)(y + h + 0.01f));
			renderCache_.Reset();
		}

		public void PushClip()
		{
			while (clipCount_ >= clipStack_.Count)
			{
				clipStack_.Add(new float[4]);
			}
			float[] array = clipStack_[clipCount_];
			array[0] = clipRect_[0];
			array[1] = clipRect_[1];
			array[2] = clipRect_[2];
			array[3] = clipRect_[3];
			clipCount_++;
		}

		public void PushClip(float x, float y, float w, float h)
		{
			PushClip();
			SetClip(x, y, w, h);
		}

		public void PopClip()
		{
			if (this.clipCount_ <= 0)
			{
				return;
			}
			float[] item = this.clipStack_[this.clipCount_ - 1];
			this.clipCount_--;
			this._setClip(item[0], item[1], item[2], item[3]);
		}

		public void SetClip(float x, float y, float w, float h)
		{
			BeginRenderMatrix(ref x, ref y);
			try
			{
				RectangleF transRect = GetTransRect(x, y, w, h, SetClip_tmp);
				_setClip(transRect.left, transRect.top, transRect.Width(), transRect.Height());
			}
			finally
			{
				EndRenderMatrix();
			}
		}

		public void ClipRect(float x, float y, float width, float height, bool push = true)
		{
			if (push)
			{
				PushClip();
			}
			BeginRenderMatrix(ref x, ref y);
			try
			{
				RectangleF transRect = GetTransRect(x, y, width, height, ClipRect_tmp);
				x = transRect.left;
				y = transRect.top;
				width = transRect.Width();
				height = transRect.Height();
				float[] array = clipRect_;
				float num = x + width;
				if (num > array[0] + array[2])
				{
					num = array[0] + array[2];
				}
				float num2 = y + height;
				if (num2 > array[1] + array[3])
				{
					num2 = array[1] + array[3];
				}
				if (x < array[0])
				{
					x = array[0];
				}
				if (y < array[1])
				{
					y = array[1];
				}
				width = num - x;
				height = num2 - y;
				if (width < 0f)
				{
					width = 0f;
				}
				if (height < 0f)
				{
					height = 0f;
				}
				_setClip(x, y, width, height);
			}
			finally
			{
				EndRenderMatrix();
			}
		}

		public void ClearClip()
		{
			_setClip(renderRect_[0], renderRect_[1], renderRect_[2], renderRect_[3]);
		}

		public void SetFigureClipBegin(bool clear = true)
		{
			if (!(CheckClip())) { return; }
if (offscreen_ != null)
				{
					offscreen_.Flush();
				}
if (glRender_)
				{
					FlushGLRender(true);
				}
setFigureClip_ = true;
isFigureClipMask_ = false;
GLClear(true, false, true);
if (!clear)
				{
					MaterialManager.GetInstance().GetMaterial(this, null, 1f, true).SetPass(0);
					GL.PushMatrix();
					GL.LoadPixelMatrix();
					GL.Viewport(new Rect(0f, 0f, UnityEngine.Screen.width, UnityEngine.Screen.height));
					GL.Begin(7);
					float num = clipRegion_.left;
					float num2 = clipRegion_.Width();
					float num3 = clipRegion_.top;
					float num4 = clipRegion_.Height();
					num3 = (float)UnityEngine.Screen.height - num3 - num4;
					GL.Vertex3(num, num3, 0f);
					GL.Vertex3(num, num3 + num4, 0f);
					GL.Vertex3(num + num2, num3 + num4, 0f);
					GL.Vertex3(num + num2, num3, 0f);
					GL.End();
					GL.PopMatrix();
				}
renderCache_.Reset();
		}

		public void SetFigureClipEnd()
		{
			if (!this.CheckClip())
			{
				return;
			}
			if (this.offscreen_ != null)
			{
				this.offscreen_.Flush();
			}
			if (this.glRender_)
			{
				this.FlushGLRender(true);
			}
			this.setFigureClip_ = false;
			this.useFigureClip_ = true;
			this.isFigureClipMask_ = false;
			this.renderCache_.Reset();
		}

		public void SetFigureClipMode(bool clip)
		{
			isFigureClipMask_ = !clip;
		}

		public void ClearFigureClip()
		{
			if (!this.CheckClip())
			{
				return;
			}
			this.useFigureClip_ = false;
			this.isClipRegionGL = false;
			this.renderCache_.Reset();
		}

		public int[] GetClip()
		{
			for (int i = 0; i < clipRect_.Length; i++)
			{
				getClip_tmp[i] = (int)clipRect_[i];
			}
			return getClip_tmp;
		}

		public virtual void ClearRect(float x, float y, float width, float height)
		{
			int color = GetColor();
			SetColor(canvas_.GetBackColor());
			FillRect(x, y, width, height);
			SetColor(color);
		}

		public void Dispose(bool keep = false)
		{
			if (!keep)
			{
				for (int i = (int)this.meshManList_.Length - 1; i >= 0; i--)
				{
					if (this.meshManList_[i] != null)
					{
						this.meshManList_[i].Clear();
					}
					this.meshManList_[i] = null;
				}
				this.meshManList_ = new MeshManager[1];
			}
		}

		public void DrawArc(float x, float y, float width, float height, float startAngle, float arcAngle, float lineWidth = 1f)
		{
			BeginRenderMatrix(ref x, ref y);
			try
			{
				if (arcAngle < -360f)
				{
					arcAngle = -360f;
				}
				if (arcAngle > 360f)
				{
					arcAngle = 360f;
				}
				startAngle %= 360f;
				if (startAngle < 0f)
				{
					startAngle += 360f;
				}
				if (arcAngle < 0f)
				{
					startAngle += arcAngle;
					arcAngle *= -1f;
				}
				UpdateGLClip();
				float num = width / 2f;
				float num2 = height / 2f;
				float num3 = width / 2f - lineWidth;
				float num4 = height / 2f - lineWidth;
				x += width / 2f;
				y += height / 2f;
				float num5 = 360f / (float)(int)((1f + (width + height) * 2f / 50f) * 8f + 8f);
				foreach (RenderPass renderPass2 in GetRenderPass(guiStyle_.normal.textColor))
				{
					Color color = renderPass2.BeginShape()[0];
					try
					{
						kairo.unity.graphics.Material material = ((renderPass2.Material != null) ? mateMan_.GetMaterial(renderPass2.Material, null) : mateMan_.GetMaterial(this, null, color.a));
						float num6 = startAngle + arcAngle;
						float num7 = startAngle;
						float[][] array = drawArcPoints;
						if (!IsRenderGUIMode())
						{
							MeshBuilder meshBuilder = meshManList_[meshId_].GetMeshBuilder(material);
							meshBuilder.ReadyAddition();
							float num8 = FastMath.Sin(num7);
							float num9 = FastMath.Cos(num7);
							GetTransPointGL(x + num9 * num3, y - num8 * num4, array[0]);
							GetTransPointGL(x + num9 * num, y - num8 * num2, array[1]);
							meshBuilder.AddVertex(array[0][0], array[0][1], 0f, color);
							meshBuilder.AddVertex(array[1][0], array[1][1], 0f, color);
							int num10 = 0;
							while (num7 < num6)
							{
								float num11 = num7 + num5;
								if (num10 == 0)
								{
									num11 = (float)(int)(num11 / num5) * num5 + num5;
									if (num11 < num7)
									{
										num11 += num5;
									}
								}
								if (num11 > num6)
								{
									num11 = num6;
								}
								float num12 = FastMath.Sin(num11);
								float num13 = FastMath.Cos(num11);
								GetTransPointGL(x + num13 * num3, y - num12 * num4, array[2]);
								GetTransPointGL(x + num13 * num, y - num12 * num2, array[3]);
								meshBuilder.AddVertex(array[2][0], array[2][1], 0f, color);
								meshBuilder.AddVertex(array[3][0], array[3][1], 0f, color);
								int num14 = num10 * 2;
								int[] array2 = addIndiciesBuffer_;
								array2[0] = num14 + 0;
								array2[1] = num14 + 1;
								array2[2] = num14 + 3;
								array2[3] = num14 + 0;
								array2[4] = num14 + 3;
								array2[5] = num14 + 2;
								meshBuilder.AddIndicies(array2, 6);
								num7 = num11;
								num10++;
							}
							continue;
						}
						material.SetPass(0);
						if (lineWidth == 0f)
						{
							GL.Begin(1);
						}
						else
						{
							GL.Begin(7);
						}
						GL.Color(color);
						for (int num15 = 0; num7 < num6; )
						{
							float num16 = num7 + num5;
							if (num15 == 0)
							{
								num16 = (float)(int)(num16 / num5) * num5 + num5;
								if (num16 < num7)
								{
									num16 += num5;
								}
							}
							if (num16 > num6)
							{
								num16 = num6;
							}
							if (num15 == 0)
							{
								float num17 = FastMath.Sin(num7);
								float num18 = FastMath.Cos(num7);
								GetTransPointGL(x + num18 * num3, y - num17 * num4, array[0]);
								GetTransPointGL(x + num18 * num, y - num17 * num2, array[1]);
							}
							else
							{
								Array.Copy(array[2], array[0], 2);
								Array.Copy(array[3], array[1], 2);
							}
							float num19 = FastMath.Sin(num16);
							float num20 = FastMath.Cos(num16);
							GetTransPointGL(x + num20 * num3, y - num19 * num4, array[2]);
							GetTransPointGL(x + num20 * num, y - num19 * num2, array[3]);
							GL.Vertex3(array[0][0], array[0][1], 0f);
							GL.Vertex3(array[2][0], array[2][1], 0f);
							GL.Vertex3(array[3][0], array[3][1], 0f);
							GL.Vertex3(array[1][0], array[1][1], 0f);
							num7 = num16;
							num15++;
						}
						GL.End();
					}
					finally
					{
						renderPass2.End();
					}
				}
				if (Config.COUNT_ENABLE)
				{
					Config.drawShapeCount_++;
				}
			}
			finally
			{
				EndRenderMatrix();
			}
		}

		public void DrawArc2(float x, float y, float width, float height, float startAngle, float arcAngle)
		{
			DrawArc(x, y, width - 1f, height - 1f, startAngle, arcAngle);
		}

		public void DrawChars(char[] data, float x, float y, int off, int len)
		{
			DrawString(new string(data, off, len), x, y);
		}

		public void DrawPolygons(Image image, float[] verticies, float[] texCoords, int[] colors, int[] indicies)
		{
			float num = verticies[0];
			float num2 = verticies[1];
			BeginRenderMatrix(ref verticies[0], ref verticies[1]);
			num = verticies[0] - num;
			num2 = verticies[1] - num2;
			for (int i = 3; i < verticies.Length; i += 3)
			{
				verticies[i + 0] += num;
				verticies[i + 1] += num2;
			}
			try
			{
				if (texCoords == null)
				{
					image = null;
				}
				if (image == null)
				{
					texCoords = null;
				}
				int num3 = verticies.Length / 3;
				if (DrawPolygon_TexCoordBuffer.Length < num3)
				{
					DrawPolygon_TexCoordBuffer = new Vector2[num3];
				}
				Vector2[] drawPolygon_TexCoordBuffer = DrawPolygon_TexCoordBuffer;
				if (texCoords != null)
				{
					int num4 = 0;
					int num5 = 0;
					while (num4 < num3)
					{
						drawPolygon_TexCoordBuffer[num4].x = texCoords[num5 + 0];
						drawPolygon_TexCoordBuffer[num4].y = texCoords[num5 + 1];
						num4++;
						num5 += 2;
					}
				}
				Texture texture = null;
				if (image != null)
				{
					ImageCheckUse(image);
					texture = image.GetImage();
					float num6 = texture.width;
					float num7 = texture.height;
					for (int j = 0; j < num3; j++)
					{
						drawPolygon_TexCoordBuffer[j].x = drawPolygon_TexCoordBuffer[j].x / num6;
						drawPolygon_TexCoordBuffer[j].y = (num7 - drawPolygon_TexCoordBuffer[j].y) / num7;
					}
				}
				if (DrawPolygon_VertexBuffer.Length < num3)
				{
					DrawPolygon_VertexBuffer = new Vector3[num3];
				}
				Vector3[] drawPolygon_VertexBuffer = DrawPolygon_VertexBuffer;
				float[] drawPolygon_Pt = DrawPolygon_Pt;
				for (int num8 = 0, num9 = 0; num8 < num3; )
				{
					GetTransPointGL(verticies[num9 + 0], verticies[num9 + 1], drawPolygon_Pt);
					drawPolygon_VertexBuffer[num8].x = drawPolygon_Pt[0];
					drawPolygon_VertexBuffer[num8].y = drawPolygon_Pt[1];
					drawPolygon_VertexBuffer[num8].z = verticies[num9 + 2];
					num8++;
					num9 += 3;
				}
				if (DrawPolygon_ColorBuffer.Length < num3)
				{
					DrawPolygon_ColorBuffer = new Color[num3];
				}
				Color[] drawPolygon_ColorBuffer = DrawPolygon_ColorBuffer;
				if (colors != null)
				{
					for (int k = 0; k < num3; k++)
					{
						drawPolygon_ColorBuffer[k] = ArgbToColor(colors[k]);
					}
				}
				else
				{
					Color color = ArgbToColor(GetColorOfRGB(255, 255, 255));
					if (image == null)
					{
						color = ArgbToColor(color_);
					}
					for (int l = 0; l < num3; l++)
					{
						drawPolygon_ColorBuffer[l] = color;
					}
				}
				if (image != null && image.renderQueue_ != 0)
				{
					PushRenderQueue(renderQueue_ + image.renderQueue_);
				}
				UpdateGLClip();
				RenderPass[] renderPass = GetRenderPass(image, drawPolygon_ColorBuffer);
				for (int recoveredPassIndex = 0; recoveredPassIndex < renderPass.Length; recoveredPassIndex++)
				{
RenderPass renderPass2 = renderPass[recoveredPassIndex];
					Color[] array = ((image != null) ? renderPass2.Begin() : renderPass2.BeginShape());
					try
					{
						kairo.unity.graphics.Material material = ((renderPass2.Material != null) ? mateMan_.GetMaterial(renderPass2.Material, image) : mateMan_.GetMaterial(this, image, array, array[0].a, false, renderPass2.opaqueTransOrder_, false, false, false));
						if (!IsRenderGUIMode())
						{
							MeshBuilder meshBuilder = meshManList_[meshId_].GetMeshBuilder(material);
							meshBuilder.ReadyAddition();
							if (texCoords == null)
							{
								for (int n = 0; n < num3; n++)
								{
									meshBuilder.AddVertex(drawPolygon_VertexBuffer[n].x, drawPolygon_VertexBuffer[n].y, drawPolygon_VertexBuffer[n].z, array[n]);
								}
							}
							else
							{
								for (int num10 = 0; num10 < num3; num10++)
								{
									meshBuilder.AddVertex(drawPolygon_VertexBuffer[num10].x, drawPolygon_VertexBuffer[num10].y, drawPolygon_VertexBuffer[num10].z, drawPolygon_TexCoordBuffer[num10].x, drawPolygon_TexCoordBuffer[num10].y, array[num10]);
								}
							}
							meshBuilder.AddIndicies(indicies);
							continue;
						}
						material.SetPass(0);
						GL.Begin(4);
						if (texCoords == null)
						{
							for (int recoveredIndexA = 0; recoveredIndexA < indicies.Length; recoveredIndexA++)
							{
int num12 = indicies[recoveredIndexA];
								GL.Color(array[num12]);
								GL.Vertex3(drawPolygon_VertexBuffer[num12].x, drawPolygon_VertexBuffer[num12].y, drawPolygon_VertexBuffer[num12].z);
							}
						}
						else
						{
							for (int recoveredIndexB = 0; recoveredIndexB < indicies.Length; recoveredIndexB++)
							{
int num14 = indicies[recoveredIndexB];
								GL.Color(array[num14]);
								GL.TexCoord2(drawPolygon_TexCoordBuffer[num14].x, drawPolygon_TexCoordBuffer[num14].y);
								GL.Vertex3(drawPolygon_VertexBuffer[num14].x, drawPolygon_VertexBuffer[num14].y, drawPolygon_VertexBuffer[num14].z);
							}
						}
						GL.End();
					}
					finally
					{
						renderPass2.End();
					}
				}
				if (image != null && image.renderQueue_ != 0)
				{
					PopRenderQueue();
				}
				if (Config.COUNT_ENABLE)
				{
					if (image != null)
					{
						Config.drawImageCount_++;
						for (int num15 = 0; num15 < indicies.Length; num15 += 3)
						{
							Vector3 vector = drawPolygon_TexCoordBuffer[indicies[num15 + 0]];
							Vector3 vector2 = drawPolygon_TexCoordBuffer[indicies[num15 + 1]];
							Vector3 vector3 = drawPolygon_TexCoordBuffer[indicies[num15 + 2]];
							Config.drawImageArea_ += (int)KairoMath.GetTriangleArea(vector.x * (float)texture.width, vector.y * (float)texture.height, vector2.x * (float)texture.width, vector2.y * (float)texture.height, vector3.x * (float)texture.width, vector3.y * (float)texture.height);
						}
						if (Config.checkCount_ != image.checkCount_)
						{
							Config.drawSourceImageArea_ += image.GetWidth() * image.GetHeight();
						}
					}
					else
					{
						Config.drawShapeCount_++;
					}
				}
				if (image != null)
				{
					image.checkCount_ = Config.checkCount_;
				}
			}
			finally
			{
				EndRenderMatrix();
			}
		}

		public void SetMaterial(kairo.unity.graphics.Material material)
		{
			renderForceMaterial_ = material;
			renderCache_.Reset();
		}

		private void DrawImageRepeat(Image image, float x, float y, int sx, int sy, int width, int height)
		{
			if (Config.PRINT && image == null)
			{
				Log.Info("[drawImage] image=null", null);
			}
			if (image == null)
			{
				return;
			}
			image.GetImage().wrapMode = TextureWrapMode.Repeat;
			float[] drawImageRepeatVerticies = kairo.unity.ui.Graphics.DrawImageRepeat_Verticies;
			float[] drawImageRepeatTexCoords = kairo.unity.ui.Graphics.DrawImageRepeat_TexCoords;
			int[] drawImageRepeatIndicies = kairo.unity.ui.Graphics.DrawImageRepeat_Indicies;
			drawImageRepeatVerticies[3] = (float)width;
			drawImageRepeatVerticies[4] = 0f;
			drawImageRepeatVerticies[6] = (float)width;
			drawImageRepeatVerticies[7] = (float)height;
			drawImageRepeatVerticies[9] = 0f;
			drawImageRepeatVerticies[10] = (float)height;
			drawImageRepeatTexCoords[0] = (float)sx;
			drawImageRepeatTexCoords[1] = (float)sy;
			drawImageRepeatTexCoords[2] = (float)(sx + width);
			drawImageRepeatTexCoords[3] = (float)sy;
			drawImageRepeatTexCoords[4] = (float)(sx + width);
			drawImageRepeatTexCoords[5] = (float)(sy + height);
			drawImageRepeatTexCoords[6] = (float)sx;
			drawImageRepeatTexCoords[7] = (float)(sy + height);
			this.DrawPolygons(image, drawImageRepeatVerticies, drawImageRepeatTexCoords, null, drawImageRepeatIndicies);
		}

		public void DrawImage(Image image, float x, float y)
		{
			if (Config.PRINT && image == null)
			{
				Log.Info("[drawImage] image=null");
			}
			if (image == null)
			{
				return;
			}
			if (image.optimizePass_ != 0)
			{
				int optimizeBlockWidth_ = image.optimizeBlockWidth_;
				int optimizeBlockHeight_ = image.optimizeBlockHeight_;
				for (int i = 0; i < image.optimizeHeightNum_; i++)
				{
					for (int j = 0; j < image.optimizeWidthNum_; j++)
					{
						_drawImage(image, x + (float)(j * optimizeBlockWidth_), y + (float)(i * optimizeBlockHeight_), optimizeBlockWidth_, optimizeBlockHeight_, j * optimizeBlockWidth_, i * optimizeBlockHeight_, optimizeBlockWidth_, optimizeBlockHeight_);
					}
				}
				return;
			}
			float num = image.GetWidth();
			float num2 = image.GetHeight();
			if (image.standardWidth_ > 0)
			{
				float num3 = (float)image.GetWidth() / (float)image.standardWidth_;
				num /= num3;
			}
			if (image.standardHeight_ > 0)
			{
				float num4 = (float)image.GetHeight() / (float)image.standardHeight_;
				num2 /= num4;
			}
			_drawImage(image, x, y, num, num2, 0, 0, image.GetWidth(), image.GetHeight());
		}

		public void DrawImage(Image image, float dx, float dy, int sx, int sy, int width, int height)
		{
			if (Config.PRINT && image == null)
			{
				Log.Info("[drawImage] image=null");
			}
			if (image != null)
			{
				_drawImage(image, dx, dy, width, height, sx, sy, width, height);
			}
		}

		public void DrawScaledImage(Image image, float dx, float dy, float width, float height, int sx, int sy, int swidth, int sheight)
		{
			if (Config.PRINT && image == null)
			{
				Log.Info("[drawImage] image=null");
			}
			if (image != null)
			{
				_drawImage(image, dx, dy, width, height, sx, sy, swidth, sheight);
			}
		}

		protected void _drawImage(Image image, float dx, float dy, float width, float height, int sx, int sy, float swidth, float sheight)
		{
			if (image == null)
			{
				return;
			}
			bool flag = !float.IsNaN(imageClip_[0]) && !float.IsNaN(imageClip_[2]);
			bool flag2 = !float.IsNaN(imageClip_[1]) && !float.IsNaN(imageClip_[3]);
			if ((flag || flag2) && image.optimizePass_ != 0 && image.optimizeSeb_ != null)
			{
				if (!imageClipWarning_ && Config.PRINT)
				{
					KairoPlugin.ShowToast("sebを使って最適化した画像に ImageClip は使えません。");
					imageClipWarning_ = true;
				}
				flag = false;
				flag2 = false;
			}
			if (flag)
			{
				float num = dx;
				float num2 = dx + width;
				if (num < imageClip_[0])
				{
					num = imageClip_[0];
				}
				if (imageClip_[0] + imageClip_[2] < num2)
				{
					num2 = imageClip_[0] + imageClip_[2];
				}
				float num3 = (num - dx) / width;
				float num4 = (num2 - dx) / width;
				float num5 = (float)sx + swidth * num3;
				float num6 = (float)sx + swidth * num4;
				float num7 = sx;
				float num8 = swidth;
				dx = num;
				width = num2 - num;
				sx = (int)num5;
				swidth = num6 - num5;
				if ((flipmode_ & 1) != 0)
				{
					sx = (int)(num7 + (num7 + num8) - ((float)sx + swidth));
				}
				if (width <= 0f || swidth <= 0f)
				{
					return;
				}
			}
			if (flag2)
			{
				float num9 = dy;
				float num10 = dy + height;
				if (num9 < imageClip_[1])
				{
					num9 = imageClip_[1];
				}
				if (imageClip_[1] + imageClip_[3] < num10)
				{
					num10 = imageClip_[1] + imageClip_[3];
				}
				float num11 = (num9 - dy) / height;
				float num12 = (num10 - dy) / height;
				float num13 = (float)sy + sheight * num11;
				float num14 = (float)sy + sheight * num12;
				float num15 = sy;
				float num16 = sheight;
				dy = num9;
				height = num10 - num9;
				sy = (int)num13;
				sheight = num14 - num13;
				if ((flipmode_ & 2) != 0)
				{
					sy = (int)(num15 + (num15 + num16) - ((float)sy + sheight));
				}
				if (height <= 0f || sheight <= 0f)
				{
					return;
				}
			}
			if (flipmode_ != 0)
			{
				if (flipmode_ == 4)
				{
					PushMatrix(Matrix.RotateTemporary(-90f, swidth / 2f, sheight / 2f));
				}
				else if (flipmode_ == 5)
				{
					PushMatrix(Matrix.RotateTemporary(-90f, swidth / 2f, sheight / 2f));
				}
				else
				{
					float x = (((flipmode_ & 1) != 0) ? ((-1)) : (1));
					float y = (((flipmode_ & 2) != 0) ? ((-1)) : (1));
					PushMatrix(Matrix.ScaleTemporary(x, y, swidth / 2f, sheight / 2f));
				}
			}
			try
			{
				if (image.optimizePass_ != 0)
				{
					float num17 = (float)width / swidth;
					float num18 = (float)height / sheight;
					if (image.optimizeSeb_ == null)
					{
						int num19 = sx / image.optimizeBlockWidth_;
						int num20 = sy / image.optimizeBlockHeight_;
						if (num20 < 0 || image.optimizeHeightNum_ <= num20 || num19 < 0 || image.optimizeWidthNum_ <= num19)
						{
							return;
						}
						for (int i = 0; i < image.optimizePass_; i++)
						{
							int[] optimize = image.GetOptimize(num19, num20, i);
							if (optimize == null)
							{
								continue;
							}
							Image image2 = image;
							if (optimize[0] != -1)
							{
								image2 = image.images_[optimize[0]];
								ImageCheckUse(image2);
							}
							RectangleF rectangleF = drawImageRect_;
							rectangleF.Set(dx, dy, dx + width, dy + height);
							float num21 = sx - num19 * image.optimizeBlockWidth_ - optimize[1];
							float num22 = sy - num20 * image.optimizeBlockHeight_ - optimize[2];
							float num23 = swidth;
							float num24 = sheight;
							if (num21 < 0f)
							{
								num23 += num21;
								rectangleF.left -= num21 * num17;
								num21 = 0f;
							}
							if ((float)optimize[5] < num21 + num23)
							{
								rectangleF.right -= (num21 + num23 - (float)optimize[5]) * num17;
								num23 = (float)optimize[5] - num21;
							}
							if (num22 < 0f)
							{
								num24 += num22;
								rectangleF.top -= num22 * num18;
								num22 = 0f;
							}
							if ((float)optimize[6] < num22 + num24)
							{
								rectangleF.bottom -= (num22 + num24 - (float)optimize[6]) * num18;
								num24 = (float)optimize[6] - num22;
							}
							if (CheckLocalMatrix())
							{
								PushMatrix(Matrix.TranslateTemporary(rectangleF.left - dx, rectangleF.top - dy));
								try
								{
									if (image2 != null)
									{
										_drawBitmap(image2, dx, dy, rectangleF.Width(), rectangleF.Height(), (float)optimize[3] + num21, (float)optimize[4] + num22, num23, num24);
									}
								}
								finally
								{
									PopMatrix();
								}
							}
							else if (image2 != null)
							{
								_drawBitmap(image2, rectangleF.left, rectangleF.top, rectangleF.Width(), rectangleF.Height(), (float)optimize[3] + num21, (float)optimize[4] + num22, num23, num24);
							}
							if (Config.COUNT_ENABLE)
							{
								Config.drawImageCount_++;
								Config.drawImageArea_ += optimize[5] * optimize[6];
								if (Config.checkCount_ != image2.checkCount_)
								{
									Config.drawSourceImageArea_ += image2.GetWidth() * image2.GetHeight();
								}
							}
							image2.checkCount_ = Config.checkCount_;
						}
						return;
					}
					{
					int num25 = -1;
					int num26 = image.optimizeSeb_.Length / 4;
					for (int j = 0; j < num26; j++)
					{
						int num27 = j * 4;
						if (image.optimizeSeb_[num27 + 0] == -1)
						{
							num27 = image.optimizeSeb_[num27 + 1] * 4;
							if (image.optimizeSebEx_[num27 + 0] == sx && image.optimizeSebEx_[num27 + 1] == sy && (float)image.optimizeSebEx_[num27 + 2] == swidth && (float)image.optimizeSebEx_[num27 + 3] == sheight)
							{
								num25 = j;
								break;
							}
						}
						else if ((image.optimizeSeb_[num27 + 0] & 0xFF) == sx && (image.optimizeSeb_[num27 + 1] & 0xFF) == sy && (float)(image.optimizeSeb_[num27 + 2] & 0xFF) == swidth && (float)(image.optimizeSeb_[num27 + 3] & 0xFF) == sheight)
						{
							num25 = j;
							break;
						}
					}
					if (num25 != -1) {
					image.GetOptimizeSeb(num25);
					for (int k = 0; k < image.optimizePass_; k++)
					{
						int num28 = num25 * image.optimizePass_ + k;
						int[] array = null;
						if (image.optimizeAccess_ == null) { array = image.GetOptimize(image.optimizeAccessL_[num28]); } else { array = image.GetOptimize(image.optimizeAccess_[num28]); }
						if (array == null)
						{
							continue;
						}
						Image image3 = image;
						if (array[0] != -1)
						{
							image3 = image.images_[array[0]];
							ImageCheckUse(image3);
						}
						RectangleF rectangleF2 = drawImageRect_;
						rectangleF2.Set(dx, dy, dx + width, dy + height);
						float num29 = -array[1];
						float num30 = -array[2];
						float num31 = swidth;
						float num32 = sheight;
						if (num29 < 0f)
						{
							num31 += num29;
							rectangleF2.left -= num29 * num17;
							num29 = 0f;
						}
						if ((float)array[5] < num29 + num31)
						{
							rectangleF2.right -= (num29 + num31 - (float)array[5]) * num17;
							num31 = (float)array[5] - num29;
						}
						if (num30 < 0f)
						{
							num32 += num30;
							rectangleF2.top -= num30 * num18;
							num30 = 0f;
						}
						if ((float)array[6] < num30 + num32)
						{
							rectangleF2.bottom -= (num30 + num32 - (float)array[6]) * num18;
							num32 = (float)array[6] - num30;
						}
						if (CheckLocalMatrix())
						{
							PushMatrix(Matrix.TranslateTemporary(rectangleF2.left - dx, rectangleF2.top - dy));
							try
							{
								if (image3 != null)
								{
									_drawBitmap(image3, dx, dy, rectangleF2.Width(), rectangleF2.Height(), (float)array[3] + num29, (float)array[4] + num30, num31, num32);
								}
							}
							finally
							{
								PopMatrix();
							}
						}
						else if (image3 != null)
						{
							_drawBitmap(image3, rectangleF2.left, rectangleF2.top, rectangleF2.Width(), rectangleF2.Height(), (float)array[3] + num29, (float)array[4] + num30, num31, num32);
						}
						if (Config.COUNT_ENABLE)
						{
							Config.drawImageCount_++;
							Config.drawImageArea_ += array[5] * array[6];
							if (Config.checkCount_ != image3.checkCount_)
							{
								Config.drawSourceImageArea_ += image3.GetWidth() * image3.GetHeight();
							}
						}
						image3.checkCount_ = Config.checkCount_;
					}
					}
					}
				}
				else
				{
					_drawBitmap(image, dx, dy, width, height, sx, sy, swidth, sheight);
				}
			}
			finally
			{
				if (flipmode_ != 0)
				{
					PopMatrix();
				}
			}
		}

		protected void _drawBitmap(Image image, float dx, float dy, float width, float height, float sx, float sy, float swidth, float sheight)
		{
			ImageCheckUse(image);
			if (image.standardWidth_ > 0)
			{
				float num = (float)image.GetWidth() / (float)image.standardWidth_;
				sx *= num;
				swidth *= num;
			}
			if (image.standardHeight_ > 0)
			{
				float num2 = (float)image.GetHeight() / (float)image.standardHeight_;
				sy *= num2;
				sheight *= num2;
			}
			Image image2 = image;
			int num3 = (int)(sx + 0.1f);
			int num4 = (int)(sy + 0.1f);
			int num5 = (int)(swidth + 0.1f);
			int num6 = (int)(sheight + 0.1f);
			if (image.atlasRegion_ != null && image.atlasRegion_.IsValid())
			{
				sx += (float)image.atlasRegion_.X;
				sy += (float)image.atlasRegion_.Y;
				image2 = image.atlasRegion_.Atlas.GetImage();
				image2.SetAlphaType(image.GetAlphaType());
			}
			if (blendMode_ == 3 && image2.image_ is Texture2D)
			{
				GrayscaleImage grayscaleImage = null;
				for (int i = 0; i < grayscaleImages_.Count; i++)
				{
					GrayscaleImage grayscaleImage2 = grayscaleImages_[i];
					if (grayscaleImage2.Source == image && grayscaleImage2.X == num3 && grayscaleImage2.Y == num4 && grayscaleImage2.Width == num5 && grayscaleImage2.Height == num6)
					{
						if (grayscaleImage2.Image.IsDisposed)
						{
							grayscaleImages_.RemoveAt(i);
						}
						else
						{
							grayscaleImage = grayscaleImage2;
						}
						break;
					}
				}
				if (grayscaleImage == null)
				{
					grayscaleImage = new GrayscaleImage();
					grayscaleImage.Source = image;
					grayscaleImage.X = num3;
					grayscaleImage.Y = num4;
					grayscaleImage.Width = num5;
					grayscaleImage.Height = num6;
					Color[] pixels = image2.GetPixels((int)(sx + 0.1f), (int)(sy + 0.1f), num5, num6);
					for (int j = 0; j < pixels.Length; j++)
					{
						float num7 = (pixels[j].r + pixels[j].g + pixels[j].b) / 3f;
						pixels[j].a = pixels[j].a;
						pixels[j].r = num7;
						pixels[j].g = num7;
						pixels[j].b = num7;
					}
					grayscaleImage.Image = Image.CreateImage(num5, num6, Image.Format.ARGB32);
					grayscaleImage.Image.SetPixels(0, 0, num5, num6, pixels);
					grayscaleImage.Image.SetExpiration(true, 20);
					grayscaleImages_.Add(grayscaleImage);
				}
				image2 = grayscaleImage.Image;
				sx = 0f;
				sy = 0f;
			}
			Texture image_ = image2.image_;
			if (image_ == null)
			{
				return;
			}
			if (image.renderQueue_ != 0)
			{
				PushRenderQueue(renderQueue_ + image.renderQueue_);
			}
			BeginRenderMatrix(ref dx, ref dy);
			bool flag = IsRenderGUIMode();
			try
			{
				RectangleF drawBitmap_rc = _drawBitmap_rc3;
				RectangleF drawBitmap_rc2 = _drawBitmap_rc4;
				if ((image.textRenderTasks_.Count > 0 || image.balloonRenderTasks_.Count > 0) && isGUI_)
				{
					drawBitmap_rc2.Set(dx, dy, dx + width, dy + height);
					drawBitmap_rc.Set(sx, sy, sx + swidth, sy + sheight);
				}
				if (image.touchTasks_.Count > 0 && isGUI_)
				{
					for (int k = 0; k < image.touchTasks_.Count; k++)
					{
						Image.TouchTask touchTask = image.touchTasks_[k];
						if (!touchTask.Surface.ExistTouchComponent(touchTask.Component))
						{
							TouchComponent touchComponent = new TouchComponent(touchTask.Component);
							if (!isGUI_)
							{
								float num8 = touchTask.Surface.GetScaleRatio() / 100f;
								touchComponent.x = (int)((float)touchComponent.x / num8 + 0.01f);
								touchComponent.y = (int)((float)touchComponent.y / num8 + 0.01f);
								touchComponent.width = (int)((float)touchComponent.width / num8 + 0.01f);
								touchComponent.height = (int)((float)touchComponent.height / num8 + 0.01f);
							}
							touchComponent.x = (int)((float)touchComponent.x + dx - sx);
							touchComponent.y = (int)((float)touchComponent.y + dy - sy);
							touchComponent.hasCount = 0;
							if (touchComponent.option != null)
							{
								touchComponent.option.hasCount = 0;
							}
							touchTask.Surface.AddTouchComponent(touchComponent);
						}
					}
				}
				bool flag2 = !renderCache_.Empty && renderCache_.Image == image2 && renderCache_.AlphaType == image2.GetAlphaType();
				renderCache_.Image = image2;
				renderCache_.AlphaType = image2.GetAlphaType();
				if (!flag2)
				{
					renderCache_.Empty = true;
				}
				if (!flag2 || renderCache_.Source[0] != sx || renderCache_.Source[1] != sy || renderCache_.Source[2] != swidth || renderCache_.Source[3] != sheight)
				{
					renderCache_.Source[0] = sx;
					renderCache_.Source[1] = sy;
					renderCache_.Source[2] = swidth;
					renderCache_.Source[3] = sheight;
					if (sx < 0f)
					{
						swidth += sx;
						sx = 0f;
					}
					if (sx + swidth > (float)image2.GetWidth())
					{
						swidth = (float)image2.GetWidth() - sx;
					}
					if (sy < 0f)
					{
						sheight += sy;
						sy = 0f;
					}
					if (sy + sheight > (float)image2.GetHeight())
					{
						sheight = (float)image2.GetHeight() - sy;
					}
					renderCache_.Valid = swidth > 0f && sheight > 0f;
					if (!renderCache_.Valid)
					{
						return;
					}
					RectangleF drawBitmap_rc3 = _drawBitmap_rc1;
					drawBitmap_rc3.Set(sx + 0.005f, sy + 0.005f, sx + swidth - 0.01f, sy + sheight - 0.01f);
					float num9 = drawBitmap_rc3.Width() / (float)image_.width;
					float num10 = drawBitmap_rc3.Height() / (float)image_.height;
					float num11 = drawBitmap_rc3.left / (float)image_.width;
					float num12 = ((float)image_.height - drawBitmap_rc3.Height() - drawBitmap_rc3.top) / (float)image_.height;
					renderCache_.SourceRect.Set(num11, num12, num11 + num9, num12 + num10);
				}
				if (!renderCache_.Valid)
				{
					return;
				}
				RectangleF sourceRect = renderCache_.SourceRect;
				RectangleF drawBitmap_rc4 = _drawBitmap_rc2;
				drawBitmap_rc4.Set(dx - imageExpand_ / 2f, dy - imageExpand_ / 2f, dx + width + imageExpand_, dy + height + imageExpand_);
				if (!(flag && renderIsReplace_ && (blendMode_ == 0 || blendMode_ == 3) && renderForceMaterial_ == null && localMatrixIndex_ == -1 && !useFigureClip_) ? false : (!setFigureClip_))
				{
					UpdateGUIGroup();
					GUI.DrawTextureWithTexCoords(GetTransRectGUI(drawBitmap_rc4, drawBitmap_rc4).ToRect(), image_, new Rect(sourceRect.left, sourceRect.top, sourceRect.Width(), sourceRect.Height()), image.GetAlphaType() != 0);
				}
				else if ((!flag) & flag2) {
					Vector3[] transRectGL = GetTransRectGL(drawBitmap_rc4, _drawBitmap_vertices);
					for (int l = 0; l < renderCache_.Count; l++)
					{
						MeshBuilder meshBuilder = renderCache_.Meshes[l];
						if (meshBuilder.vertexColor)
						{
							meshBuilder.AddQuad(transRectGL, sourceRect.left, sourceRect.top, sourceRect.right - sourceRect.left, sourceRect.bottom - sourceRect.top, renderCache_.Colors[l]);
						}
						else
						{
							meshBuilder.AddQuad(transRectGL, sourceRect.left, sourceRect.top, sourceRect.right - sourceRect.left, sourceRect.bottom - sourceRect.top);
						}
					}
				} else {
					UpdateGLClip();
					Vector3[] transRectGL2 = GetTransRectGL(drawBitmap_rc4, _drawBitmap_vertices);
					RenderPass[] renderPass = GetRenderPass(image2);
					renderCache_.Count = renderPass.Length;
					for (int m = 0; m < renderPass.Length; m++)
					{
						RenderPass renderPass2 = renderPass[m];
						Color color = (renderCache_.Colors[m] = renderPass2.Begin()[0]);
						kairo.unity.graphics.Material material = ((renderPass2.Material != null) ? mateMan_.GetMaterial(renderPass2.Material, image2) : mateMan_.GetMaterial(this, image2, color.a, false, renderPass2.opaqueTransOrder_));
						if (!flag)
						{
							MeshBuilder meshBuilder2 = (renderCache_.Meshes[m] = meshManList_[meshId_].GetMeshBuilder(material));
							if (meshBuilder2.vertexColor)
							{
								meshBuilder2.AddQuad(transRectGL2, sourceRect.left, sourceRect.top, sourceRect.right - sourceRect.left, sourceRect.bottom - sourceRect.top, color);
							}
							else
							{
								meshBuilder2.AddQuad(transRectGL2, sourceRect.left, sourceRect.top, sourceRect.right - sourceRect.left, sourceRect.bottom - sourceRect.top);
							}
						}
						else
						{
							material.SetPass(0);
							GL.Begin(7);
							GL.Color(color);
							GL.TexCoord2(sourceRect.left, sourceRect.bottom);
							GL.Vertex(transRectGL2[0]);
							GL.TexCoord2(sourceRect.right, sourceRect.bottom);
							GL.Vertex(transRectGL2[1]);
							GL.TexCoord2(sourceRect.right, sourceRect.top);
							GL.Vertex(transRectGL2[2]);
							GL.TexCoord2(sourceRect.left, sourceRect.top);
							GL.Vertex(transRectGL2[3]);
							GL.End();
						}
						renderPass2.End();
					}
					renderCache_.Image = image;
					renderCache_.Empty = false;
				}
				if (isGUI_ && image.textRenderTasks_.Count > 0)
				{
					ClipRect(drawBitmap_rc2.left, drawBitmap_rc2.top, drawBitmap_rc2.Width(), drawBitmap_rc2.Height());
					int color2 = GetColor();
					Font font = GetFont();
					try
					{
						Font.PushLocalizeSetting(false);
						for (int n = 0; n < image.textRenderTasks_.Count; n++)
						{
							Image.TextRenderTask textRenderTask = image.textRenderTasks_[n];
							float num13 = Math.Min(drawBitmap_rc2.Width() / drawBitmap_rc.Width(), drawBitmap_rc2.Height() / drawBitmap_rc.Height());
							Font drawBitmapFont = _drawBitmapFont;
							drawBitmapFont.Copy(textRenderTask.Font);
							drawBitmapFont.SetSize((int)((float)drawBitmapFont.GetHeight() * num13 + 0.01f), false);
							SetFont(drawBitmapFont);
							SetColor(textRenderTask.Color);
							DrawString(textRenderTask.Text, drawBitmap_rc2.left + (textRenderTask.X - drawBitmap_rc.left) * num13, drawBitmap_rc2.top + (textRenderTask.Y - drawBitmap_rc.top) * num13, textRenderTask.Anchor);
						}
					}
					finally
					{
						SetColor(color2);
						SetFont(font);
						Font.PopLocalizeSetting();
					}
					PopClip();
				}
				if (isGUI_ && image.balloonRenderTasks_.Count > 0)
				{
					ClipRect(drawBitmap_rc2.left, drawBitmap_rc2.top, drawBitmap_rc2.Width(), drawBitmap_rc2.Height());
					GetColor();
					GetFont();
					try
					{
						Font.PushLocalizeSetting(false);
						for (int num14 = 0; num14 < image.balloonRenderTasks_.Count; num14++)
						{
							Image.BalloonRenderTask balloonRenderTask = image.balloonRenderTasks_[num14];
							float num15 = Math.Min(drawBitmap_rc2.Width() / drawBitmap_rc.Width(), drawBitmap_rc2.Height() / drawBitmap_rc.Height());
							Matrix4x4 matrix = Matrix4x4.identity;
							if (balloonRenderTask.Offscreen != null)
							{
								matrix = balloonRenderTask.Offscreen.dynamicTransform_;
								matrix.m13 = -matrix.m13;
							}
							float scale2_ = balloonRenderTask.Balloon.scale2_;
							balloonRenderTask.Balloon.scale2_ *= num15;
							balloonRenderTask.Balloon.Draw(this, drawBitmap_rc2.left + (balloonRenderTask.X - drawBitmap_rc.left) * num15, drawBitmap_rc2.top + (balloonRenderTask.Y - drawBitmap_rc.top) * num15, matrix);
							balloonRenderTask.Balloon.scale2_ = scale2_;
						}
					}
					finally
					{
						Font.PopLocalizeSetting();
					}
					PopClip();
				}
			}
			finally
			{
				EndRenderMatrix();
				if (image.renderQueue_ != 0)
				{
					PopRenderQueue();
				}
			}
			if (Config.COUNT_ENABLE)
			{
				Config.drawImageCount_++;
				Config.drawImageArea_ += (int)(swidth * sheight);
				if (Config.checkCount_ != image.checkCount_)
				{
					Config.drawSourceImageArea_ += image.GetWidth() * image.GetHeight();
				}
			}
			image.checkCount_ = Config.checkCount_;
		}

		protected void ImageCheckUse(Image image)
		{
			if (image != null && image.image_ == null && (image.atlasRegion_ == null || !image.atlasRegion_.IsValid()))
			{
				try
				{
					image.Use();
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					if (Config.PRINT)
					{
						Log.Info("imageCheckUse", exception);
					}
				}
			}
		}

		public void DrawLine(float x1, float y1, float x2, float y2, float lineWidth = 1f)
		{
			float num = x2 - x1;
			float num2 = y2 - y1;
			if (lineWidth == 1f && (num == 0f || num2 == 0f))
			{
				FillRect(x1, y1, num + 1f, num2 + 1f);
				return;
			}
			BeginRenderMatrix(ref x1, ref y1);
			x2 = x1 + num;
			y2 = y1 + num2;
			try
			{
				UpdateGLClip();
				Vector3[] drawLine_Vertices = DrawLine_Vertices;
				Vector2 vector = default(Vector2);
				vector.Set(y2 - y1, -((x2 - x1)));
				vector.Normalize();
				float num3 = (int)(lineWidth * 0.5f);
				float num4 = lineWidth - num3;
				Vector2 vector2 = -vector * num3;
				Vector2 vector3 = vector * num4;
				drawLine_Vertices[0].Set(x1 + vector2.x, y1 + vector2.y, 0f);
				drawLine_Vertices[1].Set(x2 + vector2.x, y2 + vector2.y, 0f);
				drawLine_Vertices[2].Set(x2 + vector3.x, y2 + vector3.y, 0f);
				drawLine_Vertices[3].Set(x1 + vector3.x, y1 + vector3.y, 0f);
				GetTransPointGL(drawLine_Vertices);
				RenderPass[] renderPass = GetRenderPass(guiStyle_.normal.textColor);
				foreach (RenderPass renderPass2 in renderPass)
				{
					Color color = renderPass2.BeginShape()[0];
					try
					{
						kairo.unity.graphics.Material material = ((renderPass2.Material != null) ? mateMan_.GetMaterial(renderPass2.Material, null) : mateMan_.GetMaterial(this, null, color.a));
						if (!IsRenderGUIMode())
						{
							MeshBuilder meshBuilder = meshManList_[meshId_].GetMeshBuilder(material);
							meshBuilder.ReadyAddition();
							meshBuilder.AddVertex(drawLine_Vertices[0], color);
							meshBuilder.AddVertex(drawLine_Vertices[1], color);
							meshBuilder.AddVertex(drawLine_Vertices[2], color);
							meshBuilder.AddVertex(drawLine_Vertices[3], color);
							meshBuilder.AddQuadIndicies();
						}
						else
						{
							material.SetPass(0);
							GL.Begin(7);
							GL.Color(color);
							GL.Vertex(drawLine_Vertices[0]);
							GL.Vertex(drawLine_Vertices[1]);
							GL.Vertex(drawLine_Vertices[2]);
							GL.Vertex(drawLine_Vertices[3]);
							GL.End();
						}
					}
					finally
					{
						renderPass2.End();
					}
				}
				if (Config.COUNT_ENABLE)
				{
					Config.drawShapeCount_++;
				}
			}
			finally
			{
				EndRenderMatrix();
			}
		}

		public void DrawRect(float x, float y, float width, float height)
		{
			BeginRenderMatrix(ref x, ref y);
			try
			{
				UpdateGLClip();
				Vector3[] transRectGL = GetTransRectGL(x, y, width + 1f, height + 1f, DrawRect_Vertices1);
				Vector3[] transRectGL2 = GetTransRectGL(x + 1f, y + 1f, width - 1f, height - 1f, DrawRect_Vertices2);
				RenderPass[] renderPass = GetRenderPass(guiStyle_.normal.textColor);
				foreach (RenderPass renderPass2 in renderPass)
				{
					Color color = renderPass2.BeginShape()[0];
					try
					{
						kairo.unity.graphics.Material material = ((renderPass2.Material != null) ? mateMan_.GetMaterial(renderPass2.Material, null) : mateMan_.GetMaterial(this, null, color.a));
						if (!IsRenderGUIMode())
						{
							MeshBuilder meshBuilder = meshManList_[meshId_].GetMeshBuilder(material);
							meshBuilder.ReadyAddition();
							for (int j = 0; j < transRectGL.Length; j++)
							{
								meshBuilder.AddVertex(transRectGL[j].x, transRectGL[j].y, transRectGL[j].z, color);
							}
							for (int k = 0; k < transRectGL2.Length; k++)
							{
								meshBuilder.AddVertex(transRectGL2[k].x, transRectGL2[k].y, transRectGL2[k].z, color);
							}
							int[] array = addIndiciesBuffer_;
							array[0] = 0;
							array[6] = 1;
							array[12] = 2;
							array[18] = 3;
							array[1] = 1;
							array[7] = 2;
							array[13] = 3;
							array[19] = 0;
							array[2] = 5;
							array[8] = 6;
							array[14] = 7;
							array[20] = 4;
							array[3] = 0;
							array[9] = 1;
							array[15] = 2;
							array[21] = 3;
							array[4] = 5;
							array[10] = 6;
							array[16] = 7;
							array[22] = 4;
							array[5] = 4;
							array[11] = 5;
							array[17] = 6;
							array[23] = 7;
							meshBuilder.AddIndicies(addIndiciesBuffer_, 24);
						}
						else
						{
							material.SetPass(0);
							GL.Begin(7);
							GL.Color(color);
							GL.Vertex(transRectGL[0]);
							GL.Vertex(transRectGL[1]);
							GL.Vertex(transRectGL2[1]);
							GL.Vertex(transRectGL2[0]);
							GL.Vertex(transRectGL[1]);
							GL.Vertex(transRectGL[2]);
							GL.Vertex(transRectGL2[2]);
							GL.Vertex(transRectGL2[1]);
							GL.Vertex(transRectGL[2]);
							GL.Vertex(transRectGL[3]);
							GL.Vertex(transRectGL2[3]);
							GL.Vertex(transRectGL2[2]);
							GL.Vertex(transRectGL[3]);
							GL.Vertex(transRectGL[0]);
							GL.Vertex(transRectGL2[0]);
							GL.Vertex(transRectGL2[3]);
							GL.End();
						}
					}
					finally
					{
						renderPass2.End();
					}
				}
				if (Config.COUNT_ENABLE)
				{
					Config.drawShapeCount_++;
				}
			}
			finally
			{
				EndRenderMatrix();
			}
		}

		public void DrawRect2(float x, float y, float width, float height)
		{
			DrawRect(x, y, width - 1f, height - 1f);
		}

		public void DrawRectPoint(float left, float top, float right, float bottom)
		{
			DrawRect(left, top, right - left - 1f, bottom - top - 1f);
		}

		public void DrawString(long v, float x, float y)
		{
			DrawString(null, v.ToString(), x, y, 17);
		}

		public void DrawString(TextFormat lf, long v, float x, float y)
		{
			DrawString(lf, v.ToString(), x, y, 17);
		}

		public void DrawString(long v, float x, float y, int anchor)
		{
			DrawString(null, v.ToString(), x, y, anchor);
		}

		public void DrawString(TextFormat lf, long v, float x, float y, int anchor)
		{
			DrawString(null, v.ToString(), x, y, anchor);
		}

		public void DrawString(string str, float x, float y)
		{
			DrawString(null, str, x, y, 17);
		}

		public void DrawString(TextFormat lf, string str, float x, float y)
		{
			DrawString(lf, str, x, y, 17);
		}

		public void DrawString(string str, float x, float y, int anchor, int color)
		{
			int num = this.GetColor();
			this.SetColor(color);
			this.DrawString(null, str, (float)x, (float)y, anchor);
			this.SetColor(num);
		}

		public void DrawString(string str, float x, float y, int anchor)
		{
			this.DrawString(null, str, (float)x, (float)y, anchor);
		}

		public void DrawString(TextFormat lf, string str, float x, float y, int anchor, int color)
		{
			int color2 = GetColor();
			SetColor(color);
			DrawString(lf, str, x, y, anchor);
			SetColor(color2);
		}

		public void DrawText(string str, float x, float y, float width, int anchor = 0)
		{
			DrawText(null, str, x, y, width, anchor);
		}

		public void DrawTextBold(string str, float x, float y, float width, int color, int anchor = 0)
		{
			DrawTextBold(null, str, x, y, width, color, anchor);
		}

		public void DrawTextBold(TextFormat lf, string str, float x, float y, float width, int color, int anchor = 0)
		{
			int num = anchor;
			if ((anchor & 0x70) == 0)
			{
				anchor |= 0x20;
			}
			int num2 = -1;
			if (!Language.Japanese() && lf != null)
			{
				if (lf.GetFont() != -1 && lf.GetFont() != font_.GetSize())
				{
					num2 = font_.GetSize();
					font_.SetSize(lf.GetFont());
				}
				x = lf.GetX(x);
				y = lf.GetY(y);
				anchor = lf.GetAnchor(anchor);
			}
			bool flag = (lf == null || lf.GetFont() == -1) && font_.IsNeedLocalizeScale();
			if (flag)
			{
				num2 = font_.GetSize();
				int height = font_.GetHeight();
				font_.SetSize(font_.GetLocalizeScale());
				if ((anchor & 0x60) == 0)
				{
					y += (float)(height - font_.GetHeight()) / 2f;
				}
				Font.LockLocalizeSetting();
			}
			if ((num & 0x20) != 0)
			{
				y -= (float)(font_.GetHeight() / 2);
			}
			else if ((num & 0x40) != 0)
			{
				y -= (float)font_.GetHeight();
			}
			bool pictographVisible = TextLayout.IsPictographVisible();
			TextLayout.SetPictographVisible(false);
			int color2 = GetColor();
			try
			{
				for (int i = 0; i < 3; i++)
				{
					for (int j = 0; j < 3; j++)
					{
						if (j != 1 || i != 1)
						{
							TextLayout.Draw(this, str, x - 1f + (float)j, y - 1f + (float)i, width, font_.GetHeight(), color, lineSpacing_, anchor);
						}
					}
				}
			}
			finally
			{
				TextLayout.SetPictographVisible(pictographVisible);
			}
			TextLayout.Draw(this, str, x, y, width, font_.GetHeight(), color2, lineSpacing_, anchor);
			if (flag)
			{
				Font.UnlockLocalizeSetting();
			}
			if (num2 != -1)
			{
				font_.SetSize(num2);
			}
		}

		public void DrawText(TextFormat lf, string str, float x, float y, float width, int anchor = 0)
		{
			int num = anchor;
			if ((anchor & 0x70) == 0)
			{
				anchor |= 0x20;
			}
			int num2 = -1;
			if (!Language.Japanese() && lf != null)
			{
				if (lf.GetFont() != -1 && lf.GetFont() != font_.GetSize())
				{
					num2 = font_.GetSize();
					font_.SetSize(lf.GetFont());
				}
				x = lf.GetX(x);
				y = lf.GetY(y);
				anchor = lf.GetAnchor(anchor);
			}
			int num3;
			if (lf == null || lf.GetFont() == -1)
			{
				num3 = (font_.IsNeedLocalizeScale() ? 1 : 0);
				if (num3 != 0)
				{
					num2 = font_.GetSize();
					int height = font_.GetHeight();
					font_.SetSize(font_.GetLocalizeScale());
					if ((anchor & 0x60) == 0)
					{
						y += (float)(height - font_.GetHeight()) / 2f;
					}
					Font.LockLocalizeSetting();
				}
			}
			else
			{
				num3 = 0;
			}
			if ((num & 0x20) != 0)
			{
				y -= (float)(font_.GetHeight() / 2);
			}
			else if ((num & 0x40) != 0)
			{
				y -= (float)font_.GetHeight();
			}
			TextLayout.Draw(this, str, x, y, width, font_.GetHeight(), GetColor(), lineSpacing_, anchor);
			if (num3 != 0)
			{
				Font.UnlockLocalizeSetting();
			}
			if (num2 != -1)
			{
				font_.SetSize(num2);
			}
		}

		public void DrawString(TextFormat lf, string str, float x, float y, int anchor)
		{
			str = Language.TranslateText(str);
			if ((PeekAutoTextLayout() && str.IndexOf('<') != -1) || (lineSpacing_ != 0 && str.IndexOf('\n') != -1))
			{
				DrawString2(str, x, y, anchor);
				return;
			}
			int num = -1;
			if (!Language.Japanese() && lf != null)
			{
				if (lf.GetFont() != -1 && lf.GetFont() != font_.GetSize())
				{
					num = font_.GetSize();
					font_.SetSize(lf.GetFont());
				}
				x = lf.GetX(x);
				y = lf.GetY(y);
				anchor = lf.GetAnchor(anchor);
			}
			bool flag = (lf == null || lf.GetFont() == -1) && font_.IsNeedLocalizeScale();
			if (flag)
			{
				num = font_.GetSize();
				int height = font_.GetHeight();
				font_.SetSize(font_.GetLocalizeScale());
				if ((anchor & 0x60) == 0)
				{
					y += (float)(height - font_.GetHeight()) / 2f;
				}
				Font.LockLocalizeSetting();
			}
			BeginRenderMatrix(ref x, ref y);
			try
			{
				float num4;
				float num2;
				float num3;
				if (!isGUI_)
				{
					if (render_ != null)
					{
						Font font = new Font(GetFont());
						RectangleF transRectGUI = GetTransRectGUI(0f, 0f, font.GetWidth(), font.GetHeight(), DrawString_tmp);
						num2 = transRectGUI.Width() / font.GetWidth();
						num3 = transRectGUI.Height() / (float)font.GetHeight();
						num4 = ((num2 < num3) ? num2 : num3);
						font.SetSize((int)((float)font.GetHeight() * num4), false);
						y += ((float)font.GetHeight() - (float)font.GetHeight() * num4 / num3) / 2f;
						render_.textRenderTasks_.Add(new Image.TextRenderTask(str, x, y, anchor, color_, font));
					}
					return;
				}
				if ((anchor & 2) != 0)
				{
					x -= font_.StringWidthF(str) / 2f;
				}
				else if ((anchor & 4) != 0)
				{
					x -= font_.StringWidthF(str);
				}
				if ((anchor & 0x20) != 0)
				{
					y -= (float)font_.GetHeight() / 2f;
				}
				else if ((anchor & 0x40) != 0)
				{
					y -= (float)font_.GetHeight();
				}
				else if (fontScaleOffsetEnable_)
				{
					int height2 = font_.GetHeight();
					y += (float)((height2 * 100 / font_.fontScale_ - height2) / 2);
				}
				font_.GetHeight();
				font_.GetWidth();
				RectangleF transRectGUI2 = GetTransRectGUI(0f, 0f, font_.GetWidth(), font_.GetHeight(), DrawString_tmp);
				num2 = transRectGUI2.Width() / font_.GetWidth();
				num3 = transRectGUI2.Height() / (float)font_.GetHeight();
				num4 = ((num2 < num3) ? num2 : num3);
				guiStyle_.font = font_.GetFont();
				guiStyle_.fontSize = (int)((float)font_.GetHeight() * num4);
				guiStyle_.fontStyle = font_.GetFontStyle();
				y += ((float)font_.GetHeight() - (float)font_.GetHeight() * num4 / num3) / 2f;
				if (GUI.skin.font != guiStyle_.font)
				{
					GUI.skin.font = guiStyle_.font;
				}
				float[] transPointGUI = GetTransPointGUI(x, y, DrawString_tmp2);
				Color textColor = guiStyle_.normal.textColor;
				if (renderOperator_ == 0 && renderSrcRatio_ < 255)
				{
					Color textColor2 = guiStyle_.normal.textColor;
					textColor2.r = textColor2.r * (float)renderSrcRatio_ / 255f;
					textColor2.g = textColor2.g * (float)renderSrcRatio_ / 255f;
					textColor2.b = textColor2.b * (float)renderSrcRatio_ / 255f;
					guiStyle_.normal.textColor = textColor2;
				}
				else if (renderOperator_ == 1 && renderSrcRatio_ < 255)
				{
					Color textColor3 = guiStyle_.normal.textColor;
					textColor3.a = textColor3.a * (float)renderSrcRatio_ / 255f;
					guiStyle_.normal.textColor = textColor3;
				}
				if (glRender_)
				{
					if (glTextCount_ >= glTextList_.Count)
					{
						glTextList_.Add(new GLText());
					}
					glTextList_[glTextCount_].Init(new Rect(transPointGUI[0], transPointGUI[1], 1f, transRectGUI2.Height() * 2f), str, guiStyle_, clipRegion_);
					glTextCount_++;
				}
				else
				{
					UpdateGUIGroup();
					GUI.Label(new Rect(transPointGUI[0], transPointGUI[1], 1f, transRectGUI2.Height() * 2f), str, guiStyle_);
				}
				guiStyle_.normal.textColor = textColor;
				if (Config.COUNT_ENABLE)
				{
					Config.drawStringCount_++;
				}
			}
			finally
			{
				EndRenderMatrix();
				if (flag)
				{
					Font.UnlockLocalizeSetting();
				}
				if (num != -1)
				{
					font_.SetSize(num);
				}
			}
		}

		public void DrawString2(string str, float x, float y)
		{
			DrawString2(null, str, x, y, 17);
		}

		public void DrawString2(TextFormat lf, string str, float x, float y)
		{
			DrawString2(lf, str, x, y, 17);
		}

		public void DrawString2(string str, float x, float y, int anchor, int color)
		{
			int color2 = GetColor();
			SetColor(color);
			DrawString2(null, str, x, y, anchor);
			SetColor(color2);
		}

		public void DrawString2(string str, float x, float y, int anchor)
		{
			DrawString2(null, str, x, y, anchor);
		}

		public void DrawString2(TextFormat lf, string str, float x, float y, int anchor, int color)
		{
			int color2 = GetColor();
			SetColor(color);
			DrawString2(lf, str, x, y, anchor);
			SetColor(color2);
		}

		public void DrawString2(TextFormat lf, string str, float x, float y, int anchor)
		{
			TextLayout.Draw(lf, this, str, x, y, GetColor(), anchor);
		}

		public void DrawStringBold(string str, float x, float y, int color)
		{
			DrawStringBold(null, str, x, y, color, 17);
		}

		public void DrawStringBold(TextFormat lf, string str, float x, float y, int color)
		{
			DrawStringBold(lf, str, x, y, color, 17);
		}

		public void DrawStringBold(string str, float x, float y, int color, int anchor)
		{
			DrawStringBold(null, str, x, y, color, anchor);
		}

		public void DrawStringBold(TextFormat lf, string str, float x, float y, int color, int anchor)
		{
			int color2 = color_;
			int num = -1;
			str = Language.TranslateText(str);
			if (PeekAutoTextLayout() && str.IndexOf('<') != -1)
			{
				DrawStringBold2(lf, str, x, y, color, anchor);
				return;
			}
			if (!Language.Japanese() && lf != null)
			{
				if (lf.GetFont() != -1 && lf.GetFont() != font_.GetSize())
				{
					num = font_.GetSize();
					font_.SetSize(lf.GetFont());
				}
				x = lf.GetX(x);
				y = lf.GetY(y);
				anchor = lf.GetAnchor(anchor);
			}
			bool flag = (lf == null || lf.GetFont() == -1) && font_.IsNeedLocalizeScale();
			if (flag)
			{
				num = font_.GetSize();
				int height = font_.GetHeight();
				font_.SetSize(font_.GetLocalizeScale());
				if ((anchor & 0x60) == 0)
				{
					y += (float)(height - font_.GetHeight()) / 2f;
				}
				Font.LockLocalizeSetting();
			}
			SetColor(color);
			for (int i = 0; i < outlineQuality_ + 1; i++)
			{
				float num2 = outlineWidth_ * (float)(i + 1) / (float)(outlineQuality_ + 1);
				DrawString(str, x - num2, y - num2, anchor);
				DrawString(str, x + 0f, y - num2, anchor);
				DrawString(str, x + num2, y - num2, anchor);
				DrawString(str, x - num2, y + 0f, anchor);
				DrawString(str, x + num2, y + 0f, anchor);
				DrawString(str, x - num2, y + num2, anchor);
				DrawString(str, x + 0f, y + num2, anchor);
				DrawString(str, x + num2, y + num2, anchor);
			}
			SetColor(color2);
			DrawString(str, x, y, anchor);
			if (flag)
			{
				Font.UnlockLocalizeSetting();
			}
			if (num != -1)
			{
				font_.SetSize(num);
			}
		}

		public void DrawStringBold2(string str, float x, float y, int color)
		{
			DrawStringBold2(null, str, x, y, color, 17);
		}

		public void DrawStringBold2(TextFormat lf, string str, float x, float y, int color)
		{
			DrawStringBold2(lf, str, x, y, color, 17);
		}

		public void DrawStringBold2(string str, float x, float y, int color, int anchor)
		{
			DrawStringBold2(null, str, x, y, color, anchor);
		}

		public void DrawStringBold2(TextFormat lf, string str, float x, float y, int color, int anchor)
		{
			int color2 = color_;
			int num = -1;
			if (!Language.Japanese() && lf != null)
			{
				if (lf.GetFont() != -1 && lf.GetFont() != font_.GetSize())
				{
					num = font_.GetSize();
					font_.SetSize(lf.GetFont());
				}
				x = lf.GetX(x);
				y = lf.GetY(y);
				anchor = lf.GetAnchor(anchor);
			}
			bool flag = (lf == null || lf.GetFont() == -1) && font_.IsNeedLocalizeScale();
			if (flag)
			{
				num = font_.GetSize();
				int height = font_.GetHeight();
				font_.SetSize(font_.GetLocalizeScale());
				if ((anchor & 0x60) == 0)
				{
					y += (float)(height - font_.GetHeight()) / 2f;
				}
				Font.LockLocalizeSetting();
			}
			SetColor(color);
			for (int i = 0; i < outlineQuality_ + 1; i++)
			{
				float num2 = outlineWidth_ * (float)(i + 1) / (float)(outlineQuality_ + 1);
				string str2 = TextLayout.RemoveColorTag(str);
				DrawString2(str2, x - num2, y - num2, anchor);
				DrawString2(str2, x + 0f, y - num2, anchor);
				DrawString2(str2, x + num2, y - num2, anchor);
				DrawString2(str2, x - num2, y + 0f, anchor);
				DrawString2(str2, x + num2, y + 0f, anchor);
				DrawString2(str2, x - num2, y + num2, anchor);
				DrawString2(str2, x + 0f, y + num2, anchor);
				DrawString2(str2, x + num2, y + num2, anchor);
			}
			SetColor(color2);
			DrawString2(str, x, y, anchor);
			if (flag)
			{
				Font.UnlockLocalizeSetting();
			}
			if (num != -1)
			{
				font_.SetSize(num);
			}
		}

		public void FillArc(float x, float y, float width, float height, float startAngle, float arcAngle)
		{
			BeginRenderMatrix(ref x, ref y);
			try
			{
				if (arcAngle < -360f)
				{
					arcAngle = -360f;
				}
				if (arcAngle > 360f)
				{
					arcAngle = 360f;
				}
				startAngle %= 360f;
				if (startAngle < 0f)
				{
					startAngle += 360f;
				}
				if (arcAngle < 0f)
				{
					startAngle += arcAngle;
					arcAngle *= -1f;
				}
				UpdateGLClip();
				float num = width / 2f;
				float num2 = height / 2f;
				x += width / 2f;
				y += height / 2f;
				float num3 = 360f / (float)(int)((1f + (width + height) * 2f / 50f) * 8f + 8f);
				num3 = 8f;
				RenderPass[] renderPass = GetRenderPass(guiStyle_.normal.textColor);
				int num4 = 0;
				while (num4 < renderPass.Length)
				{
					RenderPass renderPass2 = renderPass[num4];
					Color color = renderPass2.BeginShape()[0];
					try
					{
						kairo.unity.graphics.Material material = ((renderPass2.Material != null) ? mateMan_.GetMaterial(renderPass2.Material, null) : mateMan_.GetMaterial(this, null, color.a));
						float num5 = startAngle + arcAngle;
						float num6 = startAngle;
						float[][] array = fillArcPoints_;
						GetTransPointGL(x, y, array[0]);
						GetTransPointGL(x + FastMath.Cos(num6) * num, y - FastMath.Sin(num6) * num2, array[1]);
						if (!IsRenderGUIMode())
						{
							MeshBuilder meshBuilder = meshManList_[meshId_].GetMeshBuilder(material);
							meshBuilder.ReadyAddition();
							meshBuilder.AddVertex(array[0][0], array[0][1], 0f, color);
							meshBuilder.AddVertex(array[1][0], array[1][1], 0f, color);
							int num7 = 0;
							while (num6 < num5)
							{
								float num8 = num6 + num3;
								if (num7 == 0)
								{
									num8 = (float)(int)(num8 / num3) * num3 + num3;
									if (num8 < num6)
									{
										num8 += num3;
									}
								}
								if (num8 > num5)
								{
									num8 = num5;
								}
								GetTransPointGL(x + FastMath.Cos(num8) * num, y - FastMath.Sin(num8) * num2, array[2]);
								meshBuilder.AddVertex(array[2][0], array[2][1], 0f, color);
								int[] array2 = addIndiciesBuffer_;
								array2[0] = 0;
								array2[1] = num7 + 1;
								array2[2] = num7 + 2;
								meshBuilder.AddIndicies(array2, 3);
								num6 = num8;
								num7++;
							}
						}
						else
						{
							material.SetPass(0);
							GL.Begin(4);
							GL.Color(color);
							int num9 = 0;
							while (num6 < num5)
							{
								float num10 = num6 + num3;
								if (num9 == 0)
								{
									num10 = (float)(int)(num10 / num3) * num3 + num3;
									if (num10 < num6)
									{
										num10 += num3;
									}
								}
								if (num10 > num5)
								{
									num10 = num5;
								}
								GetTransPointGL(x + FastMath.Cos(num10) * num, y - FastMath.Sin(num10) * num2, array[2]);
								GL.Vertex3(array[0][0], array[0][1], 0f);
								GL.Vertex3(array[1][0], array[1][1], 0f);
								GL.Vertex3(array[2][0], array[2][1], 0f);
								Array.Copy(array[2], array[1], 2);
								num6 = num10;
								num9++;
							}
							GL.End();
						}
					}
					finally
					{
						renderPass2.End();
					}
					num4++;
				}
				if (Config.COUNT_ENABLE)
				{
					Config.drawShapeCount_++;
				}
			}
			finally
			{
				EndRenderMatrix();
			}
		}

		public void FillRect(float x, float y, float width, float height)
		{
			BeginRenderMatrix(ref x, ref y);
			try
			{
				UpdateGLClip();
				Vector3[] transRectGL = GetTransRectGL(x, y, width, height, FillRect_Vertices);
				RenderPass[] renderPass = GetRenderPass(guiStyle_.normal.textColor);
				foreach (RenderPass renderPass2 in renderPass)
				{
					Color color = renderPass2.BeginShape()[0];
					try
					{
						kairo.unity.graphics.Material material = ((renderPass2.Material != null) ? mateMan_.GetMaterial(renderPass2.Material, null) : mateMan_.GetMaterial(this, null, color.a));
						if (!IsRenderGUIMode())
						{
							MeshBuilder meshBuilder = meshManList_[meshId_].GetMeshBuilder(material);
							meshBuilder.ReadyAddition();
							for (int j = 0; j < transRectGL.Length; j++)
							{
								meshBuilder.AddVertex(transRectGL[j].x, transRectGL[j].y, transRectGL[j].z, color);
							}
							meshBuilder.AddQuadIndicies();
							continue;
						}
						material.SetPass(0);
						GL.Begin(7);
						GL.Color(color);
						for (int k = 0; k < transRectGL.Length; k++)
						{
							GL.Vertex3(transRectGL[k].x, transRectGL[k].y, transRectGL[k].z);
						}
						GL.End();
					}
					finally
					{
						renderPass2.End();
					}
				}
			}
			finally
			{
				EndRenderMatrix();
			}
			if (Config.COUNT_ENABLE)
			{
				Config.drawShapeCount_++;
			}
		}

		internal void FillClipRect(float x, float y, float width, float height)
		{
			Vector3[] transRectGL = GetTransRectGL(x, y, width, height, FillRect_Vertices, false);
			RenderPass[] renderPass = GetRenderPass(guiStyle_.normal.textColor);
			foreach (RenderPass renderPass2 in renderPass)
			{
				Color color = renderPass2.BeginShape()[0];
				try
				{
					kairo.unity.graphics.Material material = ((renderPass2.Material != null) ? mateMan_.GetMaterial(renderPass2.Material, null) : mateMan_.GetMaterial(this, null, color.a));
					if (!IsRenderGUIMode())
					{
						MeshBuilder meshBuilder = meshManList_[meshId_].GetMeshBuilder(material);
						meshBuilder.ReadyAddition();
						for (int j = 0; j < transRectGL.Length; j++)
						{
							meshBuilder.AddVertex(transRectGL[j].x, transRectGL[j].y, transRectGL[j].z, color);
						}
						meshBuilder.AddQuadIndicies();
						continue;
					}
					material.SetPass(0);
					GL.Begin(7);
					GL.Color(color);
					for (int k = 0; k < transRectGL.Length; k++)
					{
						GL.Vertex3(transRectGL[k].x, transRectGL[k].y, transRectGL[k].z);
					}
					GL.End();
				}
				finally
				{
					renderPass2.End();
				}
			}
		}

		public void FillRectPoint(float left, float top, float right, float bottom)
		{
			FillRect(left, top, right - left, bottom - top);
		}

		public void FillRoundRect(float x, float y, float width, float height, float r)
		{
			FillArc(x, y, r * 2f, r * 2f, 90f, 90f);
			FillArc(x + width - r * 2f, y, r * 2f, r * 2f, 0f, 90f);
			FillArc(x + width - r * 2f, y + height - r * 2f, r * 2f, r * 2f, 270f, 90f);
			FillArc(x, y + height - r * 2f, r * 2f, r * 2f, 180f, 90f);
			FillRect(x, y + r, r, height - r * 2f);
			FillRect(x + r, y, width - r * 2f, height);
			FillRect(x + width - r, y + r, r, height - r * 2f);
		}

		public void FillTriangle(int x1, int y1, int x2, int y2, int x3, int y3)
		{
			FillTriangle((float)x1, (float)y1, (float)x2, (float)y2, (float)x3, (float)y3);
		}

		public void FillTriangle(float x1, float y1, float x2, float y2, float x3, float y3)
		{
			if (BeginRenderMatrix(ref x1, ref y1))
			{
				x2 -= x1;
				y2 -= y1;
				x3 -= x1;
				y3 -= y1;
			}
			try
			{
				UpdateGLClip();
				float[] transPointGL = GetTransPointGL(x1, y1, FillTriangle_tmp1);
				float[] transPointGL2 = GetTransPointGL(x2, y2, FillTriangle_tmp2);
				float[] transPointGL3 = GetTransPointGL(x3, y3, FillTriangle_tmp3);
				RenderPass[] renderPass = GetRenderPass(guiStyle_.normal.textColor);
				foreach (RenderPass renderPass2 in renderPass)
				{
					Color color = renderPass2.BeginShape()[0];
					try
					{
						kairo.unity.graphics.Material material = ((renderPass2.Material != null) ? mateMan_.GetMaterial(renderPass2.Material, null) : mateMan_.GetMaterial(this, null, color.a));
						if (!IsRenderGUIMode())
						{
							MeshBuilder meshBuilder = meshManList_[meshId_].GetMeshBuilder(material);
							meshBuilder.ReadyAddition();
							meshBuilder.AddVertex(transPointGL[0], transPointGL[1], 0f, color);
							meshBuilder.AddVertex(transPointGL2[0], transPointGL2[1], 0f, color);
							meshBuilder.AddVertex(transPointGL3[0], transPointGL3[1], 0f, color);
							meshBuilder.AddTriangleIndicies();
						}
						else
						{
							material.SetPass(0);
							GL.Begin(4);
							GL.Color(color);
							GL.Vertex3(transPointGL[0], transPointGL[1], 0f);
							GL.Vertex3(transPointGL2[0], transPointGL2[1], 0f);
							GL.Vertex3(transPointGL3[0], transPointGL3[1], 0f);
							GL.End();
						}
					}
					finally
					{
						renderPass2.End();
					}
				}
				if (Config.COUNT_ENABLE)
				{
					Config.drawShapeCount_++;
				}
			}
			finally
			{
				EndRenderMatrix();
			}
		}

		public static int GetColorOfName(int name)
		{
			return name;
		}

		public static int GetColorOfRGB(int r, int g, int b)
		{
			return GetColorOfRGB(r, g, b, 255);
		}

		public static int GetColorOfRGB(int r, int g, int b, int a)
		{
			if (r < 0)
			{
				r = 0;
			}
			if (r > 255)
			{
				r = 255;
			}
			if (g < 0)
			{
				g = 0;
			}
			if (g > 255)
			{
				g = 255;
			}
			if (b < 0)
			{
				b = 0;
			}
			if (b > 255)
			{
				b = 255;
			}
			if (a < 0)
			{
				a = 0;
			}
			if (a > 255)
			{
				a = 255;
			}
			return (a << 24) | (r << 16) | (g << 8) | b;
		}

		public static int[] GetAnchorPosition(int x, int y, int width, int height, int anchor)
		{
			if ((anchor & 2) != 0)
			{
				x -= width / 2;
			}
			else if ((anchor & 4) != 0)
			{
				x -= width;
			}
			if ((anchor & 0x20) != 0)
			{
				y -= height / 2;
			}
			else if ((anchor & 0x40) != 0)
			{
				y -= height;
			}
			getAnchorPosition_tmp[0] = x;
			getAnchorPosition_tmp[1] = y;
			return getAnchorPosition_tmp;
		}

		public static float[] GetAnchorPositionF(float x, float y, float width, float height, int anchor)
		{
			if ((anchor & 2) != 0)
			{
				x -= (float)(int)(width / 2f);
			}
			else if ((anchor & 4) != 0)
			{
				x -= width;
			}
			if ((anchor & 0x20) != 0)
			{
				y -= (float)(int)(height / 2f);
			}
			else if ((anchor & 0x40) != 0)
			{
				y -= height;
			}
			getAnchorPositionF_tmp[0] = x;
			getAnchorPositionF_tmp[1] = y;
			return getAnchorPositionF_tmp;
		}

		public void Lock()
		{
		}

		public void SetColor(int c)
		{
			color_ = c;
			guiStyle_.normal.textColor = ArgbToColor(c);
		}

		public void SetColor(int r, int g, int b)
		{
			SetColor(GetColorOfRGB(r, g, b));
		}

		public void SetColor(int r, int g, int b, int a)
		{
			SetColor(GetColorOfRGB(r, g, b, a));
		}

		public int GetColor()
		{
			return color_;
		}

		public void SetFlipMode(int flipmode)
		{
			flipmode_ = flipmode;
		}

		public void SetFont(Font f)
		{
			font_.Copy(f);
			font_.AdjustFontSize(GetScale());
		}

		public Font GetFont(Font copy = null)
		{
			if (copy == null)
			{
				return new Font(font_);
			}
			copy.Copy(font_);
			return copy;
		}

		public void PushFont()
		{
			if (fontCount_ >= fontStack_.Count)
			{
				fontStack_.Add(new Font());
			}
			fontStack_[fontCount_].Set(font_);
			fontCount_++;
		}

		public void PushFont(Font f)
		{
			PushFont();
			SetFont(f);
		}

		public void PushFont(int fontSize)
		{
			Font font = GetFont();
			font.SetSize(fontSize);
			PushFont(font);
		}

		public void PopFont()
		{
			if (this.fontCount_ <= 0)
			{
				return;
			}
			kairo.unity.ui.Font item = this.fontStack_[this.fontCount_ - 1];
			this.fontCount_--;
			this.SetFont(item);
		}

		public void SetOrigin(int x, int y)
		{
			originX_ = x;
			originY_ = y;
			UpdateWorldMatrix();
		}

		public int GetOriginX()
		{
			return originX_;
		}

		public int GetOriginY()
		{
			return originY_;
		}

		public void ResetRender()
		{
			if (defaultGuiStyle_ == null)
			{
				defaultGuiStyle_ = new GUIStyle();
			}
			isGuiGroupRect_ = false;
			if (guiStyle_ == null)
			{
				guiStyle_ = new GUIStyle();
			}
			guiStyle_.font = defaultGuiStyle_.font;
			guiStyle_.fontSize = defaultGuiStyle_.fontSize;
			guiStyle_.fontStyle = defaultGuiStyle_.fontStyle;
			Font.UnlockLocalizeSetting();
			Font.ClearLocalizeSettings();
			renderModeCount_ = 0;
			blendModeCount_ = 0;
			originCount_ = 0;
			fontCount_ = 0;
			clipCount_ = 0;
			clipEnableCount_ = 0;
			renderQueueCount_ = 0;
			imageClipCount_ = 0;
			stackStateCount_ = 0;
			autoTextLayoutCount_ = 0;
			fontScaleOffsetEnable_ = true;
			ClearImageClip();
			renderCache_.Reset();
			SetClipEnable(true);
			isClipRegionGL = false;
			ClearRenderRect();
			SetRenderMode(0, 255, 0);
			SetFlipMode(0);
			SetColor(GetColorOfRGB(0, 0, 0));
			SetPictoColorEnabled(false);
			ClearClip();
			SetRenderQueue(0);
			SetClipEnable(isGUI_);
			TextLayout.SetPictographVisible(true);
			meshId_ = 0;
			blendModeCount_ = 0;
			SetBlendMode(0);
			originX_ = (originY_ = 0);
			font_.Init();
			scale_ = 100f;
			cameraDepth_ = 0;
			setFigureClip_ = false;
			useFigureClip_ = false;
			localMatrixIndex_ = -1;
			worldMatrix_.Reset();
			localCoordinateMatrix_.Reset();
			localCoordinateCount_ = 0;
			renderCache_.Reset();
			Scale(100f);
		}

		public void Init()
		{
			ResetRender();
		}

		public void Unlock(bool forced)
		{
		}

		public void CopyArea(int x, int y, int width, int height, int dx, int dy)
		{
		}

		public bool IsRenderModeEnable()
		{
			return true;
		}

		public void SetRenderMode()
		{
			SetRenderMode(0, 255, 0);
		}

		public void SetRenderMode(int op, int srcRatio)
		{
			int dstRatio = 255;
			if (op == 1)
			{
				dstRatio = 255 - srcRatio;
			}
			SetRenderMode(op, srcRatio, dstRatio);
		}

		public void SetRenderMode(int op, int srcRatio, int dstRatio)
		{
			if (op == 1 && srcRatio + dstRatio == 256)
			{
				dstRatio--;
			}
			renderOperator_ = op;
			renderSrcRatio_ = srcRatio;
			renderDstRatio_ = dstRatio;
			renderIsReplace_ = false;
			if (renderOperator_ == 0 && renderSrcRatio_ >= 255)
			{
				renderIsReplace_ = true;
			}
			if (renderOperator_ == 1 && renderSrcRatio_ >= 255 && renderDstRatio_ == 0)
			{
				renderIsReplace_ = true;
			}
			renderCache_.Reset();
		}

		public int GetRenderModeOperator()
		{
			return renderOperator_;
		}

		public int GetRenderModeSrcRatio()
		{
			return renderSrcRatio_;
		}

		public int GetRenderModeDstRatio()
		{
			return renderDstRatio_;
		}

		public void PushRenderMode()
		{
			if (renderModeCount_ >= renderModeStack_.Count)
			{
				renderModeStack_.Add(new int[3]);
			}
			int[] array = renderModeStack_[renderModeCount_];
			array[0] = renderOperator_;
			array[1] = renderSrcRatio_;
			array[2] = renderDstRatio_;
			renderModeCount_++;
		}

		public void PushRenderMode(int op, int srcRatio, int dstRatio)
		{
			PushRenderMode();
			SetRenderMode(op, srcRatio, dstRatio);
		}

		public void PopRenderMode()
		{
			if (this.renderModeCount_ <= 0)
			{
				return;
			}
			int[] item = this.renderModeStack_[this.renderModeCount_ - 1];
			this.renderModeCount_--;
			this.SetRenderMode(item[0], item[1], item[2]);
		}

		public static bool CheckAlphaRectEnable()
		{
			return true;
		}

		public static bool CheckTransparentImageEnable()
		{
			return true;
		}

		public void SetPictoColorEnabled(bool b)
		{
		}

		public void Scale(float scaleRatio)
		{
			scale_ = scaleRatio;
			UpdateWorldMatrix();
		}

		public float GetScale()
		{
			return scale_;
		}

		public void LinearFilter(bool enable)
		{
			linearFilter_ = enable;
		}

		public bool IsLinearFilter()
		{
			return linearFilter_;
		}

		public void SetRenderRect(int x, int y, int w, int h)
		{
			renderRectOrigin_[0] = x;
			renderRectOrigin_[1] = y;
			renderRectOrigin_[2] = w;
			renderRectOrigin_[3] = h;
			ClearRenderRect();
			renderRect_[0] = (int)((float)x * scale_ / 100f + 0.01f);
			renderRect_[1] = (int)((float)y * scale_ / 100f + 0.01f);
			renderRect_[2] = (int)((float)w * scale_ / 100f + 0.01f);
			renderRect_[3] = (int)((float)h * scale_ / 100f + 0.01f);
			SetOrigin(0, 0);
			ClearClip();
		}

		public int[] GetRenderRect()
		{
			JSystem.Arraycopy(clipRect_, 0, getRenderRect_tmp, 0, getRenderRect_tmp.Length);
			return getRenderRect_tmp;
		}

		public void ClearRenderRect()
		{
			renderRect_[0] = 0;
			renderRect_[1] = 0;
			renderRect_[2] = width_;
			renderRect_[3] = height_;
			SetOrigin(0, 0);
			ClearClip();
		}

		public void Screen(int w, int h)
		{
			int num = 1024;
			int num2 = 1024;
			if (render_ != null)
			{
				num = render_.GetWidth();
				num2 = render_.GetHeight();
			}
			backupMatrix_.Set(worldMatrix_);
			worldMatrix_.Reset();
			SetRenderMode(1, 128);
			SetColor(0, 0, 0);
			FillRect(0f, 0f, num, num2);
			SetRenderMode();
			worldMatrix_.Set(backupMatrix_);
			SetMatrix(backupMatrix_);
			if (render_ != null)
			{
				num = render_.GetWidth();
				num2 = render_.GetHeight();
			}
			int x = (num - w) / 2;
			int y = (num2 - h) / 2;
			SetRenderRect(x, y, w, h);
		}

		public void SetMatrix(Matrix m)
		{
			if (localMatrixIndex_ == -1)
			{
				if (m == null)
				{
					worldMatrix_.Reset();
				}
				else
				{
					worldMatrix_.Set(m);
				}
			}
			else if (m == null)
			{
				localMatrixStack_[localMatrixIndex_][0].Reset();
			}
			else
			{
				localMatrixStack_[localMatrixIndex_][0].Set(m);
			}
			UpdateScreenMatrix();
		}

		public Matrix GetMatrix(Matrix dst = null)
		{
			if (dst == null)
			{
				dst = new Matrix();
			}
			if (localMatrixIndex_ == -1)
			{
				dst.Set(worldMatrix_);
			}
			else
			{
				dst.Set(localMatrixStack_[localMatrixIndex_][0]);
			}
			return dst;
		}

		public void PushMatrix(Matrix matrix = null)
		{
			localMatrixIndex_++;
			if (localMatrixIndex_ >= localMatrixStack_.Length)
			{
				Matrix[][] array = new Matrix[localMatrixIndex_ + 1][];
				Array.Copy(localMatrixStack_, 0, array, 0, localMatrixStack_.Length);
				int num = array.Length - 1;
				while (num >= 0 && array[num] == null)
				{
					array[num] = new Matrix[2]
					{
						new Matrix(),
						new Matrix()
					};
					num--;
				}
				localMatrixStack_ = array;
			}
			if (localMatrixIndex_ - 1 < 0)
			{
				localMatrixStack_[localMatrixIndex_][1].Reset();
			}
			else
			{
				localMatrixStack_[localMatrixIndex_][1].SetConcat(localMatrixStack_[localMatrixIndex_ - 1][0], localMatrixStack_[localMatrixIndex_ - 1][1]);
			}
			SetMatrix(matrix);
		}

		public void PopMatrix()
		{
			localMatrixIndex_--;
			if (localMatrixIndex_ < -1)
			{
				localMatrixIndex_ = -1;
				if (Config.PRINT)
				{
					Log.Info("PopMatrix の呼び出し回数が PushMatrix を超えています");
				}
			}
			UpdateScreenMatrix();
		}

		public bool CheckLocalMatrix()
		{
			if (localMatrixIndex_ < 0)
			{
				return localCoordinateCount_ > 0;
			}
			return true;
		}

		public void ClearLocalMatrix()
		{
			localMatrixIndex_ = -1;
			UpdateScreenMatrix();
		}

		private void UpdateWorldMatrix()
		{
			worldMatrix_.Reset();
			worldMatrix_.PreTranslate(renderRect_[0], renderRect_[1]);
			worldMatrix_.PreScale(scale_ / 100f, scale_ / 100f);
			worldMatrix_.PreTranslate(originX_, originY_);
			UpdateScreenMatrix();
		}

		private void UpdateScreenMatrix()
		{
			if (localMatrixIndex_ <= -1)
			{
				screenMatrix_.Reset();
			}
			else
			{
				screenMatrix_.SetConcat(localMatrixStack_[localMatrixIndex_][0], localMatrixStack_[localMatrixIndex_][1]);
			}
			screenMatrix_.PostConcat(worldMatrix_);
		}

		public Matrix GetLocalMatrix(Matrix dst = null)
		{
			if (dst == null)
			{
				dst = new Matrix();
			}
			dst.Reset();
			if (localMatrixIndex_ >= 0)
			{
				dst.SetConcat(localMatrixStack_[localMatrixIndex_][0], localMatrixStack_[localMatrixIndex_][1]);
			}
			return dst;
		}

		public Matrix GetScreenMatrix(Matrix dst = null)
		{
			if (dst == null)
			{
				dst = new Matrix();
			}
			dst.Set(screenMatrix_);
			return dst;
		}

		public void SetSharpnessEnable(bool enable)
		{
			SetSharpnessEnable(enable, 1, int.MaxValue, int.MaxValue);
		}

		public void SetSharpnessEnable(bool enable, int quality, int limitedWidth, int limitedHeight)
		{
		}

		public int[] GetSharpnessInfo()
		{
			return getSharpnessInfo_result;
		}

		public static void disposeSharpnessBuffer()
		{
		}

		public void UpdateGUIGroup()
		{
			if (this.useFigureClip_ || this.setFigureClip_)
			{
				return;
			}
			if (kairo.unity.ui.Graphics.isGuiGroupRect_ && kairo.unity.ui.Graphics.guiGroupRect_.Equals(this.clipRegion_))
			{
				return;
			}
			if (kairo.unity.ui.Graphics.guiBeginGroup_)
			{
				kairo.unity.ui.Graphics.guiBeginGroup_ = false;
				try
				{
					GUI.EndGroup();
				}
				catch (Exception exception)
				{
				}
			}
			kairo.unity.ui.Graphics.guiGroupRect_.Set(this.clipRegion_);
			kairo.unity.ui.Graphics.isGuiGroupRect_ = true;
			GUI.BeginGroup(new Rect((float)kairo.unity.ui.Graphics.guiGroupRect_.left, (float)kairo.unity.ui.Graphics.guiGroupRect_.top, (float)kairo.unity.ui.Graphics.guiGroupRect_.Width(), (float)kairo.unity.ui.Graphics.guiGroupRect_.Height()));
			kairo.unity.ui.Graphics.guiBeginGroup_ = true;
		}

		internal static void InitGUIGroup()
		{
			guiBeginGroup_ = false;
			isGuiGroupRect_ = false;
		}

		internal static void EndGUIGroup()
		{
			if (guiBeginGroup_)
			{
				guiBeginGroup_ = false;
				try
				{
					GUI.EndGroup();
				}
				catch (Exception)
				{
				}
			}
			isGuiGroupRect_ = false;
		}

		internal RectangleF GetTransRect(RectangleF rect, RectangleF output = null)
		{
			Matrix matrix = screenMatrix_;
			float x = rect.left;
			float y = rect.top;
			matrix.MapPoints(ref x, ref y);
			float x2 = rect.right;
			float y2 = rect.bottom;
			matrix.MapPoints(ref x2, ref y2);
			if (output == null)
			{
				output = new RectangleF();
			}
			output.Set(x, y, x2, y2);
			return output;
		}

		internal RectangleF GetTransRectGUI(RectangleF rect, RectangleF output = null)
		{
			output = GetTransRect(rect, output);
			output.left -= clipRegion_.left;
			output.right -= clipRegion_.left;
			output.top -= clipRegion_.top;
			output.bottom -= clipRegion_.top;
			return output;
		}

		internal Vector3[] GetTransRectGL(RectangleF rect, Vector3[] output)
		{
			return GetTransRectGL(rect.left, rect.top, rect.Width(), rect.Height(), output);
		}

		internal RectangleF GetTransRect(float x, float y, float width, float height, RectangleF dst = null)
		{
			if (dst == null)
			{
				dst = new RectangleF();
			}
			dst.Set(x, y, x + width, y + height);
			return GetTransRect(dst, dst);
		}

		internal RectangleF GetTransRectGUI(float x, float y, float width, float height, RectangleF dst = null)
		{
			if (dst == null)
			{
				dst = new RectangleF();
			}
			dst.Set(x, y, x + width, y + height);
			return GetTransRectGUI(dst, dst);
		}

		internal Vector3[] GetTransRectGL(float x, float y, float width, float height, Vector3[] output = null, bool transform = true)
		{
			if (output == null)
			{
				output = new Vector3[4];
			}
			output[0].x = x;
			output[0].y = y;
			output[1].x = x + width;
			output[1].y = y;
			output[2].x = x + width;
			output[2].y = y + height;
			output[3].x = x;
			output[3].y = y + height;
			float num;
if ((!meshManList_[meshId_].IsDynamic())) { num = 0f; } else { if ((render_ != null)) { num = ((float)render_.GetHeight()); } else { if ((camera_ != null)) { num = ((float)camera_.pixelHeight); } else { num = ((float)UnityEngine.Screen.height); } } }
			for (int i = 0; i < output.Length; i++)
			{
				output[i].z = 0f;
				if (transform)
				{
					screenMatrix_.MapPoints(ref output[i].x, ref output[i].y);
				}
				output[i].y = num - output[i].y;
			}
			return output;
		}

		internal float[] GetTransPoint(float x, float y, float[] output = null)
		{
			screenMatrix_.MapPoints(ref x, ref y);
			if (output == null)
			{
				output = new float[2];
			}
			output[0] = x;
			output[1] = y;
			return output;
		}

		internal float[] GetTransPointGUI(float x, float y, float[] output = null)
		{
			output = GetTransPoint(x, y, output);
			output[0] -= clipRegion_.left;
			output[1] -= clipRegion_.top;
			return output;
		}

		internal float[] GetTransPointGL(float x, float y, float[] output = null)
		{
			output = GetTransPoint(x, y, output);
			float num;
if ((!meshManList_[meshId_].IsDynamic())) { num = 0f; } else { if ((render_ != null)) { num = ((float)render_.GetHeight()); } else { if ((camera_ != null)) { num = ((float)camera_.pixelHeight); } else { num = ((float)UnityEngine.Screen.height); } } }
			output[1] = num - output[1];
			return output;
		}

		internal void GetTransPointGL(Vector3[] vertices)
		{
			float num;
if ((!meshManList_[meshId_].IsDynamic())) { num = 0f; } else { if ((render_ != null)) { num = ((float)render_.GetHeight()); } else { if ((camera_ != null)) { num = ((float)camera_.pixelHeight); } else { num = ((float)UnityEngine.Screen.height); } } }
			for (int i = 0; i < vertices.Length; i++)
			{
				screenMatrix_.MapPoints(ref vertices[i].x, ref vertices[i].y);
				vertices[i].y = num - vertices[i].y;
			}
		}

		public void SetBlendMode(int mode, int color = 0)
		{
			if (mode == 0 || mode == 1 || mode == 2 || mode == 3)
			{
				blendMode_ = mode;
				blendRgba_ = color;
				blendColor_ = ArgbToColor(color);
				renderCache_.Reset();
				return;
			}
			throw new ArgumentException();
		}

		public void SetBlendMode(int mode, int r, int g, int b, int a = 255)
		{
			SetBlendMode(mode, GetColorOfRGB(r, g, b, a));
		}

		public int GetBlendMode()
		{
			return blendMode_;
		}

		public int GetBlendColor()
		{
			return blendRgba_;
		}

		public void PushBlendMode()
		{
			if (blendModeCount_ >= blendModeStack_.Count)
			{
				blendModeStack_.Add(new int[2]);
			}
			int[] array = blendModeStack_[blendModeCount_];
			array[0] = blendMode_;
			array[1] = blendRgba_;
			blendModeCount_++;
		}

		public void PushBlendMode(int mode, int color = 0)
		{
			PushBlendMode();
			SetBlendMode(mode, color);
		}

		public void PushBlendMode(int mode, int r, int g, int b, int a = 255)
		{
			PushBlendMode(mode, GetColorOfRGB(r, g, b, a));
		}

		public void PopBlendMode()
		{
			if (this.blendModeCount_ <= 0)
			{
				return;
			}
			int[] item = this.blendModeStack_[this.blendModeCount_ - 1];
			this.SetBlendMode(item[0], item[1]);
			this.blendModeCount_--;
		}

		protected void UpdateGLClip()
		{
			if (useFigureClip_ || setFigureClip_) {
				return;
			}
if ((isClipRegionGL && clipRegionGL_.Equals(clipRegion_))) {
				return;
			}
			clipRegionGL_.Set(clipRegion_);
			isClipRegionGL = true;
			if (!(clipEnable_)) { return; }
if (IsRenderGUIMode())
				{
					GLClear(true, false, true);
					MaterialManager.GetInstance().GetMaterial(this, null, 1f, true).SetPass(0);
					GL.PushMatrix();
					GL.LoadPixelMatrix();
					GL.Viewport(new Rect(0f, 0f, UnityEngine.Screen.width, UnityEngine.Screen.height));
					GL.Begin(7);
					float num = clipRegionGL_.left;
					float num2 = clipRegionGL_.Width();
					float num3 = clipRegionGL_.top;
					float num4 = clipRegionGL_.Height();
					num3 = (float)UnityEngine.Screen.height - num3 - num4;
					GL.Vertex3(num, num3, 0f);
					GL.Vertex3(num, num3 + num4, 0f);
					GL.Vertex3(num + num2, num3 + num4, 0f);
					GL.Vertex3(num + num2, num3, 0f);
					GL.End();
					GL.PopMatrix();
				}
				else if (offscreen_ != null)
				{
					offscreen_.Flush();
					GLClear(true, false, true);
					setFigureClip_ = true;
					isFigureClipMask_ = false;
					FillClipRect(clipRegionGL_.left, clipRegionGL_.top, clipRegionGL_.Width(), clipRegionGL_.Height());
					setFigureClip_ = false;
					offscreen_.Flush();
				}
				else if (glRender_)
				{
					FlushGLRender(true);
					GLClear(true, false, true);
					setFigureClip_ = true;
					isFigureClipMask_ = false;
					FillClipRect(clipRegionGL_.left, clipRegionGL_.top, clipRegionGL_.Width(), clipRegionGL_.Height());
					setFigureClip_ = false;
					FlushGLRender(true);
				}
		}

		public static Color ArgbToColor(int argb)
		{
			Color result ;
			result.a = (float)((argb >> 24) & 0xFF) / 255f;
			result.r = (float)((argb >> 16) & 0xFF) / 255f;
			result.g = (float)((argb >> 8) & 0xFF) / 255f;
			result.b = (float)(argb & 0xFF) / 255f;
			return result;
		}

		public static Color RgbToColor(int rgb)
		{
			Color result ;
			result.a = 1f;
			result.r = (float)((rgb >> 16) & 0xFF) / 255f;
			result.g = (float)((rgb >> 8) & 0xFF) / 255f;
			result.b = (float)(rgb & 0xFF) / 255f;
			return result;
		}

		public static int ColorToArgb(Color color)
		{
			int num = (int)(color.a * 255f + 0.001f);
			int num2 = (int)(color.r * 255f + 0.001f);
			int num3 = (int)(color.g * 255f + 0.001f);
			int num4 = (int)(color.b * 255f + 0.001f);
			if (num > 255)
			{
				num = 255;
			}
			if (num < 0)
			{
				num = 0;
			}
			if (num2 > 255)
			{
				num2 = 255;
			}
			if (num2 < 0)
			{
				num2 = 0;
			}
			if (num3 > 255)
			{
				num3 = 255;
			}
			if (num3 < 0)
			{
				num3 = 0;
			}
			if (num4 > 255)
			{
				num4 = 255;
			}
			if (num4 < 0)
			{
				num4 = 0;
			}
			return (num << 24) | (num2 << 16) | (num3 << 8) | num4;
		}

		public static int ColorToRgb(Color color)
		{
			return -16777216 | ((int)(color.r * 255f + 0.001f) << 16) | ((int)(color.g * 255f + 0.001f) << 8) | (int)(color.b * 255f + 0.001f);
		}

		protected RenderPass[] GetRenderPass()
		{
			return GetRenderPass((Image)null);
		}

		protected RenderPass[] GetRenderPass(Image image)
		{
			if (GetRenderPass_tmp == null)
			{
				GetRenderPass_tmp = new Color[1] { Color.white };
			}
			return GetRenderPass(image, GetRenderPass_tmp);
		}

		protected RenderPass[] GetRenderPass(Color color)
		{
			GetRenderPass_color[0] = color;
			return GetRenderPass(null, GetRenderPass_color);
		}

		protected RenderPass[] GetRenderPass(params Color[] colors)
		{
			return GetRenderPass(null, colors);
		}

		protected RenderPass[] GetRenderPass(Image image, Color color)
		{
			GetRenderPass_color[0] = color;
			return GetRenderPass(image, GetRenderPass_color);
		}

		protected RenderPass[] GetRenderPass(Image image, params Color[] colors)
		{
			if (GetRenderPass_Buffer == null)
			{
				GetRenderPass_Buffer = new RenderPass[5][];
				for (int i = 0; i < GetRenderPass_Buffer.Length; i++)
				{
					GetRenderPass_Buffer[i] = new RenderPass[i];
					for (int j = 0; j < GetRenderPass_Buffer[i].Length; j++)
					{
						GetRenderPass_Buffer[i][j] = new RenderPass();
					}
				}
			}
			int num = -1;
			if (image != null)
			{
				num = image.GetAlphaType();
				if (num == 3 && IsRenderGUIMode())
				{
					num = -1;
				}
			}
			RenderPass[] array = GetRenderPass_Buffer[1];
			int num2 = 0;
			if (renderForceMaterial_ != null)
			{
				array[num2].Init(this, renderForceMaterial_, colors);
			}
			else
			{
				bool flag = renderIsReplace_;
				if (setFigureClip_)
				{
					flag = true;
				}
				if (num == 3 && !IsRenderGUIMode())
				{
					array = GetRenderPass_Buffer[array.Length * 2];
				}
				if (!flag && (renderOperator_ == 1 || renderOperator_ == 2))
				{
					if (renderDstRatio_ + renderSrcRatio_ == 255)
					{
						array[num2++].Init(this, renderOperator_, renderSrcRatio_, renderDstRatio_, 255, renderSrcRatio_, colors);
					}
					else
					{
						int num3 = 0;
						if (renderDstRatio_ < 255)
						{
							num3++;
						}
						if (renderSrcRatio_ > 0)
						{
							num3++;
						}
						if (num == 3 && !IsRenderGUIMode())
						{
							num3 *= 2;
						}
						array = GetRenderPass_Buffer[num3];
						if (renderDstRatio_ < 255)
						{
							array[num2++].Init(this, renderOperator_, 0, renderDstRatio_, 255, 255 - renderDstRatio_, colors);
						}
						if (renderSrcRatio_ > 0)
						{
							array[num2++].Init(this, renderOperator_, renderSrcRatio_, 255, 255, renderSrcRatio_, colors);
						}
					}
				}
				else
				{
					array[num2++].Init(this, -1, 255, 255, renderSrcRatio_, 255, colors);
				}
				if (num == 3)
				{
					if (!IsRenderGUIMode())
					{
						for (int k = 0; k < num2; k++)
						{
							array[k].opaqueTransOrder_ = 1;
							array[num2 + k].Copy(array[k]);
							array[num2 + k].opaqueTransOrder_ = 2;
						}
					}
					else
					{
						array[0].opaqueTransOrder_ = 2;
					}
				}
			}
			return array;
		}

		public bool IsGUI()
		{
			return isGUI_;
		}

		internal bool IsRenderGUIMode()
		{
			if (isGUI_)
			{
				return !glRender_;
			}
			return false;
		}

		public MeshManager GetMeshManager()
		{
			return meshManList_[meshId_];
		}

		public MeshManager GetMeshManager(int id)
		{
			return meshManList_[id];
		}

		public static void SetAutoTextLayout(bool enable)
		{
			autoTextLayout_ = enable;
		}

		public static bool IsAutoTextLayout()
		{
			return autoTextLayout_;
		}

		internal static void PushAutoTextLayout(bool enable)
		{
			if (autoTextLayoutStack_ == null)
			{
				autoTextLayoutStack_ = new List<bool>();
			}
			if (autoTextLayoutCount_ >= autoTextLayoutStack_.Count)
			{
				autoTextLayoutStack_.Add(false);
			}
			autoTextLayoutStack_[autoTextLayoutCount_] = enable;
			autoTextLayoutCount_++;
		}

		internal static bool PopAutoTextLayout()
		{
			if (autoTextLayoutCount_ <= 0 || autoTextLayoutStack_ == null)
			{
				return autoTextLayout_;
			}
			autoTextLayoutCount_--;
			return autoTextLayoutStack_[autoTextLayoutCount_];
		}

		internal static bool PeekAutoTextLayout()
		{
			if (autoTextLayoutCount_ <= 0 || autoTextLayoutStack_ == null)
			{
				return autoTextLayout_;
			}
			return autoTextLayoutStack_[autoTextLayoutCount_ - 1];
		}

		public bool PushClipEnable(bool enable)
		{
			PushClipEnable();
			return SetClipEnable(enable);
		}

		public void PushClipEnable()
		{
			if (clipEnableCount_ >= clipEnableStack_.Count)
			{
				clipEnableStack_.Add(true);
			}
			clipEnableStack_[clipEnableCount_] = clipEnable_;
			clipEnableCount_++;
		}

		public void PopClipEnable()
		{
			if (this.clipEnableCount_ <= 0)
			{
				return;
			}
			bool item = this.clipEnableStack_[this.clipEnableCount_ - 1];
			this.clipEnableCount_--;
			this.SetClipEnable(item);
		}

		public bool SetClipEnable(bool enable)
		{
			if (enable && !IsStencil())
			{
				return false;
			}
			clipEnable_ = enable;
			isClipRegionGL = false;
			return true;
		}

		public bool IsClipEnable()
		{
			return clipEnable_;
		}

		public void PushOrigin()
		{
			if (originCount_ >= originStack_.Count)
			{
				originStack_.Add(new int[2]);
			}
			int[] array = originStack_[originCount_];
			array[0] = originX_;
			array[1] = originY_;
			originCount_++;
		}

		public void PushOrigin(int x, int y, bool concat = false)
		{
			if (originCount_ >= originStack_.Count)
			{
				originStack_.Add(new int[2]);
			}
			int[] array = originStack_[originCount_];
			array[0] = originX_;
			array[1] = originY_;
			originCount_++;
			if (concat)
			{
				x += originX_;
				y += originY_;
			}
			SetOrigin(x, y);
		}

		public void PopOrigin()
		{
			if (this.originCount_ <= 0)
			{
				return;
			}
			int[] item = this.originStack_[this.originCount_ - 1];
			this.originCount_--;
			this.SetOrigin(item[0], item[1]);
		}

		private bool BeginRenderMatrix(ref float x, ref float y)
		{
			if (!CheckLocalMatrix())
			{
				return false;
			}
			if (backupWorldMatrixCount_ >= backupWorldMatrix_.Count)
			{
				backupWorldMatrix_.Add(new Matrix());
			}
			backupWorldMatrix_[backupWorldMatrixCount_].Set(worldMatrix_);
			backupWorldMatrixCount_++;
			if (localCoordinateCount_ == 0)
			{
				worldMatrix_.PreTranslate(x, y);
				UpdateScreenMatrix();
			}
			else
			{
				worldMatrix_.PreConcat(localCoordinateMatrix_);
				PushMatrix(Matrix.TranslateTemporary(x, y));
			}
			x = (y = 0f);
			return true;
		}

		private bool BeginRenderMatrix(ref int x, ref int y)
		{
			if (!CheckLocalMatrix())
			{
				return false;
			}
			if (backupWorldMatrixCount_ >= backupWorldMatrix_.Count)
			{
				backupWorldMatrix_.Add(new Matrix());
			}
			backupWorldMatrix_[backupWorldMatrixCount_].Set(worldMatrix_);
			backupWorldMatrixCount_++;
			if (localCoordinateCount_ == 0)
			{
				worldMatrix_.PreTranslate(x, y);
				UpdateScreenMatrix();
			}
			else
			{
				worldMatrix_.PreConcat(localCoordinateMatrix_);
				PushMatrix(Matrix.TranslateTemporary(x, y));
			}
			x = (y = 0);
			return true;
		}

		private void EndRenderMatrix()
		{
			if (this.backupWorldMatrixCount_ <= 0)
			{
				return;
			}
			this.worldMatrix_.Set(this.backupWorldMatrix_[this.backupWorldMatrixCount_ - 1]);
			this.backupWorldMatrixCount_--;
			if (this.localCoordinateCount_ == 0)
			{
				this.UpdateScreenMatrix();
				return;
			}
			this.PopMatrix();
		}

		public void BeginLocalCoordination()
		{
			int x = 0;
			int y = 0;
			BeginLocalCoordination(ref x, ref y);
		}

		public void BeginLocalCoordination(ref float x, ref float y)
		{
			if (!(CheckLocalMatrix())) { return; }
localCoordinateCount_++;
if (localCoordinateCount_ == 1)
				{
					localCoordinateMatrix_.Set(Matrix.TranslateTemporary(x, y));
					UpdateScreenMatrix();
				}
				else
				{
					PushMatrix(Matrix.TranslateTemporary(x, y));
				}
x = (y = 0f);
		}

		public void BeginLocalCoordination(ref int x, ref int y)
		{
			if (!(CheckLocalMatrix())) { return; }
UpdateGLClip();
localCoordinateCount_++;
if (localCoordinateCount_ == 1)
				{
					localCoordinateMatrix_.Set(Matrix.TranslateTemporary(x, y));
				}
				else
				{
					PushMatrix(Matrix.TranslateTemporary(x, y));
				}
x = (y = 0);
		}

		public void EndLocalCoordination()
		{
			localCoordinateCount_--;
			if (localCoordinateCount_ <= 0)
			{
				localCoordinateCount_ = 0;
			}
			else
			{
				PopMatrix();
			}
		}

		public int StringWidth(string text)
		{
			return font_.StringWidth(null, text);
		}

		public int StringWidth(TextFormat lf, string text)
		{
			return font_.StringWidth(lf, text);
		}

		public float StringWidthF(string text)
		{
			return font_.StringWidthF(null, text);
		}

		public float StringWidthF(TextFormat lf, string text)
		{
			return font_.StringWidthF(lf, text);
		}

		public bool IsStencil()
		{
			if (render_ == null)
			{
				return true;
			}
			if (render_.GetImage() is RenderTexture)
			{
				return RenderTexture.SupportsStencil((RenderTexture)render_.GetImage());
			}
			return true;
		}

		public int GetDepthBits()
		{
			if (render_ != null)
			{
				return Offscreen.GetDepthBits();
			}
			return DepthBits;
		}

		public static Graphics GetCurrent()
		{
			return current_;
		}

		public void SetRenderQueue(int queue)
		{
			renderQueue_ = queue;
		}

		public int GetRenderQueue()
		{
			return renderQueue_;
		}

		public void PushRenderQueue()
		{
			if (renderQueueCount_ >= renderQueueStack_.Count)
			{
				renderQueueStack_.Add(0);
			}
			renderQueueStack_[renderQueueCount_] = renderQueue_;
			renderQueueCount_++;
		}

		public void PushRenderQueue(int queue)
		{
			PushRenderQueue();
			SetRenderQueue(queue);
		}

		public void PopRenderQueue()
		{
			if (this.renderQueueCount_ <= 0)
			{
				return;
			}
			this.SetRenderQueue(this.renderQueueStack_[this.renderQueueCount_ - 1]);
			this.renderQueueCount_--;
		}

		public void SetCameraDepth(int depth)
		{
			cameraDepth_ = depth;
		}

		public void BeginGLRender(bool clipEnable = true)
		{
			if (offscreen_ != null || !isGUI_)
			{
				return;
			}
			if (glRender_)
			{
				FlushGLRender();
			}
			else
			{
				MeshManager[] array = meshManList_;
				foreach (MeshManager meshManager in array)
				{
					if (meshManager != null)
					{
						meshManager.BeginMesh();
					}
				}
				glTextCount_ = 0;
				glRender_ = true;
			}
			glRenderCount_++;
			glMatrix_ = Matrix4x4.identity;
			renderCache_.Reset();
			PushClipEnable(clipEnable);
		}

		public void EndGLRender(bool force = false)
		{
			if (this.offscreen_ != null || !this.glRender_ || !this.isGUI_)
			{
				return;
			}
			this.PopClipEnable();
			this.glRenderCount_--;
			if (this.glRenderCount_ <= 0 | force)
			{
				this.FlushGLRender(false);
				this.glRenderCount_ = 0;
				this.glRender_ = false;
			}
			this.renderCache_.Reset();
		}

		public void FlushGLRender(bool meshOnly = false)
		{
			if (offscreen_ != null || !glRender_ || !isGUI_)
			{
				return;
			}
			int num = cameraDepth_ * -100 / 1500000;
			Matrix4x4 matrix4x = Matrix4x4.TRS(new Vector3(0f, 0f, num), Quaternion.identity, new Vector3(1f, 1f, 1f));
			MeshManager[] array = meshManList_;
			foreach (MeshManager meshManager in array)
			{
				if (meshManager != null)
				{
					meshManager.EndMesh();
					meshManager.RenderMesh(glMatrix_ * matrix4x);
				}
			}
			if (!meshOnly && glTextCount_ > 0)
			{
				Rectangle rectangle = clipRegion_;
				UnityEngine.Font font = guiStyle_.font;
				FontStyle fontStyle = guiStyle_.fontStyle;
				int fontSize = guiStyle_.fontSize;
				Color textColor = guiStyle_.normal.textColor;
				for (int j = 0; j < glTextCount_; j++)
				{
					GLText gLText = glTextList_[j];
					clipRegion_ = (gLText.IsClip ? gLText.Clip : null);
					UpdateGUIGroup();
					guiStyle_.font = gLText.Font;
					guiStyle_.fontStyle = gLText.Style;
					guiStyle_.fontSize = gLText.Size;
					guiStyle_.normal.textColor = gLText.Color;
					GUI.Label(gLText.Position, gLText.Text, guiStyle_);
				}
				glTextCount_ = 0;
				guiStyle_.font = font;
				guiStyle_.fontStyle = fontStyle;
				guiStyle_.fontSize = fontSize;
				guiStyle_.normal.textColor = textColor;
				clipRegion_ = rectangle;
				UpdateGUIGroup();
			}
			GLClear(true, false, false);
			array = meshManList_;
			foreach (MeshManager meshManager2 in array)
			{
				if (meshManager2 != null)
				{
					meshManager2.BeginMesh();
				}
			}
		}

		public bool IsGLRender()
		{
			return glRender_;
		}

		public void SetGLMatrix(Matrix4x4 matrix)
		{
			glMatrix_ = matrix;
		}

		public static void SetRenderCache(bool enable)
		{
			renderCacheEnable_ = enable;
		}

		public static bool IsRenderCache()
		{
			return renderCacheEnable_;
		}

		public void GLClear(bool clearDepth, bool clearColor, bool clearStencil, int color = -1)
		{
			kairo.unity.graphics.Material material = MaterialManager.GetInstance().GetMaterial(this, clearDepth, clearColor, clearStencil);
			int num = UnityEngine.Screen.width;
			int num2 = UnityEngine.Screen.height;
			if (render_ != null)
			{
				num = render_.GetWidth();
				num2 = render_.GetHeight();
			}
			else if (camera_ != null)
			{
				num = camera_.pixelWidth;
				num2 = camera_.pixelHeight;
			}
			if (color == -1)
			{
				color = GetColorOfRGB(0, 0, 0, 255);
			}
			Color color2 = ArgbToColor(color);
			float z = (isGUI_ ? (-100) : 1499901);
			if (isGUI_)
			{
				GL.PushMatrix();
				GL.LoadPixelMatrix();
				GL.Viewport(new Rect(0f, 0f, num, num2));
				material.SetPass(0);
				GL.Begin(7);
				GL.Color(color2);
				GL.Vertex3(0f, num2, z);
				GL.Color(color2);
				GL.Vertex3(num, num2, z);
				GL.Color(color2);
				GL.Vertex3(num, 0f, z);
				GL.Color(color2);
				GL.Vertex3(0f, 0f, z);
				GL.End();
				GL.PopMatrix();
				return;
			}
			material.SetPass(0);
			if (clearMesh_ == null)
			{
				clearMesh_ = new Mesh();
				clearMesh_.Clear();
				clearMesh_.vertices = new Vector3[4]
				{
					new Vector3(0f, 0f, z),
					new Vector3(num, 0f, z),
					new Vector3(num, num2, z),
					new Vector3(0f, num2, z)
				};
				clearMesh_.triangles = new int[6] { 0, 1, 2, 0, 2, 3 };
			}
			if (clearColor && (clearMesh_.colors == null || clearMesh_.colors.Length == 0 || clearMesh_.colors[0] != color2))
			{
				clearMesh_.colors = new Color[4] { color2, color2, color2, color2 };
			}
			UnityEngine.Graphics.DrawMeshNow(clearMesh_, Matrix4x4.identity);
		}

		public void PushImageClip()
		{
			if (imageClipCount_ >= imageClipStack_.Count)
			{
				imageClipStack_.Add(new float[4]);
			}
			float[] array = imageClipStack_[imageClipCount_];
			array[0] = imageClip_[0];
			array[1] = imageClip_[1];
			array[2] = imageClip_[2];
			array[3] = imageClip_[3];
			imageClipCount_++;
		}

		public void PushImageClip(float x, float y, float width, float height)
		{
			PushImageClip();
			SetImageClip(x, y, width, height);
		}

		public void PopImageClip()
		{
			if (this.imageClipCount_ <= 0)
			{
				return;
			}
			float[] item = this.imageClipStack_[this.imageClipCount_ - 1];
			this.imageClipCount_--;
			this.SetImageClip(item[0], item[1], item[2], item[3]);
		}

		public float[] GetImageClip()
		{
			if (float.IsNaN(imageClip_[0]))
			{
				return null;
			}
			return imageClip_;
		}

		public void SetImageClip(float x, float y, float width, float height)
		{
			imageClip_[0] = x;
			imageClip_[1] = y;
			imageClip_[2] = width;
			imageClip_[3] = height;
		}

		public void ClearImageClip()
		{
			imageClip_[0] = float.NaN;
			imageClip_[1] = float.NaN;
			imageClip_[2] = float.NaN;
			imageClip_[3] = float.NaN;
		}

		public static void SetOutlineWidth(float width)
		{
			outlineWidth_ = width;
		}

		public static void SetOutlineQuality(int quality)
		{
			outlineQuality_ = quality;
		}

		public static float GetOutlineWidth()
		{
			return outlineWidth_;
		}

		public static int GetOutlineQuality()
		{
			return outlineQuality_;
		}

		public void SetCurrentMeshManager(int id)
		{
			meshId_ = id;
			if (meshId_ >= meshManList_.Length)
			{
				MeshManager[] destinationArray = new MeshManager[meshId_ + 1];
				Array.Copy(meshManList_, destinationArray, meshManList_.Length);
				meshManList_ = destinationArray;
			}
			if (meshManList_[id] == null)
			{
				meshManList_[id] = new MeshManager(meshManList_[0]);
				meshManList_[id].BeginMesh();
			}
		}

		public void BeginState()
		{
			if (stackStateCount_ >= stackState_.Count)
			{
				stackState_.Add(new StackState());
			}
			StackState stackState = stackState_[stackStateCount_];
			stackState.Clip = clipCount_;
			stackState.Font = fontCount_;
			stackState.RenderMode = renderModeCount_;
			stackState.Matrix = localMatrixIndex_;
			stackState.BlendMode = blendModeCount_;
			stackState.ClipEnable = clipEnableCount_;
			stackState.Origin = originCount_;
			stackState.RenderQueue = renderQueueCount_;
			stackState.ImageClip = imageClipCount_;
			stackStateCount_++;
		}

		public void EndState()
		{
			if (this.stackStateCount_ <= 0)
			{
				return;
			}
			this.stackStateCount_--;
			kairo.unity.ui.Graphics.StackState item = this.stackState_[this.stackStateCount_];
			while (item.Clip < this.clipCount_)
			{
				this.PopClip();
			}
			while (item.Font < this.fontCount_)
			{
				this.PopFont();
			}
			while (item.RenderMode < this.renderModeCount_)
			{
				this.PopRenderMode();
			}
			while (item.Matrix < this.localMatrixIndex_)
			{
				this.PopMatrix();
			}
			while (item.BlendMode < this.blendModeCount_)
			{
				this.PopBlendMode();
			}
			while (item.ClipEnable < this.clipEnableCount_)
			{
				this.PopClipEnable();
			}
			while (item.Origin < this.originCount_)
			{
				this.PopOrigin();
			}
			while (item.RenderQueue < this.renderQueueCount_)
			{
				this.PopRenderQueue();
			}
			while (item.ImageClip < this.imageClipCount_)
			{
				this.PopImageClip();
			}
		}

		public static void SetLineSpace(int lineSpace)
		{
			lineSpacing_ = lineSpace;
		}

		public static int GetLineSpace()
		{
			return lineSpacing_;
		}

		public static void SetImageExpand(float expand = 0.01f)
		{
			imageExpand_ = expand;
		}

		public void BeginMeshRecord()
		{
			MeshManager meshManager = GetMeshManager();
			meshManager.meshRecordList_.Add(meshManager.meshRecord_ = new MeshRecord());
		}

		public MeshRecord EndMeshRecord()
		{
			MeshManager meshManager = GetMeshManager();
			MeshRecord meshRecord_ = meshManager.meshRecord_;
			meshManager.meshRecord_ = null;
			return meshRecord_;
		}

		public void MaskBalloon(int x, int y, int width, int height)
		{
			if (this.render_ != null)
			{
				foreach (Image.BalloonRenderTask balloonRenderTask in this.render_.balloonRenderTasks_)
				{
					balloonRenderTask.Balloon.AddMask(x, y, width, height);
				}
			}
		}
	}
}
