"""Select body-exact isolated variants one source member at a time.

Every candidate must first match its target in an isolated complete build. Main
adoption then requires complete official Kairo verification and zero regressions.
Generated targets are changed through their originating C# member, never by
writing generated classes or patching emitted assemblies.
"""
from collections import defaultdict
import argparse
import hashlib
import json
from pathlib import Path
import re
import subprocess
import sys
import time
from compare_kairo import differences
from evaluate_kairo_variants import ROOT,VARIANTS,read,digest,parse_sources,selected_sources,method_index
from fix_kairo_callvirt import mask_non_code


def utf16_slice(text,start,end):return text.encode('utf-16-le')[start*2:end*2].decode('utf-16-le')


def body_span(code):
    masked=mask_non_code(code);start=masked.find('{')
    if start<0:return None
    depth=0
    for pos in range(start,len(masked)):
        depth+=(masked[pos]=='{')-(masked[pos]=='}')
        if depth==0:return start,pos+1
    raise ValueError('Unclosed source member')


def replacement(main_source,main_member,candidate_source,candidate_member):
    if main_member['kind']!=candidate_member['kind'] or main_member['weight']!=candidate_member['weight']:
        raise ValueError('Source member kind/slot weight differs')
    if main_member['method_ids']!=candidate_member['method_ids']:
        raise ValueError('Source member identities differ')
    main_code=utf16_slice(main_source,main_member['start'],main_member['end'])
    new_code=utf16_slice(candidate_source,candidate_member['start'],candidate_member['end'])
    # Keep declaration metadata/parameter spelling when both normal methods have
    # block bodies. Constructors/operators/properties retain their whole member
    # because initializer/accessor syntax contributes to emitted method bodies.
    old_body=body_span(main_code);new_body=body_span(new_code)
    if main_member['kind']=='MethodDeclaration' and old_body and new_body:
        a,b=old_body;c,d=new_body
        new_code=main_code[:a]+new_code[c:d]+main_code[b:]
    encoded=main_source.encode('utf-16-le')
    result=(encoded[:main_member['start']*2]+new_code.encode('utf-16-le')+encoded[main_member['end']*2:]).decode('utf-16-le')
    existing=set(re.findall(r'^using\s+[^;]+;',result,re.M))
    needed=set(re.findall(r'^using\s+[^;]+;',candidate_source,re.M))
    if ('using System;' in needed-existing and 'using UnityEngine;' in existing and
        re.search(r'\bObject\s*\.\s*Destroy\s*\(',mask_non_code(main_source)) and
        not any(re.match(r'using\s+Object\s*=',directive) for directive in existing)):
        # The original unqualified Object.Destroy bound to UnityEngine.Object.
        # Preserve that binding when a candidate brings System into scope.
        needed.add('using Object = UnityEngine.Object;')
    if needed-existing:result='\n'.join(sorted(needed-existing))+'\n'+result
    return result


def run_official():
    for attempt in range(3):
        result=subprocess.run([sys.executable,str(ROOT/'tools/verify_kairo.py')],cwd=ROOT,
            capture_output=True,text=True,encoding='utf-8',errors='replace')
        done=False
        for line in result.stdout.splitlines():
            try:summary=json.loads(line)
            except ValueError:continue
            done|=summary.get('assembly')=='KairoLibrary' and 'counts' in summary
        if done and result.returncode in (0,1):
            receipt=read(ROOT/'build/verification-KairoLibrary.json')
            if digest(ROOT/receipt['build_artifact'])!=receipt['build_sha256']:raise ValueError('Artifact changed')
            if any(digest(ROOT/s['path'])!=s['sha256'] for s in receipt['sources']):raise ValueError('Stale source receipt')
            return receipt
        if attempt<2:time.sleep(1)
    raise RuntimeError('Official verifier infrastructure failed: '+(result.stdout+result.stderr)[-3000:])


def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--apply',action='store_true');parser.add_argument('--only',help='Original stable ID substring')
    parser.add_argument('--skip-preflight',action='store_true',help='Use the full-variant proof, then run the official complete verifier once per attempted main edit')
    args=parser.parse_args()
    evaluation=read(ROOT/'reports/KairoLibrary-variant-evaluation.json')
    original=read(ROOT/'build/KairoLibrary-roslyn-1.3.2/original-methods.json')['selected_methods']
    oracle={m['stable_id']:m for m in original}
    initial=read(ROOT/'build/verification-KairoLibrary.json')
    import csv
    with (ROOT/'config/methods.csv').open(encoding='utf-8',newline='') as file:
        rows={r['token']:r for r in csv.DictReader(file) if r['assembly']=='KairoLibrary'}
    token=lambda m:'0x'+format(0x06000000|int(m['token'].split('0x')[1].rstrip(']'),16),'08x')
    ids={key:rows[token(m)]['id'] for key,m in oracle.items()};stable_of={v:k for k,v in ids.items()}
    variant_members={};source_documents={};options=defaultdict(list);unmapped=[]
    for label in ('expressions','control_flow','conservative','codemerx'):
        report=evaluation['variants'].get(label,{})
        if report.get('status')!='PASS':continue
        _,build_label=VARIANTS[label];folder=ROOT/'build'/('KairoLibrary-'+build_label)
        doc=read(folder/'source-members.json');mapping=read(ROOT/report['source_map']);selected=selected_sources(mapping)
        index=method_index(doc,{str(p):logical for logical,p in selected.items()})
        variant_members[label]=index
        source_documents[label]={str(Path(s['path']).relative_to(ROOT)):s['text'] for s in doc['sources']}
        for target in report['body_equal_ids']:
            if initial['methods'][ids[target]]['body_equal']:continue
            origins=[target] if target in index else []
            proof=initial['methods'][ids[target]].get('ownership_proof') or {}
            origins.extend(origin for origin in proof.get('origins',[]) if origin in index)
            configured=rows[token(oracle[target])]['gerado_por']
            if configured in stable_of and stable_of[configured] in index:origins.append(stable_of[configured])
            if not origins:
                match=re.search(r'<([^<>]+)>(?:b__|d__)',target)
                if match:
                    owner=target.split('/<',1)[0].split('::',1)[0]
                    origins=[key for key in index if key.startswith(owner+'::'+match.group(1)+'(') or key.startswith(owner+'::'+match.group(1)+'``')]
            if not origins:unmapped.append({'target':target,'variant':label});continue
            for origin in dict.fromkeys(origins):
                if not any(v==label and o==origin for v,o in options[target]):options[target].append((label,origin))
    queue=sorted(options,key=lambda key:(not ('KairoBase' in key and 'Start' in key),key))
    if args.only:queue=[key for key in queue if args.only in key]
    plan={'targets':len(queue),'variant_options':sum(len(options[k]) for k in queue),'unmapped':unmapped,
        'targets_and_origins':{key:[{'variant':v,'origin':o} for v,o in options[key]] for key in queue}}
    (ROOT/'reports/KairoLibrary-variant-selection-plan.json').write_text(json.dumps(plan,indent=2)+'\n',encoding='utf-8',newline='\n')
    print(json.dumps({'targets':len(queue),'options':plan['variant_options'],'apply':args.apply}),flush=True)
    if not args.apply:return
    journal_path=ROOT/'reports/KairoLibrary-variant-selections.json'
    journal=read(journal_path) if journal_path.is_file() else {'attempts':[],'initial_matched':initial['counts']['matched']}
    scratch=ROOT/'analysis/variant-selection';scratch.mkdir(parents=True,exist_ok=True)
    for target in queue:
        current=read(ROOT/'build/verification-KairoLibrary.json')
        if current['methods'][ids[target]]['state']=='matched':continue
        main_selected=selected_sources({});main_doc=parse_sources(main_selected,scratch,'main-current')
        main_index=method_index(main_doc,{str(p):logical for logical,p in main_selected.items()})
        main_text={s['path']:s['text'] for s in main_doc['sources']}
        for label,origin in options[target]:
            attempt={'target':target,'id':ids[target],'origin':origin,'variant':label}
            if origin not in main_index:attempt['status']='main_origin_not_found';journal['attempts'].append(attempt);continue
            existing=main_index[origin];candidate=variant_members[label][origin]
            path=ROOT/existing['logical_path'];before=path.read_bytes();text=before.decode('utf-8-sig')
            if text!=main_text[str(path)]:raise ValueError('Main source changed since AST snapshot')
            candidate_path=ROOT/candidate['input_path']
            if digest(candidate_path)!=candidate['input_sha256']:raise ValueError('Variant source changed since evaluation')
            try:edited=replacement(text,existing['member'],source_documents[label][candidate['input_path']],candidate['member']).encode('utf-8')
            except ValueError as error:attempt.update(status='incompatible_source_member',reason=str(error));journal['attempts'].append(attempt);continue
            if edited==before:attempt['status']='identical_source';journal['attempts'].append(attempt);continue
            serial=len(journal['attempts']);trial=scratch/('trial-'+str(serial));trial.mkdir(exist_ok=True)
            (trial/'before-source.cs').write_bytes(before)
            if not args.skip_preflight:
                trial_source=trial/path.name;trial_source.write_bytes(edited)
                source_map=trial/'source-map.json';source_map.write_text(json.dumps({existing['logical_path']:str(trial_source)}))
                build_label='selection-'+str(serial)
                build=subprocess.run([sys.executable,str(ROOT/'tools/build_kairo.py'),'--compiler',str(ROOT/'analysis/tools/roslyn-1.3.2/tools/csc.exe'),
                    '--label',build_label,'--source-map',str(source_map)],cwd=ROOT,capture_output=True,text=True,encoding='utf-8')
                if build.returncode:attempt.update(status='isolated_build_failed',log='build/KairoLibrary-'+build_label+'/compiler.log');journal['attempts'].append(attempt);continue
                binary=ROOT/'build'/('KairoLibrary-'+build_label)/'KairoLibrary.dll'
                declaring,name=target.split('::',1);name=name.split('(',1)[0].split('``',1)[0]
                inspected=subprocess.run([str(ROOT/'build/tools/ProbeInspector.exe'),str(binary),declaring+'::'+name],capture_output=True,text=True,encoding='utf-8',check=True)
                methods=json.loads(inspected.stdout)['selected_methods'];built=next((m for m in methods if m['stable_id']==target),None)
                delta,_=differences(oracle[target],built)
                if delta:attempt.update(status='isolated_body_differs',differences=delta);journal['attempts'].append(attempt);continue
                attempt['isolated_body_equal']=True;attempt['trial_artifact_sha256']=digest(binary)
            else:
                attempt['full_variant_body_equal']=True
                attempt['full_variant_artifact_sha256']=evaluation['variants'][label]['artifact_sha256']
            path.write_bytes(edited)
            try:after=run_official()
            except BaseException:
                path.write_bytes(before);run_official();raise
            previous={key for key,result in current['methods'].items() if result['state']=='matched'}
            regressions=sorted(key for key in previous if after['methods'][key]['state']!='matched')
            accepted=after['methods'][ids[target]]['state']=='matched' and not regressions and not after['regressions']
            attempt.update(status='accepted' if accepted else 'rejected_and_restored',regressions=regressions,
                matched_before=current['counts']['matched'],matched_after=after['counts']['matched'],
                body_equal_before=current['counts']['body_equal'],body_equal_after=after['counts']['body_equal'],
                source=existing['logical_path'],before_source_sha256=hashlib.sha256(before).hexdigest(),
                after_source_sha256=hashlib.sha256(edited).hexdigest(),target_result=after['methods'][ids[target]])
            if not accepted:
                path.write_bytes(before);restored=run_official()
                if any(restored['methods'][key]['state']!='matched' for key in previous):raise RuntimeError('Rollback failed to restore previous matches')
            journal['attempts'].append(attempt)
            journal['accepted_count']=sum(a['status']=='accepted' for a in journal['attempts'])
            journal['latest_counts']=read(ROOT/'build/verification-KairoLibrary.json')['counts']
            journal_path.write_text(json.dumps(journal,indent=2)+'\n',encoding='utf-8',newline='\n')
            print(json.dumps({k:attempt.get(k) for k in ('target','variant','status','matched_before','matched_after','regressions')}),flush=True)
            if accepted:break
        journal_path.write_text(json.dumps(journal,indent=2)+'\n',encoding='utf-8',newline='\n')


if __name__=='__main__':main()
