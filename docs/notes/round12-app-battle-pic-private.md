# Draw_battlePic — calculated declarations, five-zero chain and two direct dispatches

Finite private B4 study only. `form.SubForm::Draw_battlePic(kairo.unity.ui.Graphics,Int32)->Void`, original token06000232,2263 IL; current2288 IL. Both existing headers are fat/maxstack164/initlocals, no EH. **No game compilation, game execution, verifier, public/source/config/ledger edit or commit. Target equality remains UNKNOWN.** A complete target method proposal is not itself a full-match claim.

Frozen source/DTOs come from `build/round12/runs/a11/commit/postverify01/Assembly-CSharp`; the complete SubForm.cs hash is checked against the live file at packaging. `source-patches.json` binds UTF16 declaration offsets and complete before/after/source SHA. Source fields/calls/float expressions remain in place; no metadata member is renamed/reordered and no generated class is authored.

## Primary evidence and history

Fresh REA static page `rea-original-method.json` is bound to original APK DLL SHA3620ed91..., MVID48ce9f89..., method06000232:2263 IL,max164,958/958 decoded instructions, no EH, raw IL SHA27e4b6f62682f84756407dc72ab36de6e05f52d6f52f94fd3328e45719b99acc. Packaging independently hashes the frozen original CIL slice and asserts equality to REA. No original target code is loaded/run.

The bounded history scan in `history.json` covers71 retained R7–R12 Assembly-CSharp engine reports for this exact stable ID, with zero actual trials/steps. This is absence of recorded emitted candidates in that scope, not refutation of any source form or proof that no equivalent was ever written elsewhere. Source before/after hashes are preserved. The four generic solver outputs in `solver-before.json` were calculated but neither compiled nor promoted as READY; only the complete source composition in the explicit bundle has the prediction and own-fixture evidence below.

## Slot/liveness calculation

Original26 locals versus current27. The existing real source-only model11 is complete and exactly predicts all27 current emitted types. The solver's family/type/use alignment supplies tentative correspondences, and the following independent IL stores/uses anchor the important roles (`slot-lifetimes.json`, `role-constraints.json`):

| Current source role | Current slot | Desired original slot | Original first/last access | Evidence |
|---|---:|---:|---|---|
| num4 (render offset)|7|5|100..2230|RateConvert result, repeated zero stores and matching later loads|
| num5 (x coordinate)|13|6|111..2225|Initial zero at111, overwrite4 at375 before first load401; current lacks only that first zero store|
| num (y coordinate)|8|7|109..2234|25/9/32 stadium-arm values and all later drawing loads|
| num2 (home/away x)|6|8|106..985|57/183 assignments and following DrawSeb argument|
| num3 (zero-only reservation)|5|9|103..103|One zero store, zero loads in the entire original body|
| teamData/teamData2|9/10|10/11|134..1319 /157..1336|Distinct team array lookups and corresponding later l branches|
| matchInfoJoin|11|12|166..1575|GetMatchInfoJoin evaluated once, retained named selector used later|
| questMatchData|12|13|222..283|Case1-only object and name read|

All remaining named roles14..25 already correspond to their desired original slots; in particular rankIndex remains25. Original slot25 is **not** a switch cache: it has store2150 and loads2156/2222 tied to GetRankIndex and RANK_NAME. The extra current slot26 is the Int32 switch temporary, stored in the first dispatch167 and reused in the l dispatch1325. Its lifetime and role are distinct from both named selectors.

The desired declaration order is therefore `num4,num5,num,num2,num3`, after UserData and before the team declarations. Hoist only bare num5; its existing initialized declaration `int num5=4` becomes assignment `num5=4` at the same execution position. Do not move its4 initializer before team lookup/dispatch.

Original98..111 is `ldc0;dup;stloc5;dup;stloc9;dup;stloc8;dup;stloc7;stloc6`. Because chained assignment stores from the inside out, the normal C# statement is `num5 = num = num2 = num3 = num4 = 0;`. The currently recovered four-variable chain omits num5. This is not an arbitrary insertion to make a signature match: original111 explicitly stores zero to that role, and its later375 overwrite makes that first store observationally dead but still byte-required. Slot9's historical variable name is UNKNOWN; the readable decompiler name num3 is retained, not claimed recovered.

## Two independent selector caches must disappear in the same composition

First original dispatch: named selector12 is stored166, loaded168 and tested by170brtrue200, then the0 arm, join195br355 and1 comparison203bne.un355. This is direct if/else-if0/1. Current copies named selector11 into temporary26 before comparisons and adds default/section jumps. Keep GetMatchInfoJoin's named local and single evaluation; do not inline its call repeatedly.

Second original dispatch: named for index20 is loaded1312,1314brtrue1328, sets teamData3 from teamData,1323br1340, then the1 comparison1331bne.un1340 sets teamData2. Current again copies l into the same temp26. Restore only this0/1 if/else-if; preserve TeamData3's existing null initialization and all surrounding loop statements. The producer verifies both selectors/section lists, preserves the explicit case1 child block, and refuses ambiguous/nonterminal break owners.

Pinned Unity Mono source b6587926c0f146c8b12985744f33a13b39a22d57 explains the extra storage: `Switch.DoEmit`3825–3869 unconditionally creates/stores a LocalTemporary for nonconstant selectors, even an ordinary named local (the source TODO explicitly identifies this redundant copy); it releases that temp afterward, permitting reuse by the later switch. If.DoEmit233–269 emits body-interleaved comparisons instead. Block.EmitMeta2010–2052 records declared variables/registered temporaries/children; actual declaration order is confirmed by model and fixtures, not guessed from names or hash iteration. `compiler-citations.json` gives full source SHA/lines. Exact installed compiler manufacturing revision remains UNKNOWN; pinned installed smcs2.6.5 emission is tested directly on own fixtures.

## Model and own-fixture verification

`calculate.py::transform_battle_pic(method_text)` is pure, header-name independent and uses only the finite normal C# templates described above. Literal multiset260 and call-identifier multiset94 are identical before/after. All float arithmetic, signs, operands and actual drawing expressions are copied unchanged; there is no floating-point comparison or NaN/.un rewrite.

Real NoEmit after the **whole composition** is complete26, exactly equal to all original types. Every one of26 named variables lands in its calculated desired original slot (`role-constraints.json`), not just a matching unordered inventory. No model code, diagnostic or completeness flag was modified. Removing only one switch would retain a wrong27-local candidate and must be rejected by the normal gate.

Six owned smcs libraries were compiled from new mock classes/methods with the pinned flags/runtime/references. The focused fixture includes the same26 named roles and both dispatches; it is not a compiled game class. Whole-body comparison includes header/locals/resolved IL bytes/EH:

| Own variant | IL bytes | Locals | Equals independent handwritten control? |
|---|---:|---:|---|
| Before|605|27|No|
| Handwritten Control|580|26|Yes|
| Pure-calculator Candidate|580|26|Yes|
| Missing fifth zero store|577|26|No|
| Swapped num4/num5 declarations|580|26|No|
| Second switch remains|594|27|No|

All positives have header/maxstack26/no EH exactly equal to the independent control; candidate is exact across its entire body. Three complete NoEmit predictions match all actual owned local types. The three runtime positive variants returned semantic0 for20 frame/selector combinations, including unsupported selector values, with GetMatchInfoJoin checked to execute exactly once. The three emitted negatives show why equal size/count or partial cache removal is insufficient.

Six producer negatives are refused: property selector, repeated selector invocation, nonterminal break, default arm, different zero literal, changed late initializer. No bad game source was compiled. The first packaging call-inventory check incorrectly included punctuation before `(` as if it were a call; restricting that diagnostic to identifier tokens fixed the check without changing source/calculator or compiling again.

The net own size change is−25; target current2288 versus original2263 also differs by25. This supports the proposed layout/control accounting but does **not** prove the uncompiled target bytes or maxstack. The production owner must rebind live source, run genuine fresh NoEmit, isolated target probe, whole verifier and regression checks. This worker stops after this target bundle.
