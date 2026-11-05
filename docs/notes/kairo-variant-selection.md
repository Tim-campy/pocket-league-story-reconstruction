# Seleção de variantes por método

Foram compilados quatro conjuntos isolados de fontes, todos com o pacote Microsoft.Net.Compilers 1.3.2, executável 1.3.1.60621, mesmas opções e mesmas referências da linha de base Kairo. A ordem dos membros foi restaurada nas cópias isoladas antes da comparação. Nenhum conjunto inteiro substitui o fonte principal.

## Perfis que compilam

| Variante | Configuração relevante | Corpos correspondentes no build isolado | Corpos novos frente à linha de base de 2.878 corpos iguais |
|---|---|---:|---:|
| ILSpy control_flow | NullPropagation, For/DoWhile/ForEach, Using, Lock, SwitchStatementOnString, SparseIntegerSwitch e StringInterpolation desligados | 2.843 | 36 |
| ILSpy conservative | NullPropagation, ObjectOrCollectionInitializers, StringInterpolation, QueryExpressions, For/DoWhile, Using e Lock desligados | 2.835 | 26 |
| ILSpy expressions | NullPropagation, StringConcat, StringInterpolation e ObjectOrCollectionInitializers desligados | 2.896 | 21 |
| Codemerx compatível | Decompilador independente, reparos de compatibilidade C# 6 e fallback ILSpy explícito | 2.616 | 120 |

Os ganhos se sobrepõem: a união contém **145 corpos novos**, sendo 135 com membro C# direto e dez gerados a tratar pelo método de origem. Os números acima são evidência de builds isolados, não promoção automática de métodos. Adotar conjuntos inteiros perderia respectivamente 71, 69, 3 e 382 corpos já correspondentes.

Perfis iniciais mais agressivos não compilaram: desligar `MakeAssignmentExpressions` fez o ILSpy expor estruturas de closures com identificadores ilegais em C#, e outras combinações produziram locais `ref` incompatíveis com C# 6. Essas saídas não foram publicadas como fontes válidas nem reparadas escrevendo classes geradas à mão. Os perfis seguros foram gerados separadamente; os arquivos iniciais ficaram em `analysis/` para registrar a falha.

## Decompilador independente

Codemerx/JustDecompileEngine é uma implementação distinta do ILSpy. Versão, commit, hashes e forma de compilação estão em [Codemerx-decompiler.json](../../reports/Codemerx-decompiler.json). Seus binários e fontes externos não acompanham o repositório.

A saída bruta não compilava com C# 6. Em uma cópia isolada, locais `ref` usados em atualizações de array/campo foram reduzidos a atribuições compostas; labels vazios receberam uma instrução vazia; defaults numéricos de enums receberam conversão explícita. Um arquivo, `HttpObject`, continha estruturas de closure não recuperadas e recebeu fallback ILSpy. Após os primeiros reparos, 34 membros com erros semânticos também receberam fallback por identidade de símbolo e spans do AST. As alterações e limitações constam em [KairoLibrary-codemerx-compatibility.json](../../reports/KairoLibrary-codemerx-compatibility.json).

Portanto, a linha Codemerx da tabela é um **conjunto híbrido compilável**, não uma recompilação integral da saída bruta daquele decompilador. O verificador decide cada adoção pelo resultado compilado; nenhuma igualdade é inferida apenas da lógica aparente ou da autoria do trecho.

## Regras do seletor

`tools/evaluate_kairo_variants.py` confere hashes de fontes e artefatos, compila quando solicitado e compara os corpos completos usando o mesmo comparador do verificador. `SourceRecovery` identifica membros por símbolos, com spans em UTF-16. As fontes, tipos e membros recebem identidades estáveis; operações sobre os spans preservam exatamente os caracteres fora da substituição.

`tools/select_kairo_variants.py --apply` usa somente candidatos cujo build completo isolado reproduziu o corpo alvo. Métodos comuns recebem apenas o corpo da variante; construtores e operadores preservam a unidade sintática necessária ao inicializador. Usings adicionais passam pela compilação. Membros gerados são modificados através do método de origem, obtido das provas de referências no IL e dos nomes originais; nenhuma classe gerada é escrita explicitamente.

Cada tentativa altera uma única unidade de origem. O seletor exige correspondência integral do alvo e nenhuma regressão entre os métodos antes confirmados, executando `tools/verify_kairo.py`, que recompila a DLL inteira e compara todos os corpos. Tentativas que falham são restauradas e a restauração é verificada. Estados do cadastro são escritos exclusivamente pelo verificador.

O modo padrão faz também um build de preflight antes de editar o fonte principal. `--skip-preflight` evita esse build redundante quando já existe a prova do build completo da variante; mantém a verificação integral após cada tentativa, o rollback e a verificação da restauração. Não reduz o critério de correspondência.

## Resultado da rodada

Todos os **145 alvos foram avaliados**. O seletor adotou **141 métodos**, incluindo os dez membros gerados tratados através de seus métodos de origem. A KairoLibrary passou de **2.874 para 3.015 matched**, e de **2.878 para 3.019 corpos iguais**, com zero regressões em cada verificação integral. Das adoções, 98 vieram do conjunto Codemerx compatível, 22 do perfil control_flow e 21 do perfil expressions. Os ganhos de conservative estavam cobertos por outras variantes escolhidas primeiro.

Quatro tentativas de SoundPlayer falharam no preflight por um conflito de import: adicionar `using System` tornou o `Object.Destroy` preexistente ambíguo. Um alias explícito para `UnityEngine.Object` preservou a ligação original; as quatro tentativas seguintes passaram no verificador integral. Nenhuma mutação do fonte principal precisou de rollback nessa rodada.

Os quatro alvos sem um membro C# explícito no fonte principal eram inicializadores estáticos implícitos: Config, AdMobPlugin, SoundPlayer e CRC64. Eles não foram descartados por semântica nem escritos como classes geradas; foram encaminhados ao fluxo separado de recuperação de inicializadores de campos, com candidatos e provas próprios. A integração desses quatro fica fora das contagens do seletor acima. Inserir um cctor explícito sem conferir BeforeFieldInit alteraria metadados, por isso essa recuperação exige o controle adicional dos atributos e campos do tipo.

Resultados por variante: [avaliação](../../reports/KairoLibrary-variant-evaluation.json). Candidatos e origens: [plano](../../reports/KairoLibrary-variant-selection-plan.json). Cada adoção, hashes e regressões: [seleções](../../reports/KairoLibrary-variant-selections.json). As contagens finais devem ser lidas nesse último relatório e no cadastro atualizado, separadas da tabela histórica de builds isolados acima.
