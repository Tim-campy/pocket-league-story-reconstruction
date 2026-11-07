"""Own Mono controls for touch dispatch composition and empty ticket tests."""
import json,os,subprocess,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import differences,sha
from compiler_probes import MONO,SMCS,UNITY
from round9_touch_ticket_source import extract,transform_touch,transform_billing_empty_ticket
TEMPLATES=ROOT/'tools/fixtures/round9-touch-ticket'
WORK=ROOT/'build/round9/touch-ticket-fixtures'
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
 for label,header,transform in [('touch','public TouchComponent Run(Graphics g, int id, int value)',transform_touch),('cancel','public void Run(object task, string text)',transform_billing_empty_ticket),('provision','public void Run(object task, string text)',transform_billing_empty_ticket)]:
  template=(TEMPLATES/(label+'-before.cs.txt')).read_text(encoding='utf-8-sig');control_source=(TEMPLATES/(label+'-control.cs.txt')).read_text(encoding='utf-8-sig');a,b,before=extract(template,header);_,_,expected=extract(control_source,header);after=transform(before)
  old=compile_own(template,label+'-before');want=compile_own(template[:a]+expected+template[b:],label+'-control');actual=compile_own(template[:a]+after+template[b:],label+'-candidate');assert differences(old,want)[0],label;assert not differences(want,actual)[0],(label,differences(want,actual)[0])
  rows.append({'case':label,'full_body_equal':True,'before_body_different':True,'locals_unchanged':old['locals']==actual['locals'],'before_local_count':len(old['locals']),'after_local_count':len(actual['locals']),'before':before,'after':after,'manual_control':expected,'unordered_opcodes':[i['opcode']for i in actual['instructions']if'.un'in i['opcode']]});cases[label]=before
 touch,cancel,provision=cases['touch'],cases['cancel'],cases['provision'];negatives=[]
 tests=[('touch-null-guard-side-effect',touch.replace('goto dispatchExit;','sink_++;goto dispatchExit;'),transform_touch),('touch-dispatch-label-other',touch.replace('dispatchExit:','dispatchExit2:'),transform_touch),('touch-component-owner-other',touch.replace('int i = 0','int i = 1'),transform_touch),('touch-scale-expression-other',touch.replace('GetScaleRatio() : g.GetScale()','GetScaleRatio() * 2f : g.GetScale()'),transform_touch),('touch-shift-mask-other',touch.replace('id & 0x1F','id & 0x0F',1),transform_touch),('touch-collision-guard-effects',touch.replace('continue;','sink_++;continue;'),transform_touch),('cancel-nonempty-ticket',cancel.replace('else if (!text.Equals("ticket")) { }','else if (!text.Equals("ticket")) { sink_++; }'),transform_billing_empty_ticket),('cancel-extra-empty-else',cancel.replace('else if (!text.Equals("ticket")) { }','else if (!text.Equals("ticket")) { } else { }'),transform_billing_empty_ticket),('provision-after-base-effects',provision.replace('base.Provisioning(task);','base.Provisioning(task);sink_++;'),transform_billing_empty_ticket)]

 for label,bad,transform in tests:
  try:transform(bad)
  except(ValueError,AssertionError):negatives.append(label)
  else:raise AssertionError(label)
 report={'operation':'smcs-touch-ticket-and-local-join-own-fixtures','compiler_sha256':sha(SMCS),'references_sha256':{str(p.relative_to(ROOT)).replace('\\','/'):sha(p)for p in REFS},'producer_sha256':sha(ROOT/'tools/round9_touch_ticket_source.py'),'driver_sha256':sha(Path(__file__)),'game_compilations':0,'toy_compilations':9,'positive_full_bodies':len(rows),'negative_controls':len(negatives),'negatives':negatives,'fixtures':rows,'limitations':'Touch composition covers multiple compensating sites, exact Int32 shift masks, lexical for owners and a reference-null ternary. Empty ticket polarity tested without adding an else. Original target locals/IL/EH still mandatory.'}
 (ROOT/'reports/round9-touch-ticket-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in report.items()if k!='fixtures'}))
if __name__=='__main__':main()

