"""Build the read-only sequence-point inspector from installed Unity libraries."""
import hashlib
import json
import os
from pathlib import Path
import shutil
import subprocess

ROOT = Path(__file__).resolve().parents[1]


def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def prepare():
    unity = Path(os.environ.get('UNITY_ROOT', str(Path.home() / 'AppData/Local/Programs/Unity/5.3.4f1')))
    managed = unity / 'Editor/Data/Managed'
    csc = Path(os.environ.get('WINDIR', 'C:/Windows')) / 'Microsoft.NET/Framework64/v4.0.30319/csc.exe'
    sources = [ROOT / 'tools/Round4SymbolInspector.cs', ROOT / 'tools/ProbeInspector.cs']
    libraries = [managed / name for name in ('Mono.Cecil.dll', 'Mono.Cecil.Pdb.dll', 'Mono.Cecil.Mdb.dll')]
    inputs = {str(path): sha(path) for path in [csc] + sources + libraries}
    folder = ROOT / 'build/round4/symbol-tools'
    folder.mkdir(parents=True, exist_ok=True)
    target = folder / 'Round4SymbolInspector.exe'
    stamp = folder / 'manifest.json'
    if target.is_file() and stamp.is_file():
        previous = json.loads(stamp.read_text())
        if previous['inputs'] == inputs and previous['output_sha256'] == sha(target) and all(sha(folder / p.name) == sha(p) for p in libraries):
            return target
    command = [str(csc), '/nologo', '/target:exe', '/main:Round4SymbolInspector', '/out:' + str(target), '/r:System.Web.Extensions.dll']
    command += ['/r:' + str(path) for path in libraries] + [str(path) for path in sources]
    subprocess.run(command, cwd=ROOT, check=True)
    for library in libraries:
        shutil.copy2(library, folder / library.name)
    stamp.write_text(json.dumps({'inputs': inputs, 'output_sha256': sha(target)}, indent=2) + '\n', encoding='utf-8')
    return target


if __name__ == '__main__':
    print(json.dumps({'inspector': str(prepare().relative_to(ROOT))}))
