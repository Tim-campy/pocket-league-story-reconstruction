"""Freeze source sequence points only where debug+ reproduces the debug- body exactly."""
import argparse
import json
from pathlib import Path
from round9_engine import Compiler
from round4_symbols import inspect_methods
from compare_kairo import differences,sha
ROOT=Path(__file__).resolve().parents[1]
def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--snapshot',default='build/round9/cycles/a05/after')
    parser.add_argument('--label',default='control-mapping')
    parser.add_argument('--output',default='reports/round9-control-source-mapping.json')
    args=parser.parse_args()
    baseline=args.snapshot
    folder=(ROOT/'build/round9/runs'/args.label/'engine/Assembly-CSharp').resolve()
    if not folder.is_relative_to((ROOT/'build/round9/runs').resolve()):
        raise ValueError('Mapping reset path escapes round-nine runs')
    c=Compiler('Assembly-CSharp',reset=True,baseline_dir=baseline,run_label=args.label)
    binary=c.build(debug=True)
    if binary is None:raise RuntimeError(c.last_log)
    methods=inspect_methods(binary,None,symbols=True)
    plain={m['stable_id']:m for m in json.loads((c.base/'rebuilt-methods.json').read_text(encoding='utf-8-sig'))['selected_methods']}
    equal=[];different=[];without=[]
    for method in methods['methods']:
        identity=method['stable_id']
        if not differences(plain.get(identity),method)[0]:
            equal.append(identity)
            if not method.get('sequence_points'):without.append(identity)
        else:different.append(identity)
    data=c.folder/'mapping-symbols.json'
    data.write_text(json.dumps(dict(methods,plain_body_equal_ids=equal,body_different_ids=different),indent=2)+'\n',encoding='utf-8',newline='\n')
    proof={'operation':'own-source-MDB-mapping-control','baseline':baseline,
        'debug_flags_delta':['-debug- -> -debug+'],'target_source_changes':False,
        'structure_inferred_from_symbols':False,'per_method_exact_body_gate':True,
        'debug_binary':str(binary.relative_to(ROOT)),'debug_binary_sha256':sha(binary),
        'symbols_dump':str(data.relative_to(ROOT)),'symbols_dump_sha256':sha(data),
        'baseline_manifest_sha256':sha(c.base/'build-manifest.json'),
        'methods_equal':len(equal),'methods_different':len(different),'equal_without_points':len(without),
        'sources':[{'logical':k,'physical':str(v.relative_to(ROOT)),'sha256':sha(v)} for k,v in c.paths.items()]}
    (ROOT/args.output).write_text(json.dumps(proof,indent=2)+'\n',encoding='utf-8',newline='\n')
    print(json.dumps({k:v for k,v in proof.items() if k!='sources'}))
if __name__=='__main__':main()
