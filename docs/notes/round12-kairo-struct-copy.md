# Round12 private LineupData.GetRatio struct-copy hypothesis

One method only,192 original IL bytes. All writes are under this private WIP. No game assembly build/run, target verifier, src/ledger/public configuration/tool change, original mutation or commit occurred. All prior Round11 frozen studies remain untouched. No other family was opened.

## Exact input and byte evidence

`kairo.unity.system.lineup.ui.LineupData::GetRatio()->System.Int32`, original token06000671. Input is `build/round11/final-corpus/KairoLibrary`; the live source was byte-equal at initial read and the proposal is bound to complete before text, raw file/method SHA and UTF16 span in analysis.json/source-patches.json. Requested new-round base is a075dcc. Actual rebuilt DTO is from the official frozen final-corpus receipt, not a hypothetical recompilation.

Original/rebuilt headers agree: fat12bytes, flags19, InitLocals, MaxStack3,192IL. Both local signatures resolve to Boolean,Int32,GameAppli[],Int32,Int32; EH and all other compared facets agree. Exactly one metadata-resolved body byte differs: body83 = IL71, originalA3 ldelem.any versus current8F ldelema. The following four-byte GameAppli operand and ldfld Pid at76 are otherwise identical. Raw bodies also differ at header byte8 (local-signature token32 versus30); that token resolves to the same five-type signature. This is measured separately and no assembly is normalized or patched. body_cells is the existing allowed metadata-identity comparator, not an output rewrite.

REA original method evidence ev_f8c150dc1c498b40e120c4300c0f0a20730922e5d4059c45a9c2c521142ef2b9 is bound to parent ev_b0ee12b57fba06253994ebc9d5ac46d622e2d45424c748bf39295b60ea1ab575 and token/name/artifact SHA/MVID. `rea-struct-type.json` and rea-struct-pid.json independently retain GameAppli and its Int32 field Pid. Original instruction metadata explicitly encodes GameAppli as valuetype. The source definition is a struct containing Pid and other fields; this is not a reference-class array. REA is static primary evidence; no original code executes.

## Source derivation

Current loop reads `appList[i].Pid` directly. Roslyn can address the array element in place and emits ldelema. The proposed normal source creates a value copy before reading the field:

```csharp
GameAppli recoveredApp = appList[i];
if (recoveredApp.Pid == gameId_) { flag = true; }
```

The compiler can keep this one-use non-ref local on its evaluation stack, preserving ldelem.any without allocating a struct local in the emitted signature. Only this loop body is changed. GetInstance/GetAppList, flags, loop traversal, optional argument evaluation, later ratio selection/clamping and all existing member order remain untouched. GetAppList is still called once. The new local name is a readable reconstruction choice, not a recovered historical name. A collision is refused.

The pure API transform_local_copy(source,implicit=False) is header-independent and matches the full reviewed array declaration/for loop. implicit=True is a var spelling. The emitted source has no byte injection, helper method, unsafe code or handwritten generated class. Normal C# is the only mechanism.

## Own csc1.1 matrix, including negatives

Eight owned libraries were compiled: direct before, independent manual foreach Control, typed local, var local, direct foreach alternative, explicit identity cast, parentheses only and ref-addressed copy negative. No game class is compiled; Reader/GameAppli are new small owned prototypes with a struct containing Int32 Pid plus a reference field. All use the pinned csc1.1 hash and exact optimization/debug/language/reference profile from the final corpus. Only /out and /pdb output destinations are routed inside this WIP; the historical PDB path in the manifest is never written.

Typed local, var local and direct foreach over the single GetAppList call emit the same **entire**52IL owned method as the independently written Control, including headers, three local types, all instructions and body cells. Each differs from the direct-access before in exactly one resolved body byte32 (IL20),8F→A3. No additional GameAppli local appears. byte-proof.json and fixture-results.json retain offsets and full comparisons.

Measured negatives are preserved:

- Parentheses and an explicit identity cast do **not** force copying under this compiler: their whole bodies are identical to before, still ldelema. They are not target candidates.
- Passing the local copy by ref to an owned Observe method forces storage, adding a struct local/call and changing52→61IL. It still passes finite semantic samples, demonstrating why behavior is insufficient for the goal.
- Four pure-source guards reject a local-name collision, extra loop effect, different array index and different declared element type.

Owned runtime samples cover empty/one/multiple entries, targets0..4, duplicate Pid values, unchanged array elements and a null array with one collection call and the same exception. All eight variants pass these samples. Runtime only loads private fixtures. Those checks do not replace the byte comparator or claim the game's target body is matched.

## Pinned emitter explanation

Roslyn version1.1.0 source commit4cb203dab89e1542cdf7d88cb6d32767bc0e5460 is pinned by file SHA256 and verified Git blob in compiler-citations.json. EmitExpression877–891 states/emits that ldfld can read either a value or address. FieldLoadPrefersRef chooses non-ref locals/parameters differently from addressable array elements; the Local case prefers the value. Optimizer1931–1940 describes retaining a scheduled local assignment on stack rather than materializing normal storage. These source rules explain the observed owned full-body result. They do not identify which equivalent spelling the original author used.

## Genuine NoEmit limits and candidate policy

Model8 is unchanged. Baseline is already incomplete (`stack_scheduler_single_use:instance`) and predicts six types versus five emitted originals: it fails to eliminate the KairoBase receiver retained on the evaluation stack. Typed/var candidate remains genuinely incomplete and predicts seven types, additionally retaining the one-use struct copy. No flag/type list was edited to match expected output. The proposed target has not been emitted, so its actual candidate signature is UNKNOWN; the operator must use established incomplete-budget policy and require the original emitted five-type signature plus full oracle.

The direct foreach alternative is own-byte-equivalent but its genuine target prediction is **complete and wrong** (seven versus five). It is explicitly vetoed and excluded from the probe bundle, not artificially downgraded to incomplete. Identity cast is also excluded because its own byte experiment failed. The bundle selects typed-copy only; var is the same proven source mold, not a separate queued family.

## History and handoff

The precise stable ID occurs in52 R7–R11 Kairo engine receipts, with0actual compiled trials. No absence of trials is called a refutation. Three complete-method SHA gates prove the current source identical to R9D40, R10final and R11D6. This is bounded evidence, not a semantic exhaustive history claim. The primary source/model/fixture producer hashes, output manifests and checks are frozen in the index.

An initial static measurement attempt assumed continuous rather than hyphen-separated DTO hex and stopped before compiling anything; harness-note.json records the correction. It did not test/refute any source candidate.

`source-patches.json` supplies the one ordinary before/after method proposal with honest incomplete-model fields and UNKNOWN target result. Parent/operator alone may bind fresh HEAD, choose a cycle, compile a target, check its emitted signature, run the full verifier and adopt. This worker stops after GetRatio.
