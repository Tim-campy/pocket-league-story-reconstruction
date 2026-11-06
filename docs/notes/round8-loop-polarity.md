# Round8: classificação de polaridade dos 26 pontos pendentes

Baseline: commit `25b8108`, dumps originais/recompilados cycle20-after-verified, com hashes vinculados no JSON. Zero compilações do jogo. Apenas a primeira divergência alinhada de cada método foi medida; não é um censo de todos os desvios do corpo.

## Resultado medido

| Par | Métodos / IL | Header selecionado | Backedge dominador | Saída tomada original | Fica no laço, fallthrough sai | Condicional interna | Fora do laço | UNKNOWN |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| brfalse → brtrue | 11 / 31.885 | 0 | 0 | 0 | 1 / 4.343 | 3 / 2.707 | 7 / 24.835 | 0 |
| bne.un → beq | 15 / 21.664 | 0 | 0 | 0 | 1 / 3.844 | 3 / 3.037 | 11 / 14.783 | 0 |

Nos recompilados, as duas arestas de fronteira tornam-se saídas tomadas de seus laços. As seis internas continuam internas e as dezoito externas continuam externas. Todas as 26 originais estão em blocos alcançáveis pela entrada normal; nenhum ponto está em handler ou SCC irreducível. O algoritmo marca esses casos UNKNOWN em controles próprios. Transferências excepcionais não são inventadas: os números provam funções no CFG normal, e a classificação completa com arestas excepcionais permanece UNKNOWN.

REFUTADA nestes 26 pontos: tratar a polaridade como evidência de conversão while/do/for. Nenhum ponto selecionado é o teste/header do laço. Não foi criada transformação nova de laço nem renomeada regra conhecida.

## Método e limites do CFG

Blocos começam em entrada, destinos, instruções seguintes a branches/terminadores e fronteiras EH. Arestas normais mantêm destinos reais. Dominadores são calculados por ponto fixo a partir da entrada; backedge é s→h somente se h domina s. Laços naturais são fechados pelos predecessores e união por header. SCCs sem header dominador são irreducíveis e recebem UNKNOWN quando envolvem o ponto. Sair de um laço não é backedge. Um if dentro do for pode testar uma condição e sair por break sem ser o teste do for.

Fixture smcs independente confirmou 10 controles, incluindo catch/SCC irreducível UNKNOWN. While e for sem inicializador interno geraram corpos inteiros idênticos: a distinção de sintaxe é UNKNOWN só a partir desse IL. O teste de while/for é emitido no fundo e é header de dominadores por causa do salto de entrada; seu branch pode ir numericamente para trás sem ser backedge de dominadores. Do sem salto de entrada produziu backedge verdadeiro.

## Causas sustentadas por fonte e IL, por método

As formas abaixo são diagnósticos sustentados pelos trechos reais e pelos pontos MDB. Só o verificador pode confirmar uma correção de jogo. Todos os 26 corpos debug reutilizados são integralmente iguais aos corpos aceitos sem debug. Os históricos exatos de tentativas por ciclo/família/outcome estão no JSON, inclusive falhas; ausência em C20 não significa que uma família nunca foi testada.

| Método | IL | Função CFG original | Fonte recuperado / forma indicada pelo IL | Histórico mais recente |
|---|---:|---|---|---|
| `form.SubForm::Init()->System.Boolean` | 5191 | NON_LOOP_CONDITIONAL | Original empty type_==6 arm before remainder; recovered type_!=6 guard omits the empty arm/join branch. | C20: no_model_approved_candidate |
| `form.SubForm::Init2()->System.Boolean` | 3844 | LOOP_BOUNDARY_STAY_EDGE | Inside for: original equality if sets flag then breaks; recovered inequality continue followed by flag/break. Separate switch temp is in wrong slot. | C20: no_model_approved_candidate |
| `form.SpForm::Update()->System.Boolean` | 2052 | NON_LOOP_CONDITIONAL | Original num2==-3 body precedes alternative dispatch; recovered num2!=-3 switch reverses block order. | C20: no_model_approved_candidate |
| `game.GameManager::CollisionBall_proc()->System.Void` | 1970 | LOOP_INTERNAL_CONDITIONAL | Original coliBallState==4 explicit continue before range tests; recovered state!=4 AND range conditions merges guard with body test. | C20: no_model_approved_candidate |
| `game.TGroupArea::SetPosData()->System.Void` | 1810 | NON_LOOP_CONDITIONAL | Original positive tMode==3 guarded tail; recovered tMode!=3 early return adds ret. | C20: no_model_approved_candidate |
| `game.TGroupArea::ChangeState(System.Int32)->System.Void` | 1709 | NON_LOOP_CONDITIONAL | Empty tMode comparison polarity differs; original bne.un targets immediately following br. | C20: no_model_approved_candidate |
| `game.Team::Update()->System.Void` | 1489 | NON_LOOP_CONDITIONAL | Original separate empty gameState alternatives; recovered OR grouping changes branch/join layout. | C20: no_model_approved_candidate |
| `game.Player::GetToThrowInPID()->System.Int32` | 809 | LOOP_INTERNAL_CONDITIONAL | Original independent address/equality guard and line check; recovered OR continue merges guard boundaries. | C20: no_model_approved_candidate |
| `system.form.FriendForm::Init()->System.Boolean` | 710 | NON_LOOP_CONDITIONAL | Original separate empty type13/type15 alternatives; recovered OR grouping changes branch/join layout. | C20: no_model_approved_candidate |
| `system.form.BillingForm::Init()->System.Boolean` | 654 | NON_LOOP_CONDITIONAL | Empty type_==4 original test targets next instruction; recovered type_!=4 has opposite opcode. | C20: no_model_approved_candidate |
| `game.TGroupArea::GetBall(game.TPlayer,System.Int32)->game.TBall` | 487 | NON_LOOP_CONDITIONAL | Original empty tMode==4 arm then tMode==3 branch; recovered tMode!=4 && tMode==3 merges them. | C20: no_model_approved_candidate |
| `surface.TouchEffect::Draw(kairo.unity.ui.Graphics)->System.Void` | 349 | NON_LOOP_CONDITIONAL | Original separate empty id2/id3 alternatives; recovered OR grouping changes branch/join layout. | C20: no_model_approved_candidate |
| `game.Player::ChMoveDirect(System.Int32,System.Int32)->System.Boolean` | 258 | LOOP_INTERNAL_CONDITIONAL | Original separate moveDirectBox[i]==-1 guard then direction check; recovered combined OR continue fuses comparisons. | C20: no_model_approved_candidate |
| `system.form.MatchForm::_draw(kairo.unity.ui.Graphics)->System.Void` | 251 | NON_LOOP_CONDITIONAL | Empty type_==1 original test targets next instruction; recovered type_!=1 has opposite opcode. | C20: no_model_approved_candidate |
| `system.form.LanguageForm::OnTouchEvent(kairo.unity.surface.TouchEvent)->System.Boolean` | 81 | NON_LOOP_CONDITIONAL | Original positive component.value equality guards body directly; recovered negative empty arm plus else adds explicit skip branch. | C20: no_model_approved_candidate |
| `form.SubForm::Update()->System.Boolean` | 11390 | NON_LOOP_CONDITIONAL | Original Jp true/false blocks each stloc.3; recovered reversed ternary shares one stloc after its join. | C20: no_model_approved_candidate |
| `form.SubForm::_draw(kairo.unity.ui.Graphics)->System.Void` | 8704 | NON_LOOP_CONDITIONAL | Original explicit empty IsDrawWindow true arm plus else body; recovered only if(!IsDrawWindow) omits long join branch. | C20: no_model_approved_candidate |
| `game.Player::Act_dribble()->System.Void` | 4343 | LOOP_BOUNDARY_STAY_EDGE | Inside for: original positive ChOnLine if assigns flag/array then breaks; recovered negative continue followed by those statements. | C20: no_model_approved_candidate |
| `surface.GameView::_addTouch(kairo.unity.ui.Graphics,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,kairo.unity.surface.TouchOption)->kairo.unity.surface.TouchComponent` | 3554 | NON_LOOP_CONDITIONAL | Original array-nonnull guarded body; recovered array-null goto dispatchExit adds an explicit transfer. | C20: no_model_approved_candidate |
| `form.GameForm::_update(System.Int32)->System.Boolean` | 1687 | LOOP_INTERNAL_CONDITIONAL | Compound OR continue guard fused independent original guards; original CheckAlreadyEvent brfalse skips an explicit br to increment. | C20: no_model_approved_candidate |
| `system.billing.ui.BillingManager::RegistApplyTask(System.Boolean)->System.Boolean` | 629 | LOOP_INTERNAL_CONDITIONAL | Independent flag3/Consume recovery guards have been combined into an OR continue; original first test skips explicit continue branch. | C20: no_model_approved_candidate |
| `form.SpForm::AddCommand(System.Int32,System.String)->System.Void` | 572 | NON_LOOP_CONDITIONAL | Ternary bool selector polarity reversed (flag?6:2 versus !flag?2:6); value arms identical in meaning. | C20: no_model_approved_candidate |
| `system.surface.GameBanner::Load()->System.Void` | 391 | LOOP_INTERNAL_CONDITIONAL | Original independent guarded branches to loop increment; recovered compound OR plus recoveredCondition local fuses their control flow. | C20: no_model_approved_candidate |
| `form.SpForm::.ctor(System.Exception)->System.Void` | 347 | NON_LOOP_CONDITIONAL | Original non-null positive body guarded to final ret; recovered null early-return adds ret and reverses the test. | C20: no_model_approved_candidate |
| `system.billing.MyBilling::Cancel(system.billing.ui.ApplyTask)->System.Void` | 155 | NON_LOOP_CONDITIONAL | Empty terminal ticket comparison has opposite polarity; original conditional targets next instruction. | C20: no_model_approved_candidate |
| `system.billing.MyBilling::Provisioning(system.billing.ui.ApplyTask)->System.Void` | 113 | NON_LOOP_CONDITIONAL | Empty terminal ticket comparison has opposite polarity; original conditional targets next instruction. | C20: no_model_approved_candidate |

## Três exemplos e tentativas negativas relevantes

1. Player.Act_dribble (4.343 IL): original IL1647 `brfalse 1671`, depois flag/array e break; recuperado `brtrue 1657; br1676` no `if(!ChOnLine)continue`. O header do for interno é IL1677, não IL1647. Forma sustentada: `if(ChOnLine){flag=true;...;break;}` dentro do mesmo for. A família v7 `smcs-continue-positive-tail` já fez exatamente isso em C17, no ponto MDB correto, com assinatura aprovada: o prefixo regrediu **1538→1258**. Resultado negativo conhecido; nenhum retry é proposto sem outra composição comprovada.

2. SubForm.Init2 (3.844 IL): original IL83 `bne.un95; ldc1; stloc1; br121`, recuperado `beq93; br100; ldc1;stloc1;br126`. Header do for interno IL99. Forma sustentada: `if(ids==){flag=true;break;}`; a desigualdade/continue recuperada adiciona br. C17 bloqueou a família pela assinatura; o mesmo bloqueio foi reproduzido somente no modelo nesta análise. O slot Int32 original32 é um seletor nomeado antes de LeagueData; o recuperado usa `$switch` de emissão67. A composição calculada abaixo libera o gate; IL/prefixo do jogo continuam UNKNOWN.

3. SubForm.Update (11.390 IL): original IL767 `brfalse780;104;stloc3;br783;124;stloc3`; recuperado `brtrue779;124;br781;104;stloc3`. É atribuição condicional fora de laço: forma `if(Jp()){num3=104;}else{num3=124;}` versus ternário invertido com store único. Não é mudança while/do/for. SubForm._draw (8.704 IL), outro caso grande fora de laço, tem `brfalse97;br205` versus `brtrue200`, compatível com true vazio/else e já coberto pela família conhecida de braço vazio.

## Composição calculada para Init2

Original: IL1853 GetMatchInfoJoin;1858 stloc32;1860 ldloc32;brtrue1921;1921 ldloc32;1923 ldc1;bne.un1983. Recuperado: o switch faz a mesma chamada e guarda num temporário de emissão67. O modelo tem 68 slots; mover esse único Int3267 para32 reproduz toda a assinatura original. O seletor é avaliado uma vez em local nomeado e o switch de dois cases com return vira if/else-if, preservando blocos dos cases. A composição com o if positivo/break continua completa e idêntica nos 68 tipos. Fonte não foi alterado; candidato de jogo não foi compilado.

Opt-in `calculated-selector-continue-composition`, entry `AddRound8SelectorContinueComposition`. Hints calculam prova de relocação exata e stloc/ldloc/call observados, não aceitam um rótulo manual de matched. A extensão exige call de retorno Int32, dois cases0/1 com blocos separados terminados em return/throw, continue pertencente ao loop cujo corpo termina em break e nenhuma captura/goto/yield. Campo volátil, captura, variável compartilhada entre cases e break de case são rejeitados.

Fixture: 8 controles de sintaxe, dois corpos toy completos iguais, 64 cenários em cada uma de três execuções; seletor com efeito lateral é chamado uma vez; float inclui NaN e .un. O primeiro harness com arrays jagged falhou em Program.Main antes das comparações com ArrayTypeMismatchException; sua causa permanece UNKNOWN. O harness final usa argumentos de arrays simples. A falha está registrada no JSON, não é tratada como refutação da composição.

## Evidência do compilador

Arquivo pinned `analysis/tools/round5-mono-source/pre-release/mcs/mcs/statement.cs`, revisão Unity-Technologies/mono `b6587926c0f146c8b12985744f33a13b39a22d57`. A revisão exata do fonte que fabricou o smcs instalado permanece UNKNOWN; a emissão relevante foi confirmada nas fixtures com o executável instalado.

* [While.DoEmit421–467](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/statement.cs#L421-L467): não constante emite Br LoopBegin, corpo, marca LoopBegin e EmitBranchable(while_loop,true). True constante emite corpo e Br LoopBegin.
* [Do.DoEmit322–351](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/statement.cs#L322-L351): marca loop, corpo, marca LoopBegin, teste verdadeiro ao loop.
* [For.DoEmit568–619](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/statement.cs#L568-L619): Br test, corpo, LoopBegin/incremento, test/EmitBranchable(loop,true).
* [Switch.DoEmit3831–3844](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/statement.cs#L3831-L3844): cria LocalTemporary e Store durante emissão, mesmo para variável não capturada (TODO explícito no fonte). Isso explica por que só nomear o seletor mantendo switch ainda não resolve o temp.

## Estado

Diagnóstico de 26 pontos e composição fonte/modelo concluídos. Zero métodos ou bytes adotados por esta análise, zero compilações do jogo. Nada em src, ledger, configuração, verificador, engine ou helpers frozen foi modificado. O bundle calculado deve ser refeito/validado contra o hash de fonte corrente antes de uso pelo motor.
