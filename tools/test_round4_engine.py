"""Safety tests for the first-divergence engine, plus real PDB/MDB smoke tests."""
import copy
import json
from pathlib import Path
import unittest

from compare_kairo import differences
from round4_engine import apply_patch, il_cells, instruction_at, prefix

ROOT = Path(__file__).resolve().parents[1]


def body(code, *, metadata=None, locals_=None, trailer=b''):
    """Synthetic independent IL fixture, never a reconstruction output."""
    header = bytes([(len(code) << 2) | 2])
    return {'header': {'size': 1, 'code_size': len(code)},
            'raw_body_hex': (header + code + trailer).hex('-'),
            'metadata_operands': metadata or [], 'locals': locals_ or [],
            'instructions': [], 'exception_regions': [], 'extra_sections': []}


class PrefixSafetyTests(unittest.TestCase):
    def test_metadata_tokens_resolved_but_constants_retained(self):
        # ldstr token; ldc.i4.s 7; ret. Token rows may move; literal 7 may not.
        operand = [{'body_offset': 2, 'width': 4, 'identity': {'string': 'fixture'}}]
        left = body(bytes.fromhex('72 01 00 00 70 1f 07 2a'), metadata=operand)
        right = body(bytes.fromhex('72 99 00 00 70 1f 07 2a'), metadata=operand)
        self.assertEqual(prefix(left, right), 8)
        changed_constant = body(bytes.fromhex('72 99 00 00 70 1f 08 2a'), metadata=operand)
        self.assertEqual(prefix(left, changed_constant), 6)

    def test_branch_destinations_are_not_normalized(self):
        left = body(bytes.fromhex('02 2c 02 17 2a 16 2a'))
        right = body(bytes.fromhex('02 2c 03 17 2a 16 2a'))
        self.assertEqual(prefix(left, right), 2)

    def test_short_and_long_branch_widths_are_not_normalized(self):
        short = body(bytes.fromhex('02 2c 02 17 2a 16 2a'))
        long = body(bytes.fromhex('02 39 02 00 00 00 17 2a 16 2a'))
        self.assertEqual(prefix(short, long), 1)

    def test_equal_il_does_not_promote_different_locals_or_exception_bytes(self):
        left = body(bytes.fromhex('16 2a'), locals_=['System.Int32'])
        different_local = copy.deepcopy(left)
        different_local['locals'] = ['System.Boolean']
        self.assertEqual(prefix(left, different_local), 2)
        self.assertIn('locals', differences(left, different_local)[0])
        different_eh = body(bytes.fromhex('16 2a'), locals_=['System.Int32'], trailer=b'\x01')
        self.assertEqual(prefix(left, different_eh), 2)
        self.assertIn('resolved_body_bytes', differences(left, different_eh)[0])

    def test_prefix_ignores_no_instruction_including_nop(self):
        original = body(bytes.fromhex('02 2a'))
        with_nop = body(bytes.fromhex('00 02 2a'))
        self.assertEqual(prefix(original, with_nop), 0)

    def test_unresolved_metadata_fails_instead_of_accepting(self):
        broken = body(bytes.fromhex('72 01 00 00 70 2a'), metadata=[
            {'body_offset': 2, 'width': 4, 'identity': 'UNKNOWN'}])
        with self.assertRaises(ValueError):
            il_cells(broken)

    def test_operand_mismatch_maps_to_containing_instruction(self):
        method = {'instructions': [{'offset': 0, 'opcode': 'ldarg.0'},
                                   {'offset': 1, 'opcode': 'brfalse', 'operand': {'branch': 10}},
                                   {'offset': 6, 'opcode': 'ret'}]}
        self.assertEqual(instruction_at(method, 4)['offset'], 1)


class SourcePatchSafetyTests(unittest.TestCase):
    def test_utf16_span_preserves_astral_character_and_neighbors(self):
        source = '/* \U0001f680 */ return 0-x;'
        before = '0-x'
        start = len(source[:source.index(before)].encode('utf-16-le')) // 2
        patch = {'start': start, 'end': start + len(before), 'before': before, 'after': '-x'}
        self.assertEqual(apply_patch(source, patch), '/* \U0001f680 */ return -x;')

    def test_stale_patch_is_rejected(self):
        with self.assertRaises(ValueError):
            apply_patch('return y;', {'start': 7, 'end': 8, 'before': 'x', 'after': '-x'})


class ActualCompilerSymbolTests(unittest.TestCase):
    def test_native_roslyn_pdb_maps_a_real_divergent_command(self):
        from round4_symbols import inspect, source_point
        binary = ROOT / 'build/round4/baseline/KairoLibrary/KairoLibrary.dll'
        if not binary.is_file():
            self.skipTest('Local artifact baseline required')
        key = 'kairo.unity.m3d4.util3d.FastMath::Abs(System.Single)->System.Single'
        result = inspect(binary, ids=[key])
        self.assertEqual(result['symbol_kind'], 'PDB')
        method = result['methods'][key]
        point = source_point(method, 10)
        self.assertEqual(point['start_line'], 18)
        self.assertTrue(point['document'].endswith('FastMath.cs'))

    def test_mdb_maps_same_body_without_instruction_alignment(self):
        from round4_symbols import inspect, align_source_point
        baseline = ROOT / 'build/round4/baseline/Assembly-CSharp/rebuilt-methods.json'
        debug = ROOT / 'build/Assembly-CSharp-round4-symbols-debug-plus/Assembly-CSharp.dll'
        coverage = ROOT / 'reports/round4-symbol-mapping.json'
        if not all(path.is_file() for path in (baseline, debug, coverage)):
            self.skipTest('Local smcs symbol probe required')
        with coverage.open(encoding='utf-8') as stream:
            candidates = json.load(stream)['method_mappings']['Assembly-CSharp']
        selected = next(m for m in candidates if m['source_point'])
        key = selected['stable_id']
        with baseline.open(encoding='utf-8-sig') as stream:
            plain = next(m for m in json.load(stream)['selected_methods'] if m['stable_id'] == key)
        result = inspect(debug, ids=[key])
        self.assertEqual(result['symbol_kind'], 'MDB')
        method = result['methods'][key]
        self.assertEqual(differences(plain, method)[0], [])
        point = align_source_point(plain, method, selected['first_il_difference_offset'])
        self.assertEqual(point['mapping'], 'MDB_same_full_body_as_debug_minus')
        self.assertEqual(point['line'], selected['source_point']['line'])


if __name__ == '__main__':
    unittest.main()
