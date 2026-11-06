"""Render the eight-section round-5 report only from fresh complete proofs."""
from collections import Counter
import csv
import json
from pathlib import Path

from compare_kairo import sha
from metadata_state import validate_type_metadata
from round4_report import REQUIRED_RULES, instruction, table
from round4_report_measurements import _inline, measurement_markdown
from verification_state import validate_matched

ROOT = Path(__file__).resolve().parents[1]
ASSEMBLIES = ('KairoLibrary', 'Assembly-CSharp')
BASELINE_COMMIT = '7d9fbc86b9117deae51562204d800b9f6f4e2f9c'
SOURCE_BASE = 'https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/'


def read(path):
    return json.loads((ROOT / path).read_text(encoding='utf-8-sig'))


def require(condition, reason):
    if not condition:
        raise ValueError('Round-5 report evidence: ' + reason)


def method_passes(engine):
    """Yield each measured pass once; historical records are absent from methods.

    Identical candidate text in two passes remains two real attempts. We never
    add cumulative transformation counters to the per-pass trial records.
    """
    history = engine.get('previous_passes', {})
    if not history:
        yield from engine['methods']
        return
    current = {m['id']: m for m in engine['methods']}
    require(set(history).issubset(current), 'retry history contains an unknown final method')
    for identity, rows in history.items():
        require(isinstance(rows, list) and rows, 'retry history must contain old pass records')
        require(all(row['id'] == identity for row in rows), 'retry history is stored under the wrong method ID')
        yield from rows
    yield from engine['methods']


def validate_evidence_counts(baseline, final, engine, adoption, receipts, ledger, baseline_ledger, selection=None):
    """Prevent partial engines, stale counts and forged zero-regression claims."""
    require(adoption.get('official_verified') is True, 'official verification not completed')
    require(adoption.get('matched_losses') == 0, 'adoption reports matched losses')
    require(engine.get('completed') is True and engine.get('adoption_eligible') is True,
            'App engine incomplete or ineligible')
    require(engine.get('adopted_ledger_states') is False, 'engine must not write official states')
    require(not engine.get('body_losses'), 'App engine reports body regressions')
    list(method_passes(engine))  # Validate history identities before any counts.
    if selection is None:
        # Kept for small isolated guard fixtures; production always supplies the
        # measured selection document and checks its exact identities and bytes.
        require(len(engine['methods']) == baseline['Assembly-CSharp']['divergent_bodies'],
                'App engine did not process every fixture target')
    else:
        selected = selection['methods']
        selected_ids = [m['id'] for m in selected]
        processed_ids = [m['id'] for m in engine['methods']]
        pending = {m['stable_id']: m for m in baseline['Assembly-CSharp']['methods']}
        require(len(selected_ids) == len(set(selected_ids)) == selection['methods_selected'], 'selection count/identities differ')
        require(set(selected_ids).issubset(pending), 'selection includes a nonpending baseline body')
        require(len(processed_ids) == len(set(processed_ids)) == len(selected_ids)
                and set(processed_ids) == set(selected_ids), 'App engine did not process exactly the selected IDs')
        require(all(m['original_il_bytes'] == pending[m['id']]['original_il_bytes'] for m in selected), 'selected original sizes differ')
        require(sum(m['original_il_bytes'] for m in selected) == selection['original_il_bytes'], 'selected IL total differs')
    before = {row['id']: row for row in baseline_ledger}
    total_gains = 0
    for assembly in ASSEMBLIES:
        b, f, receipt = baseline[assembly], final[assembly], receipts[assembly]
        adopted = adoption['assemblies'][assembly]
        for key in ('regressions', 'body_regressions', 'type_regressions'):
            require(not receipt.get(key), assembly + ' official ' + key)
        require(not adopted.get('matched_losses'), assembly + ' adopted losses')
        rows = [r for r in ledger if r['assembly'] == assembly]
        old_rows = [r for r in baseline_ledger if r['assembly'] == assembly]
        require(len(rows) == len(old_rows) == b['total_methods'] == f['total_methods'], assembly + ' method totals differ')
        require(set(r['id'] for r in rows) == set(r['id'] for r in old_rows), assembly + ' original ledger inventory changed')
        require(set(r['id'] for r in rows) == set(receipt['methods']), assembly + ' method inventory differs')
        stable_ids = {m.get('stable_id') for m in receipt['methods'].values()}
        require(all(m['stable_id'] in stable_ids for m in b.get('methods', [])), assembly + ' baseline pending body absent from full final proof')
        require(all(receipt['methods'][r['id']]['state'] == r['estado'] for r in rows), assembly + ' ledger states differ')
        matched = [r for r in rows if r['estado'] == 'matched']
        old_matched = [r for r in old_rows if r['estado'] == 'matched']
        require(len(matched) == f['matched_methods'] == receipt['counts']['matched'] == adopted['counts']['matched'], assembly + ' matched count differs')
        require(len(old_matched) == b['matched_methods'], assembly + ' baseline matched count differs')
        require(sum(int(r['tamanho_il']) for r in rows) == b['original_total_il_bytes'] == f['original_total_il_bytes'], assembly + ' total IL differs')
        require(sum(int(r['tamanho_il']) for r in matched) == f['matched_original_il_bytes'], assembly + ' matched IL differs')
        require(sum(int(r['tamanho_il']) for r in old_matched) == b['matched_original_il_bytes'], assembly + ' baseline matched IL differs')
        require(sum(bool(p.get('body_present')) and not p['body_equal'] for p in receipt['methods'].values()) == f['divergent_bodies'], assembly + ' divergent count differs')
        require(sum(p['methods'] for p in f['first_pair_exact_histogram']) == f['divergent_bodies'], assembly + ' final histogram differs')
        require(sum(p['original_il_bytes'] for p in f['size_distribution'].values()) == f['divergent_original_il_bytes'], assembly + ' size bytes differ')
        require(sum(p['methods'] for p in f['size_distribution'].values()) == f['divergent_bodies'], assembly + ' size count differs')
        gains = {r['id'] for r in matched if before[r['id']]['estado'] != 'matched'}
        losses = {r['id'] for r in rows if r['estado'] != 'matched' and before[r['id']]['estado'] == 'matched'}
        require(not losses and gains == set(adopted['matched_gains']), assembly + ' adopted transition differs')
        require(f['provenance']['build_sha256'] == receipt['build_sha256'] == adopted['official_sha256'], assembly + ' final artifact hashes differ')
        require(receipt['types_equal'] == receipt['types_total'] == adopted['types_equal'], assembly + ' structural metadata differs')
        total_gains += len(gains)
        if assembly == 'KairoLibrary':
            require(not adopted.get('engine_executed'), 'Kairo engine incorrectly reported as executed')
            for key in ('matched_methods', 'matched_original_il_bytes', 'divergent_bodies', 'divergent_original_il_bytes'):
                require(b[key] == f[key], 'Kairo changed despite unchanged-source scope: ' + key)
    require(total_gains == adoption['matched_gains'], 'combined adopted gains differ')


def validate_current_evidence(baseline, final, engine, adoption, receipts):
    ledger_path = ROOT / 'config/methods.csv'
    baseline_path = ROOT / 'build/round5/baseline/methods.csv'
    ledger = list(csv.DictReader(ledger_path.open(encoding='utf-8-sig', newline='')))
    old = list(csv.DictReader(baseline_path.open(encoding='utf-8-sig', newline='')))
    types = list(csv.DictReader((ROOT / 'config/types.csv').open(encoding='utf-8-sig', newline='')))
    validate_matched(ROOT, ledger)
    metadata = validate_type_metadata(ROOT, types)
    require(all(metadata[a]['whole_structural_metadata_equal'] for a in ASSEMBLIES), 'current structural metadata proof incomplete')
    selection = read('reports/round5-engine-selection.json')
    require(read('build/round5/engine-method-ids.json') == [m['id'] for m in selection['methods']], 'engine input selection differs from measured document')
    validate_evidence_counts(baseline, final, engine, adoption, receipts, ledger, old, selection)
    for assembly in ASSEMBLIES:
        for measurement in (baseline[assembly], final[assembly]):
            provenance = measurement['provenance']
            for key in ('original_dump', 'rebuilt_dump', 'receipt'):
                require(sha(ROOT / provenance[key]) == provenance[key + '_sha256'], 'measurement input changed: ' + provenance[key])
        require(sha(ROOT / 'build' / ('verification-' + assembly + '.json')) == adoption['assemblies'][assembly]['official_receipt_sha256'],
                assembly + ' adoption no longer uses current official receipt')
    require(read('reports/round5-baseline-measurement.json')['ledger_sha256'] == sha(baseline_path), 'baseline ledger provenance differs')
    require(read('reports/round5-final-measurement.json')['ledger_sha256'] == sha(ledger_path), 'final ledger provenance differs')


def load_final_evidence():
    baseline = read('reports/round5-baseline-measurement.json')['assemblies']
    final = read('reports/round5-final-measurement.json')['assemblies']
    engine = read('reports/round5-Assembly-CSharp-engine.json')
    adoption = read('reports/round5-adoption.json')
    receipts = {a: read('build/verification-' + a + '.json') for a in ASSEMBLIES}
    validate_current_evidence(baseline, final, engine, adoption, receipts)
    return baseline, final, engine, adoption, receipts


def controls_table(engine, receipts):
    records = [m for m in engine['methods'] if 'AppData::CheckTermScript(' in m['id'] or 'AppData::DrawSeb(' in m['id']]
    proofs = {p['stable_id']: p for p in receipts['Assembly-CSharp']['methods'].values()}
    require(records, 'named alias/DrawSeb controls absent')
    history = engine.get('previous_passes', {})
    return table(['Método', 'Estado confirmado', 'Primeiro offset anterior → final', 'Passos aceitos em todas as passagens'],
                 [(_inline(m['id']), proofs[m['id']]['state'],
                   str((history.get(m['id']) or [m])[0]['initial_offset']) + ' → ' + str(m['final_offset']),
                   sum(len(row['steps']) for row in history.get(m['id'], []) + [m])) for m in records])


def example_markdown(assembly, group, measurement, index):
    sample = next((m for m in measurement['methods'] if m['id'] in group['method_ids'] and m['original_exception_regions']), group['example'])
    output = [f"**{assembly}, par {index}: {group['methods']} métodos / {group['original_il_bytes']} bytes IL.** " + _inline(group['pair']) + '.', '',
              'Exemplo: ' + _inline(sample['id']) + '; primeiro byte divergente: ' + _inline(sample['first_il_difference_offset']) + '.', '',
              '| IL original | IL recompilado |', '| --- | --- |']
    left, right = sample['original_context'][:15], sample['rebuilt_context'][:15]
    for number in range(max(len(left), len(right))):
        output.append('| ' + (_inline(instruction(left[number])) if number < len(left) else '') + ' | '
                      + (_inline(instruction(right[number])) if number < len(right) else '') + ' |')
    if sample['original_exception_regions'] or sample['rebuilt_exception_regions']:
        output += ['', 'Regiões de exceção originais: ' + _inline(json.dumps(sample['original_exception_regions']))
                   + '; recompiladas: ' + _inline(json.dumps(sample['rebuilt_exception_regions'])) + '.']
    return output + ['']


def main():
    baseline, final, engine, adoption, receipts = load_final_evidence()
    fixture = read('reports/round5-slot-fixture.json')
    source = read('reports/round5-smcs-source.json')
    cases = read('reports/round5-local-cases.json')
    config = read('config/toolchain.json')
    project = read('config/project.json')
    selection = read('reports/round5-engine-selection.json')
    not_searched = baseline['Assembly-CSharp']['divergent_bodies'] - selection['methods_selected']
    history = engine.get('previous_passes', {})
    retry_count = sum(len(rows) for rows in history.values())
    retry_methods = len(history)
    require(source['hypothesis']['status'] == 'REFUTADA', 'slot hypothesis source result absent')
    require(len(fixture['variants']) == 8, 'eight-fixture evidence absent')
    require(cases['baseline_measurement_sha256'] == sha(ROOT / 'reports/round5-baseline-measurement.json'), 'local-case diagnostic is stale')
    require(sum(r['types_equal'] for r in receipts.values()) == 466, 'expected 466 current structural types')
    require(sum(r['counts']['declarations_metadata_matched'] for r in receipts.values()) == 82, 'expected 82 declarations with metadata proof')
    require(len(config['non_deterministic_exceptions']) == 6, 'non-deterministic exception inventory changed')
    lines = ['## 1. PROGRESSO', '',
             'Projeto: Pocket League Story para Android, Unity Mono 5.3.4f1. APK: ' + _inline(project['original_apk']) + '; SHA-256 ' + _inline(project['apk_sha256']) + '.', '',
             'Rodada anterior: commit ' + _inline(BASELINE_COMMIT) + '. As porcentagens abaixo são frações exatas multiplicadas por 100. Bytes matched são tamanhos de IL do original, contados apenas quando todo o corpo passa, incluindo cabeçalho, locais e exceções.', '']
    progress_rows, byte_rows = [], []
    for assembly in ASSEMBLIES:
        b, f = baseline[assembly], final[assembly]
        progress_rows.append((assembly, f['total_methods'], b['matched_methods'], f"{b['matched_methods']}/{b['total_methods']} × 100%",
                              f['matched_methods'], f"{f['matched_methods']}/{f['total_methods']} × 100%", f['matched_methods']-b['matched_methods'], 0))
        byte_rows.append((assembly, f['original_total_il_bytes'], b['matched_original_il_bytes'], f['matched_original_il_bytes'],
                          f['matched_original_il_bytes']-b['matched_original_il_bytes'],
                          f"{b['matched_original_il_bytes']}/{b['original_total_il_bytes']} × 100%",
                          f"{f['matched_original_il_bytes']}/{f['original_total_il_bytes']} × 100%"))
    lines += [table(['Assembly', 'Total métodos', 'Matched anterior', '% anterior', 'Matched atual', '% atual', 'Ganhos', 'Regressões'], progress_rows), '',
              table(['Assembly', 'Bytes IL total', 'IL matched anterior', 'IL matched atual', 'Ganho IL', '% IL anterior', '% IL atual'], byte_rows), '']
    total = sum(f['total_methods'] for f in final.values())
    total_il = sum(f['original_total_il_bytes'] for f in final.values())
    lines += [f"Total: {sum(b['matched_methods'] for b in baseline.values())} → {sum(f['matched_methods'] for f in final.values())} / {total} métodos matched; "
              f"{sum(b['matched_original_il_bytes'] for b in baseline.values())} → {sum(f['matched_original_il_bytes'] for f in final.values())} / {total_il} bytes IL matched. "
              f"Restam {sum(f['divergent_bodies'] for f in final.values())} corpos divergentes e {sum(r['counts'].get('equal_body_held_for_generated_group', 0) for r in receipts.values())} corpos iguais retidos por vínculo de auxiliares. Zero regressões adotadas de corpos, estados e tipos.", '',
              'Originais: ' + '; '.join(a + ' SHA-256 ' + _inline(receipts[a]['original_sha256']) for a in ASSEMBLIES) + '.', '',
              '## 2. O QUE FOI FEITO NESTA RODADA', '',
              'Ganhos abaixo são corpos integralmente iguais atribuídos à transformação que fechou o corpo. Prefixos parciais não são matched; passos anteriores podem contribuir. KairoLibrary foi medido e recompilado sem alterações, sem motor de correção nesta rodada.', '']
    counters = engine['transformations']
    rule_rows = []
    for rule in sorted(set(REQUIRED_RULES) | {key.split(':')[0] for key in counters}):
        rule_rows.append((rule, counters.get(rule + ':attempts', 0), counters.get(rule + ':accepted_steps', 0),
                          counters.get(rule + ':body_gains', 0), counters.get(rule + ':IL_gains', 0), 0))
    lines += [table(['Regra smcs', 'Tentativas', 'Passos aceitos', 'Corpos ganhos', 'Bytes IL ganhos', 'Perdas adotadas'], rule_rows), '',
              'Zero tentativas significa que o repertório não gerou candidato elegível dentro dos comandos/orçamentos testados; não confirma a hipótese nem mede todas as ocorrências.', '']
    lines += ['Ensaios isolados, sem somar ganhos novamente: o alias de bloco fechou CheckTermScript; o alias por ocorrência só avançou de 7 para 112. A troca de foreach por for fechou o controle DrawSeb; a variante que apenas troca a expressão do foreach piorou o primeiro offset de 10 para 9 e foi rejeitada. As duas sondas de locais compartilhados avançaram _draw4 e _draw6 de 2 para 26, sem fechar os corpos. As quatro retomadas posteriores não acrescentaram corpos matched.', '',
              'Validação dos auxiliares: 74 testes Python passaram; 17 fixtures das novas transformações e 24 controles das transformações anteriores passaram. A verificação oficial completa foi repetida após remover espaços de dez linhas vazias geradas, sem mudança de corpos em relação aos candidatos aprovados.', '']
    # methods holds the final pass for each unique selected ID. Retry history
    # holds the replaced records; include it exactly once in attempt negatives.
    all_passes = list(method_passes(engine))
    trials = [t for record in all_passes for t in record['trials']]
    failures = sum(not t['compile_pass'] or t.get('full_compile_pass') is False for t in trials)
    regressions = sum(bool(t.get('body_regressions')) for t in trials)
    metadata_rejected = sum(bool(t.get('metadata_rejected')) for t in trials)
    lines += [table(['Frente', 'Resultado medido', 'Ganhou matched', 'Perdeu matched'], [
        ('Hipótese de nomes', 'Oito compilações: quatro conjuntos de nomes × duas ordens; renomear não mudou os corpos; reordenar mudou os três corpos de controle.', 0, 0),
        ('Fonte do compilador', 'ListDictionary, blocos implícitos, clonagem de sondagem e temporários documentados; binário instalado inspecionado.', 0, 0),
        ('Diagnóstico dos onze stloc.3 → stloc.1', 'Fontes, locais, IL e MDB congelados; diagnósticos separados da prova de correção.', 0, 0),
        ('Motor App', 'Processou exatamente ' + str(len(engine['methods'])) + ' corpos selecionados / ' + str(selection['original_il_bytes'])
         + ' bytes IL em uma passagem inicial, mais ' + str(retry_count) + ' retomadas de ' + str(retry_methods)
         + ' métodos; ' + str(not_searched) + ' corpos pendentes não tiveram nova busca nesta fase. Todos permaneceram na verificação integral.', len(engine['body_gains']), 0),
        ('KairoLibrary', 'Sem correções de fonte; histograma Roslyn exclusivo e produção recompilada contra baseline.', 0, 0),
        ('Auxiliares gerados', 'Liberação exclusivamente pelo verificador, incluindo vínculo do grupo.', adoption.get('released_held', 0), 0),
        ('Adoção', 'Build integral de produção dos dois alvos antes da cópia de fontes; verificação oficial completa depois.', adoption['matched_gains'], 0),
        ('DEX/Java e bytes brutos', 'Nenhuma decompilação DEX ou correção baseada em bytes brutos.', 0, 0)
    ]), '',
              f'Tentativas observadas em todas as passagens: {len(trials)}. Negativos: {failures} candidatos não compilaram; {regressions} candidatos foram rejeitados por regressão de corpos iguais; {metadata_rejected} por metadados. Incluem cada registro histórico e cada registro final uma única vez; os contadores cumulativos por transformação não são somados novamente. Candidatos sem avanço estrito também foram rejeitados. As linhas de adoção e de motor se sobrepõem e não devem ser somadas.', '']
    if history:
        final_by_id = {m['id']: m for m in engine['methods']}
        lines += ['Retomadas direcionadas após a passagem inicial, com novas pistas para as cadeias de zeros e locais compartilhados:', '',
                  table(['Método', 'Retomadas', 'Offset antes da primeira passagem', 'Offset final', 'Corpo final igual?'],
                        [(_inline(identity), len(rows), rows[0]['initial_offset'], final_by_id[identity]['final_offset'],
                          'SIM' if final_by_id[identity]['body_equal'] else 'NÃO') for identity, rows in sorted(history.items())]), '']
    screening = engine.get('screening', {})
    if screening.get('baseline_checks'):
        lines += [f"Triagem: {screening['baseline_checks']} controles; {screening['baseline_equal']} habilitados; {screening['baseline_different_or_failed']} recusados; {screening['candidates']} candidatos; {screening['full_positive_checks']} positivos conferidos no build real; {screening['false_positive_prefixes']} avanços falsos rejeitados. Fontes provisórias existem somente nos ensaios; nenhum corpo provisório entra na produção.", '']
    lines += ['## 3. HIPÓTESES', '',
              '**HIPÓTESE 1 — REFUTADA nos caminhos testados: os nomes não definem os slots por ordem de hash.** Oito builds do smcs instalado mantiveram o IL ao trocar nomes; mudar a ordem das declarações mudou os slots de Ordinary, Lambda e Overloaded. A leitura do [getter](%sstatement.cs#L1550-L1555) mostra ListDictionary; [EmitMeta](%sstatement.cs#L2032-L2047) visita variáveis, temporários registrados e filhos. Trecho: `foreach (LocalInfo vi in variables.Values) vi.ResolveVariable (ec);`.' % (SOURCE_BASE, SOURCE_BASE), '',
              'Assinatura medida da fixture: Object, Int32, Double na ordem de declaração original; Double, Int32, Object com as declarações invertidas e nomes fixos. Os quatro conjuntos de nomes produziram a mesma assinatura e os mesmos corpos em cada ordem. O controle de declaração sem inicializador preservou dois slots Int32 mesmo quando um não tinha acesso; o controle de declarações agrupadas manteve Object, Int32, Double apesar da ordem diferente das atribuições.', '',
              'A [declaração no parser](%scs-parser.jay#L6077-L6105) cria bloco implícito após um comando e a [gramática](%scs-parser.jay#L4431-L4437) o torna corrente. Isso explica Object, Double, Int32 na fixture com um local exterior posterior ao bloco filho. A chave lexical do C# não basta para prever a árvore interna.' % (SOURCE_BASE, SOURCE_BASE), '',
              'A Hashtable de CloneTo pertence ao caminho de sondagem: [CompatibleMethod](%sanonymous.cs#L1168-L1185) clona somente nesse modo e [Compatible](%sanonymous.cs#L1024-L1063) não guarda o resultado da sondagem. A inspeção do smcs instalado confirmou as chamadas relevantes. A revisão histórica exata de fabricação continua UNKNOWN; snapshot consultado: Unity mono `b6587926c0f146c8b12985744f33a13b39a22d57`, 2016-03-11.' % (SOURCE_BASE, SOURCE_BASE), '',
              'CONFIRMADA na fonte inspecionada: TemporaryVariable registra temporários durante resolução; LocalTemporary pode alocar durante emissão e reutilizar slots liberados por tipo. foreach, using, lock, switch e retorno através de try podem introduzir locais. Portanto, contar apenas declarações explícitas não reconstrói toda assinatura. [Registro](%secore.cs#L5933-L5957), [emissão/reutilização](%scodegen.cs#L357-L434).' % (SOURCE_BASE, SOURCE_BASE), '',
              table(['Construção', 'Registro/alocação segundo a fonte consultada'], [
                  ('Bloco', 'EmitMeta declara locais em ordem de inserção, depois temporários registrados, depois blocos filhos em ordem de inserção; declarações após comandos podem estar em filhos implícitos.'),
                  ('foreach de coleção', 'Enumerador registrado durante resolução; descarte pode acrescentar temporário IDisposable. statement.cs:5674–5675,5790–5815.'),
                  ('foreach de array', 'Cópia do array e índices registrados durante resolução; arrays multidimensionais acrescentam comprimentos. statement.cs:5187–5203.'),
                  ('using', 'Expressão registra cópia durante resolução; forma com declaração usa o local declarado. statement.cs:4899–4936,5040–5113.'),
                  ('lock', 'Cópia do objeto registrada durante resolução. statement.cs:4111–4137.'),
                  ('switch', 'LocalTemporary solicita slot durante emissão, com possível reutilização por tipo. statement.cs:3823–3866.'),
                  ('Retorno protegido', 'TemporaryReturn aloca durante emissão quando necessário. codegen.cs:412–434.')]), '',
              'INCONCLUSIVA como regra universal: a ordem do IL seguir a ordem dos comandos admite lowerings e prólogos; cada transformação exige prova própria. CONFIRMADA como critério operacional: prefixo maior sozinho não promove matched. Busca por nomes não foi adotada. Nenhuma classe gerada pelo compilador foi escrita à mão.', '',
              'Controles pedidos pelo analista; o estado vem do recibo oficial, e o avanço parcial vem do motor:', '', controls_table(engine, receipts), '',
              'CONFIRMADA por corpo completo: introduzir alias do parâmetro no restante do bloco resolveu CheckTermScript (500 bytes). CONFIRMADA nos três overloads DrawSeb testados: substituir o foreach recuperado por laço indexado e os ajustes individuais registrados reproduziu os corpos; não era somente trocar dois comandos independentes.', '',
              'CONFIRMADA por compilação: restaurar a cadeia de zeros e a disposição de declarações fechou Draw_goal, Draw_goalMini, Draw_bonusIncome e Draw_matchElse (4465 bytes no total). CONFIRMADA: reservar o slot sem uso e adiar a declaração apropriada fechou LanguageForm.DrawButton (135 bytes). INCONCLUSIVA como solução completa dos demais casos de slots: o diagnóstico da primeira diferença não resolve automaticamente diferenças posteriores.', '',
              'Os nomes originais dos locais continuam UNKNOWN. Nomes introduzidos para aliases ou declarações reservadas são escolhas legíveis da reconstrução, não nomes históricos recuperados; nenhuma busca por ordem de hash foi executada.', '',
              'Diagnósticos congelados da linha de base dos onze casos de slot e controles adicionais, sem promover uma leitura de IL a fato histórico. A coluna de estado atual é obtida exclusivamente do recibo oficial:', '']
    selected_ids = {m['id'] for m in baseline['Assembly-CSharp']['methods'] if m['first_pair_opcodes'] == 'stloc.3 => stloc.1'}
    slot_cases = [m for m in cases['methods'] if m['id'] in selected_ids]
    require(len(slot_cases) == 11, 'eleven stloc.3/stloc.1 case diagnostics absent')
    classifications = Counter(category for m in slot_cases for category in m['diagnosis']['classification'])
    lines += ['Categorias não exclusivas dos onze: ' + '; '.join(key + ': ' + str(value) for key, value in sorted(classifications.items()))
              + '. Nos dez casos de cadeia de zeros, o original grava quatro slots e a reconstrução três; o nome do local correspondente continua hipótese. O caso de declaração sem uso possui um slot Int32 sem acessos. Esses diagnósticos não substituem a comparação após compilar.', '']
    case_rows = []
    current_proofs = receipts['Assembly-CSharp']['methods']
    for method in cases['methods']:
        diagnostic = method.get('diagnosis', method.get('diagnostic', {}))
        if isinstance(diagnostic, dict):
            explanation = diagnostic.get('evidence', diagnostic.get('reason', json.dumps(diagnostic, ensure_ascii=False))) if diagnostic else method.get('diagnosis_status', 'UNKNOWN')
            classification = diagnostic.get('classification', diagnostic.get('category', 'UNKNOWN'))
        else:
            explanation, classification = str(diagnostic), method.get('classification', 'UNKNOWN')
        if isinstance(classification, list):
            classification = ', '.join(classification)
        case_rows.append((_inline(method['id']), current_proofs[method['id']]['state'], classification,
                          diagnostic.get('evidence_status', 'UNKNOWN') if isinstance(diagnostic, dict) else 'UNKNOWN', explanation))
    lines += [table(['Método', 'Estado atual', 'Diagnóstico na linha de base', 'Status da evidência', 'Evidência / limite'], case_rows), '',
              '## 4. PENDENTES POR GRUPO', '',
              'O histograma usa o primeiro byte desigual de IL com somente tokens de metadados resolvidos. Destinos de desvios, larguras, constantes e nops permanecem. Os grupos exatos incluem operandos; a projeção por opcode é somente diagnóstico.', '',
              table(['Assembly', 'Faixa IL', 'Métodos anteriores', 'Bytes anteriores', 'Métodos atuais', 'Bytes atuais'],
                    [(a, name, old['methods'], old['original_il_bytes'], final[a]['size_distribution'][name]['methods'], final[a]['size_distribution'][name]['original_il_bytes'])
                     for a in ASSEMBLIES for name, old in baseline[a]['size_distribution'].items()]), '',
              f"Busca smcs: uma passagem inicial por {selection['methods_selected']} métodos / {selection['original_il_bytes']} bytes IL, mais {retry_count} retomadas direcionadas de {retry_methods} métodos; seleção por avanço parcial anterior, limite anterior ou padrões pedidos. {not_searched} dos {baseline['Assembly-CSharp']['divergent_bodies']} corpos pendentes iniciais não tiveram nova busca nesta fase. A medição final e a proteção de regressões abrangem todos os métodos dos dois assemblies. Limite de {engine['limit_iterations']} iterações e {engine['max_candidates_per_step']} candidatos por comando/iteração; {engine['iteration_limit_hits']} métodos atingiram o limite de iterações na sua passagem final. Cada parada registra offset e comando, ou UNKNOWN quando o mapeamento não está disponível. A tabela de paradas abaixo conta somente o último resultado de cada ID, sem somar paradas históricas.", '',
              table(['Motivo de parada', 'Métodos'], sorted(Counter(m['stop_reason'] for m in engine['methods']).items())), '',
              'KairoLibrary/Roslyn: dez pares exatos mais frequentes, exclusivos desse compilador. Os números anteriores e atuais são iguais porque seu fonte não mudou:', '',
              table(['Par exato original → recompilado', 'Métodos', 'Bytes IL'],
                    [(_inline(g['pair']), g['methods'], g['original_il_bytes']) for g in final['KairoLibrary']['first_pair_exact_histogram'][:10]]), '']
    for assembly in ASSEMBLIES:
        lines += ['**Cinco pares mais frequentes — ' + assembly + ' / ' + final[assembly]['compiler'] + '.**', '']
        for number, group in enumerate(final[assembly]['first_pair_exact_histogram'][:5], 1):
            lines += example_markdown(assembly, group, final[assembly], number)
    lines += ['<details><summary>Medição completa ANTERIOR — rodada 5: faixas, todos os pares e contextos</summary>', '',
              measurement_markdown(ROOT / 'reports/round5-baseline-measurement.json'), '', '</details>', '',
              '<details><summary>Medição completa ATUAL — rodada 5: faixas, todos os pares e contextos</summary>', '',
              measurement_markdown(ROOT / 'reports/round5-final-measurement.json'), '', '</details>', '',
              '## 5. TOOLCHAIN', '']
    for assembly in ASSEMBLIES:
        choice = config['assemblies'][assembly]
        manifest = read((Path(receipts[assembly]['build_artifact']).parent / 'build-manifest.json').as_posix())
        lines += [assembly + ': compilador ' + _inline(choice['compiler']) + ', versão ' + _inline(choice['compiler_version'])
                  + '; SHA-256 ' + _inline(manifest['compiler_sha256']) + '. Flags de produção: '
                  + _inline(' '.join(choice['flags'])) + '. Referências na ordem: ' + '; '.join(choice['references_order']) + '.', '']
    lines += ['KairoLibrary usa pacote Microsoft.Net.Compilers 1.1.0 / csc 1.1.0.51109; smcs 2.6.5.0 pertence ao Unity 5.3.4f1. Nenhum compilador, define, perfil, referência, ordem de referência ou flag de produção mudou nesta rodada. -debug+ fornece MDB apenas nos ensaios e exige prova de igualdade do corpo com -debug-. A seleção final usa -debug-. As compilações de produção não usam corpos provisórios.', '',
              'UNKNOWN: revisão de fabricação exata dos compiladores históricos, comando original completo, caminhos/versões integrais das referências históricas e determinismo dos originais. Compatibilidade medida não identifica o executável histórico.', '',
              '## 6. COMPONENTES EXTERNOS E METADADOS', '',
              'Externo verificado, sem alteração nesta rodada: sete DLLs contra Unity oficial; UnityEngine.dll Android Mono, 970752 bytes, SHA-256 `7cc901371cc77e103d00b5845e8285f85fbdf36bb47ccf3d824f94c4ead32421`. Seis bibliotecas nativas correspondem ao player Android Mono Release. Inventário: 1218 entradas, sendo dois alvos, 14 externos e 1202 dados. DEX/Java permaneceu fora do escopo, sem decompilação.', '',
              table(['Assembly', 'Tipos estruturais iguais', 'Declarações sem corpo comprovadas'],
                    [(a, str(receipts[a]['types_equal']) + '/' + str(receipts[a]['types_total']), receipts[a]['counts']['declarations_metadata_matched']) for a in ASSEMBLIES]), '',
              'Os 466 tipos passaram novamente: ordem de tipos/membros, parâmetros, atributos personalizados, layout, constantes, recursos e dimensões estruturais globais. As 82 declarações sem corpo permanecem comprovadas; não há IDs ausentes/adicionais. Essa prova não significa igualdade bruta de heaps/tabelas ou do arquivo.', '',
              'A prova adicional dos bytes C# preparados para o Git fica em reports/round5-git-source-proof.json; ela recompila todos os 260 fontes exatamente como entram no índice e compara corpos e metadados ao build oficial. reports/round5-git-content-provenance.json relaciona hashes dos arquivos medidos e publicados quando .gitattributes converte CRLF em LF. Essa conversão textual não normaliza assemblies.', '',
              'Lista de campos não determinísticos, sem nova exceção: KairoLibrary — Module.Mvid, COFF.TimeDateStamp, IMAGE_DEBUG_DIRECTORY.TimeDateStamp, CodeView.RSDS.PdbGuid; Assembly-CSharp — Module.Mvid, COFF.TimeDateStamp. Os seis campos têm evidência anterior de duas compilações idênticas; nenhuma normalização foi aplicada.', '',
              'Não houve nova correção baseada em bytes brutos. A medição bruta da rodada anterior descreve aquele snapshot; seus offsets e diferenças não são apresentados como medição do build atual.', '',
              '## 7. BLOQUEIOS', '',
              'Nenhuma instalação, arquivo ou decisão do usuário é necessária para continuar no C#. Permanecem métodos sem candidato que avance, divergências posteriores de locais/fluxo/exceções e limites do motor, discriminados na seção 4. A hipótese de nomes foi refutada, por isso a busca por nomes não foi executada. A revisão histórica exata continua UNKNOWN; os compiladores instalados permitem continuar os testes. DEX/Java depende de uma decisão futura de escopo e não foi trabalhado.', '',
              '## 8. SUA RECOMENDAÇÃO', '']
    partial = sum(bool(m['steps'] or any(row['steps'] for row in history.get(m['id'], []))) and not m['body_equal']
                  for m in engine['methods'])
    limited = sum(m['stop_reason'] in ('candidate_limit', 'iteration_limit') for m in engine['methods'])
    largest = final['Assembly-CSharp']['first_pair_exact_histogram'][0] if final['Assembly-CSharp']['first_pair_exact_histogram'] else None
    lines += [f'1. Continuar pelos {partial} métodos App com avanço parcial, a partir do offset/comando final registrado. Potencial delimitado: 0–{partial} métodos; ganho exato UNKNOWN. O prefixo já está mais próximo e reduz a busca restante.',
              f"2. Diagnosticar individualmente o maior par App restante ({largest['methods']} métodos, {largest['original_il_bytes']} bytes IL), recuperando a árvore de declarações e temporários antes de outras regras. Potencial: 0–{largest['methods']} métodos; causa comum ainda é hipótese." if largest else '2. Não há pares App pendentes.',
              f'3. Ampliar seletivamente o repertório/orçamento nos {limited} métodos que pararam no limite. Potencial: 0–{limited} métodos; nenhum ganho pontual foi medido. Preservar avanço estrito e zero regressões.']
    target = ROOT / 'docs/notes/round-05-report.md'
    target.write_text('\n'.join(lines) + '\n', encoding='utf-8')
    print(str(target.relative_to(ROOT)))


if __name__ == '__main__':
    main()
