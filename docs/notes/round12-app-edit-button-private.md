# Private UserData.Draw_editButton: direct dispatch plus coordinate copies

Exact ledger ID: `game.UserData::Draw_editButton(kairo.unity.ui.Graphics,System.Int32,System.Int32,System.Int32,System.Int32,System.Boolean,System.Int32,System.Int32,System.Boolean,System.Boolean)->System.Void`. This is the non-generic ten-parameter overload, token060003fd,447 original IL. The eight-parameter overload token060003fc has22 IL and was excluded before analysis. Snapshot: `build/round12/runs/b13/commit/postverify01/Assembly-CSharp`.

No game target, verifier, production/source/public/config/ledger edit or additional family. All previous private packages remain unchanged. **Game whole-body result UNKNOWN.**

## What the first pair actually means

Original has five emitted locals, all Int32. Current has two: num0 plus switch temporary1. Original/current fat12-byte headers have flags19/initlocals=true,maxstack32,no EH; sizes447/430. The first aligned brtrue232 follows the Int32 `type` argument and skips the type0 assignment; current instead stores that selector into cache1 and emits a seven-entry switch. This is not evidence of an emitted Boolean local. The Bool select/isTouch/isMarker parameters remain normal conditions.

Original direct dispatch reloads type and compares0..6 in source order. Each selected arm assigns num, with constants786432,851968,917504,983040,1048576,1114112,1179648; default retains the existing initial num=786432. Case0 uses the zero-comparison false branch brtrue232→248. The other comparisons are bne.un251/270/289/308/327/346, not a table. The bounded producer restores this if/else-if chain while preserving every assignment leaf and removes only terminal case-owned breaks.

The remaining difference is four explicit argument copies **after** the dispatch join and before the marker condition:

```text
357 ldarg.2          // dx
358 stloc.1
359 ldarg.3          // dy
360 stloc.2
361 ldarg.s param3   // W
363 stloc.3
364 ldarg.s param4   // H
366 stloc.s4
```

The marker true arm loads locals1/2/3/4 at378/379/380/381; the false arm at417/418/419/420. The selected normal C# form is:

```csharp
int touchDx = dx;
int touchDy = dy;
int touchWidth = W;
int touchHeight = H;
if (isMarker) { /* existing AddTouch with these four values */ }
else { /* existing AddTouch with the same four values */ }
```

The proposal places these declarations in the existing isTouch block after the chain; this is a tested way to obtain the proven slot/store order. The historical local names and unique lexical declaration scope are **UNKNOWN**. IL proves the stores and their uses, not that no other declaration spelling/scope could emit the same program. No four-value assignment chain is inferred: the original loads four distinct arguments and has no dup for these stores.

The local count calculation is2-current −1-cache +4-coordinate aliases =5-original. This must be one composition; restoring the dispatch alone yields1 local, while inserting aliases but keeping the switch yields6. All existing drawing arithmetic, casts, calls, literals, TouchOption factory/margin expressions and prefix rendering remain unchanged. Only the coordinate argument uses in the two existing AddTouch calls change from parameters to the copied locals.

The diagnostic byte account is +8 for direct dispatch layout, +11 for the four stores, −2 for compact local loads replacing two coordinate-argument groups, total+17 (430→447). It is a calculation from original/current regions, not a measured game-candidate result.

## Models, owned fixtures and negatives

The unchanged real public R11 NoEmit model predicts **complete5/original-exact** with roles num0,touchDx1,touchDy2,touchWidth3,touchHeight4. No incomplete state was forced and no complete-wrong model was bypassed. The names are readable choices, not claimed historical names. Candidate header/maxstack/body/metadata still require the exclusive operator's emission and normal verifier.

Seven newly authored mock libraries preserve the ten-parameter calling context, drawing prefix, integer dispatch and both fluent TouchOption paths. They do not compile the game UserData class. Installed smcs2.6.5 SHA26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0; real options/references and private outputs are retained.

| Own variant | IL /locals | Entire method vs independent manual control |
|---|---:|---|
|Current switch/direct arguments|225 /2|different|
|Manual chain/shared-coordinate-copy control|242 /5|equal|
|Calculated full source|242 /5|equal|
|Dispatch alone|233 /1|different|
|Aliases alone, switch retained|236 /6|different|
|Swapped alias declaration order|242 /5|different|
|Tested root-scope alias construction|249 /5|different|

Full comparison includes instruction operands/resolved bytes/header/maxstack/locals/EH, not counts or a prefix. The last two negatives demonstrate why five Int32 types do not establish the correct roles or initialization point. The tested root form allocates coordinate aliases before the child num; its failure is not a rejection of every imaginable root bare-declaration form.

Three owned NoEmit predictions (before/control/candidate) are complete and match actual emitted signatures. All seven variants pass240 semantic combinations (three coordinate sets, selected/touch/marker flags and ten type values including defaults), with exact AddTouch/creation/margin counts and copied coordinate/value/option checks. Thus runtime equivalence does not select the byte-correct form. Six producer negatives are rejected: changed case value, effectful selector, modified coordinate argument, alias-name collision, marker not terminal in its owner and lambda capture. All fixture compilations succeeded. No Boolean-local variant was compiled or falsely reported as a failed target test.

## Evidence, history and exact binding

REA authenticates token060003fd, ten parameters/non-generic signature and all204 instructions against original SHA3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949/MVID48ce9f89-73d2-40be-bb24-22c807c04106. The raw CIL hash equals the frozen original DTO. Selected body/metadata is complete; wider selected-type string limitations remain in the saved evidence and do not authorize changes.

Compiler citations retain exact local SHA/excerpts from official Unity Mono pin b6587926c0f146c8b12985744f33a13b39a22d57: If.DoEmit233–269, Block.EmitMeta2010–2052 (named declaration ordering before child/emission temporaries), SwitchDoEmit3825–3869 and key/section layout. They explain why aliases declared after the switch text still reserve their named slots before its emission temporary, and why the switch must also disappear. Exact manufacturing revision of the installed executable remains UNKNOWN; owned output establishes actual behavior.

`history.json` contains87 retained R7–R12 reports for this exact overload and0 actual compiled trials/steps. That is bounded evidence only: it does not prove that every possible source candidate was absent from unretained/model-rejected history. The new after hash is absent from retained after hashes, and no old failed candidate is replayed as a new game test.

The live **UserData.cs file** changed after B13 through other work. The first private harness correctly stopped on the full-file SHA mismatch. The selected B13 before-method remains byte-identical and unique in the live file. This preparation keeps the authentic B13 input, records both full-file hashes/live UTF16 position, and **does not bypass the production gate**: the operator must rebind/recalculate against its fresh source and repeat the model/oracle. Harness incidents are recorded separately from source hypotheses.

Pure API: `calculate.py::transform_edit_button(method_text)`, with components `direct_dispatch` and `add_coordinate_aliases`. Exact before/after source, original/current DTOs, UTF16/SHA bindings, proof/role table, compiler/client hashes and engine-compatible normal-parent proposal are retained. Header-renaming and Path-I/O-forbidden calls reproduce the payload. Ready only for a fresh isolated game probe; zero gains are counted by this private study.
