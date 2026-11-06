# Rodada 9, duas famílias Roslyn calculadas

Estado: **READY para o oráculo; corpos do jogo UNKNOWN**. Nenhuma compilação do jogo, adoção, edição do ledger ou alteração dos modelos foi executada nesta preparação. As sete propostas preservam o texto bruto, inclusive CRLF e eventuais LF existentes, e usam offsets UTF-16 com rebase único do método completo. Corpus: `build/round9/cycles/b10/after/KairoLibrary`, após a fila B. O dump original tem SHA-256 `d3340a2a774d1c4a604c750ca0e7b36b5fbb15da96a4d0dabc60033066dc844a`.

## 1. Conversão explícita Single para Single, por ocorrência

Família `d9-explicit-float-identity-parameter`; entry `AddRound9FloatingIdentityRules`; helper `tools/Round9FloatingIdentityRules.cs`. ABI idêntica aos demais partials de `Round7TransformSyntax`. O motor deve acrescentar ao input o retorno de `round9_float_identity_hints.float_identity_conversion_hints(original, rebuilt, original_offset, rebuilt_offset)`.

O hint exige `conv.r4` no original, imediatamente após a carga de um parâmetro cujo tipo é `System.Single`; distingue a numeração de parâmetros de métodos estáticos/instância. O helper exige `IParameterSymbol` Single não constante e ordinal correspondente. Propõe `(float)(parâmetro)` em uma ocorrência do comando mapeado. Recusa cast Single existente, atribuição ao parâmetro, ref/out, incrementos e `nameof`; não troca operadores relacionais ou expressões de ponto flutuante.

Evidência primária: `analysis/tools/round6-roslyn-source/src/Compilers/CSharp/Portable/CodeGen/EmitConversion.cs:118–145`, `EmitIdentityConversion`. O código mantém `EmitNumericConversion` para uma conversão explícita float→float/double→double quando `conversion.Operand.ConstantValue == null`, pois ela pode reduzir a precisão intermediária. Esse é o snapshot Roslyn fixado em `4cb203dab89e1542cdf7d88cb6d32767bc0e5460`; a identidade exata do commit de fabricação do csc instalado continua UNKNOWN.

| ID | IL original | Recompilado atual | Proposta |
|---|---|---|---|
| `kairo.unity.graph.Graph::Easing(System.Single,System.Single,System.Single,System.Single,System.Single)->System.Single` | `16 ldarg.3; 17 conv.r4; 18 ldarg.2; 19 div` | `16 ldarg.3; 17 ldarg.2; 18 div` | `(float)(frame) / length` |
| `kairo.unity.ui.Graphics::_drawImage(kairo.unity.ui.Image,System.Single,System.Single,System.Single,System.Single,System.Int32,System.Int32,System.Single,System.Single)->System.Void` | `677 ldarg.s width; 679 conv.r4; 680 ldarg.s swidth; 682 div` | falta `conv.r4` | `(float)(width) / swidth` |
| mesmo `_drawImage` | `685 ldarg.s height; 687 conv.r4; 688 ldarg.s sheight; 690 div` | falta `conv.r4` | `(float)(height) / sheight` |

São **2 métodos / 2.345 bytes de IL original**: Easing 97 e _drawImage 2.248. São três ocorrências, não três métodos. Easing tem previsão NoEmit completa, 3 tipos idênticos aos originais. _drawImage permanece incompleto: 45 previstos contra 43 originais, com limitações do scheduler de pilha explicitadas no JSON. Também há diferenças posteriores de slots; os casts não prometem fechar o método.

Fixture independente com o csc 1.1.0.51109 selecionado, SHA-256 `53b693536e8a86f35d05d2165ff9b3e0aeaa499ecfd8135a9f99ab4404ca86c3`: três corpos completos iguais aos controles, incluindo cabeçalho/locais/EH, e exclusão apenas do nome da declaração do método de controle. Uma execução reproduzível compila quatro assemblies toy. Negativos: sem evidência original, cast já presente, parâmetro Double, expressão constante e conversão Int32→Single. As variantes `.un` nas comparações foram preservadas. Na preparação, o primeiro controle negativo forjou indevidamente um hint para o parâmetro `length` de um método com constante; esse teste falhou e foi corrigido para derivar o hint do IL real, que não contém `conv.r4`. A transformação não foi alterada para disfarçar essa falha de fixture. Reexecuções por transporte/metadados totalizaram 16 compilações históricas toy, zero jogo.

## 2. Store no mesmo local em cada braço de uma condição

Família `d9-calculated-per-arm-local-stores`; entry `AddRound9PerArmStoreRules`; helper `tools/Round9PerArmStoreRules.cs`. O motor pode acrescentar ao input `round9_per_arm_store_hints.per_arm_local_store_hints(original, rebuilt, original_offset, rebuilt_offset)`.

O hint é uma prova de padrão limitada, não um extrator de árvore de controle: verifica dois stores originais no mesmo slot, um salto do primeiro braço para uma junção após o segundo store, valores lineares sem desvios internos e os mesmos operandos em ambos os braços da reconstrução. Na reconstrução, confirma store único após a junção. A orientação false-first depende também desses operandos e de `EmitConditionalOperator`, não só da polaridade do desvio. O helper só aceita atribuição simples a `ILocalSymbol` e condição Boolean, preservada literalmente sob `!`; não aceita campo, índice, parâmetro, dynamic, await ou lambda.

Evidência primária: `EmitExpression.cs:1768–1772` emite o valor da atribuição antes de `EmitStore`; `EmitExpression.cs:2639–2710`, `EmitConditionalOperator`, emite primeiro `expr.Alternative`, depois `expr.Consequence` e a junção. Uma expressão ternária atribuída tem um store após a junção; dois comandos de atribuição em if/else conservam dois stores. O snapshot é o mesmo acima.

| ID | Primeiro store original / segundo / slot | Proposta |
|---|---|---|
| `kairo.unity.util.Compressor::Expand(System.Byte[])->System.Byte[]` | 80 / 91 / 4 | `if (!(_readBit()!=0)) node2=node2.left_; else node2=node2.right_;` |
| `kairo.unity.ui.TextLayout::_makeTextBlocks()->System.Void` | 2063 / 2076 / 9 | separar ternário do primeiro alinhamento à direita |
| mesmo TextLayout | 2545 / 2576 / 9 | separar ternário do segundo alinhamento central |
| mesmo TextLayout | 2601 / 2614 / 9 | separar ternário do segundo alinhamento à direita |

São **2 métodos / 3.287 bytes de IL original**: Compressor 190 e TextLayout 3.097. O `ret @189` de Compressor ocupa o último byte; `header.code_size=190`. O método possui EH; não se soma o cabeçalho de 12 bytes. Os JSONs contêm os operandos e as provas do símbolo local: node2 slot4/Node e num3 slot9/Single.

TextLayout: previsão incompleta, mas todos os 53 tipos previstos são iguais aos originais. Compressor: previsão incompleta, cinco tipos contra seis; a limitação inclui o local Byte[] do retorno protegido e `exception_lowering`. Esses casos podem ser compilados pelo orçamento de incompletos da rodada 9; nenhum completo errado foi aprovado.

Dois controles históricos independentes de corpo completo passaram: Node com atribuição a si próprio dentro do while e alinhamento float com `.un` e leitura do local no braço. Uma execução compila três assemblies toy. Negativos: parâmetro, campo, índice com efeito colateral, ausência de ternário e evidência original inválida. A preparação repetiu os controles ao adicionar provas/imports/metadados, totalizando 12 compilações históricas toy, zero jogo.

Esta família reutiliza uma forma de emissão já conhecida, não a apresenta como descoberta nova. O histórico de R8 mostra `assignment-ternary-if-reverse` aceito em três **outras** expressões TextLayout; a fila acima cobre as três expressões ainda presentes. Não houve variante de atribuição por braço compilada para Compressor no histórico examinado. A seleção é agora justificada pelos stores e pelos operandos originais.

## Reprodução e integração

```
python -X utf8 tools/round9_float_identity_fixtures.py
python -X utf8 tools/round9_per_arm_store_fixtures.py
python -X utf8 tools/round9_float_identity_calculate.py
python -X utf8 tools/round9_per_arm_store_calculate.py
```

Os dois últimos comandos fazem somente parsing, vinculação semântica e previsão NoEmit sobre o corpus B10 congelado; escrevem o resultado em `build/round9/part-d-*`. Os relatórios publicados são cópias desses resultados. As propostas de um mesmo método são alternativas independentes contra a mesma base; depois de uma adoção, deve-se recalcular/revalidar ou usar o helper com os novos offsets, em vez de aplicar cumulativamente um `before` antigo. O verificador oficial continua sendo o único juiz de correspondência, e o orçamento/assinatura original são controlados pelo motor do root.

Nenhum arquivo de modelo, fonte do jogo, tabela de estado, compilador, referência ou byte de assembly foi alterado. Os corpos do jogo continuam UNKNOWN até a execução oficial de cada candidato.
