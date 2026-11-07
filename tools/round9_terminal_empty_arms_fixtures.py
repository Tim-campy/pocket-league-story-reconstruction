"""Independent smcs terminal-chain controls for the eight-arm composition."""
import json,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
import test_round9_scoped_exit as compiler
from compare_kairo import differences,sha
from round9_terminal_empty_arms_source import transform_friend
WORK=ROOT/'build/round9/terminal-empty-arms-fixtures';WORK.mkdir(parents=True,exist_ok=True);compiler.HERE=WORK
HEADER='public class Fixture{public int Count;'
OR='type == 4 || type == 5 || type == 6 || type == 7 || type == 8 || type == 9 || type == 10 || type == 11'
CHAIN='if (type == 12) {int first=Count;Count+=first;} else if(type==13){for(int i=0;i<3;i++){Count+=i;}} else {Count++;}'
ARMS='if(type==4){}else if(type==5){}else if(type==6){}else if(type==7){}else if(type==8){}else if(type==9){}else if(type==10){}else if(type==11){}else '
BEFORE='public void Run(int type){if('+OR+'){return;}'+CHAIN+'}'
EXPECTED='public void Run(int type){'+ARMS+CHAIN+'}'
NESTED='public void Run(int type){if(type==0){Count+=3;}else{if('+OR+'){return;}'+CHAIN+'}}'
NESTED_EXPECTED='public void Run(int type){if(type==0){Count+=3;}else{'+ARMS+CHAIN+'}}'
def main():
 rows=[]
 for label,before,expected in [('root',BEFORE,EXPECTED),('terminal-else',NESTED,NESTED_EXPECTED)]:
  after=transform_friend(before);control=compiler.compile_toy(HEADER+expected+'}',label+'-control')['Run'];candidate=compiler.compile_toy(HEADER+after+'}',label+'-candidate')['Run'];assert not differences(control,candidate)[0],(label,differences(control,candidate)[0]);rows.append({'case':label,'body_equal':True,'locals_equal':control['locals']==candidate['locals'],'before':before,'after':after,'manual_control':expected})
 negatives=[]
 for name,bad in [('nonterminal-parent',NESTED[:-1]+'Count++;}'),('loop-owner',BEFORE.replace('{if('+OR,'{while(Count<5){if('+OR)[:-1]+'}}'),('duplicate-case',BEFORE.replace('type == 11','type == 10')),('side-effect-selector',BEFORE.replace('type == 4','GetType() == 4')),('guard-has-effects',BEFORE.replace('{return;}','{Count++;return;}')),('following-selector-differs',BEFORE.replace('if (type == 12)','if (Count == 12)'))]:
  try:transform_friend(bad)
  except ValueError:negatives.append(name)
  else:raise AssertionError(name)
 report={'operation':'smcs-terminal-OR-return-versus-joint-empty-else-arms-fixtures','compiler_sha256':sha(compiler.SMCS),'producer_sha256':sha(ROOT/'tools/round9_terminal_empty_arms_source.py'),'driver_sha256':sha(Path(__file__)),
  'game_compilations':0,'toy_compilations':4,'positive_full_bodies':len(rows),'negative_controls':len(negatives),'fixtures':rows,'negatives':negatives,
  'preserved_following_chain':True,'no_new_locals':True,'limitations':'One terminal flat same-selector OR list with block return and complete following chain; no loop/protected owner inferred.'}
 (ROOT/'reports/round9-terminal-empty-arms-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in report.items() if k!='fixtures'}))
if __name__=='__main__':main()
