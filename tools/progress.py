"""Generate progress from methods.csv and types.csv without inventing matches."""
from collections import Counter, defaultdict
import csv
import json
from pathlib import Path
from verification_state import validate_matched
from metadata_state import validate_type_metadata
from kairo_candidate import PACKAGE_VERSION
from round12_progress_status import kairo_completion, app_completion
ROOT=Path(__file__).resolve().parents[1]


def read_csv(name):
    with (ROOT/'config'/name).open(encoding='utf-8',newline='') as file:
        return list(csv.DictReader(file))


def main():
    methods=read_csv('methods.csv'); types=read_csv('types.csv'); inventory=read_csv('inventory.csv')
    if len({m['id'] for m in methods})!=len(methods): raise ValueError('Duplicate method IDs')
    allowed={'todo','decompiled','equivalent','matched','external'}
    if any(m['estado'] not in allowed for m in methods): raise ValueError('Invalid method state')
    validate_matched(ROOT,methods)
    metadata=validate_type_metadata(ROOT,types)
    matched_types=sum(t['metadados']=='matched' for t in types)
    states=Counter(m['estado'] for m in methods)
    total_il=sum(int(m['tamanho_il']) for m in methods)
    matched=[m for m in methods if m['estado']=='matched']
    matched_il=sum(int(m['tamanho_il']) for m in matched)
    summary_path=ROOT/'reports/KairoLibrary-verification.json'
    verified=json.loads(summary_path.read_text(encoding='utf-8'))['counts'] if summary_path.is_file() else {}
    categories=Counter(r['categoria'] for r in inventory)
    closure=kairo_completion(ROOT)
    app_closure=app_completion(ROOT)
    lines=['# Progresso dos métodos C#','',
        'Gerado por `python tools/progress` a partir de `config/methods.csv` e `config/types.csv`.','',
        f'**{len(matched):,} de {len(methods):,} métodos matched ({100*len(matched)/len(methods):.2f}%).** {total_il:,} bytes de IL nos assemblies alvo.',
        f'{matched_il:,} bytes IL matched ({100*matched_il/total_il:.2f}%).',
        'Cabeçalhos, locais e exceções também precisam corresponder; o denominador acima mede somente bytes IL.','',
        f'Ambos os assemblies foram recuperados e compilam integralmente. KairoLibrary usa o candidato Roslyn {PACKAGE_VERSION}; Assembly-CSharp usa smcs 2.6.5.0.','',
        f"KairoLibrary: {verified.get('body_equal',0)} corpos correspondem com tokens resolvidos; {verified.get('equal_body_held_for_generated_group',0)} ficam retidos por dependências de membros gerados. Declarações sem CIL só entram em matched com prova completa dos metadados do método e do tipo.",'',
        '## Estados','', '| Estado | Métodos |','|---|---:|']
    if closure['complete']:
        lines[2:2]=['**KairoLibrary: CONCLUÍDA.** 3.287 métodos, 337 tipos e todos os bytes fora dos quatro campos comprovadamente não determinísticos correspondem ao original.',
            f"Prova literal atual: [{closure['report']}]({closure['report']}); SHA-256 do PE recompilado `{closure['recompiled_sha256']}`. Sem normalização.",'']
    elif closure['status']=='RAW_RENEWAL_REQUIRED':
        lines[2:2]=['KairoLibrary: prova bruta aguardando renovação para o PE/fonte atual; a correspondência de métodos não substitui essa prova.','']
    if app_closure['complete']:
        lines[2:2]=['**Assembly-CSharp: CONCLUÍDA.** 1.958 métodos, 129 tipos e todos os bytes fora dos dois campos comprovadamente não determinísticos correspondem ao original.',
            f"Prova literal atual: `{app_closure['report']}`; SHA-256 do PE recompilado `{app_closure['recompiled_sha256']}`. Evidência integral em gzip lossless, sem normalização.",'']
    lines += [f'| {state} | {states[state]} |' for state in ('todo','decompiled','equivalent','matched','external')]
    for label,key in (('Assemblies','assembly'),('Namespaces','namespace'),('Tipos','tipo')):
        groups=defaultdict(list)
        namespace={(t['assembly'],t['tipo']):t['namespace'] or '(global)' for t in types}
        for m in methods:
            group=namespace[(m['assembly'],m['tipo'])] if key=='namespace' else m[key]
            if key=='tipo': group=m['assembly']+'::'+group
            groups[group].append(m)
        lines += ['',f'## {label}','','| Grupo | Métodos | Bytes IL | Métodos matched | Bytes IL matched |','|---|---:|---:|---:|---:|']
        for name,group in sorted(groups.items()):
            done=[m for m in group if m['estado']=='matched']
            size=sum(int(m['tamanho_il']) for m in group); done_size=sum(int(m['tamanho_il']) for m in done)
            lines.append(f"| `{name.replace('|','&#124;')}` | {len(group)} | {size} | {len(done)} / {100*len(done)/len(group):.2f}% | {done_size} / {100*done_size/size if size else 0:.2f}% |")
    lines += ['','## Metadados dos tipos','',f'{matched_types} de {len(types)} tipos com metadados estruturais e ordinal verificados como idênticos. Inclui `<Module>` e tipos gerados; heaps e bytes integrais são uma camada separada.','',
        '## Arquivos externos e dados','',
        f"{len(inventory)} entradas: {categories['alvo']} alvo, {categories['externo']} externas e {categories['dados']} dados.",
        'Externos e dados não entram na porcentagem de código. DEX e plugins nativos não ampliam o escopo C#.','',
        '## Próximos tipos e métodos','',
        ('Nenhum método C# pendente.' if len(matched)==len(methods) else 'Priorizar tipos com fonte compilado e menos pendências; ajustar um método por vez.'),'',
        '| Tipo | Pendentes | Primeiro método |','|---|---:|---|']
    by_type=defaultdict(list)
    for m in methods: by_type[(m['assembly'],m['tipo'])].append(m)
    pending=lambda t:[m for m in by_type[(t['assembly'],t['tipo'])] if m['estado']!='matched']
    candidates=[t for t in types if t['fonte'] and t['gerado_pelo_compilador']=='false' and pending(t)]
    candidates.sort(key=lambda t:(not (ROOT/t['fonte']).is_file(),len(pending(t)),t['assembly'],int(t['ordem'])))
    for t in candidates[:10]:
        group=pending(t)
        lines.append(f"| `{t['assembly']}::{t['tipo']}` | {len(group)} | `{group[0]['id']}` |")
    generated=sum(m['gerado_por']=='UNKNOWN' for m in methods)
    lines += ['','## Bloqueios','',
        f'- Versão histórica exata dos compiladores: UNKNOWN; pacote Roslyn {PACKAGE_VERSION} é o melhor candidato testado para KairoLibrary.',
        ('- Assembly-CSharp: todos os corpos e vínculos gerados foram aprovados.' if len(matched)==len(methods) else '- Assembly-CSharp: configuração stdlib default validada; os corpos restantes ainda exigem recuperação.'),
        ('- Os dois assemblies estão concluídos pelo critério literal, com as exceções comprovadas.' if closure['complete'] and app_closure['complete'] else '- Todos os tipos e metadados de definição têm identidade estrutural comprovada; a camada literal requer prova própria.' if matched_types==len(types) else '- Comparação estrutural dos tipos implementada; persistem diferenças de metadados e bytes brutos.'),
        f'- {generated} membros têm campo gerado_por UNKNOWN; propriedades e caches compartilhados podem ter vínculo comprovado no relatório sem uma origem única representável nesse campo.',
        '- Campos variáveis comprovados: quatro em Kairo e dois em App; nenhuma normalização de assembly foi aplicada.',
        '', 'Plano: [DECOMP_PLAN.md](DECOMP_PLAN.md). Ferramentas: [docs/toolchain.md](docs/toolchain.md).','']
    (ROOT/'PROGRESS.md').write_text('\n'.join(lines),encoding='utf-8')
    print(json.dumps({'methods':len(methods),'types':len(types),'il_bytes':total_il,'matched':len(matched),'matched_il_bytes':matched_il,'categories':dict(categories)}))


if __name__=='__main__': main()
