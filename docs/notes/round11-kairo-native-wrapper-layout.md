# Private R11 managed callback layouts: 303 +297 original IL bytes

Only parent C# from KairoLibrary was inspected and edited in private proposals. No DEX/Java implementation, native library or JNI/runtime target was analyzed or executed. No target assembly, public tool/configuration, src or ledger was written; no commit occurred. Own fixtures use newly written managed mocks rather than Unity JNI or filesystem APIs. The generated classes in the toy binaries are produced exclusively by csc1.1 from normal delegates, never handwritten.

## Selection and source binding

Inputs are the official postD4 snapshot `build/round11/cycles/cycle04/after/KairoLibrary`. Parent declarations are saved in each `parent.before.cs`; after declarations, UTF16 spans and raw before/file hashes are in `source-patches.json` and analysis.json. The whole live KairoPlugin.cs was equal at initial read, not assumed immutable afterward. Parent methods remain the source identity; generated method stable IDs select only the verification target:

- KairoPlugin/<>c__DisplayClass90_0::<CreateLobiIntent>b__0, token06000c59, original303/current301,3locals: AndroidJavaClass, AndroidJavaClass, AndroidJavaObject.
- KairoPlugin/<>c__DisplayClass82_0::<SaveToTmpCaptureDirectory>b__0, token06000c4f, original297/current293,3locals: JCalendar,String,String.

The ordinary host methods are already body-equal (CreateLobiIntent49, SaveToTmpCaptureDirectory56); only their delegate source is changed. The owned counterparts' parent bodies also remain fully equal before/after (fixture-parent-body-proof.json). No closure field, lambda parameter or lambda count is intentionally changed.

## Lobi: stores per arm rather than a shared value join

Original49 brtrue78 skips the type0 construction. CHAT ends75 stloc2/76br145. Type1 constructs RANKING and stores2 at111; the fallback constructs ACTION_MAIN and stores2 before the same call sequence. Current is a nested ternary with one shared result store and defers CHAT until118. The outer UNITY_EDITOR/APPLI_TYPE guard and all Android wrapper calls already match; its early differing destination is induced by the later length change.

Pure API `transform_lobi(parent_source)` replaces exactly the reviewed object declaration/ternary with a bare AndroidJavaObject local and ordinary if(type==0), else if(type==1), else assignments. Every constructor argument/GetStatic call and subsequent operation is preserved. The fallback GetStatic<string>(ACTION_MAIN) is evaluated once only for other types. No managed/native boundary behavior is reimplemented.

Own BuildLobi generated lambda is301 before and303 in independently handwritten Control and calculated Candidate; the complete bodies (headers/locals/EH/resolved instructions) match. Same three local roles. All mocked constructor/GetStatic/Call effects and payload decisions are checked for types−1,0,1,2, both editor states and platforms0,1,2. These are finite owned checks, not a target runtime test or target signature proof.

## Save: editor work first, dead comparison and two returns

Original193 brfalse285 skips editor work. After Log/Pop282,283 br.s295 skips the else. Else285 loads APPLI_TYPE,290 loads2,291–292 pop both values,293ldnull/294ret. Footer295ldnull/296ret is distinct. Current inverted editor guard performs only APPLI_TYPE/pop then an early null return, dropping the comparison operand and final branch/footer layout.

`transform_save(parent_source)` restores positive if(UNITY_EDITOR){existing work}, else{if(APPLI_TYPE==2){}; return(object)null;}, then return(object)null. It preserves the timestamp/file-name prefix, actual work calls, captures and their order. The comparison operator is UNKNOWN historically: == is a representative. Independent own compilations using ==, != and > produce the same **entire** generated method. No source note claims equality was recoverable from the original pops.

Own SaveOwn lambda is292 before and296 after (+4), with3locals and the exact suffix field-load/ldc2/pop/pop/null-ret plus separate footer. Its unchanged owned prefix is1byte shorter than the game; no equality between the toy and actual target bodies is claimed. Control/Candidate whole bodies are equal. Runtime checks cover both editor states and platforms0,1,2, invoke each callback once, preserve JDate construction and confirm only editor work calls the managed mock File/Directory/Log. All mocks remain in the own fixture; no real file or native call executes.

## Emission proof, ambiguity and negatives

Pinned Roslyn version1.1.0 commit4cb203dab89e1542cdf7d88cb6d32767bc0e5460 provides the relevant source. EmitConditionalOperator emits its Alternative before Consequence, explaining the ternary ladder. BasicBlock.TryOptimizeBranchToEquivalent replaces a conditional branch to the next/equivalent block with Pop and emits another Pop for relational branches. ILBuilder enables that pass in Release. The two Core files were read from the official pinned raw GitHub URLs, retained privately, and verified against Git blob SHA1 from the pinned tree. compiler-citations.json contains paths, URLs, hashes and exact lines. Source references: [BasicBlock](https://raw.githubusercontent.com/dotnet/roslyn/4cb203dab89e1542cdf7d88cb6d32767bc0e5460/src/Compilers/Core/Portable/CodeGen/BasicBlock.cs), [ILBuilder](https://raw.githubusercontent.com/dotnet/roslyn/4cb203dab89e1542cdf7d88cb6d32767bc0e5460/src/Compilers/Core/Portable/CodeGen/ILBuilder.cs).

Seven own library compilations: Current, manual Control, Candidate, wrong Lobi action, elided Save comparison, comparison!=, comparison>. Two complete positive lambda-body comparisons pass. Wrong action changes body and fails semantic1. Eliding comparison changes body while finite semantic samples remain0; that is a structural negative, not a claim of an observed behavior difference. Four source guards reject selector change, injected side effect, different platform field and altered file payload. Existing guard fixtures are not silently reused as a new trial.

## Genuine NoEmit and oracle boundary

Model8 is unchanged. Parent predictions are incomplete with anonymous_function_or_capture; generated lambda predictions are incomplete with generated_or_unmapped_source_method and no predicted types. These real outputs are saved before/after. No hand-projected method or generated class is used to fake prediction. No Map.Metadata.RootSymbol or struct exception is bypassed; the actual limitation is missing general captured-lambda mapping. The three own emitted local types are fixture evidence, never an emitted game signature. Operator must use the normal incomplete budget and require the actual original three-type signature/full verifier on any target compilation. A fresh complete wrong signature remains a veto.

## Primary REA and historical limits

REA was rerun in this worker's session; another worker's retained evidence reference initially failed as unavailable, so no stale reference was used as a new oracle. Unqualified generated names are not unique across all KairoLibrary types. Fresh selected method pages are therefore bound to token06000c4f/name/297IL and06000c59/name/303IL, plus artifact SHA/MVID/raw CIL hash in `rea-original-lambda.json`, not solely to a short generated name. The extra CreateImage callback returned by the broad type-name selection is outside this study.

Each history.json scans42 R7–R10 Kairo engine receipts for the precise stable ID and records0actual trials. Absence of trials is not called a refutation. Six SHA gates show these exact parent methods unchanged from R9D40, R10C01 and R10 final to postD4; enclosing file hashes can differ because other methods changed. No previous alias hypothesis is being retested as though it had not been considered. The scan is bounded to those receipts.

## Cohort and handoff

Both are normal UI-thread delegate callbacks in KairoPlugin, preserve the same original/reference profiles and3local roles, and expose loss of ordinary statement layout in the recovered source. That is a useful analysis cohort. Their concrete IL causes differ, so the bundle gives **two independently selectable rule IDs**, not one alleged universal opcode-pair rule. They are separate from NamedTypes.FieldData, whose metadata/type issue does not occur here. The operator should select the current highest-weight real pair and may run these separately; this package does not force a shared cycle or conceal pending work.

Both APIs are pure, header-independent transformations of the reviewed normal parent source. `source-patches.json` includes before hashes and parent IDs. No future gain or unchanged live-source assumption is asserted. Fresh binding, target compilation, signature check, full oracle, public publication/adoption and cycle accounting belong exclusively to the operator. Worker stops after these two subfamilies.
