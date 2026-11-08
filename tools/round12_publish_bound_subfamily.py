"""Publish an authenticated frozen source-only family; no target emission."""
import argparse
import hashlib
import importlib.util
import json
from pathlib import Path
import shutil
import sys

ROOT = next(p for p in Path(__file__).resolve().parents if (p/'AGENTS.md').exists())
sys.path.insert(0,str(ROOT/'tools'))
from round10_clean_added_whitespace import clean_source_lines
from round10_smcs_empty_arms_source import vals

def sha(p):
    return hashlib.sha256(Path(p).read_bytes()).hexdigest()

def read(p):
    return json.loads(Path(p).read_text(encoding='utf-8-sig'))

def main():
    p=argparse.ArgumentParser(description=__doc__)
    p.add_argument('--family',required=True)
    p.add_argument('--publication-label',required=True,help='Unique output namespace; frozen input folder remains unchanged')
    p.add_argument('--frozen-root',default='build/round11',choices=['build/round11','build/round12'])
    p.add_argument('--bundle',required=True)
    p.add_argument('--bundle-sha256',required=True)
    p.add_argument('--index',required=True)
    p.add_argument('--index-sha256',required=True)
    p.add_argument('--calculator',default='calculate.py')
    p.add_argument('--output',required=True)
    p.add_argument('--api',action='append',default=[],help='Explicit rule-prefix=pure-function mapping')
    p.add_argument('--rule-family-prefix',help='Diagnostic alias prefix; preserves original_rule and source payloads')
    a=p.parse_args()
    if not a.publication_label.replace('-','').replace('_','').isalnum():raise ValueError('Unsafe publication namespace')
    folder=ROOT/a.frozen_root/a.family
    bundle=folder/a.bundle; index=folder/a.index
    assert sha(bundle)==a.bundle_sha256 and sha(index)==a.index_sha256
    records=read(index)['files']
    files=records if isinstance(records,dict) else {(folder/r['path']).relative_to(ROOT).as_posix():r['sha256'] for r in records}
    for name,digest in files.items():
        resolved = (ROOT/name).resolve()
        assert (resolved.is_relative_to(folder.resolve()) or
                resolved.is_relative_to(ROOT/'tools') or
                resolved.is_relative_to(ROOT/'analysis/tools/round5-mono-source') or
                resolved.is_relative_to(ROOT/'analysis/tools/round6-roslyn-source')), name
        assert sha(ROOT/name)==digest,name
    files={name.replace(chr(92), chr(47)):digest for name,digest in files.items()}
    files.update({x.relative_to(ROOT).as_posix():sha(x) for x in (bundle,index)})
    def normalize_keys(value):
        if isinstance(value,dict):return {k.replace(chr(92),chr(47)):normalize_keys(v) for k,v in value.items()}
        if isinstance(value,list):return [normalize_keys(v) for v in value]
        return value
    doc=normalize_keys(read(bundle))
    deps=dict(doc.get('dependency_sha256',{}))
    for name,digest in deps.items(): assert sha(ROOT/name)==digest,name
    target=ROOT/'tools'/('round12_'+a.publication_label.replace('-','_')+'_source.py')
    if target.exists():assert sha(target)==sha(folder/a.calculator),'Existing producer differs'
    else:shutil.copyfile(folder/a.calculator,target)
    spec=importlib.util.spec_from_file_location('published_'+a.family.replace('-','_'),target)
    mod=importlib.util.module_from_spec(spec);spec.loader.exec_module(mod)
    fallback={'initial-image-use':'transform_image_null_guard_return','initial-init2':'transform_init2_control_composition'}
    patches=[]
    requested_apis=dict(v.split('=',1) for v in a.api)
    for row in doc['patches']:
        mapped=[value for prefix,value in requested_apis.items() if row['rule'].startswith(prefix)]
        if len(mapped)>1:raise ValueError('Ambiguous API rule prefix')
        api=row.get('api') or (mapped[0] if mapped else fallback[a.family])
        assert getattr(mod,api)(row['before'])==row['after'],row['method_id']
        item=dict(row)
        if a.rule_family_prefix:
            item['original_rule']=row['rule']
            item['rule']=a.rule_family_prefix+row['rule'].removeprefix('r11-')
        previous=item.pop('frozen_evidence_sha256',{})
        item['frozen_evidence_sha256']={k:v for k,v in previous.items() if k.startswith(('build/round11/','build/round12/'))}
        item['historical_frozen_evidence_sha256']={k:v for k,v in previous.items() if k.startswith('build/round10/')}
        item['pinned_compiler_source_sha256']={k:v for k,v in previous.items() if k.startswith(('analysis/tools/round5-mono-source/','analysis/tools/round6-roslyn-source/'))}
        item.setdefault('evidence_sha256',{}).update({k:v for k,v in previous.items() if k.startswith('reports/')})
        item.setdefault('dependency_sha256',{}).update({k:v for k,v in previous.items() if k.startswith('tools/')})
        assert all(k.startswith(('build/round11/','build/round12/','build/round10/','tools/','reports/','analysis/tools/round5-mono-source/','analysis/tools/round6-roslyn-source/')) for k in previous)
        # Preserve historical producer/model provenance; the engine independently
        # predicts every candidate with its current, corpus-validated model.
        item.update(rebase_unique=True,source_span_units='UTF16',game_body_status='UNKNOWN',pure_api=api)
        patches.append(item)
    deps[target.relative_to(ROOT).as_posix()]=sha(target)
    deps[Path(__file__).relative_to(ROOT).as_posix()]=sha(Path(__file__))
    evidence={}
    own={};format_proof=[]
    for name in ['fixture-results.json','fixture-report.json','history-sha-gates.json','history-sha-audit.json','model-comparison.json','model-control-results.json','compiler-citations.json','abstract-emission-audit.json']:
        src=folder/name
        if not src.exists():continue
        dst=ROOT/'reports'/('round12-'+a.publication_label+'-'+name)
        assert not dst.exists();shutil.copyfile(src,dst)
        evidence[dst.relative_to(ROOT).as_posix()]=sha(dst)
    fixture_files=[x for x in folder.rglob('*.cs') if ('fixture' in str(x.relative_to(folder)).lower() or x.name.startswith(('Fixture','Runner','ModelControls'))) and x.name!='Round11PrivateSlotModel.cs']
    for src in fixture_files:
        dst=ROOT/'tools/fixtures'/('round12-'+a.publication_label)/src.relative_to(folder)
        assert not dst.exists();dst.parent.mkdir(parents=True,exist_ok=True);shutil.copyfile(src,dst)
        before=dst.read_bytes().decode('utf-8')
        lines=[n for n,line in enumerate(before.splitlines(),1) if line.rstrip(' \t')!=line]
        if lines:
            after,changed,blank,nonblank,refused=clean_source_lines(before,lines)
            assert not refused and changed==lines and vals(before)==vals(after)
            dst.write_bytes(after.encode('utf-8'))
            format_proof.append(dict(path=dst.relative_to(ROOT).as_posix(),private_sha256=sha(src),
                                    public_sha256=sha(dst),trivia_only_lines=changed,
                                    tokens_and_literals_unchanged=True,line_endings_preserved=True))
        own[dst.relative_to(ROOT).as_posix()]=sha(dst)
    note=next((folder/n for n in ['NOTE.md','README.md'] if (folder/n).exists()),None)
    if note:
        dst=ROOT/'docs/notes'/('round12-'+a.publication_label+'.md')
        assert not dst.exists();shutil.copyfile(note,dst)
    frozen=dict(doc.get('frozen_evidence_sha256',{}));frozen.update(files)
    historical={k:frozen.pop(k) for k in list(frozen) if k.startswith('build/round10/')}
    compiler_sources={k:frozen.pop(k) for k in list(frozen) if k.startswith(('analysis/tools/round5-mono-source/','analysis/tools/round6-roslyn-source/'))}
    evidence.update({k:frozen.pop(k) for k in list(frozen) if k.startswith('reports/')})
    for name in list(frozen):
        if name.startswith('tools/'):
            deps[name]=frozen.pop(name)
    assert all(k.startswith(('build/round11/','build/round12/')) for k in frozen)
    for name in ['tools/round10_clean_added_whitespace.py','tools/round10_smcs_empty_arms_source.py']:
        deps[name]=sha(ROOT/name)
    result=dict(source_only=True,patches=patches,dependency_sha256=deps,evidence_sha256=evidence,
                frozen_evidence_sha256=frozen,historical_frozen_evidence_sha256=historical,
                pinned_compiler_source_sha256=compiler_sources,
                own_fixture_source_sha256=own,public_pure_api_reproduction_exact=True,target_compilations=0,
                own_fixture_trivia_format_review=format_proof,
                note='Candidate source only; independent live model and game oracle remain mandatory.')
    out=ROOT/a.output;assert not out.exists()
    out.write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({'family':a.family,'patches':len(patches),'frozen_files_checked':len(files),'own_fixture_sources':len(own),'output':a.output,'sha256':sha(out),'target_compilations':0}))

if __name__=='__main__': main()
