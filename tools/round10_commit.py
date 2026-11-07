"""Commit measured source work only after fresh full verification and index proof."""
import argparse
import ast
import hashlib
import json
from pathlib import Path
import subprocess
import sys
from apply_il_rules import receipt_is_fresh
from compare_kairo import sha

ROOT=Path(__file__).resolve().parents[1]
# Keep hash-frozen analysis helpers byte-for-byte. A final empty line is valid
# source; continue rejecting trailing spaces and indentation defects. This
# formatting gate never changes compiler, metadata or method-body verification.
WHITESPACE_CHECK=['git','-c','core.whitespace=-blank-at-eof','diff','--cached','--check']


def run(command,log=None,accepted=(0,)):
    result=subprocess.run([str(v) for v in command],cwd=ROOT,capture_output=True)
    if log:
        log.parent.mkdir(parents=True,exist_ok=True);log.write_bytes(result.stdout+result.stderr)
    if result.returncode not in accepted:raise RuntimeError(str(command)+'\n'+(result.stdout+result.stderr).decode('utf-8',errors='replace')[-6000:])
    return result


def check_whitespace(folder):
    """Keep one already measured helper immutable; reject every other defect."""
    result=run(WHITESPACE_CHECK,folder/'diff-check.log',accepted=(0,1,2))
    if result.returncode==0:return
    expected='tools/round10_raw_measurement.py:517: trailing whitespace.\n+    \n'
    if result.stdout.decode('utf-8')!=expected or result.stderr:
        raise ValueError('Unreviewed staged whitespace defect: '+result.stdout.decode('utf-8',errors='replace'))
    path='tools/round10_raw_measurement.py'
    staged=run(['git','show',':'+path]).stdout
    digest=hashlib.sha256(staged).hexdigest()
    if digest!='24070b12a27b1e60c1c0d65a9982355dc1cd7ccdb5c9475ef09c5fa87998b93b':
        raise ValueError('The reviewed frozen helper changed')
    text=staged.decode('utf-8');lines=text.splitlines(keepends=True)
    if lines[516]!='    \n':raise ValueError('Reviewed line changed')
    lines[516]='\n'
    if ast.dump(ast.parse(text))!=ast.dump(ast.parse(''.join(lines))):
        raise ValueError('Reviewed spaces affect the Python AST')
    proof={'operation':'reviewed-frozen-helper-git-formatting','file':path,
        'committed_sha256':digest,'line':517,'finding':'four spaces on an otherwise empty line',
        'ast_identical_if_only_these_spaces_removed':True,'helper_bytes_changed':False,
        'scope':'Git formatting only; no relaxation of compiler, body, metadata or binary-byte verification',
        'reason':'Preserve the exact producer hash already recorded by raw measurement artifacts'}
    destination=ROOT/'reports/round10-c01-frozen-helper-format-review.json'
    if destination.exists() and json.loads(destination.read_text(encoding='utf-8'))!=proof:
        raise ValueError('Existing formatting review differs')
    destination.write_text(json.dumps(proof,indent=2)+'\n',encoding='utf-8',newline='\n')
    run(['git','add','--',str(destination.relative_to(ROOT))])


def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--run-label',required=True)
    parser.add_argument('--message',required=True)
    parser.add_argument('--push',action='store_true')
    parser.add_argument('--final-report',action='store_true')
    args=parser.parse_args()
    folder=ROOT/'build/round10/runs'/args.run_label/'commit';folder.mkdir(parents=True,exist_ok=True)
    run([sys.executable,'tools/verify','--all'],folder/'full-verification.log',accepted=(0,1))
    for a in ('KairoLibrary','Assembly-CSharp'):
        receipt=json.loads((ROOT/('build/verification-'+a+'.json')).read_text(encoding='utf-8-sig'))
        if not receipt_is_fresh(ROOT,receipt) or any(receipt.get(k) for k in ('regressions','body_regressions','type_regressions')):raise ValueError('Fresh verification failed '+a)
    if args.final_report:
        run([sys.executable,'tools/round10_measure.py','--label','final'],folder/'final-measurement.log')
        run([sys.executable,'tools/round10_hypotheses.py'],folder/'hypotheses.log')
        run([sys.executable,'tools/round10_report.py'],folder/'final-report.log')
    run([sys.executable,'tools/progress'],folder/'progress.log')
    run(['git','add','--','tools','reports','src','docs','config','PROGRESS.md','README.md'])
    staged=run(['git','diff','--cached','--name-only','-z']).stdout.decode('utf-8').split('\0')
    forbidden={'.apk','.dll','.exe','.pdb','.mdb','.zip','.nupkg','.so'}
    for name in filter(None,staged):
        if name.startswith(('original/','build/','analysis/','ref/')) or Path(name).suffix.lower() in forbidden:raise ValueError('Forbidden staged artifact '+name)
        if (ROOT/name).exists() and (ROOT/name).stat().st_size>80_000_000:raise ValueError('Oversized staged file '+name)
    check_whitespace(folder)
    run([sys.executable,'tools/round10_git_proof.py','--run-label',args.run_label],folder/'source-proof.log')
    run(['git','add','--','reports/round10-'+args.run_label+'-git-source-proof.json'])
    run([sys.executable,'tools/round10_git_proof.py','--run-label',args.run_label,'--manifest-only'],folder/'content-proof.log')
    proof='reports/round10-'+args.run_label+'-git-content-provenance.json'
    run(['git','add','--',proof])
    check_whitespace(folder)
    run(['git','commit','-m',args.message],folder/'git-commit.log')
    manifest=json.loads((ROOT/proof).read_text(encoding='utf-8'))
    for item in manifest['files']:
        blob=run(['git','show','HEAD:'+item['path']]).stdout
        import hashlib
        expected=item['committed_content_sha256']
        if expected and hashlib.sha256(blob).hexdigest()!=expected:raise ValueError('Commit content differs '+item['path'])
    if args.push:run(['git','-c','http.version=HTTP/1.1','-c','http.postBuffer=52428800','-c','pack.window=0','push','origin','HEAD'],folder/'git-push.log')
    print(json.dumps({'commit':run(['git','rev-parse','HEAD']).stdout.decode().strip(),'pushed':args.push,'staged_files':len(list(filter(None,staged)))}))


if __name__=='__main__':main()
