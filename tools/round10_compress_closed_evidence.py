"""Compress closed private NTFS snapshots, proving every file byte unchanged."""
import ctypes
from ctypes import wintypes
import json
import os
from pathlib import Path
import subprocess
from compare_kairo import sha

ROOT = Path(__file__).resolve().parents[1]


def allocated(path):
    high = wintypes.DWORD()
    function = ctypes.windll.kernel32.GetCompressedFileSizeW
    function.argtypes = [wintypes.LPCWSTR, ctypes.POINTER(wintypes.DWORD)]
    function.restype = wintypes.DWORD
    low = function(str(path), ctypes.byref(high))
    if low == 0xffffffff and ctypes.windll.kernel32.GetLastError():
        raise ctypes.WinError()
    return low + (high.value << 32)


def main():
    folder = ROOT / 'build/round10/storage-review'
    assert not folder.exists()
    folder.mkdir()
    labels = ['c01'] + ['cycle%02d' % n for n in range(1, 7)]
    results = []
    for label in labels:
        source = (ROOT / 'build/round10/cycles' / label).resolve()
        assert source.is_relative_to(ROOT / 'build/round10/cycles') and source.is_dir()
        files = sorted(p for p in source.rglob('*') if p.is_file())
        assert all(p.resolve().is_relative_to(source) for p in files)
        before = {p.relative_to(ROOT).as_posix(): sha(p) for p in files}
        old_size = sum(allocated(p) for p in files)
        log = folder / (label + '-compact.log')
        with log.open('wb') as stream:
            result = subprocess.run([str(Path(os.environ['SystemRoot']) / 'System32/compact.exe'),
                '/C', '/S:' + str(source), '/EXE:LZX', '/I', '/Q'],
                stdout=stream, stderr=subprocess.STDOUT,
                creationflags=subprocess.CREATE_NO_WINDOW)
        after = {p.relative_to(ROOT).as_posix(): sha(p) for p in files}
        assert before == after, 'File bytes changed during NTFS compression'
        manifest = folder / (label + '-byte-proof.json')
        manifest.write_text(json.dumps(before, indent=2) + '\n', encoding='utf-8')
        row = dict(snapshot=label, files=len(files), unchanged_file_hashes=True,
                   allocated_bytes_before=old_size, allocated_bytes_after=sum(allocated(p) for p in files),
                   compact_exit_code=result.returncode,
                   manifest=manifest.relative_to(ROOT).as_posix(), manifest_sha256=sha(manifest))
        results.append(row)
        print(json.dumps(row), flush=True)
    report = dict(operation='NTFS-storage-only-compression-of-closed-private-evidence',
                  rows=results, file_contents_changed=False, original_files_touched=False,
                  active_cycle_snapshots_touched=False, DLL_normalization_applied=False,
                  method_state_writes=False, producer_sha256=sha(Path(__file__)))
    public = ROOT / 'reports/round10-closed-evidence-storage.json'
    assert not public.exists()
    public.write_text(json.dumps(report, indent=2) + '\n', encoding='utf-8')


if __name__ == '__main__':
    main()
