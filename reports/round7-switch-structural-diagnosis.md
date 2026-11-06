# Switch recuperado versus cadeia de if: domínio ainda não esgotado

Base medida: `reports/round7-cycle07-after-measurement.json`, Assembly-CSharp.
O par `brtrue => stloc.s` representa **11 métodos / 19.865 bytes de IL**.
Todos os 11 originais possuem **zero opcodes switch**. No ponto de divergência,
o fonte recuperado dos 11 usa um switch C#. O smcs guarda seu seletor num
temporário mesmo quando baixa um switch pequeno para comparações; portanto
ausência do opcode switch, isoladamente, não prova ausência do comando C#.

## Exemplos

### SoccerForm._update: 4.742 bytes

ID: `form.SoccerForm::_update(System.Int32)->System.Boolean`.

```text
Original                       Recompilado
IL1121 ldloc.s 6               IL1121 ldloc.s 6
IL1123 brtrue 1171             IL1123 stloc.s 41
IL1128 call IsVictoryLeague    IL1125 ldloc.s 41
IL1133 brfalse 1166            IL1127 brfalse 1145
```

O fonte tem `switch (matchInfoJoin)` com casos 0 e 1. O caso 1 é um bloco
explícito contendo uma declaração de QuestMatchData e break terminal. A
forma candidata é `if (matchInfoJoin == 0) { ... } else if (matchInfoJoin == 1)
{ ... }`, removendo somente o break pertencente ao switch.

O original tem **43 slots**, contra **42** recompilados. Eliminar o temporário
de seletor sozinho reduz para 41; há outras duas posições de locais a recuperar.
Não foi pressuposta aprovação do modelo para esse candidato.

### Player.SetDrawParam: 3.587 bytes

ID: `game.Player::SetDrawParam(System.Int32,System.Int32,System.Int32,System.Int32)->System.Int32`.

```text
Original                       Recompilado
IL0282 ldarg.3                 IL0282 ldarg.3
IL0283 brtrue 516              IL0283 stloc.s 13
IL0288 ldsfld body_off         IL0285 ldloc.s 13
IL0293 ldarg.3                 IL0287 switch (...)
```

O fonte usa `switch (seb)`, começando com case 0 e acessos `body_off[seb]`.
É um switch com mais de oito seções. Os **13 slots originais** são prefixo
exato dos **14 recompilados**, com um Int32 adicional ao final. Esse é o maior
alvo cuja falta de locais é compatível apenas com o temporário do seletor.

### FanBonusData.ChBonusEvent: 2.605 bytes

ID: `data.FanBonusData::ChBonusEvent()->System.Void`.

```text
Original                       Recompilado
IL0081 ldloc.s 8               IL0081 ldloc.s 8
IL0083 brtrue 271              IL0083 stloc.s 64
IL0088 ldloc.1                 IL0085 ldloc.s 64
IL0089 ldfld UserData.gm_      IL0087 switch (...)
```

O fonte contém `switch (num)` com muitos cases e laços internos que usam break.
Esses breaks saem de seus laços e precisam permanecer. Há **65 slots** em ambos
os lados, mas tipos e posições diferem. A conversão do switch, sozinha, não
resolve a assinatura desse método.

## Por que a regra antiga não cobre esses casos

`tools/Round7TransformSyntax.cs::switch-if` contém um limite explícito de oito
seções. SetDrawParam, FanBonusData.ChBonusEvent, Player.Act e
PlayerData.SetGameParamData ultrapassam esse limite e não recebem propostas.

A regra também só retira break quando ele é o último Statement diretamente
da seção. Em `case X: { commands; break; }`, o último Statement é o bloco;
seu break permanece. O resultado, colocado num if, pode ficar inválido ou sair
de um laço externo. Outro filtro rejeita qualquer break em statements
intermediários, inclusive o break de um for interno que deve ser preservado.
Esses limites são evidência no código da ferramenta; não se alegou que uma
compilação candidata desses métodos tivesse falhado nesta investigação.

## Nova família: dono de cada transferência de controle

`tools/Round7SmcsSwitchRules.cs` oferece a entrada
`AddRound7SmcsSwitchRules`, com a assinatura padrão. A seleção é
`smcs-owned-switch-if`, em `round7_cycle_rule` ou na lista explícita de famílias
aprendidas `round7_cycle_rules`. Sem ambas, não gera propostas.

A transformação não tem limite de oito seções. Para cada break, identifica
o primeiro ancestral for/while/do/foreach/switch:

- Break terminal cujo dono é o switch convertido é removido, inclusive dentro
  de bloco explícito de case.
- Break antecipado do switch vira goto para um rótulo comum após a cadeia.
  Não é introduzido um laço artificial.
- Breaks de laços ou switches internos permanecem intactos.
- Continue de laço interno permanece; continue cujo dono está fora do switch
  é rejeitado conservadoramente.
- Labels de case adjacentes, sem comandos entre eles, compartilham um ramo OR.
  `goto case/default` deste switch e locais usados entre seções são rejeitados.

O seletor precisa ser parâmetro, local ou campo estável não volátil. Chamadas,
propriedades e outros receptores com possível efeito colateral são rejeitados.
Não é criada uma variável nova para repetir ou guardar uma avaliação impura.

Há uma variante adicional para Team.SetGoalState: o original tem
`ldloc.0; brtrue 336; br 547`, mas o switch recuperado só possui casos 1–4.
A forma de if precisa restaurar o caso 0 vazio. Isso exige
`original_empty_zero_arm: true`, inexistência de case 0 e inexistência de default.
O hint vem de `tools/round7_switch_hints.py::original_empty_zero_arm_hint`:
o `brtrue` original pula exatamente sobre um `br` que salta para depois do
começo do ramo não zero. Um padrão ambíguo não fornece o hint.

## Prioridade por locais, medida separada do peso total

O modelo publicado em `reports/round7-slot-model-validation.json` está completo
em **11/11** métodos deste grupo. Assinaturas originais/recompiladas dos dumps:

| Método | Slots originais | Recompilados | Prefixo original idêntico |
|---|---:|---:|---|
| Player.SetDrawParam | 13 | 14 | Sim |
| GameForm.ProgressTime | 30 | 31 | Sim |
| SoccerForm.ChangeState | 34 | 35 | Sim |
| Player.Act | 11 | 12 | Sim |
| Team.SetGoalState | 5 | 6 | Sim |
| GameForm.Draw_topBarParts | 6 | 7 | Sim |
| BootForm.UpdateScoreFromNetwork | 7 | 8 | Sim |
| PlayerData.SetGameParamData | 4 | 5 | Sim |
| SoccerForm._update | 43 | 42 | Não |
| FanBonusData.ChBonusEvent | 65 | 65 | Não |
| SoccerForm.Draw_topBarParts | 10 | 12 | Sim, mas sobram dois slots |

Os oito primeiros somam **11.825 bytes**; os outros três, **8.040**. A previsão
do fonte transformado e a aprovação exata da assinatura original permanecem
**UNKNOWN** até o motor executar o modelo antes de qualquer build candidato.
Não se usa apenas igualdade da quantidade de locais como critério.

## Fonte do compilador

Snapshot Unity Mono `b6587926c0f146c8b12985744f33a13b39a22d57`:

- [Switch.DoEmit, statement.cs 3823–3878](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/statement.cs#L3823-L3878) cria `LocalTemporary`, emite o seletor e o guarda mesmo sendo uma referência a variável; depois substitui e restaura `ec.LoopEnd` para o switch.
- [Break.DoEmit, statement.cs 1117–1120](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/statement.cs#L1117-L1120) usa esse `LoopEnd` e emite br/leave.
- [If.DoEmit, statement.cs 224–272](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/statement.cs#L224-L272) explica a cadeia de comparações e o desvio compartilhado, sem temporário do seletor.

## Fixtures e limites confirmados

Reprodução: `python reports/round7-switch-structural-fixture-driver.py`.
O relatório `reports/round7-switch-structural-fixtures.json` contém os hashes
do código, hints, driver e compilador.

- **19 controles de sintaxe e escopo**;
- **4 controles puros do hint:** um positivo e três negativos;
- **9 compilações de bibliotecas artificiais com o smcs histórico**;
- **9 corpos completos iguais às referências explícitas em C#**;
- **10 compilações e execuções de executáveis artificiais**, cada uma com
  **83 cenários**; stdout dos candidatos idêntico ao switch de linha de base;
- **zero compilações candidatas do jogo**.

Há fixtures de vinte cases, blocos de case, break antecipado, for interno,
switch interno, labels compartilhados, default e campo modificado depois da
seleção. Uma fixture de switch dentro de for externo confirma que seu break
não termina o for. As negativas cobrem seletor com chamada/propriedade,
volatile, continue externo, local compartilhado entre cases, goto case,
ausência de opt-in e ausência de evidência para o braço zero.

**CONFIRMADO nas fixtures:** propriedade dos breaks, correspondência dos
corpos de referência e conservação das sequências observadas. **UNKNOWN nos
alvos reais:** ganhos, regressões, metadados e assinaturas após transformação.
