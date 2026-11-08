# Private MapMain.Draw_coat: guarded dispatch and inner-loop continues

Only `game.MapMain::Draw_coat(kairo.unity.ui.Graphics,System.Boolean)->System.Void` is in scope, token06000307,623 original IL. The exact ID/signature/token/size were confirmed against the readonly live ledger before source derivation; that row and ledger SHA are preserved in `source-proof.json`. Frozen source/DTO/manifest input: `build/round12/runs/b06/commit/postverify01/Assembly-CSharp`.

No game target build, verifier run, production/source/public/config/ledger edit or new family. Previous packages remain untouched. This package's **game whole-body outcome is UNKNOWN**.

## Distinguishing initialization, slots and control

The histogram pair `ldc.i4.0 => stloc.s` occurs at174 in the selector dispatch. It does **not** identify a missing root zero initialization or a zero assignment chain. Original slot1 (recovered num2) is first stored at54 after ldc.i4.m1 at53, inside the inner cell loop. Its other stores are the fence field at155 and value5 at261. The current bare `int num2;` declaration and per-cell num2=-1 stay unchanged.

Original locals are6: mapchipMain0,num2(Int32)1,num(Int32)2,i(Int32)3,mapchipMain2 4,image5. Current has exactly that prefix plus the Int32 switch cache6. The real NoEmit model before is complete7; calculated after is complete6 and exactly the original type order. All six declared name/type/slot triples remain unchanged. This is removal of a compiler temporary through a supported source shape, not arbitrary declaration permutation.

Both original/current headers are fat12-byte, flags19/initlocals=true,maxstack23, no EH; sizes623/630. These are baseline measurements. The candidate has not been game-compiled and its header/maxstack/body/metadata remain unmeasured.

## Three calculated components

1. **Nonnegative direct dispatch.** Original area-level test bge168→262 is followed by num2/ldc0/blt175→262. It then compares0 with brtrue181→216,3 with bne.un218→253,6 with bne.un255→262; selected assignments leave to262. The current switch instead stores slot6, emits a table plus fallback compare, and ends at279. The source shape is `if(num2>=0){if(num2==0){case0 leaf}else if(num2==3){case3 leaf}else if(num2==6){num2=5;}}`, still inside the existing area<2 block. Only the terminal case-owned breaks are removed. All case assignment expressions remain byte-exact source. Dispatch region173..279 becomes the original173..262, a17-byte reduction. No selector/field expression is duplicated beyond the original direct local reads.

2. **Rectangle false means continue i.** Original call result has brtrue448→458, then br453→596. Current positive if directly uses brfalse465→603. The required guard is `if (!(GameUtil.ChRectHitRect(existing exact arguments))) {continue;}` followed by the unchanged insert body. It adds the missing5-byte explicit branch. Negation wraps the Boolean call; no geometry/arithmetic/float comparison argument is inverted or normalized.

3. **Fence below5 means continue i.** Original num2/ldc5/bge514→524 is followed by br519→596. Current wraps the fence insert in `if(num2>=5)`. The source shape becomes `if(num2<5){continue;}` followed by the unchanged fence insert, adding5 bytes. num2 is a proven Int32 and the original bge is signed; this does not apply a floating-point complement.

Both original branches453/519 target596: slot3 is loaded/incremented/stored at596..599, then tested at600..606. That is **i's increment**. The outer num decrement is611..614; it is not the destination. The producer checks that the rectangle block is the last statement of the exact inner `for(int i=0;i<MAPCHIP_FIELD_WNUM;i++)` body and that the fence block is terminal inside it. This proves which lexical continue owner is required and makes flattening the wrappers safe. No statement is moved across another call or declaration.

The measured regional account is -17+5+5=-7, consistent with630→623. It is a diagnostic calculation, not a claimed target equality or gain. Every source call/string literal is preserved; the sole added numeric token is the0 in the original nonnegative selector guard. Existing casts, num2=-1, object initializers and declaration order are untouched.

## Own fixtures and genuine models

Eight newly authored mock libraries test this three-part control shape with row/cell loops, selector cases, image-height counters and insert traces. None compiles the original game class or production file. Installed smcs2.6.5 SHA26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0; copied real flags/references/private outputs are retained in commands/manifests.

| Own variant | IL /locals | Complete body vs independent manual control |
|---|---:|---|
|Current switch and positive drawing wrappers|385 /7|different|
|Independent guarded-if/continue control|378 /6|equal|
|Calculated three-component source|378 /6|equal|
|Missing nonnegative selector guard|371 /6|different|
|Retained positive hit wrapper|373 /6|different|
|Retained positive fence wrapper|373 /6|different|
|Hit failure uses break instead of continue|378 /6|different|
|Invented num2=0 at root|380 /6|different|

Equality uses complete ordinary DTO comparison: bytes/instruction operands/header/maxstack/locals/EH. It is not prefix equality, a slot bijection, or code-size equality. The break negative has the same size/count as control but the wrong branch target. The invented-zero negative proves why the histogram's ldc0 must not be read as an initialization to add.

Three own NoEmit predictions (before/control/candidate) are complete and match their actually emitted own signatures. The real unchanged public R11 model separately predicts the full actual game candidate complete6/original-exact, with all six declared roles preserved and only the cache removed. No incomplete flag is forced and no veto is bypassed.

Current/control/candidate and the neutral byte-negative forms pass112 semantic combinations (seven fence values including negatives, two area levels, ground choice, hit result and offscreen choice), with cell visits/image reads/insert traces. The wrong-break variant deliberately fails visit counts (semantic status1). Six producer negatives are refused: invented root initialization, effectful selector, changed case, wrong inner-loop header/owner, capture and extra work after the fence block. All own compilations succeeded. Runtime equivalence of several wrong byte forms is recorded explicitly; it does not establish matched code.

## Original evidence, history and delivery

REA authenticates token06000307 and all253 instructions against original artifact SHA3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949/MVID48ce9f89-73d2-40be-bb24-22c807c04106. Raw CIL SHA equals the frozen original DTO. Selected body/metadata decode is complete; retained wider-type user-string limitations do not authorize string modifications.

The official Unity Mono pin b6587926c0f146c8b12985744f33a13b39a22d57 is quoted with exact local SHA/lines in `compiler-citations.json`: If.DoEmit233–269, For.DoEmit571–619 (LoopBegin before increment), Continue.DoEmit1141–1150, Switch.DoEmit3825–3869 (temporary allocation/release), block declaration emission2010–2052 and signed/unsigned branch selection in expression.cs3493–3566. Exact manufacturing revision of the installed compiler remains UNKNOWN; owned fixture outputs establish its measured behavior.

`history.json` retains84 reports and2 actual trials: R7 switch-if and owned-switch-if, both plain0/3/6 chains without the nonnegative guard or the two explicit continues, both stayed at raw prefix42 and were rejected. That early operand belongs to a future loop target, so the old result does not prove that losing the switch cache was the wrong local reading. Neither old source was this complete composition; both negative outcomes remain recorded. The new whole-after hash is absent from retained after hashes. This is a bounded historical check.

Pure API: `calculate.py::transform_coat(method_text)`; components `direct_dispatch`, `fence_continue`, `hit_continue`. `source-patches.json` contains the normal-parent-C# proposal with exact UTF16/full-source/method SHA bindings and producer/model/client hashes. Header renaming and Path-I/O-forbidden calls reproduce it. The operator must bind fresh source, validate the unchanged model and run the full ordinary game oracle before any adoption. No method/byte gain is counted here.
