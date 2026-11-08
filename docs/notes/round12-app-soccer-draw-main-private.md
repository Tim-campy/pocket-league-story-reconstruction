# Private SoccerForm.Draw_main terminal shared return

Method `form.SoccerForm::Draw_main(kairo.unity.ui.Graphics)->System.Void`, original token060001d0,1308 IL. Snapshot: `build/round12/runs/b05/commit/postverify01/Assembly-CSharp`. Only this reserved method was examined; GamePad and other families were not opened. Zero game target builds, target verifier runs or public/source/ledger/config changes. Whole game candidate result **UNKNOWN**.

## Measured cause

Original and reconstructed method have the same9 local types, fat12-byte header, flags19/initlocals=true, maxstack79, and no EH. Original code size1308; current1309. Every instruction before the terminal region is equal except the last conditional target. The alignment has one inserted ret and one retarget, no other sequence edit or branch-width difference:

| Original | Current |
|---|---|
|1286 load Config.DEBUG_CMD|same|
|1291 brfalse1307|same|
|1296 load SHOW_DRAWNUM;1301 ldc1|same|
|1302 bne.un1307|1302 bne.un1308|
|1307 ret|1307 ret;1308 ret|

The current source is terminal `if ((!Config.DEBUG_CMD) || SHOW_DRAWNUM == 1) { return; }` (with recovered parentheses). The original emission is obtained by a terminal positive **empty** test:

```csharp
if (Config.DEBUG_CMD && SHOW_DRAWNUM == 1) { }
```

Both false edges then target the implicit shared method return. No drawing/debug payload is invented. This is an Int32 equality/Boolean field test; the `.un` in `bne.un` is not evidence of a floating-point comparison here. No float expression, arithmetic or call is modified.

REA independently decodes all483 instructions for token060001d0, original artifact SHA3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949, MVID48ce9f89-73d2-40be-bb24-22c807c04106. Raw CIL SHA133d4cc03b2469b0c3101fc34170a226223f50e1a3d5084bc9bae08ee22e68ce equals the frozen original DTO CIL. Original CIL/metadata is completely decoded; wider type string-inventory limitations remain recorded in the saved evidence and are not treated as a source correction.

## History and actual validation

`history.json` covers78 retained reports and7 actual trials. The old R7 empty-arm trial was `if (DEBUG_CMD && SHOW_DRAWNUM != 1) {} else { return; }`, not this empty positive equality with no else. The old tail-early-return and split-OR trials also retained explicit returns. Old solver trials changed declarations. Their measured prefix regressions remain recorded (1303→1292/1291); they are not erased or relabeled as untested. The proposed full after hash is absent from the retained historical after hashes. This is a bounded history check, not global proof of never having tried an equivalent C# program.

The unchanged real R11 NoEmit model reports complete,9 types in original order, with all9 name/type/slot roles identical before and after. Exact full-file SHA, before/after method SHA and UTF16 spans are included. Pure `transform_draw_main(text)` requires the exact unique terminal guard in the method-root block, rejects else/suffix/capture/changed-condition cases, works after header renaming, and performs no Path I/O during calls. The compiler-source citation file pins Unity Mono b6587926c0f146c8b12985744f33a13b39a22d57 (`statement.cs233–269`, return754–804, logical branch emission `expression.cs3418–3483`). The exact manufacturing source revision of installed smcs remains UNKNOWN; actual fixtures establish installed behavior.

Five own mock libraries were compiled with installed smcs2.6.5 SHA26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0 and the copied real options/references. The independent manual control uses **nested ifs**, not the producer's && syntax, proving the source spellings are emission-equivalent while retaining both original false edges:

| Own source | IL/locals | Full body vs independent control |
|---|---|---|
|Current explicit return guard|75/2|different|
|Nested empty-if control|74/2|equal|
|Calculated positive empty &&|74/2|equal|
|Wrong empty comparison !=1|74/2|different|
|Positive guard with retained return|75/2|different|

The full ordinary DTO comparison includes bytes/instruction operands/header/maxstack/locals/EH. Candidate conditional targets are explicitly checked to be the single final ret. Three own NoEmit predictions are complete and match the actually emitted own local signatures. All five own libraries pass20 semantic combinations of debug value, mode and drawing prefix, showing why behavioral equivalence alone cannot choose among these terminal forms. The two emitted negatives are byte mismatches despite equal semantics. Four producer negatives (changed comparison, added else, nonterminal suffix, lambda capture) are rejected. No compilation failed.

The candidate is supported for an isolated game probe only. Historical source spelling is not uniquely recoverable: nested empty ifs emit the same owned bytes. Game candidate size/header/locals/EH and original-method equality must still be measured by the exclusive production operator, with a fresh source binding and the unchanged full verifier. No matched count or progress gain is claimed here.
