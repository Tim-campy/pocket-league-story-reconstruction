# Toolchain e evidências

## Referência e motor

APK: `original/app.apk`, SHA-256 `d557af6b4bc7748605a39ed3548821f6763c5773a12432a3fed6e8c413b211e8`, 28.820.227 bytes. Os hashes das 1.218 entradas estão em `config/inventory.csv`, e a ordem, compressão e alinhamento em `original/zip-layout.json`.

Unity **5.3.4f1** aparece como string terminada em zero no offset `0x14` de:

| Arquivo | SHA-256 |
|---|---|
| `assets/bin/Data/globalgamemanagers` | `0ff7354b578f4a2c03e92f0cd22e20e5bf38df0513e58865bc425788bc7824df` |
| `assets/bin/Data/Resources/unity_builtin_extra` | `5bfaebf3d445f3fa935c285906b532529a5ea1441a073bd2ba5bc1a268a9f577` |

O instalador foi obtido pelo link Windows da [página oficial do Unity 5.3.4f1](https://unity.com/releases/editor/whats-new/5.3.4f1). SHA-256 do instalador: `d56600a91172f5d0d02bdf46d6d904b043e79b50c6da2488212ef4f417b57bcc`, 264.190.104 bytes. O Windows verificou a assinatura como `Valid`, emitida para **Unity Technologies SF**. Registro: `analysis/downloads/unity-5.3.4f1-download.json`.

Instalado em `C:\Users\l_abn\AppData\Local\Programs\Unity\5.3.4f1`, usando `/S /D=...`, conforme o [manual oficial da versão 5.3](https://docs.unity3d.com/530/Documentation/Manual/InstallingUnity.html). O executável do editor informa a versão de arquivo `5.3.4.16628561`. O estado e os hashes dos componentes usados estão em `config/toolchain.json` e `analysis/unity-installation.json`.

O compilador padrão incluído respondeu como **Mono C# compiler 2.6.5.0**, tanto em `gmcs.exe` quanto em `smcs.exe`. O MonoBleedingEdge incluído fornece **mcs 3.4.0.0**. Uma DLL com fonte sintético de dois métodos foi compilada com sucesso pelo gmcs; esse teste demonstra que o compilador funciona, sem validar correspondência com métodos do jogo. O editor não foi ativado nem aberto interativamente.

## REA e leitura de assemblies

O REA **6.5.0** desta instalação analisa PE/CLI, metadados e CIL. Ele é a fonte principal das duas DLLs alvo. As evidências completas foram preservadas em `analysis/rea-evidence-2026-11-05.json`, com hashes e MVIDs. Essa análise não executou o jogo.

`tools/rea_metadata.mjs` complementa o inventário usando os leitores já instalados do REA 6.5.0. O adaptador verifica a versão e registra os hashes de seus módulos em `analysis/metadata/`. Os scripts Python usam somente a biblioteca padrão para catalogar, organizar e conferir hashes.

`rea doctor` respondeu com saída 1: Ghidra está disponível, mas há avisos sobre outros providers, skill local desatualizada e registros de clientes. O MCP ativo respondeu como REA 6.5.0. Os avisos não foram tratados como falha do parser .NET, que já produziu as evidências. Nenhuma configuração global foi reparada nesta preparação.

## Compiladores por assembly

| Item | Assembly-CSharp | KairoLibrary |
|---|---|---|
| Referência | `3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949` | `bb553d341eecd9b900f772486bf4a5c289aa2f09566d9d402a3eb70268f4b3ee` |
| MVID | `48ce9f89-73d2-40be-bb24-22c807c04106` | `b564fa31-554d-4b48-a8c0-a0b41bea2550` |
| Pistas observadas | 8 nomes de tipos contendo `AnonStorey` | 76 contendo `DisplayClass`, 83 contendo `<>c`, 2 contendo `d__` |
| Família inferida | Compatível com geração mcs; hipótese | Compatível com geração Microsoft/Roslyn; hipótese |
| Compilador e versão exatos | UNKNOWN | UNKNOWN |
| Otimização, debug e versão C# | UNKNOWN | UNKNOWN |
| Símbolos definidos e perfil de API | UNKNOWN | UNKNOWN |
| Ordem de referências e fontes histórica | UNKNOWN | UNKNOWN |
| Métodos de sonda validados | 4 com Mono smcs/gmcs 2.6.5 | 4 com Roslyn 5.8; build completo com 5.8 e pacote 1.3.2 |

As pistas vêm dos nomes originais de TypeDef, com a hierarquia NestedClass resolvida; a lista completa está em `config/compiler-hints.json`. Elas não autorizam selecionar um compilador sem testar. Instalar o editor não demonstra que KairoLibrary foi compilada por ele.

## Ferramentas disponíveis e pendentes

| Ferramenta | Evidência local | Uso |
|---|---|---|
| Python | 3.12.10 | Cadastros, hashes e pré-verificação |
| Node.js | 24.19.0 | REA e adaptador de metadados |
| Git | 2.55.0.windows.3 | Histórico local |
| Java | Microsoft OpenJDK 21.0.12.1 | Runtime disponível para ferramentas Java |
| Ghidra | 12.1.4, informado pelo REA | Não necessário para decompilar as DLLs gerenciadas |
| .NET | Runtime 10.0.10 incluído no VS; também 6.0.36 no sistema | Runtime do driver ILSpy; não depende de SDK adicional |
| C# do Windows | `csc.exe` 14.8.10001.0, C# 5 | Não selecionado como compilador histórico |
| uv / ud | 0.12.23 / 0.1.0 | Instalados; não necessários aos cadastros atuais |
| ILSpy | 9.1.0.7988, engine incluído no Visual Studio 18 instalado | Driver `tools/DecompileAssembly.cs`, C# 6, sem símbolos, resolver estrito |
| Mono.Cecil | **0.9.6.0**, incluído em `Editor/Data/Managed/Mono.Cecil.dll` | Disponível para a futura comparação independente e resolução de metadados |
| monodis/ildasm e dnlib/dnfile | Não localizados como comandos/bibliotecas utilizáveis na inspeção inicial | O arquivo `monodis` sem extensão incluído pelo editor ainda não foi validado como executável Windows |
| apktool/aapt2/zipalign | Não localizados no PATH | Remontagem futura; Python cobre a inspeção ZIP atual |

O inventário de caminhos e versões consultados está em `analysis/environment.json`. Ausência no PATH ou nos caminhos consultados não prova ausência em todo o computador. Ferramentas adicionais só serão instaladas com autorização.

## Exceções de comparação

Foram registradas quatro exceções comprovadas para o candidato KairoLibrary, pacote Roslyn 1.3.2, com `/debug:pdbonly /deterministic-`:

| Campo | Localização semântica | Prova |
|---|---|---|
| MVID | GUID referenciado pela linha Module | 16 bytes mudaram entre builds idênticos |
| Timestamp COFF | Cabeçalho PE/COFF TimeDateStamp | Um byte mudou nesta repetição; campo tem quatro bytes |
| Timestamp de depuração | TimeDateStamp da entrada IMAGE_DEBUG_DIRECTORY | Um byte mudou; campo tem quatro bytes |
| GUID do PDB | GUID do registro CodeView RSDS | 16 bytes mudaram |

Evidência: [reports/KairoLibrary-repeatability.json](../reports/KairoLibrary-repeatability.json), dois builds completos dos mesmos fontes, referências, opções e caminho de saída; 34 bytes diferentes, nenhum fora desses campos. A autorização para registrá-los veio da solicitação do usuário após examinar essa prova. Exceções declaradas em `config/toolchain.json`; **nenhum normalizador de assembly foi aplicado**, nenhum campo adicional está excluído e nenhum binário foi editado. Offsets não são fixados para futuros builds: os campos terão de ser encontrados pela estrutura PE/CLI. A prova vale para esse candidato/opções; o determinismo histórico do original continua UNKNOWN.

A assinatura do APK é a exclusão final prevista pelo AGENTS.md. O inventário observou `META-INF/MANIFEST.MF`, `META-INF/CERT.SF` e `META-INF/CERT.RSA`; não observou a magic do APK Signing Block imediatamente antes do diretório central. A implementação da comparação final deverá definir as exclusões e a remontagem de forma explícita.

## Validação integral e perfis de referências

Atualização: Roslyn **5.8.0-1.26319.2** e runtime **.NET 10.0.10** foram encontrados no Visual Studio 18 Community já instalado. O runtime executou somente nosso driver ILSpy. A leitura independente usa Mono.Cecil 0.9.6.0; não carrega nem executa os assemblies do jogo no CLR. O compilador Framework 14.8.10001.0 compila o inspetor.

Assembly-CSharp usa o perfil Unity com mscorlib/System 2.0.5.0 (token `7cec85d7bea7798e`). Quatro sondas passaram com smcs/gmcs 2.6.5.0, otimização ligada ou desligada, debug desligado. `MONO_PATH` aponta para `Editor/Data/Mono/lib/mono/unity` e `MONO_CFG_DIR` para `Editor/Data/Mono/etc`. Sem esse ambiente, o Mono selecionava o perfil errado. A igualdade das sondas não identifica historicamente qual dos dois frontends foi usado.

KairoLibrary usa mscorlib 2.0.0.0 e System.Core 3.5.0.0 (token `b77a5c561934e089`), perfil clássico diferente do mscorlib do APK. Referências passadas, nesta ordem: `Editor/Data/Mono/lib/mono/2.0/mscorlib.dll`, `System.Core.dll`, **UnityEngine.dll do APK**, `Editor/Data/Mono/lib/mono/2.0/System.dll`. A identidade das referências é compatível; a igualdade dos arquivos históricos dessas referências continua UNKNOWN.

Opções explícitas dos dois builds completos Kairo: `/nologo /noconfig /nostdlib+ /target:library /platform:anycpu /optimize+ /debug:pdbonly /deterministic- /langversion:6 /unsafe+ /checked- /codepage:65001 /utf8output`. `/noconfig` fica fora do response file. Não são definidos símbolos extras. Ordem dos fontes: `config/compile-order/KairoLibrary.txt`, seguida de `AssemblyInfo.cs`. As opções são candidatas testadas; as opções históricas completas continuam UNKNOWN.

O pacote portátil 1.3.2, autorizado pelo usuário, fica somente em `analysis/`. SHA-256 do nupkg: `37aaedd86f96df0ccfef1ada743d6fefd36135b31b75f5305140b84da98918d7`; `csc.exe` versão interna 1.3.1.60621, SHA-256 `d14ba119edd71bdcb82b12659647427aa46d819caf6e72ac6d4ae86c81eb36c9`. Download explícito e verificado: `python tools/fetch_roslyn.py`.

Também foram conferidos contra o pacote fixado os hashes de csc.exe.config, Microsoft.CodeAnalysis, Microsoft.CodeAnalysis.CSharp, System.Collections.Immutable e System.Reflection.Metadata; não basta identificar apenas o executável que chama o gerador de código. [Componentes](../reports/KairoLibrary-compiler-components.json).

Ambos os builds completos compilaram sem erros. O pacote 1.3.2 corresponde em 2.787/3.211 corpos, e 5.8 em 2.736/3.211. Identificação histórica ainda UNKNOWN. Resultados, PE/debug, determinismo e limites: [linha de base Kairo](notes/kairo-baseline.md).

## Outras DLLs

Todas as nove DLLs gerenciadas foram auditadas pelo REA. **As sete externas agora coincidem por SHA-256 com arquivos oficiais**: Mono.Security, mscorlib, System, System.Core, UnityEngine.Networking e UnityEngine.UI na instalação inicial; UnityEngine.dll no módulo Android acrescentado. Versões, hashes e evidências em [config/dll-audit.json](../config/dll-audit.json). Nenhuma biblioteca externa foi decompilada.

Comparação inicial da UnityEngine.dll contra Editor/WebPlayer: 28/18 corpos diferiam, respectivamente. Essa comparação estava limitada às variantes disponíveis naquele momento. [Relatório estrutural histórico](../reports/UnityEngine-structural-comparison.json).

O módulo Android oficial 5.3.4f1 foi instalado com autorização explícita, instalador assinado como Valid por Unity Technologies SF, SHA-256 `17ce1e474487363ddcf2b18191410f1216afba3885eb88e928310029bc1523e3`. A DLL `AndroidPlayer/Variations/mono/Managed/UnityEngine.dll` é **idêntica em todos os 970.752 bytes ao APK**, SHA-256 `7cc901371cc77e103d00b5845e8285f85fbdf36bb47ccf3d824f94c4ead32421`. O layout compartilha essa DLL entre Release e Development; não há duas DLLs managed específicas por modo. As seis bibliotecas nativas do APK também correspondem; `libunity.so` em ARMv7 e x86 coincide exclusivamente com **Mono Release**. Os 140 arquivos existentes do editor/Mono/WebPlayer conferidos antes e depois ficaram inalterados. [Prova completa](../reports/UnityEngine-android-comparison.json), [nota](notes/unity-android.md).

O teste de duas compilações completas identificou os quatro campos registrados acima. As exclusões ainda não foram aplicadas à comparação de arquivos do original.

## Validação integral de Assembly-CSharp

Recuperados 96 tipos superiores mais AssemblyInfo.cs, 97 C#, com ILSpy/CSharp3 e resolver do perfil Unity primeiro. A matriz inicial com `-nostdlib` distinguia smcs 2.6.5.0 (1.444/1.952 corpos), mcs 3.4.0.0 (703) e gmcs 2.6.5.0 (oito erros de extensões). A investigação integral posterior comprovou que retirar `-nostdlib`, mantendo referências e perfil Unity, sobe para **1.530 sem perder nenhum corpo**. O smcs continua melhor candidato; otimização ligada/desligada produz os mesmos corpos e gmcs continua falhando. mcs 3.4 sem `-nostdlib` troca o corlib para 4.0 e não é comparação válida de versões. Identidade histórica exata permanece UNKNOWN.

Opções selecionadas: `-nologo -noconfig -target:library -platform:anycpu -optimize+ -debug- -langversion:Default -unsafe+ -checked- -codepage:65001`, com `--stdlib default`; `-noconfig` fora do response file. Referências do APK na ordem mscorlib, KairoLibrary, UnityEngine, System.Core, System. `MONO_PATH` e `MONO_CFG_DIR` definidos somente no subprocesso. O corlib implícito é o Unity 2.0.5.0, idêntico por hash ao APK; os AssemblyRefs resultantes coincidem também em ordem. A chamada original IndexOf(string,int) voltou ao fonte; o contorno foi retirado. Inicializadores default em dois campos recuperam dois `.cctor` apenas com `ret`, totalizando **1.532 corpos**, sem perder correspondências e mantendo BeforeFieldInit. [Nota e matrizes completas](notes/assembly-csharp-baseline.md).

## Variantes de recuperação

A numeração Roslyn depende da ordem expandida dos membros: as sondas próprias confirmam campo/método/tipo aninhado com peso um, propriedade manual get/set com peso três e automática get/set com peso quatro. A ordem recuperada combina FieldDef, MethodDef, Property e NestedType com os números dos nomes gerados. Foram restaurados dois construtores vazios omitidos pelo ILSpy. Candidatos isolados compilaram antes de qualquer adoção e preservaram todos os corpos que já correspondiam. [Sondas](../reports/KairoLibrary-member-order-probe.json), [aplicação](../reports/KairoLibrary-member-order-application.json).

Além de ILSpy 9.1.0.7988 com configurações distintas, foi preparado o decompilador independente Codemerx/JustDecompileEngine, commit `35f93d8fae9f6220bc8c976d1dbf376e57d6321d`, versão de assembly 2019.1.118.0. Compilado por MSBuild existente com referências Framework 4.7.2 portáteis, sem modificar fontes upstream ou instalar SDK global. Pacotes, engine e variantes ficam em analysis/. [Proveniência](../reports/Codemerx-decompiler.json). Variantes só podem entrar por fonte C# e comparação integral; nenhuma classe gerada é escrita manualmente.
