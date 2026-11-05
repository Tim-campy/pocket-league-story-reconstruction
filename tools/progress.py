"""Generate progress from methods.csv and types.csv without inventing matches."""
from collections import Counter, defaultdict
import csv
import json
from pathlib import Path
from verification_state import validate_matched
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
    if any(t['metadados']=='matched' for t in types):
        raise ValueError('Type matching requires a separate metadata receipt')
    states=Counter(m['estado'] for m in methods)
    total_il=sum(int(m['tamanho_il']) for m in methods)
    matched=[m for m in methods if m['estado']=='matched']
    matched_il=sum(int(m['tamanho_il']) for m in matched)
    summary_path=ROOT/'reports/KairoLibrary-verification.json'
    verified=json.loads(summary_path.read_text(encoding='utf-8'))['counts'] if summary_path.is_file() else {}
    categories=Counter(r['categoria'] for r in inventory)
    lines=['# Progresso dos métodos C#','',
        'Gerado por `python tools/progress` a partir de `config/methods.csv` e `config/types.csv`.','',
        f'**{len(matched):,} de {len(methods):,} métodos matched ({100*len(matched)/len(methods):.2f}%).** {total_il:,} bytes de IL nos assemblies alvo.',
        f'{matched_il:,} bytes IL matched ({100*matched_il/total_il:.2f}%).',
        'Cabeçalhos, locais e exceções também precisam corresponder; o denominador acima mede somente bytes IL.','',
        'KairoLibrary compilada integralmente com dois compiladores. Assembly-CSharp ainda sem fonte recuperado.','',
        f"KairoLibrary: {verified.get('body_equal',0)} corpos correspondem com tokens resolvidos; {verified.get('equal_body_held_for_generated_group',0)} ficam retidos por dependências de membros gerados. Metadados dos tipos e identidade do assembly inteiro continuam pendentes.",'',
        '## Estados','', '| Estado | Métodos |','|---|---:|']
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
    lines += ['','## Metadados dos tipos','',f'0 de {len(types)} tipos com metadados verificados como idênticos. Inclui `<Module>` e tipos gerados.','',
        '## Arquivos externos e dados','',
        f"{len(inventory)} entradas: {categories['alvo']} alvo, {categories['externo']} externas e {categories['dados']} dados.",
        'Externos e dados não entram na porcentagem de código. DEX e plugins nativos não ampliam o escopo C#.','',
        '## Próximos tipos e métodos','',
        'Priorizar tipos com fonte compilado e menos pendências; ajustar um método por vez.','',
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
        '- Versão histórica exata dos compiladores: UNKNOWN; pacote Roslyn 1.3.2 é o melhor candidato testado para KairoLibrary.',
        '- Assembly-CSharp: decompilação e compilação integral pendentes; quatro sondas Mono passaram.',
        '- Comparação completa de metadados por tipo, heaps e assembly ainda pendente.',
        f'- {generated} membros têm campo gerado_por UNKNOWN; propriedades e caches compartilhados podem ter vínculo comprovado no relatório sem uma origem única representável nesse campo.',
        '- Nenhuma exceção de MVID, timestamp ou outro campo está autorizada.',
        '', 'Plano: [DECOMP_PLAN.md](DECOMP_PLAN.md). Ferramentas: [docs/toolchain.md](docs/toolchain.md).','']
    (ROOT/'PROGRESS.md').write_text('\n'.join(lines),encoding='utf-8')
    print(json.dumps({'methods':len(methods),'types':len(types),'il_bytes':total_il,'matched':len(matched),'matched_il_bytes':matched_il,'categories':dict(categories)}))


if __name__=='__main__': main()
