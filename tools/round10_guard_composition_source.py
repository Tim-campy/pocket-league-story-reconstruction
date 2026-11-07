"""Pure source hypotheses for two guarded bodies with additional slot differences.

No I/O or compilation. Original CIL guides statement order; only verification
can establish a target body match. Floating-point comparisons are preserved.
"""
import round9_whole_guard_source as api


def transform_sp_constructor(source):
    before='string text2 = text_;\n\t\t\t\ttext_ = text2 + text + "," + Http.GetErrorCode() + "\\n";'
    # Compare token spans, preserving surrounding trivia independently of LF/CRLF.
    tokens=api.tokens(source);wanted=api.words(before)
    hits=[n for n in range(len(tokens)-len(wanted)+1)
          if [t for t,_,_ in tokens[n:n+len(wanted)]]==wanted]
    if len(hits)!=1:raise ValueError('one-string-field-snapshot-required')
    n=hits[0];start=tokens[n][1];end=tokens[n+len(wanted)-1][2]
    changed=source[:start]+'text_ += text + "," + Http.GetErrorCode() + "\\n";'+source[end:]
    return api.positive_tail(changed,'if (th == null)','th != null','if (Config.PTW)')


def transform_canvas(source):
    api.no_capture(source)
    marker='int num3 = 0;'
    start=api.once(source,marker);method_end=api.close(source,source.index('{'))
    depth=api.mask(source[:start]).count('{')-api.mask(source[:start]).count('}')
    if depth!=1:raise ValueError('late-touch-count-must-start-in-method-block')
    suffix=source[start:method_end-1]
    if api.words(suffix).count('num3')<3:raise ValueError('late-touch-count-uses-missing')
    changed=source[:start]+'{\n'+suffix+'\n}\n'+source[method_end-1:]
    old='value = (onTouchPointer_[touch.fingerId] = new float[4]);'
    p=api.once(changed,old)
    changed=changed[:p]+'value = new float[4];\n onTouchPointer_[touch.fingerId] = value;'+changed[p+len(old):]
    guard='if (invalidTouch_ || Input.touchCount <= 0 || IApplication.GetCurrentApp().GetTotalFrames() <= 10)'
    p=api.once(changed,guard);opening=changed.index('{',p);end=api.close(changed,opening)
    if api.words(changed[opening:end])!=['{','return',';','}']:raise ValueError('merged-guard-must-only-return')
    method_end=api.close(changed,changed.index('{'))
    tail=changed[end:method_end-1]
    if api.words(tail)[:len(api.words('List<int[]> list = new List<int[]>();'))]!=api.words('List<int[]> list = new List<int[]>();'):
        raise ValueError('touch-list-tail-entry-mismatch')
    replacement='if (invalidTouch_) { return; }\nif (Input.touchCount > 0 && IApplication.GetCurrentApp().GetTotalFrames() > 10) {\n'+tail+'\n}\n'
    result=changed[:p]+replacement+changed[method_end-1:]
    if api.inventories(source)!=api.inventories(result):raise ValueError('calls-or-literals-changed')
    if api.close(result,result.index('{'))!=len(result):raise ValueError('method-boundary-changed')
    return result
