# Rodada 9 — cinco leituras da Parte B

Esta preparação usa o corpus congelado após A5: `build/round9/cycles/a05/after`. O helper possui cinco opt-ins explícitos, cada um modificando uma única ocorrência de fonte no método indicado. Não modifica modelos, estado, IL ou assemblies; não implementa o extrator genérico da Parte C. As adoções e o verificador do jogo pertencem ao orquestrador.

## Resultados medidos antes das sondas do jogo

| Caso | Família | Fixture histórica | Previsão NoEmit do candidato | Estado do corpo do jogo nesta preparação |
| --- | --- | --- | --- | --- |
| GameForm._update | b9-event-loop-split-guard | controle integral idêntico | completo 13; original 14 | UNKNOWN; veto por assinatura completa errada |
| SubForm.Init | b9-empty-arm-common-return | controle integral idêntico | completo 64; original 65 | UNKNOWN; veto por assinatura completa errada |
| SoccerForm.Draw_newPlayerPopular | b9-guard-return-empty-positive-arm | controle integral idêntico | completo 8/8 originais | UNKNOWN até o verificador |
| Balloon.Draw com Matrix4x4 | b9-positive-render-with-gui-else | controle integral idêntico | incompleto, 61 tipos; original 59 | UNKNOWN; assinatura real precisa ser testada |
| Graphics._drawBitmap | b9-boolean-value-before-if | controle integral idêntico | incompleto, 53 tipos; original 52 | UNKNOWN; assinatura real precisa ser testada |

As cinco comparações de fixture incluem cabeçalho, maxstack, locais, IL com referências resolvidas e EH. Apenas o nome da declaração do método de controle difere e fica fora desse confronto do corpo. O run final fez quatro compilações artificiais, duas smcs e duas csc 1.1.0.51109. Zero compilações do jogo foram feitas por este agente.

Dois problemas de preparo foram corrigidos antes do resultado final: expansão automática de LF para CRLF no arquivo artificial invalidava os offsets do primeiro teste (nenhuma compilação histórica ocorreu nessa tentativa); o primeiro confronto incluía o nome diferente da declaração do controle (duas compilações artificiais feitas). Foram seis compilações artificiais no desenvolvimento até a entrega; o recibo do run final registra suas quatro.

## Destinos reais e limites das inferências

### GameForm._update

Original L7 é offset304: `ldloc.0; ldc.i4.1; add; stloc.0`, seguido da condição do for em308. É a continuação/incremento do for de eventos. A reconstrução R265 é apenas `br 304`, um bloco intermediário de falha do guard composto; não é uma aresta de retorno ao cabeçalho por si só.

O original usa `brfalse236` após CheckAlreadyEvent e `br304` no bloco231; depois CheckTermScript é testado com `brfalse304`. A hipótese separa o guard `null || already`, conservando seu continue, e envolve ExecEvent/return false num if positivo de CheckTermScript. As expressões e a ordem das chamadas permanecem as mesmas. Não há break introduzido, e apenas esse guard é modificado.

### SubForm.Init

Original L97 é5189: `ldc.i4.1; ret`. O trecho330 emite `bne.un340; br5189`. Isso prova a saída comum que carrega true, mas não distingue por si só um break, goto, do(false) ou outras formas históricas.

A alternativa testada restaura um braço vazio: `else if (type_ == 6) { } else { ... }`, usando o restante já existente e a folha final já existente. Não introduz return nem valor novo. A fixture produz o desenho condicional+br esperado. O controle negativo com `return true` direto difere no IL. O fonte histórico literal continua UNKNOWN.

### SoccerForm.Draw_newPlayerPopular

Original L5 é746: `br835`. O835 inicia `ldloc.s4; ldc.i4.1; add; stloc.s4`, continuando o for de jogadores. Logo528 não retorna do método; ele pula para um join intermediário, que então segue ao incremento. Substituir o return recuperado diretamente por continue eliminaria esse salto intermediário; a fixture negativa confirma a diferença.

A alternativa é `if (stateCnt_ < limite) { } else { resto desse braço }`, mantendo o if/else exterior de select_ e o for. Não se afirma que L5 era historicamente um switch exit: o destino é comprovadamente o join observado, e o C# com braço verdadeiro vazio é apenas uma hipótese testável que o reproduz. A comparação é Int32; o helper recusa a versão artificial de ponto flutuante.

### Balloon.Draw

`IsGUI()` em23 desvia em28 para254, início da parte GUI. A leitura `g.render_` em34 usa `brfalse3011`;3011 contém o ret final. O caminho não GUI que efetivamente adiciona a tarefa termina com ret253.

Uma mera troca do guard por `if (render_ != null)` seguida de return local não reproduz esses destinos e também conserva a ordem errada de locais. A proposta envolve o bloco não GUI no if positivo e põe toda a parte GUI no else exterior. Assim render_=null segue ao final do método, enquanto o caso não GUI válido conserva seu return antecipado. A fixture negativa sem esse else difere. A mudança continua sendo uma única ocorrência de estrutura de guard; nenhum cálculo numérico interno é reescrito.

### Graphics._drawBitmap

Original1874–1886 carrega setFigureClip_, emite `ldc.i4.0; ceq`, salta ao join de um valor Boolean e então usa `brfalse`. A reconstrução emite diretamente `brtrue` a partir do campo. A hipótese cria um local Boolean imediatamente antes do if e usa esse local como condição; sua expressão é copiada literalmente. O csc otimizado pode manter o valor na pilha sem um novo slot, como na fixture. A previsão conservadora continua incompleta, portanto a assinatura física só pode ser decidida compilando.

O helper modifica somente o primeiro predicado selecionado. A diferença posterior envolvendo !flag e flag2 permanece fora desse patch. Operadores e expressões numéricas existentes, incluindo possíveis casos NaN/.un em outros trechos, ficam intactos.

## Fontes primárias do compilador

- Mono/Unity snapshot `b6587926c0f146c8b12985744f33a13b39a22d57`, `mcs/mcs/statement.cs:225–267`: If.DoEmit usa `expr.EmitBranchable(..., false)` e, havendo else e um true que não termina o fluxo, emite `br end`. Um bloco verdadeiro vazio pode, portanto, produzir o desvio condicional seguido do br observado sem precisar inventar um break.
- Mesmo arquivo:3830–3866: Switch.DoEmit cria LocalTemporary para a expressão não constante, chama Store e, ao terminar, Release. O comentário registra que até uma variável não capturada é duplicada. Isso sustenta o diagnóstico dos temporários abaixo.
- Roslyn snapshot `4cb203dab89e1542cdf7d88cb6d32767bc0e5460`, `EmitOperators.cs:243–278`: EmitShortCircuitingOperator produz o valor do operando direito ou uma constante no join;672–678 emite ldc.i4.0/ceq para sentido falso. EmitStatement.cs:594–613 declara locais de um bloco antes de seus comandos, explicando o efeito do else em Balloon. O otimizador pode manter um local de leitura única na pilha.

Esses arquivos estão no cache analysis/tools; os commits exatos de fabricação dos compiladores instalados continuam UNKNOWN. Os controles foram compilados pelos executáveis históricos selecionados, não pelo Roslyn moderno do helper.

## Slots que bloqueiam GameForm/SubForm, para a Parte C posterior

Não houve composição dessas correções com os patches B.

- GameForm tem um Int32 original no slot12: GetMatchInfoJoin em1413, stloc12 em1418 e ldloc12 em1420 antes de brtrue. No fonte atual a chamada é usada diretamente na condição, sem esse local. O add recompilado12 corresponde ao original13; a assinatura de tipos, por conter Int32 repetidos, não revela sozinha essa identidade.
- SubForm tem dois Int32 originais,12 e20, vindos de GetMatchInfoJoin: stores1622/2357 e leituras1624/1686,2359/2463. Os dois switches atuais reutilizam um único temporário63, stores1582/2317, o que explica64 locais atuais contra65 originais. O original compara casos por bne.un sequenciais; a reconstrução usa beq do despacho dos switches. Hipótese posterior: locais explícitos e cadeias de if, não apenas um local colocado antes de um switch que criaria outra cópia temporária.

Essa diferença independente impede compilar os candidatos B06/B07 sob a regra de rejeitar previsão completa errada. Não é uma refutação de suas hipóteses de fluxo. Provas de usos, lifetimes e tipos estão no diagnóstico de slots.

## API e transporte

`tools/Round9PartBRules.cs`, entrada AddRound9PartBRules no partial Round7TransformSyntax, lê o opt-in round7_cycle_rule. Os cinco nomes de família são os da tabela. Sem opt-in não há propostas. O orquestrador deve usar uma família e um método por vez, isolate-family e single-step/limite de uma iteração conforme sua API.

O calculador específico da Parte B gera patches de declaração inteira a partir de uma única alteração AST interna. before/after vêm dos bytes UTF-8 originais e offsets UTF-16, sem universal-newline. GameForm possui dois LF históricos misturados com CRLF; Soccer possui cinco. Eles são preservados literalmente para rebase exato, em vez de serem convertidos silenciosamente.

Os bundles incluem hashes de helper/calculador/modelos/clients e exigem nova previsão e rebase único no motor. As provas NoEmit referem-se ao snapshot A5; após adoções, o conteúdo de Main pode ter mudado. Só o verificador do agente principal pode declarar ganhos do jogo.
