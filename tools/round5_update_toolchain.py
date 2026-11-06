"""Record round-5 evidence only after fresh official proofs of both targets.

No compiler/reference/flag/exception selection or method/type state is edited.
Importing the module never writes files.
"""
import copy
import json
from pathlib import Path

from compare_kairo import sha
from round5_report import ASSEMBLIES, BASELINE_COMMIT, load_final_evidence, read, table

ROOT = Path(__file__).resolve().parents[1]
CURRENT_HEADING = '## Estado atual — rodada iniciada em 7d9fbc8'
OLD_HEADING = '## Estado atual — rodada iniciada em 1047b1f'


def unchanged_selection(before, after):
    left, right = copy.deepcopy(before), copy.deepcopy(after)
    for document in (left, right):
        document.pop('round5_slot_order_validation', None)
        for assembly in ASSEMBLIES:
            document['assemblies'][assembly].pop('validation_scope', None)
    if left != right:
        raise ValueError('Round-5 toolchain update attempted to alter selection, references, flags or exceptions')


def normalized_references(rows):
    return [(str(Path(row['path']).resolve()).lower(), row['sha256']) for row in rows]


def make_current_section(baseline, final, engine, adoption, receipts, selection):
    history = engine.get('previous_passes', {})
    retry_count = sum(len(rows) for rows in history.values())
    lines = [CURRENT_HEADING, '',
             'Produção preservada: **Microsoft.Net.Compilers 1.1.0 / csc 1.1.0.51109** para KairoLibrary; **Unity 5.3.4f1 / smcs 2.6.5.0** para Assembly-CSharp. Compiladores, defines, flags, perfis, referências e sua ordem não mudaram. Não foram acrescentadas exceções não determinísticas.', '',
             table(['Assembly', 'Matched anterior → atual / total', 'Bytes IL matched anterior → atual / total', 'Corpos iguais', 'Tipos estruturais iguais'],
                   [(a, str(baseline[a]['matched_methods']) + ' → ' + str(final[a]['matched_methods']) + ' / ' + str(final[a]['total_methods']),
                     str(baseline[a]['matched_original_il_bytes']) + ' → ' + str(final[a]['matched_original_il_bytes']) + ' / ' + str(final[a]['original_total_il_bytes']),
                     receipts[a]['counts']['body_equal'], str(receipts[a]['types_equal']) + '/' + str(receipts[a]['types_total'])) for a in ASSEMBLIES]), '',
             '**' + str(sum(f['matched_methods'] for f in final.values())) + '/' + str(sum(f['total_methods'] for f in final.values())) + ' métodos matched; '
             + str(sum(f['matched_original_il_bytes'] for f in final.values())) + '/' + str(sum(f['original_total_il_bytes'] for f in final.values())) + ' bytes IL matched.** '
             + 'Zero regressões adotadas. Os 466 tipos e as 82 declarações sem corpo têm prova estrutural atual. '
             + str(sum(r['counts'].get('equal_body_held_for_generated_group', 0) for r in receipts.values())) + ' corpos iguais permanecem retidos pelo vínculo de auxiliares. Igualdade estrutural não significa igualdade de todos os corpos, heaps, tabelas ou do arquivo inteiro.', '',
             'Provas atuais: [adoção e verificação oficial integral](../reports/round5-adoption.json), [medição inicial](../reports/round5-baseline-measurement.json), [medição final](../reports/round5-final-measurement.json), [relatório autossuficiente](notes/round-05-report.md). Os estados foram escritos exclusivamente pelo verificador.', '',
             '**Hipótese de nomes REFUTADA nos caminhos testados.** Oito compilações da fixture pelo smcs instalado mostraram que trocar apenas nomes conserva corpos e locais; reordenar declarações altera os slots. A fonte oficial Unity usa ListDictionary em blocos ordinários; a Hashtable encontrada é usada em clonagem de sondagem. O parser cria blocos implícitos após comandos, além das chaves lexicais. A árvore interna de blocos, a posição das declarações e os temporários registrados ou alocados na emissão explicam a regra observada. Busca por nomes não foi adotada. [Fixture](../reports/round5-slot-fixture.json); [fonte e binário inspecionados](../reports/round5-smcs-source.json); [nota com referências fixadas e limites](notes/round5-smcs-source-evidence.md).', '',
             'A fonte consultada pertence ao repositório oficial Unity-Technologies/mono, snapshot `b6587926c0f146c8b12985744f33a13b39a22d57` de 2016-03-11. A revisão exata de fabricação do executável instalado continua UNKNOWN; as operações relevantes foram confirmadas na inspeção do próprio smcs instalado. Os nomes originais de locais sem símbolos também continuam UNKNOWN. Os aliases e locais reservados introduzidos usam nomes legíveis escolhidos para a reconstrução; não são nomes originais recuperados.', '',
             '**Motor desta rodada: somente Assembly-CSharp.** Foram selecionados ' + str(selection['methods_selected']) + ' métodos / '
             + str(selection['original_il_bytes']) + ' bytes IL, por avanço parcial anterior, limite anterior ou padrões pedidos, em ordem decrescente de tamanho. '
             + 'Foi feita uma passagem inicial pelos selecionados e mais ' + str(retry_count) + ' retomadas direcionadas de ' + str(len(history)) + ' métodos. '
             + str(baseline['Assembly-CSharp']['divergent_bodies'] - selection['methods_selected'])
             + ' corpos pendentes iniciais não tiveram nova busca nesta fase; todos os métodos continuaram na verificação integral e na proteção de regressões. '
             + 'KairoLibrary foi medido e recompilado sem correções de fonte; o relatório fornece seu histograma Roslyn exclusivo. O motor smcs conserva todo o prefixo correto, exige avanço estrito ou igualdade completa e rejeita regressões de corpos/metadados. Mapeamento MDB só é usado após comparação debug+/debug- do corpo atual. Os builds finais usam as flags de produção (-debug-), sem fontes provisórias.', '',
             table(['Iterações máximas por método', 'Candidatos por comando/iteração', 'Limites de iteração atingidos', 'Corpos ganhos', 'Bytes IL ganhos'],
                   [(engine['limit_iterations'], engine['max_candidates_per_step'], engine['iteration_limit_hits'], len(engine['body_gains']), engine['matched_IL_gains'])]), '',
             '[Motor App e paradas por método](../reports/round5-Assembly-CSharp-engine.json). [Diagnósticos de slots e controles alias/DrawSeb](../reports/round5-local-cases.json). Prefixo parcial não promove matched. As produções integrais dos dois assemblies foram comparadas aos corpos esperados antes da adoção dos C#; nova verificação oficial completa confirmou o resultado depois da cópia.', '',
             '**Componentes e limites preservados.** Externos permanecem verificados contra o Unity oficial. As seis exceções não determinísticas já demonstradas por pares de builds idênticos permanecem as mesmas; nenhuma normalização de DLL/IL foi aplicada. A versão histórica exata dos compiladores, o comando completo e as referências históricas continuam UNKNOWN. DEX/Java permaneceu fora do escopo. Não foram feitas correções baseadas em bytes brutos.', '',
             'As seções abaixo preservam o histórico. Contagens e valores brutos nelas descritos pertencem aos respectivos snapshots, não ao build final desta rodada.', '']
    return '\n'.join(lines)


def prepend_current_section(document, section):
    title, separator, body = document.partition('\n')
    if title.rstrip('\r') != '# Toolchain e evidências' or not separator:
        raise ValueError('Unexpected toolchain document title')
    body = body.lstrip('\r\n')
    if body.startswith(CURRENT_HEADING):
        stop = body.find('\n## ', len(CURRENT_HEADING))
        if stop < 0:
            raise ValueError('Existing current section has no historical boundary')
        body = body[stop + 1:]
    body = body.replace(OLD_HEADING, '## Histórico — rodada iniciada em 1047b1f', 1)
    return '# Toolchain e evidências\n\n' + section.rstrip() + '\n\n' + body.rstrip() + '\n'


def main():
    baseline, final, engine, adoption, receipts = load_final_evidence()
    before = read('config/toolchain.json')
    selection = read('reports/round5-engine-selection.json')
    updated = copy.deepcopy(before)
    for assembly in ASSEMBLIES:
        current, receipt, result = updated['assemblies'][assembly], receipts[assembly], final[assembly]
        manifest_path = (ROOT / receipt['build_artifact']).parent / 'build-manifest.json'
        manifest = json.loads(manifest_path.read_text(encoding='utf-8-sig'))
        flags = [flag for flag in manifest['flags'] if not flag.startswith(('/out:', '-out:'))]
        if flags != current['flags'] or normalized_references(manifest['references']) != normalized_references(current['reference_files']):
            raise ValueError('Selected production flags/references differ: ' + assembly)
        current['validation_scope'] = ('complete source build; ' + str(receipt['counts']['body_equal']) + '/'
                                       + str(result['total_methods'] - receipt['counts']['declarations_without_body']) + ' bodies equal; '
                                       + str(result['matched_methods']) + '/' + str(result['total_methods']) + ' matched; '
                                       + str(result['matched_original_il_bytes']) + '/' + str(result['original_total_il_bytes']) + ' original IL bytes matched; '
                                       + str(receipt['types_equal']) + '/' + str(receipt['types_total']) + ' structural type metadata equal; '
                                       + 'whole assembly byte identity remains false; historical compiler UNKNOWN')
    paths = ['reports/round5-adoption.json', 'reports/round5-baseline-measurement.json', 'reports/round5-final-measurement.json',
             'reports/round5-slot-fixture.json', 'reports/round5-smcs-source.json', 'reports/round5-local-cases.json',
             'reports/round5-Assembly-CSharp-engine.json', 'reports/round5-engine-selection.json', 'build/round5/engine-method-ids.json',
             'tools/round5_adopt.py', 'tools/round5_report.py',
             'tools/round5_update_toolchain.py']
    for assembly in ASSEMBLIES:
        paths.append('build/verification-' + assembly + '.json')
    updated['round5_slot_order_validation'] = {
        'baseline_commit': BASELINE_COMMIT,
        'operation': 'official complete production verification after smcs slot-order diagnosis and source reconstruction',
        'production_configuration_unchanged': True, 'new_non_deterministic_exceptions': [], 'normalization_applied': False,
        'slot_name_hash_hypothesis': 'REFUTADA in tested paths', 'historical_smcs_build_source_commit': 'UNKNOWN',
        'engine_selection': {'methods': selection['methods_selected'], 'original_il_bytes': selection['original_il_bytes'],
                             'baseline_pending_not_searched': baseline['Assembly-CSharp']['divergent_bodies'] - selection['methods_selected'],
                             'targeted_retry_passes': sum(len(rows) for rows in engine.get('previous_passes', {}).values()),
                             'targeted_retry_methods': len(engine.get('previous_passes', {}))},
        'evidence_files': [{'path': path, 'sha256': sha(ROOT / path)} for path in paths],
        'assemblies': {a: {'matched': final[a]['matched_methods'], 'total_methods': final[a]['total_methods'],
                           'matched_original_il_bytes': final[a]['matched_original_il_bytes'], 'original_total_il_bytes': final[a]['original_total_il_bytes'],
                           'body_equal': receipts[a]['counts']['body_equal'], 'types_equal': receipts[a]['types_equal'],
                           'types_total': receipts[a]['types_total'], 'official_build_sha256': receipts[a]['build_sha256'],
                           'engine_executed': a == 'Assembly-CSharp', 'whole_assembly_byte_identity': False}
                       for a in ASSEMBLIES}}
    unchanged_selection(before, updated)
    section = make_current_section(baseline, final, engine, adoption, receipts, selection)
    document_path = ROOT / 'docs/toolchain.md'
    document = prepend_current_section(document_path.read_text(encoding='utf-8-sig'), section)
    (ROOT / 'config/toolchain.json').write_text(json.dumps(updated, indent=2, ensure_ascii=False) + '\n', encoding='utf-8')
    document_path.write_text(document, encoding='utf-8')
    print(json.dumps({'updated': ['config/toolchain.json', 'docs/toolchain.md'], 'production_selection_changed': False,
                      'method_or_type_states_changed': False, 'progress_file_changed': False}))


if __name__ == '__main__':
    main()
