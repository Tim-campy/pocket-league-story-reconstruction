"""Measure one source-only metadata experiment, preserving semantic closure.

Never edits a PE or method/type state. The official verifier owns all states.
Raw source steps are separate from A's eleven body cycles and from B's cap.
"""
import argparse
import hashlib
import json
from pathlib import Path
import subprocess
import sys
from compare_kairo import sha
from apply_il_rules import receipt_is_fresh
from measure_raw_assemblies import inspect_layout

ROOT = Path(__file__).resolve().parents[1]


def read(path):
    return json.loads(Path(path).read_text(encoding='utf-8-sig'))


def write(path, value):
    Path(path).write_text(json.dumps(value, indent=2) + '\n', encoding='utf-8')


def run(args, log, accepted=(0,)):
    p = subprocess.run([sys.executable, '-X', 'utf8', *map(str, args)], cwd=ROOT, capture_output=True)
    log.parent.mkdir(parents=True, exist_ok=True)
    log.write_bytes(p.stdout + p.stderr)
    if p.returncode not in accepted:
        raise RuntimeError(str(args) + '\n' + (p.stdout+p.stderr).decode('utf-8', errors='replace')[-3000:])


def invariants():
    for assembly in ('KairoLibrary', 'Assembly-CSharp'):
        r = read(ROOT / ('build/verification-'+assembly+'.json'))
        if not receipt_is_fresh(ROOT, r) or any(r.get(k) for k in ('regressions', 'body_regressions', 'type_regressions')):
            raise ValueError('Fresh zero-regression verifier proof required: '+assembly)
        if assembly == 'KairoLibrary':
            if (r['counts']['matched'] != 3287 or r['types_equal'] != 337 or r['types_total'] != 337
                    or any(not m['body_equal'] or m.get('generated_group_pending')
                           for m in r['methods'].values() if m.get('body_present'))):
                raise ValueError('Kairo semantic/type closure changed')


def signatures(data, layout, name):
    table = next((x for x in layout['tables'] if x['name'] == name), None)
    if not table: return []
    assert table['row_size'] == 2
    heap = next(x for x in layout['streams'] if x['name'] == '#Blob')['offset']
    result = []
    for i in range(table['row_count']):
        index = int.from_bytes(data[table['offset']+2*i:table['offset']+2*i+2], 'little')
        pos = heap+index; b = data[pos]
        if b < 128: length, n = b, 1
        elif b < 192: length, n = ((b&63)<<8)|data[pos+1], 2
        else: length, n = ((b&31)<<24)|int.from_bytes(data[pos+1:pos+4], 'big'), 4
        result.append(data[pos+n:pos+n+length].hex())
    return result


def table_bytes(data, layout, name):
    table = next(x for x in layout['tables'] if x['name'] == name)
    return data[table['offset']:table['offset']+table['row_count']*table['row_size']]


def snapshot_provenance(folder):
    manifest_path = folder/'KairoLibrary/build-manifest.json'
    manifest = read(manifest_path)
    artifacts_path = folder/'assemblies/artifacts.json'
    artifacts = read(artifacts_path)
    return dict(artifacts_sha256=sha(artifacts_path),
        Kairo_PE_sha256=artifacts['KairoLibrary']['sha256'],
        build_manifest_sha256=sha(manifest_path),
        verification_receipt_sha256=sha(folder/'KairoLibrary/verification.json'),
        source_hashes={r['path']:r['sha256'] for r in manifest['sources']})


def main():
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument('--label', required=True)
    p.add_argument('--patches', required=True, type=Path)
    p.add_argument('--cause', required=True)
    p.add_argument('--require-typeref-equal', action='store_true')
    p.add_argument('--require-typespec-equal', action='store_true')
    p.add_argument('--new-constant-payload')
    p.add_argument('--new-constant-row', type=int)
    args = p.parse_args()
    if args.label not in {'r%02d' % i for i in range(1, 7)}: raise ValueError('Exactly six authorized steps')
    journal_path = ROOT/'reports/round12-raw-source-journal.json'
    journal = read(journal_path) if journal_path.exists() else dict(body_cycle_journal_unchanged=True, steps=[])
    if args.label != 'r%02d' % (len(journal['steps'])+1): raise ValueError('Do not repeat or skip a journaled source step')
    folder = ROOT/'build/round12/raw-source-trials'/args.label
    if folder.exists(): raise FileExistsError('Preserve existing trial; diagnose before retry')
    folder.mkdir(parents=True)
    patches = read(args.patches)['patches']
    originals = {}
    for patch in patches:
        path = ROOT/patch['logical_path']
        if not path.resolve().is_relative_to((ROOT/'src/KairoLibrary').resolve()): raise ValueError('Only authorized Kairo C# source')
        originals[path] = path.read_bytes()
        text = originals[path].decode('utf-8')
        if text.count(patch['before']) != 1: raise ValueError('Patch is stale or ambiguous')
    invariants()
    run(['tools/round12_freeze.py','--destination',folder/'before'],folder/'freeze-before.log')
    old_manifest = read(folder/'before/KairoLibrary/build-manifest.json')
    try:
        for patch in patches:
            path = ROOT/patch['logical_path']; text = path.read_bytes().decode('utf-8')
            path.write_bytes(text.replace(patch['before'],patch['after'],1).encode('utf-8'))
        run(['tools/verify','--all'],folder/'full-verification.log',accepted=(0,1))
        invariants()
        run(['tools/round12_freeze.py','--destination',folder/'after'],folder/'freeze-after.log')
        new_manifest = read(folder/'after/KairoLibrary/build-manifest.json')
        for key in ('compiler','compiler_sha256','flags','references'):
            if old_manifest[key] != new_manifest[key]: raise ValueError('Compiler/profile changed: '+key)
        label = 'raw-source-'+args.label
        run(['tools/round12_measure.py','--snapshot',folder/'after','--label',label],folder/'measurement.log')
        measurement_path = ROOT/('reports/round12-'+label+'-measurement.json')
        measured = read(measurement_path)['assemblies']['KairoLibrary']
        if (measured['matched_methods'] != 3287 or measured['matched_original_il_bytes'] != 269894
                or measured['divergent_bodies'] or measured['equal_body_held_methods']): raise ValueError('Measured closure changed')
        run(['tools/round12_raw_compare.py','--snapshot',folder/'after','--assembly','KairoLibrary','--label',label],folder/'raw.log')
        raw_path = ROOT/('reports/round12-'+label+'-KairoLibrary-raw.json'); raw = read(raw_path)
        layout = read(ROOT/raw['outputs']['layout']['path'])['layouts']
        data = {k:Path(v['artifact']['path']).read_bytes() for k,v in layout.items()}
        cause_checks = {}
        if args.require_typeref_equal:
            cause_checks['TypeRef_regional_bytes_equal'] = table_bytes(data['original'],layout['original'],'TypeRef') == table_bytes(data['rebuilt'],layout['rebuilt'],'TypeRef')
            if not cause_checks['TypeRef_regional_bytes_equal']: raise ValueError('TypeRef order not restored')
        if args.require_typespec_equal:
            cause_checks['TypeSpec_payload_order_equal'] = signatures(data['original'],layout['original'],'TypeSpec') == signatures(data['rebuilt'],layout['rebuilt'],'TypeSpec')
            if not cause_checks['TypeSpec_payload_order_equal']: raise ValueError('TypeSpec identity order not restored')
        if args.new_constant_payload:
            current = signatures(data['rebuilt'],layout['rebuilt'],'StandAloneSig')
            old_art = read(folder/'before/assemblies/artifacts.json')['KairoLibrary']; old_dll = folder/'before'/old_art['path']
            old_sigs = signatures(old_dll.read_bytes(),inspect_layout(old_dll),'StandAloneSig')
            if len(current) != len(old_sigs)+1 or current.count(args.new_constant_payload) != 1:
                raise ValueError('Expected exactly one new PDB constant FieldSig')
            if current[args.new_constant_row-1] != args.new_constant_payload:
                raise ValueError('PDB constant host does not restore the measured original StandSig row')
            cause_checks['new_FieldSig'] = dict(payload=args.new_constant_payload,row=args.new_constant_row)
    except BaseException as error:
        for path, value in originals.items(): path.write_bytes(value)
        run(['tools/verify','--all'],folder/'rollback-verification.log',accepted=(0,1))
        invariants()
        write(folder/'negative-attempt.json',dict(error=str(error),source_rolled_back=True,method_or_type_states_written_by='official verifier only'))
        raise
    row = dict(label=args.label,cause=args.cause,methods_gained=0,IL_bytes_gained=0,
        source_patches=str(args.patches),source_patches_sha256=sha(args.patches),
        before_snapshot=str((folder/'before').relative_to(ROOT)),after_snapshot=str((folder/'after').relative_to(ROOT)),
        before_snapshot_sha256=sha(folder/'before/snapshot.json'),after_snapshot_sha256=sha(folder/'after/snapshot.json'),
        measurement=str(measurement_path.relative_to(ROOT)),measurement_sha256=sha(measurement_path),
        raw_report=str(raw_path.relative_to(ROOT)),raw_report_sha256=sha(raw_path),
        raw_status=raw['status'],outside_exceptions_differing_bytes=raw['outside_exceptions_differing_bytes'],
        recompiled_sha256=raw['recompiled_sha256'],cause_checks=cause_checks,
        Kairo_matched_methods=3287,Kairo_matched_IL=269894,Kairo_types_equal=337,divergent_bodies=0,held_methods=0,regressions=0,
        compiler_flags_references_unchanged=True,Root_review_required_before_Part_B=True)
    row['provenance'] = dict(before=snapshot_provenance(folder/'before'),
                             after=snapshot_provenance(folder/'after'),
                             producer_sha256=sha(Path(__file__)))
    journal['steps'].append(row)
    write(journal_path,journal)
    print(json.dumps({k:row[k] for k in ["label","raw_status","outside_exceptions_differing_bytes","recompiled_sha256","cause_checks","Kairo_matched_methods","Kairo_types_equal","regressions"]}))


if __name__ == '__main__': main()
