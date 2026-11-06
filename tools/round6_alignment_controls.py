"""Measure focused raw-vs-aligned diagnosis controls from the frozen baseline."""
import json
from pathlib import Path

from compare_kairo import sha
from round4_measure import first_difference
from round6_alignment import align_methods

ROOT = Path(__file__).resolve().parents[1]
BASE = ROOT / 'build/round6/baseline'
CONTROLS = {
    'Assembly-CSharp': ['surface.GameView::OnTouchDlgSel(', 'surface.GameView::OnTouchDlgCmd(', 'form.BootForm::Load('],
    'KairoLibrary': ['kairo.unity.util.Language::Init('],
}


def main():
    result = {'operation': 'read-only-raw-vs-aligned-controls', 'baseline_commit':
              '66eea9ddeb8e02a9f357f97f8e7c9dc60f54d6e9', 'candidate_compilations': 0,
              'game_sources_modified': False, 'ledger_modified': False,
              'limitations': [
                  'Sequence alignment is a source-location diagnostic, not a semantic-equivalence or byte-acceptance test.',
                  'Ambiguous repeated instruction sequences may admit more than one valid alignment.',
                  'An original target in an unmatched sequence range is explicitly deferred. A rebuilt target in a newly inserted range, when the original target has a matched correspondence, remains a conservative branch_retarget diagnosis and can select an earlier command.',
                  'EH regions, instruction widths, header and local signatures remain under the independent strict body oracle.'
              ], 'inputs': [], 'controls': []}
    for assembly, prefixes in CONTROLS.items():
        paths = [BASE / assembly / name for name in ('original-methods.json', 'rebuilt-methods.json')]
        original, rebuilt = [{m['stable_id']: m for m in json.loads(p.read_text(encoding='utf-8-sig'))['selected_methods']}
                             for p in paths]
        result['inputs'] += [{'path': p.relative_to(ROOT).as_posix(), 'sha256': sha(p)} for p in paths]
        for prefix in prefixes:
            identities = [key for key in original if key.startswith(prefix)]
            if len(identities) != 1:
                raise ValueError('Ambiguous alignment control: ' + prefix)
            identity = identities[0]
            left, right = original[identity], rebuilt[identity]
            aligned = align_methods(left, right)
            result['controls'].append({'assembly': assembly, 'method_id': identity,
                                       'raw_first_il_difference_offset': first_difference(left, right),
                                       'alignment': aligned})
    output = ROOT / 'reports/round6-alignment-controls.json'
    output.write_text(json.dumps(result, indent=2) + '\n', encoding='utf-8', newline='\n')
    print(json.dumps([{'id': row['method_id'], 'raw_offset': row['raw_first_il_difference_offset'],
                       'aligned_original_offset': row['alignment']['original_offset'],
                       'aligned_rebuilt_offset': row['alignment']['rebuilt_offset'],
                       'pair': row['alignment']['opcode_pair']} for row in result['controls']]))


if __name__ == '__main__':
    main()
