# ExecuteScript, seven arguments: private source composition

Exact target: `main.AppData::ExecuteScript(Int32[][],Boolean,String,Int32[],Int32,Int32,Int32)->form.SubForm`, token060004c7,190 IL. The16/13/3210IL overloads are excluded. Baseline is immutable `build/round12/runs/b19/commit/postverify01/Assembly-CSharp`; source SHA and UTF16 full-method binding are in analysis/source-patches. At packaging, Main full-file SHA still equals the snapshot. Production must revalidate this binding after changes elsewhere.

REA primary page confirms token, seven signature parameters,190 IL,95/95 decoded instructions, zero truncation/body issue, original SHA3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949 and MVID48ce9f89-73d2-40be-bb24-22c807c04106. `rea-proof.json` compares raw DTO CIL hash with REA aa10cc45d63401b2d53c3e42d1d15da9a6514b94230c9e1b8174d64a7c516b75. Original/current header: fat12,flags19,initlocals,maxstack13,190IL; six same local types, no EH. Equal size/slots do not imply matching bodies.

## Actual source construction

Original dispatcher is `36 beq46; 41 br115`, followed by delay payload46..114 then default payload115. Current `if(num2==5)` uses bne.un36→110 and has no default-dispatch branch. Original default tests talkCharaIndex at131, nonnull subForm137 and unset humanCharaIndex149, all converging170. Then original170br175 targets loop increment; current source has two explicit continue guards with transfers to175 at137/154. Alignment identifies four sequence edits and one deferred target, not a changed comparison value.

`calculate.py::transform_execute(method_text)` removes only `int num2=num`, emits ordinary `switch(num){case5:existingDelayPayload; default:existingExecuteCall; if(three positive conditions){existing two stores;} break;}`, and preserves the root prefix, case payload text, calls, strings, constants, loop and return. The unchanged public R11 NoEmit model is genuinely complete with six original types: named subForm0,delayEvent1,i2,array3,num4; switch emission temporary Int32 occupies5, replacing named num2. Original local-name recovery remains UNKNOWN.

The switch/fused-positive-default composition is needed to offset length changes: the dispatch adds5 bytes relative to direct if; replacing two continues by positive default join reduces5, leaving190 total. No comparison normalization, casts, handwritten generated class or byte copying occurs.

## Owned fixtures and limitations

Final matrix: seven own library compilations with installed pinned Unity smcs2.6.5 flags/references, private outputs, no game source compiled. Current/Control/candidate share mock fields and method declarations; the manual control independently uses three nested positive ifs rather than the producer's && chain. Mock AddElement uses its observed actual Void signature. Full body comparison includes resolved tokens, fat header/maxstack, locals and EH.

| Own variant | IL | Locals | Equal to independent control | Failed mock cases /108 |
|---|---:|---:|---|---:|
| Before |190|6|No|0|
| Manual control |190|6|Yes|0|
| Candidate |190|6|Yes|0|
| Switch alone |200|6|No|0|
| Positive default guard alone |180|6|No|0|
| Default continue instead of break |190|6|Yes|0|
| Default return instead of break |183|6|No|18|

Candidate specifically reproduces original offsets36beq46,41br115,170br175. The mock108-case product covers null/empty/mixed scripts, delay position/hour override, line index, talk index, popup, call counts and payload identity. It never executes game/original code. Six source-producer mutation negatives reject changed code/alias, side effect, store value, capture and footer. Three final own NoEmit controls match emitted types; target before/after NoEmit both complete six exact. No flags were forced.

**Emission-equivalence:** at this exact default section/for-tail location, ordinary break and continue produce identical full bodies. The selected break is valid normal C#, but the original spelling is UNKNOWN. The constructor/type support code in fixtures is a new minimal mock, not copied implementation of game classes.

Preparation defects are recorded rather than hidden. Total22 own library compilations across four attempts: initial harness expected the wrong static entrypoint; a subsequent negative-case assertion dereferenced intentionally missing delay; third matrix showed191 instead of190 because the mock incorrectly returned Object from AddElement and therefore emitted pop. Correcting mock to the measured Void signature yields the final seven-row matrix above. There were zero compiler failures and zero target attempts throughout. These are implementation/harness defects, not rejected game candidates. `validation-attempts.json` preserves counts and reasons.

## Compiler source and history

Pin b6587926c0f146c8b12985744f33a13b39a22d57, Unity Mono statement.cs3454–3480 emits Beq for small nonzero integral cases;3547–3580 emits default jump and sections in source order;3821–3875 allocates selector LocalTemporary and scopes switch LoopEnd;233–269 emits positive-if joins;1115–1150 distinguishes break/continue owners. Hashed excerpts/links in compiler-source-evidence. Exact manufacturing revision of installed smcs remains UNKNOWN; own fixtures test the installed executable directly.

History bound:93 reports/7 actual trials/0 adopted steps. Mirror equality remained36→36; old empty-arm restoration and value-return guard reversal regressed36→16; declaration grouping stayed36. Retained before/after hashes are recorded, and this whole-method after hash is absent. The older source shapes did not restore cached switch plus positive default-section join together. This is a bounded history statement, not proof of global absence of semantically equivalent old attempts.

Status: PRIVATE READY FOR ISOLATED GAME PROBE ONLY. Game body equality and full assembly regression/metadata outcome remain UNKNOWN. Target builds0, target verifier runs0, public/src/config/ledger writes0. Prior frozen packages unchanged. A production owner may select and test this proposal under the current protocol; this directory does not alter state.
