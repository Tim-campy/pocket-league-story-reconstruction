## 1. PROGRESSO

Pocket League Story para Android, APK net.kairosoft.android.soccer_en_2.0.2-APK_Award.apk; reconstrução C#/Unity 5.3.4f1. Comparação com o início desta rodada, commit `aecb02b8a7b0965508e223335e44ae36a95a0939`. Matched exige corpo completo (cabeçalho, maxstack, locais, IL e EH); referências de tokens são resolvidas por identidade. Declarações sem corpo contam em métodos, sem bytes de IL.

| Assembly | Anterior (aecb02b) | Atual | Δ métodos | Δ bytes de IL | Regressões |
| --- | --- | --- | --- | --- | --- |
| KairoLibrary | 3241/3287 (98.600548%); IL 249623/269894 bytes (92.489274%) | 3261/3287 (99.209005%); IL 263611/269894 bytes (97.672049%) | 20 | 13988 | 0 |
| Assembly-CSharp | 1844/1958 (94.177732%); IL 447600/601603 bytes (74.401225%) | 1892/1958 (96.629213%); IL 549322/601603 bytes (91.309718%) | 48 | 101722 | 0 |

Total: 5153/5245 métodos; 812933/871497 bytes de IL. Retiradas de matched por ID: 0; corpos antes iguais que deixaram de bater: 0.

Motivo da parada: Limite de 35 ciclos autônomos concluídos.

## 2. O QUE FOI FEITO NESTA RODADA

**Parte A.** Functional scenario completed on physical Android device; TEST_ONLY APK, not byte-identical reconstruction progress. Evidência: `reports/round10-functional-test.json`.

| Teste funcional | Resultado |
| --- | --- |
| opens | True |
| reaches_menu | True |
| match_runs | True |
| match_completes | True |

O teste usou as DLLs congeladas do início `aecb02b8a7b0965508e223335e44ae36a95a0939`, em Samsung Galaxy S22 Ultra SM-S908E, Android 16/API 36, ABI armeabi-v7a. Partida completa: Sun Rays 1 × 0 Blue Stars, com primeiro tempo, intervalo, segundo tempo e resultado. Não cobre automaticamente as alterações posteriores das partes C/D.

APK de teste SHA-256 `8a832f276fa7b29c4e0d35121b563098543206c3119aff4a3f21325896b5f333`; chave de teste nova, assinatura v1 verificada. Ordem das 1218 entradas, métodos de compressão e restos dos offsets módulo 4096 preservados; 1213 payloads comprimidos inalterados. Mudaram duas DLLs e três entradas de assinatura. Nível original de compressão UNKNOWN nas entradas substituídas. É somente teste; não entra na correspondência byte a byte.

Logcat restrito aos processos do jogo: 37 capturas, 677 linhas distintas; 0 exceções Mono/Unity observadas nos buffers capturados. Primeira instalação recusada com INSTALL_FAILED_VERIFICATION_FAILURE; nova tentativa solicitada pelo usuário terminou em Success. Nenhuma proteção global foi desativada. Capturas, APK e chave privada ficam fora do Git.

**Parte B.** Measurement only: 23 equal table row counts; 97 persistent CodeView path bytes; no raw corrections or DLL normalization. Evidência: `reports/round10-KairoLibrary-raw-measurement.json`.

Medição da baseline com 44 corpos KairoLibrary divergentes: 311216 posições diferentes em 569856 bytes por arquivo. Regiões se sobrepõem; não somar camadas.

| Região | Bytes originais | Bytes reconstruídos | Diferentes/sem par |
| --- | --- | --- | --- |
| PE_HEADERS | 512 | 512 | 9 |
| .text | 567808 | 567808 | 311206 |
| .rsrc | 1024 | 1024 | 0 |
| .reloc | 512 | 512 | 1 |
| METADATA_ROOT | 258256 | 258288 | 34672 |
| #~ | 106324 | 106324 | 13159 |
| #Strings | 59692 | 59692 | 0 |
| #US | 68712 | 68712 | 191 |
| #GUID | 16 | 16 | 16 |
| #Blob | 23404 | 23436 | 21305 |

As 23 tabelas têm as mesmas contagens de linhas; 13 contêm bytes diferentes. AssemblyRef difere somente nos índices PublicKeyOrToken de #Blob das linhas 1, 2 e 4: 11271 → 11302; versões e token b77a5c561934e089 coincidem. Os 116 payloads FieldRVA, 16491 bytes, são idênticos.

Persistência independente dos corpos, PROVADA com caminhos/opções atuais: 97 bytes do caminho CodeView/PDB. Foram excluídos analiticamente apenas MVID, timestamp COFF, timestamp do diretório de debug e GUID RSDS. Não houve normalização, nova exceção ou correção de bytes. Persistência futura dos demais índices, RVAs e dos 32 bytes extras de #Blob: UNKNOWN.

**Parte C.** Extrator ampliado nas quatro etapas e validado nos métodos matched; 114 pendentes auditados, uma reescrita de 717 bytes adotada oficialmente, zero perdas. Árvores ambíguas continuam incompletas. Evidência: `reports/round10-control-results.json`.

Etapas de ampliação: 1 — switch, incluindo string; 2 — try/catch/finally; 3 — lógica de condições; 4 — dono de salto ambíguo.

| Aplicação do esqueleto smcs | Resultado |
| --- | --- |
| Pendentes auditados, incluindo um corpo igual retido | 114 |
| Pares de árvores completos antes | 19 |
| Árvores iguais / diferentes antes | 11 / 8 |
| Métodos reescritos com plano/árvore candidatos iguais ao original | 1 |
| Corpos aprovados pelo verificador / bytes de IL | 1 / 717 |
| Retiradas de matched | 0 |

Alvo aprovado: `surface.GameView::OnTouchEvent(kairo.unity.surface.TouchEvent)->System.Void`. Os demais bloqueios do cálculo de fonte: control-and-predicted-slots-already-equal; value-code-difference-remains-UNKNOWN: 4, no-approved-control-preserving-slot-calculus-proposal: 7, nonbijective-or-unproved-existing-value-leaves: 7, original-or-source-IR-incomplete: 90, source-method-unmapped: 5. Não foram criadas folhas ou instruções a partir de bytes originais.

No corpus fixo de 1838 corpos matched: 1555 planos completos/1555 iguais; 1305 árvores pareadas/1305 iguais. Cobertura das árvores: 71.001088%. Switch string/sparse e regiões com owner indistinguível mantêm os limites de completude documentados.

Resultados negativos preservados: a primeira ampliação EH teve 14 planos completos incorretos e foi reprovada; a lógica composta revelou uma renormalização de laço, corrigida antes do aceite. A validação final de cada etapa não tem planos/árvores incorretos nos matched. O renderizador manteve árvore/plano em 11/12 controles NoEmit; ReturnVoidFinally: protected void footer differs; independent AST gate refuses proposal.

| Etapa | Planos completos matched | Árvores pareadas matched | Árvores pareadas pendentes | Iguais nos pendentes |
| --- | --- | --- | --- | --- |
| 1 | 1487/1838 | 1267/1838 | 18/114 | 11 |
| 2 | 1555/1838 | 1292/1838 | 19/114 | 11 |
| 3 | 1555/1838 | 1305/1838 | 19/114 | 11 |
| 4 | 1555/1838 | 1305/1838 | 19/114 | 11 |

Ganhos oficiais vêm do diário após adoção e verificação completa, incluindo auxiliares liberados pelo verificador. Sondas e avanços parciais são registrados separadamente; não se somam aos ganhos oficiais. Transformações herdadas da rodada 7 só aparecem abaixo quando foram efetivamente tentadas nesta rodada.

| Ciclo | Assembly / par | Hipótese | Resultado | Matched ganhos | Bytes ganhos | Matched perdidos |
| --- | --- | --- | --- | --- | --- | --- |
| c01 | Assembly-CSharp / switch targets => switch targets | Reemitir controle completo com folhas existentes e slots calculados remove o case25 vazio ausente no dispatch original | Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 717 | 0 |
| cycle01 | Assembly-CSharp / br => ret | Ramo mode2 com saida comum e cadeia direta 0/1/3 remove cache do switch e preserva cinco slots | Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 412 | 0 |
| cycle02 | Assembly-CSharp / brtrue => brfalse | Inverter o ternario do campo estatico com vinculo AST e stsfld preserva o cctor implicito e reproduz a polaridade original | Novos corpos aprovados; 3 tentativas da regra, 3 avancos | 1 | 1816 | 0 |
| cycle03 | Assembly-CSharp / br => ret | Compor nove ajustes de controle e escopos preserva slots43-49 e91-92, calcula cadeia67-69 e remove o cache de switch93 | Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 12484 | 0 |
| cycle04 | Assembly-CSharp / brfalse => brtrue | Guarda positiva no construtor com escopo e atribuicao composta originais | Novos corpos aprovados; 11 tentativas da regra, 1 avancos | 1 | 347 | 0 |
| cycle05 | KairoLibrary / brfalse.s => brtrue.s | Separar a guarda invalidTouch e manter corpo positivo com escopos e stores originais em Canvas.OnInputUpdate | Novos corpos aprovados; 13 tentativas da regra, 3 avancos | 2 | 600 | 0 |
| cycle06 | Assembly-CSharp / bne.un => beq | Restaurar por composicao os ramos vazios terminais, OR separado e cauda positiva nos seletores Int32 | Novos corpos aprovados; 6 tentativas da regra, 6 avancos | 6 | 3458 | 0 |
| cycle07 | KairoLibrary / brtrue.s => ldc.i4.4 | Separar licenca zero e restaurar switch denso com bucket default nas entradas um a tres | Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 1054 | 0 |
| cycle08 | Assembly-CSharp / brtrue => ldc.i4.0 | Restaurar os cinco inicializadores Boolean como ternarios orientados pelo IL, inclusive DEBUG com dois ramos false | Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 341 | 0 |
| cycle09 | Assembly-CSharp / brtrue => ldc.i4.0 | Eliminar alias Boolean sintetico e restaurar guardas continue e ramos vazios originais como composicao por metodo | Novos corpos aprovados; 2 tentativas da regra, 2 avancos | 2 | 6470 | 0 |
| cycle10 | Assembly-CSharp / stloc.s => stloc.s | Calcular locais sem uso e reposicionar declaracoes e escopos para obter os slots originais, mantendo expressoes float | Novos corpos aprovados; 3 tentativas da regra, 3 avancos | 3 | 2417 | 0 |
| cycle11 | Assembly-CSharp / stloc.s => stloc.s; stloc.1 => stloc.s | Declarar seletor antes dos corpos, substituir dispatches por ifs e compor alias e cadeia de dez zeros, preservando saida protegida | Novos corpos aprovados; 2 tentativas da regra, 2 avancos | 2 | 4892 | 0 |
| cycle12 | KairoLibrary / INSERTION_POINT => ldc.i4.4 | Restaurar avaliacao do indexador antes de newarr e dup com stloc dentro do setter, mais a guarda interna list3 positiva no mesmo escopo | Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 1921 | 0 |
| cycle13 | Assembly-CSharp / stloc.1 => stloc.s | Compor seis dispatches, guardas positivas20 e30, duas Single sem uso, neg, continues, stores por braco e saida protegida conforme90slots | Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 9408 | 0 |
| cycle14 | Assembly-CSharp / brfalse => brtrue | Compor layout107, oito dispatches, Join17, unused65, cinco stores por braco, dialog99 e seis controles restantes sem mudar aritmetica | Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 11390 | 0 |
| cycle15 | Assembly-CSharp / brtrue => stloc.s | Restaurar seletores nomeados e joins locais dos quatro dispatches, compondo aliases GetMatchInfoJoin e guardas e bracos vazios especificos | Novos corpos aprovados; 4 tentativas da regra, 4 avancos | 4 | 8527 | 0 |
| cycle16 | KairoLibrary / stloc.2 => stloc.s | Calcular escopos do try e cauda, ordem de locais, dois dispatches, shortarray materializado antes dos stores e expansao de atribuicoes em array | Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 2403 | 0 |
| cycle17 | Assembly-CSharp / INSERTION_POINT => stloc.1 | Compor layout103, onze caudas positivas, bracos vazios48/2 e guarda event189 com Join101 nomeado; fixtures e NoEmit exatos, corpo aguarda oraculo. | Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 12082 | 0 |
| cycle18 | Assembly-CSharp / INSERTION_POINT => stloc.2 | Compor basex compartilhado, cadeia de quatro zeros, Boolean44 e atribuicoes por braco para strings;46 locais NoEmit exatos. | Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 7669 | 0 |
| cycle19 | KairoLibrary / stloc.s => DELETION_POINT | Restaurar stores por braco no dispatch da lambda LoadStart sem escrever classe gerada, preservando as tres regioes catch e o layout original. | Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 1928 | 0 |
| cycle20 | Assembly-CSharp / br => br | Restaurar joins e donos lexicais de cinco controles compostos, preservando locais, folhas e transferencias comprovadas pelas fixtures. | Novos corpos aprovados; 5 tentativas da regra, 5 avancos | 5 | 5339 | 0 |
| cycle21 | Assembly-CSharp / stloc.s => DELETION_POINT | Compor stores locais por braco e restaurar apenas o cache externo original de AddTouch; preservar nomes, expressoes float e negativo historico. | Novos corpos aprovados; 5 tentativas da regra, 5 avancos | 5 | 4627 | 0 |
| cycle22 | Assembly-CSharp / brfalse => brtrue | Testar composicao congelada com IDs originais, fixtures proprias e gate fresco; sem ganho presumido. | Novos corpos aprovados; 3 tentativas da regra, 3 avancos | 3 | 1592 | 0 |
| cycle23 | KairoLibrary / stloc.s => stloc.2 | Restaurar Nodes nomeados e resultado apos finally, reusar num de readBit e declarar tres indices Matrix no pai de uma vez. | Novos corpos aprovados; 3 tentativas da regra, 3 avancos | 3 | 1024 | 0 |
| cycle24 | KairoLibrary / stloc.2 => stloc.1 | Restaurar dono lexical dos locais da lambda HttpObject, escrevendo apenas o fonte pai e mantendo todas as folhas. | Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 944 | 0 |
| cycle25 | KairoLibrary / INSERTION_POINT => stloc.s | Restaurar local do corpo e valores em pilha de MakeLangPackTemplate, preservando referencias e ordem de folhas. | Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 893 | 0 |
| cycle26 | KairoLibrary / stloc.0 => stloc.2 | Restaurar escopo tardio da cauda e alias em pilha nas duas comparacoes, mantendo os locais nomeados originais. | Novos corpos aprovados; 2 tentativas da regra, 2 avancos | 2 | 770 | 0 |
| cycle27 | KairoLibrary / INSERTION_POINT => brtrue.s | Recuperar Boolean compartilhado do cache de localizacao entre os bracos de lock e unlock, sem promover modelos incompletos. | Novos corpos aprovados; 2 tentativas da regra, 2 avancos | 2 | 737 | 0 |
| cycle28 | KairoLibrary / ldarg.0 => DELETION_POINT | Restaurar primeira guarda com break no corpo dos lacos, preservando proprietario do incremento e escopos. | Novos corpos aprovados; 3 tentativas da regra, 3 avancos | 3 | 614 | 0 |
| cycle29 | KairoLibrary / stloc.3 => stloc.s | Restaurar escopo filho da cauda MaterialManager para a alocacao original de locais e liberacao. | Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 416 | 0 |
| cycle30 | Assembly-CSharp / bne.un => beq | Separar continue misto de cauda positiva nos dois Player e restaurar switch de um case no Touch, com igualdade completa em fixtures e slots5/15/3. | Novos corpos aprovados; 3 tentativas da regra, 3 avancos | 3 | 1148 | 0 |
| cycle31 | Assembly-CSharp / bne.un=>beq | Restore four owned continue guards, direct num3 dispatches and per-arm angle stores; remove two switch caches without changing floating comparisons | Avanco parcial; nenhum novo matched; 1 tentativas da regra, 1 avancos | 0 | 0 | 0 |
| cycle32 | Assembly-CSharp / br=>DELETION_POINT | Restore explicit empty outer dispatch default, preserving the accepted20locals and all floating comparisons | Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 1970 | 0 |
| cycle33 | Assembly-CSharp / ble=>bgt | Compose owned loop guards and positive payloads, restore nested ranking break, and name the final stream Boolean in its original slot | Novos corpos aprovados; 3 tentativas da regra, 3 avancos | 3 | 2307 | 0 |
| cycle34 | KairoLibrary / stloc.s=>stloc.s;stloc.3=>stloc.1 | Restore file-completion Boolean across wait and callback; declare image limits in lambda parent block while preserving assignment positions | Novos corpos aprovados; 2 tentativas da regra, 2 avancos | 1 | 684 | 0 |
| cycle35 | Assembly-CSharp / ble=>bgt | Compose state-local interval guards, per-arm English stores, state10 sibling arm, direct card dispatch and missing named frame21 | Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 2309 | 0 |

C01: sonda e passe principal fecharam o mesmo método de 717 bytes; o ganho oficial é 1 método, sem somar a sonda.

O primeiro commit C01 parou na formatação de uma linha vazia com quatro espaços no produtor raw. O hash do produtor foi preservado; revisão restrita de formatação, sem relaxar o verificador. A repetição do commit fez nova verificação completa e publicou f3e3455.

O recibo de adoção C01 escreveu baseline_commit UNKNOWN por limitação do adapter. O snapshot independente identifica ee994d2d052a9b20203791371e3a5556afe815dd; o adapter futuro passou a ler snapshot.json. Nenhum resultado de método foi editado.

Falhas de drivers na preparação (schemas assembly/maxstack e manifesto AST duplicado) foram corrigidas, preservando os rascunhos. Fixtures e candidatos de jogo são contagens distintas.

After cycle04, three additional fixture-tested source families were registered as pending tests. They prevent counting zero-gain cycles until tested; method states remain exclusively verifier-generated. The standalone eight-section report adapter was reviewed for A/B/C evidence, baseline runtime scope, raw PDB-path persistence, exact current measurements, models, and ten inline IL examples.

After cycle06, newly fixture-tested families were added to the prepared queue: d-ready-smcs-unused-slot-scopes, d-ready-smcs-guard-slot-compositions, d-ready-roslyn-loop-body-guards. Adding proposals does not promote any method state.

The three prepared Roslyn lifetime/dispatch compositions have distinct aligned pairs; their queue item was split into three pending diagnostic families, without assuming any target gain.

Cycle07 adopted HttpObject._connect with zero regressions, then its commit stopped at Git whitespace on four blank fixture lines. Failed commit logs were frozen before a trivia-only public-copy repair. The retry reruns full official verification and source/index proof; it does not rerun the cycle or add a journal gain. An earlier publication formatter preflight also rejected an incorrect blank-line assumption before writing; the subsequent literal-protected, token-identical cleanup is recorded separately.

After cycle07, newly fixture-tested families were added to the prepared queue: d-ready-canvas-residual-composition, d-ready-smcs-switch-selector-compositions. Adding proposals does not promote any method state.

After cycle08, newly fixture-tested families were added to the prepared queue: d-ready-smcs-bool-value-compositions. Adding proposals does not promote any method state.

After cycle10, newly fixture-tested families were added to the prepared queue: d-ready-smcs-named-selector-compositions. Adding proposals does not promote any method state.

Selector publication initially traversed a parallel live TPlayer study. Eight extra own-fixture copies were quarantined and removed from engine evidence before any game test; the original53-file frozen index was verified unchanged. See round10-smcs-named-selector-frozen-scope.json. Public fixture whitespace was trimmed with identical C# tokens; private compiled fixtures remain unchanged.

After cycle12, newly fixture-tested families were added to the prepared queue: d-ready-smcs-tplayer-composition. Adding proposals does not promote any method state.

After cycle13, newly fixture-tested families were added to the prepared queue: d-ready-smcs-subform-update-integral, d-ready-roslyn-late-tail-stack-alias. Adding proposals does not promote any method state.

Roslyn770 publication first used an incorrect fixture path and stopped before a bundle or game compilation. The temporary R1 producer copy was preserved privately; the actual R2 directory, producer82edf989 and validation-v1 fixture report were selected and hashed. See round10-roslyn770-publication-recovery.json. No target source or method states changed.

After cycle14, newly fixture-tested families were added to the prepared queue: d-ready-smcs-switch-layout-compositions. Adding proposals does not promote any method state.

During publication of the7347 switch family, the exporter was started before the asynchronous hash binder completed. It stopped at its first read with FileNotFoundError, before public copies or game compilation. After waiting for the62-file binder to finish, export succeeded. Four fixture-tested proposals sharing the measured first pair brtrue=>stloc.s were then grouped with unchanged C# text; the separate original bundles remain pinned.

After cycle15, newly fixture-tested families were added to the prepared queue: d-ready-roslyn-large-seb, d-ready-roslyn-large-resource, d-ready-roslyn-large-http. Adding proposals does not promote any method state.

After cycle16, newly fixture-tested families were added to the prepared queue: d-ready-smcs-subform-update2. Adding proposals does not promote any method state.

After cycle17, newly fixture-tested families were added to the prepared queue: d-ready-smcs-userdata-draw. Adding proposals does not promote any method state.

After cycle19, newly fixture-tested families were added to the prepared queue: d-ready-smcs-branch-joins. Adding proposals does not promote any method state.

After cycle20, newly fixture-tested families were added to the prepared queue: d-ready-smcs-deleted-stores, d-ready-roslyn-small-family, d-ready-roslyn-compressor. Adding proposals does not promote any method state.

Resultados negativos de todas as execuções concluídas, incluindo sondas; só aparecem linhas com alguma falha/recusa. Não são perdas adotadas.

| Execução | Compilações falhas | Regressões recusadas | Metadados recusados | Modelo/emissor | Assinatura emitida recusada | Assinatura prevista recusada |
| --- | --- | --- | --- | --- | --- | --- |
| round10-cycle04-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 | 0 | 11 |
| round10-cycle05-KairoLibrary-engine.json | 0 | 0 | 0 | 0 | 14 | 1 |
| round10-cycle05-probe-KairoLibrary-engine.json | 0 | 0 | 0 | 0 | 10 | 0 |
| round10-cycle11-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 | 0 | 82 |
| round10-cycle11-probe-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 | 0 | 82 |
| round10-cycle13-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 | 0 | 38 |
| round10-cycle13-probe-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 | 0 | 38 |

Variantes incompletas compiladas: 72; invocações dessas variantes: 108. Esses custos incluem sondas; ganhos não são contados duas vezes.

## 3. HIPÓTESES

| Estado | Hipótese | Evidência medida |
| --- | --- | --- |
| CONFIRMADA | c10-control-tree: Reemitir controle completo com folhas existentes e slots calculados remove o case25 vazio ausente no dispatch original | c01: 1 matched, 717 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos |
| CONFIRMADA | d10-exact-setwin-chara-training: Ramo mode2 com saida comum e cadeia direta 0/1/3 remove cache do switch e preserva cinco slots | cycle01: 1 matched, 412 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos |
| CONFIRMADA | static-initializer-ternary-polarity: Inverter o ternario do campo estatico com vinculo AST e stsfld preserva o cctor implicito e reproduz a polaridade original | cycle02: 1 matched, 1816 IL bytes; Novos corpos aprovados; 3 tentativas da regra, 3 avancos |
| CONFIRMADA | d10-subform-draw4-composition: Compor nove ajustes de controle e escopos preserva slots43-49 e91-92, calcula cadeia67-69 e remove o cache de switch93 | cycle03: 1 matched, 12484 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos |
| CONFIRMADA | root-guard-positive-tail: Guarda positiva no construtor com escopo e atribuicao composta originais | cycle04: 1 matched, 347 IL bytes; Novos corpos aprovados; 11 tentativas da regra, 1 avancos |
| CONFIRMADA | root-guard-positive-tail: Separar a guarda invalidTouch e manter corpo positivo com escopos e stores originais em Canvas.OnInputUpdate | cycle05: 2 matched, 600 IL bytes; Novos corpos aprovados; 13 tentativas da regra, 3 avancos |
| CONFIRMADA | d10-integer-empty-arm-composition: Restaurar por composicao os ramos vazios terminais, OR separado e cauda positiva nos seletores Int32 | cycle06: 6 matched, 3458 IL bytes; Novos corpos aprovados; 6 tentativas da regra, 6 avancos |
| CONFIRMADA | d10-roslyn-composition-http-zero-guard-dense-default-bucket: Separar licenca zero e restaurar switch denso com bucket default nas entradas um a tres | cycle07: 1 matched, 1054 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos |
| CONFIRMADA | d10-static-bool-composition: Restaurar os cinco inicializadores Boolean como ternarios orientados pelo IL, inclusive DEBUG com dois ramos false | cycle08: 1 matched, 341 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos |
| CONFIRMADA | d10-smcs-bool-value: Eliminar alias Boolean sintetico e restaurar guardas continue e ramos vazios originais como composicao por metodo | cycle09: 2 matched, 6470 IL bytes; Novos corpos aprovados; 2 tentativas da regra, 2 avancos |
| CONFIRMADA | d10-smcs-unused-slot: Calcular locais sem uso e reposicionar declaracoes e escopos para obter os slots originais, mantendo expressoes float | cycle10: 3 matched, 2417 IL bytes; Novos corpos aprovados; 3 tentativas da regra, 3 avancos |
| CONFIRMADA | d10-smcs-named-selector: Declarar seletor antes dos corpos, substituir dispatches por ifs e compor alias e cadeia de dez zeros, preservando saida protegida | cycle11: 2 matched, 4892 IL bytes; Novos corpos aprovados; 2 tentativas da regra, 2 avancos |
| CONFIRMADA | d10-canvas-two-residual-composition: Restaurar avaliacao do indexador antes de newarr e dup com stloc dentro do setter, mais a guarda interna list3 positiva no mesmo escopo | cycle12: 1 matched, 1921 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos |
| CONFIRMADA | d10-smcs-tplayer: Compor seis dispatches, guardas positivas20 e30, duas Single sem uso, neg, continues, stores por braco e saida protegida conforme90slots | cycle13: 1 matched, 9408 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos |
| CONFIRMADA | d10-smcs-subform-update: Compor layout107, oito dispatches, Join17, unused65, cinco stores por braco, dialog99 e seis controles restantes sem mudar aritmetica | cycle14: 1 matched, 11390 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos |
| CONFIRMADA | d10-smcs-switch-join: Restaurar seletores nomeados e joins locais dos quatro dispatches, compondo aliases GetMatchInfoJoin e guardas e bracos vazios especificos | cycle15: 4 matched, 8527 IL bytes; Novos corpos aprovados; 4 tentativas da regra, 4 avancos |
| CONFIRMADA | d10-large-lambda-composition-seb-root-tail-dispatch-record: Calcular escopos do try e cauda, ordem de locais, dois dispatches, shortarray materializado antes dos stores e expansao de atribuicoes em array | cycle16: 1 matched, 2403 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos |
| CONFIRMADA | d10-smcs-subform-update2: Compor layout103, onze caudas positivas, bracos vazios48/2 e guarda event189 com Join101 nomeado; fixtures e NoEmit exatos, corpo aguarda oraculo. | cycle17: 1 matched, 12082 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos |
| CONFIRMADA | d10-smcs-userdata-draw: Compor basex compartilhado, cadeia de quatro zeros, Boolean44 e atribuicoes por braco para strings;46 locais NoEmit exatos. | cycle18: 1 matched, 7669 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos |
| CONFIRMADA | d10-large-lambda-composition-resource-lambda-inline-dispatch: Restaurar stores por braco no dispatch da lambda LoadStart sem escrever classe gerada, preservando as tres regioes catch e o layout original. | cycle19: 1 matched, 1928 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos |
| CONFIRMADA | d10-smcs-branch-joins: Restaurar joins e donos lexicais de cinco controles compostos, preservando locais, folhas e transferencias comprovadas pelas fixtures. | cycle20: 5 matched, 5339 IL bytes; Novos corpos aprovados; 5 tentativas da regra, 5 avancos |
| CONFIRMADA | d10-smcs-deleted-stores: Compor stores locais por braco e restaurar apenas o cache externo original de AddTouch; preservar nomes, expressoes float e negativo historico. | cycle21: 5 matched, 4627 IL bytes; Novos corpos aprovados; 5 tentativas da regra, 5 avancos |
| CONFIRMADA | d10-smcs-guard-slot: Testar composicao congelada com IDs originais, fixtures proprias e gate fresco; sem ganho presumido. | cycle22: 3 matched, 1592 IL bytes; Novos corpos aprovados; 3 tentativas da regra, 3 avancos |
| CONFIRMADA | d10-explicit-tree-locals-and-index-owners: Restaurar Nodes nomeados e resultado apos finally, reusar num de readBit e declarar tres indices Matrix no pai de uma vez. | cycle23: 3 matched, 1024 IL bytes; Novos corpos aprovados; 3 tentativas da regra, 3 avancos |
| CONFIRMADA | d10-large-lambda-composition-http-lambda-declaration-owner: Restaurar dono lexical dos locais da lambda HttpObject, escrevendo apenas o fonte pai e mantendo todas as folhas. | cycle24: 1 matched, 944 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos |
| CONFIRMADA | d10-roslyn-composition-language-body-local-stack-candidate: Restaurar local do corpo e valores em pilha de MakeLangPackTemplate, preservando referencias e ordem de folhas. | cycle25: 1 matched, 893 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos |
| CONFIRMADA | d10-late-tail-scope-comparison: Restaurar escopo tardio da cauda e alias em pilha nas duas comparacoes, mantendo os locais nomeados originais. | cycle26: 2 matched, 770 IL bytes; Novos corpos aprovados; 2 tentativas da regra, 2 avancos |
| CONFIRMADA | d10-localize-shared-boolean-cache: Recuperar Boolean compartilhado do cache de localizacao entre os bracos de lock e unlock, sem promover modelos incompletos. | cycle27: 2 matched, 737 IL bytes; Novos corpos aprovados; 2 tentativas da regra, 2 avancos |
| CONFIRMADA | d10-roslyn-bound-first-body-break: Restaurar primeira guarda com break no corpo dos lacos, preservando proprietario do incremento e escopos. | cycle28: 3 matched, 614 IL bytes; Novos corpos aprovados; 3 tentativas da regra, 3 avancos |
| CONFIRMADA | d10-roslyn-composition-material-tail-child-scope: Restaurar escopo filho da cauda MaterialManager para a alocacao original de locais e liberacao. | cycle29: 1 matched, 416 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos |
| CONFIRMADA | d10-residual-guard-dispatch: Separar continue misto de cauda positiva nos dois Player e restaurar switch de um case no Touch, com igualdade completa em fixtures e slots5/15/3. | cycle30: 3 matched, 1148 IL bytes; Novos corpos aprovados; 3 tentativas da regra, 3 avancos |
| INCONCLUSIVA | d10-residual-collision: Restore four owned continue guards, direct num3 dispatches and per-arm angle stores; remove two switch caches without changing floating comparisons | cycle31: 0 matched, 0 IL bytes; Avanco parcial; nenhum novo matched; 1 tentativas da regra, 1 avancos |
| CONFIRMADA | d10-residual-collision-default: Restore explicit empty outer dispatch default, preserving the accepted20locals and all floating comparisons | cycle32: 1 matched, 1970 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos |
| CONFIRMADA | d10-residual-scoped-guards: Compose owned loop guards and positive payloads, restore nested ranking break, and name the final stream Boolean in its original slot | cycle33: 3 matched, 2307 IL bytes; Novos corpos aprovados; 3 tentativas da regra, 3 avancos |
| CONFIRMADA | d10-residual-lambda-local-lifetimes: Restore file-completion Boolean across wait and callback; declare image limits in lambda parent block while preserving assignment positions | cycle34: 1 matched, 684 IL bytes; Novos corpos aprovados; 2 tentativas da regra, 2 avancos |
| CONFIRMADA | d10-residual-draw-composition: Compose state-local interval guards, per-arm English stores, state10 sibling arm, direct card dispatch and missing named frame21 | cycle35: 1 matched, 2309 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos |
| CONFIRMADA | As DLLs recompiladas da baseline aecb02b executam o cenário funcional observado no aparelho Android. | round10-functional-test.json: abertura, menu e partida completa Sun Rays 1-0 Blue Stars; 37 capturas, 677 linhas distintas, zero exceções Mono/Unity observadas. Não prova todos os caminhos nem as alterações C/D posteriores. |
| CONFIRMADA | Com os caminhos e opções atuais, igualar todos os corpos da KairoLibrary não elimina a diferença do caminho CodeView/PDB. | round10-KairoLibrary-raw-measurement.json: 97 bytes do caminho PDB diferentes após excluir analiticamente os quatro campos comprovados. Nenhum arquivo foi normalizado. |
| REFUTADA | O primeiro molde ampliado de EH já era correto em todos os casos classificados como completos. | Validação inicial da etapa C2 encontrou 14 planos completos incorretos no posicionamento de labels e início de EH. A versão foi recusada; as quatro etapas finais só aceitam planos e árvores completos sem discrepância no corpus matched. |
| REFUTADA | O IL sozinho sempre identifica se um salto terminal de switch era break de switch ou continue de laço no fonte. | Fixtures da etapa C4 emitem os mesmos bytes para formas fonte distintas; o owner fica INCOMPLETE quando as evidências não distinguem as construções. |
| REFUTADA | A composição de polaridade já ensaiada em SubForm.Init2 melhora o prefixo idêntico. | Auditoria congelada da família smcs-empty-arms identificou o mesmo fonte candidato de rodada anterior: duas compilações históricas reduziram o prefixo de 43 para 9 bytes. Candidato excluído, sem repetir o teste nesta rodada. |

Zero ganho permanece INCONCLUSIVA, salvo prova isolada de refutação. A classificação não identifica o fonte histórico literal.

| Assembly/modelo | Matched do corpus | Mapeados | Sem mapeamento | Completos | Completos exatos | Precisão | Cobertura | Mapeados incompletos | Incompletos com tipos iguais |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| KairoLibrary | 3185 | 2851 | 334 | 2248 | 2248 | 100.000000% | 70.580848% | 603 | 328 |
| Assembly-CSharp | 1886 | 1804 | 82 | 1803 | 1803 | 100.000000% | 95.599152% | 1 | 0 |

**Extrator de controle smcs.** Corpus congelado: `build/round10/final-corpus`. Planos e árvores completos têm denominadores distintos; uma cobertura integral das instruções não completa uma árvore ambígua.

| Métrica | Resultado |
| --- | --- |
| Árvores completas iguais / pares completos | 1314/1314 (100.000000%) |
| Cobertura das árvores nos corpos matched | 1314/1886 (69.671262%) |
| Planos completos iguais / completos | 1592/1592 |
| Pendentes: IL completo / fonte completa / pares completos | 25 / 26 / 15 |
| Parte C: diferenças antes / reescritas com árvore igual / corpos fechados | 8 / 1 / 1 |
| Parte C: bytes de IL adotados oficialmente | 717 |

A árvore original não distingue formas fonte que emitem os mesmos saltos sem evidência adicional. Ownership de salto ambíguo, EH, switch, construções anônimas e outros casos sem molde provado continuam incompletos. A AST é extraída somente do fonte; planos da compilação própria autenticam o vínculo das folhas, nunca inventam estrutura fonte. A correspondência final continua sendo corpo completo e metadados no verificador.

Precisão usa somente previsões completas; cobertura usa todos os corpos matched do corpus congelado. Incompletos ou métodos gerados sem mapeamento continuam UNKNOWN. Coincidência de tipos em previsão incompleta não é prova do lowering. Previsão incompleta PERMITE compilar: limite de 12 variantes por método/passagem, 24 por método/ciclo, no máximo uma sonda isolada e uma passagem plena. Cada variante pode exigir screen e compilação plena; variantes e invocações são medidas separadamente. Previsão completa com assinatura errada é recusada antes de compilar. Depois da compilação, a assinatura emitida precisa ser a original; previsão completa também precisa concordar com o emissor. Cabeçalho, locais, IL e EH continuam sujeitos ao mesmo verificador integral.

## 4. PENDENTES POR GRUPO

| Assembly | Faixa de IL original | Corpos pendentes | Bytes de IL |
| --- | --- | --- | --- |
| KairoLibrary | <=64 | 1 | 46 |
| KairoLibrary | 65-256 | 13 | 1934 |
| KairoLibrary | 257-1024 | 10 | 4285 |
| KairoLibrary | >1024 | 0 | 0 |
| Assembly-CSharp | <=64 | 5 | 237 |
| Assembly-CSharp | 65-256 | 15 | 2357 |
| Assembly-CSharp | 257-1024 | 28 | 14036 |
| Assembly-CSharp | >1024 | 17 | 35644 |

Pendentes com assinatura de locais igual: KairoLibrary 12/24 (diferente: 12); Assembly-CSharp 30/65 (diferente: 35).

Corpos já iguais, retidos por vínculo/metadados gerados: 3; 25 bytes. Esses corpos não entram no histograma dos divergentes.

Peso de um par = soma do tamanho integral dos métodos nele; não representa quantidade de bytes diferentes. Alinhamento e rótulos servem ao diagnóstico, sem normalizar o verificador. Seguem os cinco pares de opcodes mais pesados de cada compilador, com IL original e recompilado lado a lado.

**KairoLibrary #1: `brfalse.s => brfalse` — 1 métodos, 680 bytes.** Exemplo: `kairo.unity.ui.Image::<Use>b__96_0()->System.Void`.

Tentativas compiladas no grupo na rodada 10: condition-local: 1, d10-residual-lambda-local-lifetimes-use: 1, root-guard-positive-tail: 4, ternary-polarity: 1; 7 com previsão incompleta. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 24.

| IL original | IL recompilado |
| --- | --- |
| `IL_0089: brfalse.s L4` | `IL_0089: brfalse L20` |
| `IL_008b: ldstr "Image.use() 失敗 ["` | `IL_008e: ldstr "Image.use() 失敗 ["` |

**KairoLibrary #2: `ldc.i4.1 => DELETION_POINT` — 1 métodos, 661 bytes.** Exemplo: `kairo.unity.ui.Seb::Draw(kairo.unity.ui.Graphics,System.Single,System.Single,kairo.unity.ui.Image[],System.Int32[])->System.Void`.

Tentativas compiladas no grupo na rodada 10: root-guard-positive-tail: 1; 1 com previsão incompleta. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 12.

| IL original | IL recompilado |
| --- | --- |
| `IL_010a: ldc.i4.1` | `IL_010a: ldarg.s 4` |
| `IL_010b: brfalse L12` | `IL_010c: ldc.i4.8` |

**KairoLibrary #3: `brfalse.s => brtrue` — 1 métodos, 537 bytes.** Exemplo: `kairo.unity.ui.SaveRecorder::AutoSaveRun()->System.Void`.

Tentativas compiladas no grupo na rodada 10: nenhum. Histórico 7/8/9 compatível: UNKNOWN; não se presume compatibilidade após alteração do fonte. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 65.

| IL original | IL recompilado |
| --- | --- |
| `IL_0031: brfalse.s L2` | `IL_0031: brtrue L11` |
| `IL_0033: leave L14` | `IL_0036: ldarg.0` |

**KairoLibrary #4: `INSERTION_POINT => ldc.i4.0` — 1 métodos, 467 bytes.** Exemplo: `kairo.unity.graphics.ShaderBuilder::Init(kairo.unity.ui.Graphics,kairo.unity.ui.Image,UnityEngine.Color[],System.Single,System.Boolean,System.Int32,System.Boolean,System.Boolean,System.Boolean)->System.Void`.

Tentativas compiladas no grupo na rodada 10: nenhum. Histórico 7/8/9 compatível: UNKNOWN; não se presume compatibilidade após alteração do fonte. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 117.

| IL original | IL recompilado |
| --- | --- |
| `IL_00f9: ldarg.3` | `IL_00f9: ldc.i4.0` |
| `IL_00fa: stloc.0` | `IL_00fa: stloc.0` |

**KairoLibrary #5: `leave => br` — 1 métodos, 383 bytes.** Exemplo: `kairo.unity.ui.SoundOGG/<>c__DisplayClass4_0::<Play>b__0()->System.Void`.

Tentativas compiladas no grupo na rodada 10: nenhum. Histórico 7/8/9 compatível: UNKNOWN; não se presume compatibilidade após alteração do fonte. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 117.

| IL original | IL recompilado |
| --- | --- |
| `IL_0065: leave L9` | `IL_0065: br L7` |
| `IL_006a: ldarg.0` | `IL_006a: ldarg.0` |

**Assembly-CSharp #1: `stloc.s => stloc.s` — 3 métodos, 4287 bytes.** Exemplo: `main.AppData::SendSaveData(System.String,System.Int32,System.Exception)->System.Void`.

Tentativas compiladas no grupo na rodada 10: nenhum. Histórico 7/8/9 compatível: UNKNOWN; não se presume compatibilidade após alteração do fonte. Antes de compilar: wrong_signature: 2; orçamento recusou 0; repertório anterior filtrou 2632.

| IL original | IL recompilado |
| --- | --- |
| `IL_0262: stloc.s 41` | `IL_0262: stloc.s 13` |
| `IL_0264: ldc.i4.5` | `IL_0264: ldc.i4.5` |

**Assembly-CSharp #2: `bne.un => beq` — 1 métodos, 3844 bytes.** Exemplo: `form.SubForm::Init2()->System.Boolean`.

Tentativas compiladas no grupo na rodada 10: nenhum. Histórico 7/8/9 compatível: UNKNOWN; não se presume compatibilidade após alteração do fonte. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 682.

| IL original | IL recompilado |
| --- | --- |
| `IL_0053: bne.un L4` | `IL_0053: beq L3` |
| `IL_0058: ldc.i4.1` | `IL_0058: br L4` |

**Assembly-CSharp #3: `stloc.2 => stloc.0` — 2 métodos, 3787 bytes.** Exemplo: `form.BootForm::Load()->System.Void`.

Tentativas compiladas no grupo na rodada 10: nenhum. Histórico 7/8/9 compatível: UNKNOWN; não se presume compatibilidade após alteração do fonte. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 462.

| IL original | IL recompilado |
| --- | --- |
| `IL_004b: stloc.2` | `IL_004b: stloc.0` |
| `IL_004c: ldloc.2` | `IL_004c: ldloc.0` |

**Assembly-CSharp #4: `stloc.s => dup` — 1 métodos, 3287 bytes.** Exemplo: `game.UserData::SetTrainingCommonMethod(System.Boolean)->System.Void`.

Tentativas compiladas no grupo na rodada 10: nenhum. Histórico 7/8/9 compatível: UNKNOWN; não se presume compatibilidade após alteração do fonte. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 528.

| IL original | IL recompilado |
| --- | --- |
| `IL_0322: stloc.s 14` | `IL_0322: dup` |
| `IL_0324: ldloc.s 14` | `IL_0323: ldc.i4.0` |

**Assembly-CSharp #5: `ldc.i4.0 => DELETION_POINT` — 2 métodos, 2997 bytes.** Exemplo: `game.Map::SetCoatLine()->System.Void`.

Tentativas compiladas no grupo na rodada 10: nenhum. Histórico 7/8/9 compatível: UNKNOWN; não se presume compatibilidade após alteração do fonte. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 352.

| IL original | IL recompilado |
| --- | --- |
| `IL_023e: ldc.i4.0` | `IL_023e: ldloc.0` |
| `IL_023f: ldloc.0` | `IL_023f: ldc.i4.0` |

## 5. TOOLCHAIN

KairoLibrary: NuGet Microsoft.Net.Compilers 1.1.0; csc.exe 1.1.0.51109; compilador SHA-256 `53b693536e8a86f35d05d2165ff9b3e0aeaa499ecfd8135a9f99ab4404ca86c3`. Mudou compilador/opções/seleção de referências nesta rodada: NÃO.

Opções: `/nologo /noconfig /nostdlib+ /target:library /platform:anycpu /optimize+ /debug:pdbonly /deterministic- /langversion:6 /unsafe- /checked- /codepage:65001 /utf8output`.

Referências em ordem: mscorlib.dll, System.Core.dll, UnityEngine.dll, System.dll.

Assembly-CSharp: smcs.exe 2.6.5.0 do Unity 5.3.4f1; compilador SHA-256 `26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0`. Mudou compilador/opções/seleção de referências nesta rodada: NÃO.

Opções: `-nologo -noconfig -target:library -platform:anycpu -optimize+ -debug- -langversion:Default -unsafe- -checked- -codepage:65001`.

A KairoLibrary referenciada é recompilada com o projeto; seu hash de artefato pode mudar, sem troca da seleção de frameworks ou do compilador.

Referências em ordem: mscorlib.dll, KairoLibrary.dll, UnityEngine.dll, System.Core.dll, System.dll.

Compilador histórico exato, comando histórico integral e hashes das referências originalmente usadas: UNKNOWN. Os candidatos acima têm hashes fixados e prova de corpos; isso não identifica sua fabricação histórica. Roslyn moderno compila auxiliares e fornece AST, não os assemblies alvos. PDB/MDB auxiliam apenas o mapeamento. Nomes escolhidos para locais não são apresentados como nomes originais.

## 6. COMPONENTES EXTERNOS E METADADOS

| Assembly | Tipos com metadados iguais | Declarações sem corpo aprovadas | Ordem de tipos | Metadados globais | Bytes brutos |
| --- | --- | --- | --- | --- | --- |
| KairoLibrary | 337/337 | 76/76 | True | True | NOT_EVALUATED |
| Assembly-CSharp | 129/129 | 6/6 | True | True | NOT_EVALUATED |

A comparação abrange ordem de tipos e membros, parâmetros, atributos personalizados, layout, constantes, recursos e referências globais; o guard de identidade confere o SHA do original e do build atual. A medição bruta KairoLibrary da Parte B descreve a baseline inicial; o estado de identidade bruta do verificador acima continua separado dessa medição. Na reconstrução das DLLs, nenhum byte original é inserido no resultado, nenhuma classe gerada é escrita à mão e nenhum assembly é normalizado. O APK funcional da Parte A contém os demais arquivos originais conforme solicitado e não é a reconstrução final.

Externos gerenciados com correspondência oficial registrada: 7/7: Mono.Security.dll, System.Core.dll, System.dll, UnityEngine.Networking.dll, UnityEngine.UI.dll, UnityEngine.dll, mscorlib.dll. UnityEngine.dll: BYTE_IDENTICAL_OFFICIAL_ANDROID_MONO; SHA-256 `7cc901371cc77e103d00b5845e8285f85fbdf36bb47ccf3d824f94c4ead32421`. A variante é Android/Mono do Unity 5.3.4f1; evidência histórica, não nova decompilação.

Externos nativos com correspondência oficial registrada: 6/6; lib/armeabi-v7a/libmain.so, lib/armeabi-v7a/libmono.so, lib/armeabi-v7a/libunity.so, lib/x86/libmain.so, lib/x86/libmono.so, lib/x86/libunity.so. DEX/Java permanece fora do escopo.

Campos não determinísticos comprovados por pares de recompilações anteriores (não normalizados): KairoLibrary — Module.Mvid, COFF.TimeDateStamp, IMAGE_DEBUG_DIRECTORY.TimeDateStamp, CodeView.RSDS.PdbGuid; Assembly-CSharp — Module.Mvid, COFF.TimeDateStamp. Determinismo histórico do original: UNKNOWN.

## 7. BLOQUEIOS

Nenhuma instalação, arquivo ou decisão de escopo exigida para esta rodada. Métodos restantes continuam divergentes; a parada segue o limite autorizado.

Parada após 35 ciclos autônomos D e 1 ciclo adicional de preparo registrado no diário; as partes A–C estão descritas na seção 2. Sequência autônoma final sem novo matched: 0. Limites configurados: 35 ciclos autônomos, 5 sem ganho consecutivos com fila vazia; cota Kairo: at least one KairoLibrary cycle in every consecutive block of five cycles.

## 8. SUA RECOMENDAÇÃO

1. Investigar layout e aliases de locais nos cinco métodos dos pares stloc.s => stloc.s (3 métodos / 4.287 bytes de IL) e stloc.2 => stloc.0 (2 / 3.787): potencial medido de 5 métodos / 8.074 bytes. Calcular os papéis e o primeiro e último uso dos slots para compor declarações e controle; permutações genéricas já foram ensaiadas. Ganho real UNKNOWN.
2. Concluir a lambda Image.Use (1 método / 680 bytes de IL): D34 confirmou a assinatura real de 12 locais e avanço de 88 para 131 bytes, mas o corpo não fechou. O primeiro par restante é brfalse.s => brfalse. Investigar a fronteira de controle e EH, preservando o resultado negativo integral de D34. Potencial de 680 bytes; ganho UNKNOWN.
3. Reconstituir o controle de SubForm.Init2 (1 método / 3.844 bytes de IL), último caso bne.un => beq: os 68 locais já coincidem, mas foram delimitadas 136 regiões de alinhamento. Exigir uma composição com donos de saídas e fixture, preservando os negativos de polaridade isolada. Potencial de 3.844 bytes; ganho UNKNOWN.
