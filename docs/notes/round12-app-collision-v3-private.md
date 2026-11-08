# Collision V3: positive height plus original terminal guard return

Private, finite composition over **B06 actual adopted source**, not over the earlier V1/V2 proposal. No game target was compiled, no target verifier ran, and no production/public files were changed. V1 and V2 remain byte-identical to their frozen indices; `source-proof.json` verifies every indexed file. The result of the new game method is **UNKNOWN**.

## Frozen identity

Method: `game.Player::Collision()->System.Void`, original token06000325,2145 IL. Input source/DTO/manifest snapshot: `build/round12/runs/b06/commit/postverify01/Assembly-CSharp`, produced by commit `f873747c7cdd35bd7ca72c790826228d50f0216e`.

Original and B06 both have a12-byte fat header, flags19/initlocals=true, maxstack67, code size2145,31 local types, and no EH regions. These are measured **baseline** facts, not predictions about the uncompiled V3 target. The saved REA original page authenticates token06000325 and all703 decoded instructions against original SHA3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949. Its raw CIL hash equals the B06 original DTO CIL hash. This reuses the immutable original evidence; it does not run the game.

## Two remaining source causes

B06's first raw prefix difference1164 is the operand of `brtrue` at1163: original destination2052, current2053. This is an induced future-distance difference. The first aligned instruction edit is the height test:

| Original | B06 |
|---|---|
|1193 `bge.un 2047`|1193 `blt 1199`|
|1198 begins the role0 work|1198 `ret`;1199 begins the work|

The original therefore keeps `if (udat_.ball_.pos_.y_ < 20f) { ... }` around the complete remaining role0 work. V3 restores that shape from the B06 negative guard. The comparison remains **`<20f`**, not `>=20f` or its assumed float complement. NaN must skip the original positive region; the original false branch is unordered `bge.un`.

The second aligned edit is at the terminal role-nonzero branch:

| Original | B06 |
|---|---|
|2121 `brtrue 2127`|2122 `brfalse 2144`|
|2126 `ret`|no corresponding return|
|2127 starts ChTrapOrHeading path|later conditional path without that guard return|

B06 recovered it as a single else-if:

```csharp
else if ((!BitUtil.Check(udat_.ball_.flag_, 16)
       || !BitUtil.Check(udat_.ball_.flag_, 32)
       || udat_.ball_.boundNum_ != 0) && ChTrapOrHeading()) {
    ChRepetition();
}
```

V3 restores the distinct return and subsequent call condition:

```csharp
else {
    if (BitUtil.Check(udat_.ball_.flag_, 16)
        && BitUtil.Check(udat_.ball_.flag_, 32)
        && udat_.ball_.boundNum_ == 0) { return; }
    if (ChTrapOrHeading()) { ChRepetition(); }
}
```

These two regions must be composed. Removing the misplaced height ret alone changes the length/targets before the tail; restoring the missing tail ret balances that byte in the proper region. The baseline alignment has exactly these two sequence edits,0 independent retarget edits and0 branch-width changes. This diagnosis does not replace the full oracle. The DEBUG guard's real original return is1382 (0x566); it remains byte-identical source. Original rectangle target1047 is still j's increment; V3 does not modify that region or reopen its ownership.

## Real model and source binding

`calculate.py::transform_collision_v3(method_text)` is a pure, header-independent normal C# producer with exact token templates, a checked role0 block owner, and no file reads during calls. It does not write a generated class or an IL byte. Its two component APIs are `positive_height` and `restore_tail_return`.

`method.before.cs` is the exact B06 parent declaration; `method.after.cs`, `source.after.cs`, and `source-patches.json` preserve its normal C# parent. UTF16 start/end, full-file before SHA, before/after method SHA, producer/model/client SHA and reference manifest are retained. The live Player file still equaled the B06 source at packaging. The production operator must nevertheless revalidate fresh identity before any probe.

The unchanged real public R11 NoEmit model reports **complete,31 local types, exactly the original order**. Every one of the31 variable name/type/slot role triples is identical between B06 before and V3 after, including the four unused Singles added in V1. No incomplete flag was forced and no complete-wrong model was bypassed. Target header/maxstack/body equality remain UNKNOWN until emission.

## Own fixtures and negatives

Six newly authored mock-library variants were compiled with the installed Unity smcs2.6.5 and the same optimization/debug/language/checked/reference options in the copied manifest. Compiler SHA26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0. No target class/source was passed to the compiler.

| Own method | Calculated candidate | Independent manual control |
|---|---|---|
|RunHeight|133 IL,2 locals|entire body/header/locals/EH equal|
|RunTail|126 IL,0 locals|entire body/header/locals/EH equal|
|RunCombined|284 IL,2 locals|entire body/header/locals/EH equal|

All three before methods differ from their controls. Comparison uses the complete ordinary method DTO via `compare_kairo.differences`, including header/maxstack/locals/normalized operand identities and EH; no prefix or diagnostic normalization is accepted as equality. All own methods have zero EH, as does the game method. Nine genuine NoEmit predictions (before/control/candidate across three methods) are complete and equal to the actually emitted own local signatures.

Current/control/candidate each pass **10752 semantic combinations** of height (including NaN, infinities and signed zero), role, manager states, bits16/32, bound count, hit/trap, DEBUG and team, with bit-reader/trap/repetition evaluation counts. Three deliberately emitted negative variants differ in complete combined body and return semantic failure1:

- replacing `<20f` by `!(>=20f)` admits NaN incorrectly;
- removing the restored tail return allows an unwanted trap/repetition path;
- removing the pre-existing DEBUG return executes guarded work.

Five producer negatives are rejected: changed height threshold, changed bitmask, incorrect role owner, changed DEBUG guard, and lambda capture. No fixture compilation failed. All tests and compiler commands/output DTOs are retained in `fixture-results.json` and `fixtures/`. Fixture equality proves the emission templates on owned code; it does not assert equality of the uncompiled full game candidate.

## History and limits

`history.json` covers79 retained R7–R12 method reports with60 recorded trials and30 distinct trial-after hashes. B06 probe and main deliberately repeat the same30 trial sources; this is not60 unique candidate programs. The new complete V3 after hash is absent from these retained after hashes.

The actual B06 reports record initial raw prefix60, V2 partial60→1106, then legacy `tail-early-return` partial1106→1164. Earlier conversational58 was not the value in these frozen main/probe receipts. Neither step matched the body. B06 official adoption was0 new matched methods/0 losses, with partial source changes retained. Thus **V2 as sufficient whole-body reconstruction is REFUTED**; the observed local progress remains real. The legacy ret's partial prefix advancement does not establish its historical location. V3 is a distinct composed hypothesis, not a rerun of that isolated guard.

`compiler-citations.json` pins official Unity Mono source b6587926c0f146c8b12985744f33a13b39a22d57: If.DoEmit false branch/join (`statement.cs233–269`), direct/protected return emission (`statement.cs754–804`), and ordered/unordered float comparison selection (`expression.cs3493–3566`). Exact installed manufacturing revision remains UNKNOWN; own fixture outputs establish behavior on the actual pinned installed smcs. No target compilation, target metadata comparison, or matched-state transition is claimed by this package.
