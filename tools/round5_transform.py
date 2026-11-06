"""Small JSON-lines client; emitted patches remain unverified hypotheses."""
import json
from pathlib import Path
import subprocess
from prepare_round5_transform import ensure_helper


class TransformClient:
    def __init__(self, manifest):
        self.process = subprocess.Popen(ensure_helper() + [str(Path(manifest).resolve())],
                                        stdin=subprocess.PIPE, stdout=subprocess.PIPE,
                                        stderr=subprocess.PIPE, text=True, encoding='utf-8')

    def _exchange(self, request):
        self.process.stdin.write(json.dumps(request, ensure_ascii=True) + '\n')
        self.process.stdin.flush()
        result = self.process.stdout.readline()
        if not result:
            raise RuntimeError('Transform helper exited: ' + self.process.stderr.read())
        return json.loads(result)

    def screen_source(self, source, keep_offset):
        """Parse-only screening; callers must still use full-body/full-build gates."""
        result = self._exchange({'mode': 'screen-target', 'source': source,
                                 'keep_offset': keep_offset})
        if 'error' in result:
            raise ValueError('Target screening failed: ' + json.dumps(result))
        return result['source']

    def request(self, path, source=None, line=None, column=1, offset=None,
                method_id=None, max_candidates=256, **hints):
        request = {'path': str(Path(path).resolve()), 'max_candidates': max_candidates}
        if source is not None:
            request['source'] = source
        if offset is not None:
            request['offset'] = offset
        else:
            request.update(line=line, column=column)
        if method_id is not None:
            request['method_id'] = method_id
        request.update(hints)
        return self._exchange(request)

    def close(self):
        if self.process.poll() is None:
            self.process.stdin.close()
            self.process.wait(timeout=10)

    def __enter__(self):
        return self

    def __exit__(self, *_):
        self.close()
