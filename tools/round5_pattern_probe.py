"""Isolated real-source tests of round-5 pattern hypotheses; no state promotion."""
import json
from pathlib import Path

from compare_kairo import differences, sha
from metadata_compare import compare_paths
from round3_metadata_candidates import compile_frozen, dump
from round4_engine import apply_patch, prefix, il_cells
from round5_transform import TransformClient

ROOT=Path(__file__).resolve().parents[1]

def read(p):return json.loads(Path(p).read_text(encoding='utf-8-sig'))

def main():
    base=ROOT/'build/round5/baseline/Assembly-CSharp'
    manifest=read(base/'build-manifest.json')
    original={m['stable_id']:m for m in read(base/'original-methods.json')['selected_methods']}
    baseline={m['stable_id']:m for m in read(base/'rebuilt-methods.json')['selected_methods']}
    equal={k for k,m in original.items() if 'raw_body_hex'in m and not differences(m,baseline.get(k))[0]}
    cases=[
      ('alias-one','src/Assembly-CSharp/main/AppData.cs','public bool CheckTermScript(int[] terms)','int num2 = terms[1];','alias-parameter-occurrence',{'expected_alias_type':'System.Int32[]'},'main.AppData::CheckTermScript(System.Int32[])->System.Boolean'),
      ('alias-tail','src/Assembly-CSharp/main/AppData.cs','public bool CheckTermScript(int[] terms)','int num2 = terms[1];','alias-parameter-block-tail',{'expected_alias_type':'System.Int32[]'},'main.AppData::CheckTermScript(System.Int32[])->System.Boolean'),
      ('draw-for','src/Assembly-CSharp/main/AppData.cs','public void DrawSeb(Graphics g, int x, int y, Seb seb, Image[] imgs','foreach (int[] array in sprites)','foreach-array-indexed-for',{},'main.AppData::DrawSeb(kairo.unity.ui.Graphics,System.Int32,System.Int32,kairo.unity.ui.Seb,kairo.unity.ui.Image[],System.Int32,System.Int32)->System.Void'),
      ('draw-foreach-direct','src/Assembly-CSharp/main/AppData.cs','public void DrawSeb(Graphics g, int x, int y, Seb seb, Image[] imgs','foreach (int[] array in sprites)','foreach-direct-array-expression',{},'main.AppData::DrawSeb(kairo.unity.ui.Graphics,System.Int32,System.Int32,kairo.unity.ui.Seb,kairo.unity.ui.Image[],System.Int32,System.Int32)->System.Void'),
      ('goal','src/Assembly-CSharp/form/DebugForm.cs','public void Draw_goal(Graphics g)','int num = (num2 = (num3 = 0));','restore-zero-chain-local',{},'form.DebugForm::Draw_goal(kairo.unity.ui.Graphics)->System.Void'),
      ('button','src/Assembly-CSharp/system/form/LanguageForm.cs','void DrawButton(','y = 20;','reserve-unused-slot-and-defer-local',{'original_unused_slots':[{'index':1,'type':'System.Int32','loads':0,'stores':0}]},None),
    ]
    records=[]
    with TransformClient(base/'build-manifest.json') as client:
      for label,logical,method_needle,needle,rule,hints,key in cases:
        source=(base/logical).read_bytes().decode('utf-8-sig')
        start=source.index(needle,source.index(method_needle))
        response=client.request(ROOT/logical,source=source,offset=len(source[:start].encode('utf-16-le'))//2,max_candidates=1024,**hints)
        if key is None:key=response['mapped']['method_id']
        patches=[p for p in response.get('patches',[]) if p['rule']==rule]
        if not patches:raise ValueError((label,response))
        patch=patches[0]
        dest=ROOT/'analysis/variants'/('round5-pattern-'+label);dest.mkdir(parents=True,exist_ok=True)
        candidate=dest/Path(logical).name;candidate.write_text(apply_patch(source,patch),encoding='utf-8',newline='')
        mapping={s['path']:str(base/s['path']) for s in manifest['sources']};mapping[logical]=str(candidate)
        compiled=compile_frozen('Assembly-CSharp','round5-pattern-'+label,manifest,mapping)
        record={'label':label,'method_id':key,'rule':rule,'compile_pass':compiled.returncode==0,'patch':patch,'game_sources_modified':False,'states_promoted':False}
        build=ROOT/'build'/('Assembly-CSharp-round5-pattern-'+label)
        if compiled.returncode==0:
          rebuilt={m['stable_id']:m for m in dump(build/'Assembly-CSharp.dll',build/'probe.json')['selected_methods']}
          after={k for k,m in original.items() if 'raw_body_hex'in m and not differences(m,rebuilt.get(k))[0]}
          meta=compare_paths(ROOT/'original/extracted/assets/bin/Data/Managed/Assembly-CSharp.dll',build/'Assembly-CSharp.dll',build/'metadata.json')
          old=prefix(original[key],baseline[key]);new=prefix(original[key],rebuilt[key])
          record.update(body_equal=key in after,old_prefix=old,new_prefix=new,prefix_preserved=il_cells(original[key])[:old]==il_cells(rebuilt[key])[:old],
                        body_gains=sorted(after-equal),body_losses=sorted(equal-after),types_equal=meta['types_equal'],types_total=meta['types_total'],
                        whole_structural_metadata_equal=meta['whole_structural_metadata_equal'],facets=differences(original[key],rebuilt[key])[0],
                        original_locals=original[key]['locals'],candidate_locals=rebuilt[key]['locals'])
        else:record['compiler_error']=(compiled.stdout+compiled.stderr).decode('utf-8',errors='replace')
        records.append(record)
        (ROOT/'reports/round5-pattern-probes.json').write_text(json.dumps({'scope':'frozen baseline source-only pattern probes','cases':records},indent=2)+'\n',encoding='utf-8',newline='\n')
        print(json.dumps({k:v for k,v in record.items() if k not in ('patch','original_locals','candidate_locals')}),flush=True)

if __name__=='__main__':main()
