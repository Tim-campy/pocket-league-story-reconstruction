public class FormBase{public int value;}
public class Fixture{
 int[] et_cur=new int[4];int[][] formation=new int[][]{new int[2],new int[2]};
 void Use(FormBase value){value.value++;}
 public bool Run(bool isSoccerForm){
 if(!isSoccerForm){
  {bool recoveredCondition_68130=et_cur[0]%3==0;
   if(recoveredCondition_68130){et_cur[0]+=2;}else{et_cur[0]=(et_cur[0]-1+6)%6;}
  }
  if(et_cur[0]!=5){
   if(et_cur[0]==0){FormBase topForm=new FormBase();Use(topForm);}
   else if(et_cur[0]==1){FormBase topForm2=new FormBase();Use(topForm2);}
   else if(et_cur[0]==3){FormBase topForm3=new FormBase();Use(topForm3);}
  }
 }
 if(isSoccerForm){
  int num=et_cur[2];int[] array3=null;int[] array4=null;
  for(int i=0;i<formation.Length;i++){
   if(formation[i][0]==num){array3=formation[i];}
   if(formation[i][1]==num){array4=formation[i];}
  }
  if(array3!=null&&array4!=null){int swap=array3[0];array3[0]=array4[0];array4[0]=swap;}
  else if(array3!=null){array3[0]=et_cur[3]/10;array3[1]=et_cur[3]%10;}
  else if(array4!=null){array4[0]=et_cur[2]/10;array4[1]=et_cur[2]%10;}
  et_cur[2]=et_cur[3];
 }
 return false;
}
}