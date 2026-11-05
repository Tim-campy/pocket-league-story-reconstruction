# Linha de base de Assembly-CSharp

A validação foi ampliada de quatro sondas para a compilação integral. Foram recuperados 96 tipos superiores mais `AssemblyInfo.cs`, 97 arquivos C# e 73.367 linhas, na ordem inicial de `config/compile-order/Assembly-CSharp.txt`. O assembly original tem SHA-256 `3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949`, 1.958 métodos, sendo 1.952 com corpo CIL e seis declarações sem corpo.

## Compiladores e opções

O melhor candidato testado é o **smcs 2.6.5.0 incluído no Unity 5.3.4f1**, com stdlib default. A identidade histórica exata continua UNKNOWN. A igualdade de sondas simples entre smcs e gmcs não antecipou diferenças na compilação integral. A tabela abaixo registra a matriz inicial, que ainda utilizava `-nostdlib`; a configuração corrigida está na seção seguinte.

| Candidato | Build integral | Corpos correspondentes |
|---|---|---:|
| smcs 2.6.5.0, optimize+ | Sem erros | 1.444/1.952 |
| smcs 2.6.5.0, optimize- | Sem erros | 1.444/1.952 |
| gmcs 2.6.5.0, optimize+ | Oito erros de resolução de extensões | Não comparado |
| mcs 3.4.0.0 incluído no MonoBleedingEdge do Unity | Sem erros | 703/1.952 |

Os dois builds smcs produzem os mesmos corpos para todos os métodos, não apenas a mesma contagem. A comparação smcs versus mcs 3.4 tem 698 correspondências comuns, 746 somente smcs, cinco somente mcs 3.4 e 503 que não correspondem em nenhum. Os cinco casos somente mcs 3.4 são `TeamData.Init`, `TrainingData.Load2`, `UserData.CustomerNumReset`, `AppData.ClearZSortObjects` e `GameView.GetKey`. Nenhum deles justifica trocar globalmente de compilador, pois isso perderia 746 correspondências. Resultados completos em [reports/Assembly-CSharp-compiler-comparison.json](../../reports/Assembly-CSharp-compiler-comparison.json).

smcs usa o runtime `Editor/Data/Mono/bin/mono.exe`, com ambiente restrito à invocação: `MONO_PATH=Editor/Data/Mono/lib/mono/unity`, `MONO_CFG_DIR=Editor/Data/Mono/etc`. SHA-256 smcs: `26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0`; runtime: `3d102d9a63dae72e48396998bdbae4e9b2d8952ca6a8ebbcba698d3bc4345076`.

Flags selecionadas: `-nologo -noconfig -target:library -platform:anycpu -optimize+ -debug- -langversion:Default -unsafe+ -checked- -codepage:65001`, com `--stdlib default` no driver e sem `-nostdlib`. `-noconfig` fica fora do response file. `Default` aceita os parâmetros opcionais presentes no fonte; `-langversion:3` rejeita essa sintaxe, e `-langversion:4` não é uma opção aceita por esse compilador. Isso não identifica a versão de linguagem do fonte histórico.

Referências, na ordem passada: **DLLs do próprio APK** `mscorlib.dll`, `KairoLibrary.dll`, `UnityEngine.dll`, `System.Core.dll`, `System.dll`. KairoLibrary entra apenas como referência de compilação, nunca como resultado reconstruído. Com stdlib default, os AssemblyRefs resultantes coincidem em identidade **e ordem** com o original, incluindo mscorlib/System 2.0.5.0. O corlib implícito do perfil Unity é igual por SHA-256 ao arquivo do APK. Isso ainda não declara igualdade completa de metadados ou arquivo. Não há atributo Debuggable no original nem no candidato selecionado.

## Correção da opção de biblioteca padrão

Sondas isoladas demonstraram que a ambiguidade de `String.IndexOf(string,int)` dependia de `-nostdlib`, e não da ausência desse overload na API. O mesmo smcs, com as mesmas referências explícitas e ambiente Unity, compila a chamada quando essa opção é retirada. A causa interna do comportamento do importador não foi investigada; a diferença de configuração foi comprovada diretamente.

No assembly inteiro, retirar somente `-nostdlib` aumenta de **1.444 para 1.530 corpos iguais**, com 86 ganhos e nenhuma perda. A chamada original `IndexOf("<co=", 0)` foi então restaurada, sem contorno. `Draw_helpMessage` ainda difere em outras instruções e cabeçalho, por isso não foi declarado correspondente. [Comparação completa](../../reports/Assembly-CSharp-stdlib-comparison.json), [sondas por perfil](../../reports/Assembly-CSharp-indexof-profile-probes.json).

O recheck mantém 1.530 em smcs optimize+ e optimize-, com todos os corpos iguais entre essas opções. gmcs continua com oito erros de extensões. mcs 3.4 sem `-nostdlib` seleciona corlib 4.0.0.0 e é um controle de perfil inválido para o original; sua contagem não deve ser usada como comparação de versões. [Matriz corrigida](../../reports/Assembly-CSharp-stdlib-matrix.json).

Por fim, os campos `FormManager.instance_` e `StoreBase.ks_` receberam inicializadores de valor default `= null`. Essa forma C# reproduz os dois `.cctor` originais contendo apenas `ret`, mantendo `BeforeFieldInit=true`, campos e ordem dos métodos; não prova que esse era o texto exato do fonte histórico. O candidato combinado tem **1.532/1.952 corpos iguais**, zero métodos ausentes e zero adicionais, sem perda de correspondências. Não foi inserido construtor estático explícito. [Prova](../../reports/Assembly-CSharp-static-initializers.json).

## Recuperação e correções para compilar

ILSpy 9.1.0.7988, já instalado no Visual Studio, foi executado pelo runtime .NET 10.0.10 em um driver próprio, com linguagem CSharp3, símbolos desligados e resolução estrita. O resolver procura primeiro o perfil Unity 2.0.5.0, depois os arquivos do APK. A decompilação não executa os assemblies do jogo.

Mesmo com configurações CSharp3 e `UseRefLocalsForAccurateOrderOfEvaluation=false`, ILSpy emitiu três declarações `ref ... = ref ...` incompatíveis com smcs. Para obter o primeiro build integral, os trechos de `main/Main.cs` e `form/SubForm.cs` foram expressos como atribuições compostas de strings; `Tenant.Draw` usa `camera_pos[1] += 0`. São alterações de C#, verificadas depois pelo comparador; não houve transplante de IL nem edição de binários.

O quarto caso é uma limitação de compilação ainda sem causa exata comprovada: em `AppData.Draw_helpMessage`, smcs e gmcs reportam ambiguidade entre duas assinaturas exibidas como `string.IndexOf(string,int)`. A chamada original no IL é `System.String.IndexOf(System.String,System.Int32)` do mscorlib 2.0.5.0. O erro também aparece em um programa sintético que referencia apenas esse mscorlib, sem KairoLibrary ou UnityEngine. Argumento nomeado, variável int e cast não resolveram; mcs 3.4 compila essas sondas. [Relatório das sondas](../../reports/Assembly-CSharp-indexof-probe.json).

Na linha de base inicial foi usado `text.IndexOf("<co=")` para permitir compilar. Esse contorno foi removido após a investigação de stdlib; o fonte atual voltou a usar `text.IndexOf("<co=", 0)`, com o mesmo opcode e referência do original. O método continua divergente por outras causas. O comportamento interno do compilador que causa a ambiguidade com `-nostdlib` permanece UNKNOWN.

## Verificação e limites

Na primeira execução conservadora foram encontrados 1.444 corpos iguais, com 1.416 elegíveis à promoção e 28 retidos por vínculos gerados. Desses 28, 27 referenciam campos de `<PrivateImplementationDetails>`: o Mono emite um construtor nesse tipo, e a regra compartilhada de grupos inicialmente sombreava a verificação dos valores desses campos. O construtor do próprio tipo não tem origem comprovada. As contagens finais após integrar a regra ficam no [relatório de verificação](../../reports/Assembly-CSharp-verification.json).

Após corrigir a precedência dessa regra, a configuração stdlib e os dois inicializadores default, o resultado é **1.531 matched em 1.958 métodos (78,19%)**, com 1.532 corpos iguais. O construtor auxiliar continua retido; as seis declarações sem CIL e os 420 corpos divergentes ficam pendentes. Não houve perda de nenhum corpo anteriormente correspondente.

Os dois IDs inicialmente ausentes, `.cctor` de `form.FormManager` e de `system.billing.ui.StoreBase`, foram recuperados pelos inicializadores default descritos acima. Não há IDs adicionais no build smcs. As seis declarações sem CIL permanecem pendentes até a comparação completa dos metadados. O tipo auxiliar sem fonte de origem não deve ser promovido apenas por o corpo corresponder.

O verificador confere cabeçalho, maxstack, locais, instruções, destinos de switch, padding e cláusulas de exceção, resolvendo somente os operandos de tokens. O registro de estado vem dessa execução, nunca de edição manual. Metadados de tipos, recursos, heaps e identidade do assembly permanecem NOT_VERIFIED. Nenhum campo foi normalizado nesta validação.

## Reprodução

Com Unity 5.3.4f1, Python e sua própria extração do APK disponíveis:

```powershell
python tools/build_csharp.py
python tools/verify_csharp_sidecar.py
python tools/verify_csharp.py
```

O sidecar não altera o cadastro mestre. `verify_csharp.py` recompila, compara e atualiza automaticamente apenas as linhas de Assembly-CSharp, produzindo `build/verification-Assembly-CSharp.json`; retorna 1 enquanto existirem diferenças. A entrada global `tools/verify` integra essa etapa separadamente. `probe_csharp_indexof.py` e `compare_csharp_compilers.py` reproduzem os diagnósticos; os arquivos necessários da matriz ficam em `build/` e não são distribuídos.
