"""Execute the four remaining explicit Part-B probes serially.

smcs tests are one occurrence of one family: no slot/flow composition. An
incompatible complete slot forecast remains inconclusive and is not bypassed.
"""
import json
from pathlib import Path
import subprocess
import sys

ROOT = Path(__file__).resolve().parents[1]
ROWS = [
    (7, 'Assembly-CSharp', 'b9-empty-arm-common-return',
     'bne.un + br / beq', 'form.SubForm::Init()->System.Boolean',
     'Braco verdadeiro vazio e else preservam o join comum; return direto e controle negativo'),
    (8, 'Assembly-CSharp', 'b9-guard-return-empty-positive-arm',
     'br / ret', 'form.SoccerForm::Draw_newPlayerPopular(',
     'Substituir uma guarda de retorno por braco vazio e else para cair no join do if'),
    (9, 'KairoLibrary', 'b9-positive-render-with-gui-else',
     'brfalse / brtrue.s + ret', 'kairo.unity.ui.Balloon::Draw(',
     'Corpo sob teste positivo de render com ramo GUI em else preserva saida final'),
    (10, 'KairoLibrary', 'b9-boolean-value-before-if',
     'ldc.i4.0 + ceq / brtrue.s', 'kairo.unity.ui.Graphics::_drawBitmap(',
     'Materializar condicao como valor bool antes do if preserva negacao ceq'),
]


def main():
    for number, assembly, rule, pair, method, hypothesis in ROWS:
        journal = json.loads((ROOT / 'reports/round9-cycle-journal.json').read_text(encoding='utf-8'))
        if len(journal['cycles']) >= number:
            row = journal['cycles'][number - 1]
            if row['rule'] != rule or row.get('phase') != 'B':
                raise ValueError('Existing step differs from queue')
            continue
        if len(journal['cycles']) != number - 1:
            raise ValueError('Previous authorized step has not completed')
        folder = ROOT / 'build/round9/part-b-queue'
        folder.mkdir(parents=True, exist_ok=True)
        command = [sys.executable, '-X', 'utf8', 'tools/round9_cycle.py',
                   '--number', str(number), '--phase', 'B', '--assembly', assembly,
                   '--rule', rule, '--pair', pair, '--hypothesis', hypothesis,
                   '--probe-substring', method, '--target-substring', method,
                   '--isolate-family', '--single-step', '--push']
        print(json.dumps({'starting_part_B_step': number, 'rule': rule}), flush=True)
        with (folder / ('b%02d.log' % number)).open('wb') as log:
            result = subprocess.run(command, cwd=ROOT, stdout=log, stderr=subprocess.STDOUT)
        if result.returncode:
            raise RuntimeError('Part B step failed; see ' + str(folder / ('b%02d.log' % number)))
        row = json.loads((ROOT / 'reports/round9-cycle-journal.json').read_text(encoding='utf-8'))['cycles'][-1]
        print(json.dumps({'finished_part_B_step': number, 'gains': row['gains'], 'result': row['result']}), flush=True)


if __name__ == '__main__':
    main()
