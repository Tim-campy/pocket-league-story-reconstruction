"""Protected guards and array aliases. Pure source only; no generated classes."""
import re
class Refused(ValueError):pass
def unique(pattern,text):
    found=list(re.finditer(pattern,text))
    if len(found)!=1:raise Refused('anchor-count-%s:%s'%(len(found),pattern))
    return found[0]
def closing(text,start):
    if text[start]!='{':raise Refused('not-block')
    depth=0;quote=None;escape=False;i=start
    while i<len(text):
        c=text[i]
        if quote:
            if escape:escape=False
            elif c=='\\':escape=True
            elif c==quote:quote=None
        elif text.startswith('//',i):
            j=text.find('\n',i+2);i=len(text)if j<0 else j;continue
        elif text.startswith('/*',i):
            j=text.find('*/',i+2)
            if j<0:raise Refused('unclosed-comment')
            i=j+2;continue
        elif c in('"',"'"):quote=c
        elif c=='{':depth+=1
        elif c=='}':
            depth-=1
            if depth==0:return i
        i+=1
    raise Refused('unclosed-block')

def transform_sound_protected_returns(source):
    nl='\r\n'if'\r\n'in source else'\n'
    first=unique(r'if\s*\(base\.track_\s*!=\s*null\s*&&\s*base\.state_\s*==\s*2\s*&&\s*base\.track_\.isPlaying\)\s*\{',source)
    start=source.index('{',first.start());end=closing(source,start)
    leaf=source[start+1:end]
    if not re.fullmatch(r'\s*SetVolume\(volume,\s*mute\)\s*;\s*base\.track_\.time\s*=\s*0f\s*;\s*',leaf):raise Refused('playing-arm-leaves-differ')
    tail=re.match(r'\s*else\s*\{',source[end+1:])
    if tail is None:raise Refused('playing-else-missing')
    else_open=end+1+tail.end()-1;else_end=closing(source,else_open)
    # Preserve the old else as a plain block, with the same declarations.
    source=source[:end]+nl+'return;'+nl+source[end:end+1]+nl+source[else_open:else_end+1]+source[else_end+1:]
    mute_guard=unique(r'if\s*\(!\(type_\s*==\s*1\s*&&\s*mute\)\)\s*\{',source)
    open_brace=source.index('{',mute_guard.start());source=source[:mute_guard.start()]+'if (type_ == 1 && mute) { return; }'+nl+source[open_brace:]
    port_guard=unique(r'if\s*\(port\s*!=\s*-1\)\s*\{',source)
    open_brace=source.index('{',port_guard.start());source=source[:port_guard.start()]+'if (port == -1) { return; }'+nl+source[open_brace:]
    # The single try in this parent must enclose all three introduced exits.
    attempt=unique(r'\btry\s*\{',source);try_open=source.index('{',attempt.start());try_end=closing(source,try_open)
    if not(try_open<first.start()<try_end):raise Refused('playing-arm-not-in-try')
    if len(re.findall(r'\breturn\s*;',source[try_open:try_end]))!=3:raise Refused('unexpected-protected-return-count')
    return source

def transform_shader_array_alias_and_ordered_selector(source):
    name='recoveredShaderColors';nl='\r\n'if'\r\n'in source else'\n'
    if re.search(r'\b'+name+r'\b',source):raise Refused('array-alias-name-not-fresh')
    loop=unique(r'for\s*\(int\s+i\s*=\s*0\s*;\s*i\s*<\s*colors\.Length\s*;\s*i\+\+\)',source)
    source=source[:loop.start()]+'Color[] '+name+' = colors;'+nl+'for (int i = 0; i < '+name+'.Length; i++)'+source[loop.end():]
    element=unique(r'Color\s+color\s*=\s*colors\[i\]\s*;',source)
    source=source[:element.start()]+'Color color = '+name+'[i];'+source[element.end():]
    dispatch=unique(r'switch\s*\(opaTransOrder\)\s*\{',source);end=closing(source,source.index('{',dispatch.start()))+1;old=source[dispatch.start():end]
    if not re.fullmatch(r'switch\s*\(opaTransOrder\)\s*\{\s*case\s+1\s*:\s*alphaType\s*=\s*1\s*;\s*break\s*;\s*case\s+2\s*:\s*alphaType\s*=\s*2\s*;\s*break\s*;\s*\}',old):raise Refused('selector-arms-differ')
    return source[:dispatch.start()]+'if (opaTransOrder == 1) { alphaType = 1; } else if (opaTransOrder == 2) { alphaType = 2; }'+source[end:]

def transform_save_cancel_returns_and_log_scope(source):
    nl='\r\n'if'\r\n'in source else'\n'
    declared=unique(r'AutoSaveLogData\s+log\s*;',source)
    source=source[:declared.start()]+source[declared.end():]
    clock=unique(r'long\s+num2\s*;\s*long\s+num\s*=\s*JSystem\.CurrentTimeMillis\(\)\s*;',source)
    source=source[:clock.start()]+'AutoSaveLogData log;'+nl+'long num = JSystem.CurrentTimeMillis();'+nl+'long num2;'+source[clock.end():]
    loop=unique(r'\btry\s*\{\s*do\s*\{',source);loop_open=source.rfind('{',loop.start(),loop.end());loop_end=closing(source,loop_open)
    if not re.match(r'\s*while\s*\(asRedo_\)\s*;',source[loop_end+1:]):raise Refused('do-loop-tail-differs')
    matches=list(re.finditer(r'if\s*\(asCancel_\)\s*\{\s*break\s*;\s*\}',source))
    if len(matches)!=3:raise Refused('cancel-guard-count')
    for guard in matches:
        if not loop_open<guard.start()<loop_end:raise Refused('cancel-guard-not-in-do')
        if re.search(r'\b(?:for|foreach|while|switch)\s*\(',source[loop_open+1:guard.start()]):raise Refused('guard-owner-not-proven-outer-do')
    for guard in reversed(matches):source=source[:guard.start()]+'if (asCancel_) { return; }'+source[guard.end():]
    return source

def main():
    from pathlib import Path
    import json,hashlib
    w=Path(__file__).resolve().parent;r=next(p for p in w.parents if(p/'tools/compiler_probes.py').exists());b=r/'build/round10/final-corpus/KairoLibrary';sha=lambda raw:hashlib.sha256(raw).hexdigest();patches=[]
    entries=[('r11-c1-sound-protected-returns','kairo.unity.ui.SoundOGG/<>c__DisplayClass4_0::<Play>b__0()->System.Void','src/KairoLibrary/kairo/unity/ui/SoundOGG.cs',r'public\s+override\s+void\s+Play\(int volume, bool mute\)',transform_sound_protected_returns),('r11-c2-shader-array-alias-selector','kairo.unity.graphics.ShaderBuilder::Init(kairo.unity.ui.Graphics,kairo.unity.ui.Image,UnityEngine.Color[],System.Single,System.Boolean,System.Int32,System.Boolean,System.Boolean,System.Boolean)->System.Void','src/KairoLibrary/kairo/unity/graphics/ShaderBuilder.cs',r'public\s+void\s+Init\(kairo\.unity\.ui\.Graphics g, Image img, Color\[\] colors,[^\r\n]*\)',transform_shader_array_alias_and_ordered_selector),('r11-c3-save-protected-cancel-log-scope','kairo.unity.ui.SaveRecorder::AutoSaveRun()->System.Void','src/KairoLibrary/kairo/unity/ui/SaveRecorder.cs',r'private\s+void\s+AutoSaveRun\(\)',transform_save_cancel_returns_and_log_scope)]
    for rule,key,logical,pattern,fn in entries:
        raw=(b/logical).read_bytes();source=raw.decode('utf-8-sig');match=unique(pattern,source);end=closing(source,source.index('{',match.end()))+1;before=source[match.start():end];after=fn(before)
        patches.append({'method_id':key,'logical_path':logical,'rule':rule,'start':len(source[:match.start()].encode('utf-16-le'))//2,'end':len(source[:end].encode('utf-16-le'))//2,'before':before,'after':after,'source_file_sha256':sha(raw),'source_text_sha256':sha(source.encode()),'calculator_sha256':sha(Path(__file__).read_bytes()),'source_snapshot':'build/round10/final-corpus/KairoLibrary','own_compilation_receipt_sha256':sha((b/'verification.json').read_bytes()),'own_dump_sha256':sha((b/'rebuilt-methods.json').read_bytes()),'original_dump_sha256':sha((b/'original-methods.json').read_bytes()),'source_only':True,'own_body_source_bound':True,'readiness':'NOT_READY','game_outcome':'UNKNOWN','game_compilations':0,'generated_source_policy':'edit parent only; compiler owns every generated type/member','rebase_policy':'fresh exact unique UTF16 before declaration; no current Main binding'})
        (w/(rule+'-before.cs')).write_bytes(before.encode());(w/(rule+'-after.cs')).write_bytes(after.encode())
    (w/'c123-proposals.json').write_text(json.dumps({'patches':patches,'game_compilations':0,'adopted':False},indent=2)+'\n',encoding='utf-8');print([p['method_id']for p in patches])
if __name__=='__main__':main()
