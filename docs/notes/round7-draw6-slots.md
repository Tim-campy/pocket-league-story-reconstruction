# SubForm._draw6: seletor, escopo e assinatura global

Alvo: `form.SubForm::_draw6(kairo.unity.ui.Graphics)->System.Void`,
13.213 bytes de IL original. A investigação usa o snapshot congelado anterior
ao ciclo 11. Nenhum candidato do jogo foi compilado nesta frente.

## Diagnóstico medido

O original tem **105 locais**, o recompilado tem **107**. A diferença de
inventário é de dois Int32: 79 originais contra 81 recompilados. Os outros
26 tipos e suas multiplicidades coincidem. O modelo smcs congelado prevê
completamente a assinatura recompilada de 107 locais, sem desacordo.

A primeira atribuição ternária escreve o local MapchipMain de slot 7. No IL
original, a comparação em IL289 usa `brfalse IL323`; o braço não nulo escreve
7 em IL316 e o braço nulo escreve 7 em IL348. A recompilação recuperada
seleciona um valor na pilha e faz um store comum. As duas transformações já
existentes, `assignment-ternary-if` e `assignment-ternary-if-reverse`, mantêm
107 locais. Elas não podem passar pelo gate global de assinatura: o problema
não é um temporário criado por essa atribuição nem uma limitação da regra.

O braço não nulo aparece primeiro no original; isso seleciona a variante
reverse, com `if (tenantData != null)`, sem tentativa de compilar o jogo.

## Dois defeitos independentes do fonte recuperado

Três declarações de `width2` representam o mesmo slot original 61:

| Slot recompilado | Primeiro uso IL | Último uso IL |
|---:|---:|---:|
| 66 | 7647 | 7770 |
| 73 | 7937 | 7968 |
| 74 | 8026 | 8125 |

Os intervalos não se intersectam. As três declarações são Int32 não capturados
e seu menor ancestral comum é o bloco `type_ == 69`, que também declara
`num28`. O IL original começa esse caso com `ldc.i4.0; dup; stloc.s 62;
stloc.s 61`, antes de `GameView.GetInstance`. O original usa 61 de IL7577 a
IL8127 e 62 de IL7575 a IL8174. Unificar as três variáveis, preservando seus
inicializadores nas posições antigas, e declarar `coalescedWidth, num28`
com `coalescedWidth = num28 = 0` recupera essa ordem.

O outro defeito é o local que guarda `GetMatchInfoJoin()`. O original chama
o método uma vez em IL5362, escreve o Int32 49 em IL5367, carrega-o em IL5369
e testa-o com `brtrue`. Seu último uso é IL5834. O recompilado também chama
uma vez, mas guarda o resultado no temporário de switch 106, alocado depois
de todas as declarações. Isso desloca LeagueData, QuestMatchData e os tipos
posteriores na assinatura. O local explícito recuperado fica antes dos braços,
na posição 49, e a chamada continua sendo avaliada uma única vez.

Dois switches do fonte recuperado reutilizam o mesmo temporário 106. O
original tem zero opcodes `switch`; o recompilado tem um. A composição
converte os dois para cadeias de if, mantendo os seletores estáveis e
avaliando a chamada `GetMatchInfoJoin` em um local explícito. Não se tenta
remover o temporário somente de um dos switches, pois o outro continuaria
mantendo o slot extra. Essa forma de fonte permanece hipótese até o verificador.

Não existe local original sem uso neste método. Nenhuma declaração fictícia
foi reservada para forçar a contagem.

## Resultado da previsão independente

| Etapa | Locais previstos | Ordem completa igual ao original |
|---|---:|---|
| Fonte recuperado | 107 | Não |
| Somente if da primeira atribuição | 107 | Não |
| Coalescência das três larguras e cadeia de zeros | 105 | Não |
| Seletor 49 e dois switches convertidos em if | 105 | Sim |
| Variante reverse da primeira atribuição | 105 | Sim |

O modelo fonte-only confirma individualmente `mapchipMain=7`,
`switchValue=49`, `coalescedWidth=61` e `num28=62`, além da igualdade de todos
os 105 tipos. A assinatura original é apenas a restrição comparada depois
da previsão; não é usada como resposta do modelo.

Os nomes dos novos locais são escolhas legíveis. Os nomes históricos dos
locais originais são UNKNOWN. Renomeiam-se somente referências vinculadas
aos símbolos das três declarações selecionadas; membros e parâmetros não
são renomeados. A coalescência é recusada em caso de captura, tipos diferentes,
declarações incompatíveis ou intervalos de uso sobrepostos.

## Provas de emissão independentes

A fixture própria reproduz dois switches e três locais de largura em blocos
distintos. Depois da transformação semântica, o smcs instalado produz corpo
completo, assinatura de locais e EH iguais ao exemplo original da fixture.
O modelo prevê seis locais e a compilação confirma seis. A fixture também
confirma a recusa de uma captura na análise semântica e de intervalos
intersectados medidos no IL real de outro método de controle.

Os exemplos da fixture têm nomes distintos: a comparação de emissão distingue
esse fato da igualdade de corpo completo. Não normaliza assemblies nem promove
estado do jogo. Não foram copiados bytes de IL para qualquer resultado.

O patch composto usa a família nova
`calculated-named-switch-selector-coalescence`, com os campos de método,
caminho lógico, before/after e offsets UTF-16 do formato já usado pela
coalescência. CRLF é preservado. A reaplicação futura precisa exigir uma única
ocorrência exata da declaração `before` e repetir o gate do modelo no fonte
corrente. A compilação e o verificador completo continuam sendo os únicos
responsáveis por medir o avanço real e promover matched.

A assinatura está confirmada como **MODEL_APPROVED**. O corpo e o avanço
do prefixo após compilação continuam **UNKNOWN**. Métodos e bytes efetivamente
ganhos por esta frente: **0 e 0**. O alvo representa 13.213 bytes potenciais,
sem estimativa de fechamento integral.

## Reprodução e proveniência

`python tools/round7_draw6_slots_calculate.py` refaz o diagnóstico e a composição
fonte-only a partir do snapshot anterior ao ciclo 11. Grava
`reports/round7-draw6-calculated-patches.json`; o diagnóstico intermediário fica
em `reports/round7-draw6-slot-diagnosis.json`.
`python tools/test_round7_draw6_slot_calculus.py` executa somente a fixture
independente e grava `reports/round7-draw6-calculus-fixture.json`.

O `before` é histórico, mas a declaração `_draw6` permaneceu inalterada no Main
atual após os ciclos 11 e 12: a publicação confirmou uma única ocorrência exata
de `before` no texto cru atual. Mudanças nos métodos anteriores do mesmo arquivo
podem deslocar o offset; `rebase_unique_patch(source,patch)` é reexportado pelo
calculador e recalcula somente esse offset UTF-16. O consumidor deve repetir
a previsão no fonte corrente antes de compilar. Não se ignora diferença de texto.

O documento registra `calculator_sha256`, `syntax_source_sha256` e um mapa
`dependency_sha256` dos utilitários de coalescência, solver de slots e diagnóstico.
Cada patch também registra os hashes do modelo e do cliente congelados.
O auxiliar de sintaxe é uma extensão isolada dos utilitários existentes e não
foi adicionado à lista de ferramentas ativas.

Após corrigir a assinatura, as duas orientações do if da primeira atribuição
passam pelo modelo de tipos. Isso não comprova o IL das duas: o `brfalse` e a
ordem dos braços originais sustentam a escolha reverse. O corpo real do jogo,
inclusive esses dois candidatos de orientação, continua UNKNOWN até o verificador.
