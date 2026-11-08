public class Vec {public float x_,y_,z_;public void Set(float x,float y,float z){x_=x;y_=y;z_=z;} }
public class TBall {public Vec pos_,v_,a_;public void ReceiveBall(Fixture p){}public void ReleaseBall(Fixture p){} }
public class Data {public int[] GetCollisionData(int a,int b){return new int[4];} }
public class Player {public static Data udat_;}
public static class BitUtil {public static bool Check(int flags,int mask){return(flags&mask)!=0;} }
public static class GameUtil {
public static bool ChRectHitRect(float a,float b,float c,float d,float e,float f,float g,float h,bool z){return a>e;}
public static int Random(int max){return max;}
public static int RateConvert(int x,int a,int b,int c,int d,bool e){return x;}
public static float RateConvert(float x,float a,float b,float c,float d,bool e){return x;}
public static float Cos(int x){return x;}public static float Sin(int x){return x;}
public static float SetVStop(float distance,int time){return distance;}public static float SetAStop(float distance,int time){return distance;}
}
public class Fixture {public Vec pos_;public int flag_,team_id_;
public void Run(TBall tball){
			int[] collisionData = Player.udat_.GetCollisionData(0, 0);
			int[] collisionData2 = Player.udat_.GetCollisionData(1, 1);
			if (GameUtil.ChRectHitRect(pos_.x_ + (float)collisionData[0], pos_.z_ + (float)collisionData[1], collisionData[2], collisionData[3], tball.pos_.x_ + (float)collisionData2[0], tball.pos_.z_ + (float)collisionData2[1], collisionData2[2], collisionData2[3], true) && BitUtil.Check(flag_, 8))
			{
				switch (GameUtil.Random(3))
				{
				case 2:
					tball.v_.Set(0f, 0f, 0f);
					tball.ReceiveBall(this);
					break;
				case 1:
				{
					int angle2 = GameUtil.Random(180) + team_id_ * 180;
					float num2 = GameUtil.RateConvert(GameUtil.Random(100), 0f, 99f, 5f, 10f, true);
					int x2 = GameUtil.Random(100);
					float dis2 = GameUtil.RateConvert(x2, 0, 99, 5, 20, true);
					int t2 = GameUtil.RateConvert(x2, 0, 99, 5, 10, true);
					tball.ReceiveBall(this);
					tball.ReleaseBall(this);
					tball.v_.Set(num2 * GameUtil.Cos(angle2), 0f, num2 * GameUtil.Sin(angle2));
					tball.v_.y_ = GameUtil.SetVStop(dis2, t2);
					tball.a_.y_ = GameUtil.SetAStop(dis2, t2);
					break;
				}
				default:
				{
					int angle = GameUtil.Random(90) + 45 + (1 - team_id_) * 180;
					float num = GameUtil.RateConvert(GameUtil.Random(100), 0f, 99f, 2.5f, 5f, true);
					int x = GameUtil.Random(100);
					float dis = GameUtil.RateConvert(x, 0, 99, 2, 10, true);
					int t = GameUtil.RateConvert(x, 0, 99, 5, 10, true);
					tball.ReceiveBall(this);
					tball.ReleaseBall(this);
					tball.v_.Set(num * GameUtil.Cos(angle), 0f, num * GameUtil.Sin(angle));
					tball.v_.y_ = GameUtil.SetVStop(dis, t);
					tball.a_.y_ = GameUtil.SetAStop(dis, t);
					break;
				}
				}
			}
		}
}