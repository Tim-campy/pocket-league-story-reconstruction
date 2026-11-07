"""Historical Roslyn controls for composed Boolean/block/store recovery."""
import json,sys,hashlib
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
from round7_roslyn_for_declaration_fixture import compile_fixture
from compare_kairo import differences,sha
from round9_boolean_composition_source import transform_graphics,transform_shader
WORK=ROOT/'build/round9/boolean-composition-fixtures';WORK.mkdir(parents=True,exist_ok=True)
P='flag && renderIsReplace_ && (blendMode_ == 0 || blendMode_ == 3) && renderForceMaterial_ == null && localMatrixIndex_ == -1 && !useFigureClip_'
CACHE='''{int[] cachedVertices=Cache();for(int cachedIndex=0;cachedIndex<cachedVertices.Length;cachedIndex++){long cachedMesh=cachedVertices[cachedIndex];Count+=(int)cachedMesh;}}'''
FRESH='''{string[] freshVectors=Fresh();object[] renderPasses=Passes();for(int passIndex=0;passIndex<renderPasses.Length;passIndex++){object renderPass=renderPasses[passIndex];double color=Count;Count+=(int)color+freshVectors.Length+(renderPass==null?0:1);}}'''
GRAPH_BEFORE='public void Run(bool flag,bool flag2){if ('+P+' && !setFigureClip_) {Gui();} else if ((flag) || (!(flag2))) '+FRESH+' else '+CACHE+'}'
GRAPH_EXPECTED='public void Run(bool flag,bool flag2){if (!('+P+') ? false : (!setFigureClip_)) {Gui();} else if ((!flag) & flag2) '+CACHE+' else '+FRESH+'}'
HEADER='''public class Fixture{public int Count;public bool renderIsReplace_,useFigureClip_,setFigureClip_;public int blendMode_,localMatrixIndex_;public object renderForceMaterial_;public void Gui(){Count++;}public int[] Cache(){Count++;return new int[]{1,2};}public string[] Fresh(){Count++;return new string[]{"x","y"};}public object[] Passes(){Count++;return new object[]{null,new object()};}
'''
SHADER_BEFORE=r'''public string Run(int nameId){string text="";string text3="";string text4="";string text8="";int num=Count;if((nameId&1)!=0){text="base";}else{bool recoveredCondition_7106 = (nameId & 2) != 0;if (recoveredCondition_7106){text8="A";}else{text8="B";}if((nameId&0x20)==0){text4 = (((nameId & 0x10000) != 0) ? "SetTexture [_MainTex] { combine texture }" : "SetTexture [_MainTex] { combine texture * primary }");}if((nameId&0x40)!=0){text3="root";}else if ((nameId & 0x80) == 0){text3=(((nameId&0x100)==0)?"Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha":"Blend Off");}else if ((nameId & 0x2000) != 0){text3="positive2";}else if((nameId&0x4000)!=0){text3="positive4";}else if((nameId&0x8000)!=0){text3="positive8";}if (num != 0){text8=text8+num;}text=text3+text4+text8;}return text.Replace("'", "\"");}'''
SHADER_EXPECTED=r'''public string Run(int nameId){string text="";string text3="";string text4="";string text8="";int num=Count;if((nameId&1)!=0){text="base";}else{if((nameId&2)!=0){text8="A";}else{text8="B";}if((nameId&0x20)==0){if((nameId&0x10000)==0){text4="SetTexture [_MainTex] { combine texture * primary }";}else{text4="SetTexture [_MainTex] { combine texture }";}}if((nameId&0x40)!=0){text3="root";}else if((nameId&0x80)!=0){if((nameId&0x2000)!=0){text3="positive2";}else if((nameId&0x4000)!=0){text3="positive4";}else if((nameId&0x8000)!=0){text3="positive8";}}else{if((nameId&0x100)!=0){text3="Blend Off";}else{text3="Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha";}}if(num!=0){text8=text8+num;}text=text3+text4+text8;}text=text.Replace("'", "\"");return text;}'''
SHADER_BEFORE=SHADER_BEFORE.replace('text3=','text3 =')
def pick(rows):return next(v for k,v in rows.items() if k.startswith('Fixture::Run('))
def main():
 manifest=json.loads((ROOT/'build/round9/cycles/cycle13/after/KairoLibrary/build-manifest.json').read_text(encoding='utf-8-sig'));rows=[]
 for role,fn,before,expected in [('graphics',transform_graphics,GRAPH_BEFORE,GRAPH_EXPECTED),('shader',transform_shader,SHADER_BEFORE,SHADER_EXPECTED)]:
  after=fn(before);base,_=compile_fixture(WORK/(role+'-before'),HEADER+before+'}',manifest);control,_=compile_fixture(WORK/(role+'-expected'),HEADER+expected+'}',manifest);candidate,_=compile_fixture(WORK/(role+'-candidate'),HEADER+after+'}',manifest)
  initial=[x for x in differences(pick(base),pick(control))[0] if x!='signature'];assert initial,role
  delta=differences(pick(control),pick(candidate))[0];assert not delta,(role,delta)
  rows.append({'role':role,'before':before,'after':after,'manual_control':expected,'before_body_differs':True,'body_equal':True,'locals_equal':pick(control)['locals']==pick(candidate)['locals']})
 negative=[]
 for name,fn,source in [('graphics-duplicate-anchor',transform_graphics,GRAPH_BEFORE+GRAPH_BEFORE),('graphics-unknown-second-condition',transform_graphics,GRAPH_BEFORE.replace('else if ((flag) || (!(flag2))) ','else if(flag2) ')),('shader-extra-alias-read',transform_shader,SHADER_BEFORE+' recoveredCondition_7106'),('shader-missing-positive-chain',transform_shader,SHADER_BEFORE.replace('text3 ="positive8";','Count++;'))]:
  try:fn(source)
  except (ValueError,AssertionError):negative.append(name)
  else:raise AssertionError(name)
 report={'operation':'historical-Roslyn-composed-Boolean-block-storage-fixtures','compiler_sha256':manifest['compiler_sha256'],'producer_sha256':sha(ROOT/'tools/round9_boolean_composition_source.py'),'driver_sha256':sha(Path(__file__)),
  'game_compilations':0,'toy_compilations':6,'positive_full_bodies':len(rows),'negative_controls':len(negative),'fixtures':rows,'negatives':negative,
  'baseline_forms_demonstrably_differ':True,'original_IL_bytes_copied':False,
  'initial_harness_failures':['Shader toy omitted the exact text3 assignment spacing required by the narrow template guard; rejected before Shader compilation. Corrected toy formatting;3 previous Graphics toy compilations.'],
  'limitations':'Source templates and complete toy bodies only; real signatures and full method bodies remain mandatory.'}
 (ROOT/'reports/round9-boolean-composition-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in report.items() if k!='fixtures'}))
if __name__=='__main__':main()
