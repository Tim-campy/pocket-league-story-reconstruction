using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using java.io;
using java.lang;
using java.util;
using kairo.common.cfg;
using kairo.unity.graphics;
using kairo.unity.io;
using kairo.unity.math;
using kairo.unity.surface;
using kairo.unity.util;
using UnityEngine;

namespace kairo.unity.ui
{
	public class Image
	{
		public enum Format
		{
			Default = -1,
			ARGB32 = 5,
			ARGB4444 = 2,
			RGB24 = 3,
			RGB565 = 7
		}

		public enum Filter
		{
			Default = -1,
			Point,
			Bilinear,
			Trilinear
		}

		internal struct TextRenderTask
		{
			public string Text;

			public float X;

			public float Y;

			public int Anchor;

			public int Color;

			public Font Font;

			public TextRenderTask(string text, float x, float y, int anchor, int color, Font font)
			{
				Text = text;
				X = x;
				Y = y;
				Anchor = anchor;
				Color = color;
				Font = font;
			}
		}

		internal struct BalloonRenderTask
		{
			public Balloon Balloon;

			public float X;

			public float Y;

			public Offscreen Offscreen;

			public BalloonRenderTask(Balloon balloon, float x, float y, Offscreen offscreen)
			{
				Balloon = new Balloon(balloon);
				X = x;
				Y = y;
				Offscreen = offscreen;
			}
		}

		internal struct TouchTask
		{
			public SurfaceBase Surface;

			public TouchComponent Component;

			public TouchTask(SurfaceBase surface, TouchComponent component)
			{
				Surface = surface;
				Component = component;
				float num = Surface.GetScaleRatio() / 100f;
				Component.x = (int)((float)Component.x * num + 0.01f);
				Component.y = (int)((float)Component.y * num + 0.01f);
				Component.width = (int)((float)Component.width * num + 0.01f);
				Component.height = (int)((float)Component.height * num + 0.01f);
			}
		}

		private static ImageAtlasManager atlasMan_;

		internal Texture image_;

		protected string name_ = "";

		protected int width_;

		protected int height_;

		internal Format format_;

		internal Filter filter_;

		internal Format requestFormat_ = Format.Default;

		internal Filter requestFilter_;

		internal byte[] src_;

		internal string file_;

		internal string paletteFile_;

		internal int rsId_ = -1;

		internal int rcId_ = -1;

		internal bool useBin_;

		internal bool lockExpiration_;

		protected static FastVector sourceImageList_ = new FastVector();

		protected static int sourceImagePixels_;

		protected static int sourceImageMaxCount_;

		protected static int sourceImageMaxPixels_;

		protected static int sourceImageValidateFrames_;

		protected int localSourceImageValidateFrames_ = -1;

		internal int standardWidth_;

		internal int standardHeight_;

		internal const int OPT_REF = 0;

		internal const int OPT_X = 1;

		internal const int OPT_Y = 2;

		internal const int OPT_SRCX = 3;

		internal const int OPT_SRCY = 4;

		internal const int OPT_SRCW = 5;

		internal const int OPT_SRCH = 6;

		internal const int OPT_END = 7;

		internal sbyte[] optimize_;

		internal short[] optimizeEx_;

		internal sbyte[] optimizeAccess_;

		internal short[] optimizeAccessL_;

		internal int optimizeWidthNum_;

		internal int optimizeHeightNum_;

		internal int optimizePass_;

		internal const int OPTSEB_U = 0;

		internal const int OPTSEB_V = 1;

		internal const int OPTSEB_W = 2;

		internal const int OPTSEB_H = 3;

		internal const int OPTSEB_END = 4;

		internal sbyte[] optimizeSeb_;

		internal short[] optimizeSebEx_;

		internal Image[] images_;

		internal short optimizeBlockWidth_;

		internal short optimizeBlockHeight_;

		internal int checkCount_;

		private static bool copeOutOfMemoryEnable_ = true;

		protected int transparentColor_;

		protected bool isDisposed_;

		protected bool isCreateImage_;

		protected int alpha_ = 255;

		public const int ALPHATYPE_OPAQUE = 0;

		public const int ALPHATYPE_ALPHATEST = 1;

		public const int ALPHATYPE_TRANSPARENT = 2;

		public const int ALPHATYPE_OPATRANS = 3;

		protected int alphaType_ = -1;

		internal int renderQueue_;

		internal int imageAtlasId_ = -1;

		internal ImageAtlas.AtlasRegion atlasRegion_;

		internal List<TextRenderTask> textRenderTasks_ = new List<TextRenderTask>();

		internal List<BalloonRenderTask> balloonRenderTasks_ = new List<BalloonRenderTask>();

		internal List<TouchTask> touchTasks_ = new List<TouchTask>();

		public static Format DEFAULT_FORMAT = Format.ARGB32;

		public static Filter DEFAULT_FILTER = Filter.Point;

		public bool IsDisposed => isDisposed_;

		public Image()
		{
			if (atlasMan_ == null)
			{
				atlasMan_ = ImageAtlasManager.GetInstance();
			}
			width_ = 0;
			height_ = 0;
		}

		public Image(Texture image)
		{
			if (atlasMan_ == null)
			{
				atlasMan_ = ImageAtlasManager.GetInstance();
			}
			image_ = image;
			width_ = image.width;
			height_ = image.height;
			if (Config.COUNT_ENABLE)
			{
				Config.createImageArea_ += GetWidth() * GetHeight();
			}
		}

		public Image(int[] rgb, int width, int height, Format format = Format.Default, Filter filterMode = Filter.Default)
		{
			if (atlasMan_ == null)
			{
				atlasMan_ = ImageAtlasManager.GetInstance();
			}
			requestFormat_ = format;
			requestFilter_ = filterMode;
			CreateImage(width, height, requestFormat_, requestFilter_).SetRGBPixels(0, 0, width, height, rgb);
			width_ = width;
			height_ = height;
		}

		public Image(byte[] src, Format format = Format.Default, Filter filterMode = Filter.Default)
		{
			if (atlasMan_ == null)
			{
				atlasMan_ = ImageAtlasManager.GetInstance();
			}
			requestFormat_ = format;
			requestFilter_ = filterMode;
			int[] areaSize = GetAreaSize(src);
			src_ = src;
			width_ = areaSize[0];
			height_ = areaSize[1];
		}

		public Image(string file, Format format = Format.Default, Filter filterMode = Filter.Default)
		{
			if (atlasMan_ == null)
			{
				atlasMan_ = ImageAtlasManager.GetInstance();
			}
			requestFormat_ = format;
			requestFilter_ = filterMode;
			file_ = file;
			width_ = 240;
			height_ = 240;
		}

		public Image(string file, string paletteFile, Format format = Format.Default, Filter filterMode = Filter.Default)
		{
			if (atlasMan_ == null)
			{
				atlasMan_ = ImageAtlasManager.GetInstance();
			}
			requestFormat_ = format;
			requestFilter_ = filterMode;
			file_ = file;
			width_ = 240;
			height_ = 240;
			paletteFile_ = paletteFile;
		}

		public Image(int rsId, int rcId, string file, string paletteFile, Format format = Format.Default, Filter filterMode = Filter.Default)
		{
			if (atlasMan_ == null)
			{
				atlasMan_ = ImageAtlasManager.GetInstance();
			}
			requestFormat_ = format;
			requestFilter_ = filterMode;
			rsId_ = rsId;
			rcId_ = rcId;
			file_ = file;
			width_ = 240;
			height_ = 240;
			paletteFile_ = paletteFile;
		}

		public static Image CreateImage(int width, int height, Format format = Format.RGB24, Filter filterMode = Filter.Default)
		{
			if (format == Format.Default)
			{
				format = Format.RGB24;
			}
			if (filterMode == Filter.Default)
			{
				filterMode = DEFAULT_FILTER;
			}
			return (Image)IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				Texture2D texture2D = new Texture2D(width, height, (TextureFormat)format, false);
				texture2D.filterMode = (FilterMode)filterMode;
				Image image = new Image(texture2D);
				image.isCreateImage_ = true;
				image.lockExpiration_ = true;
				image.checkCount_ = Config.checkCount_;
				sourceImageList_.AddElement(image);
				sourceImagePixels_ += image.GetWidth() * image.GetHeight();
				return image;
			}, true);
		}

		public Texture GetImage()
		{
			return image_;
		}

		public static Image Load(InputStream stream, Format format = Format.Default, Filter filterMode = Filter.Default)
		{
			return Load(StreamUtil.Read(stream), format, filterMode);
		}

		public static Image Load(byte[] data, Format format = Format.Default, Filter filterMode = Filter.Default)
		{
			return (Image)IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				int num = 0;
				while (true)
				{
					try
					{
						Image image = new Image(DecodeByteArray(data, format, filterMode));
						int alphaType = GetAlphaType(data);
						if (alphaType != -1)
						{
							image.SetAlpha(alphaType);
						}
						return image;
					}
					catch (OutOfMemoryException error)
					{
						CopeOutOfMemory(num, error);
					}
					num++;
				}
			}, true);
		}

		private static int[] getOptimize_result = new int[7];

		private static int[] getOptimizeSeb_result = new int[4];

		public static Image Load(string path, Format format = Format.Default, Filter filterMode = Filter.Default)
		{
			return Load(Storage.Read(3, false, path), format, filterMode);
		}

		public void LoadOptimize(InputStream stream, Image[] images)
		{
			images_ = images;
			optimizeSeb_ = null;
			int num = 0;
			Vector vector = new Vector();
			Vector vector2 = new Vector();
			optimizeBlockWidth_ = StreamUtil.ReadByte(stream);
			if (optimizeBlockWidth_ == -1)
			{
				optimizeBlockHeight_ = -1;
				optimizePass_ = 1;
				int num2 = StreamUtil.ReadByte(stream);
				short[][] array = new short[num2][];
				optimizeSeb_ = new sbyte[num2 * 4];
				Vector vector3 = null;
				for (int i = 0; i < array.Length; i++)
				{
					num2 = StreamUtil.ReadByte(stream);
					bool flag = false;
					short[] array2 = new short[4];
					for (int j = 0; j < array2.Length; j++)
					{
						array2[j] = (short)(StreamUtil.ReadByte(stream) & 0xFF);
						if (array2[j] > 255)
						{
							flag = true;
						}
					}
					if (flag || array2[0] == 255)
					{
						if (vector3 == null)
						{
							vector3 = new Vector();
						}
						vector3.AddElement(array2);
						optimizeSeb_[i * 4 + 0] = -1;
						optimizeSeb_[i * 4 + 1] = (sbyte)(vector3.Size() - 1);
					}
					else
					{
						for (int k = 0; k < array2.Length; k++)
						{
							optimizeSeb_[i * 4 + k] = (sbyte)array2[k];
						}
					}
					array[i] = new short[num2 * 7];
					for (int l = 0; l < array[i].Length; l++)
					{
						array[i][l] = StreamUtil.ReadShort(stream);
					}
					if (optimizePass_ < num2)
					{
						optimizePass_ = num2;
					}
				}
				if (vector3 != null)
				{
					optimizeSebEx_ = new short[vector3.Size() * 4];
					for (int m = 0; m < vector3.Size(); m++)
					{
						JSystem.Arraycopy((short[])vector3.ElementAt(m), 0, optimizeSebEx_, m * 4, 4);
					}
				}
				optimizeAccessL_ = new short[array.Length * optimizePass_];
				for (int n = 0; n < array.Length; n++)
				{
					short[] array3 = array[n];
					int num3;
for ( num3 = 0; num3 < optimizePass_; num3++)
					{
						optimizeAccessL_[n * optimizePass_ + num3] = -1;
					}
					int num4 = 0;
					int num5 = 0;
					while (num4 < array3.Length)
					{
						byte[] array4 = new byte[7];
						for (int num6 = 0; num6 < 7; num6++)
						{
							array4[num6] = (byte)array3[num4 + num6];
						}
						short num7 = -1;
						if (array4[0] != array3[num4 + 0] || (array4[1] & 0xFF) != array3[num4 + 1] || (array4[2] & 0xFF) != array3[num4 + 2] || (array4[3] & 0xFF) != array3[num4 + 3] || (array4[4] & 0xFF) != array3[num4 + 4] || (array4[5] & 0xFF) != array3[num4 + 5] || (array4[6] & 0xFF) != array3[num4 + 6])
						{
							vector2.AddElement(array3);
							num7 = (short)(vector2.Size() - 1);
							if (num < num7)
							{
								num = num7;
							}
							ushort num8 = 32768;
							num7 |= (short)num8;
						}
						else
						{
							vector.AddElement(array4);
							num7 = (short)(vector.Size() - 1);
							if (num < num7)
							{
								num = num7;
							}
						}
						optimizeAccessL_[n * optimizePass_ + num5] = num7;
						num4 += 7;
						num5++;
					}
					array[n] = null;
				}
			}
			else
			{
				optimizeBlockHeight_ = StreamUtil.ReadByte(stream);
				optimizeWidthNum_ = StreamUtil.ReadByte(stream);
				optimizeHeightNum_ = StreamUtil.ReadByte(stream);
				optimizePass_ = 1;
				optimizeBlockWidth_ &= 255;
				optimizeBlockHeight_ &= 255;
				short[][][] array5 = JTool.MakeArray<short[][][]>(new int[2] { optimizeHeightNum_, optimizeWidthNum_ });
				for (int num9 = 0; num9 < array5.Length; num9++)
				{
					for (int num10 = 0; num10 < array5[num9].Length; num10++)
					{
						int num11 = StreamUtil.ReadByte(stream);
						array5[num9][num10] = new short[num11 * 7];
						short[] array6 = array5[num9][num10];
						for (int num12 = 0; num12 < array6.Length; num12++)
						{
							array6[num12] = StreamUtil.ReadShort(stream);
						}
						if (optimizePass_ < num11)
						{
							optimizePass_ = num11;
						}
					}
				}
				optimizeAccessL_ = new short[optimizeWidthNum_ * optimizeHeightNum_ * optimizePass_];
				for (int num13 = 0; num13 < array5.Length; num13++)
				{
					for (int num14 = 0; num14 < array5[num13].Length; num14++)
					{
						short[] array7 = array5[num13][num14];
						int num15;
for ( num15 = 0; num15 < optimizePass_; num15++)
						{
							optimizeAccessL_[(num13 * optimizeWidthNum_ + num14) * optimizePass_ + num15] = -1;
						}
						int num16 = 0;
						int num17 = 0;
						while (num16 < array7.Length)
						{
							sbyte[] array8 = new sbyte[7];
							for (int num18 = 0; num18 < 7; num18++)
							{
								array8[num18] = (sbyte)array7[num16 + num18];
							}
							short num19 = -1;
							if (array8[0] != array7[num16 + 0] || (array8[1] & 0xFF) != array7[num16 + 1] || (array8[2] & 0xFF) != array7[num16 + 2] || (array8[3] & 0xFF) != array7[num16 + 3] || (array8[4] & 0xFF) != array7[num16 + 4] || (array8[5] & 0xFF) != array7[num16 + 5] || (array8[6] & 0xFF) != array7[num16 + 6])
							{
								vector2.AddElement(array7);
								num19 = (short)(vector2.Size() - 1);
								if (num < num19)
								{
									num = num19;
								}
								ushort num20 = 32768;
								num19 |= (short)num20;
							}
							else
							{
								vector.AddElement(array8);
								num19 = (short)(vector.Size() - 1);
								if (num < num19)
								{
									num = num19;
								}
							}
							optimizeAccessL_[(num13 * optimizeWidthNum_ + num14) * optimizePass_ + num17] = num19;
							num16 += 7;
							num17++;
						}
						array5[num13][num14] = null;
					}
				}
			}
			if (num < 127)
			{
				optimizeAccess_ = new sbyte[optimizeAccessL_.Length];
				for (int num21 = 0; num21 < optimizeAccess_.Length; num21++)
				{
					if (optimizeAccessL_[num21] == -1)
					{
						optimizeAccess_[num21] = -1;
						continue;
					}
					optimizeAccess_[num21] = (sbyte)(optimizeAccessL_[num21] & 0x7F);
					if ((optimizeAccessL_[num21] & 0x8000) != 0)
					{
						byte b = 128;
						optimizeAccess_[num21] |= (sbyte)b;
					}
				}
				optimizeAccessL_ = null;
			}
			if (vector.Size() > 0)
			{
				optimize_ = new sbyte[vector.Size() * 7];
				for (int num22 = 0; num22 < vector.Size(); num22++)
				{
					JSystem.Arraycopy((sbyte[])vector.ElementAt(num22), 0, optimize_, num22 * 7, 7);
				}
			}
			if (vector2.Size() > 0)
			{
				optimizeEx_ = new short[vector2.Size() * 7];
				for (int num23 = 0; num23 < vector2.Size(); num23++)
				{
					JSystem.Arraycopy((short[])vector2.ElementAt(num23), 0, optimizeEx_, num23 * 7, 7);
				}
			}
		}

		public void LoadOptimize(byte[] data, Image[] images)
		{
			ByteArrayInputStream byteArrayInputStream = null;
			try
			{
				byteArrayInputStream = new ByteArrayInputStream(data);
				LoadOptimize(byteArrayInputStream, images);
			}
			finally
			{
				byteArrayInputStream?.Close();
				byteArrayInputStream = null;
			}
		}

		public int[] GetOptimize(int bx, int by, int passIndex)
		{
			if (this.optimizeAccess_ != null)
			{
				sbyte num = this.optimizeAccess_[(by * this.optimizeWidthNum_ + bx) * this.optimizePass_ + passIndex];
				if (num != -1)
				{
					return this.GetOptimize(((num & 128) == 0 ? 0 : 1), num & 127);
				}
			}
			else if (this.optimizeAccessL_ != null)
			{
				short num1 = this.optimizeAccessL_[(by * this.optimizeWidthNum_ + bx) * this.optimizePass_ + passIndex];
				if (num1 != -1)
				{
					return this.GetOptimize(((num1 & 32768) == 0 ? 0 : 1), num1 & 32767);
				}
			}
			return null;
		}

		public int[] GetOptimize(sbyte access)
		{
			return this.GetOptimize(((access & 128) == 0 ? 0 : 1), access & 127);
		}

		public int[] GetOptimize(short access)
		{
			return this.GetOptimize(((access & 32768) == 0 ? 0 : 1), access & 32767);
		}

		public int[] GetOptimize(int type, int index)
		{
			int num = index * 7;
			int[] array = getOptimize_result;
			if (type == 0)
			{
				array[0] = optimize_[num + 0];
				array[1] = optimize_[num + 1] & 0xFF;
				array[2] = optimize_[num + 2] & 0xFF;
				array[3] = optimize_[num + 3] & 0xFF;
				array[4] = optimize_[num + 4] & 0xFF;
				array[5] = optimize_[num + 5] & 0xFF;
				array[6] = optimize_[num + 6] & 0xFF;
			}
			else
			{
				array[0] = optimizeEx_[num + 0];
				array[1] = optimizeEx_[num + 1];
				array[2] = optimizeEx_[num + 2];
				array[3] = optimizeEx_[num + 3];
				array[4] = optimizeEx_[num + 4];
				array[5] = optimizeEx_[num + 5];
				array[6] = optimizeEx_[num + 6];
			}
			return array;
		}

		public int[] GetOptimizeSeb(int index)
		{
			int num = index * 4;
			int[] array = getOptimizeSeb_result;
			if (optimizeSeb_[num + 0] != -1)
			{
				array[0] = optimizeSeb_[num + 0] & 0xFF;
				array[1] = optimizeSeb_[num + 1] & 0xFF;
				array[2] = optimizeSeb_[num + 2] & 0xFF;
				array[3] = optimizeSeb_[num + 3] & 0xFF;
			}
			else
			{
				num = optimizeSeb_[num + 1] * 4;
				array[0] = optimizeSebEx_[num + 0];
				array[1] = optimizeSebEx_[num + 1];
				array[2] = optimizeSebEx_[num + 2];
				array[3] = optimizeSebEx_[num + 3];
			}
			return array;
		}

		public void Use()
		{
			if ((object)image_ != null)
			{
				return;
			}
			if (src_ == null && file_ == null)
			{
				return;
			}
			IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				byte[] array = src_; int num2; int num3;
				if (array == null && file_ != null)
				{
					if (rsId_ == -1)
					{
						array = Storage.Read(3, false, file_);
						if (paletteFile_ != null)
						{
							OverwritePaletteData(array, Storage.Read(3, false, paletteFile_));
						}
					}
					else
					{
						JarInflater jarInflater = ResourceManager.OpenJarCache(rsId_, rcId_);
						array = jarInflater.GetData(file_);
						if (paletteFile_ != null)
						{
							OverwritePaletteData(array, jarInflater.GetData(paletteFile_));
						}
					}
				}
				if (array == null)
				{
					if (Config.PRINT)
					{
						Log.Info("Image.use() 失敗 [" + name_ + "]" + ((file_ == null) ? "" : (" file=" + file_)));
					}

return;
}
{
					int num = 0;
					while (true)
					{
						try
						{
							image_ = DecodeByteArray(array);
						}
						catch (OutOfMemoryException error)
						{
							CopeOutOfMemory(num, error);
							goto IL_00e8;
						}
						break;
						IL_00e8:
						num++;
					}
					if (alphaType_ == -1)
					{
						int alphaType = GetAlphaType(array);
						if (alphaType != -1)
						{
							SetAlpha(alphaType);
						}
					}
					width_ = image_.width;
					height_ = image_.height;
					checkCount_ = Config.checkCount_;
					sourceImageList_.AddElement(this);
					sourceImagePixels_ += GetWidth() * GetHeight();
					if (Config.COUNT_ENABLE)
					{
						Config.createImageArea_ += GetWidth() * GetHeight();
					}
					num2 = sourceImageMaxCount_;
					num3 = sourceImageMaxPixels_;
					while (sourceImageList_.elementCount > 1 && ((num2 > 0 && num2 < sourceImageList_.elementCount) || (num3 > 0 && num3 < sourceImagePixels_)))
					{
						Image image = null;
						long num4 = long.MinValue;
						for (int i = 0; i < sourceImageList_.elementCount - 1; i++)
						{
							Image image2 = (Image)sourceImageList_.elementData[i];
							long num5 = (long)(Config.checkCount_ - image2.checkCount_) * 1000L - image2.GetWidth() * image2.GetHeight() / 100;
							if (num4 < num5)
							{
								num4 = num5;
								image = image2;
							}
						}
						if (Config.USE_UNUSE_CHECK && Config.checkCount_ - image.checkCount_ == 1)
						{
							Log.Info("[" + image.name_ + "] use-unuse");
						}
						if (image.isCreateImage_)
						{
							image.Dispose();
						}
						else
						{
							image.Unuse();
						}
					}
					if (imageAtlasId_ != -1 && atlasMan_.enable_)
					{
						atlasMan_.ImageToAtlas(imageAtlasId_, this);
					}
				}
			}, true);
		}

		public void Unuse()
		{
			if ((object)image_ == null)
			{
				return;
			}
			if (!isCreateImage_ && src_ == null && file_ == null)
			{
				return;
			}
			IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				Release();
				sourceImageList_.RemoveElement(this);
				sourceImagePixels_ -= GetWidth() * GetHeight();
				if (Config.COUNT_ENABLE)
				{
					Config.createImageArea_ -= GetWidth() * GetHeight();
				}
				if (imageAtlasId_ != -1 && atlasMan_.enable_)
				{
					atlasMan_.RemoveAtlas(this);
				}
			}, true);
		}

		public Graphics GetGraphics()
		{
			return (Graphics)IApplication.GetCurrentApp().RunOnUiThread(() => ((object)image_ == null) ? (null) : (new Graphics(this)), true);
		}

		public int GetHeight()
		{
			return height_;
		}

		public int GetWidth()
		{
			return width_;
		}

		public int GetAlpha()
		{
			return alpha_;
		}

		public int GetTransparentColor()
		{
			return transparentColor_;
		}

		public void SetAlpha(int alpha)
		{
			Texture2D texture2D = GetTexture2D();
			if (texture2D == null) {
				return;
			}
if (alpha_ != alpha) {
				return;
			}
			alpha_ = alpha;
			float a = (float)alpha / 255f;
			Color[] pixels = texture2D.GetPixels();
			for (int i = 0; i < pixels.Length; i++)
			{
				pixels[i].a = a;
			}
			texture2D.SetPixels(pixels);
			texture2D.Apply();
			if (alpha < 255)
			{
				if (alphaType_ == 0 || alphaType_ == 1)
				{
					alphaType_ = 2;
				}
			}
			else
			{
				alphaType_ = -1;
			}
		}

		public void SetTransparentColor(int color)
		{
			transparentColor_ = color;
		}

		public void SetTransparentEnabled(bool enabled)
		{
			int num = transparentColor_ & 0xFFFFFF;
			int[] aRGBPixels = GetARGBPixels(0, 0, GetWidth(), GetHeight());
			if (enabled)
			{
				bool flag = false;
				if (!IsAlphaFormat((Format)GetTexture2D().format))
				{
					image_ = new Texture2D(image_.width, image_.height, TextureFormat.ARGB32, false);
					flag = true;
				}
				for (int i = 0; i < aRGBPixels.Length; i++)
				{
					if ((aRGBPixels[i] & 0xFFFFFF) == num)
					{
						aRGBPixels[i] = aRGBPixels[i] & 16777215;
					}
					else if (flag)
					{
						aRGBPixels[i] = -16777216 | aRGBPixels[i];
					}
				}
				SetARGBPixels(0, 0, GetWidth(), GetHeight(), aRGBPixels);
				if (alphaType_ == 0)
				{
					alphaType_ = 1;
				}
			}
			else
			{
				if (IsAlphaFormat((Format)GetTexture2D().format))
				{
					image_ = new Texture2D(image_.width, image_.height, TextureFormat.RGB24, false);
				}
				SetRGBPixels(0, 0, GetWidth(), GetHeight(), aRGBPixels);
				alphaType_ = 0;
			}
		}

		public void Dispose()
		{
			Unuse();
			if (Config.COUNT_ENABLE && (object)image_ != null)
			{
				Config.createImageArea_ -= GetWidth() * GetHeight();
			}
			Release();
			if (imageAtlasId_ != -1 && atlasMan_.enable_)
			{
				atlasMan_.RemoveAtlas(this);
			}
			images_ = null;
			optimize_ = null;
			image_ = null;
			file_ = null;
			src_ = null;
			paletteFile_ = null;
			foreach (TouchTask item in touchTasks_)
			{
				item.Component.hasCount--;
				if (item.Component.option != null)
				{
					item.Component.option.hasCount--;
				}
			}
			touchTasks_.Clear();
			width_ = (height_ = 0);
			isDisposed_ = true;
		}

		internal void Release()
		{
			if ((object)image_ == null)
			{
				return;
			}
			IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				if (image_ is RenderTexture)
				{
					((RenderTexture)image_).Release();
					UnityEngine.Object.Destroy(image_);
				}
				else
				{
					UnityEngine.Object.Destroy(image_);
				}
				image_ = null;
			}, true);
		}

		private static int[] getPaletteInfo_result = new int[2];

		private static int[] getAreaSize_result = new int[2];

		public static void SetSourceImageMaxCount(int max)
		{
			sourceImageMaxCount_ = max;
		}

		public static void SetSourceImageMaxPixels(int max)
		{
			sourceImageMaxPixels_ = max;
		}

		public static void SetSourceImageValidateFrames(int frames)
		{
			sourceImageValidateFrames_ = frames;
		}

		public void SetName(string name)
		{
			name_ = name;
		}

		public string GetName()
		{
			return name_;
		}

		public override string ToString()
		{
			return "[" + name_ + "] " + base.ToString();
		}

		public static void DisposeBrightnessImages()
		{
		}

		private static void CopeOutOfMemory(int tryCount, OutOfMemoryException error)
		{
		}

		public static void SetCopeOutOfMemoryEnable(bool enable)
		{
			copeOutOfMemoryEnable_ = enable;
		}

		public void EraseColor(int color)
		{
			Texture2D texture2D = this.GetTexture2D();
			if (texture2D == null)
			{
				return;
			}
			Color[] pixels = texture2D.GetPixels();
			Color color1 = kairo.unity.ui.Graphics.ArgbToColor(color);
			for (int i = 0; i < (int)pixels.Length; i++)
			{
				pixels[i] = color1;
			}
			texture2D.SetPixels(pixels);
			texture2D.Apply();
		}

		public void EraseColor(int r, int g, int b, int a)
		{
			EraseColor(Graphics.GetColorOfRGB(r, g, b, a));
		}

		public static byte[] OverwritePaletteData(byte[] image, byte[] palette)
		{
			if (palette == null || image == null)
			{
				return image;
			}
			return OverwritePaletteData(image, palette, 0, 0, palette.Length / 3);
		}

		public static byte[] OverwritePaletteData(byte[] image, byte[] palette, int imageOffset, int paletteOffset, int colors)
		{
			if (palette == null || image == null)
			{
				return image;
			}
			int[] paletteInfo = GetPaletteInfo(image);
			if (paletteInfo == null)
			{
				return image;
			}
			if (imageOffset + colors > palette.Length / 3)
			{
				colors = palette.Length / 3 - imageOffset;
			}
			if (paletteOffset + colors > paletteInfo[1])
			{
				colors = paletteInfo[1] - paletteOffset;
			}
			JSystem.Arraycopy(palette, paletteOffset * 3, image, paletteInfo[0] + imageOffset * 3, colors * 3);
			return image;
		}

		private static int[] GetPaletteInfo(byte[] data)
		{
			if (data == null)
			{
				return null;
			}
			string fileFormat = GetFileFormat(data);
			if (fileFormat == "gif")
			{
				byte b = data[10];
				if (((b >> 7) & 1) == 1)
				{
					getPaletteInfo_result[0] = 13;
					getPaletteInfo_result[1] = KairoMath.Pow(2, (b & 7) + 1);
					return getPaletteInfo_result;
				}
				int num = 13;
				while (num < data.Length)
				{
					int num2 = data[num];
					if (num2 == 44)
					{
						b = data[num + 9];
						if (((b >> 7) & 1) == 1)
						{
							getPaletteInfo_result[0] = num + 10;
							getPaletteInfo_result[1] = KairoMath.Pow(2, (b & 7) + 1);
							return getPaletteInfo_result;
						}
						return null;
					}
					if (num2 != 33)
					{
						break;
					}
					num += 3 + data[num + 2];
					if (data[num] == 0)
					{
						num++;
					}
				}
			}
			else if (fileFormat == "png")
			{
				if (Config.DEBUG)
				{
					Log.Info("getPaletteInfo:PNGは対応していません");
				}
				return null;
			}
			return null;
		}

		public static int[] GetAreaSize(byte[] data)
		{
			int[] array = getAreaSize_result;
			array[0] = (array[1] = 0); string fileFormat = GetFileFormat(data);
			if (fileFormat == "gif") {array[0] = (data[6] & 0xFF) | ((data[7] & 0xFF) << 8);
				array[1] = (data[8] & 0xFF) | ((data[9] & 0xFF) << 8);
				} else if (fileFormat == "png") {array[0] = ((data[16] & 0xFF) << 24) | ((data[17] & 0xFF) << 16) | ((data[18] & 0xFF) << 8) | (data[19] & 0xFF);
				array[1] = ((data[20] & 0xFF) << 24) | ((data[21] & 0xFF) << 16) | ((data[22] & 0xFF) << 8) | (data[23] & 0xFF);
				} else if (fileFormat == "jpg") {{
				int num = 2;
				while (true)
				{
					int num2 = (data[num + 0] << 8) | data[num + 1];
					num += 2;
					int num3 = (data[num + 0] << 8) | data[num + 1];
					if (num2 == 65472) {array[1] = (data[num + 3] << 8) | data[num + 4]; array[0] = (data[num + 5] << 8) | data[num + 6]; break;}
					num += num3;
				}


			}}
			return array;
		}

		protected static Texture2D DecodeByteArray(byte[] data, Format format = Format.Default, Filter filter = Filter.Default)
		{
			if (format == Format.Default)
			{
				format = DEFAULT_FORMAT;
			}
			if (filter == Filter.Default)
			{
				filter = DEFAULT_FILTER;
			}
			Texture2D texture2D = new Texture2D(1, 1, (TextureFormat)format, false);
			texture2D.filterMode = (FilterMode)filter;
			texture2D.wrapMode = TextureWrapMode.Clamp;
			texture2D.LoadImage(data);
			return texture2D;
		}

		protected static int GetAlphaType(byte[] data)
		{
			string fileFormat = GetFileFormat(data);
			if (fileFormat == "gif")
			{
				return 1;
			}
			if (fileFormat == "jpg")
			{
				return 0;
			}
			if (fileFormat == "png")
			{
				int num = data[25];
				if (num == 0 || num == 2)
				{
					return 0;
				}
				if (num == 3)
				{
					return 1;
				}
			}
			return -1;
		}

		public int GetAlphaType()
		{
			if (alpha_ == 0)
			{
				return 1;
			}
			if (alpha_ < 255)
			{
				return 2;
			}
			if (alphaType_ != -1)
			{
				return alphaType_;
			}
			alphaType_ = 0;
			Texture2D texture2D = GetTexture2D();
			if (texture2D != null)
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				Color[] pixels = texture2D.GetPixels();
				for (int i = 0; i < pixels.Length; i++)
				{
					if (pixels[i].a < 0.01f)
					{
						num++;
					}
					else if (pixels[i].a <= 0.999f)
					{
						num2++;
					}
				}
				num3 = texture2D.width * texture2D.height - num2 - num;
				if (num2 > 0)
				{
					if (num3 == 0)
					{
						alphaType_ = 2;
					}
					else
					{
						alphaType_ = 3;
					}
				}
				else if (num > 0 || num2 > 0)
				{
					alphaType_ = 1;
				}
			}
			return alphaType_;
		}

		internal void SetAlphaType(int alphaType)
		{
			alphaType_ = alphaType;
		}

		public override bool Equals(object obj)
		{
			return this == obj;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static int GetPowerOf2Value(int value)
		{
			int result = value;
			int num = 1;
			while (num < value)
			{
				result = (num *= 2);
			}
			return result;
		}

		public Color GetPixel(int x, int y)
		{
			Texture2D texture2D = GetTexture2D();
			if (texture2D == null)
			{
				return Color.white;
			}
			return texture2D.GetPixel(x, texture2D.height - 1 - y);
		}

		public Color GetPixelBilinear(float x, float y)
		{
			Texture2D texture2D = GetTexture2D();
			if (texture2D == null)
			{
				return Color.white;
			}
			return texture2D.GetPixelBilinear(x / (float)texture2D.width, ((float)(texture2D.height - 1) - y) / (float)texture2D.height);
		}

		public int GetRGBPixel(int x, int y)
		{
			return Graphics.ColorToRgb(GetPixel(x, y));
		}

		public int GetRGBPixelBilinear(float x, float y)
		{
			return Graphics.ColorToRgb(GetPixelBilinear(x, y));
		}

		public int GetARGBPixel(int x, int y)
		{
			return Graphics.ColorToArgb(GetPixel(x, y));
		}

		public int GetARGBPixelBilinear(float x, float y)
		{
			return Graphics.ColorToArgb(GetPixelBilinear(x, y));
		}

		public void SetPixel(int x, int y, Color color)
		{
			Texture2D texture2D = this.GetTexture2D();
			if (texture2D == null)
			{
				return;
			}
			texture2D.SetPixel(x, texture2D.height - 1 - y, color);
			texture2D.Apply();
		}

		public void SetRGBPixel(int x, int y, int color)
		{
			SetPixel(x, y, Graphics.RgbToColor(color));
		}

		public void SetARGBPixel(int x, int y, int color)
		{
			SetPixel(x, y, Graphics.ArgbToColor(color));
		}

		public Color[] GetPixels(int x, int y, int width, int height, Color[] pixels = null, int off = 0)
		{
			Texture2D texture2D = GetTexture2D();
			if (texture2D == null)
			{
				return pixels;
			}
			if (pixels == null)
			{
				pixels = new Color[off + width * height];
			}
			Color[] pixels2 = texture2D.GetPixels(x, texture2D.height - 1 - (y + height - 1), width, height);
			int num = off;
			for (int num2 = height - 1; num2 >= 0; num2--)
			{
				for (int i = 0; i < width; i++)
				{
					pixels[num++] = pixels2[num2 * width + i];
				}
			}
			return pixels;
		}

		internal Color[] GetNativePixels(int x, int y, int width, int height)
		{
			Texture2D texture2D = GetTexture2D();
			if (texture2D != null)
			{
				return texture2D.GetPixels(x, texture2D.height - 1 - (y + height - 1), width, height);
			}
			return null;
		}

		internal void SetNativePixels(int x, int y, int width, int height, Color[] pixels)
		{
			Texture2D texture2D = GetTexture2D();
			if (texture2D != null)
			{
				texture2D.SetPixels(x, texture2D.height - 1 - (y + height - 1), width, height, pixels);
			}
		}

		internal void Apply()
		{
			Texture2D texture2D = GetTexture2D();
			if (texture2D != null)
			{
				texture2D.Apply();
			}
		}

		public int[] GetARGBPixels(int x, int y, int width, int height, int[] pixels = null, int off = 0)
		{
			Texture2D texture2D = GetTexture2D();
			if (texture2D == null)
			{
				return pixels;
			}
			if (pixels == null)
			{
				pixels = new int[off + width * height];
			}
			Color[] pixels2 = texture2D.GetPixels(x, texture2D.height - 1 - (y + height - 1), width, height);
			int num = off;
			for (int num2 = height - 1; num2 >= 0; num2--)
			{
				for (int i = 0; i < width; i++)
				{
					pixels[num++] = Graphics.ColorToArgb(pixels2[num2 * width + i]);
				}
			}
			return pixels;
		}

		public int[] GetRGBPixels(int x, int y, int width, int height, int[] pixels = null, int off = 0)
		{
			Texture2D texture2D = GetTexture2D();
			if (texture2D == null)
			{
				return pixels;
			}
			if (pixels == null)
			{
				pixels = new int[off + width * height];
			}
			Color[] pixels2 = texture2D.GetPixels(x, texture2D.height - 1 - (y + height - 1), width, height);
			int num = off;
			for (int num2 = height - 1; num2 >= 0; num2--)
			{
				for (int i = 0; i < width; i++)
				{
					pixels[num++] = Graphics.ColorToRgb(pixels2[num2 * width + i]);
				}
			}
			return pixels;
		}

		public void SetPixels(int x, int y, int width, int height, Color[] pixels, int off = 0)
		{
			Texture2D texture2D = GetTexture2D();
			if (texture2D == null)
			{
				return;
			}
			Color[] array = new Color[width * height];
			int num = off;
			for (int num2 = height - 1; num2 >= 0; num2--)
			{
				for (int i = 0; i < width; i++)
				{
					array[num2 * width + i] = pixels[num++];
				}
			}
			texture2D.SetPixels(x, texture2D.height - 1 - (y + height - 1), width, height, array);
			texture2D.Apply();
		}

		public void SetARGBPixels(int x, int y, int width, int height, int[] pixels, int off = 0)
		{
			Texture2D texture2D = GetTexture2D();
			if (texture2D == null)
			{
				return;
			}
			Color[] array = new Color[width * height];
			int num = off;
			for (int num2 = height - 1; num2 >= 0; num2--)
			{
				for (int i = 0; i < width; i++)
				{
					array[num2 * width + i] = Graphics.ArgbToColor(pixels[num++]);
				}
			}
			texture2D.SetPixels(x, texture2D.height - 1 - (y + height - 1), width, height, array);
			texture2D.Apply();
		}

		public void SetRGBPixels(int x, int y, int width, int height, int[] pixels, int off = 0)
		{
			Texture2D texture2D = GetTexture2D();
			if (texture2D == null)
			{
				return;
			}
			Color[] array = new Color[width * height];
			int num = off;
			for (int num2 = height - 1; num2 >= 0; num2--)
			{
				for (int i = 0; i < width; i++)
				{
					array[num2 * width + i] = Graphics.RgbToColor(pixels[num++]);
				}
			}
			texture2D.SetPixels(x, texture2D.height - 1 - (y + height - 1), width, height, array);
			texture2D.Apply();
		}

		internal Texture2D GetTexture2D()
		{
			Texture image = GetImage();
			if (image == null)
			{
				return null;
			}
			if (image is Texture2D)
			{
				return (Texture2D)image;
			}
			throw new Exception("Texture2D 以外を引数に呼び出されました");
		}

		internal static RenderTextureFormat GetRenderTextureFormat(Format format)
		{
			if (format == Format.ARGB4444)
			{
				return RenderTextureFormat.ARGB4444;
			}
			if (format == Format.RGB24)
			{
				return RenderTextureFormat.ARGB32;
			}
			if (format == Format.RGB565)
			{
				return RenderTextureFormat.RGB565;
			}
			return RenderTextureFormat.ARGB32;
		}

		internal static bool IsAlphaFormat(Format format)
		{
			if (format == Format.RGB24 || format == Format.RGB565)
			{
				return false;
			}
			return true;
		}

		private static string GetFileFormat(byte[] data)
		{
			if (data == null)
			{
				return null;
			}
			if (data.Length >= 3 && data[0] == 71 && data[1] == 73 && data[2] == 70)
			{
				return "gif";
			}
			if (data.Length >= 8 && (data[0] & 0xFF) == 137 && data[1] == 80 && data[2] == 78 && data[3] == 71 && data[4] == 13 && data[5] == 10 && data[6] == 26 && data[7] == 10)
			{
				return "png";
			}
			if (data.Length >= 2 && (data[0] & 0xFF) == 255 && data[1] == 216)
			{
				return "jpg";
			}
			return null;
		}

		public void Resize(int width, int height)
		{
			Resize(width, height, 0, 0, GetWidth(), GetHeight());
		}

		public void Resize(int width, int height, int srcX, int srcY, int srcWidth, int srcHeight)
		{
			if (atlasRegion_ != null)
			{
				throw new Exception("アトラス化されています");
			}
			IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				Texture2D texture2D = GetTexture2D();
				if ((texture2D == null)) { return; }
Color[] array = new Color[height * width];
for (int i = 0; i < height; i++)
					{
						for (int j = 0; j < width; j++)
						{
							float u = (float)(srcX + j * srcWidth / width) / (float)texture2D.width;
							float v = (float)(texture2D.height - (srcY + (height - i) * srcHeight / height)) / (float)texture2D.height;
							array[i * width + j] = texture2D.GetPixelBilinear(u, v);
						}
					}
sourceImagePixels_ = sourceImagePixels_ - texture2D.width * texture2D.height + width * height;
if (Config.COUNT_ENABLE)
					{
						Config.createImageArea_ = Config.createImageArea_ - texture2D.width * texture2D.height + width * height;
					}
Texture2D texture2D2 = new Texture2D(width, height, texture2D.format, false);
texture2D2.filterMode = texture2D.filterMode;
texture2D2.SetPixels(array);
texture2D2.Apply();
image_ = texture2D2;
width_ = width;
height_ = height;
			}, true);
		}

		public void SetFilterMode(Filter filterMode)
		{
			IApplication.GetCurrentApp().RunOnUiThread(() => {
				Texture2D texture2D = this.GetTexture2D();
				if (texture2D == null)
				{
					return;
				}
				texture2D.filterMode = (FilterMode)filterMode;
			}, true);
		}

		public void SetWrapMode(bool repeat)
		{
			IApplication.GetCurrentApp().RunOnUiThread(() => {
				Texture2D texture2D = this.GetTexture2D();
				if (texture2D == null)
				{
					return;
				}
				texture2D.wrapMode = (repeat ? TextureWrapMode.Repeat : TextureWrapMode.Clamp);
			}, true);
		}

		public static void UnuseExpirationImages()
		{
			for (int num = sourceImageList_.Size() - 1; num >= 0; num--)
			{
				Image image = (Image)sourceImageList_.elementData[num];
				int num2 = image.localSourceImageValidateFrames_;
				if (num2 == -1)
				{
					num2 = sourceImageValidateFrames_;
				}
				if (num2 > 0 && !image.lockExpiration_ && Config.checkCount_ - image.checkCount_ > num2)
				{
					if (image.isCreateImage_)
					{
						image.Dispose();
					}
					else
					{
						image.Unuse();
					}
				}
			}
		}

		public void Clear(int color)
		{
			Color color2 = Graphics.ArgbToColor(color);
			Texture2D texture2D = GetTexture2D();
			if (texture2D == null)
			{
				return;
			}
			int width = GetWidth();
			int height = GetHeight();
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					texture2D.SetPixel(j, i, color2);
				}
			}
			texture2D.Apply();
		}

		public void Clear(int x, int y, int width, int height, int color)
		{
			Color color2 = Graphics.ArgbToColor(color);
			Texture2D texture2D = GetTexture2D();
			if (texture2D == null)
			{
				return;
			}
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					texture2D.SetPixel(x + j, texture2D.height - 1 - (y + i), color2);
				}
			}
			texture2D.Apply();
		}

		public void SetImageAtlasId(int id)
		{
			if (!(atlasMan_.enable_)) { return; }
atlasMan_.AtlasToImage(this);
imageAtlasId_ = id;
if (id != -1 && (object)image_ != null)
				{
					atlasMan_.ImageToAtlas(id, this);
				}
		}

		public void SetExpiration(bool yes, int validateFrames = -1)
		{
			lockExpiration_ = !yes;
			localSourceImageValidateFrames_ = validateFrames;
		}

		public bool IsExpiration()
		{
			return !lockExpiration_;
		}

		public void SetStandardResolution(int width, int height)
		{
			standardWidth_ = width;
			standardHeight_ = height;
		}
	}
}
