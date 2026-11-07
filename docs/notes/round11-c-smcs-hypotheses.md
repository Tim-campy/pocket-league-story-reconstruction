# Round11 C4/C5: private, source-only derivation

No original, live src, ledger, config or public tool was modified. No game assembly was compiled. The only compiled binaries are new owned fixture programs. Target bodies remain UNKNOWN until the verifier. No new family was researched.

## Inputs and primary evidence

All target DTOs and source come from `build/round10/final-corpus`, commit `f172df0549b83136af370a9bdec0ad7a7ad7ed56`. Each `analysis.json` records raw source SHA, method SHA, UTF16 span, full O/R DTOs, locals and alignment. Primary REA evidence is retained in `r11_*primary*.json`: original Assembly-CSharp SHA `3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949`, MVID `48ce9f89-73d2-40be-bb24-22c807c04106`. Training token06000403 has3287IL/1373decoded, Map token060002f2 has2711IL/1512decoded, both complete CIL/no body issue. Primary evidence does not replace the normalized-body/header/locals/EH verifier.

## C4: corrected diagnosis and composed proposal

`game.UserData::SetTrainingCommonMethod(Boolean)` original newarr797/store14@802/load14@804 contrasts with current newarr797/dup802. The source is `Vector[] array=new Vector[2]{new Vector(),new Vector()};`; this is an ARRAY initializer, not an assignment chain. `expand_training_array` restores allocation followed by stores0 and1 in the same order. Own fixtures confirm that mold on smcs and Roslyn. A separate inverse-assignment-chain fixture changes smcs IL but Roslyn optimizes both forms identically; this is not claimed as the target fix.

Array-only NoEmit is complete but predicts64 versus original63; all first63 types are already exact, with extra Int32 `$switch`63. This partial is preserved in `source-patches.json`, excluded from its initial probe bundle. It is not a sufficiency claim or a game trial.

Original2371..2398 tests slots42/43, stores0 or Random(2) into EXISTING slot40, then tests slot40. Slot40 is already named Int32 `num23` in current source. Original2438..2441 tests40==1; values other than0/1 skip both bodies. The final composition therefore restores:

```csharp
if(num25<num26){num23=0;}else{num23=GameUtil.Random(2);}
if(num23==0){ /* existing case0 payload */ }
else if(num23==1){ /* existing case1 payload */ }
```

It evaluates RNG exactly once only on the original branch, retains both payloads, adds no local and does not assume RNG range. Source-only real NoEmit `model-composed.json` is complete,63/63types exact. `composition.py::transform_training_composed` is a pure API; it imports only the local calculator. It requires the existing unique `int num23=0;` and complete switch template. Do not call it on arbitrary semantic contexts without the frozen source/ID binding.

Other control differences remain later in the method: guard around1036 and vector-loop test placement152x..173x. The composition makes C4 eligible for an honest isolated oracle but does not predict whole-body equality.

## C5: corrected diagnosis; mirror mold and real Map proposal separated

Map remaining16 alignment edits are exactly eight pairs of original `ldc.i4.0; operand; sub` versus current unary `neg`. No remaining hunk is a mirrored comparison. The reviewed operands are CLI Int32 array elements and Int32 MAPCHIP_COAT_WNUM arithmetic. `restore_map_zero_subtractions` changes only those eight full assignment templates and preserves the two already matching floating COAT_BASEPOINT negations. Source-only real NoEmit remains complete,10/10types exact. Target rebuilt2703 versus original2711 is consistent with +8bytes, but this count is diagnostic, not target equality.

The requested operand-mirroring mold was independently implemented and tested on pure primitive parameter/local identifiers in BOTH compilers. `mirror_comparison(a,<,b,Single)` yields `b>a`; explicit negation remains `!(b>a)`, never `b>=a`. User-defined types, properties and calls are rejected. Signed-zero, NaN and infinity runtime controls pass. smcs false-branch `<` emits bge.un versus mirrored `>` ble.un; explicit `!<` emits blt versus mirrored `!>` bgt. Roslyn has the corresponding `.s` forms. All are compared as whole methods, not normalized opcodes.

Negative float complement `!(a<b)`→`b>=a` fails NaN on both (semantic4). Float `-a` and `0f-a` are NOT bit-identical: at+0 they yield -2147483648 and0 bits. Thus the real Map inverse-negation API is limited to proven unchecked Int32, rejecting float/checked contexts. No float operator was rewritten in Map.

## Own experiments and exact limits

`fixture-results.json`:10 library compilations,20 candidate/manual-control whole-body equalities,4 compiled negatives and6 rejected negative inputs. `composition-fixture-results.json`:8 more compilations,2 whole-body equalities,2 compiled repeated-RNG negatives and3 rejected inputs. One separate owned runner compilation. Total18 fixture library compilations +1 runner;22 equal whole-body comparisons;6 compiled semantic/body negatives;9 guard refusals. Current/control/candidate each execute owned reflection-loaded fixtures only. Compiler/runtime/reference SHAs and exact flags are recorded, pinned from the final-corpus manifests. smcs2.6.5 uses optimize+/debug-/checked-/Unity references; Roslyn1.1 uses optimize+/pdbonly/lang6/checked-/pinned references. No tool installation or compiler update occurred.

The comparison runtime tests49 Single pairs and49 Double pairs containing NaN, infinities and both zero signs; Int32 tests include MinValue andMaxValue. Array constructor order and count are checked. This finite matrix supports only the typed molds/guards shown, not arbitrary algebra, side effects, custom operators or generalized optimizer behavior.

## History and source citations

`history-summary.json` scans R7–R10 engine reports: Training66 reports/0 actual trials; Map67reports/19actual trial records including probe/full duplicates. The Map array-population trial was adopted partially in R9D09 (158→213→574); this package does not repeat it. Training array primitive was known but not target-compiled in those records. Map's other trials are retained by SHA and rule; no prior eight-site zero-subtraction proposal is present in this bounded scan. This is not a claim of exhaustive semantic history outside those reports.

`history-sha-gates.json` shows both current methods are byte-identical to R9D09after, R9D40after, R10C01after andR10D20after despite changing enclosing-file SHAs. Full before-method SHAs: Training `d723bc4dd9fbb0c9519612662979a07fca9c35ebca6747d148949a1e8af592b1`; Map `c7e7aa09e88086bc012430d4206daa7f7afff65b515cf7231c9fe6addb6b4f4b`.

`compiler-citations.json` pins Mono source b6587926c0f146c8b12985744f33a13b39a22d57. `expression.cs6293–6331` explicitly duplicates the array for each dynamic initializer;6372–6405 emits newarr then dynamic initialization. `expression.cs3493–3538` emits left then right and distinguishes floating/unordered false comparisons. Both-compiler output/runtime proofs, rather than an unverified Roslyn source recollection, establish the fixture claims.

## Handoff

Use `final-probe-bundle.json`, not the preserved array-only initial bundle. It contains C4 composed and C5 Map source-only whole-method patches with UTF16 spans/before/source SHA. APIs `composition.transform_training_composed(text)` and `calculator.restore_map_zero_subtractions(text)` do no I/O. Parent must rebind fresh source, run fresh NoEmit, isolate/probe and use the full verifier before any adoption. This worker did not change any matched state. Hypotheses and scope of each negative are explicit in `hypotheses-final.json`. `frozen-index.json` pins all source/proof artifacts. No further family starts after this delivery.
