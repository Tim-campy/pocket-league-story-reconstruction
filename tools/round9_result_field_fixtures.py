"""Own historical-csc controls for two field-outer cached result-array chains."""
import json,re,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import differences,sha
from round7_roslyn_for_declaration_fixture import compile_fixture
import round9_result_field_source as api
PATTERN=re.compile(r'(?m)^[ \t]*public[ \t]+void[ \t]+(CurrentMaterial|ExpectedMaterial|CurrentOperators|ExpectedOperators)\s*\(')
def main():
 work=ROOT/'build/round9/result-field-fixtures';path=ROOT/'tools/fixtures/round9-result-field/FixtureResultField.cs';source=path.read_text(encoding='utf-8-sig');manifest=json.loads((ROOT/'build/round9/cycles/cycle39/after/KairoLibrary/build-manifest.json').read_text(encoding='utf-8-sig'));spans={}
 for m in PATTERN.finditer(source):
  end=api.close(source,source.index('{',m.end()));spans[m.group(1)]=(m.start(),end,source[m.start():end])
 assert len(spans)==4
 def rename(s):return re.sub(r'\b(CurrentMaterial|ExpectedMaterial|CurrentOperators|ExpectedOperators)(?=\s*\()', 'Run',s,count=1)
 def assemble(method,current):
  result=source
  for n,(a,b,_)in sorted(spans.items(),key=lambda p:p[1][0],reverse=True):result=result[:a]+(method if n==current else '')+result[b:]
  return result
 def compile_run(label,method,current):
  rows,_=compile_fixture(work/label,assemble(method,current),manifest);hits=[m for k,m in rows.items()if'::Run('in k];assert len(hits)==1;return hits[0]
 rows=[];cases=[];components=[]
 for suffix in ['Material','Operators']:
  current='Current'+suffix;before=rename(spans[current][2]);expected=rename(spans['Expected'+suffix][2]);after=api.transform_renderpass_resultcolors_chain(before)
  old=compile_run(suffix+'-before',before,current);want=compile_run(suffix+'-control',expected,current);got=compile_run(suffix+'-candidate',after,current)
  assert differences(old,want)[0];assert not differences(want,got)[0],(suffix,differences(want,got)[0]);cases.append(before)
  rows.append({'case':suffix,'full_body_equal':True,'before_body_different':True,'before_local_count':len(old['locals']),'after_local_count':len(got['locals']),'before':before,'after':after,'manual_control':expected})
 before=cases[0];tests=[('extra-array-use',before.replace('resultColors_ = array;','resultColors_ = array;Consume(array);')),('between-effect',before.replace('resultColors_ = array;','Effects++;resultColors_ = array;')),('captured-array',before.replace('resultColors_ = array;','System.Action capture=delegate{array=null;};resultColors_ = array;')),('protected-observation',before.replace('resultColors_ = array;','try{}finally{}resultColors_ = array;')),('other-allocation-length',before.replace('new Color[Colors.Length]','new Color[colors.Length]')),('other-array-type',before.replace('Color[] array','object array')),('property-receiver',before.replace('resultColors_ = array;','Other.resultColors_ = array;'))];negatives=[]

 for label,bad in tests:
  try:api.transform_renderpass_resultcolors_chain(bad)
  except(AssertionError,ValueError):negatives.append(label)
  else:raise AssertionError(label)
 report={'operation':'historical-csc-field-outer-result-chain-fixtures','compiler_sha256':manifest['compiler_sha256'],'producer_sha256':sha(ROOT/'tools/round9_result_field_source.py'),'driver_sha256':sha(Path(__file__)),'template_sha256':sha(path),'game_compilations':0,'toy_compilations':6,'positive_full_bodies':len(rows),'negative_controls':len(negatives),'negatives':negatives,'fixtures':rows,'limitations':'Two adjacent statements merge into instance-field outer assignment. The named array must have no other use; no captures/protected observer/property receiver. Full real local signature/maxstack/body mandatory; not the indexer-local-inner rule.'}
 (ROOT/'reports/round9-result-field-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in report.items()if k!='fixtures'}))
if __name__=='__main__':main()
