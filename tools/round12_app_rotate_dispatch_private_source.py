"""Pure calculation of measured integer dispatch order and interval test."""
METHOD_ID='game.UserData::RotateTestPlayParam()->System.Void'
RULE='r12-rotate-empty-123-before-inclusive-47-range'
OLD='''if ((i == 0)) { AddTestPlayerParam(i, (int)apdat_.save_.lngs[0]);
continue; } else if ((i == 4) || (i == 5) || (i == 6) || (i == 7)) { AddTestPlayerParam(i, researchPoint_[i - 4]);
continue; } else if ((i == 1) || (i == 2) || (i == 3)) { continue; }'''

def transform_rotate(source,order=True,interval=True):
    # Existing short control insertion uses LF; preserve every other source byte.
    if source.count(OLD)!=1:raise ValueError('unique measured control chain required')
    if source.count('for (int i = 0; i < 9; i++)')!=1:raise ValueError('measured Int32 loop required')
    first='if ((i == 0)) { AddTestPlayerParam(i, (int)apdat_.save_.lngs[0]);\ncontinue; }'
    skip='else if ((i == 1) || (i == 2) || (i == 3)) { continue; }'
    test='(i >= 4) && (i <= 7)'if interval else'(i == 4) || (i == 5) || (i == 6) || (i == 7)'
    research='else if ('+test+') { AddTestPlayerParam(i, researchPoint_[i - 4]);\ncontinue; }'
    replacement=first+' '+skip+' '+research if order else first+' '+research+' '+skip
    return source.replace(OLD,replacement)

def closing(source,start):
    depth=0
    for i in range(start,len(source)):
        if source[i]=='{':depth+=1
        elif source[i]=='}':
            depth-=1
            if depth==0:return i
    raise ValueError('unbalanced braces')
