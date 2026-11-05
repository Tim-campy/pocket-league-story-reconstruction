"""Restore proven original member ordinals in an isolated Kairo source variant.

Uses semantic member identities and the same constrained metadata-order merge as
the validated main recovery. It only writes analysis/variants/<label>-ordered/;
never edits src/, the oracle, compiler outputs or the method ledger. Output is
an unverified source candidate requiring complete compilation and comparison.
"""
from collections import defaultdict
import argparse
import copy
import hashlib
import json
from pathlib import Path
import re
import subprocess
from compiler_probes import UNITY,ROSLYN
from plan_kairo_member_order import merge_members,load_constraints

ROOT=Path(__file__).resolve().parents[1]


def digest(path):return hashlib.sha256(path.read_bytes()).hexdigest()


def parse_sources(selected,folder,label):
    vs=next(p for p in ROSLYN.parents if (p/'dotnet').is_dir())
    dotnet=vs/'dotnet/net10.0/runtime/dotnet.exe'
    driver=ROOT/'build/source-recovery/SourceRecovery.dll'
    if not driver.is_file():raise FileNotFoundError('Run python tools/prepare_source_recovery.py first')
    profile=UNITY/'Editor/Data/Mono/lib/mono/2.0'
    references=[profile/'mscorlib.dll',profile/'System.Core.dll',
        ROOT/'original/extracted/assets/bin/Data/Managed/UnityEngine.dll',profile/'System.dll']
    manifest=folder/(label+'-parse-input.json');output=folder/(label+'-members.json')
    manifest.write_text(json.dumps({'sources':[str(path) for path in selected.values()],
        'references':[str(path) for path in references]},indent=2)+'\n',encoding='utf-8')
    with output.open('wb') as file:
        result=subprocess.run([str(dotnet),str(driver),'members',str(manifest)],stdout=file,stderr=subprocess.PIPE)
    if result.returncode:raise RuntimeError(result.stderr.decode('utf-8',errors='replace')[-4000:])
    document=json.loads(output.read_text(encoding='utf-8-sig'))
    if document['errors']:raise ValueError('Variant has semantic errors; repair before ordering: '+json.dumps(document['errors']))
    return document


def constructor_candidate(type_record,original_type,original_methods,text):
    constructor=type_record['name']+'::.ctor()->System.Void'
    members=copy.deepcopy(type_record['members'])
    if not original_type['methods'] or original_type['methods'][0]!=constructor:return members,False
    if any(constructor in member['method_ids'] for member in members):return members,False
    definition=original_methods[constructor]
    access={1:'private',2:None,3:'internal',4:'protected',5:'protected internal',6:'public'}[definition['method_attributes']&7]
    if access is None:raise ValueError('Historical constructor access is unavailable in C# 6')
    name=type_record['name'].split('/')[-1].rsplit('.',1)[-1].split('`')[0]
    newline='\r\n' if '\r\n' in text else '\n'
    declaration=newline+'\t'+access+' @'+name+'()'+newline+'\t{'+newline+'\t}'+newline
    members.insert(0,{'kind':'ConstructorDeclaration','weight':1,'name':name,'method_ids':[constructor],
        'full_start':None,'full_end':None,'synthetic_source':declaration})
    return members,True


def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--source-map',type=Path,required=True)
    parser.add_argument('--label',required=True)
    args=parser.parse_args()
    if not re.fullmatch(r'[A-Za-z0-9][A-Za-z0-9_.-]*',args.label):raise ValueError('Invalid label')
    folder=(ROOT/'analysis/variants'/(args.label+'-ordered')).resolve()
    if not folder.is_relative_to((ROOT/'analysis/variants').resolve()):raise ValueError('Output escaped analysis/variants')
    folder.mkdir(parents=True,exist_ok=True)
    order=[p for p in (ROOT/'config/compile-order/KairoLibrary.txt').read_text(encoding='utf-8').splitlines() if p and not p.startswith('#')]
    if 'src/KairoLibrary/AssemblyInfo.cs' not in order:order.append('src/KairoLibrary/AssemblyInfo.cs')
    replacements=json.loads(args.source_map.read_text(encoding='utf-8-sig'))
    if not isinstance(replacements,dict) or set(replacements)-set(order):raise ValueError('Unknown logical source in map')
    selected={logical:Path(replacements[logical]).resolve() if logical in replacements else ROOT/logical for logical in order}
    if any(not path.is_relative_to(ROOT) or path.is_relative_to(ROOT/'original') for path in selected.values()):
        raise ValueError('Source inputs must stay inside workspace and outside original/')
    if len(set(selected.values()))!=len(selected):raise ValueError('Each logical source needs a unique input file')
    hashes={logical:digest(path) for logical,path in selected.items()}
    semantic=parse_sources(selected,folder,'input')
    logical_of={str(path):logical for logical,path in selected.items()}
    oracle=json.loads((ROOT/'build/KairoLibrary-roslyn-1.3.2/original-methods.json').read_text(encoding='utf-8-sig'))
    original_types={t['name']:t for t in oracle['types']}
    original_methods={m['stable_id']:m for m in oracle['selected_methods']}
    constraints=load_constraints()
    by_type=defaultdict(list)
    for row in constraints:by_type[row['id'].split('::',1)[0]].append(row)
    mapping={logical:str(path) for logical,path in selected.items() if logical in replacements}
    reports=[];rewritten=0;constructors=0;unresolved=0
    for file in semantic['sources']:
        logical=logical_of[file['path']];text=file['text'];encoded=text.encode('utf-16-le');edits=[]
        if selected[logical].read_bytes().decode('utf-8-sig')!=text:raise ValueError('Input changed since parsing: '+logical)
        for record in file['types']:
            rows=by_type.get(record['name'],[])
            if not rows:continue
            slots={key:member['roslyn_slot'] for member in record['members'] for key in member['method_ids']}
            rows=[dict(row,current_slot=slots.get(row['id']),difference=None if row['id'] not in slots else
                slots[row['id']]-row['original_slot_candidates'][0]) for row in rows]
            if all(row['difference']==0 for row in rows):continue
            report={'type':record['name'],'source_path':logical,'constraints':rows}
            try:
                absent=[row['id'] for row in rows if row['current_slot'] is None]
                if absent:raise ValueError('Constrained original method missing from variant: '+','.join(absent))
                members,inserted=constructor_candidate(record,original_types[record['name']],original_methods,text)
                sequence,visits=merge_members(members,original_types[record['name']],rows)
                chunks=[];slot=0;new_slots={}
                for index in sequence:
                    member=members[index]
                    chunks.append(member['synthetic_source'].encode('utf-16-le') if 'synthetic_source' in member else
                        encoded[member['full_start']*2:member['full_end']*2])
                    for key in member['method_ids']:new_slots[key]=slot
                    slot+=member['weight']
                if any(new_slots[row['id']]!=row['original_slot_candidates'][0] for row in rows):
                    raise ValueError('Merge did not satisfy every original ordinal')
                original_members=record['members']
                if not original_members:raise ValueError('Cannot replace an empty member span')
                start=original_members[0]['full_start']*2;end=original_members[-1]['full_end']*2
                if any(start<old_end and old_start<end for old_start,old_end,_ in edits):
                    raise ValueError('Overlapping nested edits require a separate pass')
                edits.append((start,end,b''.join(chunks)));constructors+=inserted
                report.update({'status':'ISOLATED_ORDER_CANDIDATE','constructor_restored':inserted,
                    'search_visits':visits,'permutation':sequence,
                    'candidate_slots':{row['id']:new_slots[row['id']] for row in rows}})
            except (ValueError,KeyError,TimeoutError) as error:
                unresolved+=1;report.update({'status':'UNRESOLVED','reason':str(error)})
            reports.append(report)
        if edits:
            for start,end,replacement in sorted(edits,reverse=True):encoded=encoded[:start]+replacement+encoded[end:]
            destination=folder/'sources'/logical;destination.parent.mkdir(parents=True,exist_ok=True)
            destination.write_bytes(encoded.decode('utf-16-le').encode('utf-8'))
            mapping[logical]=str(destination);selected[logical]=destination;rewritten+=1
    for logical,path in selected.items():
        if logical not in mapping and digest(path)!=hashes[logical]:raise ValueError('Fallback main source changed during planning')
    output_map=folder/'source-map.json';output_map.write_text(json.dumps(mapping,indent=2)+'\n',encoding='utf-8',newline='\n')
    report={'label':args.label,'input_source_map':str(args.source_map.resolve()),'output_source_map':str(output_map),
        'reordered_source_files':rewritten,'constructors_restored':constructors,'unresolved_types':unresolved,
        'types':reports,'full_compilation_verified':False,'applied_to_main':False,'ledger_updated':False}
    (folder/'ordering-report.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8',newline='\n')
    # Parse the final candidate again: verify actual semantic ordinals instead
    # of trusting only the arithmetic used by the search.
    final=parse_sources(selected,folder,'ordered')
    actual={record['name']:{key:member['roslyn_slot'] for member in record['members'] for key in member['method_ids']}
        for file in final['sources'] for record in file['types']}
    for item in reports:
        if item['status']!='ISOLATED_ORDER_CANDIDATE':continue
        for row in item['constraints']:
            if actual[item['type']].get(row['id'])!=row['original_slot_candidates'][0]:
                raise ValueError('Reparsed candidate ordinal differs from original constraint')
    report['final_semantic_ordinals_verified']=True
    report['constraints_checked']=len(constraints)
    report['remaining_unaligned_constraints']=[{'id':row['id'],'expected':row['original_slot_candidates'],
        'actual':actual.get(row['id'].split('::',1)[0],{}).get(row['id'])} for row in constraints
        if actual.get(row['id'].split('::',1)[0],{}).get(row['id']) not in row['original_slot_candidates']]
    (folder/'ordering-report.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8',newline='\n')
    print(json.dumps({key:value for key,value in report.items() if key!='types'}))


if __name__=='__main__':main()
