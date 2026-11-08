public class Vec { public float x_,z_;public void Set(float x,float y,float z){} }
public class Player {public Vec pos_;public int[] formation_;}
public class Ball {public void ReceiveBall(Player p){} }
public class GM {public int gameState_;}
public class UserData {public GM gm_;public Ball ball_;}
public class Fixture {public UserData udat_;public int team_id_;public Player[] players_;public static Vec[] FORMATION_KICKOFF;
public void Run(){if(udat_.gm_.gameState_==team_id_){
 if(team_id_==0){players_[players_.Length-1].pos_.Set(FORMATION_KICKOFF[0].x_,0f,FORMATION_KICKOFF[0].z_);}
 else{
  if(players_[players_.Length-2].formation_[0]<=50){players_[players_.Length-2].pos_.Set(FORMATION_KICKOFF[1].x_,0f,-FORMATION_KICKOFF[1].z_);}
  else{players_[players_.Length-2].pos_.Set(-FORMATION_KICKOFF[1].x_,0f,-FORMATION_KICKOFF[1].z_);}
 }
 udat_.ball_.ReceiveBall(players_[players_.Length-1]);
}else{}}
}