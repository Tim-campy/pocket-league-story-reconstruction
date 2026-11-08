# Private CgiGetFriendList: ordinary false Boolean preserving emitted tail

Exact ledger/DTO identity: `system.friend.MyFriend::CgiGetFriendList()->java.util.Vector`, non-generic/no parameters, token0600073c,286 original IL. Input source/DTO/manifest: immutable `build/round12/runs/b13/commit/postverify01/Assembly-CSharp`. No game target, verifier, production/source/public/config/ledger change or additional family. All previous private studies remain untouched. **Game candidate whole-body result UNKNOWN.**

## Original initialization and current folding

Original entry is explicit and complete:

```text
0 ldc.i4.0
1 stloc.0      // signature says System.Boolean
2 ldloc.0
3 brtrue15
8 ldarg.0
9 call base.CgiGetFriendList
14 ret
15 newobj Vector
...
285 ret
```

Slot0 has only the store/load at1/2. Its value is false, so normal execution takes the base call and return; the custom tail remains in CIL despite being unreachable at runtime with this initializer. There is no dup/assignment chain, no EH and no protected return temporary. All nine original locals are Boolean,Vector,Vector,Byte[],String[],Int32,Friend,Int32,Int32.

The current recovered source instead begins `if(0==0){return base.CgiGetFriendList();}`. Under the pinned smcs, that compile-time condition folds true and the remaining statements are removed. Current CIL is only7 bytes/maxstack3, while metadata still reserves the eight tail locals. Original is286 bytes/maxstack33 and has the extra leading Boolean. Both headers are fat12-byte, flags19/initlocals=true, with no EH. The current body cannot be treated as an almost-matching initialization fix: its full tail was omitted by compilation.

The supported normal C# form is:

```csharp
bool useCustomFriendList = false;
if (!useCustomFriendList) { return base.CgiGetFriendList(); }
// Entire existing custom tail preserved byte-for-byte source.
```

The local is ordinary, not const; its chosen readable name is not asserted to be historical. Other source spellings can emit the same condition (the manual control uses `flag==false`). No configurable or externally mutable flag is introduced. All original runtime behavior still goes through the same base call; no new reachable custom request path is enabled.

## Real model and exact source proposal

The unchanged public R11 NoEmit model predicts **complete9/original-exact**. The existing eight variable name/type roles shift by exactly+1 slot, and the new Boolean occupies0. No incomplete state is forced or model modified. The entire tail after the base guard is byte-identical source before/after; every call/string/char literal is unchanged. Source/member references, casts, three while loops, state tests, increments and list operations are not rewritten or guessed.

Pure API: `calculate.py::transform_friend_list(method_text)`. It requires the exact constant guard as the first method-root statement, rejects a local-name collision/capture/changed base call and verifies the reviewed vector declarations in the tail. Header-renaming and calls with Path I/O forbidden reproduce the payload. Exact parent C# before/after, UTF16 spans, live-before uniqueness, source/method/producer/model/client SHA and engine-compatible proposal are retained. A production probe still requires fresh source binding, model and full verifier.

## Own fixtures and negative outcomes

Six newly authored mock libraries reproduce the full guard/tail shape with vectors, parsing/state grouping and three loops. The apparent Connect call is a private in-memory stub returning canned bytes; **no network implementation or real game class is invoked**. Positive runtime checks require exactly one base call, zero mock Connect calls, zero local vector allocations and the identical base return object. The deliberately wrong guard can only execute this in-memory stub.

Compiler: installed Unity smcs2.6.5 SHA26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0, same copied flags/references with private output paths. Commands/warnings/DTOs are saved.

| Own variant | IL /locals /maxstack | Complete body vs independent control |
|---|---:|---|
|Current constant comparison|7 /8 /3|different|
|Manual ordinary false local, `local==false`|286 /9 /33|equal|
|Calculated ordinary false local, `!local`|286 /9 /33|equal|
|Ordinary true local with positive guard|286 /9 /33|different|
|Const false local with negative guard|7 /9 /3|different|
|Ordinary false local with wrong positive guard|286 /9 /33|different|

Comparison includes the full ordinary method DTO: resolved body bytes/instructions/operand identities/header/maxstack/local types/EH. Candidate and independent control match in the entire owned method, not only the first branch or size. The candidate entry is also explicitly checked for ldc0/stloc0/ldloc0/brtrue15. Three owned NoEmit predictions (before/control/candidate) are complete and match actual emitted signatures. No negative is falsely reported as a target model veto.

The true-condition local preserves the same base-only runtime behavior and all9 types but has the wrong initializer/branch polarity. The const local also preserves runtime behavior, but compilation removes the branch/tail: this smcs still reserves9 metadata locals even though only7 IL remains. The inverted false guard returns semantic failure1 and exercises only the canned mock. Six producer negatives are rejected: changed constant test, changed base call, chosen-name collision, guard not first, missing reviewed tail vector initialization and capture. All fixture compilations succeeded; unreachable-code warnings for folded forms are retained rather than suppressed.

## Compiler source and primary original evidence

`compiler-citations.json` quotes exact local SHA/lines from official Unity Mono pin b6587926c0f146c8b12985744f33a13b39a22d57:

- statement.cs165–211 and233–269: If.Resolve eliminates dead constant branches; If.DoEmit recognizes Constant and emits only the taken statement. An ordinary VariableReference condition follows the branchable path.
- statement.cs2138–2177: unreachable statements are resolved for diagnostics and replaced with EmptyStatement, explaining tail removal.
- statement.cs2029–2052 and LocalInfo.ResolveVariable1225–1240: metadata declares variable entries independently of surviving statement emission; a retained local signature does not prove retained body instructions.
- expression.cs553–559: logical-not reverses the requested branch sense, yielding brtrue for the false local's negative guard.

Exact manufacturing source revision of the installed executable remains UNKNOWN; the owned output matrix corroborates actual behavior. The const-negative local count is measured, not inferred from another compiler's behavior.

REA authenticates token0600073c/no-parameter signature and all103 original instructions against original SHA3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949/MVID48ce9f89-73d2-40be-bb24-22c807c04106. Raw CIL hash equals the frozen original DTO. Selected body/metadata decode is complete; wider selected-type string limitations stay recorded and are not changed.

`history.json` contains88 retained reports and0 actual compiled trials/steps for this exact ID. No new after hash matches a retained after; this bounded search does not claim every model-rejected/unretained variant was absent. There is no blind candidate search or replay of a known failed target candidate.

The owned286/9/max33 match is evidence for the source template, not an emitted game result. The actual286-byte game tail must still pass the exclusive production operator's normal oracle before any state/count/byte-progress gain is reported.
