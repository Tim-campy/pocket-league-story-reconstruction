"""Pure two-residue composition on the authenticated D5 Canvas candidate."""
import re
class Refused(ValueError):pass
def unique(pattern,text):
 matches=list(re.finditer(pattern,text))
 if len(matches)!=1:raise Refused('anchor-not-unique:'+pattern)
 return matches[0]
def closing(text,start):
 depth=0;quote=None;escape=False;i=start
 if text[i]!='{':raise Refused('not-block')
 while i<len(text):
  c=text[i]
  if quote:
   if escape:escape=False
   elif c=='\\':escape=True
   elif c==quote:quote=None
  elif text.startswith('//',i):
   j=text.find('\n',i+2);i=len(text) if j<0 else j;continue
  elif text.startswith('/*',i):
   j=text.find('*/',i+2)
   if j<0:raise Refused('unclosed-comment')
   i=j+2;continue
  elif c in ('"',"'"):quote=c
  elif c=='{':depth+=1
  elif c=='}':
   depth-=1
   if depth==0:return i
  i+=1
 raise Refused('unclosed-block')
def transform_canvas_pointer_chain(source):
 pattern=r'value\s*=\s*new\s+float\[4\]\s*;\s*onTouchPointer_\[touch\.fingerId\]\s*=\s*value\s*;'
 match=unique(pattern,source)
 # Ensure the captured two statements are the complete Began arm.
 owners=[]
 for arm in re.finditer(r'if\s*\(touch\.phase\s*==\s*TouchPhase\.Began\)\s*{',source):
  end=closing(source,arm.end()-1)
  if arm.end()<=match.start() and match.end()<=end and not source[arm.end():match.start()].strip() and not source[match.end():end].strip():owners.append(arm)
 if len(owners)!=1:raise Refused('Began-arm-owner-not-unique-or-has-other-commands')
 return source[:match.start()]+'onTouchPointer_[touch.fingerId] = (value = new float[4]);'+source[match.end():]
def transform_canvas_list3_positive(source):
 match=unique(r'if\s*\(list3\.Count\s*<=\s*0\)\s*{\s*return\s*;\s*}',source)
 following=match.end()
 while following<len(source) and source[following].isspace():following+=1
 if following==len(source) or source[following]!='{':raise Refused('tail-is-not-an-existing-explicit-child-block')
 end=closing(source,following);body=source[following+1:end]
 if not re.match(r'\s*int\s+num3\s*=\s*0\s*;',body):raise Refused('tail-first-local-differs')
 if re.search(r'\b(?:goto|yield|await)\b|=>|\bdelegate\b',body):raise Refused('unproved-tail-transfer-or-capture')
 # The old child block itself becomes the if body. Its scope is not widened.
 return source[:match.start()]+'if (list3.Count > 0) '+source[following:]
def transform_canvas_two_residues(source):
 return transform_canvas_list3_positive(transform_canvas_pointer_chain(source))
def main():
 from pathlib import Path
 import hashlib,json
 w=Path(__file__).resolve().parent;r=next(p for p in w.parents if (p/'tools/round7_slot_model.py').exists());report_path=r/'reports/round10-cycle05-probe-KairoLibrary-engine.json';report=json.loads(report_path.read_text(encoding='utf-8'));key='kairo.unity.ui.Canvas::OnInputUpdate()->System.Void';method=next(x for x in report['methods'] if x['id']==key);step=next(x for x in method['steps'] if x['accepted']);before=step['after'];after=transform_canvas_two_residues(before);logical='src/KairoLibrary/kairo/unity/ui/Canvas.cs';source_path=Path(json.loads(Path(report['source_map']).read_text())[logical]);raw=source_path.read_bytes();source=raw.decode('utf-8-sig');sha=lambda raw:hashlib.sha256(raw).hexdigest();assert sha(raw)==report['source_hashes'][logical];assert source.count(before)==1;start=source.index(before);end=start+len(before)
 p={'method_id':key,'logical_path':logical,'rule':'d10-canvas-two-residual-composition','start':len(source[:start].encode('utf-16-le'))//2,'end':len(source[:end].encode('utf-16-le'))//2,'before':before,'after':after,'source_file_sha256':sha(raw),'source_text_sha256':sha(source.encode()),'before_sha256':sha(before.encode()),'after_sha256':sha(after.encode()),'source_snapshot':'build/round10/runs/cycle05-probe/engine/KairoLibrary/sources','calculator_sha256':sha(Path(__file__).read_bytes()),'own_compilation_receipt_sha256':sha(report_path.read_bytes()),'own_dump_sha256':sha((r/'build/round10/runs/cycle05-probe/engine/KairoLibrary/final-methods.json').read_bytes()),'original_dump_sha256':sha((Path(report['baseline_dir'])/'original-methods.json').read_bytes()),'source_only':True,'game_compilations':0,'own_body_source_bound':True,'readiness':'NOT_READY','rebase_policy':'fresh exact unique method before; authenticated D5 candidate, no current Main binding'}
 (w/'method.before.cs').write_bytes(before.encode());(w/'method.after.cs').write_bytes(after.encode());(w/'proposals.json').write_text(json.dumps({'operation':'two-residue-source-composition-on-real-D5-partial','family':'d10-canvas-two-residual-composition','game_compilations':0,'adopted':False,'patches':[p]},indent=2)+'\n',encoding='utf-8');print(p['source_file_sha256'],p['before_sha256'],p['after_sha256'])
if __name__=='__main__':main()
