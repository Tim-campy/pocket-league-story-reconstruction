"""Historical csc controls for protected foreach exits and positive try tail."""
import hashlib,json,re,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import differences,sha
from round7_roslyn_for_declaration_fixture import compile_fixture
import round9_protected_tail_source as api
TEMPLATE_ROOT=ROOT/'tools/fixtures/round9-protected-tail'
PATTERN=re.compile(r'(?m)^[ \t]*public[ \t]+(?:static[ \t]+)?(?:float|int|void)[ \t]+(Current|Expected)[ \t]*\(')
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
 work=ROOT/'build/round9/protected-tail-fixtures';manifest=json.loads((ROOT/'build/round9/cycles/cycle31/after/KairoLibrary/build-manifest.json').read_text(encoding='utf-8-sig'));rows=[];cases={}
 specs=[('stream','FixtureStream19.cs',api.transform_stream_writearray_common_exit),('lineup','FixtureLineup.cs',api.transform_lineup_start_composition)]
 for label,file,transform in specs:
  path=TEMPLATE_ROOT/file;src=path.read_text(encoding='utf-8-sig');spans=parts(src);before=run_name(spans['Current'][2]);control=run_name(spans['Expected'][2]);after=transform(before)
  old,_=compile_fixture(work/(label+'-before'),assemble(src,spans,before),manifest);want,_=compile_fixture(work/(label+'-control'),assemble(src,spans,control),manifest);actual,_=compile_fixture(work/(label+'-candidate'),assemble(src,spans,after),manifest)
  a,b,c=pick(old),pick(want),pick(actual);assert differences(a,b)[0],label;assert not differences(b,c)[0],(label,differences(b,c)[0]);cases[label]=(before,transform,spans)
  rows.append({'case':label,'full_body_equal':True,'before_body_different':True,'template_sha256':sha(path),'before_local_count':len(a['locals']),'after_local_count':len(c['locals']),'before':before,'after':after,'manual_control':control})
 stream=cases['stream'][0];lineup=cases['lineup'][0];negatives=[]
 tests=[('stream-branch-condition-other',stream.replace('elementType == typeof(bool)','elementType == typeof(char)'),api.transform_stream_writearray_common_exit),('stream-terminal-return-effects',stream.replace('return;','Effects++;return;',2),api.transform_stream_writearray_common_exit),('stream-extra-tail-effect',stream.replace('throw new ArgumentException','Effects++;throw new ArgumentException'),api.transform_stream_writearray_common_exit),('stream-capture',stream.replace('Type elementType =','System.Action f=delegate{}; Type elementType ='),api.transform_stream_writearray_common_exit),('lineup-null-return-effect',lineup.replace('return;','Effects++;return;',1),api.transform_lineup_start_composition),('lineup-close-call-receiver',lineup.replace('jarInflater?.Close();','GetInflater()?.Close();'),api.transform_lineup_start_composition),('lineup-finalizer-observation',lineup.replace('jarInflater?.Close();','Effects++;jarInflater?.Close();'),api.transform_lineup_start_composition),('lineup-capture',lineup.replace('JarInflater jarInflater = null;','JarInflater jarInflater = null; System.Action f=delegate{};'),api.transform_lineup_start_composition)]
 for label,bad,transform in tests:
  try:transform(bad)
  except(ValueError,AssertionError):negatives.append(label)
  else:raise AssertionError(label)
 report={'operation':'historical-csc-protected-type-and-array-tail-fixtures','compiler_sha256':manifest['compiler_sha256'],'producer_sha256':sha(ROOT/'tools/round9_protected_tail_source.py'),'driver_sha256':sha(Path(__file__)),'game_compilations':0,'toy_compilations':6,'positive_full_bodies':len(rows),'negative_controls':len(negatives),'negatives':negatives,'fixtures':rows,'limitations':'Nineteen exact type branches exercised in own code, full header/locals/IL/EH comparison with independently written chain. Stable existing receiver in finalizer, no new named game local; actual original signature required.'}
 (ROOT/'reports/round9-protected-tail-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in report.items()if k!='fixtures'}))
if __name__=='__main__':main()

