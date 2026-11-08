# R12 — KairoBase.Start, lambda de182bytes

Proposta privada **READY_FOR_GAME_PROBE**. Zero compilação do jogo, zero
verificação oficial, zero alteração pública ou de classe gerada. A adoção e
a liberação dos auxiliares continuam UNKNOWN até o oráculo do operador.

## Identidade e fonte

Alvo: `kairo.unity.system.KairoBase/<>c::<Start>b__55_0()->System.Object`,
token `0x06000be9`. Pai normal: `kairo.unity.system.KairoBase::Start()->System.Void`.
O único patch modifica o pai `src/KairoLibrary/kairo/unity/system/KairoBase.cs`.
Seu SHA256 bruto é `e56198657a69bfd958acdea29d621dacaf38773fff0c8e229e0af28458b61f0e`.
Foi conferida igualdade entre HEADa075dcc, final-corpus11 e baseline12;
os corpos originais/atuais dos snapshots11/12 também coincidem.
`inputs.json` vincula os dois snapshots e o receipt novo.

REA:

- **SHA256 do artefato original**:
  `bb553d341eecd9b900f772486bf4a5c289aa2f09566d9d402a3eb70268f4b3ee`.
- MVID: `b564fa31-554d-4b48-a8c0-a0b41bea2550`.
- **SHA256 somente dos182bytes CIL do método**:
  `3d46779e2369148627a0bcb27977fdae045a48103b6ce24856447b561e646532`.
- Evidence da página:
  `ev_29f89e895c400e85f1dba5cdc49a507ad35244ad65f540022f7eabee92d2144a`.

Esses dois hashes têm escopos diferentes. O REA desta instalação apresenta
o nome plano `<>c`; os filtros com slash/plus não encontraram a classe
aninhada. A seleção final cobre sete tipos de nome plano `<>c`. O vínculo
exato usa token, artefato, MVID e hash CIL, conferido em
`rea-byte-binding.json`; não depende de adivinhar qual tipo plano é o pai.
O doctor CLI respondeu audit-wide unhealthy por ferramentas opcionais e
drift de registro/skill; o MCP managed respondeu e forneceu as evidências
usadas. Nenhuma instalação/configuração foi alterada.

## Fato medido e hipótese

Original e atual têm182bytes, seis locais idênticos, maxstack6, cabeçalho
fat12/flags27/initlocals e o mesmo catch System.Object: try3..168,
handler168..179. Os três ternários de idioma e os três retornos Boolean
boxed são iguais fora da região divergente. O problema medido é a posição
dos blocos de retry/cancelamento, não a representação do Boolean.

| Original | Atual |
|---|---|
|147 bne.un.s158|147 beq.s159|
|149 ldloc.0|149 ldc.i4.0|
|150 ldc.i4.1|150 box Boolean|
|151 add|155 stloc.s5|
|152 stloc.0|157 leave.s179|
|153 leave2|159 ldloc.0|
|158 ldc.i4.0|160 ldc.i4.1|
|159 box Boolean|161 add|
|164 stloc.s5|162 stloc.0|
|166 leave.s179|163 leave2|

O destino2 foi lido: é o `nop` do início do laço, antes do try3.
O destino158 original começa `ldc.i4.0; box Boolean` e retorna pelo
temporário5. Não se confundiu destino de RET com entrada de outro fluxo.
`Dialog.Show` retorna Int32, portanto a inversão usada não envolve NaN/.un
de ponto flutuante.

Hipótese final, **CONFIRMADA SOMENTE NA FIXTURE PRÓPRIA**:

```csharp
if (dialog.Show() == 4) { num++; continue; }
return false;
```

O `continue` deixa o try e retoma o mesmo `while(true)`. Ele não é break de
switch, retorno ou repetição da chamada Show. O produtor verifica o dono
lexical, o try envolvente e a ausência de outro laço/switch intermediário.
As folhas `num++` e `return false` são preservadas; chamadas e strings do
pai permanecem iguais e Show é avaliado uma vez.

## Resultado negativo preservado

A primeira forma `if (Show()==4) { num++; } else { return false; }` foi
**REFUTADA** em ownfixture:184bytes contra182 do controle. Ela insere em153
um `br.s165`, antes do `leave` do laço. Isso também desloca o catch e o
retorno final. Os quatro builds R1, produtor, driver, dumps e relatório
estão retidos em `fixtures/`, `calculate_v1_refuted.py`,
`fixtures_v1_refuted.py` e `fixture-report-v1-refuted.json`.
Foi uma falha da hipótese de lowering, não uma compilação falhada ou teste
do alvo. Não se repetirá essa forma no jogo.

R2 usou quatro outros builds privados: before, candidato, controle manual
independente e polaridade errada. Todos têm182bytes. O candidato coincide
com o controle em todas as facetas de corpo de `compare_kairo.differences`;
before e polaridade errada diferem. Seus seis locais e o catch mantêm os
limites acima. A lambda foi escrita no pai da fixture e gerada pelo csc;
nenhuma classe gerada foi escrita à mão. O PDB de cada build é privado.
Total: **oito compilações próprias, zero do jogo**.

## Compilador fixado e análise sem emissão

csc1.1.0.51109, SHA256
`53b693536e8a86f35d05d2165ff9b3e0aeaa499ecfd8135a9f99ab4404ca86c3`,
flags e referências fixadas; somente paths `/out` e `/pdb` foram isolados.
Fonte Roslyn: revisão `4cb203dab89e1542cdf7d88cb6d32767bc0e5460`, git-blobs
conferidos. A revisão exata de fabricação continua UNKNOWN.

- `LocalRewriter_IfStatement.cs:49–91`: a alternativa cria salto adicional
  para a junção; a forma sem else não o exige.
- `LocalRewriter_ContinueStatement.cs:12–16`: continue vira BoundGoto para
  o rótulo que já pertence ao laço.
- `LocalRewriter_WhileStatement.cs:45–96`: usa o ContinueLabel original e
  a condição do while para retornar ao início.
- `EmitStatement.cs:566–568`: emite Br para o destino do goto.
- `BasicBlock.cs:275–293`: salto incondicional que cruza os limites do
  handler vira Leave. Isso explica o153→2 observado na fixture/original.
- `EmitStatement.cs:676–701`: retorno protegido grava o LazyReturnTemp e
  salta ao retorno indireto, mantendo o boxed Boolean no objeto do slot5.

NoEmit real antes/depois: lambda genuinamente não mapeada, incompleta,
zero locais previstos (`generated_or_unmapped_source_method`). O pai é
incompleto e prevê os mesmos três locais antes/depois, com motivos
anonymous_function_or_capture, usos múltiplos e exception_lowering.
Não se forçou completude, não se afrouxou previsão completa errada e não se
alterou o modelo. A assinatura real da lambda exigirá a prova do operador.

Um diagnóstico simbólico privado reordena apenas as folhas atuais e
recalcula rótulos; prevê182bytes, zero diferenças de todos os opcodes,
operandos e offsets e EH inalterado. Não emite IL/DLL e não é oráculo.

## Histórico e auxiliares condicionais

Histórico capturado:53registros,26trials reais. Não há proposta anterior
de Show positivo com continue nem mesmo texto/hash completo do candidato.
As tentativas de hoist, condições, locais de retorno, raiz positiva e
espelhamento primitivo estão preservadas. Zero candidatos não é refutação.
`history-sha-audit.json` limita a busca aos textos/hashes capturados.

O receipt baseline12 já marca `.ctor` e `.cctor` desse `<>c` como
body_equal, mas generated_group_pending, porque esta lambda ainda difere.
Isso é evidência de dois auxiliares retidos, **não ganho desta tarefa**.
Somente o verificador pode liberá-los depois do corpo da lambda e da
checagem completa; não se edita seu estado nem os fontes gerados.

Produtor puro: `transform_start_retry_positive(parent_method_source)`;
família `r12-start-dialog-positive-retry-arm`. Bundle e índice:
`ready-for-game-probe.json`, `ready-freeze.json`. Um patch do método pai,
sem instalação ou alterações em APK/nativo/DEX. Depois da entrega esta
frente para; não há nova família neste escopo.
