# Fila da rodada 10

READY significa que há uma proposta C# e uma fixture própria aprovada. Não
significa corpo do jogo em matched. A previsão de slots, a compilação isolada,
a comparação do corpo inteiro e a verificação oficial continuam obrigatórias.

| Ordem solicitada | Alvo | Preparação |
|---|---|---|
| 1 | `form.SubForm::SetWinCharaTraining(System.Int32,System.Int32)->System.Void` | Composição do ramo tMode2 com saída comum e cadeia 0/1/3. Uma fixture smcs, três compilações e três controles negativos passaram. Proposta em `reports/round10-training-patches.json`. |
| 2 | `main.AppData::.cctor()->System.Void`, inicializador `CGI_COMMON_QUERY` | AST semântico identifica o campo estático; o primeiro stsfld após a divergência tem a mesma identidade no original e na reconstrução. Fixture própria passou nas três compilações. Não se acrescenta construtor explícito. Proposta em `reports/round10-initializer-isolated-manifest-patches.json`. |
| 3 | `form.SubForm::_draw4(kairo.unity.ui.Graphics)->System.Void` | Nove edições de controle/escopos, com 29 hunks diagnosticados. Fixture smcs bateu com controle manual próprio. Corpo do jogo e maxstack continuam UNKNOWN. Proposta em `reports/round10-draw4-patches.json`. |

Há dois `.cctor` com corpo divergente na linha de base do Assembly-CSharp:
`cfg.MyConfig::.cctor()->System.Void` e `main.AppData::.cctor()->System.Void`.
O novo mapeamento pode ser usado para qualquer inicializador com âncoras
unívocas; o resultado para o outro construtor permanece UNKNOWN até a execução.

As previsões completas com assinatura incompatível recusam candidatos.
As incompletas permitem até 12 variantes compiladas por método em cada passe e
24 no conjunto sonda+passe do ciclo; a assinatura real precisa coincidir.
Nenhum estado de método é preenchido por esses calculadores.

As guardas de `Canvas.OnInputUpdate` e `SpForm..ctor(Exception)` também têm uma
família preparada, `root-guard-positive-tail`. O IL da Canvas preserva um retorno
separado para `invalidTouch_` e um if positivo para touchCount/frames. Por isso,
há uma variante que separa o prefixo da disjunção antes de envolver o restante.
O construtor aceita o mesmo cálculo sem exigir MethodDeclarationSyntax.
Três corpos de fixtures independentes bateram com controles escritos à parte,
usando os dois compiladores históricos; três controles negativos foram recusados.
Isso não substitui a aprovação dos métodos do jogo.

Falhas de preparação foram preservadas em `build/round10/`: a primeira execução
das fixtures de guarda passou no construtor smcs e parou antes do caso Roslyn
por um KeyError no driver (`assembly` ausente no manifesto). Após a correção,
as nove compilações das três fixtures passaram. A primeira chamada de treinamento
parou antes de compilar porque o módulo ainda não havia sido promovido; a execução
posterior passou. O primeiro recibo do inicializador foi arquivado como rascunho:
o manifesto da consulta AST misturava caminhos lógicos vivos e congelados,
duplicando declarações. A repetição com manifesto congelado isolado passou.
Nenhuma dessas tentativas compilou o jogo nem alterou seu ledger.

Na preparação de `_draw4`, as três compilações da primeira fixture passaram na
comparação de corpos, mas o registro falhou por usar o nome `max_stack` em vez
de `maxstack`. O driver foi corrigido e repetido em outra pasta; os três corpos
voltaram a bater e o recibo foi gravado. São seis compilações próprias no total,
zero compilações do jogo nessa preparação. A assinatura e o maxstack da fixture
não são apresentados como medição do método do jogo.
