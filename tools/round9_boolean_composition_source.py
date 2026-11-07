"""Pure textual composition; no transformer, compiler, model or verifier.

Retain exact input bytes/newlines. Brace discovery masks strings/comments only;
this is not a C# semantic parser and supplies no semantic/oracle approval.
"""
from pathlib import Path
from collections import Counter
import hashlib,json,re

sha=lambda b:hashlib.sha256(b).hexdigest()
read=lambda p:json.loads(p.read_text(encoding='utf-8-sig'))
U=lambda s:len(s.encode('utf-16-le'))//2

TOKEN=re.compile(r'//[^\r\n]*|/\*[\s\S]*?\*/|@"(?:""|[^"])*"|"(?:\\.|[^"\\])*"|\x27(?:\\.|[^\x27\\])*\x27|[A-Za-z_][A-Za-z_0-9]*|[^\s]',re.M)
def tokens(s):return [(m.group(),m.start(),m.end())for m in TOKEN.finditer(s)if not m.group().startswith(('//','/*'))]
def mask(s):
 out=list(s)
 for m in TOKEN.finditer(s):
  t=m.group()
  if t.startswith(('//','/*','"','@"',"'")):
   for n in range(m.start(),m.end()):
    if out[n]not in '\r\n':out[n]=' '
 return ''.join(out)
def close(s,opening):
 assert s[opening]=='{'
 m=mask(s);depth=0
 for i in range(opening,len(s)):
  if m[i]=='{':depth+=1
  elif m[i]=='}':
   depth-=1
   if not depth:return i+1
 raise ValueError('Unclosed block')
def once(s,value):
 assert s.count(value)==1,(value[:80],s.count(value))
 return s.index(value)
def edit(s,changes):
 for a,b,t in sorted(changes,reverse=True):s=s[:a]+t+s[b:]
 return s
def literals(s):return Counter(t for t,_,_ in tokens(s)if t.startswith(('"','@"',"'")))
def calls(s):
 ts=[t for t,_,_ in tokens(s)]
 return Counter(t for i,t in enumerate(ts[:-1])if re.fullmatch('[A-Za-z_][A-Za-z_0-9]*',t)and ts[i+1]=='('and t not in {'if','for','foreach','while','switch','catch','using','lock','typeof','sizeof','default','checked','unchecked'})
def nl(s):return dict(CRLF=s.count('\r\n'),LF_only=s.count('\n')-s.count('\r\n'),CR_only=s.count('\r')-s.count('\r\n'))

def _finish(method_source,changes,reasons,proofs):
 after=edit(method_source,changes)
 assert literals(method_source)==literals(after),'Literal leaf inventory changed'
 assert calls(method_source)==calls(after),'Call-name inventory changed'
 assert close(after,after.index('{'))==len(after),'Method lexical braces invalid'
 return dict(after=after,changes=changes,reasons=reasons,proofs=proofs)

def calculate_graphics(method_source):
 """Pure detail API; source method signature/class/name is unrestricted."""
 before=method_source;changes=[];reasons=[];proofs={}
 condition='flag && renderIsReplace_ && (blendMode_ == 0 || blendMode_ == 3) && renderForceMaterial_ == null && localMatrixIndex_ == -1 && !useFigureClip_ && !setFigureClip_'
 c=once(before,condition);left=condition[:-len(' && !setFigureClip_')]
 changes.append((c,c+len(condition),'!('+left+') ? false : (!setFigureClip_)'))
 reasons.append(dict(edit='materialize-first-Boolean-value-without-local',original_offsets=[1880,1881,1883,1885,1886],evidence='Preserved D11 SCREEN candidate reproduces these exact instructions; does not prove complete body'))
 marker='else if ((flag) || (!(flag2))) '
 a=once(before,marker);fresh_open=a+len(marker);fresh_end=close(before,fresh_open)
 join=before[fresh_end:];separator=re.match(r'\s*else\s*',join);assert separator
 cached_open=fresh_end+separator.end();cached_end=close(before,cached_open)
 fresh=before[fresh_open:fresh_end];cached=before[cached_open:cached_end]
 after_branch='else if ((!flag) & flag2) '+cached+' else '+fresh
 changes.append((a,cached_end,after_branch))
 reasons.append(dict(edit='restore-eager-Boolean-and-with-cache-first-block-order',original_offsets=[1963,1965,1966,1968,1970,1971,1989,1992,2013,2198,2207,2225,2237],evidence='Original ceq;ldloc17;and and cached-mesh block precede fresh-pass block; unlike previously rejected R6 logical-&& polarity-only reversal'))
 proofs.update(cached_block_copied_exact=True,fresh_block_copied_exact=True,branches_select_same_old_path_for_all_Boolean_pairs=all(((not flag)&flag2)==(not(flag or not flag2))for flag in [False,True]for flag2 in [False,True]),first_P_expression_text_copied_exact=left in condition)
 return _finish(before,changes,reasons,proofs)

def transform_graphics(method_source):
 """Return transformed method text; no I/O or external dependencies."""
 return calculate_graphics(method_source)['after']

def calculate_shader(method_source):
 """Pure detail API; source method signature/class/name is unrestricted."""
 before=method_source;changes=[];reasons=[];proofs={}
 alias='bool recoveredCondition_7106 = (nameId & 2) != 0;'
 a=once(before,alias);test='if (recoveredCondition_7106)';b=once(before,test)+len(test)
 assert before.count('recoveredCondition_7106')==2
 changes.append((a,b,'if ((nameId & 2) != 0)'))
 reasons.append(dict(edit='inline-adjacent-tested-Boolean-alias',original_offsets=[197,198,199,200],evidence='Preserved D11 candidate removes extra ldc0;cgt.un; unchanged16-slot signature'))
 lhs='text4 = (((nameId & 0x10000) != 0) ? "SetTexture [_MainTex] { combine texture }" : "SetTexture [_MainTex] { combine texture * primary }");'
 a=once(before,lhs)
 replacement='if ((nameId & 0x10000) == 0)\r\n\t\t\t\t\t\t{\r\n\t\t\t\t\t\t\ttext4 = "SetTexture [_MainTex] { combine texture * primary }";\r\n\t\t\t\t\t\t}\r\n\t\t\t\t\t\telse\r\n\t\t\t\t\t\t{\r\n\t\t\t\t\t\t\ttext4 = "SetTexture [_MainTex] { combine texture }";\r\n\t\t\t\t\t\t}'
 changes.append((a,a+len(lhs),replacement))
 reasons.append(dict(edit='text4-store-in-each-arm',original_offsets=[454,456,461,463,465,470,472],evidence='Original has stloc4 in both arms, candidate common stloc4 at468; original block53bytes vs candidate51'))
 marker='else if ((nameId & 0x80) == 0)'
 a=once(before,marker);old_default_open=before.index('{',a);old_default_end=close(before,old_default_open)
 chain_start=before.index('else if ((nameId & 0x2000) != 0)',old_default_end)
 chain_end=before.index('if (num != 0)',chain_start)
 # Use exact existing three-arm positive chain, including its literal leaves.
 chain=before[chain_start+len('else '):chain_end].rstrip()
 assert chain.count('text3 =')==3
 old_span=before[a:chain_end]
 assert old_span.count('text3 =')==4
 default='if ((nameId & 0x100) != 0)\r\n\t\t\t\t\t{\r\n\t\t\t\t\t\ttext3 = "Blend Off";\r\n\t\t\t\t\t}\r\n\t\t\t\t\telse\r\n\t\t\t\t\t{\r\n\t\t\t\t\t\ttext3 = "Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha";\r\n\t\t\t\t\t}'
 replacement='else if ((nameId & 0x80) != 0)\r\n\t\t\t\t{\r\n\t\t\t\t\t'+chain+'\r\n\t\t\t\t}\r\n\t\t\t\telse\r\n\t\t\t\t{\r\n\t\t\t\t\t'+default+'\r\n\t\t\t\t}\r\n\t\t\t\t'
 changes.append((a,chain_end,replacement))
 reasons.extend([dict(edit='bit0x80-positive-chain-before-default',original_offsets=[545,552,554,571,588,605],evidence='Original bit0x80 set arm emits positive chain before bit0x100 default; current source does the opposite'),dict(edit='text3-default-store-in-each-arm',original_offsets=[612,614,619,620,622,627],evidence='Original has stloc3 in both default arms; current ternary stores common result')])
 terminal='return text.Replace("\x27", "\\\"");'
 a=once(before,terminal)
 changes.append((a,a+len(terminal),'text = '+terminal[len('return '):]+'\r\n\t\t\treturn text;'))
 reasons.append(dict(edit='assign-final-Replace-result-then-return-text',original_offsets=[1069,1070,1071],evidence='Original stloc0;ldloc0;ret; current returns Replace result directly'))
 proofs.update(alias_exactly_declaration_and_one_read=True,positive_bit0x80_chain_copied_exact=True,bit0x80_ordering_guard_uses_same_integer_flag=True)
 return _finish(before,changes,reasons,proofs)

def transform_shader(method_source):
 """Return transformed method text; no I/O or external dependencies."""
 return calculate_shader(method_source)['after']

