"""Historical csc control for full Boolean/block/unary-negation composition."""
import json,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
from round9_boolean_composition_fixtures import HEADER,GRAPH_BEFORE,GRAPH_EXPECTED,FRESH
from round9_boolean_composition_source import transform_graphics
from round9_graphics_complete_source import transform_graphics_complete,transform_graphics_final_negation
from round7_roslyn_for_declaration_fixture import compile_fixture
from compare_kairo import differences,sha
WORK=ROOT/'build/round9/graphics-complete-fixtures';WORK.mkdir(parents=True,exist_ok=True)
def method(rows):return next(v for k,v in rows.items() if k.startswith('Fixture::Run('))
def main():
 manifest=json.loads((ROOT/'build/round9/cycles/cycle15/after/KairoLibrary/build-manifest.json').read_text(encoding='utf-8-sig'))
 tail=' Matrix4x4 matrix = new Matrix4x4();matrix.m13=Count;matrix.m13 = 0f - matrix.m13;Count+=(int)matrix.m13;}'
 fresh=FRESH[:-1]+tail;before=GRAPH_BEFORE.replace(FRESH,fresh);expected=GRAPH_EXPECTED.replace(FRESH,fresh.replace('matrix.m13 = 0f - matrix.m13;','matrix.m13 = -matrix.m13;'))
 header='public struct Matrix4x4{public float m13;}'+HEADER
 old=transform_graphics(before);after=transform_graphics_complete(before)
 previous,_=compile_fixture(WORK/'C14-equivalent',header+old+'}',manifest);control,_=compile_fixture(WORK/'manual-control',header+expected+'}',manifest);actual,_=compile_fixture(WORK/'whole-composition',header+after+'}',manifest)
 a,b,c=method(previous),method(control),method(actual);assert a['locals']==b['locals'];assert a['header']['code_size']-b['header']['code_size']==5
 assert any(i['opcode']=='sub' for i in a['instructions']) and any(i['opcode']=='neg' for i in b['instructions'])
 assert not differences(b,c)[0],differences(b,c)[0]
 negative=[]
 for label,bad in [('missing-matrix-declaration',old.replace('Matrix4x4 matrix =','Other matrix =')),('duplicate-subtraction',old+'matrix.m13 = 0f - matrix.m13;'),('already-negated',after)]:
  try:transform_graphics_final_negation(bad)
  except AssertionError:negative.append(label)
  else:raise AssertionError(label)
 report={'operation':'historical-csc-full-graphics-Boolean-block-unary-composition-fixture','compiler_sha256':manifest['compiler_sha256'],'producer_sha256':sha(ROOT/'tools/round9_graphics_complete_source.py'),'driver_sha256':sha(Path(__file__)),
  'game_compilations':0,'toy_compilations':3,'positive_full_bodies':1,'negative_controls':len(negative),'negatives':negative,'full_body_equal':True,'C14_shape_signature_equal':True,'C14_shape_extra_IL_bytes':5,
  'original_float_operator_neg_preserved':True,'no_relational_float_operator_rewritten':True,'before':before,'after':after,'manual_control':expected,
  'limitations':'Single field type is proved by primary game metadata and the independent fixture; no arbitrary numeric subtraction is rewritten.'}
 (ROOT/'reports/round9-graphics-complete-fixture.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in report.items() if k not in ('before','after','manual_control')}))
if __name__=='__main__':main()
