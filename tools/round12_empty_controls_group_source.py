"""Two separate erased-body control hypotheses, not one generic rewrite."""
import re
RULE='r12-smcs-empty-control-recovery'
CASES=[('game.GameUtil::.ctor()->System.Void','src/Assembly-CSharp/game/GameUtil.cs','public GameUtil()','terminal-empty-if'),('game.UserData::SetMatchInfoJoin(System.Int32)->System.Void','src/Assembly-CSharp/game/UserData.cs','public void SetMatchInfoJoin(int matchInfoJoin)','three-empty-if-arms')]
def transform(method):
 if 'Config.DEBUG_CMD' in method:
  out,count=re.subn(r'if \(Config\.DEBUG_CMD\)\s*\{\s*return;\s*\}','if (Config.DEBUG_CMD) { }',method)
 elif 'SetMatchInfoJoin' in method:
  out,count=re.subn(r'switch \(matchInfoJoin\)\s*\{\s*\}','if (matchInfoJoin == 0) { } else if (matchInfoJoin == 1) { } else if (matchInfoJoin == 2) { }',method)
 else:raise ValueError('Unsupported exact source')
 if count!=1:raise ValueError('Unique control anchor required')
 return out
