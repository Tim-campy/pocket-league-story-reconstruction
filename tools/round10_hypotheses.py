"""Record only measured round-eight outcomes; never changes reconstruction state."""
import json
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1]
def main():
    journal=ROOT/'reports/round10-cycle-journal.json'
    rows=json.loads(journal.read_text(encoding='utf-8'))['cycles'] if journal.exists() else []
    hypotheses=[{'status':'CONFIRMADA' if r['methods_gained'] else 'INCONCLUSIVA',
        'hypothesis':r['rule']+': '+r['hypothesis'],
        'evidence':r['label']+': '+str(r['methods_gained'])+' matched, '+str(r['il_bytes_gained'])+' IL bytes; '+r['result']} for r in rows]
    extra=ROOT/'reports/round10-additional-hypotheses.json'
    if extra.exists():hypotheses+=json.loads(extra.read_text(encoding='utf-8'))['hypotheses']
    (ROOT/'reports/round10-hypotheses.json').write_text(json.dumps({'hypotheses':hypotheses},indent=2,ensure_ascii=False)+'\n',encoding='utf-8',newline='\n')
if __name__=='__main__':main()
