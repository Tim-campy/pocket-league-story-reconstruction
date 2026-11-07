"""Own Mono controls for empty state arms and local intermediate joins."""
import json,os,subprocess,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import differences,sha
from compiler_probes import MONO,SMCS,UNITY
from round9_empty_state_source import extract,transform_set_pos,transform_change_state,transform_team_update
TEMPLATES=ROOT/'tools/fixtures/round9-empty-state'
WORK=ROOT/'build/round9/empty-state-fixtures'
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
 for label,header,transform in [('setpos','public void Run()',transform_set_pos),('change','public void Run()',transform_change_state),('team','public void Run()',transform_team_update)]:
  template=(TEMPLATES/(label+'-before.cs.txt')).read_text(encoding='utf-8-sig');control_source=(TEMPLATES/(label+'-control.cs.txt')).read_text(encoding='utf-8-sig');a,b,before=extract(template,header);_,_,expected=extract(control_source,header);after=transform(before)
  old=compile_own(template,label+'-before');want=compile_own(template[:a]+expected+template[b:],label+'-control');actual=compile_own(template[:a]+after+template[b:],label+'-candidate');assert differences(old,want)[0],label;assert not differences(want,actual)[0],(label,differences(want,actual)[0])
  rows.append({'case':label,'full_body_equal':True,'before_body_different':True,'locals_unchanged':old['locals']==actual['locals'],'before_local_count':len(old['locals']),'after_local_count':len(actual['locals']),'before':before,'after':after,'manual_control':expected,'unordered_opcodes':[i['opcode']for i in actual['instructions']if'.un'in i['opcode']]});cases[label]=before
 setpos,change,team=cases['setpos'],cases['change'],cases['team'];negatives=[]
 tests=[('setpos-other-case',setpos.replace('case 2:','case 3:'),transform_set_pos),('setpos-other-phase',setpos.replace('tArea_.tMode_ != 3','tArea_.tMode_ != 4'),transform_set_pos),('setpos-other-negation',setpos.replace('0f - num13','1f - num13'),transform_set_pos),('change-nonempty-last-arm',change.replace('else if ((tArea_.tMode_ != 3)) { }','else if ((tArea_.tMode_ != 3)) { state_++; }'),transform_change_state),('change-tenant-continue-effects',change.replace('continue;','state_++;continue;',1),transform_change_state),('change-wrong-loop-owner',change.replace('int num5 = 0','int num5 = 1'),transform_change_state),('team-nonempty-else',team.replace('else { }','else { team_id_++; }'),transform_team_update),('team-other-state-complement',team.replace('1 - team_id_','2 - team_id_'),transform_team_update),('team-other-final-state',team.replace('udat_.gm_.state_ == 8','udat_.gm_.state_ == 9'),transform_team_update)]

 for label,bad,transform in tests:
  try:transform(bad)
  except(ValueError,AssertionError):negatives.append(label)
  else:raise AssertionError(label)
 report={'operation':'smcs-empty-state-and-local-join-own-fixtures','compiler_sha256':sha(SMCS),'references_sha256':{str(p.relative_to(ROOT)).replace('\\','/'):sha(p)for p in REFS},'producer_sha256':sha(ROOT/'tools/round9_empty_state_source.py'),'driver_sha256':sha(Path(__file__)),'game_compilations':0,'toy_compilations':9,'positive_full_bodies':len(rows),'negative_controls':len(negatives),'negatives':negatives,'fixtures':rows,'limitations':'Own source controls; removing a switch cache can change local inventory. All candidate facets must match manual control, not the old program. Continue and intermediate if join have distinct proven owners. No floating relational operator is complemented.'}
 (ROOT/'reports/round9-empty-state-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in report.items()if k!='fixtures'}))
if __name__=='__main__':main()

