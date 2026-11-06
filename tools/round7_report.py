"""Render the standalone eight-section report from fresh measured evidence."""
from collections import Counter
import csv
import json
from pathlib import Path
import re
from apply_il_rules import receipt_is_fresh
from compare_kairo import sha

ROOT=Path(__file__).resolve().parents[1]
ASSEMBLIES=('KairoLibrary','Assembly-CSharp')


def read(path):return json.loads((ROOT/path).read_text(encoding='utf-8-sig'))
def table(headers,rows):
    return '\n'.join(['| '+' | '.join(headers)+' |','| '+' | '.join('---' for _ in headers)+' |']+
        ['| '+' | '.join(str(v).replace('|','\\|').replace('\n','<br>') for v in row)+' |' for row in rows])
def pct(n,d):return f'{100*n/d:.6f}%'
def inline(text):
    text=str(text);fence='``' if '`' in text else '`'
    return fence+text+fence
def counts(m):
    return f"{m['matched_methods']}/{m['total_methods']} ({pct(m['matched_methods'],m['total_methods'])}); IL {m['matched_original_il_bytes']}/{m['original_total_il_bytes']} bytes ({pct(m['matched_original_il_bytes'],m['original_total_il_bytes'])})"
def operand(value):
    if value is None:return ''
    if isinstance(value,dict):
        if 'local' in value:return str(value['local'])
        if 'labels' in value:return ','.join(value['labels'])
        value=next(iter(value.values())) if len(value)==1 else json.dumps(value,ensure_ascii=False)
    result=str(value)
    return re.sub(r' @ [^,<>]+, Version=[^,]+, Culture=[^,]+, PublicKeyToken=[a-zA-Z0-9]+','',result)
def il(row):
    value=row.get('operand')
    rendered=json.dumps(value,ensure_ascii=False) if row['opcode']=='ldstr' else operand(value)
    return f"IL_{row['offset']:04x}: "+row['opcode']+' '+rendered
def passes(engine):
    for rows in engine.get('previous_passes',{}).values():
        yield from rows
    yield from engine['methods']


def main():
    baseline=read('reports/round7-baseline-measurement.json')
    final=read('reports/round7-final-measurement.json')
    inventory=list(csv.DictReader((ROOT/'config/inventory.csv').open(encoding='utf-8-sig',newline='')))
    apk_hashes={r['apk_sha256'] for r in inventory if r.get('apk_sha256')}
    if len(apk_hashes)!=1:raise ValueError('Ambiguous original APK identity')
    original_targets={Path(r['caminho']).name:r['sha256'] for r in inventory
        if r['categoria']=='alvo' and r['caminho'].endswith('.dll')}
    if final['ledger_sha256']!=sha(ROOT/'config/methods.csv'):raise ValueError('Stale final measurement')
    baseline_ledger=list(csv.DictReader((ROOT/'build/round7/baseline/methods.csv').open(encoding='utf-8-sig',newline='')))
    current_ledger=list(csv.DictReader((ROOT/'config/methods.csv').open(encoding='utf-8-sig',newline='')))
    losses={}
    body_losses={}
    for a in ASSEMBLIES:
        receipt=read('build/verification-'+a+'.json')
        metadata=read('reports/'+a+'-metadata.json')
        if metadata['original_sha256']!=original_targets[a+'.dll'] or not metadata['whole_structural_metadata_equal']:
            raise ValueError('Original identity/global metadata mismatch: '+a)
        if not receipt_is_fresh(ROOT,receipt) or any(receipt.get(k) for k in ('regressions','body_regressions','type_regressions')):raise ValueError('Stale/regressive official receipt '+a)
        provenance=final['assemblies'][a]['provenance']
        for path_key,hash_key in [('original_dump','original_dump_sha256'),('rebuilt_dump','rebuilt_dump_sha256'),('receipt','receipt_sha256')]:
            if sha(ROOT/provenance[path_key])!=provenance[hash_key]:raise ValueError('Stale final input '+a+' '+path_key)
        if receipt['build_sha256']!=provenance['build_sha256']:raise ValueError('Final measurement describes another build')
        before={r['id'] for r in baseline_ledger if r['assembly']==a and r['estado']=='matched'}
        after={r['id'] for r in current_ledger if r['assembly']==a and r['estado']=='matched'}
        losses[a]=sorted(before-after)
        old_receipt=read('build/round7/baseline/'+a+'/verification.json')
        body_losses[a]=[key for key,p in old_receipt['methods'].items()
            if p.get('body_equal') and not receipt['methods'].get(key,{}).get('body_equal')]
        if losses[a] or body_losses[a]:raise ValueError('Regression from round baseline: '+a)
    journal=read('reports/round7-cycle-journal.json') if (ROOT/'reports/round7-cycle-journal.json').exists() else {'cycles':[]}
    stop=read('reports/round7-stop.json')
    engines=[]
    for path in sorted((ROOT/'reports').glob('round7-*-engine.json')):
        doc=read(path)
        if doc.get('completed'):engines.append((path.name,doc))
    # Published probe results remain distinct from adopted run/cycle results.
    adopted={read(p).get('run_label',p.name.removeprefix('round7-').removesuffix('-adoption.json')) for p in (ROOT/'reports').glob('round7-*-adoption.json') if read(p).get('official_verified')}
    out=['## 1. PROGRESSO','',
        'APK: `net.kairosoft.android.soccer_en_2.0.2-APK_Award.apk`; reconstrução gerenciada Unity 5.3.4f1/Mono. Linha anterior: commit `cdd0f7f3b8dda09f296a7cfa16cd00236a3346ae`. `matched` exige cabeçalho, maxstack, locais, IL e regiões de exceção iguais; somente identidades de tokens são resolvidas. Declarações sem corpo entram em métodos, não em bytes de IL.','']
    out += [table(['Artefato original','SHA-256'],[
        ('APK',inline(next(iter(apk_hashes)))),
        *[(a+'.dll',inline(original_targets[a+'.dll'])) for a in ASSEMBLIES]]),'']
    rows=[]
    for a in ASSEMBLIES:
        b=baseline['assemblies'][a];f=final['assemblies'][a]
        rows.append((a,counts(b),counts(f),f['matched_methods']-b['matched_methods'],f['matched_original_il_bytes']-b['matched_original_il_bytes'],len(losses[a])))
    out += [table(['Assembly','Anterior','Atual','Métodos ganhos','Bytes ganhos','Regressões'],rows),'',
            'Total atual: '+str(sum(final['assemblies'][a]['matched_methods'] for a in ASSEMBLIES))+'/5245 métodos; '+str(sum(final['assemblies'][a]['matched_original_il_bytes'] for a in ASSEMBLIES))+'/871497 bytes de IL.', '',
            'Retiradas de estado matched desde cdd0f7f: '+str(sum(map(len,losses.values())))+'. Corpos antes iguais que deixaram de bater: '+str(sum(map(len,body_losses.values())))+'. As duas medidas são calculadas por ID, não inferidas da contagem líquida.','',
            '**Motivo da parada:** '+stop['reason']+'.','', '## 2. O QUE FOI FEITO NESTA RODADA','',
            'As linhas abaixo distinguem sondas e execuções adotadas. Ganho do motor significa corpo completo aprovado; avanços de prefixo não contam como método concluído. O corpo ganho é atribuído à última transformação que o fechou; uma operação preparatória pode ter zero nessa coluna e ainda ter permitido o resultado. Métodos auxiliares liberados pelo verificador são incluídos no progresso oficial.','']
    rows=[]
    for name,d in engines:
        rules=sorted({key.rsplit(':',1)[0] for key in d.get('transformations',{})})
        for rule in rules:
            c=d['transformations'];rows.append((name,rule,c.get(rule+':attempts',c.get(rule+':candidates',0)),c.get(rule+':accepted_steps',0),c.get(rule+':body_gains',0),c.get(rule+':IL_gains',0),0))
    out += [table(['Execução','Transformação','Tentativas compiladas','Avanços','Corpos ganhos','Bytes ganhos','Perdidos'],rows),'',
            'Execuções em `probes` são testes isolados e não devem ser somadas novamente às aplicações completas. Modelos rejeitam candidatos incompletos ou com assinatura prevista diferente da original antes da compilação.','']
    negatives=[]
    for name,d in engines:
        history=list(passes(d))
        trials=[t for m in history for t in m.get('trials',[])]
        gate=d.get('slot_model_gate',{})
        negatives.append((name,str(len(d['methods']))+' / '+str(len(history)),dict(Counter(m['stop_reason'] for m in history)),
            gate.get('incomplete',0),gate.get('wrong_signature',0),
            sum(not t['compile_pass'] or t.get('full_compile_pass') is False for t in trials),
            sum(bool(t.get('body_regressions')) for t in trials),sum(bool(t.get('metadata_rejected')) for t in trials),
            gate.get('post_compile_disagreements',0)))
    out += [table(['Execução','Métodos / passes','Paradas por passe','Modelo incompleto','Assinatura prevista errada','Compilações falhas','Regressões de candidatos rejeitados','Metadados rejeitados','Modelo/emissor discordam'],negatives),'',
            'Perdas adotadas: zero. Uma regressão de candidato, quando registrada, foi rejeitada antes da adoção; não é escondida pela coluna de ganhos. Os recibos preservam comandos, erros, offsets, trechos e todas as tentativas.','', '**Diário dos ciclos autônomos**','']
    out += [table(['Ciclo','Compilador / par ponderado','Hipótese','Resultado','Métodos','Bytes'],[
        (c['cycle'],c['assembly']+' / '+c['pair'],c['hypothesis'],c['result'],c['methods_gained'],c['il_bytes_gained']) for c in journal['cycles']]),'',
        '## 3. HIPÓTESES','']
    publication=ROOT/'reports/round7-publication-events.json'
    if publication.exists():
        events=read(publication)['events']
        out[-2:-2]=['Publicação: '+ '; '.join('ciclo '+str(e['cycle'])+' — '+e['first_push']+'; repetição: '+e['retry_push'] for e in events)+'. Nenhuma alteração de fonte foi necessária para repetir o envio.','']
    if (ROOT/'reports/round7-cycle12-formatting-repair.json').exists():
        out[-2:-2]=['No ciclo 12, git diff --cached --check recusou um espaço final na linha de uma chave em DebugForm.cs. O espaço foi removido sem mudar linhas ou finais de linha; a prova de todos os corpos e metadados foi repetida e o commit foi publicado. A falha de formatação e os hashes antes/depois estão em reports/round7-cycle12-formatting-repair.json.','']
    if (ROOT/'reports/round7-cycle16-preparation-retry.json').exists():
        out[-2:-2]=['A preparação do ciclo 16 recusou o seletor abreviado LoadOptimize por encontrar duas sobrecargas. Nenhum candidato havia sido compilado. O teste prosseguiu com a assinatura InputStream completa, reutilizando o snapshot imutável somente após conferir HEAD, configurações, fontes e recibos.','']
    if (ROOT/'reports/round7-cycle20-disk-recovery.json').exists():
        out[-2:-2]=['No ciclo 20, faltou espaço durante a cópia do snapshot final. Foram removidos 3310 binários temporários de candidatos (902658048 bytes) e 1118 arquivos de cache de metadados regenerável (7575228256 bytes). Fontes, relatórios oficiais, originais e snapshots congelados foram preservados. A cópia incompleta não foi sobrescrita; o snapshot completo e verificado está em build/round7/cycles/cycle20/after-verified. A verificação completa e a prova do índice Git foram repetidas antes da publicação.','']
    hypothesis=read('reports/round7-hypotheses.json') if (ROOT/'reports/round7-hypotheses.json').exists() else {'hypotheses':[]}
    out += [table(['Estado','Hipótese','Evidência'],[(h['status'],h['hypothesis'],h['evidence']) for h in hypothesis['hypotheses']]),'',
        'FastVector: o original grava `<>7__wrap1:!0[]` com `stfld` no offset 35; `<>7__wrap2:Int32` é o índice sintético. O campo é a cópia da coleção do `foreach`, cujo escopo atravessa os `yield return` e precisa sobreviver às retomadas do iterador. Ele existe nos dois lados porque os dois fontes possuem esse foreach. A reconstrução acrescentava antes dele `T[] array = elementData;`: esse alias só era usado para iniciar o foreach, não precisava sobreviver ao yield e ficava em um slot local adicional. Usar diretamente `foreach (T val in elementData)` removeu o slot redundante e preservou os campos; o corpo MoveNext completo bateu. A declaração histórica literal permanece UNKNOWN; não foi necessário escrever um campo ou classe gerada.','',
        '**Modelos de slots — validação contra a linha de base**','']
    rows=[]
    for name,file in [('smcs','round7-slot-model-validation.json'),('Roslyn 1.1','round7-roslyn-model-validation.json')]:
        d=read('reports/'+file)
        assembly='Assembly-CSharp' if name=='smcs' else 'KairoLibrary'
        stem='SlotModel' if name=='smcs' else 'RoslynModel'
        client='round7_slot_model.py' if name=='smcs' else 'round7_roslyn_model.py'
        if sha(ROOT/('tools/Round7'+stem+'.cs'))!=d['source_sha256'] or sha(ROOT/('tools/'+client))!=d['client_sha256']:
            raise ValueError('Model validation source/client changed: '+name)
        if sha(ROOT/('build/round7/baseline/'+assembly+'/verification.json'))!=d['baseline_receipt_sha256']:
            raise ValueError('Model validation baseline changed: '+name)
        rows.append((name,d['matched_bodies'],d['matched_source_mapped'],d['matched_source_unmapped'],d['matched_model_complete'],d['matched_complete_exact'],pct(d['matched_complete_exact'],d['matched_model_complete']),pct(d['matched_model_complete'],d['matched_bodies']),d['matched_mapped_incomplete']))
    out += [table(['Modelo','Corpos matched','Mapeados','Gerados/não mapeados','Assinaturas completas','Exatas completas','Precisão','Cobertura de completas','Mapeados incompletos'],rows),'',
        'A taxa das previsões completas não mede cobertura total. Casos incompletos e gerados sem mapeamento permanecem UNKNOWN; não recebem aprovação por assinatura vazia. O smcs resolve variáveis em ordem de inserção do bloco; nomes não determinam slots. O Roslyn usa ordem de declaração e temporários por assinatura, mas o escalonador de pilha pode eliminar locais. Capturas, lowering e escalonamento não modelados são rejeitados explicitamente. As notas `docs/notes/round7-slot-model.md` e `docs/notes/round7-roslyn-model.md` identificam os arquivos e trechos dos compiladores consultados.','',
        'Trechos consultados: Roslyn, snapshot `4cb203dab89e1542cdf7d88cb6d32767bc0e5460`, `src/Compilers/CSharp/Portable/CodeGen/EmitStatement.cs:594–613`: `foreach (var local in block.Locals)` chama `DefineLocal(...)` antes de `foreach (var statement in block.Statements) EmitStatement(statement)`. Mono/Unity, snapshot `b6587926c0f146c8b12985744f33a13b39a22d57`, `mcs/mcs/statement.cs:1225–1239`: `if (HoistedVariableReference != null) return;`, seguido da declaração do local quando `builder == null`. Em `ExplicitBlock.Emit`, o storey é instanciado na entrada do bloco, após os locais ordinários. A correspondência exata desses snapshots à fabricação dos executáveis instalados permanece UNKNOWN.','',
        '## 4. PENDENTES POR GRUPO','']
    rows=[]
    for a in ASSEMBLIES:
        d=final['assemblies'][a]
        for size,item in d['size_distribution'].items():rows.append((a,size,item['methods'],item['original_il_bytes']))
    out += [table(['Assembly','Tamanho original de IL','Corpos divergentes','Bytes'],rows),'']
    ledger=list(csv.DictReader((ROOT/'config/methods.csv').open(encoding='utf-8-sig',newline='')))
    ledger_by_id={row['id']:row for row in ledger}
    held=[]
    for a in ASSEMBLIES:
        original={m['stable_id']:m for m in read('build/round7/baseline/'+a+'/original-methods.json')['selected_methods']}
        receipt=read('build/verification-'+a+'.json')
        for ledger_id,proof in receipt['methods'].items():
            if proof['state']!=ledger_by_id[ledger_id]['estado']:raise ValueError('Ledger/receipt state disagreement')
            identity=proof['stable_id']
            if proof.get('body_equal') and proof['state']!='matched' and 'raw_body_hex' in original[identity]:
                held.append((a,identity,original[identity]['header']['code_size']))
    out += ['Corpos já iguais, mas ainda sem promoção por vínculo gerado: '+str(len(held))+' ('+str(sum(r[2] for r in held))+' bytes).','',table(['Assembly','ID','Bytes'],held),'']
    app=final['assemblies']['Assembly-CSharp']
    out += [f"smcs pendentes com assinatura de locais igual: {app['pending_local_signatures_equal']}/{app['divergent_bodies']}; diferente: {app['pending_local_signatures_different']}.",'',
        'Histograma por alinhamento de sequências; destinos viram rótulos apenas no diagnóstico. Peso = soma do tamanho integral do IL original dos métodos do grupo, não quantidade de bytes divergentes. Agrupamento abaixo por par de opcodes; operandos exatos, todas as classes e cada método estão em `reports/round7-final-measurement.json`.','']
    for a in ASSEMBLIES:
        d=final['assemblies'][a];groups=d['alignment']['opcode_histogram']
        out += ['**'+a+' — todos os pares restantes**','',table(['Par alinhado','Métodos','Peso em bytes'],[(g['pair'],g['methods'],g['original_il_bytes']) for g in groups]),'']
        exact_groups=d['alignment']['exact_histogram']
        out += ['Os cinco pares com operandos exatos mais pesados são apresentados também; os endereços de saltos tornam esse agrupamento mais fragmentado que o de opcodes.','',
            table(['Par exato','Métodos','Peso em bytes'],[(inline(g['pair']),g['methods'],g['original_il_bytes']) for g in exact_groups[:5]]),'']
        examples=[('opcodes',n,g) for n,g in enumerate(groups[:5],1)]
        examples += [('operandos exatos',n,g) for n,g in enumerate(exact_groups[:5],1)]
        for grouping,n,g in examples:
            example=g['example'];identity=example['stable_id'];left=example['original_context'][:15];right=example['rebuilt_context'][:15]
            attempts=Counter()
            reasons=Counter()
            model_rejections=Counter()
            exhausted_candidates=0
            for _,engine in engines:
                for row in passes(engine):
                    if row['id'] in g['method_ids']:
                        attempts.update(t['rule'] for t in row.get('trials',[]))
                        reasons[row['stop_reason']]+=1
                        for rejection in row.get('model_rejections',[]):model_rejections.update(rejection)
                        exhausted_candidates+=sum(f.get('removed_legacy_candidates',0) for f in row.get('cycle_repertoire_filters',[]))
            pair_label=g['pair'] if grouping=='opcodes' else example['opcode_pair']
            out += [f"**{a}, par {n} ({grouping}): "+inline(pair_label)+f"; {g['methods']} métodos / {g['original_il_bytes']} bytes.** Exemplo: "+inline(identity)+'.','',
                'Já compilado no grupo: '+(', '.join(f'{rule}: {count}' for rule,count in attempts.items()) or 'nenhum candidato chegou à compilação')+'. Recusas e limites antes de compilar: '+str(dict(model_rejections))+'. Candidatos antigos descartados por busca já esgotada: '+str(exhausted_candidates)+'. Nomes individuais das regras recusadas pelo modelo: UNKNOWN (não foram registrados por candidato). Paradas: '+str(dict(reasons))+'.','',
                table(['IL original','IL recompilado'],[(inline(il(left[i])) if i<len(left) else '',inline(il(right[i])) if i<len(right) else '') for i in range(max(len(left),len(right)))]),'']
            if example.get('original_exception_regions') or example.get('rebuilt_exception_regions'):
                out += ['Regiões de exceção originais: `'+json.dumps(example['original_exception_regions'],ensure_ascii=False)+'`; recompiladas: `'+json.dumps(example['rebuilt_exception_regions'],ensure_ascii=False)+'`.','']
    out += ['## 5. TOOLCHAIN','',
        'Nenhum compilador ou referência de produção foi trocado nesta rodada. Identidade exata da distribuição original, comando/opções históricos completos e arquivos de referência exatos usados originalmente: UNKNOWN. As identidades declaradas em AssemblyRef são observáveis; isso não prova o hash de cada arquivo histórico. A marca Repro original está ausente; o modo determinístico original permanece UNKNOWN. As recompilações selecionadas são não determinísticas, conforme as duas compilações e seis campos documentados.','']
    for a in ASSEMBLIES:
        manifest=read('build/round7/baseline/'+a+'/build-manifest.json')
        version='Microsoft.Net.Compilers 1.1.0; csc.exe 1.1.0.51109' if a=='KairoLibrary' else 'smcs.exe 2.6.5.0 distribuído no Unity 5.3.4f1'
        out += ['**'+a+'**: '+version+'; SHA-256 `'+manifest['compiler_sha256']+'`.','',
            'Opções: `'+ ' '.join(f for f in manifest['flags'] if not f.startswith(('/out:','-out:')))+'`.','',
            'Referências, na ordem: '+', '.join('`'+r['path']+'` (SHA-256 `'+r['sha256']+'`)' for r in manifest['references'])+'.','']
        if a=='Assembly-CSharp':
            out += ['Runtime Mono: '+inline(manifest['runtime'])+'; SHA-256 '+inline(manifest['runtime_sha256'])+'. Ligação da biblioteca padrão: '+inline(manifest['stdlib_binding'])+'. O mscorlib implícito é o perfil Unity 2.0.5.0, conferido contra o APK. Ambiente somente do subprocesso: '+inline(json.dumps(manifest['scoped_environment'],ensure_ascii=False))+'.','']
    out += ['O Roslyn moderno compila os auxiliares e fornece AST/semântica. Não emite os assemblies alvos. PDB Roslyn e MDB smcs auxiliam o mapeamento; a comparação integral continua independente dos símbolos. Nenhum nome escolhido para local é apresentado como nome original.','',
        '## 6. COMPONENTES EXTERNOS E METADADOS','',
        'Os 466 tipos continuam com metadados estruturais iguais (KairoLibrary 337/337; Assembly-CSharp 129/129): ordem de tipos e membros, parâmetros, atributos, layout, constantes e recursos. A comparação estrutural global também passa, incluindo referências de assemblies e módulos, atributos e recursos. As 82 declarações sem corpo continuam aprovadas. Tabelas/heaps/PE brutos: NOT_EVALUATED nesta rodada; a distribuição atual das diferenças brutas é UNKNOWN. Os hashes integrais dos arquivos diferem. Nenhuma normalização ou correção baseada em bytes brutos foi feita.','',
        'UnityEngine.dll corresponde em todos os 970.752 bytes à variante oficial Android/Mono do Unity 5.3.4f1 (SHA-256 `7cc901371cc77e103d00b5845e8285f85fbdf36bb47ccf3d824f94c4ead32421`). As outras seis DLLs externas verificadas são Mono.Security, System.Core, System, UnityEngine.Networking, UnityEngine.UI e mscorlib. Os seis externos nativos são libmain.so, libmono.so e libunity.so, nas arquiteturas armeabi-v7a e x86; permanecem conferidos contra a distribuição oficial. DEX/Java permanece fora do escopo.','',
        'Exceções não determinísticas, comprovadas anteriormente por duas compilações do mesmo fonte: KairoLibrary — Module.Mvid, COFF.TimeDateStamp, IMAGE_DEBUG_DIRECTORY.TimeDateStamp e CodeView.RSDS.PdbGuid; Assembly-CSharp — Module.Mvid e COFF.TimeDateStamp. São seis campos; nenhum outro é normalizado.','',
        '## 7. BLOQUEIOS','',stop.get('blockers','Nenhuma instalação, arquivo adicional ou decisão do usuário foi necessária. O limite técnico atual é a cobertura dos modelos e das transformações: UNKNOWN não autoriza candidato.'),'',
        '## 8. SUA RECOMENDAÇÃO','']
    recommendations=stop.get('recommendations',[])
    out += [f"{i}. {r}" for i,r in enumerate(recommendations,1)]
    target=ROOT/'docs/notes/round7-report.md';target.parent.mkdir(parents=True,exist_ok=True)
    target.write_text('\n'.join(out)+'\n',encoding='utf-8',newline='\n')
    print(json.dumps({'report':str(target.relative_to(ROOT)),'sha256':sha(target)}))


if __name__=='__main__':main()
