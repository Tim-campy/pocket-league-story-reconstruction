public static class Map {public static int MAPCHIP_COAT_HNUM,MAPCHIP_COAT_WNUM;}
public static class GameUtil {public static int RateConvert(int x,int a,int b,int c,int d,bool e){return x;} }
public static class AppData {public static int ClampMax(int x,int max){return x<max?x:max;} }
public class Player {public int[] address_;}
public class Team {public Player[] players_;}
public class UserData {public Team[] teams_;}
public class Fixture {public int[][] coatEva_new_offence,coatEva_new_offence_passer,amefto_address_;public int team_id_,chargeNum_;public UserData udat_;
public static int[] ch_ceva,AMEFTO_PASSER_COATEVA;public static int[][] DEAFULT_FORMATION_EVA_PENALTYAREA;public static float[] CEVA_PASSER_MP2;public bool area;
bool ChPAreaLineOver(){return area;}void Cal_evaCoatMulti9block(int[][] a,int b,int c,float[] d){}
public void Run(){
			for (int i = 0; i < Map.MAPCHIP_COAT_HNUM; i++)
			{
				for (int j = 0; j < Map.MAPCHIP_COAT_WNUM; j++)
				{
					coatEva_new_offence[i][j] = 0;
					coatEva_new_offence_passer[i][j] = 0;
				}
			}
			if (ch_ceva[0] == 1)
			{
				{ bool recoveredCondition_27548 = team_id_ == 0;
if (recoveredCondition_27548)
				{
					for (int k = 0; k < Map.MAPCHIP_COAT_HNUM; k++)
					{
						float num = GameUtil.RateConvert(k, amefto_address_[0][1] - 4, amefto_address_[0][1] + 1, 0, AMEFTO_PASSER_COATEVA[chargeNum_], true) + 30;
						num = AppData.ClampMax((int)num, 100);
						for (int l = 0; l < Map.MAPCHIP_COAT_WNUM; l++)
						{
							coatEva_new_offence[k][l] = (int)num;
							coatEva_new_offence_passer[k][l] = (int)num;
						}
					}
					if (ChPAreaLineOver())
					{
						for (int m = 0; m < Map.MAPCHIP_COAT_HNUM; m++)
						{
							for (int n = 0; n < Map.MAPCHIP_COAT_WNUM; n++)
							{
								coatEva_new_offence[m][n] = DEAFULT_FORMATION_EVA_PENALTYAREA[m][n];
							}
						}
					}
				}
				else
				{
					for (int num2 = 0; num2 < Map.MAPCHIP_COAT_HNUM; num2++)
					{
						float num3 = GameUtil.RateConvert(num2, amefto_address_[0][1] + 4, amefto_address_[0][1] - 1, 0, AMEFTO_PASSER_COATEVA[chargeNum_], true) + 30;
						num3 = AppData.ClampMax((int)num3, 100);
						for (int num4 = 0; num4 < Map.MAPCHIP_COAT_WNUM; num4++)
						{
							coatEva_new_offence[num2][num4] = (int)num3;
							coatEva_new_offence_passer[num2][num4] = (int)num3;
						}
					}
					if (ChPAreaLineOver())
					{
						for (int num5 = 0; num5 < Map.MAPCHIP_COAT_HNUM; num5++)
						{
							for (int num6 = 0; num6 < Map.MAPCHIP_COAT_WNUM; num6++)
							{
								coatEva_new_offence[num5][num6] = DEAFULT_FORMATION_EVA_PENALTYAREA[Map.MAPCHIP_COAT_HNUM - 1 - num5][Map.MAPCHIP_COAT_WNUM - 1 - num6];
							}
						}
					}
				}
}
			}
			if (ch_ceva[2] == 1)
			{
				Team team = udat_.teams_[1 - team_id_];
				float[] cEVA_PASSER_MP = CEVA_PASSER_MP2;
				for (int num7 = 0; num7 < team.players_.Length; num7++)
				{
					Player player = team.players_[num7];
					Cal_evaCoatMulti9block(coatEva_new_offence, player.address_[0], player.address_[1], cEVA_PASSER_MP);
					Cal_evaCoatMulti9block(coatEva_new_offence_passer, player.address_[0], player.address_[1], cEVA_PASSER_MP);
				}
			}
		}
}