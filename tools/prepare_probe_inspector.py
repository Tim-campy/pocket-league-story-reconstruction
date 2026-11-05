"""Build the read-only probe inspector with existing Windows/Unity components."""
import hashlib
import json
import os
from pathlib import Path
import shutil
import subprocess

ROOT=Path(__file__).resolve().parents[1]


def main():
    unity=Path(os.environ.get('UNITY_ROOT',str(Path.home()/'AppData/Local/Programs/Unity/5.3.4f1')))
    csc=Path(os.environ.get('WINDIR','C:/Windows'))/'Microsoft.NET/Framework64/v4.0.30319/csc.exe'
    cecil=unity/'Editor/Data/Managed/Mono.Cecil.dll'
    if not csc.is_file() or not cecil.is_file():
        raise SystemExit('Existing .NET Framework compiler and Unity Mono.Cecil are required; no tools are downloaded')
    output=ROOT/'build/tools';output.mkdir(parents=True,exist_ok=True)
    stamp=output/'inspector-manifest.json'
    inputs={str(path):hashlib.sha256(path.read_bytes()).hexdigest() for path in (ROOT/'tools/ProbeInspector.cs',csc,cecil)}
    if stamp.is_file() and (output/'ProbeInspector.exe').is_file() and (output/'Mono.Cecil.dll').is_file():
        previous=json.loads(stamp.read_text())
        if previous['inputs']==inputs and previous['output_sha256']==hashlib.sha256((output/'ProbeInspector.exe').read_bytes()).hexdigest() and inputs[str(cecil)]==hashlib.sha256((output/'Mono.Cecil.dll').read_bytes()).hexdigest():
            print(json.dumps({'inspector':'build/tools/ProbeInspector.exe','cache':'current'}));return
    command=[str(csc),'/nologo','/target:exe','/out:'+str(output/'ProbeInspector.exe'),
             '/r:'+str(cecil),'/r:System.Web.Extensions.dll',str(ROOT/'tools/ProbeInspector.cs')]
    subprocess.run(command,check=True)
    shutil.copy2(cecil,output/'Mono.Cecil.dll')
    stamp.write_text(json.dumps({'inputs':inputs,'output_sha256':hashlib.sha256((output/'ProbeInspector.exe').read_bytes()).hexdigest()},indent=2)+'\n')
    print(json.dumps({'inspector':'build/tools/ProbeInspector.exe','cecil_sha256':hashlib.sha256(cecil.read_bytes()).hexdigest()}))


if __name__=='__main__': main()
