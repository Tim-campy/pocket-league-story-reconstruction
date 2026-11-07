"""Historical csc controls for reference and integer whole guard exits."""
import hashlib,json,re,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import differences,sha
from round7_roslyn_for_declaration_fixture import compile_fixture
import round9_whole_guard_source as api
TEMPLATE_ROOT=ROOT/'tools/fixtures/round9-whole-guard'
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
 work=ROOT/'build/round9/whole-guard-fixtures';manifest=json.loads((ROOT/'build/round9/cycles/cycle37/after/KairoLibrary/build-manifest.json').read_text(encoding='utf-8-sig'));rows=[];cases={}
 specs=[('surface','FixtureSurface.cs',api.transform_surface_debug_positive_tail),('unlock','FixtureUnlock.cs',api.transform_save_unlock_positive_tail),('offscreen','FixtureOffscreen.cs',api.transform_offscreen_two_guards)]
 for label,file,transform in specs:
  path=TEMPLATE_ROOT/file;src=path.read_text(encoding='utf-8-sig');spans=parts(src);before=run_name(spans['Current'][2]);control=run_name(spans['Expected'][2]);after=transform(before)
  old,_=compile_fixture(work/(label+'-before'),assemble(src,spans,before),manifest);want,_=compile_fixture(work/(label+'-control'),assemble(src,spans,control),manifest);actual,_=compile_fixture(work/(label+'-candidate'),assemble(src,spans,after),manifest)
  a,b,c=pick(old),pick(want),pick(actual);assert differences(a,b)[0],label;assert not differences(b,c)[0],(label,differences(b,c)[0]);cases[label]=(before,transform,spans)
  rows.append({'case':label,'full_body_equal':True,'before_body_different':True,'template_sha256':sha(path),'before_local_count':len(a['locals']),'after_local_count':len(c['locals']),'before':before,'after':after,'manual_control':control})
 surface,unlock,offscreen=cases['surface'][0],cases['unlock'][0],cases['offscreen'][0];negatives=[]
 tests=[('surface-nonempty-return',surface.replace('return;','Effects++;return;',1),api.transform_surface_debug_positive_tail),('surface-bounds-other',surface.replace('paintRects_.Count <= 0','paintRects_.Count <= 1'),api.transform_surface_debug_positive_tail),('surface-capture',surface.replace('SurfaceBase surfaceBase =','System.Action f=delegate{};SurfaceBase surfaceBase ='),api.transform_surface_debug_positive_tail),('unlock-return-effects',unlock.replace('return;','Effects++;return;',1),api.transform_save_unlock_positive_tail),('unlock-other-flag',unlock.replace('if (!forced)','if (forced)'),api.transform_save_unlock_positive_tail),('unlock-other-loop-entry',unlock.replace('int i = 0','int i = 1'),api.transform_save_unlock_positive_tail),('offscreen-extra-effect',offscreen.replace('Resize(width, height, format, filterMode);','Resize(width, height, format, filterMode);Effects++;'),api.transform_offscreen_two_guards),('offscreen-other-delegate',offscreen.replace('GetLayout(out width, out height, out format, out filterMode);','OtherLayout(out width, out height, out format, out filterMode);'),api.transform_offscreen_two_guards),('offscreen-before-effect',offscreen.replace('if (image_ != null','Effects++;if (image_ != null'),api.transform_offscreen_two_guards)]

 for label,bad,transform in tests:
  try:transform(bad)
  except(ValueError,AssertionError):negatives.append(label)
  else:raise AssertionError(label)
 report={'operation':'historical-csc-reference-integer-whole-guard-fixtures','compiler_sha256':manifest['compiler_sha256'],'producer_sha256':sha(ROOT/'tools/round9_whole_guard_source.py'),'driver_sha256':sha(Path(__file__)),'game_compilations':0,'toy_compilations':9,'positive_full_bodies':len(rows),'negative_controls':len(negatives),'negatives':negatives,'fixtures':rows,'limitations':'Own independent controls check positive root tails and two distinct Offscreen early returns, not old combined OR guard. Reference/int tests only; no float relation inversion. Candidate game locals/header/EH mandatory.'}
 (ROOT/'reports/round9-whole-guard-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in report.items()if k!='fixtures'}))
if __name__=='__main__':main()

