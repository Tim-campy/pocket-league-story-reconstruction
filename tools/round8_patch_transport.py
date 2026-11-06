"""Transport an LF source patch to an exact, unique CRLF declaration; no binary editing."""
import argparse,hashlib,json
from pathlib import Path
from round7_coalesce_calculate import rebase_unique_patch
ROOT=Path(__file__).resolve().parents[1]
def digest(data):return hashlib.sha256(data).hexdigest()
def transport(source,patch):
    try:return rebase_unique_patch(source,patch),False
    except ValueError:pass
    if '\r' in patch['before'] or '\r' in patch['after'] or '\r\n' not in source:
        raise ValueError('Only a uniformly LF patch may be transported to CRLF')
    converted=dict(patch,before=patch['before'].replace('\n','\r\n'),after=patch['after'].replace('\n','\r\n'))
    return rebase_unique_patch(source,converted),True
def main():
    ap=argparse.ArgumentParser(description=__doc__);ap.add_argument('--input',type=Path,required=True);ap.add_argument('--output',type=Path,required=True);args=ap.parse_args()
    if args.output.exists():raise FileExistsError('Preserve prior patch transports')
    doc=json.loads(args.input.read_text(encoding='utf-8'));patches=[];proof=[]
    for patch in doc['patches']:
        path=(ROOT/patch['logical_path']).resolve()
        if not path.is_relative_to(ROOT/'src'):raise ValueError('Outside source tree')
        raw=path.read_bytes();source=raw.decode('utf-8-sig')
        result,converted=transport(source,patch);patches.append(result)
        proof.append({'logical_path':patch['logical_path'],'source_sha256':digest(raw),'converted_lf_patch_only':converted,
            'exact_unique_rebase':True,'raw_source_modified':False,'before_utf16_length':len(result['before'].encode('utf-16-le'))//2})
    result=dict(doc,patches=patches,transport={'input':str(args.input),'input_sha256':digest(args.input.read_bytes()),'source_changes':False,'proof':proof})
    args.output.write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8',newline='\n');print(json.dumps(proof))
if __name__=='__main__':main()
