# RotateTestPlayParam — ordem de grupos e teste de intervalo

Entrega privada READY_FOR_GAME_PROBE. Resultado do alvo e regressões oficiais UNKNOWN. Zero compilações do jogo, verificadores oficiais, alterações públicas ou de ledger. Os pacotes anteriores continuam imutáveis; somente este método foi estudado.

## Vínculo e medida

`public void RotateTestPlayParam()`, ID `game.UserData::RotateTestPlayParam()->System.Void`, token0x0600041c. Base B13 em `build/round12/runs/b13/commit/postverify01/Assembly-CSharp`. Original272IL e atual286IL; ambos fat12/flags19/maxstack12, cinco locais idênticos e EH0.

REA primário `ev_9e456c0a35f27910a9c09923c3816741ec36f593b8a7bbc85331135f1f8a5d98`, pai `ev_0a92842d503efdedf04b18d2e0f16198ab062c3d5b2b91e2bd7791514ff7b119`. Artefato Assembly-CSharp original SHA256 `3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949`, MVID `48ce9f89-73d2-40be-bb24-22c807c04106`. Bindings guardam hash de IL conferido nos272bytes do dump, hashes de fonte/manifest/receipt/dumps, CRLF e spansUTF16. O before completo ocorre uma vez no Main observado; futuras mudanças em UserData.cs exigem rebase por corpo único e vínculo fresco.

A primeira diferença bruta está no byte3, destino do br@2, que depende do tamanho do corpo. A primeira edição causal alinhada é offset45: ldc.i4.1 original contra ldc.i4.4 atual. Não se trata de substituir um literal4 por1 no mesmo cálculo: os blocos de dispatch estão em outra ordem.

## Forma calculada

O primeiro grupo i==0 já coincide e continua presente com sua chamada e continue. No original, em44 começa o grupo sem folhas para i==1/2/3: beq46/53→65 e bne.un60→70.65 é br140.140 inicia o incremento do for (`ldloc0;ldc1;add;stloc0`), seguido da condição144; é o destino de continue, não saída do método.

Em70 vem o intervalo inclusivo: `i>=4 && i<=7`. IL72 blt106 e79 bgt106 são os dois ramos falsos para o teste posterior i==8. O corpo de pesquisa está em84–96 e seu br101→140 continua o for. Em106 vem o teste i==8 e a folha TeamData/warPotential, que cai normalmente no incremento140.

A fonte atual testa primeiro os quatro valores4/5/6/7 por OR, depois o grupo vazio1/2/3. A proposta reordena esses dois braços elseif e substitui o OR4/5/6/7 por `(i>=4)&&(i<=7)`. Não troca o significado dos constantes nas folhas: researchPoint_[i-4], índice0 da equipe e argumentos de AddTestPlayerParam continuam iguais. Mantém os continues originais, o teste i==8 separado e todos os dois laços posteriores de cópia/limpeza de tpplay_stock.

O índice i é Int32, com limites/declaração conferidos; não se aplica a floats, enums ou expressões com efeitos. Nenhuma chamada é adiantada/repetida e os braços1/2/3 não têm folhas. O produtor limita-se à cadeia textual exata e ao for Int32 medido. Não é uma reordenação por significado presumido do jogo.

O cálculo de ordem, opcodes e relocação coincide em todos130objetos de instrução originais,272IL, locais/EH iguais. A economia14bytes vem do teste de quatro igualdades substituído por duas comparações; só reordenar conserva286IL. Isto é diagnóstico calculado, não um build/verificação do alvo; nenhum IL original foi copiado para resultado.

## Emissão e modelo

Mono fixado revisão `b6587926c0f146c8b12985744f33a13b39a22d57`, `analysis/tools/round5-mono-source/mcs/mcs`:

- expression.cs3418–3442, Binary.EmitBranchable: igualdade com zero usa o caminho de teste simples; isso explica o grupo inicial i==0 sem materialização Boolean.
- expression.cs3450–3467: AND em contexto de ramo falso emite os dois testes falsos para o mesmo destino, como blt72/bgt79→106.
- expression.cs3472–3481: OR usa os ramos verdadeiros de esquerda e o falso de direita, compatíveis com o grupo1/2/3 e seu encontro65.
- statement.cs163–218 registra fluxo inalcançável dos braços que terminam em continue;225–267 emite braços na ordem do fonte sem br de else redundante quando esse fluxo não cai adiante.

NoEmit genuíno, helper já existente e hash conferido: before e candidato complete=true, sequência original exata `[Int32,data.TeamData,Int32,Int32,Int32]`. `slot-constraints.json` guarda usos e âncoras dos cinco slots. Não há local novo, temporário de switch ou relaxamento de previsão completa incorreta.

## Fixtures, negativos e histórico

Cinco compilações smcs2.6.5 de fixtures próprias, referências/flags fixadas, saída privada Fixture.dll e `-debug-`. Candidato e controle manual independente com variáveis renomeadas coincidem integralmente em cabeçalho/locais/IL/EH,272IL/maxstack12/cinco locais/EH0. Negativos preservados: before286 difere; somente ordem286 difere; somente intervalo272 também difere, apesar de tamanho correto. Três negativos do produtor rejeitam mudança do tipo do índice, da folha researchPoint e do continue. Nenhum alvo foi compilado.

Capturados91receipts e sete trials incluindo repetições. R4 tentou loop-continue-guard, hoist, switch-if, tail-early-return e condition-local; R6 repetiu switch-if, todos sem avanço do prefixo3. R7ParteA registrou switch-if aceito com prefixo3→3; esse payload corresponde à ordem atual0,4/5/6/7,1/2/3 e não fecha o corpo. Essa aceitação histórica não é contada como avanço ou aprovação atual. A proposta nova corrige a ordem para0,1/2/3,intervalo4–7 e conserva os destinos de continue; não repete o payload switch-if anterior. Nenhum hash da proposta inteira apareceu nos registros; receipts sem payload/SHA não excluem universalmente toda forma histórica. Zero candidatos não é refutação.

## Entrega

API pura `calculate.py::transform_rotate(method_source)->str`; os parâmetros opcionais order/interval servem apenas aos controles negativos próprios. patches.json contém before/after completo e spansUTF16. ready-for-game-probe.json aponta produtor, pacote e freeze. A adoção é reservada ao executor, após rebase fresco e oráculo completo com zero regressões. Nenhum método/byte foi contado como ganho nesta entrega.
