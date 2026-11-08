# APK final — procedimento para execução futura

**Este documento não registra execução. Nenhum APK foi montado, assinado, instalado ou executado na rodada de encerramento. As DLLs finais ainda não foram testadas em aparelho.** A reconstrução C# está concluída; a futura montagem produz um artefato separado, com chave própria de teste.

## Entradas e critérios de aceitação

O original autenticado (`original/app.apk`, SHA-256 `d557af6b4bc7748605a39ed3548821f6763c5773a12432a3fed6e8c413b211e8`) tem **1.218 entradas**, com sequência, método de compressão e alinhamento individual registrados em `original/zip-layout.json`. O contrato público [round11-apk-contract.json](../reports/round11-apk-contract.json) contém a lista integral ordenada das 1.218 entradas, seus métodos, tamanhos, SHA do payload comprimido e resto do offset de dados módulo 4096. Esse contrato é suficiente para conferir o layout em um clone; a tabela privada detalha também cabeçalhos e atributos. O template de montagem é a cópia própria do APK, nunca um reempacotamento por apktool.

Para cada ordinal, manter o nome, método ZIP, flags, atributos, timestamps e o mesmo `data_offset % 4096`, inclusive restos não zero módulo 4. Recalcular CRC, tamanhos, data descriptors quando presentes, offsets centrais e EOCD de forma coerente. Não executar um zipalign indiscriminado depois: ele poderia mudar as congruências não uniformes que fazem parte deste critério. Identidade dos payloads não significa identidade posicional de todos os cabeçalhos ZIP, pois DLLs e assinatura mudam de tamanho.

| Entradas | Quantidade | Comparação |
|---|---:|---|
| Todos os demais arquivos | 1213 | Payload **comprimido** integralmente idêntico ao original; comparar também conteúdo descomprimido/CRC |
| `assets/bin/Data/Managed/KairoLibrary.dll`, `assets/bin/Data/Managed/Assembly-CSharp.dll` | 2 | Bytes descomprimidos idênticos às saídas autenticadas; contra as originais, somente os seis campos comprovados podem diferir |
| `META-INF/MANIFEST.MF`, `META-INF/CERT.SF`, `META-INF/CERT.RSA` | 3 | Novos portadores da assinatura própria de teste, verificáveis; não copiar a assinatura original |

Não excluir genericamente todo META-INF. Um eventual APK Signing Block seria também assinatura; o original fornecido não contém esse bloco. O procedimento abaixo escolhe assinatura **v1 JAR/SHA-256/RSA3072**, já usada no teste anterior. Assinatura e confiança no certificado não são equivalência à identidade do desenvolvedor original. O resultado completo não terá o hash do APK original.

As únicas exceções de DLL, em offsets absolutos base zero, são:

| DLL | Campo | Offset | Bytes |
|---|---|---:|---:|
| KairoLibrary | COFF.TimeDateStamp | 136 | 4 |
| KairoLibrary | Module.Mvid | 527096 | 16 |
| KairoLibrary | IMAGE_DEBUG_DIRECTORY.TimeDateStamp | 550520 | 4 |
| KairoLibrary | CodeView.RSDS.PdbGuid | 550548 | 16 |
| Assembly-CSharp | COFF.TimeDateStamp | 136 | 4 |
| Assembly-CSharp | Module.Mvid | 893568 | 16 |

Não modificar esses campos para fechar hashes. O PDB original tem caminho reproduzido na compilação; ele não é exceção. Preservar as duas DLLs exatamente como foram produzidas e verificadas, inclusive seus campos variáveis.

## Preparação futura

1. Seguir `docs/toolchain.md`, compilar e executar a verificação integral e a comparação literal de ambas as DLLs; exigir 5.245 matched e zero regressões/resíduos. Congelar e registrar os dois arquivos, recibos, manifests e SHA. Não recompilar entre congelamento, montagem e teste.
2. Validar as 1.218 entradas do original contra o contrato público. Não usar DLLs do antigo snapshot `build/round10/baseline`: elas não são as finais. As canônicas ficam em `build/KairoLibrary-roslyn-1.1.0/KairoLibrary.dll` e `build/Assembly-CSharp-mono-smcs/Assembly-CSharp.dll`.
3. Verificar disponibilidade de JDK com keytool/jarsigner e ADB/emulador ou dispositivo suportado. Se faltar dispositivo/emulador, registrar bloqueio antes de tentar o teste; a instalação de ferramentas exige nova decisão do usuário. No teste anterior foi usado Microsoft JDK 21.0.12.101. Nenhuma aquisição é executada por este documento.
4. Usar uma pasta **nova** sob `build/apk-final/`; nunca sobrescrever evidências anteriores. Gerar chave RSA 3072 de teste local, senha aleatória e certificado autoassinado, registrar SHA DER. Guardar chave/senha somente em `build/`, fora do Git. Nunca usar a chave original nem tentar imitar seu certificado.

## Montagem e assinatura futuras

Fixar Python 3.12.10/zlib runtime **1.3.1**. Nas duas DLLs, usar DEFLATE bruto **level=6, wbits=-15, memLevel=8, strategy=Z_DEFAULT_STRATEGY**, uma chamada compress com o arquivo integral e flush **Z_FINISH**. Isso reproduziu todos os bytes comprimidos das DLLs originais: 248.092 bytes Kairo e 321.171 bytes App. As DLLs reconstruídas podem ter tamanhos comprimidos diferentes por causa dos seis campos variáveis; são julgadas descomprimidas. O encoder histórico exato permanece UNKNOWN.

**Não executar diretamente o main de `tools/build_test_apk.py`: ele fixa a baseline antiga e nível 9.** As funções de leitura/escrita ZIP e comparação desse módulo podem ser reutilizadas conforme o trecho abaixo, que seleciona recibos atuais e substitui explicitamente a função de compressão por nível 6. Isto é uma receita a salvar como `build/apk-final/future_recipe.py` e executar **somente quando a futura rodada for autorizada**, na raiz do repositório. Não foi executada neste encerramento.

```python
from pathlib import Path
import hashlib, json, os, secrets, shutil, sys, zlib
sys.path.insert(0, str(Path('tools').resolve()))
import build_test_apk as pack
from apply_il_rules import receipt_is_fresh

root = Path.cwd().resolve()
folder = root/'build/apk-final/release-test-01'
assert not folder.exists(), 'Use a new folder'
assert zlib.ZLIB_RUNTIME_VERSION == '1.3.1'
original = root/'original/app.apk'
contract = json.loads((root/'reports/round11-apk-contract.json').read_text())
assert pack.sha(original.read_bytes()) == contract['apk_sha256']
rows, _ = pack.parse_archive(original)
assert len(rows) == len(contract['entries']) == 1218
for row, expected in zip(rows, contract['entries']):
    assert row['info'].filename == expected['path']
    assert row['info'].compress_type == expected['compression_method']
    assert row['data_offset'] % 4096 == expected['data_offset_mod4096']
    assert pack.sha(row['payload']) == expected['compressed_sha256']

replacements = {}
fields = {'KairoLibrary': [(136,4),(527096,16),(550520,4),(550548,16)],
          'Assembly-CSharp': [(136,4),(893568,16)]}
for entry, assembly in pack.DLL_NAMES.items():
    receipt = json.loads((root/('build/verification-'+assembly+'.json')).read_text())
    assert receipt_is_fresh(root, receipt)
    assert receipt['counts']['matched'] == (3287 if assembly == 'KairoLibrary' else 1958)
    assert not any(receipt.get(k) for k in ('regressions','body_regressions','type_regressions'))
    data = (root/receipt['build_artifact']).read_bytes()
    old = (root/'original/extracted'/entry).read_bytes()
    assert pack.sha(data) == receipt['build_sha256']
    assert pack.sha(old) == receipt['original_sha256']
    allowed = {i for start, size in fields[assembly] for i in range(start,start+size)}
    assert len(data) == len(old)
    assert all(x == y or i in allowed for i,(x,y) in enumerate(zip(old,data)))
    replacements[entry] = data

def compress6(data, method):
    if method == 0:
        return data
    assert method == 8
    c = zlib.compressobj(6, zlib.DEFLATED, -15, 8, zlib.Z_DEFAULT_STRATEGY)
    return c.compress(data) + c.flush(zlib.Z_FINISH)

pack.compressed = compress6
keytool, jarsigner = shutil.which('keytool'), shutil.which('jarsigner')
assert keytool and jarsigner
folder.mkdir(parents=True)
unsigned = folder/'unsigned.apk'
signed = folder/'jarsigned.apk'
final = folder/'final-test.apk'
pack.write_archive(original, unsigned, replacements, pack.SIG_NAMES)
shutil.copyfile(unsigned, signed)
password = secrets.token_urlsafe(24)
(folder/'test-password.txt').write_text(password, encoding='ascii')
env = dict(os.environ, PLS_TEST_KEY_PASSWORD=password)
key = folder/'test-signing.p12'
cert = folder/'test-public.der'
pack.run_logged([keytool, '-genkeypair', '-alias', 'pls-test', '-keyalg', 'RSA',
    '-keysize', '3072', '-sigalg', 'SHA256withRSA', '-validity', '3650',
    '-storetype', 'PKCS12', '-keystore', key, '-storepass:env', 'PLS_TEST_KEY_PASSWORD',
    '-dname', 'CN=Pocket League Story Reconstruction Test,OU=Testing,O=Local Project,C=BR',
    '-noprompt'], folder/'key.log', env)
pack.run_logged([keytool, '-exportcert', '-alias', 'pls-test', '-keystore', key,
    '-storepass:env', 'PLS_TEST_KEY_PASSWORD', '-file', cert], folder/'cert.log', env)
pack.run_logged([jarsigner, '-keystore', key, '-storepass:env', 'PLS_TEST_KEY_PASSWORD',
    '-sigfile', 'CERT', '-sigalg', 'SHA256withRSA', '-digestalg', 'SHA-256',
    signed, 'pls-test'], folder/'sign.log', env)
import zipfile
with zipfile.ZipFile(signed) as z:
    replacements.update({name: z.read(name) for name in pack.SIG_NAMES})
# V1 authenticates entry content: after signing, restore original order/alignment.
pack.write_archive(original, final, replacements)
output = pack.run_logged([jarsigner, '-verify', '-verbose', '-certs', final],
                         folder/'signature-verification.log')
assert 'jar verified.' in output.lower()
entries = pack.verify_repack(original, final, replacements)
proof = dict(operation='FUTURE_FINAL_APK_TEST', entries=entries, entry_count=len(entries),
    original_apk_sha256=pack.sha(original.read_bytes()), final_apk_sha256=pack.sha(final.read_bytes()),
    dll_sha256={name:pack.sha(data) for name,data in replacements.items() if name in pack.DLL_NAMES},
    test_certificate_der_sha256=pack.sha(cert.read_bytes()),
    compression_level=6, zlib_runtime=zlib.ZLIB_RUNTIME_VERSION,
    signature_verified=True, DLL_postcompile_writes=False, device_tested=False)
(folder/'apk-final-proof.json').write_text(json.dumps(proof,indent=2)+'\n')
```

Essa receita reaproveita o writer já exercitado para manter os restos dos offsets módulo 4096: acrescenta padding em extra field local, preserva payloads opacos e data descriptors, recalcula o diretório central. A assinatura v1 é obtida de um intermediário novo e inserida no template sem copiar a original. Verificar novamente assinatura, as 1.218 linhas da comparação e todos os SHA antes de instalar. Não declarar sucesso só porque jarsigner aprovou.

## Instalação e roteiro funcional futuros

Usar um emulador Android **x86 com suporte ao aplicativo antigo** ou aparelho com ARMv7 disponível. O teste anterior foi em S22 Ultra SM-S908E, Android 16/API 36, que executou ARMv7. Um sistema somente ARM64 sem suporte a ARMv7 não serve para concluir falha do jogo.

Consultar o ADB e selecionar explicitamente o serial. Se o pacote `net.kairosoft.android.soccer_en` já estiver instalado, verificar a assinatura: uma chave diferente impede atualização. **Não desinstalar nem apagar saves automaticamente**. Usar ambiente isolado ou pedir decisão sobre preservar os dados antes de uma operação destrutiva. Autorizações USB e confirmação específica de instalação podem exigir interação do usuário. Não desativar proteções globais.

Comandos para a futura rodada, depois de resolver essa situação:

```powershell
$adb = 'C:\caminho\platform-tools\adb.exe'
$serial = 'SERIAL_DO_DISPOSITIVO'
$package = 'net.kairosoft.android.soccer_en'
& $adb devices -l
& $adb -s $serial shell getprop ro.product.cpu.abilist
& $adb -s $serial shell pm path $package
& $adb -s $serial install 'build/apk-final/release-test-01/final-test.apk'
# Somente se o sistema bloquear exclusivamente o targetSdk antigo:
# & $adb -s $serial install --bypass-low-target-sdk-block 'build/apk-final/release-test-01/final-test.apk'
& $adb -s $serial shell cmd package resolve-activity --brief $package
# Usar a atividade realmente resolvida no comando seguinte:
& $adb -s $serial shell am start -W -n 'PACOTE/ATIVIDADE_RESOLVIDA'
& $adb -s $serial shell pidof $package
# Substituir PID_DO_JOGO; não limpar buffers globais:
& $adb -s $serial logcat -d -v threadtime --pid=PID_DO_JOGO > 'build/apk-final/release-test-01/logcat-start.txt'
```

Registrar versão do aparelho/emulador, API, ABI, serial em evidência privada, horário UTC, SHA do APK/certificado/DLLs e cada comando/retorno. Capturar logcat filtrado por PID desde a abertura, novamente em cada marco; se o processo reiniciar, registrar o novo PID e manter o log anterior. Preservar linhas originais e contexto, sem filtrar só erros. Evidências devem ficar fora do Git quando contiverem dados do aparelho.

1. Confirmar abertura e ausência de crash; registrar screenshot e log.
2. Chegar ao menu e carregar/iniciar uma equipe; registrar operações e save utilizado.
3. Iniciar uma partida e observar primeiro tempo, intervalo, segundo tempo, fim, placar e retorno ao fluxo normal; registrar screenshot/log em cada marco. Não apenas deixar a tela inicial aberta.
4. Consultar logs por `Exception`, `Unhandled`, `FATAL EXCEPTION`, `AndroidRuntime`, `SIGSEGV`, falhas Mono, `DllNotFoundException`, `MissingMethodException`, `TypeLoadException` e erros Unity. Relatar ocorrências completas e distinguir avisos.
5. Gerar relatório dizendo separadamente **abre / menu / partida completa / exceções observadas / limitações**. Ausência de erro nos buffers capturados não prova caminhos não exercitados, cobrança, anúncios ou DEX.

A aceitação funcional é adicional: não substitui a comparação byte a byte nem muda estados de métodos. O teste de baseline [da rodada 10](notes/round10-functional-test.md) continua histórico até o procedimento final ser realmente executado.
