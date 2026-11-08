"""Restore two for-condition guards and explicit stores, preserving leaves."""
import re
RULE='r12-boot-draw-for-condition-guards-and-explicit-selection-stores'
METHOD_ID='form.BootForm::Draw(kairo.unity.ui.Graphics)->System.Void'
def transform(method):
 text=method.replace('\r\n','\n')
 for name in ('i','j'):
  pattern=re.compile(r'for \(int '+name+r' = 4; '+name+r' < frame_; '+name+r' \+= 4\)\s*\{\s*if \(bowWait_ != 0\)\s*\{\s*break;\s*\}')
  text,count=pattern.subn('for (int '+name+' = 4; '+name+' < frame_ && bowWait_ == 0; '+name+' += 4)\n{',text)
  if count!=1:raise ValueError('one exact loop guard required: '+name)
 pattern=re.compile(r'empty = \(\(!MyFormBase\.Jp\(\)\) \? AppData\.resourceString\[6\] : (?P<expression>MyFormBase\.LT\("[^"\n]+"\))\);')
 def replace(m):return 'if (MyFormBase.Jp())\n{\nempty = '+m['expression']+';\n}\nelse\n{\nempty = AppData.resourceString[6];\n}'
 text,count=pattern.subn(replace,text)
 if count!=1:raise ValueError('one exact selection required')
 return text
