"""Independent smcs toy fixture. No game source/build/ledger mutation."""
import hashlib,json,os,subprocess,sys
from pathlib import Path
SCRIPT=Path(__file__).resolve().parent
ROOT=next(p for p in SCRIPT.parents if (p/'AGENTS.md').is_file())
HERE=ROOT/'build/round9/control-il-fixtures';HERE.mkdir(parents=True,exist_ok=True)
sys.path.insert(0,str(ROOT/'tools'));sys.path.insert(0,str(SCRIPT))
from round9_control_il import extract_il,canonical_plan,canonical_tree
from compiler_probes import MONO,SMCS,UNITY
SOURCE='''using System;using System.Collections;
public sealed class Disposable:IDisposable { public void Dispose(){} }
public class Fixture {
 public int Field; public static object Gate=new object();
 public void Straight(){Field++;}
 public int Value(int n){int x=n+1;return x;}
 public void If(bool b){if(b){Field++;}Field+=2;}
 public void IfElse(bool b){if(b){Field++;}else{Field+=2;}Field+=3;}
 public void EmptyElse(bool b){if(b){Field++;}else{}Field+=3;}
 public void NoEmptyElse(bool b){if(b){Field++;}Field+=3;}
 public int Guard(int n){if(n!=3){return 1;}return 2;}
 public int GuardElse(int n){if(n!=3){return 1;}else{return 2;}}
 public int While(int n){int total=0;int i=0;while(i<n){total+=i;i++;}return total;}
 public int For(int n){int total=0;int i=0;for(;i<n;i++){total+=i;}return total;}
 public int Do(int n){int i=0;do{i++;}while(i<n);return i;}
 public void Infinite(bool b){while(true){Field++;if(b)break;}}
 public void ForInfinite(bool b){for(;;){Field++;if(b)break;}}
 public int Break(int n){int i=0;while(i<n){if(i==3)break;i++;}return i;}
 public int Continue(int n){int i=0;while(i<n){i++;if(i==3)continue;Field++;}return i;}
 public int ForContinue(int n){int total=0;for(int i=0;i<n;i++){if(i==3)continue;total+=i;}return total;}
 public int ForContinueWork(int n){int total=0;for(int i=0;i<n;i++){if(i==3){total++;continue;}total+=i;}return total;}
 public int ForContinueWorkAsElse(int n){int total=0;for(int i=0;i<n;i++){if(i==3){total++;}else{total+=i;}}return total;}
 public void Ternary(bool b){Field=b?1:2;}
 public void AndValue(bool a,bool b){Field=(a&&b)?3:4;}
 public bool LogicalValue(bool a,bool b){return a&&b;}
 public void AndIf(bool a,bool b){if(a&&b)Field++;}
 public void NestedAndIf(bool a,bool b){if(a){if(b)Field++;}}
 public void OrIf(bool a,bool b){if(a||b)Field++;}
 public int Float(float a){if(a<0f)Field++;while(a<0f){Field++;a=0;}return Field;}
 public int Foreach(int[] values){int result=0;foreach(int value in values)result+=value;return result;}
 public int Enumerable(IEnumerable values){int result=0;foreach(object value in values)result++;return result;}
 public int Switch(int n){switch(n){case 0:return 1;case 1:return 2;case 2:return 3;case 3:return 4;case 4:return 5;default:return 6;}}
 public int SparseSwitch(int n){switch(n){case 10:return 1;case 99:return 2;default:return 3;}}
 public int CommonReturn(int n){int x;if(n>0){x=1;goto exit;}x=2;exit:return x;}
 public int CommonIfElse(int n){int x;if(n>0)x=1;else x=2;return x;}
 public int Protected(int n){try{return n+1;}finally{Field++;}}
 public int Catch(int n){try{return 10/n;}catch(DivideByZeroException){return -1;}}
 public void Using(){using(Disposable d=new Disposable()){Field++;}}
 public void Lock(){lock(Gate){Field++;}}
}
'''
def compile_fixture():
 folder=HERE/'fixtures';folder.mkdir(exist_ok=True);source=folder/'Fixture.cs';source.write_text(SOURCE,encoding='utf-8',newline='\n')
 binary=folder/'Fixture.dll';core=ROOT/'original/extracted/assets/bin/Data/Managed/mscorlib.dll'
 env=os.environ.copy();env['MONO_PATH']=str(UNITY/'Editor/Data/Mono/lib/mono/unity');env['MONO_CFG_DIR']=str(UNITY/'Editor/Data/Mono/etc')
 command=[str(MONO),str(SMCS),'-nologo','-noconfig','-nostdlib','-optimize+','-debug-','-target:library','-out:'+str(binary),'-r:'+str(core),str(source)]
 result=subprocess.run(command,env=env,capture_output=True,text=True,timeout=45);assert result.returncode==0,result.stdout+result.stderr
 dump=subprocess.run([str(ROOT/'build/tools/ProbeInspector.exe'),str(binary),'--all'],capture_output=True,text=True,encoding='utf-8',timeout=30,check=True)
 dto=json.loads(dump.stdout);(folder/'methods.json').write_text(json.dumps(dto,indent=2),encoding='utf-8')
 return dto['selected_methods'],{'source':str(source),'source_sha256':hashlib.sha256(source.read_bytes()).hexdigest(),'compiler':str(SMCS),'compiler_sha256':hashlib.sha256(SMCS.read_bytes()).hexdigest(),'output_sha256':hashlib.sha256(binary.read_bytes()).hexdigest(),'game_compilations':0}

def main():
 methods,provenance=compile_fixture();irs=[extract_il(m) for m in methods];by={m['stable_id'].split('::',1)[1].split('(',1)[0]:m for m in methods if m['stable_id'].startswith('Fixture::')}
 for method,ir in zip(methods,irs):
  assert ir['emission_plan_complete'],(method['stable_id'],ir['reasons'])
  assert ir['coverage']['exact_instruction_coverage'] and ir['coverage']['instruction_count']==ir['coverage']['covered_instruction_count']
  assert ir['coverage']['branch_instruction_count']==len(ir['branch_roles'])
  assert ir['input_unchanged']
  if method['extra_sections']:assert ir['extra_section_validation']['complete'] and ir['extra_section_validation']['all_raw_bytes_accounted']
 eq=[('While','For'),('Guard','GuardElse'),('AndIf','NestedAndIf'),('CommonReturn','CommonIfElse'),('ForContinueWork','ForContinueWorkAsElse')]
 for left,right in eq:assert by[left]['raw_body_hex']==by[right]['raw_body_hex'],(left,right)
 assert by['EmptyElse']['raw_body_hex']!=by['NoEmptyElse']['raw_body_hex']
 assert by['Infinite']['raw_body_hex']!=by['ForInfinite']['raw_body_hex']
 for name in ('ForContinue','ForContinueWork','ForContinueWorkAsElse'):
  ir=next(ir for ir in irs if '::'+name+'(' in ir['method_id'])
  assert not ir['tree_complete'] and any(r['code']=='for-increment-versus-if-join-ambiguous' for r in ir['reasons'])
 explicit=next(ir for ir in irs if '::EmptyElse(' in ir['method_id'])
 assert any(e.get('role')=='explicit-transfer-to-next' for e in explicit['branch_roles'])
 long=[{'kind':'leaf'},{'kind':'branch','opcode':'br','target_label':'end'},{'kind':'label','label':'end'},{'kind':'return','opcode':'ret'}]
 short=[dict(e,opcode='br.s') if e.get('opcode')=='br' else e for e in long]
 removed=[e for e in long if e['kind']!='branch']
 assert canonical_plan(long)!=canonical_plan(short)
 assert canonical_plan(long)!=canonical_plan(removed)
 polarity=[dict(e,opcode='brfalse') if e.get('opcode')=='br' else e for e in long]
 assert canonical_plan(long)!=canonical_plan(polarity)
 floatir=next(ir for ir in irs if '::Float(' in ir['method_id'])
 assert any(e['opcode']=='bge.un' for e in floatir['branch_roles'])
 assert any(e['opcode']=='blt' for e in floatir['branch_roles'])
 rows=[{'method':ir['method_id'],'complete':ir['complete'],'emission_plan_complete':ir['emission_plan_complete'],'tree_complete':ir['tree_complete'],
   'instruction_count':ir['coverage']['instruction_count'],'branch_count':ir['coverage']['branch_instruction_count'],'reasons':[r['code'] for r in ir['reasons']]} for ir in irs]
 report={'provenance':provenance,'method_count':len(irs),'exact_instruction_coverage_count':sum(ir['coverage']['exact_instruction_coverage'] for ir in irs),
   'plan_complete_count':sum(ir['emission_plan_complete'] for ir in irs),'tree_complete_count':sum(ir['tree_complete'] for ir in irs),'rows':rows,
   'full_body_emission_equivalences':[{'left':a,'right':b,'raw_body_equal':True} for a,b in eq],
   'negative_controls':{'empty_else_vs_no_else_different':True,'branch_width_not_normalized':True,'branch_to_next_retained':True,'polarity_not_normalized':True,'float_un_preserved':True,'while_true_vs_for_true_entry_branch_distinguished':True,'work_before_continue_not_misclassified_complete':True},
   'caveat':'Plan completeness is lossless evidence coverage; tree completeness is conservative and separate. Neither replaces the byte verifier.'}
 (HERE/'fixture-report.json').write_text(json.dumps(report,indent=2),encoding='utf-8');(HERE/'fixture-irs.json').write_text(json.dumps(irs,indent=2),encoding='utf-8');print(json.dumps(report,indent=2))
if __name__=='__main__':main()
