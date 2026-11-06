"""Independent Roslyn1.1 regression fixture: source path, not candidate intent."""
import hashlib,json,subprocess,sys
from pathlib import Path
ROOT=next(p for p in Path(__file__).resolve().parents if (p/'AGENTS.md').is_file())
sys.path.insert(0,str(ROOT/'tools'))
from kairo_candidate import COMPILER_PATH,COMPILER_SHA256,CORELIB_PATH
from compare_kairo import differences
from round8_candidate_sources import write_candidate_sources
FOLDER=ROOT/'build/round8/roslyn-switch-next/sequential-fixture'
BASELINE='public class Fixture { public int Target() { return 1; } public int Other() { return 99; } }'
CANDIDATE='public class Fixture { public int Target() { return 2; } public int Other() { return 99; } }'


def main():
    FOLDER.mkdir(parents=True,exist_ok=True)
    compiler=ROOT/COMPILER_PATH;assert hashlib.sha256(compiler.read_bytes()).hexdigest()==COMPILER_SHA256
    main_path=FOLDER/'sources/Fixture.cs';main_path.parent.mkdir(exist_ok=True)
    target_path=FOLDER/'target-screen/Fixture.cs';target_path.parent.mkdir(exist_ok=True)
    # Same essential source_filter as the engine's target-screen pass: keep the
    # target real and replace another body with an unrelated source stub.
    source_filter=lambda source:source.replace('public int Other() { return 99; }','public int Other() { throw new System.NotImplementedException(); }')
    logical='Fixture.cs';screen_paths={logical:target_path};io_records=[]
    io_records.append(write_candidate_sources(BASELINE,main_path,screen_paths,logical,source_filter))
    compiled=[]
    def compile_screen(label,selected):
        folder=FOLDER/label;folder.mkdir(exist_ok=True);binary=folder/'Fixture.dll'
        flags=['/nologo','/nostdlib+','/target:library','/platform:anycpu','/optimize+','/debug:pdbonly',
            '/deterministic-','/langversion:6','/unsafe-','/checked-','/codepage:65001','/utf8output',
            '/out:'+str(binary),'/reference:'+str(CORELIB_PATH),str(selected)]
        rsp=folder/'compile.rsp';rsp.write_text('\n'.join('"'+a+'"' for a in flags)+'\n',encoding='utf-8')
        process=subprocess.run([str(compiler),'/noconfig','@'+str(rsp)],capture_output=True,text=True,timeout=30)
        assert process.returncode==0,process.stdout+process.stderr
        dump=subprocess.run([str(ROOT/'build/tools/ProbeInspector.exe'),str(binary),'Fixture::Target'],capture_output=True,check=True)
        method=json.loads(dump.stdout.decode('utf-8-sig'))['selected_methods'][0]
        compiled.append({'label':label,'selected_source':str(selected),'selected_source_sha256':hashlib.sha256(selected.read_bytes()).hexdigest(),
            'main_source_sha256':hashlib.sha256(main_path.read_bytes()).hexdigest(),'instructions':method['instructions']})
        return method
    baseline=compile_screen('baseline-check',target_path)
    io_records.append(write_candidate_sources(CANDIDATE,main_path))
    # Reproduces incomplete/sequential engine behavior before the fix.
    stale=compile_screen('sequential-stale-target-screen',target_path)
    assert not differences(baseline,stale)[0]
    assert any(i['opcode']=='ldc.i4.1' for i in stale['instructions'])
    assert compiled[-1]['selected_source_sha256']!=hashlib.sha256(source_filter(CANDIDATE).encode('utf-8')).hexdigest()
    # Proposed path-only correction, immediately before the compiler call.
    io_records.append(write_candidate_sources(CANDIDATE,main_path,screen_paths,logical,source_filter))
    fixed=compile_screen('sequential-synchronized-target-screen',target_path)
    assert differences(baseline,fixed)[0]
    assert any(i['opcode']=='ldc.i4.2' for i in fixed['instructions'])
    assert compiled[-1]['selected_source_sha256']==hashlib.sha256(source_filter(CANDIDATE).encode('utf-8')).hexdigest()
    io_records.append(write_candidate_sources(CANDIDATE,main_path))
    direct=compile_screen('ordinary-path-candidate',main_path)
    assert not differences(fixed,direct)[0]
    # Reset source coherently before the next baseline/symbol request.
    # Shared I/O must preserve exact CRLF/non-ASCII source bytes too.
    unicode_source=CANDIDATE+'\r\n// source control: \u221e\r\n'
    io_records.append(write_candidate_sources(unicode_source,main_path,screen_paths,logical,source_filter))
    assert main_path.read_bytes()==unicode_source.encode('utf-8')
    assert target_path.read_bytes()==source_filter(unicode_source).encode('utf-8')
    rejected=False
    try:write_candidate_sources(CANDIDATE,main_path,{logical:main_path},logical,source_filter)
    except ValueError:rejected=True
    assert rejected,'same-path filtering must be rejected'
    io_records.append(write_candidate_sources(BASELINE,main_path,screen_paths,logical,source_filter))
    reset=compile_screen('explicit-next-baseline-reset',target_path)
    assert not differences(baseline,reset)[0]
    report={'operation':'historical-Roslyn-sequential-screen-source-regression','compiler_sha256':COMPILER_SHA256,
        'toy_compilations':5,'game_compilations':0,'stale_build_equals_baseline':True,'synchronized_build_contains_candidate':True,
        'direct_and_synchronized_target_bodies_equal':True,'next_baseline_reset_equals_original':True,
        'body_oracle_changed':False,'records':compiled,'shared_io_records':io_records,
        'shared_io_sha256':hashlib.sha256((ROOT/'tools/round8_candidate_sources.py').read_bytes()).hexdigest(),
        'CRLF_and_unicode_preserved':True,'filtered_same_path_rejected':True}
    (FOLDER/'fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({k:report[k] for k in ('toy_compilations','game_compilations','stale_build_equals_baseline',
        'synchronized_build_contains_candidate','direct_and_synchronized_target_bodies_equal','next_baseline_reset_equals_original')}))


if __name__=='__main__':main()
