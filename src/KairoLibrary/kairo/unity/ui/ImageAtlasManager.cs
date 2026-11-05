using System;
using System.Collections.Generic;

namespace kairo.unity.ui
{
	public class ImageAtlasManager : IDisposable
	{
		private static ImageAtlasManager instance_;

		private List<ImageAtlas> imageAtlas_ = new List<ImageAtlas>();

		internal bool enable_ = true;

		private ImageAtlasManager()
		{
		}

		public static ImageAtlasManager GetInstance()
		{
			if (instance_ == null)
			{
				instance_ = new ImageAtlasManager();
			}
			return instance_;
		}

		public void Dispose()
		{
			IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				for (int num = imageAtlas_.Count - 1; num >= 0; num--)
				{
					if (imageAtlas_[num] != null)
					{
						imageAtlas_[num].Dispose();
					}
					imageAtlas_[num] = null;
				}
			}, true);
		}

		public void CreateImageAtlas(int id, int width = 512, int height = 512, Image.Format format = Image.Format.ARGB32, Image.Filter filterMode = Image.Filter.Default)
		{
			if (!enable_)
			{
				return;
			}
			while (id >= imageAtlas_.Count)
			{
				imageAtlas_.Add(null);
			}
			IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				if (format == Image.Format.Default)
				{
					format = Image.Format.ARGB32;
				}
				if (filterMode == Image.Filter.Default)
				{
					filterMode = Image.DEFAULT_FILTER;
				}
				if (imageAtlas_[id] != null)
				{
					Image image = imageAtlas_[id].GetImage();
					imageAtlas_[id].AtlasToImages();
					if (image.GetWidth() == width && image.GetHeight() == height && image.format_ == format)
					{
						imageAtlas_[id].Reset();
						image.SetFilterMode(filterMode);
					}
					else
					{
						imageAtlas_[id].Dispose();
						imageAtlas_[id] = null;
					}
				}
				if (imageAtlas_[id] == null)
				{
					imageAtlas_[id] = ImageAtlas.CreateImageAtlas(width, height, format, filterMode);
				}
			}, true);
		}

		public void Compaction(int id = -1)
		{
			if (!enable_)
			{
				return;
			}
			IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				if (id == -1)
				{
					for (int i = 0; i < imageAtlas_.Count; i++)
					{
						if (imageAtlas_[i] != null)
						{
							imageAtlas_[i].Compaction();
						}
					}
				}
				else if (imageAtlas_[id] != null)
				{
					imageAtlas_[id].Compaction();
				}
			}, true);
		}

		public int GetAtlasCount()
		{
			return imageAtlas_.Count;
		}

		public ImageAtlas GetAtlas(int id)
		{
			return imageAtlas_[id];
		}

		internal void ImageToAtlas(int id, Image image)
		{
			IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				if (id >= imageAtlas_.Count || imageAtlas_[id] == null)
				{
					CreateImageAtlas(id);
				}
				imageAtlas_[id].ImageToAtlas(image);
			}, true);
		}

		internal void AtlasToImage(Image image)
		{
			IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				if (image.atlasRegion_ != null && (object)image.image_ == null)
				{
					image.atlasRegion_.Atlas.AtlasToImage(image);
				}
			}, true);
		}

		internal void RemoveAtlas(Image image)
		{
			IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				if (image.atlasRegion_ != null)
				{
					image.atlasRegion_.Atlas.RemoveAtlas(image);
				}
			}, true);
		}

		public void SetEnable(bool enable)
		{
			enable_ = enable;
		}
	}
}
