# Diagnóstico smcs: `bne.un => beq` após cycle03

Base medida: `reports/round7-cycle03-after-measurement.json`, commit publicado
`295509a`. O par aparece em **20 métodos**, cujo IL original soma **38.863
bytes**. A proveniência completa dos dumps e os trechos dos 20 métodos estão em
`reports/round7-guard-diagnosis.json`. Estas contagens descrevem alvos; não
prometem que uma transformação isolada feche cada corpo.

| Forma encontrada no fonte e no IL | Métodos | Bytes de IL | Situação |
|---|---:|---:|---|
| Braço vazio ausente antes de else | 13 | 26.845 | Nova família sustentada por fixtures |
| Guarda continue seguida do restante da iteração | 2 | 4.839 | Transformação inversa ausente; nova família |
| Guarda continue com OR | 2 | 1.067 | `condition-split-or-guards` já existe; não é nova transformação |
| Guarda void terminal em SetPosData | 1 | 1.810 | B1 já cobre a forma; bloqueio é assinatura de locais |
| Guarda void terminal dentro de try/catch/finally | 1 | 2.250 | B1 não atravessa a região protegida; novo alcance |
| Guarda com retorno de valor antes de outro ramo | 1 | 2.052 | B1 só cobre void; nova inversão com else |
| **Total** | **20** | **38.863** | |

## 1. O maior peso é de braços vazios, não de guardas

### SubForm._draw2: 10.209 bytes

ID: `form.SubForm::_draw2(kairo.unity.ui.Graphics)->System.Void`.

```text
Original                       Recompilado
IL10184 ldfld type_             IL10184 ldfld type_
IL10189 ldc.i4.s 36             IL10189 ldc.i4.s 36
IL10191 bne.un 10201            IL10191 beq 10203
IL10196 br 10208                IL10196 ldarg.0
IL10201 ldarg.0                 IL10197 ldarg.1
IL10202 ldarg.1                 IL10198 call _draw3
IL10203 call _draw3             IL10203 ret
IL10208 ret
```

O fonte atual termina a cadeia com `else if (type_ != 36) { _draw3(g); }`.
A forma candidata é `else if (type_ == 36) { } else { _draw3(g); }`. Ela
conserva a orientação original `bne.un`, além do `br` que salta o else. B1
não se aplica: não há um `return` no corpo desse if.

### SubForm.Init: 5.191 bytes

ID: `form.SubForm::Init()->System.Boolean`.

```text
Original                       Recompilado
IL00324 ldfld type_             IL00324 ldfld type_
IL00329 ldc.i4.6                IL00329 ldc.i4.6
IL00330 bne.un 340              IL00330 beq 5099
IL00335 br 5189                 IL00335 ldarg.0
IL00340 ldarg.0                 IL00336 ldfld type_
IL00341 ldfld type_             IL00341 ldc.i4.7
IL00346 ldc.i4.7                IL00342 bne.un 458
```

Fonte atual: `else if (type_ != 6) { if (type_ == 7) ... }`. A forma candidata
restaura `else if (type_ == 6) { } else { if (type_ == 7) ... }`. O mesmo
padrão aparece em SoccerForm._draw, comparando `state_` com 13.

Em condições `A && B && C`, inverter tudo e trocar um único corpo de lado pode
fundir os destinos de desvio. O protótipo também oferece uma ocorrência por
vez: `if (!A) { } else if (B && C) body`. Isso permite recuperar cadeias de
casos vazios sem alterar todas as comparações num único passo. Uma terceira
variante conserva a conjunção inicial já correta e restaura o braço vazio
apenas no sufixo guardado. Ela cobre a forma observada em StoreBase.GetUnityJson.

**Hipótese de fonte:** ainda UNKNOWN nos assemblies reais. **Evidência medida:**
o IL original tem o teste e o salto explícito sobre o else; o recompilado
eliminou esse braço. **Confirmado nas fixtures:** as três variantes geram o
corpo completo das respectivas formas de referência.

## 2. Guarda continue: retirar o desvio extra sem alterar break

### SubForm.Init2: 3.844 bytes

ID: `form.SubForm::Init2()->System.Boolean`.

```text
Original                       Recompilado
IL00077 ldloc.3                IL00077 ldloc.3
IL00078 ldfld id_              IL00078 ldfld id_
IL00083 bne.un 95              IL00083 beq 93
IL00088 ldc.i4.1               IL00088 br 100
IL00089 stloc.1                IL00093 ldc.i4.1
IL00090 br 121                 IL00094 stloc.1
IL00095 ...                    IL00095 br 126
```

Fonte atual no for interno:

```csharp
if (TrainingData.TRA_PLAYER[i] != playerData.id_) { continue; }
flag = true;
break;
```

Forma candidata:

```csharp
if (TrainingData.TRA_PLAYER[i] == playerData.id_) {
    flag = true;
    break;
}
```

Somente o continue de guarda é removido; o break do corpo continua sendo break.
O bloco precisa terminar a iteração do mesmo laço. Havendo código após um
bloco envolvente, a proposta é rejeitada. MapMain.Draw_trainingCoat, de **995
bytes**, tem a mesma falta: `if (tMode_ != 3) continue;` antes do restante da
iteração. A direção antiga, if positivo para guarda continue, já existia; esta
é a direção inversa.

## 3. Os três casos restantes têm causas diferentes

- **TGroupArea.SetPosData, 1.810 bytes:** o fonte tem guarda void terminal e B1
  já propõe a inversão correta. O relatório de cycle01 registra
  `no_model_approved_candidate`, com **uma rejeição wrong_signature**. Não há
  justificativa para cadastrar a mesma transformação como nova.
- **GamePad.Draw, 2.250 bytes:** a guarda `if (mode_ != 2) return;` está num
  try/catch/finally terminal. O original usa `bne.un 2208`; o recompilado usa
  `beq 1721` seguido de `leave 2254`. O protótipo só atravessa um try terminal,
  preservando catch/finally e exigindo que todo o fluxo externo termine no
  método. Um comando posterior ao try impede a proposta.
- **SpForm.Update, 2.052 bytes:** `if (num2 != -3) { switch ...; return true; }`
  antecede `canvas_.ImeOnBlock(...)`. O original, após `bne.un` em IL488,
  emite primeiro o ramo de ImeOnBlock; o recompilado emite primeiro o switch.
  A forma candidata é `if (num2 == -3) { ImeOnBlock(...); } else {
  switch ...; return true; }`. É inversão com else, preservando o retorno do
  ramo, mesmo quando o bloco não é o último do método.

As duas guardas continue com OR, Player.ChMoveDirect e Player.GetToThrowInPID,
somam **1.067 bytes**. A transformação para guardas separadas já consta do
repertório. Elas não foram contadas entre as novas famílias.

## Compilador e fixtures

Fonte primária consultada: Unity Mono, snapshot
`b6587926c0f146c8b12985744f33a13b39a22d57`,
[If.DoEmit em statement.cs 224–272](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/statement.cs#L224-L272).
Após emitir o ramo verdadeiro, ele emite `br end` se existe ramo falso e o ramo
verdadeiro não retorna. Um ramo vazio continua atendendo a essa condição. O
mesmo código explica por que inverter só o operador não recupera o bloco ausente.

O bundle está em `tools/Round7SmcsGuardRules.cs`, entrada
`AddRound7SmcsGuardRules`, com seleção explícita por `round7_cycle_rule`:

| Seleção | Propostas |
|---|---|
| `smcs-empty-arm-restore` | Ramo vazio com else; variantes por primeiro operando AND ou sufixo |
| `smcs-continue-positive-tail` | Guarda continue → corpo positivo no fim da iteração |
| `smcs-value-return-guard-reverse` | Ramo que termina em retorno de valor + tail → ordem inversa com else |
| `smcs-protected-void-guard-tail` | Guarda void dentro de try terminal → corpo positivo, com EH preservada |

Sem seleção, nenhuma proposta é gerada. As negações usam o helper existente,
que mantém negação explícita das comparações de float em vez de trocar
operadores e perder o tratamento de NaN.

Reprodução: `python reports/round7-guard-fixture-driver.py`.
Resultado em `reports/round7-guard-fixtures.json`: **12 controles**, **8
compilações de classes artificiais pelo smcs histórico**, **8 corpos completos
iguais às referências**. A comparação inclui cabeçalho, locais, instruções,
cláusulas de exceção, seções extras e flags. Há fixtures de foreach com EH,
try/catch/finally e float, além de negativas para break, código após o laço/try
e ausência de seleção. **Candidatos do jogo compilados por esta investigação:
zero.** As quatro famílias ainda dependem do modelo e do verificador completo
nos alvos reais.
