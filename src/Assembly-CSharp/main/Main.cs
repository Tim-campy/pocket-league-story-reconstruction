using System;
using analytics;
using form;
using java.lang;
using kairo.common.cfg;
using kairo.unity.form;
using kairo.unity.io;
using kairo.unity.ui;
using kairo.unity.util;
using native;
using UnityEngine;

namespace main
{
	public class Main : IApplication
	{
		public static SoundBase jingle_;

		public static int jingleWait_;

		public Main()
		{
			Init(this);
		}

		public static Main GetInstance()
		{
			return (Main)IApplication.GetCurrentApp();
		}

		protected override void OnCreate()
		{
			base.OnCreate();
			AppPlugin.Init(Language.Get(), Config.PRINT, Config.BASE64_PUBLIC_KEY);
			SetViewport(240, 280);
			AppData instance = AppData.GetInstance();
			kairo.unity.ui.Canvas instance2 = kairo.unity.ui.Canvas.GetInstance();
			instance2.SetBackground(kairo.unity.ui.Graphics.GetColorOfRGB(0, 0, 0));
			FormManager instance3 = FormManager.GetInstance();
			instance2.SetDisplayCurrent(this);
			instance3.ChangeCurrentForm(instance.frmBoot_);
			JarInflater jarInflater = null;
			try
			{
				string[] array = null;
				string[][] array2 = null;
				jarInflater = new JarInflater(RecordStore.ReadRecord(1, 6));
				array = StringUtil.LoadStringArray(jarInflater, "softkey.txt");
				if (array != null)
				{
					array2 = new string[array.Length][];
					for (int i = 0; i < array2.Length; i++)
					{
						array2[i] = StringUtil.Split(array[i], "\t", false, false, false);
					}
					Language.SetSoftLabelTable(array2);
				}
				array = StringUtil.LoadStringArray(jarInflater, "text.txt");
				if (array != null)
				{
					array2 = new string[array.Length][];
					for (int j = 0; j < array2.Length; j++)
					{
						array[j] = StringUtil.Replace(array[j], "\\n", "\n");
						array2[j] = StringUtil.Split(array[j], "\t", false, false, false);
					}
					Language.SetTextTable(array2);
				}
			}
			catch (Exception t)
			{
				if (Config.PRINT)
				{
					Log.Info(t);
				}
			}
			finally
			{
				if (jarInflater != null)
				{
					jarInflater.Close();
				}
			}
		}

		protected override void OnUpdate()
		{
			AppData instance = AppData.GetInstance();
			kairo.unity.ui.Canvas instance2 = kairo.unity.ui.Canvas.GetInstance();
			FormManager instance3 = FormManager.GetInstance();
			try
			{
				instance3.SetTargetFps(20);
				instance3.Execute();
				if (jingle_ != null && (jingle_.GetState() == 3 || jingle_.GetState() == 0) && jingleWait_-- <= 0)
				{
					SetJingle(null);
				}
				if (Config.FPS_UNLIMITED)
				{
					instance3.SetTargetFps(120);
				}
				if (Config.DEBUG_FAST && instance2.CheckKeyState(16))
				{
					instance3.SetTargetFps(400);
				}
				if (Config.DEBUG_SLOW && instance2.CheckKeyState(1024))
				{
					instance3.SetTargetFps(2);
				}
				QualitySettings.vSyncCount = 0;
				Application.targetFrameRate = instance3.GetTargetFps();
			}
			catch (NotPermitException ex)
			{
				Dialog dialog = new Dialog(0, "確認");
				dialog.SetText(Language.LT("起動できません。", string.Empty) + "<br>" + ex.DisplayMessage);
				dialog.Show();
				instance3.RemoveAllForms();
				if (instance3.GetFormsNum() == 0)
				{
					Terminate();
				}
			}
			catch (Exception ex2)
			{
				if (Config.PRINT)
				{
					Log.Info(ex2);
				}
				bool flag = false;
				if (instance.save_ == null)
				{
					Dialog dialog2 = new Dialog(0, "ｴﾗｰ");
					dialog2.SetText("エラーが発生しました。" + ((!Config.DEBUG) ? string.Empty : ("\n" + ex2)));
					dialog2.Show();
					if ((instance2.GetKeypadState() & 0x800) != 0)
					{
						flag = true;
					}
				}
				else
				{
					string text = string.Empty;
					for (int i = 0; i < instance3.GetFormsNum(); i++)
					{
						text = string.Concat(text, instance3.GetForm(i), "\n");
					}
					instance.sysSave_.strs[10] += ex2.ToString() + ":" + text;
					if (instance.sysSave_.strs[10].Length() > 1024)
					{
						instance.sysSave_.strs[10] = instance.sysSave_.strs[10].SubstringJ(instance.sysSave_.strs[10].Length() - 1024);
					}
					try
					{
						instance.SaveSystem();
					}
					catch (Exception)
					{
					}
					Dialog dialog3 = new Dialog(0, "ｴﾗｰ");
					dialog3.SetText("エラーが発生しました。" + ((!Config.DEBUG) ? string.Empty : ("\n" + ex2)));
					dialog3.Show();
					if ((instance2.GetKeypadState() & 0x800) != 0)
					{
						flag = true;
					}
				}
				if (flag)
				{
					instance3.RemoveAllForms();
					instance3.Push(new SpForm(ex2));
				}
				else
				{
					instance3.RemoveAllForms();
				}
				if (instance3.GetFormsNum() == 0)
				{
					Terminate();
				}
			}
		}

		protected override void OnDraw()
		{
			try
			{
				FormManager instance = FormManager.GetInstance();
				instance.OnDraw();
			}
			catch (Exception t)
			{
				Log.Info(t);
			}
		}

		protected override void OnResume()
		{
		}

		protected override void OnSuspend()
		{
			AnalyticsTracker.DispatchPageView();
		}

		protected override void OnDestroy()
		{
			AppPlugin.Finish();
		}

		public static void SetJingle(SoundBase sound, int wait = 20)
		{
			AppData instance = AppData.GetInstance();
			SoundPlayer instance2 = SoundPlayer.GetInstance();
			jingle_ = sound;
			jingleWait_ = wait;
			if (jingle_ == null)
			{
				instance2.SetVolume(0, instance.sysSave_.ints[3] * 51);
			}
			else
			{
				instance2.SetVolume(0, 0);
			}
		}

		public static SoundBase GetJingle()
		{
			return jingle_;
		}

		public override FormManagerBase GetFormManager()
		{
			return FormManager.GetInstance();
		}
	}
}
