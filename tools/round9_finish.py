"""Finish reporting only after an explicit user-defined autonomous stop.

Runs no reconstruction transformations. Source, IL and metadata proof remain
with the ordinary verifier; final models/extractor measurements are read-only.
"""
import argparse
import hashlib
from concurrent.futures import ThreadPoolExecutor
import json
from pathlib import Path
import subprocess
import sys

ROOT=Path(__file__).resolve().parents[1]


def read(path):return json.loads((ROOT/path).read_text(encoding='utf-8-sig'))
def run(arguments,label,accepted=(0,)):
    folder=ROOT/'build/round9/final-report';folder.mkdir(parents=True,exist_ok=True)
    result=subprocess.run([sys.executable,'-X','utf8',*arguments],cwd=ROOT,capture_output=True)
    (folder/(label+'.log')).write_bytes(result.stdout+result.stderr)
    if result.returncode not in accepted:
        raise RuntimeError(label+' failed: '+(result.stdout+result.stderr).decode('utf-8',errors='replace')[-5000:])
    return result


def validate_final_control(path,corpus,expected_matched):
    document=read(path)
    if document.get('valid_frozen_input_measurement') is not True or document.get('inputs_changed_during_run'):
        raise ValueError('Final control extraction changed inputs or is invalid')
    if document.get('own_IL_or_MDB_used_to_infer_AST_control') is not False:
        raise ValueError('Final AST control independence is not proved')
    if (ROOT/document['snapshot']).resolve()!=(ROOT/corpus).resolve():
        raise ValueError('Final control extraction describes another snapshot')
    checks={str(Path(corpus)/'methods.csv'):document['ledger_sha256'],
            document['method_irs']:document['method_irs_sha256'],
            str(Path(document['method_irs']).parent/'source-manifest.json'):document['source_manifest_sha256'],
            **document['extractor_inputs']}
    for name,digest in checks.items():
        if hashlib.sha256((ROOT/name).read_bytes()).hexdigest()!=digest:
            raise ValueError('Stale final control evidence: '+name)
    if document['matched']['methods']!=expected_matched:
        raise ValueError('Final control and model matched denominators disagree')
    return document


def main():
    p=argparse.ArgumentParser(description=__doc__)
    p.add_argument('--recommendations',required=True,type=Path)
    p.add_argument('--push',action='store_true')
    args=p.parse_args()
    cfg=read('config/round9-protocol.json');journal=read('reports/round9-cycle-journal.json')
    autonomous=[r for r in journal['cycles'] if r.get('phase')=='D']
    pending=[q['id'] for q in cfg['prepared_queue'] if q['status']=='pending']
    if len(autonomous)>=cfg['maximum_cycles']:
        reason=f"Limite de {cfg['maximum_cycles']} ciclos autônomos concluídos"
    elif not pending and journal['consecutive_zero_cycles']>=cfg['consecutive_zero_stop']:
        reason=f"{journal['consecutive_zero_cycles']} ciclos D consecutivos sem novo matched, com a fila preparada vazia"
    else:
        raise ValueError('No measured normal stop condition; continue the authorized round')
    recommendations=json.loads(args.recommendations.read_text(encoding='utf-8-sig'))
    if not isinstance(recommendations,list) or not 2<=len(recommendations)<=3:
        raise ValueError('Provide two or three evidence-based recommendations')
    run(['tools/verify','--all'],'full-verification',accepted=(0,1))
    corpus='build/round9/final-corpus'
    run(['tools/round9_freeze.py','--destination',corpus],'freeze-final')
    jobs=[(['tools/round7_slot_validate.py','--baseline',corpus+'/Assembly-CSharp',
            '--output','reports/round9-final-smcs-model-validation.json'],'smcs-final'),
          (['tools/round9_roslyn_validate.py','--baseline',corpus+'/KairoLibrary',
            '--output','reports/round9-final-roslyn-model-validation.json'],'roslyn-final'),
          (['tools/round9_control_validate.py','--snapshot',corpus,'--label','final'],'control-final')]
    with ThreadPoolExecutor(max_workers=3) as pool:
        futures=[pool.submit(run,arguments,label) for arguments,label in jobs]
        for future in futures:future.result()
    smcs_validation=read('reports/round9-final-smcs-model-validation.json')
    validate_final_control('reports/round9-control-validation-final.json',corpus,smcs_validation['matched_bodies'])
    cfg['final_model_measurements']={
        'Assembly-CSharp':{'validation':'reports/round9-final-smcs-model-validation.json','corpus_baseline':corpus+'/Assembly-CSharp'},
        'KairoLibrary':{'validation':'reports/round9-final-roslyn-model-validation.json','corpus_baseline':corpus+'/KairoLibrary'}}
    cfg['control_extractor_validation']='reports/round9-control-validation-final.json'
    cfg.update(phase='stopped',autonomous_started=True)
    (ROOT/'config/round9-protocol.json').write_text(json.dumps(cfg,indent=2)+'\n',encoding='utf-8',newline='\n')
    stop={'operation':'measured-round9-stop','new_reconstruction_work_stopped':True,
        'reason':reason,'cycles_completed':len(journal['cycles']),
        'autonomous_cycles_completed':len(autonomous),
        'consecutive_zero_D_cycles':journal['consecutive_zero_cycles'],
        'prepared_pending_at_stop':pending,
        'blockers':['Nenhuma instalação, arquivo ou decisão de escopo exigida para esta rodada. Métodos restantes continuam divergentes; a parada segue o limite autorizado.'],
        'recommendations':recommendations,
        'preparation_notes':[
            'Dois primeiros ensaios de extração AST falharam por profundidade JSON64; o limite do serializador foi corrigido para2048. Uma tentativa posterior falhou no vínculo de IDs aninhados, corrigido pelo mapa oficial do receipt. Nenhum desses ensaios mudou fontes do jogo.',
            'As primeiras medições de desenvolvimento com helpers alterados durante a execução foram marcadas inválidas; os resultados publicados e finais usam hashes congelados e completude explícita.',
            'O commit B07 foi retido quando um arquivo de análise mudou depois do stage; a prova completa foi refeita. O localizador B09 exigiu assinatura completa para resolver uma sobrecarga.',
            'Um EOF vazio de helper congelado motivou permitir somente blank-at-eof no lint de estilo; o verificador de métodos/metadados não mudou. D01 revelou trivia final em duas linhas geradas: a limpeza protege literais e preserva finais de linha, passou quatro controles e exigiu nova prova completa.',
            'O planner inicial não vinculava / no stable_id a + na chave oficial de um parâmetro aninhado. Foi corrigido usando exclusivamente o mapa stable_id→chave do receipt, sem renomear assinaturas nem editar estados.']}
    stop['preparation_notes'].extend(['D29: a API pura v1 aceitou e removeu SideEffects++ em um controle de texto; foi endurecida por igualdade de todos os tokens antes de compilar candidatos do jogo. O controle foi recusado depois da corre??o; n?o houve regress?o adotada.', 'D33 e D34: nomes de diret?rios dos runners foram corrigidos antes de qualquer compila??o. O gravador do checkpoint e uma leitura de handoff falharam com cp1252 e foram corrigidos para UTF-8 antes de escrever dados. Nenhuma fonte, estado ou candidato foi afetado. Nenhum controle C# de D30 a D40 falhou.', 'Os WIPs congelados conservam NOT_TESTED mesmo quando a fam?lia foi testada depois; os relat?rios p?blicos, recibos e di?rio s?o a autoridade. Ao parar, SetWinCharaTraining ficou somente como API/templates n?o testados; CGI_COMMON_QUERY ficou sem rota de mapeamento e sem teste. O diagn?stico de _draw4 n?o foi for?ado a uma API integral.', 'Manuten??o nativa de armazenamento em JSONs privados arquivados preservou conte?do e tamanho, com prova SHA antes/depois. N?o alterou original, refer?ncias, fontes p?blicas ou bin?rios; seus bytes f?sicos n?o entram nas m?tricas de IL e nenhum assembly foi normalizado.'])
    (ROOT/'reports/round9-stop.json').write_text(json.dumps(stop,indent=2)+'\n',encoding='utf-8',newline='\n')
    command=['tools/round9_commit.py','--run-label','final-report','--message','docs: round 9 measured results and stop','--final-report']
    if args.push:command+=['--push']
    result=run(command,'final-commit')
    print(result.stdout.decode('utf-8',errors='replace'))


if __name__=='__main__':main()
