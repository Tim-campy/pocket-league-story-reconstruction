# Round11 initial queue: five source layouts, 8,074 original IL bytes

Private preparation only. No game target was compiled or executed. No public source, tools, configuration, ledger, original artifact or earlier frozen study was changed. Source, metadata and IL are frozen Round10 final-corpus inputs; the operator must rebind current source and validate the new model on the full matched corpus before production use.

## Deliverables and exact readiness

`source-patches.json` contains five complete-source-declaration patches with UTF16 spans, before text and raw file/method SHAs. `calculator.py` exports pure functions:

| Method | API | Original IL | Public model before → after | Private model after |
|---|---|---:|---|---|
| AppData.SendSaveData(String,Int32,Exception) | save_concat_and_dispatch | 2,207 | incomplete43 → incomplete41 | incomplete42, exact type sequence; CS0030 remains |
| GamePad.AddTouch(Graphics,Single,Single,Int32,Int32,Boolean) | gamepad_layout | 1,685 | complete22 → complete20 | complete20, exact |
| BillingManager.ConnectRegistMarketPurchase | billing_concat | 395 | complete11 → complete10 WRONG | complete11, exact |
| BootForm.Load | boot_layout | 3,277 | complete37 → complete37 reordered | complete37, exact |
| AppData.Draw(String,Boolean) | app_draw_layout | 510 | complete6 → complete5 | complete5, exact |

Full details and exact unsupported reasons are in `model-comparison.json`. All public before predictions match the rebuilt emitted type sequence, including Send's incomplete prediction. Billing's public after prediction is a genuine complete-but-wrong model result, not permission to compile or override a gate. It requires the new model capability. Send is genuinely incomplete both before and after, never relabeled complete; handling it must follow the existing incomplete budget and actual emitted-signature/full-oracle checks. No compiled signature of any target candidate exists in this study. Target body equality remains UNKNOWN for all five, even where the local layout is explained completely.

## Derived source forms and roles

Boot: original String[] slot0 is used throughout the try; JarInflater remains root slot1; RecordStore slot2 and byte[] slot3 belong to the try. Current root RecordStore0 and try String[]3 are the wrong declaration owners. Hoist only the bare String[] before JarInflater, put RecordStore's declaration back on its existing assignment in the try, and leave every initializer/call in place. Remaining locals4–36 retain order. No call or resource close was moved.

Draw: original shared String0 is used by both mutually exclusive message arms. Declare it bare in the parent; replace both arm declarations with assignments. Canvas stays1; Graphics is declared on BeginPaint assignment so it becomes2, Font3, num4. The current artificial Graphics root declaration and two arm-local strings explain all six versus five slots. Neither message evaluation nor painting calls are moved.

GamePad: remove only the synthetic Boolean `recoveredCondition_12162` local and test text==null directly. Preserve the original outer switch on id (first evaluated/stored at103, table109). Three nested switches recovered as tables are replaced by stable Int32 parameter if/else-if chains, preserving case order and grouped0/5. Removing their overlapping cache lifetime leaves the original single Int32 temp19. The inner21/22 and2/3/4 dispatches keep their leaves. The converter rejects nonterminal transfer ownership rather than removing arbitrary breaks. It skips the outer default switch; its only accepted switch cases are exactly the reviewed lists. Parent must retain the proven Int32 parameter binding, not apply this lexical matcher to arbitrary property/enum/custom-operator selectors.

Billing: original String temp10 is stored at90 and read100 before an Object[] concatenation, while all named locals0–9 precede it in metadata. Current `string text2=text; text=text2+...` wrongly makes that role named slot4 and shifts every later declaration. Restore `text += purchaseRecord.productId_ + "," + purchaseRecord.orderId_ + "," + purchaseRecord.state_;`. The compiler's params-array lowering, not a late textual declaration, creates temp10.

Send: the same artificial concat alias text10 originally corresponds to an emission temp41 (store610/read620). Restore compound append text8+=...; text11/text12/num2 then occupy original13/14/15. Convert the two type switches to if/else-if in their existing1/2/0 and1/2 orders, preserving the explicit block in case2 and try/catch in case0. This removes the extra Int32 switch cache42. Original42-type sequence is reproduced by the private model. Numerous remaining control hunks/retargets are not claimed fixed by this layout proposal. It is a partial source candidate, not a whole-body match claim.

`roles-and-ranges.json` in each method folder lists original/rebuilt first/last offset and use count for every mapped role, plus all per-slot accesses. These are diagnostics, not a token-renaming acceptance oracle.

## Why the private NoEmit extension is necessary

`private-model.diff` is the exact delta from immutable `tools/Round7SlotModel.cs`; `Round11PrivateSlotModel.cs` and `private_model.py` are private source/helper/client. The extension is generic for string **local** compound-add assignments resolved to Concat params arrays. It contains no game method, type or variable-name dispatch. It flattens string addition, accounts for adjacent compile-time string constant folding, distinguishes the four-string overload from four-object params and allocates/releases the String spill through the model's existing temporary pool. It deliberately does not claim new field/property/indexer compound-target support. Diagnostic output additionally exposes CS0030; no error is relaxed.

Pinned Mono source `b6587926c0f146c8b12985744f33a13b39a22d57` supplies the rule, recorded with complete snippets and file SHAs in `compiler-citations.json`:

- expression.cs5965–5969 recognizes CompoundAssign.TargetExpression in ArrayCreation.ResolveArrayElement and replaces it with LocalTemporary.
- expression.cs6376–6378 emits that expression and stores the temporary **before** Newarr;6406–6407 releases it after the array is initialized.
- assign.cs LocalTemporary.Store calls GetTemporaryLocal, and codegen.cs GetTemporaryLocal uses the existing per-type pool or declares a new local. This is why the concat temp follows named metadata locals and is reusable by type.
- CompoundAssign.DoResolve wraps the existing target in TargetExpression before building the addition. It is not equivalent to inserting an arbitrary explicit named local at the late metadata position.

Private model source and binary are separate; no public helper/cache was replaced. The operator should publish a new Round11 namespace/cache, preserve older helpers, and validate the whole matched corpus before D1. Do not mark Billing incomplete artificially to evade the public wrong-signature veto.

Send's real unsupported diagnostics are casts from System.Exception to kairo.unity.io.ConnectionException and kairo.unity.ui.UIException in AppData.cs5602/5606 of the candidate snapshot. They already occur in baseline. This study neither alters references nor asserts why the historical smcs accepted them. They remain `semantic_errors:CS0030`; types resolve but completeness does not.

## Own fixtures and model tests

No fixture is a compiled game class: all bodies/stubs are new minimal prototypes. Current versus independent handwritten Control versus pure-calculator Candidate were compiled with pinned smcs2.6.5/optimize+/debug-/checked-/reference SHAs from Round10 final-corpus. Five whole bodies differ before and match Control exactly after, including header/locals/EH/resolved-body comparison. Five negative templates were refused. Each candidate's entire local-type sequence is correctly predicted by the private model and compared with its actual own compiled method.

One additional own library tests five model boundary forms: four strings (no spill), five strings (spill), four objects (spill), adjacent constant folding (no spill), nested parentheses (spill). All five complete predictions equal actual entire local-type sequences. This supports the added capability and its bounds; it does not replace the operator's matched-corpus validation.

Totals: four owned fixture-library compilations (Current, Control, Candidate, model controls); one owned reflection runner compilation; three successive private analysis-helper versions compiled during development. Zero target compilations. Five whole-body positive comparisons, ten complete own local-type-sequence checks, five template guard negatives. Earlier helper versions are retained rather than deleted. No claim that development required only the final helper build.

The own runtime runner calls Boot once, Draw for −1..2, Billing once, Save for −1..3 and Pad for −1..23. Current/Control/Candidate produce identical full trace bytes and6 counted calls. The runtime loads only the private fixture libraries, never the game. `runtime-comparison.json` pins these finite checks; it is not a gameplay test.

## Primary identity and history

REA method pages `rea-{send,pad,billing,boot,draw}.json` are original-file static evidence, all bound to artifact SHA3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949 and MVID48ce9f89-73d2-40be-bb24-22c807c04106. Tokens060004ea/0600052b/060005ff/06000114/060004ec identify the five methods in that build. Metadata/CIL decode is primary; user-string terminal-flag limitations in enclosing views are not used to normalize or modify the artifact.

Each method's history.json scans66 R7–R10 Assembly-CSharp engine receipts: all have0 actual trials for these precise stable IDs. No absence-of-trial is called a refutation. `history-sha-gates.json` proves each full method byte-identical across R9D40, R10C01 and R10D20 to the current frozen final input (15/15). The new proposals are derived from role ownership and actual compiler lowering; no generic brute slot permutation or previously refuted exact candidate is relabeled as a new trial. This bounded history scan does not claim semantic exhaustiveness outside the recorded receipts.

## Handoff

All five after methods remain source proposals. Four have complete private signatures; Send has matching types with legitimate incompleteness. Fresh model validation, source binding, isolated probes and full verifier adoption belong exclusively to the operator. Source APIs require the complete reviewed template and have no file I/O. `source-patches.json`, private model sources, fixture manifests, exact SHAs and `frozen-index.json` are the handoff. This worker stops preparation after the five-method package; no new family is queued here.
