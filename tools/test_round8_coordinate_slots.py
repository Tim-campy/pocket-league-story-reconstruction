"""Independent toy compiler fixtures for coordinate coalescence and first-store sinks."""
import json,sys,os,subprocess
from pathlib import Path
sys.path.insert(0,str(Path(__file__).resolve().parent))
from round8_coordinate_slot_calculate import ROOT,WIP,BASE,Syntax
from round7_slot_model import SlotModelClient
from round8_slot_anchor_calculate import disjoint
from compare_kairo import body_cells,sha
OLD='''int n=10;Player p;int mode;int count=0;for(int i=0;i<2;i++){int x=i*n;int y=count*n;Use(x+y);}
if(flag){int x=n+1;int y=n+2;Use(x+y);}p=Get();Use(p);mode=-1;Use(mode);
for(int j=0;j<2;j++){int x=Scale(j);int y=Scale(j+1);Use(x+y);}'''
NEW='''int n=10;int x,y;int count=0;for(int i=0;i<2;i++){x=i*n;y=count*n;Use(x+y);}
if(flag){x=n+1;y=n+2;Use(x+y);}Player p=Get();Use(p);int mode=-1;Use(mode);
for(int j=0;j<2;j++){x=Scale(j);y=Scale(j+1);Use(x+y);}'''
OLD_BALL='''int n=1;bool active=flag;for(int i=0;i<2;i++){if(active){Use(i);}else{if(i>0){float x=Scale(i);float y=Scale(i+1);Use(x+y);}else{float x;float y;if(flag){x=Scale(3);y=Scale(4);}else{x=Scale(5);y=Scale(6);}Use(x+y);}}}'''
NEW_BALL='''int n=1;float x,y;bool active=flag;for(int i=0;i<2;i++){if(active){Use(i);}else{if(i>0){x=Scale(i);y=Scale(i+1);Use(x+y);}else{if(flag){x=Scale(3);y=Scale(4);}else{x=Scale(5);y=Scale(6);}Use(x+y);}}}'''
SOURCE='class Fixture{class Player{}delegate void Work();static int state;static Player Get(){return null;}static int Scale(int x){return x+1;}static void Use(float n){state+=(int)n;}static void Use(Player p){state++;}'+''.join('public static void '+name+'(bool flag){'+body+'}\n' for name,body in [('Original',NEW),('Recovered',OLD),('OriginalBall',NEW_BALL),('RecoveredBall',OLD_BALL),('Untouched',OLD)])+'public static void Unsafe(){Player p;Use(p=Get());Use(p);}public static void Interleaved(){int x=1;int y=2;Use(x);Use(y);Use(x);}}'
def main():
 folder=WIP/'fixture';folder.mkdir(exist_ok=True);source=folder/'Fixture.cs';source.write_text(SOURCE,encoding='utf-8',newline='\n');base=json.loads((BASE/'build-manifest.json').read_text(encoding='utf-8'));mp=folder/'manifest.json';mp.write_text(json.dumps(dict(base,sources=[{'path':str(source),'input_path':str(source)}])),encoding='utf-8');syntax=Syntax(mp);changed=SOURCE;negative=False;capture=False
 try:
  with SlotModelClient(mp) as c:
   key='Fixture::Recovered(System.Boolean)->System.Void';model=c.predict(source,changed,key)
   for name in ['p','mode']:
    v=next(v for v in model['variables'] if v['name']==name);changed=syntax.change(source,changed,model,'sink-first-assignment',group_starts=[v['start']]);model=c.predict(source,changed,key)
   for index,name in enumerate(['x','y']):
    group=[v for v in model['variables'] if v['name']==name];before=next(v for v in model['variables'] if v['name']=='count');insertion=before['statement_start'] if index==0 else next(v for v in model['variables'] if v['name']=='coordinateX')['statement_end'];changed=syntax.change(source,changed,model,'anchored-hoist',group_starts=[v['start'] for v in group],insert_at=insertion,name=['coordinateX','coordinateY'][index]);model=c.predict(source,changed,key)
   key='Fixture::RecoveredBall(System.Boolean)->System.Void';model=c.predict(source,changed,key)
   for index,name in enumerate(['x','y']):
    group=[v for v in model['variables'] if v['name']==name];before=next(v for v in model['variables'] if v['name']=='active');insertion=before['statement_start'] if index==0 else next(v for v in model['variables'] if v['name']=='coordinateX')['statement_end'];changed=syntax.change(source,changed,model,'anchored-hoist',group_starts=[v['start'] for v in group],insert_at=insertion,name=['coordinateX','coordinateY'][index]);model=c.predict(source,changed,key)
   unsafe=c.predict(source,changed,'Fixture::Unsafe()->System.Void');p=next(v for v in unsafe['variables'] if v['name']=='p')
   try:syntax.change(source,changed,unsafe,'sink-first-assignment',group_starts=[p['start']])
   except RuntimeError as e:assert 'sink_first_use_not_simple_assignment_statement' in str(e);negative=True
   else:raise AssertionError('nested assignment sink permitted')
   captured=changed.replace('Player p=Get();Use(p);','Player p;Work action=delegate{Use(p);};p=Get();action();',1);cm=c.predict(source,captured,'Fixture::Original(System.Boolean)->System.Void');start=captured.index('Player p;',captured.index('void Original('))+7;vstart=len(captured[:start].encode('utf-16-le'))//2
   try:syntax.change(source,captured,cm,'sink-first-assignment',group_starts=[vstart])
   except RuntimeError as e:assert 'captured_group_rejected' in str(e);capture=True
   else:raise AssertionError('captured sink permitted')
 finally:syntax.close()
 source.write_text(changed,encoding='utf-8',newline='\n');binary=folder/'Fixture.dll';args=[f for f in base['flags'] if not f.startswith('-out:') and f!='-noconfig']+['-out:'+str(binary)]+['-r:'+r['path'] for r in base['references']]+[str(source)];rsp=folder/'compile.rsp';rsp.write_text('\n'.join('"'+a+'"' for a in args),encoding='utf-8');env=os.environ.copy();env.update(base['scoped_environment']);p=subprocess.run([base['runtime'],base['compiler'],'-noconfig','@'+str(rsp)],env=env,cwd=ROOT,capture_output=True);(folder/'compiler.log').write_bytes(p.stdout+p.stderr)
 if p.returncode:raise RuntimeError(p.stdout+p.stderr)
 raw=subprocess.check_output([str(ROOT/'build/tools/ProbeInspector.exe'),str(binary),'--all']);methods={m['stable_id']:m for m in json.loads(raw.decode('utf-8-sig'))['selected_methods']};results={}
 for suffix in ['', 'Ball']:
  a=methods['Fixture::Original'+suffix+'(System.Boolean)->System.Void'];b=methods['Fixture::Recovered'+suffix+'(System.Boolean)->System.Void'];equal=body_cells(a)==body_cells(b) and a['locals']==b['locals'] and a['exception_regions']==b['exception_regions'];assert equal,suffix;results[suffix or 'Coordinates']={'complete_body_exact':equal,'il_bytes':a['header']['code_size'],'locals':len(a['locals'])}
 try:disjoint(methods['Fixture::Interleaved()->System.Void'],[0,1])
 except ValueError:interleaving=True
 else:raise AssertionError('interleaved slots permitted')
 assert negative and capture
 result={'candidate_game_compilations':0,'fixture_assembly_compilations':1,'cases':results,'sink_nested_assignment_rejected':negative,'captured_local_rejected':capture,'interleaving_rejected':interleaving,'compiler_sha256':base['compiler_sha256'],'fixture_source_sha256':sha(source)};(WIP/'fixture-result.json').write_text(json.dumps(result,indent=2),encoding='utf-8');print(json.dumps(result))
if __name__=='__main__':main()
