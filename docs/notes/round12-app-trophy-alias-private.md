# DrawTrophyEffect — aliases de origem

Estado READY_FOR_GAME_PROBE; alvo e regressões oficiais UNKNOWN. Zero compilações do jogo, verificador oficial, alterações públicas ou de ledger. Esta frente cobre somente este método.

## Vínculo e medida

`main.AppData::DrawTrophyEffect(kairo.unity.ui.Graphics,System.Int32,System.Int32,System.Int32,System.Int32)->System.Void`, token0x060004ef. Base B13 em `build/round12/runs/b13/commit/postverify01/Assembly-CSharp`. Original506IL/20locais e atual497IL/18locais; ambos fat12/flags19/maxstack19/EH0. A primeira diferença bruta e a edição causal alinhada ocorrem no offset58, depois da chamada DrawSeb. Os destinos de desvio posteriores mudam por deslocamento, sem novo controle necessário.

REA primário `ev_da15547de98347a609c6e7fbe701373e7adc7496dcfa15cf1f1dcde01de5596b`, pai `ev_b095d7634e75e384655f2c593ab1b65437f5e1aa208418ab5af26fba0402d2cf`. Artefato Assembly-CSharp original SHA256 `3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949`, MVID `48ce9f89-73d2-40be-bb24-22c807c04106`. `bindings.json` registra hash de IL conferido contra os506bytes do dump, hashes do arquivo/manifest/receipt/dumps, CRLF e spansUTF16. O before foi localizado uma vez no Main observado. Após futuras mudanças em AppData.cs, rebase por corpo único e vínculo fresco são necessários.

## Fonte calculado

Após DrawSeb, o original emite `ldarg.s(px)@58; stloc0@60; ldarg.s(py)@61; stloc1@63`. Depois calcula o frame em slot2. O original usa `ldloc0@271` para a coordenada X e `ldloc1@286` para Y; o atual usa diretamente px@262 e py@278. A proposta introduz `int trophyOriginX=px; int trophyOriginY=py;` nesse ponto e substitui somente esses dois usos posteriores. Os argumentos px/py da chamada inicial DrawSeb permanecem diretos. Nomes dos aliases são reconstruídos; não se afirma que sejam os nomes históricos.

Os18locais existentes passam aos slots2–19, exatamente onde o original os usa. Seis bytes de cargas/armazenamentos novos, ajustes de opcodes compactos dos slots e duas cargas de argumento substituídas por cargas locais explicam o delta total+9IL. `slot-constraints.json` contém os20slots com tipos e primeiro/último uso. A relocação analítica coincide em todos273 objetos de instrução, inclusive operandos resolvidos já existentes, larguras e destinos, tamanho506. Isto não é emissão nem aprovação do alvo; nenhum IL é copiado para um resultado.

Aritmética inteira, divisões, ordem de chamadas, inicializadores das duas arrays constantes, clipping, loops, condições e ordem de avaliação foram mantidos. Não há inversão de operador, simplificação numérica ou mudança de dono de salto. O produtor limita-se às âncoras medidas e rejeita colisões/âncoras diferentes.

## Hipótese retirada e negativos preservados

R1 também retirava `int[] array=null` antes de GetSprite. Essa leitura foi REFUTADA pela releitura completa do original: `ldnull@72; stloc3@73` existem. A saída truncada da primeira inspeção havia ocultado esses dois comandos; a confirmação em toy, que dava504IL, não confirmava o original. Todos os arquivos e cinco compilações próprias R1 foram preservados em `rejected-r1-direct-init`; o cálculo R2 mantém integralmente a inicialização null. Nenhum candidato R1 foi compilado no jogo ou proposto ao executor como pronto.

R1 teve cinco fixtures próprias: before497/18, candidato504/20, controle504/20, aliases-only506/20 e direct-init-only495/18. Apenas os dois próprios504 coincidiam, demonstrando a forma construída, não a exigida pelo original.

R2 compilou somente dois novos casos próprios: candidato corrigido e controle manual independente, ambos506IL/20locais/fat12/flags19/maxstack19/EH0 e comparação integral de cabeçalho/locais/IL/EH vazia. O controle foi escrito com nomes distintos e preserva null explicitamente. Foram reutilizados como negativos autenticados os artefatos R1 before497, remove-null-after-aliases504 e direct-init-only495; todos diferem do controle R2. Total real desta frente: sete compilações próprias, zero de alvo. `fixture-report-r2.json` distingue os dois novos casos dos cinco anteriores. Não se recompilou negativo já preservado. Três negativos do produtor rejeitam colisão, GetSprite alterado e coordenada alterada.

## Modelo e compilador

NoEmit genuíno na base final B13: before complete=true/18tipos; candidate complete=true/20tipos e sequência original exata. O helper existente teve hash conferido antes da chamada. Não há previsão completa errada relaxada para incomplete. Um primeiro preparo de análise apontava B12; foi atualizado para B13 e os vínculos/NoEmit foram repetidos antes de qualquer entrega, sem emissão de alvo.

O mecanismo usa alocação normal de locais do smcs2.6.5. Fonte Mono fixado revisão `b6587926c0f146c8b12985744f33a13b39a22d57`, em `analysis/tools/round5-mono-source/mcs/mcs`: `statement.cs:2032–2047` (`Block.EmitMeta`) resolve locais nomeados antes de temporários e filhos; `expression.cs:4321–4355` emite a fonte e armazena o local (`Variable.EmitAssign`), com Dup somente quando leave_copy. Aqui cada alias é um comando simples e suas cargas posteriores referenciam os slots0/1; não há temporário de construção nem reuso hipotético. O modelo é confirmação independente de assinatura, não juiz do corpo.

## Histórico e entrega

Foram capturados88receipts deste método,12trials incluindo repetições. R4 tentou somente local-declaration-order-later e local-declaration-scope-hoist em declarações já existentes, sem adoção; os registros não trazem screen_prefix em parte dos casos e não permitem alegar emissão bem-sucedida de todos. Nenhum deles introduz os dois aliases de parâmetros. Nenhum hash da proposta inteira foi encontrado nos payloads capturados; ausência de payload/SHA não exclui universalmente uma forma histórica. Zero candidatos de outros ciclos não é refutação.

API pura `calculate.py::transform_trophy(method_source)->str`. `patches.json` contém o método normal completo before/after e vínculos; `ready-for-game-probe.json` aponta produtor, pacote e freeze. Os originais R1 e demais pacotes permanecem imutáveis. Nenhum ganho de método/byte é contado até o oráculo oficial completo.
