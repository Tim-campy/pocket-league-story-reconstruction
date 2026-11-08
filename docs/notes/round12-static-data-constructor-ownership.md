# Origem do construtor do reservatório de dados do compilador

O último auxiliar retido era `<PrivateImplementationDetails>::.ctor()->System.Void`, token original `0x06000795`, de sete bytes. Não era o construtor da lambda de Update2. Seu corpo já correspondia; faltava a prova de origem. A regra anterior procurava uma aresta de chamada para o construtor, mas nenhum dos dois assemblies o chama.

O smcs cria o reservatório como uma classe não pública comum, com base `System.Object`, e marca o tipo como gerado pelo compilador. Ao fechar esse tipo, Reflection.Emit sintetiza o construtor público padrão. O corpo é `ldarg.0; call System.Object::.ctor; ret`, sem locais ou regiões de exceção. Seus atributos são `Public|SpecialName|RTSpecialName`, valor `6150` (`0x1806`), sem `HideBySig`.

O código consultado é a revisão `b6587926c0f146c8b12985744f33a13b39a22d57` do repositório Mono da Unity. `rootcontext.cs:405–423` cria o tipo e seus campos inicializados; `rootcontext.cs:280–281` aplica o atributo e chama `CreateType()`. `TypeBuilder.cs:831–836` solicita o construtor padrão quando não há construtor declarado; `TypeBuilder.cs:519–521` emite as três instruções; `ConstructorBuilder.cs:70` acrescenta os atributos especiais. Isso explica a forma observada. A revisão exata usada para fabricar os binários históricos instalados continua **UNKNOWN**.

Seis bibliotecas próprias, escritas apenas como C# comum, confirmaram o mecanismo: dois casos positivos geraram reservatório, campos RVA e construtor; quatro controles com três constantes, zeros, valores dinâmicos ou strings não geraram o reservatório. Nenhuma classe gerada foi escrita à mão. A fixture com seis bytes tem campo de oito bytes por alinhamento: a comparação considera todo o payload físico, inclusive o preenchimento.

A regra genérica exige, simultaneamente:

- Identidade completa dos metadados do reservatório e dos layouts aninhados, atributo `CompilerGenerated` e forma exata do construtor implícito.
- Corpo completo do construtor já aprovado pelo comparador independente.
- Somente campos estáticos RVA, com nomes, ordem, tipos, tamanhos e todo o conteúdo inicial idênticos.
- Para cada campo, a sequência correspondente `dup; ldtoken campo; call RuntimeHelpers.InitializeArray` nos dois lados.
- Mesmos consumidores e suas declarações; corpos completos de todos eles já iguais.
- Presença de todos os arquivos de origem consumidores no manifesto efetivamente compilado.

No jogo, são 217 campos, 15 layouts, 6.660 bytes de payload e 217 arestas em 32 métodos, distribuídos em 19 arquivos C#. Não existe uma origem única correta para a coluna `fonte` desse construtor. O verificador passa a registrar o conjunto de origens e sua disponibilidade compilada; a coluna não é preenchida artificialmente. `gerado_por` também não recebe um método arbitrário.

Quinze testes da regra verificam os dois positivos e quatro negativos emitidos pelo compilador, além de alterações de payload, layout, atributo, consumidor, corpo, chamada de inicialização e disponibilidade de fonte. Os sete testes anteriores de comparação continuam passando. A certificação lê os snapshots e não promove estados; somente uma nova execução completa do verificador pode liberar o auxiliar.

O ganho de estado, se confirmado, é registrado como liberação de origem separada dos 39 ciclos B. Ele não muda retrospectivamente o ganho de LanguageForm no ciclo 39 e não inventa um ciclo 40 de correção de corpo.

Evidência: `reports/round12-static-data-constructor-ownership.json`, `reports/round12-static-data-constructor-emission-fixture-report.json`, `reports/round12-static-data-constructor-compiler-evidence.json` e `reports/round12-static-data-owner-tests.json`.
