public class TeamData{public int warPotential_;}
public class Property{public long[] lngs;}
public class AppData{public Property save_;public TeamData[] tData_;}
public class Fixture{public AppData apdat_;public int[] researchPoint_;public int[][] tpplay_stock;
public void AddTestPlayerParam(int id,int value){}
public void Run(){for(int param=0;param<9;param++){
 if(param==0){AddTestPlayerParam(param,(int)apdat_.save_.lngs[0]);continue;}
 else if(param==1||param==2||param==3){continue;}
 else if(param>=4&&param<=7){AddTestPlayerParam(param,researchPoint_[param-4]);continue;}
 if(param==8){TeamData ownTeam=apdat_.tData_[0];AddTestPlayerParam(param,ownTeam.warPotential_);}
}
for(int row=tpplay_stock.Length-1;row>=1;row--){
 for(int col=0;col<tpplay_stock[row].Length;col++){tpplay_stock[row][col]=tpplay_stock[row-1][col];}
}
for(int clear=0;clear<tpplay_stock[0].Length;clear++){tpplay_stock[0][clear]=0;}}
}