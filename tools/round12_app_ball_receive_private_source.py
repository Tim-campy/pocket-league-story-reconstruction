"""Pure restoration of two explicit bounds guards, preserving float operators."""
import re
RULE='r12-ball-receive-two-original-bounds-return-guards'
METHOD_ID='game.Ball::ReceiveBall(game.Player,System.Boolean)->System.Void'
CONDITION='if ((udat_.gm_.state_ != 0 && coatOut_ok) || (!(pos_.z_ >= 900f) && !(pos_.z_ <= -900f) && !(pos_.x_ <= -450f) && !(pos_.x_ >= 450f) && !(player.pos_.z_ >= 900f) && !(player.pos_.z_ <= -900f) && !(player.pos_.x_ <= -450f) && !(player.pos_.x_ >= 450f)))'

def close(source,start):
 depth=0
 for m in re.finditer(r'//[^\r\n]*|/\*[\s\S]*?\*/|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'|[{}]',source[start:]):
  if m[0]=='{':depth+=1
  elif m[0]=='}':
   depth-=1
   if not depth:return start+m.start()
 raise ValueError('unbalanced source')

def transform(source):
 if source.count(CONDITION)!=1:raise ValueError('One exact merged bounds condition required')
 start=source.index(CONDITION);opening=source.index('{',start);end=close(source,opening);body=source[opening+1:end]
 if source[end+1:].strip()!='}':raise ValueError('Merged condition must own the complete remaining method body')
 guards='if (udat_.gm_.state_ == 0 || !coatOut_ok)\n{\nif (pos_.z_ >= 900f || pos_.z_ <= -900f || pos_.x_ <= -450f || pos_.x_ >= 450f) { return; }\nif (player.pos_.z_ >= 900f || player.pos_.z_ <= -900f || player.pos_.x_ <= -450f || player.pos_.x_ >= 450f) { return; }\n}\n'
 return source[:start]+guards+body+source[end+1:]
