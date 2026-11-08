"""Pure source composition: distinct empty switch sections and measured empty test."""
import re
METHOD_ID='form.SoccerForm::OnTouchCamera(System.Int32,System.Int32,kairo.unity.surface.TouchEvent)->System.Void'
RULE='r12-touchcamera-empty-cases-0-8-and-zero-test'

def closing(source,start):
    depth=0
    for i in range(start,len(source)):
        if source[i]=='{':depth+=1
        elif source[i]=='}':
            depth-=1
            if depth==0:return i
    raise ValueError('unbalanced braces')

def restore_empty_cases(source):
    if source.count('switch (type)')!=1:raise ValueError('unique switch(type) required')
    if re.findall(r'\bcase\s+([0-9]+)\s*:',source)!=['5','1','12']:
        raise ValueError('measured case order/inventory differs')
    anchor='case 5:'
    return source.replace(anchor,'case 0:\r\nbreak;\r\ncase 8:\r\nbreak;\r\n'+anchor,1)

def restore_empty_zero_test(source):
    anchor='if (touchCameraPoint_.Size() != 0)'
    if source.count(anchor)!=1:raise ValueError('unique measured point-count test required')
    st=source.index(anchor);op=source.index('{',st);en=closing(source,op)
    if source[op+1:en].strip():raise ValueError('test body must remain empty')
    return source.replace(anchor,'if (touchCameraPoint_.Size() == 0)',1)

def transform_touchcamera(source):return restore_empty_zero_test(restore_empty_cases(source))
