"""IL-directed syntax hypotheses, separated by compiler; adopt only verified gains.

No compiled artifact is edited. Isolated trial builds never promote ledger states.
"""
import argparse,csv,json,re,subprocess,sys
from pathlib import Path
from collections import Counter
from compare_kairo import differences,sha
from compiler_probes import ROSLYN
from kairo_candidate import LABEL,COMPILER_PATH
ROOT=Path(__file__).resolve().parents[1]
def read(p):return json.loads(p.read_text(encoding='utf-8-sig'))
def receipt_is_fresh(root,receipt):
    # Exit1 can mean a known mismatch OR a compile failure; reject stale proof.
    try:
        return sha(root/receipt['build_artifact'])==receipt['build_sha256'] and all(
            sha(root/s['path'])==s['sha256'] for s in receipt['sources'])
    except (OSError,KeyError):return False
def run(args,log):
    p=subprocess.run([str(x) for x in args],cwd=ROOT,capture_output=True)
    log.write_bytes(p.stdout+p.stderr)
    return p.returncode
def dump(binary,out):
    with out.open('wb') as f:subprocess.run([str(ROOT/'build/tools/ProbeInspector.exe'),str(binary),'--structure'],stdout=f,check=True)
    return {m['stable_id']:m for m in read(out)['selected_methods']}
def index_current(a):
    label=LABEL if a=='KairoLibrary' else 'mono-smcs'
    folder=ROOT/'build'/(a+'-'+label)
    return {m['stable_id']:m for m in read(folder/'original-methods.json')['selected_methods']},{m['stable_id']:m for m in read(folder/'rebuilt-methods.json')['selected_methods']},read(folder/'build-manifest.json')
def branchops(m):
    conditions={'brtrue','brfalse','beq','bne.un','bge','bge.un','bgt','bgt.un','ble','ble.un','blt','blt.un'}
    ops=[i['opcode'][:-2] if i['opcode'].endswith('.s') else i['opcode'] for i in m.get('instructions',[])]
    return [op for op in ops if op in conditions]
def patchtext(text,patches):
    encoded=text.encode('utf-16-le')
    for p in sorted(patches,key=lambda p:p['start'],reverse=True):
        start,end=2*p['start'],2*p['end']
        if encoded[start:end].decode('utf-16-le')!=p['before']:raise ValueError('Stale syntax span')
        encoded=encoded[:start]+p['after'].encode('utf-16-le')+encoded[end:]
    return encoded.decode('utf-16-le')
def nonoverlap(patches):
    output=[]
    for p in sorted(patches,key=lambda p:(p['start'],-p['end'])):
        if not any(p['start']<q['end'] and p['end']>q['start'] for q in output):output.append(p)
    return output
def main():
    ap=argparse.ArgumentParser();ap.add_argument('--rule',required=True,choices=['tail-early-return','return-local','ternary-polarity','if-polarity','switch-if','guard-return','guard-return-reverse','condition-local','value-guard','value-guard-reverse']);ap.add_argument('--assembly',required=True,choices=['KairoLibrary','Assembly-CSharp']);args=ap.parse_args()
    a=args.assembly;rule=args.rule;folder=ROOT/'build/round3'/a/rule;folder.mkdir(parents=True,exist_ok=True)
    oracle,before,manifest=index_current(a)
    equal={k for k,m in oracle.items() if 'raw_body_hex'in m and not differences(m,before.get(k))[0]}
    ledger=list(csv.DictReader((ROOT/'config/methods.csv').open(encoding='utf-8',newline='')))
    rows={r['id'].split('::',1)[1].replace('+','/'):r for r in ledger if r['assembly']==a}
    sources={s['path']:ROOT/s['path'] for s in manifest['sources']}
    vs=next(p for p in ROSLYN.parents if (p/'dotnet').is_dir())
    parse=folder/'parse-input.json';parse.write_text(json.dumps({'sources':[str(p) for p in sources.values()],'references':[r['path'] for r in manifest['references']]}))
    with (folder/'syntax.json').open('wb') as f:
        subprocess.run([str(vs/'dotnet/net10.0/runtime/dotnet.exe'),str(ROOT/'build/source-recovery/IlRuleSyntax.dll'),str(parse)],stdout=f,check=True)
    syntax=read(folder/'syntax.json');members={m['id']:m for m in syntax['methods']}
    corpus=Counter();examples=[]
    for key in sorted(equal):
        if key not in members:continue
        member=members[key];original=oracle[key];corpus['explicit_matched_methods']+=1
        for p in member['patches']:corpus['syntax_'+p['rule']]+=1
        c=member['constructs'];ops=[i['opcode'] for i in original['instructions']]
        if not c['lambda_count']:
            corpus['lambda_free_methods']+=1
            if len(member['locals'])==len(original['locals']):corpus['source_local_count_equals_IL_count']+=1
            else:corpus['source_local_count_differs_from_IL_count']+=1
            if not any(c[k] for k in ['foreach_count','using_count','lock_count','switch_count']) and len(member['locals'])!=len(original['locals']):
                corpus['count_difference_without_four_named_constructs']+=1
                if len(examples)<8:examples.append({'id':key,'source_locals':member['locals'],'IL_locals':original['locals'],'constructs':c})
        if len(ops)>2 and ops[-1]=='ret' and ops[-2].startswith('ldloc') and ops[-3].startswith('stloc'):corpus['terminal_store_load_return']+=1
    targets=[]
    for key,m in oracle.items():
        if key in equal or 'raw_body_hex' not in m:continue
        delta=differences(m,before.get(key))[0]
        facets=set(delta)-{'resolved_body_bytes'}
        broad=rule in ('ternary-polarity','if-polarity','guard-return','guard-return-reverse','condition-local','value-guard','value-guard-reverse')
        if not broad and facets not in ({'header','instructions'},{'header','locals','instructions'}) and '<GetGraphics>' not in key:continue
        origin=key
        if key not in members:
            origin=rows.get(key,{}).get('gerado_por','').split('::',1)[-1].replace('+','/')
        if origin not in members:continue
        member=members[origin];patches=[p for p in member['patches'] if p['rule']==rule]
        if not patches:continue
        ops=[i['opcode'] for i in m['instructions']]
        if rule=='tail-early-return' and not (len(ops)>=2 and ops[-2:]==['ret','ret']):continue
        if rule=='return-local' and not (a=='Assembly-CSharp' and len(m['locals'])==len(before[key]['locals'])+1 and len(ops)>2 and ops[-1]=='ret' and ops[-2].startswith('ldloc') and ops[-3].startswith('stloc')):continue
        if rule=='switch-if' and not (a=='Assembly-CSharp' and len(before[key]['locals'])>len(m['locals']) and 'switch' not in ops):continue
        if rule in ('guard-return','guard-return-reverse') and a!='Assembly-CSharp':continue
        if rule in ('value-guard','value-guard-reverse'):
            if a!='Assembly-CSharp' or key not in before:continue
            if ops.count('ret')<=sum(i['opcode']=='ret' for i in before[key].get('instructions',[])):continue
        if rule=='condition-local':
            if a!='Assembly-CSharp' or key not in before:continue
            slots=m['locals'];oldslots=before[key]['locals'];missing=[]
            if len(slots)!=len(oldslots)+1:continue
            for i,slot in enumerate(slots):
                if slots[:i]+slots[i+1:]==oldslots and slot.startswith('valuetype System.Boolean @'):missing.append(i)
            if not missing:continue
            def slotop(ins,op):
                code=ins['opcode'];operand=ins.get('operand')
                if code.startswith(op+'.') and code[len(op)+1:].isdigit():return int(code[len(op)+1:])
                if code in (op,op+'.s') and isinstance(operand,dict):return operand.get('local')
                return None
            anchors=[i for i in range(len(m['instructions'])-2) if slotop(m['instructions'][i],'stloc') in missing and slotop(m['instructions'][i+1],'ldloc')==slotop(m['instructions'][i],'stloc') and m['instructions'][i+2]['opcode'].startswith(('brtrue','brfalse'))]
            if not anchors:continue
            patches=[p for p in patches if p.get('temp_type')=='System.Boolean']
            if not patches:continue
        if rule in ('ternary-polarity','if-polarity') and branchops(m)==branchops(before.get(key,{})):continue
        targets.append({'id':key,'origin':origin,'path':member['path'],'patches':nonoverlap(patches)})
    # The user's Image example is outside H+I/H+L+I and is tested explicitly.
    if a=='KairoLibrary' and rule=='ternary-polarity':
        origin='kairo.unity.ui.Image::GetGraphics()->kairo.unity.ui.Graphics';key='kairo.unity.ui.Image::<GetGraphics>b__98_0()->System.Object'
        if key not in equal and not any(t['id']==key for t in targets):
            m=members[origin];targets.append({'id':key,'origin':origin,'path':m['path'],'patches':nonoverlap([p for p in m['patches'] if p['rule']==rule])})
    unique={t['origin']:t for t in targets};original_text={p:Path(p).read_bytes().decode('utf-8-sig') for p in {t['path'] for t in targets}}
    def candidate(selected,label):
        dest=folder/label;dest.mkdir(exist_ok=True);byfile={}
        for t in selected:byfile.setdefault(t['path'],[]).extend(t['patches'])
        mapping={}
        for n,(p,patches) in enumerate(byfile.items()):
            out=dest/(str(n)+'-'+Path(p).name);out.write_text(patchtext(original_text[p],nonoverlap(patches)),encoding='utf-8',newline='')
            mapping[Path(p).relative_to(ROOT).as_posix()]=str(out)
        mp=dest/'source-map.json';mp.write_text(json.dumps(mapping))
        blabel='round3-il-rule-'+a.lower()+'-'+rule+'-'+label
        cmd=[sys.executable,ROOT/'tools'/('build_kairo.py' if a=='KairoLibrary' else 'build_csharp.py'),'--label',blabel,'--source-map',mp]
        if a=='KairoLibrary':cmd+=['--compiler',ROOT/COMPILER_PATH]
        code=run(cmd,dest/'build.log')
        if code:return None,[],[],mapping
        built=dump(ROOT/'build'/(a+'-'+blabel)/(a+'.dll'),dest/'methods.json')
        current={k for k,m in oracle.items() if 'raw_body_hex'in m and not differences(m,built.get(k))[0]}
        return built,sorted(current-equal),sorted(equal-current),mapping
    report={'assembly':a,'compiler':LABEL if a=='KairoLibrary' else 'smcs2.6.5','rule':rule,'before_body_equal':len(equal),'corpus':dict(corpus),'local_hypothesis_counterexamples':examples,'syntax_errors':syntax['errors'],'target_count':len(targets),'targets':targets,'adopted':[],'state_gains':0,'state_losses':0}
    if targets:
        built,gains,losses,mapping=candidate(list(unique.values()),'trial');report.update(trial_new_equal=gains,trial_losses=losses,trial_compile_pass=built is not None)
        if built is not None:
            badtypes={k.split('::')[0].split('/<')[0] for k in losses}
            adopted=[t for t in unique.values() if t['origin'].split('::')[0] not in badtypes and any(x['id'] in gains for x in targets if x['origin']==t['origin'])]
            if adopted:
                final,fg,fl,fm=candidate(adopted,'selected')
                if final is None or fl:raise ValueError('Selected candidate fails no-regression check: '+str(fl))
                snapshot={logical:(ROOT/logical).read_bytes() for logical in fm}
                for logical,path in fm.items():(ROOT/logical).write_bytes(Path(path).read_bytes())
                code=run([sys.executable,ROOT/'tools/verify','--all','--assembly',a],folder/'official-verify.log')
                receipt=read(ROOT/'build'/('verification-'+a+'.json'))
                if code not in (0,1) or not receipt_is_fresh(ROOT,receipt) or receipt['regressions'] or receipt.get('type_regressions'):
                    for logical,data in snapshot.items():(ROOT/logical).write_bytes(data)
                    run([sys.executable,ROOT/'tools/verify','--all','--assembly',a],folder/'rollback-verify.log')
                    raise ValueError('Official verification regression; sources restored')
                oldstates={r['id']:r['estado'] for r in ledger if r['assembly']==a}
                report.update(adopted=[t['origin'] for t in adopted],adopted_new_equal=fg,selected_losses=fl,state_gains=sum(v['state']=='matched' and oldstates[k]!='matched' for k,v in receipt['methods'].items()),state_losses=sum(v['state']!='matched' and oldstates[k]=='matched' for k,v in receipt['methods'].items()))
    out=ROOT/'reports'/('round3-'+a+'-IL-rule-'+rule+'.json');out.write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({k:v for k,v in report.items() if k in ['assembly','rule','corpus','target_count','state_gains','state_losses','trial_compile_pass','trial_losses','adopted']}),flush=True)
if __name__=='__main__':main()
