"""Bounded parallel *screening* builds; no ledger states or real source changes.

Every candidate gets independent immutable inputs, response file, compiler log,
and output folder. Results are merely candidates: a complete real-source build
and the normal body/metadata verifier remain required before adoption.
"""
from concurrent.futures import CancelledError, ThreadPoolExecutor
import hashlib
import json
import os
from pathlib import Path
import shutil
import subprocess
import threading
import time
import traceback

ROOT = Path(__file__).resolve().parents[1]


def _sha(path):
    return hashlib.sha256(Path(path).read_bytes()).hexdigest()


def _patched(text, patch):
    raw = text.encode('utf-16-le')
    start, end = patch['start'] * 2, patch['end'] * 2
    if not (0 <= start <= end <= len(raw)) or raw[start:end].decode('utf-16-le') != patch['before']:
        raise ValueError('Stale or invalid UTF-16 candidate patch')
    return (raw[:start] + patch['after'].encode('utf-16-le') + raw[end:]).decode('utf-16-le')


class TrialPool:
    def __init__(self, manifest, assembly, source_paths, target_logical, folder, max_workers=3):
        if assembly not in ('Assembly-CSharp', 'KairoLibrary'):
            raise ValueError('Unsupported target assembly')
        if not 1 <= max_workers <= 3:
            raise ValueError('Screen pool must have one to three workers')
        self.manifest = json.loads(json.dumps(manifest))
        self.assembly = assembly
        self.target = target_logical
        self.folder = Path(folder).resolve()
        if not self.folder.is_relative_to((ROOT / 'build').resolve()):
            raise ValueError('Trial outputs must stay inside build/')
        self.folder.mkdir(parents=True, exist_ok=True)
        self.order = [s['path'] if isinstance(s, dict) else s for s in manifest['sources']]
        if len(self.order) != len(set(self.order)) or target_logical not in self.order:
            raise ValueError('Invalid source order or target source')
        if set(self.order) - set(source_paths):
            raise ValueError('Missing selected source paths')
        self.paths = {p: Path(source_paths[p]).resolve() for p in self.order}
        self.input_hashes = {p: _sha(path) for p, path in self.paths.items()}
        # Freeze non-target inputs once, so later owner edits cannot race a worker.
        frozen = self.folder / 'inputs'
        frozen.mkdir(exist_ok=True)
        self.frozen = {}
        for number, logical in enumerate(self.order):
            if logical == self.target:
                continue
            dest = frozen / ('%03d-' % number + self.paths[logical].name)
            shutil.copyfile(self.paths[logical], dest)
            self.frozen[logical] = dest
        self.environment = os.environ.copy()
        self.environment.update(manifest.get('scoped_environment', {}))
        self.futures = []
        self.observed = set()
        self.closed = False
        self.summary = None
        self.lock = threading.Lock()
        self.started = time.monotonic()
        self.executor = ThreadPoolExecutor(max_workers=max_workers, thread_name_prefix='round4-screen')

    def submit(self, patches, text, source_filter=None):
        """Submit one batch; optional filtering runs serially before worker launch.

        A source_filter receives the complete patched target text and returns the
        screening-only source. The caller must prove the baseline target body
        survives this filtering and fully build every advancing candidate.
        """
        if self.closed:
            raise RuntimeError('Trial pool is closed')
        indices = []
        for patch in patches:
            candidate = _patched(text, patch)
            if source_filter is not None:
                candidate = source_filter(candidate)
                if not isinstance(candidate, str):
                    raise TypeError('source_filter must return source text')
            index = len(self.futures)
            folder = self.folder / ('candidate-%04d' % index)
            folder.mkdir(exist_ok=False)
            source = folder / self.paths[self.target].name
            source.write_text(candidate, encoding='utf-8', newline='')
            future = self.executor.submit(self._build, index, folder, source, patch)
            self.futures.append(future)
            indices.append(index)
        return indices

    def _build(self, index, folder, source, patch):
        started = time.monotonic()
        binary = folder / (self.assembly + '.dll')
        is_app = self.assembly == 'Assembly-CSharp'
        flags = [f for f in self.manifest['flags']
                 if not f.startswith(('/out:', '-out:')) and f not in ('/noconfig', '-noconfig')]
        flags += [('-out:' if is_app else '/out:') + str(binary)]
        selected = {p: source if p == self.target else self.frozen[p] for p in self.order}
        args = flags + [('-r:' if is_app else '/reference:') + r['path'] for r in self.manifest['references']]
        args += [str(selected[p]) for p in self.order]
        rsp = folder / 'compile.rsp'
        rsp.write_text('\n'.join('"' + arg + '"' for arg in args) + '\n', encoding='utf-8')
        command = ([self.manifest['runtime']] if is_app else []) + [self.manifest['compiler'],
                   '-noconfig' if is_app else '/noconfig', '@' + str(rsp)]
        log_path = folder / 'compiler.log'
        error = None
        try:
            result = subprocess.run(command, cwd=ROOT, env=self.environment, capture_output=True)
            log = (result.stdout + result.stderr).decode('utf-8', errors='replace')
            code = result.returncode
        except Exception:
            code = None
            error = traceback.format_exc()
            log = error
        seconds = time.monotonic() - started
        log_path.write_text(log, encoding='utf-8')
        passed = code == 0 and binary.is_file()
        record = {'index': index, 'binary': str(binary) if passed else None,
                  'compile_pass': passed, 'exit_code': code, 'seconds': seconds,
                  'log_path': str(log_path), 'error': error,
                  'screening_only': True, 'target_logical': self.target,
                  'patch': patch, 'compiler': self.manifest['compiler'],
                  'compiler_sha256': self.manifest.get('compiler_sha256'),
                  'flags': flags, 'references': self.manifest['references'],
                  'sources': [{'path': p, 'input_path': str(selected[p]),
                               'sha256': _sha(source) if p == self.target else self.input_hashes[p]}
                              for p in self.order]}
        (folder / 'build-manifest.json').write_text(json.dumps(record, indent=2) + '\n', encoding='utf-8')
        return dict(record, binary=binary if passed else None, log=log)

    def result(self, index):
        result = self.futures[index].result()
        with self.lock:
            self.observed.add(index)
        return result

    def close(self):
        """Cancel queued work, await workers, and account for every submitted job."""
        if self.summary is not None:
            return self.summary
        self.closed = True
        for future in self.futures:
            future.cancel()
        self.executor.shutdown(wait=True, cancel_futures=True)
        statuses = []
        for index, future in enumerate(self.futures):
            status = {'index': index, 'observed_by_caller': index in self.observed,
                      'cancelled': future.cancelled()}
            if not future.cancelled():
                try:
                    result = future.result()
                    status.update(compile_pass=result['compile_pass'], seconds=result['seconds'],
                                  binary=str(result['binary']) if result['binary'] else None,
                                  log_path=result['log_path'], error=result['error'], exit_code=result['exit_code'])
                    if not result['compile_pass']:
                        status['compiler_log'] = result['log']
                except Exception:
                    status.update(compile_pass=False, seconds=0, error=traceback.format_exc())
            statuses.append(status)
        finished = [s for s in statuses if not s['cancelled']]
        self.summary = {'submitted': len(statuses), 'finished': len(finished),
                        'cancelled': sum(s['cancelled'] for s in statuses),
                        'compile_failures': sum(not s['compile_pass'] for s in finished),
                        'seconds': sum(s['seconds'] for s in finished),
                        'wall_seconds': time.monotonic() - self.started,
                        'unused_speculative_finished': sum(not s['observed_by_caller'] for s in finished),
                        'screening_only': True, 'statuses': statuses}
        (self.folder / 'pool-summary.json').write_text(json.dumps(self.summary, indent=2) + '\n', encoding='utf-8')
        return self.summary

    def __enter__(self):
        return self

    def __exit__(self, *_):
        self.close()
