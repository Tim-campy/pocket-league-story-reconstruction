"""Pure three-component Draw_edit hypothesis; no IO or target compilation."""
import re
from proposal import *

def restore_string_assignments(text):
 literal=r'"(?:\\.|[^"\\])*"'
 pattern=r'\bempty\s*=\s*\(\(et_cur\[4\]\s*==\s*0\)\s*\?\s*('+literal+r')\s*:\s*\(\(et_cur\[4\]\s*!=\s*1\)\s*\?\s*('+literal+r')\s*:\s*('+literal+r')\)\)\s*;'
 match=unique(pattern,text)
 # All operands are literal strings; branch orientation comes from bne.un.
 replacement='if (et_cur[4] == 0) { empty = '+match[1]+'; } else if (et_cur[4] == 1) { empty = '+match[3]+'; } else { empty = '+match[2]+'; }'
 return text[:match.start()]+replacement+text[match.end():]

STAGES=[('merge_zero_chain',merge_x_and_zero_chain),('money_boolean',condition_local_money),('string_assignments',restore_string_assignments)]
def transform_integral(method_source):
 for _,fn in STAGES:method_source=fn(method_source)
 return method_source
