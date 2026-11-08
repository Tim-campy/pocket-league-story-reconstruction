# Round11 private residuals: TouchEvent.ToString and Image.GetAreaSize

Two source proposals only, total682 original IL bytes. No game target was compiled or run. No source, ledger, public configuration/tool, model8 source, original artifact or earlier frozen study was modified. The operator chooses whether/when either family is eligible after current C/queue work; this study neither inserts a production queue entry nor counts a cycle/gain.

## Exact inputs and primary identity

The source/DTO/local signatures are from `build/round10/final-corpus/KairoLibrary`. Both full live source files were byte-identical at initial read; that is a recorded observation, not a promise after C5 or later operator changes. The whole-method before text/UTF16 spans and raw source/method SHA live in each `analysis.json` and `source-patches.json`. Method hashes:

- TouchEvent.ToString: `0d186df762cbffc8371372076e2a42d99aa92c047a3bf89a2bab35bacfbc0f70`.
- Image.GetAreaSize(Byte[]): `088bb2b81af11814e77ec554e6f120a8551efe032745c23b8db459a520a97408`.

REA `rea-summary.json`/`rea-method.json` under each folder are original-file static evidence. TouchEvent method token06000705 (372 IL) is bound to parent ev_bf579bfdd468fb8b2a49ec5d0772d19eb6fed772f00f938442ee9e3b917de4a3 and page ev_528ca52ac705193c852bdc53181c2240443a4ece572d9b66a811c0d57d78bbdc. Image token060003f2 (310 IL) is bound to parent ev_190df691736338c9aacb170110e33adf5ae4408c4762cc6a97d48999b5cdbfef and page ev_d3ffdb8f3332049add3f07cd84f9d82e10f72278ff3f08bef07fe835f31dbd88. Artifact SHA/MVID and decoded completeness are retained verbatim in these files; tokens are not portable across artifacts. These pages do not replace the full verifier.

## TouchEvent: assignment control, not value ternaries

Original after type0 emits each successful type1..10 comparison/body in source order, stores text in slot0 per arm, and jumps to common join286. E.g. original61 bne.un.s74,63 ldstr TYPE_UP,68 stloc0,69 br286. Current61 beq266 defers the string to a reversed ladder of value arms with a single store at the shared value join. Original13 likewise tests positively, stores TYPE_ON at256 and skips the NONE fallback259..285. These are ordinary if/else-if assignments, not a reordered set of ternaries.

`transform_touch_event(text)` replaces the entire reviewed chain with type0,1,...10,13 positive assignment arms and NONE fallback, preserving the initial component-null branch and final message construction exactly. It does not attempt to fix unrelated behavior in those already matching regions. The type field is CLI Int32; no floating comparison or custom operator is rewritten. Each type is still read in the same semantic source order until a match.

The own Classifier.Describe fixture differs from the game and does not override ToString. It includes the same classifier construction with owned surroundings. Candidate and independent manual Control whole bodies match; Current284 versus Candidate298 is +14, consistent with target358 versus372, but only the verifier can establish the target result. Compiler code explicitly explains why the recovered ternary ladder reversed value-arm emission: EmitExpression.cs2646 branches to the true arm and2647 emits Alternative first;2690 later emits Consequence. Ordinary lowered statement branches retain their stores and joins. Do not apply a universal textual-order assumption to value ternaries.

## Image: selector scope plus the marker-loop exit

Original starts with array local0 and a named/cached format String1, then array assignment temp2. Its gif test falls through directly into gif body and the next test is after that body; png is similarly interleaved. Current switch first performs all string tests before any body, uses String3 and a root cursor Int321. A parent String declaration plus if/else-if chain gives the required order. The String is assigned from GetFileFormat(data) once, not reevaluated per comparison. Existing fileFormat-name collisions and unreviewed/default cases are refused.

The source bare `int num;` is removed from the parent, and the existing `num=2` becomes `int num=2` inside the JPEG block. No initialization is moved. The assignment `array[0]=(array[1]=0)` is preserved; its Int32 assignment temp stays2. The original JPEG cursor is3; length is4. The marker num2 is held on the evaluation stack across cursor advancement and length evaluation, so it has no original local slot.

Original265 bne.un.s301 skips the dimension writes to the increment. On marker65472, dimension writes267..298 execute and299 branches to308, the arm/common exit. Current instead uses equality to break the infinite loop, increments on nonmatch, and writes dimensions after the loop. The proposal moves exactly the two reviewed assignments into the terminal positive if(num2==65472) before its existing break. The false-path increment remains after that if, preserving marker/data reads and break ownership. There are no EH regions or loop-owned continues to guess in this method.

`transform_image_area(text)` composes selector alias/if chain, cursor scope, and marker-positive tail. Masks, shifts, byte offsets, expression operands and all calls remain unchanged. The own parser has deliberately smaller gif/png payloads, so its Current/Candidate both174 IL; this is not claimed as the target's316→310 result. In the actual target, interleaving tests permits two previously long string-dispatch branches to become short; the -6 is diagnostic until compiled. The own parser's full actual signature after transformation is Int32[],String,Int32,Int32,Int32, identical to the original target pattern.

## Genuine model8 limits, unchanged

`model-before.json` and `model-after.json` are authentic model8 outputs; source/client hashes are pinned. No private override or model mutation was introduced here.

- TouchEvent candidate is incomplete for the preexisting `string_concat_value_temporary` capability gap, and predicts the correct one String type. It is not relabeled complete.
- Image candidate is incomplete for `stack_scheduler_multiple_use:fileFormat` and `stack_scheduler_single_use:num2`; it predicts six types, including a marker Int32 that real Roslyn schedules on stack. The own compiled Parser has five, while the same genuine model predicts six and reports the same reasons. This directly demonstrates the model gap in a toy, not an emitted signature of the game target.

The operator must follow the established incomplete-model budget and check the actual emitted original signature if probing these candidates. Do not edit expected states, mark the model complete, replace its six-type result with the toy's five types, or treat the fixture signature as a compiled game signature. A fresh complete-but-wrong model result remains a veto.

Pinned Roslyn1.1 source (tag version-1.1.0, commit4cb203dab89e1542cdf7d88cb6d32767bc0e5460) supports block-local allocation and stack scheduling: EmitStatement.cs594–612 defines block locals before its statements; Optimizer.cs54–74 runs the stack optimizer, and1926–1940 retains a stack-local assignment without allocating a normal local. `compiler-citations.json` verifies Git blob SHA1 against the pinned source tree and includes exact lines/file SHA256. It does not claim a complete independent implementation of the scheduler.

## Own experiments

Five owned libraries compiled with csc1.1.0 and the exact final-corpus flags/reference hashes: Current, independent manual Control, Candidate, wrong classifier label, duplicate format-call negative. Both proposed method bodies equal their manual Controls including headers/locals/EH/resolved-body comparison. Both Current bodies differ. Two compiled negatives differ and fail semantic checks (wrong classifier=1; duplicate call=3). Five guard negatives reject changed label, extra assignment, alias collision, different JPEG marker and an extra format case.

The runtime executes only owned fixtures: type cases0..14 and−1 plus the owned null-component prefix; gif, png, unknown, JPEG first-marker hit and JPEG skipped-marker then hit. It checks result dimensions and exactly one GetFileFormat call per parser invocation. No game code executes. Five fixture compilations, zero new runner compilations (reuses an owned frozen runner), zero game compilations. Compiler/runtime/reference identities and actual signatures are in fixture-results.json. Finite own tests support these molds, not arbitrary format parser restructuring or type-dependent comparisons.

## History, binding and handoff

Each history.json scans42 R7–R10 Kairo engine receipts for the exact stable method ID: zero actual trial records for each. No trial absence is called refutation. Six SHA gates prove both whole method sources byte-identical at R9D40, R10C01, R10D20 and this final frozen input. This is bounded receipt/source history, not a claim about every semantically related experiment ever considered.

The source calculator exports two pure APIs, independent of method headers and without I/O: `transform_touch_event(str)` and `transform_image_area(str)`. It replaces reviewed complete token templates, preserves payloads and refuses unsupported layouts. Fresh method/source binding is required after C5; no future usage or gain is assumed. `source-patches.json` has engine-compatible fields, honest incomplete flags and UNKNOWN target-body state. The operator alone may publish/rebase/select a cycle, build a target, verify or adopt. Stop preparation after these two; no further family is queued by this worker.
