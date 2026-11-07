"""Private source-only hypotheses. No compiler/model/IO at import or API call."""
import re

IDENT = r"[A-Za-z_][A-Za-z_0-9]*"


def transform_boolean_initializer(expression, *, builtin_boolean_operands=False):
    """Only callers with metadata proof of built-in Boolean operands may opt in.

    Does not generalize to user-defined logical operators or float comparisons.
    Equivalent Boolean values do not constitute evidence of byte equality.
    """
    if not builtin_boolean_operands:
        raise ValueError("built-in Boolean operand proof is required")
    text = expression.strip()
    m = re.fullmatch(r"(" + IDENT + r")\s*&&\s*!\s*(" + IDENT + r")", text)
    if m:
        return "(!" + m[1] + ") ? false : !" + m[2]
    m = re.fullmatch(r"(" + IDENT + r")\s*&&\s*false", text)
    if m:
        return "(!" + m[1] + ") ? false : false"
    m = re.fullmatch(r"\(?\s*(" + IDENT + r")\s*\?\s*true\s*:\s*false\s*\)?", text)
    if m:
        return "(!" + m[1] + ") ? false : true"
    raise ValueError("unsupported initializer spelling; no speculative rewrite")


def transform_cfg(source, fields=("USE_UNUSE_CHECK", "DEBUG_CMD", "SHOW_DEBUG", "SHOW_FPS", "SHOW_MEM")):
    """Apply only explicit, unique static bool field initializers in received text.

    Header/class independent. Does not add a static constructor or declarations.
    Caller must prove DEBUG/PTW built-in bool and field initializer ownership.
    Returns transformed source and UTF-16 spans against exactly the input text.
    """
    edits = []
    for name in fields:
        pattern = r"\bstatic\s+bool\s+" + re.escape(name) + r"\s*=\s*(?P<expr>[^;{}\r\n]+);"
        matches = list(re.finditer(pattern, source))
        if len(matches) != 1:
            raise ValueError("expected one explicit static bool initializer: " + name)
        match = matches[0]
        start, end = match.span("expr")
        before = source[start:end]
        after = transform_boolean_initializer(before, builtin_boolean_operands=True)
        edits.append(dict(field=name, start=len(source[:start].encode("utf-16-le")) // 2,
                          end=len(source[:end].encode("utf-16-le")) // 2,
                          before=before, after=after, _start=start, _end=end))
    result = source
    for edit in sorted(edits, key=lambda e: e["_start"], reverse=True):
        result = result[:edit["_start"]] + edit["after"] + result[edit["_end"]:]
    return result, [{k: v for k, v in e.items() if not k.startswith("_")} for e in edits]
