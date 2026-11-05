"""Apply proved initializer/scope/guard candidates, one target per full verify.

Rebase source members onto the current files. Never overwrite unrelated methods
with frozen snapshots. Preserve every previously equal body, including held
generated members; rollback immediately if the official verifier disagrees.
"""
import json
from pathlib import Path
import sys
from compare_kairo import sha
from evaluate_kairo_variants import parse_sources,selected_sources,method_index
from select_kairo_variants import replacement,run_official
from recover_kairo_initializers import prepare,replace_initializer
from recover_kairo_empty_initializers import PLANS
from probe_kairo_closure_guards import PATCHES

ROOT=Path(__file__).resolve().parents[1]


def read(path):return json.loads(path.read_text(encoding='utf-8-sig'))


def main():
    oracle=read(ROOT/'build/KairoLibrary-roslyn-1.3.2/original-methods.json')['selected_methods']
    reference=read(ROOT/'config/method-reference.json')
    id_of={m['stable_id']:next(key for key,ref in reference.items() if key.startswith('KairoLibrary::') and ref['token']==
        '0x'+format(0x06000000|int(m['token'].split('0x')[1].rstrip(']'),16),'08x')) for m in oracle}
    journal={'attempts':[]};scratch=ROOT/'analysis/manual-kairo-recovery';scratch.mkdir(exist_ok=True)

    def apply(key,logical,edited,kind):
        receipt=read(ROOT/'build/verification-KairoLibrary.json');target=id_of[key]
        if receipt['methods'][target]['state']=='matched':return
        path=ROOT/logical;before=path.read_bytes();previous_equal={k for k,v in receipt['methods'].items() if v['body_equal']}
        record={'target':key,'source':logical,'kind':kind,'before_sha256':sha(path)}
        path.write_bytes(edited)
        try:
            verified=run_official()
            lost=[k for k in previous_equal if not verified['methods'][k]['body_equal']]
            if lost or verified['regressions'] or verified['methods'][target]['state']!='matched':
                raise ValueError('Candidate failed target/body/state regression checks')
        except BaseException:
            path.write_bytes(before);run_official();raise
        record.update({'accepted':True,'matched_before':receipt['counts']['matched'],
            'matched_after':verified['counts']['matched'],'previous_equal_bodies_lost':[],
            'after_sha256':sha(path),'target_result':verified['methods'][target]})
        journal['attempts'].append(record)
        (ROOT/'reports/KairoLibrary-manual-recovery.json').write_text(json.dumps(journal,indent=2)+'\n',encoding='utf-8')
        print(json.dumps({'target':key,'matched':verified['counts']['matched'],'accepted':True}),flush=True)

    # These files are freshly generated from current main, rather than copied
    # from earlier whole-file experiments with other methods at older versions.
    mapping,inputs=prepare()
    for logical,candidate in mapping.items():
        kind='Config' if logical.endswith('/Config.cs') else 'AdMobPlugin'
        owner='kairo.common.cfg.Config' if kind=='Config' else 'kairo.unity.native.AdMobPlugin'
        expected=next(item['base_sha256'] for item in inputs if item['logical_path']==logical)
        if sha(ROOT/logical)!=expected:raise ValueError('Initializer base changed')
        apply(owner+'::.cctor()->System.Void',logical,Path(candidate).read_bytes(),'static-field-initializers')
    for owner,logical,field in PLANS:
        text=(ROOT/logical).read_bytes().decode('utf-8-sig')
        edited=replace_initializer(text,field,'null').encode('utf-8')
        apply(owner+'::.cctor()->System.Void',logical,edited,'default-static-field-initializer')

    def apply_member(key,logical,candidate_path):
        if read(ROOT/'build/verification-KairoLibrary.json')['methods'][id_of[key]]['state']=='matched':return
        main_selected=selected_sources({});main_doc=parse_sources(main_selected,scratch,'main')
        candidates=selected_sources({logical:str(candidate_path)});candidate_doc=parse_sources(candidates,scratch,'candidate')
        main_index=method_index(main_doc,{str(p):k for k,p in main_selected.items()})
        other_index=method_index(candidate_doc,{str(p):k for k,p in candidates.items()})
        current=main_index[key];other=other_index[key]
        main_text=next(f['text'] for f in main_doc['sources'] if f['path']==str(ROOT/logical))
        candidate_text=next(f['text'] for f in candidate_doc['sources'] if f['path']==str(candidate_path))
        if (ROOT/logical).read_bytes().decode('utf-8-sig')!=main_text:raise ValueError('Main changed since semantic snapshot')
        edited=replacement(main_text,current['member'],candidate_text,other['member']).encode('utf-8')
        apply(key,logical,edited,'one-CSharp-member-body')

    locks=read(ROOT/'reports/KairoLibrary-lock-scope-probes.json')['results']
    for key,item in locks.items():
        if not item['body_equal']:continue
        apply_member(key,item['logical_source'],ROOT/item['candidate_source'])
    concatenation=read(ROOT/'reports/KairoLibrary-concat-recovery.json')['trials']
    for item in concatenation:
        if not item['body_equal']:continue
        key=item['id'];owner=key.split('::',1)[0]
        # The independent candidate has the original namespace/type path.
        logical=next(row['path'] for row in read(ROOT/'build/KairoLibrary-roslyn-1.3.2/build-manifest.json')['sources']
            if row['path'].endswith('/'+owner.split('.')[-1]+'.cs'))
        apply_member(key,logical,ROOT/item['candidate_source'])
    for name,(needle,substitute) in PATCHES.items():
        logical='src/KairoLibrary/kairo/unity/ui/Image.cs';text=(ROOT/logical).read_bytes().decode('utf-8-sig')
        if text.count(needle)!=1:raise ValueError('Image guard shape changed')
        if '\r\n' in text:substitute=substitute.replace('\n','\r\n')
        apply('kairo.unity.ui.Image::'+name+'()->System.Void',logical,text.replace(needle,substitute).encode('utf-8'),'separate-early-guards')
    journal['final_counts']=read(ROOT/'build/verification-KairoLibrary.json')['counts']
    (ROOT/'reports/KairoLibrary-manual-recovery.json').write_text(json.dumps(journal,indent=2)+'\n',encoding='utf-8')


if __name__=='__main__':main()
