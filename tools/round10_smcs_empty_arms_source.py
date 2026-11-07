"""Pure source-only integer-empty-arm proposals for a frozen Round10 group.

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


def terminal_empty_or_chain(text, selector, constants, steps=None):
    """Restore a terminal empty else-if chain; no type/semantic claim here.

    Bound snapshot evidence must establish a primitive Int32 selector. This is
    explicitly an extension of the known empty-arm emission family, not a new
    compiler rule. The final empty else must disappear along with OR splitting.
    """
    if not re.fullmatch(r'[A-Za-z_][A-Za-z_0-9]*', selector):
        raise ValueError('simple-reviewed-selector-required')
    if len(constants)!=2 or any(type(x) is not int for x in constants) or len(set(constants))!=2:
        raise ValueError('exactly-two-distinct-Int32-constants-required')
    steps=[] if steps is None else steps
    condition=' || '.join('('+selector+' == '+str(x)+')' for x in constants)
    needle='if ('+condition+') { } else { }'
    t=lex(text);i,n=find(t,needle)
    if i==0 or t[i-1].value!='else':
        raise ValueError('empty-or-must-continue-existing-reviewed-chain')
    result='if ('+selector+' == '+str(constants[0])+') { } else if ('+selector+' == '+str(constants[1])+') { }'
    return edit(text,t[i].start,t[i+n-1].end,result,'terminal-empty-OR-chain-'+selector,steps)


def transform_friend_init(text):
    return terminal_empty_or_chain(text,'type_',(13,15))


def transform_touch_effect(text):
    return terminal_empty_or_chain(text,'id_',(2,3))


def billing_draw_empty_dispatch(text, steps=None):
    """Three empty Int32 arms + final positive arm, preserving tail source."""
    steps=[] if steps is None else steps
    t=lex(text)
    i,_=find(t,'if(type_==2||type_==3||type_==4||type_!=5)')
    opening,closing=body(t,i)
    if [z.value for z in t[opening+1:closing]]!=vals('return;'):
        raise ValueError('billing-empty-dispatch-guard-only-return-required')
    owner=frames(t,i)[-1];end=matching(t,owner)
    if i!=owner+1 or owner==0 or t[owner-1].value!='else':
        raise ValueError('billing-guard-must-start-the-existing-else')
    tail=text[t[closing].end:t[end].start]
    if vals(tail)[:len(vals('int num3=174;int num4=107;'))]!=vals('int num3=174;int num4=107;'):
        raise ValueError('billing-tail-declarations-not-reviewed')
    # This patch is bound to the exact before hash by the proposal. Additional
    # lexical transfer/capture exclusions protect reuse in own fixture controls.
    if any(z.value in ('return','goto','yield','await','delegate','=>') for z in lex(tail)):
        raise ValueError('billing-tail-unsupported-transfer-or-capture')
    replacement=('if (type_ == 2) { } else if (type_ == 3) { } '
                 'else if (type_ == 4) { } else if (type_ == 5) {'+tail+'\n}')
    return edit(text,t[i].start,t[end].start,replacement,'billing-three-empty-positive-tail-composition',steps)


def transform_billing_draw(text):
    return billing_draw_empty_dispatch(text)


def groupball_empty_case_and_positive_return(text, steps=None):
    """Composed original joins; don't repeat either isolated old primitive."""
    steps=[] if steps is None else steps
    t=lex(text)
    i,n=find(t,'else if(tArea_.tMode_!=4&&tArea_.tMode_==3)')
    text=edit(text,t[i].start,t[i+n-1].end,
              'else if (tArea_.tMode_ == 4) { } else if (tArea_.tMode_ == 3)',
              'groupball-empty-mode4-before-positive3',steps)
    t=lex(text)
    f,_=find(t,'for(int k=0;k<tBallList_.Size();k++)')
    fo,fc=body(t,f)
    g,_=find(t,'if((tBall3.t_owner_==null)||(tBall3.t_owner_.trainingId_[0]!=player.trainingId_[0]))')
    go,gc=body(t,g)
    if frames(t,g)[-1]!=fo or [z.value for z in t[go+1:gc]]!=vals('continue;') or [z.value for z in t[gc+1:fc]]!=vals('return tBall3;'):
        raise ValueError('groupball-exact-loop-continue-and-return-tail-required')
    return edit(text,t[g].start,t[fc].start,
                'if (tBall3.t_owner_ != null && tBall3.t_owner_.trainingId_[0] == player.trainingId_[0]) { return tBall3; }\n',
                'groupball-positive-owner-return-composition',steps)


def transform_groupball(text):
    return groupball_empty_case_and_positive_return(text)


def positive_empty_equal(text, constant, steps=None):
    """Known primitive, offered explicitly as coverage extension, NOT new rule."""
    if constant not in (1,4):
        raise ValueError('constant-not-reviewed')
    steps=[] if steps is None else steps
    t=lex(text);hits=[]
    for i,z in enumerate(t):
        if z.value!='if' or i+1>=len(t) or t[i+1].value!='(':
            continue
        ce=matching(t,i+1);a,b=i+2,ce
        while a<b and t[a].value=='(' and matching(t,a)==b-1:
            a+=1;b-=1
        if [x.value for x in t[a:b]]!=vals('type_ != '+str(constant)):
            continue
        opening,closing=body(t,i)
        if closing!=opening+1:
            continue
        if closing+1<len(t) and t[closing+1].value=='else':
            raise ValueError('empty-positive-equality-cannot-drop-else')
        hits.append((i,ce,opening,closing))
    if len(hits)!=1:
        raise ValueError('one-reviewed-empty-inequality-required')
    i,ce,opening,closing=hits[0]
    return edit(text,t[i].start,t[ce].end,'if (type_ == '+str(constant)+')',
                'known-empty-positive-equality-'+str(constant),steps)


def transform_billing_init(text):
    return positive_empty_equal(text,4)


def transform_match_draw(text):
    return positive_empty_equal(text,1)


