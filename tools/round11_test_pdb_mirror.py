"""One owned redirected-PDB fixture; no game assembly build or binary rewriting."""
import hashlib
import json
from pathlib import Path
import subprocess
import sys
from compare_kairo import differences, sha
from round4_symbols import inspect_methods
from round11_pdb_mirror import codeview, pdb_identity, mirror

ROOT = Path(__file__).resolve().parents[1]


def main():
    folder = ROOT / 'build/round11/pdb-mirror-fixture'
    folder.mkdir(exist_ok=False)
    manifest = json.loads((ROOT / 'build/round11/baseline/KairoLibrary/build-manifest.json').read_text())
    source = folder / 'Fixture.cs'
    source.write_text('public class Fixture { public int Run(int n) { if (n < 0) return 0; return n + 1; } }\n')
    binary = folder / 'private/Fixture.dll'
    binary.parent.mkdir()
    redirected = folder / 'redirected/Fixture.pdb'
    redirected.parent.mkdir()
    flags = [f for f in manifest['flags'] if not f.startswith(('/out:', '/pdb:')) and f != '/noconfig']
    flags += ['/out:' + str(binary), '/pdb:' + str(redirected)]
    args = flags + ['/r:' + r['path'] for r in manifest['references']] + [str(source)]
    response = folder / 'compile.rsp'
    response.write_text('\n'.join('"' + a + '"' for a in args) + '\n')
    run = subprocess.run([manifest['compiler'], '/noconfig', '@' + str(response)], capture_output=True)
    (folder / 'compiler.log').write_bytes(run.stdout + run.stderr)
    if run.returncode:
        raise RuntimeError('Own fixture failed to compile')
    before_sha = sha(binary)
    before = inspect_methods(binary, symbols=True)
    assert before['symbol_kind'] == 'NONE'
    proof = mirror(binary, flags)
    after = inspect_methods(binary, symbols=True)
    assert after['symbol_kind'] == 'PDB' and any(m['sequence_points'] for m in after['methods'])
    left = {m['stable_id']: m for m in before['methods']}
    right = {m['stable_id']: m for m in after['methods']}
    assert all(not differences(m, right[k])[0] for k, m in left.items())
    assert sha(binary) == before_sha
    payload = redirected.read_bytes()
    identity = pdb_identity(payload)
    negatives = []
    for name, offset in [('GUID', identity['guid_file_offset']), ('age', identity['age_file_offset'])]:
        corrupted = bytearray(payload)
        corrupted[offset] ^= 1
        redirected.write_bytes(corrupted)
        try:
            mirror(binary, flags)
        except ValueError as error:
            negatives.append({'kind': name, 'rejected': True, 'reason': str(error)})
        else:
            raise AssertionError('Stale PDB accepted')
        finally:
            redirected.write_bytes(payload)
        assert sha(binary) == before_sha and sha(binary.with_suffix('.pdb')) == hashlib.sha256(payload).hexdigest()
    result = dict(operation='own-redirected-native-PDB-mirror-fixture', target_compilations=0,
                  own_fixture_compilations=1, symbols_before='NONE', symbols_after='PDB',
                  sequence_points_after=sum(len(m['sequence_points']) for m in after['methods']),
                  all_method_bodies_unchanged=True, assembly_sha256_before_and_after=before_sha,
                  mirror=proof, negatives=negatives, corrupted_files='owned fixture PDB input only; restored exactly',
                  compiler_sha256=sha(Path(manifest['compiler'])),
                  dependency_sha256={str(p.relative_to(ROOT)): sha(p) for p in
                                     (Path(__file__), ROOT / 'tools/round11_pdb_mirror.py')})
    (folder / 'report.json').write_text(json.dumps(result, indent=2) + '\n')
    print(json.dumps(result))


if __name__ == '__main__':
    main()
