"""Update toolchain evidence ONLY after fresh round-4 official final proofs.

No build, compiler/reference selection, normalization, method/type state or
PROGRESS file is changed. Importing this module never writes files.
"""
import copy
import json
from pathlib import Path

from compare_kairo import sha
from round4_report import ASSEMBLIES, read, table, validate_current_evidence

ROOT = Path(__file__).resolve().parents[1]
CURRENT_HEADING = '## Estado atual — rodada iniciada em 1047b1f'
OLD_HEADING = '## Estado atual — rodada iniciada em 8851963'


def unchanged_selection(before, after):
    """Permit only new proof records, validation text and the measured debug flag."""
    left, right = copy.deepcopy(before), copy.deepcopy(after)
    for document in (left, right):
        document.pop('round4_first_divergence_validation', None)
        for assembly in ASSEMBLIES:
            document['assemblies'][assembly].pop('validation_scope', None)
            document['assemblies'][assembly].pop('debug_tested', None)
    if left != right:
        raise ValueError('Toolchain update attempted to change a selection, reference, flag or exception')


def normalized_references(rows):
    return [(str(Path(r['path']).resolve()).lower(), r['sha256']) for r in rows]


def make_current_section(config, baseline, final, engines, receipts, raw, symbols):
    total_methods = sum(m['total_methods'] for m in final.values())
    matched = sum(m['matched_methods'] for m in final.values())
    matched_bytes = sum(m['matched_original_il_bytes'] for m in final.values())
    total_bytes = sum(m['original_total_il_bytes'] for m in final.values())
    type_equal = sum(r['types_equal'] for r in receipts.values())
    type_total = sum(r['types_total'] for r in receipts.values())
    declarations = sum(r['counts']['declarations_metadata_matched'] for r in receipts.values())
    held = sum(r['counts'].get('equal_body_held_for_generated_group', 0) for r in receipts.values())
    lines = [CURRENT_HEADING, '',
             'Seleção de produção preservada: **Microsoft.Net.Compilers 1.1.0 / csc 1.1.0.51109** para KairoLibrary e **smcs 2.6.5.0 / Unity 5.3.4f1** para Assembly-CSharp. Flags, defines, referências e sua ordem permanecem os da rodada anterior. Nenhum compilador alternativo, referência Microsoft adicional ou exceção não determinística foi adotado.', '',
             'Os auxiliares de análise sintática usam Roslyn **5.8.0-1.26319.2**, commit `375c93f06396eff88a355bc473d1586bf71befb5`, e runtime **.NET 10.0.10**. Eles geram hipóteses de edição do C#; os assemblies do jogo são emitidos pelos compiladores de produção registrados acima.', '',
             table(['Assembly', 'Métodos matched anterior → atual', 'Corpos integralmente iguais', 'Bytes IL matched anterior → atual / total', 'Tipos estruturais iguais'],
                   [(a, str(baseline[a]['matched_methods']) + ' → ' + str(final[a]['matched_methods']) + ' / ' + str(final[a]['total_methods']),
                     str(receipts[a]['counts']['body_equal']),
                     str(baseline[a]['matched_original_il_bytes']) + ' → ' + str(final[a]['matched_original_il_bytes']) + ' / ' + str(final[a]['original_total_il_bytes']),
                     str(receipts[a]['types_equal']) + '/' + str(receipts[a]['types_total'])) for a in ASSEMBLIES]), '',
             '**' + str(matched) + '/' + str(total_methods) + ' métodos matched; ' + str(matched_bytes) + '/' + str(total_bytes) + ' bytes IL matched; ' + str(type_equal) + '/' + str(type_total) + ' tipos com identidade estrutural comprovada.** As ' + str(declarations) + ' declarações sem corpo têm prova de metadados; ' + str(held) + ' auxiliares com corpo igual continuam retidos pela regra de vínculo. Zero regressões adotadas. Igualdade estrutural não significa igualdade de todos os corpos, heaps, tabelas ou do arquivo inteiro.', '',
             'Os números vieram da [adoção após verificação oficial](../reports/round4-adoption.json), [medição final](../reports/round4-final-measurement.json) e recibos oficiais atuais. O [relatório autossuficiente](notes/round-04-report.md) inclui o histograma completo antes/depois, exemplos e negativos. [Medição inicial](../reports/round4-baseline-measurement.json).', '',
             '**Mapeamento e motor.** Roslyn usa pontos de sequência do PDB nativo emitido com `/debug:pdbonly`. No smcs, `-debug+` preservou os 1952 corpos do controle congelado, inclusive bytes literais, e seis declarações sem corpo; o MDB fornece os mesmos offsets. Cada método atual do motor repete a prova debug+/debug- antes de usar seu MDB. Cobertura inicial: ' + str(symbols['coverage']['KairoLibrary']['mapped_first_difference']) + '/' + str(symbols['coverage']['KairoLibrary']['with_first_il_difference']) + ' Kairo e ' + str(symbols['coverage']['Assembly-CSharp']['mapped_first_difference']) + '/' + str(symbols['coverage']['Assembly-CSharp']['with_first_il_difference']) + ' App com primeira diferença de IL. [Prova](../reports/round4-symbol-mapping.json).', '',
             'O motor aplica transformações ao comando mapeado, conserva todos os bytes do prefixo já correto, exige avanço estrito ou corpo integralmente igual e rejeita regressões de corpos/metadados. Limites medidos por assembly:', '',
             table(['Assembly', 'Iterações por método', 'Candidatos por iteração', 'Limites de iteração atingidos', 'Corpos novos iguais', 'Bytes IL novos iguais'],
                   [(a, engines[a]['limit_iterations'], engines[a]['max_candidates_per_step'], engines[a]['iteration_limit_hits'],
                     len(engines[a]['body_gains']), engines[a]['matched_IL_gains']) for a in ASSEMBLIES]), '',
             '[Motor Kairo](../reports/round4-KairoLibrary-engine.json); [motor App](../reports/round4-Assembly-CSharp-engine.json). Ganho parcial de prefixo continua fora de matched até o verificador conferir corpo, locais e exceções completos.', '',
             '**Referências Kairo.** System 2.0 e System.Core 3.5 da Microsoft foram testados separadamente e juntos contra os equivalentes Unity, nos mesmos fontes congelados do commit `1047b1f`: zero corpos alterados, zero ganhos/perdas e 337/337 tipos em todas as combinações. A configuração selecionada continua Microsoft mscorlib 2.0, Unity System.Core 3.5, UnityEngine do APK, Unity System 2.0, nessa ordem. [Matriz com hashes](../reports/round4-reference-probes.json).', '',
             'As três diferenças brutas AssemblyRef dos controles congelados estão em `PublicKeyOrToken`, linhas 1/mscorlib, 2/System.Core e 4/System: índice #Blob **11271 → 11351**, bytes `07 2c → 57 2c`. Os três apontam para o mesmo token `b77a5c561934e089`; os valores numéricos são do baseline, não uma afirmação sobre os offsets/índices finais. Nenhuma referência foi corrigida por esses três bytes.', '',
             '**Opções de ensaio.** Roslyn `/shared` foi usado somente nos ensaios do motor e em sondas de transporte; na CLI preservou os 3287 métodos e metadados estruturais. No RSP foi rejeitado com CS2007, registrado como resultado negativo. `-debug+` smcs foi usado somente para o MDB. Os candidatos foram recompilados com as flags anteriores de produção e comparados integralmente antes da adoção. [Sondas com metadados](../reports/round4-shared-compiler-probe.json); [sonda independente](../reports/round4-compiler-server-probe.json).', '']
    screening = [(a, e.get('screening', {})) for a, e in engines.items() if e.get('screening', {}).get('baseline_checks')]
    if screening:
        lines += ['**Triagem opcional por fonte.** A triagem usa corpos C# provisórios nos outros arquivos apenas para acelerar tentativas descartáveis. Um controle deve provar a igualdade completa do corpo alvo contra o build real antes de habilitá-la por método. Todo candidato que pareça avançar passa novamente por build real completo, preservação do prefixo e prova de zero regressões antes de qualquer adoção. A DLL de produção não contém os corpos provisórios. Contraexemplos registrados impedem tratar essa otimização como uma equivalência universal. [Controles](../reports/round4-screen-validation.json).', '',
                  table(['Assembly', 'Controles do alvo', 'Habilitados', 'Recusados', 'Positivos conferidos no build real', 'Avanços falsos rejeitados'],
                        [(a, s['baseline_checks'], s['baseline_equal'], s['baseline_different_or_failed'], s['full_positive_checks'], s['false_positive_prefixes']) for a, s in screening]), '']
    lines += ['**Conteúdo Git.** Os C# extraídos do índice Git e normalizados conforme `.gitattributes` foram recompilados com os comandos de produção: zero corpos alterados e 466/466 tipos iguais. [Prova](../reports/round4-git-source-proof.json). Os hashes de evidência identificam os bytes medidos no workspace; o [manifesto de conteúdo](../reports/round4-git-content-provenance.json) também registra os hashes dos bytes commitados quando há conversão CRLF→LF. Essa conversão não altera DLLs/IL.', '',
              '**Bytes brutos: somente medição.** As [contagens finais de linhas](../reports/round4-final-raw-row-counts.json) dizem quais tabelas diferem em bytes e se têm a mesma quantidade de linhas. [Kairo](../' + raw['KairoLibrary']['measurement'] + '); [App](../' + raw['Assembly-CSharp']['measurement'] + '). Essas diferenças não foram usadas para editar bytes, heaps, tabelas ou estados.', '',
              'Tabelas com quantidades de linhas diferentes: ' + '; '.join(a + ': ' + (', '.join(t['table'] + ' ' + str(t['original_rows']) + '/' + str(t['rebuilt_rows']) for t in raw[a]['different_tables'] if not t['row_count_equal']) or 'nenhuma') for a in ASSEMBLIES) + '.', '',
              '**Limites históricos.** O executável histórico exato, o comando e as referências históricas completas e o determinismo dos originais continuam **UNKNOWN**. A lista de seis campos não determinísticos comprovados em duas compilações permanece inalterada em `config/toolchain.json`; nenhuma normalização foi aplicada. DEX/Java continua fora do escopo.', '',
              'As seções abaixo preservam o histórico de outras rodadas. Seus números, referências ou rótulos de configuração valem para os snapshots nelas descritos; o estado atual é o desta seção.', '']
    return '\n'.join(lines)


def prepend_current_section(document, section):
    title, separator, body = document.partition('\n')
    if title.rstrip('\r') != '# Toolchain e evidências' or not separator:
        raise ValueError('Unexpected toolchain document title')
    body = body.lstrip('\r\n')
    if body.startswith(CURRENT_HEADING):
        stop = body.find('\n## ', len(CURRENT_HEADING))
        if stop < 0:
            raise ValueError('Existing current section has no preserved historical boundary')
        body = body[stop + 1:]
    body = body.replace(OLD_HEADING, '## Histórico — rodada iniciada em 8851963', 1)
    return '# Toolchain e evidências\n\n' + section.rstrip() + '\n\n' + body.rstrip() + '\n'


def main():
    # Deliberately no fallback to partial engine snapshots or stale receipts.
    baseline = read('reports/round4-baseline-measurement.json')['assemblies']
    final = read('reports/round4-final-measurement.json')['assemblies']
    engines = {a: read('reports/round4-' + a + '-engine.json') for a in ASSEMBLIES}
    adoption = read('reports/round4-adoption.json')
    raw = read('reports/round4-final-raw-row-counts.json')['assemblies']
    receipts = {a: read('build/verification-' + a + '.json') for a in ASSEMBLIES}
    validate_current_evidence(baseline, final, engines, adoption, receipts, raw)
    symbols = read('reports/round4-symbol-mapping.json')
    if symbols['smcs']['canonical_whole_bodies_changed'] or symbols['smcs']['raw_method_bodies_changed']:
        raise ValueError('smcs symbol probe changed method bodies')
    config_path, doc_path = ROOT / 'config/toolchain.json', ROOT / 'docs/toolchain.md'
    before = read('config/toolchain.json')
    updated = copy.deepcopy(before)
    for assembly in ASSEMBLIES:
        current, f = updated['assemblies'][assembly], final[assembly]
        receipt = receipts[assembly]
        manifest_path = (ROOT / receipt['build_artifact']).parent / 'build-manifest.json'
        manifest = json.loads(manifest_path.read_text(encoding='utf-8-sig'))
        production_flags = [flag for flag in manifest['flags'] if not flag.startswith(('/out:', '-out:'))]
        if production_flags != current['flags'] or normalized_references(manifest['references']) != normalized_references(current['reference_files']):
            raise ValueError('Selected production flags/references no longer match config: ' + assembly)
        current['validation_scope'] = ('complete source build; ' + str(receipt['counts']['body_equal']) + '/' + str(f['total_methods'] - receipt['counts']['declarations_without_body']) +
                                       ' bodies equal; ' + str(f['matched_methods']) + '/' + str(f['total_methods']) + ' matched; ' +
                                       str(f['matched_original_il_bytes']) + '/' + str(f['original_total_il_bytes']) + ' original IL bytes matched; ' +
                                       str(receipt['types_equal']) + '/' + str(receipt['types_total']) +
                                       ' structural type metadata equal; whole assembly byte identity remains false; historical compiler UNKNOWN')
        # This records the newly measured experiment, not a production flag edit.
        current['debug_tested'] = True
    evidence_paths = ['reports/round4-adoption.json', 'reports/round4-baseline-measurement.json',
                      'reports/round4-final-measurement.json', 'reports/round4-symbol-mapping.json',
                      'reports/round4-reference-probes.json', 'reports/round4-shared-compiler-probe.json',
                      'reports/round4-compiler-server-probe.json', 'reports/round4-final-raw-row-counts.json',
                      'tools/round4_engine.py', 'tools/Round4TransformSyntax.cs', 'tools/round4_update_toolchain.py']
    if (ROOT / 'reports/round4-screen-validation.json').exists():
        evidence_paths.append('reports/round4-screen-validation.json')
    if (ROOT / 'reports/round4-git-source-proof.json').exists():
        evidence_paths.append('reports/round4-git-source-proof.json')
    for assembly in ASSEMBLIES:
        evidence_paths += ['reports/round4-' + assembly + '-engine.json',
                          'build/verification-' + assembly + '.json', raw[assembly]['measurement']]
    updated['round4_first_divergence_validation'] = {
        'baseline_commit': '1047b1fce99f8407ef3f1f38fd19ded6823d09c2',
        'operation': 'official full production verification after first-divergence source reconstruction',
        'production_configuration_unchanged': True, 'new_non_deterministic_exceptions': [], 'normalization_applied': False,
        'evidence_files': [{'path': path, 'sha256': sha(ROOT / path)} for path in dict.fromkeys(evidence_paths)],
        'assemblies': {a: {'matched': final[a]['matched_methods'], 'total_methods': final[a]['total_methods'],
                           'matched_original_il_bytes': final[a]['matched_original_il_bytes'], 'original_total_il_bytes': final[a]['original_total_il_bytes'],
                           'body_equal': receipts[a]['counts']['body_equal'], 'types_equal': receipts[a]['types_equal'], 'types_total': receipts[a]['types_total'],
                           'official_build_sha256': receipts[a]['build_sha256'], 'whole_assembly_byte_identity': False,
                           'limit_iterations': engines[a]['limit_iterations'], 'max_candidates_per_iteration': engines[a]['max_candidates_per_step'],
                           'iteration_limit_hits': engines[a]['iteration_limit_hits'], 'body_losses': engines[a]['body_losses'],
                           'mapping': 'native PDB sequence points' if a == 'KairoLibrary' else 'MDB sequence points after per-method debug-body equality proof',
                           'trial_only_flags': ['/shared (direct compiler client command line)'] if a == 'KairoLibrary' else ['-debug+ (MDB mapping only)']}
                       for a in ASSEMBLIES}}
    unchanged_selection(before, updated)
    section = make_current_section(updated, baseline, final, engines, receipts, raw, symbols)
    new_document = prepend_current_section(doc_path.read_text(encoding='utf-8-sig'), section)
    # All proofs and both complete documents are prepared before either write.
    config_path.write_text(json.dumps(updated, indent=2, ensure_ascii=False) + '\n', encoding='utf-8')
    doc_path.write_text(new_document, encoding='utf-8')
    print(json.dumps({'updated': ['config/toolchain.json', 'docs/toolchain.md'], 'production_selection_changed': False,
                      'method_or_type_states_changed': False, 'progress_file_changed': False}))


if __name__ == '__main__':
    main()
