# InsertGameObject: private source hypothesis, 253 IL

The ten-parameter overload is `main.AppData::InsertGameObject(Int32,Int32,Int32,Int32,Int32,Int32,Int32,Int32,Boolean,Int32)->Void`, token `060004d4`. Four matched wrappers of 21/22/22/23 IL are excluded. The source/DTO baseline is immutable `build/round12/runs/b19/commit/postverify01/Assembly-CSharp`; full file SHA, UTF16 method binding, original/rebuilt DTOs, headers and alignment are recorded in `analysis.json`. `source-patches.json` is a source proposal, not a matched-state declaration.

Original and current bodies are both253 IL, fat header12/flags19/maxstack5/initlocals, two local types Int32 and Int32[], and no EH. REA confirms253 IL,127 decoded instructions, token, original SHA/MVID and raw CIL hash; `rea-proof.json` independently matches the DTO CIL bytes to REA. No original bytes are copied into generated output.

## Actual edit, after branch alignment

The first raw mismatch at61 is the target operand of `blt` at60. Both sides perform the same signed byte-count/Int32 comparison against30; the comparison is not the cause. Current code has `br75` at65 and `br85` at70, placing the insertion tail outside the retry loop. Original code increments depth at65, continues at70→20, then inserts inside that loop from75. Original footer is `242 br252; 247 br20; 252 ret`; current footer has no corresponding two branches. The producer retains the initial pool-capacity guard and bounds guard, uses `if(count>=30){depth++;continue;}`, moves the unchanged insertion tail inside the same `while(true)`, then emits ordinary `break;`. It neither changes comparisons nor introduces casts.

`calculate.py::transform_insert(method_text)` is pure, header-name independent and constrained to the reviewed structure. Tail raw text, calls, strings and numeric literals are preserved. No generated class or IL injection is involved. The original local names and exact historical lexical declaration scope remain UNKNOWN; the predicted role/slot relationship is num:Int32/0 and array:Int32[]/1 in both versions.

## Owned controls, not game builds

Five own library compilations use the pinned installed Unity smcs2.6.5 flags/references, with private output files. The independent manual control spells the integer condition as `!(count<30)`, while the calculated candidate retains `>=30`.

| Own source | IL bytes | Locals | Entire body equals manual control |
|---|---:|---:|---|
| Before |253|2|No|
| Manual control |253|2|Yes|
| Calculated candidate |253|2|Yes|
| Tail return instead of break |248|2|No|
| for(;;) instead of while(true) |258|2|No|

Comparison includes resolved instructions/body bytes, header/maxstack, local types and EH. Candidate reproduces60blt75,70br20 and footer242br252/247br20/252ret. All five variants pass240 mock semantic cases each, illustrating why semantic equivalence alone is insufficient. Six producer negatives reject capacity/threshold mutations, a new effect, tail return, capture and different loop syntax. Genuine unchanged public model predicts both target source versions complete with the original two types and unchanged named roles; three owned model checks also pass. Nothing is made complete by overriding flags. `fixture-results.json` contains actual results and hashes.

## Compiler and historical evidence

`compiler-source-evidence.json` cites Unity Mono pin b6587926c0f146c8b12985744f33a13b39a22d57, statement.cs426–471 (While constant loop emits unconditional backedge),1115–1118 (Break to LoopEnd),1141–1150 (Continue to LoopBegin),571–619 (For emission),2010–2052 (locals/child blocks). Exact manufacturing revision of installed executable is UNKNOWN; owned fixtures establish its behavior.

Bounded history:91 reports examined,8 actual trials,1 adopted partial. R7 empty-arm partial created the present shape; plain polarity and guard-break-reverse regressed61→60, declaration variants stayed61, and R10 root-positive-guard changed the initial capacity guard and regressed61→14. None of retained candidate after-text hashes equals this calculated whole method. This is not a universal assertion that no equivalent source has ever been tried.

Readiness: PRIVATE READY FOR ISOLATED GAME PROBE ONLY. Game body equality, target metadata impact and full regression count are UNKNOWN. Target compilations0, target verifier runs0, public/src/config/ledger writes0. The production owner must revalidate the exact method/full-file binding and perform its own gate/oracle. Freeze only indexes this private directory; no old package is changed.
