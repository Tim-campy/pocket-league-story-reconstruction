"""Prove generated ownership using matching names, metadata and CIL edges.

Ownership proof is separate from body equality. Every generated method still
needs its own complete body comparison; a group can have mixed body states.
"""
from collections import defaultdict
import re
import hashlib


def type_of(key): return key.split('::',1)[0]


def generated_type(name):
    return any(part.startswith('<') for part in name.split('/'))


def edges(method):
    return {(r['opcode'],r['kind'],r['id'],r['declaring_type']) for r in method.get('direct_references',[])}


def static_data_constructor_proof(left, right, old_types, new_types, key, body_equal_ids):
    owner = key.split('::', 1)[0]
    result = dict(proved=False, kind='compiler_static_data_default_ctor', group=owner,
                  origins=[], field_ids=[], field_witnesses=[], all_consumer_bodies_equal=False)

    def refuse(reason):
        result['reason'] = reason
        return result

    if owner != '<PrivateImplementationDetails>' or key != owner + '::.ctor()->System.Void':
        return refuse('not the recognized compiler-data default-constructor shape')
    if body_equal_ids is None or key not in body_equal_ids or key not in left or key not in right:
        return refuse('complete constructor body equality not established')
    before, after = old_types.get(owner), new_types.get(owner)
    if not before or before != after or before.get('attributes') != 0:
        return refuse('complete holder metadata does not match the witnessed compiler shape')
    if (before.get('methods') != [key] or any(before.get(k) for k in ('interfaces', 'generic_parameters', 'properties', 'events'))
            or not before.get('base_type', '').startswith('class System.Object @')
            or not any('System.Runtime.CompilerServices.CompilerGeneratedAttribute @' in a.get('type', '') and a.get('blob') == '01-00-00-00'
                       for a in before.get('custom_attributes', []))):
        return refuse('holder is not a pure compiler-generated data container')
    for document in (left, right):
        method = document[key]
        if (method.get('method_attributes') != 6150 or method.get('impl_attributes') != 0
                or method.get('header', {}).get('code_size') != 7
                or method.get('locals') or method.get('exception_regions') or method.get('extra_sections')
                or [i['opcode'] for i in method.get('instructions', [])] != ['ldarg.0', 'call', 'ret']
                or {(r['opcode'], r['kind'], r['id'], r['declaring_type']) for r in method.get('direct_references', [])}
                != {('call', 'method', 'System.Object::.ctor()->System.Void', 'System.Object')}):
            return refuse('constructor is not the witnessed implicit Object constructor')
        if any(r.get('kind') == 'method' and r.get('id') == key
               for m in document.values() for r in m.get('direct_references', [])):
            return refuse('direct constructor references require the regular IL-path proof')
    if any(left[key].get(k) != right[key].get(k) for k in ('signature', 'method_attributes', 'impl_attributes')):
        return refuse('constructor declarations differ')
    fields = before.get('fields', [])
    if not fields or len({f['id'] for f in fields}) != len(fields):
        return refuse('data fields missing or ambiguous')
    payloads = {}
    for field in fields:
        if (not field['id'].startswith(owner + '::') or field.get('attributes', 0) & 0x110 != 0x110
                or not field.get('initial_value')):
            return refuse('holder includes a field without static RVA data')
        match = re.fullmatch(r'valuetype (' + re.escape(owner) + r'/[^ @]+) @ .+', field['type'])
        if not match:
            return refuse('unknown backing-layout type')
        name = match[1]
        layout = old_types.get(name)
        try:
            payload = bytes.fromhex(field['initial_value'].replace('-', ''))
        except ValueError:
            return refuse('invalid retained RVA payload')
        if (not payload or not layout or layout != new_types.get(name)
                or layout.get('packing_size') != 1 or layout.get('class_size') != len(payload)
                or layout.get('attributes', 0) & 0x18 != 0x10
                or layout.get('fields') or layout.get('methods')
                or not layout.get('base_type', '').startswith('class System.ValueType @')):
            return refuse('backing-layout metadata or physical data size differs')
        payloads[field['id']] = dict(bytes=len(payload), sha256=hashlib.sha256(payload).hexdigest(), layout=name)
    if set(before.get('nested_types', [])) != {p['layout'] for p in payloads.values()}:
        return refuse('holder has an unexplained nested type')
    initializer = 'System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(System.Array,System.RuntimeFieldHandle)->System.Void'

    def consumers(document):
        found = {}
        for identity, method in document.items():
            refs = method.get('direct_references', [])
            instructions = {i['offset']: i['opcode'] for i in method.get('instructions', [])}
            values = set()
            for ref in refs:
                if ref.get('kind') != 'field' or ref.get('declaring_type') != owner:
                    continue
                offset = ref['offset']
                if (ref['opcode'] != 'ldtoken' or ref['id'] not in payloads
                        or instructions.get(offset - 1) != 'dup'
                        or instructions.get(offset) != 'ldtoken' or instructions.get(offset + 5) != 'call'
                        or not any(r['offset'] == offset + 5 and r['opcode'] == 'call'
                                   and r['kind'] == 'method' and r['id'] == initializer for r in refs)):
                    raise ValueError('unrecognized data consumption')
                values.add((offset, ref['id']))
            if values:
                found[identity] = values
        return found

    try:
        old_consumers, new_consumers = consumers(left), consumers(right)
    except ValueError:
        return refuse('a field is not consumed by a paired ldtoken/InitializeArray sequence')
    if not old_consumers or old_consumers != new_consumers:
        return refuse('consumer inventory, names, offsets or field identities differ')
    if any(any(left[identity].get(k) != right[identity].get(k)
               for k in ('signature', 'method_attributes', 'impl_attributes'))
           or old_types.get(identity.split('::', 1)[0]) != new_types.get(identity.split('::', 1)[0])
           for identity in old_consumers):
        return refuse('consumer declarations or declaring-type metadata differ')
    pending = sorted(set(old_consumers) - set(body_equal_ids))
    covered = {field for uses in old_consumers.values() for offset, field in uses}
    if covered != set(payloads):
        return refuse('one or more data fields lack a paired consumer')
    result.update(origins=sorted(old_consumers), field_ids=sorted(payloads), field_payloads=payloads,
                  payload_bytes=sum(p['bytes'] for p in payloads.values()),
                  pending_consumer_bodies=pending, all_consumer_bodies_equal=not pending,
                  matching_holder_metadata=True, matching_backing_layouts=True, matching_consumer_declarations=True,
                  implicit_constructor_shape=True, direct_constructor_references=0,
                  field_witnesses=[dict(origin=origin, offset=offset, field=field,
                                        sequence=['dup', 'ldtoken', 'call InitializeArray'], matched_in_both=True)
                                   for origin, uses in sorted(old_consumers.items()) for offset, field in sorted(uses)])
    result['proved'] = not pending
    if pending:
        result['reason'] = 'one or more complete consumer bodies are not equal'
    return result


def compiled_static_data_origins(proof, rows_by_stable, compiled_sources):
    result = dict(proved=False, kind='compiled_origins_for_generated_data_holder', source_paths=[])
    if not proof or proof.get('kind') != 'compiler_static_data_default_ctor' or not proof.get('proved'):
        return result
    origins = proof.get('origins', [])
    missing = [identity for identity in origins if rows_by_stable.get(identity, {}).get('fonte') not in compiled_sources]
    result.update(origins=origins, unavailable_origins=missing,
                  source_paths=sorted({rows_by_stable[i]['fonte'] for i in origins if i not in missing}),
                  proved=bool(origins) and not missing)
    return result


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

    # A Reflection.Emit data container has no incoming constructor edge.
    # Its complete static data and paired source consumers provide the roots.
    for key in method_group:
        if type_of(key) != '<PrivateImplementationDetails>' or proofs[key]['proved']:
            continue
        data_proof = static_data_constructor_proof(left, right, old_types, new_types, key, body_equal_ids)
        proofs[key] = data_proof
        if data_proof['proved']:
            root_sets[method_group[key]].update(data_proof['origins'])

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
