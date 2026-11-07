"""Historical csc controls for protected retry/heap exit owners."""
import hashlib,json,re,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import differences,sha
from round7_roslyn_for_declaration_fixture import compile_fixture
import round9_protected_owner_source as api
TEMPLATE_ROOT=ROOT/'tools/fixtures/round9-protected-owner'
PATTERN=re.compile(r'(?m)^[ \t]*public[ \t]+(?:static[ \t]+)?(?:float|int|void|byte(?:\[\]){2})[ \t]+(Current|Expected)[ \t]*\(')
def parts(source):
 found={}
 for m in PATTERN.finditer(source):
  end=api.close(source,source.index('{',m.end()));found[m.group(1)]=(m.start(),end,source[m.start():end])
 assert 'Current'in found and'Expected'in found
 return found
def run_name(text):
 return re.sub(r'\b(Current|Expected)(?=\s*\()', 'Run',text,count=1)
def assemble(source,spans,method):
 for name,(a,b,old)in sorted(spans.items(),key=lambda q:q[1][0],reverse=True):
  source=source[:a]+(method if name=='Current'else'')+source[b:]
 return source
def pick(rows):
 hits=[v for k,v in rows.items()if'::Run('in k];assert len(hits)==1;return hits[0]
def main():
 work=ROOT/'build/round9/protected-owner-fixtures';manifest=json.loads((ROOT/'build/round9/cycles/cycle36/after/KairoLibrary/build-manifest.json').read_text(encoding='utf-8-sig'));rows=[];cases={}
 specs=[('payment','FixturePayment.cs',api.transform_payment_retry_else),('http','FixtureHttp.cs',api.transform_http_retry_continue),('storage','FixtureStorage.cs',api.transform_storage_write_protected_exits)]
 for label,file,transform in specs:
  path=TEMPLATE_ROOT/file;src=path.read_text(encoding='utf-8-sig');spans=parts(src);before=run_name(spans['Current'][2]);control=run_name(spans['Expected'][2]);after=transform(before)
  old,_=compile_fixture(work/(label+'-before'),assemble(src,spans,before),manifest);want,_=compile_fixture(work/(label+'-control'),assemble(src,spans,control),manifest);actual,_=compile_fixture(work/(label+'-candidate'),assemble(src,spans,after),manifest)
  a,b,c=pick(old),pick(want),pick(actual);assert differences(a,b)[0],label;assert not differences(b,c)[0],(label,differences(b,c)[0]);cases[label]=(before,transform,spans)
  rows.append({'case':label,'full_body_equal':True,'before_body_different':True,'template_sha256':sha(path),'before_local_count':len(a['locals']),'after_local_count':len(c['locals']),'before':before,'after':after,'manual_control':control})
 payment,http,storage=cases['payment'][0],cases['http'][0],cases['storage'][0];negatives=[]
 tests=[('payment-loop-owned-break',payment.replace('result += array[j].Length;','result += array[j].Length;break;'),api.transform_payment_retry_else),('payment-tail-effects',payment.replace('} catch(Exception t)', 'Log.Info(9); } catch(Exception t)'),api.transform_payment_retry_else),('payment-different-retry-label',payment.replace('IL_0304','IL_0305'),api.transform_payment_retry_else),('http-nested-continue-owner',http.replace('num++;','while(true) { num++; }'),api.transform_http_retry_continue),('http-label-end-effects',http.replace('end_IL_0014:;','end_IL_0014:;Trace(9);'),api.transform_http_retry_continue),('http-rethrow-other',http.replace('throw ex;','throw;'),api.transform_http_retry_continue),('storage-heap-setter-observer',storage.replace('heapData_.SetElementAt(data, num); return;','heapData_.SetElementAt(data, num); Effects++;return;'),api.transform_storage_write_protected_exits),('storage-media-other',storage.replace('media_ != 3','media_ != 2'),api.transform_storage_write_protected_exits),('storage-capture',storage.replace('filename=Config.ConvertExtension(filename);','System.Action f=delegate{};filename=Config.ConvertExtension(filename);'),api.transform_storage_write_protected_exits)]

 for label,bad,transform in tests:
  try:transform(bad)
  except(ValueError,AssertionError):negatives.append(label)
  else:raise AssertionError(label)
 report={'operation':'historical-csc-protected-retry-and-heap-exit-fixtures','compiler_sha256':manifest['compiler_sha256'],'producer_sha256':sha(ROOT/'tools/round9_protected_owner_source.py'),'driver_sha256':sha(Path(__file__)),'game_compilations':0,'toy_compilations':9,'positive_full_bodies':len(rows),'negative_controls':len(negatives),'negatives':negatives,'fixtures':rows,'limitations':'Independent manual controls preserve retry do/while and catch continue owners, heap common returns and media3 guard. Heavy own catch tests backward leave width; game exact headers/locals/EH mandatory. No rethrow substitution or invented source mapping.'}
 (ROOT/'reports/round9-protected-owner-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in report.items()if k!='fixtures'}))
if __name__=='__main__':main()

