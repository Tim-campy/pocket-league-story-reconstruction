"""Source-only explicit locals for tree construction, protected result and loops."""
import re
class Refused(ValueError):pass
def unique(pattern,text):
    result=list(re.finditer(pattern,text))
    if len(result)!=1:raise Refused('anchor-count-%s:%s'%(len(result),pattern))
    return result[0]
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

def transform_compress_explicit_nodes_result(source):
    nl='\r\n'if'\r\n'in source else'\n'
    if re.search(r'\brecoveredCombinedNode\b',source):raise Refused('combined-node-name-not-fresh')
    declaration=unique(r'(?:global::kairo\.unity\.util\.Compressor\.)?Node\s+node\s*;',source)
    if len(re.findall(r'\bnode\b',source))!=4:raise Refused('unexpected-node-use-or-shadow')
    source=source[:declaration.start()]+source[declaration.end():]
    creation=unique(r'\bnode\s*=\s*new\s+Node\(null,\s*null,\s*null,\s*weights_\[l\],\s*l\)\s*;',source)
    source=source[:creation.start()]+'Node '+source[creation.start():]
    single=unique(r'Node\s+node3\s*=\s*\(node2\.parent_\s*=\s*(?P<ctor>new\s+Node\(null,\s*node2,\s*node2,\s*node2\.weight_\s*\+\s*node2\.weight_,\s*0\))\)\s*;',source)
    source=source[:single.start()]+'Node node3 = '+single.group('ctor')+';'+nl+'node2.parent_ = node3;'+source[single.end():]
    combined=unique(r'fastVector\.AddElement\(node5\.parent_\s*=\s*\(node4\.parent_\s*=\s*(?P<ctor>new\s+Node\(null,\s*node4,\s*node5,\s*node4\.weight_\s*\+\s*node5\.weight_,\s*0\))\)\)\s*;',source)
    replacement='Node recoveredCombinedNode = '+combined.group('ctor')+';'+nl+'node4.parent_ = recoveredCombinedNode;'+nl+'node5.parent_ = recoveredCombinedNode;'+nl+'fastVector.AddElement(recoveredCombinedNode);'
    source=source[:combined.start()]+replacement+source[combined.end():]
    unique(r'byte\[\]\s+array2\s*=\s*null\s*;',source)
    result=unique(r'return\s+byteArrayOutputStream\.ToByteArray\(\)\s*;',source)
    source=source[:result.start()]+'array2 = byteArrayOutputStream.ToByteArray();'+source[result.end():]
    final=unique(r'finally\s*\{\s*byteArrayOutputStream\?\.Close\(\)\s*;\s*byteArrayOutputStream\s*=\s*null\s*;\s*\}',source)
    if not re.fullmatch(r'\s*\}',source[final.end():]):raise Refused('finally-not-last-statement-in-method')
    return source[:final.end()]+nl+'return array2;'+source[final.end():]

def transform_readbit_reassign_existing(source):
    unique(r'int\s+num\s*=\s*0\s*;',source)
    unique(r'num\s*=\s*_readBit\(\)\s*;',source)
    test=unique(r'if\s*\(_readBit\(\)\s*==\s*0\)',source)
    nl='\r\n'if'\r\n'in source else'\n'
    return source[:test.start()]+'num = _readBit();'+nl+'if (num == 0)'+source[test.end():]

def transform_preconcat_outer_indices(source):
    first=unique(r'for\s*\(int\s+i\s*=\s*0\s*;\s*i\s*<\s*mul_\.Length\s*;\s*i\+\+\)',source)
    if re.search(r'\b(?:j|k|l)\b',source[:first.start()]):raise Refused('indices-used-before-first-loop')
    if re.search(r'\b(?:goto|yield|await|delegate)\b|=>',source):raise Refused('unproved-transfer-or-capture')
    for name in('j','k','l'):
        match=unique(r'for\s*\(int\s+'+name+r'\s*=\s*0\s*;\s*'+name+r'\s*<\s*3\s*;\s*'+name+r'\+\+\)',source)
        type_span=re.search(r'\bint\s+',source[match.start():match.end()]);start=match.start()+type_span.start();end=match.start()+type_span.end();source=source[:start]+source[end:]
    first=unique(r'for\s*\(int\s+i\s*=\s*0\s*;\s*i\s*<\s*mul_\.Length\s*;\s*i\+\+\)',source)
    nl='\r\n'if'\r\n'in source else'\n'
    return source[:first.start()]+'int j, k, l;'+nl+source[first.start():]

def main():
    from pathlib import Path
    import json,hashlib
    w=Path(__file__).resolve().parent;r=next(p for p in w.parents if(p/'tools/round7_slot_model.py').exists());b=r/'build/round10/cycles/cycle15/after/KairoLibrary';sha=lambda raw:hashlib.sha256(raw).hexdigest();patches=[]
    entries=[('compress-explicit-nodes-result','kairo.unity.util.Compressor::Compress(System.Byte[])->System.Byte[]','src/KairoLibrary/kairo/unity/util/Compressor.cs',r'public\s+static\s+byte\[\]\s+Compress\(byte\[\] data\)',transform_compress_explicit_nodes_result),('readbit-existing-num','kairo.unity.util.Compressor::_readNodeFromBitBuffer()->kairo.unity.util.Compressor/Node','src/KairoLibrary/kairo/unity/util/Compressor.cs',r'private\s+static\s+Node\s+_readNodeFromBitBuffer\(\)',transform_readbit_reassign_existing),('preconcat-parent-indices','kairo.unity.ui.Matrix::PreConcat(System.Single[])->kairo.unity.ui.Matrix','src/KairoLibrary/kairo/unity/ui/Matrix.cs',r'public\s+Matrix\s+PreConcat\(float\[\] transform\)',transform_preconcat_outer_indices)]
    for rule,key,logical,pattern,fn in entries:
        raw=(b/logical).read_bytes();source=raw.decode('utf-8-sig');match=unique(pattern,source);end=closing(source,source.index('{',match.end()))+1;before=source[match.start():end];after=fn(before)
        patches.append({'method_id':key,'logical_path':logical,'rule':'d10-explicit-tree-locals-and-index-owners-'+rule,'start':len(source[:match.start()].encode('utf-16-le'))//2,'end':len(source[:end].encode('utf-16-le'))//2,'before':before,'after':after,'source_file_sha256':sha(raw),'source_text_sha256':sha(source.encode()),'source_snapshot':'build/round10/cycles/cycle15/after/KairoLibrary','calculator_sha256':sha(Path(__file__).read_bytes()),'own_compilation_receipt_sha256':sha((b/'verification.json').read_bytes()),'own_dump_sha256':sha((b/'rebuilt-methods.json').read_bytes()),'original_dump_sha256':sha((b/'original-methods.json').read_bytes()),'own_body_source_bound':True,'source_only':True,'readiness':'NOT_READY','game_compilations':0,'game_body_outcome':'UNKNOWN','rebase_policy':'fresh exact unique UTF16 before; two Compressor proposals share a file and require serial rebase after adoption; no Main binding'})
        (w/(rule+'.before.cs')).write_bytes(before.encode());(w/(rule+'.after.cs')).write_bytes(after.encode())
    (w/'proposals.json').write_text(json.dumps({'operation':'explicit-tree-locals-and-loop-ownership-source-proposals','family':'d10-explicit-tree-locals-and-index-owners','patches':patches,'game_compilations':0,'adopted':False},indent=2)+'\n',encoding='utf-8');print([p['method_id']for p in patches])
if __name__=='__main__':main()
