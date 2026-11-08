"""Private, header-agnostic source proposals; import/calls perform no I/O."""
import re

IMAGE_RULE='r11-array-element-explicit-mask-assignment'
LANGUAGE_RULE='r11-softlabel-input-alias-before-table-loops'

def _once(source,old,new):
    if source.count(old)!=1:raise ValueError('unique source anchor required: '+old)
    return source.replace(old,new,1)

def transform_image_explicit_mask(method_source):
    # Only array/local i accesses, with no call, property or captured mutation
    # in either receiver or index; the original already evaluates this twice.
    return _once(method_source,'aRGBPixels[i] &= 16777215;',
                 'aRGBPixels[i] = aRGBPixels[i] & 16777215;')

def transform_language_input_alias(method_source):
    name='softLabelInput'
    if re.search(r'\b'+name+r'\b',method_source):raise ValueError('fresh alias name required')
    guard=re.compile(r'if\s*\(softlabel == null \|\| softlabel.Length > 10\)\s*\{\s*return softlabel;\s*\}')
    guards=list(guard.finditer(method_source))
    if len(guards)!=1:raise ValueError('unique original argument guard required')
    guard=guards[0];head=method_source[:guard.end()];tail=method_source[guard.end():]
    if tail.count('softlabel.Length')!=2 or tail.count('softlabel.Equals(')!=2:
        raise ValueError('two original table comparison pairs required')
    if not re.search(r'return softlabel;\s*\}\s*$',tail):
        raise ValueError('original fallback argument return required')
    tail=tail.replace('softlabel.Length',name+'.Length').replace('softlabel.Equals(',name+'.Equals(')
    newline='\r\n'if'\r\n'in method_source else'\n'
    return head+newline+'string '+name+' = softlabel;'+tail
