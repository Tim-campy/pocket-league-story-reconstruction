# Receita final e reproduzível dos dois assemblies

**Reconstrução C# concluída:** 3.287 métodos / 269.894 bytes IL / 337 tipos em KairoLibrary; 1.958 métodos / 601.603 bytes IL / 129 tipos em Assembly-CSharp. Total: 5.245 métodos, 871.497 bytes IL e 466 tipos, todos correspondentes. Incluem-se 82 declarações sem corpo, aprovadas por metadados. Além dos métodos, todos os bytes dos PEs correspondem, exceto os seis campos abaixo. Não há normalização nem edição de DLL depois de compilar.

Este documento, seguido das tabelas de opções, referências e fontes, é a receita vigente. As instruções de preparação abaixo destinam-se à reprodução pelo leitor; **nenhuma instalação nem APK foi produzido nesta rodada de encerramento**. A compilação é feita pelos compiladores C#, sem abrir o editor Unity ou executar o jogo. O APK, assets, instaladores, referências, PDBs e DLLs não são distribuídos neste repositório.

## Critério e seis exceções comprovadas

Offsets são absolutos, base zero, em bytes, iguais nos dois arquivos. A extensão dos campos não é a quantidade de bytes que necessariamente muda em cada build.

| Assembly | Campo | Offset decimal | Tamanho |
|---|---|---:|---:|
| KairoLibrary | COFF.TimeDateStamp | 136 | 4 |
| KairoLibrary | Module.Mvid | 527096 | 16 |
| KairoLibrary | IMAGE_DEBUG_DIRECTORY.TimeDateStamp | 550520 | 4 |
| KairoLibrary | CodeView.RSDS.PdbGuid | 550548 | 16 |
| Assembly-CSharp | COFF.TimeDateStamp | 136 | 4 |
| Assembly-CSharp | Module.Mvid | 893568 | 16 |

KairoLibrary: 569.856 bytes totais, 569.816 comparados fora das exceções; Assembly-CSharp: 895.488 totais, 895.468 comparados. Total: 1.465.284 bytes literalmente iguais fora de 60 bytes de extensão. Provas de variação entre builds do mesmo fonte: `reports/round3-Kairo-repeatability.json` e `reports/round12-Assembly-CSharp-repeatability-profile.json`. O segundo revalida fisicamente o par smcs anterior; não acrescenta campos. Caminho do PDB, idade RSDS, cabeçalhos restantes, assinatura de locais, heaps, tabelas, RVAs, padding e recursos não são exceções. Não se gera hash ou DLL normalizada. Hashes integrais variam legitimamente entre builds.

## Ambiente e dependências

Ambiente medido: Windows 11 x64, PowerShell, Python **3.12.10** x64 (zlib **1.3.1**), Unity **5.3.4f1** com Mono e módulo Android. O compilador Kairo é o pacote **Microsoft.Net.Compilers 1.1.0**, executável **1.1.0.51109**. O compilador App é o **smcs 2.6.5.0** que acompanha esse Unity. A identidade de fabricação dos compiladores históricos é **UNKNOWN**; estes executáveis e esta receita reproduzem os bytes sob o critério declarado. Não se precisa de Visual Studio para compilar os alvos.

Pré-requisitos a fornecer: Unity 5.3.4f1 oficial; Microsoft .NET Framework com `C:\Windows\Microsoft.NET\Framework\v2.0.50727\mscorlib.dll` do hash especificado; runtime .NET Framework 4.x e `C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe` para os inspetores próprios. Não substituir o mscorlib Microsoft pelo Mono nem os System do perfil classic pelos do perfil unity. Para consultar os componentes Android oficiais, o instalador registrado foi `UnitySetup-Android-Support-for-Editor-5.3.4f1.exe`, SHA-256 `17ce1e474487363ddcf2b18191410f1216afba3885eb88e928310029bc1523e3`, com Authenticode válido para Unity Technologies SF. A origem registrada é a página oficial `https://unity.com/releases/editor/whats-new/5.3.4f1`.

O builder procura o Unity em `%USERPROFILE%\AppData\Local\Programs\Unity\5.3.4f1`; pode-se selecionar outra instalação pelo ambiente **do processo** `UNITY_ROOT`. Mono usado para smcs:

| Componente | Caminho sob UNITY_ROOT | SHA-256 |
|---|---|---|
| smcs | `Editor/Data/Mono/lib/mono/unity/smcs.exe` | `26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0` |
| Mono runtime | `Editor/Data/Mono/bin/mono.exe` | `3d102d9a63dae72e48396998bdbae4e9b2d8952ca6a8ebbcba698d3bc4345076` |

O builder smcs configura apenas no subprocesso `MONO_PATH={UNITY}/Editor/Data/Mono/lib/mono/unity` e `MONO_CFG_DIR={UNITY}/Editor/Data/Mono/etc`. O programa compilado não é executado. Roslyn 5.8/.NET 10 foram ferramentas auxiliares de análise histórica e dos motores, **não são compiladores dos alvos nem pré-requisitos desta receita de build e verificação por métodos**. Mono.Cecil e os dois inspetores próprios vêm do Unity/fontes do repositório; Python usa a biblioteca padrão nesse caminho.

## Preparar um clone e a cópia original

Execute todos os comandos na raiz do clone. Preserve UTF-8 e LF dos fontes: `.gitattributes` fixa os endings. Não passe os fontes por um editor que ordene usings, membros, constantes ou remova elementos não usados. Os aliases e constantes locais não usados também afetam a emissão de metadados com PDB.

```powershell
git clone https://github.com/Tim-campy/pocket-league-story-reconstruction.git
Set-Location pocket-league-story-reconstruction
git checkout v1.0.0-csharp-reconstruction
$env:UNITY_ROOT = 'C:\caminho\Unity\5.3.4f1'
```

Coloque **sua cópia** do APK em `original/app.apk`, sem modificá-la. SHA-256 obrigatório: `d557af6b4bc7748605a39ed3548821f6763c5773a12432a3fed6e8c413b211e8`. O nome original fornecido foi `net.kairosoft.android.soccer_en_2.0.2-APK_Award.apk`. A extração abaixo é só leitura do APK para uma pasta nova e não redefine o cadastro de métodos:

```powershell
@'
from pathlib import Path
import hashlib, zipfile
apk = Path('original/app.apk')
assert hashlib.sha256(apk.read_bytes()).hexdigest() == 'd557af6b4bc7748605a39ed3548821f6763c5773a12432a3fed6e8c413b211e8'
target = Path('original/extracted').resolve()
assert not target.exists(), 'Use a fresh extraction directory'
with zipfile.ZipFile(apk) as z:
    assert len(z.infolist()) == 1218
    assert len({i.filename for i in z.infolist()}) == 1218
    for i in z.infolist():
        assert (target/i.filename).resolve().is_relative_to(target)
    z.extractall(target)
print('Original extracted; APK unchanged')
'@ | python -
```

Originais das DLLs: KairoLibrary SHA-256 `bb553d341eecd9b900f772486bf4a5c289aa2f09566d9d402a3eb70268f4b3ee`; Assembly-CSharp SHA-256 `3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949`. As referências extraídas do APK são usadas só para leitura de APIs/metadados. O App é compilado contra a **KairoLibrary original como referência**, conforme a receita medida; isso não copia sua implementação para a saída. O resultado a empacotar futuramente continua sendo a KairoLibrary reconstruída.

Obtenha **somente o pacote portátil 1.1.0** e conserve todos os arquivos de `tools/` dele. Este trecho usa a origem NuGet e hashes fixados, não instala ferramentas globalmente:

```powershell
@'
from pathlib import Path
from urllib.request import urlopen
import hashlib, zipfile
package = Path('analysis/downloads/microsoft.net.compilers.1.1.0.nupkg')
package.parent.mkdir(parents=True, exist_ok=True)
if not package.exists():
    with urlopen('https://api.nuget.org/v3-flatcontainer/microsoft.net.compilers/1.1.0/microsoft.net.compilers.1.1.0.nupkg', timeout=60) as r:
        package.write_bytes(r.read())
assert hashlib.sha256(package.read_bytes()).hexdigest() == 'cd84fb2c71cafe03cdae0add609fa12c75f61c89ea87b1422fd108ac7e570745'
target = Path('analysis/tools/roslyn-1.1.0').resolve()
with zipfile.ZipFile(package) as z:
    for i in z.infolist():
        assert (target/i.filename).resolve().is_relative_to(target)
    z.extractall(target)
assert hashlib.sha256((target/'tools/csc.exe').read_bytes()).hexdigest() == '53b693536e8a86f35d05d2165ff9b3e0aeaa499ecfd8135a9f99ab4404ca86c3'
print('Pinned compiler available')
'@ | python -
```

O script histórico `tools/fetch_roslyn_neighbors.py` também obtém essa versão, mas baixa e sonda versões vizinhas; não é necessário para reproduzir o resultado final.

## Caminho PDB e SUBST E:

O CodeView original contém exatamente **`E:\workspace\Library\KairoLibrary\obj\Android\KairoLibrary.pdb`**. O Roslyn recebe esse caminho por `/pdb:` e escreve ali durante a compilação. A DLL não é alterada depois. O helper `tools/kairo_pdb_output.py` cria automaticamente `SUBST E: "{ROOT}\build\historical-pdb-drive"`, verifica a propriedade da unidade com QueryDosDevice, cria o diretório físico e recusa uma unidade E: que aponte para qualquer outro lugar. Não remover/substituir uma unidade alheia para fazer o build. Se E: estiver ocupado, esse ambiente não satisfaz a receita até o conflito ser resolvido pelo responsável da máquina.

O diretório físico do PDB é `{ROOT}/build/historical-pdb-drive/workspace/Library/KairoLibrary/obj/Android/`. O builder espelha o PDB em `build/KairoLibrary-roslyn-1.1.0/KairoLibrary.pdb` para leitura; **somente o PDB é copiado**, não se reescreve a DLL. Esse caminho fixo eliminou 97 diferenças de caminho/padding antes presentes no CodeView. App usa `-debug-`, portanto não há PDB nem MDB final.

## Compilar e verificar, sem APK

```powershell
python tools/regenerate_references.py
if ($LASTEXITCODE -ne 0) { throw 'Reference regeneration failed' }
python tools/verify --all
if ($LASTEXITCODE -ne 0) { throw 'Full verification failed' }
```

`regenerate_references.py` lê as DLLs da cópia autenticada e recria `ref/` sem precisar dos bundles privados antigos, sem decompilar e sem resetar o cadastro. O verificador já compila ambos os alvos. Para apenas compilar, o comando é `python tools/build`. Equivalentes explícitos:

```powershell
python tools/build_kairo.py --compiler analysis/tools/roslyn-1.1.0/tools/csc.exe --label roslyn-1.1.0
python tools/build_csharp.py --compiler smcs --optimize + --debug - --stdlib default --label mono-smcs
```

Em Kairo o processo executado é `csc.exe /noconfig @compile.rsp`; em App, `mono.exe smcs.exe -noconfig @compile.rsp`. Cada response file é UTF-8, um argumento entre aspas por linha: opções, referências e fontes na ordem integral das tabelas abaixo. Os builders geram `compiler.log`, `compile.rsp` e `build-manifest.json` com SHA de todos os insumos. Não adicionar `/unsafe+`, `-nostdlib` ao smcs, símbolos UNITY, references automáticas de um projeto moderno ou flags de determinismo. Stdlib do smcs é **default**, apesar da referência explícita mscorlib do APK.

Saídas canônicas: `build/KairoLibrary-roslyn-1.1.0/KairoLibrary.dll` e `build/Assembly-CSharp-mono-smcs/Assembly-CSharp.dll`. Recibos: `build/verification-KairoLibrary.json`, `build/verification-Assembly-CSharp.json`; esperados: 3287 e 1958 matched, 337 e 129 tipos iguais, zero regressões. `tools/verify` retorna 0 para sucesso, 1 para diferença/erro e 2 para falta de escopo. Não executar os ciclos/motores históricos para reproduzir o build.

Depois dessa verificação, compare **todo byte** dos arquivos. Este teste independente da biblioteca padrão não edita, mascara em memória, normaliza ou produz DLL: enumera diferenças e exige que todas estejam nas seis coordenadas comprovadas. A identidade do original e a igualdade dos demais bytes fixam a geometria do arquivo.

```powershell
@'
from pathlib import Path
import hashlib, json
specs = {
 'KairoLibrary': ('build/KairoLibrary-roslyn-1.1.0', 569856, 'bb553d341eecd9b900f772486bf4a5c289aa2f09566d9d402a3eb70268f4b3ee', [(136,4),(527096,16),(550520,4),(550548,16)]),
 'Assembly-CSharp': ('build/Assembly-CSharp-mono-smcs', 895488, '3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949', [(136,4),(893568,16)])}
for name, (folder, size, expected, fields) in specs.items():
    a = Path('original/extracted/assets/bin/Data/Managed', name+'.dll').read_bytes()
    b = Path(folder, name+'.dll').read_bytes()
    assert hashlib.sha256(a).hexdigest() == expected
    assert len(a) == len(b) == size
    allowed = {i for start, length in fields for i in range(start,start+length)}
    differences = [i for i,(x,y) in enumerate(zip(a,b)) if x != y]
    assert not (set(differences)-allowed), 'Residual byte differences'
    receipt = json.loads(Path('build/verification-'+name+'.json').read_text(encoding='utf-8-sig'))
    assert receipt['build_sha256'] == hashlib.sha256(b).hexdigest()
    assert receipt['counts']['matched'] == (3287 if name == 'KairoLibrary' else 1958)
    print(name, 'PASS', size-len(allowed), 'identical bytes outside proved fields;', len(differences), 'variable bytes')
'@ | python -
```

`PROGRESS.md` publicado é o artefato gerado da verificação de encerramento. `python tools/progress` regenera contagens de um clone; o selo literal adicional exige renovar a prova para o PE atual. Snapshots privados dos ciclos antigos não são necessários para os comandos de build, verificação e comparação literal acima. Não se deve tentar recriar seu diário nem usar um recibo antigo como prova de um novo MVID. O manifesto publicado `reports/reconstruction-closure.json` contém hashes do build do marco; não se espera o mesmo SHA integral em uma nova compilação.
