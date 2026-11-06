# Rodada 7, preparação do segundo ciclo: `br => ret`

Este diagnóstico foi feito após a Parte A, antes de recompilar qualquer candidato
do jogo para estas hipóteses. A medição usada é
`reports/round7-part-a-after-measurement.json`, seção
`assemblies.Assembly-CSharp.alignment`. Os dumps e seus hashes constam em
`reports/round7-smcs-branch-exit-diagnosis.json`, que preserva a proveniência
da medição. O grupo contém **24 métodos e 41.632 bytes de IL original**.

## Classificação dos destinos, não apenas dos opcodes

| Destino do `br` original | Métodos | Bytes de IL dos métodos |
|---|---:|---:|
| Instrução `ret` | 14 | 17.880 |
| `ldloc X; ldc.i4.1; add; stloc X` | 7 | 18.434 |
| Outro destino, interpretação ainda UNKNOWN | 3 | 5.318 |
| Total | 24 | 41.632 |

A segunda linha comprova um destino de incremento de local. Interpretá-lo como
o incremento do `for` do fonte ainda passa pelo vínculo de origem, pelo modelo
de slots e pela comparação do corpo. As contagens são inventário dos alvos;
**não são previsão de quantos métodos fecharão** com uma regra isolada.

## Exemplo 1: TeamData.SetScore

ID: `data.TeamData::SetScore(System.Int32,System.Boolean)->System.Void`.
Corpo original: **115 bytes**. Primeiro par: original IL12 `br 114`, recompilado
IL12 `ret`. O destino original IL114 é o único `ret` do método.

```text
Original                       Recompilado
IL0006 ldarg.2                 IL0006 ldarg.2
IL0007 brfalse 17              IL0007 brfalse 13
IL0012 br 114                  IL0012 ret
IL0017 ldc.i4.0                IL0013 ldc.i4.0
IL0018 stloc.1                 IL0014 stloc.1
...                            ...
IL0114 ret                     IL0110 ret
```

Fonte atual: `if (isUserTeam) return;` seguido do laço de atualização dos gols.
A forma candidata é `if (isUserTeam) { } else { laço; }`. A regra de emissão do
smcs executa o ramo verdadeiro, emite `br end` quando há um `else` e depois
emite o ramo falso. Portanto até um ramo verdadeiro vazio conserva
`brfalse body; br exit; body: ...`.

A Parte A testava a forma positiva `if (!isUserTeam) { laço; }`, cujo primeiro
desvio se torna `brtrue exit`. Essa forma não reproduz o prefixo original. A
alternativa com switch introduz uma estrutura de seletor desnecessária para este
padrão. **Nova hipótese:** ramo verdadeiro vazio mais else, preservando a condição.

Fonte do compilador: snapshot Unity Mono
`b6587926c0f146c8b12985744f33a13b39a22d57`,
[If.DoEmit, statement.cs 224–272](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/statement.cs#L224-L272).
O código emite `OpCodes.Br, end` se existe `FalseStatement` e o ramo verdadeiro
não termina por return. Aqui o ramo verdadeiro é vazio.

## Exemplo 2: SubForm._draw2

ID: `form.SubForm::_draw2(kairo.unity.ui.Graphics)->System.Void`.
Corpo original: **10.209 bytes**. Primeiro par: original IL9268 `br 9358`,
recompilado IL9268 `ret`. O único `ret` original está em IL10208.

```text
Original                       Recompilado
IL9261 ldloc.s 48              IL9261 ldloc.s 48
IL9263 brtrue 9273             IL9263 brtrue 9269
IL9268 br 9358                 IL9268 ret
IL9273 ldarg.1                 IL9269 ldarg.1
...
IL9358 ldloc.s 47
IL9360 ldc.i4.1
IL9361 add
IL9362 stloc.s 47
IL9364 ldloc.s 47
IL9366 ldc.i4.s 16
IL9368 blt 9065
```

Fonte atual: `if (teamData4 == null) { return; }` no corpo do `for` de `num20`.
O destino original é o incremento do contador do laço, não a saída do método.
**Nova hipótese:** substituir apenas esse `return;` por `continue;`, sem
inverter a condição nem reorganizar o restante do bloco.

## Exemplo 3: Player.Act_dribble

ID: `game.Player::Act_dribble()->System.Void`.
Corpo original: **4.343 bytes**. Primeiro par: original IL638 `br 776`,
recompilado IL638 `ret`. O destino original começa com
`ldloc.s 6; ldc.i4.1; add; stloc.s 6`, o incremento do `for` que percorre jogadores.
Os returns originais estão em IL3765 e IL4342.

Fonte atual: `if (player.pos_.x_ * pos_.x_ > 0f) { return; }`.
**Nova hipótese:** `continue;` nessa mesma guarda. A comparação de ponto
flutuante permanece escrita exatamente como está: o original usa `ble.un` em
IL633 para pular a guarda. Reescrever o operador sem preservar o tratamento
de NaN poderia produzir outro IL.

Fonte do compilador:
[Continue.DoEmit, statement.cs 1150–1153](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/statement.cs#L1150-L1153)
usa `ec.LoopBegin`; a emissão do `for` coloca esse rótulo antes do incremento.
Essa estrutura explica os dois destinos de incremento observados.

## Hipótese refutada na regra anterior

**REFUTADA:** estar no fim de um bloco lexical basta para trocar um if por guarda
com retorno antecipado. Um bloco pode ser o corpo de um laço ou um ramo cujo
fluxo continua depois dele. A antiga `tail-early-return` verificava somente
`parentBlock.Statements.Last() == branch`, sem exigir fim real do método.

Isso explica candidatos parciais cujo prefixo melhorava, mas que introduziam
`ret` onde o original tinha continuação de laço. Esses métodos permaneciam
pendentes; o diagnóstico não identifica perda de método já em matched. A
restrição nova feita pelo fluxo principal usa `Round7TailToMethod`.

## Protótipos e fixtures

O novo arquivo separado `tools/Round7SmcsExitRules.cs` expõe
`AddRound7SmcsExitRules` com a mesma assinatura dos demais repertórios. Ele não
foi incluído no helper ativo durante este diagnóstico. Exige uma seleção e
evidência de destino independentes:

| `round7_cycle_rule` | `original_exit_target_kind` obrigatório | Proposta |
|---|---|---|
| `void-empty-then-common-exit` | `method-ret` | Guarda void no fim real do método → ramo verdadeiro vazio com restante no else |
| `loop-guard-return-to-continue` | `loop-increment` | Return void dentro de for com incremento → continue, condição intacta |

`tools/round7_smcs_branch_exit_diagnose.py::branch_exit_hints(original, offset)`
classifica o destino original. Sem evidência ou sem seleção, o protótipo não
propõe alteração. A sequência de incremento deve guardar o mesmo slot que
carregou; slots diferentes produzem `UNKNOWN`.

`python tools/test_round7_smcs_exit_rules.py` passou **10 controles**, com
**3 compilações de fixtures pelo smcs histórico** e **3 comparações completas de
corpo iguais** a métodos de referência escritos em C#. Foram comparados
cabeçalho, locais, instruções, EH, seções extras e flags do método.

As três fixtures cobrem ramo vazio com else, guarda de referência dentro de for
e guarda de float dentro de for. As negativas incluem ausência da evidência IL,
ausência da seleção da família, continuação fora do método e slot de incremento
incompatível. **Compilações candidatas do jogo nesta investigação: zero.**

A correspondência dos três métodos reais continua **UNKNOWN para os protótipos**
até o motor realizar o ciclo e o verificador completo aprovar o resultado.
