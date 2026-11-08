public class TeamData{public int warPotential_;}
public class Property{public long[] lngs;}
public class AppData{public Property save_;public TeamData[] tData_;}
public class Fixture{public AppData apdat_;public int[] researchPoint_;public int[][] tpplay_stock;
public void AddTestPlayerParam(int id,int value){}
public void Run(){
			for (int i = 0; i < 9; i++)
			{
				if ((i == 0)) { AddTestPlayerParam(i, (int)apdat_.save_.lngs[0]);
continue; } else if ((i >= 4) && (i <= 7)) { AddTestPlayerParam(i, researchPoint_[i - 4]);
continue; } else if ((i == 1) || (i == 2) || (i == 3)) { continue; }
				if (i == 8)
				{
					TeamData teamData = apdat_.tData_[0];
					AddTestPlayerParam(i, teamData.warPotential_);
				}
			}
			for (int num = tpplay_stock.Length - 1; num >= 1; num--)
			{
				for (int j = 0; j < tpplay_stock[num].Length; j++)
				{
					tpplay_stock[num][j] = tpplay_stock[num - 1][j];
				}
			}
			for (int k = 0; k < tpplay_stock[0].Length; k++)
			{
				tpplay_stock[0][k] = 0;
			}
		}
}