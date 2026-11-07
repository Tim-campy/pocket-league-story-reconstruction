using System;
public class Pos {public float y_;}
public class PlayerLike {public int coliBallState_,direct_,team_id_;public int[]address_=new int[2];}
public class BallLike {public Pos pos_=new Pos();public int[] address_=new int[2];}
public class GameUtil {public static int Random(int n){return n;}public static bool ChRectHitRect(PlayerLike p){return p.direct_>0;}}
public class Fixture {BallLike ball_=new BallLike();PlayerLike[] players=new PlayerLike[1];float total;void Add(PlayerLike p){total+=p.direct_;}
public void Run(int num3){for(int j=0;j<players.Length;j++){PlayerLike player=players[j];if(player.coliBallState_==4){continue;}if(player.address_[0]-ball_.address_[0] < -1 || player.address_[0]-ball_.address_[0] > 1){continue;}if(player.address_[1]-ball_.address_[1] < -1 || player.address_[1]-ball_.address_[1] > 1){continue;}float num=3f;if(ball_.pos_.y_ >= num){continue;}if(GameUtil.ChRectHitRect(player)){Add(player);}}
PlayerLike player3=players[0];if(num3==2){total=2;}else if(num3==1||num3==0){int num4=30;int n=120;float num5=3f;float num6=0f;
if(num3==1){num6=(float)GameUtil.Random(9);}else if(num3==0){num6=(float)GameUtil.Random(9);}
num5*=num6;int angle=0;if(player3.team_id_==0){if(num3==1){angle=num4+GameUtil.Random(n);}else if(num3==0){if(player3.direct_==0){angle=190+GameUtil.Random(160);}else if(player3.direct_==1){angle=280+GameUtil.Random(70);}else{angle=190+GameUtil.Random(70);}}}
else{if(num3==1){angle=180+num4+GameUtil.Random(n);}else if(num3==0){if(player3.direct_==2){angle=10+GameUtil.Random(160);}else if(player3.direct_==1){angle=10+GameUtil.Random(70);}else{angle=100+GameUtil.Random(70);}}}
total=num5+angle;}total+=1;}

}
