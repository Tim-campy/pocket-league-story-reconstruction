# Rodada 7: identidades locais ancoradas nas atribuições

O alvo é `form.SubForm::_draw3(kairo.unity.ui.Graphics)->System.Void`, com
13.116 bytes de IL. A análise usa o fonte aceito do ciclo 05 e o dump
`adoption-rechecked-final-methods.json`, não a DLL transitória de uma tentativa.
O calculador exige um recibo oficial fresco para o Main atual e comprova que
seu corpo reconstruído é idêntico ao checkpoint aceito do ciclo 05. O patch
usa o texto cru do Main como `before`, preservando a limpeza de espaços.
Nenhum candidato do jogo foi compilado por esta frente.

O primeiro desvio real registrado pelo alinhamento está no store em IL3518:
o original usa o slot 32 e o recompilado usa 36. A chamada anterior é a mesma
`RateConvert(num16, 0, 140, 23, 75, true)`. Todas as seis cargas de argumentos
e a identidade completa do método chamado coincidem. Esse contexto aparece
uma única vez antes de um store em cada corpo. A declaração recuperada de `r`
está dentro do while e por isso recebe o slot 36; o slot 32 pertence a `num22`.

Não se trata de unir `r` e `num22`. As atribuições posteriores provam quatro
identidades distintas e uma permutação nas declarações:

| Avaliação preservada | Store original | Slot original | Slot recompilado |
|---|---:|---:|---:|
| `RateConvert(num16,0,140,23,75,true)` | 3518 | 32 | 36 (`r`) |
| `gameWidth / 2 - 120` | 3932 | 36 | 37 (`num20`) |
| `gameHeight - 240` | 3942 | 37 | 38 (`num21`) |
| `14` | 3946 | 38 | 32 (`num22`) |

Cada contexto de avaliação usado pelo calculador é idêntico e único nos dois
corpos. A âncora de largura inclui o contexto anterior necessário para
distinguir outra expressão igual; não se presume identidade só pelo tipo ou
por um único opcode. Não há normalização de desvios ou de operandos para
aprovar corpos. As âncoras são apenas evidência para calcular o fonte.

O slot original 32 é usado entre IL3490 e IL3564, na inicialização de zero e
na cor do gradiente. O slot original 36 só aparece de IL3932 a IL3997; 37, de
IL3942 a IL4019; 38, de IL3946 a IL4127. Assim, o original não usa o slot 32
para o valor 14 posterior ao laço. Os votos globais também sugerem a permutação,
mas misturam a inicialização de zero ligada erroneamente a `num22` com seus
usos posteriores. É necessário acompanhar a atribuição, não apenas o nome.

A proposta faz quatro edições de fonte:

* Troca a declaração antecipada `int num22;` por `int r;`.
* Recupera `r = num17 = num18 = 0;`, mantendo os stores iniciais nos slots
  34, 33 e 32, na mesma ordem.
* Mantém a chamada RateConvert dentro do while e converte a declaração
  `int r = ...;` em `r = ...;`.
* Mantém a avaliação de 14 depois dos cálculos de largura e altura e converte
  `num22 = 14;` em `int num22 = 14;`.

As quatro avaliações da tabela continuam nas posições anteriores do fonte.
Nenhuma chamada ou inicialização é antecipada para fora do laço. A previsão
independente do modelo smcs congelado produz **119 locais**, com todos os tipos
na ordem original, e confirma especificamente `r=32`, `num20=36`, `num21=37`
e `num22=38`. Essas posições são verificadas além da igualdade da assinatura,
que sozinha não distingue quatro locais Int32.

A fixture independente compilada pelo smcs instalado reproduz o problema:
o fonte recuperado tem corpo diferente, enquanto as quatro correções geram
corpo completo, assinatura de locais e EH idênticos ao exemplo original da
fixture. Os exemplos da fixture têm nomes de método diferentes; o relatório
distingue essa identidade da comparação do corpo. Não se promove estado do jogo.

`tools/round7_store_anchor_calculate.py` regenera
`reports/round7-store-anchor-patches.json` usando o Main oficialmente verificado,
cujo corpo é conferido contra o snapshot aceito do ciclo 05.
O documento contém uma proposta com a família
`calculated-store-anchored-declarations`, hashes do modelo/cliente, âncoras,
proveniência dos dumps e offsets UTF-16 com CRLF preservado.
`rebase_unique_patch(source, patch)` é reexportado da API de coalescência:
só permite reaplicar a declaração completa quando `before` aparece uma única
vez. O consumidor precisa executar uma nova previsão sobre o fonte corrente.

`tools/round7_store_anchor_fixture.py` regenera a prova independente em
`reports/round7-store-anchor-fixture.json`. Nenhum desses comandos instala
ferramentas ou compila candidatos do jogo.

A função `unique_store_anchor(original, rebuilt, offset, min_size)` pode
encontrar âncoras de outras atribuições, exigindo contexto único em ambos os
corpos. Ela não decide automaticamente uma permutação de variáveis: uma mesma
variável recuperada pode representar identidades originais diferentes em
atribuições distintas, como ocorre na cadeia de zeros deste caso.

O avanço real do prefixo e o fechamento do corpo continuam UNKNOWN até a
compilação isolada e o verificador completo executados pelo processo principal.
O potencial deste alvo é 13.116 bytes; não há método ou byte ganho declarado
por esta frente.

Uma tentativa inicial de reaplicar literalmente o texto do checkpoint falhou:
a adoção do ciclo 05 limpou espaços em linhas vazias do Main. Isso não indica
que os avanços parciais foram descartados. A hipótese inicial de ausência de
adoção foi refutada pelo recibo fresco e pela igualdade completa dos corpos.
O calculador final deriva os quatro edits sobre o Main atual com 119 locais;
não exporta uma variante antiga de 126 locais nem tenta ignorar diferenças de
texto ao reaplicar o patch.
