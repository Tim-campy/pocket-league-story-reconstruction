"""Recover two empty comparisons, accepting C# only through complete verification."""
import json,subprocess,sys
from pathlib import Path
from apply_il_rules import index_current,dump,read,receipt_is_fresh
from compare_kairo import differences,sha
from kairo_candidate import COMPILER_PATH
ROOT=Path(__file__).resolve().parents[1]
def main():
    logical='src/KairoLibrary/kairo/unity/native/KairoPlugin.cs';reports=[]
    for label,key,old_lines,new_lines in [
        ('statusbar','kairo.unity.native.KairoPlugin::GetStatusBarHeight()->System.Int32',
         ['if (statusBarHeight_ != int.MaxValue)','\t\t\t{','\t\t\t\tint statusBarHeight_2 = statusBarHeight_;','\t\t\t\tint num = 0;','\t\t\t}'],
         ['if (statusBarHeight_ != int.MaxValue && statusBarHeight_ > 0) { }']),
        ('capture','kairo.unity.native.KairoPlugin/<>c__DisplayClass82_0::<SaveToTmpCaptureDirectory>b__0()->System.Object',
         ['int aPPLI_TYPE = kairo.common.cfg.Config.APPLI_TYPE;','\t\t\t\t\tint num = 2;'],
         ['if (kairo.common.cfg.Config.APPLI_TYPE == 2) { }'])]:
        original,before,_=index_current('KairoLibrary');equal={k for k,m in original.items() if 'raw_body_hex'in m and not differences(m,before.get(k))[0]}
        source=ROOT/logical;text=source.read_bytes().decode('utf-8-sig');nl='\r\n' if '\r\n'in text else '\n'
        old=nl.join(old_lines);new=nl.join(new_lines)
        if text.count(old)!=1:raise ValueError('Source empty-guard shape changed: '+label)
        folder=ROOT/'analysis/variants'/('round3-guard-'+label);folder.mkdir(parents=True,exist_ok=True)
        out=folder/'KairoPlugin.cs';out.write_bytes(text.replace(old,new).encode('utf-8'))
        mp=folder/'source-map.json';mp.write_text(json.dumps({logical:str(out)}))
        p=subprocess.run([sys.executable,str(ROOT/'tools/build_kairo.py'),'--compiler',str(ROOT/COMPILER_PATH),'--label','round3-guard-'+label,'--source-map',str(mp)],capture_output=True,cwd=ROOT)
        (folder/'build.log').write_bytes(p.stdout+p.stderr)
        if p.returncode:raise ValueError('Candidate compilation failed')
        built=dump(ROOT/'build'/('KairoLibrary-round3-guard-'+label)/'KairoLibrary.dll',folder/'methods.json')
        gains=[k for k,m in original.items() if k not in equal and 'raw_body_hex'in m and not differences(m,built.get(k))[0]]
        losses=[k for k in sorted(equal) if differences(original[k],built.get(k))[0]]
        record={'id':key,'source_before':old,'source_after':new,'candidate_equal':not differences(original[key],built.get(key))[0],
            'body_gains':gains,'body_losses':losses,'adopted':False,'state_gains':0,'state_losses':0,'operator_historical':'UNKNOWN'}
        if record['candidate_equal'] and not losses:
            backup=source.read_bytes();oldreceipt=read(ROOT/'build/verification-KairoLibrary.json');source.write_bytes(out.read_bytes())
            p=subprocess.run([sys.executable,str(ROOT/'tools/verify'),'--all','--assembly','KairoLibrary'],cwd=ROOT,capture_output=True)
            (folder/'official-verify.log').write_bytes(p.stdout+p.stderr)
            receipt=read(ROOT/'build/verification-KairoLibrary.json')
            if p.returncode not in (0,1) or not receipt_is_fresh(ROOT,receipt) or receipt['regressions'] or receipt.get('type_regressions'):
                source.write_bytes(backup)
                subprocess.run([sys.executable,str(ROOT/'tools/verify'),'--all','--assembly','KairoLibrary'],cwd=ROOT,stdout=subprocess.DEVNULL,check=False)
                raise ValueError('Empty guard rejected; restored sources and reverified')
            record['adopted']=True;record['state_gains']=sum(r['state']=='matched' and oldreceipt['methods'][k]['state']!='matched' for k,r in receipt['methods'].items())
        reports.append(record)
        (ROOT/'reports/round3-elided-guard-adoption.json').write_text(json.dumps({'methods':reports},indent=2)+'\n',encoding='utf-8')
        print(json.dumps({'label':label,'candidate_equal':record['candidate_equal'],'gained':record['state_gains'],'lost':len(losses),'adopted':record['adopted']}),flush=True)
if __name__=='__main__':main()
