"""Plan metadata-order merges for both assemblies without changing project inputs.

All original TypeDefs are accounted for, including generated, module, enum and
delegate types. FieldDef and MethodDef provide separate partial orders; neither
table determines the historical interleaving between fields and methods. Roslyn
ordinal constraints are retained. Only an isolated source-map is written.
Compilation and full verification are deliberately separate acceptance steps.
"""
from collections import Counter, defaultdict
import argparse
import copy
import csv
import hashlib
import json
from pathlib import Path
import re
import subprocess
from compiler_probes import ROSLYN, UNITY
from plan_kairo_member_order import load_constraints, merge_members

ROOT = Path(__file__).resolve().parents[1]
LABELS = {'KairoLibrary':'roslyn-1.3.2', 'Assembly-CSharp':'mono-smcs'}


def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def member_predecessors(members, original):
    """Return independent metadata orders over source declaration nodes."""
    methods = {key:i for i,m in enumerate(members) for key in m['method_ids']}
    fields = {name:i for i,m in enumerate(members) for name in m.get('field_names', [])}
    properties = {m['name']:i for i,m in enumerate(members) if m['kind'] in ('PropertyDeclaration','IndexerDeclaration')}
    events = {m['name']:i for i,m in enumerate(members) if m['kind'] in ('EventDeclaration','EventFieldDeclaration')}
    nested = {m['name']:i for i,m in enumerate(members) if m['kind'] in
              ('ClassDeclaration','StructDeclaration','InterfaceDeclaration','DelegateDeclaration','EnumDeclaration')}
    for field in original['fields']:
        match = re.fullmatch(r'<(.+)>k__BackingField', field['name'])
        if match and match[1] in properties:
            fields[field['name']] = properties[match[1]]
    sequences = {
        'methods':[(key,methods[key]) for key in original['methods'] if key in methods],
        'fields':[(f['name'],fields[f['name']]) for f in original['fields'] if f['name'] in fields],
        'properties':[(p['name'],properties[p['name']]) for p in original['properties'] if p['name'] in properties],
        'events':[(e['name'],events[e['name']]) for e in original['events'] if e['name'] in events],
        'nested_types':[(n,nested[n.split('/')[-1]]) for n in original['nested_types'] if n.split('/')[-1] in nested]}
    predecessors = [set() for _ in members]
    internal_conflicts = []
    for category,sequence in sequences.items():
        nodes = [node for _,node in sequence]
        unique = []
        for node in nodes:
            if not unique or node != unique[-1]:
                unique.append(node)
        if len(set(unique)) != len(unique):
            internal_conflicts.append(category+': declarations interleave in original table')
        for left,right in zip(unique,unique[1:]):
            if left != right:
                predecessors[right].add(left)
        # A single declaration emitting several fields/accessors cannot be fixed
        # by moving the declaration if its own within-declaration order is wrong.
        if category in ('fields','methods'):
            expected_by_node = defaultdict(list)
            for key,node in sequence:
                expected_by_node[node].append(key)
            for node,expected in expected_by_node.items():
                actual = members[node].get('field_names' if category=='fields' else 'method_ids', [])
                actual = [key for key in actual if key in expected]
                if actual and actual != expected:
                    internal_conflicts.append(category+': within-declaration order differs: '+str(node))
    return predecessors, sequences, internal_conflicts


def stable_order(predecessors):
    todo = set(range(len(predecessors)))
    result = []
    while todo:
        available = sorted(i for i in todo if not (predecessors[i] & todo))
        if not available:
            raise ValueError('Metadata partial orders contain a cycle at source-declaration granularity')
        node = available[0]
        result.append(node)
        todo.remove(node)
    return result


def namespace_compile_order(original_top, top_files, reverse_children):
    """Invert namespace traversal measured on our own csc 1.3.2 fixtures.

    The csc emitter visits types of the current namespace first and traverses
    sibling namespaces in reverse declaration order at each level. The source
    order inside one namespace remains forward. Mono is kept in original file
    order until its traversal has been independently measured.
    """
    tree = {'types':[], 'children':{}}
    for name in original_top:
        namespace = name.rsplit('.',1)[0] if '.' in name else ''
        node = tree
        for component in namespace.split('.') if namespace else []:
            node = node['children'].setdefault(component,{'types':[], 'children':{}})
        node['types'].append(name)

    def walk(node, reverse):
        result = list(node['types'])
        children = list(node['children'].values())
        if reverse:
            children.reverse()
        for child in children:
            result.extend(walk(child,reverse))
        return result

    representable = walk(tree,False)==original_top
    desired_names = walk(tree,reverse_children) if reverse_children else original_top
    desired_files = list(dict.fromkeys(top_files[name] for name in desired_names))
    return desired_files,representable


def choose_order(members, original, constraints):
    predecessors, sequences, conflicts = member_predecessors(members, original)
    if conflicts:
        raise ValueError('; '.join(conflicts))
    sequence = stable_order(predecessors)  # Detect cycles before constrained search.
    if constraints:
        normalized = copy.deepcopy(members)
        for m in normalized:
            if m['kind']=='EventFieldDeclaration':
                m['kind']='FieldDeclaration'
        sequence,_ = merge_members(normalized, original, constraints)
        positions = {node:i for i,node in enumerate(sequence)}
        if any(positions[p] >= positions[node] for node,previous in enumerate(predecessors) for p in previous):
            raise ValueError('Constrained merge violates an independently checked metadata order')
    return sequence, {name:len(rows) for name,rows in sequences.items()}


def render_edits(encoded, edits):
    """Render nested permutations without invalidating original UTF-16 spans."""
    def render(start, end, excluded=None):
        contained = [edit for edit in edits if edit is not excluded and start<=edit['start'] and edit['end']<=end]
        outer = [edit for edit in contained if not any(other is not edit and other['start']<=edit['start']
            and edit['end']<=other['end'] for other in contained)]
        result = []
        cursor = start
        for edit in sorted(outer,key=lambda e:e['start']):
            result.append(encoded[cursor:edit['start']])
            result.extend(render(a,b,edit) for a,b in edit['chunks'])
            cursor = edit['end']
        result.append(encoded[cursor:end])
        return b''.join(result)
    return render(0,len(encoded))


def inventory_sources(assembly, selected, folder):
    vs = next(p for p in ROSLYN.parents if (p/'dotnet').is_dir())
    dotnet = vs/'dotnet/net10.0/runtime/dotnet.exe'
    driver = ROOT/'build/order-syntax/OrderSyntax.dll'
    if not driver.is_file():
        raise FileNotFoundError('Run python tools/prepare_order_syntax.py first')
    managed = ROOT/'original/extracted/assets/bin/Data/Managed'
    if assembly=='KairoLibrary':
        profile = UNITY/'Editor/Data/Mono/lib/mono/2.0'
        references = [profile/'mscorlib.dll', profile/'System.Core.dll', managed/'UnityEngine.dll', profile/'System.dll']
    else:
        # This helper binds declarations only, not the reconstruction build.
        # Roslyn cannot perform Mono's cross-profile core binding (APK core
        # 2.0.5 versus KairoLibrary's classic 2.0). Use classic reference
        # assemblies here so inherited declarations resolve; production smcs
        # continues to use the exact APK profile documented in its manifest.
        profile = UNITY/'Editor/Data/Mono/lib/mono/2.0'
        references = [profile/'mscorlib.dll',managed/'KairoLibrary.dll',managed/'UnityEngine.dll',
                      profile/'System.Core.dll',profile/'System.dll']
    manifest = folder/'parse-input.json'
    manifest.write_text(json.dumps({'assembly':assembly,'sources':[str(p) for p in selected.values()],
        'references':[str(p) for p in references]},indent=2)+'\n',encoding='utf-8')
    output = folder/'source-members.json'
    with output.open('wb') as file:
        result = subprocess.run([str(dotnet),str(driver),str(manifest)],stdout=file,stderr=subprocess.PIPE)
    if result.returncode:
        raise RuntimeError(result.stderr.decode('utf-8',errors='replace'))
    document = json.loads(output.read_text(encoding='utf-8-sig'))
    if document['errors']:
        raise ValueError('Semantic inventory errors: '+json.dumps(document['errors'][:20]))
    return document


def plan(assembly, label, input_map):
    folder = ROOT/'analysis/variants'/label/assembly
    folder.mkdir(parents=True,exist_ok=True)
    order = [p for p in (ROOT/'config/compile-order'/f'{assembly}.txt').read_text(encoding='utf-8').splitlines()
             if p and not p.startswith('#')]
    info = f'src/{assembly}/AssemblyInfo.cs'
    if info not in order:
        order.append(info)
    if set(input_map)-set(order):
        raise ValueError('Unknown input logical source')
    selected = {logical:(ROOT/input_map.get(logical,logical)).resolve() for logical in order}
    if any(not p.is_relative_to(ROOT) or p.is_relative_to(ROOT/'original') for p in selected.values()):
        raise ValueError('Source inputs must stay in workspace and outside original/')
    before = {logical:sha(path) for logical,path in selected.items()}
    source = inventory_sources(assembly, selected, folder)
    logical_of = {str(p):logical for logical,p in selected.items()}
    original = json.loads((ROOT/'build'/f'{assembly}-{LABELS[assembly]}'/'original-methods.json').read_text(encoding='utf-8-sig'))
    original_types = {t['name']:t for t in original['types']}
    with (ROOT/'config/types.csv').open(encoding='utf-8',newline='') as file:
        type_rows = {r['tipo'].replace('+','/'):r for r in csv.DictReader(file) if r['assembly']==assembly}
    by_type = defaultdict(list)
    if assembly=='KairoLibrary':
        for row in load_constraints():
            by_type[row['id'].split('::',1)[0]].append(row)
    reports = {}
    mapping = dict(input_map)
    top_files = {}
    for file in source['sources']:
        logical = logical_of[file['path']]
        if selected[logical].read_bytes().decode('utf-8-sig')!=file['text']:
            raise ValueError('Source changed during planning: '+logical)
        edits = []
        encoded = file['text'].encode('utf-16-le')
        for record in file['types']:
            name = record['name']
            if name not in original_types:
                reports[name] = {'type':name,'status':'EXTRA_EXPLICIT_TYPE','source':logical}
                continue
            if record['parent'] is None:
                top_files[name] = logical
            original_type = original_types[name]
            item = {'type':name,'source':logical,'kind':record['kind'],'constraints':len(by_type[name])}
            members = copy.deepcopy(record['members'])
            accessor_edits = []
            accessor_changes = []
            for member in members:
                accessors = member.get('accessors', [])
                if len(accessors)<2:
                    continue
                positions = {key:i for i,key in enumerate(original_type['methods'])}
                if any(a['id'] not in positions for a in accessors):
                    continue
                ordered = sorted(accessors,key=lambda a:positions[a['id']])
                if accessors!=ordered:
                    accessor_edits.append({'start':accessors[0]['full_start']*2,'end':accessors[-1]['full_end']*2,
                        'chunks':[(a['full_start']*2,a['full_end']*2) for a in ordered]})
                    member['method_ids'] = [a['id'] for a in ordered]
                    accessor_changes.append({'member':member['name'],'method_order':member['method_ids']})
            if record['kind']=='DelegateDeclaration':
                item.update(status='DECLARATION_HAS_NO_REORDERABLE_MEMBERS',permutation=[])
            elif record['kind']=='EnumDeclaration':
                actual = [m['name'] for m in members]
                expected = [f['name'] for f in original_type['fields'] if f['name']!='value__']
                item.update(status='ENUM_FIELD_ORDER_ALREADY_EQUAL' if actual==expected else 'UNRESOLVED_ENUM_FIELD_ORDER',
                            actual_fields=actual,expected_fields=expected)
            else:
                try:
                    sequence,coverage = choose_order(members,original_type,by_type[name])
                    changed = sequence!=list(range(len(members))) or bool(accessor_edits)
                    item.update(status='ISOLATED_ORDER_CANDIDATE' if changed else 'SOURCE_PARTIAL_ORDERS_ALREADY_SATISFIED',
                                permutation=sequence,represented_rows=coverage,accessor_changes=accessor_changes)
                    edits.extend(accessor_edits)
                    if sequence!=list(range(len(members))):
                        edits.append({'start':members[0]['full_start']*2,'end':members[-1]['full_end']*2,
                            'chunks':[(members[i]['full_start']*2,members[i]['full_end']*2) for i in sequence]})
                except (ValueError,KeyError,TimeoutError) as error:
                    item.update(status='UNRESOLVED',reason=str(error))
            reports[name] = item
        if edits:
            candidate = render_edits(encoded,edits).decode('utf-16-le').encode('utf-8')
            destination = folder/'sources'/logical
            destination.parent.mkdir(parents=True,exist_ok=True)
            destination.write_bytes(candidate)
            mapping[logical] = str(destination)
    for name,t in original_types.items():
        if name in reports:
            continue
        row = type_rows.get(name,{})
        status = 'MODULE_NO_SOURCE_DECLARATION' if name=='<Module>' else 'GENERATED_TYPE_NOT_DIRECTLY_EDITABLE' if row.get('gerado_pelo_compilador')=='true' else 'UNRESOLVED_EXPLICIT_TYPE_NOT_IN_SOURCE'
        reports[name] = {'type':name,'status':status,'source':row.get('fonte')}
    for logical,path in selected.items():
        if sha(path)!=before[logical]:
            raise ValueError('Source changed during planning: '+logical)
    original_top = [t['name'] for t in original['types'] if t['name'] in top_files]
    expected_top = list(dict.fromkeys(top_files[name] for name in original_top))
    current_top = [p for p in order if p!=info]
    desired_top,namespace_representable = namespace_compile_order(original_top,top_files,assembly=='KairoLibrary')
    candidate_order = desired_top+[p for p in current_top if p not in desired_top]
    (folder/'compile-order.txt').write_text('\n'.join(candidate_order)+'\n',encoding='utf-8')
    map_path = folder/'source-map.json'
    map_path.write_text(json.dumps(mapping,indent=2)+'\n',encoding='utf-8')
    final_folder = folder/'final-syntax'
    final_folder.mkdir(exist_ok=True)
    final_selected = {logical:Path(mapping.get(logical,str(path))) for logical,path in selected.items()}
    final_source = inventory_sources(assembly,final_selected,final_folder)
    syntax_failures = []
    final_constraints = 0
    for file in final_source['sources']:
        for record in file['types']:
            item = reports.get(record['name'])
            if item is None or item['status'] not in ('ISOLATED_ORDER_CANDIDATE','SOURCE_PARTIAL_ORDERS_ALREADY_SATISFIED'):
                continue
            try:
                actual,_ = choose_order(record['members'],original_types[record['name']],by_type[record['name']])
                if actual!=list(range(len(record['members']))):
                    raise ValueError('Reparsed declaration sequence still requires a permutation')
                slots = {key:m['roslyn_slot'] for m in record['members'] for key in m['method_ids']}
                for constraint in by_type[record['name']]:
                    final_constraints += 1
                    if slots.get(constraint['id']) not in constraint['original_slot_candidates']:
                        raise ValueError('Reparsed generated ordinal differs: '+constraint['id'])
            except (ValueError,KeyError,TimeoutError) as error:
                syntax_failures.append({'type':record['name'],'reason':str(error)})
    if syntax_failures:
        raise ValueError('Final candidate syntax order was not reproduced: '+json.dumps(syntax_failures))
    report = {'assembly':assembly,'original_types':len(original_types),'source_types':sum(len(f['types']) for f in source['sources']),
        'states':dict(Counter(r['status'] for r in reports.values())), 'types':[reports[name] for name in original_types],
        'extra_explicit_types':[r for name,r in reports.items() if name not in original_types],
        'original_type_order':[t['name'] for t in original['types']],
        'source_compile_order_already_in_original_type_sequence':current_top==expected_top,
        'top_level_compile_order_already_equal':current_top==candidate_order,'candidate_compile_order':str(folder/'compile-order.txt'),
        'namespace_traversal_rule':'reverse sibling namespace declaration order recursively; own types forward' if assembly=='KairoLibrary' else 'source order; no namespace traversal transformation',
        'original_namespace_group_sequence_representable':namespace_representable,
        'input_source_hashes':before,'changed_source_files':len(set(mapping)-set(input_map))+sum(mapping[k]!=v for k,v in input_map.items()),
        'source_map':str(map_path),'applied_to_main':False,'full_compilation_verified':False,'ledger_updated':False,
        'final_source_partial_orders_verified':True,'final_roslyn_constraints_checked':final_constraints,
        'syntax_binding_profile':'classic Unity 2.0; declaration inventory only; production build references unchanged',
        'limitations':['Independent metadata tables do not reveal historical field/method interleaving.',
            'Implicit constructors, generated classes and generated fields are left to the compiler.',
            'This plan proves only represented source-node partial orders; emitted order must be checked after compilation.']}
    (folder/'ordering-report.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    return {key:value for key,value in report.items() if key not in ('types','input_source_hashes','original_type_order')}


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--assembly',choices=list(LABELS),required=True)
    parser.add_argument('--label',default='all-member-order')
    parser.add_argument('--source-map',type=Path)
    args = parser.parse_args()
    if not re.fullmatch(r'[A-Za-z0-9][A-Za-z0-9_.-]*',args.label):
        raise ValueError('Invalid label')
    mapping = json.loads(args.source_map.read_text(encoding='utf-8-sig')) if args.source_map else {}
    print(json.dumps(plan(args.assembly,args.label,mapping)))


if __name__=='__main__':
    main()
