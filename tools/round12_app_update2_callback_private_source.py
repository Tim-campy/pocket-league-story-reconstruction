"""Pure normal-parent source changes; no generated class is written."""
METHOD_ID='form.SubForm/<Update2>c__AnonStorey0::<>m__0(System.String)->System.Void'
PARENT_ID='form.SubForm::Update2()->System.Boolean'
RULE='r12-update2-first-name-callback-unused-slot-and-branch-stores'
TERNARY='num39 = ((!MyFormBase.Jp()) ? 8 : ((stringLength != 1) ? 4 : 5));'
BRANCHES='''if (MyFormBase.Jp()) {
 if (stringLength == 1) { num39 = 5; } else { num39 = 4; }
} else { num39 = 8; }'''

def closing(source,start):
    """Brace matching with quoted literals and comments skipped."""
    depth=0;i=start
    while i<len(source):
        c=source[i]
        if source.startswith('//',i):
            j=source.find('\n',i);i=len(source)if j<0 else j+1;continue
        if source.startswith('/*',i):
            j=source.find('*/',i+2)
            if j<0:raise ValueError('unterminated comment')
            i=j+2;continue
        if c in ('"',"'"):
            quote=c;verbatim=c=='"'and i>0 and source[i-1]=='@';i+=1
            while i<len(source):
                if not verbatim and source[i]=='\\':i+=2;continue
                if source[i]==quote:
                    if verbatim and i+1<len(source)and source[i+1]==quote:i+=2;continue
                    i+=1;break
                i+=1
            continue
        if c=='{':depth+=1
        elif c=='}':
            depth-=1
            if depth==0:return i
        i+=1
    raise ValueError('unbalanced braces')

def callback_span(source):
    if source.count(TERNARY)!=1:raise ValueError('unique measured nested ternary required')
    pos=source.index(TERNARY);anchor='delegate(string temp_name)';start=source.rfind(anchor,0,pos)
    if start<0 or source[:start].count(anchor)!=0:raise ValueError('must be first source callback')
    op=source.index('{',start);end=closing(source,op)+1
    if not op<pos<end:raise ValueError('ternary not in measured callback')
    body=source[op+1:end-1]
    required=['if (temp_name != null)','t_data.name_ = JString.New(temp_name);',
        'apdat_.frmTitle_.newGameName_ = JString.New(temp_name);','string text = string.Empty;',
        'int num38 = 0;','int num39 = 0;','for (int num40 = 0; num40 < temp_name.Length(); num40++)',
        't_data.name2_ = text;']
    if any(body.count(s)!=1 for s in required):raise ValueError('callback leaves/declarations differ')
    if 'recoveredUnusedNameWidth'in source:raise ValueError('reserved-name collision')
    return start,end

def transform_update2_callback(source,reserve=True,branches=True):
    start,end=callback_span(source);callback=source[start:end]
    if reserve:callback=callback.replace('string text = string.Empty;','int recoveredUnusedNameWidth;\r\nstring text = string.Empty;',1)
    if branches:callback=callback.replace(TERNARY,BRANCHES,1)
    return source[:start]+callback+source[end:]
