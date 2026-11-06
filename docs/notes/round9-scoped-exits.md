# Saídas estruturadas do smcs: duas famílias novas para a fase D

Estado: fixtures confirmadas; **nenhum método do jogo foi compilado ou promovido a matched por este trabalho**. Snapshot somente leitura: `build/round9/cycles/b10/after/Assembly-CSharp`. A API CIL da fase C permaneceu congelada.

## Alvos e cobertura da preparação

| Família | Métodos/peso considerados | Candidatas com modelo completo e assinatura original | Recusas por assinatura da candidata |
|---|---:|---:|---|
| `terminal-if-arm-return-flatten-else` | 7 / 12.447 bytes IL | 6 / 9.993 bytes IL | `Camera.Update_pos`: previsão completa com 15 slots, original 19 |
| `scoped-terminal-arm-common-join` | 7 / 8.046 bytes IL | 6 / 7.634 bytes IL | `SubForm.SetWinCharaTraining`: previsão completa com 6 slots, original 5 |

Isso mede assinatura e possibilidade de produzir um candidato; **não mede ganhos em corpos**. As duas candidatas recusadas não entram nos bundles de fallback. As previsões completas descrevem as candidatas, não uma suposta assinatura correta diferente do original.

Alvos aprovados da primeira família: `system.form.FriendForm::_draw` (6.994), `game.Player::Act_receive` (1.324), `system.form.BillingForm::_draw` (1.007), `game.Player::Act_charge` (271), `game.Camera::DrawCursor` (207), `surface.GamePad::DrawVersion` (190).

Alvos aprovados da segunda: `form.TitleForm::_draw` (3.603), `game.Player::Act_gk` (1.982), `game.Team::Update_coatEvaNewDeffence` (716), `game.UserData::Update_bus` (605), `game.TGroupArea::Draw_obj` (570), `form.SubForm::Draw_up` (158).

Os IDs completos, pontos MDB, assinaturas previstas/originais e candidatos estão no recibo de preflight. `GameForm._update` e `SubForm.Init` foram excluídos desta frente conforme a divisão de trabalho.

## Família 1: ret original contra br recompilado

Forma recuperada: `if (c) { A; } else { B; }` no fim real do método. Forma candidata: `if (c) { A; return; } { B; }`. O bloco lexical do else é preservado. Se o if estiver embutido em um else-if, a substituição inteira ganha um bloco externo para manter o binding. Nenhuma condição é negada, duplicada ou reordenada.

Exemplos de evidência:

| Método | Original | Recompilado |
|---|---|---|
| `Camera.DrawCursor` | IL6 `brtrue 12`; IL11 `ret`; IL12 carrega cursorCnt | IL6 `brtrue 16`; IL11 `br` para a saída; IL16 carrega cursorCnt |
| `FriendForm._draw` | IL62 `brfalse 74`; IL67 `ldarg.1`; IL68 `call DrawConnect`; IL73 `ret` | Mesmo trabalho no then, seguido de IL73 `br`, com o próximo else começando em IL78 |
| `BillingForm._draw` | Depois do try/catch, IL275 `leave 280`; IL280 `ret`; IL281 começa teste type | IL275 `leave 280`; IL280 `br`; IL285 começa teste type |

O opcode ret prova saída do método, independentemente do destino do br recuperado. O helper só acrescenta return a um braço que atualmente pode cair e exige caminho lexical terminal até o corpo do método; não atravessa um laço ou região protegida externa. O try/catch contido inteiramente no braço de BillingForm permanece intacto; o return fica depois dele.

Novidade em relação às rodadas 7/8: é a inversa específica da restauração de braço vazio/common-exit. O repertório anterior retirava return ou criava braço vazio; não acrescentava return a um braço que ainda não termina nem retirava seu else. A regra geral guard-return exigia um return já existente, portanto não alcançava estes casos. Em R7/C6, `Camera.DrawCursor` adotou apenas avanço parcial com empty-arm-first-and (prefixo bruto 6→7, corpo não igual); esta família resolve a próxima descontinuidade, em vez de repetir aquela candidata.

## Família 2: br original contra ret recompilado

Forma recuperada: `if (c) { A; return; } B;`. Forma candidata: `if (c) { A; } else { B; }`, conservando o operador e a ordem dos comandos. O fim de B é limitado pelo destino original provado. Há dois casos aceitos: ret final original e caminho terminal do bloco até o método; ou próximo comando após um if sem else que contém o bloco atual.

| Método | Original | Recompilado |
|---|---|---|
| `Team.Update_coatEvaNewDeffence` | IL594 `bne.un 604`; IL599 `br 626`; IL604 chama Cal_evaCoatMulti9block; IL626 começa ch_ceva[8] | IL594 mantém bne.un; IL599 `ret`; o código ch_ceva[8] continua existindo depois |
| `TitleForm._draw` | IL1122 `brfalse 1142`; IL1132 `brfalse 1142`; IL1137 `br 1275`; IL1142 desenha ícones; IL1275 começa próximo if no mesmo laço | Mesmos dois testes, mas IL1137 `ret` |
| `SubForm.Draw_up` | Depois do primeiro for, IL84 `br 157`; IL89 nega cnt e começa o segundo for; IL157 `ret` | Depois do primeiro for, IL84 `ret` |

Os joins locais foram medidos: Team O626↔R622, label L28, MDB linha1134; Title O1275↔R1271, label L11, MDB linha589. Eles retomam o próximo comando após o if envolvente; não são saída do método nem incremento do laço.

Novidade: o helper R7TerminalArm exigia ações retas antes do return e cauda real do método; rejeitava loops e ifs no braço. R7SmcsExit exigia OnlyReturn e método terminal. Esta família acrescenta joins locais provados e braços não lineares. Candidatos simples method-ret já cobertos na v7 são explicitamente recusados, incluindo `_draw4`; eles não reaparecem sob um nome novo. Em R7/ParteA, Team e Title adotaram tail-early-return como avanço de instrução alinhada, mantendo prefixo bruto 472→472 e855→855 respectivamente; ambos continuaram sem corpo igual. As tentativas empty-arm posteriores não eram a transformação atual de mesmo operador com join local.

## Emissão e limites

Fonte fixada: Unity-Technologies/mono `b6587926c0f146c8b12985744f33a13b39a22d57`, `mcs/mcs/statement.cs:247-264`: o if emite a condição para false, o braço true e um `br` para o join quando existe else e o braço true pode concluir. `statement.cs:770-782`: return normal emite ret; protegido emite temporário e leave. SHA de statement.cs: `063a437095e1d608d045383d76291c9359e9a98220eea7750018a3d42712437a`. A revisão exata que fabricou o smcs instalado continua UNKNOWN; os moldes foram exercitados com seu executável SHA `26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0`.

Nenhuma família reescreve a condição. Operadores customizados e comparações float, inclusive `.un`, preservam avaliação, polaridade e NaN. Gotos, rótulos, capturas, funções locais, yield e fixed na região editada são recusados. Casos sem prova de join ou ownership lexical continuam UNKNOWN. O hint CIL sozinho não completa uma árvore nem dispensa o verificador.

## API dinâmica e integração

Adicionar somente `Round9ScopedExitCandidates.cs` e `Round9ScopedExitAdapter.cs` ao conjunto do syntax helper. O driver de fixture não entra no helper ativo. Chamada:

```csharp
AddRound9ScopedExitRules(input,text,mapped,methodBody,model,point,patches,seen);
```

API Python:

```python
scoped_exit_hints(original, rebuilt, alignment, symbol_method, source_text,
                  source_identity_confirmed=True)
```

O default de source_identity_confirmed é False. O chamador só o confirma depois de vincular fonte atual, método aceito e geração MDB. A função também confere, pelo comparador estrito, o corpo inteiro debug+ contra o debug- atual. Retorna `round9_scoped_exit_valid`, `round9_scoped_exit_original_opcode`, `round9_scoped_exit_join_kind`, `round9_scoped_exit_join_source_point` (UTF16), `round9_scoped_exit_mapping_rebuilt_offset` e evidência. Para ret=>br, usa como âncora MDB a instrução anterior ao join sintético, evitando mapear o próximo else. Para join local, exige label alinhado único, identidade exata da instrução alvo e ponto MDB; o helper ainda exige que o ponto seja o próximo comando após o if lexical envolvente.

Integrar depois de obter symbol_method e texto atual, antes da chamada syntax.request: calcular hints, atualizar o point usando mapping_rebuilt_offset quando valid e juntar os campos aos hints da requisição. Registrar a evidência; não alterar alignment nem o prefixo usado pelo verificador. O adapter aceita a família selecionada ou já aprendida, permitindo novas ocorrências após avanços parciais. A previsão de slots deve ser recalculada para cada fonte candidata antes de compilá-la.

Os bundles externos são fallback de uma ocorrência calculada por método, em declaração inteira com rebase único e CRLF preservado. Usar --external-proposals e --cycle-rule com o nome da família. O produtor expõe --snapshot, --measurement e --symbols para recalcular num snapshot posterior. O modo dinâmico é necessário para iterar ocorrências seguintes sem depender do before congelado.

## Verificações e tentativas negativas

Fixture independente: **10 corpos completos literalmente iguais** aos brinquedos esperados, **10 recusas**. Inclui braço vazio, ações, if/loop interno, break/continue, operador true customizado, float, else-if embutido e EH concluído antes do return. Os negativos cobrem método não void, if não terminal, labels/capturas, hint de opcode errado, join/source-boundary errado, leave no lugar de br e os dois padrões simples já cobertos na v7.

Hints frescos somente leitura: **15 pares reais**, com corpo MDB igual; **14 métodos com candidatos novos** e `_draw4` recusado pela política de não repetição. Quatro recusas adicionais: fonte não vinculada, corpo MDB alterado, pontos ausentes e label de join não provado. O adapter compilou isoladamente com um stub da forma de Patch; não emitiu código do jogo.

Falhas iniciais de harness: o perfil Unity não tinha System.Action, substituído por ThreadStart no negativo de captura; o localizador de método assumia void e não achou o negativo int. Ambas foram corrigidas e asserções executadas novamente. A primeira preparação de bundle normalizava CRLF na leitura; antes da publicação foi corrigida para decode dos bytes, e todos os **12 rebases integrais exatos** foram comprovados com o rebaser do engine. Nenhuma dessas tentativas compilou um assembly do jogo, editou seu fonte ou alterou estado.
