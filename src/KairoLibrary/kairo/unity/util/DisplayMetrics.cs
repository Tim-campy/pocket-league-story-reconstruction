using kairo.unity.native;
using kairo.unity.ui;
using UnityEngine;

namespace kairo.unity.util
{
	public class DisplayMetrics
	{
		public static float Density { get; protected set; }

		public static int DensityDPI { get; protected set; }

		public static int HeightPixels { get; protected set; }

		public static int WidthPixels { get; protected set; }

		public static float ScaledDensity { get; protected set; }

		public static float XDPI { get; protected set; }

		public static float YDPI { get; protected set; }

		static DisplayMetrics()
		{
			Update();
		}

		internal static void Update()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					using (new AndroidJavaClass("android.util.DisplayMetrics"))
					{
						using (AndroidJavaObject androidJavaObject = new AndroidJavaObject("android.util.DisplayMetrics"))
						{
							using (AndroidJavaObject androidJavaObject2 = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
							{
								using (AndroidJavaObject androidJavaObject3 = androidJavaObject2.Call<AndroidJavaObject>("getWindowManager", new object[0]))
								{
									using (AndroidJavaObject androidJavaObject4 = androidJavaObject3.Call<AndroidJavaObject>("getDefaultDisplay", new object[0]))
									{
										using (AndroidJavaObject androidJavaObject5 = new AndroidJavaObject("android.graphics.Point"))
										{
											IApplication currentApp = IApplication.GetCurrentApp();
											androidJavaObject4.Call("getMetrics", androidJavaObject);
											Density = androidJavaObject.Get<float>("density");
											DensityDPI = androidJavaObject.Get<int>("densityDpi");
											HeightPixels = (int)((float)androidJavaObject.Get<int>("heightPixels") * currentApp.GetResolutionScaleRatio() / 100f + 0.01f);
											WidthPixels = (int)((float)androidJavaObject.Get<int>("widthPixels") * currentApp.GetResolutionScaleRatio() / 100f + 0.01f);
											ScaledDensity = androidJavaObject.Get<float>("scaledDensity");
											XDPI = androidJavaObject.Get<float>("xdpi");
											YDPI = androidJavaObject.Get<float>("ydpi");
											string[] array = KairoPlugin.GetReleaseNumber().Split('.');
											int num = 0;
											if (array.Length != 0)
											{
												num += int.Parse(array[0]) * 100;
											}
											if (1 < array.Length)
											{
												num += int.Parse(array[1]) * 10;
											}
											if (2 < array.Length)
											{
												num += int.Parse(array[2]) * 1;
											}
											if (kairo.unity.ui.Canvas.GetInstance().IsFullscreen() && num >= 440)
											{
												androidJavaObject4.Call("getRealSize", androidJavaObject5);
												WidthPixels = (int)((float)androidJavaObject5.Get<int>("x") * currentApp.GetResolutionScaleRatio() / 100f + 0.01f);
												HeightPixels = (int)((float)androidJavaObject5.Get<int>("y") * currentApp.GetResolutionScaleRatio() / 100f + 0.01f);
											}
											return;
										}
									}
								}
							}
						}
					}
				}
			}
			Density = 1.6f;
			DensityDPI = 240;
			HeightPixels = Screen.height;
			WidthPixels = Screen.width;
			ScaledDensity = 1.6f;
			XDPI = 1.6f;
			YDPI = 1.6f;
		}
	}
}
