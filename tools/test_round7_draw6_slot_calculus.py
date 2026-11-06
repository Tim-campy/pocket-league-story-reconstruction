"""Toy-only exact emission and unsafe-coalescence rejection checks."""
import inspect,json,os,subprocess,sys
from pathlib import Path
ROOT=next(p for p in Path(__file__).resolve().parents if (p/'tools/round7_slot_model.py').exists())
sys.path.insert(0,str(ROOT/'tools'))
import round7_coalesce_calculate as frozen
from round7_slot_model import SlotModelClient
from round7_slot_solver import lifetimes
from compare_kairo import body_cells,sha
FOLDER=ROOT/'build/round7/draw6-slot-calculus';FOLDER.mkdir(parents=True,exist_ok=True)
ORIGINAL='''int number=mode;if(number==0){Use(0);}else if(number==1){Use(1);}
int selected=GetJoin();if(selected==0){Use(2);}else if(selected==1){Use(3);}
if(include){int shared,height;shared=height=0;int span=30;
if(mode==0){shared=span/3;Use(shared);}
if(mode==1){for(int k=0;k<2;k++){shared=Scale(k);Use(shared);}}
if(mode==2){shared=20;height=20;Use(shared+height);}}'''
RECOVERED='''int number=mode;switch(number){case 0:Use(0);break;case 1:Use(1);break;}
switch(GetJoin()){case 0:Use(2);break;case 1:Use(3);break;}
if(include){int height=0;int span=30;
if(mode==0){int width2=span/3;Use(width2);}
if(mode==1){for(int k=0;k<2;k++){int width2=Scale(k);Use(width2);}}
if(mode==2){int width2=20;height=20;Use(width2+height);}}'''
SOURCE='''class Fixture {
delegate void Work();static int state;static void Use(int x){state+=x;}static int GetJoin(){return state&1;}static int Scale(int x){return x+1;}
'''+''.join('public static void '+name+'(int mode,bool include){'+body+'}\n' for name,body in [('Original',ORIGINAL),('Recovered',RECOVERED),('Untouched',RECOVERED)])+'''
public static void Interleaved(){int a=1;int b=2;Use(a);Use(b);Use(a);}
}'''


def main():
 folder=FOLDER/'fixture';folder.mkdir(exist_ok=True)
 base=json.loads((ROOT/'build/round7/cycles/cycle11/baseline/Assembly-CSharp/build-manifest.json').read_text(encoding='utf-8-sig'))
 source=folder/'Fixture.cs';source.write_text(SOURCE,encoding='utf-8',newline='\n')
 mp=folder/'manifest.json';mp.write_text(json.dumps(dict(base,sources=[{'path':str(source),'input_path':str(source)}])),encoding='utf-8')
 namespace=dict(vars(frozen));namespace['FOLDER']=FOLDER
 exec(inspect.getsource(frozen.Syntax).replace("(ROOT/'tools/round7_coalesce_syntax.cs')","(ROOT/'tools/round7_draw6_slot_syntax.cs.txt')"),namespace)
 syntax=namespace['Syntax'](mp)
 key='Fixture::Recovered(System.Int32,System.Boolean)->System.Void'
 try:
  with SlotModelClient(mp) as model:
   current=model.predict(source,SOURCE,key)
   widths=[v for v in current['variables'] if v['name']=='width2'];height=next(v for v in current['variables'] if v['name']=='height')
   changed=syntax.change(source,SOURCE,current,'merge-width-chain',group_starts=[v['start'] for v in widths],height_start=height['start'])
   prediction=model.predict(source,changed,key)
   changed=syntax.change(source,changed,prediction,'switches-to-if',allow_call_selector='GetJoin()')
   final=model.predict(source,changed,key)
   captured=SOURCE.replace('int width2=span/3;Use(width2);','int width2=span/3;Work action=delegate{Use(width2);};action();',1)
   captured_model=model.predict(source,captured,key)
   # Locate the three declarators in this one method, not the Untouched copy.
   start=captured.index('void Recovered(');end=captured.index('void Untouched(',start)
   starts=[];position=start
   while True:
    position=captured.find('int width2=',position,end)
    if position<0:break
    starts.append(len(captured[:position+4].encode('utf-16-le'))//2);position+=11
   captured_height=next(v for v in captured_model['variables'] if v['name']=='height')
   try:syntax.change(source,captured,captured_model,'merge-width-chain',group_starts=starts,height_start=captured_height['start'])
   except RuntimeError as error:
    assert 'captured_width_group_rejected' in str(error);capture_rejected=True
   else:raise AssertionError('Captured coalescence authorized')
   untouched_model=model.predict(source,SOURCE,'Fixture::Untouched(System.Int32,System.Boolean)->System.Void')
   interleaved_model=model.predict(source,SOURCE,'Fixture::Interleaved()->System.Void')
 finally:syntax.close()
 source.write_text(changed,encoding='utf-8',newline='\n');binary=folder/'Fixture.dll'
 args=[f for f in base['flags'] if not f.startswith('-out:') and f!='-noconfig']+['-out:'+str(binary)]+['-r:'+r['path'] for r in base['references']]+[str(source)]
 rsp=folder/'compile.rsp';rsp.write_text('\n'.join('"'+a+'"' for a in args),encoding='utf-8')
 env=os.environ.copy();env.update(base['scoped_environment'])
 p=subprocess.run([base['runtime'],base['compiler'],'-noconfig','@'+str(rsp)],cwd=ROOT,env=env,capture_output=True)
 (folder/'compiler.log').write_bytes(p.stdout+p.stderr)
 if p.returncode:raise RuntimeError(p.stdout+p.stderr)
 raw=subprocess.run([str(ROOT/'build/tools/ProbeInspector.exe'),str(binary),'--all'],capture_output=True,check=True)
 methods={m['stable_id']:m for m in json.loads(raw.stdout.decode('utf-8-sig'))['selected_methods']}
 a,b=methods['Fixture::Original(System.Int32,System.Boolean)->System.Void'],methods[key]
 equal=body_cells(a)==body_cells(b) and a['locals']==b['locals'] and a['exception_regions']==b['exception_regions']
 assert equal
 widths_slots={v['slot'] for v in untouched_model['variables'] if v['name']=='width2'}
 widths_ranges=sorted((v['first_use'],v['last_use']) for v in lifetimes(methods['Fixture::Untouched(System.Int32,System.Boolean)->System.Void']) if v['slot'] in widths_slots)
 safe=all(right[0]>left[1] for left,right in zip(widths_ranges,widths_ranges[1:]));assert safe
 intervals=lifetimes(methods['Fixture::Interleaved()->System.Void'])
 intersect=not(intervals[0]['last_use']<intervals[1]['first_use'] or intervals[1]['last_use']<intervals[0]['first_use']);assert intersect
 result={'operation':'independent-smcs-draw6-selector-width-fixture','candidate_game_compilations':0,'fixture_assembly_compilations':1,
         'combined_complete_body_exact':equal,'capture_rejected_source_only':capture_rejected,'interleaving_rejected_using_real_IL':intersect,
         'safe_width_intervals':widths_ranges,'unsafe_intervals':intervals,'model_complete':final['complete'],
         'model_local_count':len(final['predicted_types']),'original_local_count':len(a['locals']),
         'source_sha256':sha(source),'compiler_sha256':base['compiler_sha256'],
         'fixture_identity_note':'Different fixture method names; complete canonical body, local signature and EH compared without affecting game states.'}
 assert final['complete'] and len(final['predicted_types'])==len(a['locals'])
 (ROOT/'reports/round7-draw6-calculus-fixture.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8')
 print(json.dumps(result))


if __name__=='__main__':main()
