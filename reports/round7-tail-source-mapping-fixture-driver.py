"""Real smcs MDB mapping fixture. Compiles only independent toy C#."""
import hashlib,json,os,runpy,subprocess,sys
from pathlib import Path
ROOT=next(p for p in Path(__file__).resolve().parents if (p/'AGENTS.md').is_file())
sys.path.insert(0,str(ROOT/'tools'))
from compiler_probes import MONO,SMCS,UNITY
from test_round7_smcs_rules import CORE,compile_toy,apply
from round4_symbols import inspect_methods,select_point
from compare_kairo import differences
from round7_infinite_tail_guard_hints import infinite_tail_guard_hints
from round7_tail_source_mapping import tail_source_mapping_offset,tail_source_mapping_record
import round7_transform
FIXTURE_DRIVER=ROOT/'reports/round7-infinite-tail-guard-fixture-driver.py'
round7_transform.ensure_helper=runpy.run_path(str(FIXTURE_DRIVER))['ensure_tail_helper']
FOLDER=ROOT/'build/round7/infinite-tail-guard-experiment/mapping'
SOURCE='''public class Fixture {
 public int Pair(bool first,bool second,int limit) {
  int n=0;
  if(first) {
   do {
    n++;
   } while(n<limit);
  }
  if(second) {
   do {
    n+=2;
   } while(n<limit+1);
  }
  return n;
 }
 public int ExpectedPair(bool first,bool second,int limit) {
  int n=0;
  if(first) {
   while(true) {{
    n++;
   } if(n>=limit) {break;}}
  }
  if(second) {
   do {
    n+=2;
   } while(n<limit+1);
  }
  return n;
 }
}
'''


def selected(methods,name):return next(m for m in methods if m['stable_id'].startswith('Fixture::'+name+'('))


def main():
 FOLDER.mkdir(parents=True,exist_ok=True)
 source_path=FOLDER/'Fixture.cs';source_path.write_text(SOURCE,encoding='utf-8',newline='\n')
 binaries={};symbols={}
 env=os.environ.copy();env['MONO_PATH']=str(UNITY/'Editor/Data/Mono/lib/mono/unity');env['MONO_CFG_DIR']=str(UNITY/'Editor/Data/Mono/etc')
 for debug in (False,True):
  binary=FOLDER/('debug' if debug else 'plain')/'Fixture.dll';binary.parent.mkdir(exist_ok=True)
  command=[str(MONO),str(SMCS),'-nologo','-noconfig','-nostdlib','-target:library','-optimize+',
    '-debug+' if debug else '-debug-','-out:'+str(binary),'-r:'+str(CORE),str(source_path)]
  process=subprocess.run(command,env=env,capture_output=True,text=True,timeout=45)
  assert process.returncode==0,process.stdout+process.stderr
  binaries[debug]=binary;symbols[debug]=inspect_methods(binary,None,symbols=debug)
 before=selected(symbols[False]['methods'],'Pair');debug=selected(symbols[True]['methods'],'Pair');original=selected(symbols[False]['methods'],'ExpectedPair')
 assert not differences(before,debug)[0],'debug-plus changed body'
 points=debug['sequence_points'];first_tail=next(i for i in before['instructions'] if i['opcode']=='blt')
 after_offset=next(i['offset'] for i in before['instructions'] if i['offset']>first_tail['offset'])
 original_cond=next(i for i in original['instructions'] if i['opcode']=='blt')
 hints=infinite_tail_guard_hints(original,before,original_cond['offset']+5,after_offset)
 assert hints,hints
 mapping=tail_source_mapping_offset(after_offset,hints)
 old_point=select_point(debug,after_offset);new_point=select_point(debug,mapping['new_offset'])
 assert new_point['start_line']<old_point['start_line'],(old_point,new_point)
 manifest=FOLDER/'manifest.json';manifest.write_text(json.dumps({'sources':[str(source_path)],'references':[str(CORE)]}),encoding='utf-8')
 with round7_transform.TransformClient(manifest) as syntax:
  def query(point):
   return syntax.request(source_path,source=SOURCE,line=point['start_line'],column=max(1,point['start_column']),
    compiler='smcs',round7_cycle_rule='smcs-do-infinite-tail-break',**hints)
  wrong=query(old_point);right=query(new_point)
  wrong_patches=[p for p in wrong.get('patches',[]) if p['rule']=='smcs-do-infinite-tail-break']
  right_patches=[p for p in right.get('patches',[]) if p['rule']=='smcs-do-infinite-tail-break']
  assert len(wrong_patches)==len(right_patches)==1,(wrong,right)
  assert 'n+=2' in wrong_patches[0]['before'],wrong_patches
  assert 'n++;' in right_patches[0]['before'],right_patches
  after=next(m for m in compile_toy(apply(SOURCE,right_patches[0]),'tail-mapping-fixed').values() if m['stable_id'].startswith('Fixture::Pair('))
  facets=('header','locals','instructions','exception_regions','extra_sections','method_attributes','impl_attributes')
  assert not [name for name in facets if original[name]!=after[name]],'correct mapping failed full reference body equality'
 tests=[]
 for label,old,hint,compiler in [
  ('missing-evidence',after_offset,{},'smcs'),
  ('false-evidence',after_offset,dict(hints,original_infinite_tail_guard=False),'smcs'),
  ('future-offset',after_offset,dict(hints,infinite_tail_rebuilt_offset=after_offset+1),'smcs'),
  ('negative-offset',after_offset,dict(hints,infinite_tail_rebuilt_offset=-1),'smcs'),
  ('bool-offset',after_offset,dict(hints,infinite_tail_rebuilt_offset=True),'smcs'),
  ('roslyn',after_offset,hints,'roslyn')]:
  assert tail_source_mapping_offset(old,hint,compiler)['new_offset']==old,label
  tests.append(label)
 assert tail_source_mapping_offset(first_tail['offset'],hints)['new_offset']==first_tail['offset']
 report={'operation':'tail-source-mapping-real-MDB-fixture','target_compilations':0,
  'toy_compilations':3,'debug_plus_minus_full_body_equal':True,'complete_reference_body_equal':True,
  'old_selected_second_do':True,'corrected_selected_first_do':True,'oracle_changed':False,
  'old_mapping':old_point,'corrected_mapping':tail_source_mapping_record(mapping,new_point),
  'negative_controls':tests,'evidence':hints,'wrong_patch':wrong_patches[0],'correct_patch':right_patches[0],
  'smcs_sha256':hashlib.sha256(SMCS.read_bytes()).hexdigest(),
  'fixture_driver_sha256':hashlib.sha256(Path(__file__).read_bytes()).hexdigest(),
  'mapping_module_sha256':hashlib.sha256((ROOT/'tools/round7_tail_source_mapping.py').read_bytes()).hexdigest()}
 (ROOT/'reports/round7-tail-source-mapping-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
 print(json.dumps({k:report[k] for k in ('toy_compilations','old_selected_second_do','corrected_selected_first_do','complete_reference_body_equal')}))


if __name__=='__main__':main()
