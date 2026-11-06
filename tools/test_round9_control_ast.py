"""Toy-only validation. AST extractor receives source only, never the oracle DTO."""
import json,sys,os,subprocess,collections
from pathlib import Path
sys.path.insert(0,str(Path(__file__).resolve().parent))
from round9_control_ast import ROOT,WIP,Client,SOURCE as AST_SOURCE
from round9_control_il import canonical_plan,extract_il,canonical_tree
from compare_kairo import sha
SOURCE=r"""class Fixture {
 static int state;static void U(int x){state+=x;}static bool B(){return state>0;}
 public static void IfNoElse(int x){if(x>0)U(x);}
 public static void IfElse(int x){if(x>0)U(x);else U(-x);}
 public static int Early(bool b){if(b)return 1;return 2;}
 public static void DuplicateRet(bool b){if(!b)return;}
 public static int Null(object x){if(x==null)return 1;return 2;}
 public static int NotNull(object x){if(x!=null)return 1;return 2;}
 public static int Zero(int x){if(x==0)return 1;return 2;}
 public static int BoolTrue(bool x){if(x==true)return 1;return 2;}
 public static int StringCompare(string x,string y){if(x==y)return 1;return 2;}
 public static int StringNull(string x){if(x==null)return 1;return 2;}
 public static int FloatLt(float x,float y){if(x<y)return 1;return 2;}
 public static int FloatNegated(float x,float y){if(!(x<y))return 1;return 2;}
 public static int UInt(uint x,uint y){if(x<y)return 1;return 2;}
 public static int BytePromotion(byte x,byte y){if(x<y)return 1;return 2;}
 public static int LogicalAnd(bool a,bool b){if(a&&b)return 1;return 2;}
 public static int LogicalOr(bool a,bool b){if(a||b)return 1;return 2;}
 public static bool LogicalValueAnd(bool a,bool b){return a&&b;}
 public static bool LogicalValueOr(bool a,bool b){return a||b;}
 public static int Ternary(bool x,int a,int b){return x?a:b;}
 public static void TernaryArg(bool x,int a,int b){U(x?a:b);}
 public static int While(int x){while(x>0){x--;if(x==2)continue;if(x==1)break;}return x;}
 public static int For(int x){for(int i=0;i<x;i++){if(i==2)continue;if(i==3)break;U(i);}return x;}
 public static int Do(int x){do{x--;if(x==2)continue;if(x==1)break;}while(x>0);return x;}
 public static int Array(int[] array){int result=0;foreach(int x in array){if(x==2)continue;if(x==3)break;result+=x;}return result;}
 public static int Infinite(int x){while(true){return x;}}
 public static void EmptyIf(bool x){if(x){}else{}U(1);}
 public static void ConstantIf(){if(true)U(1);else U(2);}
 public static void ZeroLoop(){while(false)U(1);}
 public static int DecimalEq(decimal x,decimal y){if(x==y)return 1;return 2;}
 public static int DecimalLt(decimal x,decimal y){if(x<y)return 1;return 2;}
 public delegate void D();public static int DelegateEq(D x,D y){if(x==y)return 1;return 2;}
 public static bool ConstantAnd(bool x){return x&&false;}
 public static void OnlyConst(){const int x=1;}
 public static void TryFinally(){try{U(1);}finally{U(2);}}
 public static void Switch(int x){switch(x){case 1:U(1);break;case 2:U(2);break;}}
}"""
def raw_validation_plan(method):
 # Lossless transfer projection only for fixture comparison; no tree inference.
 instructions=method['instructions'];targets={t for i in instructions for t in (([i['operand']['branch']] if isinstance(i.get('operand'),dict) and 'branch'in i['operand'] else i.get('operand',{}).get('switch',[]) if isinstance(i.get('operand'),dict) else []))};events=[]
 for i in instructions:
  offset=i['offset'];op=i['opcode'];operand=i.get('operand')
  if offset in targets:events.append({'kind':'label','label':'IL_'+str(offset)})
  if isinstance(operand,dict) and 'branch'in operand:events.append({'kind':'branch','opcode':op,'target_label':'IL_'+str(operand['branch'])})
  elif isinstance(operand,dict) and 'switch'in operand:events.append({'kind':'branch','opcode':op,'target_labels':['IL_'+str(x) for x in operand['switch']]})
  elif op=='ret':events.append({'kind':'return','opcode':op})
  elif op in ('throw','rethrow'):events.append({'kind':'throw','opcode':op})
  elif op in ('endfinally','endfilter','jmp'):events.append({'kind':'terminal','opcode':op})
  else:events.append({'kind':'leaf'})
 return events

def main():
 folder=WIP/'fixture';folder.mkdir(exist_ok=True);source=folder/'Fixture.cs';source.write_text(SOURCE,encoding='utf-8',newline='\n');base=json.loads((ROOT/'build/round9/cycles/a05/after/Assembly-CSharp/build-manifest.json').read_text(encoding='utf-8'));mp=folder/'manifest.json';mp.write_text(json.dumps(dict(base,sources=[{'path':str(source),'input_path':str(source)}])),encoding='utf-8')
 with Client(mp)as c:ast=c.extract(source,SOURCE)
 # Capture independent predictions before compiling or inspecting the fixture.
 (folder/'predictions-before-compilation.json').write_text(json.dumps(ast,indent=2),encoding='utf-8')
 binary=folder/'Fixture.dll';args=[f for f in base['flags'] if not f.startswith('-out:') and f!='-noconfig']+['-out:'+str(binary)]+['-r:'+r['path'] for r in base['references']]+[str(source)];rsp=folder/'compile.rsp';rsp.write_text('\n'.join('"'+a+'"' for a in args),encoding='utf-8');env=os.environ.copy();env.update(base['scoped_environment']);proc=subprocess.run([base['runtime'],base['compiler'],'-noconfig','@'+str(rsp)],env=env,cwd=ROOT,capture_output=True);(folder/'compiler.log').write_bytes(proc.stdout+proc.stderr)
 if proc.returncode:raise RuntimeError(proc.stdout+proc.stderr)
 raw=subprocess.check_output([str(ROOT/'build/tools/ProbeInspector.exe'),str(binary),'--all']);il={m['stable_id']:m for m in json.loads(raw.decode('utf-8-sig'))['selected_methods']};rows=[]
 for ir in ast:
  key=ir['method_id'];m=il.get(key)
  if not m:continue
  if ir.get('emission_plan_complete',False):
   left,right=canonical_plan(ir['emission_plan']),canonical_plan(raw_validation_plan(m));equal=left==right
   row={'method_id':key,'complete':ir['complete'],'plan_complete':True,'tree_complete':ir.get('tree_complete',False),'equal':equal,'source_plan':left,'IL_plan':right}
  else:row={'method_id':key,'complete':False,'plan_complete':False,'tree_complete':ir.get('tree_complete',False),'reasons':ir['reasons']}
  il_ir=extract_il(m)
  row['paired_tree_complete']=bool(ir.get('tree_complete')and il_ir.get('tree_complete'))
  if row['paired_tree_complete']:
   left_tree,right_tree=canonical_tree(ir),canonical_tree(il_ir);row['trees_equal']=left_tree is not None and right_tree is not None and left_tree==right_tree
   if not row['trees_equal']:row.update(source_tree=left_tree,IL_tree=right_tree)
  rows.append(row);print(key,'complete',row['complete'],'equal',row.get('equal'),'reasons',[x['code'] for x in row.get('reasons',[])])
 result={'operation':'independent-AST-control-plan-fixture-validation','game_compilations':0,'fixture_compilations':1,'AST_predictions_captured_before_fixture_compilation':True,'complete':sum(x['complete']for x in rows),'plan_complete':sum(x['plan_complete']for x in rows),'paired_tree_complete':sum(x.get('paired_tree_complete',False)for x in rows),'trees_equal':sum(x.get('trees_equal',False)for x in rows),'equal':sum(x.get('equal',False)for x in rows),'wrong_complete':sum(x['plan_complete']and not x.get('equal')for x in rows),'rows':rows,'source_sha256':sha(source),'AST_source_sha256':sha(AST_SOURCE)};(WIP/'fixture-validation.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in result.items()if k!='rows'}))
if __name__=='__main__':main()
