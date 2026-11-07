using System;
public class Pos {public float y_;}
public class PlayerLike {public int coliBallState_,direct_,team_id_;public int[]address_=new int[2];}
public class BallLike {public Pos pos_=new Pos();public int[] address_=new int[2];}
public class GameUtil {public static int Random(int n){return n;}public static bool ChRectHitRect(PlayerLike p){return p.direct_>0;}}
public class Fixture {BallLike ball_=new BallLike();PlayerLike[] players=new PlayerLike[1];float total;void Add(PlayerLike p){total+=p.direct_;}
public void Run(int num3){for(int j=0;j<players.Length;j++){PlayerLike player=players[j];if(player.coliBallState_ != 4 && player.address_[0] - ball_.address_[0] >= -1 && player.address_[0] - ball_.address_[0] <= 1 && player.address_[1] - ball_.address_[1] >= -1 && player.address_[1] - ball_.address_[1] <= 1){float num=3f;if (!(ball_.pos_.y_ >= num) && GameUtil.ChRectHitRect(player)){Add(player);}}}
PlayerLike player3=players[0];switch(num3){case 2:total=2;break;case 0:case 1:{int num4=30;int n=120;float num5=3f;float num6=0f;
switch(num3){case 1:num6=(float)GameUtil.Random(9);break;case 0:num6=(float)GameUtil.Random(9);break;}
num5*=num6;int angle=0;if(player3.team_id_==0){switch(num3){case 1:angle=num4+GameUtil.Random(n);break;case 0:angle=((player3.direct_ !=0)?((player3.direct_ !=1)?(190+GameUtil.Random(70)):(280+GameUtil.Random(70))):(190+GameUtil.Random(160)));break;}}
else{switch(num3){case 1:angle=180+num4+GameUtil.Random(n);break;case 0:angle=((player3.direct_ !=2)?((player3.direct_ !=1)?(100+GameUtil.Random(70)):(10+GameUtil.Random(70))):(10+GameUtil.Random(160)));break;}}
total=num5+angle;break;}}total+=1;}

}
