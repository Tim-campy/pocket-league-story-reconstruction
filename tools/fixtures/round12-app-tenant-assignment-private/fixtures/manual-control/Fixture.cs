public class TPlayer{public int useTenantCnt_;}
public class TenantData{public int charm_;}
public class Vec{public int Size(){return 0;}public void RemoveAllElements(){}public void AddElement(object x){}public int IndexOf(object x){return 0;}public object ElementAt(int i){return null;}public void SetElementAt(object x,int i){}}
public class Tenant{public int state_,tenantDataId_;public Vec useTPlayers_;}
public class AppData{public TenantData[] tenantData_;}
public static class GameUtil{public static int Random(int x){return x;}}
public class Fixture{public Vec tenantList,restTPlayers_;public Tenant[] tenants_;public AppData apdat_;
public bool Run(TPlayer tPlayer){tPlayer.useTenantCnt_=0;
if(tenantList.Size()>0){tenantList.RemoveAllElements();}
for(int first=0;first<tenants_.Length;first++){
 if(tenants_[first].state_!=1){continue;}
 if(tenants_[first].useTPlayers_.Size()>3){continue;}
 tenantList.AddElement(tenants_[first]);
}
if(tenantList.Size()==0){
 for(int fallback=0;fallback<tenants_.Length;fallback++){
  if(tenants_[fallback].state_!=1){continue;}
  tenantList.AddElement(tenants_[fallback]);
 }
 if(tenantList.Size()==0){
  if(restTPlayers_.IndexOf(tPlayer)==-1){restTPlayers_.AddElement(tPlayer);}
  return false;
 }
}
for(int outer=0;outer<tenantList.Size()-1;outer++){
 for(int inner=tenantList.Size()-1;inner>outer;inner--){
  Tenant left=(Tenant)tenantList.ElementAt(outer);
  Tenant right=(Tenant)tenantList.ElementAt(inner);
  TenantData leftData=apdat_.tenantData_[left.tenantDataId_];
  TenantData rightData=apdat_.tenantData_[right.tenantDataId_];
  if(rightData.charm_>leftData.charm_){tenantList.SetElementAt(right,outer);tenantList.SetElementAt(left,inner);}
 }
}
int[] cumulative=new int[tenantList.Size()];
int sum=0;
for(int weight=0;weight<tenantList.Size();weight++){
 Tenant current=(Tenant)tenantList.ElementAt(weight);
 sum=sum+apdat_.tenantData_[current.tenantDataId_].charm_;
 cumulative[weight]=sum;
}
int chosen=GameUtil.Random(sum);
for(int position=0;position<tenantList.Size();position++){
 Tenant selected=(Tenant)tenantList.ElementAt(position);
 if(chosen<cumulative[position]){selected.useTPlayers_.AddElement(tPlayer);break;}
}
return true;}
}