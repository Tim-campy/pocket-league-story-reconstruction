# Image.Use — destino do retorno, preservando o hoist R10D34

Status privado: **READY_FOR_GAME_PROBE**, corpo do alvo **UNKNOWN**,0compilações/adoções do jogo. Base exclusiva `build/round10/final-corpus/KairoLibrary`. Não houve mudança pública, fonte alvo, modelo, ledger, configuração ou compilador. Nenhum estudo de Init2 foi iniciado.

## Medição atual e evidência primária

Alvo completo: `kairo.unity.ui.Image::<Use>b__96_0()->System.Void`; original680bytes, próprio683bytes. Ambos têm12locais com **tipos na mesma ordem**, cabeçalho fat12/flags27/initlocals=true e maxstack5. Após alinhar instruções e resolver destinos, há **0edições de sequência e1retarget real**. A diferença de largura de3bytes é consequência desse retarget.

REA original: artifact SHA256 `bb553d341eecd9b900f772486bf4a5c289aa2f09566d9d402a3eb70268f4b3ee`, MVID `b564fa31-554d-4b48-a8c0-a0b41bea2550`, token0x0600041c. Evidence de página `ev_62988ec6a4ca7b5c53f752c7238ab10b3efe9fc84ec0d1a134ec1de6a7a56792`, pai `ev_0ffbc96d1f2e2e1b3682f620df32e6aa4d78c880e8df9ee60f924cb2b3369555`. A seleção Image devolveu92métodos, cobertura complete sem issues, nenhuma execução do target.

`rea-byte-binding.json` comprova que os680bytes IL extraídos do raw body Cecil congelado têm o mesmo SHA observado pelo REA: `ed3b6fe63b67cae9c2c8852c6b70ae3403cd88dba2b09e299af0fbd669bd1dc0`. `diagnostics.json` contém ambos os corpos, cabeçalhos, EH, tipos e alinhamento. Os tokens são coordenadas somente desse SHA/MVID.

## Destinos efetivamente resolvidos

| Trecho | Original | Próprio finalR10 |
|---|---|---|
| Teste do array | brtrue.s@130→199, início do decode | brtrue.s@130→202, início do decode |
| Config.PRINT falso | brfalse.s@137→**198:ret** | brfalse@137→**682:ret** |
| Caminho após o log | pop@197;ret@198 | pop@200;ret@201 |
| Entrada do decode | ldc0@199;stloc4@200;nop@202 | ldc0@202;stloc4@203;nop@205 |

Os dois destinos do teste PRINT são RETs fora do catch de decode. Não há alteração de comportamento de arraynull/PRINTfalse sustentada por esse fato: ambos terminam a lambda. Os demais opcodes/operandos se alinham. O catch original é try203..219/handler219..232; próprio206..222/handler222..235, deslocado pelos3bytes anteriores.

## Hipótese incorreta retirada antes de qualquer alvo

Minha primeira leitura classificou erroneamente198como início do decode. A resolução da instrução mostrou que198é RET e199é o decode. A hipótese `if(array==null && PRINT){Log;return;}` foi **REFUTADA/RETIRADA ANTES DO TARGET**; ela deixaria PRINTfalse seguir ao decode e não representa o original. A correção foi comunicada ao root antes de produzir um candidato para o jogo.

Esse erro não foi escondido: a fixture contém uma variante própria WrongCompoundGuard. Ela foi compilada somente como controle negativo, falha na comparação integral com o controle correto e seu destino PRINTfalse não é RET. Nenhum fonte público ou alvo recebeu essa forma.

## Composição mínima calculada

O antes recuperado tem:

```csharp
if (array == null) {
    if (Config.PRINT) { /* Log.Info existente */ }
} else {
    // payload inteiro: decode, catch/goto, alfa, cache, atlas
}
```

A proposta coloca o retorno explícito no guard null e conserva o antigo else **como bloco simples**, com todo seu texto intacto:

```csharp
if (array == null) {
    if (Config.PRINT) { /* o mesmo Log.Info */ }
    return;
}
{
    // o mesmo payload, incluindo todas as declarações e o EH
}
```

Isso faz o label após o if PRINT apontar ao RET local explícito, antes do decode. Não se escolhe brfalse.s manualmente nem se tenta reduzir a distância por padding, labels artificiais ou bytes. Nenhuma expressão/call/string/folha é alterada ou reavaliada. O número do opcode curto resulta do destino correto.

`byte[] array=src_; int num2; int num3;` já foi adotado em R10D34 e permanece literalmente igual. Não há nova variável e o escopo filho do payload foi preservado. A lambda continua sendo criada pelo compilador a partir do pai Use; nenhuma classe ou método gerado foi escrito à mão. O produtor recusa baseline sem o hoist, log/corpo diferente, transferência preexistente no braço de log ou outro statement no guard null.

## Regra do Roslyn fixado

Revisão `4cb203dab89e1542cdf7d88cb6d32767bc0e5460`. Fontes em `pinned/`, autenticadas por SHA1 do Git blob e SHA256 em `provenance.json`:

- `src/Compilers/CSharp/Portable/Lowering/LocalRewriter/LocalRewriter_IfStatement.cs:40..99`: com else, emite label da alternativa e um BoundGoto para afterif depois do primeiro braço; sem else, emite apenas o conditional-goto e o label após a consequência. Um return explícito antes do bloco seguinte mantém a saída no guard, sem transformar esse bloco em else.
- `src/Compilers/CSharp/Portable/CodeGen/EmitStatement.cs:676..730`: fora do contexto que exige retorno indireto, emite ret para o retorno void. O log/guard está fora do catch do decode, portanto não se adiciona leave nem EH.
- `src/Compilers/Core/Portable/CodeGen/ILBuilder.cs:473..532`: ForwardLabelsNoLeaving segue labels vazios que só possuem Br ou Nop, respeitando o mesmo handler. Isso explica o encaminhamento do label do if PRINT ao afterif final na forma anterior.
- `src/Compilers/Core/Portable/CodeGen/BasicBlock.cs:487..511`: TryOptimizeBranchToNextOrRet só trata **Br/Br_s**, convertendo branch incondicional para ret quando seu destino é bloco vazio com Ret. Não aplica essa substituição a brfalse. Por isso uma origem pode terminar em RET local após o log enquanto a condicional ainda se dirige ao RET final.

O retorno explícito muda a árvore/labels do fonte, em vez de ajustar o tamanho do salto. A fixture confirma a emissão real da versão histórica para essa composição. A autoria histórica exata do fonte continua UNKNOWN.

## Fixtures e negativos

Quatro compilações próprias com csc1.1.0.51109 SHA256 `53b693536e8a86f35d05d2165ff9b3e0aeaa499ecfd8135a9f99ab4404ca86c3`, optimize+,debug:pdbonly,deterministic−,langversion6 e referências exatas do manifesto finalR10: before, controle manual independente, candidato e a hipótese errada como negativo.

- Before439→candidato436IL, exatamente−3.
-12locais iguais nas mesmas posições, maxstack4inalterado no toy.
- Todos os corpos do candidato, incluindo a lambda gerada pelo csc, iguais ao controle manual; before diferente.
- Alinhamento before/candidato:0edições de sequência,1retarget, branch PRINTfalse passa ao RET local.
- Início do EH do toy também diminui3, sem alteração de suas folhas/tipo.
- Compound guard negativo difere do controle e encaminha PRINTfalse a instrução que não é RET.
- Três negativos textuais recusam statement adicional, remoção do hoist já adotado e return preexistente nesse braço.

O toy usa classes próprias e não é a recompilação do alvo. Não se iguala seu maxstack4ao maxstack5do jogo: a prova é o mesmo maxstack entre before/candidato e o mesmo padrão de destino. A assinatura real do candidato do jogo continua UNKNOWN até o compilador oficial; nenhum ganho é contado aqui.

## Histórico sem repetição do candidato R10

`history.json` captura43registros para o ID exato. Existem7trials reais:2em R10D5 sem avanço88→88 e5em R10D34, com1passo adotado88→131. O passo foi o hoist `d10-residual-lambda-local-lifetimes-use`, que deixou as12posições atuais; os demais trials de guard/condition/ternário não fecharam.

`history-sha-audit.json` registra SHA dos relatórios e snippets e compara o candidato completo com os full-source SHA disponíveis: nenhum igual foi encontrado. A proposta atual não refaz o hoist; exige-o e só acrescenta return/retira o else estrutural. Proveniências antigas parciais não permitem alegar exaustividade global.

## NoEmit, entrega e limite

O modelo congelado foi executado com163fontes finalR10 vinculados por input_path/hash e helper já em cache, sem Emit. Resultado genuíno: complete=false,0tipos, `generated_or_unmapped_source_method`. Não foi convertido artificialmente em previsão de12tipos. Os12locais são fato do build original/próprio congelado e da fixture própria, não resposta inventada do modelo. O executor deverá confirmar a assinatura real antes de aceitar o candidato.

API pura: `calculate.py::transform_image_null_guard_return(method_source)->str`, sem I/O no import/chamada. Proposta: pai Use inteiro, UTF16 exato, antes/depois, SHA do arquivo/recibo/dumps/REA e regra `r11-d-initial-image-use-local-null-return`. Destinos, header, IL, EH e metadata do jogo continuam sujeitos ao oráculo completo. Após congelar este pacote a frente para, sem iniciar Init2.
