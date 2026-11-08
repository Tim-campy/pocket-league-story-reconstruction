"""Pure explicit range else and score-local declaration roles."""
import re
RULE='r12-sliding-eva-empty-positive-range-and-root-score-locals'
METHOD_ID='game.Player::ChSlidingTackleEva(game.Player)->System.Boolean'

def once(source,before,after):
 if source.count(before)!=1:raise ValueError('Unique source anchor required: '+before)
 return source.replace(before,after,1)

def transform(source):
 pattern=re.compile(r'if \(!\(num >= 30f\) \|\| !\(num < 70f\)\)\s*\{\s*return false;\s*\}')
 matches=list(pattern.finditer(source))
 if len(matches)!=1:raise ValueError('One measured floating range guard required')
 m=matches[0];replacement='int num4;\nint num5;\nint num8;\nif (num >= 30f && num < 70f) { } else { return false; }'
 source=source[:m.start()]+replacement+source[m.end():]
 source=once(source,'int num4 = (int)GameUtil.RateConvert(x, 0f, -1f, 0f, 100f, true);','num4 = (int)GameUtil.RateConvert(x, 0f, -1f, 0f, 100f, true);')
 source=once(source,'int num5 = 0;','num5 = 0;')
 source=once(source,'int num8 = GameUtil.RateConvert(num7 - num6, 20, 0, 0, 100, true);','num8 = GameUtil.RateConvert(num7 - num6, 20, 0, 0, 100, true);')
 return source
