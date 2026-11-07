## 1. PROGRESSO

Pocket League Story para Android, APK net.kairosoft.android.soccer_en_2.0.2-APK_Award.apk; reconstrução C#/Unity 5.3.4f1. Comparação com o início desta rodada, commit `c51d204f57a17d5fa025d3b4ed629dfc8ab7ac64`. Matched exige corpo completo (cabeçalho, maxstack, locais, IL e EH); referências de tokens são resolvidas por identidade. Declarações sem corpo contam em métodos, sem bytes de IL.

| Assembly | Anterior (c51d204) | Atual | Δ métodos | Δ bytes de IL | Regressões |
| --- | --- | --- | --- | --- | --- |
| KairoLibrary | 3211/3287 (97.687861%); IL 216823/269894 bytes (80.336354%) | 3241/3287 (98.600548%); IL 249623/269894 bytes (92.489274%) | 30 | 32800 | 0 |
| Assembly-CSharp | 1790/1958 (91.419816%); IL 352587/601603 bytes (58.607919%) | 1844/1958 (94.177732%); IL 447600/601603 bytes (74.401225%) | 54 | 95013 | 0 |

Total: 5085/5245 métodos; 697223/871497 bytes de IL. Retiradas de matched por ID: 0; corpos antes iguais que deixaram de bater: 0.

Motivo da parada: Limite de 40 ciclos autônomos concluídos.

## 2. O QUE FOI FEITO NESTA RODADA

**Parte A — fila preparada concluída.** 5 métodos e 6830 bytes de IL ganhos; 0 perdas. O resultado de cada alvo:

| Alvo preparado | Estado final da Parte A | Prefixo IL antes → depois | Regras efetivamente aceitas |
| --- | --- | --- | --- |
| `kairo.unity.util.StringUtil::GetCode(System.Byte[])->System.String` | matched | 33 → 1326 | roslyn-store-use-lifetime-scope-shared-scratch-locals |
| `kairo.unity.ui.Graphics::DrawArc(System.Single,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single)->System.Void` | matched | 370 → 1188 | roslyn-store-use-lifetime-scope-while-counter-scope-inline-array |
| `kairo.unity.util.StreamUtil::WriteArray(java.io.OutputStream,System.Array,System.Int32)->System.Void` | decompiled | 1 → 188 | nullable-coalesce-negative-ternary, switch-owned-break-if-chain, switch-owned-break-if-chain |
| `form.DebugForm::Draw_coatEva(kairo.unity.ui.Graphics)->System.Void` | matched | 418 → 1937 | calculated-coordinate-local-coalescence |
| `game.Ball::SetSavePos(kairo.unity.m3d4.util3d.Vector3D,kairo.unity.m3d4.util3d.Vector3D,kairo.unity.m3d4.util3d.Vector3D)->System.Void` | matched | 242 → 1251 | calculated-coordinate-local-coalescence, expand-compound-field-multiply, expand-compound-field-multiply, expand-compound-field-multiply, expand-compound-field-multiply, expand-compound-field-multiply |
| `form.DebugForm::Draw_moveArea(kairo.unity.ui.Graphics,System.Int32,System.Int32)->System.Void` | matched | 91 → 1128 | calculated-coordinate-local-coalescence, smcs-continue-positive-tail |
| `game.Ball::Update()->System.Boolean` | decompiled | 450 → 4294 | expand-compound-field-multiply, expand-compound-field-multiply, expand-compound-field-multiply, expand-compound-field-multiply, expand-compound-field-multiply, unary-negation, expand-compound-field-multiply, expand-compound-field-multiply, unary-negation, expand-compound-field-multiply, expand-compound-field-multiply, smcs-empty-arm-restore-and-suffix, if-polarity, if-polarity, smcs-balanced-empty-arms |

**Parte B — testes isolados.** Cada sonda usou uma família e uma ocorrência; recusa por assinatura prevista não refuta a hipótese de controle.

| Alvo | Resultado | Variantes compiladas | Prefixo antes → depois |
| --- | --- | --- | --- |
| `form.GameForm::_update(System.Int32)->System.Boolean` | INCONCLUSIVA: candidato recusado antes da compilacao | 0 | 193 → 193 |
| `form.SubForm::Init()->System.Boolean` | INCONCLUSIVA: candidato recusado antes da compilacao | 0 | 163 → 163 |
| `form.SoccerForm::Draw_newPlayerPopular(kairo.unity.ui.Graphics)->System.Void` | CONFIRMADA: corpo integral | 1 | 105 → 999 |
| `kairo.unity.ui.Balloon::Draw(kairo.unity.ui.Graphics,System.Single,System.Single,UnityEngine.Matrix4x4)->System.Void` | CONFIRMADA somente no prefixo medido; corpo INCONCLUSIVO | 1 | 29 → 1445 |
| `kairo.unity.ui.Graphics::_drawBitmap(kairo.unity.ui.Image,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single)->System.Void` | REFUTADA para esta forma isolada; demais formas UNKNOWN | 1 | 1822 → 1822 |

**Parte C — extração e aplicação do esqueleto smcs.** Corpus antes de C: 1253/1253 árvores completas iguais; cobertura 1253/1788 corpos matched. Pendentes: 50 extrações IL completas, 43 fontes completas, 27 pares completos; 14 iguais e 13 diferentes antes. 8 diferenças com folhas bijetivas; 7 reescritas calculadas com árvore/plano originais, 7 corpos fechados. Mais 1 corpo por locais sem uso. Total oficial: 8 métodos, 2269 bytes, zero regressões.

Ganhos oficiais vêm do diário após adoção e verificação completa, incluindo auxiliares liberados pelo verificador. Sondas e avanços parciais são registrados separadamente; não se somam aos ganhos oficiais. Transformações herdadas da rodada 7 só aparecem abaixo quando foram efetivamente tentadas nesta rodada.

| Ciclo | Assembly / regra | Matched ganhos | Bytes ganhos |
| --- | --- | --- | --- |
| a01 | KairoLibrary / roslyn-store-use-lifetime-scope-shared-scratch-locals | 1 | 1326 |
| a02 | KairoLibrary / roslyn-store-use-lifetime-scope-while-counter-scope | 1 | 1188 |
| a03 | KairoLibrary / nullable-coalesce-negative-ternary | 0 | 0 |
| a04 | Assembly-CSharp / calculated-coordinate-local-coalescence | 3 | 4316 |
| a05 | Assembly-CSharp / expand-compound-field-multiply | 0 | 0 |
| b06 | Assembly-CSharp / b9-event-loop-split-guard | 0 | 0 |
| b07 | Assembly-CSharp / b9-empty-arm-common-return | 0 | 0 |
| b08 | Assembly-CSharp / b9-guard-return-empty-positive-arm | 1 | 999 |
| b09 | KairoLibrary / b9-positive-render-with-gui-else | 0 | 0 |
| b10 | KairoLibrary / b9-boolean-value-before-if | 0 | 0 |
| c11 | Assembly-CSharp / c9-control-skeleton | 8 | 2269 |
| cycle01 | Assembly-CSharp / terminal-if-arm-return-flatten-else | 2 | 1514 |
| cycle02 | Assembly-CSharp / d9-joint-flow-local-stores | 1 | 1687 |
| cycle03 | Assembly-CSharp / scoped-terminal-arm-common-join | 2 | 2552 |
| cycle04 | KairoLibrary / d9-calculated-per-arm-local-stores | 1 | 190 |
| cycle05 | KairoLibrary / d9-explicit-float-identity-parameter | 1 | 97 |
| cycle06 | Assembly-CSharp / d9-calculated-multiple-selector-local-splitting | 1 | 2067 |
| cycle07 | KairoLibrary / d9-calculated-state-dispatch-index | 1 | 1109 |
| cycle08 | KairoLibrary / d9-named-eh-result-rejoin | 1 | 189 |
| cycle09 | Assembly-CSharp / d9-array-local-before-population | 2 | 2579 |
| cycle10 | Assembly-CSharp / d9-explicit-field-division | 2 | 2183 |
| cycle11 | KairoLibrary / d9-boolean-value-branch-context | 0 | 0 |
| cycle12 | Assembly-CSharp / d9-protected-exit-owner | 1 | 222 |
| cycle13 | Assembly-CSharp / d9-calculated-foreach-slot-control-composition | 4 | 2352 |
| cycle14 | KairoLibrary / d9-calculated-boolean-block-storage-composition | 1 | 1072 |
| cycle15 | Assembly-CSharp / d9-calculated-root-guard-declaration-composition | 2 | 2949 |
| cycle16 | KairoLibrary / d9-calculated-graphics-Boolean-block-negation-composition | 1 | 3513 |
| cycle17 | KairoLibrary / d9-calculated-surface-switch-float-loop-composition | 1 | 2305 |
| cycle18 | KairoLibrary / d9-calculated-named-receiver-protected-return-order | 0 | 0 |
| cycle19 | Assembly-CSharp / d9-calculated-terminal-empty-arm-chain-composition | 1 | 6994 |
| cycle20 | KairoLibrary / d9-calculated-execute-eager-tail-nullguards-if-dispatch | 1 | 1088 |
| cycle21 | Assembly-CSharp / d9-calculated-terminal-branch-joint-compositions | 2 | 9820 |
| cycle22 | Assembly-CSharp / d9-calculated-arithmetic-scope-dispatch-compositions | 2 | 5086 |
| cycle23 | KairoLibrary / d9-calculated-late-local-child-scopes | 3 | 4904 |
| cycle24 | Assembly-CSharp / d9-calculated-dribble-six-hunk-composition | 1 | 4343 |
| cycle25 | Assembly-CSharp / d9-calculated-draw-ten-hunk-composition | 1 | 8704 |
| cycle26 | KairoLibrary / d9-calculated-render-and-format-foreach-compositions | 2 | 3022 |
| cycle27 | Assembly-CSharp / d9-calculated-sibling-block-exit-compositions | 2 | 4319 |
| cycle28 | Assembly-CSharp / d9-calculated-section-and-loop-target-compositions | 3 | 5902 |
| cycle29 | KairoLibrary / d9-calculated-text-and-image-scope-compositions | 2 | 4222 |
| cycle30 | Assembly-CSharp / d9-calculated-condition-gate-compositions | 2 | 3897 |
| cycle31 | Assembly-CSharp / d9-calculated-bound-guard-compositions | 3 | 4926 |
| cycle32 | KairoLibrary / d9-calculated-protected-type-and-array-tail-compositions | 2 | 2745 |
| cycle33 | Assembly-CSharp / d9-calculated-empty-state-compositions | 3 | 5008 |
| cycle34 | Assembly-CSharp / d9-calculated-join-loop-compositions | 2 | 6503 |
| cycle35 | Assembly-CSharp / d9-calculated-touch-ticket-compositions | 3 | 3822 |
| cycle36 | KairoLibrary / d9-calculated-image-protected-exit-compositions | 1 | 2248 |
| cycle37 | KairoLibrary / d9-calculated-protected-exit-owner-compositions | 3 | 1155 |
| cycle38 | KairoLibrary / d9-calculated-whole-guard-exit-compositions | 3 | 1130 |
| cycle39 | KairoLibrary / d9-calculated-indexer-local-inner-compositions | 2 | 872 |
| cycle40 | KairoLibrary / d9-calculated-field-outer-result-compositions | 2 | 425 |

Dois primeiros ensaios de extração AST falharam por profundidade JSON64; o limite do serializador foi corrigido para2048. Uma tentativa posterior falhou no vínculo de IDs aninhados, corrigido pelo mapa oficial do receipt. Nenhum desses ensaios mudou fontes do jogo.

As primeiras medições de desenvolvimento com helpers alterados durante a execução foram marcadas inválidas; os resultados publicados e finais usam hashes congelados e completude explícita.

O commit B07 foi retido quando um arquivo de análise mudou depois do stage; a prova completa foi refeita. O localizador B09 exigiu assinatura completa para resolver uma sobrecarga.

Um EOF vazio de helper congelado motivou permitir somente blank-at-eof no lint de estilo; o verificador de métodos/metadados não mudou. D01 revelou trivia final em duas linhas geradas: a limpeza protege literais e preserva finais de linha, passou quatro controles e exigiu nova prova completa.

O planner inicial não vinculava / no stable_id a + na chave oficial de um parâmetro aninhado. Foi corrigido usando exclusivamente o mapa stable_id→chave do receipt, sem renomear assinaturas nem editar estados.

D29: a API pura v1 aceitou e removeu SideEffects++ em um controle de texto; foi endurecida por igualdade de todos os tokens antes de compilar candidatos do jogo. O controle foi recusado depois da correção; não houve regressão adotada.

D33 e D34: nomes de diretórios dos runners foram corrigidos antes de qualquer compilação. O gravador do checkpoint e uma leitura de handoff falharam com cp1252 e foram corrigidos para UTF-8 antes de escrever dados. Nenhuma fonte, estado ou candidato foi afetado. Nenhum controle C# de D30 a D40 falhou.

Os WIPs congelados conservam NOT_TESTED mesmo quando a família foi testada depois; os relatórios públicos, recibos e diário são a autoridade. Ao parar, SetWinCharaTraining ficou somente como API/templates não testados; CGI_COMMON_QUERY ficou sem rota de mapeamento e sem teste. O diagnóstico de _draw4 não foi forçado a uma API integral.

Manutenção nativa de armazenamento em JSONs privados arquivados preservou conteúdo e tamanho, com prova SHA antes/depois. Não alterou original, referências, fontes públicas ou binários; seus bytes físicos não entram nas métricas de IL e nenhum assembly foi normalizado.

A primeira geração final teve caracteres corrompidos nas notas acrescentadas por um pipeline de texto do Windows; os textos foram corrigidos por arquivo UTF-8 e a prova completa do commit foi refeita. Nenhuma fonte alvo, método, estado ou métrica foi alterado por essa correção.

Frentes preparadas antes da parada, sem teste dos respectivos candidatos no jogo; ganhos e perdas adotados: zero.

| Frente | Preparado / prova | Compilações de candidatos do jogo | Métodos ganhos / perdidos |
| --- | --- | --- | --- |
| SetWinCharaTraining: API pura e templates NOT_TESTED; nenhuma fixture/modelo/candidatura do jogo desta proposta | 1 alvos; 412 bytes; 0 corpos de fixture exatos | 0 | 0 / 0 |
| CGI_COMMON_QUERY / AppData..cctor: expressão pura; rota de inicializador sem mapeamento; sem fixture/modelo/candidatura, sem model-veto | 1 alvos; 1816 bytes; 0 corpos de fixture exatos | 0 | 0 / 0 |

Resultados negativos de todas as execuções concluídas, incluindo sondas; só aparecem linhas com alguma falha/recusa. Não são perdas adotadas.

| Execução | Compilações falhas | Regressões recusadas | Metadados recusados | Modelo/emissor | Assinatura emitida recusada | Assinatura prevista recusada |
| --- | --- | --- | --- | --- | --- | --- |
| round9-a04-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 | 0 | 4 |
| round9-a05-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 | 0 | 133 |
| round9-a05-probe-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 | 0 | 132 |
| round9-b06-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 | 0 | 1 |
| round9-b06-probe-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 | 0 | 1 |
| round9-b07-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 | 0 | 1 |
| round9-b07-probe-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 | 0 | 1 |
| round9-b10-KairoLibrary-engine.json | 0 | 0 | 0 | 0 | 1 | 0 |
| round9-b10-probe-KairoLibrary-engine.json | 0 | 0 | 0 | 0 | 1 | 0 |
| round9-cycle01-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 | 0 | 93 |
| round9-cycle01-probe-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 | 0 | 9 |
| round9-cycle02-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 | 0 | 697 |
| round9-cycle02-probe-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 | 0 | 697 |
| round9-cycle03-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 | 0 | 1 |
| round9-cycle04-KairoLibrary-engine.json | 0 | 0 | 0 | 0 | 2 | 0 |
| round9-cycle04-probe-KairoLibrary-engine.json | 0 | 0 | 0 | 0 | 2 | 0 |
| round9-cycle05-KairoLibrary-engine.json | 0 | 0 | 0 | 0 | 1 | 0 |
| round9-cycle05-probe-KairoLibrary-engine.json | 0 | 0 | 0 | 0 | 1 | 0 |
| round9-cycle09-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 | 0 | 14 |
| round9-cycle09-probe-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 | 0 | 6 |
| round9-cycle10-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 | 0 | 44 |
| round9-cycle10-probe-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 | 0 | 44 |
| round9-cycle11-KairoLibrary-engine.json | 0 | 0 | 0 | 0 | 1 | 0 |
| round9-cycle11-probe-KairoLibrary-engine.json | 0 | 0 | 0 | 0 | 1 | 0 |
| round9-cycle12-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 | 0 | 2 |
| round9-cycle12-probe-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 | 0 | 2 |
| round9-cycle13-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 | 0 | 8 |
| round9-cycle13-probe-Assembly-CSharp-engine.json | 0 | 0 | 0 | 0 | 0 | 4 |
| round9-cycle18-KairoLibrary-engine.json | 0 | 0 | 0 | 0 | 6 | 0 |
| round9-cycle18-probe-KairoLibrary-engine.json | 0 | 0 | 0 | 0 | 6 | 0 |
| round9-cycle29-KairoLibrary-engine.json | 0 | 0 | 0 | 0 | 8 | 0 |

Variantes incompletas compiladas: 118; invocações dessas variantes: 181. Esses custos incluem sondas; ganhos não são contados duas vezes.

## 3. HIPÓTESES

| Estado | Hipótese | Evidência medida |
| --- | --- | --- |
| CONFIRMADA | roslyn-store-use-lifetime-scope-shared-scratch-locals: FilaA: quatro scratch locais compartilhados nos blocos de GetCode reproduzem os31slotsoriginais; candidatos exclusivamente do bundle preparado. | a01: 1 matched, 1326 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos |
| CONFIRMADA | roslyn-store-use-lifetime-scope-while-counter-scope: FilaA: contador no escopo do laco e inline do alias de array em DrawArc; modelo incompleto pode compilar sob orcamento e assinatura real decide. | a02: 1 matched, 1188 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos |
| INCONCLUSIVA | nullable-coalesce-negative-ternary: FilaA: ternario negativo preserva orientacao call-first e callvirt do original; teste do candidato preparado sem normalizacao. | a03: 0 matched, 0 IL bytes; Avanco parcial; nenhum novo matched; 1 tentativas da regra, 1 avancos |
| CONFIRMADA | calculated-coordinate-local-coalescence: FilaA: coalescencia de coordenadas e escopos calculados preservam inicializadores e igualam assinaturas originais. | a04: 3 matched, 4316 IL bytes; Novos corpos aprovados; 3 tentativas da regra, 3 avancos |
| INCONCLUSIVA | expand-compound-field-multiply: FilaA: *= em campos float ou double de receiver estavel vira atribuicao explicita com recarga do receiver, sem mudar ordem ou agrupar RHS. | a05: 0 matched, 0 IL bytes; Avanco parcial; nenhum novo matched; 9 tentativas da regra, 9 avancos |
| INCONCLUSIVA | b9-event-loop-split-guard: Separar uma guarda protegida no for preserva o salto ao incremento, sem inferir break pelo rotulo | b06: 0 matched, 0 IL bytes; INCONCLUSIVA: nenhum candidato autorizado pelo modelo/mapeamento; 0 tentativas da regra, 0 avancos |
| INCONCLUSIVA | b9-empty-arm-common-return: Braco verdadeiro vazio e else preservam o join comum; return direto e controle negativo | b07: 0 matched, 0 IL bytes; INCONCLUSIVA: nenhum candidato autorizado pelo modelo/mapeamento; 0 tentativas da regra, 0 avancos |
| CONFIRMADA | b9-guard-return-empty-positive-arm: Substituir uma guarda de retorno por braco vazio e else para cair no join do if | b08: 1 matched, 999 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos |
| INCONCLUSIVA | b9-positive-render-with-gui-else: Corpo sob teste positivo de render com ramo GUI em else preserva saida final | b09: 0 matched, 0 IL bytes; Avanco parcial; nenhum novo matched; 1 tentativas da regra, 1 avancos |
| INCONCLUSIVA | b9-boolean-value-before-if: Materializar condicao como valor bool antes do if preserva negacao ceq | b10: 0 matched, 0 IL bytes; Nenhum candidato compilado avancou; 1 tentativas da regra, 0 avancos |
| CONFIRMADA | c9-control-skeleton: Reemitir somente o controle completo com folhas bijetivas e locais calculados reproduz o corpo original | c11: 8 matched, 2269 IL bytes; Novos corpos aprovados; 8 tentativas da regra, 8 avancos |
| CONFIRMADA | terminal-if-arm-return-flatten-else: Ret original contra join sintetico: encerrar then e retirar else preservando escopo e condicao | cycle01: 2 matched, 1514 IL bytes; Novos corpos aprovados; 7 tentativas da regra, 7 avancos |
| CONFIRMADA | d9-joint-flow-local-stores: Calcular num mesmo candidato fluxo e locais de seletores que impediam aprovar as correcoes isoladas | cycle02: 1 matched, 1687 IL bytes; Novos corpos aprovados; 2 tentativas da regra, 2 avancos |
| CONFIRMADA | scoped-terminal-arm-common-join: Br original para join provado contra return recuperado: conservar operador e acoes e estruturar else ate o destino, aceitando somente join local ou braco nao linear novo. | cycle03: 2 matched, 2552 IL bytes; Novos corpos aprovados; 2 tentativas da regra, 2 avancos |
| CONFIRMADA | d9-calculated-per-arm-local-stores: If/else com armazenamento do local em cada braco, em vez do join de valor ternario, reproduz stores por braco comprovados. | cycle04: 1 matched, 190 IL bytes; Novos corpos aprovados; 4 tentativas da regra, 4 avancos |
| CONFIRMADA | d9-explicit-float-identity-parameter: Conversao explicita Single para Single na ocorrencia comprovada preserva conv.r4 apos carga de parametro Single. | cycle05: 1 matched, 97 IL bytes; Novos corpos aprovados; 2 tentativas da regra, 1 avancos |
| CONFIRMADA | d9-calculated-multiple-selector-local-splitting: Nomear conjuntamente os tres seletores Int32 nos slots originais e converter seus switches em if-chains, avaliando cada seletor uma vez e preservando donos de breaks. | cycle06: 1 matched, 2067 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos |
| CONFIRMADA | d9-calculated-state-dispatch-index: Declarar indice e estado no bloco pai e recuperar dispatch if(estado==0)/else if(estado==1), preservando a leitura do estado e somente retirando os breaks finais comprovados do switch. | cycle07: 1 matched, 1109 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos |
| CONFIRMADA | d9-named-eh-result-rejoin: Manter resultado Byte[] inicializado antes do try, atribuir ToByteArray ao mesmo local e retorna-lo depois do finally, evitando o temporario de retorno protegido separado. | cycle08: 1 matched, 189 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos |
| CONFIRMADA | d9-array-local-before-population: Array unidimensional atribuido a local antes da populacao explicita reproduz newarr;stloc;ldloc;indice em vez de inicializador dup; mesma ordem de elementos e nenhum local novo. | cycle09: 2 matched, 2579 IL bytes; Novos corpos aprovados; 5 tentativas da regra, 5 avancos |
| CONFIRMADA | d9-explicit-field-division: Divisao simples f=f/rhs em campo de receptor estavel reproduz carga dupla original; compound /= usa dup. Aplicar uma ocorrencia com tipos primitivos iguais e assinatura original aprovada. | cycle10: 2 matched, 2183 IL bytes; Novos corpos aprovados; 3 tentativas da regra, 3 avancos |
| INCONCLUSIVA | d9-boolean-value-branch-context: Contexto Boolean de valor exige ternario semlocal com juncao0/1; contexto de branch elimina alias adjacente de uma leitura e normalizacao explicita com zero. Preservar expressoes internas e ordem de chamadas. | cycle11: 0 matched, 0 IL bytes; Nenhum candidato compilado avancou; 2 tentativas da regra, 0 avancos |
| CONFIRMADA | d9-protected-exit-owner: Restaurar owner EH: mover break terminal do For para Try e remover Continue terminal Catch; em void IfTryReturns retirar returns protegidos e criar else com tail, preservando leaves automaticos e limites EH. | cycle12: 1 matched, 222 IL bytes; Novos corpos aprovados; 2 tentativas da regra, 2 avancos |
| CONFIRMADA | d9-calculated-foreach-slot-control-composition: Compor arrayforeach smcs com despacho/Boolean ou continue ou local de retorno final para igualar inventario/ordem original; Process e controle explicito de primitiva antiga sem trial compilado anterior provado. | cycle13: 4 matched, 2352 IL bytes; Novos corpos aprovados; 4 tentativas da regra, 4 avancos |
| CONFIRMADA | d9-calculated-boolean-block-storage-composition: Compor hunksBooleanD11 com ordemcache-first e eagerAnd para restaurarslotsGraphics; Shader inlinealias+perarmstores+bit80positivechain+finaltextlocal para restaurartamanho/joins. Nao repetirformas isoladas insuficientes. | cycle14: 1 matched, 1072 IL bytes; Novos corpos aprovados; 2 tentativas da regra, 1 avancos |
| CONFIRMADA | d9-calculated-root-guard-declaration-composition: Compor guarda positiva com bare declarations preservadas no pai; somenteinicializacoes+tail entramnoif. Mantemslots9/30originale elimina br+ret residual, diferentedaspiecesisoladasnegativas. | cycle15: 2 matched, 2949 IL bytes; Novos corpos aprovados; 2 tentativas da regra, 2 avancos |
| CONFIRMADA | d9-calculated-graphics-Boolean-block-negation-composition: PayloadinteiroGraphicsD14maisUnaryNegation comprova52slots e removecinco bytes extrasde0f-sub, semadotarintermediariorejeitado; somenteCSharpcompiladoeoraculocompleto. | cycle16: 1 matched, 3513 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos |
| CONFIRMADA | d9-calculated-surface-switch-float-loop-composition: Comporcase3vazio comifFloatpositivoNaN-safe ePositiveguardnobodyFor; removeartefatosdeordemdecompilada semnovosnomeselocais; modeloincompletolimitadoeassinaturarealobrigatoria. | cycle17: 1 matched, 2305 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos |
| INCONCLUSIVA | d9-calculated-named-receiver-protected-return-order: NomearreceptorNotifyManagerGetInstance noMESMOtryantesLazyReturnTemp restabelecelocal8 ecallvirt, avaliandouma vezsemchildblock; modeloINCOMPLETOlimitado eassinaturareal16obrigatoria. | cycle18: 0 matched, 0 IL bytes; Avanco parcial; nenhum novo matched; 1 tentativas da regra, 1 avancos |
| CONFIRMADA | d9-calculated-terminal-empty-arm-chain-composition: Restaurar oito braçosvazios type4..11 e ligara caudatype12 inteira, em vezORreturn abreviado; 93locaismodelados exatamentee nenhuma repeticaodocandidatoD01. | cycle19: 1 matched, 6994 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos |
| CONFIRMADA | d9-calculated-execute-eager-tail-nullguards-if-dispatch: Compose eager Boolean tail under reference guard, explicit stable Lock/Unlock null guards and two owned-case if dispatch while preserving D18 receiver and sixteen original locals | cycle20: 1 matched, 1088 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos |
| CONFIRMADA | d9-calculated-terminal-branch-joint-compositions: Compose owner-proven three tenant continues with terminal62/63/64 chain preserving D02 selectors; separately compose precise DEBUG empty suffix plus positive float block order preserving NaN and original slots | cycle21: 2 matched, 9820 IL bytes; Novos corpos aprovados; 2 tentativas da regra, 2 avancos |
| CONFIRMADA | d9-calculated-arithmetic-scope-dispatch-compositions: Compose two owner-proven if dispatches, continue, per-arm scalar stores, root declaration order and local-before-field with known divisions; restore shared training floats and unused original Single slot with known multiply and negation forms | cycle22: 2 matched, 5086 IL bytes; Novos corpos aprovados; 2 tentativas da regra, 2 avancos |
| CONFIRMADA | d9-calculated-late-local-child-scopes: Balloon post-continue tail as child block and paired Seb scan counters in for-init child scopes, preserving all body/control order and requiring actual original local signature and full body equality | cycle23: 3 matched, 4904 IL bytes; Novos corpos aprovados; 3 tentativas da regra, 3 avancos |
| CONFIRMADA | d9-calculated-dribble-six-hunk-composition: Whole six-hunk composition: positive collision tail with loop break plus nested empty info join, explicit self continue, separate ordered team float continues, and address equality continue before positive collision; not isolated C17 replay | cycle24: 1 matched, 4343 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos |
| CONFIRMADA | d9-calculated-draw-ten-hunk-composition: Whole ten-hunk draw composition preserves valid Window returns, restores two null-continue owners and terminal type empty arms, removes only extra last Int32 switch cache via direct two-case if dispatch and final29 empty arm | cycle25: 1 matched, 8704 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos |
| CONFIRMADA | d9-calculated-render-and-format-foreach-compositions: Compose whole Offscreen graphics child scope, per-arm callback result, stable struct-array operator assignments, four negations and fallback else; Language parser array for becomes resource-once foreach with first effect Split invocation and temp reuse judged only by compiler | cycle26: 2 matched, 3022 IL bytes; Novos corpos aprovados; 2 tentativas da regra, 2 avancos |
| CONFIRMADA | d9-calculated-sibling-block-exit-compositions: Title highlight guard skips only two DrawSeb calls then preserves next menu sibling, with three zero-first Int32 ternaries; Team state5 skips only section7 Cal tail then preserves section8 sibling; no method or loop exit invented | cycle27: 2 matched, 4319 IL bytes; Novos corpos aprovados; 2 tentativas da regra, 2 avancos |
| CONFIRMADA | d9-calculated-section-and-loop-target-compositions: Compose ExecuteScript local if joins before section breaks, Map direct continue guards with preserved child blocks, and nine Zsort direct loop guards while preserving the distinct case9 and proving switch exhausts loop tail | cycle28: 3 matched, 5902 IL bytes; Novos corpos aprovados; 3 tentativas da regra, 3 avancos |
| CONFIRMADA | d9-calculated-text-and-image-scope-compositions: Compose strict-token text alignment stores and negation, pictograph frame stores plus late length scope, and Graphics Single casts with explicit optimize stores and shared positive lookup scope; signatures and whole bodies require actual compiler proof | cycle29: 2 matched, 4222 IL bytes; Novos corpos aprovados; 3 tentativas da regra, 3 avancos |
| CONFIRMADA | d9-calculated-condition-gate-compositions: Separate null continue from positive purchase admission; restore empty type4 and num-X-Y guards with positive collision phases and per-arm floating result stores | cycle30: 2 matched, 3897 IL bytes; Novos corpos aprovados; 2 tentativas da regra, 2 avancos |
| CONFIRMADA | d9-calculated-bound-guard-compositions: Restore four signed bounds returns, compose frame-continue and type6 elseif, and invert Lineup inner bound while removing its outer empty else | cycle31: 3 matched, 4926 IL bytes; Novos corpos aprovados; 3 tentativas da regra, 3 avancos |
| CONFIRMADA | d9-calculated-protected-type-and-array-tail-compositions: Nineteen protected type arms form one common-exit chain; positive first array guard inside try plus explicit stable Close receiver guard restores normal exits | cycle32: 2 matched, 2745 IL bytes; Novos corpos aprovados; 2 tentativas da regra, 2 avancos |
| CONFIRMADA | d9-calculated-empty-state-compositions: Compose positive mode/state tails with exact empty-arm chains, direct index tests and unary neg; preserve TenantIn intermediate if join instead of loop continue | cycle33: 3 matched, 5008 IL bytes; Novos corpos aprovados; 3 tentativas da regra, 3 avancos |
| CONFIRMADA | d9-calculated-join-loop-compositions: Restore Friend mail/apply/dialog if-else joins with anonymous bodies unchanged; compose both flag-for tests and positive-hit continues before final state14 | cycle34: 2 matched, 6503 IL bytes; Novos corpos aprovados; 2 tentativas da regra, 2 avancos |
| CONFIRMADA | d9-calculated-touch-ticket-compositions: Compose touch positive array tail, owned component continue, reference ternary, implicit Int32 shift masks and separate collision guard; empty ticket positive Equals without extra else | cycle35: 3 matched, 3822 IL bytes; Novos corpos aprovados; 3 tentativas da regra, 3 avancos |
| CONFIRMADA | d9-calculated-image-protected-exit-compositions: Add explicit protected terminal return at existing lookup child and restore SByte-first access branch together; access-only D29 trial remains rejected | cycle36: 1 matched, 2248 IL bytes; Novos corpos aprovados; 1 tentativas da regra, 1 avancos |
| CONFIRMADA | d9-calculated-protected-exit-owner-compositions: Restore successful retry break inside try and catch fallthrough, catch continue to unique while, shared heap return with explicit media3 return | cycle37: 3 matched, 1155 IL bytes; Novos corpos aprovados; 3 tentativas da regra, 3 avancos |
| CONFIRMADA | d9-calculated-whole-guard-exit-compositions: Surface and forced Unlock positive whole tails retain prefix and EH; Offscreen restores two distinct early guards before exact out-delegate payload | cycle38: 3 matched, 1130 IL bytes; Novos corpos aprovados; 3 tentativas da regra, 3 avancos |
| CONFIRMADA | d9-calculated-indexer-local-inner-compositions: In both MaterialManager overloads place named Dictionary and Material local stores inside their indexer assignments, eliminating used-assignment spill temps together | cycle39: 2 matched, 872 IL bytes; Novos corpos aprovados; 2 tentativas da regra, 2 avancos |
| CONFIRMADA | d9-calculated-field-outer-result-compositions: Fuse adjacent named Color array and field store into instance-field outer indexer assignment, retaining one synthetic Color array temp and original receiver stack order | cycle40: 2 matched, 425 IL bytes; Novos corpos aprovados; 2 tentativas da regra, 2 avancos |
| INCONCLUSIVA | As guardas isoladas B06/B07 bastariam para GameForm._update e SubForm.Init. | reports/round9-part-b-results.json: zero candidatos compilados em cada alvo; o modelo completo recusou as assinaturas. A recusa não testa a emissão do corpo. Composições diferentes fecharam _update em D02 e Init em D21. |
| REFUTADA | Em SoccerForm.Draw_newPlayerPopular, o alvo L5 da leitura B08 seria a saída do método. | A análise B08 identifica L5 no fluxo para incremento do laço (746→835); braço positivo vazio com else compilou corpo inteiro de999 bytes. Continue direto foi negativo na fixture. |
| CONFIRMADA | A guarda positiva de Balloon.Draw(Matrix) explica o prefixo B09, mas precisa de correções de escopo para o corpo inteiro. | B09 avançou29→1445 sem fechar o corpo; D23, com os escopos tardios adicionados, verificou o corpo inteiro de3012 bytes. Não prova a grafia histórica do fonte. |
| REFUTADA | O local Boolean isolado B10 seria suficiente para Graphics._drawBitmap. | B10 compilou uma variante, não avançou o prefixo1822 e recusou a assinatura emitida. Demais formas não são refutadas por esse resultado; o corpo inteiro fechou com composição distinta em D16. |
| CONFIRMADA | O ternário Boolean de D11 reproduz o trecho local de Graphics._drawBitmap, embora não resolva sozinho o método. | Inspeção da emissão D11 confirmou o trecho original1880..1886; a assinatura permutada já existia no baseline, e um leave anterior mudou de destino. D14 corrigiu a assinatura real52; D16 acrescentou neg e fechou3513 bytes. |
| CONFIRMADA | Inlining do alias de Shader.GetShader elimina a normalização Boolean local, mas precisa de stores por braço e ordem de comandos. | D11 removeu a normalização no trecho200 mantendo16 locais, mas o desvio anterior192 mudou671→668 e violou o prefixo. A composição D14 com stores e ordem restaurados fechou1072 bytes. |
| REFUTADA | A composição Graphics de D14, sem alteração aritmética, seria suficiente para fechar _drawBitmap. | D14 emitiu assinatura original52, mas3518 contra3513 bytes, com somente ldc.r4 0/sub versus neg nos dois trechos alinhados remanescentes. D16 adicionou menos unário e verificou corpo, cabeçalho, locais e EH completos. |
| REFUTADA | Uma previsão incompleta com tipos discordantes implica necessariamente assinatura emitida incorreta. | D17 SurfaceBase.DoEvent: previsão explicitamente INCOMPLETE30 versus29 locais originais; compilação real emitiu os29 corretos e passou o corpo inteiro2305 bytes. Previsão incompleta continua sem poder de veto, dentro do orçamento12/pass e24/método/ciclo. |
| REFUTADA | A API pura v1 de make-alignment-tail seria segura para trechos arbitrários apenas com os controles originais. | Preparação D29: o controle NegativeExtraFieldWrite com SideEffects++ foi aceito e perdeu o comando na API v1; nenhum candidato do jogo foi compilado nessa etapa. O helper publicado exige igualdade de todos os tokens da cauda e recusou os8 negativos, mantendo3fixtures de corpo completo em9compilaçõestoy. O before real congelado não continha a escrita extra. |
| REFUTADA | A composição D29 de Graphics._drawImage seria suficiente para reproduzir o corpo inteiro. | O main D29 completo59/59 e o full oracle confirmaram somente um avanço parcial673→1589 para esse ID,12trials e parada incomplete_candidate_budget; o método não entrou em matched e nenhum byte foi contado como ganho dele. Isso refuta a suficiência do candidato integral, sem refutar os trechos que passaram. Os outros dois alvos D29 fecharam4222IL. |

Zero ganho permanece INCONCLUSIVA, salvo prova isolada de refutação. A classificação não identifica o fonte histórico literal.

| Assembly/modelo | Matched do corpus | Mapeados | Sem mapeamento | Completos | Completos exatos | Precisão | Cobertura | Mapeados incompletos | Incompletos com tipos iguais |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| KairoLibrary | 3165 | 2834 | 331 | 2242 | 2242 | 100.000000% | 70.837283% | 592 | 323 |
| Assembly-CSharp | 1838 | 1758 | 80 | 1758 | 1758 | 100.000000% | 95.647443% | 0 | 0 |

**Extrator de controle smcs.** Corpus congelado: `build/round9/final-corpus`. Planos e árvores completos têm denominadores distintos; uma cobertura integral das instruções não completa uma árvore ambígua.

| Métrica | Resultado |
| --- | --- |
| Árvores completas iguais / pares completos | 1267/1267 (100.000000%) |
| Cobertura das árvores nos corpos matched | 1267/1838 (68.933624%) |
| Planos completos iguais / completos | 1459/1459 |
| Pendentes: IL completo / fonte completa / pares completos | 36 / 27 / 15 |
| Parte C: diferenças antes / reescritas com árvore igual / corpos fechados | 13 / 7 / 7 |
| Parte C: corpo adicional por locais sem uso | 1 |

A árvore original não distingue formas fonte que emitem os mesmos saltos sem evidência adicional. Ownership de salto ambíguo, EH, switch, construções anônimas e outros casos sem molde provado continuam incompletos. A AST é extraída somente do fonte; planos da compilação própria autenticam o vínculo das folhas, nunca inventam estrutura fonte. A correspondência final continua sendo corpo completo e metadados no verificador.

Precisão usa somente previsões completas; cobertura usa todos os corpos matched do corpus congelado. Incompletos ou métodos gerados sem mapeamento continuam UNKNOWN. Coincidência de tipos em previsão incompleta não é prova do lowering. Previsão incompleta PERMITE compilar: limite de 12 variantes por método/passagem, 24 por método/ciclo, no máximo uma sonda isolada e uma passagem plena. Cada variante pode exigir screen e compilação plena; variantes e invocações são medidas separadamente. Previsão completa com assinatura errada é recusada antes de compilar. Depois da compilação, a assinatura emitida precisa ser a original; previsão completa também precisa concordar com o emissor. Cabeçalho, locais, IL e EH continuam sujeitos ao mesmo verificador integral.

## 4. PENDENTES POR GRUPO

| Assembly | Faixa de IL original | Corpos pendentes | Bytes de IL |
| --- | --- | --- | --- |
| KairoLibrary | <=64 | 2 | 79 |
| KairoLibrary | 65-256 | 18 | 2784 |
| KairoLibrary | 257-1024 | 20 | 10084 |
| KairoLibrary | >1024 | 4 | 7306 |
| Assembly-CSharp | <=64 | 5 | 237 |
| Assembly-CSharp | 65-256 | 17 | 2689 |
| Assembly-CSharp | 257-1024 | 56 | 30780 |
| Assembly-CSharp | >1024 | 35 | 120290 |

Pendentes com assinatura de locais igual: KairoLibrary 19/44 (diferente: 25); Assembly-CSharp 53/113 (diferente: 60).

Corpos já iguais, retidos por vínculo/metadados gerados: 3; 25 bytes. Esses corpos não entram no histograma dos divergentes.

Peso de um par = soma do tamanho integral dos métodos nele; não representa quantidade de bytes diferentes. Alinhamento e rótulos servem ao diagnóstico, sem normalizar o verificador. Seguem os cinco pares de opcodes mais pesados de cada compilador, com IL original e recompilado lado a lado.

**KairoLibrary #1: `stloc.2 => stloc.s` — 1 métodos, 2403 bytes.** Exemplo: `kairo.unity.ui.Seb::_load(java.io.InputStream)->System.Void`.

Tentativas compiladas no grupo na rodada 9: nenhum. Histórico 7/8 compatível: UNKNOWN; não se presume compatibilidade após alteração do fonte. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 247.

| IL original | IL recompilado |
| --- | --- |
| `IL_0025: stloc.2` | `IL_0025: stloc.s 4` |
| `IL_0026: br.s L1` | `IL_0027: br.s L1` |

**KairoLibrary #2: `brfalse.s => brtrue.s` — 2 métodos, 2218 bytes.** Exemplo: `kairo.unity.ui.Canvas::OnInputUpdate()->System.Void`.

Tentativas compiladas no grupo na rodada 9: nenhum. Histórico 7/8 compatível: UNKNOWN; não se presume compatibilidade após alteração do fonte. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 475.

| IL original | IL recompilado |
| --- | --- |
| `IL_0006: brfalse.s L0` | `IL_0006: brtrue.s R30` |
| `IL_0008: ret` | ```IL_0008: call class UnityEngine.Input::get_touchCount``0()->valuetype System.Int32;this=False;explicit=False;cc=Default``` |

**KairoLibrary #3: `stloc.s => DELETION_POINT` — 1 métodos, 1928 bytes.** Exemplo: `kairo.unity.ui.ResourceManager/<>c__DisplayClass51_0::<LoadStart>b__0()->System.Void`.

Tentativas compiladas no grupo na rodada 9: nenhum. Histórico 7/8 compatível: UNKNOWN; não se presume compatibilidade após alteração do fonte. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 285.

| IL original | IL recompilado |
| --- | --- |
| `IL_01f7: stloc.s 13` | `IL_01f7: br.s L18` |
| `IL_01f9: br.s L23` | `IL_01f9: ldloc.s 18` |

**KairoLibrary #4: `brtrue.s => ldc.i4.4` — 1 métodos, 1054 bytes.** Exemplo: `kairo.unity.io.HttpObject::_connect(System.String,System.Byte[],System.Boolean,System.Boolean)->System.Byte[][]`.

Tentativas compiladas no grupo na rodada 9: nenhum. Histórico 7/8 compatível: UNKNOWN; não se presume compatibilidade após alteração do fonte. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 114.

| IL original | IL recompilado |
| --- | --- |
| `IL_00c8: brtrue.s O222` | `IL_00c8: ldc.i4.4` |
| `IL_00ca: ldc.i4.1` | `IL_00c9: bgt.s R215` |

**KairoLibrary #5: `stloc.2 => stloc.1` — 1 métodos, 944 bytes.** Exemplo: `kairo.unity.io.HttpObject/<>c__DisplayClass10_0::<_connect>b__2()->System.Void`.

Tentativas compiladas no grupo na rodada 9: nenhum. Histórico 7/8 compatível: UNKNOWN; não se presume compatibilidade após alteração do fonte. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 19.

| IL original | IL recompilado |
| --- | --- |
| `IL_00da: stloc.2` | `IL_00da: stloc.1` |
| `IL_00db: ldarg.0` | `IL_00db: ldarg.0` |

**Assembly-CSharp #1: `brfalse => brtrue` — 5 métodos, 13329 bytes.** Exemplo: `form.SpForm::.ctor(System.Exception)->System.Void`.

Tentativas compiladas no grupo na rodada 9: nenhum. Histórico 7/8 compatível: UNKNOWN; não se presume compatibilidade após alteração do fonte. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 2024.

| IL original | IL recompilado |
| --- | --- |
| `IL_000f: brfalse L8` | `IL_000f: brtrue L0` |
| `IL_0014: ldsfld class kairo.common.cfg.Config::PTW:valuetype System.Boolean` | `IL_0014: ret` |

**Assembly-CSharp #2: `br => ret` — 2 métodos, 12896 bytes.** Exemplo: `form.SubForm::_draw4(kairo.unity.ui.Graphics)->System.Void`.

Tentativas compiladas no grupo na rodada 9: nenhum. Histórico 7/8 compatível: UNKNOWN; não se presume compatibilidade após alteração do fonte. Antes de compilar: wrong_signature: 1; orçamento recusou 0; repertório anterior filtrou 832.

| IL original | IL recompilado |
| --- | --- |
| `IL_1077: br L156` | `IL_1077: ret` |
| `IL_107c: ldarg.0` | `IL_1078: ldarg.0` |

**Assembly-CSharp #3: `INSERTION_POINT => stloc.1` — 1 métodos, 12082 bytes.** Exemplo: `form.SubForm::Update2()->System.Boolean`.

Tentativas compiladas no grupo na rodada 9: nenhum. Histórico 7/8 compatível: UNKNOWN; não se presume compatibilidade após alteração do fonte. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 11792.

| IL original | IL recompilado |
| --- | --- |
| `IL_0029: brfalse L0` | `IL_0029: stloc.1` |
| `IL_002e: ldarg.0` | `IL_002a: ldloc.1` |

**Assembly-CSharp #4: `stloc.1 => stloc.s` — 2 métodos, 10828 bytes.** Exemplo: `game.Player::Script()->System.Boolean`.

Tentativas compiladas no grupo na rodada 9: nenhum. Histórico 7/8 compatível: UNKNOWN; não se presume compatibilidade após alteração do fonte. Antes de compilar: nenhum; orçamento recusou 0; repertório anterior filtrou 11286.

| IL original | IL recompilado |
| --- | --- |
| `IL_0016: stloc.1` | `IL_0016: stloc.s 4` |
| `IL_0017: ldloc.1` | `IL_0018: ldloc.s 4` |

**Assembly-CSharp #5: `bne.un => beq` — 11 métodos, 10420 bytes.** Exemplo: `form.SubForm::Init2()->System.Boolean`.

Tentativas compiladas no grupo na rodada 9: condition-split-or-guards: 1, initialized-declaration-swap: 4, local-declaration-scope-hoist: 5, smcs-empty-arm-restore: 1, solver-group-all-calculated-blocks: 1, solver-group-block-declarations: 3, terminal-guard-positive-tail: 1, terminal-if-arm-return-flatten-else: 1. Antes de compilar: wrong_signature: 4; orçamento recusou 0; repertório anterior filtrou 6122.

| IL original | IL recompilado |
| --- | --- |
| `IL_0053: bne.un L4` | `IL_0053: beq L3` |
| `IL_0058: ldc.i4.1` | `IL_0058: br L4` |

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

Nenhuma instalação, arquivo ou decisão de escopo exigida para esta rodada. Métodos restantes continuam divergentes; a parada segue o limite autorizado.

Parada após 40 ciclos autônomos D e 11 etapas A-C; sequência autônoma final sem novo matched: 0. Limites configurados: 40 ciclos autônomos, 5 sem ganho consecutivos com fila vazia; cota Kairo: at least one KairoLibrary cycle in every consecutive block of five cycles.

## 8. SUA RECOMENDAÇÃO

1. Testar a composição já preparada de SetWinCharaTraining (412 bytes), após fixture própria e modelo novo. Limite de ganho: 0..1 método; a proposta ainda não foi compilada no jogo. É o menor custo de preparação restante identificado.
2. Implementar e validar o vínculo AST de inicializador estático com o stsfld de CGI_COMMON_QUERY em AppData..cctor (1816 bytes), sem construtor manual. Depois testar o ternário proposto. Limite: 0..1 método; a rota atual não tem mapeamento, e a expressão ainda não foi compilada.
3. Resolver os quatro limites de declarações/escopos de SubForm._draw4 (12484 bytes: locais43..49,67..69 com zerochain,91/92 e cache93/maxstack) antes de propor outra fonte integral. Fechamento esperado: UNKNOWN; limite0..1 método, pois os29 hunks e93/94 locais ainda não têm composição certificada. O grupo br=>ret contém2 métodos/12896 bytes, incluindo o SetWin acima; não somar as estimativas como alvos disjuntos.
