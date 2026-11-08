using System;
using cfg;
using data;
using game;
using java.io;
using java.lang;
using java.util;
using kairo.common.cfg;
using kairo.unity.io;
using kairo.unity.surface;
using kairo.unity.ui;
using kairo.unity.util;
using main;
using surface;
using system.form;

namespace form
{
	public class TitleForm : MyFormBase
	{
		private const int RESTITLE_IMG_TITLE00 = 0;

		private const int RESTITLE_IMG_WINDOW = 1;

		private const int RESTITLE_IMG_LOGO = 2;

		private const int RESTITLE_IMG_KAIROAD = 3;

		private const int RESTITLE_IMG_KAIROAD_CANCEL = 4;

		private const int STATE_MENU = 0;

		private const int STATE_SAVESLOT = 1;

		private const int MENU_START = 0;

		private const int MENU_RANKING = 1;

		private const int MENU_RELEASE = 1;

		private const int MENU_END = 2;

		public const string CAPTION1 = "チーム名(全角７文字)";

		public const string CAPTION2 = "短縮名(全角４文字)";

		public const string DEF_NAME1 = "すこやかＦＣ";

		public const string DEF_NAME2 = "すこやか";

		private const int HOBJ_MAX = 20;

		private const int HOBJP_EXIST = 0;

		private const int HOBJP_PDATAID = 1;

		private const int HOBJP_PX = 2;

		private const int HOBJP_PY = 3;

		private const int HOBJP_DIRECT = 4;

		private const int HOBJP_FRAME = 5;

		private const int HOBJP_END = 6;

		private const int TITLE_INIT_PY_MIN = 228;

		private const int TITLE_INIT_PY_MAX = 238;

		private const int TITLE_MOVE_V = 1;

		public UserData udat_;

		public ResourceManager resTitle_;

		private static readonly int[] title_anime = new int[3] { 10, 53, 78 };

		private int state_;

		private static readonly int[][] softLabels_ = new int[2][]
		{
			new int[2] { 5, 1 },
			new int[2] { 0, 2 }
		};

		private int selMenu_;

		private int titleY_;

		private int titleFrame_;

		private int POUND_PRESS_CNT;

		private bool STAR;

		private int selectSaveType_;

		public int selectSave_;

		private int currentSave_;

		public string newGameName_;

		public int newGameFrame1_;

		public int newGameFrame2_;

		public static readonly int[][] IMG_SL = new int[2][]
		{
			new int[6] { 0, 0, 6, 3, 16, 16 },
			new int[6] { -4, -2, 2, 23, 25, 20 }
		};

		internal int frame_;

		internal int x_ = 60;

		internal int y_ = 60;

		internal long[] time_;

		private int[][] ta_data = JTool.MakeArray<int[][]>(new int[2] { 20, 6 });

		private static readonly int[] TITLE_GUEST = new int[10] { 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 };

		private int APPEAR_TIME;

		private int apeear_cnt;

		private int[][] zsort_mhobj = JTool.MakeArray<int[][]>(new int[2] { 20, 2 });

		private static string kairoAdUrl_jp = "market://details?id=net.kairosoft.android.soccer2_ja&referrer=utm_source%3Dappli%26utm_medium%3Dhouse_ad%26utm_content%3soccer_ja_title%26utm_campaign%3Dbanner";

		public int kairoAdClickNum_;

		public TitleForm()
			: base(1)
		{
		}

		public override string ToString()
		{
			return "TitleForm:" + state_;
		}

		public override bool Init()
		{
			base.Init();
			udat_ = UserData.GetInstance();
			canvas_.SetBackground(Graphics.GetColorOfRGB(255, 255, 255));
			canvas_.DecideKeyState();
			if (apdat_.frmTitle_.resTitle_ != null)
			{
				apdat_.frmTitle_.resTitle_.Dispose();
			}
			apdat_.frmTitle_.resTitle_ = new ResourceManager(null, null, null, null, null, null, null);
			apdat_.frmTitle_.resTitle_.Load(RecordStore.ReadRecord(1, 7));
			TeamData teamData = apdat_.tData_[0];
			newGameName_ = teamData.name_;
			ChangeState(0);
			titleY_ = 0;
			selMenu_ = 0;
			selectSave_ = apdat_.sysSave_.ints[8];
			currentSave_ = apdat_.sysSave_.ints[8];
			selectSaveType_ = 1;
			SoundPlayer instance = SoundPlayer.GetInstance();
			instance.SetVolume(0, apdat_.sysSave_.ints[3] * 51);
			instance.SetMute(0, apdat_.sysSave_.ints[3] == 0);
			instance.SetVolume(1, apdat_.sysSave_.ints[4] * 51);
			instance.SetMute(1, apdat_.sysSave_.ints[4] == 0);
			apdat_.PlayBgm(4);
			POUND_PRESS_CNT = 0;
			STAR = false;
			titleFrame_ = 0;
			app_.ResetOrientation();
			int num = 0;
			IApplication currentApp = IApplication.GetCurrentApp();
			if (currentApp.GetWidth() <= 480 || currentApp.GetHeight() <= 480)
			{
				num = 1;
			}
			canvas_.SetLinearFilterEnable(num == 1);
			if (!MyConfig.SMART_PASS && !MyConfig.AMAZON && !Config.TRIAL_VERSION && !Config.ADMOB_ENABLE)
			{
				try
				{
					LoadKiaroAdClickNum();
				}
				catch (Exception t)
				{
					Log.Info(t);
				}
			}
			return true;
		}

		public override bool Update()
		{
			base.Update();
			titleFrame_ = AppData.ClampMax(titleFrame_ + 1, 2147483646);
			frame_ = (frame_ + 1) % int.MaxValue;
			Update_titleAnime();
			int pOUND_PRESS_CNT = POUND_PRESS_CNT;
			if (canvas_.CheckKeyState(2048))
			{
				POUND_PRESS_CNT++;
				if (POUND_PRESS_CNT > 40)
				{
					STAR = true;
				}
			}
			else
			{
				POUND_PRESS_CNT = 0;
			}
			if (state_ == 0)
			{
				if (Config.DEBUG_CMD_DEV && POUND_PRESS_CNT == 0 && 0 < pOUND_PRESS_CNT && pOUND_PRESS_CNT < 20 && !Config.PTW)
				{
					apdat_.sysSave_.ints[1] = BitUtil.Off(apdat_.sysSave_.ints[1], 1);
					apdat_.sysSave_.ints[2] = 0;
					RecordStore recordStore = null;
					try
					{
						recordStore = RecordStore.OpenRecordStore(1);
						recordStore.RemoveAllRecord();
						apdat_.SaveSystem();
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
						if (recordStore != null)
						{
							try
							{
								recordStore.CloseRecordStore();
							}
							catch (Exception)
							{
							}
						}
						recordStore = null;
					}
					return formMan_.ChangeCurrentForm(apdat_.frmBoot_);
				}
				apdat_.resCom2_.seb[61].Frame();
				if (canvas_.CheckKeyRepeat(131072))
				{
					selMenu_ = (selMenu_ - 1 + 2) % 2;
				}
				if (canvas_.CheckKeyRepeat(524288))
				{
					selMenu_ = (selMenu_ + 1 + 2) % 2;
				}
				if (dialog_ != null)
				{
					if (dialog_.value1_ == 0 && dialog_.result_ == 0)
					{
						dialog_ = null;
						return formMan_.RemoveAllForms();
					}
					if (dialog_.value1_ == 1 && dialog_.result_ == 0)
					{
						dialog_ = null;
						Canvas.OpenBrowser(Config.URL_SITE);
					}
					dialog_ = null;
				}
				if (apdat_.IsPushSoftLabel(1))
				{
					string buttons = "はい\tいいえ";
					dialog_ = SubForm.MakeDialog("ゲームを終了します", buttons, 1);
					dialog_.value1_ = 0;
					return formMan_.Push(dialog_);
				}
				if (apdat_.IsPushSoftLabel(5))
				{
					Canvas.OpenBrowser(Config.URL_SITE);
					return true;
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					if (!Config.TRIAL_VERSION)
					{
						if (selMenu_ == 0)
						{
							ChangeState(1);
						}
						else if (selMenu_ == 1)
						{
							return Push(new RankForm());
						}
					}
					else if (selMenu_ == 0)
					{
						ChangeState(1);
					}
					else if (selMenu_ == 1)
					{
						string packageName = IApplication.GetCurrentApp().GetPackageName();
						packageName = packageName.SubstringJ(0, packageName.Length() - 1);
						Canvas.OpenBrowser("market://details?id=" + packageName + "&referrer=utm_source%3Dappli%26utm_medium%3Dhouse_ad%26utm_content%3D" + packageName + "_lite%26utm_campaign%3Dbanner");
					}
				}
				if (STAR && canvas_.CheckKeyPulse(2))
				{
					AppData.currentSaveSlot_ = selectSave_;
					apdat_.SendSaveData("送信SYSTEM:" + apdat_.sysSave_.strs[10], true);
				}
				else if (Config.DEBUG_CMD_DEV && !Config.PTW && canvas_.CheckKeyPulse(2))
				{
					RecordStore recordStore2 = null;
					string url = Config.URL_SP + "&ver=" + 202 + "&load=save.bin";
					try
					{
						byte[] data = Http.Connect(url, null, false);
						recordStore2 = RecordStore.OpenRecordStore(0);
						recordStore2.WriteRecord(1 + AppData.currentSaveSlot_, data);
						apdat_.sysSave_.ints[6 + AppData.currentSaveSlot_] = 300;
						apdat_.sysSave_.strs[4 + AppData.currentSaveSlot_] = "120";
						apdat_.sysSave_.strs[2 + AppData.currentSaveSlot_] = "[受信データ]";
						apdat_.SaveSystem();
						Dialog dialog = new Dialog(0, "完了");
						dialog.SetText("セーブデータの上書き完了");
						dialog.Show();
					}
					catch (Exception)
					{
						Dialog dialog2 = new Dialog(0, "エラー");
						dialog2.SetText("通信失敗");
						dialog2.Show();
					}
					finally
					{
						if (recordStore2 != null)
						{
							try
							{
								recordStore2.CloseRecordStore();
							}
							catch (Exception)
							{
							}
						}
						recordStore2 = null;
					}
				}
			}
			else if (state_ == 1)
			{
				apdat_.resCom2_.seb[61].Frame();
				int num = 2;
				if (canvas_.CheckKeyRepeat(65536))
				{
					selectSave_ = (selectSave_ - 1 + num) % num;
				}
				if (canvas_.CheckKeyRepeat(262144))
				{
					selectSave_ = (selectSave_ + 1 + num) % num;
				}
				if (canvas_.CheckKeyRepeat(131072))
				{
					selectSaveType_ = (selectSaveType_ - 1 + 2) % 2;
				}
				if (canvas_.CheckKeyRepeat(524288))
				{
					selectSaveType_ = (selectSaveType_ + 1 + 2) % 2;
				}
				if (selectSaveType_ == 0 && apdat_.sysSave_.ints[11 + selectSave_] == -1)
				{
					selectSaveType_ = 1;
				}
				if (dialog_ != null)
				{
					if (dialog_.result_ == 0)
					{
						ContinueGame();
					}
					else if (dialog_.result_ == 1)
					{
						NewGame();
					}
					else if (dialog_.result_ == 2)
					{
						apdat_.sysSave_.ints[((selectSaveType_ == 0) ? (11) : (6)) + selectSave_] = -1;
						try
						{
							apdat_.SaveSystem();
						}
						catch (Exception)
						{
						}
					}
					dialog_ = null;
					return false;
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					if (apdat_.sysSave_.ints[((selectSaveType_ == 0) ? (11) : (6)) + selectSave_] != -1)
					{
						if (selectSaveType_ == 0)
						{
							ContinueGame();
							return true;
						}
						dialog_ = new SubForm(22, 136, 97 + selectSaveType_ * 47);
						dialog_.value1_ = selectSave_;
						return Push(dialog_);
					}
					NewGame();
				}
				else if (apdat_.IsPushSoftLabel(2))
				{
					ChangeState(0);
				}
				else if (STAR && canvas_.CheckKeyPulse(2))
				{
					AppData.currentSaveSlot_ = selectSave_;
					apdat_.SendSaveData("送信SYSTEM:" + apdat_.sysSave_.strs[10], true);
				}
			}
			if (STAR && canvas_.CheckKeyPulse(1024))
			{
				Dialog dialog3 = new Dialog(3, "データの再ダウンロード");
				dialog3.SetText("データを再ダウンロードしますか？\n※セーブデータは消えません");
				if (dialog3.Show() == 4)
				{
					apdat_.ResourceReDownload();
					return formMan_.ChangeCurrentForm(apdat_.frmBoot_);
				}
			}
			return true;
		}

		private void ContinueGame()
		{
			if (apdat_.sysSave_.ints[((selectSaveType_ == 0) ? (11) : (6)) + selectSave_] != -1)
			{
				try
				{
					canvas_.SetSoftLabel(string.Empty, string.Empty);
					canvas_.UpdateSoftLabel();
					apdat_.LoadGame(selectSave_, selectSaveType_ == 0);
					formMan_.ChangeCurrentForm(apdat_.frmGame_);
				}
				catch (Exception t)
				{
					apdat_.save_ = null;
					if (Config.PRINT)
					{
						Log.Info(t);
					}
					formMan_.Push(SubForm.MakeDialog("ロードに失敗しました"));
				}
			}
else{			formMan_.Push(SubForm.MakeDialog("セーブデータがありません"));
}		}

		private void NewGame()
		{
			dialog_ = new SubForm(71);
			Push(dialog_);
		}

		public void SmartBeginPaint(Graphics g)
		{
			g.SetColor(Graphics.GetColorOfRGB(0, 0, 0));
			g.FillRect(0f, 0f, view_.GetGameWidth(), view_.GetGameHeight());
			g.SetOrigin((view_.GetGameWidth() - 240) / 2, 0);
		}

		public override void Draw(Graphics g)
		{
			try
			{
				_draw(g);
			}
			catch (Exception t)
			{
				Log.Info(t);
			}
		}

		public void _draw(Graphics g)
		{
			GameView instance = GameView.GetInstance();
			SmartBeginPaint(g);
			g.DrawImage(resTitle_.img[0], 0f, instance.GetGameHeight() - resTitle_.img[0].GetHeight());
			int num20, num, num2, num3;


			num20 = num = num2 = num3 = 0;
			Draw_titleAnime(g);
			apdat_.Draw_trophy(g, titleFrame_);
			if (titleFrame_ >= title_anime[0])
			{
				num3 = -111;
				num = GameUtil.RateConvert(titleFrame_, title_anime[0], title_anime[1] - 1, 0, 134, true);
				int num4 = 6;
				int num5 = titleFrame_ - title_anime[title_anime.Length - 1];
				int num6 = num5 % (num4 + num4 + 32);
				int num7 = 0;
				if (num6 < num4)
				{
					num7 = num6;
				}
				else if (num6 < num4 + num4)
				{
					num7 = num4 + num4 - num6;
				}
				int num8 = 0;
				if (num7 > 0)
				{
					int num9 = (int)GameUtil.SetVStop(-6000f, num4);
					int num10 = (int)GameUtil.SetAStop(-6000f, num4);
					num8 = num9 * num7 + num10 * num7 * (num7 + 1) / 2;
				}
				g.DrawImage(resTitle_.img[2], 0f, num3 + num + num8 / 1000);
				if (!MyFormBase.Jp() && Language.GetLanguageCode() != null && Language.GetLanguageCode() != "en-US" && !Language.GetLanguageCode().StartsWith("*"))
				{
					string text = MyFormBase.LT("サッカークラブ物語");
					g.PushFont(9);
					int num11 = AppData.ClampMin(g.StringWidth(text), 80);
					int num12 = num3 + num + 120 - 2;
					g.SetColor(0, 0, 0, 96);
					g.FillRect(120 - num11 / 2 - 1, (float)num12 - 0.5f, num11 + 2, g.GetFont(null).GetHeight() + 1);
					g.SetColor(255, 255, 255);
					g.DrawString(text, 120f, num12, 2);
					g.PopFont();
				}
			}
			if (MyFormBase.Jp() && !MyConfig.SMART_PASS && !MyConfig.AMAZON && kairoAdClickNum_ < 2)
			{
				int num13 = 60;
				num2 = -6;
				num3 = instance.GetGameHeight() - 372 - num13 - 3 - 9;
				num = 121 + num13;
				int num14 = 0;
				if (MyFormBase.En())
				{
					num14 = -8;
				}
				num14 += resTitle_.img[2].GetHeight() - 5;
				if (MyFormBase.En())
				{
					num14 += -8;
				}
				g.DrawImage(resTitle_.img[3], num2, num3 + num + num14);
				GameView.AddTouch(g, 27, num2, num3 + num + num14, 240, 63, 1441792, TouchOption.Create().Margin(-15, -15, 0, 0));
				int num15 = num2 + 180 + 20 + 3;
				int num16 = num3 + num + num14 - 12 + 9;
				g.DrawImage(resTitle_.img[4], num15, num16);
				GameView.AddTouch(g, 27, num15, num16, 30, 30, 1507328, TouchOption.Create().Margin(10, 10, 10, 10));
			}
			if (state_ == 0)
			{
				string[] array = null;
				if (!(Config.TRIAL_VERSION)) { array = new string[2] { "スタート", "ハイスコア" }; } else { array = new string[2] { "スタート", "正式版へ" }; }
				if (state_ == 0)
				{
					int num17 = instance.GetGameHeight() - 70;
					int num18 = num17 - 14 + 2;
					int num19 = 22;
					num20 = 67;
					g.DrawImage(resTitle_.img[1], num20, num18 - 7, 0, 0, 106, 31);
					g.DrawImage(resTitle_.img[1], num20, num18 + 24, 0, 36, 106, 25);
					int num21 = 94;
					int num22 = 0;
					for (int i = 0; i < array.Length; i++)
					{
						int num23 = 120;
						if (i == selMenu_)
						{
							g.SetColor(Graphics.GetColorOfRGB(44, 138, 255));
							g.FillRect(num23 - num21 / 2, num18 + num19 * i + 4, num21, 16f);
							if ((Config.TRIAL_VERSION) && (Language.English())) { } else {
apdat_.DrawSeb(g, 79 - num22, num18 + num19 * i + 18, apdat_.resCom2_.seb[0], apdat_.resCom2_.img[2], 0);
apdat_.DrawSeb(g, 160 + num22, num18 + num19 * i + 18, apdat_.resCom2_.seb[0], apdat_.resCom2_.img[2], 0);
						}}
						if (i == selMenu_)
						{
							g.SetColor(Graphics.GetColorOfRGB(255, 255, 224));
						}
						else
						{
							g.SetColor(Graphics.GetColorOfRGB(255, 255, 224));
						}
						g.DrawString(array[i], num23, num18 + num19 * i + 6, 2);
						GameView.AddTouch(g, 0, num23 - 50, num18 + num19 * i + 1, 100, num19, i);
					}
				}
				if (Config.DEBUG && Config.SHOW_DEBUG && !formMan_.IsCapturing())
				{
					string text2 = Language.GetLanguageCode();
					if (text2 == null)
					{
						text2 = " null ";
					}
					g.SetColor(Graphics.GetColorOfRGB(255, 0, 0));
					g.DrawString("DEBUG MODE[" + text2 + "]", 2f, 2f);
					g.DrawStringBold("2017/02/03 12:01:57", 2f, 18f, Graphics.GetColorOfRGB(255, 255, 255), 1);
				}
			}
			if (STAR)
			{
				g.SetColor(Graphics.GetColorOfRGB(255, 0, 0));
				g.DrawString("☆", 2f, 16f);
			}
			if (state_ == 1 && (apdat_.IsTopForm(this) || (formMan_.GetTopForm().GetId() == 4 && ((SubForm)formMan_.GetTopForm()).IsMenu())))
			{
				int num24 = (instance.GetGameHeight() - 240) / 2;
				apdat_.DrawWindow(g, 198, 146, num24, MyFormBase.LT("セーブスロット <0>/2", selectSave_ + 1), -12);
				if (Active())
				{
					GameView.AddTouch(g, 1, 34 - frame_ % 20 / 5, 54 + num24, apdat_.resCom2_.seb[24], 0, 16);
					GameView.AddTouch(g, 1, 203 + frame_ % 20 / 5, 54 + num24, apdat_.resCom2_.seb[24], 1, 18);
				}
				int num25 = num24 + 74 + 6;
				g.SetColor(60, 100, 200);
				int dx = 42;
				int dx2 = 199;
				int num26 = 93 + num24;
				g.ClipRect(0f, num26 - 28, 240f, 20f);
				SubForm.Draw_chara(g, 171, 0, 0, frame_, dx, num26);
				SubForm.Draw_chara(g, 172, 0, 1, frame_, dx2, num26);
				g.PopClip();
				if (selectSaveType_ == 0)
				{
					g.DrawString("中断データから再開", 120f, num25 - 11, 2);
				}
				else
				{
					g.DrawString("セーブデータを読込", 120f, num25 - 11, 2);
				}
				int num27 = 29;
				int num28 = 181;
				int num29 = 41;
				num25 += 4;
				for (int j = 0; j < 2; j++)
				{
					apdat_.DrawBox(g, num27, num25 - 12, num27 + num28, num25 - 12 + num29);
					int num30 = apdat_.sysSave_.ints[((j == 0) ? 11 : 6) + selectSave_];
					if (num30 == -1 && j == 0)
					{
						g.DrawImage(apdat_.resCom2_.img[193], num27 + 10 + IMG_SL[1][0], num25 + 15 + IMG_SL[1][1], IMG_SL[1][2], IMG_SL[1][3], IMG_SL[1][4], IMG_SL[1][5]);
						g.SetColor(Graphics.GetColorOfRGB(30, 30, 30));
						g.DrawString("中断データなし", 7 + num27 + num28 / 2, num25 + 15, 2);
						g.SetRenderMode(1, 128);
						g.SetColor(0, 0, 0);
						g.FillRect(num27, num25, num28, num29);
						g.SetRenderMode();
						if (selectSaveType_ == j && apdat_.IsTopForm(this))
						{
							apdat_.resCom2_.seb[61].Draw(g, 7 + num27 + num28 - 40, num25 + 21, apdat_.resCom2_.img);
						}
					}
					else if (num30 == -1 && j == 1)
					{
						if (selectSaveType_ == j)
						{
							g.SetColor(Graphics.GetColorOfRGB(200, 200, 255));
							g.FillRect(7 + num27 + num28 / 2 - 36 - 5, num25 + 15 - 1, 82f, 14f);
						}
						GameView.AddTouch(g, 3, 0, num25 - 5, 240, num29 + 10, 0x20000 | j);
						g.DrawImage(apdat_.resCom2_.img[193], num27 + 10 + IMG_SL[0][0], num25 + 15 + IMG_SL[0][1], IMG_SL[0][2], IMG_SL[0][3], IMG_SL[0][4], IMG_SL[0][5]);
						g.SetColor(Graphics.GetColorOfRGB(30, 30, 30));
						g.DrawString("空きスロット", 7 + num27 + num28 / 2, num25 + 15, 2);
						if (selectSaveType_ == j && apdat_.IsTopForm(this))
						{
							apdat_.resCom2_.seb[61].Draw(g, 7 + num27 + num28 - 40, num25 + 21, apdat_.resCom2_.img);
						}
					}
					else
					{
						if (selectSaveType_ == j)
						{
							g.SetColor(Graphics.GetColorOfRGB(200, 200, 255));
							g.FillRect(7 + num27 + 90 - 42 - 5, num25 + 7 - 1, 94f, 14f);
						}
						GameView.AddTouch(g, 3, 0, num25 - 5, 240, num29 + 10, 0x20000 | j);
						int num31 = 1 - j;
						g.DrawImage(apdat_.resCom2_.img[193], num27 + 10 + IMG_SL[num31][0], num25 + 6 + IMG_SL[num31][1], IMG_SL[num31][2], IMG_SL[num31][3], IMG_SL[num31][4], IMG_SL[num31][5]);
						g.SetColor(Graphics.GetColorOfRGB(30, 30, 30));
						g.DrawString(apdat_.sysSave_.strs[((j == 0) ? 6 : 2) + selectSave_], 7 + num27 + 90, num25 + 7, 2);
						int num32 = num30 / 10000;
						int num33 = num30 % 10000 / 100;
						int num34 = num30 % 100;
						g.SetColor(Graphics.GetColorOfRGB(30, 30, 30));
						g.DrawString(LF("10,,+2"), MyFormBase.LT("<0>年<1>月<2>週", StringUtil.FormatString(JString.ValueOf(num32 + 1), 2, ' ') + "\t" + StringUtil.FormatString(JString.ValueOf(num33 + 1), 2, ' ', false) + "\t" + JString.ValueOf(num34 + 1)), 7 + num27 + 70 - 72, num25 + 24);
						g.DrawString(MyFormBase.YEN("万円", JLong.ParseLong(apdat_.sysSave_.strs[((j == 0) ? 8 : 4) + selectSave_])), num27 + 172, num25 + 24, 4);
						if (selectSaveType_ == j && apdat_.IsTopForm(this))
						{
							apdat_.resCom2_.seb[61].Draw(g, 7 + num27 + 145, num25 + 12, apdat_.resCom2_.img);
						}
					}
					num25 += 47;
				}
				num25 -= 2;
				g.SetColor(Graphics.GetColorOfRGB(125, 158, 116));
				g.DrawString("データを選択してください", 120f, num25, 2);
			}
			if (Config.TRIAL_VERSION)
			{
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				g.DrawStringBold("Lite Version", 235f, 5f, Graphics.GetColorOfRGB(0, 0, 0), 4);
			}
			g.SetOrigin(0, 0);
			if (!Language.Japanese())
			{
				LanguageForm.DrawButton(g, delegate
				{
					formMan_.ChangeCurrentForm(apdat_.frmBoot_);
				});
			}
		}

		public override bool Resume()
		{
			return base.Resume();
		}

		public override bool Finish()
		{
			base.Finish();
			if (resTitle_ != null)
			{
				resTitle_.Dispose();
			}
			resTitle_ = null;
			GamePad.versionVisible_ = false;
			return true;
		}

		private void ChangeState(int state)
		{
			int[] array = softLabels_[state];
			if (array != null)
			{
				SetSoftLabel(AppData.softLabels_[array[0]], AppData.softLabels_[array[1]]);
			}
			state_ = state;
		}

		public void Update_titleAnime()
		{
			for (int i = 0; i < ta_data.Length; i++)
			{
				if (ta_data[i][0] == 0) { continue; }
ta_data[i][5] = (ta_data[i][5] + 1) % int.MaxValue;
ta_data[i][2] += -1 + 2 * ta_data[i][4];
if (ta_data[i][2] < -10 || ta_data[i][2] > 250)
					{
						ResetHobjExist(i);
					}
			}
			apeear_cnt++;
			if (apeear_cnt >= APPEAR_TIME) {

			apeear_cnt = 0;
			SetAppearCnt();
			int num = -1;
			for (int j = 0; j < ta_data.Length; j++)
			{
				if (ta_data[j][0] == 0)
				{
					num = j;
					break;
				}
			}
			if (num != -1)
			{
				ta_data[num][0] = 1;
				ta_data[num][1] = TITLE_GUEST[GameUtil.Random(TITLE_GUEST.Length)];
				ta_data[num][4] = GameUtil.Random(2);
				if (ta_data[num][4] == 1)
				{
					ta_data[num][2] = 0;
					ta_data[num][3] = GameUtil.RateConvert(GameUtil.Random(100), 0, 99, 228, 238, true);
				}
				else
				{
					ta_data[num][2] = 240;
					ta_data[num][3] = GameUtil.RateConvert(GameUtil.Random(100), 0, 99, 228, 238, true);
				}
			}
}
		}

		private void SetAppearCnt()
		{
			APPEAR_TIME = GameUtil.RateConvert(GameUtil.Random(100), 0, 99, 20, 70, true);
		}

		public void ResetHobjExist(int index)
		{
			ta_data[index][0] = 0;
		}

		public void Draw_titleAnime(Graphics g)
		{
			for (int i = 0; i < ta_data.Length; i++)
			{
				zsort_mhobj[i][0] = i;
				zsort_mhobj[i][1] = ta_data[i][3];
			}
			for (int j = 0; j < zsort_mhobj.Length - 1; j++)
			{
				for (int num = zsort_mhobj.Length - 1; num > j; num--)
				{
					if (zsort_mhobj[j][1] > zsort_mhobj[num][1])
					{
						int num2 = zsort_mhobj[num][1];
						zsort_mhobj[num][1] = zsort_mhobj[j][1];
						zsort_mhobj[j][1] = num2;
						num2 = zsort_mhobj[num][0];
						zsort_mhobj[num][0] = zsort_mhobj[j][0];
						zsort_mhobj[j][0] = num2;
					}
				}
			}
			int num3 = view_.GetGameHeight() - 240;
			for (int k = 0; k < zsort_mhobj.Length; k++)
			{
				int num4 = zsort_mhobj[k][0];
				if (ta_data[num4][0] == 0) { continue; }
int seb = 0;
int frame = ta_data[num4][5] % 20 / 5;
int direct = 2;
if (ta_data[num4][4] == 1)
					{
						direct = 1;
					}
PlayerData playerData = apdat_.pData_[ta_data[num4][1]];
apdat_.DrawSeb(g, ta_data[num4][2] - 1, ta_data[num4][3] + 1 + num3, apdat_.resCom2_.seb[0], apdat_.resCom2_.img[2], 4);
udat_.SetDispPlayerData(seb, udat_.selectUniform_, playerData.faceImg_, frame, direct);
udat_.DrawDispPlayer(g, ta_data[num4][2], ta_data[num4][3] + num3);
			}
		}

		public override void SetTouchValue(int type, int value)
		{
			type = (int)(type & 0xFFFF0000u);
			switch (type)
			{
			case 196608:
				selMenu_ = value;
				break;
			case 131072:
				selectSaveType_ = value;
				break;
			default:
				base.SetTouchValue(type, value);
				break;
			}
		}

		public override bool OnTouchEvent(TouchEvent e)
		{
			if (!LanguageForm.OnTouchEvent(e))
			{
				return false;
			}
			TouchComponent component = e.component;
			if (state_ == 0)
			{
				switch (component.id)
{
case 27:
if (e.type == 1)
{
					if ((component.value & 0xFFFF0000u) == 1441792)
					{
						try
						{
							UpdateKairoAdClickNum();
							apdat_.SaveSystem();
						}
						catch (Exception t)
						{
							Log.Info(t);
						}
						if (!MyConfig.SMART_PASS && !MyConfig.AMAZON && kairoAdClickNum_ <= 2 && MyFormBase.Jp())
						{
							Canvas.OpenBrowser(kairoAdUrl_jp);
						}
					}
					else if ((component.value & 0xFFFF0000u) == 1507328)
					{
						try
						{
							UpdateKairoAdClickNum(10);
							apdat_.SaveSystem();
						}
						catch (Exception t2)
						{
							Log.Info(t2);
						}
					}
					return false;

}
break;
}
			}
			return true;
		}

		public void LoadKiaroAdClickNum()
		{
			if (MyConfig.SMART_PASS || MyConfig.AMAZON)
			{
				return;
			}
			kairoAdClickNum_ = 0;
			if (apdat_.sysSave_.bins[5].Length > 0) {

			ByteArrayInputStream byteArrayInputStream = null;
			try
			{
				byteArrayInputStream = new ByteArrayInputStream(apdat_.sysSave_.bins[5]);
				kairoAdClickNum_ = StreamUtil.ReadShort(byteArrayInputStream);
			}
			finally
			{
				if (byteArrayInputStream != null)
				{
					byteArrayInputStream.Close();
				}
				byteArrayInputStream = null;
			}
}
		}

		public void UpdateKairoAdClickNum()
		{
			UpdateKairoAdClickNum(1);
		}

		public void UpdateKairoAdClickNum(int addClickNum)
		{
			if (MyConfig.SMART_PASS || MyConfig.AMAZON)
			{
				return;
			}
			kairoAdClickNum_ += addClickNum;
			ByteArrayOutputStream byteArrayOutputStream = null;
			try
			{
				byteArrayOutputStream = new ByteArrayOutputStream();
				StreamUtil.WriteShort(byteArrayOutputStream, (short)kairoAdClickNum_);
				apdat_.sysSave_.bins[5] = byteArrayOutputStream.ToByteArray();
			}
			finally
			{
				if (byteArrayOutputStream != null)
				{
					byteArrayOutputStream.Close();
				}
				byteArrayOutputStream = null;
			}
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
