using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using java.lang;
using kairo.common.cfg;
using kairo.unity.analytics;
using kairo.unity.form;
using kairo.unity.graphics;
using kairo.unity.io;
using kairo.unity.native;
using kairo.unity.panel;
using kairo.unity.surface;
using kairo.unity.system;
using kairo.unity.system.notification.ui;
using kairo.unity.util;
using UnityEngine;

namespace kairo.unity.ui
{
	public abstract class IApplication : MonoBehaviour
	{

	protected IApplication()
	{
	}
		public delegate object UIMethod();

		private class MethodTask
		{
			public UIMethod method1;

			public ThreadStart method2;

			public Exception error;

			public bool finished;

			public object result;

			public MethodTask(UIMethod method)
			{
				method1 = method;
			}

			public MethodTask(ThreadStart method)
			{
				method2 = method;
			}
		}

		public enum Seb
		{
			ButtonLanguage,
			ConnectingBg,
			ConnectingAnim,
			Balloon
		}

		private static IApplication instance_;

		protected static string platform_ = null;

		protected bool termination_;

		protected bool visible_;

		private DisplayMetrics metrics_;

		private ScreenOrientation defaultOrientation_;

		private bool isFocus_;

		private bool isPause_;

		internal Handler handler_ = new Handler();

		private List<MethodTask> uiMethodTasks_ = new List<MethodTask>();

		private List<MethodTask> updateMethodTasks_ = new List<MethodTask>();

		private JThread uiThread_;

		private int totalFrames_;

		private int viewportWidth_ = 240;

		private int viewportHeight_ = 240;

		private object lockUITask_ = new object();

		private string lastPermitPermissionName_ = "";

		private string lastNotPermitPermissionName_ = "";

		private string dataPath_;

		private bool rendering_;

		private bool update_;

		private float fixedScaleRatio_ = float.NaN;

		private long renderBeginTime_;

		private long renderTime_;

		private long updateTime_;

		private long lastUpdateDisplayMetrics_;

		private bool created_;

		private float resolutionScaleRatio_ = 100f;

		private bool changeResolution_;

		private int changeResolutionWidth_;

		private int changeResolutionHeight_;

		private float changeResolutionScaleRatio_;

		private int resolutionChangedWait_;

		private float resolutionScaleMax_ = float.NaN;

		private int delayQuitFrames_;

		private ResourceManager resource_;

		private static int isTabletResult_ = -1;

		private static int getSystemBarHeightResult_ = -1;

		private static int[] GetViewport_result = new int[2];

		protected void Init(IApplication app)
		{
			instance_ = app;
		}

		public static IApplication GetCurrentApp()
		{
			return instance_;
		}

		public string GetSourceURL()
		{
			return Config.URL_PACKAGE;
		}

		private void Awake()
		{
			try
			{
				uiThread_ = JThread.CurrentThread();
				Type config = null;
				Type[] types = Assembly.GetAssembly(instance_.GetType()).GetTypes();
				foreach (Type type in types)
				{
					if (type.IsSubclassOf(typeof(Config)))
					{
						config = type;
						break;
					}
				}
				Config.Create(config);
				if (Config.UNITY_EDITOR)
				{
					dataPath_ = string.Concat(Application.persistentDataPath, "/", Application.productName);
				}
				else if (Application.platform == RuntimePlatform.IPhonePlayer)
				{
					string text = string.Concat(Application.persistentDataPath, "/", Application.productName);
					string text2 = Application.temporaryCachePath.Replace("Caches", "Application Support");
					string text3 = "";
					if (Config.ICLOUD)
					{
						text3 = text2;
						dataPath_ = text;
					}
					else
					{
						text3 = text;
						dataPath_ = text2;
					}
					if (Directory.Exists(text3) && Directory.GetDirectories(text3).Length != 0 && (!Directory.Exists(dataPath_) || Directory.GetDirectories(dataPath_).Length == 0))
					{
						bool flag = false;
						try
						{
							CopyDirectory(text3, dataPath_);
							flag = true;
						}
						catch (Exception ex)
						{
							Log.Info(string.Concat("移行に失敗しました。", ex));
							if (Directory.Exists(dataPath_))
							{
								Directory.Delete(dataPath_, true);
							}
							dataPath_ = text3;
						}
						if (flag)
						{
							Directory.Delete(text3, true);
						}
					}
					Log.Info(string.Concat("[DataPath] ", dataPath_));
				}
				else if (Application.platform == RuntimePlatform.WindowsPlayer)
				{
					dataPath_ = Application.dataPath;
					Log.Info(string.Concat("DataPath[Windows] ", dataPath_));
				}
				else
				{
					dataPath_ = Application.persistentDataPath;
				}
				created_ = true;
				KairoPlugin.Init(Language.Get(), Config.PRINT, Config.BASE64_PUBLIC_KEY);
				KairoPlugin.CancelNotifications(true, false);
				KairoPlugin.SetNotificationBackground(false);
				AnalyticsTracker.Setup();
				if (Config.FULLSCREEN && (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight))
				{
					Config.FULLSCREEN = true;
				}
				if (!Config.FULLSCREEN)
				{
					KairoPlugin.Fullscreen(false);
				}
				Camera.main.orthographicSize = Screen.height / 2;
				Camera.main.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.transform.position.z);
				metrics_ = new DisplayMetrics();
				Canvas.GetInstance().SetDisplayCurrent(this);
				platform_ = SystemInfo.deviceModel;
				defaultOrientation_ = Screen.orientation;
				try
				{
					ResourceManager resourceManager = new ResourceManager();
					resourceManager.Load(Storage.Read(3, false, "kairolib.dat"));
					Image[] img = resourceManager.img;
					foreach (Image image in img)
					{
						if (image != null)
						{
							image.SetExpiration(true, 40);
						}
					}
					resource_ = resourceManager;
				}
				catch
				{
				}
				OnCreate();
				Language.Init();
			}
			catch (Exception e)
			{
				Error(e);
			}
		}

		private static void CopyDirectory(string src, string dst)
		{
			if (Directory.Exists(src))
			{
				Directory.CreateDirectory(dst);
				string[] files = Directory.GetFiles(src);
				foreach (string text in files)
				{
					File.Copy(text, dst + "/" + Path.GetFileName(text));
				}
				files = Directory.GetDirectories(src);
				foreach (string text2 in files)
				{
					CopyDirectory(text2, dst + "/" + Path.GetFileName(text2));
				}
			}
		}

		private void Start()
		{
			try
			{
				if (!Config.FULLSCREEN)
				{
					KairoPlugin.Fullscreen(false);
				}
				Time.fixedDeltaTime = 1f;
				if (!float.IsNaN(resolutionScaleMax_))
				{
					SetResolutionScaleRatio(resolutionScaleMax_);
				}
				Camera.main.orthographicSize = Screen.height / 2;
				Camera.main.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.transform.position.z);
				if (QualitySettings.GetQualityLevel() >= 1 && Config.DEBUG)
				{
					KairoPlugin.ShowToast("Quality Settings が " + QualitySettings.names[QualitySettings.GetQualityLevel()] + " の為、古い端末で速度低下の恐れがあります。" + QualitySettings.names[0] + ", " + QualitySettings.names[1] + " を推奨します。");
				}
				OnStart();
				Screen.sleepTimeout = -1;
				visible_ = true;
			}
			catch (Exception e)
			{
				Error(e);
			}
		}

		private void Update()
		{
			if (delayQuitFrames_ > 0)
			{
				if (delayQuitFrames_ == 1)
				{
					Application.Quit();
				}
				else
				{
					delayQuitFrames_--;
				}
			}
			else
			{
				if (TerminateCheck())
				{
					return;
				}
				UpdateResolution();
				totalFrames_++;
				long num = JSystem.CurrentTimeMillis();
				update_ = true;
				if (JSystem.CurrentTimeMillis() - lastUpdateDisplayMetrics_ > 1000 && FepPanel.Current == null)
				{
					DisplayMetrics.Update();
					lastUpdateDisplayMetrics_ = JSystem.CurrentTimeMillis();
				}
				try
				{
					ExecuteUIMethods();
					ExecuteUpdateMethods();
					OnUpdate();
				}
				catch (Exception e)
				{
					Error(e);
				}
				finally
				{
					update_ = false;
				}
				updateTime_ = JSystem.CurrentTimeMillis() - num;
				TerminateCheck();
			}
		}

		private void LateUpdate()
		{
			if (this.TerminateCheck())
			{
				return;
			}
			this.UpdateResolution();
			this.ExecuteUIMethods();
			this.TerminateCheck();
		}

		private void OnPreRender()
		{
			renderBeginTime_ = JSystem.CurrentTimeMillis();
		}

		private void OnGUI()
		{
			if (TerminateCheck())
			{
				return;
			}
			UpdateResolution();
			if (resolutionChangedWait_ > 0)
			{
				resolutionChangedWait_--;
			}
			else
			{
				if (changeResolution_)
				{
					return;
				}
				rendering_ = true;
				try
				{
					ExecuteUIMethods();
					if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseUp)
					{
						Canvas.GetInstance().OnInputUpdate();
					}
					if (Event.current.type != EventType.Repaint || termination_)
					{
						return;
					}
					try
					{
						Graphics.InitGUIGroup();
						OnDraw();
					}
					catch (Exception e)
					{
						if (!termination_)
						{
							Error(e);
							FormManagerBase formManager = GetFormManager();
							if (formManager != null)
							{
								formManager.RemoveAllForms();
							}
						}
					}
					finally
					{
						Graphics.EndGUIGroup();
					}
				}
				finally
				{
					rendering_ = false;
				}
				renderTime_ = JSystem.CurrentTimeMillis() - renderBeginTime_;
			}
		}

		private void OnApplicationPause(bool pauseStatus)
		{
			isPause_ = pauseStatus;
			if (pauseStatus)
			{
				NotifyManager.GetInstance()?.DoNotify();
				KairoPlugin.SetNotificationBackground(true);
				FormManagerBase.ProcessEvent(AppEventType.Hidden);
				FormManagerBase formManager = GetFormManager();
				for (int num = formManager.GetFormsNum() - 1; num >= 0; num--)
				{
					FormBase form = formManager.GetForm(num);
					try
					{
						form.Suspend();
					}
					catch (Exception)
					{
					}
				}
				OnSuspend();
				return;
			}
			NotifyManager.GetInstance()?.DoCancel();
			KairoPlugin.SetNotificationBackground(false);
			FormManagerBase.ProcessEvent(AppEventType.Resume);
			AnalyticsTracker.DispatchPageView();
			FormManagerBase formManager2 = GetFormManager();
			for (int num2 = formManager2.GetFormsNum() - 1; num2 >= 0; num2--)
			{
				FormBase form2 = formManager2.GetForm(num2);
				try
				{
					form2.Resume();
				}
				catch (Exception)
				{
				}
			}
			OnResume();
		}

		private void OnApplicationFocus(bool focusStatus)
		{
			if (Config.UNITY_EDITOR && !focusStatus)
			{
				return;
			}
			this.isFocus_ = focusStatus;
			if (focusStatus)
			{
				this.OnRestart();
				this.visible_ = true;
				return;
			}
			this.visible_ = false;
			this.OnStop();
			SoundPlayer.GetInstance().Suspend();
		}

		private void OnApplicationQuit()
		{
			NotifyManager instance = NotifyManager.GetInstance();
			if (instance != null)
			{
				instance.DoNotify();
			}
			KairoPlugin.SetNotificationBackground(true);
			FormManagerBase.ProcessEvent(AppEventType.Exit);
			OnDestroy();
			KairoPlugin.Finish();
		}

		private bool TerminateCheck()
		{
			FormManagerBase formManager = GetFormManager();
			if ((formManager == null || formManager.GetFormsNum() == 0) && delayQuitFrames_ <= 0)
			{
				Terminate();
				if (Config.UNITY_EDITOR)
				{
					Type type = Type.GetType("UnityEditor.EditorApplication, UnityEditor.dll");
					if (type != null)
					{
						type.GetProperty("isPlaying").SetValue(null, false, null);
					}
				}
				if (Canvas.lastOpenBrowserFrame_ == GetTotalFrames())
				{
					delayQuitFrames_ = 3;
				}
				else
				{
					Application.Quit();
				}
				return true;
			}
			return false;
		}

		protected virtual void OnCreate()
		{
		}

		protected virtual void OnStart()
		{
		}

		protected virtual void OnRestart()
		{
		}

		protected virtual void OnStop()
		{
		}

		protected virtual void OnResume()
		{
		}

		protected virtual void OnSuspend()
		{
		}

		protected abstract void OnUpdate();

		protected abstract void OnDraw();

		protected virtual void OnRender()
		{
		}

		protected virtual void OnDestroy()
		{
		}

		public void Terminate()
		{
			if (this.termination_)
			{
				return;
			}
			this.termination_ = true;
			FormManagerBase.ProcessEvent(AppEventType.Exit, new object[0]);
			KairoBase instance = KairoBase.GetInstance();
			if (instance != null)
			{
				instance.Finish();
			}
			ClockTime.Destroy();
		}

		public static string GetProperty(string key)
		{
			if (key.Equals("microedition.platform"))
			{
				key = "Platform";
			}
			if (key.Equals("Platform"))
			{
				return platform_;
			}
			return JSystem.GetProperty(key);
		}

		public float GetScaleRatio(bool forceAuto = false)
		{
			if (forceAuto || float.IsNaN(fixedScaleRatio_))
			{
				return KairoPlugin.GetScaleRatio(240, 240);
			}
			return fixedScaleRatio_ * GetResolutionScaleRatio() / 100f;
		}

		public void SetScaleRatio(float scaleRatio = float.NaN)
		{
			fixedScaleRatio_ = scaleRatio;
		}

		public int GetBattery()
		{
			return KairoPlugin.GetBatteryInfo()[1];
		}

		public bool IsBatteryCharging()
		{
			return KairoPlugin.GetBatteryInfo()[0] == 2;
		}

		public bool IsTermination()
		{
			return termination_;
		}

		public bool Visibe()
		{
			return visible_;
		}

		public bool IsSide()
		{
			if (Screen.orientation != ScreenOrientation.LandscapeLeft && Screen.orientation != ScreenOrientation.LandscapeLeft)
			{
				return Screen.orientation == ScreenOrientation.LandscapeRight;
			}
			return true;
		}

		public int GetWidth()
		{
			return KairoPlugin.GetAppWidth();
		}

		public int GetHeight()
		{
			return KairoPlugin.GetAppHeight();
		}

		public int GetWidth2()
		{
			return (int)((float)KairoPlugin.GetAppWidth() * 100f / GetResolutionScaleRatio() + 0.01f);
		}

		public int GetHeight2()
		{
			return (int)((float)KairoPlugin.GetAppHeight() * 100f / GetResolutionScaleRatio() + 0.01f);
		}

		public int GetOrientation()
		{
			switch (Screen.orientation)
			{
			case ScreenOrientation.Portrait:
				return 0;
			case ScreenOrientation.LandscapeRight:
				return 1;
			case ScreenOrientation.PortraitUpsideDown:
				return 2;
			case ScreenOrientation.LandscapeLeft:
				return 3;
			default:
				return 0;
			}
		}

		public void SetOrientation(int orientation)
		{
			ScreenOrientation orientation2 = ScreenOrientation.AutoRotation;
			switch (orientation)
			{
			case 0:
				orientation2 = ScreenOrientation.Portrait;
				break;
			case 1:
			case 90:
				orientation2 = ScreenOrientation.LandscapeRight;
				break;
			case 2:
			case 180:
				orientation2 = ScreenOrientation.PortraitUpsideDown;
				break;
			case 3:
			case 270:
				orientation2 = ScreenOrientation.LandscapeLeft;
				break;
			}
			Screen.orientation = orientation2;
		}

		public bool IsAutoOrientation()
		{
			return Screen.autorotateToPortrait;
		}

		public void ResetOrientation()
		{
			Screen.orientation = defaultOrientation_;
		}

		public static void Sleep(int ms)
		{
			Canvas instance = Canvas.GetInstance();
			SurfaceManagerBase surfaceManager = instance.GetSurfaceManager();
			long num = JSystem.CurrentTimeMillis();
			while (0 <= JSystem.CurrentTimeMillis() - num && JSystem.CurrentTimeMillis() - num <= ms)
			{
				long num2 = JSystem.CurrentTimeMillis();
				try
				{
					surfaceManager?.AdjustSurfaceLayout();
				}
				catch (Exception)
				{
					instance.LockAndUnlock();
				}
				long num3 = 50 - (JSystem.CurrentTimeMillis() - num2);
				if (num3 > 50)
				{
					num3 = 50L;
				}
				if (num3 >= 0)
				{
					try
					{
						JThread.Sleep(num3);
					}
					catch (ThreadAbortException)
					{
					}
				}
				JThread.Yield();
			}
		}

		public bool Focus()
		{
			if (!Visibe())
			{
				return false;
			}
			if (!HasWindowFocus())
			{
				return false;
			}
			return true;
		}

		public bool HasWindowFocus()
		{
			return isFocus_;
		}

		public abstract FormManagerBase GetFormManager();

		public bool GetRotateCheck()
		{
			return false;
		}

		public void SetRotateCheck(bool flag)
		{
		}

		public bool IsTablet()
		{
			if (isTabletResult_ == -1)
			{
				isTabletResult_ = (KairoPlugin.IsTablet() ? 1 : 0);
			}
			return isTabletResult_ == 1;
		}

		public int GetSystemBarHeight()
		{
			if (getSystemBarHeightResult_ == -1)
			{
				getSystemBarHeightResult_ = KairoPlugin.GetSystemBarHeight();
			}
			return getSystemBarHeightResult_;
		}

		public string GetPackageName()
		{
			return KairoPlugin.GetPackageName();
		}

		public Handler GetHandler()
		{
			return handler_;
		}

		public bool RunOnUpdate(ThreadStart start)
		{
			if (update_)
			{
				try
				{
					start();
				}
				catch (Exception ex)
				{
					throw ex;
				}
				return true;
			}
			MethodTask item = new MethodTask(start);
			updateMethodTasks_.Add(item);
			return false;
		}

		public void RunOnUiThread(ThreadStart start, bool wait = false)
		{
			if (CheckUiThread())
			{
				try
				{
					start();
					return;
				}
				catch (Exception ex)
				{
					if (wait)
					{
						throw ex;
					}
					return;
				}
			}
			{
				MethodTask methodTask = new MethodTask(start);
				lock (lockUITask_)
				{
					uiMethodTasks_.Add(methodTask);
				}
				if (wait)
				{
					while (!methodTask.finished)
					{
						Thread.Sleep(10);
					}
					if (methodTask.error != null)
					{
						throw methodTask.error;
					}
				}
			}
		}

		public object RunOnUiThread(UIMethod start, bool wait = false)
		{
			if (CheckUiThread())
			{
				try
				{
					return start();
				}
				catch (Exception ex)
				{
					if (Config.PRINT)
					{
						Log.Info(string.Concat("RunOnUiThread:", start, ":", ex));
					}
					if (wait)
					{
						throw ex;
					}
				}
			}
			else
			{
				MethodTask methodTask = new MethodTask(start);
				lock (lockUITask_)
				{
					uiMethodTasks_.Add(methodTask);
				}
				if (wait)
				{
					while (!methodTask.finished)
					{
						Thread.Sleep(10);
					}
					if (methodTask.error != null)
					{
						throw methodTask.error;
					}
					return methodTask.result;
				}
			}
			return null;
		}

		public bool CheckUiThread()
		{
			if (!created_)
			{
				Log.Info("アプリケーションの初期化が完了していません。MyConfigでstatic変数の初期値に関数を指定してませんか？");
			}
			return JThread.IsCurrentThread(uiThread_);
		}

		private void ExecuteUIMethods()
		{
			lock (lockUITask_)
			{
				while (uiMethodTasks_.Count > 0)
				{
					MethodTask methodTask = uiMethodTasks_[0];
					uiMethodTasks_.RemoveAt(0);
					if (methodTask == null)
					{
						continue;
					}
					try
					{
						if (methodTask.method1 != null)
						{
							methodTask.result = methodTask.method1();
						}
						else
						{
							methodTask.method2();
						}
					}
					catch (Exception ex)
					{
						if (Config.PRINT)
						{
							Log.Info("ExecuteUIMethods:" + ((methodTask.method1 != null) ? methodTask.method1.ToString() : methodTask.method2.ToString()) + ":" + ex);
						}
						methodTask.error = ex;
					}
					methodTask.finished = true;
				}
			}
		}

		private void ExecuteUpdateMethods()
		{
			while (updateMethodTasks_.Count > 0)
			{
				MethodTask methodTask = updateMethodTasks_[0];
				updateMethodTasks_.RemoveAt(0);
				if (methodTask == null)
				{
					continue;
				}
				try
				{
					if (methodTask.method1 != null)
					{
						methodTask.result = methodTask.method1();
					}
					else
					{
						methodTask.method2();
					}
				}
				catch (Exception ex)
				{
					if (Config.PRINT)
					{
						Log.Info("ExecuteUpdateMethods:" + ((methodTask.method1 != null) ? methodTask.method1.ToString() : methodTask.method2.ToString()) + ":" + ex);
					}
					methodTask.error = ex;
					throw ex;
				}
				finally
				{
					methodTask.finished = true;
				}
			}
		}

		private void Error(Exception e)
		{
			Dialog dialog = new Dialog(2, "ｴﾗｰ");
			dialog.SetText(string.Concat(e));
			dialog.Show();
			Terminate();
		}

		public int GetTotalFrames()
		{
			return totalFrames_;
		}

		public string GetDataPath()
		{
			return dataPath_;
		}

		public bool IsRendering()
		{
			if (!rendering_)
			{
				return Offscreen.IsRendering();
			}
			return true;
		}

		public void SetViewport(int width, int height)
		{
			viewportWidth_ = width;
			viewportHeight_ = height;
		}

		public int[] GetViewport()
		{
			GetViewport_result[0] = viewportWidth_;
			GetViewport_result[1] = viewportHeight_;
			return GetViewport_result;
		}

		public long GetRenderTime()
		{
			return renderTime_;
		}

		public long GetUpdateTime()
		{
			return updateTime_;
		}

		public void SetResolutionScaleRatio(float scaleRatio)
		{
			if (!float.IsNaN(this.resolutionScaleMax_) && this.resolutionScaleMax_ < scaleRatio)
			{
				scaleRatio = this.resolutionScaleMax_;
			}
			if (this.changeResolution_ && scaleRatio == this.changeResolutionScaleRatio_)
			{
				return;
			}
			if (!this.changeResolution_ && scaleRatio == this.resolutionScaleRatio_)
			{
				return;
			}
			float single = (float)(Screen.width * 100) / this.resolutionScaleRatio_;
			float single1 = (float)(Screen.height * 100) / this.resolutionScaleRatio_;
			this.changeResolution_ = true;
			this.changeResolutionWidth_ = (int)(single * scaleRatio / 100f + 0.01f);
			this.changeResolutionHeight_ = (int)(single1 * scaleRatio / 100f + 0.01f);
			this.changeResolutionScaleRatio_ = scaleRatio;
			this.RunOnUiThread(() => {
				Screen.SetResolution(this.changeResolutionWidth_, this.changeResolutionHeight_, Screen.fullScreen);
				this.UpdateResolution();
			}, true);
		}

		public float GetResolutionScaleRatio()
		{
			return resolutionScaleRatio_;
		}

		private void UpdateResolution()
		{
			KairoPlugin.UpdateAppSize();
			if (!this.changeResolution_)
			{
				return;
			}
			if (Screen.width == this.changeResolutionWidth_ && Screen.height == this.changeResolutionHeight_)
			{
				this.resolutionScaleRatio_ = this.changeResolutionScaleRatio_;
				DisplayMetrics.Update();
				KairoPlugin.ResetResolution();
				Camera.main.orthographicSize = (float)(Screen.height / 2);
				Camera.main.transform.position = new Vector3((float)(Screen.width / 2), (float)(Screen.height / 2), Camera.main.transform.position.z);
				this.resolutionChangedWait_ = 10;
				this.changeResolution_ = false;
			}
		}

		public float GetHalfResolutionScaleRatio()
		{
			int width = GetWidth2();
			int height = GetHeight2();
			int num = ((width < height) ? width : height);
			int num2 = num * 100 / 240 / 2;
			if (num2 % 100 > 0)
			{
				num2 += 100;
			}
			num2 /= 100;
			float num3 = 240f * (float)num2 * 100f / (float)num;
			if (num3 > 100f)
			{
				num3 = 100f;
			}
			return num3;
		}

		protected void OnPermit(string permissionName)
		{
			lastPermitPermissionName_ = permissionName;
			Dialog dialog = new Dialog(0, "確認");
			dialog.SetText("アプリを再起動してください。");
			dialog.Show();
			FormManagerBase formManager = GetFormManager();
			formManager.RemoveAllForms();
			if (formManager.GetFormsNum() == 0)
			{
				Terminate();
			}
		}

		protected void OnNotPermit(string permissionName)
		{
			lastNotPermitPermissionName_ = permissionName;
			FormManagerBase formManager = GetFormManager();
			formManager.RemoveAllForms();
			if (formManager.GetFormsNum() == 0)
			{
				Terminate();
			}
		}

		protected string GetLastPermitPermissionName()
		{
			return lastPermitPermissionName_;
		}

		protected string GetLastNotPermitPermissionName()
		{
			return lastNotPermitPermissionName_;
		}

		public ResourceManager GetResource()
		{
			return resource_;
		}
	}
}
