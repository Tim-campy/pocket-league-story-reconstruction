"""Two Round11 hypotheses. Pure source edits with reviewed, narrow anchors.

No target compilation. Float comparison mirroring is a fixture-only mold here:
the Map target has no such divergent comparison. Unknown types are rejected.
"""
import re


def _tokens(text):
    pattern=r'//[^\n]*|/\*[\s\S]*?\*/|@"(?:[^"]|"")*"|"(?:\\.|[^"\\])*"|[A-Za-z_][A-Za-z_0-9]*|\d+(?:\.\d+)?[fFdDmMuUlL]*|==|!=|<=|>=|&&|\|\||\+\+|--|[^\s]'
    return [(m.group(),m.start(),m.end()) for m in re.finditer(pattern,text)
            if not m.group().startswith(('//','/*'))]


def _replace_exact(text,before,after):
    t=_tokens(text);wanted=[x[0] for x in _tokens(before)]
    hits=[i for i in range(len(t)-len(wanted)+1) if [x[0] for x in t[i:i+len(wanted)]]==wanted]
    if len(hits)!=1:raise ValueError('reviewed-template-not-unique:'+before)
    i=hits[0];return text[:t[i][1]]+after+text[t[i+len(wanted)-1][2]:]


def expand_training_array(text):
    """Actual C4 first region: local array allocation BEFORE element stores."""
    return _replace_exact(text,
        'Vector[] array = new Vector[2] { new Vector(), new Vector() };',
        'Vector[] array = new Vector[2]; array[0] = new Vector(); array[1] = new Vector();')


MAP_ASSIGNMENTS=[
    ('COAT_CONNER[0][0]', 'array[0]'),
    ('COAT_CONNER[0][1]', 'array[1]'),
    ('COAT_CONNER[1][1]', '(MAPCHIP_COAT_WNUM * 30 / 2)'),
    ('COAT_GOAL1[0][1]', '(MAPCHIP_COAT_WNUM * 30 / 2 / 4)'),
    ('COAT_GOAL1[1][1]', '(MAPCHIP_COAT_WNUM * 3 * 30 / 2 / 4)'),
    ('COAT_GOAL1_IN[0][1]', '(MAPCHIP_COAT_WNUM * 15 * 3 / 8)'),
    ('COAT_GOAL1_IN[1][1]', '(MAPCHIP_COAT_WNUM * 15 * 5 / 8)'),
    ('GOAL1[0][1]', '(MAPCHIP_COAT_WNUM * 30 / 2 / 2)'),
]


def inverse_int_negation(expression,primitive_type,checked_context=False):
    # Map operands are proven Int32 from the original CLI array/field types.
    # This is deliberately not generalized to Single/Double or custom operators.
    if primitive_type!='System.Int32' or checked_context:
        raise ValueError('requires-proven-unchecked-Int32')
    if not expression or re.search(r'\+\+|--|=|\b(?:new|await|yield|throw)\b',expression):
        raise ValueError('unsupported-operand-effects')
    return '(0 - '+expression+')'


def restore_map_zero_subtractions(text):
    for lhs,operand in MAP_ASSIGNMENTS:
        tail='' if operand.startswith('array[') else ' - array[1]'
        before=lhs+' = -'+operand+tail+';'
        after=lhs+' = '+inverse_int_negation(operand,'System.Int32')+tail+';'
        text=_replace_exact(text,before,after)
    return text


_MIRROR={'<':'>','<=':'>=','>':'<','>=':'<=','==':'==','!=':'!='}
_PRIMITIVES={'System.Int32','System.UInt32','System.Int64','System.UInt64','System.Single','System.Double'}


def mirror_comparison(left,operator,right,primitive_type,negated=False):
    """Fixture-only typed mold: swap operands and mirror, never complement.

    Accept bare, distinct parameter/local identifiers only. The caller must
    establish the primitive type; this API does not guess property purity.
    """
    if primitive_type not in _PRIMITIVES:raise ValueError('unproven-or-custom-comparison-type')
    if operator not in _MIRROR:raise ValueError('unsupported-comparison-operator')
    if not all(re.fullmatch(r'[A-Za-z_][A-Za-z_0-9]*',v) for v in [left,right]):
        raise ValueError('requires-pure-parameter-or-local-identifiers')
    if left==right:raise ValueError('identical-operands-not-diagnostic')
    result='('+right+' '+_MIRROR[operator]+' '+left+')'
    return '!'+result if negated else result


def fixture_source(text):
    text=expand_training_array(text)
    text=restore_map_zero_subtractions(text)
    text=_replace_exact(text,'sink = local = Read();','local = Read(); sink = local;')
    for name,ctype,op,negated in [('LtFloat','System.Single','<',False),('NotLtFloat','System.Single','<',True),
                                  ('LeDouble','System.Double','<=',False),('GeDouble','System.Double','>=',False),
                                  ('GtInt','System.Int32','>',False),('EqFloat','System.Single','==',False),
                                  ('NeDouble','System.Double','!=',False)]:
        marker='public static int '+name+'('
        first=text.find(marker)
        if first<0:raise ValueError('missing-fixture-method:'+name)
        end=text.find('\n    }',first)
        if end<0:raise ValueError('missing-fixture-method-close:'+name)
        part=text[first:end]
        before='if ('+('!(a '+op+' b)' if negated else 'a '+op+' b')+')'
        after='if ('+mirror_comparison('a',op,'b',ctype,negated)+')'
        changed=_replace_exact(part,before,after)
        text=text[:first]+changed+text[end:]
    return text
