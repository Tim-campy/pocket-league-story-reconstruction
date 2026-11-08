public static class Map {public static int MAPCHIP_COAT_HNUM,MAPCHIP_COAT_WNUM;}
public static class GameUtil {public static int RateConvert(int x,int a,int b,int c,int d,bool e){return x;} }
public static class AppData {public static int ClampMax(int x,int max){return x<max?x:max;} }
public class Player {public int[] address_;}
public class Team {public Player[] players_;}
public class UserData {public Team[] teams_;}
public class Fixture {public int[][] coatEva_new_offence,coatEva_new_offence_passer,amefto_address_;public int team_id_,chargeNum_;public UserData udat_;
public static int[] ch_ceva,AMEFTO_PASSER_COATEVA;public static int[][] DEAFULT_FORMATION_EVA_PENALTYAREA;public static float[] CEVA_PASSER_MP2;public bool area;
bool ChPAreaLineOver(){return area;}void Cal_evaCoatMulti9block(int[][] a,int b,int c,float[] d){}
public void Run(){for(int row=0;row<Map.MAPCHIP_COAT_HNUM;row++){
 for(int column=0;column<Map.MAPCHIP_COAT_WNUM;column++){
  coatEva_new_offence[row][column]=0;coatEva_new_offence_passer[row][column]=0;
 }
}
if(ch_ceva[0]==1){
 if(team_id_==0){
  for(int line=0;line<Map.MAPCHIP_COAT_HNUM;line++){
   float factor=GameUtil.RateConvert(line,amefto_address_[0][1]-4,amefto_address_[0][1]+1,0,AMEFTO_PASSER_COATEVA[chargeNum_],true)+30;
   factor=AppData.ClampMax((int)factor,100);
   for(int cell=0;cell<Map.MAPCHIP_COAT_WNUM;cell++){
    coatEva_new_offence[line][cell]=(int)factor;coatEva_new_offence_passer[line][cell]=(int)factor;
   }
  }
  if(!ChPAreaLineOver()){}else{
   for(int penaltyRow=0;penaltyRow<Map.MAPCHIP_COAT_HNUM;penaltyRow++){
    for(int penaltyColumn=0;penaltyColumn<Map.MAPCHIP_COAT_WNUM;penaltyColumn++){
     coatEva_new_offence[penaltyRow][penaltyColumn]=DEAFULT_FORMATION_EVA_PENALTYAREA[penaltyRow][penaltyColumn];
    }
   }
  }
 }else{
  for(int line2=0;line2<Map.MAPCHIP_COAT_HNUM;line2++){
   float factor2=GameUtil.RateConvert(line2,amefto_address_[0][1]+4,amefto_address_[0][1]-1,0,AMEFTO_PASSER_COATEVA[chargeNum_],true)+30;
   factor2=AppData.ClampMax((int)factor2,100);
   for(int cell2=0;cell2<Map.MAPCHIP_COAT_WNUM;cell2++){
    coatEva_new_offence[line2][cell2]=(int)factor2;coatEva_new_offence_passer[line2][cell2]=(int)factor2;
   }
  }
  if(!ChPAreaLineOver()){}else{
   for(int penaltyRow2=0;penaltyRow2<Map.MAPCHIP_COAT_HNUM;penaltyRow2++){
    for(int penaltyColumn2=0;penaltyColumn2<Map.MAPCHIP_COAT_WNUM;penaltyColumn2++){
     coatEva_new_offence[penaltyRow2][penaltyColumn2]=DEAFULT_FORMATION_EVA_PENALTYAREA[Map.MAPCHIP_COAT_HNUM-1-penaltyRow2][Map.MAPCHIP_COAT_WNUM-1-penaltyColumn2];
    }
   }
  }
 }
}
if(ch_ceva[2]==1){
 Team other=udat_.teams_[1-team_id_];
 float[] weights=CEVA_PASSER_MP2;
 for(int playerIndex=0;playerIndex<other.players_.Length;playerIndex++){
  Player player=other.players_[playerIndex];
  Cal_evaCoatMulti9block(coatEva_new_offence,player.address_[0],player.address_[1],weights);
  Cal_evaCoatMulti9block(coatEva_new_offence_passer,player.address_[0],player.address_[1],weights);
 }
}}
}