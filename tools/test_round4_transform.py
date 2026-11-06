"""Focused syntax/mapping checks; these do not promote any reconstructed method."""
import json
from pathlib import Path
from round4_transform import TransformClient

ROOT = Path(__file__).resolve().parents[1]


def main():
    # Unicode before the statements checks that patches use Roslyn UTF-16 spans.
    source = '''// \U0001f50e Local transform fixtures, never adopted into reconstruction.
class Round4Fixture {
 int field;
 bool Check() { return true; }
 int Ternary(int x) { return x == 2 ? 7 : 8; }
 int Floating(float x) { if (x < 0f) { return 1; } else { return 2; } }
 int Integral(int x) { if (x < 0) { return 1; } else { return 2; } }
 int Demorgan(int x) { if (x == 0 && x < 3) { return 1; } else { return 2; } }
 void Loops(int x) { while (x > 0) { if (x > 2 && Check()) { field++; } } }
 void Breaks(int x) { while (true) { if (x > 0) { break; } else { field++; } } }
 void OrGuards(int x) { while (true) { if (x == 0 || Check()) { continue; } field++; } }
 void MergeGuards(int x) { while (true) { if (x == 0) { continue; } if (Check()) { continue; } field++; } }
 void Nested(int x) { if (x > 0) { if (Check()) { field++; } } }
 void Chain() { int local; local = (field = 7); }
 float Neg(float x) { return 0f - x; }
 void Condition() { if (!Check()) { field++; } }
 void Tail(bool x) { if (x) { field++; } }
 int Switch(int x) { switch(x) { case 0:return 1; default:return 2; } }
 int Locals() { int a = 1; int b = 2; int c = 3; return a+b+c; }
 int Scoped() { int sum=0; for (int j = 0; j < 3; j++) { float f=1f; int p=j; int q=j; sum+=p+q; } return sum; }
 int AssignChoice(bool x) { int result = 0; result = x ? 1 : 2; return result; }
 int DeclareChoice(bool x) { int result = x ? 1 : 2; return result; }
}
'''
    path = ROOT / 'build/round4-transform/Fixture.cs'
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(source, encoding='utf-8')
    manifest = ROOT / 'build/round4/baseline/KairoLibrary/build-manifest.json'
    records = []
    def query(client, needle, rule, expected=None):
        position = source.index(needle)
        offset = len(source[:position].encode('utf-16-le')) // 2
        response = client.request(path, source=source, offset=offset)
        candidates = [patch for patch in response.get('patches', []) if patch['rule'] == rule]
        assert candidates, (needle, rule, response)
        if expected:
            assert any(expected in patch['after'] for patch in candidates), candidates
        for patch in response['patches']:
            raw = source.encode('utf-16-le')
            actual = raw[patch['start'] * 2:patch['end'] * 2].decode('utf-16-le')
            assert actual == patch['before'], (actual, patch)
        records.append({'rule': rule, 'needle': needle, 'result': 'PASS', 'after': candidates[0]['after']})
    with TransformClient(manifest) as client:
        query(client, 'return x == 2 ? 7 : 8;', 'ternary-polarity', 'x != 2')
        query(client, 'if (x < 0f)', 'if-polarity', '!(x < 0f)')
        query(client, 'if (x < 0)', 'if-polarity', 'x >= 0')
        query(client, 'if (x == 0 && x < 3)', 'if-polarity', '(x != 0) || (x >= 3)')
        query(client, 'if (x > 2 && Check())', 'loop-continue-guards', 'continue;')
        query(client, 'if (x > 0) { break;', 'guard-break', 'field++;')
        query(client, 'if (x == 0 || Check())', 'condition-split-or-guards', 'if (Check())')
        query(client, 'if (x == 0) { continue; } if', 'condition-merge-or-guards', '||')
        query(client, 'if (x > 0) { if', 'condition-merge-and', '&&')
        query(client, 'local = (field = 7)', 'assignment-chain-order', 'field = (local = 7)')
        query(client, 'return 0f - x;', 'unary-negation', '-(x)')
        query(client, 'if (!Check())', 'condition-local', 'bool recoveredCondition_')
        query(client, 'if (x) { field++; }', 'tail-early-return', 'return;')
        query(client, 'switch(x)', 'switch-if', 'if ((x == 0))')
        query(client, 'while (x > 0)', 'loop-cache-scope', 'while (true)')
        query(client, 'return x == 2 ? 7 : 8;', 'return-local', 'int recoveredReturn_')
        query(client, 'int b = 2;', 'local-declaration-order-earlier', 'b = 2;')
        query(client, 'int a = 1;', 'local-declaration-order-later', 'int b;')
        query(client, 'for (int j = 0;', 'local-declaration-scope-hoist', 'int p;')
        query(client, 'result = x ? 1 : 2;', 'assignment-ternary-if', 'if (x)')
        query(client, 'result = x ? 1 : 2;', 'assignment-ternary-if-reverse', 'if (!(x))')
        query(client, 'int result = x ? 1 : 2;', 'declaration-ternary-if', 'int result;')
        query(client, 'int result = x ? 1 : 2;', 'declaration-ternary-if-reverse', 'if (!(x))')
        target_offset = len(source[:source.index('return x == 2 ? 7 : 8;')].encode('utf-16-le')) // 2
        before_screen = client.request(path, offset=target_offset)
        screened = client.screen_source(source, before_screen['declaration_start'])
        assert 'int Ternary(int x) { return x == 2 ? 7 : 8; }' in screened
        assert 'int Floating(float x) { throw new global::System.NotImplementedException(); }' in screened
        # The screen mode must not replace the full compilation tree in memory.
        after_screen = client.request(path, offset=target_offset)
        assert before_screen['mapped'] == after_screen['mapped']
        assert before_screen['patches'] == after_screen['patches']
        records.append({'rule': 'target-screen-stateless', 'result': 'PASS',
                        'checks': ['UTF-16 declaration position', 'target body preserved',
                                   'other method stubbed', 'semantic tree unchanged']})
    output = ROOT / 'reports/round4-transform-tests.json'
    previous = json.loads(output.read_text(encoding='utf-8')) if output.exists() else {}
    previous['fixture_checks'] = records
    previous['fixture_pass_count'] = len(records)
    previous['checks_scope'] = 'Syntax selection, safe negation, rule generation and UTF-16 spans only; IL matching is measured by the engine.'
    output.write_text(json.dumps(previous, indent=2, ensure_ascii=False) + '\n', encoding='utf-8')
    print(json.dumps({'fixture_checks_passed': len(records), 'report': str(output.relative_to(ROOT))}))


if __name__ == '__main__':
    main()
