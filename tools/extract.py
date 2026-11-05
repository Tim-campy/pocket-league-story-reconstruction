"""Generate the Mono method/type inventory and reference IL from retained REA evidence.

Reads original/ only. Never resets an existing method ledger or writes game C#.
Supplementary snapshots are made by rea_metadata.mjs using installed REA code.
"""
from collections import Counter
import csv
import hashlib
import json
from pathlib import Path
import re
import struct
from urllib.parse import quote

ROOT = Path(__file__).resolve().parents[1]
METHOD_COLUMNS = 'id,assembly,tipo,metodo,token,tamanho_il,estado,percentual,fonte,gerado_por,notas'.split(',')
PRIMITIVES = {'void':'System.Void','bool':'System.Boolean','char':'System.Char','i1':'System.SByte',
 'u1':'System.Byte','i2':'System.Int16','u2':'System.UInt16','i4':'System.Int32','u4':'System.UInt32',
 'i8':'System.Int64','u8':'System.UInt64','r4':'System.Single','r8':'System.Double',
 'string':'System.String','object':'System.Object','native int':'System.IntPtr','native uint':'System.UIntPtr',
 'typedbyref':'System.TypedReference'}


def read_json(path):
    return json.loads(path.read_text(encoding='utf-8'))


def write_json(path, value):
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(value, ensure_ascii=True, indent=2)+'\n', encoding='utf-8')


def write_csv(path, rows, columns):
    with path.open('w', newline='', encoding='utf-8') as file:
        writer=csv.DictWriter(file, fieldnames=columns, lineterminator='\n')
        writer.writeheader(); writer.writerows(rows)


def reference_body(data, method):
    b=method['body']
    if b['status']=='absent': return None
    if b['status']!='present': raise ValueError('Incomplete original body')
    start=b['file_offset']; first=data[start]
    if first&3==2: header,size,flags=1,first>>2,0
    elif first&3==3:
        word=struct.unpack_from('<H',data,start)[0]
        header,size,flags=(word>>12)*4,struct.unpack_from('<I',data,start+4)[0],word&0xfff
    else: raise ValueError('Unknown method header')
    code=start+header; end=code+size
    if size!=b['il_size'] or hashlib.sha256(data[code:end]).hexdigest()!=b['il_sha256']:
        raise ValueError('Raw CIL mismatch')
    if flags&8:
        end=(end+3)&~3
        while True:
            kind=data[end]
            length=int.from_bytes(data[end+1:end+4],'little') if kind&0x40 else data[end+1]
            if length<4 or end+length>len(data): raise ValueError('Invalid exception section')
            end+=length
            if not kind&0x80: break
            end=(end+3)&~3
    return {'file_offset':start,'size':end-start,'sha256':hashlib.sha256(data[start:end]).hexdigest(),
        'code_file_offset':code,'il_size':size,'il_sha256':b['il_sha256'],
        'local_var_sig_token':b['local_var_sig_token'],'exception_regions':b['exception_regions']}


def main():
    config=ROOT/'config'; config.mkdir(exist_ok=True)
    if (config/'methods.csv').exists():
        raise SystemExit('methods.csv already exists; extraction refuses to reset method progress')
    project=read_json(config/'project.json')
    layout=read_json(ROOT/project['reference_archive_layout'])
    bundle=read_json(ROOT/'analysis/rea-evidence-2026-11-05.json')
    entries=layout['entries']; inventory=[]; assemblies=[]
    for entry in entries:
        path=entry['path']; name=Path(path).name
        managed=path.startswith('assets/bin/Data/Managed/') and path.endswith('.dll')
        if managed and (name.startswith('Assembly-CSharp') or name=='KairoLibrary.dll'):
            category, reason='alvo','DLL própria incluída no escopo do AGENTS.md'
            assemblies.append(name[:-4])
        elif managed:
            if not (name.startswith(('UnityEngine','System')) or name in ('mscorlib.dll','Mono.Security.dll')):
                raise ValueError('Assembly não classificado; perguntar antes de ampliar o escopo: '+path)
            category, reason='externo','Biblioteca padrão ou Unity; versão oficial/hash de distribuição pendentes'
        elif path.startswith('lib/') or re.fullmatch(r'classes\d*\.dex',path):
            category, reason='externo','Fora do escopo C#; composição/possível código próprio UNKNOWN; inclusão exige consulta'
        else:
            category, reason='dados','Recurso, metadado de pacote ou assinatura; excluído da métrica de código'
        inventory.append({'caminho':path,'sha256':entry['uncompressed_sha256'],'categoria':category,
            'tamanho':entry['uncompressed_size'],'ordem_zip':entry['ordinal'],'compressao':entry['compression_method'],
            'alinhamento_4':entry['alignment_mod_4'],'apk_sha256':project['apk_sha256'],'notas':reason})
    records={r['normalized_result']['module']['name'][:-4]:r for r in bundle['records']
             if r['operation']=='inspect_managed_members'}
    references={}; methods=[]; types=[]; hints={}; compile_orders={}; generated_count=0
    for assembly in assemblies:
        record=records[assembly]; n=record['normalized_result']
        extra=read_json(ROOT/f'analysis/metadata/{assembly}.json')
        assembly_data=(ROOT/project['extracted']/f'assets/bin/Data/Managed/{assembly}.dll').read_bytes()
        if extra['artifact_sha256']!=n['artifact']['sha256']:
            raise ValueError('Metadata snapshot identity mismatch')
        defs={int(t['token'],16)&0xffffff:t for t in n['types']}
        parents={r['nested']:r['parent'] for r in extra['nested_classes']}
        refs={r['row']:r for r in extra['type_refs']}
        specs={r['row']:r['decoded'] for r in extra['type_specs']}
        fullname_cache={}; ref_cache={}
        def full_name(row, visited=()):
            if row in fullname_cache: return fullname_cache[row]
            if row in visited: raise ValueError('Nested type cycle')
            t=defs[row]
            result=(full_name(parents[row],visited+(row,))+'+'+t['name']) if row in parents else t['full_name']
            fullname_cache[row]=result
            return result
        def reference_name(row, visited=()):
            if row in ref_cache: return ref_cache[row]
            if row in visited: raise ValueError('TypeRef cycle')
            r=refs[row]; scope=r['scope']
            result=(reference_name(scope>>2,visited+(row,))+'+'+r['name']) if scope&3==3 else '.'.join(filter(None,(r['namespace'],r['name'])))
            ref_cache[row]=result
            return result
        def encoded_name(value, visited=()):
            tag,row=value&3,value>>2
            if tag==0: return full_name(row)
            if tag==1: return reference_name(row)
            if tag==2:
                if row in visited: raise ValueError('TypeSpec cycle')
                return resolve(specs[row],visited+(row,))
            raise ValueError('Invalid TypeDefOrRef')
        def resolve(text, visited=()):
            if text is None: return 'UNKNOWN'
            text=re.sub(r'\b(?:class|valuetype):(\d+)',lambda m:encoded_name(int(m[1]),visited),text)
            for raw in sorted(PRIMITIVES,key=len,reverse=True):
                text=re.sub(r'(?<![\w.])'+re.escape(raw)+r'(?![\w.])',PRIMITIVES[raw],text)
            return text
        generated_tokens={a['parent_token'] for a in extra['attributes']
                          if a['type_name']=='System.Runtime.CompilerServices.CompilerGeneratedAttribute'}
        def root_type(row):
            while row in parents: row=parents[row]
            return row
        def is_generated(row):
            t=defs[row]
            return t['token'] in generated_tokens or t['name'].startswith('<') and t['name']!='<Module>' or (row in parents and is_generated(parents[row]))
        def source_path(row):
            top=root_type(row); t=defs[top]
            if t['name']=='<Module>' or is_generated(top): return ''
            return '/'.join(['src',assembly,*filter(None,t['namespace'].split('.')),t['name']+'.cs'])
        tokens={t['token']:full_name(row) for row,t in defs.items()}
        tokens.update({f'0x{0x01000000|row:08x}':reference_name(row) for row in refs})
        for method in n['methods']:
            row=int(method['declaring_type_token'],16)&0xffffff
            sig=method['signature']
            if sig['parse_status']!='decoded': raise ValueError('Unresolved original method signature')
            arity=sig['generic_parameter_count'] or 0
            generic=f'``{arity}' if arity else ''
            identifier=f"{assembly}::{full_name(row)}::{method['name']}{generic}({','.join(resolve(p) for p in sig['parameter_types'])})->{resolve(sig['return_type'])}"
            if identifier in references: raise ValueError('Duplicate full method signature: '+identifier)
            original={'artifact_sha256':n['artifact']['sha256'],'mvid':n['module']['mvid'],
                'token':method['token'],'signature':sig,'flags':method['flags'],'impl_flags':method['impl_flags'],
                'evidence_id':record['evidence_id'],'body':reference_body(assembly_data,method)}
            references[identifier]={**original,'assembly':assembly,'type':full_name(row),
                'declaring_type_token':method['declaring_type_token'],'signature_id':identifier,
                'max_stack':method['body']['max_stack'],'init_locals':method['body']['init_locals'],
                'il_size':method['body']['il_size'],'artifact_path':f'assets/bin/Data/Managed/{assembly}.dll'}
            generated=is_generated(row) or method['token'] in generated_tokens
            generated_count+=int(generated)
            methods.append(dict(zip(METHOD_COLUMNS,[identifier,assembly,full_name(row),method['name'],method['token'],
                method['body']['il_size'],'todo','UNKNOWN',source_path(row),'UNKNOWN' if generated else '',
                'membro gerado: origem ainda UNKNOWN' if generated else ('sem corpo CIL' if method['body']['status']=='absent' else '')])))
            tokens[method['token']]=identifier
        for field in n['fields']:
            row=int(field['declaring_type_token'],16)&0xffffff
            tokens[field['token']]=full_name(row)+'::'+field['name']
        for member in n['member_refs']:
            tokens[member['token']]=str(member['parent_name'])+'::'+member['name']
        for literal in (n['user_strings'] or {}).get('literals',[]):
            tokens[literal['token']]=json.dumps(literal['value'],ensure_ascii=True)
        order=[]
        for row,t in defs.items():
            types.append({'id':assembly+'::'+full_name(row),'assembly':assembly,'token':t['token'],'ordem':row,
                'namespace':defs[root_type(row)]['namespace'],'tipo':full_name(row),
                'tipo_pai':full_name(parents[row]) if row in parents else '', 'fonte':source_path(row),
                'gerado_pelo_compilador':str(is_generated(row)).lower(),'metodos':t['method_list']['count'],
                'metadados':'UNKNOWN'})
            source=source_path(row)
            if row not in parents and source and source not in order: order.append(source)
        compile_orders[assembly]=order
        hints[assembly]={pattern:[full_name(row) for row,t in defs.items() if pattern in t['name']]
            for pattern in ('AnonStorey','am$cache','DisplayClass','CachedAnonymousMethodDelegate','<>c','d__')}
        instruction_map={m['token']:m for m in extra['methods']}
        method_lookup={m['token']:m for m in methods if m['assembly']==assembly}
        data=(ROOT/project['extracted']/f'assets/bin/Data/Managed/{assembly}.dll').read_bytes()
        for original_method in n['methods']:
            m=method_lookup[original_method['token']]; r=references[m['id']]; body=r['body']
            type_row=int(original_method['declaring_type_token'],16)&0xffffff
            type_folder=f"{type_row:06x}_"+quote(full_name(type_row),safe='._')[:70]
            relative=f"ref/{assembly}/{type_folder}/{m['token'][2:]}_{quote(m['metodo'],safe='._')[:30]}.il"
            reference_file=ROOT/relative; reference_file.parent.mkdir(parents=True,exist_ok=True)
            lines=['// REFERENCE ONLY. Never a compiler/build input.', '// '+record['evidence_id'],
                   '// '+m['id'], '// SHA256 assembly: '+n['artifact']['sha256'],
                   '// maxstack: '+str(r['max_stack'])+'; initlocals: '+str(r['init_locals'])]
            if body:
                start=body['file_offset']; code=body['code_file_offset']
                lines.append('// header bytes: '+data[start:code].hex())
                local=body['local_var_sig_token']
                if local:
                    local_row=int(local,16)&0xffffff
                    lines.append('// locals signature '+local+': '+extra['standalone_signatures'][local_row-1]['raw_hex'])
                for instruction in instruction_map[m['token']]['instructions']:
                    operand=instruction['operand']
                    text='' if operand is None else str(operand)
                    if text in tokens: text+=' // '+tokens[text]
                    lines.append(f"IL_{instruction['offset']:04x}: {instruction['opcode']} {text}".rstrip())
                lines.append('// exception regions: '+json.dumps(body['exception_regions'],ensure_ascii=True))
            else: lines.append('// No CIL body in the original declaration.')
            reference_file.write_text('\n'.join(lines)+'\n',encoding='utf-8')
            r['reference_il']=relative
        write_json(ROOT/f'ref/{assembly}/tokens.json',tokens)
    if len(methods)!=len(references): raise ValueError('Method inventory mismatch')
    write_csv(config/'methods.csv',methods,METHOD_COLUMNS)
    write_csv(config/'types.csv',types,list(types[0]))
    write_csv(config/'inventory.csv',inventory,list(inventory[0]))
    (config/'compile-order').mkdir(exist_ok=True)
    for assembly,order in compile_orders.items():
        (config/f'compile-order/{assembly}.txt').write_text('\n'.join(order)+'\n',encoding='utf-8')
        for source in order: (ROOT/source).parent.mkdir(parents=True,exist_ok=True)
        (ROOT/f'src/{assembly}/.gitkeep').touch()
    write_json(config/'method-reference.json',references)
    write_json(config/'compiler-hints.json',hints)
    write_json(config/'coverage.json',{'target_assemblies':assemblies,'methods':len(methods),'types':len(types),
        'target_method_inventory_complete':True,'type_metadata_comparison_complete':False,
        'generated_method_origin_mapping':'UNKNOWN','outside_managed_code_scope':'excluded; consult user if own code is found'})
    print(json.dumps({'methods':len(methods),'types':len(types),'inventory':len(inventory),
        'il_bytes':sum(int(m['tamanho_il']) for m in methods),'generated_members_pending_origin':generated_count,
        'categories':dict(Counter(r['categoria'] for r in inventory))}))


if __name__=='__main__': main()
