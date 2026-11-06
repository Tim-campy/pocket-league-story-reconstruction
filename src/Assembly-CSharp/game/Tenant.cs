using data;
using form;
using java.io;
using java.util;
using kairo.unity.ui;
using kairo.unity.util;
using main;

namespace game
{
	public class Tenant
	{
		public const int STATE_VACANT = 0;

		public const int STATE_TENANT = 1;

		public const int STATE_BUILDING = 2;

		public const int STATE_DESTRUCT = 3;

		public const int STATE_STRENGH = 4;

		public const int USE_TPLAYER_MAX = 3;

		public const int USETENANT_BAR_W = 51;

		public const int USETENANT_BAR_H = 18;

		public const int DESTRUCT_TIME = 12;

		private const int DRAW_CNT = 4;

		public AppData apdat_;

		public UserData udat_;

		public int id_;

		public int[] address_;

		public int state_;

		public int stateCnt_;

		public int tenantDataId_;

		public int root_;

		public Vector useTPlayers_;

		public static readonly int[] USETENANT_TIME = new int[2] { 200, 230 };

		private int[] useTPlayersId;

		private static int[] camera_pos = new int[2];

		private int draw_cnt;

		public Tenant(int id)
		{
			id_ = id;
			useTPlayers_ = new Vector();
			address_ = new int[2];
			state_ = 0;
		}

		public void Init(int root)
		{
			apdat_ = AppData.GetInstance();
			udat_ = UserData.GetInstance();
			root_ = root;
		}

		public void Serialize(OutputStream os)
		{
			StreamUtil.WriteShort(os, (short)id_);
			StreamUtil.WriteShort(os, (short)address_[0]);
			StreamUtil.WriteShort(os, (short)address_[1]);
			StreamUtil.WriteShort(os, (short)state_);
			StreamUtil.WriteInt(os, stateCnt_);
			StreamUtil.WriteShort(os, (short)tenantDataId_);
			StreamUtil.WriteShort(os, (short)root_);
			StreamUtil.WriteInt(os, useTPlayers_.Size());
			for (int i = 0; i < useTPlayers_.Size(); i++)
			{
				TPlayer tPlayer = (TPlayer)useTPlayers_.ElementAt(i);
				StreamUtil.WriteShort(os, (short)tPlayer.trainingId_[0]);
			}
		}

		public void Deserialize(InputStream stream, int index)
		{
			Init(TenantData.TENANT_ROOT[index]);
			id_ = StreamUtil.ReadShort(stream);
			address_[0] = StreamUtil.ReadShort(stream);
			address_[1] = StreamUtil.ReadShort(stream);
			state_ = StreamUtil.ReadShort(stream);
			stateCnt_ = StreamUtil.ReadInt(stream);
			tenantDataId_ = StreamUtil.ReadShort(stream);
			root_ = StreamUtil.ReadShort(stream);
			int num = StreamUtil.ReadInt(stream);
			useTPlayersId = new int[num];
			for (int i = 0; i < useTPlayersId.Length; i++)
			{
				useTPlayersId[i] = StreamUtil.ReadShort(stream);
			}
		}

		public void Deserialize2()
		{
			useTPlayers_ = new Vector();
			for (int i = 0; i < useTPlayersId.Length; i++)
			{
				TPlayer obj = UserData.GetInstance().tPlayers_[useTPlayersId[i]];
				useTPlayers_.AddElement(obj);
			}
		}

		public void ChangeState(int state)
		{
			state_ = state;
			stateCnt_ = 0;
		}

		public void Update()
		{
stateCnt_ = (stateCnt_ + 1) % int.MaxValue;
if (state_ == 3) {
if (stateCnt_ >= 12) {
ChangeState(0);
apdat_.mapMain_.UpdateTenantMapcip();
}
return;
}
TenantData tenantData = null;
if (tenantDataId_ != -1) {
tenantData = apdat_.tenantData_[tenantDataId_];
} else {
return;
}
if (state_ == 2) {
if (stateCnt_ >= tenantData.buildTime_) {
ChangeState(1);
apdat_.mapMain_.UpdateTenantMapcip();
if (tenantDataId_ == TenantData.TENANT_ID_GOODS) {
if (!apdat_.CheckAlreadyEvent(171)) {
apdat_.ExecEvent(171, null, null);
}
}
}
} else {
if (state_ == 4) {
if (stateCnt_ >= 200) {
ChangeState(1);
apdat_.mapMain_.UpdateTenantMapcip();
}
}
}
if (useTPlayers_.Size() > 0) {
TPlayer tPlayer = (TPlayer)useTPlayers_.ElementAt(0);
tPlayer.useTenantCnt_++;
if (tPlayer.useTenantCnt_ == 1) {
int userPrice = tenantData.GetUserPrice();
int[] array = MapMain.MAPCHIP_SPOS[address_[1]][address_[0]];
apdat_.AddEffectMoneyUp(array[0], array[1], userPrice);
apdat_.AddMoney(userPrice, 1);
} else {
if (tPlayer.useTenantCnt_ == USETENANT_TIME[0]) {
int[] tenantUseResearchPoint = tenantData.GetTenantUseResearchPoint();
int[] array2 = MapMain.MAPCHIP_SPOS[address_[1]][address_[0]];
apdat_.AddEffectRpUp(array2[0], array2[1], tenantUseResearchPoint[0], tenantUseResearchPoint[1]);
udat_.AddResearchPoint(tenantUseResearchPoint[0], tenantUseResearchPoint[1]);
PlayerData playerData = apdat_.pData_[tPlayer.pData_id_];
playerData.AddAuraPoint(tenantData.paramEfToFan_, false);
}
}
if (tPlayer.useTenantCnt_ >= USETENANT_TIME[1]) {
useTPlayers_.RemoveElementAt(0);
udat_.SetTenantIn(tPlayer);
}
}
return;
}

		public void Draw(Graphics g)
		{
			Draw_tenantEffect(g);
			if (useTPlayers_.Size() > 0) {

			TPlayer tPlayer = (TPlayer)useTPlayers_.ElementAt(0);
			Camera.ConvertScreenToCamera(MapMain.MAPCHIP_SPOS[address_[1]][address_[0]], camera_pos);
			camera_pos[0] += 30;
			camera_pos[1] += 0;
			if (GameUtil.ChRectHitRect(camera_pos[0] - 25, camera_pos[1] - 18, 51, 18, GameForm.VIEW_X, GameForm.VIEW_Y, GameForm.VIEW_W, GameForm.VIEW_H, false))
			{
				apdat_.InsertGameObject(10, tPlayer.trainingId_[0], camera_pos[1] + 60, 0, 0, 0, camera_pos[0], camera_pos[1], false, 0);
			}
			if ((useTPlayers_.Size() > 1) && (apdat_.save_.ints[12] == 1)) {

			camera_pos[1] += 40;
			int num = 0;
			draw_cnt = 0;
			int num2 = AppData.ClampMax(4, useTPlayers_.Size() - 1);
			for (int num3 = num2; num3 >= 1; num3--)
			{
				int x = camera_pos[0] + (num3 - 1) * 8;
				num = camera_pos[1] + (num3 - 1) * -4;
				TPlayer tPlayer2 = (TPlayer)useTPlayers_.ElementAt(num3);
				tPlayer2.direct_ = 2;
				tPlayer2.flag_ = BitUtil.Off(tPlayer2.flag_, 2);
				int layer = tPlayer2.SetDrawParam(tPlayer2.seb_, -1);
				apdat_.InsertGameObject(2, tPlayer2.trainingId_[0], camera_pos[1] + 60 + 1, 0, 0, 0, x, num, false, layer);
				draw_cnt++;
				if (draw_cnt >= 4)
				{
					break;
				}
			}
}
}
		}

		public void Draw_tenantEffect(Graphics g)
		{
			if (state_ == 2)
			{
				TenantData tenantData = apdat_.tenantData_[tenantDataId_];
				int val = GameUtil.RateConvert(stateCnt_, 0, tenantData.buildTime_, 0, 3, true);
				val = AppData.ClampMax(val, 2);
				Image image = apdat_.resCom2_.img[155 + val];
				for (int i = 0; i < 2; i++)
				{
					for (int j = 0; j < 2; j++)
					{
						Camera.ConvertScreenToCamera(MapMain.MAPCHIP_SPOS[address_[1] - i][address_[0] + j], camera_pos);
						if (!(GameUtil.ChRectHitRect(camera_pos[0], camera_pos[1] + 29 - image.GetHeight(), 60, image.GetHeight(), GameForm.VIEW_X, GameForm.VIEW_Y, GameForm.VIEW_W, GameForm.VIEW_H, false))) { continue; }
apdat_.InsertGameObject(8, 0, camera_pos[1] + 1, 155 + val, 85 + val, 0, camera_pos[0], camera_pos[1], false, 0);
					}
				}
			}
			else if (state_ == 3)
			{
				int val2 = GameUtil.RateConvert(stateCnt_, 0, 12, 0, 3, true);
				val2 = AppData.ClampMax(val2, 2);
				Image image2 = apdat_.resCom2_.img[158];
				for (int k = 0; k < 2; k++)
				{
					for (int l = 0; l < 2; l++)
					{
						Camera.ConvertScreenToCamera(MapMain.MAPCHIP_SPOS[address_[1] - k][address_[0] + l], camera_pos);
						if (!(GameUtil.ChRectHitRect(camera_pos[0], camera_pos[1] + 29 - image2.GetHeight(), 60, image2.GetHeight(), GameForm.VIEW_X, GameForm.VIEW_Y, GameForm.VIEW_W, GameForm.VIEW_H, false))) { continue; }
apdat_.InsertGameObject(8, 0, camera_pos[1], 158, 88, val2, camera_pos[0], camera_pos[1], false, 0);
					}
				}
			}
			else
			{
				if (state_ == 4) {

				int val3 = GameUtil.RateConvert(stateCnt_, 0, 200, 0, 3, true);
				val3 = AppData.ClampMax(val3, 2);
				Image image3 = apdat_.resCom2_.img[155 + val3];
				for (int m = 0; m < 2; m++)
				{
					for (int n = 0; n < 2; n++)
					{
						Camera.ConvertScreenToCamera(MapMain.MAPCHIP_SPOS[address_[1] - m][address_[0] + n], camera_pos);
						if (!(GameUtil.ChRectHitRect(camera_pos[0], camera_pos[1] + 29 - image3.GetHeight(), 60, image3.GetHeight(), GameForm.VIEW_X, GameForm.VIEW_Y, GameForm.VIEW_W, GameForm.VIEW_H, false))) { continue; }
apdat_.InsertGameObject(8, 0, camera_pos[1] + 1, 155 + val3, 85 + val3, 0, camera_pos[0], camera_pos[1], false, 0);
					}
				}
}
			}
		}

		public void BuildTenant(int tenantDataId)
		{
			tenantDataId_ = tenantDataId;
			ChangeState(2);
			for (int num = udat_.restTPlayers_.Size() - 1; num >= 0; num--)
			{
				TPlayer tenantIn = (TPlayer)udat_.restTPlayers_.ElementAt(num);
				if (udat_.SetTenantIn(tenantIn))
				{
					udat_.restTPlayers_.RemoveElementAt(num);
				}
			}
		}

		public void DestructTenant()
		{
			tenantDataId_ = -1;
			ChangeState(3);
			apdat_.mapMain_.UpdateTenantMapcip();
			OutUserTPlayers();
		}

		public void StrenghTenant()
		{
			ChangeState(4);
			apdat_.mapMain_.UpdateTenantMapcip();
			OutUserTPlayers();
		}

		public void OutUserTPlayers()
		{
			for (int i = 0; i < useTPlayers_.Size(); i++)
			{
				TPlayer tenantIn = (TPlayer)useTPlayers_.ElementAt(i);
				udat_.SetTenantIn(tenantIn);
			}
			if (useTPlayers_.Size() > 0)
			{
				useTPlayers_.RemoveAllElements();
			}
		}
	}
}
