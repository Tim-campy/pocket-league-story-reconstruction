"""Physical target ownership evidence, independent of any source AST.

Offset aliasing does not select a lexical keyword. Frames must already have
been recovered conservatively by the IL region parser; this module does not
invent frames from a branch's proximity to an exit.
"""


def resolve_transfer(frames, target, opcode):
    candidates = []
    breakable_depth = loop_depth = 0
    for frame in reversed(frames):
        owner_kind = frame.get('kind', 'loop')
        if owner_kind == 'EH':
            if opcode in ('leave', 'leave.s') and target in frame.get('return_targets', []):
                candidates.append({'kind': 'return', 'target_owner_kind': 'method',
                                   'target_breakable_depth': 0, 'lexical_loop_depth': 0,
                                   'target_index': target})
            continue
        if owner_kind not in ('loop', 'switch'):
            return {'complete': False, 'reason': 'transfer-frame-kind-unknown', 'owner_candidates': candidates}
        if target == frame['exit']:
            candidates.append({'kind': 'break', 'target_owner_kind': owner_kind,
                               'target_breakable_depth': breakable_depth, 'lexical_loop_depth': loop_depth,
                               'target_index': target})
        if owner_kind == 'loop' and target == frame['test']:
            candidates.append({'kind': 'continue', 'target_owner_kind': 'loop',
                               'target_breakable_depth': breakable_depth, 'lexical_loop_depth': loop_depth,
                               'target_index': target})
        breakable_depth += 1
        if owner_kind == 'loop':
            loop_depth += 1
    if not candidates:
        return {'complete': False, 'reason': 'unowned-transfer-unmodeled', 'owner_candidates': []}
    if len(candidates) != 1:
        return {'complete': False, 'reason': 'transfer-owner-alias-ambiguous', 'owner_candidates': candidates}
    candidate = candidates[0]
    if candidate['kind'] == 'break' and candidate['target_breakable_depth'] > 0:
        return {'complete': False, 'reason': 'outer-break-keyword-unmodeled', 'owner_candidates': candidates}
    if candidate['kind'] == 'continue' and candidate['lexical_loop_depth'] > 0:
        return {'complete': False, 'reason': 'outer-continue-keyword-unmodeled', 'owner_candidates': candidates}
    return {'complete': True, 'owner_candidates': candidates, 'chosen': candidate}
