"""Source-only additions for the for initializer already represented by AST spans."""
from round9_control_trace import source_fragments as base_fragments,source_slice

def _header(text):
 at=text.find('(')
 if at<0:return None
 start=at+1;depth=0;quote=None;escape=False;i=start;parts=[];part=start
 while i<len(text):
  c=text[i]
  if quote:
   if escape:escape=False
   elif c=='\\':escape=True
   elif c==quote:quote=None
  elif c in ('"',"'"):quote=c
  elif text.startswith('//',i):
   end=text.find('\n',i+2)
   if end<0:return None
   i=end;continue
  elif text.startswith('/*',i):
   end=text.find('*/',i+2)
   if end<0:return None
   i=end+2;continue
  elif c in '([{':depth+=1
  elif c in ')]}':
   if c==')'and depth==0:parts.append(text[part:i]);return parts if len(parts)==3 else None
   depth-=1
  elif c==';'and depth==0:parts.append(text[part:i]);part=i+1
  i+=1
 return None

def source_fragments(leaf,source):
 out=[]
 for origin in leaf.get('_origins',[]):
  try:rows=base_fragments({'_origins':[origin]},source)
  except ValueError as original:
   rows=[]
   for ancestor in reversed(origin['ancestors']):
    if ancestor['kind']!='sequence':continue
    span=ancestor['span'];text=source_slice(source,span['start'],span['end']).strip()
    if not text.startswith('for'):continue
    parts=_header(text)
    if not parts or not parts[0].strip():continue
    # Only explicit single/multiple declarations are lifted, not arbitrary comma
    # expressions. The subsequent semantic model rejects widened-name conflicts.
    import re
    if not re.match(r'^(?:global::)?[A-Za-z_]\w*(?:[.<>\[\],?:]\w*)*\s+[A-Za-z_]\w*\s*(?:=|,)',parts[0].strip()):continue
    node_span=origin['node']['span'];header_offset=span['start']+len(text[:text.find('(')+1].encode('utf-16-le'))//2;header_end=header_offset+len(parts[0].encode('utf-16-le'))//2
    if not(header_offset<=node_span['start']<=node_span['end']<=header_end):continue
    rows=[{'kind':'statement','start':header_offset,'end':header_end,'text':parts[0].strip()+';','for_initializer_as_statement':True}];break
   if not rows:raise original
  for row in rows:
   key=(row['kind'],row['start'],row['end'],row['text'])
   if not any((r['kind'],r['start'],r['end'],r['text'])==key for r in out):out.append(row)
 # Origins are in emitted tree order. A for iterator is textually in its
 # header but runs after the body; source offsets must never reorder payloads.
 return out
