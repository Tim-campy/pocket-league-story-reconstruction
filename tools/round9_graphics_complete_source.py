"""New WIP composition from unchanged Main, never an adoption of rejected C14.

Pure component API above main; no file I/O/import side effects. No C# compiler,
transformer, model or verifier is invoked anywhere in this module.
"""
from collections import Counter
import re

TOKENS=re.compile(r'//[^\r\n]*|/\*[\s\S]*?\*/|@"(?:""|[^"])*"|"(?:\\.|[^"\\])*"|\x27(?:\\.|[^\x27\\])*\x27|[A-Za-z_][A-Za-z_0-9]*|[^\s]',re.M)
def tokens(source):return [(m.group(),m.start(),m.end())for m in TOKENS.finditer(source)if not m.group().startswith(('//','/*'))]
def mask(source):
 result=list(source)
 for m in TOKENS.finditer(source):
  if m.group().startswith(('//','/*','"','@"',"'")):
   for i in range(m.start(),m.end()):
    if result[i]not in '\r\n':result[i]=' '
 return ''.join(result)
def literal_inventory(source):return Counter(t for t,_,_ in tokens(source)if t.startswith(('"','@"',"'")))
def call_inventory(source):
 ts=[t for t,_,_ in tokens(source)]
 return Counter(t for i,t in enumerate(ts[:-1])if re.fullmatch('[A-Za-z_][A-Za-z_0-9]*',t)and ts[i+1]=='('and t not in {'if','for','foreach','while','switch','catch','using','lock','typeof','sizeof','default','checked','unchecked'})
def numeric_inventory(source):
 return Counter(re.findall(r'(?<![\w.])(?:0[xX][0-9a-fA-F]+|\d+(?:\.\d*)?(?:[eE][+-]?\d+)?)[fFdDmMuUlL]*(?![\w.])',mask(source)))

def transform_graphics_final_negation(method_source):
 """Pure component, usable in Fixture.Run; declaration name/type is irrelevant.

 The field/variable anchor is intentionally exact. Semantic float evidence for
 the actual game is external (preserved C14 memberref says m13:System.Single).
 An arbitrary caller must supply its own type proof/fixture before adoption.
 """
 before='matrix.m13 = 0f - matrix.m13;';after='matrix.m13 = -matrix.m13;'
 assert method_source.count(before)==1,'require one exact matrix.m13 subtraction'
 assert len(re.findall(r'\bMatrix4x4\s+matrix\s*=',mask(method_source)))==1,'require one declared Matrix4x4 matrix local'
 result=method_source.replace(before,after,1)
 assert literal_inventory(method_source)==literal_inventory(result),'string/character literals changed'
 assert call_inventory(method_source)==call_inventory(result),'call-name inventory changed'
 old,new=numeric_inventory(method_source),numeric_inventory(result)
 assert old-new==Counter({'0f':1})and not(new-old),'only permitted numeric leaf change is removal of one0f'
 return result

from round9_boolean_composition_source import transform_graphics as _compose_blocks

def transform_graphics_complete(method_source):
 return transform_graphics_final_negation(_compose_blocks(method_source))
