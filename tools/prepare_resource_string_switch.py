"""Prepare an uncompiled C# candidate from the original string comparison chain.

Only writes an isolated source, textual diff and diagnostic report. Does not
compile, modify src/, edit states or write compiler-generated classes.
"""
import difflib
import hashlib
import json
from pathlib import Path
import re

ROOT=Path(__file__).resolve().parents[1]
LOGICAL='src/KairoLibrary/kairo/unity/ui/ResourceManager.cs'
ORIGIN='kairo.unity.ui.ResourceManager::LoadStart(kairo.unity.util.JarInflater,System.Int32)->System.Void'
GENERATED='kairo.unity.ui.ResourceManager/<>c__DisplayClass51_0::<LoadStart>b__0()->System.Void'
HELPER='<PrivateImplementationDetails>::ComputeStringHash(System.String)->System.UInt32'


def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def transform(text,expected_order):
    matches=list(re.finditer(r'(?m)^(?P<indent>\t+)switch \(text4\)\r?\n(?P=indent)\{',text))
    if len(matches)!=1:
        raise ValueError('Expected one exact switch(text4) source node')
    match=matches[0];indent=match['indent'];newline='\r\n' if '\r\n' in text else '\n'
    end=text.index(newline+indent+'}',match.end())
    body=text[match.end():end]
    labels=list(re.finditer(r'(?m)^'+re.escape(indent)+r'(?:case "(?P<value>[^"]+)"|(?P<default>default)):\r?\n',body))
    order=[label['value'] for label in labels if label['value'] is not None]
    if order!=expected_order or len(labels)!=len(order)+1 or labels[-1]['default'] is None:
        raise ValueError('Source cases do not have the proven original comparison order')
    branches=[];internal_breaks=0
    for index,label in enumerate(labels):
        stop=labels[index+1].start() if index+1<len(labels) else len(body)
        statements=body[label.end():stop].rstrip('\r\n')
        suffix=newline+indent+'\tbreak;'
        if not statements.endswith(suffix):
            raise ValueError('Case does not terminate with an unconditional switch break')
        statements=statements[:-len(suffix)]
        if label['default'] is not None:
            # The only surviving break belongs to !Language.English(). Its
            # original destination is the option loop increment, so after
            # removing the switch its source representation is continue.
            count=len(re.findall(r'\bbreak;',statements))
            if count!=1 or 'if (!Language.English())' not in statements:
                raise ValueError('Unexpected nested break structure in default')
            statements=statements.replace('break;','continue;')
            internal_breaks+=count
            head='else'
        else:
            head=('if' if index==0 else 'else if')+' (text4 == '+json.dumps(label['value'])+')'
        branches.append(indent+head+newline+indent+'{'+newline+statements+newline+indent+'}')
    replacement=newline.join(branches)
    candidate=text[:match.start()]+replacement+text[end+len(newline+indent+'}'):]
    return candidate,{'original_case_order':order,'removed_terminal_switch_breaks':len(labels),
                      'nested_switch_breaks_replaced_by_loop_continue':internal_breaks}


def main():
    base=ROOT/'build/KairoLibrary-roslyn-1.3.2'
    documents={which:json.loads((base/f'{which}-methods.json').read_text(encoding='utf-8-sig'))
               for which in ('original','rebuilt')}
    methods={which:next(m for m in document['selected_methods'] if m['stable_id']==GENERATED)
             for which,document in documents.items()}
    original=methods['original'];rebuilt=methods['rebuilt']
    order=[];comparison_sites=[]
    instructions=original['instructions']
    for index,instruction in enumerate(instructions):
        if not (178<=instruction['offset']<=365) or instruction['opcode']!='call':
            continue
        operand=instruction.get('operand')
        if not isinstance(operand,dict) or '::op_Equality``0(' not in operand.get('method',''):
            continue
        previous=instructions[index-1]
        if previous['opcode']!='ldstr':
            raise ValueError('Expected a literal immediately before String.op_Equality')
        order.append(previous['operand'])
        comparison_sites.append({'literal':previous['operand'],'ldstr_offset':previous['offset'],
                                 'equality_call_offset':instruction['offset'],
                                 'branch':instructions[index+1]})
    if len(order)!=9:
        raise ValueError('Expected nine observed direct string comparisons')
    callers={which:[{'method':m['stable_id'],'site':reference} for m in document['selected_methods']
                    for reference in m.get('direct_references',[]) if reference.get('id')==HELPER]
             for which,document in documents.items()}
    if callers['original'] or len(callers['rebuilt'])!=1 or callers['rebuilt'][0]['method']!=GENERATED:
        raise ValueError('Helper caller inventory changed')
    source=ROOT/LOGICAL;before=source.read_bytes();text=before.decode('utf-8-sig')
    edited,diagnostic=transform(text,order)
    folder=ROOT/'analysis/variants/round2-resource-string-if';folder.mkdir(parents=True,exist_ok=True)
    candidate=folder/'ResourceManager.cs'
    candidate.write_bytes((b'\xef\xbb\xbf' if before.startswith(b'\xef\xbb\xbf') else b'')+edited.encode('utf-8'))
    if source.read_bytes()!=before:
        raise ValueError('Main source changed while preparing candidate')
    mapping=folder/'source-map.json'
    mapping.write_text(json.dumps({LOGICAL:str(candidate)},indent=2)+'\n',encoding='utf-8')
    patch=ROOT/'reports/KairoLibrary-resource-string-if.patch'
    patch.write_text(''.join(difflib.unified_diff(text.splitlines(keepends=True),edited.splitlines(keepends=True),
        fromfile=LOGICAL,tofile=candidate.relative_to(ROOT).as_posix())),encoding='utf-8',newline='')
    locals_match_without_hash=rebuilt['locals'][:19]+rebuilt['locals'][20:]==original['locals']
    report={'origin':ORIGIN,'generated_method':GENERATED,'original_token':'0x06000bac',
        'helper':HELPER,'callers':callers,'source':LOGICAL,'source_before_sha256':sha(source),
        'candidate':candidate.relative_to(ROOT).as_posix(),'candidate_sha256':sha(candidate),
        'source_map':mapping.relative_to(ROOT).as_posix(),'patch':patch.relative_to(ROOT).as_posix(),
        'source_order_unchanged':True,'lambda_syntax_retained':True,
        'original_header':original['header'],'rebuilt_header':rebuilt['header'],
        'original_local_count':len(original['locals']),'rebuilt_local_count':len(rebuilt['locals']),
        'rebuilt_local_19':rebuilt['locals'][19],'locals_equal_after_removing_hash_temp':locals_match_without_hash,
        'original_comparison_sites':comparison_sites,
        'original_exception_regions':original['exception_regions'],
        'rebuilt_exception_regions':rebuilt['exception_regions'],
        'candidate_compiled':False,'candidate_matches_original':'UNKNOWN','regressions':'UNKNOWN',
        'main_source_modified':False,'ledger_updated':False,
        'hypothesis':'Replacing the only hashing string switch by the observed equality chain removes the sole ComputeStringHash caller and its compiler-generated helper; complete compilation and verification are still required.',
        **diagnostic}
    output=ROOT/'reports/KairoLibrary-resource-string-if-plan.json'
    output.write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({key:value for key,value in report.items() if key not in
                     ('original_comparison_sites','original_exception_regions','rebuilt_exception_regions')}))


if __name__=='__main__':
    main()
