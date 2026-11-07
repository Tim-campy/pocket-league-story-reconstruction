"""Read-only PE/CLI measurement with frozen-input/proof provenance.

Compare named sections, streams, tables and row ordinals without changing bytes.
Only fields declared in the supplied frozen toolchain and demonstrated by its
repeatability reports may be excluded analytically. No normalized DLL or hash
is produced. All output paths are new-only; an existing report is never replaced.
"""
import argparse
import csv
import hashlib
import io
import json
import re
import struct
import sys
import uuid
from collections import Counter
from pathlib import Path

sys.dont_write_bytecode = True
ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT / "tools"))
from measure_raw_assemblies import measure_pair, byte_difference, checked_region

def sha(data):
    return hashlib.sha256(data).hexdigest()

def read_json(path):
    return json.loads(Path(path).read_text(encoding="utf-8-sig"))

def serialized(value):
    return json.dumps(value, indent=2, ensure_ascii=True) + "\n"

def json_digest(value):
    return sha(serialized(value).encode("utf-8"))

def measure_detail(raw, accepted_fields):
    L = raw["layouts"]
    D = {key: Path(L[key]["artifact"]["path"]).read_bytes() for key in L}
    for key in L:
        assert sha(D[key]) == L[key]["artifact"]["sha256"]
    U=lambda data,p,n:int.from_bytes(data[p:p+n],'little')
    T={k:{t['name']:t for t in L[k]['tables']}for k in L}
    H={k:{h['name']:h for h in L[k]['streams']}for k in L}
    coded={
     'ResolutionScope':(2,['Module','ModuleRef','AssemblyRef','TypeRef']),
     'TypeDefOrRef':(2,['TypeDef','TypeRef','TypeSpec']),
     'MemberRefParent':(3,['TypeDef','TypeRef','ModuleRef','MethodDef','TypeSpec']),
     'HasConstant':(2,['Field','Param','Property']),
     'HasCustomAttribute':(5,['MethodDef','Field','TypeRef','TypeDef','Param','InterfaceImpl','MemberRef','Module','DeclSecurity','Property','Event','StandAloneSig','ModuleRef','TypeSpec','Assembly','AssemblyRef','File','ExportedType','ManifestResource','GenericParam','GenericParamConstraint','MethodSpec']),
     'CustomAttributeType':(3,['MethodDef','MemberRef']),
     'HasSemantics':(1,['Event','Property']),
     'MethodDefOrRef':(1,['MethodDef','MemberRef']),
     'TypeOrMethodDef':(1,['TypeDef','MethodDef'])}
    schemas={
     'Module':[('Generation',2),('Name','str'),('Mvid','guid'),('EncId','guid'),('EncBaseId','guid')],
     'TypeRef':[('ResolutionScope','c:ResolutionScope'),('TypeName','str'),('TypeNamespace','str')],
     'TypeDef':[('Flags',4),('TypeName','str'),('TypeNamespace','str'),('Extends','c:TypeDefOrRef'),('FieldList','t:Field'),('MethodList','t:MethodDef')],
     'Field':[('Flags',2),('Name','str'),('Signature','blob')],
     'MethodDef':[('RVA',4),('ImplFlags',2),('Flags',2),('Name','str'),('Signature','blob'),('ParamList','t:Param')],
     'Param':[('Flags',2),('Sequence',2),('Name','str')],
     'InterfaceImpl':[('Class','t:TypeDef'),('Interface','c:TypeDefOrRef')],
     'MemberRef':[('Class','c:MemberRefParent'),('Name','str'),('Signature','blob')],
     'Constant':[('Type',1),('Padding',1),('Parent','c:HasConstant'),('Value','blob')],
     'CustomAttribute':[('Parent','c:HasCustomAttribute'),('Type','c:CustomAttributeType'),('Value','blob')],
     'ClassLayout':[('PackingSize',2),('ClassSize',4),('Parent','t:TypeDef')],
     'StandAloneSig':[('Signature','blob')],
     'PropertyMap':[('Parent','t:TypeDef'),('PropertyList','t:Property')],
     'Property':[('Flags',2),('Name','str'),('Type','blob')],
     'MethodSemantics':[('Semantics',2),('Method','t:MethodDef'),('Association','c:HasSemantics')],
     'MethodImpl':[('Class','t:TypeDef'),('MethodBody','c:MethodDefOrRef'),('MethodDeclaration','c:MethodDefOrRef')],
     'TypeSpec':[('Signature','blob')],
     'FieldRVA':[('RVA',4),('Field','t:Field')],
     'Assembly':[('HashAlgId',4),('MajorVersion',2),('MinorVersion',2),('BuildNumber',2),('RevisionNumber',2),('Flags',4),('PublicKey','blob'),('Name','str'),('Culture','str')],
     'AssemblyRef':[('MajorVersion',2),('MinorVersion',2),('BuildNumber',2),('RevisionNumber',2),('Flags',4),('PublicKeyOrToken','blob'),('Name','str'),('Culture','str'),('HashValue','blob')],
     'NestedClass':[('NestedClass','t:TypeDef'),('EnclosingClass','t:TypeDef')],
     'GenericParam':[('Number',2),('Flags',2),('Owner','c:TypeOrMethodDef'),('Name','str')],
     'MethodSpec':[('Method','c:MethodDefOrRef'),('Instantiation','blob')]}
    def width(side,kind):
     if isinstance(kind,int):return kind
     if kind in ('str','blob','guid'):return L[side]['index_widths'][{'str':'strings','blob':'blob','guid':'guid'}[kind]]
     if kind.startswith('t:'):return 2 if T[side].get(kind[2:],{}).get('row_count',0)<65536 else 4
     bits,names=coded[kind[2:]];return 2 if max(T[side].get(n,{}).get('row_count',0)for n in names)<1<<(16-bits) else 4

    coded.update({
     'HasFieldMarshal':(1,['Field','Param']),
     'HasDeclSecurity':(2,['TypeDef','MethodDef','Assembly']),
     'MemberForwarded':(1,['Field','MethodDef']),
     'Implementation':(2,['File','AssemblyRef','ExportedType'])})
    schemas.update({
     'FieldPtr':[('Field','t:Field')], 'MethodPtr':[('Method','t:MethodDef')],
     'ParamPtr':[('Param','t:Param')],
     'EventMap':[('Parent','t:TypeDef'),('EventList','t:Event')],
     'EventPtr':[('Event','t:Event')],
     'Event':[('EventFlags',2),('Name','str'),('EventType','c:TypeDefOrRef')],
     'PropertyPtr':[('Property','t:Property')],
     'FieldMarshal':[('Parent','c:HasFieldMarshal'),('NativeType','blob')],
     'DeclSecurity':[('Action',2),('Parent','c:HasDeclSecurity'),('PermissionSet','blob')],
     'ModuleRef':[('Name','str')],
     'ImplMap':[('MappingFlags',2),('MemberForwarded','c:MemberForwarded'),('ImportName','str'),('ImportScope','t:ModuleRef')],
     'ENCLog':[('Token',4),('FuncCode',4)], 'ENCMap':[('Token',4)],
     'AssemblyProcessor':[('Processor',4)],
     'AssemblyOS':[('OSPlatformId',4),('OSMajorVersion',4),('OSMinorVersion',4)],
     'AssemblyRefProcessor':[('Processor',4),('AssemblyRef','t:AssemblyRef')],
     'AssemblyRefOS':[('OSPlatformId',4),('OSMajorVersion',4),('OSMinorVersion',4),('AssemblyRef','t:AssemblyRef')],
     'File':[('Flags',4),('Name','str'),('HashValue','blob')],
     'ExportedType':[('Flags',4),('TypeDefId',4),('TypeName','str'),('TypeNamespace','str'),('Implementation','c:Implementation')],
     'ManifestResource':[('Offset',4),('Flags',4),('Name','str'),('Implementation','c:Implementation')],
     'GenericParamConstraint':[('Owner','t:GenericParam'),('Constraint','c:TypeDefOrRef')]})

    rows={}
    for side in L:
     rows[side]={}
     assert set(T[side])<=set(schemas),'unsupported table schema; raw measurement stays available through measure_raw_assemblies.py'
     for name,desc in T[side].items():
      schema=schemas[name];assert sum(width(side,k)for _,k in schema)==desc['row_size'],('schema does not match REA',side,name)
      values=[]
      for i in range(desc['row_count']):
       at=desc['offset']+i*desc['row_size'];row={}
       for col,kind in schema:
        n=width(side,kind);row[col]=U(D[side],at,n);at+=n
       values.append(row)
      rows[side][name]=values

    def compressed(data,i):
     b=data[i]
     if b<128:return b,1
     if b&192==128:return ((b&63)<<8)|data[i+1],2
     if b&224==192:return ((b&31)<<24)|(data[i+1]<<16)|(data[i+2]<<8)|data[i+3],4
     raise ValueError(('reserved compressed length',i))
    def payload(side,index,heap='#Blob'):
     if index==0:return b''
     h=H[side][heap];assert 0<index<h['size'];at=h['offset']+index;n,prefix=compressed(D[side],at);assert index+prefix+n<=h['size'];return D[side][at+prefix:at+prefix+n]
    def string(side,index):
     h=H[side]['#Strings'];assert index<h['size'];start=h['offset']+index;end=D[side].index(0,start,h['offset']+h['size']);return D[side][start:end].decode('utf-8')
    column_results=[]
    for name in dict.fromkeys(list(T['original'])+list(T['rebuilt'])):
     schema=schemas[name];a=rows['original'].get(name,[]);b=rows['rebuilt'].get(name,[])
     record=dict(table=name,rows=min(len(a),len(b)),original_rows=len(a),rebuilt_rows=len(b),columns=[])
     for col,kind in schema:
      numeric=[i+1 for i,(x,y)in enumerate(zip(a,b))if x[col]!=y[col]]
      c=dict(name=col,encoding=kind,width_original=width('original',kind),width_rebuilt=width('rebuilt',kind),different_numeric_rows=len(numeric),examples=[dict(row=i,original=a[i-1][col],rebuilt=b[i-1][col])for i in numeric[:8]])
      if kind in ('blob','str'):
       get=(lambda side,n:payload(side,n))if kind=='blob'else(lambda side,n:string(side,n).encode('utf-8'))
       resolved=[(i+1,x[col],y[col],get('original',x[col]),get('rebuilt',y[col]))for i,(x,y)in enumerate(zip(a,b))]
       c.update(same_referenced_payload_rows=sum(x==y for _,_,_,x,y in resolved),different_referenced_payload_rows=sum(x!=y for _,_,_,x,y in resolved),different_index_but_identical_payload_rows=sum(oi!=bi and x==y for _,oi,bi,x,y in resolved),same_index_but_different_payload_rows=sum(oi==bi and x!=y for _,oi,bi,x,y in resolved))
       if kind=='blob':c['payload_limit']='raw payload only; ECMA signatures may still contain build-local coded type indexes'
      record['columns'].append(c)
     column_results.append(record)

    def heap_records(side,name):
     h=H[side][name];data=checked_region(D[side],h);i=1;records=[]
     while i<len(data):
      if not any(data[i:]):return records,len(data)-i
      n,size=compressed(data,i);assert i+size+n<=len(data)
      value=data[i+size:i+size+n]
      if name=='#US':assert len(value)>=1 and (len(value)-1)%2==0
      records.append(dict(index=i,prefix_size=size,size=n,payload=value));i+=size+n
     return records,0
    heap=[]
    for name in ['#US','#Blob']:
     if name not in H['original']or name not in H['rebuilt']:continue
     a,pa=heap_records('original',name);b,pb=heap_records('rebuilt',name);ca=Counter(x['payload']for x in a);cb=Counter(x['payload']for x in b)
     samples=[]
     for side,values in [('only_original',ca-cb),('only_rebuilt',cb-ca)]:
      for value,count in list(values.items())[:10]:
       row=dict(side=side,count=count,byte_length=len(value),sha256=sha(value))
       if name=='#US':row['utf16_text']=value[:-1].decode('utf-16-le',errors='surrogatepass');row['special_flag']=value[-1]
       samples.append(row)
     heap.append(dict(name=name,original_physical_records=len(a),rebuilt_physical_records=len(b),original_trailing_all_zero_bytes=pa,rebuilt_trailing_all_zero_bytes=pb,multiset_payloads_equal=ca==cb,unmatched_original_payload_occurrences=sum((ca-cb).values()),unmatched_rebuilt_payload_occurrences=sum((cb-ca).values()),examples=samples,limitation='Physical stream records, not metadata rows. Trailing zero bytes are reported separately without claiming that they are semantic entries/padding. Blob raw payloads can contain build-local type indexes.'))

    # Recompute each of the four proven fields from current layout; old offsets
    # are evidence of field instability, never treated as fixed coordinates.
    def coordinates(side):
     data=D[side];layout=L[side];pe=U(data,60,4);assert data[pe:pe+4]==b'PE\0\0';opt=pe+24;magic=U(data,opt,2);assert magic in(0x10b,0x20b)
     def offset(rva,size):
      sec=next(s for s in layout['sections']if s['virtual_address']<=rva and rva+size<=s['virtual_address']+s['size']);return sec['offset']+rva-sec['virtual_address']
     fields={'COFF.TimeDateStamp':dict(offset=pe+8,size=4)}
     module=rows[side].get('Module',[]);assert len(module)==1
     mvid_index=module[0]['Mvid']
     if mvid_index:
      guid=H[side]['#GUID']['offset']+16*(mvid_index-1);assert guid+16<=H[side]['#GUID']['offset']+H[side]['#GUID']['size'];fields['Module.Mvid']=dict(offset=guid,size=16)
     result=dict(fields=fields,debug_directory=None,codeview=None,pdb_path=None,pdb_age=None)
     directory_base=opt+(96 if magic==0x10b else 112)
     debug_rva,debug_size=struct.unpack_from('<II',data,directory_base+6*8)
     if not debug_rva and not debug_size:return result
     assert debug_size and debug_size%28==0
     debug=offset(debug_rva,debug_size);result['debug_directory']=dict(offset=debug,size=debug_size)
     entries=[p for p in range(debug,debug+debug_size,28)if U(data,p+12,4)==2]
     if len(entries)!=1:
      result['debug_detail_status']='UNKNOWN: ambiguous/missing single CodeView entry'
      return result
     entry=entries[0];cv_size=U(data,entry+16,4);cv=U(data,entry+24,4);assert cv+cv_size<=len(data)
     result['codeview']=dict(offset=cv,size=cv_size)
     if data[cv:cv+4]!=b'RSDS':
      result['debug_detail_status']='UNKNOWN: non-RSDS CodeView record'
      return result
     assert cv_size>=24
     fields.update({'IMAGE_DEBUG_DIRECTORY.TimeDateStamp':dict(offset=entry+4,size=4),'CodeView.RSDS.PdbGuid':dict(offset=cv+4,size=16)})
     result.update(pdb_path=data[cv+24:cv+cv_size].split(b'\0',1)[0].decode('utf-8'),pdb_age=U(data,cv+20,4))
     return result
    C={k:coordinates(k)for k in L};fields=[]
    for name in accepted_fields:
     assert all(name in C[side]['fields']for side in C),'proven field has no unambiguous current coordinate'
     a=C['original']['fields'][name];b=C['rebuilt']['fields'][name];fields.append(dict(field=name,original_range=a,rebuilt_range=b,**byte_difference(checked_region(D['original'],a),checked_region(D['rebuilt'],b))))
    def exclude_named_regions(a,b,field_names):
     left=checked_region(D['original'],a);right=checked_region(D['rebuilt'],b);base=byte_difference(left,right);skipped=[];different=equal=0;excluded_diff=0
     for i,(x,y)in enumerate(zip(left,right)):
      exempt=any(C['original']['fields'][f]['offset']<=a['offset']+i<C['original']['fields'][f]['offset']+C['original']['fields'][f]['size'] or C['rebuilt']['fields'][f]['offset']<=b['offset']+i<C['rebuilt']['fields'][f]['offset']+C['rebuilt']['fields'][f]['size']for f in field_names)
      if exempt:skipped.append(i);excluded_diff+=x!=y
      elif x!=y:different+=1
      else:equal+=1
     return dict(original_region=a,rebuilt_region=b,raw=base,analytical_excluded_fields=field_names,excluded_coordinate_pairs=len(skipped),excluded_different_coordinate_pairs=excluded_diff,different_common_pairs_outside_exclusions=different,equal_common_pairs_outside_exclusions=equal,limitation='Comparison only; original/rebuilt byte arrays were never patched, rewritten or normalized.')
    allowed=lambda name:[name]if name in accepted_fields else[]
    analytic={'PE_HEADERS':exclude_named_regions(L['original']['headers'],L['rebuilt']['headers'],allowed('COFF.TimeDateStamp'))}
    if '#GUID'in H['original']and'#GUID'in H['rebuilt']:
     analytic['#GUID']=exclude_named_regions(H['original']['#GUID'],H['rebuilt']['#GUID'],allowed('Module.Mvid'))
    for key,kind,field in [('IMAGE_DEBUG_DIRECTORY','debug_directory','IMAGE_DEBUG_DIRECTORY.TimeDateStamp'),('CodeView.RSDS','codeview','CodeView.RSDS.PdbGuid')]:
     if C['original'][kind]is not None or C['rebuilt'][kind]is not None:
      analytic[key]=exclude_named_regions(C['original'][kind],C['rebuilt'][kind],allowed(field))
    # Whole-file coordinates remain unaligned. The following counts compare only
    # pairs with neither endpoint inside a proven field, not a normalized hash or
    # a total of corresponding semantic bytes.
    analytic['whole_file_positional_outside_field_endpoints']=exclude_named_regions(dict(offset=0,size=len(D['original'])),dict(offset=0,size=len(D['rebuilt'])),list(accepted_fields))
    analytic['whole_file_positional_outside_field_endpoints']['limitation']='Absolute file offset comparator only. Shifted fields may overlap other structures on the opposite side; excluding pairs touching a field is not semantic alignment, normalization, or a claim that all other bytes match after bodies are repaired.'
    output=dict(schema_version=1,operation='measurement-only-pe-cli-columns-and-four-field-analytical-exclusions',raw_report_sha256=json_digest(raw),table_columns=column_results,heap_physical_records=heap,nondeterministic_fields=fields,coordinates=C,analytical_comparisons=analytic,normalization_applied=False,DLL_writes=False,compiler_invocations=0,method_or_type_states_modified=False,source_config_or_ledger_writes=False,producer_sha256=sha(Path(__file__).read_bytes()))

    return output, rows

def measure_value_order(raw, rows):
    L=raw['layouts']
    D={s:Path(L[s]['artifact']['path']).read_bytes()for s in L}
    H={s:{h['name']:h for h in L[s]['streams']}for s in L}
    T={s:{t['name']:t for t in L[s]['tables']}for s in L}
    U=lambda d,p,n:int.from_bytes(d[p:p+n],'little')
    def text(s,index):
     h=H[s]['#Strings'];p=h['offset']+index;e=D[s].index(0,p,h['offset']+h['size']);return D[s][p:e].decode('utf-8')
    def compressed(d,i):
     b=d[i]
     if b<128:return b,1
     if b&192==128:return((b&63)<<8)|d[i+1],2
     if b&224==192:return((b&31)<<24)|(d[i+1]<<16)|(d[i+2]<<8)|d[i+3],4
     raise ValueError('compressed length')
    def userstrings(s):
     if '#US'not in H[s]:return []
     h=H[s]['#US'];d=D[s][h['offset']:h['offset']+h['size']];i=1;result=[]
     while i<len(d)and any(d[i:]):
      n,size=compressed(d,i);p=d[i+size:i+size+n];result.append(dict(index=i,payload=p));i+=size+n
     return result
    us={s:userstrings(s)for s in L};changed=[]
    for i,(a,b)in enumerate(zip(us['original'],us['rebuilt'])):
     if a['payload']!=b['payload']:changed.append(dict(ordinal=i+1,original_index=a['index'],rebuilt_index=b['index'],original_utf16=a['payload'][:-1].decode('utf-16-le',errors='surrogatepass'),rebuilt_utf16=b['payload'][:-1].decode('utf-16-le',errors='surrogatepass')))
    def blob(s,index):
     if index==0:return b''
     h=H[s]['#Blob'];p=h['offset']+index;n,z=compressed(D[s],p);assert index+z+n<=h['size'];return D[s][p+z:p+z+n]
    refs={}
    for s in L:
     refs[s]=[]
     for row in rows[s].get('AssemblyRef',[]):
      refs[s].append(dict(name=text(s,row['Name']),version='.'.join(str(row[k])for k in ['MajorVersion','MinorVersion','BuildNumber','RevisionNumber']),culture=text(s,row['Culture'])or None,flags=row['Flags'],key_bytes_hex=blob(s,row['PublicKeyOrToken']).hex()))
    type_refs={s:[(row['ResolutionScope'],text(s,row['TypeName']),text(s,row['TypeNamespace']))for row in rows[s].get('TypeRef',[])]for s in L}

    def type_key(s,index,seen=()):
     assert index not in seen and index>0
     scope,name,ns=type_refs[s][index-1];tag=scope&3;rid=scope>>2
     if tag==2:
      a=refs[s][rid-1];parent=('assembly',a['name'],a['version'],a['culture'],a['flags'],a['key_bytes_hex'])
     elif tag==3:parent=('type',type_key(s,rid,seen+(index,)))
     elif tag==0:parent=('module',text(s,rows[s]['Module'][0]['Name']))
     elif tag==1:parent=('module-ref',text(s,rows[s]['ModuleRef'][rid-1]['Name']))
     else:raise ValueError('unexpected resolution scope')
     return(parent,ns,name)
    keys={s:[type_key(s,i+1)for i in range(len(type_refs[s]))]for s in L};trchanges=[dict(row=i+1,original=a,rebuilt=b)for i,(a,b)in enumerate(zip(keys['original'],keys['rebuilt']))if a!=b]
    def typedef_info(s,index):
     row=rows[s]['TypeDef'][index-1]
     return dict(name=text(s,row['TypeName']),namespace=text(s,row['TypeNamespace']),extends=row['Extends'])
    def extends(s,value):
     tag=value&3;rid=value>>2
     if tag==1:return('typeref',type_key(s,rid))
     if tag==0:
      if rid==0:return None
      t=typedef_info(s,rid);return('typedef',t['namespace'],t['name'])
     sig=blob(s,rows[s]['TypeSpec'][rid-1]['Signature'])
     # Bounded actual changed Extends case: GENERICINST CLASS TypeDef/Ref,
     # one ELEMENT_TYPE_OBJECT argument. This is not a general signature parser.
     if sig[:2]==bytes([0x15,0x12]):
      target,q=compressed(sig,2);argc,z=compressed(sig,2+q)
      if argc==1 and sig[2+q+z:]==bytes([0x1c]):return('genericinst-class-one-object',extends(s,target),'System.Object')
     return('UNKNOWN-typespec-signature',sig.hex())
    td=[]
    for i in range(min(len(rows['original'].get('TypeDef',[])),len(rows['rebuilt'].get('TypeDef',[])))):
     a,b=typedef_info('original',i+1),typedef_info('rebuilt',i+1)
     if a['extends']!=b['extends']:td.append(dict(row=i+1,type=a['namespace']+'.'+a['name'],original_coded_index=a['extends'],rebuilt_coded_index=b['extends'],original_referenced_base=extends('original',a['extends']),rebuilt_referenced_base=extends('rebuilt',b['extends']),resolved_base_equal=None if 'UNKNOWN'in repr(extends('original',a['extends']))+repr(extends('rebuilt',b['extends'])) else extends('original',a['extends'])==extends('rebuilt',b['extends'])))
    result=dict(operation='read-only-decoded-value-order-diagnostics',US=dict(original_records=len(us['original']),rebuilt_records=len(us['rebuilt']),multiset_payloads_equal=Counter(x['payload']for x in us['original'])==Counter(x['payload']for x in us['rebuilt']),different_payload_ordinals=len(changed),changed_ordinal_examples=changed[:20]),TypeRef=dict(original_rows=len(keys['original']),rebuilt_rows=len(keys['rebuilt']),canonical_reference_multiset_equal=Counter(keys['original'])==Counter(keys['rebuilt']),different_canonical_ordinals=len(trchanges),changed_row_examples=trchanges[:16]),TypeDef_extends_changed=td,limitation='Analytical value resolution only; no row or heap order was changed, no normalized hash produced. Full signature and MemberRef canonicalization are not attempted. Future persistence of current order differences after body repair is UNKNOWN.',producer_sha256=hashlib.sha256(Path(__file__).read_bytes()).hexdigest())

    return result

def measure_field_rva(raw, rows):
    L=raw['layouts']
    D={s:Path(L[s]['artifact']['path']).read_bytes()for s in L}
    H={s:{h['name']:h for h in L[s]['streams']}for s in L}
    def integer(data,p,n):
        return int.from_bytes(data[p:p+n],'little')
    def compressed(data,p):
        b=data[p]
        if b<128:return b,1
        if b&192==128:return((b&63)<<8)|data[p+1],2
        if b&224==192:return((b&31)<<24)|(data[p+1]<<16)|(data[p+2]<<8)|data[p+3],4
        raise ValueError('reserved compressed integer')
    def fielddata(side):
        data=D[side]
        assert sha(data)==L[side]['artifact']['sha256']
        sizes={r['Parent']:r['ClassSize']for r in rows[side].get('ClassLayout',[])}
        result=[]
        for i,row in enumerate(rows[side].get('FieldRVA',[])):
            rva,rid=row['RVA'],row['Field']
            field=rows[side]['Field'][rid-1]
            at=H[side]['#Blob']['offset']+field['Signature']
            length,prefix=compressed(data,at)
            sig=data[at+prefix:at+prefix+length]
            size=None
            reason='UNKNOWN'
            if sig[:2]==bytes([6,17]):
                coded,_=compressed(sig,2)
                if coded&3==0:
                    size=sizes.get(coded>>2)
                    reason='Field ELEMENT_TYPE_VALUETYPE TypeDef + ClassLayout.ClassSize'
            if size is None and len(sig)==2 and sig[0]==6:
                size={2:1,3:2,4:1,5:1,6:2,7:2,8:4,9:4,10:8,11:8,12:4,13:8}.get(sig[1])
                reason='primitive ECMA field type'
            item=dict(row=i+1,field_row=rid,rva=rva,declared_size=size,size_evidence=reason)
            if size is not None:
                section=next(s for s in L[side]['sections']if s['virtual_address']<=rva and rva+size<=s['virtual_address']+s['size'])
                offset=section['offset']+rva-section['virtual_address']
                item.update(offset=offset,sha256=sha(data[offset:offset+size]))
            else:
                item['signature_hex']=sig.hex()
            result.append(item)
        return result
    values={s:fielddata(s)for s in L}
    pairs=[]
    for left,right in zip(values['original'],values['rebuilt']):
        equal=None if left['declared_size']is None or right['declared_size']is None else left['declared_size']==right['declared_size']and left['sha256']==right['sha256']
        pairs.append(dict(original=left,rebuilt=right,field_row_equal=left['field_row']==right['field_row'],content_equal=equal,rva_delta=right['rva']-left['rva']))
    return dict(operation='read-only-FieldRVA-payload-measurement',comparison='same FieldRVA row ordinal; Field ids and unequal row tails retained explicitly',rows=len(pairs),original_rows=len(values['original']),rebuilt_rows=len(values['rebuilt']),original_unpaired_rows=max(0,len(values['original'])-len(values['rebuilt'])),rebuilt_unpaired_rows=max(0,len(values['rebuilt'])-len(values['original'])),fully_sized_rows=sum(x['content_equal']is not None for x in pairs),equal_payload_rows=sum(x['content_equal']is True for x in pairs),different_payload_rows=sum(x['content_equal']is False for x in pairs),unknown_rows=sum(x['content_equal']is None for x in pairs),rva_deltas=sorted(set(x['rva_delta']for x in pairs)),declared_payload_byte_sum_original=sum(x['declared_size']or 0 for x in values['original']),declared_payload_byte_sum_rebuilt=sum(x['declared_size']or 0 for x in values['rebuilt']),pairs=pairs,limitation='Read-only field data. Byte sums may not be added to overlapping section/table layers. Unsupported field-size signatures remain UNKNOWN.')

def measure_body_links(baseline, assembly, raw, detail, seeds):
    result=dict(operation='bounded read-only literal-link measurement from frozen dumps',seeds=seeds,results={},input_provenance={},status='NOT_EVALUATED'if not baseline or not seeds else'pending',limitation='Literal-order links only, not a normalized assembly or a proof of every metadata allocation choice.')
    if not baseline or not seeds:return result
    folder=baseline/assembly
    for side,name in [('original','original-methods.json'),('rebuilt','rebuilt-methods.json')]:
        path=folder/name
        if not path.is_file():
            result['status']='UNKNOWN: dump missing'
            return result
        blob=path.read_bytes()
        dump=json.loads(blob)
        field=detail['coordinates'][side]['fields'].get('Module.Mvid')
        assert field is not None,'cannot bind frozen dump without MVID'
        data=Path(raw['layouts'][side]['artifact']['path']).read_bytes()
        mvid=str(uuid.UUID(bytes_le=data[field['offset']:field['offset']+16]))
        assert dump['mvid']==mvid,'frozen dump MVID does not match artifact'
        result['input_provenance'][name]=dict(path=str(path),sha256=sha(blob),mvid=mvid,artifact_sha256=raw[side]['sha256'])
        selected=[]
        for method in dump['selected_methods']:
            texts=[i.get('operand')for i in method.get('instructions',[])if i.get('opcode')=='ldstr'and isinstance(i.get('operand'),str)]
            for seed in seeds:
                if seed in texts:
                    selected.append(dict(seed=seed,method_id=method['stable_id'],ordered_literals=texts))
        result['results'][name]=selected
    result['status']='MEASURED'
    return result

def proof_profile(baseline, assembly, disabled):
    if disabled or baseline is None:
        return [],dict(status='NO_EXCLUSIONS',reason='disabled or no frozen toolchain profile')
    toolchain_path=baseline/'toolchain.json'
    if not toolchain_path.is_file():
        return [],dict(status='NO_EXCLUSIONS',reason='frozen toolchain profile absent')
    configured=[x for x in read_json(toolchain_path).get('non_deterministic_exceptions',[])if x['assembly']==assembly]
    known={'Module.Mvid':16,'COFF.TimeDateStamp':4,'IMAGE_DEBUG_DIRECTORY.TimeDateStamp':4,'CodeView.RSDS.PdbGuid':16}
    aliases={'MVID':'Module.Mvid','COFF timestamp':'COFF.TimeDateStamp','debug directory timestamp':'IMAGE_DEBUG_DIRECTORY.TimeDateStamp','CodeView PDB GUID':'CodeView.RSDS.PdbGuid'}
    selected=[]
    evidence=[]
    build_path=baseline/assembly/'build-manifest.json'
    build=read_json(build_path)if build_path.is_file()else None
    for item in configured:
        name=item['field']
        assert name in known,'unreviewed exception field'
        path=Path(item['evidence'])
        if not path.is_absolute():path=ROOT/path
        proof=read_json(path)
        assert proof.get('all_identified_fields_observed_changing')is True
        assert proof.get('unexplained_changed_offsets')==[]
        assert proof.get('post_compile_binary_edits')is False
        ranges={aliases.get(r['field'],r['field']):r for r in proof['identified_ranges']}
        assert name in ranges and ranges[name]['size']==known[name]
        assert ranges[name]['changed_bytes']>0
        if build and build.get('compiler_sha256'):
            assert build['compiler_sha256']==proof['compiler_sha256'],'compiler differs from repeatability proof; use --no-exclusions for raw-only'
        selected.append(name)
        evidence.append(dict(field=name,proof=str(path),proof_sha256=sha(path.read_bytes()),proved_size=known[name],observed_changed_bytes_in_proof=ranges[name]['changed_bytes'],proof_coordinates_reused=False,scope=item.get('scope')))
    assert len(selected)==len(set(selected)),'duplicate exception field'
    return selected,dict(status='PROVEN_PROFILE_ONLY',toolchain=str(toolchain_path),toolchain_sha256=sha(toolchain_path.read_bytes()),fields=evidence,original_determinism='UNKNOWN',new_exceptions=[])

def csv_outputs(raw, detail):
    output={}
    names=['layer','name','original_offset','rebuilt_offset','original_bytes','rebuilt_bytes','different_common_bytes','original_unpaired_bytes','rebuilt_unpaired_bytes','different_or_unpaired_bytes']
    rows=[dict(layer='PE',**raw['pe_headers'])]+[dict(layer='PE',**r)for r in raw['pe_sections']]+[dict(layer='metadata',**raw['metadata_root'])]+[dict(layer='stream',**r)for r in raw['metadata_streams']]
    stream=io.StringIO(newline='')
    writer=csv.DictWriter(stream,fieldnames=names)
    writer.writeheader()
    for row in rows:
        values={key:row.get(key)for key in names if key not in('original_offset','rebuilt_offset')}
        values.update(original_offset=row['original_region']['offset']if row['original_region']else None,rebuilt_offset=row['rebuilt_region']['offset']if row['rebuilt_region']else None)
        writer.writerow(values)
    output['regions.csv']=stream.getvalue()
    names=['table','original_rows','rebuilt_rows','original_row_size','rebuilt_row_size','different_common_rows','different_raw_bytes']
    stream=io.StringIO(newline='')
    writer=csv.DictWriter(stream,fieldnames=names)
    writer.writeheader()
    for row in raw['metadata_tables']:
        writer.writerow(dict(table=row['name'],different_raw_bytes=row['different_or_unpaired_bytes'],**{key:row['rows'][key]for key in names if key not in('table','different_raw_bytes')}))
    output['tables.csv']=stream.getvalue()
    names=['table','column','encoding','different_numeric_rows','same_referenced_payload_rows','different_referenced_payload_rows','different_index_but_identical_payload_rows']
    stream=io.StringIO(newline='')
    writer=csv.DictWriter(stream,fieldnames=names)
    writer.writeheader()
    for table in detail['table_columns']:
        for column in table['columns']:
            writer.writerow(dict(table=table['table'],column=column['name'],**{key:column.get(key)for key in names if key not in('table','column')}))
    output['columns.csv']=stream.getvalue()
    return output

def portable_evidence(folder, raw, detail, source_prefix=None):
    output={}
    if folder is None:return output
    for side in ['original','rebuilt']:
        stem=('rea-managed-'+side)if source_prefix is None else source_prefix+'rea-'+side
        summary=read_json(folder/(stem+'-summary.json'))
        summary_result=summary['normalized_result']
        artifact=summary_result['summary']['artifact']
        assert artifact['sha256']==raw[side]['sha256'],'retained evidence is for another artifact'
        assert summary_result['summary']['coverage']['state']=='complete'
        field=detail['coordinates'][side]['fields'].get('Module.Mvid')
        if field:
            data=Path(raw['layouts'][side]['artifact']['path']).read_bytes()
            assert summary_result['summary']['module']['mvid']==str(uuid.UUID(bytes_le=data[field['offset']:field['offset']+16]))
        references=read_json(folder/(stem+'-references.json'))
        result=references['normalized_result']
        assert result['artifact']['sha256']==raw[side]['sha256']
        assert result['parent_evidence_id']==summary_result['parent_evidence_id']
        assert result['source_metadata_status']=='complete'and result['source_coverage']['state']=='complete'
        output['rea-'+side+'-summary.json']=summary
        output['rea-'+side+'-references.json']=references
    return output

def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--baseline',type=Path,help='Frozen directory with assemblies/artifacts.json, assembly/verification.json and toolchain.json')
    parser.add_argument('--assembly',default='KairoLibrary')
    parser.add_argument('--original',type=Path)
    parser.add_argument('--rebuilt',type=Path)
    parser.add_argument('--output-dir',type=Path,default=ROOT/'reports')
    parser.add_argument('--label',default='round10')
    parser.add_argument('--rea-root',type=Path)
    parser.add_argument('--rea-evidence-dir',type=Path,help='Optional retained summary/reference JSONs; their artifact SHA/MVID linkage is checked')
    parser.add_argument('--rea-evidence-prefix',help='Optional prefix of already published raw-rea-*.json records in the evidence directory')
    parser.add_argument('--literal-seed',action='append',default=[])
    parser.add_argument('--no-exclusions',action='store_true',help='Retain every byte, without applying any analytical exception')
    args=parser.parse_args()
    for name in [args.assembly,args.label]+([args.rea_evidence_prefix]if args.rea_evidence_prefix is not None else[]):
        if not re.fullmatch('[A-Za-z0-9][A-Za-z0-9_.-]*',name):
            raise ValueError('unsafe assembly/label')
    baseline=args.baseline.resolve()if args.baseline else None
    original=(args.original or ROOT/'original/extracted/assets/bin/Data/Managed'/(args.assembly+'.dll')).resolve()
    rebuilt=(args.rebuilt or baseline/'assemblies'/(args.assembly+'.dll')if baseline else args.rebuilt)
    if rebuilt is None:parser.error('--rebuilt is required without --baseline')
    rebuilt=rebuilt.resolve()
    authority={}
    if baseline:
        manifest_path=baseline/'assemblies/artifacts.json'
        receipt_path=baseline/args.assembly/'verification.json'
        artifact=read_json(manifest_path)[args.assembly]
        receipt=read_json(receipt_path)
        assert sha(rebuilt.read_bytes())==artifact['sha256']==receipt['build_sha256']
        assert sha(original.read_bytes())==receipt['original_sha256']
        snapshot_path=baseline/'snapshot.json'
        authority=dict(kind='frozen-baseline',baseline=str(baseline),snapshot=read_json(snapshot_path)if snapshot_path.is_file()else None,artifact_manifest_sha256=sha(manifest_path.read_bytes()),verification_receipt_sha256=sha(receipt_path.read_bytes()))
    else:
        authority=dict(kind='explicit-read-only-pair',original_sha256=sha(original.read_bytes()),rebuilt_sha256=sha(rebuilt.read_bytes()))
    accepted,proof=proof_profile(baseline,args.assembly,args.no_exclusions)
    raw=measure_pair(original,rebuilt,args.rea_root)
    raw.update(assembly=args.assembly,label=args.label,input_authority=authority,input_mode='direct immutable read; no DLL snapshot/copy',analytical_exception_profile=proof)
    detail,rows=measure_detail(raw,accepted)
    values=measure_value_order(raw,rows)
    field_rva=measure_field_rva(raw,rows)
    links=measure_body_links(baseline,args.assembly,raw,detail,args.literal_seed)
    evidence=portable_evidence(args.rea_evidence_dir,raw,detail,args.rea_evidence_prefix)
    prefix=args.label+'-'+args.assembly+'-raw-'
    documents={'measurement.json':raw,'detail.json':detail,'value-order.json':values,'field-rva.json':field_rva,'body-links.json':links,**evidence}
    encoded={prefix+name:serialized(value).encode('utf-8')for name,value in documents.items()}
    encoded.update({prefix+name:value.encode('utf-8')for name,value in csv_outputs(raw,detail).items()})
    # Recheck input identities after all readers, without touching target bytes.
    assert sha(original.read_bytes())==raw['original']['sha256']
    assert sha(rebuilt.read_bytes())==raw['rebuilt']['sha256']
    publication=dict(schema_version=1,operation='read-only-raw-measurement-publication',assembly=args.assembly,label=args.label,input_authority=authority,original=raw['original'],rebuilt=raw['rebuilt'],analytical_exception_profile=proof,producer_sha256=sha(Path(__file__).read_bytes()),existing_producer_source_hashes=raw['producer_source_hashes'],files_sha256={name:sha(data)for name,data in encoded.items()},DLLs_written_or_copied=False,normalization_applied=False,normalized_hash_produced=False,compiler_invocations=0,target_executed=False,reconstruction_source_or_ledger_config_edits=False,outputs_new_only=True,commits=0)
    encoded[prefix+'manifest.json']=serialized(publication).encode('utf-8')
    destination=args.output_dir.resolve()
    paths=[destination/name for name in encoded]
    if any(path.exists()for path in paths):
        raise FileExistsError('Output exists; choose a new --label or --output-dir. No report was replaced.')
    destination.mkdir(parents=True,exist_ok=True)
    for name,data in encoded.items():
        with(destination/name).open('xb')as stream:stream.write(data)
    print(json.dumps(dict(prefix=str(destination/prefix),files=len(encoded),raw_whole_file=raw['raw_whole_file'],tables=len(raw['metadata_tables']),excluded_fields=accepted,normalization_applied=False)))
    
if __name__=='__main__':
    main()
