# Ações seguidas de retorno num braço terminal

Família opt-in: `smcs-terminal-actions-common-exit`.
Entrada: `AddRound7TerminalArmRules(input,text,mapped,methodBody,model,point,patches,seen)`.
Arquivo candidato: `tools/Round7TerminalArmRules.cs`.

A hipótese restaura o seguinte fonte, preservando a condição e a ordem das ações:

```csharp
if (condicao) { Acao1(); Acao2(); return; }
Cauda();
```

```csharp
if (condicao) { Acao1(); Acao2(); }
else { Cauda(); }
```

Isso só preserva o retorno ao fim do método quando o bloco pai é terminal.
A regra exige `Round7TailToMethod(block, body)`: cada bloco que contém o
caminho até o corpo do método precisa ser o último comando do seu pai, e o
caminho só pode atravessar blocos, if e else. Loops, try e outros ancestrais
interrompem a prova. Não basta o if ou seu braço parecerem terminais localmente.

As regras existentes usam `Only(branch.Statement) is ReturnStatementSyntax`.
Um braço com chamadas e um return permanece um bloco nessa consulta e não
recebe a transformação. A regra nova exige um bloco com pelo menos uma ação
linear seguida de return void direto. Recusa return aninhado, controle de fluxo
nas ações anteriores, else preexistente, retorno com valor, corpo de lambda,
capturas no trecho movido, try/using/lock/fixed, yield, goto/labels e foreach.
Um for comum na cauda é permitido; ele é necessário nos exemplos de serialização
e desenho. Não se permite que o bloco transformado esteja dentro de um loop.

Não é obrigatório que o destino imediato do br original seja ret. Em `_draw5`,
o caminho medido é `IL9734 br IL9859`, `IL9859 br IL12061`, `IL12061 ret`.
A prova estrutural do fonte chega ao corpo do método, sem loops ou EH. O
comparador independente continua decidindo se os ramos emitidos correspondem.

## Casos lidos

O histograma por alinhamento após o ciclo 08 tinha 14 métodos e 37.050 bytes
no par `br => ret`. Após o ciclo 09, `_draw5` entrou nesse grupo: são 15 métodos
e 49.112 bytes. Esses números descrevem o grupo diagnóstico, não ganhos previstos.

| Caso | Evidência original | Forma recuperada | Gate fonte-only |
|---|---|---|---|
| `SoccerForm.Draw_newPlayerLv` | IL1914 br IL2038; IL2038 ret | AddTouch e return; depois cálculo e for | Completo, 13 locais iguais |
| `DelayEvent.Serialize` | IL111 br IL169; IL169 ret | WriteInt(os,0) e return; depois gravação e for | Completo, 1 local igual |
| `SubForm._draw5` | IL9734 br IL9859; br IL12061; ret | Dois AddTouch e return; depois TouchOptions e duas chamadas | Completo, 79 locais iguais |

Os dois primeiros pais são o próprio corpo do método. O terceiro pai é um
bloco de um braço de uma cadeia de if/else terminal. O relatório conserva a
lista de ancestrais e seus offsets: somente Block, IfStatement e ElseClause,
terminando explicitamente em `Block:method-body`. Os trechos antes/depois
também estão no relatório para revisão independente da classificação.

## Fixtures e limites

`round7-terminal-arm-fixtures.json` registra 20 checks aprovados:

* Quatro casos de emissão real smcs: bloco raiz, else terminal aninhado, cauda
  com for e declaração local no braço. Todos produzem corpo completo, locais
  e EH iguais aos exemplos estruturados em if/else.
* Dez recusas: braço sem ações, bloco com código posterior fora da cadeia,
  ancestral loop, ancestral try/finally, using na cauda, return aninhado,
  captura, corpo de lambda, retorno com valor e else já existente.
* Dois controles opt-in: família ausente e compilador Roslyn não produzem a regra.
* As quatro propostas positivas também têm spans UTF-16 e identidade do texto
  `before` verificados antes da compilação das fixtures.

Os exemplos da fixture têm nomes diferentes. A comparação de emissão usa
corpo canônico completo, assinatura de locais e EH, distinguindo os nomes;
não altera estado de método do jogo e não normaliza assemblies.

O auxiliar de fixtures compila cópias isoladas dos fontes das ferramentas e
injeta a chamada opt-in apenas nessa cópia. Os arquivos ativos, Main, modelos
de slots, clientes e motor permaneceram intocados. Os três exemplos reais foram
apenas analisados, sem compilação de candidatos do jogo.

O SHA-256 do arquivo candidato é
`36e632883c5a3727ddf0bc41dc25dc04f051eca0ea5053834073058d072fd501`.
O fechamento de qualquer método do jogo e o ganho efetivo em bytes continuam
UNKNOWN até o processo principal compilar e executar a verificação completa.

## Reprodução e ponto de entrada

`python tools/test_round7_terminal_arm_rules.py` regenera o relatório de fixtures
em `reports/round7-terminal-arm-fixtures.json`.
`python tools/round7_terminal_arm_examples.py` lê o Main atual, recalcula os três
exemplos sem emitir assemblies do jogo e grava
`reports/round7-terminal-arm-examples.json`. A publicação foi revalidada depois
do commit do ciclo 10; os hashes dos três fontes são do Main atual, não do
snapshot histórico. Os offsets de IL do ciclo 08 continuam rotulados como tal.

O arquivo candidato não foi ligado ao helper ativo por esta frente. O processo
principal pode acrescentá-lo à lista de fontes e chamar
`AddRound7TerminalArmRules(cycleRequest.RootElement,text,mapped,methodBody,model,point,patches,seen)`
no dispatcher opt-in. A família precisa ser selecionada explicitamente e o
compilador precisa ser smcs. Os modelos e todos os gates do motor permanecem
necessários. `Round7TerminalArmPath` só expõe a prova de ancestrais nas cópias
isoladas usadas pelos testes; não influencia a decisão de aceitação.
