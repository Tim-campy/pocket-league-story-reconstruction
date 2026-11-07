using System;
public class Node {
 public int state_,buildCost_,paramStrengthCost_,id_; public Node link;
 public Node(int n){id_=n;} public bool IsStreangth(){return state_>0;}
 public bool IsTAreaOrStadiumTenant(){return id_>1;}
 public static void ProcStrenSpecialTenant(Node n){n.state_++;}
}
public class App {
 public int sum; public bool ChPayMoney(int n){return n>0;} public void ExecEvent(int n,object a,object b){sum+=n;}
 public bool CheckAlreadyEvent(int n){return sum>n;} public void Show(Action<string> a){a("x");}
 public void Draw(){sum++;} public void NewGame(){sum=0;}
}
public class TenantData {public static void ProcStrenSpecialTenant(Node n){Node.ProcStrenSpecialTenant(n);}}
public class Fixture {
 int mode,select_,page_,type_; Node first,second; App apdat_=new App(); int[] values=new int[3];
 bool Pop(){apdat_.sum--;return false;} bool Push(Node n){first=n;return true;}
 void Tick(int n){apdat_.sum+=n;}
 public bool States(){
 if(mode==1){Node sponsorData=first;
 if(sponsorData.state_ !=2){Node subForm=new Node(43);subForm.link=sponsorData;return Push(subForm);}
 Tick(144);
 }else{Node fanBonusData=second;if(fanBonusData.state_ !=0){return Pop();}Tick(190);}
 return true;}

public bool Select(){
 if(mode==1){Node keep=first;if(select_ !=0){return Pop();}if(keep.state_>0){Node x=new Node(44);Push(x);}else{Tick(13);}Pop();}
 else if(mode==2){if(select_ !=0){return Pop();}Node keep=second;if(keep.state_>0){Tick(14);}else{Tick(15);}}
 else if(mode==3){if(select_ !=0){return Pop();}Node keep=first;Tick(keep.state_);}
 else if(mode==4){if(select_ !=0){return Pop();}Tick(17);}
 else{if(select_ !=0){apdat_.ExecEvent(116,null,null);return Pop();}if(first.state_>11){Tick(117);return Push(new Node(31));}Tick(121);}
 return true;}

public bool Page(){if(mode==1){if(page_ !=0){return Pop();}Node keep=first;if(keep.state_>0||keep.id_==0){Tick(132);}else{Tick(14);}}return true;}

public bool Pay(){
 if(mode==1){Node tenantData3=first;if(!apdat_.ChPayMoney(tenantData3.buildCost_)){apdat_.ExecEvent(14,null,null);return Pop();}Tick(tenantData3.id_);}
 else {Node tenantData4=second;if(!tenantData4.IsStreangth()){return Pop();}
 if(!apdat_.ChPayMoney(tenantData4.paramStrengthCost_)){apdat_.ExecEvent(14,null,null);return Pop();}
 Tick(18);if(!tenantData4.IsTAreaOrStadiumTenant()){Tick(32);}else{
 if(!tenantData4.IsStreangth()){return Pop();}Tick(19);TenantData.ProcStrenSpecialTenant(tenantData4);if(tenantData4.id_==3){Tick(34);}else{Tick(33);}
 }}return true;}

public bool Event(){for(int l=0;l<values.Length;l++){if(values[l]==-1){continue;}Node playerData2=first;Tick(playerData2.id_);if(apdat_.CheckAlreadyEvent(189)){continue;}for (int m=0;m<values.Length;m++){if(values[m]>=1000){Tick(189);break;}}}return true;}

public bool Empty(){if(type_==47){Tick(47);}else if(type_ !=48){if(type_==49){Tick(49);}else{Tick(50);}}return true;}

public bool Captured(){Node t_data=first;if(select_==0){try{apdat_.Show(delegate(string value){if(value!=null){t_data.state_=value.Length;}});}finally{apdat_.Draw();}}else if(select_==1){try{apdat_.Show(delegate(string value){if(value!=null){t_data.id_=value.Length;}});}finally{apdat_.Draw();}}else if(select_ !=2 && select_==3){try{apdat_.NewGame();}catch(Exception e){Tick(e.Message.Length);}}return true;}

}
