"""Own historical-csc protected-return/receiver-lifetime controls."""
import json,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import differences,sha
from round7_roslyn_for_declaration_fixture import compile_fixture
from round9_named_receiver_source import transform_notify_receiver,calculate_notify_receiver
WORK=ROOT/'build/round9/named-receiver-fixtures';WORK.mkdir(parents=True,exist_ok=True)
HEADER='''public class NotifyManager{public static NotifyManager Instance;public static int Calls;public static NotifyManager GetInstance(){Calls++;return Instance;}public void UpdateValues(){Calls++;}}public class Fixture{public int Count;
'''
BASE='''public bool Run(bool early)
{
 try
 {
  if(early)return true;
  NotifyManager.GetInstance()?.UpdateValues();
  return false;
 }
 finally{Count++;}
}'''
CONTROL='''public bool Run(bool early){try{if(early)return true;NotifyManager recoveredNotifyManager=NotifyManager.GetInstance();if((object)recoveredNotifyManager!=null)recoveredNotifyManager.UpdateValues();return false;}finally{Count++;}}'''
PREFIX=BASE.replace(' try\n',' int prefix=Count;\n try\n').replace('if(early)return true;','if(early)return prefix>0;').replace('return false;','Count+=prefix;return Count>0;')
PREFIX_CONTROL='''public bool Run(bool early){int prefix=Count;try{if(early)return prefix>0;NotifyManager recoveredNotifyManager=NotifyManager.GetInstance();if((object)recoveredNotifyManager!=null)recoveredNotifyManager.UpdateValues();Count+=prefix;return Count>0;}finally{Count++;}}'''
COLLISION=BASE.replace(' try\n',' int recoveredNotifyManager=Count;\n try\n').replace('return false;','return recoveredNotifyManager>0;')
COLLISION_CONTROL='''public bool Run(bool early){int recoveredNotifyManager=Count;try{if(early)return true;NotifyManager recoveredNotifyManager2=NotifyManager.GetInstance();if((object)recoveredNotifyManager2!=null)recoveredNotifyManager2.UpdateValues();return recoveredNotifyManager>0;}finally{Count++;}}'''
def pick(rows):return next(v for k,v in rows.items() if k.startswith('Fixture::Run('))
def main():
 manifest=json.loads((ROOT/'build/round9/cycles/cycle17/after/KairoLibrary/build-manifest.json').read_text(encoding='utf-8-sig'));rows=[]
 for label,before,expected in [('early-protected-return',BASE,CONTROL),('earlier-root-local',PREFIX,PREFIX_CONTROL),('fresh-name-collision',COLLISION,COLLISION_CONTROL)]:
  after=transform_notify_receiver(before);old,_=compile_fixture(WORK/(label+'-before'),HEADER+before+'}',manifest);control,_=compile_fixture(WORK/(label+'-control'),HEADER+expected+'}',manifest);candidate,_=compile_fixture(WORK/(label+'-candidate'),HEADER+after+'}',manifest)
  a,b,c=pick(old),pick(control),pick(candidate);assert differences(a,b)[0];assert not differences(b,c)[0],(label,differences(b,c)[0])
  assert len(c['locals'])==len(a['locals'])+1
  notify_calls=[i for i in c['instructions'] if i.get('operand') and isinstance(i['operand'],dict) and '::UpdateValues``0(' in i['operand'].get('method','')];assert len(notify_calls)==1 and notify_calls[0]['opcode']=='callvirt'
  rows.append({'case':label,'full_body_equal':True,'extra_named_receiver_local':1,'receiver_callvirt':True,'source_parent_try_unchanged':True,'before':before,'after':after,'manual_control':expected})
 negative=[]
 for label,bad in [('not-in-try',BASE.replace(' try\n',' ')),('child-block',BASE.replace('  NotifyManager.GetInstance()?.UpdateValues();','  {\n  NotifyManager.GetInstance()?.UpdateValues();\n  }')),('duplicate-target',BASE.replace('  return false;','  NotifyManager.GetInstance()?.UpdateValues();\n  return false;')),('target-only-in-comment',BASE.replace('NotifyManager.GetInstance()?.UpdateValues();','/*NotifyManager.GetInstance()?.UpdateValues();*/'))]:
  try:calculate_notify_receiver(bad)
  except (AssertionError,ValueError):negative.append(label)
  else:raise AssertionError(label)
 report={'operation':'historical-csc-named-receiver-before-protected-return-temp-fixtures','compiler_sha256':manifest['compiler_sha256'],'producer_sha256':sha(ROOT/'tools/round9_named_receiver_source.py'),'driver_sha256':sha(Path(__file__)),
  'game_compilations':0,'toy_compilations':9,'positive_full_bodies':len(rows),'negative_controls':len(negative),'negatives':negative,'fixtures':rows,'named_receiver_reserved_before_lazy_return_temp':True,
  'limitations':'One proven game receiver; no hidden later use or artificial ref argument introduced. Actual signature/body still mandatory.'}
 (ROOT/'reports/round9-named-receiver-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in report.items() if k!='fixtures'}))
if __name__=='__main__':main()
