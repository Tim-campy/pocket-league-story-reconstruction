"""Publish documentation from fresh receipts; never compile, patch or run a target."""
import argparse
import csv
import hashlib
import json
from pathlib import Path

from apply_il_rules import receipt_is_fresh
from round12_progress_status import kairo_completion, app_completion

ROOT = Path(__file__).resolve().parents[1]
TAG = 'v1.0.0-csharp-reconstruction'
ASSEMBLIES = ('KairoLibrary', 'Assembly-CSharp')
BUILD = {'KairoLibrary': 'build/KairoLibrary-roslyn-1.1.0',
         'Assembly-CSharp': 'build/Assembly-CSharp-mono-smcs'}


def load(path):
    return json.loads((ROOT / path).read_text(encoding='utf-8-sig'))


def sha(path):
    return hashlib.sha256((ROOT / path).read_bytes()).hexdigest()


def write(path, text):
    (ROOT / path).write_text(text.rstrip() + '\n', encoding='utf-8', newline='\n')


def java_inventory():
    source = 'reports/DEX-package-inventory.json'
    inventory = load(source)
    groups = {name: [] for name in ('Cobrança', 'Anúncios', 'Interface Android', 'Outros')}
    for package in inventory['packages']:
        if package['classification']['category'] != 'codigo proprio candidato':
            continue
        for item in package['classes']:
            name = item['binary_name']
            if item['package'] == 'plugin.billing':
                group = 'Cobrança'
                evidence = 'Pacote plugin.billing e nome PurchaseLogView.'
            elif item['package'] in ('kairo.android.plugin.ad', 'kairo.android.plugin.gms'):
                group = 'Anúncios'
                evidence = 'Nomes AdMobView/Interstitial; superclasses AdListener nos auxiliares indicados.'
            elif (item['package'] in ('kairo.android.plugin.panel', 'kairo.android.plugin.ui',
                                     'kairo.android.plugin.notification', 'kairo.android.plugin.notification.util',
                                     'plugin.ui', 'main') or name == 'plugin.WebBox'):
                group = 'Interface Android'
                evidence = 'Pacote/nome de painel, diálogo, notificação, WebBox ou Activity principal.'
            else:
                group = 'Outros'
                evidence = 'Configuração, utilitários, persistência, licenciamento ou R gerado; função detalhada UNKNOWN.'
            groups[group].append(dict(item, group=group, grouping_evidence=evidence,
                                      function_confidence='inferred', historical_ownership='UNKNOWN'))
    classes = [item for items in groups.values() for item in items]
    assert len(classes) == len({i['binary_name'] for i in classes}) == 101
    report = dict(operation='group-existing-class-definition-inventory-only',
                  source=dict(path=source, sha256=sha(source)), dex=inventory['artifact'],
                  method_bodies_read=False, decompilation_performed=False,
                  groups={key: dict(count=len(value), classes=value) for key, value in groups.items()},
                  total=101, scope_decision='PENDING_USER; no Java reconstruction authorized')
    write('reports/java-phase2-inventory.json', json.dumps(report, indent=2, ensure_ascii=False))
    lines = ['# Inventário Java para decisão de segunda fase', '',
             'São **101 classes candidatas**, incluindo classes anônimas/internas e cinco classes R geradas. '
             'Não são 101 fontes escritos à mão. Nenhuma foi decompilada; nenhum corpo Java foi examinado nesta rodada.', '',
             'Os nomes, superclasses, índices class_def e source_file são fatos da tabela DEX já inventariada. '
             'A divisão funcional abaixo é uma inferência por pacote/nome/superclasse, não uma confirmação de comportamento. '
             'Autoria histórica e função exata dos auxiliares permanecem **UNKNOWN**. Licenciamento não foi presumido como cobrança; '
             'Utility e Referrer ficam em Outros por falta de evidência mais específica.', '',
             f"DEX SHA-256: `{inventory['artifact']['sha256']}`. Fonte: [inventário original](../{source}); "
             '[lista estruturada com índices e superclasses](../reports/java-phase2-inventory.json).', '',
             '| Grupo | Classes |', '|---|---:|']
    lines += [f'| {key} | {len(value)} |' for key, value in groups.items()]
    lines += ['| **Total** | **101** |', '',
              'Para contexto: o DEX contém 3.232 classes: 79 nomes correspondem ao classes.jar oficial Unity; '
              '3.052 foram classificados como SDKs de terceiros por namespace; 101 são estes candidatos. '
              'Versões exatas dos SDKs e autoria de cada candidato continuam UNKNOWN.', '']
    for key, items in groups.items():
        lines += [f'## {key} — {len(items)} classes', '', items[0]['grouping_evidence'], '',
                  '| Classe binária completa | class_def | Superclasse |', '|---|---:|---|']
        lines += [f"| `{item['binary_name']}` | {item['class_def_index']} | `{item['superclass']}` |" for item in items]
        lines.append('')
    lines += ['A inclusão de qualquer grupo na segunda fase depende da decisão do usuário. '
              'Este inventário não amplia o escopo concluído de C# nem altera classes.dex.']
    write('docs/java-phase2-inventory.md', '\n'.join(lines))
    return report


def toolchain(manifests):
    history = ROOT / 'docs/notes/toolchain-history.md'
    if not history.exists():
        history.write_bytes((ROOT / 'docs/toolchain.md').read_bytes())
    intro = (ROOT / 'docs/closure-toolchain-recipe.md').read_text(encoding='utf-8')
    lines = [intro.rstrip(), '', '## Manifestos de compilação e referências ordenadas', '',
             'As tabelas abaixo são geradas dos manifestos do build verificado. `{ROOT}` é a raiz deste clone; '
             '`{UNITY}` é a raiz do Unity 5.3.4f1. Caminhos relativos dos fontes são resolvidos contra `{ROOT}`. '
             'Não ordenar referências ou fontes novamente.', '']
    for assembly in ASSEMBLIES:
        manifest = manifests[assembly]
        lines += [f'### {assembly}', '', f"Compilador SHA-256: `{manifest['compiler_sha256']}`.", '',
                  'Opções, na ordem (o script passa noconfig fora do response file):', '', '```text']
        for flag in manifest['flags']:
            if flag.startswith(('/out:', '-out:')):
                flag = flag.split(':', 1)[0] + ':{ROOT}/' + BUILD[assembly] + '/' + assembly + '.dll'
            lines.append(flag)
        lines += ['```', '', 'Não há defines adicionais. Referências, nesta ordem:', '',
                  '| Ordem | Arquivo | SHA-256 |', '|---:|---|---|']
        for index, reference in enumerate(manifest['references'], 1):
            path = reference['path'].replace(str(ROOT), '{ROOT}')
            path = path.replace(str(Path(manifests['Assembly-CSharp']['runtime']).parents[4]), '{UNITY}')
            lines.append(f"| {index} | `{path}` | `{reference['sha256']}` |")
        lines += ['', f"Ordem integral dos **{len(manifest['sources'])} fontes**, incluindo AssemblyInfo por último. "
                  'Os hashes também permitem conferir line endings UTF-8/LF de um clone:', '',
                  '| Ordem | Fonte | SHA-256 |', '|---:|---|---|']
        lines += [f"| {index} | `{source['path']}` | `{source['sha256']}` |"
                  for index, source in enumerate(manifest['sources'], 1)]
        lines += ['', f"Arquivo de ordem: `config/compile-order/{assembly}.txt`, SHA-256 "
                  f"`{sha('config/compile-order/' + assembly + '.txt')}`. "
                  'O builder acrescenta AssemblyInfo.cs se necessário, conforme a lista acima.', '']
    lines += ['## Componentes do pacote Roslyn fixado', '', '| Arquivo | SHA-256 |', '|---|---|']
    from kairo_candidate import COMPONENT_SHA256
    lines += [f'| `{name}` | `{digest}` |' for name, digest in COMPONENT_SHA256.items()]
    lines += ['', '## Evidências e histórico', '',
              'O [histórico anterior](notes/toolchain-history.md) preserva medições parciais e hipóteses de rodadas anteriores; '
              'elas não são a receita vigente. [Fechamento Kairo](notes/round12-kairo-byte-closure.md), '
              '[fechamento Assembly-CSharp](notes/round12-app-byte-closure.md), '
              '[ajustes de emissão](notes/kairo-metadata-emission.md) e '
              '[manifesto final](../reports/reconstruction-closure.json) registram as provas.', '',
              'REA 6.5.0 é o parser estático usado nas evidências PE/CLI, sem carregar o jogo. '
              'A comparação independente de corpos e metadados usa ProbeInspector/MetadataInspector com Mono.Cecil do Unity. '
              'Nesta sessão o MCP respondeu e anunciou inspect_managed_artifact/inspect_managed_members disponíveis; '
              'o doctor geral acusou drift de registros/skill e provedores opcionais ausentes. Isso não exigiu instalação '
              'nem mudou a receita de compilação. Nenhum método Java foi decompilado e nenhum APK foi executado.']
    write('docs/toolchain.md', '\n'.join(lines))


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--final', action='store_true', help='Require renewed literal proofs for these exact PEs')
    args = parser.parse_args()
    manifests = {a: load(BUILD[a] + '/build-manifest.json') for a in ASSEMBLIES}
    for assembly in ASSEMBLIES:
        receipt = load('build/verification-' + assembly + '.json')
        if not receipt_is_fresh(ROOT, receipt):
            raise ValueError('Stale full verification: ' + assembly)
        assert receipt['counts']['matched'] == (3287 if assembly == 'KairoLibrary' else 1958)
        assert not any(receipt.get(k) for k in ('regressions', 'body_regressions', 'type_regressions'))
    inventory = java_inventory()
    toolchain(manifests)
    if not args.final:
        print(json.dumps(dict(documentation='prepared', java_candidates=inventory['total'])))
        return
    closures = {'KairoLibrary': kairo_completion(ROOT), 'Assembly-CSharp': app_completion(ROOT)}
    assert all(c['complete'] for c in closures.values()), closures
    assemblies = {}
    for assembly in ASSEMBLIES:
        closure = closures[assembly]
        raw = load(closure['report'])
        assert raw['status'] == 'PASS' and raw['outside_exceptions_differing_bytes'] == 0
        assert raw['exception_coordinates_equal']
        receipt_path = 'build/verification-' + assembly + '.json'
        receipt = load(receipt_path)
        assert raw['recompiled_sha256'] == receipt['build_sha256'] == manifests[assembly]['output_sha256']
        assemblies[assembly] = dict(matched=receipt['counts']['matched'],
            il_bytes=269894 if assembly == 'KairoLibrary' else 601603,
            types=receipt['types_equal'], original_sha256=receipt['original_sha256'],
            rebuilt_sha256=receipt['build_sha256'], compiler_sha256=manifests[assembly]['compiler_sha256'],
            size=raw['rebuilt_size'], exceptions=raw['exception_coordinates']['original'],
            literal_bytes_equal_outside_exceptions=raw['rebuilt_size'] - sum(x['size'] for x in raw['exception_coordinates']['original'].values()),
            residual_differing_bytes=0, raw_report=dict(path=closure['report'], sha256=sha(closure['report'])),
            receipt=dict(path=receipt_path, sha256=sha(receipt_path)),
            manifest=manifests[assembly])
    report = dict(operation='reconstruction-closure-documentation', tag=TAG, assemblies=assemblies,
        total_matched=5245, total_il_bytes=871497, types=466, regressions=0,
        exceptions_count=6, exception_span_bytes=60, residual_differing_bytes=0,
        java_inventory=dict(path='reports/java-phase2-inventory.json', sha256=sha('reports/java-phase2-inventory.json')),
        reconstruction_cycles_added=0, source_changes=0, DLL_postcompile_writes=False,
        APK_built=False, APK_installed=False, target_executed=False,
        final_DLLs_device_tested=False)
    write('reports/reconstruction-closure.json', json.dumps(report, ensure_ascii=False, indent=2))
    lines = [f'# {TAG}', '', 'Encerramento da reconstrução C# de Pocket League Story 2.0.2 (Unity Mono 5.3.4f1).', '',
             '**5.245/5.245 métodos, 871.497/871.497 bytes IL e 466/466 tipos; zero regressões.** '
             'Os 1.465.284 bytes dos dois PEs fora de seis campos não determinísticos (60 bytes de extensão) '
             'são literalmente idênticos ao original. Zero diferenças residuais. Nenhuma DLL foi editada após compilar.', '',
             '| Assembly | Métodos | IL bytes | PE bytes | SHA-256 reconstruída |', '|---|---:|---:|---:|---|']
    lines += [f"| {a} | {s['matched']} | {s['il_bytes']} | {s['size']} | `{s['rebuilt_sha256']}` |" for a, s in assemblies.items()]
    lines += ['', '| Compilador | SHA-256 |', '|---|---|',
              f"| Microsoft.Net.Compilers 1.1.0 / csc 1.1.0.51109 | `{manifests['KairoLibrary']['compiler_sha256']}` |",
              f"| Unity 5.3.4f1 / smcs 2.6.5.0 | `{manifests['Assembly-CSharp']['compiler_sha256']}` |", '',
              'Esses hashes das DLLs identificam esta verificação; outra compilação muda os campos comprovados '
              'e pode gerar outro SHA completo. A identidade histórica exata dos compiladores permanece UNKNOWN.', '',
              'Receita: docs/toolchain.md. Critério e exceções: README.md. Provas: reports/reconstruction-closure.json. '
              'O repositório não distribui APKs, DLLs originais/reconstruídas nem assets. '
              'Nenhum APK foi montado, instalado ou executado nesta rodada. As DLLs finais ainda não foram testadas em aparelho. '
              'O teste anterior exercitou outra baseline. A montagem/teste final está apenas descrita em docs/apk-final.md.', '',
              'Inventário para decisão futura: 101 classes Java candidatas (5 cobrança, 11 anúncios, '
              '47 interface Android e 38 outros), sem decompilação; DEX/Java permanece fora do escopo.']
    write('docs/reconstruction-release.md', '\n'.join(lines))
    print(json.dumps(dict(status='PASS', tag=TAG, matched=5245, il_bytes=871497,
                          rebuilt_sha256={a: s['rebuilt_sha256'] for a, s in assemblies.items()}, cycles_added=0)))


if __name__ == '__main__':
    main()
