"""Pure late-tail scopes plus one stack-only comparison alias. Main is WIP-only."""
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
def late_tail(source,anchor,names,prior_return):
 start=unique(re.escape(anchor).replace(r'\ ',r'\s*'),source).start();end=closing(source,source.index('{'))
 prefix=source[:start];tail=source[start:end]
 if not re.search(prior_return+ r'\s*}\s*$',prefix):raise Refused('preceding-complete-returning-arm-differs')
 if re.search(r'\b(?:goto|yield|await)\b|=>|\bdelegate\b',tail):raise Refused('unproved-tail-entry-or-capture')
 if any(re.search(r'\b'+re.escape(name)+r'\b',prefix) for name in names):raise Refused('late-local-has-previous-use-or-shadow')
 nl='\r\n' if '\r\n' in source else '\n'
 return source[:start]+'{'+nl+tail+nl+'}'+nl+source[end:]
def transform_image_tail_scope(source):
 return late_tail(source,'float num = image.GetWidth();',('num','num2'),r'return\s*;')
def transform_translate_tail_scope(source):
 return late_tail(source,'string result = text;',('result','text5'),r'return\s+text\s*;')
def transform_translate_stack_alias(source):
 name='comparisonSource'
 if re.search(r'\b'+name+r'\b',source):raise Refused('comparison-alias-name-not-fresh')
 match=unique(r'string\s+text3\s*=\s*_translateText\(text2\)\s*;\s*if\s*\(text2\s*!=\s*text3\)',source)
 nl='\r\n' if '\r\n' in source else '\n'
 after='string comparisonSource = text2;'+nl+'string text3 = _translateText(comparisonSource);'+nl+'if (comparisonSource != text3)'
 return source[:match.start()]+after+source[match.end():]
def transform_translate_composition(source):
 return transform_translate_stack_alias(transform_translate_tail_scope(source))
def main():
 from pathlib import Path
 import json,hashlib
 w=Path(__file__).resolve().parent;r=next(p for p in w.parents if (p/'tools/round7_slot_model.py').exists());b=r/'build/round10/cycles/cycle07/after/KairoLibrary';sha=lambda raw:hashlib.sha256(raw).hexdigest();patches=[]
 entries=[('image-late-tail-scope','kairo.unity.ui.Graphics::DrawImage(kairo.unity.ui.Image,System.Single,System.Single)->System.Void','src/KairoLibrary/kairo/unity/ui/Graphics.cs',r'public\s+void\s+DrawImage\(Image image, float x, float y\)',transform_image_tail_scope),('translate-late-tail-stack-alias','kairo.unity.util.Language::TranslateText(System.String,System.Boolean)->System.String','src/KairoLibrary/kairo/unity/util/Language.cs',r'public\s+static\s+string\s+TranslateText\(string text, bool tabCheck = true\)',transform_translate_composition)]
 for rule,key,logical,pattern,transform in entries:
  raw=(b/logical).read_bytes();source=raw.decode('utf-8-sig');match=unique(pattern,source);end=closing(source,source.index('{',match.end()))+1;before=source[match.start():end];after=transform(before)
  p={'method_id':key,'logical_path':logical,'rule':'d10-late-tail-scope-comparison-'+rule,'start':len(source[:match.start()].encode('utf-16-le'))//2,'end':len(source[:end].encode('utf-16-le'))//2,'before':before,'after':after,'source_file_sha256':sha(raw),'source_text_sha256':sha(source.encode()),'source_snapshot':'build/round10/cycles/cycle07/after/KairoLibrary','calculator_sha256':sha(Path(__file__).read_bytes()),'own_compilation_receipt_sha256':sha((b/'verification.json').read_bytes()),'own_dump_sha256':sha((b/'rebuilt-methods.json').read_bytes()),'original_dump_sha256':sha((b/'original-methods.json').read_bytes()),'own_body_source_bound':True,'source_only':True,'game_compilations':0,'readiness':'NOT_READY','game_body_outcome':'UNKNOWN','rebase_policy':'fresh exact unique UTF16 method before; no current Main binding'}
  patches.append(p);(w/(rule+'.before.cs')).write_bytes(before.encode());(w/(rule+'.after.cs')).write_bytes(after.encode())
 (w/'proposals.json').write_text(json.dumps({'operation':'two-late-tail-scope-source-hypotheses','family':'d10-late-tail-scope-comparison','patches':patches,'game_compilations':0,'adopted':False},indent=2)+'\n',encoding='utf-8');print([p['method_id'] for p in patches])
if __name__=='__main__':main()
