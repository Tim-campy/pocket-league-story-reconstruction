1. PROGRESSO

| Assembly | Anterior | Atual | Regressões |
| --- | --- | --- | --- |
| KairoLibrary | 3261/3287 (99.209005%); IL 263611/269894 bytes (97.672049%) | 3272/3287 (99.543657%); IL 268111/269894 bytes (99.339370%) | 0 |
| Assembly-CSharp | 1892/1958 (96.629213%); IL 549322/601603 bytes (91.309718%) | 1898/1958 (96.935649%); IL 561744/601603 bytes (93.374534%) | 0 |

Matched exige cabeçalho, locais, IL e EH integrais; avanço de prefixo não é método concluído.

2. O QUE FOI FEITO NESTA RODADA

| Frente/ciclo | Assembly/par | Tentativa e resultado | Métodos ganhos | IL ganho | Perdas |
| --- | --- | --- | --- | --- | --- |
| A | evidência | Caminho PDB original emitido pelo compilador; 97 diferencas de caminho/padding para zero; CodeView ex-GUID igual; nenhum corpo ou estado alterado | 0 | 0 | 0 |
| B | evidência | Contrato final documentado; DEFLATE nivel6 reproduz streams originais exatos das duas DLLs; nenhum APK montado ou executado | 0 | 0 | 0 |
| c01 / C1 | KairoLibrary / leave=>br | Restore three protected returns in the Play lambda while retaining payloads and lexical scopes; INCONCLUSIVA: nenhum candidato autorizado pelo modelo/mapeamento; 0 tentativas da regra, 0 avancos | 0 | 0 | 0 |
| c02 / C1 | KairoLibrary / leave=>br | After GUID-bound private PDB mirroring, test the three protected returns in the Play lambda; first pass had no mapped trial; Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 383 | 0 |
| c03 / C2 | KairoLibrary / INSERTION_POINT=>ldc.i4.0 | Copy colors once to a named array local and restore ordered selector1/2 if arms without changing color comparisons; Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 467 | 0 |
| c04 / C3 | KairoLibrary / brfalse.s=>brtrue | Restore three cancel returns protected by try and log-before-clock local declarations without moving evaluations; Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 537 | 0 |
| c05 / C4 | Assembly-CSharp / newarr=>stloc; switch=>named-selector | Restore array stores and reuse the existing named selector before ordered tests, preserving calls and local allocation; Avanco parcial; nenhum novo matched; 1 tentativas da regra, 1 avancos | 0 | 0 | 0 |
| c06 / C5 | Assembly-CSharp / neg=>ldc.i4.0; primitive-comparison operand order | Recover eight original Int32 zero-subtractions in Map and test symbol-proven mirrored primitive comparisons across all pending smcs bodies; Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 2711 | 0 |
| c07 / C5 | Assembly-CSharp / primitive-comparison operand order including literal0 | Evaluate pure mirrored numeric comparisons including safe converted-type literals and retained repertoire; preserve explicit negation and all local-signature gates; Nenhum candidato compilado avancou; 7 tentativas da regra, 0 avancos | 0 | 0 | 0 |
| c08 / C5 | KairoLibrary / primitive-comparison operand order including literal0 | Evaluate pure mirrored primitive numeric operands and safe literals in all pending Roslyn bodies, preserving outer negation and complete-signature vetoes; Nenhum candidato compilado avancou; 5 tentativas da regra, 0 avancos | 0 | 0 | 0 |
| c09 / C6 | KairoLibrary / missing-ldc.i4.1-conditional-branch | Restore a nonconstant true local whose branch survives Roslyn emission while optimizer schedules away the named local; do not promote incomplete model prediction; Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 661 | 0 |
| cycle01 / D | Assembly-CSharp / local-slot layout and concat spill | Calculate named local scopes, restore compound concat spills, and remove extra selector temporaries using the new corpus-validated smcs model; Novos corpos aprovados; 5 tentativas da regra, 5 avancos | 4 | 5867 | 0 |
| cycle02 / D | KairoLibrary / brtrue.s retarget due missing null-array return | Restore the local null-array return inside its owning guard and retain the former else as an anonymous block without changing twelve existing locals; Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 680 | 0 |
| cycle03 / D | Assembly-CSharp / named selector at slot32 plus twenty control regions | Restore the exact typed68-slot order with named GetMatchInfoJoin selector and compose the original empty arms and loop-owned joins; old count-only equality claim is withdrawn; Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 3844 | 0 |
| cycle04 / D | KairoLibrary / bne.un.s=>beq372; stloc.1=>stloc.3 310 | Recover per-arm string stores and parent-owned format/cursor locals with interleaved original control; fresh top two aligned Kairo pairs weigh372 and310 bytes; Novos corpos aprovados; 2 tentativas da regra, 2 avancos | 2 | 682 | 0 |
| cycle05 / D | KairoLibrary / brtrue.s=>brfalse.s303; brfalse.s=>brtrue.s297 | Restore stores per Lobi branch and compose positive editor body with empty application test plus branch-owned and common null returns; discarded comparison relation remains UNKNOWN; Novos corpos aprovados; 2 tentativas da regra, 2 avancos | 2 | 600 | 0 |
| cycle06 / D | KairoLibrary / ldloc.1=>ldelema275; missing-string-alias215 | Restore explicit indexed integer-mask assignment and input-string alias before protected table loops while preserving the argument fallback; diagnostic rule aliases retain original rule identities; Novos corpos aprovados; 2 tentativas da regra, 2 avancos | 2 | 490 | 0 |

C1: extend-try-SoundOGG-Play; avaliações: c01, c02. Verificada descreve a frente avaliada; a hipótese pode continuar INCONCLUSIVA.

C2: parameter-alias-ShaderBuilder-Init; avaliações: c03. Verificada descreve a frente avaliada; a hipótese pode continuar INCONCLUSIVA.

C3: try-return-guard-SaveRecorder-AutoSaveRun; avaliações: c04. Verificada descreve a frente avaliada; a hipótese pode continuar INCONCLUSIVA.

C4: split-chain-UserData-SetTrainingCommonMethod; avaliações: c05. Verificada descreve a frente avaliada; a hipótese pode continuar INCONCLUSIVA.

C5: mirror-comparison-operands-both-compilers; avaliações: c06, c07, c08. Verificada descreve a frente avaliada; a hipótese pode continuar INCONCLUSIVA.

C6: constant-true-lowering-Seb.Draw; avaliações: c09. Verificada descreve a frente avaliada; a hipótese pode continuar INCONCLUSIVA.

3. HIPÓTESES

A — caminho CodeView/PDB: diferenças de caminho e padding 97 → 0; caminho passado ao compilador: `E:\workspace\Library\KairoLibrary\obj\Android\KairoLibrary.pdb`; edição posterior de DLL: False.

B — KairoLibrary: níveis DEFLATE com stream integral idêntico: [6]; stream original 248092 bytes; zlib 1.3.1; parâmetros {"method": "DEFLATED", "wbits": -15, "memLevel": 8, "strategy": "Z_DEFAULT_STRATEGY", "flush": "Z_FINISH", "input": "entire original DLL in one compress call"}.

B — Assembly-CSharp: níveis DEFLATE com stream integral idêntico: [6]; stream original 321171 bytes; zlib 1.3.1; parâmetros {"method": "DEFLATED", "wbits": -15, "memLevel": 8, "strategy": "Z_DEFAULT_STRATEGY", "flush": "Z_FINISH", "input": "entire original DLL in one compress call"}.

APK montado: False; alvo executado nesta calibração: False.

Critério final do APK: payload comprimido idêntico para todas as entradas salvo as duas DLLs e os portadores exatos da assinatura; preservar ordem, método e alinhamento. Comparar as duas DLLs descomprimidas e integralmente, permitindo somente os seis campos não determinísticos comprovados. Exceções ZIP originais de assinatura: META-INF/MANIFEST.MF, META-INF/CERT.SF e META-INF/CERT.RSA; eventual APK Signing Block tratado separadamente. Não copiar nem forjar a assinatura original. Encoder histórico exato: UNKNOWN.

C6: Roslyn revisão 4cb203dab89e1542cdf7d88cb6d32767bc0e5460, src/Compilers/CSharp/Portable/Lowering/LocalRewriter/LocalRewriter_IfStatement.cs:40–63 emite BoundConditionalGoto(rewrittenCondition.Syntax, rewrittenCondition, false, afterif); CodeGen/EmitStatement.cs:364–385 só dobra quando condition.ConstantValue != null. Um local Boolean não const de uso único conserva o branch e pode ser agendado na pilha; if(true) e const bool foram refutados nas fixtures. Seb.Draw fechou com esse local, no c09.

INCONCLUSIVA — A família r11-c1 produz ao menos um novo corpo integral. Evidência: c01: 0 matched, 0 IL bytes; INCONCLUSIVA: nenhum candidato autorizado pelo modelo/mapeamento; 0 tentativas da regra, 0 avancos. Proposta investigada: Restore three protected returns in the Play lambda while retaining payloads and lexical scopes

CONFIRMADA — A família r11-c1 produz ao menos um novo corpo integral. Evidência: c02: 1 matched, 383 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos. Proposta investigada: After GUID-bound private PDB mirroring, test the three protected returns in the Play lambda; first pass had no mapped trial

CONFIRMADA — A família r11-c2 produz ao menos um novo corpo integral. Evidência: c03: 1 matched, 467 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos. Proposta investigada: Copy colors once to a named array local and restore ordered selector1/2 if arms without changing color comparisons

CONFIRMADA — A família r11-c3 produz ao menos um novo corpo integral. Evidência: c04: 1 matched, 537 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos. Proposta investigada: Restore three cancel returns protected by try and log-before-clock local declarations without moving evaluations

INCONCLUSIVA — A família r11-c4 produz ao menos um novo corpo integral. Evidência: c05: 0 matched, 0 IL bytes; Avanco parcial; nenhum novo matched; 1 tentativas da regra, 1 avancos. Proposta investigada: Restore array stores and reuse the existing named selector before ordered tests, preserving calls and local allocation

INCONCLUSIVA — O candidato parcial de c05 reproduz integralmente game.UserData::SetTrainingCommonMethod(System.Boolean)->System.Void. Evidência: 2 avanços aceitos, mas corpo integral não igual no fim desse ciclo; parada no_transformation_advances. Ganhos de outros métodos da família não confirmam este candidato.

CONFIRMADA — A família r11-c5 produz ao menos um novo corpo integral. Evidência: c06: 1 matched, 2711 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos. Proposta investigada: Recover eight original Int32 zero-subtractions in Map and test symbol-proven mirrored primitive comparisons across all pending smcs bodies

INCONCLUSIVA — A família r11-c5 produz ao menos um novo corpo integral. Evidência: c07: 0 matched, 0 IL bytes; Nenhum candidato compilado avancou; 7 tentativas da regra, 0 avancos. Proposta investigada: Evaluate pure mirrored numeric comparisons including safe converted-type literals and retained repertoire; preserve explicit negation and all local-signature gates

INCONCLUSIVA — A família r11-c5 produz ao menos um novo corpo integral. Evidência: c08: 0 matched, 0 IL bytes; Nenhum candidato compilado avancou; 5 tentativas da regra, 0 avancos. Proposta investigada: Evaluate pure mirrored primitive numeric operands and safe literals in all pending Roslyn bodies, preserving outer negation and complete-signature vetoes

CONFIRMADA — A família r11-c6 produz ao menos um novo corpo integral. Evidência: c09: 1 matched, 661 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos. Proposta investigada: Restore a nonconstant true local whose branch survives Roslyn emission while optimizer schedules away the named local; do not promote incomplete model prediction

CONFIRMADA — A família r11-initial-slot-layout-and-concat-alias-composition produz ao menos um novo corpo integral. Evidência: cycle01: 4 matched, 5867 IL bytes; Novos corpos aprovados; 5 tentativas da regra, 5 avancos. Proposta investigada: Calculate named local scopes, restore compound concat spills, and remove extra selector temporaries using the new corpus-validated smcs model

INCONCLUSIVA — O candidato parcial de cycle01 reproduz integralmente main.AppData::SendSaveData(System.String,System.Int32,System.Exception)->System.Void. Evidência: 1 avanços aceitos, mas corpo integral não igual no fim desse ciclo; parada incomplete_candidate_budget. Ganhos de outros métodos da família não confirmam este candidato.

CONFIRMADA — A família r11-d-initial-image-use-local-null-return produz ao menos um novo corpo integral. Evidência: cycle02: 1 matched, 680 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos. Proposta investigada: Restore the local null-array return inside its owning guard and retain the former else as an anonymous block without changing twelve existing locals

CONFIRMADA — A família r11-initial-init2-control-composition produz ao menos um novo corpo integral. Evidência: cycle03: 1 matched, 3844 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos. Proposta investigada: Restore the exact typed68-slot order with named GetMatchInfoJoin selector and compose the original empty arms and loop-owned joins; old count-only equality claim is withdrawn

CONFIRMADA — A família r11-kairo-parent-assignment-and-interleaved-control-composition produz ao menos um novo corpo integral. Evidência: cycle04: 2 matched, 682 IL bytes; Novos corpos aprovados; 2 tentativas da regra, 2 avancos. Proposta investigada: Recover per-arm string stores and parent-owned format/cursor locals with interleaved original control; fresh top two aligned Kairo pairs weigh372 and310 bytes

CONFIRMADA — A família r11-kairo-ui produz ao menos um novo corpo integral. Evidência: cycle05: 2 matched, 600 IL bytes; Novos corpos aprovados; 2 tentativas da regra, 2 avancos. Proposta investigada: Restore stores per Lobi branch and compose positive editor body with empty application test plus branch-owned and common null returns; discarded comparison relation remains UNKNOWN

CONFIRMADA — A família r11-kairo-mid- produz ao menos um novo corpo integral. Evidência: cycle06: 2 matched, 490 IL bytes; Novos corpos aprovados; 2 tentativas da regra, 2 avancos. Proposta investigada: Restore explicit indexed integer-mask assignment and input-string alias before protected table loops while preserving the argument fallback; diagnostic rule aliases retain original rule identities

CONFIRMADA — Reproduzir o caminho histórico do PDB elimina as diferenças de caminho sem reescrever a DLL. Evidência: Parte A: 97 diferenças de caminho/padding caíram para zero; corpos e metadados preservados.

CONFIRMADA — Deflate nível 6 com os parâmetros registrados reproduz os dois streams originais. Evidência: Parte B: Kairo 248092 bytes e App 321171 bytes, igualdade integral; zlib 1.3.1. Encoder histórico UNKNOWN.

REFUTADA — Literal if(true) ou const bool=true produz o branch de Seb.Draw. Evidência: As duas fixtures dobraram o teste; local Boolean não const conservou ldc.i4.1/brfalse e fechou Seb.Draw em c09.

INCONCLUSIVA — Espelhar operandos resolve os pendentes desta rodada. Evidência: 7 trials smcs e 5 Roslyn: zero avanços. Isso não refuta a forma para toda construção.

REFUTADA — A diferença de Map.SetCoatLine é comparação espelhada. Evidência: C06: oito subtrações Int32 neg versus 0-x; a restauração fechou os 2711 bytes IL.

CONFIRMADA — O modelo Roslyn8 omite um temporário de getter de struct por valor em MeshManager.GetMeshBuilder. Evidência: Baseline prevê 4 locais contra 5 reais. Extensão privada prevê 5 no baseline/4 no candidato; não foi selecionada.

INCONCLUSIVA — As quatro propostas privadas restantes reproduzem os corpos reais. Evidência: Material 110 + Mesh 141 + Pause 199 + Transpose 197 = 647 bytes IL. Fixtures próprias passaram; zero compilações do jogo.

INCONCLUSIVA — O operador histórico do teste vazio de SaveToTmpCaptureDirectory pode ser identificado pelo IL. Evidência: Fixtures ==, != e > geraram o mesmo corpo. D05 reproduziu 297 bytes IL; relação exata UNKNOWN.

INCONCLUSIVA — `game.UserData::SetTrainingCommonMethod(System.Boolean)->System.Void`: 2 avanços parciais em round11-c05-Assembly-CSharp-engine.json; prefixo 802 → 1334; corpo integral ainda diferente.

INCONCLUSIVA — `game.UserData::SetTrainingCommonMethod(System.Boolean)->System.Void`: 2 avanços parciais em round11-c05-probe-Assembly-CSharp-engine.json; prefixo 802 → 1334; corpo integral ainda diferente.

INCONCLUSIVA — `main.AppData::SendSaveData(System.String,System.Int32,System.Exception)->System.Void`: 1 avanços parciais em round11-cycle01-Assembly-CSharp-engine.json; prefixo 611 → 986; corpo integral ainda diferente.

| Modelo | Corpos matched | Mapeados | Sem mapeamento | Previsões completas | Completas exatas | Precisão | Cobertura | Incompletas |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| KairoLibrary | 3196 | 2858 | 338 | 2250 | 2250 | 100.000000% | 70.400501% | 608 |
| Assembly-CSharp | 1892 | 1810 | 82 | 1809 | 1809 | 100.000000% | 95.613108% | 1 |

Extrator smcs: medição somente de leitura no corpus final; nenhuma árvore incompleta foi tratada como recuperação completa.

| Corpus | Métodos | Duas árvores completas | Árvores iguais | Cobertura | Igualdade entre completas |
| --- | --- | --- | --- | --- | --- |
| matched | 1892 | 1316 | 1316 | 69.556025% | 100.000000% |
| pendentes | 60 | 13 | 8 | 21.666667% | 61.538462% |

4. PENDENTES POR GRUPO

KairoLibrary: 13 corpos divergentes / 1765 bytes de IL.

| Faixa bytes | Métodos | IL bytes |
| --- | --- | --- |
| <=64 | 1 | 46 |
| 65-256 | 12 | 1719 |
| 257-1024 | 0 | 0 |
| >1024 | 0 | 0 |

| Primeiro par alinhado | Métodos | IL bytes |
| --- | --- | --- |
| INSERTION_POINT => stloc.2 | 2 | 251 |
| stloc.0 => dup | 1 | 199 |
| INSERTION_POINT => dup | 1 | 197 |
| ldelem.any => ldelema | 1 | 192 |
| bne.un.s => beq.s | 1 | 182 |
| leave.s => br.s | 1 | 164 |
| stloc.0 => stloc.1 | 1 | 140 |
| constrained. => call | 1 | 128 |
| INSERTION_POINT => stloc.1 | 1 | 105 |
| stloc.0 => stloc.3 | 1 | 94 |
| ldloc.0 => DELETION_POINT | 1 | 67 |
| stloc.2 => stloc.0 | 1 | 46 |

`INSERTION_POINT => stloc.2`; exemplo `kairo.unity.graphics.MaterialManager::GetMaterial(kairo.unity.graphics.Material,kairo.unity.ui.Image)->kairo.unity.graphics.Material`.

| IL original | IL recompilado |
| --- | --- |
| ```IL_0023: newobj class System.Collections.Generic.Dictionary`2<class System.String,class kairo.unity.graphics.Material>::.ctor``0()->class System.Void;this=True;explicit=False;cc=Default``` | ```IL_0023: newobj class System.Collections.Generic.Dictionary`2<class System.String,class kairo.unity.graphics.Material>::.ctor``0()->class System.Void;this=True;explicit=False;cc=Default``` |
| `IL_0028: dup` | `IL_0028: dup` |
| `IL_0029: stloc.1` | `IL_0029: stloc.2` |
| ```IL_002a: callvirt class System.Collections.Generic.Dictionary`2<class kairo.unity.ui.Image,class System.Collections.Generic.Dictionary`2<class System.String,class kairo.unity.graphics.Material>>::set_Item``0(!0,!1)->class System.Void;this=True;explicit=False;cc=Default``` | ```IL_002a: callvirt class System.Collections.Generic.Dictionary`2<class kairo.unity.ui.Image,class System.Collections.Generic.Dictionary`2<class System.String,class kairo.unity.graphics.Material>>::set_Item``0(!0,!1)->class System.Void;this=True;explicit=False;cc=Default``` |
| `IL_002f: ldloc.1` | `IL_002f: ldloc.2` |
|  | `IL_0030: stloc.1` |
|  | `IL_0031: ldloc.1` |

Tentativas compiladas nesta rodada: nenhum; recusas anteriores à compilação: nenhum; orçamento recusou 0. Histórico 7–10 compatível por identidade original e hash do fonte: UNKNOWN.

`stloc.0 => dup`; exemplo `kairo.unity.ui.IApplication::OnApplicationPause(System.Boolean)->System.Void`.

| IL original | IL recompilado |
| --- | --- |
| `IL_0008: brfalse.s L4` | `IL_0008: brfalse.s L4` |
| ```IL_000a: call class kairo.unity.system.notification.ui.NotifyManager::GetInstance``0()->class kairo.unity.system.notification.ui.NotifyManager;this=False;explicit=False;cc=Default``` | ```IL_000a: call class kairo.unity.system.notification.ui.NotifyManager::GetInstance``0()->class kairo.unity.system.notification.ui.NotifyManager;this=False;explicit=False;cc=Default``` |
| `IL_000f: stloc.0` | `IL_000f: dup` |
| `IL_0010: ldloc.0` | `IL_0010: brtrue.s R21` |
| `IL_0011: brfalse.s L0` | `IL_0012: pop` |
| `IL_0013: ldloc.0` | `IL_0013: br.s L0` |
| ```IL_0014: callvirt class kairo.unity.system.notification.ui.NotifyManager::DoNotify``0()->class System.Void;this=True;explicit=False;cc=Default``` | ```IL_0015: call class kairo.unity.system.notification.ui.NotifyManager::DoNotify``0()->class System.Void;this=True;explicit=False;cc=Default``` |
| `IL_0019: ldc.i4.1` | `IL_001a: ldc.i4.1` |
| ```IL_001a: call class kairo.unity.native.KairoPlugin::SetNotificationBackground``0(valuetype System.Boolean)->class System.Void;this=False;explicit=False;cc=Default``` | ```IL_001b: call class kairo.unity.native.KairoPlugin::SetNotificationBackground``0(valuetype System.Boolean)->class System.Void;this=False;explicit=False;cc=Default``` |

Tentativas compiladas nesta rodada: nenhum; recusas anteriores à compilação: nenhum; orçamento recusou 0. Histórico 7–10 compatível por identidade original e hash do fonte: UNKNOWN.

`INSERTION_POINT => dup`; exemplo `kairo.unity.m3d4.util3d.Transform::Transpose()->System.Void`.

| IL original | IL recompilado |
| --- | --- |
| `IL_00b6: ldelem.r4` | `IL_00b6: ldelem.r4` |
| `IL_00b7: stelem.r4` | `IL_00b7: stelem.r4` |
| `IL_00b8: ldc.i4.s 15` | `IL_00b8: dup` |
| `IL_00ba: ldarg.0` | `IL_00b9: ldc.i4.s 15` |
| `IL_00bb: ldfld class kairo.unity.m3d4.util3d.Transform::mtx_:valuetype System.Single[]` | `IL_00bb: ldarg.0` |

Tentativas compiladas nesta rodada: nenhum; recusas anteriores à compilação: nenhum; orçamento recusou 0. Histórico 7–10 compatível por identidade original e hash do fonte: UNKNOWN.

`ldelem.any => ldelema`; exemplo `kairo.unity.system.lineup.ui.LineupData::GetRatio()->System.Int32`.

| IL original | IL recompilado |
| --- | --- |
| `IL_0045: ldloc.2` | `IL_0045: ldloc.2` |
| `IL_0046: ldloc.3` | `IL_0046: ldloc.3` |
| `IL_0047: ldelem.any valuetype kairo.unity.system.appli.GameAppli` | `IL_0047: ldelema valuetype kairo.unity.system.appli.GameAppli` |
| `IL_004c: ldfld valuetype kairo.unity.system.appli.GameAppli::Pid:valuetype System.Int32` | `IL_004c: ldfld valuetype kairo.unity.system.appli.GameAppli::Pid:valuetype System.Int32` |
| `IL_0051: ldarg.0` | `IL_0051: ldarg.0` |

Tentativas compiladas nesta rodada: nenhum; recusas anteriores à compilação: nenhum; orçamento recusou 0. Histórico 7–10 compatível por identidade original e hash do fonte: UNKNOWN.

`bne.un.s => beq.s`; exemplo `kairo.unity.system.KairoBase/<>c::<Start>b__55_0()->System.Object`.

| IL original | IL recompilado |
| --- | --- |
| ```IL_008d: callvirt class kairo.unity.ui.Dialog::Show``0()->valuetype System.Int32;this=True;explicit=False;cc=Default``` | ```IL_008d: callvirt class kairo.unity.ui.Dialog::Show``0()->valuetype System.Int32;this=True;explicit=False;cc=Default``` |
| `IL_0092: ldc.i4.4` | `IL_0092: ldc.i4.4` |
| `IL_0093: bne.un.s O158` | `IL_0093: beq.s L9` |
| `IL_0095: ldloc.0` | `IL_0095: ldc.i4.0` |
| `IL_0096: ldc.i4.1` | `IL_0096: box valuetype System.Boolean` |
|  | `IL_009b: stloc.s 5` |
|  | `IL_009d: leave.s L10` |
|  | `IL_009f: ldloc.0` |
|  | `IL_00a0: ldc.i4.1` |

Tentativas compiladas nesta rodada: r11-c5-mirror-primitive-operands: 1; recusas anteriores à compilação: nenhum; orçamento recusou 0. Histórico 7–10 compatível por identidade original e hash do fonte: UNKNOWN.

Pendentes com assinatura de locais igual: 5; diferente: 8.

Assembly-CSharp: 59 corpos divergentes / 39852 bytes de IL.

| Faixa bytes | Métodos | IL bytes |
| --- | --- | --- |
| <=64 | 5 | 237 |
| 65-256 | 15 | 2357 |
| 257-1024 | 26 | 13131 |
| >1024 | 13 | 24127 |

| Primeiro par alinhado | Métodos | IL bytes |
| --- | --- | --- |
| INSERTION_POINT => ldloc.s | 2 | 3940 |
| stloc.3 => stloc.1 | 2 | 2955 |
| INSERTION_POINT => stloc.s | 2 | 2565 |
| ret => br | 1 | 2454 |
| bne.un => beq | 1 | 2207 |
| brtrue => brtrue | 1 | 2145 |
| stloc.s => stloc.2 | 2 | 1874 |
| stloc.s => stloc.0 | 2 | 1800 |
| dup => DELETION_POINT | 2 | 1715 |
| ldc.i4.5 => DELETION_POINT | 1 | 1599 |
| brfalse => DELETION_POINT | 1 | 1342 |
| bne.un => bne.un | 1 | 1308 |
| stloc.2 => stloc.s | 3 | 1263 |
| stloc.s => stloc.1 | 1 | 1029 |
| br => DELETION_POINT | 6 | 878 |
| INSERTION_POINT => ldc.r4 | 1 | 857 |
| stloc.3 => dup | 1 | 758 |
| brtrue => DELETION_POINT | 1 | 723 |
| INSERTION_POINT => ldc.i4.s | 1 | 707 |
| INSERTION_POINT => ldloc.2 | 1 | 691 |
| ldc.i4.0 => stloc.s | 1 | 623 |
| blt.un => bge | 1 | 594 |
| bge.un => blt | 1 | 562 |
| ldc.i4.1 => switch | 2 | 556 |
| ldarg.s => DELETION_POINT | 1 | 506 |
| brtrue => stloc.1 | 1 | 447 |
| ldarg.1 => ldloc.s | 1 | 432 |
| ldarg.2 => DELETION_POINT | 1 | 343 |
| INSERTION_POINT => stloc.3 | 1 | 325 |
| switch => ldc.i4.1 | 1 | 322 |
| ldc.i4.0 => DELETION_POINT | 1 | 286 |
| ldc.i4.1 => ldc.i4.4 | 1 | 272 |
| brtrue => brfalse | 2 | 266 |
| blt => blt | 1 | 253 |
| INSERTION_POINT => ldarg.0 | 1 | 241 |
| INSERTION_POINT => stloc.0 | 1 | 196 |
| beq => bne.un | 1 | 190 |
| brfalse => brfalse | 1 | 166 |
| ldarg.3 => DELETION_POINT | 1 | 110 |
| stloc.0 => DELETION_POINT | 2 | 109 |
| INSERTION_POINT => ldloc.1 | 1 | 106 |
| stloc.0 => stloc.s | 1 | 99 |
| brtrue => stloc.0 | 1 | 38 |

`INSERTION_POINT => ldloc.s`; exemplo `game.UserData::SetTrainingCommonMethod(System.Boolean)->System.Void`.

| IL original | IL recompilado |
| --- | --- |
| `IL_05f4: stloc.s 32` | `IL_05f4: stloc.s 32` |
| `IL_05f6: br L65` | `IL_05f6: br L65` |
| `IL_05fb: ldarg.0` | `IL_05fb: ldloc.s 14` |
| `IL_05fc: ldfld class game.UserData::tModeList_:valuetype System.Int32[][]` | `IL_05fd: ldloc.s 27` |
| `IL_0601: ldloc.s 27` | `IL_05ff: ldelem.ref` |
|  | ```IL_0600: callvirt class java.util.Vector`1<class System.Object>::Size``0()->valuetype System.Int32;this=True;explicit=False;cc=Default``` |
|  | `IL_0605: ldc.i4.0` |
|  | `IL_0606: bgt L59` |
|  | `IL_060b: br L66` |
|  | `IL_0610: ldarg.0` |
|  | `IL_0611: ldfld class game.UserData::tModeList_:valuetype System.Int32[][]` |

Tentativas compiladas nesta rodada: local-declaration-scope-hoist: 2, r11-c4-array-population-existing-selector-composition: 2, smcs-empty-arm-restore: 4, solver-group-all-calculated-blocks: 4, solver-group-block-declarations: 16; recusas anteriores à compilação: wrong_signature: 40; orçamento recusou 0. Histórico 7–10 compatível por identidade original e hash do fonte: UNKNOWN.

`stloc.3 => stloc.1`; exemplo `form.GameForm::_draw(kairo.unity.ui.Graphics)->System.Void`.

| IL original | IL recompilado |
| --- | --- |
| `IL_0008: ldc.i4.0` | `IL_0008: ldc.i4.0` |
| `IL_0009: dup` | `IL_0009: dup` |
| `IL_000a: stloc.3` | `IL_000a: stloc.1` |
| `IL_000b: dup` | `IL_000b: dup` |
| `IL_000c: stloc.2` | `IL_000c: stloc.0` |

Tentativas compiladas nesta rodada: nenhum; recusas anteriores à compilação: nenhum; orçamento recusou 0. Histórico 7–10 compatível por identidade original e hash do fonte: UNKNOWN.

`INSERTION_POINT => stloc.s`; exemplo `form.SubForm::Draw_battlePic(kairo.unity.ui.Graphics,System.Int32)->System.Void`.

| IL original | IL recompilado |
| --- | --- |
| `IL_0062: ldc.i4.0` | `IL_0062: ldc.i4.0` |
| `IL_0063: dup` | `IL_0063: dup` |
| `IL_0064: stloc.s 5` | `IL_0064: stloc.s 7` |
| `IL_0066: dup` | `IL_0066: dup` |
| `IL_0067: stloc.s 9` | `IL_0067: stloc.s 5` |
|  | `IL_0069: dup` |

Tentativas compiladas nesta rodada: nenhum; recusas anteriores à compilação: nenhum; orçamento recusou 0. Histórico 7–10 compatível por identidade original e hash do fonte: UNKNOWN.

`ret => br`; exemplo `game.Camera::Update_pos()->System.Void`.

| IL original | IL recompilado |
| --- | --- |
| `IL_0310: conv.r4` | `IL_0310: conv.r4` |
| `IL_0311: stfld class kairo.unity.m3d4.util3d.Vector3D::y_:valuetype System.Single` | `IL_0311: stfld class kairo.unity.m3d4.util3d.Vector3D::y_:valuetype System.Single` |
| `IL_0316: ret` | `IL_0316: br L28` |
| `IL_0317: ldarg.0` | `IL_031b: ldarg.0` |
| `IL_0318: ldfld class game.Camera::mode_:valuetype System.Int32` | `IL_031c: ldfld class game.Camera::mode_:valuetype System.Int32` |

Tentativas compiladas nesta rodada: nenhum; recusas anteriores à compilação: nenhum; orçamento recusou 0. Histórico 7–10 compatível por identidade original e hash do fonte: UNKNOWN.

`bne.un => beq`; exemplo `main.AppData::SendSaveData(System.String,System.Int32,System.Exception)->System.Void`.

| IL original | IL recompilado |
| --- | --- |
| `IL_03df: ldfld class main.AppData::sendState_:valuetype System.Int32` | `IL_03df: ldfld class main.AppData::sendState_:valuetype System.Int32` |
| `IL_03e4: ldc.i4.1` | `IL_03e4: ldc.i4.1` |
| `IL_03e5: bne.un L46` | `IL_03e5: beq L19` |
| `IL_03ea: ldarg.0` | `IL_03ea: br L46` |
| `IL_03eb: ldloc.s 10` | `IL_03ef: ldarg.0` |
|  | `IL_03f0: ldloc.s 10` |

Tentativas compiladas nesta rodada: condition-local: 1, local-declaration-scope-hoist: 9, r11-initial-slot-layout-and-concat-alias-composition: 1, smcs-empty-arm-restore: 1; recusas anteriores à compilação: nenhum; orçamento recusou 11. Histórico 7–10 compatível por identidade original e hash do fonte: UNKNOWN.

Pendentes com assinatura de locais igual: 31; diferente: 28.

5. TOOLCHAIN

KairoLibrary: roslyn-csc; versão 1.1.0.51109; SHA-256 `53b693536e8a86f35d05d2165ff9b3e0aeaa499ecfd8135a9f99ab4404ca86c3`; flags efetivas `/nologo /noconfig /nostdlib+ /target:library /platform:anycpu /optimize+ /debug:pdbonly /deterministic- /langversion:6 /unsafe- /checked- /codepage:65001 /utf8output /pdb:E:\workspace\Library\KairoLibrary\obj\Android\KairoLibrary.pdb`.

Referências efetivas, na ordem: `[{"path": "C:\\Windows\\Microsoft.NET\\Framework\\v2.0.50727\\mscorlib.dll", "sha256": "c4729c6fc2279933fa9638f60775ab2f7eb731d5b04f9e04c15187afbdf94558"}, {"path": "C:\\Users\\l_abn\\AppData\\Local\\Programs\\Unity\\5.3.4f1\\Editor\\Data\\Mono\\lib\\mono\\2.0\\System.Core.dll", "sha256": "4998eda31c3d7bebf606f98cef77ad697f9e82e629efc37c06517019c2c2c271"}, {"path": "C:\\Users\\l_abn\\Documents\\Projetos\\Pocket League Story\\original\\extracted\\assets\\bin\\Data\\Managed\\UnityEngine.dll", "sha256": "7cc901371cc77e103d00b5845e8285f85fbdf36bb47ccf3d824f94c4ead32421"}, {"path": "C:\\Users\\l_abn\\AppData\\Local\\Programs\\Unity\\5.3.4f1\\Editor\\Data\\Mono\\lib\\mono\\2.0\\System.dll", "sha256": "c2225d7ba58a12e22c266384d3c1e4e882ab4e42449b91b721c21b5f1fcd63be"}]`. Compilador/opções alterados desde baseline: True; compilador histórico exato: UNKNOWN.

Assembly-CSharp: mono-smcs; versão 2.6.5.0; SHA-256 `26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0`; flags efetivas `-nologo -noconfig -target:library -platform:anycpu -optimize+ -debug- -langversion:Default -unsafe- -checked- -codepage:65001`.

Referências efetivas, na ordem: `[{"path": "C:\\Users\\l_abn\\Documents\\Projetos\\Pocket League Story\\original\\extracted\\assets\\bin\\Data\\Managed\\mscorlib.dll", "sha256": "7f16171cf5f87ca1e4c229727526d2fbc786079fc0c960f36197013ea0d986e1"}, {"path": "C:\\Users\\l_abn\\Documents\\Projetos\\Pocket League Story\\original\\extracted\\assets\\bin\\Data\\Managed\\KairoLibrary.dll", "sha256": "bb553d341eecd9b900f772486bf4a5c289aa2f09566d9d402a3eb70268f4b3ee"}, {"path": "C:\\Users\\l_abn\\Documents\\Projetos\\Pocket League Story\\original\\extracted\\assets\\bin\\Data\\Managed\\UnityEngine.dll", "sha256": "7cc901371cc77e103d00b5845e8285f85fbdf36bb47ccf3d824f94c4ead32421"}, {"path": "C:\\Users\\l_abn\\Documents\\Projetos\\Pocket League Story\\original\\extracted\\assets\\bin\\Data\\Managed\\System.Core.dll", "sha256": "104278a0bf67b4de8fd1e910e3292a3ef51808ebee7a5f2a229125f18463c98c"}, {"path": "C:\\Users\\l_abn\\Documents\\Projetos\\Pocket League Story\\original\\extracted\\assets\\bin\\Data\\Managed\\System.dll", "sha256": "7e72b92ddcd6b7b791ab24dd5756544f2da81f65e7ba7d1ebe8223123720d0ac"}]`. Compilador/opções alterados desde baseline: False; compilador histórico exato: UNKNOWN.

A configuração do caminho PDB da Parte A é produção pelo compilador; não é normalização nem edição da DLL.

6. COMPONENTES EXTERNOS E METADADOS

KairoLibrary: tipos/metadados estruturais 337/337; igualdade estrutural global True.

Assembly-CSharp: tipos/metadados estruturais 129/129; igualdade estrutural global True.

| Assembly | Campo não determinístico | Prova |
| --- | --- | --- |
| KairoLibrary | Module.Mvid | reports/round3-Kairo-repeatability.json |
| KairoLibrary | COFF.TimeDateStamp | reports/round3-Kairo-repeatability.json |
| KairoLibrary | IMAGE_DEBUG_DIRECTORY.TimeDateStamp | reports/round3-Kairo-repeatability.json |
| KairoLibrary | CodeView.RSDS.PdbGuid | reports/round3-Kairo-repeatability.json |
| Assembly-CSharp | Module.Mvid | reports/round3-Assembly-CSharp-repeatability.json |
| Assembly-CSharp | COFF.TimeDateStamp | reports/round3-Assembly-CSharp-repeatability.json |

Essas exceções são somente contabilidade de diferenças: nenhum byte é normalizado. Externos não foram decompilados nesta rodada; seu inventário/provas anteriores permanecem em docs/toolchain.md. DEX/Java permanece fora do escopo.

Provas externas preservadas: sete DLLs gerenciadas e seis bibliotecas nativas coincidem com a distribuição oficial Unity 5.3.4f1. UnityEngine.dll corresponde à variante AndroidPlayer/Variations/mono/Managed, SHA-256 7cc901371cc77e103d00b5845e8285f85fbdf36bb47ccf3d824f94c4ead32421; libunity ARMv7/x86 corresponde a Mono Release. Não houve nova auditoria ou decompilação de externos nesta rodada.

Comparação bruta completa não repetida nesta rodada: KairoLibrary ainda tem corpos divergentes e Assembly-CSharp tem mais de dez. Além do caminho PDB medido na Parte A, as diferenças remanescentes de tabelas, heaps, seções e suas causas são UNKNOWN neste checkpoint; igualdade estrutural não prova igualdade bruta.

7. BLOQUEIOS

Motivo da parada: Solicitação explícita do usuário: me envie o relatorio final; parada após o ciclo D06. Ciclos D: 6/40; ciclos C: 9.

Fila inicial explícita precedeu a descoberta autônoma. Após ela, os ciclos D04–D06 priorizaram Kairo, que ainda não fechou seus corpos. A/B: zero ganhos de métodos e IL. Orçamento de previsões incompletas: 12 variantes por método/passe e 24/ciclo; previsões completas erradas foram recusadas.

Parada solicitada pelo usuário após D06, com zero ciclos D consecutivos sem ganho. O limite de 40 e a condição de 5 zeros não foram atingidos. Não houve D07. Quatro propostas privadas congeladas permanecem pendentes: 4 métodos/647 bytes IL; fixtures não são ganho real.

C5: espelhamento genérico compilou 7 candidatos smcs e 5 Roslyn sem avanço. Map.SetCoatLine fechou ao restaurar oito subtrações Int32 na forma 0-x. A proteção foi reforçada contra conversões definidas pelo usuário; os 12 trials anteriores eram Int32 nativos e continuam válidos.

C1 inicial: INFRA_NOT_EVALUATED, 24 alvos sem PDB mapeável, zero trials. O espelhamento autenticado de PDB corrigiu a infraestrutura e c02 fechou SoundOGG. Init2 teve rejeição de vínculo de modelo antes de trials; o vínculo foi refeito e D03 fechou o corpo.

Dois testes puros falharam em D04 por reutilizar estados da fila viva; a fixture foi isolada e 21 testes passaram. Na finalização, 23 testes passaram, incluindo parada explícita e recusa de parada inferida pelo agente.

Roslyn8: MeshManager.GetMeshBuilder prevê 4 locais contra 5 reais, por faltar temporário KeyValuePair de getter sobre struct por valor. Extensão privada medida em 3196 corpos matched: 2250 previsões completas/exatas, zero falsos completos. Não foi selecionada nem publicada. Nenhuma proposta nova foi compilada no jogo após D06.

Errata: Assembly-CSharp referencia KairoLibrary original como metadados/API na compilação; o APK funcional anterior usou a DLL reconstruída. A referência não mudou. Init2 tinha 68 locais, mas 25 posições de tipos divergiam antes da correção.

| Proposta preservada sem teste no jogo | Métodos | IL original potencial | Estado |
| --- | --- | --- | --- |
| prepared-kairo-pause-notify | kairo.unity.ui.IApplication::OnApplicationPause(System.Boolean)->System.Void | 199 | UNKNOWN — não adotada |
| prepared-kairo-transpose-stack | kairo.unity.m3d4.util3d.Transform::Transpose()->System.Void | 197 | UNKNOWN — não adotada |
| prepared-kairo-cache-material | kairo.unity.graphics.MaterialManager::GetMaterial(kairo.unity.graphics.Material,kairo.unity.ui.Image)->kairo.unity.graphics.Material | 110 | UNKNOWN — não adotada |
| prepared-kairo-cache-mesh | kairo.unity.graphics.MeshManager::GetMeshBuilder(kairo.unity.graphics.Material)->kairo.unity.graphics.MeshBuilder | 141 | UNKNOWN — não adotada |

Nenhuma instalação ou arquivo solicitado ao usuário. A rodada foi encerrada pelo pedido de relatório final, antes dos limites normais; há quatro propostas privadas não compiladas no jogo e um defeito conhecido do modelo Roslyn em um método pendente.

8. SUA RECOMENDAÇÃO

1. Validar e selecionar a extensão genérica do modelo Roslyn para getter de struct retornado por indexador; testar MaterialManager.GetMaterial e MeshManager.GetMeshBuilder. Potencial: 2 métodos/251 bytes IL; fixtures passaram, resultado real UNKNOWN.
2. Testar as propostas congeladas de IApplication.OnApplicationPause e Transform.Transpose. Potencial: 2 métodos/396 bytes IL; fixtures inteiras passaram, sem compilação do jogo até a parada.
3. Continuar a KairoLibrary pelos outros nove corpos, começando por LineupData.GetRatio (192 bytes) e a lambda KairoBase.Start (182 bytes). São 2 alvos/374 bytes de busca, sem ganho previsto. Após fechar Kairo, medir bytes brutos e voltar ao Assembly-CSharp.
