"""Ignored experiment: compose semantic source-only coalescence proposals."""
import hashlib,json,shutil,subprocess,sys
from pathlib import Path
ROOT=next(p for p in Path(__file__).resolve().parents if (p/'tools/round7_slot_model.py').exists())
sys.path.insert(0,str(ROOT/'tools'))
from compiler_probes import ROSLYN
from compare_kairo import sha
from round7_slot_model import SlotModelClient,ensure_helper
from round7_slot_solver import correspondences,lifetimes,_patch
BASE=ROOT/'build/round7/cycles/cycle02/baseline/Assembly-CSharp'
FOLDER=ROOT/'build/round7/coalesce-experiment'
TARGETS=[('form.SubForm::_draw3(kairo.unity.ui.Graphics)->System.Void','src/Assembly-CSharp/form/SubForm.cs'),('form.SubForm::_draw5(kairo.unity.ui.Graphics)->System.Void','src/Assembly-CSharp/form/SubForm.cs'),('game.UserData::Draw_edit2(kairo.unity.ui.Graphics,System.Boolean)->System.Void','src/Assembly-CSharp/game/UserData.cs')]


def read(p):return json.loads(Path(p).read_text(encoding='utf-8-sig'))


def rebase_unique_patch(source,patch):
 """Rebase a frozen whole declaration only when its exact text is unique.

 Offsets stay UTF-16 and CRLF is preserved. A fresh model prediction and the
 complete verifier are still required by the caller before adoption.
 """
 raw=source.encode('utf-16-le');needle=patch['before'].encode('utf-16-le')
 positions=[];at=0
 while True:
  at=raw.find(needle,at)
  if at<0:break
  if at%2==0:positions.append(at)
  at+=2
 if len(positions)!=1:raise ValueError('Frozen declaration is missing or ambiguous')
 start=positions[0]//2
 return dict(patch,start=start,end=start+len(needle)//2)


class Syntax:
 def __init__(self,manifest):
  command=ensure_helper();previous=Path(command[-1]);runtime=Path(command[0]).parent
  text=(ROOT/'tools/Round7SlotModel.cs').read_text(encoding='utf-8-sig')
  inject=(ROOT/'tools/round7_coalesce_syntax.cs').read_text(encoding='utf-8-sig')
  text=text.replace('    static object Respond(JsonElement input){',inject+'\n    static object Respond(JsonElement input){\n        if(input.TryGetProperty("coalesce_mode",out var cm))return CoalesceResponse(input);')
  source=FOLDER/'CoalesceSourceOnly.cs';source.write_text(text,encoding='utf-8',newline='\n')
  binary=FOLDER/'CoalesceSourceOnly.dll';framework=runtime/'shared/Microsoft.NETCore.App/10.0.10'
  refs=[framework/(n+'.dll') for n in ['System.Private.CoreLib','System.Runtime','System.Console','System.Linq','System.Collections','System.Text.Json','System.Reflection.Metadata','System.Collections.Immutable','System.Memory','netstandard']]
  refs += [previous.parent/(n+'.dll') for n in ['Microsoft.CodeAnalysis','Microsoft.CodeAnalysis.CSharp']]
  args=['/nologo','/nostdlib+','/target:exe','/out:'+str(binary)]+['/r:'+str(p) for p in refs]+[str(source)]
  rsp=FOLDER/'syntax.rsp';rsp.write_text('\n'.join('"'+a+'"' for a in args),encoding='utf-8')
  subprocess.run([str(ROSLYN),'/noconfig','@'+str(rsp)],check=True)
  for p in refs[-2:]:shutil.copyfile(p,FOLDER/p.name)
  shutil.copyfile(previous.with_suffix('.runtimeconfig.json'),binary.with_suffix('.runtimeconfig.json'))
  self.process=subprocess.Popen([str(runtime/'dotnet.exe'),str(binary),str(manifest)],stdin=subprocess.PIPE,stdout=subprocess.PIPE,stderr=subprocess.PIPE,text=True,encoding='utf-8')
 def change(self,path,source,model,mode,**options):
  request={'path':str(ROOT/path),'source':source,'body_start':model['body_start'],'coalesce_mode':mode,**options}
  self.process.stdin.write(json.dumps(request)+'\n');self.process.stdin.flush()
  line=self.process.stdout.readline()
  if not line:raise RuntimeError(self.process.stderr.read())
  result=json.loads(line)
  if 'error' in result:raise RuntimeError(result)
  return result['source']
 def close(self):self.process.stdin.close();self.process.wait(timeout=10)


def main():
 FOLDER.mkdir(parents=True,exist_ok=True)
 manifest=read(BASE/'build-manifest.json')
 for item in manifest['sources']:item['input_path']=str(BASE/item['path'])
 mp=FOLDER/'manifest.json';mp.write_text(json.dumps(manifest),encoding='utf-8')
 original={m['stable_id']:m for m in read(BASE/'original-methods.json')['selected_methods']}
 rebuilt={m['stable_id']:m for m in read(BASE/'rebuilt-methods.json')['selected_methods']}
 types=read(BASE/'plain-local-types.json');current_types=read(BASE/'plain-local-types-rebuilt.json')
 patches=[];diagnostics=[]
 syntax=Syntax(mp)
 try:
  with SlotModelClient(mp) as client:
   for identity,logical in TARGETS:
    source=(BASE/logical).read_bytes().decode('utf-8-sig')
    initial=client.predict(ROOT/logical,source,identity);mapping,votes=correspondences(original[identity],rebuilt[identity],types[identity],current_types[identity])
    selected=[v for v in initial['variables'] if v['kind']=='declared' and mapping.get(v['slot'])==0]
    slots={v['slot'] for v in selected};ranges=sorted((v['first_use'],v['last_use']) for v in lifetimes(rebuilt[identity]) if v['slot'] in slots)
    if any(b[0]<=a[1] for a,b in zip(ranges,ranges[1:])):raise RuntimeError('interleaved_group_rejected:'+identity)
    roots=['$coalesced']+[next(v['name'] for v in initial['variables'] if v['kind']=='declared' and mapping.get(v['slot'])==slot) for slot in [1,2,3]]
    candidate=syntax.change(logical,source,initial,'merge-root-chain',group_starts=[v['start'] for v in selected],root_names_by_slot=roots)
    model=client.predict(ROOT/logical,candidate,identity)
    stages=[{'stage':'merge-root-chain','count':len(model['predicted_types']),'complete':model['complete'],'types_equal':model['predicted_types']==types[identity]}]
    if '::_draw3(' in identity:
     # Original GetMatchInfoJoin call result is demonstrably stored in slot20.
     candidate=syntax.change(logical,candidate,model,'tested-local',expression='udat_.GetMatchInfoJoin()')
     model=client.predict(ROOT/logical,candidate,identity)
     stages.append({'stage':'tested-local-original20','count':len(model['predicted_types']),'complete':model['complete'],'types_equal':model['predicted_types']==types[identity]})
     if any(i['opcode']=='switch' for i in original[identity]['instructions']):raise RuntimeError('original_switch_exists')
     candidate=syntax.change(logical,candidate,model,'switches-to-if',allow_call_selector='udat_.GetMatchInfoJoin()')
     model=client.predict(ROOT/logical,candidate,identity)
     stages.append({'stage':'stable-switches-to-if-and-original-call-local61','count':len(model['predicted_types']),'complete':model['complete'],'types_equal':model['predicted_types']==types[identity]})
    if '::_draw5(' in identity:
     if any(i['opcode']=='switch' for i in original[identity]['instructions']):raise RuntimeError('original_switch_exists')
     candidate=syntax.change(logical,candidate,model,'switches-to-if')
     model=client.predict(ROOT/logical,candidate,identity)
     stages.append({'stage':'two-stable-switches-to-if-no-original-switch','count':len(model['predicted_types']),'complete':model['complete'],'types_equal':model['predicted_types']==types[identity]})
    if '::Draw_edit2(' in identity:
     used={l['slot'] for l in lifetimes(original[identity])};unused=[slot for slot in range(len(types[identity])) if slot not in used]
     for slot in unused:
      choices=[v for v in model['variables'] if v['kind']=='declared' and v['slot']>=slot and v['type']==types[identity][slot]]
      chosen=min(choices,key=lambda v:v['slot'])
      candidate=syntax.change(logical,candidate,model,'reserve-variable-before',variable_start=chosen['start'],original_slot=slot,type_fullname=types[identity][slot])
      model=client.predict(ROOT/logical,candidate,identity)
      reserved=next(v for v in model['variables'] if v['name']=='unusedOriginalSlot'+str(slot))
      if reserved['slot']!=slot:raise RuntimeError('unused_slot_position_not_proved')
      stages.append({'stage':'unused-original-slot-'+str(slot),'count':len(model['predicted_types']),'complete':model['complete'],'types_equal':model['predicted_types']==types[identity],'inserted_before_variable':chosen['name'],'before_slot':chosen['slot']})
    equal=model['complete'] and model['predicted_types']==types[identity]
    row={'method_id':identity,'logical_path':logical,'stages':stages,'model_complete':model['complete'],'predicted_types':model['predicted_types'],'expected_types':types[identity],'signature_equal':equal,'unsupported_reasons':model.get('unsupported_reasons',[]),'special_variables':[{'name':v['name'],'slot':v['slot'],'type':v['type']} for v in model['variables'] if v['name'].startswith(('unusedOriginalSlot','coalescedValue','testedValue','switchValue'))]}
    diagnostics.append(row)
    (FOLDER/(identity.split('::')[1].split('(')[0]+'.candidate.cs')).write_bytes(candidate.encode('utf-8'))
    print(json.dumps({k:v for k,v in row.items() if k not in {'predicted_types','expected_types'}}),flush=True)
    if equal:
     patch=_patch(source,initial,candidate,'calculated-original-local-coalescence')
     patches.append(dict(patch,method_id=identity,logical_path=logical,baseline_source_sha256=sha(BASE/logical),source_only=True,candidate_compilations=0,model_source_sha256=sha(ROOT/'tools/Round7SlotModel.cs'),client_sha256=sha(ROOT/'tools/round7_slot_model.py'),predicted_types=model['predicted_types'],complete=True,stages=stages))
 finally:syntax.close()
 result={'operation':'source-only-composed-coalescence-proposals','candidate_game_compilations':0,'frozen_baseline':str(BASE.relative_to(ROOT)),'calculator_sha256':sha(Path(__file__)), 'syntax_source_sha256':sha(ROOT/'tools/round7_coalesce_syntax.cs'),'patches':patches,'methods':diagnostics}
 (ROOT/'reports/round7-coalesce-patches.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8',newline='\n')


if __name__=='__main__':main()
