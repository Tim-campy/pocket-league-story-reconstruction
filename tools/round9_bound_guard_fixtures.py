"""Own Mono controls for signed bounds and two compensating scoped joins."""
import json,os,subprocess,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import differences,sha
from compiler_probes import MONO,SMCS,UNITY
from round9_bound_guard_source import extract,transform_coat_line,transform_effect,transform_lineup_composed
TEMPLATES=ROOT/'tools/fixtures/round9-bound-guards'
WORK=ROOT/'build/round9/bound-guard-fixtures'
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
 for label,header,transform in [('map','public void Run(int address_x, int address_y)',transform_coat_line),('effect','public void Run()',transform_effect),('lineup','public bool Run()',transform_lineup_composed)]:
  template=(TEMPLATES/(label+'-before.cs.txt')).read_text(encoding='utf-8-sig');control_source=(TEMPLATES/(label+'-control.cs.txt')).read_text(encoding='utf-8-sig');a,b,before=extract(template,header);_,_,expected=extract(control_source,header);after=transform(before)
  old=compile_own(template,label+'-before');want=compile_own(template[:a]+expected+template[b:],label+'-control');actual=compile_own(template[:a]+after+template[b:],label+'-candidate');assert differences(old,want)[0],label;assert not differences(want,actual)[0],(label,differences(want,actual)[0]);assert old['locals']==actual['locals']
  rows.append({'case':label,'full_body_equal':True,'before_body_different':True,'locals_unchanged':True,'before':before,'after':after,'manual_control':expected,'unordered_opcodes':[i['opcode']for i in actual['instructions']if'.un'in i['opcode']]});cases[label]=before
 map_,effect,lineup=cases['map'],cases['effect'],cases['lineup'];negatives=[]
 tests=[('map-side-effects',map_.replace('return;','observed++;return;',1),transform_coat_line),('map-limit-other',map_.replace('num2 < -1','num2 < -2'),transform_coat_line),('map-condition-extra',map_.replace('num < 0','num < 0 || observed == 7'),transform_coat_line),('effect-loop-owner-other',effect.replace('int i = 0','int i = 1'),transform_effect),('effect-frame-operator-other',effect.replace('array[1] >= 0','array[1] > 0'),transform_effect),('effect-default-guard-side-effects',effect.replace('{ continue; }','{ observed++;continue; }',1),transform_effect),('lineup-external-else-has-effect',lineup.replace('else { }','else { observed++; }'),transform_lineup_composed),('lineup-no-external-else',lineup.replace('else { }',''),transform_lineup_composed),('lineup-inner-return-effect',lineup.replace('return Pop();','observed++;return Pop();'),transform_lineup_composed)]
 for label,bad,transform in tests:
  try:transform(bad)
  except(ValueError,AssertionError):negatives.append(label)
  else:raise AssertionError(label)
 report={'operation':'smcs-signed-bounds-and-compensating-joins-own-fixtures','compiler_sha256':sha(SMCS),'references_sha256':{str(p.relative_to(ROOT)).replace('\\','/'):sha(p)for p in REFS},'producer_sha256':sha(ROOT/'tools/round9_bound_guard_source.py'),'driver_sha256':sha(Path(__file__)),'game_compilations':0,'toy_compilations':9,'positive_full_bodies':len(rows),'negative_controls':len(negatives),'negatives':negatives,'fixtures':rows,'limitations':'Own independent controls only. Integer signed bounds retain operators. Effect continue belongs to for-i; standalone child local block retained. Lineup requires both edits and explicitly rejects the previous inner-only form.'}
 (ROOT/'reports/round9-bound-guard-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in report.items()if k!='fixtures'}))
if __name__=='__main__':main()

