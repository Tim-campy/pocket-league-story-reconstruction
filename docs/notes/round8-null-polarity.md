# Roslyn1.1: null-conditional expansion must also choose ternary polarity

CONFIRMED in independent compiler fixtures: `((object)array == null) ? -1 : array.GetLength(0)` emits the original call-first layout. The 14 emitted bytes through conv.i8 match the read-only StreamUtil.WriteArray expression prefix with resolved metadata tokens. No game candidate was compiled, no method was promoted, and no active helper was changed.

## Source-order hypothesis

**REFUTED as a universal rule:** IL follows the textual order of every C# construction. The pinned Roslyn code generator emits a conditional expression's Alternative before its Consequence. This restriction is specific and does not indicate a wrong toolchain. Ordinary statement sequencing and previously observed constructs retain their own evidence; do not generalize the textual-order claim across expressions or optimizations.

Pinned source: `analysis/tools/round6-roslyn-source/src/Compilers/CSharp/Portable/CodeGen/EmitExpression.cs`, Roslyn tag version-1.1.0, commit `4cb203dab89e1542cdf7d88cb6d32767bc0e5460`. [EmitConditionalOperator2646 onward](https://github.com/dotnet/roslyn/blob/4cb203dab89e1542cdf7d88cb6d32767bc0e5460/src/Compilers/CSharp/Portable/CodeGen/EmitExpression.cs#L2646) calls EmitCondBranch with sense:true, emits expr.Alternative, then branches to done, marks the consequence label, and emits expr.Consequence. This is why the null condition must be true for the fallback if the non-null member access is to appear first.

Exact local file SHA256: `4beb87ae114cd851204eed776ed12f9cc8d533b86299d8683bf455c212876f09`. Lines2646–2647 emit the conditional transfer and Alternative; lines2689–2690 mark the consequence label and emit Consequence; line2706 marks the common done label. These are source-file line numbers, not offsets into an assembly.

## Seven measured forms

The toy method is static with an unused first argument, so its array argument is ldarg.1 like the game method. Every source is independently written C# compiled by the pinned Microsoft.Net.Compilers1.1.0 / csc1.1.0.51109, with unchanged production options and Microsoft mscorlib2.0.

| Source form | First conditional | First expression block | Member opcode | Exact negative-reference body |
|---|---|---|---|---|
| array?.GetLength(0) ?? -1 | brtrue.s6 | fallback -1 | call | no |
| array!=null ? call : -1 | brtrue.s6 | fallback -1 | callvirt | no |
| array==null ? -1 : call | brfalse.s12 | call | callvirt | yes |
| !(array!=null) ? -1 : call | brfalse.s12 | call | callvirt | yes |
| !(array==null) ? call : -1 | brtrue.s6 | fallback -1 | callvirt | no |
| if(array!=null) result=call; else result=-1 | brfalse.s13 | call | callvirt | no: adds a named local/store/load |
| if(array==null) result=-1; else result=call | brtrue.s7 | fallback -1 | callvirt | no: adds a named local/store/load |

Object casts were used in the comparisons. The original call-first expression is IL0 ldarg.1;1 brfalse.s12;3 ldarg.1;4 ldc0;5 callvirt GetLength;10 br.s13;12 ldc.m1;13 conv.i8. The independently emitted negative ternary matches all14 bytes. The fixture ends with ret after this expression; the game continues with its local use. This proves an expression prefix, not the whole game body.

**REFUTED fixture expectation:** native ?. and the positive explicit ternary are whole-body-identical. They differ exactly at call versus callvirt for GetLength. The native null-conditional path already proves a non-null receiver to this compiler; the explicit expression retains callvirt. The initial assertion failed on that real difference, which is preserved in forms.json. It is not normalized away. The negative explicit form changes both the relevant orientation and the required member opcode.

## Three real examples and scope

Current group brfalse.s→brtrue.s has five methods / 4,079 original IL bytes. Only StreamUtil.WriteArray (1,686 IL) has a null-conditional/coalescing expression at the selected frontier. Pure IL hints accept exactly that one member-call/fallback pattern.

* StreamUtil.WriteArray: current source `long num = array?.GetLength(0) ?? (-1);`; corrected C5 positive explicit variant was actually compiled and kept default-first/brtrue. This is the new polarity composition target. Its whole body, later switches, EH and locals still require the real verifier.
* Canvas.OnInputUpdate (1,921 IL): original invalidTouch brfalse goes past an immediate ret; recovered source fuses invalidTouch, touchCount and frame tests into one OR guard. This is statement/guard partitioning, not a null-coalescing expression.
* SaveRecorder.Unlock(Boolean) (117 IL): original forced brfalse skips a positive tail block; recovered `if(!forced)return;` adds a return before that block. This is a positive-if versus early-return shape, not the ternary rule.

The other two members also have different causes: Offscreen.LateUpdate58IL combines image/dispose/layout conditions into one positive AND; KairoPlugin's generated SaveToTmpCaptureDirectory body297IL uses a non-editor early return versus an editor-positive body. No compiler-generated class is authored by this rule. The new null transformation is not applied to these four methods merely because their first opcode pair is the same.

## Proposed opt-in composition and safety

WIP `Round8NullConditionalPolarityRules.cs`, entry AddRound8NullConditionalPolarityRules with the usual partial-class API; family `nullable-coalesce-negative-ternary`, compiler roslyn. It requires `original_nullable_call_first` from `null_polarity_hints(original,rebuilt,original_offset,rebuilt_offset)`.

The hint requires original brfalse over exactly one non-null call followed by a join, a constant fallback at the branch target, and the same member in the rebuilt non-null block after brtrue. It matches only one of the five measured frontiers. It does not itself prove a complete method or authorize status changes.

The transform directly composes conditional-access/coalescing expansion with a negative reference-null condition and swapped ternary arms. Receiver is a local, a by-value parameter, or an unqualified/this/base nonvolatile field; side-effectful receiver calls and ref parameters are rejected. Member result must be a nonnullable value type, because a nullable/reference member may itself need the fallback even with a non-null receiver. Fallback must be a semantic constant. Object cast bypasses overloaded ==/!=. Property, method and index access are covered. No floating-point comparison is negated by this rule.

Twelve syntax controls pass: array method/property/index and learned-family positives; nullable member, reference member, unstable receiver, volatile field, nonconstant fallback, missing opt-in/evidence and smcs selection reject. Three emitted toy bodies match independently written manual reference methods in every body facet. Four executable runs each cover four scenarios, checking null/non-null, property/index side effects and zero calls to overloaded equality. The separate seven-form matrix has four runtime inputs and records the14-byte original expression prefix comparison.

Source-only game prediction remains incomplete before and after the one candidate; this is recorded without claiming a complete slot proof. The compiled candidate signature and full body are UNKNOWN because the author has not compiled the game. The normal incomplete-variant budget, compiled-signature gate, strict prefix invariant and full verifier remain mandatory.
