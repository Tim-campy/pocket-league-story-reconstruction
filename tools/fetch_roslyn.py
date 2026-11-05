"""Explicitly download the pinned, portable Microsoft.Net.Compilers 1.3.2 package.

No global installation. Package/compiler binaries stay in ignored analysis/.
"""
import hashlib
import json
from pathlib import Path
from urllib.request import urlopen
import zipfile

ROOT=Path(__file__).resolve().parents[1]
URL='https://api.nuget.org/v3-flatcontainer/microsoft.net.compilers/1.3.2/microsoft.net.compilers.1.3.2.nupkg'
SHA256='37aaedd86f96df0ccfef1ada743d6fefd36135b31b75f5305140b84da98918d7'
CSC_SHA256='d14ba119edd71bdcb82b12659647427aa46d819caf6e72ac6d4ae86c81eb36c9'


def main():
    package=ROOT/'analysis/downloads/microsoft.net.compilers.1.3.2.nupkg'
    if not package.is_file():
        with urlopen(URL,timeout=60) as response: data=response.read()
        if hashlib.sha256(data).hexdigest()!=SHA256: raise ValueError('Package hash mismatch')
        package.parent.mkdir(parents=True,exist_ok=True);package.write_bytes(data)
    if hashlib.sha256(package.read_bytes()).hexdigest()!=SHA256: raise ValueError('Package hash mismatch')
    destination=ROOT/'analysis/tools/roslyn-1.3.2'
    with zipfile.ZipFile(package) as archive:
        for entry in archive.infolist():
            path=(destination/entry.filename).resolve()
            if not path.is_relative_to(destination.resolve()): raise ValueError('Unsafe archive member')
        archive.extractall(destination)
    if hashlib.sha256((destination/'tools/csc.exe').read_bytes()).hexdigest()!=CSC_SHA256:
        raise ValueError('Compiler hash mismatch')
    print(json.dumps({'package':'Microsoft.Net.Compilers 1.3.2','compiler':'1.3.1.60621','sha256':SHA256}))


if __name__=='__main__': main()
