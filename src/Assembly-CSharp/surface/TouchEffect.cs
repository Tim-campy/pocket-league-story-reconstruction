using java.lang;
using kairo.unity.surface;
using kairo.unity.ui;

namespace surface
{
	public class TouchEffect
	{
		public const int ID_BUTTON = 0;

		public const int ID_BUILDTAB = 1;

		public const int ID_HUMAN = 2;

		public const int ID_MAPCHIP = 3;

		public SurfaceBase surface_;

		public float x_;

		public float y_;

		public float vx_;

		public float vy_;

		public float ax_;

		public float ay_;

		public long time_;

		public long key_;

		public int id_;

		public object arg_;

		internal long lastFrameTime_;

		public void Init(SurfaceBase surface, int id, float x, float y, TouchComponent component)
		{
			Init(surface, id, x, y, component, null);
		}

		public void Init(SurfaceBase surface, int id, float x, float y, TouchComponent component, object arg)
		{
			surface_ = surface;
			id_ = id;
			x_ = x;
			y_ = y;
			time_ = (lastFrameTime_ = JSystem.CurrentTimeMillis());
			arg_ = arg;
			key_ = 0L;
			if (component != null)
			{
				key_ = component.GetKey();
			}
			vx_ = (vy_ = 0f);
			ax_ = (ay_ = 0f);
		}

		public bool Frame()
		{
			int num = 8;
			int num2 = (int)(JSystem.CurrentTimeMillis() - time_) / 50;
			if (num2 >= num)
			{
				return true;
			}
			double num3 = vx_;
			double num4 = vy_;
			double num5 = (double)(JSystem.CurrentTimeMillis() - lastFrameTime_) / 1000.0;
			x_ += (float)((double)vx_ * num5);
			y_ += (float)((double)vy_ * num5);
			vx_ += (float)((double)ax_ * num5);
			vy_ += (float)((double)ay_ * num5);
			if ((num3 >= 0.0 && vx_ < 0f) || (vx_ >= 0f && num3 < 0.0))
			{
				vx_ = 0f;
			}
			if ((num4 >= 0.0 && vy_ < 0f) || (vy_ >= 0f && num4 < 0.0))
			{
				vx_ = 0f;
			}
			lastFrameTime_ = JSystem.CurrentTimeMillis();
			return false;
		}

		public void Draw(Graphics g)
		{
			float num = surface_.GetScaleRatio() / 100f;
			int num2 = (int)(JSystem.CurrentTimeMillis() - time_) / 50;
			if (id_ == 0)
			{
				object[] array = (object[])arg_;
				Image image = (Image)array[0];
				float[] array2 = (float[])array[1];
				int num3 = 160 - 18 * num2;
				if (num3 <= 0) { return; }
float num4 = 2.5f * (float)num2 * num;
float num5 = 2.5f * (float)num2 * num;
float dx = (float)surface_.GetX() + x_ * num - num4 / 2f;
float dy = (float)surface_.GetY() + y_ * num - num5 / 2f;
float num6 = array2[0] * num;
float num7 = array2[1] * num;
g.SetRenderMode(1, num3, 255 - num3);
g.DrawScaledImage(image, dx, dy, num6 + num4, num7 + num5, (int)(array2[2] + 0.5f), (int)(array2[3] + 0.5f), (int)(array2[4] + 0.5f), (int)(array2[5] + 0.5f));
g.SetRenderMode(0, 255, 0);
			}
			else if (id_ == 1) { } else if (id_ == 2) { } else if (id_ == 3) { }
		}
	}
}
