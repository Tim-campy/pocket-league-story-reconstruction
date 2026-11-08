# SoccerForm.Draw_topBar — argumento final

Estado privado READY_FOR_GAME_PROBE. Resultado do jogo e regressões oficiais UNKNOWN. Zero compilações do alvo, verificador oficial, mudanças públicas ou de ledger. Todos os pacotes anteriores permanecem congelados.

## Overload e identidade

Declaração exata `private void Draw_topBar(Graphics g)`, ID `form.SoccerForm::Draw_topBar(kairo.unity.ui.Graphics)->System.Void`, token0x060001d5. Não é Draw_bonusIncome nem UpdateOffscreenTopBar. Base B13 `build/round12/runs/b13/commit/postverify01/Assembly-CSharp`; o método foi extraído por sua declaração, não por uma chamada anterior do mesmo nome.

Original432IL e reconstruído433IL; ambos fat12, flags19, maxstack35, seis locais idênticos, EH0. A única edição alinhada está no offset424: `ldarg.1` original contra `ldloc.s5` reconstruído. Original426 chama `UpdateOffscreenTopBar(Graphics,Boolean)` depois de ldc.i4.0@425 e retorna431. Atual desloca o ldc para426, call427 e ret432. Todos os150objetos de instrução coincidem após essa substituição e relocação−1; esse cálculo não é emissão/aprovação do alvo.

REA primário `ev_546fa4229dac8496ffad66512a8f2c6d893622973e01ca7de340fee019d3824c`, pai `ev_630f98352807fac305364880ca215fc4c5b94a049f8f76f6d69e71cfb2f83414`. Artefato original SHA256 `3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949`, MVID `48ce9f89-73d2-40be-bb24-22c807c04106`; `bindings.json` contém hash dos432bytes de IL conferido no dump, hashes de fonte/manifest/receipt/dumps e spansUTF16. O before foi localizado uma vez no Main observado; após outras mudanças em SoccerForm.cs, exige rebase por corpo único e vínculo fresco.

## Origem do local e proposta

O slot5 Graphics vem de `ldarg.1@81; stloc.s5@82`. Esse alias já existe tanto no original quanto na reconstrução e precisa permanecer: os onze receivers de SetColor/DrawString usam o slot5. A fonte atual contém `global::kairo.unity.ui.Graphics gAlias=g;`; não há nova atribuição, ref/out ou reatribuição do parâmetro. Os cinco outros locais (TeamData,TeamData,Int32,Int32,Int32) também estão corretos.

A proposta altera somente `UpdateOffscreenTopBar(gAlias, false);` para `UpdateOffscreenTopBar(g, false);`. Não remove o alias, não troca os onze receivers, não move comandos e não altera o Boolean. Uma carga do argumento é1byte, contra2bytes do ldloc.s5; isso explica a diferença inteira de tamanho, sem mudança de assinatura ou cabeçalho além de code_size. As avaliações/calls/folhas e a aritmética permanecem na mesma ordem.

Esse limite estreito corrige o excesso de uma transformação antiga, sem desfazer a parte aprovada. O produtor é literal e verifica as duas âncoras, escrita única do alias, ausência de ref/out e reatribuição de g. Não é uma regra genérica de substituir locais por parâmetros quando seu valor pode mudar.

## Modelo e fixtures

NoEmit genuíno: before e candidato complete=true, com os mesmos seis tipos originais. Helper existente/hash conferido; não houve relaxamento de previsão completa incorreta. A declaração e a alocação do alias não mudam. A regra normal do smcs está em Mono fixado revisão `b6587926c0f146c8b12985744f33a13b39a22d57`: `statement.cs:2032–2047` aloca os locais nomeados; `expression.cs:4321–4355` trata sua atribuição. A evidência do argumento final é o próprio ldarg.1 original, não uma inferência de otimização do compilador.

Quatro compilações de fixtures próprias smcs2.6.5 com referências/flags fixadas, saída privada Fixture.dll e `-debug-`. Candidato e controle manual independente com variáveis renomeadas coincidem integralmente em cabeçalho, locais, IL e EH,432IL/seis locais/fat12/flags19/maxstack35/EH0. Before433 difere; controle com Boolean true tem432 mas difere no IL. O tipo Graphics da fixture é stub próprio no namespace correspondente; a comparação não é um teste do jogo. Logs, hashes e diferenças completas estão em fixture-report.json.

Três negativos do produtor rejeitam nova escrita do alias, passagem ref e mudança de overload para argumento inteiro; nenhum deles foi compilado. A API pura não faz IO nem emite código.

## Histórico e entrega

Foram capturados89receipts e sete trials incluindo repetições. R4 tentou duas polaridades e dois condition-local, todos sem avanço81. R6 adotou `alias-parameter-block-tail`,81→424; essa adoção criou gAlias para todo o restante do bloco, alcançando indevidamente a última chamada. R6 e R7ParteA tentaram agrupamento de declarações,424→424. A proposta atual preserva o alias aprovado nos receivers e restaura o argumento somente na última chamada, sem repetir as regras de condições/agrupamento. Nenhum hash da proposta inteira apareceu nos receipts capturados; registros sem payload/SHA não excluem universalmente tentativas antigas. Zero candidatos não é refutação.

API `calculate.py::transform_topbar(method_source)->str`; patches.json guarda método completo before/after, spansUTF16 e hashes. O ready-for-game-probe.json aponta produtor, pacote e freeze. Nenhum método/byte foi contado como ganho nesta entrega; somente o oráculo oficial completo pode adotar o resultado.
