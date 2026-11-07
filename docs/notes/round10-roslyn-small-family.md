# Cache lógico compartilhado entre Lock e Unlock — somente WIP

Snapshot exclusivo: `build/round10/cycles/cycle12/after/KairoLibrary`. Medição: `reports/round10-cycle12-after-measurement.json`. Nenhuma leitura de Main foi usada para vincular propostas, nenhuma compilação do jogo ocorreu e nenhum método foi adotado. Os alvos continuam **UNKNOWN** até o oráculo oficial.

## Família selecionada

| Método | IL original / próprio | Locais original / próprio | maxstack original / próprio | Regiões de sequência |
|---|---:|---:|---:|---:|
| `Graphics::DrawText(TextFormat,String,Single,Single,Single,Int32)->Void` | 341 / 350 | 3 / 4 | 10 / 9 | 6 |
| `TextLayout::Draw(TextFormat,Graphics,String,Single,Single,Single,Single,Int32,Int32,Int32)->Void` | 396 / 420 | 5 / 6 | 9 / 8 | 19 |

Os IDs completos, ambos os corpos e todos os hunks estão em `diagnostics.json`. Os dois métodos somam737bytes de IL original; esse número é peso de seleção, não ganho previsto ou comprovado. O alinhamento não encontrou retarget independente nos dois métodos.

Os dois fontes recuperados têm a mesma construção: declaram `int numX`, testam `lf==null || lf.GetFont()==-1`, atribuem `font.IsNeedLocalizeScale()?1:0` e testam o Int32; o else atribui zero. Depois de várias chamadas, testam novamente o mesmo Int32 para Unlock. No próprio, isso gera normalização explícita do Boolean, slot Int32 e loads posteriores.

No original, o resultado Boolean é usado diretamente e mantido na pilha. Graphics134..143 e TextLayout136..145 mostram:

```text
callvirt Font.IsNeedLocalizeScale
br.s     junção
ldc.i4.0
junção: dup
brfalse.s  depois-do-Lock
```

O último teste de Unlock consome o valor que permaneceu na pilha, sem `ldloc` imediatamente anterior. O height local e, em TextLayout, o local do layout mudaram de slot como consequência da eliminação desse Int32 extra. As demais operações nos seis/dezenove hunks correspondem a essa normalização, ao store/load do Int32 e aos slots deslocados; não foi observada outra expressão, chamada ou região EH divergente. Nenhum dos métodos tem EH.

## Composição proposta

API pura: `calculate.py::transform_localize_boolean_cache(method_source)->str`, sem I/O em import ou chamada. Para cada um dos dois métodos:

```csharp
bool numX = (lf == null || lf.GetFont() == -1) && font.IsNeedLocalizeScale();
if (numX) {
    // Corpo existente inteiro, incluindo height, ajuste y e Lock.
}
// Chamadas existentes e seus argumentos preservados.
if (numX) {
    Font.UnlockLocalizeSetting();
}
```

Graphics usa o mesmo receiver `font_`; TextLayout usa o mesmo local `font`. A declaração conserva o identificador recuperado e muda seu tipo para bool. Não se inventa um nome histórico. A chamada continua sendo avaliada uma única vez, somente quando a condição antiga habilitava o primeiro if. Corpo, folhas e chamadas posteriores são preservados. As expressões flutuantes do ajuste de y ficam literalmente iguais; não se inverte operador de comparação flutuante.

O produtor recusa usos adicionais do cache antes/depois ou dentro do corpo habilitado, braços diferentes, statements adicionais, ausência de unicidade ou else que não atribua zero. Ele só aceita os dois moldes explícitos deste snapshot. As propostas inteiras têm offsets UTF16, SHA do arquivo, antes/depois exatos e exigem rebase único futuro; não se declara vínculo atual com Main.

## Fonte primária da emissão

Revisão Roslyn1.1.0: `4cb203dab89e1542cdf7d88cb6d32767bc0e5460`. `pinned/provenance.json` vincula as cópias ao SHA1 do Git blob e ao SHA256; EmitOperators foi conferido contra o tree fixado, sem baixar ou instalar compilador.

- `src/Compilers/CSharp/Portable/CodeGen/EmitOperators.cs:243..278`: EmitShortCircuitingOperator avalia o operando direito como valor, desvia para a junção e emite zero no caminho falso para &&. Isso explica por que o resultado direto da chamada pode alcançar a junção, sem a conversão `?1:0`. Linhas290..321 selecionam exatamente esse caminho para LogicalAnd.
- `src/Compilers/CSharp/Portable/CodeGen/Optimizer.cs:1153..1177`: condições consomem a entrada da pilha e ramos/condicionais registram consistência entre estados. Não é permitido eliminar um local só por ter tipo bool ou poucos usos.
- Mesmo arquivo:1629..1680 limita scheduling pela profundidade e pelo símbolo no topo;1866..1881 transforma leitura não final em BoundDup. Os dois originais e as duas fixtures demonstram o resultado concreto com dup e o Boolean vivo até o último teste. Não se generaliza para caches que atravessam EH ou outros usos.
- `src/Compilers/CSharp/Portable/CodeGen/EmitExpression.cs:576..585,2188..2197` emite o dup e evita store para local efetivamente agendado na pilha. O máximo da pilha aumenta em1, justamente como medido nos dois originais.

## Histórico real, incluindo resultados negativos

`history.json` captura166registros para os cinco exemplos inicialmente considerados. `history-selected.json` retém66registros dos dois métodos selecionados: Graphics tem9trials reais em R4, TextLayout tem10. Todos esses trials de R4 falharam em produzir avanço adotado; seus prefixos permaneceram127 e134. As regras foram ternário→if, polaridade, condition-local, hoist e tail-early-return isolados. Registros com0trials não são chamados de testes negativos desta hipótese.

A composição nova altera a declaração para Boolean e combina a condição externa com o resultado da chamada; não repete esses snippets isolados. `history-sha-audit.json` compara os SHA dos fontes candidatos completos às proveniências históricas disponíveis e não encontra candidato idêntico. Alguns trials antigos não têm SHA completo, portanto ausência de registro não prova exaustividade global.

## call versus callvirt: sondagem limitada, sem proposta adicional

`call-kind-survey.json` examinou somente os40corpos pendentes C12, associando chamadas pela identidade resolvida do método e sua ordem. Só encontrou duas trocas de opcode entre chamadas assim pareadas: `IApplication.OnApplicationPause` original20 `callvirt DoNotify` versus próprio21 `call`, e original108 `callvirt DoCancel` versus próprio107 `call`. O fonte recuperado usa `NotifyManager.GetInstance()?.DoNotify()` e `?.DoCancel()`.

O emitter fixado explica esses casos: `EmitExpression.cs:1253..1254` considera BoundConditionalReceiver comprovadamente não nulo;1287..1336 escolhe call para método não virtual com receiver desse tipo. BoundThisReference também permite call (1231..1234), mas um BoundLocal comum não consta da lista e retorna false (1259), mesmo que sua carga possa depois usar a pilha. Portanto um alias de this/local poderia mudar o opcode; aqui isso continua apenas uma possibilidade geral, não uma transformação nova confirmada por fixture ou um candidato proposto.

OnApplicationPause tem outras diferenças de locais/escopos e já pertence ao padrão de guard explícito investigado anteriormente. Ele não foi incluído no bundle. A sondagem não afirma que todas as chamadas não pareadas ou com membro diferente foram explicadas; sua conclusão se limita às chamadas de identidade resolvida igual. Os outros dois exemplos inicialmente lidos, AddPictograph105 e GetMaterial110, mostram associação de atribuição em indexador já coberta pelo repertório/Canvas e também não receberam proposta nesta tarefa.

## Fixtures e modelo

Seis compilações próprias, em dois trios before/controle manual independente/candidato, com csc1.1.0.51109 SHA53b6935… e flags/referências do manifesto C12. O produtor não gera os controles; `write_manual_fixtures.py` contém seus corpos explícitos.

- Graphics toy:263→254IL,4→3locais, maxstack8→9.
- TextLayout toy:252→234IL,6→5locais, maxstack9→10.
- Todos os corpos de cada candidato igualam integralmente os controles, incluindo cabeçalho, locais, IL e EH; os dois before diferem. Ambos reproduzem o padrão de cinco opcodes original e removem exatamente um slot. As classes Font/Layout próprias são tipos independentes: mesma contagem/papel não significa igualdade literal da assinatura com os tipos do jogo.
- Quatro negativos textuais foram recusados: uso dentro do bloco, uso adicional posterior, else não zero e statement extra no braço habilitado. Não foram contados como compilações.

O modelo congelado foi executado somente em NoEmit com os163fontes C12 vinculados por input_path/hash e helper já em cache. Ambos os resultados são genuinamente **complete=false**: Graphics prevê4tipos (múltiplos usos de num/num3); TextLayout prevê6 (múltiplos usos de num2). Esses números não foram manualmente reduzidos para3/5. As fixtures mostraram o scheduling real nos moldes próprios; a assinatura real do alvo continua UNKNOWN e a previsão incompleta não é veto conforme a política atual.

Estado: **READY_FOR_GAME_PROBE**, condicionado ao rebase e às verificações oficiais. Ganhos do jogo nesta tarefa:0métodos/0bytes. Nenhum arquivo público, modelo, alvo, ledger, configuração ou bundle anterior foi alterado.
