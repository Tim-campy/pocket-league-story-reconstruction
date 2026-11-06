"""Remeasure the unchanged model on the final matched corpus, preserving earlier evidence."""
import hashlib,json,sys
from pathlib import Path
import round8_roslyn_validate as retained
ROOT=Path(__file__).resolve().parents[1]
def sha(path):return hashlib.sha256(path.read_bytes()).hexdigest()
class ReportDestination:
    """Keep every model/source input unchanged; redirect just the output JSON."""
    def __truediv__(self,key):
        actual=ROOT/key
        if actual==ROOT/'reports/round8-roslyn-model-validation.json':
            return ROOT/'reports/round8-final-roslyn-model-validation.json'
        return actual
def main():
    baseline=ROOT/'build/round8/final-model-corpus/KairoLibrary'
    frozen=ROOT/'reports/round8-roslyn-model-validation.json';old=sha(frozen)
    retained.ROOT=ReportDestination()
    sys.argv=[str(ROOT/'tools/round8_roslyn_validate.py'),'--baseline',str(baseline)]
    retained.main()
    if sha(frozen)!=old:raise ValueError('Earlier validation was unexpectedly changed')
    (ROOT/'reports/round8-final-model-measurement-producer.json').write_text(json.dumps({
        'operation':'same-validator-new-output-only','validator':'tools/round8_roslyn_validate.py',
        'validator_sha256':sha(ROOT/'tools/round8_roslyn_validate.py'),
        'earlier_validation_sha256_unchanged':old,'candidate_game_compilations':0,
        'final_corpus':str(baseline.relative_to(ROOT))},indent=2)+'\n',encoding='utf-8',newline='\n')
if __name__=='__main__':main()
