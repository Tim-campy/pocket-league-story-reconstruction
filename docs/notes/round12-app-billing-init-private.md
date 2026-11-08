# Private generic BillingManager.Init shift-mask diagnosis

Only `system.billing.ui.BillingManager::Init``1(System.String[][],System.String,System.String[])->System.Void` is in scope: token060005e4,707 original IL. Identity/arity/signature were checked before body analysis. The same type has three other Init overloads (19/19/10 IL), none substituted for this method. Input source/DTO/manifest is immutable B06: `build/round12/runs/b06/commit/postverify01/Assembly-CSharp`.

Zero game target compilations, target verifier runs, production/public/source/config/ledger edits or new families. The proposed **game body result remains UNKNOWN**. All previous private packages stay frozen.

## Measured insertion

Original and reconstructed have21 identical local types, fat12-byte header/flags19/initlocals=true, maxstack53 and no EH; original707 IL, current710. The complete alignment has exactly one inserted two-instruction pair, with0 independent retarget edits/0 width changes. The first raw prefix discrepancy in old receipts was61, but the first aligned difference is a repeated shift mask at113. No source statement or argument needs moving.

Original shift region:

```text
109 ldloc.3          // Int32 flags accumulator, recovered num
110 ldc.i4.1
111 ldloc.s4         // Int32 loop index j
113 ldc.i4.s31
115 and
116 ldc.i4.s31
118 and
119 shl
120 or
121 stloc.3
```

The current source `num |= 1 << (j & 0x1F);` emits THREE ldc31/and pairs at113/116/119 before shl122. The original already has TWO; the initial simplified hypothesis of a single original compiler mask was wrong. Sequence alignment can place the identical extra pair at113 or later; those identical instructions do not themselves identify which was historical explicit source versus compiler augmentation.

The bounded candidate changes only this reviewed expression:

```csharp
num |= 1 << j;
```

All calls, char/string literals, operands, arithmetic outside this expression, comparisons, casts, generic constraints, loops, initializers and array/group/sort logic remain source-byte identical. The producer checks the Int32 num declaration, Int32 j loop, char extraction and `c=='1'` owner. It does not generalize the rule to Int64, user operators or effectful shift counts.

## Why this compiler emits two or three masks

The exact local source SHA and excerpts are in `compiler-citations.json`, official Unity Mono pin **b6587926c0f146c8b12985744f33a13b39a22d57**:

1. `assign.cs629–631`, CompoundAssign.DoResolve, first resolves its right-hand expression.
2. `assign.cs661` builds `new Binary(op,left,right,true)`;693 calls `base.DoResolve`.
3. `assign.cs338`, Assign.DoResolve, resolves this new source tree; `expression.cs2632`, Binary.DoResolve, resolves its right operand again.
4. `expression.cs1814–1831`, PredefinedShiftOperator.ConvertResult, selects mask0x1f for Int32/UInt32 (0x3f for Int64/UInt64), then wraps the current shift RHS in a new BitwiseAnd/IntConstant mask. It does not deduplicate an already-present mask on this path.

Thus this local compound assignment revisits the shift twice: one explicit source mask plus two augmentations gives3; no explicit source mask plus two augmentations gives2. An ordinary assignment visits the shift once, so an explicit mask plus one augmentation also gives2. This is compile-time resolution, not duplicate runtime evaluation of j/CharAt.

The exact manufacturing source revision of the installed executable remains UNKNOWN; the above pinned source explains the path and the installed-smcs own fixture matrix below corroborates the observed3/2/2/1 counts. It does not imply a compiler/reference mismatch.

## Own fixtures, models and negatives

Six newly authored mock libraries contain a generic Run<T> with the same Int32 flags/index/char loop and a new()-constrained BillingItem construction. This is a focused generic-method region, not a copy or compilation of the707-byte game class. Compiler SHA26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0; actual commands, flags/reference hashes and output DTOs are retained.

| Own construction | Emitted mask chain | IL | Full body vs independent control |
|---|---|---:|---|
|Current `num |= 1 << (j &31)`|31,31,31|170|different|
|Manual control `num = num | (1 << (j &31))`|31,31|167|equal|
|Calculated `num |= 1 << j`|31,31|167|equal|
|Ordinary assignment without explicit mask|31|164|different|
|Compound assignment with explicit15|15,31,31|170|different|
|Compound assignment with `(int)(1L << j)`|63|166|different|

Comparison uses the complete ordinary method DTO, including header/maxstack/local signature/instruction operand identities and EH, without normalizing masks or casts. Candidate/control are identical in the entire owned method. All own variants have the same7 local types and no EH. Three own NoEmit predictions (before/control/candidate) are complete and equal to the actually emitted own signatures. All compilations succeeded.

Current/control/candidate and the one-mask negative pass36 input vectors (lengths0..97 crossing16/32/64 and three bit patterns), each run as single-item and two-item data (72 runs per positive variant). Equal runtime semantics therefore do not identify the byte-correct mask count. The wrong15 and widened-shift variants deliberately fail semantics with status1. Six producer negatives are rejected: wrong mask, j++ count, wide left operand, changed loop-index type, changed char guard and lambda capture. No cast or comparison is normalized as a proposed correction.

The unchanged real R11 NoEmit model predicts **complete,21 types exactly matching original order** for the full actual game parent method after the change. All21 name/type/slot triples are identical before and after. No incomplete prediction is forced to complete; this particular prediction genuinely is complete. Model source/client hashes, exact full-source/method SHA and UTF16 bindings are in the bundle/proof. A fresh game compile/header/body/meta comparison is still required by the production operator.

## Original evidence, history and handoff

REA authenticates token060005e4 with generic arity1 and exactly three parameters string[][],string,string[]. All293 original instructions decode with no body issue; raw CIL SHA equals the frozen original DTO. Artifact SHA3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949/MVID48ce9f89-73d2-40be-bb24-22c807c04106. Wider selected-type string-inventory limitations remain recorded and are not corrected here.

`history.json` retains82 reports and5 actual target trial entries. They changed empty-arm polarity or grouped declarations and stayed at prefix61; none changed this shift construction. Their failures are preserved. The new full after hash is absent from retained trial/step after hashes. This is a bounded check, not a claim of global absence of equivalent source variants.

The general phenomenon of unnecessary explicit Int32 shift masks was already used in the earlier GameView touch study. `related-known-rule.json` records that unchanged producer's SHA and its two specific id/component2.id templates. This is a newly evidenced application to the confirmed generic Init overload and compound-assignment double-resolution context, not a claim that all shift masking is a new discovery or safely removable everywhere.

Pure API: `calculate.py::transform_init(method_text)`. `source-patches.json` is a normal-parent-C# proposal; import/calls do no file I/O and the header can be renamed without affecting the transformation. Historical spelling remains nonunique: the owned ordinary-assignment control and compound candidate emit identical bytes. Readiness is for an isolated fresh game probe only. No matched method or IL-byte gain is counted by this private study.
