"""Recover a bool value shared between lock/unlock branches. Pure source only."""
import re
class Refused(ValueError): pass

def unique(pattern,text):
    found=list(re.finditer(pattern,text))
    if len(found)!=1:raise Refused('anchor-count-%s:%s'%(len(found),pattern))
    return found[0]

def closing(text,start):
    if text[start]!='{':raise Refused('not-block')
    depth=0;quote=None;escape=False;i=start
    while i<len(text):
        c=text[i]
        if quote:
            if escape:escape=False
            elif c=='\\':escape=True
            elif c==quote:quote=None
        elif text.startswith('//',i):
            j=text.find('\n',i+2);i=len(text)if j<0 else j;continue
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

def transform_localize_boolean_cache(source):
    match=unique(r'int\s+(?P<local>num\d*)\s*;\s*if\s*\((?P<condition>lf\s*==\s*null\s*\|\|\s*lf\.GetFont\(\)\s*==\s*-1)\)\s*\{',source)
    name=match.group('local');outer_open=source.rfind('{',match.start(),match.end());outer_end=closing(source,outer_open)
    outer=source[outer_open+1:outer_end]
    prefix=unique(r'^\s*'+name+r'\s*=\s*\(\s*(?P<call>font_?\.IsNeedLocalizeScale\(\))\s*\?\s*1\s*:\s*0\s*\)\s*;\s*if\s*\('+name+r'\s*!=\s*0\)\s*\{',outer)
    body_open=outer.rfind('{',prefix.start(),prefix.end());body_end=closing(outer,body_open)
    body=outer[body_open:body_end+1]
    if outer[body_end+1:].strip():raise Refused('extra-statement-in-enabled-arm')
    if re.search(r'\b'+re.escape(name)+r'\b',body):raise Refused('cache-used-inside-enabled-body')
    suffix=source[outer_end+1:]
    otherwise=re.match(r'\s*else\s*\{\s*'+name+r'\s*=\s*0\s*;\s*\}',suffix)
    if otherwise is None:raise Refused('disabled-arm-differs')
    later=suffix[otherwise.end():]
    later_test=unique(r'if\s*\('+name+r'\s*!=\s*0\)',later)
    if len(re.findall(r'\b'+re.escape(name)+r'\b',later))!=1:raise Refused('other-later-cache-use')
    if re.search(r'\b'+re.escape(name)+r'\b',source[:match.start()]):raise Refused('cache-used-before-declaration')
    later=later[:later_test.start()]+'if ('+name+')'+later[later_test.end():]
    nl='\r\n'if'\r\n'in source else'\n'
    replacement='bool '+name+' = ('+match.group('condition')+') && '+prefix.group('call')+';'+nl+'if ('+name+') '+body
    return source[:match.start()]+replacement+later

def main():
    import hashlib,json
    from pathlib import Path
    w=Path(__file__).resolve().parent;r=next(p for p in w.parents if(p/'tools/round7_slot_model.py').exists());b=r/'build/round10/cycles/cycle12/after/KairoLibrary';sha=lambda raw:hashlib.sha256(raw).hexdigest();patches=[]
    entries=[('graphics-localize-stack-bool','kairo.unity.ui.Graphics::DrawText(kairo.unity.ui.TextFormat,System.String,System.Single,System.Single,System.Single,System.Int32)->System.Void','src/KairoLibrary/kairo/unity/ui/Graphics.cs',r'public\s+void\s+DrawText\(TextFormat lf, string str, float x, float y, float width, int anchor = 0\)'),
      ('layout-localize-stack-bool','kairo.unity.ui.TextLayout::Draw(kairo.unity.ui.TextFormat,kairo.unity.ui.Graphics,System.String,System.Single,System.Single,System.Single,System.Single,System.Int32,System.Int32,System.Int32)->System.Void','src/KairoLibrary/kairo/unity/ui/TextLayout.cs',r'public\s+static\s+void\s+Draw\(TextFormat lf, Graphics g, string text, float x, float y, float w, float h, int color, int lineSpace, int anchor = 0\)')]
    for rule,key,logical,pattern in entries:
        raw=(b/logical).read_bytes();source=raw.decode('utf-8-sig');match=unique(pattern,source);end=closing(source,source.index('{',match.end()))+1;before=source[match.start():end];after=transform_localize_boolean_cache(before)
        patch={'method_id':key,'logical_path':logical,'rule':'d10-localize-shared-boolean-cache-'+rule,'start':len(source[:match.start()].encode('utf-16-le'))//2,'end':len(source[:end].encode('utf-16-le'))//2,'before':before,'after':after,'source_file_sha256':sha(raw),'source_text_sha256':sha(source.encode()),'source_snapshot':'build/round10/cycles/cycle12/after/KairoLibrary','calculator_sha256':sha(Path(__file__).read_bytes()),'own_compilation_receipt_sha256':sha((b/'verification.json').read_bytes()),'own_dump_sha256':sha((b/'rebuilt-methods.json').read_bytes()),'original_dump_sha256':sha((b/'original-methods.json').read_bytes()),'own_body_source_bound':True,'source_only':True,'readiness':'NOT_READY','game_compilations':0,'game_body_outcome':'UNKNOWN','rebase_policy':'fresh exact unique UTF16 before parent declaration; no current Main binding'}
        patches.append(patch);(w/(rule+'.before.cs')).write_bytes(before.encode());(w/(rule+'.after.cs')).write_bytes(after.encode())
    (w/'proposals.json').write_text(json.dumps({'operation':'shared-logical-bool-stack-cache-source-proposals','family':'d10-localize-shared-boolean-cache','patches':patches,'game_compilations':0,'adopted':False},indent=2)+'\n',encoding='utf-8')
    print([(p['method_id'],p['start'],p['end'])for p in patches])
if __name__=='__main__':main()
