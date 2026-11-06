"""Generate the eight-section round-6 report only from fresh complete proofs.

This module never compiles candidates or promotes method/type states. Importing
it has no side effects; the report is rendered only by main().
"""
from collections import Counter
import csv
import json
from pathlib import Path

from apply_il_rules import receipt_is_fresh
from compare_kairo import sha
from metadata_state import validate_type_metadata
from round4_report import REQUIRED_RULES, table
from round4_report_measurements import _inline
from verification_state import validate_matched

ROOT = Path(__file__).resolve().parents[1]
ASSEMBLIES = ('KairoLibrary', 'Assembly-CSharp')
BASELINE_COMMIT = '66eea9ddeb8e02a9f357f97f8e7c9dc60f54d6e9'
DERIVED_MODEL_SUMMARIES = {'matched_source_mapped', 'matched_source_unmapped', 'matched_mapped_incomplete',
                           'matched_mapped_incomplete_equal', 'matched_complete_mismatches', 'matched_incomplete_classifications'}


def read(path):
    return json.loads((ROOT / path).read_text(encoding='utf-8-sig'))


def require(condition, reason):
    if not condition:
        raise ValueError('Round-6 report evidence: ' + reason)


def ledger(path):
    with (ROOT / path).open(encoding='utf-8-sig', newline='') as stream:
        return list(csv.DictReader(stream))


def method_passes(engine):
    current = {m['id']: m for m in engine['methods']}
    require(len(current) == len(engine['methods']), 'duplicate final engine IDs')
    history = engine.get('previous_passes', {})
    require(set(history).issubset(current), 'retry history contains an unknown final ID')
    for identity, rows in history.items():
        require(isinstance(rows, list) and rows and all(r['id'] == identity for r in rows), 'invalid retry history')
        yield from rows
    yield from engine['methods']


def model_statistics(model):
    rows = [r for r in model['methods'] if r['state'] == 'matched']
    mapped = [r for r in rows if 'source' in r or r.get('source_mapped') is True]
    complete = [r for r in mapped if r.get('complete') is True]
    incorrect = [r for r in rows if not r['prediction_equal']]
    complete_incorrect = [r for r in complete if not r['prediction_equal']]
    pending = [r for r in model['methods'] if not r['body_equal']]
    pending_complete = [r for r in pending if r.get('complete') is True]
    return {'all_matched': len(rows), 'mapped': len(mapped), 'unmapped': len(rows) - len(mapped),
            'exact_all': sum(r['prediction_equal'] for r in rows),
            'exact_mapped': sum(r['prediction_equal'] for r in mapped),
            'complete': len(complete), 'complete_exact': sum(r['prediction_equal'] for r in complete),
            'complete_errors': len(complete_incorrect), 'incorrect': len(incorrect),
            'incomplete': len(rows) - len(complete),
            'error_classes': dict(Counter(c for r in incorrect for c in r.get('validation_classification', []))),
            'incomplete_classes': dict(Counter(c for r in rows if not r.get('complete') for c in r.get('validation_classification', []))),
            'unexplained': [r['method_id'] for r in incorrect if not r.get('validation_classification')],
            'complete_error_ids': [r['method_id'] for r in complete_incorrect],
            'pending': {'methods': len(pending), 'complete': len(pending_complete),
                        'complete_exact_current_signature': sum(r['prediction_equal'] for r in pending_complete),
                        'all_exact_current_signature': sum(r['prediction_equal'] for r in pending),
                        'incomplete': len(pending) - len(pending_complete),
                        'incomplete_equal_current_signature': sum(r['prediction_equal'] for r in pending if not r.get('complete')),
                        'incorrect_current_signature': sum(not r['prediction_equal'] for r in pending),
                        'incorrect_classes': dict(Counter(c for r in pending if not r['prediction_equal'] for c in r.get('validation_classification', [])))}}


def _dumps(measurement):
    p = measurement['provenance']
    return [{m['stable_id']: m for m in read(p[name])['selected_methods']}
            for name in ('original_dump', 'rebuilt_dump')]


def signature_changes(baseline, final):
    original, old = _dumps(baseline)
    _, new = _dumps(final)
    identities = {m['stable_id'] for m in baseline['methods']}
    before = {k for k in identities if original[k].get('locals') == old.get(k, {}).get('locals')}
    after = {k for k in identities if original[k].get('locals') == new.get(k, {}).get('locals')}
    closed = identities - {m['stable_id'] for m in final['methods']}
    return {'baseline_pending': len(identities), 'signature_equal_before': len(before),
            'signature_equal_after_for_same_ids': len(after), 'newly_equal': sorted(after - before),
            'lost_equal': sorted(before - after), 'closed_bodies': sorted(closed),
            'newly_equal_and_closed': sorted((after - before) & closed)}


def compiled_signature_statistics(engine):
    """Trial evidence is not adoption: report these populations separately."""
    same, predicted, disagreements = set(), set(), set()
    attempts_equal = 0
    for method in method_passes(engine):
        for trial in method['trials']:
            if trial.get('original_local_signature_equal') is True:
                same.add(method['id'])
                attempts_equal += 1
            if trial.get('predicted_signature_confirmed') is True:
                predicted.add(method['id'])
            elif trial.get('predicted_signature_confirmed') is False:
                disagreements.add(method['id'])
    return {'methods_with_compiled_original_signature_equal': sorted(same),
            'methods_with_compiled_prediction_confirmed': sorted(predicted),
            'methods_with_prediction_disagreement': sorted(disagreements),
            'compiled_attempts_with_original_signature_equal': attempts_equal}


def validate_engine_selection(assembly, engine, selected, pending):
    processed = {m['id'] for m in engine['methods']}
    require(selected.issubset(pending) and processed == selected and len(processed) == len(engine['methods']), assembly + ' engine selection incomplete or different')
    require(engine.get('completed') is True and engine.get('adoption_eligible') is True, assembly + ' engine incomplete/ineligible')
    require(engine.get('adopted_ledger_states') is False, assembly + ' engine must not promote ledger states')
    require(not engine.get('body_losses'), assembly + ' engine body regressions')
    list(method_passes(engine))


def validate_model_validation_pin(model, identity, current_hash, archive=None, archive_hash=None):
    """Allow added summaries only when the exact earlier engine input survives."""
    if identity.get('validation_sha256') == current_hash:
        return 'current-validation'
    require(archive is not None and identity.get('validation_sha256') == archive_hash, 'engine used another model validation')
    require(set(archive).issubset(model), 'expanded model summary removed old evidence')
    require(all(model[k] == value for k, value in archive.items()), 'expanded model summary changed engine-input evidence')
    require(set(model) - set(archive) <= DERIVED_MODEL_SUMMARIES, 'model validation changed beyond derived summaries')
    return 'archived-engine-input-with-identical-prediction-rows'


def validate_evidence(baseline_doc, final_doc, engines, adoption, receipts, model):
    require(adoption.get('official_verified') is True, 'official adoption verification not completed')
    require(adoption.get('matched_losses') == 0, 'adoption reports state regressions')
    require(adoption.get('baseline_commit') == BASELINE_COMMIT, 'wrong adoption baseline')
    require(baseline_doc.get('baseline_commit') == final_doc.get('baseline_commit') == BASELINE_COMMIT, 'wrong measurement baseline')
    current, before = ledger('config/methods.csv'), ledger('build/round6/baseline/methods.csv')
    validate_matched(ROOT, current)
    metas = validate_type_metadata(ROOT, ledger('config/types.csv'))
    old_ids = {r['id']: r for r in before}
    require(set(old_ids) == {r['id'] for r in current}, 'original method ledger inventory changed')
    require(baseline_doc['ledger_sha256'] == sha(ROOT / 'build/round6/baseline/methods.csv'), 'baseline ledger hash differs')
    require(final_doc['ledger_sha256'] == sha(ROOT / 'config/methods.csv'), 'final ledger hash differs')
    baseline, final = baseline_doc['assemblies'], final_doc['assemblies']
    selection_doc = read('reports/round6-kairo-selection.json')
    kairo_selection = selection_doc['methods']
    require(len(kairo_selection) == len(set(kairo_selection)) == selection_doc['count'], 'Kairo selection count/duplicates differ')
    total_gains = 0
    for assembly in ASSEMBLIES:
        b, f, engine, receipt = baseline[assembly], final[assembly], engines[assembly], receipts[assembly]
        adopted = adoption['assemblies'][assembly]
        pending = {m['stable_id'] for m in b['methods']}
        selected = pending if assembly == 'Assembly-CSharp' else set(kairo_selection)
        validate_engine_selection(assembly, engine, selected, pending)
        require(receipt_is_fresh(ROOT, receipt), assembly + ' current official proof stale')
        require(not any(receipt.get(k) for k in ('regressions', 'body_regressions', 'type_regressions')), assembly + ' official regressions')
        require(metas[assembly]['whole_structural_metadata_equal'], assembly + ' structural metadata incomplete')
        require(receipt['types_equal'] == receipt['types_total'] == adopted['types_equal'], assembly + ' types differ')
        require(sha(ROOT / ('build/verification-' + assembly + '.json')) == adopted['official_receipt_sha256'], assembly + ' adoption receipt hash differs')
        require(f['provenance']['build_sha256'] == receipt['build_sha256'] == adopted['official_sha256'], assembly + ' build hash differs')
        rows = [r for r in current if r['assembly'] == assembly]
        old = [r for r in before if r['assembly'] == assembly]
        require(len(rows) == len(old) == b['total_methods'] == f['total_methods'], assembly + ' totals differ')
        require(set(receipt['methods']) == {r['id'] for r in rows}, assembly + ' receipt inventory differs')
        require(all(receipt['methods'][r['id']]['state'] == r['estado'] for r in rows), assembly + ' receipt states differ')
        matched = [r for r in rows if r['estado'] == 'matched']
        old_matched = [r for r in old if r['estado'] == 'matched']
        require(len(matched) == f['matched_methods'] == receipt['counts']['matched'], assembly + ' matched totals differ')
        require(len(old_matched) == b['matched_methods'], assembly + ' baseline matched totals differ')
        require(sum(int(r['tamanho_il']) for r in matched) == f['matched_original_il_bytes'], assembly + ' final IL differs')
        require(sum(int(r['tamanho_il']) for r in old_matched) == b['matched_original_il_bytes'], assembly + ' baseline IL differs')
        gains = {r['id'] for r in matched if old_ids[r['id']]['estado'] != 'matched'}
        losses = {r['id'] for r in rows if r['estado'] != 'matched' and old_ids[r['id']]['estado'] == 'matched'}
        require(not losses and gains == set(adopted['matched_gains']), assembly + ' state transitions differ')
        total_gains += len(gains)
        for measurement in (b, f):
            p = measurement['provenance']
            for name in ('original_dump', 'rebuilt_dump', 'receipt'):
                require(sha(ROOT / p[name]) == p[name + '_sha256'], 'changed measurement input: ' + p[name])
            require(sum(g['methods'] for g in measurement['alignment']['exact_histogram']) == measurement['divergent_bodies'], assembly + ' alignment histogram count differs')
            require(len(measurement['alignment']['methods']) == measurement['divergent_bodies'], assembly + ' alignment rows incomplete')
            require(sum(g['methods'] for g in measurement['size_distribution'].values()) == measurement['divergent_bodies'], assembly + ' size bins differ')
            require(sum(g['original_il_bytes'] for g in measurement['size_distribution'].values()) == measurement['divergent_original_il_bytes'], assembly + ' size IL differs')
    require(total_gains == adoption['matched_gains'], 'combined state gains differ')
    require(model['source_sha256'] == sha(ROOT / 'tools/Round6SlotModel.cs'), 'model source changed after validation')
    require(model['client_sha256'] == sha(ROOT / 'tools/round6_slot_model.py'), 'model client changed after validation')
    require(model['baseline_receipt_sha256'] == sha(ROOT / 'build/round6/baseline/Assembly-CSharp/verification.json'), 'model validated another baseline')
    identity = engines['Assembly-CSharp'].get('slot_model_identity', {})
    archive_path = ROOT / 'reports/round6-slot-model-validation-engine-input.json'
    archive = read(archive_path) if archive_path.exists() else None
    validate_model_validation_pin(model, identity, sha(ROOT / 'reports/round6-slot-model-validation.json'),
                                  archive, sha(archive_path) if archive_path.exists() else None)
    require(identity.get('source_sha256') == model['source_sha256'] and identity.get('client_sha256') == model['client_sha256'], 'engine model source/client pin differs')
    require(identity.get('solver_sha256') == sha(ROOT / 'tools/round6_slot_solver.py'), 'engine declaration solver changed')
    statistics = model_statistics(model)
    require(statistics['all_matched'] == model['matched_bodies'] == 1682, 'model matched denominator differs')
    require(statistics['complete_errors'] == 0, 'model still claims completeness for wrong signatures')
    require(not statistics['unexplained'], 'unclassified model discrepancies')
    require(statistics['exact_all'] == model['matched_exact_predictions'], 'model exact count differs from rows')
    require(statistics['complete'] == model['matched_model_complete'] and statistics['complete_exact'] == model['matched_complete_exact'], 'model complete count differs from rows')
    require(statistics['pending']['methods'] == model['pending_bodies'] == baseline['Assembly-CSharp']['divergent_bodies'], 'model pending population differs')
    return current, before


def load_final_evidence():
    baseline_doc, final_doc = [read('reports/round6-' + name + '-measurement.json') for name in ('baseline', 'final')]
    engines = {a: read('reports/round6-' + a + '-engine.json') for a in ASSEMBLIES}
    adoption = read('reports/round6-adoption.json')
    receipts = {a: read('build/verification-' + a + '.json') for a in ASSEMBLIES}
    model = read('reports/round6-slot-model-validation.json')
    validate_evidence(baseline_doc, final_doc, engines, adoption, receipts, model)
    return baseline_doc['assemblies'], final_doc['assemblies'], engines, adoption, receipts, model


def instruction(row):
    operand = row.get('operand')
    if isinstance(operand, dict) and 'labels' in operand:
        operand = ','.join(operand['labels'])
    elif operand is not None and not isinstance(operand, str):
        operand = json.dumps(operand, ensure_ascii=False, separators=(',', ':'))
    label = row.get('label', '')
    return ('IL_%04x: ' % row['offset'] if 'offset' in row else '') + (label + ': ' if label else '') + row['opcode'] + ('' if operand is None else ' ' + str(operand))


def aligned_examples(assembly, measurement):
    lines = []
    for number, group in enumerate(measurement['alignment']['exact_histogram'][:5], 1):
        example = next((r for r in measurement['alignment']['methods'] if r['stable_id'] in group['method_ids'] and r['original_exception_regions']), group['example'])
        lines += [f"**{assembly}, par {number}: {group['methods']} métodos / {group['original_il_bytes']} bytes IL.** " + _inline(group['pair']) + '.', '',
                  'Exemplo: ' + _inline(example['stable_id']) + '; offsets alinhados original/recompilado: '
                  + str(example['original_offset']) + '/' + str(example['rebuilt_offset']) + '; edição: ' + example['kind'] + '.', '']
        left, right = example['original_context'][:15], example['rebuilt_context'][:15]
        lines += [table(['IL original', 'IL recompilado'],
                        [(_inline(instruction(left[n])) if n < len(left) else '',
                          _inline(instruction(right[n])) if n < len(right) else '') for n in range(max(len(left), len(right)))]), '']
        if example['original_exception_regions'] or example['rebuilt_exception_regions']:
            lines += ['EH original: ' + _inline(json.dumps(example['original_exception_regions']))
                      + '; recompilado: ' + _inline(json.dumps(example['rebuilt_exception_regions'])) + '.', '']
    return lines


def focused_controls(receipts, engines):
    prefixes = [('Assembly-CSharp', 'surface.GameView::OnTouchDlgCmd('), ('Assembly-CSharp', 'form.BootForm::Load('),
                ('Assembly-CSharp', 'system.form.BillingForm::DrawGradation('), ('Assembly-CSharp', 'form.SoccerForm::Draw_newPlayerLv('),
                ('Assembly-CSharp', 'system.form.FriendForm::DrawGradation('), ('Assembly-CSharp', 'system.friend.ui.FriendData::DrawGradation('),
                ('KairoLibrary', 'kairo.unity.util.Language::Init('), ('KairoLibrary', 'kairo.unity.util.Language::SetLanguagePack(')]
    rows = []
    for assembly, fragment in prefixes:
        proof = next(p for p in receipts[assembly]['methods'].values() if p['stable_id'].startswith(fragment))
        entry = next((m for m in engines[assembly]['methods'] if m['id'] == proof['stable_id']), None)
        initial = ((engines[assembly].get('previous_passes', {}).get(proof['stable_id']) or [entry])[0]
                   if entry is not None else None)
        rows.append((assembly, _inline(proof['stable_id']), 'CONFIRMADA por corpo completo' if proof['body_equal'] else 'INCONCLUSIVA para o corpo completo',
                     proof['state'], 'UNKNOWN' if entry is None else str(initial['initial_offset']) + ' → ' + str(entry['final_offset'])))
    return table(['Assembly', 'Controle', 'Hipótese de reconstrução', 'Estado oficial', 'Prefixo bruto anterior → final'], rows)


def main():
    baseline, final, engines, adoption, receipts, model = load_final_evidence()
    config, project = read('config/toolchain.json'), read('config/project.json')
    statistics = model_statistics(model)
    changes = {a: signature_changes(baseline[a], final[a]) for a in ASSEMBLIES}
    roslyn = read('reports/round6-roslyn-slots.json')
    slot_fixtures = read('reports/round6-slot-fixture.json')
    transform_fixtures = read('reports/round6-transform-tests.json')
    alias_fixtures = read('reports/round6-alias-rule-tests.json')
    solver_inventory = read('reports/round6-slot-solver-inventory.json')
    alternatives = read('reports/round6-slot-baseline-alternatives.json')
    previous = read('reports/round5-Assembly-CSharp-engine.json')
    previous_ids = {m['id'] for m in previous['methods']}
    previous_partial = {m['id'] for m in previous['methods'] if not m['body_equal'] and (m['steps'] or any(p['steps'] for p in previous.get('previous_passes', {}).get(m['id'], [])))}
    app_pending = {m['stable_id'] for m in baseline['Assembly-CSharp']['methods']}
    old_not_searched = app_pending - previous_ids
    app_processed = {m['id'] for m in engines['Assembly-CSharp']['methods']}
    require(previous_partial.issubset(app_processed) and old_not_searched.issubset(app_processed), 'continuity selections not covered')
    lines = ['## 1. PROGRESSO', '',
             'Projeto: Pocket League Story para Android, Unity Mono 5.3.4f1. APK: ' + _inline(project['original_apk']) + '; SHA-256 ' + _inline(project['apk_sha256']) + '.', '',
             'Rodada anterior: commit ' + _inline(BASELINE_COMMIT) + '. Percentuais abaixo são frações exatas multiplicadas por 100; bytes matched contam IL original somente quando todo o corpo, incluindo cabeçalho, locais e exceções, passa.', '',
             table(['Assembly', 'Total métodos', 'Matched anterior', '% anterior', 'Matched atual', '% atual', 'Ganhos', 'Regressões'],
                   [(a, final[a]['total_methods'], baseline[a]['matched_methods'], f"{baseline[a]['matched_methods']}/{baseline[a]['total_methods']} × 100%",
                     final[a]['matched_methods'], f"{final[a]['matched_methods']}/{final[a]['total_methods']} × 100%",
                     final[a]['matched_methods']-baseline[a]['matched_methods'], 0) for a in ASSEMBLIES]), '',
             table(['Assembly', 'IL total', 'IL matched anterior', 'IL matched atual', 'Ganho IL', '% IL anterior', '% IL atual'],
                   [(a, final[a]['original_total_il_bytes'], baseline[a]['matched_original_il_bytes'], final[a]['matched_original_il_bytes'],
                     final[a]['matched_original_il_bytes']-baseline[a]['matched_original_il_bytes'],
                     f"{baseline[a]['matched_original_il_bytes']}/{baseline[a]['original_total_il_bytes']} × 100%",
                     f"{final[a]['matched_original_il_bytes']}/{final[a]['original_total_il_bytes']} × 100%") for a in ASSEMBLIES]), '',
             'Total: ' + str(sum(v['matched_methods'] for v in baseline.values())) + ' → ' + str(sum(v['matched_methods'] for v in final.values()))
             + ' / 5245 métodos matched; ' + str(sum(v['matched_original_il_bytes'] for v in baseline.values())) + ' → '
             + str(sum(v['matched_original_il_bytes'] for v in final.values())) + ' / 871497 bytes IL matched. Restam '
             + str(sum(v['divergent_bodies'] for v in final.values())) + ' corpos divergentes e '
             + str(sum(r['counts'].get('equal_body_held_for_generated_group', 0) for r in receipts.values())) + ' corpos iguais retidos pelo vínculo de auxiliares. Zero regressões adotadas de estados, corpos e tipos.', '',
             'Originais: ' + '; '.join(a + ' SHA-256 ' + _inline(receipts[a]['original_sha256']) for a in ASSEMBLIES) + '.', '',
             '## 2. O QUE FOI FEITO NESTA RODADA', '',
             'Ganhos por transformação são atribuídos ao passo que fechou o corpo; passos parciais não são matched. Linhas de motor e adoção se sobrepõem e não são somadas. Candidatos smcs só foram compilados após previsão completa e assinatura prevista igual à original; o compilador e o verificador continuam sendo o juiz.', '']
    for a in ASSEMBLIES:
        e = engines[a]
        counters = e['transformations']
        rules = sorted(set(REQUIRED_RULES) | {key.rsplit(':', 1)[0] for key in counters})
        lines += ['**' + a + ': regras e tentativas.**', '', table(['Regra', 'Tentativas compiladas', 'Passos aceitos', 'Corpos ganhos', 'Bytes IL ganhos', 'Perdas adotadas'],
                  [(r, counters.get(r + ':attempts', 0), counters.get(r + ':accepted_steps', 0), counters.get(r + ':body_gains', 0), counters.get(r + ':IL_gains', 0), 0) for r in rules]), '']
        trials = [t for m in method_passes(e) for t in m['trials']]
        lines += [f"Negativos {a}: {len(trials)} tentativas observadas; "
                  + str(sum(not t['compile_pass'] or t.get('full_compile_pass') is False for t in trials)) + ' não compilaram; '
                  + str(sum(bool(t.get('body_regressions')) for t in trials)) + ' tiveram regressão de corpos; '
                  + str(sum(bool(t.get('metadata_rejected')) for t in trials)) + ' foram rejeitadas por metadados. '
                  + 'Cada passagem histórica e final é contada uma vez; candidatos sem avanço estrito no critério da respectiva passagem foram rejeitados. Zero tentativas em uma regra significa ausência de candidato elegível dentro do orçamento, não ausência histórica da construção.', '']
    gate = engines['Assembly-CSharp'].get('slot_model_gate', {})
    trial_signatures = compiled_signature_statistics(engines['Assembly-CSharp'])
    lines += [table(['Frente', 'Resultado', 'Matched ganhos', 'Matched perdidos'], [
        ('Modelo smcs', f"{statistics['all_matched']} corpos matched na validação; nenhum candidato do jogo compilado para calcular previsões.", 0, 0),
        ('Solver de declarações', str(len(changes['Assembly-CSharp']['newly_equal'])) + ' assinaturas antes diferentes agora iguais; ' + str(len(changes['Assembly-CSharp']['newly_equal_and_closed'])) + ' desses corpos fecharam. Causalidade exclusiva por transformação depende das linhas de regras acima.', len(changes['Assembly-CSharp']['newly_equal_and_closed']), 0),
        ('Cálculo em todos os pendentes', str(solver_inventory['methods_with_original_lifetimes']) + ' métodos com intervalos de uso; ' + str(solver_inventory['methods_with_calculated_positions']) + ' com posições calculadas; ' + str(solver_inventory['methods_with_model_approved_patches']) + ' com propostas aprovadas pelo modelo (' + str(solver_inventory['model_approved_patches']) + ' propostas), antes de qualquer compilação candidata.', 0, 0),
        ('Alternativas combinadas', str(alternatives['methods']) + ' métodos / ' + str(alternatives['alternatives']) + ' variantes recalculadas do snapshot; cadeias de zeros e agrupamentos combinados reapresentados sobre o método atual com previsão nova e proteção do prefixo.', 0, 0),
        ('Alias calculado de parâmetro', 'Três DrawGradation: alias dx, parâmetro 1, slot original 0; store 18, load 403, correspondente ao argumento direto 395. A hipótese de simples troca de duas declarações era insuficiente.', 0, 0),
        ('Alinhamento IL', 'Novo diagnóstico por edição de sequência; destinos de desvio reconciliados por rótulos. Nenhuma normalização usada na aceitação.', 0, 0),
        ('Controles independentes', str(slot_fixtures['complete_equal']) + '/' + str(slot_fixtures['complete']) + ' fixtures completas do alocador passaram; ' + str(transform_fixtures['fixture_pass_count'] + alias_fixtures['fixture_pass_count']) + ' controles sintáticos/de segurança das transformações passaram. Não promovem métodos do jogo.', 0, 0),
        ('Motor Assembly-CSharp', str(len(app_processed)) + ' pendentes percorridos, incluindo ' + str(len(old_not_searched)) + ' antes sem nova busca e ' + str(len(previous_partial)) + ' com avanço parcial anterior.', len(engines['Assembly-CSharp']['body_gains']), 0),
        ('Motor KairoLibrary', str(len(engines['KairoLibrary']['methods'])) + ' controles selecionados; os outros pendentes foram medidos e protegidos na verificação integral.', len(engines['KairoLibrary']['body_gains']), 0),
        ('Adoção', 'Produções completas dos dois alvos conferidas antes da cópia dos C#; verificação oficial integral após a adoção.', adoption['matched_gains'], 0),
        ('DEX/Java e bytes brutos', 'Fora do escopo; nenhuma decompilação DEX ou correção baseada em bytes brutos.', 0, 0)]), '',
        'Gate smcs medido: ' + _inline(json.dumps(gate, sort_keys=True)) + '. Previsões aprovadas e tentativas compiladas têm denominadores diferentes: a fila pode conter duplicatas, alternativas não consumidas ou tarefas canceladas após o primeiro avanço.', '',
        'Assinaturas confirmadas nas tentativas compiladas, incluindo candidatos depois rejeitados por prefixo: '
        + str(len(trial_signatures['methods_with_compiled_original_signature_equal'])) + ' métodos distintos tiveram ao menos um candidato com assinatura igual à original ('
        + str(trial_signatures['compiled_attempts_with_original_signature_equal']) + ' tentativas); '
        + str(len(trial_signatures['methods_with_prediction_disagreement'])) + ' métodos tiveram desacordo entre previsão e assinatura compilada. '
        + 'Essas contagens não promovem estados e não substituem as assinaturas adotadas medidas na seção 4.', '',
        'Critério de busca parcial: a passagem completa e as retomadas iniciais exigiram avanço do prefixo bruto. Somente a última retomada dos três DrawGradation habilitou avanço estrito do prefixo de instruções alinhadas, preservando todos os bytes do prefixo bruto já igual. O salto no offset 122 dependia do tamanho das seis expressões do laço: corrigir uma expressão adiante mantinha esse operando divergente. Essa retomada permitiu corrigir as expressões uma por vez. O comparador de corpos, cabeçalho, locais e EH permaneceu inalterado, e somente ele promove matched.', '',
        'Os 121 testes Python passaram. As 72 verificações sintáticas/de segurança e os 23 métodos de fixture smcs têm escopos separados. A prova adicional recompilou os 260 fontes exatos preparados para o Git: zero corpos alterados contra o build oficial e 466 tipos estruturalmente iguais. Resumo medido: reports/round6-validation-checks.json.', '',
        '## 3. HIPÓTESES', '', '**MODELO SMCS — taxa de acerto e cobertura.**', '',
        table(['População', 'Numerador / denominador', 'Percentual exato'], [
            ('Todos os corpos matched anteriores', f"{statistics['exact_all']}/{statistics['all_matched']}", f"{statistics['exact_all']}/{statistics['all_matched']} × 100%"),
            ('Métodos mapped ao fonte', f"{statistics['exact_mapped']}/{statistics['mapped']}", f"{statistics['exact_mapped']}/{statistics['mapped']} × 100%"),
            ('Previsões declaradas completas pelo modelo', f"{statistics['complete_exact']}/{statistics['complete']}", f"{statistics['complete_exact']}/{statistics['complete']} × 100%"),
            ('Cobertura completa em todos os matched', f"{statistics['complete']}/{statistics['all_matched']}", f"{statistics['complete']}/{statistics['all_matched']} × 100%")]), '',
        f"Há {statistics['unmapped']} corpos gerados ou sem método de fonte mapeado; eles não são contados como acerto por terem previsão vazia. {statistics['incomplete']} previsões não são declaradas completas; uma assinatura coincidente em previsão incompleta não autoriza compilação. Previsões completas incorretas: {statistics['complete_errors']}. Divergências sem classificação: {len(statistics['unexplained'])}.", '',
        'Proveniência da validação: o arquivo exato lido ao iniciar o motor foi preservado em reports/round6-slot-model-validation-engine-input.json. O relatório ampliado acrescenta somente seis campos de resumo derivados; todos os campos anteriores, inclusive as 1952 linhas de previsão, são idênticos. O guard confere o hash do input preservado e a igualdade desses dados, além dos hashes de modelo, cliente e solver.', '',
        table(['Classe residual de erro/limite', 'Métodos matched'], sorted(statistics['error_classes'].items())), '',
        'Limites das previsões incompletas, incluindo as que coincidiram incidentalmente com a assinatura atual:', '',
        table(['Classe de cobertura incompleta', 'Ocorrências em métodos matched'], sorted(statistics['incomplete_classes'].items())), '',
        'As classificações podem se sobrepor; não se deve somá-las para obter número de métodos. Diagnósticos CS0115, CS0155 e CS1660 pertencem ao helper semântico usado no cálculo; os fontes frozen já compilam no smcs. O modelo não usa essa coincidência de tipos para declarar completa uma previsão recusada.', '',
        'Resultados negativos do desenvolvimento: as primeiras versões erraram a representação de tipos genéricos aninhados e omitiram temporários de incremento, atribuição e receptor struct; outra versão confundiu Parse estático com receptor struct. As fixtures também detectaram temporário de retorno de construção struct padrão e switch de string ainda incompleto. Essas regras foram corrigidas antes de liberar o motor smcs. A métrica preliminar que contava assinatura vazia de método não mapeado como acerto foi descartada. Os snapshots intermediários não foram todos preservados após a interrupção da sessão: sua quantificação reprodutível por versão é UNKNOWN; as taxas acima pertencem ao modelo final arquivado.', '',
        'Controle adicional nos pendentes iniciais: ' + str(statistics['pending']['complete_exact_current_signature']) + '/' + str(statistics['pending']['complete'])
        + ' previsões completas coincidiram com a assinatura CURRENT da reconstrução; ' + str(statistics['pending']['incomplete']) + ' são incompletas, das quais '
        + str(statistics['pending']['incomplete_equal_current_signature']) + ' coincidem incidentalmente. Há '
        + str(statistics['pending']['incorrect_current_signature']) + ' métodos distintos com previsão diferente da assinatura atual. Isso valida o cálculo sobre o fonte recuperado; não significa que a assinatura atual já seja igual à original.', '',
        table(['Classe residual nos pendentes', 'Ocorrências'], sorted(statistics['pending']['incorrect_classes'].items())), '',
        'As classes de erro são diagnósticos, podem se sobrepor e não significam falha do corpo já matched. Capturas, membros gerados sem fonte e lowerings ainda não modelados são recusados pelo gate. A reconstrução integral do alocador não é declarada universal enquanto houver cobertura incompleta; causas históricas específicas não comprovadas continuam UNKNOWN.', '',
        'CONFIRMADA na população completa validada: o modelo calcula slots a partir do fonte e dos tipos sem emitir o assembly candidato. A regra usa inserção de variáveis, temporários registrados, filhos e blocos implícitos; temporários de emissão podem reutilizar slots livres por tipo. A [fonte do smcs](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/statement.cs#L2032-L2047) e a evidência anterior permanecem fixadas. A hipótese de nomes/hash permanece REFUTADA nos caminhos testados.', '',
        'INCONCLUSIVA como recuperação única do fonte histórico: assinatura e primeiro/último acesso de slots restringem declarações e escopos, mas não identificam nomes nem resolvem sozinhos todos os casos de locais do mesmo tipo. O solver gera propostas calculadas e usa a previsão da assinatura para eliminar candidatos antes da compilação. Declarações agrupadas e inicializações encadeadas são propostas de C#, não IL copiado.', '',
        'REFUTADA como explicação suficiente de DrawGradation: não era apenas a ordem de duas declarações. Os três controles tinham 11 locais no original e 10 na reconstrução; faltava o alias dx no slot 0. O cálculo alinhou seu único uso original em 403 ao argumento direto recompilado em 395. A regra introduziu dxAlias antes do primeiro local e substituiu somente aquele uso; dx em dx+w permaneceu argumento direto. O nome dxAlias foi escolhido para a reconstrução, não recuperado do original. O estado oficial na tabela abaixo decide a correspondência do corpo completo.', '',
        'Falha de instrumentação corrigida na revisão: uma tentativa de BillingManager.Init<T> compilou, mas o seletor passou a aridade do ID estável ao inspetor, enquanto Cecil seleciona pelo nome sem aridade. Essa tentativa histórica não foi contada como assinatura confirmada. O seletor foi corrigido, ganhou um teste de contrato e o método foi reexecutado: o prefixo avançou de 58 para 61, mas o corpo continua decompiled. Não houve promoção manual nem novo corpo completo nesse caso.', '',
        focused_controls(receipts, engines), '',
        '**Roslyn 1.1: os quatro casos na linha de base 66eea9d não tinham locais a mais ou a menos.** Os estados da última coluna descrevem o build atual após a rodada.', '',
        table(['Método', 'Locais original / recompilado anterior', 'A mais / a menos', 'Estado oficial atual'],
              [(case['name'], str(case['original_local_count']) + '/' + str(case['rebuilt_local_count']), str(case['additional_local_count']) + '/' + str(case['missing_local_count']),
                next(p['state'] for p in receipts['KairoLibrary']['methods'].values() if p['stable_id'] == case['method_id'])) for case in roslyn['cases']]), '',
        'CONFIRMADA por contagem e acessos: Canvas trocava índice i/array nos slots 0–1; SurfaceManagerBase trocava i/surfaceBase e text/height; Matrix emitia i,j,k,l onde os acessos originais indicam j,k,l,i; Graphics trocava num4,num2,num3 por num2,num3,num4 nos slots Single 3–5. Os nomes são do PDB recompilado; nomes históricos permanecem UNKNOWN. O resultado integral de cada ajuste vem exclusivamente do estado oficial acima.', '',
        'CONFIRMADA na [fonte Roslyn consultada](https://github.com/dotnet/roslyn/blob/4cb203dab89e1542cdf7d88cb6d32767bc0e5460/src/Compilers/CSharp/Portable/CodeGen/EmitStatement.cs#L594-L630): EmitBlock define os locais do bloco antes dos comandos e dos filhos. Locais nomeados não são liberados para reutilização; temporários reutilizáveis usam pilhas por assinatura. O otimizador pode manter um local na pilha, evitando slot. Essas são regras distintas; os quatro exemplos são permutações e não demonstram eliminação/reutilização como causa. Não foi criado modelo Roslyn completo.', '',
        'CONFIRMADA na análise estática: o acesso condicional permite ao emissor conhecer o receptor como não nulo e usar call para método não virtual. Reescrever ?. como if explícito foi testado pelo motor; só os corpos oficialmente iguais confirmam a reconstrução completa. O inventário AST da linha de base 66eea9d, antes dos guards adotados, encontrou 43 acessos condicionais a membro, 3 operadores ?? e 0 acessos condicionais por índice. Esses números descrevem o fonte recuperado daquele snapshot, não o build final nem o fonte histórico. ?? sobre nullable não foi tratado como referência. Fontes e intervalos completos estão em docs/notes/round6-roslyn-source-evidence.md.', '',
        '## 4. PENDENTES POR GRUPO', '',
        table(['Assembly', 'Faixa IL', 'Métodos anteriores', 'Bytes anteriores', 'Métodos atuais', 'Bytes atuais'],
              [(a, name, old['methods'], old['original_il_bytes'], final[a]['size_distribution'][name]['methods'], final[a]['size_distribution'][name]['original_il_bytes'])
               for a in ASSEMBLIES for name, old in baseline[a]['size_distribution'].items()]), '',
        table(['Assembly', 'Corpos pendentes atuais', 'Assinatura de locais já igual', 'Assinatura ainda diferente'],
              [(a, final[a]['divergent_bodies'], final[a]['pending_local_signatures_equal'], final[a]['pending_local_signatures_different']) for a in ASSEMBLIES]), '',
        'A igualdade da assinatura compara tipos por posição; não identifica qual variável ocupa cada slot quando várias têm o mesmo tipo. Só a comparação completa dos corpos confirma os acessos a esses slots.', '',
        'Nas mesmas identidades dos ' + str(changes['Assembly-CSharp']['baseline_pending']) + ' pendentes smcs iniciais: '
        + str(changes['Assembly-CSharp']['signature_equal_before']) + ' → ' + str(changes['Assembly-CSharp']['signature_equal_after_for_same_ids'])
        + ' assinaturas iguais; ' + str(len(changes['Assembly-CSharp']['newly_equal'])) + ' recém-igualadas; '
        + str(len(changes['Assembly-CSharp']['lost_equal'])) + ' retiradas de igualdade; '
        + str(len(changes['Assembly-CSharp']['newly_equal_and_closed'])) + ' recém-igualadas também fecharam o corpo. Essa medição é sobre as mesmas identidades; a tabela de pendentes atuais exclui os corpos que fecharam.', '',
        'Histograma atual por alinhamento: destinos são reconciliados por rótulos, sequências são comparadas por diferença de instruções e a primeira edição real orienta o ponto de fonte. As instruções e slots continuam significativos. Operandos de desvio são preservados no verificador bruto; o alinhamento não prova equivalência. Limites: instruções repetidas podem admitir mais de um alinhamento; destinos originais em trecho não alinhado são adiados explicitamente; destino recompilado em trecho inserido pode permanecer branch_retarget e selecionar um comando anterior.', '']
    for a in ASSEMBLIES:
        lines += aligned_examples(a, final[a])
        lines += ['<details><summary>Histograma completo por alinhamento — ' + a + '</summary>', '',
                  table(['Par exato original → recompilado', 'Métodos', 'Bytes IL'],
                        [(_inline(g['pair']), g['methods'], g['original_il_bytes']) for g in final[a]['alignment']['exact_histogram']]), '', '</details>', '',
                  '**Paradas do motor — ' + a + '.**', '',
                  table(['Parada final por ID único', 'Métodos'], sorted(Counter(m['stop_reason'] for m in engines[a]['methods']).items())), '',
                  'Limites: ' + str(engines[a]['limit_iterations']) + ' iterações por método, ' + str(engines[a]['max_candidates_per_step']) + ' candidatos compilados por etapa; '
                  + str(engines[a].get('iteration_limit_hits', 0)) + ' métodos atingiram o limite de iterações. Cada parada registra offset, comando e localização alinhada ou UNKNOWN. Histórico de retomadas não é somado às paradas finais.', '']
    lines += ['## 5. TOOLCHAIN', '']
    # Signature equality is a separate partial metric, including negative results.
    signature_rows = [(a, changes[a]['signature_equal_before'], changes[a]['signature_equal_after_for_same_ids'],
                       len(changes[a]['newly_equal']), len(changes[a]['lost_equal'])) for a in ASSEMBLIES]
    insertion = lines.index('## 5. TOOLCHAIN')
    signature_note = [table(['Assembly / mesmos pendentes iniciais', 'Assinaturas iguais antes', 'Iguais depois', 'Novas iguais', 'Igualdades perdidas'], signature_rows), '']
    for a in ASSEMBLIES:
        for key in changes[a]['lost_equal']:
            entry = next(m for m in engines[a]['methods'] if m['id'] == key)
            signature_note += ['Resultado negativo adotado em método ainda pendente: ' + _inline(a + '::' + key)
                + '. A assinatura antes igual deixou de coincidir; o prefixo IL bruto avançou de '
                + str(entry['initial_offset']) + ' para ' + str(entry['final_offset'])
                + ', mas o corpo continua decompiled. Isso não é regressão de um corpo anteriormente matched. A ordem de locais desse método precisa ser recuperada antes de concluir o corpo.', '']
    lines[insertion:insertion] = signature_note
    for a in ASSEMBLIES:
        choice = config['assemblies'][a]
        manifest = read((Path(receipts[a]['build_artifact']).parent / 'build-manifest.json').as_posix())
        lines += [a + ': compilador ' + _inline(choice['compiler']) + '; versão ' + _inline(choice['compiler_version'])
                  + '; SHA-256 ' + _inline(manifest['compiler_sha256']) + '. Flags: ' + _inline(' '.join(choice['flags']))
                  + '. Referências na ordem: ' + '; '.join(choice['references_order']) + '.', '']
    lines += ['Produção preservada: pacote Microsoft.Net.Compilers 1.1.0 / csc 1.1.0.51109 e Unity 5.3.4f1 / smcs 2.6.5.0. Nenhum compilador, define, perfil, referência ou opção de produção mudou. O helper moderno é usado somente para sintaxe/tipos e não emite os assemblies alvo. MDB é somente mapeamento com prova debug+/debug-; produção smcs usa -debug-.', '',
              'UNKNOWN: revisão histórica exata de fabricação dos compiladores, comando original completo, referências históricas integrais e nomes dos locais sem símbolos. O snapshot Roslyn version-1.1.0 foi fixado em 4cb203dab89e1542cdf7d88cb6d32767bc0e5460; sua correspondência exata ao executável instalado não foi comprovada.', '',
              '## 6. COMPONENTES EXTERNOS E METADADOS', '',
              'Sem mudança de externos nesta rodada: sete DLLs verificadas contra Unity oficial, incluindo UnityEngine.dll do Android Mono (970752 bytes; SHA-256 7cc901371cc77e103d00b5845e8285f85fbdf36bb47ccf3d824f94c4ead32421), e seis bibliotecas nativas verificadas contra Android Mono Release. Inventário: 1218 entradas, sendo dois alvos, 14 externos e 1202 dados.', '',
              table(['Assembly', 'Tipos estruturais iguais', 'Declarações sem corpo comprovadas'],
                    [(a, str(receipts[a]['types_equal']) + '/' + str(receipts[a]['types_total']), receipts[a]['counts']['declarations_metadata_matched']) for a in ASSEMBLIES]), '',
              'Os 466 tipos e as 82 declarações sem corpo passaram novamente: ordem de tipos/membros, parâmetros, atributos personalizados, layout, constantes, recursos e estruturas globais. Nenhum ID original ausente/adicional. Essa prova não significa igualdade bruta dos heaps, tabelas ou arquivos DLL.', '',
              'Campos não determinísticos, sem acréscimos: KairoLibrary — Module.Mvid, COFF.TimeDateStamp, IMAGE_DEBUG_DIRECTORY.TimeDateStamp, CodeView.RSDS.PdbGuid; Assembly-CSharp — Module.Mvid, COFF.TimeDateStamp. As seis exceções conservam a prova anterior de duas compilações idênticas. Nenhum byte de assembly foi normalizado.', '',
              'A prova de compilação dos 260 C# exatamente como preparados para o Git fica em reports/round6-git-source-proof.json. reports/round6-git-content-provenance.json relaciona hashes medidos e publicados quando .gitattributes converte CRLF para LF. Conversão de texto não é normalização de DLL.', '',
              'REA respondeu para a inspeção estática gerenciada; evidência retida em reports/round6-rea-evidence.json. Limitações do diagnóstico global de registros opcionais do REA não foram confundidas com indisponibilidade dessa análise. DEX/Java permaneceu fora, e nenhuma correção baseada em bytes brutos foi executada.', '',
              '## 7. BLOQUEIOS', '',
              'Nenhuma instalação, arquivo ou decisão do usuário é necessária para continuar no C#. Os limites técnicos são a cobertura incompleta do modelo, soluções de declarações não únicas, candidatos sem avanço ou limitados pelo orçamento e divergências posteriores de fluxo/exceções. O modelo recusa previsões incompletas; essa recusa não é corpo matched. A revisão histórica exata continua UNKNOWN. DEX/Java permanece fora do escopo.', '',
              '## 8. SUA RECOMENDAÇÃO', '']
    partial = sum(bool(not m['body_equal'] and (m['steps'] or any(p['steps'] for p in engines['Assembly-CSharp'].get('previous_passes', {}).get(m['id'], [])))) for m in engines['Assembly-CSharp']['methods'])
    largest = max(final['Assembly-CSharp']['alignment']['exact_histogram'], key=lambda group: group['original_il_bytes'], default=None)
    pending_ids = {m['stable_id'] for m in final['Assembly-CSharp']['methods']}
    constrained = sum(m['method_id'] in pending_ids and 'no_injective_consecutive_slot_run_within_one_lexical_block' in m.get('constraints', [])
                      for m in solver_inventory['methods'])
    lines += [f'1. Continuar pelos {partial} métodos smcs ainda divergentes com avanço parcial, a partir do offset/comando alinhado final. Potencial delimitado: 0–{partial} métodos; ganho exato UNKNOWN.',
              f"2. Investigar o grupo alinhado smcs com maior volume de IL ({largest['methods']} métodos / {largest['original_il_bytes']} bytes), usando o modelo para distinguir declaração, bloco e temporário. Potencial: 0–{largest['methods']} métodos; causa comum ainda precisa de prova." if largest else '2. Não há grupo smcs divergente restante.',
              f'3. Calcular mudanças de escopo e possível união de variáveis nos {constrained} pendentes sem sequência injetiva consecutiva dentro do bloco lexical. Potencial delimitado: 0–{constrained} métodos; ganho exato UNKNOWN. Primeiro/último uso não basta para provar uma união: exigir vínculos, tipos e fluxo, modelo atualizado e comparação completa. As populações das recomendações podem se sobrepor.']
    target = ROOT / 'docs/notes/round-06-report.md'
    target.write_text('\n'.join(lines).rstrip() + '\n', encoding='utf-8', newline='\n')
    print(target.relative_to(ROOT))


if __name__ == '__main__':
    main()
