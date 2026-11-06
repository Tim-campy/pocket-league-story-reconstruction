# Rodada 7: transformações smcs e sondas isoladas

Este documento registra as regras da Parte A e duas regras preparadas para os
ciclos seguintes. Uma proposta de texto não altera o estado de um método. O
modelo prevê os locais antes da compilação candidata; a comparação completa de
corpo e metadados continua obrigatória. Somente `tools/verify` promove estados.

## Implementação e evidência de emissão

`tools/Round7SmcsRules.cs` usa a sintaxe e os símbolos do fonte recuperado.
`tools/round7_smcs_hints.py` extrai somente restrições do IL original: tipo do
slot, existência de saída compartilhada e ordem de stores numa cadeia de zero.
Os nomes das variáveis vêm do modelo do fonte e de votos não ambíguos de usos
alinhados dos slots. Nenhum trecho de IL é emitido ou copiado para uma DLL.

| Frente | Proposta | Evidência e limite |
|---|---|---|
| Valor testado | Declarar local do tipo do slot, inclusive `Int32` e referências, antes de testar | `stloc; ldloc` nas fixtures compiladas com smcs; não se antecipa o lado direito de `&&`/`||` |
| Resultado único | Converter retornos em atribuição ao resultado e saída comum; testar variável existente sem usos e variável nova | `Return.DoEmit` emite `ret` diretamente fora de região protegida; o desvio precisa existir na estrutura do fonte |
| Saída comum void | Converter guarda do restante em `if` positivo ou `switch` com `break` | O `switch` ganha um temporário próprio; um alias recuperado pode já representar esse mesmo slot |
| Objeto em local | Construção, atribuições aos membros e chamada receptora separadas | A forma `Push(new T { campo = x })` antecipa o carregamento de `this` e cria temporário de inicializador; a variável explícita muda essa emissão |
| Cadeia de zero longa | Agrupar declarações e emitir atribuição encadeada na ordem calculada | As fixtures incluem quatro variáveis e unificação de declarações homônimas em ramos disjuntos; capturas e colisões com campos/parâmetros são rejeitadas |

Fontes primárias locais: snapshot Unity Mono
`b6587926c0f146c8b12985744f33a13b39a22d57`, em
`analysis/tools/round5-mono-source/mcs/mcs/`. A identidade exata do commit que
gerou o executável smcs instalado continua `UNKNOWN`, como nas rodadas anteriores.

- [If.DoEmit, statement.cs 224–272](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/statement.cs#L224-L272): emite a condição, o ramo verdadeiro, o desvio de saída e o ramo falso nessa ordem.
- [Return.DoEmit, statement.cs 770–784](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/statement.cs#L770-L784): `Expr.Emit(ec)` e `ret`; retorno protegido guarda o resultado e usa `leave`.
- [Goto.DoEmit, statement.cs 831–839](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/statement.cs#L831-L839): emite `Br` ou `Leave`, inclusive quando o rótulo está logo adiante.
- [Break.DoEmit, statement.cs 1117–1120](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/statement.cs#L1117-L1120): usa `ec.LoopEnd`, o destino de saída do `switch`/laço.
- [Block.EmitMeta, statement.cs 2032–2047](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/statement.cs#L2032-L2047): locais declarados, temporários registrados e filhos são alocados em etapas separadas.
- [VariableReference.EmitAssign, expression.cs 4321–4357](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/expression.cs#L4321-L4357): a atribuição encadeada solicita cópia, emite `Dup` e guarda a variável.

## Fixtures

`python tools/test_round7_smcs_rules.py` executa controles de sintaxe e compila
apenas a classe artificial `Fixture` com o smcs instalado no Unity 5.3.4f1.
O relatório `reports/round7-smcs-rule-fixtures.json` fornece a contagem exata.
Inclui controles de spans UTF-16, teste de referência e inteiro, retorno de
lambda preservado, alias de parâmetro, receptor com efeito colateral rejeitado,
inicializador de objeto em lambda, cadeia longa e captura rejeitada.

`python tools/test_round7_smcs_hints.py` passou **8 fixtures** de geração das
restrições. Um modelo incompleto ou um vínculo de slot com dois nomes possíveis
não fornece nomes para a cadeia. Esses testes não compilam código do jogo.

A primeira versão da fixture usou `System.Func`/`System.Action`, ausentes nas
referências mínimas do perfil testado. A fixture foi corrigida para declarar
seus próprios delegates; nenhuma referência do projeto foi trocada por causa
desse erro da fixture.

## Hipóteses verificadas nas sondas conduzidas pelo motor central

O relatório `reports/round7-probes-Assembly-CSharp-engine.json` foi consultado
após a conclusão da sonda. Seus quatro ganhos de corpo completo somam
**508 bytes de IL**, com **zero perdas de corpo**. Esse resultado é evidência
de comparação completa do motor; adoção oficial e estado do ledger são
responsabilidade do fluxo principal.

- **CONFIRMADA:** `data.LeagueData::IsVictoryLeague()->System.Boolean` precisa
  de um local `Int32` para o retorno de `GetMatchInfoJoin`, antes do teste.
  `tested-value-local` fechou os **199 bytes** do corpo.
- **CONFIRMADA:** `data.PlayerData::GetPhyParamBarW(System.Int32,System.Int32)->System.Int32`
  usa o local existente como resultado comum. A primeira proposta com `goto`
  melhorou o prefixo, mas acrescentou um segundo desvio antes do rótulo.
  Remover somente esse desvio de queda direta fechou os **79 bytes**.
- **REFUTADA para o método, não como regra de equivalência:** transformar
  diretamente a guarda de `OnTouchDlgSel` em `if` positivo compilou e manteve
  a assinatura de locais, mas não avançou o prefixo.
- **CONFIRMADA:** `surface.GameView::OnTouchDlgSel(System.Int32,System.Int32,kairo.unity.surface.TouchEvent)->System.Void`
  precisa de `switch` sobre o parâmetro; o alias explícito recuperado ocupa o
  slot que deve ser o temporário do seletor. A primeira tentativa de `switch`
  sobre o alias seria barrada pelo slot adicional. Remover o alias, restaurar
  os usos do parâmetro e emitir o switch fechou os **134 bytes**. O original
  usa `ldarg.1` no teste interno em IL41, sustentando essa distinção.
- **CONFIRMADA:** `system.form.FriendForm::<_update>m__17()->System.Void` precisa
  de construção em local, atribuição a `friend_` e só então `Push(local)`.
  A regra inicial limitada a atribuição direta a campo não produzia candidato
  para o inicializador passado a `Push`. A regra ampliada fechou os **96 bytes**.

## SubForm: resultado parcial e a causa do bloqueio inicial

Em `form.SubForm::_draw2(kairo.unity.ui.Graphics)->System.Void`, os votos
alinhados associam `num4`, `num`, `num2`, `num3` aos slots originais 0, 1, 2, 3.
A declaração e a expressão encadeada calculadas usam essa ordem. O diagnóstico
fonte está em `reports/round7-smcs-zerochain-diagnosis.json`.

Somente a cadeia reduz a previsão de **59 para 56 slots**; os primeiros **55**
tipos já coincidem com o original, mas sobra um `Int32` de emissão de switch.
O recompilado possui dois opcodes `switch`, em IL1964 e IL8288 na linha de base,
e o original não possui nenhum. Eles reutilizam o mesmo temporário por tipo.

`extended-original-zero-chain-and-switch-if` combina a cadeia calculada com
conversão dos switches de seletores estáveis para if/else. A previsão torna-se
completa e coincide com os **55 slots** do original. O motor central aceitou
o avanço para prefixo **2218**, mas o corpo inteiro continuou divergente.
Portanto a recuperação completa desse método permanece **INCONCLUSIVA**.

## Preparação dos ciclos posteriores

`tools/round7_smcs_cycle_rules.cs` oferece entrada separada
`AddRound7SmcsCycleRules`. Sem `round7_cycle_rule`, nenhuma proposta é gerada.
O chamador deve escolher exatamente uma regra por ciclo:

- `terminal-guard-positive-tail`: dobra `if (!cond) return; restante;` em
  `if (cond) { restante; }`, também dentro do else terminal de uma cadeia.
  Todos os blocos/ifs envolventes precisam levar ao fim do método. Um comando
  posterior fora da cadeia impede a transformação. A hipótese para
  `DebugForm.Draw_param` ainda depende da comparação do motor.
- `empty-if-terminal-polarity`: inverte somente o último operando lógico de um
  if vazio, preservando os testes que decidem se ele é avaliado. Em
  `GameForm.Update`, original IL376 usa `brfalse 381`; recompilado usa
  `brtrue 381`, ambos após `CheckKeyRepeat(32)`. Trocar `A && !B` por `A && B`
  não muda quais chamadas são executadas quando o corpo é vazio. A hipótese
  de correspondência completa desse método ainda é **UNKNOWN**.

As fixtures isoladas exercitam essas duas regras. Elas não autorizam inclusão
silenciosa no motor ativo nem substituem o ciclo e a verificação completa.
