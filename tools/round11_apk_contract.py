"""Calibrate raw DEFLATE on original DLLs; never build or modify an APK."""
import hashlib
import json
from pathlib import Path
import struct
import sys
import zipfile
import zlib

ROOT=Path(__file__).resolve().parents[1]


def sha(data):return hashlib.sha256(data).hexdigest()
def compressed_member(apk,entry):
    with apk.open('rb') as stream:
        stream.seek(entry.header_offset);header=stream.read(30)
        if header[:4]!=b'PK\x03\x04':raise ValueError('Invalid local ZIP header')
        name,extra=struct.unpack_from('<HH',header,26)
        offset=entry.header_offset+30+name+extra;stream.seek(offset)
        raw=stream.read(entry.compress_size)
        if len(raw)!=entry.compress_size:raise ValueError('Truncated compressed ZIP member')
        return raw,offset


def main():
    apk=ROOT/'original/app.apk'
    if not apk.is_file():raise FileNotFoundError(apk)
    before_sha=sha(apk.read_bytes());calibrations={};inventory=[]
    with zipfile.ZipFile(apk) as archive:
        for entry in archive.infolist():
            raw,offset=compressed_member(apk,entry)
            inventory.append({'path':entry.filename,'compressed_sha256':sha(raw),
                'compressed_size':entry.compress_size,'uncompressed_size':entry.file_size,
                'compression_method':entry.compress_type,'data_offset_mod4096':offset%4096})
        for assembly in ('KairoLibrary','Assembly-CSharp'):
            name='assets/bin/Data/Managed/'+assembly+'.dll';entry=archive.getinfo(name)
            if entry.compress_type!=zipfile.ZIP_DEFLATED:raise ValueError('Expected a DEFLATE DLL member')
            original=archive.read(entry);raw,_=compressed_member(apk,entry)
            assert zlib.decompress(raw,-15)==original
            levels=[]
            for level in range(10):
                compressor=zlib.compressobj(level,zlib.DEFLATED,-15,8,zlib.Z_DEFAULT_STRATEGY)
                candidate=compressor.compress(original)+compressor.flush(zlib.Z_FINISH)
                assert zlib.decompress(candidate,-15)==original
                levels.append({'level':level,'compressed_size':len(candidate),
                    'size_matches_original':len(candidate)==len(raw),'compressed_bytes_equal_original':candidate==raw,
                    'compressed_sha256':sha(candidate)})
            exact=[r['level'] for r in levels if r['compressed_bytes_equal_original']]
            size=[r['level'] for r in levels if r['size_matches_original']]
            selected=max(exact or size) if (exact or size) else None
            calibrations[assembly]={'path':name,'original_dll_sha256':sha(original),
                'original_compressed_size':len(raw),'original_compressed_sha256':sha(raw),
                'original_uncompressed_size':len(original),'levels':levels,
                'byte_identical_levels':exact,'size_matching_levels':size,'selected_level':selected,
                'selection_basis':'byte-identical-stream' if exact else 'compressed-size-only' if size else 'UNKNOWN: no default-strategy level matches',
                'historical_encoder_identity':'UNKNOWN; calibration does not identify the original library/version'}
    assert sha(apk.read_bytes())==before_sha
    report={'operation':'read-only-apk-dll-deflate-calibration','apk_sha256':before_sha,
        'entry_count':len(inventory),'python_version':sys.version,'zlib_compile_version':zlib.ZLIB_VERSION,
        'zlib_runtime_version':zlib.ZLIB_RUNTIME_VERSION,'parameters':{'method':'DEFLATED','wbits':-15,'memLevel':8,
            'strategy':'Z_DEFAULT_STRATEGY','flush':'Z_FINISH','input':'entire original DLL in one compress call'},
        'assemblies':calibrations,'entries':inventory,'apk_built':False,'target_executed':False,
        'original_modified':False,'method_gains':0,'method_losses':0,'producer_sha256':sha(Path(__file__).read_bytes())}
    output=ROOT/'reports/round11-apk-contract.json'
    with output.open('x',encoding='utf-8',newline='\n') as stream:json.dump(report,stream,indent=2);stream.write('\n')
    print(json.dumps({a:{k:v[k] for k in ('original_compressed_size','size_matching_levels','byte_identical_levels','selected_level','selection_basis')} for a,v in calibrations.items()}))


if __name__=='__main__':main()
