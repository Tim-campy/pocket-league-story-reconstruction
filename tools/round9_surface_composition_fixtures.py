"""Own switch/float/loop controls in historical csc; no game compilation."""
import json,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import differences,sha
from round7_roslyn_for_declaration_fixture import compile_fixture
from round9_surface_composition_source import transform_surface
WORK=ROOT/'build/round9/surface-composition-fixtures';WORK.mkdir(parents=True,exist_ok=True)
HEADER='''using System.Collections.Generic;public class TouchComponent{public int Action;public float X,Y;}public class TouchEvent{public int Value;public bool IsPositive(){return Value>0;}}public class Buffer{public int Calls;private List<TouchEvent>items=new List<TouchEvent>();public void Add(TouchEvent e){Calls++;items.Add(e);}public int Size(){Calls++;return items.Count;}public TouchEvent ElementAt(int i){Calls++;return items[i];}}public class Fixture{public int Count;private Buffer taskBuffer_=new Buffer();private TouchEvent CreateTouchEvent(int kind,TouchComponent c){Count++;return new TouchEvent{Value=kind};}
'''
BEFORE='''public bool Run(TouchComponent c)
{
 int action = c.Action;
 int num7=0;
 TouchEvent touchEvent=null;
 switch (action)
 {
 case 0:
  taskBuffer_.Add(CreateTouchEvent(0,c));
  break;
 case 1:
 {
  float num8 = c.X;
  float num9 = c.Y;
  if (!(num8 * num8 + num9 * num9 < 225f))
  {
   touchEvent = CreateTouchEvent(1, c);
   goto case 2;
  }
  taskBuffer_.Add(CreateTouchEvent(5, c));
 }
 break;
 case 2:
  taskBuffer_.Add(touchEvent);
  break;
 }
 int i;
 for (i = num7; i < taskBuffer_.Size() && taskBuffer_.ElementAt(i).IsPositive(); i++)
 {
 }
 return i==num7;
}'''
EXPECTED='''public bool Run(TouchComponent c)
{
 int action=c.Action;int num7=0;TouchEvent touchEvent=null;
 switch(action){
 case 0:taskBuffer_.Add(CreateTouchEvent(0,c));break;
 case 1:{float num8=c.X;float num9=c.Y;
 if(num8*num8+num9*num9<225f){taskBuffer_.Add(CreateTouchEvent(5,c));}
 else{touchEvent=CreateTouchEvent(1,c);goto case 2;}}
 break;
 case 2:taskBuffer_.Add(touchEvent);break;
 case 3:break;
 }
 int i;for(i=num7;i<taskBuffer_.Size();i++){if(!taskBuffer_.ElementAt(i).IsPositive())break;}
 return i==num7;
}'''
def pick(rows):return next(v for k,v in rows.items() if k.startswith('Fixture::Run('))
def main():
 manifest=json.loads((ROOT/'build/round9/cycles/cycle16/after/KairoLibrary/build-manifest.json').read_text(encoding='utf-8-sig'));after=transform_surface(BEFORE)
 before,_=compile_fixture(WORK/'before',HEADER+BEFORE+'}',manifest);expected,_=compile_fixture(WORK/'expected',HEADER+EXPECTED+'}',manifest);candidate,_=compile_fixture(WORK/'candidate',HEADER+after+'}',manifest)
 a,b,c=pick(before),pick(expected),pick(candidate);assert differences(a,b)[0];assert not differences(b,c)[0],differences(b,c)[0]
 switches=lambda m:[i['operand']['switch'] for i in m['instructions'] if i['opcode']=='switch'];assert len(switches(a)[0])==3 and len(switches(c)[0])==4
 assert any(i['opcode'].startswith('bge.un') for i in c['instructions'])
 negative=[]
 for name,bad in [('enum-unknown-selector',BEFORE.replace('int action =','Other action =')),('shadowed-selector',BEFORE.replace('int num7=0;','int num7=0;int action=1;')),('unknown-float-type',BEFORE.replace('float num8 =','var num8 =')),('nonempty-loop',BEFORE.replace('i++)\n {\n }','i++)\n {Count++;\n }')),('different-goto-owner',BEFORE.replace('goto case 2;','goto case 0;')),('extra-switch',BEFORE.replace('int num7=0;','int num7=0;switch(action){}'))]:
  try:transform_surface(bad)
  except (ValueError,AssertionError):negative.append(name)
  else:raise AssertionError(name)
 report={'operation':'historical-csc-Surface-switch-float-loop-composition-fixture','compiler_sha256':manifest['compiler_sha256'],'producer_sha256':sha(ROOT/'tools/round9_surface_composition_source.py'),'driver_sha256':sha(Path(__file__)),
  'game_compilations':0,'toy_compilations':3,'positive_full_bodies':1,'negative_controls':len(negative),'negatives':negative,'full_body_equal':True,'switch_arity_before':3,'switch_arity_after':4,
  'unordered_float_branch_preserved':True,'existing_index_declaration_preserved':True,'body_before_demonstrably_differs':True,'before':BEFORE,'after':after,'manual_control':EXPECTED,
  'limitations':'One real target proved by bounded scan; no second or third real analog inferred. Actual field types and body oracle still required.'}
 (ROOT/'reports/round9-surface-composition-fixture.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in report.items() if k not in ('before','after','manual_control')}))
if __name__=='__main__':main()
