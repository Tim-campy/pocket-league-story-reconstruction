"""Observe byte differences between two identical full candidate builds.

No output editing or normalization; observations are not comparison exceptions.
"""
import json
from pathlib import Path
import struct
import subprocess
import sys
import time
from probe_reproducibility import snapshot

ROOT=Path(__file__).resolve().parents[1]


def volatile_ranges(data,info):
    result=[{'field':'MVID','offset':info['mvid_offset'],'size':16},
        {'field':'COFF timestamp','offset':info['timestamp_offset'],'size':4}]
    pe=struct.unpack_from('<I',data,60)[0];optional=pe+24
    directory=optional+(96 if struct.unpack_from('<H',data,optional)[0]==0x10b else 112)
    rva,size=struct.unpack_from('<II',data,directory+6*8)
    first=optional+struct.unpack_from('<H',data,pe+20)[0]
    def offset(address):
        for index in range(struct.unpack_from('<H',data,pe+6)[0]):
            section=first+40*index;virtual,va,raw,ptr=struct.unpack_from('<IIII',data,section+8)
            if va<=address<va+max(virtual,raw):return ptr+address-va
        raise ValueError('Unmapped PE RVA')
    if rva:
        start=offset(rva)
        for entry in range(start,start+size,28):
            result.append({'field':'debug directory timestamp','offset':entry+4,'size':4})
            kind=struct.unpack_from('<I',data,entry+12)[0];pointer=struct.unpack_from('<I',data,entry+24)[0]
            if kind==2 and data[pointer:pointer+4]==b'RSDS':
                result.append({'field':'CodeView PDB GUID','offset':pointer+4,'size':16})
    return result


def main():
    command=[sys.executable,str(ROOT/'tools/build_kairo.py'),'--compiler',
        str(ROOT/'analysis/tools/roslyn-1.3.2/tools/csc.exe'),'--label','roslyn-1.3.2']
    target=ROOT/'build/KairoLibrary-roslyn-1.3.2/KairoLibrary.dll'
    subprocess.run(command,cwd=ROOT,check=True,capture_output=True);first,a=snapshot(target,'KairoLibrary')
    manifest=json.loads((target.parent/'build-manifest.json').read_text())
    time.sleep(1.1)
    subprocess.run(command,cwd=ROOT,check=True,capture_output=True);second,b=snapshot(target,'KairoLibrary')
    other_manifest=json.loads((target.parent/'build-manifest.json').read_text())
    for key in ('compiler_sha256','flags','sources','references'):
        if manifest[key]!=other_manifest[key]:raise ValueError('Build input changed: '+key)
    if len(first)!=len(second):raise ValueError('Build lengths differ')
    ranges=volatile_ranges(first,a)
    changed=[i for i,(x,y) in enumerate(zip(first,second)) if x!=y]
    known={i for item in ranges for i in range(item['offset'],item['offset']+item['size'])}
    for item in ranges:
        item['changed_bytes']=sum(i in changed for i in range(item['offset'],item['offset']+item['size']))
    result={'scope':'two identical full KairoLibrary builds, package 1.3.2, debug:pdbonly, deterministic-',
        'first':a,'second':b,'changed_byte_count':len(changed),'identified_ranges':ranges,
        'unexplained_changed_offsets':[i for i in changed if i not in known],
        'comparison_exceptions_applied':False,'original_build_determinism':'UNKNOWN'}
    (ROOT/'reports/KairoLibrary-repeatability.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8')
    print(json.dumps(result))
    if result['unexplained_changed_offsets']:raise ValueError('Unexplained repeated-build differences')


if __name__=='__main__':main()
