"""Recalculate a frozen Canvas proposal on token-identical current source."""
import hashlib
import json
from pathlib import Path
from compare_kairo import sha
from round10_training_source import extract
from round10_smcs_empty_arms_source import vals
from round10_canvas_residual_composition_source import transform_canvas_two_residues

ROOT = Path(__file__).resolve().parents[1]


def text_sha(text):
    return hashlib.sha256(text.encode('utf-8')).hexdigest()


def main():
    upstream = ROOT / 'reports/round10-canvas-residual-composition-patches.json'
    document = json.loads(upstream.read_text(encoding='utf-8'))
    assert len(document['patches']) == 1
    patch = document['patches'][0]
    path = ROOT / patch['logical_path']
    source = path.read_bytes().decode('utf-8-sig')
    start, end, before = extract(source, 'public void OnInputUpdate()')
    assert vals(before) == vals(patch['before']), 'The current method differs beyond trivia'
    assert before != patch['before'], 'No preparation rebase is necessary'
    after = transform_canvas_two_residues(before)
    assert vals(after) == vals(patch['after']), 'Recalculation changed the frozen proposal tokens'
    proof_path = ROOT / 'reports/round10-canvas-residual-trivia-rebase.json'
    assert not proof_path.exists()
    dependencies = {name: sha(ROOT / name) for name in (
        Path(__file__).relative_to(ROOT).as_posix(), 'tools/round10_training_source.py',
        'tools/round10_smcs_empty_arms_source.py', 'tools/round10_canvas_residual_composition_source.py')}
    proof = dict(operation='source-only-proposal-recalculation-after-d5-trivia-cleanup',
                 upstream_sha256=sha(upstream), source_sha256=sha(path),
                 source=patch['logical_path'],
                 frozen_before_sha256=text_sha(patch['before']), current_before_sha256=text_sha(before),
                 frozen_after_sha256=text_sha(patch['after']), current_after_sha256=text_sha(after),
                 before_tokens_equal=True, after_tokens_equal=True,
                 target_source_writes=False, game_compilations=0, method_state_writes=False,
                 dependency_sha256=dependencies,
                 reason='D5 literal-protected whitespace cleanup changed one blank line; exact frozen-text rebase would otherwise refuse')
    proof_path.write_text(json.dumps(proof, indent=2) + '\n', encoding='utf-8')
    patch['historical_source_snapshot'] = patch.get('source_snapshot')
    patch.update(start=len(source[:start].encode('utf-16-le')) // 2,
                 end=len(source[:end].encode('utf-16-le')) // 2,
                 before=before, after=after, before_sha256=text_sha(before), after_sha256=text_sha(after),
                 source_file_sha256=sha(path), source_text_sha256=text_sha(source),
                 source_snapshot='Current source at source-only preparation; require fresh model and exact unique rebase')
    document['dependency_sha256'].update(dependencies)
    document['evidence_sha256'].update({upstream.relative_to(ROOT).as_posix(): sha(upstream),
                                      proof_path.relative_to(ROOT).as_posix(): sha(proof_path)})
    document['source_only_trivia_recalculation'] = proof_path.relative_to(ROOT).as_posix()
    output = ROOT / 'reports/round10-canvas-residual-rebased-patches.json'
    assert not output.exists()
    output.write_text(json.dumps(document, indent=2) + '\n', encoding='utf-8')
    assert sha(path) == proof['source_sha256']
    print(json.dumps({'fresh_proposals': 1, 'before_tokens_equal': True, 'after_tokens_equal': True,
                      'target_compilations': 0, 'target_source_writes': False}))


if __name__ == '__main__':
    main()
