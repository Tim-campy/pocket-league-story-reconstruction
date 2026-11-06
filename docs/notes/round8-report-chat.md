## 1. PROGRESSO

Pocket League Story para Android, APK net.kairosoft.android.soccer_en_2.0.2-APK_Award.apk; reconstrução C#/Unity 5.3.4f1. Comparação com o início desta rodada, commit `25b8108711a4a8920a2600cf9ffea5b710b3a481`. Matched exige corpo completo (cabeçalho, maxstack, locais, IL e EH); referências de tokens são resolvidas por identidade. Declarações sem corpo contam em métodos, sem bytes de IL.

| Assembly | Anterior (25b8108) | Atual | Δ métodos | Δ bytes de IL | Regressões |
| --- | --- | --- | --- | --- | --- |
| KairoLibrary | 3207/3287 (97.566170%); IL 212900/269894 bytes (78.882821%) | 3211/3287 (97.687861%); IL 216823/269894 bytes (80.336354%) | 4 | 3923 | 0 |
| Assembly-CSharp | 1789/1958 (91.368744%); IL 352326/601603 bytes (58.564535%) | 1790/1958 (91.419816%); IL 352587/601603 bytes (58.607919%) | 1 | 261 | 0 |

Total: 5001/5245 métodos; 569410/871497 bytes de IL. Retiradas de matched por ID: 0; corpos antes iguais que deixaram de bater: 0.

Motivo da parada: três ciclos válidos consecutivos sem novo método em matched (5, 6 e 7).

## 2. O QUE FOI FEITO NESTA RODADA

Ganhos oficiais vêm do diário após adoção e verificação completa, incluindo auxiliares liberados pelo verificador. Sondas e avanços parciais são registrados separadamente; não se somam aos ganhos oficiais. Transformações herdadas da rodada 7 só aparecem abaixo quando foram efetivamente tentadas nesta rodada.

| Ciclo | Assembly / regra | Matched ganhos | Bytes ganhos |
| --- | --- | --- | --- |
| 1 | Assembly-CSharp / calculated-store-anchored-local-lca | 1 | 261 |
| 2 | Assembly-CSharp / smcs-value-return-guard-reverse | 0 | 0 |
| 3 | KairoLibrary / roslyn-drawpolygons-declaration-scope-and-explicit-array-indices | 1 | 1563 |
| 4 | KairoLibrary / switch-owned-break-if-chain | 3 | 2360 |
| 5 | KairoLibrary / nullable-conditional-coalesce-explicit | 0 | 0 |
| 6 | Assembly-CSharp / calculated-selector-continue-composition | 0 | 0 |
| 7 | Assembly-CSharp / calculated-shared-payload-switch-guard | 0 | 0 |

C3: o patch DrawPolygons havia sido exportado com LF para uma baseline CRLF. O rebase exato o recusou; a tentativa inicial foi preservada. O transporte corrigiu somente o patch, sem editar o fonte-base; o retry verificou o corpo completo de 1563 bytes.

C4/C5 iniciais foram experimentos INVÁLIDOS: nove triagens rotuladas como candidatos (sete das varreduras e duas sondas) compilavam o arquivo-base desatualizado. Não houve avanço, adoção, promoção ou perda. A função de escrita foi corrigida e testada em cinco compilações toy; os dois experimentos foram arquivados por ferramenta auditável e repetidos como ciclos válidos 4 e 5. Git e os hashes dos ledgers foram preservados; não houve edição manual de estados. O diário científico tem sete ciclos concluídos, além desses dois experimentos inválidos.

Foram removidos apenas 53 arquivos de cache de metadados regenerável (369180435 bytes lógicos), preservando os 16 mais recentes. Compressão NTFS sem perdas liberou espaço nos artefatos de build; não alterou bytes dos assemblies. Fontes, originais e provas congeladas foram preservados.

Frentes preparadas antes da parada, sem teste dos respectivos candidatos no jogo; ganhos e perdas adotados: zero.

| Frente | Preparado / prova | Compilações de candidatos do jogo | Métodos ganhos / perdidos |
| --- | --- | --- | --- |
| Coalescência de coordenadas smcs | 3 alvos; 4316 bytes; 2 corpos de fixture exatos | 0 | 0 / 0 |
| Lifetime/escopo Roslyn | 3 alvos; 4492 bytes; 3 corpos de fixture exatos | 0 | 0 / 0 |
| Ternário negativo após null-conditional | 1 alvos; 1686 bytes; 3 corpos de fixture exatos | 0 | 0 / 0 |
| Expansão de *= em campo float/double | UNKNOWN alvos; UNKNOWN bytes; 4 corpos de fixture exatos | 0 | 0 / 0 |

Variantes incompletas compiladas: 31; invocações dessas variantes: 43. Esses custos incluem sondas; ganhos não são contados duas vezes.

## 3. HIPÓTESES

Hipóteses registradas: CONFIRMADA: 6, INCONCLUSIVA: 5, REFUTADA: 3. Os ganhos por ciclo estão na seção 2; zero ganho permanece INCONCLUSIVA, salvo prova isolada de refutação. A classificação não identifica o fonte histórico literal.

Diagnóstico de polaridade da rodada 8, no snapshot congelado indicado pela prova: escopo limitado às fronteiras selecionadas, não a todos os desvios dos métodos. A correção sobre Act_dribble descreve um teste histórico da rodada 7, sem nova tentativa.

| Estado | Hipótese diagnosticada | Evidência / limite |
| --- | --- | --- |
| REFUTADA | Polarity groups primarily indicate while/do/for conversion | 0 selected source blocks are natural-loop headers;0 selected edges are dominator backedges.2 are body if/break boundaries,6 internal conditionals,18 outside normal natural loops. |
| REFUTADA | Act_dribble failed through source mapping or missing slot-model approval | REFUTED: C17 already mapped the correct if, approved the signature, compiled it, and rejected raw prefix1538->1258. No retry proposed. |

| Assembly/modelo | Matched do corpus | Mapeados | Sem mapeamento | Completos | Completos exatos | Precisão | Cobertura | Mapeados incompletos | Incompletos com tipos iguais |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| KairoLibrary | 3135 | 2804 | 331 | 2237 | 2237 | 100.000000% | 71.355662% | 567 | 310 |
| Assembly-CSharp | 1784 | 1704 | 80 | 1704 | 1704 | 100.000000% | 95.515695% | 0 | 0 |

Precisão usa somente previsões completas; cobertura usa todos os corpos matched do corpus congelado. Incompletos ou métodos gerados sem mapeamento continuam UNKNOWN. Coincidência de tipos em previsão incompleta não é prova do lowering. Previsão incompleta PERMITE compilar: limite de 12 variantes por método/passagem, 24 por método/ciclo, no máximo uma sonda isolada e uma passagem plena. Cada variante pode exigir screen e compilação plena; variantes e invocações são medidas separadamente. Previsão completa com assinatura errada é recusada antes de compilar. Depois da compilação, a assinatura emitida precisa ser a original; previsão completa também precisa concordar com o emissor. Cabeçalho, locais, IL e EH continuam sujeitos ao mesmo verificador integral.

## 4. PENDENTES POR GRUPO

| Assembly | Faixa de IL original | Corpos pendentes | Bytes de IL |
| --- | --- | --- | --- |
| KairoLibrary | <=64 | 3 | 137 |
| KairoLibrary | 65-256 | 26 | 4168 |
| KairoLibrary | 257-1024 | 26 | 14592 |
| KairoLibrary | >1024 | 19 | 34156 |
| Assembly-CSharp | <=64 | 5 | 237 |
| Assembly-CSharp | 65-256 | 30 | 4827 |
| Assembly-CSharp | 257-1024 | 65 | 36268 |
| Assembly-CSharp | >1024 | 67 | 207677 |

Pendentes com assinatura de locais igual: KairoLibrary 32/74 (diferente: 42); Assembly-CSharp 92/167 (diferente: 75).

Corpos já iguais, retidos por vínculo/metadados gerados: 3; 25 bytes. Esses corpos não entram no histograma dos divergentes.

Peso de um par = soma do tamanho integral dos métodos nele; não representa quantidade de bytes diferentes. Alinhamento e rótulos servem ao diagnóstico, sem normalizar o verificador. Seguem os cinco pares de opcodes mais pesados de cada compilador, com IL original e recompilado lado a lado.

**KairoLibrary #1: `stloc.s => stloc.s` — 10 métodos, 10498 bytes.** Exemplo: `kairo.unity.util.JarInflater::GetData(System.String)->System.Byte[]`.

Tentativas compiladas no grupo na rodada 8: nenhum. Histórico 7 compatível: UNKNOWN; não se presume compatibilidade após alteração do fonte. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 432.

| IL original | IL recompilado |
| --- | --- |
| `IL_0024: stloc.s 5` | `IL_0024: stloc.s 4` |
| `IL_0026: ldloc.s 5` | `IL_0026: ldloc.s 4` |

**KairoLibrary #2: `stloc.s => DELETION_POINT` — 3 métodos, 5215 bytes.** Exemplo: `kairo.unity.util.Compressor::Expand(System.Byte[])->System.Byte[]`.

Tentativas compiladas no grupo na rodada 8: alias-field-command: 2, alias-field-occurrence: 3, assignment-ternary-if: 5, assignment-ternary-if-reverse: 4, if-polarity: 1, local-declaration-scope-hoist: 6, switch-owned-break-if-chain: 2, ternary-polarity: 1; 24 com previsão incompleta. Antes de compilar: nenhum; orçamento recusou 12; repertório anterior filtrou 724.

| IL original | IL recompilado |
| --- | --- |
| `IL_0050: stloc.s 4` | `IL_0050: br.s L5` |
| `IL_0052: br.s L6` | `IL_0052: ldloc.s 4` |

**KairoLibrary #3: `brfalse.s => brtrue.s` — 5 métodos, 4079 bytes.** Exemplo: `kairo.unity.util.StreamUtil::WriteArray(java.io.OutputStream,System.Array,System.Int32)->System.Void`.

Tentativas compiladas no grupo na rodada 8: nullable-conditional-coalesce-explicit: 2; 2 com previsão incompleta. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 147.

| IL original | IL recompilado |
| --- | --- |
| `IL_0001: brfalse.s O12` | `IL_0001: brtrue.s L0` |
| `IL_0003: ldarg.1` | `IL_0003: ldc.i4.m1` |

**KairoLibrary #4: `brfalse => brtrue.s` — 2 métodos, 4071 bytes.** Exemplo: `kairo.unity.ui.Balloon::Draw(kairo.unity.ui.Graphics,System.Single,System.Single,UnityEngine.Matrix4x4)->System.Void`.

Tentativas compiladas no grupo na rodada 8: nenhum. Histórico 7 compatível: UNKNOWN; não se presume compatibilidade após alteração do fonte. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 144.

| IL original | IL recompilado |
| --- | --- |
| `IL_0027: brfalse L28` | `IL_0027: brtrue.s L0` |
| `IL_002c: ldc.i4.s 60` | `IL_0029: ret` |

**KairoLibrary #5: `ldc.i4.0 => brtrue.s` — 1 métodos, 3513 bytes.** Exemplo: `kairo.unity.ui.Graphics::_drawBitmap(kairo.unity.ui.Image,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single)->System.Void`.

Tentativas compiladas no grupo na rodada 8: nenhum. Histórico 7 compatível: UNKNOWN; não se presume compatibilidade após alteração do fonte. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 144.

| IL original | IL recompilado |
| --- | --- |
| `IL_0758: ldc.i4.0` | `IL_075b: brtrue.s L36` |
| `IL_0759: ceq` | `IL_075d: ldarg.0` |

**Assembly-CSharp #1: `brfalse => brtrue` — 11 métodos, 31885 bytes.** Exemplo: `form.GameForm::_update(System.Int32)->System.Boolean`.

Tentativas compiladas no grupo na rodada 8: nenhum. Histórico 7 compatível (mesmo original e hash do arquivo fonte atual): if-polarity: 2, local-declaration-scope-hoist: 2, smcs-continue-positive-tail: 1, smcs-empty-arm-restore: 3, solver-group-all-calculated-blocks: 1, solver-group-block-declarations: 3. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 788.

| IL original | IL recompilado |
| --- | --- |
| `IL_00e2: brfalse L5` | `IL_00e2: brtrue R265` |
| `IL_00e7: br L7` | `IL_00e7: ldarg.0` |

**Assembly-CSharp #2: `br => ret` — 9 métodos, 21529 bytes.** Exemplo: `form.SoccerForm::Draw_newPlayerPopular(kairo.unity.ui.Graphics)->System.Void`.

Tentativas compiladas no grupo na rodada 8: nenhum. Histórico 7 compatível (mesmo original e hash do arquivo fonte atual): smcs-empty-arm-restore: 8, smcs-empty-arm-restore-and-suffix: 2, smcs-empty-arm-restore-first-and: 2. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 350.

| IL original | IL recompilado |
| --- | --- |
| `IL_0210: br L5` | `IL_0210: ret` |
| `IL_0215: ldarg.0` | `IL_0211: ldarg.0` |

**Assembly-CSharp #3: `bne.un => beq` — 14 métodos, 19612 bytes.** Exemplo: `form.SubForm::Init()->System.Boolean`.

Tentativas compiladas no grupo na rodada 8: calculated-selector-continue-composition: 2, smcs-value-return-guard-reverse: 1. Antes de compilar: wrong_signature: 6; orçamento recusou 0; repertório anterior filtrou 2293.

| IL original | IL recompilado |
| --- | --- |
| `IL_014a: bne.un L7` | `IL_014a: beq L97` |
| `IL_014f: br L97` | `IL_014f: ldarg.0` |

**Assembly-CSharp #4: `stloc.s => stloc.s` — 11 métodos, 16559 bytes.** Exemplo: `form.DebugForm::Draw_coatEva(kairo.unity.ui.Graphics)->System.Void`.

Tentativas compiladas no grupo na rodada 8: nenhum. Histórico 7 compatível: UNKNOWN; não se presume compatibilidade após alteração do fonte. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 1058.

| IL original | IL recompilado |
| --- | --- |
| `IL_01a1: stloc.s 6` | `IL_01a1: stloc.s 12` |
| `IL_01a3: ldloc.s 5` | `IL_01a3: ldloc.s 5` |

**Assembly-CSharp #5: `INSERTION_POINT => stloc.1` — 2 métodos, 12768 bytes.** Exemplo: `form.SubForm::Update2()->System.Boolean`.

Tentativas compiladas no grupo na rodada 8: nenhum. Histórico 7 compatível: UNKNOWN; não se presume compatibilidade após alteração do fonte. Antes de compilar: wrong_signature: 26; orçamento recusou 0; repertório anterior filtrou 2145.

| IL original | IL recompilado |
| --- | --- |
| `IL_0029: brfalse L0` | `IL_0029: stloc.1` |
| `IL_002e: ldarg.0` | `IL_002a: ldloc.1` |

## 5. TOOLCHAIN

KairoLibrary: NuGet Microsoft.Net.Compilers 1.1.0; csc.exe 1.1.0.51109; compilador SHA-256 `53b693536e8a86f35d05d2165ff9b3e0aeaa499ecfd8135a9f99ab4404ca86c3`. Mudou compilador/opções/referências nesta rodada: NÃO.

Opções: `/nologo /noconfig /nostdlib+ /target:library /platform:anycpu /optimize+ /debug:pdbonly /deterministic- /langversion:6 /unsafe- /checked- /codepage:65001 /utf8output`.

Referências em ordem: mscorlib.dll, System.Core.dll, UnityEngine.dll, System.dll.

Assembly-CSharp: smcs.exe 2.6.5.0 do Unity 5.3.4f1; compilador SHA-256 `26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0`. Mudou compilador/opções/referências nesta rodada: NÃO.

Opções: `-nologo -noconfig -target:library -platform:anycpu -optimize+ -debug- -langversion:Default -unsafe- -checked- -codepage:65001`.

Referências em ordem: mscorlib.dll, KairoLibrary.dll, UnityEngine.dll, System.Core.dll, System.dll.

Compilador histórico exato, comando histórico integral e hashes das referências originalmente usadas: UNKNOWN. Os candidatos acima têm hashes fixados e prova de corpos; isso não identifica sua fabricação histórica. Roslyn moderno compila auxiliares e fornece AST, não os assemblies alvos. PDB/MDB auxiliam apenas o mapeamento. Nomes escolhidos para locais não são apresentados como nomes originais.

## 6. COMPONENTES EXTERNOS E METADADOS

| Assembly | Tipos com metadados iguais | Declarações sem corpo aprovadas | Ordem de tipos | Metadados globais | Bytes brutos |
| --- | --- | --- | --- | --- | --- |
| KairoLibrary | 337/337 | 76/76 | True | True | NOT_EVALUATED |
| Assembly-CSharp | 129/129 | 6/6 | True | True | NOT_EVALUATED |

A comparação abrange ordem de tipos e membros, parâmetros, atributos personalizados, layout, constantes, recursos e referências globais; o guard de identidade confere o SHA do original e do build atual. Diferenças brutas não medidas continuam UNKNOWN. Nenhum byte original é inserido no resultado, nenhuma classe gerada é escrita à mão e nenhum assembly é normalizado.

Externos gerenciados com correspondência oficial registrada: 7/7: Mono.Security.dll, System.Core.dll, System.dll, UnityEngine.Networking.dll, UnityEngine.UI.dll, UnityEngine.dll, mscorlib.dll. UnityEngine.dll: BYTE_IDENTICAL_OFFICIAL_ANDROID_MONO; SHA-256 `7cc901371cc77e103d00b5845e8285f85fbdf36bb47ccf3d824f94c4ead32421`. A variante é Android/Mono do Unity 5.3.4f1; evidência histórica, não nova decompilação.

Externos nativos com correspondência oficial registrada: 6/6; lib/armeabi-v7a/libmain.so, lib/armeabi-v7a/libmono.so, lib/armeabi-v7a/libunity.so, lib/x86/libmain.so, lib/x86/libmono.so, lib/x86/libunity.so. DEX/Java permanece fora do escopo.

Campos não determinísticos comprovados por pares de recompilações anteriores (não normalizados): KairoLibrary — Module.Mvid, COFF.TimeDateStamp, IMAGE_DEBUG_DIRECTORY.TimeDateStamp, CodeView.RSDS.PdbGuid; Assembly-CSharp — Module.Mvid, COFF.TimeDateStamp. Determinismo histórico do original: UNKNOWN.

## 7. BLOQUEIOS

Nenhuma instalação, arquivo ou decisão do usuário é necessária. A rodada parou pela condição experimental de três ciclos sem ganho; hipóteses e fontes preparadas não foram compiladas depois da parada.

Parada após 7 ciclos; sequência final sem novo matched: 3. Limites configurados: 40 ciclos, 3 sem ganho consecutivos; cota Kairo: at least one KairoLibrary cycle in every consecutive block of five cycles.

## 8. SUA RECOMENDAÇÃO

1. Testar os patches de lifetime/escopo Roslyn já preparados para StringUtil.GetCode e Graphics.DrawArc: dois alvos, 2514 bytes de IL, potencial máximo de dois métodos; ganho efetivo UNKNOWN. GetCode prevê 31 slots completos, DrawArc tem variante de 28 tipos ainda incompleta e pode ser compilada pelo protocolo vigente.
2. Testar a composição de acesso nulo com ternário negativo em StreamUtil.WriteArray: um alvo, 1686 bytes, potencial máximo de um método; os 14 bytes iniciais foram reproduzidos em fixture histórica, corpo completo do jogo UNKNOWN. A regra não está integrada e só um dos cinco membros do grupo é elegível.
3. Aplicar os três patches calculados de coalescência de coordenadas nos métodos Draw_coatEva, SetSavePos e Draw_moveArea: três alvos, 4316 bytes, potencial máximo de três métodos; previsões completas/exatas e fixtures aprovadas, ganhos do jogo UNKNOWN. Para SpForm.Update, o ponto de retomada medido é o operando do br em IL_04fe, primeira diferença no byte1279; sua assinatura de24 locais já está correta.
