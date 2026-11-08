"""Eight-section Round11 report from fresh receipts, including failed/partial work."""
from collections import Counter
import csv
import json
from pathlib import Path
import sys
from compare_kairo import sha
from apply_il_rules import receipt_is_fresh
from round11_controller import BASE_COMMIT, d_rows, stop_reason, validate_config
from round10_report import table, counts, il, inline, context_rows, attempt_summary, compact_counter, pct

ROOT = Path(__file__).resolve().parents[1]
ASSEMBLIES = ('KairoLibrary', 'Assembly-CSharp')
SECTIONS = ('1. PROGRESSO', '2. O QUE FOI FEITO NESTA RODADA', '3. HIPÓTESES',
            '4. PENDENTES POR GRUPO', '5. TOOLCHAIN', '6. COMPONENTES EXTERNOS E METADADOS',
            '7. BLOQUEIOS', '8. SUA RECOMENDAÇÃO')
APK_CONTRACT_NOTE = ('Critério final do APK: payload comprimido idêntico para todas as entradas salvo as duas DLLs e os portadores exatos da assinatura; preservar ordem, método e alinhamento. Comparar as duas DLLs descomprimidas e integralmente, permitindo somente os seis campos não determinísticos comprovados. Exceções ZIP originais de assinatura: META-INF/MANIFEST.MF, META-INF/CERT.SF e META-INF/CERT.RSA; eventual APK Signing Block tratado separadamente. Não copiar nem forjar a assinatura original. Encoder histórico exato: UNKNOWN.')
C6_SOURCE_NOTE = ('C6: Roslyn revisão 4cb203dab89e1542cdf7d88cb6d32767bc0e5460, src/Compilers/CSharp/Portable/Lowering/LocalRewriter/LocalRewriter_IfStatement.cs:40–63 emite BoundConditionalGoto(rewrittenCondition.Syntax, rewrittenCondition, false, afterif); CodeGen/EmitStatement.cs:364–385 só dobra quando condition.ConstantValue != null. Um local Boolean não const de uso único conserva o branch e pode ser agendado na pilha; if(true) e const bool foram refutados nas fixtures. Seb.Draw fechou com esse local, no c09.')
EXTERNALS_NOTE = ('Provas externas preservadas: sete DLLs gerenciadas e seis bibliotecas nativas coincidem com a distribuição oficial Unity 5.3.4f1. UnityEngine.dll corresponde à variante AndroidPlayer/Variations/mono/Managed, SHA-256 7cc901371cc77e103d00b5845e8285f85fbdf36bb47ccf3d824f94c4ead32421; libunity ARMv7/x86 corresponde a Mono Release. Não houve nova auditoria ou decompilação de externos nesta rodada.')


def read(path):
    return json.loads((ROOT / path).read_text(encoding='utf-8-sig'))


def check(path, digest):
    if sha(ROOT / path) != digest:
        raise ValueError('Stale report evidence: ' + str(path))


def check_measurement(document, ledger):
    check(ledger, document['ledger_sha256'])
    if document['baseline_commit'] != BASE_COMMIT:
        raise ValueError('Wrong Round11 baseline')
    for assembly in ASSEMBLIES:
        p = document['assemblies'][assembly]['provenance']
        for path, digest in [('original_dump', 'original_dump_sha256'),
                             ('rebuilt_dump', 'rebuilt_dump_sha256'), ('receipt', 'receipt_sha256')]:
            check(p[path], p[digest])


def validate_adoption(adoption):
    if not adoption.get('official_verified') or adoption.get('matched_losses'):
        raise ValueError('Cycle lacks full official zero-regression proof')
    if any(row.get('matched_losses') for row in adoption.get('assemblies', {}).values()):
        raise ValueError('Cycle contains assembly-level matched losses')


def load_context():
    if not (ROOT / 'reports/round11-stop.json').is_file():
        raise RuntimeError('Round11 has no stop receipt; no report was written')
    cfg, stop = read('config/round11-protocol.json'), read('reports/round11-stop.json')
    validate_config(cfg)
    if not stop.get('new_reconstruction_work_stopped') or not stop.get('reason'):
        raise ValueError('Missing measured stop')
    if not cfg.get('report_schema_reviewed'):
        raise ValueError('Round11 report schema has not been reviewed')
    baseline, final = read(cfg['baseline_measurement']), read('reports/round11-final-measurement.json')
    check_measurement(baseline, 'build/round11/baseline/methods.csv')
    check_measurement(final, 'config/methods.csv')
    journal = read('reports/round11-cycle-journal.json')
    if not stop_reason(cfg, journal) or len(d_rows(journal)) > cfg['maximum_cycles']:
        raise ValueError('Final report lacks an authorized normal stop')
    if any(cfg['required_parts'][p] != 'complete' for p in 'ABC'):
        raise ValueError('A/B and six C fronts must have measured results')
    for part in ('A', 'B'):
        r = cfg['part_evidence'][part]
        check(r['report'], r['report_sha256'])
        if r['methods_gained'] or r['il_bytes_gained']:
            raise ValueError('Evidence-only A/B were attributed method gains')
    for front in cfg['c_fronts'].values():
        if front['status'] != 'verified':
            raise ValueError('C front was not fully evaluated')
    with (ROOT / 'config/methods.csv').open(encoding='utf-8-sig', newline='') as stream:
        ledger = {r['id']: r for r in csv.DictReader(stream)}
    receipts, metadata, stable_sources, models, engines, historical = {}, {}, {}, {}, [], []
    manifests, baseline_manifests = {}, {}
    for assembly in ASSEMBLIES:
        receipt = read('build/verification-' + assembly + '.json')
        if not receipt_is_fresh(ROOT, receipt) or any(receipt.get(k) for k in ('regressions', 'body_regressions', 'type_regressions')):
            raise ValueError('Official proof is stale or regressive: ' + assembly)
        if receipt['build_sha256'] != final['assemblies'][assembly]['provenance']['build_sha256']:
            raise ValueError('Final measurement and official artifact disagree')
        receipts[assembly] = receipt
        manifest_path = (ROOT / receipt['build_artifact']).parent / 'build-manifest.json'
        manifests[assembly] = read(manifest_path)
        baseline_manifests[assembly] = read('build/round11/baseline/' + assembly + '/build-manifest.json')
        if manifests[assembly]['output_sha256'] != receipt['build_sha256']:
            raise ValueError('Manifest and official artifact disagree')
        metadata[assembly] = read('reports/' + assembly + '-metadata.json')
        if not metadata[assembly]['whole_structural_metadata_equal']:
            raise ValueError('Structural metadata differs')
        if (metadata[assembly]['original_sha256'] != receipt['original_sha256'] or
                metadata[assembly]['rebuilt_sha256'] != receipt['build_sha256']):
            raise ValueError('Metadata evidence describes another artifact')
        for identity, row in receipt['methods'].items():
            stable_sources[assembly, row['stable_id']] = ledger[identity]['fonte']
        proof = cfg['final_model_measurements'][assembly]
        model = read(proof['validation'])
        for path, key in [(cfg['models'][assembly]['source'], 'source_sha256'),
                          (cfg['models'][assembly]['client'], 'client_sha256')]:
            check(path, model[key])
        check(Path(proof['corpus_baseline']) / 'verification.json', model['baseline_receipt_sha256'])
        if model['matched_model_complete'] != model['matched_complete_exact'] or model.get('matched_complete_mismatches', 0):
            raise ValueError('A complete model prediction is wrong')
        models[assembly] = model
        for field, gain in [('matched_methods', 'methods_gained'), ('matched_original_il_bytes', 'il_bytes_gained')]:
            if sum(c['gains'][assembly][gain] for c in journal['cycles']) != final['assemblies'][assembly][field] - baseline['assemblies'][assembly][field]:
                raise ValueError('Journal net gains disagree with baseline/final')
    for cycle in journal['cycles']:
        check(cycle['before_measurement'], cycle['before_sha256'])
        check(cycle['after_measurement'], cycle['after_sha256'])
        adoption = read('reports/round11-' + cycle['label'] + '-adoption.json')
        validate_adoption(adoption)
    for event in cfg.get('raw_evaluations', {}).values():
        check(event['measurement'], event['measurement_sha256'])
        check(Path(event['snapshot']) / 'assemblies/artifacts.json', event['artifacts_sha256'])
        for path, digest in event['reports'].items():
            check(path, digest)
    for path in sorted((ROOT / 'reports').glob('round11-*-engine.json')):
        document = read(path)
        if document.get('completed'):
            engines.append((path.name, document))
    for round_number in cfg['history_rounds']:
        for path in sorted((ROOT / 'reports').glob(f'round{round_number}-*-engine.json')):
            document = read(path)
            if document.get('completed'):
                historical.append((path.name, document))
    control = read(cfg['control_extractor_validation']) if cfg.get('control_extractor_validation') else None
    return dict(cfg=cfg, stop=stop, baseline=baseline, final=final, journal=journal, control=control,
                receipts=receipts, metadata=metadata, models=models, engines=engines,
                historical=historical, stable_sources=stable_sources,
                manifests=manifests, baseline_manifests=baseline_manifests)


def render(ctx):
    out = []
    add = out.append
    cfg, cycles = ctx['cfg'], ctx['journal']['cycles']
    add(SECTIONS[0])
    add(table(['Assembly', 'Anterior', 'Atual', 'Regressões'], [
        [a, counts(ctx['baseline']['assemblies'][a]), counts(ctx['final']['assemblies'][a]), 0] for a in ASSEMBLIES]))
    add('Matched exige cabeçalho, locais, IL e EH integrais; avanço de prefixo não é método concluído.')
    add(SECTIONS[1])
    rows = [[p, 'evidência', cfg['part_evidence'][p]['result'], 0, 0, 0] for p in ('A', 'B')]
    rows += [[c['label'] + ' / ' + (c.get('front') or c['phase']), c['assembly'] + ' / ' + c['pair'], c['hypothesis'] + '; ' + c['result'],
              c['methods_gained'], c['il_bytes_gained'], 0] for c in cycles]
    add(table(['Frente/ciclo', 'Assembly/par', 'Tentativa e resultado', 'Métodos ganhos', 'IL ganho', 'Perdas'], rows))
    for front, row in cfg['c_fronts'].items():
        labels = ', '.join(a['label'] for a in row['attempts'])
        add(front + ': ' + row['name'] + '; avaliações: ' + labels + '. Verificada descreve a frente avaliada; a hipótese pode continuar INCONCLUSIVA.')
    add(SECTIONS[2])
    a = read(cfg['part_evidence']['A']['report'])
    add('A — caminho CodeView/PDB: diferenças de caminho e padding ' + str(a['before_path_and_padding_different_bytes']) + ' → ' + str(a['after_path_and_padding_different_bytes']) +
        '; caminho passado ao compilador: ' + inline(a['original_pdb_path']) + '; edição posterior de DLL: ' + str(a['assembly_bytes_rewritten_after_compilation']) + '.')
    b = read(cfg['part_evidence']['B']['report'])
    for assembly, row in b['assemblies'].items():
        levels = [r['level'] for r in row['levels'] if r['compressed_bytes_equal_original']]
        add('B — ' + assembly + ': níveis DEFLATE com stream integral idêntico: ' + str(levels) + '; stream original ' + str(row['original_compressed_size']) + ' bytes; zlib ' + b['zlib_runtime_version'] + '; parâmetros ' + json.dumps(b['parameters'], ensure_ascii=False) + '.')
    add('APK montado: ' + str(b['apk_built']) + '; alvo executado nesta calibração: ' + str(b['target_executed']) + '.')
    add(APK_CONTRACT_NOTE)
    add(C6_SOURCE_NOTE)
    hypotheses = read('reports/round11-hypotheses.json')['hypotheses']
    for h in hypotheses:
        add(h['status'] + ' — ' + h['hypothesis'] + ' Evidência: ' + h['evidence'])
    for name, engine in ctx['engines']:
        for method in engine.get('methods', []):
            accepted = len(method.get('steps', []))
            if accepted and not method['body_equal']:
                add('INCONCLUSIVA — ' + inline(method['id']) + ': ' + str(accepted) + ' avanços parciais em ' + name + '; prefixo ' + str(method.get('initial_offset')) + ' → ' + str(method.get('final_offset')) + '; corpo integral ainda diferente.')
    add(table(['Modelo', 'Corpos matched', 'Mapeados', 'Sem mapeamento', 'Previsões completas', 'Completas exatas', 'Precisão', 'Cobertura', 'Incompletas'], [
        [assembly, m['matched_bodies'], m.get('matched_source_mapped', 'UNKNOWN'), m.get('matched_source_unmapped', 'UNKNOWN'),
         m['matched_model_complete'], m['matched_complete_exact'], pct(m['matched_complete_exact'], m['matched_model_complete']),
         pct(m['matched_model_complete'], m['matched_bodies']), m.get('matched_mapped_incomplete', 'UNKNOWN')]
        for assembly, m in ctx['models'].items()]))
    if ctx.get('control'):
        control = ctx['control']
        add('Extrator smcs: medição somente de leitura no corpus final; nenhuma árvore incompleta foi tratada como recuperação completa.')
        add(table(['Corpus', 'Métodos', 'Duas árvores completas', 'Árvores iguais', 'Cobertura', 'Igualdade entre completas'], [
            [name, row['methods'], row['paired_tree_complete'], row['trees_equal'],
             pct(row['paired_tree_complete'], row['methods']), pct(row['trees_equal'], row['paired_tree_complete'])]
            for name, row in [('matched', control['matched']), ('pendentes', control['pending'])]]))
    add(SECTIONS[3])
    for assembly, a in ctx['final']['assemblies'].items():
        add(assembly + ': ' + str(a['divergent_bodies']) + ' corpos divergentes / ' + str(a['divergent_original_il_bytes']) + ' bytes de IL.')
        add(table(['Faixa bytes', 'Métodos', 'IL bytes'], [[k, v['methods'], v['original_il_bytes']] for k, v in a['size_distribution'].items()]))
        histogram = a['alignment']['opcode_histogram']
        add(table(['Primeiro par alinhado', 'Métodos', 'IL bytes'], [[r['pair'], r['methods'], r['original_il_bytes']] for r in histogram]))
        for group in histogram[:5]:
            add(inline(group['pair']) + '; exemplo ' + inline(group['method_ids'][0]) + '.')
            add(table(['IL original', 'IL recompilado'], context_rows(group['example'])))
            attempts = attempt_summary(ctx, assembly, group)
            add('Tentativas compiladas nesta rodada: ' + compact_counter(attempts['attempts']) + '; recusas anteriores à compilação: ' + compact_counter(attempts['rejections']) +
                '; orçamento recusou ' + str(attempts['budget_skipped']) + '. Histórico 7–10 compatível por identidade original e hash do fonte: ' + (compact_counter(attempts['history']) if attempts['history'] else 'UNKNOWN') + '.')
        add('Pendentes com assinatura de locais igual: ' + str(a['pending_local_signatures_equal']) + '; diferente: ' + str(a['pending_local_signatures_different']) + '.')
    add(SECTIONS[4])
    selected = read('config/toolchain.json')
    for assembly in ASSEMBLIES:
        row = selected['assemblies'][assembly]
        manifest, before = ctx['manifests'][assembly], ctx['baseline_manifests'][assembly]
        flags = lambda m: [f for f in m['flags'] if not f.startswith(('/out:', '-out:'))]
        changed = manifest['compiler_sha256'] != before['compiler_sha256'] or flags(manifest) != flags(before)
        add(assembly + ': ' + str(row.get('compiler')) + '; versão ' + str(row.get('compiler_version')) +
            '; SHA-256 ' + inline(manifest['compiler_sha256']) + '; flags efetivas ' + inline(' '.join(flags(manifest))) + '.')
        add('Referências efetivas, na ordem: ' + inline(json.dumps(manifest['references'], ensure_ascii=False)) +
            '. Compilador/opções alterados desde baseline: ' + str(changed) +
            '; compilador histórico exato: ' + str(row.get('historical_compiler_version', 'UNKNOWN')) + '.')
    add('A configuração do caminho PDB da Parte A é produção pelo compilador; não é normalização nem edição da DLL.')
    add(SECTIONS[5])
    for assembly, md in ctx['metadata'].items():
        add(assembly + ': tipos/metadados estruturais ' + str(md['types_equal']) + '/' + str(md['types_total']) + '; igualdade estrutural global ' + str(md['whole_structural_metadata_equal']) + '.')
    exceptions = selected['non_deterministic_exceptions']
    add(table(['Assembly', 'Campo não determinístico', 'Prova'], [[r['assembly'], r['field'], r['evidence']] for r in exceptions]))
    add('Essas exceções são somente contabilidade de diferenças: nenhum byte é normalizado. Externos não foram decompilados nesta rodada; seu inventário/provas anteriores permanecem em docs/toolchain.md. DEX/Java permanece fora do escopo.')
    add(EXTERNALS_NOTE)
    for assembly, row in cfg.get('raw_evaluations', {}).items():
        add('Raw ' + assembly + ': gatilho medido em ' + row['trigger_label'] + '; recibos ' + ', '.join(row['reports']) + '. Resultado integral deve ser lido nesses recibos; não se presume igualdade por fechamento de métodos.')
    if not cfg.get('raw_evaluations'):
        add('Comparação bruta completa não repetida nesta rodada: KairoLibrary ainda tem corpos divergentes e Assembly-CSharp tem mais de dez. Além do caminho PDB medido na Parte A, as diferenças remanescentes de tabelas, heaps, seções e suas causas são UNKNOWN neste checkpoint; igualdade estrutural não prova igualdade bruta.')
    add(SECTIONS[6])
    add('Motivo da parada: ' + ctx['stop']['reason'] + '. Ciclos D: ' + str(len(d_rows(ctx['journal']))) + '/' + str(cfg['maximum_cycles']) + '; ciclos C: ' + str(sum(c['phase'] == 'C' for c in cycles)) + '.')
    add('Fila inicial explícita precedeu a descoberta autônoma. Após ela, os ciclos D04–D06 priorizaram Kairo, que ainda não fechou seus corpos. A/B: zero ganhos de métodos e IL. Orçamento de previsões incompletas: 12 variantes por método/passe e 24/ciclo; previsões completas erradas foram recusadas.')
    for note in cfg.get('final_report_notes', []):
        add(note)
    pending = [q for q in cfg['prepared_queue'] if q['status'] == 'pending']
    if pending:
        add(table(['Proposta preservada sem teste no jogo', 'Métodos', 'IL original potencial', 'Estado'], [
            [q['id'], ', '.join(q['method_ids']), q['potential_original_il_bytes'], 'UNKNOWN — não adotada'] for q in pending]))
    add(' '.join(ctx['stop'].get('blockers', [])) or 'UNKNOWN: bloqueios externos não informados.')
    add(SECTIONS[7])
    add('\n'.join(str(i) + '. ' + str(row) for i, row in enumerate(ctx['stop']['recommendations'], 1)))
    return '\n\n'.join(out) + '\n'


def main():
    text = render(load_context())
    if tuple(line for line in text.splitlines() if line in SECTIONS) != SECTIONS:
        raise ValueError('Report must contain exactly the eight requested sections')
    for name in ('reports/round11-final-report.md', 'reports/round11-chat-report.md'):
        (ROOT / name).write_text(text, encoding='utf-8', newline='\n')
    print(json.dumps({'reports': ['reports/round11-final-report.md', 'reports/round11-chat-report.md']}))


if __name__ == '__main__':
    main()
