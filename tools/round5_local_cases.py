"""Read-only diagnostics for slot-index pairs and DrawSeb; no state promotion."""
import argparse
from collections import Counter
import difflib
import hashlib
import json
import os
from pathlib import Path
import re
import shutil
import subprocess

from compare_kairo import differences, sha
from round4_transform import TransformClient

ROOT = Path(__file__).resolve().parents[1]
PAIR = '{"opcode":"stloc.3","operand":null} => {"opcode":"stloc.1","operand":null}'
ROOT_HINTS = {'form.DebugForm::Draw_goal(': 'num5', 'form.DebugForm::Draw_goalMini(': 'num5',
              'form.GameForm::_draw(': 'num13', 'form.SoccerForm::Draw_bonusIncome(': 'num5',
              'form.SoccerForm::Draw_matchElse(': 'num8', 'form.SoccerForm::Draw_newPlayerPopular(': 'num4',
              'form.SoccerForm::Draw_newPlayerReserach(': 'num4', 'form.SubForm::_draw(': 'num24',
              'form.SubForm::_draw4(': 'num4', 'form.SubForm::_draw6(': 'num4'}


def read(path):
    return json.loads(Path(path).read_text(encoding='utf-8-sig'))


def local_inspector():
    unity=Path(os.environ.get('UNITY_ROOT',str(Path.home()/'AppData/Local/Programs/Unity/5.3.4f1')))
    managed=unity/'Editor/Data/Managed'
    csc=Path('C:/Windows/Microsoft.NET/Framework64/v4.0.30319/csc.exe')
    sources=[ROOT/'tools/Round5LocalInspector.cs',ROOT/'tools/ProbeInspector.cs']
    libraries=[managed/n for n in ['Mono.Cecil.dll','Mono.Cecil.Pdb.dll','Mono.Cecil.Mdb.dll']]
    identity={str(p):sha(p) for p in [csc]+sources+libraries}
    key=hashlib.sha256(json.dumps(identity,sort_keys=True).encode()).hexdigest()[:16]
    folder=ROOT/'build/round5/local-inspector'/key;folder.mkdir(parents=True,exist_ok=True)
    target=folder/'Round5LocalInspector.exe'
    if not target.exists():
        cmd=[str(csc),'/nologo','/target:exe','/main:Round5LocalInspector','/out:'+str(target),'/r:System.Web.Extensions.dll']
        cmd+=['/r:'+str(p) for p in libraries]+[str(p) for p in sources]
        subprocess.run(cmd,cwd=ROOT,check=True)
        for p in libraries:shutil.copyfile(p,folder/p.name)
        (folder/'manifest.json').write_text(json.dumps({'inputs':identity,'output_sha256':sha(target)},indent=2)+'\n',encoding='utf-8',newline='\n')
    return target


def declaration(client,logical,text,key):
    name=key.split('::',1)[1].split('(',1)[0]
    pattern=r'(?m)^[ \t]*(?:public|private|protected|internal)[^\r\n;=]*\b'+re.escape(name)+r'\s*\('
    for match in re.finditer(pattern,text):
        line=text.count('\n',0,match.start())+1
        info=client.request(ROOT/logical,source=text,line=line,column=1,max_candidates=0)
        if info.get('mapped',{}).get('method_id')!=key:continue
        start,end=info['declaration_start'],info['declaration_end']
        actual=text.encode('utf-16-le')[start*2:end*2].decode('utf-16-le')
        return {'start_utf16':start,'end_utf16':end,'line':line,'text':actual,'prefix_60_lines':'\n'.join(actual.splitlines()[:60])}
    raise ValueError('Cannot uniquely locate declaration: '+key)


def local_operation(instruction):
    for family in ('ldloca','ldloc','stloc'):
        op=instruction['opcode'];operand=instruction['operand']
        if op.startswith(family+'.') and op[len(family)+1:].isdigit():return family,int(op[len(family)+1:])
        if op in (family,family+'.s') and isinstance(operand,dict):return family,operand.get('local')
    return None


def access_counts(method):
    result={i:{'loads':0,'stores':0,'address_loads':0} for i in range(len(method['locals']))}
    for instruction in method['instructions']:
        item=local_operation(instruction)
        if item:
            family,index=item;result[index][{'ldloc':'loads','stloc':'stores','ldloca':'address_loads'}[family]]+=1
    return result


def initial_zero_chain(method):
    items=method['instructions']
    for n,instruction in enumerate(items[:-1]):
        if instruction['opcode']!='ldc.i4.0' or items[n+1]['opcode']!='dup':continue
        stores=[]
        for child in items[n+1:]:
            item=local_operation(child)
            if item and item[0]=='stloc':stores.append(item[1])
            elif child['opcode']!='dup':break
        return {'offset':instruction['offset'],'slots':stores}
    return None


def alignment_hints(old,new,names):
    """Source diagnostic only. Acceptance must not use this normalization."""
    def key(i):
        local=local_operation(i)
        if local:return local[0],None
        operand=i['operand'];op=i['opcode']
        if isinstance(operand,dict) and ('branch' in operand or 'switch' in operand):return op.removesuffix('.s'),'<target-ignored-for-diagnostic>'
        return op,json.dumps(operand,sort_keys=True)
    left,right=old['instructions'],new['instructions'];counts=Counter()
    for block in difflib.SequenceMatcher(a=[key(i) for i in left],b=[key(i) for i in right],autojunk=False).get_matching_blocks():
        if block.size<3:continue
        for k in range(block.size):
            a,b=local_operation(left[block.a+k]),local_operation(right[block.b+k])
            if a and b:counts[(a[1],b[1])]+=1
    byslot={v['slot']:v['name'] for v in names} if isinstance(names,list) else {}
    return [{'original_slot':a,'rebuilt_slot':b,'rebuilt_symbol_name':byslot.get(b,'UNKNOWN'),'matching_instruction_support':count}
            for (a,b),count in counts.most_common()]


def source_depths(text,name):
    """Brace depth outside strings/comments; only supporting source evidence."""
    pattern=re.compile(r'\bint\s+'+re.escape(name)+r'\s*(?==|;)')
    positions={m.start() for m in pattern.finditer(text)};result=[];depth=0;state='code';i=0
    while i<len(text):
        c=text[i];n=text[i+1:i+2]
        if state=='line':
            if c in '\r\n':state='code'
        elif state=='block':
            if c=='*' and n=='/':state='code';i+=1
        elif state in ('string','char'):
            if c=='\\':i+=1
            elif c==('"' if state=='string' else "'"):state='code'
        elif state=='verbatim':
            if c=='"':
                if n=='"':i+=1
                else:state='code'
        else:
            if i in positions:result.append({'relative_line':text.count('\n',0,i)+1,'brace_depth':depth})
            if c=='/' and n=='/':state='line';i+=1
            elif c=='/' and n=='*':state='block';i+=1
            elif c=='@' and n=='"':state='verbatim';i+=1
            elif c=='"':state='string'
            elif c=="'":state='char'
            elif c=='{':depth+=1
            elif c=='}':depth-=1
        i+=1
    return result


def classify(key,old,new,located,names):
    counts=access_counts(old);chain_old,chain_new=initial_zero_chain(old),initial_zero_chain(new)
    hints=alignment_hints(old,new,names)
    if 'LanguageForm::DrawButton(' in key:
        return {'classification':['omitted_unused_declaration','declaration_order'],
                'evidence_status':'CONFIRMED_IL_DISCREPANCY; SOURCE_ORIGIN_HYPOTHESIS',
                'evidence':'Original slot 1 is Int32 and has zero loads, stores and address loads; original x/y are slots 2/3, while MDB names current x/y as 2/1. An unused declaration before x and y can reproduce the missing slot; only compilation can confirm the full body.',
                'candidate':'After instance/PushOrigin: int reservedCoordinate; int x = 20; int y = 20;',
                'compiler_temporary_as_first_cause':'UNKNOWN; no foreach/switch/using/lock is present in this source method',
                'original_slot_accesses':counts,'diagnostic_alignment_only':hints}
    if 'AppData::DrawSeb(' in key:
        return {'classification':['compiler_temporary','loop_form'],
                'evidence_status':'CONFIRMED_CURRENT_FOREACH_TEMPORARIES; ORIGINAL_SOURCE_FORM_UNKNOWN',
                'evidence':'The current foreach creates an additional array-copy slot and index slot after its named element local. Original IL uses the array local directly, initializes an index with zero, then loads/stores the element. This is not merely the order of two independent statements.',
                'candidate':'Replace the array foreach with for (int spriteIndex = 0; spriteIndex < sprites.Length; spriteIndex++) { int[] array = sprites[spriteIndex]; existing body; }.',
                'compiler_temporary_as_first_cause':'SUPPORTED: current unnamed array/index slots and their stores are absent in original at the first mismatch',
                'original_slot_accesses':counts,'diagnostic_alignment_only':hints}
    if 'AppData::CheckTermScript(' in key:
        return {'classification':['omitted_parameter_alias'],
                'evidence_status':'CONFIRMED_IL_DISCREPANCY; REPAIR_REQUIRES_COMPILATION',
                'evidence':'Original begins ldarg.1; stloc.1; ldloc.1 before indexing. Current source indexes terms directly, and its first Int32 array alias slot is absent.',
                'candidate':'Introduce int[] scriptTerms = terms before int num2 = scriptTerms[1], and use the alias where original loads slot 1.',
                'original_slot_accesses':counts,'diagnostic_alignment_only':hints}
    root_name=next(value for fragment,value in ROOT_HINTS.items() if fragment in key)
    scopes=source_depths(located['text'],root_name)
    categories=['omitted_assignment','declaration_order']
    if any(s['brace_depth']>1 for s in scopes):categories.append('block_scope')
    if len(scopes)>1:categories.append('split_reused_local_across_blocks')
    return {'classification':categories,'evidence_status':'CONFIRMED_MISSING_ZERO_STORE; ROOT_NAME_MAPPING_IS_A_DIAGNOSTIC_HYPOTHESIS',
            'evidence':'Initial zero chain stores original slots '+str(chain_old['slots'])+' but rebuilt slots '+str(chain_new['slots'])+'. Current symbols/source place the likely original root coordinate '+root_name+' later'+(' in nested scopes' if 'block_scope' in categories else ' in the same method block')+'. The first discrepancy is an omitted zero assignment plus declaration/scope layout, not a proved compiler temporary.',
            'suggested_root_name':root_name,'source_declarations_for_suggested_root':scopes,
            'candidate':'Declare '+root_name+', num, num2, num3 in that order before the initial zero chain; initialize '+root_name+' = num = num2 = num3 = 0; retain later assignments while removing later declaration(s) of the same logical root variable.',
            'compiler_temporary_as_first_cause':'NO_SUPPORT; the mismatch precedes the current unnamed temporary slots. Later lowering differences can coexist.',
            'original_zero_chain':chain_old,'rebuilt_zero_chain':chain_new,
            'original_slot_accesses':counts,'diagnostic_alignment_only':hints}


def main():
    ap=argparse.ArgumentParser(description=__doc__)
    ap.add_argument('--snapshot',type=Path,required=True)
    ap.add_argument('--symbols-binary',type=Path)
    args=ap.parse_args();snapshot=args.snapshot.resolve();a='Assembly-CSharp'
    folder=snapshot/a;measurement=read(ROOT/'reports/round5-baseline-measurement.json')['assemblies'][a]
    targets=[m for m in measurement['methods'] if m['first_pair_exact']==PAIR]
    targets += [m for m in measurement['methods'] if 'main.AppData::DrawSeb(' in m['stable_id'] or 'main.AppData::CheckTermScript(' in m['stable_id']]
    original={m['stable_id']:m for m in read(folder/'original-methods.json')['selected_methods']}
    rebuilt={m['stable_id']:m for m in read(folder/'rebuilt-methods.json')['selected_methods']}
    manifest=read(folder/'build-manifest.json')
    manifest['sources']=[dict(s,input_path=str(folder/s['path'])) for s in manifest['sources']]
    work=ROOT/'build/round5/local-cases';work.mkdir(parents=True,exist_ok=True)
    mp=work/'syntax-manifest.json';mp.write_text(json.dumps(manifest)+'\n',encoding='utf-8',newline='\n')
    named={};symbol_info=None
    if args.symbols_binary:
        p=subprocess.run([str(local_inspector()),str(args.symbols_binary.resolve())]+[m['stable_id'] for m in targets],cwd=ROOT,capture_output=True,check=True)
        symbol_info=json.loads(p.stdout.decode('utf-8-sig'))
        for m in symbol_info['methods']:
            if not differences(rebuilt[m['stable_id']],m)[0]:named[m['stable_id']]=m
        (work/'symbol-methods.json').write_bytes(p.stdout)
    rows=[]
    with TransformClient(mp) as client:
        for item in targets:
            key=item['stable_id'];logical=item['source'];text=(folder/logical).read_bytes().decode('utf-8-sig')
            old,new=original[key],rebuilt[key]
            located=declaration(client,logical,text,key)
            debug=named.get(key)
            rows.append({'id':item['id'],'stable_id':key,'source':logical,'source_sha256':sha(folder/logical),
                         'source_declaration':located,'first_il_difference_offset':item['first_il_difference_offset'],
                         'original_il_bytes':old['header']['code_size'],'rebuilt_il_bytes':new['header']['code_size'],
                         'original_locals':old['locals'],'rebuilt_locals':new['locals'],
                         'original_first_70_instructions':old['instructions'][:70],
                         'rebuilt_first_70_instructions':new['instructions'][:70],
                         'verified_debug_locals':debug.get('locals_debug') if debug else 'UNKNOWN',
                         'verified_debug_scope':debug.get('scope_debug') if debug else 'UNKNOWN',
                         'symbol_body_matches_frozen_baseline':debug is not None,
                         'diagnosis':classify(key,old,new,located,debug.get('locals_debug') if debug else None)})
    result={'operation':'read-only-source-IL-local-diagnostic','snapshot':str(snapshot),'baseline_measurement_sha256':sha(ROOT/'reports/round5-baseline-measurement.json'),
            'symbol_kind':symbol_info['symbol_kind'] if symbol_info else 'NONE','methods':rows,
            'method_states_modified':False,'source_modified':False}
    out=ROOT/'reports/round5-local-cases.json';out.write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8',newline='\n')
    print(json.dumps({'cases':len(rows),'named_locals_proved':len(named),'output':str(out.relative_to(ROOT))}))


if __name__=='__main__':main()
