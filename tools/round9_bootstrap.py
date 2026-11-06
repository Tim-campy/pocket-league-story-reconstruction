"""New round namespace; retained round-eight evidence and code remain immutable."""
import json
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1]
BASE='c51d204f57a17d5fa025d3b4ed629dfc8ab7ac64'
OLD='25b8108711a4a8920a2600cf9ffea5b710b3a481'
def main():
    for stem in ('cycle','adopt','commit','git_proof','journal','measure','freeze','engine','transform','model_policy','candidate_sources','clean_added_whitespace','hypotheses'):
        source=(ROOT/'tools'/('round8_'+stem+'.py')).read_text(encoding='utf-8-sig')
        source=source.replace('round8','round9').replace('round 8','round 9').replace('rodada 8','rodada 9').replace(OLD,BASE)
        if stem=='engine':
            source=source.replace("('round7-cycle-journal.json','round9-cycle-journal.json')","('round7-cycle-journal.json','round8-cycle-journal.json','round9-cycle-journal.json')")
            source=source.replace('round9_expand_compound_fields=is_smcs','round8_expand_compound_fields=is_smcs')
        path=ROOT/'tools'/('round9_'+stem+'.py')
        if path.exists():raise FileExistsError(path)
        path.write_text(source,encoding='utf-8',newline='\n')
    source=(ROOT/'tools/prepare_round8_transform.py').read_text(encoding='utf-8-sig')
    source=source.replace('round8-transform','round9-transform').replace('Round8TransformSyntax','Round9TransformSyntax')
    source=source.replace("ROOT/'tools/Round8Rules.cs',ROOT/'tools/Round8CompoundRules.cs'","ROOT/'tools/Round9Rules.cs',ROOT/'tools/Round8CompoundRules.cs',ROOT/'tools/Round8NullConditionalPolarityRules.cs'")
    source=source.replace('AddRound8Rules(cycleRequest','AddRound9Rules(cycleRequest')
    (ROOT/'tools/prepare_round9_transform.py').write_text(source,encoding='utf-8',newline='\n')
    source=(ROOT/'tools/Round8Rules.cs').read_text(encoding='utf-8-sig').replace('AddRound8Rules','AddRound9Rules')
    source=source.replace('AddRound8CompoundRules(input,text,mapped,methodBody,model,point,patches,seen);','AddRound8CompoundRules(input,text,mapped,methodBody,model,point,patches,seen);\n        AddRound8NullConditionalPolarityRules(input,text,mapped,methodBody,model,point,patches,seen);')
    (ROOT/'tools/Round9Rules.cs').write_text(source,encoding='utf-8',newline='\n')
    cfg=json.loads((ROOT/'config/round8-protocol.json').read_text(encoding='utf-8'))
    cfg.update(baseline_commit=BASE,consecutive_zero_stop=5,maximum_cycles=40,
        phase='A',autonomous_started=False,maximum_consecutive_primary_same_method=2,
        stop_counter_requires_empty_prepared_queue=True)
    cfg.pop('final_model_measurements',None)
    cfg['prepared_queue']=[{'id':v,'status':'pending'} for v in ['lifetime-GetCode','lifetime-DrawArc','null-negative-WriteArray','coordinates-Draw_coatEva','coordinates-SetSavePos','coordinates-Draw_moveArea','compound-field-multiply']]
    (ROOT/'config/round9-protocol.json').write_text(json.dumps(cfg,indent=2)+'\n',encoding='utf-8',newline='\n')
if __name__=='__main__':main()
