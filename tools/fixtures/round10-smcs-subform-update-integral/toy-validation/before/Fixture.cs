public class Data{public bool League_update(){return true;}}
public class FanActData{public int value;}
public class Fixture{
 Data udat_=new Data();int select_,type_,frame_;int[] fw_cnt=new int[]{0,1,0};
 FanActData fanActData_=new FanActData();
 static int[] FA2_ANIME_TIME=new int[]{0,1,2,3,4,5,6};int state;
 void Use(int n){state+=n;}bool Pop(){return state>0;}bool Update2(){return state<0;}
 int[] Get(int n){return new int[]{n};}
 public bool Run(bool first,bool second,bool pulse){
 bool flag2=first;bool flag3=second;
 if(flag2){Use(2);}else if(!flag3){Use(3);}
 if(!udat_.League_update()){}
 if(pulse){
  if(select_!=0){return Pop();}
  int local=3;Use(local);
 }
 if(first){
  FanActData fanActData2=fanActData_;
  if(select_!=0){return Pop();}
  string label="tail";Use(label.Length+fanActData2.value);Pop();
 }
 int num50=0;int num49=1;
 for(int m=0;m<fw_cnt.Length*2;m++){
  if(fw_cnt[m % fw_cnt.Length]<=0){
   if(num50==num49){int rate=m+1;Use(rate);break;}
   num50++;
  }
 }
 Use(num50);
 if(pulse){Use(1);}else{
  if(type_!=41){return Update2();}
  if(frame_<FA2_ANIME_TIME[4]){frame_=FA2_ANIME_TIME[4];}else{
   if(frame_>=FA2_ANIME_TIME[6]){
    int rank_=1;
    for(int n=0;n<4;n++){int[] array=Get(n);Use(array[0]+rank_);}
    return Pop();
   }
   frame_=FA2_ANIME_TIME[6]-1;
  }
 }
 return true;
}
}