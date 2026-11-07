using System;
using data;
using game;
using java.lang;
using java.util;
using kairo.common.cfg;
using kairo.unity.form;
using kairo.unity.graphics;
using kairo.unity.io;
using kairo.unity.m3d4.util3d;
using kairo.unity.surface;
using kairo.unity.ui;
using kairo.unity.util;
using main;
using surface;

namespace form
{
	public class GameForm : MyFormBase
	{
		public const int STATE_MAIN = 0;

		public const int STATE_BUILD = 1;

		public const int STATE_TOMATCH = 2;

		public const int STATE_BACK = 3;

		public const int STATE_SP_BACK = 4;

		public const int BUSGATHER_PH_MOVEBUS = 0;

		public const int BUSGATHER_PH_MOVEBUS_WINDOW = 1;

		public const int BUSGATHER_PH_BATTLE_PIC = 2;

		public const int BUSGATHER_PH_TALK_PNUM = 3;

		public const int BUSGATHER_PH_EDIT = 4;

		public const int BUSGATHER_ANIME_S1 = 10;

		public const int BUSGATHER_ANIME_S2 = 20;

		public const int BUSGATHER_ANIME_S3 = 30;

		public const int BUSGATHER_ANIME_S4 = 40;

		public const int BUSGATHER_ANIME_S5 = 50;

		public const int BUSGATHER_ANIME_SADD = 8;

		public const int SCR_EVENT = 0;

		public const int SCR_WAIT = 1;

		public const int SCR_SCENE = 2;

		public const int SCR_SPECIAL = 3;

		public const int TRAINING_PARAMUP_DIV = 3;

		public const int PINCH_SACLE_PADDING = 40;

		private const int CURSOR_HIDE_WAIT = 40;

		public const int LEAGUE_SCHEDULE_UPDATETIME = 12;

		public const int DLGID_TESTLIMIT = 105;

		public const int TBARTYPE_START = 0;

		public const int TBARTYPE_ICON = 1;

		public const int TBARTYPE_TIME = 2;

		public const int TBARTYPE_METER = 3;

		public const int TBARTYPE_MONEY = 4;

		public const int TBARTYPE_END = 5;

		private int errFunc_;

		private int errLine_;

		private UserData udat_;

		public int state_;

		public int stateCnt_;

		private static readonly int[][] softLabels_ = new int[5][]
		{
			new int[2] { 3, 4 },
			new int[2] { 0, 2 },
			new int[2],
			new int[2],
			new int[2]
		};

		public int busgather_ph;

		public static int busgather_ph_cnt;

		private new SubForm dialog_;

		private SubForm menu_;

		private int frame_;

		public TenantData selectTenantData_;

		public long proc_time;

		public long draw_time;

		public long all_time;

		public static int BOTTOM_INFO_BAR = 0;

		public static int VIEW_X = 0;

		public static int VIEW_Y = 23;

		public static int VIEW_W = 240;

		public static int VIEW_H = 240 - VIEW_Y - BOTTOM_INFO_BAR;

		public Vector script_;

		public static int[] TRAINING_PARAMUP_TIME;

		public static int TRAINING_GETEXP_TIME;

		public int offYear_;

		public int offMonth_;

		public int offHour_;

		public long offMoney_;

		public int offSeason_;

		public int scale_;

		public int pinchScaleValue_;

		public static Offscreen offGameScreen_;

		private static bool resetOffscreen_;

		public bool buildTrackX_;

		public bool buildTrachY_;

		public bool isBuildTrack_;

		public int buildDownX_;

		public int buildDownY_;

		public bool buildLock_;

		public int touchBuildDragFrame_;

		public bool touchSkip_;

		private int cursorPressCnt_;

		private int cursorHideWait_;

		public bool autoSave_;

		public static int DISP_COPAREA = 1;

		public static int[] MENU_BACK = new int[4] { 97, 217, 223, 128 };

		public static int FLG_PROCESSTIME = 1;

		public static int SHOW_FPS = 1;

		public static int SHOW_MEMORY = 0;

		public static int SHOW_PDTIME = 1;

		public static int SHOW_DRAWNUM = 0;

		public static int SHOW_CHUUSER = 0;

		public static int SHOW_DEBUGDRAW = 0;

		public static int SHOW_DEBUGDRAW2 = 0;

		public int LEAGUE_SCHEDULE_UPDATECNT;

		public static readonly int[] TBAR_SX = new int[6] { 0, 2, 22, 113, 118, 238 };

		public static readonly int[] TBAR_W = new int[6] { 2, 20, 91, 5, 120, 2 };

		private bool debugProStop_;

		private int offscreenDayTime_ = -1;

		private string offscreenDayTimeString_ = string.Empty;

		private long offscreenMoney_;

		private string offscreenMoneyString_ = "0";

		private int[] offUpdateSRect = new int[4];

		private bool flg_allDraw;

		public string build_btmMessage = "設置場所を選択して下さい。";

		public int build_btmCnt;

		private static int[] touch_cpos = new int[2];

		private static int[] touch_startDragPos = new int[2];

		private static int[] touch_startDragSPosCenter = new int[2];

		public Vector3D touchCameraSpeed_ = new Vector3D();

		public Vector3D touchCameraDecimal_ = new Vector3D();

		public FastVector touchCameraPoint_ = new FastVector();

		public bool touchDragCamera_;

		public bool touchBuildDrag_;

		private int[] getScaleRange_tmp = new int[2];

		public GameForm()
			: base(9)
		{
		}

		public override string ToString()
		{
			return "GameForm:" + state_ + "," + errFunc_ + "," + errLine_;
		}

		public override bool Init()
		{
			base.Init();
			udat_ = UserData.GetInstance();
			apdat_.camera.ChangeMode(0);
			menu_ = new SubForm(3, 0, 25);
			canvas_.DecideKeyState();
			if (apdat_.resMapChipMain_ != null)
			{
				apdat_.resMapChipMain_.Dispose();
			}
			apdat_.resMapChipMain_ = new ResourceManager(null, null, null, null, null, null, null);
			apdat_.resMapChipMain_.Load(RecordStore.ReadRecord(1, 8));
			apdat_.mapMain_.UpdateMapGround();
			ChangeState(0);
			SetAllDraw();
			scale_ = 100;
			pinchScaleValue_ = 120;
			InitOffscreen();
			script_ = new Vector();
			apdat_.PlayBgm(0);
			TRAINING_PARAMUP_TIME = new int[3];
			for (int i = 0; i < 3; i++)
			{
				TRAINING_PARAMUP_TIME[i] = 10800 * (i + 1) / 3;
			}
			TRAINING_GETEXP_TIME = 10800 / AppData.FRAME_TIME1 / 4 / 3;
			apdat_.pData_[0].SetGameData(udat_.tp_svisor_);
			int num = 0;
			if (num == 1)
			{
			}
			Vector3D[] array = new Vector3D[4]
			{
				new Vector3D(0f, 0f, 0f),
				new Vector3D(900f, 0f, 0f),
				new Vector3D(900f, 0f, 900f),
				new Vector3D(0f, 0f, 900f)
			};
			int[][] array2 = JTool.MakeArray<int[][]>(new int[2] { 4, 2 });
			int[][] array3 = JTool.MakeArray<int[][]>(new int[2] { 4, 2 });
			for (int j = 0; j < array.Length; j++)
			{
				Camera.ConvertGlobalToScreen(array[j], array2[j]);
			}
			for (int k = 0; k < array.Length; k++)
			{
				array3[k][0] = array2[k][0] - array2[0][0];
				array3[k][1] = array2[k][1] - array2[0][1];
			}
			Camera.UPX_FROMX = array3[1][0];
			Camera.UPX_FROMZ = array3[3][0];
			Camera.UPZ_FROMX = array3[1][1];
			Camera.UPZ_FROMZ = array3[3][1];
			Camera.OFF_SX = array2[0][0];
			Camera.OFF_SY = array2[0][1];
			Vector3D[] array4 = new Vector3D[4];
			for (int l = 0; l < array.Length; l++)
			{
				array4[l] = new Vector3D();
				Camera.ConvertScreenToGlobal(array2[l], array4[l]);
			}
			if (Config.ADMOB_ENABLE)
			{
				app_.SetOrientation(0);
			}
			else
			{
				app_.SetOrientation(apdat_.sysSave_.ints[13]);
			}
			SetAllDraw();
			return true;
		}

		public void GameScreenLayout(out int width, out int height, out Image.Format format, out Image.Filter filterMode)
		{
			width = view_.GetGameWidth() * 100 / 60 + 1;
			height = view_.GetGameHeight() * 100 / 60 + 1;
			format = Image.Format.ARGB32;
			filterMode = Image.Filter.Point;
		}

		public bool StaticRender(Graphics g, Offscreen offscreen)
		{
			apdat_.ClearZSortObjects();
			resetOffscreen_ = false;
			return true;
		}

		public bool RenderGameScreen(Graphics g, Offscreen offscreen)
		{
			errLine_ = 13;
			VIEW_W = GetGameWidth();
			VIEW_H = GetGameHeight();
			Camera.SetDrawCenter(VIEW_X, VIEW_Y, VIEW_W, VIEW_H);
			UpdateOffscreen(g);
			g.SetClip(VIEW_X, VIEW_Y, VIEW_W, VIEW_H);
			Seb.SetClip(VIEW_X, VIEW_Y, VIEW_W, VIEW_H);
			apdat_.ClearZSortObjects();
			apdat_.mapMain_.Draw(g, false);
			udat_.Draw_training(g);
			udat_.Draw_bus(g);
			for (int i = 0; i < udat_.tenants_.Length; i++)
			{
				udat_.tenants_[i].Draw(g);
			}
			apdat_.DrawZSortObjects(g, false);
			apdat_.DrawEffect(g);
			apdat_.mapMain_.Draw_tenantAround(g);
			errLine_ = 14;
			Seb.ClearClip();
			g.ClearClip();
			errLine_ = 15;
			return true;
		}

		public void ResetOffscreen()
		{
			SetAllDraw();
		}

		private void InitOffscreen()
		{
			offGameScreen_ = Offscreen.CreateOffscreen(RenderGameScreen, StaticRender, GameScreenLayout);
			apdat_.offSeason_ = -1;
			apdat_.offYear_ = -1;
			apdat_.offMonth_ = -1;
			apdat_.offWeek_ = -1;
			apdat_.offHour_ = -1;
			apdat_.offSec_ = -1;
			apdat_.offMoney_ = -2147483648L;
		}

		public void Draw_topBarParts(Graphics g, int type, int dx, int w)
		{
			{
if ((type == 0)) {
g.DrawImage(apdat_.resCom2_.img[26], dx, 0f, 0, 0, w, 24);

} else if ((type == 1)) {
g.DrawImage(apdat_.resCom2_.img[26], dx, 0f, 2, 0, w, 24);

} else if ((type == 2)) {
				if(w<=TBAR_W[type]){
					g.DrawImage(apdat_.resCom2_.img[26], dx, 0f, 22, 0, w, 24);
					}else{
				int num4 = w;
				int num5 = 0;
				while (num4 > 0)
				{
					int num6 = AppData.Clamp(num4, 1, TBAR_W[type]);
					g.DrawImage(apdat_.resCom2_.img[26], dx + num5, 0f, 22, 0, num6, 24);
					num4 -= num6;
					num5 += num6;
				}

			}} else if ((type == 3)) {
g.DrawImage(apdat_.resCom2_.img[26], dx, 0f, 113, 0, w, 24);

} else if ((type == 4)) {
				if(w<=TBAR_W[type]){
					g.DrawImage(apdat_.resCom2_.img[26], dx, 0f, 118, 0, w, 24);
					}else{
				int num = w;
				int num2 = 0;
				while (num > 0)
				{
					int num3 = AppData.Clamp(num, 1, TBAR_W[type]);
					g.DrawImage(apdat_.resCom2_.img[26], dx + num2, 0f, 118, 0, num3, 24);
					num -= num3;
					num2 += num3;
				}

			}} else if ((type == 5)) {
g.DrawImage(apdat_.resCom2_.img[26], dx, 0f, 238, 0, w, 24);

}

}
		}

		public int GetTopBarOffT()
		{
			int result = 0;
			if (GameView.GetInstance().GetGameWidth() > 240)
			{
				result = 30;
			}
			return result;
		}

		public int GetTopBarOffM()
		{
			int result = 0;
			if (GameView.GetInstance().GetGameWidth() > 240)
			{
				result = 30;
			}
			return result;
		}

		public int GetTopBarOffMBack()
		{
			int result = 0;
			if (GameView.GetInstance().GetGameWidth() > 240)
			{
				result = 30;
			}
			return result;
		}

		public int[] GetFirstScript()
		{
			if (script_.Size() == 0)
			{
				return null;
			}
			return (int[])script_.FirstElement();
		}

		public void ClearScript()
		{
			script_.RemoveAllElements();
		}

		public void AddCommandWait(int wait)
		{
			script_.AddElement(new int[2] { 1, wait });
		}

		public void AddCommandEvent(int evtId)
		{
			script_.AddElement(new int[2] { 0, evtId });
		}

		public void AddCommandScene(int on)
		{
			script_.AddElement(new int[2] { 2, on });
		}

		public void AddCommandSpecial(int[] arg)
		{
			if (arg == null)
			{
				arg = new int[0];
			}
			int[] array = new int[arg.Length + 1];
			array[0] = 3;
			if (arg.Length > 0)
			{
				JSystem.Arraycopy(arg, 0, array, 1, arg.Length);
			}
			script_.AddElement(array);
		}

		public void Script()
		{
			while (script_.Size() > 0)
			{
				int[] firstScript = GetFirstScript();
				switch (firstScript[0])
				{
				case 1:
					Push(SubForm.MakeWait(firstScript[1]));
					script_.RemoveElementAt(0);
					return;
				case 0:
					apdat_.ExecEvent(firstScript[1], apdat_.save_.strs[0], null);
					script_.RemoveElementAt(0);
					return;
				case 2:
					break;
				case 3:
					script_.RemoveElementAt(0);
					return;
				default:
					return;
				}
				script_.RemoveElementAt(0);
			}
		}

		public override bool Resume()
		{
			return true;
		}

		public override bool Finish()
		{
			if (apdat_.resMapChipMain_ != null)
			{
				apdat_.resMapChipMain_.Dispose();
			}
			apdat_.resMapChipMain_ = null;
			if (apdat_.resMapChip_ != null)
			{
				apdat_.resMapChip_.Dispose();
			}
			apdat_.resMapChip_ = null;
			if (offGameScreen_ != null)
			{
				offGameScreen_.Dispose();
				offGameScreen_ = null;
			}
			udat_.Training_init();
			if (apdat_.camera.subPlayer_ != null)
			{
				apdat_.camera.subPlayer_ = null;
			}
			if (apdat_.camera.subSvisor_ != null)
			{
				apdat_.camera.subSvisor_ = null;
			}
			JSystem.Gc();
			return true;
		}

		public void ChangeState(int state)
		{
			int[] array = softLabels_[state];
			if (array != null)
			{
				SetSoftLabel(AppData.softLabels_[array[0]], AppData.softLabels_[array[1]]);
			}
			state_ = state;
			stateCnt_ = 0;
			if (state_ == 0) { } else {

			if (state_ == 1)
			{
				SetBuildBtmMessage("設置場所を選択して下さい。", 0);
			}
			else if (state_ == 2)
			{
				if (apdat_.camera.subPlayer_ != null)
				{
					apdat_.camera.subPlayer_ = null;
				}
				if (apdat_.camera.subSvisor_ != null)
				{
					apdat_.camera.subSvisor_ = null;
				}
			}
}
		}

		public override bool Update()
		{
			frame_ = (frame_ + 1) % int.MaxValue;
			if (Config.ADMOB_ENABLE && apdat_.sysSave_.ints[13] != 1)
			{
				apdat_.sysSave_.ints[13] = 1;
			}
			if (Config.TRIAL_VERSION && dialog_ != null)
			{
				if (dialog_.dialogId_ == 105)
				{
					formMan_.RemoveAllForms();
					string packageName = IApplication.GetCurrentApp().GetPackageName();
					packageName = packageName.SubstringJ(0, packageName.Length() - 1);
					Canvas.OpenBrowser("market://details?id=" + packageName + "&referrer=utm_source%3Dappli%26utm_medium%3Dhouse_ad%26utm_content%3D" + packageName + "_lite%26utm_campaign%3Dbanner");
					return true;
				}
				dialog_ = null;
			}
			int num = 1;
			if (apdat_.save_.ints[13] == 1)
			{
				num = 2;
			}
			if (state_ != 0)
			{
				num = 1;
			}
			if (Config.DEBUG_CMD || Config.PTW)
			{
				if (canvas_.CheckKeyState(16))
				{
					num = 2;
				}
				else if (canvas_.CheckKeyState(32))
				{
					num = 20;
				}
				else if (canvas_.CheckKeyState(64))
				{
					num = 10;
				}
				if (canvas_.CheckKeyRepeat(16) && canvas_.CheckKeyRepeat(32))
				{
				}
			}
			if (Config.DEBUG_CMD)
			{
				if (canvas_.CheckKeyRepeat(2))
				{
					SHOW_DEBUGDRAW = 1 - SHOW_DEBUGDRAW;
				}
				if (canvas_.CheckKeyRepeat(4))
				{
					SHOW_DEBUGDRAW2 = 1 - SHOW_DEBUGDRAW2;
				}
			}
			errFunc_ = 1;
			errLine_ = 0;
			if (state_ == 0)
			{
				apdat_.camera.Update_pos();
			}
			for (int i = 0; (i < num); i++)
{
if ((_update(i))) { break; }

}
			errFunc_ = -1;
			return true;
		}

		private bool _update(int frameCount)
		{
			stateCnt_ = (stateCnt_ + 1) % int.MaxValue;
			if (Config.DEBUG_CMD && canvas_.CheckKeyPulse(1))
			{
				return formMan_.Push(new DebugForm());
			}
			apdat_.UpdateEffect();
			if (Config.DEBUG_CMD && canvas_.CheckKeyRepeat(8))
			{
				apdat_.ExecEvent(100, null, null);
			}
			if (state_ == 0)
			{
				if (stateCnt_ == 1)
				{
					SetAllDraw();
				}
				if (apdat_.save_.lngs[0] < 0)
				{
				}
				for (int i = 0; i < apdat_.eventData_.Length; i++)
				{
					if (((apdat_.eventData_[i] == null) || (apdat_.CheckAlreadyEvent(apdat_.eventData_[i].id_)))) { continue; }
if ((apdat_.CheckTermScript(apdat_.eventData_[i].termsScript_))) {
apdat_.ExecEvent(apdat_.eventData_[i].id_, null, null);
return false;
}
				}
				for (int j = 0; j < apdat_.delayEvents_.Size(); j++)
				{
					DelayEvent delayEvent = (DelayEvent)apdat_.delayEvents_.ElementAt(j);
					delayEvent.wait_--;
					if (delayEvent.wait_ <= 0)
					{
						apdat_.delayEvents_.RemoveElementAt(j);
						j--;
						apdat_.save_.ints[9] = delayEvent.rideChara_;
						apdat_.ExecuteScript(apdat_.GetDelayScript(delayEvent.delayId_), true, delayEvent.args_, delayEvent.argi_, delayEvent.line_);
						return false;
					}
				}
				if (FanActData.GetFanEva(1) == 1000 && !apdat_.CheckAlreadyEvent(177))
				{
					apdat_.ExecEvent(177, null, null);
				}
			}
			if (state_ == 0)
			{
				udat_.Update_bus();
				udat_.Update_training();
				for (int k = 0; k < apdat_.fanActData_.Length; k++)
				{
					apdat_.fanActData_[k].Update();
				}
				for (int l = 0; l < apdat_.tenantData_.Length; l++)
				{
					apdat_.tenantData_[l].Update();
				}
				for (int m = 0; m < udat_.tenants_.Length; m++)
				{
					udat_.tenants_[m].Update();
				}
				apdat_.mapMain_.UpdateMapGround();
				if (LEAGUE_SCHEDULE_UPDATECNT > 0)
				{
					LEAGUE_SCHEDULE_UPDATECNT--;
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					SubForm subForm = OepnWindowFromMainDisp();
					if (subForm != null)
					{
						return Push(subForm);
					}
				}
				else
				{
					if (apdat_.IsPushSoftLabel(4))
					{
						return Push(menu_);
					}
					if (apdat_.IsPushSoftLabel(3))
					{
						return Push(new SubForm(16));
					}
				}
			}
			else if (state_ == 1)
			{
				if (build_btmCnt > 0)
				{
					build_btmCnt--;
					if (build_btmCnt == 0)
					{
						SetBuildBtmMessage("設置場所を選択して下さい。", 0);
					}
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					bool flag = false;
					int num = 0;
					for (int n = 0; n < udat_.tenants_.Length; n++)
					{
						Tenant tenant = udat_.tenants_[n];
						if ((Camera.camera_address[0] == tenant.address_[0] || Camera.camera_address[0] == tenant.address_[0] + 1) && (Camera.camera_address[1] == tenant.address_[1] || Camera.camera_address[1] == tenant.address_[1] - 1))
						{
							flag = true;
							num = n;
							break;
						}
					}
					if (flag)
					{
						Tenant tenant2 = udat_.tenants_[num];
						if (tenant2.state_ == 0)
						{
							tenant2.BuildTenant(selectTenantData_.id_);
							apdat_.mapMain_.UpdateTenantMapcip();
							ChangeState(0);
						}
						else
						{
							SetBuildBtmMessage("既に施設が設置してあります。", 30);
							apdat_.frmGame_.SetAllDraw();
						}
					}
				}
				else if (apdat_.IsPushSoftLabel(2))
				{
					ChangeState(0);
				}
			}
			else if (state_ == 2)
			{
				if (busgather_ph == 1)
				{
					busgather_ph_cnt++;
					apdat_.resCom2_.seb[60].Frame();
					if (busgather_ph_cnt >= udat_.memberPDataist_.Size() * 12 + 20 || canvas_.CheckKeyPulse(1048576))
					{
						SetBattleData();
						int testedMatchInfoJoin = udat_.GetMatchInfoJoin();
						if (testedMatchInfoJoin == 0 && !apdat_.CheckAlreadyEvent(176))
						{
							apdat_.ExecEvent(176, null, null);
						}
					}
				}
				else if (busgather_ph == 2)
				{
					busgather_ph_cnt++;
					if (canvas_.CheckKeyPulse(1048576))
					{
						apdat_.camera.SaveMapMainBasePos();
						formMan_.ChangeCurrentForm(apdat_.frmSoccer_);
					}
				}
			}
			else if (state_ == 3 && stateCnt_ >= 10 && canvas_.CheckKeyPulse(1048576))
			{
				ChangeState(0);
			}
			apdat_.UpdatePopupMessage();
			if (apdat_.IsPushSoftLabel(4))
			{
				return Push(menu_);
			}
			int add = AppData.FRAME_TIME1;
			if (state_ != 0)
			{
				add = 0;
			}
			ProgressTime(add, frameCount);
			return false;
		}

		public void UpdateOffscreenTopBar(Graphics g, bool all_update)
		{
			Font font = g.GetFont(null);
			int topBarOffT = GetTopBarOffT();
			int topBarOffM = GetTopBarOffM();
			int topBarOffMBack = GetTopBarOffMBack();
			if (all_update)
			{
				apdat_.offYear_ = -1;
				apdat_.offMonth_ = -1;
				apdat_.offHour_ = -1;
				apdat_.offSec_ = -1;
				apdat_.offMoney_ = -1L;
				apdat_.offWeatherAlpha_ = -1;
				apdat_.offSeason_ = -1;
				apdat_.offWeek_ = -1;
				apdat_.offMoney_ = -2147483648L;
			}
			int num = 0;
			Draw_topBarParts(g, 0, 0, TBAR_W[0]);
			num += TBAR_W[0];
			if (topBarOffT > 0)
			{
				Draw_topBarParts(g, 2, num, topBarOffT);
				num += topBarOffT;
			}
			Draw_topBarParts(g, 1, num, TBAR_W[1]);
			num += TBAR_W[1];
			Draw_topBarParts(g, 2, num, TBAR_W[2]);
			num += TBAR_W[2];
			Draw_topBarParts(g, 3, num, TBAR_W[3]);
			num += TBAR_W[3];
			int num2 = AppData.ClampMin(view_.GetGameWidth() - topBarOffMBack - TBAR_W[4], num);
			if (num2 > num)
			{
				Draw_topBarParts(g, 2, num, num2 - num);
				num = num2;
			}
			Draw_topBarParts(g, 4, num, TBAR_W[4]);
			num += TBAR_W[4];
			if (topBarOffMBack == 0)
			{
				Draw_topBarParts(g, 5, view_.GetGameWidth() - TBAR_W[5], TBAR_W[5]);
			}
			else
			{
				int num3 = view_.GetGameWidth() - num;
				if (num3 > 0)
				{
					Draw_topBarParts(g, 2, num, num3);
				}
			}
			apdat_.resCom2_.seb[23].DrawFrame(g, 50 + topBarOffT, 6f, apdat_.resCom2_.img, 10);
			apdat_.resCom2_.seb[23].DrawFrame(g, 78 + topBarOffT, 6f, apdat_.resCom2_.img, 11);
			apdat_.resCom2_.seb[23].DrawFrame(g, 100 + topBarOffT, 6f, apdat_.resCom2_.img, 12);
			int season = GetSeason(apdat_.save_.ints[2]);
			if (apdat_.offSeason_ != season)
			{
				apdat_.offSeason_ = season;
				g.DrawImage(apdat_.resCom2_.img[26], 2 + topBarOffT, 2f, 2, 2, 20, 20);
				apdat_.DrawSeb(g, 2 + topBarOffT + 1, 3, apdat_.resCom2_.seb[84], apdat_.resCom2_.img[95], apdat_.offSeason_);
			}
			if (apdat_.offYear_ != apdat_.save_.ints[1] + 1)
			{
				apdat_.offYear_ = apdat_.save_.ints[1] + 1;
				if (MyFormBase.Jp())
				{
					g.DrawImage(apdat_.resCom2_.img[26], 24 + topBarOffT, 6f, 24, 6, 24, 12);
					apdat_.DrawNumImage(g, 23, string.Empty, apdat_.offYear_, string.Empty, 48 + topBarOffT, 6, 0, 4);
				}
				else
				{
					g.DrawImage(apdat_.resCom2_.img[26], 24 + topBarOffT, 6f, 24, 6, 24, 12);
					apdat_.DrawNumImage(g, 23, string.Empty, apdat_.offYear_, string.Empty, 48 + topBarOffT, 6, 0, 4);
				}
			}
			if (apdat_.offMonth_ != apdat_.save_.ints[2] + 1)
			{
				apdat_.offMonth_ = apdat_.save_.ints[2] + 1;
				if (MyFormBase.Jp())
				{
					g.DrawImage(apdat_.resCom2_.img[26], 62 + topBarOffT, 6f, 62, 6, 16, 12);
					apdat_.DrawNumImage(g, 23, string.Empty, apdat_.offMonth_, string.Empty, 78 + topBarOffT, 6, 0, 4);
				}
				else
				{
					g.DrawImage(apdat_.resCom2_.img[26], 62 + topBarOffT, 6f, 62, 6, 16, 12);
					apdat_.DrawNumImage(g, 23, string.Empty, apdat_.offMonth_, string.Empty, 78 + topBarOffT, 6, 0, 4);
				}
			}
			if (apdat_.offWeek_ != apdat_.save_.ints[3] + 1)
			{
				apdat_.offWeek_ = apdat_.save_.ints[3] + 1;
				if (MyFormBase.Jp())
				{
					g.DrawImage(apdat_.resCom2_.img[26], 91 + topBarOffT, 6f, 91, 6, 8, 12);
					apdat_.DrawNumImage(g, 23, string.Empty, apdat_.offWeek_, string.Empty, 99 + topBarOffT, 6, 0, 4);
				}
				else
				{
					g.DrawImage(apdat_.resCom2_.img[26], 91 + topBarOffT, 6f, 91, 6, 8, 12);
					apdat_.DrawNumImage(g, 23, string.Empty, apdat_.offWeek_, string.Empty, 99 + topBarOffT, 6, 0, 4);
				}
			}
			if (offscreenMoney_ != apdat_.save_.lngs[0])
			{
				offscreenMoneyString_ = AppData.CommaSeparate(apdat_.save_.lngs[0]);
				offscreenMoney_ = apdat_.save_.lngs[0];
			}
			if (apdat_.offMoney_ != apdat_.save_.lngs[0] || flg_allDraw)
			{
				apdat_.offMoney_ = apdat_.save_.lngs[0];
				int num4 = AppData.ClampMin(view_.GetGameWidth() - topBarOffMBack - TBAR_W[4], 121);
				g.DrawImage(apdat_.resCom2_.img[26], num4, 4f, 119, 4, 117, 15);
				if (apdat_.save_.lngs[0] >= 0)
				{
					g.SetColor(Graphics.GetColorOfRGB(20, 29, 49));
				}
				else
				{
					g.SetColor(Graphics.GetColorOfRGB(255, 0, 0));
				}
				string text = MyFormBase.YEN("万円", apdat_.save_.lngs[0]);
				if (MyFormBase.Jp())
				{
					g.DrawString(text, view_.GetGameWidth() - topBarOffM - TBAR_W[5] - font.StringWidth(text), 6f);
				}
				else
				{
					g.DrawString(text, view_.GetGameWidth() - topBarOffM - TBAR_W[5] - font.StringWidth(text) - 5, 6f);
				}
			}
		}

		private void Draw_watch(Graphics g, int cx, int cy, double len, int angle)
		{
			double a = JMath.ToRadians(angle);
			double num = JMath.Cos(a);
			double num2 = JMath.Sin(a);
			if (len == (double)JMath.Abs((int)(len * num)))
			{
				num *= 0.999;
			}
			if (len == (double)JMath.Abs((int)(len * num2)))
			{
				num2 *= 0.999;
			}
			g.SetColor(Graphics.GetColorOfRGB(0, 0, 0));
			g.DrawLine(cx, cy, cx + (int)(len * num), cy - (int)(len * num2));
		}

		public void UpdateOffscreen(Graphics g)
		{
			int num = 0;
			if (Camera.old_pos != Camera.pos || Camera.old_pos != Camera.pos)
			{
				num = 1;
				if (DISP_COPAREA == 0)
				{
					num = 2;
				}
			}
			if (flg_allDraw)
			{
				flg_allDraw = false;
				num = 2;
			}
			int vIEW_W = VIEW_W;
			int vIEW_H = VIEW_H;
			VIEW_W = GetGameWidth();
			VIEW_H = GetGameHeight();
			g.ClipRect(VIEW_X, VIEW_Y, VIEW_W, VIEW_H);
			Seb.SetClip(VIEW_X, VIEW_Y, VIEW_W, VIEW_H);
			apdat_.ClearZSortObjects();
			num = 2;
			int colorOfRGB = Graphics.GetColorOfRGB(128, 210, 255);
			if (num == 2)
			{
				g.SetColor(colorOfRGB);
				g.FillRect(VIEW_X, VIEW_Y, VIEW_W, VIEW_H);
				apdat_.mapMain_.Draw(g, true);
				Camera.old_pos.x_ = Camera.pos.x_;
				Camera.old_pos.y_ = Camera.pos.y_;
			}
			else
			{
				g.SetColor(Graphics.GetColorOfRGB(0, 0, 255));
				int num2 = -(int)(Camera.pos.x_ - Camera.old_pos.x_);
				int num3 = (int)(Camera.pos.y_ - Camera.old_pos.y_);
				Camera.old_pos.x_ = Camera.pos.x_;
				Camera.old_pos.y_ = Camera.pos.y_;
				g.CopyArea(VIEW_X, VIEW_Y, VIEW_W, VIEW_H, num2, num3);
				for (int i = 0; i < offUpdateSRect.Length; i++)
				{
					offUpdateSRect[i] = 0;
				}
				if (num2 > 0)
				{
					offUpdateSRect[3] = num2;
				}
				else
				{
					offUpdateSRect[1] = -num2;
				}
				if (num3 > 0)
				{
					offUpdateSRect[0] = num3;
				}
				else
				{
					offUpdateSRect[2] = -num3;
				}
				if (offUpdateSRect[0] > 0)
				{
					g.SetColor(colorOfRGB);
					g.FillRect(0f, VIEW_Y, VIEW_W, offUpdateSRect[0]);
				}
				if (offUpdateSRect[1] > 0)
				{
					g.SetColor(colorOfRGB);
					g.FillRect(VIEW_W - offUpdateSRect[1], VIEW_Y, offUpdateSRect[1], VIEW_H);
				}
				if (offUpdateSRect[2] > 0)
				{
					g.SetColor(colorOfRGB);
					g.FillRect(0f, VIEW_Y + VIEW_H - offUpdateSRect[2], VIEW_W, offUpdateSRect[2]);
				}
				if (offUpdateSRect[3] > 0)
				{
					g.SetColor(colorOfRGB);
					g.FillRect(VIEW_X, VIEW_Y, offUpdateSRect[3], VIEW_H);
				}
				apdat_.mapMain_.Draw_copyArea(g, offUpdateSRect);
			}
			apdat_.DrawZSortObjects(g, true);
			Seb.ClearClip();
			g.PopClip();
			VIEW_W = vIEW_W;
			VIEW_H = vIEW_H;
		}

		public void SetAllDraw()
		{
			flg_allDraw = true;
			resetOffscreen_ = true;
		}

		public override void Draw(Graphics g)
		{
			VIEW_W = view_.GetGameWidth() - VIEW_X;
			VIEW_H = view_.GetGameHeight() - VIEW_Y;
			errFunc_ = 2;
			errLine_ = 0;
			int num = AppData.Clamp(scale_, GetScaleMin(), GetScaleMax() + 40);
			if (num != scale_)
			{
				SetScale(num);
			}
			try
			{
				_draw(g);
			}
			catch (Exception ex)
			{
				Log.Info(ex);
				throw ex;
			}
			errFunc_ = -1;
		}

		public void _draw(Graphics g)
		{
			errLine_ = 10;
			int num2;
			int num3;
			int num = (num2 = (num3 = 0));
			Camera.SetDrawCenter(VIEW_X, VIEW_Y, VIEW_W, VIEW_H);
			g.SetColor(Graphics.GetColorOfRGB(50, 205, 255));
			g.FillRect(VIEW_X, VIEW_Y, VIEW_W, VIEW_H);
			g.DrawScaledImage(offGameScreen_.Image, VIEW_X, VIEW_Y, VIEW_W, VIEW_H, VIEW_X, VIEW_Y, GetGameWidth(), GetGameHeight());
			errLine_ = 15;
			if (view_.CheckFirstTouch(17))
			{
				GameView.AddTouch(g, 17, -1000, -1000, 2000, 2000, 0, TouchOption.Drag(1, 1).Flag(40));
			}
			else
			{
				GameView.AddTouch(g, 17, VIEW_X, VIEW_Y, VIEW_W, VIEW_H, 0, TouchOption.Drag(1, 1));
			}
			if (state_ == 0)
			{
				GameView.AddTouch(g, 4, VIEW_X, -200, VIEW_W, 200 + VIEW_Y + 10, 22, TouchOption.Create(32));
			}
			apdat_.camera.DrawCursor(g);
			errLine_ = 16;
			num3 = VIEW_Y + VIEW_H;
			if (state_ == 0)
			{
				errLine_ = 17;
				if (apdat_.camera.subPlayer_ != null)
				{
					errLine_ = 18;
					int sideSoftWidth = view_.GetSideSoftWidth();
					int num4 = view_.GetGameWidth() - view_.GetSideSoftWidth() * 2;
					TPlayer subPlayer_ = apdat_.camera.subPlayer_;
					PlayerData playerData = apdat_.pData_[subPlayer_.pData_id_];
					int num5 = sideSoftWidth;
					g.DrawImage(apdat_.resCom2_.img[141], sideSoftWidth, num3 - 25, 0, 0, 127, 25);
					g.DrawImage(apdat_.resCom2_.img[141], sideSoftWidth + num4 - 3, num3 - 25, 237, 0, 3, 25);
					num5 += 127;
					int num6 = num4 - 130;
					while (num6 > 0)
					{
						int num7 = AppData.Clamp(num6, 1, 110);
						g.DrawImage(apdat_.resCom2_.img[141], num5, num3 - 25, 127, 0, num7, 25);
						num6 -= num7;
						num5 += num7;
					}
					g.ClipRect(sideSoftWidth + 4, num3 - 20, 15f, 15f);
					udat_.SetDispPlayerData(0, udat_.selectUniform_, playerData.faceImg_, 0, 1);
					udat_.DrawDispPlayer(g, sideSoftWidth + 4 + 8, num3);
					g.PopClip();
					g.SetColor(Graphics.GetColorOfRGB(249, 232, 223));
					g.DrawString(playerData.name_ + " " + playerData.name2_, sideSoftWidth + 27, num3 - 18);
					g.DrawImage(apdat_.resCom2_.img[126], sideSoftWidth + 129, num3 - 17);
					apdat_.DrawNumImage(g, 79, playerData.lv_, sideSoftWidth + 129 + 35 - 4, num3 - 19, 0, 2);
					errLine_ = 19;
				}
				errLine_ = 20;
			}
			else if (state_ == 1)
			{
				int sideSoftWidth2 = view_.GetSideSoftWidth();
				apdat_.Draw_helpMessage(g, build_btmMessage, sideSoftWidth2, num3 - 22);
			}
			errLine_ = 21;
			UpdateOffscreenTopBar(g, true);
			int topBarOffT = GetTopBarOffT();
			int num8 = GameUtil.RateConvert(apdat_.save_.ints[4], 0, 10800, 0, 10, true);
			g.SetColor(Graphics.GetColorOfRGB(255, 255, 0));
			g.FillRect(114 + topBarOffT, 2 + num8 * 2, 3f, 1f);
			g.DrawImage(apdat_.resCom2_.img[143], view_.GetGameWidth() - 178, VIEW_Y);
			num = VIEW_Y;
			apdat_.DrawNumImage(g, 35, udat_.researchPoint_[0], view_.GetGameWidth() - 136, num + 3, 0, 4);
			apdat_.DrawNumImage(g, 35, udat_.researchPoint_[1], view_.GetGameWidth() - 136 + 44, num + 3, 0, 4);
			apdat_.DrawNumImage(g, 35, udat_.researchPoint_[2], view_.GetGameWidth() - 136 + 88, num + 3, 0, 4);
			apdat_.DrawNumImage(g, 35, udat_.researchPoint_[3], view_.GetGameWidth() - 136 + 132, num + 3, 0, 4);
			errLine_ = 22;
			int matchInfoJoin = udat_.GetMatchInfoJoin();
			if (matchInfoJoin != 2)
			{
				num = 42;
				g.DrawImage(apdat_.resCom2_.img[144], view_.GetGameWidth() - 89, num - 3);
				int num9 = GameUtil.RateConvert(LEAGUE_SCHEDULE_UPDATECNT, 12, 0, -12, 0, true);
				int num10 = 0;
				switch (matchInfoJoin)
				{
				case 0:
				{
					LeagueData leagueData = apdat_.leagueData_[udat_.selectLeagueId_];
					num10 = leagueData.schedule_ - udat_.matchInfoWeek_;
					break;
				}
				case 1:
				{
					QuestMatchData questMatchData = apdat_.qmData_[udat_.selectQmId_];
					num10 = questMatchData.schedule_ - udat_.matchInfoWeek_;
					break;
				}
				}
				int num11 = view_.GetGameWidth() - 25;
				if (MyFormBase.En())
				{
					num11 += -53;
				}
				if (AppData.IsLanguageJpBase(false))
				{
					num11 = view_.GetGameWidth() - 25;
				}
				g.ClipRect(num11 - 5, num, 16f, 13f);
				apdat_.DrawNumImage(g, 82, num10, num11, num + num9, 0, 1);
				if (num9 != 0)
				{
					apdat_.DrawNumImage(g, 82, num10 + 1, num11, num + num9 + 12, 0, 1);
				}
				g.PopClip();
			}
			errLine_ = 23;
			if (state_ == 2)
			{
				if (busgather_ph == 1)
				{
					SubForm.Draw_moveToBus(g, frame_);
				}
				else if (busgather_ph == 2)
				{
					SubForm.Draw_battlePic(g, frame_);
				}
			}
			else if (state_ == 0)
			{
				int num12 = VIEW_Y + VIEW_H;
				if (apdat_.camera.subPlayer_ != null)
				{
					num12 -= 25;
				}
				errLine_ = 24;
				LogOut();
				apdat_.DrawPopupMessage(g, num12);
				errLine_ = 25;
				LogOut();
			}
			else if (state_ == 3)
			{
				int num13 = 0;
				num = 0;
				for (int i = 0; i < view_.GetGameHeight() / 40 + 1; i++)
				{
					for (int j = 0; j < view_.GetGameWidth() / 120 + 1; j++)
					{
						num13 = 120 * j;
						num = 40 * i;
						int width = AppData.ClampMax(120, view_.GetGameWidth() - num13);
						int height = AppData.ClampMax(40, view_.GetGameHeight() - num);
						g.DrawImage(apdat_.resCom2_.img[89], num13, num, 0, 0, width, height);
					}
				}
			}
			FormBase topForm = formMan_.GetTopForm();
			if (topForm.GetId() == 4)
			{
				SubForm subForm = (SubForm)topForm;
				if (subForm.IsMenu())
				{
					g.SetColor(Graphics.GetColorOfRGB(MENU_BACK[0], MENU_BACK[1], MENU_BACK[2], MENU_BACK[3]));
					g.FillRect(VIEW_X, VIEW_Y, VIEW_W, VIEW_H + BOTTOM_INFO_BAR);
					g.SetColor(Graphics.GetColorOfRGB(0, 0, 0, 255));
				}
			}
			errLine_ = 26;
			errLine_ = 27;
			if (Config.DEBUG_CMD && UserData.DEBUG_DISPFPS == 1)
			{
				apdat_.DrawNumImage(g, 23, string.Empty, FormManager.GetInstance().GetFps(), string.Empty, 5, 25, 0);
				apdat_.DrawNumImage(g, 23, string.Empty, proc_time, string.Empty, 5, 40, 0);
				apdat_.DrawNumImage(g, 23, string.Empty, draw_time, string.Empty, 5, 55, 0);
			}
			errLine_ = 28;
			if (Config.DEBUG_CMD && SHOW_DEBUGDRAW2 == 1)
			{
				if (SHOW_FPS == 1)
				{
					apdat_.DrawNumImage(g, 23, string.Empty, FormManager.GetInstance().GetFps(), string.Empty, 5, 25, 0);
				}
				if (SHOW_MEMORY == 1)
				{
					apdat_.DrawNumImage(g, 23, string.Empty, JRuntime.GetRuntime().FreeMemory() / 1024, string.Empty, 10, 195, 0);
					apdat_.DrawNumImage(g, 23, string.Empty, JRuntime.GetRuntime().TotalMemory() / 1024, string.Empty, 10, 210, 0);
				}
				if (SHOW_PDTIME == 1)
				{
					apdat_.DrawNumImage(g, 23, string.Empty, proc_time, string.Empty, 5, 40, 0);
					apdat_.DrawNumImage(g, 23, string.Empty, draw_time, string.Empty, 5, 55, 0);
				}
				if (SHOW_DRAWNUM == 1)
				{
					apdat_.DrawNumImage(g, 23, string.Empty, Config.drawImageCount_, string.Empty, 5, 70, 0);
				}
			}
			errLine_ = 29;
			errLine_ = 35;
		}

		public void ProgressTime(int add, int frameCount)
		{
			int num = apdat_.save_.ints[4];
			int num2 = apdat_.save_.ints[1];
			int num3 = apdat_.save_.ints[2];
			int num4 = apdat_.save_.ints[3];
			int num5 = TRAINING_PARAMUP_TIME.Length - 1;
			for (int i = 0; i < TRAINING_PARAMUP_TIME.Length; i++)
			{
				if (num <= TRAINING_PARAMUP_TIME[i])
				{
					num5 = i;
					break;
				}
			}
			apdat_.save_.ints[5] = apdat_.save_.ints[4];
			apdat_.save_.ints[4] += add;
			if (apdat_.save_.ints[4] >= 10800)
			{
				int num6 = apdat_.save_.ints[4];
				apdat_.save_.ints[4] = apdat_.save_.ints[5];
				apdat_.SaveGame(true);
				apdat_.save_.ints[4] = num6;
			}
			while (apdat_.save_.ints[4] >= 10800)
			{
				apdat_.save_.ints[3]++;
				apdat_.save_.ints[4] -= 10800;
				if (apdat_.save_.ints[3] >= 4)
				{
					apdat_.save_.ints[3] = 0;
					apdat_.save_.ints[2]++;
					if (apdat_.save_.ints[2] >= 12)
					{
						apdat_.save_.ints[2] = 0;
						apdat_.save_.ints[1]++;
					}
				}
			}
			if (num2 != apdat_.save_.ints[1])
			{
				udat_.RotateTestPlayParam();
				udat_.RecordReset();
				udat_.CustomerNumReset();
				apdat_.AddPopupMessage(36, null);
			}
			if (num3 != apdat_.save_.ints[2])
			{
				JSystem.Gc();
				if (Config.TRIAL_VERSION && apdat_.save_.ints[1] + 1 >= 2 && apdat_.save_.ints[2] + 1 >= 1)
				{
					Push(SubForm.MakeTalkForm(0, MyFormBase.LT("おつかれさまでした。<br>ここで体験版は終了となります"), null));
					Push(SubForm.MakeTalkForm(0, MyFormBase.LT("なお製品版では年数制限なく心ゆくまでゲームが遊べ、素敵なサッカーチームを運営することができます。"), null));
					Push(SubForm.MakeTalkForm(0, MyFormBase.LT("このあと正式版のサイトにアクセスすることができます。"), null));
					dialog_ = SubForm.MakeTalkForm(0, MyFormBase.LT("それではまた会いましょう。"), null);
					dialog_.dialogId_ = 105;
					Push(dialog_);
					return;
				}
				if (apdat_.save_.ints[1] == 7 && apdat_.save_.ints[2] == 3)
				{
					apdat_.ExecEvent(3, 7 + string.Empty, null);
					apdat_.Push(new SubForm(19));
					apdat_.save_.ints[14] = 1;
					apdat_.sysSave_.ints[9] = 1;
					udat_.SetContinueGameData();
				}
				apdat_.svisorData_[udat_.selectSupervisorId_].workMonth_++;
				int num7 = 0;
				SupervisorData supervisorData = apdat_.svisorData_[udat_.selectSupervisorId_];
				num7 += supervisorData.GetSallay();
				apdat_.SubMoney(supervisorData.GetSallay(), 3);
				for (int j = 0; j < udat_.tenants_.Length; j++)
				{
					Tenant tenant = udat_.tenants_[j];
					if ((tenant.state_ != 1) && (tenant.state_ != 4)) { continue; }
if (tenant.tenantDataId_ == -1) { continue; }
TenantData tenantData = apdat_.tenantData_[tenant.tenantDataId_];
num7 += tenantData.paramKeepCost_;
apdat_.SubMoney(tenantData.paramKeepCost_, 1);
				}
			}
			if (num4 != apdat_.save_.ints[3])
			{
				int matchInfoJoin = udat_.GetMatchInfoJoin();
				if (matchInfoJoin != 2)
				{
					udat_.matchInfoWeek_++;
					if ((matchInfoJoin == 0)) {
						LeagueData leagueData = apdat_.leagueData_[udat_.selectLeagueId_];
						if (udat_.matchInfoWeek_ >= leagueData.schedule_)
						{
							busgather_ph = 1;
							if (!apdat_.CheckAlreadyEvent(156))
							{
								apdat_.ExecEvent(156, null, null);
							}
							if (LeagueData.IsVictoryMatch())
							{
								apdat_.ExecEvent(131, null, null);
							}
							else if (LeagueData.IsLastMatch())
							{
								apdat_.ExecEvent(130, null, null);
							}
							else
							{
								apdat_.ExecEvent(129, udat_.league_setu_ + 1 + string.Empty, null);
							}
							SubForm form2 = new SubForm(78);
							apdat_.Push(form2);
						}
						else
						{
							LEAGUE_SCHEDULE_UPDATECNT = 12;
						}

					} else if ((matchInfoJoin == 1)) {
						QuestMatchData questMatchData = apdat_.qmData_[udat_.selectQmId_];
						if (udat_.matchInfoWeek_ >= questMatchData.schedule_)
						{
							busgather_ph = 1;
							if (!apdat_.CheckAlreadyEvent(156))
							{
								apdat_.ExecEvent(156, null, null);
							}
							if (QuestMatchData.IsVictoryMatch())
							{
								apdat_.ExecEvent(131, null, null);
							}
							else
							{
								apdat_.ExecEvent(133, questMatchData.name_, null);
							}
							SubForm form = new SubForm(78);
							apdat_.Push(form);
						}
						else
						{
							LEAGUE_SCHEDULE_UPDATECNT = 12;
						}

					}
				}
				if (apdat_.save_.ints[3] == 2 && apdat_.save_.ints[2] == 11)
				{
					TeamData teamData = apdat_.tData_[0];
					if (!apdat_.CheckAlreadyEvent(166))
					{
						apdat_.ExecEvent(166, null, null);
					}
					apdat_.ExecEvent(110, teamData.name_ + "\t" + udat_.memberPDataist_.Size(), null);
				}
				for (int num8 = udat_.restTPlayers_.Size() - 1; num8 >= 0; num8--)
				{
					TPlayer tenantIn = (TPlayer)udat_.restTPlayers_.ElementAt(num8);
					if (udat_.SetTenantIn(tenantIn))
					{
						udat_.restTPlayers_.RemoveElementAt(num8);
					}
				}
			}
			int num9 = apdat_.save_.ints[4];
			int num10 = apdat_.save_.ints[1];
			int num11 = apdat_.save_.ints[2];
			int num12 = apdat_.save_.ints[3];
			int num13 = TRAINING_PARAMUP_TIME.Length - 1;
			for (int k = 0; k < TRAINING_PARAMUP_TIME.Length; k++)
			{
				if (num9 <= TRAINING_PARAMUP_TIME[k])
				{
					num13 = k;
					break;
				}
			}
			num9 += add;
			while (num9 >= 10800)
			{
				num12++;
				num9 -= 10800;
				if (num12 >= 4)
				{
					num12 = 0;
					num11++;
					if (num11 >= 12)
					{
						num11 = 0;
						num10++;
					}
				}
			}
			SupervisorData supervisorData2;
if (num5 != num13) {
			supervisorData2 = apdat_.svisorData_[udat_.selectSupervisorId_];
			for (int l = 0; l < udat_.memberPDataist_.Size(); l++)
			{
				PlayerData playerData = (PlayerData)udat_.memberPDataist_.ElementAt(l);
				for (int m = 0; m < playerData.trainingPoint_.Length; m++)
				{
				}
			}

}
}

		public void SetBattleData()
		{
			LeagueData leagueData = apdat_.leagueData_[udat_.selectLeagueId_];
			int testedValue = udat_.GetMatchInfoJoin();
if (testedValue == 0)
			{
				int[] nextBattleLeagueIndex = udat_.GetNextBattleLeagueIndex(udat_.league_setu_);
				udat_.gm_.SetBattleTeamId(0, leagueData.matchEnemy_[nextBattleLeagueIndex[0]], nextBattleLeagueIndex[1]);
			}
			else
			{
				QuestMatchData questMatchData = apdat_.qmData_[udat_.selectQmId_];
				udat_.gm_.SetBattleTeamId(0, questMatchData.matchEnemy_[udat_.battleQmIndex_], 1);
			}
		}

		public void SetBuildBtmMessage(string str, int cnt)
		{
			build_btmMessage = str;
			build_btmCnt = cnt;
		}

		public int GetSeason(int month)
		{
			if (month >= 2 && month < 5)
			{
				return 0;
			}
			if (month >= 5 && month < 8)
			{
				return 1;
			}
			if (month >= 8 && month < 11)
			{
				return 2;
			}
			return 3;
		}

		public void Draw_stadiumName(Graphics g, int dy, int centerX)
		{
			TeamData teamData = apdat_.tData_[udat_.gm_.teamId_[0]];
			TeamData teamData2 = apdat_.tData_[udat_.gm_.teamId_[1]];
			string empty = string.Empty;
			if (udat_.gm_.homeAway_ == 0) { empty = teamData.GetHomeStadiumName(); } else { empty = teamData2.GetHomeStadiumName(); }
			int num = g.GetFont(null).StringWidth(GameManager.GAME_HOMEAWAY_STR[udat_.gm_.homeAway_]);
			int num2 = g.GetFont(null).StringWidth(empty);
			int num3 = 26 + num + 10 + num2;
			bool flag = false;
			if (MyFormBase.En())
			{
				if (udat_.gm_.homeAway_ == 0)
				{
					if (teamData.nationalFlag_ == 0)
					{
						flag = true;
					}
				}
				else if (teamData2.nationalFlag_ == 0)
				{
					flag = true;
				}
			}
			if (flag)
			{
				num3 = num + 10 + num2;
			}
			udat_.gm_.Draw_timeMessageWindow(g, dy, 222, 1, centerX);
			if (!flag)
			{
				if (udat_.gm_.homeAway_ == 0)
				{
					apdat_.DrawSeb(g, centerX - num3 / 2, dy + 2, apdat_.resCom2_.seb[100], apdat_.resCom2_.img[183], teamData.nationalFlag_);
				}
				else
				{
					apdat_.DrawSeb(g, centerX - num3 / 2, dy + 2, apdat_.resCom2_.seb[100], apdat_.resCom2_.img[183], teamData2.nationalFlag_);
				}
			}
			g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
			if (!flag)
			{
				g.DrawString(GameManager.GAME_HOMEAWAY_STR[udat_.gm_.homeAway_], centerX - num3 / 2 + 26, dy + 3);
			}
			else
			{
				g.DrawString(GameManager.GAME_HOMEAWAY_STR[udat_.gm_.homeAway_], centerX - num3 / 2, dy + 3);
			}
			if (!flag)
			{
				g.DrawString(empty, centerX - num3 / 2 + (26 + num + 10), dy + 3);
			}
			else
			{
				g.DrawString(empty, centerX - num3 / 2 + (num + 10), dy + 3);
			}
		}

		public override int GetTouchValue(int type, int index)
		{
			return base.GetTouchValue(type, index);
		}

		public override void SetTouchValue(int type, int value)
		{
			base.SetTouchValue(type, value);
		}

		public override bool OnTouchEvent(TouchEvent e)
		{
			if (e.component.id == 17)
			{
				OnTouchCamera(e.type, e.component.value, e);
				return false;
			}
			return true;
		}

		public void OnTouchCamera(int type, int v, TouchEvent e)
		{
			TouchComponent component = e.component;
			int[] array = null;
			float num = e.dx * 100 / scale_;
			float num2 = e.dy * 100 / scale_;
			int num3 = (int)(Camera.pos.x_ + (float)((e.x - view_.GetGameWidth() / 2) * 100 / scale_));
			int num4 = (int)(Camera.pos.y_ - (float)((e.y - view_.GetGameHeight() / 2) * 100 / scale_));
			int num5 = e.x * 100 / scale_;
			int num6 = (e.y - VIEW_Y) * 100 / scale_;
			switch (type)
			{
			case 0:
				touchCameraPoint_.RemoveAllElements();
				touchCameraSpeed_.Set(0f, 0f, 0f);
				touchDragCamera_ = true;
				touchBuildDrag_ = false;
				cursorHideWait_ = 40;
				buildDownX_ = e.x;
				buildDownY_ = e.y;
				isBuildTrack_ = false;
				buildLock_ = true;
				touchBuildDragFrame_ = 0;
				touch_startDragPos[0] = num5;
				touch_startDragPos[1] = num6;
				touch_startDragSPosCenter[0] = (int)Camera.pos.x_;
				touch_startDragSPosCenter[1] = (int)Camera.pos.y_;
				SetCursorPosition(num3, num4);
				apdat_.camera.flg_disp = false;
				break;
			case 8:
				if (buildLock_ && JMath.Abs(buildDownX_ - e.x) < 4 && JMath.Abs(buildDownY_ - e.y) < 4)
				{
					break;
				}
				buildLock_ = false;
				if (touchDragCamera_)
				{
					if (num * num + num2 * num2 > 625f)
					{
						touchSkip_ = true;
					}
					cursorHideWait_ = 0;
					SetCameraPosition(touch_startDragSPosCenter[0] - (num5 - touch_startDragPos[0]), touch_startDragSPosCenter[1] + (num6 - touch_startDragPos[1]));
				}
				array = null;
				if (touchCameraPoint_.Size() < 5)
				{
					array = new int[2]
					{
						(int)num,
						(int)num2
					};
				}
				else
				{
					array = (int[])touchCameraPoint_.LastElement();
					array[0] = (int)num;
					array[1] = (int)num2;
					touchCameraPoint_.RemoveElementAt(touchCameraPoint_.Size() - 1);
				}
				touchCameraPoint_.InsertElementAt(array, 0);
				break;
			case 5:
				apdat_.camera.flg_disp = true;
				if (touchCameraPoint_.Size() == 0)
				{
					apdat_.camera.old_subPlayer_ = apdat_.camera.subPlayer_;
					apdat_.camera.old_subSvisor_ = apdat_.camera.subSvisor_;
					apdat_.camera.old_subTenant_ = apdat_.camera.subTenant_;
					apdat_.camera.subPlayer_ = null;
					apdat_.camera.subSvisor_ = null;
					apdat_.camera.subTenant_ = null;
					apdat_.camera.Collision(num3, num4);
					bool flag = false;
					if (apdat_.camera.subPlayer_ != null)
					{
						flag = true;
					}
					else if (apdat_.camera.subSvisor_ != null)
					{
						flag = true;
					}
					else if (apdat_.camera.subTenant_ != null)
					{
						flag = true;
					}
					if (flag)
					{
						canvas_.KeyClick(1048576);
					}
				}
				break;
			case 1:
				apdat_.camera.flg_disp = true;
				apdat_.camera.cursorCnt_ = 0;
				if (touchCameraPoint_.Size() > 0 && touchDragCamera_)
				{
					int[] array2 = (int[])touchCameraPoint_.FirstElement();
					touchCameraSpeed_.Set((int)num - array2[0], 0f, (int)num2 - array2[1]);
					touchCameraDecimal_.Set(0f, 0f, 0f);
				}
				break;
			case 12:
			{
				touchBuildDrag_ = false;
				pinchScaleValue_ = AppData.Clamp(pinchScaleValue_ + e.pinch, GetScaleMin(), GetScaleMax() + 40);
				int num7 = pinchScaleValue_;
				if (num7 > 100)
				{
					num7 = AppData.ClampMin(num7 - 40, 100);
				}
				SetScale(num7);
				touchDragCamera_ = false;
				break;
			}
			}
		}

		public void SetCameraPosition(int x, int y)
		{
			Camera.cursor_pos.x_ = x;
			Camera.cursor_pos.y_ = y;
			Camera.pos.Set(Camera.cursor_pos);
			apdat_.camera.SetCameraRect();
			Camera.camera_spos[0] = (int)Camera.cursor_pos.x_;
			Camera.camera_spos[1] = (int)Camera.cursor_pos.y_;
			Camera.ConvertScreenToGlobal(Camera.camera_spos, Camera.camera_gpos);
			Camera.CAMERA_ADS_GPOS.Set(Camera.camera_gpos.x_ - Camera.CAMERA_BASE_GPOS.x_, 0f, Camera.camera_gpos.z_ - Camera.CAMERA_BASE_GPOS.z_);
			Camera.ConvertGlobalToAddressMain(Camera.CAMERA_ADS_GPOS, Camera.camera_address);
		}

		public void SetCursorPosition(int x, int y)
		{
			Camera.cursor_pos.x_ = x;
			Camera.cursor_pos.y_ = y;
			apdat_.camera.SetCameraRect();
			Camera.camera_spos[0] = (int)Camera.cursor_pos.x_;
			Camera.camera_spos[1] = (int)Camera.cursor_pos.y_;
			Camera.ConvertScreenToGlobal(Camera.camera_spos, Camera.camera_gpos);
			Camera.CAMERA_ADS_GPOS.Set(Camera.camera_gpos.x_ - Camera.CAMERA_BASE_GPOS.x_, 0f, Camera.camera_gpos.z_ - Camera.CAMERA_BASE_GPOS.z_);
			Camera.ConvertGlobalToAddressMain(Camera.CAMERA_ADS_GPOS, Camera.camera_address);
		}

		private int[] GetScaleRange()
		{
			getScaleRange_tmp[0] = GetScaleMin();
			getScaleRange_tmp[1] = GetScaleMax();
			return getScaleRange_tmp;
		}

		private int GetScaleMin()
		{
			if (view_.GetGameWidth() > 240)
			{
				return 75;
			}
			return 60;
		}

		private int GetScaleMax()
		{
			return 200;
		}

		private void SetScale(int scale)
		{
			int[] scaleRange = GetScaleRange();
			scale = AppData.Clamp(scale, scaleRange[0], scaleRange[1]);
			if (scale == scale_) { return; }
{
int gameWidth = GetGameWidth();
int gameHeight = GetGameHeight();
scale_ = scale;
ResetOffscreen();
}
		}

		public int GetGameWidth()
		{
			return (view_.GetGameWidth() - VIEW_X) * 100 / scale_;
		}

		public int GetGameHeight()
		{
			return (view_.GetGameHeight() - VIEW_Y) * 100 / scale_;
		}

		public SubForm OepnWindowFromMainDisp()
		{
			if (apdat_.camera.subPlayer_ != null)
			{
				SubForm subForm = new SubForm(30);
				subForm.playerData_ = apdat_.pData_[apdat_.camera.subPlayer_.pData_id_];
				return subForm;
			}
			if (apdat_.camera.subSvisor_ != null)
			{
				{ global::form.SubForm recoveredReturn_48915 = new SubForm(50); return recoveredReturn_48915; }
			}
			if (apdat_.camera.subTenant_ != null)
			{
				SubForm subForm2 = null;
				if (apdat_.camera.subTenant_.state_ == 0)
				{
					if (!apdat_.CheckAlreadyEvent(157))
					{
						apdat_.ExecEvent(157, null, null);
					}
					apdat_.ExecEvent(125, null, null);
					subForm2 = new SubForm(61);
					subForm2.tenantId_ = apdat_.camera.subTenant_.id_;
					subForm2.tenantDataId_ = -1;
					return subForm2;
				}
				if (apdat_.camera.subTenant_.state_ == 1)
				{
					subForm2 = new SubForm(65);
					subForm2.tenantId_ = apdat_.camera.subTenant_.id_;
					subForm2.tenantDataId_ = apdat_.camera.subTenant_.tenantDataId_;
					return subForm2;
				}
			}
			return null;
		}

		public void LogOut()
		{
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
