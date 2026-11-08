public class LeagueData{public int[] matchEnemy_;}public class TeamData{public int war_offence_,war_deffence_;}
public class Fixture{int[][] league_fightCombi_;LeagueData league_;TeamData[] teams;int sink;int GetTeamScore(int a,int b){return a+b;}void Use(int a,int b,int c,int d){sink=a+b+c+d;}
public void Run(){LeagueData leagueData=league_;int secondScore;int firstScore;for(int iteration=0;iteration<league_fightCombi_.Length;iteration++){
int first=league_fightCombi_[iteration][0];int second=league_fightCombi_[iteration][1];int enemyA=leagueData.matchEnemy_[first];int enemyB=leagueData.matchEnemy_[second];TeamData a=teams[enemyA];TeamData b=teams[enemyB];firstScore=GetTeamScore(a.war_offence_,b.war_deffence_);secondScore=GetTeamScore(b.war_offence_,a.war_deffence_);Use(first,second,firstScore,secondScore);Use(first,second,firstScore,secondScore);Use(first,second,firstScore,secondScore);}
}}
