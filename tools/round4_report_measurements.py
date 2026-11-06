"""Render complete, self-contained size/first-pair measurements as Markdown."""
import argparse
import json
from pathlib import Path
import re

ROOT = Path(__file__).resolve().parents[1]


def _cell(value):
    return str(value).replace('|', '\\|').replace('\n', '<br>')


def _inline(value):
    value = _cell(value)
    fence = '`' * (max((len(run) for run in re.findall(r'`+', value)), default=0) + 1)
    return fence + value + fence


def _instruction(instruction):
    value = instruction['opcode']
    if instruction.get('operand') is not None:
        value += ' ' + json.dumps(instruction['operand'], ensure_ascii=False, sort_keys=True, separators=(',', ':'))
    return value


def _context(row, side):
    context = row[side + '_context']
    instruction = row['first_' + side + '_instruction']
    if not context:
        return _inline(_instruction(instruction))
    index = next((i for i, item in enumerate(context) if item == instruction), 0)
    chosen = context[max(0, index - 1):index + 2]
    return '<br>'.join(_inline('IL_' + format(item['offset'], '04x') + ': ' + _instruction(item)) for item in chosen)


def measurement_markdown(jsonfile):
    """Return all exact groups, all counts and full operands; no top-N cutoff."""
    source = Path(jsonfile)
    document = json.loads(source.read_text(encoding='utf-8-sig'))
    assemblies = document['assemblies']
    lines = ['**Medição 1a — distribuição dos corpos divergentes.**', '',
             'Os tamanhos são os bytes de IL originais. Cabeçalhos, assinatura de locais e regiões de exceção não entram neste denominador.', '',
             '| Assembly / compilador | Faixa de bytes IL | Métodos | Bytes IL originais |',
             '|---|---|---:|---:|']
    for name, assembly in assemblies.items():
        for bin_name, values in assembly['size_distribution'].items():
            lines.append('| ' + _inline(name + ' / ' + assembly['compiler']) + ' | ' + _inline(bin_name) + ' | ' + str(values['methods']) + ' | ' + str(values['original_il_bytes']) + ' |')
        lines.append('| ' + _inline(name + ' total divergente') + ' | todas | ' + str(assembly['divergent_bodies']) + ' | ' + str(assembly['divergent_original_il_bytes']) + ' |')
    lines += ['', '**Medição 1b — histograma completo da primeira divergência.**', '',
              'A posição é o primeiro byte de IL desigual depois de resolver somente tokens de metadados. O byte pode estar dentro de um operando: dois opcodes iguais com destinos de desvio diferentes continuam sendo uma diferença. A orientação de um desvio não é normalizada. A instrução que contém esse byte define o par. Cabeçalho, locais e exceções continuam obrigatórios para o veredito final do método.', '',
              'Cada linha abaixo é um grupo exato de opcode e operando, sem truncamento. O exemplo mostra uma instrução anterior e uma posterior quando disponíveis. Operandos de metadados conservam identidade e escopo completos; destinos são offsets absolutos dentro do método. `IL_IDENTICAL` significa que somente outra parte do corpo diverge. Cada corpo divergente aparece em exatamente um grupo.', '']
    for name, assembly in assemblies.items():
        groups = assembly['first_pair_exact_histogram']
        if sum(group['methods'] for group in groups) != assembly['divergent_bodies']:
            raise ValueError('Histogram does not account for every divergent body')
        lines += ['**' + name + ' / ' + assembly['compiler'] + ': ' + str(len(groups)) + ' grupos exatos; ' + str(assembly['divergent_bodies']) + ' métodos.**', '',
                  '| Métodos | Instrução original | Instrução recompilada | ID completo do exemplo | Offset divergente | Contexto original | Contexto recompilado |',
                  '|---:|---|---|---|---:|---|---|']
        for group in groups:
            example = group['example']
            offset = example['first_il_difference_offset']
            lines.append('| ' + ' | '.join((str(group['methods']), _inline(_instruction(example['first_original_instruction'])),
                                             _inline(_instruction(example['first_rebuilt_instruction'])), _inline(example['id']),
                                             'IL_' + format(offset, '04x') if offset is not None else 'IL idêntico',
                                             _context(example, 'original'), _context(example, 'rebuilt'))) + ' |')
        lines.append('')
    lines += ['**Projeção dos pares por opcode, usada somente para prioridade.**', '',
              'Esta tabela agrupa operandos diferentes sob o mesmo par de opcodes. Não altera o histograma exato acima nem a comparação do verificador.', '']
    for name, assembly in assemblies.items():
        lines += ['**' + name + ' / ' + assembly['compiler'] + '.**', '',
                  '| Par de opcodes original → recompilado | Métodos | Bytes IL originais dos métodos |',
                  '|---|---:|---:|']
        for group in assembly['first_pair_opcode_histogram']:
            lines.append('| ' + _inline(group['pair']) + ' | ' + str(group['methods']) + ' | ' + str(group['original_il_bytes']) + ' |')
        lines.append('')
    lines += ['**Proveniência da medição.**', '',
              'SHA-256 da lista de métodos: ' + _inline(document['ledger_sha256']) + '.', '']
    for name, assembly in assemblies.items():
        provenance = assembly['provenance']
        lines.append(name + ': SHA-256 do assembly recompilado ' + _inline(provenance['build_sha256']) + '; dump original ' + _inline(provenance['original_dump_sha256']) + '; dump recompilado ' + _inline(provenance['rebuilt_dump_sha256']) + '.')
        lines.append('')
    return '\n'.join(lines)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('measurement', type=Path)
    parser.add_argument('--output', type=Path, default=ROOT / 'reports/round4-baseline-measurements.md')
    args = parser.parse_args()
    rendered = measurement_markdown(args.measurement)
    args.output.write_text(rendered.rstrip() + '\n', encoding='utf-8', newline='\n')
    print(json.dumps({'output': str(args.output), 'utf8_bytes': len(rendered.encode('utf-8')), 'lines': len(rendered.splitlines())}))


if __name__ == '__main__':
    main()
