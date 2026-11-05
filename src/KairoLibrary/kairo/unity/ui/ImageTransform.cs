using System;
using kairo.unity.m3d4.util3d;

namespace kairo.unity.ui
{
	public class ImageTransform
	{
		public int x;

		public int y;

		public float scaleX;

		public float scaleY;

		public float rotate;

		public int localX;

		public int localY;

		protected int imageTransX_;

		protected int imageTransY_;

		protected float imageScaleX_;

		protected float imageScaleY_;

		private static float cacheRotate = float.NaN;

		private static float cacheSin;

		private static float cacheCos;

		protected int[] transform = new int[6];

		private static int[] getPoint_result = new int[2];

		private static ImageTransform getPoint_transform = new ImageTransform();

		public ImageTransform()
		{
			Init();
		}

		public void Init()
		{
			x = (y = 0);
			scaleX = (scaleY = 1f);
			rotate = 0f;
			localX = (localY = 0);
			imageTransX_ = (imageTransY_ = 0);
			imageScaleX_ = (imageScaleY_ = 1f);
		}

		public void Copy(ImageTransform from)
		{
			x = from.x;
			y = from.y;
			scaleX = from.scaleX;
			scaleY = from.scaleY;
			rotate = from.rotate;
			localX = from.localX;
			localY = from.localY;
			imageTransX_ = from.imageTransX_;
			imageTransY_ = from.imageTransY_;
			imageScaleX_ = from.imageScaleX_;
			imageScaleY_ = from.imageScaleY_;
		}

		public void Set(int x, int y, float scaleX, float scaleY, float rotate)
		{
			Set(x, y, scaleX, scaleY, rotate, 0, 0, 0, 0, 1f, 1f);
		}

		public void Set(int x, int y, float scaleX, float scaleY, float rotate, int localX, int localY)
		{
			Set(x, y, scaleX, scaleY, rotate, localX, localY, 0, 0, 1f, 1f);
		}

		protected void Set(int x, int y, float scaleX, float scaleY, float rotate, int localX, int localY, int imageTransX, int imageTransY, float imageScaleX, float imageScaleY)
		{
			this.x = x;
			this.y = y;
			this.scaleX = scaleX;
			this.scaleY = scaleY;
			this.rotate = rotate;
			this.localX = localX;
			this.localY = localY;
			imageTransX_ = imageTransX;
			imageTransY_ = imageTransY;
			imageScaleX_ = imageScaleX;
			imageScaleY_ = imageScaleY;
		}

		public void SetPosition(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public void SetScale(float scale)
		{
			scaleX = scale;
			scaleY = scale;
		}

		public void SetScale(float sx, float sy)
		{
			scaleX = sx;
			scaleY = sy;
		}

		public void SetRotate(float angle)
		{
			rotate = angle;
		}

		public void SetLocalOffset(int x, int y)
		{
			localX = x;
			localY = y;
		}

		public int[] GetTransform()
		{
			return GetTransform(false);
		}

		protected int[] GetTransform(bool translate)
		{
			if (ImageTransform.cacheRotate != this.rotate)
			{
				ImageTransform.cacheRotate = this.rotate;
				ImageTransform.cacheSin = FastMath.Sin(ImageTransform.cacheRotate);
				ImageTransform.cacheCos = FastMath.Cos(ImageTransform.cacheRotate);
			}
			float single = ImageTransform.cacheCos;
			float single1 = ImageTransform.cacheSin;
			int[] numArray = this.transform;
			if (!translate)
			{
				numArray[0] = (int)(single * this.scaleX * this.imageScaleX_ * 4096f);
				numArray[1] = (int)(-single1 * this.scaleY * this.imageScaleY_ * 4096f);
				numArray[3] = (int)(single1 * this.scaleX * this.imageScaleX_ * 4096f);
				numArray[4] = (int)(single * this.scaleY * this.imageScaleY_ * 4096f);
			}
			numArray[2] = (int)((single * this.scaleX * (float)this.imageTransX_ + -single1 * this.scaleY * (float)this.imageTransY_ + single * this.scaleX * (float)this.localX + -single1 * this.scaleY * (float)this.localY + (float)this.x) * 4096f);
			numArray[5] = (int)((single1 * this.scaleX * (float)this.imageTransX_ + single * this.scaleY * (float)this.imageTransY_ + single1 * this.scaleX * (float)this.localX + single * this.scaleY * (float)this.localY + (float)this.y) * 4096f);
			return numArray;
		}

		public int[] GetPoint()
		{
			int[] array = getPoint_result;
			int[] array2 = GetTransform(true);
			array[0] = array2[2] / 4096;
			array[1] = array2[5] / 4096;
			return array;
		}

		public static int[] GetPoint(int x, int y, float scaleX, float scaleY, float rotate, int localX, int localY)
		{
			getPoint_transform.Set(x, y, scaleX, scaleY, rotate, localX, localY);
			return getPoint_transform.GetPoint();
		}
	}
}
