"""Archive two invalid screening experiments, preserving Git history and all method states.

Only zero-gain, zero-source-change experiments with the audited old screen
implementation qualify. Their receipts remain byte-identical in the archive.
The scientific cycles are then repeated with correctly bound candidate sources.
"""
import hashlib,json
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1]
def read(p):return json.loads(p.read_text(encoding='utf-8-sig'))
def sha(p):return hashlib.sha256(p.read_bytes()).hexdigest()
def main():
    journal_path=ROOT/'reports/round8-cycle-journal.json';old=read(journal_path)
    if [r['cycle'] for r in old['cycles']]!=[1,2,3,4,5]:raise ValueError('Expected precisely the two invalid final experiments')
    audit_path=ROOT/'reports/round8-screen-audit.json'
    if not audit_path.is_file():raise ValueError('Independent screen audit must be published first')
    ledger_paths=[ROOT/'config/methods.csv',ROOT/'config/types.csv']
    unchanged={str(p.relative_to(ROOT)):sha(p) for p in ledger_paths}
    for n in (4,5):
        label='cycle%02d'%n;row=old['cycles'][n-1]
        engine=read(ROOT/('reports/round8-'+label+'-KairoLibrary-engine.json'))
        adoption=read(ROOT/('reports/round8-'+label+'-adoption.json'))
        if row['methods_gained'] or row['il_bytes_gained'] or row['rule_accepted_steps']:raise ValueError('Not a zero-gain experiment')
        if engine['body_gains'] or engine['body_losses'] or any(m['steps'] for m in engine['methods']):raise ValueError('An accepted source step must not be archived')
        if adoption['changed_sources'] or adoption['matched_gains'] or adoption['matched_losses'] or not adoption['official_verified']:raise ValueError('Official source/state change found')
        trials=[t for m in engine['methods'] for t in m['trials']]
        if not trials or not all(t.get('incomplete_prediction') and t.get('screen_used') and not t.get('candidate_source_provenance') for t in trials):raise ValueError('Experiment does not match audited bug')
    archive=ROOT/'reports/round8-invalid-screen-original-journal.json'
    if archive.exists():raise FileExistsError('Never replace prior audit')
    archive.write_bytes(journal_path.read_bytes());moves=[]
    for n in (4,5):
        label='cycle%02d'%n
        for source in sorted((ROOT/'reports').glob('round8-'+label+'-*')):
            target=source.with_name(source.name.replace('round8-'+label+'-','round8-'+label+'-initial-screen-',1))
            if not source.resolve().is_relative_to(ROOT/'reports') or not target.resolve().is_relative_to(ROOT/'reports') or target.exists():raise ValueError('Unsafe or occupied report destination')
            digest=sha(source);source.rename(target);moves.append({'old':str(source.relative_to(ROOT)),'new':str(target.relative_to(ROOT)),'sha256':digest})
        for parent,name in [(ROOT/'build/round8/cycles',label),(ROOT/'build/round8/runs',label),(ROOT/'build/round8/runs',label+'-probe')]:
            source=(parent/name).resolve();target=(parent/(name+'-initial-screen')).resolve()
            if not source.is_relative_to(parent.resolve()) or not target.is_relative_to(parent.resolve()) or target.exists():raise ValueError('Unsafe or occupied run destination')
            if source.exists():source.rename(target);moves.append({'old':str(source.relative_to(ROOT)),'new':str(target.relative_to(ROOT)),'kind':'completed run, preserved'})
    current=dict(old,cycles=old['cycles'][:3]);current['consecutive_zero_cycles']=0
    for row in reversed(current['cycles']):
        if row['methods_gained']:break
        current['consecutive_zero_cycles']+=1
    current['superseded_experiments']={'journal_archive':str(archive.relative_to(ROOT)),'journal_archive_sha256':sha(archive),'audit':str(audit_path.relative_to(ROOT)),'reason':'Candidate not passed to sequential source screen; these experiments did not complete the required candidate test','public_commits_preserved':['4b0097f008e2b493239773aa2abe7e3d51c6fe86','e7d2462']}
    journal_path.write_text(json.dumps(current,indent=2,ensure_ascii=False)+'\n',encoding='utf-8',newline='\n')
    for p in ledger_paths:
        if sha(p)!=unchanged[str(p.relative_to(ROOT))]:raise ValueError('Ledger unexpectedly changed')
    result={'operation':'supersede-invalid-engine-experiments','states_manually_edited':False,'ledger_hashes_unchanged':unchanged,'archived_journal_sha256':sha(archive),'active_cycles':[1,2,3],'archive_moves':moves,'git_history_rewritten':False}
    (ROOT/'reports/round8-invalid-screen-recovery.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8',newline='\n');print(json.dumps({'archived':len(moves),'active_cycles':[1,2,3],'ledger_untouched':True}))
if __name__=='__main__':main()
