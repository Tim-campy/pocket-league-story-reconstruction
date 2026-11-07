# Comparison operand mirroring in both compiler paths

The retained Round10 dispatcher is read-only. The new Round11 dispatcher adds
one opt-in primitive comparison rule, used by either compiler path under r11-c5.
It swaps bare, distinct local/parameter operands and mirrors the operator:
`a < b` becomes `b > a`. An enclosing explicit negation is left in place;
`!(a < b)` therefore becomes `!((b > a))`, never `b >= a`.

Semantic symbols must prove equal primitive operand types. Fields, properties,
calls, mixed types, ref parameters, nullable/custom operators are rejected. The
operator is never complemented, preserving unordered/NaN behavior. The previous
C5 fixture evidence and compiler citations remain in the public smcs hypotheses
note; Map's actual remaining pattern is eight Int32 zero-subtractions and is a
separate proposal, not invented evidence of a mirrored comparison in that target.

The integrated AST rule produced seven positive patches and refused four symbol
negatives. Four new owned fixture library compilations compared the AST output
against independent manual controls: all 22 smcs bodies and 21 Roslyn bodies were
identical, including headers, locals and EH; owned runtime checks passed. No game
target was compiled in these fixture checks. See the integration fixture receipt.
Actual game gains and sufficiency remain UNKNOWN until the two C5 oracle passes.
