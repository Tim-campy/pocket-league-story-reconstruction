"""Build gate: refuse unvalidated compilers and incomplete source trees."""
import json
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1]


def main():
    toolchain=json.loads((ROOT/'config/toolchain.json').read_text(encoding='utf-8'))
    targets=json.loads((ROOT/'config/coverage.json').read_text(encoding='utf-8'))['target_assemblies']
    blockers=[]
    for name in targets:
        if not toolchain.get('assemblies',{}).get(name,{}).get('validated',False):
            blockers.append(name+': compiler and flags not validated')
        order=ROOT/f'config/compile-order/{name}.txt'
        sources=[line for line in order.read_text(encoding='utf-8').splitlines() if line and not line.startswith('#')]
        missing=[source for source in sources if not (ROOT/source).is_file()]
        if missing: blockers.append(f'{name}: {len(missing)} source files not recovered yet')
    blockers.append('Compiler invocation and provenance logging pending validation')
    print(json.dumps({'build':'BLOCKED','blockers':blockers})); return 2


if __name__=='__main__': raise SystemExit(main())
