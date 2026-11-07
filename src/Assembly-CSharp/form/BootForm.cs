using System;
using System.Threading;
using data;
using game;
using java.io;
using java.lang;
using java.util;
using kairo.common.cfg;
using kairo.unity.io;
using kairo.unity.ui;
using kairo.unity.util;
using main;

namespace form
{
	public class BootForm : MyFormBase, JRunnable
	{
		private const int WAIT = 4;

		private const int RESLOAD_SEB_LOAD = 0;

		private const int STATE_BOOT = 0;

		private const int STATE_LOAD = 1;

		private const int STATE_REGIST = 2;

		private const int STATE_ERROR = 3;

		private const int STATE_FINISH = 4;

		private const int STATE_VERSIONUP = 5;

		private const int ANIME_LEFT_X = 66;

		private const int ANIME_RIGHT_X = 174;

		private const int ANIME_ONEWAY_TIME = 20;

		private const int ANIME_KICK_TIME = 5;

		private int frame_;

		private int loop_;

		private int bowWait_;

		private int loadMax_;

		private int loadCnt_;

		private int frame_wait;

		private int frame_main;

		private byte[] regist_;

		private new SubForm dialog_;

		private string error_;

		private JThread th_;

		private Exception e_;

		private ResourceManager resLoad_;

		private int state_;

		private static readonly int[][] softLabels_ = new int[6][]
		{
			new int[2],
			new int[2],
			new int[2],
			new int[2],
			new int[2],
			new int[2]
		};

		private int anime_mode;

		private int anime_ballY;

		private int anime_cnt;

		public BootForm()
			: base(0)
		{
		}

		public override string ToString()
		{
			return "BootForm:" + state_;
		}

		public override bool Init()
		{
			base.Init();
			if (!apdat_.LoadSystem())
			{
				InitApp();
			}
			canvas_.SetKeyRepeatInterval(10);
			canvas_.SetKeyDoubleInterval(4);
			try
			{
				resLoad_ = new ResourceManager(null, null, null, null, null, null, null);
				resLoad_.Load(Storage.Read(3, false, "load.dat"));
			}
			catch (Exception t)
			{
				Log.Info(t);
			}
			Graphics.SetAutoTextLayout(true);
			loop_ = 0;
			bowWait_ = 0;
			ChangeState(0);
			error_ = null;
			regist_ = null;
			th_ = new JThread(this);
			th_.Start();
			app_.ResetOrientation();
			return true;
		}

		public override bool Update()
		{
			base.Update();
			if (resLoad_ != null)
			{
				resLoad_.seb[0].Frame();
			}
			if (canvas_.CheckKeyPulse(2))
			{
				Config.SHOW_DEBUG = !Config.SHOW_DEBUG;
			}
			frame_wait = (frame_wait + 1) % 16;
			frame_main = (frame_main + 1) % int.MaxValue;
			if (state_ == 3)
			{
				if (dialog_ != null)
				{
					formMan_.RemoveAllForms();
					if ((canvas_.GetKeypadState() & 0x800) != 0)
					{
						formMan_.Push(new SpForm(e_));
					}
					return true;
				}
				if (apdat_.resCom2_ != null)
				{
					dialog_ = SubForm.MakeDialog(error_);
					return formMan_.Push(dialog_);
				}
				if (MyFormBase.Jp())
				{
					Dialog dialog = new Dialog(0, "ｴﾗｰ");
					dialog.SetText(error_);
					dialog.Show();
				}
				else
				{
					Dialog dialog2 = new Dialog(0, AppData.resourceString[7]);
					dialog2.SetText(error_);
					dialog2.Show();
				}
				formMan_.RemoveAllForms();
				if ((canvas_.GetKeypadState() & 0x800) != 0)
				{
					formMan_.Push(new SpForm(e_));
				}
				return true;
			}
			if (state_ == 5)
			{
				Canvas.VersionUp(Config.URL_VERSIONUP);
				return formMan_.RemoveAllForms();
			}
			if (state_ == 4)
			{
				if (regist_ == null)
				{
					formMan_.ChangeCurrentForm(apdat_.frmTitle_);
				}
				else
				{
					string[] array = StringUtil.Split(JString.New(regist_), "\n");
					if (JInteger.ParseInt(array[1]) > 202)
					{
						apdat_.sysSave_.ints[5] = 1;
						apdat_.SaveSystem();
						formMan_.Push(SubForm.MakeDialog("ｱﾌﾟﾘのﾊﾞｰｼﾞｮﾝｱｯﾌﾟを行います"));
						ChangeState(5);
					}
					else
					{
						formMan_.ChangeCurrentForm(apdat_.frmTitle_);
					}
				}
			}
			JThread.Sleep(50L);
			return true;
		}

		public void SmartBeginPaint(Graphics g)
		{
			g.SetOrigin(0, 0);
			int gameWidth = view_.GetGameWidth();
			int gameHeight = view_.GetGameHeight();
			g.SetColor(Graphics.GetColorOfRGB(0, 0, 0));
			g.FillRect(0f, 0f, gameWidth, gameHeight);
			g.DrawScaledImage(resLoad_.img[1], 0f, 0f, gameWidth, gameHeight, 0, 0, 5, 240);
			g.SetOrigin((gameWidth - 240) / 2, (gameHeight - 240) / 2);
		}

		public override void Draw(Graphics g)
		{
			base.Draw(g);
			SmartBeginPaint(g);
			if (state_ == 0) { } else {
				if (state_ == 1)
				{
					string text = MyFormBase.LT("ｱﾌﾟﾘ起動中");
					for (int i = 4; i < frame_; i += 4)
					{
						if (bowWait_ != 0)
						{
							break;
						}
						text += ".";
					}
					LoadBox(g, text, loadCnt_, loadMax_);
				}
				else if (state_ == 2)
				{
					string empty = string.Empty;
					empty = ((!MyFormBase.Jp()) ? AppData.resourceString[6] : MyFormBase.LT("認証中"));
					for (int j = 4; j < frame_; j += 4)
					{
						if (bowWait_ != 0)
						{
							break;
						}
						empty += ".";
					}
					LoadBox(g, empty, 0, 0);
				}
			}
			g.SetOrigin(0, 0);
		}

		public override bool Resume()
		{
			base.Resume();
			return true;
		}

		public override bool Finish()
		{
			base.Finish();
			if (resLoad_ != null)
			{
				resLoad_.Dispose();
			}
			resLoad_ = null;
			return true;
		}

		public void LoadBox(Graphics g, string str, int cnt, int num)
		{
			g.DrawImage(resLoad_.img[0], 43f, 90f);
			resLoad_.seb[0].DrawFrame(g, 120f, 236f, resLoad_.img, 5);

int num9, num2, num3, num4, num5;

num2 = 91;
			anime_cnt = (anime_cnt + 1) % int.MaxValue;
			num3 = anime_cnt % 40;
			if (num3 % 20 == 0 && anime_cnt != 0)
			{
				anime_mode = GameUtil.Random(2);
				anime_ballY = -(GameUtil.Random(25) + 5);
			}
			if (num3 < 5)
			{
				resLoad_.seb[0].DrawFrame(g, 58f, num2, resLoad_.img, 1);
			}
			else
			{
				resLoad_.seb[0].DrawFrame(g, 58f, num2, resLoad_.img, 0);
			}
			if (num3 >= 20 && num3 < 25)
			{
				resLoad_.seb[0].DrawFrame(g, 182f, num2, resLoad_.img, 3);
			}
			else
			{
				resLoad_.seb[0].DrawFrame(g, 182f, num2, resLoad_.img, 2);
			}
			num4 = GameUtil.RateConvert(JMath.Abs(num3 - 20), 20, 0, 66, 174, true);
			num5 = num2;
			if (anime_mode == 1)
			{
				float num6 = GameUtil.SetVStop(anime_ballY, 10);
				float num7 = GameUtil.SetAStop(anime_ballY, 10);
				int num8 = JMath.Abs(num3 - 20);
				num5 = (int)(num6 * (float)num8 + num7 * (float)num8 * (float)(num8 + 1) / 2f) + num2;
				if (num5 > num2)
				{
					num5 = num2;
				}
			}
			resLoad_.seb[0].DrawFrame(g, num4, num5, resLoad_.img, 4);

			if (num > 0)
			{
				num9 = 85;
				num2 = 129;
				int num10 = 60;
				g.SetColor(Graphics.GetColorOfRGB(169, 33, 0));
				g.FillRect(num9, num2 + 1, cnt * num10 / num, 4f);
				g.SetColor(Graphics.GetColorOfRGB(255, 104, 122));
				g.FillRect(num9, num2, cnt * num10 / num, 1f);
				g.SetColor(Graphics.GetColorOfRGB(255, 25, 51));
				g.FillRect(num9, num2 + 1, cnt * num10 / num, 1f);
				g.SetColor(Graphics.GetColorOfRGB(255, 7, 37));
				g.FillRect(num9, num2 + 2, cnt * num10 / num, 1f);
				g.SetColor(Graphics.GetColorOfRGB(246, 0, 29));
				g.FillRect(num9, num2 + 3, cnt * num10 / num, 1f);
			}
			num9 = 83;
			num2 = 112;
			g.SetColor(Graphics.GetColorOfRGB(55, 51, 35));
			g.DrawString(str, num9, num2);
			g.SetColor(Graphics.GetColorOfRGB(221, 221, 221));
			g.DrawString(str, num9, num2 - 1);
		}

		private void ChangeState(int state)
		{
			int[] array = softLabels_[state];
			SetSoftLabel(AppData.softLabels_[array[0]], AppData.softLabels_[array[1]]);
			state_ = state;
			frame_wait = 0;
		}

		private void InitApp()
		{
			RecordStore.CreateRecordStore();
			RecordStore recordStore = null;
			try
			{
				recordStore = RecordStore.OpenRecordStore(0);
				recordStore.AddRecord(8192);
				recordStore.AddRecord(46080);
				recordStore.AddRecord(46080);
				recordStore.AddRecord(46080);
				recordStore.AddRecord(46080);
			}
			finally
			{
				if (recordStore != null)
				{
					recordStore.CloseRecordStore();
				}
				recordStore = null;
			}
			apdat_.InitSystemSaveData();
			apdat_.SaveSystem();
		}

		public void Run()
		{
			loadMax_ = 0;
			loadCnt_ = 0;
			try
			{
				if (state_ == 0)
				{
					JSystem.Gc();
					try
					{
						if (apdat_.sysSave_.ints[0] < 202)
						{
							apdat_.sysSave_.ints[0] = 202;
							apdat_.SaveSystem();
						}
						int state = 1;
						ChangeState(state);
						apdat_.sysSave_.ints[5]--;
						apdat_.SaveSystem();
						if (apdat_.sysSave_.ints[5] <= 0 && Config.REGIST_ENABLE)
						{
							ChangeState(2);
						}
						else
						{
							ChangeState(1);
						}
					}
					catch (Exception t)
					{
						if (Config.PRINT)
						{
							Log.Info(t);
						}
						e_ = t;
						error_ = "読み込みに失敗しました。";
						ChangeState(3);
						return;
					}
				}
				if (state_ == 2)
				{
					try
					{
						regist_ = apdat_.CgiRegist("-1", "0");
						apdat_.sysSave_.ints[5] = 30;
						apdat_.SaveSystem();
						ChangeState(1);
					}
					catch (Exception t2)
					{
						if (Config.PRINT)
						{
							Log.Info(t2);
						}
						e_ = t2;
						error_ = apdat_.GetHttpErrorText();
						ChangeState(3);
						return;
					}
				}
				if (state_ == 1)
				{
					try
					{
						Load();
						apdat_.SaveSystem();
					}
					catch (Exception t3)
					{
						if (Config.PRINT)
						{
							Log.Info(t3);
						}
						e_ = t3;
						error_ = "読み込みに失敗しました。";
						ChangeState(3);
						return;
					}
				}
				try
				{
					bowWait_ = 5;
					if (regist_ != null)
					{
						string[] array = StringUtil.Split(JString.New(regist_), "\n");
						if (JInteger.ParseInt(array[1]) > 202)
						{
							bowWait_ = 0;
						}
					}
				}
				catch (Exception)
				{
					bowWait_ = 0;
				}
				while (bowWait_ > 0)
				{
					try
					{
						JThread.Sleep(50L);
					}
					catch (ThreadAbortException)
					{
					}
					bowWait_--;
					if (canvas_.CheckKeyPulse(1048576))
					{
						bowWait_ = 0;
					}
				}
				ChangeState(4);
			}
			finally
			{
				th_ = null;
			}
		}

		public void Download()
		{
			string[] strDatFileName_ = AppData.strDatFileName_;
			RecordStore recordStore = RecordStore.OpenRecordStore(1);
			int num = AppData.dateFileHead_[apdat_.sysSave_.ints[2]];
			int num2 = recordStore.GetNumRecords() - num;
			for (int i = 0; i < num2; i++)
			{
				recordStore.RemoveLastRecord();
			}
			loadMax_ = strDatFileName_.Length * 2 + 12;
			loadCnt_ = apdat_.sysSave_.ints[2] * 2 + recordStore.GetNumRecords();
			for (int j = apdat_.sysSave_.ints[2]; j < strDatFileName_.Length; j++)
			{
				byte[][] array = apdat_.PhpDlFile(null, strDatFileName_[j]);
				loadCnt_++;
				for (int k = 0; k < array.Length; k++)
				{
					recordStore.AddRecord(array[k]);
					loadCnt_++;
				}
				loadCnt_++;
				apdat_.sysSave_.ints[2]++;
				apdat_.SaveSystem();
			}
			recordStore.CloseRecordStore();
			apdat_.sysSave_.ints[1] = BitUtil.On(apdat_.sysSave_.ints[1], 1);
			apdat_.SaveSystem();
			JThread.Sleep(100L);
			if (Config.PRINT)
			{
				Log.Info("ダウンロード=" + loadCnt_ + "/" + loadMax_);
			}
		}

		public void Load()
		{
			CreateAllClass();
			loadMax_ = 30;
			loadCnt_ = 0;
			if (apdat_.resCom2_ != null)
			{
				apdat_.resCom2_.Dispose();
			}
			loadCnt_++;
			global::kairo.unity.io.RecordStore recordStore;
JarInflater jarInflater = null;
			try
			{
				recordStore = RecordStore.OpenRecordStore(1);
				byte[] data = recordStore.ReadRecord(6);
				recordStore.CloseRecordStore();
				jarInflater = new JarInflater(data);
				string[] array = StringUtil.LoadStringArray(jarInflater, "player.txt");
				apdat_.pData_ = new PlayerData[array.Length];
				for (int i = 0; i < apdat_.pData_.Length; i++)
				{
					apdat_.pData_[i] = new PlayerData(array[i]);
				}
				loadCnt_++;
				array = StringUtil.LoadStringArray(jarInflater, "team.txt");
				apdat_.tData_ = new TeamData[array.Length];
				for (int j = 0; j < apdat_.tData_.Length; j++)
				{
					apdat_.tData_[j] = new TeamData(array[j]);
				}
				loadCnt_++;
				array = StringUtil.LoadStringArray(jarInflater, "supervisor.txt");
				apdat_.svisorData_ = new SupervisorData[array.Length];
				for (int k = 0; k < apdat_.svisorData_.Length; k++)
				{
					apdat_.svisorData_[k] = new SupervisorData(array[k]);
				}
				loadCnt_++;
				array = StringUtil.LoadStringArray(jarInflater, "formation.txt");
				apdat_.formationData_ = new FormationData[array.Length];
				for (int l = 0; l < apdat_.formationData_.Length; l++)
				{
					apdat_.formationData_[l] = new FormationData(array[l]);
				}
				loadCnt_++;
				array = StringUtil.LoadStringArray(jarInflater, "mapchip_match.txt");
				apdat_.mapchipMatch_ = new MapchipMatch[array.Length];
				for (int m = 0; m < apdat_.mapchipMatch_.Length; m++)
				{
					apdat_.mapchipMatch_[m] = new MapchipMatch(array[m]);
				}
				loadCnt_++;
				array = StringUtil.LoadStringArray(jarInflater, "mapchip_main.txt");
				apdat_.mapchipMain_ = new MapchipMain[array.Length];
				for (int n = 0; n < apdat_.mapchipMain_.Length; n++)
				{
					apdat_.mapchipMain_[n] = new MapchipMain(array[n]);
				}
				loadCnt_++;
				array = StringUtil.LoadStringArray(jarInflater, "stadium.txt");
				apdat_.stadiumData_ = new StadiumData[array.Length];
				for (int num = 0; num < apdat_.stadiumData_.Length; num++)
				{
					apdat_.stadiumData_[num] = new StadiumData(array[num]);
				}
				loadCnt_++;
				array = StringUtil.LoadStringArray(jarInflater, "fanActData.txt");
				apdat_.fanActData_ = new FanActData[array.Length];
				for (int num2 = 0; num2 < apdat_.fanActData_.Length; num2++)
				{
					apdat_.fanActData_[num2] = new FanActData(array[num2]);
				}
				loadCnt_++;
				array = StringUtil.LoadStringArray(jarInflater, "sponsorData.txt");
				apdat_.sponsorData_ = new SponsorData[array.Length];
				for (int num3 = 0; num3 < apdat_.sponsorData_.Length; num3++)
				{
					apdat_.sponsorData_[num3] = new SponsorData(array[num3]);
				}
				loadCnt_++;
				array = StringUtil.LoadStringArray(jarInflater, "fanBonus.txt");
				apdat_.fanBonus_ = new FanBonusData[array.Length];
				for (int num4 = 0; num4 < apdat_.fanBonus_.Length; num4++)
				{
					apdat_.fanBonus_[num4] = new FanBonusData(array[num4]);
				}
				loadCnt_++;
				array = StringUtil.LoadStringArray(jarInflater, "questMatch.txt");
				apdat_.qmData_ = new QuestMatchData[array.Length];
				for (int num5 = 0; num5 < apdat_.qmData_.Length; num5++)
				{
					apdat_.qmData_[num5] = new QuestMatchData(array[num5]);
				}
				loadCnt_++;
				array = StringUtil.LoadStringArray(jarInflater, "tenantData.txt");
				apdat_.tenantData_ = new TenantData[array.Length];
				for (int num6 = 0; num6 < apdat_.tenantData_.Length; num6++)
				{
					apdat_.tenantData_[num6] = new TenantData(array[num6]);
				}
				loadCnt_++;
				array = StringUtil.LoadStringArray(jarInflater, "tradePlayer.txt");
				apdat_.tradeData_ = new TradeData[array.Length];
				for (int num7 = 0; num7 < apdat_.tradeData_.Length; num7++)
				{
					apdat_.tradeData_[num7] = new TradeData(array[num7]);
				}
				loadCnt_++;
				array = StringUtil.LoadStringArray(jarInflater, "trainingData.txt");
				apdat_.trainingData_ = new TrainingData[array.Length];
				for (int num8 = 0; num8 < apdat_.trainingData_.Length; num8++)
				{
					apdat_.trainingData_[num8] = new TrainingData(array[num8]);
				}
				loadCnt_++;
				array = StringUtil.LoadStringArray(jarInflater, "leagueData.txt");
				apdat_.leagueData_ = new LeagueData[array.Length];
				for (int num9 = 0; num9 < apdat_.leagueData_.Length; num9++)
				{
					apdat_.leagueData_[num9] = new LeagueData(array[num9]);
				}
				loadCnt_++;
				InputStream inputStream = null;
				try
				{
					inputStream = jarInflater.GetInputStream("manual.txt");
					byte[] bytes = StreamUtil.Read(inputStream);
					apdat_.manualTexts_ = StringUtil.Split(JString.New(bytes), "|");
				}
				finally
				{
					if (inputStream != null)
					{
						inputStream.Close();
					}
					inputStream = null;
				}
				loadCnt_++;
				try
				{
					inputStream = jarInflater.GetInputStream("talk.txt");
					byte[] bytes2 = StreamUtil.Read(inputStream);
					AppData.talks_ = StringUtil.Split(JString.New(bytes2), "|");
				}
				finally
				{
					if (inputStream != null)
					{
						inputStream.Close();
					}
					inputStream = null;
				}
				loadCnt_++;
				try
				{
					inputStream = jarInflater.GetInputStream("news.txt");
					byte[] bytes3 = StreamUtil.Read(inputStream);
					string[] array2 = StringUtil.Split(JString.New(bytes3), "|");
					apdat_.newsTypes_ = new int[array2.Length];
					apdat_.newsTitles_ = new string[array2.Length];
					apdat_.newsTexts_ = new string[array2.Length];
					for (int num10 = 0; num10 < array2.Length; num10++)
					{
						string[] array3 = StringUtil.Split(array2[num10], "\t");
						apdat_.newsTypes_[num10] = JInteger.ParseInt(array3[0]);
						apdat_.newsTitles_[num10] = array3[1];
						apdat_.newsTexts_[num10] = array3[2];
					}
				}
				finally
				{
					if (inputStream != null)
					{
						inputStream.Close();
					}
					inputStream = null;
				}
				loadCnt_++;
				try
				{
					inputStream = jarInflater.GetInputStream("evtmsgs.txt");
					byte[] bytes4 = StreamUtil.Read(inputStream);
					apdat_.eventMsgs_ = StringUtil.Split(JString.New(bytes4), "|");
				}
				finally
				{
					if (inputStream != null)
					{
						inputStream.Close();
					}
					inputStream = null;
				}
				loadCnt_++;
				array = StringUtil.LoadStringArray(jarInflater, "events.txt");
				apdat_.eventData_ = new EventData[array.Length];
				for (int num11 = 0; num11 < array.Length; num11++)
				{
					apdat_.eventData_[num11] = new EventData(array[num11]);
				}
				apdat_.eventData_ = (EventData[])apdat_.AdjustDataList(apdat_.eventData_, 1);
				loadCnt_++;
				try
				{
					inputStream = jarInflater.GetInputStream("talk_specialChara.txt");
					byte[] bytes5 = StreamUtil.Read(inputStream);
					string[] array4 = StringUtil.Split(JString.New(bytes5), "\n");
					UserData instance = UserData.GetInstance();
					instance.TALK_SCHARA_ID = new int[array4.Length];
					instance.TALK_SCHARA_NAME = new string[array4.Length];
					instance.TALK_SCHARA_IMGINDEX = new int[array4.Length];
					for (int num12 = 0; num12 < array4.Length; num12++)
					{
						string[] array5 = StringUtil.Split(array4[num12], "\t");
						instance.TALK_SCHARA_ID[num12] = JInteger.ParseInt(array5[0]);
						instance.TALK_SCHARA_NAME[num12] = array5[1];
						instance.TALK_SCHARA_IMGINDEX[num12] = JInteger.ParseInt(array5[2]);
					}
				}
				finally
				{
					if (inputStream != null)
					{
						inputStream.Close();
					}
					inputStream = null;
				}
				loadCnt_++;
			}
			catch (Exception t)
			{
				Log.Info(t);
				if (Config.PRINT)
				{
					Log.Info("■BootForm.java load() loadCnt_ = " + loadCnt_);
				}
				if (Config.PRINT)
				{
					Log.Info("■BootForm.java load() xlsデータの読み込みでエラー");
				}
			}
			finally
			{
				if (jarInflater != null)
				{
					jarInflater.Close();
				}
				jarInflater = null;
			}
			try
			{
				if (apdat_.resCom2_ != null)
				{
					apdat_.resCom2_.Dispose();
				}
				apdat_.resCom2_ = new ResourceManager(null, null, null, null, null, null, null);
				apdat_.resCom2_.Load(RecordStore.ReadRecord(1, 1));
				loadCnt_++;
				if (apdat_.resHuman2_ != null)
				{
					apdat_.resHuman2_.Dispose();
				}
				apdat_.resHuman2_ = new ResourceManager(null, null, null, null, null, null, null);
				apdat_.resHuman2_.Load(RecordStore.ReadRecord(1, 9));
				loadCnt_++;
				if (apdat_.resHumanHead_ != null)
				{
					apdat_.resHumanHead_.Dispose();
				}
				apdat_.resHumanHead_ = new ResourceManager(null, null, null, null, null, null, null);
				apdat_.resHumanHead_.Load(RecordStore.ReadRecord(1, 10));
				loadCnt_++;
				if (apdat_.resEvent_ != null)
				{
					apdat_.resEvent_.Dispose();
				}
				apdat_.resEvent_ = new ResourceManager(null, null, null, null, null, null, null);
				apdat_.resEvent_.Load(RecordStore.ReadRecord(1, 3));
				loadCnt_++;
				apdat_.resSound_ = new ResourceManager(null, null, null, null, null, null, null);
				apdat_.resSound_.Load(AppData.RSFILENAMESS[1][11]);
				for (int num13 = 0; num13 < AppData.BGM.Length; num13++)
				{
					apdat_.resSound_.sound[AppData.BGM[num13]].SetLoop(-1);
					apdat_.resSound_.sound[AppData.BGM[num13]].SetSoundType(0);
				}
				for (int num14 = 0; num14 < AppData.SE.Length; num14++)
				{
					apdat_.resSound_.sound[AppData.SE[num14]].SetSoundType(1);
				}
			}
			catch (Exception t2)
			{
				Log.Info(t2);
				if (Config.PRINT)
				{
					Log.Info("■BootForm.java load() リソースデータ読み込みでエラー");
				}
			}
			if (Config.DEBUG_CMD)
			{
				Log.Info("■BootForm.java load() loadCnt_ = " + loadCnt_);
			}
			TenantData.Load2();
			FanBonusData.Load2();
			Map.Load2();
			MapMain.Load2();
			TrainingData.Load2();
			MapMain.CreateMap();
			UserData.GetInstance().ProcCommonMethod();
			CreateAllClass();
		}

		public void UpdateScoreFromNetwork()
		{
			global::System.String originalUnusedSlot0;

byte[] bytes = apdat_.CgiGetScore("0,1");
			string text = StringUtil.Split(JString.New(bytes), "\n")[1];
			if (text.StartsWith(","))
			{
				text = "0" + text;
			}
			if (text.EndsWith(","))
			{
				text += '0';
			}
			if (text.IndexOf(",,") != -1)
			{
				int num = text.IndexOf(",,");
				text = StringUtil.Replace(text, ",,", ",0,");
			}
			string[] array = StringUtil.Split(text, ",");
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length() == 0 || JLong.ParseLong(array[i]) == 0L)
				{
					if (i == 0) {

						apdat_.sysSave_.lngs[0] = 0L;
						apdat_.sysSave_.lngs[1] = 0L;

}
else if (i == 1) {

						apdat_.sysSave_.lngs[2] = 5000L;
						apdat_.sysSave_.lngs[3] = 0L;

}
					continue;
				}
				long num2 = JLong.ParseLong(array[i]);
				if (i == 0) {

					apdat_.sysSave_.lngs[0] = num2;
					apdat_.sysSave_.lngs[1] = num2;
					apdat_.sysSave_.strs[0] = "[以前のスコア]";

}
else if (i == 1) {

					apdat_.sysSave_.lngs[2] = num2;
					apdat_.sysSave_.lngs[3] = num2;
					apdat_.sysSave_.strs[1] = "[以前のスコア]";

}
			}
			apdat_.SaveSystem();
		}

		public byte[][] DivServerData(string url)
		{
			byte[][] array = null;
			int num = 0;
			int i = 0;
			bool flag = true;
			DataInputStream dataInputStream = null;
			try
			{
				byte[] array2 = Storage.Read(3, false, url);
				dataInputStream = new DataInputStream(new ByteArrayInputStream(array2));
				num = array2.Length;
				Vector vector = new Vector();

int chunkLength;
byte[] array3;
				for (; i < num; i += array3.Length)
				{
					chunkLength = num - i;
					if (flag)
					{
						chunkLength = dataInputStream.ReadInt();
						i += 4;
					}
					array3 = new byte[chunkLength];
					dataInputStream.Read(array3);
					vector.AddElement(array3);
				}
				if (vector.Size() == 0) { array = JTool.MakeArray<byte[][]>(new int[2] { 1, 0 }); } else { array = new byte[vector.Size()][]; }
				for (int j = 0; j < vector.Size(); j++)
				{
					array[j] = (byte[])vector.ElementAt(j);
				}
			}
			catch (Exception t)
			{
				Log.Info(t);
			}
			finally
			{
				if (dataInputStream != null)
				{
					try
					{
						dataInputStream.Close();
					}
					catch (Exception)
					{
					}
				}
				dataInputStream = null;
			}
			return array;
		}

		private void CreateAllClass()
		{
			BaseData baseData = new BaseData();
			EventData eventData = new EventData();
			FanActData fanActData = new FanActData();
			FanBonusData fanBonusData = new FanBonusData();
			FormationData formationData = new FormationData();
			LeagueData leagueData = new LeagueData();
			MapchipMain mapchipMain = new MapchipMain();
			MapchipMatch mapchipMatch = new MapchipMatch();
			PlayerData playerData = new PlayerData();
			QuestMatchData questMatchData = new QuestMatchData();
			SponsorData sponsorData = new SponsorData();
			StadiumData stadiumData = new StadiumData();
			SupervisorData supervisorData = new SupervisorData();
			TeamData teamData = new TeamData();
			TenantData tenantData = new TenantData();
			TradeData tradeData = new TradeData();
			TrainingData trainingData = new TrainingData();
			BootForm bootForm = new BootForm();
			DebugForm debugForm = new DebugForm();
			FormManager formManager = new FormManager();
			GameForm gameForm = new GameForm();
			RankForm rankForm = new RankForm();
			SoccerForm soccerForm = new SoccerForm();
			SubForm subForm = new SubForm();
			TitleForm titleForm = new TitleForm();
			Ball ball = new Ball();
			Camera instance = Camera.GetInstance();
			DelayEvent delayEvent = new DelayEvent();
			GameManager gameManager = new GameManager();
			GameUtil gameUtil = new GameUtil();
			Map map = new Map();
			MapMain mapMain = new MapMain();
			MapObject mapObject = new MapObject();
			Player player = new Player(0);
			TArea tArea = new TArea();
			TBall tBall = new TBall(0);
			Team team = new Team();
			Tenant tenant = new Tenant(0);
			TGroupArea tGroupArea = new TGroupArea(0, null);
			TPlayer tPlayer = new TPlayer(1);
			UserData instance2 = UserData.GetInstance();
		}

		public override int GetTouchValue(int type, int index)
		{
			return 0;
		}

		public override void SetTouchValue(int type, int value)
		{
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
