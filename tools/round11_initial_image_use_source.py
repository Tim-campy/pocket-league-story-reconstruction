"""Return at the existing null guard; keep the adopted local declarations."""
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

def transform_image_null_guard_return(source):
    # This is a prerequisite already adopted in R10D34, not another hoist.
    unique(r'byte\[\]\s+array\s*=\s*src_\s*;\s*int\s+num2\s*;\s*int\s+num3\s*;',source)
    guard=unique(r'if\s*\(array\s*==\s*null\)\s*\{',source)
    start=source.index('{',guard.start());end=closing(source,start);body=source[start+1:end]
    print_guard=unique(r'^\s*if\s*\(Config\.PRINT\)\s*\{',body);inner_open=body.index('{',print_guard.start());inner_end=closing(body,inner_open)
    if body[inner_end+1:].strip():raise Refused('null-guard-has-additional-statements')
    printed=body[inner_open+1:inner_end]
    if not re.match(r'\s*Log\.Info\("Image\.use\(\)',printed):raise Refused('null-log-site-differs')
    if re.search(r'\b(?:return|goto|break|continue|yield|await)\b',printed):raise Refused('print-arm-has-transfer')
    alternative=re.match(r'\s*else\s*\{',source[end+1:])
    if alternative is None:raise Refused('decode-else-missing')
    payload_start=end+1+alternative.end()-1;payload_end=closing(source,payload_start)
    payload=source[payload_start:payload_end+1]
    unique(r'image_\s*=\s*DecodeByteArray\(array\)\s*;',payload)
    nl='\r\n'if'\r\n'in source else'\n'
    # Keep the entire old else block, including EH/goto and child declarations.
    return source[:end]+nl+'return;'+nl+source[end:end+1]+nl+payload+source[payload_end+1:]

def main():
    from pathlib import Path
    import hashlib,json
    w=Path(__file__).resolve().parent;r=next(p for p in w.parents if(p/'tools/compiler_probes.py').exists());b=r/'build/round10/final-corpus/KairoLibrary';logical='src/KairoLibrary/kairo/unity/ui/Image.cs';raw=(b/logical).read_bytes();source=raw.decode('utf-8-sig');match=unique(r'public\s+void\s+Use\(\)',source);end=closing(source,source.index('{',match.end()))+1;before=source[match.start():end];after=transform_image_null_guard_return(before);sha=lambda raw:hashlib.sha256(raw).hexdigest()
    p={'method_id':'kairo.unity.ui.Image::<Use>b__96_0()->System.Void','logical_path':logical,'rule':'r11-d-initial-image-use-local-null-return','start':len(source[:match.start()].encode('utf-16-le'))//2,'end':len(source[:end].encode('utf-16-le'))//2,'before':before,'after':after,'source_file_sha256':sha(raw),'source_text_sha256':sha(source.encode()),'calculator_sha256':sha(Path(__file__).read_bytes()),'source_snapshot':'build/round10/final-corpus/KairoLibrary','own_compilation_receipt_sha256':sha((b/'verification.json').read_bytes()),'own_dump_sha256':sha((b/'rebuilt-methods.json').read_bytes()),'original_dump_sha256':sha((b/'original-methods.json').read_bytes()),'source_only':True,'own_body_source_bound':True,'generated_source_policy':'parent Use only; compiler owns generated lambda','readiness':'NOT_READY','game_outcome':'UNKNOWN','game_compilations':0,'rebase_policy':'fresh exact unique UTF16 parent before, preserving existing R10D34 hoist; no current Main binding'}
    (w/'proposals.json').write_text(json.dumps({'family':'r11-d-initial-image-use-local-null-return','patches':[p],'game_compilations':0,'adopted':False},indent=2)+'\n',encoding='utf-8');(w/'before.cs').write_bytes(before.encode());(w/'after.cs').write_bytes(after.encode());print(p['method_id'])
if __name__=='__main__':main()
