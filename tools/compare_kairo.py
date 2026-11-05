"""Three-way, source-provenance-checked comparison of the full Kairo builds.

Only metadata operands are replaced by resolved identities in method-body byte
cells. Header bytes, instruction widths/offsets, constants, padding and EH bytes
remain under comparison. No input/output assembly is edited.
"""
from collections import Counter
import csv
import hashlib
import json
from pathlib import Path

ROOT=Path(__file__).resolve().parents[1]
FACETS=('signature','header','locals','instructions','exception_regions','extra_sections','method_attributes','impl_attributes')


def read_json(relative): return json.loads((ROOT/relative).read_text(encoding='utf-8-sig'))


def sha(path): return hashlib.sha256(path.read_bytes()).hexdigest()


def body_cells(method):
    if 'raw_body_hex' not in method: return None
    cells=list(bytes.fromhex(method['raw_body_hex'].replace('-','')))
    occupied=set()
    for operand in method['metadata_operands']:
        identity=json.dumps(operand['identity'],sort_keys=True,ensure_ascii=True,separators=(',',':'))
        if 'UNKNOWN' in identity: raise ValueError('Unresolved metadata identity')
        for index in range(operand['body_offset'],operand['body_offset']+operand['width']):
            if index<0 or index>=len(cells) or index in occupied: raise ValueError('Invalid metadata-operand byte range')
            occupied.add(index);cells[index]=('metadata',identity)
    return cells


def differences(original,candidate):
    if candidate is None: return ['missing_method'],None
    changed=[facet for facet in FACETS if original.get(facet)!=candidate.get(facet)]
    left,right=body_cells(original),body_cells(candidate)
    if left is None or right is None:
        if left!=right: changed.append('body_presence')
        return changed,None
    if left!=right and 'resolved_body_bytes' not in changed: changed.append('resolved_body_bytes')
    denominator=max(len(left),len(right))
    percent=100.0*sum(x==y for x,y in zip(left,right))/denominator if denominator else 100.0
    return changed,round(percent,4)


def patterns(method,candidate):
    texts=json.dumps([method.get('instructions'),candidate.get('instructions') if candidate else None],ensure_ascii=True)
    result=[]
    if 'ldftn' in texts or 'ldvirtftn' in texts or 'DisplayClass' in texts or 'b__' in texts: result.append('lambda_or_delegate')
    if 'ComputeStringHash' in texts or ('switch' in texts and 'System.String' in texts): result.append('string_switch')
    if '::Concat' in texts or 'StringBuilder' in texts: result.append('string_concatenation')
    if '::GetEnumerator' in texts or '::MoveNext' in texts: result.append('enumeration')
    if '::Dispose' in texts: result.append('using_or_disposal')
    if 'System.Threading.Monitor' in texts: result.append('lock')
    if '<PrivateImplementationDetails>' in texts or '::InitializeArray' in texts: result.append('constant_array_initialization')
    if method.get('exception_regions'): result.append('exception_regions')
    if '/<' in method['stable_id'] or '::<script-generated' in texts: result.append('compiler_generated_identity')
    return result or ['other_source_or_codegen_difference']


def assert_build_provenance(folder):
    manifest=read_json(folder+'/build-manifest.json')
    if manifest['exit_code']!=0: raise ValueError('Build failed')
    if sha(ROOT/folder/'KairoLibrary.dll')!=manifest['output_sha256']: raise ValueError('Build artifact changed')
    for source in manifest['sources']:
        if sha(ROOT/source['path'])!=source['sha256']: raise ValueError('Source changed since build: '+source['path'])
    for reference in manifest['references']:
        if sha(Path(reference['path']))!=reference['sha256']: raise ValueError('Reference changed since build')
    return manifest


def main():
    original=read_json('build/KairoLibrary/original-methods.json')['selected_methods']
    modern=read_json('build/KairoLibrary/rebuilt-methods.json')['selected_methods']
    older=read_json('build/KairoLibrary-roslyn-1.3.2/rebuilt-methods.json')['selected_methods']
    modern_manifest=assert_build_provenance('build/KairoLibrary')
    older_manifest=assert_build_provenance('build/KairoLibrary-roslyn-1.3.2')
    if modern_manifest['sources']!=older_manifest['sources'] or modern_manifest['references']!=older_manifest['references']:
        raise ValueError('Three-way comparison requires identical sources, order and references')
    comparable_flags=lambda m:[f for f in m['flags'] if not f.startswith('/out:')]
    if comparable_flags(modern_manifest)!=comparable_flags(older_manifest): raise ValueError('Compiler options differ')
    indices=[]
    for methods in (original,older,modern):
        index={method['stable_id']:method for method in methods}
        if len(index)!=len(methods): raise ValueError('Ambiguous method identity')
        indices.append(index)
    oi,old,new=indices
    counts=Counter();facet_counts=Counter();pattern_counts=Counter();rows=[]
    with (ROOT/'config/methods.csv').open(encoding='utf-8',newline='') as file:
        ledger=[r for r in csv.DictReader(file) if r['assembly']=='KairoLibrary']
    by_token={r['token']:r for r in ledger}
    for method in original:
        key=method['stable_id'];old_method=old.get(key);new_method=new.get(key)
        old_diff,old_percent=differences(method,old_method)
        new_diff,new_percent=differences(method,new_method)
        inter_diff,_=differences(old_method,new_method) if old_method else (['missing_old_method'],None)
        present='raw_body_hex' in method
        if not present:
            status='declaration_without_body'
            counts[status]+=1
            counts['old_declaration_equal']+=not old_diff;counts['new_declaration_equal']+=not new_diff
        else:
            counts['method_bodies']+=1
            counts['old_body_equal']+=not old_diff;counts['new_body_equal']+=not new_diff
            counts['old_raw_body_equal']+=old_method is not None and method['raw_body_hex']==old_method.get('raw_body_hex')
            counts['new_raw_body_equal']+=new_method is not None and method['raw_body_hex']==new_method.get('raw_body_hex')
            counts['old_missing']+=old_method is None;counts['new_missing']+=new_method is None
            if not old_diff and not new_diff: status='both_match_original'
            elif not old_diff: status='only_1_3_2_matches_original'
            elif not new_diff: status='only_5_8_matches_original'
            elif old_method and new_method and not inter_diff: status='same_outputs_but_original_differs'
            else: status='different_or_missing_and_neither_matches'
            counts[status]+=1
        token='0x'+format(0x06000000|int(method['token'].split('0x')[1].rstrip(']'),16),'08x')
        labels=patterns(method,old_method) if old_diff else []
        if present and old_diff:
            facet_counts.update(old_diff);pattern_counts.update(labels)
        rows.append({'id':by_token[token]['id'],'original_token':token,'stable_id':key,'classification':status,
            'body_present':present,'roslyn_1_3_2_equal':not old_diff,'roslyn_5_8_equal':not new_diff,
            'outputs_same':not inter_diff if old_method and new_method else False,
            'roslyn_1_3_2_percent':old_percent if old_percent is not None else 'UNKNOWN',
            'roslyn_5_8_percent':new_percent if new_percent is not None else 'UNKNOWN',
            'roslyn_1_3_2_differences':';'.join(old_diff),'roslyn_5_8_differences':';'.join(new_diff),
            'patterns':';'.join(labels)})
    counts['extra_method_ids_1_3_2']=len(set(old)-set(oi))
    counts['extra_method_ids_5_8']=len(set(new)-set(oi))
    report={'schema_version':1,'scope':'complete KairoLibrary builds from identical recovered C# sources',
        'counts':dict(counts),'old_build_difference_facets':dict(facet_counts),'old_build_patterns':dict(pattern_counts),
        'original_sha256':sha(ROOT/'original/extracted/assets/bin/Data/Managed/KairoLibrary.dll'),
        'roslyn_1_3_2_sha256':older_manifest['output_sha256'],'roslyn_5_8_sha256':modern_manifest['output_sha256'],
        'compiler_source_provenance':'verified','assembly_byte_identity':False,'type_metadata_identity':'NOT_VERIFIED',
        'normalized_assembly_fields':[],'ledger_states_promoted':False,
        'historical_compiler_identity':'UNKNOWN; older compiler is a better compatible candidate',
        'method_metric':'fixed-offset byte agreement after resolving only metadata operands; all other bytes retained'}
    (ROOT/'reports').mkdir(exist_ok=True)
    with (ROOT/'reports/KairoLibrary-compiler-comparison.csv').open('w',encoding='utf-8',newline='') as file:
        writer=csv.DictWriter(file,fieldnames=list(rows[0]),lineterminator='\n');writer.writeheader();writer.writerows(rows)
    (ROOT/'reports/KairoLibrary-compiler-comparison.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8',newline='\n')
    print(json.dumps(report,indent=2))


if __name__=='__main__': main()
