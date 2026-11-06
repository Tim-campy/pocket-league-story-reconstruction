# Hipóteses calculadas adicionais: identidade de resultado EH e dispatch de estado

READY para o oráculo; bundle publicado e congelado. **Corpos dos alvos UNKNOWN.** Nenhuma compilação do jogo ou alteração de Main/ledger/modelos foi feita. Os dois corpos artificiais de controle passaram integralmente pelo Roslyn 1.1 selecionado. Mudanças só de declarações continuaram diferentes: estas hipóteses não são repetições de uma mudança pura de lifetime da rodada 8.

Base NoEmit final: `build/round9/cycles/c11/after/KairoLibrary`. A análise global B10 foi reaproveitada apenas após confirmar os mesmos hashes dos dois fontes em B10, C11 e Main; os dois candidatos foram previstos novamente sobre C11. Medição B10: `stloc.s => stloc.s` passou de 8/7.984 na leitura A5 para **9 métodos/10.996 bytes**, porque Balloon.Draw avançou no B09 e mudou a primeira divergência. Isso não é regressão. Os exemplos investigados foram JarInflater.GetData, SaveRecorder.MakeSaveData e Seb.GetSprite/GetSpriteLocal.

## JarInflater.GetData — 189 bytes IL

ID: `kairo.unity.util.JarInflater::GetData(System.String)->System.Byte[]`.

Correspondência global diagnóstica por instruções de uso: original0→recompilado1 (InputStream), 1→2 (ByteArrayOutputStream), 2→5 (resultado Byte[]), 3→3 (buffer Byte[]), 4→0 (len Int32), 5→4 (resultado da leitura direta de Storage). O alinhamento trata essa correspondência apenas como evidência; não modifica ou normaliza os corpos aceitos.

Há uma diferença além da ordem: o original inicializa o **mesmo** local Byte[] do resultado antes do try e o retorna depois do finally. O recompilado considera `array2 = null` morto, remove a inicialização e cria um temporário de retorno protegido separado.

```
Original                         Recompilado
107 ldnull                       [ausente]
108 stloc.2                      [ausente]
...                              ...
146 callvirt ToByteArray          142 callvirt ToByteArray
151 stloc.2                      147 stloc.s 5
152 leave.s 187                  149 leave.s 184
... finally ...                  ... finally ...
187 ldloc.2                      184 ldloc.s 5
188 ret                          186 ret
```

Hipótese `d9-named-eh-result-rejoin`: conservar a declaração existente `byte[] array2 = null`, trocar o último `return byteArrayOutputStream.ToByteArray()` do try por `array2 = byteArrayOutputStream.ToByteArray()`, e retornar array2 após o finally. Mover a declaração sem inicializador `int len` para após array3 fixa os locais do bloco pai em InputStream, OutputStream, resultado, buffer, len, e deixa o array do primeiro if no slot5. A EH original é try [120,154), finally [154,187), com os catches internos [165,168) e [181,184). A leitura `ldloc.2 @187` ocorre **após** o finally, no limite exclusivo handler_end=187; não é uma leitura feita pelo finally. Nenhuma chamada, carga ou efeito do finally muda de ordem; os mesmos objetos são fechados e zerados. A leitura `Storage.Read` continua sendo uma chamada única.

Previsão NoEmit: **6 tipos exatamente iguais aos originais**, explicitamente incompleta (`stack_scheduler_multiple_use:array`, `exception_lowering`). O binário recompilado atual tem seis locais também, porém com identidades diferentes: trocar somente slots esconderia a inicialização ausente.

Fonte primária Roslyn: `CodeGen/EmitStatement.cs:643–664` documenta o retorno indireto em região protegida; `EmitReturnStatement`, 676–698, guarda em `LazyReturnTemp`. `CodeGen/CodeGenerator.cs:139–173` cria esse temporário e usa `AllocateTemp` no Release; `CodeGenerator.cs:291–295` lê-o no epílogo. Snapshot fixado `4cb203dab89e1542cdf7d88cb6d32767bc0e5460`; o commit exato de fabricação do csc instalado segue UNKNOWN. A forma com atribuição e retorno fora do try evita criar esse retorno protegido para esse comando.

## SaveRecorder.MakeSaveData — 1.109 bytes IL

ID: `kairo.unity.ui.SaveRecorder::MakeSaveData(kairo.unity.util.Property,kairo.unity.ui.SaveRecorder/AutoSaveLogData&)->kairo.unity.util.Property`.

A correspondência global só permuta i/flag: original7→recompilado8 e original8→recompilado7; os demais 25 slots têm a mesma identidade diagnóstica. O original contém uma leitura do estado no início do segundo braço que não existe nessa posição do recompilado, porque o fonte recuperado usa switch.

```
Original                         Recompilado
187 ldloc.s 6                    187 ldloc.s 6
189 brtrue 434                   189 brfalse.s 204
[código caso0]                   191 ldloc.s 6
426 ldc.i4.1                     193 ldc.i4.1
427 stloc.s 6                    194 beq 444
429 br 797                       199 br 799
434 ldloc.s 6                    204 [código caso0]
436 ldc.i4.1                     ...
437 bne.un 797                   444 [código caso1]
```

Hipótese `d9-calculated-state-dispatch-index`: declarar os símbolos num2 e i no bloco pai (`int num2, i;`) antes do primeiro for, preservando `num2 = 0` em seu ponto de execução original e mudando o cabeçalho para `for (i = 0; ...)`. O bool flag passa ao slot8; i fica no7. Além disso, reconstituir o dispatch como `if (num2 == 0) {...} else if (num2 == 1) {...}`.

O helper de cálculo aceita exatamente dois cases 0/1, cada um em seu bloco e terminado por um break **comprovadamente pertencente ao switch selecionado**. Retira somente esses dois breaks finais. Breaks de laços internos conservam seus donos. Qualquer outro break pertencente ao switch ou qualquer goto faz a proposta ser recusada, sem patch parcial. As expressões e chamadas internas, inclusive retornos, não são alteradas.

Previsão NoEmit: **27 tipos exatamente iguais aos originais**, incompleta por limitações já conhecidas de default, scheduler e EH. A fixture demonstra que mover só a declaração do índice não é suficiente: permanece o dispatch diferente.

Evidência primária: `EmitStatement.cs:594–613` define os locais de cada bloco antes dos comandos; `EmitStatement.cs:1018–1074` trata o cabeçalho do switch separadamente, enquanto as condições if chegam a `EmitConditionalGoto`, que chama `EmitCondBranch`. O resultado específico da cadeia if0/elseif1 foi confirmado pela fixture histórica; não se deduz equivalência byte a byte somente da semântica do switch.

## Controles e resultados negativos

Uma execução de fixtures compila três assemblies toy com o csc 1.1.0.51109, SHA-256 `53b693536e8a86f35d05d2165ff9b3e0aeaa499ecfd8135a9f99ab4404ca86c3`: baseline, variantes só de declaração e candidatos conjuntos. Os dois candidatos têm corpo completo igual ao controle independente (cabeçalho, tipos/ordem dos locais, IL e EH), exceto o nome da declaração de controle. Os dois controles só de declaração continuam diferentes. Um switch com break adicional no meio do case é recusado.

A primeira execução do calculador parou antes de qualquer Emit porque o `int len` existente em JarInflater usa LF isolado em um arquivo majoritariamente CRLF. A âncora foi corrigida para o texto bruto real; nenhuma normalização de newline foi feita. Os patches conservam UTF-16 e o before literal; o before/after pode conter os LF históricos já presentes.

O histórico examinado em `analysis.json` não tem trial compilada para estes dois alvos nas rodadas 7/8/9; os bloqueios prévios eram de modelo. A forma de controle if-chain já é conhecida em outros métodos, mas aqui a seleção e a composição com os símbolos corretos são novas e sustentadas pelos operandos/usos originais.

Seb.GetSprite (918 IL) e GetSpriteLocal (974 IL) foram investigados como controles negativos de novidade. Não apresentam instrução de local extra/faltante no alinhamento global; os contadores tardios no bloco pai explicam suas permutações. Portanto nenhuma variante pura de lifetime8 foi proposta para eles, conforme solicitado.

O máximo que as duas novas propostas poderiam fechar é **2 métodos/1.298 bytes IL**. Ganhos reais, prefixos e corpos permanecem UNKNOWN até o oráculo oficial. Não há necessidade de instalação, mudança de referência ou atualização do modelo para testá-las pela política de incompletos da rodada 9.

## Arquivos e reprodução publicados

`python -X utf8 tools/round9_eh_dispatch_calculate.py` faz somente a previsão NoEmit sobre C11. `python -X utf8 tools/round9_eh_dispatch_fixtures.py` compila somente os controles artificiais (três assemblies por execução). A preparação executou as fixtures duas vezes, totalizando seis compilações toy e zero jogo. Os resultados congelados são `reports/round9-d-eh-dispatch-{patches,fixtures,evidence}.json`.

As duas propostas são métodos completos para o motor com `--external-proposals`, filtrados pela família: `d9-named-eh-result-rejoin` ou `d9-calculated-state-dispatch-index`. O helper `tools/Round9CalculatedDispatchRules.cs`, entry `AddRound9CalculatedDispatchRules`, calcula somente a troca de dispatch de SaveRecorder; ele não deve ser usado isoladamente como substituto do candidato conjunto. O calculator compõe o índice declarado no pai. Nenhuma integração em dispatcher/config ativo foi executada.
