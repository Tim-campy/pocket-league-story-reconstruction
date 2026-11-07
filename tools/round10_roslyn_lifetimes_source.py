"""Pure bounded source proposals; all historical compiler results UNKNOWN.

Import and transform_* calls do no I/O. Only main reads the frozen C1 copy.
No AST, slot model, C# compiler, verifier, assembly writer or game is invoked.
"""
import re


class Refused(ValueError):pass


def closing(text,start):
    if text[start]!='{':raise Refused('not-an-opening-brace')
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


def unique(pattern,text,flags=0):
    matches=list(re.finditer(pattern,text,flags))
    if len(matches)!=1:raise Refused('anchor-not-unique:'+pattern)
    return matches[0]


def transform_material_tail_scope(method_source):
    """Delay named dictionary/text scope until after the disposed-image branch."""
    if re.search(r'\b(?:goto|yield|await)\b|=>|\bdelegate\b',method_source):raise Refused('unsupported-cross-scope-control-or-capture')
    loop=unique(r'foreach\s*\(\s*Image\s+key\s+in\s+materlas_\.Keys\s*\)\s*\{',method_source)
    begin=loop.end()-1;end=closing(method_source,begin)
    declaration=unique(r'Dictionary<string,\s*Material>\s+dictionary\s*=\s*materlas_\[key\]\s*;',method_source)
    if not begin<declaration.start()<end:raise Refused('tail-is-outside-owning-foreach')
    prefix=method_source[begin+1:declaration.start()]
    if not re.search(r'continue\s*;\s*}\s*$',prefix):raise Refused('preceding-if-does-not-end-in-its-existing-continue')
    tail=method_source[declaration.start():end]
    if not re.search(r'if\s*\(text\s*!=\s*null\)\s*{\s*dictionary\.Remove\(text\);\s*}\s*$',tail):raise Refused('tail-end-is-not-the-existing-dictionary-remove')
    if len(re.findall(r'\bdictionary\b',method_source))!=len(re.findall(r'\bdictionary\b',tail)) or len(re.findall(r'\btext\b',method_source))!=len(re.findall(r'\btext\b',tail)):raise Refused('tail-local-has-an-external-use')
    nl='\r\n' if '\r\n' in method_source else '\n'
    return method_source[:declaration.start()]+'{'+nl+tail+nl+'}'+nl+method_source[end:]


def transform_language_body_local(method_source):
    """Hypothesis: keep the length's def/use inside one lowered loop body."""
    if len(re.findall(r'\blength\b',method_source))!=3:raise Refused('length-has-external-or-ambiguous-uses')
    literal=r'"(?:\\.|[^"\\])*"'
    pattern=(r'int\s+length\s*;\s*do\s*{\s*length\s*=\s*text6\.Length\s*;\s*'
             r'(?P<replace>text6\s*=\s*Regex\.Replace\(\s*text6\s*,\s*'+literal+r'\s*,\s*'+literal+r'\s*\)\s*;)\s*'
             r'}\s*while\s*\(\s*length\s*!=\s*text6\.Length\s*\)\s*;')
    m=unique(pattern,method_source);nl='\r\n' if '\r\n' in method_source else '\n'
    replacement=('while (true) {'+nl+'int length = text6.Length;'+nl+m.group('replace')+nl+
                 'if (length == text6.Length) { break; }'+nl+'}')
    return method_source[:m.start()]+replacement+method_source[m.end():]


def transform_http_license_dispatch(method_source):
    """Hypothesis: zero guard plus explicitly dense default-labelled bucket."""
    name='recoveredLicenseStatus'
    if re.search(r'\b'+name+r'\b',method_source):raise Refused('fresh-selector-name-is-not-fresh')
    sw=unique(r'switch\s*\(\s*KairoPlugin\.LicenseCheck\(\)\s*\)\s*{',method_source)
    begin=sw.end()-1;end=closing(method_source,begin);inside=method_source[begin+1:end]
    case0=unique(r'\bcase\s+0\s*:\s*(?P<body>return\s+new\s+byte\[1\]\[\]\s*{\s*"(?:\\.|[^"\\])*"\.GetBytes\(\)\s*}\s*;)\s*(?=case\s+4\s*:)',inside)
    labels=re.findall(r'\bcase\s+(\d+)\s*:',inside)
    if labels!=['0','4','5','257'] or len(re.findall(r'\bdefault\s*:',inside))!=1:raise Refused('license-case-set-or-order-differs')
    if re.search(r'\bgoto\s+case\b|\bgoto\s+default\b',inside):raise Refused('switch-has-cross-case-transfer')
    nl='\r\n' if '\r\n' in method_source else '\n'
    remaining=inside[:case0.start()]+inside[case0.end():]
    remaining=re.sub(r'\bdefault\s*:', 'case 1:'+nl+'case 2:'+nl+'case 3:'+nl+'default:',remaining,count=1)
    replacement=('int '+name+' = KairoPlugin.LicenseCheck();'+nl+'if ('+name+' == 0) {'+nl+case0.group('body')+nl+'}'+nl+'switch ('+name+') {'+remaining+'}')
    return method_source[:sw.start()]+replacement+method_source[end+1:]


def main():
    from pathlib import Path
    import json,hashlib
    root=next(p for p in Path(__file__).resolve().parents if (p/'config/methods.csv').exists())
    base=root/'build/round10/cycles/c01/after/KairoLibrary';out=Path(__file__).resolve().parent
    entries=[('material-tail-child-scope','kairo.unity.graphics.MaterialManager::RemoveInvalidMaterials()->System.Void','src/KairoLibrary/kairo/unity/graphics/MaterialManager.cs',r'public\s+void\s+RemoveInvalidMaterials\(\)',transform_material_tail_scope),('language-body-local-stack-candidate','kairo.unity.util.Language::MakeLangPackTemplate(System.Boolean,System.Int32,System.String&,System.String&)->System.Void','src/KairoLibrary/kairo/unity/util/Language.cs',r'public\s+static\s+void\s+MakeLangPackTemplate\([^)]*\)',transform_language_body_local),('http-zero-guard-dense-default-bucket','kairo.unity.io.HttpObject::_connect(System.String,System.Byte[],System.Boolean,System.Boolean)->System.Byte[][]','src/KairoLibrary/kairo/unity/io/HttpObject.cs',r'private\s+byte\[\]\[\]\s+_connect\([^)]*\)',transform_http_license_dispatch)]
    sha=lambda raw:hashlib.sha256(raw).hexdigest();patches=[]
    for rule,key,logical,pattern,transform in entries:
        physical=base/logical;raw=physical.read_bytes();text=raw.decode('utf-8-sig');m=unique(pattern,text);body=text.index('{',m.end());end=closing(text,body)+1;before=text[m.start():end];after=transform(before)
        assert after!=before
        p={'method_id':key,'logical_path':logical,'rule':rule,'start':len(text[:m.start()].encode('utf-16-le'))//2,'end':len(text[:end].encode('utf-16-le'))//2,'before':before,'after':after,'source_file_sha256':sha(raw),'source_text_sha256':sha(text.encode()),'before_sha256':sha(before.encode()),'after_sha256':sha(after.encode()),'calculator_sha256':sha(Path(__file__).read_bytes()),'source_snapshot':'build/round10/cycles/c01/after/KairoLibrary','source_only':True,'AST_analysis':'NOT_RUN','slot_model':'NOT_RUN','fixture':'NOT_TESTED','game_compilations':0,'readiness':'NOT_READY','rebase_policy':'fresh unique exact UTF16 before only; frozen C1 is not current Main binding'}
        patches.append(p)
        (out/(rule+'.before.cs')).write_bytes(before.encode('utf-8'));(out/(rule+'.after.cs')).write_bytes(after.encode('utf-8'))
    (out/'proposals.json').write_text(json.dumps({'operation':'three independent bounded source hypotheses','adopted':False,'game_compilations':0,'readiness':'NOT_READY','patches':patches},indent=2)+'\n',encoding='utf-8')
    print(json.dumps([{k:p[k] for k in ('method_id','rule','start','end','before_sha256','after_sha256')} for p in patches],indent=2))


if __name__=='__main__':main()
