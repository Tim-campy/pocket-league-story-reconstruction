# Rodada 6: alias de parâmetro calculado em DrawGradation

Esta nota registra uma análise do snapshot `66eea9d`, antes de compilar qualquer candidato decorrente dela. Evidência reproduzível: [round6-calculated-parameter-aliases.json](../../reports/round6-calculated-parameter-aliases.json), gerada por [round6_alias_calculus.py](../../tools/round6_alias_calculus.py). Nenhum estado foi promovido por essa análise.

## Os três controles

O mesmo padrão está em `system.form.BillingForm.DrawGradation`, `system.form.FriendForm.DrawGradation` e `system.friend.ui.FriendData.DrawGradation`, todos estáticos e com parâmetros `Graphics g, int dx, int dy, ...`.

| Evidência no snapshot | Original | Recompilado anterior |
| --- | --- | --- |
| Locais | 11 | 10 |
| Início após a guarda | `IL_0011 ldarg.1; IL_0012 stloc.0; IL_0013 ldarg.2; IL_0014 stloc.1` | `IL_0011 ldarg.2; IL_0012 stloc.0` |
| Uso correspondente no FillRect | `IL_0193 ldloc.0` (403 decimal) | `IL_018b ldarg.1` (395 decimal) |
| Stores no slot original 0 | 1 | Slot ausente |
| Loads no slot original 0 | 1 instrução IL | Leitura direta de dx |

O load original está dentro de um laço: uma única instrução IL não significa uma única execução em runtime. O valor do parâmetro é copiado para o local antes do laço e esse local é lido pelo `FillRect` em cada passagem.

Remover o primeiro `Int32` da assinatura original reproduz exatamente a assinatura recompilada de dez locais. Todos os demais slots são deslocados em uma posição. Portanto, a troca da ordem de duas declarações com inicialização é **REFUTADA como explicação suficiente**: ela não acrescentaria o local que falta.

## Proposta calculada

A hipótese é introduzir `int dxAlias = dx;` imediatamente antes da declaração recuperada `int num = dy;`, depois da guarda inicial. A leitura de `dx` usada no argumento correspondente do `FillRect` deve passar a `dxAlias`. A leitura direta de `dx` na expressão anterior `dx + w` deve continuar direta, porque o original também usa o argumento nessa expressão. Substituir todo o restante do bloco, ou copiar o `dy` encontrado no comando mapeado, não reproduz esse padrão.

`dxAlias` é um nome legível escolhido para a reconstrução. O nome do local histórico continua **UNKNOWN**. A identidade do parâmetro vem do índice de metadados 1; não foi escolhida por tentativa entre todos os parâmetros.

O cálculo aceita apenas o caso restrito de um local ausente. Ele exige um único store original do parâmetro no slot, pelo menos um load, nenhum uso por endereço, nenhum write/uso por endereço do parâmetro e que o store domine todos os loads no fluxo normal. Métodos com regiões de exceção são recusados nesse cálculo de dominância. Depois remove virtualmente o par de inicialização, projeta os demais números de slots pelo deslocamento calculado e alinha os acessos. Cada load do alias deve corresponder a um `ldarg` do mesmo parâmetro em um contexto único. Essa projeção é somente diagnóstico; não altera o verificador nem os assemblies.

## Ponto de fonte e segurança da transformação

Os MDB observados para esses métodos fornecem a linha, mas deixam coluna e fim de intervalo em `-1`. A transformação recupera o comando AST único nessa linha e confere o número e a ordem das leituras do parâmetro contra as instruções IL do ponto de sequência. Se houver mais de um comando folha possível, diferença de contagem, parâmetro escrito/ref/out/capturado ou falta de vínculo de símbolos, a proposta é recusada.

A regra está em [Round6AliasRules.cs](../../tools/Round6AliasRules.cs), separada dos auxiliares ativos durante sua preparação. Os testes sintáticos independentes ficam em [round6-alias-rule-tests.json](../../reports/round6-alias-rule-tests.json). O modelo de slots, a compilação real e a verificação completa continuam obrigatórios antes de adotar o C#. A confirmação do corpo completo pertence aos resultados posteriores do motor e do verificador, não a esta nota.
