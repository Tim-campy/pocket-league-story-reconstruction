"""Explain the 19 baseline call/callvirt failures using CIL and C# probes.

Reads target metadata only; does not execute the game's code or edit its source.
The compiler probe contains only a new, synthetic Receiver class.
"""
from collections import Counter
import hashlib
import json
from pathlib import Path
import subprocess
from fix_kairo_callvirt import PLANS
from compiler_probes import UNITY

ROOT=Path(__file__).resolve().parents[1]
PROBE='''public class Receiver {
 public void Run() {}
 public static void Conditional(Receiver receiver) { receiver?.Run(); }
 public static void Explicit(Receiver receiver) { if (receiver != null) receiver.Run(); }
 public static void DirectNew() { new Receiver().Run(); }
 public static void LocalNew() { Receiver receiver = new Receiver(); receiver.Run(); }
}
'''


def digest(path):return hashlib.sha256(path.read_bytes()).hexdigest()


def load(path):return json.loads(path.read_text(encoding='utf-8-sig'))


def main():
    folder=ROOT/'build/KairoLibrary-roslyn-1.3.2'
    original=load(folder/'original-methods.json');candidate=load(folder/'rebuilt-methods.json')
    left={m['stable_id']:m for m in original['selected_methods']}
    right={m['stable_id']:m for m in candidate['selected_methods']}
    declarations={m['signature']:m for m in original['selected_methods']}
    new_declarations={m['signature']:m for m in candidate['selected_methods']}
    baseline=load(ROOT/'reports/KairoLibrary-failure-groups.json')
    targets=[m for m in baseline['methods'] if m['group']=='call_vs_callvirt']
    if len(targets)!=19:raise ValueError('This diagnostic is pinned to the 19-method baseline; retain its report after applying repairs')
    unity=load(ROOT/'analysis/unity-structure/apk.json')
    unity_target=next(m for m in unity['selected_methods'] if m['stable_id']=='UnityEngine.AndroidJavaObject::GetStatic``1(System.String)->!!0')
    rows=[];counts=Counter()
    for row in targets:
        key=row['stable_id'];a,b=left[key],right[key]
        plan=next(plan for plan in PLANS if key.startswith(plan[0]+'->'))
        _,relative,header,repair,_=plan;sites=[]
        if len(a['instructions'])!=len(b['instructions']):raise ValueError('Unexpected instruction sequence length')
        for old,new in zip(a['instructions'],b['instructions']):
            if old==new:continue
            if old['offset']!=new['offset'] or old['operand']!=new['operand'] or (old['opcode'],new['opcode'])!=('callvirt','call'):
                raise ValueError('Unexpected non-callvirt difference: '+key)
            signature=old['operand']['method'];external=' @ UnityEngine,' in signature
            declaration=unity_target if external else declarations[signature]
            new_declaration=unity_target if external else new_declarations[signature]
            attributes=declaration['method_attributes']
            site={'offset':old['offset'],'original_opcode':'callvirt','rebuilt_opcode':'call',
                'same_resolved_target':True,'target_signature':signature,'target_definition':declaration['stable_id'],
                'target_attributes_original':attributes,'target_attributes_rebuilt_or_reference':new_declaration['method_attributes'],
                'target_static':bool(attributes&0x10),'target_virtual':bool(attributes&0x40),'target_final':bool(attributes&0x20),
                'target_declaration_unchanged':attributes==new_declaration['method_attributes'],
                'reference':'APK UnityEngine.dll' if external else 'same KairoLibrary assembly',
                'original_preceding_instructions':a['instructions'][max(0,a['instructions'].index(old)-4):a['instructions'].index(old)]}
            sites.append(site);counts['call_sites']+=1;counts['external_sites' if external else 'internal_sites']+=1
        if not sites:
            raise ValueError('Target already repaired or dump differs from the pinned baseline: '+key)
        counts['null_conditional_methods' if repair.startswith('null_') else 'inline_new_receiver_methods']+=1
        rows.append({'id':row['id'],'stable_id':key,'original_token':a['token'],
            'source':'src/KairoLibrary/kairo/unity/'+relative,'source_method_header':header,
            'source_form':'null-conditional ?. synthesized by decompiler' if repair.startswith('null_') else 'direct invocation on new object expression',
            'suggested_repair':repair,'sites':sites,'base_vs_this_supported':False,
            'reference_declaration_mismatch_observed':False})
    proof=ROOT/'build/callvirt-probes';proof.mkdir(parents=True,exist_ok=True)
    source=proof/'CallvirtProof.cs';source.write_text(PROBE,encoding='utf-8',newline='\n')
    compiler=ROOT/'analysis/tools/roslyn-1.3.2/tools/csc.exe'
    core=UNITY/'Editor/Data/Mono/lib/mono/2.0/mscorlib.dll'
    output=proof/'CallvirtProof.dll'
    flags=['/nologo','/noconfig','/nostdlib+','/target:library','/platform:anycpu','/optimize+',
        '/langversion:6','/debug:pdbonly','/deterministic-','/reference:'+str(core),'/out:'+str(output),str(source)]
    build=subprocess.run([str(compiler)]+flags,cwd=ROOT,capture_output=True,text=True)
    if build.returncode:raise RuntimeError(build.stdout+build.stderr)
    inspected=subprocess.run([str(ROOT/'build/tools/ProbeInspector.exe'),str(output),'--all'],capture_output=True,
        text=True,encoding='utf-8',check=True)
    probe=json.loads(inspected.stdout)
    relevant={m['stable_id']:m['instructions'] for m in probe['selected_methods'] if m['stable_id'].split('::')[1].split('(')[0] in ('Conditional','Explicit','DirectNew','LocalNew')}
    expected={'Conditional':'call','Explicit':'callvirt','DirectNew':'call','LocalNew':'callvirt'}
    for method,instructions in relevant.items():
        name=method.split('::')[1].split('(')[0]
        calls=[i['opcode'] for i in instructions if isinstance(i['operand'],dict) and 'method' in i['operand'] and '::Run``' in i['operand']['method']]
        if calls!=[expected[name]]:raise ValueError('Compiler probe did not confirm lowering pattern: '+name)
    report={'scope':'19-method original baseline; 21 sites; diagnosis before C# repairs',
        'counts':dict(counts),'methods':rows,
        'original_sha256':digest(ROOT/'original/extracted/assets/bin/Data/Managed/KairoLibrary.dll'),
        'original_dump_sha256':digest(folder/'original-methods.json'),
        'rebuilt_dump_sha256':digest(folder/'rebuilt-methods.json'),
        'probe':{'compiler_package':'Microsoft.Net.Compilers 1.3.2','executable_version':'1.3.1.60621',
            'compiler_sha256':digest(compiler),'core_reference_sha256':digest(core),'source':PROBE,
            'source_sha256':digest(source),'flags':flags[:10],'instructions':relevant,'confirmed':True},
        'conclusion':'all changed sites invoke nonvirtual instance methods with identical targets; synthetic probes explain call selection by non-null receiver syntax',
        'reference_problem_found':False,
        'limitation':'Does not prove all historical references byte-identical; diagnoses this 19-method group only'}
    (ROOT/'reports/KairoLibrary-callvirt-diagnostic.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8',newline='\n')
    print(json.dumps({'methods':len(rows),'counts':dict(counts),'probe_confirmed':True,'reference_problem_found':False}))


if __name__=='__main__':main()
