# SetTenantIn — atribuições sequenciais

Estado da entrega: READY_FOR_GAME_PROBE. Corpo do alvo e regressões oficiais: UNKNOWN. Nenhuma compilação do jogo, verificação oficial, escrita pública ou alteração de estado foi feita.

## Evidência vinculada

Método `game.UserData::SetTenantIn(game.TPlayer)->System.Boolean`, token `0x06000404`, original653IL. Snapshot oficial B12/commit9e330ff: `build/round12/runs/b12/commit/postverify01/Assembly-CSharp`; este corpo não mudou desde B9. O before completo foi localizado exatamente uma vez no Main observado. `bindings.json` preserva fonte, spansUTF16, hashes de receipt/manifests/dumps e SHA do arquivo observado para futuro rebase por método.

REA primário `ev_9f3b9f104e9ad7046c04a529a83d4fd1bd95f94bb7eccf8e66c89b817b94a474`, pai `ev_0a92842d503efdedf04b18d2e0f16198ab062c3d5b2b91e2bd7791514ff7b119`. Artefato original SHA256 `3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949`, MVID `48ce9f89-73d2-40be-bb24-22c807c04106`; hash somente dos653bytes de IL `e7f51353bbd1f1028dc85c713e42538f8feddda659e2f2cfa9867924620c5c60`. Os hashes foram conferidos contra original somente leitura e dump congelado. O REA registra limitações de tokens locais ao build e strings #US fora deste trecho; nenhuma string é usada pelo método.

## Causa e proposta

Ambos os cabeçalhos são fat12, flags19, maxstack46, EH0. Atual656IL/16locais; original653IL/15locais. A primeira diferença bruta@476 é o destino do `br@475`:545 atual contra542 original. A primeira edição causal por alinhamento ocorre@500, dentro da acumulação de pesos, não na condição do laço.

Fonte atual: `num2 = (array[l] = num2 + apdat_.tenantData_[tenant3.tenantDataId_].charm_);`.

Forma calculada: `num2 = num2 + apdat_.tenantData_[tenant3.tenantDataId_].charm_; array[l] = num2;`.

No original, `ldloc9@500` inicia o cálculo; `stloc9@527` termina a primeira atribuição. Depois vêm `ldloc8@529; ldloc10@531; ldloc9@533; stelem.i4@535`. O atual carrega array/index antes do cálculo e usa `dup@531; stloc15@532; stelem.i4@534; ldloc15@535; stloc9@537`. Slot15Int32 é exclusivamente temporário da atribuição array usada como expressão. Os15slots nomeados têm tipos e âncoras já iguais; nenhum precisa ser movido ou renomeado.

O cálculo explícito das posições, relocação de destinos e retirada desse temporário coincide nos237 objetos de instrução originais,653IL, com zero diferenças (`abstract-report.json`). Isto é um diagnóstico calculado, não um corpo emitido/aprovado do alvo. Os blocos/continues adotados anteriormente permanecem idênticos. A proposta não é a transformação de guarda de Training/Camera; seu mecanismo é a avaliação de uma atribuição de array como expressão versus dois comandos.

Não se afirma equivalência comportamental geral das duas formas em presença de exceção: a ordem original de atualização do acumulador foi lida no IL. O produtor fica restrito ao texto exato, com array/index locais estáveis e uma ocorrência, sem aplicação genérica a propriedades ou chamadas de índice.

## Compilador e NoEmit

Mono fixado revisão `b6587926c0f146c8b12985744f33a13b39a22d57`, arquivos `analysis/tools/round5-mono-source/mcs/mcs`:

- `assign.cs:399–412`: `Assign.Emit` passa `leave_copy=!is_statement`; comando isolado usa false.
- `expression.cs:8194–8236`: atribuição de array carrega array/argumentos antes da fonte; quando leave_copy=true emite Dup, cria LocalTemporary e armazena a cópia antes de stelem.
- `expression.cs:8266–8269`: recarrega e libera o temporário para devolver o valor da atribuição interna ao acumulador externo.
- `expression.cs:4321–4355`: atribuição local emite a fonte, Dup só se leave_copy, então Variable.EmitAssign. O comando separado não precisa da cópia temporária.

NoEmit genuíno, helper previamente existente e hash conferido: fonte atual complete=true/16tipos; proposta complete=true/15tipos e sequência original integral exata. Modelo completo incorreto não foi forçado a incomplete. `slot-constraints.json` registra os usos de cada slot original; `model.json` guarda hashes do modelo, client, helper, manifest e fontes.

## Fixtures e negativos

Cinco compilações smcs2.6.5 de fixtures próprias, referências/flags fixadas, saída somente privada Fixture.dll e `-debug-`. O candidato e um controle manual independente com variáveis renomeadas coincidem integralmente em cabeçalho, locais, IL e EH:653IL,15locais, fat12/flags19/maxstack46/EH0. As classes/stubs próprias não são o jogo; isto confirma o mecanismo, não a correspondência do alvo.

Controles negativos preservados: before656IL/16locais diverge; chain inversa `array[l]=(num2=...)`652IL/15 diverge; store antes de acumular653IL/15 também diverge apesar da mesma contagem. O produtor rejeitou três entradas sem compilação: âncora ausente, duplicada e índice com chamada. `fixture-report.json` contém comparação completa, logs/hashes/fontes/binários e todas as negativas.

## Histórico e entrega

Varredura dos receipts disponíveis:87registros do método,10trials incluindo repetições. R4 adotou dois guards de continue, prefixo38→144→476; tentou um condition-local e quatro hoists sem avanço adicional. R5 tentou três hoists, todos476→474 e rejeitados. A proposta atual mantém os guards aprovados e não repete esses hoists. Nenhum hash da proposta inteira apareceu nos trials capturados; receipts sem payload/SHA não permitem excluir universalmente toda tentativa antiga. Zero candidatos não é refutação.

API pura `calculate.py::transform_tenant(method_source)->str`, sem IO ao importar/chamar. `patches.json` guarda before/after completo CRLF preservado, spansUTF16 e hashes. O arquivo UserData.cs pode mudar por League/Outro método; o executor deve rebase por ocorrência única do before e renovar o vínculo de arquivo. Nenhum ganho é contado nesta entrega. Somente o oráculo oficial pode aceitar ou rejeitar o alvo.
