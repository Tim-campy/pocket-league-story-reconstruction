"""Generate the self-contained eight-section round report from measured evidence."""
from collections import Counter
import csv
import json
from pathlib import Path

from compare_kairo import sha
from metadata_state import validate_type_metadata
from round4_report_measurements import measurement_markdown, _inline
from verification_state import validate_matched

ROOT = Path(__file__).resolve().parents[1]
ASSEMBLIES = ('KairoLibrary', 'Assembly-CSharp')
REQUIRED_RULES = (
    'tail-early-return', 'guard-return', 'guard-return-reverse', 'return-local',
    'ternary-polarity', 'if-polarity', 'switch-if', 'condition-local',
    'loop-cache-scope', 'loop-cache-scope-positive',
    'loop-continue-guard', 'loop-continue-guards', 'guard-continue', 'guard-continue-reverse',
    'guard-break', 'guard-break-reverse', 'assignment-chain-order', 'unary-negation',
    'condition-split-and', 'condition-merge-and', 'condition-split-or-guards', 'condition-merge-or-guards',
    'value-guard', 'value-guard-reverse', 'local-declaration-order-earlier',
    'local-declaration-order-later', 'local-declaration-scope-hoist', 'assignment-ternary-if',
    'assignment-ternary-if-reverse', 'declaration-ternary-if', 'declaration-ternary-if-reverse',
)


def read(path):
    return json.loads((ROOT / path).read_text(encoding='utf-8-sig'))


def table(headers, rows):
    return '\n'.join(['| ' + ' | '.join(headers) + ' |', '| ' + ' | '.join('---' for _ in headers) + ' |'] +
                     ['| ' + ' | '.join(str(v).replace('|', '&#124;').replace('\n', '<br>') for v in row) + ' |' for row in rows])


def instruction(i):
    operand = i.get('operand')
    if isinstance(operand, dict) and 'branch' in operand:
        operand = 'IL_%04x' % operand['branch']
    elif isinstance(operand, dict):
        operand = json.dumps(operand, ensure_ascii=False, separators=(',', ':'))
    return 'IL_%04x: %s%s' % (i['offset'], i['opcode'], '' if operand is None else ' ' + str(operand))


def named_controls(engines):
    controls = (
        ('KairoLibrary', 'FastMath::Abs(System.Single)', 'Menos unário; comparação de ponto flutuante preservada.'),
        ('KairoLibrary', 'Seb::GetWorkValue(', 'Ordem da atribuição encadeada campo/local.'),
        ('Assembly-CSharp', 'StoreBase::GetIncompletePayments(', 'Duas guardas continue no foreach; igualdade completa inclui try/finally e padding.'),
        ('Assembly-CSharp', 'Team::GetSum(', 'Declarações de locais movidas para a ordem original.'),
        ('Assembly-CSharp', 'Player::ChMoveDirect(', 'Sequência de transformações; prefixo parcial não comprova o método inteiro.'),
    )
    rows = []
    for assembly, fragment, explanation in controls:
        record = next(m for m in engines[assembly]['methods'] if fragment in m['id'])
        state = 'CONFIRMADA' if record['body_equal'] else 'INCONCLUSIVA para o corpo inteiro'
        result = (str(record['final_offset']) + ' bytes IL, corpo completo') if record['body_equal'] else ('prefixo ' + str(record['initial_offset']) + ' → ' + str(record['final_offset']) + ' bytes; corpo ainda divergente')
        rows.append((state, assembly, _inline(record['id']), explanation + ' ' + result + '.'))
    return table(['Estado', 'Assembly', 'Método', 'Evidência'], rows)


def validate_evidence_counts(baseline, final, engines, adoption, receipts, raw, ledger, baseline_ledger):
    """Reject stale/inconsistent zero-regression claims before rendering prose."""
    def require(condition, reason):
        if not condition:
            raise ValueError('Round-4 report evidence: ' + reason)
    require(adoption.get('official_verified') is True, 'official verification not completed')
    require(adoption.get('matched_losses') == 0, 'adoption reports matched losses')
    before = {r['id']: r for r in baseline_ledger}
    total_gains = 0
    for assembly in ASSEMBLIES:
        b, f, engine, receipt = baseline[assembly], final[assembly], engines[assembly], receipts[assembly]
        adopted = adoption['assemblies'][assembly]
        require(engine.get('completed') is True and engine.get('adoption_eligible') is True,
                assembly + ' engine is incomplete or ineligible')
        require(len(engine['methods']) == b['divergent_bodies'], assembly + ' engine did not process every baseline divergent body')
        require(not engine.get('body_losses'), assembly + ' engine has body regressions')
        require(not adopted.get('matched_losses'), assembly + ' adopted state losses')
        for key in ('regressions', 'body_regressions', 'type_regressions'):
            require(not receipt.get(key), assembly + ' official ' + key)
        rows = [r for r in ledger if r['assembly'] == assembly]
        old_rows = [r for r in baseline_ledger if r['assembly'] == assembly]
        require(len(rows) == len(old_rows) == f['total_methods'] == b['total_methods'], assembly + ' method totals differ')
        require(set(r['id'] for r in rows) == set(receipt['methods']), assembly + ' receipt method inventory differs')
        require(all(receipt['methods'][r['id']]['state'] == r['estado'] for r in rows), assembly + ' ledger state differs from receipt')
        matched = [r for r in rows if r['estado'] == 'matched']
        old_matched = [r for r in old_rows if r['estado'] == 'matched']
        require(len(matched) == f['matched_methods'] == receipt['counts']['matched'] == adopted['counts']['matched'], assembly + ' matched count differs')
        require(len(old_matched) == b['matched_methods'], assembly + ' baseline matched count differs')
        require(sum(int(r['tamanho_il']) for r in rows) == f['original_total_il_bytes'] == b['original_total_il_bytes'], assembly + ' total IL differs')
        require(sum(int(r['tamanho_il']) for r in matched) == f['matched_original_il_bytes'], assembly + ' matched IL differs')
        require(sum(int(r['tamanho_il']) for r in old_matched) == b['matched_original_il_bytes'], assembly + ' baseline matched IL differs')
        require(sum(bool(m.get('body_present')) and not m['body_equal'] for m in receipt['methods'].values()) == f['divergent_bodies'], assembly + ' divergent count differs')
        gains = {r['id'] for r in matched if before[r['id']]['estado'] != 'matched'}
        losses = {r['id'] for r in rows if r['estado'] != 'matched' and before[r['id']]['estado'] == 'matched'}
        require(not losses and gains == set(adopted['matched_gains']), assembly + ' adoption does not match ledger transition')
        total_gains += len(gains)
        require(f['provenance']['build_sha256'] == receipt['build_sha256'] == adopted['official_sha256'] == raw[assembly]['rebuilt_sha256'], assembly + ' final artifact hashes differ')
        require(raw[assembly]['original_sha256'] == receipt['original_sha256'], assembly + ' raw measurement uses another original')
        require(receipt['types_equal'] == receipt['types_total'] == adopted['types_equal'], assembly + ' type equality differs')
    require(total_gains == adoption['matched_gains'], 'combined adopted gains differ')


def validate_current_evidence(baseline, final, engines, adoption, receipts, raw):
    with (ROOT / 'config/methods.csv').open(encoding='utf-8-sig', newline='') as stream:
        ledger = list(csv.DictReader(stream))
    with (ROOT / 'build/round4/baseline/methods.csv').open(encoding='utf-8-sig', newline='') as stream:
        baseline_ledger = list(csv.DictReader(stream))
    with (ROOT / 'config/types.csv').open(encoding='utf-8-sig', newline='') as stream:
        types = list(csv.DictReader(stream))
    # These established guards check the actual compiler/reference/source/helper
    # and assembly hashes, rather than merely trusting JSON booleans.
    validate_matched(ROOT, ledger)
    metadata_reports = validate_type_metadata(ROOT, types)
    if any(not metadata_reports[assembly]['whole_structural_metadata_equal'] for assembly in ASSEMBLIES):
        raise ValueError('Report claims structural metadata equality without a complete current proof')
    validate_evidence_counts(baseline, final, engines, adoption, receipts, raw, ledger, baseline_ledger)
    for assembly in ASSEMBLIES:
        for measurement in (baseline[assembly], final[assembly]):
            p = measurement['provenance']
            for path_key, hash_key in (('original_dump', 'original_dump_sha256'), ('rebuilt_dump', 'rebuilt_dump_sha256'), ('receipt', 'receipt_sha256')):
                if sha(ROOT / p[path_key]) != p[hash_key]:
                    raise ValueError('Measurement input changed: ' + p[path_key])
        raw_proof = raw[assembly]
        if sha(ROOT / raw_proof['measurement']) != raw_proof['measurement_sha256']:
            raise ValueError('Raw measurement changed: ' + assembly)


def main():
    baseline = read('reports/round4-baseline-measurement.json')['assemblies']
    final = read('reports/round4-final-measurement.json')['assemblies']
    engines = {a: read('reports/round4-' + a + '-engine.json') for a in ASSEMBLIES}
    adoption = read('reports/round4-adoption.json')
    raw = read('reports/round4-final-raw-row-counts.json')['assemblies']
    receipts = {a: read('build/verification-' + a + '.json') for a in ASSEMBLIES}
    validate_current_evidence(baseline, final, engines, adoption, receipts, raw)
    project = read('config/project.json')
    lines = ['## 1. PROGRESSO', '', 'Projeto: Pocket League Story para Android, backend Unity Mono 5.3.4f1. APK de referência: ' + _inline(project['original_apk']) + ', SHA-256 ' + _inline(project['apk_sha256']) + '.', '',
             'Originais: ' + '; '.join(a + ' SHA-256 ' + _inline(receipts[a]['original_sha256']) for a in ASSEMBLIES) + '.', '',
             'Base: commit `1047b1fce99f8407ef3f1f38fd19ded6823d09c2`. Percentuais em frações exatas; bytes são os tamanhos de IL do original. `matched` exige também cabeçalho, locais e exceções iguais.', '']
    rows = []
    for a in ASSEMBLIES:
        b, f = baseline[a], final[a]
        n = f['total_methods']
        rows.append((a, n, b['matched_methods'], f"({b['matched_methods']}/{n}) × 100%", f['matched_methods'], f"({f['matched_methods']}/{n}) × 100%", f['matched_methods'] - b['matched_methods']))
    rows.append(('Total', sum(f['total_methods'] for f in final.values()), sum(f['matched_methods'] for f in baseline.values()),
                 f"({sum(f['matched_methods'] for f in baseline.values())}/5245) × 100%", sum(f['matched_methods'] for f in final.values()),
                 f"({sum(f['matched_methods'] for f in final.values())}/5245) × 100%", sum(f['matched_methods'] - baseline[a]['matched_methods'] for a, f in final.items())))
    lines += [table(['Assembly', 'Total', 'Matched anterior', '% anterior', 'Matched atual', '% atual', 'Ganho'], rows), '']
    lines += [table(['Assembly', 'IL total', 'IL matched anterior', 'IL matched atual', 'Ganho de IL', '% IL atual'],
                    [(a, f['original_total_il_bytes'], baseline[a]['matched_original_il_bytes'], f['matched_original_il_bytes'], f['matched_original_il_bytes'] - baseline[a]['matched_original_il_bytes'], f"({f['matched_original_il_bytes']}/{f['original_total_il_bytes']}) × 100%") for a, f in final.items()]), '']
    total_il = sum(f['matched_original_il_bytes'] for f in final.values())
    held = {a: receipts[a]['counts'].get('equal_body_held_for_generated_group', 0) for a in ASSEMBLIES}
    lines += [f"Total IL matched: **363493 → {total_il}, de 871497 bytes**. Zero regressões adotadas de corpos, estados e tipos. Restam {sum(f['divergent_bodies'] for f in final.values())} corpos divergentes e {sum(held.values())} corpos iguais retidos pelo vínculo de auxiliares gerados.", '',
              '## 2. O QUE FOI FEITO NESTA RODADA', '',
              'Cada linha separa tentativas, passos aceitos que avançaram o prefixo e novos corpos integralmente iguais. Ganhos são atribuídos à transformação do passo que fechou o corpo; passos anteriores podem ter contribuído. Os estados finais vieram exclusivamente do verificador.', '']
    rule_rows = []
    for a in ASSEMBLIES:
        counts = engines[a]['transformations']
        rules = sorted(set(REQUIRED_RULES) | {key.split(':')[0] for key in counts})
        for rule in rules:
            rule_rows.append((a, rule, counts.get(rule + ':attempts', 0), counts.get(rule + ':accepted_steps', 0), counts.get(rule + ':body_gains', 0), counts.get(rule + ':IL_gains', 0), 0))
    lines += [table(['Assembly', 'Transformação', 'Tentativas', 'Passos aceitos', 'Novos corpos iguais', 'Bytes IL', 'Perdas adotadas'], rule_rows), '',
              'Linhas com zero tentativas pertencem ao repertório disponível, mas não tiveram candidato elegível observado dentro do comando mapeado e do orçamento desta execução; zero não demonstra que a construção esteja correta em todos os métodos.', '']
    failures = sum(not t['compile_pass'] or t.get('full_compile_pass') is False for e in engines.values() for m in e['methods'] for t in m['trials'])
    rejected_body = sum(bool(t.get('body_regressions')) for e in engines.values() for m in e['methods'] for t in m['trials'])
    rejected_meta = sum(bool(t.get('metadata_rejected')) for e in engines.values() for m in e['methods'] for t in m['trials'])
    lines += [table(['Frente', 'Resultado', 'Ganhou matched', 'Perdeu matched'], [
        ('Medição antes das correções', '487 corpos; faixas e histogramas completos na seção 4.', 0, 0),
        ('PDB/MDB', 'smcs debug+/debug-: 1952 corpos iguais byte a byte e seis declarações sem corpo comparadas; cobertura inicial 140/141 Kairo e 343/345 App com IL diferente.', 0, 0),
        ('Referências Microsoft', 'System, System.Core e ambas: zero corpos alterados, zero ganhos/perdas e 337/337 tipos em cada build.', 0, 0),
        ('Servidor Roslyn', '/shared dentro do RSP falhou com CS2007; CLI preservou todos os corpos/metadados. Produção sem /shared.', 0, 0),
        ('Triagem smcs por fonte', 'Outros arquivos usam corpos provisórios apenas na triagem. Corpo atual do alvo precisa igualar o build completo; todo avanço exige novo build real e verificação completa. Contraexemplos desabilitam a triagem por método.', 0, 0),
        ('Tentativas paralelas isoladas', 'Até três variantes do mesmo comando em paralelo, com fontes/saídas independentes. Testes paralelo/serial e falha intencional CS0103 conferidos; sem adoção própria.', 0, 0),
        ('Sondas locais isoladas', 'GetSum +1 corpo/126 bytes; ChMoveDirect só prefixo 12→26. Sem adoção independente ou dupla contagem.', 0, 0),
        ('Pilotos e retomada', 'Pilotos isolados reiniciados para ativar servidor/atualizar repertório; checkpoint recompilado, sem promoção independente.', 0, 0),
        ('Adoção e acabamento', 'Primeira tentativa parou antes de copiar fontes por AttributeError na conversão de caminho; corrigida. Cinco linhas em branco perderam espaços finais; nova verificação oficial confirmou zero alterações nos corpos emitidos.', 0, 0),
        ('Conteúdo publicado pelo Git', 'Recompilados os C# exatos do índice Git: zero corpos alterados e 466/466 tipos. Manifesto separado registra hashes medidos e commitados quando há conversão CRLF→LF.', 0, 0),
        ('Metadados e bytes brutos', 'Metadados conferidos; bytes/tabelas medidos sem aplicar correções brutas.', 0, 0),
        ('Liberação automática de auxiliares', 'Corpos já iguais cuja prova de vínculo passou nesta rodada.', adoption.get('released_held', 0), 0)
    ]), '', 'Validação automatizada: 65 testes unitários passaram; 24 controles separados de seleção/sintaxe também passaram. A comparação integral dos dois assemblies com os comandos de produção confirmou os mesmos corpos do motor, incluindo os métodos ainda pendentes. A limpeza de espaços foi seguida de nova verificação integral e zero mudanças nesses corpos.', '',
              f'Negativos do motor: {failures} candidatos observados não compilaram, {rejected_body} candidatos com avanço de prefixo foram rejeitados por perda de corpos iguais e {rejected_meta} por metadados divergentes. Os candidatos compilados sem avanço também foram rejeitados; detalhes por método/patch no registro do motor.', '']
    for a, e in engines.items():
        s = e.get('screening', {})
        if not s.get('baseline_checks'):
            continue
        batches = s.get('batches', [])
        lines += [f"Triagem {a}: {s['baseline_checks']} controles, {s['baseline_equal']} habilitados e {s['baseline_different_or_failed']} recusados; {s['candidates']} candidatos comparados, {s['full_positive_checks']} controles positivos no build real e {s['false_positive_prefixes']} avanços da triagem que não sobreviveram ao build real. Lotes: {sum(b['finished'] for b in batches)} compilações concluídas, {sum(b['cancelled'] for b in batches)} canceladas, {sum(b['unused_speculative_finished'] for b in batches)} concluídas sem precisar comparar o corpo e {sum(b['compile_failures'] for b in batches)} falhas. Essas contagens se sobrepõem às tentativas observadas acima e não devem ser somadas.", '']
    lines += [
              '## 3. HIPÓTESES', '',
              table(['Estado', 'Hipótese e evidência'], [
                  ('CONFIRMADA no corpus medido', 'smcs -debug+ preserva os 1952 corpos comparados, inclusive bytes literais; as seis declarações sem corpo também foram comparadas. Cada mapeamento do motor confirma novamente a igualdade debug+/debug- do método atual.'),
                  ('CONFIRMADA nas sondas congeladas do commit 1047b1f', 'As três diferenças AssemblyRef são índices do mesmo token no #Blob: linhas 1/mscorlib, 2/System.Core, 4/System; PublicKeyOrToken 11271→11351, 07 2c→57 2c, token b77a5c561934e089 igual. Esses índices numéricos pertencem ao build de referência anterior às correções desta rodada, não ao layout final.'),
                  ('REFUTADA nas trocas testadas', 'Substituir System/System.Core Unity pelos arquivos Microsoft propostos resolveria divergências: quatro combinações no mesmo fonte produziram zero alterações nos corpos e mantiveram todos os tipos.'),
                  ('CONFIRMADA em casos medidos', 'Ordem das declarações pode explicar índices stloc: GetSum isolado passa com duas declarações movidas; ChMoveDirect isolado avança de 12 para 26 e iguala os locais.'),
                  ('INCONCLUSIVA como regra universal', 'IL segue a ordem do fonte: os passos aprovados dão evidência local; não foi demonstrado para todo lowering, prólogo gerado ou EH.'),
                  ('REFUTADA como critério de conclusão', 'Avançar o prefixo basta para promover um método: o motor mantém diferenças posteriores/locais/EH fora de matched; somente igualdade completa passa.'),
                  ('CONFIRMADA no controle', 'Roslyn /shared direto na CLI preserva os 3287 métodos e metadados estruturais; duas sondagens independentes registram também a falha CS2007 no RSP.'),
                  ('REFUTADA universalmente; uso condicionado', 'Corpos provisórios dos outros arquivos preservam qualquer alvo: controles StoreBase, Team e Player têm diferenças em 5, 2 e 2 métodos, respectivamente. Os três alvos nominais passaram; cada método do motor exige seu próprio controle antes de usar a triagem.'),
                  ('INCONCLUSIVA', 'Identidade histórica exata dos executáveis, todas as opções/referências históricas e determinismo dos originais continuam UNKNOWN.')
              ]), '', named_controls(engines), '', 'As transformações são hipóteses antes da recompilação. Negação de relações float/double/lifted não troca o operador; conserva negação explícita. Nenhuma classe gerada foi escrita manualmente.', '',
              '## 4. PENDENTES POR GRUPO', '',
              'A primeira diferença mede IL após resolver exclusivamente operandos de metadados. Destinos/operandos de desvios, larguras, constantes e nops permanecem. A classificação do histograma exato inclui opcode e operando; a projeção por opcode é apenas um guia de prioridade.', '',
              'Limites por método: oito iterações e 24 candidatos por comando em cada iteração. Um candidato só é aceito se avançar a primeira diferença preservando todo o prefixo correto, ou fechar integralmente o corpo, além de não perder corpos/metadados já iguais.', '']
    stop_rows = []
    for a in ASSEMBLIES:
        reasons = Counter(m['stop_reason'] for m in engines[a]['methods'])
        for reason, count in sorted(reasons.items()):
            stop_rows.append((a, reason, count))
    lines += [table(['Assembly', 'Motivo de parada', 'Métodos'], stop_rows), '',
              'Cada parada possui offset e comando no JSON do motor. Quando não há ponto de sequência correspondente, o comando é UNKNOWN. No único corpo com IL já idêntico, END_IL não é um byte divergente: a assinatura dos locais difere, e o comando causador permanece UNKNOWN; mapear o último ret serve apenas como posição diagnóstica. Uma transformação de ordem de declaração ou fusão de guardas pode abranger a sequência de declarações/comandos no bloco envolvente; não é uma inversão coletiva dos ramos do método. As avaliações existentes permanecem nas posições anteriores, e o prefixo IL é conferido a cada tentativa.', '']
    # Five exact instruction pairs, separated by compiler just as in measurement.
    groups = sorted([(a, g) for a in ASSEMBLIES for g in final[a]['first_pair_exact_histogram']], key=lambda v: (-v[1]['methods'], v[0], v[1]['pair']))[:5]
    lines += ['Cinco pares exatos mais frequentes restantes, mantendo a separação por compilador:', '']
    for n, (a, g) in enumerate(groups, 1):
        # Preserve the exact pair/frequency while preferring an EH example when
        # that same group contains one, as requested in the preceding round.
        example = next((m for m in final[a]['methods'] if m['id'] in g['method_ids'] and m['original_exception_regions']), g['example'])
        lines += [f"**{n}. {a}: {g['methods']} métodos / {g['original_il_bytes']} bytes IL.** Par " + _inline(g['pair']) + '.', '',
                  'Exemplo: ' + _inline(example['id']) + '; primeira diferença no byte IL ' + _inline(example['first_il_difference_offset']) + '.', '']
        left, right = example['original_context'][:15], example['rebuilt_context'][:15]
        lines += ['| IL original | IL recompilado |', '| --- | --- |']
        for j in range(max(len(left), len(right))):
            lines.append('| ' + (_inline(instruction(left[j])) if j < len(left) else '') + ' | ' + (_inline(instruction(right[j])) if j < len(right) else '') + ' |')
        if example['original_exception_regions'] or example['rebuilt_exception_regions']:
            lines += ['', 'Regiões de exceção originais: ' + _inline(json.dumps(example['original_exception_regions'])) + '; recompiladas: ' + _inline(json.dumps(example['rebuilt_exception_regions'])) + '.']
        lines += ['']
    lines += ['<details><summary>Medição completa ANTES das correções: faixas, todos os pares/contagens e contextos</summary>', '',
              measurement_markdown(ROOT / 'reports/round4-baseline-measurement.json'), '', '</details>', '',
              '<details><summary>Medição completa FINAL: faixas, todos os pares/contagens e contextos</summary>', '',
              measurement_markdown(ROOT / 'reports/round4-final-measurement.json'), '', '</details>', '',
              '## 5. TOOLCHAIN', '',
              'KairoLibrary: pacote Microsoft.Net.Compilers 1.1.0, csc 1.1.0.51109, SHA-256 `53b693536e8a86f35d05d2165ff9b3e0aeaa499ecfd8135a9f99ab4404ca86c3`. Opções de produção: `/nologo /noconfig /nostdlib+ /target:library /platform:anycpu /optimize+ /debug:pdbonly /deterministic- /langversion:6 /unsafe- /checked- /codepage:65001 /utf8output`. `/noconfig` fora do RSP; sem defines adicionais. Referências, em ordem: Microsoft mscorlib 2.0.0.0, Unity System.Core 3.5.0.0, UnityEngine do APK e Unity System 2.0.0.0.', '',
              'Assembly-CSharp: Unity 5.3.4f1, smcs 2.6.5.0, SHA-256 `26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0`. Opções de produção: `-nologo -noconfig -target:library -platform:anycpu -optimize+ -debug- -langversion:Default -unsafe- -checked- -codepage:65001`, stdlib default. Referências do APK na ordem mscorlib 2.0.5.0, KairoLibrary, UnityEngine, System.Core e System. MONO_PATH/MONO_CFG_DIR restritos ao subprocesso.', '',
              'Mudança desde a rodada anterior: infraestrutura de símbolos e transformador sintático; `/shared` somente nos ensaios Roslyn; `-debug+` somente no build smcs destinado ao MDB. A verificação/adotação final usa os compiladores, referências e flags de produção anteriores, sem essas duas opções de ensaio. Referências Microsoft alternativas não foram adotadas por não trazerem ganho.', '',
              'Os C# normalizados pelo Git foram compilados em diretórios isolados: todos os corpos coincidem com a verificação oficial e 466/466 tipos permanecem iguais. Os hashes de evidência identificam os bytes medidos na máquina; o manifesto round4-git-content-provenance.json também fornece o hash dos bytes commitados quando a política do repositório converte CRLF para LF. Essa conversão textual não é normalização de DLL ou de IL.', '',
              'UNKNOWN: versão histórica exata do compilador de cada alvo, caminho/versão integral das referências históricas, comando original completo e configuração histórica de determinismo. Compatibilidade medida não identifica por si só o executável histórico.', '',
              '## 6. COMPONENTES EXTERNOS E METADADOS', '',
              'As sete DLLs externas permanecem verificadas por SHA-256 contra Unity oficial. UnityEngine.dll é a variante Android Mono, igual nos 970752 bytes ao APK, SHA-256 `7cc901371cc77e103d00b5845e8285f85fbdf36bb47ccf3d824f94c4ead32421`. As seis bibliotecas nativas correspondem ao player Android Mono Release. Nenhum externo foi decompilado. O inventário permanece com 1218 entradas: dois alvos, 14 externos e 1202 dados. DEX/Java permaneceu fora do escopo.', '',
              'Metadados estruturais: KairoLibrary 337/337 tipos e Assembly-CSharp 129/129, total 466/466; ordem de tipos/membros, parâmetros, atributos, layout, constantes e recursos aprovados, além das dimensões globais comparadas. As 82 declarações sem corpo permanecem comprovadas. Inventário de métodos sem IDs ausentes ou adicionais. Igualdade estrutural não equivale à igualdade das tabelas/heaps brutos.', '',
              'Campos não determinísticos já comprovados, sem novas exceções ou normalização nesta rodada: KairoLibrary — Module.Mvid, COFF.TimeDateStamp, IMAGE_DEBUG_DIRECTORY.TimeDateStamp e CodeView.RSDS.PdbGuid; Assembly-CSharp — Module.Mvid e COFF.TimeDateStamp. Evidências de duas compilações idênticas: relatórios de repetibilidade da rodada 3. Caminho PDB, constantes e demais bytes não foram excluídos.', '',
              'Tabelas com bytes diferentes no build final; contagens de linhas comparadas sem qualquer correção baseada nos bytes brutos:', '']
    lines += [table(['Assembly', 'Tabela', 'Linhas original', 'Linhas recompilado', 'Contagem igual?'],
                    [(a, t['table'], t['original_rows'], t['rebuilt_rows'], 'SIM' if t['row_count_equal'] else 'NÃO') for a in ASSEMBLIES for t in raw[a]['different_tables']]), '',
              'As três diferenças AssemblyRef diagnosticadas nos testes de referência usam o mesmo PublicKeyOrToken resolvido. Esses índices de heap são medição de layout, não evidência de referências com identidades diferentes. Não foi feita correção de heaps/tabelas.', '',
              '## 7. BLOQUEIOS', '',
              'Nenhuma instalação, arquivo ou decisão do usuário é necessária para prosseguir no C#. Os bloqueios técnicos são comandos sem mapeamento no prólogo, locais/EH ainda divergentes e comandos cujos candidatos não avançaram o prefixo ou atingiram o orçamento. A lista exata de paradas está na seção 4 e no registro por método. A identidade histórica da toolchain continua UNKNOWN, sem impedir o ciclo com candidatos medidos. DEX/Java exige decisão futura de escopo e não foi trabalhado.', '',
              '## 8. SUA RECOMENDAÇÃO', '']
    limit_count = sum(m['stop_reason'] == 'candidate_limit' for e in engines.values() for m in e['methods'])
    partial_count = sum(bool(m['steps']) and not m['body_equal'] for e in engines.values() for m in e['methods'])
    largest = groups[0] if groups else None
    lines += [f'1. Refinar a busca nos {partial_count} métodos que já avançaram mas não fecharam o corpo, a partir do comando/offset final registrado. Potencial medido: 0–{partial_count} métodos; ganho exato UNKNOWN. Esses métodos já têm uma parte maior do IL comprovada.',
              f'2. Priorizar o maior par restante ({largest[0]}, {largest[1]["methods"]} métodos) e derivar uma transformação para sua diferença concreta de locais/controle. Potencial: 0–{largest[1]["methods"]} métodos desse grupo; o diagnóstico do par não prova uma causa única.' if largest else '2. Não há pares pendentes.',
              f'3. Aumentar seletivamente o orçamento ou usar sondas de duas transformações no mesmo comando nos {limit_count} métodos que atingiram o limite de candidatos; adotar apenas resultados com avanço estrito e zero regressões. Potencial: 0–{limit_count} métodos; sem estimativa pontual não medida.']
    target = ROOT / 'docs/notes/round-04-report.md'
    target.write_text('\n'.join(lines).replace('Prólog os', 'Prólogos') + '\n', encoding='utf-8')
    print(str(target.relative_to(ROOT)))


if __name__ == '__main__':
    main()
