"""Render round-ten reports only after an explicit stop and fresh full proof.

No compiler, verifier, ledger mutation or round-seven report import is used.
The full report and the standalone chat version share the same measured data.
"""
from collections import Counter
import csv
import json
from pathlib import Path
import re
import sys

ROOT=next(p for p in Path(__file__).resolve().parents if (p/'tools/compiler_probes.py').is_file())
sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import sha
ASSEMBLIES=('KairoLibrary','Assembly-CSharp')
BASE_COMMIT='aecb02b8a7b0965508e223335e44ae36a95a0939'
SECTIONS=('1. PROGRESSO','2. O QUE FOI FEITO NESTA RODADA','3. HIPÓTESES',
          '4. PENDENTES POR GRUPO','5. TOOLCHAIN','6. COMPONENTES EXTERNOS E METADADOS',
          '7. BLOQUEIOS','8. SUA RECOMENDAÇÃO')


def read(path):return json.loads((ROOT/path).read_text(encoding='utf-8-sig'))
def ledger(path):
    with (ROOT/path).open(encoding='utf-8-sig',newline='') as stream:
        rows=list(csv.DictReader(stream))
    if len({r['id'] for r in rows})!=len(rows):raise ValueError('Duplicate ledger ID: '+str(path))
    return rows
def exists(path):return (ROOT/path).is_file()
def table(headers,rows):
    def cell(value):return str(value).replace('|','\\|').replace('\r','').replace('\n','<br>')
    rows=list(rows)
    return '\n'.join(['| '+' | '.join(headers)+' |','| '+' | '.join('---' for _ in headers)+' |']+
        ['| '+' | '.join(cell(v) for v in row)+' |' for row in rows])
def pct(n,d):return f'{100*n/d:.6f}%' if d else 'N/A (denominador zero)'
def inline(text):
    text=str(text);runs=re.findall(r'`+',text);fence='`'*(max(map(len,runs),default=0)+1)
    return fence+text+fence
def counts(row):
    return (f"{row['matched_methods']}/{row['total_methods']} ({pct(row['matched_methods'],row['total_methods'])}); "
        f"IL {row['matched_original_il_bytes']}/{row['original_total_il_bytes']} bytes "
        f"({pct(row['matched_original_il_bytes'],row['original_total_il_bytes'])})")
def operand(value):
    if value is None:return ''
    if isinstance(value,dict):
        if 'local' in value:return str(value['local'])
        if 'labels' in value:return ','.join(map(str,value['labels']))
        value=next(iter(value.values())) if len(value)==1 else json.dumps(value,ensure_ascii=False)
    return re.sub(r' @ [^,<>]+, Version=[^,]+, Culture=[^,]+, PublicKeyToken=[a-zA-Z0-9]+','',str(value))
def il(row):
    value=row.get('operand')
    rendered=json.dumps(value,ensure_ascii=False) if row['opcode']=='ldstr' else operand(value)
    return (f"IL_{row['offset']:04x}: "+row['opcode']+' '+rendered).rstrip()
def passes(engine):
    for rows in engine.get('previous_passes',{}).values():yield from rows
    yield from engine.get('methods',[])
def check_hash(path,expected,description):
    if sha(ROOT/path)!=expected:raise ValueError(description+': '+str(path))
def compact_counter(counter):
    return ', '.join(f'{k}: {v}' for k,v in sorted(counter.items())) or 'nenhum'


def check_measurement(document,ledger_path):
    check_hash(ledger_path,document['ledger_sha256'],'Stale measurement ledger')
    if document.get('baseline_commit')!=BASE_COMMIT:raise ValueError('Wrong round-ten baseline')
    for assembly in ASSEMBLIES:
        p=document['assemblies'][assembly]['provenance']
        for pathkey,hashkey in [('original_dump','original_dump_sha256'),('rebuilt_dump','rebuilt_dump_sha256'),('receipt','receipt_sha256')]:
            check_hash(p[pathkey],p[hashkey],'Stale measurement input')
        if read(p['receipt'])['build_sha256']!=p['build_sha256']:
            raise ValueError('Measurement/receipt build disagreement: '+assembly)


def fresh_receipt(receipt,assembly,original_hash):
    check_hash(receipt['build_artifact'],receipt['build_sha256'],'Stale official build')
    if receipt['original_sha256']!=original_hash:raise ValueError('Wrong original identity: '+assembly)
    for source in receipt['sources']:check_hash(source['path'],source['sha256'],'Stale source proof')
    for path,digest in receipt.get('helper_source_hashes',{}).items():check_hash(path,digest,'Stale verifier helper')
    if any(receipt.get(k) for k in ('regressions','body_regressions','type_regressions')):
        raise ValueError('Official receipt records regressions: '+assembly)


def validate_control_proof(path,corpus,expected_matched):
    document=read(path)
    if document.get('valid_frozen_input_measurement') is not True or document.get('inputs_changed_during_run'):
        raise ValueError('Final control extraction does not have frozen valid inputs')
    if document.get('own_IL_or_MDB_used_to_infer_AST_control') is not False:
        raise ValueError('Final AST control independence is not proved')
    if (ROOT/document['snapshot']).resolve()!=(ROOT/corpus).resolve():
        raise ValueError('Final control extraction uses another corpus')
    check_hash(Path(corpus)/'methods.csv',document['ledger_sha256'],'Stale final control ledger')
    for source,digest in document['extractor_inputs'].items():
        check_hash(source,digest,'Stale final control extractor')
    check_hash(document['method_irs'],document['method_irs_sha256'],'Stale final control method IRs')
    source_manifest=Path(document['method_irs']).parent/'source-manifest.json'
    check_hash(source_manifest,document['source_manifest_sha256'],'Stale final control source manifest')
    if document['matched']['methods']!=expected_matched:
        raise ValueError('Final control corpus and final smcs model corpus disagree')
    return document


def load_context():
    # This must remain the first precondition: an active cycle cannot publish a
    # premature final report, even if a final measurement from a retry exists.
    if not exists('reports/round10-stop.json'):
        raise RuntimeError('Round 10 has no stop receipt; no report was written')
    stop=read('reports/round10-stop.json')
    if stop.get('new_reconstruction_work_stopped') is False:
        raise ValueError('The stop receipt says reconstruction is still active')
    if not stop.get('reason'):raise ValueError('Missing stop reason')
    cfg=read('config/round10-protocol.json')
    if cfg.get('report_schema_reviewed') is not True:raise ValueError('Round10 ABC/runtime/raw/control report adapter not reviewed yet; no report written')
    if cfg['baseline_commit']!=BASE_COMMIT:raise ValueError('Wrong protocol baseline')
    baseline=read('reports/round10-baseline-measurement.json')
    final=read('reports/round10-final-measurement.json')
    check_measurement(baseline,'build/round10/baseline/methods.csv')
    check_measurement(final,'config/methods.csv')
    if read('build/round10/baseline/snapshot.json')['commit']!=BASE_COMMIT:
        raise ValueError('Baseline snapshot is another commit')
    old_rows=ledger('build/round10/baseline/methods.csv');rows=ledger('config/methods.csv')
    current_by_id={r['id']:r for r in rows}
    if {r['id'] for r in old_rows}!=set(current_by_id):raise ValueError('Ledger method inventory changed')
    with (ROOT/'config/inventory.csv').open(encoding='utf-8-sig',newline='') as stream:inventory=list(csv.DictReader(stream))
    apk_hashes={r['apk_sha256'] for r in inventory if r.get('apk_sha256')}
    if len(apk_hashes)!=1:raise ValueError('Ambiguous original APK identity')
    targets={Path(r['caminho']).name:r['sha256'] for r in inventory if r['categoria']=='alvo' and r['caminho'].endswith('.dll')}
    receipts={};metadata={};losses={};body_losses={};held=[];manifests={};stable_sources={}
    for assembly in ASSEMBLIES:
        original_hash=targets[assembly+'.dll']
        receipt=read('build/verification-'+assembly+'.json');receipts[assembly]=receipt
        fresh_receipt(receipt,assembly,original_hash)
        p=final['assemblies'][assembly]['provenance']
        if receipt['build_sha256']!=p['build_sha256']:raise ValueError('Final measurement describes another build')
        if receipt!=read(p['receipt']):raise ValueError('Final and official receipts disagree')
        md=read('reports/'+assembly+'-metadata.json');metadata[assembly]=md
        if md['original_sha256']!=original_hash or md['rebuilt_sha256']!=receipt['build_sha256']:
            raise ValueError('Stale metadata artifact identity: '+assembly)
        if not md['whole_structural_metadata_equal']:raise ValueError('Global structural metadata mismatch: '+assembly)
        proof=md['verification_receipt']
        check_hash(proof['manifest'],proof['manifest_sha256'],'Stale metadata manifest')
        if proof['build_artifact_sha256']!=receipt['build_sha256']:raise ValueError('Stale metadata receipt')
        for helper,digest in proof.get('helper_source_hashes',{}).items():check_hash(helper,digest,'Stale metadata helper')
        manifests[assembly]=read(proof['manifest'])
        before={r['id'] for r in old_rows if r['assembly']==assembly and r['estado']=='matched'}
        after={r['id'] for r in rows if r['assembly']==assembly and r['estado']=='matched'}
        losses[assembly]=sorted(before-after)
        previous=read('build/round10/baseline/'+assembly+'/verification.json')
        body_losses[assembly]=[key for key,value in previous['methods'].items() if value.get('body_equal') and not receipt['methods'].get(key,{}).get('body_equal')]
        if losses[assembly] or body_losses[assembly]:raise ValueError('Regression from round-ten baseline: '+assembly)
        originals={m['stable_id']:m for m in read(p['original_dump'])['selected_methods']}
        for identity,value in receipt['methods'].items():
            row=current_by_id[identity]
            if value['state']!=row['estado']:raise ValueError('Ledger/receipt state disagreement: '+identity)
            stable=value['stable_id'];stable_sources[(assembly,stable)]=row['fonte']
            if value.get('body_equal') and value['state']!='matched' and 'raw_body_hex' in originals[stable]:
                held.append((assembly,stable,originals[stable]['header']['code_size']))
        for document,selected in [(baseline,old_rows),(final,rows)]:
            subset=[r for r in selected if r['assembly']==assembly];matched=[r for r in subset if r['estado']=='matched'];m=document['assemblies'][assembly]
            if m['total_methods']!=len(subset) or m['matched_methods']!=len(matched) or m['matched_original_il_bytes']!=sum(int(r['tamanho_il']) for r in matched):
                raise ValueError('Measurement/CSV count disagreement: '+assembly)
    journal=read('reports/round10-cycle-journal.json')
    cycles=journal['cycles']
    if [c['cycle'] for c in cycles]!=list(range(1,len(cycles)+1)):raise ValueError('Nonsequential cycle journal')
    autonomous=[c for c in cycles if c.get('phase')=='D']
    if len(autonomous)>cfg['maximum_cycles']:raise ValueError('Autonomous cycle cap exceeded')
    if stop.get('cycles_completed',len(cycles))!=len(cycles):raise ValueError('Stop/journal cycle count disagreement')
    if stop.get('autonomous_cycles_completed',len(autonomous))!=len(autonomous):raise ValueError('Stop/autonomous cycle count disagreement')
    if any(cfg.get('required_parts',{}).get(p)!='complete' for p in 'ABC'):
        raise ValueError('Required Parts A-C are not complete')
    for cycle in cycles:
        check_hash(cycle['before_measurement'],cycle['before_sha256'],'Stale cycle input')
        check_hash(cycle['after_measurement'],cycle['after_sha256'],'Stale cycle output')
    for assembly in ASSEMBLIES:
        b=baseline['assemblies'][assembly];f=final['assemblies'][assembly]
        for key,field in [('methods_gained','matched_methods'),('il_bytes_gained','matched_original_il_bytes')]:
            if sum(c['gains'][assembly][key] for c in cycles)!=f[field]-b[field]:raise ValueError('Journal/official net gain disagreement: '+assembly)
    if cycles and read(cycles[-1]['after_measurement'])['ledger_sha256']!=final['ledger_sha256']:
        raise ValueError('Final ledger is outside the journal')
    if set(cfg.get('final_model_measurements',{}))!=set(ASSEMBLIES):
        raise ValueError('Both final model measurements are required for the final report')
    models={}
    for assembly,model in cfg['models'].items():
        measurement=cfg.get('final_model_measurements',{}).get(assembly,model)
        proof=read(measurement['validation']);models[assembly]=proof
        check_hash(model['source'],proof['source_sha256'],'Stale model source proof')
        check_hash(model['client'],proof['client_sha256'],'Stale model client proof')
        check_hash(Path(measurement['corpus_baseline'])/'verification.json',proof['baseline_receipt_sha256'],'Stale model corpus receipt')
        if proof['matched_model_complete']!=proof['matched_complete_exact'] or proof.get('matched_complete_mismatches',0):raise ValueError('Model has incorrect complete predictions')
        corpus=read(Path(measurement['corpus_baseline'])/'verification.json')
        if proof['matched_bodies']!=sum(v['state']=='matched' and v.get('body_present',False) for v in corpus['methods'].values()):
            raise ValueError('Model validation does not cover the declared matched corpus')
    control_path=cfg.get('control_extractor_validation')
    if not control_path:
        raise ValueError('Final control extraction proof is required')
    control_corpus=Path(cfg['final_model_measurements']['Assembly-CSharp']['corpus_baseline']).parent
    control_extraction=validate_control_proof(control_path,control_corpus,models['Assembly-CSharp']['matched_bodies'])
    engines=[]
    for path in sorted((ROOT/'reports').glob('round10-*-engine.json')):
        document=read(path)
        if document.get('completed'):engines.append((path.name,document))
    unfinished=[p.name for p in (ROOT/'reports').glob('round10-*-engine.json') if not read(p).get('completed')]
    adoptions={}
    for path in (ROOT/'reports').glob('round10-*-adoption.json'):
        document=read(path)
        if document.get('official_verified') and path.name=='round10-'+document['run_label']+'-adoption.json':adoptions[document['run_label']]=document
    for cycle in cycles:
        if cycle['label'] not in adoptions:raise ValueError('Journal cycle lacks official adoption proof: '+cycle['label'])
        for proof in adoptions[cycle['label']]['assemblies'].values():
            if proof.get('matched_losses'):raise ValueError('Cycle adoption records matched losses: '+cycle['label'])
    historical=[]
    historical_paths=sorted((ROOT/'reports').glob('round7-*-engine.json'))+sorted((ROOT/'reports').glob('round8-*-engine.json'))+sorted((ROOT/'reports').glob('round9-*-engine.json'))
    for path in historical_paths:
        document=read(path)
        if document.get('completed'):historical.append((path.name,document))
    return dict(cfg=cfg,stop=stop,baseline=baseline,final=final,journal=journal,models=models,engines=engines,
        historical=historical,adoptions=adoptions,receipts=receipts,metadata=metadata,losses=losses,
        body_losses=body_losses,held=held,manifests=manifests,stable_sources=stable_sources,
        apk_hash=next(iter(apk_hashes)),targets=targets,unfinished=unfinished,control_extraction=control_extraction)


def attempt_summary(ctx,assembly,group):
    identities=set(group['method_ids']);attempts=Counter();stops=Counter();rejections=Counter();limits=0;removed=0;incomplete=0
    for name,engine in ctx['engines']:
        if '-initial-screen-' in name:continue
        if engine['assembly']!=assembly:continue
        for row in passes(engine):
            if row['id'] not in identities:continue
            attempts.update(t['rule'] for t in row.get('trials',[]));stops[str(row.get('stop_reason','UNKNOWN'))]+=1
            for rejection in row.get('model_rejections',[]):
                # Remaining budget is a diagnostic gauge, not a rejected candidate count.
                rejections.update({key:value for key,value in rejection.items() if key!='prediction_budget_remaining'})
            limits+=row.get('incomplete_budget_skipped',0)
            incomplete+=sum(bool(t.get('incomplete_prediction')) for t in row.get('trials',[]))
            removed+=sum(f.get('removed_legacy_candidates',0) for f in row.get('cycle_repertoire_filters',[]))
    history=Counter();history_runs=set()
    if not attempts:
        # Historical attempts are admitted only when the original inventory is
        # unchanged and the entire source file equals that historical checkpoint.
        p=ctx['final']['assemblies'][assembly]['provenance']
        for name,engine in ctx['historical']:
            if engine['assembly']!=assembly:continue
            base=Path(engine.get('baseline_dir',''))
            oracle=base/'original-methods.json'
            if not (ROOT/oracle).is_file() or sha(ROOT/oracle)!=p['original_dump_sha256']:continue
            source_hashes={k.replace('\\','/'):v for k,v in engine.get('source_hashes',{}).items()}
            for row in passes(engine):
                if row['id'] not in identities:continue
                source=ctx['stable_sources'].get((assembly,row['id']))
                if not source or not exists(source) or source_hashes.get(source.replace('\\','/'))!=sha(ROOT/source):continue
                trials=row.get('trials',[])
                if trials:history.update(t['rule'] for t in trials);history_runs.add(name)
    return dict(attempts=attempts,stops=stops,rejections=rejections,budget_skipped=limits,legacy_removed=removed,
        incomplete_trials=incomplete,history=history,history_runs=sorted(history_runs))


def attempts_text(summary,short=False):
    text='Tentativas compiladas no grupo na rodada 10: '+compact_counter(summary['attempts'])
    if summary['incomplete_trials']:text+=f"; {summary['incomplete_trials']} com previsão incompleta"
    text+='.'
    if not summary['attempts']:
        if summary['history']:
            text+=' Histórico 7/8 compatível (mesmo original e hash do arquivo fonte atual): '+compact_counter(summary['history'])+'.'
            if not short:text+=' Recibos: '+', '.join(summary['history_runs'])+'.'
        else:text+=' Histórico 7/8 compatível: UNKNOWN; não se presume compatibilidade após alteração do fonte.'
    if summary['rejections'] or summary['budget_skipped'] or summary['legacy_removed']:
        text+=' Antes de compilar: '+compact_counter(summary['rejections'])+f"; orçamento recusou {summary['budget_skipped']}; repertório anterior filtrou {summary['legacy_removed']}."
    if not short:text+=' Paradas: '+compact_counter(summary['stops'])+'.'
    return text


def context_rows(example,short=False):
    def select(key,offsetkey):
        rows=example[key]
        if not short:return rows[:15]
        focus=example.get(offsetkey)
        index=next((i for i,r in enumerate(rows) if r['offset']==focus),min(2,len(rows)-1))
        return rows[index:index+2]
    left=select('original_context','original_offset');right=select('rebuilt_context','rebuilt_offset')
    return [(inline(il(left[i])) if i<len(left) else '',inline(il(right[i])) if i<len(right) else '') for i in range(max(len(left),len(right)))]


def policy_text(ctx):
    cfg=ctx['cfg'];perpass=cfg.get('incomplete_candidate_variants_per_method_per_pass','UNKNOWN');percycle=cfg.get('incomplete_candidate_variants_per_method_per_cycle','UNKNOWN')
    return (f'Previsão incompleta PERMITE compilar: limite de {perpass} variantes por método/passagem, '
        f'{percycle} por método/ciclo, no máximo uma sonda isolada e uma passagem plena. '
        'Cada variante pode exigir screen e compilação plena; variantes e invocações são medidas separadamente. '
        'Previsão completa com assinatura errada é recusada antes de compilar. Depois da compilação, '
        'a assinatura emitida precisa ser a original; previsão completa também precisa concordar com o emissor. '
        'Cabeçalho, locais, IL e EH continuam sujeitos ao mesmo verificador integral.')


def model_table(ctx):
    rows=[]
    for assembly in ASSEMBLIES:
        d=ctx['models'][assembly]
        rows.append((assembly,d['matched_bodies'],d['matched_source_mapped'],d['matched_source_unmapped'],d['matched_model_complete'],d['matched_complete_exact'],
            pct(d['matched_complete_exact'],d['matched_model_complete']),pct(d['matched_model_complete'],d['matched_bodies']),d['matched_mapped_incomplete'],d['matched_mapped_incomplete_equal']))
    return table(['Assembly/modelo','Matched do corpus','Mapeados','Sem mapeamento','Completos','Completos exatos','Precisão','Cobertura','Mapeados incompletos','Incompletos com tipos iguais'],rows)


def adopted_engine(ctx,name,engine):
    label=engine.get('run_label')
    # An initial preparation report can be preserved under a new filename while
    # retaining its run_label. It is not the adopted retry of that same cycle.
    if name!='round10-'+str(label)+'-'+engine['assembly']+'-engine.json':return False
    adoption=ctx['adoptions'].get(label)
    return bool(adoption and adoption['assemblies'].get(engine['assembly'],{}).get('engine_executed'))


def render(ctx,short=False):
    base=ctx['baseline']['assemblies'];final=ctx['final']['assemblies'];stop=ctx['stop'];cycles=ctx['journal']['cycles']
    output=[]
    def section(number):output.extend(['## '+SECTIONS[number-1],''])
    def add(*parts):
        for part in parts:output.extend([part,''])
    section(1)
    add('Pocket League Story para Android, APK net.kairosoft.android.soccer_en_2.0.2-APK_Award.apk; reconstrução C#/Unity 5.3.4f1. Comparação com o início desta rodada, commit '+inline(BASE_COMMIT)+'. Matched exige corpo completo (cabeçalho, maxstack, locais, IL e EH); referências de tokens são resolvidas por identidade. Declarações sem corpo contam em métodos, sem bytes de IL.')
    add(table(['Assembly','Anterior (aecb02b)','Atual','Δ métodos','Δ bytes de IL','Regressões'],[
        (a,counts(base[a]),counts(final[a]),final[a]['matched_methods']-base[a]['matched_methods'],final[a]['matched_original_il_bytes']-base[a]['matched_original_il_bytes'],len(ctx['losses'][a])) for a in ASSEMBLIES]))
    add('Total: '+str(sum(final[a]['matched_methods'] for a in ASSEMBLIES))+'/'+str(sum(final[a]['total_methods'] for a in ASSEMBLIES))+' métodos; '+str(sum(final[a]['matched_original_il_bytes'] for a in ASSEMBLIES))+'/'+str(sum(final[a]['original_total_il_bytes'] for a in ASSEMBLIES))+' bytes de IL. Retiradas de matched por ID: '+str(sum(map(len,ctx['losses'].values())))+'; corpos antes iguais que deixaram de bater: '+str(sum(map(len,ctx['body_losses'].values())))+'.')
    add('Motivo da parada: '+stop['reason']+'.')
    if not short:add(table(['Original','SHA-256'],[('APK',inline(ctx['apk_hash'])),*[(a+'.dll',inline(ctx['targets'][a+'.dll'])) for a in ASSEMBLIES]]))
    section(2)
    # Round10 A/B/C are new evidence, not Round9 reconstruction fronts.
    for part in 'ABC':
        evidence=ctx['cfg'].get('part_evidence',{}).get(part)
        if not evidence or not evidence.get('report') or not evidence.get('sha256'):
            raise ValueError('Missing measured Round10 part evidence: '+part)
        check_hash(evidence['report'],evidence['sha256'],'Stale Round10 part '+part)
        document=read(evidence['report'])
        add('**Parte '+part+'.** '+evidence.get('summary','UNKNOWN')+' Evidência: '+inline(evidence['report'])+'.')
        if part=='A':
            if document.get('counts_toward_byte_identical_reconstruction') is not False:raise ValueError('Runtime test APK must not count as byte-matching progress')
            add(table(['Teste funcional','Resultado'],[(key,document.get(key,'UNKNOWN')) for key in ('opens','reaches_menu','match_runs','match_completes')]))
            add('O teste usou as DLLs congeladas do início '+inline(document['baseline_commit'])+
                ', em '+document['device']['model']+', Android '+document['device']['android']+
                '/API '+str(document['device']['api'])+', ABI '+document['device']['execution_abi']+
                '. Partida completa: Sun Rays 1 × 0 Blue Stars, com primeiro tempo, intervalo, segundo tempo e resultado. '
                'Não cobre automaticamente as alterações posteriores das partes C/D.')
            add('APK de teste SHA-256 '+inline(document['test_apk_sha256'])+
                '; chave de teste nova, assinatura v1 verificada. Ordem das '+str(document['entry_count'])+
                ' entradas, métodos de compressão e restos dos offsets módulo 4096 preservados; '+
                str(document['entry_count']-len(document['changed_entries']))+
                ' payloads comprimidos inalterados. Mudaram duas DLLs e três entradas de assinatura. '
                'Nível original de compressão UNKNOWN nas entradas substituídas. É somente teste; não entra na correspondência byte a byte.')
            capture=document['log_capture']
            add('Logcat restrito aos processos do jogo: '+str(capture['files'])+' capturas, '+
                str(capture['unique_timestamped_lines'])+' linhas distintas; '+
                str(capture['mono_unity_exceptions_observed'])+' exceções Mono/Unity observadas nos buffers capturados. '
                'Primeira instalação recusada com INSTALL_FAILED_VERIFICATION_FAILURE; nova tentativa solicitada pelo usuário terminou em Success. '
                'Nenhuma proteção global foi desativada. Capturas, APK e chave privada ficam fora do Git.')
        if part=='B':
            if document.get('method_states_modified') or document.get('type_states_modified'):
                raise ValueError('Raw measurement changed reconstruction states')
            regions=[document['pe_headers'],*document['pe_sections'],document['metadata_root'],*document['metadata_streams']]
            add('Medição da baseline com 44 corpos KairoLibrary divergentes: '+
                str(document['raw_whole_file']['different_common_bytes'])+' posições diferentes em '+
                str(document['raw_whole_file']['original_bytes'])+' bytes por arquivo. Regiões se sobrepõem; não somar camadas.',
                table(['Região','Bytes originais','Bytes reconstruídos','Diferentes/sem par'],[
                    (r['name'],r['original_bytes'],r['rebuilt_bytes'],r['different_or_unpaired_bytes']) for r in regions]))
            tables=document['metadata_tables']
            if not all(r['rows']['original_rows']==r['rows']['rebuilt_rows'] for r in tables):
                raise ValueError('Raw table row-count conclusion no longer holds')
            add('As '+str(len(tables))+' tabelas têm as mesmas contagens de linhas; 13 contêm bytes diferentes. '
                'AssemblyRef difere somente nos índices PublicKeyOrToken de #Blob das linhas 1, 2 e 4: 11271 → 11302; '
                'versões e token b77a5c561934e089 coincidem. Os 116 payloads FieldRVA, 16491 bytes, são idênticos.')
            add('Persistência independente dos corpos, PROVADA com caminhos/opções atuais: 97 bytes do caminho CodeView/PDB. '
                'Foram excluídos analiticamente apenas MVID, timestamp COFF, timestamp do diretório de debug e GUID RSDS. '
                'Não houve normalização, nova exceção ou correção de bytes. Persistência futura dos demais índices, RVAs e dos 32 bytes extras de #Blob: UNKNOWN.')
        if part=='C':
            application=document['application']
            add(table(['Aplicação do esqueleto smcs','Resultado'],[
                ('Pendentes auditados, incluindo um corpo igual retido',application['audited_pending']),
                ('Pares de árvores completos antes',application['paired_complete_before']),
                ('Árvores iguais / diferentes antes',str(application['trees_equal_before'])+' / '+str(application['trees_different_before'])),
                ('Métodos reescritos com plano/árvore candidatos iguais ao original',application['candidate_tree_and_plan_equal_original']),
                ('Corpos aprovados pelo verificador / bytes de IL',str(application['matched_gains'])+' / '+str(application['original_il_bytes_gained'])),
                ('Retiradas de matched',application['matched_losses'])]))
            add('Alvo aprovado: '+inline(application['target_id'])+
                '. Os demais bloqueios do cálculo de fonte: '+compact_counter(Counter(application['groups']))+
                '. Não foram criadas folhas ou instruções a partir de bytes originais.')
            final_stage=document['stages'][-1]['groups']['matched']
            add('No corpus fixo de '+str(document['baseline_matched_bodies'])+' corpos matched: '+
                str(final_stage['source_plan_complete'])+' planos completos/'+str(final_stage['source_plan_complete'])+' iguais; '+
                str(final_stage['paired_tree_complete'])+' árvores pareadas/'+str(final_stage['trees_equal'])+' iguais. '
                'Cobertura das árvores: '+format(final_stage['paired_tree_complete']/document['baseline_matched_bodies']*100,'.6f')+
                '%. Switch string/sparse e regiões com owner indistinguível mantêm os limites de completude documentados.')
            add('Resultados negativos preservados: a primeira ampliação EH teve 14 planos completos incorretos e foi reprovada; '
                'a lógica composta revelou uma renormalização de laço, corrigida antes do aceite. '
                'A validação final de cada etapa não tem planos/árvores incorretos nos matched. '
                'O renderizador manteve árvore/plano em '+str(application['renderer_roundtrip_exact'])+'/'+
                str(application['renderer_roundtrip_tested'])+' controles NoEmit; '+application['renderer_negative']+'.')
        if evidence.get('render_rows'):
            add(table(evidence.get('render_headers',['Medida','Valor']),evidence['render_rows']))
    add('Ganhos oficiais vêm do diário após adoção e verificação completa, incluindo auxiliares liberados pelo verificador. Sondas e avanços parciais são registrados separadamente; não se somam aos ganhos oficiais. Transformações herdadas da rodada 7 só aparecem abaixo quando foram efetivamente tentadas nesta rodada.')
    if short:
        add(table(['Ciclo','Assembly / regra','Matched ganhos','Bytes ganhos','Matched perdidos'],[
            (c['label'],c['assembly']+' / '+c['rule'],c['methods_gained'],c['il_bytes_gained'],
             sum(len(p.get('matched_losses',[])) for p in ctx['adoptions'][c['label']]['assemblies'].values())) for c in cycles]))
    else:
        add(table(['Ciclo','Compilador / par','Hipótese','Resultado','Matched ganhos','Bytes ganhos','Matched perdidos'],[
            (c['label'],c['assembly']+' / '+c['pair'],c['hypothesis'],c['result'],c['methods_gained'],c['il_bytes_gained'],
             sum(len(p.get('matched_losses',[])) for p in ctx['adoptions'][c['label']]['assemblies'].values())) for c in cycles]))
    preparation=stop.get('preparation_notes',[])
    if isinstance(preparation,str):preparation=[preparation]
    for note in preparation:add(str(note))
    if exists('reports/round10-prepared-only.json'):
        prepared=read('reports/round10-prepared-only.json')['rows']
        add('Frentes preparadas antes da parada, sem teste dos respectivos candidatos no jogo; ganhos e perdas adotados: zero.',
            table(['Frente','Preparado / prova','Compilações de candidatos do jogo','Métodos ganhos / perdidos'],[
                (r['front'],str(r.get('target_methods',r.get('eligible_methods','UNKNOWN')))+' alvos; '+str(r.get('original_il_bytes',r.get('eligible_il_bytes','UNKNOWN')))+' bytes; '+str(r['fixture_complete_equal'])+' corpos de fixture exatos',r['game_candidate_compilations'],'0 / 0') for r in prepared]))
    if not short:
        rule_rows=[];negative_rows=[]
        for name,engine in ctx['engines']:
            kind='INVALIDO: screen usou baseline' if '-initial-screen-' in name else 'plena adotada' if adopted_engine(ctx,name,engine) else 'sonda/preparo/não adotada'
            counters=engine.get('transformations',{})
            for rule in sorted({key.rsplit(':',1)[0] for key in counters}):
                rule_rows.append((name,kind,rule,counters.get(rule+':attempts',0),counters.get(rule+':accepted_steps',0),counters.get(rule+':body_gains',0),counters.get(rule+':IL_gains',0),0 if kind=='plena adotada' else 'não adotado'))
            rows=list(passes(engine));trials=[trial for row in rows for trial in row.get('trials',[])];gate=engine.get('slot_model_gate',{})
            negative_rows.append((name,len(rows),sum(not t.get('compile_pass',False) or t.get('full_compile_pass') is False for t in trials),
                sum(bool(t.get('body_regressions')) for t in trials),sum(bool(t.get('metadata_rejected')) for t in trials),gate.get('post_compile_disagreements',0),
                gate.get('compiled_signature_rejections',0),sum(len(row.get('external_rebase_rejections',[])) for row in rows),sum(row.get('incomplete_budget_skipped',0) for row in rows),compact_counter(Counter(str(row.get('stop_reason','UNKNOWN')) for row in rows))))
        add(table(['Execução','Natureza','Transformação','Variantes tentadas','Avanços','Corpos ganhos no motor','Bytes no motor','Perdas adotadas'],rule_rows))
        add(table(['Execução','Métodos/passes','Compilações falhas','Regressões rejeitadas','Metadados rejeitados','Modelo/emissor discordam','Assinatura original recusada','Rebase externo recusado','Orçamento incompleto recusado','Paradas'],negative_rows))
    if short:
        negative_chat=[]
        for name,engine in ctx['engines']:
            if '-initial-screen-' in name:continue
            rows=list(passes(engine));trials=[trial for row in rows for trial in row.get('trials',[])]
            gate=engine.get('slot_model_gate',{})
            rejected_model=sum(item.get('wrong_signature',0) for row in rows for item in row.get('model_rejections',[]))
            values=(sum(not t.get('compile_pass',False) or t.get('full_compile_pass') is False for t in trials),
                    sum(bool(t.get('body_regressions')) for t in trials),sum(bool(t.get('metadata_rejected')) for t in trials),
                    gate.get('post_compile_disagreements',0),gate.get('compiled_signature_rejections',0),rejected_model)
            if any(values):negative_chat.append((name,*values))
        add('Resultados negativos de todas as execuções concluídas, incluindo sondas; só aparecem linhas com alguma falha/recusa. Não são perdas adotadas.',
            table(['Execução','Compilações falhas','Regressões recusadas','Metadados recusados','Modelo/emissor','Assinatura emitida recusada','Assinatura prevista recusada'],negative_chat)
            if negative_chat else 'Nenhuma falha de compilação, regressão, metadado ou assinatura recusada registrada nas execuções concluídas.')
    budget_rows=[]
    for name,engine in ctx['engines']:
        if '-initial-screen-' in name:continue
        rows=list(passes(engine));trials=[t for row in rows for t in row.get('trials',[]) if t.get('incomplete_prediction')]
        budget_rows.append((name,len(trials),sum(t.get('candidate_compiler_invocations',0) for t in trials),sum(bool(t.get('accepted')) for t in trials),sum(bool(r.get('incomplete_budget_limit_reached')) for r in rows)))
    if not short:add(table(['Execução','Variantes incompletas compiladas','Invocações destas variantes','Avanços aceitos','Métodos no limite'],budget_rows))
    else:add('Variantes incompletas compiladas: '+str(sum(r[1] for r in budget_rows))+'; invocações dessas variantes: '+str(sum(r[2] for r in budget_rows))+'. Esses custos incluem sondas; ganhos não são contados duas vezes.')
    section(3)
    hypotheses=read('reports/round10-hypotheses.json')['hypotheses'] if exists('reports/round10-hypotheses.json') else []
    add(table(['Estado','Hipótese','Evidência medida'],[(h['status'],h['hypothesis'],h['evidence']) for h in hypotheses]) if hypotheses else 'Hipóteses formalmente classificadas: UNKNOWN; o diário acima registra os resultados medidos.')
    if short and hypotheses:
        add('Zero ganho permanece INCONCLUSIVA, salvo prova isolada de refutação. A classificação não identifica o fonte histórico literal.')
    if exists('reports/round10-loop-polarity-diagnosis.json'):
        diagnosis=read('reports/round10-loop-polarity-diagnosis.json')
        primary=diagnosis['compiler_source']
        check_hash(primary['path'],primary['sha256'],'Changed primary compiler source for loop diagnosis')
        for pathkey,hashkey in [('original_dump','original_dump_sha256'),('rebuilt_dump','rebuilt_dump_sha256'),('receipt','receipt_sha256')]:
            check_hash(diagnosis['provenance'][pathkey],diagnosis['provenance'][hashkey],'Changed loop diagnosis snapshot')
        add('Diagnóstico de polaridade da rodada 10, no snapshot congelado indicado pela prova: escopo limitado às fronteiras selecionadas, não a todos os desvios dos métodos. A correção sobre Act_dribble descreve um teste histórico da rodada 7, sem nova tentativa.')
        diagnostic_rows=[]
        for item in diagnosis['hypotheses']:
            state=item['status'];state=('REFUTADA' if state.startswith('REFUTED') else 'CONFIRMADA' if state.startswith('CONFIRMED') else 'INCONCLUSIVA' if state.startswith('INCONCLUSIVE') else state)
            diagnostic_rows.append((state,item['hypothesis'],item.get('evidence',item.get('correction','UNKNOWN'))))
        add(table(['Estado','Hipótese diagnosticada','Evidência / limite'],diagnostic_rows))
        if not short:
            add('Fonte do compilador consultada: '+inline(primary['path'])+'; revisão '+inline(primary['revision'])+'; SHA-256 '+inline(primary['sha256'])+'. Trechos While.DoEmit '+primary['While_DoEmit']+', Do.DoEmit '+primary['Do_DoEmit']+', For.DoEmit '+primary['For_DoEmit']+'. Commit de fabricação do smcs instalado: '+primary['installed_compiler_source_commit']+'.')
    add(model_table(ctx))
    if exists('reports/round10-control-results.json'):
        control=read('reports/round10-control-results.json')
        application=control['application']
        control_path=ctx['cfg'].get('control_extractor_validation','reports/round10-control-validation-public.json')
        extraction=ctx['control_extraction'];m=extraction['matched'];p=extraction['pending']
        add('**Extrator de controle smcs.** Corpus congelado: '+inline(extraction['snapshot'])+'. Planos e árvores completos têm denominadores distintos; uma cobertura integral das instruções não completa uma árvore ambígua.',
            table(['Métrica','Resultado'],[
                ('Árvores completas iguais / pares completos',str(m['trees_equal'])+'/'+str(m['paired_tree_complete'])+' ('+pct(m['trees_equal'],m['paired_tree_complete'])+')'),
                ('Cobertura das árvores nos corpos matched',str(m['paired_tree_complete'])+'/'+str(m['methods'])+' ('+pct(m['paired_tree_complete'],m['methods'])+')'),
                ('Planos completos iguais / completos',str(m['plans_equal'])+'/'+str(m['paired_plan_complete'])),
                ('Pendentes: IL completo / fonte completa / pares completos',str(p['il_tree_complete'])+' / '+str(p['source_tree_complete'])+' / '+str(p['paired_tree_complete'])),
                ('Parte C: diferenças antes / reescritas com árvore igual / corpos fechados',str(application['trees_different_before'])+' / '+str(application['candidate_tree_and_plan_equal_original'])+' / '+str(application['body_matches_officially_adopted'])),
                ('Parte C: bytes de IL adotados oficialmente',application['original_il_bytes_gained'])]))
        add('A árvore original não distingue formas fonte que emitem os mesmos saltos sem evidência adicional. Ownership de salto ambíguo, EH, switch, construções anônimas e outros casos sem molde provado continuam incompletos. A AST é extraída somente do fonte; planos da compilação própria autenticam o vínculo das folhas, nunca inventam estrutura fonte. A correspondência final continua sendo corpo completo e metadados no verificador.')
    add('Precisão usa somente previsões completas; cobertura usa todos os corpos matched do corpus congelado. Incompletos ou métodos gerados sem mapeamento continuam UNKNOWN. Coincidência de tipos em previsão incompleta não é prova do lowering. '+policy_text(ctx))
    if not short:
        reasons=[]
        for assembly,d in ctx['models'].items():
            groups=Counter()
            for reason,count in d['matched_incomplete_classifications'].items():groups[reason.split(':',1)[0]]+=count
            reasons.extend((assembly,reason,count) for reason,count in groups.most_common())
        add('Limitações dos modelos; motivos não exclusivos, sem crédito por previsões vazias:',table(['Assembly','Motivo agrupado','Ocorrências'],reasons))
        add(table(['Modelo em uso','Fonte','Client','Solver','Validação final','Corpus final'],[(a,ctx['cfg']['models'][a]['source'],ctx['cfg']['models'][a]['client'],ctx['cfg']['models'][a]['solver'],ctx['cfg'].get('final_model_measurements',{}).get(a,ctx['cfg']['models'][a])['validation'],ctx['cfg'].get('final_model_measurements',{}).get(a,ctx['cfg']['models'][a])['corpus_baseline']) for a in ASSEMBLIES]))
    section(4)
    distribution=[(a,size,row['methods'],row['original_il_bytes']) for a in ASSEMBLIES for size,row in final[a]['size_distribution'].items()]
    add(table(['Assembly','Faixa de IL original','Corpos pendentes','Bytes de IL'],distribution))
    add('Pendentes com assinatura de locais igual: '+ '; '.join(a+f" {final[a]['pending_local_signatures_equal']}/{final[a]['divergent_bodies']} (diferente: {final[a]['pending_local_signatures_different']})" for a in ASSEMBLIES)+'.')
    add('Corpos já iguais, retidos por vínculo/metadados gerados: '+str(len(ctx['held']))+'; '+str(sum(v[2] for v in ctx['held']))+' bytes. Esses corpos não entram no histograma dos divergentes.')
    if ctx['held'] and not short:add(table(['Assembly','ID','Bytes'],ctx['held']))
    add('Peso de um par = soma do tamanho integral dos métodos nele; não representa quantidade de bytes diferentes. Alinhamento e rótulos servem ao diagnóstico, sem normalizar o verificador. Seguem os cinco pares de opcodes mais pesados de cada compilador, com IL original e recompilado lado a lado.')
    for assembly in ASSEMBLIES:
        groups=sorted(final[assembly]['alignment']['opcode_histogram'],key=lambda g:(-g['original_il_bytes'],-g['methods'],g['pair']))
        if not groups:add(assembly+': não há pares divergentes.');continue
        if not short:add(table([assembly+' — par','Métodos','Peso IL'],[(inline(g['pair']),g['methods'],g['original_il_bytes']) for g in groups]))
        for number,group in enumerate(groups[:5],1):
            example=group['example']
            add(f"**{assembly} #{number}: "+inline(group['pair'])+f" — {group['methods']} métodos, {group['original_il_bytes']} bytes.** Exemplo: "+inline(example['stable_id'])+'.',
                attempts_text(attempt_summary(ctx,assembly,group),short),table(['IL original','IL recompilado'],context_rows(example,short)))
            if not short and (example.get('original_exception_regions') or example.get('rebuilt_exception_regions')):
                add('EH original: '+inline(json.dumps(example['original_exception_regions'],ensure_ascii=False))+'; recompilado: '+inline(json.dumps(example['rebuilt_exception_regions'],ensure_ascii=False))+'.')
    section(5)
    for assembly in ASSEMBLIES:
        manifest=ctx['manifests'][assembly];previous=read('build/round10/baseline/'+assembly+'/build-manifest.json')
        version='NuGet Microsoft.Net.Compilers 1.1.0; csc.exe 1.1.0.51109' if assembly=='KairoLibrary' else 'smcs.exe 2.6.5.0 do Unity 5.3.4f1'
        flags=lambda m:[f for f in m['flags'] if not f.startswith(('/out:','-out:'))]
        ref_selection=lambda m:[(r['path'],r['sha256'] if Path(r['path']).name!='KairoLibrary.dll' else 'reconstructed-project-input') for r in m['references']]
        changed=manifest['compiler_sha256']!=previous['compiler_sha256'] or flags(manifest)!=flags(previous) or ref_selection(manifest)!=ref_selection(previous)
        add(assembly+': '+version+'; compilador SHA-256 '+inline(manifest['compiler_sha256'])+'. Mudou compilador/opções/seleção de referências nesta rodada: '+('SIM' if changed else 'NÃO')+'.',
            'Opções: '+inline(' '.join(flags(manifest)))+'.')
        if assembly=='Assembly-CSharp':add('A KairoLibrary referenciada é recompilada com o projeto; seu hash de artefato pode mudar, sem troca da seleção de frameworks ou do compilador.')
        if short:add('Referências em ordem: '+', '.join(Path(r['path']).name for r in manifest['references'])+'.')
        else:
            add(table(['Referência (ordem de compilação)','SHA-256'],[(r['path'],inline(r['sha256'])) for r in manifest['references']]))
            if assembly=='Assembly-CSharp':add('Runtime Mono: '+inline(manifest['runtime'])+'; SHA-256 '+inline(manifest['runtime_sha256'])+'. Stdlib: '+inline(manifest['stdlib_binding'])+'. Ambiente do subprocesso: '+inline(json.dumps(manifest['scoped_environment'],ensure_ascii=False))+'.')
    add('Compilador histórico exato, comando histórico integral e hashes das referências originalmente usadas: UNKNOWN. Os candidatos acima têm hashes fixados e prova de corpos; isso não identifica sua fabricação histórica. Roslyn moderno compila auxiliares e fornece AST, não os assemblies alvos. PDB/MDB auxiliam apenas o mapeamento. Nomes escolhidos para locais não são apresentados como nomes originais.')
    section(6)
    add(table(['Assembly','Tipos com metadados iguais','Declarações sem corpo aprovadas','Ordem de tipos','Metadados globais','Bytes brutos'],[
        (a,str(ctx['metadata'][a]['types_equal'])+'/'+str(ctx['metadata'][a]['types_total']),str(ctx['metadata'][a]['declarations_proved'])+'/'+str(ctx['metadata'][a]['declarations_total']),ctx['metadata'][a]['type_order_equal'],ctx['metadata'][a]['whole_structural_metadata_equal'],ctx['metadata'][a]['raw_tables_heaps_pe_identity']) for a in ASSEMBLIES]))
    add('A comparação abrange ordem de tipos e membros, parâmetros, atributos personalizados, layout, constantes, recursos e referências globais; o guard de identidade confere o SHA do original e do build atual. A medição bruta KairoLibrary da Parte B descreve a baseline inicial; o estado de identidade bruta do verificador acima continua separado dessa medição. Na reconstrução das DLLs, nenhum byte original é inserido no resultado, nenhuma classe gerada é escrita à mão e nenhum assembly é normalizado. O APK funcional da Parte A contém os demais arquivos originais conforme solicitado e não é a reconstrução final.')
    audit=read('config/dll-audit.json');externals=[r for r in audit if r['category']=='externo'];official=[r for r in externals if r.get('official_editor_hash_matches')]
    unity=next((r for r in externals if r['file']=='UnityEngine.dll'),None)
    add('Externos gerenciados com correspondência oficial registrada: '+str(len(official))+'/'+str(len(externals))+': '+', '.join(r['file'] for r in official)+'. UnityEngine.dll: '+(unity.get('external_verification','UNKNOWN')+'; SHA-256 '+inline(unity['sha256']) if unity else 'UNKNOWN')+'. A variante é Android/Mono do Unity 5.3.4f1; evidência histórica, não nova decompilação.')
    if exists('reports/UnityEngine-android-comparison.json'):
        native=read('reports/UnityEngine-android-comparison.json')['native_library_matches']
        add('Externos nativos com correspondência oficial registrada: '+str(sum(bool(n['official_module_matches']) for n in native))+'/'+str(len(native))+'; '+', '.join(n['apk_path'] for n in native)+'. DEX/Java permanece fora do escopo.')
    fields=read('config/toolchain.json')['non_deterministic_exceptions']
    add('Campos não determinísticos comprovados por pares de recompilações anteriores (não normalizados): '+ '; '.join(a+' — '+', '.join(f['field'] for f in fields if f['assembly']==a) for a in ASSEMBLIES)+'. Determinismo histórico do original: UNKNOWN.')
    if not short:add(table(['Assembly','Campo','Prova','Normalização aplicada'],[(f['assembly'],f['field'],f['evidence'],f['normalization_applied']) for f in fields]))
    section(7)
    blockers=stop.get('blockers','Dependências de instalação/arquivo/decisão do usuário: UNKNOWN (não registradas no recibo de parada).')
    if isinstance(blockers,list):blockers='; '.join(map(str,blockers)) or 'Nenhum bloqueio de instalação, arquivo ou decisão do usuário registrado.'
    add(blockers,
        'Parada após '+str(sum(c.get('phase')=='D' for c in cycles))+' ciclos autônomos D e '+str(sum(c.get('phase')!='D' for c in cycles))+' etapas A-C; sequência autônoma final sem novo matched: '+str(ctx['journal'].get('consecutive_zero_cycles','UNKNOWN'))+'. Limites configurados: '+str(ctx['cfg']['maximum_cycles'])+' ciclos autônomos, '+str(ctx['cfg']['consecutive_zero_stop'])+' sem ganho consecutivos com fila vazia; cota Kairo: '+ctx['cfg']['kairo_cycle_quota']+'.')
    if ctx['unfinished']:add('Recibos de execução incompletos preservados, excluídos das somas de execuções concluídas: '+', '.join(ctx['unfinished'])+'. Motivo detalhado: UNKNOWN se não houver nota de recuperação correspondente.')
    section(8)
    recommendations=stop.get('recommendations',[])
    if recommendations:add('\n'.join(str(i)+'. '+str(row) for i,row in enumerate(recommendations[:3],1)))
    else:add('Próximas ações e estimativas de métodos resolvíveis: UNKNOWN; o recibo de parada não as registrou. Os pesos da seção 4 medem potencial, não previsão de ganho.')
    return '\n'.join(output).rstrip()+'\n'


def main():
    ctx=load_context()
    full=render(ctx,short=False);chat=render(ctx,short=True)
    # Construct and validate both strings before publishing either artifact.
    for report in [full,chat]:
        if re.findall(r'^## (.+)$',report,re.M)!=list(SECTIONS):raise ValueError('Report section contract violated')
    targets=[('docs/notes/round10-report.md',full),('docs/notes/round10-report-chat.md',chat)]
    for name,text in targets:
        path=ROOT/name;path.parent.mkdir(parents=True,exist_ok=True);path.write_text(text,encoding='utf-8',newline='\n')
    print(json.dumps({'reports':[{'path':name,'sha256':sha(ROOT/name)} for name,_ in targets]},ensure_ascii=True))


if __name__=='__main__':main()
