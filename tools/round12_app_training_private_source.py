"""Pure source producer for one existing num18 loop only; no compiler or I/O."""
import re
RULE='r12-training-num18-size-in-loop-condition'
METHOD_ID='game.UserData::SetTrainingCommonMethod(System.Boolean)->System.Void'

def transform_training_num18_condition(method_source):
    pattern=re.compile(r'for \(int num18 = 0; num18 < areaMaxNum; num18\+\+\)(?P<space>\s*)\{(?P<bodyspace>\s*)if \(array\[num14\].Size\(\) <= 0\)\s*\{\s*break;\s*\}')
    matches=list(pattern.finditer(method_source))
    if len(matches)!=1:raise ValueError('one exact num18 loop with first size guard required')
    m=matches[0]
    if method_source.count('int areaMaxNum2 = TGroupArea.GetAreaMaxNum(tModeList_[num14][num17]);')!=1:
        raise ValueError('original first body declaration required')
    if not re.match(r'\s*int areaMaxNum2 = TGroupArea.GetAreaMaxNum\(tModeList_\[num14\]\[num17\]\);',method_source[m.end():]):
        raise ValueError('no intervening leaf may move across size check')
    replacement='for (int num18 = 0; num18 < areaMaxNum && array[num14].Size() > 0; num18++)'+m['space']+'{'+m['bodyspace']
    return method_source[:m.start()]+replacement+method_source[m.end():]

def closing(source,start):
    tokens=re.finditer(r'//[^\r\n]*|/\*[\s\S]*?\*/|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'|[{}]',source[start:]);depth=0
    for m in tokens:
        if m[0]=='{':depth+=1
        elif m[0]=='}':
            depth-=1
            if depth==0:return start+m.start()
    raise ValueError('unbalanced source block')
