"""Derive Part-C results from measured extractor and official adoption receipts."""
import json
from pathlib import Path
from compare_kairo import sha

ROOT=Path(__file__).resolve().parents[1]
def read(name):return json.loads((ROOT/name).read_text(encoding='utf-8-sig'))


def main():
    summary=read('reports/round10-control-model-summary.json')
    audit=read('reports/round10-control-application-audit.json')
    adoption=read('reports/round10-c01-adoption.json')
    checks=read('reports/round10-control-application-checks.json')['measurements']
    journal=read('reports/round10-cycle-journal.json')
    cycle=next(c for c in journal['cycles'] if c['label']=='c01')
    assert cycle['phase']=='C' and cycle['methods_gained']==1 and cycle['il_bytes_gained']==717
    assert adoption['official_verified'] and adoption['matched_losses']==0 and adoption['matched_gains']==1
    for r in summary['stages']:
        assert sha(ROOT/r['report'])==r['report_sha256']
        assert r['valid_frozen_input_measurement']
        assert r['groups']['matched']['wrong_complete_plans']==0
        assert r['groups']['matched']['different_complete_trees']==0
    proposals=read('reports/round10-control-rewrite-patches.json')['patches']
    assert len(proposals)==1 and proposals[0]['control_tree_equals_original'] and proposals[0]['control_plan_equals_original']
    inputs=['reports/round10-control-model-summary.json','reports/round10-control-application-audit.json',
            'reports/round10-control-rewrite-patches.json','reports/round10-c01-adoption.json',
            'reports/round10-c01-before-measurement.json','reports/round10-c01-after-measurement.json',
            'reports/round10-c01-git-source-proof.json','reports/round10-c01-git-content-provenance.json',
            'reports/round10-control-application-checks.json']
    final_pending=summary['stages'][-1]['groups']['pending']
    roundtrip=checks['roundtrip']
    result={'operation':'measured-round10-part-C-results','status':'COMPLETE_WITH_DOCUMENTED_EXTRACTION_LIMITS',
       'baseline_matched_bodies':summary['baseline']['matched']['methods'],
       'baseline_pending_bodies_including_held':summary['baseline']['pending']['methods'],
       'baseline':summary['baseline'],'stages':summary['stages'],'rejected_attempts':summary['rejected_attempts'],
       'application':{'audited_pending':audit['pending_bodies'],'groups':audit['groups'],
           'paired_complete_before':final_pending['paired_tree_complete'],
           'trees_equal_before':final_pending['trees_equal'],
           'trees_different_before':final_pending['paired_tree_complete']-final_pending['trees_equal'],
           'rewritten_methods':1,'candidate_tree_and_plan_equal_original':1,'body_matches_officially_adopted':1,
           'matched_gains':1,'original_il_bytes_gained':717,'matched_losses':0,
           'target_id':proposals[0]['method_id'],
           'complete_provenance_projections':checks['projection']['counts']['projected'],
           'projection_disagreements':len(checks['projection']['failures']),
           'renderer_roundtrip_exact':roundtrip['exact_AST_tree_and_plan'],
           'renderer_roundtrip_tested':len(roundtrip['methods']),
           'renderer_negative':'ReturnVoidFinally: protected void footer differs; independent AST gate refuses proposal'},
       'target_state_writer':'tools/verify only','input_sha256':{p:sha(ROOT/p) for p in inputs},
       'producer_sha256':sha(Path(__file__)),'limits':summary['limits']}
    output=ROOT/'reports/round10-control-results.json';assert not output.exists()
    output.write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({'report':str(output.relative_to(ROOT)),'matched_gains':1,'il_bytes_gained':717,
                     'matched_losses':0,'pending_audited':audit['pending_bodies']}))


if __name__=='__main__':main()
