"""Historical csc controls for text alignment, pictograph tail and image lookup."""
import hashlib,json,re,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import differences,sha
from round7_roslyn_for_declaration_fixture import compile_fixture
import round9_text_image_source as api
LEGACY_FUNCTION = "def transform_make_alignment_tail(method_source):\n s=method_source\n a=once(s,'if ((num2 == 32))');end=once(s,'if (height_ != -1f && num4 < 0f)');i=indent(s,a)\n old=s[a:end]\n no_capture(old)\n for expression in ['num4 = ((height_ != -1f) ? ((height_ - num4) / 2f) : ((-(num4)) / 2f));','num4 = ((height_ != -1f) ? (height_ - num4) : (0f - num4));','num4 = 0f;']:assert old.count(expression)==1\n assert old.count('?')==2 and words(old).count('num4')==7,'only exact final alignment expressions supported'\n after='if (num2 == 32) { if (height_ == -1f) { num4 = -num4 / 2f; } else { num4 = (height_ - num4) / 2f; } } else if (num2 == 64) { if (height_ == -1f) { num4 = -num4; } else { num4 = height_ - num4; } } else { num4 = 0f; }\\r\\n'+i\n return finish(s,s[:a]+after+s[end:])"
TEMPLATE_ROOT=ROOT/'tools/fixtures/round9-text-image'
PATTERN=re.compile(r'(?m)^[ \t]*public[ \t]+(?:static[ \t]+)?(?:float|int|void)[ \t]+(Current|Expected|NegativeExtraFieldWrite)[ \t]*\(')
def parts(source):
 found={}
 for m in PATTERN.finditer(source):
  end=api.close(source,source.index('{',m.end()));found[m.group(1)]=(m.start(),end,source[m.start():end])
 assert 'Current'in found and'Expected'in found
 return found
def run_name(text):
 return re.sub(r'\b(Current|Expected|NegativeExtraFieldWrite)(?=\s*\()', 'Run',text,count=1)
def assemble(source,spans,method):
 for name,(a,b,old)in sorted(spans.items(),key=lambda q:q[1][0],reverse=True):
  source=source[:a]+(method if name=='Current'else'')+source[b:]
 return source
def pick(rows):
 hits=[v for k,v in rows.items()if'::Run('in k];assert len(hits)==1;return hits[0]
def main():
 work=ROOT/'build/round9/text-image-fixtures';manifest=json.loads((ROOT/'build/round9/cycles/cycle28/after/KairoLibrary/build-manifest.json').read_text(encoding='utf-8-sig'));rows=[];cases={}
 specs=[('make','FixtureMakeTail.cs',api.transform_make_alignment_tail),('block','FixtureTextBlockDraw.cs',api.transform_textblock_draw_composition),('image','FixtureDrawImage.cs',api.transform_drawimage_composition)]
 for label,file,transform in specs:
  path=TEMPLATE_ROOT/file;src=path.read_text(encoding='utf-8-sig');spans=parts(src);before=run_name(spans['Current'][2]);control=run_name(spans['Expected'][2]);after=transform(before)
  old,_=compile_fixture(work/(label+'-before'),assemble(src,spans,before),manifest);want,_=compile_fixture(work/(label+'-control'),assemble(src,spans,control),manifest);actual,_=compile_fixture(work/(label+'-candidate'),assemble(src,spans,after),manifest)
  a,b,c=pick(old),pick(want),pick(actual);assert differences(a,b)[0],label;assert not differences(b,c)[0],(label,differences(b,c)[0]);cases[label]=(before,transform,spans)
  rows.append({'case':label,'full_body_equal':True,'before_body_different':True,'template_sha256':sha(path),'before_local_count':len(a['locals']),'after_local_count':len(c['locals']),'before':before,'after':after,'manual_control':control})
 extra=run_name(cases['make'][2]['NegativeExtraFieldWrite'][2])
 legacy_globals=dict(vars(api));exec(LEGACY_FUNCTION,legacy_globals);legacy_after=legacy_globals['transform_make_alignment_tail'](extra)
 assert'SideEffects++'in extra and'SideEffects++'not in legacy_after
 legacy={'kind':'pure_source_guard_control_not_game_or_compiler','v1_accepted_extra_field_write':True,'v1_removed_field_write':True,'input':extra,'output':legacy_after,'function_sha256':hashlib.sha256(LEGACY_FUNCTION.encode()).hexdigest(),'actual_game_input_unchanged':True}
 make=cases['make'][0];block=cases['block'][0];image=cases['image'][0];negatives=[]
 tests=[('make-extra-field-write',extra,api.transform_make_alignment_tail),('make-other-mode',make.replace('num2 == 64','num2 == 65'),api.transform_make_alignment_tail),('block-last-return-other',block.replace('return result;','return 2;'),api.transform_textblock_draw_composition),('block-capture',block.replace('int num11 = len;','int num11 = len; System.Threading.ThreadStart cb=delegate{};'),api.transform_textblock_draw_composition),('image-other-access-arm',image.replace('image.optimizeAccessL_[num28]','image.optimizeAccess_[num28]'),api.transform_drawimage_composition),('image-other-sentinel-comparison',image.replace('if (num25 == -1)','if (num25 < 0)'),api.transform_drawimage_composition),('image-lookup-capture',image.replace('int num25 = -1;','int num25 = -1; System.Threading.ThreadStart cb=delegate{};'),api.transform_drawimage_composition),('image-lookup-goto',image.replace('int num25 = -1;','int num25 = -1; goto stable; stable:;'),api.transform_drawimage_composition)]
 for label,bad,transform in tests:
  try:transform(bad)
  except(ValueError,AssertionError):negatives.append(label)
  else:raise AssertionError(label)
 report={'operation':'historical-csc-text-image-own-template-fixtures','compiler_sha256':manifest['compiler_sha256'],'producer_sha256':sha(ROOT/'tools/round9_text_image_source.py'),'driver_sha256':sha(Path(__file__)),'game_compilations':0,'toy_compilations':9,'positive_full_bodies':len(rows),'negative_controls':len(negatives),'negatives':negatives,'fixtures':rows,'pre_hardening_pure_guard_control':legacy,'hardening_negative_rejected':True,'limitations':'Templates were supplied untested and reviewed by executor. One v1 pure API guard failure measured, corrected before any game compilation; this is not a failed game candidacy. Same Run name/type per binary avoids signature filtering. Original game header/locals/IL/EH mandatory.'}
 (ROOT/'reports/round9-text-image-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in report.items()if k not in('fixtures','pre_hardening_pure_guard_control')}))
if __name__=='__main__':main()

