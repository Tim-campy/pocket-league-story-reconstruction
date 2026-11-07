"""Two lambda-parent compositions. Text only; no I/O or side effects on import."""
import re

class Refused(ValueError):
    pass

def unique(pattern, text):
    matches = list(re.finditer(pattern, text))
    if len(matches) != 1:
        raise Refused('anchor-count-%s:%s' % (len(matches), pattern))
    return matches[0]

def closing(text, start):
    depth = 0; quote = None; escape = False; i = start
    if text[i] != '{':
        raise Refused('not-block')
    while i < len(text):
        c = text[i]
        if quote:
            if escape: escape = False
            elif c == '\\': escape = True
            elif c == quote: quote = None
        elif text.startswith('//', i):
            j = text.find('\n', i + 2); i = len(text) if j < 0 else j
            continue
        elif text.startswith('/*', i):
            j = text.find('*/', i + 2)
            if j < 0: raise Refused('unclosed-comment')
            i = j + 2; continue
        elif c in ('"', "'"): quote = c
        elif c == '{': depth += 1
        elif c == '}':
            depth -= 1
            if depth == 0: return i
        i += 1
    raise Refused('unclosed-block')

def transform_resource_lambda_dispatch(source):
    """Two sites in the parent source; compiler continues to own the lambda."""
    if re.search(r'\brecoveredOptionChar\b', source):
        raise Refused('selector-name-not-fresh')
    atlas = unique(r'imageAtlasId_\s*=\s*\(\(text4\.Length\s*!=\s*1\)\s*\?\s*int\.Parse\(text4\.Substring\(1\)\)\s*:\s*0\)\s*;', source)
    nl = '\r\n' if '\r\n' in source else '\n'
    source = source[:atlas.start()] + 'if (text4.Length == 1) { imageAtlasId_ = 0; } else { imageAtlasId_ = int.Parse(text4.Substring(1)); }' + source[atlas.end():]
    dispatch = unique(r'switch\s*\(array9\[num22\]\.CharAt\(0\)\)\s*\{', source)
    end = closing(source, source.index('{', dispatch.start())) + 1
    old = source[dispatch.start():end]
    expected = r"switch\s*\(array9\[num22\]\.CharAt\(0\)\)\s*\{\s*case\s*'u'\s*:\s*u\s*=\s*true\s*;\s*break\s*;\s*case\s*'v'\s*:\s*v\s*=\s*true\s*;\s*break\s*;\s*case\s*'c'\s*:\s*num21\s*=\s*JInteger\.ParseInt\(array9\[num22\]\.Substring\(1\)\)\s*;\s*break\s*;\s*\}"
    if not re.fullmatch(expected, old):
        raise Refused('char-switch-body-differs')
    replacement = ('char recoveredOptionChar = array9[num22].CharAt(0);' + nl +
        "if (recoveredOptionChar == 'u') { u = true; }" + nl +
        "else if (recoveredOptionChar == 'v') { v = true; }" + nl +
        "else if (recoveredOptionChar == 'c') { num21 = JInteger.ParseInt(array9[num22].Substring(1)); }")
    return source[:dispatch.start()] + replacement + source[end:]

def transform_http_lambda_scopes(source):
    """Preserve lambda capture set; recover declaration ownership and two tests."""
    nl = '\r\n' if '\r\n' in source else '\n'
    # Declaration only; the new-object expression remains at its original site.
    root = unique(r'byte\[\]\s+bytes\s*=\s*http_\.bytes\s*;', source)
    source = source[:root.end()] + nl + 'Vector vector;' + source[root.end():]
    vector = unique(r'Vector\s+vector\s*=\s*new\s+Vector\(\)\s*;', source)
    source = source[:vector.start()] + 'vector = new Vector();' + source[vector.end():]
    # This explicit condition temporary is present in the frozen source, but
    # original IL uses the direct inverse comparison branch, not clt/brfalse.
    wrapper = unique(r'\{\s*bool\s+recoveredCondition_5948\s*=\s*1\s*<\s*array\.Length\s*;\s*if\s*\(recoveredCondition_5948\)\s*\{', source)
    begin = wrapper.start(); body_open = source.rfind('{', wrapper.start(), wrapper.end())
    body_end = closing(source, body_open); wrapper_end = closing(source, begin)
    if source[body_end + 1:wrapper_end].strip():
        raise Refused('condition-wrapper-has-additional-statements')
    source = source[:begin] + 'if (1 < array.Length) ' + source[body_open:body_end + 1] + source[wrapper_end + 1:]
    # There must be exactly this one positive payload guard in the parent.
    guard = unique(r'if\s*\(error_\s*==\s*null\)\s*\{\s*vector\s*=\s*new\s+Vector\(\)\s*;', source)
    open_brace = source.index('{', guard.start()); guard_end = closing(source, open_brace)
    # Keep the payload block; locals inside it retain their lexical ownership.
    source = source[:guard.start()] + 'if (error_ != null) { return; }' + nl + source[open_brace:guard_end + 1] + source[guard_end + 1:]
    loop = unique(r'byte\[\]\s+array2\s*;\s*for\s*\(\s*;\s*recvSize\s*<\s*totalSize\s*;\s*recvSize\s*\+=\s*array2\.Length\s*\)\s*\{', source)
    loop_open = source.rfind('{', loop.start(), loop.end()); loop_end = closing(source, loop_open)
    body = source[loop_open + 1:loop_end]
    if re.search(r'\b(?:break|continue|goto|yield|await|delegate)\b|=>', body):
        raise Refused('loop-has-transfer-or-capture')
    assign = unique(r'array2\s*=\s*StreamUtil\.Read\(bais,\s*size\)\s*;', body)
    body = body[:assign.start()] + 'byte[] ' + body[assign.start():]
    body += nl + 'recvSize += array2.Length;' + nl
    return source[:loop.start()] + 'while (recvSize < totalSize)' + nl + '{' + body + '}' + source[loop_end + 1:]

def transform_seb_scope_dispatch_record(source):
    """Recover the complete root/child declaration order, plus four IL motifs."""
    if re.search(r'\brecoveredSpriteRecord\b', source):
        raise Refused('sprite-record-name-not-fresh')
    nl = '\r\n' if '\r\n' in source else '\n'
    header = unique(r'short\[\]\[\]\s+array\s*;\s*int\s+num2\s*=\s*dataInputStream\.ReadByte\(\)\s*;\s*int\s+num3\s*;', source)
    source = source[:header.start()] + 'int num3;' + nl + 'int num2 = dataInputStream.ReadByte();' + source[header.end():]
    array = unique(r'(?<!\w)array\s*=\s*new\s+short\[num3\]\[\]\s*;', source)
    source = source[:array.start()] + 'short[][] ' + source[array.start():]
    # Array5 must remain a root-try local even though its initialization is late.
    union = unique(r'int\[\]\s+array5\s*=\s*new\s+int\[4\]\s*;', source)
    source = source[:union.start()] + 'array5 = new int[4];' + source[union.end():]
    tail = unique(r'int\s+l\s*=\s*0\s*;\s*int\s+num16\s*=\s*0\s*;', source)
    try_anchor = unique(r'\btry\s*\{\s*dataInputStream\s*=\s*new\s+DataInputStream\(stream\)\s*;', source)
    try_open = source.index('{',try_anchor.start()); try_end = closing(source,try_open)
    if not try_open < tail.start() < try_end:
        raise Refused('tail-not-in-root-try')
    source = source[:tail.start()] + 'int[] array5;' + nl + '{' + nl + source[tail.start():try_end] + nl + '}' + nl + source[try_end:]
    # Nested color dispatch. Preserve the original array-store leaves exactly.
    color = unique(r'switch\s*\(num8\)\s*\{',source)
    end = closing(source,source.index('{',color.start())) + 1
    old = source[color.start():end]
    expected = r'switch\s*\(num8\)\s*\{\s*case\s+8192\s*:\s*array3\[10\]\s*=\s*1\s*;\s*break\s*;\s*case\s+16384\s*:\s*array3\[10\]\s*=\s*2\s*;\s*break\s*;\s*case\s+32768\s*:\s*array3\[10\]\s*=\s*3\s*;\s*break\s*;\s*\}'
    if not re.fullmatch(expected,old): raise Refused('color-switch-body-differs')
    replacement = 'if (num8 == 8192) { array3[10] = 1; } else if (num8 == 16384) { array3[10] = 2; } else if (num8 == 32768) { array3[10] = 3; }'
    source = source[:color.start()] + replacement + source[end:]
    version = unique(r'switch\s*\(num\)\s*\{\s*case\s+0\s*:\s*\{',source)
    switch_open = source.index('{',version.start()); switch_end = closing(source,switch_open)
    case0_open = source.rfind('{',version.start(),version.end()); case0_end = closing(source,case0_open)
    body0 = source[case0_open+1:case0_end]
    terminal = re.search(r'\bbreak\s*;\s*$',body0)
    if terminal is None: raise Refused('version-case0-terminal-break-missing')
    body0 = body0[:terminal.start()]+body0[terminal.end():]
    rest = source[case0_end+1:switch_end]
    case1 = unique(r'^\s*case\s+1\s*:',rest); body1 = rest[case1.end():]
    terminal = re.search(r'\bbreak\s*;\s*$',body1)
    if terminal is None: raise Refused('version-case1-terminal-break-missing')
    body1 = body1[:terminal.start()]+body1[terminal.end():]
    # These particular case leaves have no internal transfer. Do not alter
    # nested loops or guess ownership for any unfamiliar switch shape.
    if re.search(r'\b(?:break|continue|goto|case|default)\b',body0+body1):
        raise Refused('version-case-unproved-transfer')
    source = source[:version.start()] + 'if (num == 0) {' + body0 + '} else if (num == 1) {' + body1 + '}' + source[switch_end+1:]
    record = unique(r'vector\.AddElement\(new\s+short\[7\]\s*\{\s*\(short\)array3\[1\]\s*,\s*\(short\)array3\[6\]\s*,\s*\(short\)array3\[7\]\s*,\s*\(short\)array3\[2\]\s*,\s*\(short\)array3\[3\]\s*,\s*\(short\)array3\[4\]\s*,\s*\(short\)array3\[5\]\s*\}\)\s*;',source)
    assignments = nl.join('recoveredSpriteRecord[%d] = (short)array3[%d];' % (i,index) for i,index in enumerate([1,6,7,2,3,4,5]))
    source = source[:record.start()] + 'short[] recoveredSpriteRecord = new short[7];' + nl + assignments + nl + 'vector.AddElement(recoveredSpriteRecord);' + source[record.end():]
    for index in (2,3):
        compound = unique(r'array5\[%d\]\s*-=\s*array5\[%d\]\s*;' % (index,index-2),source)
        source = source[:compound.start()] + 'array5[%d] = array5[%d] - array5[%d];' % (index,index,index-2) + source[compound.end():]
    return source

def main():
    import hashlib, json
    from pathlib import Path
    w = Path(__file__).resolve().parent
    r = next(p for p in w.parents if (p/'tools/round7_slot_model.py').exists())
    b = r/'build/round10/cycles/cycle07/after/KairoLibrary'
    sha = lambda raw: hashlib.sha256(raw).hexdigest()
    entries = [
        ('resource-lambda-inline-dispatch', 'kairo.unity.ui.ResourceManager/<>c__DisplayClass51_0::<LoadStart>b__0()->System.Void', 'src/KairoLibrary/kairo/unity/ui/ResourceManager.cs', r'public\s+void\s+LoadStart\(JarInflater jar, int m3dVer = 3\)', transform_resource_lambda_dispatch),
        ('http-lambda-declaration-owner', 'kairo.unity.io.HttpObject/<>c__DisplayClass10_0::<_connect>b__2()->System.Void', 'src/KairoLibrary/kairo/unity/io/HttpObject.cs', r'(?:public|private|protected|internal)\s+byte\[\]\[\]\s+_connect\([^)]*\)', transform_http_lambda_scopes),
        ('seb-root-tail-dispatch-record', 'kairo.unity.ui.Seb::_load(java.io.InputStream)->System.Void', 'src/KairoLibrary/kairo/unity/ui/Seb.cs', r'(?:public|private|protected|internal)\s+void\s+_load\(InputStream stream\)', transform_seb_scope_dispatch_record),
    ]
    patches = []
    for rule, method_id, logical, pattern, transform in entries:
        raw = (b/logical).read_bytes(); source = raw.decode('utf-8-sig')
        match = unique(pattern, source); end = closing(source, source.index('{', match.end())) + 1
        before = source[match.start():end]; after = transform(before)
        item = {'method_id':method_id, 'logical_path':logical, 'rule':'d10-large-lambda-composition-' + rule,
            'start':len(source[:match.start()].encode('utf-16-le'))//2, 'end':len(source[:end].encode('utf-16-le'))//2,
            'before':before, 'after':after, 'source_file_sha256':sha(raw), 'source_text_sha256':sha(source.encode()),
            'source_snapshot':str(b.relative_to(r)).replace('\\','/'), 'calculator_sha256':sha(Path(__file__).read_bytes()),
            'own_compilation_receipt_sha256':sha((b/'verification.json').read_bytes()), 'own_dump_sha256':sha((b/'rebuilt-methods.json').read_bytes()),
            'original_dump_sha256':sha((b/'original-methods.json').read_bytes()), 'own_body_source_bound':True,
            'source_only':True, 'game_compilations':0, 'readiness':'NOT_READY', 'game_body_outcome':'UNKNOWN',
            'generated_method_source_policy':'edit parent C# only; do not author compiler-generated classes',
            'rebase_policy':'fresh exact unique UTF16 parent declaration before; snapshot C7 only, no current Main binding'}
        patches.append(item)
        (w/(rule+'.before.cs')).write_bytes(before.encode())
        (w/(rule+'.after.cs')).write_bytes(after.encode())
    (w/'proposals.json').write_text(json.dumps({'family':'d10-large-lambda-composition', 'patches':patches, 'game_compilations':0, 'adopted':False}, indent=2)+'\n',encoding='utf-8')
    print([(p['method_id'],p['start'],p['end']) for p in patches])

if __name__ == '__main__': main()
