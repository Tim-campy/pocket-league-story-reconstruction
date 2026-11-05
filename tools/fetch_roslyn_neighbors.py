"""Acquire official portable Roslyn 1.x neighbors, recording exact identities.

The feature probe compiles only our own trivial fixture, never the target game.
No global installation and no method ledger changes are performed.
"""
import hashlib
import json
from pathlib import Path
import re
import subprocess
from urllib.request import urlopen
import zipfile
from compiler_probes import UNITY

ROOT = Path(__file__).resolve().parents[1]
INDEX = 'https://api.nuget.org/v3-flatcontainer/microsoft.net.compilers/index.json'
VERSIONS = ('1.0.0', '1.1.0', '1.2.0')
PINNED = {
    '1.0.0': ('f13ec2307bf626aa23ca5aaa74c47775270b5747c5d3a2258beb51567fa586b8',
              '8d756ad8f37c62dc3a807d5a8ca1d77c5e0d5c499762533277e53867c9c0a8f5'),
    '1.1.0': ('cd84fb2c71cafe03cdae0add609fa12c75f61c89ea87b1422fd108ac7e570745',
              '53b693536e8a86f35d05d2165ff9b3e0aeaa499ecfd8135a9f99ab4404ca86c3'),
    '1.2.0': ('e8aaac4dc7d64b0a76dc425dc9e157d3335d9d52c2cb093069dbc37d148cade1',
              '3f3aa653118cc40b8f322b06f31207c6cd4d80c8e0f133bdb7d985f35d722e79'),
}


def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def main():
    with urlopen(INDEX, timeout=60) as response:
        index = json.load(response)
    results = []
    for version in VERSIONS:
        if version not in index['versions']:
            raise ValueError('Official package absent: ' + version)
        url = INDEX.rsplit('/', 1)[0] + '/' + version + '/microsoft.net.compilers.' + version + '.nupkg'
        package = ROOT / ('analysis/downloads/microsoft.net.compilers.' + version + '.nupkg')
        package.parent.mkdir(parents=True, exist_ok=True)
        if not package.exists():
            with urlopen(url, timeout=60) as response:
                package.write_bytes(response.read())
        if sha(package) != PINNED[version][0]:
            raise ValueError('Pinned package hash mismatch: ' + version)
        folder = ROOT / ('analysis/tools/roslyn-' + version)
        with zipfile.ZipFile(package) as archive:
            for member in archive.infolist():
                target = (folder / member.filename).resolve()
                if not target.is_relative_to(folder.resolve()):
                    raise ValueError('Unsafe archive path')
            archive.extractall(folder)
        compiler = folder / 'tools/csc.exe'
        if sha(compiler) != PINNED[version][1]:
            raise ValueError('Pinned compiler hash mismatch: ' + version)
        version_result = subprocess.run([str(compiler), '/version'], capture_output=True, text=True)
        help_result = subprocess.run([str(compiler), '/help'], capture_output=True, text=True)
        probe = ROOT / ('build/toolchain-neighbor-probes/' + version)
        probe.mkdir(parents=True, exist_ok=True)
        source = probe / 'FeatureProbe.cs'
        source.write_text('public class ToolchainFeatureProbe { public int Value() { return 7; } }\n', encoding='utf-8')
        flags = ['/nologo', '/noconfig', '/nostdlib+', '/target:library', '/platform:anycpu',
                 '/optimize+', '/debug:pdbonly', '/deterministic-', '/langversion:6',
                 '/unsafe+', '/checked-', '/codepage:65001', '/utf8output']
        args = [str(compiler)] + flags + ['/out:' + str(probe / 'FeatureProbe.dll'),
                '/reference:' + str(UNITY / 'Editor/Data/Mono/lib/mono/2.0/mscorlib.dll'), str(source)]
        compiled = subprocess.run(args, capture_output=True, text=True)
        text = compiled.stdout + compiled.stderr
        (probe / 'compiler.log').write_text(text, encoding='utf-8')
        (probe / 'help.txt').write_text(help_result.stdout + help_result.stderr, encoding='utf-8')
        item = {'package': 'Microsoft.Net.Compilers', 'package_version': version,
                'official_index_url': INDEX, 'official_index_contains_version': True,
                'url': url, 'package_bytes': package.stat().st_size, 'package_sha256': sha(package),
                'compiler': str(compiler.relative_to(ROOT)), 'compiler_sha256': sha(compiler),
                'version_exit_code': version_result.returncode,
                'compiler_version': re.search(r'Compiler version ([^\r\n]+)', version_result.stdout + version_result.stderr).group(1),
                'compiler_version_output': (version_result.stdout + version_result.stderr).strip(),
                'probe_flags': flags, 'probe_exit_code': compiled.returncode,
                'probe_output': text.strip(), 'target_assembly_compiled': False}
        if version == '1.0.0' and "Unrecognized option: '/deterministic-'" in text:
            retry_args = [arg for arg in args if arg != '/deterministic-']
            retry = subprocess.run(retry_args, capture_output=True, text=True)
            retry_text = retry.stdout + retry.stderr
            (probe / 'compiler-without-unsupported-flag.log').write_text(retry_text, encoding='utf-8')
            item['compatible_probe'] = {'omitted_unsupported_flag': '/deterministic-',
                                        'exit_code': retry.returncode, 'output': retry_text.strip(),
                                        'flags': [flag for flag in flags if flag != '/deterministic-']}
        results.append(item)
        print(json.dumps(item), flush=True)
    report = {'schema_version': 1, 'packages': results,
              'limitations': ['Package hashes are recorded from official HTTPS acquisition; historical identity remains UNKNOWN.',
                              'Only an own-source option probe has been compiled, not KairoLibrary.']}
    (ROOT / 'reports/Roslyn-neighbors-acquisition.json').write_text(json.dumps(report, indent=2) + '\n', encoding='utf-8')


if __name__ == '__main__':
    main()
