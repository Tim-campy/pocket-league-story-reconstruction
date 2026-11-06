"""Read compiler sequence points; never use symbol alignment for verification.

inspect_methods(binary, selectors=None, symbols=True, points_only=False) returns
the existing ProbeInspector body representation plus sequence_points. select_point
maps an IL offset to its preceding visible sequence point. Debug/no-debug alignment
is supplied separately and is only a source-location diagnostic.
"""
import argparse
import difflib
import json
from pathlib import Path
import subprocess

from prepare_round4_symbols import prepare


def inspect_methods(binary, selectors=None, *, symbols=True, points_only=False):
    command = [str(prepare()), str(Path(binary).resolve())]
    if not symbols:
        command.append('--no-symbols')
    if points_only:
        command.append('--points-only')
    command.extend(selectors or [])
    result = subprocess.run(command, capture_output=True, encoding='utf-8-sig', errors='strict', check=True)
    return json.loads(result.stdout)


def select_point(method, il_offset):
    """Return preceding non-hidden point, with explicit diagnostic provenance."""
    points = [point for point in method.get('sequence_points', []) if not point['hidden'] and point['offset'] <= il_offset]
    if not points:
        return None
    point = max(points, key=lambda entry: entry['offset'])
    return dict(point, mapping='compiler_sequence_point', il_offset=il_offset,
                line=point['start_line'], column=point['start_column'])


def inspect(binary, ids=None, symbols=True, points_only=False):
    """Engine-facing keyed variant of inspect_methods."""
    result = inspect_methods(binary, ids, symbols=symbols, points_only=points_only)
    result['methods'] = {m['stable_id']: m for m in result['methods']}
    return result


def source_point(method, il_offset):
    return select_point(method, il_offset)


def align_source_point(plain_method, debug_method, il_offset):
    """Prefer a proved identical body; otherwise expose diagnostic alignment."""
    from compare_kairo import differences
    if not differences(plain_method, debug_method)[0]:
        point = select_point(debug_method, il_offset)
        return dict(point, mapping='MDB_same_full_body_as_debug_minus') if point else None
    return align_debug_point(plain_method, debug_method, il_offset)


def instruction_key(instruction):
    """Alignment key only; raw byte acceptance must never use this function."""
    operand = instruction.get('operand')
    if isinstance(operand, dict) and ('branch' in operand or 'switch' in operand):
        operand = '<branch-target-for-source-alignment>'
    return instruction['opcode'], json.dumps(operand, sort_keys=True, separators=(',', ':'))


def align_debug_point(plain_method, debug_method, il_offset):
    """Map identical instructions from no-debug IL into compiler MDB sequence points.

    This deliberately refuses to guess a map when the target instruction is not
    in an exact matching block after dropping nops and branch destinations. Body
    verification still compares every original byte using the normal verifier.
    """
    plain = [i for i in plain_method.get('instructions', []) if i['opcode'] != 'nop']
    debug = [i for i in debug_method.get('instructions', []) if i['opcode'] != 'nop']
    candidates = [n for n, i in enumerate(plain) if i['offset'] <= il_offset]
    if not candidates:
        return None
    target = candidates[-1]
    matcher = difflib.SequenceMatcher(a=[instruction_key(i) for i in plain], b=[instruction_key(i) for i in debug], autojunk=False)
    for block in matcher.get_matching_blocks():
        if block.a <= target < block.a + block.size:
            debug_index = block.b + target - block.a
            point = select_point(debug_method, debug[debug_index]['offset'])
            if point:
                return dict(point, mapping='diagnostic_no_debug_to_MDB_instruction_alignment',
                            il_offset=il_offset, debug_il_offset=debug[debug_index]['offset'],
                            alignment_block_instructions=block.size,
                            confidence='EXACT_OPCODE_OPERAND_IN_MATCHING_BLOCK_BRANCH_DESTINATIONS_IGNORED_FOR_MAPPING_ONLY')
    return None


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('binary', type=Path)
    parser.add_argument('selectors', nargs='*')
    parser.add_argument('--no-symbols', action='store_true')
    parser.add_argument('--points-only', action='store_true')
    args = parser.parse_args()
    print(json.dumps(inspect_methods(args.binary, args.selectors, symbols=not args.no_symbols, points_only=args.points_only), ensure_ascii=True))


if __name__ == '__main__':
    main()
