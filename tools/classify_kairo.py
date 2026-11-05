"""Group actual IL edit patterns, without changing source or declaring causes."""
from collections import Counter,defaultdict
import csv
import difflib
import hashlib
import json
from pathlib import Path
import re
from compare_kairo import differences,patterns

ROOT=Path(__file__).resolve().parents[1]


def operation(i): return json.dumps([i['opcode'],i['operand']],sort_keys=True)


def classify(method,candidate):
    if candidate is None:return 'missing_generated_identity',[],[]
    old=method.get('instructions',[]);new=candidate.get('instructions',[])
    edits=[];old_changed=[];new_changed=[]
    for tag,a,b,c,d in difflib.SequenceMatcher(None,list(map(operation,old)),list(map(operation,new)),autojunk=False).get_opcodes():
        if tag!='equal':
            edits.append({'original_opcodes':[i['opcode'] for i in old[a:b]],'rebuilt_opcodes':[i['opcode'] for i in new[c:d]]})
            old_changed+=old[a:b];new_changed+=new[c:d]
    changed=old_changed+new_changed;ops={i['opcode'] for i in changed}
    text=json.dumps(changed);all_text=json.dumps([old,new])
    same_opcode_sequence=[i['opcode'] for i in old]==[i['opcode'] for i in new]
    generated_operands=any(isinstance(i['operand'],dict) and any(re.search(r'/<|::<',str(v)) for v in i['operand'].values()) for i in changed)
    if same_opcode_sequence and generated_operands:
        group='generated_names_or_delegate_targets'
    elif same_opcode_sequence and all(i['opcode'] in ('call','callvirt') for i in changed):
        group='call_target_or_overload'
    elif ops<= {'call','callvirt'} and ops:
        group='call_vs_callvirt'
    elif ops and all(op.startswith(('br','beq','bne','bgt','bge','blt','ble','leave')) or op in ('ret','ldnull','ldc.i4.0','ldc.i4.1','box') for op in ops):
        group='branch_and_early_return_layout'
    elif 'switch' in ops:
        group='switch_lowering'
    elif 'System.String' in text and '::Concat' in text:
        group='string_concatenation_lowering'
    elif 'System.Threading.Monitor' in all_text:
        group='lock_lowering'
    elif method.get('exception_regions')!=candidate.get('exception_regions') or any('finally' in str(x) for x in (method.get('exception_regions'),candidate.get('exception_regions'))):
        group='exception_using_or_finally_layout'
    elif 'DisplayClass' in text or 'ldftn' in ops or '/<>c' in text:
        group='closure_or_delegate_lowering'
    elif method.get('locals')!=candidate.get('locals'):
        group='locals_and_expression_order'
    elif any(op in ops for op in ('add','sub','mul','div','conv.i4','conv.i8','conv.r4','conv.r8')):
        group='arithmetic_conversions_or_redundant_expressions'
    else:group='other_instruction_or_operand_changes'
    return group,edits,patterns(method,candidate)


def main():
    folder=ROOT/'build/KairoLibrary-roslyn-1.3.2'
    original=json.loads((folder/'original-methods.json').read_text(encoding='utf-8-sig'))['selected_methods']
    rebuilt={m['stable_id']:m for m in json.loads((folder/'rebuilt-methods.json').read_text(encoding='utf-8-sig'))['selected_methods']}
    with (ROOT/'reports/KairoLibrary-compiler-comparison.csv').open(encoding='utf-8',newline='') as file:
        cross={r['stable_id']:r for r in csv.DictReader(file)}
    counts=Counter();cross_counts=defaultdict(Counter);rows=[];fingerprints=defaultdict(list)
    for method in original:
        candidate=rebuilt.get(method['stable_id']);delta,_=differences(method,candidate)
        if 'raw_body_hex' not in method or not delta:continue
        group,edits,traits=classify(method,candidate);counts[group]+=1
        status=cross[method['stable_id']]['classification'];cross_counts[group][status]+=1
        fingerprint=json.dumps(edits,sort_keys=True);short=hashlib.sha256(fingerprint.encode()).hexdigest()[:12]
        fingerprints[(group,short)].append(method['stable_id'])
        rows.append({'id':cross[method['stable_id']]['id'],'stable_id':method['stable_id'],'group':group,
            'compiler_comparison':status,'difference_facets':delta,'edit_fingerprint':short,'edits':edits,'traits':traits})
    report={'scope':str(sum(counts.values()))+' original method bodies differing or missing with package 1.3.2; disjoint primary groups',
        'counts':dict(counts),'cross_compiler_by_group':{k:dict(v) for k,v in cross_counts.items()},
        'declarations_without_body':76,'body_equal_but_generated_ownership_pending':
            json.loads((ROOT/'reports/KairoLibrary-verification.json').read_text())['counts']['equal_body_held_for_generated_group'],
        'diagnostic_only':True,'causes_proven':False,'methods':rows,
        'repeated_edit_patterns':[{'group':group,'fingerprint':fp,'count':len(ids),'methods':ids}
            for (group,fp),ids in sorted(fingerprints.items(),key=lambda p:(-len(p[1]),p[0])) if len(ids)>1]}
    (ROOT/'reports/KairoLibrary-failure-groups.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    with (ROOT/'reports/KairoLibrary-failure-groups.csv').open('w',encoding='utf-8',newline='') as file:
        columns=['id','group','compiler_comparison','difference_facets','edit_fingerprint','traits']
        writer=csv.DictWriter(file,fieldnames=columns,lineterminator='\n');writer.writeheader()
        for row in rows:writer.writerow({k:';'.join(row[k]) if isinstance(row[k],list) else row[k] for k in columns})
    print(json.dumps({'counts':dict(counts),'total':sum(counts.values()),'cross_compiler':report['cross_compiler_by_group']}))


if __name__=='__main__': main()
