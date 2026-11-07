"""Bind a static field initializer to its .cctor using AST and CIL anchors.

This is mapping evidence, never permission to change method states or PE bytes.
No explicit constructor is introduced; beforefieldinit is left to the compiler.
"""
import hashlib
from pathlib import Path


def first_store(method, offset):
    return next((i for i in method.get('instructions', [])
                 if i['offset'] >= offset and i['opcode'] == 'stsfld'), None)


def field_matches(identity, instruction):
    owner, name = identity.split('::', 1)
    operand = (instruction or {}).get('operand', {}).get('field', '')
    return operand.startswith('class ' + owner + ' @ ') and ('::' + name + ':') in operand


def bind_initializer(syntax, paths, original, rebuilt, alignment):
    key = original['stable_id']
    if not key.endswith('::.cctor()->System.Void'):
        return None
    left = first_store(original, alignment['original_offset'])
    right = first_store(rebuilt, alignment['rebuilt_offset'])
    if left is None or right is None:
        return None
    matches = []
    for logical, physical in paths.items():
        text = physical.read_bytes().decode('utf-8-sig')
        result = syntax.request(path=str(physical), source=text, method_id=key,
                                mode='map-static-initializers')
        for field in result.get('fields', []):
            if field_matches(field['field_id'], left) and field_matches(field['field_id'], right):
                matches.append((logical, physical, text, field))
    if len(matches) != 1:
        return None
    logical, physical, text, field = matches[0]
    return {'point': {'document': str(Path(physical).resolve()), 'start_line': field['line'],
                      'start_column': field['column']},
            'logical_path': logical, 'field': field,
            'mapping_kind': 'AST-static-field-initializer-with-original-and-rebuilt-stsfld-anchors',
            'source_sha256': hashlib.sha256(text.encode('utf-8')).hexdigest(),
            'original_anchor': left, 'rebuilt_anchor': right,
            'unknown': 'Source point is the whole initializer; not a PDB sequence point',
            'preserves_declaration_form': True}
