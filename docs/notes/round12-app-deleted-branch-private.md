# Private R12: six missing-branch cases, five concrete templates

This finite study uses the immutable B06 source/DTO snapshot `build/round12/runs/b06/commit/postverify01/Assembly-CSharp`. It does not assume a common source cause merely because the first aligned pair is `br => DELETION_POINT`. There are **five concrete templates**, covering six methods, optionally grouped into three broad categories: dispatch restoration, a common return after switch, and missing empty else-if arms.

No game target was compiled, no game verifier ran, and no production/source/public/configuration/ledger file changed. No generated class or IL byte is written into a result. Every proposed **game whole-body result remains UNKNOWN** until the exclusive operator recompiles and applies the ordinary verifier.

## Exact inventory

| Method (assembly prefix omitted) | Token | Original/current IL | Original/candidate local count | Concrete source template |
|---|---|---:|---:|---|
|`form.MyFormBase::GetTouchValue(System.Int32,System.Int32)->System.Int32`|0600018e|488 /483|2 /2|default break, return0 outside switch|
|`form.SpForm::SetTouchValue(System.Int32,System.Int32)->System.Void`|060001fd|56 /46|1 /1|single-case switch followed by default, remove num alias|
|`game.Map::GetGoalPartsDepth(System.Int32)->System.Int32`|060002ff|116 /68|0 /0|four empty else-if arms after the original part3 region|
|`surface.GameView::OnTouchTrackBar(System.Int32,System.Int32,kairo.unity.surface.TouchEvent)->System.Void`|06000575|51 /41|1 /1|case0 break, case9 Set/break, remove typeAlias|
|`system.form.BillingForm::OnTouchEvent(kairo.unity.surface.TouchEvent)->System.Boolean`|06000692|111 /101|2 /2|case2 goto case1, then case1 payload/break; remove selector alias|
|`system.form.LineupForm::SetTouchValue(System.Int32,System.Int32)->System.Void`|0600071a|56 /46|1 /1|same concrete template as SpForm|

Original total878 IL; baseline reconstructed total785 IL. The93-byte difference is a measurement, not a promised gain or a prediction of emitted candidate sizes. All six baseline and candidate model signatures have the original type order; current aliases sometimes have the correct type count while representing the wrong source structure.

REA's selected pages independently authenticate all six tokens to original artifact SHA3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949/MVID48ce9f89-73d2-40be-bb24-22c807c04106. All314 original instructions decode (171/23/40/18/39/23), no selected body issue/truncation. Each raw CIL hash equals the frozen original DTO's CIL bytes. CIL/metadata coverage is complete; wider selected-type string limitations are retained in the REA summary and do not authorize string modifications.

## Source derivations

**SpForm and LineupForm:** original selector is copied once to local0, compared to131072, then an unconditional default jump appears at25→42. The selected-field store is at32, followed by break37→55; base.SetTouchValue executes at45 with break50→55. Current source puts the base call first in an if/else and the field store later. The calculated form restores `switch(type) {case131072: select_=value;break;default:base.SetTouchValue(type,value);break;}` after the unchanged uint mask. The recovered `int num=type` is removed: the compiler's switch temporary occupies original local0. Keeping both creates an extra local.

**TrackBar:** original local0 dispatches zero to empty-case label21, where `br50` exits. Nine targets the Set call's block26; `br45→50` ends that case. The dispatch's default `br16→50` is separate. Current Boolean OR collapses these labels. Restoring `switch(type){case0:break;case9:Set(v,e.dx,e.component);break;}` preserves all three branches. The recovered `typeAlias` declaration is removed so only the original one Int32 selector slot remains. The event is not touched for other values, matching the original.

**BillingTouch:** after selector e.type→slot1, comparisons1 and2 target62 and57 respectively. Offset57 is a real `br62`, followed by the common payload; another `br102→107` leaves the case before returnfalse. Therefore `case2:goto case1;case1:payload;break;` is required, with the physical case2 section before case1. Simply stacking labels makes both comparisons point directly to the payload and loses five bytes. The payload, inner e.type==1/KeyClick2048, outer component guard and true/false returns remain unchanged. The output goto is ordinary C# with an unambiguous owner inside the newly restored switch, not an IL injection. The named selector declaration is removed; the generated switch cache still occupies original slot1.

**GetTouch:** all prefix instructions through the default PRINT/throw region already agree. Original PRINT=false targets481, which contains `br486`; common `ldc0;ret` is at486/487. Current default directly contains return0 at481/482. The calculator leaves every existing case and throw expression intact, changes only the terminal default return0 into break, and puts return0 after the switch. It requires that the switch is method-terminal and its direct default has the reviewed PRINT/throw shape.

**GoalDepth:** original part3 first runs the existing float test and optional camera-depth return. If it finishes without returning, `br66→114` skips the following empty arms. Original then compares parts4/5/6/7 at73/85/97/109; the first three true empty arms branch to114. Current source discarded all four arms. The parameter/constant operands and targets directly establish the missing equality tests. The original `<` float comparison remains byte-for-byte source text; its false branch is `bge.un40→66`, preserving NaN behavior. The new empty arms use only Int32 equality, without payloads, locals, or calls. No floating-point operator is mirrored or negated.

## Model, provenance and source APIs

`calculate.py` exposes five pure APIs: `transform_get_touch`, `transform_set_touch`, `transform_goal_depth`, `transform_trackbar`, `transform_billing_touch`. The setter API applies to the Sp/Lineup pair. Each is header-independent and bounded by exact token/owner/payload checks. Calls with Path I/O forbidden reproduce the payloads. Extra Billing/Goal payload statements are rejected. There is no forced incomplete flag or model modification.

`source-patches.json` contains six engine-compatible normal-parent-method proposals; per-method directories contain exact before/after C#, full snapshot source after, original/current DTOs, real NoEmit predictions and authenticated REA pages. `source-proof.json` records full-source and method SHA/UTF16 bindings, live-before uniqueness, model/client/producer identity and exact before/after inventory checks. Calls and strings are preserved. The only lexical number additions are Goal's original4/5/6/7 tests and Billing's `goto case1` label (not a new runtime constant). Existing aliases are replaced by compiler-generated selector slots; their source names are not claimed to be original local names.

The unchanged real public R11 NoEmit model predicts **complete/original-exact** signatures for all six candidates:2/1/0/1/2/1. Before types already matched; this is why type equality by itself did not reveal the omitted branches. Candidate header/maxstack/metadata/body equality is still UNKNOWN. The operator must rebind the current full file and repeat its ordinary checks before compiling/adopting.

## Own fixtures and all negative results

Six newly authored mock libraries were compiled with installed Unity smcs2.6.5 SHA26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0, real copied options/references and private output paths. No game source/class was compiled. The fixture matrix includes current, independent manually authored control, calculated candidate, and three negative libraries.

| Own candidate method | IL /locals | Entire method against own manual control |
|---|---:|---|
|RunGet (short representative case inventory)|97 /2|equal|
|RunSp|56 /1|equal|
|RunGoal|116 /0|equal|
|RunTrack|51 /1|equal|
|RunBilling|111 /2|equal|
|RunLineup|56 /1|equal|

All six before methods differ. Equality uses the full ordinary DTO comparison: bytes/instruction operand identities/header/maxstack/locals/EH, not just a prefix or a count. No method has EH. Eighteen own NoEmit predictions (six methods × before/control/candidate) are complete and equal to the actually emitted own signatures. The smaller RunGet fixture proves the default/return emission template, not equality of its shortened case inventory with the488-byte game method.

Measured negatives:

- Billing stacked-case labels have five fewer bytes and differ from control, although runtime semantics agree.
- Retaining the recovered named alias while also emitting switch adds one extra Int32 local to each of four own methods (Sp,Lineup,Track,Billing); all four bodies differ.
- Replacing Goal's `<` by an assumed complement of `>=` differs in body and fails the NaN semantic check (status7).
- Six producer mutations are rejected: changed GetTouch default return, changed setter case constant, changed track Set argument, extra Billing field write, extra Goal action, changed Goal float comparison.

All fixture compilations succeeded. Current/control/candidate, stacked-label negative and alias-negative pass the owned runtime checks: masks/default exceptions, base-vs-select assignment, track event handling,45 Billing combinations and60 Goal value/part combinations including NaN/infinities/signed zero. The NaN-negative fails deliberately. This distinguishes semantic equivalence from byte equality; neither behavioral success nor model success is reported as matched game code.

## Compiler source and historical attempts

Exact pinned source SHA/line excerpts are in `compiler-citations.json`, Unity Mono pin b6587926c0f146c8b12985744f33a13b39a22d57:

- statement.cs1027–1030: GotoCase emits Br to the case code label.
- statement.cs1115–1118: Break emits Br/Leave to the lexical owner exit.
- statement.cs3385–3400 and3463–3478: switch keys are sorted; small blocks compare keys, zero can use a false branch.
- statement.cs3548–3586: default jump remains distinct; sections emit in source order.
- statement.cs3825–3869: even a named selector creates a LocalTemporary, then releases it.
- If/float branch emitters explain empty-arm joins and the retained unordered false float edge.

The exact manufacturing source revision of Unity's installed smcs remains UNKNOWN. The local code pin explains emission; full own bodies demonstrate behavior of the actual installed pinned compiler.

`history.json` retains80 reports per method. GetTouch,Sp,Lineup,Goal have0 actual trials/0 steps. Track has4 trials/2 adopted partial steps (tail-return, empty-then exit, positive condition, split OR), none restoring the switch/case0. Billing has8 trials/1 partial step, all guards/empty-arm or return rearrangements, none with a case2 goto case1 switch. Their recorded failures and partial adoptions remain in history. Every new full-method after hash is absent from retained trial/step after hashes. This is a bounded historical check, not an assertion that no equivalent source could ever have existed outside retained receipts.

The common histogram pair was therefore diagnostic only. Five source templates are supported for fresh isolated probes; zero methods or IL bytes are counted as gained by this private work.
