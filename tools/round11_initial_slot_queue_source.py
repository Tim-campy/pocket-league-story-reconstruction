"""Five reviewed source layouts. Pure source editing, no target compilation."""
import re
def tokens(s):
 p=r'//[^\n]*|/\*[\s\S]*?\*/|@"(?:[^"]|"")*"|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'|[A-Za-z_][A-Za-z_0-9]*|\d+(?:\.\d+)?[fFdDmMuUlL]*|==|!=|<=|>=|&&|\|\||\+\+|--|[^\s]'
 return [(m.group(),m.start(),m.end()) for m in re.finditer(p,s) if not m.group().startswith(('//','/*'))]
def replace(s,old,new):
 ts=tokens(s);w=[t[0] for t in tokens(old)];hits=[i for i in range(len(ts)-len(w)+1) if [t[0] for t in ts[i:i+len(w)]]==w]
 if len(hits)!=1:raise ValueError('template-not-unique:'+old[:80])
 i=hits[0];return s[:ts[i][1]]+new+s[ts[i+len(w)-1][2]:]
def boot_layout(s):
 s=replace(s,'global::kairo.unity.io.RecordStore recordStore; JarInflater jarInflater = null;','string[] array; JarInflater jarInflater = null;')
 s=replace(s,'recordStore = RecordStore.OpenRecordStore(1);','RecordStore recordStore = RecordStore.OpenRecordStore(1);')
 return replace(s,'string[] array = StringUtil.LoadStringArray(jarInflater,"player.txt");','array = StringUtil.LoadStringArray(jarInflater,"player.txt");')
def app_draw_layout(s):
 s=replace(s,'global::kairo.unity.ui.Graphics graphics; Canvas instance = Canvas.GetInstance(); graphics = instance.BeginPaint();','string text; Canvas instance = Canvas.GetInstance(); Graphics graphics = instance.BeginPaint();')
 ts=tokens(s);hits=[i for i in range(len(ts)-3) if [x[0] for x in ts[i:i+3]]==['string','text','=']]
 if len(hits)!=2:raise ValueError('requires-two-branch-text-declarations')
 for i in reversed(hits):s=s[:ts[i][1]]+s[ts[i+1][1]:]
 return s
def switch_to_if(s,selector,allowed_cases):
 ts=tokens(s);hits=[]
 for i in range(len(ts)-4):
  if [x[0] for x in ts[i:i+5]]!=['switch','(',selector,')','{']:continue
  level=1;j=i+5;labels=[];hasdefault=False
  while j<len(ts) and level:
   v=ts[j][0]
   if level==1 and v=='default':hasdefault=True
   if level==1 and v=='case':
    if not ts[j+1][0].isdigit() or ts[j+2][0]!=':':raise ValueError('non-Int32-case')
    labels.append((int(ts[j+1][0]),j,j+3))
   if v=='{':level+=1
   elif v=='}':level-=1
   j+=1
  if hasdefault or [a for a,_,_ in labels]!=allowed_cases:continue
  sections=[];values=[]
  for n,(value,_,begin) in enumerate(labels):
   end=labels[n+1][1] if n+1<len(labels) else j-1
   values.append(value)
   if begin==end:continue
   body=s[ts[begin][1]:ts[end-1][2]];bt=tokens(body)
   terminal=len(bt)-3 if bt[0][0]=='{' and bt[-1][0]=='}' else len(bt)-2
   if terminal<0 or [x[0] for x in bt[terminal:terminal+2]]!=['break',';']:raise ValueError('no-terminal-owned-break')
   body=body[:bt[terminal][1]]+body[bt[terminal+1][2]:]
   if any(t[0] in ['break','continue','goto','return','yield'] for t in tokens(body)):raise ValueError('other-transfer')
   sections.append((values,body));values=[]
  if values:raise ValueError('unbound-labels')
  new=' '.join(('if' if n==0 else 'else if')+' ('+' || '.join(selector+' == '+str(v) for v in values)+') {'+body+'}' for n,(values,body) in enumerate(sections))
  hits.append((ts[i][1],ts[j-1][2],new))
 if len(hits)!=1:raise ValueError('approved-switch-not-unique:'+str(allowed_cases))
 a,b,new=hits[0];return s[:a]+new+s[b:]
def gamepad_layout(s):
 s=replace(s,'{ bool recoveredCondition_12162 = text == null; if (recoveredCondition_12162) { text = canvas_.GetSoftLabel(3); } }','if(text == null){text=canvas_.GetSoftLabel(3);}')
 s=switch_to_if(s,'id',[0,5,1,2,3,11,12,13,14,15,16,17,18,19,20,4])
 s=switch_to_if(s,'id',[21,22])
 return switch_to_if(s,'id',[2,3,4])
def billing_concat(s):
 return replace(s,'string text2 = text; text = text2 + purchaseRecord.productId_ + "," + purchaseRecord.orderId_ + "," + purchaseRecord.state_;','text += purchaseRecord.productId_ + "," + purchaseRecord.orderId_ + "," + purchaseRecord.state_;')
def save_concat_and_dispatch(s):
 s=replace(s,'string text10 = text8; text8 = text10 + text9 + "," + Http.GetErrorCode() + "\\n";','text8 += text9 + "," + Http.GetErrorCode() + "\\n";')
 s=switch_to_if(s,'type',[1,2])
 return switch_to_if(s,'type',[1,2,0])
