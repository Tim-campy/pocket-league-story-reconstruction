"""Composed source control and missing named Boolean; original bytes never emitted."""
from round10_residual30_guard_dispatch_source import if_node,masked,owner,block
from round10_residual31_collision_source import patch_tokens

def transform_ranking(text):
 text=patch_tokens(text,'if (playerData.game_goalInLeague_ < array3[0]) { return; } break;','if (playerData.game_goalInLeague_ >= array3[0]) { break; }')
 a,_,b,e=if_node(text,'playerData.game_goalInLeague_<=0');pb,pe=owner(text,a,'l')
 if masked(text[b+1:e-1]).strip()!='continue;':raise ValueError('not empty continue guard')
 tail=text[e:pe-1]
 if not tail.strip():raise ValueError('missing positive tail')
 return text[:a]+'if (playerData.game_goalInLeague_ > 0) {'+tail+'\n}'+text[pe-1:]

def transform_repetition(text):
 condition='(((player.address_[0]-ball_.address_[0]<-2)||(player.address_[0]-ball_.address_[0]>2))||(player.address_[1]-ball_.address_[1]<-2))||(player.address_[1]-ball_.address_[1]>2)'
 a,_,b,e=if_node(text,condition);owner(text,a,'i')
 if masked(text[b+1:e-1]).strip()!='continue;':raise ValueError('range guard contains work')
 replacement='if (player.address_[0] - ball_.address_[0] < -2 || player.address_[0] - ball_.address_[0] > 2) { continue; }\nif (player.address_[1] - ball_.address_[1] < -2 || player.address_[1] - ball_.address_[1] > 2) { continue; }'
 text=text[:a]+replacement+text[e:]
 return patch_tokens(text,'if (!(player.ChTrapOrHeading())) { continue; } hitList.AddElement(player);','if (player.ChTrapOrHeading()) { hitList.AddElement(player); }')

def transform_reflect(text):
 a,_,b,e=if_node(text,'apdat_.sysSave_.bins[4].Length<=0')
 if masked(text[b+1:e-1]).strip()!='return;':raise ValueError('root guard contains work')
 clean=masked(text);opening=clean.index('{');_,end=block(text,opening)
 stack=[]
 for i,c in enumerate(clean[:a]):
  if c=='{':stack.append(i)
  elif c=='}':stack.pop()
 if stack!=[opening]:raise ValueError('guard is not root')
 tail=text[e:end-1]
 tail=patch_tokens(tail,'if (StreamUtil.ReadBoolean(byteArrayInputStream5))','bool recoveredTradePresent = StreamUtil.ReadBoolean(byteArrayInputStream5); if (recoveredTradePresent)')
 return text[:a]+'if (apdat_.sysSave_.bins[4].Length > 0) {'+tail+'\n}'+text[end-1:]
