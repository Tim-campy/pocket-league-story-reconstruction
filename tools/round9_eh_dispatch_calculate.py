"""WIP calculated EH result identity and state-dispatch candidates; no game Emit."""
import json,sys,importlib,importlib.util
from pathlib import Path
ROOT=next(p for p in Path(__file__).resolve().parents if (p/'tools/compiler_probes.py').is_file());sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import sha
SCRIPT=Path(__file__).resolve().parent;WORK=ROOT/'build/round9/part-d-next'
spec=importlib.util.spec_from_file_location('next_source_host',ROOT/'tools/round9_float_identity_fixtures.py');ff=importlib.util.module_from_spec(spec);spec.loader.exec_module(ff)
ff.WORK=WORK;ff.RULE=SCRIPT/'Round9CalculatedDispatchRules.cs';ff.HOST=ff.HOST.replace('AddRound9FloatingIdentityRules','AddRound9CalculatedDispatchRules')
BASE=ROOT/'build/round9/cycles/c11/after/KairoLibrary'
TARGETS=[('kairo.unity.util.JarInflater::GetData(System.String)->System.Byte[]','src/KairoLibrary/kairo/unity/util/JarInflater.cs','d9-named-eh-result-rejoin'),
 ('kairo.unity.ui.SaveRecorder::MakeSaveData(kairo.unity.util.Property,kairo.unity.ui.SaveRecorder/AutoSaveLogData&)->kairo.unity.util.Property','src/KairoLibrary/kairo/unity/ui/SaveRecorder.cs','d9-calculated-state-dispatch-index')]

def restore_jar(before):
 # These anchors name the computed role, not the original author variable name.
 old='\t\t\tint len;\n';assert before.count(old)==1
 after=before.replace(old,'',1)
 anchor='byte[] array3 = new byte[1024];';assert after.count(anchor)==1
 after=after.replace(anchor,anchor+'\r\n\t\t\tint len;',1)
 returned='return byteArrayOutputStream.ToByteArray();';assert after.count(returned)==1
 after=after.replace(returned,'array2 = byteArrayOutputStream.ToByteArray();',1)
 assert after.endswith('\t\t}');after=after[:-3]+'\t\t\treturn array2;\r\n\t\t}'
 return after

def restore_save_declarations(before):
 anchor='for (int i = 0; i < array2.Length; i++)';assert before.count(anchor)==1
 after=before.replace(anchor,'int num2, i;\r\n\t\t\tfor (i = 0; i < array2.Length; i++)',1)
 old='int num2 = 0;';assert after.count(old)==1;return after.replace(old,'num2 = 0;',1)

def main():
 manifest=json.loads((BASE/'build-manifest.json').read_text());
 for s in manifest['sources']:s['path']=str(BASE/s['path']);s['input_path']=s['path']
 mp=WORK/'frozen-manifest.json';mp.write_text(json.dumps(manifest,indent=2)+'\n')
 cfg=json.loads((ROOT/'config/round9-protocol.json').read_text())['models']['KairoLibrary'];types=json.loads((BASE/'plain-local-types.json').read_text());patches=[];proofs=[]
 with importlib.import_module(cfg['module']).SlotModelClient(mp)as model:
  for identity,logical,rule in TARGETS:
   path=BASE/logical;source=path.read_bytes().decode('utf-8-sig');before_model=model.predict(path,source,identity);start,end=before_model['declaration_start'],before_model['declaration_end'];raw=source.encode('utf-16-le');before=raw[start*2:end*2].decode('utf-16-le')
   if rule=='d9-named-eh-result-rejoin':after=restore_jar(before)
   else:
    response=ff.query(path,mp,dict(round7_cycle_rule=rule,target_method='MakeSaveData'));small=[p for r in response for p in r['patches']];assert len(small)==1
    changed=ff.apply(source,small[0]);delta=len(changed.encode('utf-16-le'))//2-len(raw)//2;after=changed.encode('utf-16-le')[start*2:(end+delta)*2].decode('utf-16-le');after=restore_save_declarations(after)
   candidate=(raw[:start*2]+after.encode('utf-16-le')+raw[end*2:]).decode('utf-16-le');pred=model.predict(path,candidate,identity)
   patch=dict(method_id=identity,logical_path=logical,path=str(ROOT/logical),start=start,end=end,before=before,after=after,rule=rule,
    model_source_sha256=sha(ROOT/cfg['source']),model_client_sha256=sha(ROOT/cfg['client']),client_sha256=sha(ROOT/cfg['client']))
   patches.append(patch);proofs.append(dict(method_id=identity,before_model=before_model,after_model=pred,original_types=types[identity],after_types_equal=pred['predicted_types']==types[identity],source_sha256=sha(path),game_compilations=0,body_match='UNKNOWN',
    complete_wrong_signature=pred['complete']and pred['predicted_types']!=types[identity],before_CRLF=before.count('\r\n'),before_LF_only=before.count('\n')-before.count('\r\n'),
    after_CRLF=after.count('\r\n'),after_LF_only=after.count('\n')-after.count('\r\n'),unique_raw_before=source.count(before)==1))
   print(identity,'complete',pred['complete'],'predicted',len(pred['predicted_types']),'original',len(types[identity]),'equal',proofs[-1]['after_types_equal'],pred['unsupported_reasons'])
 files=[cfg['source'],cfg['client'],'tools/round9_float_identity_fixtures.py','tools/round9_float_identity_hints.py','tools/compiler_probes.py']
 if SCRIPT==ROOT/'tools':files.extend(['tools/Round9CalculatedDispatchRules.cs','tools/round9_eh_dispatch_calculate.py','tools/round9_eh_dispatch_fixtures.py'])
 dependencies={p:sha(ROOT/p)for p in files}
 for patch in patches:patch.update(dependency_sha256=dependencies,syntax_source_sha256=sha(ff.RULE),calculator_sha256=sha(Path(__file__)))
 result=dict(operation='new-calculated-local-identity-candidates',baseline=str(BASE.relative_to(ROOT)),patches=patches,proofs=proofs,adopted=False,game_compilations=0,
  dependency_sha256=dependencies,syntax_source_sha256=sha(ff.RULE),calculator_sha256=sha(Path(__file__)),baseline_receipt_sha256=sha(BASE/'verification.json'),
  original_dump_sha256=sha(BASE/'original-methods.json'),rebuilt_dump_sha256=sha(BASE/'rebuilt-methods.json'),baseline_manifest_sha256=sha(BASE/'build-manifest.json'))
 (WORK/'patches-and-proof.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8')
if __name__=='__main__':main()
