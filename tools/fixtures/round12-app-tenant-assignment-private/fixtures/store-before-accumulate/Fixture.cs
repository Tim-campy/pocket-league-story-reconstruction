public class TPlayer{public int useTenantCnt_;}
public class TenantData{public int charm_;}
public class Vec{public int Size(){return 0;}public void RemoveAllElements(){}public void AddElement(object x){}public int IndexOf(object x){return 0;}public object ElementAt(int i){return null;}public void SetElementAt(object x,int i){}}
public class Tenant{public int state_,tenantDataId_;public Vec useTPlayers_;}
public class AppData{public TenantData[] tenantData_;}
public static class GameUtil{public static int Random(int x){return x;}}
public class Fixture{public Vec tenantList,restTPlayers_;public Tenant[] tenants_;public AppData apdat_;
public bool Run(TPlayer tPlayer){
			tPlayer.useTenantCnt_ = 0;
			if (tenantList.Size() > 0)
			{
				tenantList.RemoveAllElements();
			}
			for (int i = 0; i < tenants_.Length; i++)
			{
				if (tenants_[i].state_ != 1) { continue; }
if (tenants_[i].useTPlayers_.Size() > 3) { continue; }
tenantList.AddElement(tenants_[i]);
			}
			if (tenantList.Size() == 0)
			{
				for (int j = 0; j < tenants_.Length; j++)
				{
					if (tenants_[j].state_ != 1) { continue; }
tenantList.AddElement(tenants_[j]);
				}
				if (tenantList.Size() == 0)
				{
					if (restTPlayers_.IndexOf(tPlayer) == -1)
					{
						restTPlayers_.AddElement(tPlayer);
					}
					return false;
				}
			}
			for (int k = 0; k < tenantList.Size() - 1; k++)
			{
				for (int num = tenantList.Size() - 1; num > k; num--)
				{
					Tenant tenant = (Tenant)tenantList.ElementAt(k);
					Tenant tenant2 = (Tenant)tenantList.ElementAt(num);
					TenantData tenantData = apdat_.tenantData_[tenant.tenantDataId_];
					TenantData tenantData2 = apdat_.tenantData_[tenant2.tenantDataId_];
					if (tenantData2.charm_ > tenantData.charm_)
					{
						tenantList.SetElementAt(tenant2, k);
						tenantList.SetElementAt(tenant, num);
					}
				}
			}
			int[] array = new int[tenantList.Size()];
			int num2 = 0;
			for (int l = 0; l < tenantList.Size(); l++)
			{
				Tenant tenant3 = (Tenant)tenantList.ElementAt(l);
				array[l] = num2; num2 = num2 + apdat_.tenantData_[tenant3.tenantDataId_].charm_;
			}
			int num3 = GameUtil.Random(num2);
			for (int m = 0; m < tenantList.Size(); m++)
			{
				Tenant tenant4 = (Tenant)tenantList.ElementAt(m);
				if (num3 < array[m])
				{
					tenant4.useTPlayers_.AddElement(tPlayer);
					break;
				}
			}
			return true;
		}
}