# Moldes de controle do smcs: extração independente, rodada 9

O extrator recebe apenas o DTO de um método CIL. Não recebe fonte, AST, MDB, árvore esperada ou estado `matched`. O fonte original continua somente leitura. O plano de emissão é evidência de estrutura de controle; o verificador de corpos continua sendo o único juiz de correspondência.

## Identidade e API

Artefato primário: `Assembly-CSharp.dll`, SHA-256 `3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949`, MVID `48ce9f89-73d2-40be-bb24-22c807c04106`, confirmado pelo REA. O recibo WIP `rea-artifact-identity.json` registra o ID da evidência REA. O DTO do ProbeInspector é somente leitura; a API verifica que não foi alterado.

- `extract_il(methodDTO) -> IR`: árvore parcial, plano, folhas CIL, papéis das arestas, cobertura e razões explícitas.
- `canonical_plan(IR_or_events) -> list`: compara planos independentes de AST e CIL, renomeando rótulos e condensando somente folhas consecutivas. Mantém todos os branches, larguras, polaridades, ordem dos alvos de `switch`, retornos e fronteiras EH.
- `canonical_tree(IR) -> dict | None`: lê exclusivamente a árvore. Nunca consulta o plano. Retorna `None` se a árvore estiver incompleta.

`emission_plan_complete` significa cobertura integral do fluxo emitido. `tree_complete` exige também moldes regionais/ownership reconhecidos. `complete` exige ambos e cobertura exata. Uma lista de instruções ou eventos não vira uma árvore completa por estar integralmente coberta. Comparar `None == None` não é validação de árvores.

## Fonte do compilador

Repositório `Unity-Technologies/mono`, revisão fixada `b6587926c0f146c8b12985744f33a13b39a22d57`, diretório `mcs/mcs`. A revisão exata que fabricou o executável smcs instalado permanece **UNKNOWN**. Os moldes relevantes também foram exercitados no smcs instalado, SHA-256 `26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0`.

| Arquivo | SHA-256 |
|---|---|
| `statement.cs` | `063a437095e1d608d045383d76291c9359e9a98220eea7750018a3d42712437a` |
| `expression.cs` | `3e6392384936d08bd7142f8bf68d21ae4f8c429f9c8b0ac099da97bff53c74af` |
| `constant.cs` | `fbedc8d221b8d1f59969acb5122b093319ba1131ab42dbcf5e43011890ee31fd` |

Os caminhos locais e trechos numerados estão em `compiler-source-evidence.json`. Os links para a revisão são [statement.cs](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/statement.cs) e [expression.cs](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/expression.cs).

## Moldes e limites explícitos

| Construção | Emissão comprovada | Recuperação atual |
|---|---|---|
| `if/else` | `statement.cs:247-264`: condição para false; true; `br join` se o braço true pode concluir; false | Regiões físicas simples; retém `br` inclusive para a instrução seguinte. Captura do stack delimita o valor testado. |
| Guarda com `return` | `statement.cs:770-782`: valor, `ret`; em região protegida, `stloc TemporaryReturn` e `leave ReturnLabel` | Guarda normal reconhecida, classe emit-equivalente explícita para else após braço terminal. Retorno protegido mantém dados EH, mas árvore lexical EH incompleta. |
| `while` | `statement.cs:440-461`: `br test`; corpo; teste com branch verdadeiro para corpo. `while(true)` corpo+`br` | Teste único com prova de dominadores; forma pretested. |
| `do` | `statement.cs:322-351`: corpo; condição; branch verdadeiro para corpo | Forma posttested com backedge cujo destino domina a origem. |
| `for` | `statement.cs:568-619`: inicializador, `br test`, corpo, incremento, teste | Emissão pretested; `continue` ao incremento não é confundido com `continue` ao teste. Quando um join também pode ser incremento, fica incompleto. |
| `foreach` array | `statement.cs:5187-5320`: array e contador ocultos, laço indexado | Recupera a estrutura pretested sem afirmar a palavra-chave original. Os loads/stores ocultos continuam nas folhas. |
| `foreach` enumerator | `statement.cs:5674-5675;5790-5815`: enumerator, teste MoveNext, descarte EH quando aplicável | Plano integral, árvore lexical incompleta junto com EH. |
| `break/continue` | `statement.cs:1117-1120;1149-1151`: `br` ou `leave` para LoopEnd/LoopBegin | Ownership por alvos dos loops reconhecidos e profundidade lexical. Destino desconhecido não vira break/continue por aproximação. |
| `switch` | `statement.cs:3831-3844`: cópia do seletor em temporário; dispatcher depende dos casos | Tabela e todos os alvos retidos. Não adivinha rótulos de casos, constante subtraída ou switch esparso a partir de ifs. Árvore incompleta. |
| Ternário | `expression.cs:4196-4215`: condição false, true, `br` longo, false; temporário extra se resultado interface | Ternário simples identificado por união da pilha com valor adicional. Os temporários permanecem nas folhas. |
| `&&/||` como valor | `expression.cs:3585-3596`: branch curto-circuito, right, **`br.s`**, constante0/1 | Padrão simples reconhecido; diferença de largura para ternário não é apagada. |
| `&&/||` como condição | `expression.cs:3450-3494`: branch_on_true decide os alvos intermediários | Predicados/arestas preservados. `&&` simples pode corresponder a ifs aninhados emit-equivalentes; formas compostas não provadas ficam incompletas. |
| Ponto flutuante | `expression.cs:3500-3565`: seleção de `.un` em complementos | Opcode e tipos da pilha preservados; não substitui operador relacional por complemento que altere NaN. |
| `try/catch/finally` | `statement.cs:4697+;4761+`: emissão Reflection.Emit e cláusulas EH | Todas as fronteiras, cláusulas e instruções estão no plano. Árvore lexical incompleta; não apaga leaves implícitos nem `endfinally`. |
| `using/lock` | `statement.cs:4899+;5014+;4157-4177`: aquisição e try/finally/disposal/Monitor | Plano integral; não infere palavra-chave só por chamadas Dispose/Monitor, pois lowering manual pode coincidir. Árvore incompleta. |

Um branch de saída de laço não é backedge. A classificação de backedge exige que o destino domine a origem. `while/for` podem ter teste fisicamente no fundo que é header de dominadores por causa do salto inicial. Handlers têm entradas separadas na análise da pilha; a análise de dominadores normais não inventa arestas excepcionais. SCCs irredutíveis e regiões não suportadas produzem razões de incompletude.

## Equivalências de emissão e controles negativos

A fixture independente tem 37 métodos e foi compilada somente como brinquedo com o smcs instalado. Os 39 planos têm cobertura exata de instruções e bytes, e 27 árvores estão completas nesta versão. Nenhum assembly do jogo foi compilado.

Cinco pares possuem `raw_body_hex` inteiro literalmente igual: `While/For`, `Guard/GuardElse`, `AndIf/NestedAndIf` `CommonReturn/CommonIfElse` e `ForContinueWork/ForContinueWorkAsElse`. Isso prova a indistinguibilidade desses exemplos; não recupera a grafia original nem autoriza a apagar instruções em outros corpos. As classes e seus limites aparecem no IR. `if` com else vazio tem corpo diferente de `if` sem else, e seu `br` para a instrução seguinte é mantido. Controles também confirmam que a comparação distingue `br` de `br.s`, branch removido, polaridade invertida e `.un` para float.

A primeira versão do parser confundiu `br` para o incremento de um `for` com join de if/else no brinquedo `ForContinue`. A hipótese de ownership estava **REFUTADA** nesse caso. O parser passou a declarar `for-increment-versus-if-join-ambiguous` e árvore incompleta; o teste continua no conjunto. O erro não foi testado em build do jogo nem adotado como correção de fonte.

O primeiro lançamento da fixture após edição falhou por BOM UTF-8 lido como caracteres Windows-1252. O script foi salvo em UTF-8 e executado novamente; essa falha ocorreu antes de qualquer compilação da fixture daquela execução. É falha de harness, não evidência do compilador.

## Validação bruta de seções EH

`ProbeInspector.cs:176-207` guarda o layout das seções em `extra_sections`. Esse campo não significa uma seção desconhecida: o próprio inspector só aceita `kind & 0x3f == 1`, a tabela EH. O extrator valida isso novamente nos bytes reais de `raw_body_hex`: padding, cabeçalho small/fat, tamanho e flags de cada cláusula, intervalos try/handler, catch token resolvido por `metadata_operands`, encadeamento de seções e consumo até o último byte.

Medição somente leitura no snapshot pós-ParteA: original **130/130 métodos**, **244 cláusulas**; recompilado **130/130**, **244 cláusulas**; zero falhas e nenhum byte restante não explicado. O layout é conservado como evidência. A validação do layout não completa automaticamente a árvore EH.

## O que não foi afirmado

A extração de eventos não prova que as árvores source e CIL coincidem. A validação pareada independente é feita pelo driver da rodada, com árvores source geradas sem ler o próprio IL. As contagens globais desse driver devem citar o hash da versão dos extratores executados. A correspondência estrutural não promove um método a matched; corpo, header, locais, maxstack e EH ainda são julgados pelo verificador byte a byte.

## Valida??o pareada anterior ? publica??o

No snapshot `prototype07`, o driver independente examinou 1.788 corpos matched: 1.422 planos pareados completos, todos iguais; 1.253 ?rvores pareadas completas, todas iguais. A cobertura estrutural pareada ? 1.253/1.788 (70,08%); a precis?o entre casos completos ? 1.253/1.253 (100%). Casos incompletos n?o foram contados como acertos nem como erros. Entre os 164 corpos pendentes, incluindo um held, havia 50 ?rvores CIL completas, 43 source completas e 27 pareadas: 14 iguais, 13 diferentes. As diferen?as dos pendentes s?o diagn?sticos, n?o evid?ncia de que uma corre??o de fonte j? passou no verificador.

Essa medi??o usou o m?dulo WIP SHA-256 `3dea894be045cacbeb8ba93237994ca3a0258be6a7232a90ee365c435b691742`. O snapshot p?blico altera somente nomes de import/table path; 39 IRs da fixture foram comparados integralmente entre ambos e deram igualdade. O driver final deve vincular novamente os hashes das APIs p?blicas.
