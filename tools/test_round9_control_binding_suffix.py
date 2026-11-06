"""Historical smcs fixtures for leaf reordering and unused local recovery.

Compiles only new toy programs. No original game bytes enter their output.
"""
import argparse
import importlib
import json
from pathlib import Path
import sys
from compare_kairo import differences,sha
from round9_control_ast import Client
from round9_control_il import extract_il
from round9_control_leaf_runs import bind_control_leaves
from round9_part_b_fixtures import compile_smcs
from round9_unused_suffix_calculate import calculate
from round7_slot_model import SlotModelClient

ROOT=Path(__file__).resolve().parents[1]
BEFORE='''class LeafFixture {
 static int total;static void Sink(int n){total+=n;}
 public static void Flow(int n){if(n>0){Sink(1);}else{Sink(2);}}
 public static int Unused(int n){int value=n+1;return value;}
 public static int WithUsed(int n){int value=n+1;int second=n+2;return value+second;}
}'''
EXPECTED=BEFORE.replace('if(n>0){Sink(1);}else{Sink(2);}',
    'if(!(n>0)){Sink(2);}else{Sink(1);}').replace('int value=n+1;return value;',
    'int value=n+1;object originalObject;long originalLong;int originalInt;return value;')


def main():
    p=argparse.ArgumentParser(description=__doc__)
    p.add_argument('--trace-dir',default='build/round9/control-ast')
    args=p.parse_args();trace_dir=ROOT/args.trace_dir;sys.path.insert(0,str(trace_dir))
    trace_name='round9_control_trace' if (trace_dir/'round9_control_trace.py').exists() else 'control_trace'
    project=importlib.import_module(trace_name).project
    folder=ROOT/'build/round9/control-binding-suffix-fixture';folder.mkdir(parents=True,exist_ok=True)
    manifest=json.loads((ROOT/'build/round9/cycles/b10/after/Assembly-CSharp/build-manifest.json').read_text(encoding='utf-8-sig'))
    path=folder/'Analysis.cs';path.write_text(BEFORE,encoding='utf-8',newline='\n')
    analysis=dict(manifest,sources=[{'path':str(path),'input_path':str(path)}])
    mp=folder/'manifest.json';mp.write_text(json.dumps(analysis,indent=2)+'\n',encoding='utf-8')
    with Client(mp) as client:
        before_ast={m['method_id']:m for m in client.extract(path,BEFORE)}
        expected_ast={m['method_id']:m for m in client.extract(path,EXPECTED)}
    with SlotModelClient(mp) as model:
        before_models={m['method_id']:m for m in model.predict_all(path,BEFORE)}
        expected_models={m['method_id']:m for m in model.predict_all(path,EXPECTED)}
    (folder/'predictions-before-compilation.json').write_text(json.dumps(
        {'AST_before':before_ast,'AST_expected':expected_ast,'slots_before':before_models,
         'slots_expected':expected_models},indent=2)+'\n',encoding='utf-8')
    original=compile_smcs(folder/'expected',EXPECTED,manifest)
    compiled_before=compile_smcs(folder/'before',BEFORE,manifest)
    unused='LeafFixture::Unused(System.Int32)->System.Int32'
    flow='LeafFixture::Flow(System.Int32)->System.Void'
    expected_types=expected_models[unused]['predicted_types']
    current_types=before_models[unused]['predicted_types']
    recovery=calculate(BEFORE,before_models[unused],original[unused],compiled_before[unused],expected_types,current_types)
    assert recovery['eligible'] and len(recovery['patches'])==1
    patch=recovery['patches'][0];raw=BEFORE.encode('utf-16-le')
    candidate=(raw[:patch['start']*2]+patch['after'].encode('utf-16-le')+raw[patch['end']*2:]).decode('utf-16-le')
    with SlotModelClient(mp) as model:
        predicted=model.predict(path,candidate,unused)
    assert predicted['complete'] and predicted['predicted_types']==expected_types
    with Client(mp) as client:
        candidate_ast={m['method_id']:m for m in client.extract(path,candidate)}
    compiled=compile_smcs(folder/'candidate',candidate,manifest)
    delta=differences(original[unused],compiled[unused])[0]
    assert delta==[],delta
    assert original[unused]['instructions']==compiled_before[unused]['instructions']
    assert original[unused]['locals']!=compiled_before[unused]['locals']
    original_ir,own_ir=extract_il(original[flow]),extract_il(compiled[flow])
    bound=bind_control_leaves(original[flow],compiled[flow],original_ir,candidate_ast[flow],own_ir,project)
    assert bound['complete'],bound['reasons']
    assert any(r['original_path']!=r['source_path'] for r in bound['bindings'])
    rejected=bind_control_leaves(original[flow],compiled[flow],original_ir,expected_ast[flow],own_ir,project)
    assert not rejected['complete'] and rejected['reasons']==['independent-source-plan-to-own-build-gate-failed']
    used='LeafFixture::WithUsed(System.Int32)->System.Int32'
    rejected_suffix=calculate(BEFORE,before_models[unused],original[used],compiled_before[unused],
        expected_models[used]['predicted_types'],current_types)
    assert not rejected_suffix['eligible']
    result={'operation':'independent-historical-control-binding-and-unused-suffix-fixture',
        'game_compilations':0,'toy_compilations':3,'AST_predictions_captured_before_compilation':True,
        'unused_suffix_full_body_equal':True,'unused_suffix_changes_IL_instructions':False,
        'unused_suffix_original_types':expected_types,'before_types':current_types,
        'synthetic_local_names_are_historical':False,
        'reordered_original_to_source_leaf_bijection_complete':True,
        'reordered_leaf_pairs':sum(r['original_path']!=r['source_path'] for r in bound['bindings']),
        'wrong_source_plan_rejected':True,'missing_used_local_rejected':True,
        'fixture_source_sha256':sha(Path(__file__)),
        'binder_sha256':sha(ROOT/'tools/round9_control_leaf_runs.py'),
        'calculator_sha256':sha(ROOT/'tools/round9_unused_suffix_calculate.py'),
        'compiler_sha256':manifest['compiler_sha256']}
    (ROOT/'reports/round9-control-binding-suffix-fixture.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8',newline='\n')
    print(json.dumps(result))


if __name__=='__main__':main()
