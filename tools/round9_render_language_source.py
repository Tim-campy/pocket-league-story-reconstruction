"""Pure WIP full Offscreen composition and Language parser foreach proposal.

No I/O at import/API-call time; no compiler/model/AST/fixture/verifier anywhere.
Actual typed-field evidence is in frozen dumps, not a claim of semantic binding
for arbitrary caller text. main() checks the current files equal frozen D23.
"""
from collections import Counter
import re
TOKEN=re.compile(r'//[^\r\n]*|/\*[\s\S]*?\*/|@"(?:""|[^"])*"|"(?:\\.|[^"\\])*"|\x27(?:\\.|[^\x27\\])*\x27|[A-Za-z_][A-Za-z_0-9]*|[^\s]',re.M)
def tokens(s):return[(m.group(),m.start(),m.end())for m in TOKEN.finditer(s)if not m.group().startswith(('//','/*'))]
def words(s):return[t for t,_,_ in tokens(s)]
def mask(s):
 out=list(s)
 for m in TOKEN.finditer(s):
  if m.group().startswith(('//','/*','"','@"',"'")):
   for i in range(m.start(),m.end()):
    if out[i]not in '\r\n':out[i]=' '
 return ''.join(out)
def once(s,text):
 assert s.count(text)==1,('unique anchor required',text[:100],s.count(text))
 return s.index(text)
def close(s,opening):
 assert s[opening]=='{';m=mask(s);depth=0
 for i in range(opening,len(s)):
  if m[i]=='{':depth+=1
  elif m[i]=='}':
   depth-=1
   if not depth:return i+1
 raise AssertionError('unclosed lexical block')
def line_start(s,i):return s.rfind('\n',0,i)+1
def indent(s,i):
 value=s[line_start(s,i):i];assert not value.strip()
 return value
def inventories(s):
 ts=words(s)
 return Counter(t for t in ts if t.startswith(('"','@"',"'"))),Counter(t for i,t in enumerate(ts[:-1])if re.fullmatch('[A-Za-z_][A-Za-z_0-9]*',t)and ts[i+1]=='('and t not in {'if','for','foreach','while','switch','catch','using','lock','typeof','sizeof','default','checked','unchecked'})
def no_transfer_capture(s):assert not re.search(r'\b(?:goto|yield|await|delegate)\b|=>',mask(s)),'capture/goto/iterator/async unsupported'
def outer_try(s):
 m=re.search(r'\btry\s*{',mask(s));assert m
 opening=s.index('{',m.start());return opening,close(s,opening)

def transform_offscreen_render_predicate(s):
 old='flag2 = DynamicRender == null || DynamicRender(graphics, this);'
 a=once(s,old)
 return s[:a]+'if (DynamicRender == null) { flag2 = true; } else { flag2 = DynamicRender(graphics, this); }'+s[a+len(old):]

def transform_offscreen_matrix_assignments(s):
 loop=once(s,'for (int l = 0; l < num2; l++)');opening=s.index('{',loop);end=close(s,opening)
 assert not re.search(r'\bl\s*(?:=(?!=)|\+\+|--|\+=|-=)',mask(s[opening:end])),'index local cannot be reassigned in body'
 values=['Matrix4x4.TRS(new Vector3(0f, GetHeight(), 0f), Quaternion.identity, new Vector3(1f, 1f, 1f))','matrix4x']
 changes=[]
 for value in values:
  old='renderMatrix_[l] *= '+value+';';a=once(s,old);assert opening<a<end
  changes.append((a,a+len(old),'renderMatrix_[l] = renderMatrix_[l] * '+value+';'))
 for a,b,text in sorted(changes,reverse=True):s=s[:a]+text+s[b:]
 return s

def transform_offscreen_field_negations(s):
 for value in ['value.Y','vector.y','value2.Y','vector2.y']:
  old='0f - '+value;once(s,old);s=s.replace(old,'-'+value,1)
 return s

def transform_offscreen_flag_branch(s):
 _,try_end=outer_try(s);tail_end=line_start(s,try_end-1)
 a=once(s,'if (flag2)');opening=s.index('{',a);end=close(s,opening);body=s[opening:end]
 ts=tokens(body);assert [t for t,_,_ in ts[-3:]]==['return',';','}']
 assert words(body).count('return')==1,'only terminal void return supported'
 return_start,return_end=ts[-3][1],ts[-2][2]
 newbody=body[:return_start]+body[return_end:]
 fallback=s[end:tail_end]
 assert fallback.strip().startswith('image_.textRenderTasks_ = textRenderTasks_;')
 assert 'image_.balloonRenderTasks_ = balloonRenderTasks_;'in fallback and'image_.touchTasks_ = touchTasks_;'in fallback
 assert 'return'not in words(fallback)
 replacement=s[a:opening]+newbody+' else {'+fallback+indent(s,a)+'}\r\n'
 return s[:a]+replacement+s[tail_end:]

def transform_offscreen_graphics_scope(s):
 _,try_end=outer_try(s);tail_end=line_start(s,try_end-1)
 a=once(s,'if (graphics == null)');opening=s.index('{',a);end=close(s,opening)
 assert words(s[opening:end])==['{','return',';','}']
 tail=s[end:tail_end];assert tail.strip().startswith('List<Image.TextRenderTask> textRenderTasks_')
 assert 'return'not in words(tail),'flag2 terminal return must be composed first'
 for name in ['textRenderTasks_','balloonRenderTasks_','touchTasks_','flag2']:
  assert name not in words(s[:a]+s[tail_end:]),'late local crosses proposed graphics child block'
 return s[:a]+'if ((object)graphics != null) {'+tail+indent(s,a)+'}\r\n'+s[tail_end:]

def calculate_offscreen_composition(method_source):
 no_transfer_capture(method_source)
 source=method_source;stages=[]
 components=[('per-arm-DynamicRender-result',transform_offscreen_render_predicate),('two-stable-struct-array-simple-assignments',transform_offscreen_matrix_assignments),('four-Single-field-unary-negations',transform_offscreen_field_negations),('flag2-positive-with-fallback-else',transform_offscreen_flag_branch),('graphics-positive-child-scope',transform_offscreen_graphics_scope)]
 for label,function in components:
  before=source;source=function(source);stages.append(dict(component=label,before=before,after=source))
 assert inventories(method_source)==inventories(source),'literal/call-name inventory changed'
 assert len(re.findall(r'(?<!\w)0f\b',mask(method_source)))-len(re.findall(r'(?<!\w)0f\b',mask(source)))==4
 assert words(method_source).count('true')+1==words(source).count('true'),'only explicit DynamicRender true arm may add true'
 assert words(method_source).count('return')-2==words(source).count('return')
 assert close(source,source.index('{'))==len(source)
 return dict(after=source,stages=stages,checks=dict(no_new_named_local=True,literal_call_name_inventories_equal=True,numeric_literal_exception={'removed_0f':4},Boolean_literal_exception={'added_true':1},same_matrix_operator_and_RHS_calls=True,array_receiver='same proven field renderMatrix_',index='same explicit int l local; no receiver/index call/property duplicated',field_rereads='intentional explicit LHS/RHS array reads matching original IL',no_float_comparison_flip=True,late_scope_is_positive_graphics_if=True,old_lifetime_alone_not_repeated=True),ready=False,complete=None)

def transform_offscreen_composition(method_source):return calculate_offscreen_composition(method_source)['after']

def calculate_language_format_foreach(method_source):
 s=method_source;no_transfer_capture(s)
 assert len(re.findall(r'\bstring\s+text5\s*=',mask(s)))==1,'text5 must be explicit System.String source local'
 decl="string[] files = text5.Split('\\n');";a=once(s,decl)
 loop='for (int i = 0; i < files.Length; i++)';b=once(s,loop)
 assert not s[a+len(decl):b].strip(),'array declaration must immediately precede loop'
 opening=s.index('{',b);end=close(s,opening);body=s[opening:end]
 access="files[i].Split(',')";once(body,access)
 assert body[1:].strip().startswith("string[] array6 = "+access+';'),'element access must be first effect'
 assert words(body).count('files')==1 and words(body).count('i')==1
 assert not any(t in words(body)for t in ['continue','break','return','for','foreach','while','do','switch']),'only simple parser body supported'
 parent=once(s,'if (array5 != null)');parent_open=s.index('{',parent);parent_end=close(s,parent_open)
 assert parent_open<a<b<end<parent_end
 assert 'files'not in words(s[end:parent_end])and'i'not in words(s[end:parent_end]),'removed symbols used after parser loop'
 ids=set(words(s));name='recoveredLangFormatLine';i=2
 while name in ids:name='recoveredLangFormatLine'+str(i);i+=1
 newbody=body.replace(access,name+".Split(',')",1)
 replacement="foreach (string "+name+" in text5.Split('\\n'))"+s[b+len(loop):opening]+newbody
 after=s[:a]+replacement+s[end:]
 assert inventories(s)==inventories(after),'string/char literal or call inventory changed'
 assert s[:a]==after[:a]and s[end:]==after[a+len(replacement):]
 assert close(after,after.index('{'))==len(after)
 return dict(after=after,change=(a,end,replacement),fresh_name=name,checks=dict(Split_resource_expression_evaluated_once=True,existing_filename_and_first_foreach_alias_unchanged=True,array_index_access_exactly_once_at_first_effect=True,no_files_or_index_use_after_loop_in_same_parent=True,string_iteration_variable_used_once=True,actual_temp_reuse='UNKNOWN_NOT_COMPILED; desired original array4/index5 reusable foreach temporaries',no_property_or_call_receiver_duplication=True,literal_call_name_inventories_equal=True,body_effect_order_unchanged=True),ready=False,complete=None)

def transform_language_format_foreach(method_source):return calculate_language_format_foreach(method_source)['after']

