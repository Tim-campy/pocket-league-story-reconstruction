import json,sys,os,subprocess
from pathlib import Path
sys.path.insert(0,str(Path(__file__).resolve().parent))
from round8_slot_anchor_calculate import ROOT,WIP,BASE,syntax_class
Syntax=syntax_class()
from round7_slot_model import SlotModelClient
from round7_slot_solver import lifetimes,_edit
from compare_kairo import body_cells,sha
BOOT_OLD='''byte[] bytes;for(int i=0;i<3;i++){int size=i+2;bytes=new byte[size];Use(bytes.Length);}'''
BOOT_NEW='''int size;byte[] bytes;for(int i=0;i<3;i++){size=i+2;bytes=new byte[size];Use(bytes.Length);}'''
BALL_OLD='''if(n>0){if(n>1){float a=Rate(n);float b=Rate(n+1);Use(a);Use(b);}else{float a;float b;if(n==1){a=Rate(4);b=Rate(5);}else{a=Rate(6);b=Rate(7);}Use(a);Use(b);}}
bool yes=false;bool no=true;if(n>3){bool inner=false;Use(inner);}Use(yes);Use(no);'''
BALL_NEW='''if(n>0){float unused7;float a;float b;if(n>1){a=Rate(n);b=Rate(n+1);Use(a);Use(b);}else{if(n==1){a=Rate(4);b=Rate(5);}else{a=Rate(6);b=Rate(7);}Use(a);Use(b);}}
bool yes=false;bool no=true;float unused12;if(n>3){bool inner=false;Use(inner);}Use(yes);Use(no);'''
SOURCE='class Fixture{delegate void Work();static int state;static float Rate(int n){return n/2f;}static void Use(float x){state+=(int)x;}static void Use(bool x){state+=x?1:0;}'+''.join('public static void '+name+'(int n){'+body+'}\n' for name,body in [('OriginalBoot',BOOT_NEW),('RecoveredBoot',BOOT_OLD),('OriginalBall',BALL_NEW),('RecoveredBall',BALL_OLD),('UntouchedBall',BALL_OLD)])+'public static void Interleaved(int n){float a=Rate(n);float b=Rate(n+1);Use(a);Use(b);Use(a);}}'
def disjoint(method,slots):
 intervals=sorted((x['first_use'],x['last_use']) for x in lifetimes(method) if x['slot'] in set(slots))
 if len(intervals)!=len(set(slots)) or any(right[0]<=left[1] for left,right in zip(intervals,intervals[1:])):raise ValueError('interleaved_or_missing_local_lifetimes')
 return intervals
def main():
 folder=WIP/'fixture';folder.mkdir(exist_ok=True);source=folder/'Fixture.cs';source.write_text(SOURCE,encoding='utf-8',newline='\n');base=json.loads((BASE/'build-manifest.json').read_text());mp=folder/'manifest.json';mp.write_text(json.dumps(dict(base,sources=[{'path':str(source),'input_path':str(source)}])),encoding='utf-8')
 syntax=Syntax(mp);changed=SOURCE;capture=False
 try:
  with SlotModelClient(mp) as c:
   key='Fixture::RecoveredBoot(System.Int32)->System.Void';model=c.predict(source,changed,key);v=next(v for v in model['variables'] if v['name']=='size');arr=next(v for v in model['variables'] if v['name']=='bytes');changed=syntax.change(source,changed,model,'anchored-hoist',group_starts=[v['start']],insert_at=arr['statement_start'],name='chunkLength')
   key='Fixture::RecoveredBall(System.Int32)->System.Void';model=c.predict(source,changed,key)
   for old,name in [('a','dragX'),('b','dragZ')]:
    vs=[v for v in model['variables'] if v['name']==old];blocks={b['id']:b for b in model['blocks']};chains=[]
    for v in vs:
     chain=[];b=blocks[v['block']]
     while True:
      if b['kind'] in ('explicit','method'):chain.append(b)
      if b['parent']<0:break
      b=blocks[b['parent']]
     chains.append(chain)
    common=next(b for b in chains[0] if all(any(x['id']==b['id'] for x in chain) for chain in chains));insert=common['start']+1
    if old=='b':insert=next(v for v in model['variables'] if v['name']=='dragX')['statement_end']
    changed=syntax.change(source,changed,model,'anchored-hoist',group_starts=[v['start'] for v in vs],insert_at=insert,name=name,prefix_declarations=('float unused7;\n' if old=='a' else ''));model=c.predict(source,changed,key)
   no=next(v for v in model['variables'] if v['name']=='no');changed=_edit(changed,[(no['statement_end'],no['statement_end'],'\nfloat unused12;')]);model=c.predict(source,changed,key)
   captured=SOURCE.replace('float a=Rate(n);float b=Rate(n+1);Use(a);Use(b);','float a=Rate(n);float b=Rate(n+1);Work action=delegate{Use(a);};action();Use(b);')
   cm=c.predict(source,captured,key);start=captured.index('void RecoveredBall(');end=captured.index('void UntouchedBall(',start);group=[];position=start
   while True:
    position=captured.find('float a',position,end)
    if position<0:break
    group.append(len(captured[:position+6].encode('utf-16-le'))//2);position+=7
   # First owner arm's open brace; capture must fail before any proposed edits.
   insertion=len(captured[:captured.index('{',captured.index('if(n>0)',start))+1].encode('utf-16-le'))//2
   try:syntax.change(source,captured,cm,'anchored-hoist',group_starts=group,insert_at=insertion,name='capturedMerged')
   except RuntimeError as e:
    assert 'captured_group_rejected' in str(e),str(e);capture=True
   else:raise AssertionError('capture admitted')
 finally:syntax.close()
 source.write_text(changed,encoding='utf-8',newline='\n');binary=folder/'Fixture.dll';args=[f for f in base['flags'] if not f.startswith('-out:') and f!='-noconfig']+['-out:'+str(binary)]+['-r:'+r['path'] for r in base['references']]+[str(source)];rsp=folder/'compile.rsp';rsp.write_text('\n'.join('"'+a+'"' for a in args),encoding='utf-8');env=os.environ.copy();env.update(base['scoped_environment']);p=subprocess.run([base['runtime'],base['compiler'],'-noconfig','@'+str(rsp)],env=env,cwd=ROOT,capture_output=True);(folder/'compiler.log').write_bytes(p.stdout+p.stderr)
 if p.returncode:raise RuntimeError(p.stdout+p.stderr)
 raw=subprocess.check_output([str(ROOT/'build/tools/ProbeInspector.exe'),str(binary),'--all']);methods={m['stable_id']:m for m in json.loads(raw.decode('utf-8-sig'))['selected_methods']};results={}
 for suffix in ['Boot','Ball']:
  a=methods['Fixture::Original'+suffix+'(System.Int32)->System.Void'];b=methods['Fixture::Recovered'+suffix+'(System.Int32)->System.Void'];exact=body_cells(a)==body_cells(b) and a['locals']==b['locals'] and a['exception_regions']==b['exception_regions'];assert exact,suffix;results[suffix]={'complete_body_exact':exact,'il_bytes':a['header']['code_size'],'local_count':len(a['locals'])}
 untouched=methods['Fixture::UntouchedBall(System.Int32)->System.Void'];safe=disjoint(untouched,[0,2]);unsafe=False
 try:disjoint(methods['Fixture::Interleaved(System.Int32)->System.Void'],[0,1])
 except ValueError:unsafe=True
 assert unsafe and capture
 result={'fixture_game_compilations':0,'fixture_assembly_compilations':1,'cases':results,'capture_rejected':capture,'interleaved_lifetimes_rejected':unsafe,'safe_lifetimes':safe,'model_complete':model['complete'],'fixture_source_sha256':sha(source),'compiler_sha256':base['compiler_sha256']};(WIP/'fixture-result.json').write_text(json.dumps(result,indent=2),encoding='utf-8');print(json.dumps(result))
if __name__=='__main__':main()
