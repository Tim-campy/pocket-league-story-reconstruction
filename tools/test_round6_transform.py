"""Syntax/safety fixtures only; no game compilation or state promotion."""
import json
from pathlib import Path

from round6_transform import TransformClient

ROOT = Path(__file__).resolve().parents[1]


def main():
    source = '''// Unicode before spans: \U0001f50e
class Round6Fixture {
 int field;
 int Property {get {return field;}}
 void Use(int value) { field=value; }
 void Close() { }
 Round6Fixture Fetch() {return this;}
 void Chain(int type) { if (type==1) {Use(1);} else if(type==2) {Use(2);} else {Use(3);} }
 void Or(int type) { if(type==1 || type==2) { Use(type); } }
 void Guard(int type) { if(type!=1 && type!=2) {return;} Use(type); }
 void Different(int type,int other) { if(type==1) {Use(1);} else if(other==2) {Use(2);} }
 void Duplicate(int type) { if(type==1) {Use(1);} else if(type==1) {Use(2);} }
 void UnsafeSwitch(int type) { while(true) {if(type==1 || type==2) {break;}} }
 void Swap(int x,int y) {int a=x; int b=y;Use(a+b);}
 void Dependent(int x) {int a=x;int b=a+1;Use(b);}
 void Calls(int x) {int a=x;int b=Property;Use(a+b);}
 void Conditional(Round6Fixture obj) { obj?.Close(); }
 void ConditionalCall() { Fetch()?.Close(); }
 string NullReturn(string obj) {return obj?.ToString();}
 string NullIndex(string[] obj,int index) {return obj?[index];}
 void NullLocal(string[] obj,int index) {string item=obj?[index];Use(item.Length);}
 string Coalesce(string obj,string other) {return obj ?? other;}
 void CoalesceLocal(string obj,string other) {string item=obj ?? other;Use(item.Length);}
 int Nullable(int? obj) {return obj ?? 5;}
}
'''
    path = ROOT / 'build/round6-transform/NewFixture.cs'
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(source, encoding='utf-8', newline='\n')
    checks = []
    with TransformClient(ROOT / 'build/round6/baseline/KairoLibrary/build-manifest.json') as client:
        def query(method, needle, **hints):
            start = source.index(method)
            point = source.index(needle, start)
            result = client.request(path, source=source, offset=len(source[:point].encode('utf-16-le')) // 2,
                                    max_candidates=1024, **hints)
            assert not result.get('error'), result
            for patch in result['patches']:
                assert source.encode('utf-16-le')[2 * patch['start']:2 * patch['end']].decode('utf-16-le') == patch['before']
            return result['patches']
        def has(method, needle, rule, expected, **hints):
            found = [p for p in query(method, needle, **hints) if p['rule'] == rule]
            assert found, (method, rule)
            assert any(expected in p['after'] for p in found), (method, rule, found)
            checks.append({'check': method + ':' + rule, 'result': 'PASS'})
        def lacks(method, needle, prefix, **hints):
            assert not any(p['rule'].startswith(prefix) for p in query(method, needle, **hints)), method
            checks.append({'check': method + ':reject-' + prefix, 'result': 'PASS'})
        has('void Chain(', 'if (type==1)', 'if-chain-switch', 'switch (type)', compiler='smcs')
        has('void Or(', 'if(type==1', 'if-chain-switch', 'case 2:', compiler='smcs')
        has('void Guard(', 'if(type!=1', 'if-guard-switch', 'default:', compiler='smcs')
        has('void Guard(', 'if(type!=1', 'if-guard-tail-switch', 'Use(type);', compiler='smcs')
        lacks('void Different(', 'if(type==1', 'if-chain-switch', compiler='smcs')
        lacks('void Duplicate(', 'if(type==1', 'if-chain-switch', compiler='smcs')
        lacks('void UnsafeSwitch(', 'if(type==1', 'if-chain-switch', compiler='smcs')
        lacks('void Or(', 'if(type==1', 'if-chain-switch', compiler='roslyn')
        has('void Swap(', 'int a=x;', 'initialized-declaration-swap', 'int b=y;\nint a=x;', compiler='smcs')
        lacks('void Dependent(', 'int a=x;', 'initialized-declaration-swap', compiler='smcs')
        lacks('void Calls(', 'int a=x;', 'initialized-declaration-swap', compiler='smcs')
        has('void Conditional(', 'obj?.Close()', 'null-conditional-explicit', '(obj).Close();', compiler='roslyn')
        lacks('void ConditionalCall(', 'Fetch()?.Close()', 'null-', compiler='roslyn')
        has('string NullReturn(', 'return obj?.', 'null-conditional-return-explicit', 'return null;', compiler='roslyn')
        has('string NullIndex(', 'return obj?[', 'null-conditional-return-explicit', '(obj)[index]', compiler='roslyn')
        has('void NullLocal(', 'string item=', 'null-conditional-local-explicit', 'item = null;', compiler='roslyn')
        has('string Coalesce(', 'return obj ??', 'null-coalesce-return-explicit', 'return other;', compiler='roslyn')
        has('void CoalesceLocal(', 'string item=', 'null-coalesce-local-explicit', 'item = other;', compiler='roslyn')
        lacks('int Nullable(', 'return obj ??', 'null-coalesce', compiler='roslyn')
        lacks('void Conditional(', 'obj?.Close()', 'null-', compiler='smcs')
    results = {'new_fixture_pass_count': len(checks), 'new_checks': checks,
               'scope': 'syntax/safety only; all candidate body acceptance belongs to official verifier'}
    # Reuse the previous 41 fixture specifications, with only the helper,
    # baseline and destination changed. Historical files are never overwritten.
    for old, output in [('test_round5_transform.py', 'round6-transform-round5-controls.json'),
                        ('test_round5_legacy_transform.py', 'round6-transform-legacy-controls.json')]:
        text = (ROOT / 'tools' / old).read_text(encoding='utf-8-sig').replace('round5', 'round6')
        text = text.replace('round6-transform-tests.json', output).replace('round6-legacy-transform-tests.json', output)
        namespace = {'__file__': str(ROOT / 'tools' / old), '__name__': 'round6_fixture_reuse'}
        exec(compile(text, str(ROOT / 'tools' / old), 'exec'), namespace)
        namespace['main']()
        data = json.loads((ROOT / 'reports' / output).read_text(encoding='utf-8-sig'))
        results[output] = {'fixture_pass_count': data['fixture_pass_count']}
    results['fixture_pass_count'] = len(checks) + sum(v['fixture_pass_count'] for v in results.values() if isinstance(v, dict) and 'fixture_pass_count' in v)
    (ROOT / 'reports/round6-transform-tests.json').write_text(json.dumps(results, indent=2) + '\n', encoding='utf-8', newline='\n')
    print(json.dumps({'fixture_pass_count': results['fixture_pass_count']}))


if __name__ == '__main__':
    main()
