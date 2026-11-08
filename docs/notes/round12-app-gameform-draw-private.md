# R12 B3 — GameForm._draw

Proposta privada calculada para `form.GameForm::_draw(kairo.unity.ui.Graphics)->System.Void`, original `0x0600015b`, 2.703 bytes de IL. **Zero compilações do jogo/verificador; corpo do alvo UNKNOWN.** Esta frente não examinou `Draw_battlePic`.

## Medida e hipótese conjunta

Fonte congelado: `build/round12/baseline/Assembly-CSharp/src/Assembly-CSharp/form/GameForm.cs`; SHA `361ebf2270f0711913589c6b2f9178624cdde495eeb84c7c9c2323d27d269564`. O Main desse arquivo foi conferido byte a byte na preparação e no empacotamento. Há quebras CRLF/LF mistas, preservadas; o pacote contém método inteiro antes/depois e intervalo UTF16.

Original: IL 2.703, fat header 12, maxstack 184, 27 locais, EH vazio. Atual: IL 2.710, fat header 12, maxstack 184, 28 locais, EH vazio. Primeira diferença: `stloc.3` contra `stloc.1`, em 10. A assinatura de tipos e os usos ancorados permitem calcular as três mudanças juntas:

1. Declarar no ponto da cadeia inicial `int num13, num, num2, num3;`, nessa ordem, e inicializar `num13 = num = num2 = num3 = 0;`. O IL original em 8–15 é `ldc.i4.0; dup; stloc.3; dup; stloc.2; dup; stloc.1; stloc.0`. O atual contém somente três destinos, na ordem 1/0/2.
2. No bloco posterior `state_ == 3`, trocar somente `int num13 = 0;` por `num13 = 0;`. O original usa **o mesmo slot0** da cadeia inicial para o x dos tiles: grava0 em 1.921, grava `120*j` em 1.945 e lê o slot0 para largura/desenho. O y correspondente usa slot1. Não se movem comandos, incrementos ou laços desse bloco.
3. Converter o switch de `matchInfoJoin` em `if (matchInfoJoin == 0) { case0 } else if (matchInfoJoin == 1) { case1 }`, retirando somente os dois `break` terminais pertencentes ao switch. O seletor já existe no fonte e não é reavaliado. Original grava o resultado de `GetMatchInfoJoin` no slot14 em 1.384, testa2 e volta a ler **esse slot14** em 1.467/1.525. Atual grava no slot13 e copia para um temporário de switch27 em 1.458; o temporário não existe no original.

O slot2 original tem apenas `stloc.2` em 12 e nenhuma leitura. O nome `num2` já existe no fonte recuperado; não se cria nome para alegar a descoberta de variável histórica. `num13`, usado no state3, é movido de declaração para o grupo inicial a partir dos usos reais. Seus nomes originais continuam UNKNOWN. Não há captura, goto, yield ou retorno atravessando a região declarativa desse método.

O mapa completo de slots atual→original é `{0:2,1:3,2:1,3..19:+1,20:0,21..26:identidade}`; o temporário27 desaparece. `slot-constraints.json` registra tipo, nome calculado, primeiro/último uso, stores e loads de cada slot original. Os nomes vêm do fonte proposto e não de metadados de locais originais.

A aplicação **abstrata** dessas três mudanças aos objetos de instrução atuais, recalculando tamanhos de opcodes locais e destinos, resulta em **992 instruções e 2.703 bytes, zero objetos diferentes do original e zero hunks residuais**. É diagnóstico, sem escrever bytes/DLL e sem substituir compilação real. Header, assinatura efetivamente emitida e EH do alvo ainda dependem do oráculo.

## REA e regra de emissão

REA primário `ev_c9c1f6f48de81cbf647cdf113a8dafc3df4c9b14ecf39b284ea0b8ad2f6e3d55`, token `0x0600015b`, assembly original SHA `3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949`, MVID `48ce9f89-73d2-40be-bb24-22c807c04106`. IL SHA `f7cdeed874710164131373154e4c83d5db7bb0c4c825768af201c68a97b0f6a7`; conferido com os 2.703 bytes de IL do dump original após o fat header.

Mono fixado `b6587926c0f146c8b12985744f33a13b39a22d57`:

- `analysis/tools/round5-mono-source/mcs/mcs/statement.cs:1550–1555`: variáveis do bloco em `ListDictionary`.
- `statement.cs:2032–2047`: `Block.EmitMeta` resolve variáveis do bloco, temporários registrados, depois blocos filhos. A ordem de declaração e o escopo do quarto inteiro explicam os slots; não se faz busca por nomes.
- `analysis/tools/round5-mono-source/mcs/mcs/assign.cs:399–407`: uma atribuição usada como expressão pede `leave_copy=true` ao alvo.
- `analysis/tools/round5-mono-source/mcs/mcs/expression.cs:4321–4355`: a atribuição de variável não içada emite a origem, depois `dup` se `leave_copy`, depois `Variable.EmitAssign`. A cadeia de quatro destinos produz três `dup` e os stores de dentro para fora.
- `statement.cs:3823–3843`: `Switch.DoEmit` cria um `LocalTemporary`, emite e guarda `new_expr`, mesmo quando o seletor já é uma variável; o comentário registra `TODO: Don't duplicate non-captured VariableReference`. A cadeia de ifs evita esse temporário conforme o IL original.
- `statement.cs:225–267`: `If.DoEmit` salta para o ramo falso, emite o verdadeiro e o salto sobre o else quando necessário. O original em 1.469 usa `brtrue` para pular case0, e em 1.528 `bne.un` para pular case1.

A revisão histórica consultada não é uma prova da totalidade do fonte interno do compilador Unity. As fixtures abaixo usam o smcs instalado com SHA fixado.

## Fixtures e NoEmit

**10 compilações próprias**, zero alvo. Smcs SHA `26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0`, runtime SHA `3d102d9a63dae72e48396998bdbae4e9b2d8952ca6a8ebbcba698d3bc4345076`; opções/referências App fixadas, `-out` privado e `-debug-`. Nenhum PDB/MDB de produção.

Primeiro conjunto, sete compilações: candidato e controle manual com nomes distintos têm header, 12 locais, IL completo e EH iguais, 238 bytes. Molde anterior251. Os componentes isolados (`root-only`252 e `switch-only`237) diferem do controle; a composição é necessária nessa fixture. Negativos de ordem de stores e polaridade de case também diferem. Três controles do produtor rejeitam raiz duplicada, dono de break aninhado e break não terminal, sem compilação adicional.

Segundo conjunto, três compilações: reproduz o fato de `num2`/slot2 ser **inteiramente não lido**. Candidato/controle independente continuam iguais; slot2 fica na assinatura e possui somente o store inicial. O molde anterior difere. Os resultados completos estão em `fixture-report.json` e `fixture-unused-report.json`.

NoEmit R11 genuíno com auxiliar previamente existente e hashes: atual **complete=true/28tipos**, correspondendo ao corpo atual divergente; candidato **complete=true/27tipos exatamente iguais ao original**. O gate de candidato completo está aprovado pelo modelo; nenhuma condição de incompletude foi forçada e nenhum modelo foi alterado. A assinatura real continua obrigatória após compilação.

## Histórico, API e limite

74 recibos desse método nos relatórios engine, 30 registros de trials, sem avanço adotado. Trials registrados: `local-declaration-order-earlier`4, `local-declaration-scope-hoist`18, `local-declaration-order-later`8. Nenhum trial registrado combina cadeia de zeros/hoist do x existente/remoção do temporário de switch, e nenhuma SHA desse candidato foi encontrada. Alguns recibos não carregam payload/SHA suficiente para provar inexistência universal; zero candidatos não é refutação. Cada recibo está vinculado por SHA em `history.json`.

`calculate.py::transform_gameform_draw(method_source) -> str` é pura, independente de classe/assinatura, com três componentes explícitos. `patches.json` contém o método normal completo antes/depois e os bindings. Nenhuma classe gerada é escrita. As chamadas e strings existentes permanecem na mesma ordem; acrescenta-se apenas o destino inicial zero exigido pela cadeia original. Não há promessa de fechamento do alvo com base no toy ou na realocação abstrata.

Estado **READY_FOR_GAME_PROBE**, original2.703IL, potencial de ganho **UNKNOWN**. Somente o operador exclusivo poderá compilar, verificar assinatura/header/IL/EH e executar a verificação completa antes de adotar.
