"""Pure explicit Vector3D-array construction and measured unused slot."""
import re
RULE='r12-gameform-init-explicit-vector-elements-and-unused-single'
METHOD_ID='form.GameForm::Init()->System.Boolean'

def transform(source):
 pattern=re.compile(r'Vector3D\[\] array = new Vector3D\[4\]\s*\{(?P<items>[\s\S]*?)\};')
 matches=list(pattern.finditer(source))
 if len(matches)!=1:raise ValueError('One exact four-Vector3D initializer required')
 m=matches[0];items=re.findall(r'new Vector3D\([^()]*\)',m['items'])
 if len(items)!=4 or re.sub(r'new Vector3D\([^()]*\)','',m['items']).strip(' \t\r\n,'):raise ValueError('Only four simple constructors supported')
 expected=['new Vector3D(0f, 0f, 0f)','new Vector3D(900f, 0f, 0f)','new Vector3D(900f, 0f, 900f)','new Vector3D(0f, 0f, 900f)']
 if [''.join(x.split())for x in items]!=[''.join(x.split())for x in expected]:raise ValueError('Measured coordinate literals required')
 after='float recoveredUnusedScale;\nVector3D[] array = new Vector3D[4];\n'+'\n'.join('array['+str(i)+'] = '+item+';'for i,item in enumerate(items))
 return source[:m.start()]+after+source[m.end():]
