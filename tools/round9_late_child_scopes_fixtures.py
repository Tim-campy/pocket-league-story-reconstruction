"""Own historical Roslyn controls for calculated late child scopes."""
import json,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import differences,sha
from round7_roslyn_for_declaration_fixture import compile_fixture
from round9_late_child_scopes_source import transform_balloon_tail_scope,transform_seb_scan_loop_scope
HEADER='''using System.Collections.Generic; public class Rectangle{public float right,left,top,bottom;}public class KairoMath{public static bool CheckHitRect(Rectangle a,Rectangle b){return a.right<b.left;}}public class Fixture{public int Count;public List<Rectangle> maskList_;public Rectangle draw_tmp2;public float num18;static void Touch(ref bool v){v=!v;}static void Touch(ref int v){v++;}static void Touch(ref double v){v++;}bool Read(){Count++;return Count>0;}int ReadInt(){Count++;return Count;}double ReadDouble(){Count++;return Count;}
'''
BALLOON='''public void Run()
{
 foreach (Rectangle item2 in maskList_)
 {
  if (KairoMath.CheckHitRect(item2,draw_tmp2))
  {
   bool flag2=Read(),flag3=Read(),flag4=Read(),flag5=Read();
   int num26=ReadInt();
   Touch(ref flag2);Touch(ref flag3);Touch(ref flag4);Touch(ref flag5);Touch(ref num26);
   continue;
  }
  bool flag6 = draw_tmp2.right < num18;
  bool flag7 = Read();
  bool flag8 = Read();
  bool flag9 = Read();
  int num27 = ReadInt();
  Touch(ref flag6);Touch(ref flag7);Touch(ref flag8);Touch(ref flag9);Touch(ref num27);
 }
}'''
BALLOON_CONTROL='''public void Run(){foreach(Rectangle item2 in maskList_){if(KairoMath.CheckHitRect(item2,draw_tmp2)){bool flag2=Read(),flag3=Read(),flag4=Read(),flag5=Read();int num26=ReadInt();Touch(ref flag2);Touch(ref flag3);Touch(ref flag4);Touch(ref flag5);Touch(ref num26);continue;}{bool flag6=draw_tmp2.right<num18;bool flag7=Read();bool flag8=Read();bool flag9=Read();int num27=ReadInt();Touch(ref flag6);Touch(ref flag7);Touch(ref flag8);Touch(ref flag9);Touch(ref num27);}}}'''
SCAN='''public void Run(int num8,int num9){if(Count>0){double earlierChild=ReadDouble();Touch(ref earlierChild);}int num11 = num8;
int num12 = num8 * 8;
while (num11 <= num9){if(Count==5){break;}if(Count==6){Count++;continue;}Touch(ref num11);Touch(ref num12);num11++;num12 += 8;}Count++;}'''
SCAN_CONTROL='''public void Run(int num8,int num9){if(Count>0){double earlierChild=ReadDouble();Touch(ref earlierChild);}for(int num11=num8,num12=num8*8;num11<=num9;){if(Count==5){break;}if(Count==6){Count++;continue;}Touch(ref num11);Touch(ref num12);num11++;num12+=8;}Count++;}'''
def local_tuple(s):
 for a,b in [('num12','Nstride'),('num11','Nindex'),('num9','Nupper'),('num8','Nlower')]:s=s.replace(a,b)
 for a,b in [('Nstride','num13'),('Nindex','num12'),('Nupper','num10'),('Nlower','num9')]:s=s.replace(a,b)
 return s
def pick(rows):return next(v for k,v in rows.items()if k.startswith('Fixture::Run('))
def main():
 work=ROOT/'build/round9/late-child-scopes-fixtures';manifest=json.loads((ROOT/'build/round9/cycles/cycle22/after/KairoLibrary/build-manifest.json').read_text(encoding='utf-8-sig'));rows=[]
 for label,before,expected,transform in [('balloon-post-continue-tail',BALLOON,BALLOON_CONTROL,transform_balloon_tail_scope),('seb-index11-stride12',SCAN,SCAN_CONTROL,transform_seb_scan_loop_scope),('seb-index12-stride13',local_tuple(SCAN),local_tuple(SCAN_CONTROL),transform_seb_scan_loop_scope)]:
  after=transform(before);old,_=compile_fixture(work/(label+'-before'),HEADER+before+'}',manifest);want,_=compile_fixture(work/(label+'-control'),HEADER+expected+'}',manifest);actual,_=compile_fixture(work/(label+'-candidate'),HEADER+after+'}',manifest);a,b,c=pick(old),pick(want),pick(actual);assert differences(a,b)[0],label;assert not differences(b,c)[0],(label,differences(b,c)[0]);rows.append({'case':label,'full_body_equal':True,'before_body_different':True,'original_body_order_and_loop_actions_retained':True,'before':before,'after':after,'manual_control':expected})
 negatives=[]
 for label,bad,transform in [('balloon-capture',BALLOON.replace('int num27 = ReadInt();','int num27 = ReadInt(); System.Action f=()=>Count++;'),transform_balloon_tail_scope),('balloon-outer-use',BALLOON[:-1]+'Count+=num27;}',transform_balloon_tail_scope),('balloon-nested-continue-owner',BALLOON.replace('   continue;','   while(Count>0){continue;}'),transform_balloon_tail_scope),('balloon-between-effects',BALLOON.replace('  bool flag6','  Count++;\n  bool flag6'),transform_balloon_tail_scope),('scan-outside-use',SCAN[:-1]+'Count+=num11;}',transform_seb_scan_loop_scope),('scan-nested-owner',SCAN.replace('Touch(ref num11);','while(Count<3){break;}Touch(ref num11);'),transform_seb_scan_loop_scope),('scan-capture',SCAN.replace('Count++;}','System.Action f=()=>Count++;}',1),transform_seb_scan_loop_scope),('scan-missing-body-increment',SCAN.replace('num12 += 8;',''),transform_seb_scan_loop_scope)]:
  try:transform(bad)
  except(AssertionError,ValueError):negatives.append(label)
  else:raise AssertionError(label)
 report={'operation':'historical-csc-late-child-scopes-fixtures','compiler_sha256':manifest['compiler_sha256'],'producer_sha256':sha(ROOT/'tools/round9_late_child_scopes_source.py'),'driver_sha256':sha(Path(__file__)),'game_compilations':0,'toy_compilations':9,'positive_full_bodies':len(rows),'negative_controls':len(negatives),'fixtures':rows,'negatives':negatives,'limitations':'Reference calls in own toy make local allocation observable, never inserted in game sources. Game header/IL/locals/EH oracle remains unchanged; role mapping is diagnostic only.'}
 (ROOT/'reports/round9-late-child-scopes-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in report.items()if k!='fixtures'}))
if __name__=='__main__':main()

