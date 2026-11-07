## 1. PROGRESSO

Pocket League Story para Android, APK net.kairosoft.android.soccer_en_2.0.2-APK_Award.apk; reconstrução C#/Unity 5.3.4f1. Comparação com o início desta rodada, commit `aecb02b8a7b0965508e223335e44ae36a95a0939`. Matched exige corpo completo (cabeçalho, maxstack, locais, IL e EH); referências de tokens são resolvidas por identidade. Declarações sem corpo contam em métodos, sem bytes de IL.

| Assembly | Anterior (aecb02b) | Atual | Δ métodos | Δ bytes de IL | Regressões |
| --- | --- | --- | --- | --- | --- |
| KairoLibrary | 3241/3287 (98.600548%); IL 249623/269894 bytes (92.489274%) | 3261/3287 (99.209005%); IL 263611/269894 bytes (97.672049%) | 20 | 13988 | 0 |
| Assembly-CSharp | 1844/1958 (94.177732%); IL 447600/601603 bytes (74.401225%) | 1892/1958 (96.629213%); IL 549322/601603 bytes (91.309718%) | 48 | 101722 | 0 |

Total: 5153/5245 métodos; 812933/871497 bytes de IL. Retiradas de matched por ID: 0; corpos antes iguais que deixaram de bater: 0.

Motivo da parada: Limite de 35 ciclos autônomos concluídos.

| Original | SHA-256 |
| --- | --- |
| APK | `d557af6b4bc7748605a39ed3548821f6763c5773a12432a3fed6e8c413b211e8` |
| KairoLibrary.dll | `bb553d341eecd9b900f772486bf4a5c289aa2f09566d9d402a3eb70268f4b3ee` |
| Assembly-CSharp.dll | `3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949` |

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

| Ciclo | Compilador / par | Hipótese | Resultado | Matched ganhos | Bytes ganhos | Matched perdidos |
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

| Execução | Natureza | Transformação | Variantes tentadas | Avanços | Corpos ganhos no motor | Bytes no motor | Perdas adotadas |
| --- | --- | --- | --- | --- | --- | --- | --- |
| round10-c01-Assembly-CSharp-engine.json | plena adotada | c10-control-tree-reconstruction | 1 | 1 | 1 | 717 | 0 |
| round10-c01-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | c10-control-tree-reconstruction | 1 | 1 | 1 | 717 | não adotado |
| round10-cycle01-Assembly-CSharp-engine.json | plena adotada | d10-exact-setwin-chara-training | 1 | 1 | 1 | 412 | 0 |
| round10-cycle01-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | d10-exact-setwin-chara-training | 1 | 1 | 1 | 412 | não adotado |
| round10-cycle02-Assembly-CSharp-engine.json | plena adotada | static-initializer-ternary-polarity | 3 | 3 | 1 | 1816 | 0 |
| round10-cycle02-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | static-initializer-ternary-polarity | 1 | 1 | 1 | 1816 | não adotado |
| round10-cycle03-Assembly-CSharp-engine.json | plena adotada | d10-subform-draw4-composition | 1 | 1 | 1 | 12484 | 0 |
| round10-cycle03-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | d10-subform-draw4-composition | 1 | 1 | 1 | 12484 | não adotado |
| round10-cycle04-Assembly-CSharp-engine.json | plena adotada | root-guard-positive-tail | 10 | 0 | 0 | 0 | 0 |
| round10-cycle04-Assembly-CSharp-engine.json | plena adotada | root-guard-positive-tail-composed-sp | 1 | 1 | 1 | 347 | 0 |
| round10-cycle04-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | root-guard-positive-tail-composed-sp | 1 | 1 | 1 | 347 | não adotado |
| round10-cycle05-KairoLibrary-engine.json | plena adotada | local-declaration-scope-hoist | 7 | 0 | 0 | 0 | 0 |
| round10-cycle05-KairoLibrary-engine.json | plena adotada | root-guard-positive-tail | 10 | 2 | 2 | 600 | 0 |
| round10-cycle05-KairoLibrary-engine.json | plena adotada | root-guard-positive-tail-composed-canvas | 1 | 1 | 0 | 0 | 0 |
| round10-cycle05-KairoLibrary-engine.json | plena adotada | root-guard-positive-tail-split-prefix-1 | 1 | 0 | 0 | 0 | 0 |
| round10-cycle05-KairoLibrary-engine.json | plena adotada | root-guard-positive-tail-split-prefix-2 | 1 | 0 | 0 | 0 | 0 |
| round10-cycle05-probe-KairoLibrary-engine.json | sonda/preparo/não adotada | local-declaration-scope-hoist | 7 | 0 | 0 | 0 | não adotado |
| round10-cycle05-probe-KairoLibrary-engine.json | sonda/preparo/não adotada | root-guard-positive-tail | 2 | 0 | 0 | 0 | não adotado |
| round10-cycle05-probe-KairoLibrary-engine.json | sonda/preparo/não adotada | root-guard-positive-tail-composed-canvas | 1 | 1 | 0 | 0 | não adotado |
| round10-cycle05-probe-KairoLibrary-engine.json | sonda/preparo/não adotada | root-guard-positive-tail-split-prefix-1 | 1 | 0 | 0 | 0 | não adotado |
| round10-cycle05-probe-KairoLibrary-engine.json | sonda/preparo/não adotada | root-guard-positive-tail-split-prefix-2 | 1 | 0 | 0 | 0 | não adotado |
| round10-cycle06-Assembly-CSharp-engine.json | plena adotada | d10-integer-empty-arm-composition-empty-case-positive-return-composition | 1 | 1 | 1 | 487 | 0 |
| round10-cycle06-Assembly-CSharp-engine.json | plena adotada | d10-integer-empty-arm-composition-empty-case-positive-tail-composition | 1 | 1 | 1 | 1007 | 0 |
| round10-cycle06-Assembly-CSharp-engine.json | plena adotada | d10-integer-empty-arm-composition-known-empty-positive-equality-coverage | 2 | 2 | 2 | 905 | 0 |
| round10-cycle06-Assembly-CSharp-engine.json | plena adotada | d10-integer-empty-arm-composition-terminal-empty-OR-chain | 2 | 2 | 2 | 1059 | 0 |
| round10-cycle06-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | d10-integer-empty-arm-composition-terminal-empty-OR-chain | 1 | 1 | 1 | 710 | não adotado |
| round10-cycle07-KairoLibrary-engine.json | plena adotada | d10-roslyn-composition-http-zero-guard-dense-default-bucket | 1 | 1 | 1 | 1054 | 0 |
| round10-cycle07-probe-KairoLibrary-engine.json | sonda/preparo/não adotada | d10-roslyn-composition-http-zero-guard-dense-default-bucket | 1 | 1 | 1 | 1054 | não adotado |
| round10-cycle08-Assembly-CSharp-engine.json | plena adotada | d10-static-bool-composition | 1 | 1 | 1 | 341 | 0 |
| round10-cycle08-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | d10-static-bool-composition | 1 | 1 | 1 | 341 | não adotado |
| round10-cycle09-Assembly-CSharp-engine.json | plena adotada | d10-smcs-bool-value-calculated-smcs-bool-alias-guard-composition-edit | 1 | 1 | 1 | 5801 | 0 |
| round10-cycle09-Assembly-CSharp-engine.json | plena adotada | d10-smcs-bool-value-calculated-smcs-bool-alias-guard-composition-team | 1 | 1 | 1 | 669 | 0 |
| round10-cycle09-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | d10-smcs-bool-value-calculated-smcs-bool-alias-guard-composition-edit | 1 | 1 | 1 | 5801 | não adotado |
| round10-cycle10-Assembly-CSharp-engine.json | plena adotada | d10-smcs-unused-slot-d10-calculated-unused-local-scope-composition | 3 | 3 | 3 | 2417 | 0 |
| round10-cycle10-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | d10-smcs-unused-slot-d10-calculated-unused-local-scope-composition | 1 | 1 | 1 | 1054 | não adotado |
| round10-cycle11-Assembly-CSharp-engine.json | plena adotada | assignment-ternary-if | 7 | 7 | 0 | 0 | 0 |
| round10-cycle11-Assembly-CSharp-engine.json | plena adotada | d10-smcs-named-selector-d10-calculated-named-selector-dispatch-scope | 2 | 2 | 1 | 1420 | 0 |
| round10-cycle11-Assembly-CSharp-engine.json | plena adotada | if-polarity | 3 | 0 | 0 | 0 | 0 |
| round10-cycle11-Assembly-CSharp-engine.json | plena adotada | initialized-declaration-swap | 2 | 0 | 0 | 0 | 0 |
| round10-cycle11-Assembly-CSharp-engine.json | plena adotada | local-declaration-scope-hoist | 3 | 0 | 0 | 0 | 0 |
| round10-cycle11-Assembly-CSharp-engine.json | plena adotada | smcs-continue-positive-tail | 1 | 1 | 1 | 3472 | 0 |
| round10-cycle11-Assembly-CSharp-engine.json | plena adotada | smcs-empty-arm-restore | 3 | 0 | 0 | 0 | 0 |
| round10-cycle11-Assembly-CSharp-engine.json | plena adotada | solver-group-all-calculated-blocks | 8 | 0 | 0 | 0 | 0 |
| round10-cycle11-Assembly-CSharp-engine.json | plena adotada | solver-group-block-declarations | 8 | 0 | 0 | 0 | 0 |
| round10-cycle11-Assembly-CSharp-engine.json | plena adotada | ternary-polarity | 3 | 3 | 0 | 0 | 0 |
| round10-cycle11-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | assignment-ternary-if | 7 | 7 | 0 | 0 | não adotado |
| round10-cycle11-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | d10-smcs-named-selector-d10-calculated-named-selector-dispatch-scope | 1 | 1 | 0 | 0 | não adotado |
| round10-cycle11-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | if-polarity | 3 | 0 | 0 | 0 | não adotado |
| round10-cycle11-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | initialized-declaration-swap | 2 | 0 | 0 | 0 | não adotado |
| round10-cycle11-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | local-declaration-scope-hoist | 3 | 0 | 0 | 0 | não adotado |
| round10-cycle11-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | smcs-continue-positive-tail | 1 | 1 | 1 | 3472 | não adotado |
| round10-cycle11-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | smcs-empty-arm-restore | 3 | 0 | 0 | 0 | não adotado |
| round10-cycle11-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | solver-group-all-calculated-blocks | 8 | 0 | 0 | 0 | não adotado |
| round10-cycle11-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | solver-group-block-declarations | 8 | 0 | 0 | 0 | não adotado |
| round10-cycle11-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | ternary-polarity | 3 | 3 | 0 | 0 | não adotado |
| round10-cycle12-KairoLibrary-engine.json | plena adotada | d10-canvas-two-residual-composition | 1 | 1 | 1 | 1921 | 0 |
| round10-cycle12-probe-KairoLibrary-engine.json | sonda/preparo/não adotada | d10-canvas-two-residual-composition | 1 | 1 | 1 | 1921 | não adotado |
| round10-cycle13-Assembly-CSharp-engine.json | plena adotada | condition-merge-or-guards | 1 | 0 | 0 | 0 | 0 |
| round10-cycle13-Assembly-CSharp-engine.json | plena adotada | d10-smcs-tplayer-d10-calculated-training-script-complete-dispatch-slots | 1 | 1 | 0 | 0 | 0 |
| round10-cycle13-Assembly-CSharp-engine.json | plena adotada | smcs-continue-positive-tail | 2 | 2 | 1 | 9408 | 0 |
| round10-cycle13-Assembly-CSharp-engine.json | plena adotada | smcs-empty-arm-restore | 2 | 0 | 0 | 0 | 0 |
| round10-cycle13-Assembly-CSharp-engine.json | plena adotada | solver-group-all-calculated-blocks | 2 | 0 | 0 | 0 | 0 |
| round10-cycle13-Assembly-CSharp-engine.json | plena adotada | solver-group-block-declarations | 2 | 0 | 0 | 0 | 0 |
| round10-cycle13-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | condition-merge-or-guards | 1 | 0 | 0 | 0 | não adotado |
| round10-cycle13-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | d10-smcs-tplayer-d10-calculated-training-script-complete-dispatch-slots | 1 | 1 | 0 | 0 | não adotado |
| round10-cycle13-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | smcs-continue-positive-tail | 2 | 2 | 1 | 9408 | não adotado |
| round10-cycle13-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | smcs-empty-arm-restore | 2 | 0 | 0 | 0 | não adotado |
| round10-cycle13-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | solver-group-all-calculated-blocks | 2 | 0 | 0 | 0 | não adotado |
| round10-cycle13-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | solver-group-block-declarations | 2 | 0 | 0 | 0 | não adotado |
| round10-cycle14-Assembly-CSharp-engine.json | plena adotada | d10-smcs-subform-update-calculated-smcs-large-update-integral-control | 1 | 1 | 1 | 11390 | 0 |
| round10-cycle14-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | d10-smcs-subform-update-calculated-smcs-large-update-integral-control | 1 | 1 | 1 | 11390 | não adotado |
| round10-cycle15-Assembly-CSharp-engine.json | plena adotada | d10-smcs-switch-join-d10-smcs-switch-layout-d10-calculated-dispatch-local-join-layout | 2 | 2 | 2 | 7347 | 0 |
| round10-cycle15-Assembly-CSharp-engine.json | plena adotada | d10-smcs-switch-join-d10-smcs-switch-selector-calculated-smcs-dispatch-join-composition-boot | 1 | 1 | 1 | 487 | 0 |
| round10-cycle15-Assembly-CSharp-engine.json | plena adotada | d10-smcs-switch-join-d10-smcs-switch-selector-calculated-smcs-dispatch-join-composition-topbar | 1 | 1 | 1 | 693 | 0 |
| round10-cycle15-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | d10-smcs-switch-join-d10-smcs-switch-layout-d10-calculated-dispatch-local-join-layout | 1 | 1 | 1 | 4742 | não adotado |
| round10-cycle16-KairoLibrary-engine.json | plena adotada | d10-large-lambda-composition-seb-root-tail-dispatch-record | 1 | 1 | 1 | 2403 | 0 |
| round10-cycle16-probe-KairoLibrary-engine.json | sonda/preparo/não adotada | d10-large-lambda-composition-seb-root-tail-dispatch-record | 1 | 1 | 1 | 2403 | não adotado |
| round10-cycle17-Assembly-CSharp-engine.json | plena adotada | d10-smcs-subform-update2-calculated-smcs-update2-layout-and-terminal-tail-composition | 1 | 1 | 1 | 12082 | 0 |
| round10-cycle17-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | d10-smcs-subform-update2-calculated-smcs-update2-layout-and-terminal-tail-composition | 1 | 1 | 1 | 12082 | não adotado |
| round10-cycle18-Assembly-CSharp-engine.json | plena adotada | d10-smcs-userdata-draw-calculated-smcs-userdata-draw-layout-and-value-composition | 1 | 1 | 1 | 7669 | 0 |
| round10-cycle18-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | d10-smcs-userdata-draw-calculated-smcs-userdata-draw-layout-and-value-composition | 1 | 1 | 1 | 7669 | não adotado |
| round10-cycle19-KairoLibrary-engine.json | plena adotada | d10-large-lambda-composition-resource-lambda-inline-dispatch | 1 | 1 | 1 | 1928 | 0 |
| round10-cycle19-probe-KairoLibrary-engine.json | sonda/preparo/não adotada | d10-large-lambda-composition-resource-lambda-inline-dispatch | 1 | 1 | 1 | 1928 | não adotado |
| round10-cycle20-Assembly-CSharp-engine.json | plena adotada | d10-smcs-branch-joins-d10-calculated-branch-owner-join | 5 | 5 | 5 | 5339 | 0 |
| round10-cycle20-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | d10-smcs-branch-joins-d10-calculated-branch-owner-join | 1 | 1 | 1 | 2052 | não adotado |
| round10-cycle21-Assembly-CSharp-engine.json | plena adotada | d10-smcs-deleted-stores-d10-calculated-deleted-store-and-selector-layout | 5 | 5 | 5 | 4627 | 0 |
| round10-cycle21-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | d10-smcs-deleted-stores-d10-calculated-deleted-store-and-selector-layout | 1 | 1 | 1 | 1863 | não adotado |
| round10-cycle22-Assembly-CSharp-engine.json | plena adotada | d10-smcs-guard-slot-calculated-smcs-guard-slot-composition-banner | 1 | 1 | 1 | 391 | 0 |
| round10-cycle22-Assembly-CSharp-engine.json | plena adotada | d10-smcs-guard-slot-calculated-smcs-guard-slot-composition-billing | 1 | 1 | 1 | 629 | 0 |
| round10-cycle22-Assembly-CSharp-engine.json | plena adotada | d10-smcs-guard-slot-calculated-smcs-guard-slot-composition-sp | 1 | 1 | 1 | 572 | 0 |
| round10-cycle22-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | d10-smcs-guard-slot-calculated-smcs-guard-slot-composition-billing | 1 | 1 | 1 | 629 | não adotado |
| round10-cycle23-KairoLibrary-engine.json | plena adotada | d10-explicit-tree-locals-and-index-owners-compress-explicit-nodes-result | 1 | 1 | 1 | 784 | 0 |
| round10-cycle23-KairoLibrary-engine.json | plena adotada | d10-explicit-tree-locals-and-index-owners-preconcat-parent-indices | 1 | 1 | 1 | 149 | 0 |
| round10-cycle23-KairoLibrary-engine.json | plena adotada | d10-explicit-tree-locals-and-index-owners-readbit-existing-num | 1 | 1 | 1 | 91 | 0 |
| round10-cycle23-probe-KairoLibrary-engine.json | sonda/preparo/não adotada | d10-explicit-tree-locals-and-index-owners-compress-explicit-nodes-result | 1 | 1 | 1 | 784 | não adotado |
| round10-cycle24-KairoLibrary-engine.json | plena adotada | d10-large-lambda-composition-http-lambda-declaration-owner | 1 | 1 | 1 | 944 | 0 |
| round10-cycle24-probe-KairoLibrary-engine.json | sonda/preparo/não adotada | d10-large-lambda-composition-http-lambda-declaration-owner | 1 | 1 | 1 | 944 | não adotado |
| round10-cycle25-KairoLibrary-engine.json | plena adotada | d10-roslyn-composition-language-body-local-stack-candidate | 1 | 1 | 1 | 893 | 0 |
| round10-cycle25-probe-KairoLibrary-engine.json | sonda/preparo/não adotada | d10-roslyn-composition-language-body-local-stack-candidate | 1 | 1 | 1 | 893 | não adotado |
| round10-cycle26-KairoLibrary-engine.json | plena adotada | d10-late-tail-scope-comparison-image-late-tail-scope | 1 | 1 | 1 | 230 | 0 |
| round10-cycle26-KairoLibrary-engine.json | plena adotada | d10-late-tail-scope-comparison-translate-late-tail-stack-alias | 1 | 1 | 1 | 540 | 0 |
| round10-cycle26-probe-KairoLibrary-engine.json | sonda/preparo/não adotada | d10-late-tail-scope-comparison-translate-late-tail-stack-alias | 1 | 1 | 1 | 540 | não adotado |
| round10-cycle27-KairoLibrary-engine.json | plena adotada | d10-localize-shared-boolean-cache-graphics-localize-stack-bool | 1 | 1 | 1 | 341 | 0 |
| round10-cycle27-KairoLibrary-engine.json | plena adotada | d10-localize-shared-boolean-cache-layout-localize-stack-bool | 1 | 1 | 1 | 396 | 0 |
| round10-cycle27-probe-KairoLibrary-engine.json | sonda/preparo/não adotada | d10-localize-shared-boolean-cache-layout-localize-stack-bool | 1 | 1 | 1 | 396 | não adotado |
| round10-cycle28-KairoLibrary-engine.json | plena adotada | d10-roslyn-bound-first-body-break-matrix | 1 | 1 | 1 | 216 | 0 |
| round10-cycle28-KairoLibrary-engine.json | plena adotada | d10-roslyn-bound-first-body-break-skip | 1 | 1 | 1 | 33 | 0 |
| round10-cycle28-KairoLibrary-engine.json | plena adotada | d10-roslyn-bound-first-body-break-split | 1 | 1 | 1 | 365 | 0 |
| round10-cycle28-probe-KairoLibrary-engine.json | sonda/preparo/não adotada | d10-roslyn-bound-first-body-break-split | 1 | 1 | 1 | 365 | não adotado |
| round10-cycle29-KairoLibrary-engine.json | plena adotada | condition-local | 1 | 0 | 0 | 0 | 0 |
| round10-cycle29-KairoLibrary-engine.json | plena adotada | d10-roslyn-composition-material-tail-child-scope | 1 | 1 | 0 | 0 | 0 |
| round10-cycle29-KairoLibrary-engine.json | plena adotada | root-guard-positive-tail | 1 | 1 | 1 | 416 | 0 |
| round10-cycle29-probe-KairoLibrary-engine.json | sonda/preparo/não adotada | condition-local | 1 | 0 | 0 | 0 | não adotado |
| round10-cycle29-probe-KairoLibrary-engine.json | sonda/preparo/não adotada | d10-roslyn-composition-material-tail-child-scope | 1 | 1 | 0 | 0 | não adotado |
| round10-cycle29-probe-KairoLibrary-engine.json | sonda/preparo/não adotada | root-guard-positive-tail | 1 | 1 | 1 | 416 | não adotado |
| round10-cycle30-Assembly-CSharp-engine.json | plena adotada | d10-residual-guard-dispatch-move | 1 | 1 | 1 | 258 | 0 |
| round10-cycle30-Assembly-CSharp-engine.json | plena adotada | d10-residual-guard-dispatch-pid | 1 | 1 | 1 | 809 | 0 |
| round10-cycle30-Assembly-CSharp-engine.json | plena adotada | d10-residual-guard-dispatch-touch | 1 | 1 | 1 | 81 | 0 |
| round10-cycle30-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | d10-residual-guard-dispatch-pid | 1 | 1 | 1 | 809 | não adotado |
| round10-cycle31-Assembly-CSharp-engine.json | plena adotada | d10-residual-collision-collision | 1 | 1 | 0 | 0 | 0 |
| round10-cycle31-Assembly-CSharp-engine.json | plena adotada | root-guard-positive-tail | 1 | 0 | 0 | 0 | 0 |
| round10-cycle31-Assembly-CSharp-engine.json | plena adotada | solver-group-all-calculated-blocks | 1 | 0 | 0 | 0 | 0 |
| round10-cycle31-Assembly-CSharp-engine.json | plena adotada | solver-group-block-declarations | 5 | 0 | 0 | 0 | 0 |
| round10-cycle31-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | d10-residual-collision-collision | 1 | 1 | 0 | 0 | não adotado |
| round10-cycle31-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | root-guard-positive-tail | 1 | 0 | 0 | 0 | não adotado |
| round10-cycle31-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | solver-group-all-calculated-blocks | 1 | 0 | 0 | 0 | não adotado |
| round10-cycle31-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | solver-group-block-declarations | 5 | 0 | 0 | 0 | não adotado |
| round10-cycle32-Assembly-CSharp-engine.json | plena adotada | d10-residual-collision-default-collision | 1 | 1 | 1 | 1970 | 0 |
| round10-cycle32-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | d10-residual-collision-default-collision | 1 | 1 | 1 | 1970 | não adotado |
| round10-cycle33-Assembly-CSharp-engine.json | plena adotada | d10-residual-scoped-guards-ranking | 1 | 1 | 1 | 538 | 0 |
| round10-cycle33-Assembly-CSharp-engine.json | plena adotada | d10-residual-scoped-guards-reflect | 1 | 1 | 1 | 821 | 0 |
| round10-cycle33-Assembly-CSharp-engine.json | plena adotada | d10-residual-scoped-guards-repetition | 1 | 1 | 1 | 948 | 0 |
| round10-cycle33-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | d10-residual-scoped-guards-repetition | 1 | 1 | 1 | 948 | não adotado |
| round10-cycle34-KairoLibrary-engine.json | plena adotada | condition-local | 1 | 0 | 0 | 0 | 0 |
| round10-cycle34-KairoLibrary-engine.json | plena adotada | d10-residual-lambda-local-lifetimes-capture | 1 | 1 | 1 | 684 | 0 |
| round10-cycle34-KairoLibrary-engine.json | plena adotada | d10-residual-lambda-local-lifetimes-use | 1 | 1 | 0 | 0 | 0 |
| round10-cycle34-KairoLibrary-engine.json | plena adotada | root-guard-positive-tail | 2 | 0 | 0 | 0 | 0 |
| round10-cycle34-KairoLibrary-engine.json | plena adotada | ternary-polarity | 1 | 0 | 0 | 0 | 0 |
| round10-cycle34-probe-KairoLibrary-engine.json | sonda/preparo/não adotada | d10-residual-lambda-local-lifetimes-capture | 1 | 1 | 1 | 684 | não adotado |
| round10-cycle35-Assembly-CSharp-engine.json | plena adotada | d10-residual-draw-composition-draw | 1 | 1 | 1 | 2309 | 0 |
| round10-cycle35-probe-Assembly-CSharp-engine.json | sonda/preparo/não adotada | d10-residual-draw-composition-draw | 1 | 1 | 1 | 2309 | não adotado |

| Execução | Métodos/passes | Compilações falhas | Regressões rejeitadas | Metadados rejeitados | Modelo/emissor discordam | Assinatura original recusada | Rebase externo recusado | Orçamento incompleto recusado | Paradas |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| round10-c01-Assembly-CSharp-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1 |
| round10-c01-probe-Assembly-CSharp-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1 |
| round10-cycle01-Assembly-CSharp-engine.json | 112 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1, no_model_approved_candidate: 111 |
| round10-cycle01-probe-Assembly-CSharp-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1 |
| round10-cycle02-Assembly-CSharp-engine.json | 111 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1, no_model_approved_candidate: 110 |
| round10-cycle02-probe-Assembly-CSharp-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1 |
| round10-cycle03-Assembly-CSharp-engine.json | 110 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1, no_model_approved_candidate: 109 |
| round10-cycle03-probe-Assembly-CSharp-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1 |
| round10-cycle04-Assembly-CSharp-engine.json | 109 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1, no_model_approved_candidate: 102, no_transformation_advances: 6 |
| round10-cycle04-probe-Assembly-CSharp-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1 |
| round10-cycle05-KairoLibrary-engine.json | 44 | 0 | 0 | 0 | 0 | 14 | 1 | 6 | body_equal: 2, incomplete_candidate_budget: 1, no_model_approved_candidate: 36, no_transformation_advances: 5 |
| round10-cycle05-probe-KairoLibrary-engine.json | 1 | 0 | 0 | 0 | 0 | 10 | 1 | 6 | incomplete_candidate_budget: 1 |
| round10-cycle06-Assembly-CSharp-engine.json | 108 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 6, no_model_approved_candidate: 102 |
| round10-cycle06-probe-Assembly-CSharp-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1 |
| round10-cycle07-KairoLibrary-engine.json | 42 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1, no_model_approved_candidate: 41 |
| round10-cycle07-probe-KairoLibrary-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1 |
| round10-cycle08-Assembly-CSharp-engine.json | 102 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1, no_model_approved_candidate: 101 |
| round10-cycle08-probe-Assembly-CSharp-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1 |
| round10-cycle09-Assembly-CSharp-engine.json | 101 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 2, no_model_approved_candidate: 99 |
| round10-cycle09-probe-Assembly-CSharp-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1 |
| round10-cycle10-Assembly-CSharp-engine.json | 99 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 3, no_model_approved_candidate: 96 |
| round10-cycle10-probe-Assembly-CSharp-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1 |
| round10-cycle11-Assembly-CSharp-engine.json | 96 | 0 | 0 | 0 | 0 | 0 | 11 | 0 | body_equal: 2, no_model_approved_candidate: 94 |
| round10-cycle11-probe-Assembly-CSharp-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 11 | 0 | body_equal: 1 |
| round10-cycle12-KairoLibrary-engine.json | 41 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1, no_model_approved_candidate: 40 |
| round10-cycle12-probe-KairoLibrary-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1 |
| round10-cycle13-Assembly-CSharp-engine.json | 94 | 0 | 0 | 0 | 0 | 0 | 2 | 0 | body_equal: 1, no_model_approved_candidate: 93 |
| round10-cycle13-probe-Assembly-CSharp-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 2 | 0 | body_equal: 1 |
| round10-cycle14-Assembly-CSharp-engine.json | 93 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1, no_model_approved_candidate: 92 |
| round10-cycle14-probe-Assembly-CSharp-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1 |
| round10-cycle15-Assembly-CSharp-engine.json | 92 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 4, no_model_approved_candidate: 88 |
| round10-cycle15-probe-Assembly-CSharp-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1 |
| round10-cycle16-KairoLibrary-engine.json | 40 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1, no_model_approved_candidate: 39 |
| round10-cycle16-probe-KairoLibrary-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1 |
| round10-cycle17-Assembly-CSharp-engine.json | 88 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1, no_model_approved_candidate: 87 |
| round10-cycle17-probe-Assembly-CSharp-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1 |
| round10-cycle18-Assembly-CSharp-engine.json | 87 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1, no_model_approved_candidate: 86 |
| round10-cycle18-probe-Assembly-CSharp-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1 |
| round10-cycle19-KairoLibrary-engine.json | 39 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1, no_model_approved_candidate: 38 |
| round10-cycle19-probe-KairoLibrary-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1 |
| round10-cycle20-Assembly-CSharp-engine.json | 86 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 5, no_model_approved_candidate: 81 |
| round10-cycle20-probe-Assembly-CSharp-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1 |
| round10-cycle21-Assembly-CSharp-engine.json | 81 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 5, no_model_approved_candidate: 76 |
| round10-cycle21-probe-Assembly-CSharp-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1 |
| round10-cycle22-Assembly-CSharp-engine.json | 76 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 3, no_model_approved_candidate: 73 |
| round10-cycle22-probe-Assembly-CSharp-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1 |
| round10-cycle23-KairoLibrary-engine.json | 38 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 3, no_model_approved_candidate: 35 |
| round10-cycle23-probe-KairoLibrary-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1 |
| round10-cycle24-KairoLibrary-engine.json | 35 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1, no_model_approved_candidate: 34 |
| round10-cycle24-probe-KairoLibrary-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1 |
| round10-cycle25-KairoLibrary-engine.json | 34 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1, no_model_approved_candidate: 33 |
| round10-cycle25-probe-KairoLibrary-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1 |
| round10-cycle26-KairoLibrary-engine.json | 33 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 2, no_model_approved_candidate: 31 |
| round10-cycle26-probe-KairoLibrary-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1 |
| round10-cycle27-KairoLibrary-engine.json | 31 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 2, no_model_approved_candidate: 29 |
| round10-cycle27-probe-KairoLibrary-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1 |
| round10-cycle28-KairoLibrary-engine.json | 29 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 3, no_model_approved_candidate: 26 |
| round10-cycle28-probe-KairoLibrary-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1 |
| round10-cycle29-KairoLibrary-engine.json | 26 | 0 | 0 | 0 | 0 | 0 | 1 | 0 | body_equal: 1, no_model_approved_candidate: 25 |
| round10-cycle29-probe-KairoLibrary-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 1 | 0 | body_equal: 1 |
| round10-cycle30-Assembly-CSharp-engine.json | 73 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 3, no_model_approved_candidate: 70 |
| round10-cycle30-probe-Assembly-CSharp-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1 |
| round10-cycle31-Assembly-CSharp-engine.json | 70 | 0 | 0 | 0 | 0 | 0 | 1 | 0 | no_model_approved_candidate: 69, no_transformation_advances: 1 |
| round10-cycle31-probe-Assembly-CSharp-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 1 | 0 | no_transformation_advances: 1 |
| round10-cycle32-Assembly-CSharp-engine.json | 70 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1, no_model_approved_candidate: 69 |
| round10-cycle32-probe-Assembly-CSharp-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1 |
| round10-cycle33-Assembly-CSharp-engine.json | 69 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 3, no_model_approved_candidate: 66 |
| round10-cycle33-probe-Assembly-CSharp-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1 |
| round10-cycle34-KairoLibrary-engine.json | 25 | 0 | 0 | 0 | 0 | 0 | 1 | 0 | body_equal: 1, no_model_approved_candidate: 23, no_transformation_advances: 1 |
| round10-cycle34-probe-KairoLibrary-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1 |
| round10-cycle35-Assembly-CSharp-engine.json | 66 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1, no_model_approved_candidate: 65 |
| round10-cycle35-probe-Assembly-CSharp-engine.json | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | body_equal: 1 |

| Execução | Variantes incompletas compiladas | Invocações destas variantes | Avanços aceitos | Métodos no limite |
| --- | --- | --- | --- | --- |
| round10-c01-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-c01-probe-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle01-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle01-probe-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle02-Assembly-CSharp-engine.json | 3 | 5 | 3 | 0 |
| round10-cycle02-probe-Assembly-CSharp-engine.json | 1 | 1 | 1 | 0 |
| round10-cycle03-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle03-probe-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle04-Assembly-CSharp-engine.json | 1 | 2 | 1 | 0 |
| round10-cycle04-probe-Assembly-CSharp-engine.json | 1 | 2 | 1 | 0 |
| round10-cycle05-KairoLibrary-engine.json | 19 | 21 | 2 | 1 |
| round10-cycle05-probe-KairoLibrary-engine.json | 12 | 13 | 1 | 1 |
| round10-cycle06-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle06-probe-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle07-KairoLibrary-engine.json | 1 | 2 | 1 | 0 |
| round10-cycle07-probe-KairoLibrary-engine.json | 1 | 2 | 1 | 0 |
| round10-cycle08-Assembly-CSharp-engine.json | 1 | 2 | 1 | 0 |
| round10-cycle08-probe-Assembly-CSharp-engine.json | 1 | 2 | 1 | 0 |
| round10-cycle09-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle09-probe-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle10-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle10-probe-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle11-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle11-probe-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle12-KairoLibrary-engine.json | 1 | 2 | 1 | 0 |
| round10-cycle12-probe-KairoLibrary-engine.json | 1 | 2 | 1 | 0 |
| round10-cycle13-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle13-probe-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle14-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle14-probe-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle15-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle15-probe-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle16-KairoLibrary-engine.json | 1 | 2 | 1 | 0 |
| round10-cycle16-probe-KairoLibrary-engine.json | 1 | 2 | 1 | 0 |
| round10-cycle17-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle17-probe-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle18-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle18-probe-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle19-KairoLibrary-engine.json | 1 | 2 | 1 | 0 |
| round10-cycle19-probe-KairoLibrary-engine.json | 1 | 2 | 1 | 0 |
| round10-cycle20-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle20-probe-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle21-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle21-probe-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle22-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle22-probe-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle23-KairoLibrary-engine.json | 2 | 4 | 2 | 0 |
| round10-cycle23-probe-KairoLibrary-engine.json | 1 | 2 | 1 | 0 |
| round10-cycle24-KairoLibrary-engine.json | 1 | 2 | 1 | 0 |
| round10-cycle24-probe-KairoLibrary-engine.json | 1 | 2 | 1 | 0 |
| round10-cycle25-KairoLibrary-engine.json | 1 | 2 | 1 | 0 |
| round10-cycle25-probe-KairoLibrary-engine.json | 1 | 2 | 1 | 0 |
| round10-cycle26-KairoLibrary-engine.json | 1 | 2 | 1 | 0 |
| round10-cycle26-probe-KairoLibrary-engine.json | 1 | 2 | 1 | 0 |
| round10-cycle27-KairoLibrary-engine.json | 2 | 4 | 2 | 0 |
| round10-cycle27-probe-KairoLibrary-engine.json | 1 | 2 | 1 | 0 |
| round10-cycle28-KairoLibrary-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle28-probe-KairoLibrary-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle29-KairoLibrary-engine.json | 3 | 5 | 2 | 0 |
| round10-cycle29-probe-KairoLibrary-engine.json | 3 | 5 | 2 | 0 |
| round10-cycle30-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle30-probe-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle31-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle31-probe-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle32-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle32-probe-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle33-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle33-probe-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle34-KairoLibrary-engine.json | 6 | 8 | 2 | 0 |
| round10-cycle34-probe-KairoLibrary-engine.json | 1 | 2 | 1 | 0 |
| round10-cycle35-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |
| round10-cycle35-probe-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 |

## 3. HIPÓTESES

| Estado | Hipótese | Evidência medida |
| --- | --- | --- |
| CONFIRMADA | A família c10-control-tree produz ao menos um novo corpo integral. | c01: 1 matched, 717 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos. Proposta investigada: Reemitir controle completo com folhas existentes e slots calculados remove o case25 vazio ausente no dispatch original |
| CONFIRMADA | A família d10-exact-setwin-chara-training produz ao menos um novo corpo integral. | cycle01: 1 matched, 412 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos. Proposta investigada: Ramo mode2 com saida comum e cadeia direta 0/1/3 remove cache do switch e preserva cinco slots |
| CONFIRMADA | A família static-initializer-ternary-polarity produz ao menos um novo corpo integral. | cycle02: 1 matched, 1816 IL bytes; Novos corpos aprovados; 3 tentativas da regra, 3 avancos. Proposta investigada: Inverter o ternario do campo estatico com vinculo AST e stsfld preserva o cctor implicito e reproduz a polaridade original |
| INCONCLUSIVA | O candidato parcial de cycle02 reproduz integralmente cfg.MyConfig::.cctor()->System.Void. | 2 avanços aceitos, mas corpo integral não igual no fim desse ciclo; parada no_model_approved_candidate. Ganhos de outros métodos da família não confirmam este candidato. |
| CONFIRMADA | A família d10-subform-draw4-composition produz ao menos um novo corpo integral. | cycle03: 1 matched, 12484 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos. Proposta investigada: Compor nove ajustes de controle e escopos preserva slots43-49 e91-92, calcula cadeia67-69 e remove o cache de switch93 |
| CONFIRMADA | A família root-guard-positive-tail produz ao menos um novo corpo integral. | cycle04: 1 matched, 347 IL bytes; Novos corpos aprovados; 11 tentativas da regra, 1 avancos. Proposta investigada: Guarda positiva no construtor com escopo e atribuicao composta originais |
| CONFIRMADA | A família root-guard-positive-tail produz ao menos um novo corpo integral. | cycle05: 2 matched, 600 IL bytes; Novos corpos aprovados; 13 tentativas da regra, 3 avancos. Proposta investigada: Separar a guarda invalidTouch e manter corpo positivo com escopos e stores originais em Canvas.OnInputUpdate |
| INCONCLUSIVA | O candidato parcial de cycle05 reproduz integralmente kairo.unity.ui.Canvas::OnInputUpdate()->System.Void. | 1 avanços aceitos, mas corpo integral não igual no fim desse ciclo; parada incomplete_candidate_budget. Ganhos de outros métodos da família não confirmam este candidato. |
| CONFIRMADA | A família d10-integer-empty-arm-composition produz ao menos um novo corpo integral. | cycle06: 6 matched, 3458 IL bytes; Novos corpos aprovados; 6 tentativas da regra, 6 avancos. Proposta investigada: Restaurar por composicao os ramos vazios terminais, OR separado e cauda positiva nos seletores Int32 |
| CONFIRMADA | A família d10-roslyn-composition-http-zero-guard-dense-default-bucket produz ao menos um novo corpo integral. | cycle07: 1 matched, 1054 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos. Proposta investigada: Separar licenca zero e restaurar switch denso com bucket default nas entradas um a tres |
| CONFIRMADA | A família d10-static-bool-composition produz ao menos um novo corpo integral. | cycle08: 1 matched, 341 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos. Proposta investigada: Restaurar os cinco inicializadores Boolean como ternarios orientados pelo IL, inclusive DEBUG com dois ramos false |
| CONFIRMADA | A família d10-smcs-bool-value produz ao menos um novo corpo integral. | cycle09: 2 matched, 6470 IL bytes; Novos corpos aprovados; 2 tentativas da regra, 2 avancos. Proposta investigada: Eliminar alias Boolean sintetico e restaurar guardas continue e ramos vazios originais como composicao por metodo |
| CONFIRMADA | A família d10-smcs-unused-slot produz ao menos um novo corpo integral. | cycle10: 3 matched, 2417 IL bytes; Novos corpos aprovados; 3 tentativas da regra, 3 avancos. Proposta investigada: Calcular locais sem uso e reposicionar declaracoes e escopos para obter os slots originais, mantendo expressoes float |
| CONFIRMADA | A família d10-smcs-named-selector produz ao menos um novo corpo integral. | cycle11: 2 matched, 4892 IL bytes; Novos corpos aprovados; 2 tentativas da regra, 2 avancos. Proposta investigada: Declarar seletor antes dos corpos, substituir dispatches por ifs e compor alias e cadeia de dez zeros, preservando saida protegida |
| CONFIRMADA | A família d10-canvas-two-residual-composition produz ao menos um novo corpo integral. | cycle12: 1 matched, 1921 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos. Proposta investigada: Restaurar avaliacao do indexador antes de newarr e dup com stloc dentro do setter, mais a guarda interna list3 positiva no mesmo escopo |
| CONFIRMADA | A família d10-smcs-tplayer produz ao menos um novo corpo integral. | cycle13: 1 matched, 9408 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos. Proposta investigada: Compor seis dispatches, guardas positivas20 e30, duas Single sem uso, neg, continues, stores por braco e saida protegida conforme90slots |
| CONFIRMADA | A família d10-smcs-subform-update produz ao menos um novo corpo integral. | cycle14: 1 matched, 11390 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos. Proposta investigada: Compor layout107, oito dispatches, Join17, unused65, cinco stores por braco, dialog99 e seis controles restantes sem mudar aritmetica |
| CONFIRMADA | A família d10-smcs-switch-join produz ao menos um novo corpo integral. | cycle15: 4 matched, 8527 IL bytes; Novos corpos aprovados; 4 tentativas da regra, 4 avancos. Proposta investigada: Restaurar seletores nomeados e joins locais dos quatro dispatches, compondo aliases GetMatchInfoJoin e guardas e bracos vazios especificos |
| CONFIRMADA | A família d10-large-lambda-composition-seb-root-tail-dispatch-record produz ao menos um novo corpo integral. | cycle16: 1 matched, 2403 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos. Proposta investigada: Calcular escopos do try e cauda, ordem de locais, dois dispatches, shortarray materializado antes dos stores e expansao de atribuicoes em array |
| CONFIRMADA | A família d10-smcs-subform-update2 produz ao menos um novo corpo integral. | cycle17: 1 matched, 12082 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos. Proposta investigada: Compor layout103, onze caudas positivas, bracos vazios48/2 e guarda event189 com Join101 nomeado; fixtures e NoEmit exatos, corpo aguarda oraculo. |
| CONFIRMADA | A família d10-smcs-userdata-draw produz ao menos um novo corpo integral. | cycle18: 1 matched, 7669 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos. Proposta investigada: Compor basex compartilhado, cadeia de quatro zeros, Boolean44 e atribuicoes por braco para strings;46 locais NoEmit exatos. |
| CONFIRMADA | A família d10-large-lambda-composition-resource-lambda-inline-dispatch produz ao menos um novo corpo integral. | cycle19: 1 matched, 1928 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos. Proposta investigada: Restaurar stores por braco no dispatch da lambda LoadStart sem escrever classe gerada, preservando as tres regioes catch e o layout original. |
| CONFIRMADA | A família d10-smcs-branch-joins produz ao menos um novo corpo integral. | cycle20: 5 matched, 5339 IL bytes; Novos corpos aprovados; 5 tentativas da regra, 5 avancos. Proposta investigada: Restaurar joins e donos lexicais de cinco controles compostos, preservando locais, folhas e transferencias comprovadas pelas fixtures. |
| CONFIRMADA | A família d10-smcs-deleted-stores produz ao menos um novo corpo integral. | cycle21: 5 matched, 4627 IL bytes; Novos corpos aprovados; 5 tentativas da regra, 5 avancos. Proposta investigada: Compor stores locais por braco e restaurar apenas o cache externo original de AddTouch; preservar nomes, expressoes float e negativo historico. |
| CONFIRMADA | A família d10-smcs-guard-slot produz ao menos um novo corpo integral. | cycle22: 3 matched, 1592 IL bytes; Novos corpos aprovados; 3 tentativas da regra, 3 avancos. Proposta investigada: Testar composicao congelada com IDs originais, fixtures proprias e gate fresco; sem ganho presumido. |
| CONFIRMADA | A família d10-explicit-tree-locals-and-index-owners produz ao menos um novo corpo integral. | cycle23: 3 matched, 1024 IL bytes; Novos corpos aprovados; 3 tentativas da regra, 3 avancos. Proposta investigada: Restaurar Nodes nomeados e resultado apos finally, reusar num de readBit e declarar tres indices Matrix no pai de uma vez. |
| CONFIRMADA | A família d10-large-lambda-composition-http-lambda-declaration-owner produz ao menos um novo corpo integral. | cycle24: 1 matched, 944 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos. Proposta investigada: Restaurar dono lexical dos locais da lambda HttpObject, escrevendo apenas o fonte pai e mantendo todas as folhas. |
| CONFIRMADA | A família d10-roslyn-composition-language-body-local-stack-candidate produz ao menos um novo corpo integral. | cycle25: 1 matched, 893 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos. Proposta investigada: Restaurar local do corpo e valores em pilha de MakeLangPackTemplate, preservando referencias e ordem de folhas. |
| CONFIRMADA | A família d10-late-tail-scope-comparison produz ao menos um novo corpo integral. | cycle26: 2 matched, 770 IL bytes; Novos corpos aprovados; 2 tentativas da regra, 2 avancos. Proposta investigada: Restaurar escopo tardio da cauda e alias em pilha nas duas comparacoes, mantendo os locais nomeados originais. |
| CONFIRMADA | A família d10-localize-shared-boolean-cache produz ao menos um novo corpo integral. | cycle27: 2 matched, 737 IL bytes; Novos corpos aprovados; 2 tentativas da regra, 2 avancos. Proposta investigada: Recuperar Boolean compartilhado do cache de localizacao entre os bracos de lock e unlock, sem promover modelos incompletos. |
| CONFIRMADA | A família d10-roslyn-bound-first-body-break produz ao menos um novo corpo integral. | cycle28: 3 matched, 614 IL bytes; Novos corpos aprovados; 3 tentativas da regra, 3 avancos. Proposta investigada: Restaurar primeira guarda com break no corpo dos lacos, preservando proprietario do incremento e escopos. |
| CONFIRMADA | A família d10-roslyn-composition-material-tail-child-scope produz ao menos um novo corpo integral. | cycle29: 1 matched, 416 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos. Proposta investigada: Restaurar escopo filho da cauda MaterialManager para a alocacao original de locais e liberacao. |
| CONFIRMADA | A família d10-residual-guard-dispatch produz ao menos um novo corpo integral. | cycle30: 3 matched, 1148 IL bytes; Novos corpos aprovados; 3 tentativas da regra, 3 avancos. Proposta investigada: Separar continue misto de cauda positiva nos dois Player e restaurar switch de um case no Touch, com igualdade completa em fixtures e slots5/15/3. |
| INCONCLUSIVA | A família d10-residual-collision produz ao menos um novo corpo integral. | cycle31: 0 matched, 0 IL bytes; Avanco parcial; nenhum novo matched; 1 tentativas da regra, 1 avancos. Proposta investigada: Restore four owned continue guards, direct num3 dispatches and per-arm angle stores; remove two switch caches without changing floating comparisons |
| INCONCLUSIVA | O candidato parcial de cycle31 reproduz integralmente game.GameManager::CollisionBall_proc()->System.Void. | 1 avanços aceitos, mas corpo integral não igual no fim desse ciclo; parada no_transformation_advances. Ganhos de outros métodos da família não confirmam este candidato. |
| CONFIRMADA | A família d10-residual-collision-default produz ao menos um novo corpo integral. | cycle32: 1 matched, 1970 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos. Proposta investigada: Restore explicit empty outer dispatch default, preserving the accepted20locals and all floating comparisons |
| CONFIRMADA | A família d10-residual-scoped-guards produz ao menos um novo corpo integral. | cycle33: 3 matched, 2307 IL bytes; Novos corpos aprovados; 3 tentativas da regra, 3 avancos. Proposta investigada: Compose owned loop guards and positive payloads, restore nested ranking break, and name the final stream Boolean in its original slot |
| CONFIRMADA | A família d10-residual-lambda-local-lifetimes produz ao menos um novo corpo integral. | cycle34: 1 matched, 684 IL bytes; Novos corpos aprovados; 2 tentativas da regra, 2 avancos. Proposta investigada: Restore file-completion Boolean across wait and callback; declare image limits in lambda parent block while preserving assignment positions |
| INCONCLUSIVA | O candidato parcial de cycle34 reproduz integralmente kairo.unity.ui.Image::<Use>b__96_0()->System.Void. | 1 avanços aceitos, mas corpo integral não igual no fim desse ciclo; parada no_transformation_advances. Ganhos de outros métodos da família não confirmam este candidato. |
| CONFIRMADA | A família d10-residual-draw-composition produz ao menos um novo corpo integral. | cycle35: 1 matched, 2309 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos. Proposta investigada: Compose state-local interval guards, per-arm English stores, state10 sibling arm, direct card dispatch and missing named frame21 |
| CONFIRMADA | As DLLs recompiladas da baseline aecb02b executam o cenário funcional observado no aparelho Android. | round10-functional-test.json: abertura, menu e partida completa Sun Rays 1-0 Blue Stars; 37 capturas, 677 linhas distintas, zero exceções Mono/Unity observadas. Não prova todos os caminhos nem as alterações C/D posteriores. |
| CONFIRMADA | Com os caminhos e opções atuais, igualar todos os corpos da KairoLibrary não elimina a diferença do caminho CodeView/PDB. | round10-KairoLibrary-raw-measurement.json: 97 bytes do caminho PDB diferentes após excluir analiticamente os quatro campos comprovados. Nenhum arquivo foi normalizado. |
| REFUTADA | O primeiro molde ampliado de EH já era correto em todos os casos classificados como completos. | Validação inicial da etapa C2 encontrou 14 planos completos incorretos no posicionamento de labels e início de EH. A versão foi recusada; as quatro etapas finais só aceitam planos e árvores completos sem discrepância no corpus matched. |
| REFUTADA | O IL sozinho sempre identifica se um salto terminal de switch era break de switch ou continue de laço no fonte. | Fixtures da etapa C4 emitem os mesmos bytes para formas fonte distintas; o owner fica INCOMPLETE quando as evidências não distinguem as construções. |
| REFUTADA | A composição de polaridade já ensaiada em SubForm.Init2 melhora o prefixo idêntico. | Auditoria congelada da família smcs-empty-arms identificou o mesmo fonte candidato de rodada anterior: duas compilações históricas reduziram o prefixo de 43 para 9 bytes. Candidato excluído, sem repetir o teste nesta rodada. |

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

Limitações dos modelos; motivos não exclusivos, sem crédito por previsões vazias:

| Assembly | Motivo agrupado | Ocorrências |
| --- | --- | --- |
| Assembly-CSharp | generated_or_unmapped_source_method | 82 |
| Assembly-CSharp | semantic_errors | 1 |
| KairoLibrary | stack_scheduler_multiple_use | 530 |
| KairoLibrary | generated_or_unmapped_source_method | 334 |
| KairoLibrary | anonymous_function_or_capture | 131 |
| KairoLibrary | exception_lowering | 103 |
| KairoLibrary | stack_scheduler_single_use | 72 |
| KairoLibrary | foreach_iteration_stack_scheduler | 46 |
| KairoLibrary | string_concat_value_temporary | 33 |
| KairoLibrary | conditional_access_temporary | 27 |
| KairoLibrary | foreach_collection_local_stack_scheduler | 25 |
| KairoLibrary | rvalue_value_type_member_temporary | 18 |
| KairoLibrary | finally_written_external_local_scheduler | 17 |
| KairoLibrary | lock_lowering | 13 |
| KairoLibrary | array_postfix_temporary | 12 |
| KairoLibrary | default_expression_temporary | 10 |
| KairoLibrary | switch_lowering | 10 |
| KairoLibrary | foreach_protected_return | 5 |
| KairoLibrary | object_or_collection_initializer | 5 |
| KairoLibrary | using_lowering | 4 |
| KairoLibrary | ref_assignment_value_temporary | 4 |
| KairoLibrary | iterator_state_machine | 2 |
| KairoLibrary | foreach_runtime_dispose | 2 |

| Modelo em uso | Fonte | Client | Solver | Validação final | Corpus final |
| --- | --- | --- | --- | --- | --- |
| KairoLibrary | tools/Round8RoslynModel.cs | tools/round8_roslyn_model.py | tools/round7_roslyn_solver.py | reports/round10-final-roslyn-model-validation.json | build/round10/final-corpus/KairoLibrary |
| Assembly-CSharp | tools/Round7SlotModel.cs | tools/round7_slot_model.py | tools/round7_slot_solver.py | reports/round10-final-smcs-model-validation.json | build/round10/final-corpus/Assembly-CSharp |

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

| Assembly | ID | Bytes |
| --- | --- | --- |
| KairoLibrary | kairo.unity.system.KairoBase/<>c::.cctor()->System.Void | 11 |
| KairoLibrary | kairo.unity.system.KairoBase/<>c::.ctor()->System.Void | 7 |
| Assembly-CSharp | <PrivateImplementationDetails>::.ctor()->System.Void | 7 |

Peso de um par = soma do tamanho integral dos métodos nele; não representa quantidade de bytes diferentes. Alinhamento e rótulos servem ao diagnóstico, sem normalizar o verificador. Seguem os cinco pares de opcodes mais pesados de cada compilador, com IL original e recompilado lado a lado.

| KairoLibrary — par | Métodos | Peso IL |
| --- | --- | --- |
| `brfalse.s => brfalse` | 1 | 680 |
| `ldc.i4.1 => DELETION_POINT` | 1 | 661 |
| `brfalse.s => brtrue` | 1 | 537 |
| `INSERTION_POINT => ldc.i4.0` | 1 | 467 |
| `leave => br` | 1 | 383 |
| `bne.un.s => beq` | 1 | 372 |
| `stloc.1 => stloc.3` | 1 | 310 |
| `brtrue.s => brfalse.s` | 1 | 303 |
| `brfalse.s => brtrue.s` | 1 | 297 |
| `ldloc.1 => ldelema` | 1 | 275 |
| `INSERTION_POINT => stloc.2` | 2 | 251 |
| `ble.s => ble.s` | 1 | 215 |
| `stloc.0 => dup` | 1 | 199 |
| `INSERTION_POINT => dup` | 1 | 197 |
| `ldelem.any => ldelema` | 1 | 192 |
| `bne.un.s => beq.s` | 1 | 182 |
| `leave.s => br.s` | 1 | 164 |
| `stloc.0 => stloc.1` | 1 | 140 |
| `constrained. => call` | 1 | 128 |
| `INSERTION_POINT => stloc.1` | 1 | 105 |
| `stloc.0 => stloc.3` | 1 | 94 |
| `ldloc.0 => DELETION_POINT` | 1 | 67 |
| `stloc.2 => stloc.0` | 1 | 46 |

**KairoLibrary #1: `brfalse.s => brfalse` — 1 métodos, 680 bytes.** Exemplo: `kairo.unity.ui.Image::<Use>b__96_0()->System.Void`.

Tentativas compiladas no grupo na rodada 10: condition-local: 1, d10-residual-lambda-local-lifetimes-use: 1, root-guard-positive-tail: 4, ternary-polarity: 1; 7 com previsão incompleta. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 24. Paradas: no_model_approved_candidate: 11, no_transformation_advances: 2.

| IL original | IL recompilado |
| --- | --- |
| `IL_0082: brtrue.s L5` | `IL_0082: brtrue.s L5` |
| `IL_0084: ldsfld class kairo.common.cfg.Config::PRINT:valuetype System.Boolean` | `IL_0084: ldsfld class kairo.common.cfg.Config::PRINT:valuetype System.Boolean` |
| `IL_0089: brfalse.s L4` | `IL_0089: brfalse L20` |
| `IL_008b: ldstr "Image.use() 失敗 ["` | `IL_008e: ldstr "Image.use() 失敗 ["` |
| `IL_0090: ldarg.0` | `IL_0093: ldarg.0` |

EH original: `[{"kind": "Catch", "try_start": 203, "try_end": 219, "handler_start": 219, "handler_end": 232, "filter_start": -1, "catch_type": "class System.OutOfMemoryException @ mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"}]`; recompilado: `[{"kind": "Catch", "try_start": 206, "try_end": 222, "handler_start": 222, "handler_end": 235, "filter_start": -1, "catch_type": "class System.OutOfMemoryException @ mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"}]`.

**KairoLibrary #2: `ldc.i4.1 => DELETION_POINT` — 1 métodos, 661 bytes.** Exemplo: `kairo.unity.ui.Seb::Draw(kairo.unity.ui.Graphics,System.Single,System.Single,kairo.unity.ui.Image[],System.Int32[])->System.Void`.

Tentativas compiladas no grupo na rodada 10: root-guard-positive-tail: 1; 1 com previsão incompleta. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 12. Paradas: no_model_approved_candidate: 12, no_transformation_advances: 1.

| IL original | IL recompilado |
| --- | --- |
| `IL_0107: ldelem.i4` | `IL_0107: ldelem.i4` |
| `IL_0108: stloc.s 7` | `IL_0108: stloc.s 7` |
| `IL_010a: ldc.i4.1` | `IL_010a: ldarg.s 4` |
| `IL_010b: brfalse L12` | `IL_010c: ldc.i4.8` |
| `IL_0110: ldarg.s 4` | `IL_010d: ldelem.i4` |
| `IL_0112: ldc.i4.8` |  |

**KairoLibrary #3: `brfalse.s => brtrue` — 1 métodos, 537 bytes.** Exemplo: `kairo.unity.ui.SaveRecorder::AutoSaveRun()->System.Void`.

Tentativas compiladas no grupo na rodada 10: nenhum. Histórico 7/8/9 compatível: UNKNOWN; não se presume compatibilidade após alteração do fonte. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 65. Paradas: no_model_approved_candidate: 13.

| IL original | IL recompilado |
| --- | --- |
| `IL_002b: ldarg.0` | `IL_002b: ldarg.0` |
| `IL_002c: ldfld class kairo.unity.ui.SaveRecorder::asCancel_:valuetype System.Boolean` | `IL_002c: ldfld class kairo.unity.ui.SaveRecorder::asCancel_:valuetype System.Boolean` |
| `IL_0031: brfalse.s L2` | `IL_0031: brtrue L11` |
| `IL_0033: leave L14` | `IL_0036: ldarg.0` |
| `IL_0038: ldarg.0` | `IL_0037: ldfld class kairo.unity.ui.SaveRecorder::asRedo_:valuetype System.Boolean` |
| `IL_0039: ldfld class kairo.unity.ui.SaveRecorder::asRedo_:valuetype System.Boolean` |  |

EH original: `[{"kind": "Catch", "try_start": 11, "try_end": 481, "handler_start": 481, "handler_end": 521, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"}, {"kind": "Finally", "try_start": 11, "try_end": 521, "handler_start": 521, "handler_end": 536, "filter_start": -1, "catch_type": null}]`; recompilado: `[{"kind": "Catch", "try_start": 11, "try_end": 475, "handler_start": 475, "handler_end": 515, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"}, {"kind": "Finally", "try_start": 11, "try_end": 515, "handler_start": 515, "handler_end": 530, "filter_start": -1, "catch_type": null}]`.

**KairoLibrary #4: `INSERTION_POINT => ldc.i4.0` — 1 métodos, 467 bytes.** Exemplo: `kairo.unity.graphics.ShaderBuilder::Init(kairo.unity.ui.Graphics,kairo.unity.ui.Image,UnityEngine.Color[],System.Single,System.Boolean,System.Int32,System.Boolean,System.Boolean,System.Boolean)->System.Void`.

Tentativas compiladas no grupo na rodada 10: nenhum. Histórico 7/8/9 compatível: UNKNOWN; não se presume compatibilidade após alteração do fonte. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 117. Paradas: no_model_approved_candidate: 13.

| IL original | IL recompilado |
| --- | --- |
| `IL_00f6: ldarg.3` | `IL_00f6: ldarg.3` |
| `IL_00f7: brfalse.s L11` | `IL_00f7: brfalse.s L11` |
| `IL_00f9: ldarg.3` | `IL_00f9: ldc.i4.0` |
| `IL_00fa: stloc.0` | `IL_00fa: stloc.0` |
| `IL_00fb: ldc.i4.0` | `IL_00fb: br.s L10` |
|  | `IL_00fd: ldarg.3` |
|  | `IL_00fe: ldloc.0` |

**KairoLibrary #5: `leave => br` — 1 métodos, 383 bytes.** Exemplo: `kairo.unity.ui.SoundOGG/<>c__DisplayClass4_0::<Play>b__0()->System.Void`.

Tentativas compiladas no grupo na rodada 10: nenhum. Histórico 7/8/9 compatível: UNKNOWN; não se presume compatibilidade após alteração do fonte. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 117. Paradas: no_model_approved_candidate: 13.

| IL original | IL recompilado |
| --- | --- |
| `IL_005b: ldc.r4 00-00-00-00` | `IL_005b: ldc.r4 00-00-00-00` |
| ```IL_0060: callvirt class UnityEngine.AudioSource::set_time``0(valuetype System.Single)->class System.Void;this=True;explicit=False;cc=Default``` | ```IL_0060: callvirt class UnityEngine.AudioSource::set_time``0(valuetype System.Single)->class System.Void;this=True;explicit=False;cc=Default``` |
| `IL_0065: leave L9` | `IL_0065: br L7` |
| `IL_006a: ldarg.0` | `IL_006a: ldarg.0` |
| `IL_006b: ldfld class kairo.unity.ui.SoundOGG/<>c__DisplayClass4_0::<>4__this:class kairo.unity.ui.SoundOGG` | `IL_006b: ldfld class kairo.unity.ui.SoundOGG/<>c__DisplayClass4_0::<>4__this:class kairo.unity.ui.SoundOGG` |

EH original: `[{"kind": "Catch", "try_start": 6, "try_end": 354, "handler_start": 354, "handler_end": 382, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"}]`; recompilado: `[{"kind": "Catch", "try_start": 6, "try_end": 347, "handler_start": 347, "handler_end": 375, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"}]`.

| Assembly-CSharp — par | Métodos | Peso IL |
| --- | --- | --- |
| `stloc.s => stloc.s` | 3 | 4287 |
| `bne.un => beq` | 1 | 3844 |
| `stloc.2 => stloc.0` | 2 | 3787 |
| `stloc.s => dup` | 1 | 3287 |
| `ldc.i4.0 => DELETION_POINT` | 2 | 2997 |
| `stloc.3 => stloc.1` | 2 | 2955 |
| `INSERTION_POINT => stloc.s` | 2 | 2565 |
| `ret => br` | 1 | 2454 |
| `brtrue => brtrue` | 1 | 2145 |
| `stloc.s => stloc.2` | 2 | 1874 |
| `stloc.s => stloc.0` | 2 | 1800 |
| `dup => DELETION_POINT` | 2 | 1715 |
| `ldc.i4.5 => DELETION_POINT` | 1 | 1599 |
| `brfalse => DELETION_POINT` | 1 | 1342 |
| `bne.un => bne.un` | 1 | 1308 |
| `stloc.2 => stloc.s` | 3 | 1263 |
| `stloc.s => stloc.1` | 1 | 1029 |
| `br => DELETION_POINT` | 6 | 878 |
| `INSERTION_POINT => ldc.r4` | 1 | 857 |
| `stloc.3 => dup` | 1 | 758 |
| `brtrue => DELETION_POINT` | 1 | 723 |
| `INSERTION_POINT => ldc.i4.s` | 1 | 707 |
| `INSERTION_POINT => ldloc.2` | 1 | 691 |
| `INSERTION_POINT => ldloc.s` | 1 | 653 |
| `ldc.i4.0 => stloc.s` | 1 | 623 |
| `blt.un => bge` | 1 | 594 |
| `bge.un => blt` | 1 | 562 |
| `ldc.i4.1 => switch` | 2 | 556 |
| `ldarg.s => DELETION_POINT` | 1 | 506 |
| `brtrue => stloc.1` | 1 | 447 |
| `ldarg.1 => ldloc.s` | 1 | 432 |
| `ldarg.2 => DELETION_POINT` | 1 | 343 |
| `INSERTION_POINT => stloc.3` | 1 | 325 |
| `switch => ldc.i4.1` | 1 | 322 |
| `ldc.i4.1 => ldc.i4.4` | 1 | 272 |
| `brtrue => brfalse` | 2 | 266 |
| `blt => blt` | 1 | 253 |
| `INSERTION_POINT => ldarg.0` | 1 | 241 |
| `INSERTION_POINT => stloc.0` | 1 | 196 |
| `beq => bne.un` | 1 | 190 |
| `brfalse => brfalse` | 1 | 166 |
| `ldarg.3 => DELETION_POINT` | 1 | 110 |
| `stloc.0 => DELETION_POINT` | 2 | 109 |
| `INSERTION_POINT => ldloc.1` | 1 | 106 |
| `stloc.0 => stloc.s` | 1 | 99 |
| `brtrue => stloc.0` | 1 | 38 |

**Assembly-CSharp #1: `stloc.s => stloc.s` — 3 métodos, 4287 bytes.** Exemplo: `main.AppData::SendSaveData(System.String,System.Int32,System.Exception)->System.Void`.

Tentativas compiladas no grupo na rodada 10: nenhum. Histórico 7/8/9 compatível: UNKNOWN; não se presume compatibilidade após alteração do fonte. Antes de compilar: wrong_signature: 2; orçamento recusou 0; repertório anterior filtrou 2632. Paradas: no_model_approved_candidate: 66.

| IL original | IL recompilado |
| --- | --- |
| `IL_025e: stloc.s 11` | `IL_025e: stloc.s 11` |
| `IL_0260: ldloc.s 10` | `IL_0260: ldloc.s 10` |
| `IL_0262: stloc.s 41` | `IL_0262: stloc.s 13` |
| `IL_0264: ldc.i4.5` | `IL_0264: ldc.i4.5` |
| `IL_0265: newarr class System.Object` | `IL_0265: newarr class System.Object` |

EH original: `[{"kind": "Catch", "try_start": 2147, "try_end": 2160, "handler_start": 2160, "handler_end": 2166, "filter_start": -1, "catch_type": "class System.Threading.ThreadAbortException @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Catch", "try_start": 1927, "try_end": 1940, "handler_start": 1940, "handler_end": 1946, "filter_start": -1, "catch_type": "class System.Threading.ThreadAbortException @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Catch", "try_start": 1829, "try_end": 1864, "handler_start": 1864, "handler_end": 1986, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Catch", "try_start": 1464, "try_end": 1540, "handler_start": 1540, "handler_end": 1546, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Catch", "try_start": 1281, "try_end": 1316, "handler_start": 1316, "handler_end": 1325, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Catch", "try_start": 1173, "try_end": 1208, "handler_start": 1208, "handler_end": 1265, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Catch", "try_start": 910, "try_end": 2084, "handler_start": 2084, "handler_end": 2201, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Catch", "try_start": 868, "try_end": 881, "handler_start": 881, "handler_end": 887, "filter_start": -1, "catch_type": "class System.Threading.ThreadAbortException @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}]`; recompilado: `[{"kind": "Catch", "try_start": 2199, "try_end": 2212, "handler_start": 2212, "handler_end": 2218, "filter_start": -1, "catch_type": "class System.Threading.ThreadAbortException @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Catch", "try_start": 1965, "try_end": 1978, "handler_start": 1978, "handler_end": 1984, "filter_start": -1, "catch_type": "class System.Threading.ThreadAbortException @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Catch", "try_start": 1867, "try_end": 1897, "handler_start": 1897, "handler_end": 2024, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Catch", "try_start": 1481, "try_end": 1557, "handler_start": 1557, "handler_end": 1563, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Catch", "try_start": 1298, "try_end": 1333, "handler_start": 1333, "handler_end": 1342, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Catch", "try_start": 1185, "try_end": 1220, "handler_start": 1220, "handler_end": 1277, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Catch", "try_start": 910, "try_end": 2131, "handler_start": 2131, "handler_end": 2253, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Catch", "try_start": 868, "try_end": 881, "handler_start": 881, "handler_end": 887, "filter_start": -1, "catch_type": "class System.Threading.ThreadAbortException @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}]`.

**Assembly-CSharp #2: `bne.un => beq` — 1 métodos, 3844 bytes.** Exemplo: `form.SubForm::Init2()->System.Boolean`.

Tentativas compiladas no grupo na rodada 10: nenhum. Histórico 7/8/9 compatível: UNKNOWN; não se presume compatibilidade após alteração do fonte. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 682. Paradas: no_model_approved_candidate: 22.

| IL original | IL recompilado |
| --- | --- |
| `IL_004d: ldloc.3` | `IL_004d: ldloc.3` |
| `IL_004e: ldfld class data.BaseData::id_:valuetype System.Int32` | `IL_004e: ldfld class data.BaseData::id_:valuetype System.Int32` |
| `IL_0053: bne.un L4` | `IL_0053: beq L3` |
| `IL_0058: ldc.i4.1` | `IL_0058: br L4` |
| `IL_0059: stloc.1` | `IL_005d: ldc.i4.1` |
|  | `IL_005e: stloc.1` |

**Assembly-CSharp #3: `stloc.2 => stloc.0` — 2 métodos, 3787 bytes.** Exemplo: `form.BootForm::Load()->System.Void`.

Tentativas compiladas no grupo na rodada 10: nenhum. Histórico 7/8/9 compatível: UNKNOWN; não se presume compatibilidade após alteração do fonte. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 462. Paradas: no_model_approved_candidate: 44.

| IL original | IL recompilado |
| --- | --- |
| `IL_0045: ldc.i4.1` | `IL_0045: ldc.i4.1` |
| ```IL_0046: call class kairo.unity.io.RecordStore::OpenRecordStore``0(valuetype System.Int32)->class kairo.unity.io.RecordStore;this=False;explicit=False;cc=Default``` | ```IL_0046: call class kairo.unity.io.RecordStore::OpenRecordStore``0(valuetype System.Int32)->class kairo.unity.io.RecordStore;this=False;explicit=False;cc=Default``` |
| `IL_004b: stloc.2` | `IL_004b: stloc.0` |
| `IL_004c: ldloc.2` | `IL_004c: ldloc.0` |
| `IL_004d: ldc.i4.6` | `IL_004d: ldc.i4.6` |
| ```IL_004e: callvirt class kairo.unity.io.RecordStore::ReadRecord``0(valuetype System.Int32)->valuetype System.Byte[];this=True;explicit=False;cc=Default``` | ```IL_004e: callvirt class kairo.unity.io.RecordStore::ReadRecord``0(valuetype System.Int32)->valuetype System.Byte[];this=True;explicit=False;cc=Default``` |

EH original: `[{"kind": "Catch", "try_start": 2577, "try_end": 3155, "handler_start": 3155, "handler_end": 3192, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Finally", "try_start": 2255, "try_end": 2450, "handler_start": 2450, "handler_end": 2468, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 2037, "try_end": 2092, "handler_start": 2092, "handler_end": 2110, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1798, "try_end": 2005, "handler_start": 2005, "handler_end": 2023, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1717, "try_end": 1766, "handler_start": 1766, "handler_end": 1784, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1630, "try_end": 1685, "handler_start": 1685, "handler_end": 1703, "filter_start": -1, "catch_type": null}, {"kind": "Catch", "try_start": 69, "try_end": 2487, "handler_start": 2487, "handler_end": 2562, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Finally", "try_start": 69, "try_end": 2562, "handler_start": 2562, "handler_end": 2577, "filter_start": -1, "catch_type": null}]`; recompilado: `[{"kind": "Catch", "try_start": 2577, "try_end": 3155, "handler_start": 3155, "handler_end": 3192, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Finally", "try_start": 2255, "try_end": 2450, "handler_start": 2450, "handler_end": 2468, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 2037, "try_end": 2092, "handler_start": 2092, "handler_end": 2110, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1798, "try_end": 2005, "handler_start": 2005, "handler_end": 2023, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1717, "try_end": 1766, "handler_start": 1766, "handler_end": 1784, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1630, "try_end": 1685, "handler_start": 1685, "handler_end": 1703, "filter_start": -1, "catch_type": null}, {"kind": "Catch", "try_start": 69, "try_end": 2487, "handler_start": 2487, "handler_end": 2562, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Finally", "try_start": 69, "try_end": 2562, "handler_start": 2562, "handler_end": 2577, "filter_start": -1, "catch_type": null}]`.

**Assembly-CSharp #4: `stloc.s => dup` — 1 métodos, 3287 bytes.** Exemplo: `game.UserData::SetTrainingCommonMethod(System.Boolean)->System.Void`.

Tentativas compiladas no grupo na rodada 10: nenhum. Histórico 7/8/9 compatível: UNKNOWN; não se presume compatibilidade após alteração do fonte. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 528. Paradas: no_model_approved_candidate: 22.

| IL original | IL recompilado |
| --- | --- |
| `IL_031c: ldc.i4.2` | `IL_031c: ldc.i4.2` |
| `IL_031d: newarr class java.util.Vector` | `IL_031d: newarr class java.util.Vector` |
| `IL_0322: stloc.s 14` | `IL_0322: dup` |
| `IL_0324: ldloc.s 14` | `IL_0323: ldc.i4.0` |
| `IL_0326: ldc.i4.0` | ```IL_0324: newobj class java.util.Vector::.ctor``0()->class System.Void;this=True;explicit=False;cc=Default``` |
| ```IL_0327: newobj class java.util.Vector::.ctor``0()->class System.Void;this=True;explicit=False;cc=Default``` |  |

**Assembly-CSharp #5: `ldc.i4.0 => DELETION_POINT` — 2 métodos, 2997 bytes.** Exemplo: `game.Map::SetCoatLine()->System.Void`.

Tentativas compiladas no grupo na rodada 10: nenhum. Histórico 7/8/9 compatível: UNKNOWN; não se presume compatibilidade após alteração do fonte. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 352. Paradas: no_model_approved_candidate: 44.

| IL original | IL recompilado |
| --- | --- |
| `IL_023c: ldelem.ref` | `IL_023c: ldelem.ref` |
| `IL_023d: ldc.i4.0` | `IL_023d: ldc.i4.0` |
| `IL_023e: ldc.i4.0` | `IL_023e: ldloc.0` |
| `IL_023f: ldloc.0` | `IL_023f: ldc.i4.0` |
| `IL_0240: ldc.i4.0` | `IL_0240: ldelem.i4` |

## 5. TOOLCHAIN

KairoLibrary: NuGet Microsoft.Net.Compilers 1.1.0; csc.exe 1.1.0.51109; compilador SHA-256 `53b693536e8a86f35d05d2165ff9b3e0aeaa499ecfd8135a9f99ab4404ca86c3`. Mudou compilador/opções/seleção de referências nesta rodada: NÃO.

Opções: `/nologo /noconfig /nostdlib+ /target:library /platform:anycpu /optimize+ /debug:pdbonly /deterministic- /langversion:6 /unsafe- /checked- /codepage:65001 /utf8output`.

| Referência (ordem de compilação) | SHA-256 |
| --- | --- |
| C:\Windows\Microsoft.NET\Framework\v2.0.50727\mscorlib.dll | `c4729c6fc2279933fa9638f60775ab2f7eb731d5b04f9e04c15187afbdf94558` |
| C:\Users\l_abn\AppData\Local\Programs\Unity\5.3.4f1\Editor\Data\Mono\lib\mono\2.0\System.Core.dll | `4998eda31c3d7bebf606f98cef77ad697f9e82e629efc37c06517019c2c2c271` |
| C:\Users\l_abn\Documents\Projetos\Pocket League Story\original\extracted\assets\bin\Data\Managed\UnityEngine.dll | `7cc901371cc77e103d00b5845e8285f85fbdf36bb47ccf3d824f94c4ead32421` |
| C:\Users\l_abn\AppData\Local\Programs\Unity\5.3.4f1\Editor\Data\Mono\lib\mono\2.0\System.dll | `c2225d7ba58a12e22c266384d3c1e4e882ab4e42449b91b721c21b5f1fcd63be` |

Assembly-CSharp: smcs.exe 2.6.5.0 do Unity 5.3.4f1; compilador SHA-256 `26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0`. Mudou compilador/opções/seleção de referências nesta rodada: NÃO.

Opções: `-nologo -noconfig -target:library -platform:anycpu -optimize+ -debug- -langversion:Default -unsafe- -checked- -codepage:65001`.

A KairoLibrary referenciada é recompilada com o projeto; seu hash de artefato pode mudar, sem troca da seleção de frameworks ou do compilador.

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

A comparação abrange ordem de tipos e membros, parâmetros, atributos personalizados, layout, constantes, recursos e referências globais; o guard de identidade confere o SHA do original e do build atual. A medição bruta KairoLibrary da Parte B descreve a baseline inicial; o estado de identidade bruta do verificador acima continua separado dessa medição. Na reconstrução das DLLs, nenhum byte original é inserido no resultado, nenhuma classe gerada é escrita à mão e nenhum assembly é normalizado. O APK funcional da Parte A contém os demais arquivos originais conforme solicitado e não é a reconstrução final.

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

Nenhuma instalação, arquivo ou decisão de escopo exigida para esta rodada. Métodos restantes continuam divergentes; a parada segue o limite autorizado.

Parada após 35 ciclos autônomos D e 1 ciclo adicional de preparo registrado no diário; as partes A–C estão descritas na seção 2. Sequência autônoma final sem novo matched: 0. Limites configurados: 35 ciclos autônomos, 5 sem ganho consecutivos com fila vazia; cota Kairo: at least one KairoLibrary cycle in every consecutive block of five cycles.

## 8. SUA RECOMENDAÇÃO

1. Investigar layout e aliases de locais nos cinco métodos dos pares stloc.s => stloc.s (3 métodos / 4.287 bytes de IL) e stloc.2 => stloc.0 (2 / 3.787): potencial medido de 5 métodos / 8.074 bytes. Calcular os papéis e o primeiro e último uso dos slots para compor declarações e controle; permutações genéricas já foram ensaiadas. Ganho real UNKNOWN.
2. Concluir a lambda Image.Use (1 método / 680 bytes de IL): D34 confirmou a assinatura real de 12 locais e avanço de 88 para 131 bytes, mas o corpo não fechou. O primeiro par restante é brfalse.s => brfalse. Investigar a fronteira de controle e EH, preservando o resultado negativo integral de D34. Potencial de 680 bytes; ganho UNKNOWN.
3. Reconstituir o controle de SubForm.Init2 (1 método / 3.844 bytes de IL), último caso bne.un => beq: os 68 locais já coincidem, mas foram delimitadas 136 regiões de alinhamento. Exigir uma composição com donos de saídas e fixture, preservando os negativos de polaridade isolada. Potencial de 3.844 bytes; ganho UNKNOWN.
