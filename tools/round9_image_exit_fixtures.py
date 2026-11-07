"""Historical csc controls for protected image exit and access-arm order."""
import hashlib,json,re,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import differences,sha
from round7_roslyn_for_declaration_fixture import compile_fixture
import round9_image_exit_source as api
TEMPLATE_ROOT=ROOT/'tools/fixtures/round9-image-exit'
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
 work=ROOT/'build/round9/image-exit-fixtures';manifest=json.loads((ROOT/'build/round9/cycles/cycle35/after/KairoLibrary/build-manifest.json').read_text(encoding='utf-8-sig'));rows=[];cases={}
 specs=[('image','FixtureImageExit.cs',api.transform_drawimage_d29_residual)]
 for label,file,transform in specs:
  path=TEMPLATE_ROOT/file;src=path.read_text(encoding='utf-8-sig');spans=parts(src);before=run_name(spans['Current'][2]);control=run_name(spans['Expected'][2]);after=transform(before)
  old,_=compile_fixture(work/(label+'-before'),assemble(src,spans,before),manifest);want,_=compile_fixture(work/(label+'-control'),assemble(src,spans,control),manifest);actual,_=compile_fixture(work/(label+'-candidate'),assemble(src,spans,after),manifest)
  a,b,c=pick(old),pick(want),pick(actual);assert differences(a,b)[0],label;assert not differences(b,c)[0],(label,differences(b,c)[0]);cases[label]=(before,transform,spans)
  rows.append({'case':label,'full_body_equal':True,'before_body_different':True,'template_sha256':sha(path),'before_local_count':len(a['locals']),'after_local_count':len(c['locals']),'before':before,'after':after,'manual_control':control})
 before=cases['image'][0];transform=api.transform_drawimage_d29_residual;negatives=[]
 start=before.index('if (num25 != -1)');end=api.close(before,before.index('{',start))
 tests=[('image-extra-tail-effect',before[:end]+'\nConsume(7);'+before[end:],transform),('image-existing-terminal-return',api.transform_drawimage_terminal_return(before),transform),('image-other-access-overload',before.replace('image.optimizeAccessL_[num28]','image.optimizeAccess_[num28]'),transform),('image-other-lookup-initializer',before.replace('int num25 = -1;','int num25 = -2;'),transform),('image-capture',before.replace('int num25 = -1;','int num25 = -1; System.Action captured=delegate{};'),transform)]
 components=[]
 for label,fn in [('access-only',api.transform_drawimage_access_order),('return-only',api.transform_drawimage_terminal_return)]:
  current=fn(before);got,_=compile_fixture(work/label,assemble(src,spans,current),manifest);delta=differences(pick(want),pick(got))[0];components.append({'component':label,'equal_to_manual_control':not bool(delta),'differences':delta})

 for label,bad,transform in tests:
  try:transform(bad)
  except(ValueError,AssertionError):negatives.append(label)
  else:raise AssertionError(label)
 report={'operation':'historical-csc-image-protected-exit-fixtures','compiler_sha256':manifest['compiler_sha256'],'producer_sha256':sha(ROOT/'tools/round9_image_exit_source.py'),'driver_sha256':sha(Path(__file__)),'game_compilations':0,'toy_compilations':5,'positive_full_bodies':len(rows),'negative_controls':len(negatives),'negatives':negatives,'fixtures':rows,'isolated_component_controls':components,'limitations':'Own template checks full composition, plus isolated access/return components recorded separately. Target has43 real original locals and three EH regions; no new local or floating relation rewrite. Isolated D29 access variant was already a negative target trial and is not replayed.'}
 (ROOT/'reports/round9-image-exit-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in report.items()if k!='fixtures'}))
if __name__=='__main__':main()

