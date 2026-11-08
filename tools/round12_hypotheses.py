"""Record measured round-twelve outcomes; never changes reconstruction state."""
import json
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1]
def main():
    journal=ROOT/'reports/round12-cycle-journal.json'
    rows=json.loads(journal.read_text(encoding='utf-8'))['cycles'] if journal.exists() else []
    hypotheses=[]
    for r in rows:
        # A family gain proves an existential claim, not every candidate in it.
        hypotheses.append({'status':'CONFIRMADA' if r['methods_gained'] else 'INCONCLUSIVA',
            'hypothesis':'A família '+r['rule']+' produz ao menos um novo corpo integral.',
            'evidence':r['label']+': '+str(r['methods_gained'])+' matched, '+str(r['il_bytes_gained'])+
                ' IL bytes; '+r['result']+'. Proposta investigada: '+r['hypothesis']})
        engine_path=ROOT/'reports'/('round12-'+r['label']+'-'+r['assembly']+'-engine.json')
        if not engine_path.exists():continue
        engine=json.loads(engine_path.read_text(encoding='utf-8-sig'))
        if not engine.get('completed'):raise ValueError('Incomplete cycle evidence: '+r['label'])
        final_methods={}
        for passes in engine.get('previous_passes',{}).values():
            for method in passes:final_methods[method['id']]=method
        for method in engine.get('methods',[]):final_methods[method['id']]=method
        for identity,method in sorted(final_methods.items()):
            accepted=[t for t in method.get('trials',[]) if t.get('accepted')]
            if accepted and method.get('body_equal') is not True:
                hypotheses.append({'status':'REFUTADA',
                    'hypothesis':'O candidato parcial de '+r['label']+' reproduz integralmente '+identity+'.',
                    'evidence':str(len(accepted))+' avanços aceitos, mas corpo integral não igual no fim desse ciclo; parada '+
                        str(method.get('stop_reason','UNKNOWN'))+'. A diferença exata de corpo refuta esta afirmação de identidade integral; não implica diagnóstico de erro semântico. Ganhos de outros métodos da família não confirmam este candidato.'})
    extra=ROOT/'reports/round12-additional-hypotheses.json'
    if extra.exists():hypotheses+=json.loads(extra.read_text(encoding='utf-8'))['hypotheses']
    (ROOT/'reports/round12-hypotheses.json').write_text(json.dumps({'hypotheses':hypotheses},indent=2,ensure_ascii=False)+'\n',encoding='utf-8',newline='\n')
if __name__=='__main__':main()
