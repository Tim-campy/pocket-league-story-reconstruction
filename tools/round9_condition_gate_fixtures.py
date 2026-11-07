"""Own Mono controls for billing guards and collision phases."""
import json,os,subprocess,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import differences,sha
from compiler_probes import MONO,SMCS,UNITY
from round9_condition_gate_source import extract,transform_billing,transform_collision
TEMPLATES=ROOT/'tools/fixtures/round9-condition-gates'
WORK=ROOT/'build/round9/condition-gate-fixtures'
REFS=[ROOT/'original/extracted/assets/bin/Data/Managed'/n for n in ('mscorlib.dll','System.Core.dll')]
def compile_own(source,label):
 folder=WORK/label;folder.mkdir(parents=True,exist_ok=True);path=folder/'Fixture.cs';path.write_text(source,encoding='utf-8',newline='\n');binary=folder/'Fixture.dll'
 env=os.environ.copy();env['MONO_PATH']=str(UNITY/'Editor/Data/Mono/lib/mono/unity');env['MONO_CFG_DIR']=str(UNITY/'Editor/Data/Mono/etc')
 result=subprocess.run([str(MONO),str(SMCS),'-nologo','-noconfig','-nostdlib','-optimize+','-debug-','-target:library','-out:'+str(binary)]+['-r:'+str(p)for p in REFS]+[str(path)],env=env,capture_output=True,text=True,timeout=45)
 (folder/'compiler.log').write_text(result.stdout+result.stderr,encoding='utf-8');assert result.returncode==0,result.stdout+result.stderr
 process=subprocess.run([str(ROOT/'build/tools/ProbeInspector.exe'),str(binary),'--all'],capture_output=True,text=True,encoding='utf-8',timeout=25,check=True)
 dto=json.loads(process.stdout);(folder/'methods.json').write_text(json.dumps(dto,indent=2),encoding='utf-8');return next(m for m in dto['selected_methods']if m['stable_id'].startswith('Fixture::Run('))
def main():
 WORK.mkdir(parents=True,exist_ok=True);rows=[];cases={}
 for label,header,transform in [('billing','public bool Run(BillingItem[] items)',transform_billing),('collision','public void Run(Player p1, Player p2)',transform_collision)]:
  template=(TEMPLATES/(label+'-fixture-template.cs.txt')).read_text(encoding='utf-8-sig');control_source=(TEMPLATES/(label+'-manual-control-template.cs.txt')).read_text(encoding='utf-8-sig');a,b,before=extract(template,header);_,_,expected=extract(control_source,header);after=transform(before)
  old=compile_own(template,label+'-before');want=compile_own(template[:a]+expected+template[b:],label+'-control');actual=compile_own(template[:a]+after+template[b:],label+'-candidate');assert differences(old,want)[0],label;assert not differences(want,actual)[0],(label,differences(want,actual)[0]);assert old['locals']==actual['locals']
  rows.append({'case':label,'full_body_equal':True,'before_body_different':True,'locals_unchanged':True,'before':before,'after':after,'manual_control':expected,'unordered_opcodes':[i['opcode']for i in actual['instructions']if'.un'in i['opcode']]});cases[label]=before
 billing,collision=cases['billing'],cases['collision'];negatives=[]
 tests=[('billing-null-guard-effects',billing.replace('{ continue; }','{ observed++; continue; }',1),transform_billing),('billing-loop-initial-other',billing.replace('int j = 0','int j = 1'),transform_billing),('billing-tail-nonlocal-return',billing.replace('num5++;','num5++;return false;'),transform_billing),('billing-no-common-return',billing.replace('return true;','return false;'),transform_billing),('collision-axis-limit-other',collision.replace('address_[0] > 1','address_[0] > 2'),transform_collision),('collision-first-return-effects',collision.replace('return;','observed++;return;',1),transform_collision),('collision-phase-kind-other',collision.replace('num != 4))','num != 5))'),transform_collision),('collision-float-op-other',collision.replace('num5 > num4','num5 >= num4'),transform_collision)]
 for label,bad,transform in tests:
  try:transform(bad)
  except(ValueError,AssertionError):negatives.append(label)
  else:raise AssertionError(label)
 report={'operation':'smcs-billing-and-collision-own-fixtures','compiler_sha256':sha(SMCS),'references_sha256':{str(p.relative_to(ROOT)).replace('\\','/'):sha(p)for p in REFS},'producer_sha256':sha(ROOT/'tools/round9_condition_gate_source.py'),'driver_sha256':sha(Path(__file__)),'game_compilations':0,'toy_compilations':6,'positive_full_bodies':len(rows),'negative_controls':len(negatives),'negatives':negatives,'fixtures':rows,'limitations':'System.Core reference added only to own toy compilation for its extension method; game toolchain/references unchanged. Original floating > is preserved, not replaced with >=/<=. CGI field is not tested by this method runner.'}
 (ROOT/'reports/round9-condition-gate-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in report.items()if k!='fixtures'}))
if __name__=='__main__':main()

