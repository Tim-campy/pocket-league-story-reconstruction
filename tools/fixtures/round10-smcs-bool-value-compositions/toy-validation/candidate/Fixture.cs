using System;
public class Player{public int role_,seb_;public float a,b,temp_len;}
public class Ball{public float a,b;}
public class Fixture{
 Player[] players_=new Player[3];Ball ball=new Ball();Player[] sorted=new Player[3];int count;
 float Distance(float a,float b){return (float)Math.Sqrt(a*a+b*b);}void Add(Player p){sorted[count++]=p;}
 public void Run(){
 Ball ball_=ball;float num;float num2;
 for(int i=0;i<players_.Length;i++){
  if (players_[i].role_ == 0) { continue; }

    Player player=players_[i];
if (player.seb_ == 12) { continue; }

     num=Distance(ball_.a-player.a,ball_.b-player.b);
     num2=Distance(player.a-ball_.b,player.b-ball_.a);
     player.temp_len=(num*0.7f+num2*0.3f)/(0.7f+0.3f);
     Add(player);

 }
 for(int j=0;j<count-1;j++){
  for(int num3=count-1;num3>j;num3--){
   Player player2=sorted[j];Player player3=sorted[num3];
   if(player3.temp_len>player2.temp_len){sorted[j]=player3;sorted[num3]=player2;}
  }
 }
}
}