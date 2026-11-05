using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using kairo.common.cfg;
using kairo.unity.graphics;
using kairo.unity.util;
using UnityEngine;

namespace kairo.unity.ui
{
	public class ImageAtlas
	{
		private class HeightPoint
		{
			public int X;

			public int Y;

			public HeightPoint(int x, int y)
			{
				Set(x, y);
			}

			public void Set(int x, int y)
			{
				X = x;
				Y = y;
			}

			public void Set(int y)
			{
				Y = y;
			}
		}

		internal class AtlasRegion : IDisposable
		{
			public ImageAtlas Atlas;

			public Image Image;

			public int X;

			public int Y;

			public int Width;

			public int Height;

			public Color[] Pixels;

			public bool Valid;

			internal AtlasRegion(ImageAtlas atlas)
			{
				Atlas = atlas;
			}

			internal bool IsValid()
			{
				return Valid;
			}

			public void Dispose()
			{
				if (Image != null)
				{
					Image.atlasRegion_ = null;
				}
				Atlas = null;
				Image = null;
				Pixels = null;
				Valid = false;
			}

			public override int GetHashCode()
			{
				return base.GetHashCode();
			}
		}

		private Dictionary<Image, AtlasRegion> regions_;

		private List<AtlasRegion> spaces_;

		private Image imgAtlas_;

		private HeightPoint[] heightMap_;

		private bool change_;

		private const int BSP_WIDTH = 32;

		private const int BSP_HEIGHT = 32;

		private List<AtlasRegion>[][] bspMap_;

		private List<AtlasRegion> bspList_;

		private const int MARGIN = 1;

		private Dictionary<string, AtlasRegion> hintRegions_;

		private HeightPoint[] hintHeightMap_;

		private Rectangle GetBspRect_Result = new Rectangle();

		private ImageAtlas()
		{
		}

		internal static ImageAtlas CreateImageAtlas(int width, int height, Image.Format format, Image.Filter filterMode)
		{
			width = Image.GetPowerOf2Value(width);
			height = Image.GetPowerOf2Value(height);
			if (format == Image.Format.Default)
			{
				format = Image.Format.ARGB32;
			}
			if (filterMode == Image.Filter.Default)
			{
				filterMode = Image.DEFAULT_FILTER;
			}
			ImageAtlas imageAtlas = new ImageAtlas();
			imageAtlas.imgAtlas_ = (Image)IApplication.GetCurrentApp().RunOnUiThread(() => new Image(new Texture2D(width, height, (TextureFormat)format, false)
			{
				wrapMode = TextureWrapMode.Clamp,
				filterMode = (FilterMode)filterMode
			}), true);
			imageAtlas.imgAtlas_.Clear(Graphics.GetColorOfRGB(0, 0, 0, 0));
			imageAtlas.regions_ = new Dictionary<Image, AtlasRegion>();
			imageAtlas.hintRegions_ = new Dictionary<string, AtlasRegion>();
			imageAtlas.hintHeightMap_ = new HeightPoint[0];
			imageAtlas.spaces_ = new List<AtlasRegion>();
			imageAtlas.heightMap_ = new HeightPoint[width];
			for (int num = 0; num < imageAtlas.heightMap_.Length; num++)
			{
				imageAtlas.heightMap_[num] = new HeightPoint(num, 0);
			}
			imageAtlas.bspMap_ = new List<AtlasRegion>[height / 32][];
			for (int num2 = 0; num2 < imageAtlas.bspMap_.Length; num2++)
			{
				imageAtlas.bspMap_[num2] = new List<AtlasRegion>[width / 32];
				for (int num3 = 0; num3 < imageAtlas.bspMap_[num2].Length; num3++)
				{
					imageAtlas.bspMap_[num2][num3] = new List<AtlasRegion>();
				}
			}
			imageAtlas.bspList_ = new List<AtlasRegion>();
			return imageAtlas;
		}

		internal void Reset()
		{
			foreach (AtlasRegion value in regions_.Values)
			{
				value.Dispose();
			}
			regions_.Clear();
			spaces_.Clear();
			for (int i = 0; i < bspMap_.Length; i++)
			{
				for (int j = 0; j < bspMap_[i].Length; j++)
				{
					bspMap_[i][j].Clear();
				}
			}
			for (int k = 0; k < heightMap_.Length; k++)
			{
				heightMap_[k].Set(k, 0);
			}
			if (hintRegions_.Count > 0)
			{
				foreach (AtlasRegion value2 in hintRegions_.Values)
				{
					RegistBSP(value2);
				}
				for (int l = 0; l < heightMap_.Length && l < hintHeightMap_.Length; l++)
				{
					heightMap_[l].X = hintHeightMap_[l].X;
					heightMap_[l].Y = hintHeightMap_[l].Y;
				}
			}
			change_ = false;
		}

		internal void Dispose()
		{
			hintRegions_.Clear();
			Reset();
			if (imgAtlas_ != null)
			{
				imgAtlas_.Dispose();
			}
			imgAtlas_ = null;
		}

		public void Compaction()
		{
			if (!ImageAtlasManager.GetInstance().enable_)
			{
				return;
			}
			IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				if (!change_)
				{
					return;
				}
				try
				{
					foreach (AtlasRegion value in regions_.Values)
					{
						if (value.IsValid())
						{
							value.Pixels = imgAtlas_.GetNativePixels(value.X, value.Y, value.Width, value.Height);
						}
						else
						{
							value.Pixels = value.Image.GetNativePixels(0, 0, value.Width, value.Height);
						}
					}
					AtlasRegion[] array = regions_.Values.ToArray();
					float[] array2 = new float[array.Length];
					for (int i = 0; i < array.Length; i++)
					{
						array2[i] = array[i].Height * 100 + array[i].Width;
					}
					for (int j = 0; j < array2.Length - 1; j++)
					{
						for (int num = array2.Length - 1; num > j; num--)
						{
							if (array2[num - 1] < array2[num])
							{
								float num2 = array2[num];
								AtlasRegion atlasRegion = array[num];
								array2[num] = array2[num - 1];
								array[num] = array[num - 1];
								array2[num - 1] = num2;
								array[num - 1] = atlasRegion;
							}
						}
					}
					for (int k = 0; k < heightMap_.Length; k++)
					{
						heightMap_[k].Set(k, 0);
					}
					spaces_.Clear();
					for (int l = 0; l < bspMap_.Length; l++)
					{
						for (int m = 0; m < bspMap_[l].Length; m++)
						{
							bspMap_[l][m].Clear();
						}
					}
					for (int n = 0; n < array.Length; n++)
					{
						array[n].Valid = false;
					}
					imgAtlas_.Clear(Graphics.GetColorOfRGB(0, 0, 0, 0));
					for (int num3 = 0; num3 < array.Length; num3++)
					{
						Pack(array[num3]);
					}
					change_ = false;
				}
				catch (OutOfMemoryException)
				{
					if (Config.PRINT)
					{
						Log.Info("ImageAtlas.Compaction : OutOfMemory");
					}
				}
				finally
				{
					foreach (AtlasRegion value2 in regions_.Values)
					{
						value2.Pixels = null;
					}
					imgAtlas_.Apply();
				}
			}, true);
		}

		internal void ImageToAtlas(Image image)
		{
			AtlasRegion value = null;
			if (!regions_.TryGetValue(image, out value))
			{
				value = Pack(image);
				if (value.IsValid())
				{
					imgAtlas_.Apply();
				}
			}
		}

		internal void AtlasToImages()
		{
			AtlasRegion[] array = regions_.Values.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				AtlasToImage(array[i].Image);
			}
		}

		internal void AtlasToImage(Image image)
		{
			if (image != null)
			{
				AtlasRegion value = null;
				if (!image.IsDisposed && regions_.TryGetValue(image, out value) && value.IsValid() && (object)image.image_ == null)
				{
					Color[] nativePixels = imgAtlas_.GetNativePixels(value.X, value.Y, value.Width, value.Height);
					image.image_ = new Texture2D(value.Width, value.Height);
					image.SetNativePixels(0, 0, value.Width, value.Height, nativePixels);
					image.Apply();
				}
				RemoveAtlas(image);
			}
		}

		internal void RemoveAtlas(Image image)
		{
			AtlasRegion atlasRegion_ = image.atlasRegion_;
			if (atlasRegion_.IsValid())
			{
				AtlasRegion atlasRegion = new AtlasRegion(null);
				atlasRegion.X = atlasRegion_.X;
				atlasRegion.Y = atlasRegion_.Y;
				atlasRegion.Width = atlasRegion_.Width;
				atlasRegion.Height = atlasRegion_.Height;
				atlasRegion.Valid = true;
				spaces_.Add(atlasRegion);
				Rectangle bspRect = GetBspRect(atlasRegion_.X, atlasRegion_.Y, atlasRegion_.Width, atlasRegion_.Height);
				for (int i = bspRect.top; i <= bspRect.bottom; i++)
				{
					for (int j = bspRect.left; j <= bspRect.right; j++)
					{
						int num = bspMap_[i][j].IndexOf(atlasRegion_);
						if (num != -1)
						{
							bspMap_[i][j][num] = atlasRegion;
						}
					}
				}
				imgAtlas_.Clear(atlasRegion_.X, atlasRegion_.Y, atlasRegion_.Width, atlasRegion_.Height, Graphics.GetColorOfRGB(0, 0, 0, 0));
				change_ = true;
			}
			atlasRegion_.Dispose();
			regions_.Remove(image);
		}

		private AtlasRegion Pack(Image image, int width = 0, int height = 0, Color[] pixels = null)
		{
			if (width == 0)
			{
				width = image.GetWidth();
			}
			if (height == 0)
			{
				height = image.GetHeight();
			}
			image.GetAlphaType();
			AtlasRegion atlasRegion = new AtlasRegion(this);
			atlasRegion.Image = image;
			atlasRegion.Width = width;
			atlasRegion.Height = height;
			regions_.Add(image, atlasRegion);
			image.atlasRegion_ = atlasRegion;
			Pack(atlasRegion);
			return atlasRegion;
		}

		private void Pack(AtlasRegion region)
		{
			int width = imgAtlas_.GetWidth();
			int height = imgAtlas_.GetHeight();
			if (hintRegions_.Count > 0)
			{
				string key = region.Image.GetName() + "," + region.Width + "," + region.Height;
				AtlasRegion value = null;
				if (hintRegions_.TryGetValue(key, out value))
				{
					region.X = value.X;
					region.Y = value.Y;
					if (region.Pixels == null)
					{
						region.Pixels = region.Image.GetNativePixels(0, 0, region.Width, region.Height);
					}
					imgAtlas_.SetNativePixels(region.X, region.Y, region.Width, region.Height, region.Pixels);
					region.Pixels = null;
					region.Valid = true;
					if ((object)region.Image.image_ != null)
					{
						region.Image.Release();
					}
					return;
				}
			}
			region.Valid = false;
			bool flag = false;
			int num = int.MaxValue;
			int num2 = -1;
			for (int i = 0; i < spaces_.Count; i++)
			{
				if (spaces_[i].Width >= region.Width && spaces_[i].Height >= region.Height)
				{
					int num3 = spaces_[i].Width - region.Width;
					int num4 = spaces_[i].Height - region.Height;
					int num5 = num3 * num4 * 1000 + num3 * num4;
					if (num5 < num)
					{
						num = num5;
						num2 = i;
					}
				}
			}
			if (num < 5000)
			{
				region.X = spaces_[num2].X;
				region.Y = spaces_[num2].Y;
				if (region.Pixels == null)
				{
					region.Pixels = region.Image.GetNativePixels(0, 0, region.Width, region.Height);
				}
				imgAtlas_.SetNativePixels(region.X, region.Y, region.Width, region.Height, region.Pixels);
				UnregistBSP(spaces_[num2]);
				spaces_.RemoveAt(num2);
				flag = true;
			}
			if (!flag)
			{
				for (int j = 0; j < heightMap_.Length; j++)
				{
					HeightPoint heightPoint = heightMap_[j];
					if (width <= heightPoint.X + region.Width - 1 || height <= heightPoint.Y + region.Height - 1)
					{
						continue;
					}
					int num6 = 0;
					Rectangle bspRect = GetBspRect(heightPoint.X, heightPoint.Y, region.Width, region.Height);
					for (int k = bspRect.top; k <= bspRect.bottom; k++)
					{
						for (int l = bspRect.left; l <= bspRect.right; l++)
						{
							for (int m = 0; m < bspMap_[k][l].Count; m++)
							{
								bool flag2 = false;
								for (int n = 0; n < num6; n++)
								{
									if (bspList_[n] == bspMap_[k][l][m])
									{
										flag2 = true;
										break;
									}
								}
								if (!flag2)
								{
									if (num6 >= bspList_.Count)
									{
										bspList_.Add(null);
									}
									bspList_[num6] = bspMap_[k][l][m];
									num6++;
								}
							}
						}
					}
					int num7 = heightPoint.X - 1;
					int num8 = heightPoint.Y - 1;
					int num9 = region.Width + 2;
					int num10 = region.Height + 2;
					flag = true;
					for (int num11 = 0; num11 < num6; num11++)
					{
						AtlasRegion atlasRegion = bspList_[num11];
						if (atlasRegion.Valid && num7 <= atlasRegion.X + atlasRegion.Width - 1 && num8 <= atlasRegion.Y + atlasRegion.Height - 1 && num7 + num9 - 1 >= atlasRegion.X && num8 + num10 - 1 >= atlasRegion.Y)
						{
							flag = false;
							break;
						}
					}
					if (!flag)
					{
						continue;
					}
					region.X = heightPoint.X;
					region.Y = heightPoint.Y;
					if (region.Pixels == null)
					{
						region.Pixels = region.Image.GetNativePixels(0, 0, region.Width, region.Height);
					}
					imgAtlas_.SetNativePixels(region.X, region.Y, region.Width, region.Height, region.Pixels);
					for (int num12 = 0; num12 < heightMap_.Length; num12++)
					{
						if (region.X <= heightMap_[num12].X && heightMap_[num12].X <= region.X + region.Width - 1)
						{
							heightMap_[num12].Set(region.Y + region.Height + 1);
						}
					}
					for (int num13 = 0; num13 < heightMap_.Length - 1; num13++)
					{
						for (int num14 = heightMap_.Length - 1; num14 > num13; num14--)
						{
							if (heightMap_[num14 - 1].Y * 100 + heightMap_[num14 - 1].X > heightMap_[num14].Y * 100 + heightMap_[num14].X)
							{
								HeightPoint heightPoint2 = heightMap_[num14];
								heightMap_[num14] = heightMap_[num14 - 1];
								heightMap_[num14 - 1] = heightPoint2;
							}
						}
					}
					break;
				}
			}
			if (!flag && num2 != -1)
			{
				region.X = spaces_[num2].X;
				region.Y = spaces_[num2].Y;
				if (region.Pixels == null)
				{
					region.Pixels = region.Image.GetNativePixels(0, 0, region.Width, region.Height);
				}
				imgAtlas_.SetNativePixels(region.X, region.Y, region.Width, region.Height, region.Pixels);
				UnregistBSP(spaces_[num2]);
				spaces_.RemoveAt(num2);
				flag = true;
			}
			if (flag)
			{
				region.Pixels = null;
				region.Valid = true;
				if ((object)region.Image.image_ != null)
				{
					region.Image.Release();
				}
				RegistBSP(region);
			}
			if (!flag && region.Pixels != null && (object)region.Image.image_ == null)
			{
				region.Image.image_ = new Texture2D(region.Width, region.Height);
				region.Image.SetNativePixels(0, 0, region.Width, region.Height, region.Pixels);
				region.Pixels = null;
				region.Image.Apply();
			}
			region.Pixels = null;
			change_ = true;
		}

		private void RegistBSP(AtlasRegion region)
		{
			Rectangle bspRect = GetBspRect(region.X, region.Y, region.Width, region.Height);
			for (int i = bspRect.top; i <= bspRect.bottom; i++)
			{
				for (int j = bspRect.left; j <= bspRect.right; j++)
				{
					bspMap_[i][j].Add(region);
				}
			}
		}

		private void UnregistBSP(AtlasRegion region)
		{
			Rectangle bspRect = GetBspRect(region.X, region.Y, region.Width, region.Height);
			for (int i = bspRect.top; i <= bspRect.bottom; i++)
			{
				for (int j = bspRect.left; j <= bspRect.right; j++)
				{
					bspMap_[i][j].Remove(region);
				}
			}
		}

		private static bool CheckHitRect(int x1, int y1, int w1, int h1, int x2, int y2, int w2, int h2)
		{
			if (x1 <= x2 + w2 - 1 && y1 <= y2 + h2 - 1 && x1 + w1 - 1 >= x2)
			{
				return y1 + h1 - 1 >= y2;
			}
			return false;
		}

		private Rectangle GetBspRect(int x, int y, int width, int height)
		{
			Rectangle getBspRect_Result = GetBspRect_Result;
			getBspRect_Result.left = (x - 1) / 32;
			getBspRect_Result.top = (y - 1) / 32;
			getBspRect_Result.right = (x + width + 1 + 32 - 1) / 32;
			getBspRect_Result.bottom = (y + height + 1 + 32 - 1) / 32;
			if (getBspRect_Result.left < 0)
			{
				getBspRect_Result.left = 0;
			}
			if (getBspRect_Result.top < 0)
			{
				getBspRect_Result.top = 0;
			}
			if (getBspRect_Result.right >= bspMap_[0].Length)
			{
				getBspRect_Result.right = bspMap_[0].Length - 1;
			}
			if (getBspRect_Result.bottom >= bspMap_.Length)
			{
				getBspRect_Result.bottom = bspMap_.Length - 1;
			}
			return getBspRect_Result;
		}

		public Image GetImage()
		{
			return imgAtlas_;
		}

		public void GetAtlasInfo(out int totalPixels, out int usedPixels, out int fragmentPixels, out int totalImages, out int atlasImages)
		{
			totalPixels = imgAtlas_.GetWidth() * imgAtlas_.GetHeight();
			totalImages = regions_.Count;
			fragmentPixels = 0;
			for (int i = 0; i < spaces_.Count; i++)
			{
				fragmentPixels += spaces_[i].Width * spaces_[i].Height;
			}
			usedPixels = 0;
			atlasImages = 0;
			foreach (AtlasRegion value in regions_.Values)
			{
				if (value.IsValid())
				{
					usedPixels += value.Width * value.Height;
					atlasImages++;
				}
			}
		}

		public string GetHint()
		{
			List<string> list = new List<string>();
			list.Add(string.Concat(heightMap_.Length));
			HeightPoint[] array = heightMap_;
			foreach (HeightPoint heightPoint in array)
			{
				list.Add(heightPoint.X + "," + heightPoint.Y);
			}
			list.Add(string.Concat(regions_.Values.Count));
			foreach (AtlasRegion value in regions_.Values)
			{
				list.Add(value.Image.GetName() + "," + value.X + "," + value.Y + "," + value.Width + "," + value.Height);
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int j = 0; j < list.Count; j++)
			{
				if (j != 0)
				{
					stringBuilder.Append("\n");
				}
				stringBuilder.Append(list[j]);
			}
			return stringBuilder.ToString();
		}

		public void SetHint(string data)
		{
			hintRegions_.Clear();
			Reset();
			try
			{
				string[] array = data.Split('\n');
				int num = 0;
				int num2 = int.Parse(array[num++]);
				hintHeightMap_ = new HeightPoint[num2];
				for (int i = 0; i < num2; i++)
				{
					string[] array2 = array[num++].Split(',');
					if (i < hintHeightMap_.Length)
					{
						hintHeightMap_[i] = new HeightPoint(int.Parse(array2[0]), int.Parse(array2[1]));
					}
				}
				num2 = int.Parse(array[num++]);
				for (int j = 0; j < num2; j++)
				{
					string[] array3 = array[num++].Split(',');
					string key = array3[0] + "," + array3[3] + "," + array3[4];
					AtlasRegion atlasRegion = new AtlasRegion(this);
					atlasRegion.Image = null;
					atlasRegion.X = int.Parse(array3[1]);
					atlasRegion.Y = int.Parse(array3[2]);
					atlasRegion.Width = int.Parse(array3[3]);
					atlasRegion.Height = int.Parse(array3[4]);
					atlasRegion.Valid = false;
					hintRegions_[key] = atlasRegion;
					RegistBSP(atlasRegion);
				}
				for (int k = 0; k < heightMap_.Length && k < hintHeightMap_.Length; k++)
				{
					heightMap_[k].X = hintHeightMap_[k].X;
					heightMap_[k].Y = hintHeightMap_[k].Y;
				}
			}
			catch (Exception t)
			{
				Log.Info(t);
				hintRegions_.Clear();
			}
		}

		public byte[] GetPng()
		{
			return ((Texture2D)imgAtlas_.GetImage()).EncodeToPNG();
		}
	}
}
