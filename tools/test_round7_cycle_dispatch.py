"""Dispatch/repertoire controls, including the rejected loop-to-return hypothesis."""
import json
from pathlib import Path
from round7_transform import TransformClient

ROOT=Path(__file__).resolve().parents[1]
SOURCE='''class Fixture {
 static int count;
 static void Tail(bool flag) {if(flag) {count++;}}
 static void Loop(bool flag) {for(int i=0;i<3;i++) {if(flag) {count++;}}}
 static void Guard(bool flag) {if(flag) {return;} count++;}
 static void Empty(bool flag) {if(!flag) {} count++;}
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
    (ROOT/'reports/round7-cycle-dispatch-tests.json').write_text(json.dumps({'passed':len(checks),'checks':checks,
        'game_candidate_compilations':0},indent=2)+'\n',encoding='utf-8',newline='\n')
    print(json.dumps({'passed':len(checks)}))


if __name__=='__main__':main()
