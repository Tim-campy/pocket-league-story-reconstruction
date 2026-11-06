"""Recover unreferenced smcs local declarations from a proved signature suffix.

Only source declarations are proposed. Existing instructions/header must already
be identical; each missing original slot must have zero IL accesses. Every
proposal then needs an exact complete source-model forecast and the compiler.
"""
import argparse
import json
from pathlib import Path
import re
from compare_kairo import sha
from round7_slot_model import SlotModelClient
from round7_slot_solver import local_access

ROOT=Path(__file__).resolve().parents[1]


def read(path):return json.loads(Path(path).read_text(encoding='utf-8-sig'))
def cut(text,start,end):return text.encode('utf-16-le')[start*2:end*2].decode('utf-16-le')
def insert(text,point,value):
    raw=text.encode('utf-16-le')
    return (raw[:point*2]+value.encode('utf-16-le')+raw[point*2:]).decode('utf-16-le')


def calculate(text,model,original,compiled,expected,current):
    result={'eligible':False,'reasons':[],'patches':[],'names_are_historical':False}
    if not model.get('complete') or model.get('predicted_types')!=current:
        result['reasons'].append('source-model-not-complete-and-actual')
        return result
    if original['instructions']!=compiled['instructions'] or original['header']!=compiled['header']:
        result['reasons'].append('instructions-or-header-not-already-identical')
        return result
    if len(current)>=len(expected) or current!=expected[:len(current)]:
        result['reasons'].append('missing-slots-are-not-a-signature-suffix')
        return result
    used={access[1] for instruction in original['instructions']
          if (access:=local_access(instruction)) is not None}
    slots=list(range(len(current),len(expected)))
    if any(slot in used for slot in slots):
        result['reasons'].append('missing-original-slot-has-an-IL-access')
        return result
    variables=[v for v in model['variables'] if v['kind']=='declared' and v.get('statement_end',-1)>=0]
    if not variables:
        result['reasons'].append('no-existing-declaration-anchor')
        return result
    point=max(v['statement_end'] for v in variables)
    before_line=text.encode('utf-16-le')[:point*2].decode('utf-16-le').rsplit('\n',1)[-1]
    indent=re.match(r'[ \t]*',before_line).group(0)
    newline='\r\n' if '\r\n' in text else '\n'
    declarations=[]
    for slot in slots:
        type_name=expected[slot].replace('/','.')
        if not re.fullmatch(r'[A-Za-z_]\w*(?:\.[A-Za-z_]\w*)*(?:\[,*\])*',type_name):
            result['reasons'].append('unused-suffix-type-not-expressible-by-this-calculator')
            return result
        name='recoveredUnusedSlot'+str(slot)
        while re.search(r'\b'+re.escape(name)+r'\b',text):name+='Local'
        declarations.append('global::'+type_name+' '+name+';')
    value=newline+indent+(newline+indent).join(declarations)
    candidate=insert(text,point,value)
    start,end=model['declaration_start'],model['declaration_end']
    extra=len(value.encode('utf-16-le'))//2
    patch={'start':start,'end':end,'before':cut(text,start,end),
           'after':cut(candidate,start,end+extra),'distance':0,
           'rule':'c9-control-skeleton-calculated-unused-local-suffix'}
    result.update(eligible=True,missing_slots=slots,unused_declarations=declarations,
                  original_missing_slot_accesses=0,patches=[patch])
    return result


def main():
    p=argparse.ArgumentParser(description=__doc__)
    p.add_argument('--snapshot',default='build/round9/cycles/b10/after')
    p.add_argument('--output',default='reports/round9-control-unused-suffix-patches.json')
    args=p.parse_args();base=ROOT/args.snapshot/'Assembly-CSharp'
    original={m['stable_id']:m for m in read(base/'original-methods.json')['selected_methods']}
    compiled={m['stable_id']:m for m in read(base/'rebuilt-methods.json')['selected_methods']}
    expected,current=read(base/'plain-local-types.json'),read(base/'plain-local-types-rebuilt.json')
    verified=read(base/'verification.json')['methods']
    manifest=read(base/'build-manifest.json')
    manifest['sources']=[dict(s,input_path=str(base/s['path'])) for s in manifest['sources']]
    folder=ROOT/'build/round9/unused-suffix';folder.mkdir(parents=True,exist_ok=True)
    mp=folder/'manifest.json';mp.write_text(json.dumps(manifest,indent=2)+'\n',encoding='utf-8')
    # Receipt source is optional; official ledger paths are identity-bound via it.
    import csv
    with (ROOT/args.snapshot/'methods.csv').open(encoding='utf-8-sig',newline='') as stream:
        ledger={r['id']:r for r in csv.DictReader(stream)}
    rows=[];patches=[]
    with SlotModelClient(mp) as client:
        for identity,proof in verified.items():
            key=proof['stable_id']
            if proof['state']=='matched' or not proof.get('body_present'):continue
            if original[key]['instructions']!=compiled.get(key,{}).get('instructions'):continue
            logical=ledger[identity]['fonte']
            if not logical:continue
            text=(base/logical).read_bytes().decode('utf-8-sig')
            prediction=client.predict(ROOT/logical,text,key)
            result=calculate(text,prediction,original[key],compiled[key],expected[key],current[key])
            row={k:v for k,v in result.items() if k!='patches'};row['id']=key
            for patch in result['patches']:
                raw=text.encode('utf-16-le')
                candidate=(raw[:patch['start']*2]+patch['after'].encode('utf-16-le')+raw[patch['end']*2:]).decode('utf-16-le')
                after=client.predict(ROOT/logical,candidate,key)
                approved=after['complete'] and after['predicted_types']==expected[key]
                row.update(candidate_prediction_complete=after['complete'],candidate_signature_exact=approved,
                           candidate_predicted_types=after['predicted_types'],original_types=expected[key])
                if approved:
                    patch.update(method_id=key,logical_path=logical,
                        model_source_sha256=sha(ROOT/'tools/Round7SlotModel.cs'),
                        client_sha256=sha(ROOT/'tools/round7_slot_model.py'))
                    patches.append(patch)
            rows.append(row)
    dependencies={str(path.relative_to(ROOT)):sha(path) for path in
        [Path(__file__),ROOT/'tools/round7_slot_model.py',ROOT/'tools/Round7SlotModel.cs',ROOT/'tools/round7_slot_solver.py']}
    report={'operation':'calculated-unused-local-signature-suffix-source-proposals',
            'snapshot':args.snapshot,'game_compilations':0,'adopted':False,
            'calculator_sha256':sha(Path(__file__)),'dependency_sha256':dependencies,
            'audit':rows,'patches':patches}
    (ROOT/args.output).write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8',newline='\n')
    print(json.dumps({'patches':len(patches),'audit':rows}))


if __name__=='__main__':main()
