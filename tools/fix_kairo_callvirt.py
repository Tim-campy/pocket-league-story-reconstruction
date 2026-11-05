"""Apply the diagnosed C# callvirt repairs, one original method at a time.

Only source is edited. Every attempted change builds/verifies ALL KairoLibrary
methods and is rolled back unless its target body matches and no prior match
regresses. Re-running skips targets already confirmed by the verifier.
"""
import argparse
import hashlib
import json
from pathlib import Path
import re
import subprocess
import sys
import time

ROOT=Path(__file__).resolve().parents[1]
BASE=ROOT/'src/KairoLibrary/kairo/unity'
PLANS=[
    ('kairo.unity.util.SecureByteArray::ToArray(kairo.unity.util.SecureByteArray)', 'util/SecureByteArray.cs',
     'public static byte[] ToArray(SecureByteArray ary)', 'null_return', 'Get'),
    ('kairo.unity.ui.ResourceManager::Load(System.Byte[],System.Int32)', 'ui/ResourceManager.cs',
     'public void Load(byte[] data, int m3dVer = 3)', 'null_void', 'Close'),
    ('kairo.unity.ui.ResourceManager::Load(System.String,System.Int32)', 'ui/ResourceManager.cs',
     'public void Load(string directory, int m3dVer = 3)', 'null_void', 'Close'),
    ('kairo.unity.ui.ResourceManager::Load(System.Int32,System.Int32,System.Int32)', 'ui/ResourceManager.cs',
     'public void Load(int rsId, int rcId, int m3dVer = 3)', 'null_void', 'Close'),
    ('kairo.unity.ui.ResourceManager::LoadReady(System.Byte[])', 'ui/ResourceManager.cs',
     'public void LoadReady(byte[] data)', 'null_void', 'Close'),
    ('kairo.unity.ui.ResourceManager::LoadStart(System.Byte[],System.Int32)', 'ui/ResourceManager.cs',
     'public void LoadStart(byte[] data, int m3dVer = 3)', 'null_void', 'Close'),
    ('kairo.unity.system.lineup.ui.LineupManager::GetLineups()', 'system/lineup/ui/LineupManager.cs',
     'public LineupData[] GetLineups()', 'null_void', 'Close'),
    ('kairo.unity.native.KairoPlugin/<>c__DisplayClass89_0::<LaunchLobi>b__0()', 'native/KairoPlugin.cs',
     'public static void LaunchLobi(string data, int type, int number)', 'new_android_class', ''),
    ('kairo.unity.m3d4.util3d.Transform::Rotate(System.Single,System.Single,System.Single,System.Single)', 'm3d4/util3d/Transform.cs',
     'public void Rotate(float x, float y, float z, float angle)', 'new_vector', ''),
    ('kairo.unity.io.RecordStore::PreferencesToFile()', 'io/RecordStore.cs',
     'public static void PreferencesToFile()', 'null_void', 'CloseRecordStore'),
    ('kairo.unity.io.RecordStore::FileToPreferences()', 'io/RecordStore.cs',
     'public static void FileToPreferences()', 'null_void', 'CloseRecordStore'),
    ('kairo.unity.io.RecordStore::WriteRecord(System.Int32,System.Int32,System.Byte[])', 'io/RecordStore.cs',
     'public static void WriteRecord(int rsId, int rcId, byte[] data)', 'null_void', 'CloseRecordStore'),
    ('kairo.unity.io.RecordStore::ReadRecord(System.Int32,System.Int32)', 'io/RecordStore.cs',
     'public static byte[] ReadRecord(int rsId, int rcId)', 'null_void', 'CloseRecordStore'),
    ('kairo.unity.io.Storage::Read(System.Int32,System.Boolean,System.String)', 'io/Storage.cs',
     'public static byte[] Read(int media, bool security, string filename)', 'null_void', 'Close'),
    ('kairo.unity.io.Storage::Write(System.Int32,System.Boolean,System.String,System.Byte[])', 'io/Storage.cs',
     'public static void Write(int media, bool security, string filename, byte[] data)', 'null_void', 'Close'),
    ('kairo.unity.io.Storage::Exists(System.Int32,System.String)', 'io/Storage.cs',
     'public static bool Exists(int media, string filename)', 'null_void', 'Close'),
    ('kairo.unity.form.FormManagerBase::OnDraw()', 'form/FormManagerBase.cs',
     'public virtual void OnDraw()', 'null_void', 'ClearTouchComponent|BeginTouchRegist|EndTouchRegist'),
    ('kairo.unity.form.FormManagerBase::Capture(kairo.unity.surface.SurfaceBase,System.String[],kairo.unity.form.FormManagerBase/OnCaptureCompleted,System.Boolean)', 'form/FormManagerBase.cs',
     'public void Capture(SurfaceBase view, string[] captureTables = null, OnCaptureCompleted method = null, bool saveToTmpDirectory = false)', 'new_thread', ''),
    ('kairo.unity.form.FormManagerBase/<>c__DisplayClass85_1::<Capture>b__3()', 'form/FormManagerBase.cs',
     'public void Capture(SurfaceBase view, string[] captureTables = null, OnCaptureCompleted method = null, bool saveToTmpDirectory = false)', 'null_void', 'Dispose'),
]


def sha(data):return hashlib.sha256(data).hexdigest()


def mask_non_code(text):
    pattern=r'//[^\r\n]*|/\*[\s\S]*?\*/|@"(?:[^"]|"")*"|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\''
    return re.sub(pattern,lambda m:''.join('\n' if c=='\n' else '\r' if c=='\r' else ' ' for c in m.group()),text)


def edit_method(text,header,kind,called):
    if text.count(header)!=1:raise ValueError('Method header not unique: '+header)
    masked=mask_non_code(text);start=text.index(header);opening=masked.index('{',start+len(header));depth=0;end=None
    for i in range(opening,len(masked)):
        depth+=(masked[i]=='{')-(masked[i]=='}')
        if depth==0:end=i+1;break
    if end is None:raise ValueError('Unclosed method')
    body=text[start:end];newline='\r\n' if '\r\n' in text else '\n'
    if kind=='null_void':
        pattern=r'(?m)^([\t ]*)([A-Za-z_]\w*)\?\.('+called+r')\(([^\r\n]*)\);'
        def expand(match):
            indent,receiver,method,args=match.groups()
            return newline.join([indent+'if ('+receiver+' != null)',indent+'{',indent+'\t'+receiver+'.'+method+'('+args+');',indent+'}'])
        body,count=re.subn(pattern,expand,body)
        expected=3 if 'ClearTouchComponent' in called else 1
        if count!=expected:raise ValueError('Unexpected null-conditional site count '+str(count))
    elif kind=='null_return':
        old='return ary?.Get();'
        new=newline.join(['if (ary == null)','\t\t\t{','\t\t\t\treturn null;','\t\t\t}','\t\t\treturn ary.Get();'])
        if body.count(old)!=1:raise ValueError('Expected one return null-conditional')
        body=body.replace(old,new)
    elif kind=='new_android_class':
        old='AndroidJavaObject androidJavaObject2 = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");'
        new=newline.join(['AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");',
            '\t\t\t\t\tAndroidJavaObject androidJavaObject2 = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");'])
        if body.count(old)!=1:raise ValueError('Expected one inline UnityPlayer construction')
        body=body.replace(old,new)
    elif kind=='new_vector':
        old='new Vector3D(x, y, z).Normalize();'
        new='Vector3D axis = new Vector3D(x, y, z);'+newline+'\t\t\taxis.Normalize();'
        if body.count(old)!=1:raise ValueError('Expected one inline Vector3D construction')
        body=body.replace(old,new)
    elif kind=='new_thread':
        if body.count('new JThread(delegate')!=1 or body.count('}).Start();')!=1:raise ValueError('Expected one inline JThread construction')
        body=body.replace('new JThread(delegate','JThread captureThread = new JThread(delegate',1)
        body=body.replace('}).Start();','});'+newline+'\t\t\tcaptureThread.Start();',1)
    else:raise ValueError('Unknown repair kind')
    return text[:start]+body+text[end:]


def run_verifier():
    for attempt in range(3):
        result=subprocess.run([sys.executable,str(ROOT/'tools/verify'),'--all','--assembly','KairoLibrary'],
            cwd=ROOT,capture_output=True,text=True,encoding='utf-8',errors='replace')
        completed=False
        for line in result.stdout.splitlines():
            try:summary=json.loads(line)
            except ValueError:continue
            completed|=summary.get('assembly')=='KairoLibrary' and 'counts' in summary
        if result.returncode in (0,1) and completed:
            receipt=json.loads((ROOT/'build/verification-KairoLibrary.json').read_text(encoding='utf-8'))
            if sha((ROOT/receipt['build_artifact']).read_bytes())!=receipt['build_sha256']:
                raise RuntimeError('Verification artifact changed')
            if any(sha((ROOT/source['path']).read_bytes())!=source['sha256'] for source in receipt['sources']):
                raise RuntimeError('Stale verification receipt after source edit')
            return receipt,result.stdout+result.stderr
        if attempt<2:time.sleep(1)
    raise RuntimeError('Verifier infrastructure failed: '+(result.stdout+result.stderr)[-3500:])


def main():
    parser=argparse.ArgumentParser(description=__doc__);parser.add_argument('--apply',action='store_true')
    parser.add_argument('--only',help='Stable-ID prefix from a plan');args=parser.parse_args()
    if not args.apply:
        print(json.dumps({'plans':len(PLANS),'action':'No source edits; pass --apply explicitly'}));return
    diagnosis=json.loads((ROOT/'reports/KairoLibrary-callvirt-diagnostic.json').read_text(encoding='utf-8'))
    by_stable={m['stable_id']:m['id'] for m in diagnosis['methods']}
    journal_path=ROOT/'reports/KairoLibrary-callvirt-repairs.json'
    journal=json.loads(journal_path.read_text(encoding='utf-8')) if journal_path.is_file() else {'attempts':[]}
    for prefix,relative,header,kind,called in PLANS:
        if args.only and not prefix.startswith(args.only):continue
        keys=[key for key in by_stable if key.startswith(prefix+'->')]
        if len(keys)!=1:raise ValueError('Missing/ambiguous diagnosed target '+prefix)
        key=keys[0];identifier=by_stable[key]
        before=json.loads((ROOT/'build/verification-KairoLibrary.json').read_text(encoding='utf-8'))
        if before['methods'][identifier]['state']=='matched':
            print(json.dumps({'method':identifier,'status':'already_matched'}),flush=True);continue
        path=BASE/relative;original=path.read_bytes();text=original.decode('utf-8')
        edited=edit_method(text,header,kind,called).encode('utf-8')
        if edited==original:raise ValueError('Repair produced no change')
        path.write_bytes(edited)
        try:
            after,log=run_verifier();target=after['methods'][identifier]
            previous={k for k,v in before['methods'].items() if v['state']=='matched'}
            regressions=sorted(k for k in previous if after['methods'][k]['state']!='matched')
            accepted=target['state']=='matched' and not regressions and not after['regressions']
        except BaseException:
            path.write_bytes(original);run_verifier();raise
        attempt={'id':identifier,'stable_id':key,'source':path.relative_to(ROOT).as_posix(),'repair':kind,
            'before_source_sha256':sha(original),'after_source_sha256':sha(edited),
            'accepted':accepted,'target_state':target['state'],'target_differences':target['differences'],
            'regressions':regressions,'matched_before':before['counts']['matched'],
            'matched_after':after['counts']['matched'],'verification_log_tail':log.splitlines()[-2:]}
        if not accepted:
            path.write_bytes(original);restored,_=run_verifier();attempt['rollback_verified']=not restored['regressions']
        journal['attempts'].append(attempt)
        journal['accepted_count']=sum(a['accepted'] for a in journal['attempts'])
        journal_path.write_text(json.dumps(journal,indent=2)+'\n',encoding='utf-8',newline='\n')
        print(json.dumps(attempt),flush=True)
        if not accepted:raise SystemExit('Repair rejected and source restored; inspect the recorded difference')


if __name__=='__main__':main()
