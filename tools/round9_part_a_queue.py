"""Consume the remaining existing queue serially; never starts B/C or an autonomous cycle."""
import json,subprocess,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1]
def main():
    cfg=json.loads((ROOT/'config/round9-protocol.json').read_text(encoding='utf-8'))
    if any(r['status']=='pending' for r in cfg['prepared_queue'][:2]):raise ValueError('GetCode and DrawArc must finish first')
    tasks=[
        ('KairoLibrary','nullable-coalesce-negative-ternary','brfalse.s=>brtrue.s',
         'FilaA: ternario negativo preserva orientacao call-first e callvirt do original; teste do candidato preparado sem normalizacao.',
         ['kairo.unity.util.StreamUtil::WriteArray(java.io.OutputStream,System.Array,System.Int32)->'],
         ['null-negative-WriteArray'],None),
        ('Assembly-CSharp','calculated-coordinate-local-coalescence','stloc.s=>stloc.s',
         'FilaA: coalescencia de coordenadas e escopos calculados preservam inicializadores e igualam assinaturas originais.',
         ['form.DebugForm::Draw_coatEva(kairo.unity.ui.Graphics)->','game.Ball::SetSavePos(kairo.unity.m3d4.util3d.Vector3D,kairo.unity.m3d4.util3d.Vector3D,kairo.unity.m3d4.util3d.Vector3D)->','form.DebugForm::Draw_moveArea(kairo.unity.ui.Graphics,System.Int32,System.Int32)->'],
         ['coordinates-Draw_coatEva','coordinates-SetSavePos','coordinates-Draw_moveArea'],'reports/round8-coordinate-slot-patches.json'),
        ('Assembly-CSharp','expand-compound-field-multiply','ldarg.0=>dup',
         'FilaA: *= em campos float ou double de receiver estavel vira atribuicao explicita com recarga do receiver, sem mudar ordem ou agrupar RHS.',
         ['game.Ball::Update()->'],['compound-field-multiply'],None)]
    for assembly,rule,pair,hypothesis,targets,queue_ids,external in tasks:
        cfg=json.loads((ROOT/'config/round9-protocol.json').read_text(encoding='utf-8'))
        if all(r['status']!='pending' for r in cfg['prepared_queue'] if r['id'] in queue_ids):continue
        journal=json.loads((ROOT/'reports/round9-cycle-journal.json').read_text(encoding='utf-8'))
        number=len(journal['cycles'])+1
        args=[sys.executable,'tools/round9_cycle.py','--number',str(number),'--phase','A','--assembly',assembly,'--rule',rule,'--pair',pair,'--hypothesis',hypothesis,'--push']
        for target in targets:
            args+=['--probe-substring',target]
            if rule!='expand-compound-field-multiply':args+=['--target-substring',target]
        for item in queue_ids:args+=['--queue-id',item]
        if external:args+=['--external-proposals',external]
        print(json.dumps({'starting_part_a_step':number,'rule':rule}),flush=True)
        result=subprocess.run(args,cwd=ROOT)
        if result.returncode:raise RuntimeError('Part A step failed; no later work started')
    cfg=json.loads((ROOT/'config/round9-protocol.json').read_text(encoding='utf-8'))
    if any(r['status']=='pending' for r in cfg['prepared_queue']):raise ValueError('Prepared queue not empty')
    print(json.dumps({'part_a_queue_empty':True}),flush=True)
if __name__=='__main__':main()
