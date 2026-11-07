"""Pure named-NotifyManager source prototype; no C# emission or model calls.

The importable API does no I/O. main() alone binds current/frozen source and
writes this WIP bundle. This is not a registered/READY transformation family.
"""
from collections import Counter
import re

TOKEN=re.compile(r'//[^\r\n]*|/\*[\s\S]*?\*/|@"(?:""|[^"])*"|"(?:\\.|[^"\\])*"|\x27(?:\\.|[^\x27\\])*\x27|[A-Za-z_][A-Za-z_0-9]*|[^\s]',re.M)
def tokens(s):return [(m.group(),m.start(),m.end())for m in TOKEN.finditer(s)if not m.group().startswith(('//','/*'))]
def mask(s):
 result=list(s)
 for m in TOKEN.finditer(s):
  if m.group().startswith(('//','/*','"','@"',"'")):
   for i in range(m.start(),m.end()):
    if result[i]not in '\r\n':result[i]=' '
 return ''.join(result)
def close(s,opening):
 assert s[opening]=='{';masked=mask(s);depth=0
 for i in range(opening,len(s)):
  if masked[i]=='{':depth+=1
  elif masked[i]=='}':
   depth-=1
   if not depth:return i+1
 raise AssertionError('unclosed lexical block')
def literal_inventory(s):return Counter(t for t,_,_ in tokens(s)if t.startswith(('"','@"',"'")))
def call_inventory(s):
 words=[t for t,_,_ in tokens(s)]
 return Counter(t for i,t in enumerate(words[:-1])if re.fullmatch('[A-Za-z_][A-Za-z_0-9]*',t)and words[i+1]=='('and t not in {'if','for','foreach','while','switch','catch','using','lock','typeof','sizeof','default','checked','unchecked'})
def enclosing_try(s,offset):
 masked=mask(s);stack=[]
 for i in range(offset):
  if masked[i]=='{':stack.append(i)
  elif masked[i]=='}':stack.pop()
 assert len(stack)>=2,'receiver must be inside a try block, not the method block'
 owner=stack[-1];previous=tokens(masked[:owner])[-1][0]
 assert previous=='try','receiver must be direct child of existing try block'
 return owner

def calculate_notify_receiver(method_source):
 old='NotifyManager.GetInstance()?.UpdateValues();'
 assert method_source.count(old)==1,'require one exact NotifyManager conditional invocation'
 a=method_source.index(old);masked=mask(method_source)
 assert masked[a:a+len(old)]==old,'target cannot be in a literal/comment'
 assert method_source.count('NotifyManager.GetInstance()')==1,'GetInstance must occur once for this receiver'
 owner=enclosing_try(method_source,a)
 prefix=method_source[method_source.rfind('\n',0,a)+1:a]
 assert not prefix.strip(),'standalone expression after indentation required'
 identifiers={t for t,_,_ in tokens(method_source)if re.fullmatch('[A-Za-z_][A-Za-z_0-9]*',t)}
 name='recoveredNotifyManager';ordinal=2
 while name in identifiers:
  name='recoveredNotifyManager'+str(ordinal);ordinal+=1
 assert name not in identifiers
 line_end=method_source.find('\n',a+len(old))
 newline='\r\n'if line_end>0 and method_source[line_end-1]=='\r'else'\n'
 after='NotifyManager '+name+' = NotifyManager.GetInstance();'+newline+prefix+'if ((object)'+name+' != null)'+newline+prefix+'\t'+name+'.UpdateValues();'
 result=method_source[:a]+after+method_source[a+len(old):]
 assert enclosing_try(result,a)==owner,'new declaration must remain sibling in same try'
 assert result.count('{')==method_source.count('{')and result.count('}')==method_source.count('}'),'no child-block wrapper permitted'
 assert literal_inventory(method_source)==literal_inventory(result),'string/character literal changed'
 assert call_inventory(method_source)==call_inventory(result),'method-call name inventory changed'
 assert result.count('NotifyManager.GetInstance()')==1
 return dict(after=result,start=a,end=a+len(old),before=old,replacement=after,fresh_name=name,try_opening=owner,checks=dict(unique_target=True,fresh_identifier=True,receiver_evaluated_once=True,parent_try_unchanged=True,no_added_child_block=True,literal_token_multiset_unchanged=True,call_name_token_multiset_unchanged=True,object_cast_reference_null_check=True),ready=False,complete=None)

def transform_notify_receiver(method_source):
 """Pure method-text API, independent of method/header/declaring class."""
 return calculate_notify_receiver(method_source)['after']

