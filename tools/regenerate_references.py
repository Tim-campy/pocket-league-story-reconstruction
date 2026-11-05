"""Regenerate missing ref/ listings from the user's own pinned DLLs.

Does not reset ledgers, need retained private REA bundles, or modify original/.
Reference text is never used as build input. Existing listings are preserved.
"""
import hashlib
import json
from pathlib import Path
import subprocess
import sys

ROOT=Path(__file__).resolve().parents[1]


def token(method):
    return '0x'+format(0x06000000|int(method['token'].split('0x')[1].rstrip(']'),16),'08x')


def main():
    references=json.loads((ROOT/'config/method-reference.json').read_text(encoding='utf-8'))
    missing={key:ref for key,ref in references.items() if not (ROOT/ref['reference_il']).is_file()}
    if not missing:print(json.dumps({'missing_listings':0,'written':0}));return
    subprocess.run([sys.executable,str(ROOT/'tools/prepare_probe_inspector.py')],cwd=ROOT,check=True,capture_output=True)
    project=json.loads((ROOT/'config/project.json').read_text(encoding='utf-8'))
    by_artifact={}
    for key,ref in missing.items():by_artifact.setdefault(ref['artifact_path'],[]).append((key,ref))
    written=0
    for artifact,rows in by_artifact.items():
        path=ROOT/project['extracted']/artifact;data=path.read_bytes();digest=hashlib.sha256(data).hexdigest()
        if any(ref['artifact_sha256']!=digest for _,ref in rows):raise ValueError('Original assembly identity changed')
        result=subprocess.run([str(ROOT/'build/tools/ProbeInspector.exe'),str(path),'--all'],capture_output=True,check=True)
        methods={token(m):m for m in json.loads(result.stdout.decode('utf-8-sig'))['selected_methods']}
        for key,ref in rows:
            method=methods[ref['token']];lines=['// REFERENCE ONLY. Never a compiler/build input.',
                '// '+key,'// Original SHA256: '+digest]
            body=ref['body']
            if body:
                span=data[body['file_offset']:body['file_offset']+body['size']]
                if hashlib.sha256(span).hexdigest()!=body['sha256']:raise ValueError('Original body span differs')
                lines.append('// header: '+json.dumps(method['header'],ensure_ascii=True))
                lines.append('// locals: '+json.dumps(method['locals'],ensure_ascii=True))
                for instruction in method['instructions']:
                    lines.append('IL_'+format(instruction['offset'],'04x')+': '+instruction['opcode']+' '+json.dumps(instruction['operand'],ensure_ascii=True))
                lines.append('// exception regions: '+json.dumps(method['exception_regions'],ensure_ascii=True))
            else:lines.append('// No CIL body in original declaration.')
            output=(ROOT/ref['reference_il']).resolve()
            if not output.is_relative_to((ROOT/'ref').resolve()):raise ValueError('Reference output outside ref/')
            output.parent.mkdir(parents=True,exist_ok=True);output.write_text('\n'.join(lines)+'\n',encoding='utf-8',newline='\n');written+=1
    print(json.dumps({'missing_listings':len(missing),'written':written}))


if __name__=='__main__':main()
