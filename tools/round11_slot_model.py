"""Source-only smcs local allocator model; never compiles a game candidate."""
import hashlib
import json
from pathlib import Path
import shutil
import subprocess
from collections import OrderedDict
import sys
ROOT=next(p for p in Path(__file__).resolve().parents if(p/'AGENTS.md').is_file())
sys.path.insert(0,str(ROOT/'tools'))
from compiler_probes import ROSLYN
MODEL_SOURCE=Path(__file__).with_name('Round11SlotModel.cs')


def ensure_helper():
    vs = next(p for p in ROSLYN.parents if (p / 'dotnet').is_dir())
    runtime = vs / 'dotnet/net10.0/runtime'
    framework = runtime / 'shared/Microsoft.NETCore.App/10.0.10'
    source = MODEL_SOURCE
    identity = hashlib.sha256(source.read_bytes() + str(ROSLYN).encode()).hexdigest()
    folder = ROOT / 'build/round11/slot-model' / identity[:16]
    folder.mkdir(parents=True, exist_ok=True)
    target = folder / 'Round11SlotModel.dll'
    libraries = ['System.Private.CoreLib', 'System.Runtime', 'System.Console', 'System.Linq',
                 'System.Collections', 'System.Text.Json', 'System.Reflection.Metadata',
                 'System.Collections.Immutable', 'System.Memory', 'netstandard']
    refs = [framework / (n + '.dll') for n in libraries]
    refs += [ROSLYN.parent / (n + '.dll') for n in ['Microsoft.CodeAnalysis', 'Microsoft.CodeAnalysis.CSharp']]
    if not target.exists():
        args = ['/nologo', '/nostdlib+', '/target:exe', '/out:' + str(target)]
        args += ['/r:' + str(p) for p in refs] + [str(source)]
        rsp = folder / 'compile.rsp'
        rsp.write_text('\n'.join('"' + a + '"' for a in args) + '\n', encoding='utf-8')
        subprocess.run([str(ROSLYN), '/noconfig', '@' + str(rsp)], check=True)
        for reference in refs[-2:]:
            shutil.copy2(reference, folder / reference.name)
        (folder / 'Round11SlotModel.runtimeconfig.json').write_text(json.dumps({
            'runtimeOptions': {'tfm': 'net10.0', 'framework': {
                'name': 'Microsoft.NETCore.App', 'version': '10.0.10'}}}) + '\n')
    return [str(runtime / 'dotnet.exe'), str(target)]


class SlotModelClient:
    """Persistent source model, including directly hosted anonymous bodies.

    Compiler-generated IDs are derived from source declarations and compiler
    ordering rules; unmapped bodies always remain incomplete.
    """
    def __init__(self, manifest):
        self.manifest = Path(manifest).resolve()
        self.cache = OrderedDict()
        self.process = subprocess.Popen(ensure_helper() + [str(self.manifest)], stdin=subprocess.PIPE,
            stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True, encoding='utf-8')

    def predict_all(self, path, source=None):
        path = str(Path(path).resolve())
        request = {'path': path}
        if source is not None:
            request['source'] = source
        key = (path, hashlib.sha256((source if source is not None else Path(path).read_text(encoding='utf-8-sig')).encode()).hexdigest())
        if key not in self.cache:
            self.process.stdin.write(json.dumps(request, ensure_ascii=True) + '\n')
            self.process.stdin.flush()
            response = self.process.stdout.readline()
            if not response:
                raise RuntimeError('Slot helper exited: ' + self.process.stderr.read())
            result = json.loads(response)
            if 'error' in result:
                raise ValueError(result)
            self.cache[key] = result['methods']
        return self.cache[key]

    def predict(self, path, source=None, method_id=None):
        path=str(Path(path).resolve())
        text=source if source is not None else Path(path).read_text(encoding='utf-8-sig')
        key=(path,hashlib.sha256(text.encode()).hexdigest(),method_id)
        if key in self.cache:
            self.cache.move_to_end(key)
            return self.cache[key]
        request={'path':path,'source':text,'method_id':method_id}
        self.process.stdin.write(json.dumps(request,ensure_ascii=True)+'\n')
        self.process.stdin.flush()
        response=self.process.stdout.readline()
        if not response:raise RuntimeError('Slot helper exited: '+self.process.stderr.read())
        result=json.loads(response)
        if 'error' in result:raise ValueError(result)
        methods=result['methods']
        found = [m for m in methods if m['method_id'] == method_id]
        if len(found) != 1:
            return {'method_id': method_id, 'complete': False, 'predicted_types': [],
                    'unsupported_reasons': ['generated_or_unmapped_source_method'], 'variables': [], 'blocks': []}
        self.cache[key]=found[0]
        if len(self.cache)>256:self.cache.popitem(last=False)
        return found[0]

    def solve(self, path, source, method_id, original_method, expected_types, rebuilt_method=None):
        """Calculate positions from original slot uses, never compile candidates."""
        from round7_slot_solver import proposals
        prediction=self.predict(path,source,method_id)
        if rebuilt_method is None:
            return {'patches':[], 'constraints':['current_rebuilt_IL_required'], 'source_only':True}
        result=proposals(source,prediction,original_method,rebuilt_method,expected_types,
                         prediction.get('predicted_types',[]))
        approved=[]
        for patch in result['patches']:
            raw=source.encode('utf-16-le')
            candidate=(raw[:patch['start']*2]+patch['after'].encode('utf-16-le')+raw[patch['end']*2:]).decode('utf-16-le')
            candidate_prediction=self.predict(path,candidate,method_id)
            if candidate_prediction.get('complete') and candidate_prediction.get('predicted_types')==expected_types:
                approved.append(patch)
        result['calculated_candidates']=len(result['patches'])
        result['model_approved_candidates']=len(approved)
        result['patches']=approved
        return result

    def close(self):
        if self.process.poll() is None:
            self.process.stdin.close()
            self.process.wait(timeout=10)

    def __enter__(self): return self
    def __exit__(self, *_): self.close()


if __name__ == '__main__':
    print(json.dumps({'command': ensure_helper()}))
