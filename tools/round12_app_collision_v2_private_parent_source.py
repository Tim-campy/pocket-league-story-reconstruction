"""Source-only partial Collision repair: direct guard and runtime if dispatch."""
import re
RULE='r12-collision-direct-guard-and-catch-result-if-chain'
METHOD_ID='game.Player::Collision()->System.Void'

def close(source,start):
 depth=0
 for m in re.finditer(r'//[^\r\n]*|/\*[\s\S]*?\*/|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'|[{}]',source[start:]):
  if m[0]=='{':depth+=1
  elif m[0]=='}':
   depth-=1
   if not depth:return start+m.start()
 raise ValueError('unbalanced source')

def transform(source):
 marker='{ bool recoveredCondition_54620 = udat_.gm_.state_ == 0 && flag;\nif (recoveredCondition_54620)'
 if source.count(marker)!=1:raise ValueError('exact previously introduced materialized condition required')
 start=source.index(marker);ending=close(source,start)
 source=source[:start]+source[start+2:ending]+source[ending+1:]
 declaration='bool recoveredCondition_54620 = udat_.gm_.state_ == 0 && flag;\nif (recoveredCondition_54620)'
 source=source.replace(declaration,'if (udat_.gm_.state_ == 0 && flag)',1)
 marker='switch (PlayerData.GetCatchProc(this, udat_.ball_.v_len_, front))'
 if source.count(marker)!=1:raise ValueError('one exact catcher dispatch required')
 start=source.index(marker);opening=source.index('{',start);ending=close(source,opening);body=source[opening+1:ending]
 match=re.fullmatch(r'\s*case 2:(?P<two>[\s\S]*?)break;\s*case 1:\s*\{(?P<one>[\s\S]*?)break;\s*\}\s*default:\s*\{(?P<other>[\s\S]*?)break;\s*\}\s*',body)
 if not match:raise ValueError('exact case2/case1/default scope layout required')
 one=match['one'];other=match['other']
 for name,value in [('angle2',one),('angle',other)]:
  pattern=re.compile(r'(int '+name+r' = [^;]+;)')
  if len(pattern.findall(value))!=1:raise ValueError('one angle declaration required')
  value=pattern.sub(r'\1\nfloat recoveredUnusedVelocityX;\nfloat recoveredUnusedVelocityY;',value,1)
  if name=='angle2':one=value
  else:other=value
 replacement='int recoveredCatchResult = PlayerData.GetCatchProc(this, udat_.ball_.v_len_, front);\nif (recoveredCatchResult == 2)\n{'+match['two']+'\n}\nelse if (recoveredCatchResult == 1)\n{'+one+'\n}\nelse\n{'+other+'\n}'
 return source[:start]+replacement+source[ending+1:]
