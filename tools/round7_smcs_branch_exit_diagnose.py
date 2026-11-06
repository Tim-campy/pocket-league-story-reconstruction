"""Read measured br=>ret targets and classify evidence without modifying source."""
import json
from pathlib import Path
from collections import Counter
from round6_slot_solver import local_access

ROOT=Path(__file__).resolve().parents[1]


def branch_exit_hints(original, offset):
    instructions=original.get('instructions',[])
    at=next((i for i in instructions if i['offset']==offset),None)
    if not at or at['opcode'] not in ('br','br.s') or not isinstance(at.get('operand'),dict):
        return {}
    target=at['operand'].get('branch')
    index=next((n for n,i in enumerate(instructions) if i['offset']==target),None)
    if index is None:
        return {}
    tail=instructions[index:index+4]
    if tail[0]['opcode']=='ret':
        return {'original_exit_target_kind':'method-ret','original_exit_target_offset':target}
    if len(tail)==4 and local_access(tail[0]) and local_access(tail[0])[0]=='ldloc' \
        and tail[1]['opcode']=='ldc.i4.1' and tail[2]['opcode']=='add' \
        and local_access(tail[3])==('stloc',local_access(tail[0])[1]):
        return {'original_exit_target_kind':'loop-increment','original_exit_target_offset':target,
                'original_exit_counter_slot':local_access(tail[0])[1]}
    return {'original_exit_target_kind':'UNKNOWN','original_exit_target_offset':target}


def main():
    measured=ROOT / 'reports/round7-part-a-after-measurement.json'
    data=json.loads(measured.read_text(encoding='utf-8-sig'))['assemblies']['Assembly-CSharp']
    original={m['stable_id']:m for m in json.loads((ROOT / data['provenance']['original_dump']).read_text(encoding='utf-8-sig'))['selected_methods']}
    rows=[]
    for pair in data['alignment']['methods']:
        if pair['opcode_pair']!='br => ret':continue
        method=original[pair['stable_id']]
        hints=branch_exit_hints(method,pair['original_offset'])
        target=hints.get('original_exit_target_offset')
        rows.append({'id':pair['stable_id'],'original_il_bytes':pair['original_il_bytes'],
            'original_divergence_offset':pair['original_offset'],'rebuilt_divergence_offset':pair['rebuilt_offset'],
            'hints':hints,'target_instructions':[i for i in method['instructions'] if target<=i['offset']<=target+14],
            'original_context':pair['original_context'],'rebuilt_context':pair['rebuilt_context'],
            'original_ret_offsets':[i['offset'] for i in method['instructions'] if i['opcode']=='ret']})
    kinds=Counter(r['hints'].get('original_exit_target_kind','UNKNOWN') for r in rows)
    report={'operation':'diagnostic-only-original-branch-target-evidence','measurement':str(measured.relative_to(ROOT)),
        'provenance':data['provenance'],'groups':{kind:{'methods':count,
            'original_il_bytes':sum(r['original_il_bytes'] for r in rows if r['hints'].get('original_exit_target_kind')==kind)} for kind,count in kinds.items()},'methods':rows}
    (ROOT / 'reports/round7-smcs-branch-exit-diagnosis.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(json.dumps(report['groups'],indent=2))


if __name__=='__main__':main()
