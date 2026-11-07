"""Own smcs ten-hunk draw composition controls, never compile the game."""
import json,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
import test_round9_scoped_exit as compiler
from compare_kairo import differences,sha
from round9_draw_composition_source import transform_draw
HEADER='''public class Fixture{public int Count,type_;public string[]selectButtons_;bool IsDrawWindow(){Count++;return Count>0;}void _draw2(object g){Count++;}'''
BEFORE='''public void Run(object g){int num10=Count;if(!IsDrawWindow()){if(Count<0){return;}if(Count>10){return;}}
for(int i=0;i<selectButtons_.Length;i++){if(selectButtons_[i]!=null){int width=selectButtons_[i].Length;Count+=width;}}
for(int j=0;j<selectButtons_.Length;j++){if(selectButtons_[j]!=null){int width=selectButtons_[j].Length;Count+=width;}}
if(type_==1){Count++;}else{if(type_==2){return;}if(type_==3){Count+=3;}else{if(type_==18){return;}if(type_==19){switch(num10){case 0:Count+=10;break;case 1:Count+=20;break;}}else{if(type_==20){return;}if(type_==23){Count+=23;}else if(type_!=29){_draw2(g);}}}}}'''
CONTROL='''public void Run(object g){int num10=Count;if(IsDrawWindow()){}else{if(Count<0){return;}if(Count>10){return;}}
for(int i=0;i<selectButtons_.Length;i++){if(selectButtons_[i]==null){continue;}int width=selectButtons_[i].Length;Count+=width;}
for(int j=0;j<selectButtons_.Length;j++){if(selectButtons_[j]==null){continue;}int width=selectButtons_[j].Length;Count+=width;}
if(type_==1){Count++;}else{if(type_==2){}else if(type_==3){Count+=3;}else{if(type_==18){}else if(type_==19){if(num10==0){Count+=10;}else if(num10==1){Count+=20;}}else{if(type_==20){}else if(type_==23){Count+=23;}else if(type_==29){}else{_draw2(g);}}}}}'''
def main():
 compiler.HERE=ROOT/'build/round9/draw-composition-fixtures';compiler.HERE.mkdir(parents=True,exist_ok=True)
 after=transform_draw(BEFORE);old=compiler.compile_toy(HEADER+BEFORE+'}','before')['Run'];want=compiler.compile_toy(HEADER+CONTROL+'}','control')['Run'];actual=compiler.compile_toy(HEADER+after+'}','candidate')['Run'];assert differences(old,want)[0];assert not differences(want,actual)[0],differences(want,actual)[0];assert len(old['locals'])==len(actual['locals'])+1
 negatives=[]
 for label,bad in [('window-owner',BEFORE.replace('if(!IsDrawWindow())','while(Count<3){if(!IsDrawWindow())').replace('if(Count>10){return;}}','if(Count>10){return;}}}')),('button-extra-tail',BEFORE.replace('Count+=width;}}','Count+=width;}Count++;}',1)),('button-wrong-initial',BEFORE.replace('int i=0','int i=1')),('type-guard-effects',BEFORE.replace('if(type_==2){return;}','if(type_==2){Count++;return;}')),('nonterminal-chain',BEFORE[:-1]+'Count++;}'),('dispatch-internal-return',BEFORE.replace('case 0:Count+=10;break;','case 0:Count+=10;return;')),('dispatch-default',BEFORE.replace('case 1:','default:')),('draw2-other-leaf',BEFORE.replace('_draw2(g);','Count++;'))]:
  try:transform_draw(bad)
  except(ValueError,AssertionError):negatives.append(label)
  else:raise AssertionError(label)
 report={'operation':'smcs-ten-hunk-draw-composition-own-control','compiler_sha256':sha(compiler.SMCS),'producer_sha256':sha(ROOT/'tools/round9_draw_composition_source.py'),'driver_sha256':sha(Path(__file__)),'game_compilations':0,'toy_compilations':3,'positive_full_bodies':1,'negative_controls':len(negatives),'negatives':negatives,'fixtures':[{'before':BEFORE,'after':after,'manual_control':CONTROL,'full_body_equal':True,'before_body_different':True,'extra_switch_cache_removed':1,'window_early_returns_retained':2,'three_terminal_returns_removed':3}],'limitations':'One calculated target with ten aligned edits; same string-array null semantics as original. Original signature/body/header/EH still mandatory, no state promotion from toy.'}
 (ROOT/'reports/round9-draw-composition-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in report.items()if k!='fixtures'}))
if __name__=='__main__':main()

