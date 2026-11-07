"""Source-only lambda-local lifetime compositions, no generated type authored."""
from round10_smcs_branch_joins_source import lex,find,matching
from round10_residual30_guard_dispatch_source import owner

def splice(text,t,i,n,replacement):return text[:t[i].start]+replacement+text[t[i+n-1].end:]
def transform_use(text):
 t=lex(text);i,n=find(t,'byte[] array = src_;');text=splice(text,t,i,n,'byte[] array = src_; int num2; int num3;')
 for needle,replacement in [('int num2 = sourceImageMaxCount_;','num2 = sourceImageMaxCount_;'),('int num3 = sourceImageMaxPixels_;','num3 = sourceImageMaxPixels_;')]:
  t=lex(text);i,n=find(t,needle);text=splice(text,t,i,n,replacement)
 return text

def transform_capture(text):
 t=lex(text);decl,_=find(t,'bool flag = false;');i,n=find(t,'while (!File.Exists(path))');owner(text,t[i].start,'i');a=i+n
 if t[a].value!='{':raise ValueError('waiting loop needs block')
 b=matching(t,a)
 if [x.value for x in t[a+1:b]]!=['JThread','.','Sleep','(','10L',')',';']:raise ValueError('waiting body differs')
 j=b+1
 if [x.value for x in t[j:j+5]]!=['if','(','!','(','bool']:raise ValueError('expected bool result guard')
 c=matching(t,j+1);start=c+1;end=matching(t,start)
 if [x.value for x in t[start+1:end]]!=['i','--',';']:raise ValueError('retry owner/body differs')
 if decl>=i:raise ValueError('Boolean declaration not in original prefix')
 expression=text[t[j+3].start:t[c-1].end]
 if 'delegate'not in expression or 'RunOnUiThread'not in expression:raise ValueError('missing captured callback')
 replacement='while (true) { if (File.Exists(path)) { flag = '+expression+'; break; } JThread.Sleep(10L); } if (!flag) { i--; }'
 return text[:t[i].start]+replacement+text[t[end].end:]
