# SendSaveData protected retry composition — private R12 preparation

Only `main.AppData::SendSaveData(String,Int32,Exception)->Void`, token060004ea, original2207 IL. Source/IL comes from immutable `build/round12/runs/a11/commit/postverify01/Assembly-CSharp`, checked equal to the live source at packaging. Source text and offsets are in `source-patches.json` (UTF16 declaration span, complete file SHA, exact before/after). `calculate.py::transform_send_save(method_text)` is pure and header-name independent. No generated class is authored. **Zero target compilation, game execution, verifier call, public/source/config/ledger change or commit. Target result remains UNKNOWN.**

The retained original REA page copied as `rea-original-send.json` pins artifact3620ed91..., MVID48ce9f89..., token060004ea,2207 IL and raw IL SHAe032583988bb64275cc098df99e383cdb0f435dea2d28b42445b5724f7b75ad0. Packaging independently recomputed that SHA from the frozen original method bytes; it did not load/run the original. Current body is2232 IL. Both existing headers are fat/maxstack133/initlocals with42 resolved local types; candidate target header/bytes have **not** been emitted here.

## History and existing prefix

71 retained R7–R12 Assembly-CSharp engine reports contain this exact stable ID. They record12 actual trials and one adopted step: R11's concat/local/dispatch correction611→986. The other11 trials were one empty-arm restore, one condition-local and nine scope hoists; none was accepted. Their hashes/source snippets/outcomes are retained in `history.json`. This is a bounded recorded history, not proof of universal absence of related source forms.

Current method raw SHA differs from the old adopted payload SHA only because one blank line lost7 indentation tabs. Token streams are equal, explicitly shown in `historical-binding.json`; the body oracle is never normalized by this history comparison. The new complete method after SHA is not any retained old candidate after SHA. It combines all remaining control edits, rather than repeating an old isolated empty-arm/local change.

The old raw prefix986 ends at an operand of the branch at985, whose destination is affected by later code sizes. The first actual aligned instruction replacement is997. Restoring only an isolated early guard can still leave that earlier operand wrong. The producer preserves all source before this late control region, all literal/call inventories and every predicted name/type/slot role; emitted target prefix preservation still requires the operator's actual compiler/oracle.

## Exact control readings and composition

1. Original985 `br2071` ends the sendState0 arm. Original997 `bne.un2071` skips the positive sendState1 arm to the shared Sleep **inside the same outer try**. Current997 `beq1007;1002br2091` is a negative goto guard. The candidate expresses one normal `if(state==0)...else if(state==1){...}` and leaves Sleep after the chain. This removes the extra five-byte guard branch without turning it into a loop continue or an EH exit.

2. The two original save_ ternaries test nonnull and emit the data arm first: original1418brfalse1441 and1571brfalse1594. Candidate uses `save_!=null ? save_.strs[0] : string.Empty` twice, preserving the same field evaluations and strings. These are reference-null/Int32 comparisons; no floating-point operator/.un assumption is introduced.

3. Original retry region has1816br1997 (condition), body1821, increment1991 and2003blt1821. A normal indexed `for` recreates this layout. The body has a retry label just before Draw. Successful Post ends1854`leave1991`, proving a continue to the for increment **inside the protected try**. The automatic extra1859`leave1986` is retained by the compiler. Failed Post catch falls through1981`leave1986` and1986`br1821`: retry jumps directly to Draw without increment/test.

4. Inner catch original1917`bne.un1981` skips the positive cancel body. On Show()==8, the wait loop/reset/Clear finishes1976`leave2201`, unwinding out of both protected regions to the outer while's footer. This is **not** a break to successful completion at2008. Candidate uses an ordinary C# goto to a label after the outer catch, inside the existing while. The retry label is inside the for body, and `goto retry` is outside the inner catch. The two labels guide normal smcs emission; no IL byte or branch offset is copied into output.

5. Successful upload completion returns from the method (original2066`leave2206`). Shared Sleep2071 and automatic outer try leave2079→2201 remain separate. Outer catch original2137`bne.un2196` skips its positive Show()==8 wait/reset/Clear body and then takes automatic2196`leave2201`; candidate removes the explicit negative `continue` and uses that positive body. Footer2201 branches to DecideKeyState904. No loop owner or exception target is guessed from physical distance alone.

Original eight EH regions, their half-open extents and current extents are retained in `analysis.json`. The source composition keeps all existing try/catch payloads and catch types. Source literals/call identifier multisets are equal, including every Japanese message, Post argument and sleep/reset call; `source-proof.json` gives measured inventories. Full target EH equality is UNKNOWN until emitted.

## Compiler evidence

`compiler-citations.json` pins the official Unity Mono snapshot b6587926c0f146c8b12985744f33a13b39a22d57 and SHA of local statement.cs. Exact installed compiler manufacturing revision is UNKNOWN; the own controls use installed smcs2.6.5 SHA26b4f7c... and verify its actual output.

- If.DoEmit233–269 emits the false-edge skip and a join only when needed.
- For.DoEmit571–619 places entry-to-test, body, the current LoopBegin increment and true-edge in the observed order.
- Continue1146–1149 chooses Leave versus Br using unwind_protect and targets LoopBegin; Goto804–841 likewise selects protected Leave or ordinary Br for the resolved lexical label.
- Return770–782 uses the protected return label; Break1115–1118 targets the actual loop end, explaining why using the inner for break for cancel would be wrong.

## Owned fixture measurements and negative outcomes

Five owned libraries were compiled from new mock classes/methods, never a game class: before, independent handwritten Control, calculator Candidate and two negatives. Pinned smcs flags/runtime/reference identities are saved in `fixture-results.json` and each private command. No actual gameplay/Unity/JNI/RecordStore code is run.

The focused complete fixture retains the relevant outer catch, Post catch and two wait-loop ThreadAbort catches. It has14 locals and4 EH regions. Before606 IL differs from Control581 IL; Candidate581 IL equals Control across the **entire** method body, resolved tokens, header/maxstack22, all locals and EH extents. The−25 size difference also equals the actual game's2232−2207 difference, but that arithmetic is a hypothesis check, not a claim that the uncompiled2207-byte target has already been produced.

Two emitted negative controls fail the full-body comparison: cancel label inside outer try (same581 length/local count, wrong EH/target), and success continue after the protected try (576 IL, wrong emission/target). They demonstrate that length/local count alone cannot prove the reading. All three positive semantic variants ran only private code and returned0 over sendState0/1/2, retries, cancel, outer failure and null-save paths with checked call/reset/sleep counts. Five pure-producer negative inputs were refused (wrong state, Post receiver, increment, cancel tail and label collision).

Real NoEmit model11 predicts each of the three owned positive fixtures completely and matches all14 actual local types. For the actual game baseline and proposed source it remains **incomplete**, with the unchanged `semantic_errors:CS0030` caused by existing exception casts. Both predictions are exactly the original42-type signature, and every name/type/slot role is unchanged. No diagnostic or complete flag was suppressed, and no model was edited. This proposal requires the normal configured incomplete budget plus actual emitted-signature/full-body checks; a complete-wrong result must still be rejected.

The producer initially found the nested Sleep try when matching an unqualified unique `try` token. That implementation guard was narrowed to the exact `try{Post(` header before any owned compilation; no game candidate failed because of it. No old trial is relabeled as this new composition. After delivery this finite target study stops; production is solely the operator's responsibility.
