# R11 private Dictionary cache mold, two methods /251 original IL bytes

Only private source calculations, owned csc1.1 fixtures and NoEmit analysis were performed. No game target build/run, public source/tool/configuration/ledger change, original mutation or commit occurred. This finite study stops after these two methods; production remains exclusively with the operator.

## Inputs, identity and history

Both methods are bound to the immutable official postD6 snapshot `build/round11/cycles/cycle06/after/KairoLibrary`:

- kairo.unity.graphics.MaterialManager.GetMaterial(kairo.unity.graphics.Material,kairo.unity.ui.Image), token060008de,110 original/112 rebuilt IL.
- kairo.unity.graphics.MeshManager.GetMeshBuilder(kairo.unity.graphics.Material), token06000903,141 original/144 rebuilt IL.

Material here is the managed kairo.unity.graphics.Material, not UnityEngine.Material. REA was queried on the exact declaring types, and single method pages were selected and checked by build-local token. `rea-method.json` and summary files retain artifact SHA/MVID, signatures/raw CIL identity and completeness. Material parent ev_dc851674e9f3c4c05375e789d868ad8a02fb0e9d4e3452b170ab814cee05f716/page ev_5fbcb89f48c9f2b92b68ca7db739c736d52259b8770b841409aa2aebd38767e0; Mesh parent ev_18156d1d4fae1f30e094f31f058ff26b19c9cfc74409d0cf6a0aece3a2460b56/page ev_bf3f4c9889d1115fb50c24cf4b4cf9a7130f9c704f0496aa72b19ef767cc02e7. These are static original-file evidence, not a substitute for the verifier.

Each precise stable ID has42 R7–R10 engine receipts with0actual compiled trials. No-trial is not refutation. Six gates prove the whole methods byte-identical at R9D40, R10final, R11D4 to postD6. Other GetMaterial overloads used the related indexer primitive in earlier rounds; this is not a novel universal primitive, nor a claim that those different overloads were never tested. These exact two IDs/source compositions had no actual trial in the bounded receipt scan.

## One shared source/IL cause

Both first aligned hunks are extra stloc/callvirt/ldloc around the result of a Dictionary.set_Item. Current:

```csharp
value = (cache[key] = new Value(...));
```

Original requires the out-local assignment inside the indexer assignment:

```csharp
cache[key] = (value = new Value(...));
```

Receiver/key/RHS evaluation order is retained. The former needs to save/reload the value of an indexer assignment while its setter consumes the receiver/index/value stack. The latter duplicates/stores the named out local before invoking the setter, eliminating the extra assignment-result temporary. Neither receiver, key, constructor nor getter is duplicated. Actual field/indexer/type bindings are required; this narrow lexical recipe is not a blanket rewrite for custom setters, captured/ref locals or arbitrary names.

Material changes **only** `value=(materlas_[key]=new Dictionary<string,Material>())`. Its original locals are Image0,Dictionary1,Material2; current inserts a Dictionary temp2 and moves Material to3. The second expression `Material material2=(value[material.key_]=new Material(...))` already produces the correct original instruction pattern apart from shifted slot. It is deliberately preserved. Original constructor89dup/90stloc2/call setter then96ldloc2/dup/RenderQueueOffset/ret already establishes that separate role.

Mesh changes only the analogous meshList statement. Original locals are MeshBuilder0,Int32 renderQueue1,Int32 loop2,KVP struct3. Current inserts MeshBuilder temp2, shifts loop to3 and KVP to4. The conditional/list insertion and break are unchanged. Both are the same assignment-value/Dictionary cache mold and share a legitimate rule ID; no unrelated feature is forced into the cohort.

## Why Mesh requires a new model capability

Public model8 before is **complete but wrong** for Mesh:4predicted versus5 actual types, missing the KVP. Public after predicts3 versus4original, still complete. This remains a veto; it was not downgraded artificially or ignored.

`sortedMeshList_[i]` returns KeyValuePair by value. Calling its Key getter requires an address, so Roslyn clones that rvalue into a temporary (original83stloc3/84ldloca3). `EmitAddress.cs` defaults an expression without a storage home to EmitAddressOfTempClone: emit value, AllocateTemp, store, load its address. It is not a reference-class result or an already addressable array/local.

`Round11StructIndexerModel.cs` is a **private derivative** of unchanged model8, with an exact diff and private client. New logic applies only when a resolved instance getter's receiver is an indexer returning a resolved struct **by value**, not by-ref/ref-readonly. It emits receiver/index argument model events, allocates/releases one receiver temp through the existing type pool, and reports that lowering. It does not add speculative support for readonly fields, arrays, local/parameter addresses, method-call receivers or generic type parameters. No game IDs, variable names or expected type lists drive the prediction.

Private prediction correctly covers Mesh before5 and candidate4, matching entire canonical sequences. Five independently owned controls confirm: List<struct> getter needs one Pair temp; array element and struct parameter need none; reference-class indexer needs none; two List<struct> getter uses reuse one temp. Seven complete sequences (Mesh before/candidate plus five controls) equal actual owned compiler output. The operator must still publish a new namespace/client and validate **every matched method** on the fresh corpus before using it in production. Worker has not performed that corpus validation.

Material remains genuinely incomplete in public and private models, for `stack_scheduler_multiple_use:material2`. Public before predicts5 versus actual4; after predicts4 versus original3. The private KVP rule correctly makes no change here. This is an existing separate stack-scheduler gap, not a complete wrong-signature escape. The operator must use the established incomplete budget and check the actual emitted original3-type signature if probing. No flag or type sequence was edited to claim success.

## Own fixtures and failures

Five own libraries were compiled with pinned csc1.1/final snapshot flags and reference hashes: Current, independent manual Control, Candidate, duplicated-MeshBuilder negative and independent model controls. Both positive method bodies match their Controls entirely, including headers/locals/EH/resolved instructions. Own Material113→111 (4→3locals), Mesh144→141 (5→4locals). The owned Material context differs1byte from the game's112→110; only the -2delta/mold and own equality are established. Target whole-body matches remain UNKNOWN.

Finite runtime checks execute only new managed stubs and real BCL Dictionary/List. They cover Material cache miss/hit/null-image fallback, preserved queue offset, Mesh cache hit, three misses and sorted insertion ordering. The duplicate-constructor negative changes the body and fails semantic2. Five source negatives reject changed mesh index, constructor argument, Dictionary type, duplicate statement and material key. No game/Unity/native code executes.

Two harness issues are recorded rather than hidden: the first negative generator expected different whitespace and stopped before compiling anything; after correction, five fixtures compiled. An initial local-type normalizer then truncated generic arguments and falsely rejected the KVP prediction. It was replaced by the existing canonical Cecil --locals-only provider; the same binaries were reused, not recompiled. Those were harness failures, not failed source hypotheses or target trials. `harness-corrections.json` records them.

## Source citations and handoff

Pinned Roslyn1.1 commit4cb203dab89e1542cdf7d88cb6d32767bc0e5460: EmitExpression1756–1773 and2122–2163 describe/emit duplication and assignment-result temporary when the LHS uses stack;2245–2253 reload/free. EmitAddress88–99/166–180 and458–466 route the value-type receiver to a temporary clone. Complete excerpts/file SHA/Git blob identity (verified against the pinned tree) are in compiler-citations.json. No compiler/options/reference change was proposed.

`calculator.py` pure APIs: transform_material_cache(str), transform_mesh_cache(str), sharing local_inside_cache. Normal method before/after, UTF16 spans and file/method SHAs are in source-patches.json. The bundle honestly separates incomplete Material from Mesh's new-model requirement. Both files were live-equal at initial read, but fresh source binding/model policy is required. Only the operator may publish, compile targets, require exact signatures, run the full verifier and adopt/commit. Neither fixture equality nor this private model is an adopted game result.
