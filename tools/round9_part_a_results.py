"""Summarize the prepared queue from frozen official receipts, not manually assigned states."""
import json
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1]
def read(p):return json.loads((ROOT/p).read_text(encoding='utf-8-sig'))
def main():
    folder='build/round9/cycles/a05/after'
    specs=[('KairoLibrary','StringUtil::GetCode(', 'a01'),('KairoLibrary','Graphics::DrawArc(', 'a02'),
        ('KairoLibrary','StreamUtil::WriteArray(java.io.OutputStream,System.Array,System.Int32)', 'a03'),
        ('Assembly-CSharp','DebugForm::Draw_coatEva(', 'a04'),('Assembly-CSharp','Ball::SetSavePos(', 'a04'),
        ('Assembly-CSharp','DebugForm::Draw_moveArea(', 'a04'),('Assembly-CSharp','Ball::Update()', 'a05')]
    rows=[]
    for assembly,needle,label in specs:
        original=read(folder+'/'+assembly+'/original-methods.json')['selected_methods']
        selected=[m for m in original if needle in m['stable_id']]
        if len(selected)!=1:raise ValueError('Ambiguous prepared target')
        method=selected[0];identity=method['stable_id']
        proof=next(p for p in read(folder+'/'+assembly+'/verification.json')['methods'].values() if p['stable_id']==identity)
        engine=read('reports/round9-'+label+'-'+assembly+'-engine.json')
        search=next(m for m in engine['methods'] if m['id']==identity)
        rows.append({'assembly':assembly,'method_id':identity,'original_il_bytes':method['header']['code_size'],
            'official_state':proof['state'],'body_equal':proof.get('body_equal'),
            'initial_prefix':search['initial_offset'],'final_prefix':search['final_offset'],
            'stop_reason':search['stop_reason'],'candidate_variants_compiled':len(search['trials']),
            'accepted_rules':[s['rule'] for s in search['steps']],'run':label})
    cfg=read('config/round9-protocol.json')
    if any(q['status']=='pending' for q in cfg['prepared_queue']):raise ValueError('Queue not empty')
    result={'operation':'part-A-frozen-official-summary','frozen_after':folder,'queue_empty':True,
        'methods_newly_matched':sum(r['official_state']=='matched' for r in rows),
        'il_bytes_newly_matched':sum(r['original_il_bytes'] for r in rows if r['official_state']=='matched'),
        'regressions':0,'rows':rows}
    for assembly in ('KairoLibrary','Assembly-CSharp'):
        p=read(folder+'/'+assembly+'/verification.json')
        if any(p.get(k) for k in ('regressions','body_regressions','type_regressions')):raise ValueError('Regressive A receipt')
    (ROOT/'reports/round9-part-a-results.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8',newline='\n')
    print(json.dumps({k:v for k,v in result.items() if k!='rows'}))
if __name__=='__main__':main()
