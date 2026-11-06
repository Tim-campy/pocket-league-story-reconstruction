"""Independent smcs body fixture for shared payload + unused typed slot + switch/guard composition."""
import json,os,subprocess,sys
from pathlib import Path
sys.path.insert(0,str(Path(__file__).resolve().parent))
from round8_payload_slot_calculate import ROOT,WIP,BASE,make_syntax,transformer,request
from round7_slot_model import SlotModelClient
from round7_slot_solver import _edit
from round8_slot_anchor_calculate import disjoint
from compare_kairo import body_cells,sha
HEAD_OLD='''if(open){int command=kind;if(command!=-3){switch(command){case -4:Show(4);return true;case -1:Show(1);break;case -2:Show(2);break;default:{int index=command%10+1;Show(index);break;}}return true;}Ime();}'''
HEAD_NEW='''if(open){int command=kind;if(command==-3){Ime();}else{if(command==-4){Show(4);return true;}else if(command==-1){Show(1);}else if(command==-2){Show(2);}else{int index=command%10+1;Show(index);}return true;}}'''
TAIL_OLD='''try{int count=kind;if(count>=0){byte[] data=Read();Parser parser=new Parser();parser.Load(data);}if(count==-1){if(kind==0){byte[] data=Read();Write(0,data);}else{Store store=null;try{store=new Store();for(int j=0;j<2;j++){byte[] data=Read();store.Write(j,data);Parser parser2=new Parser();parser2.Load(data);}}finally{if(store!=null)store.Close();}}}if(count==-2){byte[] data=Read();Write(0,data);data=Read();Write(1,data);}}catch{}return true;'''
TAIL_NEW='''try{int count=kind;byte[] data;if(count>=0){data=Read();Parser parser=new Parser();parser.Load(data);}if(count==-1){if(kind==0){data=Read();Write(0,data);}else{Dialog unused;Store store=null;try{store=new Store();for(int j=0;j<2;j++){data=Read();store.Write(j,data);Parser parser2=new Parser();parser2.Load(data);}}finally{if(store!=null)store.Close();}}}if(count==-2){data=Read();Write(0,data);data=Read();Write(1,data);}}catch{}return true;'''
SOURCE='class Fixture{class Dialog{}class Parser{public void Load(byte[] data){Use(data.Length);}}class Store{public void Write(int i,byte[] data){Use(i+data.Length);}public void Close(){Use(0);}}delegate void Work();static int state;static byte[] Read(){return new byte[2];}static int ReadInt(){return state;}static void Write(int n,byte[] b){Use(n+b.Length);}static void Use(int n){state+=n;}static void Show(int n){Use(n);}static void Ime(){Use(5);}'+''.join('public static bool '+name+'(int kind,bool open){'+body+'}\n' for name,body in [('Original',HEAD_NEW+TAIL_NEW),('Recovered',HEAD_OLD+TAIL_OLD),('Untouched',HEAD_OLD+TAIL_OLD)])+'public static void SelectorOriginal(){int selected=ReadInt();if(selected==0){Show(0);}else if(selected==1){Show(1);}}public static void SelectorRecovered(){switch(ReadInt()){case 0:Show(0);break;case 1:Show(1);break;}}public static void GotoCase(int n){switch(n){case 0:goto case 1;case 1:Show(1);break;}}public static void Interleaved(){int a=1;int b=2;Use(a);Use(b);Use(a);}}'
def main():
 folder=WIP/'fixture';folder.mkdir(exist_ok=True);source=folder/'Fixture.cs';source.write_text(SOURCE,encoding='utf-8',newline='\n');base=json.loads((BASE/'build-manifest.json').read_text(encoding='utf-8'));mp=folder/'manifest.json';mp.write_text(json.dumps(dict(base,sources=[{'path':str(source),'input_path':str(source)}])),encoding='utf-8');merge=make_syntax(WIP/'fixture-merge',ROOT/'tools/round8_coordinate_slot_syntax.cs.txt')(mp);switch=make_syntax(WIP/'fixture-switch',Path(__file__).with_name('round8_payload_switch_syntax.cs.txt'))(mp);process=transformer(mp);key='Fixture::Recovered(System.Int32,System.Boolean)->System.Boolean';changed=SOURCE;rejected={}
 try:
  with SlotModelClient(mp) as c:
   original_model=c.predict(source,SOURCE,'Fixture::Original(System.Int32,System.Boolean)->System.Boolean');model=c.predict(source,changed,key);vs=[v for v in model['variables'] if v['name']=='data'];slots=[v['slot'] for v in vs];insert=changed.index('if(count>=0)',changed.index('bool Recovered('));changed=merge.change(source,changed,model,'anchored-hoist',group_starts=[v['start'] for v in vs],insert_at=len(changed[:insert].encode('utf-16-le'))//2,name='saveDataBytes');model=c.predict(source,changed,key);changed=switch.change(source,changed,model,'switches-to-if');model=c.predict(source,changed,key);store=next(v for v in model['variables'] if v['name']=='store');changed=_edit(changed,[(store['statement_start'],store['statement_start'],'Dialog unused;\n')]);model=c.predict(source,changed,key);changed,guard=request(process,source,changed,'if(command!=-3)','smcs-value-return-guard-reverse');model=c.predict(source,changed,key);(WIP/'fixture/candidate.cs').write_bytes(changed.encode('utf-8'));assert model['complete'] and model['predicted_types']==original_model['predicted_types']
   selector_model=c.predict(source,changed,'Fixture::SelectorRecovered()->System.Void')
   try:switch.change(source,changed,selector_model,'switches-to-if')
   except RuntimeError as e:assert 'switch_selector_not_stable_local' in str(e);rejected['unapproved_effectful_selector']=True
   else:raise AssertionError('effectful selector silently accepted')
   changed=switch.change(source,changed,selector_model,'switches-to-if',allow_call_selector='ReadInt()');selector_final=c.predict(source,changed,'Fixture::SelectorRecovered()->System.Void');assert selector_final['complete']
   goto_model=c.predict(source,changed,'Fixture::GotoCase(System.Int32)->System.Void')
   try:switch.change(source,changed,goto_model,'switches-to-if')
   except RuntimeError:rejected['goto_case']=True
   else:raise AssertionError('goto case accepted')
   captured=SOURCE.replace('byte[] data=Read();Parser parser=new Parser();','byte[] data=Read();Work action=delegate{Use(data.Length);};action();Parser parser=new Parser();');cm=c.predict(source,captured,key);start=captured.index('bool Recovered(');stop=captured.index('bool Untouched(',start);starts=[];at=start
   while True:
    at=captured.find('byte[] data=',at,stop)
    if at<0:break
    starts.append(len(captured[:at+7].encode('utf-16-le'))//2);at+=12
   ins=captured.index('if(count>=0)',start)
   try:merge.change(source,captured,cm,'anchored-hoist',group_starts=starts,insert_at=len(captured[:ins].encode('utf-16-le'))//2,name='capturedData')
   except RuntimeError as e:assert 'captured_group_rejected' in str(e);rejected['captured_payload']=True
   else:raise AssertionError('captured payload accepted')
 finally:merge.close();switch.close();process.stdin.close();process.wait(timeout=10)
 source.write_text(changed,encoding='utf-8',newline='\n');binary=folder/'Fixture.dll';args=[f for f in base['flags'] if not f.startswith('-out:') and f!='-noconfig']+['-out:'+str(binary)]+['-r:'+r['path'] for r in base['references']]+[str(source)];rsp=folder/'compile.rsp';rsp.write_text('\n'.join('"'+a+'"' for a in args),encoding='utf-8');env=os.environ.copy();env.update(base['scoped_environment']);p=subprocess.run([base['runtime'],base['compiler'],'-noconfig','@'+str(rsp)],env=env,cwd=ROOT,capture_output=True);(folder/'compiler.log').write_bytes(p.stdout+p.stderr)
 if p.returncode:raise RuntimeError(p.stdout+p.stderr)
 raw=subprocess.check_output([str(ROOT/'build/tools/ProbeInspector.exe'),str(binary),'--all']);methods={m['stable_id']:m for m in json.loads(raw.decode('utf-8-sig'))['selected_methods']};results={}
 for left,right in [('Fixture::Original(System.Int32,System.Boolean)->System.Boolean',key),('Fixture::SelectorOriginal()->System.Void','Fixture::SelectorRecovered()->System.Void')]:
  a,b=methods[left],methods[right];equal=body_cells(a)==body_cells(b) and a['locals']==b['locals'] and a['exception_regions']==b['exception_regions'];assert equal,(left,a['header'],b['header']);results[right]={'complete_body_exact':equal,'il_bytes':a['header']['code_size'],'locals':len(a['locals']),'exception_regions':len(a['exception_regions'])}
 ranges=disjoint(methods['Fixture::Untouched(System.Int32,System.Boolean)->System.Boolean'],slots)
 try:disjoint(methods['Fixture::Interleaved()->System.Void'],[0,1])
 except ValueError:rejected['interleaving']=True
 else:raise AssertionError('interleaving accepted')
 result={'candidate_game_compilations':0,'fixture_assembly_compilations':1,'cases':results,'negatives':rejected,'safe_payload_lifetimes':ranges,'single_effectful_selector_evaluation_verified_by_exact_body':True,'compiler_sha256':base['compiler_sha256'],'source_sha256':sha(source)};(WIP/'fixture-result.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8');print(json.dumps(result))
if __name__=='__main__':main()
