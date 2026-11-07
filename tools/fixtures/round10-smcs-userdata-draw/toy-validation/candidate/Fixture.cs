using System;
public class TeamData {public int state;}
public class Property {public int[] ints=new int[16];}
public class AppData {static AppData instance=new AppData();public Property sysSave_=new Property();public static AppData GetInstance(){return instance;}}
public class Graphics {public int sum;public void Draw(int a,int b,int c){sum+=a+b+c;}public void DrawString2(string s,int a,int b){sum+=s.Length+a+b;}public void DrawMoney2(int n){sum+=n;}}
public class Fixture {TeamData first=new TeamData();int mode;int[] et_cur=new int[5];
public void Run(Graphics g,bool soccer){
TeamData teamData=first;int num9, num, num2, num3;
num9 = num = num2 = num3 = 0;int num4=0;
if(soccer){num4=1;g.Draw(240,238,num4);}else{num4=2;g.Draw(240,236,num4);}
if(mode==0){num9=10;num=num9+1;num2=num9+3;num3=num+4;g.Draw(num2,num3,teamData.state);}
else{num9=20;num=num9+2;num2=num9+3;num3=num+4;g.Draw(num2,num3,teamData.state);}
if(mode<2){if(soccer){num9=30;num=40;}else{num9=31;num=41;}
 string empty=string.Empty;if (et_cur[4] == 0) { empty = "balanced"; } else if (et_cur[4] == 1) { empty = "attacking"; } else { empty = "defensive"; }g.DrawString2(empty,num9+28,num+6);}
if(mode>0){num9=50;num=num9+5;g.Draw(num9,num,num4);}
if(mode>1){num9=60;num=num9+6;int sallay=et_cur[0];
 if(soccer){bool flag2=AppData.GetInstance().sysSave_.ints[14]==1;int num22=0;if(!flag2){num22=1;}g.DrawMoney2(sallay+num22);}
 else if(mode==3){g.DrawMoney2(3);}
 else { bool recoveredMoneyStyle = AppData.GetInstance().sysSave_.ints[14] == 1;
if (recoveredMoneyStyle){g.DrawMoney2(sallay+2);}
 else if(sallay>=1000){g.DrawMoney2(sallay-3);}else{g.DrawMoney2(sallay-5);}
}}
g.Draw(num2,num3,num4);
}

}
