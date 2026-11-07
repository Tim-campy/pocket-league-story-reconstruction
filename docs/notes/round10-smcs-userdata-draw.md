# UserData.Draw_edit ? private calculated source composition

Status: READY_FOR_GAME_PROBE; actual game body/header/EH/prefix UNKNOWN. No game assembly was compiled here. Only the source-only frozen model and three builds of an independently authored fixture were run. No production source, method state, original or public file was changed by this study.

## Binding/API

Snapshot `build/round10/cycles/cycle15/after`, commit `d79ca4a6ebfb95b3e11ec79bebd8ae4e1f5e01c3`, ledger `63f0d241312150f41cae590aa3e7a7cfdb0c2203ed8a2b1d95fe0aaeee91e639`.
Method `game.UserData::Draw_edit(kairo.unity.ui.Graphics,System.Boolean)->System.Void`.
Raw UserData source SHA256 `68ee0849e33912bdfd84f978edf3ba5716deb70d3b4acdc4cb011c024851f421`.
Exact method substring SHA256 `0845f05323ca946d6d63a1487d41f905f1eac787b34c14727e6483dfc0dc214f`.

Pure header-independent API: `controls.py::transform_integral(method_source)`; imports only private `proposal.py`. Components `merge_x_and_zero_chain`, `condition_local_money`, `restore_string_assignments` are separately available. `source-patches.json` carries exact before/after with UTF16 positions and rebase_unique. Root must publish with parent module binding, fresh model gate and real target compilation. CalculatorSHA `50d274b9cde74376a66360d814fa27843c5b17c5578e5dc6d69925edbf0595b8`; bundleSHA `a2f1ac8931222d63ec6c6a5bf089bdbbbdc08d78d4ec1f583c69ca2a04e789a3`.

## Measured and calculated slots

Original:7,669 IL bytes, maxstack306,46 locals. Frozen rebuilt:7,602 IL, maxstack308,49 locals. Original32 Int32+4Boolean; rebuilt36 Int32+3Boolean. All other multiplicities identical (2TeamData,1String,2FormationData,1Vector,3Int32[],1PlayerData). No switch opcode or switch statement; no compiler temporary or capture occurs. Current NoEmit is complete49/49 exact. Resolved controls in the same snapshot independently agree: Team.SetSortJampBallNear9/9, UserData.Update_edit50/50, both whole bodies equal.

Original root zero chain is ldc0;dup;stloc4;dup;stloc3;dup;stloc2;stloc1. Source has only three variables/zeros. Roles are basex1,basey2,outx3,outy4, then viewport5. Rebuilt num2slot1?orig3;num3slot2?orig4;numslot3?orig2. Five declarations named num9 currently in distinct child blocks (slots9,11,14,21,26) all represent original basex1. Their IL lifetimes are301?457,817?1763,2313?3437,3574?3777,4258?4420: disjoint, no capture/address use, no outside escape. Original1 spans34?4434 with matching named coordinate-array stores. Declare `int num9,num,num2,num3;num9=num=num2=num3=0;`, remove only the five child declaration type tokens/bare declaration, retaining every initializer at its actual statement. Names are recovered/synthetic, not claimed historical. No declarations from loops or other types move.

This yields45 slots: one originalBoolean remains missing. Original44 at7051/7053 materializes `AppData.GetInstance().sysSave_.ints[14]==1` with ceq;store;load;brfalse. Current corresponding else-if branches directly with bne.un. Preserve the preceding Japanese/other-language cases, wrap only the remaining lexical else suffix and declare `bool recoveredMoneyStyle=...;if(recoveredMoneyStyle){...}`. This registers44 after the earlier inner Boolean42/Int3243. Final NoEmit46/46 exact; existing final Int32num23 becomes45. The intermediate45-slot merge stage is **not** offered as a standalone candidate.

## Diagnostic regions and control

Raw offsets are retained. Ephemeral local-identity correspondence is a diagnostic only, never a body equality test or serialized modified DTO. It reduces remaining sequence changes to6 and1retarget:

1?2 Original initial dup@32/store1@34 absent: fourth zero-chain variable.
3 Original store13@2976 absent: first string if arm.
4 Original bne.un2992?3009 vs current beq2972?2987 and defensive literal first: recover second test `et_cur[4]==1`.
5 Original store13@3002/join3004 plus defensive literal3009 missing/reordered: replace ternary assignment with three if/else assignments. Original string.Empty initialization@2951/store13@2956 stays.
6 Original ceq7049/store44@7051/load44@7053/brfalse7055 vs direct bne.un: named Boolean materialization above.
The single retarget points to old shared stack-value ternary store; separate per-arm assignments restore the join. This is a new joint calculation, not generic declaration permutation or name-based allocation.

All changed comparisons are Int32 equality/Boolean. et_cur and sysSave.ints are proven Int32[] in `primitive-type-proof.json`. All math, Single conversions, shifts, loops, owner targets, calls and string payloads remain. Literal order intentionally becomes balanced/attacking/defensive to follow the original IL; multiset is exact. No original bytes/IL are copied into output.

Byte-count crosscheck only: mapping existing local accesses changes widths by+55 (outy slot2?4 contributes+86; five basex locals?1 contribute?31), newdup/store+2, two extra per-armstores+4, ceq/store/load+6. Total+67 equals7,669?7,602. Does not establish full body/header/maxstack equality.

## Actual validations and history

`controls-noemit-proof.json`: source model complete at all3stages; merge-only45 not exact, subsequent moneyBoolean and string assignments46/46 exact. `layout-noemit-proof.json` independently proves combined layout46/46. Root/client hashes match existing frozen validation, no new model rule.

`toy-validation/report.json`:3own smcs2.6.5 compilations; own candidate fullbody/header/locals/EH matches separately written manual source; all11methods equal,8negative forms rejected (nonzero chain,type/capture/shape,financialcomparison/namecollision,ternarycase/nonliteral). Baseline differs. Own fixture moves14?11locals,495?499IL, maxstack31?29. The?2maxstack matches the target gap diagnostically; target maxstack still UNKNOWN.

`history-by-method-sha.json`:58 engine reports authenticated against final source rawSHA then exact method substringSHA. Every final method equals frozen15. Round4 recorded13trials,0accepted; remaining audited reports record0targettrials. Old generic order/scope negatives remain valid; this exact joint source was not tested there.

Mono pin `b6587926c0f146c8b12985744f33a13b39a22d57`, local `analysis/tools/round5-mono-source/mcs/mcs`: Block registers locals in statement.cs2032?2046; LocalInfo.Declare allocates1225?1238. If.Emit247?267 emits per-arm statements and long joins. Conditional expression expression.cs4196?4215 emits a stack value joined before the eventual assignment, explaining missing per-arm stores. No variable-name allocator hypothesis is used.

No further inferred region remains unexplained by this composition. Only the actual game compiler/verifier can confirm whether it closes the complete7,669-byte body and affects any other method. Do not hand-edit state if rejected.
