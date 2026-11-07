"""Whole-method source composition from fourteen diagnostic regions."""
from round10_residual30_guard_dispatch_source import if_node,masked,block
from round10_residual31_collision_source import patch_tokens
from round10_smcs_branch_joins_source import lex,find,matching

def enclosing(text,offset):
 clean=masked(text);stack=[]
 for i,c in enumerate(clean[:offset]):
  if c=='{':stack.append(i)
  elif c=='}':stack.pop()
 if not stack:raise ValueError('no containing block')
 return block(text,stack[-1])

def transform_draw(text):
 a,_,b,e=if_node(text,'num2<=6');c,_,d,f=if_node(text,'num2>36');pb,pe=enclosing(text,a)
 if masked(text[b+1:e-1]).strip()!='return;'or masked(text[d+1:f-1]).strip()!='return;'or masked(text[e:c]).strip()or enclosing(text,c)!=(pb,pe):raise ValueError('range guards not consecutive in same block')
 text=text[:a]+'if (num2 > 6 && num2 <= 36) {'+text[f:pe-1]+'\n}'+text[pe-1:]
 needle='num5 = ((!Language.English()) ? GameUtil.RateConvert(num2, 6, 14, gameView.GetGameWidth(), gameView.GetGameWidth() / 2 - 42, true) : GameUtil.RateConvert(num2, 6, 14, gameView.GetGameWidth(), gameView.GetGameWidth() / 2 - 49, true));'
 replacement='if (Language.English()) { num5 = GameUtil.RateConvert(num2, 6, 14, gameView.GetGameWidth(), gameView.GetGameWidth() / 2 - 49, true); } else { num5 = GameUtil.RateConvert(num2, 6, 14, gameView.GetGameWidth(), gameView.GetGameWidth() / 2 - 42, true); }'
 text=patch_tokens(text,needle,replacement)
 a,_,b,e=if_node(text,'state_!=10');pb,pe=enclosing(text,a)
 if masked(text[pb+1:a]).strip()or masked(text[b+1:e-1]).strip()!='return;':raise ValueError('state guard not first command of else')
 clean=masked(text);prefix=clean[:pb].rstrip()
 if not prefix.endswith('else'):raise ValueError('state arm owner is not final else')
 start=len(prefix)-4;text=text[:start]+'else if (state_ == 10) {'+text[e:pe-1]+text[pe-1:]
 t=lex(text);i,n=find(t,'switch (num12)');a=i+n;b=matching(t,a)
 expected='case 0 : case 2 : num13 = 0 ; break ; case 1 : num13 = 1 ; break ; default : num13 = 2 ; break ;'
 if [x.value for x in t[a+1:b]]!=[x.value for x in lex(expected)]:raise ValueError('dispatch payload/order differs')
 text=text[:t[i].start]+'if (num12 == 0 || num12 == 2) { num13 = 0; } else if (num12 == 1) { num13 = 1; } else { num13 = 2; }'+text[t[b].end:]
 text=patch_tokens(text,'if ((stateCnt_ - WAIT_CARD[2]) % 12 / 6 == 0)','int recoveredCardFrame = (stateCnt_ - WAIT_CARD[2]) % 12 / 6; if (recoveredCardFrame == 0)')
 return text
