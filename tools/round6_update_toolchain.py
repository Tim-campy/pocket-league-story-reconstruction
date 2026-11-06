"""Record round-6 validation without changing production selection or states."""
import copy
import json
from pathlib import Path

from compare_kairo import sha
from round6_report import ASSEMBLIES, BASELINE_COMMIT, compiled_signature_statistics, load_final_evidence, model_statistics, read, signature_changes, table

ROOT = Path(__file__).resolve().parents[1]
CURRENT_HEADING = '## Estado atual — rodada iniciada em 66eea9d'
PREVIOUS_HEADING = '## Estado atual — rodada iniciada em 7d9fbc8'
EVIDENCE_KEY = 'round6_slot_model_validation'


def unchanged_selection(before, after):
    left, right = copy.deepcopy(before), copy.deepcopy(after)
    for document in (left, right):
        document.pop(EVIDENCE_KEY, None)
        for assembly in ASSEMBLIES:
            document['assemblies'][assembly].pop('validation_scope', None)
    if left != right:
        raise ValueError('Round-6 toolchain update attempted to alter compiler/reference/flag/exception selection')


def normalized_references(rows):
    def path(value):
        p = Path(value)
        return str((p if p.is_absolute() else ROOT / p).resolve()).lower()
    return [(path(row['path']), row['sha256']) for row in rows]


def make_current_section(baseline, final, engines, adoption, receipts, model):
    statistics = model_statistics(model)
    changed = signature_changes(baseline['Assembly-CSharp'], final['Assembly-CSharp'])
    compiled = compiled_signature_statistics(engines['Assembly-CSharp'])
    lines = [CURRENT_HEADING, '',
             'Produção preservada: **Microsoft.Net.Compilers 1.1.0 / csc 1.1.0.51109** para KairoLibrary; **Unity 5.3.4f1 / smcs 2.6.5.0** para Assembly-CSharp. Compiladores, defines, flags, perfis, referências e sua ordem não mudaram. Nenhuma nova exceção não determinística.', '',
             table(['Assembly', 'Matched anterior → atual / total', 'Bytes IL matched anterior → atual / total', 'Corpos iguais', 'Tipos estruturais iguais'],
                   [(a, str(baseline[a]['matched_methods']) + ' → ' + str(final[a]['matched_methods']) + ' / ' + str(final[a]['total_methods']),
                     str(baseline[a]['matched_original_il_bytes']) + ' → ' + str(final[a]['matched_original_il_bytes']) + ' / ' + str(final[a]['original_total_il_bytes']),
                     receipts[a]['counts']['body_equal'], str(receipts[a]['types_equal']) + '/' + str(receipts[a]['types_total'])) for a in ASSEMBLIES]), '',
             '**' + str(sum(f['matched_methods'] for f in final.values())) + '/5245 métodos matched; '
             + str(sum(f['matched_original_il_bytes'] for f in final.values())) + '/871497 bytes IL matched.** '
             + 'Zero regressões adotadas de corpos, estados e tipos. Os 466 tipos e 82 declarações sem corpo passaram novamente. '
             + str(sum(r['counts'].get('equal_body_held_for_generated_group', 0) for r in receipts.values()))
             + ' corpos iguais permanecem retidos pelo vínculo de auxiliares. Igualdade estrutural não implica igualdade bruta dos heaps, tabelas e arquivos.', '',
             'Provas: [adoção oficial](../reports/round6-adoption.json), [medição inicial](../reports/round6-baseline-measurement.json), '
             '[medição final](../reports/round6-final-measurement.json), [relatório completo](notes/round-06-report.md). Estados são escritos exclusivamente pelo verificador.', '',
             '**Modelo smcs calculado a partir do fonte, sem emitir candidatos.** A regra recuperada usa inserção de variáveis, temporários registrados, blocos filhos/implícitos e temporários de emissão com reutilização por tipo. '
             + 'Validação: ' + str(statistics['exact_all']) + '/' + str(statistics['all_matched']) + ' assinaturas exatas em todos os corpos matched anteriores; '
             + str(statistics['complete_exact']) + '/' + str(statistics['complete']) + ' nas previsões declaradas completas. '
             + str(statistics['unmapped']) + ' corpos gerados ou não mapeados não contam como acerto por uma lista vazia. '
             + 'O gate exige previsão completa e assinatura prevista igual à original antes de compilar; discrepâncias observadas na assinatura compilada são rejeitadas. '
             + 'A cobertura incompleta está classificada em [round6-slot-model-validation.json](../reports/round6-slot-model-validation.json); não se declara modelo universal.', '',
             'Nas tentativas compiladas, ' + str(len(compiled['methods_with_compiled_original_signature_equal']))
             + ' métodos distintos tiveram candidato com assinatura igual à original. No fonte efetivamente adotado, '
             + str(len(changed['newly_equal'])) + ' dos pendentes iniciais passaram a ter assinatura igual e '
             + str(len(changed['newly_equal_and_closed'])) + ' desses fecharam o corpo. Tentativa confirmada não promove matched.', '',
             '**Busca por localização alinhada.** Destinos de desvio são reconciliados por rótulos e a primeira inserção, remoção ou substituição orienta o comando de fonte. '
             + 'A busca parcial exige avanço bruto por padrão. Na última retomada dos três gradientes, o prefixo de instruções alinhadas pôde avançar estritamente enquanto todos os bytes do prefixo bruto já igual permaneceram preservados. Cabeçalho, locais, EH, larguras e operandos continuam no comparador exato; nenhum estado matched usa o critério parcial. '
             + 'O motor percorreu os ' + str(len(engines['Assembly-CSharp']['methods'])) + ' pendentes smcs iniciais e '
             + str(len(engines['KairoLibrary']['methods'])) + ' controles Roslyn. Os dois assemblies foram integralmente compilados e verificados. '
             + 'Limites e paradas por método constam nos relatórios dos motores. Destino recompilado em trecho inserido pode permanecer classificado como branch_retarget; alinhamentos repetidos podem ser ambíguos.', '',
             '**Roslyn 1.1.** Nos quatro controles pedidos da linha de base 66eea9d, não havia local extra nem ausente: eram permutações de slots. '
             + 'EmitBlock define locais antes dos comandos; locais nomeados não são liberados, temporários reutilizáveis usam pilhas por assinatura, e o otimizador pode manter valores na pilha. '
             + 'Fonte oficial fixada e casos: [nota Roslyn](notes/round6-roslyn-source-evidence.md). Não foi construído modelo Roslyn completo. '
             + 'Nomes originais de locais sem símbolos e revisão histórica exata de fabricação continuam UNKNOWN; aliases/nomes reservados introduzidos são escolhas legíveis da reconstrução.', '',
             '**Limites mantidos.** Externos permanecem verificados contra Unity oficial. As seis exceções não determinísticas conservam a prova anterior de dois builds idênticos, sem acréscimos ou normalização de DLL/IL. '
             + 'DEX/Java e correções baseadas em bytes brutos ficaram fora do escopo. Os fontes exatos do índice Git são recompilados na prova adicional registrada em reports/round6-git-source-proof.json; '
             + 'reports/round6-git-content-provenance.json distingue hashes medidos dos publicados quando há conversão textual CRLF/LF.', '',
             'As seções abaixo preservam o histórico; números e diferenças brutas nelas descritos pertencem aos respectivos snapshots.', '']
    return '\n'.join(lines)


def prepend_current_section(document, section):
    title, separator, body = document.partition('\n')
    if title.rstrip('\r') != '# Toolchain e evidências' or not separator:
        raise ValueError('Unexpected toolchain document title')
    body = body.lstrip('\r\n')
    if body.startswith(CURRENT_HEADING):
        stop = body.find('\n## ', len(CURRENT_HEADING))
        if stop < 0:
            raise ValueError('Existing round-6 section has no historical boundary')
        body = body[stop + 1:]
    body = body.replace(PREVIOUS_HEADING, '## Histórico — rodada iniciada em 7d9fbc8', 1)
    return '# Toolchain e evidências\n\n' + section.rstrip() + '\n\n' + body.rstrip() + '\n'


def main():
    baseline, final, engines, adoption, receipts, model = load_final_evidence()
    before = read('config/toolchain.json')
    updated = copy.deepcopy(before)
    for assembly in ASSEMBLIES:
        choice, receipt, result = updated['assemblies'][assembly], receipts[assembly], final[assembly]
        manifest = read((Path(receipt['build_artifact']).parent / 'build-manifest.json').as_posix())
        flags = [flag for flag in manifest['flags'] if not flag.startswith(('/out:', '-out:'))]
        frozen = read('build/round6/baseline/' + assembly + '/build-manifest.json')
        if flags != choice['flags'] or normalized_references(manifest['references']) != normalized_references(choice['reference_files']):
            raise ValueError('Production flags/references differ from selection: ' + assembly)
        if manifest['compiler_sha256'] != frozen['compiler_sha256']:
            raise ValueError('Production compiler differs from baseline: ' + assembly)
        choice['validation_scope'] = ('complete source build; ' + str(receipt['counts']['body_equal']) + '/'
                                     + str(result['total_methods'] - receipt['counts']['declarations_without_body']) + ' bodies equal; '
                                     + str(result['matched_methods']) + '/' + str(result['total_methods']) + ' matched; '
                                     + str(result['matched_original_il_bytes']) + '/' + str(result['original_total_il_bytes']) + ' original IL bytes matched; '
                                     + str(receipt['types_equal']) + '/' + str(receipt['types_total']) + ' structural type metadata equal; '
                                     + 'whole assembly byte identity remains false; historical compiler UNKNOWN')
    paths = ['reports/round6-adoption.json', 'reports/round6-baseline-measurement.json', 'reports/round6-final-measurement.json',
             'reports/round6-slot-model-validation.json', 'reports/round6-kairo-selection.json', 'reports/round6-roslyn-slots.json',
             'reports/round6-slot-model-validation-engine-input.json', 'reports/round6-slot-fixture.json',
             'reports/round6-roslyn-null-operators.json', 'reports/round6-roslyn-patches.json', 'reports/round6-alignment-controls.json',
             'reports/round6-transform-tests.json', 'tools/Round6SlotModel.cs', 'tools/round6_slot_model.py',
             'reports/round6-slot-solver-inventory.json', 'reports/round6-slot-baseline-alternatives.json',
             'reports/round6-calculated-parameter-aliases.json', 'reports/round6-alias-rule-tests.json',
             'reports/round6-validation-checks.json',
             'tools/round6_slot_solver.py', 'tools/round6_solver_alternatives.py',
             'tools/round6_alias_calculus.py', 'tools/Round6AliasRules.cs',
             'tools/round6_alignment.py', 'tools/round6_adopt.py', 'tools/round6_report.py', 'tools/round6_update_toolchain.py']
    for assembly in ASSEMBLIES:
        paths += ['reports/round6-' + assembly + '-engine.json', 'build/verification-' + assembly + '.json']
    updated[EVIDENCE_KEY] = {
        'baseline_commit': BASELINE_COMMIT, 'operation': 'source-only-slot-model-and-aligned-location-reconstruction-with-official-complete-production-proof',
        'production_configuration_unchanged': True, 'new_non_deterministic_exceptions': [], 'normalization_applied': False,
        'model_validation': model_statistics(model),
        'compiled_smcs_signature_statistics': compiled_signature_statistics(engines['Assembly-CSharp']),
        'adopted_smcs_signature_changes': signature_changes(baseline['Assembly-CSharp'], final['Assembly-CSharp']),
        'adopted_signature_changes_by_assembly': {a: signature_changes(baseline[a], final[a]) for a in ASSEMBLIES},
        'evidence_files': [{'path': path, 'sha256': sha(ROOT / path)} for path in paths],
        'assemblies': {a: {'matched': final[a]['matched_methods'], 'total_methods': final[a]['total_methods'],
                           'matched_original_il_bytes': final[a]['matched_original_il_bytes'], 'original_total_il_bytes': final[a]['original_total_il_bytes'],
                           'body_equal': receipts[a]['counts']['body_equal'], 'types_equal': receipts[a]['types_equal'], 'types_total': receipts[a]['types_total'],
                           'official_build_sha256': receipts[a]['build_sha256'], 'engine_selected_methods': len(engines[a]['methods']),
                           'whole_assembly_byte_identity': False} for a in ASSEMBLIES}}
    unchanged_selection(before, updated)
    document_path = ROOT / 'docs/toolchain.md'
    document = prepend_current_section(document_path.read_text(encoding='utf-8-sig'), make_current_section(baseline, final, engines, adoption, receipts, model))
    (ROOT / 'config/toolchain.json').write_text(json.dumps(updated, indent=2, ensure_ascii=False) + '\n', encoding='utf-8', newline='\n')
    document_path.write_text(document, encoding='utf-8', newline='\n')
    print(json.dumps({'updated': ['config/toolchain.json', 'docs/toolchain.md'], 'production_selection_changed': False,
                      'method_or_type_states_changed': False, 'progress_file_changed': False}))


if __name__ == '__main__':
    main()
