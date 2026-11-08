# OnApplicationPause e Transform.Transpose — WIP finito pós-D06

Estado: propostas privadas **READY_FOR_GAME_PROBE** depois da conferência do
índice. Zero compilações do jogo, zero mudanças públicas, zero ganhos medidos.
O operador exclusivo decide a seleção, o rebase e a prova real. Nenhuma
outra família foi iniciada.

## Identidade e baseline

Usa exclusivamente `build/round11/cycles/cycle06/after/KairoLibrary`.
Hashes dos fontes brutos, manifesto, dumps e receipt em `inputs.json`.
Os patches do método inteiro mantêm before exato e offsets UTF16; não
tratam fonte WIP antigo como Main atual.

REA primário limitado a IApplication e Transform:
`ev_768c529290ad5d1b879b47869087f555a526cb9c9a2769f5a91216edc9210d79`.
Página dos104métodos:
`ev_af03e9a8dfc2d9bb0a9c653a9618c24c75a5a49725649e3c5d4a829e762b8dfa`.
Artifact SHA256 `bb553d341eecd9b900f772486bf4a5c289aa2f09566d9d402a3eb70268f4b3ee`,
MVID `b564fa31-554d-4b48-a8c0-a0b41bea2550`. `rea-byte-binding.json`
compara os hashes dos bytes CIL dos dois tokens com os DTOs congelados.

## IApplication.OnApplicationPause(Boolean) —199bytes

Original199bytes, atual195. Original tem oito locais, agrupados por braço:
NotifyManager,FormManagerBase,Int32,FormBase; novamente os mesmos quatro.
O atual tem seis, começando pelo FormManagerBase da cauda exterior ao if.
Os dois catches são System.Exception. Original: try69..77/handler77..80
e try168..178/handler178..181.

A composição preserva as folhas, as chamadas e o `return` do braço true:

1. `NotifyManager.GetInstance()?.DoNotify()` vira um NotifyManager nomeado
   dentro do braço true e um teste explícito `(object)local != null` seguido
   da chamada normal, emitida como `callvirt`.
2. A cauda de retomada passa a um `else` explícito. Seu NotifyManager nomeado
   e seu FormManagerBase deixam de ser locais do bloco exterior; isso
   reproduz o primeiro grupo0–3 e o segundo4–7. O `return` de suspensão
   continua antes da cauda, sem mudar seu destino semântico.
3. `DoCancel` recebe o mesmo molde de receiver explícito no braço false.

Não basta expandir os dois `?.`: o negativo com somente essa alteração gera
200bytes e outra ordem de locais. O componente de escopo é necessário na
fixture. O primeiro nullguard original17 salta para25, configuração de
notificação; o segundo104 salta para113, configuração de retomada. Nenhum
deles é interpretado como retorno ou entrada de outro fluxo.

API pura: `transform_pause_notify_and_else(method_source) -> str`.
Família: `r11-pause-notify-receivers-and-branch-scopes`.

## Transform.Transpose() —197bytes

Original197bytes, atual199; ambos sem locais e sem EH. O original e o atual
criam array float[16] e leem os mesmos16elementos de `mtx_`, nesta ordem:
0,4,8,12,1,5,9,13,2,6,10,14,3,7,11,15.
O original não tem `dup` antes do último elemento nem `pop` depois dele.
O campo `mtx_` **não recebe o array** em nenhum dos lados. A lógica
aparentemente inútil é preservada, sem tentar corrigir o jogo.

Proposta: separar `float[] array = new float[16] { ... };` em criação
`float[] array = new float[16];` e16atribuições individuais de elementos,
mantendo exatamente a ordem das leituras de campo. O scheduler pode manter
o local apenas na pilha: os15primeiros usos duplicam a referência e o último
a consome. Isso explica a retirada dos dois bytes sem copiar IL nem editar
o resultado. Não se armazena o array no campo nem se introduz `KeepAlive`.

API pura: `transform_transpose_last_array_read(method_source) -> str`.
Família: `r11-array-initializer-last-use-stack-consumption`.
O produtor exige os16elementos exatos, nenhuma folha anterior/posterior e
nenhuma escrita adicional no campo.

## Fixtures e análises sem emissão

Foram feitas oito compilações próprias com csc1.1.0.51109,
SHA256 `53b693536e8a86f35d05d2165ff9b3e0aeaa499ecfd8135a9f99ab4404ca86c3`,
flags e referências fixadas no manifesto D06. Somente `/out` e `/pdb`
foram redirecionados a cada pasta privada; o PDB histórico não foi tocado.

| Fixture | Before IL/locais | Candidato IL/locais | Controle manual IL/locais | Negativo IL/locais |
|---|---:|---:|---:|---:|
| Pausa |195/6|199/8|199/8|200/8|
| Transpose |199/0|197/0|197/0|203/0|

Os dois candidatos coincidem com controles manuais independentes em todas
as facetas de corpo verificadas por `compare_kairo.differences`; os dois
before e os dois negativos não coincidem. O negativo Transpose acrescenta
KeepAlive e prova que o último uso passa a exigir a referência preservada.
Os catches da fixture de pausa coincidem com os limites originais acima.
Seis negativos textuais recusam fonte já convertido, colisão de alias,
alteração da folha de suspensão, leitura de matriz reordenada e escrita
extra no campo.

NoEmit do fonte real, com modelo congelado:

- Pausa: incompleto, oito tipos explícitos. Motivos incluem os dois
  receivers com múltiplos usos, os dois locais de form de uso único e EH.
- Transpose: incompleto, prevê um local e registra
  `stack_scheduler_multiple_use:array`. A compilação da fixture mostra zero
  locais. Não se afirma que a previsão incompleta é uma assinatura real.

Não se forçou completude, não se alterou modelo e não se deve transformar
esses casos em veto anterior à compilação permitida pelo orçamento.

Uma previsão simbólica privada, sem produzir DLL ou bytes para o build,
compõe as folhas atuais com os moldes comprovados e rótulos relocados.
Ela resulta em199/197bytes, zero diferenças de instruções/operandos/offsets
e as mesmas cláusulas EH do original. Essa medição é **diagnóstica**, não
substitui compilação do alvo, header/maxstack reais ou prova de regressões.

## Fonte do Roslyn e histórico

Fonte consultada: revisão `4cb203dab89e1542cdf7d88cb6d32767bc0e5460`,
tag version-1.1.0; git-blobs conferidos contra a árvore congelada. A revisão
exata de fabricação do csc instalado continua UNKNOWN.

- `EmitStatement.cs:594–630` define locais do bloco antes dos comandos,
  respeitando os escopos dos braços. `DefineLocal:1375–1400` dispensa slot
  se o local foi escolhido para a pilha.
- `EmitExpression.cs:1178–1254` e1287–1342 distinguem receiver não nulo de
  acesso condicional e chamada ordinária; o primeiro pode usar `call` para
  método não virtual, enquanto o receiver nomeado usa `callvirt` no molde.
- `EmitArrayInitializer.cs:82–99` emite `dup` para cada elemento de um
  inicializador, incluindo o último.
- `Optimizer.cs:1762–1770` e1861–1881: leituras não finais de stack local
  viram `BoundDup`; o último acesso conserva o nó e consome a referência
  já presente na pilha. Isso é distinto do inicializador completo.

Pausa:54registros históricos, oito trials reais de hoist, todos rejeitados;
nenhuma composição de receivers+else completa igual foi encontrada.
Transpose:53registros, zero trials efetivos capturados. Um relatório sem
candidato não é uma hipótese refutada. Os SHA/textos completos capturados
estão em `history-sha-audit.json`; ausência de hash não prova exaustão global.

Os produtores e bundles anteriores permanecem congelados. Entrega:
`ready-for-game-probe.json` e `ready-freeze.json`. Potencial de alvo:
dois métodos/396bytes originais; ganho medido nesta tarefa:0métodos/0bytes.
Resultado real de ambos: **UNKNOWN até o oráculo do operador**.
