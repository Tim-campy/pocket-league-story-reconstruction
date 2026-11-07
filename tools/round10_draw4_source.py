"""Pure source-only proposal for the seven composed draw4 regions.

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


def type48_common_exit(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    i, n = find(t, 'if(type_==48){return;} if(type_==49)')
    # Exact adjacent arms, not a return buried inside a loop/protected region.
    for p in range(i):
        if t[p].value in ('for', 'foreach', 'while', 'switch', 'using', 'lock', 'catch') and p+1 < i and t[p+1].value == '(':
            o = matching(t, p+1)+1
            if o < i and t[o].value == '{' and matching(t, o) > i:
                raise ValueError('type48-owner-cannot-be-loop-or-protected')
        elif t[p].value in ('try', 'finally', 'do') and p+1 < i and t[p+1].value == '{' and matching(t,p+1) > i:
            raise ValueError('type48-owner-cannot-be-loop-or-protected')
    end = t[i + n - 1].end
    return edit(text, t[i].start, end, 'if (type_ == 48) { } else if (type_ == 49)',
                'type48-empty-arm-common-exit', steps)


def training_range_positive_scope(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    f, _ = find(t, 'for(int n=0;n<TrainingData.TRA_PLAYER.Length;n++)')
    opening, closing = body(t, f)
    g, _ = find(t, 'if(training_cnt<TRA_ANIME_TIME2[0]||training_cnt>=TRA_ANIME_TIME2[1])')
    go, gc = body(t, g)
    if not opening < g < gc < closing or frames(t, g)[-1] != opening:
        raise ValueError('range-guard-not-direct-child-of-n-loop')
    if [z.value for z in t[go+1:gc]] != vals('continue;'):
        raise ValueError('range-guard-must-only-continue')
    tail = text[t[gc].end:t[closing].start]
    # Preserve ALL declarations in their lexical order; do not hoist num25
    # ahead of the child num24. Parser implicit blocks are material here.
    start_anchor = vals('int num19 = training_cnt - TRA_ANIME_TIME2[0];')
    if vals(tail)[:len(start_anchor)] != start_anchor:
        raise ValueError('range-tail-start-changed')
    for anchor in ('int num20=num19/25;', 'int num21=num19%25;',
                   'int num22=num2-13;', 'int num23=87;',
                   'int num24=array[num20];',
                   'int num25=GameUtil.RateConvert(num21,TRA_AT[0],TRA_AT[1],12,0,true);'):
        find(lex(tail), anchor)
    if vals(tail)[-6:] != vals('g.PopClip();'):
        raise ValueError('range-tail-end-changed')
    replacement = ('if (training_cnt >= TRA_ANIME_TIME2[0] && training_cnt < TRA_ANIME_TIME2[1]) {'
                   + tail + '\n}')
    return edit(text, t[g].start, t[closing].start, replacement,
                'range-positive-if-preserve-declarations', steps)


def training_player_continue(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    f, _ = find(t, 'for(int num31=0;num31<TrainingData.TRA_PLAYER.Length;num31++)')
    fo, fc = body(t, f)
    i, _ = find(t, 'if(TrainingData.TRA_PLAYER[num31]!=-1)')
    opening, closing = body(t, i)
    if frames(t, i)[-1] != fo or closing + 1 != fc:
        raise ValueError('player-positive-if-must-be-loop-tail')
    expected_start = vals('PlayerData playerData4=apdat_.pData_[TrainingData.TRA_PLAYER[num31]];')
    if vals(text[t[opening].end:t[closing].start])[:len(expected_start)] != expected_start:
        raise ValueError('player-body-start-changed')
    # Retain the existing standalone child block, and thus declaration scope.
    return edit(text, t[i].start, t[opening].start,
                'if (TrainingData.TRA_PLAYER[num31] == -1) { continue; }\n',
                'missing-player-explicit-continue-preserve-child', steps)


_RGB = [(252,54,0), (84,188,255), (103,255,121), (248,248,248), (255,251,0)]


def color_dispatch(text, selector, steps=None):
    if selector not in ('num28', 'num47'):
        raise ValueError('selector-not-reviewed')
    steps = [] if steps is None else steps
    t = lex(text)
    i, n = find(t, 'switch(' + selector + ')')
    opening = i + n
    if t[opening].value != '{':
        raise ValueError('switch-explicit-body-required')
    closing = matching(t, opening)
    p = opening + 1
    pieces = []
    for case, rgb in enumerate(_RGB):
        wanted = vals('case ' + str(case) + ':')
        if [z.value for z in t[p:p+3]] != wanted:
            raise ValueError('color-cases-must-be-0-through-4-in-order')
        p += 3
        statement_start = p
        expected = 'g.SetColor(Graphics.GetColorOfRGB(' + ','.join(map(str, rgb)) + '));'
        statement_end = p + len(vals(expected))
        if [z.value for z in t[p:statement_end]] != vals(expected):
            raise ValueError('color-payload-exact-template-required')
        p = statement_end
        if [z.value for z in t[p:p+2]] != vals('break;'):
            raise ValueError('color-case-own-terminal-break-required')
        payload = text[t[statement_start].start:t[statement_end-1].end]
        pieces.append(('if' if case == 0 else 'else if') + ' (' + selector + ' == ' + str(case) + ') { ' + payload + ' }')
        p += 2
    if p != closing:
        raise ValueError('extra-color-section-or-default')
    return edit(text, t[i].start, t[closing].end, '\n'.join(pieces),
                'color-five-way-direct-comparisons-' + selector, steps)


def formation_decl_chain(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    i, n = find(t, 'int num37=0; for(int num38=0;num38<formationData.formation_.Length;num38++)')
    header_end = i + n
    if t[header_end].value != '{':
        raise ValueError('formation-explicit-body-required')
    closing = matching(t, header_end)
    d = header_end + 1
    anchor = 'int num39=GameUtil.RateConvert(formationData.formation_[num38][1],0,8,num2+num35-10,num2+10,true);'
    wanted = vals(anchor)
    if [z.value for z in t[d:d+len(wanted)]] != wanted:
        raise ValueError('formation-first-declaration-exact-required')
    if any(z.value in ('goto', 'delegate', '=>', 'try', 'yield', 'await') for z in t[i:closing+1]):
        raise ValueError('formation-unsupported-transfer-or-capture')
    # The two named coordinates share a declaration group, so insertion order
    # is column1 (67), column0 (68), before the child index (69).
    before_for = text[t[i+5].start:t[header_end].start]  # int num37 = 0 ; = five tokens
    if vals(before_for) != vals('for(int num38=0;num38<formationData.formation_.Length;num38++)'):
        raise ValueError('formation-header-guard')
    text = edit(text, t[d].start, t[d].end, '', 'formation-num39-declaration-to-assignment', steps)
    # Removing only "int" above does not affect the preceding offsets.
    return edit(text, t[i].start, t[i+4].end,
                'int num39, num37;\nnum39 = num37 = 0;',
                'formation-coordinate-group-and-zero-chain', steps)


def jp_per_arm_stores(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    i, n = find(t, 'num=((!MyFormBase.Jp())?51:50);')
    return edit(text, t[i].start, t[i+n-1].end,
                'if (MyFormBase.Jp()) { num = 50; } else { num = 51; }',
                'jp-positive-per-arm-stores', steps)


def arrow_common_join(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    i, _ = find(t, 'if(udArrowCnt==0)')
    opening, closing = body(t, i)
    expected = ('GameView.AddTouch(g,2,num4-25-5,num-50,70,30,1376456,TouchOption.Create(2));'
                'GameView.AddTouch(g,2,num4-25-5,num+100,70,30,1376456,TouchOption.Create(2));return;')
    if [z.value for z in t[opening+1:closing]] != vals(expected):
        raise ValueError('arrow-then-exact-two-calls-and-return-required')
    owner = frames(t, i)[-1]
    owner_end = matching(t, owner)
    expected_tail = ('TouchOption touchOption=TouchOption.Create(2);'
                     'TouchOption touchOption2=TouchOption.Create(2);'
                     'GameView.AddTouch(g,24,num4,num-13,apdat_.resCom2_.seb[24],0,17,touchOption.Margin(40,40,45,0));'
                     'GameView.AddTouch(g,24,num4,num+64+13,apdat_.resCom2_.seb[24],1,19,touchOption2.Margin(40,40,-15,60));')
    if [z.value for z in t[closing+1:owner_end]] != vals(expected_tail):
        raise ValueError('arrow-tail-exact-reviewed-declarations-and-calls-required')
    after_owner = t[owner_end+1:owner_end+9]
    if [z.value for z in after_owner] != vals('else { _draw5(g); }'):
        raise ValueError('arrow-owner-needs-draw5-sibling')
    ret = closing - 2
    tail = text[t[closing].end:t[owner_end].start]
    # Both TouchOptions remain in lexical tail order. They are the last named
    # locals, and are allocated after the preceding loop's descendant locals.
    replacement = text[t[i].start:t[ret].start] + '\n} else {' + tail + '\n}'
    return edit(text, t[i].start, t[owner_end].start, replacement,
                'arrow-local-else-common-join', steps)


def inventory(text):
    t = lex(text)
    excluded = {'if','for','while','switch','using','lock','catch','typeof','sizeof','checked','unchecked','default'}
    calls = [t[i-1].value for i,z in enumerate(t) if i and z.value == '(' and
             re.fullmatch(r'[A-Za-z_@][A-Za-z_0-9]*', t[i-1].value) and t[i-1].value not in excluded]
    return {'call_like_names': dict(sorted(Counter(calls).items())),
            'quoted_literals': [z.value for z in t if z.value.startswith(('"','@"',"'"))],
            'floating_literal_tokens': [z.value for z in t if re.fullmatch(r'[0-9]+(?:\.[0-9]+)?[fFdDmM]', z.value)],
            'return_count': sum(z.value == 'return' for z in t),
            'anonymous_functions': sum(z.value in ('delegate','=>') for z in t)}


def calculate_draw4(method_source):
    steps = []
    after = method_source
    for fn in (type48_common_exit, training_range_positive_scope,
               training_player_continue,
               lambda s, steps: color_dispatch(s, 'num28', steps),
               formation_decl_chain, jp_per_arm_stores,
               lambda s, steps: color_dispatch(s, 'num47', steps),
               arrow_common_join):
        after = fn(after, steps)
    before_inv, after_inv = inventory(method_source), inventory(after)
    for key in ('call_like_names','quoted_literals','floating_literal_tokens','anonymous_functions'):
        if before_inv[key] != after_inv[key]:
            raise ValueError('preserved-inventory-changed:' + key)
    if before_inv['return_count'] - after_inv['return_count'] != 2:
        raise ValueError('exactly-two-reviewed-returns-must-be-removed')
    return {'status': 'SOURCE_ONLY_NOT_READY', 'after': after, 'changes': steps,
            'before_inventory': before_inv, 'after_inventory': after_inv,
            'model_complete': None, 'body_equal': None,
            'compiler_model_ast_verifier_called': False,
            'limits': ['lexical types require independent semantic/model validation',
                       'slot allocation and maxstack are predictions, not measurements',
                       'all method bytes and metadata require the full oracle']}


def transform_draw4(method_source):
    return calculate_draw4(method_source)['after']
