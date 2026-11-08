"""Pure source restoration of one cached dispatch and its default-section join."""
import re
TOKEN=re.compile(r'//[^\r\n]*|/\*.*?\*/|@"(?:[^"]|"")*"|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'|[A-Za-z_]\w*|\d+|=>|!=|==|<=|>=|\+\+|--|&&|\|\||[^\s]',re.S)
def tokens(s):return [(m.group(),m.start(),m.end()) for m in TOKEN.finditer(s) if not m.group().startswith(('//','/*'))]
def span(s,pat):
 t=tokens(s);p=[x[0] for x in tokens(pat)];hits=[i for i in range(len(t)-len(p)+1) if [x[0] for x in t[i:i+len(p)]]==p]
 if len(hits)!=1:raise ValueError('unique reviewed structure required:'+pat)
 i=hits[0];return t[i][1],t[i+len(p)-1][2]
def match(t,i):
 depth=0
 for j in range(i,len(t)):
  if t[j][0]=='{':depth+=1
  elif t[j][0]=='}':
   depth-=1
   if depth==0:return j
 raise ValueError('unclosed block')
DELAY='delayEvent=new DelayEvent(GetDelayId(script),i+1,array[1],-1,save_.ints[9],args,argi);if(2<array.Length){delayEvent.hour_=array[2];}delayEvents_.AddElement(delayEvent);return subForm;'
TAIL='subForm=ExecuteScript(array,popup,args,argi,script);if((talkCharaIndex==-1)||(subForm==null)){continue;}if(subForm.humanCharaIndex_!=-1){continue;}subForm.humanCharaType_=talkCharaType;subForm.humanCharaIndex_=talkCharaIndex;'
STORES='subForm.humanCharaType_=talkCharaType;subForm.humanCharaIndex_=talkCharaIndex;'
def transform_execute(s):
 t=tokens(s)
 if any(x[0] in ('goto','delegate','=>','yield','await','try','catch','finally','switch') for x in t):raise ValueError('unreviewed capture/transfer/dispatch/EH')
 span(s,'if(script==null){return null;}');span(s,'SubForm subForm=null;');span(s,'DelayEvent delayEvent=null;');span(s,'for(int i=line;i<script.Length;i++){')
 da,db=span(s,'int num2=num;');ia,ib=span(s,'if(num2==5){');oi=next(i for i,x in enumerate(t) if x[2]==ib);ci=match(t,oi)
 if [x[0] for x in t[oi+1:ci]]!=[x[0] for x in tokens(DELAY)]:raise ValueError('exact delay section required')
 if any(x[0] not in (';',) for x in tokens(s[db:ia])):raise ValueError('adjacent dispatch alias required')
 ta,tb=span(s,TAIL)
 if s[t[ci][2]:ta].strip():raise ValueError('default payload must immediately follow dispatch arm')
 # The caller body must be the entire loop tail, followed solely by the root return.
 rest=[x[0] for x in tokens(s[tb:])]
 if rest!=['}','return','subForm',';','}']:raise ValueError('exact lexical for/method tail required')
 prefix=s[:da];delay=s[ib:t[ci][1]];call_a,call_b=span(s,'subForm=ExecuteScript(array,popup,args,argi,script);');sa,sb=span(s,STORES)
 new='switch (num) {\ncase 5:\n'+delay+'\ndefault:\n'+s[call_a:call_b]+'\nif (talkCharaIndex != -1 && subForm != null && subForm.humanCharaIndex_ == -1) {\n'+s[sa:sb]+'\n}\nbreak;\n}'
 return prefix+new+s[tb:]
