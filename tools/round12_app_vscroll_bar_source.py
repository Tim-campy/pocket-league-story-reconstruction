"""Two bounded, pure source calculations; controls are deliberately separate."""
import re
IDS={name:'surface.GameView::OnTouchVScroll'+suffix+'(System.Int32,System.Int32,kairo.unity.surface.TouchEvent)->System.Void'
     for name,suffix in [('button','Button'),('bar','Bar')]}
RULES={'button':'r12-vscroll-button-ternary-and-single-case-switch',
       'bar':'r12-vscroll-bar-ternary-and-nested-positive-guards'}

def closing(source,start):
    depth=0
    for i in range(start,len(source)):
        if source[i]=='{':depth+=1
        elif source[i]=='}':
            depth-=1
            if depth==0:return i
    raise ValueError('unbalanced braces')

def restore_ternary(source,kind):
    expression='option.args'if kind=='button'else'args'
    part=re.escape(expression)
    pattern=r'int num3;\s*if \(2 < '+part+r'\.Length\) \{ num3 = '+part+r'\[2\]; \} else \{ num3 = \(-1\); \}'
    if len(re.findall(pattern,source))!=1:raise ValueError('unique measured two-arm assignment required')
    return re.sub(pattern,'int num3 = (2 < '+expression+'.Length) ? '+expression+'[2] : -1;',source)

def button_switch(source):
    anchor='if (type == 13)'
    if source.count(anchor)!=1:raise ValueError('unique type13 control required')
    st=source.index(anchor);op=source.index('{',st);en=closing(source,op)
    body=source[op+1:en]
    if re.search(r'\b(?:return|break|continue|goto)\b',body):raise ValueError('unexpected owner/transfer in button body')
    if source[en+1:source.rfind('}')].strip():raise ValueError('unexpected tail after button control')
    return source[:st]+'switch (type) {\r\ncase 13:\r\n'+body+'\r\nbreak;\r\n}'+source[en+1:]

def bar_positive_guards(source):
    guards=[('if (Get(v) == num4)','if (Get(v) != num4)'),('if (num3 == -1)','if (num3 != -1)')]
    for old,new in guards:
        if source.count(old)!=1:raise ValueError('unique measured early guard required')
        st=source.index(old);op=source.index('{',st);end=closing(source,op)
        if source[op+1:end].strip()!='return;':raise ValueError('guard not a return-only block')
        tail_end=source.rfind('}')
        source=source[:st]+new+' {\r\n'+source[end+1:tail_end]+'\r\n}'+source[tail_end:]
    return source

def transform_vscroll_button(source):return button_switch(restore_ternary(source,'button'))
def transform_vscroll_bar(source):return bar_positive_guards(restore_ternary(source,'bar'))
