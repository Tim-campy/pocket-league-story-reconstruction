"""Test empty-condition lowering in own fixtures and two frozen target variants."""
import json,subprocess,sys
from pathlib import Path
from compare_kairo import differences,body_cells,sha
from apply_il_rules import dump,read
from compiler_probes import UNITY
from kairo_candidate import COMPILER_PATH
ROOT=Path(__file__).resolve().parents[1]
KEY='kairo.unity.native.KairoPlugin::GetStatusBarHeight()->System.Int32'
LOGICAL='src/KairoLibrary/kairo/unity/native/KairoPlugin.cs'
def main():
    folder=ROOT/'build/round3/elided-branch';folder.mkdir(parents=True,exist_ok=True)
    fixtures=[]
    for n,op in [('Gt','>'),('Ge','>='),('Lt','<'),('Le','<=')]:
        fixtures.extend(['public static void Combined'+n+'() { if (v != int.MaxValue && v '+op+' 0) {} }',
                         'public static void Nested'+n+'() { if (v != int.MaxValue) { if (v '+op+' 0) {} } }'])
    source=folder/'ElidedBranchFixture.cs';source.write_text('public class ElidedBranchFixture { public static int v;\n'+'\n'.join(fixtures)+'\n}')
    binary=folder/'Fixture.dll'
    p=subprocess.run([str(ROOT/COMPILER_PATH),'/nologo','/noconfig','/nostdlib+','/target:library','/optimize+','/debug:pdbonly','/deterministic-','/langversion:6','/unsafe-','/out:'+str(binary),'/r:'+str(UNITY/'Editor/Data/Mono/lib/mono/2.0/mscorlib.dll'),str(source)],capture_output=True)
    (folder/'fixture-compile.log').write_bytes(p.stdout+p.stderr)
    if p.returncode:raise ValueError('Own fixture compilation failed')
    own=dump(binary,folder/'fixture-methods.json');methods={k:m for k,m in own.items() if 'Combined'in k or 'Nested'in k}
    first=next(iter(methods.values()));same=all(body_cells(first)==body_cells(m) for m in methods.values())
    baseline=ROOT/'build/round3'
    original={m['stable_id']:m for m in read(baseline/'KairoLibrary-original-methods.json')['selected_methods']}
    before={m['stable_id']:m for m in read(baseline/'KairoLibrary-rebuilt-methods.json')['selected_methods']}
    existing={k for k,m in original.items() if 'raw_body_hex'in m and not differences(m,before.get(k))[0]}
    mapping=read(ROOT/'analysis/variants/round3-baseline-KairoLibrary/source-map.json')
    src=Path(mapping[LOGICAL]);text=src.read_bytes().decode('utf-8-sig');nl='\r\n' if '\r\n'in text else '\n'
    old=nl.join(['if (statusBarHeight_ != int.MaxValue)','\t\t\t{','\t\t\t\tint statusBarHeight_2 = statusBarHeight_;','\t\t\t\tint num = 0;','\t\t\t}'])
    if text.count(old)!=1:raise ValueError('Expected baseline empty-guard shape')
    replacements={
        'combined':'if (statusBarHeight_ != int.MaxValue && statusBarHeight_ > 0) { }',
        'nested':'if (statusBarHeight_ != int.MaxValue) { if (statusBarHeight_ > 0) { } }'}
    results=[]
    for name,new in replacements.items():
        variant=ROOT/'analysis/variants'/('round3-elided-'+name);variant.mkdir(parents=True,exist_ok=True)
        output=variant/'KairoPlugin.cs';output.write_bytes(text.replace(old,new).encode('utf-8'))
        selected=dict(mapping);selected[LOGICAL]=str(output);mp=variant/'source-map.json';mp.write_text(json.dumps(selected))
        label='round3-elided-'+name
        p=subprocess.run([sys.executable,str(ROOT/'tools/build_kairo.py'),'--compiler',str(ROOT/COMPILER_PATH),'--label',label,'--source-map',str(mp)],capture_output=True,cwd=ROOT)
        (variant/'compile.log').write_bytes(p.stdout+p.stderr)
        if p.returncode:raise ValueError('Frozen target candidate failed')
        built=dump(ROOT/'build'/('KairoLibrary-'+label)/'KairoLibrary.dll',variant/'methods.json')
        delta,_=differences(original[KEY],built[KEY]);lost=[k for k in sorted(existing) if differences(original[k],built.get(k))[0]]
        results.append({'variant':name,'source_before':old,'source_after':new,'logical_path':LOGICAL,
            'candidate_source':str(output.relative_to(ROOT)),'baseline_source_sha256':sha(src),'candidate_source_sha256':sha(output),
            'body_equal':not delta,'differences':delta,'previous_equal_bodies_lost':lost,'instructions':built[KEY]['instructions']})
    candidates=[]
    loads=('ldarg','ldloc','ldfld','ldsfld','ldc.','ldnull','ldstr')
    for k,m in original.items():
        if k in existing or 'raw_body_hex'not in m:continue
        ins=m['instructions'];anchors=[]
        for i,x in enumerate(ins):
            if x['opcode']!='pop':continue
            if i and ins[i-1]['opcode']=='pop':anchors.append({'kind':'adjacent_pops','offset':x['offset'],'context':ins[max(0,i-4):i+2]})
            elif i and ins[i-1]['opcode'].startswith(loads):anchors.append({'kind':'load_then_pop','offset':x['offset'],'context':ins[max(0,i-3):i+2]})
        if anchors:candidates.append({'id':k,'anchors':anchors})
    report={'method':KEY,'compiler':'Microsoft.Net.Compilers1.1.0','baseline_equal_bodies':len(existing),
        'fixture_variants':{k:{'header':m['header'],'instructions':m['instructions']} for k,m in methods.items()},
        'eight_fixture_bodies_identical':same,'target_results':results,
        'historical_relational_operator':'UNKNOWN; the four integral relational operators lower identically in the empty fixtures' if same else 'UNKNOWN',
        'pending_pop_candidates':candidates,'candidate_count':len(candidates),
        'scan_is_diagnostic_not_proof':True,'main_sources_modified':False,'states_modified':False}
    (ROOT/'reports/round3-diagnostic-B.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({'fixture_bodies_identical':same,'target_results':[{'variant':r['variant'],'equal':r['body_equal'],'losses':len(r['previous_equal_bodies_lost'])} for r in results],'pop_candidate_count':len(candidates)}))
if __name__=='__main__':main()
