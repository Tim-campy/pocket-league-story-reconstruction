"""Compile bounded C# probes and compare complete selected bodies with Cecil.

Probes establish compatibility, not the identity of the historical compiler.
They do not promote game methods to matched or edit compiled assemblies.
"""
import hashlib
import json
import os
from pathlib import Path
import subprocess

ROOT=Path(__file__).resolve().parents[1]
UNITY=Path(os.environ.get('UNITY_ROOT',str(Path.home()/'AppData/Local/Programs/Unity/5.3.4f1')))
MONO=UNITY/'Editor/Data/Mono/bin/mono.exe'
GMCS=UNITY/'Editor/Data/Mono/lib/mono/2.0/gmcs.exe'
SMCS=UNITY/'Editor/Data/Mono/lib/mono/unity/smcs.exe'
CSC=Path('C:/Windows/Microsoft.NET/Framework64/v4.0.30319/csc.exe')
def find_roslyn():
    if os.environ.get('ROSLYN_CSC'): return Path(os.environ['ROSLYN_CSC'])
    locator=Path(os.environ.get('ProgramFiles(x86)','C:/Program Files (x86)'))/'Microsoft Visual Studio/Installer/vswhere.exe'
    if locator.is_file():
        result=subprocess.run([str(locator),'-products','*','-find','MSBuild/**/Roslyn/csc.exe'],capture_output=True,text=True,timeout=20)
        paths=[Path(line) for line in result.stdout.splitlines() if line.strip()]
        if paths: return paths[-1]
    return Path('C:/Program Files/Microsoft Visual Studio/18/Community/MSBuild/Current/Bin/Roslyn/csc.exe')


ROSLYN=find_roslyn()

CSHARP='''namespace data {
 public class BaseData {
  private int flag_;
  public bool Check(int mask) { return (flag_ & mask) != 0; }
 }
 public class EventData {
  public static bool CheckScript(int[][] script, int id) {
   for (int i=0; i<script.Length; i++) { if (script[i][0]==id) return true; }
   return false;
  }
 }
 public class FanActData {
  private int rank_,rankPoint_,state_,stateCnt_; private bool new_;
  public void Init() { rank_=0; rankPoint_=0; state_=0; stateCnt_=0; new_=false; }
 }
 public class FanBonusData {
  private int state_; private int[] achiveDay_;
  public void Init() { state_=0; achiveDay_=new int[3]; }
 }
}
'''
KAIRO='''[assembly:System.Reflection.AssemblyVersion("1.0.0.0")]
namespace kairo.unity.util {
 public class BitUtil {
  public static bool Check(long value,long mask) { return (value & mask)!=0L; }
 }
 public class Compressor {
  private static byte[] bitBuffer_; private static int bitPoint_;
  private static void _setBitBuffer(byte[] buffer) { bitBuffer_=buffer; bitPoint_=0; }
 }
}
namespace java.lang {
 public class JMath { public static int Abs(int value) { if(value<0) return -value; return value; } }
}
namespace kairo.unity.math {
 public class KairoMath {
  public static bool CheckHitPointRect(int x,int y,int rx,int ry,int width,int height) {
   return rx<x && x<rx+width && ry<y && y<ry+height;
  }
 }
}
'''
SELECTORS={
 'Assembly-CSharp':['data.BaseData::Check','data.EventData::CheckScript','data.FanActData::Init','data.FanBonusData::Init'],
 'KairoLibrary':['kairo.unity.util.BitUtil::Check','kairo.unity.util.Compressor::_setBitBuffer','java.lang.JMath::Abs','kairo.unity.math.KairoMath::CheckHitPointRect']}
KEYS={
 'Assembly-CSharp':['data.BaseData::Check(System.Int32)','data.EventData::CheckScript(System.Int32[][],System.Int32)',
                    'data.FanActData::Init()','data.FanBonusData::Init()'],
 'KairoLibrary':['kairo.unity.util.BitUtil::Check(System.Int64,System.Int64)',
                'kairo.unity.util.Compressor::_setBitBuffer(System.Byte[])','java.lang.JMath::Abs(System.Int32)',
                'kairo.unity.math.KairoMath::CheckHitPointRect(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)']}
COMPARE=('signature','header','locals','instructions','exception_regions','extra_sections','method_attributes','impl_attributes')


def inspect(path,assembly):
    p=subprocess.run([str(ROOT/'build/tools/ProbeInspector.exe'),str(path),*SELECTORS[assembly]],capture_output=True,text=True,encoding='utf-8',errors='strict',timeout=30)
    if p.returncode: raise RuntimeError(p.stderr)
    result=json.loads(p.stdout)
    return {m['key']:m for m in result['selected_methods'] if m['key'] in KEYS[assembly]}


def compare(original,candidate):
    results=[]
    for key,reference in original.items():
        built=candidate.get(key)
        differences=list(COMPARE) if built is None else [k for k in COMPARE if reference[k]!=built[k]]
        results.append({'method':key,'resolved_body_equal':not differences,'different_facets':differences,
            'raw_body_equal':built is not None and reference['raw_body_hex']==built['raw_body_hex'],
            'original_token':reference['token'],'candidate_token':None if built is None else built['token']})
    return results


def main():
    output=ROOT/'build/compiler-probes';output.mkdir(parents=True,exist_ok=True)
    reference_dir=ROOT/'original/extracted/assets/bin/Data/Managed'
    candidates=[('mono-gmcs',[str(MONO),str(GMCS)],False),('mono-smcs',[str(MONO),str(SMCS)],False),
                ('framework-csc',[str(CSC)],True),('roslyn-csc',[str(ROSLYN)],True)]
    results=[]
    for assembly,source in [('Assembly-CSharp',CSHARP),('KairoLibrary',KAIRO)]:
        original=inspect(reference_dir/(assembly+'.dll'),assembly)
        if len(original)!=4: raise ValueError('Expected four original methods from distinct types')
        source_path=output/(assembly+'.cs');source_path.write_text(source,encoding='utf-8',newline='\n')
        for name,driver,microsoft in candidates:
            if not all(Path(p).is_file() for p in driver): continue
            for optimize in (True,False):
                folder=output/assembly/(name+('-opt' if optimize else '-noopt'));folder.mkdir(parents=True,exist_ok=True)
                target=folder/(assembly+'.dll')
                corelib=reference_dir/'mscorlib.dll' if assembly=='Assembly-CSharp' else UNITY/'Editor/Data/Mono/lib/mono/2.0/mscorlib.dll'
                flags=['-nologo','-noconfig','-nostdlib','-target:library','-debug-',
                    '-optimize'+('+' if optimize else '-'),'-out:'+str(target),'-r:'+str(corelib)]
                if microsoft: flags+=['-langversion:3']
                command=driver+flags+[str(source_path)]
                environment=os.environ.copy()
                if not microsoft and assembly=='Assembly-CSharp':
                    environment['MONO_PATH']=str(UNITY/'Editor/Data/Mono/lib/mono/unity')
                    environment['MONO_CFG_DIR']=str(UNITY/'Editor/Data/Mono/etc')
                p=subprocess.run(command,capture_output=True,text=True,errors='replace',timeout=45,env=environment)
                item={'assembly':assembly,'candidate':name,'optimize':optimize,'compiler_exit':p.returncode,
                    'compiler_driver':driver,'flags':flags,'source_sha256':hashlib.sha256(source_path.read_bytes()).hexdigest(),
                    'scoped_environment':{k:environment[k] for k in ('MONO_PATH','MONO_CFG_DIR') if k in environment}}
                (folder/'compiler.log').write_text(p.stdout+p.stderr,encoding='utf-8')
                if p.returncode: item['error']=(p.stdout+p.stderr)[-2000:]
                else:
                    built=inspect(target,assembly);item['methods']=compare(original,built)
                    item['passed']=sum(m['resolved_body_equal'] for m in item['methods'])
                    (folder/'inspected.json').write_text(json.dumps(built,indent=2)+'\n',encoding='utf-8')
                results.append(item)
                print(json.dumps({k:item[k] for k in ('assembly','candidate','optimize','compiler_exit','passed') if k in item}),flush=True)
        (output/(assembly+'-original.json')).write_text(json.dumps(original,indent=2)+'\n',encoding='utf-8')
    report={'scope':'four selected methods per target DLL; complete assemblies not rebuilt',
        'historical_compiler_identity':'UNKNOWN; probes prove compatibility only','normalized_assembly_fields':[],
        'matched_game_methods':0,'attempts':results}
    (ROOT/'analysis/compiler-probe-results.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    return 0


if __name__=='__main__': raise SystemExit(main())
