"""Build recovered assemblies with the documented candidate compiler."""
import argparse
import json
from pathlib import Path
import subprocess
import sys
ROOT=Path(__file__).resolve().parents[1]


def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--assembly',choices=['KairoLibrary','Assembly-CSharp'])
    args=parser.parse_args()
    if args.assembly in (None,'KairoLibrary'):
        compiler=ROOT/'analysis/tools/roslyn-1.3.2/tools/csc.exe'
        if not compiler.is_file():
            print('Compiler missing: run python tools/fetch_roslyn.py');return 2
        code=subprocess.run([sys.executable,str(ROOT/'tools/build_kairo.py'),'--compiler',str(compiler),'--label','roslyn-1.3.2'],cwd=ROOT).returncode
        if code: return code
    if args.assembly in (None,'Assembly-CSharp'):
        print(json.dumps({'assembly':'Assembly-CSharp','build':'BLOCKED','reason':'96 type source files not recovered'}));return 2
    return 0


if __name__=='__main__': raise SystemExit(main())
