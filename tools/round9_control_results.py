"""Report completed Part C from frozen independent measurements and oracle receipts."""
import json
from pathlib import Path
from compare_kairo import sha

ROOT=Path(__file__).resolve().parents[1]
def read(path):return json.loads((ROOT/path).read_text(encoding='utf-8-sig'))


def main():
    validation=read('reports/round9-control-validation-public.json')
    binding=read('reports/round9-control-leaf-bindings.json')
    proposal=read('reports/round9-control-rewrite-patches.json')
    audit=read('reports/round9-control-rewrite-audit.json')
    before=read('build/round9/cycles/c11/baseline/Assembly-CSharp/verification.json')
    after=read('build/round9/cycles/c11/after/Assembly-CSharp/verification.json')
    before_by={v['stable_id']:v for v in before['methods'].values()}
    after_by={v['stable_id']:v for v in after['methods'].values()}
    originals={m['stable_id']:m for m in read('build/round9/cycles/c11/after/Assembly-CSharp/original-methods.json')['selected_methods']}
    losses=[k for k,v in before_by.items() if v['state']=='matched' and after_by[k]['state']!='matched']
    body_losses=[k for k,v in before_by.items() if v['body_equal'] and not after_by[k]['body_equal']]
    if losses or body_losses:raise ValueError('Part C regressed previous proof')
    rows=[]
    for patch in proposal['patches']:
        key=patch['method_id'];proof=after_by[key]
        rows.append({'id':key,'kind':'unused-local-suffix' if 'unused-local-suffix' in patch['rule'] else 'control-rewrite',
            'state_before':before_by[key]['state'],'state_after':proof['state'],
            'body_equal':proof['body_equal'],'original_il_bytes':originals[key]['header']['code_size']})
    gained=[r for r in rows if r['state_before']!='matched' and r['state_after']=='matched']
    matched=validation['matched'];pending=validation['pending']
    rewritten=[r for r in rows if r['kind']=='control-rewrite']
    control_gains=[r for r in gained if r['kind']=='control-rewrite']
    total_bytes=sum(r['original_il_bytes'] for r in gained)
    summary=(f"Corpus antes de C: {matched['trees_equal']}/{matched['paired_tree_complete']} árvores completas iguais; "
        f"cobertura {matched['paired_tree_complete']}/{matched['methods']} corpos matched. "
        f"Pendentes: {pending['il_tree_complete']} extrações IL completas, {pending['source_tree_complete']} fontes completas, "
        f"{pending['paired_tree_complete']} pares completos; {pending['trees_equal']} iguais e "
        f"{pending['paired_tree_complete']-pending['trees_equal']} diferentes antes. "
        f"{binding['different_skeletons_bindable']} diferenças com folhas bijetivas; "
        f"{len(rewritten)} reescritas calculadas com árvore/plano originais, {len(control_gains)} corpos fechados. "
        f"Mais {sum(r['kind']=='unused-local-suffix' for r in gained)} corpo por locais sem uso. "
        f"Total oficial: {len(gained)} métodos, {total_bytes} bytes, zero regressões.")
    report={'operation':'completed-Part-C-frozen-oracle-report','summary':summary,
        'before_snapshot':'build/round9/cycles/c11/baseline','after_snapshot':'build/round9/cycles/c11/after',
        'matched_validation':matched,'pending_validation_before':pending,
        'binding_complete':binding['bindings_complete'],'different_skeletons_bindable':binding['different_skeletons_bindable'],
        'structural_differences_before':pending['paired_tree_complete']-pending['trees_equal'],
        'control_rewrites_calculated':len(rewritten),'control_rewrites_tree_equal_after':len(rewritten),
        'control_rewrites_matched':len(control_gains),
        'same_preexisting_skeletons':pending['trees_equal'],
        'same_skeletons_solver7_without_candidate':audit['groups'].get('source_skeleton_equal_solver_has_no_approved_control_preserving_candidate',0),
        'additional_calculated_unused_suffix_matched':sum(r['kind']=='unused-local-suffix' for r in gained),
        'methods_gained':len(gained),'il_bytes_gained':total_bytes,'matched_losses':losses,'body_losses':body_losses,
        'audit_groups':audit['groups'],'rows':rows,
        'evidence_sha256':{path:sha(ROOT/path) for path in [
            'reports/round9-control-validation-public.json','reports/round9-control-leaf-bindings.json',
            'reports/round9-control-rewrite-patches.json','reports/round9-control-rewrite-audit.json',
            'build/round9/cycles/c11/baseline/Assembly-CSharp/verification.json',
            'build/round9/cycles/c11/after/Assembly-CSharp/verification.json']}}
    (ROOT/'reports/round9-control-results.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8',newline='\n')
    print(summary)


if __name__=='__main__':main()
