"""Observe nondeterministic fields by repeating identical probe compiler commands.

Records observations only. Does not normalize or modify any compiled artifact,
and does not add exceptions to the game's final comparison policy.
"""
import hashlib
import json
import os
from pathlib import Path
import struct
import subprocess
import time
import uuid

ROOT=Path(__file__).resolve().parents[1]


def snapshot(target,assembly):
    data=target.read_bytes()
    p=subprocess.run([str(ROOT/'build/tools/ProbeInspector.exe'),str(target),
        'data.BaseData::Check' if assembly=='Assembly-CSharp' else 'java.lang.JMath::Abs'],
        capture_output=True,text=True,encoding='utf-8',check=True)
    module=json.loads(p.stdout); mvid=uuid.UUID(module['mvid']).bytes_le
    offset=data.find(mvid)
    if offset<0 or data.find(mvid,offset+1)>=0: raise ValueError('MVID coordinate ambiguous')
    timestamp=struct.unpack_from('<I',data,60)[0]+8
    return data,{'sha256':hashlib.sha256(data).hexdigest(),'mvid':module['mvid'],
        'mvid_offset':offset,'timestamp_offset':timestamp,'timestamp':struct.unpack_from('<I',data,timestamp)[0]}


def main():
    report=json.loads((ROOT/'analysis/compiler-probe-results.json').read_text())
    observations=[]
    for assembly,candidate in [('Assembly-CSharp','mono-smcs'),('KairoLibrary','roslyn-csc')]:
        item=next(a for a in report['attempts'] if a['assembly']==assembly and a['candidate']==candidate and a['optimize'])
        env=os.environ.copy();env.update(item['scoped_environment'])
        source=ROOT/f'build/compiler-probes/{assembly}.cs'
        command=item['compiler_driver']+item['flags']+[str(source)]
        target=Path(next(f[5:] for f in item['flags'] if f.startswith('-out:')))
        subprocess.run(command,env=env,capture_output=True,check=True)
        first,a=snapshot(target,assembly)
        time.sleep(1.2)
        subprocess.run(command,env=env,capture_output=True,check=True)
        second,b=snapshot(target,assembly)
        if len(first)!=len(second): raise ValueError('Size changed between identical builds')
        changed=[i for i,(x,y) in enumerate(zip(first,second)) if x!=y]
        known=set(range(a['mvid_offset'],a['mvid_offset']+16))|set(range(a['timestamp_offset'],a['timestamp_offset']+4))
        unknown=[i for i in changed if i not in known]
        result={'assembly':assembly,'candidate':candidate,'first':a,'second':b,
            'changed_byte_count':len(changed),'unexplained_changed_offsets':unknown,
            'mvid_changed':a['mvid']!=b['mvid'],'timestamp_changed':a['timestamp']!=b['timestamp'],
            'game_comparison_exception_authorized':False}
        observations.append(result);print(json.dumps(result),flush=True)
        if unknown: raise ValueError('Unexplained nondeterministic bytes')
    (ROOT/'analysis/reproducibility-results.json').write_text(json.dumps(observations,indent=2)+'\n',encoding='utf-8')


if __name__=='__main__': main()
