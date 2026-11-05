"""Build recovered assemblies with the documented candidate compiler."""
import argparse
import json
from pathlib import Path
import subprocess
import sys
from kairo_candidate import COMPILER_PATH,LABEL
ROOT=Path(__file__).resolve().parents[1]


def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--assembly',choices=['KairoLibrary','Assembly-CSharp'])
    args=parser.parse_args()
    if args.assembly in (None,'KairoLibrary'):
        compiler=ROOT/COMPILER_PATH
        if not compiler.is_file():
            print('Compiler missing: run python tools/fetch_roslyn_neighbors.py');return 2
        code=subprocess.run([sys.executable,str(ROOT/'tools/build_kairo.py'),'--compiler',str(compiler),'--label',LABEL],cwd=ROOT).returncode
        if code: return code
    if args.assembly in (None,'Assembly-CSharp'):
        code=subprocess.run([sys.executable,str(ROOT/'tools/build_csharp.py')],cwd=ROOT).returncode
        if code:return code
    return 0


if __name__=='__main__': raise SystemExit(main())
