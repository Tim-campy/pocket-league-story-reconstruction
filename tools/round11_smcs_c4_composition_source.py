"""C4 array store plus proven existing-slot dispatch; no extra local/RNG call."""
from calculator import _tokens,_replace_exact,expand_training_array

def training_dispatch(text):
    # The original stores both branches into EXISTING slot40 (named num23).
    # The statement and both payloads are exact reviewed templates. Values other
    # than0/1 still skip both arms, matching switch rather than assuming RNG range.
    before='''switch ((num25 >= num26) ? GameUtil.Random(2) : 0) {
case 0: tAreas_[num24][0].tGroupAreas_[0].tPlayerList_.AddElement(obj4); break;
case 1: tAreas_[num24][1].tGroupAreas_[0].tPlayerList_.AddElement(obj4); break;
}'''
    after='''if (num25 < num26) { num23 = 0; } else { num23 = GameUtil.Random(2); }
if (num23 == 0) { tAreas_[num24][0].tGroupAreas_[0].tPlayerList_.AddElement(obj4); }
else if (num23 == 1) { tAreas_[num24][1].tGroupAreas_[0].tPlayerList_.AddElement(obj4); }'''
    # Explicit existing Int32 declaration is part of the source contract.
    tokens=[t[0] for t in _tokens(text)]
    if sum(tokens[i:i+5]==['int','num23','=','0',';'] for i in range(len(tokens)-4))!=1:
        raise ValueError('existing-slot40-Int32-declaration-not-unique')
    return _replace_exact(text,before,after)

def transform_training_composed(text):
    return training_dispatch(expand_training_array(text))
