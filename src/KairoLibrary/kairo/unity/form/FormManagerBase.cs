using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.IO;
using java.lang;
using java.util;
using kairo.common.cfg;
using kairo.unity.ad;
using kairo.unity.native;
using kairo.unity.panel;
using kairo.unity.surface;
using kairo.unity.system;
using kairo.unity.system.notification.ui;
using kairo.unity.ui;
using kairo.unity.util;
using UnityEngine;

namespace kairo.unity.form
{
	public class FormManagerBase : JRunnable
	{
		public delegate void OnCaptureCompleted(string[] files);

		internal const int MODE_INIT = 0;

		internal const int MODE_PREUPDATE = 1;

		internal const int MODE_UPDATE = 2;

		internal const int MODE_DRAW = 3;

		internal const int MODE_FINISH = 4;

		protected Vector formStack_ = new Vector();

		protected bool resume_;

		protected bool drawall_ = true;

		protected bool drawskip_;

		protected bool softlabel_ = true;

		protected FormBase loadingForm_;

		protected FormBase printForm_;

		protected JThread th_;

		protected FormBase currentForm_;

		protected bool showTerminationDialog_;

		protected bool termination_;

		protected bool pause_;

		protected bool pauseToast_;

		protected bool surfaceChanged_;

		protected long lastGcTime_;

		protected long execGcTime_;

		protected bool requestSynchronize_;

		protected bool synchronizing_;

		protected JThread thread_;

		protected bool serviceUpdate_;

		protected bool lastKeyguard_;

		protected bool lastVisibleActivity_ = true;

		protected bool notifiedExitEvent_;

		protected bool capturing_;

		protected int captureCount_;

		protected bool warning_;

		protected bool waitLateUpdate_;

		protected bool lockUpdate_;

		protected long lockUpdateTime_;

		protected long updateStartTime_;

		protected static bool initFontTexture_;

		protected List<KeyValuePair<FormBase, bool>> formIsGame_ = new List<KeyValuePair<FormBase, bool>>();

		public FormBase GetForm(int index)
		{
			return (FormBase)formStack_.ElementAt(index);
		}

		public int GetFormIndex(FormBase form)
		{
			return formStack_.IndexOf(form);
		}

		protected FormManagerBase()
		{
		}

		public bool ChangeCurrentForm(FormBase form)
		{
			if (termination_)
			{
				return true;
			}
			bool flag = false;
			if (thread_ != null && !JThread.IsCurrentThread(thread_))
			{
				flag = true;
			}
			if (flag)
			{
				BeginSynchronize();
			}
			try
			{
				int num = formStack_.IndexOf(form);
				if (num != -1)
				{
					formStack_.RemoveElementAt(num);
				}
				for (int i = 0; i < formStack_.Size(); i++)
				{
					((FormBase)formStack_.ElementAt(i)).formProcMode_ = 4;
				}
				if (num == -1)
				{
					Push(form);
				}
				else
				{
					formStack_.AddElement(form);
					kairo.unity.ui.Canvas.GetInstance().SetSoftLabel(form.leftSoftLabel_, form.rightSoftLabel_, form.softLabel3_, form.softLabel4_);
				}
				int num2 = -1;
				foreach (KeyValuePair<FormBase, bool> item in formIsGame_)
				{
					if (item.Key == form)
					{
						num2 = (item.Value ? 1 : 0);
						break;
					}
				}
				KairoBase instance = KairoBase.GetInstance();
				if (instance != null && num2 != -1)
				{
					instance.SetGameState(num2 == 1);
				}
				else if (num2 == -1)
				{
					bool value = instance != null && instance.IsGamePlaying();
					formIsGame_.Add(new KeyValuePair<FormBase, bool>(form, value));
				}
				return true;
			}
			finally
			{
				if (flag)
				{
					EndSynchronize();
				}
			}
		}

		public bool Push(FormBase form)
		{
			return Push(null, form);
		}

		public bool Push(FormBase position, FormBase form)
		{
			return Push(position, form, false);
		}

		public bool Push(FormBase position, FormBase form, bool before)
		{
			if (termination_)
			{
				return true;
			}
			bool flag = false;
			if (thread_ != null && !JThread.IsCurrentThread(thread_))
			{
				flag = true;
			}
			if (flag)
			{
				BeginSynchronize();
			}
			try
			{
				int num = formStack_.IndexOf(form);
				if (num != -1)
				{
					formStack_.RemoveElementAt(num);
				}
				int num2 = -1;
				if (position != null && position.formProcMode_ != 0 && position.formProcMode_ != 4)
				{
					num2 = formStack_.IndexOf(position);
				}
				if (num == -1)
				{
					form.formProcMode_ = 0;
				}
				if (before)
				{
					if (num2 == -1)
					{
						num2 = 0;
					}
					formStack_.InsertElementAt(form, num2);
				}
				else if (num2 != -1)
				{
					formStack_.InsertElementAt(form, num2 + 1);
				}
				else
				{
					formStack_.AddElement(form);
				}
				DrawAllForms();
				if (formStack_.Size() == 1 || ((FormBase)formStack_.FirstElement()).formProcMode_ == 4)
				{
					kairo.unity.ui.Canvas.GetInstance().SetSoftLabel("", "");
				}
				FormBase topForm = GetTopForm();
				kairo.unity.ui.Canvas.GetInstance().SetSoftLabel(topForm.leftSoftLabel_, topForm.rightSoftLabel_, topForm.softLabel3_, topForm.softLabel4_);
				form.formManager_ = this;
			}
			finally
			{
				if (flag)
				{
					EndSynchronize();
				}
			}
			return true;
		}

		public bool Pop()
		{
			return Pop(null);
		}

		public bool Pop(FormBase form)
		{
			if (formStack_.Size() == 0)
			{
				return true;
			}
			bool flag = false;
			if (thread_ != null && !JThread.IsCurrentThread(thread_))
			{
				flag = true;
			}
			if (flag)
			{
				BeginSynchronize();
			}
			try
			{
				if (form == null)
				{
					form = (FormBase)formStack_.LastElement();
				}
				form.formProcMode_ = 4;
				DrawAllForms();
				if (formStack_.Size() > 1)
				{
					kairo.unity.ui.Canvas instance = kairo.unity.ui.Canvas.GetInstance();
					for (int num = formStack_.Size() - 1; num >= 0; num--)
					{
						form = (FormBase)formStack_.ElementAt(num);
						if (form.formProcMode_ != 4)
						{
							instance.SetSoftLabel(form.leftSoftLabel_, form.rightSoftLabel_, form.softLabel3_, form.softLabel4_);
							if (form.formProcMode_ == 3)
							{
								form.formProcMode_ = 1;
							}
							break;
						}
					}
				}
			}
			finally
			{
				if (flag)
				{
					EndSynchronize();
				}
			}
			return true;
		}

		private void ExecuteInitAndFinish()
		{
			kairo.unity.ui.Canvas instance = kairo.unity.ui.Canvas.GetInstance();
			IApplication.GetCurrentApp();
			SurfaceManagerBase surfaceManager = instance.GetSurfaceManager();
			for (int i = this.formStack_.Size() - 1; i >= 0; i--)
			{
				FormBase formBase = (FormBase)this.formStack_.ElementAt(i);
				if (formBase.formProcMode_ == 4)
				{
					try
					{
						this.currentForm_ = formBase;
						if (formBase.Finish())
						{
							if (formBase.formProcMode_ == 4)
							{
								this.formStack_.RemoveElementAt(i);
								goto Label0;
							}
						}
						else if (!this.warning_)
						{
							this.warning_ = true;
							KairoPlugin.ShowToast(string.Concat(formBase, " の Finish() で false が返されました。"), 0);
						}
					}
					finally
					{
						this.currentForm_ = null;
					}
				Label0:
					if (surfaceManager != null)
					{
						Vector<SurfaceBase> surfaceList = surfaceManager.GetSurfaceList();
						for (int j = 0; j < surfaceList.Size(); j++)
						{
							SurfaceBase surfaceBase = surfaceList.ElementAt(j);
							if (surfaceBase.IsGameSurface())
							{
								surfaceBase.ClearTouchComponent();
							}
						}
					}
					instance.ClearKeyBuffer();
				}
			}
			for (int k = 0; k < this.formStack_.Size(); k++)
			{
				FormBase formBase1 = (FormBase)this.formStack_.ElementAt(k);
				if (formBase1.formProcMode_ == 0)
				{
					try
					{
						this.currentForm_ = formBase1;
						if (formBase1.Init())
						{
							if (formBase1.formProcMode_ == 0)
							{
								formBase1.formProcMode_ = 1;
								goto Label1;
							}
						}
						else if (!this.warning_)
						{
							this.warning_ = true;
							KairoPlugin.ShowToast(string.Concat(formBase1, " の Init() で false が返されました。"), 0);
						}
					}
					finally
					{
						this.currentForm_ = null;
					}
				Label1:
					if (surfaceManager != null)
					{
						Vector<SurfaceBase> surfaceBases = surfaceManager.GetSurfaceList();
						for (int l = 0; l < surfaceBases.Size(); l++)
						{
							SurfaceBase surfaceBase1 = surfaceBases.ElementAt(l);
							if (surfaceBase1.IsGameSurface())
							{
								surfaceBase1.ClearTouchComponent();
							}
						}
					}
					instance.ClearKeyBuffer();
				}
			}
		}

		public bool Execute()
		{
			while (!_execute())
			{
			}
			return true;
		}

		private bool _execute()
		{
			KairoBase instance = KairoBase.GetInstance();
			try
			{
				thread_ = JThread.CurrentThread();
				kairo.unity.ui.Canvas instance2 = kairo.unity.ui.Canvas.GetInstance();
				IApplication currentApp = IApplication.GetCurrentApp();
				if (formStack_.Size() == 0)
				{
					return true;
				}
				if (currentApp.IsTermination())
				{
					formStack_.RemoveAllElements();
					return true;
				}
				SoundPlayer instance3 = SoundPlayer.GetInstance();
				SaveRecorder instance4 = SaveRecorder.GetInstance();
				bool flag = false;
				bool flag2 = currentApp.Visibe();
				if (instance4 != null && ((flag != lastKeyguard_) & flag)) {
					ProcessEvent(AppEventType.Sleep);
				}
				lastKeyguard_ = flag;
				lastVisibleActivity_ = flag2;
				if (flag || !flag2)
				{
					instance3.Suspend();
					IApplication.Sleep(100);
					return true;
				}
				instance3.Resume();
				SurfaceManagerBase surfaceManager = instance2.GetSurfaceManager();
				NotifyManager recoveredNotifyManager = NotifyManager.GetInstance();
				if ((object)recoveredNotifyManager != null)
					recoveredNotifyManager.UpdateValues();
				while (lockUpdate_)
				{
					if (lockUpdateTime_ == 0L)
					{
						lockUpdateTime_ = JSystem.CurrentTimeMillis();
					}
					JThread.Sleep(1L);
				}
				if (updateStartTime_ == JSystem.CurrentTimeMillis())
				{
					updateStartTime_++;
				}
				else
				{
					updateStartTime_ = JSystem.CurrentTimeMillis();
				}
				if ((object)instance4 != null) { instance4.Lock(); }
				if (resume_)
				{
					FormBase formBase = (FormBase)formStack_.LastElement();
					try
					{
						currentForm_ = formBase;
						if (formBase.Resume())
						{
							resume_ = false;
						}
						else if (!warning_)
						{
							warning_ = true;
							KairoPlugin.ShowToast(string.Concat(formBase, " の Resume() で false が返されました。"));
						}
					}
					finally
					{
						currentForm_ = null;
					}
				}
				ExecuteInitAndFinish();
				if (loadingForm_ != null)
				{
					SetLoadingForm(null);
				}
				if (currentApp.IsTermination())
				{
					formStack_.RemoveAllElements();
					return true;
				}
				if (showTerminationDialog_)
				{
					showTerminationDialog_ = false;
					if (DoTerminationDialog())
					{
						ProcessEvent(AppEventType.Exit);
						termination_ = true;
						RemoveAllForms();
						return true;
					}
				}
				if (surfaceManager != null)
				{
					bool flag3 = true;
					if (formStack_.Size() > 0 && ((FormBase)formStack_.LastElement()).formProcMode_ != 1)
					{
						flag3 = false;
					}
					if (flag3)
					{
						surfaceManager.DoEvents();
					}
				}
				ExecuteInitAndFinish();
				if (pause_)
				{
					PreUpdate();
				}
				else
				{
					if (!serviceUpdate_)
					{
						serviceUpdate_ = true;
						FormBase topForm = GetTopForm();
						instance?.Update();
						if (topForm != GetTopForm())
						{
							return false;
						}
					}
					for (int i = 0; i < formStack_.Size() - 1; i++)
					{
						((FormBase)formStack_.ElementAt(i)).continuousUpdate_ = 0;
					}
					if (FepPanel.Current != null && FepPanel.Current.Update())
					{
						ExecuteInitAndFinish();
					}
					if (formStack_.Size() > 0)
					{
						FormBase formBase2 = (FormBase)formStack_.LastElement();
						if (formBase2.formProcMode_ == 1)
						{
							PreUpdate();
							formBase2.formProcMode_ = 2;
						}
						if (formBase2.formProcMode_ == 2)
						{
							try
							{
								currentForm_ = formBase2;
								if ((FepPanel.Current != null || formBase2.Update()) && formBase2.formProcMode_ == 2)
								{
									formBase2.formProcMode_ = 3;
								}
							}
							finally
							{
								currentForm_ = null;
							}
							formBase2.continuousUpdate_++;
							return false;
						}
					}
				}
				if ((object)instance4 != null) { instance4.Unlock(true); }
				waitLateUpdate_ = false;
				Updated();
				serviceUpdate_ = false;
				while (requestSynchronize_)
				{
					synchronizing_ = true;
					while (requestSynchronize_)
					{
						try
						{
							JThread.Sleep(1L);
						}
						catch (Exception)
						{
						}
					}
					synchronizing_ = false;
					try
					{
						JThread.Yield();
						JThread.Sleep(10L);
					}
					catch (Exception)
					{
					}
				}
				if (currentApp.IsTermination())
				{
					formStack_.RemoveAllElements();
				}
				return true;
			}
			catch (Exception ex3)
			{
				if (instance != null && !instance.reserveSaveEnable)
				{
					instance.reserveSaveEnable = true;
				}
				throw ex3;
			}
			finally
			{
				if (instance != null && instance.reserveSaveEnable && instance.reserveSave != 0)
				{
					int reserveSave = instance.reserveSave;
					instance.reserveSave = 0;
					for (int j = 0; j < 3; j++)
					{
						try
						{
							if (reserveSave == 1) { instance.Save(); } else if (reserveSave == 2) { instance.SaveAll(); }
						}
						catch (Exception)
						{
							JSystem.Gc();
							JThread.Sleep(500L);
							continue;
						}
						break;
					}
				}
			}
		}

		public virtual void OnDraw()
		{
			kairo.unity.ui.Canvas instance = kairo.unity.ui.Canvas.GetInstance();
			SurfaceManagerBase surfaceManager = instance.GetSurfaceManager();
			if (formStack_.Size() == 0)
			{
				return;
			}
			PreDraw();
			if (Config.COUNT_ENABLE)
			{
				Config.drawImageCount_ = 0;
				Config.drawStringCount_ = 0;
				Config.drawShapeCount_ = 0;
				Config.drawImageArea_ = 0;
				Config.drawSourceImageArea_ = 0;
			}
			Config.checkCount_ = (Config.checkCount_ + 1) % int.MaxValue;
			if (surfaceManager != null)
			{
				surfaceManager.ClearTouchComponent();
			}
			if (!drawskip_)
			{
				surfaceManager?.AdjustSurfaceLayout();
				kairo.unity.ui.Graphics graphics = instance.BeginPaint();
				if (graphics != null)
				{
					if (surfaceChanged_)
					{
						DrawAllForms();
						surfaceChanged_ = false;
					}
					if (surfaceManager != null)
					{
						surfaceManager.BeginTouchRegist(graphics);
					}
					surfaceManager?.Draw(graphics);
					if (printForm_ != null)
					{
						DrawPrintForm(graphics);
					}
					if (surfaceManager != null)
					{
						surfaceManager.EndTouchRegist(graphics);
					}
					surfaceManager?.DrawComponents(graphics);
					InterstitialAd.DrawLoading(graphics);
					instance.EndPaint();
					Image.UnuseExpirationImages();
				}
			}
			if (formStack_.Size() > 0)
			{
				FormBase formBase = (FormBase)formStack_.LastElement();
				if (formBase.formProcMode_ != 0 && formBase.formProcMode_ != 4)
				{
					formBase.formProcMode_ = 1;
				}
			}
			drawskip_ = false;
			Drawn();
			ResourceManager.CloseJarCache();
			if (IApplication.GetCurrentApp().IsTermination())
			{
				formStack_.RemoveAllElements();
			}
		}

		protected virtual bool DoTerminationDialog()
		{
			Dialog dialog = new Dialog(3, "確認");
			dialog.SetText("ゲームを終了します");
			if (dialog.Show() == 4)
			{
				return true;
			}
			return false;
		}

		public virtual void Resume()
		{
			resume_ = true;
			kairo.unity.ui.Canvas instance = kairo.unity.ui.Canvas.GetInstance();
			SurfaceManagerBase surfaceManager = instance.GetSurfaceManager();
			if (surfaceManager != null)
			{
				surfaceManager.ClearTouchComponent();
			}
			instance.ClearKeyBuffer();
		}

		public void DrawAllForms()
		{
			DrawAllForms(false);
		}

		public void DrawAllForms(bool flush)
		{
			drawall_ = true;
		}

		public void DrawSkip()
		{
			drawskip_ = true;
		}

		public bool IsDrawSkip()
		{
			return drawskip_;
		}

		protected virtual void PreUpdate()
		{
			kairo.unity.ui.Canvas.GetInstance().DecideKeyState();
		}

		protected virtual void Updated()
		{
			kairo.unity.ui.Canvas instance = kairo.unity.ui.Canvas.GetInstance();
			if (softlabel_)
			{
				instance.UpdateSoftLabel();
			}
			softlabel_ = true;
			kairo.unity.ui.Canvas.BackLight(true);
		}

		protected virtual void PreDraw()
		{
		}

		protected virtual void Drawn()
		{
		}

		public int GetFormsNum()
		{
			return formStack_.Size();
		}

		public FormBase GetTopForm()
		{
			if (formStack_.Size() == 0)
			{
				return null;
			}
			return (FormBase)formStack_.LastElement();
		}

		public FormBase GetDownForm()
		{
			if (formStack_.Size() == 0)
			{
				return null;
			}
			return (FormBase)formStack_.FirstElement();
		}

		public bool RemoveAllForms()
		{
			ProcessEvent(AppEventType.Exit);
			for (int i = 0; i < formStack_.Size(); i++)
			{
				((FormBase)formStack_.ElementAt(i)).formProcMode_ = 4;
			}
			return true;
		}

		public FormBase GetCurrentForm()
		{
			if (currentForm_ == null)
			{
				return GetTopForm();
			}
			return currentForm_;
		}

		public void MoveToTop(FormBase form)
		{
			formStack_.RemoveElement(form);
			formStack_.AddElement(form);
			kairo.unity.ui.Canvas.GetInstance().SetSoftLabel(form.leftSoftLabel_, form.rightSoftLabel_, form.softLabel3_, form.softLabel4_);
			DrawAllForms();
			softlabel_ = false;
		}

		public void MoveToDown(FormBase form)
		{
			formStack_.RemoveElement(form);
			formStack_.InsertElementAt(form, 0);
			kairo.unity.ui.Canvas instance = kairo.unity.ui.Canvas.GetInstance();
			form = (FormBase)formStack_.LastElement();
			instance.SetSoftLabel(form.leftSoftLabel_, form.rightSoftLabel_, form.softLabel3_, form.softLabel4_);
			DrawAllForms();
			softlabel_ = false;
		}

		public void SetLoadingForm(FormBase form)
		{
			if (th_ == null && loadingForm_ != null)
			{
				try
				{
					loadingForm_.Finish();
				}
				catch (Exception)
				{
				}
			}
			loadingForm_ = form;
			if (loadingForm_ == null)
			{
				return;
			}
			try
			{
				loadingForm_.Init();
			}
			catch (Exception)
			{
			}
		}

		public void SetPrintForm(FormBase form)
		{
			if (printForm_ != null)
			{
				printForm_.Finish();
			}
			printForm_ = form;
			if (printForm_ != null)
			{
				printForm_.Init();
			}
		}

		public void StartLoading()
		{
			th_ = new JThread(this);
			th_.Start();
		}

		public void Run()
		{
			FormBase formBase = loadingForm_;
			while (loadingForm_ != null)
			{
				try
				{
					kairo.unity.ui.Canvas instance = kairo.unity.ui.Canvas.GetInstance();
					kairo.unity.ui.Graphics g = instance.BeginPaint();
					loadingForm_.Draw(g);
					if (printForm_ != null)
					{
						DrawPrintForm(g);
					}
					instance.EndPaint();
					JThread.Sleep(50L);
				}
				catch (Exception)
				{
				}
			}
			try
			{
				formBase?.Finish();
			}
			catch (Exception)
			{
			}
			th_ = null;
		}

		private void DrawPrintForm(kairo.unity.ui.Graphics g)
		{
			g.SetOrigin(0, 0);
			g.ClearClip();
			try
			{
				printForm_.Draw(g);
			}
			catch (Exception)
			{
			}
		}

		public bool CheckUsable(FormBase form)
		{
			return form.formProcMode_ != 4;
		}

		public void Draw(kairo.unity.ui.Graphics g)
		{
			try
			{
				for (int i = 0; i < formStack_.Size(); i++)
				{
					FormBase formBase = (FormBase)formStack_.ElementAt(i);
					if (!drawall_ && i != formStack_.Size() - 1 && !formBase.CheckMainWindow())
					{
						continue;
					}
					try
					{
						currentForm_ = formBase;
						if (formBase.formProcMode_ != 0 && formBase.formProcMode_ != 4)
						{
							bool num = SurfaceManagerBase.IsAutoGLRender();
							if (num)
							{
								g.BeginGLRender();
							}
							formBase.Draw(g);
							if (num)
							{
								g.EndGLRender();
							}
						}
					}
					finally
					{
						currentForm_ = null;
					}
				}
			}
			catch (Exception t)
			{
				if (Config.PRINT)
				{
					Log.Info(t);
				}
			}
			drawall_ = false;
		}

		public void ShowTerminationDialog()
		{
			showTerminationDialog_ = true;
		}

		public bool CheckShowTerminationDialog()
		{
			return showTerminationDialog_;
		}

		public void SetPause(bool b)
		{
			pause_ = b;
			if (pause_ && !pauseToast_)
			{
				pauseToast_ = true;
			}
		}

		public bool IsPause()
		{
			return pause_;
		}

		public void SurfaceChanged()
		{
			surfaceChanged_ = true;
		}

		private void BeginSynchronize()
		{
			while (requestSynchronize_ || synchronizing_)
			{
				try
				{
					JThread.Sleep(1L);
				}
				catch (Exception)
				{
				}
			}
			synchronizing_ = false;
			requestSynchronize_ = true;
			while (!synchronizing_)
			{
				try
				{
					JThread.Sleep(1L);
				}
				catch (Exception)
				{
				}
			}
		}

		private void EndSynchronize()
		{
			requestSynchronize_ = false;
		}

		internal void ProcessEvent(AppEvent evt)
		{
			if (evt.Type == AppEventType.Exit)
			{
				if (notifiedExitEvent_)
				{
					return;
				}
				notifiedExitEvent_ = true;
			}
			int num = formStack_.Size() - 1;
			while (num >= 0)
			{
				FormBase formBase = (FormBase)formStack_.ElementAt(num);
				if (formBase == null || !formBase.IsAlive() || !formBase.ProcessEvent(evt))
				{
					num--;
					continue;
				}
				break;
			}
		}

		internal void ProcessBackKey()
		{
			for (int num = formStack_.Size() - 1; num >= 0; num--)
			{
				FormBase formBase = (FormBase)formStack_.ElementAt(num);
				if (formBase != null && formBase.IsAlive() && formBase.ProcessBackKey())
				{
					return;
				}
			}
			ShowTerminationDialog();
		}

		internal static void ProcessEvent(AppEventType type, params object[] args)
		{
			IApplication currentApp = IApplication.GetCurrentApp();
			if (currentApp == null)
			{
				return;
			}
			FormManagerBase formManager = currentApp.GetFormManager();
			if (formManager == null)
			{
				return;
			}
			formManager.ProcessEvent(new AppEvent(type, args));
		}

		public void Capture(SurfaceBase view, string[] captureTables = null, OnCaptureCompleted method = null, bool saveToTmpDirectory = false)
		{
			IApplication app = IApplication.GetCurrentApp();
			kairo.unity.ui.Canvas canvas = kairo.unity.ui.Canvas.GetInstance();
			bool isPause = IsPause();
			if (!isPause)
			{
				SetPause(true);
			}
			if (captureTables == null)
			{
				captureTables = new string[6] { "768:1024,_768_1024_ipad", "640:960,_640_960_35inch", "640:1136,_640_1136_40inch", "750:1334,_750_1334_47inch", "1242:2208,_1242_2208_55inch", "2048:2732,_2048_2732_129inch" };
			}
			bool bkFullscreen = canvas.IsFullscreen();
			canvas.Fullscreen(true);
			capturing_ = true;
			int stopFrame = IApplication.GetCurrentApp().GetTotalFrames();
			JThread captureThread = new JThread(delegate
			{
				try
				{
					List<string> saveFiles = new List<string>();
					for (int i = 0; i < captureTables.Length; i++)
					{
						string text = captureTables[i];
						string path = "temporary_screenshot" + captureCount_ + ".png";
						if (Config.UNITY_EDITOR)
						{
							path = app.GetDataPath() + "/" + path;
						}
						captureCount_++;
						string[] info = text.Trim().Split(',');
						for (int j = 0; j < info.Length; j++)
						{
							info[j] = info[j].Trim();
						}
						int width = 0;
						int height = 0;
						if (info.Length != 0 && info[0].Length > 0)
						{
							string[] array = info[0].Split(':');
							width = int.Parse(array[0]);
							height = int.Parse(array[1]);
						}
						while (true)
						{
							bool completed = (bool)IApplication.GetCurrentApp().RunOnUiThread(delegate
						{
							if (IApplication.GetCurrentApp().GetTotalFrames() - stopFrame < 10)
							{
								return false;
							}
							stopFrame = IApplication.GetCurrentApp().GetTotalFrames();
							if (width != 0)
							{
								canvas.SetAspectRatio(width, height);
							}
							return true;
						}, true);
							if (completed)
							{
								break;
							}
						}
						while (true)
						{
							bool completed = (bool)IApplication.GetCurrentApp().RunOnUiThread(delegate
						{
							if (IApplication.GetCurrentApp().GetTotalFrames() - stopFrame < 10)
							{
								return false;
							}
							stopFrame = IApplication.GetCurrentApp().GetTotalFrames();
							Application.CaptureScreenshot(path);
							return true;
						}, true);
							if (completed)
							{
								break;
							}
						}
						if (!Config.UNITY_EDITOR)
						{
							RuntimePlatform platform = Application.platform;
							if (platform == RuntimePlatform.IPhonePlayer || platform == RuntimePlatform.Android)
							{
								path = Application.persistentDataPath + "/" + path;
							}
						}
						bool flag = false;
						while (!File.Exists(path))
						{
							JThread.Sleep(10L);
						}
						if (!(bool)IApplication.GetCurrentApp().RunOnUiThread(delegate
						{
							byte[] data = File.ReadAllBytes(path);
							File.Delete(path);
							float scaleRatio = app.GetScaleRatio();
							Image image = Image.Load(data, Image.Format.ARGB32, Image.Filter.Trilinear);
							if (image.GetWidth() < 100 && image.GetHeight() < 100)
							{
								return false;
							}
							try
							{
								if (width == 0)
								{
									image.Resize((int)((float)(view.GetWidth() * 100) / scaleRatio + 0.1f), (int)((float)(view.GetHeight() * 100) / scaleRatio - 1f + 0.1f), view.GetX(), view.GetY(), view.GetWidth(), view.GetHeight() - 1);
								}
								else
								{
									int num = (int)((float)image.GetWidth() * (float)height / (float)width + 0.1f);
									if (num <= image.GetHeight())
									{
										image.Resize(width, height, 0, 0, image.GetWidth(), num);
									}
									else
									{
										Image image2 = Image.CreateImage(image.GetWidth(), num, Image.Format.ARGB32, Image.Filter.Trilinear);
										int[] array2 = new int[image2.GetWidth() * image2.GetHeight()];
										for (int k = 0; k < array2.Length; k++)
										{
											array2[k] = -16777216;
										}
										image.GetARGBPixels(0, 0, image.GetWidth(), image.GetHeight(), array2);
										image2.SetARGBPixels(0, 0, image2.GetWidth(), image2.GetHeight(), array2);
										image.Dispose();
										image = image2;
										image.Resize(width, height, 0, 0, image.GetWidth(), image.GetHeight());
									}
								}
								string text2 = "{date}";
								if (1 < info.Length && info[1].Length > 0)
								{
									text2 += info[1];
								}
								string save = null;
								if (saveToTmpDirectory)
								{
									KairoPlugin.SaveToTmpCaptureDirectory(text2 + ".png", ((Texture2D)image.GetImage()).EncodeToPNG(), out save);
								}
								else
								{
									KairoPlugin.SaveToSd(text2 + ".png", ((Texture2D)image.GetImage()).EncodeToPNG(), out save);
								}
								if (save != null)
								{
									saveFiles.Add(save);
								}
							}
							catch (Exception t)
							{
								Log.Info(t);
							}
							finally
							{
								if (image != null)
								{
									image.Dispose();
								}
								image = null;
							}
							return true;
						}, true))
						{
							i--;
						}
						stopFrame = IApplication.GetCurrentApp().GetTotalFrames();
					}
					if (method != null)
					{
						method(saveFiles.ToArray());
					}
				}
				catch (Exception ex)
				{
					KairoPlugin.ShowToast(string.Concat(ex));
				}
				finally
				{
					if (!isPause)
					{
						SetPause(false);
					}
					capturing_ = false;
					canvas.Fullscreen(bkFullscreen);
					canvas.SetAspectRatio();
				}
			});
			captureThread.Start();
		}

		public bool IsCapturing()
		{
			return capturing_;
		}

		internal void WaitLateUpdate()
		{
			if (IApplication.GetCurrentApp().CheckUiThread())
			{
				return;
			}
			this.waitLateUpdate_ = true;
			while (this.waitLateUpdate_)
			{
				JThread.Sleep((long)1);
			}
		}

		internal void LockUpdate()
		{
			lockUpdate_ = true;
			lockUpdateTime_ = 0L;
		}

		internal long UnlockUpdate()
		{
			lockUpdate_ = false;
			if (lockUpdateTime_ == 0L)
			{
				return 0L;
			}
			return JSystem.CurrentTimeMillis() - lockUpdateTime_;
		}

		internal long GetUpdateStartTime()
		{
			return updateStartTime_;
		}
	}
}
