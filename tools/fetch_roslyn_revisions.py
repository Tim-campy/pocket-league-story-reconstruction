"""Acquire the requested official portable Roslyn revisions, never install globally."""
import hashlib,json,re,subprocess,zipfile
from pathlib import Path
from urllib.request import urlopen

ROOT=Path(__file__).resolve().parents[1]
VERSIONS=('1.1.1','1.2.1','1.2.2')
INDEX='https://api.nuget.org/v3-flatcontainer/microsoft.net.compilers/index.json'
COMPONENTS=('csc.exe','csc.exe.config','Microsoft.CodeAnalysis.dll','Microsoft.CodeAnalysis.CSharp.dll','System.Collections.Immutable.dll','System.Reflection.Metadata.dll')
PINNED={
    '1.1.1':('1a82b01159bf1fde80f42f871fb5b8caffd4b8545da7e524e54c4ea688c7bc0b','4161b4c4c8a1164aefdc7fe14162190385570a0e9da6439d459cab91f2fd1fd1'),
    '1.2.1':('57ed159a25e0c890af406654d15a667e074bedd6b75d16f9578c5121f70225e3','c001c9f6571a91991195221a7ed372b998d09d7c50856f09f3c2572e4f4938ec'),
    '1.2.2':('9091d74ad2c51de7a772dac0bd8901d756e3f53de2e8a154a751baf077e9745c','1a1846f6b84df0f469891996266f8d31b6c9e0dfe147a48643c34d18b71d95b1'),
}
def sha(data):return hashlib.sha256(data).hexdigest()
def main():
    with urlopen(INDEX,timeout=60) as r:index_bytes=r.read()
    index=json.loads(index_bytes);records=[]
    for version in VERSIONS:
        url=f'https://api.nuget.org/v3-flatcontainer/microsoft.net.compilers/{version}/microsoft.net.compilers.{version}.nupkg'
        record={'package':'Microsoft.Net.Compilers','package_version':version,'official_index_url':INDEX,
                'official_index_sha256':sha(index_bytes),'exists':version in index['versions'],'download_url':url,
                'gallery_url':f'https://www.nuget.org/packages/Microsoft.Net.Compilers/{version}'}
        if not record['exists']:
            record['status']='ABSENT_FROM_OFFICIAL_INDEX';records.append(record);continue
        package=ROOT/f'analysis/downloads/microsoft.net.compilers.{version}.nupkg';package.parent.mkdir(parents=True,exist_ok=True)
        if not package.exists():
            with urlopen(url,timeout=60) as r:package.write_bytes(r.read())
        data=package.read_bytes();folder=ROOT/f'analysis/tools/roslyn-{version}'
        if sha(data)!=PINNED[version][0]:raise ValueError('Package differs from pinned official acquisition')
        component_hashes={}
        with zipfile.ZipFile(package) as archive:
            for member in archive.infolist():
                target=(folder/member.filename).resolve()
                if not target.is_relative_to(folder.resolve()):raise ValueError('Unsafe NuGet archive member')
            archive.extractall(folder)
            for name in COMPONENTS:
                payload=archive.read('tools/'+name)
                if (folder/'tools'/name).read_bytes()!=payload:raise ValueError('Compiler component differs from package')
                component_hashes[name]=sha(payload)
        compiler=folder/'tools/csc.exe'
        if sha(compiler.read_bytes())!=PINNED[version][1]:raise ValueError('Compiler differs from pinned official package')
        banner=subprocess.run([str(compiler),'/help'],capture_output=True,text=True,encoding='utf-8',errors='replace')
        match=re.search(r'Compiler version ([^\r\n]+)',banner.stdout+banner.stderr)
        if not match:raise ValueError('Compiler banner version not found')
        record.update(status='ACQUIRED_PORTABLE',package_bytes=len(data),package_sha256=sha(data),
            compiler=str(compiler.relative_to(ROOT)),compiler_version=match[1],compiler_sha256=component_hashes['csc.exe'],
            components_sha256=component_hashes,help_exit_code=banner.returncode,components_equal_package=True)
        records.append(record);print(json.dumps(record),flush=True)
    out=ROOT/'reports/round3-Roslyn-revisions-acquisition.json'
    out.write_text(json.dumps({'schema_version':1,'requested_versions':VERSIONS,'packages':records,
        'absent_versions':[r['package_version'] for r in records if not r['exists']],
        'target_builds_executed':False,'global_installation':False},indent=2)+'\n',encoding='utf-8')
if __name__=='__main__':main()
