# Balanced source branch restoration (smcs)

This bundle proposes a compound source edit, not an adopted game change. No game assembly was compiled by its author. The game verifier remains the judge.

## New phenomenon and current target

`form.DebugForm::Update_gameParam()->System.Boolean` has 13,913 original IL bytes. Cycle10 original/rebuilt dumps are recorded in `reports/round7-cycle10-after-measurement.json`. Its original IL2338 is `brfalse 2348; br <join>; ...Employ`, whereas rebuilt IL2338 is `brtrue 2350; ...Employ`. An empty positive then arm must be restored before the existing body, adding one long `br` (5 bytes).

The same rebuilt method has `br 2548` at IL2543 immediately before IL2548; the original continues directly after `SetAllDraw`. This extra long branch comes from the empty else of the ancestor `if (num9 != flg_loadCheck)`. Removing that else removes 5 bytes. The candidate pairs these two edits in one source patch, with unchanged evaluation order and size. No IL or original bytes are copied into the candidate.

| Place | Original | Rebuilt |
|---|---|---|
| Earlier prefix | IL8 `brtrue 2962` | IL8 `brtrue 2962` |
| Loop entry | IL2275 `br 2361` | IL2275 `br 2356` |
| Recipient | IL2338 `brfalse 2348`; IL2343 `br <join>` | IL2338 `brtrue 2350`; IL2343 body |
| Donor | after `SetAllDraw`, direct next statement | IL2543 `br 2548`; IL2548 next statement |

The earlier branch's equal operand is accidental compensation. Restoring only the inner arm shifts its target by +5; removing only the ancestor else shifts it by -5. Thus independent edits violate strict prefix preservation even though the compound edit can succeed. In cycle08, the exact descendant empty-arm-only candidate was model-approved, but its raw prefix fell from 2276 to 9 and it was rejected. This is a new *paired* transformation; it does not relax that invariant or repeat independent empty-arm search.

## Eligibility and interfaces

`tools/Round7BalancedBranchRules.cs` declares `AddRound7BalancedBranchRules` with the normal shared partial-class transform API. Opt-in family: `smcs-balanced-empty-arms`, selected through `round7_cycle_rule` or `round7_cycle_rules`. Compiler must be smcs/mono-smcs (or legacy empty compiler label). Both evidence flags below are required.

`tools/round7_balanced_branch_hints.py` exports `balanced_branch_hints(original, rebuilt, original_offset, rebuilt_offset) -> dict`. It checks an original long conditional targeting the instruction after a long unconditional branch, a rebuilt conditional without that following branch, and a later rebuilt long `br` targeting its next instruction. Returned flags: `balanced_missing_long_branch`, `balanced_extra_fallthrough_long_branch`; added/removed widths are 5 and `balanced_donor_il_offsets` records observed donors. These flags prove widths/patterns, not the entire source-to-IL relation; recompilation must confirm the particular candidate.

The syntax transform pairs exactly one descendant if without else and one enclosing if with empty else in the same callable. The ancestor true arm must reach its endpoint, so removal can actually eliminate a branch. Both source edits are composed over that ancestor's span. It rejects custom operators/conversions and crosses no lambda boundary. Existing `Negate` preserves floating-point semantics. Frozen source-only prediction for the Debug candidate is complete and equals the original local signature; see `round7-balanced-branch-model-proof.json`. The parent's live gate must still run on its current sources.

## Fixture results

Reproduction: `python reports/round7-balanced-branch-fixture-driver.py`. Uses the genuine Unity smcs compiler on independently written toy C# only, with an isolated helper directory. Eleven dispatch/safety controls pass; seven toy libraries and three toy executables were compiled. Two complete reference bodies match, including local signatures and exception regions as checked by the fixture comparison. Each execution covers 16 trace scenarios, including NaN and positive/negative infinity; all traces are equal.

| Fixture | Initial raw prefix | Recipient-only prefix/delta | Donor-only prefix/delta | Compound prefix/size | Complete body |
|---|---:|---:|---:|---:|---|
| Bool calls in loop | 26 | 19 / +5 | 19 / -5 | 80 / 80 | equal |
| Float comparison in loop | 26 | 19 / +5 | 19 / -5 | 79 / 79 | equal |

Negatives reject missing ancestor, nonempty else, terminating ancestor, nested callable boundary, custom operators, either missing IL flag, both missing flags, and absent opt-in. No target method match or byte gain is claimed by these toy results.

## Two further inspected examples (negative scope findings)

`form.GameForm::_update(System.Int32)->System.Boolean` (1,687 original bytes): current compound OR guard needs separating conditions, already covered by `condition-split-or-guards`. Original 14 locals versus rebuilt 13; current complete source model does not equal the original signature. No later redundant branch donor was found. This is not an eligible balanced candidate.

`form.SubForm::_draw6(Graphics)->System.Void` (13,213 original bytes): original stores each ternary arm separately (`stloc.7` at IL316 and IL348); rebuilt joins the value before one store at IL346. `assignment-ternary-if` and its reverse already cover that shape; original local signature is not reproduced. Do not rename an exhausted rule as new. `SubForm._draw` also has missing/extra branch sizes, but the donor is not an ancestor of its recipient; the present narrow transform rejects that pairing.

## Status

CONFIRMED in toy fixtures: two opposite 5-byte source edits can preserve an earlier branch operand and close a complete body where each isolated edit regresses its raw prefix. CONFIRMED by source-only prediction: the narrow Debug compound candidate preserves the complete original local signature. INCONCLUSIVE until parent's target compilation: whether it advances Debug's current prefix or closes the target. Other examples do not establish a second eligible target. No active helper, model, game source, ledger, or receipt was edited by this bundle.
