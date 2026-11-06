# Rodada 7: modelo de slots Roslyn e iterador FastVector

O modelo usa somente sintaxe C# e símbolos das referências do manifesto. Não lê o IL original para prever uma assinatura, não chama `Emit` e não compila candidatos do jogo. O solver usa acessos a locais no IL original apenas para calcular a permutação desejada e as posições de declarações; a proposta ainda precisa passar pelo modelo e pelo verificador completo.

## Fonte primária e regras implementadas

O compilador de destino é o pacote Microsoft.Net.Compilers 1.1.0, executável 1.1.0.51109, SHA-256 `53b693536e8a86f35d05d2165ff9b3e0aeaa499ecfd8135a9f99ab4404ca86c3`. Foi consultado o snapshot oficial Roslyn `4cb203dab89e1542cdf7d88cb6d32767bc0e5460`; a correspondência exata desse commit com a fabricação do executável permanece UNKNOWN.

- [EmitBlock, EmitStatement.cs:594–630](https://github.com/dotnet/roslyn/blob/4cb203dab89e1542cdf7d88cb6d32767bc0e5460/src/Compilers/CSharp/Portable/CodeGen/EmitStatement.cs#L594-L630): os locais de cada bloco são definidos em ordem antes dos comandos; blocos filhos são visitados durante a emissão dos comandos. O modelo intercala a entrada nos blocos com a alocação de temporários de emissão.
- [DefineLocal, EmitStatement.cs:1375–1452](https://github.com/dotnet/roslyn/blob/4cb203dab89e1542cdf7d88cb6d32767bc0e5460/src/Compilers/CSharp/Portable/CodeGen/EmitStatement.cs#L1375-L1452): constantes e locais selecionados pelo scheduler da pilha não ganham slot. Um local inicializado e devolvido diretamente no comando seguinte é um caso implementado de eliminação; usos por endereço impedem essa eliminação.
- [RecordVarRead/RecordVarWrite, Optimizer.cs:1616–1724](https://github.com/dotnet/roslyn/blob/4cb203dab89e1542cdf7d88cb6d32767bc0e5460/src/Compilers/CSharp/Portable/CodeGen/Optimizer.cs#L1616-L1724): a altura da pilha deve coincidir com a altura da declaração, salvo quando o topo já contém o mesmo local. O modelo prova rejeições simples pelo primeiro uso a altura diferente de zero e por endereço; casos restantes são explicitamente incompletos. Não implementa todo o scheduler, especialmente o filtro global de intervalos que se cruzam.
- [LocalSlotManager.cs:121–184](https://github.com/dotnet/roslyn/blob/4cb203dab89e1542cdf7d88cb6d32767bc0e5460/src/Compilers/Core/Portable/CodeGen/LocalSlotManager.cs#L121-L184) e [KeyedStack.cs:14–37](https://github.com/dotnet/roslyn/blob/4cb203dab89e1542cdf7d88cb6d32767bc0e5460/src/Compilers/Core/Portable/Collections/KeyedStack.cs#L14-L37): temporários livres são reutilizados por assinatura, em pilhas LIFO. O modelo libera temporários anônimos; não libera locais nomeados ao sair de um bloco.
- [LocalRewriter_ForEachStatement.cs:68–118](https://github.com/dotnet/roslyn/blob/4cb203dab89e1542cdf7d88cb6d32767bc0e5460/src/Compilers/CSharp/Portable/Lowering/LocalRewriter/LocalRewriter_ForEachStatement.cs#L68-L118): o enumerador é sintético e a variável de iteração pertence ao bloco interno de cada iteração. O modelo inclui os enumeradores e os temporários array/índice e elimina a variável de iteração somente no caso estreito de um uso na pilha.

O frontend moderno Microsoft.CodeAnalysis serve para obter AST, tipos e símbolos; a política de slots acima é implementada no helper, sem usar a política de emissão do frontend moderno. Os tipos dos locais seguem a representação Cecil, incluindo os nomes dos parâmetros genéricos (`T`, em vez de `!0`) e os argumentos dos tipos genéricos contenedores. A identidade de métodos mantém a representação posicional `!0`/`!!0` exigida pelo verificador.

## Validação congelada

Baseline: `cdd0f7f3b8dda09f296a7cfa16cd00236a3346ae`, KairoLibrary, recibo SHA-256 `5cfc78e91daa0d427f9962b2a5e83a6b2ffa7015ee66a79fbdc354967d2bcfd3`.

| Medida | Corpos |
| --- | ---: |
| Matched avaliados | 3117 |
| Fonte mapeado | 2786 |
| Predição declarada completa e exata | 2214 |
| Predição declarada completa e errada | 0 |
| Fonte mapeado, modelo incompleto | 572 |
| Gerados ou sem declaração mapeada | 331 |
| Assinatura exata, incluindo predições incompletas | 2531 |

A precisão no subconjunto completo é 2214/2214; a cobertura completa nos matched é 2214/3117. Os 331 sem mapeamento não recebem crédito por listas vazias coincidentes. O resultado não demonstra um modelo completo de todo o Roslyn: as 572 previsões parciais continuam incompletas e não aprovam compilações. O relatório JSON preserva a causa por método.

Na fixture independente do csc 1.1, 15 predições completas foram exatas e 3 ficaram incompletas. Inclui ordem de blocos, declaração de índice no pai, locais nomeados sem reúso, retorno sobre pilha, endereço de local, temporário de atribuição, foreach de array, enumeradores sequenciais e MoveNext simples. A primeira tentativa de compilação falhou porque `/noconfig` estava dentro do response file e o csc carregou referências padrão duplicadas; o comando foi corrigido passando `/noconfig` diretamente. Nenhum desses ensaios compilou fontes do jogo.

Uma ampliação intermediária que tratava o primeiro uso da variável de foreach a altura diferente de zero como prova geral de slot real foi **REFUTADA**: a validação marcou 2221 previsões completas, das quais 8 erradas. A regra foi retirada. A ampliação adotada é estreita: array de struct cuja variável de iteração é usada por endereço; os demais casos ambíguos continuam incomplete. Um retorno de valor dentro de foreach com enumerador também fica incompleto por exigir o local de retorno protegido. A fixture acrescentada cobre array de struct e locais usados em inicializador de array; este último carrega array e índice antes de ler o valor.

Em `SurfaceManagerBase.GetPaintRect(Vector2[])`, a transformação `for` para `foreach` já tinha a assinatura prevista correta, mas não era aprovada: faltavam duas provas no modelo. A variável `Vector2` precisa de slot porque seus campos são usados por endereço. Os locais `num7` e `num8`, usados uma vez no inicializador `new int[4] { ... }`, também precisam de slots porque a emissão da atribuição ao elemento carrega array e índice antes do valor. As duas provas foram incorporadas e cobertas pelas fixtures `ArrayStruct` e `ArrayInitializer`. O candidato passou a ser `complete` com os onze tipos originais, sem compilar o jogo nesta análise.

## Solver de declarações

O solver calcula correspondências entre slots pelos acessos alinhados, exigindo maioria distinta de pelo menos 3/4 e tipos compatíveis. Não considera o alinhamento prova de correspondência de corpos. Além do agrupamento e permutação dentro do bloco, calcula duas mudanças de escopo:

1. Içar a declaração de um índice `for` para um bloco ancestral, deixando sua inicialização no mesmo comando. Para Canvas.EndAlterStaticRender, isso permite alocar `i:Int32` antes de `meshManList_:MeshManager[]`; o modelo aprovou duas posições ancestrais.
2. Criar um bloco em torno da declaração de índice e do `while` seguinte, sem usos posteriores do índice. Para MeshBuilder.AddQuad(Vector3[],Single,Single,Single,Single,Color), isso deixa o temporário `Layer` da atribuição de `currentLayer` ser alocado antes do índice. O modelo aprovou uma proposta.

Essas três propostas têm assinatura prevista igual à original. Correspondência do corpo inteiro e ganhos são UNKNOWN nesta nota; pertencem ao recibo do motor após compilação e verificação. Nenhum arquivo de estado foi editado por esta análise.

## FastVector: o campo já existe nos dois lados

ID: ``kairo.unity.util.FastVector`1/<GetEnumerator>d__42::MoveNext()->System.Boolean``.

O original grava `<>7__wrap1:!0[]` no offset 35, depois de carregar `this.<>4__this.elementData`. O recompilado grava o array explícito em `stloc.1` no offset 34 e depois o mesmo campo `<>7__wrap1` no offset 37. O campo `<>7__wrap2:Int32` guarda o índice do foreach. Ambos são usados novamente depois do `yield`; a cópia sintética do array e o índice precisam sobreviver à suspensão.

Portanto a variável que precisa ser içada é a cópia **sintética** do array criada pelo lowering de foreach. O local explícito `T[] array` recuperado no C# é usado somente para inicializar essa cópia, antes da primeira suspensão, e não precisa ser içado. Os metadados podem passar mesmo quando o recompilado acrescenta esse local ao MoveNext: os campos sintéticos necessários continuam sendo os mesmos.

Hipótese de correção: trocar `T[] array = elementData; foreach (T val in array)` por `foreach (T val in elementData)`, mantendo o `yield return val`. A origem da cópia sintética é o bloco de lowering do foreach; seu escopo cobre o laço inteiro, incluindo a retomada após yield. Não se declara campo ou classe gerada no fonte.

O modelo estreito do MoveNext prevê `Int32, T[], T` no fonte atual e `Int32, T` na proposta; esta última coincide com a assinatura original. A fixture independente confirma a fórmula para o padrão anterior ao ajuste. A correspondência byte a byte do candidato do jogo continua hipótese até o verificador. [IteratorMethodToStateMachineRewriter.cs:62–98](https://github.com/dotnet/roslyn/blob/4cb203dab89e1542cdf7d88cb6d32767bc0e5460/src/Compilers/CSharp/Portable/Lowering/IteratorRewriter/IteratorMethodToStateMachineRewriter.cs#L62-L98) mostra o local de estado que inicia a assinatura de MoveNext e a reescrita do corpo original; o inventário e os dois ILs completos estão no relatório da análise.

## Artefatos

- `tools/Round7RoslynModel.cs`: modelo independente, SHA-256 `9368a5354d647ff21cec27e951fa30bbf107129958b2a98650125987fd26b211`.
- `tools/round7_roslyn_model.py`: cliente persistente e gate das propostas, SHA-256 `a43c460a3b2c260eb10b98ce5fd8fda859261144e80098be1958dbe6b5ca26cf`.
- `reports/round7-roslyn-model-validation.json`: validação de todos os corpos da baseline, com os matched separados.
- `reports/round7-roslyn-fixture.json`: resultado dos ensaios independentes.
- `reports/round7-roslyn-solver-cases.json`: cálculo dos dois exemplos pedidos.
- `reports/round7-iterator-analysis.json`: ILs completos, campo exato e predições do iterador.
- `reports/round7-roslyn-patches.json`: quatro propostas de fonte aprovadas pela predição de assinatura, ainda sujeitas ao verificador.
