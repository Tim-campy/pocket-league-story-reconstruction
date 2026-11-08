public class Tenant{public int state_,tenantDataId_;public void StrenghTenant(){}}
public class UserData{public Tenant[] tenants_;}
public class Fixture{public static UserData udat_;public int lv_,id_;public void ChangeState(int state){}
public void Run(){if(lv_==5){return;}
for(int entry=0;entry<udat_.tenants_.Length;entry++){
 Tenant selected=udat_.tenants_[entry];
 if(selected.state_==0){continue;}
 if(selected.tenantDataId_==id_){selected.StrenghTenant();}
}
ChangeState(2);}
}