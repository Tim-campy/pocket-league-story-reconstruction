"""Read file-backed PE CodeView records without loading or modifying an assembly."""
from pathlib import Path
import hashlib
import struct


def inspect_codeview(path):
    path=Path(path);data=path.read_bytes()
    u=lambda at,fmt:struct.unpack_from(fmt,data,at)
    pe,=u(60,'<I')
    if data[pe:pe+4]!=b'PE\0\0':raise ValueError('Not a PE file')
    sections,=u(pe+6,'<H');optional_size,=u(pe+20,'<H');opt=pe+24
    magic,=u(opt,'<H')
    if magic not in (0x10b,0x20b):raise ValueError('Unsupported PE optional header')
    ranges=[]
    for i in range(sections):
        entry=opt+optional_size+i*40
        virtual_size,rva,raw_size,raw_offset=u(entry+8,'<IIII')
        ranges.append((rva,raw_size,raw_offset))
    def mapped(rva,size):
        hits=[raw+rva-start for start,length,raw in ranges if start<=rva and rva+size<=start+length]
        if len(hits)!=1:raise ValueError('Ambiguous or non-file-backed RVA')
        if hits[0]+size>len(data):raise ValueError('Out-of-file RVA')
        return hits[0]
    directory_base=opt+(96 if magic==0x10b else 112)
    debug_rva,debug_size=u(directory_base+6*8,'<II')
    if not debug_size or debug_size%28:raise ValueError('Missing/invalid debug directory')
    debug=mapped(debug_rva,debug_size);records=[]
    for entry in range(debug,debug+debug_size,28):
        _,stamp,major,minor,kind,size,address,pointer=u(entry,'<IIHHIIII')
        if kind!=2:continue
        if pointer+size>len(data) or data[pointer:pointer+4]!=b'RSDS':raise ValueError('Unsupported CodeView record')
        if mapped(address,size)!=pointer:raise ValueError('CodeView RVA/file offset mismatch')
        raw=data[pointer:pointer+size];name=raw[24:].split(b'\0',1)[0]
        records.append({'directory_offset':entry,'directory_timestamp_offset':entry+4,
            'record_offset':pointer,'record_size':size,'pdb_guid_offset':pointer+4,
            'pdb_guid_hex':raw[4:20].hex(),'pdb_age':u(pointer+20,'<I')[0],
            'pdb_path':name.decode('utf-8'),'pdb_path_offset':pointer+24,
            'record_excluding_guid_hex':(raw[:4]+raw[20:]).hex(),
            'path_and_padding_hex':raw[24:].hex()})
    if len(records)!=1:raise ValueError('Expected exactly one RSDS record')
    return {'path':str(path),'sha256':hashlib.sha256(data).hexdigest(),
        'byte_length':len(data),'codeview':records[0]}
