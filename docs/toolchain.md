# Toolchain e evidências

## Estado atual — rodada iniciada em 66eea9d

Produção preservada: **Microsoft.Net.Compilers 1.1.0 / csc 1.1.0.51109** para KairoLibrary; **Unity 5.3.4f1 / smcs 2.6.5.0** para Assembly-CSharp. Compiladores, defines, flags, perfis, referências e sua ordem não mudaram. Nenhuma nova exceção não determinística.

| Assembly | Matched anterior → atual / total | Bytes IL matched anterior → atual / total | Corpos iguais | Tipos estruturais iguais |
| --- | --- | --- | --- | --- |
| KairoLibrary | 3187 → 3193 / 3287 | 204103 → 208123 / 269894 | 3119 | 337/337 |
| Assembly-CSharp | 1688 → 1703 / 1958 | 203891 → 225924 / 601603 | 1698 | 129/129 |

**4896/5245 métodos matched; 434047/871497 bytes IL matched.** Zero regressões adotadas de corpos, estados e tipos. Os 466 tipos e 82 declarações sem corpo passaram novamente. 3 corpos iguais permanecem retidos pelo vínculo de auxiliares. Igualdade estrutural não implica igualdade bruta dos heaps, tabelas e arquivos.

Provas: [adoção oficial](../reports/round6-adoption.json), [medição inicial](../reports/round6-baseline-measurement.json), [medição final](../reports/round6-final-measurement.json), [relatório completo](notes/round-06-report.md). Estados são escritos exclusivamente pelo verificador.

**Modelo smcs calculado a partir do fonte, sem emitir candidatos.** A regra recuperada usa inserção de variáveis, temporários registrados, blocos filhos/implícitos e temporários de emissão com reutilização por tipo. Validação: 1575/1682 assinaturas exatas em todos os corpos matched anteriores; 1539/1539 nas previsões declaradas completas. 100 corpos gerados ou não mapeados não contam como acerto por uma lista vazia. O gate exige previsão completa e assinatura prevista igual à original antes de compilar; discrepâncias observadas na assinatura compilada são rejeitadas. A cobertura incompleta está classificada em [round6-slot-model-validation.json](../reports/round6-slot-model-validation.json); não se declara modelo universal.

Nas tentativas compiladas, 144 métodos distintos tiveram candidato com assinatura igual à original. No fonte efetivamente adotado, 15 dos pendentes iniciais passaram a ter assinatura igual e 9 desses fecharam o corpo. Tentativa confirmada não promove matched.

**Busca por localização alinhada.** Destinos de desvio são reconciliados por rótulos e a primeira inserção, remoção ou substituição orienta o comando de fonte. A busca parcial exige avanço bruto por padrão. Na última retomada dos três gradientes, o prefixo de instruções alinhadas pôde avançar estritamente enquanto todos os bytes do prefixo bruto já igual permaneceram preservados. Cabeçalho, locais, EH, larguras e operandos continuam no comparador exato; nenhum estado matched usa o critério parcial. O motor percorreu os 269 pendentes smcs iniciais e 18 controles Roslyn. Os dois assemblies foram integralmente compilados e verificados. Limites e paradas por método constam nos relatórios dos motores. Destino recompilado em trecho inserido pode permanecer classificado como branch_retarget; alinhamentos repetidos podem ser ambíguos.

**Roslyn 1.1.** Nos quatro controles pedidos da linha de base 66eea9d, não havia local extra nem ausente: eram permutações de slots. EmitBlock define locais antes dos comandos; locais nomeados não são liberados, temporários reutilizáveis usam pilhas por assinatura, e o otimizador pode manter valores na pilha. Fonte oficial fixada e casos: [nota Roslyn](notes/round6-roslyn-source-evidence.md). Não foi construído modelo Roslyn completo. Nomes originais de locais sem símbolos e revisão histórica exata de fabricação continuam UNKNOWN; aliases/nomes reservados introduzidos são escolhas legíveis da reconstrução.

**Limites mantidos.** Externos permanecem verificados contra Unity oficial. As seis exceções não determinísticas conservam a prova anterior de dois builds idênticos, sem acréscimos ou normalização de DLL/IL. DEX/Java e correções baseadas em bytes brutos ficaram fora do escopo. Os fontes exatos do índice Git são recompilados na prova adicional registrada em reports/round6-git-source-proof.json; reports/round6-git-content-provenance.json distingue hashes medidos dos publicados quando há conversão textual CRLF/LF.

As seções abaixo preservam o histórico; números e diferenças brutas nelas descritos pertencem aos respectivos snapshots.

## Histórico — rodada iniciada em 7d9fbc8

Produção preservada: **Microsoft.Net.Compilers 1.1.0 / csc 1.1.0.51109** para KairoLibrary; **Unity 5.3.4f1 / smcs 2.6.5.0** para Assembly-CSharp. Compiladores, defines, flags, perfis, referências e sua ordem não mudaram. Não foram acrescentadas exceções não determinísticas.

| Assembly | Matched anterior → atual / total | Bytes IL matched anterior → atual / total | Corpos iguais | Tipos estruturais iguais |
| --- | --- | --- | --- | --- |
| KairoLibrary | 3187 → 3187 / 3287 | 204103 → 204103 / 269894 | 3113 | 337/337 |
| Assembly-CSharp | 1676 → 1688 / 1958 | 196453 → 203891 / 601603 | 1683 | 129/129 |

**4875/5245 métodos matched; 407994/871497 bytes IL matched.** Zero regressões adotadas. Os 466 tipos e as 82 declarações sem corpo têm prova estrutural atual. 3 corpos iguais permanecem retidos pelo vínculo de auxiliares. Igualdade estrutural não significa igualdade de todos os corpos, heaps, tabelas ou do arquivo inteiro.

Provas atuais: [adoção e verificação oficial integral](../reports/round5-adoption.json), [medição inicial](../reports/round5-baseline-measurement.json), [medição final](../reports/round5-final-measurement.json), [relatório autossuficiente](notes/round-05-report.md). Os estados foram escritos exclusivamente pelo verificador.

**Hipótese de nomes REFUTADA nos caminhos testados.** Oito compilações da fixture pelo smcs instalado mostraram que trocar apenas nomes conserva corpos e locais; reordenar declarações altera os slots. A fonte oficial Unity usa ListDictionary em blocos ordinários; a Hashtable encontrada é usada em clonagem de sondagem. O parser cria blocos implícitos após comandos, além das chaves lexicais. A árvore interna de blocos, a posição das declarações e os temporários registrados ou alocados na emissão explicam a regra observada. Busca por nomes não foi adotada. [Fixture](../reports/round5-slot-fixture.json); [fonte e binário inspecionados](../reports/round5-smcs-source.json); [nota com referências fixadas e limites](notes/round5-smcs-source-evidence.md).

A fonte consultada pertence ao repositório oficial Unity-Technologies/mono, snapshot `b6587926c0f146c8b12985744f33a13b39a22d57` de 2016-03-11. A revisão exata de fabricação do executável instalado continua UNKNOWN; as operações relevantes foram confirmadas na inspeção do próprio smcs instalado. Os nomes originais de locais sem símbolos também continuam UNKNOWN. Os aliases e locais reservados introduzidos usam nomes legíveis escolhidos para a reconstrução; não são nomes originais recuperados.

**Motor desta rodada: somente Assembly-CSharp.** Foram selecionados 119 métodos / 201923 bytes IL, por avanço parcial anterior, limite anterior ou padrões pedidos, em ordem decrescente de tamanho. Foi feita uma passagem inicial pelos selecionados e mais 4 retomadas direcionadas de 4 métodos. 162 corpos pendentes iniciais não tiveram nova busca nesta fase; todos os métodos continuaram na verificação integral e na proteção de regressões. KairoLibrary foi medido e recompilado sem correções de fonte; o relatório fornece seu histograma Roslyn exclusivo. O motor smcs conserva todo o prefixo correto, exige avanço estrito ou igualdade completa e rejeita regressões de corpos/metadados. Mapeamento MDB só é usado após comparação debug+/debug- do corpo atual. Os builds finais usam as flags de produção (-debug-), sem fontes provisórias.

| Iterações máximas por método | Candidatos por comando/iteração | Limites de iteração atingidos | Corpos ganhos | Bytes IL ganhos |
| --- | --- | --- | --- | --- |
| 16 | 96 | 0 | 12 | 7438 |

[Motor App e paradas por método](../reports/round5-Assembly-CSharp-engine.json). [Diagnósticos de slots e controles alias/DrawSeb](../reports/round5-local-cases.json). Prefixo parcial não promove matched. As produções integrais dos dois assemblies foram comparadas aos corpos esperados antes da adoção dos C#; nova verificação oficial completa confirmou o resultado depois da cópia.

**Componentes e limites preservados.** Externos permanecem verificados contra o Unity oficial. As seis exceções não determinísticas já demonstradas por pares de builds idênticos permanecem as mesmas; nenhuma normalização de DLL/IL foi aplicada. A versão histórica exata dos compiladores, o comando completo e as referências históricas continuam UNKNOWN. DEX/Java permaneceu fora do escopo. Não foram feitas correções baseadas em bytes brutos.

As seções abaixo preservam o histórico. Contagens e valores brutos nelas descritos pertencem aos respectivos snapshots, não ao build final desta rodada.

## Histórico — rodada iniciada em 1047b1f

Seleção de produção preservada: **Microsoft.Net.Compilers 1.1.0 / csc 1.1.0.51109** para KairoLibrary e **smcs 2.6.5.0 / Unity 5.3.4f1** para Assembly-CSharp. Flags, defines, referências e sua ordem permanecem os da rodada anterior. Nenhum compilador alternativo, referência Microsoft adicional ou exceção não determinística foi adotado.

Os auxiliares de análise sintática usam Roslyn **5.8.0-1.26319.2**, commit `375c93f06396eff88a355bc473d1586bf71befb5`, e runtime **.NET 10.0.10**. Eles geram hipóteses de edição do C#; os assemblies do jogo são emitidos pelos compiladores de produção registrados acima.

| Assembly | Métodos matched anterior → atual | Corpos integralmente iguais | Bytes IL matched anterior → atual / total | Tipos estruturais iguais |
| --- | --- | --- | --- | --- |
| KairoLibrary | 3144 → 3187 / 3287 | 3113 | 192911 → 204103 / 269894 | 337/337 |
| Assembly-CSharp | 1611 → 1676 / 1958 | 1671 | 170582 → 196453 / 601603 | 129/129 |

**4863/5245 métodos matched; 400556/871497 bytes IL matched; 466/466 tipos com identidade estrutural comprovada.** As 82 declarações sem corpo têm prova de metadados; 3 auxiliares com corpo igual continuam retidos pela regra de vínculo. Zero regressões adotadas. Igualdade estrutural não significa igualdade de todos os corpos, heaps, tabelas ou do arquivo inteiro.

Os números vieram da [adoção após verificação oficial](../reports/round4-adoption.json), [medição final](../reports/round4-final-measurement.json) e recibos oficiais atuais. O [relatório autossuficiente](notes/round-04-report.md) inclui o histograma completo antes/depois, exemplos e negativos. [Medição inicial](../reports/round4-baseline-measurement.json).

**Mapeamento e motor.** Roslyn usa pontos de sequência do PDB nativo emitido com `/debug:pdbonly`. No smcs, `-debug+` preservou os 1952 corpos do controle congelado, inclusive bytes literais, e seis declarações sem corpo; o MDB fornece os mesmos offsets. Cada método atual do motor repete a prova debug+/debug- antes de usar seu MDB. Cobertura inicial: 140/141 Kairo e 343/345 App com primeira diferença de IL. [Prova](../reports/round4-symbol-mapping.json).

O motor aplica transformações ao comando mapeado, conserva todos os bytes do prefixo já correto, exige avanço estrito ou corpo integralmente igual e rejeita regressões de corpos/metadados. Limites medidos por assembly:

| Assembly | Iterações por método | Candidatos por iteração | Limites de iteração atingidos | Corpos novos iguais | Bytes IL novos iguais |
| --- | --- | --- | --- | --- | --- |
| KairoLibrary | 8 | 24 | 0 | 43 | 11192 |
| Assembly-CSharp | 8 | 24 | 0 | 65 | 25871 |

[Motor Kairo](../reports/round4-KairoLibrary-engine.json); [motor App](../reports/round4-Assembly-CSharp-engine.json). Ganho parcial de prefixo continua fora de matched até o verificador conferir corpo, locais e exceções completos.

**Referências Kairo.** System 2.0 e System.Core 3.5 da Microsoft foram testados separadamente e juntos contra os equivalentes Unity, nos mesmos fontes congelados do commit `1047b1f`: zero corpos alterados, zero ganhos/perdas e 337/337 tipos em todas as combinações. A configuração selecionada continua Microsoft mscorlib 2.0, Unity System.Core 3.5, UnityEngine do APK, Unity System 2.0, nessa ordem. [Matriz com hashes](../reports/round4-reference-probes.json).

As três diferenças brutas AssemblyRef dos controles congelados estão em `PublicKeyOrToken`, linhas 1/mscorlib, 2/System.Core e 4/System: índice #Blob **11271 → 11351**, bytes `07 2c → 57 2c`. Os três apontam para o mesmo token `b77a5c561934e089`; os valores numéricos são do baseline, não uma afirmação sobre os offsets/índices finais. Nenhuma referência foi corrigida por esses três bytes.

**Opções de ensaio.** Roslyn `/shared` foi usado somente nos ensaios do motor e em sondas de transporte; na CLI preservou os 3287 métodos e metadados estruturais. No RSP foi rejeitado com CS2007, registrado como resultado negativo. `-debug+` smcs foi usado somente para o MDB. Os candidatos foram recompilados com as flags anteriores de produção e comparados integralmente antes da adoção. [Sondas com metadados](../reports/round4-shared-compiler-probe.json); [sonda independente](../reports/round4-compiler-server-probe.json).

**Triagem opcional por fonte.** A triagem usa corpos C# provisórios nos outros arquivos apenas para acelerar tentativas descartáveis. Um controle deve provar a igualdade completa do corpo alvo contra o build real antes de habilitá-la por método. Todo candidato que pareça avançar passa novamente por build real completo, preservação do prefixo e prova de zero regressões antes de qualquer adoção. A DLL de produção não contém os corpos provisórios. Contraexemplos registrados impedem tratar essa otimização como uma equivalência universal. [Controles](../reports/round4-screen-validation.json).

| Assembly | Controles do alvo | Habilitados | Recusados | Positivos conferidos no build real | Avanços falsos rejeitados |
| --- | --- | --- | --- | --- | --- |
| Assembly-CSharp | 392 | 380 | 12 | 151 | 0 |

**Conteúdo Git.** Os C# extraídos do índice Git e normalizados conforme `.gitattributes` foram recompilados com os comandos de produção: zero corpos alterados e 466/466 tipos iguais. [Prova](../reports/round4-git-source-proof.json). Os hashes de evidência identificam os bytes medidos no workspace; o [manifesto de conteúdo](../reports/round4-git-content-provenance.json) também registra os hashes dos bytes commitados quando há conversão CRLF→LF. Essa conversão não altera DLLs/IL.

**Bytes brutos: somente medição.** As [contagens finais de linhas](../reports/round4-final-raw-row-counts.json) dizem quais tabelas diferem em bytes e se têm a mesma quantidade de linhas. [Kairo](../reports/round4-final-KairoLibrary-raw-measurement.json); [App](../reports/round4-final-Assembly-CSharp-raw-measurement.json). Essas diferenças não foram usadas para editar bytes, heaps, tabelas ou estados.

Tabelas com quantidades de linhas diferentes: KairoLibrary: StandAloneSig 534/536; Assembly-CSharp: StandAloneSig 514/512.

**Limites históricos.** O executável histórico exato, o comando e as referências históricas completas e o determinismo dos originais continuam **UNKNOWN**. A lista de seis campos não determinísticos comprovados em duas compilações permanece inalterada em `config/toolchain.json`; nenhuma normalização foi aplicada. DEX/Java continua fora do escopo.

As seções abaixo preservam o histórico de outras rodadas. Seus números, referências ou rótulos de configuração valem para os snapshots nelas descritos; o estado atual é o desta seção.

## Histórico — rodada iniciada em 8851963

Seleção mantida: **Microsoft.Net.Compilers 1.1.0 / csc 1.1.0.51109** para Kairo e **smcs 2.6.5.0** para App. As opções continuam /unsafe- (Kairo) e -unsafe- (App), sem defines adicionais. [Definição selecionada](../tools/kairo_candidate.py).

A primeira referência Kairo agora é **Microsoft .NET Framework 2.0**, `C:/Windows/Microsoft.NET/Framework/v2.0.50727/mscorlib.dll`, SHA-256 `c4729c6fc2279933fa9638f60775ab2f7eb731d5b04f9e04c15187afbdf94558`. System.Core 3.5 e System 2.0 continuam do perfil clássico Unity, seguidos conforme a ordem já documentada; UnityEngine vem do APK. Ordem exata: mscorlib, System.Core, UnityEngine, System. A identidade assembly é a mesma da referência Mono anterior, mas a ordem das interfaces herdadas de IEnumerator<T> muda. Esse ajuste preservou todas as saídas de métodos e aprovou os dois iteradores. Hash histórico da referência original continua UNKNOWN. [Prova](../reports/round3-mscorlib-probe.json).

**Todos os 466 tipos e metadados de definição têm identidade estrutural comprovada**: 337/337 Kairo e 129/129 App; os metadados globais comparados também coincidem. Métodos matched: **3.144/3.287 Kairo**, **1.611/1.958 App**, **4.755/5.245 no total**. Corpos idênticos: 3.070 e 1.606; 82 declarações sem CIL aprovadas; três auxiliares iguais retidos. A igualdade estrutural não implica igualdade bruta dos índices/heaps/PE nem identidade de todos os corpos.

Revisões intermediárias: pacote1.1.1 / csc1.1.0.51202 é indistinguível do 1.1.0 em todos os métodos e metadados inspecionados; 1.2.1 / csc1.2.0.60325 e 1.2.2 / csc1.2.0.60425 perdem 18 correspondências cada, sem ganhos. Os dois da série1.2 também são indistinguíveis entre si. [Matriz completa](../reports/KairoLibrary-roslyn-revisions-round3.json). Os pacotes portáteis e seis componentes de cada versão têm hashes fixados no [registro de aquisição](../reports/round3-Roslyn-revisions-acquisition.json).

Provas de campos variáveis na configuração documentada:
- Kairo: dois builds finais dos 163 fontes com a referência Microsoft; 34 bytes diferentes, restritos a MVID(16), COFF timestamp(um byte observado em campo4), debug timestamp(um em campo4), GUID do PDB(16). [Prova](../reports/round3-Kairo-repeatability.json).
- App: dois builds dos mesmos 97 fontes congelados de início da rodada; 17 bytes diferentes, restritos a MVID(16) e COFF timestamp(um observado em campo4). debug- não emitiu diretório de depuração/CodeView. [Prova](../reports/round3-Assembly-CSharp-repeatability.json).

Esses seis campos estão registrados em config/toolchain.json. **Nenhuma normalização foi aplicada.** Determinismo das compilações originais permanece UNKNOWN.

Heaps e tabelas brutas passaram de NOT_EVALUATED para medição por bytes: [Kairo](../reports/round3-final-KairoLibrary-raw-measurement.json), [App](../reports/round3-final-Assembly-CSharp-raw-measurement.json). Diferenças são posicionais dentro de regiões de mesmo nome, com caudas sem par separadas; não são contagens de erros semânticos. Nenhuma correção motivada por essa medição foi tentada. Ambos os heaps #Strings são byte a byte idênticos.

As seções abaixo preservam configurações e medições históricas das rodadas anteriores; o estado atual é o desta seção.

## Configuração selecionada — rodada iniciada em 2ab24ee

KairoLibrary: pacote **Microsoft.Net.Compilers 1.1.0**, executável **1.1.0.51109**. SHA-256 do pacote: `cd84fb2c71cafe03cdae0add609fa12c75f61c89ea87b1422fd108ac7e570745`; do csc.exe: `53b693536e8a86f35d05d2165ff9b3e0aeaa499ecfd8135a9f99ab4404ca86c3`. Os seis componentes foram conferidos byte a byte contra o pacote fixado e têm hashes em [componentes](../reports/KairoLibrary-compiler-components-1.1.0.json) e `tools/kairo_candidate.py`. O verificador confere os seis antes de compilar. Aquisição portátil: `python tools/fetch_roslyn_neighbors.py`; build selecionado: `python tools/build --assembly KairoLibrary`.

Opções Kairo: `/nologo /noconfig /nostdlib+ /target:library /platform:anycpu /optimize+ /debug:pdbonly /deterministic- /langversion:6 /unsafe- /checked- /codepage:65001 /utf8output`. Referências na ordem: mscorlib 2.0.0.0, System.Core 3.5.0.0, UnityEngine do APK, System 2.0.0.0. Perfil clássico Unity; nenhuma definição extra. A ordem dos 162 arquivos superiores foi ajustada para reproduzir os 337 TypeDefs; AssemblyInfo vem ao final.

Assembly-CSharp: smcs **2.6.5.0** do Unity **5.3.4f1**. SHA-256: `26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0`. Opções: `-nologo -noconfig -target:library -platform:anycpu -optimize+ -debug- -langversion:Default -unsafe- -checked- -codepage:65001`. Biblioteca padrão habilitada, sem `-nostdlib`; referências do APK na ordem mscorlib, KairoLibrary, UnityEngine, System.Core, System. `MONO_PATH` e `MONO_CFG_DIR` são locais ao subprocesso. São 96 tipos superiores mais AssemblyInfo.

Os dois noconfig são passados fora do arquivo de resposta. Identidades históricas exatas, comando original completo e símbolos históricos continuam **UNKNOWN**.

Matriz congelada antes da última correção estrutural: Roslyn 1.0.0/1.0.0.50618 iguala 3.013 corpos (+14/-35 contra 1.3.2); 1.1.0/1.1.0.51109 iguala 3.051 (+17/-0); 1.2.0/1.2.0.60317 e 1.3.2/1.3.1.60621 igualam 3.034 e são indistinguíveis em todos os métodos inspecionados. Três métodos são exclusivos do 1.1 entre os quatro; os outros 14 ganhos também aparecem no 1.0. [Matriz](../reports/KairoLibrary-roslyn-neighbors-round2.json). A única opção omitida foi `/deterministic-` no 1.0, que a rejeita. A adoção de 1.1 foi verificada no build principal, sem perder corpos, tipos ou declarações.

unsafe+ era a causa comprovada dos atributos extras: Roslyn gerava UnverifiableCodeAttribute e DeclSecurity/SkipVerification; smcs gerava UnverifiableCodeAttribute. As fixtures e os builds completos com unsafe- removeram esses registros, sem mudar correspondências de corpos ou tipos. Não houve normalização. [Prova](../reports/unsafe-metadata-fixtures.json).

Estado final: Kairo 3.123 matched/3.287 métodos, incluindo 76 declarações sem CIL; 3.051 corpos idênticos; 332/337 tipos. Assembly-CSharp 1.572 matched/1.958, incluindo seis declarações sem CIL; 1.567 corpos idênticos; 128/129 tipos. Comparação estrutural completa por tipo implementada; comparação bruta das tabelas, heaps e arquivo integral ainda pendente. [Kairo](../reports/KairoLibrary-metadata.json), [App](../reports/Assembly-CSharp-metadata.json).

## Exceções comprovadas para o candidato atual

Dois builds completos idênticos do **1.1.0, unsafe-, debug:pdbonly, deterministic-**, com mesmos fontes/referências/ordem/opções/caminho de saída, produziram 33 bytes diferentes e zero fora destes quatro campos:

| Campo | Tamanho | Bytes observados diferentes |
|---|---:|---:|
| Module.Mvid | 16 | 15 |
| COFF.TimeDateStamp | 4 | 1 |
| IMAGE_DEBUG_DIRECTORY.TimeDateStamp | 4 | 1 |
| CodeView.RSDS.PdbGuid | 16 | 16 |

[Prova atual](../reports/KairoLibrary-repeatability-1.1.0.json). MVID localizado pela linha Module e heap #GUID do parser REA. Offsets da prova não são constantes para builds futuros. Nenhuma normalização ou edição de DLL foi aplicada. Não estender automaticamente estas exceções a Assembly-CSharp. O determinismo do original continua UNKNOWN.

## Histórico de preparação

As seções seguintes preservam medições e configurações históricas. Elas não substituem a seleção e os resultados atuais acima.


### Referência e motor

APK: `original/app.apk`, SHA-256 `d557af6b4bc7748605a39ed3548821f6763c5773a12432a3fed6e8c413b211e8`, 28.820.227 bytes. Os hashes das 1.218 entradas estão em `config/inventory.csv`, e a ordem, compressão e alinhamento em `original/zip-layout.json`.

Unity **5.3.4f1** aparece como string terminada em zero no offset `0x14` de:

| Arquivo | SHA-256 |
|---|---|
| `assets/bin/Data/globalgamemanagers` | `0ff7354b578f4a2c03e92f0cd22e20e5bf38df0513e58865bc425788bc7824df` |
| `assets/bin/Data/Resources/unity_builtin_extra` | `5bfaebf3d445f3fa935c285906b532529a5ea1441a073bd2ba5bc1a268a9f577` |

O instalador foi obtido pelo link Windows da [página oficial do Unity 5.3.4f1](https://unity.com/releases/editor/whats-new/5.3.4f1). SHA-256 do instalador: `d56600a91172f5d0d02bdf46d6d904b043e79b50c6da2488212ef4f417b57bcc`, 264.190.104 bytes. O Windows verificou a assinatura como `Valid`, emitida para **Unity Technologies SF**. Registro: `analysis/downloads/unity-5.3.4f1-download.json`.

Instalado em `C:\Users\l_abn\AppData\Local\Programs\Unity\5.3.4f1`, usando `/S /D=...`, conforme o [manual oficial da versão 5.3](https://docs.unity3d.com/530/Documentation/Manual/InstallingUnity.html). O executável do editor informa a versão de arquivo `5.3.4.16628561`. O estado e os hashes dos componentes usados estão em `config/toolchain.json` e `analysis/unity-installation.json`.

O compilador padrão incluído respondeu como **Mono C# compiler 2.6.5.0**, tanto em `gmcs.exe` quanto em `smcs.exe`. O MonoBleedingEdge incluído fornece **mcs 3.4.0.0**. Uma DLL com fonte sintético de dois métodos foi compilada com sucesso pelo gmcs; esse teste demonstra que o compilador funciona, sem validar correspondência com métodos do jogo. O editor não foi ativado nem aberto interativamente.

### REA e leitura de assemblies

O REA **6.5.0** desta instalação analisa PE/CLI, metadados e CIL. Ele é a fonte principal das duas DLLs alvo. As evidências completas foram preservadas em `analysis/rea-evidence-2026-11-05.json`, com hashes e MVIDs. Essa análise não executou o jogo.

`tools/rea_metadata.mjs` complementa o inventário usando os leitores já instalados do REA 6.5.0. O adaptador verifica a versão e registra os hashes de seus módulos em `analysis/metadata/`. Os scripts Python usam somente a biblioteca padrão para catalogar, organizar e conferir hashes.

`rea doctor` respondeu com saída 1: Ghidra está disponível, mas há avisos sobre outros providers, skill local desatualizada e registros de clientes. O MCP ativo respondeu como REA 6.5.0. Os avisos não foram tratados como falha do parser .NET, que já produziu as evidências. Nenhuma configuração global foi reparada nesta preparação.

### Compiladores por assembly

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

### Ferramentas disponíveis e pendentes

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

### Exceções de comparação

Foram registradas quatro exceções comprovadas para o candidato KairoLibrary, pacote Roslyn 1.3.2, com `/debug:pdbonly /deterministic-`:

| Campo | Localização semântica | Prova |
|---|---|---|
| MVID | GUID referenciado pela linha Module | 16 bytes mudaram entre builds idênticos |
| Timestamp COFF | Cabeçalho PE/COFF TimeDateStamp | Um byte mudou nesta repetição; campo tem quatro bytes |
| Timestamp de depuração | TimeDateStamp da entrada IMAGE_DEBUG_DIRECTORY | Um byte mudou; campo tem quatro bytes |
| GUID do PDB | GUID do registro CodeView RSDS | 16 bytes mudaram |

Evidência: [reports/KairoLibrary-repeatability.json](../reports/KairoLibrary-repeatability.json), dois builds completos dos mesmos fontes, referências, opções e caminho de saída; 34 bytes diferentes, nenhum fora desses campos. A autorização para registrá-los veio da solicitação do usuário após examinar essa prova. Exceções declaradas em `config/toolchain.json`; **nenhum normalizador de assembly foi aplicado**, nenhum campo adicional está excluído e nenhum binário foi editado. Offsets não são fixados para futuros builds: os campos terão de ser encontrados pela estrutura PE/CLI. A prova vale para esse candidato/opções; o determinismo histórico do original continua UNKNOWN.

A assinatura do APK é a exclusão final prevista pelo AGENTS.md. O inventário observou `META-INF/MANIFEST.MF`, `META-INF/CERT.SF` e `META-INF/CERT.RSA`; não observou a magic do APK Signing Block imediatamente antes do diretório central. A implementação da comparação final deverá definir as exclusões e a remontagem de forma explícita.

### Validação integral e perfis de referências

Atualização: Roslyn **5.8.0-1.26319.2** e runtime **.NET 10.0.10** foram encontrados no Visual Studio 18 Community já instalado. O runtime executou somente nosso driver ILSpy. A leitura independente usa Mono.Cecil 0.9.6.0; não carrega nem executa os assemblies do jogo no CLR. O compilador Framework 14.8.10001.0 compila o inspetor.

Assembly-CSharp usa o perfil Unity com mscorlib/System 2.0.5.0 (token `7cec85d7bea7798e`). Quatro sondas passaram com smcs/gmcs 2.6.5.0, otimização ligada ou desligada, debug desligado. `MONO_PATH` aponta para `Editor/Data/Mono/lib/mono/unity` e `MONO_CFG_DIR` para `Editor/Data/Mono/etc`. Sem esse ambiente, o Mono selecionava o perfil errado. A igualdade das sondas não identifica historicamente qual dos dois frontends foi usado.

KairoLibrary usa mscorlib 2.0.0.0 e System.Core 3.5.0.0 (token `b77a5c561934e089`), perfil clássico diferente do mscorlib do APK. Referências passadas, nesta ordem: `Editor/Data/Mono/lib/mono/2.0/mscorlib.dll`, `System.Core.dll`, **UnityEngine.dll do APK**, `Editor/Data/Mono/lib/mono/2.0/System.dll`. A identidade das referências é compatível; a igualdade dos arquivos históricos dessas referências continua UNKNOWN.

Opções explícitas dos dois builds completos Kairo: `/nologo /noconfig /nostdlib+ /target:library /platform:anycpu /optimize+ /debug:pdbonly /deterministic- /langversion:6 /unsafe+ /checked- /codepage:65001 /utf8output`. `/noconfig` fica fora do response file. Não são definidos símbolos extras. Ordem dos fontes: `config/compile-order/KairoLibrary.txt`, seguida de `AssemblyInfo.cs`. As opções são candidatas testadas; as opções históricas completas continuam UNKNOWN.

O pacote portátil 1.3.2, autorizado pelo usuário, fica somente em `analysis/`. SHA-256 do nupkg: `37aaedd86f96df0ccfef1ada743d6fefd36135b31b75f5305140b84da98918d7`; `csc.exe` versão interna 1.3.1.60621, SHA-256 `d14ba119edd71bdcb82b12659647427aa46d819caf6e72ac6d4ae86c81eb36c9`. Download explícito e verificado: `python tools/fetch_roslyn.py`.

Também foram conferidos contra o pacote fixado os hashes de csc.exe.config, Microsoft.CodeAnalysis, Microsoft.CodeAnalysis.CSharp, System.Collections.Immutable e System.Reflection.Metadata; não basta identificar apenas o executável que chama o gerador de código. [Componentes](../reports/KairoLibrary-compiler-components.json).

Ambos os builds completos compilaram sem erros. O pacote 1.3.2 corresponde em 2.787/3.211 corpos, e 5.8 em 2.736/3.211. Identificação histórica ainda UNKNOWN. Resultados, PE/debug, determinismo e limites: [linha de base Kairo](notes/kairo-baseline.md).

### Outras DLLs

Todas as nove DLLs gerenciadas foram auditadas pelo REA. **As sete externas agora coincidem por SHA-256 com arquivos oficiais**: Mono.Security, mscorlib, System, System.Core, UnityEngine.Networking e UnityEngine.UI na instalação inicial; UnityEngine.dll no módulo Android acrescentado. Versões, hashes e evidências em [config/dll-audit.json](../config/dll-audit.json). Nenhuma biblioteca externa foi decompilada.

Comparação inicial da UnityEngine.dll contra Editor/WebPlayer: 28/18 corpos diferiam, respectivamente. Essa comparação estava limitada às variantes disponíveis naquele momento. [Relatório estrutural histórico](../reports/UnityEngine-structural-comparison.json).

O módulo Android oficial 5.3.4f1 foi instalado com autorização explícita, instalador assinado como Valid por Unity Technologies SF, SHA-256 `17ce1e474487363ddcf2b18191410f1216afba3885eb88e928310029bc1523e3`. A DLL `AndroidPlayer/Variations/mono/Managed/UnityEngine.dll` é **idêntica em todos os 970.752 bytes ao APK**, SHA-256 `7cc901371cc77e103d00b5845e8285f85fbdf36bb47ccf3d824f94c4ead32421`. O layout compartilha essa DLL entre Release e Development; não há duas DLLs managed específicas por modo. As seis bibliotecas nativas do APK também correspondem; `libunity.so` em ARMv7 e x86 coincide exclusivamente com **Mono Release**. Os 140 arquivos existentes do editor/Mono/WebPlayer conferidos antes e depois ficaram inalterados. [Prova completa](../reports/UnityEngine-android-comparison.json), [nota](notes/unity-android.md).

O teste de duas compilações completas identificou os quatro campos registrados acima. As exclusões ainda não foram aplicadas à comparação de arquivos do original.

### Validação integral de Assembly-CSharp

Recuperados 96 tipos superiores mais AssemblyInfo.cs, 97 C#, com ILSpy/CSharp3 e resolver do perfil Unity primeiro. A matriz inicial com `-nostdlib` distinguia smcs 2.6.5.0 (1.444/1.952 corpos), mcs 3.4.0.0 (703) e gmcs 2.6.5.0 (oito erros de extensões). A investigação integral posterior comprovou que retirar `-nostdlib`, mantendo referências e perfil Unity, sobe para **1.530 sem perder nenhum corpo**. O smcs continua melhor candidato; otimização ligada/desligada produz os mesmos corpos e gmcs continua falhando. mcs 3.4 sem `-nostdlib` troca o corlib para 4.0 e não é comparação válida de versões. Identidade histórica exata permanece UNKNOWN.

Opções selecionadas: `-nologo -noconfig -target:library -platform:anycpu -optimize+ -debug- -langversion:Default -unsafe+ -checked- -codepage:65001`, com `--stdlib default`; `-noconfig` fora do response file. Referências do APK na ordem mscorlib, KairoLibrary, UnityEngine, System.Core, System. `MONO_PATH` e `MONO_CFG_DIR` definidos somente no subprocesso. O corlib implícito é o Unity 2.0.5.0, idêntico por hash ao APK; os AssemblyRefs resultantes coincidem também em ordem. A chamada original IndexOf(string,int) voltou ao fonte; o contorno foi retirado. Inicializadores default em dois campos recuperam dois `.cctor` apenas com `ret`, totalizando **1.532 corpos**, sem perder correspondências e mantendo BeforeFieldInit. [Nota e matrizes completas](notes/assembly-csharp-baseline.md).

### Variantes de recuperação

A numeração Roslyn depende da ordem expandida dos membros: as sondas próprias confirmam campo/método/tipo aninhado com peso um, propriedade manual get/set com peso três e automática get/set com peso quatro. A ordem recuperada combina FieldDef, MethodDef, Property e NestedType com os números dos nomes gerados. Foram restaurados dois construtores vazios omitidos pelo ILSpy. Candidatos isolados compilaram antes de qualquer adoção e preservaram todos os corpos que já correspondiam. [Sondas](../reports/KairoLibrary-member-order-probe.json), [aplicação](../reports/KairoLibrary-member-order-application.json).

Além de ILSpy 9.1.0.7988 com configurações distintas, foi preparado o decompilador independente Codemerx/JustDecompileEngine, commit `35f93d8fae9f6220bc8c976d1dbf376e57d6321d`, versão de assembly 2019.1.118.0. Compilado por MSBuild existente com referências Framework 4.7.2 portáteis, sem modificar fontes upstream ou instalar SDK global. Pacotes, engine e variantes ficam em analysis/. [Proveniência](../reports/Codemerx-decompiler.json). Variantes só podem entrar por fonte C# e comparação integral; nenhuma classe gerada é escrita manualmente.


### Rodada 7 - modelos independentes e ciclo autonomo

Os emissores de producao permanecem Roslyn NuGet 1.1.0 (csc.exe
1.1.0.51109) para KairoLibrary e smcs 2.6.5.0 do Unity 5.3.4f1 para
Assembly-CSharp. Versoes, hashes, flags, ordem de fontes e referencias de cada
compilacao sao preservados nos manifestos e recibos. O Roslyn 5.8 dos
auxiliares apenas analisa sintaxe e tipos; nao emite os assemblies alvos.

O motor tools/round7_engine.py exige previsao independente completa e
assinatura original exata antes de compilar cada candidato. Uma previsao
incompleta continua UNKNOWN. A assinatura emitida e conferida novamente e a
adocao exige todos os corpos conhecidos e todos os metadados de tipos, seguida
do verificador oficial integral. O alinhamento com rotulos serve somente ao
diagnostico; nao muda o criterio de igualdade. Nos ciclos, o repertorio antigo
so volta a ser explorado apos avanco da familia nova, evitando repetir buscas
ja esgotadas sobre o mesmo fonte.

Nomes de locais introduzidos ou reordenados sao escolhas da reconstrucao para
reproduzir slots, nao alegacoes de nomes historicos. Nenhuma classe gerada e
escrita a mao, nenhum IL original e inserido no resultado e nenhum campo do
assembly e normalizado.

Evidencias do alocador smcs, capturas, ordem de emissao dos tipos e limites:
[modelo smcs](notes/round7-slot-model.md). Regras, fixtures historicas e
resultados negativos: [regras smcs](notes/round7-smcs-rules.md). Modelo Roslyn,
escalonamento de pilha e icamento do array do iterador:
[modelo Roslyn](notes/round7-roslyn-model.md).

### Round 8: bounded incomplete predictions

Production compilers, options and references remain unchanged. A complete
slot-model prediction which disagrees with the original signature rejects the
candidate before compilation. An incomplete prediction is diagnostic only and
does not reject compilation. The actual compiled local signature, the strict
IL-prefix progression gate, all bodies and all structural metadata are checked
before a source step can be adopted; only the official verifier promotes states.

The budget is 12 compiled incomplete-prediction variants per method per pass,
cumulative across its 24 search iterations. A cycle admits at most one isolated
probe per method plus one full assembly pass: at most 24 variants per method per
cycle. A positive source-screen result requires another real full compilation,
so at most 48 candidate compiler invocations are possible under this budget.
Actual variant and compiler-invocation counts are retained in each engine report.
Complete predictions remain subject to the separate 64-candidate step limit.

The cycle limit is 40, with three consecutive zero-new-matched cycles still a
stop condition. Each rolling block of five cycles includes KairoLibrary. REA
managed evidence at session start is retained in reports/round8-rea-start.json.
No original artifact, generated class, assembly byte or nondeterministic field
is copied or normalized by these source transformations.

## Round 9: control reconstruction and compiler-confirmed source changes

The production compilers, options and references remain unchanged from round 8.
REA managed artifact evidence is retained in `reports/round9-rea-start.json` and
`reports/round9-control-il-REA-identity.json`. The original artifacts stay read-only.

The smcs control extractor reads IL independently of the source AST reader.
`tools/round9_control_il.py` records every instruction, branch width, polarity,
join and EH boundary. Unsupported or ambiguous lexical ownership is incomplete.
`tools/Round9ControlAst.cs` reads source only; Roslyn 5.8 and the already installed
.NET 10.0.10 runtime are analysis dependencies, never production compilers here.
The desktop assembly identity comparer is necessary for analysis-only unification
of Microsoft mscorlib 2.0.0 references and the Unity 2.0.5 consumer profile; its
separate historical-compiler fixture is `reports/round9-control-ast-identity-fixture.json`.
No production reference was changed to resolve the modern analysis diagnostic.

The frozen pre-C matched corpus contains 1,788 smcs bodies: 1,253 independently
complete tree pairs agree, and all 1,422 complete source emission plans agree.
The unique source-leaf association also passes all 1,253 complete matched pairs.
Source text and UTF-16 leaf spans are authenticated against the own-compilation
receipt. The original skeleton guides C# control only; no original CIL is emitted
or inserted into the reconstruction. Full method/header/locals/EH verification
remains the only acceptance oracle. See `docs/notes/round9-control-rewrite.md`
and `reports/round9-control-results.json` for the actual Part-C result.

Recovered unused local names such as `recoveredUnusedSlot4` are synthetic. Their
types and order follow the original signature and zero recorded IL accesses;
the names are not claimed to be historical. A smcs fixture confirms that these
uninitialized declarations can restore the signature without adding instructions.

Round 9 separates the required A-C steps from at most 40 autonomous D cycles.
The stop counter is five consecutive D cycles without a new matched method and
starts only after the prepared queue is empty. A pending prepared patch is not
silently discarded to start the counter. The KairoLibrary rolling five-cycle
quota and the two-consecutive-primary-method limit remain active. Incomplete
slot predictions retain the 12-variant/pass, 24-variant/method/cycle budget.
Complete incompatible predictions still refuse compilation. Every commit
requires fresh full verification and compilation of the Git index sources.
# Nomes de locais recuperados na rodada 9

`recoveredLanguagePackChanged` e `recoveredInventoryResult` são nomes escolhidos no fonte para reproduzir tipos, ordem e duração dos slots originais. Os nomes históricos desses locais são UNKNOWN. Eles não são apresentados como nomes originais nem campos/membros recuperados dos metadados.

O mesmo vale para `recoveredNotifyManager`: o tipo e os acessos são do original; o nome legível é escolhido no fonte e não foi recuperado de PDB original.

