# Rodada 6: modelo calculado de slots do smcs

A passagem completa usou avanço do prefixo bruto. Na última retomada dos três
gradientes, o avanço foi medido pelo primeiro trecho de instruções alinhadas,
preservando todos os bytes que já coincidiam. O salto antes do laço só pode ter
seu deslocamento corrigido depois das seis expressões internas; essa opção
permitiu corrigir cada expressão por vez. A comparação exata de corpo,
cabeçalho, locais e EH permaneceu como único critério para matched.

O modelo é implementado por `tools/Round6SlotModel.cs`, acessado por
`tools/round6_slot_model.py`. O auxiliar usa o Roslyn instalado somente para
analisar a sintaxe e resolver tipos; não chama Emit, não compila candidatos do
jogo e não usa a assinatura original como resposta da previsão. A assinatura
original entra depois, no filtro e no solver de declarações.

## Regra implementada

As declarações ordinárias entram na lista do bloco em ordem de inserção. Uma
declaração posterior a um comando cria um filho implícito, inclusive quando o
comando anterior é um inicializador. A etapa de metadados aloca primeiro os
locais declarados, depois os temporários registrados e depois os blocos filhos.
Os blocos lexicais e os filhos implícitos são representados separadamente.

O modelo registra cópia de array, índices e comprimentos para foreach de array,
enumerador para foreach de coleção, cópia de using de expressão e cópia do objeto
de lock. using com declaração usa o próprio local declarado. Durante emissão,
switch solicita um temporário para o seletor; switch de string acrescenta o
temporário do dicionário e o índice inteiro. Temporários liberados são
reutilizados por tipo numa pilha. O valor de retorno através de uma região
protegida ganha um slot persistente quando necessário. O modelo também cobre os
temporários observados de incremento/atribuição cujo resultado é usado,
receptores struct sem endereço e construção de struct com valor padrão fora de
um destino direto. Chamadas estáticas como Int32.Parse não criam temporário de
receptor.

A leitura histórica continua vinculada ao snapshot oficial Unity Mono
`b6587926c0f146c8b12985744f33a13b39a22d57`. A revisão exata que fabricou o smcs
do editor é UNKNOWN. As referências principais são
[Block.EmitMeta](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/statement.cs#L2032-L2047),
[declaração no parser](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/cs-parser.jay#L6062-L6105),
[temporário durante resolução](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/ecore.cs#L5933-L5957),
[alocação e reutilização durante emissão](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/codegen.cs#L357-L434) e
[switch de string](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/statement.cs#L3706-L3866).

## Validação medida

`reports/round6-slot-model-validation.json` mede todos os **1.682 corpos em
matched** da linha de base 66eea9d. O modelo fonte consegue localizar **1.582**;
os outros **100** são corpos gerados ou sem declaração diretamente mapeada.
Esses 100 nunca são contados como acerto por possuírem assinatura vazia.

Há **1.539 previsões completas e 1.539 assinaturas exatas**, portanto a taxa
condicional de acerto é **1.539/1.539 = 100%**, com cobertura de
**1.539/1.682 × 100%** do inventário matched. Não há desacordo em uma previsão
classificada como completa.

Entre os **43 métodos mapeados mas incompletos**, **36** têm a assinatura igual
e **7** diferem por armazenamento de capturas/closures não modelado. Assim,
**1.575/1.582 × 100%** dos métodos mapeados têm assinatura prevista igual,
ou **1.575/1.682 × 100%** do inventário total. Esses denominadores medem coisas
diferentes e não podem ser trocados. As causas de incompletude são sobrepostas:
27 diagnósticos CS0115, 15 funções anônimas/capturas, um CS0155 e três CS1660,
além dos 100 corpos sem mapeamento. Diagnósticos CS0012 decorrentes das duas
identidades históricas de corlib são separados: os tipos dos slots precisam
continuar resolvidos. Nenhum método incompleto autoriza compilação candidata.

`reports/round6-slot-fixture.json` registra **uma compilação de um assembly de
fixtures independente**, sem compilar candidato do jogo: **23 previsões
completas, 23 assinaturas exatas**. Os controles incluem blocos e declarações
agrupadas, foreach de array 1D/2D e coleção, using de expressão/declaração, lock,
switch numérico/string/aninhado/reutilização, retorno protegido, incremento,
atribuição, receptor struct e Parse estático. A validação de matched detectou a
confusão entre Parse estático e receptor struct; as fixtures detectaram a omissão
do temporário de retorno de construção struct padrão. Ambos foram corrigidos.
O switch de string começou
incompleto e passou após modelar seus temporários. Isso amplia a evidência; não
prova cobertura de toda construção possível da linguagem.

## Solver de declarações

`tools/round6_slot_solver.py` calcula o primeiro e o último uso de cada slot do
IL original, contando loads, stores e tomada de endereço. Alinha famílias de
operações de locais e seus tipos com o IL recompilado e exige correspondência
de identidade com maioria distinta de pelo menos três quartos dos usos
alinhados. Intervalos de uso, sozinhos, não identificam unicamente uma variável.

Para cada bloco lexical, as variáveis identificadas são ordenadas pelos slots
originais e separadas em grupos de slots consecutivos. Cada grupo pode ser
declarado sem inicializador antes da primeira declaração do grupo; os
inicializadores permanecem como atribuições nas posições anteriores. Esse
procedimento permite que o parser forme uma árvore de blocos implícitos
diferente, calculada novamente pelo modelo. Evita antecipar um local que só
deve vir depois dos locais de um bloco filho. Grupos com tipos iguais usam
declaração agrupada. Cadeias de zero são reconstruídas na ordem exata de stores
observada no IL original. Identidades ambíguas e mudanças de escopo que não podem
ser deduzidas unicamente permanecem UNKNOWN e não são inventadas.

O controle Draw_newPlayerLv calculou `int num5, num, num2, num3, num4;` e a
cadeia `num5 = num = num2 = num3 = 0;`. Essa proposta é aprovada somente como
assinatura pelo modelo; o fechamento do corpo depende da compilação e do
verificador oficial. Nomes existentes são preservados e não são tratados como
os nomes históricos originais.

O inventário `reports/round6-slot-solver-inventory.json` calcula os intervalos dos
**269 pendentes smcs** da linha de base, inclusive quando não existe proposta
segura. O motor só recebe patches cuja previsão completa tenha a assinatura
original. Um erro de previsão constatado após compilar rejeita o candidato; a
igualdade do modelo nunca promove estado. Cabeçalhos, IL bruto com tokens
resolvidos, locais, cláusulas de exceção, corpos previamente iguais e metadados
estruturais continuam sendo julgados pelo verificador.

## Alternativas calculadas preservadas

Uma aceitação gulosa pode consumir a forma sintática que permitia calcular uma
alternativa melhor. No controle Draw_newPlayerLv, agrupar as declarações avançou
o prefixo de 56 para 59 antes de experimentar a variante atômica com a cadeia
completa de zeros. A etapa seguinte já encontrava atribuição separada, enquanto
o solver congelado reconhecia a cadeia no inicializador original.

`tools/round6_solver_alternatives.py` conserva todas as alternativas aprovadas
pelo modelo a partir do snapshot congelado, em
`reports/round6-slot-baseline-alternatives.json`. A cadeia original de zeros tem
prioridade sobre o agrupamento simples. O auxiliar de rebase exige o mesmo ID
estável e um intervalo UTF-16 válido da declaração atual; altera somente esse
intervalo. Reaplicar uma alternativa antiga é apenas uma hipótese de fonte:
exige nova previsão completa da assinatura original e as mesmas provas de
prefixo estrito, corpo integral, ausência de regressões e metadados. O módulo
não compila candidatos e não adota fontes.
