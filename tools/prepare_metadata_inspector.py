"""Build the independent read-only metadata inspector with installed components."""
import hashlib
import json
import os
from pathlib import Path
import shutil
import subprocess

ROOT = Path(__file__).resolve().parents[1]


def ensure_inspector():
    unity = Path(os.environ.get('UNITY_ROOT', str(Path.home() / 'AppData/Local/Programs/Unity/5.3.4f1')))
    csc = Path(os.environ.get('WINDIR', 'C:/Windows')) / 'Microsoft.NET/Framework64/v4.0.30319/csc.exe'
    cecil = unity / 'Editor/Data/Managed/Mono.Cecil.dll'
    source = ROOT / 'tools/MetadataInspector.cs'
    sha = lambda path: hashlib.sha256(path.read_bytes()).hexdigest()
    if not all(p.is_file() for p in (csc, cecil, source)):
        raise ValueError('Existing .NET Framework compiler and Unity Mono.Cecil are required')
    folder = ROOT / 'build/metadata-tools'
    folder.mkdir(parents=True, exist_ok=True)
    exe = folder / 'MetadataInspector.exe'
    stamp = folder / 'manifest.json'
    inputs = {str(p): sha(p) for p in (csc, cecil, source)}
    if stamp.is_file() and exe.is_file() and (folder / 'Mono.Cecil.dll').is_file():
        old = json.loads(stamp.read_text(encoding='utf-8'))
        if old['inputs'] == inputs and old['output_sha256'] == sha(exe) and sha(folder / 'Mono.Cecil.dll') == sha(cecil):
            return exe
    subprocess.run([str(csc), '/nologo', '/target:exe', '/out:' + str(exe), '/r:' + str(cecil),
                    '/r:System.Web.Extensions.dll', str(source)], check=True)
    shutil.copy2(cecil, folder / 'Mono.Cecil.dll')
    stamp.write_text(json.dumps({'inputs': inputs, 'output_sha256': sha(exe)}, indent=2) + '\n', encoding='utf-8')
    return exe


if __name__ == '__main__':
    print(ensure_inspector())
