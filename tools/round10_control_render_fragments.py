"""Source-only additions for the for initializer already represented by AST spans."""
from round10_control_trace import source_fragments as base_fragments,source_slice

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
 # Synthetic switch and exception-header leaves are compiler obligations, not
 # executable source statements. Keep their source spelling and ownership.
 switch_spans=[]
 for origin in leaf.get('_origins',[]):
  n=origin['node']
  if n.get('data',{}).get('role')=='switch-selector-temp-store':
   switch_spans.append(n['span'])
 switch_spans=list({(s['start'],s['end']):s for s in switch_spans}.values())
 out=[]
 for origin in leaf.get('_origins',[]):
  n=origin['node'];span=n['span'];role=n.get('data',{}).get('role')
  owner=next((s for s in switch_spans if s['start']<=span['start']<=span['end']<=s['end']),None)
  if owner is not None:
   raw=source_slice(source,owner['start'],owner['end'])
   # A selector's actual expression text comes from its independent AST, never
   # from disassembly or a reconstructed numeric/string literal.
   switches=[c for a in origin['ancestors'] for c in a.get('children',[]) if c['kind']=='switch' and c['span']==owner]
   if len({c['id'] for c in switches})!=1:raise ValueError('switch-selector-source-owner-not-unique')
   sw=switches[0]
   rows=[{'kind':'switch_selector','start':owner['start'],'end':owner['end'],'text':sw['data']['selector'],'switch_node':sw}]
  elif role=='protected-return-reload':
   rows=[{'kind':'protected_return_reload','start':span['start'],'end':span['end'],'text':'','lowering_only':True}]
  elif role in ('catch-exception-local-store','catch-exception-pop'):
   rows=[{'kind':'catch_header','start':span['start'],'end':span['end'],'text':source_slice(source,span['start'],span['end']),'catch_role':role}]
  else:
   rows=None
  if rows is not None:
   for row in rows:
    if not any((r['kind'],r['start'],r['end'],r['text'])==(row['kind'],row['start'],row['end'],row['text']) for r in out):out.append(row)
   continue
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
