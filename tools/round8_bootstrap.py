"""Create round-8 orchestration without changing the retained round-7 tools."""
from pathlib import Path

ROOT=Path(__file__).resolve().parents[1]
BASE='25b8108711a4a8920a2600cf9ffea5b710b3a481'

def write(name,text):
    path=ROOT/'tools'/name
    if path.exists():raise FileExistsError(path)
    path.write_text(text,encoding='utf-8',newline='\n')

def main():
    for stem in ('cycle','adopt','commit','git_proof','journal','measure'):
        source=(ROOT/'tools'/('round7_'+stem+'.py')).read_text(encoding='utf-8-sig')
        source=source.replace('round7','round8').replace('round 7','round 8').replace('rodada 7','rodada 8')
        source=source.replace('cdd0f7f3b8dda09f296a7cfa16cd00236a3346ae',BASE)
        if stem=='cycle':source=source.replace("len(journal['cycles'])>=20","len(journal['cycles'])>=40")
        if stem=='journal':source=source.replace('cdd0f7f3b8dda09f296a7cfa16cd00236a3346ae',BASE)
        write('round8_'+stem+'.py',source)
    source=(ROOT/'tools/round7_freeze.py').read_text(encoding='utf-8-sig')
    source=source.replace('build/round7/baseline','build/round8/baseline').replace('cdd0f7f3b8dda09f296a7cfa16cd00236a3346ae',BASE)
    write('round8_freeze.py',source)
    source=(ROOT/'tools/round7_engine.py').read_text(encoding='utf-8-sig')
    source=source.replace('build/round7/','build/round8/').replace("'round7-'","'round8-'").replace('reports/round7-','reports/round8-')
    source=source.replace('from round7_transform import TransformClient','from round8_transform import TransformClient')
    write('round8_engine.py',source)
    source=(ROOT/'tools/round7_transform.py').read_text(encoding='utf-8-sig').replace('prepare_round7_transform','prepare_round8_transform')
    write('round8_transform.py',source)
    source=(ROOT/'tools/round7_roslyn_validate.py').read_text(encoding='utf-8-sig')
    source=source.replace('reports/round7-roslyn-model-validation','reports/round8-roslyn-base-validation')
    write('round8_roslyn_base_validate.py',source)

if __name__=='__main__':main()
