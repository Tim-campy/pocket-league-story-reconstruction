public class LeagueData{public int[] matchEnemy_;}public class TeamData{public int war_offence_,war_deffence_;}
public class Fixture{int[][] league_fightCombi_;LeagueData league_;TeamData[] teams;int sink;int GetTeamScore(int a,int b){return a+b;}void Use(int a,int b,int c,int d){sink=a+b+c+d;}
public void Run(){LeagueData leagueData=league_;int num5;
int num6;
for (int i = 0; i < league_fightCombi_.Length; i++){
int num = league_fightCombi_[i][0];int num2 = league_fightCombi_[i][1];
int num3=leagueData.matchEnemy_[num];int num4=leagueData.matchEnemy_[num2];TeamData teamData=teams[num3];TeamData teamData2=teams[num4];
num5 = GetTeamScore(teamData.war_offence_, teamData2.war_deffence_);
num6 = GetTeamScore(teamData2.war_offence_, teamData.war_deffence_);
Use(num,num2,num5,num6);Use(num,num2,num5,num6);Use(num,num2,num5,num6);}
}}
