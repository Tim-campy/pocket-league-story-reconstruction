# Player.Collision V2 â€” remaining control reconstruction, private only

Scope is only `game.Player::Collision()->Void`, original token06000325,2145 IL. This is a **new V2 folder**; V1 at `build/round12/app-collision-private/freeze.json` is preserved. Every V1 frozen file was rehashed before preparation and again during packaging. `v1_frozen.py` is a byte-identical private copy of its pure producer, not an edit to V1. No generated class is authored.

**Zero target/game compilation, execution, verifier, public/source/config/ledger modification or commit.** The normal C# proposal remains UNKNOWN for the game. Production belongs to the designated operator after the queue. Existing original/current headers are measured, not guesses: original fat/maxstack67/initlocals/2145 IL; current fat/maxstack69/initlocals/2153 IL. Candidate target maxstack/header/IL size are not claimed without emission.

## Inputs and finite producer

Frozen source/DTO input is `build/round12/runs/a11/commit/postverify01/Assembly-CSharp`; Player.cs equals the live file at packaging. `source-patches.json` contains the exact current declaration before text, full source SHA, UTF16 spans and complete after declaration. API `calculate.py::transform_collision_v2(method_text)` first applies the frozen V1 and then the confirmed-source-form V2. It is pure and header-name independent. `transform_controls(v1_method_text)` and the three region functions are separately available for review. Producer dependency is `v1_frozen.py`, whose SHA matches V1's producer.

Fresh REA static summary/page bind original artifact3620ed91..., MVID48ce9f89..., method06000325,2145 IL,maxstack67,703/703 instructions and raw CIL SHAbece5d8ba59db317e38f89c527b6c46e402b3f1b84e6465953cea293d6e9ccd5. Packaging recomputed that hash from the frozen original body. REA is not target execution. `analysis.json` retains original/current DTOs and aligned sequence edits; no target V1/V2 assembly exists in this study.

The bounded history scan covers74 retained R7â€“R12 Assembly-CSharp engine reports for this exact method, with0 actual trials/steps. This is not a refutation or an assertion about every possible older source. V1's independently measured own fixtures and prediction are preserved as prior evidence, not relabeled as V2 tests.

## Original control anchors and ownership

`original-control-anchors.json` retains the relevant branch instructions. Two corrections to the initial task reading are explicit:

- `brfalse1047` at367 does **not** exit the whole loop.1047 starts `ldloc4;ldc1;add;stloc4`, the j increment; the test at1074 branches to84. The false edge skips the remaining current iteration. The positive hit body therefore encloses only that iteration's remaining statements, followed by the unchanged increment.
-1366 is a `bne.un1383` in the debug condition. The actual DEBUG_GK_MODE/team0 return is **1382 (0x566)**. That exact source guard and return remain untouched.

Other anchors and their normal source forms:

1. Original86brtrue96/91br1047 implements `if(j==0)continue`. The subsequent103bne.un126,116bne.un126 and121br1047 implements the separate self-player continue. The recovered OR merges these two syntactic guards; V2 splits them, preserving short-circuit access order.

2. Original172blt182/177br1047,206blt216/211br1047 and233brfalse243/238br1047 are three successive continues for AbsX>=2,AbsY>=2 and the flag check. These are Int32 address distances and Boolean checks; no floating-point comparison operator is rewritten here.

3. Original367brfalse1047 directly skips a **positive** ChRectHitRect region. The recovered negative guard plus explicit continue adds a branch. V2 uses the same Boolean call/arguments, as a positive if enclosing the remaining body of this j iteration. Every existing numeric calculation, field assignment and nested flag/body-hit block is copied without arithmetic edits.

4. Original459bge.un469/464br1047 and476bge.un486/481br1047 are separate guards `if(num3<0.001f)continue; if(num4<0.001f)continue;`. Both source operators remain `<` and the exact literal bits6F-12-83-3A are preserved. Negating them by changing to `>=` would be wrong for NaN. The unchanged original init of num5=0 is followed by493ble.un506,499stloc12/501br509 and507stloc12, hence `if(num4>num3){num5=0;}else{num5=1;}`. V2 restores stores in each arm rather than a common ternary store; the source `>` is retained, so the false/unordered side still stores1.

5. After the two loops, original1105brtrue2144,1126bne.un2144 and1152brfalse2144 bypass **the entire remaining method region**. V2 expresses the positive `state==0 && gameState==2 && Check(bit8)` if. A negative return guard has a different branch/ret shape even with equivalent behavior.

6. In role0, original1193bge.un2047 bypasses the positive `ball.pos.y<20f` body, and1338brfalse2047 bypasses its positive ChRectHitRect body. V2 wraps those exact tails with those positive tests. It does not invert `<20f`, duplicate calls or move declarations/initializers across executed statements. DEBUG ret1382 stays inside its existing BitUtil.Check(flag,8) arm. The role!=0 ChTrapOrHeading/ChRepetition suffix is copied intact within the outer positive game-state gate.

No branch is classified as break/continue solely from distance. The positive region ends are tied to the actual j increment or the measured role/method footer. No EH regions exist in this method; no protected transfer was invented.

## Source preservation and real model

V1 already removes the materialized initial Boolean, changes GetCatchProc to one named result with2/1/default if branches, and adds four named unused Singles reproducing original slots18,19,25,26. Historical names/exact scopes of those zero-use locals remain UNKNOWN; V2 preserves the selected V1 declarations exactly.

Real model11 on the **whole V2 source** is complete31 and exactly equals all original types. Every name/type/slot role equals V1's31-role prediction, not just the type multiset. No model was changed and no error/completeness flag was suppressed. `source-proof.json` separately verifies from V1 to V2:

- identical literal and call-identifier multisets;
- identical full sequence of arithmetic and relational operators (`+ - * / % < > <= >=`);
- byte-identical DEBUG guard source;
- all V1 files still match their freeze.

These are source/model invariants. They do not substitute for actual target header/IL comparison or prove all remaining game differences are closed.

## Owned fixtures, negatives and emission rules

Six new owned libraries contain three independent region projections (no game class is compiled): Current, handwritten Control, calculated Candidate and three negatives. The projections test each new family separately; V1's own two-region fixture remains a separate prior proof. Each Candidate matches its whole independent Control method across resolved body bytes/header/locals/EH:

| Region | Before IL | Candidate/Control IL | Locals |
|---|---:|---:|---:|
| RunLoop: separate Int32/Boolean continues|184|199|3|
| RunFloat: two float guards and per-arm num5 stores|80|86|2|
| RunRegions: iteration/game-state/height/rectangle positive regions|314|306|3|

Candidate versus Control headers/maxstack and every local/EH entry are equal within each fixture. The three Current bodies differ. Nine genuine NoEmit predictions (three methods in three positive variants) are complete and equal to their actual entire emitted local signatures. Every command/compiler output/DTO is retained with pinned smcs2.6.5 SHA26b4f7c.../runtime/flags/references. No game source is part of these libraries.

All three positive variants returned semantic0. Checks include exact order/count of Abs/flag evaluations,36 float pairs spanning NaN, infinities,negative zero,epsilon andfinite values, and96 positive-region state/role/hit/debug/team/height combinations. The DEBUG early return is tested as observable behavior.

Three emitted negatives fail whole-body equality: changing `<eps` to `!(>=eps)` changes NaN behavior; removing the DEBUG return changes its guard behavior; using a common ternary store loses the two original arm stores. The first two were also executed as owned negative controls: they returned semantic3 and4 respectively (`negative-runtime.json`), confirming the NaN and DEBUG failures rather than merely inferring them. Five altered producer inputs are refused (debug mode, first-rectangle transfer, epsilon, capture and address axis).

Official Unity Mono source snapshot b6587926c0f146c8b12985744f33a13b39a22d57 is cited with local SHA/line excerpts in `compiler-citations.json`. Exact installed manufacturing revision remains UNKNOWN; own outputs verify the installed pinned executable. `If.DoEmit`233â€“269 branches false around a true arm; `Continue.DoEmit`1146â€“1149 branches to the actual LoopBegin/increment. `expression.cs3493â€“3566` marks floats as unsigned for false-edge emission: `<` falseâ†’Bge_Un, `>` falseâ†’Ble_Un; true floating comparisons use ordinaryBlt/Bgt. That source plus the original opcodes and the negative NaN runtime control explains why source operators must remain literal, not algebraically complemented.

The initial V2 implementation and all owned fixture checks passed without relaxing a guard/model. This is a calculated source composition, not blind candidate enumeration. Target/body closure is still UNKNOWN until the production owner rebinds the current source, predicts the real signature, performs an isolated target probe and runs the full verifier with zero regressions. This finite study stops after delivery; no extra family is opened.
