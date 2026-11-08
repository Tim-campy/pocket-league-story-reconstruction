"""Pure, reviewed parent-source proposals. No target I/O or byte patching."""
import re
def tokens(s):
 p=r'//[^\n]*|/\*[\s\S]*?\*/|@"(?:[^"]|"")*"|"(?:\\.|[^"\\])*"|[A-Za-z_][A-Za-z_0-9]*|\d+(?:\.\d+)?[fFdDmMuUlL]*|==|!=|<=|>=|&&|\|\||[^\s]'
 return [(m.group(),m.start(),m.end()) for m in re.finditer(p,s) if not m.group().startswith(('//','/*'))]
def replace(s,old,new):
 ts=tokens(s);w=[t[0] for t in tokens(old)];hits=[i for i in range(len(ts)-len(w)+1) if [t[0] for t in ts[i:i+len(w)]]==w]
 if len(hits)!=1:raise ValueError('reviewed-template-not-unique:'+old[:60])
 i=hits[0];return s[:ts[i][1]]+new+s[ts[i+len(w)-1][2]:]
def transform_touch_event(s):
 old='''if ((type == 0)) { text = "TYPE_DOWN"; } else { text = ((type == 1) ? "TYPE_UP" : ((type == 2) ? "TYPE_ENTER" : ((type == 3) ? "TYPE_LEAVE" : ((type == 4) ? "TYPE_MOVE" : ((type == 5) ? "TYPE_CLICK" : ((type == 6) ? "TYPE_LOST" : ((type == 7) ? "TYPE_DRAG" : ((type == 8) ? "TYPE_GRID_DRAG" : ((type == 9) ? "TYPE_CELL_DRAG" : ((type == 10) ? "TYPE_CANCEL" : ((type != 13) ? ("NONE[" + type + "]") : "TYPE_ON"))))))))))); }'''
 values=['TYPE_DOWN','TYPE_UP','TYPE_ENTER','TYPE_LEAVE','TYPE_MOVE','TYPE_CLICK','TYPE_LOST','TYPE_DRAG','TYPE_GRID_DRAG','TYPE_CELL_DRAG','TYPE_CANCEL']
 new=' '.join(('if' if i==0 else 'else if')+' (type == '+str(i)+') {text="'+value+'";}' for i,value in enumerate(values))
 new+=' else if(type==13){text="TYPE_ON";} else{text="NONE["+type+"]";}'
 return replace(s,old,new)
def image_parent_dispatch(s):
 # Reviewed source contains one Int32 num declaration at the parent, one call
 # GetFileFormat(data) as switch expression and exactly gif/png/jpg sections.
 if any(t[0]=='fileFormat' for t in tokens(s)):raise ValueError('new-selector-name-collision')
 s=replace(s,'int num; array[0] = (array[1] = 0);','array[0] = (array[1] = 0); string fileFormat = GetFileFormat(data);')
 ts=tokens(s);w=[t[0] for t in tokens('switch(GetFileFormat(data)){')];hits=[i for i in range(len(ts)-len(w)+1) if [t[0] for t in ts[i:i+len(w)]]==w]
 if len(hits)!=1:raise ValueError('format-switch-not-unique')
 i=hits[0];j=i+len(w);level=1;labels=[]
 while level:
  v=ts[j][0]
  if level==1 and v=='case':
   if ts[j+1][0] not in ['"gif"','"png"','"jpg"'] or ts[j+2][0]!=':':raise ValueError('unapproved-format')
   labels.append((ts[j+1][0],j,j+3))
  if level==1 and v=='default':raise ValueError('unapproved-default')
  if v=='{':level+=1
  elif v=='}':level-=1
  j+=1
 if [v for v,_,_ in labels]!=['"gif"','"png"','"jpg"']:raise ValueError('format-order')
 arms=[]
 for n,(value,_,a) in enumerate(labels):
  end=labels[n+1][1] if n+1<len(labels) else j-1;body=s[ts[a][1]:ts[end-1][2]];bt=tokens(body);terminal=len(bt)-3 if bt[0][0]=='{' and bt[-1][0]=='}' else len(bt)-2
  if [x[0] for x in bt[terminal:terminal+2]]!=['break',';']:raise ValueError('terminal-switch-break-missing')
  body=body[:bt[terminal][1]]+body[bt[terminal+1][2]:]
  if n==2:body=replace(body,'num = 2;','int num = 2;')
  arms.append(('if' if n==0 else 'else if')+' (fileFormat == '+value+') {'+body+'}')
 return s[:ts[i][1]]+' '.join(arms)+s[ts[j-1][2]:]
def image_marker_tail(s):
 # Move only these two effectful assignments from after the infinite loop
 # into its terminal positive guard. The remaining increment stays after if.
 payload='array[1] = (data[num + 3] << 8) | data[num + 4]; array[0] = (data[num + 5] << 8) | data[num + 6];'
 s=replace(s,payload,'')
 return replace(s,'if (num2 == 65472) { break; }','if (num2 == 65472) {'+payload+' break;}')
def transform_image_area(s):
 return image_marker_tail(image_parent_dispatch(s))
