"""Load published pure producers that originally used a sibling module name."""
import hashlib
import importlib.util
from pathlib import Path
import sys
from threading import RLock

ROOT = Path(__file__).resolve().parents[1]
LOCK = RLock()
ABSENT = object()


def module(path):
    path = Path(path).resolve()
    assert path.is_relative_to(ROOT / 'tools') and path.suffix == '.py'
    name = '_round10_source_' + hashlib.sha256(path.read_bytes()).hexdigest()
    if name in sys.modules:
        return sys.modules[name]
    specification = importlib.util.spec_from_file_location(name, path)
    result = importlib.util.module_from_spec(specification)
    sys.modules[name] = result
    try:
        specification.loader.exec_module(result)
    except BaseException:
        del sys.modules[name]
        raise
    return result


def load_with_parent(child_path, parent_path, legacy_name='proposal'):
    """Bind one import during module loading, restoring the previous namespace.

    Returned functions have already bound the parent's pure functions. Calls
    perform no I/O. The loader itself reads only explicitly selected tool files.
    """
    assert legacy_name.isidentifier()
    with LOCK:
        parent = module(parent_path)
        previous = sys.modules.get(legacy_name, ABSENT)
        sys.modules[legacy_name] = parent
        try:
            return module(child_path)
        finally:
            if previous is ABSENT:
                del sys.modules[legacy_name]
            else:
                sys.modules[legacy_name] = previous
