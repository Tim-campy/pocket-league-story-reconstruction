"""Pure source-only script-selector/dispatch compositions for a frozen Round10 group.

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


def switch_sections(text,selector):
    t=lex(text);i,n=find(t,'switch('+selector+')');opening=i+n
    if t[opening].value!='{':raise ValueError('switch-explicit-body')
    closing=matching(t,opening);sections=[];p=opening+1
    while p<closing:
        labels=[]
        while p<closing and t[p].value in ('case','default'):
            if t[p].value=='default':
                if t[p+1].value!=':':raise ValueError('default-label')
                labels.append(None);p+=2
            else:
                q=p+1
                while t[q].value!=':':q+=1
                number=''.join(z.value for z in t[p+1:q])
                if not re.fullmatch(r'-?\d+',number):raise ValueError('integer-constant-case-only')
                labels.append(int(number));p=q+1
        if not labels:raise ValueError('case-label-required')
        first=p
        while p<closing and t[p].value not in ('case','default'):
            if t[p].value in ('{','(','['):p=matching(t,p)+1
            else:p+=1
        if p==first:raise ValueError('empty-section-without-payload')
        sections.append({'labels':labels,'first':first,'stop':p})
    return t,i,opening,closing,sections


def convert_switch(text,selector,named_selector=None,reverse_group=None,steps=None):
    """Strict converter: reject nonterminal breaks owned by this dispatch."""
    steps=[] if steps is None else steps
    t,i,opening,closing,sections=switch_sections(text,selector)
    if any(z.value in ('goto','yield','await','delegate','=>') for z in t[opening+1:closing]):
        raise ValueError('unmodeled-dispatch-transfer-or-capture')
    result=[]
    for no,section in enumerate(sections):
        first,stop=section['first'],section['stop'];a,b=first,stop
        while a<b and t[a].value=='{' and matching(t,a)==b-1:a+=1;b-=1
        terminal_break=(b-a>=2 and [z.value for z in t[b-2:b]]==vals('break;'))
        # Loops/switches inside the case own their own breaks, which are never
        # removed. Their complete statement intervals suffice for this corpus.
        children=[]
        for q in range(first,stop):
            if t[q].value in ('for','foreach','while','switch') and q+1<stop and t[q+1].value=='(':
                end=matching(t,q+1)+1
                if end<stop and t[end].value=='{':children.append((q,matching(t,end)))
                else:raise ValueError('single-statement-loop-unmodeled')
            elif t[q].value=='do':raise ValueError('do-break-owner-unmodeled')
        owned=[q for q in range(first,stop) if t[q].value=='break' and not any(x<q<y for x,y in children)]
        allowed=[b-2] if terminal_break else []
        if owned!=allowed:
            snippets=[text[max(0,t[q].start-70):min(len(text),t[q].end+70)] for q in owned if q not in allowed]
            raise ValueError('nonterminal-dispatch-break-requires-source-control-composition:'+repr(snippets))
        payload=text[t[first].start:t[stop-1].end]
        if terminal_break:
            cut=t[b-2].start-t[first].start;end=t[b-1].end-t[first].start
            payload=payload[:cut]+payload[end:]
        elif [z.value for z in t[b-2:b]]!=vals('continue;') and not any(z.value=='return' for z in t[max(a,b-8):b]):
            raise ValueError('case-terminal-transfer-not-proved')
        labels=section['labels']
        if reverse_group and tuple(labels)==tuple(reverse_group):labels=list(reversed(labels))
        prefix='if' if no==0 else 'else if'
        if labels==[None]:
            if no!=len(sections)-1:raise ValueError('default-must-be-last')
            header='else'
        else:
            if None in labels:raise ValueError('default-shared-case-unmodeled')
            sel=named_selector or selector
            header=prefix+' ('+' || '.join('('+sel+' == '+str(x)+')' for x in labels)+')'
        result.append(header+' {\n'+payload+'\n}')
    return edit(text,t[i].start,t[closing].end,'\n'.join(result),'strict-switch-if-'+selector,steps)


def script_outer_dispatch(text,steps=None):
    steps=[] if steps is None else steps
    if any(z.value=='recoveredScriptCommand' for z in lex(text)):raise ValueError('fresh-selector-collision')
    t=lex(text);i,n=find(t,'int[] firstScript=GetFirstScript();')
    text=edit(text,t[i+n-1].end,t[i+n-1].end,'\nint recoveredScriptCommand = firstScript[0];',
              'named-script-selector-slot1',steps)
    text=convert_switch(text,'firstScript[0]','recoveredScriptCommand',steps=steps)
    t=lex(text);hits=[]
    for q,z in enumerate(t):
        if z.value!='finally':continue
        if t[q+1].value!='{':raise ValueError('finally-explicit-body')
        end=matching(t,q+1)
        if [x.value for x in t[q+2:end]]==vals('insertScriptIndex_=-1;') and [x.value for x in t[end+1:end+3]]==vals('break;'):
            hits.append((q,end))
    if len(hits)!=1:raise ValueError('one-script-finally-and-loop-break-required')
    fin,end=hits[0];try_end=fin-1
    if t[try_end].value!='}':raise ValueError('try-before-finally-required')
    # Remove the post-finally break; place it in the try after the dispatch.
    text=edit(text,t[end+1].start,t[end+2].end,'','remove-loop-break-outside-try',steps)
    return edit(text,t[try_end].start,t[try_end].start,'break;\n','place-loop-break-inside-try',steps)


def transform_player_script(text):
    return script_outer_dispatch(text)


def bonus_zero_chain(text,steps=None):
    steps=[] if steps is None else steps
    t=lex(text)
    # Specific names and zero-chain are checked before touching declaration order.
    i,n=find(t,'int num5;int num6;int num7;int num8;int num9;int num12;int num11;int num10;'
             'int num4=(num5=(num6=(num7=(num8=(num9=(num10=(num11=(num12=0))))))));int num13=20;')
    declaration=('int num13, num4, num5, num6, num7, num8, num9, num10, num11, num12;\n'
                 'num13 = num4 = num5 = num6 = num7 = num8 = num9 = num10 = num11 = num12 = 0;\nnum13 = 20;')
    return edit(text,t[i].start,t[i+n-1].end,declaration,'bonus-ten-variable-declaration-and-zero-chain',steps)


def transform_bonus_fan(text):
    text=bonus_zero_chain(text)
    if any(z.value=='recoveredBonusKind' for z in lex(text)):raise ValueError('fresh-bonus-selector-collision')
    t=lex(text);i,_=find(t,'switch(array2[4])')
    text=edit(text,t[i].start,t[i].start,'int recoveredBonusKind = array2[4];\n','named-bonus-selector-slot32',[])
    text=convert_switch(text,'array2[4]','recoveredBonusKind')
    text=convert_switch(text,'num18')
    text=convert_switch(text,'num2')
    return convert_switch(text,'l')


