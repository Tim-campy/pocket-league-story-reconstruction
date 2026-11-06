"""Exercise the unactivated calculated-alias rule in an isolated syntax helper.

Only this helper is compiled. Game assemblies, active helper inputs and ledger
states are untouched. This script does not activate the rule in the engine.
"""
import hashlib
import json
from pathlib import Path
import shutil
import subprocess

from compiler_probes import ROSLYN
import round6_transform

ROOT = Path(__file__).resolve().parents[1]


def isolated_helper():
    source = (ROOT / 'tools/Round6TransformSyntax.cs').read_text(encoding='utf-8-sig')
    call = 'AddRound6CalculatedRules(input,text,mapped,methodBody,model,point,patches,seen);'
    if 'AddRound6CalculatedParameterAlias(input,text,mapped,methodBody,model,point,patches,seen);' not in source:
        assert source.count(call) == 1
        source = source.replace(call, call + '\n        AddRound6CalculatedParameterAlias(input,text,mapped,methodBody,model,point,patches,seen);')
    extra = (ROOT / 'tools/Round6NewRules.cs').read_text(encoding='utf-8-sig')
    draft = (ROOT / 'tools/Round6AliasRules.cs').read_text(encoding='utf-8-sig')
    key = hashlib.sha256((source + extra + draft).encode()).hexdigest()
    folder = ROOT / 'build/round6/alias-rule-tests/helper' / key[:16]
    folder.mkdir(parents=True, exist_ok=True)
    sources = []
    for name, text in [('Round6TransformSyntax.cs', source), ('Round6NewRules.cs', extra), ('Round6AliasRules.cs', draft)]:
        path = folder / name
        path.write_text(text, encoding='utf-8', newline='\n')
        sources.append(path)
    vs = next(p for p in ROSLYN.parents if (p / 'dotnet').is_dir())
    runtime = vs / 'dotnet/net10.0/runtime'
    framework = runtime / 'shared/Microsoft.NETCore.App/10.0.10'
    refs = [framework / (name + '.dll') for name in ['System.Private.CoreLib', 'System.Runtime', 'System.Console',
            'System.Linq', 'System.Collections', 'System.Text.Json', 'System.Reflection.Metadata',
            'System.Collections.Immutable', 'System.Memory', 'netstandard']]
    refs += [ROSLYN.parent / (name + '.dll') for name in ['Microsoft.CodeAnalysis', 'Microsoft.CodeAnalysis.CSharp']]
    target = folder / 'Round6AliasTransform.dll'
    if not target.exists():
        args = ['/nologo', '/nostdlib+', '/target:exe', '/out:' + str(target)]
        args += ['/r:' + str(path) for path in refs] + [str(path) for path in sources]
        rsp = folder / 'compile.rsp'
        rsp.write_text('\n'.join('"' + arg + '"' for arg in args) + '\n', encoding='utf-8', newline='\n')
        subprocess.run([str(ROSLYN), '/noconfig', '@' + str(rsp)], check=True)
        for reference in refs[-2:]:
            shutil.copyfile(reference, folder / reference.name)
        target.with_suffix('.runtimeconfig.json').write_text(json.dumps({'runtimeOptions': {'tfm': 'net10.0',
            'framework': {'name': 'Microsoft.NETCore.App', 'version': '10.0.10'}}}) + '\n')
    return [str(runtime / 'dotnet.exe'), str(target)], key


def main():
    source = '''// Non-BMP span control: \U0001f50e
class CalculatedAliasFixture {
 static void Draw(int x,int y) { }
 static void Alias(int dx,int dy) {
  int y = dy;
  int edge = dx + 2;
  Draw(dx, y);
 }
 static void Ambiguous(int dx,int dy) {
  int y = dy;
  Draw(dx, y); Draw(dx, y);
 }
 static void Repeated(int dx,int dy) {
  int y = dy;
  Draw(dx, dx);
 }
 static void Written(int dx,int dy) {
  int y = dy;
  dx++;
  Draw(dx, y);
 }
 static void Ref(ref int dx,int dy) {
  int y = dy;
  Draw(dx, y);
 }
 static void Captured(int dx,int dy) {
  int y = dy;
  System.Action action = () => Draw(dx,dy);
  Draw(dx, y);
 }
}
'''
    command, key = isolated_helper()
    original_factory = round6_transform.ensure_helper
    round6_transform.ensure_helper = lambda: command
    path = ROOT / 'build/round6/alias-rule-tests/Fixture.cs'
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(source, encoding='utf-8', newline='\n')
    checks = []
    try:
        with round6_transform.TransformClient(ROOT / 'build/round6/baseline/Assembly-CSharp/build-manifest.json') as client:
            def query(method, *, exact=False, count=1, ordinal=0, parameter=0, compiler='smcs', status='unique-calculated-alias-with-equal-body-symbols'):
                start = source.index('static void ' + method + '(')
                mapped = source.index('int y = dy;', start)
                use = source.index('Draw(dx,', mapped)
                line = source.count('\n', 0, use) + 1
                line_start = source.rfind('\n', 0, use) + 1
                line_end = source.index('\n', use)
                point = {'start_line': line, 'start_column': 1 if exact else -1,
                         'end_line': line if exact else -1, 'end_column': line_end-line_start+1 if exact else -1,
                         'parameter_occurrence': ordinal, 'parameter_occurrences': count}
                response = client.request(path, source=source, offset=len(source[:mapped].encode('utf-16-le'))//2,
                    compiler=compiler, max_candidates=512, alias_calculus_status=status,
                    original_alias_parameter_index=parameter, alias_use_source_points=[point])
                assert not response.get('error'), response
                patches = [p for p in response['patches'] if p['rule']=='calculated-parameter-alias']
                for patch in patches:
                    assert source.encode('utf-16-le')[2*patch['start']:2*patch['end']].decode('utf-16-le')==patch['before']
                return patches
            actual = query('Alias')
            assert len(actual)==1, actual
            assert 'dxAlias = dx;' in actual[0]['after'] and 'int edge = dx + 2;' in actual[0]['after']
            assert actual[0]['after'].endswith('Draw(dxAlias'), actual
            checks.append('missing-alias-target-not-present-in-mapped-dy-initializer')
            assert len(query('Alias',exact=True))==1
            checks.append('exact-source-span')
            assert not query('Alias',count=2)
            checks.append('IL-AST-occurrence-count-mismatch-rejected')
            assert not query('Alias',parameter=99)
            checks.append('invalid-parameter-index-rejected')
            assert not query('Alias',compiler='roslyn')
            checks.append('compiler-boundary-rejected')
            assert not query('Alias',status='unknown')
            checks.append('unproved-calculation-status-rejected')
            assert not query('Ambiguous')
            checks.append('line-only-multiple-leaf-statements-rejected')
            repeated = query('Repeated',count=2,ordinal=1)
            assert len(repeated)==1 and repeated[0]['after'].endswith('Draw(dx, dxAlias'), repeated
            checks.append('only-calculated-parameter-read-ordinal-replaced')
            assert not query('Written')
            checks.append('written-parameter-rejected')
            assert not query('Ref')
            checks.append('ref-parameter-rejected')
            assert not query('Captured')
            checks.append('captured-parameter-rejected')
    finally:
        round6_transform.ensure_helper = original_factory
    result = {'operation': 'isolated-syntax-tests-of-calculated-parameter-alias', 'fixture_pass_count': len(checks),
              'helper_source_identity': key, 'checks': checks, 'game_candidates_compiled': 0,
              'active_helper_sources_modified': False, 'ledger_states_promoted': False}
    (ROOT / 'reports/round6-alias-rule-tests.json').write_text(json.dumps(result, indent=2) + '\n', encoding='utf-8', newline='\n')
    print(json.dumps({'fixture_pass_count': len(checks), 'game_candidates_compiled': 0}))


if __name__=='__main__':
    main()
