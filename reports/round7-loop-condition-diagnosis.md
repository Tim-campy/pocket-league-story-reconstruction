# Condição de laço fundida com guarda do corpo

Base: `reports/round7-cycle07-after-measurement.json`, após o commit `f095bdb`.
Há evidência concreta em **3 métodos, somando 14.802 bytes de IL original**:

| Método | Forma | Bytes de IL |
|---|---|---:|
| `form.DebugForm::Update_gameParam()->System.Boolean` | while | 13.913 |
| `form.GameForm::Update()->System.Boolean` | for | 534 |
| `form.SoccerForm::Update()->System.Boolean` | for | 355 |

As contagens são o peso dos alvos, não uma previsão de corpos que fecharão.
Nenhum candidato desses assemblies foi compilado por esta investigação.

## Evidência do while

DebugForm.cs contém:

```csharp
while (num4 >= 0 && udat_.memberPDataist_.Size() < 30) {
    PlayerData playerData = apdat_.pData_[num4];
    // corpo existente
    num4--;
}
```

O IL original testa `num4 >= 0` no fim do laço e chama `Size()` no começo do
corpo. A recompilação chama `Size()` junto à condição final. Essa mudança de
posição explica a primeira divergência por alinhamento, `ldfld => DELETION_POINT`
em IL1160; não é um problema na identidade do campo.

```text
Original                               Recompilado
IL1154 br 1236                         IL1154 br 1203
IL1159 ldarg.0                         IL1159 ldarg.0
IL1160 ldfld DebugForm.udat_            IL1160 ldfld MyFormBase.apdat_
IL1165 ldfld UserData.memberPDataist_    IL1165 ldfld AppData.pData_
IL1170 callvirt Size                    IL1170 ldloc.3
IL1175 ldc.i4.s 30                     IL1171 ldelem.ref
IL1177 blt 1187                        IL1172 stloc.s 4
IL1182 br 1243                         ...
IL1187 ldarg.0                         IL1210 ldarg.0
...                                    IL1211 ldfld DebugForm.udat_
IL1236 ldloc.3                         IL1221 callvirt Size
IL1237 ldc.i4.0                        IL1226 ldc.i4.s 30
IL1238 bge 1159                        IL1228 blt 1159
```

**Hipótese de fonte:** `while (num4 >= 0) { if (Size() >= 30) break; corpo; }`.
O protótipo preserva a ordem de avaliação e usa o helper de negação existente.
Ele não troca operadores de float por seus aparentes complementos.

Há outra diferença visível posteriormente no mesmo corpo: original IL1216
`brfalse`, seguido de `br` em IL1221, versus a emissão do fonte atual
`if (!BitUtil.Check(...)) { Employ(...); }`. Portanto corrigir a condição do
while, isoladamente, não prova que os 13.913 bytes do método inteiro fecharão.
Essa segunda leitura continua sendo diagnóstico até o verificador confirmar.

## Evidência independente para o for

GameForm.cs contém `for (int i = 0; i < num && !_update(i); i++) { }`.
O original executa `_update(i)` no corpo e salta para a saída quando ele retorna
true. Seu incremento permanece depois desse teste:

```text
Original                         Recompilado
IL0492 br 518                    IL0492 br 501
IL0497 ldarg.0                   IL0497 ldloc.2
IL0498 ldloc.2                   IL0498 ldc.i4.1
IL0499 call _update              IL0499 add
IL0504 brfalse 514               IL0500 stloc.2
IL0509 br 525                    IL0501 ldloc.2
IL0514 ldloc.2                   IL0502 ldloc.1
IL0515 ldc.i4.1                  IL0503 bge 520
IL0516 add                       IL0508 ldarg.0
IL0517 stloc.2                   IL0509 ldloc.2
IL0518 ldloc.2                   IL0510 call _update
IL0519 ldloc.1                   IL0515 brfalse 497
IL0520 blt 497                   IL0520 ldarg.0
```

Forma candidata: `for (int i = 0; i < num; i++) { if (_update(i)) break; }`.
SoccerForm.Update repete a evidência: original IL312–324 chama `_update` e faz
`brfalse 329; br 340`, enquanto o recompilado só chama `_update` junto ao teste
final, em IL325. Isso sustenta incluir for nesta família, sem estendê-la a foreach.

## Regra de emissão consultada

Snapshot Unity Mono `b6587926c0f146c8b12985744f33a13b39a22d57`, disponível em
`analysis/tools/round5-mono-source/mcs/mcs/statement.cs`:

- [While.DoEmit, linhas 448–465](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/statement.cs#L448-L465): salta inicialmente para a condição, emite o corpo, marca o começo da próxima avaliação e emite `expr.EmitBranchable(..., true)`.
- [For.DoEmit, linhas 582–619](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/statement.cs#L582-L619): emite corpo, marca `LoopBegin`, emite incremento e só então emite o teste.
- [Break.DoEmit, linhas 1117–1120](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/statement.cs#L1117-L1120): emite `br` ou `leave` para `LoopEnd`.

Logo uma chamada que pertence à condição aparece depois do corpo; uma guarda
explícita aparece dentro dele. O mesmo resultado pode manter a sequência de
chamadas em execução e ainda gerar corpos de IL diferentes.

## Bundle congelado e controles

`tools/Round7LoopConditionRules.cs` oferece
`AddRound7LoopConditionRules`, com a assinatura padrão dos repertórios. A
seleção obrigatória é `round7_cycle_rule: smcs-loop-condition-body-guard`.
Sem seleção, não gera propostas.

Para cada fronteira de uma conjunção de bools comuns, propõe:

```csharp
while (A && B) { corpo; }
// passa a
while (A) { if (!B) break; corpo; }
```

O for conserva seu inicializador e incrementos. Cada proposta move uma única
fronteira; `A && B && C` oferece separações após A ou após B, em candidatos
independentes. Operadores definidos pelo usuário, condições com conversão de
tipo custom para bool, OR isolado e foreach não são tratados. Não são criadas
variáveis, nem escritos métodos gerados.

Reprodução: `python reports/round7-loop-condition-fixture-driver.py`.
`reports/round7-loop-condition-fixtures.json` registra:

- **11 controles de sintaxe e rejeição**;
- **6 compilações de bibliotecas artificiais pelo smcs histórico**;
- **6 comparações completas de corpos iguais às referências C# explícitas**;
- **7 compilações e execuções de programas artificiais**, incluindo a linha de
  base com a condição fundida; os seis candidatos preservaram todo o stdout;
- **zero compilações candidatas do jogo**.

Cada execução confere 13 cenários. Alguns resultados de sequência de chamadas:

```text
while-calls:IABM+ABM+AB
while-A-false-no-B:IA
for-calls-continue-increment:IABM+ABM+AB
for-empty:ISSS
for-A-false-no-Stop:I
chain-first:IABCM+ABCM+AB
chain-last:IABCM+ABCM+AB
```

`A`/`B`/`C` marcam avaliações, `M` o corpo, `+` o incremento e `S` a chamada
do for vazio. O cenário A falso comprova que B não é avaliado; continue no
corpo do for mantém o incremento. Também se conferem NaN, infinito negativo,
-1, zero, 1 e infinito positivo; NaN e valores não negativos não executam o
corpo da comparação `x < 0f`.

A comparação de corpo inclui cabeçalho, locais, instruções, EH, seções extras
e flags. **CONFIRMADA nas fixtures:** a forma explícita produz o corpo de
referência e conserva as sequências observadas. **UNKNOWN nos três alvos:**
correspondência após compilação, metadados e regressões. Esses resultados
dependem do modelo de slots e do verificador completo do fluxo principal.
