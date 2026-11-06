"""Choose a proved conditional's offset for source mapping, never an IL oracle."""


def tail_source_mapping_offset(alignment_rebuilt_offset, hints, compiler='smcs'):
    """Return a diagnostic dict with old_offset/new_offset/overridden.

    Call before select_point, after calculating independent tail-guard hints.
    The accepted method, its IL, alignment and equality oracle stay untouched.
    """
    result={'old_offset':alignment_rebuilt_offset,'new_offset':alignment_rebuilt_offset,
        'overridden':False,'reason':'no-proved-tail-guard','oracle_changed':False}
    if compiler not in ('smcs','mono-smcs'):return result
    if hints.get('original_infinite_tail_guard') is not True:return result
    candidate=hints.get('infinite_tail_rebuilt_offset')
    if isinstance(candidate,bool) or not isinstance(candidate,int) or candidate<0:return result
    if isinstance(alignment_rebuilt_offset,bool) or not isinstance(alignment_rebuilt_offset,int):return result
    if candidate>alignment_rebuilt_offset:return result
    result.update(new_offset=candidate,overridden=candidate!=alignment_rebuilt_offset,
        reason='proved-rebuilt-tail-conditional',original_conditional_offset=hints.get('infinite_tail_original_offset'))
    return result


def tail_source_mapping_record(mapping, source_point):
    """Append the actual selected symbol offset/document/line to an audit row."""
    result=dict(mapping)
    if source_point:
        result.update(source_offset=source_point.get('offset'),document=source_point.get('document'),
            line=source_point.get('start_line'),column=source_point.get('start_column'))
    else:result['source_offset']=None
    return result
