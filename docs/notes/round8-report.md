## 1. PROGRESSO

Pocket League Story para Android, APK net.kairosoft.android.soccer_en_2.0.2-APK_Award.apk; reconstrução C#/Unity 5.3.4f1. Comparação com o início desta rodada, commit `25b8108711a4a8920a2600cf9ffea5b710b3a481`. Matched exige corpo completo (cabeçalho, maxstack, locais, IL e EH); referências de tokens são resolvidas por identidade. Declarações sem corpo contam em métodos, sem bytes de IL.

| Assembly | Anterior (25b8108) | Atual | Δ métodos | Δ bytes de IL | Regressões |
| --- | --- | --- | --- | --- | --- |
| KairoLibrary | 3207/3287 (97.566170%); IL 212900/269894 bytes (78.882821%) | 3211/3287 (97.687861%); IL 216823/269894 bytes (80.336354%) | 4 | 3923 | 0 |
| Assembly-CSharp | 1789/1958 (91.368744%); IL 352326/601603 bytes (58.564535%) | 1790/1958 (91.419816%); IL 352587/601603 bytes (58.607919%) | 1 | 261 | 0 |

Total: 5001/5245 métodos; 569410/871497 bytes de IL. Retiradas de matched por ID: 0; corpos antes iguais que deixaram de bater: 0.

Motivo da parada: três ciclos válidos consecutivos sem novo método em matched (5, 6 e 7).

| Original | SHA-256 |
| --- | --- |
| APK | `d557af6b4bc7748605a39ed3548821f6763c5773a12432a3fed6e8c413b211e8` |
| KairoLibrary.dll | `bb553d341eecd9b900f772486bf4a5c289aa2f09566d9d402a3eb70268f4b3ee` |
| Assembly-CSharp.dll | `3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949` |

## 2. O QUE FOI FEITO NESTA RODADA

Ganhos oficiais vêm do diário após adoção e verificação completa, incluindo auxiliares liberados pelo verificador. Sondas e avanços parciais são registrados separadamente; não se somam aos ganhos oficiais. Transformações herdadas da rodada 7 só aparecem abaixo quando foram efetivamente tentadas nesta rodada.

| Ciclo | Compilador / par | Hipótese | Resultado | Matched ganhos | Bytes ganhos |
| --- | --- | --- | --- | --- | --- |
| 1 | Assembly-CSharp / stloc.s=>stloc.s | Ancoras unicas de stores e intervalos sem interleaving calculam o bloco comum; reservas apenas para slots originais comprovadamente sem uso. | Novos corpos aprovados; 2 tentativas da regra, 2 avancos | 1 | 261 |
| 2 | Assembly-CSharp / guard return=>positive if/else | SpForm.Update conserva o bloco positivo antes do retorno alternativo quando a guarda recuperada inverte a ordem original dos blocos. | Nenhum candidato compilado avancou; 6 tentativas da regra, 0 avancos | 0 | 0 |
| 3 | KairoLibrary / stloc.s=>stloc.s | O modelo void try-finally preserva o renderPass vivo no handler; indices e escopos calculados recuperam os 35 slots de Graphics.DrawPolygons. | Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 1563 |
| 4 | KairoLibrary / brtrue.s=>switch | Repeticao valida do ciclo18 anterior: switch vira if preservando donos de break; candidato incompleto realmente escrito no arquivo de screen e compilado com prova SHA. | Novos corpos aprovados; 6 tentativas da regra, 6 avancos | 3 | 2360 |
| 5 | KairoLibrary / brfalse.s=>brtrue.s | Repeticao valida do ciclo19 anterior: null conditional-coalesce vira teste explicito de nulo; screen atualizado com candidato e provas SHA; modelo incompleto permite compilar. | Nenhum candidato compilado avancou; 1 tentativas da regra, 0 avancos | 0 | 0 |
| 6 | Assembly-CSharp / bne.un=>beq | Init2 requer seletor de switch nomeado no slot32 antes da guarda positiva com break; a composicao calculada iguala68slots e preserva uma avaliacao do seletor. | Nenhum candidato compilado avancou; 1 tentativas da regra, 0 avancos | 0 | 0 |
| 7 | Assembly-CSharp / bne.un=>beq | SpForm.Update compartilha quatro byte arrays no slot11, reserva Dialog13 comprovadamente sem uso, remove temporario de switch e restaura guarda positiva apenas depois de obter24slots. | Avanco parcial; nenhum novo matched; 1 tentativas da regra, 1 avancos | 0 | 0 |

C3: o patch DrawPolygons havia sido exportado com LF para uma baseline CRLF. O rebase exato o recusou; a tentativa inicial foi preservada. O transporte corrigiu somente o patch, sem editar o fonte-base; o retry verificou o corpo completo de 1563 bytes.

C4/C5 iniciais foram experimentos INVÁLIDOS: nove triagens rotuladas como candidatos (sete das varreduras e duas sondas) compilavam o arquivo-base desatualizado. Não houve avanço, adoção, promoção ou perda. A função de escrita foi corrigida e testada em cinco compilações toy; os dois experimentos foram arquivados por ferramenta auditável e repetidos como ciclos válidos 4 e 5. Git e os hashes dos ledgers foram preservados; não houve edição manual de estados. O diário científico tem sete ciclos concluídos, além desses dois experimentos inválidos.

Foram removidos apenas 53 arquivos de cache de metadados regenerável (369180435 bytes lógicos), preservando os 16 mais recentes. Compressão NTFS sem perdas liberou espaço nos artefatos de build; não alterou bytes dos assemblies. Fontes, originais e provas congeladas foram preservados.

Frentes preparadas antes da parada, sem teste dos respectivos candidatos no jogo; ganhos e perdas adotados: zero.

| Frente | Preparado / prova | Compilações de candidatos do jogo | Métodos ganhos / perdidos |
| --- | --- | --- | --- |
| Coalescência de coordenadas smcs | 3 alvos; 4316 bytes; 2 corpos de fixture exatos | 0 | 0 / 0 |
| Lifetime/escopo Roslyn | 3 alvos; 4492 bytes; 3 corpos de fixture exatos | 0 | 0 / 0 |
| Ternário negativo após null-conditional | 1 alvos; 1686 bytes; 3 corpos de fixture exatos | 0 | 0 / 0 |
| Expansão de *= em campo float/double | UNKNOWN alvos; UNKNOWN bytes; 4 corpos de fixture exatos | 0 | 0 / 0 |

| Execução | Natureza | Transformação | Variantes tentadas | Avanços | Corpos ganhos no motor | Bytes no motor | Perdas adotadas |
| --- | --- | --- | --- | --- | --- | --- | --- |
| round8-cycle01-Assembly-CSharp-engine.json | plena adotada | assignment-ternary-if | 1 | 0 | 0 | 0 | 0 |
| round8-cycle01-Assembly-CSharp-engine.json | plena adotada | assignment-ternary-if-reverse | 1 | 1 | 1 | 261 | 0 |
| round8-cycle01-Assembly-CSharp-engine.json | plena adotada | calculated-store-anchored-local-lca | 2 | 2 | 0 | 0 | 0 |
| round8-cycle01-Assembly-CSharp-engine.json | plena adotada | solver-group-all-calculated-blocks | 2 | 0 | 0 | 0 | 0 |
| round8-cycle01-Assembly-CSharp-engine.json | plena adotada | solver-group-block-declarations | 6 | 0 | 0 | 0 | 0 |
| round8-cycle01-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | calculated-store-anchored-local-lca | 1 | 1 | 0 | 0 | não adotado |
| round8-cycle01-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | solver-group-all-calculated-blocks | 1 | 0 | 0 | 0 | não adotado |
| round8-cycle01-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | solver-group-block-declarations | 5 | 0 | 0 | 0 | não adotado |
| round8-cycle01-probe2-Assembly-CSharp-engine.json | sonda/preparo/não adotada | assignment-ternary-if | 1 | 0 | 0 | 0 | não adotado |
| round8-cycle01-probe2-Assembly-CSharp-engine.json | sonda/preparo/não adotada | assignment-ternary-if-reverse | 1 | 1 | 1 | 261 | não adotado |
| round8-cycle01-probe2-Assembly-CSharp-engine.json | sonda/preparo/não adotada | calculated-store-anchored-local-lca | 1 | 1 | 0 | 0 | não adotado |
| round8-cycle01-probe2-Assembly-CSharp-engine.json | sonda/preparo/não adotada | solver-group-all-calculated-blocks | 1 | 0 | 0 | 0 | não adotado |
| round8-cycle01-probe2-Assembly-CSharp-engine.json | sonda/preparo/não adotada | solver-group-block-declarations | 1 | 0 | 0 | 0 | não adotado |
| round8-cycle02-Assembly-CSharp-engine.json | plena adotada | smcs-value-return-guard-reverse | 6 | 0 | 0 | 0 | 0 |
| round8-cycle03-KairoLibrary-engine.json | plena adotada | roslyn-drawpolygons-declaration-scope-and-explicit-array-indices | 1 | 1 | 1 | 1563 | 0 |
| round8-cycle03-probe-KairoLibrary-engine.json | sonda/preparo/não adotada | roslyn-drawpolygons-declaration-scope-and-explicit-array-indices | 1 | 1 | 1 | 1563 | não adotado |
| round8-cycle04-initial-screen-KairoLibrary-engine.json | INVALIDO: screen usou baseline | switch-owned-break-if-chain | 6 | 0 | 0 | 0 | não adotado |
| round8-cycle04-initial-screen-probe-KairoLibrary-engine.json | INVALIDO: screen usou baseline | switch-owned-break-if-chain | 1 | 0 | 0 | 0 | não adotado |
| round8-cycle04-KairoLibrary-engine.json | plena adotada | alias-field-command | 2 | 0 | 0 | 0 | 0 |
| round8-cycle04-KairoLibrary-engine.json | plena adotada | alias-field-occurrence | 3 | 0 | 0 | 0 | 0 |
| round8-cycle04-KairoLibrary-engine.json | plena adotada | assignment-ternary-if | 5 | 1 | 0 | 0 | 0 |
| round8-cycle04-KairoLibrary-engine.json | plena adotada | assignment-ternary-if-reverse | 4 | 3 | 0 | 0 | 0 |
| round8-cycle04-KairoLibrary-engine.json | plena adotada | if-polarity | 1 | 1 | 0 | 0 | 0 |
| round8-cycle04-KairoLibrary-engine.json | plena adotada | local-declaration-scope-hoist | 6 | 0 | 0 | 0 | 0 |
| round8-cycle04-KairoLibrary-engine.json | plena adotada | switch-owned-break-if-chain | 6 | 6 | 3 | 2360 | 0 |
| round8-cycle04-KairoLibrary-engine.json | plena adotada | ternary-polarity | 1 | 0 | 0 | 0 | 0 |
| round8-cycle04-probe-KairoLibrary-engine.json | sonda/preparo/não adotada | switch-owned-break-if-chain | 1 | 1 | 1 | 1345 | não adotado |
| round8-cycle05-initial-screen-KairoLibrary-engine.json | INVALIDO: screen usou baseline | nullable-conditional-coalesce-explicit | 1 | 0 | 0 | 0 | não adotado |
| round8-cycle05-initial-screen-probe-KairoLibrary-engine.json | INVALIDO: screen usou baseline | nullable-conditional-coalesce-explicit | 1 | 0 | 0 | 0 | não adotado |
| round8-cycle05-KairoLibrary-engine.json | plena adotada | nullable-conditional-coalesce-explicit | 1 | 0 | 0 | 0 | 0 |
| round8-cycle05-probe-KairoLibrary-engine.json | sonda/preparo/não adotada | nullable-conditional-coalesce-explicit | 1 | 0 | 0 | 0 | não adotado |
| round8-cycle06-Assembly-CSharp-engine.json | plena adotada | calculated-selector-continue-composition | 1 | 0 | 0 | 0 | 0 |
| round8-cycle06-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | calculated-selector-continue-composition | 1 | 0 | 0 | 0 | não adotado |
| round8-cycle07-Assembly-CSharp-engine.json | plena adotada | calculated-shared-payload-switch-guard | 1 | 1 | 0 | 0 | 0 |
| round8-cycle07-Assembly-CSharp-engine.json | plena adotada | if-polarity | 1 | 1 | 0 | 0 | 0 |
| round8-cycle07-Assembly-CSharp-engine.json | plena adotada | smcs-empty-arm-restore | 4 | 1 | 0 | 0 | 0 |
| round8-cycle07-Assembly-CSharp-engine.json | plena adotada | solver-group-all-calculated-blocks | 3 | 0 | 0 | 0 | 0 |
| round8-cycle07-Assembly-CSharp-engine.json | plena adotada | solver-group-block-declarations | 6 | 0 | 0 | 0 | 0 |
| round8-cycle07-Assembly-CSharp-engine.json | plena adotada | ternary-polarity | 1 | 0 | 0 | 0 | 0 |
| round8-cycle07-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | calculated-shared-payload-switch-guard | 1 | 1 | 0 | 0 | não adotado |
| round8-cycle07-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | if-polarity | 1 | 1 | 0 | 0 | não adotado |
| round8-cycle07-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | smcs-empty-arm-restore | 4 | 1 | 0 | 0 | não adotado |
| round8-cycle07-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | solver-group-all-calculated-blocks | 3 | 0 | 0 | 0 | não adotado |
| round8-cycle07-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | solver-group-block-declarations | 6 | 0 | 0 | 0 | não adotado |
| round8-cycle07-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | ternary-polarity | 1 | 0 | 0 | 0 | não adotado |

| Execução | Métodos/passes | Compilações falhas | Regressões rejeitadas | Metadados rejeitados | Modelo/emissor discordam | Assinatura original recusada | Rebase externo recusado | Orçamento incompleto recusado | Paradas |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| round8-cycle01-Assembly-CSharp-engine.json | 168 | 0 | 0 | 0 | 0 | 0 | 2 | 0 | body_equal: 1, no_model_approved_candidate: 164, no_transformation_advances: 1, source_mapping_UNKNOWN: 2 |
| round8-cycle01-probe-Assembly-CSharp-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 1 | 0 | no_transformation_advances: 1 |
| round8-cycle01-probe2-Assembly-CSharp-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 1 | 0 | body_equal: 1 |
| round8-cycle02-Assembly-CSharp-engine.json | 167 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | no_model_approved_candidate: 160, no_transformation_advances: 5, source_mapping_UNKNOWN: 2 |
| round8-cycle02-probe-Assembly-CSharp-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | no_model_approved_candidate: 1 |
| round8-cycle03-initial-KairoLibrary-engine.json | 78 | 0 | 0 | 0 | 0 | 0 | 1 | 0 | no_model_approved_candidate: 78 |
| round8-cycle03-initial-probe-KairoLibrary-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 1 | 0 | no_model_approved_candidate: 1 |
| round8-cycle03-KairoLibrary-engine.json | 78 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1, no_model_approved_candidate: 77 |
| round8-cycle03-probe-KairoLibrary-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1 |
| round8-cycle04-initial-screen-KairoLibrary-engine.json | 77 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | no_model_approved_candidate: 72, no_transformation_advances: 5 |
| round8-cycle04-initial-screen-probe-KairoLibrary-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | no_transformation_advances: 1 |
| round8-cycle04-KairoLibrary-engine.json | 77 | 0 | 0 | 0 | 0 | 3 | 0 | 12 | body_equal: 3, incomplete_candidate_budget: 2, no_model_approved_candidate: 72 |
| round8-cycle04-probe-KairoLibrary-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1 |
| round8-cycle05-initial-screen-KairoLibrary-engine.json | 77 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | no_model_approved_candidate: 76, no_transformation_advances: 1 |
| round8-cycle05-initial-screen-probe-KairoLibrary-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | no_transformation_advances: 1 |
| round8-cycle05-KairoLibrary-engine.json | 74 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | no_model_approved_candidate: 73, no_transformation_advances: 1 |
| round8-cycle05-probe-KairoLibrary-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | no_transformation_advances: 1 |
| round8-cycle06-Assembly-CSharp-engine.json | 167 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | no_model_approved_candidate: 164, no_transformation_advances: 1, source_mapping_UNKNOWN: 2 |
| round8-cycle06-probe-Assembly-CSharp-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | no_transformation_advances: 1 |
| round8-cycle07-Assembly-CSharp-engine.json | 167 | 0 | 0 | 0 | 0 | 0 | 3 | 0 | no_model_approved_candidate: 164, no_transformation_advances: 1, source_mapping_UNKNOWN: 2 |
| round8-cycle07-probe-Assembly-CSharp-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 3 | 0 | no_transformation_advances: 1 |

| Execução | Variantes incompletas compiladas | Invocações destas variantes | Avanços aceitos | Métodos no limite |
| --- | --- | --- | --- | --- |
| round8-cycle01-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round8-cycle01-probe-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round8-cycle01-probe2-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round8-cycle02-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round8-cycle02-probe-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round8-cycle03-initial-KairoLibrary-engine.json | 0 | 0 | 0 | 0 |
| round8-cycle03-initial-probe-KairoLibrary-engine.json | 0 | 0 | 0 | 0 |
| round8-cycle03-KairoLibrary-engine.json | 0 | 0 | 0 | 0 |
| round8-cycle03-probe-KairoLibrary-engine.json | 0 | 0 | 0 | 0 |
| round8-cycle04-KairoLibrary-engine.json | 28 | 39 | 11 | 2 |
| round8-cycle04-probe-KairoLibrary-engine.json | 1 | 2 | 1 | 0 |
| round8-cycle05-KairoLibrary-engine.json | 1 | 1 | 0 | 0 |
| round8-cycle05-probe-KairoLibrary-engine.json | 1 | 1 | 0 | 0 |
| round8-cycle06-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round8-cycle06-probe-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round8-cycle07-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round8-cycle07-probe-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |

## 3. HIPÓTESES

| Estado | Hipótese | Evidência medida |
| --- | --- | --- |
| CONFIRMADA | calculated-store-anchored-local-lca: Ancoras unicas de stores e intervalos sem interleaving calculam o bloco comum; reservas apenas para slots originais comprovadamente sem uso. | cycle01: 1 matched, 261 IL bytes; Novos corpos aprovados; 2 tentativas da regra, 2 avancos |
| INCONCLUSIVA | smcs-value-return-guard-reverse: SpForm.Update conserva o bloco positivo antes do retorno alternativo quando a guarda recuperada inverte a ordem original dos blocos. | cycle02: 0 matched, 0 IL bytes; Nenhum candidato compilado avancou; 6 tentativas da regra, 0 avancos |
| CONFIRMADA | roslyn-drawpolygons-declaration-scope-and-explicit-array-indices: O modelo void try-finally preserva o renderPass vivo no handler; indices e escopos calculados recuperam os 35 slots de Graphics.DrawPolygons. | cycle03: 1 matched, 1563 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos |
| CONFIRMADA | switch-owned-break-if-chain: Repeticao valida do ciclo18 anterior: switch vira if preservando donos de break; candidato incompleto realmente escrito no arquivo de screen e compilado com prova SHA. | cycle04: 3 matched, 2360 IL bytes; Novos corpos aprovados; 6 tentativas da regra, 6 avancos |
| INCONCLUSIVA | nullable-conditional-coalesce-explicit: Repeticao valida do ciclo19 anterior: null conditional-coalesce vira teste explicito de nulo; screen atualizado com candidato e provas SHA; modelo incompleto permite compilar. | cycle05: 0 matched, 0 IL bytes; Nenhum candidato compilado avancou; 1 tentativas da regra, 0 avancos |
| INCONCLUSIVA | calculated-selector-continue-composition: Init2 requer seletor de switch nomeado no slot32 antes da guarda positiva com break; a composicao calculada iguala68slots e preserva uma avaliacao do seletor. | cycle06: 0 matched, 0 IL bytes; Nenhum candidato compilado avancou; 1 tentativas da regra, 0 avancos |
| INCONCLUSIVA | calculated-shared-payload-switch-guard: SpForm.Update compartilha quatro byte arrays no slot11, reserva Dialog13 comprovadamente sem uso, remove temporario de switch e restaura guarda positiva apenas depois de obter24slots. | cycle07: 0 matched, 0 IL bytes; Avanco parcial; nenhum novo matched; 1 tentativas da regra, 1 avancos |
| CONFIRMADA | Modelo Roslyn estreito para void try/finally pode preservar o local vivo no handler | DrawPolygons tem 35 slots e corpo completo de1563 bytes aprovado em C3; baseline-modelo passou de2228 para2236 completos/exatos no mesmo corpus3131, sem retirar cobertura nem criar erro completo. |
| CONFIRMADA | Candidatos incompletos precisam ser compilados a partir do arquivo alternativo atualizado | Auditoria preservou nove triagens inválidas; fixture compartilhada de cinco compilações distinguiu baseline, candidato e reset. C4 válido ganhou três métodos/2360 IL após a correção, sem regressão. |
| REFUTADA | A ordem textual de toda construção C# determina a ordem dos blocos no IL | Roslyn1.1 EmitExpression.cs:2646-2647 emite Alternative primeiro;2689-2690 emite Consequence depois. Fonte pinned4cb203dab89e1542cdf7d88cb6d32767bc0e5460, SHA4beb87ae114cd851204eed776ed12f9cc8d533b86299d8683bf455c212876f09. Fixture negativa reproduziu14 bytes iniciais do StreamUtil, não o corpo do jogo. |
| REFUTADA | Basta expandir null-conditional/coalesce para um ternário positivo para corrigir WriteArray | C5 válido compilou uma variante por passagem com assinatura original, mas manteve a diferença no byte1. O emissor põe o braço alternativo primeiro; a composição negativa foi provada apenas em fixture e não testada no jogo. |
| REFUTADA | A composição atual do seletor e guarda de Init2 preserva o prefixo necessário | C6 confirmou os68 slots originais emitidos, porém o screen real reduziu o prefixo bruto43 para9; candidato recusado e nenhuma fonte adotada. |
| CONFIRMADA | Convergência dos payloads permite corrigir a assinatura e avançar a guarda de SpForm.Update | C7 confirmou24 locais e preservou todos os matched; composição mais duas regras herdadas avançaram o prefixo295→717→1244→1279. Corpo2052 continua divergente; não houve ganho de métodos/IL matched. |
| INCONCLUSIVA | Novos bundles de coordenadas, lifetime Roslyn e polaridade nula fecham seus métodos do jogo | Foram preparados e validados em fontes/fixtures antes da parada; não houve compilação dos candidatos do jogo dessas famílias. Não entram em matched. |

Diagnóstico de polaridade da rodada 8, no snapshot congelado indicado pela prova: escopo limitado às fronteiras selecionadas, não a todos os desvios dos métodos. A correção sobre Act_dribble descreve um teste histórico da rodada 7, sem nova tentativa.

| Estado | Hipótese diagnosticada | Evidência / limite |
| --- | --- | --- |
| REFUTADA | Polarity groups primarily indicate while/do/for conversion | 0 selected source blocks are natural-loop headers;0 selected edges are dominator backedges.2 are body if/break boundaries,6 internal conditionals,18 outside normal natural loops. |
| REFUTADA | Act_dribble failed through source mapping or missing slot-model approval | REFUTED: C17 already mapped the correct if, approved the signature, compiled it, and rejected raw prefix1538->1258. No retry proposed. |

Fonte do compilador consultada: `analysis/tools/round5-mono-source/pre-release/mcs/mcs/statement.cs`; revisão `b6587926c0f146c8b12985744f33a13b39a22d57`; SHA-256 `063a437095e1d608d045383d76291c9359e9a98220eea7750018a3d42712437a`. Trechos While.DoEmit 421-467, Do.DoEmit 322-351, For.DoEmit 568-619. Commit de fabricação do smcs instalado: UNKNOWN.

| Assembly/modelo | Matched do corpus | Mapeados | Sem mapeamento | Completos | Completos exatos | Precisão | Cobertura | Mapeados incompletos | Incompletos com tipos iguais |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| KairoLibrary | 3135 | 2804 | 331 | 2237 | 2237 | 100.000000% | 71.355662% | 567 | 310 |
| Assembly-CSharp | 1784 | 1704 | 80 | 1704 | 1704 | 100.000000% | 95.515695% | 0 | 0 |

Precisão usa somente previsões completas; cobertura usa todos os corpos matched do corpus congelado. Incompletos ou métodos gerados sem mapeamento continuam UNKNOWN. Coincidência de tipos em previsão incompleta não é prova do lowering. Previsão incompleta PERMITE compilar: limite de 12 variantes por método/passagem, 24 por método/ciclo, no máximo uma sonda isolada e uma passagem plena. Cada variante pode exigir screen e compilação plena; variantes e invocações são medidas separadamente. Previsão completa com assinatura errada é recusada antes de compilar. Depois da compilação, a assinatura emitida precisa ser a original; previsão completa também precisa concordar com o emissor. Cabeçalho, locais, IL e EH continuam sujeitos ao mesmo verificador integral.

Limitações dos modelos; motivos não exclusivos, sem crédito por previsões vazias:

| Assembly | Motivo agrupado | Ocorrências |
| --- | --- | --- |
| Assembly-CSharp | generated_or_unmapped_source_method | 80 |
| KairoLibrary | stack_scheduler_multiple_use | 412 |
| KairoLibrary | generated_or_unmapped_source_method | 331 |
| KairoLibrary | anonymous_function_or_capture | 130 |
| KairoLibrary | exception_lowering | 88 |
| KairoLibrary | stack_scheduler_single_use | 58 |
| KairoLibrary | foreach_iteration_stack_scheduler | 39 |
| KairoLibrary | string_concat_value_temporary | 24 |
| KairoLibrary | conditional_access_temporary | 23 |
| KairoLibrary | foreach_collection_local_stack_scheduler | 20 |
| KairoLibrary | finally_written_external_local_scheduler | 16 |
| KairoLibrary | rvalue_value_type_member_temporary | 15 |
| KairoLibrary | lock_lowering | 13 |
| KairoLibrary | array_postfix_temporary | 11 |
| KairoLibrary | default_expression_temporary | 9 |
| KairoLibrary | switch_lowering | 8 |
| KairoLibrary | foreach_protected_return | 5 |
| KairoLibrary | object_or_collection_initializer | 5 |
| KairoLibrary | using_lowering | 4 |
| KairoLibrary | ref_assignment_value_temporary | 4 |
| KairoLibrary | iterator_state_machine | 2 |
| KairoLibrary | foreach_runtime_dispose | 1 |

| Modelo em uso | Fonte | Client | Solver | Validação final | Corpus final |
| --- | --- | --- | --- | --- | --- |
| KairoLibrary | tools/Round8RoslynModel.cs | tools/round8_roslyn_model.py | tools/round7_roslyn_solver.py | reports/round8-final-roslyn-model-validation.json | build/round8/final-model-corpus/KairoLibrary |
| Assembly-CSharp | tools/Round7SlotModel.cs | tools/round7_slot_model.py | tools/round7_slot_solver.py | reports/round8-final-smcs-model-validation.json | build/round8/final-model-corpus/Assembly-CSharp |

## 4. PENDENTES POR GRUPO

| Assembly | Faixa de IL original | Corpos pendentes | Bytes de IL |
| --- | --- | --- | --- |
| KairoLibrary | <=64 | 3 | 137 |
| KairoLibrary | 65-256 | 26 | 4168 |
| KairoLibrary | 257-1024 | 26 | 14592 |
| KairoLibrary | >1024 | 19 | 34156 |
| Assembly-CSharp | <=64 | 5 | 237 |
| Assembly-CSharp | 65-256 | 30 | 4827 |
| Assembly-CSharp | 257-1024 | 65 | 36268 |
| Assembly-CSharp | >1024 | 67 | 207677 |

Pendentes com assinatura de locais igual: KairoLibrary 32/74 (diferente: 42); Assembly-CSharp 92/167 (diferente: 75).

Corpos já iguais, retidos por vínculo/metadados gerados: 3; 25 bytes. Esses corpos não entram no histograma dos divergentes.

| Assembly | ID | Bytes |
| --- | --- | --- |
| KairoLibrary | kairo.unity.system.KairoBase/<>c::.cctor()->System.Void | 11 |
| KairoLibrary | kairo.unity.system.KairoBase/<>c::.ctor()->System.Void | 7 |
| Assembly-CSharp | <PrivateImplementationDetails>::.ctor()->System.Void | 7 |

Peso de um par = soma do tamanho integral dos métodos nele; não representa quantidade de bytes diferentes. Alinhamento e rótulos servem ao diagnóstico, sem normalizar o verificador. Seguem os cinco pares de opcodes mais pesados de cada compilador, com IL original e recompilado lado a lado.

| KairoLibrary — par | Métodos | Peso IL |
| --- | --- | --- |
| `stloc.s => stloc.s` | 10 | 10498 |
| `stloc.s => DELETION_POINT` | 3 | 5215 |
| `brfalse.s => brtrue.s` | 5 | 4079 |
| `brfalse => brtrue.s` | 2 | 4071 |
| `ldc.i4.0 => brtrue.s` | 1 | 3513 |
| `stloc.2 => stloc.s` | 1 | 2403 |
| `conv.r4 => DELETION_POINT` | 2 | 2345 |
| `switch => switch` | 1 | 2305 |
| `INSERTION_POINT => stloc.s` | 3 | 1765 |
| `INSERTION_POINT => ldc.i4.0` | 2 | 1539 |
| `stloc.2 => stloc.3` | 1 | 1125 |
| `brtrue.s => ldc.i4.4` | 1 | 1054 |
| `brfalse => brfalse.s` | 1 | 955 |
| `stloc.2 => stloc.1` | 1 | 944 |
| `ldarg.0 => DELETION_POINT` | 4 | 823 |
| `br.s => leave.s` | 1 | 789 |
| `stloc.s => stloc.2` | 1 | 784 |
| `stloc.0 => stloc.2` | 2 | 770 |
| `INSERTION_POINT => brtrue.s` | 2 | 737 |
| `stloc.3 => stloc.1` | 1 | 680 |
| `ldc.i4.1 => DELETION_POINT` | 1 | 661 |
| `stloc.0 => stloc.1` | 2 | 576 |
| `brfalse.s => brtrue` | 1 | 537 |
| `brtrue.s => brfalse.s` | 2 | 467 |
| `stloc.3 => stloc.s` | 1 | 416 |
| `leave => br` | 1 | 383 |
| `bne.un.s => beq` | 1 | 372 |
| `stloc.1 => stloc.3` | 1 | 310 |
| `ldloc.1 => ldelema` | 1 | 275 |
| `INSERTION_POINT => stloc.2` | 2 | 251 |
| `ldelem.ref => DELETION_POINT` | 1 | 216 |
| `ble.s => ble.s` | 1 | 215 |
| `stloc.0 => dup` | 1 | 199 |
| `INSERTION_POINT => dup` | 1 | 197 |
| `ldelem.any => ldelema` | 1 | 192 |
| `br.s => leave` | 1 | 191 |
| `bne.un.s => beq.s` | 1 | 182 |
| `leave => br.s` | 1 | 175 |
| `leave.s => br.s` | 1 | 164 |
| `stloc.3 => stloc.0` | 1 | 149 |
| `constrained. => call` | 1 | 128 |
| `INSERTION_POINT => stloc.1` | 1 | 105 |
| `stloc.0 => stloc.3` | 1 | 94 |
| `stloc.1 => DELETION_POINT` | 1 | 91 |
| `ldloc.0 => DELETION_POINT` | 1 | 67 |
| `stloc.2 => stloc.0` | 1 | 46 |

**KairoLibrary #1: `stloc.s => stloc.s` — 10 métodos, 10498 bytes.** Exemplo: `kairo.unity.util.JarInflater::GetData(System.String)->System.Byte[]`.

Tentativas compiladas no grupo na rodada 8: nenhum. Histórico 7 compatível: UNKNOWN; não se presume compatibilidade após alteração do fonte. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 432. Paradas: no_model_approved_candidate: 40.

| IL original | IL recompilado |
| --- | --- |
| ```IL_001a: call class System.String::Concat``0(class System.String,class System.String)->class System.String;this=False;explicit=False;cc=Default``` | ```IL_001a: call class System.String::Concat``0(class System.String,class System.String)->class System.String;this=False;explicit=False;cc=Default``` |
| ```IL_001f: call class kairo.unity.io.Storage::Read``0(valuetype System.Int32,valuetype System.Boolean,class System.String)->valuetype System.Byte[];this=False;explicit=False;cc=Default``` | ```IL_001f: call class kairo.unity.io.Storage::Read``0(valuetype System.Int32,valuetype System.Boolean,class System.String)->valuetype System.Byte[];this=False;explicit=False;cc=Default``` |
| `IL_0024: stloc.s 5` | `IL_0024: stloc.s 4` |
| `IL_0026: ldloc.s 5` | `IL_0026: ldloc.s 4` |
| `IL_0028: brfalse.s L0` | `IL_0028: brfalse.s L0` |
| `IL_002a: ldloc.s 5` | `IL_002a: ldloc.s 4` |

EH original: `[{"kind": "Catch", "try_start": 157, "try_end": 165, "handler_start": 165, "handler_end": 168, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"}, {"kind": "Catch", "try_start": 173, "try_end": 181, "handler_start": 181, "handler_end": 184, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"}, {"kind": "Finally", "try_start": 120, "try_end": 154, "handler_start": 154, "handler_end": 187, "filter_start": -1, "catch_type": null}]`; recompilado: `[{"kind": "Catch", "try_start": 154, "try_end": 162, "handler_start": 162, "handler_end": 165, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"}, {"kind": "Catch", "try_start": 170, "try_end": 178, "handler_start": 178, "handler_end": 181, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"}, {"kind": "Finally", "try_start": 118, "try_end": 151, "handler_start": 151, "handler_end": 184, "filter_start": -1, "catch_type": null}]`.

**KairoLibrary #2: `stloc.s => DELETION_POINT` — 3 métodos, 5215 bytes.** Exemplo: `kairo.unity.util.Compressor::Expand(System.Byte[])->System.Byte[]`.

Tentativas compiladas no grupo na rodada 8: alias-field-command: 2, alias-field-occurrence: 3, assignment-ternary-if: 5, assignment-ternary-if-reverse: 4, if-polarity: 1, local-declaration-scope-hoist: 6, switch-owned-break-if-chain: 2, ternary-polarity: 1; 24 com previsão incompleta. Antes de compilar: nenhum; orçamento recusou 12; repertório anterior filtrou 724. Paradas: incomplete_candidate_budget: 2, no_model_approved_candidate: 10.

| IL original | IL recompilado |
| --- | --- |
| `IL_0049: ldloc.s 4` | `IL_0049: ldloc.s 4` |
| `IL_004b: ldfld class kairo.unity.util.Compressor/Node::left_:class kairo.unity.util.Compressor/Node` | `IL_004b: ldfld class kairo.unity.util.Compressor/Node::left_:class kairo.unity.util.Compressor/Node` |
| `IL_0050: stloc.s 4` | `IL_0050: br.s L5` |
| `IL_0052: br.s L6` | `IL_0052: ldloc.s 4` |
| `IL_0054: ldloc.s 4` | `IL_0054: ldfld class kairo.unity.util.Compressor/Node::right_:class kairo.unity.util.Compressor/Node` |

EH original: `[{"kind": "Catch", "try_start": 173, "try_end": 181, "handler_start": 181, "handler_end": 184, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"}, {"kind": "Finally", "try_start": 53, "try_end": 170, "handler_start": 170, "handler_end": 187, "filter_start": -1, "catch_type": null}]`; recompilado: `[{"kind": "Catch", "try_start": 171, "try_end": 179, "handler_start": 179, "handler_end": 182, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"}, {"kind": "Finally", "try_start": 53, "try_end": 168, "handler_start": 168, "handler_end": 185, "filter_start": -1, "catch_type": null}]`.

**KairoLibrary #3: `brfalse.s => brtrue.s` — 5 métodos, 4079 bytes.** Exemplo: `kairo.unity.util.StreamUtil::WriteArray(java.io.OutputStream,System.Array,System.Int32)->System.Void`.

Tentativas compiladas no grupo na rodada 8: nullable-conditional-coalesce-explicit: 2; 2 com previsão incompleta. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 147. Paradas: no_model_approved_candidate: 19, no_transformation_advances: 2.

| IL original | IL recompilado |
| --- | --- |
| `IL_0000: ldarg.1` | `IL_0000: ldarg.1` |
| `IL_0001: brfalse.s O12` | `IL_0001: brtrue.s L0` |
| `IL_0003: ldarg.1` | `IL_0003: ldc.i4.m1` |
| `IL_0004: ldc.i4.0` | `IL_0004: br.s L1` |
|  | `IL_0006: ldarg.1` |
|  | `IL_0007: ldc.i4.0` |

EH original: `[{"kind": "Finally", "try_start": 196, "try_end": 233, "handler_start": 233, "handler_end": 253, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 273, "try_end": 309, "handler_start": 309, "handler_end": 329, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 349, "try_end": 385, "handler_start": 385, "handler_end": 405, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 425, "try_end": 461, "handler_start": 461, "handler_end": 481, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 501, "try_end": 537, "handler_start": 537, "handler_end": 557, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 577, "try_end": 613, "handler_start": 613, "handler_end": 633, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 653, "try_end": 689, "handler_start": 689, "handler_end": 709, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 729, "try_end": 765, "handler_start": 765, "handler_end": 785, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 805, "try_end": 841, "handler_start": 841, "handler_end": 861, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 881, "try_end": 917, "handler_start": 917, "handler_end": 937, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 957, "try_end": 993, "handler_start": 993, "handler_end": 1013, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1033, "try_end": 1069, "handler_start": 1069, "handler_end": 1089, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1109, "try_end": 1145, "handler_start": 1145, "handler_end": 1165, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1185, "try_end": 1226, "handler_start": 1226, "handler_end": 1246, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1266, "try_end": 1307, "handler_start": 1307, "handler_end": 1327, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1347, "try_end": 1388, "handler_start": 1388, "handler_end": 1408, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1428, "try_end": 1469, "handler_start": 1469, "handler_end": 1489, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1509, "try_end": 1545, "handler_start": 1545, "handler_end": 1565, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1595, "try_end": 1643, "handler_start": 1643, "handler_end": 1663, "filter_start": -1, "catch_type": null}]`; recompilado: `[{"kind": "Finally", "try_start": 200, "try_end": 234, "handler_start": 234, "handler_end": 254, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 275, "try_end": 308, "handler_start": 308, "handler_end": 328, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 349, "try_end": 382, "handler_start": 382, "handler_end": 402, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 423, "try_end": 456, "handler_start": 456, "handler_end": 476, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 497, "try_end": 530, "handler_start": 530, "handler_end": 550, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 571, "try_end": 604, "handler_start": 604, "handler_end": 624, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 645, "try_end": 678, "handler_start": 678, "handler_end": 698, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 719, "try_end": 752, "handler_start": 752, "handler_end": 772, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 793, "try_end": 826, "handler_start": 826, "handler_end": 846, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 867, "try_end": 900, "handler_start": 900, "handler_end": 920, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 941, "try_end": 974, "handler_start": 974, "handler_end": 994, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1015, "try_end": 1048, "handler_start": 1048, "handler_end": 1068, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1089, "try_end": 1122, "handler_start": 1122, "handler_end": 1142, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1163, "try_end": 1201, "handler_start": 1201, "handler_end": 1221, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1242, "try_end": 1280, "handler_start": 1280, "handler_end": 1300, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1321, "try_end": 1359, "handler_start": 1359, "handler_end": 1379, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1400, "try_end": 1438, "handler_start": 1438, "handler_end": 1458, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1479, "try_end": 1512, "handler_start": 1512, "handler_end": 1532, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1563, "try_end": 1611, "handler_start": 1611, "handler_end": 1631, "filter_start": -1, "catch_type": null}]`.

**KairoLibrary #4: `brfalse => brtrue.s` — 2 métodos, 4071 bytes.** Exemplo: `kairo.unity.ui.Balloon::Draw(kairo.unity.ui.Graphics,System.Single,System.Single,UnityEngine.Matrix4x4)->System.Void`.

Tentativas compiladas no grupo na rodada 8: nenhum. Histórico 7 compatível: UNKNOWN; não se presume compatibilidade após alteração do fonte. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 144. Paradas: no_model_approved_candidate: 8.

| IL original | IL recompilado |
| --- | --- |
| `IL_0021: ldarg.1` | `IL_0021: ldarg.1` |
| `IL_0022: ldfld class kairo.unity.ui.Graphics::render_:class kairo.unity.ui.Image` | `IL_0022: ldfld class kairo.unity.ui.Graphics::render_:class kairo.unity.ui.Image` |
| `IL_0027: brfalse L28` | `IL_0027: brtrue.s L0` |
| `IL_002c: ldc.i4.s 60` | `IL_0029: ret` |
| `IL_002e: stloc.0` | `IL_002a: ldc.i4.s 60` |
|  | `IL_002c: stloc.s 24` |

EH original: `[{"kind": "Finally", "try_start": 1096, "try_end": 1874, "handler_start": 1874, "handler_end": 1888, "filter_start": -1, "catch_type": null}]`; recompilado: `[{"kind": "Finally", "try_start": 1091, "try_end": 1865, "handler_start": 1865, "handler_end": 1879, "filter_start": -1, "catch_type": null}]`.

**KairoLibrary #5: `ldc.i4.0 => brtrue.s` — 1 métodos, 3513 bytes.** Exemplo: `kairo.unity.ui.Graphics::_drawBitmap(kairo.unity.ui.Image,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single)->System.Void`.

Tentativas compiladas no grupo na rodada 8: nenhum. Histórico 7 compatível: UNKNOWN; não se presume compatibilidade após alteração do fonte. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 144. Paradas: no_model_approved_candidate: 4.

| IL original | IL recompilado |
| --- | --- |
| `IL_0752: ldarg.0` | `IL_0755: ldarg.0` |
| `IL_0753: ldfld class kairo.unity.ui.Graphics::setFigureClip_:valuetype System.Boolean` | `IL_0756: ldfld class kairo.unity.ui.Graphics::setFigureClip_:valuetype System.Boolean` |
| `IL_0758: ldc.i4.0` | `IL_075b: brtrue.s L36` |
| `IL_0759: ceq` | `IL_075d: ldarg.0` |
| `IL_075b: br.s O1886` | ```IL_075e: call class kairo.unity.ui.Graphics::UpdateGUIGroup``0()->class System.Void;this=True;explicit=False;cc=Default``` |
| `IL_075d: ldc.i4.0` |  |
| `IL_075e: brfalse.s L36` |  |
| `IL_0760: ldarg.0` |  |
| ```IL_0761: call class kairo.unity.ui.Graphics::UpdateGUIGroup``0()->class System.Void;this=True;explicit=False;cc=Default``` |  |

EH original: `[{"kind": "Finally", "try_start": 2796, "try_end": 3028, "handler_start": 3028, "handler_end": 3050, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 3134, "try_end": 3393, "handler_start": 3393, "handler_end": 3399, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 763, "try_end": 3407, "handler_start": 3407, "handler_end": 3428, "filter_start": -1, "catch_type": null}]`; recompilado: `[{"kind": "Finally", "try_start": 2791, "try_end": 3023, "handler_start": 3023, "handler_end": 3045, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 3129, "try_end": 3393, "handler_start": 3393, "handler_end": 3399, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 763, "try_end": 3407, "handler_start": 3407, "handler_end": 3428, "filter_start": -1, "catch_type": null}]`.

| Assembly-CSharp — par | Métodos | Peso IL |
| --- | --- | --- |
| `brfalse => brtrue` | 11 | 31885 |
| `br => ret` | 9 | 21529 |
| `bne.un => beq` | 14 | 19612 |
| `stloc.s => stloc.s` | 11 | 16559 |
| `INSERTION_POINT => stloc.1` | 2 | 12768 |
| `ret => br` | 7 | 12447 |
| `br => br` | 8 | 11241 |
| `stloc.1 => stloc.s` | 2 | 10828 |
| `brtrue => stloc.s` | 4 | 8527 |
| `ldarg.0 => dup` | 3 | 8467 |
| `INSERTION_POINT => stloc.2` | 1 | 7669 |
| `INSERTION_POINT => br` | 3 | 6720 |
| `brtrue => ldc.i4.0` | 2 | 6470 |
| `brtrue => brfalse` | 6 | 6320 |
| `stloc.s => DELETION_POINT` | 7 | 6161 |
| `stloc.s => dup` | 2 | 5401 |
| `bge => blt` | 3 | 4926 |
| `ble => bgt` | 4 | 4616 |
| `stloc.2 => stloc.0` | 2 | 3787 |
| `brfalse => stloc.s` | 1 | 3297 |
| `stloc.0 => dup` | 2 | 3176 |
| `stloc.3 => stloc.1` | 2 | 2955 |
| `INSERTION_POINT => stloc.s` | 2 | 2565 |
| `beq => beq` | 1 | 2349 |
| `brtrue => brtrue` | 1 | 2145 |
| `stloc.s => stloc.2` | 2 | 1874 |
| `stloc.s => stloc.0` | 2 | 1800 |
| `dup => DELETION_POINT` | 2 | 1715 |
| `ldc.i4.5 => DELETION_POINT` | 1 | 1599 |
| `br => DELETION_POINT` | 7 | 1454 |
| `brfalse => DELETION_POINT` | 1 | 1342 |
| `bne.un => bne.un` | 1 | 1308 |
| `stloc.2 => stloc.s` | 3 | 1263 |
| `stloc.s => stloc.1` | 1 | 1029 |
| `INSERTION_POINT => ldc.r4` | 1 | 857 |
| `stloc.3 => dup` | 1 | 758 |
| `brtrue => DELETION_POINT` | 1 | 723 |
| `switch => switch` | 1 | 717 |
| `INSERTION_POINT => ldc.i4.s` | 1 | 707 |
| `INSERTION_POINT => ldloc.2` | 1 | 691 |
| `INSERTION_POINT => ldloc.s` | 1 | 653 |
| `ldc.i4.0 => stloc.s` | 1 | 623 |
| `leave => leave` | 1 | 600 |
| `blt.un => bge` | 1 | 594 |
| `bge.un => blt` | 1 | 562 |
| `ldc.i4.1 => switch` | 2 | 556 |
| `ldarg.s => DELETION_POINT` | 1 | 506 |
| `brtrue => stloc.1` | 1 | 447 |
| `ldarg.1 => ldloc.s` | 1 | 432 |
| `ldarg.2 => DELETION_POINT` | 1 | 343 |
| `INSERTION_POINT => stloc.3` | 1 | 325 |
| `switch => ldc.i4.1` | 1 | 322 |
| `brfalse => brfalse` | 2 | 297 |
| `ldc.i4.0 => DELETION_POINT` | 1 | 286 |
| `ldc.i4.1 => ldc.i4.4` | 1 | 272 |
| `blt => blt` | 1 | 253 |
| `INSERTION_POINT => ldarg.0` | 1 | 241 |
| `INSERTION_POINT => leave` | 1 | 222 |
| `INSERTION_POINT => stloc.0` | 1 | 196 |
| `beq => bne.un` | 1 | 190 |
| `ldloc.3 => dup` | 1 | 134 |
| `stloc.3 => DELETION_POINT` | 1 | 132 |
| `ldarg.3 => DELETION_POINT` | 1 | 110 |
| `stloc.0 => DELETION_POINT` | 2 | 109 |
| `INSERTION_POINT => ldloc.1` | 1 | 106 |
| `IL_ALIGNED_IDENTICAL => IL_ALIGNED_IDENTICAL` | 1 | 104 |
| `stloc.0 => stloc.s` | 1 | 99 |
| `brtrue => stloc.0` | 1 | 38 |

**Assembly-CSharp #1: `brfalse => brtrue` — 11 métodos, 31885 bytes.** Exemplo: `form.GameForm::_update(System.Int32)->System.Boolean`.

Tentativas compiladas no grupo na rodada 8: nenhum. Histórico 7 compatível (mesmo original e hash do arquivo fonte atual): if-polarity: 2, local-declaration-scope-hoist: 2, smcs-continue-positive-tail: 1, smcs-empty-arm-restore: 3, solver-group-all-calculated-blocks: 1, solver-group-block-declarations: 3. Recibos: round7-cycle06-Assembly-CSharp-engine.json, round7-cycle17-Assembly-CSharp-engine.json, round7-part-a-Assembly-CSharp-engine.json. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 788. Paradas: no_model_approved_candidate: 44.

| IL original | IL recompilado |
| --- | --- |
| `IL_00d8: ldfld class data.BaseData::id_:valuetype System.Int32` | `IL_00d8: ldfld class data.BaseData::id_:valuetype System.Int32` |
| ```IL_00dd: callvirt class main.AppData::CheckAlreadyEvent``0(valuetype System.Int32)->valuetype System.Boolean;this=True;explicit=False;cc=Default``` | ```IL_00dd: callvirt class main.AppData::CheckAlreadyEvent``0(valuetype System.Int32)->valuetype System.Boolean;this=True;explicit=False;cc=Default``` |
| `IL_00e2: brfalse L5` | `IL_00e2: brtrue R265` |
| `IL_00e7: br L7` | `IL_00e7: ldarg.0` |
| `IL_00ec: ldarg.0` | `IL_00e8: ldfld class form.MyFormBase::apdat_:class main.AppData` |
| `IL_00ed: ldfld class form.MyFormBase::apdat_:class main.AppData` |  |

**Assembly-CSharp #2: `br => ret` — 9 métodos, 21529 bytes.** Exemplo: `form.SoccerForm::Draw_newPlayerPopular(kairo.unity.ui.Graphics)->System.Void`.

Tentativas compiladas no grupo na rodada 8: nenhum. Histórico 7 compatível (mesmo original e hash do arquivo fonte atual): smcs-empty-arm-restore: 8, smcs-empty-arm-restore-and-suffix: 2, smcs-empty-arm-restore-first-and: 2. Recibos: round7-cycle06-Assembly-CSharp-engine.json. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 350. Paradas: no_model_approved_candidate: 36.

| IL original | IL recompilado |
| --- | --- |
| `IL_020a: ldelem.i4` | `IL_020a: ldelem.i4` |
| `IL_020b: bge L3` | `IL_020b: bge L3` |
| `IL_0210: br L5` | `IL_0210: ret` |
| `IL_0215: ldarg.0` | `IL_0211: ldarg.0` |
| `IL_0216: ldfld class form.SoccerForm::stateCnt_:valuetype System.Int32` | `IL_0212: ldfld class form.SoccerForm::stateCnt_:valuetype System.Int32` |

**Assembly-CSharp #3: `bne.un => beq` — 14 métodos, 19612 bytes.** Exemplo: `form.SubForm::Init()->System.Boolean`.

Tentativas compiladas no grupo na rodada 8: calculated-selector-continue-composition: 2, smcs-value-return-guard-reverse: 1. Antes de compilar: wrong_signature: 6; orçamento recusou 0; repertório anterior filtrou 2293. Paradas: no_model_approved_candidate: 54, no_transformation_advances: 3.

| IL original | IL recompilado |
| --- | --- |
| `IL_0144: ldfld class form.SubForm::type_:valuetype System.Int32` | `IL_0144: ldfld class form.SubForm::type_:valuetype System.Int32` |
| `IL_0149: ldc.i4.6` | `IL_0149: ldc.i4.6` |
| `IL_014a: bne.un L7` | `IL_014a: beq L97` |
| `IL_014f: br L97` | `IL_014f: ldarg.0` |
| `IL_0154: ldarg.0` | `IL_0150: ldfld class form.SubForm::type_:valuetype System.Int32` |
| `IL_0155: ldfld class form.SubForm::type_:valuetype System.Int32` |  |

**Assembly-CSharp #4: `stloc.s => stloc.s` — 11 métodos, 16559 bytes.** Exemplo: `form.DebugForm::Draw_coatEva(kairo.unity.ui.Graphics)->System.Void`.

Tentativas compiladas no grupo na rodada 8: nenhum. Histórico 7 compatível: UNKNOWN; não se presume compatibilidade após alteração do fonte. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 1058. Paradas: no_model_approved_candidate: 44.

| IL original | IL recompilado |
| --- | --- |
| `IL_019f: mul` | `IL_019f: mul` |
| `IL_01a0: add` | `IL_01a0: add` |
| `IL_01a1: stloc.s 6` | `IL_01a1: stloc.s 12` |
| `IL_01a3: ldloc.s 5` | `IL_01a3: ldloc.s 5` |
| `IL_01a5: ldloc.s 8` | `IL_01a5: ldloc.s 8` |

**Assembly-CSharp #5: `INSERTION_POINT => stloc.1` — 2 métodos, 12768 bytes.** Exemplo: `form.SubForm::Update2()->System.Boolean`.

Tentativas compiladas no grupo na rodada 8: nenhum. Histórico 7 compatível: UNKNOWN; não se presume compatibilidade após alteração do fonte. Antes de compilar: wrong_signature: 26; orçamento recusou 0; repertório anterior filtrou 2145. Paradas: no_model_approved_candidate: 8.

| IL original | IL recompilado |
| --- | --- |
| `IL_001f: ldc.i4 131072` | `IL_001f: ldc.i4 131072` |
| ```IL_0024: callvirt class kairo.unity.ui.Canvas::CheckKeyRepeat``0(valuetype System.Int32)->valuetype System.Boolean;this=True;explicit=False;cc=Default``` | ```IL_0024: callvirt class kairo.unity.ui.Canvas::CheckKeyRepeat``0(valuetype System.Int32)->valuetype System.Boolean;this=True;explicit=False;cc=Default``` |
| `IL_0029: brfalse L0` | `IL_0029: stloc.1` |
| `IL_002e: ldarg.0` | `IL_002a: ldloc.1` |
| `IL_002f: ldarg.0` | `IL_002b: brfalse L0` |
|  | `IL_0030: ldarg.0` |

EH original: `[{"kind": "Catch", "try_start": 10749, "try_end": 10831, "handler_start": 10831, "handler_end": 10897, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Finally", "try_start": 10647, "try_end": 10703, "handler_start": 10703, "handler_end": 10715, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 10561, "try_end": 10618, "handler_start": 10618, "handler_end": 10630, "filter_start": -1, "catch_type": null}]`; recompilado: `[{"kind": "Catch", "try_start": 10704, "try_end": 10786, "handler_start": 10786, "handler_end": 10852, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Finally", "try_start": 10607, "try_end": 10663, "handler_start": 10663, "handler_end": 10675, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 10521, "try_end": 10578, "handler_start": 10578, "handler_end": 10590, "filter_start": -1, "catch_type": null}]`.

## 5. TOOLCHAIN

KairoLibrary: NuGet Microsoft.Net.Compilers 1.1.0; csc.exe 1.1.0.51109; compilador SHA-256 `53b693536e8a86f35d05d2165ff9b3e0aeaa499ecfd8135a9f99ab4404ca86c3`. Mudou compilador/opções/referências nesta rodada: NÃO.

Opções: `/nologo /noconfig /nostdlib+ /target:library /platform:anycpu /optimize+ /debug:pdbonly /deterministic- /langversion:6 /unsafe- /checked- /codepage:65001 /utf8output`.

| Referência (ordem de compilação) | SHA-256 |
| --- | --- |
| C:\Windows\Microsoft.NET\Framework\v2.0.50727\mscorlib.dll | `c4729c6fc2279933fa9638f60775ab2f7eb731d5b04f9e04c15187afbdf94558` |
| C:\Users\l_abn\AppData\Local\Programs\Unity\5.3.4f1\Editor\Data\Mono\lib\mono\2.0\System.Core.dll | `4998eda31c3d7bebf606f98cef77ad697f9e82e629efc37c06517019c2c2c271` |
| C:\Users\l_abn\Documents\Projetos\Pocket League Story\original\extracted\assets\bin\Data\Managed\UnityEngine.dll | `7cc901371cc77e103d00b5845e8285f85fbdf36bb47ccf3d824f94c4ead32421` |
| C:\Users\l_abn\AppData\Local\Programs\Unity\5.3.4f1\Editor\Data\Mono\lib\mono\2.0\System.dll | `c2225d7ba58a12e22c266384d3c1e4e882ab4e42449b91b721c21b5f1fcd63be` |

Assembly-CSharp: smcs.exe 2.6.5.0 do Unity 5.3.4f1; compilador SHA-256 `26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0`. Mudou compilador/opções/referências nesta rodada: NÃO.

Opções: `-nologo -noconfig -target:library -platform:anycpu -optimize+ -debug- -langversion:Default -unsafe- -checked- -codepage:65001`.

| Referência (ordem de compilação) | SHA-256 |
| --- | --- |
| C:\Users\l_abn\Documents\Projetos\Pocket League Story\original\extracted\assets\bin\Data\Managed\mscorlib.dll | `7f16171cf5f87ca1e4c229727526d2fbc786079fc0c960f36197013ea0d986e1` |
| C:\Users\l_abn\Documents\Projetos\Pocket League Story\original\extracted\assets\bin\Data\Managed\KairoLibrary.dll | `bb553d341eecd9b900f772486bf4a5c289aa2f09566d9d402a3eb70268f4b3ee` |
| C:\Users\l_abn\Documents\Projetos\Pocket League Story\original\extracted\assets\bin\Data\Managed\UnityEngine.dll | `7cc901371cc77e103d00b5845e8285f85fbdf36bb47ccf3d824f94c4ead32421` |
| C:\Users\l_abn\Documents\Projetos\Pocket League Story\original\extracted\assets\bin\Data\Managed\System.Core.dll | `104278a0bf67b4de8fd1e910e3292a3ef51808ebee7a5f2a229125f18463c98c` |
| C:\Users\l_abn\Documents\Projetos\Pocket League Story\original\extracted\assets\bin\Data\Managed\System.dll | `7e72b92ddcd6b7b791ab24dd5756544f2da81f65e7ba7d1ebe8223123720d0ac` |

Runtime Mono: `C:\Users\l_abn\AppData\Local\Programs\Unity\5.3.4f1\Editor\Data\Mono\bin\mono.exe`; SHA-256 `3d102d9a63dae72e48396998bdbae4e9b2d8952ca6a8ebbcba698d3bc4345076`. Stdlib: `default`. Ambiente do subprocesso: `{"MONO_PATH": "C:\\Users\\l_abn\\AppData\\Local\\Programs\\Unity\\5.3.4f1\\Editor\\Data\\Mono\\lib\\mono\\unity", "MONO_CFG_DIR": "C:\\Users\\l_abn\\AppData\\Local\\Programs\\Unity\\5.3.4f1\\Editor\\Data\\Mono\\etc"}`.

Compilador histórico exato, comando histórico integral e hashes das referências originalmente usadas: UNKNOWN. Os candidatos acima têm hashes fixados e prova de corpos; isso não identifica sua fabricação histórica. Roslyn moderno compila auxiliares e fornece AST, não os assemblies alvos. PDB/MDB auxiliam apenas o mapeamento. Nomes escolhidos para locais não são apresentados como nomes originais.

## 6. COMPONENTES EXTERNOS E METADADOS

| Assembly | Tipos com metadados iguais | Declarações sem corpo aprovadas | Ordem de tipos | Metadados globais | Bytes brutos |
| --- | --- | --- | --- | --- | --- |
| KairoLibrary | 337/337 | 76/76 | True | True | NOT_EVALUATED |
| Assembly-CSharp | 129/129 | 6/6 | True | True | NOT_EVALUATED |

A comparação abrange ordem de tipos e membros, parâmetros, atributos personalizados, layout, constantes, recursos e referências globais; o guard de identidade confere o SHA do original e do build atual. Diferenças brutas não medidas continuam UNKNOWN. Nenhum byte original é inserido no resultado, nenhuma classe gerada é escrita à mão e nenhum assembly é normalizado.

Externos gerenciados com correspondência oficial registrada: 7/7: Mono.Security.dll, System.Core.dll, System.dll, UnityEngine.Networking.dll, UnityEngine.UI.dll, UnityEngine.dll, mscorlib.dll. UnityEngine.dll: BYTE_IDENTICAL_OFFICIAL_ANDROID_MONO; SHA-256 `7cc901371cc77e103d00b5845e8285f85fbdf36bb47ccf3d824f94c4ead32421`. A variante é Android/Mono do Unity 5.3.4f1; evidência histórica, não nova decompilação.

Externos nativos com correspondência oficial registrada: 6/6; lib/armeabi-v7a/libmain.so, lib/armeabi-v7a/libmono.so, lib/armeabi-v7a/libunity.so, lib/x86/libmain.so, lib/x86/libmono.so, lib/x86/libunity.so. DEX/Java permanece fora do escopo.

Campos não determinísticos comprovados por pares de recompilações anteriores (não normalizados): KairoLibrary — Module.Mvid, COFF.TimeDateStamp, IMAGE_DEBUG_DIRECTORY.TimeDateStamp, CodeView.RSDS.PdbGuid; Assembly-CSharp — Module.Mvid, COFF.TimeDateStamp. Determinismo histórico do original: UNKNOWN.

| Assembly | Campo | Prova | Normalização aplicada |
| --- | --- | --- | --- |
| KairoLibrary | Module.Mvid | reports/round3-Kairo-repeatability.json | False |
| KairoLibrary | COFF.TimeDateStamp | reports/round3-Kairo-repeatability.json | False |
| KairoLibrary | IMAGE_DEBUG_DIRECTORY.TimeDateStamp | reports/round3-Kairo-repeatability.json | False |
| KairoLibrary | CodeView.RSDS.PdbGuid | reports/round3-Kairo-repeatability.json | False |
| Assembly-CSharp | Module.Mvid | reports/round3-Assembly-CSharp-repeatability.json | False |
| Assembly-CSharp | COFF.TimeDateStamp | reports/round3-Assembly-CSharp-repeatability.json | False |

## 7. BLOQUEIOS

Nenhuma instalação, arquivo ou decisão do usuário é necessária. A rodada parou pela condição experimental de três ciclos sem ganho; hipóteses e fontes preparadas não foram compiladas depois da parada.

Parada após 7 ciclos; sequência final sem novo matched: 3. Limites configurados: 40 ciclos, 3 sem ganho consecutivos; cota Kairo: at least one KairoLibrary cycle in every consecutive block of five cycles.

## 8. SUA RECOMENDAÇÃO

1. Testar os patches de lifetime/escopo Roslyn já preparados para StringUtil.GetCode e Graphics.DrawArc: dois alvos, 2514 bytes de IL, potencial máximo de dois métodos; ganho efetivo UNKNOWN. GetCode prevê 31 slots completos, DrawArc tem variante de 28 tipos ainda incompleta e pode ser compilada pelo protocolo vigente.
2. Testar a composição de acesso nulo com ternário negativo em StreamUtil.WriteArray: um alvo, 1686 bytes, potencial máximo de um método; os 14 bytes iniciais foram reproduzidos em fixture histórica, corpo completo do jogo UNKNOWN. A regra não está integrada e só um dos cinco membros do grupo é elegível.
3. Aplicar os três patches calculados de coalescência de coordenadas nos métodos Draw_coatEva, SetSavePos e Draw_moveArea: três alvos, 4316 bytes, potencial máximo de três métodos; previsões completas/exatas e fixtures aprovadas, ganhos do jogo UNKNOWN. Para SpForm.Update, o ponto de retomada medido é o operando do br em IL_04fe, primeira diferença no byte1279; sua assinatura de24 locais já está correta.
