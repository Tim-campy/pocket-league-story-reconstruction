## 1. PROGRESSO

APK: `net.kairosoft.android.soccer_en_2.0.2-APK_Award.apk`; reconstrução gerenciada Unity 5.3.4f1/Mono. Linha anterior: commit `cdd0f7f3b8dda09f296a7cfa16cd00236a3346ae`. `matched` exige cabeçalho, maxstack, locais, IL e regiões de exceção iguais; somente identidades de tokens são resolvidas. Declarações sem corpo entram em métodos, não em bytes de IL.

| Artefato original | SHA-256 |
| --- | --- |
| APK | `d557af6b4bc7748605a39ed3548821f6763c5773a12432a3fed6e8c413b211e8` |
| KairoLibrary.dll | `bb553d341eecd9b900f772486bf4a5c289aa2f09566d9d402a3eb70268f4b3ee` |
| Assembly-CSharp.dll | `3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949` |

| Assembly | Anterior | Atual | Métodos ganhos | Bytes ganhos | Regressões |
| --- | --- | --- | --- | --- | --- |
| KairoLibrary | 3193/3287 (97.140249%); IL 208123/269894 bytes (77.112867%) | 3207/3287 (97.566170%); IL 212900/269894 bytes (78.882821%) | 14 | 4777 | 0 |
| Assembly-CSharp | 1703/1958 (86.976507%); IL 225924/601603 bytes (37.553669%) | 1789/1958 (91.368744%); IL 352326/601603 bytes (58.564535%) | 86 | 126402 | 0 |

Total atual: 4996/5245 métodos; 565226/871497 bytes de IL.

Retiradas de estado matched desde cdd0f7f: 0. Corpos antes iguais que deixaram de bater: 0. As duas medidas são calculadas por ID, não inferidas da contagem líquida.

**Motivo da parada:** limite de vinte ciclos autônomos concluídos; o ciclo 20 ganhou um método, portanto a sequência final de ciclos sem ganho é zero.

## 2. O QUE FOI FEITO NESTA RODADA

As linhas abaixo distinguem sondas e execuções adotadas. Ganho do motor significa corpo completo aprovado; avanços de prefixo não contam como método concluído. O corpo ganho é atribuído à última transformação que o fechou; uma operação preparatória pode ter zero nessa coluna e ainda ter permitido o resultado. Métodos auxiliares liberados pelo verificador são incluídos no progresso oficial.

| Execução | Transformação | Tentativas compiladas | Avanços | Corpos ganhos | Bytes ganhos | Perdidos |
| --- | --- | --- | --- | --- | --- | --- |
| round7-cycle01-Assembly-CSharp-engine.json | condition-split-and | 3 | 0 | 0 | 0 | 0 |
| round7-cycle01-Assembly-CSharp-engine.json | extended-original-zero-chain | 1 | 1 | 0 | 0 | 0 |
| round7-cycle01-Assembly-CSharp-engine.json | if-polarity | 2 | 0 | 0 | 0 | 0 |
| round7-cycle01-Assembly-CSharp-engine.json | local-declaration-order-later | 1 | 0 | 0 | 0 | 0 |
| round7-cycle01-Assembly-CSharp-engine.json | local-declaration-scope-hoist | 2 | 0 | 0 | 0 | 0 |
| round7-cycle01-Assembly-CSharp-engine.json | solver-group-all-calculated-blocks | 2 | 0 | 0 | 0 | 0 |
| round7-cycle01-Assembly-CSharp-engine.json | solver-group-block-declarations | 10 | 0 | 0 | 0 | 0 |
| round7-cycle01-Assembly-CSharp-engine.json | terminal-guard-positive-tail | 18 | 13 | 7 | 14902 | 0 |
| round7-cycle01-probe-Assembly-CSharp-engine.json | terminal-guard-positive-tail | 1 | 1 | 1 | 4356 | 0 |
| round7-cycle02-Assembly-CSharp-engine.json | condition-split-and | 1 | 0 | 0 | 0 | 0 |
| round7-cycle02-Assembly-CSharp-engine.json | initialized-declaration-swap | 1 | 0 | 0 | 0 | 0 |
| round7-cycle02-Assembly-CSharp-engine.json | loop-continue-guards | 1 | 1 | 1 | 904 | 0 |
| round7-cycle02-Assembly-CSharp-engine.json | solver-group-all-calculated-blocks | 1 | 0 | 0 | 0 | 0 |
| round7-cycle02-Assembly-CSharp-engine.json | solver-group-block-declarations | 6 | 0 | 0 | 0 | 0 |
| round7-cycle02-Assembly-CSharp-engine.json | terminal-guard-positive-tail | 1 | 1 | 0 | 0 | 0 |
| round7-cycle02-Assembly-CSharp-engine.json | void-empty-then-common-exit | 8 | 8 | 5 | 1765 | 0 |
| round7-cycle02-probe-Assembly-CSharp-engine.json | void-empty-then-common-exit | 1 | 1 | 1 | 115 | 0 |
| round7-cycle03-Assembly-CSharp-engine.json | assignment-ternary-if | 3 | 3 | 0 | 0 | 0 |
| round7-cycle03-Assembly-CSharp-engine.json | condition-merge-or-guards | 1 | 0 | 0 | 0 | 0 |
| round7-cycle03-Assembly-CSharp-engine.json | condition-split-and | 2 | 0 | 0 | 0 | 0 |
| round7-cycle03-Assembly-CSharp-engine.json | condition-split-or-guards | 1 | 0 | 0 | 0 | 0 |
| round7-cycle03-Assembly-CSharp-engine.json | if-polarity | 4 | 1 | 0 | 0 | 0 |
| round7-cycle03-Assembly-CSharp-engine.json | loop-continue-guard | 8 | 8 | 4 | 2934 | 0 |
| round7-cycle03-Assembly-CSharp-engine.json | loop-continue-guards | 2 | 2 | 0 | 0 | 0 |
| round7-cycle03-Assembly-CSharp-engine.json | loop-guard-return-to-continue | 8 | 8 | 1 | 1674 | 0 |
| round7-cycle03-Assembly-CSharp-engine.json | solver-group-all-calculated-blocks | 15 | 0 | 0 | 0 | 0 |
| round7-cycle03-Assembly-CSharp-engine.json | solver-group-block-declarations | 31 | 0 | 0 | 0 | 0 |
| round7-cycle03-Assembly-CSharp-engine.json | terminal-guard-positive-tail | 1 | 1 | 0 | 0 | 0 |
| round7-cycle03-Assembly-CSharp-engine.json | ternary-polarity | 1 | 1 | 0 | 0 | 0 |
| round7-cycle03-probe-Assembly-CSharp-engine.json | if-polarity | 2 | 0 | 0 | 0 | 0 |
| round7-cycle03-probe-Assembly-CSharp-engine.json | loop-guard-return-to-continue | 1 | 1 | 0 | 0 | 0 |
| round7-cycle03-probe-Assembly-CSharp-engine.json | solver-group-all-calculated-blocks | 1 | 0 | 0 | 0 | 0 |
| round7-cycle03-probe-Assembly-CSharp-engine.json | solver-group-block-declarations | 7 | 0 | 0 | 0 | 0 |
| round7-cycle04-Assembly-CSharp-engine.json | empty-if-terminal-polarity | 4 | 3 | 2 | 658 | 0 |
| round7-cycle04-probe-Assembly-CSharp-engine.json | empty-if-terminal-polarity | 1 | 1 | 0 | 0 | 0 |
| round7-cycle05-Assembly-CSharp-engine.json | assignment-ternary-if | 3 | 2 | 0 | 0 | 0 |
| round7-cycle05-Assembly-CSharp-engine.json | assignment-ternary-if-reverse | 1 | 1 | 1 | 6626 | 0 |
| round7-cycle05-Assembly-CSharp-engine.json | calculated-original-local-coalescence | 3 | 3 | 0 | 0 | 0 |
| round7-cycle05-Assembly-CSharp-engine.json | if-polarity | 1 | 0 | 0 | 0 | 0 |
| round7-cycle05-Assembly-CSharp-engine.json | initialized-declaration-swap | 1 | 0 | 0 | 0 | 0 |
| round7-cycle05-Assembly-CSharp-engine.json | restore-zero-chain-local | 1 | 1 | 0 | 0 | 0 |
| round7-cycle05-Assembly-CSharp-engine.json | solver-group-all-calculated-blocks | 6 | 0 | 0 | 0 | 0 |
| round7-cycle05-Assembly-CSharp-engine.json | solver-group-block-declarations | 44 | 0 | 0 | 0 | 0 |
| round7-cycle05-Assembly-CSharp-engine.json | ternary-polarity | 2 | 2 | 0 | 0 | 0 |
| round7-cycle05-probe-Assembly-CSharp-engine.json | assignment-ternary-if | 1 | 0 | 0 | 0 | 0 |
| round7-cycle05-probe-Assembly-CSharp-engine.json | assignment-ternary-if-reverse | 1 | 1 | 1 | 6626 | 0 |
| round7-cycle05-probe-Assembly-CSharp-engine.json | calculated-original-local-coalescence | 1 | 1 | 0 | 0 | 0 |
| round7-cycle05-probe-Assembly-CSharp-engine.json | solver-group-all-calculated-blocks | 1 | 0 | 0 | 0 | 0 |
| round7-cycle05-probe-Assembly-CSharp-engine.json | solver-group-block-declarations | 1 | 0 | 0 | 0 | 0 |
| round7-cycle06-Assembly-CSharp-engine.json | guard-break-reverse | 2 | 0 | 0 | 0 | 0 |
| round7-cycle06-Assembly-CSharp-engine.json | guard-return-reverse | 2 | 0 | 0 | 0 | 0 |
| round7-cycle06-Assembly-CSharp-engine.json | if-polarity | 25 | 1 | 0 | 0 | 0 |
| round7-cycle06-Assembly-CSharp-engine.json | local-declaration-order-earlier | 11 | 0 | 0 | 0 | 0 |
| round7-cycle06-Assembly-CSharp-engine.json | local-declaration-order-later | 18 | 0 | 0 | 0 | 0 |
| round7-cycle06-Assembly-CSharp-engine.json | local-declaration-scope-hoist | 9 | 0 | 0 | 0 | 0 |
| round7-cycle06-Assembly-CSharp-engine.json | smcs-empty-arm-restore | 145 | 24 | 8 | 14824 | 0 |
| round7-cycle06-Assembly-CSharp-engine.json | smcs-empty-arm-restore-and-suffix | 18 | 1 | 0 | 0 | 0 |
| round7-cycle06-Assembly-CSharp-engine.json | smcs-empty-arm-restore-first-and | 32 | 17 | 1 | 50 | 0 |
| round7-cycle06-Assembly-CSharp-engine.json | solver-group-all-calculated-blocks | 6 | 0 | 0 | 0 | 0 |
| round7-cycle06-Assembly-CSharp-engine.json | solver-group-block-declarations | 22 | 0 | 0 | 0 | 0 |
| round7-cycle06-probe-Assembly-CSharp-engine.json | if-polarity | 1 | 0 | 0 | 0 | 0 |
| round7-cycle06-probe-Assembly-CSharp-engine.json | smcs-empty-arm-restore | 2 | 1 | 0 | 0 | 0 |
| round7-cycle06-probe2-Assembly-CSharp-engine.json | smcs-empty-arm-restore | 2 | 2 | 1 | 390 | 0 |
| round7-cycle07-Assembly-CSharp-engine.json | assignment-ternary-if | 4 | 4 | 0 | 0 | 0 |
| round7-cycle07-Assembly-CSharp-engine.json | calculated-store-anchored-declarations | 1 | 1 | 0 | 0 | 0 |
| round7-cycle07-Assembly-CSharp-engine.json | if-polarity | 2 | 0 | 0 | 0 | 0 |
| round7-cycle07-Assembly-CSharp-engine.json | loop-continue-guard | 1 | 1 | 0 | 0 | 0 |
| round7-cycle07-Assembly-CSharp-engine.json | smcs-empty-arm-restore | 6 | 1 | 0 | 0 | 0 |
| round7-cycle07-Assembly-CSharp-engine.json | solver-group-all-calculated-blocks | 7 | 0 | 0 | 0 | 0 |
| round7-cycle07-Assembly-CSharp-engine.json | solver-group-block-declarations | 29 | 0 | 0 | 0 | 0 |
| round7-cycle07-Assembly-CSharp-engine.json | ternary-polarity | 2 | 2 | 0 | 0 | 0 |
| round7-cycle07-probe-Assembly-CSharp-engine.json | assignment-ternary-if | 4 | 4 | 0 | 0 | 0 |
| round7-cycle07-probe-Assembly-CSharp-engine.json | calculated-store-anchored-declarations | 1 | 1 | 0 | 0 | 0 |
| round7-cycle07-probe-Assembly-CSharp-engine.json | if-polarity | 2 | 0 | 0 | 0 | 0 |
| round7-cycle07-probe-Assembly-CSharp-engine.json | loop-continue-guard | 1 | 1 | 0 | 0 | 0 |
| round7-cycle07-probe-Assembly-CSharp-engine.json | smcs-empty-arm-restore | 6 | 1 | 0 | 0 | 0 |
| round7-cycle07-probe-Assembly-CSharp-engine.json | solver-group-all-calculated-blocks | 7 | 0 | 0 | 0 | 0 |
| round7-cycle07-probe-Assembly-CSharp-engine.json | solver-group-block-declarations | 29 | 0 | 0 | 0 | 0 |
| round7-cycle07-probe-Assembly-CSharp-engine.json | ternary-polarity | 2 | 2 | 0 | 0 | 0 |
| round7-cycle08-Assembly-CSharp-engine.json | if-polarity | 6 | 1 | 0 | 0 | 0 |
| round7-cycle08-Assembly-CSharp-engine.json | loop-continue-guard | 5 | 2 | 0 | 0 | 0 |
| round7-cycle08-Assembly-CSharp-engine.json | smcs-empty-arm-restore | 12 | 3 | 0 | 0 | 0 |
| round7-cycle08-Assembly-CSharp-engine.json | smcs-loop-condition-body-guard | 4 | 4 | 2 | 889 | 0 |
| round7-cycle08-Assembly-CSharp-engine.json | solver-group-all-calculated-blocks | 6 | 0 | 0 | 0 | 0 |
| round7-cycle08-Assembly-CSharp-engine.json | solver-group-block-declarations | 12 | 0 | 0 | 0 | 0 |
| round7-cycle08-probe-Assembly-CSharp-engine.json | if-polarity | 1 | 1 | 0 | 0 | 0 |
| round7-cycle08-probe-Assembly-CSharp-engine.json | loop-continue-guard | 3 | 2 | 0 | 0 | 0 |
| round7-cycle08-probe-Assembly-CSharp-engine.json | smcs-empty-arm-restore | 7 | 2 | 0 | 0 | 0 |
| round7-cycle08-probe-Assembly-CSharp-engine.json | smcs-loop-condition-body-guard | 1 | 1 | 0 | 0 | 0 |
| round7-cycle08-probe-Assembly-CSharp-engine.json | solver-group-all-calculated-blocks | 6 | 0 | 0 | 0 | 0 |
| round7-cycle08-probe-Assembly-CSharp-engine.json | solver-group-block-declarations | 12 | 0 | 0 | 0 | 0 |
| round7-cycle08-probe2-Assembly-CSharp-engine.json | smcs-loop-condition-body-guard | 1 | 1 | 1 | 534 | 0 |
| round7-cycle09-Assembly-CSharp-engine.json | explicit-string-concat-preserve-literals | 2 | 2 | 0 | 0 | 0 |
| round7-cycle09-Assembly-CSharp-engine.json | smcs-empty-arm-restore | 4 | 3 | 0 | 0 | 0 |
| round7-cycle09-Assembly-CSharp-engine.json | solver-group-all-calculated-blocks | 4 | 0 | 0 | 0 | 0 |
| round7-cycle09-Assembly-CSharp-engine.json | solver-group-block-declarations | 19 | 0 | 0 | 0 | 0 |
| round7-cycle09-probe-Assembly-CSharp-engine.json | explicit-string-concat-preserve-literals | 2 | 2 | 0 | 0 | 0 |
| round7-cycle09-probe-Assembly-CSharp-engine.json | smcs-empty-arm-restore | 4 | 3 | 0 | 0 | 0 |
| round7-cycle09-probe-Assembly-CSharp-engine.json | solver-group-all-calculated-blocks | 4 | 0 | 0 | 0 | 0 |
| round7-cycle09-probe-Assembly-CSharp-engine.json | solver-group-block-declarations | 19 | 0 | 0 | 0 | 0 |
| round7-cycle10-Assembly-CSharp-engine.json | assignment-ternary-if | 3 | 2 | 0 | 0 | 0 |
| round7-cycle10-Assembly-CSharp-engine.json | assignment-ternary-if-reverse | 1 | 1 | 0 | 0 | 0 |
| round7-cycle10-Assembly-CSharp-engine.json | condition-merge-and | 1 | 0 | 0 | 0 | 0 |
| round7-cycle10-Assembly-CSharp-engine.json | guard-return-reverse | 1 | 0 | 0 | 0 | 0 |
| round7-cycle10-Assembly-CSharp-engine.json | if-polarity | 8 | 1 | 1 | 3587 | 0 |
| round7-cycle10-Assembly-CSharp-engine.json | local-declaration-order-later | 3 | 0 | 0 | 0 | 0 |
| round7-cycle10-Assembly-CSharp-engine.json | local-declaration-scope-hoist | 6 | 0 | 0 | 0 | 0 |
| round7-cycle10-Assembly-CSharp-engine.json | smcs-empty-arm-restore | 14 | 2 | 0 | 0 | 0 |
| round7-cycle10-Assembly-CSharp-engine.json | smcs-owned-switch-if | 10 | 8 | 2 | 562 | 0 |
| round7-cycle10-Assembly-CSharp-engine.json | smcs-owned-switch-if-empty-zero | 1 | 1 | 1 | 786 | 0 |
| round7-cycle10-Assembly-CSharp-engine.json | solver-group-all-calculated-blocks | 6 | 0 | 0 | 0 | 0 |
| round7-cycle10-Assembly-CSharp-engine.json | solver-group-block-declarations | 33 | 0 | 0 | 0 | 0 |
| round7-cycle10-Assembly-CSharp-engine.json | ternary-polarity | 3 | 2 | 0 | 0 | 0 |
| round7-cycle10-probe-Assembly-CSharp-engine.json | assignment-ternary-if | 3 | 2 | 0 | 0 | 0 |
| round7-cycle10-probe-Assembly-CSharp-engine.json | assignment-ternary-if-reverse | 1 | 1 | 0 | 0 | 0 |
| round7-cycle10-probe-Assembly-CSharp-engine.json | if-polarity | 1 | 1 | 1 | 3587 | 0 |
| round7-cycle10-probe-Assembly-CSharp-engine.json | smcs-empty-arm-restore | 3 | 1 | 0 | 0 | 0 |
| round7-cycle10-probe-Assembly-CSharp-engine.json | smcs-owned-switch-if | 1 | 1 | 0 | 0 | 0 |
| round7-cycle10-probe-Assembly-CSharp-engine.json | solver-group-block-declarations | 5 | 0 | 0 | 0 | 0 |
| round7-cycle10-probe-Assembly-CSharp-engine.json | ternary-polarity | 2 | 1 | 0 | 0 | 0 |
| round7-cycle11-Assembly-CSharp-engine.json | assignment-ternary-if | 6 | 4 | 2 | 25178 | 0 |
| round7-cycle11-Assembly-CSharp-engine.json | assignment-ternary-if-reverse | 2 | 2 | 0 | 0 | 0 |
| round7-cycle11-Assembly-CSharp-engine.json | if-polarity | 2 | 0 | 0 | 0 | 0 |
| round7-cycle11-Assembly-CSharp-engine.json | smcs-empty-arm-restore | 2 | 1 | 0 | 0 | 0 |
| round7-cycle11-Assembly-CSharp-engine.json | smcs-terminal-actions-common-exit | 8 | 8 | 4 | 2405 | 0 |
| round7-cycle11-Assembly-CSharp-engine.json | solver-group-all-calculated-blocks | 9 | 0 | 0 | 0 | 0 |
| round7-cycle11-Assembly-CSharp-engine.json | solver-group-block-declarations | 13 | 0 | 0 | 0 | 0 |
| round7-cycle11-Assembly-CSharp-engine.json | terminal-guard-positive-tail | 1 | 0 | 0 | 0 | 0 |
| round7-cycle11-Assembly-CSharp-engine.json | ternary-polarity | 3 | 3 | 0 | 0 | 0 |
| round7-cycle11-Assembly-CSharp-engine.json | void-empty-then-common-exit | 1 | 1 | 0 | 0 | 0 |
| round7-cycle11-probe-Assembly-CSharp-engine.json | assignment-ternary-if | 4 | 2 | 1 | 12062 | 0 |
| round7-cycle11-probe-Assembly-CSharp-engine.json | assignment-ternary-if-reverse | 2 | 2 | 0 | 0 | 0 |
| round7-cycle11-probe-Assembly-CSharp-engine.json | if-polarity | 1 | 0 | 0 | 0 | 0 |
| round7-cycle11-probe-Assembly-CSharp-engine.json | smcs-terminal-actions-common-exit | 1 | 1 | 0 | 0 | 0 |
| round7-cycle11-probe-Assembly-CSharp-engine.json | solver-group-all-calculated-blocks | 4 | 0 | 0 | 0 | 0 |
| round7-cycle11-probe-Assembly-CSharp-engine.json | solver-group-block-declarations | 4 | 0 | 0 | 0 | 0 |
| round7-cycle11-probe-Assembly-CSharp-engine.json | ternary-polarity | 2 | 2 | 0 | 0 | 0 |
| round7-cycle12-Assembly-CSharp-engine.json | loop-continue-guard | 1 | 0 | 0 | 0 | 0 |
| round7-cycle12-Assembly-CSharp-engine.json | smcs-balanced-empty-arms | 1 | 1 | 0 | 0 | 0 |
| round7-cycle12-Assembly-CSharp-engine.json | smcs-empty-arm-restore | 3 | 0 | 0 | 0 | 0 |
| round7-cycle12-Assembly-CSharp-engine.json | solver-group-all-calculated-blocks | 1 | 0 | 0 | 0 | 0 |
| round7-cycle12-Assembly-CSharp-engine.json | solver-group-block-declarations | 7 | 0 | 0 | 0 | 0 |
| round7-cycle12-probe-Assembly-CSharp-engine.json | loop-continue-guard | 1 | 0 | 0 | 0 | 0 |
| round7-cycle12-probe-Assembly-CSharp-engine.json | smcs-balanced-empty-arms | 1 | 1 | 0 | 0 | 0 |
| round7-cycle12-probe-Assembly-CSharp-engine.json | smcs-empty-arm-restore | 3 | 0 | 0 | 0 | 0 |
| round7-cycle12-probe-Assembly-CSharp-engine.json | solver-group-all-calculated-blocks | 1 | 0 | 0 | 0 | 0 |
| round7-cycle12-probe-Assembly-CSharp-engine.json | solver-group-block-declarations | 7 | 0 | 0 | 0 | 0 |
| round7-cycle13-Assembly-CSharp-engine.json | assignment-ternary-if | 5 | 0 | 0 | 0 | 0 |
| round7-cycle13-Assembly-CSharp-engine.json | assignment-ternary-if-reverse | 5 | 5 | 0 | 0 | 0 |
| round7-cycle13-Assembly-CSharp-engine.json | calculated-named-switch-selector-coalescence | 1 | 1 | 0 | 0 | 0 |
| round7-cycle13-Assembly-CSharp-engine.json | smcs-empty-arm-restore | 3 | 3 | 1 | 13213 | 0 |
| round7-cycle13-Assembly-CSharp-engine.json | solver-group-all-calculated-blocks | 8 | 0 | 0 | 0 | 0 |
| round7-cycle13-Assembly-CSharp-engine.json | solver-group-block-declarations | 8 | 0 | 0 | 0 | 0 |
| round7-cycle13-Assembly-CSharp-engine.json | ternary-polarity | 1 | 0 | 0 | 0 | 0 |
| round7-cycle13-probe-Assembly-CSharp-engine.json | assignment-ternary-if | 5 | 0 | 0 | 0 | 0 |
| round7-cycle13-probe-Assembly-CSharp-engine.json | assignment-ternary-if-reverse | 5 | 5 | 0 | 0 | 0 |
| round7-cycle13-probe-Assembly-CSharp-engine.json | calculated-named-switch-selector-coalescence | 1 | 1 | 0 | 0 | 0 |
| round7-cycle13-probe-Assembly-CSharp-engine.json | smcs-empty-arm-restore | 3 | 3 | 1 | 13213 | 0 |
| round7-cycle13-probe-Assembly-CSharp-engine.json | solver-group-all-calculated-blocks | 8 | 0 | 0 | 0 | 0 |
| round7-cycle13-probe-Assembly-CSharp-engine.json | solver-group-block-declarations | 8 | 0 | 0 | 0 | 0 |
| round7-cycle13-probe-Assembly-CSharp-engine.json | ternary-polarity | 1 | 0 | 0 | 0 | 0 |
| round7-cycle14-Assembly-CSharp-engine.json | smcs-do-infinite-tail-break | 1 | 0 | 0 | 0 | 0 |
| round7-cycle14-probe-Assembly-CSharp-engine.json | smcs-do-infinite-tail-break | 1 | 0 | 0 | 0 | 0 |
| round7-cycle15-Assembly-CSharp-engine.json | smcs-do-infinite-tail-break | 5 | 5 | 2 | 14297 | 0 |
| round7-cycle15-probe-Assembly-CSharp-engine.json | smcs-do-infinite-tail-break | 4 | 4 | 1 | 13913 | 0 |
| round7-cycle16-KairoLibrary-engine.json | roslyn-for-declaration-immediately-before | 3 | 2 | 1 | 1863 | 0 |
| round7-cycle16-probe-KairoLibrary-engine.json | roslyn-for-declaration-immediately-before | 2 | 2 | 1 | 1863 | 0 |
| round7-cycle17-Assembly-CSharp-engine.json | local-declaration-scope-hoist | 1 | 0 | 0 | 0 | 0 |
| round7-cycle17-Assembly-CSharp-engine.json | smcs-continue-positive-tail | 14 | 4 | 3 | 1925 | 0 |
| round7-cycle17-Assembly-CSharp-engine.json | smcs-empty-arm-restore | 1 | 0 | 0 | 0 | 0 |
| round7-cycle17-Assembly-CSharp-engine.json | solver-group-block-declarations | 1 | 0 | 0 | 0 | 0 |
| round7-cycle17-probe2-Assembly-CSharp-engine.json | smcs-continue-positive-tail | 1 | 1 | 1 | 995 | 0 |
| round7-cycle20-Assembly-CSharp-engine.json | smcs-protected-void-guard-tail | 1 | 1 | 1 | 2250 | 0 |
| round7-cycle20-probe-Assembly-CSharp-engine.json | smcs-protected-void-guard-tail | 1 | 1 | 1 | 2250 | 0 |
| round7-part-a-Assembly-CSharp-engine.json | assignment-ternary-if | 19 | 9 | 4 | 5386 | 0 |
| round7-part-a-Assembly-CSharp-engine.json | assignment-ternary-if-reverse | 10 | 3 | 0 | 0 | 0 |
| round7-part-a-Assembly-CSharp-engine.json | calculated-parameter-alias | 2 | 1 | 1 | 537 | 0 |
| round7-part-a-Assembly-CSharp-engine.json | common-exit-remove-fallthrough-goto | 2 | 2 | 2 | 317 | 0 |
| round7-part-a-Assembly-CSharp-engine.json | condition-local | 2 | 0 | 0 | 0 | 0 |
| round7-part-a-Assembly-CSharp-engine.json | condition-merge-or-guards | 2 | 0 | 0 | 0 | 0 |
| round7-part-a-Assembly-CSharp-engine.json | condition-split-and | 17 | 0 | 0 | 0 | 0 |
| round7-part-a-Assembly-CSharp-engine.json | condition-split-or-guards | 18 | 6 | 1 | 2576 | 0 |
| round7-part-a-Assembly-CSharp-engine.json | declaration-ternary-if | 2 | 0 | 0 | 0 | 0 |
| round7-part-a-Assembly-CSharp-engine.json | declaration-ternary-if-reverse | 2 | 1 | 0 | 0 | 0 |
| round7-part-a-Assembly-CSharp-engine.json | extended-original-zero-chain | 2 | 2 | 0 | 0 | 0 |
| round7-part-a-Assembly-CSharp-engine.json | extended-original-zero-chain-and-switch-if | 1 | 1 | 0 | 0 | 0 |
| round7-part-a-Assembly-CSharp-engine.json | if-polarity | 20 | 2 | 0 | 0 | 0 |
| round7-part-a-Assembly-CSharp-engine.json | initialized-declaration-swap | 8 | 0 | 0 | 0 | 0 |
| round7-part-a-Assembly-CSharp-engine.json | local-declaration-order-earlier | 12 | 0 | 0 | 0 | 0 |
| round7-part-a-Assembly-CSharp-engine.json | local-declaration-order-later | 20 | 0 | 0 | 0 | 0 |
| round7-part-a-Assembly-CSharp-engine.json | local-declaration-scope-hoist | 60 | 0 | 0 | 0 | 0 |
| round7-part-a-Assembly-CSharp-engine.json | loop-continue-guard | 5 | 0 | 0 | 0 | 0 |
| round7-part-a-Assembly-CSharp-engine.json | loop-continue-guards | 2 | 0 | 0 | 0 | 0 |
| round7-part-a-Assembly-CSharp-engine.json | new-object-argument-local | 2 | 2 | 2 | 228 | 0 |
| round7-part-a-Assembly-CSharp-engine.json | reserve-unused-original-slot | 2 | 0 | 0 | 0 | 0 |
| round7-part-a-Assembly-CSharp-engine.json | solver-group-all-calculated-blocks | 74 | 0 | 0 | 0 | 0 |
| round7-part-a-Assembly-CSharp-engine.json | solver-group-block-declarations | 245 | 1 | 0 | 0 | 0 |
| round7-part-a-Assembly-CSharp-engine.json | solver-group-original-zero-chain | 1 | 0 | 0 | 0 | 0 |
| round7-part-a-Assembly-CSharp-engine.json | switch-if | 3 | 1 | 0 | 0 | 0 |
| round7-part-a-Assembly-CSharp-engine.json | tail-early-return | 32 | 12 | 0 | 0 | 0 |
| round7-part-a-Assembly-CSharp-engine.json | ternary-polarity | 31 | 12 | 1 | 145 | 0 |
| round7-part-a-Assembly-CSharp-engine.json | tested-value-local | 7 | 6 | 4 | 577 | 0 |
| round7-part-a-Assembly-CSharp-engine.json | tested-value-local-block | 1 | 0 | 0 | 0 | 0 |
| round7-part-a-Assembly-CSharp-engine.json | unary-negation | 11 | 10 | 2 | 1664 | 0 |
| round7-part-a-Assembly-CSharp-engine.json | unified-result-return-goto | 3 | 2 | 0 | 0 | 0 |
| round7-part-a-Assembly-CSharp-engine.json | unified-result-return-if-else | 8 | 6 | 6 | 470 | 0 |
| round7-part-a-Assembly-CSharp-engine.json | void-common-exit-if-else | 26 | 13 | 7 | 4168 | 0 |
| round7-part-a-Assembly-CSharp-engine.json | void-common-exit-switch-parameter | 7 | 7 | 7 | 905 | 0 |
| round7-part-a-KairoLibrary-engine.json | array-index-loop-foreach | 1 | 1 | 1 | 202 | 0 |
| round7-part-a-KairoLibrary-engine.json | assignment-ternary-if | 11 | 8 | 0 | 0 | 0 |
| round7-part-a-KairoLibrary-engine.json | assignment-ternary-if-reverse | 3 | 3 | 0 | 0 | 0 |
| round7-part-a-KairoLibrary-engine.json | condition-split-and | 1 | 0 | 0 | 0 | 0 |
| round7-part-a-KairoLibrary-engine.json | if-polarity | 5 | 5 | 5 | 1077 | 0 |
| round7-part-a-KairoLibrary-engine.json | local-declaration-order-later | 3 | 0 | 0 | 0 | 0 |
| round7-part-a-KairoLibrary-engine.json | local-declaration-scope-hoist | 2 | 0 | 0 | 0 | 0 |
| round7-part-a-KairoLibrary-engine.json | roslyn-iterator-foreach-direct-array | 1 | 1 | 1 | 134 | 0 |
| round7-part-a-KairoLibrary-engine.json | roslyn-solver-delay-index-scope | 5 | 3 | 3 | 1016 | 0 |
| round7-part-a-KairoLibrary-engine.json | roslyn-solver-group-all-calculated-blocks | 2 | 0 | 0 | 0 | 0 |
| round7-part-a-KairoLibrary-engine.json | roslyn-solver-group-block-declarations | 17 | 1 | 0 | 0 | 0 |
| round7-part-a-KairoLibrary-engine.json | roslyn-solver-hoist-for-index | 7 | 1 | 1 | 309 | 0 |
| round7-part-a-KairoLibrary-engine.json | struct-fieldwise-no-initializer | 2 | 2 | 2 | 176 | 0 |
| round7-part-a-KairoLibrary-engine.json | tail-early-return | 1 | 0 | 0 | 0 | 0 |
| round7-part-a-KairoLibrary-engine.json | ternary-polarity | 2 | 2 | 0 | 0 | 0 |
| round7-probes-Assembly-CSharp-engine.json | common-exit-remove-fallthrough-goto | 1 | 1 | 1 | 79 | 0 |
| round7-probes-Assembly-CSharp-engine.json | extended-original-zero-chain-and-switch-if | 1 | 1 | 0 | 0 | 0 |
| round7-probes-Assembly-CSharp-engine.json | new-object-argument-local | 1 | 1 | 1 | 96 | 0 |
| round7-probes-Assembly-CSharp-engine.json | solver-group-all-calculated-blocks | 2 | 0 | 0 | 0 | 0 |
| round7-probes-Assembly-CSharp-engine.json | solver-group-block-declarations | 9 | 0 | 0 | 0 | 0 |
| round7-probes-Assembly-CSharp-engine.json | tail-early-return | 1 | 1 | 0 | 0 | 0 |
| round7-probes-Assembly-CSharp-engine.json | tested-value-local | 1 | 1 | 1 | 199 | 0 |
| round7-probes-Assembly-CSharp-engine.json | unified-result-return-goto | 1 | 1 | 0 | 0 | 0 |
| round7-probes-Assembly-CSharp-engine.json | void-common-exit-if-else | 2 | 0 | 0 | 0 | 0 |
| round7-probes-Assembly-CSharp-engine.json | void-common-exit-switch-parameter | 1 | 1 | 1 | 134 | 0 |
| round7-probes-KairoLibrary-engine.json | array-index-loop-foreach | 1 | 1 | 1 | 202 | 0 |
| round7-probes-KairoLibrary-engine.json | roslyn-iterator-foreach-direct-array | 1 | 1 | 1 | 134 | 0 |
| round7-probes-KairoLibrary-engine.json | roslyn-solver-delay-index-scope | 1 | 1 | 1 | 666 | 0 |
| round7-probes-KairoLibrary-engine.json | roslyn-solver-hoist-for-index | 1 | 1 | 1 | 309 | 0 |
| round7-probes-KairoLibrary-engine.json | struct-fieldwise-no-initializer | 1 | 1 | 1 | 94 | 0 |

Execuções em `probes` são testes isolados e não devem ser somadas novamente às aplicações completas. Modelos rejeitam candidatos incompletos ou com assinatura prevista diferente da original antes da compilação.

| Execução | Métodos / passes | Paradas por passe | Modelo incompleto | Assinatura prevista errada | Compilações falhas | Regressões de candidatos rejeitados | Metadados rejeitados | Modelo/emissor discordam |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| round7-cycle01-Assembly-CSharp-engine.json | 217 / 217 | {'no_model_approved_candidate': 199, 'body_equal': 7, 'no_transformation_advances': 9, 'source_mapping_UNKNOWN': 2} | 0 | 44 | 0 | 0 | 0 | 0 |
| round7-cycle01-probe-Assembly-CSharp-engine.json | 1 / 1 | {'body_equal': 1} | 0 | 0 | 0 | 0 | 0 | 0 |
| round7-cycle02-Assembly-CSharp-engine.json | 210 / 210 | {'no_model_approved_candidate': 201, 'no_transformation_advances': 1, 'source_mapping_UNKNOWN': 2, 'body_equal': 6} | 0 | 18 | 0 | 0 | 0 | 0 |
| round7-cycle02-probe-Assembly-CSharp-engine.json | 1 / 1 | {'body_equal': 1} | 0 | 0 | 0 | 0 | 0 | 0 |
| round7-cycle03-Assembly-CSharp-engine.json | 204 / 204 | {'no_model_approved_candidate': 194, 'no_transformation_advances': 3, 'source_mapping_UNKNOWN': 2, 'body_equal': 5} | 0 | 173 | 0 | 0 | 0 | 0 |
| round7-cycle03-probe-Assembly-CSharp-engine.json | 1 / 1 | {'no_transformation_advances': 1} | 0 | 9 | 0 | 0 | 0 | 0 |
| round7-cycle04-Assembly-CSharp-engine.json | 199 / 199 | {'no_model_approved_candidate': 194, 'source_mapping_UNKNOWN': 2, 'body_equal': 2, 'no_transformation_advances': 1} | 0 | 0 | 0 | 0 | 0 | 0 |
| round7-cycle04-probe-Assembly-CSharp-engine.json | 1 / 1 | {'no_model_approved_candidate': 1} | 0 | 0 | 0 | 0 | 0 | 0 |
| round7-cycle05-Assembly-CSharp-engine.json | 197 / 197 | {'no_model_approved_candidate': 192, 'no_transformation_advances': 2, 'body_equal': 1, 'source_mapping_UNKNOWN': 2} | 0 | 85 | 0 | 0 | 0 | 0 |
| round7-cycle05-probe-Assembly-CSharp-engine.json | 1 / 1 | {'body_equal': 1} | 0 | 11 | 0 | 0 | 0 | 0 |
| round7-cycle06-Assembly-CSharp-engine.json | 196 / 196 | {'no_transformation_advances': 69, 'no_model_approved_candidate': 116, 'body_equal': 9, 'source_mapping_UNKNOWN': 2} | 4 | 799 | 0 | 0 | 0 | 0 |
| round7-cycle06-probe-Assembly-CSharp-engine.json | 1 / 1 | {'no_transformation_advances': 1} | 0 | 2 | 0 | 0 | 0 | 0 |
| round7-cycle06-probe2-Assembly-CSharp-engine.json | 1 / 1 | {'body_equal': 1} | 0 | 2 | 0 | 0 | 0 | 0 |
| round7-cycle07-Assembly-CSharp-engine.json | 187 / 187 | {'no_model_approved_candidate': 184, 'no_transformation_advances': 1, 'source_mapping_UNKNOWN': 2} | 0 | 121 | 0 | 0 | 0 | 0 |
| round7-cycle07-probe-Assembly-CSharp-engine.json | 1 / 1 | {'no_transformation_advances': 1} | 0 | 121 | 0 | 0 | 0 | 0 |
| round7-cycle08-Assembly-CSharp-engine.json | 187 / 187 | {'no_transformation_advances': 2, 'no_model_approved_candidate': 181, 'source_mapping_UNKNOWN': 2, 'body_equal': 2} | 0 | 127 | 0 | 0 | 0 | 0 |
| round7-cycle08-probe-Assembly-CSharp-engine.json | 1 / 1 | {'no_transformation_advances': 1} | 0 | 95 | 0 | 0 | 0 | 0 |
| round7-cycle08-probe2-Assembly-CSharp-engine.json | 1 / 1 | {'body_equal': 1} | 0 | 0 | 0 | 0 | 0 | 0 |
| round7-cycle09-Assembly-CSharp-engine.json | 185 / 185 | {'no_model_approved_candidate': 182, 'no_transformation_advances': 1, 'source_mapping_UNKNOWN': 2} | 0 | 78 | 0 | 0 | 0 | 0 |
| round7-cycle09-probe-Assembly-CSharp-engine.json | 1 / 1 | {'no_transformation_advances': 1} | 0 | 78 | 0 | 0 | 0 | 0 |
| round7-cycle10-Assembly-CSharp-engine.json | 185 / 185 | {'no_model_approved_candidate': 173, 'body_equal': 4, 'no_transformation_advances': 6, 'source_mapping_UNKNOWN': 2} | 0 | 111 | 0 | 0 | 0 | 0 |
| round7-cycle10-probe-Assembly-CSharp-engine.json | 1 / 1 | {'body_equal': 1} | 0 | 14 | 0 | 0 | 0 | 0 |
| round7-cycle11-Assembly-CSharp-engine.json | 181 / 181 | {'no_model_approved_candidate': 172, 'body_equal': 6, 'no_transformation_advances': 1, 'source_mapping_UNKNOWN': 2} | 0 | 92 | 0 | 0 | 0 | 0 |
| round7-cycle11-probe-Assembly-CSharp-engine.json | 1 / 1 | {'body_equal': 1} | 0 | 32 | 0 | 0 | 0 | 0 |
| round7-cycle12-Assembly-CSharp-engine.json | 175 / 175 | {'no_transformation_advances': 1, 'no_model_approved_candidate': 172, 'source_mapping_UNKNOWN': 2} | 0 | 11 | 0 | 0 | 0 | 0 |
| round7-cycle12-probe-Assembly-CSharp-engine.json | 1 / 1 | {'no_transformation_advances': 1} | 0 | 11 | 0 | 0 | 0 | 0 |
| round7-cycle13-Assembly-CSharp-engine.json | 175 / 175 | {'no_model_approved_candidate': 172, 'body_equal': 1, 'source_mapping_UNKNOWN': 2} | 0 | 93 | 0 | 0 | 0 | 0 |
| round7-cycle13-probe-Assembly-CSharp-engine.json | 1 / 1 | {'body_equal': 1} | 0 | 93 | 0 | 0 | 0 | 0 |
| round7-cycle14-Assembly-CSharp-engine.json | 174 / 174 | {'no_transformation_advances': 1, 'no_model_approved_candidate': 171, 'source_mapping_UNKNOWN': 2} | 0 | 0 | 0 | 0 | 0 | 0 |
| round7-cycle14-probe-Assembly-CSharp-engine.json | 1 / 1 | {'no_transformation_advances': 1} | 0 | 0 | 0 | 0 | 0 | 0 |
| round7-cycle14-probe2-Assembly-CSharp-engine.json | 1 / 1 | {'no_model_approved_candidate': 1} | 0 | 0 | 0 | 0 | 0 | 0 |
| round7-cycle14-probe3-Assembly-CSharp-engine.json | 1 / 1 | {'no_model_approved_candidate': 1} | 0 | 0 | 0 | 0 | 0 | 0 |
| round7-cycle15-Assembly-CSharp-engine.json | 174 / 174 | {'body_equal': 2, 'no_model_approved_candidate': 170, 'source_mapping_UNKNOWN': 2} | 0 | 27 | 0 | 0 | 0 | 0 |
| round7-cycle15-probe-Assembly-CSharp-engine.json | 1 / 1 | {'body_equal': 1} | 0 | 27 | 0 | 0 | 0 | 0 |
| round7-cycle16-KairoLibrary-engine.json | 79 / 79 | {'no_model_approved_candidate': 77, 'body_equal': 1, 'no_transformation_advances': 1} | 19 | 7 | 0 | 0 | 0 | 0 |
| round7-cycle16-probe-KairoLibrary-engine.json | 1 / 1 | {'body_equal': 1} | 0 | 7 | 0 | 0 | 0 | 0 |
| round7-cycle17-Assembly-CSharp-engine.json | 172 / 172 | {'no_model_approved_candidate': 156, 'no_transformation_advances': 11, 'source_mapping_UNKNOWN': 2, 'body_equal': 3} | 0 | 28 | 0 | 0 | 0 | 0 |
| round7-cycle17-probe-Assembly-CSharp-engine.json | 1 / 1 | {'no_model_approved_candidate': 1} | 0 | 1 | 0 | 0 | 0 | 0 |
| round7-cycle17-probe2-Assembly-CSharp-engine.json | 1 / 1 | {'body_equal': 1} | 0 | 0 | 0 | 0 | 0 | 0 |
| round7-cycle18-KairoLibrary-engine.json | 78 / 78 | {'no_model_approved_candidate': 78} | 6 | 0 | 0 | 0 | 0 | 0 |
| round7-cycle18-probe-KairoLibrary-engine.json | 1 / 1 | {'no_model_approved_candidate': 1} | 1 | 0 | 0 | 0 | 0 | 0 |
| round7-cycle18-probe2-KairoLibrary-engine.json | 1 / 1 | {'no_model_approved_candidate': 1} | 1 | 0 | 0 | 0 | 0 | 0 |
| round7-cycle18-probe3-KairoLibrary-engine.json | 1 / 1 | {'no_model_approved_candidate': 1} | 2 | 0 | 0 | 0 | 0 | 0 |
| round7-cycle19-KairoLibrary-engine.json | 78 / 78 | {'no_model_approved_candidate': 78} | 1 | 0 | 0 | 0 | 0 | 0 |
| round7-cycle19-probe-KairoLibrary-engine.json | 1 / 1 | {'no_model_approved_candidate': 1} | 1 | 0 | 0 | 0 | 0 | 0 |
| round7-cycle20-Assembly-CSharp-engine.json | 169 / 169 | {'no_model_approved_candidate': 166, 'body_equal': 1, 'source_mapping_UNKNOWN': 2} | 0 | 0 | 0 | 0 | 0 | 0 |
| round7-cycle20-probe-Assembly-CSharp-engine.json | 1 / 1 | {'body_equal': 1} | 0 | 0 | 0 | 0 | 0 | 0 |
| round7-part-a-Assembly-CSharp-engine.json | 254 / 254 | {'no_transformation_advances': 111, 'no_model_approved_candidate': 102, 'model_prediction_limit': 2, 'body_equal': 37, 'source_mapping_UNKNOWN': 2} | 122 | 2969 | 0 | 0 | 0 | 1 |
| round7-part-a-KairoLibrary-engine.json | 92 / 92 | {'no_model_approved_candidate': 70, 'no_transformation_advances': 9, 'body_equal': 13} | 788 | 60 | 0 | 0 | 0 | 0 |
| round7-probes-Assembly-CSharp-engine.json | 5 / 11 | {'no_model_approved_candidate': 5, 'no_transformation_advances': 2, 'body_equal': 4} | 53 | 85 | 0 | 0 | 0 | 0 |
| round7-probes-KairoLibrary-engine.json | 5 / 7 | {'no_model_approved_candidate': 2, 'body_equal': 5} | 18 | 15 | 0 | 0 | 0 | 0 |

Perdas adotadas: zero. Uma regressão de candidato, quando registrada, foi rejeitada antes da adoção; não é escondida pela coluna de ganhos. Os recibos preservam comandos, erros, offsets, trechos e todas as tentativas.

**Diário dos ciclos autônomos**

| Ciclo | Compilador / par ponderado | Hipótese | Resultado | Métodos | Bytes |
| --- | --- | --- | --- | --- | --- |
| 1 | Assembly-CSharp / bne.un => beq | Guarda terminal recuperada como retorno deve ser um if positivo envolvendo o restante do bloco final | Novos corpos aprovados; 18 tentativas da regra, 13 avancos | 7 | 14902 |
| 2 | Assembly-CSharp / br => ret | Quando o br original termina no ret comum, if com ramo verdadeiro vazio e else preserva o desvio explicito | Novos corpos aprovados; 8 tentativas da regra, 8 avancos | 6 | 2669 |
| 3 | Assembly-CSharp / br => ret (destino incremento do laco) | O desvio original para incremento do for exige continue na guarda recuperada com return | Novos corpos aprovados; 8 tentativas da regra, 8 avancos | 5 | 4608 |
| 4 | Assembly-CSharp / brfalse => brtrue | Em if vazio, inverter apenas o ultimo operando booleano preserva chamadas e reproduz a polaridade original | Novos corpos aprovados; 4 tentativas da regra, 3 avancos | 2 | 658 |
| 5 | Assembly-CSharp / stloc.3 => stloc.0 | Variaveis locais separadas pelo decompilador devem compartilhar declaracao e slot original, junto das construcoes que explicam os temporarios | Novos corpos aprovados; 3 tentativas da regra, 3 avancos | 1 | 6626 |
| 6 | Assembly-CSharp / brfalse => brtrue / bne.un => beq (braco vazio perdido) | O smcs preserva br sobre o else mesmo com then vazio; restaurar esse braco explica a condicao invertida e o desvio extra | Novos corpos aprovados; 195 tentativas da regra, 42 avancos | 9 | 14874 |
| 7 | Assembly-CSharp / stloc.s => stloc.s | Ancoras unicas dos calculos e stores provam uma permutacao de locais entre escopos; reordenar declaracoes conserva as avaliacoes | Avanco parcial; nenhum novo matched; 1 tentativas da regra, 1 avancos | 0 | 0 |
| 8 | Assembly-CSharp / ldfld=>DELETION_POINT | Separar a segunda condição do cabeçalho do laço em guarda com break no início do corpo restaura a ordem original dos blocos smcs. | Novos corpos aprovados; 4 tentativas da regra, 4 avancos | 2 | 889 |
| 9 | Assembly-CSharp / ldstr=>ldstr | Chamar String.Concat com quatro strings preserva os literais adjacentes separados que o operador mais do smcs funde. | Avanco parcial; nenhum novo matched; 2 tentativas da regra, 2 avancos | 0 | 0 |
| 10 | Assembly-CSharp / brtrue=>stloc.s | Os switches recuperados introduzem um seletor temporário ausente no original; convertê-los em ifs com dono de cada break comprovado restaura os blocos e os slots. | Novos corpos aprovados; 11 tentativas da regra, 9 avancos | 4 | 4935 |
| 11 | Assembly-CSharp / br=>ret | Um ramo terminal com ações seguido de return deve ser restaurado como if/else quando o restante do bloco vai à saída comum comprovada do método. | Novos corpos aprovados; 8 tentativas da regra, 8 avancos | 6 | 27583 |
| 12 | Assembly-CSharp / brfalse=>brtrue | Restaurar um desvio ausente e retirar um desvio redundante de cinco bytes no mesmo ancestral evita o cancelamento de erros que impedia o avanço estrito do prefixo. | Avanco parcial; nenhum novo matched; 1 tentativas da regra, 1 avancos | 0 | 0 |
| 13 | Assembly-CSharp / brfalse=>brtrue | Calcular conjuntamente a coalescência das larguras, a cadeia de zeros e o seletor nomeado iguala os slots globais e permite restaurar a atribuição dos dois braços ao slot sete. | Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 13213 |
| 14 | Assembly-CSharp / br=>DELETION_POINT | O padrão condicional seguido de salto de saída e salto de volta corresponde a while(true) com guarda final com break, recuperado como do-while compacto. | Nenhum candidato compilado avancou; 1 tentativas da regra, 0 avancos | 0 | 0 |
| 15 | Assembly-CSharp / br=>DELETION_POINT | Usar o offset comprovado da condição antes de consultar o MDB corrige a fronteira que no ciclo14 selecionava o próximo do, permitindo testar a mesma forma de while(true) no comando certo. | Novos corpos aprovados; 5 tentativas da regra, 5 avancos | 2 | 14297 |
| 16 | KairoLibrary / stloc.s=>stloc.s | Declarar o índice imediatamente antes do for em vez de movê-lo ao topo do bloco reproduz a ordem original dos locais nomeados do Roslyn sem mudar a inicialização. | Novos corpos aprovados; 3 tentativas da regra, 2 avancos | 1 | 1863 |
| 17 | Assembly-CSharp / bne.un=>beq | A guarda com continue recuperada deve ser um if positivo que envolve a cauda da mesma iteração, preservando os breaks internos e a avaliação da condição. | Novos corpos aprovados; 14 tentativas da regra, 4 avancos | 3 | 1925 |
| 18 | KairoLibrary / brtrue.s=>switch | O teste original do caso zero seguido de comparações corresponde a ifs, e a conversão do switch recuperado precisa preservar o dono de cada break e os blocos dos casos. | INCONCLUSIVA: nenhum candidato autorizado pelo modelo/mapeamento; 0 tentativas da regra, 0 avancos | 0 | 0 |
| 19 | KairoLibrary / brfalse.s=>brtrue.s | Acesso condicional com fallback constante foi recuperado como nullable-coalesce, mas o IL original testa nulo explicitamente e chama o membro com callvirt antes de escolher o fallback. | INCONCLUSIVA: nenhum candidato autorizado pelo modelo/mapeamento; 0 tentativas da regra, 0 avancos | 0 | 0 |
| 20 | Assembly-CSharp / bne.un=>beq | A guarda void dentro de try-catch-finally terminal deve envolver positivamente o restante do try, preservando catch-finally e a saída comum do método. | Novos corpos aprovados; 1 tentativa da regra, 1 avanço; snapshot final concluído em after-verified após liberar cache e binários temporários | 1 | 2250 |

Publicação: ciclo 8 — FAILED: RPC curl 55 connection reset; remote remained f095bdb; repetição: PASS: origin/main advanced f095bdb to ba598aa; ciclo 19 — FAILED: four HTTP408 attempts (default twice, HTTP1.1, HTTP1.1 with50MiBpostBuffer); remote stayed481fa8b; repetição: PASS: fifth attempt with HTTP1.1,50MiBpostBuffer,pack.window=0; 1.76MiB uploaded; origin/main advanced481fa8b to4b73d20. Nenhuma alteração de fonte foi necessária para repetir o envio.

No ciclo 12, git diff --cached --check recusou um espaço final na linha de uma chave em DebugForm.cs. O espaço foi removido sem mudar linhas ou finais de linha; a prova de todos os corpos e metadados foi repetida e o commit foi publicado. A falha de formatação e os hashes antes/depois estão em reports/round7-cycle12-formatting-repair.json.

A preparação do ciclo 16 recusou o seletor abreviado LoadOptimize por encontrar duas sobrecargas. Nenhum candidato havia sido compilado. O teste prosseguiu com a assinatura InputStream completa, reutilizando o snapshot imutável somente após conferir HEAD, configurações, fontes e recibos.

No ciclo 20, faltou espaço durante a cópia do snapshot final. Foram removidos 3310 binários temporários de candidatos (902658048 bytes) e 1118 arquivos de cache de metadados regenerável (7575228256 bytes). Fontes, relatórios oficiais, originais e snapshots congelados foram preservados. A cópia incompleta não foi sobrescrita; o snapshot completo e verificado está em build/round7/cycles/cycle20/after-verified. A verificação completa e a prova do índice Git foram repetidas antes da publicação.

## 3. HIPÓTESES

| Estado | Hipótese | Evidência |
| --- | --- | --- |
| CONFIRMADA | Local para valor testado de qualquer tipo | data.LeagueData::IsVictoryLeague()->System.Boolean: corpo integral igual; regras tested-value-local |
| CONFIRMADA | Resultado local com retorno unico | data.PlayerData::GetPhyParamBarW(System.Int32,System.Int32)->System.Int32: corpo integral igual; regras common-exit-remove-fallthrough-goto |
| CONFIRMADA | Switch no parametro e saida comum void | surface.GameView::OnTouchDlgSel(System.Int32,System.Int32,kairo.unity.surface.TouchEvent)->System.Void: corpo integral igual; regras void-common-exit-switch-parameter |
| CONFIRMADA | Objeto em local antes das atribuicoes e de Push | system.form.FriendForm::<_update>m__17()->System.Void: corpo integral igual; regras new-object-argument-local |
| CONFIRMADA | Foreach sobre array com copia e indice sinteticos | kairo.unity.surface.SurfaceManagerBase::GetPaintRect(UnityEngine.Vector2[])->System.Int32[]: corpo integral igual; regras array-index-loop-foreach |
| CONFIRMADA | Struct sem inicializador antes das atribuicoes de campos | kairo.unity.ui.Graphics::ArgbToColor(System.Int32)->UnityEngine.Color: corpo integral igual; regras struct-fieldwise-no-initializer |
| CONFIRMADA | Escopo do indice explica slots em Canvas | kairo.unity.ui.Canvas::EndAlterStaticRender(kairo.unity.ui.Graphics,System.Int32)->System.Void: corpo integral igual; regras roslyn-solver-hoist-for-index |
| CONFIRMADA | Escopo do indice explica slots em MeshBuilder | kairo.unity.graphics.MeshBuilder::AddQuad(UnityEngine.Vector3[],System.Single,System.Single,System.Single,System.Single,UnityEngine.Color)->System.Void: corpo integral igual; regras roslyn-solver-delay-index-scope |
| CONFIRMADA | Array do foreach icado em <>7__wrap1, sem copia explicita redundante | kairo.unity.util.FastVector`1/<GetEnumerator>d__42::MoveNext()->System.Boolean: corpo integral igual; regras roslyn-iterator-foreach-direct-array |
| REFUTADA | A cadeia de zeros isolada basta para SubForm._draw2 | Modelo passou de 59 para 56 slots, mas original tem 55; dois switches recompilados compartilham um temporario Int32 ausente no original. |
| REFUTADA | Cadeia de quatro zeros mais switches convertidos em if basta para resolver SubForm._draw2 inteiro | Na Parte A a assinatura de 55 slots ficou exata e o prefixo bruto avancou de 2 a 2218, mas o corpo nao fechou. Os outros desvios exigiram transformacoes adicionais; reports/round7-smcs-zerochain-diagnosis.json. |
| REFUTADA | Todo elemento foreach cujo primeiro uso ocorre com pilha ocupada precisa de slot Roslyn | Ampliacao produziu oito previsoes completas incorretas nos matched; retirada. A regra adotada exige prova mais estreita por uso do endereco da struct. |
| CONFIRMADA | O modelo smcs recupera os 43 casos mapeados incompletos anteriores | 43/43 completos e exatos, incluindo sete storeys; validacao integral 1617/1617 previsoes completas corretas nos 1697 corpos matched. |
| REFUTADA | Ordem textual dos tipos basta para numerar todos os metodos anonimos smcs | BillingManager era previsto como m__E em vez de m__10; codigo class.cs e RootContext confirmam emissao topologica de classes base antes das derivadas. |
| REFUTADA | Ultimo comando de um bloco lexical implica fim do metodo e autoriza tail-early-return | Em SubForm._draw2 o br original 9268 vai a 9358 (incremento do slot47); em Player.Act_dribble o br638 vai a776 (incremento do slot6). A regra antiga inseriu ret em corpos ainda divergentes; nenhum matched foi perdido. A nova regra exige fim comprovado do metodo; retorno em laco e diagnosticado separadamente. |
| CONFIRMADA | terminal-guard-positive-tail: Guarda terminal recuperada como retorno deve ser um if positivo envolvendo o restante do bloco final | cycle01: 7 novos matched, 14902 bytes; Novos corpos aprovados; 18 tentativas da regra, 13 avancos |
| CONFIRMADA | void-empty-then-common-exit: Quando o br original termina no ret comum, if com ramo verdadeiro vazio e else preserva o desvio explicito | cycle02: 6 novos matched, 2669 bytes; Novos corpos aprovados; 8 tentativas da regra, 8 avancos |
| CONFIRMADA | loop-guard-return-to-continue: O desvio original para incremento do for exige continue na guarda recuperada com return | cycle03: 5 novos matched, 4608 bytes; Novos corpos aprovados; 8 tentativas da regra, 8 avancos |
| CONFIRMADA | empty-if-terminal-polarity: Em if vazio, inverter apenas o ultimo operando booleano preserva chamadas e reproduz a polaridade original | cycle04: 2 novos matched, 658 bytes; Novos corpos aprovados; 4 tentativas da regra, 3 avancos |
| CONFIRMADA | calculated-original-local-coalescence: Variaveis locais separadas pelo decompilador devem compartilhar declaracao e slot original, junto das construcoes que explicam os temporarios | cycle05: 1 novos matched, 6626 bytes; Novos corpos aprovados; 3 tentativas da regra, 3 avancos |
| CONFIRMADA | smcs-empty-arm-restore: O smcs preserva br sobre o else mesmo com then vazio; restaurar esse braco explica a condicao invertida e o desvio extra | cycle06: 9 novos matched, 14874 bytes; Novos corpos aprovados; 195 tentativas da regra, 42 avancos |
| INCONCLUSIVA | calculated-store-anchored-declarations: Ancoras unicas dos calculos e stores provam uma permutacao de locais entre escopos; reordenar declaracoes conserva as avaliacoes | cycle07: 0 novos matched, 0 bytes; Avanco parcial; nenhum novo matched; 1 tentativas da regra, 1 avancos |
| CONFIRMADA | smcs-loop-condition-body-guard: Separar a segunda condição do cabeçalho do laço em guarda com break no início do corpo restaura a ordem original dos blocos smcs. | cycle08: 2 novos matched, 889 bytes; Novos corpos aprovados; 4 tentativas da regra, 4 avancos |
| INCONCLUSIVA | explicit-string-concat-preserve-literals: Chamar String.Concat com quatro strings preserva os literais adjacentes separados que o operador mais do smcs funde. | cycle09: 0 novos matched, 0 bytes; Avanco parcial; nenhum novo matched; 2 tentativas da regra, 2 avancos |
| CONFIRMADA | smcs-owned-switch-if: Os switches recuperados introduzem um seletor temporário ausente no original; convertê-los em ifs com dono de cada break comprovado restaura os blocos e os slots. | cycle10: 4 novos matched, 4935 bytes; Novos corpos aprovados; 11 tentativas da regra, 9 avancos |
| CONFIRMADA | smcs-terminal-actions-common-exit: Um ramo terminal com ações seguido de return deve ser restaurado como if/else quando o restante do bloco vai à saída comum comprovada do método. | cycle11: 6 novos matched, 27583 bytes; Novos corpos aprovados; 8 tentativas da regra, 8 avancos |
| INCONCLUSIVA | smcs-balanced-empty-arms: Restaurar um desvio ausente e retirar um desvio redundante de cinco bytes no mesmo ancestral evita o cancelamento de erros que impedia o avanço estrito do prefixo. | cycle12: 0 novos matched, 0 bytes; Avanco parcial; nenhum novo matched; 1 tentativas da regra, 1 avancos |
| CONFIRMADA | calculated-named-switch-selector-coalescence: Calcular conjuntamente a coalescência das larguras, a cadeia de zeros e o seletor nomeado iguala os slots globais e permite restaurar a atribuição dos dois braços ao slot sete. | cycle13: 1 novos matched, 13213 bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos |
| INCONCLUSIVA | smcs-do-infinite-tail-break: O padrão condicional seguido de salto de saída e salto de volta corresponde a while(true) com guarda final com break, recuperado como do-while compacto. | cycle14: 0 novos matched, 0 bytes; Nenhum candidato compilado avancou; 1 tentativas da regra, 0 avancos |
| CONFIRMADA | smcs-do-infinite-tail-break: Usar o offset comprovado da condição antes de consultar o MDB corrige a fronteira que no ciclo14 selecionava o próximo do, permitindo testar a mesma forma de while(true) no comando certo. | cycle15: 2 novos matched, 14297 bytes; Novos corpos aprovados; 5 tentativas da regra, 5 avancos |
| CONFIRMADA | roslyn-for-declaration-immediately-before: Declarar o índice imediatamente antes do for em vez de movê-lo ao topo do bloco reproduz a ordem original dos locais nomeados do Roslyn sem mudar a inicialização. | cycle16: 1 novos matched, 1863 bytes; Novos corpos aprovados; 3 tentativas da regra, 2 avancos |
| CONFIRMADA | smcs-continue-positive-tail: A guarda com continue recuperada deve ser um if positivo que envolve a cauda da mesma iteração, preservando os breaks internos e a avaliação da condição. | cycle17: 3 novos matched, 1925 bytes; Novos corpos aprovados; 14 tentativas da regra, 4 avancos |
| INCONCLUSIVA | switch-owned-break-if-chain: O teste original do caso zero seguido de comparações corresponde a ifs, e a conversão do switch recuperado precisa preservar o dono de cada break e os blocos dos casos. | cycle18: 0 novos matched, 0 bytes; INCONCLUSIVA: nenhum candidato autorizado pelo modelo/mapeamento; 0 tentativas da regra, 0 avancos |
| INCONCLUSIVA | nullable-conditional-coalesce-explicit: Acesso condicional com fallback constante foi recuperado como nullable-coalesce, mas o IL original testa nulo explicitamente e chama o membro com callvirt antes de escolher o fallback. | cycle19: 0 novos matched, 0 bytes; INCONCLUSIVA: nenhum candidato autorizado pelo modelo/mapeamento; 0 tentativas da regra, 0 avancos |
| CONFIRMADA | smcs-protected-void-guard-tail: A guarda void dentro de try-catch-finally terminal deve envolver positivamente o restante do try, preservando catch-finally e a saída comum do método. | cycle20: 1 novos matched, 2250 bytes; Novos corpos aprovados; 1 tentativa da regra, 1 avanço; snapshot final concluído em after-verified após liberar cache e binários temporários |
| REFUTADA | Uma assinatura com tipos iguais basta para posicionar os slots Int32 sem uso | Em UserData.Draw_edit2 o primeiro candidato posicionava as reservas em 10/40, apesar dos mesmos tipos; foi rejeitado. O modelo da proposta final confirmou exatamente os slots 8/38 e o verificador fechou o corpo de 6626 bytes. |
| CONFIRMADA | Duas divergências de tamanho compensado podem bloquear as correções isoladas pelo critério de prefixo | Duas fixtures bool/float: as edições isoladas de +5/-5 bytes regrediram o prefixo de 26 para 19; a composição reproduziu os corpos completos. No ciclo 12, DebugForm.Update_gameParam avançou o prefixo bruto de 2276 para 2958 sem novo matched. reports/round7-balanced-branch-fixtures.json e round7-cycle12-Assembly-CSharp-engine.json. |
| CONFIRMADA | A assinatura global incorreta impedia testar a atribuição ternária local de SubForm._draw6 | As variantes do ternário mantinham os 107 locais recuperados. A composição calculada reproduziu os 105 locais originais, incluindo slots 7/49/61/62; o ciclo 13 fechou os 13213 bytes após nove passos aceitos, sem perdas. |
| REFUTADA | for(;;) reproduz a mesma entrada de laço da variante while(true) nas fixtures da guarda final | A variante for introduziu um salto de entrada ausente dos controles e dos três alvos diagnosticados. Enquanto isso, while(true) com guarda final reproduziu seis corpos toy completos; fonte Mono statement.cs For588, While438–449 e Do322–348. reports/round7-infinite-tail-guard-fixtures.json e diagnóstico correspondente. |
| REFUTADA | A primeira fixture de sombreamento com campo sem qualificação this era um controle positivo compilável no smcs | O smcs recusou essa tentativa com CS0135; ela foi registrada como tentativa negativa. A fixture corrigida com this.value e o bloco original preservado passou na comparação completa e de trace. Nenhum fonte do jogo foi alterado por essa tentativa. |
| REFUTADA | O offset de uma instrução ausente depois da condição de um do sempre seleciona o comando fonte do próprio laço no MDB | Ciclo 14: em DebugForm.Update_gameParam, selecionar 11628 apontou a linha 2364, o próximo if, e transformou o segundo do; a primeira divergência 11628 permaneceu. O ponto anterior à condição 11623 está em 11574/linha 2359, dentro do primeiro do. Uma tentativa compilada, zero avanços e zero perdas; os MDBs foram aceitos somente após prova de corpo idêntico a debug-. |

FastVector: o original grava `<>7__wrap1:!0[]` com `stfld` no offset 35; `<>7__wrap2:Int32` é o índice sintético. O campo é a cópia da coleção do `foreach`, cujo escopo atravessa os `yield return` e precisa sobreviver às retomadas do iterador. Ele existe nos dois lados porque os dois fontes possuem esse foreach. A reconstrução acrescentava antes dele `T[] array = elementData;`: esse alias só era usado para iniciar o foreach, não precisava sobreviver ao yield e ficava em um slot local adicional. Usar diretamente `foreach (T val in elementData)` removeu o slot redundante e preservou os campos; o corpo MoveNext completo bateu. A declaração histórica literal permanece UNKNOWN; não foi necessário escrever um campo ou classe gerada.

**Modelos de slots — validação contra a linha de base**

| Modelo | Corpos matched | Mapeados | Gerados/não mapeados | Assinaturas completas | Exatas completas | Precisão | Cobertura de completas | Mapeados incompletos | Incompletos com lista de tipos coincidente |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| smcs | 1697 | 1617 | 80 | 1617 | 1617 | 100.000000% | 95.285798% | 0 | 0 |
| Roslyn 1.1 | 3117 | 2786 | 331 | 2214 | 2214 | 100.000000% | 71.029836% | 572 | 317 |

Uma lista de tipos coincidente numa previsão incompleta não autoriza testar o candidato: o lowering ou scheduler não modelado pode introduzir outros slots após a mudança de fonte.

Classificação das limitações restantes dos modelos. Contagens de razões são não exclusivas: um método pode ter mais de um motivo. Previsões completas incorretas no corpus de validação: zero; casos sem suporte permanecem UNKNOWN.

| Modelo | Razão agrupada | Ocorrências |
| --- | --- | --- |
| smcs | generated_or_unmapped_source_method | 80 |
| Roslyn 1.1 | stack_scheduler_multiple_use | 414 |
| Roslyn 1.1 | generated_or_unmapped_source_method | 331 |
| Roslyn 1.1 | anonymous_function_or_capture | 129 |
| Roslyn 1.1 | exception_lowering | 123 |
| Roslyn 1.1 | stack_scheduler_single_use | 59 |
| Roslyn 1.1 | foreach_iteration_stack_scheduler | 44 |
| Roslyn 1.1 | string_concat_value_temporary | 24 |
| Roslyn 1.1 | conditional_access_temporary | 23 |
| Roslyn 1.1 | foreach_collection_local_stack_scheduler | 21 |
| Roslyn 1.1 | rvalue_value_type_member_temporary | 15 |
| Roslyn 1.1 | lock_lowering | 13 |
| Roslyn 1.1 | array_postfix_temporary | 11 |
| Roslyn 1.1 | default_expression_temporary | 9 |
| Roslyn 1.1 | switch_lowering | 8 |
| Roslyn 1.1 | foreach_protected_return | 5 |
| Roslyn 1.1 | object_or_collection_initializer | 5 |
| Roslyn 1.1 | using_lowering | 4 |
| Roslyn 1.1 | ref_assignment_value_temporary | 4 |
| Roslyn 1.1 | iterator_state_machine | 2 |
| Roslyn 1.1 | foreach_runtime_dispose | 1 |

A taxa das previsões completas não mede cobertura total. Casos incompletos e gerados sem mapeamento permanecem UNKNOWN; não recebem aprovação por assinatura vazia. O smcs resolve variáveis em ordem de inserção do bloco; nomes não determinam slots. O Roslyn usa ordem de declaração e temporários por assinatura, mas o escalonador de pilha pode eliminar locais. Capturas, lowering e escalonamento não modelados são rejeitados explicitamente. As notas `docs/notes/round7-slot-model.md` e `docs/notes/round7-roslyn-model.md` identificam os arquivos e trechos dos compiladores consultados.

Trechos consultados: Roslyn, snapshot `4cb203dab89e1542cdf7d88cb6d32767bc0e5460`, `src/Compilers/CSharp/Portable/CodeGen/EmitStatement.cs:594–613`: `foreach (var local in block.Locals)` chama `DefineLocal(...)` antes de `foreach (var statement in block.Statements) EmitStatement(statement)`. Mono/Unity, snapshot `b6587926c0f146c8b12985744f33a13b39a22d57`, `mcs/mcs/statement.cs:1225–1239`: `if (HoistedVariableReference != null) return;`, seguido da declaração do local quando `builder == null`. Em `ExplicitBlock.Emit`, o storey é instanciado na entrada do bloco, após os locais ordinários. A correspondência exata desses snapshots à fabricação dos executáveis instalados permanece UNKNOWN.

## 4. PENDENTES POR GRUPO

| Assembly | Tamanho original de IL | Corpos divergentes | Bytes |
| --- | --- | --- | --- |
| KairoLibrary | <=64 | 3 | 137 |
| KairoLibrary | 65-256 | 26 | 4168 |
| KairoLibrary | 257-1024 | 28 | 15607 |
| KairoLibrary | >1024 | 21 | 37064 |
| Assembly-CSharp | <=64 | 5 | 237 |
| Assembly-CSharp | 65-256 | 30 | 4827 |
| Assembly-CSharp | 257-1024 | 66 | 36529 |
| Assembly-CSharp | >1024 | 67 | 207677 |

Corpos já iguais, mas ainda sem promoção por vínculo gerado: 3 (25 bytes).

| Assembly | ID | Bytes |
| --- | --- | --- |
| KairoLibrary | kairo.unity.system.KairoBase/<>c::.cctor()->System.Void | 11 |
| KairoLibrary | kairo.unity.system.KairoBase/<>c::.ctor()->System.Void | 7 |
| Assembly-CSharp | <PrivateImplementationDetails>::.ctor()->System.Void | 7 |

smcs pendentes com assinatura de locais igual: 90/168; diferente: 78.

Histograma por alinhamento de sequências; destinos viram rótulos apenas no diagnóstico. Peso = soma do tamanho integral do IL original dos métodos do grupo, não quantidade de bytes divergentes. Agrupamento abaixo por par de opcodes; operandos exatos, todas as classes e cada método estão em `reports/round7-final-measurement.json`.

**KairoLibrary — todos os pares restantes**

| Par alinhado | Métodos | Peso em bytes |
| --- | --- | --- |
| stloc.s => stloc.s | 11 | 12061 |
| brtrue.s => switch | 3 | 4720 |
| brfalse.s => brtrue.s | 5 | 4079 |
| brfalse => brtrue.s | 2 | 4071 |
| ldc.i4.0 => brtrue.s | 1 | 3513 |
| brtrue => switch | 2 | 2665 |
| stloc.2 => stloc.s | 1 | 2403 |
| conv.r4 => DELETION_POINT | 2 | 2345 |
| switch => switch | 1 | 2305 |
| INSERTION_POINT => stloc.s | 3 | 1765 |
| INSERTION_POINT => ldc.i4.0 | 2 | 1539 |
| stloc.2 => stloc.3 | 1 | 1125 |
| brtrue.s => ldc.i4.4 | 1 | 1054 |
| brfalse => brfalse.s | 1 | 955 |
| stloc.2 => stloc.1 | 1 | 944 |
| ldarg.0 => DELETION_POINT | 4 | 823 |
| br.s => leave.s | 1 | 789 |
| stloc.s => stloc.2 | 1 | 784 |
| stloc.0 => stloc.2 | 2 | 770 |
| INSERTION_POINT => brtrue.s | 2 | 737 |
| stloc.3 => stloc.1 | 1 | 680 |
| ldc.i4.1 => DELETION_POINT | 1 | 661 |
| stloc.0 => stloc.1 | 2 | 576 |
| brfalse.s => brtrue | 1 | 537 |
| brtrue.s => brfalse.s | 2 | 467 |
| stloc.3 => stloc.s | 1 | 416 |
| leave => br | 1 | 383 |
| bne.un.s => beq | 1 | 372 |
| stloc.1 => stloc.3 | 1 | 310 |
| ldloc.1 => ldelema | 1 | 275 |
| INSERTION_POINT => stloc.2 | 2 | 251 |
| ldelem.ref => DELETION_POINT | 1 | 216 |
| ble.s => ble.s | 1 | 215 |
| stloc.0 => dup | 1 | 199 |
| INSERTION_POINT => dup | 1 | 197 |
| ldelem.any => ldelema | 1 | 192 |
| br.s => leave | 1 | 191 |
| stloc.s => DELETION_POINT | 1 | 190 |
| bne.un.s => beq.s | 1 | 182 |
| leave => br.s | 1 | 175 |
| leave.s => br.s | 1 | 164 |
| stloc.3 => stloc.0 | 1 | 149 |
| constrained. => call | 1 | 128 |
| INSERTION_POINT => stloc.1 | 1 | 105 |
| stloc.0 => stloc.3 | 1 | 94 |
| stloc.1 => DELETION_POINT | 1 | 91 |
| ldloc.0 => DELETION_POINT | 1 | 67 |
| stloc.2 => stloc.0 | 1 | 46 |

Os cinco pares com operandos exatos mais pesados são apresentados também; os endereços de saltos tornam esse agrupamento mais fragmentado que o de opcodes.

| Par exato | Métodos | Peso em bytes |
| --- | --- | --- |
| `{"opcode":"ldc.i4.0","operand":null} => {"opcode":"brtrue.s","operand":{"branch":1960}}` | 1 | 3513 |
| `{"opcode":"brtrue.s","operand":{"branch":762}} => {"opcode":"switch","operand":{"switch":[769,810,823,861,908,922,945,953,960,1072]}}` | 1 | 3097 |
| `{"opcode":"brfalse","operand":{"branch":3011}} => {"opcode":"brtrue.s","operand":{"branch":42}}` | 1 | 3012 |
| `{"opcode":"stloc.2","operand":null} => {"opcode":"stloc.s","operand":{"local":4}}` | 1 | 2403 |
| `{"opcode":"conv.r4","operand":null} => {"opcode":"DELETION_POINT","operand":null}` | 2 | 2345 |

**KairoLibrary, par 1 (opcodes): `stloc.s => stloc.s`; 11 métodos / 12061 bytes.** Exemplo: `kairo.unity.util.JarInflater::GetData(System.String)->System.Byte[]`.

Já compilado no grupo: nenhum candidato chegou à compilação. Recusas e limites antes de compilar: {'incomplete': 127}. Candidatos antigos descartados por busca já esgotada: 371. Nomes individuais das regras recusadas pelo modelo: UNKNOWN (não foram registrados por candidato). Paradas: {'no_model_approved_candidate': 44}.

| IL original | IL recompilado |
| --- | --- |
| ``IL_001a: call class System.String::Concat``0(class System.String,class System.String)->class System.String;this=False;explicit=False;cc=Default`` | ``IL_001a: call class System.String::Concat``0(class System.String,class System.String)->class System.String;this=False;explicit=False;cc=Default`` |
| ``IL_001f: call class kairo.unity.io.Storage::Read``0(valuetype System.Int32,valuetype System.Boolean,class System.String)->valuetype System.Byte[];this=False;explicit=False;cc=Default`` | ``IL_001f: call class kairo.unity.io.Storage::Read``0(valuetype System.Int32,valuetype System.Boolean,class System.String)->valuetype System.Byte[];this=False;explicit=False;cc=Default`` |
| `IL_0024: stloc.s 5` | `IL_0024: stloc.s 4` |
| `IL_0026: ldloc.s 5` | `IL_0026: ldloc.s 4` |
| `IL_0028: brfalse.s L0` | `IL_0028: brfalse.s L0` |
| `IL_002a: ldloc.s 5` | `IL_002a: ldloc.s 4` |

Regiões de exceção originais: `[{"kind": "Catch", "try_start": 157, "try_end": 165, "handler_start": 165, "handler_end": 168, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"}, {"kind": "Catch", "try_start": 173, "try_end": 181, "handler_start": 181, "handler_end": 184, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"}, {"kind": "Finally", "try_start": 120, "try_end": 154, "handler_start": 154, "handler_end": 187, "filter_start": -1, "catch_type": null}]`; recompiladas: `[{"kind": "Catch", "try_start": 154, "try_end": 162, "handler_start": 162, "handler_end": 165, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"}, {"kind": "Catch", "try_start": 170, "try_end": 178, "handler_start": 178, "handler_end": 181, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"}, {"kind": "Finally", "try_start": 118, "try_end": 151, "handler_start": 151, "handler_end": 184, "filter_start": -1, "catch_type": null}]`.

**KairoLibrary, par 2 (opcodes): `brtrue.s => switch`; 3 métodos / 4720 bytes.** Exemplo: `kairo.unity.ui.ResourceManager::LoadStart(kairo.unity.util.JarInflater,System.Int32)->System.Void`.

Já compilado no grupo: nenhum candidato chegou à compilação. Recusas e limites antes de compilar: {'incomplete': 77}. Candidatos antigos descartados por busca já esgotada: 273. Nomes individuais das regras recusadas pelo modelo: UNKNOWN (não foram registrados por candidato). Paradas: {'no_model_approved_candidate': 15}.

| IL original | IL recompilado |
| --- | --- |
| `IL_0290: stelem.ref ` | `IL_0290: stelem.ref ` |
| `IL_0291: ldloc.s 8` | `IL_0291: ldloc.s 8` |
| `IL_0293: brtrue.s O725` | `IL_0293: switch L26,L29,L32` |
| `IL_0295: ldloc.s 9` | `IL_02a4: br L35` |
| `IL_0297: newarr class kairo.unity.ui.Image` | `IL_02a9: ldloc.s 9` |
|  | `IL_02ab: newarr class kairo.unity.ui.Image` |

**KairoLibrary, par 3 (opcodes): `brfalse.s => brtrue.s`; 5 métodos / 4079 bytes.** Exemplo: `kairo.unity.util.StreamUtil::WriteArray(java.io.OutputStream,System.Array,System.Int32)->System.Void`.

Já compilado no grupo: roslyn-solver-group-block-declarations: 1, local-declaration-scope-hoist: 2, tail-early-return: 1, condition-split-and: 1. Recusas e limites antes de compilar: {'incomplete': 31, 'wrong_signature': 2}. Candidatos antigos descartados por busca já esgotada: 111. Nomes individuais das regras recusadas pelo modelo: UNKNOWN (não foram registrados por candidato). Paradas: {'no_model_approved_candidate': 20, 'no_transformation_advances': 1}.

| IL original | IL recompilado |
| --- | --- |
| `IL_0000: ldarg.1 ` | `IL_0000: ldarg.1 ` |
| `IL_0001: brfalse.s O12` | `IL_0001: brtrue.s L0` |
| `IL_0003: ldarg.1 ` | `IL_0003: ldc.i4.m1 ` |
| `IL_0004: ldc.i4.0 ` | `IL_0004: br.s L1` |
|  | `IL_0006: ldarg.1 ` |
|  | `IL_0007: ldc.i4.0 ` |

Regiões de exceção originais: `[{"kind": "Finally", "try_start": 196, "try_end": 233, "handler_start": 233, "handler_end": 253, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 273, "try_end": 309, "handler_start": 309, "handler_end": 329, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 349, "try_end": 385, "handler_start": 385, "handler_end": 405, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 425, "try_end": 461, "handler_start": 461, "handler_end": 481, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 501, "try_end": 537, "handler_start": 537, "handler_end": 557, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 577, "try_end": 613, "handler_start": 613, "handler_end": 633, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 653, "try_end": 689, "handler_start": 689, "handler_end": 709, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 729, "try_end": 765, "handler_start": 765, "handler_end": 785, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 805, "try_end": 841, "handler_start": 841, "handler_end": 861, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 881, "try_end": 917, "handler_start": 917, "handler_end": 937, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 957, "try_end": 993, "handler_start": 993, "handler_end": 1013, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1033, "try_end": 1069, "handler_start": 1069, "handler_end": 1089, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1109, "try_end": 1145, "handler_start": 1145, "handler_end": 1165, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1185, "try_end": 1226, "handler_start": 1226, "handler_end": 1246, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1266, "try_end": 1307, "handler_start": 1307, "handler_end": 1327, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1347, "try_end": 1388, "handler_start": 1388, "handler_end": 1408, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1428, "try_end": 1469, "handler_start": 1469, "handler_end": 1489, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1509, "try_end": 1545, "handler_start": 1545, "handler_end": 1565, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1595, "try_end": 1643, "handler_start": 1643, "handler_end": 1663, "filter_start": -1, "catch_type": null}]`; recompiladas: `[{"kind": "Finally", "try_start": 200, "try_end": 234, "handler_start": 234, "handler_end": 254, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 275, "try_end": 308, "handler_start": 308, "handler_end": 328, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 349, "try_end": 382, "handler_start": 382, "handler_end": 402, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 423, "try_end": 456, "handler_start": 456, "handler_end": 476, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 497, "try_end": 530, "handler_start": 530, "handler_end": 550, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 571, "try_end": 604, "handler_start": 604, "handler_end": 624, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 645, "try_end": 678, "handler_start": 678, "handler_end": 698, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 719, "try_end": 752, "handler_start": 752, "handler_end": 772, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 793, "try_end": 826, "handler_start": 826, "handler_end": 846, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 867, "try_end": 900, "handler_start": 900, "handler_end": 920, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 941, "try_end": 974, "handler_start": 974, "handler_end": 994, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1015, "try_end": 1048, "handler_start": 1048, "handler_end": 1068, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1089, "try_end": 1122, "handler_start": 1122, "handler_end": 1142, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1163, "try_end": 1201, "handler_start": 1201, "handler_end": 1221, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1242, "try_end": 1280, "handler_start": 1280, "handler_end": 1300, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1321, "try_end": 1359, "handler_start": 1359, "handler_end": 1379, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1400, "try_end": 1438, "handler_start": 1438, "handler_end": 1458, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1479, "try_end": 1512, "handler_start": 1512, "handler_end": 1532, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1563, "try_end": 1611, "handler_start": 1611, "handler_end": 1631, "filter_start": -1, "catch_type": null}]`.

**KairoLibrary, par 4 (opcodes): `brfalse => brtrue.s`; 2 métodos / 4071 bytes.** Exemplo: `kairo.unity.ui.Balloon::Draw(kairo.unity.ui.Graphics,System.Single,System.Single,UnityEngine.Matrix4x4)->System.Void`.

Já compilado no grupo: nenhum candidato chegou à compilação. Recusas e limites antes de compilar: {'incomplete': 36}. Candidatos antigos descartados por busca já esgotada: 108. Nomes individuais das regras recusadas pelo modelo: UNKNOWN (não foram registrados por candidato). Paradas: {'no_model_approved_candidate': 8}.

| IL original | IL recompilado |
| --- | --- |
| `IL_0021: ldarg.1 ` | `IL_0021: ldarg.1 ` |
| `IL_0022: ldfld class kairo.unity.ui.Graphics::render_:class kairo.unity.ui.Image` | `IL_0022: ldfld class kairo.unity.ui.Graphics::render_:class kairo.unity.ui.Image` |
| `IL_0027: brfalse L28` | `IL_0027: brtrue.s L0` |
| `IL_002c: ldc.i4.s 60` | `IL_0029: ret ` |
| `IL_002e: stloc.0 ` | `IL_002a: ldc.i4.s 60` |
|  | `IL_002c: stloc.s 24` |

Regiões de exceção originais: `[{"kind": "Finally", "try_start": 1096, "try_end": 1874, "handler_start": 1874, "handler_end": 1888, "filter_start": -1, "catch_type": null}]`; recompiladas: `[{"kind": "Finally", "try_start": 1091, "try_end": 1865, "handler_start": 1865, "handler_end": 1879, "filter_start": -1, "catch_type": null}]`.

**KairoLibrary, par 5 (opcodes): `ldc.i4.0 => brtrue.s`; 1 métodos / 3513 bytes.** Exemplo: `kairo.unity.ui.Graphics::_drawBitmap(kairo.unity.ui.Image,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single)->System.Void`.

Já compilado no grupo: nenhum candidato chegou à compilação. Recusas e limites antes de compilar: {'incomplete': 36}. Candidatos antigos descartados por busca já esgotada: 106. Nomes individuais das regras recusadas pelo modelo: UNKNOWN (não foram registrados por candidato). Paradas: {'no_model_approved_candidate': 4}.

| IL original | IL recompilado |
| --- | --- |
| `IL_0752: ldarg.0 ` | `IL_0755: ldarg.0 ` |
| `IL_0753: ldfld class kairo.unity.ui.Graphics::setFigureClip_:valuetype System.Boolean` | `IL_0756: ldfld class kairo.unity.ui.Graphics::setFigureClip_:valuetype System.Boolean` |
| `IL_0758: ldc.i4.0 ` | `IL_075b: brtrue.s L36` |
| `IL_0759: ceq ` | `IL_075d: ldarg.0 ` |
| `IL_075b: br.s O1886` | ``IL_075e: call class kairo.unity.ui.Graphics::UpdateGUIGroup``0()->class System.Void;this=True;explicit=False;cc=Default`` |
| `IL_075d: ldc.i4.0 ` |  |
| `IL_075e: brfalse.s L36` |  |
| `IL_0760: ldarg.0 ` |  |
| ``IL_0761: call class kairo.unity.ui.Graphics::UpdateGUIGroup``0()->class System.Void;this=True;explicit=False;cc=Default`` |  |

Regiões de exceção originais: `[{"kind": "Finally", "try_start": 2796, "try_end": 3028, "handler_start": 3028, "handler_end": 3050, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 3134, "try_end": 3393, "handler_start": 3393, "handler_end": 3399, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 763, "try_end": 3407, "handler_start": 3407, "handler_end": 3428, "filter_start": -1, "catch_type": null}]`; recompiladas: `[{"kind": "Finally", "try_start": 2791, "try_end": 3023, "handler_start": 3023, "handler_end": 3045, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 3129, "try_end": 3393, "handler_start": 3393, "handler_end": 3399, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 763, "try_end": 3407, "handler_start": 3407, "handler_end": 3428, "filter_start": -1, "catch_type": null}]`.

**KairoLibrary, par 1 (operandos exatos): `ldc.i4.0 => brtrue.s`; 1 métodos / 3513 bytes.** Exemplo: `kairo.unity.ui.Graphics::_drawBitmap(kairo.unity.ui.Image,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single)->System.Void`.

Já compilado no grupo: nenhum candidato chegou à compilação. Recusas e limites antes de compilar: {'incomplete': 36}. Candidatos antigos descartados por busca já esgotada: 106. Nomes individuais das regras recusadas pelo modelo: UNKNOWN (não foram registrados por candidato). Paradas: {'no_model_approved_candidate': 4}.

| IL original | IL recompilado |
| --- | --- |
| `IL_0752: ldarg.0 ` | `IL_0755: ldarg.0 ` |
| `IL_0753: ldfld class kairo.unity.ui.Graphics::setFigureClip_:valuetype System.Boolean` | `IL_0756: ldfld class kairo.unity.ui.Graphics::setFigureClip_:valuetype System.Boolean` |
| `IL_0758: ldc.i4.0 ` | `IL_075b: brtrue.s L36` |
| `IL_0759: ceq ` | `IL_075d: ldarg.0 ` |
| `IL_075b: br.s O1886` | ``IL_075e: call class kairo.unity.ui.Graphics::UpdateGUIGroup``0()->class System.Void;this=True;explicit=False;cc=Default`` |
| `IL_075d: ldc.i4.0 ` |  |
| `IL_075e: brfalse.s L36` |  |
| `IL_0760: ldarg.0 ` |  |
| ``IL_0761: call class kairo.unity.ui.Graphics::UpdateGUIGroup``0()->class System.Void;this=True;explicit=False;cc=Default`` |  |

Regiões de exceção originais: `[{"kind": "Finally", "try_start": 2796, "try_end": 3028, "handler_start": 3028, "handler_end": 3050, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 3134, "try_end": 3393, "handler_start": 3393, "handler_end": 3399, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 763, "try_end": 3407, "handler_start": 3407, "handler_end": 3428, "filter_start": -1, "catch_type": null}]`; recompiladas: `[{"kind": "Finally", "try_start": 2791, "try_end": 3023, "handler_start": 3023, "handler_end": 3045, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 3129, "try_end": 3393, "handler_start": 3393, "handler_end": 3399, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 763, "try_end": 3407, "handler_start": 3407, "handler_end": 3428, "filter_start": -1, "catch_type": null}]`.

**KairoLibrary, par 2 (operandos exatos): `brtrue.s => switch`; 1 métodos / 3097 bytes.** Exemplo: `kairo.unity.ui.TextLayout::_makeTextBlocks()->System.Void`.

Já compilado no grupo: nenhum candidato chegou à compilação. Recusas e limites antes de compilar: {'incomplete': 48}. Candidatos antigos descartados por busca já esgotada: 184. Nomes individuais das regras recusadas pelo modelo: UNKNOWN (não foram registrados por candidato). Paradas: {'no_model_approved_candidate': 5}.

| IL original | IL recompilado |
| --- | --- |
| `IL_02cb: stloc.s 30` | `IL_02cb: stloc.s 30` |
| `IL_02cd: ldloc.s 26` | `IL_02cd: ldloc.s 26` |
| `IL_02cf: brtrue.s O762` | `IL_02cf: switch L15,L16,L17,L18,L19,L20,L21,L22,L23,L30` |
| `IL_02d1: ldloc.s 30` | `IL_02fc: br L38` |
| `IL_02d3: ldc.i4.s 16` | `IL_0301: ldloc.s 30` |
|  | `IL_0303: ldc.i4.s 16` |

Regiões de exceção originais: `[{"kind": "Catch", "try_start": 586, "try_end": 597, "handler_start": 597, "handler_end": 600, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"}, {"kind": "Catch", "try_start": 601, "try_end": 614, "handler_start": 614, "handler_end": 617, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"}, {"kind": "Finally", "try_start": 156, "try_end": 2930, "handler_start": 2930, "handler_end": 3096, "filter_start": -1, "catch_type": null}]`; recompiladas: `[{"kind": "Catch", "try_start": 586, "try_end": 597, "handler_start": 597, "handler_end": 600, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"}, {"kind": "Catch", "try_start": 601, "try_end": 614, "handler_start": 614, "handler_end": 617, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"}, {"kind": "Finally", "try_start": 156, "try_end": 2925, "handler_start": 2925, "handler_end": 3091, "filter_start": -1, "catch_type": null}]`.

**KairoLibrary, par 3 (operandos exatos): `brfalse => brtrue.s`; 1 métodos / 3012 bytes.** Exemplo: `kairo.unity.ui.Balloon::Draw(kairo.unity.ui.Graphics,System.Single,System.Single,UnityEngine.Matrix4x4)->System.Void`.

Já compilado no grupo: nenhum candidato chegou à compilação. Recusas e limites antes de compilar: {'incomplete': 18}. Candidatos antigos descartados por busca já esgotada: 54. Nomes individuais das regras recusadas pelo modelo: UNKNOWN (não foram registrados por candidato). Paradas: {'no_model_approved_candidate': 4}.

| IL original | IL recompilado |
| --- | --- |
| `IL_0021: ldarg.1 ` | `IL_0021: ldarg.1 ` |
| `IL_0022: ldfld class kairo.unity.ui.Graphics::render_:class kairo.unity.ui.Image` | `IL_0022: ldfld class kairo.unity.ui.Graphics::render_:class kairo.unity.ui.Image` |
| `IL_0027: brfalse L28` | `IL_0027: brtrue.s L0` |
| `IL_002c: ldc.i4.s 60` | `IL_0029: ret ` |
| `IL_002e: stloc.0 ` | `IL_002a: ldc.i4.s 60` |
|  | `IL_002c: stloc.s 24` |

Regiões de exceção originais: `[{"kind": "Finally", "try_start": 1096, "try_end": 1874, "handler_start": 1874, "handler_end": 1888, "filter_start": -1, "catch_type": null}]`; recompiladas: `[{"kind": "Finally", "try_start": 1091, "try_end": 1865, "handler_start": 1865, "handler_end": 1879, "filter_start": -1, "catch_type": null}]`.

**KairoLibrary, par 4 (operandos exatos): `stloc.2 => stloc.s`; 1 métodos / 2403 bytes.** Exemplo: `kairo.unity.ui.Seb::_load(java.io.InputStream)->System.Void`.

Já compilado no grupo: nenhum candidato chegou à compilação. Recusas e limites antes de compilar: {'incomplete': 13}. Candidatos antigos descartados por busca já esgotada: 39. Nomes individuais das regras recusadas pelo modelo: UNKNOWN (não foram registrados por candidato). Paradas: {'no_model_approved_candidate': 4}.

| IL original | IL recompilado |
| --- | --- |
| `IL_001f: ldloc.0 ` | `IL_001f: ldloc.0 ` |
| ``IL_0020: callvirt class java.io.DataInputStream::ReadShort``0()->valuetype System.Int16;this=True;explicit=False;cc=Default`` | ``IL_0020: callvirt class java.io.DataInputStream::ReadShort``0()->valuetype System.Int16;this=True;explicit=False;cc=Default`` |
| `IL_0025: stloc.2 ` | `IL_0025: stloc.s 4` |
| `IL_0026: br.s L1` | `IL_0027: br.s L1` |
| `IL_0028: ldloc.0 ` | `IL_0029: ldloc.0 ` |

Regiões de exceção originais: `[{"kind": "Finally", "try_start": 8, "try_end": 2390, "handler_start": 2390, "handler_end": 2402, "filter_start": -1, "catch_type": null}]`; recompiladas: `[{"kind": "Finally", "try_start": 8, "try_end": 2388, "handler_start": 2388, "handler_end": 2400, "filter_start": -1, "catch_type": null}]`.

**KairoLibrary, par 5 (operandos exatos): `conv.r4 => DELETION_POINT`; 2 métodos / 2345 bytes.** Exemplo: `kairo.unity.ui.Graphics::_drawImage(kairo.unity.ui.Image,System.Single,System.Single,System.Single,System.Single,System.Int32,System.Int32,System.Single,System.Single)->System.Void`.

Já compilado no grupo: roslyn-solver-group-block-declarations: 1, local-declaration-order-later: 2. Recusas e limites antes de compilar: {'incomplete': 19}. Candidatos antigos descartados por busca já esgotada: 66. Nomes individuais das regras recusadas pelo modelo: UNKNOWN (não foram registrados por candidato). Paradas: {'no_model_approved_candidate': 7, 'no_transformation_advances': 1}.

| IL original | IL recompilado |
| --- | --- |
| `IL_02a0: brfalse L52` | `IL_02a0: brfalse L52` |
| `IL_02a5: ldarg.s 3` | `IL_02a5: ldarg.s 3` |
| `IL_02a7: conv.r4 ` | `IL_02a7: ldarg.s 7` |
| `IL_02a8: ldarg.s 7` | `IL_02a9: div ` |
| `IL_02aa: div ` | `IL_02aa: stloc.s 18` |

Regiões de exceção originais: `[{"kind": "Finally", "try_start": 1114, "try_end": 1164, "handler_start": 1164, "handler_end": 1171, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1978, "try_end": 2028, "handler_start": 2028, "handler_end": 2035, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 666, "try_end": 2232, "handler_start": 2232, "handler_end": 2247, "filter_start": -1, "catch_type": null}]`; recompiladas: `[{"kind": "Finally", "try_start": 1112, "try_end": 1162, "handler_start": 1162, "handler_end": 1169, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 1976, "try_end": 2026, "handler_start": 2026, "handler_end": 2033, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 666, "try_end": 2230, "handler_start": 2230, "handler_end": 2245, "filter_start": -1, "catch_type": null}]`.

**Assembly-CSharp — todos os pares restantes**

| Par alinhado | Métodos | Peso em bytes |
| --- | --- | --- |
| brfalse => brtrue | 11 | 31885 |
| bne.un => beq | 15 | 21664 |
| br => ret | 9 | 21529 |
| stloc.s => stloc.s | 13 | 21449 |
| INSERTION_POINT => stloc.1 | 2 | 12768 |
| ret => br | 7 | 12447 |
| stloc.1 => stloc.s | 2 | 10828 |
| br => br | 7 | 9189 |
| brtrue => stloc.s | 4 | 8527 |
| INSERTION_POINT => stloc.2 | 1 | 7669 |
| INSERTION_POINT => br | 3 | 6720 |
| brtrue => ldc.i4.0 | 2 | 6470 |
| brtrue => brfalse | 6 | 6320 |
| stloc.s => DELETION_POINT | 7 | 6161 |
| stloc.s => dup | 2 | 5401 |
| bge => blt | 3 | 4926 |
| ble => bgt | 4 | 4616 |
| ldarg.0 => dup | 2 | 3838 |
| stloc.2 => stloc.0 | 2 | 3787 |
| brfalse => stloc.s | 1 | 3297 |
| stloc.0 => dup | 2 | 3176 |
| stloc.3 => stloc.1 | 2 | 2955 |
| INSERTION_POINT => stloc.s | 2 | 2565 |
| beq => beq | 1 | 2349 |
| brtrue => brtrue | 1 | 2145 |
| stloc.s => stloc.2 | 2 | 1874 |
| stloc.s => stloc.0 | 2 | 1800 |
| dup => DELETION_POINT | 2 | 1715 |
| ldc.i4.5 => DELETION_POINT | 1 | 1599 |
| br => DELETION_POINT | 7 | 1454 |
| brfalse => DELETION_POINT | 1 | 1342 |
| bne.un => bne.un | 1 | 1308 |
| stloc.2 => stloc.s | 3 | 1263 |
| stloc.s => stloc.1 | 1 | 1029 |
| INSERTION_POINT => ldc.r4 | 1 | 857 |
| stloc.3 => dup | 1 | 758 |
| brtrue => DELETION_POINT | 1 | 723 |
| switch => switch | 1 | 717 |
| INSERTION_POINT => ldc.i4.s | 1 | 707 |
| INSERTION_POINT => ldloc.2 | 1 | 691 |
| INSERTION_POINT => ldloc.s | 1 | 653 |
| ldc.i4.0 => stloc.s | 1 | 623 |
| leave => leave | 1 | 600 |
| blt.un => bge | 1 | 594 |
| bge.un => blt | 1 | 562 |
| ldc.i4.1 => switch | 2 | 556 |
| ldarg.s => DELETION_POINT | 1 | 506 |
| brtrue => stloc.1 | 1 | 447 |
| ldarg.1 => ldloc.s | 1 | 432 |
| ldarg.2 => DELETION_POINT | 1 | 343 |
| INSERTION_POINT => stloc.3 | 1 | 325 |
| switch => ldc.i4.1 | 1 | 322 |
| brfalse => brfalse | 2 | 297 |
| ldc.i4.0 => DELETION_POINT | 1 | 286 |
| ldc.i4.1 => ldc.i4.4 | 1 | 272 |
| blt => blt | 1 | 253 |
| INSERTION_POINT => ldarg.0 | 1 | 241 |
| INSERTION_POINT => leave | 1 | 222 |
| INSERTION_POINT => stloc.0 | 1 | 196 |
| beq => bne.un | 1 | 190 |
| ldloc.3 => dup | 1 | 134 |
| stloc.3 => DELETION_POINT | 1 | 132 |
| ldarg.3 => DELETION_POINT | 1 | 110 |
| stloc.0 => DELETION_POINT | 2 | 109 |
| INSERTION_POINT => ldloc.1 | 1 | 106 |
| IL_ALIGNED_IDENTICAL => IL_ALIGNED_IDENTICAL | 1 | 104 |
| stloc.0 => stloc.s | 1 | 99 |
| brtrue => stloc.0 | 1 | 38 |

Os cinco pares com operandos exatos mais pesados são apresentados também; os endereços de saltos tornam esse agrupamento mais fragmentado que o de opcodes.

| Par exato | Métodos | Peso em bytes |
| --- | --- | --- |
| `{"opcode":"INSERTION_POINT","operand":null} => {"opcode":"stloc.1","operand":null}` | 2 | 12768 |
| `{"opcode":"br","operand":{"branch":12483}} => {"opcode":"ret","operand":null}` | 1 | 12484 |
| `{"opcode":"brfalse","operand":{"branch":780}} => {"opcode":"brtrue","operand":{"branch":779}}` | 1 | 11390 |
| `{"opcode":"stloc.1","operand":null} => {"opcode":"stloc.s","operand":{"local":86}}` | 1 | 9408 |
| `{"opcode":"brfalse","operand":{"branch":97}} => {"opcode":"brtrue","operand":{"branch":200}}` | 1 | 8704 |

**Assembly-CSharp, par 1 (opcodes): `brfalse => brtrue`; 11 métodos / 31885 bytes.** Exemplo: `form.GameForm::_update(System.Int32)->System.Boolean`.

Já compilado no grupo: loop-guard-return-to-continue: 1, solver-group-block-declarations: 35, solver-group-all-calculated-blocks: 8, assignment-ternary-if: 1, if-polarity: 3, condition-split-and: 1, loop-continue-guards: 1, smcs-empty-arm-restore: 7, smcs-owned-switch-if: 1, ternary-polarity: 2, smcs-continue-positive-tail: 1, tail-early-return: 1, condition-local: 1, local-declaration-scope-hoist: 2. Recusas e limites antes de compilar: {'wrong_signature': 318, 'incomplete': 10}. Candidatos antigos descartados por busca já esgotada: 3238. Nomes individuais das regras recusadas pelo modelo: UNKNOWN (não foram registrados por candidato). Paradas: {'no_model_approved_candidate': 188, 'no_transformation_advances': 10}.

| IL original | IL recompilado |
| --- | --- |
| `IL_00d8: ldfld class data.BaseData::id_:valuetype System.Int32` | `IL_00d8: ldfld class data.BaseData::id_:valuetype System.Int32` |
| ``IL_00dd: callvirt class main.AppData::CheckAlreadyEvent``0(valuetype System.Int32)->valuetype System.Boolean;this=True;explicit=False;cc=Default`` | ``IL_00dd: callvirt class main.AppData::CheckAlreadyEvent``0(valuetype System.Int32)->valuetype System.Boolean;this=True;explicit=False;cc=Default`` |
| `IL_00e2: brfalse L5` | `IL_00e2: brtrue R265` |
| `IL_00e7: br L7` | `IL_00e7: ldarg.0 ` |
| `IL_00ec: ldarg.0 ` | `IL_00e8: ldfld class form.MyFormBase::apdat_:class main.AppData` |
| `IL_00ed: ldfld class form.MyFormBase::apdat_:class main.AppData` |  |

**Assembly-CSharp, par 2 (opcodes): `bne.un => beq`; 15 métodos / 21664 bytes.** Exemplo: `form.SpForm::Update()->System.Boolean`.

Já compilado no grupo: terminal-guard-positive-tail: 1, solver-group-block-declarations: 22, if-polarity: 27, condition-split-and: 7, empty-if-terminal-polarity: 1, smcs-empty-arm-restore: 39, smcs-empty-arm-restore-first-and: 15, local-declaration-scope-hoist: 6, solver-group-all-calculated-blocks: 6, smcs-empty-arm-restore-and-suffix: 4, local-declaration-order-earlier: 20, local-declaration-order-later: 30, guard-return-reverse: 1, smcs-loop-condition-body-guard: 1, loop-continue-guard: 5, smcs-continue-positive-tail: 3, ternary-polarity: 1, assignment-ternary-if: 1, tail-early-return: 2, condition-split-or-guards: 3, declaration-ternary-if: 1, declaration-ternary-if-reverse: 1, loop-continue-guards: 1. Recusas e limites antes de compilar: {'wrong_signature': 720, 'incomplete': 5}. Candidatos antigos descartados por busca já esgotada: 9029. Nomes individuais das regras recusadas pelo modelo: UNKNOWN (não foram registrados por candidato). Paradas: {'no_model_approved_candidate': 245, 'no_transformation_advances': 26}.

| IL original | IL recompilado |
| --- | --- |
| `IL_01e4: ldloc.s 4` | `IL_01e4: ldloc.s 4` |
| `IL_01e6: ldc.i4.s -3` | `IL_01e6: ldc.i4.s -3` |
| `IL_01e8: bne.un O518` | `IL_01e8: beq R675` |
| `IL_01ed: ldarg.0 ` | `IL_01ed: ldloc.s 4` |
| `IL_01ee: ldfld class form.MyFormBase::canvas_:class kairo.unity.ui.Canvas` | `IL_01ef: stloc.s 25` |
| `IL_01f3: ldarg.0 ` | `IL_01f1: ldloc.s 25` |
| `IL_01f4: ldfld class form.SpForm::text_:class System.String` | `IL_01f3: ldc.i4.4 ` |
| `IL_01f9: ldc.i4.0 ` | `IL_01f4: add ` |
| `IL_01fa: ldc.i4.1 ` | `IL_01f5: switch L10,L13,L12,L11` |
| ``IL_01fb: callvirt class kairo.unity.ui.Canvas::ImeOnBlock``0(class System.String,valuetype System.Int32,valuetype System.Int32)->class System.String;this=True;explicit=False;cc=Default`` | `IL_020a: br L13` |
| `IL_0200: pop ` | `IL_020f: ldarg.0 ` |
| `IL_0201: br L16` |  |
| `IL_0206: ldloc.s 4` |  |

Regiões de exceção originais: `[{"kind": "Catch", "try_start": 1796, "try_end": 1972, "handler_start": 1972, "handler_end": 1978, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Catch", "try_start": 1641, "try_end": 1657, "handler_start": 1657, "handler_end": 1663, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Catch", "try_start": 1344, "try_end": 1597, "handler_start": 1597, "handler_end": 1603, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Finally", "try_start": 1286, "try_end": 1623, "handler_start": 1623, "handler_end": 1641, "filter_start": -1, "catch_type": null}, {"kind": "Catch", "try_start": 1063, "try_end": 1235, "handler_start": 1235, "handler_end": 1241, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Catch", "try_start": 909, "try_end": 1983, "handler_start": 1983, "handler_end": 1989, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Catch", "try_start": 787, "try_end": 803, "handler_start": 803, "handler_end": 809, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}]`; recompiladas: `[{"kind": "Catch", "try_start": 1799, "try_end": 1975, "handler_start": 1975, "handler_end": 1981, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Catch", "try_start": 1644, "try_end": 1660, "handler_start": 1660, "handler_end": 1666, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Catch", "try_start": 1347, "try_end": 1600, "handler_start": 1600, "handler_end": 1606, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Finally", "try_start": 1294, "try_end": 1626, "handler_start": 1626, "handler_end": 1644, "filter_start": -1, "catch_type": null}, {"kind": "Catch", "try_start": 1071, "try_end": 1243, "handler_start": 1243, "handler_end": 1249, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Catch", "try_start": 917, "try_end": 1986, "handler_start": 1986, "handler_end": 1992, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Catch", "try_start": 795, "try_end": 811, "handler_start": 811, "handler_end": 817, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}]`.

**Assembly-CSharp, par 3 (opcodes): `br => ret`; 9 métodos / 21529 bytes.** Exemplo: `form.SoccerForm::Draw_newPlayerPopular(kairo.unity.ui.Graphics)->System.Void`.

Já compilado no grupo: terminal-guard-positive-tail: 3, solver-group-block-declarations: 30, solver-group-all-calculated-blocks: 6, extended-original-zero-chain: 2, void-empty-then-common-exit: 1, smcs-empty-arm-restore: 13, smcs-empty-arm-restore-first-and: 3, smcs-empty-arm-restore-and-suffix: 5, assignment-ternary-if: 1, assignment-ternary-if-reverse: 1, tail-early-return: 3, condition-split-and: 2, local-declaration-scope-hoist: 7, void-common-exit-if-else: 1, solver-group-original-zero-chain: 1, condition-split-or-guards: 1. Recusas e limites antes de compilar: {'wrong_signature': 98}. Candidatos antigos descartados por busca já esgotada: 1173. Nomes individuais das regras recusadas pelo modelo: UNKNOWN (não foram registrados por candidato). Paradas: {'no_model_approved_candidate': 146, 'no_transformation_advances': 16}.

| IL original | IL recompilado |
| --- | --- |
| `IL_020a: ldelem.i4 ` | `IL_020a: ldelem.i4 ` |
| `IL_020b: bge L3` | `IL_020b: bge L3` |
| `IL_0210: br L5` | `IL_0210: ret ` |
| `IL_0215: ldarg.0 ` | `IL_0211: ldarg.0 ` |
| `IL_0216: ldfld class form.SoccerForm::stateCnt_:valuetype System.Int32` | `IL_0212: ldfld class form.SoccerForm::stateCnt_:valuetype System.Int32` |

**Assembly-CSharp, par 4 (opcodes): `stloc.s => stloc.s`; 13 métodos / 21449 bytes.** Exemplo: `form.BootForm::DivServerData(System.String)->System.Byte[][]`.

Já compilado no grupo: nenhum candidato chegou à compilação. Recusas e limites antes de compilar: {'incomplete': 46, 'wrong_signature': 205}. Candidatos antigos descartados por busca já esgotada: 3990. Nomes individuais das regras recusadas pelo modelo: UNKNOWN (não foram registrados por candidato). Paradas: {'no_model_approved_candidate': 234}.

| IL original | IL recompilado |
| --- | --- |
| `IL_0035: ldloc.2 ` | `IL_0035: ldloc.2 ` |
| `IL_0036: sub ` | `IL_0036: sub ` |
| `IL_0037: stloc.s 7` | `IL_0037: stloc.s 8` |
| `IL_0039: ldloc.3 ` | `IL_0039: ldloc.3 ` |
| `IL_003a: brfalse L1` | `IL_003a: brfalse R76` |

Regiões de exceção originais: `[{"kind": "Catch", "try_start": 237, "try_end": 249, "handler_start": 249, "handler_end": 255, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Catch", "try_start": 11, "try_end": 215, "handler_start": 215, "handler_end": 230, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Finally", "try_start": 11, "try_end": 230, "handler_start": 230, "handler_end": 259, "filter_start": -1, "catch_type": null}]`; recompiladas: `[{"kind": "Catch", "try_start": 236, "try_end": 248, "handler_start": 248, "handler_end": 254, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Catch", "try_start": 11, "try_end": 214, "handler_start": 214, "handler_end": 229, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Finally", "try_start": 11, "try_end": 229, "handler_start": 229, "handler_end": 258, "filter_start": -1, "catch_type": null}]`.

**Assembly-CSharp, par 5 (opcodes): `INSERTION_POINT => stloc.1`; 2 métodos / 12768 bytes.** Exemplo: `form.SubForm::Update2()->System.Boolean`.

Já compilado no grupo: nenhum candidato chegou à compilação. Recusas e limites antes de compilar: {'wrong_signature': 366, 'prediction_budget_remaining': 312}. Candidatos antigos descartados por busca já esgotada: 8790. Nomes individuais das regras recusadas pelo modelo: UNKNOWN (não foram registrados por candidato). Paradas: {'no_model_approved_candidate': 35, 'model_prediction_limit': 1}.

| IL original | IL recompilado |
| --- | --- |
| `IL_001f: ldc.i4 131072` | `IL_001f: ldc.i4 131072` |
| ``IL_0024: callvirt class kairo.unity.ui.Canvas::CheckKeyRepeat``0(valuetype System.Int32)->valuetype System.Boolean;this=True;explicit=False;cc=Default`` | ``IL_0024: callvirt class kairo.unity.ui.Canvas::CheckKeyRepeat``0(valuetype System.Int32)->valuetype System.Boolean;this=True;explicit=False;cc=Default`` |
| `IL_0029: brfalse L0` | `IL_0029: stloc.1 ` |
| `IL_002e: ldarg.0 ` | `IL_002a: ldloc.1 ` |
| `IL_002f: ldarg.0 ` | `IL_002b: brfalse L0` |
|  | `IL_0030: ldarg.0 ` |

Regiões de exceção originais: `[{"kind": "Catch", "try_start": 10749, "try_end": 10831, "handler_start": 10831, "handler_end": 10897, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Finally", "try_start": 10647, "try_end": 10703, "handler_start": 10703, "handler_end": 10715, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 10561, "try_end": 10618, "handler_start": 10618, "handler_end": 10630, "filter_start": -1, "catch_type": null}]`; recompiladas: `[{"kind": "Catch", "try_start": 10704, "try_end": 10786, "handler_start": 10786, "handler_end": 10852, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Finally", "try_start": 10607, "try_end": 10663, "handler_start": 10663, "handler_end": 10675, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 10521, "try_end": 10578, "handler_start": 10578, "handler_end": 10590, "filter_start": -1, "catch_type": null}]`.

**Assembly-CSharp, par 1 (operandos exatos): `INSERTION_POINT => stloc.1`; 2 métodos / 12768 bytes.** Exemplo: `form.SubForm::Update2()->System.Boolean`.

Já compilado no grupo: nenhum candidato chegou à compilação. Recusas e limites antes de compilar: {'wrong_signature': 366, 'prediction_budget_remaining': 312}. Candidatos antigos descartados por busca já esgotada: 8790. Nomes individuais das regras recusadas pelo modelo: UNKNOWN (não foram registrados por candidato). Paradas: {'no_model_approved_candidate': 35, 'model_prediction_limit': 1}.

| IL original | IL recompilado |
| --- | --- |
| `IL_001f: ldc.i4 131072` | `IL_001f: ldc.i4 131072` |
| ``IL_0024: callvirt class kairo.unity.ui.Canvas::CheckKeyRepeat``0(valuetype System.Int32)->valuetype System.Boolean;this=True;explicit=False;cc=Default`` | ``IL_0024: callvirt class kairo.unity.ui.Canvas::CheckKeyRepeat``0(valuetype System.Int32)->valuetype System.Boolean;this=True;explicit=False;cc=Default`` |
| `IL_0029: brfalse L0` | `IL_0029: stloc.1 ` |
| `IL_002e: ldarg.0 ` | `IL_002a: ldloc.1 ` |
| `IL_002f: ldarg.0 ` | `IL_002b: brfalse L0` |
|  | `IL_0030: ldarg.0 ` |

Regiões de exceção originais: `[{"kind": "Catch", "try_start": 10749, "try_end": 10831, "handler_start": 10831, "handler_end": 10897, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Finally", "try_start": 10647, "try_end": 10703, "handler_start": 10703, "handler_end": 10715, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 10561, "try_end": 10618, "handler_start": 10618, "handler_end": 10630, "filter_start": -1, "catch_type": null}]`; recompiladas: `[{"kind": "Catch", "try_start": 10704, "try_end": 10786, "handler_start": 10786, "handler_end": 10852, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Finally", "try_start": 10607, "try_end": 10663, "handler_start": 10663, "handler_end": 10675, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 10521, "try_end": 10578, "handler_start": 10578, "handler_end": 10590, "filter_start": -1, "catch_type": null}]`.

**Assembly-CSharp, par 2 (operandos exatos): `br => ret`; 1 métodos / 12484 bytes.** Exemplo: `form.SubForm::_draw4(kairo.unity.ui.Graphics)->System.Void`.

Já compilado no grupo: nenhum candidato chegou à compilação. Recusas e limites antes de compilar: {'wrong_signature': 12}. Candidatos antigos descartados por busca já esgotada: 195. Nomes individuais das regras recusadas pelo modelo: UNKNOWN (não foram registrados por candidato). Paradas: {'no_model_approved_candidate': 18}.

| IL original | IL recompilado |
| --- | --- |
| `IL_1070: ldc.i4.s 48` | `IL_1070: ldc.i4.s 48` |
| `IL_1072: bne.un L53` | `IL_1072: bne.un L53` |
| `IL_1077: br L156` | `IL_1077: ret ` |
| `IL_107c: ldarg.0 ` | `IL_1078: ldarg.0 ` |
| `IL_107d: ldfld class form.SubForm::type_:valuetype System.Int32` | `IL_1079: ldfld class form.SubForm::type_:valuetype System.Int32` |

**Assembly-CSharp, par 3 (operandos exatos): `brfalse => brtrue`; 1 métodos / 11390 bytes.** Exemplo: `form.SubForm::Update()->System.Boolean`.

Já compilado no grupo: nenhum candidato chegou à compilação. Recusas e limites antes de compilar: {'wrong_signature': 15}. Candidatos antigos descartados por busca já esgotada: 255. Nomes individuais das regras recusadas pelo modelo: UNKNOWN (não foram registrados por candidato). Paradas: {'no_model_approved_candidate': 18}.

| IL original | IL recompilado |
| --- | --- |
| `IL_02f9: stloc.3 ` | `IL_02f9: stloc.3 ` |
| ``IL_02fa: call class form.MyFormBase::Jp``0()->valuetype System.Boolean;this=False;explicit=False;cc=Default`` | ``IL_02fa: call class form.MyFormBase::Jp``0()->valuetype System.Boolean;this=False;explicit=False;cc=Default`` |
| `IL_02ff: brfalse O780` | `IL_02ff: brtrue L17` |
| `IL_0304: ldc.i4.s 104` | `IL_0304: ldc.i4.s 124` |
| `IL_0306: stloc.3 ` | `IL_0306: br L18` |
|  | `IL_030b: ldc.i4.s 104` |
|  | `IL_030d: stloc.3 ` |

Regiões de exceção originais: `[{"kind": "Catch", "try_start": 5640, "try_end": 5656, "handler_start": 5656, "handler_end": 5662, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Catch", "try_start": 5402, "try_end": 5429, "handler_start": 5429, "handler_end": 5667, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Catch", "try_start": 4924, "try_end": 4956, "handler_start": 4956, "handler_end": 4981, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Catch", "try_start": 4412, "try_end": 4428, "handler_start": 4428, "handler_end": 4453, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}]`; recompiladas: `[{"kind": "Catch", "try_start": 5651, "try_end": 5667, "handler_start": 5667, "handler_end": 5673, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Catch", "try_start": 5413, "try_end": 5440, "handler_start": 5440, "handler_end": 5678, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Catch", "try_start": 4935, "try_end": 4967, "handler_start": 4967, "handler_end": 4992, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Catch", "try_start": 4427, "try_end": 4443, "handler_start": 4443, "handler_end": 4468, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}]`.

**Assembly-CSharp, par 4 (operandos exatos): `stloc.1 => stloc.s`; 1 métodos / 9408 bytes.** Exemplo: `game.TPlayer::Script()->System.Boolean`.

Já compilado no grupo: nenhum candidato chegou à compilação. Recusas e limites antes de compilar: {'wrong_signature': 245, 'incomplete': 9, 'prediction_budget_remaining': 174}. Candidatos antigos descartados por busca já esgotada: 6864. Nomes individuais das regras recusadas pelo modelo: UNKNOWN (não foram registrados por candidato). Paradas: {'no_model_approved_candidate': 17, 'model_prediction_limit': 1}.

| IL original | IL recompilado |
| --- | --- |
| `IL_0014: ldc.i4.0 ` | `IL_0014: ldc.i4.0 ` |
| `IL_0015: ldelem.i4 ` | `IL_0015: ldelem.i4 ` |
| `IL_0016: stloc.1 ` | `IL_0016: stloc.s 86` |
| `IL_0017: ldloc.1 ` | `IL_0018: ldloc.s 86` |
| `IL_0018: ldc.i4.1 ` | `IL_001a: ldc.i4.1 ` |
| `IL_0019: bne.un O100` | `IL_001b: sub ` |

Regiões de exceção originais: `[{"kind": "Finally", "try_start": 12, "try_end": 9378, "handler_start": 9378, "handler_end": 9386, "filter_start": -1, "catch_type": null}]`; recompiladas: `[{"kind": "Finally", "try_start": 12, "try_end": 9308, "handler_start": 9308, "handler_end": 9316, "filter_start": -1, "catch_type": null}]`.

**Assembly-CSharp, par 5 (operandos exatos): `brfalse => brtrue`; 1 métodos / 8704 bytes.** Exemplo: `form.SubForm::_draw(kairo.unity.ui.Graphics)->System.Void`.

Já compilado no grupo: nenhum candidato chegou à compilação. Recusas e limites antes de compilar: {'wrong_signature': 34}. Candidatos antigos descartados por busca já esgotada: 478. Nomes individuais das regras recusadas pelo modelo: UNKNOWN (não foram registrados por candidato). Paradas: {'no_model_approved_candidate': 18}.

| IL original | IL recompilado |
| --- | --- |
| `IL_0051: ldarg.0 ` | `IL_0051: ldarg.0 ` |
| ``IL_0052: call class form.SubForm::IsDrawWindow``0()->valuetype System.Boolean;this=True;explicit=False;cc=Default`` | ``IL_0052: call class form.SubForm::IsDrawWindow``0()->valuetype System.Boolean;this=True;explicit=False;cc=Default`` |
| `IL_0057: brfalse L3` | `IL_0057: brtrue L5` |
| `IL_005c: br L5` | `IL_005c: ldarg.0 ` |
| `IL_0061: ldarg.0 ` | ``IL_005d: call class form.SubForm::IsMenu``0()->valuetype System.Boolean;this=True;explicit=False;cc=Default`` |
| ``IL_0062: call class form.SubForm::IsMenu``0()->valuetype System.Boolean;this=True;explicit=False;cc=Default`` |  |

## 5. TOOLCHAIN

Nenhum compilador ou referência de produção foi trocado nesta rodada. Identidade exata da distribuição original, comando/opções históricos completos e arquivos de referência exatos usados originalmente: UNKNOWN. As identidades declaradas em AssemblyRef são observáveis; isso não prova o hash de cada arquivo histórico. A marca Repro original está ausente; o modo determinístico original permanece UNKNOWN. As recompilações selecionadas são não determinísticas, conforme as duas compilações e seis campos documentados.

**KairoLibrary**: Microsoft.Net.Compilers 1.1.0; csc.exe 1.1.0.51109; SHA-256 `53b693536e8a86f35d05d2165ff9b3e0aeaa499ecfd8135a9f99ab4404ca86c3`.

Opções: `/nologo /noconfig /nostdlib+ /target:library /platform:anycpu /optimize+ /debug:pdbonly /deterministic- /langversion:6 /unsafe- /checked- /codepage:65001 /utf8output`.

Referências, na ordem: `C:\Windows\Microsoft.NET\Framework\v2.0.50727\mscorlib.dll` (SHA-256 `c4729c6fc2279933fa9638f60775ab2f7eb731d5b04f9e04c15187afbdf94558`), `C:\Users\l_abn\AppData\Local\Programs\Unity\5.3.4f1\Editor\Data\Mono\lib\mono\2.0\System.Core.dll` (SHA-256 `4998eda31c3d7bebf606f98cef77ad697f9e82e629efc37c06517019c2c2c271`), `C:\Users\l_abn\Documents\Projetos\Pocket League Story\original\extracted\assets\bin\Data\Managed\UnityEngine.dll` (SHA-256 `7cc901371cc77e103d00b5845e8285f85fbdf36bb47ccf3d824f94c4ead32421`), `C:\Users\l_abn\AppData\Local\Programs\Unity\5.3.4f1\Editor\Data\Mono\lib\mono\2.0\System.dll` (SHA-256 `c2225d7ba58a12e22c266384d3c1e4e882ab4e42449b91b721c21b5f1fcd63be`).

**Assembly-CSharp**: smcs.exe 2.6.5.0 distribuído no Unity 5.3.4f1; SHA-256 `26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0`.

Opções: `-nologo -noconfig -target:library -platform:anycpu -optimize+ -debug- -langversion:Default -unsafe- -checked- -codepage:65001`.

Referências, na ordem: `C:\Users\l_abn\Documents\Projetos\Pocket League Story\original\extracted\assets\bin\Data\Managed\mscorlib.dll` (SHA-256 `7f16171cf5f87ca1e4c229727526d2fbc786079fc0c960f36197013ea0d986e1`), `C:\Users\l_abn\Documents\Projetos\Pocket League Story\original\extracted\assets\bin\Data\Managed\KairoLibrary.dll` (SHA-256 `bb553d341eecd9b900f772486bf4a5c289aa2f09566d9d402a3eb70268f4b3ee`), `C:\Users\l_abn\Documents\Projetos\Pocket League Story\original\extracted\assets\bin\Data\Managed\UnityEngine.dll` (SHA-256 `7cc901371cc77e103d00b5845e8285f85fbdf36bb47ccf3d824f94c4ead32421`), `C:\Users\l_abn\Documents\Projetos\Pocket League Story\original\extracted\assets\bin\Data\Managed\System.Core.dll` (SHA-256 `104278a0bf67b4de8fd1e910e3292a3ef51808ebee7a5f2a229125f18463c98c`), `C:\Users\l_abn\Documents\Projetos\Pocket League Story\original\extracted\assets\bin\Data\Managed\System.dll` (SHA-256 `7e72b92ddcd6b7b791ab24dd5756544f2da81f65e7ba7d1ebe8223123720d0ac`).

Runtime Mono: `C:\Users\l_abn\AppData\Local\Programs\Unity\5.3.4f1\Editor\Data\Mono\bin\mono.exe`; SHA-256 `3d102d9a63dae72e48396998bdbae4e9b2d8952ca6a8ebbcba698d3bc4345076`. Ligação da biblioteca padrão: `default`. O mscorlib implícito é o perfil Unity 2.0.5.0, conferido contra o APK. Ambiente somente do subprocesso: `{"MONO_PATH": "C:\\Users\\l_abn\\AppData\\Local\\Programs\\Unity\\5.3.4f1\\Editor\\Data\\Mono\\lib\\mono\\unity", "MONO_CFG_DIR": "C:\\Users\\l_abn\\AppData\\Local\\Programs\\Unity\\5.3.4f1\\Editor\\Data\\Mono\\etc"}`.

O Roslyn moderno compila os auxiliares e fornece AST/semântica. Não emite os assemblies alvos. PDB Roslyn e MDB smcs auxiliam o mapeamento; a comparação integral continua independente dos símbolos. Nenhum nome escolhido para local é apresentado como nome original.

## 6. COMPONENTES EXTERNOS E METADADOS

Os 466 tipos continuam com metadados estruturais iguais (KairoLibrary 337/337; Assembly-CSharp 129/129): ordem de tipos e membros, parâmetros, atributos, layout, constantes e recursos. A comparação estrutural global também passa, incluindo referências de assemblies e módulos, atributos e recursos. As 82 declarações sem corpo continuam aprovadas. Tabelas/heaps/PE brutos: NOT_EVALUATED nesta rodada; a distribuição atual das diferenças brutas é UNKNOWN. Os hashes integrais dos arquivos diferem. Nenhuma normalização ou correção baseada em bytes brutos foi feita.

UnityEngine.dll corresponde em todos os 970.752 bytes à variante oficial Android/Mono do Unity 5.3.4f1 (SHA-256 `7cc901371cc77e103d00b5845e8285f85fbdf36bb47ccf3d824f94c4ead32421`). As outras seis DLLs externas verificadas são Mono.Security, System.Core, System, UnityEngine.Networking, UnityEngine.UI e mscorlib. Os seis externos nativos são libmain.so, libmono.so e libunity.so, nas arquiteturas armeabi-v7a e x86; permanecem conferidos contra a distribuição oficial. DEX/Java permanece fora do escopo.

Exceções não determinísticas, comprovadas anteriormente por duas compilações do mesmo fonte: KairoLibrary — Module.Mvid, COFF.TimeDateStamp, IMAGE_DEBUG_DIRECTORY.TimeDateStamp e CodeView.RSDS.PdbGuid; Assembly-CSharp — Module.Mvid e COFF.TimeDateStamp. São seis campos; nenhum outro é normalizado.

## 7. BLOQUEIOS

Nenhuma instalação, arquivo adicional ou decisão de escopo depende do usuário. Permanecem limites técnicos: fontes ainda divergentes, cobertura parcial dos modelos e três auxiliares com corpo igual mas vínculo gerado ainda pendente. Os ciclos 18/19 tiveram seis e uma previsões incompletas, respectivamente, e zero candidatos compilados dessas famílias. A falta de espaço e as falhas de envio foram resolvidas, com recuperação documentada e verificações repetidas.

## 8. SUA RECOMENDAÇÃO

1. Generalizar o solver por âncoras de chamadas/stores e intervalos de vida para os 13 métodos smcs do grupo stloc.s=>stloc.s, que somam 21449 bytes de IL. Escopo potencial medido: até 13 métodos; quantos fecharão é UNKNOWN. A combinação de cálculo de slots e outras regras fechou _draw3, _draw5 e _draw6 nesta rodada, sustentando a prioridade.
2. Testar a família já validada em fixture smcs-value-return-guard-reverse em form.SpForm.Update, não selecionada nos vinte ciclos. Um alvo medido, 2052 bytes; estimativa conservadora de 0–1 novo matched, sem aprovação antecipada. O IL original coloca o ramo de ImeOnBlock antes do switch e o fonte recuperado os inverte.
3. Ampliar de forma conservadora o modelo Roslyn para void try/finally e locais que precisam sobreviver ao finally, revalidando todo o corpus antes de adotar. Em Graphics.DrawPolygons, uma composição de fontes em memória prevê os 35 tipos originais, mas continua incompleta por EH e scheduler. Potencial direto de 0–1 método, 1563 bytes; correspondência do corpo permanece UNKNOWN. Essa é uma extensão delimitada para reduzir recusas do modelo, sem trocar compilador ou referências.
