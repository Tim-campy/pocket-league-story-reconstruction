"""Independent smcs control renderer fixtures; no game builds."""
import json,os,subprocess,sys,hashlib
from pathlib import Path
ROOT=next(p for p in Path(__file__).resolve().parents if(p/'tools/round7_slot_model.py').exists());sys.path[:0]=[str(ROOT/'tools'),str(Path(__file__).resolve().parent)]
from round9_control_ast import Client
from round7_slot_model import SlotModelClient
from round9_control_il import extract_il,canonical_tree,canonical_plan
from round9_control_leaf_runs import bind_control_leaves
from round7_slot_solver import _edit
from compare_kairo import body_cells,sha
from round9_control_trace import project
from round9_control_rewrite import rewrite_control,calculate_named_slot_variants
WIP=ROOT/'build/round9/control-ast/renderer-fixtures'
CASES=[('IfPolarity','System.Int32','void Test(int n)','if(n>0){Use(1);}else{Use(2);}','if(n<=0){Use(2);}else{Use(1);}'),('FloatPolarity','System.Single,System.Single','int Test(float x,float y)','if(x>y){return 1;}else{return 2;}','if(!(x>y)){return 2;}else{return 1;}'),('LoopForm','System.Int32','int Test(int n)','int i=0;while(i<n){i++;Use(i);}return i;','int i=0;do{i++;Use(i);}while(i<n);return i;')]
CASES.append(('ForIteratorOrder','System.Int32','void Test(int n)','int i=0;while(i<n){Use(i);i++;}','for(int i=0;i<n;i++){Use(i);}'))
BASE=json.loads((ROOT/'build/round9/cycles/b10/after/Assembly-CSharp/build-manifest.json').read_text(encoding='utf-8'))
def compile_file(source,binary):
 binary.parent.mkdir(parents=True,exist_ok=True);args=[x for x in BASE['flags']if x!='-noconfig'and not x.startswith('-out:')]+['-out:'+str(binary)]+['-r:'+r['path']for r in BASE['references']]+[str(source)];rsp=binary.with_suffix('.rsp');rsp.write_text('\n'.join('"'+a+'"'for a in args),encoding='utf-8');env=os.environ.copy();env.update(BASE['scoped_environment']);proc=subprocess.run([BASE['runtime'],BASE['compiler'],'-noconfig','@'+str(rsp)],cwd=ROOT,env=env,capture_output=True);binary.with_suffix('.log').write_bytes(proc.stdout+proc.stderr)
 if proc.returncode:raise RuntimeError(proc.stdout+proc.stderr)
 return json.loads(subprocess.check_output([str(ROOT/'build/tools/ProbeInspector.exe'),str(binary),'--all']).decode('utf-8-sig'))['selected_methods']
def fixture_text(signature,body):return 'public static class Fixture {static int state;static void Use(int n){state+=n;}public static '+signature+'{'+body+'}}'
def plain_types(method):return[x.split(' @ ')[0].replace('valuetype ','').replace('class ','')for x in method['locals']]
def main():
 WIP.mkdir(exist_ok=True);rows=[];compiles=0
 for name,args,signature,goal_body,own_body in CASES:
  folder=WIP/name;folder.mkdir(exist_ok=True);source=folder/'Fixture.cs';old=fixture_text(signature,own_body);goal=fixture_text(signature,goal_body);source.write_text(old,encoding='utf-8');mp=folder/'manifest.json';mp.write_text(json.dumps(dict(BASE,sources=[{'path':str(source),'input_path':str(source)}])),encoding='utf-8');key='Fixture::Test('+args+')->'+('System.Void'if signature.startswith('void')else'System.Int32')
  with Client(mp)as ast,SlotModelClient(mp)as slots:
   source_ir=ast.extract(source,old,key)[0] # Before fixture inspection/compilation.
   own_methods=compile_file(source,folder/'own/Fixture.dll');compiles+=1
   goal_source=folder/'Goal.cs';goal_source.write_text(goal,encoding='utf-8');goal_methods=compile_file(goal_source,folder/'goal/Fixture.dll');compiles+=1
   original=next(m for m in goal_methods if m['stable_id']==key);own=next(m for m in own_methods if m['stable_id']==key);original_ir,own_ir=extract_il(original),extract_il(own);bindings=bind_control_leaves(original,own,original_ir,source_ir,own_ir,project);assert bindings['complete'],(name,bindings['reasons']);bindings.update(source_text_sha256=hashlib.sha256(old.encode('utf-8')).hexdigest(),own_body_source_bound=True,own_body_receipt_sha256=sha(folder/'own/Fixture.dll'))
   result=rewrite_control(original_ir,source_ir,old,bindings,source_path=source,method_id=key,expected_types=plain_types(original),slot_model_client=slots,ast_client=ast,original_method=original,compiled_method=own,compiled_types=plain_types(own));assert len(result['patches'])==1,(name,result['reasons']);patch=result['patches'][0];candidate=_edit(old,[(patch['start'],patch['end'],patch['after'])]);source.write_text(candidate,encoding='utf-8');after_methods=compile_file(source,folder/'candidate/Fixture.dll');compiles+=1;after=next(m for m in after_methods if m['stable_id']==key);equal=body_cells(original)==body_cells(after)and original['locals']==after['locals']and original['exception_regions']==after['exception_regions'];assert equal,name
   # A real changed source payload cannot be smuggled through structural equality.
   altered=fixture_text(signature,own_body.replace('Use(2)','Use(3)'))
   if altered!=old:
    # Source-only payload mismatch is caught before invoking the renderer: the
    # existing real compiled method's signature proof belongs to the old source.
    wrong_ast=ast.extract(source,altered,key)[0];assert canonical_plan(wrong_ast)==canonical_plan(source_ir)
    rejected=rewrite_control(original_ir,wrong_ast,altered,bindings,source_path=source,method_id=key,expected_types=plain_types(original),slot_model_client=slots,ast_client=ast,original_method=original,compiled_method=own,compiled_types=plain_types(own));assert not rejected['eligible']and 'source-text-is-not-authenticated-by-own-build-receipt'in rejected['reasons']
   rows.append({'case':name,'whole_body_exact':equal,'il_bytes':original['header']['code_size'],'local_count':len(original['locals']),'binding_complete':True,'source_AST_captured_before_inspecting_IL':True,'slot_composition':patch['slot_composition'],'conditions':result['audit']['render']})
 # Source declarations without initialization produce no value leaf; never drop them.
 folder=WIP/'SilentDeclarationReject';folder.mkdir(exist_ok=True);source=folder/'Fixture.cs';old=fixture_text('void Test(int n)','int silent;if(n<=0){Use(2);}else{Use(1);}');goal=fixture_text('void Test(int n)','int silent;if(n>0){Use(1);}else{Use(2);}');source.write_text(old,encoding='utf-8');mp=folder/'manifest.json';mp.write_text(json.dumps(dict(BASE,sources=[{'path':str(source),'input_path':str(source)}])),encoding='utf-8');key='Fixture::Test(System.Int32)->System.Void'
 with Client(mp)as ast,SlotModelClient(mp)as slots:
  source_ir=ast.extract(source,old,key)[0];own=next(m for m in compile_file(source,folder/'own/Fixture.dll')if m['stable_id']==key);compiles+=1;goal_source=folder/'Goal.cs';goal_source.write_text(goal,encoding='utf-8');original=next(m for m in compile_file(goal_source,folder/'goal/Fixture.dll')if m['stable_id']==key);compiles+=1;original_ir=extract_il(original);bindings=bind_control_leaves(original,own,original_ir,source_ir,extract_il(own),project);assert bindings['complete'];bindings.update(source_text_sha256=hashlib.sha256(old.encode('utf-8')).hexdigest(),own_body_source_bound=True,own_body_receipt_sha256=sha(folder/'own/Fixture.dll'));rejected=rewrite_control(original_ir,source_ir,old,bindings,source_path=source,method_id=key,expected_types=plain_types(original),slot_model_client=slots,ast_client=ast,original_method=original,compiled_method=own,compiled_types=plain_types(own));assert rejected['reasons']==['zero-IL-local-declarations-have-no-proved-render-anchor']
 # Named declaration calculus after an independently specified control edit.
 folder=WIP/'NamedSlotComposition';folder.mkdir(exist_ok=True);source=folder/'Fixture.cs';own_body='int i=1;float f=2f;if(n==0){Use(i);}else{Use((int)f);}';goal_body='float f;int i;i=1;f=2f;if(n!=0){Use((int)f);}else{Use(i);}';old=fixture_text('void Test(int n)',own_body);goal=fixture_text('void Test(int n)',goal_body);source.write_text(old,encoding='utf-8');mp=folder/'manifest.json';mp.write_text(json.dumps(dict(BASE,sources=[{'path':str(source),'input_path':str(source)}])),encoding='utf-8');key='Fixture::Test(System.Int32)->System.Void'
 own=next(m for m in compile_file(source,folder/'own/Fixture.dll')if m['stable_id']==key);compiles+=1;goal_source=folder/'Goal.cs';goal_source.write_text(goal,encoding='utf-8');original=next(m for m in compile_file(goal_source,folder/'goal/Fixture.dll')if m['stable_id']==key);compiles+=1;control_variant=fixture_text('void Test(int n)','int i=1;float f=2f;if(n!=0){Use((int)f);}else{Use(i);}')
 with SlotModelClient(mp)as slots:
  old_model=slots.predict(source,old,key);new_model=slots.predict(source,control_variant,key);calculated=calculate_named_slot_variants(control_variant,old,old_model,new_model,original,own,plain_types(original),plain_types(own));approved=[]
  for p in calculated['patches']:
   text=_edit(control_variant,[(p['start'],p['end'],p['after'])]);model=slots.predict(source,text,key)
   if model['complete']and model['predicted_types']==plain_types(original):approved.append(text)
  assert approved;candidate=approved[0];source.write_text(candidate,encoding='utf-8');after=next(m for m in compile_file(source,folder/'candidate/Fixture.dll')if m['stable_id']==key);compiles+=1;equal=body_cells(original)==body_cells(after)and original['locals']==after['locals'];assert equal
 rows.append({'case':'NamedSlotComposition','whole_body_exact':equal,'local_count':len(original['locals']),'real_own_DTO_used_for_name_map':True,'candidate_DTO_fabricated':False,'name_map':calculated['named_slot_map']})
 result={'operation':'independent-source-control-renderer-and-slot-calculus-fixtures','game_compilations':0,'toy_assembly_compilations':compiles,'cases':rows,'negative_cases':{'source_payload_authentication':'rejected','unanchored_zero_IL_declarations':'rejected'},'all_exact':all(r['whole_body_exact']for r in rows),'compiler_sha256':BASE['compiler_sha256']};(WIP/'fixture-result.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8');print(json.dumps(result,indent=2))
if __name__=='__main__':main()
