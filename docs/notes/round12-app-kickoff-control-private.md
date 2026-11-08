# R12 — Team.SetPositionKickOff, duas diferenças compensatórias

Proposta privada finita para `game.Team::SetPositionKickOff()->System.Void`, token0x060003be,857IL. **Zero builds de alvo, verificador, alterações públicas ou estado. Resultado do jogo UNKNOWN.** Snapshot oficial B6: `build/round12/runs/b06/commit/postverify01/Assembly-CSharp`.

## Medição e causa real

Original e atual: **857IL**, fat12/maxstack27/um local Int32/EH0. Não existe diferença de assinatura, inicialização de struct ou `initobj` nessa região. O par do histograma `INSERTION_POINT → ldc.r4` é a constante zero de uma subtração na última chamada Set.

O primeiro byte divergente é606: operando do `br` em605, que vai para818 no original e823 no atual. O destino está adiante, e três hunks de sequência correspondem a **duas diferenças que se compensam em tamanho**:

1. Original,800–813: lê `FORMATION_KICKOFF[1].z_`, aplica `neg` e chama Set. Atual: carrega primeiro0f em800, lê z_ em805–816 e faz `sub` em817. A forma atual acrescenta5bytes.
2. Original,851–856: `br856; ret`. Atual: após ReceiveBall em851, retorna diretamente em856. Falta o br de5bytes que o original conserva antes do ret compartilhado.

A primeira igualdade de857bytes totais mascara as duas diferenças, não prova equivalência do corpo.

## Composição calculada

```csharp
if (udat_.gm_.gameState_ == team_id_) {
    // todas as folhas existentes
    // única ocorrência alterada:
    players_[players_.Length - 2].pos_.Set(
        -(FORMATION_KICKOFF[1].x_), 0f, -(FORMATION_KICKOFF[1].z_));
    // folha final existente
    udat_.ball_.ReceiveBall(players_[players_.Length - 1]);
} else { }
```

Somente `0f - FORMATION_KICKOFF[1].z_` torna-se menos unário e acrescenta-se um **else falso vazio no if externo final**, mantendo a comparação de igualdade e o corpo verdadeiro. Não se inverte o guard, não se acrescenta retorno e não se muda outro Set ou comparação. O produtor prova que esse guard está diretamente no corpo do método, é o último comando e ainda não tem else.

Não se assume equivalência geral de `0f-x` e `-x`: ponto flutuante inclui zero com sinal/NaN. **O opcode original neg é a evidência para esta única ocorrência.** Os operadores de comparação de formação e os outros zeros/menos existentes não são alterados.

O cálculo abstrato retira o `ldc.r4` extra, troca sub por neg e insere o br final. Com realocação dos destinos, reproduz **303objetos de instrução integralmente iguais e857IL**, incluindo br605→818 e br851→856. Não escreve IL/DLL e não substitui o oráculo. O local `i` e EH permanecem iguais no diagnóstico; o header efetivamente compilado ainda exige verificação do alvo.

## Evidência primária e emissão

REA `ev_3ae7e45ab0f75160d144f0dcb0756b1f397bc20000ef5b74593c1a732eda8657`, token0x060003be, assembly SHA `3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949`, MVID `48ce9f89-73d2-40be-bb24-22c807c04106`, IL SHA `4eccb7a0626f6c0833d4058d62f5fd1e76f45684f4c3331817c02811c2a9fd6b`. Hash conferido sobre exatamente857bytes após o fat header12 do dump; não inclui header, locais ou padding.

Mono revisão fixada `b6587926c0f146c8b12985744f33a13b39a22d57`:

- `analysis/tools/round5-mono-source/mcs/mcs/expression.cs:512–522`, `Unary.EmitOperator/UnaryNegation`: para float não toma o caminho de subtração checked; emite a expressão e `neg`.
- `analysis/tools/round5-mono-source/mcs/mcs/statement.cs:247–267`, `If.DoEmit`: emite o salto falso e o corpo verdadeiro. Quando FalseStatement existe e o corpo verdadeiro não é return, linhas251–257 emitem `br end`; um bloco falso vazio ainda fornece essa estrutura. Sem else, linhas265–267 apenas marcam o destino, sem o br final.

A retenção de else vazio não foi presumida: foi medida no smcs instalado. A coincidência integral desse fonte histórico com o interno Unity é UNKNOWN.

## NoEmit e cinco fixtures próprias

NoEmit11 genuíno, auxiliar já existente: atual e candidato **complete=true/um tipo Int32 exatamente original**. Nenhuma alteração de modelo/incompletude forçada. Os blocos/variáveis e hashes estão no JSON privado.

Cinco compilações de um molde próprio da cauda, não Team/jogo. Compiler SHA `26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0`, runtime SHA `3d102d9a63dae72e48396998bdbae4e9b2d8952ca6a8ebbcba698d3bc4345076`, flags/referências B6, `-out` privado e `-debug-`, sem PDB/MDB de produção.

- Candidato289IL = controle manual independente289IL em header/locais/IL/EH; maxstack11.
- Molde anterior também289IL/maxstack11, mas corpo diferente.
- Menos unário isolado284IL e else vazio isolado294IL diferem.

Os moldes preservam o receptor Set, os três argumentos e o controle aninhado, incluindo a chamada ReceiveBall após o if interno. Três negativos do produtor recusam else já existente, guard de dono aninhado e folha final alterada. A refatoração textual do gerador de fixtures foi conferida contra o arquivo fonte capturado byte a byte, sem compilação adicional; total real continua5.

## Histórico negativo preservado

82recibos/8registros de trials. R7 adotou `void-common-exit-if-else`384→385 e quatro correções de menos unário, chegando a606; elas estão preservadas no fonte atual. **A quinta correção de menos unário, exatamente esta ocorrência z_, já foi compilada isoladamente e recusada606→385.** Retirar5bytes sem repor o br de5 faz a saída do guard externo mudar mais cedo; a correção posterior não avançava o prefixo.

R7 também compilou `smcs-empty-arm-restore` com guard **invertido**, `if(gameState!=team){ } else { corpo }`, recusado606→384. Isso é diferente da proposta atual: `if(gameState==team){ corpo } else { }` conserva a polaridade verdadeira e coloca o braço falso vazio na saída. O teste antigo de temporário Boolean também foi recusado. Não se reapresenta nenhum desses componentes isolados como novo sucesso.

A composição atual acrescenta e remove5bytes juntos, mantém a saída global em856 e corrige os destinos internos. Os oito trials ficam preservados por SHA; registros podem conter repetições entre sonda/execução. Sem payload/SHA, ausência universal de uma tentativa continua UNKNOWN, e zero candidatos não refuta construção.

## Interface e limite

API pura `calculate.py::transform_kickoff(method_source)->str`, sem classe/cabeçalho/I/O. `patches.json` contém o método completo antes/depois, UTF16, SHA do arquivo/método B6, REA, modelos e produtor. O `before` foi confirmado único no Main; o operador deve fazer rebase fresco se outra parte de Team.cs mudar.

Todas as chamadas/strings e sua ordem são preservadas. Não se escreve classe gerada, assembly ou bytes originais no resultado. Estado **READY_FOR_GAME_PROBE**; ganho em método/bytes **UNKNOWN** até header/locais/IL/EH reais e verificação completa sem regressões pelo operador exclusivo. Pacotes anteriores intactos e nenhuma outra família aberta.
