"""Source-only explicit empty dispatch default; no byte editing."""
from round10_smcs_branch_joins_source import lex,find,matching

def transform_default(text):
 t=lex(text);i,n=find(t,'if ((num3 == 2))');a=i+n
 if t[a].value!='{':raise ValueError('first arm requires block')
 b=matching(t,a);j=b+1
 if t[j].value!='else' or t[j+1].value!='if':raise ValueError('expected else-if chain')
 e=matching(t,j+2)
 cond=[x.value for x in t[j+3:e]]
 if cond!=['(','num3','==','1',')','||','(','num3','==','0',')']:raise ValueError('grouped selector order not proven')
 a=e+1
 if t[a].value!='{':raise ValueError('grouped arm requires block')
 b=matching(t,a)
 if t[b+1].value=='else':raise ValueError('default already present')
 payload=t[a+1:b]
 if any(x.value in ('return','continue','break','goto','try','yield','delegate')for x in payload):raise ValueError('non-fallthrough or protected arm')
 return text[:t[b].end]+' else { }'+text[t[b].end:]
