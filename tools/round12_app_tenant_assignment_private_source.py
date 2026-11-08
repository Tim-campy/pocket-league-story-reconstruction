"""Pure source-only producer for one measured sequential assignment."""
METHOD_ID = 'game.UserData::SetTenantIn(game.TPlayer)->System.Boolean'
RULE = 'r12-tenant-accumulator-then-array-store'
BEFORE = 'num2 = (array[l] = num2 + apdat_.tenantData_[tenant3.tenantDataId_].charm_);'
AFTER = 'num2 = num2 + apdat_.tenantData_[tenant3.tenantDataId_].charm_;\r\narray[l] = num2;'

def transform_tenant(source):
    if source.count(BEFORE) != 1:
        raise ValueError('unique fixed accumulator/array assignment required')
    return source.replace(BEFORE, AFTER)

def closing(source, start):
    """Brace matcher sufficient for this method, which has no literals/comments."""
    if any(x in source[start:] for x in ['"', "'", '/*', '//']):
        raise ValueError('unexpected literal/comment; unsupported extraction')
    level = 0
    for i in range(start, len(source)):
        if source[i] == '{': level += 1
        elif source[i] == '}':
            level -= 1
            if level == 0: return i
    raise ValueError('unbalanced method')
