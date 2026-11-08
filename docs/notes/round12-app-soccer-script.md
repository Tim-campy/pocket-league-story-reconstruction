# Script — cadeia nomeada e donos de saídas

Entrega privada READY_FOR_GAME_PROBE. Resultado dos alvos e regressões globais UNKNOWN. Nenhum target/game build, verificador oficial, alteração pública ou estado foi executado. A frente fica restrita a estes dois métodos.

## Medidas e vínculos

Base oficial B13 congelada em `build/round12/runs/b13/commit/postverify01/Assembly-CSharp`:

| Método | Original | Atual | maxstack | Locais | EH |
|---|---:|---:|---:|---:|---:|
| form.GameForm::Script()->System.Void |178IL|167IL|19|2|0|
| form.SoccerForm::Script()->System.Void |378IL|352IL|28|2|0|

Total original556IL. Os cabeçalhos são fat12/flags19/initlocals=true. Ambos os slots originais são Int32[]0 e Int32 1, já iguais aos atuais; a causa não é permutação de slots. A primeira diferença bruta é o destino do br@0, no byte1, cujo deslocamento depende do dispatch adiante. A primeira edição causal alinhada é offset17, `ldc.i4.1` original versus `switch` atual.

REA original, Assembly-CSharp SHA256 `3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949`, MVID `48ce9f89-73d2-40be-bb24-22c807c04106`:

- GameForm token0x06000150, evidência `ev_c5dbf0dc013e404cf74a6b10fae9b85b01f1b6ae9887aa43635bc3c718d71afe`.
- SoccerForm token0x060001c3, evidência `ev_91d1e9a3fbfaa5236af762d1e6f0c85867906151a099f26d3914e31e6b68a584`.

Cada `bindings.json` guarda hash de IL conferido contra os bytes do dump, fonte, manifest/receipt/dumps, spansUTF16 e SHA do arquivo observado. O before completo ocorreu uma vez no Main observado; qualquer adoção posterior no mesmo arquivo exige rebase único por método e novos hashes. Strings externas relatadas pelo REA não são usadas nestes métodos; limites de tokens locais ao build estão preservados nos receipts.

## Controle lido e fonte calculado

O original grava `firstScript[0]` em slot1 e o testa na ordem1,0,2,3 para GameForm e1,0,2,3,4,5 para SoccerForm. Não contém opcode switch. A proposta declara `int recoveredScriptKind=firstScript[0];` no corpo do while e converte o dispatch em cadeia if/else if na mesma ordem, sem reler o elemento.

GameForm: caso1 faz Push+remove; caso0 ExecEvent+remove; ambos saltam para155. Caso2 remove e continua o while, br131→160. Caso3 remove; o caminho sem caso também chega155. O comando em155 é `br177`, saída do while, antes do ret final177. Por isso os antigos returns locais tornam-se fallthrough da cadeia e um único break do while após a cadeia. A remoção comum que estava depois do switch é transferida para o corpo do caso2, onde o IL já a mostrava; nenhuma chamada é criada/omitida.

SoccerForm: casos1/0/3 chegam à saída comum355, que emite br377 para o ret final. Caso2 remove e continua, br97→360. Caso4 mantém integralmente a inicialização quando firstScript[1]==0, incremento e teste de WAIT_CARD; o caminho que muda estado/remove continua o while em302→360, o outro chega ao break comum307→355. Caso5 muda estado/remove e continua em350→360. A proposta troca apenas os breaks pertencentes ao switch por continue do while, remove os returns dos braços e coloca o break comum depois da cadeia. Comparações numéricas, folhas, chamada Random e seus pontos de avaliação permanecem iguais.

Destinos foram conferidos como instruções reais:160/360 iniciam a condição Size;177/377 são ret;155/355 são br para esse ret. Não foram tratados como simples retorno presumido pelo nome de um rótulo. Não há EH a atravessar. `slot-constraints.json` registra âncoras e todos os usos dos dois slots.

## Emissão Mono e modelos

Revisão fixada `b6587926c0f146c8b12985744f33a13b39a22d57`, `analysis/tools/round5-mono-source/mcs/mcs/statement.cs`:

- If.Resolve163–218, especialmente211: is_true_ret recebe o estado de fluxo inalcançável, não só presença lexical de return.
- If.DoEmit225–267: teste do braço falso; br comum é emitido quando existe else e o braço verdadeiro alcança o fim. Um continue torna o fim inalcançável e dispensa br redundante.
- While.DoEmit421–467:453 emite salto inicial para LoopBegin,458 marca esse início na condição e463 marca LoopEnd. Confirma os alvos de continue/break usados aqui.
- Break1110–1119 e Continue1141–1150: emitem Br/Leave respectivamente para LoopEnd e LoopBegin, com distinção de proprietário léxico; não são ret diretos.

NoEmit genuíno com helper já existente/hash conferido: before e candidate complete=true nos dois métodos, ambos com `[System.Int32[],System.Int32]` exatos. Nenhuma previsão completa incorreta foi relaxada. O nome recoveredScriptKind é escolhido para o fonte reconstruído; não se alega recuperar um nome de variável original sem PDB.

## Ensaios próprios e negativos

Dez compilações smcs2.6.5 de fixtures próprias, cinco por método, com referências/flags fixadas, saída privada Fixture.dll, `-debug-`, sem PDB/MDB de produção. Os controles manuais foram escritos independentemente, com variáveis renomeadas, mesma ordem de folhas e proprietários de transferências.

| Grupo | Before | Candidato | Controle manual | Só dispatch, mantendo returns | Continue trocado por break |
|---|---:|---:|---:|---:|---:|
|Game|167|178|178|167|178|
|Soccer|352|378|378|358|378|

Candidato e controle coincidem integralmente em cabeçalho/locais/IL/EH nos dois grupos. Todos os outros três casos por grupo diferem, inclusive o dono errado com tamanho igual. `control-projection.json` compara também cada offset/opcode/destino de branch/slot/constante e nome de folha da fixture ao original:66 instruções Game e137 Soccer, todas iguais; cabeçalho, assinatura de locais e EH também iguais. A projeção exclui assinaturas/tipos/assemblies dos stubs próprios e não é o oráculo do alvo. Nenhum byte original foi copiado para fonte ou resultado.

Três negativos do produtor foram rejeitados sem compilar: inventário de casos divergente, colisão do nome novo e break adicional no caso4. Uma tentativa inicial de chamar o produtor sobre a classe-fixture inteira foi rejeitada antes de qualquer compilação; o driver foi corrigido para passar o método isolado conforme a API. Esse ajuste de integração não é evidência de lowering nem ensaio do jogo.

## Histórico e limites

Game:87receipts,2trials históricos incluindo repetições; R4 tentou hoist de firstScript e loop-cache-scope, sem adoção, prefixo1. Soccer:87receipts,6trials; R4 tentou hoist, loop-cache-scope e dois condition-local; R7C06 tentou dois empty-arm-restore, todos sem avanço do prefixo1. Guardas/caches isolados não representam a composição atual. Nenhum hash da proposta inteira foi encontrado; receipts sem payload/SHA não provam exclusão de toda forma histórica. Zero candidatos de outros ciclos não é refutação.

APIs puras em `calculate.py`: `transform_game_script(method_source)->str` e `transform_soccer_script(method_source)->str`. Cada método tem patches.json próprio, before/after completo, CRLF preservado e spansUTF16. `ready-index.json` referencia ambos e o freeze. A adoção pode ocorrer separadamente, com verificação oficial completa e zero regressões; até lá nenhum método/byte é contado como ganho.
