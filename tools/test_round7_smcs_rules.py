"""Syntax safety plus historical-smcs toy emission; no game builds or state writes."""
import json
import os
from pathlib import Path
import subprocess
from compiler_probes import MONO, SMCS, UNITY
from round7_smcs_fixture_helper import FixtureClient

ROOT = Path(__file__).resolve().parents[1]
CORE = ROOT / 'original/extracted/assets/bin/Data/Managed/mscorlib.dll'
SOURCE = '''// UTF16 span control: \U0001f50e
class Fixture {
 delegate int IntFunc(); delegate void VoidAction();
 int field; Fixture child; static Fixture shared;
 Fixture Property {get {return child;} set {child=value;}}
 int Get() {return field;} bool Check() {return field!=0;}
 Fixture GetObj() {return child;} void Use(int x) {field=x;} void Accept(Fixture value) {child=value;}
 int AnyType() {if(Get()!=0) {return 1;} return 0;}
 int ReferenceType() {if(GetObj()!=null) {return 1;} return 0;}
 int ShortCircuit(bool flag) {if(flag&&Get()!=0) {return 1;}return 0;}
 int NoHint() {if(Get()!=0) {return 1;} return 0;}
 int Unified(int x) {int result=0; if(x<5) {return Get();} return x;}
 int Nested(int x) {IntFunc nested=delegate {return 99;};if(x<5) {return nested();}return x;}
 void VoidGuard(int x) {if(x!=1&&x!=2) {return;}Use(x);}
 void VoidAlias(int x) {int typeAlias=x;if(typeAlias!=1&&typeAlias!=2) {return;}Use(typeAlias);}
 int GotoExit(int x) {int value=0;if(x<5) {value=Get();goto exit;} {value=x;goto exit;}exit:return value;}
 void VoidSwitch(int x) {switch(x) {case 1:Use(1);return;default:Use(2);return;}}
 void NonTerminalSwitch(int x) {switch(x) {case 1:Use(1);return;}Use(3);}
 void NestedTail(int x) {if(x==0) {Use(0);}else {if(x!=11) {return;}Use(11);}}
 void NonTail(int x) {if(x==0) {Use(0);}else {if(x!=11) {return;}Use(11);}Use(99);}
 void EmptyCheck() {if(Check()&&!Check()) {}}
 void UsedCheck() {if(Check()&&!Check()) {Use(1);}}
 void NewLocal() {child=new Fixture();}
 void NewProperty() {Property=new Fixture();}
 void NewUnstable() {GetObj().child=new Fixture();}
 void NewStatic() {shared=new Fixture();}
 void NewArgument() {Accept(new Fixture() {field=Get()});}
 void NewArgumentLambda() {VoidAction act=delegate {Accept(new Fixture() {field=Get()});};act();}
 void NewArgumentUnsafe() {GetObj().Accept(new Fixture() {field=Get()});}
 void Zero() {int a;int b;int c=(b=(a=0));int d=2;Use(a+b+c+d);}
 void ZeroSwitch(int x) {int a;int b;int c=(b=(a=0));int d=2;switch(x) {case 1:Use(a);break;case 2:Use(b);break;}Use(c+d);}
 void ZeroShared(bool flag) {int a;int b=(a=0);if(flag) {int c=1;Use(c);}else {int c=2;Use(c);}Use(a+b);}
 void ZeroCapture() {int a;int b=(a=0);int c=1;VoidAction act=delegate {Use(a);};Use(b+c);act();}
}
'''


def apply(source, patch):
    data = source.encode('utf-16-le')
    return (data[:patch['start']*2] + patch['after'].encode('utf-16-le') + data[patch['end']*2:]).decode('utf-16-le')


def compile_toy(source, name):
    folder = ROOT / 'build/round7-smcs-fixtures' / name
    folder.mkdir(parents=True, exist_ok=True)
    path = folder / 'Fixture.cs'
    path.write_text(source, encoding='utf-8', newline='\n')
    dll = folder / 'Fixture.dll'
    flags = ['-nologo', '-noconfig', '-nostdlib', '-target:library', '-debug-', '-optimize+',
             '-out:' + str(dll), '-r:' + str(CORE), str(path)]
    environment = os.environ.copy()
    environment['MONO_PATH'] = str(UNITY / 'Editor/Data/Mono/lib/mono/unity')
    environment['MONO_CFG_DIR'] = str(UNITY / 'Editor/Data/Mono/etc')
    result = subprocess.run([str(MONO), str(SMCS), *flags], capture_output=True, text=True, env=environment, timeout=45)
    (folder / 'compiler.log').write_text(result.stdout + result.stderr, encoding='utf-8')
    assert result.returncode == 0, result.stdout + result.stderr
    result = subprocess.run([str(ROOT / 'build/tools/ProbeInspector.exe'), str(dll), '--all'],
                            capture_output=True, text=True, encoding='utf-8', timeout=30, check=True)
    data = json.loads(result.stdout)
    (folder / 'methods.json').write_text(json.dumps(data, indent=2) + '\n', encoding='utf-8')
    return {m['key']:m for m in data['selected_methods']}


def main():
    client = FixtureClient(SOURCE, [CORE])
    checks = []
    chosen = {}
    def has(method, needle, rule, snippet=None, **hints):
        found = [p for p in client.request(method, needle, **hints) if p['rule']==rule]
        if snippet is not None:
            found = [p for p in found if snippet in p['after']]
        assert found, (method, rule, snippet)
        chosen[(method, rule)] = found[0]
        checks.append({'fixture': method + ':' + rule, 'status': 'PASS'})
        return found[0]
    def lacks(method, needle, prefix, **hints):
        assert not any(p['rule'].startswith(prefix) for p in client.request(method, needle, **hints)), (method, prefix)
        checks.append({'fixture': method + ':reject-' + prefix, 'status': 'PASS'})
    try:
        has('int AnyType(', 'if(Get()', 'tested-value-local', 'int testedValue = Get();', expected_test_local_types=['System.Int32'])
        has('int ReferenceType(', 'if(GetObj()', 'tested-value-local', 'global::Fixture testedValue = GetObj();', expected_test_local_types=['Fixture'])
        lacks('int ShortCircuit(', 'if(flag', 'tested-value-local', expected_test_local_types=['System.Int32'])
        lacks('int NoHint(', 'if(Get()', 'tested-value-local')
        lacks('int AnyType(', 'if(Get()', 'tested-value-local', expected_test_local_types=['System.String'])
        has('int AnyType(', 'if(Get()', 'tested-value-local-block', '{ int testedValue', expected_alias_types=['System.Int32'])
        has('int Unified(', 'return Get()', 'unified-result-return-goto', 'result = Get();', original_unified_return=True)
        has('int Unified(', 'return Get()', 'unified-result-return-if-else', 'result = Get(); } else { result = x;', original_unified_return=True)
        nested=has('int Nested(', 'return nested()', 'unified-result-return-goto', 'return 99;', original_unified_return=True)
        assert 'resultValue = 99' not in nested['after']
        lacks('int Unified(', 'return Get()', 'unified-result-return')
        has('void VoidGuard(', 'if(x!=1', 'void-common-exit-if-else', '(x == 1) || (x == 2)', original_void_common_exit=True)
        has('void VoidGuard(', 'if(x!=1', 'void-common-exit-switch-break', 'case 2:', original_void_common_exit=True)
        has('void VoidAlias(', 'return;', 'void-common-exit-switch-parameter', 'switch (x)', original_void_common_exit=True)
        has('int GotoExit(', '{value=x;', 'common-exit-remove-fallthrough-goto', '')
        has('void VoidSwitch(', 'Use(1)', 'void-common-exit-switch-break', 'Use(1);break;', original_void_common_exit=True)
        lacks('void NonTerminalSwitch(', 'Use(1)', 'void-common-exit-switch-break', original_void_common_exit=True)
        has('void NestedTail(', 'if(x!=11)', 'terminal-guard-positive-tail', 'x == 11',round7_cycle_rule='terminal-guard-positive-tail')
        lacks('void NonTail(', 'if(x!=11)', 'terminal-guard-positive-tail',round7_cycle_rule='terminal-guard-positive-tail')
        has('void EmptyCheck(', 'if(Check()', 'empty-if-terminal-polarity', 'Check()&&Check()',round7_cycle_rule='empty-if-terminal-polarity')
        lacks('void UsedCheck(', 'if(Check()', 'empty-if-terminal-polarity',round7_cycle_rule='empty-if-terminal-polarity')
        lacks('void NestedTail(', 'if(x!=11)', 'terminal-guard-positive-tail')
        lacks('void EmptyCheck(', 'if(Check()', 'empty-if-terminal-polarity')
        has('void NewLocal(', 'child=new', 'new-object-local-before-field', 'global::Fixture createdValue = new Fixture();', expected_newobj_local_types=['Fixture'])
        lacks('void NewProperty(', 'Property=new', 'new-object-local-before-field', expected_newobj_local_types=['Fixture'])
        lacks('void NewUnstable(', 'GetObj().child', 'new-object-local-before-field', expected_newobj_local_types=['Fixture'])
        lacks('void NewStatic(', 'shared=new', 'new-object-local-before-field', expected_newobj_local_types=['Fixture'])
        lacks('void NewLocal(', 'child=new', 'new-object-local-before-field')
        has('void NewArgument(', 'Accept(new', 'new-object-argument-local', 'createdValue.field = Get();', expected_newobj_local_types=['Fixture'])
        has('void NewArgumentLambda(', 'Accept(new', 'new-object-argument-local', 'createdValue.field = Get();', expected_newobj_local_types=['Fixture'])
        lacks('void NewArgumentUnsafe(', 'GetObj().Accept', 'new-object-argument-local', expected_newobj_local_types=['Fixture'])
        has('void Zero(', 'int c=', 'extended-original-zero-chain', 'd = c = b = a = 0;',
            original_zero_chain_names=['d','c','b','a'], original_zero_chain_declaration_order=['a','b','c','d'])
        has('void ZeroShared(', 'int b=', 'extended-original-zero-chain', 'c = b = a = 0;',
            original_zero_chain_names=['c','b','a'], original_zero_chain_declaration_order=['a','b','c'])
        has('void ZeroSwitch(', 'int c=', 'extended-original-zero-chain-and-switch-if', 'if ((x == 1))',
            original_zero_chain_names=['d','c','b','a'], original_zero_chain_declaration_order=['a','b','c','d'],original_has_no_switch=True)
        lacks('void ZeroCapture(', 'int b=', 'extended-original-zero-chain',
            original_zero_chain_names=['c','b','a'], original_zero_chain_declaration_order=['a','b','c'])
        lacks('void Zero(', 'int c=', 'extended-original-zero-chain',
            original_zero_chain_names=['d','c','b','a'], original_zero_chain_declaration_order=['a','b','c'])
    finally:
        client.close()
    # Every accepted syntax fixture is compiled by the exact historical smcs.
    emission = []
    for (method, rule), patch in chosen.items():
        methods = compile_toy(apply(SOURCE, patch), 'candidate-' + rule + '-' + method.split('(')[0].replace(' ', '-'))
        emission.append({'fixture': method + ':' + rule, 'compile_exit': 0})
        target = next(m for k,m in methods.items() if k.startswith('Fixture::' + method.split('(')[0].split()[-1] + '('))
        ops = [i['opcode'] for i in target['instructions']]
        if rule == 'tested-value-local' and method == 'int AnyType(':
            assert any(ops[i].startswith('stloc') and ops[i+1].startswith('ldloc') for i in range(len(ops)-1)), ops
        if rule == 'new-object-local-before-field':
            pos=ops.index('newobj')
            assert ops[pos+1].startswith('stloc') and ops[pos+2]=='ldarg.0', ops
        if rule == 'unified-result-return-if-else':
            assert ops.count('ret')==1 and ops[-2].startswith('ldloc'), ops
        if rule == 'extended-original-zero-chain':
            expected=3 if method=='void Zero(' else 2
            assert ops.count('dup')>=expected, ops
    report = {'syntax_fixture_count':len(checks), 'historical_smcs_compile_count':len(emission),
              'scope':'isolated toy fixtures only; game-method acceptance is UNKNOWN until official verifier',
              'checks':checks, 'emission_compiles':emission}
    (ROOT / 'reports/round7-smcs-rule-fixtures.json').write_text(json.dumps(report, indent=2) + '\n', encoding='utf-8')
    print(json.dumps({k:report[k] for k in ('syntax_fixture_count','historical_smcs_compile_count')}))


if __name__=='__main__':
    main()
