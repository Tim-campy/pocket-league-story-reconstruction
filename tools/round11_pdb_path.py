"""Measure the compiler-produced historical CodeView path; no assembly edits."""
import json
from pathlib import Path
import subprocess
import sys
from compare_kairo import sha,differences
from apply_il_rules import receipt_is_fresh
from pe_codeview import inspect_codeview
from kairo_candidate import BUILD_FOLDER

ROOT=Path(__file__).resolve().parents[1]
BASE=ROOT/'build/round11/baseline'


def read(path):return json.loads(path.read_text(encoding='utf-8-sig'))
def delta(a,b):
    a=bytes.fromhex(a);b=bytes.fromhex(b)
    return sum(x!=y for x,y in zip(a,b))+abs(len(a)-len(b))


def main():
    original=ROOT/'original/extracted/assets/bin/Data/Managed/KairoLibrary.dll'
    snapshot=ROOT/'build/round11/pdb-output-proof'
    if snapshot.exists():raise FileExistsError('Historical PDB proof snapshot is immutable')
    subprocess.run([sys.executable,'-X','utf8','tools/round11_freeze.py','--destination',str(snapshot)],cwd=ROOT,check=True)
    before=BASE/'assemblies/KairoLibrary.dll';after=snapshot/'assemblies/KairoLibrary.dll'
    cfg=read(ROOT/'config/kairo-pdb-output.json')
    records={key:inspect_codeview(path) for key,path in [('original',original),('before',before),('after',after)]}
    a,b,c=(records[key]['codeview'] for key in ('original','before','after'))
    assert a['pdb_path']==cfg['original_pdb_path']==c['pdb_path']
    assert a['record_excluding_guid_hex']==c['record_excluding_guid_hex']
    manifest=read(snapshot/'KairoLibrary/build-manifest.json')
    assert manifest['output_sha256']==sha(after)
    assert '/pdb:'+cfg['original_pdb_path'] in manifest['flags']
    assert sha(Path(manifest['pdb_output']['analysis_mirror']))==manifest['pdb_output']['pdb_sha256']
    proof={}
    for assembly in ('KairoLibrary','Assembly-CSharp'):
        receipt=read(snapshot/assembly/'verification.json')
        old_receipt=read(BASE/assembly/'verification.json')
        assert receipt_is_fresh(ROOT,receipt)
        assert not any(receipt.get(k) for k in ('regressions','body_regressions','type_regressions'))
        prior=read(BASE/assembly/'rebuilt-methods.json')['selected_methods']
        current=read(snapshot/assembly/'rebuilt-methods.json')['selected_methods']
        index={m['stable_id']:m for m in current}
        assert len(index)==len(current)==len(prior)
        changed=[m['stable_id'] for m in prior if differences(m,index.get(m['stable_id']))[0]]
        assert not changed
        assert {k:v['state'] for k,v in old_receipt['methods'].items()}=={k:v['state'] for k,v in receipt['methods'].items()}
        proof[assembly]={'changed_methods_vs_baseline':changed,'counts':receipt['counts'],
            'types_equal':receipt['types_equal'],'types_total':receipt['types_total'],
            'verification_receipt_sha256':sha(snapshot/assembly/'verification.json')}
    report={'operation':'compiler-produced-historical-pdb-path','baseline_commit':read(BASE/'snapshot.json')['commit'],
        'records':records,'original_pdb_path':a['pdb_path'],
        'before_path_and_padding_different_bytes':delta(a['path_and_padding_hex'],b['path_and_padding_hex']),
        'after_path_and_padding_different_bytes':delta(a['path_and_padding_hex'],c['path_and_padding_hex']),
        'after_codeview_excluding_only_proven_guid_equal':True,
        'input_snapshot':str(snapshot.relative_to(ROOT)),
        'build_manifest_sha256':sha(snapshot/'KairoLibrary/build-manifest.json'),
        'pdb_output':manifest['pdb_output'],'assembly_bytes_rewritten_after_compilation':False,
        'method_gains':0,'method_losses':0,'full_verification':proof,
        'rea_artifact_evidence':'reports/round11-KairoLibrary-rea-artifact.json',
        'producer_sha256':sha(Path(__file__)),'reader_sha256':sha(ROOT/'tools/pe_codeview.py')}
    out=ROOT/'reports/round11-pdb-path.json'
    with out.open('x',encoding='utf-8',newline='\n') as stream:json.dump(report,stream,indent=2,ensure_ascii=False);stream.write('\n')
    print(json.dumps({k:report[k] for k in ('original_pdb_path','before_path_and_padding_different_bytes','after_path_and_padding_different_bytes','method_gains','method_losses')}))


if __name__=='__main__':main()
