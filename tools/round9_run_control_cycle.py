"""Compile and verify the frozen calculated Part-C candidates, one at a time."""
import json
from pathlib import Path
import subprocess
import sys

ROOT=Path(__file__).resolve().parents[1]


def main():
    bundle=ROOT/'reports/round9-control-rewrite-patches.json'
    candidates=json.loads(bundle.read_text(encoding='utf-8'))['patches']
    identities=sorted({p['method_id'] for p in candidates})
    if len(identities)!=8:
        raise ValueError('Frozen Part-C candidate inventory changed')
    command=[sys.executable,'-X','utf8','tools/round9_cycle.py','--number','11','--phase','C',
        '--assembly','Assembly-CSharp','--rule','c9-control-skeleton',
        '--pair','complete original control tree => existing-source leaves + calculated slots',
        '--hypothesis','Reemitir somente o controle completo com folhas bijetivas e locais calculados reproduz o corpo original',
        '--probe-substring','data.PlayerData::AddExp(System.Int32,System.Boolean)->System.Void',
        '--external-proposals',str(bundle),'--isolate-family','--single-step','--push']
    for identity in identities:command+=['--target-substring',identity]
    log=ROOT/'build/round9/control-cycle.log'
    with log.open('wb') as stream:
        result=subprocess.run(command,cwd=ROOT,stdout=stream,stderr=subprocess.STDOUT)
    if result.returncode:raise RuntimeError('Control cycle failed; see '+str(log))
    print(json.dumps({'Part_C_cycle_completed':True,'targets':len(identities),'log':str(log.relative_to(ROOT))}))


if __name__=='__main__':main()
