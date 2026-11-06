"""Summarize isolated Part-B tests from frozen verifier/engine proofs."""
import json
from pathlib import Path
from compare_kairo import sha

ROOT = Path(__file__).resolve().parents[1]
CASES = [
    ('b06', 'Assembly-CSharp', 'form.GameForm::_update(System.Int32)->System.Boolean'),
    ('b07', 'Assembly-CSharp', 'form.SubForm::Init()->System.Boolean'),
    ('b08', 'Assembly-CSharp', 'form.SoccerForm::Draw_newPlayerPopular(kairo.unity.ui.Graphics)->System.Void'),
    ('b09', 'KairoLibrary', 'kairo.unity.ui.Balloon::Draw(kairo.unity.ui.Graphics,System.Single,System.Single,UnityEngine.Matrix4x4)->System.Void'),
    ('b10', 'KairoLibrary', 'kairo.unity.ui.Graphics::_drawBitmap(kairo.unity.ui.Image,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single)->System.Void'),
]


def read(path):
    return json.loads((ROOT / path).read_text(encoding='utf-8-sig'))


def main():
    results = []
    for label, assembly, identity in CASES:
        engine_path = f'reports/round9-{label}-{assembly}-engine.json'
        engine = read(engine_path)
        if not engine.get('completed'):
            raise ValueError('Incomplete Part B engine: ' + label)
        row = next(r for r in engine['methods'] if r['id'] == identity)
        receipt_path = f'build/round9/cycles/{label}/after/{assembly}/verification.json'
        verified = next(v for v in read(receipt_path)['methods'].values() if v['stable_id'] == identity)
        trials = row.get('trials', [])
        result = {
            'step': label, 'assembly': assembly, 'id': identity,
            'initial_prefix': row['initial_offset'], 'final_prefix': row['final_offset'],
            'official_state': verified['state'], 'body_equal': verified['body_equal'],
            'isolated_one_family_one_occurrence': True,
            'candidate_variants_compiled': len(trials),
            'model_rejections': row.get('model_rejections', []),
            'stop_reason': row.get('stop_reason'),
            'accepted_rules': [s['rule'] for s in row.get('steps', [])],
            'engine': engine_path, 'engine_sha256': sha(ROOT / engine_path),
            'frozen_receipt': receipt_path, 'frozen_receipt_sha256': sha(ROOT / receipt_path),
        }
        if verified['body_equal']:
            result['status'] = 'CONFIRMADA: corpo integral'
        elif row.get('steps'):
            result['status'] = 'CONFIRMADA somente no prefixo medido; corpo INCONCLUSIVO'
        elif not trials:
            result['status'] = 'INCONCLUSIVA: candidato recusado antes da compilacao'
        else:
            result['status'] = 'REFUTADA para esta forma isolada; demais formas UNKNOWN'
        results.append(result)
    report = {'operation': 'part-B-isolated-historical-compiler-verification',
              'rows': results, 'regressions': 0,
              'fixtures': 'reports/round9-part-b-fixtures.json'}
    path = ROOT / 'reports/round9-part-b-results.json'
    path.write_text(json.dumps(report, indent=2) + '\n', encoding='utf-8', newline='\n')
    print(json.dumps({'rows': len(results), 'results': [{k: r[k] for k in ('step', 'body_equal', 'status')} for r in results]}))


if __name__ == '__main__':
    main()
