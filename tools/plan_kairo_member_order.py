"""Recover a constrained merge of metadata member orders and Roslyn ordinals.

Writes isolated source candidates only. A satisfiable merge is not accepted
until complete compilation, generated-name checks and regression checks pass.
"""
from collections import defaultdict
import json
import hashlib
from pathlib import Path
import re
import time

ROOT=Path(__file__).resolve().parents[1]


def load_constraints(source=None):
    document=json.loads((ROOT/'config/generated-ordinals.json').read_text(encoding='utf-8'))
    oracle=ROOT/'original/extracted/assets/bin/Data/Managed/KairoLibrary.dll'
    if hashlib.sha256(oracle.read_bytes()).hexdigest()!=document['original_sha256']:
        raise ValueError('Ordinal constraints belong to a different original assembly')
    rows=[dict(row) for row in document['constraints']]
    if source is not None:
        slots={key:m['roslyn_slot'] for f in source['sources'] for t in f['types'] for m in t['members'] for key in m['method_ids']}
        for row in rows:
            row['current_slot']=slots.get(row['id'])
            row['difference']=None if row['id'] not in slots else slots[row['id']]-row['original_slot_candidates'][0]
    return rows


def merge_members(members,original_type,constraints):
    count=len(members);predecessors=[set() for _ in members]
    method_nodes={key:index for index,m in enumerate(members) for key in m['method_ids']}
    field_nodes={name:index for index,m in enumerate(members) for name in (m['name'] or '').split(',') if m['kind']=='FieldDeclaration'}
    property_nodes={m['name']:index for index,m in enumerate(members) if m['kind'] in ('PropertyDeclaration','IndexerDeclaration')}
    nested_nodes={m['name']:index for index,m in enumerate(members) if m['kind'] in ('ClassDeclaration','StructDeclaration','InterfaceDeclaration','DelegateDeclaration','EnumDeclaration')}
    for field in original_type['fields']:
        match=re.match(r'<(.+)>k__BackingField$',field['name'])
        if match and match[1] in property_nodes:field_nodes[field['name']]=property_nodes[match[1]]
    sequences=[
        [method_nodes[key] for key in original_type['methods'] if key in method_nodes],
        [field_nodes[f['name']] for f in original_type['fields'] if f['name'] in field_nodes],
        [property_nodes[p['name']] for p in original_type['properties'] if p['name'] in property_nodes],
        [nested_nodes[n.split('/')[-1]] for n in original_type['nested_types'] if n.split('/')[-1] in nested_nodes]]
    for sequence in sequences:
        unique=[]
        for node in sequence:
            if not unique or node!=unique[-1]:unique.append(node)
        for left,right in zip(unique,unique[1:]):predecessors[right].add(left)
    targets={}
    for constraint in constraints:
        if len(constraint['original_slot_candidates'])!=1:raise ValueError('Ambiguous generated ordinal')
        node=method_nodes[constraint['id']];value=constraint['original_slot_candidates'][0]
        if node in targets and targets[node]!=value:raise ValueError('Conflicting ordinal constraints')
        targets[node]=value
    bits=[sum(1<<x for x in group) for group in predecessors]
    full=(1<<count)-1;failed=set();started=time.monotonic();visits=0

    def solve(mask,slot):
        nonlocal visits
        visits+=1
        if visits>400000 or time.monotonic()-started>12:raise TimeoutError('Constrained merge search limit')
        if mask==full:return []
        if mask in failed:return None
        upcoming=[(value,node) for node,value in targets.items() if not mask>>node&1]
        if upcoming and min(upcoming)[0]<slot:failed.add(mask);return None
        boundary=min(upcoming)[0] if upcoming else None
        available=[i for i in range(count) if not mask>>i&1 and bits[i]&mask==bits[i]]
        for node in available:
            weight=members[node]['weight']
            if node in targets and targets[node]!=slot:continue
            if boundary is not None and slot+weight>boundary and targets.get(node)!=slot:continue
            result=solve(mask|1<<node,slot+weight)
            if result is not None:return [node]+result
        failed.add(mask);return None
    sequence=solve(0,0)
    if sequence is None:raise ValueError('No merge satisfies metadata orders and source ordinal constraints')
    return sequence,visits


def main():
    source=json.loads((ROOT/'analysis/kairo-source-members.json').read_text())
    original=json.loads((ROOT/'build/KairoLibrary-roslyn-1.3.2/original-methods.json').read_text(encoding='utf-8-sig'))
    original_types={t['name']:t for t in original['types']}
    constraints=load_constraints(source)
    by_type=defaultdict(list)
    for row in constraints:by_type[row['id'].split('::',1)[0]].append(row)
    reports=[];source_map={}
    folder=ROOT/'analysis/variants/kairo-member-order'
    for file in source['sources']:
        edits=[];path=Path(file['path']);text=file['text'];encoded=text.encode('utf-16-le')
        for type in file['types']:
            rows=by_type[type['name']]
            if not any(r['difference'] for r in rows):continue
            record={'type':type['name'],'constraints':rows}
            try:
                sequence,visits=merge_members(type['members'],original_types[type['name']],rows)
                members=type['members'];start=members[0]['full_start'];end=members[-1]['full_end']
                replacement=b''.join(encoded[members[i]['full_start']*2:members[i]['full_end']*2] for i in sequence)
                edits.append((start*2,end*2,replacement));record.update({'status':'CANDIDATE','permutation':sequence,'search_visits':visits})
            except (ValueError,TimeoutError) as error:record.update({'status':'UNRESOLVED','reason':str(error)})
            reports.append(record)
        if edits:
            for start,end,replacement in sorted(edits,reverse=True):encoded=encoded[:start]+replacement+encoded[end:]
            relative=path.relative_to(ROOT);output=folder/relative;output.parent.mkdir(parents=True,exist_ok=True)
            output.write_bytes(encoded.decode('utf-16-le').encode('utf-8'));source_map[relative.as_posix()]=str(output)
    (ROOT/'analysis/kairo-order-source-map.json').write_text(json.dumps(source_map,indent=2)+'\n')
    (ROOT/'reports/KairoLibrary-member-order-plan.json').write_text(json.dumps({'types':reports,'source_files':len(source_map),'applied_to_main':False},indent=2)+'\n',encoding='utf-8')
    print(json.dumps({'source_files':len(source_map),'types':[{'type':r['type'],'status':r['status'],'reason':r.get('reason')} for r in reports]}))


if __name__=='__main__':main()
