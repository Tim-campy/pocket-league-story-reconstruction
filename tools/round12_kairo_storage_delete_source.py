"""Pure Storage source producer. No I/O, compilation or state updates."""
import re
RULE='r12-storage-delete-heap-return-media-positive-body'
METHOD_ID='kairo.unity.io.Storage::Delete(System.String)->System.Void'

def _close(source,start):
    # This producer's selected block has no embedded strings with braces.
    # Tokenize comments/strings/chars so malformed new inputs fail closed.
    tokens=re.finditer(r'//[^\r\n]*|/\*[\s\S]*?\*/|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'|[{}]',source[start:])
    depth=0
    for m in tokens:
        if m[0]=='{':depth+=1
        elif m[0]=='}':
            depth-=1
            if depth==0:return start+m.start()
    raise ValueError('unbalanced block')

def transform_storage_delete_exits(method_source):
    anchor='if (Config.USE_HEAP_STORAGE && media_ == 2)'
    if method_source.count(anchor)!=1:raise ValueError('one original heap branch required')
    first=method_source.index(anchor);ho=method_source.index('{',first+len(anchor));he=_close(method_source,ho)
    tail=method_source[he+1:];em=re.match(r'\s*else\s*\{',tail)
    if em is None:raise ValueError('original alternative branch required')
    eo=he+1+em.end()-1;ee=_close(method_source,eo);body=method_source[eo+1:ee]
    guard=re.match(r'\s*if\s*\(media_ == 3 \|\| \(media_ != 1 && media_ != 4\)\)\s*\{\s*return;\s*\}',body)
    if guard is None:raise ValueError('original compound unsupported-media guard required')
    heap=method_source[ho+1:he]
    if 'return;'in heap or heap.count('_SearchHeapFile(filename)')!=1:raise ValueError('heap leaves changed')
    payload=body[guard.end():]
    if not payload.lstrip().startswith("if (filename.Contains(':'))")or payload.count('File.Delete(')!=2:
        raise ValueError('original two file-delete leaves required')
    replacement=method_source[first:ho+1]+heap+'\nreturn;\n}'+'\nif (media_ == 3) { return; }\nif (media_ == 1 || media_ == 4) {'+payload+'\n}'
    return method_source[:first]+replacement+method_source[ee+1:]
