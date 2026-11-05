"""Rebase metadata source fixes onto current C#, checking each complete assembly."""
import json,subprocess,sys
from pathlib import Path
from compare_kairo import sha
from round3_metadata_candidates import transform,TARGETS
from apply_il_rules import receipt_is_fresh
ROOT=Path(__file__).resolve().parents[1]
def read(p):return json.loads(p.read_text(encoding='utf-8-sig'))
def main():
    results=[]
    for name in ['LanguageForm','ResourceScope','HttpScope','FormScope']:
        assembly,logical=TARGETS[name];source=ROOT/logical;before=source.read_bytes();text=before.decode('utf-8-sig')
        changed,patches=transform(name,text);receipt_before=read(ROOT/'build'/('verification-'+assembly+'.json'))
        metadata_before=read(ROOT/'reports'/(assembly+'-metadata.json'))
        folder=ROOT/'build/round3'/('metadata-adoption-'+name);folder.mkdir(parents=True,exist_ok=True);(folder/source.name).write_bytes(before)
        original_sha=sha(source);source.write_bytes(changed.encode('utf-8'))
        p=subprocess.run([sys.executable,str(ROOT/'tools/verify'),'--all','--assembly',assembly],cwd=ROOT,capture_output=True)
        (folder/'verify.log').write_bytes(p.stdout+p.stderr)
        receipt=read(ROOT/'build'/('verification-'+assembly+'.json'));metadata=read(ROOT/'reports'/(assembly+'-metadata.json'))
        if p.returncode not in (0,1) or not receipt_is_fresh(ROOT,receipt) or receipt['regressions'] or receipt.get('type_regressions'):
            source.write_bytes(before)
            subprocess.run([sys.executable,str(ROOT/'tools/verify'),'--all','--assembly',assembly],cwd=ROOT,stdout=subprocess.DEVNULL)
            raise ValueError('Metadata source candidate regressed; restored source and verified')
        gains=[k for k,r in receipt['methods'].items() if r['state']=='matched' and receipt_before['methods'][k]['state']!='matched']
        types_before={k for k,t in metadata_before['types'].items() if t['equal']};types_after={k for k,t in metadata['types'].items() if t['equal']}
        record={'name':name,'assembly':assembly,'source':logical,'before_sha256':original_sha,'after_sha256':sha(source),'patches':patches,
            'state_gains':gains,'state_losses':receipt['regressions'],'type_gains':sorted(types_after-types_before),'type_losses':sorted(types_before-types_after),
            'counts_before':receipt_before['counts'],'counts_after':receipt['counts']}
        results.append(record)
        (ROOT/'reports/round3-metadata-adoption.json').write_text(json.dumps({'steps':results},indent=2)+'\n',encoding='utf-8')
        print(json.dumps({'name':name,'methods_gained':len(gains),'methods_lost':0,'types_gained':len(record['type_gains']),'types_lost':len(record['type_losses'])}),flush=True)
if __name__=='__main__':main()
