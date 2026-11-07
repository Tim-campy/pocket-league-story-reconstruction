"""Toy-only validation. AST extractor receives source only, never the oracle DTO."""
import json,sys,os,subprocess,collections
from pathlib import Path
sys.path.insert(0,str(Path(__file__).resolve().parent))
from round10_control_ast import ROOT,WIP,Client,SOURCE as AST_SOURCE
from round10_control_il import canonical_plan,extract_il,canonical_tree
import round10_control_il as cil
cil.CONTROL_STAGE=int(os.environ.get('REA_CONTROL_STAGE','1'))
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
 public static void And3(float a,float b,float c){if(a!=0f&&b>0f&&c>=a)U(1);}
 public static void And3WithFor(bool a,bool b,bool c){if(a&&b&&c){for(int i=0;i<3;i++)U(i);}}
 public static void And3WithForeach(bool a,bool b,bool c,int[] array){if(a&&b&&c){foreach(int x in array)U(x);}}
 public static void WhileOr(int x){while(x>0||B()){x--;if(x<-10)break;U(x);}}
 public static void WhileOr3(int x,int y){while(x>0||y>0||B()){x--;y--;if(x<-10)break;U(x);}}
 public static void WhileNotAnd(float x,float y){while(!(x<y&&B())){x-=1f;if(x<-10f)break;U((int)x);}}
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
 public static void UnnamedCatch(){U(0);try{U(1);}catch(System.Exception){}U(2);}
 public static void NamedCatch(){try{U(1);}catch(System.Exception e){U(e.Message.Length);}}
 public static void MultiCatch(){try{U(1);}catch(System.ArgumentException e){U(e.Message.Length);}catch(System.Exception){U(2);}}
 public static int ReturnFinally(int x){try{return x+1;}finally{U(2);}}
 public static int ReturnCatch(int x){try{return x+1;}catch(System.Exception){return -1;}}
 public static void ReturnVoidFinally(bool x){try{if(x)return;U(1);}finally{U(2);}}
 public static void WhileTry(int x){while(x-->0){try{U(x);}catch(System.Exception){U(-x);}}}
 public static void ForTry(int x){for(int i=0;i<x;i++){try{U(i);}finally{U(-i);}}}
 public static void InsideTryWhile(int x){try{while(x-->0){U(x);}}catch(System.Exception){U(-1);}}
 public static void OutsideInsideWhile(int x){while(x-->0){try{for(int i=0;i<x;i++){U(i);}}catch(System.Exception){U(-1);}}}
 public static void IfTry(bool x){if(x){try{U(1);}catch(System.Exception){U(2);}}}
 public static void NestedEH(){try{try{U(1);}catch(System.Exception){U(2);}}finally{U(3);}}
 public static void CombinedEH(){try{U(1);}catch(System.Exception){U(2);}finally{U(3);}}
 public static void TerminalFinally(){try{U(1);}finally{throw new System.Exception();}}

 public static void Switch(int x){switch(x){case 1:U(1);break;case 2:U(2);break;}}
 public static void SwitchBreakTail(int x){for(int i=0;i<x;i++){switch(i){case 0:U(1);break;case 1:U(2);break;case 2:U(3);break;}}}
 public static void SwitchContinueTail(int x){for(int i=0;i<x;i++){switch(i){case 0:U(1);continue;case 1:U(2);continue;case 2:U(3);continue;}}}
 public static void SwitchBreakMid(int x){for(int i=0;i<x;i++){switch(i){case 0:U(1);break;case 1:U(2);break;case 2:U(3);break;}U(99);}}
 public static void SwitchContinueMid(int x){for(int i=0;i<x;i++){switch(i){case 0:U(1);continue;case 1:U(2);continue;case 2:U(3);continue;}U(99);}}
 public static void Dense(int x){switch(x){case 0:U(1);break;case 1:U(2);break;case 2:U(3);break;}}
 public static void DenseGap(int x){switch(x){case 3:U(1);break;case 5:U(2);break;}}
 public static void DenseNegative(int x){switch(x){case -3:U(1);break;case -2:U(2);break;case -1:U(3);break;}}
 public static int DenseReturn(int x){switch(x){case 0:return 1;case 1:U(2);break;case 2:return 3;}return 0;}
 public static void Sparse(int x){switch(x){case 0:U(1);break;case 800:U(2);break;case 1500:U(3);break;}}
 public static void Mixed(int x){switch(x){case -100:U(1);break;case 4:case 5:U(2);break;case 6:U(3);break;default:U(4);break;}}
 public static void StringSwitch(string x){switch(x){case "a":U(1);break;case "b":case "c":U(2);break;case null:U(3);break;default:U(4);break;}}
 public static void StringOne(string x){switch(x){case "a":U(1);break;}}
 public static void DefaultOnly(int x){switch(x){default:U(1);break;}}
 public static void ConstantSwitch(){switch(1){case 0:U(0);break;case 1:U(1);break;}}

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

def eh_entry_edges(ir,method):
 # Read-only fixture proof. EH markers/labels have zero instruction width.
 # This is not used by either independent extractor or canonical_tree.
 plan=ir['emission_plan'];position=0;labels={};regions={};branches=[]
 for event in plan:
  kind=event['kind']
  if kind=='label':labels[event['label']]=position
  elif kind.startswith('eh_'):
   key=(event.get('source_node'),event.get('handler_kind'),event.get('catch_type'))
   regions.setdefault(key,{})[event['region_role']]=position
  else:
   if kind=='branch':branches.append((position,event))
   position+=1
 source=[]
 for ordinal,(pos,event) in enumerate(branches):
  for target in ([event['target_label']] if 'target_label'in event else event.get('target_labels',[])):
   for key,region in regions.items():
    if labels.get(target)==region.get('try_start'):
     source.append((ordinal,key[1],key[2],not(region['try_start']<=pos<region['try_end'])))
 actual=[];rawbranches=[i for i in method['instructions'] if isinstance(i.get('operand'),dict)and('branch'in i['operand']or'switch'in i['operand'])]
 for ordinal,ins in enumerate(rawbranches):
  targets=[ins['operand']['branch']]if'branch'in ins['operand']else ins['operand']['switch']
  for target in targets:
   for clause in method.get('exception_regions',[]):
    if target==clause['try_start']:
     typ=clause.get('catch_type');typ=typ.split(' @ ',1)[0].removeprefix('class ').removeprefix('valuetype ')if typ else None
     actual.append((ordinal,clause['kind'].lower(),typ,not(clause['try_start']<=ins['offset']<clause['try_end'])))
 return {'source':sorted(source),'IL':sorted(actual),'equal':sorted(source)==sorted(actual)}


def owner_tests():
 from round10_control_owners import resolve_transfer
 loop={'kind':'loop','test':9,'exit':20,'body':1}
 switch={'kind':'switch','test':-1,'exit':9,'body':2}
 cases=[('switch-break-or-loop-continue',[loop,switch],9,False),
        ('switch-break-or-loop-break',[loop,dict(switch,exit=20)],20,False),
        ('unique-switch-break',[loop,dict(switch,exit=12)],12,True),
        ('continue-skips-switch',[loop,dict(switch,exit=12)],9,True),
        ('outer-break-needs-goto',[dict(loop,exit=30),dict(loop,test=19,exit=20)],30,False),
        ('protected-return-or-loop-exit',[{'kind':'EH','return_targets':[42]},dict(loop,exit=42)],42,False)]
 result=[]
 for name,frames,target,expected in cases:
  actual=resolve_transfer(frames,target,'leave'if name.startswith('protected')else'br')
  result.append({'name':name,'expected_complete':expected,'actual':actual,'pass':actual['complete']==expected})
 return result


def main():
 folder=WIP/('fixture-stage'+os.environ.get('REA_CONTROL_STAGE','1')+'-'+os.environ.get('REA_CONTROL_ATTEMPT','current'));folder.mkdir(parents=True,exist_ok=True);source=folder/'Fixture.cs';source.write_text(SOURCE,encoding='utf-8',newline='\n');base=json.loads((ROOT/'build/round10/baseline/Assembly-CSharp/build-manifest.json').read_text(encoding='utf-8'));mp=folder/'manifest.json';mp.write_text(json.dumps(dict(base,sources=[{'path':str(source),'input_path':str(source)}])),encoding='utf-8')
 with Client(mp,stage=int(os.environ.get('REA_CONTROL_STAGE','1')))as c:ast=c.extract(source,SOURCE)
 # Capture independent predictions before compiling or inspecting the fixture.
 (folder/'predictions-before-compilation.json').write_text(json.dumps(ast,indent=2),encoding='utf-8')
 binary=folder/'Fixture.dll';args=[f for f in base['flags'] if not f.startswith('-out:') and f!='-noconfig']+['-out:'+str(binary)]+['-r:'+r['path'] for r in base['references']]+[str(source)];rsp=folder/'compile.rsp';rsp.write_text('\n'.join('"'+a+'"' for a in args),encoding='utf-8');env=os.environ.copy();env.update(base['scoped_environment']);proc=subprocess.run([base['runtime'],base['compiler'],'-noconfig','@'+str(rsp)],env=env,cwd=ROOT,capture_output=True);(folder/'compiler.log').write_bytes(proc.stdout+proc.stderr)
 if proc.returncode:raise RuntimeError(proc.stdout+proc.stderr)
 raw=subprocess.check_output([str(ROOT/'build/tools/ProbeInspector.exe'),str(binary),'--all']);il={m['stable_id']:m for m in json.loads(raw.decode('utf-8-sig'))['selected_methods']};rows=[]
 for ir in ast:
  key=ir['method_id'];m=il.get(key)
  if not m:continue
  if ir.get('emission_plan_complete',False):
   left,right=canonical_plan(ir['emission_plan']),canonical_plan(extract_il(m));equal=left==right
   row={'method_id':key,'complete':ir['complete'],'plan_complete':True,'tree_complete':ir.get('tree_complete',False),'equal':equal,'source_plan':left,'IL_plan':right}
  else:row={'method_id':key,'complete':False,'plan_complete':False,'tree_complete':ir.get('tree_complete',False),'reasons':ir['reasons']}
  il_ir=extract_il(m)
  if row['plan_complete']:row['eh_entry_edges']=eh_entry_edges(ir,m)
  row['paired_tree_complete']=bool(ir.get('tree_complete')and il_ir.get('tree_complete'))
  if row['paired_tree_complete']:
   left_tree,right_tree=canonical_tree(ir),canonical_tree(il_ir);row['trees_equal']=left_tree is not None and right_tree is not None and left_tree==right_tree
   if not row['trees_equal']:row.update(source_tree=left_tree,IL_tree=right_tree)
  rows.append(row);print(key,'complete',row['complete'],'equal',row.get('equal'),'reasons',[x['code'] for x in row.get('reasons',[])])
 pairs=[]
 for left,right,expected in [('SwitchBreakTail','SwitchContinueTail',True),('SwitchBreakMid','SwitchContinueMid',False)]:
  a=il['Fixture::'+left+'(System.Int32)->System.Void'];b=il['Fixture::'+right+'(System.Int32)->System.Void'];equal=a['raw_body_hex']==b['raw_body_hex'];pairs.append({'left':left,'right':right,'raw_body_equal':equal,'expected_equal':expected,'pass':equal==expected})
 owners=owner_tests()
 result={'operation':'independent-AST-control-plan-fixture-validation','owner_resolution_tests':owners,'raw_owner_body_pairs':pairs,'game_compilations':0,'fixture_compilations':1,'AST_predictions_captured_before_fixture_compilation':True,'complete':sum(x['complete']for x in rows),'plan_complete':sum(x['plan_complete']for x in rows),'paired_tree_complete':sum(x.get('paired_tree_complete',False)for x in rows),'trees_equal':sum(x.get('trees_equal',False)for x in rows),'equal':sum(x.get('equal',False)for x in rows),'wrong_complete':sum(x['plan_complete']and not x.get('equal')for x in rows),'EH_entry_edge_proofs_equal':sum(x.get('eh_entry_edges',{}).get('equal',False)for x in rows),'EH_entry_edge_proofs_wrong':sum(x['plan_complete']and not x.get('eh_entry_edges',{}).get('equal')for x in rows),'rows':rows,'source_sha256':sha(source),'AST_source_sha256':sha(AST_SOURCE)};(ROOT/'reports'/('round10-control-fixture-stage'+os.environ.get('REA_CONTROL_STAGE','1')+'.json')).write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in result.items()if k!='rows'}))
if __name__=='__main__':main()
