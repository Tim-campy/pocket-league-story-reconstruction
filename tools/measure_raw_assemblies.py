"""Measure raw PE/CLI differences only; no normalization, repair or state edits.

Within each named region, compare corresponding byte positions in the common
prefix. Report each unpaired tail separately; never zero-pad or resolve tokens.
Section, stream and table layers overlap and must never be added together.
"""
import argparse
import hashlib
import json
import os
from pathlib import Path
import re
import subprocess

ROOT=Path(__file__).resolve().parents[1]
ASSEMBLIES=('KairoLibrary','Assembly-CSharp')

def sha(data):
    return hashlib.sha256(data).hexdigest()

def read_json(path):
    return json.loads(Path(path).read_text(encoding='utf-8-sig'))

def checked_region(data,descriptor):
    if descriptor is None:
        return b''
    start,size=descriptor['offset'],descriptor['size']
    if not isinstance(start,int) or not isinstance(size,int) or start<0 or size<0 or start>len(data)-size:
        raise ValueError('Region leaves file: '+str(descriptor))
    return data[start:start+size]

def byte_difference(left,right):
    common=min(len(left),len(right))
    positions=[i for i,(a,b) in enumerate(zip(left,right)) if a!=b]
    unpaired_left=len(left)-common
    unpaired_right=len(right)-common
    return {'original_bytes':len(left),'rebuilt_bytes':len(right),
        'common_bytes_compared':common,'equal_common_bytes':common-len(positions),
        'different_common_bytes':len(positions),
        'original_unpaired_bytes':unpaired_left,'rebuilt_unpaired_bytes':unpaired_right,
        'different_or_unpaired_bytes':len(positions)+unpaired_left+unpaired_right,
        'first_different_relative_offsets':positions[:16],
        'raw_bytes_equal':left==right}

def unique_regions(regions):
    result={r['name']:r for r in regions}
    if len(result)!=len(regions):
        raise ValueError('Ambiguous duplicate region names')
    return result

def complement_regions(container,regions):
    """Checked disjoint partition: every gap/padding byte remains measured."""
    start,size=container['offset'],container['size']
    end=start+size
    if start<0 or size<0:
        raise ValueError('Invalid container extent')
    for region in regions:
        if region['offset']<start or region['size']<0 or region['offset']>end-region['size']:
            raise ValueError('Region leaves container: '+region['name'])
    previous='START'
    cursor=start
    result=[]
    for r in sorted((r for r in regions if r['size']),key=lambda r:(r['offset'],r['name'])):
        if r['offset']<cursor or r['size']<0 or r['offset']+r['size']>end:
            raise ValueError('Regions overlap or leave container: '+r['name'])
        if r['offset']>cursor:
            result.append({'name':'gap:'+previous+':'+r['name'],'offset':cursor,'size':r['offset']-cursor})
        cursor=r['offset']+r['size'];previous=r['name']
    if cursor<end:
        result.append({'name':'tail-after:'+previous,'offset':cursor,'size':end-cursor})
    if sum(r['size'] for r in regions)+sum(r['size'] for r in result)!=size:
        raise ValueError('Partition does not account for every container byte')
    return result

def compare_regions(left_data,right_data,left_regions,right_regions):
    left=unique_regions(left_regions);right=unique_regions(right_regions)
    result=[]
    for name in list(left)+[name for name in right if name not in left]:
        a,b=left.get(name),right.get(name)
        delta=byte_difference(checked_region(left_data,a),checked_region(right_data,b))
        result.append({'name':name,'original_region':a,'rebuilt_region':b,
            'offset_delta':b['offset']-a['offset'] if a and b else None,
            'region_presence_equal':(a is None)==(b is None),**delta})
    return result

def inspect_layout(binary,rea_root=None):
    rea_root=Path(rea_root or Path(os.environ['APPDATA'])/'npm/node_modules/rea-agents')
    process=subprocess.run(['node',str(ROOT/'tools/rea_raw_layout.mjs'),str(rea_root),str(binary)],
        check=True,stdout=subprocess.PIPE,stderr=subprocess.PIPE)
    result=json.loads(process.stdout.decode('utf-8-sig'))
    data=Path(binary).read_bytes()
    if result['artifact']['sha256']!=sha(data) or result['artifact']['size']!=len(data):
        raise ValueError('Artifact changed during coordinate inspection')
    return result

def row_measurement(left_data,right_data,left,right):
    la=left or {'row_count':0,'row_size':0,'offset':0}
    rb=right or {'row_count':0,'row_size':0,'offset':0}
    common=min(la['row_count'],rb['row_count'])
    changed=[]
    row_common_differences=0
    row_unpaired_left=0
    row_unpaired_right=0
    for i in range(common):
        a=checked_region(left_data,{'offset':la['offset']+i*la['row_size'],'size':la['row_size']})
        b=checked_region(right_data,{'offset':rb['offset']+i*rb['row_size'],'size':rb['row_size']})
        difference=byte_difference(a,b)
        if not difference['raw_bytes_equal']:
            changed.append(i+1)
        row_common_differences+=difference['different_common_bytes']
        row_unpaired_left+=difference['original_unpaired_bytes']
        row_unpaired_right+=difference['rebuilt_unpaired_bytes']
    return {'comparison':'same table, same one-based row ordinal; raw indexes retained',
        'original_rows':la['row_count'],'rebuilt_rows':rb['row_count'],'common_rows':common,
        'equal_common_rows':common-len(changed),'different_common_rows':len(changed),
        'original_unpaired_rows':la['row_count']-common,'rebuilt_unpaired_rows':rb['row_count']-common,
        'original_row_size':la['row_size'],'rebuilt_row_size':rb['row_size'],
        'different_bytes_within_common_rows':row_common_differences,
        'original_unpaired_bytes_within_common_rows':row_unpaired_left,
        'rebuilt_unpaired_bytes_within_common_rows':row_unpaired_right,
        'first_different_row_ordinals':changed[:16]}

def measure_pair(original_path,rebuilt_path,rea_root=None):
    original_path=Path(original_path).resolve();rebuilt_path=Path(rebuilt_path).resolve()
    a=inspect_layout(original_path,rea_root);b=inspect_layout(rebuilt_path,rea_root)
    left=original_path.read_bytes();right=rebuilt_path.read_bytes()
    if sha(left)!=a['artifact']['sha256'] or sha(right)!=b['artifact']['sha256']:
        raise ValueError('Artifact changed after layout inspection')
    section_a=[a['headers']]+a['sections'];section_b=[b['headers']]+b['sections']
    file_a={'offset':0,'size':len(left)};file_b={'offset':0,'size':len(right)}
    outside_a=complement_regions(file_a,section_a);outside_b=complement_regions(file_b,section_b)
    meta_a=a['metadata'];meta_b=b['metadata']
    meta_other_a=complement_regions(meta_a,a['streams']);meta_other_b=complement_regions(meta_b,b['streams'])
    table_stream_a=next(x for x in a['streams'] if x['name'] in ('#~','#-'))
    table_stream_b=next(x for x in b['streams'] if x['name'] in ('#~','#-'))
    table_other_a=complement_regions(table_stream_a,a['tables']);table_other_b=complement_regions(table_stream_b,b['tables'])
    tables=compare_regions(left,right,a['tables'],b['tables'])
    tables_a=unique_regions(a['tables']);tables_b=unique_regions(b['tables'])
    for table in tables:
        table['rows']=row_measurement(left,right,tables_a.get(table['name']),tables_b.get(table['name']))
    helpers=('tools/measure_raw_assemblies.py','tools/rea_raw_layout.mjs')
    return {'schema_version':1,'operation':'raw-byte-measurement-only',
        'metric':{'coordinate_alignment':'relative byte position within a region matched by exact name',
            'common_prefix':'count unequal corresponding bytes up to min(original length, rebuilt length)',
            'unpaired_tails':'count each side separately; no zero padding',
            'table_rows':'same one-based row ordinal, retaining raw token/heap indexes',
            'normalization_applied':False,'normalized_fields':[],
            'warning':'PE sections, metadata root, streams and tables overlap; do not sum across these layers.'},
        'original':a['artifact'],'rebuilt':b['artifact'],
        'raw_whole_file':byte_difference(left,right),
        'pe_headers':compare_regions(left,right,[a['headers']],[b['headers']])[0],
        'pe_sections':compare_regions(left,right,a['sections'],b['sections']),
        'outside_headers_and_sections':compare_regions(left,right,outside_a,outside_b),
        'metadata_root':compare_regions(left,right,[dict(meta_a,name='METADATA_ROOT')],[dict(meta_b,name='METADATA_ROOT')])[0],
        'metadata_streams':compare_regions(left,right,a['streams'],b['streams']),
        'metadata_non_stream_bytes':compare_regions(left,right,meta_other_a,meta_other_b),
        'metadata_tables':tables,
        'table_stream_non_table_bytes':compare_regions(left,right,table_other_a,table_other_b),
        'partition_accounting':{'original_file_bytes':len(left),'rebuilt_file_bytes':len(right),
            'original_headers_sections_outside_total':sum(x['size'] for x in section_a+outside_a),
            'rebuilt_headers_sections_outside_total':sum(x['size'] for x in section_b+outside_b),
            'original_metadata_streams_plus_other':sum(x['size'] for x in a['streams']+meta_other_a),
            'rebuilt_metadata_streams_plus_other':sum(x['size'] for x in b['streams']+meta_other_b),
            'original_table_bytes_plus_other':sum(x['size'] for x in a['tables']+table_other_a),
            'rebuilt_table_bytes_plus_other':sum(x['size'] for x in b['tables']+table_other_b)},
        'layouts':{'original':a,'rebuilt':b},
        'producer_source_hashes':{name:sha((ROOT/name).read_bytes()) for name in helpers},
        'interpretation':'Raw differences include layout/index reordering and nondeterministic fields; not a count of semantic errors.',
        'method_states_modified':False,'type_states_modified':False,
        'raw_column_semantics':'NOT_EVALUATED; raw table bytes and row ordinals only',
        'coverage':'All admitted PE raw sections, metadata streams/tables, and complementary gaps measured.'}

def freeze_rebuilt(source,destination):
    data=Path(source).read_bytes()
    if destination.exists():
        if destination.read_bytes()!=data:
            raise ValueError('Immutable measurement snapshot already exists with different bytes; choose a new --label')
    else:
        destination.parent.mkdir(parents=True,exist_ok=True)
        destination.write_bytes(data)
    if sha(Path(source).read_bytes())!=sha(data):
        raise ValueError('Build changed while freezing measurement input')
    return sha(data)

def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--assembly',choices=ASSEMBLIES)
    parser.add_argument('--label',default='round3-final')
    parser.add_argument('--rebuilt-dir',type=Path,help='Directory of immutable DLLs; default uses the current verification receipt')
    parser.add_argument('--rea-root',type=Path)
    args=parser.parse_args()
    if not re.fullmatch('[A-Za-z0-9][A-Za-z0-9_.-]*',args.label):
        raise ValueError('Unsafe measurement label')
    summary={}
    for assembly in ([args.assembly] if args.assembly else ASSEMBLIES):
        receipt=read_json(ROOT/'build'/('verification-'+assembly+'.json'))
        source=args.rebuilt_dir/(assembly+'.dll') if args.rebuilt_dir else ROOT/receipt['build_artifact']
        if not source.is_absolute():source=ROOT/source
        if not args.rebuilt_dir and sha(source.read_bytes())!=receipt['build_sha256']:
            raise ValueError('Current build differs from its verification receipt')
        snapshot=ROOT/'build/raw-measurements'/args.label/(assembly+'.dll')
        frozen_sha=freeze_rebuilt(source,snapshot)
        original=ROOT/'original/extracted/assets/bin/Data/Managed'/(assembly+'.dll')
        if sha(original.read_bytes())!=receipt['original_sha256']:
            raise ValueError('Original differs from the verified assembly identity')
        result=measure_pair(original,snapshot,args.rea_root)
        result['assembly']=assembly;result['label']=args.label
        result['input_snapshot']={'source':str(source),'snapshot':str(snapshot.relative_to(ROOT)),
            'sha256':frozen_sha,'verified_current_build':not bool(args.rebuilt_dir)}
        snapshot_info=source.with_name(assembly+'-snapshot.json')
        if args.rebuilt_dir and snapshot_info.is_file():
            result['input_snapshot']['prior_snapshot_provenance']=read_json(snapshot_info)
        output=ROOT/'reports'/(args.label+'-'+assembly+'-raw-measurement.json')
        output.write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8')
        summary[assembly]={'report':str(output.relative_to(ROOT)),
            'whole_file':result['raw_whole_file'],
            'sections':{x['name']:x['different_or_unpaired_bytes'] for x in result['pe_sections']},
            'streams':{x['name']:x['different_or_unpaired_bytes'] for x in result['metadata_streams']}}
    print(json.dumps(summary))
if __name__=='__main__':main()
