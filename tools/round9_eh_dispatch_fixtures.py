"""Independent historical Roslyn controls; only toy source may be compiled here."""
import json,sys
from pathlib import Path
ROOT=next(p for p in Path(__file__).resolve().parents if (p/'tools/compiler_probes.py').is_file());sys.path.insert(0,str(ROOT/'tools'))
try:import round9_eh_dispatch_calculate as calc
except ImportError:import calculate as calc
from compare_kairo import differences,sha
from round7_roslyn_for_declaration_fixture import compile_fixture
WORK=ROOT/'build/round9/part-d-next'
JAR='''\t\tpublic byte[] Jar(string name)
\t\t{
\t\t\tname = ConvertExtension(name);
\t\t\tint len;
if (directory_ != null)
\t\t\t{
\t\t\t\tbyte[] array = ReadStorage(directory_ + name);
\t\t\t\tif (array != null) { return array; }
\t\t\t}
\t\t\tif ((int)GetSize(name) == -1) { return null; }
\t\t\tStream inputStream = GetInputStream(name);
\t\t\tif (inputStream == null) { return null; }
\t\t\tCollector byteArrayOutputStream = new Collector();
\t\t\tbyte[] array2 = null;
\t\t\tbyte[] array3 = new byte[1024];
\t\t\ttry
\t\t\t{
\t\t\t\twhile ((len = inputStream.Read(array3)) >= 0) { byteArrayOutputStream.Write(array3, 0, len); }
\t\t\t\treturn byteArrayOutputStream.ToByteArray();
\t\t\t}
\t\t\tfinally
\t\t\t{
\t\t\t\tif (byteArrayOutputStream != null) { try { byteArrayOutputStream.Close(); } catch (System.Exception) { } }
\t\t\t\tbyteArrayOutputStream = null;
\t\t\t\tif (inputStream != null) { try { inputStream.Close(); } catch (System.Exception) { } }
\t\t\t\tinputStream = null;
\t\t\t}
\t\t}'''.replace('\n','\r\n').replace('int len;\r\n','int len;\n')
EXPECTED_JAR='''\t\tpublic byte[] ExpectedJar(string name)
\t\t{
\t\t\tname = ConvertExtension(name);
\t\t\tif (directory_ != null)
\t\t\t{
\t\t\t\tbyte[] array = ReadStorage(directory_ + name);
\t\t\t\tif (array != null) { return array; }
\t\t\t}
\t\t\tif ((int)GetSize(name) == -1) { return null; }
\t\t\tStream inputStream = GetInputStream(name);
\t\t\tif (inputStream == null) { return null; }
\t\t\tCollector byteArrayOutputStream = new Collector();
\t\t\tbyte[] array2 = null;
\t\t\tbyte[] array3 = new byte[1024];
\t\t\tint len;
\t\t\ttry
\t\t\t{
\t\t\t\twhile ((len = inputStream.Read(array3)) >= 0) { byteArrayOutputStream.Write(array3, 0, len); }
\t\t\t\tarray2 = byteArrayOutputStream.ToByteArray();
\t\t\t}
\t\t\tfinally
\t\t\t{
\t\t\t\tif (byteArrayOutputStream != null) { try { byteArrayOutputStream.Close(); } catch (System.Exception) { } }
\t\t\t\tbyteArrayOutputStream = null;
\t\t\t\tif (inputStream != null) { try { inputStream.Close(); } catch (System.Exception) { } }
\t\t\t\tinputStream = null;
\t\t\t}
\t\t\treturn array2;
\t\t}'''.replace('\n','\r\n')
SAVE='''public int Save(){
 int[] array2=new int[3];int[] array3=new int[3];
 for (int i = 0; i < array2.Length; i++){array2[i]=i;}
 int num2 = 0;bool flag;
 do{counter_++;flag=counter_>=5;if(flag){for(int j=0;j<array3.Length;j++){array3[j]++;}}
 switch(num2){
 case 0:{for(int n=0;n<array2.Length;n++){if(Check())return n;Use(array2[n]);}num2=1;break;}
 case 1:{int count=0;for(int k=0;k<array3.Length;k++){if(array3[k]>0)count++;}Use(count);break;}
 }
 bool complete=true;for(int k=0;k<array2.Length;k++){if(array3[k]==0)complete=false;}if(complete)return counter_;
 }while(!flag);throw new System.Exception();
}'''.replace('\n','\r\n')
EXPECTED_SAVE='''public int ExpectedSave(){
 int[] array2=new int[3];int[] array3=new int[3];int num2, i;
 for (i = 0; i < array2.Length; i++){array2[i]=i;}
 num2 = 0;bool flag;
 do{counter_++;flag=counter_>=5;if(flag){for(int j=0;j<array3.Length;j++){array3[j]++;}}
 if(num2==0){for(int n=0;n<array2.Length;n++){if(Check())return n;Use(array2[n]);}num2=1;}
 else if(num2==1){int count=0;for(int k=0;k<array3.Length;k++){if(array3[k]>0)count++;}Use(count);}
 bool complete=true;for(int k=0;k<array2.Length;k++){if(array3[k]==0)complete=false;}if(complete)return counter_;
 }while(!flag);throw new System.Exception();
}'''.replace('\n','\r\n')
PREAMBLE='''// UTF-16 control: 🔎
public class Fixture {
 public class Stream { public int Read(byte[] b){return -1;}public void Close(){} }
 public class Collector { public byte[] ToByteArray(){return new byte[0];}public void Close(){}public void Write(byte[] b,int offset,int count){} }
 string directory_;int counter_;string ConvertExtension(string s){return s;}byte[] ReadStorage(string s){return null;}long GetSize(string s){return -1;}Stream GetInputStream(string s){return null;}
 bool Check(){return counter_<0;}void Use(int n){}
'''.replace('\n','\r\n')
def method(d,name):return next(v for k,v in d.items()if k.startswith('Fixture::'+name+'('))
def delta(a,b):return[x for x in differences(a,b)[0]if x!='signature']
def main():
 manifest=json.loads((calc.BASE/'build-manifest.json').read_text());folder=WORK/'fixtures';folder.mkdir(parents=True,exist_ok=True);path=folder/'Input.cs';mp=folder/'manifest.json';mp.write_text(json.dumps(dict(manifest,sources=[dict(path=str(path),input_path=str(path))])))
 source=PREAMBLE+JAR+'\r\n'+EXPECTED_JAR+'\r\n'+SAVE+'\r\n'+EXPECTED_SAVE+'\r\n}'
 baseline,_=compile_fixture(folder/'baseline',source,manifest)
 candidate=source.replace(JAR,calc.restore_jar(JAR));path.write_bytes(candidate.encode('utf-8'))
 rows=calc.ff.query(path,mp,dict(round7_cycle_rule='d9-calculated-state-dispatch-index',target_method='Save'));small=[p for r in rows for p in r['patches']];assert len(small)==1
 candidate=calc.ff.apply(candidate,small[0]);assert SAVE not in candidate
 # This pure declaration move is independently tested against the same control.
 pure_jar=JAR.replace('\t\t\tint len;\n','').replace('byte[] array3 = new byte[1024];','byte[] array3 = new byte[1024];\r\n\t\t\tint len;')
 pure_source=source.replace(JAR,pure_jar).replace(SAVE,calc.restore_save_declarations(SAVE));pure,_=compile_fixture(folder/'pure-declaration',pure_source,manifest)
 # Only the source-only host's selected Save method receives the declaration grouping.
 save_start=candidate.index('public int Save()');save_stop=candidate.index('public int ExpectedSave()',save_start)
 fragment=candidate[save_start:save_stop];candidate=candidate[:save_start]+calc.restore_save_declarations(fragment)+candidate[save_stop:]
 actual,_=compile_fixture(folder/'candidate',candidate,manifest);checks=[]
 for name,control in [('Jar','ExpectedJar'),('Save','ExpectedSave')]:
  difference=delta(method(actual,name),method(actual,control));assert not difference,(name,difference)
  pure_diff=delta(method(pure,name),method(pure,control));assert pure_diff,(name,'pure declaration already equal')
  checks.append(dict(method=name,control=control,full_body_differences=difference,pure_declaration_differences=pure_diff,
   baseline=method(baseline,name),candidate=method(actual,name),expected=method(actual,control),pure_declaration=method(pure,name)))
 # Refuse ambiguous switch ownership rather than rewriting a nonterminal break.
 bad=source.replace('Use(count);break;}','if(Check())break;Use(count);break;}');path.write_bytes(bad.encode('utf-8'))
 assert not any(r['patches']for r in calc.ff.query(path,mp,dict(round7_cycle_rule='d9-calculated-state-dispatch-index',target_method='Save')))
 result=dict(operation='WIP-new-EH-result-and-state-index-historical-fixtures',compiler_sha256=manifest['compiler_sha256'],helper_sha256=sha(calc.ff.RULE),driver_sha256=sha(Path(__file__)),
  historical_fixture_compilations=3,game_compilations=0,full_body_controls_equal=2,fixtures=checks,negatives=['declaration changes alone do not match either control','additional owned switch break rejected'])
 (WORK/'fixture-results.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in result.items()if k!='fixtures'}))
if __name__=='__main__':main()
