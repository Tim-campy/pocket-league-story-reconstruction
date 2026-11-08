# Private R12: two missing local-initialization `dup` instructions

This is a finite source proposal, not a game-build result. No target DLL was compiled, no game verifier ran, and no public/source/ledger/configuration file was changed. The full reconstructed game bodies remain **UNKNOWN** until the exclusive production operator recompiles them and runs the ordinary oracle.

## Identities and inputs

| Stable method ID (assembly prefix omitted) | Original token | Original IL | Frozen reconstructed IL | Original/current locals |
|---|---:|---:|---:|---:|
| `form.SubForm::DrawMenu_btmInfo(kairo.unity.ui.Graphics)->System.Void` | 06000222 | 1223 | 1250 | 19 / 20 |
| `game.Tenant::Draw(kairo.unity.ui.Graphics)->System.Void` | 060003cd | 492 | 485 | 7 / 7 |

Both IDs were selected from the current method identity inventory/ledger and cross-checked against the original DTOs. The input snapshot is `build/round12/cycles/b03/after/Assembly-CSharp`. REA pages independently bind both tokens and raw CIL hashes to the original artifact SHA `3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949`, MVID `48ce9f89-73d2-40be-bb24-22c807c04106`. REA decoded 535/535 and 205/205 instructions, respectively; both selected bodies have no decoding issue. CIL/metadata coverage is complete. SubForm's wider selected-type user-string inventory is partial because of existing terminal flags; this work does not change strings or assert that those flags are valid.

The live SubForm **file** changed after snapshot B03 because a different method was adopted. The exact DrawMenu method before-text is still unique and identical. `source-proof.json` records this distinction and the current UTF16 location. The proposals retain their genuine B03 source SHA and spans; the operator must rebind the exact method and run a fresh model/oracle before adopting. Tenant's whole file still equals the frozen input at packaging time. Neither equality is a promise about future source changes.

## Original evidence and calculated source

DrawMenu starts with two independent local chains:

```text
0 ldc.i4.0    1 dup    2 stloc.1    3 stloc.0
4 ldc.i4.0    5 dup    6 stloc.3    7 stloc.2
```

The slot-use operands identify slot0 as the left x coordinate, slot1 as left y, slot2 as right x, slot3 as right y. The proposal uses readable recovered names, without claiming historical local names:

```csharp
int num3, num;
num3 = num = 0;
int num7, num2;
num7 = num2 = 0;
```

Only the declarations move. `num3 = instance.GetSideSoftWidth() + 1` stays inside the original optional block; `num7 = instance.GetGameWidth() - 123 - instance.GetSideSoftWidth()` stays after it. Calls and all drawing leaves keep their execution locations. The two recovered Int32 switches (`matchInfoJoin`:0/1; `i`:0/1/2) are converted to direct if/else-if comparisons, keeping case child blocks and removing only the proven terminal case-owned breaks. This additional composition removes the reused `$switch` slot19. It is necessary to make the entire original local signature exact, not evidence that the missing initial dup itself was a selector cache.

Tenant's distinct array-element compound assignment already has its correct dup at offset264. The missing dup is later:

```text
270 ldc.i4.0
271 dup
272 stloc.2    // y, recovered num
273 stloc.1    // x
274 ldarg.0
275 ldc.i4.0
276 stfld draw_cnt
```

The following clamp result occupies original3; the for index occupies original4. x is subsequently assigned at323 and y at339. The proposal declares `int x, num; x = num = 0;`, leaves `draw_cnt = 0` as a **separate** field assignment, and leaves the old x initializer expression as an assignment inside the original for loop. The loop/guards/increment/drawing leaves are untouched. Thus the original first four relevant roles become x1/y2/clamp3/index4 instead of baseline y1/clamp2/index3/x4. Equal type count alone cannot prove these roles.

`slot-lifetimes.json` records the original first/last/access offsets of all 26 proposed roles. It is diagnostic evidence, not a normalized judge.

## Compiler and model evidence

`compiler-source-evidence.json` includes exact local source SHA, lines, excerpts, and official Unity Mono pin `b6587926c0f146c8b12985744f33a13b39a22d57`:

- `assign.cs:398–413`: a value-producing assignment requests `leave_copy`; an assignment statement does not.
- `expression.cs:4321–4356`: normal VariableReference assignment emits source, optional Dup, then the variable store. A local assignment nested as the source of another local assignment therefore yields the observed dup/inner-store/outer-store.
- `statement.cs:2010–2052`: block declaration/registered-temporary/child emission order accounts for the slot change when bare locals move but initializer statements stay.
- `statement.cs:3825–3869`: a nonconstant switch creates a LocalTemporary even for a named selector, then releases it. The existing two switches can reuse the extra Int32 slot.

The exact manufacturing source revision of Unity's installed smcs remains UNKNOWN; this pinned source is explanatory evidence and own fixtures on the installed smcs establish the measured behavior. The installed compiler SHA is `26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0`; complete runtime/options/references and source/model/client hashes are retained in the manifests and fixture results.

The real public R11 NoEmit model was used unchanged, against the full frozen target source and real reference set:

| Target | Before prediction | Candidate prediction | Candidate matches original type order |
|---|---|---|---|
| DrawMenu | complete,20 | complete,19 | yes |
| Tenant | complete,7 | complete,7 | yes |

Every original slot also has an explicit proposed source role and original producer/use anchor. No incomplete prediction was promoted and no model veto was bypassed. These predictions are not target emission or whole-body equality.

## Own fixtures and negative results

All fixture source is newly authored C# with small mocks; no game class is compiled. The final matrix contains six own library compilations. The identical matrix was run twice because the token lexer was strengthened to recognize lambda `=>` for its capture rejection: **12 actual own compilations**, zero failures, zero target compilations. Do not mistake the two repeated matrices for twelve distinct variants.

| Own method | Current IL/locals | Independent manual control | Calculated candidate |
|---|---|---|---|
| RunMenu | 441/20, different | 419/19 | 419/19, entire body/header/locals/EH equal |
| RunTenant | 206/7, different | 213/7 | 213/7, entire body/header/locals/EH equal |

Comparison uses `compare_kairo.differences` on complete method DTOs, including header/maxstack/initlocals/local types/instruction identities/bytes/EH; the comparison is not limited to a prefix or opcode count. Both fixture methods have zero EH regions; no exception-region inference is needed by the target edits. Current/control/candidate each return semantic status0 in the owned runner, including four menu modes, getter-evaluation counters, six player counts, and the disabled drawing branch. Six NoEmit predictions in each final matrix are complete and equal to their actually emitted own local signatures.

Three emitted negative variants test the two producers:

- Keeping the declaration order but splitting each local zero chain into separate stores differs from the independent control in both methods, despite the same type counts/code sizes.
- Swapping the paired declarations differs from control in both methods, again despite equal type inventories.
- Inserting `draw_cnt` inside Tenant's chain differs:8 locals/217 IL instead of7/213. The field is not part of the original local chain. The unchanged RunMenu in this negative library intentionally remains equal; it is not counted as a negative result for RunMenu.

Six producer negatives are rejected: repeated selector call, default arm, nonterminal/ambiguous break, changed menu initializer, changed Tenant initializer, and goto. Two additional pure-call lambda negatives are rejected. `test_pure.py` verifies import and calls with Path I/O forbidden, and exact header-renaming independence is checked when building the bundle. The APIs are finite token-guarded templates, not arbitrary refactoring engines.

The pure-call harness initially failed before producer import because Windows cp1252 could not decode the UTF-8 Japanese method text. Explicit UTF-8 byte reads corrected it; the retest passed without modifying any source proposal. This and the lambda-tokenizer correction are recorded in `preparation-incidents.json`, separately from source-hypothesis outcomes.

## History and handoff

`history.json` retains SHA and entries from75 reports per method across R7–R12. DrawMenu has0 actual trials/0 adopted steps in those reports. Tenant has10 actual trials/2 adopted steps: old void common-exit/polarity partials, declaration ordering, empty arms, and condition variants. The retained old `solver-group-block-declarations` grouped tPlayer2/layer within the for body; it did not move x before the loop or create x=num=0. The old `local-declaration-order-later` only moved a bare clamp declaration. Neither is the current candidate. New full-method after hashes are absent from the retained historical after hashes. This is a bounded historical check, not proof no semantically equivalent source was ever tested elsewhere.

Pure API: `calculate.py::transform_menu(text)` and `transform_tenant(text)`. Engine-compatible full-parent-method proposals: `source-patches.json`; exact before/after C# and snapshot full sources: per-method subfolders. No compiler-generated class or IL byte is synthesized in source. Eligibility for an isolated game probe is supported by own fixtures and exact real model signatures. **Original-vs-game-candidate whole-body result, target headers/maxstack/EH/metadata and methods gained remain UNKNOWN.** The exclusive production operator must bind the fresh source, validate the model, compile and apply its ordinary verifier before any state transition.
