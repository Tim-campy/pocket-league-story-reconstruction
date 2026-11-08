"""Two Dictionary cache statements, one local-inside-indexer emission mold."""
import re
def tokens(s):
 p=r'//[^\n]*|/\*[\s\S]*?\*/|@"(?:[^"]|"")*"|"(?:\\.|[^"\\])*"|[A-Za-z_][A-Za-z_0-9]*|\d+|==|!=|<=|>=|&&|\|\||[^\s]'
 return [(m.group(),m.start(),m.end()) for m in re.finditer(p,s) if not m.group().startswith(('//','/*'))]
def local_inside_cache(s,cache,key,local,creation):
 # Callers bind these to exact Dictionary fields and uncaptured out locals.
 # No receiver, index, constructor or getter is duplicated/reordered.
 old=local+' = ('+cache+'['+key+'] = '+creation+');'
 new=cache+'['+key+'] = ('+local+' = '+creation+');'
 ts=tokens(s);w=[t[0] for t in tokens(old)];hits=[i for i in range(len(ts)-len(w)+1) if [t[0] for t in ts[i:i+len(w)]]==w]
 if len(hits)!=1:raise ValueError('reviewed-cache-statement-not-unique')
 i=hits[0];return s[:ts[i][1]]+new+s[ts[i+len(w)-1][2]:]
def transform_material_cache(s):
 return local_inside_cache(s,'materlas_','key','value','new Dictionary<string, Material>()')
def transform_mesh_cache(s):
 return local_inside_cache(s,'meshList_','material','value','new MeshBuilder(this, !material.UntiColor)')
