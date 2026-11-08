using kairo.unity.surface;
namespace kairo.unity.surface{public class TouchEvent{public int dx,dy,pinch;}}
public class Vec{public int Size(){return 0;}public object FirstElement(){return null;}}
public class Vector3D{public void Set(float x,float y,float z){}}
public static class AppData{public static int Clamp(int x,int min,int max){return x;}public static int ClampMin(int x,int min){return x;}}
public class Fixture{public int scale_,pinchScaleValue_;public Vec touchCameraPoint_;public Vector3D touchCameraSpeed_,touchCameraDecimal_;public bool touchDragCamera_,touchBuildDrag_;
public int GetScaleMin(){return 0;}public int GetScaleMax(){return 0;}public void SetScale(int x){}
public void Run(int type,int v,TouchEvent e){
			float num = e.dx * 100 / scale_;
			float num2 = e.dy * 100 / scale_;
			switch (type)
			{
			case 5:
				if (touchCameraPoint_.Size() != 0)
				{
				}
				break;
			case 1:
				if (touchCameraPoint_.Size() > 0 && touchDragCamera_)
				{
					int[] array = (int[])touchCameraPoint_.FirstElement();
					touchCameraSpeed_.Set((int)num - array[0], 0f, (int)num2 - array[1]);
					touchCameraDecimal_.Set(0f, 0f, 0f);
				}
				break;
			case 12:
			{
				touchBuildDrag_ = false;
				pinchScaleValue_ = AppData.Clamp(pinchScaleValue_ + e.pinch, GetScaleMin(), GetScaleMax() + 40);
				int num3 = pinchScaleValue_;
				if (num3 > 100)
				{
					num3 = AppData.ClampMin(num3 - 40, 100);
				}
				SetScale(num3);
				touchDragCamera_ = false;
				break;
			}
			}
		}
}