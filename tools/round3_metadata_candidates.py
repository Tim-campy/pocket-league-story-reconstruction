"""Prepare and measure metadata hypotheses using frozen round-3 C# only.

Never edits main sources, ledgers, type states, production builds or assemblies.
"""
import argparse,hashlib,json,os,subprocess,sys
from pathlib import Path
from compare_kairo import differences,sha
from metadata_compare import compare_paths,compare_metadata,inspect_metadata
from prepare_metadata_inspector import ensure_inspector
from kairo_candidate import COMPILER_PATH
ROOT=Path(__file__).resolve().parents[1]
TARGETS={
 'ResourceManager':('KairoLibrary','src/KairoLibrary/kairo/unity/ui/ResourceManager.cs'),
 'HttpObject':('KairoLibrary','src/KairoLibrary/kairo/unity/io/HttpObject.cs'),
 'FormManagerBase':('KairoLibrary','src/KairoLibrary/kairo/unity/form/FormManagerBase.cs'),
 'LanguageForm':('Assembly-CSharp','src/Assembly-CSharp/system/form/LanguageForm.cs'),
 'ResourceScope':('KairoLibrary','src/KairoLibrary/kairo/unity/ui/ResourceManager.cs'),
 'HttpScope':('KairoLibrary','src/KairoLibrary/kairo/unity/io/HttpObject.cs'),
 'FormScope':('KairoLibrary','src/KairoLibrary/kairo/unity/form/FormManagerBase.cs'),
 'ClosureCombined':('KairoLibrary',None)}
def read(path):return json.loads(Path(path).read_text(encoding='utf-8-sig'))
def dump(path,output):
    with output.open('wb') as file:subprocess.run([str(ROOT/'build/tools/ProbeInspector.exe'),str(path),'--structure'],stdout=file,check=True)
    return read(output)
def frozen_metadata(assembly):
    manifest=read(ROOT/'build/round3'/(assembly+'-build-manifest.json'))
    identity={'binary_sha256':manifest['output_sha256'],'inspector_sha256':sha(ensure_inspector())}
    key=hashlib.sha256(json.dumps(identity,sort_keys=True).encode()).hexdigest()
    cache=ROOT/'build/metadata-cache'/(key+'.json')
    if not cache.is_file():raise ValueError('Frozen baseline metadata cache missing; do not substitute live production metadata')
    document=read(cache)
    if document['artifact_sha256']!=identity['binary_sha256']:raise ValueError('Frozen baseline metadata hash mismatch')
    return document
def compile_frozen(assembly,label,manifest,mapping):
    folder=ROOT/'build'/(assembly+'-'+label);folder.mkdir(parents=True,exist_ok=True)
    output=folder/(assembly+'.dll')
    for item in manifest['references']:
        if sha(Path(item['path']))!=item['sha256']:raise ValueError('Frozen reference changed')
    compiler=Path(manifest['compiler'])
    if sha(compiler)!=manifest['compiler_sha256']:raise ValueError('Frozen compiler changed')
    sources=[{**s,'input_path':mapping[s['path']],'sha256':sha(Path(mapping[s['path']]))} for s in manifest['sources']]
    if assembly=='KairoLibrary':
        flags=[f for f in manifest['flags'] if not f.startswith('/out:')]+['/out:'+str(output)]
        args=[f for f in flags if f!='/noconfig']+['/reference:'+r['path'] for r in manifest['references']]+[s['input_path'] for s in sources]
        command=[str(compiler),'/noconfig'];env=None
    else:
        flags=[f for f in manifest['flags'] if not f.startswith('-out:')]+['-out:'+str(output)]
        args=[f for f in flags if f!='-noconfig']+['-r:'+r['path'] for r in manifest['references']]+[s['input_path'] for s in sources]
        command=[manifest['runtime'],str(compiler),'-noconfig'];env=os.environ.copy();env.update(manifest['scoped_environment'])
        if sha(Path(manifest['runtime']))!=manifest['runtime_sha256']:raise ValueError('Frozen runtime changed')
    rsp=folder/'compile.rsp';rsp.write_text('\n'.join('"'+x+'"' for x in args)+'\n',encoding='utf-8')
    result=subprocess.run(command+['@'+str(rsp)],cwd=ROOT,env=env,capture_output=True)
    (folder/'compiler.log').write_bytes(result.stdout+result.stderr)
    recorded={**manifest,'sources':sources,'flags':flags,'exit_code':result.returncode,'output_sha256':sha(output) if result.returncode==0 else None}
    (folder/'build-manifest.json').write_text(json.dumps(recorded,indent=2)+'\n')
    return result
def matching(text,start,left,right):
    depth=0;quote=None;escape=False;index=start
    while index<len(text):
        char=text[index]
        if quote:
            if escape:escape=False
            elif char=='\\':escape=True
            elif char==quote:quote=None
        elif char in ('"',"'"):quote=char
        elif char==left:depth+=1
        elif char==right:
            depth-=1
            if depth==0:return index
        index+=1
    raise ValueError('Unbalanced frozen source')
def scope_transform(name,text):
    nl='\r\n' if '\r\n'in text else '\n';patches=[]
    if name=='ResourceScope':
        needle='\t\t\tbool flag;'+nl+'\t\t\tdo'+nl+'\t\t\t{'
        start=text.index(needle);brace=start+len(needle)-1;end=matching(text,brace,'{','}')
        suffix=nl+'\t\t\twhile (!flag);'
        if text[end+1:end+1+len(suffix)]!=suffix:raise ValueError('Unexpected resource do-while end')
        old=text[start:end+1+len(suffix)];body=text[brace+1:end]
        declaration=nl+'\t\t\t\tflag = true;'
        if body.count(declaration)!=1:raise ValueError('Resource flag assignment changed')
        body=body.replace(declaration,nl+'\t\t\t\tbool flag = true;')
        body=body.rstrip()+nl+'\t\t\t\tif (flag)'+nl+'\t\t\t\t{'+nl+'\t\t\t\t\tbreak;'+nl+'\t\t\t\t}'+nl+'\t\t\t'
        new='\t\t\twhile (true)'+nl+'\t\t\t{'+body+'}'
        return text[:start]+new+text[end+1+len(suffix):],[(old,new)]
    needle='while (!(bool)IApplication.GetCurrentApp().RunOnUiThread(delegate'
    cursor=0
    while True:
        start=text.find(needle,cursor)
        if start<0:break
        line=text.rfind('\n',0,start)+1;indent=text[line:start]
        paren=start+len('while ');end_condition=matching(text,paren,'(',')')
        brace=end_condition+1
        while text[brace].isspace():brace+=1
        if text[brace]!='{':raise ValueError('While body must be a block')
        end=matching(text,brace,'{','}')
        positive=text[paren+1:end_condition][1:]
        body=text[brace+1:end].strip()
        old=text[start:end+1]
        new='while (true)'+nl+indent+'{'+nl+indent+'\tbool completed = '+positive+';'+nl+indent+'\tif (completed)'+nl+indent+'\t{'+nl+indent+'\t\tbreak;'+nl+indent+'\t}'
        if body:new+=nl+indent+'\t'+body
        new+=nl+indent+'}'
        patches.append((old,new));text=text[:start]+new+text[end+1:];cursor=start+len(new)
    if len(patches)!=(1 if name=='HttpScope' else 2):raise ValueError('Unexpected while count')
    return text,patches
def transform(name,text):
    if name.endswith('Scope'):return scope_transform(name,text)
    if name=='LanguageForm':
        replacements=[('HttpForm form = new HttpForm(delegate','HttpForm languageForm = new HttpForm(delegate(HttpForm form, HttpForm.ResultData data)'),
            ('Push(form);','Push(languageForm);'),
            ('}, delegate\r\n','}, delegate(HttpForm form, HttpForm.ResultData data)\r\n'),
            ('new HttpForm(delegate\r\n','new HttpForm(delegate(HttpForm form, HttpForm.ResultData data)\r\n'),
            ('delegate(HttpForm httpForm2, HttpForm.ResultData resultData)','delegate(HttpForm form, HttpForm.ResultData data)'),
            ('resultData.Succeed','data.Succeed'),('resultData.Error','data.Error')]
        for old,new in replacements:
            if text.count(old)!=1:raise ValueError('LanguageForm shape changed: '+repr(old))
            text=text.replace(old,new)
        return text,replacements
    needle='IApplication.GetCurrentApp().RunOnUiThread(delegate'
    if name=='ResourceManager':
        old='\t\t\t\t'+needle
        if text.count(old)<1:raise ValueError('Resource delegate absent')
        index=text.index(old,text.index('public void LoadStart(JarInflater jar'))
        return text[:index]+text[index:].replace(needle,needle.replace('delegate','() =>'),1),[(needle,'IApplication.GetCurrentApp().RunOnUiThread(() =>')]
    if name=='HttpObject':
        needle='while (!(bool)IApplication.GetCurrentApp().RunOnUiThread(delegate'
        if text.count(needle)!=1:raise ValueError('Http delegate shape changed')
        return text.replace(needle,needle.replace('delegate','() =>')),[(needle,needle.replace('delegate','() =>'))]
    needle='while (!(bool)IApplication.GetCurrentApp().RunOnUiThread(delegate'
    if text.count(needle)!=2:raise ValueError('Capture delegate shape changed')
    return text.replace(needle,needle.replace('delegate','() =>')),[(needle,needle.replace('delegate','() =>'))]
def evaluate(name):
    assembly,logical=TARGETS[name]
    baseline=ROOT/'build/round3';original=read(baseline/(assembly+'-original-methods.json'));before=read(baseline/(assembly+'-rebuilt-methods.json'))
    source_map=read(ROOT/'analysis/variants'/('round3-baseline-'+assembly)/'source-map.json')
    baseline_manifest=read(baseline/(assembly+'-build-manifest.json'))
    if any(sha(Path(source_map[s['path']]))!=s['sha256'] for s in baseline_manifest['sources']):raise ValueError('Frozen baseline source changed')
    folder=ROOT/'analysis/variants'/('round3-meta-'+name);folder.mkdir(parents=True,exist_ok=True)
    changes=[]
    for part in (['ResourceScope','HttpScope','FormScope'] if name=='ClosureCombined' else [name]):
        part_logical=TARGETS[part][1];input_path=Path(source_map[part_logical]);text=input_path.read_bytes().decode('utf-8-sig')
        candidate,patches=transform(part,text)
        out=folder/(part+'.cs');out.write_bytes(candidate.encode('utf-8'));source_map[part_logical]=str(out)
        changes.append({'logical':part_logical,'baseline_source_sha256':sha(input_path),'candidate_source_sha256':sha(out),'candidate_source':str(out.relative_to(ROOT)),'patches':patches})
    map_path=folder/'source-map.json';map_path.write_text(json.dumps(source_map,indent=2)+'\n')
    label='round3-meta-'+name
    result=compile_frozen(assembly,label,baseline_manifest,source_map);(folder/'compile.log').write_bytes(result.stdout+result.stderr)
    report={'assembly':assembly,'candidate':name,'source_changes':changes,
       'source_map':str(map_path.relative_to(ROOT)),'compile_exit_code':result.returncode,'main_sources_modified':False,'ledger_modified':False,
       'baseline_artifact_sha256':baseline_manifest['output_sha256'],'baseline_metadata_source':'hash-bound inspector cache for frozen baseline manifest; never live production'}
    if len(changes)==1:report.update(changes[0])
    if result.returncode:
        report['errors']=(result.stdout+result.stderr).decode('utf-8',errors='replace')[-5000:]
    else:
        build=ROOT/'build'/(assembly+'-'+label);rebuilt=dump(build/(assembly+'.dll'),build/'rebuilt-methods.json')
        oi={m['stable_id']:m for m in original['selected_methods']};bi={m['stable_id']:m for m in before['selected_methods']};ni={m['stable_id']:m for m in rebuilt['selected_methods']}
        old_equal={k for k,m in oi.items() if 'raw_body_hex'in m and not differences(m,bi.get(k))[0]}
        new_equal={k for k,m in oi.items() if 'raw_body_hex'in m and not differences(m,ni.get(k))[0]}
        oracle=ROOT/'original/extracted/assets/bin/Data/Managed'/(assembly+'.dll')
        oldmeta=compare_metadata(inspect_metadata(oracle),frozen_metadata(assembly))
        newmeta=compare_paths(oracle,build/(assembly+'.dll'),build/'metadata-comparison.json')
        ot={k for k,v in oldmeta['types'].items() if v['equal']};nt={k for k,v in newmeta['types'].items() if v['equal']}
        report.update(body_equal_before=len(old_equal),body_equal_after=len(new_equal),body_gains=sorted(new_equal-old_equal),body_losses=sorted(old_equal-new_equal),
           type_equal_before=len(ot),type_equal_after=len(nt),type_gains=sorted(nt-ot),type_losses=sorted(ot-nt),
           original_methods_missing=sorted(set(oi)-set(ni)),extra_method_ids=sorted(set(ni)-set(oi)),candidate_sha256=sha(build/(assembly+'.dll')),
           declarations_proved=newmeta['declarations_proved'],remaining_type_differences=newmeta['type_difference_counts_nonexclusive'])
    path=ROOT/'reports'/('round3-metadata-'+name+'.json');path.write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({k:v for k,v in report.items() if k in ['candidate','compile_exit_code','body_equal_before','body_equal_after','body_gains','body_losses','type_gains','type_losses','type_equal_after']}),flush=True)
def main():
    parser=argparse.ArgumentParser();parser.add_argument('--candidate',choices=list(TARGETS),action='append');args=parser.parse_args()
    for name in args.candidate or TARGETS:evaluate(name)
if __name__=='__main__':main()
