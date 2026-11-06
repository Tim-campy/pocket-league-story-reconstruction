"""Pure, conservative evidence for an original scalar String.Concat4 call.

Only the first System.String.Concat call in the next straight-line context may
supply a hint. Unknown, mixed, array, instance and generic overloads supply none.
This is source-location evidence; it never changes the body oracle or state.
"""
import re

_STRING_TYPE = re.compile(
    r'class System\.String @ [^,<>()\[\]&*;]+'
    r'(?:,\s*(?:Version|Culture|PublicKeyToken)=[^,<>()\[\]&*;]+)*\Z')
_PARAMETER_START = re.compile(r',(?=(?:class|valuetype)\s)')
_STOP = ('br', 'beq', 'bne', 'bge', 'bgt', 'ble', 'blt', 'leave')


def _scalar_string4(signature):
    marker = '::Concat``0('
    if not isinstance(signature, str) or marker not in signature:
        return False
    declaring, remainder = signature.split(marker, 1)
    if not _STRING_TYPE.fullmatch(declaring):
        return False
    if ')->' not in remainder:
        return False
    parameters, tail = remainder.split(')->', 1)
    result, separator, options = tail.partition(';')
    if not separator or not _STRING_TYPE.fullmatch(result):
        return False
    if 'this=False' not in options.split(';'):
        return False
    parts = _PARAMETER_START.split(parameters)
    return len(parts) == 4 and all(_STRING_TYPE.fullmatch(part) for part in parts)


def original_concat_hints(original, offset):
    """Prove the next original String.Concat has exactly four String arguments.

    Searches at most 64 instructions without crossing a branch/exit. It does
    not skip a closer Concat overload, infer arity from the C# or parse assembly
    identity commas as parameter separators. Empty evidence is returned as {}.
    """
    if not isinstance(original, dict) or not isinstance(offset, int) or isinstance(offset, bool) or offset < 0:
        return {}
    instructions = original.get('instructions')
    if not isinstance(instructions, list):
        return {}
    context = [ins for ins in instructions if isinstance(ins, dict)
               and isinstance(ins.get('offset'), int) and ins['offset'] >= offset]
    for ins in context[:64]:
        opcode = ins.get('opcode', '')
        if not isinstance(opcode, str):
            return {}
        if opcode.startswith(_STOP) or opcode in ('ret', 'throw', 'rethrow', 'switch', 'jmp', 'endfinally', 'endfilter'):
            return {}
        if opcode not in ('call', 'callvirt'):
            continue
        operand = ins.get('operand')
        signature = operand.get('method') if isinstance(operand, dict) else None
        if not isinstance(signature, str):
            return {}
        if not signature.startswith('class System.String @ ') or '::Concat' not in signature:
            continue
        # A nearer Concat3, Concat(Object,...) or Concat(String[]) makes the
        # desired four-string hint unavailable; never search past that call.
        if opcode != 'call' or not _scalar_string4(signature):
            return {}
        return {'original_string_concat_arity': 4,
                'original_string_concat_parameter_types': ['System.String'] * 4,
                'original_string_concat_call_offset': ins['offset'],
                'original_string_concat_call_signature': signature}
    return {}
