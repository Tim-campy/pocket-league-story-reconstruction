public class LeagueData { public int schedule_; }
public class QuestMatchData { public int schedule_; }
public class Fixture {
 public int state, selector, errLine_;
 public LeagueData league = new LeagueData();
 public QuestMatchData quest = new QuestMatchData();
 public void Sink(int n) { errLine_ += n; }
 public int Get() { return selector; }
 public void Run() { errLine_ = 10;
int num2;
int num3;
int num = (num2 = (num3 = 0));
if(state == 0) { int side = Get(); Sink(side); }
num3 = Get();
int matchInfoJoin = Get();
if(matchInfoJoin != 2) {
 int num9 = Get();
 int num10 = 0;
 switch (matchInfoJoin) {
 case 0: {
   LeagueData leagueData = league;
   num10 = leagueData.schedule_ - num9;
   break;
 }
 case 1: {
   QuestMatchData questMatchData = quest;
   num10 = questMatchData.schedule_ - num9;
   break;
 }
 }
 Sink(num10);
}
if(state == 3) {
 int num13 = 0;
 num = 0;
 for(int i=0;i<2;i++) {
  for(int j=0;j<3;j++) { num13 = 120*j; num=40*i; Sink(num13+num); }
 }
}
Sink(num + num3); }
}