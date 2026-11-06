# Rodada 9: candidato conjunto de fluxo B e locais nomeados smcs

**READY para o oráculo; corpos do jogo UNKNOWN.** Uma família, dois métodos, sem novos alvos: `d9-joint-flow-local-stores`. A preparação não compilou o jogo, não mudou o ledger e não alterou fontes oficiais, modelos ou referências. Corpus congelado: `build/round9/cycles/b10/after/Assembly-CSharp`.

As propostas B06/B07 isoladas foram inconclusivas porque a previsão completa de locais divergia antes de qualquer compilação. Este candidato reúne a forma de fluxo já testada em B e a correção de locais sustentada pelo IL original. Não é repetição da mesma variante isolada, nem promoção da forma de fluxo B a fato histórico.

| Método | IL original | Previsão atual | Previsão conjunta |
|---|---|---|---|
| `form.GameForm::_update(System.Int32)->System.Boolean` | 1.687 bytes; 14 locais | 13 locais | completa, 14 tipos exatamente iguais |
| `form.SubForm::Init()->System.Boolean` | 5.191 bytes; 65 locais | 64 locais | completa, 65 tipos exatamente iguais |

Total exposto ao oráculo: **2 métodos / 6.878 bytes de IL original**. Isso não é estimativa de ganho confirmado.

## GameForm

Mantém a divisão do guard do evento em um guard de `continue` e um if positivo para executar o evento e retornar. Acrescenta somente o local Int32 do único `GetMatchInfoJoin` dentro de `_update`, imediatamente após `SetBattleData` e antes de seu teste.

```
1408 ldfld form.GameForm::udat_:game.UserData
1413 callvirt game.UserData::GetMatchInfoJoin()->System.Int32
1418 stloc.s 12
1420 ldloc.s 12
1422 brtrue 1467
```

O cálculo source-only insere `int testedMatchInfoJoin = udat_.GetMatchInfoJoin();` nesse bloco. O modelo fixa esse símbolo no slot12 e o local `add` seguinte no slot13, como o original. A previsão anterior colocava `add` no slot12 e não tinha o local testado. A expressão `testedMatchInfoJoin == 0 && !apdat_.CheckAlreadyEvent(176)` preserva avaliação e ordem da chamada; não repete `GetMatchInfoJoin`.

O rótulo L7 examinado em B era o incremento do for em IL304; R265 era um salto intermediário para ele. A forma de fonte não transforma esse destino em break ou em retorno do método.

## SubForm

Mantém o braço vazio de `type_ == 6` seguido do bloco else existente. Converte exclusivamente os dois switches de `udat_.GetMatchInfoJoin()` dentro de `Init` em if-chains com seletores Int32 nomeados separados. Todos os braços selecionados já terminavam em return. Os blocos, declarações dos cases, expressões e valores retornados são conservados; não há reescrita de break/goto.

```
1617 callvirt game.UserData::GetMatchInfoJoin()->System.Int32
1622 stloc.s 12
1624 ldloc.s 12
1626 ldc.i4.1
1627 bne.un 1686

2352 callvirt game.UserData::GetMatchInfoJoin()->System.Int32
2357 stloc.s 20
2359 ldloc.s 20
2361 brtrue 2463
```

`testedMatchInfoJoin1` fica no slot12; `testedMatchInfoJoin2`, no slot20. O switch recuperado registrava um temporário Int32 reutilizado pelo compilador, em vez de dois locais nomeados nos dois blocos originais. Os nomes são escolhas de reconstrução e não nomes históricos recuperados do assembly.

Evidência primária da emissão: `analysis/tools/round5-mono-source/pre-release/mcs/mcs/statement.cs:3830–3866`, `Switch.DoEmit`, registra `LocalTemporary`, guarda o seletor e depois o libera. O snapshot Mono fixado é `b6587926c0f146c8b12985744f33a13b39a22d57`; esse arquivo tem SHA-256 `063a437095e1d608d045383d76291c9359e9a98220eea7750018a3d42712437a`. A correção usa variáveis nomeadas e ifs para que os dois valores sejam armazenados nos dois slots originais.

O rótulo L97 continua sendo a folha existente `5189 ldc.i4.1; 5190 ret`. Isso não identifica, sozinho, a construção histórica de C# responsável por alcançar essa folha. Não se inventou return adicional.

## Controle histórico e limites

`tools/round9_joint_flow_local_fixtures.py` compila somente uma fixture independente com o smcs selecionado, SHA-256 `26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0`. Os dois corpos conjuntos são integralmente iguais aos controles independentes, incluindo cabeçalhos, locais e regiões de exceção, excluindo somente o nome de declaração do controle. Os mesmos métodos com apenas o fluxo B continuam diferentes dos controles. Duas variantes negativas com break no primeiro ou segundo switch são recusadas atomicamente: não deixam uma conversão parcial do outro switch na lista de patches.

Cada reprodução usa três compilações históricas toy, zero jogo. A preparação executou os controles três vezes, incluindo a proteção atômica e a publicação dos imports: nove compilações históricas toy ao todo. As previsões NoEmit foram feitas sobre todos os sources congelados da montagem, com os arquivos físicos do snapshot como caminhos lógicos; o candidato é passado explicitamente ao modelo. Os vetores completos de 14 e65 tipos são comparados ao original, não usados como resposta do modelo.

## Arquivos e reprodução

```
python -X utf8 tools/round9_joint_flow_local_fixtures.py
python -X utf8 tools/round9_joint_flow_local_calculate.py
```

O segundo comando faz apenas parsing/vinculação e previsão; escreve `build/round9/part-d-joint/patches-and-proof.json`. A cópia congelada é `reports/round9-d-joint-flow-local-patches.json`, com duas propostas de método completo, spans UTF-16, raw before/after, hashes das dependências e dos modelos, SHA do recibo B10 e do bundle B reutilizado. O root integra o bundle por `--external-proposals`; não é necessário integrar o helper ao dispatcher ativo para testar esses dois candidatos.

`tools/Round9JointNamedSelectorRules.cs`, entry `AddRound9JointNamedSelectorRules`, é um helper opt-in de cálculo estreito: exige exatamente dois switches de `udat_.GetMatchInfoJoin()` no método, seletor Int32, labels constantes, braços terminando em return e ausência de break/goto. O helper calcula apenas a parte dos seletores de SubForm. O calculator compõe-a com os patches B verificados como únicos e com o alias de GameForm. Não é um extrator genérico C e não altera seus contratos.

A assinatura prevista correta libera uma tentativa; não comprova o corpo. Ambos os corpos e qualquer ganho de métodos/bytes continuam UNKNOWN até a compilação e verificação completas oficiais. Os modelos e arquivos da rodada 8 permanecem imutáveis.
