# Round 8: SpForm.Update ? payload compartilhado, slot vazio, switch e guarda

Status real: UNKNOWN; zero compila??es de jogo. Baseline25b8108711a4a8920a2600cf9ffea5b710b3a481, snapshots build/round8/baseline/Assembly-CSharp. M?todo form.SpForm::Update()->System.Boolean, corpo original2052 bytes. O candidato ? aprovado apenas por c?lculo de fonte e pelo modelo congelado, cujo resultado independente tem24 tipos exatamente iguais aos originais.

## Diagn?stico da tentativa anterior

A guarda reversa isolada gerava um candidato completo com os mesmos27 tipos atuais, contra24 originais. O gate a rejeitou antes da compila??o. Portanto a hip?tese da guarda continuou INCONCLUSIVA, n?o refutada por IL de candidato. A assinatura exige tr?s corre??es combinadas antes de testar polaridade.

## Composi??o calculada

1. Os quatro byte[] data recuperados nos slots11,13,17,21 apontam ao slot original11. Cinco stores t?m contextos assim?tricos ?nicos em ambos os lados:1060?1052,1275?1267,1327?1319,1683?1680,1702?1699. Os intervalos reconstru?dos s?o[1060,1105],[1275,1279],[1327,1381],[1683,1706], sem interleaving. A an?lise sem?ntica une apenas os quatro s?mbolos, rejeita captura e move somente a declara??o ao bloco que cont?m as quatro alternativas. As chamadas Read permanecem nos pontos originais. O nome saveDataBytes ? sint?tico, n?o hist?rico. Contagem27?24.
2. O original tem zero opcodes switch; o recuperado tem um switch de num2, um local j? calculado. Convert?-lo em cadeia de if remove o tempor?rio de emiss?o:24?23. A leitura do seletor ? livre de efeito e cada teste usa o mesmo valor local.
3. O slot original13 ? kairo.unity.ui.Dialog e n?o aparece em nenhum acesso local de todo o m?todo. Antes da reserva, recordStore ocupa13. Uma declara??o Dialog sem inicializador nessa fronteira reserva13 e devolve recordStore a14, sem acrescentar IL:23?24. O escopo e nome hist?ricos desse local s?o UNKNOWN; a posi??o escolhida ? comprovada pelo modelo e n?o pretende recuper?-los.
4. Somente ap?s confirmar os24 tipos exatos, aplicar a guarda reversa j? existente em num2 != -3. O modelo continua completo e com os mesmos24 tipos. Ainda falta executar o verificador sobre a compila??o real do jogo.

Slots especiais finais comprovados individualmente: saveDataBytes11, unusedOriginalDialog13, recordStore14. Fontes brutos preservam CRLF e offsets UTF-16; before da declara??o inteira tem correspond?ncia exata e ?nica no Main corrente. O gate precisa repetir a previs?o com o fonte corrente antes de compilar.

## Primitive novo para switch

A regra legada switch-if encontrou um default com Block e break terminal e preservou esse break depois de retirar o switch, produzindo CS0139 na an?lise moderna. N?o se relaxou o modelo. O primitive opt-in corrigido achata esse bloco, remove somente o break que sai do switch e aceita return/throw terminal. goto case/labels incompat?veis s?o rejeitados. Um seletor com efeito s? pode ser convertido mediante autoriza??o expl?cita da express?o e introdu??o de local calculado uma vez; no jogo o seletor j? ? local.

## Fixture

Fixture pr?pria com a composi??o completa: corpo285 bytes,9 locais,2 regi?es de exce??o, exatamente igual ? forma declarativa de refer?ncia. Fixture adicional de seletor com efeito: corpo37 bytes/1local exato, comprovando uma ?nica chamada. Negativos rejeitam seletor com efeito n?o autorizado, goto case, captura do payload e lifetimes intercalados. Duas execu??es finais da fixture (mesmo fonte e resultado) foram validadas; cada uma compila um ?nico assembly independente. A primeira montagem da fixture continha uma chave ausente no texto de teste, detectada pelo modelo como CS1513 e corrigida antes de qualquer compila??o; isso n?o afetou o alvo nem seus estados.

## API

Fam?lia calculated-shared-payload-switch-guard. Calculador source-only round8_payload_slot_calculate.py usa o modelo congelado e helpers isolados, nunca Emit do jogo. Bundle com schema de propostas externas: method_id/logical_path/start/end/before/after, complete/predicted_types, calculator/syntax/dependency/model/client SHA256, anchors/stages/special_variables. Uso python tools/round8_payload_slot_calculate.py; sa?da build/round8/smcs-slot-anchors/spform/round8-payload-slot-patches.json. Exigir rebase ?nico, gate independente exato, avan?o do prefixo e verifica??o completa antes de ado??o/commit.

Os r?tulos/operandos abstra?dos nos contextos de diagn?stico n?o participam da compara??o final nem alteram assemblies. Nenhum byte original foi copiado ao resultado. Nenhuma classe gerada foi escrita ? m?o e nenhuma fonte ativa, modelo ou ledger foi alterado por este c?lculo.
