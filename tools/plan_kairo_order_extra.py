"""Recover omitted explicit constructors for two constrained order candidates.

Writes isolated C# only; does not alter src/, compile DLLs or promote methods.
An empty explicit instance constructor lets C# compile existing field
initializers normally. No original method-body bytes are copied.
"""
import copy
import json
from pathlib import Path
from plan_kairo_member_order import merge_members,load_constraints

ROOT=Path(__file__).resolve().parents[1]
TARGETS=('kairo.unity.ui.IApplication','kairo.unity.system.KairoBase')


def main():
    source=json.loads((ROOT/'analysis/kairo-source-members.json').read_text())
    oracle=json.loads((ROOT/'build/KairoLibrary-roslyn-1.3.2/original-methods.json').read_text(encoding='utf-8-sig'))
    original_types={t['name']:t for t in oracle['types']}
    original_methods={m['stable_id']:m for m in oracle['selected_methods']}
    constraints=load_constraints(source)
    folder=ROOT/'analysis/variants/kairo-order-extra';mapping={};reports=[]
    for file in source['sources']:
        for type in file['types']:
            if type['name'] not in TARGETS:continue
            name=type['name'];members=copy.deepcopy(type['members'])
            constructor=name+'::.ctor()->System.Void';definition=original_methods[constructor]
            if any(constructor in m['method_ids'] for m in members):
                raise ValueError('Source already contains this constructor')
            access={1:'private',2:'private protected',3:'internal',4:'protected',5:'protected internal',6:'public'}[definition['method_attributes']&7]
            if access=='private protected':raise ValueError('C# 6 cannot express the constructor access')
            constructor_text='\n\t'+access+' '+name.rsplit('.',1)[1]+'()\n\t{\n\t}\n'
            pseudo={'kind':'ConstructorDeclaration','weight':1,'name':name.rsplit('.',1)[1],
                'method_ids':[constructor],'full_start':None,'full_end':None,'synthetic_source':constructor_text}
            members.insert(0,pseudo)
            rows=[r for r in constraints if r['id'].startswith(name+'::')]
            sequence,visits=merge_members(members,original_types[name],rows)
            text=file['text'];encoded=text.encode('utf-16-le');source_path=Path(file['path'])
            if source_path.read_bytes().decode('utf-8-sig')!=text:raise ValueError('Source changed since span capture')
            chunks=[];slots={};slot=0
            for index in sequence:
                member=members[index]
                chunks.append(member['synthetic_source'].encode('utf-16-le') if 'synthetic_source' in member else
                    encoded[member['full_start']*2:member['full_end']*2])
                for key in member['method_ids']:slots[key]=slot
                slot+=member['weight']
            original_members=type['members'];start=original_members[0]['full_start']*2;end=original_members[-1]['full_end']*2
            rebuilt=encoded[:start]+b''.join(chunks)+encoded[end:]
            relative=source_path.relative_to(ROOT);destination=folder/relative
            destination.parent.mkdir(parents=True,exist_ok=True)
            destination.write_bytes(rebuilt.decode('utf-16-le').encode('utf-8'))
            mapping[relative.as_posix()]=str(destination)
            reports.append({'type':name,'status':'ISOLATED_SOURCE_CANDIDATE','original_ctor_method_index':
                original_types[name]['methods'].index(constructor),'original_ctor_attributes':definition['method_attributes'],
                'constructor_restored_in_source':constructor_text.strip(),'search_visits':visits,
                'constraints':[{'id':r['id'],'expected':r['original_slot_candidates'],
                    'candidate_slot':slots[r['id']],'previous_slot':r['current_slot']} for r in rows],
                'ordered_members':[m['name'] for m in (members[i] for i in sequence)],
                'source_path':relative.as_posix(),'candidate_path':destination.relative_to(ROOT).as_posix(),
                'full_compilation_verified':False})
    (ROOT/'analysis/kairo-order-extra-source-map.json').write_text(json.dumps(mapping,indent=2)+'\n',encoding='utf-8',newline='\n')
    (ROOT/'reports/KairoLibrary-order-unresolved.json').write_text(json.dumps({'types':reports,
        'finding':'Original constructors occupy the first MethodDef while recovered source omits declarations; restoring one-slot explicit constructors satisfies all constraints',
        'applied_to_main':False,'assembly_oracle_modified':False},indent=2)+'\n',encoding='utf-8',newline='\n')
    print(json.dumps({'source_files':len(mapping),'types':[{'type':r['type'],'constraints':r['constraints']} for r in reports]}))


if __name__=='__main__':main()
