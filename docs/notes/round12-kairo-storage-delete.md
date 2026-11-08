# R12 — Storage e UTC, dois subgrupos independentes

As propostas em `storage/` e `utc/` são independentes. Foram preparadas
somente sobre a baseline12 congelada; seus dois fontes relevantes também
coincidiam com o Main lido. Start e os pacotes anteriores não foram alterados.
Zero compilação do jogo, zero verifier, zero mudança pública ou de estado.
Todos os resultados reais dos alvos continuam **UNKNOWN**.

Original comum: artifact SHA256
`bb553d341eecd9b900f772486bf4a5c289aa2f09566d9d402a3eb70268f4b3ee`,
MVID `b564fa31-554d-4b48-a8c0-a0b41bea2550`. Não confundir esse hash com
os hashes CIL individuais abaixo. Evidence REA dos30métodos selecionados:
`ev_a806f11ece8b88b5c2465ff07902c3af0c34e9cc3be4ca9a752d9f6f61d12f2b`.
O display class é identificado pelo token preciso, MVID e hash; o REA
apresenta o nome plano `<>c__DisplayClass8_0`.

## Storage.Delete(String) —164bytes

ID: `kairo.unity.io.Storage::Delete(System.String)->System.Void`.
Token `0x060008b6`; CIL SHA256
`55195b0ac4329cb02f763259ae411e4af3087f0a9c4beed7911422070c1f20f8`.
Fonte `src/KairoLibrary/kairo/unity/io/Storage.cs`, SHA256
`b96651130c469bb46ac09dbf73e9bbddcd5538bd3fadc3774e8eec298d16406f`.

Original e atual têm164bytes, dois locais Int32/UnauthorizedAccessException,
maxstack3 e o mesmo catch: try0..148, handler148..163, retorno163.
Os três hunks são de controle:

| Original | Atual |
|---|---|
|34 beq.s58|34 beq.s146|
|58 leave.s163|58 br.s146|
|67 bne.un.s71|67 beq.s87|
|69 leave.s163|69 ldarg.0|
|78 beq.s89|76 beq.s89|
|87 bne.un.s146|85 beq.s89|
|146 leave.s163|146 leave.s163|

O destino58 original sai do try;146 é a saída comum ainda dentro do try;
163 é o RET depois do catch. Não se substitui um deles por um salto
supostamente equivalente sem considerar a região EH.

Proposta integral:

```csharp
if (Config.USE_HEAP_STORAGE && media_ == 2) {
    // Folhas existentes de procura/remoção.
    return;
}
if (media_ == 3) { return; }
if (media_ == 1 || media_ == 4) {
    // As duas alternativas existentes de exclusão de arquivo.
}
```

O catch e todas as folhas permanecem no try original. O teste inteiro de
mídia conserva a ordem e o curto-circuito das leituras de campo. Não há
avaliação extra de filename nem alteração das chamadas File/Exists.
API pura `storage/calculate.py::transform_storage_delete_exits`;
família `r12-storage-delete-heap-return-media-positive-body`.

Ownfixture: before/candidato/controle164bytes, dois locais, maxstack3,
catch exatamente nos limites acima. Corpo inteiro candidato=controle;
before e negativo sem o return do ramo heap diferem. O negativo tem162bytes.
NoEmit **modelo12 atual**: incompleto2 antes/depois, motivos
`stack_scheduler_multiple_use:num`, `exception_lowering`; não se promoveu
completude nem se relaxou previsão completa errada.

Histórico:54registros e três trials reais, todos rejeitados: espelhamento
primitivo, retorno antecipado para num==-1 e temporário de condição.
A composição atual não repete nenhum texto/hash completo capturado.

## JavaPlugin.GetUtcOffset, lambda —140bytes

ID: `java.native.JavaPlugin/<>c__DisplayClass8_0::<GetUtcOffset>b__0()->System.Object`.
Pai: `java.native.JavaPlugin::GetUtcOffset(System.String)->System.Int32`.
Token `0x06000cc2`; CIL SHA256
`bc87ba74387aed93741eb139c90ce88970519c3d07acd3f7b14ffc5d3bfc20fa`.
Fonte `src/KairoLibrary/java/native/JavaPlugin.cs`, SHA256
`a40729088e537334b47b842698bbaa0dcc62b3e6dbfa4f014c736733b60e7a01`.

Ambos têm140bytes, maxstack6 e finally58..83/83..93. A diferença é somente
a permutação dos três locais, inclusive no ldloca.s do argumento out:

| Papel | Original | Atual |
|---|---:|---:|
|AndroidJavaObject do using|0|1|
|Object do retorno protegido|1|2|
|Int32 do fallback de dicionário|2|0|

O slot Object é escrito em80, sai do using por leave81→138 e é retornado
em138; o finally descarta AndroidJavaObject. O fallback começa em93, usa
Int32 em111/123/131 e retorna diretamente em137. O retorno protegido e o
fallback não são fundidos nem deslocados.

A proposta envolve em um bloco filho somente o fallback já existente:
teste de Count/CreateUtcOffsetTable, declaração `int value=0`, TryGetValue
e `return value`. O using anterior é emitido primeiro, aloca o Android0 e
o Object1; depois o bloco filho declara o Int32 no2. As folhas e a variável
timezone capturada permanecem iguais; nenhuma classe gerada é escrita.

API pura `utc/calculate.py::transform_utc_fallback_scope`;
família `r12-utc-fallback-local-after-using-return-temp`.
Ownfixture: corpo completo candidato=controle,140bytes, três locais na
ordem original e mesmo finally; before e negativo com Int32 exterior
diferem. O número de bytes sozinho não seria suficiente.

NoEmit modelo12: lambda genuinamente não mapeada, incompleta0; pai
incompleto0 por anonymous_function_or_capture. Nenhuma previsão foi
artificialmente completada. A assinatura real exigirá compilação do alvo.
Histórico:54registros e um trial real rejeitado de variável de retorno no
using. Não era o novo bloco do fallback. Nenhum payload igual foi encontrado.

## Prova, ferramentas e limites comuns

Oito compilações próprias: quatro por grupo (before, candidato, controle
manual independente e negativo). Duas positivas de corpo completo e quatro
negativas passaram os critérios esperados. csc1.1.0.51109, SHA256
`53b693536e8a86f35d05d2165ff9b3e0aeaa499ecfd8135a9f99ab4404ca86c3`,
flags e referências da baseline12. `/out` e `/pdb` foram redirecionados
individualmente às pastas privadas; o PDB histórico em E: não foi escrito.

Fonte Roslyn fixa: `4cb203dab89e1542cdf7d88cb6d32767bc0e5460`.
Git-blobs e SHA256 estão em `pinned/provenance.json`.

- `BasicBlock.cs:275–293` converte branch incondicional que cruza a região
  do handler em Leave. Isso distingue os exits do Storage.
- `LocalRewriter_IfStatement.cs:49–91` conserva consequência/alternativa e
  seus rótulos de junção. Não é uma simples troca global de opcode.
- `EmitStatement.cs:594–630` define os locais de cada bloco ao visitar o
  bloco;648–664 e676–701 materializam retorno em handler via LazyReturnTemp.
- `CodeGenerator.cs:139–174` aloca o temporário de retorno sob demanda.
  Com o Int32 num filho posterior, o temporário Object passa a precedê-lo.

As previsões simbólicas privadas explicam todos os opcodes, operandos,
offsets e destinos, sem diferenças, para164/140bytes. Elas não produzem IL
para o build, não normalizam assemblies e não são o verificador. Headers,
locais e EH concretos são apenas os observados nas fixtures próprias e
baseline; os alvos candidatos não foram emitidos.

Os bundles independentes estão em `storage/ready-for-game-probe.json` e
`utc/ready-for-game-probe.json`, com índice comum `ready-index.json` e
freeze `ready-freeze.json`. As porcentagens/estados oficiais não foram
tocados. Depois destes dois grupos a tarefa para; nenhum estudo adicional.
