"""Test local-scope repairs for three lock-labelled methods in a frozen snapshot.

Only isolated C# files are written. The original Monitor overloads/EH layout
already match; source scopes determine the differing local slot order.
"""
import argparse
import json
from pathlib import Path
import re
import subprocess
import sys
from compare_kairo import differences,sha
from compare_csharp_stdlib import inspect

ROOT=Path(__file__).resolve().parents[1]
TARGETS=('kairo.unity.ui.IApplication::RunOnUiThread(System.Threading.ThreadStart,System.Boolean)->System.Void',
    'kairo.unity.analytics.AnalyticsTracker::Run()->System.Void',
    'kairo.unity.surface.SurfaceBase::OnReceiveTouchEvent(kairo.unity.surface.SurfaceBase/MotionEventRecord)->System.Void')


def repair(key,text):
    newline='\r\n' if '\r\n' in text else '\n'
    if key.startswith('kairo.unity.ui.IApplication::'):
        needle='\t\t\tMethodTask methodTask = new MethodTask(start);'
        if text.count(needle)!=1:raise ValueError('Unexpected ThreadStart overload source')
        start=text.index(needle);end=text.rfind(newline+'\t\t}')
        if end<start:raise ValueError('Method end missing')
        tail=text[start:end]
        return text[:start]+'\t\t\t{'+newline+newline.join('\t'+line for line in tail.split(newline))+newline+'\t\t\t}'+text[end:]
    if key.startswith('kairo.unity.surface.SurfaceBase::'):
        needle='((!evt.IsMousePointer()) ? GetTouchOffset() : 0)'
        if text.count(needle)!=1:raise ValueError('Mouse offset conditional shape changed')
        text=text.replace(needle,'(evt.IsMousePointer() ? 0 : GetTouchOffset())')
        start=text.index('\t\t\t\tif (start_.IsEmpty())');end=text.rfind(newline+'\t\t\t}')
        tail=text[start:end]
        negative=re.search(r'(?m)^([ \t]*)if \(!multiTouchEnable_\)\r?\n\1\{\r?\n\1\treturn;\r?\n\1\}\r?\n',tail)
        if negative is None:raise ValueError('Early multitouch return shape changed')
        prefix=tail[:negative.start()];remaining=tail[negative.end():]
        numeric=remaining.index('\t\t\t\tint num = -1;')
        numeric_tail=remaining[numeric:]
        indent=lambda block:newline.join('\t'+line for line in block.split(newline))
        remaining=remaining[:numeric]+'\t\t\t\t{'+newline+indent(numeric_tail)+newline+'\t\t\t\t}'
        tail=prefix+'\t\t\t\tif (multiTouchEnable_)'+newline+'\t\t\t\t{'+newline+indent(remaining)+newline+'\t\t\t\t}'
        return text[:start]+'\t\t\t\t{'+newline+indent(tail)+newline+'\t\t\t\t}'+text[end:]
    declaration=re.search(r'(?m)^([ \t]*)AnalyticsData analyticsData = null;',text)
    if declaration is None:raise ValueError('Analytics local declaration missing')
    inserted=declaration.group(0)+newline+declaration.group(1)+'HttpObject httpObject;'
    result=text[:declaration.start()]+inserted+text[declaration.end():]
    needle='HttpObject httpObject = new HttpObject();'
    if result.count(needle)!=1:raise ValueError('HttpObject assignment missing')
    return result.replace(needle,'httpObject = new HttpObject();')


def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--only',choices=['ui','analytics','surface'])
    parser.add_argument('--combine',action='store_true',help='Validate composition of the three previously proven isolated candidates')
    args=parser.parse_args()
    frozen=ROOT/'analysis/variants/kairo-lock-frozen';folder=ROOT/'analysis/variants/kairo-lock-experiments'
    snapshot=json.loads((frozen/'receipt.json').read_text(encoding='utf-8'))
    base_map=json.loads((frozen/'source-map.json').read_text(encoding='utf-8'))
    for source in snapshot['sources']:
        if sha(Path(base_map[source['path']]))!=source['sha256']:raise ValueError('Frozen source differs from verified snapshot')
    base_folder=ROOT/'build/KairoLibrary-lock-frozen'
    manifest=json.loads((base_folder/'build-manifest.json').read_text(encoding='utf-8'))
    if manifest['exit_code'] or sha(base_folder/'KairoLibrary.dll')!=manifest['output_sha256']:raise ValueError('Frozen build changed')
    original=json.loads((ROOT/'build/KairoLibrary-roslyn-1.3.2/original-methods.json').read_text(encoding='utf-8-sig'))
    baseline=json.loads((base_folder/'rebuilt-methods.json').read_text(encoding='utf-8-sig'))
    oracle={m['stable_id']:m for m in original['selected_methods']};previous={m['stable_id']:m for m in baseline['selected_methods']}
    previous_equal={key for key,m in oracle.items() if 'raw_body_hex' in m and not differences(m,previous.get(key))[0]}
    if len(previous_equal)!=snapshot['counts']['body_equal']:raise ValueError('Frozen baseline does not reproduce the snapshot count')
    references=json.loads((ROOT/'config/method-reference.json').read_text(encoding='utf-8'))
    ids_by_token={r['token']:key for key,r in references.items() if key.startswith('KairoLibrary::')}
    for key,method in oracle.items():
        token='0x'+format(0x06000000|int(method['token'].split('0x')[1].rstrip(']'),16),'08x')
        if ('raw_body_hex' in method and not differences(method,previous.get(key))[0])!=snapshot['methods'][ids_by_token[token]]['body_equal']:
            raise ValueError('Frozen baseline changed an original body-equality state')
    captured=json.loads((folder/'target-sources.json').read_text(encoding='utf-8'))
    inspector=ROOT/'build/Assembly-CSharp-profile-tools/ProbeInspector.exe';results={}
    report_path=ROOT/'reports/KairoLibrary-lock-scope-probes.json'
    if (args.only or args.combine) and report_path.is_file():
        previous_report=json.loads(report_path.read_text(encoding='utf-8'))
        if previous_report['frozen_compilation_build_sha256']!=manifest['output_sha256']:
            raise ValueError('Cannot append probes from a different frozen baseline')
        results=previous_report['results']
    for index,key in enumerate(TARGETS):
        if args.combine:continue
        if args.only and args.only!=('ui','analytics','surface')[index]:continue
        if key in previous_equal:
            results[key]={'status':'ALREADY_EQUAL_IN_FROZEN_SNAPSHOT'};continue
        record=captured[key];before=record['text'];after=repair(key,before)
        source=Path(record['path']);encoded=source.read_bytes().decode('utf-8-sig').encode('utf-16-le')
        span=record['span'];start=span['start']*2;end=span['end']*2
        if encoded[start:end].decode('utf-16-le')!=before:raise ValueError('Captured method span changed')
        logical=next(k for k,v in base_map.items() if Path(v)==source)
        output=folder/('candidate-'+str(index))/logical;output.parent.mkdir(parents=True,exist_ok=True)
        output.write_bytes((encoded[:start]+after.encode('utf-16-le')+encoded[end:]).decode('utf-16-le').encode('utf-8'))
        mapping=dict(base_map);mapping[logical]=str(output)
        source_map=folder/('candidate-'+str(index)+'-source-map.json');source_map.write_text(json.dumps(mapping,indent=2)+'\n',encoding='utf-8')
        label='lock-scope-'+str(index)
        completed=subprocess.run([sys.executable,str(ROOT/'tools/build_kairo.py'),'--compiler',
            str(ROOT/'analysis/tools/roslyn-1.3.2/tools/csc.exe'),'--label',label,'--source-map',str(source_map)],
            cwd=ROOT,capture_output=True,text=True,encoding='utf-8',errors='replace')
        if completed.returncode:raise RuntimeError(completed.stdout+completed.stderr)
        build_folder=ROOT/'build'/('KairoLibrary-'+label)
        built=inspect(inspector,build_folder/'KairoLibrary.dll',build_folder/'rebuilt-methods.json')
        methods={m['stable_id']:m for m in built['selected_methods']}
        equal={name for name,m in oracle.items() if 'raw_body_hex' in m and not differences(m,methods.get(name))[0]}
        delta,percent=differences(oracle[key],methods.get(key));lost=sorted(previous_equal-equal);gained=sorted(equal-previous_equal)
        bm=json.loads((build_folder/'build-manifest.json').read_text(encoding='utf-8'))
        if bm['compiler_sha256']!=manifest['compiler_sha256'] or bm['references']!=manifest['references']:raise ValueError('Compiler/reference inputs changed')
        results[key]={'status':'EXACT_CANDIDATE' if not delta and not lost else 'UNRESOLVED',
            'original_locals':oracle[key]['locals'],'before_locals':previous[key]['locals'],'after_locals':methods[key]['locals'],
            'before_differences':differences(oracle[key],previous[key])[0],'after_differences':delta,'percent':percent,
            'header_equal':oracle[key]['header']==methods[key]['header'],
            'exception_regions_equal':oracle[key]['exception_regions']==methods[key]['exception_regions'],
            'body_equal':not delta,'previously_equal_body_losses':lost,'gained_body_ids':gained,'body_equal_total':len(equal),
            'method_before':before,'method_after':after,'logical_source':logical,'candidate_source':str(output),
            'source_map':str(source_map),'build_sha256':bm['output_sha256'],'compiler_sha256':bm['compiler_sha256'],
            'flags':bm['flags'],'references':bm['references']}
    combined=None
    if args.combine:
        if any(results.get(key,{}).get('status')!='EXACT_CANDIDATE' for key in TARGETS):
            raise ValueError('Only combine the three already proven candidates')
        mapping=dict(base_map)
        for key in TARGETS:mapping[results[key]['logical_source']]=results[key]['candidate_source']
        source_map=folder/'combined-source-map.json';source_map.write_text(json.dumps(mapping,indent=2)+'\n',encoding='utf-8')
        completed=subprocess.run([sys.executable,str(ROOT/'tools/build_kairo.py'),'--compiler',
            str(ROOT/'analysis/tools/roslyn-1.3.2/tools/csc.exe'),'--label','lock-scope-combined','--source-map',str(source_map)],
            cwd=ROOT,capture_output=True,text=True,encoding='utf-8',errors='replace')
        if completed.returncode:raise RuntimeError(completed.stdout+completed.stderr)
        build_folder=ROOT/'build/KairoLibrary-lock-scope-combined'
        built=inspect(inspector,build_folder/'KairoLibrary.dll',build_folder/'rebuilt-methods.json')
        methods={m['stable_id']:m for m in built['selected_methods']}
        equal={name for name,m in oracle.items() if 'raw_body_hex' in m and not differences(m,methods.get(name))[0]}
        combined={'source_map':str(source_map),'body_equal_total':len(equal),
            'gained_body_ids':sorted(equal-previous_equal),'previously_equal_body_losses':sorted(previous_equal-equal),
            'target_equal':{key:not differences(oracle[key],methods.get(key))[0] for key in TARGETS},
            'build_sha256':sha(build_folder/'KairoLibrary.dll')}
    report={'scope':'local-scope corrections in three lock-labelled methods; full frozen-assembly regression comparison',
        'frozen_body_equal_count':len(previous_equal),'frozen_receipt_build_sha256':snapshot['build_sha256'],
        'frozen_compilation_build_sha256':manifest['output_sha256'],'source_hashes_match_verified_receipt':True,
        'results':results,'applied_to_main':False,'ledger_updated':False,'normalized_assembly_fields':[]}
    if combined is not None:report['combined']=combined
    report_path.write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({key:{name:value[name] for name in ('status','body_equal','body_equal_total','after_differences','previously_equal_body_losses') if name in value} for key,value in results.items()}))


if __name__=='__main__':main()
