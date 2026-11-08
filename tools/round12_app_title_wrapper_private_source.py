"""Pure source rule: one named selector/compound if back to one-case switch."""
import re
RULE = 'r12-title-single-case-switch-emission-temp'
METHOD_ID = 'form.TitleForm::OnTouchEvent(kairo.unity.surface.TouchEvent)->System.Boolean'

def close(source,start):
 depth=0
 for m in re.finditer(r'//[^\r\n]*|/\*[\s\S]*?\*/|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'|[{}]',source[start:]):
  if m[0]=='{':depth+=1
  elif m[0]=='}':
   depth-=1
   if not depth:return start+m.start()
 raise ValueError('unbalanced source')

def transform(method):
 pattern=re.compile(r'int id = component\.id;\s*if \(id == 27 && e\.type == 1\)\s*\{')
 matches=list(pattern.finditer(method))
 if len(matches)!=1:raise ValueError('one exact selector and compound condition required')
 m=matches[0];end=close(method,m.end()-1)
 body=method[m.end():end]
 if not re.search(r'return false;\s*$',body):raise ValueError('case-local false return required')
 replacement='switch (component.id)\n{\ncase 27:\nif (e.type == 1)\n{'+body+'\n}\nbreak;\n}'
 return method[:m.start()]+replacement+method[end+1:]
