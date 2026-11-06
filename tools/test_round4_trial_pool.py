"""Independent compiler fixtures prove parallel screen isolation and accounting."""
import copy
import json
from pathlib import Path
import tempfile
import threading
import unittest

from compare_kairo import differences
from round4_symbols import inspect_methods
from round4_trial_pool import TrialPool

ROOT = Path(__file__).resolve().parents[1]


class TrialPoolTests(unittest.TestCase):
    def setUp(self):
        baseline = ROOT / 'build/round4/baseline/Assembly-CSharp/build-manifest.json'
        if not baseline.is_file():
            self.skipTest('Local Mono toolchain baseline required')
        self.manifest = json.loads(baseline.read_text(encoding='utf-8'))
        if not Path(self.manifest['compiler']).is_file():
            self.skipTest('Pinned Unity smcs installation required')
        parent = ROOT / 'build/round4/trial-pool-tests'
        parent.mkdir(exist_ok=True)
        self.folder = Path(tempfile.mkdtemp(prefix='case-', dir=parent))
        self.logical = 'fixture/TrialFixture.cs'
        self.source = self.folder / 'TrialFixture.cs'
        self.text = 'public static class TrialFixture { public static int Sign(int value) { return value > 0 ? 1 : -1; } }'
        self.source.write_text(self.text, encoding='utf-8')
        self.manifest['sources'] = [{'path': self.logical}]
        self.paths = {self.logical: self.source}
        before = 'return value > 0 ? 1 : -1;'
        offset = self.text.index(before)
        self.patches = [{'start': offset, 'end': offset + len(before), 'before': before,
                         'after': after, 'rule': 'independent-fixture'}
                        for after in ('return value <= 0 ? -1 : 1;',
                                      'if(value > 0) return 1; return -1;',
                                      'return missing_symbol;')]

    def test_parallel_matches_serial_and_retains_compile_failures(self):
        original = self.source.read_bytes()
        parallel = TrialPool(self.manifest, 'Assembly-CSharp', self.paths, self.logical,
                             self.folder / 'parallel', max_workers=3)
        serial = TrialPool(self.manifest, 'Assembly-CSharp', self.paths, self.logical,
                           self.folder / 'serial', max_workers=1)
        try:
            self.assertEqual(parallel.submit(self.patches, self.text), [0, 1, 2])
            self.assertEqual(serial.submit(self.patches, self.text), [0, 1, 2])
            results_parallel = [parallel.result(i) for i in range(3)]
            results_serial = [serial.result(i) for i in range(3)]
        finally:
            parallel_summary = parallel.close()
            serial_summary = serial.close()
        self.assertEqual(self.source.read_bytes(), original)
        for a, b in zip(results_parallel, results_serial):
            self.assertEqual(a['compile_pass'], b['compile_pass'])
            if a['compile_pass']:
                pa = {m['stable_id']: m for m in inspect_methods(a['binary'], symbols=False)['methods']}
                pb = {m['stable_id']: m for m in inspect_methods(b['binary'], symbols=False)['methods']}
                self.assertEqual(set(pa), set(pb))
                self.assertTrue(all(not differences(method, pb[key])[0] for key, method in pa.items()))
            else:
                self.assertIn('CS0103', a['log'])
                self.assertIn('CS0103', b['log'])
        for summary in (parallel_summary, serial_summary):
            self.assertEqual(summary['submitted'], 3)
            self.assertEqual(summary['finished'], 3)
            self.assertEqual(summary['cancelled'], 0)
            self.assertEqual(summary['compile_failures'], 1)
            self.assertEqual(summary['unused_speculative_finished'], 0)
            self.assertEqual(len(summary['statuses']), 3)

    def test_close_cancels_queued_and_accounts_for_unused_result(self):
        entered = threading.Event()
        release = threading.Event()

        class WaitingPool(TrialPool):
            def _build(self, index, folder, source, patch):
                entered.set()
                if not release.wait(10):
                    raise RuntimeError('Test release event timed out')
                return {'compile_pass': True, 'binary': None, 'log': '', 'log_path': str(folder / 'compiler.log'),
                        'error': None, 'exit_code': 0, 'seconds': 0.0}

        pool = WaitingPool(self.manifest, 'Assembly-CSharp', self.paths, self.logical,
                           self.folder / 'cancel', max_workers=1)
        pool.submit(self.patches, self.text)
        self.assertTrue(entered.wait(5))
        # Cancellation happens in the calling thread; only the active job awaits.
        for future in pool.futures:
            future.cancel()
        release.set()
        summary = pool.close()
        self.assertEqual(summary['submitted'], 3)
        self.assertEqual(summary['finished'], 1)
        self.assertEqual(summary['cancelled'], 2)
        self.assertEqual(summary['unused_speculative_finished'], 1)
        self.assertIs(pool.close(), summary)

    def test_stale_patch_and_unsafe_output_are_rejected(self):
        pool = TrialPool(self.manifest, 'Assembly-CSharp', self.paths, self.logical,
                         self.folder / 'invalid', max_workers=1)
        try:
            bad = copy.deepcopy(self.patches[0])
            bad['before'] = 'different command'
            with self.assertRaises(ValueError):
                pool.submit([bad], self.text)
        finally:
            self.assertEqual(pool.close()['submitted'], 0)
        with self.assertRaises(ValueError):
            TrialPool(self.manifest, 'Assembly-CSharp', self.paths, self.logical, ROOT / 'original')

    def test_source_filter_receives_patched_text_in_caller_thread(self):
        seen = []
        caller = threading.get_ident()

        def source_filter(candidate):
            seen.append((threading.get_ident(), candidate))
            return candidate + '\n// filtered screening source\n'

        pool = TrialPool(self.manifest, 'Assembly-CSharp', self.paths, self.logical,
                         self.folder / 'filter', max_workers=1)
        try:
            indices = pool.submit(self.patches[:1], self.text, source_filter=source_filter)
            result = pool.result(indices[0])
            self.assertTrue(result['compile_pass'])
        finally:
            summary = pool.close()
        self.assertEqual(len(seen), 1)
        self.assertEqual(seen[0][0], caller)
        self.assertIn(self.patches[0]['after'], seen[0][1])
        self.assertNotIn(self.patches[0]['before'], seen[0][1])
        generated = next(source for source in result['sources'] if source['path'] == self.logical)
        self.assertTrue(Path(generated['input_path']).read_text(encoding='utf-8').endswith('// filtered screening source\n'))
        self.assertEqual(summary['compile_failures'], 0)


if __name__ == '__main__':
    unittest.main()
