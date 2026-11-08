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

## Manifestos de compilação e referências ordenadas

As tabelas abaixo são geradas dos manifestos do build verificado. `{ROOT}` é a raiz deste clone; `{UNITY}` é a raiz do Unity 5.3.4f1. Caminhos relativos dos fontes são resolvidos contra `{ROOT}`. Não ordenar referências ou fontes novamente.

### KairoLibrary

Compilador SHA-256: `53b693536e8a86f35d05d2165ff9b3e0aeaa499ecfd8135a9f99ab4404ca86c3`.

Opções, na ordem (o script passa noconfig fora do response file):

```text
/nologo
/noconfig
/nostdlib+
/target:library
/platform:anycpu
/optimize+
/debug:pdbonly
/deterministic-
/langversion:6
/unsafe-
/checked-
/codepage:65001
/utf8output
/pdb:E:\workspace\Library\KairoLibrary\obj\Android\KairoLibrary.pdb
/out:{ROOT}/build/KairoLibrary-roslyn-1.1.0/KairoLibrary.dll
```

Não há defines adicionais. Referências, nesta ordem:

| Ordem | Arquivo | SHA-256 |
|---:|---|---|
| 1 | `C:\Windows\Microsoft.NET\Framework\v2.0.50727\mscorlib.dll` | `c4729c6fc2279933fa9638f60775ab2f7eb731d5b04f9e04c15187afbdf94558` |
| 2 | `{UNITY}\Editor\Data\Mono\lib\mono\2.0\System.Core.dll` | `4998eda31c3d7bebf606f98cef77ad697f9e82e629efc37c06517019c2c2c271` |
| 3 | `{ROOT}\original\extracted\assets\bin\Data\Managed\UnityEngine.dll` | `7cc901371cc77e103d00b5845e8285f85fbdf36bb47ccf3d824f94c4ead32421` |
| 4 | `{UNITY}\Editor\Data\Mono\lib\mono\2.0\System.dll` | `c2225d7ba58a12e22c266384d3c1e4e882ab4e42449b91b721c21b5f1fcd63be` |

Ordem integral dos **163 fontes**, incluindo AssemblyInfo por último. Os hashes também permitem conferir line endings UTF-8/LF de um clone:

| Ordem | Fonte | SHA-256 |
|---:|---|---|
| 1 | `src/KairoLibrary/java/io/ByteArrayInputStream.cs` | `bf59c4da1d7fd2a2f532b6e72c98eba8978c193fc854bd63d4da983787483fc3` |
| 2 | `src/KairoLibrary/java/io/ByteArrayOutputStream.cs` | `e96ca313d69c7f83e0de36dbae7f404220491d3b67f46545a327f614442c81cc` |
| 3 | `src/KairoLibrary/java/io/DataInputStream.cs` | `5b71e5b0f9bd6a92d123691d097fb55943d297c0d480cd69ae039669b6d7ea3d` |
| 4 | `src/KairoLibrary/java/io/DataOutputStream.cs` | `1c72151b8f932e37cb347fba3d5204a9704c8890c2862f997754469620086fe7` |
| 5 | `src/KairoLibrary/java/io/EOFException.cs` | `df6306e24611a14af9831cc6098de89c0c60435e2210cb125375e296c52de074` |
| 6 | `src/KairoLibrary/java/io/InputStream.cs` | `6424aff75a6b336259aaed79770bb87cd8eb524d9863aef73719703dab98adbc` |
| 7 | `src/KairoLibrary/java/io/OutputStream.cs` | `331ef7c6992b60c014d4f856e82d0b868d0de1fbfba6aa756888ecde889851f4` |
| 8 | `src/KairoLibrary/java/io/WrappedSystemStream.cs` | `07fc699da1fdd259fa7b15f0f633ccf3a5dd17447faa9dd6c4924100db502525` |
| 9 | `src/KairoLibrary/java/lang/JBoolean.cs` | `b45139f4c8a1f60ec11543d5b094bae2d00d541a5f46b4a993c84f1abea0c731` |
| 10 | `src/KairoLibrary/java/lang/JByte.cs` | `869b57d05875e475ff69b6c000afa6818bd081abc5f2f61b4969a633d4d12a39` |
| 11 | `src/KairoLibrary/java/lang/JCharacter.cs` | `2f5e3f5260865906902c8430a136bfa7e931fe2c721dabd58e302476db7c3a84` |
| 12 | `src/KairoLibrary/java/lang/JDouble.cs` | `af598bfc864a7d0da1e79489fa96f6ea1291cc2b6d76a15b42d5a3034f2aab33` |
| 13 | `src/KairoLibrary/java/lang/JFloat.cs` | `31cca5849a25c72ccc59d7b2e6811c065ae19e85b1e67fa52d936c31129b1db8` |
| 14 | `src/KairoLibrary/java/lang/JInteger.cs` | `ad491b155065c2b61475cee5d0457e1eb1233e33d7fad193337d9ebeb415ce51` |
| 15 | `src/KairoLibrary/java/lang/JLong.cs` | `63ad332aa4282a0bbe16f31c06dfebe1182c54fb9b59591e5a95cdf7507f3f3e` |
| 16 | `src/KairoLibrary/java/lang/JMath.cs` | `9860752a04780c6108a761ff687ec96e71a938acbddf34ff86a885fbea04a4da` |
| 17 | `src/KairoLibrary/java/lang/JRunnable.cs` | `f1451d338a46b812d0eec90e1dd4ba04ff4688f7eb40a7a64ff4b524fc7387d4` |
| 18 | `src/KairoLibrary/java/lang/JRuntime.cs` | `c209ab01265338850fc561ff82b9ea21d17d51b9a9c12963e94931cbb9134c41` |
| 19 | `src/KairoLibrary/java/lang/JShort.cs` | `0737b41b70b0e0b28f772f4d7fda7982424d85a2f9a8a4734dc1b33b15380258` |
| 20 | `src/KairoLibrary/java/lang/JString.cs` | `61273608ed20657cee74d35d8951a086760cbf3bad0c56a8d3230291aa85c375` |
| 21 | `src/KairoLibrary/java/lang/JSystem.cs` | `c4e8f5f6d6ca6427aedcab629aee84d40b42821300758fe52b297cea360b4230` |
| 22 | `src/KairoLibrary/java/lang/JThread.cs` | `ea36bd38b1e14578b9591753dd8be1d3e2b264fcfb8f3b4437019bcd9ce339be` |
| 23 | `src/KairoLibrary/java/lang/StringEx.cs` | `13f801b687ff748c145be10f0c97def8a22008ae7c405318896fe46ffa4e0843` |
| 24 | `src/KairoLibrary/java/native/JavaPlugin.cs` | `ba7be4ef4cd851059046bc725efedd41dd7f657edbcd6ee7ece808ceb71a1a96` |
| 25 | `src/KairoLibrary/java/native/PluginBase.cs` | `03bf86121c47ad50fbf5f098e949d56d06050d09a10f4c9894b812bca5e85ed3` |
| 26 | `src/KairoLibrary/java/net/URLDecoder.cs` | `60067294268bd549b1ecd00fda848d76b83410650aed7dec28e4374d94a03c7c` |
| 27 | `src/KairoLibrary/java/net/URLEncoder.cs` | `751748a4537ed834c6a71e609e69c1c51ace5a2afbe1e89e9bb3d8ef2211ae3e` |
| 28 | `src/KairoLibrary/java/util/JCalendar.cs` | `cddc58cbfcbf02185a8ce3e63e7ae6748502bd4861821c2e34a7eb3d17852e7e` |
| 29 | `src/KairoLibrary/java/util/JDate.cs` | `82229716d32ac69db721dc274101d2c01afd57ee81caec36f3d35fbd6b509a79` |
| 30 | `src/KairoLibrary/java/util/JLocale.cs` | `8635242e6792240f3766c85f33fe2e108b32ad969c545d37feb2d94c11679f5a` |
| 31 | `src/KairoLibrary/java/util/JRandom.cs` | `ea9a23d75f7d24d8627c4b4027060fd61b56b39ff87ac58a2f337a9a69f03c93` |
| 32 | `src/KairoLibrary/java/util/JTimeZone.cs` | `38735853684da45b9c8d81f5d4df75f7be0b8a522f78c879b46743ef58656f56` |
| 33 | `src/KairoLibrary/java/util/JTool.cs` | `1bb01e7f7ce103ff187bb17bdb80a59617d8f02a6efdf5fb21db8dbebf88be6c` |
| 34 | `src/KairoLibrary/java/util/IVector.cs` | `c055c8932caf49c9895f4f71c5c0bc8a9cb15672acf0f1cdcfa26e46f2ae57ff` |
| 35 | `src/KairoLibrary/java/util/Vector.cs` | `2ef88b058fdc50346525f719b06fbff47b968eb1c3d27a7f1442aaa9639cad24` |
| 36 | `src/KairoLibrary/java/util/Vector`1.cs` | `0951b847db56ebf876037602509e4bdb5a040cece046e0798fdd16f2c8be6f0b` |
| 37 | `src/KairoLibrary/kairo/common/cfg/Config.cs` | `212375994c0421ffc843d8c8e610a7ecfd230af8ef0d9d743defae3fe87ddb52` |
| 38 | `src/KairoLibrary/kairo/unity/ad/AdSize.cs` | `04a2ec1e4afdc1ba20bb3d3db437bbb0f42451197f3712d6642684d561cb94e5` |
| 39 | `src/KairoLibrary/kairo/unity/ad/AdMobView.cs` | `643bc0ef8666625005a950034c848654c35f81ab75dcd27d1f0599ff4a02a0b6` |
| 40 | `src/KairoLibrary/kairo/unity/ad/InterstitialAd.cs` | `98231f8536456608948cf4d819db7f3306a48ff1e43878e9ac6fd22f8deee89a` |
| 41 | `src/KairoLibrary/kairo/unity/analytics/AnalyticsTracker.cs` | `e5fcf568c56deaec547dfcae6bea3315fe73e54e632148ba5c5c39387e4150a6` |
| 42 | `src/KairoLibrary/kairo/unity/form/FormBase.cs` | `a40cdc44121704f35b678c16fc8ba9240a4110552cd8f269b166597ae513cb01` |
| 43 | `src/KairoLibrary/kairo/unity/form/FormManagerBase.cs` | `69a757b6eac1b42af6dc993cd406823bf23a0f904fe2e75b7f74a70bccb34b9c` |
| 44 | `src/KairoLibrary/kairo/unity/graphics/Material.cs` | `5c5e097e1b687aac67607194218d6c426e0a938b615e5d46acf3f6d733850645` |
| 45 | `src/KairoLibrary/kairo/unity/graphics/MaterialManager.cs` | `e0467be91c038523cc17ba4e914ea6981e358d9e1afde857ab2cef8d4116c0f4` |
| 46 | `src/KairoLibrary/kairo/unity/graphics/MeshBuilder.cs` | `c1f27bc426227f74c7fd4ca15a7ac231c64a019ca43dcf5fc44de3719bf5dba5` |
| 47 | `src/KairoLibrary/kairo/unity/graphics/MeshManager.cs` | `e2f81339d86d51b750e32c0e5a0553389879f2fd46dc84437e5c91ad8dda81ad` |
| 48 | `src/KairoLibrary/kairo/unity/graphics/MeshRecord.cs` | `4f67b2f6a5be0b109965b3a389f45f7fda9bf6577dcf24c3ca3f1b132ec048c4` |
| 49 | `src/KairoLibrary/kairo/unity/graphics/Offscreen.cs` | `74e958cd810afb75439a27b126e4cdee83771ecf4e1bae1f30e7863955ce0c8d` |
| 50 | `src/KairoLibrary/kairo/unity/graphics/Point.cs` | `91db25eec03d19e0b6d89de36b7bd03533d676e6178a6c9d676c9a3637d3eca3` |
| 51 | `src/KairoLibrary/kairo/unity/graphics/Rectangle.cs` | `696b8a93241dd3b928a0903710377c075094f89816e7340075341f25d78e64d0` |
| 52 | `src/KairoLibrary/kairo/unity/graphics/RectangleF.cs` | `62f3aff0e2f8d568de63a7017ba1ea1ff4bd7d0e7428683b88a3a202dd9dd502` |
| 53 | `src/KairoLibrary/kairo/unity/graphics/ShaderBuilder.cs` | `af721dfcf236c718de7a0c668c6f02a0d1cc79f5a2b475bf3fd0b18beb8cb362` |
| 54 | `src/KairoLibrary/kairo/unity/graphics/ShaderManager.cs` | `13db0e48ef2d55a5e8c6c89268307b0aa60be6b1b161f920979793facfd046a9` |
| 55 | `src/KairoLibrary/kairo/unity/graph/Graph.cs` | `5345e63dcf7bff410c4aed3fdef67e45d109919919cf2cce32cf1a29912c9cfe` |
| 56 | `src/KairoLibrary/kairo/unity/iOSsupport/DeviceUtil.cs` | `543b571c87b47f8b8dddafb57515800bc739898739b0e9ea77294c74828fbe68` |
| 57 | `src/KairoLibrary/kairo/unity/iOSsupport/RecordUtil.cs` | `873feecdff9ab9b4774463e3c0f3e6484f2b5d05edc326036ab62e643df2591c` |
| 58 | `src/KairoLibrary/kairo/unity/io/AssetReader.cs` | `8d79e2e996ef0268675e28297e50006b6570457deddc9fb94697dfc744938ebd` |
| 59 | `src/KairoLibrary/kairo/unity/io/BitArrayInputStream.cs` | `53fbffc44d64544dc518d08010a2a6c5c4424b480d71c2b64ed42ffb86bb095f` |
| 60 | `src/KairoLibrary/kairo/unity/io/BitArrayOutputStream.cs` | `7be30a0aa206526f9d22165bb429efd2179ccab07df440ee825ed7dfe74b861f` |
| 61 | `src/KairoLibrary/kairo/unity/io/ByteListOutputStream.cs` | `eca65276382432917eedecf9d91f35ce57b6fd739332e3a694fa504ce05135b2` |
| 62 | `src/KairoLibrary/kairo/unity/io/ConnectionException.cs` | `771948ea8a875c03ca41353f8ccaefcbdcfef9ee5e31bc2c38d6bcf8cb3e6c4f` |
| 63 | `src/KairoLibrary/kairo/unity/io/HttpObject.cs` | `c8b186ca577db3c3317d4285a9dd203648b55b97a144cace084b94facd8def5d` |
| 64 | `src/KairoLibrary/kairo/unity/io/Http.cs` | `a9ff9a7aa42c8a58e156c3a34fd9fd19870bc160c271de176339835290f56e1e` |
| 65 | `src/KairoLibrary/kairo/unity/io/RecordStore.cs` | `0b7db60bb734d8d4ee11f24ec38a56f525ef18e5b44a11d1efcf0d3848192580` |
| 66 | `src/KairoLibrary/kairo/unity/io/RecordStoreListener.cs` | `616bacc49e494623ffa9f09fdd6331a360a33a2ee464dbcc44b67184d6070225` |
| 67 | `src/KairoLibrary/kairo/unity/io/ScratchPad.cs` | `fa92a2e95f9a51a87ad5bd67f3fc613790ef78e4a2744be7cb1d6db5797ab890` |
| 68 | `src/KairoLibrary/kairo/unity/io/Storage.cs` | `1c19cd40f78875e23d0252b1b5993f2f97a09b65bbdb9ea91e5d598957134b91` |
| 69 | `src/KairoLibrary/kairo/unity/licensing/LicenseChecker.cs` | `f5ecf6fbb95825deff195988f8d604c92ceeb0a829e739fbea3c2a098e11d1b8` |
| 70 | `src/KairoLibrary/kairo/unity/m3d4/util3d/FastMath.cs` | `90a38a6fbf0df2ebf00127354f929b8b9a3e26fd1d78a1f06d339dd0eb175d5b` |
| 71 | `src/KairoLibrary/kairo/unity/m3d4/util3d/Transform.cs` | `e74870d012070490b13fbcc2287f38319f5d3cf7d725c43dc2299f10474625de` |
| 72 | `src/KairoLibrary/kairo/unity/m3d4/util3d/Vector3D.cs` | `c73a63e5653ef517efb5c48ed6cc4f3f4c39186b0075ed83fc5de28f7fa1ab7e` |
| 73 | `src/KairoLibrary/kairo/unity/math/KairoMath.cs` | `3444b630d67437bda57522a05c157b600bcb65f8d8d7332bd552cbd710320ac6` |
| 74 | `src/KairoLibrary/kairo/unity/native/AdMobPlugin.cs` | `36501ec2868f0c53bd121bafcc3e0c8f3db3dd10b5fe77eecbba6df8898d238b` |
| 75 | `src/KairoLibrary/kairo/unity/native/KairoPlugin.cs` | `7363298e7c149988ea3e08dcee08cab92cf0dce7908460b9e8271c535bc3d95f` |
| 76 | `src/KairoLibrary/kairo/unity/native/util/AsyncThread.cs` | `7cbfef707e14b032f9b353660c550a573b17c04ae94ed5be2eb71c117a8be69b` |
| 77 | `src/KairoLibrary/kairo/unity/native/util/Dialog.cs` | `7f54b1372d89f8b1686ee6c750527bd417fbc78062ef93d1df636c38d5091f4f` |
| 78 | `src/KairoLibrary/kairo/unity/native/util/StringUtil.cs` | `f1576b1c78c48c853d64c794fa5c2116fc70f7cbca9c03c755f283fdb732cd8b` |
| 79 | `src/KairoLibrary/kairo/unity/panel/Button.cs` | `6d6b8c2f0fffb065db98832582e452e53e1000d945600bbe370a3a40782824ee` |
| 80 | `src/KairoLibrary/kairo/unity/panel/Component.cs` | `2305b813199edb2268d2fd67476e06478aeb7e429aa0be40c6c25d518b36bbb2` |
| 81 | `src/KairoLibrary/kairo/unity/panel/FepPanel.cs` | `c990eca26d29d2796ca604eef954119d843f03834d63e2c76bb535bf235de451` |
| 82 | `src/KairoLibrary/kairo/unity/panel/Label.cs` | `79db331937c9aaf62546918aade44fe3c2e2eb9cb5db06badf2694d07f9cbcab` |
| 83 | `src/KairoLibrary/kairo/unity/panel/TextBox.cs` | `58c622ddc280a9f2c3deb75e275c3fc055b585ca088ada1ff9772b24b2dcd140` |
| 84 | `src/KairoLibrary/kairo/unity/surface/MotionEvent.cs` | `30fa575067d06816e67625f2ae42d3b2bdfebec4a6c235835af26d68ee8cc388` |
| 85 | `src/KairoLibrary/kairo/unity/surface/SurfaceBase.cs` | `9be14b9b1a6154bcb5cc6b683021f55b4d3ed25930336cf2626146906a9e585c` |
| 86 | `src/KairoLibrary/kairo/unity/surface/SurfaceManagerBase.cs` | `8b9942f26309c61c836de756a23358a0b63511f6bf73016237dd29fc2dac6ec9` |
| 87 | `src/KairoLibrary/kairo/unity/surface/TouchComponent.cs` | `86d414f7eee9b5ac0f9feb4c4b60736d0f439527a9379597e60cfdf3bfd5d35e` |
| 88 | `src/KairoLibrary/kairo/unity/surface/TouchEvent.cs` | `9b6d5b14b1feb302d59e0944d3b8b427d468b06df51e11bd816a24dbb56564a4` |
| 89 | `src/KairoLibrary/kairo/unity/surface/TouchOption.cs` | `c9622254885d4834b0e3217403a36d6b42acaa7f079eff85a5429b95106a3928` |
| 90 | `src/KairoLibrary/kairo/unity/surface/TouchValueAccessor.cs` | `4edae0fbcad5bc6e7a228f3426a64e4b870ef2d872cee8afa2abebd3aeb20530` |
| 91 | `src/KairoLibrary/kairo/unity/system/ErrorCode.cs` | `39fddfef0aaad1fe0a201332978a6d744ea9d8a55919491199b79e354fbe10e4` |
| 92 | `src/KairoLibrary/kairo/unity/system/GameDataRecord.cs` | `dd53dd4b7005dad54618bdb648d0e242f1443f913daccc5965574aa00c1da2a6` |
| 93 | `src/KairoLibrary/kairo/unity/system/KairoBase.cs` | `2528165f03e189929be1cdfc92cb8d7020deadc2d7e6e5afd92fe036d8f3cd58` |
| 94 | `src/KairoLibrary/kairo/unity/system/KairoClub.cs` | `5db7f7f788c0371fe087053fcb38f238bc1bc3e639096d2edee1b3ce661fe82a` |
| 95 | `src/KairoLibrary/kairo/unity/system/appli/GameAppli.cs` | `e645fc711a88f17ba1dcff86008f30a1945322275bf869c4dc65dd7530feb948` |
| 96 | `src/KairoLibrary/kairo/unity/system/lineup/ui/LineupData.cs` | `0fea4e67d6ab9d98ac9f41e858f5b5619ee3c61ed6d7fefee27679027a00a715` |
| 97 | `src/KairoLibrary/kairo/unity/system/lineup/ui/LineupManager.cs` | `20451e13877e7aaa381d1e36c506dda73ee9a1f756ec304e5147d0994ec5dd0f` |
| 98 | `src/KairoLibrary/kairo/unity/system/lobi/LobiManager.cs` | `ffbb9ba9dfc02e6e2fa06a7fdbf202e97937182671031869f2385ff2e9fafaa0` |
| 99 | `src/KairoLibrary/kairo/unity/system/notification/ui/NotifyManager.cs` | `befc10cb39566bede5e0c29a8434aa0e2adccf4f1118100dca5cd9ff7bfca857` |
| 100 | `src/KairoLibrary/kairo/unity/system/payment/IllegalPurchaseException.cs` | `9e11be0c74c766789388c5698e3302251e6986d3ad7726fa82938fd9c8e82849` |
| 101 | `src/KairoLibrary/kairo/unity/system/payment/PaymentDataRecord.cs` | `3e3bbd5a293ab61504b81a90b2c3903a0a342aaeaad31f4905ad18b5ad8ec571` |
| 102 | `src/KairoLibrary/kairo/unity/system/tapjoy/ui/ITapjoy.cs` | `8b215cb0cadee39cde5a3c4ba9b65dfc8822f8c1d1fd2da73aaa7127d03ff2f0` |
| 103 | `src/KairoLibrary/kairo/unity/ui/Canvas.cs` | `180c02aae50c99db2e8f6659d90d6272e4c2b12c157f5123e299e244289bee2a` |
| 104 | `src/KairoLibrary/kairo/unity/ui/DepthInfo.cs` | `127396c63d9519d1df9293494b117cc7789587463216b8c3d9d27263b858b52a` |
| 105 | `src/KairoLibrary/kairo/unity/ui/Dialog.cs` | `9bc23188172bc86210c41124073af7f034c7b3becf9226ad8070bf1834890c85` |
| 106 | `src/KairoLibrary/kairo/unity/ui/AppEventType.cs` | `0db4b9259ab51bb21cbaa32ff5c5ff51de22be041f7e7929a80daade619a373a` |
| 107 | `src/KairoLibrary/kairo/unity/ui/AppEvent.cs` | `60ad6f54f666045bbc31bad86626426b074d8aeecf1a3ba6dec88608fd5a0710` |
| 108 | `src/KairoLibrary/kairo/unity/ui/Font.cs` | `89ace2fab97e420b2999fdbd3f04057370c9bf850e23456cadcc9ec902042495` |
| 109 | `src/KairoLibrary/kairo/unity/ui/Graphics.cs` | `0a71677089edddbde65b080a094d5e1acd372e625876dc00d5e7224cfd224f97` |
| 110 | `src/KairoLibrary/kairo/unity/ui/Handler.cs` | `3849e0df15c90d1121e4eddb2fd03112e99b4e55c35a2c423027d4e2c035c071` |
| 111 | `src/KairoLibrary/kairo/unity/ui/IApplication.cs` | `a5328c2d441eeb6c8689d0c3659a6d6f59583c57da2ee67c06252b27084702ea` |
| 112 | `src/KairoLibrary/kairo/unity/ui/Image.cs` | `e13d04404eb55cf28caa9878e42ca8471f73a555e26ac5a5596fdb5e845a357f` |
| 113 | `src/KairoLibrary/kairo/unity/ui/ImageAtlasManager.cs` | `497c89b0733e1a059f4d64b8df137485efa542f7d6077138d7139d9dc2b20df8` |
| 114 | `src/KairoLibrary/kairo/unity/ui/ImageAtlas.cs` | `d5bdbf0f56375983bde5e3b7c01e2ccbedcac541723c168c9115ceaf299b36ec` |
| 115 | `src/KairoLibrary/kairo/unity/ui/ImageTransform.cs` | `1ca1ad76e0f4f35a7ab4be2895c281947d238e5e4327c87996dc4470937f9519` |
| 116 | `src/KairoLibrary/kairo/unity/ui/KeyEvent.cs` | `fe6cecd7a981197311d3ecbdbd2e3a305a6062ff2243afb4c0a746056cf7479e` |
| 117 | `src/KairoLibrary/kairo/unity/ui/Matrix.cs` | `fffbe2263a3e6aacfe2e801ebf90a023a91bea4d7cbf08bbbc50caa2232ab083` |
| 118 | `src/KairoLibrary/kairo/unity/ui/Pictograph.cs` | `5e406e63f9cde4e7dd9a969a7ecce8a8b56902480a1743f6beb2526e8c2c8bd6` |
| 119 | `src/KairoLibrary/kairo/unity/ui/ResourceManager.cs` | `1a98843775764d222b4907f1adf189260c5f8322924132dc48b2cee6d1c53d6b` |
| 120 | `src/KairoLibrary/kairo/unity/ui/SaveRecorder.cs` | `7d7ce5adb8348db57160096de80ffd3c7ce1280c99ecc981521d6ce59d0c6d9f` |
| 121 | `src/KairoLibrary/kairo/unity/ui/Seb.cs` | `3788d6d2fa46fd83ea42575896b133f64b3b5e9c30585f4a1528721258393306` |
| 122 | `src/KairoLibrary/kairo/unity/ui/SoundBase.cs` | `0058dff25c20cc4cf5ffb4f49c1d1869ed3156d2980b3febf7972b36d247482e` |
| 123 | `src/KairoLibrary/kairo/unity/ui/SoundOGG.cs` | `bce422aa0b1d24d95024c19f0853a16f7a943ca7ff7fbb24120bea47b44c2d3f` |
| 124 | `src/KairoLibrary/kairo/unity/ui/SoundPlayer.cs` | `82de7c191939b9a34d8469c59b77421c29a8955be62cedb03d4d77453e99fde3` |
| 125 | `src/KairoLibrary/kairo/unity/ui/TextFormat.cs` | `34c7ea462e3071c46ec76a64d46dc0f4cfef2f67aade72ad227557ef77491f4e` |
| 126 | `src/KairoLibrary/kairo/unity/ui/TextLayout.cs` | `9f458059da0336dfb92306202ac7a2028739b0ee9badd4caf27bfcf336e078d7` |
| 127 | `src/KairoLibrary/kairo/unity/ui/UIException.cs` | `e96c5c24a01c6189a98d21e55181676c74ef4cc95264db4ce5bf96a8da3caaab` |
| 128 | `src/KairoLibrary/kairo/unity/ui/Balloon.cs` | `b341581b65f4d9efd0db448696711bd21f1454a9050acd8bfad94dda7e918cc9` |
| 129 | `src/KairoLibrary/kairo/unity/util/Base64.cs` | `ca1bbe861320dfe4757ab13605d7d241aa0763139e34006831a8b88c80d0a1a4` |
| 130 | `src/KairoLibrary/kairo/unity/util/BitUtil.cs` | `6874c1049a7869b84ead42fdf98175c5dd1c47e2d0281780ded070b32205f121` |
| 131 | `src/KairoLibrary/kairo/unity/util/ClockTime.cs` | `791316640318d8782ee3b2312b8210f39eb8373c7daab43b2708cf7de0bbe52f` |
| 132 | `src/KairoLibrary/kairo/unity/util/Compressor.cs` | `c9c6ea94a4c9c95ed431a0881f17f88bec09fdb46104758ce6dcc162b4397764` |
| 133 | `src/KairoLibrary/kairo/unity/util/CRC64.cs` | `f3c8b1d65d8507b0f0099067ac9f5c4520c70d932650ec298a0ea2efb758ad5c` |
| 134 | `src/KairoLibrary/kairo/unity/util/DisplayMetrics.cs` | `e300c768ca92552b08c06ee4c7da9d5de1241b3eb62423877e3a82d2e4b1a0e8` |
| 135 | `src/KairoLibrary/kairo/unity/util/EncodeUtil.cs` | `c980a6c922f7baa6dce35c6cdbb818ec7a4cd185469a42c653ce04540d02299e` |
| 136 | `src/KairoLibrary/kairo/unity/util/Encrypter.cs` | `8a9a2fb3e4885bd024a48eaa3e00bd69eb39b4b4c6b831759a05dbddf5bcca0a` |
| 137 | `src/KairoLibrary/kairo/unity/util/FastVector.cs` | `9be6c0d3cccf5f38880db879557862b7772d03b412a3c411dcc2216c5f2f59af` |
| 138 | `src/KairoLibrary/kairo/unity/util/FastVector`1.cs` | `1c31bc9546e3ce559bf4ec4278985739f3e157450b3b5f1a1bd668492a7f8365` |
| 139 | `src/KairoLibrary/kairo/unity/util/IllegalFileException.cs` | `0724314f8f7edc37679e6248888ad00fa42aea5276491ca2da8fe0c6ec7eb6f5` |
| 140 | `src/KairoLibrary/kairo/unity/util/IntentUtil.cs` | `5cf14d2580d7e0cd430a930f7fd5bb69b104e17d989adfd92db36af5defa2bac` |
| 141 | `src/KairoLibrary/kairo/unity/util/ISerializable.cs` | `f91d426aa59f5deda65ca1fdf9f62cd13955006b5092eeb28788046e4e8374e9` |
| 142 | `src/KairoLibrary/kairo/unity/util/JarFormatException.cs` | `33c84c053284cc5c879cbc5ee452277fb3d19393074135d2d9964079de52e795` |
| 143 | `src/KairoLibrary/kairo/unity/util/JarInflater.cs` | `3ccf34a79c652050490b21ef176bdfa9525d4292b635dfad7df603a5f955a220` |
| 144 | `src/KairoLibrary/kairo/unity/util/KairoException.cs` | `7b1827e82ba73d2da60252230ff6d5b1386ba0d73311b56be359ec27fa010d68` |
| 145 | `src/KairoLibrary/kairo/unity/util/Language.cs` | `2614e0a9ec418850bc9cedc740461cda8b318392c5b32c6ff3e7a6506f528a62` |
| 146 | `src/KairoLibrary/kairo/unity/util/Log.cs` | `436af2223427fd8a90a02cc74397f4b953ae1092d730c86b26712c05decca724` |
| 147 | `src/KairoLibrary/kairo/unity/util/LogListener.cs` | `e69b744b38b4ca0548bca6185f076904a0f1eeb45474f18207c337c1a50e8551` |
| 148 | `src/KairoLibrary/kairo/unity/util/MD5.cs` | `f6762dab6f5b8def9e23ba7cb6b4f0f46a0e1b038fd475d151835eefb64e25dd` |
| 149 | `src/KairoLibrary/kairo/unity/util/NotPermitException.cs` | `cc4b82d222d4303f89ef57aa7cd0070c1cc94ef6c7bf8c155bd691d44d6ff779` |
| 150 | `src/KairoLibrary/kairo/unity/util/Property.cs` | `7c7c33d8fd66ba96440ed9c32b26c172033fa318f00c2ae8d660242e5de377c5` |
| 151 | `src/KairoLibrary/kairo/unity/util/SecureByte.cs` | `e6cebcbcedf361fd197fcfe47a1442b69a4f9a1afe1d05d38f77c738ed1f93c6` |
| 152 | `src/KairoLibrary/kairo/unity/util/SecureByteArray.cs` | `b3b207f7e9e58cf7a5d845a7c1d74d92a74e4cf08f1abe4657d5ff00767a504c` |
| 153 | `src/KairoLibrary/kairo/unity/util/SecureChar.cs` | `cd050e0d278a5bd646579fc07098ed3ab474551f664e41f6f853d40acb01bec9` |
| 154 | `src/KairoLibrary/kairo/unity/util/SecureInt.cs` | `34ac9175ef77ca9583b03e55c2b1c3bac019f457ce29b240f40768e1268400eb` |
| 155 | `src/KairoLibrary/kairo/unity/util/SecureLong.cs` | `5a019476bfb8877dac040fbb97625b059b3f8fe206304767b41aa70c0984f639` |
| 156 | `src/KairoLibrary/kairo/unity/util/SecureNumber.cs` | `2cc1ec50a4931179a5664346484c02cc1a2018ef7e1ca63b7b068d2dc8ec5e9b` |
| 157 | `src/KairoLibrary/kairo/unity/util/SecureProperty.cs` | `313e32ca50841677744364ad4ef0d7225c86f084b8be8cb2f5838d985debcb77` |
| 158 | `src/KairoLibrary/kairo/unity/util/SecureShort.cs` | `09600fcb3c491e68e8996ef04de13f535987cc9209463ff2dbf8f58ee0bf5259` |
| 159 | `src/KairoLibrary/kairo/unity/util/SecureString.cs` | `d154cd88c482ab1b59e0e38302f7c07eeb8aa9ef1cb554bb4f0091cedbf9a251` |
| 160 | `src/KairoLibrary/kairo/unity/util/StreamUtil.cs` | `a6ba984a8d152d283b1a3fe9cea34c9e74de2efd57a5c3f08ccc346490a3993b` |
| 161 | `src/KairoLibrary/kairo/unity/util/StringUtil.cs` | `aa228b4ac85a3359d34fcac3b281d45b5c3968c52ee7086e0157f81dcaf0a1d5` |
| 162 | `src/KairoLibrary/kfw/panel/MainPanel.cs` | `992f4614346de7822c434e0cbf677e61930fb42816676e53bff7c63cbf1e4ec4` |
| 163 | `src/KairoLibrary/AssemblyInfo.cs` | `1cbf6b1009bb124159b5efe89cd83d0b051b8e0c05ab8295a984f132679525c5` |

Arquivo de ordem: `config/compile-order/KairoLibrary.txt`, SHA-256 `4ed4fe75262b5431379d3681662bfdb630ec4093c994a1d7a7a3b6bac7a8a973`. O builder acrescenta AssemblyInfo.cs se necessário, conforme a lista acima.

### Assembly-CSharp

Compilador SHA-256: `26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0`.

Opções, na ordem (o script passa noconfig fora do response file):

```text
-nologo
-noconfig
-target:library
-platform:anycpu
-optimize+
-debug-
-langversion:Default
-unsafe-
-checked-
-codepage:65001
-out:{ROOT}/build/Assembly-CSharp-mono-smcs/Assembly-CSharp.dll
```

Não há defines adicionais. Referências, nesta ordem:

| Ordem | Arquivo | SHA-256 |
|---:|---|---|
| 1 | `{ROOT}\original\extracted\assets\bin\Data\Managed\mscorlib.dll` | `7f16171cf5f87ca1e4c229727526d2fbc786079fc0c960f36197013ea0d986e1` |
| 2 | `{ROOT}\original\extracted\assets\bin\Data\Managed\KairoLibrary.dll` | `bb553d341eecd9b900f772486bf4a5c289aa2f09566d9d402a3eb70268f4b3ee` |
| 3 | `{ROOT}\original\extracted\assets\bin\Data\Managed\UnityEngine.dll` | `7cc901371cc77e103d00b5845e8285f85fbdf36bb47ccf3d824f94c4ead32421` |
| 4 | `{ROOT}\original\extracted\assets\bin\Data\Managed\System.Core.dll` | `104278a0bf67b4de8fd1e910e3292a3ef51808ebee7a5f2a229125f18463c98c` |
| 5 | `{ROOT}\original\extracted\assets\bin\Data\Managed\System.dll` | `7e72b92ddcd6b7b791ab24dd5756544f2da81f65e7ba7d1ebe8223123720d0ac` |

Ordem integral dos **97 fontes**, incluindo AssemblyInfo por último. Os hashes também permitem conferir line endings UTF-8/LF de um clone:

| Ordem | Fonte | SHA-256 |
|---:|---|---|
| 1 | `src/Assembly-CSharp/analytics/AnalyticsTracker.cs` | `37782c22bbdc0fc6077fb2a5c9066af07a4942e6b773fd97dbf8d30ce9e00d1a` |
| 2 | `src/Assembly-CSharp/cfg/MyConfig.cs` | `13da0e5690d1b2429c5779983d1bf9cf1c573e4494f4aaaaa01504802a39cbe0` |
| 3 | `src/Assembly-CSharp/data/BaseData.cs` | `c76e5ed1e843f05b2805591ba5f91f67bc9f6db54a2c6893d74ad26780b3e659` |
| 4 | `src/Assembly-CSharp/data/EventData.cs` | `a1dd40337426addad019d0fd58c23f2df0faeb5c20c094de52b9c8298d404059` |
| 5 | `src/Assembly-CSharp/data/FanActData.cs` | `564763156b651fcb13767fe52d3a756e6783bac363ee8ebab441f8af35ea280e` |
| 6 | `src/Assembly-CSharp/data/FanBonusData.cs` | `a7b50ebde98efe42abf282dbb8dfc756664fd2d4a6a4575f78a2342d0910d8c6` |
| 7 | `src/Assembly-CSharp/data/FormationData.cs` | `a4c4b756311ac499d3708a9bf5fa8b39e55099115defddf29502dac317289945` |
| 8 | `src/Assembly-CSharp/data/LeagueData.cs` | `fede8c6998dce143cb11d8be31e9389fc4d10feb7f5265d272ee208a655b1fc0` |
| 9 | `src/Assembly-CSharp/data/MapchipMain.cs` | `f3f756d925748a5a1a5ee8dde6ce07f30ddad7a9332c4466d0e8ba3af0f90089` |
| 10 | `src/Assembly-CSharp/data/MapchipMatch.cs` | `98aad97031a51ef57936a2d78e021f1a2f2e5d99a50269c6bb75d242c4ccdbbd` |
| 11 | `src/Assembly-CSharp/data/PlayerData.cs` | `29f28beb00145c890f12269f694b5a5499a6c164f92c5c65332f62ac8da8c8ad` |
| 12 | `src/Assembly-CSharp/data/QuestMatchData.cs` | `ffd77ad5db3144c296a98e424b5a072c53e1cbe23d33685ee28bf57c427586ce` |
| 13 | `src/Assembly-CSharp/data/SponsorData.cs` | `8587a49b5e571b3bbc15cfdebda685c9e1c1789b2c34c08d483f89380986942a` |
| 14 | `src/Assembly-CSharp/data/StadiumData.cs` | `9dddf654649d63fd848e58a2d8185d975846ad950931d51f8bc76e6e58f0e413` |
| 15 | `src/Assembly-CSharp/data/SupervisorData.cs` | `2ff8ce4966049a27f34e762dfdf2f1f2202f54960ecaa5c262db4ac9b68bf3a5` |
| 16 | `src/Assembly-CSharp/data/TeamData.cs` | `0b837a7ab5c2b3bffbbc8f80cad499f04d1913beb388deb51e27923a197ce2ad` |
| 17 | `src/Assembly-CSharp/data/TenantData.cs` | `f623b9f1ed19e1f183f53eda7bb9a25a40c9cba09c863decc51ab2cb8541619c` |
| 18 | `src/Assembly-CSharp/data/TradeData.cs` | `893d4a2de0255b18d9ef58221630bfb0f69e00ad62fab4abe4e2deabdff209df` |
| 19 | `src/Assembly-CSharp/data/TrainingData.cs` | `cf191ed429d5a382e3f5b48f71f0b19ad6312aea4f6fdf2349f7b75c7b435ff8` |
| 20 | `src/Assembly-CSharp/form/BootForm.cs` | `4dc7148f38f885e7c9e9af50d22f507b7e3eefc4bc14e898c8abd9c1fcb566f1` |
| 21 | `src/Assembly-CSharp/form/DebugForm.cs` | `eb712bc99e60bba79288d3b53c7acbc33124c3f3e467c1ec391189d6fe93ce51` |
| 22 | `src/Assembly-CSharp/form/FormManager.cs` | `d75a0cbac4234f33da896eb2e9f227e496b74c0c0194b6793144da060938f791` |
| 23 | `src/Assembly-CSharp/form/GameForm.cs` | `8811a141be8d13343e88fb066611bf96de87a01eb02401448e787ba971b4d885` |
| 24 | `src/Assembly-CSharp/form/MyFormBase.cs` | `08c51be89bf90ebfafc6ee53356fa5da1a435862812642ab4c8aa81bd9c9ed91` |
| 25 | `src/Assembly-CSharp/form/RankForm.cs` | `04605a2d57d51ba71c45356a8d7a6f9e4b766cd93819f6671d3907b4e4c8ac76` |
| 26 | `src/Assembly-CSharp/form/SoccerForm.cs` | `e5c27fa053d1fa15ff50acf6cdc5b6db2c61ef976d32e1480bbda79d38e18323` |
| 27 | `src/Assembly-CSharp/form/SpForm.cs` | `a1e9e00a82d89e76dd9d6474f9d9f7c2dccd9f7bd0177445d7364f9041420d09` |
| 28 | `src/Assembly-CSharp/form/SubForm.cs` | `06ceb87516cb1066e25643b40cff06be39c3828dc2b20b8101172f021d59306c` |
| 29 | `src/Assembly-CSharp/form/TitleForm.cs` | `f8adc8d88c826c2fbbd2ea5a704fa167fcfc07aaeb5869fa0fba8cd5b5ef5861` |
| 30 | `src/Assembly-CSharp/game/Ball.cs` | `89a7d6fdcaeb9d1a53d7a562bee8d19384f6281a069aac9e60df41b8e71bb2e0` |
| 31 | `src/Assembly-CSharp/game/Camera.cs` | `57ba7b8b47b08c71b7fc628a876442970788517b087ceff53cf7eea35d40d2d0` |
| 32 | `src/Assembly-CSharp/game/DelayEvent.cs` | `308beae0798cf9c1f61f1af3defdfc90ddf78dac9e89a4787826b0e1022b2c3d` |
| 33 | `src/Assembly-CSharp/game/GameManager.cs` | `087fa5b935cf4d332cc48807916b7e9bb64890820af74b86829ece1d3e52984c` |
| 34 | `src/Assembly-CSharp/game/GameUtil.cs` | `1bae0c391830e0e6b6747cf737e53166240e0d5c931d822c81e0394bc21b65bd` |
| 35 | `src/Assembly-CSharp/game/Map.cs` | `ffab07908dac44a8c8c4620e3fbfd2a8d50f8b083460f61b3e88e47c1bb791d5` |
| 36 | `src/Assembly-CSharp/game/MapMain.cs` | `9c91433ec4c078067cc601f0a8f2f073f06cb4116ed9806f8f5ddee30a1d1e35` |
| 37 | `src/Assembly-CSharp/game/MapObject.cs` | `04a287bd44ee6e0f8695181354f73f44cd711c80e35eb547b1521bbcf461069c` |
| 38 | `src/Assembly-CSharp/game/Player.cs` | `6822750ad23bb7a0a81f812814908be1bbc099f3cb7f95f23b27d9c64e738d1a` |
| 39 | `src/Assembly-CSharp/game/TArea.cs` | `84f78f61794cf0de64bd640702af04adf166126596244fdf3fde3ed0c2c8353d` |
| 40 | `src/Assembly-CSharp/game/TBall.cs` | `b3b7addd6df4b58510b844ff876cb3b110fff849e752671f719d5bc6d9dd7438` |
| 41 | `src/Assembly-CSharp/game/TGroupArea.cs` | `d9cfc61c935b89249cec8d948dc77620ffa9bc5261b95724f4f913a690e5c3ba` |
| 42 | `src/Assembly-CSharp/game/TPlayer.cs` | `067b98fa3e1d37cb348c3e4042979842f62d4a86c844e1e5811622bea71ae3a0` |
| 43 | `src/Assembly-CSharp/game/Team.cs` | `98df78d3c9a8126efcc278ebd872ceb52fc2336bfc3d7eb4781f6eb7dd9f787f` |
| 44 | `src/Assembly-CSharp/game/Tenant.cs` | `c6d8fac7fee1664042cba58d6df98287338e8f7d111395044c85e60e7cd19f2e` |
| 45 | `src/Assembly-CSharp/game/UserData.cs` | `7513c50d32a2bddf9a9c215f1695574c03b03584055aeaf8c503fd204fbbb3a1` |
| 46 | `src/Assembly-CSharp/main/AppData.cs` | `862c77226bd3b844b5130df3217f8932f02c5a984c97e67fc6efe1f72ef260ef` |
| 47 | `src/Assembly-CSharp/main/Main.cs` | `ac5571d0fd7f076531391709d81d01376d5e4e1d2902a316a90cd4dd14a1d49e` |
| 48 | `src/Assembly-CSharp/main/WebBox.cs` | `dc72d92418951f6ecd28adb89304b0580a8c72ac98c40cc2999615de1063ce09` |
| 49 | `src/Assembly-CSharp/native/AppPlugin.cs` | `6b3f41ec43f966e0784dbf537becb93740d7ada21abab4ca91d4399631fb7118` |
| 50 | `src/Assembly-CSharp/BuildInfos/BuildInfo.cs` | `bb17bcc61d48230b0d9f5471c0873d0c705bcd34142455c816f8eeddd88fd7d9` |
| 51 | `src/Assembly-CSharp/native/WebBoxPlugin.cs` | `a58fc5a50f683c49d48ee7de16ed93a6dcf92839d83d3ffe0959f29418c71691` |
| 52 | `src/Assembly-CSharp/panel/MainPanel.cs` | `694a9f627d23856312dd2a496f0b9a14306f36b063a27f0b2f1226f008fcecb7` |
| 53 | `src/Assembly-CSharp/surface/GamePad.cs` | `fe8e0dcd3bdae5fd58f481fd8f987cc77c30ee65119b849c9165e958fbcc1796` |
| 54 | `src/Assembly-CSharp/surface/GameView.cs` | `fca59391180648a5725a40d4fa191ff419eb826087e19b44b60d0429d924b7b7` |
| 55 | `src/Assembly-CSharp/surface/SurfaceManager.cs` | `208504f20ad8f226da2f4cef3e5218005494dd14825ae2e0012243eec542b57c` |
| 56 | `src/Assembly-CSharp/surface/TouchEffect.cs` | `5e329b04882976772ea607bab19321b41f0feaa6068a9bf90c2dd3c9d3b66fdf` |
| 57 | `src/Assembly-CSharp/surface/TouchEffectManager.cs` | `86a8dafef273d3c87d48804eeebfc0962183c8e79751545ff1bfa69987a2db84` |
| 58 | `src/Assembly-CSharp/system/KairoService.cs` | `e3d8cf223d475e52d69174943beda5c27915d4e22225f295799865bfd7b75bfe` |
| 59 | `src/Assembly-CSharp/system/KairoText.cs` | `885764eaa5ee4194675162bafa55c11ff91143efff512bc444b036c2cc236377` |
| 60 | `src/Assembly-CSharp/system/billing/MyBilling.cs` | `eb618e2d683329fe09b4f713fe189fe3a685899305d903a1b7564857daf6c28b` |
| 61 | `src/Assembly-CSharp/system/billing/ui/ApplyTask.cs` | `4b82311ae314e49e73dfb88b2ba394e9358fc720fe792eeaaa78001658902577` |
| 62 | `src/Assembly-CSharp/system/billing/ui/BillingItem.cs` | `8836404274d24b05e700ba8ba8b163becbf7b64149180833264d1e1eac1c2ab7` |
| 63 | `src/Assembly-CSharp/system/billing/ui/BillingManager.cs` | `9ec71a52d402b70f13ec45ba6fac88494746a420c2e2738065792afad1db1808` |
| 64 | `src/Assembly-CSharp/system/billing/ui/PurchaseLogView.cs` | `17dc4a5fea634e8ed9f98d599d8f66923f9ed9a677db425e98a7c0b78b58eaf5` |
| 65 | `src/Assembly-CSharp/system/billing/ui/PurchaseRecord.cs` | `621755a071db827e85089f6661803187ea23f5a7c630e30e45315cd132a13093` |
| 66 | `src/Assembly-CSharp/system/billing/ui/Receipt.cs` | `3118a2f025c0f181e51e282368f26afd4278719ce04eb4bd20a4e24f772944f7` |
| 67 | `src/Assembly-CSharp/UnityEngine/Purchasing/IStoreListener.cs` | `fae6d27bf37bad07c856e30ec7bb78edc1eac33b5454a5480dc21a95bb2b3a05` |
| 68 | `src/Assembly-CSharp/UnityEngine/Purchasing/IStoreController.cs` | `b84ddeccb9cbdce5a3d09bcc961ca794f307992a28fc4533bff9a61ca56c6ba1` |
| 69 | `src/Assembly-CSharp/UnityEngine/Purchasing/IExtensionProvider.cs` | `fc5d0f8f1e8a116e34a50bedcc20ac31c2de4471c4c2a1dee5e4dd9e1f5824f4` |
| 70 | `src/Assembly-CSharp/UnityEngine/Purchasing/ProductType.cs` | `011e53f22e8fb7c5484f2da8c337a3868e26da15eef6175dacb5159f0144e9c2` |
| 71 | `src/Assembly-CSharp/UnityEngine/Purchasing/Product.cs` | `dc65496b832ecba47b3008547686edeca875e60a122c477ebd2a7ef2d3d8d32c` |
| 72 | `src/Assembly-CSharp/system/billing/ui/StoreBase.cs` | `9924c795d5cf90f04695fde5890d7af4f146cc6bbe619fae4253cbf42e2ffe02` |
| 73 | `src/Assembly-CSharp/system/billing/util/IabHelper.cs` | `b2dbbd66a672d8cac0d1175b6579efbfc0e99491c5e7fdc2ee4c322bb289c451` |
| 74 | `src/Assembly-CSharp/system/billing/util/Inventory.cs` | `1dd5f79c7b974bf930d45835dfb3f8eb8e2fe067011e4a03d7db489d4adf764a` |
| 75 | `src/Assembly-CSharp/system/billing/util/Json.cs` | `ff001fc868a9a3d553a9e778746a7002066b3a9529bea50f5448e9ce316cbb0a` |
| 76 | `src/Assembly-CSharp/system/billing/util/Purchase.cs` | `fb8bf2b8d62212b5a426e7e475db5f749d0347b817e4a4f93b5a874af9849bb7` |
| 77 | `src/Assembly-CSharp/system/billing/util/SkuDetails.cs` | `939dcedce19386f95d7269e4df871f163e416d727cd4edf41dda041838361c07` |
| 78 | `src/Assembly-CSharp/system/form/BillingForm.cs` | `b2a9ede35ac787fe073275f3e8587238e424f3f0cda62471fa5dadc436e6ddab` |
| 79 | `src/Assembly-CSharp/system/form/ConnectForm.cs` | `4c6dc6e086ae48f40764eea42640cdfa8f38ec3bc7d94aa21dda7d95d8757051` |
| 80 | `src/Assembly-CSharp/system/form/FriendForm.cs` | `633c53118f32e567c64b2dbbebf935533597ec4c33fd6205da173d9e1e36c512` |
| 81 | `src/Assembly-CSharp/system/form/HttpForm.cs` | `1e47dfeec8d29f611e06b5ddee14de90e542291fc7f56f9c506438f3d01e0fd6` |
| 82 | `src/Assembly-CSharp/system/form/LanguageForm.cs` | `dd2aad304ae8637c68549934df27feef12755f0cd696f3541eb74f5cddb17f8f` |
| 83 | `src/Assembly-CSharp/system/form/LineupForm.cs` | `8c9e5f8c1738aea0147a068d9882f7b276df982103c9c91ca3311d3979b38d3e` |
| 84 | `src/Assembly-CSharp/system/form/MatchForm.cs` | `905a63b1886960e70b0e429b630c54fbcb30e518e9c4617a60e17d817624270b` |
| 85 | `src/Assembly-CSharp/system/form/RestoreForm.cs` | `f0e36c3c03afcd9deb0296212c60a2b8204115da14a9fb920dd1560f203aada4` |
| 86 | `src/Assembly-CSharp/system/friend/MyFriend.cs` | `2f1039afe97324e32c4faf1f3c8a9475cca4a50f96a2cec0b2ed89184b2d30c0` |
| 87 | `src/Assembly-CSharp/system/friend/ui/Friend.cs` | `3d5c5fae94eb8b18b06707ba5192c3b8bcef5685ce7009ccad7b3c242a4d9a98` |
| 88 | `src/Assembly-CSharp/system/friend/ui/FriendData.cs` | `42570968d493799b13518fa882e26687628b94b296bdd23accadba1003f01d16` |
| 89 | `src/Assembly-CSharp/system/friend/ui/FriendPlayTimeData.cs` | `04b7b45f260c7f9b5750807b400743975576bd13eeecdf18771b666817a3d791` |
| 90 | `src/Assembly-CSharp/system/lineup/MyLineup.cs` | `20c6be26fce2c4b8e1ea59569aa31e17f5be0d799640d2a8bbae3b42a0670ee5` |
| 91 | `src/Assembly-CSharp/system/notification/MyNotify.cs` | `0471c9873db577e637e84b218320cb4237e68ad951907cceb7e6a37a020d6062` |
| 92 | `src/Assembly-CSharp/system/panel/InputPanel.cs` | `e11bbc09ab9cf547834d32c3914b4c2fccbf01673f1d354008c93e680d5c9e8c` |
| 93 | `src/Assembly-CSharp/system/surface/GameBanner.cs` | `a30d07427672606c4e3368ddd75aecdf543e04a9cb468bea1ddbe3df25f19917` |
| 94 | `src/Assembly-CSharp/system/tapjoy/MyTapjoy.cs` | `3f5d41c07920b2de7e61fed48d8480547666b2bcfa6e29fd28054f750303f44a` |
| 95 | `src/Assembly-CSharp/TJPlacement.cs` | `9d1078f1b6b44d40e7c28e2ca6b25df297dbb702bb59f53481c0c2c7db6555a6` |
| 96 | `src/Assembly-CSharp/system/tapjoy/ui/Tapjoy.cs` | `326210f2849f6bde65f1abb67c4fdb2c399248e761522911753028c9758875ac` |
| 97 | `src/Assembly-CSharp/AssemblyInfo.cs` | `249d1af9956fd21e20afd63daf7347aa18127a5319d84222af9dfba0e08f8f94` |

Arquivo de ordem: `config/compile-order/Assembly-CSharp.txt`, SHA-256 `19ccbb241f0115342807139854135cde7e9cec35ebb19567a2815a0d6b3a670a`. O builder acrescenta AssemblyInfo.cs se necessário, conforme a lista acima.

## Componentes do pacote Roslyn fixado

| Arquivo | SHA-256 |
|---|---|
| `csc.exe` | `53b693536e8a86f35d05d2165ff9b3e0aeaa499ecfd8135a9f99ab4404ca86c3` |
| `csc.exe.config` | `0501cae075d0ab6002dbb47eeb92e61935b5e85fb40d9a7c3bf6e6f36ab53bfa` |
| `Microsoft.CodeAnalysis.dll` | `4d9c797aa6a62f850d8417f20bfc213844a4777c3cd7e75e744225b5812fb781` |
| `Microsoft.CodeAnalysis.CSharp.dll` | `1800cb4d49698d4f908338d4bb443c10f7549b9be907c7c70a2e4e6b1e17693b` |
| `System.Collections.Immutable.dll` | `51d4f7c607d5a2f636671d4ad1dc756a272018898267145dbfffc5bdb09df1a1` |
| `System.Reflection.Metadata.dll` | `e538275e53148ad564061dd9566d6e13bd820a50645ea415d58784857a625852` |

## Evidências e histórico

O [histórico anterior](notes/toolchain-history.md) preserva medições parciais e hipóteses de rodadas anteriores; elas não são a receita vigente. [Fechamento Kairo](notes/round12-kairo-byte-closure.md), [fechamento Assembly-CSharp](notes/round12-app-byte-closure.md), [ajustes de emissão](notes/kairo-metadata-emission.md) e [manifesto final](../reports/reconstruction-closure.json) registram as provas.

REA 6.5.0 é o parser estático usado nas evidências PE/CLI, sem carregar o jogo. A comparação independente de corpos e metadados usa ProbeInspector/MetadataInspector com Mono.Cecil do Unity. Nesta sessão o MCP respondeu e anunciou inspect_managed_artifact/inspect_managed_members disponíveis; o doctor geral acusou drift de registros/skill e provedores opcionais ausentes. Isso não exigiu instalação nem mudou a receita de compilação. Nenhum método Java foi decompilado e nenhum APK foi executado.
