# Update2 ? private source-only composition

Status: **READY_FOR_GAME_PROBE; actual game body/header/EH/prefix UNKNOWN**.
No game/target compile, binary mutation, source adoption, state edit, public write or commit was performed here. The package remains private. The full verifier owned by root is the only acceptance test.

## Frozen inputs and reproduction

Method: `form.SubForm::Update2()->System.Boolean`.
Snapshot: `build/round10/cycles/cycle13/after`, commit `dcd2af719a40237e61a11b6a00d1994ffea5d34e`.
Raw frozen SubForm source SHA256: `5e42071f8bd42d8280dbda8834b170fe9b1e143074b68f08e28eb6684b788a4d`.
Method substring UTF8 SHA256: `a2210789a710aeaa05a1814681b7086b1d2ba27b5c8ed942ae4ee6b8c23e6942`.
Source is read exclusively from the immutable snapshot. No Main source is consulted.

Pure API: `controls.py::transform_integral(method_source)`. It accepts the complete C# method declaration, without depending on its header/name, returns a new string, and has no filesystem, model or compiler side effects. `STAGES` exposes the 9 separately measurable functions. It imports only private `proposal.py` for the layout and balanced lexical parser.

Private bundle: `source-patches.json`, SHA256 `c7a7e4952fb2583e3b7082dbbdfc01a942091219cfaaeea17be99b1386d099e8`.
Calculator: `controls.py`, SHA256 `53820a63d338cae25d5fdd93f3a3aa4d29b71454970d009d8937d85dfe7ee095`.
Layout dependency: `proposal.py`, SHA256 `8bd5b8ea4d650e17a8cb965ff6bca5e124dfc7288424981a3886054bd52e1551`.
Final NoEmit proof SHA256: `e7f3b018d1b9cccb701e953e06cc505a151d32a922968ae5e39534c73213c45f`.

The bundle has one patch and one method, potential 12,082 original IL bytes. Before/after are exact raw-text substrings with UTF16 source spans; `rebase_unique` permits binding to the same unchanged method after another method in SubForm changed. The consumer must perform a fresh model gate and actual game compilation/verifier. No frozen input should be rewritten while testing.

Analysis-only reproduction: `python .../calculate_controls.py` (all nine source-only NoEmit stages). Own toy reproduction: `validate_layout.py` and `validate_controls.py`, **never passing game sources to compile_smcs**. `export.py` only verifies frozen source/proofs and exports text. All public model/compiler helpers stay unchanged.

## Measured baseline

| Measure | Original | Frozen rebuilt |
|---|---:|---:|
| IL bytes | 12,082 | 12,023 |
| maxstack | 803 | 803 |
| locals | 103 | 103 |
| Int32 slots | 54 | 53 |
| Boolean slots | 1 | 2 |
| exception regions | 3 | 3 |
| unused slots | 0 | 0 |
| switch opcodes | 0 | 0 |

Both bodies have fat 12-byte headers, flags27, initlocals=true. All other type multiplicities agree. There are no switch statements in the source and no switch temporaries; the sole model emission temporary is captured-frame `$storey` at slot102. A hypothesis attributing this layout to switch temps is REFUTED for this method.

The unmodified source NoEmit predicts all103 rebuilt local types exactly. Two resolved controls in the same snapshot also have literal full-body equality and independently exact model signatures: Team.SetSortJampBallNear9/9, UserData.Update_edit50/50. These are controls of model/source binding, not evidence of the target's new body.

## Calculated layout

- Rebuilt Boolean1, `recoveredCondition_273013`, is the extra key-repeat alias. Its only two uses are store@41 and load@42 after CheckKeyRepeat(131072); original call@36 branches directly@41. Remove only this wrapper alias, retaining the if/body.
- Rebuilt0 corresponds to original0; rebuilt2..101 correspond to original1..100; frame102 remains102.
- Original Int32 slot101 is a named GetMatchInfoJoin result: call@11857, store101@11862, load101@11864. Those are its first/last and only accesses; it is not an unused reserve. The rebuilt call@11802 is tested directly. Introduce synthetic `recoveredMatchInfoJoin` at that call, in the same containing block. The name is chosen for clarity, not claimed historical.
- Captured frame102 is constructed/stored@10515/10520, last used10680. Its runtime initialization precedes late named local101; the allocator registers named locals before emission temporaries. Both existing anonymous functions remain byte-for-byte identical source substrings, independently hashed in `leaf-inventory.json`; no generated type is authored.
- Original type sequence admits insertion of the missing Int32 at position101 uniquely after removal of Boolean1. No guessed unused local is inserted.

`layout-noemit-proof.json` proves103/103 exact. All8 further control stages preserve103/103 exactly, complete, without unsupported model reasons. The final Join is101 and `$storey`102.

## Diagnostic alignment and source composition

Baseline raw alignment reports316 sequence edits and1 branch-retarget diagnostic. After a diagnostic-only local identity correspondence, it has24 sequence regions and0 retargets. No derived DTO, modified instruction list, normalized assembly or alternate judge is written. The correspondence is only a way to locate source defects.

| Regions | IL anchor original | Source composition |
|---|---|---|
|1|call36; branch41 vs store/load41/42|remove extra Boolean alias|
|2?3|state2 bne.un223?321; join316?344|positive Sponsor state2 arm; old construct/assign/Push-return becomes else|
|4?5|select brtrue507?631; join626|select0 positive same-block tail, old Pop-return in else|
|6|bne.un2966?2976; br2971?12080|explicit empty type48 arm, same following false block|
|7|event189 brtrue3428?outer increment3536|wrap only following inner for-m in !CheckAlreadyEvent; remove redundant outer continue|
|8?9|select brtrue5430?5574; join5569|select0 tail for type54|
|10?11|page brtrue7056?7236; join7231|page0 tail for type59|
|12?14|select8009; pay brfalse8072?8175; joins8170/8198|nested select0 and positive payment, false Exec14+Pop-return retained|
|15?18|select8393; pay8435; inner-strength8534; joins8631/8643/8671|same nesting; only second IsStreangth becomes positive tail|
|19|bne.un10727?10737; br10732?10897|explicit empty select2 before select3; two earlier captured delegates/EH bodies untouched|
|20?21|select11058?11137; join11132|select0 positive tail, old Exec116+Pop-return in else|
|22?23|state0 brtrue11663?11692; join11687|positive FanBonus state0 tail|
|24|store/load101@11862/11864|named Join local|

This is14 control sites:11 positive-tail/else restorations,1 loop-guard restoration,2 explicit empty arms. All preceding declarations/statements stay in their current lexical parent; no root declaration is silently moved into an unrelated block. Guards may contain straight-line actions and a terminal value return (the older only-return primitive would miss Sponsor and event116). The inner-strength selector requires the first statement of its actual naked else and the ProcStrenSpecialTenant tail; the first strength check in type63 stays unchanged.

For event189, the enclosing for-l, the earlier TRA_PLAYER=-1 continue, and PlayerData/AddPhysicalParam prefix are retained. The existing inner for-m break is unchanged. Its target can coincide with the outer increment because the inner loop is last: the historical break-vs-continue owner at that coincident target remains UNKNOWN. We do not relabel it or invent a new owner; the edited transfer is the separately proved lone outer continue guard.

Every changed comparison uses proven Int32 fields or Boolean-returning methods (`primitive-type-proof.json`). No floating comparison, .un meaning, NaN behavior, arithmetic expression or string payload is complemented. Declaration/call/string multisets are unchanged apart from the synthetic local replacement. Physical call order changes at the restored if/else arms, as required by the original IL; delegates are text-identical.

A byte-count crosscheck, only diagnostic: alias removes2; original-slot3 short forms remove3 bytes across three accesses to rebuilt slot4; named Join adds4. Layout net?1. Eleven required long join branches add55; two empty arms add10; redundant outer-continue branch removes5. Control net+60, combined+59 = original-minus-rebuilt59. This predicts code size only, **does not establish target body/header/maxstack/EH equality**.

## Compiler source evidence

The local Unity Mono source is pinned to `b6587926c0f146c8b12985744f33a13b39a22d57` under `analysis/tools/round5-mono-source/mcs/mcs/`. In `statement.cs247?267`, If.Emit emits the predicate to false_target, emits the true statement, then emits a long `br end` before the false statement when the true arm can fall through. `is_true_ret` is set by actual reachability at211. This explains the added join for the11 positive tails and the explicit empty true arms; it does not infer a new historical scope from the presence of a branch alone.

`statement.cs1117?1119` emits break to LoopEnd, and1148?1150 emits continue to LoopBegin; `For.Emit569?616` identifies the updater/increment placement. We preserve the known for-l/for-m lexical owner rather than merging coincident labels. Block locals are resolved in registration order at2032?2046 and LocalInfo is allocated with DeclareLocal at1225?1238. Existing model handles captured-frame registration/emission separately, confirmed by the own lambda fixture and the independently exact source signature. No new allocator rule based on variable names is introduced.

## Own fixture validation

`toy-layout-validation/report.json`:3 own compiles, layout candidate full-body equal to independent manual source; all10 methods including storey ctor/lambda equal;5 typed/shape negatives rejected. Baseline differs.

`toy-control-validation/report.json`:3 own compiles containing7 independent transformed methods (States,Select,Page,Pay,Event,Empty,Captured). Every candidate full body equals independently written manual control; all27 methods including captured-frame ctor and2 lambdas equal;9 negatives rejected. Baselines differ in all7 transformed methods.

Total6 own compiles,8 transformed fixture methods,37 full-method comparisons exact,14 negative rejections. Comparison includes header, locals, entire instruction sequence, EH, sections and attributes via existing `differences`, not just opcodes. Toy types/calls/payloads are authored separately and do not embed target code. Intermediate first fixture check found the inner-strength selector ambiguous when an outer fixture else also contained the marker. The parser was narrowed to require a first-statement guard in the actual naked else; the game source was independently re-NoEmit-validated afterward. No target was compiled during that correction.

## Historical attempts and novelty

`history-by-method-sha.json` authenticates57 historical engine reports: raw final source SHA must agree with report.source_hashes, then exact lexical method substring is hashed. All57 terminal sources contain the same method text as cycle13 (despite other SubForm methods changing).

Round4:48 trials,1 accepted condition-local partial step; Round5:96 trials,0 accepted; Round6 through the audited Round10 cycles1?13:0 target trials each. This source has therefore remained unchanged since that Round4 partial adoption. Removing the particular extra alias is intentionally a reversal of that partial source decision, coupled with the missing Join101 and complete control composition; it is not a claim that the original accepted step was body-equal. Existing negative polarity/hoist/loop-condition/return-local trials remain recorded. The14-site joint candidate has no prior trial in the authenticated history. No negative history is erased or relabeled.

## Remaining limits

All24 diagnostic regions have a stated source composition and all9 stages have exact real NoEmit layout. This is enough to submit a single source-only game probe; it is not a measured game match. Actual target body equality, emitted prefix, maxstack, EH offsets, generated members and effects on other methods remain UNKNOWN until root compiles/verifies the whole assembly. If rejected, preserve the receipt and first remaining frontier rather than modifying state or claiming a normalized match.
