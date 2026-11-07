"""Bind native compiler PDB to its PE by GUID/age; copy sidecar, never edit PE."""
import hashlib
import json
from pathlib import Path
import shutil
import struct


def u32(data, offset):
    return struct.unpack_from('<I', data, offset)[0]


def codeview(data):
    pe = u32(data, 60)
    if data[pe:pe + 4] != b'PE\0\0':
        raise ValueError('Not PE')
    opt = pe + 24
    magic = struct.unpack_from('<H', data, opt)[0]
    directory = 96 if magic == 0x10b else 112 if magic == 0x20b else None
    if directory is None:
        raise ValueError('Unknown optional header')
    rva, size = struct.unpack_from('<II', data, opt + directory + 6 * 8)
    sections = opt + struct.unpack_from('<H', data, pe + 20)[0]
    def offset(value):
        for i in range(struct.unpack_from('<H', data, pe + 6)[0]):
            p = sections + i * 40
            address, rawsize, raw = struct.unpack_from('<III', data, p + 12)
            if address <= value < address + rawsize:
                return raw + value - address
        raise ValueError('Debug RVA is not file-backed')
    start = offset(rva)
    records = []
    for p in range(start, start + size, 28):
        if u32(data, p + 12) != 2:
            continue
        length, _, raw = struct.unpack_from('<III', data, p + 16)
        record = data[raw:raw + length]
        if record[:4] != b'RSDS' or len(record) < 25:
            raise ValueError('Unknown CodeView record')
        records.append(dict(guid=record[4:20].hex(), age=u32(record, 20),
                            path=record[24:].split(b'\0', 1)[0].decode('utf-8')))
    if len(records) != 1:
        raise ValueError('Expected one unambiguous RSDS record')
    return records[0]


def pdb_identity(data):
    if data[:32] != b'Microsoft C/C++ MSF 7.00\r\n\x1aDS\0\0\0':
        raise ValueError('Expected native MSF7 PDB')
    size, _, blocks, directory_size, _, blockmap = struct.unpack_from('<6I', data, 32)
    if size not in (512, 1024, 2048, 4096, 8192) or blocks * size > len(data):
        raise ValueError('Invalid PDB block layout')
    pages = (directory_size + size - 1) // size
    if pages * 4 > size:
        raise ValueError('Unsupported multi-page PDB block map')
    indices = struct.unpack_from('<' + 'I' * pages, data, blockmap * size)
    if any(n >= blocks for n in indices):
        raise ValueError('Invalid PDB directory page')
    directory = b''.join(data[n * size:(n + 1) * size] for n in indices)[:directory_size]
    count = u32(directory, 0)
    sizes = struct.unpack_from('<' + 'I' * count, directory, 4)
    cursor = 4 + count * 4
    streams = []
    for length in sizes:
        n = 0 if length == 0xffffffff else (length + size - 1) // size
        ids = struct.unpack_from('<' + 'I' * n, directory, cursor)
        cursor += n * 4
        if any(index >= blocks for index in ids):
            raise ValueError('Invalid PDB stream page')
        streams.append((length, ids))
    length, ids = streams[1]
    info = b''.join(data[n * size:(n + 1) * size] for n in ids)[:length]
    if len(info) < 28:
        raise ValueError('Missing PDB information stream')
    return dict(guid=info[12:28].hex(), age=u32(info, 8),
                guid_file_offset=ids[0] * size + 12,
                age_file_offset=ids[0] * size + 8)


def mirror(binary, flags):
    values = [f[5:] for f in flags if f.lower().startswith('/pdb:')]
    if not values:
        return None
    if len(values) != 1:
        raise ValueError('Ambiguous compiler PDB output')
    binary = Path(binary)
    data = binary.read_bytes()
    before = hashlib.sha256(data).hexdigest()
    cv = codeview(data)
    if cv['path'].casefold() != values[0].casefold():
        raise ValueError('PE CodeView path differs from compiler flag')
    source = Path(values[0])
    payload = source.read_bytes()
    identity = pdb_identity(payload)
    if (identity['guid'], identity['age']) != (cv['guid'], cv['age']):
        raise ValueError('Compiler PDB GUID/age does not match private PE')
    target = binary.with_suffix('.pdb')
    if source.resolve() != target.resolve():
        shutil.copyfile(source, target)
    digest = hashlib.sha256(payload).hexdigest()
    if hashlib.sha256(target.read_bytes()).hexdigest() != digest or hashlib.sha256(binary.read_bytes()).hexdigest() != before:
        raise ValueError('Sidecar mirror changed bytes unexpectedly')
    result = dict(compiler_pdb=str(source), analysis_mirror=str(target), pdb_sha256=digest,
                  guid=cv['guid'], age=cv['age'], guid_and_age_bound=True,
                  private_pe_sha256=before, assembly_bytes_modified=False)
    binary.with_suffix('.pdb-mirror.json').write_text(json.dumps(result, indent=2) + '\n', encoding='utf-8')
    return result
