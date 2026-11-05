## 1. PROGRESSO

Rodada iniciada no commit `2ab24ee6f9b8d8f204c587b878d38ed120151467`. Percentuais expressos como frações exatas.

| Assembly | Total | Matched anterior | Percentual anterior | Matched atual | Percentual atual | Ganho |
|---|---:|---:|---|---:|---|---:|
| KairoLibrary | 3.287 | 3.027 | (3027/3287) × 100% | 3.123 | (3123/3287) × 100% | 96 |
| Assembly-CSharp | 1.958 | 1.531 | (1531/1958) × 100% | 1.572 | (1572/1958) × 100% | 41 |
| Total | 5.245 | 4.558 | (4558/5245) × 100% | 4.695 | (4695/5245) × 100% | 137 |

Zero regressões de corpos, zero retiradas de matched e zero regressões de tipos adotados. Os 137 ganhos são 55 novos corpos idênticos e 82 declarações sem CIL aprovadas por metadados.

Kairo: 3.051 corpos idênticos, 76 declarações aprovadas, quatro corpos iguais retidos, 160 corpos divergentes. App: 1.567 corpos idênticos, seis declarações aprovadas, um corpo igual retido, 385 divergentes. Restam 550 métodos fora de matched: 545 divergentes e cinco retidos. Bytes IL matched: 340.534 → 352.867, de 871.497.

## 2. O QUE FOI FEITO NESTA RODADA

Ganhos/perdas são estados matched, salvo a linha explicitamente rejeitada.

| Regra/frente | Tentativa e resultado | Ganhou | Perdeu |
|---|---|---:|---:|
| Retorno antecipado final, smcs | Um alvo; Receipt.DebugVerificationReceipt confirmado. | 1 | 0 |
| Retorno antecipado final, Roslyn | Nenhum alvo no filtro estrito de dois ret consecutivos. | 0 | 0 |
| Local antes do retorno, smcs | 15 alvos, 13 aprovados; inclui AppData.GetAllStock. | 13 | 0 |
| Local antes do retorno, Roslyn | Regra não generalizada entre compiladores; nenhum alvo adotado. | 0 | 0 |
| Polaridade de ternários, smcs | 39 alvos; nenhum aprovado. | 0 | 0 |
| Polaridade de ternários, Roslyn | 11 alvos; Image.GetGraphics confirmado. | 1 | 0 |
| Inversão de if/else, smcs | 66 alvos; nenhum aprovado. | 0 | 0 |
| Inversão de if/else, Roslyn | 16 alvos; nenhum aprovado. | 0 | 0 |
| Switch simples → if, smcs | 41 alvos; 21 aprovados; remove temporários do switch recuperado. | 21 | 0 |
| Ordem geral de membros e namespaces | Todos os 466 tipos contabilizados; oito C# Kairo, um C# App e compile-order Kairo alterados. | 0 | 0 |
| Ordinal da lambda SoundBase | soundList_ movido depois de state_; getter e lambda passam. | 2 | 0 |
| Construtores explícitos vazios | SurfaceManagerBase e ShaderBuilder: ordem MethodDef corrigida. | 0 | 0 |
| Verificador de metadados | Provas estritas do método, tipo e ordinal liberam 76 + 6 declarações. | 82 | 0 |
| unsafe+ → unsafe- | Remove atributos globais extras, sem mudar correspondências de corpos/tipos. | 0 | 0 |
| Roslyn 1.1 adotado | Build completo e metadados: 17 ganhos, nenhuma perda. | 17 | 0 |
| Roslyn 1.2 testado | Mesmo conjunto e mesmas saídas de métodos que 1.3.2. | 0 | 0 |
| Switch string de ResourceManager | Remove ComputeStringHash; um tipo adicional aprovado, corpo da lambda ainda divergente. | 0 | 0 |
| Inventário DEX e extrato GetStatusBarHeight | Somente inventário/listagem, sem decompilação Java nem ajuste do método. | 0 | 0 |

Roslyn 1.0 foi rejeitado: ganhou 14 corpos e perdeu 35 contra 1.3.2, saldo -21. Esses resultados não entraram no saldo adotado.

Resultados negativos adicionais: a primeira aplicação do transformador foi rejeitada por spans incompatíveis com CRLF, sem alteração do main; corrigido com coordenadas UTF-16 e testes. O auxiliar Roslyn usado para enumerar sintaxe App registra erros de binding entre os perfis 2.0 e 2.0.5; ele não julga compilabilidade nem correspondência. O build smcs completo e o verificador decidem. Duas variantes de local de retorno e 20 variantes switch-if não foram adotadas. A eliminação do hash não resolveu o corpo da lambda: 1.928 bytes originais, 1.929 candidatos, 37 locais iguais; várias instruções, cabeçalho e EH ainda diferem.

Validação: 46 controles dos comparadores, recibos, ordenação e edição sintática passaram; três controles DEX e três controles da comparação de conjuntos também passaram. Verificação completa final compilou ambos os assemblies, conferiu 1.218 entradas originais e 5.163 corpos de referência. Retorno global 1 corresponde às diferenças restantes; não houve erro de compilação.

## 3. HIPÓTESES

| Estado | Hipótese e evidência |
|---|---|
| CONFIRMADA | O retorno antecipado proposto para Receipt reproduz o corpo original: método promovido pelo verificador. |
| CONFIRMADA | Declarar e devolver um local reproduz GetAllStock e outros 12 métodos smcs. |
| CONFIRMADA | Inverter condição/ramos do ternário de Image reproduz a lambda original de GetGraphics. |
| REFUTADA como regra suficiente | Inverter if/else ou ternários indiscriminadamente resolve os grupos: 66 + 16 ifs e 39 ternários smcs não produziram ganhos. |
| CONFIRMADA em 21 casos | Switch recuperado introduzia um local ausente no original: trocar por ifs eliminou a divergência integral nesses métodos. |
| INCONCLUSIVA como regra universal | Cada local smcs tem origem declarada ou temporário conhecido: no corpus de 1.407 métodos explícitos sem lambda, 1.374 contagens coincidem após incluir catch/foreach; os 33 restantes foram correlacionados a switch, retorno através de EH, foreach, receiver struct, atribuições encadeadas e pós-incremento, sem prova formal por slot. |
| CONFIRMADA no candidato testado | Roslyn emite namespaces na ordem inversa recursiva da primeira aparição: seis fixtures e igualdade final dos 337 TypeDefs. |
| CONFIRMADA | O ID incorreto de SoundBase vinha da posição de soundList_: mover o campo troca b__12_0 por b__11_0 e acrescenta dois corpos iguais. |
| CONFIRMADA | unsafe+ gerava metadados globais extras: fixtures e builds completos unsafe- removem exatamente os registros ausentes nos originais. |
| CONFIRMADA | Roslyn 1.1 supera os vizinhos testados neste fonte: 3.051 corpos, com +17/-0 contra 1.3.2. |
| INCONCLUSIVA | 1.1 era o compilador histórico: melhor correspondência não identifica unicamente executável/comando original. |
| CONFIRMADA | ComputeStringHash adicional vinha do switch string recuperado: a cadeia original de nove comparações elimina o helper e deixa zero IDs adicionais. |
| CONFIRMADA apenas para o candidato | Quatro campos são variáveis entre builds idênticos do 1.1; zero alterações fora deles. |
| INCONCLUSIVA | Determinismo da DLL original: ausência de REPRO não basta para provar o modo histórico. |

## 4. PENDENTES POR GRUPO

H = cabeçalho; L = assinatura de locais; I = instruções/operandos/desvios; E = regiões de exceção ou seções adicionais. Grupos exclusivos; não são causas comprovadas.

| Grupo | Kairo | App | Total |
|---|---:|---:|---:|
| H + I | 51 | 178 | 229 |
| H + L + I | 26 | 124 | 150 |
| I | 31 | 24 | 55 |
| H + I + E | 16 | 28 | 44 |
| H + L + I + E | 18 | 20 | 38 |
| L + I | 17 | 10 | 27 |
| L + I + E | 1 | 0 | 1 |
| L | 0 | 1 | 1 |
| Total | 160 | 385 | 545 |

Os cinco auxiliares com corpo idêntico e vínculo pendente ficam fora da tabela. Não receberam ação direta.

**H + I:** `Assembly-CSharp::system.tapjoy.ui.Tapjoy::Quit()->System.Void`. Cabeçalhos tiny, sem locais; CodeSize 24 → 23.

| Original | Recompilado |
|---|---|
| `0000 ldarg.0` | `0000 ldarg.0` |
| `0001 ldfld Tapjoy::initialize_` | `0001 ldfld Tapjoy::initialize_` |
| `0006 brtrue 000c` | `0006 brfalse 0016` |
| `000b ret` | `000b ldarg.0` |
| `000c ldarg.0` | `000c ldstr "[Tapjoy] Quit"` |
| `000d ldstr "[Tapjoy] Quit"` | `0011 call Tapjoy::Print(string)` |
| `0012 call Tapjoy::Print(string)` | `0016 ret` |
| `0017 ret` | — |

**H + L + I:** `Assembly-CSharp::main.WebBox::Create()->main.WebBox`. Original fat/MaxStack3/InitLocals/um Boolean; recompilado tiny/MaxStack8/sem locais.

| Original | Recompilado |
|---|---|
| `0000 call WebBoxPlugin::Create()` | `0000 call WebBoxPlugin::Create()` |
| `0005 stloc.0` | `0005 brtrue 000c` |
| `0006 ldloc.0` | `000a ldnull` |
| `0007 brtrue 000e` | `000b ret` |
| `000c ldnull` | `000c newobj WebBox::.ctor()` |
| `000d ret` | `0011 ret` |
| `000e newobj WebBox::.ctor()` | — |
| `0013 ret` | — |

**I:** `Assembly-CSharp::system.billing.ui.Receipt::GetItemType(UnityEngine.Purchasing.ProductType)->System.String`. Ambos tiny, 23 bytes, MaxStack8, sem locais.

| Original | Recompilado |
|---|---|
| `0000 ldarg.0` | `0000 ldarg.0` |
| `0001 ldc.i4.2` | `0001 ldc.i4.2` |
| `0002 bne.un 0011` | `0002 beq 0011` |
| `0007 ldstr "subs"` | `0007 ldstr "inapp"` |
| `000c br 0016` | `000c br 0016` |
| `0011 ldstr "inapp"` | `0011 ldstr "subs"` |
| `0016 ret` | `0016 ret` |

Offsets são hexadecimais; nomes de operandos abreviados apenas na apresentação.

## 5. TOOLCHAIN

**Kairo:** pacote Microsoft.Net.Compilers 1.1.0; csc 1.1.0.51109. SHA-256 pacote `cd84fb2c71cafe03cdae0add609fa12c75f61c89ea87b1422fd108ac7e570745`; csc `53b693536e8a86f35d05d2165ff9b3e0aeaa499ecfd8135a9f99ab4404ca86c3`.

```text
/nologo /noconfig /nostdlib+ /target:library /platform:anycpu
/optimize+ /debug:pdbonly /deterministic- /langversion:6
/unsafe- /checked- /codepage:65001 /utf8output
```

Referências: mscorlib 2.0.0.0, System.Core 3.5.0.0, UnityEngine do APK, System 2.0.0.0. Perfil clássico Unity; sem defines extras; 163 C#. Ordem de compilação ajustada.

**App:** Unity 5.3.4f1, smcs 2.6.5.0; SHA-256 smcs.exe `26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0`.

```text
-nologo -noconfig -target:library -platform:anycpu -optimize+
-debug- -langversion:Default -unsafe- -checked- -codepage:65001
```

Biblioteca padrão habilitada, sem -nostdlib. Referências do APK: mscorlib 2.0.5.0, KairoLibrary, UnityEngine, System.Core 2.0.5.0, System 2.0.5.0. MONO_PATH/MONO_CFG_DIR do Unity locais ao subprocesso; 97 C#. noconfig fica fora do response file nos dois.

Mudanças: Kairo 1.3.2 → 1.1.0; unsafe desabilitado em ambos; ordem de fontes Kairo corrigida. UNKNOWN: compiladores históricos exatos, comando original completo, símbolos e opções históricas. O APK e os assemblies não foram executados.

Matriz antes da correção final do switch string:

| Pacote / executável | Corpos iguais | Ganhos / perdas contra 1.3.2 | Exclusivos entre os quatro |
|---|---:|---|---:|
| 1.0.0 / 1.0.0.50618 | 3.013 | +14 / -35 | 0 |
| 1.1.0 / 1.1.0.51109 | 3.051 | +17 / -0 | 3 |
| 1.2.0 / 1.2.0.60317 | 3.034 | +0 / -0 | 0 |
| 1.3.2 / 1.3.1.60621 | 3.034 | referência | 0 |

Exclusivos do 1.1: MeshBuilder.AddQuad(RectangleF,Single,Single,Single,Single,Color), AddQuadIndicies() e AddTriangleIndicies(). Os outros 14 ganhos também aparecem no 1.0. O 1.0 rejeita /deterministic-; essa opção foi omitida somente nele. /version não é suportado pelos vizinhos; versões obtidas do banner. Não confundir o empate com 1.2 com o resultado diferente de 1.1. A matriz não foi repetida após a última correção estrutural; o 1.1 final foi verificado integralmente.

## 6. COMPONENTES EXTERNOS E METADADOS

As sete DLLs gerenciadas externas continuam idênticas às oficiais Unity 5.3.4f1: Mono.Security, mscorlib, System, System.Core, UnityEngine.Networking, UnityEngine.UI e UnityEngine. A última é Android/Mono, 970.752 bytes, SHA-256 `7cc901371cc77e103d00b5845e8285f85fbdf36bb47ccf3d824f94c4ead32421`. As seis bibliotecas nativas ARMv7/x86 também foram identificadas; libunity corresponde a Mono Release.

Metadados: Kairo 332/337 tipos; App 128/129; total 460/466. Antes da rodada havia zero tipos registrados como aprovados pelo verificador completo. A ordem TypeDef coincide nos 466; FieldDef/MethodDef, parâmetros, atributos, layout, constantes e recursos são comparados por identidade resolvida. Ambos têm zero recursos embutidos e zero diferenças globais nas dimensões comparadas. Heaps, tabelas brutas e arquivo integral continuam NOT_EVALUATED.

| Falha de metadados | Kairo | App |
|---|---:|---:|
| Inventário de campos | 3 | 0 |
| Ordem de campos | 3 | 0 |
| Interfaces | 2 | 0 |
| Metadados de métodos | 0 | 1 |

As três ocorrências de campos são os mesmos três tipos; contagens não exclusivas. Tipos pendentes:
- kairo.unity.ui.ResourceManager/<>c__DisplayClass51_0, kairo.unity.io.HttpObject/<>c__DisplayClass10_1, kairo.unity.form.FormManagerBase/<>c__DisplayClass85_1: inventário/ordem dos campos.
- kairo.unity.util.FastVector\`1/<GetEnumerator>d__42 e java.util.Vector\`1/<GetEnumerator>d__40: interfaces.
- system.form.LanguageForm: oito nomes de parâmetros em quatro métodos gerados de Update diferem.

IDs estruturais da rodada:
- Ausente original `kairo.unity.ui.SoundBase::<get_state_>b__11_0()->System.Object`: agora presente.
- Extra `kairo.unity.ui.SoundBase::<get_state_>b__12_0()->System.Object`: eliminado.
- Extra `<PrivateImplementationDetails>::ComputeStringHash(System.String)->System.UInt32`: eliminado.
Ambos os assemblies terminam com zero IDs ausentes ou adicionais.

Campos variáveis, comprovados para Kairo 1.1 unsafe-/pdbonly/deterministic-: Module.Mvid (16 bytes; 15 mudaram), COFF.TimeDateStamp (4; um mudou), IMAGE_DEBUG_DIRECTORY.TimeDateStamp (4; um mudou), CodeView.RSDS.PdbGuid (16; 16 mudaram). Total 33 bytes diferentes entre dois builds idênticos, zero fora dos intervalos. Nenhuma normalização foi aplicada. Não estender a App; determinismo original UNKNOWN.

**Inventário DEX, sem decompilação:** DEX035, 3.232 classes/102 pacotes, SHA-256 `4b5d18e92e9364ffd22cdaed6d47b470a4881120cd5b8ab35b3c223ed5753f94`. Foram lidos cabeçalho, strings/type_ids/class_defs; nenhum class_data, code_item ou método. Unity: 79 classes em três pacotes, corroboradas por nomes no classes.jar oficial. SDK: 3.052 classes/84 pacotes, classificação inferida por nomes/superclass/source_file; versão histórica UNKNOWN. Próprio candidato: 101 classes/15 pacotes; propriedade inferida, lógica de jogo UNKNOWN. Inclui cinco R/R$* geradas. Chamadas AndroidJavaClass no C# corroboram plugin.Config/Utility/WebBox e kairo.android.plugin.Config/Utility. O escopo Java não foi ampliado.

| Pacote | Classes | Classificação |
|---|---:|---|
| `android.support.annotation` | 41 | SDK de terceiros |
| `android.support.v4.accessibilityservice` | 7 | SDK de terceiros |
| `android.support.v4.animation` | 12 | SDK de terceiros |
| `android.support.v4.app` | 210 | SDK de terceiros |
| `android.support.v4.content` | 54 | SDK de terceiros |
| `android.support.v4.content.pm` | 1 | SDK de terceiros |
| `android.support.v4.content.res` | 5 | SDK de terceiros |
| `android.support.v4.database` | 1 | SDK de terceiros |
| `android.support.v4.graphics` | 10 | SDK de terceiros |
| `android.support.v4.graphics.drawable` | 32 | SDK de terceiros |
| `android.support.v4.hardware.display` | 4 | SDK de terceiros |
| `android.support.v4.hardware.fingerprint` | 13 | SDK de terceiros |
| `android.support.v4.internal.view` | 3 | SDK de terceiros |
| `android.support.v4.media` | 127 | SDK de terceiros |
| `android.support.v4.media.session` | 79 | SDK de terceiros |
| `android.support.v4.net` | 17 | SDK de terceiros |
| `android.support.v4.os` | 22 | SDK de terceiros |
| `android.support.v4.print` | 14 | SDK de terceiros |
| `android.support.v4.provider` | 6 | SDK de terceiros |
| `android.support.v4.speech.tts` | 5 | SDK de terceiros |
| `android.support.v4.text` | 25 | SDK de terceiros |
| `android.support.v4.util` | 24 | SDK de terceiros |
| `android.support.v4.view` | 205 | SDK de terceiros |
| `android.support.v4.view.accessibility` | 70 | SDK de terceiros |
| `android.support.v4.view.animation` | 8 | SDK de terceiros |
| `android.support.v4.widget` | 158 | SDK de terceiros |
| `bitter.jnibridge` | 2 | Unity |
| `com.android.vending.licensing` | 23 | SDK de terceiros |
| `com.android.vending.licensing.util` | 2 | SDK de terceiros |
| `com.google.ads` | 4 | SDK de terceiros |
| `com.google.ads.mediation` | 11 | SDK de terceiros |
| `com.google.ads.mediation.admob` | 4 | SDK de terceiros |
| `com.google.ads.mediation.customevent` | 10 | SDK de terceiros |
| `com.google.ads.mediation.jsadapter` | 5 | SDK de terceiros |
| `com.google.android.gms.ads` | 9 | SDK de terceiros |
| `com.google.android.gms.ads.doubleclick` | 6 | SDK de terceiros |
| `com.google.android.gms.ads.identifier` | 2 | SDK de terceiros |
| `com.google.android.gms.ads.mediation` | 1 | SDK de terceiros |
| `com.google.android.gms.ads.mediation.admob` | 1 | SDK de terceiros |
| `com.google.android.gms.ads.mediation.customevent` | 1 | SDK de terceiros |
| `com.google.android.gms.ads.search` | 4 | SDK de terceiros |
| `com.google.android.gms.analytics` | 86 | SDK de terceiros |
| `com.google.android.gms.appstate` | 27 | SDK de terceiros |
| `com.google.android.gms.auth` | 5 | SDK de terceiros |
| `com.google.android.gms.cast` | 52 | SDK de terceiros |
| `com.google.android.gms.common` | 14 | SDK de terceiros |
| `com.google.android.gms.common.annotation` | 1 | SDK de terceiros |
| `com.google.android.gms.common.api` | 29 | SDK de terceiros |
| `com.google.android.gms.common.data` | 14 | SDK de terceiros |
| `com.google.android.gms.common.images` | 13 | SDK de terceiros |
| `com.google.android.gms.common.internal.safeparcel` | 4 | SDK de terceiros |
| `com.google.android.gms.drive` | 30 | SDK de terceiros |
| `com.google.android.gms.drive.events` | 8 | SDK de terceiros |
| `com.google.android.gms.drive.internal` | 113 | SDK de terceiros |
| `com.google.android.gms.drive.metadata` | 3 | SDK de terceiros |
| `com.google.android.gms.drive.metadata.internal` | 11 | SDK de terceiros |
| `com.google.android.gms.drive.query` | 6 | SDK de terceiros |
| `com.google.android.gms.drive.query.internal` | 15 | SDK de terceiros |
| `com.google.android.gms.drive.widget` | 1 | SDK de terceiros |
| `com.google.android.gms.dynamic` | 16 | SDK de terceiros |
| `com.google.android.gms.games` | 28 | SDK de terceiros |
| `com.google.android.gms.games.achievement` | 6 | SDK de terceiros |
| `com.google.android.gms.games.leaderboard` | 19 | SDK de terceiros |
| `com.google.android.gms.games.multiplayer` | 20 | SDK de terceiros |
| `com.google.android.gms.games.multiplayer.realtime` | 17 | SDK de terceiros |
| `com.google.android.gms.games.multiplayer.turnbased` | 17 | SDK de terceiros |
| `com.google.android.gms.games.request` | 9 | SDK de terceiros |
| `com.google.android.gms.gcm` | 2 | SDK de terceiros |
| `com.google.android.gms.identity.intents` | 14 | SDK de terceiros |
| `com.google.android.gms.identity.intents.model` | 4 | SDK de terceiros |
| `com.google.android.gms.internal` | 672 | SDK de terceiros |
| `com.google.android.gms.location` | 20 | SDK de terceiros |
| `com.google.android.gms.maps` | 46 | SDK de terceiros |
| `com.google.android.gms.maps.internal` | 67 | SDK de terceiros |
| `com.google.android.gms.maps.model` | 48 | SDK de terceiros |
| `com.google.android.gms.maps.model.internal` | 24 | SDK de terceiros |
| `com.google.android.gms.panorama` | 12 | SDK de terceiros |
| `com.google.android.gms.plus` | 36 | SDK de terceiros |
| `com.google.android.gms.plus.internal` | 26 | SDK de terceiros |
| `com.google.android.gms.plus.model.moments` | 5 | SDK de terceiros |
| `com.google.android.gms.plus.model.people` | 17 | SDK de terceiros |
| `com.google.android.gms.tagmanager` | 200 | SDK de terceiros |
| `com.google.android.gms.wallet` | 54 | SDK de terceiros |
| `com.mnp.popups` | 4 | SDK de terceiros |
| `com.mnp.utils` | 1 | SDK de terceiros |
| `com.unity3d.player` | 75 | Unity |
| `kairo.android.plugin` | 9 | Próprio candidato |
| `kairo.android.plugin.ad` | 2 | Próprio candidato |
| `kairo.android.plugin.gms` | 9 | Próprio candidato |
| `kairo.android.plugin.io` | 5 | Próprio candidato |
| `kairo.android.plugin.licensing` | 3 | Próprio candidato |
| `kairo.android.plugin.notification` | 7 | Próprio candidato |
| `kairo.android.plugin.notification.util` | 2 | Próprio candidato |
| `kairo.android.plugin.panel` | 24 | Próprio candidato |
| `kairo.android.plugin.ui` | 6 | Próprio candidato |
| `kairo.android.plugin.util` | 13 | Próprio candidato |
| `main` | 1 | Próprio candidato |
| `net.kairosoft.android.soccer_en` | 5 | Próprio candidato |
| `org.fmod` | 2 | Unity |
| `plugin` | 4 | Próprio candidato |
| `plugin.billing` | 5 | Próprio candidato |
| `plugin.ui` | 6 | Próprio candidato |

**GetStatusBarHeight — IL integral, sem novos ajustes ou tentativas direcionadas.**

ID `KairoLibrary::kairo.unity.native.KairoPlugin::GetStatusBarHeight()->System.Int32`. O corpo final 1.1 foi comparado ao extrato e permanece idêntico à recompilação abaixo. Original 102 bytes/29 instruções; recompilado 100/27. Ambos: fat12, flags0x13, MaxStack3, InitLocals true, zero locais/EH/seções extras.

Aliases: K = kairo.unity.native.KairoPlugin; C = K/<>c; UI = kairo.unity.ui.IApplication; D = UI/UIMethod.

Original:

```il
IL_0000: ldsfld K::statusBarHeight_
IL_0005: ldc.i4 2147483647
IL_000a: beq.s IL_0014
IL_000c: ldsfld K::statusBarHeight_
IL_0011: ldc.i4.0
IL_0012: pop
IL_0013: pop
IL_0014: ldsfld K::app_
IL_0019: callvirt UI::CheckUiThread()->System.Boolean
IL_001e: brfalse.s IL_002c
IL_0020: call K::_getStatusBarHeight()->System.Int32
IL_0025: stsfld K::statusBarHeight_
IL_002a: br.s IL_0060
IL_002c: ldsfld K::app_
IL_0031: ldsfld C::<>9__29_0
IL_0036: dup
IL_0037: brtrue.s IL_0050
IL_0039: pop
IL_003a: ldsfld C::<>9
IL_003f: ldftn C::<GetStatusBarHeight>b__29_0()->System.Object
IL_0045: newobj D::.ctor(System.Object,System.IntPtr)->System.Void
IL_004a: dup
IL_004b: stsfld C::<>9__29_0
IL_0050: ldc.i4.1
IL_0051: callvirt UI::RunOnUiThread(D,System.Boolean)->System.Object
IL_0056: unbox.any System.Int32
IL_005b: stsfld K::statusBarHeight_
IL_0060: ldsfld K::statusBarHeight_
IL_0065: ret
```

Recompilado:

```il
IL_0000: ldsfld K::statusBarHeight_
IL_0005: ldc.i4 2147483647
IL_000a: beq.s IL_0012
IL_000c: ldsfld K::statusBarHeight_
IL_0011: pop
IL_0012: ldsfld K::app_
IL_0017: callvirt UI::CheckUiThread()->System.Boolean
IL_001c: brfalse.s IL_002a
IL_001e: call K::_getStatusBarHeight()->System.Int32
IL_0023: stsfld K::statusBarHeight_
IL_0028: br.s IL_005e
IL_002a: ldsfld K::app_
IL_002f: ldsfld C::<>9__29_0
IL_0034: dup
IL_0035: brtrue.s IL_004e
IL_0037: pop
IL_0038: ldsfld C::<>9
IL_003d: ldftn C::<GetStatusBarHeight>b__29_0()->System.Object
IL_0043: newobj D::.ctor(System.Object,System.IntPtr)->System.Void
IL_0048: dup
IL_0049: stsfld C::<>9__29_0
IL_004e: ldc.i4.1
IL_004f: callvirt UI::RunOnUiThread(D,System.Boolean)->System.Object
IL_0054: unbox.any System.Int32
IL_0059: stsfld K::statusBarHeight_
IL_005e: ldsfld K::statusBarHeight_
IL_0063: ret
```

A causa de ldc.i4.0/pop/pop versus pop continua UNKNOWN.

## 7. BLOQUEIOS

Nenhuma instalação, arquivo ou autorização adicional é necessária para continuar C#. Restam 545 corpos divergentes, cinco auxiliares retidos e seis tipos de metadados diferentes. Identidades históricas exatas dos compiladores e comparação bruta dos heaps/arquivos continuam UNKNOWN ou pendentes.

Depende do usuário decidir se o futuro escopo deve incluir os 101 candidatos Java; não é bloqueio para C#. Não houve decompilação Java nem ação direta nos cinco auxiliares.

## 8. SUA RECOMENDAÇÃO

1. Ampliar a regra de retorno antecipado para guardas seguidos de código, como Tapjoy.Quit: conjunto H+I com 229 alvos; teto 229 ganhos, ganho efetivo UNKNOWN. O filtro desta rodada exigia dois ret consecutivos e não cobriu esse exemplo.
2. Recuperar temporários de condição a partir de stloc/ldloc, como WebBox.Create: conjunto H+L+I com 150 alvos; teto 150, ganho efetivo UNKNOWN. Não generalizar a regra de local de retorno para esse padrão diferente.
3. Restaurar os oito nomes de parâmetros dos quatro métodos gerados de LanguageForm pelo fonte de origem: alvo um tipo; expectativa de zero novos corpos por mera troca de nomes, aprovação do tipo ainda depende do verificador. Preservar nomes de membros e ordinais; não escrever métodos gerados à mão.
