"""Own Mono controls for Friend joins and paired loop condition/continue."""
import json,os,subprocess,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import differences,sha
from compiler_probes import MONO,SMCS,UNITY
from round9_join_loop_source import extract,transform_friend_update,transform_area_update
TEMPLATES=ROOT/'tools/fixtures/round9-join-loop'
WORK=ROOT/'build/round9/join-loop-fixtures'
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
 for label,header,transform in [('friend','public bool Run()',transform_friend_update),('area','public void Run()',transform_area_update)]:
  template=(TEMPLATES/(label+'-before.cs.txt')).read_text(encoding='utf-8-sig');control_source=(TEMPLATES/(label+'-control.cs.txt')).read_text(encoding='utf-8-sig');a,b,before=extract(template,header);_,_,expected=extract(control_source,header);after=transform(before)
  old=compile_own(template,label+'-before');want=compile_own(template[:a]+expected+template[b:],label+'-control');actual=compile_own(template[:a]+after+template[b:],label+'-candidate');assert differences(old,want)[0],label;assert not differences(want,actual)[0],(label,differences(want,actual)[0])
  cd=json.loads((WORK/(label+'-control')/'methods.json').read_text(encoding='utf-8'));ad=json.loads((WORK/(label+'-candidate')/'methods.json').read_text(encoding='utf-8'));cm={m['stable_id']:m for m in cd['selected_methods']};am={m['stable_id']:m for m in ad['selected_methods']};assert cm.keys()==am.keys();assert not any(differences(cm[k],am[k])[0]for k in cm),label
  rows.append({'case':label,'full_body_equal':True,'before_body_different':True,'all_fixture_methods_equal':True,'fixture_method_count':len(cm),'locals_unchanged':old['locals']==actual['locals'],'before':before,'after':after,'manual_control':expected,'unordered_opcodes':[i['opcode']for i in actual['instructions']if'.un'in i['opcode']]});cases[label]=before
 friend,area=cases['friend'],cases['area'];negatives=[]
 tests=[('friend-mail-arm-effects',friend.replace('if (select_ == 0) { }','if (select_ == 0) { sink_++; }',1),transform_friend_update),('friend-apply-guard-effects',friend.replace('if (friendData_.applyList_.Size() <= 0) { return Pop(); }','if (friendData_.applyList_.Size() <= 0) { sink_++;return Pop(); }'),transform_friend_update),('friend-state-dialog-other',friend.replace('new FriendForm(9)','new FriendForm(8)'),transform_friend_update),('area-for-initial-other',area.replace('int k = 0','int k = 1'),transform_area_update),('area-initial-guard-effects',area.replace('if (!flag) { break; }','if (!flag) { sink_++;break; }'),transform_area_update),('area-hit-break-effects',area.replace('flag = false;','flag = false;sink_++;'),transform_area_update),('area-terminal-state-other',area.replace('state_ != 14','state_ != 15'),transform_area_update),('area-nonterminal-hit',area.replace('flag2 = false;','flag2 = false; ChangeState(9);'),transform_area_update)]

 for label,bad,transform in tests:
  try:transform(bad)
  except(ValueError,AssertionError):negatives.append(label)
  else:raise AssertionError(label)
 report={'operation':'smcs-friend-joins-and-paired-loop-own-fixtures','compiler_sha256':sha(SMCS),'references_sha256':{str(p.relative_to(ROOT)).replace('\\','/'):sha(p)for p in REFS},'producer_sha256':sha(ROOT/'tools/round9_join_loop_source.py'),'driver_sha256':sha(Path(__file__)),'game_compilations':0,'toy_compilations':6,'positive_full_bodies':len(rows),'negative_controls':len(negatives),'negatives':negatives,'fixtures':rows,'limitations':'Paired loop changes tested together, not the rejected isolated continue. Existing lambda body/order and all compiled toy auxiliary bodies compared; full target oracle remains mandatory. No float comparison is complemented.'}
 (ROOT/'reports/round9-join-loop-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in report.items()if k!='fixtures'}))
if __name__=='__main__':main()

