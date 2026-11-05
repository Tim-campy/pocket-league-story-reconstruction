"""Prove generated ownership using matching names, metadata and CIL edges.

Ownership proof is separate from body equality. Every generated method still
needs its own complete body comparison; a group can have mixed body states.
"""
from collections import defaultdict
import re


def type_of(key): return key.split('::',1)[0]


def generated_type(name):
    return any(part.startswith('<') for part in name.split('/'))


def edges(method):
    return {(r['opcode'],r['kind'],r['id'],r['declaring_type']) for r in method.get('direct_references',[])}


def prove_groups(original,rebuilt,generated_ids,body_equal_ids=None):
    left={m['stable_id']:m for m in original['selected_methods']}
    right={m['stable_id']:m for m in rebuilt['selected_methods']}
    old_types={t['name']:t for t in original['types']}
    new_types={t['name']:t for t in rebuilt['types']}
    old_fields={f['id']:f for t in original['types'] for f in t['fields']}
    new_fields={f['id']:f for t in rebuilt['types'] for f in t['fields']}
    groups=defaultdict(set);method_group={}
    for key in sorted(generated_ids):
        owner=type_of(key);name=key.split('::',1)[1].split('(',1)[0]
        if generated_type(owner): group=owner
        elif name.startswith('<'): group=owner+'::'+name.split('>',1)[0]+'>'
        else: continue  # auto-property accessors are proved separately below
        groups[group].add(key);method_group[key]=group
    proofs={};group_reports={}

    # A shared cache type can contain unrelated lambdas, some still renamed.
    # Establish roots over matching creation/delegate edges, then prove each
    # existing member independently. Missing siblings do not erase a valid edge.
    root_sets={group:set() for group in groups}
    shared_incoming=defaultdict(list)
    for source,method in left.items():
        for edge in sorted(edges(method) & edges(right.get(source,{}))):
            if edge[1]=='method' and edge[2] in method_group:
                shared_incoming[edge[2]].append((source,edge))
    for _ in range(len(groups)+1):
        changed=False
        for member,incoming_edges in shared_incoming.items():
            group=method_group[member]
            for source,edge in incoming_edges:
                parent=method_group.get(source)
                roots=root_sets[parent] if parent else {source}
                before=len(root_sets[group]);root_sets[group].update(roots)
                changed|=before!=len(root_sets[group])
        if not changed: break
    for key,group in method_group.items():
        if key not in right:
            proofs[key]={'proved':False,'kind':'generated_member_il_path','group':group,'origins':[],
                'reason':'original generated member name absent in rebuild'}
            continue
        name=key.split('::',1)[1].split('(',1)[0]
        embedded=set(re.findall(r'<([^<>]+)>(?:b__|d__)',key))
        roots=set();witnesses=[]
        # Named lambda methods need an edge to that exact method. Constructors
        # and iterator protocol methods inherit the generated type's roots.
        if re.match(r'<[^<>]+>b__',name):
            for source,edge in shared_incoming[key]:
                parent=method_group.get(source)
                roots.update(root_sets[parent] if parent else {source});witnesses.append([source,list(edge)])
        else:
            roots.update(root_sets[group])
            for member in sorted(groups[group]):
                witnesses.extend([source,list(edge)] for source,edge in shared_incoming[member])
        root_names={r.split('::',1)[1].split('(',1)[0].split('``')[0] for r in roots}
        owner=type_of(key)
        proved=bool(roots and witnesses and embedded.issubset(root_names) and
            (not generated_type(owner) or owner in old_types and owner in new_types))
        proofs[key]={'proved':proved,'kind':'generated_member_il_path','group':group,
            'origins':sorted(roots),'matching_member_name':key in right,
            'embedded_names_agree':embedded.issubset(root_names),'il_witnesses':witnesses}

    if body_equal_ids is not None:
        # These two helpers belong to the shared cache type, rather than one
        # lambda. Its complete noncapturing-lambda set must correspond first.
        for key in method_group:
            owner=type_of(key);name=key.split('::',1)[1].split('(',1)[0]
            if not owner.endswith('/<>c') or name not in ('.ctor','.cctor'):continue
            lambdas={m for m in groups[method_group[key]] if '>b__' in m}
            pending=sorted(lambdas-set(body_equal_ids))
            proofs[key].update({'kind':'shared_cache_type','lambda_members':sorted(lambdas),
                'pending_lambda_bodies':pending,'all_lambda_bodies_equal':bool(lambdas) and not pending})
            proofs[key]['proved']=proofs[key]['proved'] and bool(lambdas) and not pending

    for group,members in groups.items():
        group_reports[group]={'proved':all(proofs[m]['proved'] for m in members),
            'origins':sorted(root_sets[group]),'member_ids':sorted(members),
            'proved_members':sorted(m for m in members if proofs[m]['proved']),
            'unproved_members':sorted(m for m in members if not proofs[m]['proved']),
            'matching_names':all(m in right for m in members),
            'scope':'ownership only; every method body is independently checked'}

    for key in generated_ids-set(method_group):
        method=left[key];owner=type_of(key);properties=old_types.get(owner,{}).get('properties',[])
        prop=next((p for p in properties if key in (p['get'],p['set'])),None)
        if prop is None: continue
        rebuilt_prop=next((p for p in new_types.get(owner,{}).get('properties',[]) if p['name']==prop['name']),None)
        backing=owner+'::<'+prop['name']+'>k__BackingField'
        witness=any(e[1]=='field' and e[2]==backing for e in edges(method) & edges(right.get(key,{})))
        proved=prop==rebuilt_prop and witness and backing in old_fields and old_fields[backing]==new_fields.get(backing)
        proofs[key]={'proved':proved,'kind':'auto_property','property':owner+'::'+prop['name'],
            'backing_field':backing,'origins':[],'matching_metadata':prop==rebuilt_prop,'il_reference_in_both':witness}

    for key,method in left.items():
        if key in generated_ids: continue
        dependencies=[];unproved=[];roots=set()
        for edge in edges(method):
            _,kind,ref,owner=edge
            group=method_group.get(ref) if kind=='method' else None
            if group is None and owner in groups: group=owner
            if owner.startswith('<PrivateImplementationDetails>') and kind=='field':
                valid=edge in edges(right.get(key,{})) and ref in old_fields and old_fields[ref]==new_fields.get(ref)
                dependencies.append(ref)
                if not valid: unproved.append(ref)
            elif group:
                dependencies.append(group)
                if kind=='method' and ref in method_group:
                    valid=proofs.get(ref,{}).get('proved',False)
                else:
                    valid=bool(root_sets[group]) and edge in edges(right.get(key,{}))
                if not valid: unproved.append(group)
            elif ref in generated_ids:
                valid=proofs.get(ref,{}).get('proved',False)
                dependencies.append(ref)
                if not valid: unproved.append(ref)
        if dependencies:
            proofs[key]={'proved':not unproved,'kind':'origin_dependencies','groups':sorted(set(dependencies)),
                'unproved':sorted(set(unproved)),'origins':[key]}
    return proofs,group_reports
