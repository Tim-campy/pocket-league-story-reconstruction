"""Own historical-csc controls for two Dictionary/local assignment chains."""
import json,re,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import differences,sha
from round7_roslyn_for_declaration_fixture import compile_fixture
import round9_indexer_inner_source as api
PATTERN=re.compile(r'(?m)^[ \t]*public[ \t]+Material[ \t]+(CurrentGraphics|ExpectedGraphics|CurrentShader|ExpectedShader)\s*\(')
def main():
 work=ROOT/'build/round9/indexer-inner-fixtures';path=ROOT/'tools/fixtures/round9-indexer-inner/FixtureIndexer.cs';source=path.read_text(encoding='utf-8-sig');manifest=json.loads((ROOT/'build/round9/cycles/cycle38/after/KairoLibrary/build-manifest.json').read_text(encoding='utf-8-sig'));spans={}
 for m in PATTERN.finditer(source):
  end=api.close(source,source.index('{',m.end()));spans[m.group(1)]=(m.start(),end,source[m.start():end])
 assert len(spans)==4
 def rename(s):return re.sub(r'\b(CurrentGraphics|ExpectedGraphics|CurrentShader|ExpectedShader)(?=\s*\()', 'Run',s,count=1)
 def assemble(method,current):
  result=source
  for n,(a,b,_)in sorted(spans.items(),key=lambda p:p[1][0],reverse=True):result=result[:a]+(method if n==current else '')+result[b:]
  return result
 def compile_run(label,method,current):
  rows,_=compile_fixture(work/label,assemble(method,current),manifest);hits=[m for k,m in rows.items()if'::Run('in k];assert len(hits)==1;return hits[0]
 rows=[];cases=[];components=[]
 dictionary_old='value = (materlas_[key] = new Dictionary<string, Material>());';dictionary_new='materlas_[key] = (value = new Dictionary<string, Material>());'
 material_old='value2 = (value[text] = new Material(shader, image));';material_new='value[text] = (value2 = new Material(shader, image));'
 for suffix in ['Graphics','Shader']:
  current='Current'+suffix;before=rename(spans[current][2]);expected=rename(spans['Expected'+suffix][2]);after=api.transform_material_indexer_local_inner(before)
  old=compile_run(suffix+'-before',before,current);want=compile_run(suffix+'-control',expected,current);got=compile_run(suffix+'-candidate',after,current)
  assert differences(old,want)[0];assert not differences(want,got)[0],(suffix,differences(want,got)[0]);cases.append(before)
  rows.append({'case':suffix,'full_body_equal':True,'before_body_different':True,'before_local_count':len(old['locals']),'after_local_count':len(got['locals']),'before':before,'after':after,'manual_control':expected})
  for label,partial in [('dictionary-only',before.replace(dictionary_old,dictionary_new)),('material-only',before.replace(material_old,material_new))]:
   partial_body=compile_run(suffix+'-'+label,partial,current);delta=differences(want,partial_body)[0];components.append({'case':suffix,'component':label,'equal_to_control':not bool(delta),'differences':delta})
 before=cases[0];tests=[('captured-local',before.replace('Material value2 = null;','Material value2 = null; System.Action capture=delegate{value2=null;};')),('protected-observation',before.replace('Material value2 = null;','Material value2 = null;try{}finally{}')),('other-constructor',before.replace('new Material(shader, image)','new Material(shader, another)')),('index-call',before.replace('materlas_[key]','materlas_[GetKey()]')),('missing-pair',before.replace(material_old,'value2 = new Material(shader, image);')),('other-local-type',before.replace('Material value2 = null;','object value2 = null;')),('custom-local-indexer',before.replace('Dictionary<string, Material> value;','CustomMap value;'))];negatives=[]
 for label,bad in tests:
  try:api.transform_material_indexer_local_inner(bad)
  except(AssertionError,ValueError):negatives.append(label)
  else:raise AssertionError(label)
 report={'operation':'historical-csc-indexer-local-inner-pair-fixtures','compiler_sha256':manifest['compiler_sha256'],'producer_sha256':sha(ROOT/'tools/round9_indexer_inner_source.py'),'driver_sha256':sha(Path(__file__)),'template_sha256':sha(path),'game_compilations':0,'toy_compilations':10,'positive_full_bodies':len(rows),'negative_controls':len(negatives),'negatives':negatives,'fixtures':rows,'isolated_components':components,'limitations':'Narrow named Dictionary locals and exact two assignment statements, no captures/protected observers. Arbitrary custom setters/properties are not a general semantics-preservation claim. Full actual game signature/header/EH required.'}
 (ROOT/'reports/round9-indexer-inner-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in report.items()if k!='fixtures'}))
if __name__=='__main__':main()
