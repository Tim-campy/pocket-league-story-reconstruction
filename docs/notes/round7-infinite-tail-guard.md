# smcs do/while reconstructed from an infinite loop with explicit tail break

Status: WIP, no game compilation or adoption. The source shape is confirmed by six independent smcs toy-body comparisons; application to the game remains a verifier hypothesis.

## Three measured methods

Read-only baseline: `build/round7/cycles/cycle12/after/Assembly-CSharp/{original,rebuilt}-methods.json`.

| Method | Original IL bytes | Original tail | Rebuilt tail |
|---|---:|---|---|
| `form.DebugForm::Update_gameParam()->System.Boolean`, PLAYER_FACE decrement | 13913 | 11623 brfalse11633;11628 br11638;11633 br11544 | 11623 brfalse11544;11628 next statement |
| Same, PLAYER_FACE increment | same method | 11738 brfalse11748;11743 br11753;11748 br11659 | 11728 brfalse11649 |
| Same, PLAYER_BODY decrement | same method | 11846 brfalse11856;11851 br11861;11856 br11786 | 11826 brfalse11766 |
| Same, PLAYER_BODY increment | same method | 11942 brfalse11952;11947 br11957;11952 br11882 | 11912 brfalse11852 |
| `main.AppData::Draw_helpMessage(Graphics,String,Int32,Int32)->Void` | 384 | 295 blt305;300 br310;305 br186 | 295 blt186 |
| `data.PlayerData::AddExp(Int32,Boolean)->Void` | 217 | 201 blt211;206 br216;211 br16 | 201 blt16 |

Total: three methods, 14,514 original IL bytes, six demonstrated loop occurrences. Rebuilt offsets diverge cumulatively by ten bytes per missing explicit tail guard in Debug. AddExp also has earlier differences, so tail repair alone is not claimed to close it. Draw_helpMessage's first remaining sequence difference is this tail. Existing loop body breaks in both methods remain breaks of the converted loop.

Current recovered shape `do { body; } while(condition);` emits one conditional backward transfer. Proposed shape `while(true) { { body; } if(!condition) { break; } }` emits a conditional forward over `break`, the unconditional exit branch from break, and an unconditional backward loop transfer. This adds ten bytes with this historical compiler. It is not a conditional reversal or restoration of an empty if arm.

## Compiler evidence

Source inspected locally: `analysis/tools/round5-mono-source/pre-release/mcs/mcs/statement.cs`, the already recorded Unity-Technologies/mono pre-release snapshot `b6587926c0f146c8b12985744f33a13b39a22d57`. The exact build commit of the installed smcs remains UNKNOWN; independently compiled toy output confirms the relevant emission rules.

* [Do.DoEmit, lines 322–348](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/statement.cs#L322-L348): emits EmbeddedStatement, then `expr.EmitBranchable(ec,loop,true)` for a nonconstant condition.
* [While.DoEmit, lines 438–449](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/statement.cs#L438-L449): constant true marks LoopBegin, emits Statement, then `ig.Emit(OpCodes.Br,ec.LoopBegin)`.
* [If.DoEmit, lines 247–271](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/statement.cs#L247-L271): branches to false target, emits true arm, marks false target when no else exists.
* [Break.DoEmit, line1119](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/statement.cs#L1119): emits Br/Leave to LoopEnd.

`for(;;)` is deliberately not proposed: For.DoEmit has an initial unconditional transfer to its test label (line588), which is absent from these originals. `while(true)` supplies exactly the observed layout without that extra entry branch.

**REFUTADA — `for(;;)` as the exact source form for these tails:** compiler-source evidence shows its extra entry branch; this alternative was not compiled against the game. **REFUTADA — validity of the first bare-field Shadow fixture:** the installed smcs rejected that independently written toy source with four CS0135 errors, so it was corrected to explicitly qualified field references before measuring emission. This negative concerns that fixture's validity, not whether the game source had an unqualified field.

## Safety and dispatch

Partial entry: `AddRound7InfiniteTailGuardRules(...)`, normal transform API. Opt-in `smcs-do-infinite-tail-break`, via singular selection or learned list, smcs only. Requires `original_infinite_tail_guard=true` computed by `infinite_tail_guard_hints(original,rebuilt,original_offset,rebuilt_offset)`.

The hint checks an original conditional forward to a backward br, a long unconditional exit br in between targeting just after the back edge, and a nearby rebuilt conditional backward. It supports alignment at the inserted br as well as at the condition. It supplies observed original/rebuilt offsets, loop start, and exit. It does not itself certify equality or map a loop to source.

The old body remains a nested block. This preserves binding and local/capture scope instead of moving the condition into the body block. A continue whose nearest loop is this do is rejected: its target changes from the tail condition to the head in an infinite loop. A continue owned by a child for/while/do/foreach is retained. Existing breaks retain their loop/switch owners. Captures/lambdas, local functions, labels/goto, and yield inside the changed body are conservatively rejected; custom operators/conversions in the condition are rejected. Shared Negate retains explicit negation for floating-point comparisons.

## Fixtures and source-only model

Six manually written reference methods close every compared body facet: headers, locals, instructions, EH, extra sections, method attributes, implementation attributes. Positive fixtures: bool call with ordered trace, float including NaN and infinities, child-loop continue, try/finally, homonymous field/local with preserved nested scope, and body-owned break. Seven successful toy library compilations and seven executable compilations/runs; 21 runtime scenarios in each run. Thirteen syntax controls include six positives and seven rejections (owned continue, switch-nested owned continue, captures, labels/goto, custom operators, missing opt-in, missing IL evidence). There was one earlier invalid toy design compile with four CS0135 diagnostics: unqualified field `value` outside the child block conflicts with the child's local of the same name in this smcs; explicit `this.value` fixes the fixture. No semantic or code-emission mismatch was hidden.

Frozen source-only prediction: Debug before and each of its four individually transformed candidates have complete model coverage and the exact original signature. The AddExp and Draw_helpMessage candidate models are also complete and equal the original signature. Source and model hashes are recorded in `model-proof.json`. No target candidate was compiled by the bundle author. The active engine must repeat its frozen gate and preserve its strict raw prefix before the full verifier can adopt anything.
