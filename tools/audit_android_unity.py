"""Audit installed official Android player variants without decompilation.

Install/download is a separate explicit action. This command only reads the
installed module and original APK extraction, and writes a public report.
"""
import hashlib
import json
from pathlib import Path
from compiler_probes import UNITY

ROOT=Path(__file__).resolve().parents[1]


def sha(path): return hashlib.sha256(path.read_bytes()).hexdigest()


def main():
    module=UNITY/'Editor/Data/PlaybackEngines/AndroidPlayer'
    if not module.is_dir(): raise SystemExit('Android support module is not installed')
    original=ROOT/'original/extracted/assets/bin/Data/Managed/UnityEngine.dll'
    original_bytes=original.read_bytes();original_sha=sha(original)
    records=[]
    for path in sorted(module.rglob('UnityEngine.dll')):
        data=path.read_bytes()
        records.append({'editor_relative_path':path.relative_to(UNITY).as_posix(),
            'size':len(data),'sha256':hashlib.sha256(data).hexdigest(),
            'raw_file_bytes_equal_to_apk':data==original_bytes})
    native_candidates={}
    for path in sorted(module.rglob('*.so')):
        native_candidates.setdefault(sha(path),[]).append(path.relative_to(UNITY).as_posix())
    native_matches=[]
    for path in sorted((ROOT/'original/extracted/lib').rglob('*.so')):
        digest=sha(path)
        native_matches.append({'apk_path':path.relative_to(ROOT/'original/extracted').as_posix(),
            'sha256':digest,'official_module_matches':native_candidates.get(digest,[])})
    before_path=ROOT/'analysis/unity-android-before.json'
    preserved=None
    if before_path.is_file():
        before=json.loads(before_path.read_text())
        changes=[path for path,digest in before.items() if not (UNITY/path).is_file() or sha(UNITY/path)!=digest]
        preserved={'files_checked':len(before),'changed_paths':changes,'all_preserved':not changes}
        if changes: raise ValueError('Existing editor files changed: '+str(changes))
    build_mode_layout={}
    for backend in ('mono','il2cpp'):
        base=module/'Variations'/backend
        build_mode_layout[backend]={'shared_managed_dll':(base/'Managed/UnityEngine.dll').relative_to(UNITY).as_posix(),
            'release_directory_present':(base/'Release').is_dir(),
            'development_directory_present':(base/'Development').is_dir(),
            'mode_specific_UnityEngine_dlls':[p.relative_to(UNITY).as_posix()
                for mode in ('Release','Development') for p in (base/mode).rglob('UnityEngine.dll')]}
    report={'unity_version':'5.3.4f1','status':'OFFICIAL_ANDROID_MONO_BYTE_IDENTICAL',
        'original_sha256':original_sha,'original_size':len(original_bytes),'variants':records,
        'raw_comparison_includes_all_pe_metadata_and_method_bytes':True,
        'normalized_fields':[], 'build_mode_layout':build_mode_layout,'native_library_matches':native_matches,
        'existing_editor_preservation':preserved,
        'installer':{'release_page':'https://unity.com/releases/editor/whats-new/5.3.4f1',
            'url':'https://download.unity3d.com/download_unity/fdbb5133b820/TargetSupportInstaller/UnitySetup-Android-Support-for-Editor-5.3.4f1.exe',
            'bytes':64934904,'sha256':'17ce1e474487363ddcf2b18191410f1216afba3885eb88e928310029bc1523e3',
            'authenticode_status_observed':'Valid','signer':'Unity Technologies SF',
            'signer_certificate_thumbprint':'DCDCEDECA67384D353AC4F6FB870629A880D54C9'},
        'rea_identity_evidence':{'summary_evidence_id':'ev_8bd72c536f5eb55e128d95a5c4216199bb95758d4be460f573449a55c09ac187',
            'parent_evidence_id':'ev_cb07768bdd58b9844204a7f981194b68d1e0ac300d7737ba158d496208e2a0e6',
            'mvid':'8ff1c57a-358d-4cd9-bfaa-aa0ef82c83ff','type_definitions':871,'method_definitions':10459},
        'scope':'Complete byte comparison of official installed engine DLLs; no original or external code executed or decompiled'}
    if not any(item['raw_file_bytes_equal_to_apk'] for item in records):
        report['status']='EXTERNAL_UNVERIFIED'
    (ROOT/'reports/UnityEngine-android-comparison.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8',newline='\n')
    print(json.dumps({'status':report['status'],'variants':records,'native_matches':native_matches,
        'editor_preserved':preserved}))


if __name__=='__main__': main()
