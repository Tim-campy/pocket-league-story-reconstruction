"""Pure source-only bounded loop guards. No I/O except in main()."""
import re

class Refused(ValueError):pass

def closing(text,start):
 if text[start]!='{':raise Refused('not-opening-brace')
 depth=0;i=start;quote=None;escape=False
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

def unique(pattern,text):
 found=list(re.finditer(pattern,text))
 if len(found)!=1:raise Refused('anchor-not-unique:'+pattern)
 return found[0]

def move_guard(source,old_header,new_header,guard):
 # Exactly the observed bounded syntax, never splitting arbitrary && ASTs.
 pattern=re.escape(old_header).replace(r'\ ',r'\s*')+r'\s*{'
 match=unique(pattern,source);opening=match.end()-1;end=closing(source,opening)
 body=source[opening+1:end]
 if re.search(r'\b(?:goto|yield|await)\b|=>|\bdelegate\b|\b[A-Za-z_]\w*\s*:',body):raise Refused('loop-body-has-unproved-entry-or-capture')
 nl='\r\n' if '\r\n' in source else '\n'
 return source[:match.start()]+new_header+' {'+nl+guard+nl+body+source[end:]

def transform_skip_body_guard(method_source):
 return move_guard(method_source,'while (num2 < n && Read() != -1)','while (num2 < n)','if (Read() == -1) { break; }')

def transform_matrix_body_guard(method_source):
 return move_guard(method_source,'while (num >= 0 && array[num] == null)','while (num >= 0)','if ((object)array[num] != null) { break; }')

def transform_split_guard_composition(method_source):
 source=move_guard(method_source,r"while (num3 >= num && text.CharAt(num3) == '\\')",'while (num3 >= num)',r"if (text.CharAt(num3) != '\\') { break; }")
 source=move_guard(source,"for (i = 0; i < text2.Length && text2[i] <= ' '; i++)",'for (i = 0; i < text2.Length; i++)',"if (text2[i] > ' ') { break; }")
 source=move_guard(source,"while (num4 >= 0 && text2[num4] <= ' ')",'while (num4 >= 0)',"if (text2[num4] > ' ') { break; }")
 old='text2 = ((num4 - i >= 0) ? text2.Substring(i, num4 - i + 1) : "");'
 match=unique(re.escape(old).replace(r'\ ',r'\s*'),source)
 replacement='if (num4 - i < 0) { text2 = ""; } else { text2 = text2.Substring(i, num4 - i + 1); }'
 return source[:match.start()]+replacement+source[match.end():]

def main():
 from pathlib import Path
 import hashlib,json
 root=next(p for p in Path(__file__).resolve().parents if (p/'tools/round7_slot_model.py').exists());w=Path(__file__).resolve().parent;b=root/'build/round10/cycles/cycle03/after/KairoLibrary'
 entries=[('skip-bound-first-body-break','kairo.unity.io.BitArrayInputStream::Skip(System.Int64)->System.Int64','src/KairoLibrary/kairo/unity/io/BitArrayInputStream.cs',r'public\s+long\s+Skip\([^)]*\)',transform_skip_body_guard),('matrix-bound-first-body-break','kairo.unity.ui.Graphics::PushMatrix(kairo.unity.ui.Matrix)->System.Void','src/KairoLibrary/kairo/unity/ui/Graphics.cs',r'public\s+void\s+PushMatrix\([^)]*\)',transform_matrix_body_guard),('split-bound-first-guards-per-arm-store','kairo.unity.util.StringUtil::Split(System.String,System.String,System.Boolean,System.Boolean,System.Boolean)->System.String[]','src/KairoLibrary/kairo/unity/util/StringUtil.cs',r'public\s+static\s+string\[\]\s+Split\(string text, string sep, bool trim, bool excludeZeroLength, bool escape\)',transform_split_guard_composition)]
 sha=lambda raw:hashlib.sha256(raw).hexdigest();patches=[]
 for rule,key,logical,pattern,function in entries:
  raw=(b/logical).read_bytes();source=raw.decode('utf-8-sig');m=unique(pattern,source);end=closing(source,source.index('{',m.end()))+1;before=source[m.start():end];after=function(before)
  patch={'method_id':key,'logical_path':logical,'rule':'d10-roslyn-'+rule,'start':len(source[:m.start()].encode('utf-16-le'))//2,'end':len(source[:end].encode('utf-16-le'))//2,'before':before,'after':after,'before_sha256':sha(before.encode()),'after_sha256':sha(after.encode()),'source_file_sha256':sha(raw),'source_text_sha256':sha(source.encode()),'source_snapshot':'build/round10/cycles/cycle03/after/KairoLibrary','calculator_sha256':sha(Path(__file__).read_bytes()),'readiness':'NOT_READY','source_only':True,'game_compilations':0,'own_compilation_receipt_sha256':sha((b/'verification.json').read_bytes()),'own_dump_sha256':sha((b/'rebuilt-methods.json').read_bytes()),'original_dump_sha256':sha((b/'original-methods.json').read_bytes()),'rebase_policy':'fresh unique exact UTF16 before only; no current Main binding'}
  patches.append(patch);(w/(rule+'.before.cs')).write_bytes(before.encode());(w/(rule+'.after.cs')).write_bytes(after.encode())
 (w/'proposals.json').write_text(json.dumps({'operation':'bounded-Roslyn-loop-body-guards-with-existing-leaves','family':'d10-roslyn-bound-first-body-break','source_only':True,'game_compilations':0,'adopted':False,'patches':patches},indent=2)+'\n',encoding='utf-8');print([p['method_id'] for p in patches])

if __name__=='__main__':main()
