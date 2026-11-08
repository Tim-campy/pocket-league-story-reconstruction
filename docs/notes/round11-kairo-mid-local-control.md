# Dois métodos Kairo — propostas privadas após D4

Nenhum fonte público, modelo, ledger ou DLL foi alterado. Nenhum alvo foi
compilado nesta tarefa. Os corpos do jogo continuam **UNKNOWN** até o
oráculo do operador. Não houve ganho contado.

## Snapshot e evidência primária

Snapshot: `build/round11/cycles/cycle04/after/KairoLibrary`. Os hashes de
fontes, dumps, manifesto e receipt estão em `inputs.json`; os patches contêm
o intervalo UTF16 e o before bruto, inclusive CRLF.

REA: `ev_11e6061b2da39cfabf58d940bdfa431c57dedc9d7ecd7b13246139ccb6d88b07`,
consulta limitada aos tipos Image e Language. Artefato original SHA256
`bb553d341eecd9b900f772486bf4a5c289aa2f09566d9d402a3eb70268f4b3ee`,
MVID `b564fa31-554d-4b48-a8c0-a0b41bea2550`. A seleção de membros é
completa; o REA informa dois flags terminais de #US inconsistentes, mantendo
o UTF16 legível, sem afirmar aceitação em runtime. Não se altera nenhum #US.
`rea-byte-binding.json` prova igualdade dos hashes de IL dos dois tokens
entre os DTOs congelados e o REA. Nenhum destino é interpretado apenas pelo
número: a instrução de chegada foi conferida.

## Image.SetTransparentEnabled

ID: `kairo.unity.ui.Image::SetTransparentEnabled(System.Boolean)->System.Void`.
Original 275 bytes, atual 279; quatro locais idênticos e nenhuma região EH.
O primeiro par alinhado real é, em 117, `ldloc.1` contra `ldelema Int32`.
O original carrega array/índice para o destino, depois array/índice/elemento
novamente, aplica o AND e usa `stelem.i4`. O atual usa referência ao elemento,
`dup`, `ldind.i4` e `stind.i4`. As duas regiões do alinhamento são partes
desse único lowering; não é uma inversão de if.

Proposta: somente
`aRGBPixels[i] &= 16777215;` →
`aRGBPixels[i] = aRGBPixels[i] & 16777215;`.
Array e índice são locais estáveis; não há chamada/propriedade a reavaliar
nem alteração de ordem da operação. A comparação do pixel e o restante dos
dois braços ficam intactos. API pura:
`transform_image_explicit_mask(method_source) -> str`.
Família: `r11-array-element-explicit-mask-assignment`.

Fixture própria, compilada com csc1.1.0.51109:
before164, candidato160, controle manual independente160. O corpo inteiro
do candidato passa contra o controle. O before e a variante negativa
`mask & elemento`, que troca a ordem das cargas, falham. NoEmit do método
real: completo, quatro tipos previstos exatamente iguais aos originais.

A previsão abstrata, montada das folhas atuais e do molde comprovado,
explica todos os opcodes, operandos resolvidos e destinos: 275 bytes,
zero diferenças. Não é DLL emitida nem substitui header/maxstack/oráculo.

## Language.TranslateSoftLabel

ID: `kairo.unity.util.Language::TranslateSoftLabel(System.String)->System.String`.
Original215, atual211. Original tem cinco locais: String, Int32, Int32,
Int32, String; o atual tem três Int32 e o temporário String de retorno.
O guard inicial permanece igual. Original15 é `ldarg.0; stloc.0`, antes do
teste da tabela; os receptores em28/50/104/126 leem esse local. O retorno
de fallback em210 continua `ldarg.0; ret`, não o alias. O retorno protegido
em187 grava o quinto local, faz `leave.s212` e retorna esse local em212.

Proposta: declarar `string softLabelInput = softlabel;` imediatamente depois
do guard inicial. Trocar apenas os quatro receptores dos dois pares de
comparações `.Length`/`.Equals` para o alias. O guard e os dois retornos do
argumento permanecem exatamente sobre `softlabel`. Os outros retornos,
folhas, chamadas, try/catch e laços ficam no mesmo lugar lexical.
API pura: `transform_language_input_alias(method_source) -> str`.
Família: `r11-softlabel-input-alias-before-table-loops`.

Fixture própria: before211, candidato215 e controle manual215, cinco locais,
corpo completo e catch iguais. O before e a variante negativa que retorna
o alias no fallback falham contra o controle. NoEmit real permanece
**incompleto**, motivo genuíno `exception_lowering`: prevê quatro locais
explícitos e omite o temporário String de retorno protegido. Não se forçou
completude, não se alterou o modelo e isso não veta o experimento conforme
o protocolo. O quinto slot foi observado na fixture compilada, não inferido
como inexistente a partir do modelo.

A previsão abstrata calcula215 bytes, todos os operandos/offsets iguais e
catch original com try100..207, handler207..210 e catch System.Object.
O salto em11 leva à cópia do argumento em15; o retorno de fallback210 e
o retorno protegido212 continuam distintos. Os17hunks e a retargetização
do alinhamento inicial são explicados pelo alias e a mudança dos índices.
O oráculo do jogo permanece UNKNOWN.

## Fonte Roslyn fixada

Revisão `4cb203dab89e1542cdf7d88cb6d32767bc0e5460`, tag version-1.1.0.
Os git-blob SHA1 dos arquivos consultados coincidem com `tree.json`
congelado; hashes SHA256 e intervalos estão em `compiler-source-evidence.json`.
A equivalência exata da revisão de fabricação do csc instalado é UNKNOWN.

- `LocalRewriter_CompoundAssignmentOperator.cs:479–527` descreve o lowering
  não dinâmico de `array[index] op= R` para um temporário por referência.
- `EmitAddress.cs:352–370` carrega array/índices e emite `ldelema` para SZArray.
- `EmitExpression.cs:2031–2037` carrega array/índices do lado esquerdo de uma
  atribuição comum; 2199–2204 grava com `EmitArrayElementStore`.
- `EmitStatement.cs:594–630` define os locais do bloco antes dos comandos;
  1375–1400 dispensa o slot apenas se o otimizador escolheu stack local.
- `Optimizer.cs:1419–1437` exige só a pilha de avaliação nas fronteiras de
  try/catch; 1549–1604 registra esses requisitos e os estados em saltos e
  rótulos. Isso restringe manter o alias através dos dois laços e do try
  somente na pilha. A materialização concreta foi conferida na fixture.

## Histórico, negativos e contagens

Para cada método foram encontrados51registros de engine. Image tem nove
trials reais: hoist, polaridade, temporário de condição, tail-return e solver
de declarações, todos preservados como rejeitados. Language tem sete trials:
temporários, hoist, divisão de condições, guardas e local de retorno.
Não há candidato completo com mesmo texto/hash no histórico capturado.
Não se chama um relatório com zero candidatos de hipótese refutada.
`history.json` mantém os trials integrais; `history-sha-audit.json` contém
os hashes e limites dessa busca. Não se afirma exaustão global por SHA ausente.

Foram feitas12compilações próprias no total: oito finais retidas e quatro
Image bem-sucedidas, depois supersedidas. O primeiro driver passou a classe
Language inteira para uma API que aceita método, causando ValueError antes
de qualquer compilação Language; o chamador foi corrigido, o produtor não.
O SHA dos binários supersedidos não foi capturado e está UNKNOWN.
Nenhum erro de compilador foi observado. Quatro controles finais negativos
falham (os dois before e as duas variantes erradas); dois candidatos passam
contra controles independentes em todas as facetas de corpo.
Os paths `/out` e `/pdb` das fixtures são privados, portanto não sobrescrevem
o PDB histórico do build do operador.

Bundle e freeze: `ready-for-game-probe.json` e `ready-freeze.json`.
Ganhos medidos do jogo nesta tarefa: **0 métodos,0bytes**.
