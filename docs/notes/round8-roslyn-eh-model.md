# Round 8 — modelo Roslyn: try/finally void e sobrevivência de locais

Estado: bundle WIP independente, ainda não integrado. Baseline `25b8108`, corpus congelado em `build/round8/baseline/KairoLibrary`. Nenhum fonte oficial, modelo v7, helper ativo ou assembly do jogo foi alterado ou compilado nesta tarefa. A correspondência do corpo candidato de DrawPolygons continua **UNKNOWN**.

## Medição do mesmo corpus

| Medida | v7 congelado | Extensão v8 |
|---|---:|---:|
| Corpos em matched avaliados | 3131 | 3131 |
| Previsões completas | 2228 | 2236 |
| Completas com assinatura exata | 2228 | 2236 |
| Completas erradas | 0 | 0 |
| Mapeados incompletos | 572 | 564 |
| Gerados/sem fonte mapeável | 331 | 331 |

O ganho é de **8 previsões completas**, sem retirada de nenhuma previsão completa v7. Isso mede cobertura do modelo, não novos métodos matched. Os 564 incompletos mapeados incluem 309 cujas listas de tipos coincidem: igualdade isolada não os torna completos. Os outros 331 não recebem crédito por uma previsão vazia.

Os oito novos completos são Graphics.SetClip; as três sobrecargas ResourceManager.DrawSebAnchor; SurfaceBase.OnDraw; Offscreen.RenderStatics; Offscreen.RenderAlterStatics; Offscreen.RenderMultiStatics. Os IDs completos estão em `bundle-manifest.json`. A versão intermediária sem a restrição de reatribuição no finally chegou a 2243 completos; esse número foi retirado. O bundle final tem 2236.

As causas residuais se sobrepõem: 406 ocorrências de stack_scheduler_multiple_use, 129 de captura/função anônima, 88 de exception_lowering, 58 de stack_scheduler_single_use, 39 de foreach_iteration_stack_scheduler e 16 de finally_written_external_local_scheduler. Não são grupos disjuntos nem sua soma é o número de métodos. O verificador do jogo não foi executado para promover estados.

## Extensão estreita e evidência primária

O source snapshot é o Roslyn `4cb203dab89e1542cdf7d88cb6d32767bc0e5460`, cache `analysis/tools/round6-roslyn-source/src/Compilers/CSharp/Portable/CodeGen`. O commit exato de fabricação do executável instalado permanece **UNKNOWN**.

- EmitStatement.cs:676–731: EmitReturnStatement só armazena em LazyReturnTemp quando expressionOpt não é null. Um retorno void protegido pode usar a saída comum sem criar um local de resultado.
- EmitStatement.cs:733–808: EmitTryStatement exige pilha vazia e visita os blocos de try/finally nos escopos de EH. Isso não introduz, por si, uma variável de resultado em método void.
- EmitStatement.cs:594–629: o bloco define todos os seus locais antes de emitir seus comandos; a ordem dos blocos continua sendo a do modelo anterior.
- Optimizer.cs:1419–1431 chama EnsureOnlyEvalStack antes do try, antes do finally e depois do finally. O trecho é:

```csharp
EnsureOnlyEvalStack();
var tryBlock = (BoundBlock)this.Visit(node.TryBlock);
var catchBlocks = this.VisitList(node.CatchBlocks);
EnsureOnlyEvalStack();
var finallyBlock = (BoundBlock)this.Visit(node.FinallyBlockOpt);
EnsureOnlyEvalStack();
```

Optimizer.cs:1549–1554 implementa EnsureOnlyEvalStack por acesso ao local virtual vazio; 1629–1681 verifica as condições de leitura na pilha. A regra modela uma restrição de sobrevivência, não um suposto campo denominado UsedInFinally nessas duas fontes.

O novo ramo aceita somente método void com try/finally sem catch, yield, await, funções locais ou lambdas nesse trecho. As outras dúvidas já existentes continuam marcadas. Um local definido antes do try e lido no finally precisa atravessar a fronteira de pilha e permanece em slot. A mesma prova vale para o elemento do foreach, inclusive quando seu único uso é no finally; ela é aplicada antes da antiga eliminação de leitura única. Referências são vinculadas pelo símbolo semântico, não pelo nome textual.

Uma atribuição ao local dentro do próprio finally pode gerar seu valor apenas no handler, sem atravessar a fronteira. Essa família fica explicitamente incompleta; não se usa a simples presença do nome no finally como prova. As funções com retorno de valor, catches/filtros e outros lowerings não modelados também continuam incompletas.

## Fixtures históricas

Compilador real: pacote Microsoft.Net.Compilers 1.1.0, executável 1.1.0.51109, SHA-256 `53b693536e8a86f35d05d2165ff9b3e0aeaa499ecfd8135a9f99ab4404ca86c3`. As flags e referências são as do manifesto congelado; incluem C# 6, optimize+, debug:pdbonly, deterministic-, unsafe-, checked- e mscorlib Microsoft .NET 2.0. O helper moderno só analisa o fonte; a fixture é compilada pelo csc histórico.

O run final compila duas vezes o fixture independente de 36 métodos: 29 previsões completas, 29 exatas, zero completas erradas. A comparação dos métodos entre ambas as compilações encontra zero diferenças de cabeçalho, assinatura de locais, IL com referências resolvidas e regiões EH. Isso não afirma identidade dos arquivos DLL inteiros.

Nove casos EH positivos cobrem try/finally sem locais, retorno void antecipado, referência lida exclusivamente no finally, usos repetidos, passagem por ref, finally aninhado, elemento foreach só no handler, elemento no try e no handler e break/continue protegidos. Quatro controles negativos são retorno de valor, catch, filtro e variável atribuída apenas no finally. As fixtures de regressão independentes do v7 também estão presentes. Houve três runs do fixture durante o desenvolvimento: seis compilações históricas de assemblies artificiais no total; o recibo final registra as duas do seu próprio run. Nenhuma compilação do jogo.

## DrawPolygons: composição de fonte proposta

ID: `kairo.unity.ui.Graphics::DrawPolygons(kairo.unity.ui.Image,System.Single[],System.Single[],System.Int32[],System.Int32[])->System.Void`.

REA inspecionou o original integralmente: método `0x060002a6`, 1563 bytes de IL, 654 instruções decodificadas, maxstack 10, initlocals true, assinatura local `0x1100006b`. Evidência `ev_cf11e8ee5ceddec7fc7787b2fc6fff1f37a06c5539f765c4cf7bd5608c09ff89`. O dump completo está em rea-graphics-original.json; o extrato verificável em source-evidence.json.

Há dois finally: try 679–1217 / handler 1217–1225; try 84–1555 / handler 1555–1562. No handler interno, o original emite:

```text
IL_04c1: ldloc.s 21
IL_04c3: callvirt Graphics/RenderPass::End()->Void
IL_04c8: endfinally
```

O fonte composto associa o slot 21 ao renderPass2: a prova de sobrevivência vem dessa leitura no finally, após o local ter sido definido no corpo anterior ao try.

As quatro reescritas recuperam a hipótese registrada no round 7:

1. num8 e num9 saem do bloco pai e passam ao inicializador conjunto de um for; incrementos e condição mantêm sua ordem de avaliação no corpo.
2. O foreach de RenderPass vira for indexado sobre o array nomeado renderPass, sem segunda cópia sintética.
3. O primeiro foreach de indicies vira for com índice próprio e leitura direta do parâmetro.
4. O segundo foreach recebe um índice distinto, também lendo diretamente o parâmetro.

O modelo novo prevê **35 tipos completos, exatamente na ordem da assinatura original**. O original guarda GetRenderPass em local 9; a reconstrução atual usa o 19. O original não possui local Int32[] para copiar indicies, e conserva índices nomeados separados. Esses fatos sustentam a hipótese de escopo/laços, mas não demonstram o fonte histórico.

drawpolygons-source-patch.json contém o patch completo, offsets UTF-16, hash do fonte do baseline, assinatura original e ambas as previsões. Seu potencial é um corpo de 1563 bytes; ganho real em matched: **UNKNOWN até compilar e passar no verificador completo**. Nenhum byte original, classe gerada ou tabela do assembly é escrito por esse bundle.

## Uso e integração

O client portátil encontra a raiz pelo tools/compiler_probes.py e lê Round8RoslynModel.cs ao lado do próprio módulo. Seu cache fica em build/round8/roslyn-model-cache. A API permanece SlotModelClient(manifest).predict / predict_all / solve. O solver delegado continua sendo tools/round7_roslyn_solver.py, sem modificação.

O manifesto do bundle fixa hashes dos arquivos, fontes do corpus, recibo, manifesto, fixtures e evidência. A integração deve copiar apenas os arquivos novos indicados por expected_publication e atualizar o protocolo em janela autorizada pelo agente principal. O bundle não substitui o v7 por conta própria. Modelos incompletos continuam descritos como incompletos; a política de orçamento das compilações deles pertence ao motor da rodada 8.

REA CLI 6.5.0 respondeu, e a inspeção managed pelo MCP funcionou. O doctor global indicou ferramentas opcionais ausentes e drift de cadastro/skill; nenhuma instalação ou reparo foi feita. Isso não impediu a evidência managed usada acima. A comparação do jogo, publicação do bundle e adoção de fonte são tarefas do agente principal.
