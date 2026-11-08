public class Tenant{public int state_,tenantDataId_;public void StrenghTenant(){}}
public class UserData{public Tenant[] tenants_;}
public class Fixture{public static UserData udat_;public int lv_,id_;public void ChangeState(int state){}
public void Run(){
			if (lv_ == 5)
			{
				return;
			}
			for (int i = 0; i < udat_.tenants_.Length; i++)
			{
				Tenant tenant = udat_.tenants_[i];
				if ((tenant.state_ == 0) || (tenant.tenantDataId_ != id_)) { continue; }
tenant.StrenghTenant();
			}
			ChangeState(2);
		}
}