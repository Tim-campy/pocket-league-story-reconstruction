"""Generate progress from methods.csv and types.csv without inventing matches."""
from collections import Counter, defaultdict
import csv
import json
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1]


def read_csv(name):
    with (ROOT/'config'/name).open(encoding='utf-8',newline='') as file:
        return list(csv.DictReader(file))


def main():
    methods=read_csv('methods.csv'); types=read_csv('types.csv'); inventory=read_csv('inventory.csv')
    if len({m['id'] for m in methods})!=len(methods): raise ValueError('Duplicate method IDs')
    allowed={'todo','decompiled','equivalent','matched','external'}
    if any(m['estado'] not in allowed for m in methods): raise ValueError('Invalid method state')
    if any(m['estado']=='matched' for m in methods) or any(t['metadados']=='matched' for t in types):
        raise ValueError('Matched requires the compiler-backed verifier; no receipts exist yet')
    states=Counter(m['estado'] for m in methods)
    total_il=sum(int(m['tamanho_il']) for m in methods)
    categories=Counter(r['categoria'] for r in inventory)
    lines=['# Progresso dos métodos C#','',
        'Gerado por `python tools/progress` a partir de `config/methods.csv` e `config/types.csv`.','',
        f'**0 de {len(methods):,} métodos matched (0%).** {total_il:,} bytes de IL nos assemblies alvo.',
        'Cabeçalhos, locais e exceções também precisam corresponder; o denominador acima mede somente bytes IL.','',
        'A referência está inventariada. A recompilação integral e a correspondência ainda estão pendentes.','',
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
            lines.append(f"| `{name.replace('|','&#124;')}` | {len(group)} | {sum(int(m['tamanho_il']) for m in group)} | 0 / 0% | 0 / 0% |")
    lines += ['','## Metadados dos tipos','',f'0 de {len(types)} tipos com metadados verificados como idênticos. Inclui `<Module>` e tipos gerados.','',
        '## Arquivos externos e dados','',
        f"{len(inventory)} entradas: {categories['alvo']} alvo, {categories['externo']} externas e {categories['dados']} dados.",
        'Externos e dados não entram na porcentagem de código. DEX e plugins nativos não ampliam o escopo C#.','',
        '## Próximos tipos e métodos','',
        'Após decompilar e compilar os assemblies inteiros, priorizar tipos com menos pendências e ajustar um método por vez.','',
        '| Tipo | Pendentes | Primeiro método |','|---|---:|---|']
    by_type=defaultdict(list)
    for m in methods: by_type[(m['assembly'],m['tipo'])].append(m)
    candidates=[t for t in types if t['fonte'] and t['gerado_pelo_compilador']=='false' and t['metodos']!='0']
    candidates.sort(key=lambda t:(len(by_type[(t['assembly'],t['tipo'])]),t['assembly'],int(t['ordem'])))
    for t in candidates[:10]:
        group=by_type[(t['assembly'],t['tipo'])]
        lines.append(f"| `{t['assembly']}::{t['tipo']}` | {len(group)} | `{group[0]['id']}` |")
    generated=sum(m['gerado_por']=='UNKNOWN' for m in methods)
    lines += ['','## Bloqueios','',
        '- Compilador e flags precisam ser validados separadamente para cada DLL.',
        '- Decompilador C# e comparação completa com resolução de tokens pendentes.',
        f'- {generated} membros gerados têm método de origem UNKNOWN.',
        '- Nenhuma exceção de MVID, timestamp ou outro campo está autorizada.',
        '', 'Plano: [DECOMP_PLAN.md](DECOMP_PLAN.md). Ferramentas: [docs/toolchain.md](docs/toolchain.md).','']
    (ROOT/'PROGRESS.md').write_text('\n'.join(lines),encoding='utf-8')
    print(json.dumps({'methods':len(methods),'types':len(types),'il_bytes':total_il,'matched':0,'categories':dict(categories)}))


if __name__=='__main__': main()
