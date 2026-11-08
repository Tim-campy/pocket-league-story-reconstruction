# SubForm.Init2 — composição privada para a fila inicial D3

Estado: **READY_FOR_GAME_PROBE** somente após conferir `ready-freeze.json`.
Nenhuma compilação do jogo foi feita aqui; corpo, maxstack emitido, cabeçalho
emitido, metadados globais e regressões do candidato continuam **UNKNOWN**.

## Identidade e medida

- Método: `form.SubForm::Init2()->System.Boolean`, token `0x06000211`.
- Baseline R11: `build/round11/baseline/Assembly-CSharp`.
- Fonte lógico: `src/Assembly-CSharp/form/SubForm.cs`.
- Fonte congelado SHA256: `4bbba7380796ab5df5dd2f48f09c1a44fad0254cf9c3e5e6dfc615126cf9c39f`.
- Original: 3.844 bytes de IL; recompilado: 3.769; diferença: +75 bytes.
- Ambos: maxstack 154, cabeçalho fat de 12 bytes, initlocals, nenhuma região EH.
- Ambos têm 68 locais, **mas 25 posições de tipo são diferentes**. A premissa
  inicial “68 slots já iguais” só era verdadeira para a contagem.
- REA primário: `ev_56ed6dc22fafff6b14eb38ee682a1143a34601ef42c98f57da3dfab77650ac9e`.
  Artefato SHA256 `3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949`,
  MVID `48ce9f89-73d2-40be-bb24-22c807c04106`.
  Os 3.844 bytes do corpo congelado coincidem com o hash de IL reportado pelo
  REA: `132a0db1f8c4ea6c3bbe20c4b7c51a376843ff0390c4602b8a265b0da37ce531`.

## Diagnóstico completo proposto

O alinhamento estrito tem 136 regiões. Uma correspondência diagnóstica por
papéis de uso resolve os 68 locais sem colisões: 0–31 mantêm o índice;
32–66 atuais correspondem a 33–67 originais; o atual 67 corresponde ao original
32. São 178 ocorrências de codificação de local diferentes. Depois de expor
esses papéis, restam **20 regiões reais de controle**, descritas abaixo.
A correspondência não altera o original, o resultado ou o verificador.

1. Primeiro laço `j`: substituir apenas o guard `!= ... continue` por `==`
   envolvendo o `flag=true; break` existente. Original 83 salta para 95,
   incremento de `j`; o `break` em 90 chega à saída do laço em 121. Não é
   `break` do laço exterior. Diminui cinco bytes.
2. `if (!flag)` recebe um `else` vazio. Original 135 é `br 140`, inclusive
   quando o destino é a próxima instrução. 140 incrementa `i`. Acrescenta
   cinco bytes, compensando exatamente o item anterior.
3. Restaurar 16 braços verdadeiros vazios, mantendo os corpos atuais no `else`:
   dois testes no guard de `TRA_PLAYER[num3]`/`value1_`; um em `num7`;
   tipos 52, 54, 56, 57, 66, 69, 71, 72 e 74; `num13`, `num18`,
   `questMatchData3.state_`, e a chamada booleana `BitUtil.Check`.
   São 80 bytes adicionais. Os nove braços de tipo chegam à saída comum
   3842 (`ldc.i4.1; ret`). Os demais chegam aos incrementos dos respectivos
   laços: 697, 1227, 2038, 2440, 2945 e 3816. Não se introduz um `return`
   no lugar desses saltos. Não se troca qualquer comparação flutuante.
4. `switch (udat_.GetMatchInfoJoin())` vira um Int32 nomeado, avaliado uma
   vez, seguido de `if(selector==0)`/`else if(selector==1)`. Os dois corpos
   existentes já terminam em `return Pop()`; não há `break` de switch a
   reconstruir. O seletor nomeado é previsto no slot 32; as classes locais
   dos casos passam a 33/34. A dispatch nesta posição usa menos cinco bytes.

As chamadas de negócio e sua ordem lexical, as strings, os `break`, os
retornos e todas as folhas dos braços existentes são preservados. Retira-se
um único `continue`, o do primeiro laço `j`, pela estrutura positiva provada.
Não se escreve classe gerada nem se copia qualquer byte para um assembly.

## Fonte do compilador e fixtures

Snapshot Mono consultado: revisão
`b6587926c0f146c8b12985744f33a13b39a22d57`. A revisão exata de fabricação
do smcs instalado continua UNKNOWN. Os hashes locais constam de
`compiler-source-evidence.json`.

- `mcs/mcs/statement.cs:225–267`, especialmente 247–267: o `if` emite o
  salto falso, o braço verdadeiro e, se existe `else` e o braço verdadeiro
  não termina, um `br` para a junção. Isso também vale para braço vazio.
- Mesmo arquivo: 577–618 (`for`, incremento/LoopBegin/LoopEnd),
  1117–1120 (`break` → LoopEnd), 1148–1151 (`continue` → LoopBegin),
  2032–2047 (variáveis/temporários registrados/filhos),
  3823–3866 (`switch`, temporário criado na emissão e depois liberado).
- `mcs/mcs/codegen.cs:357–397`: temporários de emissão podem reutilizar
  armazenamento por tipo. `cs-parser.jay:6077–6105` cria o bloco implícito
  após comandos anteriores a uma declaração.

Foram feitas **12 compilações de fixtures próprias**: quatro casos,
cada qual com before, candidato e controle manual independente, usando
smcs SHA256 `26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0`,
as flags/referências fixadas no manifesto R11, sem compilar fonte do jogo.
Os quatro candidatos coincidem com seus controles em todos os aspectos
de corpo comparados por `compare_kairo.differences`; os quatro before diferem.

| Fixture | Before IL | Candidato IL | Controle IL | Delta |
|---|---:|---:|---:|---:|
| Primeiro laço + else vazio | 146 | 146 | 146 | 0 |
| 16 braços vazios | 435 | 515 | 515 | +80 |
| Seletor nomeado | 165 | 159 | 159 | −6 |
| Composição integrada | 750 | 825 | 825 | +75 |

Resultado negativo preservado: a estimativa inicial −5 do seletor isolado
estava errada. Na fixture isolada há outra redução de um byte pelas formas
curtas dos locais 0–4. Na fixture integrada os locais envolvidos já têm
operandos `.s`, e a diferença é −5, tal como na composição proposta para
o método real. `fixture-report.json` mantém a estimativa refutada;
`fixture-width-audit.json` a explica. Não foi necessário repetir builds.
Cinco controles textuais rejeitam âncoras duplicadas, fonte já convertido,
colisão do nome, case adicional/incompatível e transferência `break` no case.

## Análises sem emissão e limites

O modelo de locais congelado foi executado com NoEmit sobre o fonte completo
antes/depois. Antes: completo, 68 tipos, reproduz os tipos atuais e não os
originais. Candidato: completo, **68 tipos exatamente iguais aos originais**.
Não se promoveu completude nem se relaxou o modelo para este método.

Uma previsão abstrata de emissão, separada do resultado e nunca consumida
pelo build/verificador, combina as folhas recompiladas existentes, os
20 moldes de controle testados, rótulos simbólicos e os papéis de locais.
Ela prevê **1.344 instruções, 3.844 bytes e zero diferenças** de opcode,
operando resolvido ou offset contra o original. Isso explica os 34 destinos
antes desalinhados. A previsão **não é** uma compilação ou aprovação do corpo;
maxstack/cabeçalho emitidos e ausência de regressões exigem o oráculo real.

## Histórico e handoff

O candidato R8 `calculated-selector-continue-composition` mudou apenas o
primeiro match e o seletor: duas sondas registradas terminaram 43→9, sem
adoção. Esse negativo foi mantido. A nova composição acrescenta 16 braços
vazios e a junção do `else` do flag; não repete aquele candidato isolado.
Os relatórios posteriores com zero candidatos não são classificados como
hipóteses refutadas. `history-sha-audit.json` contém relatórios e hashes,
contagens reais e ausência de candidato completo idêntico no histórico lido.

Produtor puro: `calculate.py`, API
`transform_init2_control_composition(method_source) -> str`, sem I/O ao
importar ou chamar. Componentes separados estão no mesmo arquivo para o
executor revisar fixtures. Bundle: `ready-for-game-probe.json`, patch do
método inteiro com before UTF16, fonte bruto congelado e dependências.
Só o operador poderá compilar o alvo e declarar se o corpo passa.

Ganhos medidos do jogo nesta tarefa: **0 métodos, 0 bytes**.
