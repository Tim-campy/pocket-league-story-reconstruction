# Rodada 7: cobertura de capturas no modelo smcs

O modelo desta rodada está em `tools/Round7SlotModel.cs`, acessado por
`tools/round7_slot_model.py`. Os arquivos da rodada 6 permanecem congelados.
O auxiliar moderno analisa sintaxe e símbolos; não chama `Emit` e não compila
candidatos do jogo. O cliente preserva `SlotModelClient.predict` e `.solve`.
O solver de declarações foi copiado para `tools/round7_slot_solver.py`, sem
ampliar suas regras conservadoras nesta frente.

## Regra confirmada no compilador histórico

O snapshot consultado é o Unity Mono
`b6587926c0f146c8b12985744f33a13b39a22d57`, disponível localmente em
`analysis/tools/round5-mono-source/mcs/mcs/`. A revisão exata que produziu o
executável instalado continua UNKNOWN.

* [LocalInfo.ResolveVariable, statement.cs 1225–1239](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/statement.cs#L1225-L1239)
  começa com `if (HoistedVariableReference != null) return;`: um local capturado
  não recebe um slot ordinário durante a emissão dos metadados locais.
* [ExplicitBlock.CreateAnonymousMethodStorey, statement.cs 2375–2398](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/statement.cs#L2375-L2398)
  cria no máximo um storey por bloco explícito. A associação não usa o filho
  implícito criado pelo parser depois de um comando.
* [ExplicitBlock.Emit, statement.cs 2403–2415](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/statement.cs#L2403-L2415)
  chama `am_storey.EmitStoreyInstantiation(ec)` antes de emitir o bloco.
* [AnonymousMethodStorey.EmitStoreyInstantiation, anonymous.cs 278–322](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/anonymous.cs#L278-L322)
  constrói a instância e só então executa `Instance = new LocalTemporary(...);`
  e `Instance.Store(ec)`. O slot da instância é alocado durante a emissão,
  depois dos locais ordinários, na entrada do bloco explícito correspondente.
* [anonymous.cs 133–166](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/anonymous.cs#L133-L166)
  usa um contador estático `unique_id` e `ID = unique_id++`.
  [MakeName, anonymous.cs 23–25](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/anonymous.cs#L23-L25)
  escreve o ordinal em hexadecimal maiúsculo (`ToString("X")`).
* [anonymous.cs 1421–1423](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/anonymous.cs#L1421-L1423)
  distingue captura apenas de `this`, ausência de captura e captura de
  variáveis. As duas primeiras não exigem local de storey.

A previsão encontra referências de locais e parâmetros dentro das funções
anônimas, distingue os símbolos declarados dentro delas, elimina somente os
locais capturados e calcula os storeys pela ordem dos arquivos e declarações
do fonte. Os nomes gerados são calculados; não são lidos da assinatura desejada
nem escritos como classes C# na reconstrução. Parâmetros capturados pertencem
ao bloco do corpo do método. Capturas de locais pertencem ao bloco explícito
da declaração. Funções anônimas aninhadas continuam incompletas.

## Diagnósticos provocados pelas duas identidades de corlib

O Roslyn moderno lê referências históricas com identidades de corlib distintas.
Isso não altera o smcs usado para compilar o jogo. Foram demonstrados três
diagnósticos espúrios em fixtures independentes compiladas pelo smcs real:

* CS0115 em `ToString`/`GetHashCode` de uma classe que deriva de um tipo da
  KairoLibrary. A exceção é limitada a overrides sem parâmetros, com esses
  nomes e os tipos de retorno `System.String` e `System.Int32`.
* CS0155 ao capturar `kairo.unity.util.NotPermitException`. A exceção exige que
  a cadeia de tipos resolvida do catch alcance `System.Exception`.
* CS1660 em `new java.lang.JThread(delegate { ... })`: o Roslyn apresenta o
  overload `JRunnable` e o parâmetro `System.Threading.ThreadStart` do outro
  overload como `ErrorType`, devido à corlib referenciada pela KairoLibrary.
  A exceção exige um parâmetro naquele índice cujo tipo resolvido, ou o tipo
  com o mesmo nome completo na corlib fornecida, tenha `Invoke` e derive de
  `System.MulticastDelegate`. O diagnóstico inicialmente abrangia um nó
  `Argument`; a busca pelo nó mais interno recuperou a função anônima correta.

CS0012 já era separado no modelo anterior. Os tipos dos slots continuam
obrigatoriamente resolvidos. Nenhuma exceção é uma autorização para ignorar
erros na compilação real: o gate final continua sendo o compilador histórico
e o verificador oficial. Três controles negativos (`MissingOverride`, catch
de classe que não é exceção e argumento de classe que não é delegate)
permaneceram incompletos, com os diagnósticos correspondentes.

## Medição congelada

`reports/round7-slot-model-validation.json` usa a linha de base da rodada 7,
congelada em `build/round7/baseline/Assembly-CSharp`. O recibo tem SHA-256
`ccb120ce8749d78d03a01b3548f35f019a85ae05067c8db0eae00249f018ba08`.

| Medida | Contagem |
|---|---:|
| Corpos em matched | 1697 |
| Mapeados a declaração de fonte, incluindo funções anônimas | 1617 |
| Previsões completas e exatas | 1617 |
| Previsões completas incorretas | 0 |
| Corpos gerados ou não mapeados | 80 |
| Antigos 43 incompletos encontrados | 43 |
| Antigos 43 agora completos e exatos | 43 |
| Antigos 7 desacordos de storey agora exatos | 7 |

A taxa condicional é **1617/1617 = 100%**; a cobertura do inventário de corpos
matched é **1617/1697**. Os 80 não mapeados permanecem `complete=false`;
assinatura vazia não os transforma em acerto. Também não houve desacordo nas
previsões completas dos corpos pendentes contra o assembly recompilado da
linha de base. Isso valida a previsão do fonte corrente, não prova que o corpo
pendente corresponda ao original. Seis corpos pendentes continuam incompletos:
quatro sem declaração diretamente mapeada e dois com CS0030 não relaxado.

Os sete storeys corrigidos são os de `AppData.DrawFukidashiKimochi`,
`AppPlugin.ShowPurchaseLog`, `AppPlugin.LaunchMailer`,
`WebBoxPlugin.SetViewRect`, `WebBoxPlugin.LoadUrl`, `WebBoxPlugin.SetVisible`
e `StoreBase.SetRestoreRequest`. Os ordinais calculados foram respectivamente
1 a 7. O storey 0 é gerado antes deles pelo fonte de `SubForm`.

`reports/round7-slot-fixture.json` registra uma compilação independente com o
smcs instalado, **40 previsões completas, 40 exatas, zero incompletas**, mais
três controles semânticos negativos feitos apenas na análise do fonte. Inclui
captura de parâmetro, captura de local, dois blocos capturados, apenas `this`,
ausência de captura, inicializador de objeto, local explícito dentro de função
anônima e os três diagnósticos de identidade de corlib. São 35 métodos declarados
e cinco corpos anônimos derivados do fonte. Os 23
controles de slots da rodada anterior são reutilizados no mesmo assembly de
fixture. `tools/test_round7_slot_solver.py` passa os quatro testes conservadores
de intervalos, identidade por tipo, edição UTF-16 e rejeição de previsão
incompleta. Nenhum candidato do jogo foi compilado nesta frente e nenhum
estado de método foi promovido por ela.

O auxiliar congelado tem SHA-256
`79df4cafdebe4bdd9e4a93d567416e08a38cd62abec844945cb102fc0d33c287`.
O cliente tem SHA-256
`041ef2e413e7750550a287ba4b64d4a8d99b6749c6aa19e9500437ca556e8f2a`.

## Ampliação para corpos anônimos hospedados diretamente no tipo

A primeira ampliação resolveu os 43 antigos incompletos com 1597/1597 previsões
completas exatas, deixando 100 corpos matched sem mapeamento. A segunda foi
testada isoladamente por `tools/round7_slot_generated_experiment.py` antes de
ser integrada. Nenhuma execução do motor usou uma alteração de modelo ainda
não revalidada; a atualização foi coordenada entre os dois probes e a passagem
completa.

O contador de métodos anônimos é separado do contador de storeys:
[anonymous.cs 1371 e 1440–1441](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/anonymous.cs#L1371)
declara outro `static int unique_id` e o incrementa em `MakeName(..., "m", ...)`.
O nome inclui o método de origem quando a função é hospedada diretamente no
tipo. O modelo não escreve esse método no fonte; usa o corpo anônimo existente.

A ordem somente textual dos tipos foi testada e refutada: previa
`BillingManager.<Provisioning>m__E`, enquanto o smcs recompilado escreve
`m__10`. `MyBilling` deriva de `BillingManager`, que deriva de `StoreBase`:
os dois corpos anônimos de `StoreBase` são emitidos antes do de `BillingManager`.
[class.cs 1058–1075](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/class.cs#L1058)
registra o tipo após resolver suas bases na ordenação topológica;
[RootContext.RegisterOrder e EmitCode](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/rootcontext.cs#L208)
acumula essa ordem e chama `tc.EmitType()` nela (linhas 358–362).
O modelo calcula essa precedência a partir dos símbolos de tipos no fonte.

Um segundo erro intermediário identificou `WebBoxPlugin.<Create>m__7` como
retornando Boolean, usando o tipo inferido da expressão retornada. O delegate
alvo é `IApplication.UIMethod`, cujo `Invoke` retorna Object; o smcs faz boxing.
A assinatura gerada agora vem do delegate alvo, inclusive quando é necessário
resolver seu tipo na corlib fornecida. A versão final do experimento localizou
**23 IDs existentes e previu 23 assinaturas exatas**: 20 já matched e três
pendentes. Não houve lookup de IDs originais para construir os nomes previstos.

Os três pendentes mapeados são `FriendForm.<_update>m__17`,
`FriendForm.<_update>m__18` e `LanguageForm.<Update>m__20`. Funções anônimas em
storeys, funções anônimas aninhadas, iteradores e corpos de expressão não
receberam previsão gerada nesta ampliação; continuam sem autorização do gate.

O modelo também precisou distinguir o temporário de inicializador de objeto
de uma atribuição de campo cujo resultado é usado.
[NewInitialize.Emit, expression.cs 9670–9704](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/expression.cs#L9670)
aloca um `LocalTemporary` do tipo construído e emite os membros sobre ele.
O modelo suporta apenas inicializadores simples de objeto, sem inicializador
aninhado ou coleção; avalia os argumentos, aloca esse temporário e avalia os
valores dos membros sem inventar temporários para cada atribuição.

Em `FriendForm.<_update>m__17`, a previsão corrente é
`Friend, Exception, FriendForm`, exatamente como a reconstrução da linha de
base. O original exige `Friend, FriendForm, Exception`. As fixtures confirmam
a causa: o inicializador recuperado produz o temporário do objeto depois dos
metadados de catch; declarar e preencher o objeto explicitamente produz o slot
nomeado antes do catch. Isso explica a assinatura, mas a confirmação do corpo
do jogo depende da transformação e do verificador executados pelo motor.

Os resultados negativos intermediários foram preservados como diagnóstico:
na primeira passagem havia 40 dos 43 antigos casos completos; os outros três
continuavam recusados por CS1660. A primeira fixture tinha 32 previsões
completas exatas e uma incompleta. Depois da correção de mapeamento do nó e
identidade de delegate, passou a 33/33, com os três controles negativos
continuando recusados. O primeiro experimento de corpos gerados produziu
23 IDs, dos quais 21 existiam; havia 20 previsões completas e 18 completas
exatas. Após ordenar as bases e modelar os inicializadores, eram 23 previsões
completas, 22 IDs existentes e 22 exatas. A correção da assinatura pelo delegate
alvo levou a 23 IDs existentes e 23 previsões completas exatas. Essas etapas
experimentais não autorizaram candidatos do jogo; a versão principal só foi
atualizada depois da confirmação das hipóteses e passou pela validação completa.
