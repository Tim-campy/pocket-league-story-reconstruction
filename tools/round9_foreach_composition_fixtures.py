"""Independent smcs source-composition controls, including anonymous methods."""
import hashlib,json,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
import test_round9_scoped_exit as compiler
from compare_kairo import differences,sha
from round9_foreach_composition_source import transform_language,transform_billing,transform_query,transform_process
WORK=ROOT/'build/round9/foreach-composition-fixtures';WORK.mkdir(parents=True,exist_ok=True);compiler.HERE=WORK
COMMON='''using System;using System.Collections.Generic;
public class Inventory{}public class BillingItem{}
public static class Language{public static string[][] Data=new string[0][];public static string File;public static int Calls;public static string[][] GetSystemLanguagePackFiles(){Calls++;return Data;}public static string GetLanguagePackFile(){Calls++;return File;}public static bool SetLanguagePack(string text){Calls++;File=text;return true;}}
public class Fixture{public int Count;public int command_;public ThreadStart callback_;public string[][] languages_=new string[0][];public void Keep(string text){}public bool Pop(){return Count>0;}
'''.replace('ThreadStart','System.Threading.ThreadStart')
LANG_BEFORE='''public bool Run(){List<string[]> list=new List<string[]>();string[][] systemLanguagePackFiles=Language.GetSystemLanguagePackFiles();for(int spriteIndex=0;spriteIndex<systemLanguagePackFiles.Length;spriteIndex++){string[] item=systemLanguagePackFiles[spriteIndex];list.Add(item);}int num=command_;switch(num){case -3:Keep("case -2: break;");Count+=3;break;case -2:{System.Threading.ThreadStart action=delegate{Count+=2;};action();break;}case -1:{System.Threading.ThreadStart action=delegate{Count++;};action();break;}default:{string text2="x";if(num!=-1){text2=languages_[num][1];}if(Language.GetLanguagePackFile()!=text2&&Language.SetLanguagePack(text2)&&callback_!=null){callback_();}break;}}return Pop();}'''
LANG_EXPECTED='''public bool Run(){List<string[]> list=new List<string[]>();foreach(string[] item in Language.GetSystemLanguagePackFiles()){list.Add(item);}int num=command_;if(num==-3){Keep("case -2: break;");Count+=3;}else if(num==-2){System.Threading.ThreadStart action=delegate{Count+=2;};action();}else if(num==-1){System.Threading.ThreadStart action=delegate{Count++;};action();}else{string text2="x";if(num!=-1){text2=languages_[num][1];}if(Language.GetLanguagePackFile()!=text2){bool recoveredLanguagePackChanged=Language.SetLanguagePack(text2);if(recoveredLanguagePackChanged&&callback_!=null){callback_();}}}return Pop();}'''
BILL_BEFORE='''public void Run(string[][] array,string productId){string[][] array2=array;foreach(string[] array3 in array2){if((array3[0]!=productId)){return;}Count+=array3[1].Length;}Count+=7;}'''
BILL_EXPECTED='''public void Run(string[][] array,string productId){foreach(string[] array3 in array){if((array3[0]!=productId)){continue;}Count+=array3[1].Length;}Count+=7;}'''
QUERY_BEFORE='''public Inventory Run(List<string> moreItemSkus,List<string> moreSubsSkus){List<string> list=new List<string>();List<string>[] array=new List<string>[2]{moreItemSkus,moreSubsSkus};List<string>[] array2=array;foreach(List<string> list2 in array2){if(list2!=null){for(int j=0;j<list2.Count;j++){list.Add(list2[j]);}}}if(list.Count<=0){return new Inventory();}return new Inventory();}'''
QUERY_EXPECTED='''public Inventory Run(List<string> moreItemSkus,List<string> moreSubsSkus){List<string> list=new List<string>();List<string>[] array=new List<string>[2]{moreItemSkus,moreSubsSkus};foreach(List<string> list2 in array){if(list2!=null){for(int j=0;j<list2.Count;j++){list.Add(list2[j]);}}}if(list.Count<=0){return new Inventory();}{Inventory recoveredInventoryResult=new Inventory();return recoveredInventoryResult;}}'''
PROCESS_BEFORE='''public void Run(BillingItem[] items){BillingItem found=null;BillingItem[] array=items;foreach(BillingItem billingItem2 in array){if(billingItem2!=null){found=billingItem2;break;}}if(found!=null)Count++;}'''
PROCESS_EXPECTED='''public void Run(BillingItem[] items){BillingItem found=null;foreach(BillingItem billingItem2 in items){if(billingItem2!=null){found=billingItem2;break;}}if(found!=null)Count++;}'''
def main():
 rows=[]
 for role,fn,before,expected in [('language',transform_language,LANG_BEFORE,LANG_EXPECTED),('billing',transform_billing,BILL_BEFORE,BILL_EXPECTED),('query',transform_query,QUERY_BEFORE,QUERY_EXPECTED),('process-existing-primitive',transform_process,PROCESS_BEFORE,PROCESS_EXPECTED)]:
  after=fn(before);control=compiler.compile_toy(COMMON+expected+'}',role+'-expected')['Run'];actual=compiler.compile_toy(COMMON+after+'}',role+'-candidate')['Run']
  delta=differences(control,actual)[0];assert not delta,(role,delta)
  rows.append({'role':role,'before':before,'after':after,'expected_manual':expected,'full_body_equal':True,'locals_equal':control['locals']==actual['locals'],'anonymous_refs_compared':role=='language'})
 negatives=[]
 for name,fn,bad in [('extra-array-use',transform_process,PROCESS_BEFORE.replace('if(found!=null)','Keep(array.ToString());if(found!=null)')),('duplicate-alias',transform_billing,BILL_BEFORE.replace('string[][] array2=array;','string[][] array2=array;string[][] array2=array;')),('extra-index-use',transform_language,LANG_BEFORE.replace('list.Add(item);','list.Add(item);Count+=spriteIndex;')),('wrong-case-order',transform_language,LANG_BEFORE.replace('case -2:','case -9:')),('unknown-break-owner',transform_language,LANG_BEFORE.replace('Count+=3;break;','for(int j=0;j<2;j++){break;}Count+=3;break;')),('wrong-final-return',transform_query,QUERY_BEFORE.replace('}return new Inventory();}','}return null;}'))]:
  try:fn(bad)
  except ValueError:negatives.append(name)
  else:raise AssertionError(name)
 report={'operation':'smcs-array-foreach-composition-fixtures','compiler_sha256':sha(compiler.SMCS),'producer_sha256':sha(ROOT/'tools/round9_foreach_composition_source.py'),'driver_sha256':sha(Path(__file__)),
  'game_compilations':0,'toy_compilations':8,'positive_full_bodies':len(rows),'negative_controls':len(negatives),'fixtures':rows,'negatives':negatives,
  'language_array_expression_evaluated_once':True,'compiler_generated_classes_not_written_manually':True,'existing_process_primitive_not_claimed_new':True,
  'limitations':'Exact source templates supported by original IL; no game method is claimed matched before the real oracle.'}
 (ROOT/'reports/round9-foreach-composition-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in report.items() if k!='fixtures'}))
if __name__=='__main__':main()
