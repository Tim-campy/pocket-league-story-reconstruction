public class LeagueData { public int schedule_; }
public class QuestMatchData { public int schedule_; }
public class Fixture {
 public int state, selector, errLine_;
 public LeagueData league = new LeagueData();
 public QuestMatchData quest = new QuestMatchData();
 public void Sink(int n) { errLine_ += n; }
 public int Get() { return selector; }
 public void Run() { errLine_ = 10;
int x, y, unused, bottom;
x = y = unused = bottom = 0;
if(state == 0) { int side = Get(); Sink(side); }
bottom = Get();
int mode = Get();
if(mode != 2) {
 int slide = Get();
 int remaining = 0;
 if(mode == 0) {
   LeagueData leagueData = league;
   remaining = leagueData.schedule_ - slide;
 } else if(mode == 1) {
   QuestMatchData questMatchData = quest;
   remaining = questMatchData.schedule_ - slide;
 }
 Sink(remaining);
}
if(state == 3) {
 x = 0;
 y = 0;
 for(int row=0;row<2;row++) {
  for(int column=0;column<3;column++) { x = 120*column; y=40*row; Sink(x+y); }
 }
}
Sink(y + bottom); }
}