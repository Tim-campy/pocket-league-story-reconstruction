"""Dispatch/repertoire controls, including the rejected loop-to-return hypothesis."""
import json
from pathlib import Path
from round7_transform import TransformClient

ROOT=Path(__file__).resolve().parents[1]
SOURCE='''class Fixture {
 static int count;
 static void Tail(bool flag) {if(flag) {count++;}}
 static void Loop(bool flag) {for(int i=0;i<3;i++) {if(flag) {count++;}}}
 static void LoopGuard(bool flag) {for(int i=0;i<3;i++) {if(flag) {return;} count++;}}
 static void Guard(bool flag) {if(flag) {return;} count++;}
 static void Empty(bool flag) {if(!flag) {} count++;}
 static void EmptyBranch(int x) {if(x!=3) {count++;}}
 static void EmptyAnd(int x,int y) {if(x!=3 && y!=7) {count++;}}
 static string Concat(string a,string b) {return a+"\\t"+" "+b;}
 static void LoopCondition(bool a,bool b) {while(a && b) {count++;}}
 static void SwitchOwned(int x) {switch(x) {case 0: {count++;break;} case 1: count--;break;}}
 static void ForDeclaration() {int sum=0;for(int i=0;i<3;i++) {sum+=i;}count=sum;}
}'''


def main():
    folder=ROOT/'build/round7/cycle-dispatch';folder.mkdir(parents=True,exist_ok=True)
    path=folder/'Fixture.cs';path.write_text(SOURCE,encoding='utf-8')
    manifest=folder/'manifest.json';manifest.write_text(json.dumps({'sources':[str(path)],
        'references':[str(ROOT/'original/extracted/assets/bin/Data/Managed/mscorlib.dll')]}))
    checks=[]
    with TransformClient(manifest) as client:
        def rules(method,**hints):
            point=SOURCE.index('if(',SOURCE.index('void '+method+'('))
            r=client.request(path,source=SOURCE,offset=point,compiler='smcs',max_candidates=2048,**hints)
            assert not r.get('error'),r
            return {p['rule'] for p in r['patches']}
        assert 'tail-early-return' in rules('Tail');checks.append('legacy terminal tail remains available')
        loop=rules('Loop');assert 'tail-early-return' not in loop;checks.append('loop tail cannot return from method')
        assert 'loop-continue-guard' in loop;checks.append('loop continue alternative retained')
        assert 'terminal-guard-positive-tail' not in rules('Guard');checks.append('cycle requires activation')
        assert 'terminal-guard-positive-tail' in rules('Guard',round7_cycle_rule='terminal-guard-positive-tail');checks.append('selected rule dispatch')
        assert 'empty-if-terminal-polarity' in rules('Empty',round7_cycle_rule='terminal-guard-positive-tail',
            round7_cycle_rules=['empty-if-terminal-polarity','terminal-guard-positive-tail']);checks.append('learned family remains in repertoire')
        assert 'void-empty-then-common-exit' in rules('Guard',round7_cycle_rule='void-empty-then-common-exit',
            original_exit_target_kind='method-ret');checks.append('common method exit dispatch')
        assert 'void-empty-then-common-exit' not in rules('Guard',round7_cycle_rule='void-empty-then-common-exit');checks.append('method exit requires original branch evidence')
        assert 'loop-guard-return-to-continue' in rules('LoopGuard',round7_cycle_rule='loop-guard-return-to-continue',
            original_exit_target_kind='loop-increment');checks.append('original loop increment dispatch')
        assert 'loop-guard-return-to-continue' not in rules('LoopGuard',round7_cycle_rule='loop-guard-return-to-continue',
            original_exit_target_kind='UNKNOWN');checks.append('unknown branch destination rejected')
        assert 'smcs-empty-arm-restore' in rules('EmptyBranch',round7_cycle_rule='smcs-empty-arm-restore');checks.append('empty arm dispatch')
        variants=rules('EmptyAnd',round7_cycle_rule='smcs-empty-arm-restore')
        assert 'smcs-empty-arm-restore-first-and' in variants and 'smcs-empty-arm-restore-and-suffix' in variants
        assert all(v.startswith('smcs-empty-arm-restore') for v in variants if v.startswith('smcs-empty-arm'))
        checks.append('all empty-arm variants survive selected-family filter')
        point=SOURCE.index('return a+',SOURCE.index('static string Concat('))
        concat=client.request(path,source=SOURCE,offset=point,compiler='smcs',max_candidates=2048,
            round7_cycle_rule='explicit-string-concat-preserve-literals',original_string_concat_arity=4)
        assert any(p['rule']=='explicit-string-concat-preserve-literals' for p in concat['patches'])
        checks.append('explicit Concat4 with original arity dispatch')
        point=SOURCE.index('while(',SOURCE.index('void LoopCondition('))
        loop_condition=client.request(path,source=SOURCE,offset=point,compiler='smcs',max_candidates=2048,
            round7_cycle_rule='smcs-loop-condition-body-guard')
        assert any(p['rule']=='smcs-loop-condition-body-guard' for p in loop_condition['patches'])
        checks.append('loop condition split dispatch')
        point=SOURCE.index('switch(',SOURCE.index('void SwitchOwned('))
        switch_owned=client.request(path,source=SOURCE,offset=point,compiler='smcs',max_candidates=2048,
            round7_cycle_rule='smcs-owned-switch-if')
        assert any(p['rule']=='smcs-owned-switch-if' for p in switch_owned['patches'])
        checks.append('switch break ownership dispatch')
        point=SOURCE.index('for(',SOURCE.index('void ForDeclaration('))
        for_decl=client.request(path,source=SOURCE,offset=point,compiler='roslyn',max_candidates=2048,
            round7_cycle_rule='roslyn-for-declaration-immediately-before')
        assert any(p['rule']=='roslyn-for-declaration-immediately-before' for p in for_decl['patches'])
        checks.append('Roslyn immediate for declaration dispatch')
    (ROOT/'reports/round7-cycle-dispatch-tests.json').write_text(json.dumps({'passed':len(checks),'checks':checks,
        'game_candidate_compilations':0},indent=2)+'\n',encoding='utf-8',newline='\n')
    print(json.dumps({'passed':len(checks)}))


if __name__=='__main__':main()
