"""Pure parent-source tail-scope producer; no generated class is written."""
import re
RULE='r12-utc-fallback-local-after-using-return-temp'
METHOD_ID='java.native.JavaPlugin/<>c__DisplayClass8_0::<GetUtcOffset>b__0()->System.Object'
PARENT_ID='java.native.JavaPlugin::GetUtcOffset(System.String)->System.Int32'

def transform_utc_fallback_scope(parent_method_source):
    pattern=re.compile(r'if \(utcOffsetTable_.Count == 0\)\s*\{\s*CreateUtcOffsetTable\(\);\s*\}\s*int value = 0;\s*utcOffsetTable_.TryGetValue\(timezone, out value\);\s*return value;')
    matches=list(pattern.finditer(parent_method_source))
    if len(matches)!=1:raise ValueError('one exact original dictionary fallback required')
    m=matches[0]
    if parent_method_source[:m.start()].rstrip().endswith('{'):
        raise ValueError('fallback already has a separate child scope')
    if parent_method_source[:m.start()].count('using (AndroidJavaObject androidJavaObject')!=1:
        raise ValueError('one preceding Android using scope required')
    if len(re.findall(r'\bint\s+value\b',parent_method_source))!=1:raise ValueError('one existing fallback value local required')
    return parent_method_source[:m.start()]+'{\n'+m[0]+'\n}'+parent_method_source[m.end():]
