"""Provide the original PDB output path through an owned build-only DOS alias."""
import ctypes
import hashlib
import json
import os
from pathlib import Path,PureWindowsPath
import subprocess

ROOT=Path(__file__).resolve().parents[1]
CONFIG=ROOT/'config/kairo-pdb-output.json'


def prepare():
    cfg=json.loads(CONFIG.read_text(encoding='utf-8'))
    name=PureWindowsPath(cfg['original_pdb_path'])
    if os.name!='nt' or not name.is_absolute():raise ValueError('Historical PDB output requires Windows absolute path')
    root=(ROOT/cfg['drive_alias_root']).resolve()
    if not root.is_relative_to((ROOT/'build').resolve()):raise ValueError('Drive alias must stay inside build/')
    root.mkdir(parents=True,exist_ok=True)
    drive=name.drive
    query=ctypes.WinDLL('kernel32',use_last_error=True).QueryDosDeviceW
    query.argtypes=[ctypes.c_wchar_p,ctypes.c_wchar_p,ctypes.c_uint32];query.restype=ctypes.c_uint32
    def current():
        buffer=ctypes.create_unicode_buffer(32768)
        if query(drive,buffer,len(buffer)):return buffer.value
        error=ctypes.get_last_error()
        if error not in (2,3):raise ctypes.WinError(error)
        return None
    expected='\\??\\'+str(root);observed=current();created=False
    if observed is None:
        result=subprocess.run(['subst',drive,str(root)],capture_output=True)
        if result.returncode:raise RuntimeError('Could not create the owned PDB drive alias: '+result.stderr.decode(errors='replace'))
        observed=current();created=True
    if observed.casefold()!=expected.casefold():
        raise ValueError('Historical drive is occupied by another target; no files were written there: '+drive)
    pdb=Path(str(name));physical=root.joinpath(*name.parts[1:])
    if not physical.resolve().is_relative_to(root):raise ValueError('PDB path escaped the build alias')
    physical.parent.mkdir(parents=True,exist_ok=True)
    if not os.path.samefile(pdb.parent,physical.parent):raise ValueError('PDB alias identity mismatch')
    return pdb,{'compiler_output_path':str(name),'physical_output_path':str(physical),
        'drive':drive,'drive_target':str(root),'alias_created_this_build':created,
        'configuration_sha256':hashlib.sha256(CONFIG.read_bytes()).hexdigest(),
        'helper_sha256':hashlib.sha256(Path(__file__).read_bytes()).hexdigest()}
