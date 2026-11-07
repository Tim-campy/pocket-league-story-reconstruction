"""Pure source-only unused-local/scope compositions for a frozen Round10 group.

No file access at import/call. NOT_READY: no model, compiler or oracle invoked.
The lexical anchors bind this proposal to the reviewed source patterns; they do
not establish types, metadata, slot allocation or a game method's correctness.
"""
from dataclasses import dataclass
from collections import Counter
import re


@dataclass(frozen=True)
class Token:
    value: str
    start: int
    end: int


def lex(text):
    out = []
    i = 0
    while i < len(text):
        if text[i].isspace():
            i += 1
            continue
        if text.startswith('//', i):
            end = text.find('\n', i + 2)
            i = len(text) if end < 0 else end
            continue
        if text.startswith('/*', i):
            end = text.find('*/', i + 2)
            if end < 0:
                raise ValueError('unterminated-comment')
            i = end + 2
            continue
        start = i
        verbatim = text.startswith('@"', i)
        if verbatim or text[i] in ('"', "'"):
            quote = '"' if verbatim else text[i]
            i += 2 if verbatim else 1
            while i < len(text):
                if text[i] == quote:
                    if verbatim and text.startswith('""', i):
                        i += 2
                        continue
                    i += 1
                    break
                i += 2 if not verbatim and text[i] == '\\' else 1
            else:
                raise ValueError('unterminated-literal')
        else:
            m = re.match(r'[A-Za-z_@][A-Za-z_0-9]*|(?:0[xX][A-Fa-f0-9]+|[0-9]+(?:\.[0-9]+)?)(?:[fFdDmMuUlL]+)?|==|!=|<=|>=|&&|\|\||\+\+|--|=>', text[i:])
            i += len(m.group()) if m else 1
        out.append(Token(text[start:i], start, i))
    return out


def vals(text):
    return [t.value for t in lex(text)]


def matching(t, start):
    pairs = {'(': ')', '[': ']', '{': '}'}
    stack = [pairs[t[start].value]]
    for i in range(start + 1, len(t)):
        v = t[i].value
        if v in pairs:
            stack.append(pairs[v])
        elif v in (')', ']', '}'):
            if not stack or stack.pop() != v:
                raise ValueError('unbalanced-delimiter')
            if not stack:
                return i
    raise ValueError('unclosed-delimiter')


def find(t, needle):
    wanted = vals(needle)
    hits = [i for i in range(len(t) - len(wanted) + 1)
            if [z.value for z in t[i:i + len(wanted)]] == wanted]
    if len(hits) != 1:
        raise ValueError('pattern-not-unique:' + needle[:90])
    return hits[0], len(wanted)


def frames(t, pos):
    stack = []
    for i in range(pos):
        if t[i].value == '{':
            stack.append(i)
        elif t[i].value == '}':
            stack.pop()
    return stack


def body(t, start):
    if t[start].value not in ('if', 'for') or t[start + 1].value != '(':
        raise ValueError('if-or-for-required')
    opening = matching(t, start + 1) + 1
    if t[opening].value != '{':
        raise ValueError('explicit-body-required')
    return opening, matching(t, opening)


def edit(text, start, end, replacement, tag, steps):
    steps.append({'component': tag,
                  'start_utf16_in_step_input': len(text[:start].encode('utf-16-le')) // 2,
                  'end_utf16_in_step_input': len(text[:end].encode('utf-16-le')) // 2,
                  'before': text[start:end], 'after': replacement})
    return text[:start] + replacement + text[end:]


def copy_area_declarations(text, chip_type, chip_name, steps=None):
    steps=[] if steps is None else steps
    if (chip_type,chip_name) not in [('MapchipMatch','mapchipMatch'),('MapchipMain','mapchipMain')]:
        raise ValueError('unreviewed-chip-type')
    t=lex(text)
    if any(z.value in ('delegate','=>','goto','yield','await','unsafe') for z in t):
        raise ValueError('unsupported-capture-or-transfer')
    fresh=['recoveredUnusedSlot'+str(n) for n in [3,*range(5,13)]]
    if any(z.value in fresh for z in t):raise ValueError('fresh-local-collision')
    i,n=find(t,'global::System.Int32 originalUnusedSlot2; int num2; bool flag2=false;')
    stop=i+n
    chip_header=vals('global::data.'+chip_type+' '+chip_name)
    if [z.value for z in t[stop:stop+len(chip_header)]]!=chip_header:
        raise ValueError('chip-declaration-must-follow-reviewed-prefix')
    declarations=('global::System.Int32 originalUnusedSlot2, recoveredUnusedSlot3;\n'
                  'bool flag2 = false;\nint '+', '.join(fresh[1:]+['width','height'])+';\n')
    text=edit(text,t[i].start,t[stop-1].end,declarations,'copyarea-root-slots2-through14',steps)
    t=lex(text);i,n=find(t,'num2=0; int num3=0;')
    # The counter is now declared after the chip reference; do not initialize it
    # alongside the earlier bare locals or it would emit extra stores there.
    text=edit(text,t[i].start,t[i+n-1].end,'int num2 = 0;\nint num3 = 0;',
              'copyarea-counter-after-chip-slot16',steps)
    for name,getter in [('height','GetHeight'),('width','GetWidth')]:
        t=lex(text);i,n=find(t,'int '+name+'=image.'+getter+'();')
        text=edit(text,t[i].start,t[i].end,'','copyarea-'+name+'-assignment-to-root-local',steps)
    return text


def copy_area_main_per_arm(text, steps=None):
    steps=[] if steps is None else steps
    t=lex(text);i,n=find(t,'mapchipMain=((MAP_GROUND!=0)?mapchipGrounds[MAP_GROUND-1]:null);')
    return edit(text,t[i].start,t[i+n-1].end,
                'if (MAP_GROUND == 0) { mapchipMain = null; } else { mapchipMain = mapchipGrounds[MAP_GROUND - 1]; }',
                'copyarea-main-per-arm-reference-stores',steps)


def transform_map_copyarea(text):
    return copy_area_declarations(text,'MapchipMatch','mapchipMatch')


def transform_mapmain_copyarea(text):
    return copy_area_main_per_arm(copy_area_declarations(text,'MapchipMain','mapchipMain'))


def default_coat_shared_locals(text, steps=None):
    steps=[] if steps is None else steps
    t=lex(text)
    if any(z.value in ('delegate','=>','goto','yield','await','unsafe') for z in t):
        raise ValueError('unsupported-capture-or-transfer')
    if any(z.value=='recoveredUnusedSingleSlot1' for z in t):raise ValueError('fresh-local-collision')
    i,n=find(t,'float num=100f; int num5; float num2=1000f; global::System.Single originalUnusedSlot1;')
    replacement=('float num = 100f;\nglobal::System.Single recoveredUnusedSingleSlot1;\n'
                 'float num2 = 1000f;\nglobal::System.Single originalUnusedSlot1;\nfloat num3, num4;')
    text=edit(text,t[i].start,t[i+n-1].end,replacement,'defaultcoat-six-root-floats',steps)
    t=lex(text);i,n=find(t,'for(int i=0;i<MAPCHIP_COAT_HNUM;i++)')
    fo,fc=body(t,i)
    refs=[j for j,z in enumerate(t) if z.value=='num5']
    if not refs or not all(fo<j<fc for j in refs):raise ValueError('num5-must-only-belong-to-first-i-loop')
    i,n=find(t,'num5=COAT_GOAL2_ADDRESS[1][1]-i;')
    text=edit(text,t[i].start,t[i].start,'int ','defaultcoat-num5-first-loop-scope',steps)
    t=lex(text);hits=[]
    for j in range(len(t)-3):
        if t[j].value=='float' and t[j+1].value in ('num3','num4') and t[j+2].value=='=':
            hits.append(j)
    counts=Counter(t[j+1].value for j in hits)
    if counts!={'num3':4,'num4':4}:raise ValueError('exactly-four-declarations-of-each-shared-float-required')
    for j in reversed(hits):
        text=edit(text,t[j].start,t[j].end,'','defaultcoat-local-declaration-to-shared-assignment',steps)
    return text


def transform_default_coat(text):
    return default_coat_shared_locals(text)


