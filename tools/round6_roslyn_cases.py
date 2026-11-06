"""Read-only Roslyn 1.1 slot diagnoses and uncompiled source patch proposals."""
from collections import Counter
import difflib
import hashlib
import json
from pathlib import Path
import shutil
import subprocess

from compare_kairo import differences, sha
from compiler_probes import ROSLYN
from round5_local_cases import declaration, local_operation
from round5_transform import TransformClient

ROOT = Path(__file__).resolve().parents[1]
BASE = ROOT / 'build/round6/baseline/KairoLibrary'
WORK = ROOT / 'build/round6/roslyn'
COMMIT = '4cb203dab89e1542cdf7d88cb6d32767bc0e5460'
SOURCE = ROOT / 'analysis/tools/round6-roslyn-source'
CASE_IDS = {
    'Canvas.EndPaint': 'kairo.unity.ui.Canvas::EndPaint(kairo.unity.ui.Graphics,System.Int32)->System.Void',
    'SurfaceManagerBase.Draw': 'kairo.unity.surface.SurfaceManagerBase::Draw(kairo.unity.ui.Graphics)->System.Void',
    'Matrix.PostConcat': 'kairo.unity.ui.Matrix::PostConcat(System.Single[])->kairo.unity.ui.Matrix',
    'Graphics.DrawString': 'kairo.unity.ui.Graphics::DrawString(kairo.unity.ui.TextFormat,System.String,System.Single,System.Single,System.Int32)->System.Void',
}


def read(path):
    return json.loads(Path(path).read_text(encoding='utf-8-sig'))


def write(path, value):
    Path(path).write_text(json.dumps(value, indent=2, ensure_ascii=False) + '\n', encoding='utf-8', newline='\n')


def syntax_inventory(manifest):
    vs = next(p for p in ROSLYN.parents if (p / 'dotnet').is_dir())
    runtime = vs / 'dotnet/net10.0/runtime'
    framework = runtime / 'shared/Microsoft.NETCore.App/10.0.10'
    source = ROOT / 'tools/Round6RoslynSyntax.cs'
    folder = WORK / 'syntax' / sha(source)[:16]
    folder.mkdir(parents=True, exist_ok=True)
    output = folder / 'Round6RoslynSyntax.dll'
    libs = ['System.Private.CoreLib', 'System.Runtime', 'System.Console', 'System.Linq',
            'System.Collections', 'System.Text.Json', 'System.Collections.Immutable', 'netstandard']
    refs = [framework / (n + '.dll') for n in libs]
    refs += [ROSLYN.parent / (n + '.dll') for n in ['Microsoft.CodeAnalysis', 'Microsoft.CodeAnalysis.CSharp']]
    if not output.exists():
        args = ['/nologo', '/nostdlib+', '/target:exe', '/out:' + str(output)]
        args += ['/r:' + str(p) for p in refs] + [str(source)]
        rsp = folder / 'compile.rsp'
        rsp.write_text('\n'.join('"' + a + '"' for a in args) + '\n', encoding='utf-8', newline='\n')
        subprocess.run([str(ROSLYN), '/noconfig', '@' + str(rsp)], check=True)
        for p in refs[-2:]:
            shutil.copyfile(p, folder / p.name)
        write(folder / 'Round6RoslynSyntax.runtimeconfig.json', {'runtimeOptions': {
            'tfm': 'net10.0', 'framework': {'name': 'Microsoft.NETCore.App', 'version': '10.0.10'}}})
    paths = [str(BASE / s['path']) for s in manifest['sources']]
    run = subprocess.run([str(runtime / 'dotnet.exe'), str(output)] + paths, capture_output=True, check=True)
    return read_bytes_json(run.stdout)


def read_bytes_json(data):
    return json.loads(data.decode('utf-8-sig'))


def alignment_shape(instruction):
    operation = local_operation(instruction)
    if operation:
        return operation[0]
    operand = instruction['operand']
    if isinstance(operand, dict) and ('branch' in operand or 'branches' in operand):
        operand = '<LABEL>'
    return instruction['opcode'] + ' ' + json.dumps(operand, sort_keys=True)


def local_mapping(original, rebuilt):
    """Diagnostic alignment only; never changes either IL body or a state."""
    left, right = original['instructions'], rebuilt['instructions']
    matcher = difflib.SequenceMatcher(a=[alignment_shape(i) for i in left],
                                     b=[alignment_shape(i) for i in right], autojunk=False)
    counts = Counter()
    for i, j, size in matcher.get_matching_blocks():
        for a, b in zip(left[i:i + size], right[j:j + size]):
            x, y = local_operation(a), local_operation(b)
            if x and y:
                counts[x[1], y[1]] += 1
    return [{'original_slot': a, 'rebuilt_slot': b, 'aligned_accesses': count}
            for (a, b), count in sorted(counts.items())]


def accesses(method, names=None):
    result = []
    for slot, kind in enumerate(method['locals']):
        events = [i for i in method['instructions'] if local_operation(i) and local_operation(i)[1] == slot]
        row = {'slot': slot, 'type': kind, 'access_count': len(events),
               'first_offset': events[0]['offset'] if events else None,
               'last_offset': events[-1]['offset'] if events else None,
               'accesses': [{'offset': i['offset'], 'operation': local_operation(i)[0]} for i in events]}
        if names is not None:
            row['recompiled_pdb_name'] = names[slot]['name']
        result.append(row)
    return result


def patch_for_case(name, text):
    if name == 'Canvas.EndPaint':
        # Named array declaration in the enclosing block currently precedes the
        # first loop's child block during EmitBlock, despite a later initializer.
        old = '\t\t\tif (!g.IsRenderGUIMode())\n\t\t\t{'
        new = old + '\n\t\t\t\tint i;'
        assert text.count(old) == 1
        return text.replace(old, new, 1).replace('for (int i = 0;', 'for (i = 0;', 1)
    if name == 'SurfaceManagerBase.Draw':
        text = text.replace('bool flag = IsAutoGLRender();', 'bool flag = IsAutoGLRender();\n\t\t\tint i;', 1)
        text = text.replace('for (int i = 0;', 'for (i = 0;', 1)
        text = text.replace('int height = g.GetFont().GetHeight();', 'string text;\n\t\t\t\t\tint height = g.GetFont().GetHeight();', 1)
        return text.replace('string text = paintRect[0]', 'text = paintRect[0]', 1)
    if name == 'Matrix.PostConcat':
        text = text.replace('for (int i = 0;', 'int j, k, l;\n\t\t\tfor (int i = 0;', 1)
        for local in ('j', 'k', 'l'):
            text = text.replace('for (int ' + local + ' = 0;', 'for (' + local + ' = 0;', 1)
        return text
    if name == 'Graphics.DrawString':
        old = 'float num2;\n\t\t\t\tfloat num3;\n\t\t\t\tfloat num4;'
        new = 'float num4;\n\t\t\t\tfloat num2;\n\t\t\t\tfloat num3;'
        assert text.count(old) == 1
        return text.replace(old, new, 1)
    raise KeyError(name)


def main():
    WORK.mkdir(parents=True, exist_ok=True)
    manifest = read(BASE / 'build-manifest.json')
    manifest['sources'] = [dict(s, input_path=str(BASE / s['path'])) for s in manifest['sources']]
    write(WORK / 'syntax-manifest.json', manifest)
    original = {m['stable_id']: m for m in read(BASE / 'original-methods.json')['selected_methods']}
    rebuilt = {m['stable_id']: m for m in read(BASE / 'rebuilt-methods.json')['selected_methods']}
    symbols = {m['stable_id']: m for m in read(WORK / 'pdb-methods.json')['methods']}
    cases, patches = [], []
    with TransformClient(WORK / 'syntax-manifest.json') as client:
        for name, key in CASE_IDS.items():
            logical = next(s['path'] for s in manifest['sources'] if s['path'].endswith('/' + name.split('.')[0] + '.cs'))
            text = (BASE / logical).read_bytes().decode('utf-8-sig')
            located = declaration(client, logical, text, key)
            before = located['text']
            newline = '\r\n' if '\r\n' in before else '\n'
            after = patch_for_case(name, before.replace('\r\n', '\n')).replace('\n', newline)
            assert before != after
            old, new, named = original[key], rebuilt[key], symbols[key]
            assert not differences(new, named)[0], 'PDB body differs from frozen production body'
            mapping = local_mapping(old, new)
            cases.append({'name': name, 'method_id': key, 'logical_path': logical,
                          'original_il_bytes': old['header']['code_size'], 'rebuilt_il_bytes': new['header']['code_size'],
                          'original_local_count': len(old['locals']), 'rebuilt_local_count': len(new['locals']),
                          'additional_local_count': 0, 'missing_local_count': 0,
                          'local_count_equal': len(old['locals']) == len(new['locals']),
                          'original_slots': accesses(old), 'rebuilt_slots': accesses(new, named['locals_debug']),
                          'aligned_slot_mapping': mapping, 'mapping_status': 'DIAGNOSTIC_ALIGNMENT_AND_PDB_NAMES',
                          'symbol_body_matches_frozen_production': True, 'source_declaration': before,
                          'candidate_status': 'UNCOMPILED_SOURCE_HYPOTHESIS'})
            patches.append({'method_id': key, 'logical_path': logical, 'path': logical,
                            'rule': 'roslyn-declaration-slot-layout', 'start': located['start_utf16'],
                            'end': located['end_utf16'], 'before': before, 'after': after,
                            'expected_local_signature': old['locals'], 'expected_slot_map': mapping,
                            'status': 'UNCOMPILED_SOURCE_HYPOTHESIS'})
        for key in ['kairo.unity.util.Language::Init()->System.Void',
                    'kairo.unity.util.Language::SetLanguagePack(System.String,System.Byte[],System.Boolean)->System.Boolean']:
            logical = 'src/KairoLibrary/kairo/unity/util/Language.cs'
            text = (BASE / logical).read_bytes().decode('utf-8-sig')
            located = declaration(client, logical, text, key)
            before = located['text']
            assert before.count('jarInflater?.Close();') == 1
            after = before.replace('jarInflater?.Close();', 'if (jarInflater != null)\n\t\t\t\t{\n\t\t\t\t\tjarInflater.Close();\n\t\t\t\t}', 1)
            patches.append({'method_id': key, 'logical_path': logical, 'path': logical,
                            'rule': 'roslyn-conditional-access-explicit-null-guard', 'start': located['start_utf16'],
                            'end': located['end_utf16'], 'before': before, 'after': after,
                            'expected_local_signature': original[key]['locals'],
                            'status': 'UNCOMPILED_SOURCE_HYPOTHESIS',
                            'single_evaluation_evidence': 'Receiver is uncaptured non-volatile local jarInflater; plain repeated local loads preserve receiver.'})
    inventory = syntax_inventory(manifest)
    for row in inventory:
        row['path'] = str(Path(row['path']).relative_to(BASE)).replace('\\', '/')
    sources = [{'path': str(p.relative_to(SOURCE)).replace('\\', '/'), 'sha256': sha(p),
                'url': 'https://github.com/dotnet/roslyn/blob/' + COMMIT + '/' + str(p.relative_to(SOURCE)).replace('\\', '/')}
               for p in sorted((SOURCE / 'src').rglob('*.cs'))]
    write(ROOT / 'reports/round6-roslyn-slots.json', {'operation': 'read-only-source-PDB-IL-slot-diagnosis',
          'source_tag': 'version-1.1.0', 'source_commit': COMMIT, 'source_snapshot_exact_historical_build': 'UNKNOWN',
          'compiler_sha256': manifest['compiler_sha256'], 'original_sha256': sha(ROOT / 'original/extracted/assets/bin/Data/Managed/KairoLibrary.dll'),
          'sources': sources, 'cases': cases, 'game_sources_modified': False, 'ledger_modified': False})
    write(ROOT / 'reports/round6-roslyn-patches.json', {'operation': 'uncompiled-source-proposals',
          'baseline_commit': '66eea9ddeb8e02a9f357f97f8e7c9dc60f54d6e9', 'patches': patches, 'states_promoted': False})
    write(ROOT / 'reports/round6-roslyn-null-operators.json', {'operation': 'CSharp-AST-null-operator-inventory',
          'source_files': len(manifest['sources']), 'counts': dict(Counter(row['kind'] for row in inventory)),
          'occurrences': inventory, 'candidate_compiles': 0, 'states_promoted': False})
    print(json.dumps({'slot_cases': len(cases), 'uncompiled_patches': len(patches),
                      'null_operator_counts': dict(Counter(row['kind'] for row in inventory))}))


if __name__ == '__main__':
    main()
