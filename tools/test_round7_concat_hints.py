"""Synthetic negative controls plus the original _draw5 diagnostic contexts."""
import json
from pathlib import Path

from round7_concat_hints import original_concat_hints

ROOT=Path(__file__).resolve().parents[1]
STRING='class System.String @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e'


def signature(parts):
    return STRING+'::Concat``0('+','.join(parts)+')->'+STRING+';this=False;explicit=False;cc=Default'


def call(offset,parts):
    return {'offset':offset,'opcode':'call','operand':{'method':signature(parts)}}


def main():
    good={'instructions':[{'offset':0,'opcode':'ldstr','operand':'\t'},call(7,[STRING]*4)]}
    assert original_concat_hints(good,0)['original_string_concat_call_offset']==7
    bad=[]
    for parts in ([STRING]*3,[STRING]*5,[STRING+'[]'],[STRING,STRING,STRING,STRING.replace('System.String','System.Object')]):
        bad.append({'instructions':[call(7,parts)]})
    for changed in (signature([STRING]*4).replace(';this=False',';this=True'),
                    signature([STRING]*4).replace('::Concat``0','::Concat``1'),
                    'System.String::Concat(System.String,System.String,System.String,System.String)->System.String'):
        bad.append({'instructions':[{'offset':7,'opcode':'call','operand':{'method':changed}}]})
    bad.extend([
        {'instructions':[call(7,[STRING]*3),call(12,[STRING]*4)]},
        {'instructions':[{'offset':3,'opcode':'brtrue.s','operand':{'branch':7}},call(7,[STRING]*4)]},
        {'instructions':[{'offset':3,'opcode':'ret','operand':None},call(7,[STRING]*4)]},
        {'instructions':[dict(call(7,[STRING]*4),opcode='callvirt')]},
        {'instructions':[{'offset':3,'opcode':'call','operand':None},call(7,[STRING]*4)]},
        {},None,
    ])
    assert all(not original_concat_hints(method,0) for method in bad)
    assert not original_concat_hints(good,-1)
    assert not original_concat_hints(good,True)
    assert not original_concat_hints(good,8)
    baseline=ROOT/'build/round7/baseline/Assembly-CSharp/original-methods.json'
    rows=json.loads(baseline.read_text(encoding='utf-8-sig'))['selected_methods']
    method=next(m for m in rows if m['stable_id']=='form.SubForm::_draw5(kairo.unity.ui.Graphics)->System.Void')
    assert original_concat_hints(method,1391)['original_string_concat_call_offset']==1408
    assert original_concat_hints(method,3380)['original_string_concat_call_offset']==3397
    assert not original_concat_hints(method,1335)
    result={'operation':'pure-original-concat-hint-controls','synthetic_negative_cases':len(bad)+3,
            'positive_contexts':3,'neighbor_concat3_rejected':True,'game_compilations':0}
    (ROOT/'reports/round7-string-concat-hints-tests.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8',newline='\n')
    print(json.dumps(result))


if __name__=='__main__':main()
