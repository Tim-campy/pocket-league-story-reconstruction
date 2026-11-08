"""Three finite pure source producers; no I/O, target emit or state mutation."""
import re
RECORD_RULE='r12-recordstore-preference-array-foreach-reused-temps'
TRANSLATE_RULE='r12-translate-inline-mixed-array-set-argument'
SQRT_RULE='r12-sqrt-root-local-order-preserved-initialization'

def transform_recordstore_preference_foreach(source):
    pattern=re.compile(r'string\[\] files = KairoPlugin.GetPreferenceKeys\(\);\s*for \(int i = 0; i < files.Length; i\+\+\)\s*\{\s*KairoPlugin.RemovePreference\(files\[i\]\);\s*\}')
    matches=list(pattern.finditer(source))
    if len(matches)!=1:raise ValueError('one original preference array for-loop required')
    if 'string key'in source or 'foreach (string key'in source:raise ValueError('fresh foreach key required')
    if source.count('foreach (string filename in files)')!=1:raise ValueError('first filename foreach must remain untouched')
    m=matches[0];return source[:m.start()]+'foreach (string key in KairoPlugin.GetPreferenceKeys()) { KairoPlugin.RemovePreference(key); }'+source[m.end():]

def transform_translate_inline_array(source):
    pattern=re.compile(r'float\[\] obj = new float\[16\]\s*\{(?P<values>[^{}]+)\};\s*obj\[3\] = x;\s*obj\[7\] = y;\s*obj\[11\] = z;\s*transform.Set\(obj\);')
    matches=list(pattern.finditer(source))
    if len(matches)!=1:raise ValueError('one original identity-array and x/y/z store sequence required')
    m=matches[0];values=[v.strip()for v in m['values'].split(',')]
    expected=['1f'if i in[0,5,10,15]else'0f'for i in range(16)]
    if values!=expected:raise ValueError('original sixteen constant identity entries required')
    if source.count('Transform transform = new Transform();')!=1 or source.count('Multiply(transform);')!=1:raise ValueError('original constructed receiver and Multiply leaf required')
    values[3]='x';values[7]='y';values[11]='z'
    replacement='transform.Set(new float[16] { '+', '.join(values)+' });'
    return source[:m.start()]+replacement+source[m.end():]

def transform_sqrt_root_local_order(source):
    pattern=re.compile(r'int num = 10;\s*long num2 = 0L;')
    matches=list(pattern.finditer(source))
    if len(matches)!=1 or source.count('for (int i = 0; i < num; i++)')!=1:raise ValueError('original iteration limit, result and index declarations required')
    if 'long num2;'in source or 'int i;'in source:raise ValueError('source already contains hoisted declaration')
    m=matches[0];replacement='long num2; int i; int num = 10; num2 = 0L;'
    source=source[:m.start()]+replacement+source[m.end():]
    return source.replace('for (int i = 0; i < num; i++)','for (i = 0; i < num; i++)',1)

def closing(source,start):
    tokens=re.finditer(r'//[^\r\n]*|/\*[\s\S]*?\*/|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'|[{}]',source[start:]);depth=0
    for m in tokens:
        if m[0]=='{':depth+=1
        elif m[0]=='}':
            depth-=1
            if depth==0:return start+m.start()
    raise ValueError('unbalanced block')
