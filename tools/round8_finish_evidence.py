"""Close reporting inputs from completed receipts; never changes reconstruction state."""
import json
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1]
def read(path):return json.loads((ROOT/path).read_text(encoding='utf-8-sig'))
def write(path,data):(ROOT/path).write_text(json.dumps(data,indent=2,ensure_ascii=False)+'\n',encoding='utf-8',newline='\n')
def main():
    journal=read('reports/round8-cycle-journal.json')
    if journal['consecutive_zero_cycles']<3:raise ValueError('Expected measured stop condition')
    invalid=[]
    for p in (ROOT/'reports').glob('round8-cycle0[45]-initial-screen-*-engine.json'):
        d=read(p);invalid.extend(t for m in d['methods'] for t in m['trials'])
    if len(invalid)!=9:raise ValueError('Unexpected invalid-screen audit inventory')
    stop={'reason':'três ciclos válidos consecutivos sem novo método em matched (5, 6 e 7)',
        'new_reconstruction_work_stopped':True,'cycles_completed':len(journal['cycles']),
        'blockers':'Nenhuma instalação, arquivo ou decisão do usuário é necessária. A rodada parou pela condição experimental de três ciclos sem ganho; hipóteses e fontes preparadas não foram compiladas depois da parada.',
        'preparation_notes':[
            'C3: o patch DrawPolygons havia sido exportado com LF para uma baseline CRLF. O rebase exato o recusou; a tentativa inicial foi preservada. O transporte corrigiu somente o patch, sem editar o fonte-base; o retry verificou o corpo completo de 1563 bytes.',
            'C4/C5 iniciais foram experimentos INVÁLIDOS: nove triagens rotuladas como candidatos (sete das varreduras e duas sondas) compilavam o arquivo-base desatualizado. Não houve avanço, adoção, promoção ou perda. A função de escrita foi corrigida e testada em cinco compilações toy; os dois experimentos foram arquivados por ferramenta auditável e repetidos como ciclos válidos 4 e 5. Git e os hashes dos ledgers foram preservados; não houve edição manual de estados. O diário científico tem sete ciclos concluídos, além desses dois experimentos inválidos.',
            'Foram removidos apenas 53 arquivos de cache de metadados regenerável (369180435 bytes lógicos), preservando os 16 mais recentes. Compressão NTFS sem perdas liberou espaço nos artefatos de build; não alterou bytes dos assemblies. Fontes, originais e provas congeladas foram preservados.'
        ],
        'recommendations':[
            'Testar os patches de lifetime/escopo Roslyn já preparados para StringUtil.GetCode e Graphics.DrawArc: dois alvos, 2514 bytes de IL, potencial máximo de dois métodos; ganho efetivo UNKNOWN. GetCode prevê 31 slots completos, DrawArc tem variante de 28 tipos ainda incompleta e pode ser compilada pelo protocolo vigente.',
            'Testar a composição de acesso nulo com ternário negativo em StreamUtil.WriteArray: um alvo, 1686 bytes, potencial máximo de um método; os 14 bytes iniciais foram reproduzidos em fixture histórica, corpo completo do jogo UNKNOWN. A regra não está integrada e só um dos cinco membros do grupo é elegível.',
            'Aplicar os três patches calculados de coalescência de coordenadas nos métodos Draw_coatEva, SetSavePos e Draw_moveArea: três alvos, 4316 bytes, potencial máximo de três métodos; previsões completas/exatas e fixtures aprovadas, ganhos do jogo UNKNOWN. Para SpForm.Update, o ponto de retomada medido é o operando do br em IL_04fe, primeira diferença no byte1279; sua assinatura de24 locais já está correta.'
        ]}
    write('reports/round8-stop.json',stop)
    cfg=read('config/round8-protocol.json')
    cfg['final_model_measurements']={a:{'validation':'reports/round8-final-'+stem+'-model-validation.json',
        'corpus_baseline':'build/round8/final-model-corpus/'+a} for a,stem in [('Assembly-CSharp','smcs'),('KairoLibrary','roslyn')]}
    write('config/round8-protocol.json',cfg)
    hypotheses=[
        {'status':'CONFIRMADA','hypothesis':'Modelo Roslyn estreito para void try/finally pode preservar o local vivo no handler',
         'evidence':'DrawPolygons tem 35 slots e corpo completo de1563 bytes aprovado em C3; baseline-modelo passou de2228 para2236 completos/exatos no mesmo corpus3131, sem retirar cobertura nem criar erro completo.'},
        {'status':'CONFIRMADA','hypothesis':'Candidatos incompletos precisam ser compilados a partir do arquivo alternativo atualizado',
         'evidence':'Auditoria preservou nove triagens inválidas; fixture compartilhada de cinco compilações distinguiu baseline, candidato e reset. C4 válido ganhou três métodos/2360 IL após a correção, sem regressão.'},
        {'status':'REFUTADA','hypothesis':'A ordem textual de toda construção C# determina a ordem dos blocos no IL',
         'evidence':'Roslyn1.1 EmitExpression.cs:2646-2647 emite Alternative primeiro;2689-2690 emite Consequence depois. Fonte pinned4cb203dab89e1542cdf7d88cb6d32767bc0e5460, SHA4beb87ae114cd851204eed776ed12f9cc8d533b86299d8683bf455c212876f09. Fixture negativa reproduziu14 bytes iniciais do StreamUtil, não o corpo do jogo.'},
        {'status':'REFUTADA','hypothesis':'Basta expandir null-conditional/coalesce para um ternário positivo para corrigir WriteArray',
         'evidence':'C5 válido compilou uma variante por passagem com assinatura original, mas manteve a diferença no byte1. O emissor põe o braço alternativo primeiro; a composição negativa foi provada apenas em fixture e não testada no jogo.'},
        {'status':'REFUTADA','hypothesis':'A composição atual do seletor e guarda de Init2 preserva o prefixo necessário',
         'evidence':'C6 confirmou os68 slots originais emitidos, porém o screen real reduziu o prefixo bruto43 para9; candidato recusado e nenhuma fonte adotada.'},
        {'status':'CONFIRMADA','hypothesis':'Convergência dos payloads permite corrigir a assinatura e avançar a guarda de SpForm.Update',
         'evidence':'C7 confirmou24 locais e preservou todos os matched; composição mais duas regras herdadas avançaram o prefixo295→717→1244→1279. Corpo2052 continua divergente; não houve ganho de métodos/IL matched.'},
        {'status':'INCONCLUSIVA','hypothesis':'Novos bundles de coordenadas, lifetime Roslyn e polaridade nula fecham seus métodos do jogo',
         'evidence':'Foram preparados e validados em fontes/fixtures antes da parada; não houve compilação dos candidatos do jogo dessas famílias. Não entram em matched.'}
    ]
    write('reports/round8-additional-hypotheses.json',{'hypotheses':hypotheses})
    write('reports/round8-prepared-only.json',{'rows':[
        {'front':'Coalescência de coordenadas smcs','target_methods':3,'original_il_bytes':4316,'game_candidate_compilations':0,'methods_gained':0,'fixture_complete_equal':2},
        {'front':'Lifetime/escopo Roslyn','target_methods':3,'source_variants':4,'original_il_bytes':4492,'game_candidate_compilations':0,'methods_gained':0,'fixture_complete_equal':3},
        {'front':'Ternário negativo após null-conditional','eligible_methods':1,'group_methods':5,'eligible_il_bytes':1686,'game_candidate_compilations':0,'methods_gained':0,'fixture_controls':12,'fixture_complete_equal':3,'original_prefix_bytes_equal':14},
        {'front':'Expansão de *= em campo float/double','game_candidate_compilations':0,'methods_gained':0,'fixture_complete_equal':4,'fixture_negative_controls':9}
    ]})
    print(json.dumps({'stopped':True,'valid_cycles':len(journal['cycles']),'invalid_screen_trials_archived':len(invalid)}))
if __name__=='__main__':main()
