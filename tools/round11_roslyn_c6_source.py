"""Single-use non-const true guard. Pure source transformation only."""
import re
class Refused(ValueError):pass
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
        elif c in('"',"'"):quote=c
        elif c=='{':depth+=1
        elif c=='}':
            depth-=1
            if depth==0:return i
        i+=1
    raise Refused('unclosed-block')

def transform_seb_nonconstant_true(source):
    name='recoveredDrawRegion'
    if re.search(r'\b'+name+r'\b',source):raise Refused('guard-name-not-fresh')
    guard=unique(r'if\s*\(true\)\s*\{',source)
    body_open=source.index('{',guard.start());body_end=closing(source,body_open)
    body=source[body_open:body_end+1]
    # The IL-authorized site only; no condition or leaf is rewritten.
    unique(r'int\s+num4\s*=\s*sp\[8\]\s*\*\s*1\s*\+\s*sp\[9\]\s*\*\s*2\s*;',body)
    unique(r'g\.DrawImage\(img\[sp\[1\]\],\s*x,\s*y,\s*sx,\s*sy,\s*width,\s*height\)\s*;',body)
    if re.search(r'\b(?:goto|yield|await|delegate)\b|=>',body):raise Refused('unproved-transfer-or-capture')
    nl='\r\n'if'\r\n'in source else'\n'
    return source[:guard.start()]+'bool '+name+' = true;'+nl+'if ('+name+') '+source[body_open:]

def main():
    from pathlib import Path
    import json,hashlib
    w=Path(__file__).resolve().parent;r=next(p for p in w.parents if(p/'tools/compiler_probes.py').exists());b=r/'build/round10/final-corpus/KairoLibrary';logical='src/KairoLibrary/kairo/unity/ui/Seb.cs';raw=(b/logical).read_bytes();source=raw.decode('utf-8-sig');match=unique(r'public\s+void\s+Draw\(Graphics g, float x, float y, Image\[\] img, int\[\] sp\)',source);end=closing(source,source.index('{',match.end()))+1;before=source[match.start():end];after=transform_seb_nonconstant_true(before);sha=lambda raw:hashlib.sha256(raw).hexdigest()
    p={'method_id':'kairo.unity.ui.Seb::Draw(kairo.unity.ui.Graphics,System.Single,System.Single,kairo.unity.ui.Image[],System.Int32[])->System.Void','logical_path':logical,'rule':'r11-c6-nonconstant-true-stack-guard','start':len(source[:match.start()].encode('utf-16-le'))//2,'end':len(source[:end].encode('utf-16-le'))//2,'before':before,'after':after,'source_file_sha256':sha(raw),'source_text_sha256':sha(source.encode()),'calculator_sha256':sha(Path(__file__).read_bytes()),'source_snapshot':'build/round10/final-corpus/KairoLibrary','own_compilation_receipt_sha256':sha((b/'verification.json').read_bytes()),'own_dump_sha256':sha((b/'rebuilt-methods.json').read_bytes()),'original_dump_sha256':sha((b/'original-methods.json').read_bytes()),'source_only':True,'own_body_source_bound':True,'readiness':'NOT_READY','game_outcome':'UNKNOWN','game_compilations':0,'rebase_policy':'fresh exact unique UTF16 before declaration; no current Main binding'}
    (w/'c6-proposals.json').write_text(json.dumps({'family':'r11-c6-nonconstant-true-stack-guard','patches':[p],'game_compilations':0,'adopted':False},indent=2)+'\n',encoding='utf-8');(w/'c6-before.cs').write_bytes(before.encode());(w/'c6-after.cs').write_bytes(after.encode());print(p['method_id'])
if __name__=='__main__':main()
