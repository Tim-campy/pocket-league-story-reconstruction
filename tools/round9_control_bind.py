"""Read-only binding audit for complete pending smcs control skeletons."""
import argparse
from collections import Counter
import importlib
import hashlib
import json
from pathlib import Path
import sys
from compare_kairo import sha
from round9_control_il import extract_il
from round9_control_leaf_runs import bind_control_leaves

ROOT=Path(__file__).resolve().parents[1]


def read(path):return json.loads(Path(path).read_text(encoding='utf-8-sig'))


def main():
    p=argparse.ArgumentParser(description=__doc__)
    p.add_argument('--validation',default='reports/round9-control-validation-public.json')
    p.add_argument('--trace-dir',default='build/round9/control-ast')
    p.add_argument('--output',default='reports/round9-control-leaf-bindings.json')
    p.add_argument('--matched-only',action='store_true',help='Validate association consistency on already verified bodies')
    args=p.parse_args()
    trace_dir=ROOT/args.trace_dir
    sys.path.insert(0,str(trace_dir))
    trace_name='round9_control_trace' if (trace_dir/'round9_control_trace.py').exists() else 'control_trace'
    trace=importlib.import_module(trace_name)
    validation=read(ROOT/args.validation)
    if not validation['valid_frozen_input_measurement']:
        raise ValueError('Validation inputs changed during its extraction')
    data=read(ROOT/validation['method_irs'])
    base=ROOT/validation['snapshot']/'Assembly-CSharp'
    original={r['stable_id']:r for r in read(base/'original-methods.json')['selected_methods']}
    compiled={r['stable_id']:r for r in read(base/'rebuilt-methods.json')['selected_methods']}
    receipt=read(base/'verification.json')
    source_hashes={r['path']:r['sha256'] for r in receipt['sources']}
    receipt_sha=sha(base/'verification.json')
    own_dump_sha=sha(base/'rebuilt-methods.json')
    rows=[]
    for row in validation['methods']:
        eligible_state=(row['state']=='matched') if args.matched_only else (row['state']!='matched')
        if not eligible_state or not row.get('paired_tree_complete'):
            continue
        identity=row['id'];pair=data[identity]
        logical=pair['source_ast_ir']['source']
        source_file=base/logical
        if sha(source_file)!=source_hashes[logical]:
            raise ValueError('Frozen source differs from own-compilation receipt: '+logical)
        source=source_file.read_bytes().decode('utf-8-sig')
        source_raw=source.encode('utf-16-le')
        for leaf in pair['source_ast_ir']['leaves']:
            if leaf.get('text')!=source_raw[leaf['source_start']*2:leaf['source_end']*2].decode('utf-16-le'):
                raise ValueError('AST leaf text does not match the proved source span: '+identity)
        own_ir=extract_il(compiled[identity])
        bound=bind_control_leaves(original[identity],compiled[identity],
            pair['original_il_ir'],pair['source_ast_ir'],own_ir,trace.project)
        rows.append(dict(bound,id=identity,skeletons_equal_before=row.get('trees_equal',False),
            logical_source=logical,source_text_sha256=hashlib.sha256(source.encode('utf-8')).hexdigest(),
            source_file_sha256=source_hashes[logical],own_body_source_bound=True,
            own_compilation_receipt_sha256=receipt_sha,own_dump_sha256=own_dump_sha))
    report={'operation':'matched-control-leaf-binding-consistency' if args.matched_only else 'complete-pending-control-leaf-binding-audit',
            'snapshot':validation['snapshot'], 'validation':args.validation,
            'validation_sha256':sha(ROOT/args.validation),
            'binder_sha256':sha(ROOT/'tools/round9_control_leaf_runs.py'),
            'trace_module':str(Path(trace.__file__).relative_to(ROOT)),
            'trace_sha256':sha(Path(trace.__file__)),
            'own_compilation_body_receipt_sha256':sha(base/'verification.json'),
            'paired_complete':len(rows),'bindings_complete':sum(r['complete'] for r in rows),
            'different_skeletons_bindable':sum(r['complete'] and not r['skeletons_equal_before'] for r in rows),
            'same_skeletons_bindable':sum(r['complete'] and r['skeletons_equal_before'] for r in rows),
            'incomplete_groups':dict(Counter(reason for r in rows for reason in set(r['reasons']))),
            'methods':rows}
    (ROOT/args.output).write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8',newline='\n')
    print(json.dumps({k:v for k,v in report.items() if k!='methods'}))


if __name__=='__main__':main()
