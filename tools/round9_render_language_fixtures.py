"""Historical csc own controls: whole Offscreen and parser array foreach."""
import json,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import differences,sha
from round7_roslyn_for_declaration_fixture import compile_fixture
from round9_render_language_source import transform_offscreen_composition,transform_language_format_foreach
OFF_HEADER='''using System;using System.Collections.Generic;
public struct Vector3{public float x,y,z;public Vector3(float a,float b,float c){x=a;y=b;z=c;}}
public struct Quaternion{public static Quaternion identity;}
public struct Matrix4x4{public float Value;public static int Calls;public static Matrix4x4 TRS(Vector3 a,Quaternion b,Vector3 c){Calls++;return new Matrix4x4();}public static Matrix4x4 operator*(Matrix4x4 a,Matrix4x4 b){Calls++;return a;}}
public class Graphics{}
public class Image{public struct TextRenderTask{public float X,Y;}public struct BalloonRenderTask{public float X,Y;}public struct TouchTask{public int N;}public List<TextRenderTask>textRenderTasks_;public List<BalloonRenderTask>balloonRenderTasks_;public List<TouchTask>touchTasks_;}
public delegate bool Render(Graphics graphics,Fixture owner);
public class Fixture{public int Count;public Image image_;public Matrix4x4[]renderMatrix_;public Render DynamicRender;Graphics GetGraphics(){Count++;return new Graphics();}int GetHeight(){Count++;return Count;}
'''
OFF='''public void Run()
{
 try
 {
  for(int earlier=0;earlier<Count;earlier++){int child=Count;Count+=child;}
  Graphics graphics = GetGraphics();
  if (graphics == null) { return; }
  List<Image.TextRenderTask> textRenderTasks_ = image_.textRenderTasks_;
  List<Image.BalloonRenderTask> balloonRenderTasks_ = image_.balloonRenderTasks_;
  List<Image.TouchTask> touchTasks_ = image_.touchTasks_;
  bool flag2 = false;
  try { flag2 = DynamicRender == null || DynamicRender(graphics, this); }
  finally { Count++; }
  if (flag2)
  {
   Matrix4x4 matrix4x = new Matrix4x4();
   int num2 = Count;
   for (int l = 0; l < num2; l++)
   {
    renderMatrix_[l] *= Matrix4x4.TRS(new Vector3(0f, GetHeight(), 0f), Quaternion.identity, new Vector3(1f, 1f, 1f));
    renderMatrix_[l] *= matrix4x;
   }
   Image.TextRenderTask value = new Image.TextRenderTask();
   Vector3 vector = new Vector3(value.X, 0f - value.Y, 0f);
   value.Y = 0f - vector.y;
   Image.BalloonRenderTask value2 = new Image.BalloonRenderTask();
   Vector3 vector2 = new Vector3(value2.X, 0f - value2.Y, 0f);
   value2.Y = 0f - vector2.y;
   return;
  }
  image_.textRenderTasks_ = textRenderTasks_;
  image_.balloonRenderTasks_ = balloonRenderTasks_;
  image_.touchTasks_ = touchTasks_;
 }
 catch(Exception){Count++;}
 finally{Count++;}
}'''
OFF_CONTROL='''public void Run(){try{for(int earlier=0;earlier<Count;earlier++){int child=Count;Count+=child;}Graphics graphics=GetGraphics();if((object)graphics!=null){List<Image.TextRenderTask>textRenderTasks_=image_.textRenderTasks_;List<Image.BalloonRenderTask>balloonRenderTasks_=image_.balloonRenderTasks_;List<Image.TouchTask>touchTasks_=image_.touchTasks_;bool flag2=false;try{if(DynamicRender==null){flag2=true;}else{flag2=DynamicRender(graphics,this);}}finally{Count++;}if(flag2){Matrix4x4 matrix4x=new Matrix4x4();int num2=Count;for(int l=0;l<num2;l++){renderMatrix_[l]=renderMatrix_[l]*Matrix4x4.TRS(new Vector3(0f,GetHeight(),0f),Quaternion.identity,new Vector3(1f,1f,1f));renderMatrix_[l]=renderMatrix_[l]*matrix4x;}Image.TextRenderTask value=new Image.TextRenderTask();Vector3 vector=new Vector3(value.X,-value.Y,0f);value.Y=-vector.y;Image.BalloonRenderTask value2=new Image.BalloonRenderTask();Vector3 vector2=new Vector3(value2.X,-value2.Y,0f);value2.Y=-vector2.y;}else{image_.textRenderTasks_=textRenderTasks_;image_.balloonRenderTasks_=balloonRenderTasks_;image_.touchTasks_=touchTasks_;}}}catch(Exception){Count++;}finally{Count++;}}'''
LANG_HEADER='public class Fixture{public int Count;'
LANG=r'''public void Run(string[] array5)
{
 foreach(string earlier in array5){Count+=earlier.Length;}
 if (array5 != null)
 {
  string text5 = array5[0];
  string[] files = text5.Split('\n');
  for (int i = 0; i < files.Length; i++)
  {
   string[] array6 = files[i].Split(',');
   Count+=array6.Length;
  }
 }
}'''
LANG_CONTROL=r'''public void Run(string[] array5){foreach(string earlier in array5){Count+=earlier.Length;}if(array5!=null){string text5=array5[0];foreach(string recoveredLangFormatLine in text5.Split('\n')){string[] array6=recoveredLangFormatLine.Split(',');Count+=array6.Length;}}}'''
def pick(rows):return next(v for k,v in rows.items()if k.startswith('Fixture::Run('))
def main():
 work=ROOT/'build/round9/render-language-fixtures';manifest=json.loads((ROOT/'build/round9/cycles/cycle25/after/KairoLibrary/build-manifest.json').read_text(encoding='utf-8-sig'));rows=[]
 for label,before,expected,header,transform in [('offscreen-whole-five-components',OFF,OFF_CONTROL,OFF_HEADER,transform_offscreen_composition),('parser-array-foreach-reuse',LANG,LANG_CONTROL,LANG_HEADER,transform_language_format_foreach)]:
  after=transform(before);old,_=compile_fixture(work/(label+'-before'),header+before+'}',manifest);want,_=compile_fixture(work/(label+'-control'),header+expected+'}',manifest);actual,_=compile_fixture(work/(label+'-candidate'),header+after+'}',manifest);a,b,c=pick(old),pick(want),pick(actual);assert differences(a,b)[0],label;assert not differences(b,c)[0],(label,differences(b,c)[0]);rows.append({'case':label,'full_body_equal':True,'before_body_different':True,'before_local_count':len(a['locals']),'after_local_count':len(c['locals']),'before':before,'after':after,'manual_control':expected,'matrix_operator_calls':[i for i in c['instructions']if isinstance(i.get('operand'),dict)and '::op_Multiply' in i['operand'].get('method','')]})
 negatives=[]
 for label,bad,transform in [('matrix-index-call',OFF.replace('renderMatrix_[l] *= matrix4x;','renderMatrix_[GetHeight()] *= matrix4x;'),transform_offscreen_composition),('matrix-index-write',OFF.replace('renderMatrix_[l] *= matrix4x;','l++;renderMatrix_[l] *= matrix4x;'),transform_offscreen_composition),('nonzero-subtraction',OFF.replace('0f - value.Y','1f - value.Y'),transform_offscreen_composition),('flag-extra-return',OFF.replace('   return;','   if(Count<0)return;return;'),transform_offscreen_composition),('late-name-outside-scope',OFF.replace('finally{Count++;}','finally{Count+=image_.textRenderTasks_.Count;}'),transform_offscreen_composition),('parser-nonadjacent-array-loop',LANG.replace('  for (int i','  Count++;\n  for (int i'),transform_language_format_foreach),('parser-first-effect',LANG.replace("   string[] array6","   Count++;\n   string[] array6"),transform_language_format_foreach),('parser-index-more-than-once',LANG.replace('Count+=array6.Length;','Count+=array6.Length+i;'),transform_language_format_foreach),('parser-loop-nested',LANG.replace('Count+=array6.Length;','while(Count<3){Count++;}'),transform_language_format_foreach)]:
  try:transform(bad)
  except(ValueError,AssertionError):negatives.append(label)
  else:raise AssertionError(label)
 report={'operation':'historical-csc-render-and-language-composition-own-fixtures','compiler_sha256':manifest['compiler_sha256'],'producer_sha256':sha(ROOT/'tools/round9_render_language_source.py'),'driver_sha256':sha(Path(__file__)),'game_compilations':0,'toy_compilations':6,'positive_full_bodies':len(rows),'negative_controls':len(negatives),'negatives':negatives,'fixtures':rows,'limitations':'Struct-array operator is user-defined in toy and called once per expression, stable receiver/index only. No casts/ref/artificial usage inserted into game. Historical source spelling and actual game signature/body unknown until unchanged oracle.'}
 (ROOT/'reports/round9-render-language-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in report.items()if k!='fixtures'}))
if __name__=='__main__':main()

