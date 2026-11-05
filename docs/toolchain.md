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
| Métodos de sonda validados | 0 | 0 |

As pistas vêm dos nomes originais de TypeDef, com a hierarquia NestedClass resolvida; a lista completa está em `config/compiler-hints.json`. Elas não autorizam selecionar um compilador sem testar. Instalar o editor não demonstra que KairoLibrary foi compilada por ele.

## Ferramentas disponíveis e pendentes

| Ferramenta | Evidência local | Uso |
|---|---|---|
| Python | 3.12.10 | Cadastros, hashes e pré-verificação |
| Node.js | 24.19.0 | REA e adaptador de metadados |
| Git | 2.55.0.windows.3 | Histórico local |
| Java | Microsoft OpenJDK 21.0.12.1 | Runtime disponível para ferramentas Java |
| Ghidra | 12.1.4, informado pelo REA | Não necessário para decompilar as DLLs gerenciadas |
| .NET | Runtime e Windows Desktop 6.0.36; SDK não localizado | Não comprova disponibilidade de compilador moderno |
| C# do Windows | `csc.exe` 14.8.10001.0, C# 5 | Não selecionado como compilador histórico |
| uv / ud | 0.12.23 / 0.1.0 | Instalados; não necessários aos cadastros atuais |
| ILSpy/dnSpy | Não localizados na inspeção inicial | Necessários à futura recuperação de C# |
| Mono.Cecil | **0.9.6.0**, incluído em `Editor/Data/Managed/Mono.Cecil.dll` | Disponível para a futura comparação independente e resolução de metadados |
| monodis/ildasm e dnlib/dnfile | Não localizados como comandos/bibliotecas utilizáveis na inspeção inicial | O arquivo `monodis` sem extensão incluído pelo editor ainda não foi validado como executável Windows |
| apktool/aapt2/zipalign | Não localizados no PATH | Remontagem futura; Python cobre a inspeção ZIP atual |

O inventário de caminhos e versões consultados está em `analysis/environment.json`. Ausência no PATH ou nos caminhos consultados não prova ausência em todo o computador. Ferramentas adicionais só serão instaladas com autorização.

## Exceções de comparação

**Nenhum campo autorizado.** MVID, timestamp PE e qualquer outro campo permanecem sob comparação até que duas compilações seguidas do mesmo fonte demonstrem sua variação e o campo seja identificado. Não há normalização genérica nem edição do resultado compilado.

A assinatura do APK é a exclusão final prevista pelo AGENTS.md. O inventário observou `META-INF/MANIFEST.MF`, `META-INF/CERT.SF` e `META-INF/CERT.RSA`; não observou a magic do APK Signing Block imediatamente antes do diretório central. A implementação da comparação final deverá definir as exclusões e a remontagem de forma explícita.
