"""Frozen-control-IR source application audit. Never compiles a target assembly.

The extractor directory must be a hash-bound frozen copy, never an in-progress
extractor. Slot predictions are analysis only: incomplete predictions do not
veto proposals, while complete wrong predictions do. The game oracle remains
the sole judge of method equality and adoption.
"""
import argparse
import collections
import hashlib
import importlib
import json
from pathlib import Path
import sys

ROOT=next(p for p in Path(__file__).resolve().parents if (p/'tools/round7_slot_model.py').exists())


def read(path):
    return json.loads(Path(path).read_text(encoding='utf-8-sig'))


def sha(path):
    return hashlib.sha256(Path(path).read_bytes()).hexdigest()


def resolved(path):
    p=Path(path)
    return p if p.is_absolute() else ROOT/p


def install_frozen_extractor(validation,directory):
    """Verify immutable source inputs before any imported analysis helper runs."""
    directory=resolved(directory)
    dependencies={}
    for logical,expected in validation['extractor_inputs'].items():
        p=directory/Path(logical.replace('\\','/')).name
        if not p.exists() or sha(p)!=expected:
            raise ValueError('Frozen extractor dependency absent or changed: '+str(p))
        dependencies[str(p)]=expected
    sys.path[:0]=[str(directory),str(ROOT/'tools')]
    for name in ('round10_control_il','round10_control_ast'):
        if name in sys.modules:raise ValueError('Extractor was imported before frozen-input checks: '+name)
    il=importlib.import_module('round10_control_il')
    ast=importlib.import_module('round10_control_ast')
    if Path(il.__file__).resolve().parent!=directory.resolve() or Path(ast.__file__).resolve().parent!=directory.resolve():
        raise ValueError('Extractor import did not resolve to the frozen directory')
    il.CONTROL_STAGE=validation['stage']
    return il,ast,dependencies


def calculated_slot_candidates(source,model,original,compiled,expected,compiled_types):
    """Use actual DTO votes as constraints, without pretending model completeness.

    Unlike Solver7.proposals(), this adapter does not return early when the
    prediction is incomplete. Predicted slot numbers remain a hypothesis; every
    proposal is re-predicted and independently AST-checked, then awaits the real
    compiler's signature/body checks. No model field is overwritten.
    """
    from round7_slot_solver import correspondences,_group_candidates,_edit,_patch
    slots,evidence=correspondences(original,compiled,expected,compiled_types)
    declared=[v for v in model.get('variables',[]) if v.get('kind')=='declared' and v.get('statement_start',-1)>=0 and isinstance(v.get('slot'),int)]
    scopes=collections.defaultdict(list)
    for v in declared:
        if v['slot'] in slots:scopes[v['lexical_scope_start']].append(v)
    patches=[];combined=[];constraints=[]
    for variables in scopes.values():
        ordered=sorted(variables,key=lambda v:slots[v['slot']]);runs=[];run=[]
        for v in ordered:
            if run and slots[v['slot']]!=slots[run[-1]['slot']]+1:
                if len(run)>1:runs.append(run)
                run=[]
            run.append(v)
        if len(run)>1:runs.append(run)
        for group in runs:
            edits,rows=_group_candidates(source,model,original,expected,slots,declared,group)
            patches.extend(rows);combined.extend(edits)
    if len(combined)>1:
        intervals=sorted((a,b) for a,b,_ in combined)
        if all(a>=previous for (_,previous),(a,_) in zip(intervals,intervals[1:])):
            patches.append(_patch(source,model,_edit(source,combined),'solver-group-all-calculated-blocks'))
    if not model.get('complete'):constraints.append('incomplete-model-slot-identities-remain-hypotheses; real-compiler-gate-required')
    if not patches:constraints.append('no-calculated-same-scope-consecutive-declaration-group')
    return {'patches':list({sha_text(p['after']):p for p in patches}.values()),'constraints':constraints,'slot_correspondence':evidence,'model_complete':model.get('complete',False),'model_was_promoted':False}


def sha_text(text):
    return hashlib.sha256(text.encode('utf-8')).hexdigest()


def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--validation',required=True)
    parser.add_argument('--extractor-dir',required=True)
    parser.add_argument('--out',required=True,help='New WIP directory; existing output files are refused')
    parser.add_argument('--expected-pending',type=int,default=114)
    parser.add_argument('--audit-only',action='store_true',help='Bind frozen trees/leaves; do not start AST or slot analysis processes')
    parser.add_argument('--protocol',default='config/round10-protocol.json')
    args=parser.parse_args()
    validation_path=resolved(args.validation);validation=read(validation_path)
    if not validation.get('valid_frozen_input_measurement') or validation.get('inputs_changed_during_run'):
        raise ValueError('Control validation is not a valid frozen measurement')
    details_path=resolved(validation['method_irs'])
    if sha(details_path)!=validation['method_irs_sha256']:raise ValueError('Method IR evidence changed')
    il,ast_module,extractor_hashes=install_frozen_extractor(validation,args.extractor_dir)
    from round10_control_trace import project
    from round10_control_leaf_runs import bind_control_leaves
    from round10_control_rewrite import rewrite_control
    from round7_slot_solver import _edit
    protocol_path=resolved(args.protocol);protocol=read(protocol_path)
    model_config=protocol['models']['Assembly-CSharp']
    model_module=importlib.import_module(model_config['module'])
    base=resolved(validation['snapshot'])/'Assembly-CSharp'
    receipt_path=base/'verification.json';receipt=read(receipt_path)
    source_hashes={r['path']:r['sha256'] for r in receipt['sources']}
    original_path=base/'original-methods.json';own_path=base/'rebuilt-methods.json'
    originals={m['stable_id']:m for m in read(original_path)['selected_methods']}
    own={m['stable_id']:m for m in read(own_path)['selected_methods']}
    expected_types=read(base/'plain-local-types.json');own_types=read(base/'plain-local-types-rebuilt.json')
    details=read(details_path);manifest=read(base/'build-manifest.json')
    if receipt['build_sha256']!=manifest['output_sha256']:raise ValueError('Build manifest and verification receipt refer to different binaries')
    artifacts_path=base.parent/'assemblies/artifacts.json'
    artifact=read(artifacts_path)['Assembly-CSharp'];binary_path=resolved(artifact['path'])
    if sha(binary_path)!=artifact['sha256'] or artifact['sha256']!=receipt['build_sha256']:
        raise ValueError('Frozen compiled artifact does not match the own-build receipt')
    reference_hashes={}
    for reference in manifest['references']:
        ref=resolved(reference['path']);digest=sha(ref)
        if digest!=reference['sha256']:raise ValueError('Frozen reference metadata changed: '+str(ref))
        reference_hashes[str(ref)]=digest
    for item in manifest['sources']:
        item['input_path']=str((base/item['path']).resolve())
        if sha(base/item['path'])!=item['sha256'] or item['sha256']!=source_hashes[item['path']]:
            raise ValueError('Source is not bound to frozen compilation receipt: '+item['path'])
        # Analyzer dictionaries use logical full paths; physical reads remain
        # bound to the snapshot via input_path. This is independent of cwd and
        # never reads the current Main source in place of the frozen file.
        item['path']=str((ROOT/item['path']).resolve())
    out=resolved(args.out);out.mkdir(parents=True,exist_ok=True)
    for name in ('audit.json','patches.json','source-manifest.json'):
        if (out/name).exists():raise FileExistsError(out/name)
    manifest_path=out/'source-manifest.json'
    manifest_path.write_text(json.dumps(manifest,indent=2)+'\n',encoding='utf-8',newline='\n')
    dependencies={str((ROOT/'tools'/('round10_control_'+name+'.py')).relative_to(ROOT)):sha(ROOT/'tools'/('round10_control_'+name+'.py')) for name in ('trace','leaf_runs','rewrite','render_fragments','apply')}
    dependencies.update({p:sha(ROOT/p) for p in (model_config['source'],model_config['client'],model_config['solver'])})
    evidence={str(validation_path):sha(validation_path),str(details_path):sha(details_path),str(receipt_path):sha(receipt_path),str(original_path):sha(original_path),str(own_path):sha(own_path),str(protocol_path):sha(protocol_path),str(artifacts_path):sha(artifacts_path),str(binary_path):sha(binary_path),str(base/'build-manifest.json'):sha(base/'build-manifest.json')}
    initial_hashes=dict(extractor_hashes,**{str(resolved(k)):v for k,v in dependencies.items()},**evidence,**reference_hashes)
    rows=[];patches=[];bindings=[]
    pending=[m for m in validation['methods'] if m['state']!='matched']
    if len(pending)!=args.expected_pending:raise ValueError('Pending corpus size differs: '+str(len(pending)))
    ast=slots=None
    try:
        if not args.audit_only:
            ast=ast_module.Client(manifest_path,stage=validation['stage'])
            slots=model_module.SlotModelClient(manifest_path)
        for m in pending:
            key=m['id'];pair=details[key];goal=pair['original_il_ir'];source=pair.get('source_ast_ir')
            row={'method_id':key,'state':m['state'],'original_il_bytes':m['original_il_bytes'],'patches':0,'game_compilations':0,'reasons':[]};rows.append(row)
            if source is None:row['reasons']=['source-method-unmapped'];continue
            row.update(original_tree_complete=goal.get('tree_complete',False),original_plan_complete=goal.get('emission_plan_complete',False),source_tree_complete=source.get('tree_complete',False),source_plan_complete=source.get('emission_plan_complete',False))
            if not all(row[k] for k in ('original_tree_complete','original_plan_complete','source_tree_complete','source_plan_complete')):
                row.update(reasons=['original-or-source-IR-incomplete'],original_reasons=goal.get('reasons',[]),source_reasons=source.get('reasons',[]));continue
            logical=source['source'];path=base/logical;helper_path=ROOT/logical;text=path.read_bytes().decode('utf-8-sig')
            for leaf in source['leaves']:
                actual=text.encode('utf-16-le')[leaf['source_start']*2:leaf['source_end']*2].decode('utf-16-le')
                if actual!=leaf.get('text'):raise ValueError('AST leaf text/source span mismatch: '+key)
            row['skeletons_equal']=il.canonical_tree(goal)==il.canonical_tree(source)
            try:
                own_ir=il.extract_il(own[key])
                binding=bind_control_leaves(originals[key],own[key],goal,source,own_ir,project)
            except (ValueError,KeyError,TypeError) as exc:
                row['reasons']=['provenance-or-binding-refused:'+str(exc)];continue
            binding.update(method_id=key,source_text_sha256=sha_text(text),source_file_sha256=source_hashes[logical],own_body_source_bound=True,own_compilation_receipt_sha256=sha(receipt_path),own_dump_sha256=sha(own_path))
            bindings.append(binding);row['leaf_binding_complete']=binding['complete'];row['binding_reasons']=binding['reasons']
            if args.audit_only:row['reasons']=['audit-only-no-candidate-analysis-executed'];continue
            if row['skeletons_equal']:
                model=slots.predict(helper_path,text,key)
                row['slot_model_complete']=model.get('complete',False)
                if model.get('complete') and model.get('predicted_types')==expected_types[key]:
                    row['reasons']=['control-and-predicted-slots-already-equal; value-code-difference-remains-UNKNOWN'];continue
                calculated=calculated_slot_candidates(text,model,originals[key],own[key],expected_types[key],own_types[key])
                row['slot_calculus']={k:v for k,v in calculated.items() if k!='patches'};checks=[];row['variant_checks']=checks
                for p in calculated['patches'][:24]:
                    candidate=_edit(text,[(p['start'],p['end'],p['after'])]);prediction=slots.predict(helper_path,candidate,key)
                    check={'rule':p['rule'],'slot_model_complete':prediction.get('complete',False),'predicted_types':prediction.get('predicted_types',[])};checks.append(check)
                    if prediction.get('complete') and prediction.get('predicted_types')!=expected_types[key]:check['reason']='complete-wrong-slot-signature';continue
                    fresh=ast.extract(helper_path,candidate,key)
                    if len(fresh)!=1 or not fresh[0].get('tree_complete') or not fresh[0].get('emission_plan_complete'):check['reason']='candidate-control-incomplete';continue
                    if il.canonical_tree(fresh[0])!=il.canonical_tree(goal) or il.canonical_plan(fresh[0])!=il.canonical_plan(goal):check['reason']='candidate-control-differs-original';continue
                    check['reason']='source-only-control-proposal-awaits-real-compiler'
                    p.update(method_id=key,logical_path=logical,complete=prediction.get('complete',False),predicted_types=prediction.get('predicted_types',[]),rule='c10-control-equal-declaration-calculus',source_only=True,candidate_compilations=0,control_IR_complete=True,control_plan_equals_original=True,control_tree_equals_original=True)
                    patches.append(p);row['patches']+=1
                row['reasons']=[] if row['patches'] else ['no-approved-control-preserving-slot-calculus-proposal']
            elif not binding['complete']:row['reasons']=['nonbijective-or-unproved-existing-value-leaves']
            else:
                result=rewrite_control(goal,source,text,binding,source_path=helper_path,method_id=key,expected_types=expected_types[key],slot_model_client=slots,ast_client=ast,original_method=originals[key],compiled_method=own[key],compiled_types=own_types[key])
                row.update(reasons=result['reasons'],rewrite_audit=result['audit'],patches=len(result['patches']))
                for p in result['patches']:p['logical_path']=logical;p['rule']='c10-'+p['rule'];patches.append(p)
        if any(sha(path)!=digest for path,digest in initial_hashes.items()):raise ValueError('Inputs changed during application audit')
        for p in patches:
            source=(base/p['logical_path']).read_bytes().decode('utf-8-sig')
            if source.encode('utf-16-le')[p['start']*2:p['end']*2].decode('utf-16-le')!=p['before']:raise ValueError('Exported UTF16 before does not match source')
            p.update(source_file_sha256=sha(base/p['logical_path']),source_text_sha256=sha_text(source),source_snapshot=validation['snapshot']+'/Assembly-CSharp',dependency_sha256=dependencies,extractor_dependency_sha256=extractor_hashes,evidence_sha256=evidence,calculator_sha256=sha(Path(__file__)),syntax_source_sha256=validation['extractor_inputs'][next(k for k in validation['extractor_inputs'] if k.endswith('Round10ControlAst.cs'))],model_source_sha256=sha(ROOT/model_config['source']),client_sha256=sha(ROOT/model_config['client']),own_compilation_receipt_sha256=sha(receipt_path),own_dump_sha256=sha(own_path),original_dump_sha256=sha(original_path),own_body_source_bound=True,rebase_policy='unique-exact-UTF16-before-only;fresh-model-and-real-compiler-required')
        ids={p['method_id'] for p in patches};potential=sum(originals[k]['header']['code_size'] for k in ids)
        audit={'operation':'round10-control-application-all-pending-source-only-audit','snapshot':validation['snapshot'],'stage':validation['stage'],'audit_only':args.audit_only,'pending_bodies':len(rows),'game_compilations':0,'adopted':False,'methods_with_proposals':len(ids),'potential_original_IL_bytes':potential,'gains_measured':False,'incomplete_slot_prediction_is_veto':False,'complete_wrong_slot_prediction_is_veto':True,'methods':rows,'leaf_bindings':bindings,'groups':dict(collections.Counter(reason for row in rows for reason in row['reasons'])),'input_sha256':initial_hashes,'inputs_changed':False}
        bundle={'operation':'round10-control-source-proposals','family':'c10-control-tree','source_only':True,'game_compilations':0,'adopted':False,'potential_methods':len(ids),'potential_original_IL_bytes':potential,'dependency_sha256':dependencies,'extractor_dependency_sha256':extractor_hashes,'evidence_sha256':evidence,'patches':patches}
        for name,value in (('audit.json',audit),('patches.json',bundle)):
            (out/name).write_text(json.dumps(value,indent=2)+'\n',encoding='utf-8',newline='\n')
        print(json.dumps({k:v for k,v in audit.items() if k not in ('methods','leaf_bindings','input_sha256')}))
    finally:
        if ast:ast.close()
        if slots:slots.close()


if __name__=='__main__':main()
